// $1 = peering
// $2 = 'virtualNetwork/name'
// $3 = true
// $4 = true
// $5 = true
// $6 = true
// $7 = 'REQUIRED'

resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
  name: 'virtualNetwork/name'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: true
    allowGatewayTransit: true
    useRemoteGateways: true
    remoteVirtualNetwork: {
      id: resourceId('Microsoft.Network/virtualNetworks', 'REQUIRED')
    }
  }
}
// Insert snippet here

