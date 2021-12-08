// $1 = appInsightsAutoScaleSettings
// $2 = 'name'
// $3 = location
// $4 = web
// $5 = appServiceId
// $6 = 'name'
// $7 = 'name'
// $8 = 'minimum'
// $9 = 'maximum'
// $10 = 'default'
// $11 = 'name'
// $12 = 'metricResourceUri'
// $13 = 'value'
// $14 = 'metricName'
// $15 = 'metricResourceUri'
// $16 = 'value'
// $17 = 'metricResourceUri'

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

