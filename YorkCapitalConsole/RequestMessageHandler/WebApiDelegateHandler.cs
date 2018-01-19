using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using RequestMessageHandler.Entities;
using RequestMessageHandler.Process;

namespace RequestMessageHandler
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-message-handlers    
    /// This is mainly for WEB API and not for normal MVC
    /// </summary>
    /// <typeparam name="Identity"></typeparam>

    public class WebApiDelegateHandler<Identity> : DelegatingHandler where Identity : WebIdentity, new()
    {
        private WebCheckOptions Options;

        public WebApiDelegateHandler(WebCheckOptions options = null) 
        {
            Options = options;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            WebProcessHeader.BindHeadersToPrincipal<Identity>(Options);
            
            return base.SendAsync(request, cancellationToken);
        }

    }
}
