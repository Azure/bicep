// VPN Local Network Gateway
resource ${1:'localNetworkGateway'} 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        ${2:'UPDATEME'}
      ]
    }
    gatewayIpAddress: ${3:'gatewayIpAddress'}
  }
}
