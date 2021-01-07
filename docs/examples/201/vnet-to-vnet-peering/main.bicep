param location string = resourceGroup().location
param vNet1Name string = 'VNet1'
param vNet2Name string = 'VNet2'

resource vNet1 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vNet1Name
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/24'
      ]
    }
    subnets: [
      {
        name: 'subnet1'
        properties: {
          addressPrefix: '10.0.0.0/24'
        }
      }
    ]
  }
}

resource VnetPeering1 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-06-01' = {
  name: '${vNet1.name}/${vNet1.name}-${vNet2.name}'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
      id: vNet2.id
    }
  }
}

resource vNet2 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vNet2Name
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '192.168.0.0/24'
      ]
    }
    subnets: [
      {
        name: 'subnet1'
        properties: {
          addressPrefix: '192.168.0.0/24'
        }
      }
    ]
  }
}

resource VnetPeering2 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-06-01' = {
  name: '${vNet2.name}/${vNet2.name}-${vNet1.name}'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
      id: vNet1.id
    }
  }
}
