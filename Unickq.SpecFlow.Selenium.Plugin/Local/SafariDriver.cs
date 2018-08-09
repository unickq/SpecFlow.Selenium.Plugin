using System;
using System.Collections.Generic;
using OpenQA.Selenium.Safari;
using Unickq.SpecFlow.Selenium.Helpers;

namespace Unickq.SpecFlow.Selenium.Local
{
    public class SafariDriver : OpenQA.Selenium.Safari.SafariDriver
    {
        public SafariDriver(Dictionary<string, object> capabilities) : base(SetOptions(capabilities))
        {
        }

        private static SafariOptions SetOptions(Dictionary<string, object> capabilities)
        {
            var options = new SafariOptions();
            foreach (var cap in capabilities)
            {
                if (cap.Key.StartsWith("Capability", StringComparison.OrdinalIgnoreCase))
                {
                    var args = Extensions.ParseWithDelimiter(cap.Value.ToString());
                    options.AddAdditionalCapability(args[0], args[1]);
                }
                else if (cap.Key.Equals("AcceptInsecureCertificates", StringComparison.OrdinalIgnoreCase))
                {
                    options.AcceptInsecureCertificates = true;
                }
            }
            return options;
        }
    }
}