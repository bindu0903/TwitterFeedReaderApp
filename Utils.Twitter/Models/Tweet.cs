using System;

namespace Utils.Twitter.Models
{
    public class Tweet
    {
        public long Id { get; set; }
        public string CreatedAt { get; set; }
        public string UserName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ScreenName { get; set; }
        public string Text { get; set; }
        public int RetweetCount { get; set; }
        public object[] Urls { get; set; }

        public object[] Media { get; set; }

        public string ProfileBannerUrl { get; set; }

    }
}