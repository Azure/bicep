// $1 = vpnVnetConnection
// $2 = 'name'
// $3 = 'virtualNetworkGateways.id'
// $4 = 'localNetworkGateways.id'
// $5 = IPsec
// $6 = 0
// $7 = 'sharedkey'

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

