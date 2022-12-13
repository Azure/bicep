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
//@    }
