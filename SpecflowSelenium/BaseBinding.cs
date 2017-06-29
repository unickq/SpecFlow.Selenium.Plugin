using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Unickq.SeleniumHelper
{
    public class SeleniumHelperBinding
    {
        protected readonly ScenarioContext ScenarioContext;
        protected readonly IWebDriver Browser;

        public SeleniumHelperBinding(ScenarioContext scenarioContext)
        {
            ScenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            Browser = scenarioContext.Get<IWebDriver>("Driver");
        }

    }
}
