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
        FacebookDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            string prefix = ConfigurationManager.AppSettings["mode"];
            string mRedirectUri = ConfigurationManager.AppSettings[prefix + "_redirectUri"];
            string clientId = ConfigurationManager.AppSettings[prefix + "_clientId"];
            string clientSecret = ConfigurationManager.AppSettings[prefix + "_clientSecret"];
            GridView1.PageIndexChanging += GridView1_PageIndexChanging;

            FacebookAuthentication auth = new FacebookAuthentication(clientId, clientSecret, mRedirectUri);
            string accessToken = auth.Login(Request.Params["code"]);

            var fb = new FacebookClient(accessToken);
            db = new FacebookDataContext(fb);
            FqlToLinqSample();
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