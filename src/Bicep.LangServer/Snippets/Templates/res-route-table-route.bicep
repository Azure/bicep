// Azure Route Table Route
resource /*${1:routeTableRoute}*/routeTableRoute 'Microsoft.Network/routeTables/routes@2019-11-01' = {
  name: /*${2:'routeTableName/name'}*/'routeTableName/name'
  properties: {
    addressPrefix: /*${3:'addressPrefix'}*/'addressPrefix'
    nextHopType: /*${4|'VirtualNetworkGateway','VnetLocal','Internet','VirtualAppliance','None'|}*/'VirtualNetworkGateway'
    nextHopIpAddress: /*${5:'nextHopIp'}*/'nextHopIp'
  }
}
