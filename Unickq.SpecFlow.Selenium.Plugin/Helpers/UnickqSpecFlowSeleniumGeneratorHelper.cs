using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Unickq.SpecFlow.Selenium.Helpers
{
    public class UnickqSpecFlowSeleniumGeneratorHelper
    {
        private readonly IContainer _container;
        public IWebDriver Driver { get; private set; }

        public UnickqSpecFlowSeleniumGeneratorHelper()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader());
            _container = builder.Build();
        }

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
            {
                if (registration.Services.Count() == 1)
                {
                    var serviceDescription = registration.Services.First().Description;
                    list.Add(serviceDescription.Substring(0, serviceDescription.IndexOf(' ')));
                }
            }
            return list;
        }

        public virtual void SetUp()
        {
            string browserName;
            var categories = TestContext.CurrentContext.Test.Arguments.Select(args => args?.ToString()).ToList();
            var configList = GetPossibleBrowsersFromConfig(_container);
            var common = categories.Intersect(configList).ToList();
            if (common.Count == 1)
            {
                browserName = common.First();
            }
            else
            {
                Assert.Ignore("Unable to register browser. Please check @Browser tag" +
                              $"\n    Possible values are:\n      {string.Join(" ", configList)}\n");
                throw new SpecFlowSeleniumException(null);
            }

            try
            {
                Driver = _container.ResolveNamed<IWebDriver>(browserName);
            }
            catch (Autofac.Core.Registration.ComponentNotRegisteredException)
            {
                throw new SpecFlowSeleniumException(
                    $"Unable to register {browserName}. Please check the name of componens");
            }
            catch (Autofac.Core.DependencyResolutionException e)
            {
                throw new SpecFlowSeleniumException(
                    $"Unable to initialize {browserName}. Please validate configuration parameters", e?.InnerException);
            }
        }


        public virtual void TearDown()
        {
            if (Driver != null)
            {
                if (Driver is WebDriverGrid.PaidWebDriver)
                {
                    ((WebDriverGrid.PaidWebDriver) Driver).UpdateTestResult();
                }

                System.Threading.Thread.Sleep(50);
                Driver.Quit();
                Driver = null;
            }
        }

        public void ClearScenarioContext(ScenarioContext testRunnerScenarioContext, string key)
        {
            if (testRunnerScenarioContext != null)
            {
                if (testRunnerScenarioContext.ContainsKey(key))
                {
                    testRunnerScenarioContext.Remove(key);
                }
            }
        }
    }
}