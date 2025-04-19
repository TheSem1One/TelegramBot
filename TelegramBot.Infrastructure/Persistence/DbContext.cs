using MongoDB.Driver;
using TelegramBot.Domain.Entity;

namespace TelegramBot.Infrastructure.Persistence
{
    public interface DbContext
    {
        IMongoCollection<User> Users { get; set; }
    }
}
