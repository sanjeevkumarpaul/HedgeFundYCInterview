using RequestMessageHandler.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Web.Routing;
using System.Web.Mvc;

using Extensions;
using Reflections;
using RequestMessageHandler.Entities.BreadCrumbs;

namespace RequestMessageHandler.Process
{
    internal class WebProcessBreadCrumb
    {
        private BreadCrumbOptions Options = null;
        private Principal principal = null;
        private Contents content = null;

        private class Contents
        {
            internal string Description { get; set; }
            internal string Url { get; set; }
            internal string RefUrl { get; set; }
            internal string Method { get; set; }
            internal bool IsAjax { get; set; }
            internal bool IsAlienDomain { get; set; }
            internal bool IsResetKeyExists { get; set; }
        }


        internal WebProcessBreadCrumb(BreadCrumbOptions options)
        {
            Options = options;
            principal = (Principal)Thread.CurrentPrincipal;
        }

        /// <summary>
        /// This is for MVC
        /// </summary>
        /// <param name="requestContext"></param>
        internal void Process(RequestContext requestContext, Type controllerType = null)
        {
            if (!Options.CreateBreadCrumbs) return;
            GetContents(requestContext, controllerType);
            Assign(Create());            
        }

        private void GetContents(RequestContext context, Type controllerType = null)
        {
            var action = context.RouteData.GetRequiredString("action");
            var controller = context.RouteData.GetRequiredString("controller");

            var desc = CustomAttributes.FindMethodFirst<BreadCrumbAttribute>(controllerType, action)?.Crumb;
            if (!desc.Empty())
                action = desc;
            else
            {
                if (action.EqualsIgnoreCase("index")) action = controller;
                if (!Options.MVCCrumbKey.Empty())
                {
                    var value = (context.RouteData.Values.Keys.Contains(Options.MVCCrumbKey) ?
                                    context.RouteData.Values[Options.MVCCrumbKey].ToString() : "").RemoveSpecialCharacters();
                    try
                    {
                        if (Convert.ToInt32(value) < 0) action = value;
                    }
                    catch
                    {
                        action = value;
                    }
                }
            }
            var req = System.Web.HttpContext.Current.Request;
            content = new Contents
            {
                Description = action,
                Url = req.Url.OriginalString,
                RefUrl = req.UrlReferrer == null ? "" : req.UrlReferrer.OriginalString,
                Method = req.HttpMethod,
                IsAjax = new System.Web.HttpRequestWrapper(req).IsAjaxRequest(),
                IsAlienDomain = req.UrlReferrer == null ? false : (req.UrlReferrer.GetLeftPart(UriPartial.Authority) == req.Url.GetLeftPart(UriPartial.Authority)),
                IsResetKeyExists = !Options.RedirctionKeyInUrl.Empty() && req.Url.GetLeftPart(UriPartial.Path).Contains( Options.RedirctionKeyInUrl )
            };
        }

        private BreadCrumbList Create()
        {
            BreadCrumbList bread = null;
            if ( (bread = BreadCrumbList.Get(content.Url)) != null)
            {
                if (content.RefUrl.Empty() || content.IsAlienDomain || content.IsResetKeyExists) bread.Reset();

                bread.Add(content.Url, content.Description);
            }

            return bread;
        }

        private void Assign(BreadCrumbList bread)
        {
            var html = Options.GetHtml(bread);

            principal.BreadCrumb = html;
        }
    }
}
