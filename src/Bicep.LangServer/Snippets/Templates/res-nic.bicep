// Network Interface
resource /*${1:networkInterface}*/networkInterface 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    ipConfigurations: [
      {
        name: /*${4:'name'}*/'name'
        properties: {
          privateIPAllocationMethod: /*'${5|Dynamic,Static|}'*/'Dynamic'
          subnet: {
            id: /*${6:'subnet.id'}*/'subnet.id'
          }
        }
      }
    ]
  }
}
