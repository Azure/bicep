// $1 = virtualNetworkGateway
// $2 = 'name'
// $3 = 'name'
// $4 = 'subnet.id'
// $5 = 'publicIPAdresses.id'
// $6 = Basic
// $7 = Basic
// $8 = Vpn
// $9 = PolicyBased
// $10 = true

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

