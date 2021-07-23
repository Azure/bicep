// VPN Virtual Network Gateway

resource vnet 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:subnet}*/subnet 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' existing = {
  parent: vnet
  name: /*${3:'name'}*/'name'
}

resource /*${4:publicIPAdresses}*/publicIPAdresses 'Microsoft.Network/publicIPAddresses@2021-02-01' existing = {
  name: /*${5:'name'}*/'name'
}

resource /*${6:virtualNetworkGateway}*/virtualNetworkGateway 'Microsoft.Network/virtualNetworkGateways@2020-11-01' = {
  name: /*${7:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    ipConfigurations: [
      {
        name: /*${8:'name'}*/'name'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: subnet.id
          }
          publicIPAddress: {
            id: publicIPAdresses.id
          }
        }
      }
    ]
    sku: {
      name: /*'${9|Basic,HighPerformance,Standard,UltraPerformance,VpnGw1,VpnGw2,VpnGw3,VpnGw1AZ,VpnGw2AZ,VpnGw3AZ,ErGw1AZ,ErGw2AZ,ErGw3AZ|}'*/'Basic'
      tier: /*'${10|Basic,HighPerformance,Standard,UltraPerformance,VpnGw1,VpnGw2,VpnGw3,VpnGw1AZ,VpnGw2AZ,VpnGw3AZ,ErGw1AZ,ErGw2AZ,ErGw3AZ|}'*/'Basic'
    }
    gatewayType: /*'${11|Vpn,ExpressRoute|}'*/'Vpn'
    vpnType: /*'${12|PolicyBased,RouteBased|}'*/'PolicyBased'
    enableBgp: /*${13|true,false|}*/true
  }
}
