// $1 = loadBalancerExternal
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = 'publicIPAddresses.id'
// $6 = 'name'
// $7 = 'name'
// $8 = 'frontendIPConfiguration.id'
// $9 = Tcp
// $10 = 50001
// $11 = 3389
// $12 = 'name'
// $13 = 'frontendIPConfiguration.id'
// $14 = 'backendAddressPool.id'
// $15 = Tcp
// $16 = 80
// $17 = 80
// $18 = 'probe.id'
// $19 = 'name'
// $20 = Tcp
// $21 = 80

param location string

resource loadBalancerExternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: 'name'
  location: location
  properties: {
    frontendIPConfigurations: [
      {
        name: 'name'
        properties: {
          publicIPAddress: {
            id: 'publicIPAddresses.id'
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
            id: 'frontendIPConfiguration.id'
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
             id: 'frontendIPConfiguration.id'
          }
          backendAddressPool: {
            id: 'backendAddressPool.id'
          }
          protocol: 'Tcp'
          frontendPort: 80
          backendPort: 80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: 'probe.id'
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

