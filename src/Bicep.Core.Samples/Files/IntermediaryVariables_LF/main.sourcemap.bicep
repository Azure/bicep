var boolVal = true
//@[line00->line25]     "boolVal": true,

var vmProperties = {
//@[line02->line26]     "vmProperties": {
//@[line02->line35]     }
  diagnosticsProfile: {
//@[line03->line27]       "diagnosticsProfile": {
//@[line03->line33]       },
    bootDiagnostics: {
//@[line04->line28]         "bootDiagnostics": {
//@[line04->line32]         }
      enabled: 123
//@[line05->line29]           "enabled": 123,
      storageUri: true
//@[line06->line30]           "storageUri": true,
      unknownProp: 'asdf'
//@[line07->line31]           "unknownProp": "asdf"
    }
  }
  evictionPolicy: boolVal
//@[line10->line34]       "evictionPolicy": "[variables('boolVal')]"
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[line13->line38]     {
//@[line13->line39]       "type": "Microsoft.Compute/virtualMachines",
//@[line13->line40]       "apiVersion": "2020-12-01",
//@[line13->line41]       "name": "vm",
//@[line13->line44]     },
  name: 'vm'
  location: 'West US'
//@[line15->line42]       "location": "West US",
  properties: vmProperties
//@[line16->line43]       "properties": "[variables('vmProperties')]"
}

var ipConfigurations = [for i in range(0, 2): {
//@[line19->line12]       {
//@[line19->line13]         "name": "ipConfigurations",
//@[line19->line14]         "count": "[length(range(0, 2))]",
//@[line19->line15]         "input": {
//@[line19->line22]         }
//@[line19->line23]       }
  id: true
//@[line20->line16]           "id": true,
  name: 'asdf${i}'
//@[line21->line17]           "name": "[format('asdf{0}', range(0, 2)[copyIndex('ipConfigurations')])]",
  properties: {
//@[line22->line18]           "properties": {
//@[line22->line21]           }
    madeUpProperty: boolVal
//@[line23->line19]             "madeUpProperty": "[variables('boolVal')]",
    subnet: 'hello'
//@[line24->line20]             "subnet": "hello"
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[line28->line45]     {
//@[line28->line46]       "type": "Microsoft.Network/networkInterfaces",
//@[line28->line47]       "apiVersion": "2020-11-01",
//@[line28->line48]       "name": "abc",
//@[line28->line52]     },
  name: 'abc'
  properties: {
//@[line30->line49]       "properties": {
//@[line30->line51]       }
    ipConfigurations: ipConfigurations
//@[line31->line50]         "ipConfigurations": "[variables('ipConfigurations')]"
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[line35->line53]     {
//@[line35->line54]       "copy": {
//@[line35->line55]         "name": "nicLoop",
//@[line35->line56]         "count": "[length(range(0, 2))]"
//@[line35->line57]       },
//@[line35->line58]       "type": "Microsoft.Network/networkInterfaces",
//@[line35->line59]       "apiVersion": "2020-11-01",
//@[line35->line60]       "name": "[format('abc{0}', range(0, 2)[copyIndex()])]",
//@[line35->line66]     },
  name: 'abc${i}'
  properties: {
//@[line37->line61]       "properties": {
//@[line37->line65]       }
    ipConfigurations: [
//@[line38->line62]         "ipConfigurations": [
//@[line38->line64]         ]
      // TODO: fix this
      ipConfigurations[i]
//@[line40->line63]           "[variables('ipConfigurations')[range(0, 2)[copyIndex()]]]"
    ]
  }
}]

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[line45->line67]     {
//@[line45->line68]       "copy": {
//@[line45->line69]         "name": "nicLoop2",
//@[line45->line70]         "count": "[length(variables('ipConfigurations'))]"
//@[line45->line71]       },
//@[line45->line72]       "type": "Microsoft.Network/networkInterfaces",
//@[line45->line73]       "apiVersion": "2020-11-01",
//@[line45->line74]       "name": "[format('abc{0}', variables('ipConfigurations')[copyIndex()].name)]",
//@[line45->line80]     }
  name: 'abc${ipConfig.name}'
  properties: {
//@[line47->line75]       "properties": {
//@[line47->line79]       }
    ipConfigurations: [
//@[line48->line76]         "ipConfigurations": [
//@[line48->line78]         ]
      // TODO: fix this
      ipConfig
//@[line50->line77]           "[variables('ipConfigurations')[copyIndex()]]"
    ]
  }
}]

