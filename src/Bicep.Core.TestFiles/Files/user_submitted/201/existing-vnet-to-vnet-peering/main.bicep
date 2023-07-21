// converted from: https://github.com/Azure/azure-quickstart-templates/tree/master/201-existing-vnet-to-vnet-peering
param localVnetName string
param remoteVnetName string
param remoteVnetRg string

resource peer 'microsoft.network/virtualNetworks/virtualNetworkPeerings@2020-05-01' = {
  name: '${localVnetName}/peering-to-remote-vnet'
  properties: {
    allowVirtualNetworkAccess: true
    allowForwardedTraffic: false
    allowGatewayTransit: false
    useRemoteGateways: false
    remoteVirtualNetwork: {
      id: resourceId(remoteVnetRg, 'Microsoft.Network/virtualNetworks', remoteVnetName)
    }
  }
}
