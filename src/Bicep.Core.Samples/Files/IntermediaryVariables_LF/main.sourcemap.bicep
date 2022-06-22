var boolVal = true
//@[25:25]     "boolVal": true,

var vmProperties = {
//@[26:35]     "vmProperties": {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: boolVal
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[38:44]       "type": "Microsoft.Compute/virtualMachines",
  name: 'vm'
  location: 'West US'
  properties: vmProperties
}

var ipConfigurations = [for i in range(0, 2): {
//@[12:23]         "name": "ipConfigurations",
  id: true
  name: 'asdf${i}'
  properties: {
    madeUpProperty: boolVal
    subnet: 'hello'
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[45:52]       "type": "Microsoft.Network/networkInterfaces",
  name: 'abc'
  properties: {
    ipConfigurations: ipConfigurations
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[53:66]       "copy": {
  name: 'abc${i}'
  properties: {
    ipConfigurations: [
      // TODO: fix this
      ipConfigurations[i]
//@[63:63]           "[variables('ipConfigurations')[range(0, 2)[copyIndex()]]]"
    ]
  }
}]

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[67:80]       "copy": {
  name: 'abc${ipConfig.name}'
  properties: {
    ipConfigurations: [
      // TODO: fix this
      ipConfig
//@[77:77]           "[variables('ipConfigurations')[copyIndex()]]"
    ]
  }
}]

