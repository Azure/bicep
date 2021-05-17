// External Load Balancer
resource ${1:loadBalancerExternal} 'Microsoft.Network/loadBalancers@2019-11-01' = {
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
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', ${2:'name'}, ${11:'loadBalancerFrontEnd'})
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', ${2:'name'}, ${5:'name'})
          }
          protocol: '${12|Tcp,Udp,All|}'
          frontendPort: ${13:80}
          backendPort: ${14:80}
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: resourceId('Microsoft.Network/loadBalancers/probes', ${2:'name'}, ${15:'name'})
          }
        }
      }
    ]
    probes: [
      {
        name: ${15:'name'}
        properties: {
          protocol: '${16|Tcp,Udp,All|}'
          port: ${17:80}
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
