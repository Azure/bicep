// $1 = 'name'
// $2 = ipConfiguration
// $3 = 'name'
// $4 = 'subnet.id'

resource virtualHub 'Microsoft.Network/virtualHubs@2021-02-01' = {
  name: 'name'
  location: resourceGroup().location
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

