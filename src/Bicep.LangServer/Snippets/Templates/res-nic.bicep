// Network Interface
resource /*${1:networkInterface}*/networkInterface 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          privateIPAllocationMethod: /*'${4|Dynamic,Static|}'*/'Dynamic'
          subnet: {
            id: /*${5:'subnet.id'}*/'subnet.id'
          }
        }
      }
    ]
  }
}
