var subscriptionId = readCliArg('subscription-id')
//@[00:329) ProgramSyntax
//@[00:050) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |subscriptionId|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:050) | └─FunctionCallSyntax
//@[21:031) |   ├─IdentifierSyntax
//@[21:031) |   | └─Token(Identifier) |readCliArg|
//@[31:032) |   ├─Token(LeftParen) |(|
//@[32:049) |   ├─FunctionArgumentSyntax
//@[32:049) |   | └─StringSyntax
//@[32:049) |   |   └─Token(StringComplete) |'subscription-id'|
//@[49:050) |   └─Token(RightParen) |)|
//@[50:052) ├─Token(NewLine) |\r\n|
var resourceGroup = readCliArg('resource-group')
//@[00:048) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:017) | ├─IdentifierSyntax
//@[04:017) | | └─Token(Identifier) |resourceGroup|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:048) | └─FunctionCallSyntax
//@[20:030) |   ├─IdentifierSyntax
//@[20:030) |   | └─Token(Identifier) |readCliArg|
//@[30:031) |   ├─Token(LeftParen) |(|
//@[31:047) |   ├─FunctionArgumentSyntax
//@[31:047) |   | └─StringSyntax
//@[31:047) |   |   └─Token(StringComplete) |'resource-group'|
//@[47:048) |   └─Token(RightParen) |)|
//@[48:052) ├─Token(NewLine) |\r\n\r\n|

using 'main.bicep' with {
//@[00:223) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:018) | ├─StringSyntax
//@[06:018) | | └─Token(StringComplete) |'main.bicep'|
//@[19:223) | └─UsingWithClauseSyntax
//@[19:023) |   ├─Token(Identifier) |with|
//@[24:223) |   └─ObjectSyntax
//@[24:025) |     ├─Token(LeftBrace) |{|
//@[25:027) |     ├─Token(NewLine) |\r\n|
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
//@[02:075) |     ├─ObjectPropertySyntax
//@[02:007) |     | ├─IdentifierSyntax
//@[02:007) |     | | └─Token(Identifier) |scope|
//@[07:008) |     | ├─Token(Colon) |:|
//@[09:075) |     | └─StringSyntax
//@[09:027) |     |   ├─Token(StringLeftPiece) |'/subscriptions/${|
//@[27:041) |     |   ├─VariableAccessSyntax
//@[27:041) |     |   | └─IdentifierSyntax
//@[27:041) |     |   |   └─Token(Identifier) |subscriptionId|
//@[41:060) |     |   ├─Token(StringMiddlePiece) |}/resourceGroups/${|
//@[60:073) |     |   ├─VariableAccessSyntax
//@[60:073) |     |   | └─IdentifierSyntax
//@[60:073) |     |   |   └─Token(Identifier) |resourceGroup|
//@[73:075) |     |   └─Token(StringRightPiece) |}'|
//@[75:077) |     ├─Token(NewLine) |\r\n|
  actionOnUnmanage: {
//@[02:051) |     ├─ObjectPropertySyntax
//@[02:018) |     | ├─IdentifierSyntax
//@[02:018) |     | | └─Token(Identifier) |actionOnUnmanage|
//@[18:019) |     | ├─Token(Colon) |:|
//@[20:051) |     | └─ObjectSyntax
//@[20:021) |     |   ├─Token(LeftBrace) |{|
//@[21:023) |     |   ├─Token(NewLine) |\r\n|
    resources: 'delete'
//@[04:023) |     |   ├─ObjectPropertySyntax
//@[04:013) |     |   | ├─IdentifierSyntax
//@[04:013) |     |   | | └─Token(Identifier) |resources|
//@[13:014) |     |   | ├─Token(Colon) |:|
//@[15:023) |     |   | └─StringSyntax
//@[15:023) |     |   |   └─Token(StringComplete) |'delete'|
//@[23:025) |     |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |     |   └─Token(RightBrace) |}|
//@[03:005) |     ├─Token(NewLine) |\r\n|
  denySettings: {
//@[02:046) |     ├─ObjectPropertySyntax
//@[02:014) |     | ├─IdentifierSyntax
//@[02:014) |     | | └─Token(Identifier) |denySettings|
//@[14:015) |     | ├─Token(Colon) |:|
//@[16:046) |     | └─ObjectSyntax
//@[16:017) |     |   ├─Token(LeftBrace) |{|
//@[17:019) |     |   ├─Token(NewLine) |\r\n|
    mode: 'denyDelete'
//@[04:022) |     |   ├─ObjectPropertySyntax
//@[04:008) |     |   | ├─IdentifierSyntax
//@[04:008) |     |   | | └─Token(Identifier) |mode|
//@[08:009) |     |   | ├─Token(Colon) |:|
//@[10:022) |     |   | └─StringSyntax
//@[10:022) |     |   |   └─Token(StringComplete) |'denyDelete'|
//@[22:024) |     |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |     |   └─Token(RightBrace) |}|
//@[03:005) |     ├─Token(NewLine) |\r\n|
  mode: 'stack'
//@[02:015) |     ├─ObjectPropertySyntax
//@[02:006) |     | ├─IdentifierSyntax
//@[02:006) |     | | └─Token(Identifier) |mode|
//@[06:007) |     | ├─Token(Colon) |:|
//@[08:015) |     | └─StringSyntax
//@[08:015) |     |   └─Token(StringComplete) |'stack'|
//@[15:017) |     ├─Token(NewLine) |\r\n|
}
//@[00:001) |     └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|

//@[00:000) └─Token(EndOfFile) ||
