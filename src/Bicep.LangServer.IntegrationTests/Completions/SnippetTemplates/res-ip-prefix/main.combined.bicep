resource publicIPPrefix 'Microsoft.Network/publicIPPrefixes@2019-11-01' = {
  name: 'publicIPPrefix'
  location: resourceGroup().location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAddressVersion: 'IPv4'
    prefixLength: 28
  }
}
