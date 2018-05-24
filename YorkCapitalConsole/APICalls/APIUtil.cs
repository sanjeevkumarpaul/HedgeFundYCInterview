using APICalls.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APICalls
{
    public class APIUtil<T> where T : IAPIProspect, new()
    {
        private static readonly HttpClient client = new HttpClient();
        private APIProspect<T> prospect;

        public APIUtil(APIProspect<T> prospect)
        {
            this.prospect = prospect;
        }

        public async Task<T> GetAsync()
        {
            return ReturnResult("");
        }


        private T ReturnResult(string data)
        {
            Decrypt(data);
            return prospect.Result;
        }

        private void Decrypt(string data)
        {
            if (data != null)
            {
                var _result = Newtonsoft.Json.JsonConvert.DeserializeObject(data, prospect.Result.GetType());

                prospect.Result = (T)_result;
            }
        }

        private void AddContentTypes()
        {

        }

        private void AddParameters()
        {

        }


        private Task<string> GetItAsync()
        {
            try
            {

            }
            catch
            {

            }
        }

    }
}
