using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;
using Unickq.SpecFlow.Selenium;

[assembly: RuntimePlugin(typeof(UnickqSpecFlowSeleniumRuntimePlugin))]

namespace Unickq.SpecFlow.Selenium
{
    public class UnickqSpecFlowSeleniumRuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.RegisterGlobalDependencies += RuntimePluginEvents_RegisterGlobalDependencies;
        }

        private static void RuntimePluginEvents_RegisterGlobalDependencies(object sender,
            RegisterGlobalDependenciesEventArgs e)
        {
            e.ObjectContainer.RegisterInstanceAs<IUnitTestRuntimeProvider>(new NUnitRuntimeProvider(),
                "Unickq.SpecFlow.Selenium");
        }
    }
}