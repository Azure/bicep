// VPN Virtual Network Gateway
resource /*${1:virtualNetworkGateway}*/virtualNetworkGateway 'Microsoft.Network/virtualNetworkGateways@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: /*${4:'subnet.id'}*/'subnet.id'
          }
          publicIPAddress: {
            id: /*${5:'publicIPAdresses.id'}*/'publicIPAdresses.id'
          }
        }
      }
    ]
    sku: {
      name: /*'${6|Basic,HighPerformance,Standard,UltraPerformance,VpnGw1,VpnGw2,VpnGw3,VpnGw1AZ,VpnGw2AZ,VpnGw3AZ,ErGw1AZ,ErGw2AZ,ErGw3AZ|}'*/'Basic'
      tier: /*'${7|Basic,HighPerformance,Standard,UltraPerformance,VpnGw1,VpnGw2,VpnGw3,VpnGw1AZ,VpnGw2AZ,VpnGw3AZ,ErGw1AZ,ErGw2AZ,ErGw3AZ|}'*/'Basic'
    }
    gatewayType: /*'${8|Vpn,ExpressRoute|}'*/'Vpn'
    vpnType: /*'${9|PolicyBased,RouteBased|}'*/'PolicyBased'
    enableBgp: /*${10|true,false|}*/true
  }
}
