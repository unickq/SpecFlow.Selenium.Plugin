using System;
using System.Collections.Generic;
using OpenQA.Selenium.Firefox;
using Unickq.SpecFlow.Selenium.Helpers;

namespace Unickq.SpecFlow.Selenium.Local
{
    public class FirefoxDriver : OpenQA.Selenium.Firefox.FirefoxDriver
    {
        public FirefoxDriver(Dictionary<string, object> capabilities) : base(SetOptions(capabilities))
        {
        }

        private static FirefoxOptions SetOptions(Dictionary<string, object> capabilities)
        {
            var options = new FirefoxOptions();
            var profile = new FirefoxProfile();
            foreach (var cap in capabilities)
                if (cap.Key.Equals("FirefoxProfile", StringComparison.OrdinalIgnoreCase))
                {
                    profile = new FirefoxProfileManager().GetProfile(cap.Value.ToString());
                }
                else if (cap.Key.StartsWith("Capability", StringComparison.OrdinalIgnoreCase))
                {
                    var args = Extensions.ParseWithDelimiter(cap.Value.ToString());
                    options.AddAdditionalCapability(args[0], args[1]);
                }
                else if (cap.Key.Equals("Binary", StringComparison.OrdinalIgnoreCase))
                {
                    options.BrowserExecutableLocation = cap.Value.ToString();
                }
                else if (cap.Key.Equals("Arguments", StringComparison.OrdinalIgnoreCase))
                {
                    options.AddArguments(cap.Value.ToString().Split(';'));
                }
                else if (cap.Key.StartsWith("Preference", StringComparison.OrdinalIgnoreCase))
                {
                    var args = Extensions.ParseWithDelimiter(cap.Value.ToString());
                    options.SetPreference(args[0], args[1]);
                }
                else if (cap.Key.Equals("Extensions", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var extension in cap.Value.ToString().Split(';')) profile.AddExtension(extension);
                }
                else if (cap.Key.Equals("AcceptUntrustedCertificates", StringComparison.OrdinalIgnoreCase))
                {
                    profile.AcceptUntrustedCertificates = true;
                }
                else if (cap.Key.Equals("AcceptInsecureCertificates", StringComparison.OrdinalIgnoreCase))
                {
                    options.AcceptInsecureCertificates = true;
                }
                else if (cap.Key.Equals("AssumeUntrustedCertificateIssuer", StringComparison.OrdinalIgnoreCase))
                {
                    profile.AssumeUntrustedCertificateIssuer = true;
                }

            options.Profile = profile;
            return options;
        }
    }
}