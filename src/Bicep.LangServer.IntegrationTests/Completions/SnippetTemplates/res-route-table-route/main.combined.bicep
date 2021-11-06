// $1 = 'routeTableName'
// $2 = routeTableRoute
// $3 = 'name'
// $4 = '0.11.26.162'
// $5 = 'None'
// $6 = '248.233.26.131'

resource routeTable 'Microsoft.Network/routeTables@2021-03-01' existing = {
  name: 'name'
}

resource routeTableRoute 'Microsoft.Network/routeTables/routes@2019-11-01' = {
  parent: routeTable
  name: 'name'
  properties: {
    addressPrefix: '0.11.26.162'
    nextHopType: 'None'
    nextHopIpAddress: '248.233.26.131'
  }
}
// Insert snippet here

