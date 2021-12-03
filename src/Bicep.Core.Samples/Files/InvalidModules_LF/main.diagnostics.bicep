module nonExistentFileRef './nonExistent.bicep' = {
//@[26:47) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (CodeDescription: none) |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[35:56) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (CodeDescription: none) |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[40:73) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (CodeDescription: none) |'abc/def/../../nonExistent.bicep'|

}

module moduleWithoutPath = {
//@[25:26) [BCP097 (Error)] Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep' (CodeDescription: none) |=|
//@[25:28) [BCP090 (Error)] This module declaration is missing a file path reference. (CodeDescription: none) |= {|
//@[28:28) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

// #completionTest(41) -> moduleBodyCompletions
module moduleWithPath './moduleb.bicep' =
//@[41:41) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

// missing identifier #completionTest(7) -> empty
module 
//@[7:7) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) ||
//@[7:7) [BCP090 (Error)] This module declaration is missing a file path reference. (CodeDescription: none) ||

// #completionTest(24,25) -> moduleObject
module missingValue '' = 
//@[20:22) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
//@[25:25) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

var interp = 'hello'
module moduleWithInterpPath './${interp}.bicep' = {
//@[28:47) [BCP092 (Error)] String interpolation is not supported in file paths. (CodeDescription: none) |'./${interp}.bicep'|

}

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[40:59) [BCP092 (Error)] String interpolation is not supported in file paths. (CodeDescription: none) |'./${interp}.bicep'|

}

module moduleWithSelfCycle './main.bicep' = {
//@[27:41) [BCP094 (Error)] This module references itself, which is not allowed. (CodeDescription: none) |'./main.bicep'|

}

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[39:53) [BCP094 (Error)] This module references itself, which is not allowed. (CodeDescription: none) |'./main.bicep'|

}

module './main.bicep' = {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|

}

module './main.bicep' = if (1 + 2 == 3) {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|

}

module './main.bicep' = if
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[26:26) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) ||

module './main.bicep' = if (
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[28:28) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

module './main.bicep' = if (true
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[32:32) [BCP018 (Error)] Expected the ")" character at this location. (CodeDescription: none) ||

module './main.bicep' = if (true)
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[33:33) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) ||

module './main.bicep' = if {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[27:28) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |{|

}

module './main.bicep' = if () {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[28:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|

}

module './main.bicep' = if ('true') {
//@[7:21) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|

}

module modANoName './modulea.bicep' = {
//@[7:17) [BCP028 (Error)] Identifier "modANoName" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |modANoName|
//@[7:17) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". (CodeDescription: none) |modANoName|
// #completionTest(0) -> moduleATopLevelProperties

}

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[7:30) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". (CodeDescription: none) |modANoNameWithCondition|
// #completionTest(0) -> moduleAWithConditionTopLevelProperties

}

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[35:49) [BCP094 (Error)] This module references itself, which is not allowed. (CodeDescription: none) |'./main.bicep'|

}

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[34:48) [BCP094 (Error)] This module references itself, which is not allowed. (CodeDescription: none) |'./main.bicep'|

}


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[7:17) [BCP028 (Error)] Identifier "modANoName" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |modANoName|
//@[44:44) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) ||
//@[51:51) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) ||

}

module modANoInputs './modulea.bicep' = {
//@[7:19) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |modANoInputs|
  name: 'modANoInputs'
//@[8:22) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'modANoInputs'|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
  
}

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[7:32) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |modANoInputsWithCondition|
  'foo'
]) == 1) {
  name: 'modANoInputs'
//@[8:22) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'modANoInputs'|
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
  
}

module modAEmptyInputs './modulea.bicep' = {
  name: 'modANoInputs'
//@[8:22) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'modANoInputs'|
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (CodeDescription: none) |params|
    // #completionTest(0,1,2,3,4) -> moduleAParams
    
  }
}

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
  name: 'modANoInputs'
//@[8:22) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'modANoInputs'|
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (CodeDescription: none) |params|
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
    
  }
}

// #completionTest(55) -> moduleATopLevelPropertyAccess
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[4:35) [no-unused-vars (Warning)] Variable "modulePropertyAccessCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |modulePropertyAccessCompletions|
//@[54:55) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". (CodeDescription: none) |o|

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[4:48) [no-unused-vars (Warning)] Variable "moduleWithConditionPropertyAccessCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |moduleWithConditionPropertyAccessCompletions|
//@[80:81) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". (CodeDescription: none) |o|

// #completionTest(56) -> moduleAOutputs
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[4:28) [no-unused-vars (Warning)] Variable "moduleOutputsCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |moduleOutputsCompletions|
//@[55:56) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (CodeDescription: none) |s|

// #completionTest(82) -> moduleAWithConditionOutputs
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[4:41) [no-unused-vars (Warning)] Variable "moduleWithConditionOutputsCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |moduleWithConditionOutputsCompletions|
//@[81:82) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (CodeDescription: none) |s|

module modAUnspecifiedInputs './modulea.bicep' = {
  name: 'modAUnspecifiedInputs'
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam". (CodeDescription: none) |params|
    stringParamB: ''
    objParam: {}
    objArray: []
//@[4:12) [BCP037 (Error)] The property "objArray" is not allowed on objects of type "params". Permissible properties include "arrayParam", "stringParamA". (CodeDescription: none) |objArray|
    unspecifiedInput: ''
//@[4:20) [BCP037 (Error)] The property "unspecifiedInput" is not allowed on objects of type "params". Permissible properties include "arrayParam", "stringParamA". (CodeDescription: none) |unspecifiedInput|
  }
}

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[4:21) [no-unused-vars (Warning)] Variable "unspecifiedOutput" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |unspecifiedOutput|
//@[54:58) [BCP053 (Error)] The type "outputs" does not contain property "test". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (CodeDescription: none) |test|

module modCycle './cycle.bicep' = {
//@[16:31) [BCP095 (Error)] The module is involved in a cycle ("${TEST_OUTPUT_DIR}/cycle.bicep" -> "${TEST_OUTPUT_DIR}/main.bicep"). (CodeDescription: none) |'./cycle.bicep'|
  
}

module moduleWithEmptyPath '' = {
//@[27:29) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
}

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[30:46) [BCP051 (Error)] The specified path begins with "/". Files must be referenced using relative paths. (CodeDescription: none) |'/abc/def.bicep'|
}

module moduleWithBackslash 'child\\file.bicep' = {
//@[27:46) [BCP098 (Error)] The specified file path contains a "\" character. Use "/" instead as the directory separator character. (CodeDescription: none) |'child\\file.bicep'|
}

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[29:48) [BCP085 (Error)] The specified file path contains one ore more invalid path characters. The following are not permitted: """, "*", ":", "<", ">", "?", "\", "|". (CodeDescription: none) |'child/fi|le.bicep'|
}

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[39:52) [BCP086 (Error)] The specified file path ends with an invalid character. The following are not permitted: " ", ".". (CodeDescription: none) |'child/test.'|
}

module moduleWithValidScope './empty.bicep' = {
  name: 'moduleWithValidScope'
}

module moduleWithInvalidScope './empty.bicep' = {
  name: 'moduleWithInvalidScope'
  scope: moduleWithValidScope
//@[9:29) [BCP134 (Error)] Scope "module" is not valid for this module. Permitted scopes: "resourceGroup". (CodeDescription: none) |moduleWithValidScope|
}

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[7:37) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "scope". (CodeDescription: none) |moduleWithMissingRequiredScope|
//@[69:113) [BCP134 (Error)] Scope "resourceGroup" is not valid for this module. Permitted scopes: "subscription". (CodeDescription: none) |{\n  name: 'moduleWithMissingRequiredScope'\n}|
  name: 'moduleWithMissingRequiredScope'
}

module moduleWithInvalidScope2 './empty.bicep' = {
  name: 'moduleWithInvalidScope2'
  scope: managementGroup()
//@[24:26) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
}

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
  name: 'moduleWithUnsupprtedScope1'
  scope: managementGroup()
//@[24:26) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
}

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
  name: 'moduleWithUnsupprtedScope2'
  scope: managementGroup('MG')
//@[9:30) [BCP134 (Error)] Scope "managementGroup" is not valid for this module. Permitted scopes: "subscription". (CodeDescription: none) |managementGroup('MG')|
}

module moduleWithBadScope './empty.bicep' = {
  name: 'moduleWithBadScope'
  scope: 'stringScope'
//@[9:22) [BCP036 (Error)] The property "scope" expected a value of type "resourceGroup" but the provided value is of type "'stringScope'". (CodeDescription: none) |'stringScope'|
}

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'runtimeValidRes1Name'
  location: 'westeurope'
//@[12:24) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'westeurope' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westeurope'|
  kind: 'Storage'
  sku: {
    name: 'Standard_GRS'
  }
}

module runtimeValidModule1 'empty.bicep' = {
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[8:89) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)|
}

module runtimeInvalidModule1 'empty.bicep' = {
  name: runtimeValidRes1.location
//@[8:33) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
}

module runtimeInvalidModule2 'empty.bicep' = {
  name: runtimeValidRes1['location']
//@[8:36) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['location']|
}

module runtimeInvalidModule3 'empty.bicep' = {
  name: runtimeValidRes1.sku.name
//@[8:28) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.sku|
}

module runtimeInvalidModule4 'empty.bicep' = {
  name: runtimeValidRes1.sku['name']
//@[8:28) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.sku|
}

module runtimeInvalidModule5 'empty.bicep' = {
  name: runtimeValidRes1['sku']['name']
//@[8:31) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['sku']|
}

module runtimeInvalidModule6 'empty.bicep' = {
  name: runtimeValidRes1['sku'].name
//@[8:31) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['sku']|
}

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[7:34) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |singleModuleForRuntimeCheck|
  name: 'test'
}

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
var moduleRuntimeCheck2 = moduleRuntimeCheck

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[7:32) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |moduleLoopForRuntimeCheck|
//@[7:32) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |moduleLoopForRuntimeCheck|
  name: moduleRuntimeCheck2
//@[8:27) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("moduleRuntimeCheck2" -> "moduleRuntimeCheck" -> "singleModuleForRuntimeCheck"). Properties of singleModuleForRuntimeCheck which can be calculated at the start include "name". (CodeDescription: none) |moduleRuntimeCheck2|
}]

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
var moduleRuntimeCheck4 = moduleRuntimeCheck3
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[7:33) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |moduleLoopForRuntimeCheck2|
//@[7:33) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |moduleLoopForRuntimeCheck2|
  name: moduleRuntimeCheck4
//@[8:27) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("moduleRuntimeCheck4" -> "moduleRuntimeCheck3" -> "moduleLoopForRuntimeCheck"). Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (CodeDescription: none) |moduleRuntimeCheck4|
}]

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[7:33) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |moduleLoopForRuntimeCheck3|
//@[7:33) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |moduleLoopForRuntimeCheck3|
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
//@[8:119) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )|
//@[15:51) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (CodeDescription: none) |moduleLoopForRuntimeCheck[1].outputs|
//@[67:103) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (CodeDescription: none) |moduleLoopForRuntimeCheck[1].outputs|
}]

module moduleWithDuplicateName1 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
//@[8:33) [BCP122 (Error)] Modules: "moduleWithDuplicateName1", "moduleWithDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'moduleWithDuplicateName'|
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
//@[8:33) [BCP122 (Error)] Modules: "moduleWithDuplicateName1", "moduleWithDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'moduleWithDuplicateName'|
}

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionB ''
//@[19:21) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
//@[21:21) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionC '' =
//@[19:21) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
//@[23:23) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionD '' = {}
//@[19:21) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionE '' = {
//@[19:21) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
  name: 'hello'
}

// #completionTest(29) -> cwdDotFileCompletions
module cwdFileCompletionA './m'
//@[26:31) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/m'. (CodeDescription: none) |'./m'|
//@[31:31) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(26, 27) -> cwdFileCompletions
module cwdFileCompletionB m
//@[26:27) [BCP097 (Error)] Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep' (CodeDescription: none) |m|
//@[26:27) [BCP090 (Error)] This module declaration is missing a file path reference. (CodeDescription: none) |m|
//@[27:27) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
module cwdFileCompletionC 'm'
//@[26:29) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/m'. (CodeDescription: none) |'m'|
//@[29:29) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childFileCompletions
module childCompletionA 'ChildModules/'
//@[24:39) [BCP091 (Error)] An error occurred reading file. Access to the path '${TEST_OUTPUT_DIR}/ChildModules/' is denied. (CodeDescription: none) |'ChildModules/'|
//@[39:39) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotFileCompletions
module childCompletionB './ChildModules/'
//@[24:41) [BCP091 (Error)] An error occurred reading file. Access to the path '${TEST_OUTPUT_DIR}/ChildModules/' is denied. (CodeDescription: none) |'./ChildModules/'|
//@[41:41) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childDotFileCompletions
module childCompletionC './ChildModules/m'
//@[24:42) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/ChildModules/m'. (CodeDescription: none) |'./ChildModules/m'|
//@[42:42) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childFileCompletions
module childCompletionD 'ChildModules/e'
//@[24:40) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/ChildModules/e'. (CodeDescription: none) |'ChildModules/e'|
//@[40:40) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

@minValue()
//@[1:9) [BCP128 (Error)] Function "minValue" cannot be used as a module decorator. (CodeDescription: none) |minValue|
module moduleWithNotAttachableDecorators './empty.bicep' = {
  name: 'moduleWithNotAttachableDecorators'
}

// loop parsing cases
module expectedForKeyword 'modulea.bicep' = []
//@[45:46) [BCP012 (Error)] Expected the "for" keyword at this location. (CodeDescription: none) |]|

module expectedForKeyword2 'modulea.bicep' = [f]
//@[46:47) [BCP012 (Error)] Expected the "for" keyword at this location. (CodeDescription: none) |f|

module expectedLoopVar 'modulea.bicep' = [for]
//@[45:45) [BCP162 (Error)] Expected a loop item variable identifier or "(" at this location. (CodeDescription: none) ||

module expectedInKeyword 'modulea.bicep' = [for x]
//@[49:50) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |]|

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[51:52) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |b|
//@[52:53) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[58:59) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

module expectedColon 'modulea.bicep' = [for x in y]
//@[49:50) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[50:51) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |]|

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[52:53) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[54:55) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |]|

// indexed loop parsing cases
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[51:52) [BCP136 (Error)] Expected a loop item variable identifier at this location. (CodeDescription: none) |)|

module expectedComma 'modulea.bicep' = [for (x)]
//@[46:47) [BCP018 (Error)] Expected the "," character at this location. (CodeDescription: none) |)|

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[54:55) [BCP163 (Error)] Expected a loop index variable identifier at this location. (CodeDescription: none) |)|

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[54:55) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |]|

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[64:65) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[54:55) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[55:56) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |]|

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[57:58) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[59:60) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |]|

// loop filter parsing cases
module expectedLoopFilterOpenParen 'modulea.bicep' = [for x in y: if]
//@[63:64) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[68:69) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |]|
module expectedLoopFilterOpenParen2 'modulea.bicep' = [for (x,y) in z: if]
//@[68:69) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[73:74) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |]|

module expectedLoopFilterPredicateAndBody 'modulea.bicep' = [for x in y: if()]
//@[70:71) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[76:77) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|
//@[77:78) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |]|
module expectedLoopFilterPredicateAndBody2 'modulea.bicep' = [for (x,y) in z: if()]
//@[75:76) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[81:82) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|
//@[82:83) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |]|

// wrong loop body type
var emptyArray = []
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[64:65) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |4|
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[69:70) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |4|

// missing loop body properties
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[7:32) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". (CodeDescription: none) |missingLoopBodyProperties|
}]
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[7:33) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". (CodeDescription: none) |missingLoopBodyProperties2|
}]

// wrong array type
var notAnArray = true
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[50:60) [BCP137 (Error)] Loop expected an expression of type "array" but the provided value is of type "bool". (CodeDescription: none) |notAnArray|
}]

// missing fewer properties
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
  name: 'hello-${x}'
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (CodeDescription: none) |params|

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
//@[4:13) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |notAThing|
  }
}]
module wrongModuleParameterInFilteredLoop 'modulea.bicep' = [for x in emptyArray: if(true) {
  // #completionTest(17) -> symbolsPlusX_if
  name: 'hello-${x}'
  params: {
    arrayParam: []
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    notAThing: 'test'
//@[4:13) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |notAThing|
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
//@[4:13) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |notAThing|
  }
}]

module paramNameCompletionsInFilteredLoops 'modulea.bicep' = [for (x,i) in emptyArray: if(true) {
  name: 'hello-${x}'
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (CodeDescription: none) |params|
    // #completionTest(0,1,2) -> moduleAParams
  
  }
}]

// #completionTest(100) -> moduleAOutputs
var propertyAccessCompletionsForFilteredModuleLoop = paramNameCompletionsInFilteredLoops[0].outputs.
//@[4:50) [no-unused-vars (Warning)] Variable "propertyAccessCompletionsForFilteredModuleLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |propertyAccessCompletionsForFilteredModuleLoop|
//@[100:100) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// nonexistent arrays and loop variables
var evenMoreDuplicates = 'there'
//@[4:22) [no-unused-vars (Warning)] Variable "evenMoreDuplicates" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |evenMoreDuplicates|
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[7:24) [BCP179 (Warning)] The loop item variable "evenMoreDuplicates" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |nonexistentArrays|
//@[70:86) [BCP057 (Error)] The name "alsoDoesNotExist" does not exist in the current context. (CodeDescription: none) |alsoDoesNotExist|
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
//@[17:55) [BCP057 (Error)] The name "whyChooseRealVariablesWhenWeCanPretend" does not exist in the current context. (CodeDescription: none) |whyChooseRealVariablesWhenWeCanPretend|
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[43:54) [BCP057 (Error)] The name "totallyFake" does not exist in the current context. (CodeDescription: none) |totallyFake|
//@[56:68) [BCP057 (Error)] The name "doesNotExist" does not exist in the current context. (CodeDescription: none) |doesNotExist|
  }
}]

output directRefToCollectionViaOutput array = nonexistentArrays
//@[46:63) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
  name: 'hello'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[23:49) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |wrongModuleParameterInLoop|
//@[51:68) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
  name: 'hello2'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[23:49) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |wrongModuleParameterInLoop|
//@[51:68) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
//@[7:39) [BCP179 (Warning)] The loop item variable "test" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |directRefToCollectionViaLoopBody|
  name: 'hello3'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[23:49) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |wrongModuleParameterInLoop|
//@[51:68) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}]

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
//@[7:57) [BCP179 (Warning)] The loop item variable "test" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |directRefToCollectionViaLoopBodyWithExtraDependsOn|
  name: 'hello4'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[23:49) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |wrongModuleParameterInLoop|
//@[51:68) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
    dependsOn: [
//@[4:13) [BCP037 (Error)] The property "dependsOn" is not allowed on objects of type "params". Permissible properties include "stringParamA". (CodeDescription: none) |dependsOn|
      nonexistentArrays
//@[6:23) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    ]
  }
  dependsOn: [
    
  ]
}]


// module body that isn't an object
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
//@[63:70) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |'hello'|
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[64:70) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |concat|
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[68:75) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |'hello'|
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[68:74) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |concat|

module anyTypeInScope 'empty.bicep' = {
//@[7:21) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInScope|
  dependsOn: [
    any('s')
//@[4:12) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('s')|
  ]

  scope: any(42)
//@[9:16) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(42)|
}

module anyTypeInScopeConditional 'empty.bicep' = if(false) {
//@[7:32) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInScopeConditional|
  dependsOn: [
    any('s')
//@[4:12) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('s')|
  ]

  scope: any(42)
//@[9:16) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(42)|
}

module anyTypeInScopeLoop 'empty.bicep' = [for thing in []: {
//@[7:25) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInScopeLoop|
  dependsOn: [
    any('s')
//@[4:12) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('s')|
  ]

  scope: any(42)
//@[9:16) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(42)|
}]

// Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'moduleb.bicep' = {
  name: 'secureModule1'
  params: {       
    stringParamA: kv.getSecret('mySecret')
//@[18:42) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
    stringParamB: '${kv.getSecret('mySecret')}'
//@[21:45) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
    objParam: kv.getSecret('mySecret')
//@[14:38) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
//@[14:38) [BCP036 (Error)] The property "objParam" expected a value of type "object" but the provided value is of type "string". (CodeDescription: none) |kv.getSecret('mySecret')|
    arrayParam: kv.getSecret('mySecret')
//@[16:40) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
//@[16:40) [BCP036 (Error)] The property "arrayParam" expected a value of type "array" but the provided value is of type "string". (CodeDescription: none) |kv.getSecret('mySecret')|
    secureStringParam: '${kv.getSecret('mySecret')}'
//@[26:50) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
    secureObjectParam: kv.getSecret('mySecret')
//@[23:47) [BCP036 (Error)] The property "secureObjectParam" expected a value of type "object" but the provided value is of type "string". (CodeDescription: none) |kv.getSecret('mySecret')|
    secureStringParam2: '${kv.getSecret('mySecret')}'
//@[4:22) [BCP037 (Error)] The property "secureStringParam2" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |secureStringParam2|
//@[27:51) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
    secureObjectParam2: kv.getSecret('mySecret')
//@[4:22) [BCP037 (Error)] The property "secureObjectParam2" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |secureObjectParam2|
//@[24:48) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
  }
}

module secureModule2 'BAD_MODULE_PATH.bicep' = {
//@[21:44) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/BAD_MODULE_PATH.bicep'. (CodeDescription: none) |'BAD_MODULE_PATH.bicep'|
  name: 'secureModule2'
  params: {       
    secret: kv.getSecret('mySecret')
  }
}

module issue3000 'empty.bicep' = {
  name: 'issue3000Module'
  params: {}
  identity: {
//@[2:10) [BCP037 (Error)] The property "identity" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |identity|
    type: 'SystemAssigned'
  }
  extendedLocation: {}
//@[2:18) [BCP037 (Error)] The property "extendedLocation" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |extendedLocation|
  sku: {}
//@[2:5) [BCP037 (Error)] The property "sku" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |sku|
  kind: 'V1'
//@[2:6) [BCP037 (Error)] The property "kind" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |kind|
  managedBy: 'string'
//@[2:11) [BCP037 (Error)] The property "managedBy" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |managedBy|
  mangedByExtended: [
//@[2:18) [BCP037 (Error)] The property "mangedByExtended" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |mangedByExtended|
   'str1'
   'str2'
  ]
  zones: [
//@[2:7) [BCP037 (Error)] The property "zones" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |zones|
   'str1'
   'str2'
  ]
  plan: {}
//@[2:6) [BCP037 (Error)] The property "plan" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |plan|
  eTag: ''
//@[2:6) [BCP037 (Error)] The property "eTag" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |eTag|
  scale: {}  
//@[2:7) [BCP037 (Error)] The property "scale" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |scale|
}

module invalidJsonMod 'modulec.json' = {
//@[7:21) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name". (CodeDescription: none) |invalidJsonMod|
//@[22:36) [BCP188 (Error)] The referenced ARM template has errors. Please see https://aka.ms/arm-template for information on how to diagnose and fix the template. (CodeDescription: none) |'modulec.json'|
}

module jsonModMissingParam 'moduled.json' = {
  name: 'jsonModMissingParam'
  params: {
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "bar". (CodeDescription: none) |params|
    foo: 123
//@[9:12) [BCP036 (Error)] The property "foo" expected a value of type "string" but the provided value is of type "int". (CodeDescription: none) |123|
    baz: 'C'
//@[9:12) [BCP088 (Error)] The property "baz" expected a value of type "'A' | 'B'" but the provided value is of type "'C'". Did you mean "'A'"? (CodeDescription: none) |'C'|
  }
}

