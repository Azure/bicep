// $1 = trafficManagerProfile
// $2 = 'name'
// $3 = 'Performance'
// $4 = 'dnsConfigRelativeName'
// $5 = 'HTTP'
// $6 = 80
// $7 = 'path'
// $8 = 30
// $9 = 5
// $10 = 3
// $11 = 'targetResourceId'
// $12 = 'Enabled'
// $13 = 100
// $14 = 1

resource trafficManagerProfile 'Microsoft.Network/trafficManagerProfiles@2018-08-01' = {
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
// Insert snippet here

