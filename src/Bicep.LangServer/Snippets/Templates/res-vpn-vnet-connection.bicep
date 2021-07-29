// VPN Virtual Network Connection
resource /*${1:virtualNetworkGateways}*/virtualNetworkGateways 'Microsoft.Network/virtualNetworkGateways@2021-02-01' existing = {
  name: /*${2:'name'}*/'name'
}

resource /*${3:localNetworkGateways}*/localNetworkGateways 'Microsoft.Network/localNetworkGateways@2021-02-01' existing = {
  name: /*${4:'name'}*/'name'
}

resource /*${5:vpnVnetConnection}*/vpnVnetConnection 'Microsoft.Network/connections@2020-11-01' = {
  name: /*${6:'name'}*/'name'
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
    connectionType: /*'${7|IPsec,Vnet2Vnet,ExpressRoute,VPNClient|}'*/'IPsec'
    routingWeight: /*${8:0}*/0
    sharedKey: /*${9:'sharedkey'}*/'sharedkey'
  }
}
