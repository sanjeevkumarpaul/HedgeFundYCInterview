using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Configurations
{
    public class APIObjectParameter
    {
        private List<object> _params = null;

        internal List<object> ObjectParams { get { return _params; } }

        public APIObjectParameter() { _params = new List<object>(); }
    }
    
    public interface IAPIResult
    {
        void Reponses(IAPIProspect resultProspect, ApiXmlConfiguration config);        
    }
}
