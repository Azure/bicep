// $1 = publicIPPrefix
// $2 = 'name'
// $3 = 28

param location string

resource publicIPPrefix 'Microsoft.Network/publicIPPrefixes@2019-11-01' = {
  name: 'name'
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAddressVersion: 'IPv4'
    prefixLength: 28
  }
}
// Insert snippet here

