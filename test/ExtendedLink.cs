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
        public LikeString Like { get; set; }
    }
    [Serializable]
    public class LikeString
    {
        public LikeString(string likes,int count)
        {
            mLike = likes;
            Count = count;
        }
        private string mLike;
        public override string ToString()
        {
            return mLike;
        }
        public int Count { get; set; }
    }
}