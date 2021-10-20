// VPN Virtual Network Connection
resource /*${1:vpnVnetConnection}*/vpnVnetConnection 'Microsoft.Network/connections@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    virtualNetworkGateway1: {
      id: /*${3:'virtualNetworkGateways.id'}*/'virtualNetworkGateways.id'
      properties:{}
    }
    localNetworkGateway2: {
      id: /*${4:'localNetworkGateways.id'}*/'localNetworkGateways.id'
      properties:{}
    }
    connectionType: /*'${5|IPsec,Vnet2Vnet,ExpressRoute,VPNClient|}'*/'IPsec'
    routingWeight: /*${6:0}*/0
    sharedKey: /*${7:'sharedkey'}*/'sharedkey'
  }
}
