// Virtual Network Peering
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2021-03-01' existing = {
  name: /*${1:'virtualNetwork'}*/'name'
}

resource /*${2:peering}*/peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
  parent: virtualNetwork
  name: /*${3:'name'}*/'name'
  properties: {
    allowVirtualNetworkAccess: /*${4|true,false|}*/true
    allowForwardedTraffic: /*${5|true,false|}*/true
    allowGatewayTransit: /*${6|true,false|}*/true
    useRemoteGateways: /*${7|true,false|}*/true
    remoteVirtualNetwork: {
      id: /*${8:'virtualNetworks.id'}*/'virtualNetworks.id'
    }
  }
}
