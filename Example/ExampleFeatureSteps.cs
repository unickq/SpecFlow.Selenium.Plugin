using NUnit.Framework;
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
            Browser.Current.Navigate().GoToUrl(url);
        }
        
        [Then(@"the titls should contains (.*)")]
        public void ThenTheTitlsShouldContains(string part)
        {
            StringAssert.Contains(part.ToLower(), Browser.Current.Title.ToLower());
        }
    }
}
