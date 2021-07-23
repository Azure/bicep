// Virtual Network Peering
resource /*${1:virtualNetworks}*/virtualNetworks 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
  name:  /*${2:'name'}*/'name'
} 

resource /*${3:peering}*/peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
  name: /*${4:'virtualNetwork/name'}*/'virtualNetwork/name'
  properties: {
    allowVirtualNetworkAccess: /*${5|true,false|}*/true
    allowForwardedTraffic: /*${6|true,false|}*/true
    allowGatewayTransit: /*${7|true,false|}*/true
    useRemoteGateways: /*${8|true,false|}*/true
    remoteVirtualNetwork: {
      id: virtualNetworks.id
    }
  }
}
