param location string {
  default: resourceGroup().location
  metadata: {
    description: 'Specifies the Azure location where the resource should be created.'
  }
}
param wantype string {
  default: 'Standard'
  allowed: [
    'Standard'
    'Basic'
  ]
  metadata: {
    description: 'Specifies the type of Virtual WAN.'
  }
}
param wanname string {
  metadata: {
    description: 'Specifies the name to use for the Virtual WAN resources.'
  }
}

resource wan 'Microsoft.Network/virtualWans@2020-06-01' = {
  name: wanname
  location: location
  properties: {
    type: wantype
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    office365LocalBreakoutCategory: 'None'
  }
}

output id string = wan.id
