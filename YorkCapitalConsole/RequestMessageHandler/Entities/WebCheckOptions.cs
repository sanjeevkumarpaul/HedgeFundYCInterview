using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestMessageHandler.Entities
{
    public class WebCheckOptions
    {
        public WebCheckOptions()
        {
            CustomHeaders = new List<CustomHeaderOptions>();
        }


        public bool TokenMustExist { internal get; set; }
        public string TokenIdentity { internal get; set; }
        public string TokenExactValue { internal get; set; }
        
        public List<CustomHeaderOptions> CustomHeaders { internal get; set; }        
    }

    public class CustomHeaderOptions
    {
        public string Name { internal get; set; }
        public bool MustExist { internal get; set; }
    }
}
