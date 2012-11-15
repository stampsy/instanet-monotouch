using System;
using RestSharp;

namespace InstaNet.Authenticators
{
    public class OAuth2Authenticator : OAuth2UriQueryParameterAuthenticator
    {
        public OAuth2Authenticator (string token) : base (token) { }

        public override void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddParameter("access_token", AccessToken, ParameterType.GetOrPost);
        }
    }
}

