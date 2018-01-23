using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebSessions
{
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

}
