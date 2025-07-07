param siteName string

var apiSiteName = siteName
//@[4:15) [no-unused-vars (Warning)] Variable "apiSiteName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |apiSiteName|
var apiAppInsightsName = '${siteName}Insights'
//@[4:22) [no-unused-vars (Warning)] Variable "apiAppInsightsName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |apiAppInsightsName|

