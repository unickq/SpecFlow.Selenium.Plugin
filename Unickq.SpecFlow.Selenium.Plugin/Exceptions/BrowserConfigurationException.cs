using System;

namespace Unickq.SpecFlow.Selenium.Exceptions
{
    public class BrowserConfigurationException : SpecFlowSeleniumException
    {
        public BrowserConfigurationException(string message) : base(message)
        {
        }

        public BrowserConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}