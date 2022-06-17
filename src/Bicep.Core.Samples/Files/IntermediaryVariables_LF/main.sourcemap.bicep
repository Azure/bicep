var boolVal = true
//@[26:26]     "boolVal": true,

var vmProperties = {
//@[27:36]     "vmProperties": {
  diagnosticsProfile: {
//@[28:34]       "diagnosticsProfile": {
    bootDiagnostics: {
//@[29:33]         "bootDiagnostics": {
      enabled: 123
//@[30:30]           "enabled": 123,
      storageUri: true
//@[31:31]           "storageUri": true,
      unknownProp: 'asdf'
//@[32:32]           "unknownProp": "asdf"
    }
  }
  evictionPolicy: boolVal
//@[35:35]       "evictionPolicy": "[variables('boolVal')]"
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[39:45]       "type": "Microsoft.Compute/virtualMachines",
  name: 'vm'
  location: 'West US'
//@[43:43]       "location": "West US",
  properties: vmProperties
//@[44:44]       "properties": "[variables('vmProperties')]"
}

var ipConfigurations = [for i in range(0, 2): {
//@[13:24]         "name": "ipConfigurations",
  id: true
//@[17:17]           "id": true,
  name: 'asdf${i}'
//@[18:18]           "name": "[format('asdf{0}', range(0, 2)[copyIndex('ipConfigurations')])]",
  properties: {
//@[19:22]           "properties": {
    madeUpProperty: boolVal
//@[20:20]             "madeUpProperty": "[variables('boolVal')]",
    subnet: 'hello'
//@[21:21]             "subnet": "hello"
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[46:53]       "type": "Microsoft.Network/networkInterfaces",
  name: 'abc'
  properties: {
//@[50:52]       "properties": {
    ipConfigurations: ipConfigurations
//@[51:51]         "ipConfigurations": "[variables('ipConfigurations')]"
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[54:67]       "copy": {
  name: 'abc${i}'
  properties: {
//@[62:66]       "properties": {
    ipConfigurations: [
//@[63:65]         "ipConfigurations": [
      // TODO: fix this
      ipConfigurations[i]
//@[64:64]           "[variables('ipConfigurations')[range(0, 2)[copyIndex()]]]"
    ]
  }
}]

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[68:81]       "copy": {
  name: 'abc${ipConfig.name}'
  properties: {
//@[76:80]       "properties": {
    ipConfigurations: [
//@[77:79]         "ipConfigurations": [
      // TODO: fix this
      ipConfig
//@[78:78]           "[variables('ipConfigurations')[copyIndex()]]"
    ]
  }
}]

