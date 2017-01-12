using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public class SauceLabsWebDriver : RemoteWebDriver, ICustomRemoteWebDriver
    {
        private new string SessionId => base.SessionId.ToString();
        public string SecretUser { get; }
        public string SecretKey { get; }
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

        public void UpdateTestResult()
        {
            var passed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
            ((IJavaScriptExecutor)Browser.Current).ExecuteScript("sauce:test-result=" + (passed ? "passed" : "failed"));
        }

        string ICustomRemoteWebDriver.SessionId => SessionId;

        private static Dictionary<string, string> Auth(string userName, string accessKey, Dictionary<string, string> capabilities)
        {
            if (userName == null) throw new Exception("saucelabs.username can't be found");
            if (accessKey == null) throw new Exception("saucelabs.accessKey can't be found");
            capabilities.Add("username", userName);
            capabilities.Add("accessKey", accessKey);
            capabilities.Add("name", TestContext.CurrentContext.Test.Name);
            return capabilities;
        }

       
    }
}
