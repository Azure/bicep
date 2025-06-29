type foo = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name

type test = {
  resA: resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  resB: sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
  resC: sys.array
  resD: sys.resourceInput<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
}

type strangeFormatting = {
  test: resourceInput<
    'Astronomer.Astro/organizations@2023-08-01-preview'
  >.name
  test2: resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  test3: resourceInput</*    */ 'Microsoft.Storage/storageAccounts@2023-01-01' /*     */>.name
}

@description('I love space(s)')
type test2 = resourceInput<
  'Astronomer.Astro/organizations@2023-08-01-preview'
>.name

param bar resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties = {
  tags: {
    fizz: 'buzz'
    snap: 'crackle'
  }
}

output baz resourceInput<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'

type storageAccountName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
type tag = resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
