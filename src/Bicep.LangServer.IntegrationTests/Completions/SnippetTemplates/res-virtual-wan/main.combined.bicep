resource virtualWan 'Microsoft.Network/virtualWans@2020-07-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    type: 'Standard'
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    office365LocalBreakoutCategory: 'None'
  }
}
