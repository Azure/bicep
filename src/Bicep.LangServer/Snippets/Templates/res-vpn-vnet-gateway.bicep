// VPN Virtual Network Gateway
resource ${1:virtualNetworkGateway} 'Microsoft.Network/virtualNetworkGateways@2020-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    ipConfigurations: [
      {
        name: ${3:'name'}
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', ${4:'virtualNetwork'}, ${5:'subnet'})
          }
          publicIPAddress: {
            id: resourceId('Microsoft.Network/publicIPAddresses', ${6:'publicIPAddress'})
          }
        }
      }
    ]
    sku: {
      name: '${7|Basic,HighPerformance,Standard,UltraPerformance,VpnGw1,VpnGw2,VpnGw3,VpnGw1AZ,VpnGw2AZ,VpnGw3AZ,ErGw1AZ,ErGw2AZ,ErGw3AZ|}'
      tier: '${8|Basic,HighPerformance,Standard,UltraPerformance,VpnGw1,VpnGw2,VpnGw3,VpnGw1AZ,VpnGw2AZ,VpnGw3AZ,ErGw1AZ,ErGw2AZ,ErGw3AZ|}'
    }
    gatewayType: '${9|Vpn,ExpressRoute|}'
    vpnType: '${10|PolicyBased,RouteBased|}'
    enableBgp: ${11|true,false|}
  }
}
