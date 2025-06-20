module nonExistentFileRef './nonExistent.bicep' = {
//@[026:047) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[035:056) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[040:073) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/nonExistent.bicep'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'abc/def/../../nonExistent.bicep'|

}

module moduleWithoutPath = {
//@[025:026) [BCP097 (Error)] Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep' (bicep https://aka.ms/bicep/core-diagnostics#BCP097) |=|
//@[025:028) [BCP090 (Error)] This module declaration is missing a file path reference. (bicep https://aka.ms/bicep/core-diagnostics#BCP090) |= {|
//@[028:028) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

}
//@[000:001) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |}|

module modDOne 'moduled.bicep' = {
  name: 'modDOne'
  params: {
    input: 'NameOne'
  }
}

module modDTwo 'moduled.bicep' = {
  name: 'modDTwo'
  params: {
    input: modDOne.outputs.storageAccountName
//@[011:045) [what-if-short-circuiting (Warning)] Runtime value 'modDOne.outputs.storageAccountName' will reduce the precision of what-if analysis for module 'modDTwo' (bicep core linter https://aka.ms/bicep/linter-diagnostics#what-if-short-circuiting) |modDOne.outputs.storageAccountName|
  }
}


// #completionTest(41) -> moduleBodyCompletions
module moduleWithPath './moduleb.bicep' =
//@[041:041) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP118) ||

// missing identifier #completionTest(7) -> empty
module 
//@[007:007) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) ||
//@[007:007) [BCP090 (Error)] This module declaration is missing a file path reference. (bicep https://aka.ms/bicep/core-diagnostics#BCP090) ||

// #completionTest(24,25) -> moduleObject
module missingValue '' = 
//@[020:022) [BCP050 (Error)] The specified path is empty. (bicep https://aka.ms/bicep/core-diagnostics#BCP050) |''|
//@[025:025) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP118) ||

var interp = 'hello'
module moduleWithInterpPath './${interp}.bicep' = {
//@[028:047) [BCP092 (Error)] String interpolation is not supported in file paths. (bicep https://aka.ms/bicep/core-diagnostics#BCP092) |'./${interp}.bicep'|

}

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[040:059) [BCP092 (Error)] String interpolation is not supported in file paths. (bicep https://aka.ms/bicep/core-diagnostics#BCP092) |'./${interp}.bicep'|

}

module moduleWithSelfCycle './main.bicep' = {
//@[027:041) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|

}

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[039:053) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|

}

module './main.bicep' = {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) |'./main.bicep'|
//@[007:021) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|

}

module './main.bicep' = if (1 + 2 == 3) {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) |'./main.bicep'|
//@[007:021) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|

}

module './main.bicep' = if
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) |'./main.bicep'|
//@[007:021) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|
//@[026:026) [BCP018 (Error)] Expected the "(" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

module './main.bicep' = if (
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) |'./main.bicep'|
//@[007:021) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|
//@[028:036) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) |\n\nmodule|

module './main.bicep' = if (true
//@[007:021) [BCP018 (Error)] Expected the ")" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |'./main.bicep'|

module './main.bicep' = if (true)
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) |'./main.bicep'|
//@[007:021) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|
//@[033:033) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

module './main.bicep' = if {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) |'./main.bicep'|
//@[007:021) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|
//@[027:028) [BCP018 (Error)] Expected the "(" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |{|

}

module './main.bicep' = if () {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) |'./main.bicep'|
//@[007:021) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|
//@[028:028) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||

}

module './main.bicep' = if ('true') {
//@[007:021) [BCP096 (Error)] Expected a module identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP096) |'./main.bicep'|
//@[007:021) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|

}

module modANoName './modulea.bicep' = {
//@[007:017) [BCP028 (Error)] Identifier "modANoName" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |modANoName|
//@[007:017) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |modANoName|
// #completionTest(0) -> moduleATopLevelProperties

}

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[007:030) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |modANoNameWithCondition|
// #completionTest(0) -> moduleAWithConditionTopLevelProperties

}

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[035:049) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|

}

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[034:048) [BCP094 (Error)] This module references itself, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP094) |'./main.bicep'|

}


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[007:017) [BCP028 (Error)] Identifier "modANoName" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |modANoName|
//@[007:017) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |modANoName|
//@[044:047) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |'a'|
//@[049:050) [BCP057 (Error)] The name "b" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |b|

}

module modANoInputs './modulea.bicep' = {
//@[007:019) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |modANoInputs|
  name: 'modANoInputs'
//@[008:022) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |'modANoInputs'|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
  
}

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[007:032) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |modANoInputsWithCondition|
  'foo'
]) == 1) {
  name: 'modANoInputs'
//@[008:022) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |'modANoInputs'|
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
  
}

module modAEmptyInputs './modulea.bicep' = {
  name: 'modANoInputs'
//@[008:022) [BCP122 (Error)] Modules: "modANoInputs", "modANoInputsWithCondition", "modAEmptyInputs" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |'modANoInputs'|
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |params|
    // #completionTest(0,1,2,3,4) -> moduleAParams
    
  }
}

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
  name: 'modANoInputs'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |params|
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
    
  }
}

// #completionTest(55) -> moduleATopLevelPropertyAccess
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[004:035) [no-unused-vars (Warning)] Variable "modulePropertyAccessCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |modulePropertyAccessCompletions|
//@[054:055) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |o|

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[004:048) [no-unused-vars (Warning)] Variable "moduleWithConditionPropertyAccessCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |moduleWithConditionPropertyAccessCompletions|
//@[080:081) [BCP053 (Error)] The type "module" does not contain property "o". Available properties include "name", "outputs". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |o|

// #completionTest(56) -> moduleAOutputs
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[004:028) [no-unused-vars (Warning)] Variable "moduleOutputsCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |moduleOutputsCompletions|
//@[055:056) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |s|

// #completionTest(82) -> moduleAWithConditionOutputs
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[004:041) [no-unused-vars (Warning)] Variable "moduleWithConditionOutputsCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |moduleWithConditionOutputsCompletions|
//@[081:082) [BCP053 (Error)] The type "outputs" does not contain property "s". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |s|

module modAUnspecifiedInputs './modulea.bicep' = {
  name: 'modAUnspecifiedInputs'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |params|
    stringParamB: ''
    objParam: {}
    objArray: []
//@[004:012) [BCP037 (Error)] The property "objArray" is not allowed on objects of type "params". Permissible properties include "arrayParam", "stringParamA". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |objArray|
    unspecifiedInput: ''
//@[004:020) [BCP037 (Error)] The property "unspecifiedInput" is not allowed on objects of type "params". Permissible properties include "arrayParam", "stringParamA". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |unspecifiedInput|
  }
}

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[004:021) [no-unused-vars (Warning)] Variable "unspecifiedOutput" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |unspecifiedOutput|
//@[054:058) [BCP053 (Error)] The type "outputs" does not contain property "test". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |test|

module modCycle './cycle.bicep' = {
//@[016:031) [BCP095 (Error)] The file is involved in a cycle ("${TEST_OUTPUT_DIR}/cycle.bicep" -> "${TEST_OUTPUT_DIR}/main.bicep"). (bicep https://aka.ms/bicep/core-diagnostics#BCP095) |'./cycle.bicep'|
  
}

module moduleWithEmptyPath '' = {
//@[027:029) [BCP050 (Error)] The specified path is empty. (bicep https://aka.ms/bicep/core-diagnostics#BCP050) |''|
}

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[030:046) [BCP051 (Error)] The specified path begins with "/". Files must be referenced using relative paths. (bicep https://aka.ms/bicep/core-diagnostics#BCP051) |'/abc/def.bicep'|
}

module moduleWithBackslash 'child\\file.bicep' = {
//@[027:046) [BCP098 (Error)] The specified file path contains a "\" character. Use "/" instead as the directory separator character. (bicep https://aka.ms/bicep/core-diagnostics#BCP098) |'child\\file.bicep'|
}

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[029:048) [BCP085 (Error)] The specified file path contains one ore more invalid path characters. The following are not permitted: """, "*", ":", "<", ">", "?", "\", "|". (bicep https://aka.ms/bicep/core-diagnostics#BCP085) |'child/fi|le.bicep'|
}

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[039:052) [BCP086 (Error)] The specified file path ends with an invalid character. The following are not permitted: " ", ".". (bicep https://aka.ms/bicep/core-diagnostics#BCP086) |'child/test.'|
}

module moduleWithValidScope './empty.bicep' = {
  name: 'moduleWithValidScope'
}

module moduleWithInvalidScope './empty.bicep' = {
  name: 'moduleWithInvalidScope'
  scope: moduleWithValidScope
//@[009:029) [BCP134 (Error)] Scope "module" is not valid for this module. Permitted scopes: "resourceGroup". (bicep https://aka.ms/bicep/core-diagnostics#BCP134) |moduleWithValidScope|
}

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[007:037) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |moduleWithMissingRequiredScope|
//@[069:113) [BCP134 (Error)] Scope "resourceGroup" is not valid for this module. Permitted scopes: "subscription". (bicep https://aka.ms/bicep/core-diagnostics#BCP134) |{\n  name: 'moduleWithMissingRequiredScope'\n}|
  name: 'moduleWithMissingRequiredScope'
}

module moduleWithInvalidScope2 './empty.bicep' = {
  name: 'moduleWithInvalidScope2'
  scope: managementGroup()
//@[024:026) [BCP071 (Error)] Expected 1 argument, but got 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |()|
}

module moduleWithUnsupportedScope1 './mg_empty.bicep' = {
  name: 'moduleWithUnsupportedScope1'
  scope: managementGroup()
//@[024:026) [BCP071 (Error)] Expected 1 argument, but got 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |()|
}

module moduleWithUnsupportedScope2 './mg_empty.bicep' = {
  name: 'moduleWithUnsupportedScope2'
  scope: managementGroup('MG')
//@[009:030) [BCP134 (Error)] Scope "managementGroup" is not valid for this module. Permitted scopes: "subscription". (bicep https://aka.ms/bicep/core-diagnostics#BCP134) |managementGroup('MG')|
}

module moduleWithBadScope './empty.bicep' = {
  name: 'moduleWithBadScope'
  scope: 'stringScope'
//@[009:022) [BCP036 (Error)] The property "scope" expected a value of type "resourceGroup" but the provided value is of type "'stringScope'". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |'stringScope'|
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
//@[008:089) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)|
//@[015:065) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(runtimeValidRes1.id, runtimeValidRes1.name)|
}

module runtimeInvalidModule1 'empty.bicep' = {
  name: runtimeValidRes1.location
//@[008:033) [BCP122 (Error)] Modules: "runtimeInvalidModule1", "runtimeInvalidModule2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |runtimeValidRes1.location|
//@[008:033) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |runtimeValidRes1.location|
}

module runtimeInvalidModule2 'empty.bicep' = {
  name: runtimeValidRes1['location']
//@[008:036) [BCP122 (Error)] Modules: "runtimeInvalidModule1", "runtimeInvalidModule2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |runtimeValidRes1['location']|
//@[008:036) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |runtimeValidRes1['location']|
//@[024:036) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['location']|
}

module runtimeInvalidModule3 'empty.bicep' = {
  name: runtimeValidRes1.sku.name
//@[008:028) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |runtimeValidRes1.sku|
//@[008:033) [BCP122 (Error)] Modules: "runtimeInvalidModule3", "runtimeInvalidModule4", "runtimeInvalidModule5", "runtimeInvalidModule6" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |runtimeValidRes1.sku.name|
}

module runtimeInvalidModule4 'empty.bicep' = {
  name: runtimeValidRes1.sku['name']
//@[008:028) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |runtimeValidRes1.sku|
//@[008:036) [BCP122 (Error)] Modules: "runtimeInvalidModule3", "runtimeInvalidModule4", "runtimeInvalidModule5", "runtimeInvalidModule6" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |runtimeValidRes1.sku['name']|
//@[028:036) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['name']|
}

module runtimeInvalidModule5 'empty.bicep' = {
  name: runtimeValidRes1['sku']['name']
//@[008:031) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |runtimeValidRes1['sku']|
//@[008:039) [BCP122 (Error)] Modules: "runtimeInvalidModule3", "runtimeInvalidModule4", "runtimeInvalidModule5", "runtimeInvalidModule6" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |runtimeValidRes1['sku']['name']|
//@[024:031) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['sku']|
//@[031:039) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['name']|
}

module runtimeInvalidModule6 'empty.bicep' = {
  name: runtimeValidRes1['sku'].name
//@[008:031) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |runtimeValidRes1['sku']|
//@[008:036) [BCP122 (Error)] Modules: "runtimeInvalidModule3", "runtimeInvalidModule4", "runtimeInvalidModule5", "runtimeInvalidModule6" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |runtimeValidRes1['sku'].name|
//@[024:031) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['sku']|
}

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[007:034) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |singleModuleForRuntimeCheck|
  name: 'test'
}

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
var moduleRuntimeCheck2 = moduleRuntimeCheck

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[007:032) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |moduleLoopForRuntimeCheck|
//@[007:032) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (bicep https://aka.ms/bicep/core-diagnostics#BCP179) |moduleLoopForRuntimeCheck|
  name: moduleRuntimeCheck2
//@[008:027) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("moduleRuntimeCheck2" -> "moduleRuntimeCheck" -> "singleModuleForRuntimeCheck"). Properties of singleModuleForRuntimeCheck which can be calculated at the start include "name". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |moduleRuntimeCheck2|
}]

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
var moduleRuntimeCheck4 = moduleRuntimeCheck3
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[007:033) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |moduleLoopForRuntimeCheck2|
//@[007:033) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (bicep https://aka.ms/bicep/core-diagnostics#BCP179) |moduleLoopForRuntimeCheck2|
  name: moduleRuntimeCheck4
//@[008:027) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("moduleRuntimeCheck4" -> "moduleRuntimeCheck3" -> "moduleLoopForRuntimeCheck"). Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |moduleRuntimeCheck4|
}]

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[007:033) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |moduleLoopForRuntimeCheck3|
//@[007:033) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (bicep https://aka.ms/bicep/core-diagnostics#BCP179) |moduleLoopForRuntimeCheck3|
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
//@[008:119) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )|
//@[015:051) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |moduleLoopForRuntimeCheck[1].outputs|
//@[067:103) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "module" type, which requires a value that can be calculated at the start of the deployment. Properties of moduleLoopForRuntimeCheck which can be calculated at the start include "name". (bicep https://aka.ms/bicep/core-diagnostics#BCP120) |moduleLoopForRuntimeCheck[1].outputs|
}]

module moduleWithDuplicateName1 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
//@[008:033) [BCP122 (Error)] Modules: "moduleWithDuplicateName1", "moduleWithDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |'moduleWithDuplicateName'|
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
//@[008:033) [BCP122 (Error)] Modules: "moduleWithDuplicateName1", "moduleWithDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (bicep https://aka.ms/bicep/core-diagnostics#BCP122) |'moduleWithDuplicateName'|
}

// #completionTest(19, 20, 21) -> cwdFileAndBicepRegitryTemplateSpecSchemaCompletions
module completionB ''
//@[019:021) [BCP050 (Error)] The specified path is empty. (bicep https://aka.ms/bicep/core-diagnostics#BCP050) |''|
//@[021:021) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(19, 20, 21) -> cwdFileAndBicepRegitryTemplateSpecSchemaCompletions
module completionC '' =
//@[019:021) [BCP050 (Error)] The specified path is empty. (bicep https://aka.ms/bicep/core-diagnostics#BCP050) |''|
//@[023:023) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP118) ||

// #completionTest(19, 20, 21) -> cwdFileAndBicepRegitryTemplateSpecSchemaCompletions
module completionD '' = {}
//@[019:021) [BCP050 (Error)] The specified path is empty. (bicep https://aka.ms/bicep/core-diagnostics#BCP050) |''|

// #completionTest(19, 20, 21) -> cwdFileAndBicepRegitryTemplateSpecSchemaCompletions
module completionE '' = {
//@[019:021) [BCP050 (Error)] The specified path is empty. (bicep https://aka.ms/bicep/core-diagnostics#BCP050) |''|
  name: 'hello'
}

// #completionTest(29) -> cwdDotFileCompletions
module cwdFileCompletionA './m'
//@[026:031) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/m'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'./m'|
//@[031:031) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(26, 27) -> cwdFileCompletions
module cwdFileCompletionB m
//@[026:027) [BCP097 (Error)] Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep' (bicep https://aka.ms/bicep/core-diagnostics#BCP097) |m|
//@[026:027) [BCP090 (Error)] This module declaration is missing a file path reference. (bicep https://aka.ms/bicep/core-diagnostics#BCP090) |m|
//@[027:027) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
module cwdFileCompletionC 'm'
//@[026:029) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/m'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'m'|
//@[029:029) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childFileCompletions
module childCompletionA 'ChildModules/'
//@[024:039) [BCP275 (Error)] Unable to open file at path "${TEST_OUTPUT_DIR}/ChildModules/". Found a directory instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP275) |'ChildModules/'|
//@[039:039) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotFileCompletions
module childCompletionB './ChildModules/'
//@[024:041) [BCP275 (Error)] Unable to open file at path "${TEST_OUTPUT_DIR}/ChildModules/". Found a directory instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP275) |'./ChildModules/'|
//@[041:041) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childDotFileCompletions
module childCompletionC './ChildModules/m'
//@[024:042) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/ChildModules/m'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'./ChildModules/m'|
//@[042:042) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childFileCompletions
module childCompletionD 'ChildModules/e'
//@[024:040) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/ChildModules/e'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'ChildModules/e'|
//@[040:040) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

@minValue()
//@[001:009) [BCP128 (Error)] Function "minValue" cannot be used as a module decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP128) |minValue|
module moduleWithNotAttachableDecorators './empty.bicep' = {
  name: 'moduleWithNotAttachableDecorators'
}

// loop parsing cases
module expectedForKeyword 'modulea.bicep' = []
//@[045:046) [BCP012 (Error)] Expected the "for" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) |]|

module expectedForKeyword2 'modulea.bicep' = [f]
//@[046:047) [BCP012 (Error)] Expected the "for" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) |f|

module expectedLoopVar 'modulea.bicep' = [for]
//@[045:045) [BCP162 (Error)] Expected a loop item variable identifier or "(" at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP162) ||

module expectedInKeyword 'modulea.bicep' = [for x]
//@[049:050) [BCP012 (Error)] Expected the "in" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) |]|

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[051:052) [BCP012 (Error)] Expected the "in" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) |b|
//@[052:053) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |]|

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[058:059) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |]|

module expectedColon 'modulea.bicep' = [for x in y]
//@[049:050) [BCP057 (Error)] The name "y" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |y|
//@[050:051) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |]|

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[052:053) [BCP057 (Error)] The name "y" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |y|
//@[054:055) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP167) |]|

// indexed loop parsing cases
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[050:052) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP249) |()|

module expectedComma 'modulea.bicep' = [for (x)]
//@[044:047) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP249) |(x)|

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[051:055) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP249) |(x,)|

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[054:055) [BCP012 (Error)] Expected the "in" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP012) |]|

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[064:065) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |]|

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[054:055) [BCP057 (Error)] The name "z" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |z|
//@[055:056) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |]|

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[057:058) [BCP057 (Error)] The name "z" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |z|
//@[059:060) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP167) |]|

// loop filter parsing cases
module expectedLoopFilterOpenParen 'modulea.bicep' = [for x in y: if]
//@[063:064) [BCP057 (Error)] The name "y" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |y|
//@[068:069) [BCP018 (Error)] Expected the "(" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |]|
module expectedLoopFilterOpenParen2 'modulea.bicep' = [for (x,y) in z: if]
//@[068:069) [BCP057 (Error)] The name "z" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |z|
//@[073:074) [BCP018 (Error)] Expected the "(" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |]|

module expectedLoopFilterPredicateAndBody 'modulea.bicep' = [for x in y: if()]
//@[070:071) [BCP057 (Error)] The name "y" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |y|
//@[076:076) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||
//@[077:078) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |]|
module expectedLoopFilterPredicateAndBody2 'modulea.bicep' = [for (x,y) in z: if()]
//@[075:076) [BCP057 (Error)] The name "z" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |z|
//@[081:081) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||
//@[082:083) [BCP018 (Error)] Expected the "{" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |]|

// wrong loop body type
var emptyArray = []
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[064:065) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP167) |4|
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[069:070) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP167) |4|

// missing loop body properties
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[007:032) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |missingLoopBodyProperties|
}]
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[007:033) [BCP035 (Error)] The specified "module" declaration is missing the following required properties: "params". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |missingLoopBodyProperties2|
}]

// wrong array type
var notAnArray = true
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[050:060) [BCP137 (Error)] Loop expected an expression of type "array" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP137) |notAnArray|
}]

// missing fewer properties
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
  name: 'hello-${x}'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |params|

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
//@[004:013) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |notAThing|
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
//@[004:013) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |notAThing|
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
//@[004:013) [BCP037 (Error)] The property "notAThing" is not allowed on objects of type "params". No other properties are allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |notAThing|
  }
}]

module paramNameCompletionsInFilteredLoops 'modulea.bicep' = [for (x,i) in emptyArray: if(true) {
  name: 'hello-${x}'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "objParam", "stringParamB". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |params|
    // #completionTest(0,1,2) -> moduleAParams
  
  }
}]

// #completionTest(100) -> moduleAOutputs
var propertyAccessCompletionsForFilteredModuleLoop = paramNameCompletionsInFilteredLoops[0].outputs.
//@[004:050) [no-unused-vars (Warning)] Variable "propertyAccessCompletionsForFilteredModuleLoop" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |propertyAccessCompletionsForFilteredModuleLoop|
//@[100:100) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||

// nonexistent arrays and loop variables
var evenMoreDuplicates = 'there'
//@[004:022) [no-unused-vars (Warning)] Variable "evenMoreDuplicates" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |evenMoreDuplicates|
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[007:024) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "evenMoreDuplicates" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (bicep https://aka.ms/bicep/core-diagnostics#BCP179) |nonexistentArrays|
//@[070:086) [BCP057 (Error)] The name "alsoDoesNotExist" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |alsoDoesNotExist|
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
//@[017:055) [BCP057 (Error)] The name "whyChooseRealVariablesWhenWeCanPretend" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |whyChooseRealVariablesWhenWeCanPretend|
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[043:054) [BCP057 (Error)] The name "totallyFake" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |totallyFake|
//@[056:068) [BCP057 (Error)] The name "doesNotExist" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |doesNotExist|
  }
}]

output directRefToCollectionViaOutput array = nonexistentArrays
//@[038:043) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
  name: 'hello'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
  name: 'hello2'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
//@[007:039) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "test" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (bicep https://aka.ms/bicep/core-diagnostics#BCP179) |directRefToCollectionViaLoopBody|
  name: 'hello3'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[023:049) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP144) |wrongModuleParameterInLoop|
//@[051:068) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP144) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
  }
}]

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
//@[007:057) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "test" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (bicep https://aka.ms/bicep/core-diagnostics#BCP179) |directRefToCollectionViaLoopBodyWithExtraDependsOn|
  name: 'hello4'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[023:049) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP144) |wrongModuleParameterInLoop|
//@[051:068) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP144) |nonexistentArrays|
    objParam: {}
    stringParamB: ''
    dependsOn: [
//@[004:013) [BCP037 (Error)] The property "dependsOn" is not allowed on objects of type "params". Permissible properties include "stringParamA". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |dependsOn|
      nonexistentArrays
//@[006:023) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP144) |nonexistentArrays|
    ]
  }
  dependsOn: [
    
  ]
}]


// module body that isn't an object
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
//@[063:070) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP167) |'hello'|
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[064:070) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP167) |concat|
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[068:075) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP167) |'hello'|
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[068:074) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP167) |concat|

module anyTypeInScope 'empty.bicep' = {
  dependsOn: [
    any('s')
//@[004:012) [BCP176 (Error)] Values of the "any" type are not allowed here. (bicep https://aka.ms/bicep/core-diagnostics#BCP176) |any('s')|
  ]

  scope: any(42)
//@[009:016) [BCP176 (Error)] Values of the "any" type are not allowed here. (bicep https://aka.ms/bicep/core-diagnostics#BCP176) |any(42)|
}

module anyTypeInScopeConditional 'empty.bicep' = if(false) {
  dependsOn: [
    any('s')
//@[004:012) [BCP176 (Error)] Values of the "any" type are not allowed here. (bicep https://aka.ms/bicep/core-diagnostics#BCP176) |any('s')|
  ]

  scope: any(42)
//@[009:016) [BCP176 (Error)] Values of the "any" type are not allowed here. (bicep https://aka.ms/bicep/core-diagnostics#BCP176) |any(42)|
}

module anyTypeInScopeLoop 'empty.bicep' = [for thing in []: {
  dependsOn: [
    any('s')
//@[004:012) [BCP176 (Error)] Values of the "any" type are not allowed here. (bicep https://aka.ms/bicep/core-diagnostics#BCP176) |any('s')|
  ]

  scope: any(42)
//@[009:016) [BCP176 (Error)] Values of the "any" type are not allowed here. (bicep https://aka.ms/bicep/core-diagnostics#BCP176) |any(42)|
}]

// Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'moduleb.bicep' = {
  name: 'secureModule1'
  params: {       
    stringParamA: kv.getSecret('mySecret')
//@[018:042) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
    stringParamB: '${kv.getSecret('mySecret')}'
//@[018:047) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#simplify-interpolation) |'${kv.getSecret('mySecret')}'|
//@[021:045) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
    objParam: kv.getSecret('mySecret')
//@[014:038) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
//@[014:038) [BCP036 (Error)] The property "objParam" expected a value of type "object" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |kv.getSecret('mySecret')|
    arrayParam: kv.getSecret('mySecret')
//@[016:040) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
//@[016:040) [BCP036 (Error)] The property "arrayParam" expected a value of type "array" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |kv.getSecret('mySecret')|
    secureStringParam: '${kv.getSecret('mySecret')}'
//@[023:052) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#simplify-interpolation) |'${kv.getSecret('mySecret')}'|
//@[026:050) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
    secureObjectParam: kv.getSecret('mySecret')
//@[023:047) [BCP036 (Error)] The property "secureObjectParam" expected a value of type "null | object" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |kv.getSecret('mySecret')|
    secureStringParam2: '${kv.getSecret('mySecret')}'
//@[004:022) [BCP037 (Error)] The property "secureStringParam2" is not allowed on objects of type "params". No other properties are allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |secureStringParam2|
//@[024:053) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#simplify-interpolation) |'${kv.getSecret('mySecret')}'|
//@[027:051) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
    secureObjectParam2: kv.getSecret('mySecret')
//@[004:022) [BCP037 (Error)] The property "secureObjectParam2" is not allowed on objects of type "params". No other properties are allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |secureObjectParam2|
//@[024:048) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
  }
}

module secureModule2 'BAD_MODULE_PATH.bicep' = {
//@[021:044) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/BAD_MODULE_PATH.bicep'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'BAD_MODULE_PATH.bicep'|
  name: 'secureModule2'
  params: {       
    secret: kv.getSecret('mySecret')
  }
}

module issue3000 'empty.bicep' = {
  name: 'issue3000Module'
  params: {}
  identity: {
//@[002:010) [BCP037 (Error)] The property "identity" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |identity|
    type: 'SystemAssigned'
  }
  extendedLocation: {}
//@[002:018) [BCP037 (Error)] The property "extendedLocation" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |extendedLocation|
  sku: {}
//@[002:005) [BCP037 (Error)] The property "sku" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |sku|
  kind: 'V1'
//@[002:006) [BCP037 (Error)] The property "kind" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |kind|
  managedBy: 'string'
//@[002:011) [BCP037 (Error)] The property "managedBy" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |managedBy|
  mangedByExtended: [
//@[002:018) [BCP037 (Error)] The property "mangedByExtended" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |mangedByExtended|
   'str1'
   'str2'
  ]
  zones: [
//@[002:007) [BCP037 (Error)] The property "zones" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |zones|
   'str1'
   'str2'
  ]
  plan: {}
//@[002:006) [BCP037 (Error)] The property "plan" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |plan|
  eTag: ''
//@[002:006) [BCP037 (Error)] The property "eTag" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |eTag|
  scale: {}  
//@[002:007) [BCP037 (Error)] The property "scale" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope". (bicep https://aka.ms/bicep/core-diagnostics#BCP037) |scale|
}

module invalidJsonMod 'modulec.json' = {
//@[022:036) [BCP188 (Error)] The referenced ARM template has errors. Please see https://aka.ms/arm-template for information on how to diagnose and fix the template. (bicep https://aka.ms/bicep/core-diagnostics#BCP188) |'modulec.json'|
}

module jsonModMissingParam 'moduled.json' = {
  name: 'jsonModMissingParam'
  params: {
//@[002:008) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "bar". (bicep https://aka.ms/bicep/core-diagnostics#BCP035) |params|
    foo: 123
//@[009:012) [BCP036 (Error)] The property "foo" expected a value of type "string" but the provided value is of type "123". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |123|
    baz: 'C'
//@[009:012) [BCP088 (Error)] The property "baz" expected a value of type "'A' | 'B'" but the provided value is of type "'C'". Did you mean "'A'"? (bicep https://aka.ms/bicep/core-diagnostics#BCP088) |'C'|
  }
}

module assignToOutput 'empty.bicep' = {
  name: 'assignToOutput'
  outputs: {}
//@[002:009) [BCP073 (Error)] The property "outputs" is read-only. Expressions cannot be assigned to read-only properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP073) |outputs|
}

