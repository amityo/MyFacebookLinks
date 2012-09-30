using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using facebook;
using Facebook;
using facebook.Tables;
namespace test
{
    public class FacebookQuerier
    {
        private FacebookClient mFacebookClient;
        private FacebookDataContext mFacebookDataContext;

        public Dictionary<string, string> Friends { get; set; }
        public List<ExtendedLink> LinksSource { get; set; }

        private FacebookClient FqlQuery
        {
            get
            {
                return mFacebookClient;
            }
        }
        public FacebookDataContext LinfQuery
        {
            get
            {
                return mFacebookDataContext;
            }
        }

        public FacebookQuerier(string accessToken)
        {
             mFacebookClient = new FacebookClient(accessToken);
             mFacebookDataContext = new FacebookDataContext(mFacebookClient);
        }

        public Dictionary<string,string> FriendsList()
        {
            var friendUids = from friend in LinfQuery.Friend
                             where friend.Uid1 == LinfQuery.Me
                             select friend.Uid2;

            var friends = (from user in LinfQuery.User
                         where friendUids.Contains(user.Uid)
                         select new { user.Uid, user.FirstName, user.LastName }).ToDictionary(x => x.Uid.Value, x => x.FirstName + " " + x.LastName);
            Friends = friends;
            return friends;
        }

        public List<ExtendedLink> Links()
        {
            var links = (from link in LinfQuery.Link
                         where link.Owner == LinfQuery.Me
                         select link).ToList();

            var extended = (from link in links
                            orderby link.CreatedTime descending
                            select new ExtendedLink(link)).ToList();

            extended.FixYoutubeLinks();
            LinksSource = extended;
            return extended;
        }

        public void AllLikes()
        {
            LinksSource.ForEach(x =>
            {
                if (x.Like == null)
                {
                    x.Like = new UserLikes(GetNamesOfUsersThatLikedTheLink(x));
                }
            });
        }


        public List<string> GetNamesOfUsersThatLikedTheLink(ExtendedLink link)
        {
            var uids = GetUsersLikeByLink(link);
            return GetUsersNames(uids);

        }

        #region Private Methods
       

        private List<Uid> GetUsersLikeByLink(ExtendedLink link)
        {
            return (from like in LinfQuery.Like
                    where like.ObjectId == new ObjectId(link.LinkId.Value)
                    select like.UserId).ToList();
        }



        private List<string> GetUsersNames(List<Uid> uids)
        {
            List<string> names = new List<string>();
            uids.ForEach(fid => names.Add(GetNameByFId(fid.Value)));
            return names;
        }

        public string GetNameByFId(string fid)
        {
            string result;
            if(Friends.TryGetValue(fid,out result))
            {
               return result;
            }
            return string.Empty;
        }

        public dynamic QueryFql(string query)
        {
            dynamic result = FqlQuery.Get("fql", new { q = query });
            return result["data"];

        }
        #endregion
       
    }

   
}