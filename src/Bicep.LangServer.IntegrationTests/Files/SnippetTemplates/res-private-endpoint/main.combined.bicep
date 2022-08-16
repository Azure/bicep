// $1 = privateEndpoint
// $2 = 'name'
// $3 = 'name'
// $4 = 'privateLinkServiceId'
// $5 = 'groupId'
// $6 = 'subnetId'

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2022-01-01' = {
  name: 'name'
  properties: {
    privateLinkServiceConnections: [
      {
        name: 'name'
        properties: {
          privateLinkServiceId: 'privateLinkServiceId'
          groupIds: [
            'groupId'
          ]
        }
      }
    ]
    subnet: {
      id: 'subnetId'
    }
  }
}
// Insert snippet here

