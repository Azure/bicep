// $1 = vpnVnetConnection
// $2 = 'name'
// $3 = 'vnetGateway'
// $4 = 'localGateway'
// $5 = IPsec
// $6 = 0
// $7 = 'sharedkey'

resource vpnVnetConnection 'Microsoft.Network/connections@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    virtualNetworkGateway1: {
      id: resourceId('Microsoft.Network/virtualNetworkGateways', 'vnetGateway')
      properties:{}
    }
    localNetworkGateway2: {
      id: resourceId('Microsoft.Network/localNetworkGateways', 'localGateway')
      properties:{}
    }
    connectionType: 'IPsec'
    routingWeight: 0
    sharedKey: 'sharedkey'
  }
}
// Insert snippet here

