// $1 = appInsightsAutoScaleSettings
// $2 = 'name'
// $3 = web
// $4 = appServiceId
// $5 = 'name'
// $6 = 'name'
// $7 = 'minimum'
// $8 = 'maximum'
// $9 = 'default'
// $10 = 'name'
// $11 = 'metricResourceUri'
// $12 = 'value'
// $13 = 'metricName'
// $14 = 'metricResourceUri'
// $15 = 'value'
// $16 = 'metricResourceUri'

param location string

resource appInsightsAutoScaleSettings 'Microsoft.Insights/autoscalesettings@2015-04-01' = {
  name: 'name'
  location: location
  tags: {
    Application_Type: 'web'
    'hidden-link:appServiceId': 'Resource'
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
              metricResourceUri: 'metricResourceUri'
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
              metricName: 'metricName'
              metricResourceUri: 'metricResourceUri'
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
    targetResourceUri: 'metricResourceUri'
  }
}
// Insert snippet here

