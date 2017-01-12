Feature: ExampleFeature

@Browser:BrowserStack
@Browser:TestingBot
@Browser:SauceLabs
#@Ignore
Scenario Outline: Check website titles
	Given I have opened <URL>
	Then the titls should contains <titlePart>
Examples: 
| URL               | titlePart |
| http://google.com | google    |
#| http://apple.com  | apple     |
#
#@Browser:Chrome
#@Browser:Firefox
#@Ignore
#Scenario Outline: Check website titles with ignored tag
#	Given I have opened <URL>
#	Then the titls should contains <titlePart>
#Examples: 
#| URL					| titlePart |
#| http://microsoft.com  | ms		|

