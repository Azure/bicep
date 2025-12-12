var vmProperties = {
//@    "vmProperties": {
//@    },
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
  evictionPolicy: 'Deallocate'
//@      "evictionPolicy": "Deallocate"
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@    {
//@      "type": "Microsoft.Compute/virtualMachines",
//@      "apiVersion": "2020-12-01",
//@      "name": "vm",
//@    }
  name: 'vm'
  location: 'West US'
//@      "location": "West US",
#disable-next-line BCP036 BCP037
  properties: vmProperties
//@      "properties": "[variables('vmProperties')]"
}
#disable-next-line no-unused-params
param storageAccount1 string = 'testStorageAccount'
//@    "storageAccount1": {
//@      "type": "string",
//@      "defaultValue": "testStorageAccount"
//@    },
#disable-next-line          no-unused-params
param storageAccount2 string = 'testStorageAccount'
//@    "storageAccount2": {
//@      "type": "string",
//@      "defaultValue": "testStorageAccount"
//@    },
#disable-next-line   no-unused-params                /* Test comment 1 */
param storageAccount3 string = 'testStorageAccount'
//@    "storageAccount3": {
//@      "type": "string",
//@      "defaultValue": "testStorageAccount"
//@    },
         #disable-next-line   no-unused-params                // Test comment 2
param storageAccount5 string = 'testStorageAccount'
//@    "storageAccount5": {
//@      "type": "string",
//@      "defaultValue": "testStorageAccount"
//@    },

#disable-diagnostics                 no-unused-params                      no-unused-vars
param storageAccount4 string = 'testStorageAccount'
//@    "storageAccount4": {
//@      "type": "string",
//@      "defaultValue": "testStorageAccount"
//@    },
var unusedVar1 = 'This is an unused variable'
//@    "unusedVar1": "This is an unused variable",
var unusedVar2 = 'This is another unused variable'
//@    "unusedVar2": "This is another unused variable",
#restore-diagnostics   no-unused-vars
param storageAccount6 string = 'testStorageAccount'
//@    "storageAccount6": {
//@      "type": "string",
//@      "defaultValue": "testStorageAccount"
//@    },
var unusedVar3 = 'This is yet another unused variable'
//@    "unusedVar3": "This is yet another unused variable"
#restore-diagnostics    no-unused-params
param storageAccount7 string = 'testStorageAccount'
//@    "storageAccount7": {
//@      "type": "string",
//@      "defaultValue": "testStorageAccount"
//@    }

