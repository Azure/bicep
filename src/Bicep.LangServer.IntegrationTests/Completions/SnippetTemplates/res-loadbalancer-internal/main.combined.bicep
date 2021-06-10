// $1 = loadBalancerInternal
// $2 = 'name'
// $3 = 'name'
// $4 = '0.0.0.0'
// $5 = 'virtualNetwork'
// $6 = 'subnet'
// $7 = 'name'
// $8 = 'name'
// $9 = Tcp
// $10 = 80
// $11 = 80
// $12 = 'name'
// $13 = Tcp
// $14 = 80

resource loadBalancerInternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: 'name'
        properties: {
          privateIPAddress: '0.0.0.0'
          privateIPAllocationMethod: 'Static'
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', 'virtualNetwork', 'subnet')
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: 'name'
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
// Insert snippet here

