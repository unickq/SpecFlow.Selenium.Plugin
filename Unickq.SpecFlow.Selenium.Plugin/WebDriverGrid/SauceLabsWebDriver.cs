using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unickq.SpecFlow.Selenium.WebDriverGrid
{
    public class SauceLabsWebDriver : PaidWebDriver
    {
        private const string ApiUrl = "http://ondemand.saucelabs.com:80/wd/hub";

        private static readonly string SaucelabsUser = ConfigurationManager.AppSettings["saucelabs.username"];
        private static readonly string SaucelabsKey = ConfigurationManager.AppSettings["saucelabs.accessKey"];

        public SauceLabsWebDriver(string browser, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(SaucelabsUser, SaucelabsKey, capabilities))
        {
            SecretUser = SaucelabsUser;
            SecretKey = SaucelabsKey;
        }

        public SauceLabsWebDriver(string browser, string userName, string accessKey, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(userName, accessKey, capabilities))
        {
            SecretUser = userName;
            SecretKey = accessKey;
        }

        public SauceLabsWebDriver(string userName, string accessKey, Dictionary<string, string> capabilities)
            : base(ApiUrl, Auth(userName, accessKey, capabilities))
        {
            SecretUser = userName;
            SecretKey = accessKey;
        }

        public SauceLabsWebDriver(Dictionary<string, string> capabilities)
            : base(ApiUrl, Auth(SaucelabsUser, SaucelabsKey, capabilities))
        {
            SecretUser = SaucelabsUser;
            SecretKey = SaucelabsKey;
        }

        public override void UpdateTestResult()
        {
            var passed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
            Publish("{\"passed\":"+ passed.ToString().ToLower() + "}");                 
        }

        private static Dictionary<string, string> Auth(string userName, string accessKey, Dictionary<string, string> capabilities)
        {
            if (userName == null) throw new Exception("saucelabs.username can't be found");
            if (accessKey == null) throw new Exception("saucelabs.accessKey can't be found");
            capabilities.Add("username", userName);
            capabilities.Add("accessKey", accessKey);

            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (key.StartsWith("saucelabs.", StringComparison.InvariantCultureIgnoreCase))
                {
                    var capabilityName = key.Replace("saucelabs.", string.Empty);
                    var capabilityValue = ConfigurationManager.AppSettings[key];
                    capabilities.Add(capabilityName, NameTransform(capabilityValue));
                }
            }

            if (!capabilities.ContainsKey("name"))
                capabilities.Add("name", FixedTestName);

            return capabilities;
        }

        protected override Uri Uri => new Uri($"https://saucelabs.com/rest/v1/{SecretUser}/jobs/{SessionId}");
    }
}
