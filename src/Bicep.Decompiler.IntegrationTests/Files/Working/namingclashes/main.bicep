targetScope = 'subscription'
param name1 string
param nAmE2 string

var name1_var = name1
//@[04:13) [decompiler-cleanup (Warning)] The name of variable 'name1_var' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality). (bicep core linter https://aka.ms/bicep/linter-diagnostics#decompiler-cleanup) |name1_var|
var NamE2_var = nAmE2
//@[04:13) [decompiler-cleanup (Warning)] The name of variable 'NamE2_var' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality). (bicep core linter https://aka.ms/bicep/linter-diagnostics#decompiler-cleanup) |NamE2_var|

resource name1_resource 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'name1'
//@[08:15) [decompiler-cleanup (Warning)] The symbolic name of resource 'name1_resource' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality). (bicep core linter https://aka.ms/bicep/linter-diagnostics#decompiler-cleanup) |'name1'|
  location: 'West US'
  properties: {
    val1: name1_var
//@[04:08) [BCP037 (Warning)] The property "val1" is not allowed on objects of type "ResourceGroupProperties". No other properties are allowed. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues. (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |val1|
  }
}

resource naME2_resource 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'naME2'
//@[08:15) [decompiler-cleanup (Warning)] The symbolic name of resource 'naME2_resource' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality). (bicep core linter https://aka.ms/bicep/linter-diagnostics#decompiler-cleanup) |'naME2'|
  location: 'West US'
  properties: {
    val2: NamE2_var
//@[04:08) [BCP037 (Warning)] The property "val2" is not allowed on objects of type "ResourceGroupProperties". No other properties are allowed. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues. (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |val2|
  }
}

output name1 string = name1_resource.properties.val1
//@[48:52) [BCP053 (Warning)] The type "ResourceGroupProperties" does not contain property "val1". Available properties include "provisioningState". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |val1|
output Name2 string = naME2_resource.properties.val2
//@[48:52) [BCP053 (Warning)] The type "ResourceGroupProperties" does not contain property "val2". Available properties include "provisioningState". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |val2|

