type 44
//@[00:993) ProgramSyntax
//@[00:007) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:007) | ├─IdentifierSyntax
//@[05:007) | | └─SkippedTriviaSyntax
//@[05:007) | |   └─Token(Integer) |44|
//@[07:007) | ├─SkippedTriviaSyntax
//@[07:007) | └─SkippedTriviaSyntax
//@[07:009) ├─Token(NewLine) |\n\n|

type noAssignment
//@[00:017) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:017) | ├─IdentifierSyntax
//@[05:017) | | └─Token(Identifier) |noAssignment|
//@[17:017) | ├─SkippedTriviaSyntax
//@[17:017) | └─SkippedTriviaSyntax
//@[17:019) ├─Token(NewLine) |\n\n|

type incompleteAssignment =
//@[00:027) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:025) | ├─IdentifierSyntax
//@[05:025) | | └─Token(Identifier) |incompleteAssignment|
//@[26:027) | ├─Token(Assignment) |=|
//@[27:027) | └─SkippedTriviaSyntax
//@[27:029) ├─Token(NewLine) |\n\n|

@sealed()
//@[00:036) ├─TypeDeclarationSyntax
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |sealed|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:010) | ├─Token(NewLine) |\n|
type sealedString = string
//@[00:004) | ├─Token(Identifier) |type|
//@[05:017) | ├─IdentifierSyntax
//@[05:017) | | └─Token(Identifier) |sealedString|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:026) | └─SimpleTypeSyntax
//@[20:026) |   └─Token(Identifier) |string|
//@[26:028) ├─Token(NewLine) |\n\n|

type disallowedUnion = 'foo'|21
//@[00:031) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:020) | ├─IdentifierSyntax
//@[05:020) | | └─Token(Identifier) |disallowedUnion|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:031) | └─UnionTypeSyntax
//@[23:028) |   ├─UnionTypeMemberSyntax
//@[23:028) |   | └─StringSyntax
//@[23:028) |   |   └─Token(StringComplete) |'foo'|
//@[28:029) |   ├─Token(Pipe) |||
//@[29:031) |   └─UnionTypeMemberSyntax
//@[29:031) |     └─IntegerLiteralSyntax
//@[29:031) |       └─Token(Integer) |21|
//@[31:033) ├─Token(NewLine) |\n\n|

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[00:048) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:028) | ├─IdentifierSyntax
//@[05:028) | | └─Token(Identifier) |validStringLiteralUnion|
//@[29:030) | ├─Token(Assignment) |=|
//@[31:048) | └─UnionTypeSyntax
//@[31:036) |   ├─UnionTypeMemberSyntax
//@[31:036) |   | └─StringSyntax
//@[31:036) |   |   └─Token(StringComplete) |'foo'|
//@[36:037) |   ├─Token(Pipe) |||
//@[37:042) |   ├─UnionTypeMemberSyntax
//@[37:042) |   | └─StringSyntax
//@[37:042) |   |   └─Token(StringComplete) |'bar'|
//@[42:043) |   ├─Token(Pipe) |||
//@[43:048) |   └─UnionTypeMemberSyntax
//@[43:048) |     └─StringSyntax
//@[43:048) |       └─Token(StringComplete) |'baz'|
//@[48:050) ├─Token(NewLine) |\n\n|

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[00:059) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:030) | ├─IdentifierSyntax
//@[05:030) | | └─Token(Identifier) |validUnionInvalidAddition|
//@[31:032) | ├─Token(Assignment) |=|
//@[33:059) | └─UnionTypeSyntax
//@[33:056) |   ├─UnionTypeMemberSyntax
//@[33:056) |   | └─TypeAccessSyntax
//@[33:056) |   |   └─IdentifierSyntax
//@[33:056) |   |     └─Token(Identifier) |validStringLiteralUnion|
//@[56:057) |   ├─Token(Pipe) |||
//@[57:059) |   └─UnionTypeMemberSyntax
//@[57:059) |     └─IntegerLiteralSyntax
//@[57:059) |       └─Token(Integer) |10|
//@[59:061) ├─Token(NewLine) |\n\n|

type invalidUnionInvalidAddition = disallowedUnion|true
//@[00:055) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:032) | ├─IdentifierSyntax
//@[05:032) | | └─Token(Identifier) |invalidUnionInvalidAddition|
//@[33:034) | ├─Token(Assignment) |=|
//@[35:055) | └─UnionTypeSyntax
//@[35:050) |   ├─UnionTypeMemberSyntax
//@[35:050) |   | └─TypeAccessSyntax
//@[35:050) |   |   └─IdentifierSyntax
//@[35:050) |   |     └─Token(Identifier) |disallowedUnion|
//@[50:051) |   ├─Token(Pipe) |||
//@[51:055) |   └─UnionTypeMemberSyntax
//@[51:055) |     └─BooleanLiteralSyntax
//@[51:055) |       └─Token(TrueKeyword) |true|
//@[55:057) ├─Token(NewLine) |\n\n|

type nullLiteral = null
//@[00:023) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:016) | ├─IdentifierSyntax
//@[05:016) | | └─Token(Identifier) |nullLiteral|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:023) | └─NullLiteralSyntax
//@[19:023) |   └─Token(NullKeyword) |null|
//@[23:025) ├─Token(NewLine) |\n\n|

type unionOfNulls = null|null
//@[00:029) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:017) | ├─IdentifierSyntax
//@[05:017) | | └─Token(Identifier) |unionOfNulls|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:029) | └─UnionTypeSyntax
//@[20:024) |   ├─UnionTypeMemberSyntax
//@[20:024) |   | └─NullLiteralSyntax
//@[20:024) |   |   └─Token(NullKeyword) |null|
//@[24:025) |   ├─Token(Pipe) |||
//@[25:029) |   └─UnionTypeMemberSyntax
//@[25:029) |     └─NullLiteralSyntax
//@[25:029) |       └─Token(NullKeyword) |null|
//@[29:031) ├─Token(NewLine) |\n\n|

@minLength(3)
//@[00:045) ├─TypeDeclarationSyntax
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
type lengthConstrainedInt = int
//@[00:004) | ├─Token(Identifier) |type|
//@[05:025) | ├─IdentifierSyntax
//@[05:025) | | └─Token(Identifier) |lengthConstrainedInt|
//@[26:027) | ├─Token(Assignment) |=|
//@[28:031) | └─SimpleTypeSyntax
//@[28:031) |   └─Token(Identifier) |int|
//@[31:033) ├─Token(NewLine) |\n\n|

@minValue(3)
//@[00:049) ├─TypeDeclarationSyntax
//@[00:012) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:012) | | └─FunctionCallSyntax
//@[01:009) | |   ├─IdentifierSyntax
//@[01:009) | |   | └─Token(Identifier) |minValue|
//@[09:010) | |   ├─Token(LeftParen) |(|
//@[10:011) | |   ├─FunctionArgumentSyntax
//@[10:011) | |   | └─IntegerLiteralSyntax
//@[10:011) | |   |   └─Token(Integer) |3|
//@[11:012) | |   └─Token(RightParen) |)|
//@[12:013) | ├─Token(NewLine) |\n|
type valueConstrainedString = string
//@[00:004) | ├─Token(Identifier) |type|
//@[05:027) | ├─IdentifierSyntax
//@[05:027) | | └─Token(Identifier) |valueConstrainedString|
//@[28:029) | ├─Token(Assignment) |=|
//@[30:036) | └─SimpleTypeSyntax
//@[30:036) |   └─Token(Identifier) |string|
//@[36:038) ├─Token(NewLine) |\n\n|

type tautology = tautology
//@[00:026) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |tautology|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:026) | └─TypeAccessSyntax
//@[17:026) |   └─IdentifierSyntax
//@[17:026) |     └─Token(Identifier) |tautology|
//@[26:028) ├─Token(NewLine) |\n\n|

type tautologicalUnion = tautologicalUnion|'foo'
//@[00:048) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:022) | ├─IdentifierSyntax
//@[05:022) | | └─Token(Identifier) |tautologicalUnion|
//@[23:024) | ├─Token(Assignment) |=|
//@[25:048) | └─UnionTypeSyntax
//@[25:042) |   ├─UnionTypeMemberSyntax
//@[25:042) |   | └─TypeAccessSyntax
//@[25:042) |   |   └─IdentifierSyntax
//@[25:042) |   |     └─Token(Identifier) |tautologicalUnion|
//@[42:043) |   ├─Token(Pipe) |||
//@[43:048) |   └─UnionTypeMemberSyntax
//@[43:048) |     └─StringSyntax
//@[43:048) |       └─Token(StringComplete) |'foo'|
//@[48:050) ├─Token(NewLine) |\n\n|

type tautologicalArray = tautologicalArray[]
//@[00:044) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:022) | ├─IdentifierSyntax
//@[05:022) | | └─Token(Identifier) |tautologicalArray|
//@[23:024) | ├─Token(Assignment) |=|
//@[25:044) | └─ArrayTypeSyntax
//@[25:042) |   ├─TypeAccessSyntax
//@[25:042) |   | └─IdentifierSyntax
//@[25:042) |   |   └─Token(Identifier) |tautologicalArray|
//@[42:043) |   ├─Token(LeftSquare) |[|
//@[43:044) |   └─Token(RightSquare) |]|
//@[44:046) ├─Token(NewLine) |\n\n|

type directCycleStart = directCycleReturn
//@[00:041) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:021) | ├─IdentifierSyntax
//@[05:021) | | └─Token(Identifier) |directCycleStart|
//@[22:023) | ├─Token(Assignment) |=|
//@[24:041) | └─TypeAccessSyntax
//@[24:041) |   └─IdentifierSyntax
//@[24:041) |     └─Token(Identifier) |directCycleReturn|
//@[41:043) ├─Token(NewLine) |\n\n|

type directCycleReturn = directCycleStart
//@[00:041) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:022) | ├─IdentifierSyntax
//@[05:022) | | └─Token(Identifier) |directCycleReturn|
//@[23:024) | ├─Token(Assignment) |=|
//@[25:041) | └─TypeAccessSyntax
//@[25:041) |   └─IdentifierSyntax
//@[25:041) |     └─Token(Identifier) |directCycleStart|
//@[41:043) ├─Token(NewLine) |\n\n|

type cycleRoot = connector
//@[00:026) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |cycleRoot|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:026) | └─TypeAccessSyntax
//@[17:026) |   └─IdentifierSyntax
//@[17:026) |     └─Token(Identifier) |connector|
//@[26:028) ├─Token(NewLine) |\n\n|

type connector = cycleBack
//@[00:026) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |connector|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:026) | └─TypeAccessSyntax
//@[17:026) |   └─IdentifierSyntax
//@[17:026) |     └─Token(Identifier) |cycleBack|
//@[26:028) ├─Token(NewLine) |\n\n|

type cycleBack = cycleRoot
//@[00:026) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |cycleBack|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:026) | └─TypeAccessSyntax
//@[17:026) |   └─IdentifierSyntax
//@[17:026) |     └─Token(Identifier) |cycleRoot|
//@[26:028) ├─Token(NewLine) |\n\n|

type objectWithInvalidPropertyDecorators = {
//@[00:168) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:040) | ├─IdentifierSyntax
//@[05:040) | | └─Token(Identifier) |objectWithInvalidPropertyDecorators|
//@[41:042) | ├─Token(Assignment) |=|
//@[43:168) | └─ObjectTypeSyntax
//@[43:044) |   ├─Token(LeftBrace) |{|
//@[44:045) |   ├─Token(NewLine) |\n|
  @sealed()
//@[02:029) |   ├─ObjectTypePropertySyntax
//@[02:011) |   | ├─DecoratorSyntax
//@[02:003) |   | | ├─Token(At) |@|
//@[03:011) |   | | └─FunctionCallSyntax
//@[03:009) |   | |   ├─IdentifierSyntax
//@[03:009) |   | |   | └─Token(Identifier) |sealed|
//@[09:010) |   | |   ├─Token(LeftParen) |(|
//@[10:011) |   | |   └─Token(RightParen) |)|
//@[11:012) |   | ├─Token(NewLine) |\n|
  fooProp: string
//@[02:009) |   | ├─IdentifierSyntax
//@[02:009) |   | | └─Token(Identifier) |fooProp|
//@[09:010) |   | ├─Token(Colon) |:|
//@[11:017) |   | └─SimpleTypeSyntax
//@[11:017) |   |   └─Token(Identifier) |string|
//@[17:019) |   ├─Token(NewLine) |\n\n|

  @secure()
//@[02:029) |   ├─ObjectTypePropertySyntax
//@[02:011) |   | ├─DecoratorSyntax
//@[02:003) |   | | ├─Token(At) |@|
//@[03:011) |   | | └─FunctionCallSyntax
//@[03:009) |   | |   ├─IdentifierSyntax
//@[03:009) |   | |   | └─Token(Identifier) |secure|
//@[09:010) |   | |   ├─Token(LeftParen) |(|
//@[10:011) |   | |   └─Token(RightParen) |)|
//@[11:012) |   | ├─Token(NewLine) |\n|
  barProp: string
//@[02:009) |   | ├─IdentifierSyntax
//@[02:009) |   | | └─Token(Identifier) |barProp|
//@[09:010) |   | ├─Token(Colon) |:|
//@[11:017) |   | └─SimpleTypeSyntax
//@[11:017) |   |   └─Token(Identifier) |string|
//@[17:019) |   ├─Token(NewLine) |\n\n|

  @allowed(['snap', 'crackle', 'pop'])
//@[02:059) |   ├─ObjectTypePropertySyntax
//@[02:038) |   | ├─DecoratorSyntax
//@[02:003) |   | | ├─Token(At) |@|
//@[03:038) |   | | └─FunctionCallSyntax
//@[03:010) |   | |   ├─IdentifierSyntax
//@[03:010) |   | |   | └─Token(Identifier) |allowed|
//@[10:011) |   | |   ├─Token(LeftParen) |(|
//@[11:037) |   | |   ├─FunctionArgumentSyntax
//@[11:037) |   | |   | └─ArraySyntax
//@[11:012) |   | |   |   ├─Token(LeftSquare) |[|
//@[12:018) |   | |   |   ├─ArrayItemSyntax
//@[12:018) |   | |   |   | └─StringSyntax
//@[12:018) |   | |   |   |   └─Token(StringComplete) |'snap'|
//@[18:019) |   | |   |   ├─Token(Comma) |,|
//@[20:029) |   | |   |   ├─ArrayItemSyntax
//@[20:029) |   | |   |   | └─StringSyntax
//@[20:029) |   | |   |   |   └─Token(StringComplete) |'crackle'|
//@[29:030) |   | |   |   ├─Token(Comma) |,|
//@[31:036) |   | |   |   ├─ArrayItemSyntax
//@[31:036) |   | |   |   | └─StringSyntax
//@[31:036) |   | |   |   |   └─Token(StringComplete) |'pop'|
//@[36:037) |   | |   |   └─Token(RightSquare) |]|
//@[37:038) |   | |   └─Token(RightParen) |)|
//@[38:039) |   | ├─Token(NewLine) |\n|
  krispyProp: string
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |krispyProp|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:020) |   | └─SimpleTypeSyntax
//@[14:020) |   |   └─Token(Identifier) |string|
//@[20:021) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

@sealed()
//@[00:040) ├─ParameterDeclarationSyntax
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |sealed|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:010) | ├─Token(NewLine) |\n|
param sealedStringParam string
//@[00:005) | ├─Token(Identifier) |param|
//@[06:023) | ├─IdentifierSyntax
//@[06:023) | | └─Token(Identifier) |sealedStringParam|
//@[24:030) | └─SimpleTypeSyntax
//@[24:030) |   └─Token(Identifier) |string|
//@[30:032) ├─Token(NewLine) |\n\n|

param disallowedUnionParam 'foo'|-99
//@[00:036) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:026) | ├─IdentifierSyntax
//@[06:026) | | └─Token(Identifier) |disallowedUnionParam|
//@[27:036) | └─UnionTypeSyntax
//@[27:032) |   ├─UnionTypeMemberSyntax
//@[27:032) |   | └─StringSyntax
//@[27:032) |   |   └─Token(StringComplete) |'foo'|
//@[32:033) |   ├─Token(Pipe) |||
//@[33:036) |   └─UnionTypeMemberSyntax
//@[33:036) |     └─UnaryOperationSyntax
//@[33:034) |       ├─Token(Minus) |-|
//@[34:036) |       └─IntegerLiteralSyntax
//@[34:036) |         └─Token(Integer) |99|
//@[36:037) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
