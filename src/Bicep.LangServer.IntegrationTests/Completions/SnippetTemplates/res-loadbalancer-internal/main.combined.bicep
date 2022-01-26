// $1 = loadBalancerInternal
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = '0.0.0.0'
// $6 = 'subnet.id'
// $7 = 'name'
// $8 = 'name'
// $9 = 'frontendIPConfiguration.id'
// $10 = 'backendAddressPool.id'
// $11 = Tcp
// $12 = 80
// $13 = 80
// $14 = 'probe.id'
// $15 = 'name'
// $16 = Tcp
// $17 = 80

param location string

resource loadBalancerInternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: 'name'
  location: location
  properties: {
    frontendIPConfigurations: [
      {
        name: 'name'
        properties: {
          privateIPAddress: '0.0.0.0'
          privateIPAllocationMethod: 'Static'
          subnet: {
            id: 'subnet.id'
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

