using System;

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
