using CarInsurance.Entities;
using CarInsurance.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CarInsurance.Persistence
{
    public class UserContext : DbContext
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
