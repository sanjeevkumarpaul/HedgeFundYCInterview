using RequestMessageHandler.Entities;
using RequestMessageHandler.Process;
using System;
using System.Web.Mvc;
using System.Web.Routing;

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

            var controller = base.CreateController(requestContext, controllerName);

            WebProcessHeader.BindHeadersToPrincipal<Identity>(Options);
            new WebProcessBreadCrumb(Options.BreadCrumbOption).Process(requestContext, ControllerType(requestContext, controllerName));

            return controller;
        }

        internal IController Create(RequestContext requestContext, string controllerName)
        {
            var type = GetControllerType(requestContext, controllerName);
            IController controller = GetControllerInstance(requestContext, type);

            return controller;
        }

        internal Type ControllerType(RequestContext requestContext, string controllerName)
        {
            return GetControllerType(requestContext, controllerName);
        }
    }
}
