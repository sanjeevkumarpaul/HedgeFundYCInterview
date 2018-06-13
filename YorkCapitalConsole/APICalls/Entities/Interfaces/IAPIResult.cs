using APICalls.Configurations;
using APICalls.Entities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities.Interfaces
{
/*
 Usage
 //Usage
private class APIProspectResults : IAPIResult
{
    public void Reponses(IAPIProspect resultProspect, ApiXmlConfiguration config)
    {
        Console.WriteLine("Type : " + resultProspect.GetType().Name);                    
    }
}
*/
    public interface IAPIResult
    {
        void Reponses(IAPIProspect resultProspect, APIConfiguration config);
        void Post(IEnumerable<IAPIProspect> results);
        void Final(IAPIProspect result);
        void Error<T>(T exception, APIConfiguration config, params object[] others) where T : APIException;        
    }

    public interface IAPIParallelResult : IAPIResult
    {
        object[] ParallelStart();  //If required any paramters to be taken into consideration.
        void ParallelProgress(IAPIParallelProgress progress);        
    }
}
