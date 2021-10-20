// VPN Local Network Gateway
resource /*${1:localNetworkGateway}*/localNetworkGateway 'Microsoft.Network/localNetworkGateways@2019-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    localNetworkAddressSpace: {
      addressPrefixes: [
        /*${3:'REQUIRED'}*/'REQUIRED'
      ]
    }
    gatewayIpAddress: /*${4:'gatewayIpAddress'}*/'gatewayIpAddress'
  }
}
