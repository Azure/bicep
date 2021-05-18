resource vpnVnetConnection 'Microsoft.Network/connections@2019-11-01' = {
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

