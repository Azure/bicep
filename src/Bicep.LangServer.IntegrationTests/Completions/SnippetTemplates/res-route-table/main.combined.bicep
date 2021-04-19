resource routeTable 'Microsoft.Network/routeTables@2019-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    routes: [
      {
        name: 'route'
        properties: {
          addressPrefix: 'destinationCIDR'
          nextHopType: 'VirtualNetworkGateway'
          nextHopIpAddress: '0.11.26.162'
        }
      }
    ]
    disableBgpRoutePropagation: true
  }
}
