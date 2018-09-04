using System;
using System.Collections.Generic;
using Allure.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Unickq.SpecFlow.Selenium.Allure;
using Unickq.SpecFlow.Selenium.Exceptions;
using Unickq.SpecFlow.Selenium.Helpers;
using Unickq.SpecFlow.Selenium.WebDriverGrid;
using RemoteWebDriver = OpenQA.Selenium.Remote.RemoteWebDriver;

namespace Unickq.SpecFlow.Selenium
{
    public class UnickqSpecFlowSeleniumAllureGeneratorHelper : UnickqSpecFlowSeleniumGeneratorHelper
    {
        private readonly AllureLifecycle _allure = AllureLifecycle.Instance;

        public UnickqSpecFlowSeleniumAllureGeneratorHelper(ITestRunner testRunner) : base(testRunner)
        {
            RootContainerId = Guid.NewGuid() + "_fc";
        }

        public string RootContainerId { get; }
        public string TestContainerId { get; private set; }
        public string TestResultId { get; private set; }

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
            TestResultId = Guid.NewGuid() + "_test";
            TestContainerId = Guid.NewGuid() + "_tc";
            _allure.StartTestContainer(RootContainerId,
                new TestResultContainer {uuid = TestContainerId});
            _allure.StartBeforeFixture(TestContainerId, Guid.NewGuid().ToString(),
                new FixtureResult());

            var step = new StepResult
            {
                name = "Starting browser",
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                status = Status.passed
            };

            var test = new TestResult
            {
                uuid = TestResultId,
                name = TestContext.CurrentContext.Test.Name,
                fullName = TestContext.CurrentContext.Test.FullName,
                descriptionHtml = $"<pre><code>{TestRunner.FeatureContext.FeatureInfo.Description}</pre></code>",
                labels = new List<Label>
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.TestMethod(TestContext.CurrentContext.Test.MethodName)
                }
            };

            try
            {
                base.SetUp();
                step.name = $"Starting {BrowserName}";
                step.stage = Stage.finished;
            }
            catch (SpecFlowSeleniumException e)
            {
                step.status = Status.failed;
                step.statusDetails = PluginHelper.GetStatusDetails(e);
                step.stage = Stage.interrupted;

                test.status = Status.none;
                test.statusDetails = PluginHelper.GetStatusDetails(e);

                _allure.UpdateFixture(fixture => { fixture.status = Status.failed; });
                Assert.Inconclusive(e.Message);
            }
            finally
            {
                test.parameters.AddRange(ParametersForBuild());
                step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                _allure.StopFixture(result =>
                {
                    result.stage = Stage.finished;
                    result.steps.Add(step);
                });
                _allure.StartTestCase(TestContainerId, test);
            }
        }

        public override void TearDown()
        {
            _allure.UpdateTestCase(test =>
            {
                var packageName = TestContext.CurrentContext.Test.ClassName;
                var className = packageName.Substring(packageName.LastIndexOf('.') + 1);

                test.labels.AddRange(new List<Label>
                {
                    Label.Suite(TestRunner.FeatureContext?.FeatureInfo.Title.Trim()),
                    Label.Feature(TestRunner.FeatureContext?.FeatureInfo.Title),
                    Label.TestClass(className),
                    Label.Package(packageName)
                });

                if (Driver != null)
                {
                    test.labels.Add(Label.ParentSuite(BrowserName));
                    try
                    {
                        test.parameters.Add(new Parameter
                        {
                            name = "Screen",
                            value = string.Concat(Driver.Manage().Window.Size.Width, "x",
                                Driver.Manage().Window.Size.Height)
                        });
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
                else
                {
                    test.labels.Add(Label.ParentSuite(test.statusDetails.message));
                }

                if (TestRunner.ScenarioContext != null)
                {
                    var tags = PluginHelper.GetTags(TestRunner.FeatureContext?.FeatureInfo,
                        TestRunner.ScenarioContext?.ScenarioInfo);
                    test.labels.Add(Label.SubSuite(TestRunner.ScenarioContext?.ScenarioInfo.Title));
                    test.labels.AddRange(tags.Item1);
                    test.links = tags.Item2;
                }
            });


            if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Passed)
            {
                if (Driver != null)
                    try
                    {
                        var screen = ((ITakesScreenshot) Driver).GetScreenshot();
                        _allure.AddAttachment("ScreenShot", "image/png", screen.AsByteArray);
                    }
                    catch (Exception e)
                    {
                        _allure.UpdateTestCase(test =>
                            test.parameters.Add(new Parameter {name = "ScreenShotError", value = e.Message}));
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

            _allure.StopFixture(fixture => { fixture.stage = Stage.finished; });
            _allure.UpdateTestContainer(TestContainerId,
                testContainer => testContainer.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds());
            _allure.StopTestContainer(TestContainerId);
            _allure.WriteTestContainer(TestContainerId);
        }

        private IEnumerable<Parameter> ParametersForBuild()
        {
            var list = new List<Parameter>();
            try
            {
                if (Driver != null)
                {
                    if (Driver is BrowserStackWebDriver bs)
                    {
                        var apiResponse =
                            bs.ExecuteApiCall($"https://api.browserstack.com/automate/sessions/{bs.SessionId}.json");
                        if (apiResponse != null)
                        {
                            list.Add(new Parameter
                            {
                                name = "Session",
                                value = apiResponse.automation_session.public_url
                            });
                            list.Add(new Parameter
                            {
                                name = "OS",
                                value = string.Concat(apiResponse.automation_session.os, " ",
                                    apiResponse.automation_session.os_version)
                            });
                        }
                    }

                    var caps = ((RemoteWebDriver) Driver).Capabilities;
                    list.Add(new Parameter
                    {
                        name = "Browser",
                        value = string.Concat(caps.GetCapability("browserName"), " ", caps.GetCapability("version"))
                    });
                }
            }
            catch (Exception e)
            {
                list.Add(new Parameter
                {
                    name = e.GetType().Name,
                    value = e.Message
                });
            }

            return list;
        }
    }
}