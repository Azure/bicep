// $1 = routeTableRoute
// $2 = 'routeTableName/name'
// $3 = '0.11.26.162'
// $4 = 'None'
// $5 = '248.233.26.131'

resource routeTableRoute 'Microsoft.Network/routeTables/routes@2019-11-01' = {
  name: 'routeTableName/name'
  properties: {
    addressPrefix: '0.11.26.162'
    nextHopType: 'None'
    nextHopIpAddress: '248.233.26.131'
  }
}
// Insert snippet here

