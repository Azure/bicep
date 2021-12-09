// Virtual WAN
resource /*${1:virtualWan}*/virtualWan 'Microsoft.Network/virtualWans@2020-07-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: any({
    type: /*${4|'Standard','Basic'|}*/'Standard'
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    office365LocalBreakoutCategory: /*${5|'Optimize','OptimizeAndAllow','All','None'|}*/'Optimize'
  })
}
