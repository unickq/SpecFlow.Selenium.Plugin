using System;
using System.Collections.Generic;
using Allure.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Unickq.SpecFlow.Selenium.Allure;
using Unickq.SpecFlow.Selenium.Helpers;
using Unickq.SpecFlow.Selenium.WebDriverGrid;

namespace Unickq.SpecFlow.Selenium
{
    public class UnickqSpecFlowSeleniumAllureGeneratorHelper : UnickqSpecFlowSeleniumGeneratorHelper
    {
        private readonly AllureLifecycle _allure = AllureLifecycle.Instance;
        public string RootContainerId { get; }
        public string TestContainerId { get; private set; }
        public string TestResultId { get; } = TestContext.CurrentContext.Test.FullName;

        public override void FeatureSetup()
        {
            _allure.StartTestContainer(new TestResultContainer {uuid = RootContainerId});
        }

        public override void FeatureTearDown()
        {
            _allure.UpdateTestContainer(RootContainerId,
                x => x.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds());
            _allure.StopTestContainer(RootContainerId);
            _allure.WriteTestContainer(RootContainerId);
        }

        public override void SetUp()
        {
            TestContainerId = Guid.NewGuid() + "t";
            _allure.StartTestContainer(RootContainerId,
                new TestResultContainer {uuid = TestContainerId});
            _allure.StartBeforeFixture(TestContainerId, Guid.NewGuid().ToString(),
                new FixtureResult());

            var sr = new StepResult
            {
                name = "Starting browser",
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                status = Status.passed
            };

            var testResult = new TestResult
            {
                uuid = TestResultId,
                name = TestContext.CurrentContext.Test.Name,
                descriptionHtml = TestRunner.FeatureContext.FeatureInfo.Description,
                labels = new List<Label>
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.TestClass(TestContext.CurrentContext.Test.ClassName),
                    Label.TestMethod(TestContext.CurrentContext.Test.MethodName),
                    Label.Package(TestContext.CurrentContext.Test.ClassName)
                }
            };

            try
            {
                base.SetUp();
                sr.name = $"Starting {BrowserName}";
            }
            catch (Exception e)
            {
                sr.status = Status.broken;
                sr.statusDetails.message = e.Message;

                _allure.UpdateFixture(f =>
                {
                    f.status = Status.failed;
                    f.statusDetails = PluginHelper.GetStatusDetails(e);
                });

                throw;
            }
            finally
            {
                sr.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                sr.stage = Stage.finished;
                _allure.StopFixture(result =>
                {
                    result.stage = Stage.finished;
                    result.steps.Add(sr);
                });
                testResult.parameters = ParametersForBuild();
                _allure.StartTestCase(TestContainerId, testResult);
            }
        }

        public override void TearDown()
        {
            try
            {
                _allure.UpdateTestCase(x =>
                {
                    x.labels.Add(Label.Epic(TestRunner.FeatureContext.FeatureInfo.Title));
                    x.labels.Add(Label.Feature(TestRunner.ScenarioContext.ScenarioInfo.Title));
                    x.labels.Add(Label.ParentSuite(BrowserName));
                });
            }
            catch (Exception)
            {
                _allure.UpdateTestCase(x => { x.labels.Add(Label.Feature("Not run")); });
            }

            if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Passed)
            {
                if (Driver != null)
                {
                    var screen = ((ITakesScreenshot) Driver).GetScreenshot();
                    _allure.AddAttachment("Screenshot", "image/png", screen.AsByteArray);
                }
            }
            else
            {
                _allure.UpdateTestCase(x => x.status = Status.passed);
            }

            _allure.StopTestCase(TestResultId);
            _allure.WriteTestCase(TestResultId);
            _allure.StartAfterFixture(TestContainerId, Guid.NewGuid().ToString(), new FixtureResult());

            if (Driver != null)
            {
                if (Driver is PaidWebDriver driver)
                    _allure.WrapInStep(UpdateApi, $"Updating {driver.Name} results through API");
                _allure.WrapInStep(KillWebDriver, $"Killing {BrowserName}");
            }

            _allure.StopFixture(result => { result.stage = Stage.finished; });

            _allure.UpdateTestContainer(TestContainerId,
                x => x.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds());
            _allure.StopTestContainer(TestContainerId);
            _allure.WriteTestContainer(TestContainerId);
        }

        public UnickqSpecFlowSeleniumAllureGeneratorHelper(ITestRunner testRunner) : base(testRunner)
        {
            RootContainerId = Guid.NewGuid().ToString() + "f";
        }

        public List<Parameter> ParametersForBuild()
        {
            var list = new List<Parameter>();
            if (Driver is BrowserStackWebDriver bs)
            {
                var apiResponce =
                    bs.ExecuteApiCall($"https://api.browserstack.com/automate/sessions/{bs.SessionId}.json");
                list.Add(new Parameter
                {
                    name = "OS",
                    value = string.Concat(apiResponce.automation_session.os, " ",
                        apiResponce.automation_session.os_version)
                });
                list.Add(new Parameter
                {
                    name = "Session",
                    value = apiResponce.automation_session.public_url
                });
            }

            var caps = ((OpenQA.Selenium.Remote.RemoteWebDriver) Driver).Capabilities;

            list.Add(new Parameter
            {
                name = "Browser",
                value = string.Concat(caps.BrowserName, " ", caps.Version)
            });
            list.Add(new Parameter
            {
                name = "Screen",
                value = string.Concat(Driver.Manage().Window.Size.Width, "x", Driver.Manage().Window.Size.Height)
            });
            return list;
        }
    }
}