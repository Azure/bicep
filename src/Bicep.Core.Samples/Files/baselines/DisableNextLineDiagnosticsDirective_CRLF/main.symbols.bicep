var vmProperties = {
//@[4:16) Variable vmProperties. Type: object. Declaration start char: 0, length: 187
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
//@[9:11) Resource vm. Type: Microsoft.Compute/virtualMachines@2020-12-01. Declaration start char: 0, length: 164
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036 BCP037
  properties: vmProperties
}
#disable-next-line no-unused-params
param storageAccount1 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount1. Type: string. Declaration start char: 0, length: 51
#disable-next-line          no-unused-params
param storageAccount2 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount2. Type: string. Declaration start char: 0, length: 51
#disable-next-line   no-unused-params                /* Test comment 1 */
param storageAccount3 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount3. Type: string. Declaration start char: 0, length: 51
         #disable-next-line   no-unused-params                // Test comment 2
param storageAccount5 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount5. Type: string. Declaration start char: 0, length: 51

#disable-diagnostics                 no-unused-params                      no-unused-vars
param storageAccount4 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount4. Type: string. Declaration start char: 0, length: 51
var unusedVar1 = 'This is an unused variable'
//@[4:14) Variable unusedVar1. Type: 'This is an unused variable'. Declaration start char: 0, length: 45
var unusedVar2 = 'This is another unused variable'
//@[4:14) Variable unusedVar2. Type: 'This is another unused variable'. Declaration start char: 0, length: 50
#restore-diagnostics   no-unused-vars
param storageAccount6 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount6. Type: string. Declaration start char: 0, length: 51
var unusedVar3 = 'This is yet another unused variable'
//@[4:14) Variable unusedVar3. Type: 'This is yet another unused variable'. Declaration start char: 0, length: 54
#restore-diagnostics    no-unused-params
param storageAccount7 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount7. Type: string. Declaration start char: 0, length: 51

