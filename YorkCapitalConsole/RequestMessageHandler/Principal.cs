using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RequestMessageHandler
{
    public class Principal : IPrincipal
    {
        private readonly WebIdentity _identity;

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public Principal(WebIdentity identity)
        {
            _identity = identity;
            _identity.IsAuthenticated = identity.IsAuthenticated;
            _identity.AuthenticationType = identity.AuthenticationType;
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }

}
