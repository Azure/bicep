resource trafficManagerProfile 'Microsoft.Network/trafficManagerProfiles@2018-04-01' = {
  name: 'name'
  location: 'global'
  properties: {
    profileStatus: 'Enabled'
    trafficRoutingMethod: 'Performance'
    dnsConfig: {
      relativeName: 'dnsConfigRelativeName'
      ttl: 30
    }
    monitorConfig: {
      protocol: 'HTTP'
      port: 80
      path: 'path'
      intervalInSeconds: 30
      timeoutInSeconds: 5
      toleratedNumberOfFailures: 3
    }
    endpoints: [
      {
        properties: {
          targetResourceId: 'targetResourceId'
          endpointStatus: 'Enabled'
          weight: 100
          priority: 1
        }
      }
    ]
  }
}

