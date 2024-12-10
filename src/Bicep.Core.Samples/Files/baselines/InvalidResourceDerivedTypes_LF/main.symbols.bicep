type invalid1 = resourceInput
//@[5:13) TypeAlias invalid1. Type: error. Declaration start char: 0, length: 29

type invalid2 = resourceInput<>
//@[5:13) TypeAlias invalid2. Type: error. Declaration start char: 0, length: 31

type invalid3 = resourceInput<'abc', 'def'>
//@[5:13) TypeAlias invalid3. Type: error. Declaration start char: 0, length: 43
type invalid4 = resourceInput<hello>
//@[5:13) TypeAlias invalid4. Type: error. Declaration start char: 0, length: 36
type invalid5 = resourceInput<'Microsoft.Storage/storageAccounts'>
//@[5:13) TypeAlias invalid5. Type: error. Declaration start char: 0, length: 66
type invalid6 = resourceInput<'Microsoft.Storage/storageAccounts@'>
//@[5:13) TypeAlias invalid6. Type: error. Declaration start char: 0, length: 67
type invalid7 = resourceInput<'Microsoft.Storage/storageAccounts@hello'>
//@[5:13) TypeAlias invalid7. Type: error. Declaration start char: 0, length: 72
type invalid8 = resourceInput<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[5:13) TypeAlias invalid8. Type: error. Declaration start char: 0, length: 95
type invalid9 = resourceInput<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[5:13) TypeAlias invalid9. Type: error. Declaration start char: 0, length: 78
type invalid10 = resourceInput<'abc' 'def'>
//@[5:14) TypeAlias invalid10. Type: error. Declaration start char: 0, length: 43
type invalid11 = resourceInput<123>
//@[5:14) TypeAlias invalid11. Type: error. Declaration start char: 0, length: 35
type invalid12 = resourceInput<resourceGroup()>
//@[5:14) TypeAlias invalid12. Type: error. Declaration start char: 0, length: 47

type thisIsWeird = resourceInput</*
//@[5:16) TypeAlias thisIsWeird. Type: Type<Astronomer.Astro/organizations>. Declaration start char: 0, length: 98
*/'Astronomer.Astro/organizations@2023-08-01-preview'
///  >
>

type interpolated = resourceInput<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[5:17) TypeAlias interpolated. Type: error. Declaration start char: 0, length: 86

@sealed()
type shouldNotBeSealable = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[5:24) TypeAlias shouldNotBeSealable. Type: Type<Microsoft.Storage/storageAccounts>. Declaration start char: 0, length: 98

type hello = {
//@[5:10) TypeAlias hello. Type: Type<{ bar: Astronomer.Astro/organizations }>. Declaration start char: 0, length: 113
  @discriminator('hi')
  bar: resourceInput<'Astronomer.Astro/organizations@2023-08-01-preview'>
}

type typoInPropertyName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
//@[5:23) TypeAlias typoInPropertyName. Type: error. Declaration start char: 0, length: 91
type typoInPropertyName2 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
//@[5:24) TypeAlias typoInPropertyName2. Type: error. Declaration start char: 0, length: 117
type typoInPropertyName3 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
//@[5:24) TypeAlias typoInPropertyName3. Type: error. Declaration start char: 0, length: 118
type typoInPropertyName4 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
//@[5:24) TypeAlias typoInPropertyName4. Type: error. Declaration start char: 0, length: 119
type typoInPropertyName5 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type
//@[5:24) TypeAlias typoInPropertyName5. Type: error. Declaration start char: 0, length: 118

module mod 'modules/mod.json' = {
//@[7:10) Module mod. Type: module. Declaration start char: 0, length: 77
  name: 'mod'
  params: {
    foo: {}
  }
}

