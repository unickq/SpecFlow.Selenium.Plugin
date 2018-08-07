using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unickq.SpecFlow.Selenium.WebDriverGrid
{
    public class TestingBotWebDriver : PaidWebDriver
    {
        private const string ApiUrl = "http://hub.testingbot.com/wd/hub/";
        protected override Uri Uri => new Uri($"https://api.testingbot.com/v1/tests/{SessionId}");

        private static readonly string TestingBotKey = ConfigurationManager.AppSettings["testingbot.key"];
        private static readonly string TestingBotSecret = ConfigurationManager.AppSettings["testingbot.secret"];

        public TestingBotWebDriver(string browser, string key, string secret, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(key, secret, capabilities))
        {
            SecretUser = key;
            SecretKey = secret;
        }
        public TestingBotWebDriver(string browser, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(TestingBotKey, TestingBotSecret, capabilities))
        {
            SecretUser = TestingBotKey;
            SecretKey = TestingBotSecret;
        }

        public TestingBotWebDriver(string key, string secret, Dictionary<string, string> capabilities)
            : base(ApiUrl, Auth(key, secret, capabilities))
        {
            SecretUser = key;
            SecretKey = secret;
        }

        public TestingBotWebDriver(Dictionary<string, string> capabilities)
            : base(ApiUrl, Auth(TestingBotKey, TestingBotSecret, capabilities))
        {
            SecretUser = TestingBotKey;
            SecretKey = TestingBotSecret;
        }

        private static Dictionary<string, string> Auth(string key, string secret, Dictionary<string, string> capabilities)
        {
            if (key == null) throw new Exception("testingbot.key can't be found");
            if (secret == null) throw new Exception("testingbot.secret can't be found");
            capabilities.Add("key", key);
            capabilities.Add("secret", secret);

            foreach (var configKey in ConfigurationManager.AppSettings.AllKeys)
            {
                if (configKey.StartsWith("testingbot.", StringComparison.InvariantCultureIgnoreCase))
                {
                    var capabilityName = configKey.Replace("testingbot.", string.Empty);
                    var capabilityValue = ConfigurationManager.AppSettings[configKey];
                    capabilities.Add(capabilityName, NameTransform(capabilityValue));
                }
            }

            if (!capabilities.ContainsKey("name"))
                capabilities.Add("name", FixedTestName);

            return capabilities;
        }


        public override void UpdateTestResult()
        {
            var success = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
            var request = (HttpWebRequest)WebRequest.Create(Uri);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "PUT";
            var usernamePassword = SecretUser + ":" + SecretKey;
            var mycache = new CredentialCache
            {
                {
                    Uri, "Basic",
                    new NetworkCredential(SecretUser, SecretKey)
                }
            };
            request.Credentials = mycache;
            request.Headers.Add("Authorization",
                "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(usernamePassword)));

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(
                    "test[success]=" + (success ? "1" : "0") +
                    "&test[status_message]=" + TestContext.CurrentContext.Result.Message);
            }
            var response = request.GetResponse();
            response.Close();
        }
    }
}
