param prefix string
param azFwlIp string

resource route 'Microsoft.Network/routeTables@2020-06-01' = {
  name: '${prefix}-rot'
  location: resourceGroup().location
  properties: {
    disableBgpRoutePropagation: false
    routes: [
      {
        name: 'DefaultRoute'
        properties: {
          addressPrefix: '0.0.0.0/0'
          nextHopType: 'VirtualAppliance'
          nextHopIpAddress: azFwlIp
        }
      }
    ]
  }
}

output id string = route.id
