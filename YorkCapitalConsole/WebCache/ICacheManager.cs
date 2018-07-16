using System;

namespace WebCache
{
    public interface ICacheManagerFactory
    {
        T Add<T>(string key, T item, Nullable<TimeSpan> time = null);
        T Update<T>(string key, Func<T> action, Nullable<TimeSpan> time = null);
        T Get<T>(string key);
        T Remove<T>(string key);
        void RemoveAll();
    }

    public interface ICacheManager<CacheType> : ICacheManagerFactory
    {
        
    }
}
