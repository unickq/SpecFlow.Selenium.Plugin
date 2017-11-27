using System;
using System.Collections.Generic;
using OpenQA.Selenium.Edge;

namespace Unickq.SpecFlow.Selenium.Local
{
    public class Edge : LocalDriver
    {
        public Edge(Dictionary<string, object> capabilities) : base(new EdgeDriver(SetOptions(capabilities)))
        {
        }

        private static EdgeOptions SetOptions(Dictionary<string, object> capabilities)
        {
            var options = new EdgeOptions();
            foreach (var cap in capabilities)
            {
                if (cap.Key.StartsWith("Capability", StringComparison.OrdinalIgnoreCase))
                {
                    var args = ParseWithDelimiter(cap.Value.ToString());
                    options.AddAdditionalCapability(args[0], args[1]);
                }
            }
            return options;
        }
    }
}
