// Public IP Prefix
resource publicIPPrefix 'Microsoft.Network/publicIPPrefixes@2019-11-01' = {
  name: '${1:publicIPPrefix}'
  location: resourceGroup().location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAddressVersion: 'IPv4'
    prefixLength: '${2:28}'
  }
}