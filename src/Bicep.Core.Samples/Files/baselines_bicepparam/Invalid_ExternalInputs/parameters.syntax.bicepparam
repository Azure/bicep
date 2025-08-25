using none
//@[00:573) ProgramSyntax
//@[00:010) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:010) | ├─NoneLiteralSyntax
//@[06:010) | | └─Token(Identifier) |none|
//@[10:010) | └─SkippedTriviaSyntax
//@[10:014) ├─Token(NewLine) |\r\n\r\n|

var myVar = 1 + 2
//@[00:017) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:009) | ├─IdentifierSyntax
//@[04:009) | | └─Token(Identifier) |myVar|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:017) | └─BinaryOperationSyntax
//@[12:013) |   ├─IntegerLiteralSyntax
//@[12:013) |   | └─Token(Integer) |1|
//@[14:015) |   ├─Token(Plus) |+|
//@[16:017) |   └─IntegerLiteralSyntax
//@[16:017) |     └─Token(Integer) |2|
//@[17:019) ├─Token(NewLine) |\r\n|
param p = externalInput('sys.envVar', myVar)
//@[00:044) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:007) | ├─IdentifierSyntax
//@[06:007) | | └─Token(Identifier) |p|
//@[08:009) | ├─Token(Assignment) |=|
//@[10:044) | └─FunctionCallSyntax
//@[10:023) |   ├─IdentifierSyntax
//@[10:023) |   | └─Token(Identifier) |externalInput|
//@[23:024) |   ├─Token(LeftParen) |(|
//@[24:036) |   ├─FunctionArgumentSyntax
//@[24:036) |   | └─StringSyntax
//@[24:036) |   |   └─Token(StringComplete) |'sys.envVar'|
//@[36:037) |   ├─Token(Comma) |,|
//@[38:043) |   ├─FunctionArgumentSyntax
//@[38:043) |   | └─VariableAccessSyntax
//@[38:043) |   |   └─IdentifierSyntax
//@[38:043) |   |     └─Token(Identifier) |myVar|
//@[43:044) |   └─Token(RightParen) |)|
//@[44:048) ├─Token(NewLine) |\r\n\r\n|

var x = 42
//@[00:010) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:005) | ├─IdentifierSyntax
//@[04:005) | | └─Token(Identifier) |x|
//@[06:007) | ├─Token(Assignment) |=|
//@[08:010) | └─IntegerLiteralSyntax
//@[08:010) |   └─Token(Integer) |42|
//@[10:012) ├─Token(NewLine) |\r\n|
var myVar2 = 'abcd-${x}'
//@[00:024) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |myVar2|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:024) | └─StringSyntax
//@[13:021) |   ├─Token(StringLeftPiece) |'abcd-${|
//@[21:022) |   ├─VariableAccessSyntax
//@[21:022) |   | └─IdentifierSyntax
//@[21:022) |   |   └─Token(Identifier) |x|
//@[22:024) |   └─Token(StringRightPiece) |}'|
//@[24:026) ├─Token(NewLine) |\r\n|
param p2 = externalInput('sys.envVar', myVar2)
//@[00:046) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:008) | ├─IdentifierSyntax
//@[06:008) | | └─Token(Identifier) |p2|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:046) | └─FunctionCallSyntax
//@[11:024) |   ├─IdentifierSyntax
//@[11:024) |   | └─Token(Identifier) |externalInput|
//@[24:025) |   ├─Token(LeftParen) |(|
//@[25:037) |   ├─FunctionArgumentSyntax
//@[25:037) |   | └─StringSyntax
//@[25:037) |   |   └─Token(StringComplete) |'sys.envVar'|
//@[37:038) |   ├─Token(Comma) |,|
//@[39:045) |   ├─FunctionArgumentSyntax
//@[39:045) |   | └─VariableAccessSyntax
//@[39:045) |   |   └─IdentifierSyntax
//@[39:045) |   |     └─Token(Identifier) |myVar2|
//@[45:046) |   └─Token(RightParen) |)|
//@[46:050) ├─Token(NewLine) |\r\n\r\n|

var myVar3 = 'test'
//@[00:019) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |myVar3|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:019) | └─StringSyntax
//@[13:019) |   └─Token(StringComplete) |'test'|
//@[19:021) ├─Token(NewLine) |\r\n|
param p3 = externalInput(myVar3, myVar3)
//@[00:040) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:008) | ├─IdentifierSyntax
//@[06:008) | | └─Token(Identifier) |p3|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:040) | └─FunctionCallSyntax
//@[11:024) |   ├─IdentifierSyntax
//@[11:024) |   | └─Token(Identifier) |externalInput|
//@[24:025) |   ├─Token(LeftParen) |(|
//@[25:031) |   ├─FunctionArgumentSyntax
//@[25:031) |   | └─VariableAccessSyntax
//@[25:031) |   |   └─IdentifierSyntax
//@[25:031) |   |     └─Token(Identifier) |myVar3|
//@[31:032) |   ├─Token(Comma) |,|
//@[33:039) |   ├─FunctionArgumentSyntax
//@[33:039) |   | └─VariableAccessSyntax
//@[33:039) |   |   └─IdentifierSyntax
//@[33:039) |   |     └─Token(Identifier) |myVar3|
//@[39:040) |   └─Token(RightParen) |)|
//@[40:044) ├─Token(NewLine) |\r\n\r\n|

var myVar4 = {
//@[00:033) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |myVar4|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:033) | └─ObjectSyntax
//@[13:014) |   ├─Token(LeftBrace) |{|
//@[14:016) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[02:014) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:014) |   | └─StringSyntax
//@[08:014) |   |   └─Token(StringComplete) |'test'|
//@[14:016) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
param p4 = externalInput('sys.cli', myVar4)
//@[00:043) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:008) | ├─IdentifierSyntax
//@[06:008) | | └─Token(Identifier) |p4|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:043) | └─FunctionCallSyntax
//@[11:024) |   ├─IdentifierSyntax
//@[11:024) |   | └─Token(Identifier) |externalInput|
//@[24:025) |   ├─Token(LeftParen) |(|
//@[25:034) |   ├─FunctionArgumentSyntax
//@[25:034) |   | └─StringSyntax
//@[25:034) |   |   └─Token(StringComplete) |'sys.cli'|
//@[34:035) |   ├─Token(Comma) |,|
//@[36:042) |   ├─FunctionArgumentSyntax
//@[36:042) |   | └─VariableAccessSyntax
//@[36:042) |   |   └─IdentifierSyntax
//@[36:042) |   |     └─Token(Identifier) |myVar4|
//@[42:043) |   └─Token(RightParen) |)|
//@[43:047) ├─Token(NewLine) |\r\n\r\n|

var test = 'test'
//@[00:017) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:008) | ├─IdentifierSyntax
//@[04:008) | | └─Token(Identifier) |test|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:017) | └─StringSyntax
//@[11:017) |   └─Token(StringComplete) |'test'|
//@[17:019) ├─Token(NewLine) |\r\n|
var myVar5 = {
//@[00:031) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |myVar5|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:031) | └─ObjectSyntax
//@[13:014) |   ├─Token(LeftBrace) |{|
//@[14:016) |   ├─Token(NewLine) |\r\n|
  name: test
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:012) |   | └─VariableAccessSyntax
//@[08:012) |   |   └─IdentifierSyntax
//@[08:012) |   |     └─Token(Identifier) |test|
//@[12:014) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
param p5 = externalInput('sys.cli', {
//@[00:057) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:008) | ├─IdentifierSyntax
//@[06:008) | | └─Token(Identifier) |p5|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:057) | └─FunctionCallSyntax
//@[11:024) |   ├─IdentifierSyntax
//@[11:024) |   | └─Token(Identifier) |externalInput|
//@[24:025) |   ├─Token(LeftParen) |(|
//@[25:034) |   ├─FunctionArgumentSyntax
//@[25:034) |   | └─StringSyntax
//@[25:034) |   |   └─Token(StringComplete) |'sys.cli'|
//@[34:035) |   ├─Token(Comma) |,|
//@[36:056) |   ├─FunctionArgumentSyntax
//@[36:056) |   | └─ObjectSyntax
//@[36:037) |   |   ├─Token(LeftBrace) |{|
//@[37:039) |   |   ├─Token(NewLine) |\r\n|
  name: myVar5
//@[02:014) |   |   ├─ObjectPropertySyntax
//@[02:006) |   |   | ├─IdentifierSyntax
//@[02:006) |   |   | | └─Token(Identifier) |name|
//@[06:007) |   |   | ├─Token(Colon) |:|
//@[08:014) |   |   | └─VariableAccessSyntax
//@[08:014) |   |   |   └─IdentifierSyntax
//@[08:014) |   |   |     └─Token(Identifier) |myVar5|
//@[14:016) |   |   ├─Token(NewLine) |\r\n|
})
//@[00:001) |   |   └─Token(RightBrace) |}|
//@[01:002) |   └─Token(RightParen) |)|
//@[02:006) ├─Token(NewLine) |\r\n\r\n|

param p6 = externalInput('custom', 'test')
//@[00:042) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:008) | ├─IdentifierSyntax
//@[06:008) | | └─Token(Identifier) |p6|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:042) | └─FunctionCallSyntax
//@[11:024) |   ├─IdentifierSyntax
//@[11:024) |   | └─Token(Identifier) |externalInput|
//@[24:025) |   ├─Token(LeftParen) |(|
//@[25:033) |   ├─FunctionArgumentSyntax
//@[25:033) |   | └─StringSyntax
//@[25:033) |   |   └─Token(StringComplete) |'custom'|
//@[33:034) |   ├─Token(Comma) |,|
//@[35:041) |   ├─FunctionArgumentSyntax
//@[35:041) |   | └─StringSyntax
//@[35:041) |   |   └─Token(StringComplete) |'test'|
//@[41:042) |   └─Token(RightParen) |)|
//@[42:044) ├─Token(NewLine) |\r\n|
param p7 = externalInput(p6)
//@[00:028) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:008) | ├─IdentifierSyntax
//@[06:008) | | └─Token(Identifier) |p7|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:028) | └─FunctionCallSyntax
//@[11:024) |   ├─IdentifierSyntax
//@[11:024) |   | └─Token(Identifier) |externalInput|
//@[24:025) |   ├─Token(LeftParen) |(|
//@[25:027) |   ├─FunctionArgumentSyntax
//@[25:027) |   | └─VariableAccessSyntax
//@[25:027) |   |   └─IdentifierSyntax
//@[25:027) |   |     └─Token(Identifier) |p6|
//@[27:028) |   └─Token(RightParen) |)|
//@[28:032) ├─Token(NewLine) |\r\n\r\n|

param p8 = externalInput('custom', externalInput('custom', 'foo'))
//@[00:066) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:008) | ├─IdentifierSyntax
//@[06:008) | | └─Token(Identifier) |p8|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:066) | └─FunctionCallSyntax
//@[11:024) |   ├─IdentifierSyntax
//@[11:024) |   | └─Token(Identifier) |externalInput|
//@[24:025) |   ├─Token(LeftParen) |(|
//@[25:033) |   ├─FunctionArgumentSyntax
//@[25:033) |   | └─StringSyntax
//@[25:033) |   |   └─Token(StringComplete) |'custom'|
//@[33:034) |   ├─Token(Comma) |,|
//@[35:065) |   ├─FunctionArgumentSyntax
//@[35:065) |   | └─FunctionCallSyntax
//@[35:048) |   |   ├─IdentifierSyntax
//@[35:048) |   |   | └─Token(Identifier) |externalInput|
//@[48:049) |   |   ├─Token(LeftParen) |(|
//@[49:057) |   |   ├─FunctionArgumentSyntax
//@[49:057) |   |   | └─StringSyntax
//@[49:057) |   |   |   └─Token(StringComplete) |'custom'|
//@[57:058) |   |   ├─Token(Comma) |,|
//@[59:064) |   |   ├─FunctionArgumentSyntax
//@[59:064) |   |   | └─StringSyntax
//@[59:064) |   |   |   └─Token(StringComplete) |'foo'|
//@[64:065) |   |   └─Token(RightParen) |)|
//@[65:066) |   └─Token(RightParen) |)|
//@[66:068) ├─Token(NewLine) |\r\n|

//@[00:000) └─Token(EndOfFile) ||
