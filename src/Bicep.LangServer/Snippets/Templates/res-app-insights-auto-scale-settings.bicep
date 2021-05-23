// Application Insights Auto Scale Settings
resource ${1:appInsightsAutoScaleSettings} 'Microsoft.Insights/autoscalesettings@2015-04-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  tags: {
    Application_Type: '${3|web,other|}'
    'hidden-link:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${4:appServicePlan}': 'Resource'
  }
  properties: {
    name: ${5:'name'}
    profiles: [
      {
        name: ${6:'name'}
        capacity: {
          minimum: ${7:'minimum'}
          maximum: ${8:'maximum'}
          default: ${9:'default'}
        }
        rules: [
          {
            metricTrigger: {
              metricName: ${10:'name'}
              metricResourceUri: '${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${4:appServicePlan}'
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT10M'
              timeAggregation: 'Average'
              operator: 'GreaterThan'
              threshold: 80
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value:  ${11:'value'}
              cooldown: 'PT10M'
            }
          }
          {
            metricTrigger: {
              metricName: ${12:'name'}
              metricResourceUri: '${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${4:appServicePlan}'
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT1H'
              timeAggregation: 'Average'
              operator: 'LessThan'
              threshold: 60
            }
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: ${13:'value'}
              cooldown: 'PT1H'
            }
          }
        ]
      }
    ]
    enabled: false
    targetResourceUri: '${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${4:appServicePlan}'
  }
}
