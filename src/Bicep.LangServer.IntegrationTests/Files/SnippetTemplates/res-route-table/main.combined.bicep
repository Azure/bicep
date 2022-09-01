// $1 = routeTable
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = 'destinationCIDR'
// $6 = 'VirtualNetworkGateway'
// $7 = '0.11.26.162'
// $8 = true

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

