// $1 = 'name'
// $2 = subnet
// $3 = 'name'
// $4 = networkInterface
// $5 = 'name'
// $6 = 'name'
// $7 = Dynamic

resource vnet 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
  name: 'name'
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' existing = {
  parent: vnet
  name: 'name'
}

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
            id: subnet.id
          }
        }
      }
    ]
  }
}
// Insert snippet here
