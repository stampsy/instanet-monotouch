using InstaNet.Models;
using RestSharp;
using InstaNet.Authenticators;
using System;

namespace InstaNet
{
    public partial class InstaNetClient
    {
        public User GetSelf ()
        {
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = new InstaNet.Authenticators.OAuth2Authenticator (UserLogin.Token);

            var request = _requestHelper.CreateSelfRequest();

            return Execute<User> (request);
        }
    }
}
