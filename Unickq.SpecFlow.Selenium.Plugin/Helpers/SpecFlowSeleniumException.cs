using System;
using TechTalk.SpecFlow;

namespace Unickq.SpecFlow.Selenium
{
    public class SpecFlowSeleniumException : SpecFlowException
    {
        public SpecFlowSeleniumException(string message)
            : base(message)
        {
        }

        public SpecFlowSeleniumException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
