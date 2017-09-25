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
    }
}
