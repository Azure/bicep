// $1 = virtualWan
// $2 = 'name'
// $3 = 'Standard'
// $4 = 'None'

resource virtualWan 'Microsoft.Network/virtualWans@2020-07-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: any({
    type: 'Standard'
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    office365LocalBreakoutCategory: 'None'
  })
}
// Insert snippet here

