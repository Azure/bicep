// $1 = routeTable
// $2 = 'name'
// $3 = 'name'
// $4 = 'destinationCIDR'
// $5 = 'VirtualNetworkGateway'
// $6 = '0.11.26.162'
// $7 = true

param location string

resource routeTable 'Microsoft.Network/routeTables@2019-11-01' = {
  name: 'name'
  location: location
  properties: {
    routes: [
      {
        name: 'name'
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
// Insert snippet here

