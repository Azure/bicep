var boolVal = true
//@    "boolVal": true,

var vmProperties = {
//@    "vmProperties": {
//@    }
  diagnosticsProfile: {
//@      "diagnosticsProfile": {
//@      },
    bootDiagnostics: {
//@        "bootDiagnostics": {
//@        }
      enabled: 123
//@          "enabled": 123,
      storageUri: true
//@          "storageUri": true,
      unknownProp: 'asdf'
//@          "unknownProp": "asdf"
    }
  }
  evictionPolicy: boolVal
//@      "evictionPolicy": "[variables('boolVal')]"
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@    {
//@      "type": "Microsoft.Compute/virtualMachines",
//@      "apiVersion": "2020-12-01",
//@      "name": "vm",
//@    },
  name: 'vm'
  location: 'West US'
//@      "location": "West US",
  properties: vmProperties
//@      "properties": "[variables('vmProperties')]"
}

var ipConfigurations = [for i in range(0, 2): {
//@      {
//@        "name": "ipConfigurations",
//@        "count": "[length(range(0, 2))]",
//@        "input": {
//@        }
//@      }
  id: true
//@          "id": true,
  name: 'asdf${i}'
//@          "name": "[format('asdf{0}', range(0, 2)[copyIndex('ipConfigurations')])]",
  properties: {
//@          "properties": {
//@          }
    madeUpProperty: boolVal
//@            "madeUpProperty": "[variables('boolVal')]",
    subnet: 'hello'
//@            "subnet": "hello"
  }
}]

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@    {
//@      "type": "Microsoft.Network/networkInterfaces",
//@      "apiVersion": "2020-11-01",
//@      "name": "abc",
//@    },
  name: 'abc'
  properties: {
//@      "properties": {
//@      }
    ipConfigurations: ipConfigurations
//@        "ipConfigurations": "[variables('ipConfigurations')]"
  }
}

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@    {
//@      "copy": {
//@        "name": "nicLoop",
//@        "count": "[length(range(0, 2))]"
//@      },
//@      "type": "Microsoft.Network/networkInterfaces",
//@      "apiVersion": "2020-11-01",
//@      "name": "[format('abc{0}', range(0, 2)[copyIndex()])]",
//@    },
  name: 'abc${i}'
  properties: {
//@      "properties": {
//@      }
    ipConfigurations: [
//@        "ipConfigurations": [
//@        ]
      // TODO: fix this
      ipConfigurations[i]
//@          "[variables('ipConfigurations')[range(0, 2)[copyIndex()]]]"
    ]
  }
}]

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@    {
//@      "copy": {
//@        "name": "nicLoop2",
//@        "count": "[length(variables('ipConfigurations'))]"
//@      },
//@      "type": "Microsoft.Network/networkInterfaces",
//@      "apiVersion": "2020-11-01",
//@      "name": "[format('abc{0}', variables('ipConfigurations')[copyIndex()].name)]",
//@    }
  name: 'abc${ipConfig.name}'
  properties: {
//@      "properties": {
//@      }
    ipConfigurations: [
//@        "ipConfigurations": [
//@        ]
      // TODO: fix this
      ipConfig
//@          "[variables('ipConfigurations')[copyIndex()]]"
    ]
  }
}]

