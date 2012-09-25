using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace test
{

    public class StatisticsBuilder
    {
        private List<ExtendedLink> mSource;
        private List<Action<StringBuilder>> mTasks;
        private Html mHtmlBuilder;
        public StatisticsBuilder(List<ExtendedLink> source)
        {
            mSource = source;
            mHtmlBuilder = new Html(new StringBuilder());
            mTasks = new List<Action<StringBuilder>>()
                {
                    Count,
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

        private void Count(StringBuilder builder)
        {
            builder.Append("you published <strong>");
            builder.Append(mSource.Count);
            builder.Append("</strong> links from <strong>");

            var urls = (from link in mSource
                        select new Uri(link.Url).GetLeftPart(UriPartial.Authority)).GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase);

            builder.Append(urls.Count());
            builder.Append(" </strong> different sites");
        }
        private void Last(StringBuilder builder)
        {
            ExtendedLink first = mSource.Last();
            builder.Append("your latest link is <strong><a target='_blank' href='");
            builder.Append(first.Url);
            builder.Append("'>");
            builder.Append(first.Title);
            builder.Append("</a>");
            builder.Append("</strong>  from <strong>");
            builder.Append(first.CreatedTime.Value.ToShortDateString());
            builder.Append("</strong>");
        }
        private void First(StringBuilder builder)
        {
            ExtendedLink last = mSource.First();
            builder.Append("your first link is <strong><a target='_blank' href='");
            builder.Append(last.Url);
            builder.Append("'>");
            builder.Append(last.Title);
            builder.Append("</a>");
            builder.Append("</strong>  from <strong>");
            builder.Append(last.CreatedTime.Value.ToShortDateString());
            builder.Append("</strong>");
        }

        private void MostPublishedSites(StringBuilder builder)
        {
            var urls = (from link in mSource
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