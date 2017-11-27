using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Unickq.SpecFlow.Selenium.Local
{
    public abstract class LocalDriver : IWebDriver
    {
        private readonly IWebDriver _driver;

        protected LocalDriver(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement FindElement(By by)
        {
            return _driver.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _driver.FindElements(by);
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        public void Close()
        {
            _driver.Close();
        }

        public void Quit()
        {
            _driver.Quit();
        }

        public IOptions Manage()
        {
            return _driver.Manage();
        }

        public INavigation Navigate()
        {
            return _driver.Navigate();
        }

        public ITargetLocator SwitchTo()
        {
            return _driver.SwitchTo();
        }

        public string Url
        {
            get { return _driver.Url; }
            set { _driver.Url = value; }
        }

        public string Title => _driver.Title;
        public string PageSource => _driver.PageSource;
        public string CurrentWindowHandle => _driver.CurrentWindowHandle;
        public ReadOnlyCollection<string> WindowHandles => _driver.WindowHandles;

        protected static string[] ParseWithDelimiter(string input, char splitter = '=')
        {
            var prms = input.Split(splitter);
            if (prms.Length != 2)
            {
                throw new ArgumentException($"Preference count doesn't equal to 2. Please use '{splitter}' delimiter");
            }
            return new[] {prms[0], prms[1]};
        }
    }
}