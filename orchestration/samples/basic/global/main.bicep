param apps {
  subscriptionId: string
  resourceGroup: string
  name: string
}[]

param afdName string

resource containerApps 'Microsoft.App/containerApps@2025-02-02-preview' existing = [for app in apps: {
  scope: resourceGroup(app.subscriptionId, app.resourceGroup)
  name: app.name
}]

var afd = {
  endpointName: 'hello-world-endpoint'
  routeName: 'hello-world-route'
  originGroup: 'hello-world-group'
}

module profile 'br/public:avm/res/cdn/profile:0.14.0' = {
  params: {
    name: afdName
    sku: 'Standard_AzureFrontDoor'
    afdEndpoints: [
      {
        name: afd.endpointName
        routes: [
          {
            name: afd.routeName
            originGroupName: afd.originGroup
            ruleSets: []
          }
        ]
      }
    ]
    location: 'global'
    managedIdentities: {
      systemAssigned: true
    }
    originGroups: [
      {
        loadBalancingSettings: {
          additionalLatencyInMilliseconds: 50
          sampleSize: 4
          successfulSamplesRequired: 3
        }
        name: afd.originGroup
        origins: [for (app, i) in apps: {
          hostName: containerApps[i].properties.configuration.ingress.fqdn
          name: 'containerapp-${i}'
        }]
        healthProbeSettings: {
          probePath: '/'
          probeRequestType: 'GET'
          probeProtocol: 'Https'
          probeIntervalInSeconds: 60
        }
      }
    ]
    originResponseTimeoutSeconds: 60
  }
}
