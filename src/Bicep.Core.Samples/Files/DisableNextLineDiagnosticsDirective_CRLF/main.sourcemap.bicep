var vmProperties = {
//@[29:38]     "vmProperties": {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[41:47]       "type": "Microsoft.Compute/virtualMachines",
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036 BCP037
  properties: vmProperties
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
