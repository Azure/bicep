// Network Interface
resource vnet 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:'subnet'}*/subnet 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' existing = {
  parent: vnet
  name: /*${3:'name'}*/'name'
}

resource /*${4:networkInterface}*/networkInterface 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: /*${5:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    ipConfigurations: [
      {
        name: /*${6:'name'}*/'name'
        properties: {
          privateIPAllocationMethod: /*'${7|Dynamic,Static|}'*/'Dynamic'
          subnet: {
            id: subnet.id
          }
        }
      }
    ]
  }
}
