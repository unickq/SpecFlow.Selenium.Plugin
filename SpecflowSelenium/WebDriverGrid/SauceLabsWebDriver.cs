using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Unickq.SeleniumHelper.WebDriverGrid
{
    public class SauceLabsWebDriver : PaidWebDriver
    {
        private const string ApiUrl = "http://ondemand.saucelabs.com:80/wd/hub";

        private static readonly string SaucelabsUser = ConfigurationManager.AppSettings["saucelabs.username"];
        private static readonly string SaucelabsKey = ConfigurationManager.AppSettings["saucelabs.accessKey"];

        private static readonly string Name = ConfigurationManager.AppSettings["saucelabs.name"];
        private static readonly string SeleniumVersion = ConfigurationManager.AppSettings["saucelabs.seleniumVersion"];
        private static readonly string ChromedriverVersion = ConfigurationManager.AppSettings["saucelabs.chromedriverVersion"];
        private static readonly string IedriverVersion = ConfigurationManager.AppSettings["saucelabs.iedriverVersion"];
        private static readonly string AppiumVersion = ConfigurationManager.AppSettings["saucelabs.appiumVersion"];
        private static readonly string App = ConfigurationManager.AppSettings["saucelabs.app"];
        private static string Build = ConfigurationManager.AppSettings["saucelabs.build"];
        private static readonly string CustomData = ConfigurationManager.AppSettings["saucelabs.customData"];
        private static readonly string MaxDuration = ConfigurationManager.AppSettings["saucelabs.maxDuration"];
        private static readonly string CommandTimeout = ConfigurationManager.AppSettings["saucelabs.commandTimeout"];
        private static readonly string IdleTimeout = ConfigurationManager.AppSettings["saucelabs.idleTimeout"];
        private static readonly string TunnelIdentifier = ConfigurationManager.AppSettings["saucelabs.tunnelIdentifier"];
        private static readonly string ParentTunnel = ConfigurationManager.AppSettings["saucelabs.parentTunnel"];
        private static readonly string ScreenResolution = ConfigurationManager.AppSettings["saucelabs.screenResolution"];
        private static readonly string TimeZone = ConfigurationManager.AppSettings["saucelabs.timeZone"];
        private static readonly string AvoidProxy = ConfigurationManager.AppSettings["saucelabs.avoidProxy"];
        private static readonly string Public = ConfigurationManager.AppSettings["saucelabs.public"];
        private static readonly string RecordVideo = ConfigurationManager.AppSettings["saucelabs.recordVideo"];
        private static readonly string VideoUploadOnPass = ConfigurationManager.AppSettings["saucelabs.videoUploadOnPass"];
        private static readonly string RecordScreenshots = ConfigurationManager.AppSettings["saucelabs.recordScreenshots"];
        private static readonly string RecordLogs = ConfigurationManager.AppSettings["saucelabs.recordLogs"];
        private static readonly string CaptureHtml = ConfigurationManager.AppSettings["saucelabs.captureHtml"];
        private static readonly string WebdriverRemoteQuietExceptions = ConfigurationManager.AppSettings["saucelabs.webdriverRemoteQuietExceptions"];

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

            capabilities.Add("name",
                !string.IsNullOrEmpty(Name)
                    ? Name
                    : FixedTestName);

            Build = BuildTransform(Build);

            if (!string.IsNullOrEmpty(SeleniumVersion)) capabilities.Add("seleniumVersion", SeleniumVersion);
            if (!string.IsNullOrEmpty(ChromedriverVersion)) capabilities.Add("chromedriverVersion", ChromedriverVersion);
            if (!string.IsNullOrEmpty(IedriverVersion)) capabilities.Add("iedriverVersion", IedriverVersion);
            if (!string.IsNullOrEmpty(AppiumVersion)) capabilities.Add("appiumVersion", AppiumVersion);
            if (!string.IsNullOrEmpty(App)) capabilities.Add("app", App);
            if (!string.IsNullOrEmpty(Build)) capabilities.Add("build", Build);
            if (!string.IsNullOrEmpty(CustomData)) capabilities.Add("customData", CustomData);
            if (!string.IsNullOrEmpty(MaxDuration)) capabilities.Add("maxDuration", MaxDuration);
            if (!string.IsNullOrEmpty(CommandTimeout)) capabilities.Add("commandTimeout", CommandTimeout);
            if (!string.IsNullOrEmpty(IdleTimeout)) capabilities.Add("idleTimeout", IdleTimeout);
            if (!string.IsNullOrEmpty(TunnelIdentifier)) capabilities.Add("tunnelIdentifier", TunnelIdentifier);
            if (!string.IsNullOrEmpty(ParentTunnel)) capabilities.Add("parentTunnel", ParentTunnel);
            if (!string.IsNullOrEmpty(ScreenResolution)) capabilities.Add("screenResolution", ScreenResolution);
            if (!string.IsNullOrEmpty(TimeZone)) capabilities.Add("timeZone", TimeZone);
            if (!string.IsNullOrEmpty(AvoidProxy)) capabilities.Add("avoidProxy", AvoidProxy);
            if (!string.IsNullOrEmpty(Public)) capabilities.Add("public", Public);
            if (!string.IsNullOrEmpty(RecordVideo)) capabilities.Add("recordVideo", RecordVideo);
            if (!string.IsNullOrEmpty(VideoUploadOnPass)) capabilities.Add("videoUploadOnPass", VideoUploadOnPass);
            if (!string.IsNullOrEmpty(RecordScreenshots)) capabilities.Add("recordScreenshots", RecordScreenshots);
            if (!string.IsNullOrEmpty(RecordLogs)) capabilities.Add("recordLogs", RecordLogs);
            if (!string.IsNullOrEmpty(CaptureHtml)) capabilities.Add("captureHtml", CaptureHtml);
            if (!string.IsNullOrEmpty(WebdriverRemoteQuietExceptions)) capabilities.Add("webdriverRemoteQuietExceptions", WebdriverRemoteQuietExceptions);
            return capabilities;
        }

        protected override Uri Uri => new Uri($"https://saucelabs.com/rest/v1/{SecretUser}/jobs/{SessionId}");
    }
}
