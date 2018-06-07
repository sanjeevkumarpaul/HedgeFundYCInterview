using APICalls.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Constructor, takes Prospect information to proceed. 
        /// </summary>
        /// <param name="prospect"></param>
        public APIUtil(APIProspect<T> prospect)
        {
            this.prospect = prospect;
        }

        /// <summary>
        /// Synchronouse Call to API
        /// </summary>
        /// <returns></returns>
        public T Call()
        {
            dynamic data;

            switch (prospect.Method)
            {
                case APIMethod.GET: data = GetIt(); break;
                case APIMethod.POST: data = PostIt(); break;
                case APIMethod.PUT: data = PostIt(); break;
                case APIMethod.DELETE: data = PostIt(); break;
                case APIMethod.STREAM: data = StreamIt(); break;
                case APIMethod.BYTEARRAY: data = ByteIt(); break;
                case APIMethod.STRINGARRAY: data = StringIt(); break;
                default: data = string.Empty; break;
            }
           
            return ReturnResult(data);
        }

        /// <summary>
        /// Asynchronous Call to API
        /// </summary>
        /// <returns></returns>
        public async Task<T> CallAsync()
        {
            string data = string.Empty;

            switch (prospect.Method)
            {
                case APIMethod.GET: data = await GetItAsync(); break;
                case APIMethod.POST: data = await PostItAsync(); break;                
            }

            return ReturnResult(data);
        }
        
        private T ReturnResult(dynamic data)
        {
            Decrypt(data);
            return prospect.Result;
        }

        private void Decrypt(dynamic data)
        {
            if (data != null)
            {
                try
                {
                    if (prospect.Method == APIMethod.STREAM || prospect.Method == APIMethod.BYTEARRAY || prospect.Method == APIMethod.STRINGARRAY)
                    {
                        prospect.Result = new T { OtherResponses = data };
                    }
                    else
                    {
                        var _result = Newtonsoft.Json.JsonConvert.DeserializeObject((string)data, typeof(T));

                        prospect.Result = (T)_result;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error: ${ex.Message}");
                }
            }
        }

        private HttpRequestMessage CreateRequest()
        {
            AddContentTypes();
            HttpRequestMessage request = AddParameters(new HttpRequestMessage(prospect.HttpMethod, prospect.Url));
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
            var headers = ( token != null ) ? new APINamePareMedia[] { new APINamePareMedia { Key = "Authorization", Value = token  } }  : prospect.RequestHeaders?.Headers ?? null;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (prospect.HttpMethod != HttpMethod.Get)
                        request.Headers.Add(header.Key, header.Value);
                    else
                    {
                        if (client.DefaultRequestHeaders.Contains(header.Key)) client.DefaultRequestHeaders.Remove(header.Key);
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }
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
            HttpResponseMessage response = null;
            try
            {
                var request = CreateRequest();
                response = await client.GetAsync(prospect.Url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                throw CreateException(response, e);
            }
        }

        private async Task<string> PostItAsync()
        {
            HttpResponseMessage response = null;
            try
            {
                var request = CreateRequest();

                response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                throw CreateException(response, e);
            }
        }

        private string GetIt()
        {
            HttpResponseMessage response = null;
            try
            {
                var request = CreateRequest();

                response = client.GetAsync(prospect.Url).Result; 
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    return data;
                }
                else throw new Exception();
            }           
            catch (Exception e)
            {
                throw CreateException(response, e);
            }
            
        }

        private string PostIt()
        {
            HttpResponseMessage response = null;
            try
            {
                var request = CreateRequest();

                response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    return responseString;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                throw CreateException(response, e);
            }
        }

        private string PutIt()
        {
            HttpResponseMessage response = null;
            try
            {
                var request = CreateRequest();

                response = client.PutAsync(prospect.Url, request.Content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    return responseString;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                throw CreateException(response, e);
            }
        }

        private string DeleteIt()
        {
            HttpResponseMessage response = null;
            try
            {
                var request = CreateRequest();

                response = client.DeleteAsync(prospect.Url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    return responseString;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                throw CreateException(response, e);
            }
        }

        private string StringIt()
        {
            string response = null;
            try
            {
                var request = CreateRequest();

                response = client.GetStringAsync(prospect.Url).Result;
                if (response != null)
                {
                    return response;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                throw CreateException(null, e);
            }
        }


        private byte[] ByteIt()
        {
            byte[] response = null;
            try
            {
                var request = CreateRequest();

                response = client.GetByteArrayAsync(prospect.Url).Result;
                if (response != null)
                {
                    return response;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                throw CreateException(null, e);
            }
        }

        private Stream StreamIt()
        {
            Stream response = null;
            try
            {
                var request = CreateRequest();

                response = client.GetStreamAsync(prospect.Url).Result;
                if (response != null)
                {
                    return response;
                }
                else throw new Exception();
            }
            catch (Exception e)
            {
                throw CreateException(null, e);
            }
        }


        private APIException CreateException(HttpResponseMessage response, Exception e)
        {
            return
            new APIException(response != null ? response : new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.SeeOther, ReasonPhrase = e.Message },
                                       (APIProspectOptionBase)prospect);
        }

    }
}
