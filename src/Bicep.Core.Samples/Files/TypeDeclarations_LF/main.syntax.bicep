@description('The foo type')
//@[00:900) ProgramSyntax
//@[00:298) ├─TypeDeclarationSyntax
//@[00:028) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:028) | | └─FunctionCallSyntax
//@[01:012) | |   ├─IdentifierSyntax
//@[01:012) | |   | └─Token(Identifier) |description|
//@[12:013) | |   ├─Token(LeftParen) |(|
//@[13:027) | |   ├─FunctionArgumentSyntax
//@[13:027) | |   | └─StringSyntax
//@[13:027) | |   |   └─Token(StringComplete) |'The foo type'|
//@[27:028) | |   └─Token(RightParen) |)|
//@[28:029) | ├─Token(NewLine) |\n|
@sealed()
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |sealed|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:010) | ├─Token(NewLine) |\n|
type foo = {
//@[00:004) | ├─Token(Identifier) |type|
//@[05:008) | ├─IdentifierSyntax
//@[05:008) | | └─Token(Identifier) |foo|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:259) | └─ObjectTypeSyntax
//@[11:012) |   ├─Token(LeftBrace) |{|
//@[12:013) |   ├─Token(NewLine) |\n|
  @minLength(3)
//@[02:089) |   ├─ObjectTypePropertySyntax
//@[02:015) |   | ├─DecoratorSyntax
//@[02:003) |   | | ├─Token(At) |@|
//@[03:015) |   | | └─FunctionCallSyntax
//@[03:012) |   | |   ├─IdentifierSyntax
//@[03:012) |   | |   | └─Token(Identifier) |minLength|
//@[12:013) |   | |   ├─Token(LeftParen) |(|
//@[13:014) |   | |   ├─FunctionArgumentSyntax
//@[13:014) |   | |   | └─IntegerLiteralSyntax
//@[13:014) |   | |   |   └─Token(Integer) |3|
//@[14:015) |   | |   └─Token(RightParen) |)|
//@[15:016) |   | ├─Token(NewLine) |\n|
  @maxLength(10)
//@[02:016) |   | ├─DecoratorSyntax
//@[02:003) |   | | ├─Token(At) |@|
//@[03:016) |   | | └─FunctionCallSyntax
//@[03:012) |   | |   ├─IdentifierSyntax
//@[03:012) |   | |   | └─Token(Identifier) |maxLength|
//@[12:013) |   | |   ├─Token(LeftParen) |(|
//@[13:015) |   | |   ├─FunctionArgumentSyntax
//@[13:015) |   | |   | └─IntegerLiteralSyntax
//@[13:015) |   | |   |   └─Token(Integer) |10|
//@[15:016) |   | |   └─Token(RightParen) |)|
//@[16:017) |   | ├─Token(NewLine) |\n|
  @description('A string property')
//@[02:035) |   | ├─DecoratorSyntax
//@[02:003) |   | | ├─Token(At) |@|
//@[03:035) |   | | └─FunctionCallSyntax
//@[03:014) |   | |   ├─IdentifierSyntax
//@[03:014) |   | |   | └─Token(Identifier) |description|
//@[14:015) |   | |   ├─Token(LeftParen) |(|
//@[15:034) |   | |   ├─FunctionArgumentSyntax
//@[15:034) |   | |   | └─StringSyntax
//@[15:034) |   | |   |   └─Token(StringComplete) |'A string property'|
//@[34:035) |   | |   └─Token(RightParen) |)|
//@[35:036) |   | ├─Token(NewLine) |\n|
  stringProp: string
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |stringProp|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:020) |   | └─SimpleTypeSyntax
//@[14:020) |   |   └─Token(Identifier) |string|
//@[20:022) |   ├─Token(NewLine) |\n\n|

  objectProp: {
//@[02:088) |   ├─ObjectTypePropertySyntax
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |objectProp|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:088) |   | └─ObjectTypeSyntax
//@[14:015) |   |   ├─Token(LeftBrace) |{|
//@[15:016) |   |   ├─Token(NewLine) |\n|
    @minValue(1)
//@[04:033) |   |   ├─ObjectTypePropertySyntax
//@[04:016) |   |   | ├─DecoratorSyntax
//@[04:005) |   |   | | ├─Token(At) |@|
//@[05:016) |   |   | | └─FunctionCallSyntax
//@[05:013) |   |   | |   ├─IdentifierSyntax
//@[05:013) |   |   | |   | └─Token(Identifier) |minValue|
//@[13:014) |   |   | |   ├─Token(LeftParen) |(|
//@[14:015) |   |   | |   ├─FunctionArgumentSyntax
//@[14:015) |   |   | |   | └─IntegerLiteralSyntax
//@[14:015) |   |   | |   |   └─Token(Integer) |1|
//@[15:016) |   |   | |   └─Token(RightParen) |)|
//@[16:017) |   |   | ├─Token(NewLine) |\n|
    intProp: int
//@[04:011) |   |   | ├─IdentifierSyntax
//@[04:011) |   |   | | └─Token(Identifier) |intProp|
//@[11:012) |   |   | ├─Token(Colon) |:|
//@[13:016) |   |   | └─SimpleTypeSyntax
//@[13:016) |   |   |   └─Token(Identifier) |int|
//@[16:018) |   |   ├─Token(NewLine) |\n\n|

    intArrayArrayProp?: int [] []
//@[04:033) |   |   ├─ObjectTypePropertySyntax
//@[04:021) |   |   | ├─IdentifierSyntax
//@[04:021) |   |   | | └─Token(Identifier) |intArrayArrayProp|
//@[21:022) |   |   | ├─Token(Question) |?|
//@[22:023) |   |   | ├─Token(Colon) |:|
//@[24:033) |   |   | └─ArrayTypeSyntax
//@[24:030) |   |   |   ├─ArrayTypeMemberSyntax
//@[24:030) |   |   |   | └─ArrayTypeSyntax
//@[24:027) |   |   |   |   ├─ArrayTypeMemberSyntax
//@[24:027) |   |   |   |   | └─SimpleTypeSyntax
//@[24:027) |   |   |   |   |   └─Token(Identifier) |int|
//@[28:029) |   |   |   |   ├─Token(LeftSquare) |[|
//@[29:030) |   |   |   |   └─Token(RightSquare) |]|
//@[31:032) |   |   |   ├─Token(LeftSquare) |[|
//@[32:033) |   |   |   └─Token(RightSquare) |]|
//@[33:034) |   |   ├─Token(NewLine) |\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\n\n|

  typeRefProp: bar
//@[02:018) |   ├─ObjectTypePropertySyntax
//@[02:013) |   | ├─IdentifierSyntax
//@[02:013) |   | | └─Token(Identifier) |typeRefProp|
//@[13:014) |   | ├─Token(Colon) |:|
//@[15:018) |   | └─TypeAccessSyntax
//@[15:018) |   |   └─IdentifierSyntax
//@[15:018) |   |     └─Token(Identifier) |bar|
//@[18:020) |   ├─Token(NewLine) |\n\n|

  literalProp: 'literal'
//@[02:024) |   ├─ObjectTypePropertySyntax
//@[02:013) |   | ├─IdentifierSyntax
//@[02:013) |   | | └─Token(Identifier) |literalProp|
//@[13:014) |   | ├─Token(Colon) |:|
//@[15:024) |   | └─StringSyntax
//@[15:024) |   |   └─Token(StringComplete) |'literal'|
//@[24:026) |   ├─Token(NewLine) |\n\n|

  recursion?: foo
//@[02:017) |   ├─ObjectTypePropertySyntax
//@[02:011) |   | ├─IdentifierSyntax
//@[02:011) |   | | └─Token(Identifier) |recursion|
//@[11:012) |   | ├─Token(Question) |?|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:017) |   | └─TypeAccessSyntax
//@[14:017) |   |   └─IdentifierSyntax
//@[14:017) |   |     └─Token(Identifier) |foo|
//@[17:018) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

@minLength(3)
//@[00:163) ├─TypeDeclarationSyntax
//@[00:013) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:013) | | └─FunctionCallSyntax
//@[01:010) | |   ├─IdentifierSyntax
//@[01:010) | |   | └─Token(Identifier) |minLength|
//@[10:011) | |   ├─Token(LeftParen) |(|
//@[11:012) | |   ├─FunctionArgumentSyntax
//@[11:012) | |   | └─IntegerLiteralSyntax
//@[11:012) | |   |   └─Token(Integer) |3|
//@[12:013) | |   └─Token(RightParen) |)|
//@[13:014) | ├─Token(NewLine) |\n|
@description('An array of array of arrays of arrays of ints')
//@[00:061) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:061) | | └─FunctionCallSyntax
//@[01:012) | |   ├─IdentifierSyntax
//@[01:012) | |   | └─Token(Identifier) |description|
//@[12:013) | |   ├─Token(LeftParen) |(|
//@[13:060) | |   ├─FunctionArgumentSyntax
//@[13:060) | |   | └─StringSyntax
//@[13:060) | |   |   └─Token(StringComplete) |'An array of array of arrays of arrays of ints'|
//@[60:061) | |   └─Token(RightParen) |)|
//@[61:062) | ├─Token(NewLine) |\n|
@metadata({
//@[00:064) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:064) | | └─FunctionCallSyntax
//@[01:009) | |   ├─IdentifierSyntax
//@[01:009) | |   | └─Token(Identifier) |metadata|
//@[09:010) | |   ├─Token(LeftParen) |(|
//@[10:063) | |   ├─FunctionArgumentSyntax
//@[10:063) | |   | └─ObjectSyntax
//@[10:011) | |   |   ├─Token(LeftBrace) |{|
//@[11:012) | |   |   ├─Token(NewLine) |\n|
  examples: [
//@[02:049) | |   |   ├─ObjectPropertySyntax
//@[02:010) | |   |   | ├─IdentifierSyntax
//@[02:010) | |   |   | | └─Token(Identifier) |examples|
//@[10:011) | |   |   | ├─Token(Colon) |:|
//@[12:049) | |   |   | └─ArraySyntax
//@[12:013) | |   |   |   ├─Token(LeftSquare) |[|
//@[13:014) | |   |   |   ├─Token(NewLine) |\n|
    [[[[1]]], [[[2]]], [[[3]]]]
//@[04:031) | |   |   |   ├─ArrayItemSyntax
//@[04:031) | |   |   |   | └─ArraySyntax
//@[04:005) | |   |   |   |   ├─Token(LeftSquare) |[|
//@[05:012) | |   |   |   |   ├─ArrayItemSyntax
//@[05:012) | |   |   |   |   | └─ArraySyntax
//@[05:006) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[06:011) | |   |   |   |   |   ├─ArrayItemSyntax
//@[06:011) | |   |   |   |   |   | └─ArraySyntax
//@[06:007) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[07:010) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[07:010) | |   |   |   |   |   |   | └─ArraySyntax
//@[07:008) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[08:009) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[08:009) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[08:009) | |   |   |   |   |   |   |   |   └─Token(Integer) |1|
//@[09:010) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[10:011) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[11:012) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[12:013) | |   |   |   |   ├─Token(Comma) |,|
//@[14:021) | |   |   |   |   ├─ArrayItemSyntax
//@[14:021) | |   |   |   |   | └─ArraySyntax
//@[14:015) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[15:020) | |   |   |   |   |   ├─ArrayItemSyntax
//@[15:020) | |   |   |   |   |   | └─ArraySyntax
//@[15:016) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[16:019) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[16:019) | |   |   |   |   |   |   | └─ArraySyntax
//@[16:017) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[17:018) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[17:018) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[17:018) | |   |   |   |   |   |   |   |   └─Token(Integer) |2|
//@[18:019) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[19:020) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[20:021) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[21:022) | |   |   |   |   ├─Token(Comma) |,|
//@[23:030) | |   |   |   |   ├─ArrayItemSyntax
//@[23:030) | |   |   |   |   | └─ArraySyntax
//@[23:024) | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[24:029) | |   |   |   |   |   ├─ArrayItemSyntax
//@[24:029) | |   |   |   |   |   | └─ArraySyntax
//@[24:025) | |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[25:028) | |   |   |   |   |   |   ├─ArrayItemSyntax
//@[25:028) | |   |   |   |   |   |   | └─ArraySyntax
//@[25:026) | |   |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[26:027) | |   |   |   |   |   |   |   ├─ArrayItemSyntax
//@[26:027) | |   |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[26:027) | |   |   |   |   |   |   |   |   └─Token(Integer) |3|
//@[27:028) | |   |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[28:029) | |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[29:030) | |   |   |   |   |   └─Token(RightSquare) |]|
//@[30:031) | |   |   |   |   └─Token(RightSquare) |]|
//@[31:032) | |   |   |   ├─Token(NewLine) |\n|
  ]
//@[02:003) | |   |   |   └─Token(RightSquare) |]|
//@[03:004) | |   |   ├─Token(NewLine) |\n|
})
//@[00:001) | |   |   └─Token(RightBrace) |}|
//@[01:002) | |   └─Token(RightParen) |)|
//@[02:003) | ├─Token(NewLine) |\n|
type bar = int[][][][]
//@[00:004) | ├─Token(Identifier) |type|
//@[05:008) | ├─IdentifierSyntax
//@[05:008) | | └─Token(Identifier) |bar|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:022) | └─ArrayTypeSyntax
//@[11:020) |   ├─ArrayTypeMemberSyntax
//@[11:020) |   | └─ArrayTypeSyntax
//@[11:018) |   |   ├─ArrayTypeMemberSyntax
//@[11:018) |   |   | └─ArrayTypeSyntax
//@[11:016) |   |   |   ├─ArrayTypeMemberSyntax
//@[11:016) |   |   |   | └─ArrayTypeSyntax
//@[11:014) |   |   |   |   ├─ArrayTypeMemberSyntax
//@[11:014) |   |   |   |   | └─SimpleTypeSyntax
//@[11:014) |   |   |   |   |   └─Token(Identifier) |int|
//@[14:015) |   |   |   |   ├─Token(LeftSquare) |[|
//@[15:016) |   |   |   |   └─Token(RightSquare) |]|
//@[16:017) |   |   |   ├─Token(LeftSquare) |[|
//@[17:018) |   |   |   └─Token(RightSquare) |]|
//@[18:019) |   |   ├─Token(LeftSquare) |[|
//@[19:020) |   |   └─Token(RightSquare) |]|
//@[20:021) |   ├─Token(LeftSquare) |[|
//@[21:022) |   └─Token(RightSquare) |]|
//@[22:024) ├─Token(NewLine) |\n\n|

type aUnion = 'snap'|'crackle'|'pop'
//@[00:036) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |aUnion|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:036) | └─UnionTypeSyntax
//@[14:020) |   ├─UnionTypeMemberSyntax
//@[14:020) |   | └─StringSyntax
//@[14:020) |   |   └─Token(StringComplete) |'snap'|
//@[20:021) |   ├─Token(Pipe) |||
//@[21:030) |   ├─UnionTypeMemberSyntax
//@[21:030) |   | └─StringSyntax
//@[21:030) |   |   └─Token(StringComplete) |'crackle'|
//@[30:031) |   ├─Token(Pipe) |||
//@[31:036) |   └─UnionTypeMemberSyntax
//@[31:036) |     └─StringSyntax
//@[31:036) |       └─Token(StringComplete) |'pop'|
//@[36:038) ├─Token(NewLine) |\n\n|

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[00:047) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:018) | ├─IdentifierSyntax
//@[05:018) | | └─Token(Identifier) |expandedUnion|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:047) | └─UnionTypeSyntax
//@[21:027) |   ├─UnionTypeMemberSyntax
//@[21:027) |   | └─TypeAccessSyntax
//@[21:027) |   |   └─IdentifierSyntax
//@[21:027) |   |     └─Token(Identifier) |aUnion|
//@[27:028) |   ├─Token(Pipe) |||
//@[28:034) |   ├─UnionTypeMemberSyntax
//@[28:034) |   | └─StringSyntax
//@[28:034) |   |   └─Token(StringComplete) |'fizz'|
//@[34:035) |   ├─Token(Pipe) |||
//@[35:041) |   ├─UnionTypeMemberSyntax
//@[35:041) |   | └─StringSyntax
//@[35:041) |   |   └─Token(StringComplete) |'buzz'|
//@[41:042) |   ├─Token(Pipe) |||
//@[42:047) |   └─UnionTypeMemberSyntax
//@[42:047) |     └─StringSyntax
//@[42:047) |       └─Token(StringComplete) |'pop'|
//@[47:049) ├─Token(NewLine) |\n\n|

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[00:090) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:015) | ├─IdentifierSyntax
//@[05:015) | | └─Token(Identifier) |mixedArray|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:090) | └─ArrayTypeSyntax
//@[18:088) |   ├─ArrayTypeMemberSyntax
//@[18:088) |   | └─ParenthesizedExpressionSyntax
//@[18:019) |   |   ├─Token(LeftParen) |(|
//@[19:087) |   |   ├─UnionTypeSyntax
//@[19:030) |   |   | ├─UnionTypeMemberSyntax
//@[19:030) |   |   | | └─StringSyntax
//@[19:030) |   |   | |   └─Token(StringComplete) |'heffalump'|
//@[30:031) |   |   | ├─Token(Pipe) |||
//@[31:039) |   |   | ├─UnionTypeMemberSyntax
//@[31:039) |   |   | | └─StringSyntax
//@[31:039) |   |   | |   └─Token(StringComplete) |'woozle'|
//@[39:040) |   |   | ├─Token(Pipe) |||
//@[40:064) |   |   | ├─UnionTypeMemberSyntax
//@[40:064) |   |   | | └─ObjectTypeSyntax
//@[40:041) |   |   | |   ├─Token(LeftBrace) |{|
//@[42:052) |   |   | |   ├─ObjectTypePropertySyntax
//@[42:047) |   |   | |   | ├─IdentifierSyntax
//@[42:047) |   |   | |   | | └─Token(Identifier) |shape|
//@[47:048) |   |   | |   | ├─Token(Colon) |:|
//@[49:052) |   |   | |   | └─StringSyntax
//@[49:052) |   |   | |   |   └─Token(StringComplete) |'*'|
//@[52:053) |   |   | |   ├─Token(Comma) |,|
//@[54:063) |   |   | |   ├─ObjectTypePropertySyntax
//@[54:058) |   |   | |   | ├─IdentifierSyntax
//@[54:058) |   |   | |   | | └─Token(Identifier) |size|
//@[58:059) |   |   | |   | ├─Token(Colon) |:|
//@[60:063) |   |   | |   | └─StringSyntax
//@[60:063) |   |   | |   |   └─Token(StringComplete) |'*'|
//@[63:064) |   |   | |   └─Token(RightBrace) |}|
//@[64:065) |   |   | ├─Token(Pipe) |||
//@[65:067) |   |   | ├─UnionTypeMemberSyntax
//@[65:067) |   |   | | └─IntegerLiteralSyntax
//@[65:067) |   |   | |   └─Token(Integer) |10|
//@[67:068) |   |   | ├─Token(Pipe) |||
//@[68:071) |   |   | ├─UnionTypeMemberSyntax
//@[68:071) |   |   | | └─UnaryOperationSyntax
//@[68:069) |   |   | |   ├─Token(Minus) |-|
//@[69:071) |   |   | |   └─IntegerLiteralSyntax
//@[69:071) |   |   | |     └─Token(Integer) |10|
//@[71:072) |   |   | ├─Token(Pipe) |||
//@[72:076) |   |   | ├─UnionTypeMemberSyntax
//@[72:076) |   |   | | └─BooleanLiteralSyntax
//@[72:076) |   |   | |   └─Token(TrueKeyword) |true|
//@[76:077) |   |   | ├─Token(Pipe) |||
//@[77:082) |   |   | ├─UnionTypeMemberSyntax
//@[77:082) |   |   | | └─UnaryOperationSyntax
//@[77:078) |   |   | |   ├─Token(Exclamation) |!|
//@[78:082) |   |   | |   └─BooleanLiteralSyntax
//@[78:082) |   |   | |     └─Token(TrueKeyword) |true|
//@[82:083) |   |   | ├─Token(Pipe) |||
//@[83:087) |   |   | └─UnionTypeMemberSyntax
//@[83:087) |   |   |   └─NullLiteralSyntax
//@[83:087) |   |   |     └─Token(NullKeyword) |null|
//@[87:088) |   |   └─Token(RightParen) |)|
//@[88:089) |   ├─Token(LeftSquare) |[|
//@[89:090) |   └─Token(RightSquare) |]|
//@[90:092) ├─Token(NewLine) |\n\n|

type String = string
//@[00:020) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |String|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:020) | └─SimpleTypeSyntax
//@[14:020) |   └─Token(Identifier) |string|
//@[20:022) ├─Token(NewLine) |\n\n|

param inlineObjectParam {
//@[00:123) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:023) | ├─IdentifierSyntax
//@[06:023) | | └─Token(Identifier) |inlineObjectParam|
//@[24:080) | ├─ObjectTypeSyntax
//@[24:025) | | ├─Token(LeftBrace) |{|
//@[25:026) | | ├─Token(NewLine) |\n|
  foo: string
//@[02:013) | | ├─ObjectTypePropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |foo|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:013) | | | └─SimpleTypeSyntax
//@[07:013) | | |   └─Token(Identifier) |string|
//@[13:014) | | ├─Token(NewLine) |\n|
  bar: 100|200|300|400|500
//@[02:026) | | ├─ObjectTypePropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |bar|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:026) | | | └─UnionTypeSyntax
//@[07:010) | | |   ├─UnionTypeMemberSyntax
//@[07:010) | | |   | └─IntegerLiteralSyntax
//@[07:010) | | |   |   └─Token(Integer) |100|
//@[10:011) | | |   ├─Token(Pipe) |||
//@[11:014) | | |   ├─UnionTypeMemberSyntax
//@[11:014) | | |   | └─IntegerLiteralSyntax
//@[11:014) | | |   |   └─Token(Integer) |200|
//@[14:015) | | |   ├─Token(Pipe) |||
//@[15:018) | | |   ├─UnionTypeMemberSyntax
//@[15:018) | | |   | └─IntegerLiteralSyntax
//@[15:018) | | |   |   └─Token(Integer) |300|
//@[18:019) | | |   ├─Token(Pipe) |||
//@[19:022) | | |   ├─UnionTypeMemberSyntax
//@[19:022) | | |   | └─IntegerLiteralSyntax
//@[19:022) | | |   |   └─Token(Integer) |400|
//@[22:023) | | |   ├─Token(Pipe) |||
//@[23:026) | | |   └─UnionTypeMemberSyntax
//@[23:026) | | |     └─IntegerLiteralSyntax
//@[23:026) | | |       └─Token(Integer) |500|
//@[26:027) | | ├─Token(NewLine) |\n|
  baz: bool
//@[02:011) | | ├─ObjectTypePropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |baz|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:011) | | | └─SimpleTypeSyntax
//@[07:011) | | |   └─Token(Identifier) |bool|
//@[11:012) | | ├─Token(NewLine) |\n|
} = {
//@[00:001) | | └─Token(RightBrace) |}|
//@[02:044) | └─ParameterDefaultValueSyntax
//@[02:003) |   ├─Token(Assignment) |=|
//@[04:044) |   └─ObjectSyntax
//@[04:005) |     ├─Token(LeftBrace) |{|
//@[05:006) |     ├─Token(NewLine) |\n|
  foo: 'foo'
//@[02:012) |     ├─ObjectPropertySyntax
//@[02:005) |     | ├─IdentifierSyntax
//@[02:005) |     | | └─Token(Identifier) |foo|
//@[05:006) |     | ├─Token(Colon) |:|
//@[07:012) |     | └─StringSyntax
//@[07:012) |     |   └─Token(StringComplete) |'foo'|
//@[12:013) |     ├─Token(NewLine) |\n|
  bar: 300
//@[02:010) |     ├─ObjectPropertySyntax
//@[02:005) |     | ├─IdentifierSyntax
//@[02:005) |     | | └─Token(Identifier) |bar|
//@[05:006) |     | ├─Token(Colon) |:|
//@[07:010) |     | └─IntegerLiteralSyntax
//@[07:010) |     |   └─Token(Integer) |300|
//@[10:011) |     ├─Token(NewLine) |\n|
  baz: false
//@[02:012) |     ├─ObjectPropertySyntax
//@[02:005) |     | ├─IdentifierSyntax
//@[02:005) |     | | └─Token(Identifier) |baz|
//@[05:006) |     | ├─Token(Colon) |:|
//@[07:012) |     | └─BooleanLiteralSyntax
//@[07:012) |     |   └─Token(FalseKeyword) |false|
//@[12:013) |     ├─Token(NewLine) |\n|
}
//@[00:001) |     └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[00:075) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:016) | ├─IdentifierSyntax
//@[06:016) | | └─Token(Identifier) |unionParam|
//@[17:054) | ├─UnionTypeSyntax
//@[17:035) | | ├─UnionTypeMemberSyntax
//@[17:035) | | | └─ObjectTypeSyntax
//@[17:018) | | |   ├─Token(LeftBrace) |{|
//@[18:034) | | |   ├─ObjectTypePropertySyntax
//@[18:026) | | |   | ├─IdentifierSyntax
//@[18:026) | | |   | | └─Token(Identifier) |property|
//@[26:027) | | |   | ├─Token(Colon) |:|
//@[28:034) | | |   | └─StringSyntax
//@[28:034) | | |   |   └─Token(StringComplete) |'ping'|
//@[34:035) | | |   └─Token(RightBrace) |}|
//@[35:036) | | ├─Token(Pipe) |||
//@[36:054) | | └─UnionTypeMemberSyntax
//@[36:054) | |   └─ObjectTypeSyntax
//@[36:037) | |     ├─Token(LeftBrace) |{|
//@[37:053) | |     ├─ObjectTypePropertySyntax
//@[37:045) | |     | ├─IdentifierSyntax
//@[37:045) | |     | | └─Token(Identifier) |property|
//@[45:046) | |     | ├─Token(Colon) |:|
//@[47:053) | |     | └─StringSyntax
//@[47:053) | |     |   └─Token(StringComplete) |'pong'|
//@[53:054) | |     └─Token(RightBrace) |}|
//@[55:075) | └─ParameterDefaultValueSyntax
//@[55:056) |   ├─Token(Assignment) |=|
//@[57:075) |   └─ObjectSyntax
//@[57:058) |     ├─Token(LeftBrace) |{|
//@[58:074) |     ├─ObjectPropertySyntax
//@[58:066) |     | ├─IdentifierSyntax
//@[58:066) |     | | └─Token(Identifier) |property|
//@[66:067) |     | ├─Token(Colon) |:|
//@[68:074) |     | └─StringSyntax
//@[68:074) |     |   └─Token(StringComplete) |'pong'|
//@[74:075) |     └─Token(RightBrace) |}|
//@[75:077) ├─Token(NewLine) |\n\n|

param paramUsingType mixedArray
//@[00:031) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:020) | ├─IdentifierSyntax
//@[06:020) | | └─Token(Identifier) |paramUsingType|
//@[21:031) | └─TypeAccessSyntax
//@[21:031) |   └─IdentifierSyntax
//@[21:031) |     └─Token(Identifier) |mixedArray|
//@[31:032) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
