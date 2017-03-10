using System;
using System.Net;

namespace TypedHttpClient
{
    public class HttpServerException : Exception
    {
        public HttpServerException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}