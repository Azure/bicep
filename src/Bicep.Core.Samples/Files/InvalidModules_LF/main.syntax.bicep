module moduleWithMissingPath './nonExistent.bicep' = {
//@[0:57) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |moduleWithMissingPath|
//@[29:50)  StringSyntax
//@[29:50)   StringComplete |'./nonExistent.bicep'|
//@[51:52)  Assignment |=|
//@[53:57)  ObjectSyntax
//@[53:54)   LeftBrace |{|
//@[54:56)   NewLine |\n\n|

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
//@[0:42) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |modANoName|
//@[18:35)  StringSyntax
//@[18:35)   StringComplete |'./modulea.bicep'|
//@[36:37)  Assignment |=|
//@[38:42)  ObjectSyntax
//@[38:39)   LeftBrace |{|
//@[39:41)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoInputs './modulea.bicep' = {
//@[0:66) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |modANoInputs|
//@[20:37)  StringSyntax
//@[20:37)   StringComplete |'./modulea.bicep'|
//@[38:39)  Assignment |=|
//@[40:66)  ObjectSyntax
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
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modAEmptyInputs './modulea.bicep' = {
//@[0:86) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |modAEmptyInputs|
//@[23:40)  StringSyntax
//@[23:40)   StringComplete |'./modulea.bicep'|
//@[41:42)  Assignment |=|
//@[43:86)  ObjectSyntax
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
//@[2:16)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:16)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:13)     NewLine |\n\n|

  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

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

module moduleWithBackslash 'child\\file.bicep' = {
//@[0:55) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |moduleWithBackslash|
//@[27:46)  StringSyntax
//@[27:46)   StringComplete |'child\\file.bicep'|
//@[47:48)  Assignment |=|
//@[49:55)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
  
//@[2:3)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

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
//@[1:1) EndOfFile ||
