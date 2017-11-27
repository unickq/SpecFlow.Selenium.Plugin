using System;
using System.Collections.Generic;
using OpenQA.Selenium.Firefox;

namespace Unickq.SpecFlow.Selenium.Local
{
    public class FirefoxDriver : OpenQA.Selenium.Firefox.FirefoxDriver
    {
        private static FirefoxOptions SetOptions(Dictionary<string, object> capabilities)
        {
            var options = new FirefoxOptions();
            foreach (var cap in capabilities)
            {
                if (cap.Key.StartsWith("Capability", StringComparison.OrdinalIgnoreCase))
                {
                    var args = Extensions.ParseWithDelimiter(cap.Value.ToString());              
                    options.AddAdditionalCapability(args[0], args[1]);
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
                    foreach (var extension in cap.Value.ToString().Split(';'))
                    {
                        options.Profile.AddExtension(extension);
                    }               
                }
                else if (cap.Key.Equals("AcceptUntrustedCertificates", StringComparison.OrdinalIgnoreCase))
                {
                    options.Profile.AcceptUntrustedCertificates = true;
                }
                else if (cap.Key.Equals("AssumeUntrustedCertificateIssuer", StringComparison.OrdinalIgnoreCase))
                {
                    options.Profile.AssumeUntrustedCertificateIssuer = true;
                }
                else if (cap.Key.Equals("FirefoxProfile", StringComparison.OrdinalIgnoreCase))
                {
                    var profile = new FirefoxProfileManager().GetProfile(cap.Value.ToString());
                    options.Profile = profile;
                }
            }
            return options;
        }

        public FirefoxDriver(Dictionary<string, object> capabilities) : base(SetOptions(capabilities))
        {
        }
    }
}
