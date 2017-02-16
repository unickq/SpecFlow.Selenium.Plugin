using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        private static readonly string Name = ConfigurationManager.AppSettings["browserstack.name"];
        private static readonly string Resolution = ConfigurationManager.AppSettings["browserstack.resolution"];
        private static readonly string Project = ConfigurationManager.AppSettings["browserstack.project"];
        private static string Build = ConfigurationManager.AppSettings["browserstack.build"];
        private static readonly string Debug = ConfigurationManager.AppSettings["browserstack.debug"];
        private static readonly string Video = ConfigurationManager.AppSettings["browserstack.video"];
        private static readonly string Local = ConfigurationManager.AppSettings["browserstack.local"];
        private static readonly string Timezone = ConfigurationManager.AppSettings["browserstack.timezone"];

        private static readonly string LocalIdentifier =
            ConfigurationManager.AppSettings["browserstack.localIdentifier"];

        private static readonly string DeviceOrientation =
            ConfigurationManager.AppSettings["browserstack.deviceOrientation"];

        private static readonly string SeleniumVersion =
            ConfigurationManager.AppSettings["browserstack.selenium_version"];

        private static readonly string NoFlash = ConfigurationManager.AppSettings["browserstack.ie.noFlash"];
        private static readonly string Compatibility = ConfigurationManager.AppSettings["browserstack.ie.compatibility"];
        private static readonly string Driver = ConfigurationManager.AppSettings["browserstack.ie.driver"];
        private static readonly string IeEnablePopups = ConfigurationManager.AppSettings["browserstack.ie.enablePopups"];

        private static readonly string SafariEnablePopups =
            ConfigurationManager.AppSettings["browserstack.safari.enablePopups"];

        private static readonly string SafariAllowAllCookies =
            ConfigurationManager.AppSettings["browserstack.safari.allowAllCookies"];

        private static readonly string SafariDriver = ConfigurationManager.AppSettings["browserstack.safari.driver"];

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
            if (browserstackUser == null) throw new Exception("browserstack.user can't be found");
            if (browserstackKey == null) throw new Exception("browserstack.key can't be found");
            capabilities.Add("browserstack.user", browserstackUser);
            capabilities.Add("browserstack.key", browserstackKey);

            capabilities.Add("name",
                !string.IsNullOrEmpty(Name)
                    ? Name
                    : TestContext.CurrentContext.Test.Name);

            if (Build.Equals("@@debug")) Build = DateTime.Now.ToString("yyyy/MM/dd hhtt");

            if (!string.IsNullOrEmpty(Resolution)) capabilities.Add("browserstack.resolution", Resolution);
            if (!string.IsNullOrEmpty(Build)) capabilities.Add("build", Build);
            if (!string.IsNullOrEmpty(Project)) capabilities.Add("project", Project);
            if (!string.IsNullOrEmpty(Debug)) capabilities.Add("browserstack.debug", Debug);
            if (!string.IsNullOrEmpty(Video)) capabilities.Add("browserstack.video", Video);
            if (!string.IsNullOrEmpty(Local)) capabilities.Add("browserstack.local", Local);
            if (!string.IsNullOrEmpty(DeviceOrientation)) capabilities.Add("deviceOrientation", DeviceOrientation);
            if (!string.IsNullOrEmpty(SeleniumVersion))
                capabilities.Add("browserstack.selenium_version", SeleniumVersion);
            if (!string.IsNullOrEmpty(LocalIdentifier))
                capabilities.Add("browserstack.localIdentifier", LocalIdentifier);
            if (!string.IsNullOrEmpty(Timezone)) capabilities.Add("browserstack.timezone", Timezone);
            if (!string.IsNullOrEmpty(NoFlash)) capabilities.Add("browserstack.ie.noFlash", NoFlash);
            if (!string.IsNullOrEmpty(Compatibility)) capabilities.Add("browserstack.ie.compatibility", Compatibility);
            if (!string.IsNullOrEmpty(Driver)) capabilities.Add("browserstack.ie.driver", Driver);
            if (!string.IsNullOrEmpty(IeEnablePopups)) capabilities.Add("browserstack.ie.enablePopups", IeEnablePopups);
            if (!string.IsNullOrEmpty(SafariEnablePopups))
                capabilities.Add("browserstack.safari.enablePopups", SafariEnablePopups);
            if (!string.IsNullOrEmpty(SafariAllowAllCookies))
                capabilities.Add("browserstack.safari.allowAllCookies", SafariAllowAllCookies);
            if (!string.IsNullOrEmpty(SafariDriver)) capabilities.Add("browserstack.safari.driver", SafariDriver);
            return capabilities;
        }

        public void UpdateTestResult()
        {
            var result = TestContext.CurrentContext.Result;
            var resultStr = "passed";
            var fixedErrorMessage = DateTime.Now.ToString("G");
            if (result.Outcome.Status != TestStatus.Passed)
            {
                resultStr = "failed";
                fixedErrorMessage = result.Message
                    .Replace(Environment.NewLine, string.Empty)
                    .Replace("\"", "'")
                    .Replace("{", string.Empty)
                    .Replace("}", string.Empty)
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);
            }
            string reason = $"{result.Outcome.Status} - {fixedErrorMessage}";
            var reqString = $"{{\"status\":\"{resultStr}\", \"reason\":\"{reason.Trim()}\"}}";
            try
            {
                Publish(reqString);
            }
            catch (Exception)
            {
                reqString = $"{{\"status\":\"{resultStr}\", \"reason\":\"{TestContext.CurrentContext.Result.Outcome}\"}}";
                Publish(reqString);
            }
        }

        public void Publish(string reqString)
        {
            var uri = new Uri($"https://www.browserstack.com/automate/sessions/{SessionId}.json");
            var requestData = Encoding.UTF8.GetBytes(reqString);
            var myWebRequest = WebRequest.Create(uri);
            var myHttpWebRequest = (HttpWebRequest)myWebRequest;
            myWebRequest.ContentType = "application/json";
            myWebRequest.Method = "PUT";
            myWebRequest.ContentLength = requestData.Length;
            using (var st = myWebRequest.GetRequestStream()) st.Write(requestData, 0, requestData.Length);
            var networkCredential = new NetworkCredential(SecretUser, SecretKey);
            var myCredentialCache = new CredentialCache { { uri, "Basic", networkCredential } };
            myHttpWebRequest.PreAuthenticate = true;
            myHttpWebRequest.Credentials = myCredentialCache;
            myWebRequest.GetResponse().Close();
        }
    }
}