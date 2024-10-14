type invalid1 = resource
//@[016:024) [BCP231 (Error)] Using resource-typed parameters and outputs requires enabling EXPERIMENTAL feature "ResourceTypedParamsAndOutputs". (bicep https://aka.ms/bicep/core-diagnostics#BCP231) |resource|
//@[024:024) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP068) ||

type invalid2 = resource<>
//@[024:026) [BCP071 (Error)] Expected 1 argument, but got 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |<>|

type invalid3 = resource<'abc', 'def'>
//@[024:038) [BCP071 (Error)] Expected 1 argument, but got 2. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |<'abc', 'def'>|
type invalid4 = resource<hello>
//@[025:030) [BCP070 (Error)] Argument of type "{ bar: Astronomer.Astro/organizations }" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |hello|
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
//@[025:060) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP029) |'Microsoft.Storage/storageAccounts'|
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
//@[025:061) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP029) |'Microsoft.Storage/storageAccounts@'|
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
//@[025:066) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP029) |'Microsoft.Storage/storageAccounts@hello'|
type invalid8 = resource<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[025:089) [BCP208 (Error)] The specified namespace "notARealNamespace" is not recognized. Specify a resource reference using one of the following namespaces: "az", "sys". (bicep https://aka.ms/bicep/core-diagnostics#BCP208) |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[025:072) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP029) |':Microsoft.Storage/storageAccounts@2022-09-01'|
type invalid10 = resource<'abc' 'def'>
//@[025:038) [BCP071 (Error)] Expected 1 argument, but got 2. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |<'abc' 'def'>|
//@[032:032) [BCP236 (Error)] Expected a new line or comma character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP236) ||
type invalid11 = resource<123>
//@[026:029) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |123|
type invalid12 = resource<resourceGroup()>
//@[025:042) [BCP071 (Error)] Expected 1 argument, but got 2. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |<resourceGroup()>|
//@[039:039) [BCP236 (Error)] Expected a new line or comma character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP236) ||
//@[040:040) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||

type thisIsWeird = resource</*
//@[019:093) [BCP394 (Error)] Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted. (bicep https://aka.ms/bicep/core-diagnostics#BCP394) |resource</*\n*/'Astronomer.Astro/organizations@2023-08-01-preview'\n///  >\n>|
*/'Astronomer.Astro/organizations@2023-08-01-preview'
///  >
>

type interpolated = resource<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[029:080) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |'Microsoft.${'Storage'}/storageAccounts@2022-09-01'|

@sealed()
//@[000:009) [BCP386 (Error)] The decorator "sealed" may not be used on statements whose declared type is a reference to a resource-derived type. (bicep https://aka.ms/bicep/core-diagnostics#BCP386) |@sealed()|
type shouldNotBeSealable = resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[027:083) [BCP394 (Error)] Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted. (bicep https://aka.ms/bicep/core-diagnostics#BCP394) |resource<'Microsoft.Storage/storageAccounts@2022-09-01'>|

type hello = {
  @discriminator('hi')
//@[002:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (bicep https://aka.ms/bicep/core-diagnostics#BCP363) |@discriminator('hi')|
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[007:068) [BCP394 (Error)] Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted. (bicep https://aka.ms/bicep/core-diagnostics#BCP394) |resource<'Astronomer.Astro/organizations@2023-08-01-preview'>|
}

type typoInPropertyName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
//@[083:086) [BCP053 (Error)] The type "Microsoft.Storage/storageAccounts" does not contain property "nom". Available properties include "apiVersion", "asserts", "eTag", "extendedLocation", "id", "identity", "kind", "location", "managedBy", "managedByExtended", "name", "plan", "properties", "scale", "sku", "tags", "type", "zones". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |nom|
type typoInPropertyName2 = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
//@[105:112) [BCP083 (Error)] The type "AccessPolicyEntry" does not contain property "tenatId". Did you mean "tenantId"? (bicep https://aka.ms/bicep/core-diagnostics#BCP083) |tenatId|
type typoInPropertyName3 = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
//@[086:089) [BCP390 (Error)] The array item type access operator ('[*]') can only be used with typed arrays. (bicep https://aka.ms/bicep/core-diagnostics#BCP390) |[*]|
type typoInPropertyName4 = resource<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
//@[109:114) [BCP083 (Error)] The type "ConnectionParameter" does not contain property "tyype". Did you mean "type"? (bicep https://aka.ms/bicep/core-diagnostics#BCP083) |tyype|
type typoInPropertyName5 = resource<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type
//@[086:087) [BCP389 (Error)] The type "CustomApiPropertiesDefinition" does not declare an additional properties type. (bicep https://aka.ms/bicep/core-diagnostics#BCP389) |*|

module mod 'modules/mod.json' = {
//@[011:029) [BCP392 (Warning)] The supplied resource type identifier "not a valid resource type identifier" was not recognized as a valid resource type name. (bicep https://aka.ms/bicep/core-diagnostics#BCP392) |'modules/mod.json'|
  name: 'mod'
  params: {
    foo: {}
  }
}

