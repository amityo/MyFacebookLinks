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
            List<Uid> unknown = new List<Uid>();
            List<string> names = GetUsersNames(uids,out unknown);
            if (unknown.Count > 0)
            {
                names.AddRange(GetNotFriendsNames(unknown));
            }
            return names;

        }

        #region Private Methods

        private List<string> GetNotFriendsNames(List<Uid> unknownUids)
        {
            List<string> uidsStrings = unknownUids.Select(x => x.Value).ToList();
            List<string> names = new List<string>();

            string concats = string.Join(", ", uidsStrings);
            string query = string.Format("select uid,first_name,last_name from user where uid in({0})", concats);
            var result = QueryFql(query);
            for (int i = 0; i < result.Count; i++)
            {
                string name = result[i]["first_name"] + " " + result[i]["last_name"];
                string id = result[i]["uid"].ToString();
                Friends.Add(id, name);
                names.Add(name);
            }
            return names;
        }

        private List<Uid> GetUsersLikeByLink(ExtendedLink link)
        {
            return (from like in LinfQuery.Like
                    where like.ObjectId == new ObjectId(link.LinkId.Value)
                    select like.UserId).ToList();
        }



        private List<string> GetUsersNames(List<Uid> uids,out List<Uid> unknown)
        {
            List<string> names = new List<string>();
            List<Uid> unknown1 = new List<Uid>();
            uids.ForEach(fid => 
            {
                string name = GetNameByFId(fid.Value);
                if(name == string.Empty)
                {
                    unknown1.Add(fid);
                }
                else
                {
                    names.Add(name);
                }
            });
            unknown = unknown1;
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