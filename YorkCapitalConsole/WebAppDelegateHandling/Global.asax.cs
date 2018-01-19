using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using RequestMessageHandler;

namespace WebAppDelegateHandling
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            var config = GlobalConfiguration.Configuration;

            config.Routes.MapHttpRoute(
                name: "MvcRequestHandlerDispatcher",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action="Index", key = RouteParameter.Optional },
                constraints: null,
                handler: new WebMvcRequestHandler(config)
            );
        }
    }
}
