using APICalls.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
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
}
