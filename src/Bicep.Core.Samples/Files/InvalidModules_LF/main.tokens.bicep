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

module './main.bicep' = {
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:27) NewLine |\n\n|

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
//@[1:1) EndOfFile ||
