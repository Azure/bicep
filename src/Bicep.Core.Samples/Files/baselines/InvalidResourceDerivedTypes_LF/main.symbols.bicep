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

@sealed()
type shouldNotBeSealable2 = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
//@[5:25) TypeAlias shouldNotBeSealable2. Type: Type<StorageAccountPropertiesCreateParametersOrStorageAccountProperties>. Declaration start char: 0, length: 110

@sealed()
type shouldNotBeSealable3 = resourceOutput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties?
//@[5:25) TypeAlias shouldNotBeSealable3. Type: Type<StorageAccountPropertiesCreateParametersOrStorageAccountProperties | null>. Declaration start char: 0, length: 112

@sealed()
type shouldNotBeSealable4 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*
//@[5:25) TypeAlias shouldNotBeSealable4. Type: Type<ConnectionParameter>. Declaration start char: 0, length: 124

@sealed()
type shouldNotBeSealable5 = shouldNotBeSealable2
//@[5:25) TypeAlias shouldNotBeSealable5. Type: Type<StorageAccountPropertiesCreateParametersOrStorageAccountProperties>. Declaration start char: 0, length: 58

@sealed()
param shouldNotBeSealable6 resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
//@[6:26) Parameter shouldNotBeSealable6. Type: StorageAccountPropertiesCreateParametersOrStorageAccountProperties. Declaration start char: 0, length: 109

@sealed()
param shouldNotBeSealable7 shouldNotBeSealable2
//@[6:26) Parameter shouldNotBeSealable7. Type: StorageAccountPropertiesCreateParametersOrStorageAccountProperties. Declaration start char: 0, length: 57

type shouldNotBeSealable8 = {
//@[5:25) TypeAlias shouldNotBeSealable8. Type: Type<{ prop: StorageAccountPropertiesCreateParametersOrStorageAccountProperties }>. Declaration start char: 0, length: 124
  @sealed()
  prop: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
}

type containsResourceDerivedTypes = {
//@[5:33) TypeAlias containsResourceDerivedTypes. Type: Type<{ prop: StorageAccountPropertiesCreateParametersOrStorageAccountProperties, tuple: [string, StorageAccountPropertiesCreateParametersOrStorageAccountProperties], array: StorageAccountPropertiesCreateParametersOrStorageAccountProperties[], *: StorageAccountPropertiesCreateParametersOrStorageAccountProperties }>. Declaration start char: 0, length: 374
  prop: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
  tuple: [string, resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties]
  array: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties[]
  *: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
}

@sealed()
type shouldNotBeSealable9 = containsResourceDerivedTypes.prop
//@[5:25) TypeAlias shouldNotBeSealable9. Type: Type<StorageAccountPropertiesCreateParametersOrStorageAccountProperties>. Declaration start char: 0, length: 71

@sealed()
type shouldNotBeSealable10 = containsResourceDerivedTypes.tuple[1]
//@[5:26) TypeAlias shouldNotBeSealable10. Type: Type<StorageAccountPropertiesCreateParametersOrStorageAccountProperties>. Declaration start char: 0, length: 76

@sealed()
type shouldNotBeSealable11 = containsResourceDerivedTypes.array[*]
//@[5:26) TypeAlias shouldNotBeSealable11. Type: Type<StorageAccountPropertiesCreateParametersOrStorageAccountProperties>. Declaration start char: 0, length: 76

@sealed()
type shouldNotBeSealable12 = containsResourceDerivedTypes.*
//@[5:26) TypeAlias shouldNotBeSealable12. Type: Type<StorageAccountPropertiesCreateParametersOrStorageAccountProperties>. Declaration start char: 0, length: 69

@sealed()
type shouldNotBeSealable13 = containsResourceDerivedTypes['prop']
//@[5:26) TypeAlias shouldNotBeSealable13. Type: Type<StorageAccountPropertiesCreateParametersOrStorageAccountProperties>. Declaration start char: 0, length: 75

@sealed()
type sealableUserDefinedObject = {
//@[5:30) TypeAlias sealableUserDefinedObject. Type: Type<{ prop: containsResourceDerivedTypes.prop }>. Declaration start char: 0, length: 88
  prop: containsResourceDerivedTypes.prop
}

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

