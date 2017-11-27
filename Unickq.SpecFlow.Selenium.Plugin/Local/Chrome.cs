using System;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;

namespace Unickq.SpecFlow.Selenium.Local
{
    public class Chrome : LocalDriver
    {
        private static ChromeOptions SetOptions(Dictionary<string, object> capabilities)
        {
            var options = new ChromeOptions();
            foreach (var cap in capabilities)
            {
                if (cap.Key.Equals("MobileEmulation", StringComparison.OrdinalIgnoreCase))
                {
                    options.EnableMobileEmulation(cap.Value.ToString());
                }
                else if (cap.Key.Equals("Arguments", StringComparison.OrdinalIgnoreCase))
                {
                    options.AddArguments(cap.Value.ToString().Split(';'));
                }
                else if (cap.Key.Equals("Extensions", StringComparison.OrdinalIgnoreCase))
                {
                    options.AddExtensions(cap.Value.ToString().Split(';'));
                }
                else if (cap.Key.StartsWith("UserProfilePreference", StringComparison.OrdinalIgnoreCase))
                {
                    var args = ParseWithDelimiter(cap.Value.ToString());
                    options.AddUserProfilePreference(args[0], args[1]);        
                }
                else if (cap.Key.Equals("BinaryLocation", StringComparison.OrdinalIgnoreCase))
                {
                    options.BinaryLocation = cap.Value.ToString();
                }
                else if (cap.Key.Equals("AcceptInsecureCertificates", StringComparison.OrdinalIgnoreCase))
                {
                    options.AcceptInsecureCertificates = true;
                }
                else if (cap.Key.StartsWith("Capability", StringComparison.OrdinalIgnoreCase))
                {
                    var args = ParseWithDelimiter(cap.Value.ToString());
                    options.AddAdditionalCapability(args[0], args[1]);
                }
            }
            return options;
        }

        public Chrome(Dictionary<string, object> capabilities) : base(new ChromeDriver(SetOptions(capabilities)))
        {
        }
    }
}