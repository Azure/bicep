param localVnetName string
param remoteVnetName string
param remoteVnetRg string
param location string = resourceGroup().location

resource peer 'microsoft.network/virtualNetworks/virtualNetworkPeerings@2019-11-01' = {
  name: '${localVnetName}/peering-to-remote-vnet'
  location: location
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    alloweGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
        id: resourceId(remoteVnetRg, 'Microsoft.Network/virtualNetworks', remoteVnetName)
    }
  }
}