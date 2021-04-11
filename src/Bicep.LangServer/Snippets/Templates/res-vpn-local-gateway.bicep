// VPN Local Network Gateway
resource localNetworkGateway 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: '${1:localNetworkGateway}'
  location: resourceGroup().location
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        '${2:networkprefix}'
      ]
    }
    gatewayIpAddress: '${3:gatewayIP}'
  }
}