using 'main.bicep'
//@[00:272) ProgramSyntax
//@[00:018) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:018) | ├─StringSyntax
//@[06:018) | | └─Token(StringComplete) |'main.bicep'|
//@[18:018) | └─SkippedTriviaSyntax
//@[18:020) ├─Token(NewLine) |\n\n|

@description('blah blah')
//@[00:043) ├─VariableDeclarationSyntax
//@[00:025) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:025) | | └─FunctionCallSyntax
//@[01:012) | |   ├─IdentifierSyntax
//@[01:012) | |   | └─Token(Identifier) |description|
//@[12:013) | |   ├─Token(LeftParen) |(|
//@[13:024) | |   ├─FunctionArgumentSyntax
//@[13:024) | |   | └─StringSyntax
//@[13:024) | |   |   └─Token(StringComplete) |'blah blah'|
//@[24:025) | |   └─Token(RightParen) |)|
//@[25:026) | ├─Token(NewLine) |\n|
var blah = 'blah'
//@[00:003) | ├─Token(Identifier) |var|
//@[04:008) | ├─IdentifierSyntax
//@[04:008) | | └─Token(Identifier) |blah|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:017) | └─StringSyntax
//@[11:017) |   └─Token(StringComplete) |'blah'|
//@[17:019) ├─Token(NewLine) |\n\n|

param foo = blah
//@[00:016) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:009) | ├─IdentifierSyntax
//@[06:009) | | └─Token(Identifier) |foo|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:016) | └─VariableAccessSyntax
//@[12:016) |   └─IdentifierSyntax
//@[12:016) |     └─Token(Identifier) |blah|
//@[16:018) ├─Token(NewLine) |\n\n|

var abc = 'abc'
//@[00:015) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:007) | ├─IdentifierSyntax
//@[04:007) | | └─Token(Identifier) |abc|
//@[08:009) | ├─Token(Assignment) |=|
//@[10:015) | └─StringSyntax
//@[10:015) |   └─Token(StringComplete) |'abc'|
//@[15:016) ├─Token(NewLine) |\n|
var def = {
//@[00:033) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:007) | ├─IdentifierSyntax
//@[04:007) | | └─Token(Identifier) |def|
//@[08:009) | ├─Token(Assignment) |=|
//@[10:033) | └─ObjectSyntax
//@[10:011) |   ├─Token(LeftBrace) |{|
//@[11:012) |   ├─Token(NewLine) |\n|
  abc: toUpper(abc)
//@[02:019) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |abc|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:019) |   | └─FunctionCallSyntax
//@[07:014) |   |   ├─IdentifierSyntax
//@[07:014) |   |   | └─Token(Identifier) |toUpper|
//@[14:015) |   |   ├─Token(LeftParen) |(|
//@[15:018) |   |   ├─FunctionArgumentSyntax
//@[15:018) |   |   | └─VariableAccessSyntax
//@[15:018) |   |   |   └─IdentifierSyntax
//@[15:018) |   |   |     └─Token(Identifier) |abc|
//@[18:019) |   |   └─Token(RightParen) |)|
//@[19:020) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

param fooObj = {
//@[00:042) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:012) | ├─IdentifierSyntax
//@[06:012) | | └─Token(Identifier) |fooObj|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:042) | └─ObjectSyntax
//@[15:016) |   ├─Token(LeftBrace) |{|
//@[16:017) |   ├─Token(NewLine) |\n|
  def: def
//@[02:010) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |def|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:010) |   | └─VariableAccessSyntax
//@[07:010) |   |   └─IdentifierSyntax
//@[07:010) |   |     └─Token(Identifier) |def|
//@[10:011) |   ├─Token(NewLine) |\n|
  ghi: 'ghi'
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |ghi|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:012) |   | └─StringSyntax
//@[07:012) |   |   └─Token(StringComplete) |'ghi'|
//@[12:013) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|
var list = 'FOO,BAR,BAZ'
//@[00:024) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:008) | ├─IdentifierSyntax
//@[04:008) | | └─Token(Identifier) |list|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:024) | └─StringSyntax
//@[11:024) |   └─Token(StringComplete) |'FOO,BAR,BAZ'|
//@[24:025) ├─Token(NewLine) |\n|
param bar = join(map(range(0, 3), i => split(list, ',')[2 - i]), ',')
//@[00:069) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:009) | ├─IdentifierSyntax
//@[06:009) | | └─Token(Identifier) |bar|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:069) | └─FunctionCallSyntax
//@[12:016) |   ├─IdentifierSyntax
//@[12:016) |   | └─Token(Identifier) |join|
//@[16:017) |   ├─Token(LeftParen) |(|
//@[17:063) |   ├─FunctionArgumentSyntax
//@[17:063) |   | └─FunctionCallSyntax
//@[17:020) |   |   ├─IdentifierSyntax
//@[17:020) |   |   | └─Token(Identifier) |map|
//@[20:021) |   |   ├─Token(LeftParen) |(|
//@[21:032) |   |   ├─FunctionArgumentSyntax
//@[21:032) |   |   | └─FunctionCallSyntax
//@[21:026) |   |   |   ├─IdentifierSyntax
//@[21:026) |   |   |   | └─Token(Identifier) |range|
//@[26:027) |   |   |   ├─Token(LeftParen) |(|
//@[27:028) |   |   |   ├─FunctionArgumentSyntax
//@[27:028) |   |   |   | └─IntegerLiteralSyntax
//@[27:028) |   |   |   |   └─Token(Integer) |0|
//@[28:029) |   |   |   ├─Token(Comma) |,|
//@[30:031) |   |   |   ├─FunctionArgumentSyntax
//@[30:031) |   |   |   | └─IntegerLiteralSyntax
//@[30:031) |   |   |   |   └─Token(Integer) |3|
//@[31:032) |   |   |   └─Token(RightParen) |)|
//@[32:033) |   |   ├─Token(Comma) |,|
//@[34:062) |   |   ├─FunctionArgumentSyntax
//@[34:062) |   |   | └─LambdaSyntax
//@[34:035) |   |   |   ├─LocalVariableSyntax
//@[34:035) |   |   |   | └─IdentifierSyntax
//@[34:035) |   |   |   |   └─Token(Identifier) |i|
//@[36:038) |   |   |   ├─Token(Arrow) |=>|
//@[39:062) |   |   |   └─ArrayAccessSyntax
//@[39:055) |   |   |     ├─FunctionCallSyntax
//@[39:044) |   |   |     | ├─IdentifierSyntax
//@[39:044) |   |   |     | | └─Token(Identifier) |split|
//@[44:045) |   |   |     | ├─Token(LeftParen) |(|
//@[45:049) |   |   |     | ├─FunctionArgumentSyntax
//@[45:049) |   |   |     | | └─VariableAccessSyntax
//@[45:049) |   |   |     | |   └─IdentifierSyntax
//@[45:049) |   |   |     | |     └─Token(Identifier) |list|
//@[49:050) |   |   |     | ├─Token(Comma) |,|
//@[51:054) |   |   |     | ├─FunctionArgumentSyntax
//@[51:054) |   |   |     | | └─StringSyntax
//@[51:054) |   |   |     | |   └─Token(StringComplete) |','|
//@[54:055) |   |   |     | └─Token(RightParen) |)|
//@[55:056) |   |   |     ├─Token(LeftSquare) |[|
//@[56:061) |   |   |     ├─BinaryOperationSyntax
//@[56:057) |   |   |     | ├─IntegerLiteralSyntax
//@[56:057) |   |   |     | | └─Token(Integer) |2|
//@[58:059) |   |   |     | ├─Token(Minus) |-|
//@[60:061) |   |   |     | └─VariableAccessSyntax
//@[60:061) |   |   |     |   └─IdentifierSyntax
//@[60:061) |   |   |     |     └─Token(Identifier) |i|
//@[61:062) |   |   |     └─Token(RightSquare) |]|
//@[62:063) |   |   └─Token(RightParen) |)|
//@[63:064) |   ├─Token(Comma) |,|
//@[65:068) |   ├─FunctionArgumentSyntax
//@[65:068) |   | └─StringSyntax
//@[65:068) |   |   └─Token(StringComplete) |','|
//@[68:069) |   └─Token(RightParen) |)|
//@[69:070) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
