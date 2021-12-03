// $1 = resourceGroup
// $2 = 'name'

targetScope = 'subscription'
param location string
//@[6:14) [no-unused-params (Warning)] Parameter "location" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |location|

resource resourceGroup resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
//@[23:36) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |resourceGroup|
//@[23:88) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {|
//@[88:88) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
  name: 'name' 'name'
//@[2:6) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |name|
  location: location
//@[2:10) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
// Insert snippet here

