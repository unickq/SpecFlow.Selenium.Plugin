using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Unickq.SpecFlow.Selenium.WebDriverGrid;

namespace Unickq.SpecFlow.Selenium
{
    public static class Extensions
    {
        internal const string Driver = "Driver";
        internal const string Name = "Unickq.SpecFlow.Selenium";

        public static IWebDriver GetWebDriver(this ScenarioContext scenarioContext)
        {
            IWebDriver driver;
            scenarioContext.TryGetValue("Driver", out driver);         
            return driver;
//            if (scenarioContext.ContainsKey(Driver))
//            {
//                return scenarioContext.Get<IWebDriver>(Driver);
//            }
//            throw new SpecFlowException("Driver is not present in ScenarioContext");
        }

        public static string[] ParseWithDelimiter(string input, char splitter = '=')
        {
            var prms = input.Split(splitter);
            if (prms.Length != 2)
            {
                throw new ArgumentException($"Preference count doesn't equal to 2. Please use '{splitter}' delimiter");
            }
            return new[] { prms[0], prms[1] };
        }

        public static string GetBrowserName(this IWebDriver driver)
        {
            if (driver == null) throw new ArgumentNullException(nameof(driver));
            var browserName = driver.GetType().Name;
            var webDriver = driver as PaidWebDriver;
            if (webDriver != null)
            {
                var caps = webDriver.Capabilities;
                browserName = $"Remote {caps.BrowserName.ToUpperInvariant()} {caps.Version}";
            }
            return browserName;
        }
    }
}
