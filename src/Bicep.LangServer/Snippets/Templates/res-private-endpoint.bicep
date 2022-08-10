
resource /*${1:privateEndpoint}*/privateEndpoint 'Microsoft.Network/privateEndpoints@2022-01-01' = {
  name: /*${2:name}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    privateLinkServiceConnections: [
      {
        name: /*${4:privateLinkName}*/'privateLinkName'
        properties: {
          privateLinkServiceId: /*${5:serviceId}*/'serviceId'
          groupIds: [
            /*${6:groupId}*/'groupId'
          ]
        }
      }
    ]
    subnet: {
      id: /*${7:subnetId}*/'subnetId'
    }
  }
}
