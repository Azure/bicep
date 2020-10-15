module moduleWithMissingPath './nonExistent.bicep' = {
//@[29:50) [BCP089 (Error)] An error occurred loading the module. Received failure "Could not find file '${TEST_OUTPUT_DIR}nonExistent.bicep'.". |'./nonExistent.bicep'|

}

module moduleWithoutPath = {
//@[25:26) [BCP095 (Error)] Expected a module path string. |=|
//@[25:28) [BCP088 (Error)] Unable to find file path for module. |= {|
//@[28:28) [BCP018 (Error)] Expected the "=" character at this location. ||

}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |}|

var interp = 'hello'
module moduleWithInterpPath './${interp}.bicep' = {
//@[28:47) [BCP090 (Error)] String interpolation is unsupported for specifying the module path. |'./${interp}.bicep'|

}

module moduleWithSelfCycle './main.bicep' = {
//@[27:41) [BCP092 (Error)] This module references its own declaring file, which is not allowed. |'./main.bicep'|

}

module './main.bicep' = {
//@[7:21) [BCP094 (Error)] Expected a module identifier at this location. |'./main.bicep'|
//@[7:21) [BCP092 (Error)] This module references its own declaring file, which is not allowed. |'./main.bicep'|

}

module modANoInputs './modulea.bicep' = {
//@[40:46) [BCP035 (Error)] The specified object is missing the following required properties: "arrayParam", "objParam", "stringParamB". |{\n  \n}|
  
}

module modAUnspecifiedInputs './modulea.bicep' = {
//@[49:124) [BCP035 (Error)] The specified object is missing the following required properties: "arrayParam". |{\n  stringParamB: ''\n  objParam: {}\n  objArray: []\n  unspecifiedInput: ''\n}|
  stringParamB: ''
  objParam: {}
  objArray: []
//@[2:10) [BCP038 (Error)] The property "objArray" is not allowed on objects of type "modAUnspecifiedInputs". Permissible properties include "arrayParam", "stringParamA". |objArray|
  unspecifiedInput: ''
//@[2:18) [BCP038 (Error)] The property "unspecifiedInput" is not allowed on objects of type "modAUnspecifiedInputs". Permissible properties include "arrayParam", "stringParamA". |unspecifiedInput|
}

var unspecifiedOutput = modAUnspecifiedInputs.test
//@[46:50) [BCP053 (Error)] The type "modAUnspecifiedInputs" does not contain property "test". Available properties include "arrayOutput", "objOutput", "stringOutputA", "stringOutputB". |test|

module moduleWithBackslash 'child\\file.bicep' = {
//@[27:46) [BCP097 (Error)] File paths must use forward slash ("/") characters instead of back slash ("\") characters for directory separators. |'child\\file.bicep'|
  
}

module modCycle './cycle.bicep' = {
//@[16:31) [BCP093 (Error)] The module is involved in a cycle ("${TEST_OUTPUT_DIR}cycle.bicep" -> "${TEST_OUTPUT_DIR}main.bicep"). |'./cycle.bicep'|
  
}
