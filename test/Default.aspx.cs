using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Facebook;
using System.Net;
using System.IO;
using System.Configuration;
using facebook.Tables;

namespace test
{
    public partial class _Default : System.Web.UI.Page
    {
        string m_clientId;
        string m_clientSecret;
        string m_redirectUri ="";
        string m_scope = "read_stream";
        FacebookDataContext db;
        // the getting the token code is from here: http://multitiered.wordpress.com/tag/c/

        private string GetRedirectUri()
        {
            return HttpUtility.UrlEncode(m_redirectUri);


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
            string prefix = ConfigurationManager.AppSettings["mode"];
            m_redirectUri = ConfigurationManager.AppSettings[prefix + "_redirectUri"];
            m_clientId= ConfigurationManager.AppSettings[prefix + "_clientId"];
            m_clientSecret = ConfigurationManager.AppSettings[prefix + "_clientSecret"];


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
                var fb = new FacebookClient(GetAccessToken());
                db = new FacebookDataContext(fb);
                FqlToLinqSample();
            }
        }

        public void RowDataBounded(object sender, GridViewRowEventArgs eventArgs)
        {
            if (eventArgs.Row.RowType == DataControlRowType.DataRow)
            {
                Link dataitem = eventArgs.Row.DataItem as Link;
                Label likes = eventArgs.Row.FindControl("likes") as Label;
                HtmlGenericControl tooltipDiv = eventArgs.Row.FindControl("tooltip") as HtmlGenericControl;

                var likeQuery = from like in db.Like
                                where like.ObjectId == new ObjectId(dataitem.LinkId.Value)
                                select like.UserId;

                var users = (from user in db.User where likeQuery.Contains(user.Uid) select new {user.FirstName,user.LastName}).ToList();

                string str = "";
                users.ForEach(x => str += x.FirstName + " " + x.LastName + "<br/>");
                //users.ToList().ForEach(x=>str += x.Name + Environment.NewLine);
                likes.Text = users.Count.ToString();
                if (users.Count > 0)
                    tooltipDiv.InnerHtml = str;
                else
                    tooltipDiv.Visible = false;
            }
        }

        private void FqlToLinqSample()
        {
            
            
            var links = (from link in db.Link
                        where link.Owner == db.Me
                        select link).ToList();


            var youtube = from link in links
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

            GridView1.DataSource = list;
            GridView1.DataBind();
        }


        void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GridView1.DataBind();
        }

    }
}