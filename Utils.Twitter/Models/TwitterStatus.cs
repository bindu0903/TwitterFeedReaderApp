using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Twitter.Models
{
    public class TwitterStatus
    {
        public string CreatedAt;
        public long ID;
        public string Text;
        public string Source;
        public bool Truncated;
        public long InReplyToStatusID;
        public long InReplyToUserID;
        public bool Favorited;
        public string InReplyToScreenName;
        public TwitterUser User;
    }
}
