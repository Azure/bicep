// $1 = virtualWan
// $2 = 'name'
// $3 = 'Standard'
// $4 = 'None'

param location string

resource virtualWan 'Microsoft.Network/virtualWans@2020-07-01' = {
  name: 'name'
  location: location
  properties: any({
    type: 'Standard'
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    office365LocalBreakoutCategory: 'None'
  })
}
// Insert snippet here

