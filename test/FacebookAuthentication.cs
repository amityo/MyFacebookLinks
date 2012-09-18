using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;

namespace test
{
    public class FacebookAuthentication
    {
        #region Data Members

        private string mClientId;
        private string mClientSecret;
        private string mRedirectUri;
        private string mScope = "read_stream";
        private HttpContext mPage;
        #endregion

        #region Initialize

        public FacebookAuthentication(string clientId, string clientSecret,string redirectUri)
        {
            mClientId = clientId;
            mClientSecret = clientSecret;
            mRedirectUri = GetRedirectUri(redirectUri);
            mPage = HttpContext.Current;
        }

        private string GetRedirectUri(string redirectUri)
        {
            return HttpUtility.UrlEncode(redirectUri);
        }
        #endregion

        /// <summary>
        /// login to facebook user and returns token to access the user's facebook information
        /// </summary>
        /// <param name="code"></param>
        /// <returns>the access token</returns>
        public string Login(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                RedirectEmptyCode();
            }

            if (mPage.Session["access_token"] == null)
            {
                string access_token = GetAccessToken(code);
                mPage.Session.Add("access_token", access_token);
            }

            return mPage.Session["access_token"].ToString();
        }

        #region Private Methods

        private void RedirectEmptyCode()
        {
            string dialog_url = string.Format("http://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope={2}",
                            mClientId, mRedirectUri, mScope);
            mPage.Response.Redirect(dialog_url);
        }
        private string GetAccessToken(string code)
        {
            string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&scope={4}",
                            mClientId, mRedirectUri, mClientSecret, code, mScope);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using(StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseString = reader.ReadToEnd();

                string key = "access_token=";
                int index = responseString.IndexOf(key) + key.Length;
                int count = responseString.IndexOf("&", index) - index;

                return responseString.Substring(index, count);
            }
        }
        #endregion


    }
}