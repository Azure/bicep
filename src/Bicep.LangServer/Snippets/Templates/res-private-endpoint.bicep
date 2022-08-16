resource /*${1:privateEndpoint}*/privateEndpoint 'Microsoft.Network/privateEndpoints@2022-01-01' = {
  name: /*${2:name}*/'name'
  properties: {
    privateLinkServiceConnections: [
      {
        name: /*${3:name}*/'name'
        properties: {
          privateLinkServiceId: /*${4:privateLinkServiceId}*/'privateLinkServiceId'
          groupIds: [
            /*${5:groupId}*/'groupId'
          ]
        }
      }
    ]
    subnet: {
      id: /*${6:subnetId}*/'subnetId'
    }
  }
}
