using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public class CrossBrowserTestingWebDriver : CustomRemoteWebDriver
    {
        private const string ApiUrl = "http://hub.crossbrowsertesting.com:80/wd/hub";
        protected override Uri Uri => new Uri("https://crossbrowsertesting.com/api/v3/selenium");

        private static readonly string CbtUser = ConfigurationManager.AppSettings["cbt.user"];
        private static readonly string CbtKey = ConfigurationManager.AppSettings["cbt.key"];

        private static readonly string Name = ConfigurationManager.AppSettings["cbt.name"];
        private static readonly string Resolution = ConfigurationManager.AppSettings["cbt.screen_resolution"];
        private static string Build = ConfigurationManager.AppSettings["cbt.build"];
        private static readonly string Browser = ConfigurationManager.AppSettings["cbt.browser_api_name"];
        private static readonly string Os = ConfigurationManager.AppSettings["cbt.os_api_name"];
        private static readonly string RecordVideo = ConfigurationManager.AppSettings["cbt.record_video"];

        public CrossBrowserTestingWebDriver(string browser, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(CbtUser, CbtKey, capabilities))
        {
            SecretUser = CbtUser;
            SecretKey = CbtKey;
        }

        public CrossBrowserTestingWebDriver(string browser, string cbtUser, string cbtKey,
            Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(cbtUser, cbtKey, capabilities))
        {
            SecretUser = CbtUser;
            SecretKey = CbtKey;
        }

        private static Dictionary<string, string> Auth(string cbtUser, string cbtKey,
            Dictionary<string, string> capabilities)
        {
            if (cbtUser == null) throw new Exception("cbt.user can't be found");
            if (cbtKey == null) throw new Exception("cbt.key can't be found");
            capabilities.Add("username", cbtUser);
            capabilities.Add("password", cbtKey);

            capabilities.Add("name",
                !string.IsNullOrEmpty(Name)
                    ? Name
                    : TestContext.CurrentContext.Test.Name);

            Build = BuildTransform(Build);

            if (!string.IsNullOrEmpty(Resolution)) capabilities.Add("screen_resolution", Resolution);
            if (!string.IsNullOrEmpty(Build)) capabilities.Add("build", Build);
            if (!string.IsNullOrEmpty(Browser)) capabilities.Add("browser_api_name", Browser);
            if (!string.IsNullOrEmpty(Os)) capabilities.Add("os_api_name", Os);
            if (!string.IsNullOrEmpty(RecordVideo)) capabilities.Add("record_video", RecordVideo);
            return capabilities;
        }

        public override void UpdateTestResult()
        {
            var passed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
            var result = "fail";
            if (passed) result = "pass";
            var cbtApi = new CbtApi(Uri.AbsoluteUri, CbtUser, CbtKey);
            cbtApi.SetScore(SessionId.ToString(), result);
            cbtApi.SetDescription(SessionId.ToString(), cbtApi.TakeSnapshot(SessionId.ToString()), TestContext.CurrentContext.Result.Message);
        }
    }

    public class CbtApi
    {
        private readonly string _cbtUser;
        private readonly string _cbtKey;
        private readonly string _cbtApiUrl;

        public CbtApi(string cbtApiUrl, string cbtUser, string cbtKey)
        {
            _cbtUser = cbtUser;
            _cbtKey = cbtKey;
            _cbtApiUrl = cbtApiUrl;
        }

        private HttpWebRequest GetCommonRequest(string url, string method)
        {
            Console.WriteLine(url);
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method;
            request.Credentials = new NetworkCredential(_cbtUser, _cbtKey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "HttpWebRequest";
            return request;
        }

        public string TakeSnapshot(string sessionId)
        {
            var request = GetCommonRequest(_cbtApiUrl + "/" + sessionId + "/snapshots", "POST");
            var response = (HttpWebResponse) request.GetResponse();
            var stream = response.GetResponseStream();
            if (stream == null) return null;
            var responseString = new StreamReader(stream).ReadToEnd();
            var myregex = new Regex("(?<=\"hash\": \")((\\w|\\d)*)");
            var snapshotHash = myregex.Match(responseString).Value;
            Console.WriteLine(snapshotHash);
            request.GetResponse().Close();
            return snapshotHash;
        }

        public void SetDescription(string sessionId, string snapshotHash, string description)
        {
            var encoding = new ASCIIEncoding();
            var putData = encoding.GetBytes("description=" + description);
            // create the request
            var request = GetCommonRequest(_cbtApiUrl + "/" + sessionId + "/snapshots/" + snapshotHash, "PUT");
            // write data to stream
            var stream = request.GetRequestStream();
            stream.Write(putData, 0, putData.Length);
            stream.Close();
            request.GetResponse().Close();
        }

        public void SetScore(string sessionId, string score)
        {
            var encoding = new ASCIIEncoding();
            var data = "action=set_score&score=" + score;
            var putdata = encoding.GetBytes(data);
            var request = GetCommonRequest(_cbtApiUrl + "/" + sessionId, "PUT");
            var newStream = request.GetRequestStream();
            newStream.Write(putdata, 0, putdata.Length);
            request.GetResponse().Close();
            newStream.Close();
        }
    }
}