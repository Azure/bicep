// $1 = vpnVnetConnection
// $2 = 'name'
// $3 = location
// $4 = 'virtualNetworkGateways.id'
// $5 = 'localNetworkGateways.id'
// $6 = IPsec
// $7 = 0
// $8 = 'sharedkey'

param location string

resource vpnVnetConnection 'Microsoft.Network/connections@2020-11-01' = {
  name: 'name'
  location: location
  properties: {
    virtualNetworkGateway1: {
      id: 'virtualNetworkGateways.id'
      properties:{}
    }
    localNetworkGateway2: {
      id: 'localNetworkGateways.id'
      properties:{}
    }
    connectionType: 'IPsec'
    routingWeight: 0
    sharedKey: 'sharedkey'
  }
}
// Insert snippet here

