using CarInsurance.Entities.Dto;

namespace CarInsurance.Repositories
{
    public interface IInsuranceBotService
    {
        Task HandleUserMessageAsync(UserMessageDto message, IBotClient botClient, CancellationToken ct);
    }
}
