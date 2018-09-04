using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;
using Unickq.SpecFlow.Selenium;
using Unickq.SpecFlow.Selenium.Allure;

[assembly: RuntimePlugin(typeof(UnickqSpecFlowSeleniumAllureRuntimePlugin))]

namespace Unickq.SpecFlow.Selenium
{
    public class UnickqSpecFlowSeleniumAllureRuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.RegisterGlobalDependencies += (sender, args) =>
                args.ObjectContainer.RegisterInstanceAs<IUnitTestRuntimeProvider>(new NUnitRuntimeProvider(),
                    "Unickq.SpecFlow.Selenium.Allure");
            runtimePluginEvents.CustomizeTestThreadDependencies += (sender, args) =>
                args.ObjectContainer.RegisterTypeAs<AllureTestTracerWrapper, ITestTracer>();
        }
    }
}