// Traffic Manager Profile
resource ${1:'trafficManagerProfile'} 'Microsoft.Network/trafficManagerProfiles@2018-04-01' = {
  name: ${2:'name'}
  location: 'global'
  properties: {
    profileStatus: 'Enabled'
    trafficRoutingMethod: '${3|Performance,Priority,Weighted,Geographic|}'
    dnsConfig: {
      relativeName: ${4:'dnsConfigRelativeName'}
      ttl: 30
    }
    monitorConfig: {
      protocol: '${5|HTTP,HTTPS,TCP|}'
      port: ${6:80}
      path: ${7:'path'}
      intervalInSeconds: ${8:30}
      timeoutInSeconds: ${9:5}
      toleratedNumberOfFailures: ${10:3}
    }
    endpoints: [
      {
        properties: {
          targetResourceId: ${11:'targetResourceId'}
          endpointStatus: '${12|Enabled,Disabled|}'
          weight: ${13:100}
          priority: ${14:1}
        }
      }
    ]
  }
}
