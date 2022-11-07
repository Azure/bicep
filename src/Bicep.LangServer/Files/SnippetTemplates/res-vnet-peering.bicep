// Virtual Network Peering
resource /*${1:peering}*/peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
  name: /*${2:'virtualNetwork/name'}*/'virtualNetwork/name'
  properties: {
    allowVirtualNetworkAccess: /*${3|true,false|}*/true
    allowForwardedTraffic: /*${4|true,false|}*/true
    allowGatewayTransit: /*${5|true,false|}*/true
    useRemoteGateways: /*${6|true,false|}*/true
    remoteVirtualNetwork: {
      id: /*${7:'virtualNetworks.id'}*/'virtualNetworks.id'
    }
  }
}
