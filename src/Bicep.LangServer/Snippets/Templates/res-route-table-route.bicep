// Azure Route Table Route
resource routeTableRoute 'Microsoft.Network/routeTables/routes@2019-11-01' = {
  name: ${1:'routeTableRoute'}
  properties: {
    addressPrefix: ${2:'addressPrefix'}
    nextHopType: '${3|VirtualNetworkGateway,VnetLocal,Internet,VirtualAppliance,None|}'
    nextHopIpAddress: ${4:'nextHopIp'}
  }
}
