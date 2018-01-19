using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestMessageHandler.Exceptions
{
    public class WebRequestInterceptorException : Exception
    {
        public WebRequestInterceptorException(string message) : base(message) { }

    }
}
