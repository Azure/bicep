// Azure Route Table Route
resource ${1:'routeTableRoute'} 'Microsoft.Network/routeTables/routes@2019-11-01' = {
  name: 'routeTableName/name'
  properties: {
    addressPrefix: ${2:'addressPrefix'}
    nextHopType: '${3|VirtualNetworkGateway,VnetLocal,Internet,VirtualAppliance,None|}'
    nextHopIpAddress: ${4:'nextHopIp'}
  }
}
