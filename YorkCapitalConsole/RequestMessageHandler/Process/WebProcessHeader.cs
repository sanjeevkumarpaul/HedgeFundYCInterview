using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

using Extensions;
using RequestMessageHandler.Entities;
using RequestMessageHandler.Constants;
using System.Threading;
using System.Collections.Specialized;
using RequestMessageHandler.Exceptions;

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
            foreach (var key in ValidateHeader(headers, options)) _identityExpando[key.Replace("-", "")] = headers[key];
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

        private static IEnumerable<string> ValidateHeader(NameValueCollection headers, WebCheckOptions options)
        {
            var keys = headers.AllKeys.Where(k => !IsKnownKey(k));

            if (options!= null)
            {
                string _token = keys.FirstOrDefault(k => k.Equals(options.TokenIdentity, StringComparison.CurrentCultureIgnoreCase));

                if ( options.TokenMustExist && !options.TokenIdentity.Empty() && _token == null)
                    throw new WebRequestInterceptorException("Token Identity not present during current request.");

                _token = !_token.Empty() ? headers[options.TokenIdentity] : headers[WebCustomHeaderKey.Token];

                if (options.TokenMustExist && _token.Empty())                                
                    throw new WebRequestInterceptorException("Token value not present during current request.");
                
                if (!options.TokenExactValue.Empty() && !_token.Equals(options.TokenExactValue))
                    throw new WebRequestInterceptorException("Token value expected is incorrect during current request.");


                foreach(var chead in options.CustomHeaders)
                {
                    if (!chead.MustExist) continue;

                    string _value = headers[chead.Name];
                    if (_value.Empty())
                        throw new WebRequestInterceptorException($"Header '{chead.Name}' not present during current request.");
                }
            }
            
            return keys;
        }
    }
}
