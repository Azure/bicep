param location string {
  default: resourceGroup().location
}
param pipname string {
  default: 'super-pip'
}
param ipprefixlength int {
  default: 31
  allowed: [
    28
    29
    30
    31
  ]
  metadata: {
    description: 'Specifies the size of the Public IP Prefix'
  }
}
param ipprefixname string {
  default: 'super-ipprefix'
}

resource fwipprefix 'Microsoft.Network/publicIPPrefixes@2020-06-01' = {
  name: ipprefixname
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    prefixLength: ipprefixlength
    publicIPAddressVersion: 'IPv4'
    ipTags: []
  }
}

resource fwip 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: pipname
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    publicIPPrefix: {
      id: fwipprefix.id
    }
  }
}

output id string = fwip.id