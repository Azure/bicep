type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name

type test = {
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
  resC: sys.array
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
}

type strangeFormattings = {
  test: resource<

  'Astronomer.Astro/organizations@2023-08-01-preview'

>.name
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
}

@description('I love space(s)')
type test2 = resource<

     'Astronomer.Astro/organizations@2023-08-01-preview'

>.name

param bar resource<'Microsoft.Resources/tags@2022-09-01'>.properties = {
  tags: {
    fizz: 'buzz'
    snap: 'crackle'
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'

type storageAccountName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
type accessPolicy = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
type tag = resource<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
