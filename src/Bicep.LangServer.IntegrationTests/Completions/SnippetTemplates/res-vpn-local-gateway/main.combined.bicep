// $1 = localNetworkGateway
// $2 = 'name'
// $3 = 'REQUIRED'
// $4 = '98.139.180.149'

resource localNetworkGateway 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        'REQUIRED'
      ]
    }
    gatewayIpAddress: '98.139.180.149'
  }
}
// Insert snippet here

