Feature: ExampleFeature

@Browser:IE_local
@Browser:BrowserStack_Win10_Chrome
@Browser:TestingBot_ElCapitan_Safari
@Browser:SauceLabs_Win7_Firefox
Scenario Outline: Check website title
	Given I have opened <URL>
	Then the title should contain <string>
Examples: 
| URL               | string |
| http://google.com | google |
