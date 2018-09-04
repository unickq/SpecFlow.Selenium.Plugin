using System;
using System.Collections.Generic;
using OpenQA.Selenium.IE;
using Unickq.SpecFlow.Selenium.Helpers;

namespace Unickq.SpecFlow.Selenium.Local
{
    public class InternetExplorerDriver : OpenQA.Selenium.IE.InternetExplorerDriver
    {
        public InternetExplorerDriver(Dictionary<string, object> capabilities) : base(SetOptions(capabilities))
        {
        }

        private static InternetExplorerOptions SetOptions(Dictionary<string, object> capabilities)
        {
            var options = new InternetExplorerOptions();
            foreach (var cap in capabilities)
                if (cap.Key.StartsWith("capability", StringComparison.OrdinalIgnoreCase))
                {
                    var args = Extensions.ParseWithDelimiter(cap.Value.ToString());
                    options.AddAdditionalCapability(args[0], args[1]);
                }
                else if (cap.Key.Equals("BrowserCommandLineArguments", StringComparison.OrdinalIgnoreCase))
                {
                    options.BrowserCommandLineArguments = cap.Value.ToString();
                }
                else if (cap.Key.Equals("IgnoreZoomLevel", StringComparison.OrdinalIgnoreCase))
                {
                    options.IgnoreZoomLevel = true;
                }
                else if (cap.Key.Equals("AcceptInsecureCertificates", StringComparison.OrdinalIgnoreCase))
                {
                    options.AcceptInsecureCertificates = true;
                }
                else if (cap.Key.Equals("EnableNativeEvents", StringComparison.OrdinalIgnoreCase))
                {
                    options.EnableNativeEvents = true;
                }
                else if (cap.Key.Equals("EnsureCleanSession", StringComparison.OrdinalIgnoreCase))
                {
                    options.EnsureCleanSession = true;
                }
                else if (cap.Key.Equals("EnablePersistentHover", StringComparison.OrdinalIgnoreCase))
                {
                    options.EnablePersistentHover = true;
                }

            return options;
        }
    }
}