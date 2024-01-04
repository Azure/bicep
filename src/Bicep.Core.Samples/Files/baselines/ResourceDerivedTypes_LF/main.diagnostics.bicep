type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@[6:9) [no-unused-params (Warning)] Parameter "bar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |bar|
  name: 'default'
  properties: {
    tags: {
      fizz: 'buzz'
      snap: 'crackle'
    }
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
  name: 'myId'
  location: 'eastus'
}

