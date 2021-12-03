// Application Insights Auto Scale Settings
resource /*${1:appInsightsAutoScaleSettings}*/appInsightsAutoScaleSettings 'Microsoft.Insights/autoscalesettings@2015-04-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  tags: {
    Application_Type: /*'${3|web,other|}'*/'web'
    /*'hidden-link:${4:appServiceId}'*/'appServiceId': 'Resource'
  }
  properties: {
    name: /*${5:'name'}*/'name'
    profiles: [
      {
        name: /*${6:'name'}*/'name'
        capacity: {
          minimum: /*${7:'minimum'}*/'minimum'
          maximum: /*${8:'maximum'}*/'maximum'
          default: /*${9:'default'}*/'default'
        }
        rules: [
          {
            metricTrigger: {
              metricName: /*${10:'name'}*/'name'
              metricResourceUri: /*${11:'metricResourceUri'}*/'metricResourceUri'
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
              value:  /*${12:'value'}*/'value'
              cooldown: 'PT10M'
            }
          }
          {
            metricTrigger: {
              metricName: /*${13:'metricName'}*/'metricName'
              metricResourceUri: /*${14:'metricResourceUri'}*/'metricResourceUri'
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
              value: /*${15:'value'}*/'value'
              cooldown: 'PT1H'
            }
          }
        ]
      }
    ]
    enabled: false
    targetResourceUri: /*${16:'targetResourceUri'}*/'targetResourceUri'
  }
}
