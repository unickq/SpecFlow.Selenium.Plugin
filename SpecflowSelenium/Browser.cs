using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Unickq.SeleniumHelper
{
    [Binding]
    public static class Browser
    {
        public static IWebDriver Current => ScenarioContext.Current.Get<IWebDriver>("Driver");
    }
}
