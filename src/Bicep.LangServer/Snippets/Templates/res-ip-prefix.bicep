resource publicIPPrefix1 'Microsoft.Network/publicIPPrefixes@2019-11-01' = {
  name: '${1:publicIPPrefix1}'
  location: resourceGroup().location
  tags: {}
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAddressVersion: 'IPv4'
    prefixLength: '${2:28}'
  }
}