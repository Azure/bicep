// Application Insights Auto Scale Settings
resource ${1:appInsightsAutoScaleSettings} 'Microsoft.Insights/autoscalesettings@2015-04-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  tags: {
    Application_Type: '${3|web,other|}'
    'hidden-link:${4:appServiceId}': 'Resource'
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
              metricResourceUri: ${11:'metricResourceUri'}
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
              value:  ${12:'value'}
              cooldown: 'PT10M'
            }
          }
          {
            metricTrigger: {
              metricName: ${13:'metricName'}
              metricResourceUri: ${14:'metricResourceUri'}
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
              value: ${15:'value'}
              cooldown: 'PT1H'
            }
          }
        ]
      }
    ]
    enabled: false
    targetResourceUri: ${16:'targetResourceUri'}
  }
}
