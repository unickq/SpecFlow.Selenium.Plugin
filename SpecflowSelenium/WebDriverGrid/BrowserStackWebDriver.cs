using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public class BrowserStackWebDriver : RemoteWebDriver, ICustomRemoteWebDriver
    {
        public new string SessionId => base.SessionId.ToString();
        public string SecretUser { get; }
        public string SecretKey { get; }
        private const string ApiUrl = "http://hub-cloud.browserstack.com/wd/hub/";

        private static readonly string BrowserstackUser = ConfigurationManager.AppSettings["browserstack.user"];
        private static readonly string BrowserstackKey = ConfigurationManager.AppSettings["browserstack.key"];

        public BrowserStackWebDriver(string browser, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(BrowserstackUser, BrowserstackKey, capabilities))
        {
            SecretUser = BrowserstackUser;
            SecretKey = BrowserstackKey;
        }

        public BrowserStackWebDriver(string browser, string browserstackUser, string browserstackKey, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(browserstackUser, browserstackKey, capabilities))
        {
            SecretUser = browserstackUser;
            SecretKey = browserstackKey;
        }

        private static Dictionary<string, string> Auth(string browserstackUser, string browserstackKey, Dictionary<string, string> capabilities)
        {
            if (browserstackUser == null) throw new Exception("browserstack.user can't be found");
            if (browserstackKey == null) throw new Exception("browserstack.key can't be found");
            capabilities.Add("browserstack.user", browserstackUser);
            capabilities.Add("browserstack.key", browserstackKey);
            capabilities.Add("name", TestContext.CurrentContext.Test.Name);
            return capabilities;
        }

        public void UpdateTestResult()
        {
            var result = TestContext.CurrentContext.Result;
            var resultStr = "failed";
            if (result.Outcome.Status == TestStatus.Passed)
            {
                resultStr = "passed";
            }
            var reason = $"{result.Outcome.Status} {result.Message}";
            var reqString = $"{{\"status\":\"{resultStr}\", \"reason\":\"{reason}\"}}";
            var uri = new Uri($"https://www.browserstack.com/automate/sessions/{SessionId}.json");
            var requestData = Encoding.UTF8.GetBytes(reqString);
            var myWebRequest = WebRequest.Create(uri);
            var myHttpWebRequest = (HttpWebRequest)myWebRequest;
            myWebRequest.ContentType = "application/json";
            myWebRequest.Method = "PUT";
            myWebRequest.ContentLength = requestData.Length;
            using (var st = myWebRequest.GetRequestStream()) st.Write(requestData, 0, requestData.Length);
            var networkCredential = new NetworkCredential(SecretUser, SecretKey);
            var myCredentialCache = new CredentialCache {{uri, "Basic", networkCredential}};
            myHttpWebRequest.PreAuthenticate = true;
            myHttpWebRequest.Credentials = myCredentialCache;
            myWebRequest.GetResponse().Close();
        }

        public void SetTestName()
        {
        }
    }
}