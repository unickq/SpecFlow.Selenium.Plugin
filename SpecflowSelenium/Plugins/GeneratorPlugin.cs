using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Utils;
using Unickq.SeleniumHelper.Plugins;

[assembly: GeneratorPlugin(typeof(GeneratorPlugin))]

namespace Unickq.SeleniumHelper.Plugins
{
    public class GeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters)
        {
            generatorPluginEvents.RegisterDependencies += GeneratorPluginEvents_RegisterDependencies;
        }

        private static void GeneratorPluginEvents_RegisterDependencies(object sender, RegisterDependenciesEventArgs e)
        {           
            var container = e.ObjectContainer;
            var projectSettings = container.Resolve<ProjectSettings>();
            var codeDomHelper = container.Resolve<CodeDomHelper>(projectSettings.ProjectPlatformSettings.Language);
            var generatorProvider = new UnickqNUnitTestGeneratorProvider(codeDomHelper);
            container.RegisterInstanceAs<IUnitTestGeneratorProvider>(generatorProvider, "UnickqNUnit");
        }
    }
}
