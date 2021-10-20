// Public IP Prefix
resource /*${1:publicIPPrefix}*/publicIPPrefix 'Microsoft.Network/publicIPPrefixes@2019-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAddressVersion: 'IPv4'
    prefixLength: /*${3:28}*/28
  }
}
