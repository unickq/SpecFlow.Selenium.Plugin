using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public class BrowserStackWebDriver : PaidWebDriver
    {
        private const string ApiUrl = "http://hub-cloud.browserstack.com/wd/hub/";
        protected override Uri Uri => new Uri($"https://www.browserstack.com/automate/sessions/{SessionId}.json");

        private static readonly string BrowserstackUser = ConfigurationManager.AppSettings["browserstack.user"];
        private static readonly string BrowserstackKey = ConfigurationManager.AppSettings["browserstack.key"];
        private static readonly string Name = ConfigurationManager.AppSettings["browserstack.name"];
        private static readonly string Resolution = ConfigurationManager.AppSettings["browserstack.resolution"];
        private static readonly string Project = ConfigurationManager.AppSettings["browserstack.project"];
        private static string Build = ConfigurationManager.AppSettings["browserstack.build"];
        private static readonly string Debug = ConfigurationManager.AppSettings["browserstack.debug"];
        private static readonly string Video = ConfigurationManager.AppSettings["browserstack.video"];
        private static readonly string Local = ConfigurationManager.AppSettings["browserstack.local"];
        private static readonly string Timezone = ConfigurationManager.AppSettings["browserstack.timezone"];
        private static readonly string LocalEnv = ConfigurationManager.AppSettings["browserstack.localEnv"];

        private static readonly string LocalIdentifier =
            ConfigurationManager.AppSettings["browserstack.localIdentifier"];

        private static readonly string DeviceOrientation =
            ConfigurationManager.AppSettings["browserstack.deviceOrientation"];

        private static readonly string SeleniumVersion =
            ConfigurationManager.AppSettings["browserstack.selenium_version"];

        private static readonly string NoFlash = ConfigurationManager.AppSettings["browserstack.ie.noFlash"];

        private static readonly string Compatibility =
            ConfigurationManager.AppSettings["browserstack.ie.compatibility"];

        private static readonly string Driver = ConfigurationManager.AppSettings["browserstack.ie.driver"];

        private static readonly string IeEnablePopups =
            ConfigurationManager.AppSettings["browserstack.ie.enablePopups"];

        private static readonly string EdgeEnablePopups =
            ConfigurationManager.AppSettings["browserstack.edge.enablePopups"];

        private static readonly string SafariEnablePopups =
            ConfigurationManager.AppSettings["browserstack.safari.enablePopups"];

        private static readonly string SafariAllowAllCookies =
            ConfigurationManager.AppSettings["browserstack.safari.allowAllCookies"];

        private static readonly string SafariDriver = ConfigurationManager.AppSettings["browserstack.safari.driver"];
        private static readonly string NetworkLogs = ConfigurationManager.AppSettings["browserstack.networkLogs"];


        public BrowserStackWebDriver(string browser, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(BrowserstackUser, BrowserstackKey, capabilities))
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

            capabilities.Add("name",
                !string.IsNullOrEmpty(Name)
                    ? Name
                    : FixedTestName);

            Build = BuildTransform(Build);

            if (!string.IsNullOrEmpty(Resolution)) capabilities.Add("resolution", Resolution);
            if (!string.IsNullOrEmpty(Build)) capabilities.Add("build", Build);
            if (!string.IsNullOrEmpty(Project)) capabilities.Add("project", Project);
            if (!string.IsNullOrEmpty(Debug)) capabilities.Add("browserstack.debug", Debug);
            if (!string.IsNullOrEmpty(Video)) capabilities.Add("browserstack.video", Video);
            if (!string.IsNullOrEmpty(DeviceOrientation)) capabilities.Add("deviceOrientation", DeviceOrientation);
            if (!string.IsNullOrEmpty(SeleniumVersion))
                capabilities.Add("browserstack.selenium_version", SeleniumVersion);
            if (!string.IsNullOrEmpty(Timezone)) capabilities.Add("browserstack.timezone", Timezone);
            if (!string.IsNullOrEmpty(NoFlash)) capabilities.Add("browserstack.ie.noFlash", NoFlash);
            if (!string.IsNullOrEmpty(Compatibility)) capabilities.Add("browserstack.ie.compatibility", Compatibility);
            if (!string.IsNullOrEmpty(Driver)) capabilities.Add("browserstack.ie.driver", Driver);
            if (!string.IsNullOrEmpty(IeEnablePopups)) capabilities.Add("browserstack.ie.enablePopups", IeEnablePopups);
            if (!string.IsNullOrEmpty(EdgeEnablePopups))
                capabilities.Add("browserstack.edge.enablePopups", EdgeEnablePopups);
            if (!string.IsNullOrEmpty(SafariEnablePopups))
                capabilities.Add("browserstack.safari.enablePopups", SafariEnablePopups);
            if (!string.IsNullOrEmpty(SafariAllowAllCookies))
                capabilities.Add("browserstack.safari.allowAllCookies", SafariAllowAllCookies);
            if (!string.IsNullOrEmpty(SafariDriver)) capabilities.Add("browserstack.safari.driver", SafariDriver);
            if (!string.IsNullOrEmpty(NetworkLogs)) capabilities.Add("browserstack.networkLogs", NetworkLogs);


            if (!string.IsNullOrEmpty(LocalEnv))
            {
                var envLocalId = Environment.GetEnvironmentVariable("BROWSERSTACK_LOCAL_IDENTIFIER");
                if (string.IsNullOrEmpty(envLocalId)) throw new Exception("Unable to find BROWSERSTACK_LOCAL_IDENTIFIER env variable");
                capabilities.Add("browserstack.local", "true");
                capabilities.Add("browserstack.localIdentifier", envLocalId);
            }
            else
            {
                if (!string.IsNullOrEmpty(Local)) capabilities.Add("browserstack.local", Local);
                if (!string.IsNullOrEmpty(LocalIdentifier))
                    capabilities.Add("browserstack.localIdentifier", LocalIdentifier);
            }

                capabilities.Add("browserstack.networkLogs", NetworkLogs);
            return capabilities;
        }

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