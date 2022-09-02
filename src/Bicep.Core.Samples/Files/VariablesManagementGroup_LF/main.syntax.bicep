targetScope='managementGroup'
//@[00:173) ProgramSyntax
//@[00:029) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[11:012) | ├─Token(Assignment) |=|
//@[12:029) | └─StringSyntax
//@[12:029) |   └─Token(StringComplete) |'managementGroup'|
//@[29:031) ├─Token(NewLine) |\n\n|

var deploymentLocation = deployment().location
//@[00:046) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:022) | ├─IdentifierSyntax
//@[04:022) | | └─Token(Identifier) |deploymentLocation|
//@[23:024) | ├─Token(Assignment) |=|
//@[25:046) | └─PropertyAccessSyntax
//@[25:037) |   ├─FunctionCallSyntax
//@[25:035) |   | ├─IdentifierSyntax
//@[25:035) |   | | └─Token(Identifier) |deployment|
//@[35:036) |   | ├─Token(LeftParen) |(|
//@[36:037) |   | └─Token(RightParen) |)|
//@[37:038) |   ├─Token(Dot) |.|
//@[38:046) |   └─IdentifierSyntax
//@[38:046) |     └─Token(Identifier) |location|
//@[46:048) ├─Token(NewLine) |\n\n|

var scopesWithArmRepresentation = {
//@[00:093) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:031) | ├─IdentifierSyntax
//@[04:031) | | └─Token(Identifier) |scopesWithArmRepresentation|
//@[32:033) | ├─Token(Assignment) |=|
//@[34:093) | └─ObjectSyntax
//@[34:035) |   ├─Token(LeftBrace) |{|
//@[35:036) |   ├─Token(NewLine) |\n|
  tenant: tenant()
//@[02:018) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |tenant|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:018) |   | └─FunctionCallSyntax
//@[10:016) |   |   ├─IdentifierSyntax
//@[10:016) |   |   | └─Token(Identifier) |tenant|
//@[16:017) |   |   ├─Token(LeftParen) |(|
//@[17:018) |   |   └─Token(RightParen) |)|
//@[18:019) |   ├─Token(NewLine) |\n|
  managementGroup: managementGroup()
//@[02:036) |   ├─ObjectPropertySyntax
//@[02:017) |   | ├─IdentifierSyntax
//@[02:017) |   | | └─Token(Identifier) |managementGroup|
//@[17:018) |   | ├─Token(Colon) |:|
//@[19:036) |   | └─FunctionCallSyntax
//@[19:034) |   |   ├─IdentifierSyntax
//@[19:034) |   |   | └─Token(Identifier) |managementGroup|
//@[34:035) |   |   ├─Token(LeftParen) |(|
//@[35:036) |   |   └─Token(RightParen) |)|
//@[36:037) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
