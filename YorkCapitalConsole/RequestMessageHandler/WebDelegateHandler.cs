using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Extensions;

namespace RequestMessageHandler
{
    public class WebDelegateHandler<Identity> : DelegatingHandler where Identity : WebIdentity
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = HttpContext.Current.Request.Headers;

            var requestSourceIsWeb = bool.Parse(headers["requestSourceIsWeb"] ?? "true");

            var customHeaderInformation = (headers["CustomHeaderInformation"] ?? string.Empty).Split(',').ToList();              //THIS iS WHAT to handle.

            var identity = new WebIdentity
            {
                RequestSourceIsWeb = requestSourceIsWeb,

                CustomHeaderInformation = customHeaderInformation
            };

            Dictionary<string, string> keyInfo = new Dictionary<string, string>();
            //foreach (var key in headers.Keys) keyInfo.Add(key, headers[key]);


            identity.SetProperties(this);

            var principal = new Principal(identity);
            
            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;

            return base.SendAsync(request, cancellationToken);
        }

    }
}
