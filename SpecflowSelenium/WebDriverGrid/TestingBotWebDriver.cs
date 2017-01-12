using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public class TestingBotWebDriver : RemoteWebDriver, ICustomRemoteWebDriver
    {
        public new string SessionId => base.SessionId.ToString();
        public string SecretUser { get; }
        public string SecretKey { get; }
        private const string ApiUrl = "http://hub.testingbot.com/wd/hub/";

        public TestingBotWebDriver(string browser, string key, string secret, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(key, secret, capabilities))
        {
            SecretUser = key;
            SecretKey = secret;
        }

        private static readonly string TestingBotKey = ConfigurationManager.AppSettings["testingbot.key"];
        private static readonly string TestingBotSecret = ConfigurationManager.AppSettings["testingbot.secret"];

        public TestingBotWebDriver(string browser, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(TestingBotKey, TestingBotSecret, capabilities))
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
            capabilities.Add("name", TestContext.CurrentContext.Test.Name);
            return capabilities;
        }

        public void UpdateTestResult()
        {
            var passed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
            ((IJavaScriptExecutor)Browser.Current).ExecuteScript("tb:test-result=" + (passed ? "passed" : "failed"));
        }
    }
}
