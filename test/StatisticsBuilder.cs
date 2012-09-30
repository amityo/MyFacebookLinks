using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.CSharp;
using Facebook;
namespace test
{

    public class StatisticsBuilder
    {
        private FacebookQuerier mQuerier;
        private List<Action<StringBuilder>> mTasks;
        private Html mHtmlBuilder;
        public StatisticsBuilder(FacebookQuerier querier)
        {
            mQuerier = querier;
            mHtmlBuilder = new Html(new StringBuilder());
            mTasks = new List<Action<StringBuilder>>()
                {
                    LinksCount,
                    TotalAndDifferentUsersLikes,
                    Last,
                    First,
                    MostPublishedSites,
                    TopUsersThatLiked,
                    MostLikedLink
                };

        }

        public string Build()
        {
            if (mQuerier.LinksSource.Count == 0)
            {
                return "you don't have any links :(";
            }
            mHtmlBuilder.StartULTag(); 
            mHtmlBuilder.AddAttribute("class", "stats");
            foreach (var item in mTasks)
            {
                mHtmlBuilder.StartLITag();
                {
                    item(mHtmlBuilder.Builder);
                }
                mHtmlBuilder.EndLITag();
            }
            mHtmlBuilder.EndULTag();
            return mHtmlBuilder.ToString();
        }

        private void LinksCount(StringBuilder builder)
        {
            builder.Append("you published <strong>");
            builder.Append(mQuerier.LinksSource.Count);
            builder.Append("</strong> links from <strong>");


            var links = new List<string>();
            foreach (var link in mQuerier.LinksSource)
            {
                try     
                {
                    Uri uri = new Uri(link.Url);
                    links.Add(uri.GetLeftPart(UriPartial.Authority));
                }
                catch
                {
                }
            }
            var urls = links.GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase);

            //var urls = (from link in mQuerier.LinksSource
            //            select new Uri(link.Url).GetLeftPart(UriPartial.Authority)).GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase);
            if (urls != null)
            {
                builder.Append(urls.Count());
                builder.Append(" </strong> different sites");
            }
        }


        private void First(StringBuilder builder)
        {
            ExtendedLink first = mQuerier.LinksSource.Last();
            builder.Append("your first link is <strong><a target='_blank' href='");
            builder.Append(first.Url);
            builder.Append("'>");
            builder.Append(first.Title);
            builder.Append("</a>");
            builder.Append("</strong>  from <strong>");
            builder.Append(first.CreatedTime.Value.ToShortDateString());
            builder.Append("</strong>");
        }
        private void Last(StringBuilder builder)
        {
            ExtendedLink last = mQuerier.LinksSource.First();
            builder.Append("your latest link is <strong><a target='_blank' href='");
            builder.Append(last.Url);
            builder.Append("'>");
            builder.Append(last.Title);
            builder.Append("</a>");
            builder.Append("</strong>  from <strong>");
            builder.Append(last.CreatedTime.Value.ToShortDateString());
            builder.Append("</strong>");
        }

        private void OrderByMonth(StringBuilder builder)
        {
            //var grouped = from link in mQuerier.LinksSource
            //              group link by new { link.CreatedTime.Value.Year, link.CreatedTime.Value.Month } into temp
            //              orderby temp.Count() descending
            //              select new {temp.Key.Month,temp.Key.Year,temp,Count = temp.Count()};
        }

        private List<object> mUserLikesIds;
        private List<object> GetUsersThatLiked()
        {
            if (mUserLikesIds == null)
            {
                JsonArray users = mQuerier.QueryFql("select user_id from like where object_id in (select link_id from link where owner=me())");
                List<object> total = new List<object>();
                foreach (JsonObject item in users)
                {
                    total.AddRange(item.Values);
                }
                mUserLikesIds = total;
            }
            return mUserLikesIds;
        }
        
        private void TotalAndDifferentUsersLikes(StringBuilder builder)
        {
            var users = GetUsersThatLiked();
            if (users == null || users.Count == 0)
            {
                return;
            }
            builder.Append("you received a total of <strong>");
            builder.Append(users.Count);
            builder.Append("</strong> likes from <strong>");
            builder.Append(users.Distinct().Count());
            builder.Append(" </strong> different users");
        }

        public void TopUsersThatLiked(StringBuilder builder)
        {
            var users = GetUsersThatLiked();
            if (users == null || users.Count == 0)
            {
                return;
            }
            var groupedUsers = users.GroupBy(x => x).Select(x => new { x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).ToList();

            int to = 0;
            if (groupedUsers == null || groupedUsers.Count == 0)
            {
                return;

            }
            else if (groupedUsers.Count > 0 && groupedUsers.Count < 3)
            {
                to = groupedUsers.Count + 1;
            }
            else
            {
                to = 4;
            }


            builder.Append("top users that liked your links: ");
            mHtmlBuilder.For(1, to, i =>
                {
                    builder.Append(i);
                    builder.Append(". <strong>");
                    builder.Append(mQuerier.GetNameByFId(groupedUsers[i-1].Key.ToString()));
                    builder.Append("</strong> - <strong>");
                    builder.Append(groupedUsers[i - 1].Count);
                    builder.Append("</strong> times");
                });
        }

        private void MostPublishedSites(StringBuilder builder)
        {
            var links = new List<string>();
            foreach (var link in mQuerier.LinksSource)
            {
                Uri uri;
                try
                {
                    uri = new Uri(link.Url);
                }
                catch
                {
                    continue;
                }
                links.Add(uri.GetLeftPart(UriPartial.Authority));
            }
            var urls = links.GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase);
            //var urls = (from link in mQuerier.LinksSource
            //            select new Uri(link.Url).GetLeftPart(UriPartial.Authority)).GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase);
            var urlsOrderByCount = (from url in urls
                                    select new { Url = url.Key, Count = url.Count() }
                                        into urlss
                                        orderby urlss.Count descending
                                        select urlss).ToList();
            int to = 0;
            if (urlsOrderByCount == null || urlsOrderByCount.Count == 0)
            {
                return;
                
            }
            else if (urlsOrderByCount.Count > 0 && urlsOrderByCount.Count < 3)
            {
                to = urlsOrderByCount.Count + 1;
            }
            else
            {
                to = 4;
            }
            builder.Append("most links from: ");
            mHtmlBuilder.For(1, to, i =>
            {
                var currentUrl = urlsOrderByCount[i - 1];
                if (currentUrl.Count == 1)
                {
                    int oneUrl = urlsOrderByCount.Where(x => x.Count == 1).Count();
                    builder.Append("<strong>");
                    builder.Append(oneUrl);
                    builder.Append("</strong>");
                    builder.Append(" sites with <strong>1</strong> link");
                    return;
                }
                else
                {
                    builder.Append("<strong>");
                    builder.Append(currentUrl.Count);
                    builder.Append(" </strong> links from ");
                    builder.Append("<strong><a target='_blank' href='");
                    builder.Append(currentUrl.Url);
                    builder.Append("'>");
                    builder.Append(currentUrl.Url);
                    builder.Append("</a></strong>");
                }
            });
        }
        private void MostLikedLink(StringBuilder builder)
        {
            JsonArray users = mQuerier.QueryFql("select object_id from like where object_id in (select link_id from link where owner=me())");
            List<object> total = new List<object>();
            foreach (JsonObject item in users)
            {
                total.AddRange(item.Values);
            }
            var most = total.GroupBy(x => x).Select(x => new { Key = x.Key.ToString(), Count = x.Count() }).OrderByDescending(x=>x.Count);
            if (most == null || most.Count() == 0)
            {
                return;
            }
            var most2 = most.First();

            var mostLikedLink = mQuerier.LinksSource.First(x => x.LinkId.Value == most2.Key);
            if (mostLikedLink == null)
            {
                return;
            }

            builder.Append("most liked link is: <strong>");
            builder.Append("<a  target='_blank' href='");
            builder.Append(mostLikedLink.Url);
            builder.Append("'>");
            builder.Append(mostLikedLink.Title);
            builder.Append("</a></strong> with <strong>");
            builder.Append(most2.Count);
            builder.Append(" <strong>likes");

        }
    }
}