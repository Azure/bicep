type foo = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[5:08) [no-unused-types (Warning)] Type "foo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-types) |foo|

type test = {
//@[5:09) [no-unused-types (Warning)] Type "test" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-types) |test|
  resA: resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  resB: sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
  resC: sys.array
  resD: sys.resourceInput<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
}

type strangeFormatting = {
//@[5:22) [no-unused-types (Warning)] Type "strangeFormatting" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-types) |strangeFormatting|
  test: resourceInput<

  'Astronomer.Astro/organizations@2023-08-01-preview'

>.name
  test2: resourceInput    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  test3: resourceInput</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
}

@description('I love space(s)')
type test2 = resourceInput<
//@[5:10) [no-unused-types (Warning)] Type "test2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-types) |test2|

     'Astronomer.Astro/organizations@2023-08-01-preview'

>.name

param bar resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@[6:09) [no-unused-params (Warning)] Parameter "bar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |bar|
  tags: {
    fizz: 'buzz'
    snap: 'crackle'
  }
}

output baz resourceInput<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'

type storageAccountName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[5:23) [no-unused-types (Warning)] Type "storageAccountName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-types) |storageAccountName|
type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[5:17) [no-unused-types (Warning)] Type "accessPolicy" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-types) |accessPolicy|
type tag = resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[5:08) [no-unused-types (Warning)] Type "tag" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-types) |tag|

