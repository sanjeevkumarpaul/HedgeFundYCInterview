﻿using RequestMessageHandler.Entities;
using RequestMessageHandler.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace RequestMessageHandler
{
    /// <summary>
    /// USAGE
    /// Register it in Application_Start ->  ControllerBuilder.Current.SetControllerFactory(new WebMvcControllerFactory<WebIdentity>());
    /// </summary>
    /// <typeparam name="Identity"></typeparam>

    public class WebMvcDefaultControllerFactory<Identity> : DefaultControllerFactory  where Identity : WebIdentity, new()
    {
        private WebCheckOptions Options;

        public WebMvcDefaultControllerFactory(WebCheckOptions options = null)
        {
            Options = options;
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
                throw new ArgumentNullException("controllerName");

            WebProcessHeader.BindHeadersToPrincipal<Identity>(Options);
                        
            return Create(requestContext, controllerName);
        }

        internal IController Create(RequestContext requestContext, string controllerName)
        {
            var type = GetControllerType(requestContext, controllerName);
            IController controller = GetControllerInstance(requestContext, type);

            return controller;
        }
    }
}