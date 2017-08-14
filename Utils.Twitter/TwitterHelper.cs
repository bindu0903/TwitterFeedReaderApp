using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Utils.Twitter.Models;

namespace Utils.Twitter
{
    public class TwitterHelper
    {
        private readonly OAuthInfo oauth;

        public TwitterHelper()
        {
            var tokensFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OAuthInfo.xml");

            if (File.Exists(tokensFile))
            {
                var serializer = new XmlSerializer(typeof(OAuthInfo));
                using (var stream = File.OpenRead(tokensFile))
                    this.oauth = (OAuthInfo)serializer.Deserialize(stream);
            }

        }

        public TwitterHelper(OAuthInfo oauth)
        {
            this.oauth = oauth;
        }

        public void UpdateStatus(string message)
        {
            new RequestBuilder(
                oauth,
                "POST",
                "https://api.twitter.com/1.1/statuses/update.json"
            ).AddParameter("status", message)
            .Execute();
        }

        public IEnumerable<Tweet> GetHomeTimeline(long? sinceId = null, long? maxId = null, int? count = 20)
        {
            return GetTimeline("https://api.twitter.com/1.1/statuses/home_timeline.json", sinceId, maxId, count, "");
        }

        public IEnumerable<Tweet> GetMentions(long? sinceId = null, long? maxId = null, int? count = 20)
        {
            return GetTimeline("https://api.twitter.com/1.1/statuses/mentions.json", sinceId, maxId, count, "");
        }

        public IEnumerable<Tweet> GetUserTimeline(long? sinceId = null, long? maxId = null, int? count = 20, string screenName = "")
        {
            return GetTimeline("https://api.twitter.com/1.1/statuses/user_timeline.json", sinceId, maxId, count, screenName);
        }

        private IEnumerable<Tweet> GetTimeline(string url, long? sinceId, long? maxId, int? count, string screenName)
        {
            var builder = new RequestBuilder(oauth, "GET", url);

            if (sinceId.HasValue)
                builder.AddParameter("since_id", sinceId.Value.ToString());

            if (maxId.HasValue)
                builder.AddParameter("max_id", maxId.Value.ToString());

            if (count.HasValue)
                builder.AddParameter("count", count.Value.ToString());

            if (screenName != "")
                builder.AddParameter("screen_name", screenName);

            var responseContent = builder.Execute();

            var serializer = new JavaScriptSerializer();

            var tweets = (object[])serializer.DeserializeObject(responseContent);




            return tweets.Cast<Dictionary<string, object>>().Select(tweet =>
            {
                var user = ((Dictionary<string, object>)tweet["user"]);
                var entities = ((Dictionary<string, object>)tweet["entities"]);
                var date = DateTime.ParseExact(tweet["created_at"].ToString(),
                    "ddd MMM dd HH:mm:ss zz00 yyyy",
                    CultureInfo.InvariantCulture).ToLocalTime();

              

                return new Tweet
                {
                    Id = (long)tweet["id"],
                    CreatedAt =date.ToString(),
                    Text = (string)tweet["text"],
                    RetweetCount = (int)tweet["retweet_count"],
                    UserName = (string)user["name"],
                    ScreenName = (string)user["screen_name"],
                    ProfileImageUrl = (string)user["profile_image_url"],
                    ProfileBannerUrl = (string)user["profile_banner_url"],
                    //Urls = entities["urls"] == null ? new object[0] : (object[])entities["urls"],
                    //Media = entities["media"] == null ? new object[0] : (object[])entities["media"]
                };
            }).ToArray();
        }

        #region RequestBuilder

        public class RequestBuilder
        {
            private const string VERSION = "1.0";
            private const string SIGNATURE_METHOD = "HMAC-SHA1";

            private readonly OAuthInfo oauth;
            private readonly string method;
            private readonly IDictionary<string, string> customParameters;
            private readonly string url;

            public RequestBuilder(OAuthInfo oauth, string method, string url)
            {
                this.oauth = oauth;
                this.method = method;
                this.url = url;
                customParameters = new Dictionary<string, string>();
            }

            public RequestBuilder AddParameter(string name, string value)
            {
                customParameters.Add(name, value.EncodeRFC3986());
                return this;
            }

            public string Execute()
            {
                var timespan = GetTimestamp();
                var nonce = CreateNonce();

                var parameters = new Dictionary<string, string>(customParameters);
                AddOAuthParameters(parameters, timespan, nonce);

                var signature = GenerateSignature(parameters);
                var headerValue = GenerateAuthorizationHeaderValue(parameters, signature);

                var request = (HttpWebRequest)WebRequest.Create(GetRequestUrl());
                request.Method = method;
                request.ContentType = "application/x-www-form-urlencoded";

                request.Headers.Add("Authorization", headerValue);

                WriteRequestBody(request);

                // It looks like a bug in HttpWebRequest. It throws random TimeoutExceptions
                // after some requests. Abort the request seems to work. More info:
                // http://stackoverflow.com/questions/2252762/getrequeststream-throws-timeout-exception-randomly

                var response = request.GetResponse();

                string content;

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }
                }

                request.Abort();

                return content;
            }

            private void WriteRequestBody(HttpWebRequest request)
            {
                if (method == "GET")
                    return;

                var requestBody = Encoding.ASCII.GetBytes(GetCustomParametersString());
                using (var stream = request.GetRequestStream())
                    stream.Write(requestBody, 0, requestBody.Length);
            }

            private string GetRequestUrl()
            {
                if (method != "GET" || customParameters.Count == 0)
                    return url;

                return string.Format("{0}?{1}", url, GetCustomParametersString());
            }

            private string GetCustomParametersString()
            {
                return customParameters.Select(x => string.Format("{0}={1}", x.Key, x.Value)).Join("&");
            }

            private string GenerateAuthorizationHeaderValue(IEnumerable<KeyValuePair<string, string>> parameters, string signature)
            {
                return new StringBuilder("OAuth ")
                    .Append(parameters.Concat(new KeyValuePair<string, string>("oauth_signature", signature))
                                .Where(x => x.Key.StartsWith("oauth_"))
                                .Select(x => string.Format("{0}=\"{1}\"", x.Key, x.Value.EncodeRFC3986()))
                                .Join(","))
                    .ToString();
            }

            private string GenerateSignature(IEnumerable<KeyValuePair<string, string>> parameters)
            {
                var dataToSign = new StringBuilder()
                    .Append(method).Append("&")
                    .Append(url.EncodeRFC3986()).Append("&")
                    .Append(parameters
                                .OrderBy(x => x.Key)
                                .Select(x => string.Format("{0}={1}", x.Key, x.Value))
                                .Join("&")
                                .EncodeRFC3986());

                var signatureKey = string.Format("{0}&{1}", oauth.ConsumerSecret.EncodeRFC3986(), oauth.AccessSecret.EncodeRFC3986());
                var sha1 = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));

                var signatureBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(dataToSign.ToString()));
                return Convert.ToBase64String(signatureBytes);
            }

            private void AddOAuthParameters(IDictionary<string, string> parameters, string timestamp, string nonce)
            {
                parameters.Add("oauth_version", VERSION);
                parameters.Add("oauth_consumer_key", oauth.ConsumerKey);
                parameters.Add("oauth_nonce", nonce);
                parameters.Add("oauth_signature_method", SIGNATURE_METHOD);
                parameters.Add("oauth_timestamp", timestamp);
                parameters.Add("oauth_token", oauth.AccessToken);
            }

            private static string GetTimestamp()
            {
                return ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            }

            private static string CreateNonce()
            {
                return new Random().Next(0x0000000, 0x7fffffff).ToString("X8");
            }
        }

        #endregion RequestBuilder
    }
}
