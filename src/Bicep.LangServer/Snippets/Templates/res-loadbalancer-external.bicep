// External Load Balancer
resource /*${1:loadBalancerExternal}*/loadBalancerExternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    frontendIPConfigurations: [
      {
        name: /*${4:'name'}*/'name'
        properties: {
          publicIPAddress: {
            id: /*${5:'publicIPAddresses.id'}*/'publicIPAddresses.id'
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: /*${6:'name'}*/'name'
      }
    ]
    inboundNatRules: [
      {
        name: /*${7:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: /*${8:'frontendIPConfiguration.id'}*/'frontendIPConfiguration.id'
          }
          protocol: /*'${9|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${10:50001}*/50001
          backendPort: /*${11:3389}*/3389
          enableFloatingIP: false
        }
      }
    ]
    loadBalancingRules: [
      {
        name: /*${12:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
             id: /*${13:'frontendIPConfiguration.id'}*/'frontendIPConfiguration.id'
          }
          backendAddressPool: {
            id: /*${14:'backendAddressPool.id'}*/'backendAddressPool.id'
          }
          protocol: /*'${15|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${16:80}*/80
          backendPort: /*${17:80}*/80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: /*${18:'probe.id'}*/'probe.id'
          }
        }
      }
    ]
    probes: [
      {
        name: /*${19:'name'}*/'name'
        properties: {
          protocol: /*'${20|Tcp,Udp,All|}'*/'Tcp'
          port: /*${21:80}*/80
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
