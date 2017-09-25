using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Unickq;
using Unickq.SpecFlow.Selenium;

namespace Example.Steps
{
    [Binding]
    public class ExampleFeatureSteps
    {
       
        [Given(@"I have opened (.*)")]
        public void GivenIHaveOpened(string url)
        {
            string language;
            ScenarioContext.TryGetValue("GoogleTranslate", out language);
            Browser.Navigate().GoToUrl(url + $"?hl={language}");
            var text = Browser.FindElement(By.Id("gt-appname")).Text;
            //            Console.WriteLine(text);
            Thread.Sleep(2000);
        }
        [Then(@"the title should contain '(.*)'")]
        public void ThenTheTitleShouldContain(string part)
        {
            StringAssert.Contains(part.ToLower(), Browser.Title.ToLower());
            Console.WriteLine($"-> !!!!! Used WebDriver - {Browser.GetType()}");
            Thread.Sleep(2000);
        }

        public ExampleFeatureSteps(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException(nameof(scenarioContext));
            ScenarioContext = scenarioContext;
            Browser = scenarioContext.GetWebDriver();
        }

        protected readonly ScenarioContext ScenarioContext;
        protected readonly IWebDriver Browser;
    }
}
