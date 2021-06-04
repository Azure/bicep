resource appInsightsAlertRules 'Microsoft.Insights/alertrules@2016-03-01' = {
  name: 'name'
  location: resourceGroup().location
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

