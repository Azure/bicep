resource localNetworkGateway 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        'UPDATEME'
      ]
    }
    gatewayIpAddress: '98.139.180.149'
  }
}

