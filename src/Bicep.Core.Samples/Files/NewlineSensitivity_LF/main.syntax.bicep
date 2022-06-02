var singleLineFunction = concat('abc', 'def')
//@[00:728) ProgramSyntax
//@[00:045) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:022) | ├─IdentifierSyntax
//@[04:022) | | └─Token(Identifier) |singleLineFunction|
//@[23:024) | ├─Token(Assignment) |=|
//@[25:045) | └─FunctionCallSyntax
//@[25:031) | | ├─IdentifierSyntax
//@[25:031) | | | └─Token(Identifier) |concat|
//@[31:032) | | ├─Token(LeftParen) |(|
//@[32:037) | | ├─FunctionArgumentSyntax
//@[32:037) | | | └─StringSyntax
//@[32:037) | | | | └─Token(StringComplete) |'abc'|
//@[37:038) | | ├─Token(Comma) |,|
//@[39:044) | | ├─FunctionArgumentSyntax
//@[39:044) | | | └─StringSyntax
//@[39:044) | | | | └─Token(StringComplete) |'def'|
//@[44:045) | | └─Token(RightParen) |)|
//@[45:047) ├─Token(NewLine) |\n\n|

var multiLineFunction = concat(
//@[00:050) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:021) | ├─IdentifierSyntax
//@[04:021) | | └─Token(Identifier) |multiLineFunction|
//@[22:023) | ├─Token(Assignment) |=|
//@[24:050) | └─FunctionCallSyntax
//@[24:030) | | ├─IdentifierSyntax
//@[24:030) | | | └─Token(Identifier) |concat|
//@[30:031) | | ├─Token(LeftParen) |(|
//@[31:032) | | ├─Token(NewLine) |\n|
  'abc',
//@[02:007) | | ├─FunctionArgumentSyntax
//@[02:007) | | | └─StringSyntax
//@[02:007) | | | | └─Token(StringComplete) |'abc'|
//@[07:008) | | ├─Token(Comma) |,|
//@[08:009) | | ├─Token(NewLine) |\n|
  'def'
//@[02:007) | | ├─FunctionArgumentSyntax
//@[02:007) | | | └─StringSyntax
//@[02:007) | | | | └─Token(StringComplete) |'def'|
//@[07:008) | | ├─Token(NewLine) |\n|
)
//@[00:001) | | └─Token(RightParen) |)|
//@[01:003) ├─Token(NewLine) |\n\n|

var singleLineArray = ['abc', 'def']
//@[00:036) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:019) | ├─IdentifierSyntax
//@[04:019) | | └─Token(Identifier) |singleLineArray|
//@[20:021) | ├─Token(Assignment) |=|
//@[22:036) | └─ArraySyntax
//@[22:023) | | ├─Token(LeftSquare) |[|
//@[23:028) | | ├─ArrayItemSyntax
//@[23:028) | | | └─StringSyntax
//@[23:028) | | | | └─Token(StringComplete) |'abc'|
//@[28:029) | | ├─Token(Comma) |,|
//@[30:035) | | ├─ArrayItemSyntax
//@[30:035) | | | └─StringSyntax
//@[30:035) | | | | └─Token(StringComplete) |'def'|
//@[35:036) | | └─Token(RightSquare) |]|
//@[36:037) ├─Token(NewLine) |\n|
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[00:051) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:033) | ├─IdentifierSyntax
//@[04:033) | | └─Token(Identifier) |singleLineArrayTrailingCommas|
//@[34:035) | ├─Token(Assignment) |=|
//@[36:051) | └─ArraySyntax
//@[36:037) | | ├─Token(LeftSquare) |[|
//@[37:042) | | ├─ArrayItemSyntax
//@[37:042) | | | └─StringSyntax
//@[37:042) | | | | └─Token(StringComplete) |'abc'|
//@[42:043) | | ├─Token(Comma) |,|
//@[44:049) | | ├─ArrayItemSyntax
//@[44:049) | | | └─StringSyntax
//@[44:049) | | | | └─Token(StringComplete) |'def'|
//@[49:050) | | ├─Token(Comma) |,|
//@[50:051) | | └─Token(RightSquare) |]|
//@[51:053) ├─Token(NewLine) |\n\n|

var multiLineArray = [
//@[00:040) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |multiLineArray|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:040) | └─ArraySyntax
//@[21:022) | | ├─Token(LeftSquare) |[|
//@[22:023) | | ├─Token(NewLine) |\n|
  'abc'
//@[02:007) | | ├─ArrayItemSyntax
//@[02:007) | | | └─StringSyntax
//@[02:007) | | | | └─Token(StringComplete) |'abc'|
//@[07:008) | | ├─Token(NewLine) |\n|
  'def'
//@[02:007) | | ├─ArrayItemSyntax
//@[02:007) | | | └─StringSyntax
//@[02:007) | | | | └─Token(StringComplete) |'def'|
//@[07:008) | | ├─Token(NewLine) |\n|
]
//@[00:001) | | └─Token(RightSquare) |]|
//@[01:002) ├─Token(NewLine) |\n|
var multiLineArrayCommas = [
//@[00:048) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:024) | ├─IdentifierSyntax
//@[04:024) | | └─Token(Identifier) |multiLineArrayCommas|
//@[25:026) | ├─Token(Assignment) |=|
//@[27:048) | └─ArraySyntax
//@[27:028) | | ├─Token(LeftSquare) |[|
//@[28:029) | | ├─Token(NewLine) |\n|
  'abc',
//@[02:007) | | ├─ArrayItemSyntax
//@[02:007) | | | └─StringSyntax
//@[02:007) | | | | └─Token(StringComplete) |'abc'|
//@[07:008) | | ├─Token(Comma) |,|
//@[08:009) | | ├─Token(NewLine) |\n|
  'def',
//@[02:007) | | ├─ArrayItemSyntax
//@[02:007) | | | └─StringSyntax
//@[02:007) | | | | └─Token(StringComplete) |'def'|
//@[07:008) | | ├─Token(Comma) |,|
//@[08:009) | | ├─Token(NewLine) |\n|
]
//@[00:001) | | └─Token(RightSquare) |]|
//@[01:003) ├─Token(NewLine) |\n\n|

var mixedArray = ['abc', 'def'
//@[00:051) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:014) | ├─IdentifierSyntax
//@[04:014) | | └─Token(Identifier) |mixedArray|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:051) | └─ArraySyntax
//@[17:018) | | ├─Token(LeftSquare) |[|
//@[18:023) | | ├─ArrayItemSyntax
//@[18:023) | | | └─StringSyntax
//@[18:023) | | | | └─Token(StringComplete) |'abc'|
//@[23:024) | | ├─Token(Comma) |,|
//@[25:030) | | ├─ArrayItemSyntax
//@[25:030) | | | └─StringSyntax
//@[25:030) | | | | └─Token(StringComplete) |'def'|
//@[30:031) | | ├─Token(NewLine) |\n|
'ghi', 'jkl',
//@[00:005) | | ├─ArrayItemSyntax
//@[00:005) | | | └─StringSyntax
//@[00:005) | | | | └─Token(StringComplete) |'ghi'|
//@[05:006) | | ├─Token(Comma) |,|
//@[07:012) | | ├─ArrayItemSyntax
//@[07:012) | | | └─StringSyntax
//@[07:012) | | | | └─Token(StringComplete) |'jkl'|
//@[12:013) | | ├─Token(Comma) |,|
//@[13:014) | | ├─Token(NewLine) |\n|
'lmn']
//@[00:005) | | ├─ArrayItemSyntax
//@[00:005) | | | └─StringSyntax
//@[00:005) | | | | └─Token(StringComplete) |'lmn'|
//@[05:006) | | └─Token(RightSquare) |]|
//@[06:008) ├─Token(NewLine) |\n\n|

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[00:048) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:020) | ├─IdentifierSyntax
//@[04:020) | | └─Token(Identifier) |singleLineObject|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:048) | └─ObjectSyntax
//@[23:024) | | ├─Token(LeftBrace) |{|
//@[25:035) | | ├─ObjectPropertySyntax
//@[25:028) | | | ├─IdentifierSyntax
//@[25:028) | | | | └─Token(Identifier) |abc|
//@[28:029) | | | ├─Token(Colon) |:|
//@[30:035) | | | └─StringSyntax
//@[30:035) | | | | └─Token(StringComplete) |'def'|
//@[35:036) | | ├─Token(Comma) |,|
//@[37:047) | | ├─ObjectPropertySyntax
//@[37:040) | | | ├─IdentifierSyntax
//@[37:040) | | | | └─Token(Identifier) |ghi|
//@[40:041) | | | ├─Token(Colon) |:|
//@[42:047) | | | └─StringSyntax
//@[42:047) | | | | └─Token(StringComplete) |'jkl'|
//@[47:048) | | └─Token(RightBrace) |}|
//@[48:049) ├─Token(NewLine) |\n|
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[00:063) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:034) | ├─IdentifierSyntax
//@[04:034) | | └─Token(Identifier) |singleLineObjectTrailingCommas|
//@[35:036) | ├─Token(Assignment) |=|
//@[37:063) | └─ObjectSyntax
//@[37:038) | | ├─Token(LeftBrace) |{|
//@[39:049) | | ├─ObjectPropertySyntax
//@[39:042) | | | ├─IdentifierSyntax
//@[39:042) | | | | └─Token(Identifier) |abc|
//@[42:043) | | | ├─Token(Colon) |:|
//@[44:049) | | | └─StringSyntax
//@[44:049) | | | | └─Token(StringComplete) |'def'|
//@[49:050) | | ├─Token(Comma) |,|
//@[51:061) | | ├─ObjectPropertySyntax
//@[51:054) | | | ├─IdentifierSyntax
//@[51:054) | | | | └─Token(Identifier) |ghi|
//@[54:055) | | | ├─Token(Colon) |:|
//@[56:061) | | | └─StringSyntax
//@[56:061) | | | | └─Token(StringComplete) |'jkl'|
//@[61:062) | | ├─Token(Comma) |,|
//@[62:063) | | └─Token(RightBrace) |}|
//@[63:064) ├─Token(NewLine) |\n|
var multiLineObject = {
//@[00:051) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:019) | ├─IdentifierSyntax
//@[04:019) | | └─Token(Identifier) |multiLineObject|
//@[20:021) | ├─Token(Assignment) |=|
//@[22:051) | └─ObjectSyntax
//@[22:023) | | ├─Token(LeftBrace) |{|
//@[23:024) | | ├─Token(NewLine) |\n|
  abc: 'def'
//@[02:012) | | ├─ObjectPropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |abc|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:012) | | | └─StringSyntax
//@[07:012) | | | | └─Token(StringComplete) |'def'|
//@[12:013) | | ├─Token(NewLine) |\n|
  ghi: 'jkl'
//@[02:012) | | ├─ObjectPropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |ghi|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:012) | | | └─StringSyntax
//@[07:012) | | | | └─Token(StringComplete) |'jkl'|
//@[12:013) | | ├─Token(NewLine) |\n|
}
//@[00:001) | | └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|
var multiLineObjectCommas = {
//@[00:059) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:025) | ├─IdentifierSyntax
//@[04:025) | | └─Token(Identifier) |multiLineObjectCommas|
//@[26:027) | ├─Token(Assignment) |=|
//@[28:059) | └─ObjectSyntax
//@[28:029) | | ├─Token(LeftBrace) |{|
//@[29:030) | | ├─Token(NewLine) |\n|
  abc: 'def',
//@[02:012) | | ├─ObjectPropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |abc|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:012) | | | └─StringSyntax
//@[07:012) | | | | └─Token(StringComplete) |'def'|
//@[12:013) | | ├─Token(Comma) |,|
//@[13:014) | | ├─Token(NewLine) |\n|
  ghi: 'jkl',
//@[02:012) | | ├─ObjectPropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |ghi|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:012) | | | └─StringSyntax
//@[07:012) | | | | └─Token(StringComplete) |'jkl'|
//@[12:013) | | ├─Token(Comma) |,|
//@[13:014) | | ├─Token(NewLine) |\n|
}
//@[00:001) | | └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|
var mixedObject = { abc: 'abc', def: 'def'
//@[00:079) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:015) | ├─IdentifierSyntax
//@[04:015) | | └─Token(Identifier) |mixedObject|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:079) | └─ObjectSyntax
//@[18:019) | | ├─Token(LeftBrace) |{|
//@[20:030) | | ├─ObjectPropertySyntax
//@[20:023) | | | ├─IdentifierSyntax
//@[20:023) | | | | └─Token(Identifier) |abc|
//@[23:024) | | | ├─Token(Colon) |:|
//@[25:030) | | | └─StringSyntax
//@[25:030) | | | | └─Token(StringComplete) |'abc'|
//@[30:031) | | ├─Token(Comma) |,|
//@[32:042) | | ├─ObjectPropertySyntax
//@[32:035) | | | ├─IdentifierSyntax
//@[32:035) | | | | └─Token(Identifier) |def|
//@[35:036) | | | ├─Token(Colon) |:|
//@[37:042) | | | └─StringSyntax
//@[37:042) | | | | └─Token(StringComplete) |'def'|
//@[42:043) | | ├─Token(NewLine) |\n|
ghi: 'ghi', jkl: 'jkl',
//@[00:010) | | ├─ObjectPropertySyntax
//@[00:003) | | | ├─IdentifierSyntax
//@[00:003) | | | | └─Token(Identifier) |ghi|
//@[03:004) | | | ├─Token(Colon) |:|
//@[05:010) | | | └─StringSyntax
//@[05:010) | | | | └─Token(StringComplete) |'ghi'|
//@[10:011) | | ├─Token(Comma) |,|
//@[12:022) | | ├─ObjectPropertySyntax
//@[12:015) | | | ├─IdentifierSyntax
//@[12:015) | | | | └─Token(Identifier) |jkl|
//@[15:016) | | | ├─Token(Colon) |:|
//@[17:022) | | | └─StringSyntax
//@[17:022) | | | | └─Token(StringComplete) |'jkl'|
//@[22:023) | | ├─Token(Comma) |,|
//@[23:024) | | ├─Token(NewLine) |\n|
lmn: 'lmn' }
//@[00:010) | | ├─ObjectPropertySyntax
//@[00:003) | | | ├─IdentifierSyntax
//@[00:003) | | | | └─Token(Identifier) |lmn|
//@[03:004) | | | ├─Token(Colon) |:|
//@[05:010) | | | └─StringSyntax
//@[05:010) | | | | └─Token(StringComplete) |'lmn'|
//@[11:012) | | └─Token(RightBrace) |}|
//@[12:014) ├─Token(NewLine) |\n\n|

var nestedMixed = {
//@[00:088) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:015) | ├─IdentifierSyntax
//@[04:015) | | └─Token(Identifier) |nestedMixed|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:088) | └─ObjectSyntax
//@[18:019) | | ├─Token(LeftBrace) |{|
//@[19:020) | | ├─Token(NewLine) |\n|
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[02:066) | | ├─ObjectPropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |abc|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:066) | | | └─ObjectSyntax
//@[07:008) | | | | ├─Token(LeftBrace) |{|
//@[09:021) | | | | ├─ObjectPropertySyntax
//@[09:014) | | | | | ├─StringSyntax
//@[09:014) | | | | | | └─Token(StringComplete) |'def'|
//@[14:015) | | | | | ├─Token(Colon) |:|
//@[16:021) | | | | | └─StringSyntax
//@[16:021) | | | | | | └─Token(StringComplete) |'ghi'|
//@[21:022) | | | | ├─Token(Comma) |,|
//@[23:033) | | | | ├─ObjectPropertySyntax
//@[23:026) | | | | | ├─IdentifierSyntax
//@[23:026) | | | | | | └─Token(Identifier) |abc|
//@[26:027) | | | | | ├─Token(Colon) |:|
//@[28:033) | | | | | └─StringSyntax
//@[28:033) | | | | | | └─Token(StringComplete) |'def'|
//@[33:034) | | | | ├─Token(Comma) |,|
//@[35:064) | | | | ├─ObjectPropertySyntax
//@[35:038) | | | | | ├─IdentifierSyntax
//@[35:038) | | | | | | └─Token(Identifier) |foo|
//@[38:039) | | | | | ├─Token(Colon) |:|
//@[40:064) | | | | | └─ArraySyntax
//@[40:041) | | | | | | ├─Token(LeftSquare) |[|
//@[41:042) | | | | | | ├─Token(NewLine) |\n|
    'bar', 'blah',
//@[04:009) | | | | | | ├─ArrayItemSyntax
//@[04:009) | | | | | | | └─StringSyntax
//@[04:009) | | | | | | | | └─Token(StringComplete) |'bar'|
//@[09:010) | | | | | | ├─Token(Comma) |,|
//@[11:017) | | | | | | ├─ArrayItemSyntax
//@[11:017) | | | | | | | └─StringSyntax
//@[11:017) | | | | | | | | └─Token(StringComplete) |'blah'|
//@[17:018) | | | | | | ├─Token(Comma) |,|
//@[18:019) | | | | | | ├─Token(NewLine) |\n|
  ] }
//@[02:003) | | | | | | └─Token(RightSquare) |]|
//@[04:005) | | | | └─Token(RightBrace) |}|
//@[05:006) | | ├─Token(NewLine) |\n|
}
//@[00:001) | | └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
