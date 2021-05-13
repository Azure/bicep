resource networkInterface 'Microsoft.Network/networkInterfaces@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    ipConfigurations: [
      {
        name: 'name'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', 'virtualNetwork', 'subnet')
          }
        }
      }
    ]
  }
}

