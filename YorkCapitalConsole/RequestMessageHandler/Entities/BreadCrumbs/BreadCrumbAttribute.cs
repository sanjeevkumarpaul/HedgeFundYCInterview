using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestMessageHandler.Entities.BreadCrumbs
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =false , Inherited =false)]
    public class BreadCrumbAttribute : Attribute
    {
        /// <summary>
        /// Description on the Page to be displayed.
        /// </summary>
        public string Crumb { get; set; }

    }
}
