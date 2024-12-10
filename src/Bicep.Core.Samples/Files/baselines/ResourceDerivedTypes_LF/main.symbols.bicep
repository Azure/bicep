type foo = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[5:08) TypeAlias foo. Type: Type<string>. Declaration start char: 0, length: 77

type test = {
//@[5:09) TypeAlias test. Type: Type<{ resA: string, resB: string, resC: array, resD: string }>. Declaration start char: 0, length: 269
  resA: resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  resB: sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
  resC: sys.array
  resD: sys.resourceInput<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
}

type strangeFormatting = {
//@[5:22) TypeAlias strangeFormatting. Type: Type<{ test: string, test2: string, test3: string }>. Declaration start char: 0, length: 287
  test: resourceInput<

  'Astronomer.Astro/organizations@2023-08-01-preview'

>.name
  test2: resourceInput    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  test3: resourceInput</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
}

@description('I love space(s)')
type test2 = resourceInput<
//@[5:10) TypeAlias test2. Type: Type<string>. Declaration start char: 0, length: 125

     'Astronomer.Astro/organizations@2023-08-01-preview'

>.name

param bar resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@[6:09) Parameter bar. Type: Tags. Declaration start char: 0, length: 130
  tags: {
    fizz: 'buzz'
    snap: 'crackle'
  }
}

output baz resourceInput<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@[7:10) Output baz. Type: string. Declaration start char: 0, length: 101

type storageAccountName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[5:23) TypeAlias storageAccountName. Type: Type<string>. Declaration start char: 0, length: 92
type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[5:17) TypeAlias accessPolicy. Type: Type<AccessPolicyEntry>. Declaration start char: 0, length: 102
type tag = resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[5:08) TypeAlias tag. Type: Type<string>. Declaration start char: 0, length: 81

