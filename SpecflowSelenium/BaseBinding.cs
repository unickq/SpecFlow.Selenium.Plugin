using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Unickq.SeleniumHelper
{
    public class SeleniumHelperBinding
    {
        protected readonly ScenarioContext ScenarioContext;
        protected readonly IWebDriver Browser;

        protected SeleniumHelperBinding(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
            ScenarioContext = scenarioContext;
            Browser = scenarioContext.Get<IWebDriver>("Driver");
        }

    }
}
