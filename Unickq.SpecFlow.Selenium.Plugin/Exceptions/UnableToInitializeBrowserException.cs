using System;

namespace Unickq.SpecFlow.Selenium.Exceptions
{
    public class UnableToInitializeBrowserException : SpecFlowSeleniumException
    {
        public UnableToInitializeBrowserException(string message) : base(message)
        {
        }

        public UnableToInitializeBrowserException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}