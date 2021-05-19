// External Load Balancer
resource ${1:loadBalancerExternal} 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: ${3:'name'}
        properties: {
          publicIPAddress: {
            id: resourceId('Microsoft.Network/publicIPAddresses', ${4:'publicIP'})
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: ${5:'name'}
      }
    ]
    inboundNatRules: [
      {
        name: ${6:'name'}
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', ${2:'name'}, ${3:'name'})
          }
          protocol: '${7|Tcp,Udp,All|}'
          frontendPort: ${8:50001}
          backendPort: ${9:3389}
          enableFloatingIP: false
        }
      }
    ]
    loadBalancingRules: [
      {
        name: ${10:'name'}
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', ${2:'name'}, ${3:'name'})
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', ${2:'name'}, ${5:'name'})
          }
          protocol: '${11|Tcp,Udp,All|}'
          frontendPort: ${12:80}
          backendPort: ${13:80}
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: resourceId('Microsoft.Network/loadBalancers/probes', ${2:'name'}, ${14:'name'})
          }
        }
      }
    ]
    probes: [
      {
        name: ${14:'name'}
        properties: {
          protocol: '${15|Tcp,Udp,All|}'
          port: ${16:80}
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
