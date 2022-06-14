var boolVal = true
//@[25:25]     "boolVal": true,\r

var vmProperties = {
//@[26:35]     "vmProperties": {\r
  diagnosticsProfile: {
//@[27:33]       "diagnosticsProfile": {\r
    bootDiagnostics: {
//@[28:32]         "bootDiagnostics": {\r
      enabled: 123
//@[29:29]           "enabled": 123,\r
      storageUri: true
//@[30:30]           "storageUri": true,\r
      unknownProp: 'asdf'
//@[31:31]           "unknownProp": "asdf"\r
    }
  }
  evictionPolicy: boolVal
//@[34:34]       "evictionPolicy": "[variables('boolVal')]"\r
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[38:44]       "type": "Microsoft.Compute/virtualMachines",\r
  name: 'vm'
  location: 'West US'
//@[42:42]       "location": "West US",\r
  properties: vmProperties
//@[43:43]       "properties": "[variables('vmProperties')]"\r
}

var ipConfigurations = [for i in range(0, 2): {
//@[12:23]         "name": "ipConfigurations",\r
  id: true
//@[16:16]           "id": true,\r
  name: 'asdf${i}'
//@[17:17]           "name": "[format('asdf{0}', range(0, 2)[copyIndex('ipConfigurations')])]",\r
  properties: {
//@[18:21]           "properties": {\r
    madeUpProperty: boolVal
//@[19:19]             "madeUpProperty": "[variables('boolVal')]",\r
    subnet: 'hello'
//@[20:20]             "subnet": "hello"\r
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[45:52]       "type": "Microsoft.Network/networkInterfaces",\r
  name: 'abc'
  properties: {
//@[49:51]       "properties": {\r
    ipConfigurations: ipConfigurations
//@[50:50]         "ipConfigurations": "[variables('ipConfigurations')]"\r
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[53:66]       "copy": {\r
  name: 'abc${i}'
  properties: {
//@[61:65]       "properties": {\r
    ipConfigurations: [
//@[62:64]         "ipConfigurations": [\r
      // TODO: fix this
      ipConfigurations[i]
//@[63:63]           "[variables('ipConfigurations')[range(0, 2)[copyIndex()]]]"\r
    ]
  }
}]

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[67:80]       "copy": {\r
  name: 'abc${ipConfig.name}'
  properties: {
//@[75:79]       "properties": {\r
    ipConfigurations: [
//@[76:78]         "ipConfigurations": [\r
      // TODO: fix this
      ipConfig
//@[77:77]           "[variables('ipConfigurations')[copyIndex()]]"\r
    ]
  }
}]

