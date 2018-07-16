using System.Collections.Generic;
using System.Security.Principal;

namespace RequestMessageHandler.Entities
{
    public class WebIdentity : IIdentity
    {
        public List<string> CustomInformation { get; set; }
        public string Token { get; set; }
        

        #region IIdentityProperties
        public string Name { get; set; }
        public bool IsAuthenticated { get; set; }
        public string AuthenticationType { get; set; }       
	    #endregion	  
    }
}
