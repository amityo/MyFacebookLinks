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
using System.Globalization;

namespace test
{
    public partial class _Default : System.Web.UI.Page
    {
        FacebookDataContext db;

        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.PageIndexChanging += GridView1_PageIndexChanging;

            string prefix = ConfigurationManager.AppSettings["mode"];
            string mRedirectUri = ConfigurationManager.AppSettings[prefix + "_redirectUri"];
            string clientId = ConfigurationManager.AppSettings[prefix + "_clientId"];
            string clientSecret = ConfigurationManager.AppSettings[prefix + "_clientSecret"];

            FacebookAuthentication auth = new FacebookAuthentication(clientId, clientSecret, mRedirectUri);
            string accessToken = auth.Login(Request.Params["code"]);

            var fb = new FacebookClient(accessToken);
            db = new FacebookDataContext(fb);
            if (!IsPostBack)
            {
                QueryFriendList();
                QueryLinks();
            }
        }

        private void QueryFriendList()
        {
            var friendUids = from friend in db.Friend
                             where friend.Uid1 == db.Me
                             select friend.Uid2;

            var names = (from user in db.User
                         where friendUids.Contains(user.Uid)
                         select new { user.Uid, user.FirstName, user.LastName }).ToDictionary(x => x.Uid.Value, x => x.FirstName + " " + x.LastName);

            ViewState.Add("users", names);
        }

        public void RowDataBounded(object sender, GridViewRowEventArgs eventArgs)
        {
            if (eventArgs.Row.RowType == DataControlRowType.DataRow)
            {
                ExtendedLink elink = eventArgs.Row.DataItem as ExtendedLink;
                Label likes = eventArgs.Row.FindControl("likes") as Label;
                HtmlGenericControl tooltipDiv = eventArgs.Row.FindControl("tooltip") as HtmlGenericControl;

                if (elink.Like == null)
                {
                    var likeQuery = (from like in db.Like
                                     where like.ObjectId == new ObjectId(elink.LinkId.Value)
                                     select like.UserId).ToList();

                    var users = (Dictionary<string, string>)ViewState["users"];
                    string str = string.Empty;
                    likeQuery.ForEach(x => str += users[x.Value] + " " + "<br/>");
                    elink.Like = new LikeString(str, likeQuery.Count);
                }
                //var users = (from user in db.User where likeQuery.Contains(user.Uid) select new { user.FirstName, user.LastName }).ToList();

                likes.Text = elink.Like.Count.ToString();
                if (elink.Like.Count > 0)
                    tooltipDiv.InnerHtml = elink.Like.ToString();
                else
                    tooltipDiv.Visible = false;
            }
        }

        private void QueryLinks()
        {
            var links = (from link in db.Link
                         where link.Owner == db.Me
                         select link).ToList();

            var extended = (from link in links
                            orderby link.CreatedTime descending
                            select new ExtendedLink(link)).ToList();



            ManipulateLinks(extended);

            ViewState.Add("dataSource", extended);
            ViewState.Add("currentDataSource", extended);

            BindGrid(extended, 0);
        }

        private void ManipulateLinks(IEnumerable<ExtendedLink> links)
        {

            foreach (var item in links)
            {
                if (item.Url.Contains("gdata"))
                {
                    item.Url = "http://www.youtube.com/watch?v=" + item.Url.Split('/').Last();
                }
            }
        }


        void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            object source = ViewState["currentDataSource"];
            BindGrid(source, e.NewPageIndex);
        }

        public void SerachUrlClick(object sender, EventArgs eventArgs)
        {
            List<ExtendedLink> list = (List<ExtendedLink>)ViewState["dataSource"];
            List<ExtendedLink> currentList = FilterBySearch(list, searchDDL.Text);
            if (currentList != null)
            {
                ViewState.Add("currentDataSource", currentList);
                BindGrid(currentList, 0);
            }
        }

        private List<ExtendedLink> FilterBySearch(List<ExtendedLink> source, string filterBy)
        {
            if (filterBy == "URL")
            {
                return source.Where(x => x.Url.IndexOf(searchBox.Text, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            else if (filterBy == "Title")
            {
                return source.Where(x => x.Title.IndexOf(searchBox.Text, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            return null;
        }
        public void AllClicked(object sender, EventArgs eventArgs)
        {
            searchBox.Text = string.Empty;
            object originalSource = ViewState["dataSource"];
            ViewState.Add("currentDataSource", originalSource);
            BindGrid(originalSource, 0);
        }

        private void BindGrid(object dataSource, int index)
        {
            GridView1.DataSource = dataSource;
            GridView1.PageIndex = index;
            GridView1.DataBind();
        }

        protected void Stats_Click(object sender, EventArgs e)
        {
            List<ExtendedLink> originalSource = (List<ExtendedLink>)ViewState["dataSource"];
            
            stats.InnerHtml = "you have " + originalSource.Count + " links";

            System.Threading.Thread.Sleep(2000);
        }
    }
}