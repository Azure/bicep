// $1 = networkInterface
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = Dynamic
// $6 = 'subnet.id'

param location string

resource networkInterface 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: 'name'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'name'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: 'subnet.id'
          }
        }
      }
    ]
  }
}
// Insert snippet here

