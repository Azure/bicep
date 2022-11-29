@description('The foo type')
//@[00:1122) ProgramSyntax
//@[00:0298) ├─TypeDeclarationSyntax
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
//@[11:0259) | └─ObjectTypeSyntax
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
//@[02:0088) |   ├─ObjectTypePropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |objectProp|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0088) |   | └─ObjectTypeSyntax
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

    intArrayArrayProp?: int [] []
//@[04:0033) |   |   ├─ObjectTypePropertySyntax
//@[04:0021) |   |   | ├─IdentifierSyntax
//@[04:0021) |   |   | | └─Token(Identifier) |intArrayArrayProp|
//@[21:0022) |   |   | ├─Token(Question) |?|
//@[22:0023) |   |   | ├─Token(Colon) |:|
//@[24:0033) |   |   | └─ArrayTypeSyntax
//@[24:0030) |   |   |   ├─ArrayTypeMemberSyntax
//@[24:0030) |   |   |   | └─ArrayTypeSyntax
//@[24:0027) |   |   |   |   ├─ArrayTypeMemberSyntax
//@[24:0027) |   |   |   |   | └─VariableAccessSyntax
//@[24:0027) |   |   |   |   |   └─IdentifierSyntax
//@[24:0027) |   |   |   |   |     └─Token(Identifier) |int|
//@[28:0029) |   |   |   |   ├─Token(LeftSquare) |[|
//@[29:0030) |   |   |   |   └─Token(RightSquare) |]|
//@[31:0032) |   |   |   ├─Token(LeftSquare) |[|
//@[32:0033) |   |   |   └─Token(RightSquare) |]|
//@[33:0034) |   |   ├─Token(NewLine) |\n|
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

  recursion?: foo
//@[02:0017) |   ├─ObjectTypePropertySyntax
//@[02:0011) |   | ├─IdentifierSyntax
//@[02:0011) |   | | └─Token(Identifier) |recursion|
//@[11:0012) |   | ├─Token(Question) |?|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0017) |   | └─VariableAccessSyntax
//@[14:0017) |   |   └─IdentifierSyntax
//@[14:0017) |   |     └─Token(Identifier) |foo|
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

type tupleUnion = ['foo', 'bar', 'baz']|['fizz', 'buzz']|['snap', 'crackle', 'pop']
//@[00:0083) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0015) | ├─IdentifierSyntax
//@[05:0015) | | └─Token(Identifier) |tupleUnion|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0083) | └─UnionTypeSyntax
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
//@[39:0040) |   ├─Token(Pipe) |||
//@[40:0056) |   ├─UnionTypeMemberSyntax
//@[40:0056) |   | └─TupleTypeSyntax
//@[40:0041) |   |   ├─Token(LeftSquare) |[|
//@[41:0047) |   |   ├─TupleTypeItemSyntax
//@[41:0047) |   |   | └─StringSyntax
//@[41:0047) |   |   |   └─Token(StringComplete) |'fizz'|
//@[47:0048) |   |   ├─Token(Comma) |,|
//@[49:0055) |   |   ├─TupleTypeItemSyntax
//@[49:0055) |   |   | └─StringSyntax
//@[49:0055) |   |   |   └─Token(StringComplete) |'buzz'|
//@[55:0056) |   |   └─Token(RightSquare) |]|
//@[56:0057) |   ├─Token(Pipe) |||
//@[57:0083) |   └─UnionTypeMemberSyntax
//@[57:0083) |     └─TupleTypeSyntax
//@[57:0058) |       ├─Token(LeftSquare) |[|
//@[58:0064) |       ├─TupleTypeItemSyntax
//@[58:0064) |       | └─StringSyntax
//@[58:0064) |       |   └─Token(StringComplete) |'snap'|
//@[64:0065) |       ├─Token(Comma) |,|
//@[66:0075) |       ├─TupleTypeItemSyntax
//@[66:0075) |       | └─StringSyntax
//@[66:0075) |       |   └─Token(StringComplete) |'crackle'|
//@[75:0076) |       ├─Token(Comma) |,|
//@[77:0082) |       ├─TupleTypeItemSyntax
//@[77:0082) |       | └─StringSyntax
//@[77:0082) |       |   └─Token(StringComplete) |'pop'|
//@[82:0083) |       └─Token(RightSquare) |]|
//@[83:0085) ├─Token(NewLine) |\n\n|

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

type tuple = [
//@[00:0134) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |tuple|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0134) | └─TupleTypeSyntax
//@[13:0014) |   ├─Token(LeftSquare) |[|
//@[14:0015) |   ├─Token(NewLine) |\n|
    @description('A leading string')
//@[04:0048) |   ├─TupleTypeItemSyntax
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
//@[36:0038) |   | ├─Token(NewLine) |\r\n|
    string
//@[04:0010) |   | └─VariableAccessSyntax
//@[04:0010) |   |   └─IdentifierSyntax
//@[04:0010) |   |     └─Token(Identifier) |string|
//@[10:0014) |   ├─Token(NewLine) |\r\n\r\n|

    @description('A second element using a type alias')
//@[04:0064) |   ├─TupleTypeItemSyntax
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
//@[55:0057) |   | ├─Token(NewLine) |\r\n|
    bar
//@[04:0007) |   | └─VariableAccessSyntax
//@[04:0007) |   |   └─IdentifierSyntax
//@[04:0007) |   |     └─Token(Identifier) |bar|
//@[07:0009) |   ├─Token(NewLine) |\r\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0001) └─Token(EndOfFile) ||
