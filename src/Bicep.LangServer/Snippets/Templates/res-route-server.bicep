// Route Server
resource virtualHub 'Microsoft.Network/virtualHubs@2021-02-01' = {
  name: /*${1:'name'}*/'name'
  location: location
  properties: {
    sku: 'Standard'
  }
}

resource /*${2:ipConfiguration}*/ipConfiguration 'Microsoft.Network/virtualHubs/ipConfigurations@2021-02-01' = {
  name: /*${3:'name'}*/'name'
  parent: virtualHub
  properties: {
    subnet: {
      id: /*${4:'subnet.id'}*/'subnet.id'
    }
  }
}
