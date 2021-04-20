// Azure Route Table Route
resource ${1:'routeTableRoute'} 'Microsoft.Network/routeTables/routes@2019-11-01' = {
  name: ${2:'routeTableName/name'}
  properties: {
    addressPrefix: ${3:'addressPrefix'}
    nextHopType: ${4|'VirtualNetworkGateway','VnetLocal','Internet','VirtualAppliance','None'|}
    nextHopIpAddress: ${5:'nextHopIp'}
  }
}
