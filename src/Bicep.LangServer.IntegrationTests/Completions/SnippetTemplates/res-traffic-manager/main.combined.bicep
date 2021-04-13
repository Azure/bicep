resource trafficManagerProfile 'Microsoft.Network/trafficManagerProfiles@2018-04-01' = {
  name: 'testTrafficManagerProfile'
  location: 'global'
  properties: {
    profileStatus: 'Enabled'
    trafficRoutingMethod: 'Performance'
    dnsConfig: {
      relativeName: 'testConfig'
      ttl: 30
    }
    monitorConfig: {
      protocol: 'HTTP'
      port: 80
      path: 'testPath'
      intervalInSeconds: 30
      timeoutInSeconds: 5
      toleratedNumberOfFailures: 3
    }
    endpoints: [
      {
        properties: {
          targetResourceId: 'testTargetId'
          endpointStatus: 'Enabled'
          weight: 100
          priority: 1
        }
      }
    ]
  }
}
