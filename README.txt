# HedgeFundYCInterview
Just Practice.


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

        #region ^Ctor

        /// <summary>
        /// Constructor method, called every time Collection to Session is refered.
        /// Always checks the Session Collection name and makes sure it is of type 'SessionCollection' type.
        /// </summary>
        /// <param name="sessionCollectionEnum">SessionCollectionType, from which the Session Name is derived.</param>
        internal SessionCollectionManager(E sessionCollectionEnum)
        {
            if (!(typeof(E).Equals(typeof(SessionCollection))))
                throw new Exception("Session Collection Manager only Accepts Enum from SCA.VAS.Common.Sessions.SessionCollection");

            _sessionName = sessionCollectionEnum.ToString();
        }
        #endregion ~Ctor

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
            if (Get()) _collection.Remove(value) ;            
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
            if ( ( Get() && !_collection.Contains(value) ) || !Get()) return false;            

            return true;
        }
        #endregion ~Public methods

    }
    
    
    

public class SessionManager
    {
        #region ^Public Methods
        /// <summary>
        /// Set Value to a Session Variable.
        /// </summary>
        /// <typeparam name="T">Known Type to be set</typeparam>
        /// <param name="name">Session Variable used</param>
        /// <param name="value">Value of Type 'T' to be set</param>
        public static void Set<T>(string name, T value)
        {
            SetSession<T>(name, value);
        }
        
        /// <summary>
        /// Get Value From Session Variable.
        /// </summary>
        /// <typeparam name="T">Known Type to get</typeparam>
        /// <param name="name">Session Variable used</param>
        /// <param name="value">Value of Type 'T' to get</param>
        public static T Get<T>(string name)
        {
            if (Exists(name))
            {
                return (T)HttpContext.Current.Session[name];
            }

            return default(T);
        }

        /// <summary>
        /// Remove Session Variable.
        /// </summary>     
        /// <param name="name">Session Variable to Remove</param>     
        public static void Remove(string name)
        {
            if (Exists(name))
            {
                HttpContext.Current.Session.Remove(name);
            }
        }

        /// <summary>
        /// Remove Session Variable(s).
        /// </summary>     
        /// <param name="name">Session Variable Names to Remove</param>   
        public static void Remove(List<string> names)
        {
            names.ForEach((name) => Remove(name));            
        }

        /// <summary>
        /// Remove All Session Variables, irrespective of Session Names.
        /// </summary>           
        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
        }
        #endregion ~Public Methods
        
        #region ^Private methods
        
        private static void SetSession<T>(string name, T value)
        {
            if (value != null)
            {
                HttpContext.Current.Session[name] = value;
            }
        }

        private static bool Exists(string key)
        {
            try
            {
                return HttpContext.Current.Session[key] != null;
            }
            catch
            {

            }

            return false;
        }
        #endregion ~Private methods
    }
    
    
    



