module nonExistentFileRef './nonExistent.bicep' = {
//@[26:47) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[35:56) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[40:73) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. |'abc/def/../../nonExistent.bicep'|

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
//@[25:25) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. ||

var interp = 'hello'
module moduleWithInterpPath './${interp}.bicep' = {
//@[28:47) [BCP092 (Error)] String interpolation is not supported in module paths. |'./${interp}.bicep'|

}

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[40:59) [BCP092 (Error)] String interpolation is not supported in module paths. |'./${interp}.bicep'|

}

module moduleWithSelfCycle './main.bicep' = {
//@[27:41) [BCP094 (Error)] This module references itself, which is not allowed. |'./main.bicep'|

}

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[39:53) [BCP094 (Error)] This module references itself, which is not allowed. |'./main.bicep'|

}

module './main.bicep' = {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|

}

module './main.bicep' = if (1 + 2 == 3) {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|

}

module './main.bicep' = if
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|
//@[26:26) [BCP018 (Error)] Expected the "(" character at this location. ||

module './main.bicep' = if (
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|
//@[28:28) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

module './main.bicep' = if (true
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|
//@[32:32) [BCP018 (Error)] Expected the ")" character at this location. ||

module './main.bicep' = if (true)
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|
//@[33:33) [BCP018 (Error)] Expected the "{" character at this location. ||

module './main.bicep' = if {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|
//@[27:28) [BCP018 (Error)] Expected the "(" character at this location. |{|

}

module './main.bicep' = if () {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|
//@[28:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |)|

}

module './main.bicep' = if ('true') {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. |'./main.bicep'|

}

module modANoName './modulea.bicep' = {
//@[7:17) [BCP028 (Error)] Identifier "modANoName" is declared multiple times. Remove or rename the duplicates. |modANoName|
//@[7:17) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". |modANoName|
// #completionTest(0) -> moduleATopLevelProperties

}

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[7:30) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". |modANoNameWithCondition|
// #completionTest(0) -> moduleAWithConditionTopLevelProperties

}

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[35:49) [BCP094 (Error)] This module references itself, which is not allowed. |'./main.bicep'|

}

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[34:48) [BCP094 (Error)] This module references itself, which is not allowed. |'./main.bicep'|

}


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[7:17) [BCP028 (Error)] Identifier "modANoName" is declared multiple times. Remove or rename the duplicates. |modANoName|
//@[51:52) [BCP019 (Error)] Expected a new line character at this location. |}|

}
//@[1:1) [BCP018 (Error)] Expected the ")" character at this location. ||

module modANoInputs './modulea.bicep' = {
//@[7:19) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". |modANoInputs|
  name: 'modANoInputs'
//@[8:22) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'modANoInputs'|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
  
}

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[7:32) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". |modANoInputsWithCondition|
  'foo'
]) == 1) {
  name: 'modANoInputs'
//@[8:22) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'modANoInputs'|
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
  
}

module modAEmptyInputs './modulea.bicep' = {
  name: 'modANoInputs'
//@[8:22) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'modANoInputs'|
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". |params|
    // #completionTest(0,1,2,3,4) -> moduleAParams
    
  }
}

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
  name: 'modANoInputs'
//@[8:22) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'modANoInputs'|
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". |params|
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
    
  }
}

// #completionTest(55) -> moduleATopLevelPropertyAccess
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[54:55) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". |o|

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[80:81) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". |o|

// #completionTest(56) -> moduleAOutputs
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[55:56) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". |s|

// #completionTest(82) -> moduleAWithConditionOutputs
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[81:82) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". |s|

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
//@[16:31) [BCP095 (Error)] The module is involved in a cycle ("${TEST_OUTPUT_DIR}/cycle.bicep" -> "${TEST_OUTPUT_DIR}/main.bicep"). |'./cycle.bicep'|
  
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
}

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[7:37) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "scope". |moduleWithMissingRequiredScope|
//@[69:113) [BCP134 (Error)] Scope "resourceGroup" is not valid for this module. Permitted scopes: "subscription". |{\n  name: 'moduleWithMissingRequiredScope'\n}|
  name: 'moduleWithMissingRequiredScope'
}

module moduleWithInvalidScope2 './empty.bicep' = {
  name: 'moduleWithInvalidScope2'
  scope: managementGroup()
//@[24:26) [BCP071 (Error)] Expected 1 argument, but got 0. |()|
}

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
  name: 'moduleWithUnsupprtedScope1'
  scope: managementGroup()
//@[24:26) [BCP071 (Error)] Expected 1 argument, but got 0. |()|
}

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
  name: 'moduleWithUnsupprtedScope2'
  scope: managementGroup('MG')
//@[9:30) [BCP134 (Error)] Scope "managementGroup" is not valid for this module. Permitted scopes: "subscription". |managementGroup('MG')|
}

module moduleWithBadScope './empty.bicep' = {
  name: 'moduleWithBadScope'
  scope: 'stringScope'
//@[9:22) [BCP036 (Error)] The property "scope" expected a value of type "resourceGroup" but the provided value is of type "'stringScope'". |'stringScope'|
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
//@[8:33) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of runtimeValidRes1 are "apiVersion", "id", "name", "type". |runtimeValidRes1.location|
}

module runtimeInvalidModule2 'empty.bicep' = {
  name: runtimeValidRes1['location']
//@[8:36) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of runtimeValidRes1 are "apiVersion", "id", "name", "type". |runtimeValidRes1['location']|
}

module runtimeInvalidModule3 'empty.bicep' = {
  name: runtimeValidRes1.sku.name
//@[8:33) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of runtimeValidRes1 are "apiVersion", "id", "name", "type". |runtimeValidRes1.sku.name|
}

module runtimeInvalidModule4 'empty.bicep' = {
  name: runtimeValidRes1.sku['name']
//@[8:36) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of runtimeValidRes1 are "apiVersion", "id", "name", "type". |runtimeValidRes1.sku['name']|
}

module runtimeInvalidModule5 'empty.bicep' = {
  name: runtimeValidRes1['sku']['name']
//@[8:39) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of runtimeValidRes1 are "apiVersion", "id", "name", "type". |runtimeValidRes1['sku']['name']|
}

module runtimeInvalidModule6 'empty.bicep' = {
  name: runtimeValidRes1['sku'].name
//@[8:36) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of runtimeValidRes1 are "apiVersion", "id", "name", "type". |runtimeValidRes1['sku'].name|
}

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[7:34) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". |singleModuleForRuntimeCheck|
  name: 'test'
}

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
var moduleRuntimeCheck2 = moduleRuntimeCheck

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[7:32) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". |moduleLoopForRuntimeCheck|
  name: moduleRuntimeCheck2
//@[8:27) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. You are referencing a variable which cannot be calculated in time ("moduleRuntimeCheck2" -> "moduleRuntimeCheck" -> "singleModuleForRuntimeCheck"). Accessible properties of singleModuleForRuntimeCheck are "name", "scope". |moduleRuntimeCheck2|
}]

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
var moduleRuntimeCheck4 = moduleRuntimeCheck3
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[7:33) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". |moduleLoopForRuntimeCheck2|
  name: moduleRuntimeCheck4
//@[8:27) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. You are referencing a variable which cannot be calculated in time ("moduleRuntimeCheck4" -> "moduleRuntimeCheck3" -> "moduleLoopForRuntimeCheck"). Accessible properties of moduleLoopForRuntimeCheck are "name", "scope". |moduleRuntimeCheck4|
}]

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[7:33) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". |moduleLoopForRuntimeCheck3|
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
//@[15:65) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of moduleLoopForRuntimeCheck are "name", "scope". |moduleLoopForRuntimeCheck[1].outputs.stringOutputB|
//@[67:117) [BCP120 (Error)] The property "name" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of moduleLoopForRuntimeCheck are "name", "scope". |moduleLoopForRuntimeCheck[1].outputs.stringOutputA|
}]

module moduleWithDuplicateName1 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
//@[8:33) [BCP122 (Error)] Modules: "moduleWithDuplicateName1", "moduleWithDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'moduleWithDuplicateName'|
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
//@[8:33) [BCP122 (Error)] Modules: "moduleWithDuplicateName1", "moduleWithDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'moduleWithDuplicateName'|
}

// #completionTest(19, 20, 21) -> cwdCompletions
module completionB ''
//@[19:21) [BCP050 (Error)] The specified module path is empty. |''|
//@[21:21) [BCP018 (Error)] Expected the "=" character at this location. ||

// #completionTest(19, 20, 21) -> cwdCompletions
module completionC '' =
//@[19:21) [BCP050 (Error)] The specified module path is empty. |''|
//@[23:23) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. ||

// #completionTest(19, 20, 21) -> cwdCompletions
module completionD '' = {}
//@[19:21) [BCP050 (Error)] The specified module path is empty. |''|

// #completionTest(19, 20, 21) -> cwdCompletions
module completionE '' = {
//@[19:21) [BCP050 (Error)] The specified module path is empty. |''|
  name: 'hello'
}

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
module cwdFileCompletionA '.'
//@[26:29) [BCP086 (Error)] The specified module path ends with an invalid character. The following are not permitted: " ", ".". |'.'|
//@[29:29) [BCP018 (Error)] Expected the "=" character at this location. ||

// #completionTest(26, 27) -> cwdMCompletions
module cwdFileCompletionB m
//@[26:27) [BCP097 (Error)] Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep' |m|
//@[26:27) [BCP090 (Error)] This module declaration is missing a file path reference. |m|
//@[27:27) [BCP018 (Error)] Expected the "=" character at this location. ||

// #completionTest(26, 27, 28, 29) -> cwdMCompletions
module cwdFileCompletionC 'm'
//@[26:29) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/m'. |'m'|
//@[29:29) [BCP018 (Error)] Expected the "=" character at this location. ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childCompletions
module childCompletionA 'ChildModules/'
//@[24:39) [BCP091 (Error)] An error occurred reading file. Access to the path '${TEST_OUTPUT_DIR}/ChildModules/' is denied. |'ChildModules/'|
//@[39:39) [BCP018 (Error)] Expected the "=" character at this location. ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotCompletions
module childCompletionB './ChildModules/'
//@[24:41) [BCP091 (Error)] An error occurred reading file. Access to the path '${TEST_OUTPUT_DIR}/ChildModules/' is denied. |'./ChildModules/'|
//@[41:41) [BCP018 (Error)] Expected the "=" character at this location. ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childMCompletions
module childCompletionC './ChildModules/m'
//@[24:42) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/ChildModules/m'. |'./ChildModules/m'|
//@[42:42) [BCP018 (Error)] Expected the "=" character at this location. ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childECompletions
module childCompletionD 'ChildModules/e'
//@[24:40) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/ChildModules/e'. |'ChildModules/e'|
//@[40:40) [BCP018 (Error)] Expected the "=" character at this location. ||

@minValue()
//@[1:9) [BCP128 (Error)] Function "minValue" cannot be used as a module decorator. |minValue|
module moduleWithNotAttachableDecorators './empty.bicep' = {
  name: 'moduleWithNotAttachableDecorators'
}

// loop parsing cases
module expectedForKeyword 'modulea.bicep' = []
//@[45:46) [BCP012 (Error)] Expected the "for" keyword at this location. |]|

module expectedForKeyword2 'modulea.bicep' = [f]
//@[46:47) [BCP012 (Error)] Expected the "for" keyword at this location. |f|

module expectedLoopVar 'modulea.bicep' = [for]
//@[45:45) [BCP162 (Error)] Expected a loop item variable identifier or "(" at this location. ||

module expectedInKeyword 'modulea.bicep' = [for x]
//@[49:50) [BCP012 (Error)] Expected the "in" keyword at this location. |]|

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[51:52) [BCP012 (Error)] Expected the "in" keyword at this location. |b|
//@[52:53) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |]|

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[58:59) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |]|

module expectedColon 'modulea.bicep' = [for x in y]
//@[49:50) [BCP057 (Error)] The name "y" does not exist in the current context. |y|
//@[50:51) [BCP018 (Error)] Expected the ":" character at this location. |]|

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[52:53) [BCP057 (Error)] The name "y" does not exist in the current context. |y|
//@[54:55) [BCP018 (Error)] Expected the "{" character at this location. |]|

// indexed loop parsing cases
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[51:52) [BCP136 (Error)] Expected a loop item variable identifier at this location. |)|

module expectedComma 'modulea.bicep' = [for (x)]
//@[46:47) [BCP018 (Error)] Expected the "," character at this location. |)|

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[54:55) [BCP163 (Error)] Expected a loop index variable identifier at this location. |)|

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[54:55) [BCP012 (Error)] Expected the "in" keyword at this location. |]|

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[64:65) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |]|

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[54:55) [BCP057 (Error)] The name "z" does not exist in the current context. |z|
//@[55:56) [BCP018 (Error)] Expected the ":" character at this location. |]|

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[57:58) [BCP057 (Error)] The name "z" does not exist in the current context. |z|
//@[59:60) [BCP018 (Error)] Expected the "{" character at this location. |]|

// wrong loop body type
var emptyArray = []
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[64:65) [BCP018 (Error)] Expected the "{" character at this location. |4|
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[69:70) [BCP018 (Error)] Expected the "{" character at this location. |4|

// missing loop body properties
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[7:32) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". |missingLoopBodyProperties|
}]
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[7:33) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". |missingLoopBodyProperties2|
}]

// wrong array type
var notAnArray = true
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[50:60) [BCP137 (Error)] Loop expected an expression of type "array" but the provided value is of type "bool". |notAnArray|
}]

// missing fewer properties
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
  name: 'hello-${x}'
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". |params|

  }
}]

// wrong parameter in the module loop
module wrongModuleParameterInLoop 'modulea.bicep' = [for x in emptyArray:{
  // #completionTest(17) -> symbolsPlusX
  name: 'hello-${x}'
  params: {
    arrayParam: []
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    notAThing: 'test'
//@[4:13) [BCP037 (Error)] No other properties are allowed on objects of type "params". |notAThing|
  }
}]
module wrongModuleParameterInLoop2 'modulea.bicep' = [for (x,i) in emptyArray:{
  name: 'hello-${x}'
  params: {
    arrayParam: [
      i
    ]
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    notAThing: 'test'
//@[4:13) [BCP037 (Error)] No other properties are allowed on objects of type "params". |notAThing|
  }
}]

// nonexistent arrays and loop variables
var evenMoreDuplicates = 'there'
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[70:86) [BCP057 (Error)] The name "alsoDoesNotExist" does not exist in the current context. |alsoDoesNotExist|
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
//@[17:55) [BCP057 (Error)] The name "whyChooseRealVariablesWhenWeCanPretend" does not exist in the current context. |whyChooseRealVariablesWhenWeCanPretend|
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[43:54) [BCP057 (Error)] The name "totallyFake" does not exist in the current context. |totallyFake|
//@[56:68) [BCP057 (Error)] The name "doesNotExist" does not exist in the current context. |doesNotExist|
  }
}]

output directRefToCollectionViaOutput array = nonexistentArrays
//@[46:63) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |nonexistentArrays|

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
  name: 'hello'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[23:49) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |wrongModuleParameterInLoop|
//@[51:68) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
  name: 'hello2'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[23:49) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |wrongModuleParameterInLoop|
//@[51:68) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
  name: 'hello3'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[23:49) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |wrongModuleParameterInLoop|
//@[51:68) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}]

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
  name: 'hello4'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[23:49) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |wrongModuleParameterInLoop|
//@[51:68) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |nonexistentArrays|
    objParam: {}
    stringParamB: ''
    dependsOn: [
//@[4:13) [BCP038 (Error)] The property "dependsOn" is not allowed on objects of type "params". Permissible properties include "stringParamA". |dependsOn|
      nonexistentArrays
//@[6:23) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. |nonexistentArrays|
    ]
  }
  dependsOn: [
    
  ]
}]


// module body that isn't an object
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
//@[63:70) [BCP018 (Error)] Expected the "{" character at this location. |'hello'|
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[64:70) [BCP018 (Error)] Expected the "{" character at this location. |concat|
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[68:75) [BCP018 (Error)] Expected the "{" character at this location. |'hello'|
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[68:74) [BCP018 (Error)] Expected the "{" character at this location. |concat|

