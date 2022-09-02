// $1 = virtualNetworkGateway
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = 'subnet.id'
// $6 = 'publicIPAdresses.id'
// $7 = Basic
// $8 = Basic
// $9 = Vpn
// $10 = PolicyBased
// $11 = true

param location string

resource virtualNetworkGateway 'Microsoft.Network/virtualNetworkGateways@2020-11-01' = {
  name: 'name'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'name'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: 'subnet.id'
          }
          publicIPAddress: {
            id: 'publicIPAdresses.id'
          }
        }
      }
    ]
    sku: {
      name: 'Basic'
      tier: 'Basic'
    }
    gatewayType: 'Vpn'
    vpnType: 'PolicyBased'
    enableBgp: true
  }
}
// Insert snippet here

