type invalid1 = resource
//@[5:13) TypeAlias invalid1. Type: error. Declaration start char: 0, length: 24

type invalid2 = resource<>
//@[5:13) TypeAlias invalid2. Type: error. Declaration start char: 0, length: 26

type invalid3 = resource<'abc', 'def'>
//@[5:13) TypeAlias invalid3. Type: error. Declaration start char: 0, length: 38
type invalid4 = resource<hello>
//@[5:13) TypeAlias invalid4. Type: error. Declaration start char: 0, length: 31
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
//@[5:13) TypeAlias invalid5. Type: error. Declaration start char: 0, length: 61
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
//@[5:13) TypeAlias invalid6. Type: error. Declaration start char: 0, length: 62
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
//@[5:13) TypeAlias invalid7. Type: error. Declaration start char: 0, length: 67
type invalid8 = resource<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[5:13) TypeAlias invalid8. Type: error. Declaration start char: 0, length: 90
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[5:13) TypeAlias invalid9. Type: error. Declaration start char: 0, length: 73
type invalid10 = resource<'abc' 'def'>
//@[5:14) TypeAlias invalid10. Type: error. Declaration start char: 0, length: 38
type invalid11 = resource<123>
//@[5:14) TypeAlias invalid11. Type: error. Declaration start char: 0, length: 30
type invalid12 = resource<resourceGroup()>
//@[5:14) TypeAlias invalid12. Type: error. Declaration start char: 0, length: 42

type thisIsWeird = resource</*
//@[5:16) TypeAlias thisIsWeird. Type: Type<Astronomer.Astro/organizations>. Declaration start char: 0, length: 93
*/'Astronomer.Astro/organizations@2023-08-01-preview'
///  >
>

type interpolated = resource<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[5:17) TypeAlias interpolated. Type: error. Declaration start char: 0, length: 81

@sealed()
type shouldNotBeSealable = resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[5:24) TypeAlias shouldNotBeSealable. Type: Type<Microsoft.Storage/storageAccounts>. Declaration start char: 0, length: 93

type hello = {
//@[5:10) TypeAlias hello. Type: Type<{ bar: Astronomer.Astro/organizations }>. Declaration start char: 0, length: 108
  @discriminator('hi')
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
}

type typoInPropertyName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
//@[5:23) TypeAlias typoInPropertyName. Type: error. Declaration start char: 0, length: 86
type typoInPropertyName2 = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
//@[5:24) TypeAlias typoInPropertyName2. Type: error. Declaration start char: 0, length: 112
type typoInPropertyName3 = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
//@[5:24) TypeAlias typoInPropertyName3. Type: error. Declaration start char: 0, length: 113
type typoInPropertyName4 = resource<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
//@[5:24) TypeAlias typoInPropertyName4. Type: error. Declaration start char: 0, length: 114
type typoInPropertyName5 = resource<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type
//@[5:24) TypeAlias typoInPropertyName5. Type: error. Declaration start char: 0, length: 113

module mod 'modules/mod.json' = {
//@[7:10) Module mod. Type: module. Declaration start char: 0, length: 77
  name: 'mod'
  params: {
    foo: {}
  }
}

