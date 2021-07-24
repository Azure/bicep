// $1 = virtualNetworks
// $2 = 'name'
// $3 = peering
// $4 = 'virtualNetwork/name'
// $5 = true
// $6 = true
// $7 = true
// $8 = true

resource virtualNetworks 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
  name:  'name'
} 

resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
  name: 'virtualNetwork/name'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: true
    allowGatewayTransit: true
    useRemoteGateways: true
    remoteVirtualNetwork: {
      id: virtualNetworks.id
    }
  }
}
// Insert snippet here
