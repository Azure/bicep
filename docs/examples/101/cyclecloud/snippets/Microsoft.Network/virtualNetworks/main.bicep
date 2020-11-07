resource vnet 'Microsoft.Network/virtualNetworks@2020-05-01' = {
    name: 'myvnet'
    location: 'eastus'
    properties:{
      addressSpace: {
        addressPrefixes: [
          '10.0.0.0/22'
        ]
      }
      subnets:[
        {
          name: 'Default'
          properties:{
            addressPrefix: '10.0.0.0/24'
            networkSecurityGroup: {
              id: 'nsg Id'
            }
          }
        }
      ]
    }
  }