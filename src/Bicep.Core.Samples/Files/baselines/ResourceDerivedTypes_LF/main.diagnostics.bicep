type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>

type test = {
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
  resC: sys.array
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>
}

type strangeFormattings = {
  test: resource<

  'Astronomer.Astro/organizations@2023-08-01-preview'

>
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>
}

@description('I love space(s)')
type test2 = resource<

     'Astronomer.Astro/organizations@2023-08-01-preview'

>

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

type storageAccountName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
type accessPolicy = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
type tag = resource<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*

