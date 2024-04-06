using System;
using System.Runtime.Caching;

namespace KPILib
{
    public static class CacheManager
    {
        private static MemoryCache cache;
        private static int jwtTokenExpiryTime;
        public static void CreatCacheInstance(int CacheExpiry)
        {
            cache = new MemoryCache("KPI");
            jwtTokenExpiryTime = CacheExpiry;
        }

        public static void AddCache(string key, object data, int ExpirationTime = 0)
        {
            ExpirationTime = ExpirationTime == 0 ? jwtTokenExpiryTime : ExpirationTime;

            CacheItemPolicy policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(ExpirationTime)
            };

            cache.Add(key, data, policy);
        }

        public static object GetCache(string key)
        {
            return cache.Get(key); 
        }

        public static object Remove(string key)
        {
            return cache.Remove(key);
        }
    }
}
