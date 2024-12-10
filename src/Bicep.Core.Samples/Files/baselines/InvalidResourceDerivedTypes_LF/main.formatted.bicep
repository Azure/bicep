type invalid1 = resourceInput

type invalid2 = resourceInput<>

type invalid3 = resourceInput<'abc', 'def'>
type invalid4 = resourceInput<hello>
type invalid5 = resourceInput<'Microsoft.Storage/storageAccounts'>
type invalid6 = resourceInput<'Microsoft.Storage/storageAccounts@'>
type invalid7 = resourceInput<'Microsoft.Storage/storageAccounts@hello'>
type invalid8 = resourceInput<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
type invalid9 = resourceInput<':Microsoft.Storage/storageAccounts@2022-09-01'>
type invalid10 = resourceInput<'abc' 'def'>
type invalid11 = resourceInput<123>
type invalid12 = resourceInput<resourceGroup()>

type thisIsWeird = resourceInput<
  /*
*/ 'Astronomer.Astro/organizations@2023-08-01-preview'
  ///  >
>

type interpolated = resourceInput<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>

@sealed()
type shouldNotBeSealable = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>

type hello = {
  @discriminator('hi')
  bar: resourceInput<'Astronomer.Astro/organizations@2023-08-01-preview'>
}

type typoInPropertyName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
type typoInPropertyName2 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
type typoInPropertyName3 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
type typoInPropertyName4 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
type typoInPropertyName5 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type

module mod 'modules/mod.json' = {
  name: 'mod'
  params: {
    foo: {}
  }
}
