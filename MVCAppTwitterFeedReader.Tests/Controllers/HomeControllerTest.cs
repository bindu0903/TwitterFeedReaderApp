using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCAppTwitterFeedReader;
using MVCAppTwitterFeedReader.Controllers;
using System.Threading;
using Utils.Twitter.Models;
using Utils.Twitter;
using System.IO;
using System.Xml.Serialization;
using static Utils.Twitter.TwitterHelper;

namespace MVCAppTwitterFeedReader.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void WriteTweetAndReadTimeline_GivenUser_AreSame()
        {
            var twitter = CreateTwitterHelper();

            var status = Guid.NewGuid().ToString();
            twitter.UpdateStatus(status);

            // Wait a little to let twitter update timelines
            Thread.Sleep(TimeSpan.FromSeconds(3));

            var tweets = twitter.GetHomeTimeline();
            Assert.AreEqual(status, tweets.First().Text);
        }

        [TestMethod]
        public void Write_Many_Tweets()
        {
            // Just to show the problem with HttpWebRequests and TimeoutExceptions

            var twitter = CreateTwitterHelper();

            for (var i = 0; i < 5; i++)
            {
                twitter.UpdateStatus(Guid.NewGuid().ToString());
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }


        private static OAuthInfo GetOAuthInfo()
        {
            var tokensFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OAuthInfo.xml");

            if (!File.Exists(tokensFile))
                Assert.Fail("Cannot find oauth parameter file. Add a file named OAuthInfo.xml to the project with your own OAuth tokens. You can find a sample in sample.OAuthInfo.xml");

            var serializer = new XmlSerializer(typeof(OAuthInfo));
            using (var stream = File.OpenRead(tokensFile))
                return (OAuthInfo)serializer.Deserialize(stream);
        }

        private static TwitterHelper CreateTwitterHelper()
        {
            var oauth = GetOAuthInfo();

            var twitter = new TwitterHelper(oauth);
            return twitter;
        }

    }
}
