targetScope = 'subscription'
param name1 string
param nAmE2 string

var name1_var = name1
var NamE2_var = nAmE2

resource name1_resource 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'name1'
  location: 'West US'
//@[12:21) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'West US' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'West US'|
  properties: {
    val1: name1_var
//@[04:08) [BCP037 (Warning)] The property "val1" is not allowed on objects of type "ResourceGroupProperties". No other properties are allowed. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |val1|
  }
}

resource naME2_resource 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'naME2'
  location: 'West US'
//@[12:21) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'West US' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'West US'|
  properties: {
    val2: NamE2_var
//@[04:08) [BCP037 (Warning)] The property "val2" is not allowed on objects of type "ResourceGroupProperties". No other properties are allowed. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |val2|
  }
}

output name1 string = name1_resource.properties.val1
//@[48:52) [BCP053 (Warning)] The type "ResourceGroupProperties" does not contain property "val1". Available properties include "provisioningState". (CodeDescription: none) |val1|
output Name2 string = naME2_resource.properties.val2
//@[48:52) [BCP053 (Warning)] The type "ResourceGroupProperties" does not contain property "val2". Available properties include "provisioningState". (CodeDescription: none) |val2|
