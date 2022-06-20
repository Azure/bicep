var vmProperties = {
//@[29:38]     "vmProperties": {
  diagnosticsProfile: {
//@[30:36]       "diagnosticsProfile": {
    bootDiagnostics: {
//@[31:35]         "bootDiagnostics": {
      enabled: 123
//@[32:32]           "enabled": 123,
      storageUri: true
//@[33:33]           "storageUri": true,
      unknownProp: 'asdf'
//@[34:34]           "unknownProp": "asdf"
    }
  }
  evictionPolicy: 'Deallocate'
//@[37:37]       "evictionPolicy": "Deallocate"
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[41:47]       "type": "Microsoft.Compute/virtualMachines",
  name: 'vm'
  location: 'West US'
//@[45:45]       "location": "West US",
#disable-next-line BCP036 BCP037
  properties: vmProperties
//@[46:46]       "properties": "[variables('vmProperties')]"
}
#disable-next-line no-unused-params
param storageAccount1 string = 'testStorageAccount'
//@[11:14]     "storageAccount1": {
#disable-next-line          no-unused-params
param storageAccount2 string = 'testStorageAccount'
//@[15:18]     "storageAccount2": {
#disable-next-line   no-unused-params                /* Test comment 1 */
param storageAccount3 string = 'testStorageAccount'
//@[19:22]     "storageAccount3": {
         #disable-next-line   no-unused-params                // Test comment 2
param storageAccount5 string = 'testStorageAccount'
//@[23:26]     "storageAccount5": {
