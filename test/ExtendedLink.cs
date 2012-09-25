using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using facebook.Tables;

namespace test
{
    [Serializable]
    public class ExtendedLink
    {
        public ExtendedLink(Link link)
        {
            mLink = link;
        }
        private Link mLink;

        public string Picture { get { return mLink.Picture; } }
        public string Url { get { return mLink.Url; } set { mLink.Url = value; } }
        public string OwnerComment { get { return mLink.OwnerComment; } }
        public DateTime? CreatedTime { get { return mLink.CreatedTime; } }
        public string Summary { get { return mLink.Summary; } }
        public string Title { get { return mLink.Title; } }
        public LinkId LinkId { get { return mLink.LinkId;} }
        public UserLikes Like { get; set; }
        public string FacebookLink { get { return "http://facebook.com/" + mLink.LinkId.Value; } }
    }
    [Serializable]
    public class UserLikes
    {
        private string mUsersHtml;
        public UserLikes(List<string> users)
        {
            Users = users;
            mUsersHtml = string.Empty;
        }
        public string UserHtml()
        {
            if (mUsersHtml == string.Empty)
            {
                mUsersHtml = GetUsersHtml();
            }
            return mUsersHtml;
        }
        public int Count { get { return Users.Count; } }
        public List<string> Users { get; private set; }

        private string GetUsersHtml()
        {
            string str = string.Empty; ;
            Users.ForEach(x => str += x + "</br>");
            return str;
        }
        
    }

    public static class LinkExtensions
    {
        public static void FixYoutubeLinks(this IEnumerable<ExtendedLink> links)
        {
            foreach (var item in links)
            {
                if (item.Url.Contains("gdata"))
                {
                    item.Url = "http://www.youtube.com/watch?v=" + item.Url.Split('/').Last();
                }
            }
        }

        public static IEnumerable<ExtendedLink> FilterBy(this IEnumerable<ExtendedLink> source,string text, string filter)
        {
            if (filter == "URL")
            {
                return source.Where(x => x.Url.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1);
            }
            else if (filter == "Title")
            {
                return source.Where(x => x.Title.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1);
            }
            return null;
        }
    }
}