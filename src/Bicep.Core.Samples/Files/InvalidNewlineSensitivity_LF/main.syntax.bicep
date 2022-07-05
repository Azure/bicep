var singleLineFunctionNoCommas = concat('abc' 'def')
//@[00:354) ProgramSyntax
//@[00:052) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:030) | ├─IdentifierSyntax
//@[04:030) | | └─Token(Identifier) |singleLineFunctionNoCommas|
//@[31:032) | ├─Token(Assignment) |=|
//@[33:052) | └─FunctionCallSyntax
//@[33:039) |   ├─IdentifierSyntax
//@[33:039) |   | └─Token(Identifier) |concat|
//@[39:040) |   ├─Token(LeftParen) |(|
//@[40:045) |   ├─FunctionArgumentSyntax
//@[40:045) |   | └─StringSyntax
//@[40:045) |   |   └─Token(StringComplete) |'abc'|
//@[46:046) |   ├─SkippedTriviaSyntax
//@[46:051) |   ├─FunctionArgumentSyntax
//@[46:051) |   | └─StringSyntax
//@[46:051) |   |   └─Token(StringComplete) |'def'|
//@[51:052) |   └─Token(RightParen) |)|
//@[52:054) ├─Token(NewLine) |\n\n|

var multiLineFunctionTrailingComma = concat(
//@[00:064) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:034) | ├─IdentifierSyntax
//@[04:034) | | └─Token(Identifier) |multiLineFunctionTrailingComma|
//@[35:036) | ├─Token(Assignment) |=|
//@[37:064) | └─FunctionCallSyntax
//@[37:043) |   ├─IdentifierSyntax
//@[37:043) |   | └─Token(Identifier) |concat|
//@[43:044) |   ├─Token(LeftParen) |(|
//@[44:045) |   ├─Token(NewLine) |\n|
  'abc',
//@[02:007) |   ├─FunctionArgumentSyntax
//@[02:007) |   | └─StringSyntax
//@[02:007) |   |   └─Token(StringComplete) |'abc'|
//@[07:008) |   ├─Token(Comma) |,|
//@[08:009) |   ├─Token(NewLine) |\n|
  'def',
//@[02:007) |   ├─FunctionArgumentSyntax
//@[02:007) |   | └─StringSyntax
//@[02:007) |   |   └─Token(StringComplete) |'def'|
//@[07:008) |   ├─Token(Comma) |,|
//@[08:009) |   ├─Token(NewLine) |\n|
)
//@[00:000) |   ├─FunctionArgumentSyntax
//@[00:000) |   | └─SkippedTriviaSyntax
//@[00:001) |   └─Token(RightParen) |)|
//@[01:003) ├─Token(NewLine) |\n\n|

var singleLineArrayNoCommas = ['abc' 'def']
//@[00:043) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:027) | ├─IdentifierSyntax
//@[04:027) | | └─Token(Identifier) |singleLineArrayNoCommas|
//@[28:029) | ├─Token(Assignment) |=|
//@[30:043) | └─ArraySyntax
//@[30:031) |   ├─Token(LeftSquare) |[|
//@[31:036) |   ├─ArrayItemSyntax
//@[31:036) |   | └─StringSyntax
//@[31:036) |   |   └─Token(StringComplete) |'abc'|
//@[37:037) |   ├─SkippedTriviaSyntax
//@[37:042) |   ├─ArrayItemSyntax
//@[37:042) |   | └─StringSyntax
//@[37:042) |   |   └─Token(StringComplete) |'def'|
//@[42:043) |   └─Token(RightSquare) |]|
//@[43:045) ├─Token(NewLine) |\n\n|

var multiLineArrayMultipleCommas = [
//@[00:059) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:032) | ├─IdentifierSyntax
//@[04:032) | | └─Token(Identifier) |multiLineArrayMultipleCommas|
//@[33:034) | ├─Token(Assignment) |=|
//@[35:059) | └─ArraySyntax
//@[35:036) |   ├─Token(LeftSquare) |[|
//@[36:037) |   ├─Token(NewLine) |\n|
  'abc',,
//@[02:007) |   ├─ArrayItemSyntax
//@[02:007) |   | └─StringSyntax
//@[02:007) |   |   └─Token(StringComplete) |'abc'|
//@[07:008) |   ├─Token(Comma) |,|
//@[08:009) |   ├─SkippedTriviaSyntax
//@[08:009) |   | └─Token(Comma) |,|
//@[09:010) |   ├─Token(NewLine) |\n|
  'def',,,
//@[02:007) |   ├─ArrayItemSyntax
//@[02:007) |   | └─StringSyntax
//@[02:007) |   |   └─Token(StringComplete) |'def'|
//@[07:008) |   ├─Token(Comma) |,|
//@[08:010) |   ├─SkippedTriviaSyntax
//@[08:009) |   | ├─Token(Comma) |,|
//@[09:010) |   | └─Token(Comma) |,|
//@[10:011) |   ├─Token(NewLine) |\n|
]
//@[00:001) |   └─Token(RightSquare) |]|
//@[01:004) ├─Token(NewLine) |\n\n\n|


var singleLineObjectNoCommas = { abc: 'def' ghi: 'jkl'}
//@[00:055) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:028) | ├─IdentifierSyntax
//@[04:028) | | └─Token(Identifier) |singleLineObjectNoCommas|
//@[29:030) | ├─Token(Assignment) |=|
//@[31:055) | └─ObjectSyntax
//@[31:032) |   ├─Token(LeftBrace) |{|
//@[33:043) |   ├─ObjectPropertySyntax
//@[33:036) |   | ├─IdentifierSyntax
//@[33:036) |   | | └─Token(Identifier) |abc|
//@[36:037) |   | ├─Token(Colon) |:|
//@[38:043) |   | └─StringSyntax
//@[38:043) |   |   └─Token(StringComplete) |'def'|
//@[44:044) |   ├─SkippedTriviaSyntax
//@[44:054) |   ├─ObjectPropertySyntax
//@[44:047) |   | ├─IdentifierSyntax
//@[44:047) |   | | └─Token(Identifier) |ghi|
//@[47:048) |   | ├─Token(Colon) |:|
//@[49:054) |   | └─StringSyntax
//@[49:054) |   |   └─Token(StringComplete) |'jkl'|
//@[54:055) |   └─Token(RightBrace) |}|
//@[55:056) ├─Token(NewLine) |\n|
var multiLineObjectMultipleCommas = {
//@[00:070) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:033) | ├─IdentifierSyntax
//@[04:033) | | └─Token(Identifier) |multiLineObjectMultipleCommas|
//@[34:035) | ├─Token(Assignment) |=|
//@[36:070) | └─ObjectSyntax
//@[36:037) |   ├─Token(LeftBrace) |{|
//@[37:038) |   ├─Token(NewLine) |\n|
  abc: 'def',,,
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |abc|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:012) |   | └─StringSyntax
//@[07:012) |   |   └─Token(StringComplete) |'def'|
//@[12:013) |   ├─Token(Comma) |,|
//@[13:015) |   ├─ObjectPropertySyntax
//@[13:015) |   | ├─SkippedTriviaSyntax
//@[13:014) |   | | ├─Token(Comma) |,|
//@[14:015) |   | | └─Token(Comma) |,|
//@[15:015) |   | ├─SkippedTriviaSyntax
//@[15:015) |   | └─SkippedTriviaSyntax
//@[15:016) |   ├─Token(NewLine) |\n|
  ghi: 'jkl',,
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |ghi|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:012) |   | └─StringSyntax
//@[07:012) |   |   └─Token(StringComplete) |'jkl'|
//@[12:013) |   ├─Token(Comma) |,|
//@[13:014) |   ├─ObjectPropertySyntax
//@[13:014) |   | ├─SkippedTriviaSyntax
//@[13:014) |   | | └─Token(Comma) |,|
//@[14:014) |   | ├─SkippedTriviaSyntax
//@[14:014) |   | └─SkippedTriviaSyntax
//@[14:015) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
