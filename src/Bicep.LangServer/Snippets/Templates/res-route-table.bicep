// Route Table
resource routeTable 'Microsoft.Network/routeTables@2019-11-01' = {
  name: '${1:routeTable}'
  location: resourceGroup().location
  properties: {
    routes: [
      {
        name: '${2:route}'
        properties: {
          addressPrefix: '${3:destinationCIDR}'
          nextHopType: '${4|VirtualNetworkGateway,VnetLocal,Internet,VirtualAppliance,None|}'
          nextHopIpAddress: '${5:nextHopIp}'
        }
      }
    ]
    disableBgpRoutePropagation: ${6|true,false|}
  }
}