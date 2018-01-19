using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RequestMessageHandler.Entities
{
    public class WebIdentity : IIdentity
    {
        public List<string> CustomHeaderInformation { get; set; }
        

        #region IIdentityProperties
        public string Name { get; set; }
        public bool IsAuthenticated { get; set; }
        public string AuthenticationType { get; set; }
        public bool RequestSourceIsWeb { get; set; }
	    #endregion

	    #region Custom Properties

	    ///...

	    #endregion
    }
}
