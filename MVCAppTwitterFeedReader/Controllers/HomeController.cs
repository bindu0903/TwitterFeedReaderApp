using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utils.Twitter;
using Utils.Twitter.Models;

namespace MVCAppTwitterFeedReader.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var twitter = new TwitterHelper();
            var tweets = twitter.GetUserTimeline(null, null, 10, "SalesForce");
            ViewBag.Tweets = tweets;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}