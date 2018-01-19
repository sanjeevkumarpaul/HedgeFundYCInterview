using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Extensions;
using RequestMessageHandler.Entities;

namespace RequestMessageHandler
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-message-handlers    
    /// This is mainly for WEB API and not for normal MVC
    /// </summary>
    /// <typeparam name="Identity"></typeparam>

    public class WebDelegateHandler<Identity> : DelegatingHandler where Identity : WebIdentity, new()
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var principal = WebProcessHeader.ReadHeaders<Identity>(request);
            
            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;

            return base.SendAsync(request, cancellationToken);
        }

    }
}
