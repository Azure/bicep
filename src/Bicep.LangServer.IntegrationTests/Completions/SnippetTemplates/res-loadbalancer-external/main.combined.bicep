resource loadBalancerExternal 'Microsoft.Network/loadBalancers@2019-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: 'name'
        properties: {
          publicIPAddress: {
            id: resourceId('Microsoft.Network/publicIPAddresses', 'publicIP')
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: 'name'
      }
    ]
    inboundNatRules: [
      {
        name: 'name'
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', 'name', 'name')
          }
          protocol: 'Tcp'
          frontendPort: 50001
          backendPort: 3389
          enableFloatingIP: false
        }
      }
    ]
    loadBalancingRules: [
      {
        name: 'name'
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', 'name', 'name')
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', 'name', 'name')
          }
          protocol: 'Tcp'
          frontendPort: 80
          backendPort: 80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: resourceId('Microsoft.Network/loadBalancers/probes', 'name', 'name')
          }
        }
      }
    ]
    probes: [
      {
        name: 'name'
        properties: {
          protocol: 'Tcp'
          port: 80
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}

