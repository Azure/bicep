resource foo 'Microsoft.Storage/storageAccounts@2021-01-01' = {
  name: 'foo'
  kind: 'StorageV2'
  location: ''
//@[12:14) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: '' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |''|
  sku: {
    name: 'Standard_RAGRS'
  }
}
