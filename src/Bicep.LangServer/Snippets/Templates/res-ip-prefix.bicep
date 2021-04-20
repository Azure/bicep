﻿// Public IP Prefix
resource ${1:publicIPPrefix} 'Microsoft.Network/publicIPPrefixes@2019-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAddressVersion: 'IPv4'
    prefixLength: ${3:28}
  }
}
