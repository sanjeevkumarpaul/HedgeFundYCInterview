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
using RequestMessageHandler.Constants;
using System.Threading;
using System.Linq.Expressions;

namespace RequestMessageHandler.Process
{
    internal class WebProcessHeader
    {
        internal static IPrincipal BindHeadersToPrincipal<Identity>(WebCheckOptions options) where Identity : WebIdentity, new()
        {
            var headers = HttpContext.Current.Request.Headers;

            var identity = new Identity()
            {
                CustomInformation = (headers[WebCustomHeaderKey.Key] ?? string.Empty).Split(',').ToList(),
                Token = headers[WebCustomHeaderKey.Token] ?? string.Empty
            };

            WebIdentityExpando _identityExpando = new WebIdentityExpando { Expansions = "Identity Object for Web" };
            foreach (var key in headers.AllKeys.Where(k => !IsKnownKey(k))) _identityExpando[key.Replace("-", "")] = headers[key];
            _identityExpando.SetValuesToOtherType(identity);

            var _principal = new Principal(identity);
            Thread.CurrentPrincipal = _principal;
            HttpContext.Current.User = _principal;

            return _principal;
        }


        private static bool IsKnownKey(string key)
        {
            if (key.Equals(WebCustomHeaderKey.Key, StringComparison.CurrentCultureIgnoreCase) ||
                key.Equals(WebCustomHeaderKey.Token, StringComparison.CurrentCultureIgnoreCase))
                return true;

            return false;
        }
    }
}
