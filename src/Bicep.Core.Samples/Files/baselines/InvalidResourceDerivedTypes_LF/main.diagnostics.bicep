type invalid1 = resourceInput
//@[016:029) [BCP384 (Error)] The "resourceInput<ResourceTypeIdentifier>" type requires 1 argument(s). (bicep https://aka.ms/bicep/core-diagnostics#BCP384) |resourceInput|

type invalid2 = resourceInput<>
//@[029:031) [BCP071 (Error)] Expected 1 argument, but got 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |<>|

type invalid3 = resourceInput<'abc', 'def'>
//@[029:043) [BCP071 (Error)] Expected 1 argument, but got 2. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |<'abc', 'def'>|
type invalid4 = resourceInput<hello>
//@[030:035) [BCP070 (Error)] Argument of type "{ bar: Astronomer.Astro/organizations }" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |hello|
type invalid5 = resourceInput<'Microsoft.Storage/storageAccounts'>
//@[030:065) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP029) |'Microsoft.Storage/storageAccounts'|
type invalid6 = resourceInput<'Microsoft.Storage/storageAccounts@'>
//@[030:066) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP029) |'Microsoft.Storage/storageAccounts@'|
type invalid7 = resourceInput<'Microsoft.Storage/storageAccounts@hello'>
//@[030:071) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP029) |'Microsoft.Storage/storageAccounts@hello'|
type invalid8 = resourceInput<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[030:094) [BCP208 (Error)] The specified namespace "notARealNamespace" is not recognized. Specify a resource reference using one of the following namespaces: "az", "sys". (bicep https://aka.ms/bicep/core-diagnostics#BCP208) |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
type invalid9 = resourceInput<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[030:077) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<type-name>@<apiVersion>". (bicep https://aka.ms/bicep/core-diagnostics#BCP029) |':Microsoft.Storage/storageAccounts@2022-09-01'|
type invalid10 = resourceInput<'abc' 'def'>
//@[030:043) [BCP071 (Error)] Expected 1 argument, but got 2. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |<'abc' 'def'>|
//@[037:037) [BCP236 (Error)] Expected a new line or comma character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP236) ||
type invalid11 = resourceInput<123>
//@[031:034) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |123|
type invalid12 = resourceInput<resourceGroup()>
//@[030:047) [BCP071 (Error)] Expected 1 argument, but got 2. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |<resourceGroup()>|
//@[044:044) [BCP236 (Error)] Expected a new line or comma character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP236) ||
//@[045:045) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||

type thisIsWeird = resourceInput</*
//@[019:098) [BCP394 (Error)] Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted. (bicep https://aka.ms/bicep/core-diagnostics#BCP394) |resourceInput</*\n*/'Astronomer.Astro/organizations@2023-08-01-preview'\n///  >\n>|
*/'Astronomer.Astro/organizations@2023-08-01-preview'
///  >
>

type interpolated = resourceInput<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[034:085) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |'Microsoft.${'Storage'}/storageAccounts@2022-09-01'|

@sealed()
//@[000:009) [BCP386 (Error)] The decorator "sealed" may not be used on statements whose declared type is a reference to a resource-derived type. (bicep https://aka.ms/bicep/core-diagnostics#BCP386) |@sealed()|
type shouldNotBeSealable = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[027:088) [BCP394 (Error)] Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted. (bicep https://aka.ms/bicep/core-diagnostics#BCP394) |resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>|

type hello = {
  @discriminator('hi')
//@[002:022) [BCP363 (Error)] The "discriminator" decorator can only be applied to object-only union types with unique member types. (bicep https://aka.ms/bicep/core-diagnostics#BCP363) |@discriminator('hi')|
  bar: resourceInput<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[007:073) [BCP394 (Error)] Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted. (bicep https://aka.ms/bicep/core-diagnostics#BCP394) |resourceInput<'Astronomer.Astro/organizations@2023-08-01-preview'>|
}

type typoInPropertyName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
//@[088:091) [BCP053 (Error)] The type "Microsoft.Storage/storageAccounts" does not contain property "nom". Available properties include "apiVersion", "asserts", "dependsOn", "eTag", "extendedLocation", "id", "identity", "kind", "location", "managedBy", "managedByExtended", "name", "plan", "properties", "scale", "sku", "tags", "type", "zones". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |nom|
type typoInPropertyName2 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
//@[110:117) [BCP083 (Error)] The type "AccessPolicyEntry" does not contain property "tenatId". Did you mean "tenantId"? (bicep https://aka.ms/bicep/core-diagnostics#BCP083) |tenatId|
type typoInPropertyName3 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
//@[091:094) [BCP390 (Error)] The array item type access operator ('[*]') can only be used with typed arrays. (bicep https://aka.ms/bicep/core-diagnostics#BCP390) |[*]|
type typoInPropertyName4 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
//@[114:119) [BCP083 (Error)] The type "ConnectionParameter" does not contain property "tyype". Did you mean "type"? (bicep https://aka.ms/bicep/core-diagnostics#BCP083) |tyype|
type typoInPropertyName5 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type
//@[091:092) [BCP389 (Error)] The type "CustomApiPropertiesDefinition" does not declare an additional properties type. (bicep https://aka.ms/bicep/core-diagnostics#BCP389) |*|

module mod 'modules/mod.json' = {
//@[011:029) [BCP392 (Warning)] The supplied resource type identifier "not a valid resource type identifier" was not recognized as a valid resource type name. (bicep https://aka.ms/bicep/core-diagnostics#BCP392) |'modules/mod.json'|
  name: 'mod'
  params: {
    foo: {}
  }
}

