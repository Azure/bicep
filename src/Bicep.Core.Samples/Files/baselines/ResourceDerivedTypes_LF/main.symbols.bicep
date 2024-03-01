type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[5:08) TypeAlias foo. Type: Type<string>. Declaration start char: 0, length: 72

type test = {
//@[5:09) TypeAlias test. Type: Type<{ resA: string, resB: string, resC: array, resD: string }>. Declaration start char: 0, length: 254
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
  resC: sys.array
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
}

type strangeFormattings = {
//@[5:23) TypeAlias strangeFormattings. Type: Type<{ test: string, test2: string, test3: string }>. Declaration start char: 0, length: 273
  test: resource<

  'Astronomer.Astro/organizations@2023-08-01-preview'

>.name
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
}

@description('I love space(s)')
type test2 = resource<
//@[5:10) TypeAlias test2. Type: Type<string>. Declaration start char: 0, length: 120

     'Astronomer.Astro/organizations@2023-08-01-preview'

>.name

param bar resource<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@[6:09) Parameter bar. Type: Tags. Declaration start char: 0, length: 125
  tags: {
    fizz: 'buzz'
    snap: 'crackle'
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@[7:10) Output baz. Type: string. Declaration start char: 0, length: 96

type storageAccountName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[5:23) TypeAlias storageAccountName. Type: Type<string>. Declaration start char: 0, length: 87
type accessPolicy = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[5:17) TypeAlias accessPolicy. Type: Type<AccessPolicyEntry>. Declaration start char: 0, length: 97
type tag = resource<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[5:08) TypeAlias tag. Type: Type<string>. Declaration start char: 0, length: 76

