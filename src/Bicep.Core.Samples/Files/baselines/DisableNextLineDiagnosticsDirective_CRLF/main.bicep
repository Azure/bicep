var vmProperties = {
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
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036 BCP037
  properties: vmProperties
}
#disable-next-line no-unused-params
param storageAccount1 string = 'testStorageAccount'
#disable-next-line          no-unused-params
param storageAccount2 string = 'testStorageAccount'
#disable-next-line   no-unused-params                /* Test comment 1 */
param storageAccount3 string = 'testStorageAccount'
         #disable-next-line   no-unused-params                // Test comment 2
param storageAccount5 string = 'testStorageAccount'

#disable-diagnostics                 no-unused-params                      no-unused-vars
param storageAccount4 string = 'testStorageAccount'
var unusedVar1 = 'This is an unused variable'
var unusedVar2 = 'This is another unused variable'
#restore-diagnostics   no-unused-vars
param storageAccount6 string = 'testStorageAccount'
var unusedVar3 = 'This is yet another unused variable'
#restore-diagnostics    no-unused-params
param storageAccount7 string = 'testStorageAccount'
