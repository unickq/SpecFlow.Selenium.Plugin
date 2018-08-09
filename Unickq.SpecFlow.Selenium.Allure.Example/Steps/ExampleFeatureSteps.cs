using System;
using System.Threading;
using Allure.Commons;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Unickq.SpecFlow.Selenium.Allure;

namespace Unickq.SpecFlow.Selenium.Example.Steps
{
    [Binding]
    public class ExampleFeatureSteps
    {
        protected readonly ScenarioContext ScenarioContext;
        private IWebDriver Browser => ScenarioContext.Get<IWebDriver>("Driver");

        [Given(@"I have opened (.*)")]
        public void GivenIHaveOpened(string url)
        {
            string language;
            ScenarioContext.TryGetValue("GoogleTranslate", out language);
            Browser.Navigate().GoToUrl(url + $"?hl={language}");
            var text = Browser.FindElement(By.Id("gt-appname")).Text;
            Console.WriteLine(text);
        }

        [Then(@"the title should contain '(.*)'")]
        public void ThenTheTitleShouldContain(string part)
        {
            AllureLifecycle.Instance.WrapInStep(() => { Thread.Sleep(1000); }, "Sleep 1000");
            AllureLifecycle.Instance.WrapInStep(() => { StringAssert.Contains(part.ToLower(), Browser.Title.ToLower()); }, "Validation");
        }

        public ExampleFeatureSteps(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new ArgumentNullException(nameof(scenarioContext));
            ScenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            AllureLifecycle.Instance.CleanupResultDirectory();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            AllureLifecycle.Instance.WrapInStep(() => { Console.WriteLine("BeforeScenario"); }, "BeforeScenario");
        }
    }
}