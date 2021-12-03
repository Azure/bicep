// External Load Balancer
resource /*${1:loadBalancerExternal}*/loadBalancerExternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    frontendIPConfigurations: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          publicIPAddress: {
            id: /*${4:'publicIPAddresses.id'}*/'publicIPAddresses.id'
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: /*${5:'name'}*/'name'
      }
    ]
    inboundNatRules: [
      {
        name: /*${6:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: /*${7:'frontendIPConfiguration.id'}*/'frontendIPConfiguration.id'
          }
          protocol: /*'${8|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${9:50001}*/50001
          backendPort: /*${10:3389}*/3389
          enableFloatingIP: false
        }
      }
    ]
    loadBalancingRules: [
      {
        name: /*${11:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
             id: /*${12:'frontendIPConfiguration.id'}*/'frontendIPConfiguration.id'
          }
          backendAddressPool: {
            id: /*${13:'backendAddressPool.id'}*/'backendAddressPool.id'
          }
          protocol: /*'${14|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${15:80}*/80
          backendPort: /*${16:80}*/80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: /*${17:'probe.id'}*/'probe.id'
          }
        }
      }
    ]
    probes: [
      {
        name: /*${18:'name'}*/'name'
        properties: {
          protocol: /*'${19|Tcp,Udp,All|}'*/'Tcp'
          port: /*${20:80}*/80
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
