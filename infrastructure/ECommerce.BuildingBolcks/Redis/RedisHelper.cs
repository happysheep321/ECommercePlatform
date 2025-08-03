using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.BuildingBlocks.Redis
{
    public class RedisHelper:IRedisHelper
    {
        private readonly IDatabase database;

        public RedisHelper(IConnectionMultiplexer connectionMultiplexer)
        {
            database = connectionMultiplexer.GetDatabase();
        }

        public async Task SetAsync<T>(string key,T value,TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await database.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await database.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(json!);
        }

        public async Task DeleteAsync(string key)
        {
            await database.KeyDeleteAsync(key);
        }
    }
}
