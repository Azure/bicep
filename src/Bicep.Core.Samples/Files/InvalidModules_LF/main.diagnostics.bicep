module nonExistentFileRef './nonExistent.bicep' = {
//@[26:47) [BCP091 (Error)] An error occurred loading the module. Could not find file '${TEST_OUTPUT_DIR}nonExistent.bicep'. |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[35:56) [BCP091 (Error)] An error occurred loading the module. Could not find file '${TEST_OUTPUT_DIR}nonExistent.bicep'. |'./nonExistent.bicep'|

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[40:73) [BCP091 (Error)] An error occurred loading the module. Could not find file '${TEST_OUTPUT_DIR}nonExistent.bicep'. |'abc/def/../../nonExistent.bicep'|

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
//@[20:22) [BCP094 (Error)] This module references itself, which is not allowed. |''|
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
//@[38:42) [BCP035 (Error)] The specified object is missing the following required properties: "name", "params". |{\n\n}|

}

module modANoInputs './modulea.bicep' = {
//@[40:66) [BCP035 (Error)] The specified object is missing the following required properties: "params". |{\n  name: 'modANoInputs'\n}|
  name: 'modANoInputs'
}

module modAEmptyInputs './modulea.bicep' = {
  name: 'modANoInputs'
  params: {
//@[10:16) [BCP035 (Error)] The specified object is missing the following required properties: "arrayParam", "objParam", "stringParamB". |{\n\n  }|

  }
}

module modAUnspecifiedInputs './modulea.bicep' = {
  name: 'modAUnspecifiedInputs'
  params: {
//@[10:95) [BCP035 (Error)] The specified object is missing the following required properties: "arrayParam". |{\n    stringParamB: ''\n    objParam: {}\n    objArray: []\n    unspecifiedInput: ''\n  }|
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

module moduleWithBackslash 'child\\file.bicep' = {
//@[27:46) [BCP098 (Error)] File paths must use forward slash ("/") characters instead of back slash ("\") characters for directory separators. |'child\\file.bicep'|
  
}

module modCycle './cycle.bicep' = {
//@[16:31) [BCP095 (Error)] The module is involved in a cycle ("${TEST_OUTPUT_DIR}cycle.bicep" -> "${TEST_OUTPUT_DIR}main.bicep"). |'./cycle.bicep'|
  
}
