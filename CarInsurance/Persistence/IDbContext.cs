using CarInsurance.Entities;
using MongoDB.Driver;

namespace CarInsurance.Persistence
{
    public interface IDbContext
    {
        IMongoCollection<User> Users { get; set; }
    }
}