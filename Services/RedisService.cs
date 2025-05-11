using StackExchange.Redis;

namespace BeautyStore.Services
{
    public interface IRedisService
    {
        Task SetStringAsync(string key, string value, int db = 0, TimeSpan? expiry = null);
        Task<string?> GetStringAsync(string key, int db = 0);
    }

    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task SetStringAsync(string key, string value, int db = 0, TimeSpan? expiry = null)
        {
            var redisDb = _redis.GetDatabase(db);
            await redisDb.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetStringAsync(string key, int db = 0)
        {
            var redisDb = _redis.GetDatabase(db);
            return await redisDb.StringGetAsync(key);
        }
    }

}
