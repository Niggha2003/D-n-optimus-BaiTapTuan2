using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BaiTap2.Services
{
    public class RedisCacheService
    {

        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
            _redis = redis;
        }

        public async Task<bool> SetAsync<T>(string key, T? value, TimeSpan? expiry = null)
        {
            try
            {
                string valueString = JsonConvert.SerializeObject(value);

                await _database.StringSetAsync(key, valueString, expiry);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task<List<T>?> GetListData<T>(string key)
        {
            List<T>? resultObject = null;

            string? listRedisCacheString = await GetAsync(key);

            if (listRedisCacheString != null && listRedisCacheString != "")
            {
                resultObject = JsonConvert.DeserializeObject<List<T>>(listRedisCacheString);
            }

            return resultObject;
        }

        public async Task<T?> GetData<T>(string key)
        {
            T? resultObject = default(T);

            string? listRedisCacheString = await GetAsync(key);

            if (listRedisCacheString != null && listRedisCacheString != "")
            {
                resultObject = JsonConvert.DeserializeObject<T>(listRedisCacheString);
            }

            return resultObject;
        }

        // Xóa tất cả cache trong redis database
        public void ClearCache()
        {
            var endpoints = _redis.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                foreach (var key in server.Keys())
                {
                    _database.KeyDelete(key);
                }
            }
        }

        public async Task<bool> DeleteCache(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
    }
}
