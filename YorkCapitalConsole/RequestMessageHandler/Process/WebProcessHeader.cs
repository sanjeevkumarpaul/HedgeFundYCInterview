using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Extensions;
using RequestMessageHandler.Entities;
using System.Threading;

namespace RequestMessageHandler.Process
{
    internal class WebProcessHeader
    {
        internal static IPrincipal BindHeadersToPrincipal<Identity>(HttpRequestMessage request) where Identity : WebIdentity, new()
        {
            var headers = HttpContext.Current.Request.Headers;

            var customHeaderInformation = (headers["CustomHeaderInformation"] ?? string.Empty).Split(',').ToList();              //THIS iS WHAT to handle.

            var identity = new Identity()
            {               
                CustomHeaderInformation = customHeaderInformation
            };


            WebIdentityExpando _identityExpando = new WebIdentityExpando { Expansions = "Identity Object for Web" };
            foreach (var key in headers.AllKeys) _identityExpando[key.Replace("-","")] = headers[key];            
            _identityExpando.SetValuesToOtherType(identity);

            var _principal = new Principal(identity);
            Thread.CurrentPrincipal = _principal;
            HttpContext.Current.User = _principal;

            return _principal;
        }

    }
}
