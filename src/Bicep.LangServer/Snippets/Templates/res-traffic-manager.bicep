// Traffic Manager Profile
resource trafficManagerProfile 'Microsoft.Network/trafficManagerProfiles@2018-04-01' = {
  name: ${1:'trafficManagerProfile'}
  location: 'global'
  properties: {
    profileStatus: 'Enabled'
    trafficRoutingMethod: '${2|Performance,Priority,Weighted,Geographic|}'
    dnsConfig: {
      relativeName: ${3:'dnsConfigRelativeName'}
      ttl: 30
    }
    monitorConfig: {
      protocol: '${4|HTTP,HTTPS,TCP|}'
      port: ${5:80}
      path: ${6:'path'}
      intervalInSeconds: ${7:30}
      timeoutInSeconds: ${8:5}
      toleratedNumberOfFailures: ${9:3}
    }
    endpoints: [
      {
        properties: {
          targetResourceId: ${10:'targetResourceId'}
          endpointStatus: '${11|Enabled,Disabled|}'
          weight: ${12:100}
          priority: ${13:1}
        }
      }
    ]
  }
}
