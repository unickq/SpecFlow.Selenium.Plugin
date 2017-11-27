using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Unickq.SpecFlow.Selenium
{
    public static class Extensions
    {
        internal const string Driver = "Driver";
        internal const string Name = "Unickq.SpecFlow.Selenium";

        public static IWebDriver GetWebDriver(this ScenarioContext scenarioContext)
        {
            if (scenarioContext.ContainsKey(Driver))
            {
                return scenarioContext.Get<IWebDriver>(Driver);
            }
            throw new SpecFlowException("Driver is not present in ScenarioContext");
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
    }
}
