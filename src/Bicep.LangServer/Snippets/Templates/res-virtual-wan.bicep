// Virtual WAN
resource ${1:virtualWan} 'Microsoft.Network/virtualWans@2020-07-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    type: ${3|'Standard','Basic'|}
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    office365LocalBreakoutCategory: ${4|'Optimize','OptimizeAndAllow','All','None'|}
  }
}
