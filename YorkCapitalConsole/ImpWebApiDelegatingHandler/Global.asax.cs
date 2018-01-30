using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using RequestMessageHandler;
using RequestMessageHandler.Entities;

namespace ImpWebApiDelegatingHandler
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);            
            BundleConfig.RegisterBundles(BundleTable.Bundles);                  
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var options = new WebCheckOptions { BreadCrumbOption = new BreadCrumbOptions { CreateBreadCrumbs = true  } };

            ControllerBuilder.Current.SetControllerFactory(new WebMvcDefaultControllerFactory<MessageInterceptors.ApiKeyInterceptor>(options));
            //OR//
            //ControllerBuilder.Current.SetControllerFactory(new WebMvcIControllerFactory<MessageInterceptors.ApiKeyInterceptor>());
        }
    }
}
