﻿using RequestMessageHandler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace RequestMessageHandler
{
    /// <summary>
    /// Another way to Read headers and send global message to all responses.
    /// </summary>
    public class WebMvcRequestHandler<Identity> : HttpControllerDispatcher where Identity : WebIdentity, new()
    {

        public WebMvcRequestHandler(HttpConfiguration config)   : base(config)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken)
        {

            var principal = WebProcessHeader.ReadHeaders<Identity>(request);

            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;

            return base.SendAsync(request, cancellationToken);
        }
    }
}
