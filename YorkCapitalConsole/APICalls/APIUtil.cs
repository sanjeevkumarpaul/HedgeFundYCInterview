using APICalls.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var data = await GetItAsync();

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

        private HttpRequestMessage CreateRequest(HttpMethod method )
        {
            AddContentTypes();
            HttpRequestMessage request = AddParameters(new HttpRequestMessage(method, prospect.Url));
            AddAuthorization(request);

            return request;
        }

        private void AddContentTypes()
        {
            var contents = prospect.RequestHeaders?.AcceptContentTypes ?? null;
            client.DefaultRequestHeaders.Accept.Clear();

            if (contents != null)
            {                
                foreach (var content in contents) client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(content));
            }
            else client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private W AddParameters<W>(W request)
        {            
            if (request is HttpRequestMessage && !prospect.ParametersIsQueryString && prospect.Parameters != null)
                (request as HttpRequestMessage).Content = new StringContent(JsonConvert.SerializeObject(prospect.Parameters), Encoding.UTF8, prospect.RequestHeaders?.ParameterContentType ?? "application/json");

            //TODO: extended further if something comes up.

            return request;
        }

        private void AddHeaders(HttpRequestMessage request, string token = null)
        {
            if (token == null)
            {
                var headers = prospect.RequestHeaders?.Headers ?? null;

                if (headers != null)
                    foreach (var header in headers)                    
                        request.Headers.Add(header.Key, header.Value);                    
            }
            else
                request.Headers.Add("Authorization", token);
        }

        private void AddAuthorization(HttpRequestMessage request)
        {
            client.DefaultRequestHeaders.Authorization = null;

            if (prospect.Authorization?.Token != null)
            {
                if (prospect.Authorization.IsTokenAHeader)
                    AddHeaders(request, prospect.Authorization.Token);
                else
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(prospect.Authorization.Type.ToString(), prospect.Authorization.Token);
            }
            else if (prospect.Authorization?.Username != null && prospect.Authorization?.Password != null)
            {
                var bytearray = Encoding.ASCII.GetBytes($"{prospect.Authorization.Username}:{prospect.Authorization.Password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(prospect.Authorization.Type.ToString(), Convert.ToBase64String(bytearray));
            }
        }

        private async Task<string> GetItAsync()
        {
            try
            {
                var request = CreateRequest(HttpMethod.Get);
                HttpResponseMessage response = await client.GetAsync(prospect.Url + ( !prospect.ParametersIsQueryString ? prospect.ParametersIsQueryString : "" ));
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return data;
                }
            }
            catch
            {

            }
            return string.Empty;
        }

        private async Task<string> PostItAsync()
        {
            try
            {
                var request = CreateRequest(HttpMethod.Post);

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
            }
            catch
            {

            }

            return string.Empty;
        }

        private string GetIt()
        {
            try
            {
                var request = CreateRequest(HttpMethod.Get);

                HttpResponseMessage response = client.GetAsync(prospect.Url + (!prospect.ParametersIsQueryString ? prospect.ParametersIsQueryString : "")).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    return data;
                }
            }
            catch
            {

            }

            return string.Empty;
        }

        private string PostIt()
        {
            try
            {
                var request = CreateRequest(HttpMethod.Post);

                var response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    return responseString;
                }
            }
            catch
            {

            }

            return string.Empty;
        }

    }
}
