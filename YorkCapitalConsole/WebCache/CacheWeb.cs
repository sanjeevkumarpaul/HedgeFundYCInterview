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

        public override void RemoveAll()
        {
            foreach (var item in Storage)
            {
                Storage.Remove(((System.Collections.DictionaryEntry)item).Key.ToString());
            }
        }

        //Private methods
        protected override T Set<T>(string key, T value, Nullable<TimeSpan> time = null)
        {
            Delete<T>(key);  //Always delete it before inserting it.
            if (time.HasValue)
              Storage.Insert(key, value, null, DateTime.UtcNow.AddTicks(time.Value.Ticks), TimeSpan.Zero); 
            else
               Storage.Insert(key, value, null, DateTime.UtcNow.AddHours(1), TimeSpan.Zero); //Caching temporary for an hour

            return value;
        }

        protected override T Delete<T>(string key)
        {
            var _val = Fetch<T>(key);
            if (_val != null) Storage.Remove(key);

            return _val;
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
