using System;
using System.Web.Caching;

namespace WebCache
{
    public sealed class CacheWeb : CacheManager<Cache>
    {
        public volatile static CacheWeb Cache;
        
        static CacheWeb() //Can not create instance since it has Static constructor.
        {
            if (Cache == null)
            {
                lock (lockobj)
                {
                    if (Cache == null) Cache = new CacheWeb();
                }
            }
        }

        private CacheWeb() : base() { }

      
        //Private methods
        protected override T Set<T>(string key, T value, Nullable<TimeSpan> time = null)
        {            
            Storage.Insert(key, value, null, DateTime.Now.AddDays(1), time ?? new TimeSpan(1,0,0)); //Caching temporary for an hour and/or a Day.

            return value;
        }

        protected override T Fetch<T>(string key)
        {
            return  Exists(key) ? (T)Storage[key] : default(T);
        }

        protected override bool Exists(string key)
        {
            return Storage[key] != null;
        }
    }
}
