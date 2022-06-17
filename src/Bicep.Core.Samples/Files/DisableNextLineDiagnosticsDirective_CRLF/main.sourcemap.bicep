var vmProperties = {
//@[30:39]     "vmProperties": {
  diagnosticsProfile: {
//@[31:37]       "diagnosticsProfile": {
    bootDiagnostics: {
//@[32:36]         "bootDiagnostics": {
      enabled: 123
//@[33:33]           "enabled": 123,
      storageUri: true
//@[34:34]           "storageUri": true,
      unknownProp: 'asdf'
//@[35:35]           "unknownProp": "asdf"
    }
  }
  evictionPolicy: 'Deallocate'
//@[38:38]       "evictionPolicy": "Deallocate"
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[42:48]       "type": "Microsoft.Compute/virtualMachines",
  name: 'vm'
  location: 'West US'
//@[46:46]       "location": "West US",
#disable-next-line BCP036 BCP037
  properties: vmProperties
//@[47:47]       "properties": "[variables('vmProperties')]"
}
#disable-next-line no-unused-params
param storageAccount1 string = 'testStorageAccount'
//@[12:15]     "storageAccount1": {
#disable-next-line          no-unused-params
param storageAccount2 string = 'testStorageAccount'
//@[16:19]     "storageAccount2": {
#disable-next-line   no-unused-params                /* Test comment 1 */
param storageAccount3 string = 'testStorageAccount'
//@[20:23]     "storageAccount3": {
         #disable-next-line   no-unused-params                // Test comment 2
param storageAccount5 string = 'testStorageAccount'
//@[24:27]     "storageAccount5": {
