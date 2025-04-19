using Mindee.Input;

namespace TelegramBot.Domain.Repositories
{
    public interface IImageProcessingRepository
    {
        Task<string> ProcessIdCardAsync(LocalInputSource local, CancellationToken ct,long userId);
        Task<string> ProcessTechPassportAsync(LocalInputSource local, CancellationToken ct, long userId);
    }
}
