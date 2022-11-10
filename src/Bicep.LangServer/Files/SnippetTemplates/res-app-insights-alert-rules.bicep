// Application Insights Alert Rules
resource /*${1:appInsightsAlertRules}*/appInsightsAlertRules 'Microsoft.Insights/alertrules@2016-03-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    name: /*${4:'name'}*/'name'
    description: /*${5:'description'}*/'description'
    isEnabled: false
    condition: {
      failedLocationCount: /*${6:'failedLocationCount'}*/'failedLocationCount'
      'odata.type': /*'${7|Microsoft.Azure.Management.Insights.Models.LocationThresholdRuleCondition,Microsoft.Azure.Management.Insights.Models.ManagementEventRuleCondition,Microsoft.Azure.Management.Insights.Models.ThresholdRuleCondition|}'*/'Microsoft.Azure.Management.Insights.Models.LocationThresholdRuleCondition'
      dataSource: {
        'odata.type': /*'${8|Microsoft.Azure.Management.Insights.Models.RuleManagementEventDataSource,Microsoft.Azure.Management.Insights.Models.RuleMetricDataSource|}'*/'Microsoft.Azure.Management.Insights.Models.RuleManagementEventDataSource'
        resourceUri: /*${9:'resourceUri'}*/'resourceUri'
      }
      windowSize: /*${10:'windowSize'}*/'windowSize'
    }
    action: {
      'odata.type': /*'${11|Microsoft.Azure.Management.Insights.Models.RuleEmailAction,Microsoft.Azure.Management.Insights.Models.RuleWebhookAction|}'*/'Microsoft.Azure.Management.Insights.Models.RuleEmailAction'
      sendToServiceOwners: true
    }
  }
}
