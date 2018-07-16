using System;
using System.Collections.Generic;
using System.Linq;
using WebSessions;

namespace RequestMessageHandler.Entities.BreadCrumbs
{
    internal partial class BreadCrumbList
    {
        internal BreadCrumbList() { Reset(); }

        /// <summary>
        /// Any given name - (Commonly we can use GUID for the same)
        /// </summary>
        internal string Name { get; set; } //any random name for a start.
        /// <summary>
        /// Each click of user, when browser moves pagse, for every page a single item will be created.
        /// </summary>
        internal IList<BreadCrumbItem> Items { get; set; }

        internal int Count { get { return Items.Count; } }        
    }

    /// <summary>
    /// Static Methods.
    /// </summary>
    partial class BreadCrumbList
    {
        public static BreadCrumbList Get(string currentUrl)
        {
            if (SessionManager.Get<BreadCrumbList>(Session) == null) SessionManager.Set<BreadCrumbList>(Session, new BreadCrumbList { Name = Session });
            var bread = SessionManager.Get<BreadCrumbList>(Session); //should not be null at all.

            //return if same url is called via @Html.RenderAction() or @Html.Action().
            //We need to perform this before .Reset().
            if (bread.Count > 0 && bread.Items[bread.Count - 1].Url.Equals(currentUrl, StringComparison.CurrentCultureIgnoreCase)) return null;

            return bread;
        }

    }

    /// <summary>
    /// Public Methods
    /// </summary>
    partial class BreadCrumbList
    {
        public void Reset()
        {
            Items = new List<BreadCrumbItem>();
        }

        public void Add(string url, string Description)
        {
            var foundBread = Items.FirstOrDefault(i => i.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase));
            if (foundBread != null) //If found to be there, remove all other beyound that.            
                Items = Items.TakeWhile(i => i.Index <= foundBread.Index).ToList();
            else
                Items.Add(new BreadCrumbItem { Url = url, Index = Count + 1, Crumb = SeparateWords(Description) });

            SessionManager.Set<BreadCrumbList>(Session, this);
        }
    }

    /// <summary>
    /// Private method to format description.
    /// </summary>
    partial class BreadCrumbList
    {
        private const string Session = "IIS_$application$_Breadcrumb_";

        private string SeparateWords(string str)
        {
            var final = "";
            bool firstCharacterCheckIsDone = false;
            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    if (str.IndexOf(c) == 0 && !firstCharacterCheckIsDone)
                    {
                        final += " " + c.ToString();
                        firstCharacterCheckIsDone = true;
                    }
                    else
                        final += " " + c.ToString().ToUpper();
                }
                else
                {
                    if (str.IndexOf(c) == 0 && !firstCharacterCheckIsDone)
                        final += c.ToString().ToUpper();
                    else
                        final += c.ToString();
                }
            }
            return final;
        }
    }
}
