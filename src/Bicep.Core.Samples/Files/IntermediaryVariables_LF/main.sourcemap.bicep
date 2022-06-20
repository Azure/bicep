var boolVal = true
//@[25:25]     "boolVal": true,

var vmProperties = {
//@[26:35]     "vmProperties": {
  diagnosticsProfile: {
//@[27:33]       "diagnosticsProfile": {
    bootDiagnostics: {
//@[28:32]         "bootDiagnostics": {
      enabled: 123
//@[29:29]           "enabled": 123,
      storageUri: true
//@[30:30]           "storageUri": true,
      unknownProp: 'asdf'
//@[31:31]           "unknownProp": "asdf"
    }
  }
  evictionPolicy: boolVal
//@[34:34]       "evictionPolicy": "[variables('boolVal')]"
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[38:44]       "type": "Microsoft.Compute/virtualMachines",
  name: 'vm'
  location: 'West US'
//@[42:42]       "location": "West US",
  properties: vmProperties
//@[43:43]       "properties": "[variables('vmProperties')]"
}

var ipConfigurations = [for i in range(0, 2): {
//@[12:23]         "name": "ipConfigurations",
  id: true
//@[16:16]           "id": true,
  name: 'asdf${i}'
//@[17:17]           "name": "[format('asdf{0}', range(0, 2)[copyIndex('ipConfigurations')])]",
  properties: {
//@[18:21]           "properties": {
    madeUpProperty: boolVal
//@[19:19]             "madeUpProperty": "[variables('boolVal')]",
    subnet: 'hello'
//@[20:20]             "subnet": "hello"
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[45:52]       "type": "Microsoft.Network/networkInterfaces",
  name: 'abc'
  properties: {
//@[49:51]       "properties": {
    ipConfigurations: ipConfigurations
//@[50:50]         "ipConfigurations": "[variables('ipConfigurations')]"
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[53:66]       "copy": {
  name: 'abc${i}'
  properties: {
//@[61:65]       "properties": {
    ipConfigurations: [
//@[62:64]         "ipConfigurations": [
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
//@[75:79]       "properties": {
    ipConfigurations: [
//@[76:78]         "ipConfigurations": [
      // TODO: fix this
      ipConfig
//@[77:77]           "[variables('ipConfigurations')[copyIndex()]]"
    ]
  }
}]

