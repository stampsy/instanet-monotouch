using InstaNet.Models;
using RestSharp;
using System;
using InstaNet.Authenticators;
using InstaNet.Exceptions;

namespace InstaNet
{
    public partial class InstaNetClient
    {
        public void GetSelfAsync (Action <User> success, Action <InstagramException> failure)
        {
            _restClient.BaseUrl = ApiBaseUrl;
            _restClient.Authenticator = new InstaNet.Authenticators.OAuth2Authenticator (UserLogin.Token);

            var request = _requestHelper.CreateSelfRequest();

            ExecuteAsync<User> (request, success, failure);
        }
    }
}