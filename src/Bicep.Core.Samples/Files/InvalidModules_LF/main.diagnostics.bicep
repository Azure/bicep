module nonExistentFileRef './nonExistent.bicep' = {
//@[026:047) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (CodeDescription: none) |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[035:056) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (CodeDescription: none) |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[040:073) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (CodeDescription: none) |'abc/def/../../nonExistent.bicep'|

}

module moduleWithoutPath = {
//@[025:026) [BCP097 (Error)] Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep' (CodeDescription: none) |=|
//@[025:028) [BCP090 (Error)] This module declaration is missing a file path reference. (CodeDescription: none) |= {|
//@[028:028) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

}
//@[000:001) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

// #completionTest(41) -> moduleBodyCompletions
module moduleWithPath './moduleb.bicep' =
//@[041:041) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

// missing identifier #completionTest(7) -> empty
module 
//@[007:007) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) ||
//@[007:007) [BCP090 (Error)] This module declaration is missing a file path reference. (CodeDescription: none) ||

// #completionTest(24,25) -> moduleObject
module missingValue '' = 
//@[020:022) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
//@[025:025) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

var interp = 'hello'
module moduleWithInterpPath './${interp}.bicep' = {
//@[028:047) [BCP092 (Error)] String interpolation is not supported in file paths. (CodeDescription: none) |'./${interp}.bicep'|

}

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[040:059) [BCP092 (Error)] String interpolation is not supported in file paths. (CodeDescription: none) |'./${interp}.bicep'|

}

module moduleWithSelfCycle './main.bicep' = {
//@[027:041) [BCP094 (Error)] This module references itself, which is not allowed. (CodeDescription: none) |'./main.bicep'|

}

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[039:053) [BCP094 (Error)] This module references itself, which is not allowed. (CodeDescription: none) |'./main.bicep'|

}

module './main.bicep' = {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|

}

module './main.bicep' = if (1 + 2 == 3) {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|

}

module './main.bicep' = if
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[026:026) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) ||

module './main.bicep' = if (
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[028:028) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

module './main.bicep' = if (true
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[032:032) [BCP018 (Error)] Expected the ")" character at this location. (CodeDescription: none) ||

module './main.bicep' = if (true)
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[033:033) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) ||

module './main.bicep' = if {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[027:028) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |{|

}

module './main.bicep' = if () {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|
//@[028:028) [BCP243 (Error)] Parentheses must contain exactly one expression. (CodeDescription: none) ||

}

module './main.bicep' = if ('true') {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (CodeDescription: none) |'./main.bicep'|

}

module modANoName './modulea.bicep' = {
//@[007:017) [BCP028 (Error)] Identifier "modANoName" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |modANoName|
//@[007:017) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". (CodeDescription: none) |modANoName|
// #completionTest(0) -> moduleATopLevelProperties

}

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[007:030) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". (CodeDescription: none) |modANoNameWithCondition|
// #completionTest(0) -> moduleAWithConditionTopLevelProperties

}

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[035:049) [BCP094 (Error)] This module references itself, which is not allowed. (CodeDescription: none) |'./main.bicep'|

}

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[034:048) [BCP094 (Error)] This module references itself, which is not allowed. (CodeDescription: none) |'./main.bicep'|

}


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[007:017) [BCP028 (Error)] Identifier "modANoName" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |modANoName|
//@[044:047) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'a'|

}

module modANoInputs './modulea.bicep' = {
//@[007:019) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |modANoInputs|
  name: 'modANoInputs'
//@[008:022) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'modANoInputs'|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
  
}

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[007:032) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |modANoInputsWithCondition|
  'foo'
]) == 1) {
  name: 'modANoInputs'
//@[008:022) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'modANoInputs'|
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
  
}

module modAEmptyInputs './modulea.bicep' = {
  name: 'modANoInputs'
//@[008:022) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'modANoInputs'|
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (CodeDescription: none) |params|
    // #completionTest(0,1,2,3,4) -> moduleAParams
    
  }
}

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
  name: 'modANoInputs'
//@[008:022) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs", "modAEmptyInputsWithCondition" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'modANoInputs'|
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (CodeDescription: none) |params|
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
    
  }
}

// #completionTest(55) -> moduleATopLevelPropertyAccess
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[004:035) [no-unused-vars (Warning)] Variable "modulePropertyAccessCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |modulePropertyAccessCompletions|
//@[054:055) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". (CodeDescription: none) |o|

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[004:048) [no-unused-vars (Warning)] Variable "moduleWithConditionPropertyAccessCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |moduleWithConditionPropertyAccessCompletions|
//@[080:081) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". (CodeDescription: none) |o|

// #completionTest(56) -> moduleAOutputs
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[004:028) [no-unused-vars (Warning)] Variable "moduleOutputsCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |moduleOutputsCompletions|
//@[055:056) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (CodeDescription: none) |s|

// #completionTest(82) -> moduleAWithConditionOutputs
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[004:041) [no-unused-vars (Warning)] Variable "moduleWithConditionOutputsCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |moduleWithConditionOutputsCompletions|
//@[081:082) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (CodeDescription: none) |s|

module modAUnspecifiedInputs './modulea.bicep' = {
  name: 'modAUnspecifiedInputs'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam". (CodeDescription: none) |params|
    stringParamB: ''
    objParam: {}
    objArray: []
//@[004:012) [BCP037 (Error)] The property "objArray" is not allowed on objects of type "params". Permissible properties include "arrayParam", "stringParamA". (CodeDescription: none) |objArray|
    unspecifiedInput: ''
//@[004:020) [BCP037 (Error)] The property "unspecifiedInput" is not allowed on objects of type "params". Permissible properties include "arrayParam", "stringParamA". (CodeDescription: none) |unspecifiedInput|
  }
}

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[004:021) [no-unused-vars (Warning)] Variable "unspecifiedOutput" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |unspecifiedOutput|
//@[054:058) [BCP053 (Error)] The type "outputs" does not contain property "test". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (CodeDescription: none) |test|

module modCycle './cycle.bicep' = {
//@[016:031) [BCP095 (Error)] The module is involved in a cycle ("${TEST_OUTPUT_DIR}/cycle.bicep" -> "${TEST_OUTPUT_DIR}/main.bicep"). (CodeDescription: none) |'./cycle.bicep'|
  
}

module moduleWithEmptyPath '' = {
//@[027:029) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
}

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[030:046) [BCP051 (Error)] The specified path begins with "/". Files must be referenced using relative paths. (CodeDescription: none) |'/abc/def.bicep'|
}

module moduleWithBackslash 'child\\file.bicep' = {
//@[027:046) [BCP098 (Error)] The specified file path contains a "\" character. Use "/" instead as the directory separator character. (CodeDescription: none) |'child\\file.bicep'|
}

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[029:048) [BCP085 (Error)] The specified file path contains one ore more invalid path characters. The following are not permitted: """, "*", ":", "<", ">", "?", "\", "|". (CodeDescription: none) |'child/fi|le.bicep'|
}

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[039:052) [BCP086 (Error)] The specified file path ends with an invalid character. The following are not permitted: " ", ".". (CodeDescription: none) |'child/test.'|
}

module moduleWithValidScope './empty.bicep' = {
  name: 'moduleWithValidScope'
}

module moduleWithInvalidScope './empty.bicep' = {
  name: 'moduleWithInvalidScope'
  scope: moduleWithValidScope
//@[009:029) [BCP134 (Error)] Scope "module" is not valid for this module. Permitted scopes: "resourceGroup". (CodeDescription: none) |moduleWithValidScope|
}

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[007:037) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "scope". (CodeDescription: none) |moduleWithMissingRequiredScope|
//@[069:113) [BCP134 (Error)] Scope "resourceGroup" is not valid for this module. Permitted scopes: "subscription". (CodeDescription: none) |{\n  name: 'moduleWithMissingRequiredScope'\n}|
  name: 'moduleWithMissingRequiredScope'
}

module moduleWithInvalidScope2 './empty.bicep' = {
  name: 'moduleWithInvalidScope2'
  scope: managementGroup()
//@[024:026) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
}

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
  name: 'moduleWithUnsupprtedScope1'
  scope: managementGroup()
//@[024:026) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
}

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
  name: 'moduleWithUnsupprtedScope2'
  scope: managementGroup('MG')
//@[009:030) [BCP134 (Error)] Scope "managementGroup" is not valid for this module. Permitted scopes: "subscription". (CodeDescription: none) |managementGroup('MG')|
}

module moduleWithBadScope './empty.bicep' = {
  name: 'moduleWithBadScope'
  scope: 'stringScope'
//@[009:022) [BCP036 (Error)] The property "scope" expected a value of type "resourceGroup" but the provided value is of type "'stringScope'". (CodeDescription: none) |'stringScope'|
}

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'runtimeValidRes1Name'
  location: 'westeurope'
//@[012:024) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westeurope' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westeurope'|
  kind: 'Storage'
  sku: {
    name: 'Standard_GRS'
  }
}

module runtimeValidModule1 'empty.bicep' = {
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[008:089) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)|
}

module runtimeInvalidModule1 'empty.bicep' = {
  name: runtimeValidRes1.location
//@[008:033) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
}

module runtimeInvalidModule2 'empty.bicep' = {
  name: runtimeValidRes1['location']
//@[008:036) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['location']|
//@[024:036) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['location']|
}

module runtimeInvalidModule3 'empty.bicep' = {
  name: runtimeValidRes1.sku.name
//@[008:028) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.sku|
}

module runtimeInvalidModule4 'empty.bicep' = {
  name: runtimeValidRes1.sku['name']
//@[008:028) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.sku|
//@[028:036) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['name']|
}

module runtimeInvalidModule5 'empty.bicep' = {
  name: runtimeValidRes1['sku']['name']
//@[008:031) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['sku']|
//@[024:031) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['sku']|
//@[031:039) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['name']|
}

module runtimeInvalidModule6 'empty.bicep' = {
  name: runtimeValidRes1['sku'].name
//@[008:031) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['sku']|
//@[024:031) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['sku']|
}

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[007:034) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |singleModuleForRuntimeCheck|
  name: 'test'
}

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
var moduleRuntimeCheck2 = moduleRuntimeCheck

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[007:032) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |moduleLoopForRuntimeCheck|
//@[007:032) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |moduleLoopForRuntimeCheck|
  name: moduleRuntimeCheck2
//@[008:027) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("moduleRuntimeCheck2" -> "moduleRuntimeCheck" -> "singleModuleForRuntimeCheck"). Properties of singleModuleForRuntimeCheck which can be calculated at the start include "name". (CodeDescription: none) |moduleRuntimeCheck2|
}]

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
var moduleRuntimeCheck4 = moduleRuntimeCheck3
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[007:033) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |moduleLoopForRuntimeCheck2|
//@[007:033) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |moduleLoopForRuntimeCheck2|
  name: moduleRuntimeCheck4
//@[008:027) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("moduleRuntimeCheck4" -> "moduleRuntimeCheck3" -> "moduleLoopForRuntimeCheck"). Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (CodeDescription: none) |moduleRuntimeCheck4|
}]

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[007:033) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (CodeDescription: none) |moduleLoopForRuntimeCheck3|
//@[007:033) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |moduleLoopForRuntimeCheck3|
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
//@[008:119) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )|
//@[015:051) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (CodeDescription: none) |moduleLoopForRuntimeCheck[1].outputs|
//@[067:103) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (CodeDescription: none) |moduleLoopForRuntimeCheck[1].outputs|
}]

module moduleWithDuplicateName1 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
//@[008:033) [BCP122 (Error)] Modules: "moduleWithDuplicateName1", "moduleWithDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'moduleWithDuplicateName'|
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
//@[008:033) [BCP122 (Error)] Modules: "moduleWithDuplicateName1", "moduleWithDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'moduleWithDuplicateName'|
}

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionB ''
//@[019:021) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
//@[021:021) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionC '' =
//@[019:021) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
//@[023:023) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionD '' = {}
//@[019:021) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionE '' = {
//@[019:021) [BCP050 (Error)] The specified path is empty. (CodeDescription: none) |''|
  name: 'hello'
}

// #completionTest(29) -> cwdDotFileCompletions
module cwdFileCompletionA './m'
//@[026:031) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/m'. (CodeDescription: none) |'./m'|
//@[031:031) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(26, 27) -> cwdFileCompletions
module cwdFileCompletionB m
//@[026:027) [BCP097 (Error)] Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep' (CodeDescription: none) |m|
//@[026:027) [BCP090 (Error)] This module declaration is missing a file path reference. (CodeDescription: none) |m|
//@[027:027) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
module cwdFileCompletionC 'm'
//@[026:029) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/m'. (CodeDescription: none) |'m'|
//@[029:029) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childFileCompletions
module childCompletionA 'ChildModules/'
//@[024:039) [BCP275 (Error)] Unable to open file at path "${TEST_OUTPUT_DIR}/ChildModules/". Found a directory instead. (CodeDescription: none) |'ChildModules/'|
//@[039:039) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotFileCompletions
module childCompletionB './ChildModules/'
//@[024:041) [BCP275 (Error)] Unable to open file at path "${TEST_OUTPUT_DIR}/ChildModules/". Found a directory instead. (CodeDescription: none) |'./ChildModules/'|
//@[041:041) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childDotFileCompletions
module childCompletionC './ChildModules/m'
//@[024:042) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/ChildModules/m'. (CodeDescription: none) |'./ChildModules/m'|
//@[042:042) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childFileCompletions
module childCompletionD 'ChildModules/e'
//@[024:040) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/ChildModules/e'. (CodeDescription: none) |'ChildModules/e'|
//@[040:040) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

@minValue()
//@[001:009) [BCP128 (Error)] Function "minValue" cannot be used as a module decorator. (CodeDescription: none) |minValue|
module moduleWithNotAttachableDecorators './empty.bicep' = {
  name: 'moduleWithNotAttachableDecorators'
}

// loop parsing cases
module expectedForKeyword 'modulea.bicep' = []
//@[045:046) [BCP012 (Error)] Expected the "for" keyword at this location. (CodeDescription: none) |]|

module expectedForKeyword2 'modulea.bicep' = [f]
//@[046:047) [BCP012 (Error)] Expected the "for" keyword at this location. (CodeDescription: none) |f|

module expectedLoopVar 'modulea.bicep' = [for]
//@[045:045) [BCP162 (Error)] Expected a loop item variable identifier or "(" at this location. (CodeDescription: none) ||

module expectedInKeyword 'modulea.bicep' = [for x]
//@[049:050) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |]|

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[051:052) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |b|
//@[052:053) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[058:059) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

module expectedColon 'modulea.bicep' = [for x in y]
//@[049:050) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[050:051) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |]|

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[052:053) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[054:055) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |]|

// indexed loop parsing cases
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[050:052) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 0. (CodeDescription: none) |()|

module expectedComma 'modulea.bicep' = [for (x)]
//@[044:047) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 1. (CodeDescription: none) |(x)|

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[051:055) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 1. (CodeDescription: none) |(x,)|

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[054:055) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |]|

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[064:065) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[054:055) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[055:056) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |]|

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[057:058) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[059:060) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |]|

// loop filter parsing cases
module expectedLoopFilterOpenParen 'modulea.bicep' = [for x in y: if]
//@[063:064) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[068:069) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |]|
module expectedLoopFilterOpenParen2 'modulea.bicep' = [for (x,y) in z: if]
//@[068:069) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[073:074) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |]|

module expectedLoopFilterPredicateAndBody 'modulea.bicep' = [for x in y: if()]
//@[070:071) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[076:076) [BCP243 (Error)] Parentheses must contain exactly one expression. (CodeDescription: none) ||
//@[077:078) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |]|
module expectedLoopFilterPredicateAndBody2 'modulea.bicep' = [for (x,y) in z: if()]
//@[075:076) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[081:081) [BCP243 (Error)] Parentheses must contain exactly one expression. (CodeDescription: none) ||
//@[082:083) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |]|

// wrong loop body type
var emptyArray = []
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[064:065) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |4|
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[069:070) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |4|

// missing loop body properties
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[007:032) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". (CodeDescription: none) |missingLoopBodyProperties|
}]
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[007:033) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name", "params". (CodeDescription: none) |missingLoopBodyProperties2|
}]

// wrong array type
var notAnArray = true
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[050:060) [BCP137 (Error)] Loop expected an expression of type "array" but the provided value is of type "bool". (CodeDescription: none) |notAnArray|
}]

// missing fewer properties
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
  name: 'hello-${x}'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (CodeDescription: none) |params|

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
//@[004:013) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |notAThing|
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
//@[004:013) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |notAThing|
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
//@[004:013) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |notAThing|
  }
}]

module paramNameCompletionsInFilteredLoops 'modulea.bicep' = [for (x,i) in emptyArray: if(true) {
  name: 'hello-${x}'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (CodeDescription: none) |params|
    // #completionTest(0,1,2) -> moduleAParams
  
  }
}]

// #completionTest(100) -> moduleAOutputs
var propertyAccessCompletionsForFilteredModuleLoop = paramNameCompletionsInFilteredLoops[0].outputs.
//@[004:050) [no-unused-vars (Warning)] Variable "propertyAccessCompletionsForFilteredModuleLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |propertyAccessCompletionsForFilteredModuleLoop|
//@[100:100) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// nonexistent arrays and loop variables
var evenMoreDuplicates = 'there'
//@[004:022) [no-unused-vars (Warning)] Variable "evenMoreDuplicates" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |evenMoreDuplicates|
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[007:024) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "evenMoreDuplicates" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |nonexistentArrays|
//@[070:086) [BCP057 (Error)] The name "alsoDoesNotExist" does not exist in the current context. (CodeDescription: none) |alsoDoesNotExist|
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
//@[017:055) [BCP057 (Error)] The name "whyChooseRealVariablesWhenWeCanPretend" does not exist in the current context. (CodeDescription: none) |whyChooseRealVariablesWhenWeCanPretend|
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[043:054) [BCP057 (Error)] The name "totallyFake" does not exist in the current context. (CodeDescription: none) |totallyFake|
//@[056:068) [BCP057 (Error)] The name "doesNotExist" does not exist in the current context. (CodeDescription: none) |doesNotExist|
  }
}]

output directRefToCollectionViaOutput array = nonexistentArrays
//@[046:063) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
  name: 'hello'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[023:049) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |wrongModuleParameterInLoop|
//@[051:068) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
  name: 'hello2'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[023:049) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |wrongModuleParameterInLoop|
//@[051:068) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
//@[007:039) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "test" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |directRefToCollectionViaLoopBody|
  name: 'hello3'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[023:049) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |wrongModuleParameterInLoop|
//@[051:068) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}]

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
//@[007:057) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "test" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |directRefToCollectionViaLoopBodyWithExtraDependsOn|
  name: 'hello4'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[023:049) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |wrongModuleParameterInLoop|
//@[051:068) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
    dependsOn: [
//@[004:013) [BCP037 (Error)] The property "dependsOn" is not allowed on objects of type "params". Permissible properties include "stringParamA". (CodeDescription: none) |dependsOn|
      nonexistentArrays
//@[006:023) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |nonexistentArrays|
    ]
  }
  dependsOn: [
    
  ]
}]


// module body that isn't an object
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
//@[063:070) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |'hello'|
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[064:070) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |concat|
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[068:075) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |'hello'|
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[068:074) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |concat|

module anyTypeInScope 'empty.bicep' = {
//@[007:021) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInScope|
  dependsOn: [
    any('s')
//@[004:012) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('s')|
  ]

  scope: any(42)
//@[009:016) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(42)|
}

module anyTypeInScopeConditional 'empty.bicep' = if(false) {
//@[007:032) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInScopeConditional|
  dependsOn: [
    any('s')
//@[004:012) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('s')|
  ]

  scope: any(42)
//@[009:016) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(42)|
}

module anyTypeInScopeLoop 'empty.bicep' = [for thing in []: {
//@[007:025) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInScopeLoop|
  dependsOn: [
    any('s')
//@[004:012) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('s')|
  ]

  scope: any(42)
//@[009:016) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(42)|
}]

// Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'moduleb.bicep' = {
  name: 'secureModule1'
  params: {       
    stringParamA: kv.getSecret('mySecret')
//@[018:042) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
    stringParamB: '${kv.getSecret('mySecret')}'
//@[018:047) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/simplify-interpolation)) |'${kv.getSecret('mySecret')}'|
//@[021:045) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
    objParam: kv.getSecret('mySecret')
//@[014:038) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
//@[014:038) [BCP036 (Error)] The property "objParam" expected a value of type "object" but the provided value is of type "string". (CodeDescription: none) |kv.getSecret('mySecret')|
    arrayParam: kv.getSecret('mySecret')
//@[016:040) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
//@[016:040) [BCP036 (Error)] The property "arrayParam" expected a value of type "array" but the provided value is of type "string". (CodeDescription: none) |kv.getSecret('mySecret')|
    secureStringParam: '${kv.getSecret('mySecret')}'
//@[023:052) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/simplify-interpolation)) |'${kv.getSecret('mySecret')}'|
//@[026:050) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
    secureObjectParam: kv.getSecret('mySecret')
//@[023:047) [BCP036 (Error)] The property "secureObjectParam" expected a value of type "object" but the provided value is of type "string". (CodeDescription: none) |kv.getSecret('mySecret')|
    secureStringParam2: '${kv.getSecret('mySecret')}'
//@[004:022) [BCP037 (Error)] The property "secureStringParam2" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |secureStringParam2|
//@[024:053) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/simplify-interpolation)) |'${kv.getSecret('mySecret')}'|
//@[027:051) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
    secureObjectParam2: kv.getSecret('mySecret')
//@[004:022) [BCP037 (Error)] The property "secureObjectParam2" is not allowed on objects of type "params". No other properties are allowed. (CodeDescription: none) |secureObjectParam2|
//@[024:048) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
  }
}

module secureModule2 'BAD_MODULE_PATH.bicep' = {
//@[021:044) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/BAD_MODULE_PATH.bicep'. (CodeDescription: none) |'BAD_MODULE_PATH.bicep'|
  name: 'secureModule2'
  params: {       
    secret: kv.getSecret('mySecret')
  }
}

module issue3000 'empty.bicep' = {
  name: 'issue3000Module'
  params: {}
  identity: {
//@[002:010) [BCP037 (Error)] The property "identity" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |identity|
    type: 'SystemAssigned'
  }
  extendedLocation: {}
//@[002:018) [BCP037 (Error)] The property "extendedLocation" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |extendedLocation|
  sku: {}
//@[002:005) [BCP037 (Error)] The property "sku" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |sku|
  kind: 'V1'
//@[002:006) [BCP037 (Error)] The property "kind" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |kind|
  managedBy: 'string'
//@[002:011) [BCP037 (Error)] The property "managedBy" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |managedBy|
  mangedByExtended: [
//@[002:018) [BCP037 (Error)] The property "mangedByExtended" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |mangedByExtended|
   'str1'
   'str2'
  ]
  zones: [
//@[002:007) [BCP037 (Error)] The property "zones" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |zones|
   'str1'
   'str2'
  ]
  plan: {}
//@[002:006) [BCP037 (Error)] The property "plan" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |plan|
  eTag: ''
//@[002:006) [BCP037 (Error)] The property "eTag" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |eTag|
  scale: {}  
//@[002:007) [BCP037 (Error)] The property "scale" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (CodeDescription: none) |scale|
}

module invalidJsonMod 'modulec.json' = {
//@[007:021) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "name". (CodeDescription: none) |invalidJsonMod|
//@[022:036) [BCP188 (Error)] The referenced ARM template has errors. Please see https://aka.ms/arm-template for information on how to diagnose and fix the template. (CodeDescription: none) |'modulec.json'|
}

module jsonModMissingParam 'moduled.json' = {
  name: 'jsonModMissingParam'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "bar". (CodeDescription: none) |params|
    foo: 123
//@[009:012) [BCP036 (Error)] The property "foo" expected a value of type "string" but the provided value is of type "int". (CodeDescription: none) |123|
    baz: 'C'
//@[009:012) [BCP088 (Error)] The property "baz" expected a value of type "'A' | 'B'" but the provided value is of type "'C'". Did you mean "'A'"? (CodeDescription: none) |'C'|
  }
}

module assignToOutput 'empty.bicep' = {
  name: 'assignToOutput'
  outputs: {}
//@[002:009) [BCP073 (Error)] The property "outputs" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |outputs|
}
