using CarInsurance.Entities;
using MongoDB.Driver;

namespace CarInsurance.Persistence
{
    public interface DbContext
    {
        IMongoCollection<User> Users { get; set; }
    }
}