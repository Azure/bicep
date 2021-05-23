resource appInsightsAutoScaleSettings 'Microsoft.Insights/autoscalesettings@2015-04-01' = {
  name: 'name'
  location: resourceGroup().location
  tags: {
    Application_Type: 'web'
    'hidden-link:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan': 'Resource'
  }
  properties: {
    name: 'name'
    profiles: [
      {
        name: 'name'
        capacity: {
          minimum: 'minimum'
          maximum: 'maximum'
          default: 'default'
        }
        rules: [
          {
            metricTrigger: {
              metricName: 'name'
              metricResourceUri: '${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan'
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
              value:  'value'
              cooldown: 'PT10M'
            }
          }
          {
            metricTrigger: {
              metricName: 'name'
              metricResourceUri: '${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan'
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
              value: 'value'
              cooldown: 'PT1H'
            }
          }
        ]
      }
    ]
    enabled: false
    targetResourceUri: '${resourceGroup().id}/providers/Microsoft.Web/serverfarms/appServicePlan'
  }
}

