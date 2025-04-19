using Telegram.Bot;
using TelegramBot.Domain.Entity.Dto;
using User = TelegramBot.Domain.Entity.User;

namespace TelegramBot.Domain.Repositories
{
    public interface IBotClient
    {
        ITelegramBotClient Client { get; }
        Task SendMessageAsync(long chatId, string text, CancellationToken ct);
        Task StartReceivingAsync(CancellationToken cancellationToken);
        Task SendDocumentAsync(ITelegramBotClient botClient, UserMessageDto message, User user);
    }
}
