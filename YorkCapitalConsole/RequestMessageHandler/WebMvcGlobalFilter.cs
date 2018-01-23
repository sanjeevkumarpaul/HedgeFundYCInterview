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
    public class WebMvcGlobalFilter : IControllerFactory
    {
        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
                throw new ArgumentNullException("controllerName");

            IController controller = Activator.CreateInstance(Type.GetType(controllerName)) as IController;

            return controller;
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        public void ReleaseController(IController controller)
        {
            if (controller is IDisposable)
                (controller as IDisposable).Dispose();
            else
                controller = null;
        }


    }
}

/*
 public class ConditionalFilterProvider : IFilterProvider
{
    public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, 
            ActionDescriptor actionDescriptor)
    {
        //place here your logic for application of conditional filters.
    }
}

    var provider = new ConditionalFilterProvider(); 
FilterProviders.Providers.Add(provider); 

    //to remove
    var oldProvider = FilterProviders.Providers.Single(
                f => f is FilterAttributeFilterProvider
);
FilterProviders.Providers.Remove(oldProvider);

    or

    using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace IControllerFactorySample.ControllerFactories
{
    public class YourControllerFactory : IControllerFactory
    {
        #region IControllerFactory Members

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName))
                throw new ArgumentNullException("controllerName");

            IController controller = Activator.CreateInstance(Type.GetType(controllerName)) as IController;

            return controller;
        }

        public void ReleaseController(IController controller)
        {
            if (controller is IDisposable)
                (controller as IDisposable).Dispose();
            else
                controller = null;
        }

        #endregion
    }
}
and register
ControllerBuilder.Current.SetControllerFactory(
                typeof(YourControllerFactory));

     
     
     */
