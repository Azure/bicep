param prefix string
param addressSpaces array
param subnets array

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: '${prefix}-rg'
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: addressSpaces
    }
    subnets: subnets
  }
}

output name string = vnet.name
output id string = vnet.id
