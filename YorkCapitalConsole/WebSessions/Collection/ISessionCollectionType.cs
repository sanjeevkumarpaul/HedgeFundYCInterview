using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSessions.Collection
{
    public interface ISessionCollectionType<T>
    {
        void Set(List<T> collection);
        void Remove(T value);
        void Insert(T value);
        bool Contains(T value);
    }
}
