// $1 = virtualNetworkGateways
// $2 = 'name'
// $3 = localNetworkGateways
// $4 = 'name'
// $5 = vpnVnetConnection
// $6 = 'name'
// $7 = IPsec
// $8 = 0
// $9 = 'sharedkey'

resource virtualNetworkGateways 'Microsoft.Network/virtualNetworkGateways@2021-02-01' existing = {
  name: 'name'
}

resource localNetworkGateways 'Microsoft.Network/localNetworkGateways@2021-02-01' existing = {
  name: 'name'
}

resource vpnVnetConnection 'Microsoft.Network/connections@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    virtualNetworkGateway1: {
      id: virtualNetworkGateways.id
      properties:{}
    }
    localNetworkGateway2: {
      id: localNetworkGateways.id
      properties:{}
    }
    connectionType: 'IPsec'
    routingWeight: 0
    sharedKey: 'sharedkey'
  }
}
// Insert snippet here
