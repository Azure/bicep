resource routeTable 'Microsoft.Network/routeTables@2019-11-01' = {
  name: 'testRouteTable'
  location: resourceGroup().location
  properties: {
    routes: [
      {
        name: 'testRoute'
        properties: {
          addressPrefix: 'testDestinationCIDR'
          nextHopType: 'VirtualNetworkGateway'
          nextHopIpAddress: '0.11.26.162'
        }
      }
    ]
    disableBgpRoutePropagation: true
  }
}
