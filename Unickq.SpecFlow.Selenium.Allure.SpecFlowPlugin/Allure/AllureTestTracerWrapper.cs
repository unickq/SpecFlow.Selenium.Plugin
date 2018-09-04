using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Allure.Commons;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Tracing;

namespace Unickq.SpecFlow.Selenium.Allure
{
    public class AllureTestTracerWrapper : TestTracer, ITestTracer
    {
        private const string NoMatchingStepMessage = "No matching step definition found for the step";
        private static readonly AllureLifecycle Allure = AllureLifecycle.Instance;
        private static readonly PluginConfiguration PluginConfiguration = PluginHelper.PluginConfiguration;

        public AllureTestTracerWrapper(ITraceListener traceListener, IStepFormatter stepFormatter,
            IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider, SpecFlowConfiguration specFlowConfiguration)
            : base(traceListener, stepFormatter, stepDefinitionSkeletonProvider, specFlowConfiguration)
        {
        }

        void ITestTracer.TraceStep(StepInstance stepInstance, bool showAdditionalArguments)
        {
            TraceStep(stepInstance, showAdditionalArguments);
            StartStep(stepInstance);
        }

        void ITestTracer.TraceStepDone(BindingMatch match, object[] arguments, TimeSpan duration)
        {
            TraceStepDone(match, arguments, duration);
            Allure.StopStep(x => x.status = Status.passed);
        }

        void ITestTracer.TraceError(Exception ex)
        {
            TraceError(ex);
            Allure.StopStep(x => x.status = Status.failed);
            FailScenario(ex);
        }

        void ITestTracer.TraceStepSkipped()
        {
            TraceStepSkipped();
            Allure.StopStep(x => x.status = Status.skipped);
        }

        void ITestTracer.TraceStepPending(BindingMatch match, object[] arguments)
        {
            TraceStepPending(match, arguments);
            Allure.StopStep(x => x.status = Status.skipped);
        }

        void ITestTracer.TraceNoMatchingStepDefinition(StepInstance stepInstance, ProgrammingLanguage targetLanguage,
            CultureInfo bindingCulture, List<BindingMatch> matchesWithoutScopeCheck)
        {
            TraceNoMatchingStepDefinition(stepInstance, targetLanguage, bindingCulture, matchesWithoutScopeCheck);
            Allure.StopStep(x => x.status = Status.broken);
            Allure.UpdateTestCase(x =>
            {
                x.status = Status.broken;
                x.statusDetails = new StatusDetails {message = NoMatchingStepMessage};
            });
        }

        private void StartStep(StepInstance stepInstance)
        {
            var stepResult = new StepResult
            {
                name = $"{stepInstance.Keyword} {stepInstance.Text}"
            };


            // parse MultilineTextArgument
            if (stepInstance.MultilineTextArgument != null)
                Allure.AddAttachment(
                    "multiline argument",
                    "text/plain",
                    Encoding.ASCII.GetBytes(stepInstance.MultilineTextArgument),
                    ".txt");

            var table = stepInstance.TableArgument;
            var isTableProcessed = table == null;

            // parse table as step params
            if (table != null)
            {
                var header = table.Header.ToArray();
                if (PluginConfiguration.stepArguments.convertToParameters)
                {
                    var parameters = new List<Parameter>();

                    // convert 2 column table into param-value
                    if (table.Header.Count == 2)
                    {
                        var paramNameMatch = Regex.IsMatch(header[0], PluginConfiguration.stepArguments.paramNameRegex);
                        var paramValueMatch =
                            Regex.IsMatch(header[1], PluginConfiguration.stepArguments.paramValueRegex);
                        if (paramNameMatch && paramValueMatch)
                        {
                            for (var i = 0; i < table.RowCount; i++)
                                parameters.Add(new Parameter {name = table.Rows[i][0], value = table.Rows[i][1]});

                            isTableProcessed = true;
                        }
                    }
                    // add step params for 1 row table
                    else if (table.RowCount == 1)
                    {
                        for (var i = 0; i < table.Header.Count; i++)
                            parameters.Add(new Parameter {name = header[i], value = table.Rows[0][i]});

                        isTableProcessed = true;
                    }

                    stepResult.parameters = parameters;
                }
            }

            Allure.StartStep(Guid.NewGuid().ToString(), stepResult);
        }

        private static void FailScenario(Exception ex)
        {
            Allure.UpdateTestCase(
                testCase =>
                {
                    testCase.statusDetails = PluginHelper.GetStatusDetails(ex);
                    testCase.status = testCase.statusDetails.trace.Contains("OpenQA.Selenium")
                        ? Status.broken
                        : Status.failed;
                });
        }
    }
}