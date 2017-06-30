using System;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Unickq.SeleniumHelper;

namespace Example.Steps
{
    [Binding]
    public class ExampleFeatureSteps : SeleniumHelperBinding
    {
       
        [Given(@"I have opened (.*)")]
        public void GivenIHaveOpened(string url)
        {
            ScenarioContext.TryGetValue("GoogleTranslate", out string language);
            Browser.Navigate().GoToUrl(url + $"?hl={language}");
            var text = Browser.FindElement(By.Id("gt-appname")).Text;
            Console.WriteLine(text);
        }
        [Then(@"the title should contain '(.*)'")]
        public void ThenTheTitleShouldContain(string part)
        {
            StringAssert.Contains(part.ToLower(), ScenarioContext.Get<IWebDriver>("Driver").Title.ToLower());
        }

        public ExampleFeatureSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }
    }
}
