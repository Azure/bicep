// Route Table
resource ${1:'routeTable'} 'Microsoft.Network/routeTables@2019-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    routes: [
      {
        name: ${3:'name'}
        properties: {
          addressPrefix: ${4:'destinationCIDR'}
          nextHopType: '${5|VirtualNetworkGateway,VnetLocal,Internet,VirtualAppliance,None|}'
          nextHopIpAddress: ${6:'nextHopIp'}
        }
      }
    ]
    disableBgpRoutePropagation: ${7|true,false|}
  }
}
