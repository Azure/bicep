module nonExistentFileRef './nonExistent.bicep' = {
//@[26:47) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}nonExistent.bicep'. |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[35:56) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}nonExistent.bicep'. |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[40:73) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}nonExistent.bicep'. |'abc/def/../../nonExistent.bicep'|

}

module moduleWithoutPath = {
//@[25:26) [BCP097 (Error)] Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep' |=|
//@[25:28) [BCP090 (Error)] This module declaration is missing a file path reference. |= {|
//@[28:28) [BCP018 (Error)] Expected the "=" character at this location. ||

}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |}|

// missing identifier #completionTest(7) -> empty
module 
//@[7:7) [BCP096 (Error)] Expected a module identifier at this location. ||
//@[7:7) [BCP090 (Error)] This module declaration is missing a file path reference. ||

// #completionTest(24,25) -> object
module missingValue '' = 
//@[20:22) [BCP050 (Error)] The specified module path is empty. |''|
//@[25:25) [BCP018 (Error)] Expected the "{" character at this location. ||

var interp = 'hello'
module moduleWithInterpPath './${interp}.bicep' = {
//@[28:47) [BCP092 (Error)] String interpolation is not supported in module paths. |'./${interp}.bicep'|

}

module moduleWithSelfCycle './main.bicep' = {
//@[27:41) [BCP094 (Error)] This module references itself, which is not allowed. |'./main.bicep'|

}

module './main.bicep' = {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|

}

module modANoName './modulea.bicep' = {
//@[7:17) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". |modANoName|
// #completionTest(0) -> moduleATopLevelProperties

}

module modANoInputs './modulea.bicep' = {
//@[7:19) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". |modANoInputs|
  name: 'modANoInputs'
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
  
}

module modAEmptyInputs './modulea.bicep' = {
  name: 'modANoInputs'
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". |params|
    // #completionTest(0,1,2,3,4) -> moduleAParams
    
  }
}

// #completionTest(55) -> moduleATopLevelPropertyAccess
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[54:55) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". |o|

// #completionTest(56) -> moduleAOutputs
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[55:56) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". |s|

module modAUnspecifiedInputs './modulea.bicep' = {
  name: 'modAUnspecifiedInputs'
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam". |params|
    stringParamB: ''
    objParam: {}
    objArray: []
//@[4:12) [BCP038 (Error)] The property "objArray" is not allowed on objects of type "params". Permissible properties include "arrayParam", "stringParamA". |objArray|
    unspecifiedInput: ''
//@[4:20) [BCP038 (Error)] The property "unspecifiedInput" is not allowed on objects of type "params". Permissible properties include "arrayParam", "stringParamA". |unspecifiedInput|
  }
}

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[54:58) [BCP053 (Error)] The type "outputs" does not contain property "test". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". |test|

module modCycle './cycle.bicep' = {
//@[16:31) [BCP095 (Error)] The module is involved in a cycle ("${TEST_OUTPUT_DIR}cycle.bicep" -> "${TEST_OUTPUT_DIR}main.bicep"). |'./cycle.bicep'|
  
}

module moduleWithEmptyPath '' = {
//@[27:29) [BCP050 (Error)] The specified module path is empty. |''|
}

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[30:46) [BCP051 (Error)] The specified module path begins with "/". Module files must be referenced using relative paths. |'/abc/def.bicep'|
}

module moduleWithBackslash 'child\\file.bicep' = {
//@[27:46) [BCP098 (Error)] The specified module path contains a "\" character. Use "/" instead as the directory separator character. |'child\\file.bicep'|
}

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[29:48) [BCP085 (Error)] The specified module path contains one ore more invalid path characters. The following are not permitted: """, "*", ":", "<", ">", "?", "\", "|". |'child/fi|le.bicep'|
}

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[39:52) [BCP086 (Error)] The specified module path ends with an invalid character. The following are not permitted: " ", ".". |'child/test.'|
}

module moduleWithValidScope './empty.bicep' = {
  name: 'moduleWithValidScope'
}

module moduleWithInvalidScope './empty.bicep' = {
  name: 'moduleWithInvalidScope'
  scope: moduleWithValidScope
//@[9:29) [BCP036 (Error)] The property "scope" expected a value of type "resourceGroup" but the provided value is of type "module". |moduleWithValidScope|
//@[9:29) [BCP116 (Error)] Unsupported scope for module deployment in a "resourceGroup" target scope. Omit this property to inherit the current scope, or specify a valid scope. Permissible scopes include current resource group: resourceGroup(), named resource group in same subscription: resourceGroup(<name>), named resource group in a different subscription: resourceGroup(<subId>, <name>), or tenant: tenant(). |moduleWithValidScope|
}

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[7:37) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "scope". |moduleWithMissingRequiredScope|
  name: 'moduleWithMissingRequiredScope'
}

module moduleWithInvalidScope2 './empty.bicep' = {
  name: 'moduleWithInvalidScope2'
  scope: managementGroup()
//@[9:24) [BCP057 (Error)] The name "managementGroup" does not exist in the current context. |managementGroup|
//@[9:26) [BCP116 (Error)] Unsupported scope for module deployment in a "resourceGroup" target scope. Omit this property to inherit the current scope, or specify a valid scope. Permissible scopes include current resource group: resourceGroup(), named resource group in same subscription: resourceGroup(<name>), named resource group in a different subscription: resourceGroup(<subId>, <name>), or tenant: tenant(). |managementGroup()|
}

module moduleWithBadScope './empty.bicep' = {
  name: 'moduleWithBadScope'
  scope: 'stringScope'
//@[9:22) [BCP036 (Error)] The property "scope" expected a value of type "resourceGroup" but the provided value is of type "'stringScope'". |'stringScope'|
//@[9:22) [BCP116 (Error)] Unsupported scope for module deployment in a "resourceGroup" target scope. Omit this property to inherit the current scope, or specify a valid scope. Permissible scopes include current resource group: resourceGroup(), named resource group in same subscription: resourceGroup(<name>), named resource group in a different subscription: resourceGroup(<subId>, <name>), or tenant: tenant(). |'stringScope'|
}

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'runtimeValidRes1Name'
  location: 'westeurope'
  kind: 'Storage'
  sku: {
    name: 'Standard_GRS'
  }
}

module runtimeValidModule1 'empty.bicep' = {
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
}

module runtimeInvalidModule1 'empty.bicep' = {
  name: runtimeValidRes1.location
//@[8:33) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1.location|
}

module runtimeInvalidModule2 'empty.bicep' = {
  name: runtimeValidRes1['location']
//@[8:36) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1['location']|
}

module runtimeInvalidModule3 'empty.bicep' = {
  name: runtimeValidRes1.sku.name
//@[8:33) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1.sku.name|
}

module runtimeInvalidModule4 'empty.bicep' = {
  name: runtimeValidRes1.sku['name']
//@[8:36) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1.sku['name']|
}

module runtimeInvalidModule5 'empty.bicep' = {
  name: runtimeValidRes1['sku']['name']
//@[8:39) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1['sku']['name']|
}

module runtimeInvalidModule6 'empty.bicep' = {
  name: runtimeValidRes1['sku'].name
//@[8:36) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1['sku'].name|
}
