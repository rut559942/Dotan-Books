using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Service.Caching
{
    public static class DistributedCacheExtensions
    {
        //פונקציית המרה לJSON 
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
        //חילוץ נתונים מרדיס
        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string key)
        {
            var json = await cache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, SerializerOptions);
        }

        //שמירת נתונים ברדיס
        public static async Task SetRecordAsync<T>(
            this IDistributedCache cache,
            string key,
            T data,
            TimeSpan ttl)
        {
            var json = JsonSerializer.Serialize(data, SerializerOptions);
            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });
        }
    }
}