using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;
using Unickq.SeleniumHelper.Plugins;

[assembly: RuntimePlugin(typeof(RuntimePlugin))]

namespace Unickq.SeleniumHelper.Plugins
{
    public class RuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.RegisterGlobalDependencies += RuntimePluginEvents_RegisterGlobalDependencies;
        }

        private static void RuntimePluginEvents_RegisterGlobalDependencies(object sender, RegisterGlobalDependenciesEventArgs e)
        {
            var runtimeProvider = new NUnitRuntimeProvider();
            e.ObjectContainer.RegisterInstanceAs<IUnitTestRuntimeProvider>(runtimeProvider, "UnickqNUnit");
        }
    }
}
