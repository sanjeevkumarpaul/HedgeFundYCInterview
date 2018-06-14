﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCache
{
    public abstract class CacheManager<CacheType> : ICacheManager<CacheType> where CacheType : class, new()
    {
        protected CacheType Storage = null;
        protected static readonly object lockobj = new object();

        internal CacheManager()
        {
            Storage = new CacheType();
        }

        public T Add<T>(string key, T item, Nullable<TimeSpan> time = null)
        {
            return Set<T>(key, item, time);
        }

        public T Update<T>(string key, Func<T> action, Nullable<TimeSpan> time = null)
        {
            return Change<T>(key, action, time);
        }

        public T Get<T>(string key)
        {
            return Fetch<T>(key);
        }

        public T Remove<T>(string key)
        {
            return Delete<T>(key);
        }

        public abstract void RemoveAll();
        
        protected abstract T Set<T>(string key, T item, Nullable<TimeSpan> time = null);
        protected abstract T Change<T>(string key, Func<T> action, Nullable<TimeSpan> time = null);
        protected abstract T Fetch<T>(string key);
        protected abstract bool Exists(string key);
        protected abstract T Delete<T>(string key);
    }
}
