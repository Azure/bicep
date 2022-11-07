// Route Server
resource virtualHub 'Microsoft.Network/virtualHubs@2021-02-01' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:location}*/'location'
  properties: {
    sku: 'Standard'
  }
}

resource /*${3:ipConfiguration}*/ipConfiguration 'Microsoft.Network/virtualHubs/ipConfigurations@2021-02-01' = {
  name: /*${4:'name'}*/'name'
  parent: virtualHub
  properties: {
    subnet: {
      id: /*${5:'subnet.id'}*/'subnet.id'
    }
  }
}
