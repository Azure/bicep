type invalid1 = resource
//@[16:24) [BCP231 (Error)] Using resource-typed parameters and outputs requires enabling EXPERIMENTAL feature "ResourceTypedParamsAndOutputs". (CodeDescription: none) |resource|
//@[24:24) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||

type invalid2 = resource<>
//@[24:26) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |<>|

type invalid3 = resource<'abc', 'def'>
//@[24:38) [BCP071 (Error)] Expected 1 argument, but got 2. (CodeDescription: none) |<'abc', 'def'>|
type invalid4 = resource<hello>
//@[25:30) [BCP070 (Error)] Argument of type "{ bar: Astronomer.Astro/organizations }" is not assignable to parameter of type "string". (CodeDescription: none) |hello|
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
//@[25:60) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Storage/storageAccounts'|
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
//@[25:61) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Storage/storageAccounts@'|
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
//@[25:66) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Storage/storageAccounts@hello'|
type invalid8 = resource<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[25:89) [BCP208 (Error)] The specified namespace "notARealNamespace" is not recognized. Specify a resource reference using one of the following namespaces: "az", "sys". (CodeDescription: none) |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[25:72) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |':Microsoft.Storage/storageAccounts@2022-09-01'|
type invalid10 = resource<'abc' 'def'>
//@[25:38) [BCP071 (Error)] Expected 1 argument, but got 2. (CodeDescription: none) |<'abc' 'def'>|
//@[32:32) [BCP236 (Error)] Expected a new line or comma character at this location. (CodeDescription: none) ||
type invalid11 = resource<123>
//@[26:29) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (CodeDescription: none) |123|
type invalid12 = resource<resourceGroup()>
//@[25:42) [BCP071 (Error)] Expected 1 argument, but got 2. (CodeDescription: none) |<resourceGroup()>|
//@[39:39) [BCP236 (Error)] Expected a new line or comma character at this location. (CodeDescription: none) ||
//@[40:40) [BCP243 (Error)] Parentheses must contain exactly one expression. (CodeDescription: none) ||

type thisIsWeird = resource</*
*/'Astronomer.Astro/organizations@2023-08-01-preview' 
//@[54:54) [BCP237 (Error)] Expected a comma character at this location. (CodeDescription: none) ||
///  >
>

type shouldWeBlockThis = resource<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>

@sealed() // this was offered as a completion
//@[00:09) [BCP316 (Error)] The "sealed" decorator may not be used on object types with an explicit additional properties type declaration. (CodeDescription: none) |@sealed()|
type shouldWeBlockThis2 = resource<'Microsoft.Storage/storageAccounts@2022-09-01'>

type hello = {
  @discriminator('hi')
//@[02:22) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (CodeDescription: none) |@discriminator('hi')|
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
}

