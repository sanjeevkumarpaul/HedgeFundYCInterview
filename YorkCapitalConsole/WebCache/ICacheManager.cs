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
    }

    public interface ICacheManager<CacheType> : ICacheManagerFactory
    {
        
    }
}
