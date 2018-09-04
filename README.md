# Unickq.SpecFlow.Selenium.Plugin
   
[![Build status](https://ci.appveyor.com/api/projects/status/24ct5aixw0thsii4?svg=true)](https://ci.appveyor.com/project/unickq/specflow-selenium-plugin)


[![NuGet Unickq.SpecFlow.Selenium](http://flauschig.ch/nubadge.php?id=Unickq.SpecFlow.Selenium)](https://www.nuget.org/packages/Unickq.SpecFlow.Selenium)

Old version:
[![NuGet Unickq.SeleniumHelper](http://flauschig.ch/nubadge.php?id=Unickq.SeleniumHelper)](https://www.nuget.org/packages/Unickq.SeleniumHelper)

SpecFlow plugin for WebDriver instances generation. Write your tests easily

### Features:
- Tests generation using tags *@Browser:BrowserType*, where BrowserType definded in *App.config*
- [Specflow 2.2.0](https://github.com/techtalk/SpecFlow/releases/tag/V2.2.0) support
- Ability to run features in [parralel](https://github.com/techtalk/SpecFlow/wiki/Parallel-Execution) with [NUnit3](https://github.com/nunit/docs/wiki/Parallelizable-Attribute)
- Support of [BrowserStack](browserstack.com/), [SauceLabs](https://saucelabs.com/), [TestingBot](https://testingbot.com), [CrossBrowserTesting](https://crossbrowsertesting.com/) with required RestApi methods and keys.
- Scenario annotation with ***Key:Value*** syntax
- Custom WebDriver configuration

[![N|Solid](https://raw.githubusercontent.com/unickq/SpecFlow.Selenium.Plugin/master/Example.png)]()

### Usage:
1. Download nuget package with all dependencies
2. Create App.config with desired WebDriver instances
3. Enjoy 😁

##### Examples:
* [Example project](https://github.com/unickq/SpecFlow.Selenium.Plugin/tree/master/Unickq.SpecFlow.Selenium.Example)
* [Wiki](https://github.com/unickq/SpecFlow.Selenium.Plugin/wiki)

###### Fork of [Baseclass.Contrib.SpecFlow.Selenium.NUnit](https://github.com/baseclass/Contrib.SpecFlow.Selenium.NUnit) for SpeckFlow 2 support