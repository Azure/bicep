// Network Interface
resource /*${1:networkInterface}*/networkInterface 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    ipConfigurations: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          privateIPAllocationMethod: /*'${4|Dynamic,Static|}'*/'Dynamic'
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', /*${5:'virtualNetwork'}*/'virtualNetwork', /*${6:'subnet'}*/'subnet')
          }
        }
      }
    ]
  }
}
