using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace RequestMessageHandler.Entities.BreadCrumbs
{
    public class BreadCrumbOptions
    {
        /// <summary>
        /// User specifies if Breadcrumbs are to be stored as a part of Controller Factory.
        /// If Stored, it will be a part of IPrincipal data.
        /// </summary>
        public bool CreateBreadCrumbs { get; set; }
        
        /// <summary>
        /// This key at the route may contain the description.
        /// Description is based on ACTION. If action is INDEX, CONTROLLER name is taken instead. If route contains this key, and its not Numeric, this key is taken into consideration.
        /// </summary>
        public string MVCCrumbKey { get; set; }

        /// <summary>
        /// This is to make sure to reset Breadcrumbs when this key appears somwehere within the query string.
        /// It alwyas resets when user hits the address bar of browser directly instead of clicking on a link or urlreferal is from some other domain.
        /// </summary>
        public string RedirctionKeyInUrl { get; set; }

        /// <summary>
        /// this is a html template for breadcrumb to be presented to client.
        /// can contain these place holders.
        /// {Description} {Url}
        /// Usage
        /// "<span><a href='{Url}'>{Description}</a>></span>"
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// This is a html template for last or current page breadcrumb. If not given it would be <strong>-descrpition-</strong> and is not clickable.
        /// </summary>
        public string LastTemplate { get; set; }

        /// <summary>
        /// Separator between the Breadcrumbs.
        /// Default is " >> "
        /// </summary>
        public string Separator { get; set; }


        internal string GetHtml(BreadCrumbList breads)
        {
            string breadCrumb = "";          
            if (breads != null && breads.Items.Count > 0)
            {
                foreach (var bread in breads.Items.Take(breads.Items.Count - 1)) { breadCrumb += FormatTemplate(bread); };
                               
                breadCrumb += FormatTemplate(breads.Items.Last(), true, false);
            }

            return breadCrumb;
        }

        private string FormatTemplate(BreadCrumbItem item, bool isLastBread = false, bool separatorToBePlaced = true)
        {
            string template = "";
            if (!isLastBread)
                template = Template.Empty() ? "<a href='{Url}'>{Description}</a>" : Template;
            else
                template = LastTemplate.Empty() ? "<strong>{Description}</strong>" : LastTemplate;

            return ( template.Replace("{Url}", item.Url)
                             .Replace("{Description}", item.Crumb) ) + ( separatorToBePlaced ? " " + ( Separator ?? " >> " ) + " " : "" ) ;
        }
    }
}
