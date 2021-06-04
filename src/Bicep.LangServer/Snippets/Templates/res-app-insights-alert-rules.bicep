// Application Insights Alert Rules
resource ${1:appInsightsAlertRules} 'Microsoft.Insights/alertrules@2016-03-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    name: ${3:'name'}
    description: ${4:'description'}
    isEnabled: false
    condition: {
      failedLocationCount: ${5:'failedLocationCount'}
      'odata.type': '${6|Microsoft.Azure.Management.Insights.Models.LocationThresholdRuleCondition,Microsoft.Azure.Management.Insights.Models.ManagementEventRuleCondition,Microsoft.Azure.Management.Insights.Models.ThresholdRuleCondition|}'
      dataSource: {
        'odata.type': '${7|Microsoft.Azure.Management.Insights.Models.RuleManagementEventDataSource,Microsoft.Azure.Management.Insights.Models.RuleMetricDataSource|}'
        resourceUri: ${8:'resourceUri'}
      }
      windowSize: ${9:'windowSize'}
    }
    action: {
      'odata.type': '${10|Microsoft.Azure.Management.Insights.Models.RuleEmailAction,Microsoft.Azure.Management.Insights.Models.RuleWebhookAction|}'
      sendToServiceOwners: true
    }
  }
}
