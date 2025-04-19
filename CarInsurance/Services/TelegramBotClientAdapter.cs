using CarInsurance.Entities.Dto;
using CarInsurance.Repositories;
using Microsoft.Extensions.Options;
using Mindee.Input;
using QuestPDF.Fluent;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using CarInsurance.Options;
using User = CarInsurance.Entities.User;
namespace CarInsurance.Services
{
    public class TelegramBotClientAdapter(IOptions<ConnectionOptions> options,
    IInsuranceBotService botService) : IBotClient, IHostedService
    {
        private readonly TelegramBotClient _botClient = new (options.Value.TelegramAPI);
        private readonly IInsuranceBotService _botService = botService;

        public ITelegramBotClient Client => throw new NotImplementedException();

        public async Task StartReceivingAsync(CancellationToken cancellationToken)
        {
            var receiverOptions = new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationToken);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message is not { } message) return;

            try
            {
                var userMessage = await ConvertToUserMessageAsync(message, ct);
                userMessage.UserId = message.From.Id;
                await _botService.HandleUserMessageAsync(userMessage, this, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex}");
                await SendMessageAsync(message.Chat.Id, "Сталася помилка при обробці. Спробуйте ще раз.", ct);
            }
        }

        private async Task<UserMessageDto> ConvertToUserMessageAsync(Message message, CancellationToken ct)
        {
            var photos = new List<LocalInputSource>();
            if (message.Photo != null)
            {
                var fileId = message.Photo!.Last().FileId;
                var fileInfo = await _botClient.GetFile(fileId, ct);
                var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
                // Download and save the file with proper extension
                await using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await _botClient.DownloadFile(fileInfo.FilePath!, fileStream, ct);
                }
                photos.Add(new LocalInputSource(tempFilePath));

            }
            return new UserMessageDto()
            {
                ChatId = message.Chat.Id,
                Text = message.Text,
                Photos = photos
            };
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiEx => $"Telegram API Error: {apiEx.ErrorCode} - {apiEx.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
        public async Task SendMessageAsync(long chatId, string text, CancellationToken ct)
        {
            await _botClient.SendMessage(chatId, text, cancellationToken: ct);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return StartReceivingAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SendDocumentAsync(ITelegramBotClient botClient, UserMessageDto message, User user)
        {
            var doc = new InsuranceDocument(user);

            // Зберігаємо у MemoryStream
            using var stream = new MemoryStream();
            doc.GeneratePdf(stream);
            stream.Position = 0;

            var inputFile = InputFile.FromStream(stream, "insurance_policy.pdf");

            await botClient.SendDocument(
                chatId: message.ChatId,
                document: inputFile,
                caption: "Ось ваш страховий поліс 📄"
            );
        }
    }
}
