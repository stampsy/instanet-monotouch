using System;
using RestSharp;
using System.Net;

namespace InstaNet.Exceptions
{
    public class InstagramException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// The response of the error call (for Debugging use)
        /// </summary>
        public IRestResponse Response { get; private set; }

        public InstagramException()
        {
        }

        public InstagramException(string message)
            : base(message)
        {

        }

        public InstagramException(IRestResponse r)
        {
            Response = r;
            StatusCode = r.StatusCode;
        }

    }
}
