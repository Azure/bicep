@description('The foo type')
//@[00:2846) ProgramSyntax
//@[00:0299) ├─TypeDeclarationSyntax
//@[00:0028) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0028) | | └─FunctionCallSyntax
//@[01:0012) | |   ├─IdentifierSyntax
//@[01:0012) | |   | └─Token(Identifier) |description|
//@[12:0013) | |   ├─Token(LeftParen) |(|
//@[13:0027) | |   ├─FunctionArgumentSyntax
//@[13:0027) | |   | └─StringSyntax
//@[13:0027) | |   |   └─Token(StringComplete) |'The foo type'|
//@[27:0028) | |   └─Token(RightParen) |)|
//@[28:0029) | ├─Token(NewLine) |\n|
@sealed()
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |sealed|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0010) | ├─Token(NewLine) |\n|
type foo = {
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0008) | ├─IdentifierSyntax
//@[05:0008) | | └─Token(Identifier) |foo|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0260) | └─ObjectTypeSyntax
//@[11:0012) |   ├─Token(LeftBrace) |{|
//@[12:0013) |   ├─Token(NewLine) |\n|
  @minLength(3)
//@[02:0089) |   ├─ObjectTypePropertySyntax
//@[02:0015) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0015) |   | | └─FunctionCallSyntax
//@[03:0012) |   | |   ├─IdentifierSyntax
//@[03:0012) |   | |   | └─Token(Identifier) |minLength|
//@[12:0013) |   | |   ├─Token(LeftParen) |(|
//@[13:0014) |   | |   ├─FunctionArgumentSyntax
//@[13:0014) |   | |   | └─IntegerLiteralSyntax
//@[13:0014) |   | |   |   └─Token(Integer) |3|
//@[14:0015) |   | |   └─Token(RightParen) |)|
//@[15:0016) |   | ├─Token(NewLine) |\n|
  @maxLength(10)
//@[02:0016) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0016) |   | | └─FunctionCallSyntax
//@[03:0012) |   | |   ├─IdentifierSyntax
//@[03:0012) |   | |   | └─Token(Identifier) |maxLength|
//@[12:0013) |   | |   ├─Token(LeftParen) |(|
//@[13:0015) |   | |   ├─FunctionArgumentSyntax
//@[13:0015) |   | |   | └─IntegerLiteralSyntax
//@[13:0015) |   | |   |   └─Token(Integer) |10|
//@[15:0016) |   | |   └─Token(RightParen) |)|
//@[16:0017) |   | ├─Token(NewLine) |\n|
  @description('A string property')
//@[02:0035) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0035) |   | | └─FunctionCallSyntax
//@[03:0014) |   | |   ├─IdentifierSyntax
//@[03:0014) |   | |   | └─Token(Identifier) |description|
//@[14:0015) |   | |   ├─Token(LeftParen) |(|
//@[15:0034) |   | |   ├─FunctionArgumentSyntax
//@[15:0034) |   | |   | └─StringSyntax
//@[15:0034) |   | |   |   └─Token(StringComplete) |'A string property'|
//@[34:0035) |   | |   └─Token(RightParen) |)|
//@[35:0036) |   | ├─Token(NewLine) |\n|
  stringProp: string
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |stringProp|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0020) |   | └─VariableAccessSyntax
//@[14:0020) |   |   └─IdentifierSyntax
//@[14:0020) |   |     └─Token(Identifier) |string|
//@[20:0022) |   ├─Token(NewLine) |\n\n|

  objectProp: {
//@[02:0089) |   ├─ObjectTypePropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |objectProp|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0089) |   | └─ObjectTypeSyntax
//@[14:0015) |   |   ├─Token(LeftBrace) |{|
//@[15:0016) |   |   ├─Token(NewLine) |\n|
    @minValue(1)
//@[04:0033) |   |   ├─ObjectTypePropertySyntax
//@[04:0016) |   |   | ├─DecoratorSyntax
//@[04:0005) |   |   | | ├─Token(At) |@|
//@[05:0016) |   |   | | └─FunctionCallSyntax
//@[05:0013) |   |   | |   ├─IdentifierSyntax
//@[05:0013) |   |   | |   | └─Token(Identifier) |minValue|
//@[13:0014) |   |   | |   ├─Token(LeftParen) |(|
//@[14:0015) |   |   | |   ├─FunctionArgumentSyntax
//@[14:0015) |   |   | |   | └─IntegerLiteralSyntax
//@[14:0015) |   |   | |   |   └─Token(Integer) |1|
//@[15:0016) |   |   | |   └─Token(RightParen) |)|
//@[16:0017) |   |   | ├─Token(NewLine) |\n|
    intProp: int
//@[04:0011) |   |   | ├─IdentifierSyntax
//@[04:0011) |   |   | | └─Token(Identifier) |intProp|
//@[11:0012) |   |   | ├─Token(Colon) |:|
//@[13:0016) |   |   | └─VariableAccessSyntax
//@[13:0016) |   |   |   └─IdentifierSyntax
//@[13:0016) |   |   |     └─Token(Identifier) |int|
//@[16:0018) |   |   ├─Token(NewLine) |\n\n|

    intArrayArrayProp: int [] [] ?
//@[04:0034) |   |   ├─ObjectTypePropertySyntax
//@[04:0021) |   |   | ├─IdentifierSyntax
//@[04:0021) |   |   | | └─Token(Identifier) |intArrayArrayProp|
//@[21:0022) |   |   | ├─Token(Colon) |:|
//@[23:0034) |   |   | └─NullableTypeSyntax
//@[23:0032) |   |   |   ├─ArrayTypeSyntax
//@[23:0029) |   |   |   | ├─ArrayTypeMemberSyntax
//@[23:0029) |   |   |   | | └─ArrayTypeSyntax
//@[23:0026) |   |   |   | |   ├─ArrayTypeMemberSyntax
//@[23:0026) |   |   |   | |   | └─VariableAccessSyntax
//@[23:0026) |   |   |   | |   |   └─IdentifierSyntax
//@[23:0026) |   |   |   | |   |     └─Token(Identifier) |int|
//@[27:0028) |   |   |   | |   ├─Token(LeftSquare) |[|
//@[28:0029) |   |   |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   |   |   | ├─Token(LeftSquare) |[|
//@[31:0032) |   |   |   | └─Token(RightSquare) |]|
//@[33:0034) |   |   |   └─Token(Question) |?|
//@[34:0035) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\n\n|

  typeRefProp: bar
//@[02:0018) |   ├─ObjectTypePropertySyntax
//@[02:0013) |   | ├─IdentifierSyntax
//@[02:0013) |   | | └─Token(Identifier) |typeRefProp|
//@[13:0014) |   | ├─Token(Colon) |:|
//@[15:0018) |   | └─VariableAccessSyntax
//@[15:0018) |   |   └─IdentifierSyntax
//@[15:0018) |   |     └─Token(Identifier) |bar|
//@[18:0020) |   ├─Token(NewLine) |\n\n|

  literalProp: 'literal'
//@[02:0024) |   ├─ObjectTypePropertySyntax
//@[02:0013) |   | ├─IdentifierSyntax
//@[02:0013) |   | | └─Token(Identifier) |literalProp|
//@[13:0014) |   | ├─Token(Colon) |:|
//@[15:0024) |   | └─StringSyntax
//@[15:0024) |   |   └─Token(StringComplete) |'literal'|
//@[24:0026) |   ├─Token(NewLine) |\n\n|

  recursion: foo?
//@[02:0017) |   ├─ObjectTypePropertySyntax
//@[02:0011) |   | ├─IdentifierSyntax
//@[02:0011) |   | | └─Token(Identifier) |recursion|
//@[11:0012) |   | ├─Token(Colon) |:|
//@[13:0017) |   | └─NullableTypeSyntax
//@[13:0016) |   |   ├─VariableAccessSyntax
//@[13:0016) |   |   | └─IdentifierSyntax
//@[13:0016) |   |   |   └─Token(Identifier) |foo|
//@[16:0017) |   |   └─Token(Question) |?|
//@[17:0018) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

@minLength(3)
//@[00:0163) ├─TypeDeclarationSyntax
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |minLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0012) | |   ├─FunctionArgumentSyntax
//@[11:0012) | |   | └─IntegerLiteralSyntax
//@[11:0012) | |   |   └─Token(Integer) |3|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0014) | ├─Token(NewLine) |\n|
@description('An array of array of arrays of arrays of ints')
//@[00:0061) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0061) | | └─FunctionCallSyntax
//@[01:0012) | |   ├─IdentifierSyntax
//@[01:0012) | |   | └─Token(Identifier) |description|
//@[12:0013) | |   ├─Token(LeftParen) |(|
//@[13:0060) | |   ├─FunctionArgumentSyntax
//@[13:0060) | |   | └─StringSyntax
//@[13:0060) | |   |   └─Token(StringComplete) |'An array of array of arrays of arrays of ints'|
//@[60:0061) | |   └─Token(RightParen) |)|
//@[61:0062) | ├─Token(NewLine) |\n|
@metadata({
//@[00:0064) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0064) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |metadata|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0063) | |   ├─FunctionArgumentSyntax
//@[10:0063) | |   | └─ObjectSyntax
//@[10:0011) | |   |   ├─Token(LeftBrace) |{|
//@[11:0012) | |   |   ├─Token(NewLine) |\n|
  examples: [
//@[02:0049) | |   |   ├─ObjectPropertySyntax
//@[02:0010) | |   |   | ├─IdentifierSyntax
//@[02:0010) | |   |   | | └─Token(Identifier) |examples|
//@[10:0011) | |   |   | ├─Token(Colon) |:|
//@[12:0049) | |   |   | └─ArraySyntax
//@[12:0013) | |   |   |   ├─Token(LeftSquare) |[|
//@[13:0014) | |   |   |   ├─Token(NewLine) |\n|
    [[[[1]]], [[[2]]], [[[3]]]]
//@[04:0031) | |   |   |   ├─ArrayItemSyntax
//@[04:0031) | |   |   |   | └─ArraySyntax
//@[04:0005) | |   |   |   |   ├─Token(LeftSquare) |[|
//@[05:0012) | |   |   |   |   ├─ArrayItemSyntax
//@[05:0012) | |   |   |   |   | └─ArraySyntax
//@[05:0006) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[06:0011) | |   |   |   |   |   ├─ArrayItemSyntax
//@[06:0011) | |   |   |   |   |   | └─ArraySyntax
//@[06:0007) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[07:0010) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[07:0010) | |   |   |   |   |   |   | └─ArraySyntax
//@[07:0008) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[08:0009) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[08:0009) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[08:0009) | |   |   |   |   |   |   |   |   └─Token(Integer) |1|
//@[09:0010) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[10:0011) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[11:0012) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[12:0013) | |   |   |   |   ├─Token(Comma) |,|
//@[14:0021) | |   |   |   |   ├─ArrayItemSyntax
//@[14:0021) | |   |   |   |   | └─ArraySyntax
//@[14:0015) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[15:0020) | |   |   |   |   |   ├─ArrayItemSyntax
//@[15:0020) | |   |   |   |   |   | └─ArraySyntax
//@[15:0016) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[16:0019) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[16:0019) | |   |   |   |   |   |   | └─ArraySyntax
//@[16:0017) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[17:0018) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[17:0018) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[17:0018) | |   |   |   |   |   |   |   |   └─Token(Integer) |2|
//@[18:0019) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[19:0020) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[20:0021) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[21:0022) | |   |   |   |   ├─Token(Comma) |,|
//@[23:0030) | |   |   |   |   ├─ArrayItemSyntax
//@[23:0030) | |   |   |   |   | └─ArraySyntax
//@[23:0024) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[24:0029) | |   |   |   |   |   ├─ArrayItemSyntax
//@[24:0029) | |   |   |   |   |   | └─ArraySyntax
//@[24:0025) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[25:0028) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[25:0028) | |   |   |   |   |   |   | └─ArraySyntax
//@[25:0026) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[26:0027) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[26:0027) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[26:0027) | |   |   |   |   |   |   |   |   └─Token(Integer) |3|
//@[27:0028) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[28:0029) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[29:0030) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[30:0031) | |   |   |   |   └─Token(RightSquare) |]|
//@[31:0032) | |   |   |   ├─Token(NewLine) |\n|
  ]
//@[02:0003) | |   |   |   └─Token(RightSquare) |]|
//@[03:0004) | |   |   ├─Token(NewLine) |\n|
})
//@[00:0001) | |   |   └─Token(RightBrace) |}|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0003) | ├─Token(NewLine) |\n|
type bar = int[][][][]
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0008) | ├─IdentifierSyntax
//@[05:0008) | | └─Token(Identifier) |bar|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0022) | └─ArrayTypeSyntax
//@[11:0020) |   ├─ArrayTypeMemberSyntax
//@[11:0020) |   | └─ArrayTypeSyntax
//@[11:0018) |   |   ├─ArrayTypeMemberSyntax
//@[11:0018) |   |   | └─ArrayTypeSyntax
//@[11:0016) |   |   |   ├─ArrayTypeMemberSyntax
//@[11:0016) |   |   |   | └─ArrayTypeSyntax
//@[11:0014) |   |   |   |   ├─ArrayTypeMemberSyntax
//@[11:0014) |   |   |   |   | └─VariableAccessSyntax
//@[11:0014) |   |   |   |   |   └─IdentifierSyntax
//@[11:0014) |   |   |   |   |     └─Token(Identifier) |int|
//@[14:0015) |   |   |   |   ├─Token(LeftSquare) |[|
//@[15:0016) |   |   |   |   └─Token(RightSquare) |]|
//@[16:0017) |   |   |   ├─Token(LeftSquare) |[|
//@[17:0018) |   |   |   └─Token(RightSquare) |]|
//@[18:0019) |   |   ├─Token(LeftSquare) |[|
//@[19:0020) |   |   └─Token(RightSquare) |]|
//@[20:0021) |   ├─Token(LeftSquare) |[|
//@[21:0022) |   └─Token(RightSquare) |]|
//@[22:0024) ├─Token(NewLine) |\n\n|

type aUnion = 'snap'|'crackle'|'pop'
//@[00:0036) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |aUnion|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0036) | └─UnionTypeSyntax
//@[14:0020) |   ├─UnionTypeMemberSyntax
//@[14:0020) |   | └─StringSyntax
//@[14:0020) |   |   └─Token(StringComplete) |'snap'|
//@[20:0021) |   ├─Token(Pipe) |||
//@[21:0030) |   ├─UnionTypeMemberSyntax
//@[21:0030) |   | └─StringSyntax
//@[21:0030) |   |   └─Token(StringComplete) |'crackle'|
//@[30:0031) |   ├─Token(Pipe) |||
//@[31:0036) |   └─UnionTypeMemberSyntax
//@[31:0036) |     └─StringSyntax
//@[31:0036) |       └─Token(StringComplete) |'pop'|
//@[36:0038) ├─Token(NewLine) |\n\n|

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[00:0047) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0018) | ├─IdentifierSyntax
//@[05:0018) | | └─Token(Identifier) |expandedUnion|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0047) | └─UnionTypeSyntax
//@[21:0027) |   ├─UnionTypeMemberSyntax
//@[21:0027) |   | └─VariableAccessSyntax
//@[21:0027) |   |   └─IdentifierSyntax
//@[21:0027) |   |     └─Token(Identifier) |aUnion|
//@[27:0028) |   ├─Token(Pipe) |||
//@[28:0034) |   ├─UnionTypeMemberSyntax
//@[28:0034) |   | └─StringSyntax
//@[28:0034) |   |   └─Token(StringComplete) |'fizz'|
//@[34:0035) |   ├─Token(Pipe) |||
//@[35:0041) |   ├─UnionTypeMemberSyntax
//@[35:0041) |   | └─StringSyntax
//@[35:0041) |   |   └─Token(StringComplete) |'buzz'|
//@[41:0042) |   ├─Token(Pipe) |||
//@[42:0047) |   └─UnionTypeMemberSyntax
//@[42:0047) |     └─StringSyntax
//@[42:0047) |       └─Token(StringComplete) |'pop'|
//@[47:0049) ├─Token(NewLine) |\n\n|

type tupleUnion = ['foo', 'bar', 'baz']
//@[00:0085) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0015) | ├─IdentifierSyntax
//@[05:0015) | | └─Token(Identifier) |tupleUnion|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0085) | └─UnionTypeSyntax
//@[18:0039) |   ├─UnionTypeMemberSyntax
//@[18:0039) |   | └─TupleTypeSyntax
//@[18:0019) |   |   ├─Token(LeftSquare) |[|
//@[19:0024) |   |   ├─TupleTypeItemSyntax
//@[19:0024) |   |   | └─StringSyntax
//@[19:0024) |   |   |   └─Token(StringComplete) |'foo'|
//@[24:0025) |   |   ├─Token(Comma) |,|
//@[26:0031) |   |   ├─TupleTypeItemSyntax
//@[26:0031) |   |   | └─StringSyntax
//@[26:0031) |   |   |   └─Token(StringComplete) |'bar'|
//@[31:0032) |   |   ├─Token(Comma) |,|
//@[33:0038) |   |   ├─TupleTypeItemSyntax
//@[33:0038) |   |   | └─StringSyntax
//@[33:0038) |   |   |   └─Token(StringComplete) |'baz'|
//@[38:0039) |   |   └─Token(RightSquare) |]|
//@[39:0040) |   ├─Token(NewLine) |\n|
|['fizz', 'buzz']
//@[00:0001) |   ├─Token(Pipe) |||
//@[01:0017) |   ├─UnionTypeMemberSyntax
//@[01:0017) |   | └─TupleTypeSyntax
//@[01:0002) |   |   ├─Token(LeftSquare) |[|
//@[02:0008) |   |   ├─TupleTypeItemSyntax
//@[02:0008) |   |   | └─StringSyntax
//@[02:0008) |   |   |   └─Token(StringComplete) |'fizz'|
//@[08:0009) |   |   ├─Token(Comma) |,|
//@[10:0016) |   |   ├─TupleTypeItemSyntax
//@[10:0016) |   |   | └─StringSyntax
//@[10:0016) |   |   |   └─Token(StringComplete) |'buzz'|
//@[16:0017) |   |   └─Token(RightSquare) |]|
//@[17:0018) |   ├─Token(NewLine) |\n|
|['snap', 'crackle', 'pop']
//@[00:0001) |   ├─Token(Pipe) |||
//@[01:0027) |   └─UnionTypeMemberSyntax
//@[01:0027) |     └─TupleTypeSyntax
//@[01:0002) |       ├─Token(LeftSquare) |[|
//@[02:0008) |       ├─TupleTypeItemSyntax
//@[02:0008) |       | └─StringSyntax
//@[02:0008) |       |   └─Token(StringComplete) |'snap'|
//@[08:0009) |       ├─Token(Comma) |,|
//@[10:0019) |       ├─TupleTypeItemSyntax
//@[10:0019) |       | └─StringSyntax
//@[10:0019) |       |   └─Token(StringComplete) |'crackle'|
//@[19:0020) |       ├─Token(Comma) |,|
//@[21:0026) |       ├─TupleTypeItemSyntax
//@[21:0026) |       | └─StringSyntax
//@[21:0026) |       |   └─Token(StringComplete) |'pop'|
//@[26:0027) |       └─Token(RightSquare) |]|
//@[27:0029) ├─Token(NewLine) |\n\n|

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[00:0090) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0015) | ├─IdentifierSyntax
//@[05:0015) | | └─Token(Identifier) |mixedArray|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0090) | └─ArrayTypeSyntax
//@[18:0088) |   ├─ArrayTypeMemberSyntax
//@[18:0088) |   | └─ParenthesizedExpressionSyntax
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0087) |   |   ├─UnionTypeSyntax
//@[19:0030) |   |   | ├─UnionTypeMemberSyntax
//@[19:0030) |   |   | | └─StringSyntax
//@[19:0030) |   |   | |   └─Token(StringComplete) |'heffalump'|
//@[30:0031) |   |   | ├─Token(Pipe) |||
//@[31:0039) |   |   | ├─UnionTypeMemberSyntax
//@[31:0039) |   |   | | └─StringSyntax
//@[31:0039) |   |   | |   └─Token(StringComplete) |'woozle'|
//@[39:0040) |   |   | ├─Token(Pipe) |||
//@[40:0064) |   |   | ├─UnionTypeMemberSyntax
//@[40:0064) |   |   | | └─ObjectTypeSyntax
//@[40:0041) |   |   | |   ├─Token(LeftBrace) |{|
//@[42:0052) |   |   | |   ├─ObjectTypePropertySyntax
//@[42:0047) |   |   | |   | ├─IdentifierSyntax
//@[42:0047) |   |   | |   | | └─Token(Identifier) |shape|
//@[47:0048) |   |   | |   | ├─Token(Colon) |:|
//@[49:0052) |   |   | |   | └─StringSyntax
//@[49:0052) |   |   | |   |   └─Token(StringComplete) |'*'|
//@[52:0053) |   |   | |   ├─Token(Comma) |,|
//@[54:0063) |   |   | |   ├─ObjectTypePropertySyntax
//@[54:0058) |   |   | |   | ├─IdentifierSyntax
//@[54:0058) |   |   | |   | | └─Token(Identifier) |size|
//@[58:0059) |   |   | |   | ├─Token(Colon) |:|
//@[60:0063) |   |   | |   | └─StringSyntax
//@[60:0063) |   |   | |   |   └─Token(StringComplete) |'*'|
//@[63:0064) |   |   | |   └─Token(RightBrace) |}|
//@[64:0065) |   |   | ├─Token(Pipe) |||
//@[65:0067) |   |   | ├─UnionTypeMemberSyntax
//@[65:0067) |   |   | | └─IntegerLiteralSyntax
//@[65:0067) |   |   | |   └─Token(Integer) |10|
//@[67:0068) |   |   | ├─Token(Pipe) |||
//@[68:0071) |   |   | ├─UnionTypeMemberSyntax
//@[68:0071) |   |   | | └─UnaryOperationSyntax
//@[68:0069) |   |   | |   ├─Token(Minus) |-|
//@[69:0071) |   |   | |   └─IntegerLiteralSyntax
//@[69:0071) |   |   | |     └─Token(Integer) |10|
//@[71:0072) |   |   | ├─Token(Pipe) |||
//@[72:0076) |   |   | ├─UnionTypeMemberSyntax
//@[72:0076) |   |   | | └─BooleanLiteralSyntax
//@[72:0076) |   |   | |   └─Token(TrueKeyword) |true|
//@[76:0077) |   |   | ├─Token(Pipe) |||
//@[77:0082) |   |   | ├─UnionTypeMemberSyntax
//@[77:0082) |   |   | | └─UnaryOperationSyntax
//@[77:0078) |   |   | |   ├─Token(Exclamation) |!|
//@[78:0082) |   |   | |   └─BooleanLiteralSyntax
//@[78:0082) |   |   | |     └─Token(TrueKeyword) |true|
//@[82:0083) |   |   | ├─Token(Pipe) |||
//@[83:0087) |   |   | └─UnionTypeMemberSyntax
//@[83:0087) |   |   |   └─NullLiteralSyntax
//@[83:0087) |   |   |     └─Token(NullKeyword) |null|
//@[87:0088) |   |   └─Token(RightParen) |)|
//@[88:0089) |   ├─Token(LeftSquare) |[|
//@[89:0090) |   └─Token(RightSquare) |]|
//@[90:0092) ├─Token(NewLine) |\n\n|

type bool = string
//@[00:0018) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0009) | ├─IdentifierSyntax
//@[05:0009) | | └─Token(Identifier) |bool|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0018) | └─VariableAccessSyntax
//@[12:0018) |   └─IdentifierSyntax
//@[12:0018) |     └─Token(Identifier) |string|
//@[18:0020) ├─Token(NewLine) |\n\n|

param inlineObjectParam {
//@[00:0127) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0023) | ├─IdentifierSyntax
//@[06:0023) | | └─Token(Identifier) |inlineObjectParam|
//@[24:0084) | ├─ObjectTypeSyntax
//@[24:0025) | | ├─Token(LeftBrace) |{|
//@[25:0026) | | ├─Token(NewLine) |\n|
  foo: string
//@[02:0013) | | ├─ObjectTypePropertySyntax
//@[02:0005) | | | ├─IdentifierSyntax
//@[02:0005) | | | | └─Token(Identifier) |foo|
//@[05:0006) | | | ├─Token(Colon) |:|
//@[07:0013) | | | └─VariableAccessSyntax
//@[07:0013) | | |   └─IdentifierSyntax
//@[07:0013) | | |     └─Token(Identifier) |string|
//@[13:0014) | | ├─Token(NewLine) |\n|
  bar: 100|200|300|400|500
//@[02:0026) | | ├─ObjectTypePropertySyntax
//@[02:0005) | | | ├─IdentifierSyntax
//@[02:0005) | | | | └─Token(Identifier) |bar|
//@[05:0006) | | | ├─Token(Colon) |:|
//@[07:0026) | | | └─UnionTypeSyntax
//@[07:0010) | | |   ├─UnionTypeMemberSyntax
//@[07:0010) | | |   | └─IntegerLiteralSyntax
//@[07:0010) | | |   |   └─Token(Integer) |100|
//@[10:0011) | | |   ├─Token(Pipe) |||
//@[11:0014) | | |   ├─UnionTypeMemberSyntax
//@[11:0014) | | |   | └─IntegerLiteralSyntax
//@[11:0014) | | |   |   └─Token(Integer) |200|
//@[14:0015) | | |   ├─Token(Pipe) |||
//@[15:0018) | | |   ├─UnionTypeMemberSyntax
//@[15:0018) | | |   | └─IntegerLiteralSyntax
//@[15:0018) | | |   |   └─Token(Integer) |300|
//@[18:0019) | | |   ├─Token(Pipe) |||
//@[19:0022) | | |   ├─UnionTypeMemberSyntax
//@[19:0022) | | |   | └─IntegerLiteralSyntax
//@[19:0022) | | |   |   └─Token(Integer) |400|
//@[22:0023) | | |   ├─Token(Pipe) |||
//@[23:0026) | | |   └─UnionTypeMemberSyntax
//@[23:0026) | | |     └─IntegerLiteralSyntax
//@[23:0026) | | |       └─Token(Integer) |500|
//@[26:0027) | | ├─Token(NewLine) |\n|
  baz: sys.bool
//@[02:0015) | | ├─ObjectTypePropertySyntax
//@[02:0005) | | | ├─IdentifierSyntax
//@[02:0005) | | | | └─Token(Identifier) |baz|
//@[05:0006) | | | ├─Token(Colon) |:|
//@[07:0015) | | | └─PropertyAccessSyntax
//@[07:0010) | | |   ├─VariableAccessSyntax
//@[07:0010) | | |   | └─IdentifierSyntax
//@[07:0010) | | |   |   └─Token(Identifier) |sys|
//@[10:0011) | | |   ├─Token(Dot) |.|
//@[11:0015) | | |   └─IdentifierSyntax
//@[11:0015) | | |     └─Token(Identifier) |bool|
//@[15:0016) | | ├─Token(NewLine) |\n|
} = {
//@[00:0001) | | └─Token(RightBrace) |}|
//@[02:0044) | └─ParameterDefaultValueSyntax
//@[02:0003) |   ├─Token(Assignment) |=|
//@[04:0044) |   └─ObjectSyntax
//@[04:0005) |     ├─Token(LeftBrace) |{|
//@[05:0006) |     ├─Token(NewLine) |\n|
  foo: 'foo'
//@[02:0012) |     ├─ObjectPropertySyntax
//@[02:0005) |     | ├─IdentifierSyntax
//@[02:0005) |     | | └─Token(Identifier) |foo|
//@[05:0006) |     | ├─Token(Colon) |:|
//@[07:0012) |     | └─StringSyntax
//@[07:0012) |     |   └─Token(StringComplete) |'foo'|
//@[12:0013) |     ├─Token(NewLine) |\n|
  bar: 300
//@[02:0010) |     ├─ObjectPropertySyntax
//@[02:0005) |     | ├─IdentifierSyntax
//@[02:0005) |     | | └─Token(Identifier) |bar|
//@[05:0006) |     | ├─Token(Colon) |:|
//@[07:0010) |     | └─IntegerLiteralSyntax
//@[07:0010) |     |   └─Token(Integer) |300|
//@[10:0011) |     ├─Token(NewLine) |\n|
  baz: false
//@[02:0012) |     ├─ObjectPropertySyntax
//@[02:0005) |     | ├─IdentifierSyntax
//@[02:0005) |     | | └─Token(Identifier) |baz|
//@[05:0006) |     | ├─Token(Colon) |:|
//@[07:0012) |     | └─BooleanLiteralSyntax
//@[07:0012) |     |   └─Token(FalseKeyword) |false|
//@[12:0013) |     ├─Token(NewLine) |\n|
}
//@[00:0001) |     └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[00:0075) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |unionParam|
//@[17:0054) | ├─UnionTypeSyntax
//@[17:0035) | | ├─UnionTypeMemberSyntax
//@[17:0035) | | | └─ObjectTypeSyntax
//@[17:0018) | | |   ├─Token(LeftBrace) |{|
//@[18:0034) | | |   ├─ObjectTypePropertySyntax
//@[18:0026) | | |   | ├─IdentifierSyntax
//@[18:0026) | | |   | | └─Token(Identifier) |property|
//@[26:0027) | | |   | ├─Token(Colon) |:|
//@[28:0034) | | |   | └─StringSyntax
//@[28:0034) | | |   |   └─Token(StringComplete) |'ping'|
//@[34:0035) | | |   └─Token(RightBrace) |}|
//@[35:0036) | | ├─Token(Pipe) |||
//@[36:0054) | | └─UnionTypeMemberSyntax
//@[36:0054) | |   └─ObjectTypeSyntax
//@[36:0037) | |     ├─Token(LeftBrace) |{|
//@[37:0053) | |     ├─ObjectTypePropertySyntax
//@[37:0045) | |     | ├─IdentifierSyntax
//@[37:0045) | |     | | └─Token(Identifier) |property|
//@[45:0046) | |     | ├─Token(Colon) |:|
//@[47:0053) | |     | └─StringSyntax
//@[47:0053) | |     |   └─Token(StringComplete) |'pong'|
//@[53:0054) | |     └─Token(RightBrace) |}|
//@[55:0075) | └─ParameterDefaultValueSyntax
//@[55:0056) |   ├─Token(Assignment) |=|
//@[57:0075) |   └─ObjectSyntax
//@[57:0058) |     ├─Token(LeftBrace) |{|
//@[58:0074) |     ├─ObjectPropertySyntax
//@[58:0066) |     | ├─IdentifierSyntax
//@[58:0066) |     | | └─Token(Identifier) |property|
//@[66:0067) |     | ├─Token(Colon) |:|
//@[68:0074) |     | └─StringSyntax
//@[68:0074) |     |   └─Token(StringComplete) |'pong'|
//@[74:0075) |     └─Token(RightBrace) |}|
//@[75:0077) ├─Token(NewLine) |\n\n|

param paramUsingType mixedArray
//@[00:0031) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0020) | ├─IdentifierSyntax
//@[06:0020) | | └─Token(Identifier) |paramUsingType|
//@[21:0031) | └─VariableAccessSyntax
//@[21:0031) |   └─IdentifierSyntax
//@[21:0031) |     └─Token(Identifier) |mixedArray|
//@[31:0033) ├─Token(NewLine) |\n\n|

output outputUsingType mixedArray = paramUsingType
//@[00:0050) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0022) | ├─IdentifierSyntax
//@[07:0022) | | └─Token(Identifier) |outputUsingType|
//@[23:0033) | ├─VariableAccessSyntax
//@[23:0033) | | └─IdentifierSyntax
//@[23:0033) | |   └─Token(Identifier) |mixedArray|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0050) | └─VariableAccessSyntax
//@[36:0050) |   └─IdentifierSyntax
//@[36:0050) |     └─Token(Identifier) |paramUsingType|
//@[50:0052) ├─Token(NewLine) |\n\n|

type tuple = [
//@[00:0129) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |tuple|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0129) | └─TupleTypeSyntax
//@[13:0014) |   ├─Token(LeftSquare) |[|
//@[14:0015) |   ├─Token(NewLine) |\n|
    @description('A leading string')
//@[04:0047) |   ├─TupleTypeItemSyntax
//@[04:0036) |   | ├─DecoratorSyntax
//@[04:0005) |   | | ├─Token(At) |@|
//@[05:0036) |   | | └─FunctionCallSyntax
//@[05:0016) |   | |   ├─IdentifierSyntax
//@[05:0016) |   | |   | └─Token(Identifier) |description|
//@[16:0017) |   | |   ├─Token(LeftParen) |(|
//@[17:0035) |   | |   ├─FunctionArgumentSyntax
//@[17:0035) |   | |   | └─StringSyntax
//@[17:0035) |   | |   |   └─Token(StringComplete) |'A leading string'|
//@[35:0036) |   | |   └─Token(RightParen) |)|
//@[36:0037) |   | ├─Token(NewLine) |\n|
    string
//@[04:0010) |   | └─VariableAccessSyntax
//@[04:0010) |   |   └─IdentifierSyntax
//@[04:0010) |   |     └─Token(Identifier) |string|
//@[10:0012) |   ├─Token(NewLine) |\n\n|

    @description('A second element using a type alias')
//@[04:0063) |   ├─TupleTypeItemSyntax
//@[04:0055) |   | ├─DecoratorSyntax
//@[04:0005) |   | | ├─Token(At) |@|
//@[05:0055) |   | | └─FunctionCallSyntax
//@[05:0016) |   | |   ├─IdentifierSyntax
//@[05:0016) |   | |   | └─Token(Identifier) |description|
//@[16:0017) |   | |   ├─Token(LeftParen) |(|
//@[17:0054) |   | |   ├─FunctionArgumentSyntax
//@[17:0054) |   | |   | └─StringSyntax
//@[17:0054) |   | |   |   └─Token(StringComplete) |'A second element using a type alias'|
//@[54:0055) |   | |   └─Token(RightParen) |)|
//@[55:0056) |   | ├─Token(NewLine) |\n|
    bar
//@[04:0007) |   | └─VariableAccessSyntax
//@[04:0007) |   |   └─IdentifierSyntax
//@[04:0007) |   |     └─Token(Identifier) |bar|
//@[07:0008) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0003) ├─Token(NewLine) |\n\n|

type stringStringDictionary = {
//@[00:0047) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0027) | ├─IdentifierSyntax
//@[05:0027) | | └─Token(Identifier) |stringStringDictionary|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0047) | └─ObjectTypeSyntax
//@[30:0031) |   ├─Token(LeftBrace) |{|
//@[31:0032) |   ├─Token(NewLine) |\n|
    *: string
//@[04:0013) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[04:0005) |   | ├─Token(Asterisk) |*|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0013) |   | └─VariableAccessSyntax
//@[07:0013) |   |   └─IdentifierSyntax
//@[07:0013) |   |     └─Token(Identifier) |string|
//@[13:0014) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

@minValue(1)
//@[00:0052) ├─TypeDeclarationSyntax
//@[00:0012) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0012) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |minValue|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0011) | |   ├─FunctionArgumentSyntax
//@[10:0011) | |   | └─IntegerLiteralSyntax
//@[10:0011) | |   |   └─Token(Integer) |1|
//@[11:0012) | |   └─Token(RightParen) |)|
//@[12:0013) | ├─Token(NewLine) |\n|
@maxValue(10)
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |maxValue|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0012) | |   ├─FunctionArgumentSyntax
//@[10:0012) | |   | └─IntegerLiteralSyntax
//@[10:0012) | |   |   └─Token(Integer) |10|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0014) | ├─Token(NewLine) |\n|
type constrainedInt = int
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0019) | ├─IdentifierSyntax
//@[05:0019) | | └─Token(Identifier) |constrainedInt|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0025) | └─VariableAccessSyntax
//@[22:0025) |   └─IdentifierSyntax
//@[22:0025) |     └─Token(Identifier) |int|
//@[25:0027) ├─Token(NewLine) |\n\n|

param mightIncludeNull ({key: 'value'} | null)[]
//@[00:0048) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0022) | ├─IdentifierSyntax
//@[06:0022) | | └─Token(Identifier) |mightIncludeNull|
//@[23:0048) | └─ArrayTypeSyntax
//@[23:0046) |   ├─ArrayTypeMemberSyntax
//@[23:0046) |   | └─ParenthesizedExpressionSyntax
//@[23:0024) |   |   ├─Token(LeftParen) |(|
//@[24:0045) |   |   ├─UnionTypeSyntax
//@[24:0038) |   |   | ├─UnionTypeMemberSyntax
//@[24:0038) |   |   | | └─ObjectTypeSyntax
//@[24:0025) |   |   | |   ├─Token(LeftBrace) |{|
//@[25:0037) |   |   | |   ├─ObjectTypePropertySyntax
//@[25:0028) |   |   | |   | ├─IdentifierSyntax
//@[25:0028) |   |   | |   | | └─Token(Identifier) |key|
//@[28:0029) |   |   | |   | ├─Token(Colon) |:|
//@[30:0037) |   |   | |   | └─StringSyntax
//@[30:0037) |   |   | |   |   └─Token(StringComplete) |'value'|
//@[37:0038) |   |   | |   └─Token(RightBrace) |}|
//@[39:0040) |   |   | ├─Token(Pipe) |||
//@[41:0045) |   |   | └─UnionTypeMemberSyntax
//@[41:0045) |   |   |   └─NullLiteralSyntax
//@[41:0045) |   |   |     └─Token(NullKeyword) |null|
//@[45:0046) |   |   └─Token(RightParen) |)|
//@[46:0047) |   ├─Token(LeftSquare) |[|
//@[47:0048) |   └─Token(RightSquare) |]|
//@[48:0050) ├─Token(NewLine) |\n\n|

var nonNull = mightIncludeNull[0]!.key
//@[00:0038) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |nonNull|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0038) | └─PropertyAccessSyntax
//@[14:0034) |   ├─NonNullAssertionSyntax
//@[14:0033) |   | ├─ArrayAccessSyntax
//@[14:0030) |   | | ├─VariableAccessSyntax
//@[14:0030) |   | | | └─IdentifierSyntax
//@[14:0030) |   | | |   └─Token(Identifier) |mightIncludeNull|
//@[30:0031) |   | | ├─Token(LeftSquare) |[|
//@[31:0032) |   | | ├─IntegerLiteralSyntax
//@[31:0032) |   | | | └─Token(Integer) |0|
//@[32:0033) |   | | └─Token(RightSquare) |]|
//@[33:0034) |   | └─Token(Exclamation) |!|
//@[34:0035) |   ├─Token(Dot) |.|
//@[35:0038) |   └─IdentifierSyntax
//@[35:0038) |     └─Token(Identifier) |key|
//@[38:0040) ├─Token(NewLine) |\n\n|

output nonNull string = nonNull
//@[00:0031) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0014) | ├─IdentifierSyntax
//@[07:0014) | | └─Token(Identifier) |nonNull|
//@[15:0021) | ├─VariableAccessSyntax
//@[15:0021) | | └─IdentifierSyntax
//@[15:0021) | |   └─Token(Identifier) |string|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0031) | └─VariableAccessSyntax
//@[24:0031) |   └─IdentifierSyntax
//@[24:0031) |     └─Token(Identifier) |nonNull|
//@[31:0033) ├─Token(NewLine) |\n\n|

var maybeNull = mightIncludeNull[0].?key
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0013) | ├─IdentifierSyntax
//@[04:0013) | | └─Token(Identifier) |maybeNull|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0040) | └─PropertyAccessSyntax
//@[16:0035) |   ├─ArrayAccessSyntax
//@[16:0032) |   | ├─VariableAccessSyntax
//@[16:0032) |   | | └─IdentifierSyntax
//@[16:0032) |   | |   └─Token(Identifier) |mightIncludeNull|
//@[32:0033) |   | ├─Token(LeftSquare) |[|
//@[33:0034) |   | ├─IntegerLiteralSyntax
//@[33:0034) |   | | └─Token(Integer) |0|
//@[34:0035) |   | └─Token(RightSquare) |]|
//@[35:0036) |   ├─Token(Dot) |.|
//@[36:0037) |   ├─Token(Question) |?|
//@[37:0040) |   └─IdentifierSyntax
//@[37:0040) |     └─Token(Identifier) |key|
//@[40:0042) ├─Token(NewLine) |\n\n|

output maybeNull string? = maybeNull
//@[00:0036) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0016) | ├─IdentifierSyntax
//@[07:0016) | | └─Token(Identifier) |maybeNull|
//@[17:0024) | ├─NullableTypeSyntax
//@[17:0023) | | ├─VariableAccessSyntax
//@[17:0023) | | | └─IdentifierSyntax
//@[17:0023) | | |   └─Token(Identifier) |string|
//@[23:0024) | | └─Token(Question) |?|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0036) | └─VariableAccessSyntax
//@[27:0036) |   └─IdentifierSyntax
//@[27:0036) |     └─Token(Identifier) |maybeNull|
//@[36:0038) ├─Token(NewLine) |\n\n|

type nullable = string?
//@[00:0023) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |nullable|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0023) | └─NullableTypeSyntax
//@[16:0022) |   ├─VariableAccessSyntax
//@[16:0022) |   | └─IdentifierSyntax
//@[16:0022) |   |   └─Token(Identifier) |string|
//@[22:0023) |   └─Token(Question) |?|
//@[23:0025) ├─Token(NewLine) |\n\n|

type nonNullable = nullable!
//@[00:0028) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0016) | ├─IdentifierSyntax
//@[05:0016) | | └─Token(Identifier) |nonNullable|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0028) | └─NonNullAssertionSyntax
//@[19:0027) |   ├─VariableAccessSyntax
//@[19:0027) |   | └─IdentifierSyntax
//@[19:0027) |   |   └─Token(Identifier) |nullable|
//@[27:0028) |   └─Token(Exclamation) |!|
//@[28:0030) ├─Token(NewLine) |\n\n|

type typeA = {
//@[00:0044) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |typeA|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0044) | └─ObjectTypeSyntax
//@[13:0014) |   ├─Token(LeftBrace) |{|
//@[14:0015) |   ├─Token(NewLine) |\n|
  type: 'a'
//@[02:0011) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |type|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0011) |   | └─StringSyntax
//@[08:0011) |   |   └─Token(StringComplete) |'a'|
//@[11:0012) |   ├─Token(NewLine) |\n|
  value: string
//@[02:0015) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |value|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0015) |   | └─VariableAccessSyntax
//@[09:0015) |   |   └─IdentifierSyntax
//@[09:0015) |   |     └─Token(Identifier) |string|
//@[15:0016) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type typeB = {
//@[00:0041) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |typeB|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0041) | └─ObjectTypeSyntax
//@[13:0014) |   ├─Token(LeftBrace) |{|
//@[14:0015) |   ├─Token(NewLine) |\n|
  type: 'b'
//@[02:0011) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |type|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0011) |   | └─StringSyntax
//@[08:0011) |   |   └─Token(StringComplete) |'b'|
//@[11:0012) |   ├─Token(NewLine) |\n|
  value: int
//@[02:0012) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |value|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0012) |   | └─VariableAccessSyntax
//@[09:0012) |   |   └─IdentifierSyntax
//@[09:0012) |   |     └─Token(Identifier) |int|
//@[12:0013) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type typeC = {
//@[00:0059) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |typeC|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0059) | └─ObjectTypeSyntax
//@[13:0014) |   ├─Token(LeftBrace) |{|
//@[14:0015) |   ├─Token(NewLine) |\n|
  type: 'c'
//@[02:0011) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |type|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0011) |   | └─StringSyntax
//@[08:0011) |   |   └─Token(StringComplete) |'c'|
//@[11:0012) |   ├─Token(NewLine) |\n|
  value: bool
//@[02:0013) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |value|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0013) |   | └─VariableAccessSyntax
//@[09:0013) |   |   └─IdentifierSyntax
//@[09:0013) |   |     └─Token(Identifier) |bool|
//@[13:0014) |   ├─Token(NewLine) |\n|
  value2: string
//@[02:0016) |   ├─ObjectTypePropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |value2|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0016) |   | └─VariableAccessSyntax
//@[10:0016) |   |   └─IdentifierSyntax
//@[10:0016) |   |     └─Token(Identifier) |string|
//@[16:0017) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type typeD = {
//@[00:0044) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |typeD|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0044) | └─ObjectTypeSyntax
//@[13:0014) |   ├─Token(LeftBrace) |{|
//@[14:0015) |   ├─Token(NewLine) |\n|
  type: 'd'
//@[02:0011) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |type|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0011) |   | └─StringSyntax
//@[08:0011) |   |   └─Token(StringComplete) |'d'|
//@[11:0012) |   ├─Token(NewLine) |\n|
  value: object
//@[02:0015) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |value|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0015) |   | └─VariableAccessSyntax
//@[09:0015) |   |   └─IdentifierSyntax
//@[09:0015) |   |     └─Token(Identifier) |object|
//@[15:0016) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type typeE = {
//@[00:0047) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |typeE|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0047) | └─ObjectTypeSyntax
//@[13:0014) |   ├─Token(LeftBrace) |{|
//@[14:0015) |   ├─Token(NewLine) |\n|
  type: 'e'
//@[02:0011) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |type|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0011) |   | └─StringSyntax
//@[08:0011) |   |   └─Token(StringComplete) |'e'|
//@[11:0012) |   ├─Token(NewLine) |\n|
  value: 'a' | 'b'
//@[02:0018) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |value|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0018) |   | └─UnionTypeSyntax
//@[09:0012) |   |   ├─UnionTypeMemberSyntax
//@[09:0012) |   |   | └─StringSyntax
//@[09:0012) |   |   |   └─Token(StringComplete) |'a'|
//@[13:0014) |   |   ├─Token(Pipe) |||
//@[15:0018) |   |   └─UnionTypeMemberSyntax
//@[15:0018) |   |     └─StringSyntax
//@[15:0018) |   |       └─Token(StringComplete) |'b'|
//@[18:0019) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0063) ├─TypeDeclarationSyntax
//@[00:0022) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0022) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0021) | |   ├─FunctionArgumentSyntax
//@[15:0021) | |   | └─StringSyntax
//@[15:0021) | |   |   └─Token(StringComplete) |'type'|
//@[21:0022) | |   └─Token(RightParen) |)|
//@[22:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion1 = typeA | typeB
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0024) | ├─IdentifierSyntax
//@[05:0024) | | └─Token(Identifier) |discriminatedUnion1|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0040) | └─UnionTypeSyntax
//@[27:0032) |   ├─UnionTypeMemberSyntax
//@[27:0032) |   | └─VariableAccessSyntax
//@[27:0032) |   |   └─IdentifierSyntax
//@[27:0032) |   |     └─Token(Identifier) |typeA|
//@[33:0034) |   ├─Token(Pipe) |||
//@[35:0040) |   └─UnionTypeMemberSyntax
//@[35:0040) |     └─VariableAccessSyntax
//@[35:0040) |       └─IdentifierSyntax
//@[35:0040) |         └─Token(Identifier) |typeB|
//@[40:0042) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0107) ├─TypeDeclarationSyntax
//@[00:0022) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0022) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0021) | |   ├─FunctionArgumentSyntax
//@[15:0021) | |   | └─StringSyntax
//@[15:0021) | |   |   └─Token(StringComplete) |'type'|
//@[21:0022) | |   └─Token(RightParen) |)|
//@[22:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0024) | ├─IdentifierSyntax
//@[05:0024) | | └─Token(Identifier) |discriminatedUnion2|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0084) | └─UnionTypeSyntax
//@[27:0055) |   ├─UnionTypeMemberSyntax
//@[27:0055) |   | └─ObjectTypeSyntax
//@[27:0028) |   |   ├─Token(LeftBrace) |{|
//@[29:0038) |   |   ├─ObjectTypePropertySyntax
//@[29:0033) |   |   | ├─IdentifierSyntax
//@[29:0033) |   |   | | └─Token(Identifier) |type|
//@[33:0034) |   |   | ├─Token(Colon) |:|
//@[35:0038) |   |   | └─StringSyntax
//@[35:0038) |   |   |   └─Token(StringComplete) |'c'|
//@[38:0039) |   |   ├─Token(Comma) |,|
//@[40:0053) |   |   ├─ObjectTypePropertySyntax
//@[40:0045) |   |   | ├─IdentifierSyntax
//@[40:0045) |   |   | | └─Token(Identifier) |value|
//@[45:0046) |   |   | ├─Token(Colon) |:|
//@[47:0053) |   |   | └─VariableAccessSyntax
//@[47:0053) |   |   |   └─IdentifierSyntax
//@[47:0053) |   |   |     └─Token(Identifier) |string|
//@[54:0055) |   |   └─Token(RightBrace) |}|
//@[56:0057) |   ├─Token(Pipe) |||
//@[58:0084) |   └─UnionTypeMemberSyntax
//@[58:0084) |     └─ObjectTypeSyntax
//@[58:0059) |       ├─Token(LeftBrace) |{|
//@[60:0069) |       ├─ObjectTypePropertySyntax
//@[60:0064) |       | ├─IdentifierSyntax
//@[60:0064) |       | | └─Token(Identifier) |type|
//@[64:0065) |       | ├─Token(Colon) |:|
//@[66:0069) |       | └─StringSyntax
//@[66:0069) |       |   └─Token(StringComplete) |'d'|
//@[69:0070) |       ├─Token(Comma) |,|
//@[71:0082) |       ├─ObjectTypePropertySyntax
//@[71:0076) |       | ├─IdentifierSyntax
//@[71:0076) |       | | └─Token(Identifier) |value|
//@[76:0077) |       | ├─Token(Colon) |:|
//@[78:0082) |       | └─VariableAccessSyntax
//@[78:0082) |       |   └─IdentifierSyntax
//@[78:0082) |       |     └─Token(Identifier) |bool|
//@[83:0084) |       └─Token(RightBrace) |}|
//@[84:0086) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0122) ├─TypeDeclarationSyntax
//@[00:0022) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0022) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0021) | |   ├─FunctionArgumentSyntax
//@[15:0021) | |   | └─StringSyntax
//@[15:0021) | |   |   └─Token(StringComplete) |'type'|
//@[21:0022) | |   └─Token(RightParen) |)|
//@[22:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0024) | ├─IdentifierSyntax
//@[05:0024) | | └─Token(Identifier) |discriminatedUnion3|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0099) | └─UnionTypeSyntax
//@[27:0046) |   ├─UnionTypeMemberSyntax
//@[27:0046) |   | └─VariableAccessSyntax
//@[27:0046) |   |   └─IdentifierSyntax
//@[27:0046) |   |     └─Token(Identifier) |discriminatedUnion1|
//@[47:0048) |   ├─Token(Pipe) |||
//@[49:0068) |   ├─UnionTypeMemberSyntax
//@[49:0068) |   | └─VariableAccessSyntax
//@[49:0068) |   |   └─IdentifierSyntax
//@[49:0068) |   |     └─Token(Identifier) |discriminatedUnion2|
//@[69:0070) |   ├─Token(Pipe) |||
//@[71:0099) |   └─UnionTypeMemberSyntax
//@[71:0099) |     └─ObjectTypeSyntax
//@[71:0072) |       ├─Token(LeftBrace) |{|
//@[73:0082) |       ├─ObjectTypePropertySyntax
//@[73:0077) |       | ├─IdentifierSyntax
//@[73:0077) |       | | └─Token(Identifier) |type|
//@[77:0078) |       | ├─Token(Colon) |:|
//@[79:0082) |       | └─StringSyntax
//@[79:0082) |       |   └─Token(StringComplete) |'e'|
//@[82:0083) |       ├─Token(Comma) |,|
//@[84:0097) |       ├─ObjectTypePropertySyntax
//@[84:0089) |       | ├─IdentifierSyntax
//@[84:0089) |       | | └─Token(Identifier) |value|
//@[89:0090) |       | ├─Token(Colon) |:|
//@[91:0097) |       | └─VariableAccessSyntax
//@[91:0097) |       |   └─IdentifierSyntax
//@[91:0097) |       |     └─Token(Identifier) |string|
//@[98:0099) |       └─Token(RightBrace) |}|
//@[99:0101) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0101) ├─TypeDeclarationSyntax
//@[00:0022) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0022) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0021) | |   ├─FunctionArgumentSyntax
//@[15:0021) | |   | └─StringSyntax
//@[15:0021) | |   |   └─Token(StringComplete) |'type'|
//@[21:0022) | |   └─Token(RightParen) |)|
//@[22:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0024) | ├─IdentifierSyntax
//@[05:0024) | | └─Token(Identifier) |discriminatedUnion4|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0078) | └─UnionTypeSyntax
//@[27:0046) |   ├─UnionTypeMemberSyntax
//@[27:0046) |   | └─VariableAccessSyntax
//@[27:0046) |   |   └─IdentifierSyntax
//@[27:0046) |   |     └─Token(Identifier) |discriminatedUnion1|
//@[47:0048) |   ├─Token(Pipe) |||
//@[49:0078) |   └─UnionTypeMemberSyntax
//@[49:0078) |     └─ParenthesizedExpressionSyntax
//@[49:0050) |       ├─Token(LeftParen) |(|
//@[50:0077) |       ├─UnionTypeSyntax
//@[50:0069) |       | ├─UnionTypeMemberSyntax
//@[50:0069) |       | | └─VariableAccessSyntax
//@[50:0069) |       | |   └─IdentifierSyntax
//@[50:0069) |       | |     └─Token(Identifier) |discriminatedUnion2|
//@[70:0071) |       | ├─Token(Pipe) |||
//@[72:0077) |       | └─UnionTypeMemberSyntax
//@[72:0077) |       |   └─VariableAccessSyntax
//@[72:0077) |       |     └─IdentifierSyntax
//@[72:0077) |       |       └─Token(Identifier) |typeE|
//@[77:0078) |       └─Token(RightParen) |)|
//@[78:0080) ├─Token(NewLine) |\n\n|

type inlineDiscriminatedUnion1 = {
//@[00:0083) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0030) | ├─IdentifierSyntax
//@[05:0030) | | └─Token(Identifier) |inlineDiscriminatedUnion1|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0083) | └─ObjectTypeSyntax
//@[33:0034) |   ├─Token(LeftBrace) |{|
//@[34:0035) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[02:0046) |   ├─ObjectTypePropertySyntax
//@[02:0024) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0024) |   | | └─FunctionCallSyntax
//@[03:0016) |   | |   ├─IdentifierSyntax
//@[03:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[16:0017) |   | |   ├─Token(LeftParen) |(|
//@[17:0023) |   | |   ├─FunctionArgumentSyntax
//@[17:0023) |   | |   | └─StringSyntax
//@[17:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[23:0024) |   | |   └─Token(RightParen) |)|
//@[24:0025) |   | ├─Token(NewLine) |\n|
  prop: typeA | typeC
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |prop|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0021) |   | └─UnionTypeSyntax
//@[08:0013) |   |   ├─UnionTypeMemberSyntax
//@[08:0013) |   |   | └─VariableAccessSyntax
//@[08:0013) |   |   |   └─IdentifierSyntax
//@[08:0013) |   |   |     └─Token(Identifier) |typeA|
//@[14:0015) |   |   ├─Token(Pipe) |||
//@[16:0021) |   |   └─UnionTypeMemberSyntax
//@[16:0021) |   |     └─VariableAccessSyntax
//@[16:0021) |   |       └─IdentifierSyntax
//@[16:0021) |   |         └─Token(Identifier) |typeC|
//@[21:0022) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type inlineDiscriminatedUnion2 = {
//@[00:0104) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0030) | ├─IdentifierSyntax
//@[05:0030) | | └─Token(Identifier) |inlineDiscriminatedUnion2|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0104) | └─ObjectTypeSyntax
//@[33:0034) |   ├─Token(LeftBrace) |{|
//@[34:0035) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[02:0067) |   ├─ObjectTypePropertySyntax
//@[02:0024) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0024) |   | | └─FunctionCallSyntax
//@[03:0016) |   | |   ├─IdentifierSyntax
//@[03:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[16:0017) |   | |   ├─Token(LeftParen) |(|
//@[17:0023) |   | |   ├─FunctionArgumentSyntax
//@[17:0023) |   | |   | └─StringSyntax
//@[17:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[23:0024) |   | |   └─Token(RightParen) |)|
//@[24:0025) |   | ├─Token(NewLine) |\n|
  prop: { type: 'a', value: bool } | typeB
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |prop|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0042) |   | └─UnionTypeSyntax
//@[08:0034) |   |   ├─UnionTypeMemberSyntax
//@[08:0034) |   |   | └─ObjectTypeSyntax
//@[08:0009) |   |   |   ├─Token(LeftBrace) |{|
//@[10:0019) |   |   |   ├─ObjectTypePropertySyntax
//@[10:0014) |   |   |   | ├─IdentifierSyntax
//@[10:0014) |   |   |   | | └─Token(Identifier) |type|
//@[14:0015) |   |   |   | ├─Token(Colon) |:|
//@[16:0019) |   |   |   | └─StringSyntax
//@[16:0019) |   |   |   |   └─Token(StringComplete) |'a'|
//@[19:0020) |   |   |   ├─Token(Comma) |,|
//@[21:0032) |   |   |   ├─ObjectTypePropertySyntax
//@[21:0026) |   |   |   | ├─IdentifierSyntax
//@[21:0026) |   |   |   | | └─Token(Identifier) |value|
//@[26:0027) |   |   |   | ├─Token(Colon) |:|
//@[28:0032) |   |   |   | └─VariableAccessSyntax
//@[28:0032) |   |   |   |   └─IdentifierSyntax
//@[28:0032) |   |   |   |     └─Token(Identifier) |bool|
//@[33:0034) |   |   |   └─Token(RightBrace) |}|
//@[35:0036) |   |   ├─Token(Pipe) |||
//@[37:0042) |   |   └─UnionTypeMemberSyntax
//@[37:0042) |   |     └─VariableAccessSyntax
//@[37:0042) |   |       └─IdentifierSyntax
//@[37:0042) |   |         └─Token(Identifier) |typeB|
//@[42:0043) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0232) ├─TypeDeclarationSyntax
//@[00:0022) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0022) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0021) | |   ├─FunctionArgumentSyntax
//@[15:0021) | |   | └─StringSyntax
//@[15:0021) | |   |   └─Token(StringComplete) |'type'|
//@[21:0022) | |   └─Token(RightParen) |)|
//@[22:0023) | ├─Token(NewLine) |\n|
type inlineDiscriminatedUnion3 = {
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0030) | ├─IdentifierSyntax
//@[05:0030) | | └─Token(Identifier) |inlineDiscriminatedUnion3|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0209) | └─UnionTypeSyntax
//@[33:0116) |   ├─UnionTypeMemberSyntax
//@[33:0116) |   | └─ObjectTypeSyntax
//@[33:0034) |   |   ├─Token(LeftBrace) |{|
//@[34:0035) |   |   ├─Token(NewLine) |\n|
  type: 'a'
//@[02:0011) |   |   ├─ObjectTypePropertySyntax
//@[02:0006) |   |   | ├─IdentifierSyntax
//@[02:0006) |   |   | | └─Token(Identifier) |type|
//@[06:0007) |   |   | ├─Token(Colon) |:|
//@[08:0011) |   |   | └─StringSyntax
//@[08:0011) |   |   |   └─Token(StringComplete) |'a'|
//@[11:0012) |   |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[02:0067) |   |   ├─ObjectTypePropertySyntax
//@[02:0024) |   |   | ├─DecoratorSyntax
//@[02:0003) |   |   | | ├─Token(At) |@|
//@[03:0024) |   |   | | └─FunctionCallSyntax
//@[03:0016) |   |   | |   ├─IdentifierSyntax
//@[03:0016) |   |   | |   | └─Token(Identifier) |discriminator|
//@[16:0017) |   |   | |   ├─Token(LeftParen) |(|
//@[17:0023) |   |   | |   ├─FunctionArgumentSyntax
//@[17:0023) |   |   | |   | └─StringSyntax
//@[17:0023) |   |   | |   |   └─Token(StringComplete) |'type'|
//@[23:0024) |   |   | |   └─Token(RightParen) |)|
//@[24:0025) |   |   | ├─Token(NewLine) |\n|
  prop: { type: 'a', value: bool } | typeB
//@[02:0006) |   |   | ├─IdentifierSyntax
//@[02:0006) |   |   | | └─Token(Identifier) |prop|
//@[06:0007) |   |   | ├─Token(Colon) |:|
//@[08:0042) |   |   | └─UnionTypeSyntax
//@[08:0034) |   |   |   ├─UnionTypeMemberSyntax
//@[08:0034) |   |   |   | └─ObjectTypeSyntax
//@[08:0009) |   |   |   |   ├─Token(LeftBrace) |{|
//@[10:0019) |   |   |   |   ├─ObjectTypePropertySyntax
//@[10:0014) |   |   |   |   | ├─IdentifierSyntax
//@[10:0014) |   |   |   |   | | └─Token(Identifier) |type|
//@[14:0015) |   |   |   |   | ├─Token(Colon) |:|
//@[16:0019) |   |   |   |   | └─StringSyntax
//@[16:0019) |   |   |   |   |   └─Token(StringComplete) |'a'|
//@[19:0020) |   |   |   |   ├─Token(Comma) |,|
//@[21:0032) |   |   |   |   ├─ObjectTypePropertySyntax
//@[21:0026) |   |   |   |   | ├─IdentifierSyntax
//@[21:0026) |   |   |   |   | | └─Token(Identifier) |value|
//@[26:0027) |   |   |   |   | ├─Token(Colon) |:|
//@[28:0032) |   |   |   |   | └─VariableAccessSyntax
//@[28:0032) |   |   |   |   |   └─IdentifierSyntax
//@[28:0032) |   |   |   |   |     └─Token(Identifier) |bool|
//@[33:0034) |   |   |   |   └─Token(RightBrace) |}|
//@[35:0036) |   |   |   ├─Token(Pipe) |||
//@[37:0042) |   |   |   └─UnionTypeMemberSyntax
//@[37:0042) |   |   |     └─VariableAccessSyntax
//@[37:0042) |   |   |       └─IdentifierSyntax
//@[37:0042) |   |   |         └─Token(Identifier) |typeB|
//@[42:0043) |   |   ├─Token(NewLine) |\n|
} | {
//@[00:0001) |   |   └─Token(RightBrace) |}|
//@[02:0003) |   ├─Token(Pipe) |||
//@[04:0094) |   └─UnionTypeMemberSyntax
//@[04:0094) |     └─ObjectTypeSyntax
//@[04:0005) |       ├─Token(LeftBrace) |{|
//@[05:0006) |       ├─Token(NewLine) |\n|
  type: 'b'
//@[02:0011) |       ├─ObjectTypePropertySyntax
//@[02:0006) |       | ├─IdentifierSyntax
//@[02:0006) |       | | └─Token(Identifier) |type|
//@[06:0007) |       | ├─Token(Colon) |:|
//@[08:0011) |       | └─StringSyntax
//@[08:0011) |       |   └─Token(StringComplete) |'b'|
//@[11:0012) |       ├─Token(NewLine) |\n|
  @discriminator('type')
//@[02:0074) |       ├─ObjectTypePropertySyntax
//@[02:0024) |       | ├─DecoratorSyntax
//@[02:0003) |       | | ├─Token(At) |@|
//@[03:0024) |       | | └─FunctionCallSyntax
//@[03:0016) |       | |   ├─IdentifierSyntax
//@[03:0016) |       | |   | └─Token(Identifier) |discriminator|
//@[16:0017) |       | |   ├─Token(LeftParen) |(|
//@[17:0023) |       | |   ├─FunctionArgumentSyntax
//@[17:0023) |       | |   | └─StringSyntax
//@[17:0023) |       | |   |   └─Token(StringComplete) |'type'|
//@[23:0024) |       | |   └─Token(RightParen) |)|
//@[24:0025) |       | ├─Token(NewLine) |\n|
  prop: discriminatedUnion1 | discriminatedUnion2
//@[02:0006) |       | ├─IdentifierSyntax
//@[02:0006) |       | | └─Token(Identifier) |prop|
//@[06:0007) |       | ├─Token(Colon) |:|
//@[08:0049) |       | └─UnionTypeSyntax
//@[08:0027) |       |   ├─UnionTypeMemberSyntax
//@[08:0027) |       |   | └─VariableAccessSyntax
//@[08:0027) |       |   |   └─IdentifierSyntax
//@[08:0027) |       |   |     └─Token(Identifier) |discriminatedUnion1|
//@[28:0029) |       |   ├─Token(Pipe) |||
//@[30:0049) |       |   └─UnionTypeMemberSyntax
//@[30:0049) |       |     └─VariableAccessSyntax
//@[30:0049) |       |       └─IdentifierSyntax
//@[30:0049) |       |         └─Token(Identifier) |discriminatedUnion2|
//@[49:0050) |       ├─Token(NewLine) |\n|
}
//@[00:0001) |       └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type discriminatorUnionAsPropertyType = {
//@[00:0101) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0037) | ├─IdentifierSyntax
//@[05:0037) | | └─Token(Identifier) |discriminatorUnionAsPropertyType|
//@[38:0039) | ├─Token(Assignment) |=|
//@[40:0101) | └─ObjectTypeSyntax
//@[40:0041) |   ├─Token(LeftBrace) |{|
//@[41:0042) |   ├─Token(NewLine) |\n|
  prop1: discriminatedUnion1
//@[02:0028) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |prop1|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0028) |   | └─VariableAccessSyntax
//@[09:0028) |   |   └─IdentifierSyntax
//@[09:0028) |   |     └─Token(Identifier) |discriminatedUnion1|
//@[28:0029) |   ├─Token(NewLine) |\n|
  prop2: discriminatedUnion3
//@[02:0028) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |prop2|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0028) |   | └─VariableAccessSyntax
//@[09:0028) |   |   └─IdentifierSyntax
//@[09:0028) |   |     └─Token(Identifier) |discriminatedUnion3|
//@[28:0029) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0137) ├─TypeDeclarationSyntax
//@[00:0022) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0022) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0021) | |   ├─FunctionArgumentSyntax
//@[15:0021) | |   | └─StringSyntax
//@[15:0021) | |   |   └─Token(StringComplete) |'type'|
//@[21:0022) | |   └─Token(RightParen) |)|
//@[22:0023) | ├─Token(NewLine) |\n|
type discriminatorInnerSelfOptionalCycle1 = typeA | {
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0041) | ├─IdentifierSyntax
//@[05:0041) | | └─Token(Identifier) |discriminatorInnerSelfOptionalCycle1|
//@[42:0043) | ├─Token(Assignment) |=|
//@[44:0114) | └─UnionTypeSyntax
//@[44:0049) |   ├─UnionTypeMemberSyntax
//@[44:0049) |   | └─VariableAccessSyntax
//@[44:0049) |   |   └─IdentifierSyntax
//@[44:0049) |   |     └─Token(Identifier) |typeA|
//@[50:0051) |   ├─Token(Pipe) |||
//@[52:0114) |   └─UnionTypeMemberSyntax
//@[52:0114) |     └─ObjectTypeSyntax
//@[52:0053) |       ├─Token(LeftBrace) |{|
//@[53:0054) |       ├─Token(NewLine) |\n|
  type: 'b'
//@[02:0011) |       ├─ObjectTypePropertySyntax
//@[02:0006) |       | ├─IdentifierSyntax
//@[02:0006) |       | | └─Token(Identifier) |type|
//@[06:0007) |       | ├─Token(Colon) |:|
//@[08:0011) |       | └─StringSyntax
//@[08:0011) |       |   └─Token(StringComplete) |'b'|
//@[11:0012) |       ├─Token(NewLine) |\n|
  value: discriminatorInnerSelfOptionalCycle1?
//@[02:0046) |       ├─ObjectTypePropertySyntax
//@[02:0007) |       | ├─IdentifierSyntax
//@[02:0007) |       | | └─Token(Identifier) |value|
//@[07:0008) |       | ├─Token(Colon) |:|
//@[09:0046) |       | └─NullableTypeSyntax
//@[09:0045) |       |   ├─VariableAccessSyntax
//@[09:0045) |       |   | └─IdentifierSyntax
//@[09:0045) |       |   |   └─Token(Identifier) |discriminatorInnerSelfOptionalCycle1|
//@[45:0046) |       |   └─Token(Question) |?|
//@[46:0047) |       ├─Token(NewLine) |\n|
}
//@[00:0001) |       └─Token(RightBrace) |}|
//@[01:0001) └─Token(EndOfFile) ||
