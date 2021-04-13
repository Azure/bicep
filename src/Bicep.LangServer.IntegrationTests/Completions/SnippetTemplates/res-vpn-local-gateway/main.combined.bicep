resource localNetworkGateway 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: 'testVirtualNetwork'
  location: resourceGroup().location
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        'testNetworkPrefix'
      ]
    }
    gatewayIpAddress: '98.139.180.149'
  }
}
