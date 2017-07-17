using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public class TestingBotWebDriver : CustomRemoteWebDriver
    {
        private const string ApiUrl = "http://hub.testingbot.com/wd/hub/";
        protected override Uri Uri => new Uri($"https://api.testingbot.com/v1/tests/{SessionId}");

        public TestingBotWebDriver(string browser, string key, string secret, Dictionary<string, string> capabilities)
            : base(ApiUrl, browser, Auth(key, secret, capabilities))
        {
            SecretUser = key;
            SecretKey = secret;
        }

        private static readonly string TestingBotKey = ConfigurationManager.AppSettings["testingbot.key"];
        private static readonly string TestingBotSecret = ConfigurationManager.AppSettings["testingbot.secret"];

        private static readonly string Screenshot = ConfigurationManager.AppSettings["testingbot.screenshot"];
        private static readonly string Video = ConfigurationManager.AppSettings["testingbot.screenrecorder"];
        private static readonly string SeleniumVersion = ConfigurationManager.AppSettings["testingbot.selenium-version"];
        private static readonly string ChromedriverVersion = ConfigurationManager.AppSettings["testingbot.chromedriverVersion"];
        private static readonly string IedriverVersion = ConfigurationManager.AppSettings["testingbot.iedriverVersion"];
        private static readonly string GeckodriverVersion = ConfigurationManager.AppSettings["testingbot.geckodriverVersion"];
        private static readonly string Name = ConfigurationManager.AppSettings["testingbot.name"];
        private static readonly string ScreenResolution = ConfigurationManager.AppSettings["testingbot.screen-resolution"];
        private static readonly string AvoidProxy = ConfigurationManager.AppSettings["testingbot.avoidProxy"];
        private static string Build = ConfigurationManager.AppSettings["testingbot.build"];
        private static readonly string Extra = ConfigurationManager.AppSettings["testingbot.extra"];
        private static readonly string Idletimeout = ConfigurationManager.AppSettings["testingbot.idletimeout"];
        private static readonly string Public = ConfigurationManager.AppSettings["testingbot.public"];
        private static readonly string TimeZone = ConfigurationManager.AppSettings["testingbot.timeZone"];
        private static readonly string UserExtension = ConfigurationManager.AppSettings["testingbot.user-extension"];
        private static readonly string LoadExtension = ConfigurationManager.AppSettings["testingbot.load-extension"];
        private static readonly string Groups = ConfigurationManager.AppSettings["testingbot.load-groups"];
        private static readonly string Prerun = ConfigurationManager.AppSettings["testingbot.prerun"];
        private static readonly string Upload = ConfigurationManager.AppSettings["testingbot.upload"];
        private static readonly string UploadFilepath = ConfigurationManager.AppSettings["testingbot.uploadFilepath"];
        private static readonly string UploadMultiple = ConfigurationManager.AppSettings["testingbot.uploadMultiple"];
        private static readonly string Autoclick = ConfigurationManager.AppSettings["testingbot.autoclick"];
        private static readonly string Sikuli = ConfigurationManager.AppSettings["testingbot.sikuli"];
        private static readonly string Hosts = ConfigurationManager.AppSettings["testingbot.blacklist"];
        private static readonly string RecordLogs = ConfigurationManager.AppSettings["testingbot.recordLogs"];


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

            capabilities.Add("name",
                !string.IsNullOrEmpty(Name)
                    ? Name
                    : FixedTestName);

            Build = BuildTransform(Build);

            if (!string.IsNullOrEmpty(Screenshot)) capabilities.Add("screenshot", Screenshot);
            if (!string.IsNullOrEmpty(Video)) capabilities.Add("screenrecorder", Video);
            if (!string.IsNullOrEmpty(SeleniumVersion)) capabilities.Add("selenium-version", SeleniumVersion);
            if (!string.IsNullOrEmpty(ChromedriverVersion)) capabilities.Add("chromedriverVersion", ChromedriverVersion);
            if (!string.IsNullOrEmpty(IedriverVersion)) capabilities.Add("iedriverVersion", IedriverVersion);
            if (!string.IsNullOrEmpty(GeckodriverVersion)) capabilities.Add("geckodriverVersion", GeckodriverVersion);
            if (!string.IsNullOrEmpty(ScreenResolution)) capabilities.Add("screen-resolution", ScreenResolution);
            if (!string.IsNullOrEmpty(AvoidProxy)) capabilities.Add("avoidProxy", AvoidProxy);
            if (!string.IsNullOrEmpty(Build)) capabilities.Add("build", Build);
            if (!string.IsNullOrEmpty(Extra)) capabilities.Add("extra", Extra);
            if (!string.IsNullOrEmpty(Idletimeout)) capabilities.Add("idletimeout", Idletimeout);
            if (!string.IsNullOrEmpty(Public)) capabilities.Add("public", Public);
            if (!string.IsNullOrEmpty(TimeZone)) capabilities.Add("timeZone", TimeZone);
            if (!string.IsNullOrEmpty(UserExtension)) capabilities.Add("user-extension", UserExtension);
            if (!string.IsNullOrEmpty(LoadExtension)) capabilities.Add("load-extension", LoadExtension);
            if (!string.IsNullOrEmpty(Groups)) capabilities.Add("load-groups", Groups);
            if (!string.IsNullOrEmpty(Prerun)) capabilities.Add("prerun", Prerun);
            if (!string.IsNullOrEmpty(Upload)) capabilities.Add("upload", Upload);
            if (!string.IsNullOrEmpty(UploadFilepath)) capabilities.Add("uploadFilepath", UploadFilepath);
            if (!string.IsNullOrEmpty(UploadMultiple)) capabilities.Add("uploadMultiple", UploadMultiple);
            if (!string.IsNullOrEmpty(Autoclick)) capabilities.Add("autoclick", Autoclick);
            if (!string.IsNullOrEmpty(Sikuli)) capabilities.Add("sikuli", Sikuli);
            if (!string.IsNullOrEmpty(Hosts)) capabilities.Add("hosts", Hosts);
            if (!string.IsNullOrEmpty(RecordLogs)) capabilities.Add("recordLogs", RecordLogs);


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
