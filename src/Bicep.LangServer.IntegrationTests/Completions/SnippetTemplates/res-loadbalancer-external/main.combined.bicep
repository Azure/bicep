// $1 = publicIPAddresses
// $2 = 'name'
// $3 = backendAddressPool
// $4 = 'loadBalancerExternal/loadBalancerBackEndPool'
// $5 = loadBalancerExternal
// $6 = 'name'
// $7 = 'name'
// $8 = 'name'
// $9 = 'name'
// $10 = 'id'
// $11 = Tcp
// $12 = 50001
// $13 = 3389
// $14 = 'name'
// $15 = 'id'
// $16 = Tcp
// $17 = 80
// $18 = 80
// $19 = 'id'
// $20 = 'name'
// $21 = Tcp
// $22 = 80

resource publicIPAddresses 'Microsoft.Network/publicIPAddresses@2021-02-01' existing = {
  name: 'name'
}

resource backendAddressPool 'Microsoft.Network/loadBalancers/backendAddressPools@2021-02-01' existing = {
  name: 'loadBalancerExternal/loadBalancerBackEndPool'
}

resource loadBalancerExternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: 'name'
        properties: {
          publicIPAddress: {
            id: publicIPAddresses.id
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
            id: 'id'
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
