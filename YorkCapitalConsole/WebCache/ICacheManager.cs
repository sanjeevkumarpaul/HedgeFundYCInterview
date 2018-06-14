using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCache
{
    public interface ICacheManagerFactory
    {
        T Add<T>(string key, T item);
        T Get<T>(string key);
        T Remove<T>(string key);
        void RemoveAll();
    }

    public interface ICacheManager<CacheType> : ICacheManagerFactory
    {
        
    }
}
