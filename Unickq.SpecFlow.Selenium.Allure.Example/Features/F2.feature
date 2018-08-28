Feature: f2
As Dasdsadsa
sadas
asd
sad
sa
das
d
as

#@Browser:Chrome_local
#@Browser:Chrome_local2
#@Browser:Firefox_local
#@Browser:BrowserStack_Win10_Chrome
#@Browser:Cbt_Win10_Chrome
#@Browser:TestingBot_ElCapitan_Safari
#@Browser:SauceLabs_Win7_Firefox
#@Browser:BrowserStack_Win10_Chrome
#@Browser:ChromeDebugs
@Browser:ChromeDebug
@GoogleTranslate:DE
@GoogleTranslate:UA
Scenario Outline: Check website title
	Given I have opened https://translate.google.com/ 
	Then the title should contain '<string>'
	Examples: 
	| string  |
	| Gwoogle |