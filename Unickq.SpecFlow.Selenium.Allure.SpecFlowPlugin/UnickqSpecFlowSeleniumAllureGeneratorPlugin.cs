using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Utils;
using Unickq.SpecFlow.Selenium;

[assembly: GeneratorPlugin(typeof(UnickqSpecFlowSeleniumAllureGeneratorPlugin))]

namespace Unickq.SpecFlow.Selenium
{
    public class UnickqSpecFlowSeleniumAllureGeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents,
            GeneratorPluginParameters generatorPluginParameters)
        {
            generatorPluginEvents.RegisterDependencies += GeneratorPluginEvents_RegisterDependencies;
        }

        private static void GeneratorPluginEvents_RegisterDependencies(object sender, RegisterDependenciesEventArgs e)
        {
            var container = e.ObjectContainer;
            var projectSettings = container.Resolve<ProjectSettings>();
            var codeDomHelper = container.Resolve<CodeDomHelper>(projectSettings.ProjectPlatformSettings.Language);
            var generatorProvider = new UnickqSpecFlowSeleniumAllureGeneratorProvider(codeDomHelper);
            container.RegisterInstanceAs<IUnitTestGeneratorProvider>(generatorProvider,
                "Unickq.SpecFlow.Selenium.Allure");
        }
    }
}