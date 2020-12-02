targetScope = 'subscription'
param name1 string
param nAmE2 string

var name1_var = name1
var NamE2_var = nAmE2

resource name1_resource 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'name1'
  location: 'West US'
  properties: {
    val1: name1_var
//@[4:8) [BCP037 (Warning)] No other properties are allowed on objects of type "ResourceGroupProperties". |val1|
  }
}

resource naME2_resource 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'naME2'
  location: 'West US'
  properties: {
    val2: NamE2_var
//@[4:8) [BCP037 (Warning)] No other properties are allowed on objects of type "ResourceGroupProperties". |val2|
  }
}

output name1_output string = name1_resource.properties.val1
//@[55:59) [BCP053 (Warning)] The type "ResourceGroupProperties" does not contain property "val1". Available properties include "provisioningState". |val1|
output Name2_output string = naME2_resource.properties.val2
//@[55:59) [BCP053 (Warning)] The type "ResourceGroupProperties" does not contain property "val2". Available properties include "provisioningState". |val2|
