Feature: f2

#@Browser:Chrome_local
#@Browser:Chrome_local2
#@Browser:Firefox_local
#@Browser:BrowserStack_Win10_Chrome
#@Browser:Cbt_Win10_Chrome
#@Browser:TestingBot_ElCapitan_Safari
#@Browser:SauceLabs_Win7_Firefox
#@GoogleTranslate:FR
#@GoogleTranslate:DE
#@GoogleTranslate:UK

@Browser:ChromeDebug
@Browser:FirefoxDebug
Scenario Outline: Check website title
	Given I have opened <URL>
	Then the title should contain '<string>'
Examples: 
| URL							| string |
| https://translate.google.com/ | Google |
