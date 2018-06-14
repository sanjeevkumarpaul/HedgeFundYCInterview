using System;
using System.Runtime.Caching;

namespace WebCache
{
    public sealed class CacheMemory : CacheManager<object>
    {
        public volatile static CacheMemory Cache;
        
        static CacheMemory() //Can not create instance since it has Static constructor.
        {
            if (Cache == null)
            {
                lock (lockobj)
                {
                    if (Cache == null) Cache = new CacheMemory();
                }
            }
        }

        private CacheMemory() : base() { }

        public override void RemoveAll()
        {
            MemoryCache.Default.Trim(100);
        }

        //Private methods
        protected override T Set<T>(string key, T value, Nullable<TimeSpan> time = null)
        {
            Delete<T>(key);  //Always delete it before inserting it.
            if (value != null)
            {                
                var policy = GetCachePolicy(key, time);
                MemoryCache.Default.Add(key, value, policy, null);
            }

            return value;
        }

        protected override T Change<T>(string key, Func<T> action, Nullable<TimeSpan> time = null)
        {
            var _value = action();
            return Set<T>(key, _value, time);
        }

        protected override T Delete<T>(string key)
        {
            var _val = Fetch<T>(key);
            if (_val != null)
            {
                //Storage.Remove(key);
                MemoryCache.Default.Remove(key);
                MemoryCache.Default.Remove(GetAbsoluteKey(key));
            }

            return _val;
        }
              

        protected override T Fetch<T>(string key)
        {
            return  Exists(key) ? (T)MemoryCache.Default.GetCacheItem(key).Value : default(T);
        }

        protected override bool Exists(string key)
        {
            try
            {
                //return MemoryCache.Default.GetCacheItem(key) != null;
                return MemoryCache.Default.Contains(key);
            }
            catch
            {
                return false;
            }
        }

        //Private Methods...
        private CacheItemPolicy GetCachePolicy(string key, Nullable<TimeSpan> time = null)
        {
            key = GetAbsoluteKey(key);
            string[] absKey = new string[] { key };

            MemoryCache.Default.Add(key,
                                    new object(),
                                    time.HasValue ? DateTime.UtcNow.AddTicks(time.Value.Ticks) : DateTime.UtcNow.AddHours(1) //default it to only 1 hour.
                                    );

            CacheEntryChangeMonitor monitor = MemoryCache.Default.CreateCacheEntryChangeMonitor(absKey);

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.ChangeMonitors.Add(monitor);
            policy.SlidingExpiration = !time.HasValue ? new TimeSpan(1, 0, 0) : time.Value; //default it to only 1 hour

            return policy;
        }

        private string GetAbsoluteKey(string name)
        {
            return $"Absolute~${name}";
        }
    }
}