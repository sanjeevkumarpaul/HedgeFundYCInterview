using RequestMessageHandler.Entities;
using RequestMessageHandler.Process;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace RequestMessageHandler
{
    /// <summary>
    /// Another way to Read headers and send global message to all responses.
    /// </summary>
    public class WebApiRequestHandler<Identity> : HttpControllerDispatcher where Identity : WebIdentity, new()
    {
        private WebCheckOptions Options;

        public WebApiRequestHandler(HttpConfiguration config, WebCheckOptions options = null)   : base(config)
        {
            Options = options;
        }

        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken)
        {
            WebProcessHeader.BindHeadersToPrincipal<Identity>(Options);
            
            return base.SendAsync(request, cancellationToken);
        }
    }
}
