using Microsoft.Extensions.FileSystemGlobbing.Internal;
using StackExchange.Redis;

namespace BeautyStore.Services
{
    public interface IRedisService
    {
        Task SetStringAsync(string key, string value, int db = 0, TimeSpan? expiry = null);
        Task<string?> GetStringAsync(string key, int db = 0);
        Task<List<KeyValuePair<string, string>>> GetKeyValueAsync(int db = 0, string prefix = "");
        Task<bool> CheckKeyAsync(string key, int db = 0);
        Task<bool> DeleteByKeyAsync(string key, int db = 0);
    }

    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IConfiguration _config;

        public RedisService(IConnectionMultiplexer redis, IConfiguration config)
        {
            _redis = redis;
            _config = config;
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

        public async Task<List<KeyValuePair<string, string>>> GetKeyValueAsync(int db = 0, string prefix = "")
        {
            var host = _config["Redis:Host"];
            var port = int.Parse(_config["Redis:Port"] ?? "6379");

            var server = _redis.GetServer(host, port);
            var database = _redis.GetDatabase(db);

            var keys = string.IsNullOrEmpty(prefix) 
                ? server.Keys(db) 
                : server.Keys(db, $"{prefix}*");

            var result = new List<KeyValuePair<string, string>>();
            foreach (var key in keys)
            {
                var value = await database.StringGetAsync(key);
                if (!value.IsNullOrEmpty)
                {
                    result.Add(new KeyValuePair<string, string>(key, value));
                }
            }

            return result;
        }

        public async Task<bool> CheckKeyAsync(string key, int db = 0)
        {
            var database = _redis.GetDatabase(db);
            return await database.KeyExistsAsync(key);
        }

        public async Task<bool> DeleteByKeyAsync(string key, int db = 0)
        {
            var database = _redis.GetDatabase(db);
            return await database.KeyDeleteAsync(key);
        }


    }

}
