using System;
using System.Collections;
using System.Timers;

namespace WebCache
{
    public sealed class CacheHashtable : CacheManager<Hashtable>
    {
        public volatile static CacheHashtable Cache;
        
        static CacheHashtable() //Can not create instance since it has Static constructor.
        {
            if (Cache == null)
            {
                lock (lockobj)
                {
                    if (Cache == null) Cache = new CacheHashtable();
                }
            }
        }

        private CacheHashtable() : base() { }

        public override void RemoveAll()
        {
            Storage.Clear();
        }

        //Private methods
        protected override T Set<T>(string key, T value, Nullable<TimeSpan> time = null)
        {
            //Always delete it before inserting it.
            Delete<T>(key);
            //Caching temporary for an hour
            Storage.Add(key, value); //null, time.HasValue ? DateTime.UtcNow.AddTicks(time.Value.Ticks) : DateTime.UtcNow.AddHours(1), TimeSpan.Zero); 

            //A timer to handle expiration.
            if (time.HasValue && time.Value != TimeSpan.Zero )
            {
                var timer = new Timer(time.Value.TotalMilliseconds);
                timer.Elapsed += (s, o) => { Delete<T>(key); timer.Stop(); timer.Dispose(); };
                timer.Start();
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
            if (_val != null) Storage.Remove(key);

            return _val;
        }
              

        protected override T Fetch<T>(string key)
        {
            return  Exists(key) ? (T)Storage[key] : default(T);
        }

        protected override bool Exists(string key)
        {
            return Storage.ContainsKey(key);
        }
    }
}
