resource routeName 'Microsoft.Network/routeTables/routes@2019-11-01' = {
  name: 'testRouteTabl/route'
  properties: {
    addressPrefix: '0.11.26.162'
    nextHopType: 'None'
    nextHopIpAddress: '248.233.26.131'
  }
}
