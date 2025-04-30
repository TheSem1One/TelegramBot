using System.Text.Json;
using CarInsurance.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace CarInsurance.Services
{
    public class RedisSessionService(IDistributedCache cache) : IUserSessionService
    {
        private readonly IDistributedCache _cache = cache;
        private const string settingBot =
            "Ти дружній страхувальний бот, який може допомогти застрахувати авто. " +
            "Не забувай, що все відбувається в телеграмі." +
            "Для того щоб отримати стахування потрібно буде спочатку додати паспорт (id картка) " +
            "а потім технічний паспорт і після цього за допомогою команди /buyinsurace отримати страхування";

        public async Task<List<string>> GetOrCreate(long chatId,string message)
        {
            var stringConnection = await _cache.GetAsync(chatId.ToString());
            if (stringConnection is null)
            {
                var list = new List<string>
                {
                    settingBot,message
                };
                var messages = JsonSerializer.Serialize(list); 
                await _cache.SetStringAsync(chatId.ToString(), messages);
            }
            var connection = JsonSerializer.Deserialize<List<string>>(stringConnection);
            connection.Add(message);
            Save(chatId, message);
            return connection;

        }

        public async Task Save(long chatId, string message)
        {
            var stringConnection = await _cache.GetAsync(chatId.ToString());
            var connection = JsonSerializer.Deserialize<List<string>>(stringConnection);
            connection.Add(message);
            var messages = JsonSerializer.Serialize(connection);
            await _cache.SetStringAsync(chatId.ToString(), messages);
        }


    }
}