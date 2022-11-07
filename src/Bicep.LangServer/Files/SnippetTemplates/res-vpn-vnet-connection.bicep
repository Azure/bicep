// VPN Virtual Network Connection
resource /*${1:vpnVnetConnection}*/vpnVnetConnection 'Microsoft.Network/connections@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    virtualNetworkGateway1: {
      id: /*${4:'virtualNetworkGateways.id'}*/'virtualNetworkGateways.id'
      properties:{}
    }
    localNetworkGateway2: {
      id: /*${5:'localNetworkGateways.id'}*/'localNetworkGateways.id'
      properties:{}
    }
    connectionType: /*'${6|IPsec,Vnet2Vnet,ExpressRoute,VPNClient|}'*/'IPsec'
    routingWeight: /*${7:0}*/0
    sharedKey: /*${8:'sharedkey'}*/'sharedkey'
  }
}
