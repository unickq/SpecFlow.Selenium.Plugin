using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unickq.SpecFlow.Selenium.WebDriverGrid
{
    public class BrowserStackWebDriver : PaidWebDriver
    {
        private const string ApiUrl = "http://hub-cloud.browserstack.com/wd/hub/";
        protected override Uri Uri => new Uri($"https://www.browserstack.com/automate/sessions/{SessionId}.json");

        private static readonly string BrowserstackUser = ConfigurationManager.AppSettings["browserstack.user"];
        private static readonly string BrowserstackKey = ConfigurationManager.AppSettings["browserstack.key"];

        public BrowserStackWebDriver(string browser, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(BrowserstackUser, BrowserstackKey, capabilities))
        {
            SecretUser = BrowserstackUser;
            SecretKey = BrowserstackKey;
        }

        public BrowserStackWebDriver(string browserstackUser, string browserstackKey, Dictionary<string, string> capabilities)
            : base(ApiUrl, Auth(browserstackUser, browserstackKey, capabilities))
        {
            SecretUser = browserstackUser;
            SecretKey = browserstackKey;
        }

        public BrowserStackWebDriver(Dictionary<string, string> capabilities)
            : base(ApiUrl, Auth(BrowserstackUser, BrowserstackKey, capabilities))
        {
            SecretUser = BrowserstackUser;
            SecretKey = BrowserstackKey;
        }

        public BrowserStackWebDriver(string browser, string browserstackUser, string browserstackKey,
            Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(browserstackUser, browserstackKey, capabilities))
        {
            SecretUser = browserstackUser;
            SecretKey = browserstackKey;
        }

        private static Dictionary<string, string> Auth(string browserstackUser, string browserstackKey,
            Dictionary<string, string> capabilities)
        {
            capabilities.Add("browserstack.user", browserstackUser);
            capabilities.Add("browserstack.key", browserstackKey);

            var list = new List<string>
            {
                "resolution",
                "build",
                "deviceOrientation",
                "project",
                "name",
                "acceptSslCerts"
            };
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (key.StartsWith("browserstack.", StringComparison.InvariantCultureIgnoreCase))
                {
                    var capabilityName = key.Replace("browserstack.", string.Empty);
                    var capabilityValue = ConfigurationManager.AppSettings[key];
                    if (!list.Contains(capabilityName))
                        capabilityName = key;
                    if (!capabilities.ContainsKey(capabilityName))
                        capabilities.Add(capabilityName, NameTransform(capabilityValue));
                }
            }
            if (!capabilities.ContainsKey("name"))
                capabilities.Add("name", FixedTestName);

            return capabilities;
        }

        public override string Name => "BrowserStack";

        public override void UpdateTestResult()
        {
            var testResult = TestContext.CurrentContext.Result;
            var resultStr = "failed";
            if (testResult.Outcome.Status == TestStatus.Passed)
            {
                resultStr = "passed";
            }

            dynamic statusObj = new ExpandoObject();
            statusObj.status = resultStr;
            statusObj.reason = testResult.Message;
            var settings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };
            var json = JsonConvert.SerializeObject(statusObj, settings);
            try
            {
                Publish(json);
            }
            catch (Exception)
            {
                Publish($"{{\"status\":\"{resultStr}\", \"reason\":\"Unparsable reason\"}}");
                Console.WriteLine($"Unparsable json for BrowserStack:\n{json}");
            }
        }
    }
}