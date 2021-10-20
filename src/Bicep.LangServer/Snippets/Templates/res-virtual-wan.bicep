// Virtual WAN
resource /*${1:virtualWan}*/virtualWan 'Microsoft.Network/virtualWans@2020-07-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: any({
    type: /*${3|'Standard','Basic'|}*/'Standard'
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    office365LocalBreakoutCategory: /*${4|'Optimize','OptimizeAndAllow','All','None'|}*/'Optimize'
  })
}
