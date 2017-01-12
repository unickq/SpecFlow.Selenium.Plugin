using System;
using System.Collections.Generic;
using System.Reflection;
using OpenQA.Selenium.Remote;

namespace Unickq.SeleniumHelper
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

        private static DesiredCapabilities GetCapabilities(string browserName,
            Dictionary<string, string> additionalCapabilities = null)
        {
            var capabilityCreationMethod = typeof(DesiredCapabilities).GetMethod(browserName,
                BindingFlags.Public | BindingFlags.Static);
            if (capabilityCreationMethod == null)
                throw new NotSupportedException("Can't find DesiredCapabilities with name " + browserName);

            var capabilities = capabilityCreationMethod.Invoke(null, null) as DesiredCapabilities;
            if (capabilities == null)
                throw new NotSupportedException("Can't find DesiredCapabilities with name " + browserName);

            if (additionalCapabilities != null)
                foreach (var capability in additionalCapabilities)
                    capabilities.SetCapability(capability.Key, capability.Value);

            return capabilities;
        }
    }
}