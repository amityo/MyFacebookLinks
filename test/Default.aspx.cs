using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facebook;
using System.Net;
using System.IO;

namespace test
{
    public partial class _Default : System.Web.UI.Page
    {
        string m_clientId = "348858645200865";
        string m_clientSecret = "b6a16ea47534cf2ef2c2f56b68d8ebea";
        string m_scope = "read_stream";

        // the getting the token code is from here: http://multitiered.wordpress.com/tag/c/

        private static string GetRedirectUri()
        {
            return HttpUtility.UrlEncode("http://linkbrowsing.apphb.com/");


            var url = HttpContext.Current.Request.Url;
            string urll = url.GetLeftPart(UriPartial.Path).Split(':')[0] + url.GetLeftPart(UriPartial.Path).Split(':')[1];
            string redirectUri = HttpUtility.UrlEncode(urll + "/");
            return redirectUri;
        }

        private string GetAccessToken()
        {
            if (Session["access_token"] == null)
            {
                Dictionary<string, string> args = GetOauthTokens(Request.Params["code"]);
                Session.Add("access_token", args["access_token"]);
            }

            return Session["access_token"].ToString();
        }


        private Dictionary<string, string> GetOauthTokens(string code)
        {
            Dictionary<string, string> tokens = new Dictionary<string, string>();

            string redirectUri = GetRedirectUri();

            string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&scope={4}",
                            m_clientId, redirectUri, m_clientSecret, code, m_scope);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {

                StreamReader reader = new StreamReader(response.GetResponseStream());

                string retVal = reader.ReadToEnd();

                foreach (string token in retVal.Split('&'))
                {
                    tokens.Add(token.Substring(0, token.IndexOf("=")),
                    token.Substring(token.IndexOf("=") + 1, token.Length - token.IndexOf("=") - 1));
                }
            }

            return tokens;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
            GridView1.PageIndexChanging += GridView1_PageIndexChanging;

            if (Request.Params["code"] == null)
            {
                string redirectUri = GetRedirectUri();

                string dialog_url = string.Format("http://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope={2}",
                                m_clientId, redirectUri, m_scope);
                Response.Redirect(dialog_url);
            }
            else
            {
                FqlToLinqSample();
            }
        }

        private void FqlToLinqSample()
        {
            var fb = new FacebookClient(GetAccessToken());
            var db = new FacebookDataContext(fb);
            
            var links = (from link in db.Link
                        where link.Owner == db.Me.Value
                        select link).ToList();


            var youtube = from link in links
                         where link.Url.Contains("youtube") 
                         orderby link.CreatedTime descending
                         select link;

            var list = youtube.ToList();
            list.ForEach(x =>
            {
                if (x.Url.Contains("gdata"))
                {
                    x.Url = "http://www.youtube.com/watch?v=" + x.Url.Split('/').Last();
                }
            });

            var fixedList = (from link in youtube
                             select new { link.LinkId, link.OwnerComment, link.CreatedTime, link.Title, link.Summary, link.Url, link.Picture }).ToList();
            GridView1.DataSource = fixedList;
            GridView1.DataBind();
        }

        void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GridView1.DataBind();
        }

    }
}