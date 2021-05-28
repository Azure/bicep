param wvdvnetID string
param peeringnamefromhubvnet string

resource hubpeer 'microsoft.network/virtualNetworks/virtualNetworkPeerings@2020-05-01' = {
  name: peeringnamefromhubvnet
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
      id: wvdvnetID
    }
  }
}
