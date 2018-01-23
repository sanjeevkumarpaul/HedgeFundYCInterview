using ImpWebApiDelegatingHandler.MessageInterceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImpWebApiDelegatingHandler.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var identity = (ApiKeyInterceptor)User.Identity;

            ViewBag.Title = "Home Page And " + identity.Host;

            return View();
        }
    }
}
