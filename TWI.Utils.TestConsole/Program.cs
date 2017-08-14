using System;
using System.Configuration;
using System.IO;
using Utils.Twitter;

namespace TWI.Utils.TestConsole
{
    class Program
    {
        public static string ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        public static string ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        public static string AccessToken = ConfigurationManager.AppSettings["AccessToken"];
        public static string AccessTokenSecret = ConfigurationManager.AppSettings["AccessTokenSecret"];

        
        static void Main(string[] args)
        {
           var twitter = new TwitterHelper();
            var tweets = twitter.GetUserTimeline(null, null, 20, "SalesForce");
            foreach (var tweet in tweets)
            {
                Console.WriteLine(string.Format(@"{0} alias {1} posted tweet {2}: {3}", tweet.UserName, tweet.ScreenName, tweet.Id, tweet.Text));
            }
            Console.WriteLine();
            Console.ReadKey(); 
            //var twitter = new TwitterHelper();
            //var tweets = twitter.GetUserTimeline();
            //foreach(var tweet in tweets)
            //{
            //    Console.WriteLine(tweet.Text);
            //}
            //Console.WriteLine();
            //Console.ReadKey();
        }
    }
}
