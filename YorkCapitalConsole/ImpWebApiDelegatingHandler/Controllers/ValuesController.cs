using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using ImpWebApiDelegatingHandler.MessageInterceptors;

namespace ImpWebApiDelegatingHandler.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            var identity = (ApiKeyInterceptor)HttpContext.Current.User.Identity;   //Right Usage of getting Headers read through Expando to an Object.

            return new string[] { "value1", "value2", identity.Host };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
