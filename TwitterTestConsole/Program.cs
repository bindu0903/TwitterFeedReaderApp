using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Twitter;

namespace TwitterTestConsole
{
    public class Program
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
        }
    }
}
