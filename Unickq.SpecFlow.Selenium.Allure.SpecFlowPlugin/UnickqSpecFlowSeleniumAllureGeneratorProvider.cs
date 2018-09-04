using System.CodeDom;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Utils;

namespace Unickq.SpecFlow.Selenium
{
    public class UnickqSpecFlowSeleniumAllureGeneratorProvider : UnickqSpecFlowSeleniumGeneratorProvider
    {
        public UnickqSpecFlowSeleniumAllureGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        {
        }

        public override void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestInitializeMethod, TestSetupAttr);
            generationContext.Namespace.Imports.Add(new CodeNamespaceImport("Unickq.SpecFlow.Selenium"));

            generationContext.TestClassInitializeMethod.Statements.Add(
                GenerateCodeSnippetStatement("helper = new UnickqSpecFlowSeleniumAllureGeneratorHelper(testRunner);"));
            generationContext.TestClassInitializeMethod.Statements.Add(
                GenerateCodeSnippetStatement("helper.FeatureSetup();"));
            generationContext.TestInitializeMethod.Statements.Add(GenerateCodeSnippetStatement("helper.SetUp();"));
        }
    }
}