// $1 = privateEndpoint
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = 'privateLinkServiceId'
// $6 = 'groupId'
// $7 = 'subnetId'

param location string

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2022-01-01' = {
  name: 'name'
  location: location
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

