using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSessions.Collection
{
    public class SessionCollectionManager<T, E> : ISessionCollectionType<T> where E : struct, IConvertible
    {
        #region ^Private members not exposed
        private List<T> _collection = null;
        private string _sessionName = string.Empty;
        #endregion ~Private members not exposed

        #region ^Private methods
        private void Set()
        {
            SessionManager.Set<List<T>>(_sessionName, _collection);
        }

        private bool Get()
        {
            return (_collection = SessionManager.Get<List<T>>(_sessionName)) != null;
        }
        #endregion ~Private methods

        
        #region ^Public methods

        /// <summary>
        /// Sets the List of Generic type to Session
        /// </summary>
        /// <param name="collection"></param>
        virtual public void Set(List<T> collection)
        {
            if (collection == null) return;
            _collection = collection;
            Set();
        }

        /// <summary>
        /// Removes the specific Generic Type from List and updates the Session
        /// </summary>
        /// <param name="value">Generic Type</param>
        virtual public void Remove(T value)
        {
            if (Get()) _collection.Remove(value);
            Set();
        }

        /// <summary>
        /// Inserts a specific Generic Type into List and updates the Session
        /// </summary>
        /// <param name="value">Generic Type</param>
        virtual public void Insert(T value)
        {
            if (!Get()) _collection = new List<T>();

            _collection.Add(value);
            Set();
        }

        /// <summary>
        /// Checks for the Generic Type in List if the List is avilable in Session
        /// </summary>
        /// <param name="value">Generic Type</param>
        /// <returns>true or false based on availibility of the value</returns>
        virtual public bool Contains(T value)
        {
            if ((Get() && !_collection.Contains(value)) || !Get()) return false;

            return true;
        }
        #endregion ~Public methods

    }
}
