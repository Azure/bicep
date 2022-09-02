// VPN Local Network Gateway
resource /*${1:localNetworkGateway}*/localNetworkGateway 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        /*${4:'REQUIRED'}*/'REQUIRED'
      ]
    }
    gatewayIpAddress: /*${5:'gatewayIpAddress'}*/'gatewayIpAddress'
  }
}
