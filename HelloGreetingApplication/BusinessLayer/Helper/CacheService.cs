using System;
using StackExchange.Redis;
using System.Text.Json;
namespace BusinessLayer.Helper
{
	public class CacheService
	{
		private readonly IDatabase _cache;
		private readonly TimeSpan _cacheExpiry;
		public CacheService(string redisConnectionString , int cacheExpiryMinutes) 
		{
			var redis = ConnectionMultiplexer.Connect(redisConnectionString);
			_cache = redis.GetDatabase();
			_cacheExpiry = TimeSpan.FromMinutes(cacheExpiryMinutes);
		}
		public async Task SetCacheAsync<T>(string key, T data)
		{
			if (data == null) return;
			string jsonData = JsonSerializer.Serialize(data);
			await _cache.StringSetAsync(key, jsonData, _cacheExpiry);
		}
        public async Task<T> GetCacheAsync<T>(string key)
        {
            string jsonData = await _cache.StringGetAsync(key);
            return jsonData is not null ? JsonSerializer.Deserialize<T>(jsonData) : default;
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }
    }
}

