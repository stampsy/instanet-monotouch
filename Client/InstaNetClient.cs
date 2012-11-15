using System;
using InstaNet.Models;
using RestSharp;
using RestSharp.Deserializers;
using InstaNet.Helpers;
using System.Net;
using InstaNet.Exceptions;
using InstaNet.Authenticators;

namespace InstaNet
{
    public partial class InstaNetClient
    {
        private const string ApiBaseUrl = "https://api.instagram.com";
        private const string Version = "v1";

        private UserLogin _userLogin;

        /// <summary>
        /// Contains the Users Token and Secret
        /// </summary>
        public UserLogin UserLogin
        {
            get { return _userLogin; }
            set
            {
                _userLogin = value;
                SetAuthProviders ();
            }
        }
        
        protected readonly string _apiKey;

        private RestClient _restClient;
        protected RequestHelper _requestHelper;

        /// <summary>
        /// Default Constructor for the InstaNetClient
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Instagram Requests</param>
        public InstaNetClient (string apiKey)
        {
            _apiKey = apiKey;

            LoadClient();
        }

        /// <summary>
        /// Creates an instance of the InstaNetClient given an API Key/Secret and a User Token/Secret
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="userToken">The User authentication token</param>
        /// <param name="userSecret">The Users matching secret</param>
        public InstaNetClient (string apiKey, string userToken)
        {
            _apiKey = apiKey;

            LoadClient();

            UserLogin = new UserLogin { Token = userToken};
        }

        private void LoadClient ()
        {
            _restClient = new RestClient (ApiBaseUrl);
            _restClient.ClearHandlers();
            var jd = new JsonDeserializer();
            /*
            _restClient.AddHandler("application/json", jd);
            _restClient.AddHandler("text/json", jd);
            _restClient.AddHandler("text/x-json", jd);
            _restClient.AddHandler("text/javascript", jd);
            */
            _restClient.AddHandler("*", jd);

            _requestHelper = new RequestHelper (Version);
        }

        [Flags]
        public enum Scope {
            Basic = 1,
            Comments = 2,
            Relationships = 4,
            Likes = 8
        }

        public string BuildAuthUrl (string clientId, string callback, Scope scopes)
        {
            var scopeStr = scopes.ToString ().ToLower ().Replace (", ", "+");
            return String.Format ("https://api.instagram.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}&response_type=token",
                                  clientId,
                                  callback,
                                  scopeStr);
        }

#if !WINDOWS_PHONE && !WINRT
        private T Execute<T>(IRestRequest request) where T : new()
        {
            IRestResponse<T> response;

            response = _restClient.Execute<T>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InstagramException(response);
            }

            return response.Data;
        }

        private IRestResponse Execute(IRestRequest request)
        {
            IRestResponse response;

            response = _restClient.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InstagramException(response);
            }

            return response;
        }
#endif

        protected void ExecuteAsync(IRestRequest request, Action<IRestResponse> success, Action<InstagramException> failure)
        {
#if WINDOWS_PHONE
            //check for network connection
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                //do nothing
                failure(new DropboxException
                {
                    StatusCode = System.Net.HttpStatusCode.BadGateway
                });
                return;
            }
#endif
            _restClient.ExecuteAsync(request, (response, asynchandle) =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    failure(new InstagramException(response));
                }
                else
                {
                    success(response);
                }
            });
        }

        private void ExecuteAsync<T>(IRestRequest request, Action<T> success, Action<InstagramException> failure) where T : new()
        {
#if WINDOWS_PHONE
            //check for network connection
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                //do nothing
                failure(new DropboxException
                {
                    StatusCode = System.Net.HttpStatusCode.BadGateway
                });
                return;
            }
#endif
            _restClient.ExecuteAsync<T>(request, (response, asynchandle) =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    failure(new InstagramException(response));
                }
                else
                {
                    success(response.Data);
                }
            });
        }

        protected UserLogin GetUserLoginFromParams(string urlParams)
        {
            var userLogin = new UserLogin();

            //TODO - Make this not suck
            var parameters = urlParams.Split('&');

            foreach (var parameter in parameters)
            {
                var keyVal = parameter.Split('=');
                switch (keyVal[0]) {
                case "uid":
                    userLogin.Uid = keyVal[1];
                    break;
                case "oauth_token":
                    userLogin.Token = keyVal[1];
                    break;
                }
            }

            return userLogin;
        }

        private void SetAuthProviders ()
        {
            if (UserLogin != null)
            {
                //Set the OauthAuthenticator only when the UserLogin property changes
                _restClient.Authenticator = new InstaNet.Authenticators.OAuth2Authenticator (UserLogin.Token);
            }
        }

        public event EventHandler <UserLoginEventArgs> LoggedIn;

        public void Auth () 
        {
            /** /
            var dbUrl = "dbapi-1://1/connect";
            if (UIApplication.SharedApplication.CanOpenUrl (NSUrl.FromString (dbUrl))) {
                var appUrl = String.Format ("{0}?k={1}&s={2}{3}", dbUrl, _apiKey, _appsecret, "");
                UIApplication.SharedApplication.OpenUrl (NSUrl.FromString (appUrl));
            } else {
                GetTokenAsync ((ul) => {
                    var userLogin = GetAccessToken ();
                    OnLoggedIn (userLogin);
                }, (ex) => {
                    Debug.WriteLine (ex.Message);
                });
            }
            /**/
        }

        private void OnLoggedIn (Models.UserLogin userLogin)
        {
            if (LoggedIn != null)
                LoggedIn (this, new UserLoginEventArgs (userLogin));
        }

        public bool HandleOpenUrl (string url)
        {
            var schema = "ig-" + _apiKey + "://";
            if (url.StartsWith (schema)) {
                var parms = url.Split ('?')[1];
                var userLogin = GetUserLoginFromParams (parms);

                if (!String.IsNullOrWhiteSpace (userLogin.Token)) {
                    UserLogin = userLogin;

                    OnLoggedIn (userLogin);
                }

                return true;
            }

            return false;
        }
    }

    public class UserLoginEventArgs : EventArgs 
    {
        public Models.UserLogin UserLogin { get; private set; }

        public UserLoginEventArgs (Models.UserLogin userLogin)
        {
            UserLogin = userLogin;
        }
    }
}

