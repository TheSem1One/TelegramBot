using CarInsurance.Entities;

namespace CarInsurance.Repositories
{
    public interface IUserSessionService
    {
        Task<List<string>> GetOrCreate(long chatId,string message);
        Task Save(long chatId, string session);

    }
}
