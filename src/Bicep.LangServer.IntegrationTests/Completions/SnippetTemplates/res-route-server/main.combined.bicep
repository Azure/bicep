// $1 = 'name'
// $2 = location
// $3 = ipConfiguration
// $4 = 'name'
// $5 = 'subnet.id'

param location string

resource virtualHub 'Microsoft.Network/virtualHubs@2021-02-01' = {
  name: 'name'
  location: location
  properties: {
    sku: 'Standard'
  }
}

resource ipConfiguration 'Microsoft.Network/virtualHubs/ipConfigurations@2021-02-01' = {
  name: 'name'
  parent: virtualHub
  properties: {
    subnet: {
      id: 'subnet.id'
    }
  }
}
// Insert snippet here

