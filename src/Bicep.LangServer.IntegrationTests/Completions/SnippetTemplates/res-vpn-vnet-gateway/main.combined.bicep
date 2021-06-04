// $1 = virtualNetworkGateway
// $2 = 'name'
// $3 = 'name'
// $4 = 'virtualNetwork'
// $5 = 'subnet'
// $6 = 'publicIPAddress'
// $7 = Basic
// $8 = Basic
// $9 = Vpn
// $10 = PolicyBased
// $11 = true

resource virtualNetworkGateway 'Microsoft.Network/virtualNetworkGateways@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    ipConfigurations: [
      {
        name: 'name'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', 'virtualNetwork', 'subnet')
          }
          publicIPAddress: {
            id: resourceId('Microsoft.Network/publicIPAddresses', 'publicIPAddress')
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

