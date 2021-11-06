// Azure Route Table Route
resource routeTable 'Microsoft.Network/routeTables@2021-03-01' existing = {
  name: /*${3:'routeTableName'}*/'routeTableName'
}

resource /*${2:routeTableRoute}*/routeTableRoute 'Microsoft.Network/routeTables/routes@2019-11-01' = {
  parent: routeTable
  name: /*${3:'name'}*/'name'
  properties: {
    addressPrefix: /*${4:'addressPrefix'}*/'addressPrefix'
    nextHopType: /*${5|'VirtualNetworkGateway','VnetLocal','Internet','VirtualAppliance','None'|}*/'VirtualNetworkGateway'
    nextHopIpAddress: /*${6:'nextHopIp'}*/'nextHopIp'
  }
}
