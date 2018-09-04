using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Autofac.Configuration;
using Autofac.Core;
using Autofac.Core.Registration;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Unickq.SpecFlow.Selenium.Exceptions;
using Unickq.SpecFlow.Selenium.WebDriverGrid;

namespace Unickq.SpecFlow.Selenium
{
    public class UnickqSpecFlowSeleniumGeneratorHelper
    {
        protected readonly IContainer Container;
        protected readonly ITestRunner TestRunner;
        protected string BrowserName;

        public UnickqSpecFlowSeleniumGeneratorHelper(ITestRunner testRunner)
        {
            TestRunner = testRunner;
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader());
            Container = builder.Build();
        }

        public IWebDriver Driver { get; private set; }

        public virtual void FeatureSetup()
        {
        }

        public virtual void FeatureTearDown()
        {
        }

        private static List<string> GetPossibleBrowsersFromConfig(IContainer container)
        {
            var list = new List<string>();
            foreach (var registration in container.ComponentRegistry.Registrations)
                if (registration.Services.Count() == 1)
                {
                    var serviceDescription = registration.Services.First().Description;
                    list.Add(serviceDescription.Substring(0, serviceDescription.IndexOf(' ')));
                }

            return list;
        }

        public virtual void SetUp()
        {
            var categories = TestContext.CurrentContext.Test.Arguments.Select(args => args?.ToString()).ToList();
            var configList = GetPossibleBrowsersFromConfig(Container);
            var common = categories.Intersect(configList).ToList();
            if (common.Count == 1)
                BrowserName = common.First();
            else
                throw new UnableToInitializeBrowserException("Unable to register browser. Please check @Browser tag" +
                                                             $"\n    Possible values are:\n      {string.Join(" ", configList)}\n");

            try
            {
                Driver = Container.ResolveNamed<IWebDriver>(BrowserName);
            }
            catch (ComponentNotRegisteredException)
            {
                throw new SpecFlowSeleniumException(
                    $"Unable to register {BrowserName}.");
            }
            catch (DependencyResolutionException e)
            {
                throw new BrowserConfigurationException(
                    $"Unable to initialize {BrowserName}. Please validate configuration parameters\n{e.InnerException?.Message}");
            }
        }


        public virtual void TearDown()
        {
            if (Driver != null)
            {
                if (Driver is PaidWebDriver) UpdateApi();

                KillWebDriver();
            }
        }

        protected void KillWebDriver()
        {
            Thread.Sleep(50);
            Driver.Quit();
            Driver = null;
        }

        protected void UpdateApi()
        {
            ((PaidWebDriver) Driver).UpdateTestResult();
        }

        public void ClearScenarioContext(ScenarioContext testRunnerScenarioContext, string key)
        {
            if (testRunnerScenarioContext != null)
                if (testRunnerScenarioContext.ContainsKey(key))
                    testRunnerScenarioContext.Remove(key);
        }
    }
}