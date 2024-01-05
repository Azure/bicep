type invalid1 = resource

type invalid2 = resource<>

type invalid3 = resource<'abc', 'def'>
type invalid4 = resource<hello>
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
type invalid8 = resource<
  'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'
>
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
type invalid10 = resource<'abc' 'def'>
type invalid11 = resource<123>
type invalid12 = resource<resourceGroup()>

type thisIsWeird = resource</*
*/'Astronomer.Astro/organizations@2023-08-01-preview'
///  >
>

type interpolated = resource<
  'Microsoft.${'Storage'}/storageAccounts@2022-09-01'
>

@sealed()
type shouldNotBeSealable = resource<
  'Microsoft.Storage/storageAccounts@2022-09-01'
>

type hello = {
  @discriminator('hi')
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
}
