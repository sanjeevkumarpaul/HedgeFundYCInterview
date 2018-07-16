using System;
using APICalls.Bases;
using APICalls.Entities.Interfaces;

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
