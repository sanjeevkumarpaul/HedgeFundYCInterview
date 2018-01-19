using ImpWebApiDelegatingHandler.MessageInterceptors;
using RequestMessageHandler;
using RequestMessageHandler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ImpWebApiDelegatingHandler
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            var options = new WebCheckOptions { /*TokenMustExist = true*/ };
            
            config.Routes.MapHttpRoute(
                name: "apiDefault",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                handler: new WebApiRequestHandler<ApiKeyInterceptor>(config, options),
                constraints: null
            );
        }
    }
}
