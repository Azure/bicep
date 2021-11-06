// $1 = 'virtualNetwork'
// $2 = peering
// $3 = 'name'
// $4 = true
// $5 = true
// $6 = true
// $7 = true
// $8 = 'virtualNetworks.id'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2021-03-01' existing = {
  name: 'virtualNetwork'
}

resource peering 'Microsoft.Network/virtualNetworks/virtualNetworkPeerings@2020-07-01' = {
  parent: virtualNetwork
  name: 'name'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: true
    allowGatewayTransit: true
    useRemoteGateways: true
    remoteVirtualNetwork: {
      id: 'virtualNetworks.id'
    }
  }
}
// Insert snippet here

