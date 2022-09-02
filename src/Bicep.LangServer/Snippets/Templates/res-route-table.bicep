// Route Table
resource /*${1:routeTable}*/routeTable 'Microsoft.Network/routeTables@2019-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    routes: [
      {
        name: /*${4:'name'}*/'name'
        properties: {
          addressPrefix: /*${5:'destinationCIDR'}*/'destinationCIDR'
          nextHopType: /*${6|'VirtualNetworkGateway','VnetLocal','Internet','VirtualAppliance','None'|}*/'VirtualNetworkGateway'
          nextHopIpAddress: /*${7:'nextHopIp'}*/'nextHopIp'
        }
      }
    ]
    disableBgpRoutePropagation: /*${8|true,false|}*/true
  }
}
