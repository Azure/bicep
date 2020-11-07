resource nic 'Microsoft.Network/networkInterfaces@2020-05-01' = {
    name: 'cycleserver-nic'
    location: 'eastus'
    properties: {
      ipConfigurations: [
        {
          name: 'ipconfig'
          properties: {
            privateIPAllocationMethod: 'Dynamic'
            subnet: {
              id: 'subnet Id'
            }
            publicIPAddress: {
              id: 'publicip Id'
            }
          }
        }
      ]
    }
  }