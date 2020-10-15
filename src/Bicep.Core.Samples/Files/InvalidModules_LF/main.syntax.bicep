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

module modANoInputs './modulea.bicep' = {
//@[0:46) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |modANoInputs|
//@[20:37)  StringSyntax
//@[20:37)   StringComplete |'./modulea.bicep'|
//@[38:39)  Assignment |=|
//@[40:46)  ObjectSyntax
//@[40:41)   LeftBrace |{|
//@[41:42)   NewLine |\n|
  
//@[2:3)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modAUnspecifiedInputs './modulea.bicep' = {
//@[0:124) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |modAUnspecifiedInputs|
//@[29:46)  StringSyntax
//@[29:46)   StringComplete |'./modulea.bicep'|
//@[47:48)  Assignment |=|
//@[49:124)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
  stringParamB: ''
//@[2:18)   ObjectPropertySyntax
//@[2:14)    IdentifierSyntax
//@[2:14)     Identifier |stringParamB|
//@[14:15)    Colon |:|
//@[16:18)    StringSyntax
//@[16:18)     StringComplete |''|
//@[18:19)   NewLine |\n|
  objParam: {}
//@[2:14)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |objParam|
//@[10:11)    Colon |:|
//@[12:14)    ObjectSyntax
//@[12:13)     LeftBrace |{|
//@[13:14)     RightBrace |}|
//@[14:15)   NewLine |\n|
  objArray: []
//@[2:14)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |objArray|
//@[10:11)    Colon |:|
//@[12:14)    ArraySyntax
//@[12:13)     LeftSquare |[|
//@[13:14)     RightSquare |]|
//@[14:15)   NewLine |\n|
  unspecifiedInput: ''
//@[2:22)   ObjectPropertySyntax
//@[2:18)    IdentifierSyntax
//@[2:18)     Identifier |unspecifiedInput|
//@[18:19)    Colon |:|
//@[20:22)    StringSyntax
//@[20:22)     StringComplete |''|
//@[22:23)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var unspecifiedOutput = modAUnspecifiedInputs.test
//@[0:50) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |unspecifiedOutput|
//@[22:23)  Assignment |=|
//@[24:50)  PropertyAccessSyntax
//@[24:45)   VariableAccessSyntax
//@[24:45)    IdentifierSyntax
//@[24:45)     Identifier |modAUnspecifiedInputs|
//@[45:46)   Dot |.|
//@[46:50)   IdentifierSyntax
//@[46:50)    Identifier |test|
//@[50:52) NewLine |\n\n|

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
