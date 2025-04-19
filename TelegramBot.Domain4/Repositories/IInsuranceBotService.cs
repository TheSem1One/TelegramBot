using TelegramBot.Domain.Entity.Dto;

namespace TelegramBot.Domain.Repositories
{
    public interface IInsuranceBotService
    {
        Task HandleUserMessageAsync(UserMessageDto message, IBotClient botClient, CancellationToken ct);
    }
}
