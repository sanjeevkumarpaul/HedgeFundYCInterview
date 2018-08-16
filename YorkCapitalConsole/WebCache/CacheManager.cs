using System;
namespace WebCache
{
    public abstract class CacheManager<CacheType> : ICacheManager<CacheType> where CacheType : class
    {
        protected CacheType Storage = null;
        protected static readonly object lockobj = new object();

        /// <summary>
        /// Making sure that Chache object is retrieved.
        /// If System.Web.Caching.Cache is used as cache object take theone which ASP.net Ha
        /// </summary>
        internal CacheManager()
        {
            if (typeof(CacheType).Equals(typeof(System.Web.Caching.Cache)))
                if (System.Web.HttpContext.Current != null)
                    Storage = (System.Web.HttpContext.Current.Cache as CacheType );                
            else
                Storage = Activator.CreateInstance<CacheType>();
        }

        public T Add<T>(string key, T item, Nullable<TimeSpan> time = null)
        {
            return Set(key, item, time);
        }

        public T Update<T>(string key, Func<T> action, Nullable<TimeSpan> time = null)
        {
            return Change(key, action, time);
        }

        public T Get<T>(string key) => Fetch<T>(key);

        public T Remove<T>(string key) => Delete<T>(key);

        public abstract void RemoveAll();
        
        protected abstract T Set<T>(string key, T item, Nullable<TimeSpan> time = null);
        protected abstract T Change<T>(string key, Func<T> action, Nullable<TimeSpan> time = null);
        protected abstract T Fetch<T>(string key);
        protected abstract bool Exists(string key);
        protected abstract T Delete<T>(string key);
    }
}
