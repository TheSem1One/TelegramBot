using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TelegramBot.Domain.Entity;
using TelegramBot.Domain.Options;

namespace TelegramBot.Infrastructure.Persistence
{
    public class UserContext :DbContext
    {
        public IMongoCollection<User> Users { get; set; }

        public UserContext(IOptions<ConnectionOptions> options)
        {
            var client = new MongoClient(options.Value.DbConnection);
            var database = client.GetDatabase(options.Value.DbName);
            Users = database.GetCollection<User>(options.Value.DbCollection);

        }
    }
}
