// $1 = 'name'
// $2 = subnet
// $3 = 'name'
// $4 = backendAddressPool
// $5 = 'loadBalancerExternal/loadBalancerBackEndPool'
// $6 = loadBalancerInternal
// $7 = 'name'
// $8 = 'name'
// $9 = '0.0.0.0'
// $10 = 'name'
// $11 = 'name'
// $12 = Tcp
// $13 = 80
// $14 = 80
// $15 = 'name'
// $16 = Tcp
// $17 = 80

resource vnet 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
  name: 'name'
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' existing = {
  parent: vnet
  name: 'name'
}

resource backendAddressPool 'Microsoft.Network/loadBalancers/backendAddressPools@2021-02-01' existing = {
  name: 'loadBalancerExternal/loadBalancerBackEndPool'
}

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
            id: subnet.id
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
            id: 'id'
          }
          backendAddressPool: {
            id: backendAddressPool.id
          }
          protocol: 'Tcp'
          frontendPort: 80
          backendPort: 80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: 'id'
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
