module nonExistentFileRef './nonExistent.bicep' = {
//@[0:54) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |nonExistentFileRef|
//@[26:47)  StringSyntax
//@[26:47)   StringComplete |'./nonExistent.bicep'|
//@[48:49)  Assignment |=|
//@[50:54)  ObjectSyntax
//@[50:51)   LeftBrace |{|
//@[51:53)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// we should only look this file up once, but should still return the same failure
//@[82:83) NewLine |\n|
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[0:63) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:34)  IdentifierSyntax
//@[7:34)   Identifier |nonExistentFileRefDuplicate|
//@[35:56)  StringSyntax
//@[35:56)   StringComplete |'./nonExistent.bicep'|
//@[57:58)  Assignment |=|
//@[59:63)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:62)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// we should only look this file up once, but should still return the same failure
//@[82:83) NewLine |\n|
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[0:80) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |nonExistentFileRefEquivalentPath|
//@[40:73)  StringSyntax
//@[40:73)   StringComplete |'abc/def/../../nonExistent.bicep'|
//@[74:75)  Assignment |=|
//@[76:80)  ObjectSyntax
//@[76:77)   LeftBrace |{|
//@[77:79)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithoutPath = {
//@[0:28) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:24)  IdentifierSyntax
//@[7:24)   Identifier |moduleWithoutPath|
//@[25:28)  SkippedTriviaSyntax
//@[25:26)   Assignment |=|
//@[27:28)   LeftBrace |{|
//@[28:28)  SkippedTriviaSyntax
//@[28:28)  SkippedTriviaSyntax
//@[28:30) NewLine |\n\n|

}
//@[0:1) SkippedTriviaSyntax
//@[0:1)  RightBrace |}|
//@[1:3) NewLine |\n\n|

// missing identifier #completionTest(7) -> empty
//@[49:50) NewLine |\n|
module 
//@[0:7) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:7)  SkippedTriviaSyntax
//@[7:7)  SkippedTriviaSyntax
//@[7:7)  SkippedTriviaSyntax
//@[7:9) NewLine |\n\n|

// #completionTest(24,25) -> object
//@[35:36) NewLine |\n|
module missingValue '' = 
//@[0:25) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |missingValue|
//@[20:22)  StringSyntax
//@[20:22)   StringComplete |''|
//@[23:24)  Assignment |=|
//@[25:25)  SkippedTriviaSyntax
//@[25:27) NewLine |\n\n|

var interp = 'hello'
//@[0:20) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:10)  IdentifierSyntax
//@[4:10)   Identifier |interp|
//@[11:12)  Assignment |=|
//@[13:20)  StringSyntax
//@[13:20)   StringComplete |'hello'|
//@[20:21) NewLine |\n|
module moduleWithInterpPath './${interp}.bicep' = {
//@[0:54) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |moduleWithInterpPath|
//@[28:47)  StringSyntax
//@[28:33)   StringLeftPiece |'./${|
//@[33:39)   VariableAccessSyntax
//@[33:39)    IdentifierSyntax
//@[33:39)     Identifier |interp|
//@[39:47)   StringRightPiece |}.bicep'|
//@[48:49)  Assignment |=|
//@[50:54)  ObjectSyntax
//@[50:51)   LeftBrace |{|
//@[51:53)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithSelfCycle './main.bicep' = {
//@[0:48) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |moduleWithSelfCycle|
//@[27:41)  StringSyntax
//@[27:41)   StringComplete |'./main.bicep'|
//@[42:43)  Assignment |=|
//@[44:48)  ObjectSyntax
//@[44:45)   LeftBrace |{|
//@[45:47)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = {
//@[0:28) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:28)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:27)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoName './modulea.bicep' = {
//@[0:93) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |modANoName|
//@[18:35)  StringSyntax
//@[18:35)   StringComplete |'./modulea.bicep'|
//@[36:37)  Assignment |=|
//@[38:93)  ObjectSyntax
//@[38:39)   LeftBrace |{|
//@[39:40)   NewLine |\n|
// #completionTest(0) -> moduleATopLevelProperties
//@[50:52)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoInputs './modulea.bicep' = {
//@[0:135) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |modANoInputs|
//@[20:37)  StringSyntax
//@[20:37)   StringComplete |'./modulea.bicep'|
//@[38:39)  Assignment |=|
//@[40:135)  ObjectSyntax
//@[40:41)   LeftBrace |{|
//@[41:42)   NewLine |\n|
  name: 'modANoInputs'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:22)    StringSyntax
//@[8:22)     StringComplete |'modANoInputs'|
//@[22:23)   NewLine |\n|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
//@[65:66)   NewLine |\n|
  
//@[2:3)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modAEmptyInputs './modulea.bicep' = {
//@[0:141) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |modAEmptyInputs|
//@[23:40)  StringSyntax
//@[23:40)   StringComplete |'./modulea.bicep'|
//@[41:42)  Assignment |=|
//@[43:141)  ObjectSyntax
//@[43:44)   LeftBrace |{|
//@[44:45)   NewLine |\n|
  name: 'modANoInputs'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:22)    StringSyntax
//@[8:22)     StringComplete |'modANoInputs'|
//@[22:23)   NewLine |\n|
  params: {
//@[2:71)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:71)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    // #completionTest(0,1,2,3,4) -> moduleAParams
//@[50:51)     NewLine |\n|
    
//@[4:5)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(55) -> moduleATopLevelPropertyAccess
//@[55:56) NewLine |\n|
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[0:55) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:35)  IdentifierSyntax
//@[4:35)   Identifier |modulePropertyAccessCompletions|
//@[36:37)  Assignment |=|
//@[38:55)  PropertyAccessSyntax
//@[38:53)   VariableAccessSyntax
//@[38:53)    IdentifierSyntax
//@[38:53)     Identifier |modAEmptyInputs|
//@[53:54)   Dot |.|
//@[54:55)   IdentifierSyntax
//@[54:55)    Identifier |o|
//@[55:57) NewLine |\n\n|

// #completionTest(56) -> moduleAOutputs
//@[40:41) NewLine |\n|
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[0:56) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |moduleOutputsCompletions|
//@[29:30)  Assignment |=|
//@[31:56)  PropertyAccessSyntax
//@[31:54)   PropertyAccessSyntax
//@[31:46)    VariableAccessSyntax
//@[31:46)     IdentifierSyntax
//@[31:46)      Identifier |modAEmptyInputs|
//@[46:47)    Dot |.|
//@[47:54)    IdentifierSyntax
//@[47:54)     Identifier |outputs|
//@[54:55)   Dot |.|
//@[55:56)   IdentifierSyntax
//@[55:56)    Identifier |s|
//@[56:58) NewLine |\n\n|

module modAUnspecifiedInputs './modulea.bicep' = {
//@[0:180) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |modAUnspecifiedInputs|
//@[29:46)  StringSyntax
//@[29:46)   StringComplete |'./modulea.bicep'|
//@[47:48)  Assignment |=|
//@[49:180)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
  name: 'modAUnspecifiedInputs'
//@[2:31)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:31)    StringSyntax
//@[8:31)     StringComplete |'modAUnspecifiedInputs'|
//@[31:32)   NewLine |\n|
  params: {
//@[2:95)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:95)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    stringParamB: ''
//@[4:20)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |stringParamB|
//@[16:17)      Colon |:|
//@[18:20)      StringSyntax
//@[18:20)       StringComplete |''|
//@[20:21)     NewLine |\n|
    objParam: {}
//@[4:16)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |objParam|
//@[12:13)      Colon |:|
//@[14:16)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:16)       RightBrace |}|
//@[16:17)     NewLine |\n|
    objArray: []
//@[4:16)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |objArray|
//@[12:13)      Colon |:|
//@[14:16)      ArraySyntax
//@[14:15)       LeftSquare |[|
//@[15:16)       RightSquare |]|
//@[16:17)     NewLine |\n|
    unspecifiedInput: ''
//@[4:24)     ObjectPropertySyntax
//@[4:20)      IdentifierSyntax
//@[4:20)       Identifier |unspecifiedInput|
//@[20:21)      Colon |:|
//@[22:24)      StringSyntax
//@[22:24)       StringComplete |''|
//@[24:25)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[0:58) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |unspecifiedOutput|
//@[22:23)  Assignment |=|
//@[24:58)  PropertyAccessSyntax
//@[24:53)   PropertyAccessSyntax
//@[24:45)    VariableAccessSyntax
//@[24:45)     IdentifierSyntax
//@[24:45)      Identifier |modAUnspecifiedInputs|
//@[45:46)    Dot |.|
//@[46:53)    IdentifierSyntax
//@[46:53)     Identifier |outputs|
//@[53:54)   Dot |.|
//@[54:58)   IdentifierSyntax
//@[54:58)    Identifier |test|
//@[58:60) NewLine |\n\n|

module modCycle './cycle.bicep' = {
//@[0:40) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:15)  IdentifierSyntax
//@[7:15)   Identifier |modCycle|
//@[16:31)  StringSyntax
//@[16:31)   StringComplete |'./cycle.bicep'|
//@[32:33)  Assignment |=|
//@[34:40)  ObjectSyntax
//@[34:35)   LeftBrace |{|
//@[35:36)   NewLine |\n|
  
//@[2:3)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithEmptyPath '' = {
//@[0:35) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |moduleWithEmptyPath|
//@[27:29)  StringSyntax
//@[27:29)   StringComplete |''|
//@[30:31)  Assignment |=|
//@[32:35)  ObjectSyntax
//@[32:33)   LeftBrace |{|
//@[33:34)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[0:52) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:29)  IdentifierSyntax
//@[7:29)   Identifier |moduleWithAbsolutePath|
//@[30:46)  StringSyntax
//@[30:46)   StringComplete |'/abc/def.bicep'|
//@[47:48)  Assignment |=|
//@[49:52)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithBackslash 'child\\file.bicep' = {
//@[0:52) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |moduleWithBackslash|
//@[27:46)  StringSyntax
//@[27:46)   StringComplete |'child\\file.bicep'|
//@[47:48)  Assignment |=|
//@[49:52)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[0:54) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |moduleWithInvalidChar|
//@[29:48)  StringSyntax
//@[29:48)   StringComplete |'child/fi|le.bicep'|
//@[49:50)  Assignment |=|
//@[51:54)  ObjectSyntax
//@[51:52)   LeftBrace |{|
//@[52:53)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[0:58) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:38)  IdentifierSyntax
//@[7:38)   Identifier |moduleWithInvalidTerminatorChar|
//@[39:52)  StringSyntax
//@[39:52)   StringComplete |'child/test.'|
//@[53:54)  Assignment |=|
//@[55:58)  ObjectSyntax
//@[55:56)   LeftBrace |{|
//@[56:57)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithValidScope './empty.bicep' = {
//@[0:80) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |moduleWithValidScope|
//@[28:43)  StringSyntax
//@[28:43)   StringComplete |'./empty.bicep'|
//@[44:45)  Assignment |=|
//@[46:80)  ObjectSyntax
//@[46:47)   LeftBrace |{|
//@[47:48)   NewLine |\n|
  name: 'moduleWithValidScope'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:30)     StringComplete |'moduleWithValidScope'|
//@[30:31)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidScope './empty.bicep' = {
//@[0:114) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:29)  IdentifierSyntax
//@[7:29)   Identifier |moduleWithInvalidScope|
//@[30:45)  StringSyntax
//@[30:45)   StringComplete |'./empty.bicep'|
//@[46:47)  Assignment |=|
//@[48:114)  ObjectSyntax
//@[48:49)   LeftBrace |{|
//@[49:50)   NewLine |\n|
  name: 'moduleWithInvalidScope'
//@[2:32)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:32)    StringSyntax
//@[8:32)     StringComplete |'moduleWithInvalidScope'|
//@[32:33)   NewLine |\n|
  scope: moduleWithValidScope
//@[2:29)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:29)    VariableAccessSyntax
//@[9:29)     IdentifierSyntax
//@[9:29)      Identifier |moduleWithValidScope|
//@[29:30)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[0:113) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:37)  IdentifierSyntax
//@[7:37)   Identifier |moduleWithMissingRequiredScope|
//@[38:66)  StringSyntax
//@[38:66)   StringComplete |'./subscription_empty.bicep'|
//@[67:68)  Assignment |=|
//@[69:113)  ObjectSyntax
//@[69:70)   LeftBrace |{|
//@[70:71)   NewLine |\n|
  name: 'moduleWithMissingRequiredScope'
//@[2:40)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:40)    StringSyntax
//@[8:40)     StringComplete |'moduleWithMissingRequiredScope'|
//@[40:41)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidScope2 './empty.bicep' = {
//@[0:113) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:30)  IdentifierSyntax
//@[7:30)   Identifier |moduleWithInvalidScope2|
//@[31:46)  StringSyntax
//@[31:46)   StringComplete |'./empty.bicep'|
//@[47:48)  Assignment |=|
//@[49:113)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
  name: 'moduleWithInvalidScope2'
//@[2:33)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:33)    StringSyntax
//@[8:33)     StringComplete |'moduleWithInvalidScope2'|
//@[33:34)   NewLine |\n|
  scope: managementGroup()
//@[2:26)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:26)    FunctionCallSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |managementGroup|
//@[24:25)     LeftParen |(|
//@[25:26)     RightParen |)|
//@[26:27)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithBadScope './empty.bicep' = {
//@[0:99) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |moduleWithBadScope|
//@[26:41)  StringSyntax
//@[26:41)   StringComplete |'./empty.bicep'|
//@[42:43)  Assignment |=|
//@[44:99)  ObjectSyntax
//@[44:45)   LeftBrace |{|
//@[45:46)   NewLine |\n|
  name: 'moduleWithBadScope'
//@[2:28)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:28)    StringSyntax
//@[8:28)     StringComplete |'moduleWithBadScope'|
//@[28:29)   NewLine |\n|
  scope: 'stringScope'
//@[2:22)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:22)    StringSyntax
//@[9:22)     StringComplete |'stringScope'|
//@[22:23)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:1) EndOfFile ||
