﻿using System.Security.Principal;

namespace RequestMessageHandler.Entities
{
    public class Principal : IPrincipal
    {
        private readonly WebIdentity _identity;
        private string _breadcrumb;
        
        public IIdentity Identity
        {
            get { return _identity; }
        }

        public string BreadCrumb
        {
            get { return _breadcrumb;  }
            internal set { _breadcrumb = value;  }
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
