targetScope='subscription'
//@[00:164) ProgramSyntax
//@[00:026) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[11:012) | ├─Token(Assignment) |=|
//@[12:026) | └─StringSyntax
//@[12:026) |   └─Token(StringComplete) |'subscription'|
//@[26:028) ├─Token(NewLine) |\n\n|

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
//@[00:087) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:031) | ├─IdentifierSyntax
//@[04:031) | | └─Token(Identifier) |scopesWithArmRepresentation|
//@[32:033) | ├─Token(Assignment) |=|
//@[34:087) | └─ObjectSyntax
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
  subscription: subscription()
//@[02:030) |   ├─ObjectPropertySyntax
//@[02:014) |   | ├─IdentifierSyntax
//@[02:014) |   | | └─Token(Identifier) |subscription|
//@[14:015) |   | ├─Token(Colon) |:|
//@[16:030) |   | └─FunctionCallSyntax
//@[16:028) |   |   ├─IdentifierSyntax
//@[16:028) |   |   | └─Token(Identifier) |subscription|
//@[28:029) |   |   ├─Token(LeftParen) |(|
//@[29:030) |   |   └─Token(RightParen) |)|
//@[30:031) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
