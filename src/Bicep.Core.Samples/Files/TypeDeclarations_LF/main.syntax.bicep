@description('The foo type')
//@[000:4814) ProgramSyntax
//@[000:0299) ├─TypeDeclarationSyntax
//@[000:0028) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0028) | | └─FunctionCallSyntax
//@[001:0012) | |   ├─IdentifierSyntax
//@[001:0012) | |   | └─Token(Identifier) |description|
//@[012:0013) | |   ├─Token(LeftParen) |(|
//@[013:0027) | |   ├─FunctionArgumentSyntax
//@[013:0027) | |   | └─StringSyntax
//@[013:0027) | |   |   └─Token(StringComplete) |'The foo type'|
//@[027:0028) | |   └─Token(RightParen) |)|
//@[028:0029) | ├─Token(NewLine) |\n|
@sealed()
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type foo = {
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0008) | ├─IdentifierSyntax
//@[005:0008) | | └─Token(Identifier) |foo|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0260) | └─ObjectTypeSyntax
//@[011:0012) |   ├─Token(LeftBrace) |{|
//@[012:0013) |   ├─Token(NewLine) |\n|
  @minLength(3)
//@[002:0089) |   ├─ObjectTypePropertySyntax
//@[002:0015) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0015) |   | | └─FunctionCallSyntax
//@[003:0012) |   | |   ├─IdentifierSyntax
//@[003:0012) |   | |   | └─Token(Identifier) |minLength|
//@[012:0013) |   | |   ├─Token(LeftParen) |(|
//@[013:0014) |   | |   ├─FunctionArgumentSyntax
//@[013:0014) |   | |   | └─IntegerLiteralSyntax
//@[013:0014) |   | |   |   └─Token(Integer) |3|
//@[014:0015) |   | |   └─Token(RightParen) |)|
//@[015:0016) |   | ├─Token(NewLine) |\n|
  @maxLength(10)
//@[002:0016) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0016) |   | | └─FunctionCallSyntax
//@[003:0012) |   | |   ├─IdentifierSyntax
//@[003:0012) |   | |   | └─Token(Identifier) |maxLength|
//@[012:0013) |   | |   ├─Token(LeftParen) |(|
//@[013:0015) |   | |   ├─FunctionArgumentSyntax
//@[013:0015) |   | |   | └─IntegerLiteralSyntax
//@[013:0015) |   | |   |   └─Token(Integer) |10|
//@[015:0016) |   | |   └─Token(RightParen) |)|
//@[016:0017) |   | ├─Token(NewLine) |\n|
  @description('A string property')
//@[002:0035) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0035) |   | | └─FunctionCallSyntax
//@[003:0014) |   | |   ├─IdentifierSyntax
//@[003:0014) |   | |   | └─Token(Identifier) |description|
//@[014:0015) |   | |   ├─Token(LeftParen) |(|
//@[015:0034) |   | |   ├─FunctionArgumentSyntax
//@[015:0034) |   | |   | └─StringSyntax
//@[015:0034) |   | |   |   └─Token(StringComplete) |'A string property'|
//@[034:0035) |   | |   └─Token(RightParen) |)|
//@[035:0036) |   | ├─Token(NewLine) |\n|
  stringProp: string
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |stringProp|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0020) |   | └─VariableAccessSyntax
//@[014:0020) |   |   └─IdentifierSyntax
//@[014:0020) |   |     └─Token(Identifier) |string|
//@[020:0022) |   ├─Token(NewLine) |\n\n|

  objectProp: {
//@[002:0089) |   ├─ObjectTypePropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |objectProp|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0089) |   | └─ObjectTypeSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0016) |   |   ├─Token(NewLine) |\n|
    @minValue(1)
//@[004:0033) |   |   ├─ObjectTypePropertySyntax
//@[004:0016) |   |   | ├─DecoratorSyntax
//@[004:0005) |   |   | | ├─Token(At) |@|
//@[005:0016) |   |   | | └─FunctionCallSyntax
//@[005:0013) |   |   | |   ├─IdentifierSyntax
//@[005:0013) |   |   | |   | └─Token(Identifier) |minValue|
//@[013:0014) |   |   | |   ├─Token(LeftParen) |(|
//@[014:0015) |   |   | |   ├─FunctionArgumentSyntax
//@[014:0015) |   |   | |   | └─IntegerLiteralSyntax
//@[014:0015) |   |   | |   |   └─Token(Integer) |1|
//@[015:0016) |   |   | |   └─Token(RightParen) |)|
//@[016:0017) |   |   | ├─Token(NewLine) |\n|
    intProp: int
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |intProp|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0016) |   |   | └─VariableAccessSyntax
//@[013:0016) |   |   |   └─IdentifierSyntax
//@[013:0016) |   |   |     └─Token(Identifier) |int|
//@[016:0018) |   |   ├─Token(NewLine) |\n\n|

    intArrayArrayProp: int [] [] ?
//@[004:0034) |   |   ├─ObjectTypePropertySyntax
//@[004:0021) |   |   | ├─IdentifierSyntax
//@[004:0021) |   |   | | └─Token(Identifier) |intArrayArrayProp|
//@[021:0022) |   |   | ├─Token(Colon) |:|
//@[023:0034) |   |   | └─NullableTypeSyntax
//@[023:0032) |   |   |   ├─ArrayTypeSyntax
//@[023:0029) |   |   |   | ├─ArrayTypeMemberSyntax
//@[023:0029) |   |   |   | | └─ArrayTypeSyntax
//@[023:0026) |   |   |   | |   ├─ArrayTypeMemberSyntax
//@[023:0026) |   |   |   | |   | └─VariableAccessSyntax
//@[023:0026) |   |   |   | |   |   └─IdentifierSyntax
//@[023:0026) |   |   |   | |   |     └─Token(Identifier) |int|
//@[027:0028) |   |   |   | |   ├─Token(LeftSquare) |[|
//@[028:0029) |   |   |   | |   └─Token(RightSquare) |]|
//@[030:0031) |   |   |   | ├─Token(LeftSquare) |[|
//@[031:0032) |   |   |   | └─Token(RightSquare) |]|
//@[033:0034) |   |   |   └─Token(Question) |?|
//@[034:0035) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\n\n|

  typeRefProp: bar
//@[002:0018) |   ├─ObjectTypePropertySyntax
//@[002:0013) |   | ├─IdentifierSyntax
//@[002:0013) |   | | └─Token(Identifier) |typeRefProp|
//@[013:0014) |   | ├─Token(Colon) |:|
//@[015:0018) |   | └─VariableAccessSyntax
//@[015:0018) |   |   └─IdentifierSyntax
//@[015:0018) |   |     └─Token(Identifier) |bar|
//@[018:0020) |   ├─Token(NewLine) |\n\n|

  literalProp: 'literal'
//@[002:0024) |   ├─ObjectTypePropertySyntax
//@[002:0013) |   | ├─IdentifierSyntax
//@[002:0013) |   | | └─Token(Identifier) |literalProp|
//@[013:0014) |   | ├─Token(Colon) |:|
//@[015:0024) |   | └─StringSyntax
//@[015:0024) |   |   └─Token(StringComplete) |'literal'|
//@[024:0026) |   ├─Token(NewLine) |\n\n|

  recursion: foo?
//@[002:0017) |   ├─ObjectTypePropertySyntax
//@[002:0011) |   | ├─IdentifierSyntax
//@[002:0011) |   | | └─Token(Identifier) |recursion|
//@[011:0012) |   | ├─Token(Colon) |:|
//@[013:0017) |   | └─NullableTypeSyntax
//@[013:0016) |   |   ├─VariableAccessSyntax
//@[013:0016) |   |   | └─IdentifierSyntax
//@[013:0016) |   |   |   └─Token(Identifier) |foo|
//@[016:0017) |   |   └─Token(Question) |?|
//@[017:0018) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@minLength(3)
//@[000:0163) ├─TypeDeclarationSyntax
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |3|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
@description('An array of array of arrays of arrays of ints')
//@[000:0061) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0061) | | └─FunctionCallSyntax
//@[001:0012) | |   ├─IdentifierSyntax
//@[001:0012) | |   | └─Token(Identifier) |description|
//@[012:0013) | |   ├─Token(LeftParen) |(|
//@[013:0060) | |   ├─FunctionArgumentSyntax
//@[013:0060) | |   | └─StringSyntax
//@[013:0060) | |   |   └─Token(StringComplete) |'An array of array of arrays of arrays of ints'|
//@[060:0061) | |   └─Token(RightParen) |)|
//@[061:0062) | ├─Token(NewLine) |\n|
@metadata({
//@[000:0064) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0064) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0063) | |   ├─FunctionArgumentSyntax
//@[010:0063) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  examples: [
//@[002:0049) | |   |   ├─ObjectPropertySyntax
//@[002:0010) | |   |   | ├─IdentifierSyntax
//@[002:0010) | |   |   | | └─Token(Identifier) |examples|
//@[010:0011) | |   |   | ├─Token(Colon) |:|
//@[012:0049) | |   |   | └─ArraySyntax
//@[012:0013) | |   |   |   ├─Token(LeftSquare) |[|
//@[013:0014) | |   |   |   ├─Token(NewLine) |\n|
    [[[[1]]], [[[2]]], [[[3]]]]
//@[004:0031) | |   |   |   ├─ArrayItemSyntax
//@[004:0031) | |   |   |   | └─ArraySyntax
//@[004:0005) | |   |   |   |   ├─Token(LeftSquare) |[|
//@[005:0012) | |   |   |   |   ├─ArrayItemSyntax
//@[005:0012) | |   |   |   |   | └─ArraySyntax
//@[005:0006) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[006:0011) | |   |   |   |   |   ├─ArrayItemSyntax
//@[006:0011) | |   |   |   |   |   | └─ArraySyntax
//@[006:0007) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[007:0010) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[007:0010) | |   |   |   |   |   |   | └─ArraySyntax
//@[007:0008) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[008:0009) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[008:0009) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[008:0009) | |   |   |   |   |   |   |   |   └─Token(Integer) |1|
//@[009:0010) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[010:0011) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[011:0012) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[012:0013) | |   |   |   |   ├─Token(Comma) |,|
//@[014:0021) | |   |   |   |   ├─ArrayItemSyntax
//@[014:0021) | |   |   |   |   | └─ArraySyntax
//@[014:0015) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[015:0020) | |   |   |   |   |   ├─ArrayItemSyntax
//@[015:0020) | |   |   |   |   |   | └─ArraySyntax
//@[015:0016) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[016:0019) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[016:0019) | |   |   |   |   |   |   | └─ArraySyntax
//@[016:0017) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[017:0018) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[017:0018) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[017:0018) | |   |   |   |   |   |   |   |   └─Token(Integer) |2|
//@[018:0019) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[019:0020) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[020:0021) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[021:0022) | |   |   |   |   ├─Token(Comma) |,|
//@[023:0030) | |   |   |   |   ├─ArrayItemSyntax
//@[023:0030) | |   |   |   |   | └─ArraySyntax
//@[023:0024) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[024:0029) | |   |   |   |   |   ├─ArrayItemSyntax
//@[024:0029) | |   |   |   |   |   | └─ArraySyntax
//@[024:0025) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[025:0028) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[025:0028) | |   |   |   |   |   |   | └─ArraySyntax
//@[025:0026) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[026:0027) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[026:0027) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[026:0027) | |   |   |   |   |   |   |   |   └─Token(Integer) |3|
//@[027:0028) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[028:0029) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[029:0030) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[030:0031) | |   |   |   |   └─Token(RightSquare) |]|
//@[031:0032) | |   |   |   ├─Token(NewLine) |\n|
  ]
//@[002:0003) | |   |   |   └─Token(RightSquare) |]|
//@[003:0004) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
type bar = int[][][][]
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0008) | ├─IdentifierSyntax
//@[005:0008) | | └─Token(Identifier) |bar|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0022) | └─ArrayTypeSyntax
//@[011:0020) |   ├─ArrayTypeMemberSyntax
//@[011:0020) |   | └─ArrayTypeSyntax
//@[011:0018) |   |   ├─ArrayTypeMemberSyntax
//@[011:0018) |   |   | └─ArrayTypeSyntax
//@[011:0016) |   |   |   ├─ArrayTypeMemberSyntax
//@[011:0016) |   |   |   | └─ArrayTypeSyntax
//@[011:0014) |   |   |   |   ├─ArrayTypeMemberSyntax
//@[011:0014) |   |   |   |   | └─VariableAccessSyntax
//@[011:0014) |   |   |   |   |   └─IdentifierSyntax
//@[011:0014) |   |   |   |   |     └─Token(Identifier) |int|
//@[014:0015) |   |   |   |   ├─Token(LeftSquare) |[|
//@[015:0016) |   |   |   |   └─Token(RightSquare) |]|
//@[016:0017) |   |   |   ├─Token(LeftSquare) |[|
//@[017:0018) |   |   |   └─Token(RightSquare) |]|
//@[018:0019) |   |   ├─Token(LeftSquare) |[|
//@[019:0020) |   |   └─Token(RightSquare) |]|
//@[020:0021) |   ├─Token(LeftSquare) |[|
//@[021:0022) |   └─Token(RightSquare) |]|
//@[022:0024) ├─Token(NewLine) |\n\n|

type aUnion = 'snap'|'crackle'|'pop'
//@[000:0036) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0011) | ├─IdentifierSyntax
//@[005:0011) | | └─Token(Identifier) |aUnion|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0036) | └─UnionTypeSyntax
//@[014:0020) |   ├─UnionTypeMemberSyntax
//@[014:0020) |   | └─StringSyntax
//@[014:0020) |   |   └─Token(StringComplete) |'snap'|
//@[020:0021) |   ├─Token(Pipe) |||
//@[021:0030) |   ├─UnionTypeMemberSyntax
//@[021:0030) |   | └─StringSyntax
//@[021:0030) |   |   └─Token(StringComplete) |'crackle'|
//@[030:0031) |   ├─Token(Pipe) |||
//@[031:0036) |   └─UnionTypeMemberSyntax
//@[031:0036) |     └─StringSyntax
//@[031:0036) |       └─Token(StringComplete) |'pop'|
//@[036:0038) ├─Token(NewLine) |\n\n|

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[000:0047) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0018) | ├─IdentifierSyntax
//@[005:0018) | | └─Token(Identifier) |expandedUnion|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0047) | └─UnionTypeSyntax
//@[021:0027) |   ├─UnionTypeMemberSyntax
//@[021:0027) |   | └─VariableAccessSyntax
//@[021:0027) |   |   └─IdentifierSyntax
//@[021:0027) |   |     └─Token(Identifier) |aUnion|
//@[027:0028) |   ├─Token(Pipe) |||
//@[028:0034) |   ├─UnionTypeMemberSyntax
//@[028:0034) |   | └─StringSyntax
//@[028:0034) |   |   └─Token(StringComplete) |'fizz'|
//@[034:0035) |   ├─Token(Pipe) |||
//@[035:0041) |   ├─UnionTypeMemberSyntax
//@[035:0041) |   | └─StringSyntax
//@[035:0041) |   |   └─Token(StringComplete) |'buzz'|
//@[041:0042) |   ├─Token(Pipe) |||
//@[042:0047) |   └─UnionTypeMemberSyntax
//@[042:0047) |     └─StringSyntax
//@[042:0047) |       └─Token(StringComplete) |'pop'|
//@[047:0049) ├─Token(NewLine) |\n\n|

type tupleUnion = ['foo', 'bar', 'baz']
//@[000:0085) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0015) | ├─IdentifierSyntax
//@[005:0015) | | └─Token(Identifier) |tupleUnion|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0085) | └─UnionTypeSyntax
//@[018:0039) |   ├─UnionTypeMemberSyntax
//@[018:0039) |   | └─TupleTypeSyntax
//@[018:0019) |   |   ├─Token(LeftSquare) |[|
//@[019:0024) |   |   ├─TupleTypeItemSyntax
//@[019:0024) |   |   | └─StringSyntax
//@[019:0024) |   |   |   └─Token(StringComplete) |'foo'|
//@[024:0025) |   |   ├─Token(Comma) |,|
//@[026:0031) |   |   ├─TupleTypeItemSyntax
//@[026:0031) |   |   | └─StringSyntax
//@[026:0031) |   |   |   └─Token(StringComplete) |'bar'|
//@[031:0032) |   |   ├─Token(Comma) |,|
//@[033:0038) |   |   ├─TupleTypeItemSyntax
//@[033:0038) |   |   | └─StringSyntax
//@[033:0038) |   |   |   └─Token(StringComplete) |'baz'|
//@[038:0039) |   |   └─Token(RightSquare) |]|
//@[039:0040) |   ├─Token(NewLine) |\n|
|['fizz', 'buzz']
//@[000:0001) |   ├─Token(Pipe) |||
//@[001:0017) |   ├─UnionTypeMemberSyntax
//@[001:0017) |   | └─TupleTypeSyntax
//@[001:0002) |   |   ├─Token(LeftSquare) |[|
//@[002:0008) |   |   ├─TupleTypeItemSyntax
//@[002:0008) |   |   | └─StringSyntax
//@[002:0008) |   |   |   └─Token(StringComplete) |'fizz'|
//@[008:0009) |   |   ├─Token(Comma) |,|
//@[010:0016) |   |   ├─TupleTypeItemSyntax
//@[010:0016) |   |   | └─StringSyntax
//@[010:0016) |   |   |   └─Token(StringComplete) |'buzz'|
//@[016:0017) |   |   └─Token(RightSquare) |]|
//@[017:0018) |   ├─Token(NewLine) |\n|
|['snap', 'crackle', 'pop']
//@[000:0001) |   ├─Token(Pipe) |||
//@[001:0027) |   └─UnionTypeMemberSyntax
//@[001:0027) |     └─TupleTypeSyntax
//@[001:0002) |       ├─Token(LeftSquare) |[|
//@[002:0008) |       ├─TupleTypeItemSyntax
//@[002:0008) |       | └─StringSyntax
//@[002:0008) |       |   └─Token(StringComplete) |'snap'|
//@[008:0009) |       ├─Token(Comma) |,|
//@[010:0019) |       ├─TupleTypeItemSyntax
//@[010:0019) |       | └─StringSyntax
//@[010:0019) |       |   └─Token(StringComplete) |'crackle'|
//@[019:0020) |       ├─Token(Comma) |,|
//@[021:0026) |       ├─TupleTypeItemSyntax
//@[021:0026) |       | └─StringSyntax
//@[021:0026) |       |   └─Token(StringComplete) |'pop'|
//@[026:0027) |       └─Token(RightSquare) |]|
//@[027:0029) ├─Token(NewLine) |\n\n|

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[000:0090) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0015) | ├─IdentifierSyntax
//@[005:0015) | | └─Token(Identifier) |mixedArray|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0090) | └─ArrayTypeSyntax
//@[018:0088) |   ├─ArrayTypeMemberSyntax
//@[018:0088) |   | └─ParenthesizedExpressionSyntax
//@[018:0019) |   |   ├─Token(LeftParen) |(|
//@[019:0087) |   |   ├─UnionTypeSyntax
//@[019:0030) |   |   | ├─UnionTypeMemberSyntax
//@[019:0030) |   |   | | └─StringSyntax
//@[019:0030) |   |   | |   └─Token(StringComplete) |'heffalump'|
//@[030:0031) |   |   | ├─Token(Pipe) |||
//@[031:0039) |   |   | ├─UnionTypeMemberSyntax
//@[031:0039) |   |   | | └─StringSyntax
//@[031:0039) |   |   | |   └─Token(StringComplete) |'woozle'|
//@[039:0040) |   |   | ├─Token(Pipe) |||
//@[040:0064) |   |   | ├─UnionTypeMemberSyntax
//@[040:0064) |   |   | | └─ObjectTypeSyntax
//@[040:0041) |   |   | |   ├─Token(LeftBrace) |{|
//@[042:0052) |   |   | |   ├─ObjectTypePropertySyntax
//@[042:0047) |   |   | |   | ├─IdentifierSyntax
//@[042:0047) |   |   | |   | | └─Token(Identifier) |shape|
//@[047:0048) |   |   | |   | ├─Token(Colon) |:|
//@[049:0052) |   |   | |   | └─StringSyntax
//@[049:0052) |   |   | |   |   └─Token(StringComplete) |'*'|
//@[052:0053) |   |   | |   ├─Token(Comma) |,|
//@[054:0063) |   |   | |   ├─ObjectTypePropertySyntax
//@[054:0058) |   |   | |   | ├─IdentifierSyntax
//@[054:0058) |   |   | |   | | └─Token(Identifier) |size|
//@[058:0059) |   |   | |   | ├─Token(Colon) |:|
//@[060:0063) |   |   | |   | └─StringSyntax
//@[060:0063) |   |   | |   |   └─Token(StringComplete) |'*'|
//@[063:0064) |   |   | |   └─Token(RightBrace) |}|
//@[064:0065) |   |   | ├─Token(Pipe) |||
//@[065:0067) |   |   | ├─UnionTypeMemberSyntax
//@[065:0067) |   |   | | └─IntegerLiteralSyntax
//@[065:0067) |   |   | |   └─Token(Integer) |10|
//@[067:0068) |   |   | ├─Token(Pipe) |||
//@[068:0071) |   |   | ├─UnionTypeMemberSyntax
//@[068:0071) |   |   | | └─UnaryOperationSyntax
//@[068:0069) |   |   | |   ├─Token(Minus) |-|
//@[069:0071) |   |   | |   └─IntegerLiteralSyntax
//@[069:0071) |   |   | |     └─Token(Integer) |10|
//@[071:0072) |   |   | ├─Token(Pipe) |||
//@[072:0076) |   |   | ├─UnionTypeMemberSyntax
//@[072:0076) |   |   | | └─BooleanLiteralSyntax
//@[072:0076) |   |   | |   └─Token(TrueKeyword) |true|
//@[076:0077) |   |   | ├─Token(Pipe) |||
//@[077:0082) |   |   | ├─UnionTypeMemberSyntax
//@[077:0082) |   |   | | └─UnaryOperationSyntax
//@[077:0078) |   |   | |   ├─Token(Exclamation) |!|
//@[078:0082) |   |   | |   └─BooleanLiteralSyntax
//@[078:0082) |   |   | |     └─Token(TrueKeyword) |true|
//@[082:0083) |   |   | ├─Token(Pipe) |||
//@[083:0087) |   |   | └─UnionTypeMemberSyntax
//@[083:0087) |   |   |   └─NullLiteralSyntax
//@[083:0087) |   |   |     └─Token(NullKeyword) |null|
//@[087:0088) |   |   └─Token(RightParen) |)|
//@[088:0089) |   ├─Token(LeftSquare) |[|
//@[089:0090) |   └─Token(RightSquare) |]|
//@[090:0092) ├─Token(NewLine) |\n\n|

type bool = string
//@[000:0018) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0009) | ├─IdentifierSyntax
//@[005:0009) | | └─Token(Identifier) |bool|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0018) | └─VariableAccessSyntax
//@[012:0018) |   └─IdentifierSyntax
//@[012:0018) |     └─Token(Identifier) |string|
//@[018:0020) ├─Token(NewLine) |\n\n|

param inlineObjectParam {
//@[000:0127) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |inlineObjectParam|
//@[024:0084) | ├─ObjectTypeSyntax
//@[024:0025) | | ├─Token(LeftBrace) |{|
//@[025:0026) | | ├─Token(NewLine) |\n|
  foo: string
//@[002:0013) | | ├─ObjectTypePropertySyntax
//@[002:0005) | | | ├─IdentifierSyntax
//@[002:0005) | | | | └─Token(Identifier) |foo|
//@[005:0006) | | | ├─Token(Colon) |:|
//@[007:0013) | | | └─VariableAccessSyntax
//@[007:0013) | | |   └─IdentifierSyntax
//@[007:0013) | | |     └─Token(Identifier) |string|
//@[013:0014) | | ├─Token(NewLine) |\n|
  bar: 100|200|300|400|500
//@[002:0026) | | ├─ObjectTypePropertySyntax
//@[002:0005) | | | ├─IdentifierSyntax
//@[002:0005) | | | | └─Token(Identifier) |bar|
//@[005:0006) | | | ├─Token(Colon) |:|
//@[007:0026) | | | └─UnionTypeSyntax
//@[007:0010) | | |   ├─UnionTypeMemberSyntax
//@[007:0010) | | |   | └─IntegerLiteralSyntax
//@[007:0010) | | |   |   └─Token(Integer) |100|
//@[010:0011) | | |   ├─Token(Pipe) |||
//@[011:0014) | | |   ├─UnionTypeMemberSyntax
//@[011:0014) | | |   | └─IntegerLiteralSyntax
//@[011:0014) | | |   |   └─Token(Integer) |200|
//@[014:0015) | | |   ├─Token(Pipe) |||
//@[015:0018) | | |   ├─UnionTypeMemberSyntax
//@[015:0018) | | |   | └─IntegerLiteralSyntax
//@[015:0018) | | |   |   └─Token(Integer) |300|
//@[018:0019) | | |   ├─Token(Pipe) |||
//@[019:0022) | | |   ├─UnionTypeMemberSyntax
//@[019:0022) | | |   | └─IntegerLiteralSyntax
//@[019:0022) | | |   |   └─Token(Integer) |400|
//@[022:0023) | | |   ├─Token(Pipe) |||
//@[023:0026) | | |   └─UnionTypeMemberSyntax
//@[023:0026) | | |     └─IntegerLiteralSyntax
//@[023:0026) | | |       └─Token(Integer) |500|
//@[026:0027) | | ├─Token(NewLine) |\n|
  baz: sys.bool
//@[002:0015) | | ├─ObjectTypePropertySyntax
//@[002:0005) | | | ├─IdentifierSyntax
//@[002:0005) | | | | └─Token(Identifier) |baz|
//@[005:0006) | | | ├─Token(Colon) |:|
//@[007:0015) | | | └─PropertyAccessSyntax
//@[007:0010) | | |   ├─VariableAccessSyntax
//@[007:0010) | | |   | └─IdentifierSyntax
//@[007:0010) | | |   |   └─Token(Identifier) |sys|
//@[010:0011) | | |   ├─Token(Dot) |.|
//@[011:0015) | | |   └─IdentifierSyntax
//@[011:0015) | | |     └─Token(Identifier) |bool|
//@[015:0016) | | ├─Token(NewLine) |\n|
} = {
//@[000:0001) | | └─Token(RightBrace) |}|
//@[002:0044) | └─ParameterDefaultValueSyntax
//@[002:0003) |   ├─Token(Assignment) |=|
//@[004:0044) |   └─ObjectSyntax
//@[004:0005) |     ├─Token(LeftBrace) |{|
//@[005:0006) |     ├─Token(NewLine) |\n|
  foo: 'foo'
//@[002:0012) |     ├─ObjectPropertySyntax
//@[002:0005) |     | ├─IdentifierSyntax
//@[002:0005) |     | | └─Token(Identifier) |foo|
//@[005:0006) |     | ├─Token(Colon) |:|
//@[007:0012) |     | └─StringSyntax
//@[007:0012) |     |   └─Token(StringComplete) |'foo'|
//@[012:0013) |     ├─Token(NewLine) |\n|
  bar: 300
//@[002:0010) |     ├─ObjectPropertySyntax
//@[002:0005) |     | ├─IdentifierSyntax
//@[002:0005) |     | | └─Token(Identifier) |bar|
//@[005:0006) |     | ├─Token(Colon) |:|
//@[007:0010) |     | └─IntegerLiteralSyntax
//@[007:0010) |     |   └─Token(Integer) |300|
//@[010:0011) |     ├─Token(NewLine) |\n|
  baz: false
//@[002:0012) |     ├─ObjectPropertySyntax
//@[002:0005) |     | ├─IdentifierSyntax
//@[002:0005) |     | | └─Token(Identifier) |baz|
//@[005:0006) |     | ├─Token(Colon) |:|
//@[007:0012) |     | └─BooleanLiteralSyntax
//@[007:0012) |     |   └─Token(FalseKeyword) |false|
//@[012:0013) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[000:0075) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0016) | ├─IdentifierSyntax
//@[006:0016) | | └─Token(Identifier) |unionParam|
//@[017:0054) | ├─UnionTypeSyntax
//@[017:0035) | | ├─UnionTypeMemberSyntax
//@[017:0035) | | | └─ObjectTypeSyntax
//@[017:0018) | | |   ├─Token(LeftBrace) |{|
//@[018:0034) | | |   ├─ObjectTypePropertySyntax
//@[018:0026) | | |   | ├─IdentifierSyntax
//@[018:0026) | | |   | | └─Token(Identifier) |property|
//@[026:0027) | | |   | ├─Token(Colon) |:|
//@[028:0034) | | |   | └─StringSyntax
//@[028:0034) | | |   |   └─Token(StringComplete) |'ping'|
//@[034:0035) | | |   └─Token(RightBrace) |}|
//@[035:0036) | | ├─Token(Pipe) |||
//@[036:0054) | | └─UnionTypeMemberSyntax
//@[036:0054) | |   └─ObjectTypeSyntax
//@[036:0037) | |     ├─Token(LeftBrace) |{|
//@[037:0053) | |     ├─ObjectTypePropertySyntax
//@[037:0045) | |     | ├─IdentifierSyntax
//@[037:0045) | |     | | └─Token(Identifier) |property|
//@[045:0046) | |     | ├─Token(Colon) |:|
//@[047:0053) | |     | └─StringSyntax
//@[047:0053) | |     |   └─Token(StringComplete) |'pong'|
//@[053:0054) | |     └─Token(RightBrace) |}|
//@[055:0075) | └─ParameterDefaultValueSyntax
//@[055:0056) |   ├─Token(Assignment) |=|
//@[057:0075) |   └─ObjectSyntax
//@[057:0058) |     ├─Token(LeftBrace) |{|
//@[058:0074) |     ├─ObjectPropertySyntax
//@[058:0066) |     | ├─IdentifierSyntax
//@[058:0066) |     | | └─Token(Identifier) |property|
//@[066:0067) |     | ├─Token(Colon) |:|
//@[068:0074) |     | └─StringSyntax
//@[068:0074) |     |   └─Token(StringComplete) |'pong'|
//@[074:0075) |     └─Token(RightBrace) |}|
//@[075:0077) ├─Token(NewLine) |\n\n|

param paramUsingType mixedArray
//@[000:0031) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0020) | ├─IdentifierSyntax
//@[006:0020) | | └─Token(Identifier) |paramUsingType|
//@[021:0031) | └─VariableAccessSyntax
//@[021:0031) |   └─IdentifierSyntax
//@[021:0031) |     └─Token(Identifier) |mixedArray|
//@[031:0033) ├─Token(NewLine) |\n\n|

output outputUsingType mixedArray = paramUsingType
//@[000:0050) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0022) | ├─IdentifierSyntax
//@[007:0022) | | └─Token(Identifier) |outputUsingType|
//@[023:0033) | ├─VariableAccessSyntax
//@[023:0033) | | └─IdentifierSyntax
//@[023:0033) | |   └─Token(Identifier) |mixedArray|
//@[034:0035) | ├─Token(Assignment) |=|
//@[036:0050) | └─VariableAccessSyntax
//@[036:0050) |   └─IdentifierSyntax
//@[036:0050) |     └─Token(Identifier) |paramUsingType|
//@[050:0052) ├─Token(NewLine) |\n\n|

type tuple = [
//@[000:0129) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |tuple|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0129) | └─TupleTypeSyntax
//@[013:0014) |   ├─Token(LeftSquare) |[|
//@[014:0015) |   ├─Token(NewLine) |\n|
    @description('A leading string')
//@[004:0047) |   ├─TupleTypeItemSyntax
//@[004:0036) |   | ├─DecoratorSyntax
//@[004:0005) |   | | ├─Token(At) |@|
//@[005:0036) |   | | └─FunctionCallSyntax
//@[005:0016) |   | |   ├─IdentifierSyntax
//@[005:0016) |   | |   | └─Token(Identifier) |description|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0035) |   | |   ├─FunctionArgumentSyntax
//@[017:0035) |   | |   | └─StringSyntax
//@[017:0035) |   | |   |   └─Token(StringComplete) |'A leading string'|
//@[035:0036) |   | |   └─Token(RightParen) |)|
//@[036:0037) |   | ├─Token(NewLine) |\n|
    string
//@[004:0010) |   | └─VariableAccessSyntax
//@[004:0010) |   |   └─IdentifierSyntax
//@[004:0010) |   |     └─Token(Identifier) |string|
//@[010:0012) |   ├─Token(NewLine) |\n\n|

    @description('A second element using a type alias')
//@[004:0063) |   ├─TupleTypeItemSyntax
//@[004:0055) |   | ├─DecoratorSyntax
//@[004:0005) |   | | ├─Token(At) |@|
//@[005:0055) |   | | └─FunctionCallSyntax
//@[005:0016) |   | |   ├─IdentifierSyntax
//@[005:0016) |   | |   | └─Token(Identifier) |description|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0054) |   | |   ├─FunctionArgumentSyntax
//@[017:0054) |   | |   | └─StringSyntax
//@[017:0054) |   | |   |   └─Token(StringComplete) |'A second element using a type alias'|
//@[054:0055) |   | |   └─Token(RightParen) |)|
//@[055:0056) |   | ├─Token(NewLine) |\n|
    bar
//@[004:0007) |   | └─VariableAccessSyntax
//@[004:0007) |   |   └─IdentifierSyntax
//@[004:0007) |   |     └─Token(Identifier) |bar|
//@[007:0008) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

type stringStringDictionary = {
//@[000:0047) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0027) | ├─IdentifierSyntax
//@[005:0027) | | └─Token(Identifier) |stringStringDictionary|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0047) | └─ObjectTypeSyntax
//@[030:0031) |   ├─Token(LeftBrace) |{|
//@[031:0032) |   ├─Token(NewLine) |\n|
    *: string
//@[004:0013) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[004:0005) |   | ├─Token(Asterisk) |*|
//@[005:0006) |   | ├─Token(Colon) |:|
//@[007:0013) |   | └─VariableAccessSyntax
//@[007:0013) |   |   └─IdentifierSyntax
//@[007:0013) |   |     └─Token(Identifier) |string|
//@[013:0014) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@minValue(1)
//@[000:0052) ├─TypeDeclarationSyntax
//@[000:0012) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0012) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0011) | |   ├─FunctionArgumentSyntax
//@[010:0011) | |   | └─IntegerLiteralSyntax
//@[010:0011) | |   |   └─Token(Integer) |1|
//@[011:0012) | |   └─Token(RightParen) |)|
//@[012:0013) | ├─Token(NewLine) |\n|
@maxValue(10)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |maxValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0012) | |   ├─FunctionArgumentSyntax
//@[010:0012) | |   | └─IntegerLiteralSyntax
//@[010:0012) | |   |   └─Token(Integer) |10|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
type constrainedInt = int
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0019) | ├─IdentifierSyntax
//@[005:0019) | | └─Token(Identifier) |constrainedInt|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0025) | └─VariableAccessSyntax
//@[022:0025) |   └─IdentifierSyntax
//@[022:0025) |     └─Token(Identifier) |int|
//@[025:0027) ├─Token(NewLine) |\n\n|

param mightIncludeNull ({key: 'value'} | null)[]
//@[000:0048) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0022) | ├─IdentifierSyntax
//@[006:0022) | | └─Token(Identifier) |mightIncludeNull|
//@[023:0048) | └─ArrayTypeSyntax
//@[023:0046) |   ├─ArrayTypeMemberSyntax
//@[023:0046) |   | └─ParenthesizedExpressionSyntax
//@[023:0024) |   |   ├─Token(LeftParen) |(|
//@[024:0045) |   |   ├─UnionTypeSyntax
//@[024:0038) |   |   | ├─UnionTypeMemberSyntax
//@[024:0038) |   |   | | └─ObjectTypeSyntax
//@[024:0025) |   |   | |   ├─Token(LeftBrace) |{|
//@[025:0037) |   |   | |   ├─ObjectTypePropertySyntax
//@[025:0028) |   |   | |   | ├─IdentifierSyntax
//@[025:0028) |   |   | |   | | └─Token(Identifier) |key|
//@[028:0029) |   |   | |   | ├─Token(Colon) |:|
//@[030:0037) |   |   | |   | └─StringSyntax
//@[030:0037) |   |   | |   |   └─Token(StringComplete) |'value'|
//@[037:0038) |   |   | |   └─Token(RightBrace) |}|
//@[039:0040) |   |   | ├─Token(Pipe) |||
//@[041:0045) |   |   | └─UnionTypeMemberSyntax
//@[041:0045) |   |   |   └─NullLiteralSyntax
//@[041:0045) |   |   |     └─Token(NullKeyword) |null|
//@[045:0046) |   |   └─Token(RightParen) |)|
//@[046:0047) |   ├─Token(LeftSquare) |[|
//@[047:0048) |   └─Token(RightSquare) |]|
//@[048:0050) ├─Token(NewLine) |\n\n|

var nonNull = mightIncludeNull[0]!.key
//@[000:0038) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |nonNull|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0038) | └─PropertyAccessSyntax
//@[014:0034) |   ├─NonNullAssertionSyntax
//@[014:0033) |   | ├─ArrayAccessSyntax
//@[014:0030) |   | | ├─VariableAccessSyntax
//@[014:0030) |   | | | └─IdentifierSyntax
//@[014:0030) |   | | |   └─Token(Identifier) |mightIncludeNull|
//@[030:0031) |   | | ├─Token(LeftSquare) |[|
//@[031:0032) |   | | ├─IntegerLiteralSyntax
//@[031:0032) |   | | | └─Token(Integer) |0|
//@[032:0033) |   | | └─Token(RightSquare) |]|
//@[033:0034) |   | └─Token(Exclamation) |!|
//@[034:0035) |   ├─Token(Dot) |.|
//@[035:0038) |   └─IdentifierSyntax
//@[035:0038) |     └─Token(Identifier) |key|
//@[038:0040) ├─Token(NewLine) |\n\n|

output nonNull string = nonNull
//@[000:0031) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0014) | ├─IdentifierSyntax
//@[007:0014) | | └─Token(Identifier) |nonNull|
//@[015:0021) | ├─VariableAccessSyntax
//@[015:0021) | | └─IdentifierSyntax
//@[015:0021) | |   └─Token(Identifier) |string|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0031) | └─VariableAccessSyntax
//@[024:0031) |   └─IdentifierSyntax
//@[024:0031) |     └─Token(Identifier) |nonNull|
//@[031:0033) ├─Token(NewLine) |\n\n|

var maybeNull = mightIncludeNull[0].?key
//@[000:0040) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |maybeNull|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0040) | └─PropertyAccessSyntax
//@[016:0035) |   ├─ArrayAccessSyntax
//@[016:0032) |   | ├─VariableAccessSyntax
//@[016:0032) |   | | └─IdentifierSyntax
//@[016:0032) |   | |   └─Token(Identifier) |mightIncludeNull|
//@[032:0033) |   | ├─Token(LeftSquare) |[|
//@[033:0034) |   | ├─IntegerLiteralSyntax
//@[033:0034) |   | | └─Token(Integer) |0|
//@[034:0035) |   | └─Token(RightSquare) |]|
//@[035:0036) |   ├─Token(Dot) |.|
//@[036:0037) |   ├─Token(Question) |?|
//@[037:0040) |   └─IdentifierSyntax
//@[037:0040) |     └─Token(Identifier) |key|
//@[040:0042) ├─Token(NewLine) |\n\n|

var maybeNull2 = mightIncludeNull[0][?'key']
//@[000:0044) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0014) | ├─IdentifierSyntax
//@[004:0014) | | └─Token(Identifier) |maybeNull2|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0044) | └─ArrayAccessSyntax
//@[017:0036) |   ├─ArrayAccessSyntax
//@[017:0033) |   | ├─VariableAccessSyntax
//@[017:0033) |   | | └─IdentifierSyntax
//@[017:0033) |   | |   └─Token(Identifier) |mightIncludeNull|
//@[033:0034) |   | ├─Token(LeftSquare) |[|
//@[034:0035) |   | ├─IntegerLiteralSyntax
//@[034:0035) |   | | └─Token(Integer) |0|
//@[035:0036) |   | └─Token(RightSquare) |]|
//@[036:0037) |   ├─Token(LeftSquare) |[|
//@[037:0038) |   ├─Token(Question) |?|
//@[038:0043) |   ├─StringSyntax
//@[038:0043) |   | └─Token(StringComplete) |'key'|
//@[043:0044) |   └─Token(RightSquare) |]|
//@[044:0046) ├─Token(NewLine) |\n\n|

output maybeNull string? = maybeNull
//@[000:0036) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0016) | ├─IdentifierSyntax
//@[007:0016) | | └─Token(Identifier) |maybeNull|
//@[017:0024) | ├─NullableTypeSyntax
//@[017:0023) | | ├─VariableAccessSyntax
//@[017:0023) | | | └─IdentifierSyntax
//@[017:0023) | | |   └─Token(Identifier) |string|
//@[023:0024) | | └─Token(Question) |?|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0036) | └─VariableAccessSyntax
//@[027:0036) |   └─IdentifierSyntax
//@[027:0036) |     └─Token(Identifier) |maybeNull|
//@[036:0038) ├─Token(NewLine) |\n\n|

type nullable = string?
//@[000:0023) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |nullable|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0023) | └─NullableTypeSyntax
//@[016:0022) |   ├─VariableAccessSyntax
//@[016:0022) |   | └─IdentifierSyntax
//@[016:0022) |   |   └─Token(Identifier) |string|
//@[022:0023) |   └─Token(Question) |?|
//@[023:0025) ├─Token(NewLine) |\n\n|

type nonNullable = nullable!
//@[000:0028) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0016) | ├─IdentifierSyntax
//@[005:0016) | | └─Token(Identifier) |nonNullable|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0028) | └─NonNullAssertionSyntax
//@[019:0027) |   ├─VariableAccessSyntax
//@[019:0027) |   | └─IdentifierSyntax
//@[019:0027) |   |   └─Token(Identifier) |nullable|
//@[027:0028) |   └─Token(Exclamation) |!|
//@[028:0030) ├─Token(NewLine) |\n\n|

type typeA = {
//@[000:0044) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeA|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0044) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  type: 'a'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'a'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  value: string
//@[002:0015) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |value|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0015) |   | └─VariableAccessSyntax
//@[009:0015) |   |   └─IdentifierSyntax
//@[009:0015) |   |     └─Token(Identifier) |string|
//@[015:0016) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type typeB = {
//@[000:0041) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeB|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0041) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  type: 'b'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'b'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  value: int
//@[002:0012) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |value|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0012) |   | └─VariableAccessSyntax
//@[009:0012) |   |   └─IdentifierSyntax
//@[009:0012) |   |     └─Token(Identifier) |int|
//@[012:0013) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type typeC = {
//@[000:0059) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeC|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0059) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  type: 'c'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'c'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  value: bool
//@[002:0013) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |value|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0013) |   | └─VariableAccessSyntax
//@[009:0013) |   |   └─IdentifierSyntax
//@[009:0013) |   |     └─Token(Identifier) |bool|
//@[013:0014) |   ├─Token(NewLine) |\n|
  value2: string
//@[002:0016) |   ├─ObjectTypePropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |value2|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0016) |   | └─VariableAccessSyntax
//@[010:0016) |   |   └─IdentifierSyntax
//@[010:0016) |   |     └─Token(Identifier) |string|
//@[016:0017) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type typeD = {
//@[000:0044) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeD|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0044) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  type: 'd'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'d'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  value: object
//@[002:0015) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |value|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0015) |   | └─VariableAccessSyntax
//@[009:0015) |   |   └─IdentifierSyntax
//@[009:0015) |   |     └─Token(Identifier) |object|
//@[015:0016) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type typeE = {
//@[000:0047) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeE|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0047) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  type: 'e'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'e'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  value: 'a' | 'b'
//@[002:0018) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |value|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0018) |   | └─UnionTypeSyntax
//@[009:0012) |   |   ├─UnionTypeMemberSyntax
//@[009:0012) |   |   | └─StringSyntax
//@[009:0012) |   |   |   └─Token(StringComplete) |'a'|
//@[013:0014) |   |   ├─Token(Pipe) |||
//@[015:0018) |   |   └─UnionTypeMemberSyntax
//@[015:0018) |   |     └─StringSyntax
//@[015:0018) |   |       └─Token(StringComplete) |'b'|
//@[018:0019) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type typeF = {
//@[000:0040) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeF|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0040) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  type: 'f'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'f'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  *: string
//@[002:0011) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[002:0003) |   | ├─Token(Asterisk) |*|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0011) |   | └─VariableAccessSyntax
//@[005:0011) |   |   └─IdentifierSyntax
//@[005:0011) |   |     └─Token(Identifier) |string|
//@[011:0012) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0063) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion1 = typeA | typeB
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |discriminatedUnion1|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0040) | └─UnionTypeSyntax
//@[027:0032) |   ├─UnionTypeMemberSyntax
//@[027:0032) |   | └─VariableAccessSyntax
//@[027:0032) |   |   └─IdentifierSyntax
//@[027:0032) |   |     └─Token(Identifier) |typeA|
//@[033:0034) |   ├─Token(Pipe) |||
//@[035:0040) |   └─UnionTypeMemberSyntax
//@[035:0040) |     └─VariableAccessSyntax
//@[035:0040) |       └─IdentifierSyntax
//@[035:0040) |         └─Token(Identifier) |typeB|
//@[040:0042) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0107) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |discriminatedUnion2|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0084) | └─UnionTypeSyntax
//@[027:0055) |   ├─UnionTypeMemberSyntax
//@[027:0055) |   | └─ObjectTypeSyntax
//@[027:0028) |   |   ├─Token(LeftBrace) |{|
//@[029:0038) |   |   ├─ObjectTypePropertySyntax
//@[029:0033) |   |   | ├─IdentifierSyntax
//@[029:0033) |   |   | | └─Token(Identifier) |type|
//@[033:0034) |   |   | ├─Token(Colon) |:|
//@[035:0038) |   |   | └─StringSyntax
//@[035:0038) |   |   |   └─Token(StringComplete) |'c'|
//@[038:0039) |   |   ├─Token(Comma) |,|
//@[040:0053) |   |   ├─ObjectTypePropertySyntax
//@[040:0045) |   |   | ├─IdentifierSyntax
//@[040:0045) |   |   | | └─Token(Identifier) |value|
//@[045:0046) |   |   | ├─Token(Colon) |:|
//@[047:0053) |   |   | └─VariableAccessSyntax
//@[047:0053) |   |   |   └─IdentifierSyntax
//@[047:0053) |   |   |     └─Token(Identifier) |string|
//@[054:0055) |   |   └─Token(RightBrace) |}|
//@[056:0057) |   ├─Token(Pipe) |||
//@[058:0084) |   └─UnionTypeMemberSyntax
//@[058:0084) |     └─ObjectTypeSyntax
//@[058:0059) |       ├─Token(LeftBrace) |{|
//@[060:0069) |       ├─ObjectTypePropertySyntax
//@[060:0064) |       | ├─IdentifierSyntax
//@[060:0064) |       | | └─Token(Identifier) |type|
//@[064:0065) |       | ├─Token(Colon) |:|
//@[066:0069) |       | └─StringSyntax
//@[066:0069) |       |   └─Token(StringComplete) |'d'|
//@[069:0070) |       ├─Token(Comma) |,|
//@[071:0082) |       ├─ObjectTypePropertySyntax
//@[071:0076) |       | ├─IdentifierSyntax
//@[071:0076) |       | | └─Token(Identifier) |value|
//@[076:0077) |       | ├─Token(Colon) |:|
//@[078:0082) |       | └─VariableAccessSyntax
//@[078:0082) |       |   └─IdentifierSyntax
//@[078:0082) |       |     └─Token(Identifier) |bool|
//@[083:0084) |       └─Token(RightBrace) |}|
//@[084:0086) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0122) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |discriminatedUnion3|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0099) | └─UnionTypeSyntax
//@[027:0046) |   ├─UnionTypeMemberSyntax
//@[027:0046) |   | └─VariableAccessSyntax
//@[027:0046) |   |   └─IdentifierSyntax
//@[027:0046) |   |     └─Token(Identifier) |discriminatedUnion1|
//@[047:0048) |   ├─Token(Pipe) |||
//@[049:0068) |   ├─UnionTypeMemberSyntax
//@[049:0068) |   | └─VariableAccessSyntax
//@[049:0068) |   |   └─IdentifierSyntax
//@[049:0068) |   |     └─Token(Identifier) |discriminatedUnion2|
//@[069:0070) |   ├─Token(Pipe) |||
//@[071:0099) |   └─UnionTypeMemberSyntax
//@[071:0099) |     └─ObjectTypeSyntax
//@[071:0072) |       ├─Token(LeftBrace) |{|
//@[073:0082) |       ├─ObjectTypePropertySyntax
//@[073:0077) |       | ├─IdentifierSyntax
//@[073:0077) |       | | └─Token(Identifier) |type|
//@[077:0078) |       | ├─Token(Colon) |:|
//@[079:0082) |       | └─StringSyntax
//@[079:0082) |       |   └─Token(StringComplete) |'e'|
//@[082:0083) |       ├─Token(Comma) |,|
//@[084:0097) |       ├─ObjectTypePropertySyntax
//@[084:0089) |       | ├─IdentifierSyntax
//@[084:0089) |       | | └─Token(Identifier) |value|
//@[089:0090) |       | ├─Token(Colon) |:|
//@[091:0097) |       | └─VariableAccessSyntax
//@[091:0097) |       |   └─IdentifierSyntax
//@[091:0097) |       |     └─Token(Identifier) |string|
//@[098:0099) |       └─Token(RightBrace) |}|
//@[099:0101) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0101) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |discriminatedUnion4|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0078) | └─UnionTypeSyntax
//@[027:0046) |   ├─UnionTypeMemberSyntax
//@[027:0046) |   | └─VariableAccessSyntax
//@[027:0046) |   |   └─IdentifierSyntax
//@[027:0046) |   |     └─Token(Identifier) |discriminatedUnion1|
//@[047:0048) |   ├─Token(Pipe) |||
//@[049:0078) |   └─UnionTypeMemberSyntax
//@[049:0078) |     └─ParenthesizedExpressionSyntax
//@[049:0050) |       ├─Token(LeftParen) |(|
//@[050:0077) |       ├─UnionTypeSyntax
//@[050:0069) |       | ├─UnionTypeMemberSyntax
//@[050:0069) |       | | └─VariableAccessSyntax
//@[050:0069) |       | |   └─IdentifierSyntax
//@[050:0069) |       | |     └─Token(Identifier) |discriminatedUnion2|
//@[070:0071) |       | ├─Token(Pipe) |||
//@[072:0077) |       | └─UnionTypeMemberSyntax
//@[072:0077) |       |   └─VariableAccessSyntax
//@[072:0077) |       |     └─IdentifierSyntax
//@[072:0077) |       |       └─Token(Identifier) |typeE|
//@[077:0078) |       └─Token(RightParen) |)|
//@[078:0080) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0066) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion5 = (typeA | typeB)?
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |discriminatedUnion5|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0043) | └─NullableTypeSyntax
//@[027:0042) |   ├─ParenthesizedExpressionSyntax
//@[027:0028) |   | ├─Token(LeftParen) |(|
//@[028:0041) |   | ├─UnionTypeSyntax
//@[028:0033) |   | | ├─UnionTypeMemberSyntax
//@[028:0033) |   | | | └─VariableAccessSyntax
//@[028:0033) |   | | |   └─IdentifierSyntax
//@[028:0033) |   | | |     └─Token(Identifier) |typeA|
//@[034:0035) |   | | ├─Token(Pipe) |||
//@[036:0041) |   | | └─UnionTypeMemberSyntax
//@[036:0041) |   | |   └─VariableAccessSyntax
//@[036:0041) |   | |     └─IdentifierSyntax
//@[036:0041) |   | |       └─Token(Identifier) |typeB|
//@[041:0042) |   | └─Token(RightParen) |)|
//@[042:0043) |   └─Token(Question) |?|
//@[043:0045) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0066) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatedUnion6 = (typeA | typeB)!
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |discriminatedUnion6|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0043) | └─NonNullAssertionSyntax
//@[027:0042) |   ├─ParenthesizedExpressionSyntax
//@[027:0028) |   | ├─Token(LeftParen) |(|
//@[028:0041) |   | ├─UnionTypeSyntax
//@[028:0033) |   | | ├─UnionTypeMemberSyntax
//@[028:0033) |   | | | └─VariableAccessSyntax
//@[028:0033) |   | | |   └─IdentifierSyntax
//@[028:0033) |   | | |     └─Token(Identifier) |typeA|
//@[034:0035) |   | | ├─Token(Pipe) |||
//@[036:0041) |   | | └─UnionTypeMemberSyntax
//@[036:0041) |   | |   └─VariableAccessSyntax
//@[036:0041) |   | |     └─IdentifierSyntax
//@[036:0041) |   | |       └─Token(Identifier) |typeB|
//@[041:0042) |   | └─Token(RightParen) |)|
//@[042:0043) |   └─Token(Exclamation) |!|
//@[043:0045) ├─Token(NewLine) |\n\n|

type inlineDiscriminatedUnion1 = {
//@[000:0083) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0030) | ├─IdentifierSyntax
//@[005:0030) | | └─Token(Identifier) |inlineDiscriminatedUnion1|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0083) | └─ObjectTypeSyntax
//@[033:0034) |   ├─Token(LeftBrace) |{|
//@[034:0035) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0046) |   ├─ObjectTypePropertySyntax
//@[002:0024) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0024) |   | | └─FunctionCallSyntax
//@[003:0016) |   | |   ├─IdentifierSyntax
//@[003:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0023) |   | |   ├─FunctionArgumentSyntax
//@[017:0023) |   | |   | └─StringSyntax
//@[017:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |   | |   └─Token(RightParen) |)|
//@[024:0025) |   | ├─Token(NewLine) |\n|
  prop: typeA | typeC
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |prop|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0021) |   | └─UnionTypeSyntax
//@[008:0013) |   |   ├─UnionTypeMemberSyntax
//@[008:0013) |   |   | └─VariableAccessSyntax
//@[008:0013) |   |   |   └─IdentifierSyntax
//@[008:0013) |   |   |     └─Token(Identifier) |typeA|
//@[014:0015) |   |   ├─Token(Pipe) |||
//@[016:0021) |   |   └─UnionTypeMemberSyntax
//@[016:0021) |   |     └─VariableAccessSyntax
//@[016:0021) |   |       └─IdentifierSyntax
//@[016:0021) |   |         └─Token(Identifier) |typeC|
//@[021:0022) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type inlineDiscriminatedUnion2 = {
//@[000:0104) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0030) | ├─IdentifierSyntax
//@[005:0030) | | └─Token(Identifier) |inlineDiscriminatedUnion2|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0104) | └─ObjectTypeSyntax
//@[033:0034) |   ├─Token(LeftBrace) |{|
//@[034:0035) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0067) |   ├─ObjectTypePropertySyntax
//@[002:0024) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0024) |   | | └─FunctionCallSyntax
//@[003:0016) |   | |   ├─IdentifierSyntax
//@[003:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0023) |   | |   ├─FunctionArgumentSyntax
//@[017:0023) |   | |   | └─StringSyntax
//@[017:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |   | |   └─Token(RightParen) |)|
//@[024:0025) |   | ├─Token(NewLine) |\n|
  prop: { type: 'a', value: bool } | typeB
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |prop|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0042) |   | └─UnionTypeSyntax
//@[008:0034) |   |   ├─UnionTypeMemberSyntax
//@[008:0034) |   |   | └─ObjectTypeSyntax
//@[008:0009) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0019) |   |   |   ├─ObjectTypePropertySyntax
//@[010:0014) |   |   |   | ├─IdentifierSyntax
//@[010:0014) |   |   |   | | └─Token(Identifier) |type|
//@[014:0015) |   |   |   | ├─Token(Colon) |:|
//@[016:0019) |   |   |   | └─StringSyntax
//@[016:0019) |   |   |   |   └─Token(StringComplete) |'a'|
//@[019:0020) |   |   |   ├─Token(Comma) |,|
//@[021:0032) |   |   |   ├─ObjectTypePropertySyntax
//@[021:0026) |   |   |   | ├─IdentifierSyntax
//@[021:0026) |   |   |   | | └─Token(Identifier) |value|
//@[026:0027) |   |   |   | ├─Token(Colon) |:|
//@[028:0032) |   |   |   | └─VariableAccessSyntax
//@[028:0032) |   |   |   |   └─IdentifierSyntax
//@[028:0032) |   |   |   |     └─Token(Identifier) |bool|
//@[033:0034) |   |   |   └─Token(RightBrace) |}|
//@[035:0036) |   |   ├─Token(Pipe) |||
//@[037:0042) |   |   └─UnionTypeMemberSyntax
//@[037:0042) |   |     └─VariableAccessSyntax
//@[037:0042) |   |       └─IdentifierSyntax
//@[037:0042) |   |         └─Token(Identifier) |typeB|
//@[042:0043) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0232) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type inlineDiscriminatedUnion3 = {
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0030) | ├─IdentifierSyntax
//@[005:0030) | | └─Token(Identifier) |inlineDiscriminatedUnion3|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0209) | └─UnionTypeSyntax
//@[033:0116) |   ├─UnionTypeMemberSyntax
//@[033:0116) |   | └─ObjectTypeSyntax
//@[033:0034) |   |   ├─Token(LeftBrace) |{|
//@[034:0035) |   |   ├─Token(NewLine) |\n|
  type: 'a'
//@[002:0011) |   |   ├─ObjectTypePropertySyntax
//@[002:0006) |   |   | ├─IdentifierSyntax
//@[002:0006) |   |   | | └─Token(Identifier) |type|
//@[006:0007) |   |   | ├─Token(Colon) |:|
//@[008:0011) |   |   | └─StringSyntax
//@[008:0011) |   |   |   └─Token(StringComplete) |'a'|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0067) |   |   ├─ObjectTypePropertySyntax
//@[002:0024) |   |   | ├─DecoratorSyntax
//@[002:0003) |   |   | | ├─Token(At) |@|
//@[003:0024) |   |   | | └─FunctionCallSyntax
//@[003:0016) |   |   | |   ├─IdentifierSyntax
//@[003:0016) |   |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   |   | |   ├─Token(LeftParen) |(|
//@[017:0023) |   |   | |   ├─FunctionArgumentSyntax
//@[017:0023) |   |   | |   | └─StringSyntax
//@[017:0023) |   |   | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |   |   | |   └─Token(RightParen) |)|
//@[024:0025) |   |   | ├─Token(NewLine) |\n|
  prop: { type: 'a', value: bool } | typeB
//@[002:0006) |   |   | ├─IdentifierSyntax
//@[002:0006) |   |   | | └─Token(Identifier) |prop|
//@[006:0007) |   |   | ├─Token(Colon) |:|
//@[008:0042) |   |   | └─UnionTypeSyntax
//@[008:0034) |   |   |   ├─UnionTypeMemberSyntax
//@[008:0034) |   |   |   | └─ObjectTypeSyntax
//@[008:0009) |   |   |   |   ├─Token(LeftBrace) |{|
//@[010:0019) |   |   |   |   ├─ObjectTypePropertySyntax
//@[010:0014) |   |   |   |   | ├─IdentifierSyntax
//@[010:0014) |   |   |   |   | | └─Token(Identifier) |type|
//@[014:0015) |   |   |   |   | ├─Token(Colon) |:|
//@[016:0019) |   |   |   |   | └─StringSyntax
//@[016:0019) |   |   |   |   |   └─Token(StringComplete) |'a'|
//@[019:0020) |   |   |   |   ├─Token(Comma) |,|
//@[021:0032) |   |   |   |   ├─ObjectTypePropertySyntax
//@[021:0026) |   |   |   |   | ├─IdentifierSyntax
//@[021:0026) |   |   |   |   | | └─Token(Identifier) |value|
//@[026:0027) |   |   |   |   | ├─Token(Colon) |:|
//@[028:0032) |   |   |   |   | └─VariableAccessSyntax
//@[028:0032) |   |   |   |   |   └─IdentifierSyntax
//@[028:0032) |   |   |   |   |     └─Token(Identifier) |bool|
//@[033:0034) |   |   |   |   └─Token(RightBrace) |}|
//@[035:0036) |   |   |   ├─Token(Pipe) |||
//@[037:0042) |   |   |   └─UnionTypeMemberSyntax
//@[037:0042) |   |   |     └─VariableAccessSyntax
//@[037:0042) |   |   |       └─IdentifierSyntax
//@[037:0042) |   |   |         └─Token(Identifier) |typeB|
//@[042:0043) |   |   ├─Token(NewLine) |\n|
} | {
//@[000:0001) |   |   └─Token(RightBrace) |}|
//@[002:0003) |   ├─Token(Pipe) |||
//@[004:0094) |   └─UnionTypeMemberSyntax
//@[004:0094) |     └─ObjectTypeSyntax
//@[004:0005) |       ├─Token(LeftBrace) |{|
//@[005:0006) |       ├─Token(NewLine) |\n|
  type: 'b'
//@[002:0011) |       ├─ObjectTypePropertySyntax
//@[002:0006) |       | ├─IdentifierSyntax
//@[002:0006) |       | | └─Token(Identifier) |type|
//@[006:0007) |       | ├─Token(Colon) |:|
//@[008:0011) |       | └─StringSyntax
//@[008:0011) |       |   └─Token(StringComplete) |'b'|
//@[011:0012) |       ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0074) |       ├─ObjectTypePropertySyntax
//@[002:0024) |       | ├─DecoratorSyntax
//@[002:0003) |       | | ├─Token(At) |@|
//@[003:0024) |       | | └─FunctionCallSyntax
//@[003:0016) |       | |   ├─IdentifierSyntax
//@[003:0016) |       | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |       | |   ├─Token(LeftParen) |(|
//@[017:0023) |       | |   ├─FunctionArgumentSyntax
//@[017:0023) |       | |   | └─StringSyntax
//@[017:0023) |       | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |       | |   └─Token(RightParen) |)|
//@[024:0025) |       | ├─Token(NewLine) |\n|
  prop: discriminatedUnion1 | discriminatedUnion2
//@[002:0006) |       | ├─IdentifierSyntax
//@[002:0006) |       | | └─Token(Identifier) |prop|
//@[006:0007) |       | ├─Token(Colon) |:|
//@[008:0049) |       | └─UnionTypeSyntax
//@[008:0027) |       |   ├─UnionTypeMemberSyntax
//@[008:0027) |       |   | └─VariableAccessSyntax
//@[008:0027) |       |   |   └─IdentifierSyntax
//@[008:0027) |       |   |     └─Token(Identifier) |discriminatedUnion1|
//@[028:0029) |       |   ├─Token(Pipe) |||
//@[030:0049) |       |   └─UnionTypeMemberSyntax
//@[030:0049) |       |     └─VariableAccessSyntax
//@[030:0049) |       |       └─IdentifierSyntax
//@[030:0049) |       |         └─Token(Identifier) |discriminatedUnion2|
//@[049:0050) |       ├─Token(NewLine) |\n|
}
//@[000:0001) |       └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type inlineDiscriminatedUnion4 = {
//@[000:0086) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0030) | ├─IdentifierSyntax
//@[005:0030) | | └─Token(Identifier) |inlineDiscriminatedUnion4|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0086) | └─ObjectTypeSyntax
//@[033:0034) |   ├─Token(LeftBrace) |{|
//@[034:0035) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0049) |   ├─ObjectTypePropertySyntax
//@[002:0024) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0024) |   | | └─FunctionCallSyntax
//@[003:0016) |   | |   ├─IdentifierSyntax
//@[003:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0023) |   | |   ├─FunctionArgumentSyntax
//@[017:0023) |   | |   | └─StringSyntax
//@[017:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |   | |   └─Token(RightParen) |)|
//@[024:0025) |   | ├─Token(NewLine) |\n|
  prop: (typeA | typeC)?
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |prop|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0024) |   | └─NullableTypeSyntax
//@[008:0023) |   |   ├─ParenthesizedExpressionSyntax
//@[008:0009) |   |   | ├─Token(LeftParen) |(|
//@[009:0022) |   |   | ├─UnionTypeSyntax
//@[009:0014) |   |   | | ├─UnionTypeMemberSyntax
//@[009:0014) |   |   | | | └─VariableAccessSyntax
//@[009:0014) |   |   | | |   └─IdentifierSyntax
//@[009:0014) |   |   | | |     └─Token(Identifier) |typeA|
//@[015:0016) |   |   | | ├─Token(Pipe) |||
//@[017:0022) |   |   | | └─UnionTypeMemberSyntax
//@[017:0022) |   |   | |   └─VariableAccessSyntax
//@[017:0022) |   |   | |     └─IdentifierSyntax
//@[017:0022) |   |   | |       └─Token(Identifier) |typeC|
//@[022:0023) |   |   | └─Token(RightParen) |)|
//@[023:0024) |   |   └─Token(Question) |?|
//@[024:0025) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatorUnionAsPropertyType = {
//@[000:0101) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0037) | ├─IdentifierSyntax
//@[005:0037) | | └─Token(Identifier) |discriminatorUnionAsPropertyType|
//@[038:0039) | ├─Token(Assignment) |=|
//@[040:0101) | └─ObjectTypeSyntax
//@[040:0041) |   ├─Token(LeftBrace) |{|
//@[041:0042) |   ├─Token(NewLine) |\n|
  prop1: discriminatedUnion1
//@[002:0028) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |prop1|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0028) |   | └─VariableAccessSyntax
//@[009:0028) |   |   └─IdentifierSyntax
//@[009:0028) |   |     └─Token(Identifier) |discriminatedUnion1|
//@[028:0029) |   ├─Token(NewLine) |\n|
  prop2: discriminatedUnion3
//@[002:0028) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |prop2|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0028) |   | └─VariableAccessSyntax
//@[009:0028) |   |   └─IdentifierSyntax
//@[009:0028) |   |     └─Token(Identifier) |discriminatedUnion3|
//@[028:0029) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatedUnionInlineAdditionalProps1 = {
//@[000:0095) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0045) | ├─IdentifierSyntax
//@[005:0045) | | └─Token(Identifier) |discriminatedUnionInlineAdditionalProps1|
//@[046:0047) | ├─Token(Assignment) |=|
//@[048:0095) | └─ObjectTypeSyntax
//@[048:0049) |   ├─Token(LeftBrace) |{|
//@[049:0050) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0043) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[002:0024) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0024) |   | | └─FunctionCallSyntax
//@[003:0016) |   | |   ├─IdentifierSyntax
//@[003:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0023) |   | |   ├─FunctionArgumentSyntax
//@[017:0023) |   | |   | └─StringSyntax
//@[017:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |   | |   └─Token(RightParen) |)|
//@[024:0025) |   | ├─Token(NewLine) |\n|
  *: typeA | typeB
//@[002:0003) |   | ├─Token(Asterisk) |*|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0018) |   | └─UnionTypeSyntax
//@[005:0010) |   |   ├─UnionTypeMemberSyntax
//@[005:0010) |   |   | └─VariableAccessSyntax
//@[005:0010) |   |   |   └─IdentifierSyntax
//@[005:0010) |   |   |     └─Token(Identifier) |typeA|
//@[011:0012) |   |   ├─Token(Pipe) |||
//@[013:0018) |   |   └─UnionTypeMemberSyntax
//@[013:0018) |   |     └─VariableAccessSyntax
//@[013:0018) |   |       └─IdentifierSyntax
//@[013:0018) |   |         └─Token(Identifier) |typeB|
//@[018:0019) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatedUnionInlineAdditionalProps2 = {
//@[000:0098) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0045) | ├─IdentifierSyntax
//@[005:0045) | | └─Token(Identifier) |discriminatedUnionInlineAdditionalProps2|
//@[046:0047) | ├─Token(Assignment) |=|
//@[048:0098) | └─ObjectTypeSyntax
//@[048:0049) |   ├─Token(LeftBrace) |{|
//@[049:0050) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0046) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[002:0024) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0024) |   | | └─FunctionCallSyntax
//@[003:0016) |   | |   ├─IdentifierSyntax
//@[003:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0023) |   | |   ├─FunctionArgumentSyntax
//@[017:0023) |   | |   | └─StringSyntax
//@[017:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |   | |   └─Token(RightParen) |)|
//@[024:0025) |   | ├─Token(NewLine) |\n|
  *: (typeA | typeB)?
//@[002:0003) |   | ├─Token(Asterisk) |*|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0021) |   | └─NullableTypeSyntax
//@[005:0020) |   |   ├─ParenthesizedExpressionSyntax
//@[005:0006) |   |   | ├─Token(LeftParen) |(|
//@[006:0019) |   |   | ├─UnionTypeSyntax
//@[006:0011) |   |   | | ├─UnionTypeMemberSyntax
//@[006:0011) |   |   | | | └─VariableAccessSyntax
//@[006:0011) |   |   | | |   └─IdentifierSyntax
//@[006:0011) |   |   | | |     └─Token(Identifier) |typeA|
//@[012:0013) |   |   | | ├─Token(Pipe) |||
//@[014:0019) |   |   | | └─UnionTypeMemberSyntax
//@[014:0019) |   |   | |   └─VariableAccessSyntax
//@[014:0019) |   |   | |     └─IdentifierSyntax
//@[014:0019) |   |   | |       └─Token(Identifier) |typeB|
//@[019:0020) |   |   | └─Token(RightParen) |)|
//@[020:0021) |   |   └─Token(Question) |?|
//@[021:0022) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0111) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatorMemberHasAdditionalProperties1 = typeA | typeF | { type: 'g', *: int }
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0048) | ├─IdentifierSyntax
//@[005:0048) | | └─Token(Identifier) |discriminatorMemberHasAdditionalProperties1|
//@[049:0050) | ├─Token(Assignment) |=|
//@[051:0088) | └─UnionTypeSyntax
//@[051:0056) |   ├─UnionTypeMemberSyntax
//@[051:0056) |   | └─VariableAccessSyntax
//@[051:0056) |   |   └─IdentifierSyntax
//@[051:0056) |   |     └─Token(Identifier) |typeA|
//@[057:0058) |   ├─Token(Pipe) |||
//@[059:0064) |   ├─UnionTypeMemberSyntax
//@[059:0064) |   | └─VariableAccessSyntax
//@[059:0064) |   |   └─IdentifierSyntax
//@[059:0064) |   |     └─Token(Identifier) |typeF|
//@[065:0066) |   ├─Token(Pipe) |||
//@[067:0088) |   └─UnionTypeMemberSyntax
//@[067:0088) |     └─ObjectTypeSyntax
//@[067:0068) |       ├─Token(LeftBrace) |{|
//@[069:0078) |       ├─ObjectTypePropertySyntax
//@[069:0073) |       | ├─IdentifierSyntax
//@[069:0073) |       | | └─Token(Identifier) |type|
//@[073:0074) |       | ├─Token(Colon) |:|
//@[075:0078) |       | └─StringSyntax
//@[075:0078) |       |   └─Token(StringComplete) |'g'|
//@[078:0079) |       ├─Token(Comma) |,|
//@[080:0086) |       ├─ObjectTypeAdditionalPropertiesSyntax
//@[080:0081) |       | ├─Token(Asterisk) |*|
//@[081:0082) |       | ├─Token(Colon) |:|
//@[083:0086) |       | └─VariableAccessSyntax
//@[083:0086) |       |   └─IdentifierSyntax
//@[083:0086) |       |     └─Token(Identifier) |int|
//@[087:0088) |       └─Token(RightBrace) |}|
//@[088:0090) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0137) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatorInnerSelfOptionalCycle1 = typeA | {
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0041) | ├─IdentifierSyntax
//@[005:0041) | | └─Token(Identifier) |discriminatorInnerSelfOptionalCycle1|
//@[042:0043) | ├─Token(Assignment) |=|
//@[044:0114) | └─UnionTypeSyntax
//@[044:0049) |   ├─UnionTypeMemberSyntax
//@[044:0049) |   | └─VariableAccessSyntax
//@[044:0049) |   |   └─IdentifierSyntax
//@[044:0049) |   |     └─Token(Identifier) |typeA|
//@[050:0051) |   ├─Token(Pipe) |||
//@[052:0114) |   └─UnionTypeMemberSyntax
//@[052:0114) |     └─ObjectTypeSyntax
//@[052:0053) |       ├─Token(LeftBrace) |{|
//@[053:0054) |       ├─Token(NewLine) |\n|
  type: 'b'
//@[002:0011) |       ├─ObjectTypePropertySyntax
//@[002:0006) |       | ├─IdentifierSyntax
//@[002:0006) |       | | └─Token(Identifier) |type|
//@[006:0007) |       | ├─Token(Colon) |:|
//@[008:0011) |       | └─StringSyntax
//@[008:0011) |       |   └─Token(StringComplete) |'b'|
//@[011:0012) |       ├─Token(NewLine) |\n|
  value: discriminatorInnerSelfOptionalCycle1?
//@[002:0046) |       ├─ObjectTypePropertySyntax
//@[002:0007) |       | ├─IdentifierSyntax
//@[002:0007) |       | | └─Token(Identifier) |value|
//@[007:0008) |       | ├─Token(Colon) |:|
//@[009:0046) |       | └─NullableTypeSyntax
//@[009:0045) |       |   ├─VariableAccessSyntax
//@[009:0045) |       |   | └─IdentifierSyntax
//@[009:0045) |       |   |   └─Token(Identifier) |discriminatorInnerSelfOptionalCycle1|
//@[045:0046) |       |   └─Token(Question) |?|
//@[046:0047) |       ├─Token(NewLine) |\n|
}
//@[000:0001) |       └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatedUnionMemberOptionalCycle1 = {
//@[000:0144) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0043) | ├─IdentifierSyntax
//@[005:0043) | | └─Token(Identifier) |discriminatedUnionMemberOptionalCycle1|
//@[044:0045) | ├─Token(Assignment) |=|
//@[046:0144) | └─ObjectTypeSyntax
//@[046:0047) |   ├─Token(LeftBrace) |{|
//@[047:0048) |   ├─Token(NewLine) |\n|
  type: 'b'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'b'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0082) |   ├─ObjectTypePropertySyntax
//@[002:0024) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0024) |   | | └─FunctionCallSyntax
//@[003:0016) |   | |   ├─IdentifierSyntax
//@[003:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0023) |   | |   ├─FunctionArgumentSyntax
//@[017:0023) |   | |   | └─StringSyntax
//@[017:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |   | |   └─Token(RightParen) |)|
//@[024:0025) |   | ├─Token(NewLine) |\n|
  prop: (typeA | discriminatedUnionMemberOptionalCycle1)?
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |prop|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0057) |   | └─NullableTypeSyntax
//@[008:0056) |   |   ├─ParenthesizedExpressionSyntax
//@[008:0009) |   |   | ├─Token(LeftParen) |(|
//@[009:0055) |   |   | ├─UnionTypeSyntax
//@[009:0014) |   |   | | ├─UnionTypeMemberSyntax
//@[009:0014) |   |   | | | └─VariableAccessSyntax
//@[009:0014) |   |   | | |   └─IdentifierSyntax
//@[009:0014) |   |   | | |     └─Token(Identifier) |typeA|
//@[015:0016) |   |   | | ├─Token(Pipe) |||
//@[017:0055) |   |   | | └─UnionTypeMemberSyntax
//@[017:0055) |   |   | |   └─VariableAccessSyntax
//@[017:0055) |   |   | |     └─IdentifierSyntax
//@[017:0055) |   |   | |       └─Token(Identifier) |discriminatedUnionMemberOptionalCycle1|
//@[055:0056) |   |   | └─Token(RightParen) |)|
//@[056:0057) |   |   └─Token(Question) |?|
//@[057:0058) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatedUnionTuple1 = [
//@[000:0066) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0029) | ├─IdentifierSyntax
//@[005:0029) | | └─Token(Identifier) |discriminatedUnionTuple1|
//@[030:0031) | ├─Token(Assignment) |=|
//@[032:0066) | └─TupleTypeSyntax
//@[032:0033) |   ├─Token(LeftSquare) |[|
//@[033:0034) |   ├─Token(NewLine) |\n|
  discriminatedUnion1
//@[002:0021) |   ├─TupleTypeItemSyntax
//@[002:0021) |   | └─VariableAccessSyntax
//@[002:0021) |   |   └─IdentifierSyntax
//@[002:0021) |   |     └─Token(Identifier) |discriminatedUnion1|
//@[021:0022) |   ├─Token(NewLine) |\n|
  string
//@[002:0008) |   ├─TupleTypeItemSyntax
//@[002:0008) |   | └─VariableAccessSyntax
//@[002:0008) |   |   └─IdentifierSyntax
//@[002:0008) |   |     └─Token(Identifier) |string|
//@[008:0009) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatedUnionInlineTuple1 = [
//@[000:0122) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0035) | ├─IdentifierSyntax
//@[005:0035) | | └─Token(Identifier) |discriminatedUnionInlineTuple1|
//@[036:0037) | ├─Token(Assignment) |=|
//@[038:0122) | └─TupleTypeSyntax
//@[038:0039) |   ├─Token(LeftSquare) |[|
//@[039:0040) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0071) |   ├─TupleTypeItemSyntax
//@[002:0024) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0024) |   | | └─FunctionCallSyntax
//@[003:0016) |   | |   ├─IdentifierSyntax
//@[003:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0023) |   | |   ├─FunctionArgumentSyntax
//@[017:0023) |   | |   | └─StringSyntax
//@[017:0023) |   | |   |   └─Token(StringComplete) |'type'|
//@[023:0024) |   | |   └─Token(RightParen) |)|
//@[024:0025) |   | ├─Token(NewLine) |\n|
  typeA | typeB | { type: 'c', value: object }
//@[002:0046) |   | └─UnionTypeSyntax
//@[002:0007) |   |   ├─UnionTypeMemberSyntax
//@[002:0007) |   |   | └─VariableAccessSyntax
//@[002:0007) |   |   |   └─IdentifierSyntax
//@[002:0007) |   |   |     └─Token(Identifier) |typeA|
//@[008:0009) |   |   ├─Token(Pipe) |||
//@[010:0015) |   |   ├─UnionTypeMemberSyntax
//@[010:0015) |   |   | └─VariableAccessSyntax
//@[010:0015) |   |   |   └─IdentifierSyntax
//@[010:0015) |   |   |     └─Token(Identifier) |typeB|
//@[016:0017) |   |   ├─Token(Pipe) |||
//@[018:0046) |   |   └─UnionTypeMemberSyntax
//@[018:0046) |   |     └─ObjectTypeSyntax
//@[018:0019) |   |       ├─Token(LeftBrace) |{|
//@[020:0029) |   |       ├─ObjectTypePropertySyntax
//@[020:0024) |   |       | ├─IdentifierSyntax
//@[020:0024) |   |       | | └─Token(Identifier) |type|
//@[024:0025) |   |       | ├─Token(Colon) |:|
//@[026:0029) |   |       | └─StringSyntax
//@[026:0029) |   |       |   └─Token(StringComplete) |'c'|
//@[029:0030) |   |       ├─Token(Comma) |,|
//@[031:0044) |   |       ├─ObjectTypePropertySyntax
//@[031:0036) |   |       | ├─IdentifierSyntax
//@[031:0036) |   |       | | └─Token(Identifier) |value|
//@[036:0037) |   |       | ├─Token(Colon) |:|
//@[038:0044) |   |       | └─VariableAccessSyntax
//@[038:0044) |   |       |   └─IdentifierSyntax
//@[038:0044) |   |       |     └─Token(Identifier) |object|
//@[045:0046) |   |       └─Token(RightBrace) |}|
//@[046:0047) |   ├─Token(NewLine) |\n|
  string
//@[002:0008) |   ├─TupleTypeItemSyntax
//@[002:0008) |   | └─VariableAccessSyntax
//@[002:0008) |   |   └─IdentifierSyntax
//@[002:0008) |   |     └─Token(Identifier) |string|
//@[008:0009) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

param paramDiscriminatedUnionTypeAlias1 discriminatedUnion1
//@[000:0059) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0039) | ├─IdentifierSyntax
//@[006:0039) | | └─Token(Identifier) |paramDiscriminatedUnionTypeAlias1|
//@[040:0059) | └─VariableAccessSyntax
//@[040:0059) |   └─IdentifierSyntax
//@[040:0059) |     └─Token(Identifier) |discriminatedUnion1|
//@[059:0060) ├─Token(NewLine) |\n|
param paramDiscriminatedUnionTypeAlias2 discriminatedUnion5
//@[000:0059) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0039) | ├─IdentifierSyntax
//@[006:0039) | | └─Token(Identifier) |paramDiscriminatedUnionTypeAlias2|
//@[040:0059) | └─VariableAccessSyntax
//@[040:0059) |   └─IdentifierSyntax
//@[040:0059) |     └─Token(Identifier) |discriminatedUnion5|
//@[059:0061) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0073) ├─ParameterDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
param paramInlineDiscriminatedUnion1 typeA | typeB
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0036) | ├─IdentifierSyntax
//@[006:0036) | | └─Token(Identifier) |paramInlineDiscriminatedUnion1|
//@[037:0050) | └─UnionTypeSyntax
//@[037:0042) |   ├─UnionTypeMemberSyntax
//@[037:0042) |   | └─VariableAccessSyntax
//@[037:0042) |   |   └─IdentifierSyntax
//@[037:0042) |   |     └─Token(Identifier) |typeA|
//@[043:0044) |   ├─Token(Pipe) |||
//@[045:0050) |   └─UnionTypeMemberSyntax
//@[045:0050) |     └─VariableAccessSyntax
//@[045:0050) |       └─IdentifierSyntax
//@[045:0050) |         └─Token(Identifier) |typeB|
//@[050:0052) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0101) ├─ParameterDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
param paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0036) | ├─IdentifierSyntax
//@[006:0036) | | └─Token(Identifier) |paramInlineDiscriminatedUnion2|
//@[037:0052) | ├─ParenthesizedExpressionSyntax
//@[037:0038) | | ├─Token(LeftParen) |(|
//@[038:0051) | | ├─UnionTypeSyntax
//@[038:0043) | | | ├─UnionTypeMemberSyntax
//@[038:0043) | | | | └─VariableAccessSyntax
//@[038:0043) | | | |   └─IdentifierSyntax
//@[038:0043) | | | |     └─Token(Identifier) |typeA|
//@[044:0045) | | | ├─Token(Pipe) |||
//@[046:0051) | | | └─UnionTypeMemberSyntax
//@[046:0051) | | |   └─VariableAccessSyntax
//@[046:0051) | | |     └─IdentifierSyntax
//@[046:0051) | | |       └─Token(Identifier) |typeB|
//@[051:0052) | | └─Token(RightParen) |)|
//@[053:0078) | └─ParameterDefaultValueSyntax
//@[053:0054) |   ├─Token(Assignment) |=|
//@[055:0078) |   └─ObjectSyntax
//@[055:0056) |     ├─Token(LeftBrace) |{|
//@[057:0066) |     ├─ObjectPropertySyntax
//@[057:0061) |     | ├─IdentifierSyntax
//@[057:0061) |     | | └─Token(Identifier) |type|
//@[061:0062) |     | ├─Token(Colon) |:|
//@[063:0066) |     | └─StringSyntax
//@[063:0066) |     |   └─Token(StringComplete) |'b'|
//@[066:0067) |     ├─Token(Comma) |,|
//@[068:0076) |     ├─ObjectPropertySyntax
//@[068:0073) |     | ├─IdentifierSyntax
//@[068:0073) |     | | └─Token(Identifier) |value|
//@[073:0074) |     | ├─Token(Colon) |:|
//@[075:0076) |     | └─IntegerLiteralSyntax
//@[075:0076) |     |   └─Token(Integer) |0|
//@[077:0078) |     └─Token(RightBrace) |}|
//@[078:0080) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0076) ├─ParameterDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
param paramInlineDiscriminatedUnion3 (typeA | typeB)?
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0036) | ├─IdentifierSyntax
//@[006:0036) | | └─Token(Identifier) |paramInlineDiscriminatedUnion3|
//@[037:0053) | └─NullableTypeSyntax
//@[037:0052) |   ├─ParenthesizedExpressionSyntax
//@[037:0038) |   | ├─Token(LeftParen) |(|
//@[038:0051) |   | ├─UnionTypeSyntax
//@[038:0043) |   | | ├─UnionTypeMemberSyntax
//@[038:0043) |   | | | └─VariableAccessSyntax
//@[038:0043) |   | | |   └─IdentifierSyntax
//@[038:0043) |   | | |     └─Token(Identifier) |typeA|
//@[044:0045) |   | | ├─Token(Pipe) |||
//@[046:0051) |   | | └─UnionTypeMemberSyntax
//@[046:0051) |   | |   └─VariableAccessSyntax
//@[046:0051) |   | |     └─IdentifierSyntax
//@[046:0051) |   | |       └─Token(Identifier) |typeB|
//@[051:0052) |   | └─Token(RightParen) |)|
//@[052:0053) |   └─Token(Question) |?|
//@[053:0055) ├─Token(NewLine) |\n\n|

output outputDiscriminatedUnionTypeAlias1 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[000:0091) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0041) | ├─IdentifierSyntax
//@[007:0041) | | └─Token(Identifier) |outputDiscriminatedUnionTypeAlias1|
//@[042:0061) | ├─VariableAccessSyntax
//@[042:0061) | | └─IdentifierSyntax
//@[042:0061) | |   └─Token(Identifier) |discriminatedUnion1|
//@[062:0063) | ├─Token(Assignment) |=|
//@[064:0091) | └─ObjectSyntax
//@[064:0065) |   ├─Token(LeftBrace) |{|
//@[066:0075) |   ├─ObjectPropertySyntax
//@[066:0070) |   | ├─IdentifierSyntax
//@[066:0070) |   | | └─Token(Identifier) |type|
//@[070:0071) |   | ├─Token(Colon) |:|
//@[072:0075) |   | └─StringSyntax
//@[072:0075) |   |   └─Token(StringComplete) |'a'|
//@[075:0076) |   ├─Token(Comma) |,|
//@[077:0089) |   ├─ObjectPropertySyntax
//@[077:0082) |   | ├─IdentifierSyntax
//@[077:0082) |   | | └─Token(Identifier) |value|
//@[082:0083) |   | ├─Token(Colon) |:|
//@[084:0089) |   | └─StringSyntax
//@[084:0089) |   |   └─Token(StringComplete) |'str'|
//@[090:0091) |   └─Token(RightBrace) |}|
//@[091:0092) ├─Token(NewLine) |\n|
@discriminator('type')
//@[000:0114) ├─OutputDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
output outputDiscriminatedUnionTypeAlias2 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0041) | ├─IdentifierSyntax
//@[007:0041) | | └─Token(Identifier) |outputDiscriminatedUnionTypeAlias2|
//@[042:0061) | ├─VariableAccessSyntax
//@[042:0061) | | └─IdentifierSyntax
//@[042:0061) | |   └─Token(Identifier) |discriminatedUnion1|
//@[062:0063) | ├─Token(Assignment) |=|
//@[064:0091) | └─ObjectSyntax
//@[064:0065) |   ├─Token(LeftBrace) |{|
//@[066:0075) |   ├─ObjectPropertySyntax
//@[066:0070) |   | ├─IdentifierSyntax
//@[066:0070) |   | | └─Token(Identifier) |type|
//@[070:0071) |   | ├─Token(Colon) |:|
//@[072:0075) |   | └─StringSyntax
//@[072:0075) |   |   └─Token(StringComplete) |'a'|
//@[075:0076) |   ├─Token(Comma) |,|
//@[077:0089) |   ├─ObjectPropertySyntax
//@[077:0082) |   | ├─IdentifierSyntax
//@[077:0082) |   | | └─Token(Identifier) |value|
//@[082:0083) |   | ├─Token(Colon) |:|
//@[084:0089) |   | └─StringSyntax
//@[084:0089) |   |   └─Token(StringComplete) |'str'|
//@[090:0091) |   └─Token(RightBrace) |}|
//@[091:0092) ├─Token(NewLine) |\n|
output outputDiscriminatedUnionTypeAlias3 discriminatedUnion5 = null
//@[000:0068) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0041) | ├─IdentifierSyntax
//@[007:0041) | | └─Token(Identifier) |outputDiscriminatedUnionTypeAlias3|
//@[042:0061) | ├─VariableAccessSyntax
//@[042:0061) | | └─IdentifierSyntax
//@[042:0061) | |   └─Token(Identifier) |discriminatedUnion5|
//@[062:0063) | ├─Token(Assignment) |=|
//@[064:0068) | └─NullLiteralSyntax
//@[064:0068) |   └─Token(NullKeyword) |null|
//@[068:0070) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0131) ├─OutputDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
output outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0038) | ├─IdentifierSyntax
//@[007:0038) | | └─Token(Identifier) |outputInlineDiscriminatedUnion1|
//@[039:0080) | ├─UnionTypeSyntax
//@[039:0044) | | ├─UnionTypeMemberSyntax
//@[039:0044) | | | └─VariableAccessSyntax
//@[039:0044) | | |   └─IdentifierSyntax
//@[039:0044) | | |     └─Token(Identifier) |typeA|
//@[045:0046) | | ├─Token(Pipe) |||
//@[047:0052) | | ├─UnionTypeMemberSyntax
//@[047:0052) | | | └─VariableAccessSyntax
//@[047:0052) | | |   └─IdentifierSyntax
//@[047:0052) | | |     └─Token(Identifier) |typeB|
//@[053:0054) | | ├─Token(Pipe) |||
//@[055:0080) | | └─UnionTypeMemberSyntax
//@[055:0080) | |   └─ObjectTypeSyntax
//@[055:0056) | |     ├─Token(LeftBrace) |{|
//@[057:0066) | |     ├─ObjectTypePropertySyntax
//@[057:0061) | |     | ├─IdentifierSyntax
//@[057:0061) | |     | | └─Token(Identifier) |type|
//@[061:0062) | |     | ├─Token(Colon) |:|
//@[063:0066) | |     | └─StringSyntax
//@[063:0066) | |     |   └─Token(StringComplete) |'c'|
//@[066:0067) | |     ├─Token(Comma) |,|
//@[068:0078) | |     ├─ObjectTypePropertySyntax
//@[068:0073) | |     | ├─IdentifierSyntax
//@[068:0073) | |     | | └─Token(Identifier) |value|
//@[073:0074) | |     | ├─Token(Colon) |:|
//@[075:0078) | |     | └─VariableAccessSyntax
//@[075:0078) | |     |   └─IdentifierSyntax
//@[075:0078) | |     |     └─Token(Identifier) |int|
//@[079:0080) | |     └─Token(RightBrace) |}|
//@[081:0082) | ├─Token(Assignment) |=|
//@[083:0108) | └─ObjectSyntax
//@[083:0084) |   ├─Token(LeftBrace) |{|
//@[085:0094) |   ├─ObjectPropertySyntax
//@[085:0089) |   | ├─IdentifierSyntax
//@[085:0089) |   | | └─Token(Identifier) |type|
//@[089:0090) |   | ├─Token(Colon) |:|
//@[091:0094) |   | └─StringSyntax
//@[091:0094) |   |   └─Token(StringComplete) |'a'|
//@[094:0095) |   ├─Token(Comma) |,|
//@[096:0106) |   ├─ObjectPropertySyntax
//@[096:0101) |   | ├─IdentifierSyntax
//@[096:0101) |   | | └─Token(Identifier) |value|
//@[101:0102) |   | ├─Token(Colon) |:|
//@[103:0106) |   | └─StringSyntax
//@[103:0106) |   |   └─Token(StringComplete) |'a'|
//@[107:0108) |   └─Token(RightBrace) |}|
//@[108:0110) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0131) ├─OutputDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
output outputInlineDiscriminatedUnion2 typeA | typeB | ({ type: 'c', value: int }) = { type: 'c', value: 1 }
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0038) | ├─IdentifierSyntax
//@[007:0038) | | └─Token(Identifier) |outputInlineDiscriminatedUnion2|
//@[039:0082) | ├─UnionTypeSyntax
//@[039:0044) | | ├─UnionTypeMemberSyntax
//@[039:0044) | | | └─VariableAccessSyntax
//@[039:0044) | | |   └─IdentifierSyntax
//@[039:0044) | | |     └─Token(Identifier) |typeA|
//@[045:0046) | | ├─Token(Pipe) |||
//@[047:0052) | | ├─UnionTypeMemberSyntax
//@[047:0052) | | | └─VariableAccessSyntax
//@[047:0052) | | |   └─IdentifierSyntax
//@[047:0052) | | |     └─Token(Identifier) |typeB|
//@[053:0054) | | ├─Token(Pipe) |||
//@[055:0082) | | └─UnionTypeMemberSyntax
//@[055:0082) | |   └─ParenthesizedExpressionSyntax
//@[055:0056) | |     ├─Token(LeftParen) |(|
//@[056:0081) | |     ├─ObjectTypeSyntax
//@[056:0057) | |     | ├─Token(LeftBrace) |{|
//@[058:0067) | |     | ├─ObjectTypePropertySyntax
//@[058:0062) | |     | | ├─IdentifierSyntax
//@[058:0062) | |     | | | └─Token(Identifier) |type|
//@[062:0063) | |     | | ├─Token(Colon) |:|
//@[064:0067) | |     | | └─StringSyntax
//@[064:0067) | |     | |   └─Token(StringComplete) |'c'|
//@[067:0068) | |     | ├─Token(Comma) |,|
//@[069:0079) | |     | ├─ObjectTypePropertySyntax
//@[069:0074) | |     | | ├─IdentifierSyntax
//@[069:0074) | |     | | | └─Token(Identifier) |value|
//@[074:0075) | |     | | ├─Token(Colon) |:|
//@[076:0079) | |     | | └─VariableAccessSyntax
//@[076:0079) | |     | |   └─IdentifierSyntax
//@[076:0079) | |     | |     └─Token(Identifier) |int|
//@[080:0081) | |     | └─Token(RightBrace) |}|
//@[081:0082) | |     └─Token(RightParen) |)|
//@[083:0084) | ├─Token(Assignment) |=|
//@[085:0108) | └─ObjectSyntax
//@[085:0086) |   ├─Token(LeftBrace) |{|
//@[087:0096) |   ├─ObjectPropertySyntax
//@[087:0091) |   | ├─IdentifierSyntax
//@[087:0091) |   | | └─Token(Identifier) |type|
//@[091:0092) |   | ├─Token(Colon) |:|
//@[093:0096) |   | └─StringSyntax
//@[093:0096) |   |   └─Token(StringComplete) |'c'|
//@[096:0097) |   ├─Token(Comma) |,|
//@[098:0106) |   ├─ObjectPropertySyntax
//@[098:0103) |   | ├─IdentifierSyntax
//@[098:0103) |   | | └─Token(Identifier) |value|
//@[103:0104) |   | ├─Token(Colon) |:|
//@[105:0106) |   | └─IntegerLiteralSyntax
//@[105:0106) |   |   └─Token(Integer) |1|
//@[107:0108) |   └─Token(RightBrace) |}|
//@[108:0110) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0085) ├─OutputDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'type'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
output outputInlineDiscriminatedUnion3 (typeA | typeB)? = null
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0038) | ├─IdentifierSyntax
//@[007:0038) | | └─Token(Identifier) |outputInlineDiscriminatedUnion3|
//@[039:0055) | ├─NullableTypeSyntax
//@[039:0054) | | ├─ParenthesizedExpressionSyntax
//@[039:0040) | | | ├─Token(LeftParen) |(|
//@[040:0053) | | | ├─UnionTypeSyntax
//@[040:0045) | | | | ├─UnionTypeMemberSyntax
//@[040:0045) | | | | | └─VariableAccessSyntax
//@[040:0045) | | | | |   └─IdentifierSyntax
//@[040:0045) | | | | |     └─Token(Identifier) |typeA|
//@[046:0047) | | | | ├─Token(Pipe) |||
//@[048:0053) | | | | └─UnionTypeMemberSyntax
//@[048:0053) | | | |   └─VariableAccessSyntax
//@[048:0053) | | | |     └─IdentifierSyntax
//@[048:0053) | | | |       └─Token(Identifier) |typeB|
//@[053:0054) | | | └─Token(RightParen) |)|
//@[054:0055) | | └─Token(Question) |?|
//@[056:0057) | ├─Token(Assignment) |=|
//@[058:0062) | └─NullLiteralSyntax
//@[058:0062) |   └─Token(NullKeyword) |null|
//@[062:0063) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
