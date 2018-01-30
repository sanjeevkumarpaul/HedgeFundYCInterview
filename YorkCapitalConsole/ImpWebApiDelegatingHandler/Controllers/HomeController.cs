﻿using ImpWebApiDelegatingHandler.MessageInterceptors;
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
            var breadcrumb = ((RequestMessageHandler.Entities.Principal)User).BreadCrumb;

            ViewBag.Title = "Home Page And " + identity.Host + " BREAD CRUMB: " + breadcrumb;

            return View();
        }
    }
}
