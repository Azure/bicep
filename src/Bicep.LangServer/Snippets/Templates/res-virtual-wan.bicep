// Virtual WAN
resource vwan 'Microsoft.Network/virtualWans@2020-07-01' = {
  name: ${1:'virtualWanName'}
  location: resourceGroup().location
  properties: {
    type: '${2|Standard,Basic|}'
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    office365LocalBreakoutCategory: '${3|Optimize,OptimizeAndAllow,All,None|}'
  }
}
