// Internal Load Balancer
resource ${1:loadBalancerInternal} 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: ${3:'name'}
        properties: {
          privateIPAddress: ${4:'0.0.0.0'}
          privateIPAllocationMethod: 'Static'
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', ${5:'virtualNetwork'}, ${6:'subnet'})
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: ${7:'name'}
      }
    ]
    loadBalancingRules: [
      {
        name: ${8:'name'}
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', ${2:'name'}, ${3:'name'})
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', ${2:'name'}, ${7:'name'})
          }
          protocol: '${9|Tcp,Udp,All|}'
          frontendPort: ${10:80}
          backendPort: ${11:80}
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: resourceId('Microsoft.Network/loadBalancers/probes', ${2:'name'}, ${12:'name'})
          }
        }
      }
    ]
    probes: [
      {
        name: ${12:'name'}
        properties: {
          protocol: '${13|Tcp,Udp,All|}'
          port: ${14:80}
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
