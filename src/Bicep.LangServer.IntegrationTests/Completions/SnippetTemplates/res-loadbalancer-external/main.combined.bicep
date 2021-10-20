// $1 = loadBalancerExternal
// $2 = 'name'
// $3 = 'name'
// $4 = 'publicIPAddresses.id'
// $5 = 'name'
// $6 = 'name'
// $7 = 'frontendIPConfiguration.id'
// $8 = Tcp
// $9 = 50001
// $10 = 3389
// $11 = 'name'
// $12 = 'frontendIPConfiguration.id'
// $13 = 'backendAddressPool.id'
// $14 = Tcp
// $15 = 80
// $16 = 80
// $17 = 'probe.id'
// $18 = 'name'
// $19 = Tcp
// $20 = 80

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

