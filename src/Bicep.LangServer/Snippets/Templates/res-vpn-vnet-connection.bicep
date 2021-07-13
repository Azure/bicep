// VPN Virtual Network Connection
resource /*${1:vpnVnetConnection}*/vpnVnetConnection 'Microsoft.Network/connections@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    virtualNetworkGateway1: {
      id: resourceId('Microsoft.Network/virtualNetworkGateways', /*${3:'vnetGateway'}*/'vnetGateway')
      properties:{}
    }
    localNetworkGateway2: {
      id: resourceId('Microsoft.Network/localNetworkGateways', /*${4:'localGateway'}*/'localGateway')
      properties:{}
    }
    connectionType: /*'${5|IPsec,Vnet2Vnet,ExpressRoute,VPNClient|}'*/'IPsec'
    routingWeight: /*${6:0}*/0
    sharedKey: /*${7:'sharedkey'}*/'sharedkey'
  }
}
