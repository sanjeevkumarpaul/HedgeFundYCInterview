using RequestMessageHandler.Entities;
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

    public class WebMvcIControllerFactory<Identity> : IControllerFactory  where Identity : WebIdentity, new()
    {
        private WebCheckOptions Options;

        public WebMvcIControllerFactory(WebCheckOptions options = null)
        {
            Options = options;
        }

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
                throw new ArgumentNullException("controllerName");

           WebProcessHeader.BindHeadersToPrincipal<Identity>(Options);
           new WebProcessBreadCrumb(Options.BreadCrumbOption).Process(requestContext, ControllerType(requestContext, controllerName));

            return Create(requestContext, controllerName);
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        public void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null) disposable.Dispose();
                
        }

        private IController Create(RequestContext context, string controllerName)
        {
            var factory = new WebMvcDefaultControllerFactory<Identity>();

            return factory.Create(context, controllerName);
        } 

        private Type ControllerType(RequestContext context, string controllerName)
        {
            var factory = new WebMvcDefaultControllerFactory<Identity>();

            return factory.ControllerType(context, controllerName);
        }

    }
}
