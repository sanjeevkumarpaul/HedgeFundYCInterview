using APICalls.Bases;
using APICalls.Dependents;
using APICalls.Entities.Contracts;
using APICalls.Entities.Interfaces;
using APICalls.Enum;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APICalls
{
    public sealed class APIUtil<T> where T : IAPIProspect
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
            dynamic data = CallAsyn().Result;
            return ReturnResult(data);
        }

        /// <summary>
        /// Asynchronous Call to API
        /// </summary>
        /// <returns></returns>
        public async Task<T> CallAsync()
        {
            dynamic data = await CallAsyn();

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
                        if (typeof(T) is IAPIProspectUpgrade)
                        {
                            var _upgrade = Activator.CreateInstance<T>();
                            ((IAPIProspectUpgrade)_upgrade).OtherResponses = data;

                            prospect.Result = _upgrade;
                        }
                    }
                    else
                    {
                        var _result = Newtonsoft.Json.JsonConvert.DeserializeObject((string)data, typeof(T));

                        prospect.Result = (T)_result;
                    }
                }
                catch(Exception e)
                {
                    throw CreateException(null, e);
                }
            }
        }

        /// <summary>
        /// creates the request based on content Type/Authorization etc.
        /// </summary>
        /// <returns>HttpRequestMessage</returns>
        private HttpRequestMessage CreateRequest()
        {
            AddContentTypes();            
            HttpRequestMessage request = AddParameters(new HttpRequestMessage(prospect.HttpMethod, prospect.Url));
            AddAuthorization(request);

            return request;
        }

        /// <summary>
        /// Adding content types from options.
        /// </summary>
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

        /// <summary>
        /// Adding parameters for the body of API reqeust. Mostly for POST method.
        /// </summary>
        /// <typeparam name="W"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private W AddParameters<W>(W request)
        {
            if (request is HttpRequestMessage && prospect.ParameterBody != null)
                (request as HttpRequestMessage).Content = new StringContent(JsonConvert.SerializeObject(prospect.ParameterBody), Encoding.UTF8, prospect.RequestHeaders?.ParameterContentType ?? "application/json");

            //TODO: extended further if something comes up.

            return request;
        }

        /// <summary>
        /// Adding headers or Autorization token as Header
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <param name="token">string denoted as token</param>
        private void AddHeaders(HttpRequestMessage request, string token = null)
        {
            var headers = ( token != null ) ? new APINamePareMedia[] { new APINamePareMedia { Key = "Authorization", Value = token  } }  : prospect.RequestHeaders?.Headers ?? null;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (prospect.Method != APIMethod.GET)
                        request.Headers.Add(header.Key, header.Value);
                    else
                    {
                        if (client.DefaultRequestHeaders.Contains(header.Key)) client.DefaultRequestHeaders.Remove(header.Key);
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }
        }

        /// <summary>
        ///  Adding authorization token as Header or Autorization iteself
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
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

        /// <summary>
        /// Calling web API asynchronously.
        /// </summary>
        /// <returns>dynamic object which will be deserialized with respect to Result object set via Generic Type</returns>
        private async Task<dynamic> CallAsyn()
        {
            HttpResponseMessage response = null;
            dynamic content = null;
            try
            {
                var request = CreateRequest();

                bool readContent = true;
                switch(prospect.Method)
                {
                    case APIMethod.GET: response = await client.GetAsync(prospect.Url); break;
                    case APIMethod.POST: response = await client.SendAsync(request); break;
                    case APIMethod.PUT: response = await client.PutAsync(prospect.Url, request.Content); break;
                    case APIMethod.DELETE: response = await client.DeleteAsync(prospect.Url); break;

                    case APIMethod.STREAM: readContent = false; content = await client.GetStreamAsync(prospect.Url); break;
                    case APIMethod.STRINGARRAY: readContent = false; content = await client.GetStringAsync(prospect.Url); break;
                    case APIMethod.BYTEARRAY: readContent = false; content = await client.GetByteArrayAsync(prospect.Url); break;
                }

                if (readContent)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString;
                    }
                    else throw new Exception();
                }
                else
                {
                    if (content != null)                    
                        return content;                    
                    else throw new Exception();
                }
            }
            catch (Exception e)
            {
                throw CreateException(response, e);
            }
        }
        
        /// <summary>
        /// Creates Exception object related to API 
        /// </summary>
        /// <param name="response">Status/Message is put as exception message</param>
        /// <param name="e">Exception</param>
        /// <returns></returns>
        private APIException CreateException(HttpResponseMessage response, Exception e)
        {
            return
            new APIException(response != null ? response : new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.SeeOther, ReasonPhrase = e.Message },
                                       (APIProspectOptionBase)prospect);
        }

    }
}
