using System;
using System.Collections.Generic;
using OpenQA.Selenium.Edge;
using Unickq.SpecFlow.Selenium.Helpers;

namespace Unickq.SpecFlow.Selenium.Local
{
    public class EdgeDriver : OpenQA.Selenium.Edge.EdgeDriver
    {
        public EdgeDriver(Dictionary<string, object> capabilities) : base(SetOptions(capabilities))
        {
        }

        private static EdgeOptions SetOptions(Dictionary<string, object> capabilities)
        {
            var options = new EdgeOptions();
            foreach (var cap in capabilities)
                if (cap.Key.StartsWith("Capability", StringComparison.OrdinalIgnoreCase))
                {
                    var args = Extensions.ParseWithDelimiter(cap.Value.ToString());
                    options.AddAdditionalCapability(args[0], args[1]);
                }

            return options;
        }
    }
}