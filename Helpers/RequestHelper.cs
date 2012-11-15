using System;
using System.IO;
using RestSharp;
//using InstaNet.Models;
using RestSharp.Serializers;

namespace InstaNet.Helpers
{
    /// <summary>
    /// Helper class for creating DropNet RestSharp Requests
    /// </summary>
    public class RequestHelper
    {       
        private readonly string _version;

        public RequestHelper(string version)
        {
            _version = version;
        }

        public RestRequest CreateLoginRequest(string apiKey, string email, string password)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("oauth_consumer_key", apiKey);

            request.AddParameter("email", email);

            request.AddParameter("password", password);

            return request;
        }

        public RestRequest CreateTokenRequest()
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/oauth/request_token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateAccessTokenRequest()
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/oauth/access_token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateSelfRequest()
        {
            var request = new RestRequest(Method.GET);
            request.RootElement = "data";

            request.Resource = "{version}/users/self";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }
    }

	internal static class StreamUtils
	{
		private const int STREAM_BUFFER_SIZE = 128 * 1024; // 128KB

		public static void CopyStream (Stream source, Stream target)
		{ CopyStream (source, target, new byte[STREAM_BUFFER_SIZE]); }

		public static void CopyStream (Stream source, Stream target, byte[] buffer)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (target == null) throw new ArgumentNullException ("target");

			if (buffer == null) buffer = new byte[STREAM_BUFFER_SIZE];
			int bufferLength = buffer.Length;
			int bytesRead;
			while ((bytesRead = source.Read (buffer, 0, bufferLength)) > 0)
				target.Write (buffer, 0, bytesRead);
		}
	}
}
