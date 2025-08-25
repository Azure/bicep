using './main.bicep'
//@[00:308) ProgramSyntax
//@[00:020) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:020) | ├─StringSyntax
//@[06:020) | | └─Token(StringComplete) |'./main.bicep'|
//@[20:020) | └─SkippedTriviaSyntax
//@[20:024) ├─Token(NewLine) |\r\n\r\n|

param para1 = 'value
//@[00:020) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:011) | ├─IdentifierSyntax
//@[06:011) | | └─Token(Identifier) |para1|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:020) | └─SkippedTriviaSyntax
//@[14:020) |   └─Token(StringComplete) |'value|
//@[20:024) ├─Token(NewLine) |\r\n\r\n|

para
//@[00:004) ├─SkippedTriviaSyntax
//@[00:004) | └─Token(Identifier) |para|
//@[04:008) ├─Token(NewLine) |\r\n\r\n|

para2
//@[00:005) ├─SkippedTriviaSyntax
//@[00:005) | └─Token(Identifier) |para2|
//@[05:009) ├─Token(NewLine) |\r\n\r\n|

param expr = 1 + 2
//@[00:018) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:010) | ├─IdentifierSyntax
//@[06:010) | | └─Token(Identifier) |expr|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:018) | └─BinaryOperationSyntax
//@[13:014) |   ├─IntegerLiteralSyntax
//@[13:014) |   | └─Token(Integer) |1|
//@[15:016) |   ├─Token(Plus) |+|
//@[17:018) |   └─IntegerLiteralSyntax
//@[17:018) |     └─Token(Integer) |2|
//@[18:022) ├─Token(NewLine) |\r\n\r\n|

param interp = 'abc${123}def'
//@[00:029) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:012) | ├─IdentifierSyntax
//@[06:012) | | └─Token(Identifier) |interp|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:029) | └─StringSyntax
//@[15:021) |   ├─Token(StringLeftPiece) |'abc${|
//@[21:024) |   ├─IntegerLiteralSyntax
//@[21:024) |   | └─Token(Integer) |123|
//@[24:029) |   └─Token(StringRightPiece) |}def'|
//@[29:033) ├─Token(NewLine) |\r\n\r\n|

param doubleinterp = 'abc${interp + 'blah'}def'
//@[00:047) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:018) | ├─IdentifierSyntax
//@[06:018) | | └─Token(Identifier) |doubleinterp|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:047) | └─StringSyntax
//@[21:027) |   ├─Token(StringLeftPiece) |'abc${|
//@[27:042) |   ├─BinaryOperationSyntax
//@[27:033) |   | ├─VariableAccessSyntax
//@[27:033) |   | | └─IdentifierSyntax
//@[27:033) |   | |   └─Token(Identifier) |interp|
//@[34:035) |   | ├─Token(Plus) |+|
//@[36:042) |   | └─StringSyntax
//@[36:042) |   |   └─Token(StringComplete) |'blah'|
//@[42:047) |   └─Token(StringRightPiece) |}def'|
//@[47:051) ├─Token(NewLine) |\r\n\r\n|

param objWithExpressions = {
//@[00:091) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:024) | ├─IdentifierSyntax
//@[06:024) | | └─Token(Identifier) |objWithExpressions|
//@[25:026) | ├─Token(Assignment) |=|
//@[27:091) | └─ObjectSyntax
//@[27:028) |   ├─Token(LeftBrace) |{|
//@[28:030) |   ├─Token(NewLine) |\r\n|
  foo: 1 + 2
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |foo|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:012) |   | └─BinaryOperationSyntax
//@[07:008) |   |   ├─IntegerLiteralSyntax
//@[07:008) |   |   | └─Token(Integer) |1|
//@[09:010) |   |   ├─Token(Plus) |+|
//@[11:012) |   |   └─IntegerLiteralSyntax
//@[11:012) |   |     └─Token(Integer) |2|
//@[12:014) |   ├─Token(NewLine) |\r\n|
  bar: {
//@[02:044) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |bar|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:044) |   | └─ObjectSyntax
//@[07:008) |   |   ├─Token(LeftBrace) |{|
//@[08:010) |   |   ├─Token(NewLine) |\r\n|
    baz: concat('abc', 'def')
//@[04:029) |   |   ├─ObjectPropertySyntax
//@[04:007) |   |   | ├─IdentifierSyntax
//@[04:007) |   |   | | └─Token(Identifier) |baz|
//@[07:008) |   |   | ├─Token(Colon) |:|
//@[09:029) |   |   | └─FunctionCallSyntax
//@[09:015) |   |   |   ├─IdentifierSyntax
//@[09:015) |   |   |   | └─Token(Identifier) |concat|
//@[15:016) |   |   |   ├─Token(LeftParen) |(|
//@[16:021) |   |   |   ├─FunctionArgumentSyntax
//@[16:021) |   |   |   | └─StringSyntax
//@[16:021) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[21:022) |   |   |   ├─Token(Comma) |,|
//@[23:028) |   |   |   ├─FunctionArgumentSyntax
//@[23:028) |   |   |   | └─StringSyntax
//@[23:028) |   |   |   |   └─Token(StringComplete) |'def'|
//@[28:029) |   |   |   └─Token(RightParen) |)|
//@[29:031) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

param arrayWithExpressions = [1 + 1, 'ok']
//@[00:042) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:026) | ├─IdentifierSyntax
//@[06:026) | | └─Token(Identifier) |arrayWithExpressions|
//@[27:028) | ├─Token(Assignment) |=|
//@[29:042) | └─ArraySyntax
//@[29:030) |   ├─Token(LeftSquare) |[|
//@[30:035) |   ├─ArrayItemSyntax
//@[30:035) |   | └─BinaryOperationSyntax
//@[30:031) |   |   ├─IntegerLiteralSyntax
//@[30:031) |   |   | └─Token(Integer) |1|
//@[32:033) |   |   ├─Token(Plus) |+|
//@[34:035) |   |   └─IntegerLiteralSyntax
//@[34:035) |   |     └─Token(Integer) |1|
//@[35:036) |   ├─Token(Comma) |,|
//@[37:041) |   ├─ArrayItemSyntax
//@[37:041) |   | └─StringSyntax
//@[37:041) |   |   └─Token(StringComplete) |'ok'|
//@[41:042) |   └─Token(RightSquare) |]|
//@[42:042) └─Token(EndOfFile) ||
