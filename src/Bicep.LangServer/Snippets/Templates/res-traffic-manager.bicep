// Traffic Manager Profile
resource /*${1:trafficManagerProfile}*/trafficManagerProfile 'Microsoft.Network/trafficManagerProfiles@2018-08-01' = {
  name: /*${2:'name'}*/'name'
  location: 'global'
  properties: {
    profileStatus: 'Enabled'
    trafficRoutingMethod: /*${3|'Performance','Priority','Weighted','Geographic'|}*/'Performance'
    dnsConfig: {
      relativeName: /*${4:'dnsConfigRelativeName'}*/'dnsConfigRelativeName'
      ttl: 30
    }
    monitorConfig: {
      protocol: /*${5|'HTTP','HTTPS','TCP'|}*/'HTTP'
      port: /*${6:80}*/80
      path: /*${7:'path'}*/'path'
      intervalInSeconds: /*${8:30}*/30
      timeoutInSeconds: /*${9:5}*/5
      toleratedNumberOfFailures: /*${10:3}*/3
    }
    endpoints: [
      {
        properties: {
          targetResourceId: /*${11:'targetResourceId'}*/'targetResourceId'
          endpointStatus: /*${12|'Enabled','Disabled'|}*/'Enabled'
          weight: /*${13:100}*/100
          priority: /*${14:1}*/1
        }
      }
    ]
  }
}
