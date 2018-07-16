using System;

namespace RequestMessageHandler.Exceptions
{
    public class WebRequestInterceptorException : Exception
    {
        public WebRequestInterceptorException(string message) : base(message) { }

    }
}
