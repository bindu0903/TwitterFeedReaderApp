using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Twitter.Models
{
    public class TwitterUser
    {
        public long ID;
        public string Name;
        public string ScreenName;
        public string Location;
        public string Description;
        public string ProfileImage;
        public string Url;
        public bool IsProtected;
        public long FollowersCount;
        public long FriendsCount;
        public string CreatedAt;
        public long FavoritesCount;
        public bool Verified;
        public bool Following;
        public long StatusCount;
    }
}
