// Route Table
resource /*${1:routeTable}*/routeTable 'Microsoft.Network/routeTables@2019-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    routes: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          addressPrefix: /*${4:'destinationCIDR'}*/'destinationCIDR'
          nextHopType: /*${5|'VirtualNetworkGateway','VnetLocal','Internet','VirtualAppliance','None'|}*/'VirtualNetworkGateway'
          nextHopIpAddress: /*${6:'nextHopIp'}*/'nextHopIp'
        }
      }
    ]
    disableBgpRoutePropagation: /*${7|true,false|}*/true
  }
}
