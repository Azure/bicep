// VPN Local Network Gateway
resource ${1:localNetworkGateway} 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        ${3:'REQUIRED'}
      ]
    }
    gatewayIpAddress: ${4:'gatewayIpAddress'}
  }
}
