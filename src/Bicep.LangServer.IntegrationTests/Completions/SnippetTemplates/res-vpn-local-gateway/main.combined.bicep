resource localNetworkGateway 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: 'localNetworkGateway'
  location: resourceGroup().location
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        'addressPrefixes'
      ]
    }
    gatewayIpAddress: '98.139.180.149'
  }
}

