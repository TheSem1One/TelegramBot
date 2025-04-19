using CarInsurance.Entities;
using CarInsurance.Entities.Dto;
using Telegram.Bot;

namespace CarInsurance.Repositories
{
    public interface IBotClient
    {
        ITelegramBotClient Client { get; }
        Task SendMessageAsync(long chatId, string text, CancellationToken ct);
        Task StartReceivingAsync(CancellationToken cancellationToken);
        Task SendDocumentAsync(ITelegramBotClient botClient, UserMessageDto message, User user);
    }
}
