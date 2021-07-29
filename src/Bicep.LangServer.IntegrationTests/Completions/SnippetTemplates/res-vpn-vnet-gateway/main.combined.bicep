// $1 = 'name'
// $2 = subnet
// $3 = 'name'
// $4 = publicIPAdresses
// $5 = 'name'
// $6 = virtualNetworkGateway
// $7 = 'name'
// $8 = 'name'
// $9 = Basic
// $10 = Basic
// $11 = Vpn
// $12 = PolicyBased
// $13 = true

resource vnet 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
  name: 'name'
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' existing = {
  parent: vnet
  name: 'name'
}

resource publicIPAdresses 'Microsoft.Network/publicIPAddresses@2021-02-01' existing = {
  name: 'name'
}

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
            id: subnet.id
          }
          publicIPAddress: {
            id: publicIPAdresses.id
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
