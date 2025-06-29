// $1 = appInsightsAlertRules
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = 'description'
// $6 = 3
// $7 = Microsoft.Azure.Management.Insights.Models.LocationThresholdRuleCondition
// $8 = Microsoft.Azure.Management.Insights.Models.RuleManagementEventDataSource
// $9 = 'resourceUri'
// $10 = 'windowSize'
// $11 = Microsoft.Azure.Management.Insights.Models.RuleEmailAction

param location string

resource appInsightsAlertRules 'Microsoft.Insights/alertrules@2024-03-01' = {
//@[31:73) [BCP081 (Warning)] Resource type "Microsoft.Insights/alertrules@2024-03-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'Microsoft.Insights/alertrules@2024-03-01'|
  name: 'name'
  location: location
  properties: {
    name: 'name'
    description: 'description'
    isEnabled: false
    condition: {
      failedLocationCount: 3
      'odata.type': 'Microsoft.Azure.Management.Insights.Models.LocationThresholdRuleCondition'
      dataSource: {
        'odata.type': 'Microsoft.Azure.Management.Insights.Models.RuleManagementEventDataSource'
        resourceUri: 'resourceUri'
      }
      windowSize: 'windowSize'
    }
    action: {
      'odata.type': 'Microsoft.Azure.Management.Insights.Models.RuleEmailAction'
      sendToServiceOwners: true
    }
  }
}


