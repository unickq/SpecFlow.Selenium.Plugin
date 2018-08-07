Feature: F1

@GoogleTranslate:DE
#@Lang:UA
#@Browser:ChromeDebug
@Browser:BrowserStack_Win10_Chrome
@Browser:BrowserStack_Win10_Firefox
#@Browser:BrowserStack_Win10_IE
Scenario Outline: Check website title
	Given I have opened <URL>
	Then the title should contain '<string>'
	Examples: 
	| URL                           | string        |
	| https://translate.google.com/ | Google        |

@Manual
Scenario: Check website title manual
	Given I have opened <URL>
	Then the title should contain '<string>'