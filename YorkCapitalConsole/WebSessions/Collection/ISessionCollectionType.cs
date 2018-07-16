using System.Collections.Generic;

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
