﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Entities
{
    public class APIException : Exception, IAPIProspect
    {
        public string Status { get; private set; }        
        public APIException(HttpResponseMessage response) : base(response.ReasonPhrase)
        {
            Status = response.StatusCode.ToString();                      
        }
    }
}
