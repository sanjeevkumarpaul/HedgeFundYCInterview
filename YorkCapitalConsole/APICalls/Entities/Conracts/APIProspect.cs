using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APICalls.Bases;
using APICalls.Entities.Interfaces;
using Extensions;

namespace APICalls.Entities.Contracts
{
    public class APIProspect<T> : APIProspectOptionBase where T: IAPIProspect
    {        
        public T Result { get; set; }

        public APIProspect()
        {            
            Result = Activator.CreateInstance<T>();
        }
    }
}
