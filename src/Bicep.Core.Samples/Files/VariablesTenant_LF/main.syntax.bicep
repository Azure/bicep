targetScope='tenant'
//@[0:20) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[11:12)  Assignment |=|
//@[12:20)  StringSyntax
//@[12:20)   StringComplete |'tenant'|
//@[20:22) NewLine |\n\n|

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
//@[46:46) EndOfFile ||
