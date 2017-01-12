using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Utils;

namespace Unickq.SeleniumHelper.Plugins
{
    public class UnickqNUnitTestGeneratorProvider : IUnitTestGeneratorProvider
    {
        private const string DefaultMethodIndent = "            ";
        private const string TestfixtureAttr = "NUnit.Framework.TestFixtureAttribute";
        private const string TestAttr = "NUnit.Framework.TestAttribute";
        private const string RowAttr = "NUnit.Framework.TestCaseAttribute";
        private const string CategoryAttr = "NUnit.Framework.CategoryAttribute";
        private const string TestsetupAttr = "NUnit.Framework.SetUpAttribute";
        private const string TestfixturesetupAttr = "NUnit.Framework.OneTimeSetUp";
        private const string TestfixtureteardownAttr = "NUnit.Framework.OneTimeTearDown";
        private const string TestteardownAttr = "NUnit.Framework.TearDownAttribute";
        private const string IgnoreAttr = "NUnit.Framework.IgnoreAttribute";
        private const string DescriptionAttr = "NUnit.Framework.DescriptionAttribute";
        private readonly CodeDomHelper _codeDomHelper;
        private bool _scenarioSetupMethodsAdded;

        public UnickqNUnitTestGeneratorProvider(CodeDomHelper codeDomHelper)
        {
            _codeDomHelper = codeDomHelper;
        }

        public bool SupportsRowTests => true;
        public bool SupportsAsyncTests => false;

        public void SetTestMethodCategories(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            var categories = scenarioCategories as string[] ?? scenarioCategories.ToArray();
            _codeDomHelper.AddAttributeForEachValue(testMethod, CategoryAttr,
                categories.Where(cat => !cat.StartsWith("Browser:")));

            var hasBrowser = false;

            foreach (
                var browser in
                categories.Where(cat => cat.StartsWith("Browser:")).Select(cat => cat.Replace("Browser:", "")))
            {
                testMethod.UserData.Add("Browser:" + browser, browser);

                var withBrowserArgs = new[] {new CodeAttributeArgument(new CodePrimitiveExpression(browser))}
                    .Concat(new[]
                    {
                        new CodeAttributeArgument("Category", new CodePrimitiveExpression(browser)),
                        new CodeAttributeArgument("TestName",
                            new CodePrimitiveExpression($"{testMethod.Name} on {browser}"))
                    })
                    .ToArray();

                _codeDomHelper.AddAttribute(testMethod, RowAttr, withBrowserArgs);

                hasBrowser = true;
            }

            if (hasBrowser)
            {
                if (!_scenarioSetupMethodsAdded)
                {
                    generationContext.ScenarioInitializeMethod.Statements.Add(
                        new CodeSnippetStatement(DefaultMethodIndent + "if(this.driver != null)"));
                    generationContext.ScenarioInitializeMethod.Statements.Add(
                        new CodeSnippetStatement(DefaultMethodIndent +
                                                 "ScenarioContext.Current.Add(\"Driver\", this.driver);"));
                    generationContext.ScenarioInitializeMethod.Statements.Add(
                        new CodeSnippetStatement(DefaultMethodIndent + "if(this.container != null)"));
                    generationContext.ScenarioInitializeMethod.Statements.Add(
                        new CodeSnippetStatement(DefaultMethodIndent +
                                                 "  ScenarioContext.Current.Add(\"Container\", this.container);"));
                    _scenarioSetupMethodsAdded = true;
                }

                testMethod.Statements.Insert(0,
                    new CodeSnippetStatement(DefaultMethodIndent + "InitializeSelenium(browser);"));
                testMethod.Parameters.Insert(0, new CodeParameterDeclarationExpression("System.string", "browser"));
            }
        }

        public void SetRow(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            var args = arguments.Select(
                arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            var exampleTagExpressionList = tags.Select(t => new CodePrimitiveExpression(t)).ToArray();
            var exampleTagsExpression = exampleTagExpressionList.Length == 0
                ? (CodeExpression) new CodePrimitiveExpression(null)
                : new CodeArrayCreateExpression(typeof(string[]), exampleTagExpressionList);
            args.Add(new CodeAttributeArgument(exampleTagsExpression));

            if (isIgnored) args.Add(new CodeAttributeArgument("Ignored", new CodePrimitiveExpression(true)));

            var browsers = testMethod.UserData.Keys.OfType<string>()
                .Where(key => key.StartsWith("Browser:"))
                .Select(key => (string) testMethod.UserData[key]).ToArray();

            if (browsers.Any())
            {
                foreach (
                    var codeAttributeDeclaration in
                    testMethod.CustomAttributes.Cast<CodeAttributeDeclaration>()
                        .Where(attr => attr.Name == RowAttr && attr.Arguments.Count == 3)
                        .ToList())
                    testMethod.CustomAttributes.Remove(codeAttributeDeclaration);

                foreach (var browser in browsers)
                {
                    var argsString =
                        string.Concat(
                            args.Take(args.Count - 1)
                                .Select(arg => $"\"{((CodePrimitiveExpression) arg.Value).Value}\" ,"));
                    argsString = argsString.TrimEnd(' ', ',');

                    var withBrowserArgs = new[] {new CodeAttributeArgument(new CodePrimitiveExpression(browser))}
                        .Concat(args)
                        .Concat(new[]
                        {
                            new CodeAttributeArgument("Category", new CodePrimitiveExpression(browser)),
                            new CodeAttributeArgument("TestName",
                                new CodePrimitiveExpression($"{testMethod.Name} on {browser} with: {argsString}"))
                        })
                        .ToArray();

                    _codeDomHelper.AddAttribute(testMethod, RowAttr, withBrowserArgs);
                }
            }
            else
            {
                _codeDomHelper.AddAttribute(testMethod, RowAttr, args.ToArray());
            }
        }

        public void SetTestClass(TestClassGenerationContext generationContext,
            string featureTitle, string featureDescription)
        {
            _codeDomHelper.AddAttribute(generationContext.TestClass, TestfixtureAttr);
            _codeDomHelper.AddAttribute(generationContext.TestClass, DescriptionAttr, featureTitle);
            generationContext.Namespace.Imports.Add(new CodeNamespaceImport("Autofac"));
            generationContext.Namespace.Imports.Add(new CodeNamespaceImport("Autofac.Configuration"));
            generationContext.TestClass.Members.Add(new CodeMemberField("OpenQA.Selenium.IWebDriver", "driver"));
            generationContext.TestClass.Members.Add(new CodeMemberField("IContainer", "container"));
            CreateInitializeSeleniumMethod(generationContext);
        }

        public void SetTestClassCategories(TestClassGenerationContext generationContext,
            IEnumerable<string> featureCategories)
        {
            _codeDomHelper.AddAttributeForEachValue(generationContext.TestClass, CategoryAttr, featureCategories);
        }

        public void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
            _codeDomHelper.AddAttribute(generationContext.TestClassCleanupMethod, TestfixtureteardownAttr);
        }

        public void SetTestClassIgnore(TestClassGenerationContext generationContext)
        {
            _codeDomHelper.AddAttribute(generationContext.TestClass, IgnoreAttr, "Test feature is ignored\n");
        }

        public void SetTestClassInitializeMethod(
            TestClassGenerationContext generationContext)
        {
            _codeDomHelper.AddAttribute(generationContext.TestClassInitializeMethod, TestfixturesetupAttr);

            generationContext.TestClassInitializeMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent + "var builder = new ContainerBuilder();"));
            generationContext.TestClassInitializeMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent +
                                         "builder.RegisterModule(new ConfigurationSettingsReader());"));
            generationContext.TestClassInitializeMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent + "this.container = builder.Build();"));
        }

        public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
        {
            _codeDomHelper.AddAttribute(generationContext.TestCleanupMethod, TestteardownAttr);
        }

        public void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            _codeDomHelper.AddAttribute(generationContext.TestInitializeMethod, TestsetupAttr);
        }

        public void SetTestMethod(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, string scenarioTitle)
        {
            _codeDomHelper.AddAttribute(testMethod, TestAttr);
            _codeDomHelper.AddAttribute(testMethod, DescriptionAttr, scenarioTitle);
        }

        public void SetTestMethodIgnore(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod)
        {
            _codeDomHelper.AddAttribute(testMethod, IgnoreAttr, "Test scenario is ignored");
        }

        public void SetRowTest(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, string scenarioTitle)
        {
            SetTestMethod(generationContext, testMethod, scenarioTitle);
        }

        public void SetTestMethodAsRow(TestClassGenerationContext generationContext,
            CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName,
            IEnumerable<KeyValuePair<string, string>> arguments)
        {
        }

        public void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            generationContext.TestCleanupMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent +
                                         "try {" +
                                         "((Unickq.SeleniumHelper.WebDriverGrid.ICustomRemoteWebDriver) this.driver).UpdateTestResult();" +
                                         "} catch (System.Exception) {}"));
            generationContext.TestCleanupMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent +
                                         "try {" +
                                         "System.Threading.Thread.Sleep(50); " +
                                         "this.driver.Quit(); " +
                                         "} catch (System.Exception) {}"));
            generationContext.TestCleanupMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent + "this.driver = null;"));
            generationContext.TestCleanupMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent + "ScenarioContext.Current.Remove(\"Driver\");"));
            generationContext.TestCleanupMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent + "ScenarioContext.Current.Remove(\"Container\");"));
            generationContext.TestCleanupMethod.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent +
                                         "  System.Console.WriteLine(NUnit.Framework.TestContext.CurrentContext.Result.Outcome.Status.ToString());"));
        }

        public UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.None;
        }

        private static void CreateInitializeSeleniumMethod(
            TestClassGenerationContext generationContext)
        {
            var initializeSelenium = new CodeMemberMethod {Name = "InitializeSelenium"};
            initializeSelenium.Parameters.Add(new CodeParameterDeclarationExpression("System.String", "browser"));
            initializeSelenium.Statements.Add(
                new CodeSnippetStatement(DefaultMethodIndent +
                                         "this.driver = this.container.ResolveNamed<OpenQA.Selenium.IWebDriver>(browser);"));
            generationContext.TestClass.Members.Add(initializeSelenium);
        }
    }
}