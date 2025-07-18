@description('Name for the Application Insights')
param appInsightsName string

@description('Location for resource.')
param location string

@description('Log Analytics Workspace ID to send App Insights Log To')
param logAnalyticsWorkspaceID string

@description('What language was used to deploy this resource')
param language string

resource ai_appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: toLower('ai-${appInsightsName}')
  location: location
  kind: 'string'
  tags: {
    displayName: 'AppInsight'
    Language: language
  }
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspaceID
  }
}

output appInsightsInstrumentationKey string = reference(ai_appInsights.id, '2020-02-02').InstrumentationKey
//@[46:88) [use-resource-symbol-reference (Warning)] Use a resource reference instead of invoking function "reference". This simplifies the syntax and allows Bicep to better understand your deployment dependency graph. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-resource-symbol-reference) |reference(ai_appInsights.id, '2020-02-02')|

