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
using System.Text;

namespace test
{
    public partial class _Default : System.Web.UI.Page
    {
        FacebookQuerier mQuerier;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.PageIndexChanging += GridView1_PageIndexChanging;

            string prefix = ConfigurationManager.AppSettings["mode"];
            string mRedirectUri = ConfigurationManager.AppSettings[prefix + "_redirectUri"];
            string clientId = ConfigurationManager.AppSettings[prefix + "_clientId"];
            string clientSecret = ConfigurationManager.AppSettings[prefix + "_clientSecret"];

            FacebookAuthentication auth = new FacebookAuthentication(clientId, clientSecret, mRedirectUri);
            string accessToken = auth.Login(Request.Params["code"]);
            //string accessToken = auth.Login("AQCHwpz8JfD6pKWoE7BpKFI9PE0PBOR4ofhgZLH4jT9fRbozX_dnle-VqMIWwNaiB5HmQag_wzSRKXyOFyUMdAy48YP7kyoGjj9Uf8e2qpwe2mHn4gl1aResQ10ZgEJY9eon530AMSYG5O9W8O4W9tARsZi0AjY8KGZhmDOM9TcOkksDwaLlgMs5tcdBX501aTPRQMtl7X1ccXrC5n6hDaKv");
            mQuerier = new FacebookQuerier(accessToken);

            if (ViewState["users"] == null)
            {
                var friends = mQuerier.FriendsList();
                ViewState.Add("users", friends);
            }
            else
            {
                mQuerier.Friends = (Dictionary<string,string>)ViewState["users"];
            }


            if (ViewState["dataSource"] == null)
            {
                var links = mQuerier.Links();
                ViewState.Add("dataSource", links);
                ViewState.Add("currentDataSource", links);
                BindGrid(links, 0);
            }
            else
            {
                mQuerier.LinksSource = (List<ExtendedLink>)ViewState["dataSource"];
            }
        }

        

        public void RowDataBounded(object sender, GridViewRowEventArgs eventArgs)
        {
            if (eventArgs.Row.RowType == DataControlRowType.DataRow)
            {
                ExtendedLink link = eventArgs.Row.DataItem as ExtendedLink;
                Label likes = eventArgs.Row.FindControl("likes") as Label;
                HtmlGenericControl tooltipDiv = eventArgs.Row.FindControl("tooltip") as HtmlGenericControl;

                if (link.Like == null)
                {
                    //var userslikes = mQuerier.GetNamesOfUsersThatLikedTheLink(link);
                    //link.Like = new UserLikes(userslikes);
                    Console.WriteLine();
                }
                likes.Text = link.Like.Count.ToString();
                if (link.Like.Count > 0)
                    tooltipDiv.InnerHtml = link.Like.UserHtml();
                else
                    tooltipDiv.Visible = false;
            }
        }
        void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            object source = ViewState["currentDataSource"];
            BindGrid(source, e.NewPageIndex);
        }

        public void SerachUrlClick(object sender, EventArgs eventArgs)
        {
            List<ExtendedLink> currentList = mQuerier.LinksSource.FilterBy(searchBox.Text, searchDDL.Text).ToList();
            if (currentList != null)
            {
                ViewState.Add("currentDataSource", currentList);
                BindGrid(currentList, 0);
            }
        }

       

        public void AllClicked(object sender, EventArgs eventArgs)
        {
            searchBox.Text = string.Empty;
            ViewState.Add("currentDataSource", mQuerier.LinksSource);
            BindGrid(mQuerier.LinksSource, 0);
        }

        private void InitLikes(List<ExtendedLink> source,int index)
        {
            var LinkToInit = new List<ExtendedLink>();
            for (int i = index * 10; i < index * 10 + 10; i++)
            {
                if (source[i].Like == null)
                {
                    LinkToInit.Add(source[i]);
                }
            }
            mQuerier.GetNamesOfUsersThatLikedTheLink2(LinkToInit);
            
        }

        private void BindGrid(object dataSource, int index)
        {
            InitLikes(dataSource as List<ExtendedLink>, index);
            GridView1.DataSource = dataSource;
            GridView1.PageIndex = index;
            GridView1.DataBind();
        }

        protected void Stats_Click(object sender, EventArgs e)
        {
            string statistics = string.Empty;
            if (ViewState["stats"] != null)
            {
                statistics = ViewState["stats"].ToString();
            }
            else
            {
                StatisticsBuilder statisticsBuilder = new StatisticsBuilder(mQuerier);
                statistics = statisticsBuilder.Build();
                ViewState["stats"] = statistics;
            }
            stats.InnerHtml = statistics;
        }
    }
}