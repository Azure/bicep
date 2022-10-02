param location string

var vmName = [
  'adPDC'
  'adBDC'
]
var nicName = [
  'adPDCNic'
  'adBDCNic'
]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
  name: nicName[i]
  location: location
}]

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = [for i in range(0, 2): {
  name: vmName[i]
  location: location
  zones: [
    (i + 1)
//@[4:11) [BCP034 (Warning)] The enclosing array expected an item of type "string", but the provided item was of type "int". (CodeDescription: none) |(i + 1)|
  ]
  dependsOn: [
    nic
  ]
}]

resource vmName_0_CreateAdForest 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  name: '${vmName[0]}/CreateAdForest'
  location: location
}

resource vmName_1_PepareBDC 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  name: '${vmName[1]}/PepareBDC'
  location: location
}
