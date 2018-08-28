using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Unickq.SpecFlow.Selenium.WebDriverGrid
{
    public class RemoteWebDriver : OpenQA.Selenium.Remote.RemoteWebDriver
    {
        public RemoteWebDriver(string url, string browser)
            : base(new Uri(url), GetCapabilities(browser))
        {
        }

        public RemoteWebDriver(string url, string browser, Dictionary<string, string> capabilities)
            : base(new Uri(url), GetCapabilities(browser, capabilities))
        {
        }

        public RemoteWebDriver(string url, Dictionary<string, string> capabilities) : base(new Uri(url), GetCapabilities(capabilities))
        {
        }

        private static DesiredCapabilities GetCapabilities(Dictionary<string, string> additionalCapabilities)
        {
            var capabilities = new DesiredCapabilities();
            foreach (var capability in additionalCapabilities)
                capabilities.SetCapability(capability.Key, capability.Value);
            return capabilities;
        }

        private static DesiredCapabilities GetCapabilities(string browserName,
            Dictionary<string, string> additionalCapabilities = null)
        {

            var capabilities = new DesiredCapabilities(browserName, string.Empty, new Platform(PlatformType.Any));
            if (additionalCapabilities == null) return capabilities;
            foreach (var capability in additionalCapabilities)
                capabilities.SetCapability(capability.Key, capability.Value);
            return capabilities;
        }
    }
}
