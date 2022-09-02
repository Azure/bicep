param siteName string

var apiSiteName = siteName
//@[4:15) [no-unused-vars (Warning)] Variable "apiSiteName" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |apiSiteName|
var apiAppInsightsName = '${siteName}Insights'
//@[4:22) [no-unused-vars (Warning)] Variable "apiAppInsightsName" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |apiAppInsightsName|
