// Virtual Network Peering
resource ${1:peering} 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
  name: ${2:'virtualNetwork/name'}
  properties: {
    allowVirtualNetworkAccess: ${3|true,false|}
    allowForwardedTraffic: ${4|true,false|}
    allowGatewayTransit: ${5|true,false|}
    useRemoteGateways: ${6|true,false|}
    remoteVirtualNetwork: {
      id: resourceId('Microsoft.Network/virtualNetworks', ${7:'REQUIRED'})
    }
  }
}
