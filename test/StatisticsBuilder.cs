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
                    MostPublishedSites
                };

        }

        public string Build()
        {
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

            var urls = (from link in mQuerier.LinksSource
                        select new Uri(link.Url).GetLeftPart(UriPartial.Authority)).GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase);

            builder.Append(urls.Count());
            builder.Append(" </strong> different sites");
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

        private void TotalAndDifferentUsersLikes(StringBuilder builder)
        {
            JsonArray result = mQuerier.QueryFql("select user_id from like where object_id in (select link_id from link where owner=me())");

            List<object> total = new List<object>();
            foreach (JsonObject item in result)
            {
                total.Add(item.Values.ElementAt(0));
            }
            

            builder.Append("you received a total of <strong>");
            builder.Append(result.Count);
            builder.Append("</strong> likes from <strong>");
            builder.Append(total.Distinct().Count());
            builder.Append(" </strong> different users");
        }

        private void MostPublishedSites(StringBuilder builder)
        {
            var urls = (from link in mQuerier.LinksSource
                        select new Uri(link.Url).GetLeftPart(UriPartial.Authority)).GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase);
            var urlsOrderByCount = (from url in urls
                                    select new { Url = url.Key, Count = url.Count() }
                                        into urlss
                                        orderby urlss.Count descending
                                        select urlss).ToList();

            builder.Append("most links from: ");
            mHtmlBuilder.StartULTag();
            {
                for (int i = 1; i < 4; i++)
                {
                    mHtmlBuilder.StartLITag();
                    {
                        var currentUrl = urlsOrderByCount[i - 1];
                        if (currentUrl.Count == 1)
                        {
                            int oneUrl = urlsOrderByCount.Where(x => x.Count == 1).Count();
                            builder.Append("<strong>");
                            builder.Append(oneUrl);
                            builder.Append("</strong>");
                            builder.Append(" sites with <strong>1</strong> link");
                            break;
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
                    }
                    mHtmlBuilder.EndLITag();

                }
            }
            mHtmlBuilder.EndULTag();
        }
    }
}