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
//@[12:21) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'West US' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'West US'|
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
