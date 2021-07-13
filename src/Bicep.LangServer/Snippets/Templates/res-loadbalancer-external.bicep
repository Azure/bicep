// External Load Balancer
resource /*${1:loadBalancerExternal}*/loadBalancerExternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          publicIPAddress: {
            id: resourceId('Microsoft.Network/publicIPAddresses', /*${4:'publicIP'}*/'publicIP')
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
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', /*${2:'name'}*/'name', /*${3:'name'}*/'name')
          }
          protocol: /*'${7|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${8:50001}*/50001
          backendPort: /*${9:3389}*/3389
          enableFloatingIP: false
        }
      }
    ]
    loadBalancingRules: [
      {
        name: /*${10:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', /*${2:'name'}*/'name', /*${3:'name'}*/'name')
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', /*${2:'name'}*/'name', /*${5:'name'}*/'name')
          }
          protocol: /*'${11|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${12:80}*/80
          backendPort: /*${13:80}*/80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: resourceId('Microsoft.Network/loadBalancers/probes', /*${2:'name'}*/'name', /*${14:'name'}*/'name')
          }
        }
      }
    ]
    probes: [
      {
        name: /*${14:'name'}*/'name'
        properties: {
          protocol: /*'${15|Tcp,Udp,All|}'*/'Tcp'
          port: /*${16:80}*/80
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
