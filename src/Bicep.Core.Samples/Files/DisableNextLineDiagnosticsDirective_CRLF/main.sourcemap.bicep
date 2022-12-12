var vmProperties = {
//@[line00->line29]     "vmProperties": {
//@[line00->line38]     }
  diagnosticsProfile: {
//@[line01->line30]       "diagnosticsProfile": {
//@[line01->line36]       },
    bootDiagnostics: {
//@[line02->line31]         "bootDiagnostics": {
//@[line02->line35]         }
      enabled: 123
//@[line03->line32]           "enabled": 123,
      storageUri: true
//@[line04->line33]           "storageUri": true,
      unknownProp: 'asdf'
//@[line05->line34]           "unknownProp": "asdf"
    }
  }
  evictionPolicy: 'Deallocate'
//@[line08->line37]       "evictionPolicy": "Deallocate"
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[line10->line41]     {
//@[line10->line42]       "type": "Microsoft.Compute/virtualMachines",
//@[line10->line43]       "apiVersion": "2020-12-01",
//@[line10->line44]       "name": "vm",
//@[line10->line47]     }
  name: 'vm'
  location: 'West US'
//@[line12->line45]       "location": "West US",
#disable-next-line BCP036 BCP037
  properties: vmProperties
//@[line14->line46]       "properties": "[variables('vmProperties')]"
}
#disable-next-line no-unused-params
param storageAccount1 string = 'testStorageAccount'
//@[line17->line11]     "storageAccount1": {
//@[line17->line12]       "type": "string",
//@[line17->line13]       "defaultValue": "testStorageAccount"
//@[line17->line14]     },
#disable-next-line          no-unused-params
param storageAccount2 string = 'testStorageAccount'
//@[line19->line15]     "storageAccount2": {
//@[line19->line16]       "type": "string",
//@[line19->line17]       "defaultValue": "testStorageAccount"
//@[line19->line18]     },
#disable-next-line   no-unused-params                /* Test comment 1 */
param storageAccount3 string = 'testStorageAccount'
//@[line21->line19]     "storageAccount3": {
//@[line21->line20]       "type": "string",
//@[line21->line21]       "defaultValue": "testStorageAccount"
//@[line21->line22]     },
         #disable-next-line   no-unused-params                // Test comment 2
param storageAccount5 string = 'testStorageAccount'
//@[line23->line23]     "storageAccount5": {
//@[line23->line24]       "type": "string",
//@[line23->line25]       "defaultValue": "testStorageAccount"
//@[line23->line26]     }
