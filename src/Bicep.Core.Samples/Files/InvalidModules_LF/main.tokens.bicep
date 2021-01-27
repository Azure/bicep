module nonExistentFileRef './nonExistent.bicep' = {
//@[0:6) Identifier |module|
//@[7:25) Identifier |nonExistentFileRef|
//@[26:47) StringComplete |'./nonExistent.bicep'|
//@[48:49) Assignment |=|
//@[50:51) LeftBrace |{|
//@[51:53) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// we should only look this file up once, but should still return the same failure
//@[82:83) NewLine |\n|
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[0:6) Identifier |module|
//@[7:34) Identifier |nonExistentFileRefDuplicate|
//@[35:56) StringComplete |'./nonExistent.bicep'|
//@[57:58) Assignment |=|
//@[59:60) LeftBrace |{|
//@[60:62) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// we should only look this file up once, but should still return the same failure
//@[82:83) NewLine |\n|
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[0:6) Identifier |module|
//@[7:39) Identifier |nonExistentFileRefEquivalentPath|
//@[40:73) StringComplete |'abc/def/../../nonExistent.bicep'|
//@[74:75) Assignment |=|
//@[76:77) LeftBrace |{|
//@[77:79) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithoutPath = {
//@[0:6) Identifier |module|
//@[7:24) Identifier |moduleWithoutPath|
//@[25:26) Assignment |=|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// missing identifier #completionTest(7) -> empty
//@[49:50) NewLine |\n|
module 
//@[0:6) Identifier |module|
//@[7:9) NewLine |\n\n|

// #completionTest(24,25) -> object
//@[35:36) NewLine |\n|
module missingValue '' = 
//@[0:6) Identifier |module|
//@[7:19) Identifier |missingValue|
//@[20:22) StringComplete |''|
//@[23:24) Assignment |=|
//@[25:27) NewLine |\n\n|

var interp = 'hello'
//@[0:3) Identifier |var|
//@[4:10) Identifier |interp|
//@[11:12) Assignment |=|
//@[13:20) StringComplete |'hello'|
//@[20:21) NewLine |\n|
module moduleWithInterpPath './${interp}.bicep' = {
//@[0:6) Identifier |module|
//@[7:27) Identifier |moduleWithInterpPath|
//@[28:33) StringLeftPiece |'./${|
//@[33:39) Identifier |interp|
//@[39:47) StringRightPiece |}.bicep'|
//@[48:49) Assignment |=|
//@[50:51) LeftBrace |{|
//@[51:53) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[0:6) Identifier |module|
//@[7:39) Identifier |moduleWithConditionAndInterpPath|
//@[40:45) StringLeftPiece |'./${|
//@[45:51) Identifier |interp|
//@[51:59) StringRightPiece |}.bicep'|
//@[60:61) Assignment |=|
//@[62:64) Identifier |if|
//@[65:66) LeftParen |(|
//@[66:70) TrueKeyword |true|
//@[70:71) RightParen |)|
//@[72:73) LeftBrace |{|
//@[73:75) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithSelfCycle './main.bicep' = {
//@[0:6) Identifier |module|
//@[7:26) Identifier |moduleWithSelfCycle|
//@[27:41) StringComplete |'./main.bicep'|
//@[42:43) Assignment |=|
//@[44:45) LeftBrace |{|
//@[45:47) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[0:6) Identifier |module|
//@[7:38) Identifier |moduleWithConditionAndSelfCycle|
//@[39:53) StringComplete |'./main.bicep'|
//@[54:55) Assignment |=|
//@[56:58) Identifier |if|
//@[59:60) LeftParen |(|
//@[60:65) StringComplete |'foo'|
//@[66:68) Equals |==|
//@[69:74) StringComplete |'bar'|
//@[74:75) RightParen |)|
//@[76:77) LeftBrace |{|
//@[77:79) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = {
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:27) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = if (1 + 2 == 3) {
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:26) Identifier |if|
//@[27:28) LeftParen |(|
//@[28:29) Number |1|
//@[30:31) Plus |+|
//@[32:33) Number |2|
//@[34:36) Equals |==|
//@[37:38) Number |3|
//@[38:39) RightParen |)|
//@[40:41) LeftBrace |{|
//@[41:43) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = if
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:26) Identifier |if|
//@[26:28) NewLine |\n\n|

module './main.bicep' = if (
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:26) Identifier |if|
//@[27:28) LeftParen |(|
//@[28:30) NewLine |\n\n|

module './main.bicep' = if (true
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:26) Identifier |if|
//@[27:28) LeftParen |(|
//@[28:32) TrueKeyword |true|
//@[32:34) NewLine |\n\n|

module './main.bicep' = if (true)
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:26) Identifier |if|
//@[27:28) LeftParen |(|
//@[28:32) TrueKeyword |true|
//@[32:33) RightParen |)|
//@[33:35) NewLine |\n\n|

module './main.bicep' = if {
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:26) Identifier |if|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = if () {
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:26) Identifier |if|
//@[27:28) LeftParen |(|
//@[28:29) RightParen |)|
//@[30:31) LeftBrace |{|
//@[31:33) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = if ('true') {
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:26) Identifier |if|
//@[27:28) LeftParen |(|
//@[28:34) StringComplete |'true'|
//@[34:35) RightParen |)|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoName './modulea.bicep' = {
//@[0:6) Identifier |module|
//@[7:17) Identifier |modANoName|
//@[18:35) StringComplete |'./modulea.bicep'|
//@[36:37) Assignment |=|
//@[38:39) LeftBrace |{|
//@[39:40) NewLine |\n|
// #completionTest(0) -> moduleATopLevelProperties
//@[50:52) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[0:6) Identifier |module|
//@[7:30) Identifier |modANoNameWithCondition|
//@[31:48) StringComplete |'./modulea.bicep'|
//@[49:50) Assignment |=|
//@[51:53) Identifier |if|
//@[54:55) LeftParen |(|
//@[55:59) TrueKeyword |true|
//@[59:60) RightParen |)|
//@[61:62) LeftBrace |{|
//@[62:63) NewLine |\n|
// #completionTest(0) -> moduleAWithConditionTopLevelProperties
//@[63:65) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[0:6) Identifier |module|
//@[7:34) Identifier |modWithReferenceInCondition|
//@[35:49) StringComplete |'./main.bicep'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[56:65) Identifier |reference|
//@[65:66) LeftParen |(|
//@[66:108) StringComplete |'Micorosft.Management/managementGroups/MG'|
//@[108:109) Comma |,|
//@[110:122) StringComplete |'2020-05-01'|
//@[122:123) RightParen |)|
//@[123:124) Dot |.|
//@[124:128) Identifier |name|
//@[129:131) Equals |==|
//@[132:143) StringComplete |'something'|
//@[143:144) RightParen |)|
//@[145:146) LeftBrace |{|
//@[146:148) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[0:6) Identifier |module|
//@[7:33) Identifier |modWithListKeysInCondition|
//@[34:48) StringComplete |'./main.bicep'|
//@[49:50) Assignment |=|
//@[51:53) Identifier |if|
//@[54:55) LeftParen |(|
//@[55:63) Identifier |listKeys|
//@[63:64) LeftParen |(|
//@[64:69) StringComplete |'foo'|
//@[69:70) Comma |,|
//@[71:83) StringComplete |'2020-05-01'|
//@[83:84) RightParen |)|
//@[84:85) Dot |.|
//@[85:88) Identifier |bar|
//@[89:91) Equals |==|
//@[92:96) TrueKeyword |true|
//@[96:97) RightParen |)|
//@[98:99) LeftBrace |{|
//@[99:101) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:4) NewLine |\n\n\n|


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[0:6) Identifier |module|
//@[7:17) Identifier |modANoName|
//@[18:35) StringComplete |'./modulea.bicep'|
//@[36:37) Assignment |=|
//@[38:40) Identifier |if|
//@[41:42) LeftParen |(|
//@[42:43) LeftBrace |{|
//@[44:47) StringComplete |'a'|
//@[47:48) Colon |:|
//@[49:50) Identifier |b|
//@[51:52) RightBrace |}|
//@[52:53) Dot |.|
//@[53:54) Identifier |a|
//@[55:57) Equals |==|
//@[58:62) TrueKeyword |true|
//@[62:63) RightParen |)|
//@[64:65) LeftBrace |{|
//@[65:67) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoInputs './modulea.bicep' = {
//@[0:6) Identifier |module|
//@[7:19) Identifier |modANoInputs|
//@[20:37) StringComplete |'./modulea.bicep'|
//@[38:39) Assignment |=|
//@[40:41) LeftBrace |{|
//@[41:42) NewLine |\n|
  name: 'modANoInputs'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'modANoInputs'|
//@[22:23) NewLine |\n|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
//@[65:66) NewLine |\n|
  
//@[2:3) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[0:6) Identifier |module|
//@[7:32) Identifier |modANoInputsWithCondition|
//@[33:50) StringComplete |'./modulea.bicep'|
//@[51:52) Assignment |=|
//@[53:55) Identifier |if|
//@[56:57) LeftParen |(|
//@[57:63) Identifier |length|
//@[63:64) LeftParen |(|
//@[64:65) LeftSquare |[|
//@[65:66) NewLine |\n|
  'foo'
//@[2:7) StringComplete |'foo'|
//@[7:8) NewLine |\n|
]) == 1) {
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[3:5) Equals |==|
//@[6:7) Number |1|
//@[7:8) RightParen |)|
//@[9:10) LeftBrace |{|
//@[10:11) NewLine |\n|
  name: 'modANoInputs'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'modANoInputs'|
//@[22:23) NewLine |\n|
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
//@[78:79) NewLine |\n|
  
//@[2:3) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modAEmptyInputs './modulea.bicep' = {
//@[0:6) Identifier |module|
//@[7:22) Identifier |modAEmptyInputs|
//@[23:40) StringComplete |'./modulea.bicep'|
//@[41:42) Assignment |=|
//@[43:44) LeftBrace |{|
//@[44:45) NewLine |\n|
  name: 'modANoInputs'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'modANoInputs'|
//@[22:23) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    // #completionTest(0,1,2,3,4) -> moduleAParams
//@[50:51) NewLine |\n|
    
//@[4:5) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
//@[0:6) Identifier |module|
//@[7:35) Identifier |modAEmptyInputsWithCondition|
//@[36:53) StringComplete |'./modulea.bicep'|
//@[54:55) Assignment |=|
//@[56:58) Identifier |if|
//@[59:60) LeftParen |(|
//@[60:61) Number |1|
//@[62:63) Plus |+|
//@[64:65) Number |2|
//@[66:68) Equals |==|
//@[69:70) Number |2|
//@[70:71) RightParen |)|
//@[72:73) LeftBrace |{|
//@[73:74) NewLine |\n|
  name: 'modANoInputs'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'modANoInputs'|
//@[22:23) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
//@[63:64) NewLine |\n|
    
//@[4:5) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(55) -> moduleATopLevelPropertyAccess
//@[55:56) NewLine |\n|
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[0:3) Identifier |var|
//@[4:35) Identifier |modulePropertyAccessCompletions|
//@[36:37) Assignment |=|
//@[38:53) Identifier |modAEmptyInputs|
//@[53:54) Dot |.|
//@[54:55) Identifier |o|
//@[55:57) NewLine |\n\n|

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
//@[68:69) NewLine |\n|
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[0:3) Identifier |var|
//@[4:48) Identifier |moduleWithConditionPropertyAccessCompletions|
//@[49:50) Assignment |=|
//@[51:79) Identifier |modAEmptyInputsWithCondition|
//@[79:80) Dot |.|
//@[80:81) Identifier |o|
//@[81:83) NewLine |\n\n|

// #completionTest(56) -> moduleAOutputs
//@[40:41) NewLine |\n|
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[0:3) Identifier |var|
//@[4:28) Identifier |moduleOutputsCompletions|
//@[29:30) Assignment |=|
//@[31:46) Identifier |modAEmptyInputs|
//@[46:47) Dot |.|
//@[47:54) Identifier |outputs|
//@[54:55) Dot |.|
//@[55:56) Identifier |s|
//@[56:58) NewLine |\n\n|

// #completionTest(82) -> moduleAWithConditionOutputs
//@[53:54) NewLine |\n|
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[0:3) Identifier |var|
//@[4:41) Identifier |moduleWithConditionOutputsCompletions|
//@[42:43) Assignment |=|
//@[44:72) Identifier |modAEmptyInputsWithCondition|
//@[72:73) Dot |.|
//@[73:80) Identifier |outputs|
//@[80:81) Dot |.|
//@[81:82) Identifier |s|
//@[82:84) NewLine |\n\n|

module modAUnspecifiedInputs './modulea.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |modAUnspecifiedInputs|
//@[29:46) StringComplete |'./modulea.bicep'|
//@[47:48) Assignment |=|
//@[49:50) LeftBrace |{|
//@[50:51) NewLine |\n|
  name: 'modAUnspecifiedInputs'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:31) StringComplete |'modAUnspecifiedInputs'|
//@[31:32) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    stringParamB: ''
//@[4:16) Identifier |stringParamB|
//@[16:17) Colon |:|
//@[18:20) StringComplete |''|
//@[20:21) NewLine |\n|
    objParam: {}
//@[4:12) Identifier |objParam|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) RightBrace |}|
//@[16:17) NewLine |\n|
    objArray: []
//@[4:12) Identifier |objArray|
//@[12:13) Colon |:|
//@[14:15) LeftSquare |[|
//@[15:16) RightSquare |]|
//@[16:17) NewLine |\n|
    unspecifiedInput: ''
//@[4:20) Identifier |unspecifiedInput|
//@[20:21) Colon |:|
//@[22:24) StringComplete |''|
//@[24:25) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[0:3) Identifier |var|
//@[4:21) Identifier |unspecifiedOutput|
//@[22:23) Assignment |=|
//@[24:45) Identifier |modAUnspecifiedInputs|
//@[45:46) Dot |.|
//@[46:53) Identifier |outputs|
//@[53:54) Dot |.|
//@[54:58) Identifier |test|
//@[58:60) NewLine |\n\n|

module modCycle './cycle.bicep' = {
//@[0:6) Identifier |module|
//@[7:15) Identifier |modCycle|
//@[16:31) StringComplete |'./cycle.bicep'|
//@[32:33) Assignment |=|
//@[34:35) LeftBrace |{|
//@[35:36) NewLine |\n|
  
//@[2:3) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithEmptyPath '' = {
//@[0:6) Identifier |module|
//@[7:26) Identifier |moduleWithEmptyPath|
//@[27:29) StringComplete |''|
//@[30:31) Assignment |=|
//@[32:33) LeftBrace |{|
//@[33:34) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[0:6) Identifier |module|
//@[7:29) Identifier |moduleWithAbsolutePath|
//@[30:46) StringComplete |'/abc/def.bicep'|
//@[47:48) Assignment |=|
//@[49:50) LeftBrace |{|
//@[50:51) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithBackslash 'child\\file.bicep' = {
//@[0:6) Identifier |module|
//@[7:26) Identifier |moduleWithBackslash|
//@[27:46) StringComplete |'child\\file.bicep'|
//@[47:48) Assignment |=|
//@[49:50) LeftBrace |{|
//@[50:51) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |moduleWithInvalidChar|
//@[29:48) StringComplete |'child/fi|le.bicep'|
//@[49:50) Assignment |=|
//@[51:52) LeftBrace |{|
//@[52:53) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[0:6) Identifier |module|
//@[7:38) Identifier |moduleWithInvalidTerminatorChar|
//@[39:52) StringComplete |'child/test.'|
//@[53:54) Assignment |=|
//@[55:56) LeftBrace |{|
//@[56:57) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithValidScope './empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:27) Identifier |moduleWithValidScope|
//@[28:43) StringComplete |'./empty.bicep'|
//@[44:45) Assignment |=|
//@[46:47) LeftBrace |{|
//@[47:48) NewLine |\n|
  name: 'moduleWithValidScope'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:30) StringComplete |'moduleWithValidScope'|
//@[30:31) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidScope './empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:29) Identifier |moduleWithInvalidScope|
//@[30:45) StringComplete |'./empty.bicep'|
//@[46:47) Assignment |=|
//@[48:49) LeftBrace |{|
//@[49:50) NewLine |\n|
  name: 'moduleWithInvalidScope'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:32) StringComplete |'moduleWithInvalidScope'|
//@[32:33) NewLine |\n|
  scope: moduleWithValidScope
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:29) Identifier |moduleWithValidScope|
//@[29:30) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:37) Identifier |moduleWithMissingRequiredScope|
//@[38:66) StringComplete |'./subscription_empty.bicep'|
//@[67:68) Assignment |=|
//@[69:70) LeftBrace |{|
//@[70:71) NewLine |\n|
  name: 'moduleWithMissingRequiredScope'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:40) StringComplete |'moduleWithMissingRequiredScope'|
//@[40:41) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidScope2 './empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:30) Identifier |moduleWithInvalidScope2|
//@[31:46) StringComplete |'./empty.bicep'|
//@[47:48) Assignment |=|
//@[49:50) LeftBrace |{|
//@[50:51) NewLine |\n|
  name: 'moduleWithInvalidScope2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:33) StringComplete |'moduleWithInvalidScope2'|
//@[33:34) NewLine |\n|
  scope: managementGroup()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:24) Identifier |managementGroup|
//@[24:25) LeftParen |(|
//@[25:26) RightParen |)|
//@[26:27) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithBadScope './empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:25) Identifier |moduleWithBadScope|
//@[26:41) StringComplete |'./empty.bicep'|
//@[42:43) Assignment |=|
//@[44:45) LeftBrace |{|
//@[45:46) NewLine |\n|
  name: 'moduleWithBadScope'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:28) StringComplete |'moduleWithBadScope'|
//@[28:29) NewLine |\n|
  scope: 'stringScope'
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:22) StringComplete |'stringScope'|
//@[22:23) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes1|
//@[26:72) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74) Assignment |=|
//@[75:76) LeftBrace |{|
//@[76:77) NewLine |\n|
  name: 'runtimeValidRes1Name'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:30) StringComplete |'runtimeValidRes1Name'|
//@[30:31) NewLine |\n|
  location: 'westeurope'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:24) StringComplete |'westeurope'|
//@[24:25) NewLine |\n|
  kind: 'Storage'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:17) StringComplete |'Storage'|
//@[17:18) NewLine |\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
    name: 'Standard_GRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:24) StringComplete |'Standard_GRS'|
//@[24:25) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeValidModule1 'empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:26) Identifier |runtimeValidModule1|
//@[27:40) StringComplete |'empty.bicep'|
//@[41:42) Assignment |=|
//@[43:44) LeftBrace |{|
//@[44:45) NewLine |\n|
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |concat|
//@[14:15) LeftParen |(|
//@[15:21) Identifier |concat|
//@[21:22) LeftParen |(|
//@[22:38) Identifier |runtimeValidRes1|
//@[38:39) Dot |.|
//@[39:41) Identifier |id|
//@[41:42) Comma |,|
//@[43:59) Identifier |runtimeValidRes1|
//@[59:60) Dot |.|
//@[60:64) Identifier |name|
//@[64:65) RightParen |)|
//@[65:66) Comma |,|
//@[67:83) Identifier |runtimeValidRes1|
//@[83:84) Dot |.|
//@[84:88) Identifier |type|
//@[88:89) RightParen |)|
//@[89:90) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule1 'empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |runtimeInvalidModule1|
//@[29:42) StringComplete |'empty.bicep'|
//@[43:44) Assignment |=|
//@[45:46) LeftBrace |{|
//@[46:47) NewLine |\n|
  name: runtimeValidRes1.location
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:33) Identifier |location|
//@[33:34) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule2 'empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |runtimeInvalidModule2|
//@[29:42) StringComplete |'empty.bicep'|
//@[43:44) Assignment |=|
//@[45:46) LeftBrace |{|
//@[46:47) NewLine |\n|
  name: runtimeValidRes1['location']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) LeftSquare |[|
//@[25:35) StringComplete |'location'|
//@[35:36) RightSquare |]|
//@[36:37) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule3 'empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |runtimeInvalidModule3|
//@[29:42) StringComplete |'empty.bicep'|
//@[43:44) Assignment |=|
//@[45:46) LeftBrace |{|
//@[46:47) NewLine |\n|
  name: runtimeValidRes1.sku.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:28) Identifier |sku|
//@[28:29) Dot |.|
//@[29:33) Identifier |name|
//@[33:34) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule4 'empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |runtimeInvalidModule4|
//@[29:42) StringComplete |'empty.bicep'|
//@[43:44) Assignment |=|
//@[45:46) LeftBrace |{|
//@[46:47) NewLine |\n|
  name: runtimeValidRes1.sku['name']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:28) Identifier |sku|
//@[28:29) LeftSquare |[|
//@[29:35) StringComplete |'name'|
//@[35:36) RightSquare |]|
//@[36:37) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule5 'empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |runtimeInvalidModule5|
//@[29:42) StringComplete |'empty.bicep'|
//@[43:44) Assignment |=|
//@[45:46) LeftBrace |{|
//@[46:47) NewLine |\n|
  name: runtimeValidRes1['sku']['name']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) LeftSquare |[|
//@[25:30) StringComplete |'sku'|
//@[30:31) RightSquare |]|
//@[31:32) LeftSquare |[|
//@[32:38) StringComplete |'name'|
//@[38:39) RightSquare |]|
//@[39:40) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule6 'empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |runtimeInvalidModule6|
//@[29:42) StringComplete |'empty.bicep'|
//@[43:44) Assignment |=|
//@[45:46) LeftBrace |{|
//@[46:47) NewLine |\n|
  name: runtimeValidRes1['sku'].name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) LeftSquare |[|
//@[25:30) StringComplete |'sku'|
//@[30:31) RightSquare |]|
//@[31:32) Dot |.|
//@[32:36) Identifier |name|
//@[36:37) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithDuplicateName1 './empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:31) Identifier |moduleWithDuplicateName1|
//@[32:47) StringComplete |'./empty.bicep'|
//@[48:49) Assignment |=|
//@[50:51) LeftBrace |{|
//@[51:52) NewLine |\n|
  name: 'moduleWithDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:33) StringComplete |'moduleWithDuplicateName'|
//@[33:34) NewLine |\n|
  scope: resourceGroup()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:24) RightParen |)|
//@[24:25) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithDuplicateName2 './empty.bicep' = {
//@[0:6) Identifier |module|
//@[7:31) Identifier |moduleWithDuplicateName2|
//@[32:47) StringComplete |'./empty.bicep'|
//@[48:49) Assignment |=|
//@[50:51) LeftBrace |{|
//@[51:52) NewLine |\n|
  name: 'moduleWithDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:33) StringComplete |'moduleWithDuplicateName'|
//@[33:34) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdCompletions
//@[48:49) NewLine |\n|
module completionB ''
//@[0:6) Identifier |module|
//@[7:18) Identifier |completionB|
//@[19:21) StringComplete |''|
//@[21:23) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdCompletions
//@[48:49) NewLine |\n|
module completionC '' =
//@[0:6) Identifier |module|
//@[7:18) Identifier |completionC|
//@[19:21) StringComplete |''|
//@[22:23) Assignment |=|
//@[23:25) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdCompletions
//@[48:49) NewLine |\n|
module completionD '' = {}
//@[0:6) Identifier |module|
//@[7:18) Identifier |completionD|
//@[19:21) StringComplete |''|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:26) RightBrace |}|
//@[26:28) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdCompletions
//@[48:49) NewLine |\n|
module completionE '' = {
//@[0:6) Identifier |module|
//@[7:18) Identifier |completionE|
//@[19:21) StringComplete |''|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
  name: 'hello'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) StringComplete |'hello'|
//@[15:16) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
//@[56:57) NewLine |\n|
module cwdFileCompletionA '.'
//@[0:6) Identifier |module|
//@[7:25) Identifier |cwdFileCompletionA|
//@[26:29) StringComplete |'.'|
//@[29:31) NewLine |\n\n|

// #completionTest(26, 27) -> cwdMCompletions
//@[45:46) NewLine |\n|
module cwdFileCompletionB m
//@[0:6) Identifier |module|
//@[7:25) Identifier |cwdFileCompletionB|
//@[26:27) Identifier |m|
//@[27:29) NewLine |\n\n|

// #completionTest(26, 27, 28, 29) -> cwdMCompletions
//@[53:54) NewLine |\n|
module cwdFileCompletionC 'm'
//@[0:6) Identifier |module|
//@[7:25) Identifier |cwdFileCompletionC|
//@[26:29) StringComplete |'m'|
//@[29:31) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childCompletions
//@[102:103) NewLine |\n|
module childCompletionA 'ChildModules/'
//@[0:6) Identifier |module|
//@[7:23) Identifier |childCompletionA|
//@[24:39) StringComplete |'ChildModules/'|
//@[39:41) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotCompletions
//@[105:106) NewLine |\n|
module childCompletionB './ChildModules/'
//@[0:6) Identifier |module|
//@[7:23) Identifier |childCompletionB|
//@[24:41) StringComplete |'./ChildModules/'|
//@[41:43) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childMCompletions
//@[107:108) NewLine |\n|
module childCompletionC './ChildModules/m'
//@[0:6) Identifier |module|
//@[7:23) Identifier |childCompletionC|
//@[24:42) StringComplete |'./ChildModules/m'|
//@[42:44) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childECompletions
//@[107:108) NewLine |\n|
module childCompletionD 'ChildModules/e'
//@[0:6) Identifier |module|
//@[7:23) Identifier |childCompletionD|
//@[24:40) StringComplete |'ChildModules/e'|
//@[40:40) EndOfFile ||
