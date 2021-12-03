// $1 = appInsightsAlertRules
// $2 = 'name'
// $3 = 'name'
// $4 = 'description'
// $5 = 3
// $6 = Microsoft.Azure.Management.Insights.Models.LocationThresholdRuleCondition
// $7 = Microsoft.Azure.Management.Insights.Models.RuleManagementEventDataSource
// $8 = 'resourceUri'
// $9 = 'windowSize'
// $10 = Microsoft.Azure.Management.Insights.Models.RuleEmailAction

param location string

resource appInsightsAlertRules 'Microsoft.Insights/alertrules@2016-03-01' = {
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
// Insert snippet here

