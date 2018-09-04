using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TechTalk.SpecFlow;

namespace Unickq.SpecFlow.Selenium.Helpers
{
    public static class Extensions
    {
        internal const string Driver = "Driver";

        public static IWebDriver GetWebDriver(this ScenarioContext scenarioContext)
        {
            IWebDriver driver;
            scenarioContext.TryGetValue("Driver", out driver);
            return driver;
        }

        public static string[] ParseWithDelimiter(string input, char splitter = '=')
        {
            var prms = input.Split(splitter);
            if (prms.Length != 2)
                throw new ArgumentException($"Preference count doesn't equal to 2. Please use '{splitter}' delimiter");

            return new[] {prms[0], prms[1]};
        }

        public static string GetBrowserName(this IWebDriver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            var browserName = driver.GetType().Name;
            var webDriver = driver as RemoteWebDriver;
            if (webDriver != null)
            {
                var caps = webDriver.Capabilities;
                browserName = $"Remote {caps.GetCapability("browserName")} {caps.GetCapability("version")}";
            }

            return browserName;
        }

        public static long ToUnixTimeMilliseconds(this DateTimeOffset dateTimeOffset)
        {
            return (long) DateTimeOffset.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}