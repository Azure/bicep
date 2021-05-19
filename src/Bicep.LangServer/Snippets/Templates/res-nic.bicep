// Network Interface
resource ${1:networkInterface} 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    ipConfigurations: [
      {
        name: ${3:'name'}
        properties: {
          privateIPAllocationMethod: '${4|Dynamic,Static|}'
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', ${5:'virtualNetwork'}, ${6:'subnet'})
          }
        }
      }
    ]
  }
}
