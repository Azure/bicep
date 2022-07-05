@allowed(['abc', 'def', 'ghi'])
//@[00:1053) ProgramSyntax
//@[00:0048) ├─ParameterDeclarationSyntax
//@[00:0031) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0031) | | └─FunctionCallSyntax
//@[01:0008) | |   ├─IdentifierSyntax
//@[01:0008) | |   | └─Token(Identifier) |allowed|
//@[08:0009) | |   ├─Token(LeftParen) |(|
//@[09:0030) | |   ├─FunctionArgumentSyntax
//@[09:0030) | |   | └─ArraySyntax
//@[09:0010) | |   |   ├─Token(LeftSquare) |[|
//@[10:0015) | |   |   ├─ArrayItemSyntax
//@[10:0015) | |   |   | └─StringSyntax
//@[10:0015) | |   |   |   └─Token(StringComplete) |'abc'|
//@[15:0016) | |   |   ├─Token(Comma) |,|
//@[17:0022) | |   |   ├─ArrayItemSyntax
//@[17:0022) | |   |   | └─StringSyntax
//@[17:0022) | |   |   |   └─Token(StringComplete) |'def'|
//@[22:0023) | |   |   ├─Token(Comma) |,|
//@[24:0029) | |   |   ├─ArrayItemSyntax
//@[24:0029) | |   |   | └─StringSyntax
//@[24:0029) | |   |   |   └─Token(StringComplete) |'ghi'|
//@[29:0030) | |   |   └─Token(RightSquare) |]|
//@[30:0031) | |   └─Token(RightParen) |)|
//@[31:0032) | ├─Token(NewLine) |\n|
param foo string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |foo|
//@[10:0016) | └─SimpleTypeSyntax
//@[10:0016) |   └─Token(Identifier) |string|
//@[16:0018) ├─Token(NewLine) |\n\n|

var singleLineFunction = concat('abc', 'def')
//@[00:0045) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |singleLineFunction|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0045) | └─FunctionCallSyntax
//@[25:0031) |   ├─IdentifierSyntax
//@[25:0031) |   | └─Token(Identifier) |concat|
//@[31:0032) |   ├─Token(LeftParen) |(|
//@[32:0037) |   ├─FunctionArgumentSyntax
//@[32:0037) |   | └─StringSyntax
//@[32:0037) |   |   └─Token(StringComplete) |'abc'|
//@[37:0038) |   ├─Token(Comma) |,|
//@[39:0044) |   ├─FunctionArgumentSyntax
//@[39:0044) |   | └─StringSyntax
//@[39:0044) |   |   └─Token(StringComplete) |'def'|
//@[44:0045) |   └─Token(RightParen) |)|
//@[45:0047) ├─Token(NewLine) |\n\n|

var multiLineFunction = concat(
//@[00:0050) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |multiLineFunction|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0050) | └─FunctionCallSyntax
//@[24:0030) |   ├─IdentifierSyntax
//@[24:0030) |   | └─Token(Identifier) |concat|
//@[30:0031) |   ├─Token(LeftParen) |(|
//@[31:0032) |   ├─Token(NewLine) |\n|
  'abc',
//@[02:0007) |   ├─FunctionArgumentSyntax
//@[02:0007) |   | └─StringSyntax
//@[02:0007) |   |   └─Token(StringComplete) |'abc'|
//@[07:0008) |   ├─Token(Comma) |,|
//@[08:0009) |   ├─Token(NewLine) |\n|
  'def'
//@[02:0007) |   ├─FunctionArgumentSyntax
//@[02:0007) |   | └─StringSyntax
//@[02:0007) |   |   └─Token(StringComplete) |'def'|
//@[07:0008) |   ├─Token(NewLine) |\n|
)
//@[00:0001) |   └─Token(RightParen) |)|
//@[01:0003) ├─Token(NewLine) |\n\n|

var multiLineFunctionUnusualFormatting = concat(
//@[00:0101) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0038) | ├─IdentifierSyntax
//@[04:0038) | | └─Token(Identifier) |multiLineFunctionUnusualFormatting|
//@[39:0040) | ├─Token(Assignment) |=|
//@[41:0101) | └─FunctionCallSyntax
//@[41:0047) |   ├─IdentifierSyntax
//@[41:0047) |   | └─Token(Identifier) |concat|
//@[47:0048) |   ├─Token(LeftParen) |(|
//@[48:0049) |   ├─Token(NewLine) |\n|
              'abc',          any(['hello']),
//@[14:0019) |   ├─FunctionArgumentSyntax
//@[14:0019) |   | └─StringSyntax
//@[14:0019) |   |   └─Token(StringComplete) |'abc'|
//@[19:0020) |   ├─Token(Comma) |,|
//@[30:0044) |   ├─FunctionArgumentSyntax
//@[30:0044) |   | └─FunctionCallSyntax
//@[30:0033) |   |   ├─IdentifierSyntax
//@[30:0033) |   |   | └─Token(Identifier) |any|
//@[33:0034) |   |   ├─Token(LeftParen) |(|
//@[34:0043) |   |   ├─FunctionArgumentSyntax
//@[34:0043) |   |   | └─ArraySyntax
//@[34:0035) |   |   |   ├─Token(LeftSquare) |[|
//@[35:0042) |   |   |   ├─ArrayItemSyntax
//@[35:0042) |   |   |   | └─StringSyntax
//@[35:0042) |   |   |   |   └─Token(StringComplete) |'hello'|
//@[42:0043) |   |   |   └─Token(RightSquare) |]|
//@[43:0044) |   |   └─Token(RightParen) |)|
//@[44:0045) |   ├─Token(Comma) |,|
//@[45:0046) |   ├─Token(NewLine) |\n|
'def')
//@[00:0005) |   ├─FunctionArgumentSyntax
//@[00:0005) |   | └─StringSyntax
//@[00:0005) |   |   └─Token(StringComplete) |'def'|
//@[05:0006) |   └─Token(RightParen) |)|
//@[06:0008) ├─Token(NewLine) |\n\n|

var nestedTest = concat(
//@[00:0108) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |nestedTest|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0108) | └─FunctionCallSyntax
//@[17:0023) |   ├─IdentifierSyntax
//@[17:0023) |   | └─Token(Identifier) |concat|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0025) |   ├─Token(NewLine) |\n|
concat(
//@[00:0074) |   ├─FunctionArgumentSyntax
//@[00:0074) |   | └─FunctionCallSyntax
//@[00:0006) |   |   ├─IdentifierSyntax
//@[00:0006) |   |   | └─Token(Identifier) |concat|
//@[06:0007) |   |   ├─Token(LeftParen) |(|
//@[07:0008) |   |   ├─Token(NewLine) |\n|
concat(
//@[00:0057) |   |   ├─FunctionArgumentSyntax
//@[00:0057) |   |   | └─FunctionCallSyntax
//@[00:0006) |   |   |   ├─IdentifierSyntax
//@[00:0006) |   |   |   | └─Token(Identifier) |concat|
//@[06:0007) |   |   |   ├─Token(LeftParen) |(|
//@[07:0008) |   |   |   ├─Token(NewLine) |\n|
concat(
//@[00:0039) |   |   |   ├─FunctionArgumentSyntax
//@[00:0039) |   |   |   | └─FunctionCallSyntax
//@[00:0006) |   |   |   |   ├─IdentifierSyntax
//@[00:0006) |   |   |   |   | └─Token(Identifier) |concat|
//@[06:0007) |   |   |   |   ├─Token(LeftParen) |(|
//@[07:0008) |   |   |   |   ├─Token(NewLine) |\n|
concat(
//@[00:0023) |   |   |   |   ├─FunctionArgumentSyntax
//@[00:0023) |   |   |   |   | └─FunctionCallSyntax
//@[00:0006) |   |   |   |   |   ├─IdentifierSyntax
//@[00:0006) |   |   |   |   |   | └─Token(Identifier) |concat|
//@[06:0007) |   |   |   |   |   ├─Token(LeftParen) |(|
//@[07:0008) |   |   |   |   |   ├─Token(NewLine) |\n|
'level',
//@[00:0007) |   |   |   |   |   ├─FunctionArgumentSyntax
//@[00:0007) |   |   |   |   |   | └─StringSyntax
//@[00:0007) |   |   |   |   |   |   └─Token(StringComplete) |'level'|
//@[07:0008) |   |   |   |   |   ├─Token(Comma) |,|
//@[08:0009) |   |   |   |   |   ├─Token(NewLine) |\n|
'one'),
//@[00:0005) |   |   |   |   |   ├─FunctionArgumentSyntax
//@[00:0005) |   |   |   |   |   | └─StringSyntax
//@[00:0005) |   |   |   |   |   |   └─Token(StringComplete) |'one'|
//@[05:0006) |   |   |   |   |   └─Token(RightParen) |)|
//@[06:0007) |   |   |   |   ├─Token(Comma) |,|
//@[07:0008) |   |   |   |   ├─Token(NewLine) |\n|
'two'),
//@[00:0005) |   |   |   |   ├─FunctionArgumentSyntax
//@[00:0005) |   |   |   |   | └─StringSyntax
//@[00:0005) |   |   |   |   |   └─Token(StringComplete) |'two'|
//@[05:0006) |   |   |   |   └─Token(RightParen) |)|
//@[06:0007) |   |   |   ├─Token(Comma) |,|
//@[07:0008) |   |   |   ├─Token(NewLine) |\n|
'three'),
//@[00:0007) |   |   |   ├─FunctionArgumentSyntax
//@[00:0007) |   |   |   | └─StringSyntax
//@[00:0007) |   |   |   |   └─Token(StringComplete) |'three'|
//@[07:0008) |   |   |   └─Token(RightParen) |)|
//@[08:0009) |   |   ├─Token(Comma) |,|
//@[09:0010) |   |   ├─Token(NewLine) |\n|
'four'),
//@[00:0006) |   |   ├─FunctionArgumentSyntax
//@[00:0006) |   |   | └─StringSyntax
//@[00:0006) |   |   |   └─Token(StringComplete) |'four'|
//@[06:0007) |   |   └─Token(RightParen) |)|
//@[07:0008) |   ├─Token(Comma) |,|
//@[08:0009) |   ├─Token(NewLine) |\n|
'five')
//@[00:0006) |   ├─FunctionArgumentSyntax
//@[00:0006) |   | └─StringSyntax
//@[00:0006) |   |   └─Token(StringComplete) |'five'|
//@[06:0007) |   └─Token(RightParen) |)|
//@[07:0009) ├─Token(NewLine) |\n\n|

var singleLineArray = ['abc', 'def']
//@[00:0036) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |singleLineArray|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0036) | └─ArraySyntax
//@[22:0023) |   ├─Token(LeftSquare) |[|
//@[23:0028) |   ├─ArrayItemSyntax
//@[23:0028) |   | └─StringSyntax
//@[23:0028) |   |   └─Token(StringComplete) |'abc'|
//@[28:0029) |   ├─Token(Comma) |,|
//@[30:0035) |   ├─ArrayItemSyntax
//@[30:0035) |   | └─StringSyntax
//@[30:0035) |   |   └─Token(StringComplete) |'def'|
//@[35:0036) |   └─Token(RightSquare) |]|
//@[36:0037) ├─Token(NewLine) |\n|
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[00:0051) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0033) | ├─IdentifierSyntax
//@[04:0033) | | └─Token(Identifier) |singleLineArrayTrailingCommas|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0051) | └─ArraySyntax
//@[36:0037) |   ├─Token(LeftSquare) |[|
//@[37:0042) |   ├─ArrayItemSyntax
//@[37:0042) |   | └─StringSyntax
//@[37:0042) |   |   └─Token(StringComplete) |'abc'|
//@[42:0043) |   ├─Token(Comma) |,|
//@[44:0049) |   ├─ArrayItemSyntax
//@[44:0049) |   | └─StringSyntax
//@[44:0049) |   |   └─Token(StringComplete) |'def'|
//@[49:0050) |   ├─Token(Comma) |,|
//@[50:0051) |   └─Token(RightSquare) |]|
//@[51:0053) ├─Token(NewLine) |\n\n|

var multiLineArray = [
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |multiLineArray|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0040) | └─ArraySyntax
//@[21:0022) |   ├─Token(LeftSquare) |[|
//@[22:0023) |   ├─Token(NewLine) |\n|
  'abc'
//@[02:0007) |   ├─ArrayItemSyntax
//@[02:0007) |   | └─StringSyntax
//@[02:0007) |   |   └─Token(StringComplete) |'abc'|
//@[07:0008) |   ├─Token(NewLine) |\n|
  'def'
//@[02:0007) |   ├─ArrayItemSyntax
//@[02:0007) |   | └─StringSyntax
//@[02:0007) |   |   └─Token(StringComplete) |'def'|
//@[07:0008) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0003) ├─Token(NewLine) |\n\n|

var mixedArray = ['abc', 'def'
//@[00:0050) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |mixedArray|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0050) | └─ArraySyntax
//@[17:0018) |   ├─Token(LeftSquare) |[|
//@[18:0023) |   ├─ArrayItemSyntax
//@[18:0023) |   | └─StringSyntax
//@[18:0023) |   |   └─Token(StringComplete) |'abc'|
//@[23:0024) |   ├─Token(Comma) |,|
//@[25:0030) |   ├─ArrayItemSyntax
//@[25:0030) |   | └─StringSyntax
//@[25:0030) |   |   └─Token(StringComplete) |'def'|
//@[30:0031) |   ├─Token(NewLine) |\n|
'ghi', 'jkl'
//@[00:0005) |   ├─ArrayItemSyntax
//@[00:0005) |   | └─StringSyntax
//@[00:0005) |   |   └─Token(StringComplete) |'ghi'|
//@[05:0006) |   ├─Token(Comma) |,|
//@[07:0012) |   ├─ArrayItemSyntax
//@[07:0012) |   | └─StringSyntax
//@[07:0012) |   |   └─Token(StringComplete) |'jkl'|
//@[12:0013) |   ├─Token(NewLine) |\n|
'lmn']
//@[00:0005) |   ├─ArrayItemSyntax
//@[00:0005) |   | └─StringSyntax
//@[00:0005) |   |   └─Token(StringComplete) |'lmn'|
//@[05:0006) |   └─Token(RightSquare) |]|
//@[06:0008) ├─Token(NewLine) |\n\n|

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[00:0048) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0020) | ├─IdentifierSyntax
//@[04:0020) | | └─Token(Identifier) |singleLineObject|
//@[21:0022) | ├─Token(Assignment) |=|
//@[23:0048) | └─ObjectSyntax
//@[23:0024) |   ├─Token(LeftBrace) |{|
//@[25:0035) |   ├─ObjectPropertySyntax
//@[25:0028) |   | ├─IdentifierSyntax
//@[25:0028) |   | | └─Token(Identifier) |abc|
//@[28:0029) |   | ├─Token(Colon) |:|
//@[30:0035) |   | └─StringSyntax
//@[30:0035) |   |   └─Token(StringComplete) |'def'|
//@[35:0036) |   ├─Token(Comma) |,|
//@[37:0047) |   ├─ObjectPropertySyntax
//@[37:0040) |   | ├─IdentifierSyntax
//@[37:0040) |   | | └─Token(Identifier) |ghi|
//@[40:0041) |   | ├─Token(Colon) |:|
//@[42:0047) |   | └─StringSyntax
//@[42:0047) |   |   └─Token(StringComplete) |'jkl'|
//@[47:0048) |   └─Token(RightBrace) |}|
//@[48:0049) ├─Token(NewLine) |\n|
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[00:0063) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0034) | ├─IdentifierSyntax
//@[04:0034) | | └─Token(Identifier) |singleLineObjectTrailingCommas|
//@[35:0036) | ├─Token(Assignment) |=|
//@[37:0063) | └─ObjectSyntax
//@[37:0038) |   ├─Token(LeftBrace) |{|
//@[39:0049) |   ├─ObjectPropertySyntax
//@[39:0042) |   | ├─IdentifierSyntax
//@[39:0042) |   | | └─Token(Identifier) |abc|
//@[42:0043) |   | ├─Token(Colon) |:|
//@[44:0049) |   | └─StringSyntax
//@[44:0049) |   |   └─Token(StringComplete) |'def'|
//@[49:0050) |   ├─Token(Comma) |,|
//@[51:0061) |   ├─ObjectPropertySyntax
//@[51:0054) |   | ├─IdentifierSyntax
//@[51:0054) |   | | └─Token(Identifier) |ghi|
//@[54:0055) |   | ├─Token(Colon) |:|
//@[56:0061) |   | └─StringSyntax
//@[56:0061) |   |   └─Token(StringComplete) |'jkl'|
//@[61:0062) |   ├─Token(Comma) |,|
//@[62:0063) |   └─Token(RightBrace) |}|
//@[63:0064) ├─Token(NewLine) |\n|
var multiLineObject = {
//@[00:0051) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |multiLineObject|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0051) | └─ObjectSyntax
//@[22:0023) |   ├─Token(LeftBrace) |{|
//@[23:0024) |   ├─Token(NewLine) |\n|
  abc: 'def'
//@[02:0012) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |abc|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0012) |   | └─StringSyntax
//@[07:0012) |   |   └─Token(StringComplete) |'def'|
//@[12:0013) |   ├─Token(NewLine) |\n|
  ghi: 'jkl'
//@[02:0012) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |ghi|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0012) |   | └─StringSyntax
//@[07:0012) |   |   └─Token(StringComplete) |'jkl'|
//@[12:0013) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
var mixedObject = { abc: 'abc', def: 'def'
//@[00:0078) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |mixedObject|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0078) | └─ObjectSyntax
//@[18:0019) |   ├─Token(LeftBrace) |{|
//@[20:0030) |   ├─ObjectPropertySyntax
//@[20:0023) |   | ├─IdentifierSyntax
//@[20:0023) |   | | └─Token(Identifier) |abc|
//@[23:0024) |   | ├─Token(Colon) |:|
//@[25:0030) |   | └─StringSyntax
//@[25:0030) |   |   └─Token(StringComplete) |'abc'|
//@[30:0031) |   ├─Token(Comma) |,|
//@[32:0042) |   ├─ObjectPropertySyntax
//@[32:0035) |   | ├─IdentifierSyntax
//@[32:0035) |   | | └─Token(Identifier) |def|
//@[35:0036) |   | ├─Token(Colon) |:|
//@[37:0042) |   | └─StringSyntax
//@[37:0042) |   |   └─Token(StringComplete) |'def'|
//@[42:0043) |   ├─Token(NewLine) |\n|
ghi: 'ghi', jkl: 'jkl'
//@[00:0010) |   ├─ObjectPropertySyntax
//@[00:0003) |   | ├─IdentifierSyntax
//@[00:0003) |   | | └─Token(Identifier) |ghi|
//@[03:0004) |   | ├─Token(Colon) |:|
//@[05:0010) |   | └─StringSyntax
//@[05:0010) |   |   └─Token(StringComplete) |'ghi'|
//@[10:0011) |   ├─Token(Comma) |,|
//@[12:0022) |   ├─ObjectPropertySyntax
//@[12:0015) |   | ├─IdentifierSyntax
//@[12:0015) |   | | └─Token(Identifier) |jkl|
//@[15:0016) |   | ├─Token(Colon) |:|
//@[17:0022) |   | └─StringSyntax
//@[17:0022) |   |   └─Token(StringComplete) |'jkl'|
//@[22:0023) |   ├─Token(NewLine) |\n|
lmn: 'lmn' }
//@[00:0010) |   ├─ObjectPropertySyntax
//@[00:0003) |   | ├─IdentifierSyntax
//@[00:0003) |   | | └─Token(Identifier) |lmn|
//@[03:0004) |   | ├─Token(Colon) |:|
//@[05:0010) |   | └─StringSyntax
//@[05:0010) |   |   └─Token(StringComplete) |'lmn'|
//@[11:0012) |   └─Token(RightBrace) |}|
//@[12:0014) ├─Token(NewLine) |\n\n|

var nestedMixed = {
//@[00:0087) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |nestedMixed|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0087) | └─ObjectSyntax
//@[18:0019) |   ├─Token(LeftBrace) |{|
//@[19:0020) |   ├─Token(NewLine) |\n|
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[02:0065) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |abc|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0065) |   | └─ObjectSyntax
//@[07:0008) |   |   ├─Token(LeftBrace) |{|
//@[09:0021) |   |   ├─ObjectPropertySyntax
//@[09:0014) |   |   | ├─StringSyntax
//@[09:0014) |   |   | | └─Token(StringComplete) |'def'|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0021) |   |   | └─StringSyntax
//@[16:0021) |   |   |   └─Token(StringComplete) |'ghi'|
//@[21:0022) |   |   ├─Token(Comma) |,|
//@[23:0033) |   |   ├─ObjectPropertySyntax
//@[23:0026) |   |   | ├─IdentifierSyntax
//@[23:0026) |   |   | | └─Token(Identifier) |abc|
//@[26:0027) |   |   | ├─Token(Colon) |:|
//@[28:0033) |   |   | └─StringSyntax
//@[28:0033) |   |   |   └─Token(StringComplete) |'def'|
//@[33:0034) |   |   ├─Token(Comma) |,|
//@[35:0063) |   |   ├─ObjectPropertySyntax
//@[35:0038) |   |   | ├─IdentifierSyntax
//@[35:0038) |   |   | | └─Token(Identifier) |foo|
//@[38:0039) |   |   | ├─Token(Colon) |:|
//@[40:0063) |   |   | └─ArraySyntax
//@[40:0041) |   |   |   ├─Token(LeftSquare) |[|
//@[41:0042) |   |   |   ├─Token(NewLine) |\n|
    'bar', 'blah'
//@[04:0009) |   |   |   ├─ArrayItemSyntax
//@[04:0009) |   |   |   | └─StringSyntax
//@[04:0009) |   |   |   |   └─Token(StringComplete) |'bar'|
//@[09:0010) |   |   |   ├─Token(Comma) |,|
//@[11:0017) |   |   |   ├─ArrayItemSyntax
//@[11:0017) |   |   |   | └─StringSyntax
//@[11:0017) |   |   |   |   └─Token(StringComplete) |'blah'|
//@[17:0018) |   |   |   ├─Token(NewLine) |\n|
  ] }
//@[02:0003) |   |   |   └─Token(RightSquare) |]|
//@[04:0005) |   |   └─Token(RightBrace) |}|
//@[05:0006) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[00:0172) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0020) | ├─IdentifierSyntax
//@[04:0020) | | └─Token(Identifier) |brokenFormatting|
//@[21:0022) | ├─Token(Assignment) |=|
//@[23:0172) | └─ArraySyntax
//@[23:0024) |   ├─Token(LeftSquare) |[|
//@[39:0044) |   ├─ArrayItemSyntax
//@[39:0044) |   | └─StringSyntax
//@[39:0044) |   |   └─Token(StringComplete) |'bar'|

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''
//@[02:0003) |   ├─Token(Comma) |,|
//@[11:0020) |   ├─ArrayItemSyntax
//@[11:0020) |   | └─StringSyntax
//@[11:0020) |   |   └─Token(StringComplete) |'asdfdsf'|
//@[20:0021) |   ├─Token(Comma) |,|
//@[34:0039) |   ├─ArrayItemSyntax
//@[34:0039) |   | └─IntegerLiteralSyntax
//@[34:0039) |   |   └─Token(Integer) |12324|
//@[39:0040) |   ├─Token(Comma) |,|
//@[59:0061) |   ├─ArrayItemSyntax
//@[59:0061) |   | └─StringSyntax
//@[59:0061) |   |   └─Token(StringComplete) |''|
//@[61:0062) |   ├─Token(Comma) |,|
//@[67:0076) |   ├─ArrayItemSyntax
//@[67:0076) |   | └─StringSyntax
//@[67:0076) |   |   └─Token(MultilineString) |'''\n\n\n'''|


'''
//@[03:0004) |   ├─Token(NewLine) |\n|
123,      233535
//@[00:0003) |   ├─ArrayItemSyntax
//@[00:0003) |   | └─IntegerLiteralSyntax
//@[00:0003) |   |   └─Token(Integer) |123|
//@[03:0004) |   ├─Token(Comma) |,|
//@[10:0016) |   ├─ArrayItemSyntax
//@[10:0016) |   | └─IntegerLiteralSyntax
//@[10:0016) |   |   └─Token(Integer) |233535|
//@[16:0017) |   ├─Token(NewLine) |\n|
true
//@[00:0004) |   ├─ArrayItemSyntax
//@[00:0004) |   | └─BooleanLiteralSyntax
//@[00:0004) |   |   └─Token(TrueKeyword) |true|
//@[04:0005) |   ├─Token(NewLine) |\n|
              ]
//@[14:0015) |   └─Token(RightSquare) |]|
//@[15:0016) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
