// Application Insights Auto Scale Settings
resource /*${1:appInsightsAutoScaleSettings}*/appInsightsAutoScaleSettings 'Microsoft.Insights/autoscalesettings@2015-04-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  tags: {
    Application_Type: /*'${4|web,other|}'*/'web'
    /*'hidden-link:${5:appServiceId}'*/'appServiceId': 'Resource'
  }
  properties: {
    name: /*${6:'name'}*/'name'
    profiles: [
      {
        name: /*${7:'name'}*/'name'
        capacity: {
          minimum: /*${8:'minimum'}*/'minimum'
          maximum: /*${9:'maximum'}*/'maximum'
          default: /*${10:'default'}*/'default'
        }
        rules: [
          {
            metricTrigger: {
              metricName: /*${11:'name'}*/'name'
              metricResourceUri: /*${12:'metricResourceUri'}*/'metricResourceUri'
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
              value:  /*${13:'value'}*/'value'
              cooldown: 'PT10M'
            }
          }
          {
            metricTrigger: {
              metricName: /*${14:'metricName'}*/'metricName'
              metricResourceUri: /*${15:'metricResourceUri'}*/'metricResourceUri'
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
              value: /*${16:'value'}*/'value'
              cooldown: 'PT1H'
            }
          }
        ]
      }
    ]
    enabled: false
    targetResourceUri: /*${17:'targetResourceUri'}*/'targetResourceUri'
  }
}
