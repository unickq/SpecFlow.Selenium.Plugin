using System;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Unickq.SeleniumHelper;

namespace Example
{
    [Binding]
    public class ExampleFeatureSteps
    {
        [Given(@"I have opened (.*)")]
        public void GivenIHaveOpened(string url)
        {
            string language;
            ScenarioContext.Current.TryGetValue("GoogleTranslate", out language);
            Browser.Current.Navigate().GoToUrl(url + $"?hl={language}");
            Console.WriteLine(Browser.Current.FindElement(By.Id("gt-appname")).Text);
        }
        [Then(@"the title should contain '(.*)'")]
        public void ThenTheTitleShouldContain(string part)
        {
            StringAssert.Contains(part.ToLower(), Browser.Current.Title.ToLower());
        }
    }
}
