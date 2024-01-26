type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[5:08) TypeAlias foo. Type: Type<Microsoft.Storage/storageAccounts>. Declaration start char: 0, length: 67

type test = {
//@[5:09) TypeAlias test. Type: Type<{ resA: Microsoft.Storage/storageAccounts, resB: Microsoft.Storage/storageAccounts, resC: array, resD: Microsoft.Storage/storageAccounts }>. Declaration start char: 0, length: 239
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
  resC: sys.array
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>
}

type strangeFormattings = {
//@[5:23) TypeAlias strangeFormattings. Type: Type<{ test: Astronomer.Astro/organizations, test2: Microsoft.Storage/storageAccounts, test3: Microsoft.Storage/storageAccounts }>. Declaration start char: 0, length: 258
  test: resource<

  'Astronomer.Astro/organizations@2023-08-01-preview'

>
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>
}

@description('I love space(s)')
type test2 = resource<
//@[5:10) TypeAlias test2. Type: Type<Astronomer.Astro/organizations>. Declaration start char: 0, length: 115

     'Astronomer.Astro/organizations@2023-08-01-preview'

>

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@[6:09) Parameter bar. Type: Microsoft.Resources/tags. Declaration start char: 0, length: 160
  name: 'default'
  properties: {
    tags: {
      fizz: 'buzz'
      snap: 'crackle'
    }
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
//@[7:10) Output baz. Type: Microsoft.ManagedIdentity/userAssignedIdentities. Declaration start char: 0, length: 124
  name: 'myId'
  location: 'eastus'
}

type storageAccountName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[5:23) TypeAlias storageAccountName. Type: Type<string>. Declaration start char: 0, length: 87
type accessPolicy = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[5:17) TypeAlias accessPolicy. Type: Type<AccessPolicyEntry>. Declaration start char: 0, length: 97
type tag = resource<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[5:08) TypeAlias tag. Type: Type<string>. Declaration start char: 0, length: 76

