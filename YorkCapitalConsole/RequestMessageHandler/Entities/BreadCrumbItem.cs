using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestMessageHandler.Entities
{
    internal class BreadCrumbItem
    {
        /// <summary>
        /// URL of the Item
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Index number of the item clicked by user.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Description to be displayed on top
        /// </summary>
        public string Crumb { get; set; }
        /// <summary>
        /// to be set true or false as per user action.
        /// </summary>
        public bool Disabled { get; set; }

    }
}
