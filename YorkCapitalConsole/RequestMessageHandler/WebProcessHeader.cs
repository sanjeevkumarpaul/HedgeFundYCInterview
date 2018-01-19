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

namespace RequestMessageHandler
{
    internal class WebProcessHeader
    {
        internal static IPrincipal ReadHeaders<Identity>(HttpRequestMessage request) where Identity : WebIdentity, new()
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

            return new Principal(identity);

        }

    }
}
