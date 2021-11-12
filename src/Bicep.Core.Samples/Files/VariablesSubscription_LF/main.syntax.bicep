targetScope='subscription'
//@[0:26) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[11:12)  Assignment |=|
//@[12:26)  StringSyntax
//@[12:26)   StringComplete |'subscription'|
//@[26:28) NewLine |\n\n|

var deploymentLocation = deployment().location
//@[0:46) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |deploymentLocation|
//@[23:24)  Assignment |=|
//@[25:46)  PropertyAccessSyntax
//@[25:37)   FunctionCallSyntax
//@[25:35)    IdentifierSyntax
//@[25:35)     Identifier |deployment|
//@[35:36)    LeftParen |(|
//@[36:37)    RightParen |)|
//@[37:38)   Dot |.|
//@[38:46)   IdentifierSyntax
//@[38:46)    Identifier |location|
//@[46:48) NewLine |\n\n|

var scopesWithArmRepresentation = {
//@[0:87) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:31)  IdentifierSyntax
//@[4:31)   Identifier |scopesWithArmRepresentation|
//@[32:33)  Assignment |=|
//@[34:87)  ObjectSyntax
//@[34:35)   LeftBrace |{|
//@[35:36)   NewLine |\n|
  tenant: tenant()
//@[2:18)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |tenant|
//@[8:9)    Colon |:|
//@[10:18)    FunctionCallSyntax
//@[10:16)     IdentifierSyntax
//@[10:16)      Identifier |tenant|
//@[16:17)     LeftParen |(|
//@[17:18)     RightParen |)|
//@[18:19)   NewLine |\n|
  subscription: subscription()
//@[2:30)   ObjectPropertySyntax
//@[2:14)    IdentifierSyntax
//@[2:14)     Identifier |subscription|
//@[14:15)    Colon |:|
//@[16:30)    FunctionCallSyntax
//@[16:28)     IdentifierSyntax
//@[16:28)      Identifier |subscription|
//@[28:29)     LeftParen |(|
//@[29:30)     RightParen |)|
//@[30:31)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
