type 44
//@[00:3039) ProgramSyntax
//@[00:0007) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0007) | ├─IdentifierSyntax
//@[05:0007) | | └─SkippedTriviaSyntax
//@[05:0007) | |   └─Token(Integer) |44|
//@[07:0007) | ├─SkippedTriviaSyntax
//@[07:0007) | └─SkippedTriviaSyntax
//@[07:0009) ├─Token(NewLine) |\n\n|

type noAssignment
//@[00:0017) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0017) | ├─IdentifierSyntax
//@[05:0017) | | └─Token(Identifier) |noAssignment|
//@[17:0017) | ├─SkippedTriviaSyntax
//@[17:0017) | └─SkippedTriviaSyntax
//@[17:0019) ├─Token(NewLine) |\n\n|

type incompleteAssignment =
//@[00:0027) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0025) | ├─IdentifierSyntax
//@[05:0025) | | └─Token(Identifier) |incompleteAssignment|
//@[26:0027) | ├─Token(Assignment) |=|
//@[27:0027) | └─SkippedTriviaSyntax
//@[27:0029) ├─Token(NewLine) |\n\n|

type resource = bool
//@[00:0020) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |resource|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0020) | └─VariableAccessSyntax
//@[16:0020) |   └─IdentifierSyntax
//@[16:0020) |     └─Token(Identifier) |bool|
//@[20:0022) ├─Token(NewLine) |\n\n|

@sealed()
//@[00:0036) ├─TypeDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |sealed|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0010) | ├─Token(NewLine) |\n|
type sealedString = string
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0017) | ├─IdentifierSyntax
//@[05:0017) | | └─Token(Identifier) |sealedString|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0026) | └─VariableAccessSyntax
//@[20:0026) |   └─IdentifierSyntax
//@[20:0026) |     └─Token(Identifier) |string|
//@[26:0028) ├─Token(NewLine) |\n\n|

@sealed()
//@[00:0048) ├─TypeDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |sealed|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0010) | ├─Token(NewLine) |\n|
type sealedDictionary = {
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0021) | ├─IdentifierSyntax
//@[05:0021) | | └─Token(Identifier) |sealedDictionary|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0038) | └─ObjectTypeSyntax
//@[24:0025) |   ├─Token(LeftBrace) |{|
//@[25:0026) |   ├─Token(NewLine) |\n|
	*: string
//@[01:0010) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[01:0002) |   | ├─Token(Asterisk) |*|
//@[02:0003) |   | ├─Token(Colon) |:|
//@[04:0010) |   | └─VariableAccessSyntax
//@[04:0010) |   |   └─IdentifierSyntax
//@[04:0010) |   |     └─Token(Identifier) |string|
//@[10:0011) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type disallowedUnion = 'foo'|21
//@[00:0031) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0020) | ├─IdentifierSyntax
//@[05:0020) | | └─Token(Identifier) |disallowedUnion|
//@[21:0022) | ├─Token(Assignment) |=|
//@[23:0031) | └─UnionTypeSyntax
//@[23:0028) |   ├─UnionTypeMemberSyntax
//@[23:0028) |   | └─StringSyntax
//@[23:0028) |   |   └─Token(StringComplete) |'foo'|
//@[28:0029) |   ├─Token(Pipe) |||
//@[29:0031) |   └─UnionTypeMemberSyntax
//@[29:0031) |     └─IntegerLiteralSyntax
//@[29:0031) |       └─Token(Integer) |21|
//@[31:0033) ├─Token(NewLine) |\n\n|

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[00:0048) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0028) | ├─IdentifierSyntax
//@[05:0028) | | └─Token(Identifier) |validStringLiteralUnion|
//@[29:0030) | ├─Token(Assignment) |=|
//@[31:0048) | └─UnionTypeSyntax
//@[31:0036) |   ├─UnionTypeMemberSyntax
//@[31:0036) |   | └─StringSyntax
//@[31:0036) |   |   └─Token(StringComplete) |'foo'|
//@[36:0037) |   ├─Token(Pipe) |||
//@[37:0042) |   ├─UnionTypeMemberSyntax
//@[37:0042) |   | └─StringSyntax
//@[37:0042) |   |   └─Token(StringComplete) |'bar'|
//@[42:0043) |   ├─Token(Pipe) |||
//@[43:0048) |   └─UnionTypeMemberSyntax
//@[43:0048) |     └─StringSyntax
//@[43:0048) |       └─Token(StringComplete) |'baz'|
//@[48:0050) ├─Token(NewLine) |\n\n|

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[00:0059) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0030) | ├─IdentifierSyntax
//@[05:0030) | | └─Token(Identifier) |validUnionInvalidAddition|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0059) | └─UnionTypeSyntax
//@[33:0056) |   ├─UnionTypeMemberSyntax
//@[33:0056) |   | └─VariableAccessSyntax
//@[33:0056) |   |   └─IdentifierSyntax
//@[33:0056) |   |     └─Token(Identifier) |validStringLiteralUnion|
//@[56:0057) |   ├─Token(Pipe) |||
//@[57:0059) |   └─UnionTypeMemberSyntax
//@[57:0059) |     └─IntegerLiteralSyntax
//@[57:0059) |       └─Token(Integer) |10|
//@[59:0061) ├─Token(NewLine) |\n\n|

type invalidUnionInvalidAddition = disallowedUnion|true
//@[00:0055) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0032) | ├─IdentifierSyntax
//@[05:0032) | | └─Token(Identifier) |invalidUnionInvalidAddition|
//@[33:0034) | ├─Token(Assignment) |=|
//@[35:0055) | └─UnionTypeSyntax
//@[35:0050) |   ├─UnionTypeMemberSyntax
//@[35:0050) |   | └─VariableAccessSyntax
//@[35:0050) |   |   └─IdentifierSyntax
//@[35:0050) |   |     └─Token(Identifier) |disallowedUnion|
//@[50:0051) |   ├─Token(Pipe) |||
//@[51:0055) |   └─UnionTypeMemberSyntax
//@[51:0055) |     └─BooleanLiteralSyntax
//@[51:0055) |       └─Token(TrueKeyword) |true|
//@[55:0057) ├─Token(NewLine) |\n\n|

type nullLiteral = null
//@[00:0023) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0016) | ├─IdentifierSyntax
//@[05:0016) | | └─Token(Identifier) |nullLiteral|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0023) | └─NullLiteralSyntax
//@[19:0023) |   └─Token(NullKeyword) |null|
//@[23:0025) ├─Token(NewLine) |\n\n|

type unionOfNulls = null|null
//@[00:0029) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0017) | ├─IdentifierSyntax
//@[05:0017) | | └─Token(Identifier) |unionOfNulls|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0029) | └─UnionTypeSyntax
//@[20:0024) |   ├─UnionTypeMemberSyntax
//@[20:0024) |   | └─NullLiteralSyntax
//@[20:0024) |   |   └─Token(NullKeyword) |null|
//@[24:0025) |   ├─Token(Pipe) |||
//@[25:0029) |   └─UnionTypeMemberSyntax
//@[25:0029) |     └─NullLiteralSyntax
//@[25:0029) |       └─Token(NullKeyword) |null|
//@[29:0031) ├─Token(NewLine) |\n\n|

@minLength(3)
//@[00:0045) ├─TypeDeclarationSyntax
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
type lengthConstrainedInt = int
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0025) | ├─IdentifierSyntax
//@[05:0025) | | └─Token(Identifier) |lengthConstrainedInt|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0031) | └─VariableAccessSyntax
//@[28:0031) |   └─IdentifierSyntax
//@[28:0031) |     └─Token(Identifier) |int|
//@[31:0033) ├─Token(NewLine) |\n\n|

@minValue(3)
//@[00:0049) ├─TypeDeclarationSyntax
//@[00:0012) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0012) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |minValue|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0011) | |   ├─FunctionArgumentSyntax
//@[10:0011) | |   | └─IntegerLiteralSyntax
//@[10:0011) | |   |   └─Token(Integer) |3|
//@[11:0012) | |   └─Token(RightParen) |)|
//@[12:0013) | ├─Token(NewLine) |\n|
type valueConstrainedString = string
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0027) | ├─IdentifierSyntax
//@[05:0027) | | └─Token(Identifier) |valueConstrainedString|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0036) | └─VariableAccessSyntax
//@[30:0036) |   └─IdentifierSyntax
//@[30:0036) |     └─Token(Identifier) |string|
//@[36:0038) ├─Token(NewLine) |\n\n|

type tautology = tautology
//@[00:0026) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |tautology|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0026) | └─VariableAccessSyntax
//@[17:0026) |   └─IdentifierSyntax
//@[17:0026) |     └─Token(Identifier) |tautology|
//@[26:0028) ├─Token(NewLine) |\n\n|

type tautologicalUnion = tautologicalUnion|'foo'
//@[00:0048) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0022) | ├─IdentifierSyntax
//@[05:0022) | | └─Token(Identifier) |tautologicalUnion|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0048) | └─UnionTypeSyntax
//@[25:0042) |   ├─UnionTypeMemberSyntax
//@[25:0042) |   | └─VariableAccessSyntax
//@[25:0042) |   |   └─IdentifierSyntax
//@[25:0042) |   |     └─Token(Identifier) |tautologicalUnion|
//@[42:0043) |   ├─Token(Pipe) |||
//@[43:0048) |   └─UnionTypeMemberSyntax
//@[43:0048) |     └─StringSyntax
//@[43:0048) |       └─Token(StringComplete) |'foo'|
//@[48:0050) ├─Token(NewLine) |\n\n|

type tautologicalArray = tautologicalArray[]
//@[00:0044) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0022) | ├─IdentifierSyntax
//@[05:0022) | | └─Token(Identifier) |tautologicalArray|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0044) | └─ArrayTypeSyntax
//@[25:0042) |   ├─ArrayTypeMemberSyntax
//@[25:0042) |   | └─VariableAccessSyntax
//@[25:0042) |   |   └─IdentifierSyntax
//@[25:0042) |   |     └─Token(Identifier) |tautologicalArray|
//@[42:0043) |   ├─Token(LeftSquare) |[|
//@[43:0044) |   └─Token(RightSquare) |]|
//@[44:0046) ├─Token(NewLine) |\n\n|

type directCycleStart = directCycleReturn
//@[00:0041) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0021) | ├─IdentifierSyntax
//@[05:0021) | | └─Token(Identifier) |directCycleStart|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0041) | └─VariableAccessSyntax
//@[24:0041) |   └─IdentifierSyntax
//@[24:0041) |     └─Token(Identifier) |directCycleReturn|
//@[41:0043) ├─Token(NewLine) |\n\n|

type directCycleReturn = directCycleStart
//@[00:0041) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0022) | ├─IdentifierSyntax
//@[05:0022) | | └─Token(Identifier) |directCycleReturn|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0041) | └─VariableAccessSyntax
//@[25:0041) |   └─IdentifierSyntax
//@[25:0041) |     └─Token(Identifier) |directCycleStart|
//@[41:0043) ├─Token(NewLine) |\n\n|

type cycleRoot = connector
//@[00:0026) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |cycleRoot|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0026) | └─VariableAccessSyntax
//@[17:0026) |   └─IdentifierSyntax
//@[17:0026) |     └─Token(Identifier) |connector|
//@[26:0028) ├─Token(NewLine) |\n\n|

type connector = cycleBack
//@[00:0026) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |connector|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0026) | └─VariableAccessSyntax
//@[17:0026) |   └─IdentifierSyntax
//@[17:0026) |     └─Token(Identifier) |cycleBack|
//@[26:0028) ├─Token(NewLine) |\n\n|

type cycleBack = cycleRoot
//@[00:0026) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |cycleBack|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0026) | └─VariableAccessSyntax
//@[17:0026) |   └─IdentifierSyntax
//@[17:0026) |     └─Token(Identifier) |cycleRoot|
//@[26:0028) ├─Token(NewLine) |\n\n|

type objectWithInvalidPropertyDecorators = {
//@[00:0168) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0040) | ├─IdentifierSyntax
//@[05:0040) | | └─Token(Identifier) |objectWithInvalidPropertyDecorators|
//@[41:0042) | ├─Token(Assignment) |=|
//@[43:0168) | └─ObjectTypeSyntax
//@[43:0044) |   ├─Token(LeftBrace) |{|
//@[44:0045) |   ├─Token(NewLine) |\n|
  @sealed()
//@[02:0029) |   ├─ObjectTypePropertySyntax
//@[02:0011) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0011) |   | | └─FunctionCallSyntax
//@[03:0009) |   | |   ├─IdentifierSyntax
//@[03:0009) |   | |   | └─Token(Identifier) |sealed|
//@[09:0010) |   | |   ├─Token(LeftParen) |(|
//@[10:0011) |   | |   └─Token(RightParen) |)|
//@[11:0012) |   | ├─Token(NewLine) |\n|
  fooProp: string
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |fooProp|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0017) |   | └─VariableAccessSyntax
//@[11:0017) |   |   └─IdentifierSyntax
//@[11:0017) |   |     └─Token(Identifier) |string|
//@[17:0019) |   ├─Token(NewLine) |\n\n|

  @secure()
//@[02:0029) |   ├─ObjectTypePropertySyntax
//@[02:0011) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0011) |   | | └─FunctionCallSyntax
//@[03:0009) |   | |   ├─IdentifierSyntax
//@[03:0009) |   | |   | └─Token(Identifier) |secure|
//@[09:0010) |   | |   ├─Token(LeftParen) |(|
//@[10:0011) |   | |   └─Token(RightParen) |)|
//@[11:0012) |   | ├─Token(NewLine) |\n|
  barProp: string
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |barProp|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0017) |   | └─VariableAccessSyntax
//@[11:0017) |   |   └─IdentifierSyntax
//@[11:0017) |   |     └─Token(Identifier) |string|
//@[17:0019) |   ├─Token(NewLine) |\n\n|

  @allowed(['snap', 'crackle', 'pop'])
//@[02:0059) |   ├─ObjectTypePropertySyntax
//@[02:0038) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0038) |   | | └─FunctionCallSyntax
//@[03:0010) |   | |   ├─IdentifierSyntax
//@[03:0010) |   | |   | └─Token(Identifier) |allowed|
//@[10:0011) |   | |   ├─Token(LeftParen) |(|
//@[11:0037) |   | |   ├─FunctionArgumentSyntax
//@[11:0037) |   | |   | └─ArraySyntax
//@[11:0012) |   | |   |   ├─Token(LeftSquare) |[|
//@[12:0018) |   | |   |   ├─ArrayItemSyntax
//@[12:0018) |   | |   |   | └─StringSyntax
//@[12:0018) |   | |   |   |   └─Token(StringComplete) |'snap'|
//@[18:0019) |   | |   |   ├─Token(Comma) |,|
//@[20:0029) |   | |   |   ├─ArrayItemSyntax
//@[20:0029) |   | |   |   | └─StringSyntax
//@[20:0029) |   | |   |   |   └─Token(StringComplete) |'crackle'|
//@[29:0030) |   | |   |   ├─Token(Comma) |,|
//@[31:0036) |   | |   |   ├─ArrayItemSyntax
//@[31:0036) |   | |   |   | └─StringSyntax
//@[31:0036) |   | |   |   |   └─Token(StringComplete) |'pop'|
//@[36:0037) |   | |   |   └─Token(RightSquare) |]|
//@[37:0038) |   | |   └─Token(RightParen) |)|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  krispyProp: string
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |krispyProp|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0020) |   | └─VariableAccessSyntax
//@[14:0020) |   |   └─IdentifierSyntax
//@[14:0020) |   |     └─Token(Identifier) |string|
//@[20:0021) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type objectWithInvalidRecursion = {
//@[00:0092) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0031) | ├─IdentifierSyntax
//@[05:0031) | | └─Token(Identifier) |objectWithInvalidRecursion|
//@[32:0033) | ├─Token(Assignment) |=|
//@[34:0092) | └─ObjectTypeSyntax
//@[34:0035) |   ├─Token(LeftBrace) |{|
//@[35:0036) |   ├─Token(NewLine) |\n|
  requiredAndRecursiveProp: objectWithInvalidRecursion
//@[02:0054) |   ├─ObjectTypePropertySyntax
//@[02:0026) |   | ├─IdentifierSyntax
//@[02:0026) |   | | └─Token(Identifier) |requiredAndRecursiveProp|
//@[26:0027) |   | ├─Token(Colon) |:|
//@[28:0054) |   | └─VariableAccessSyntax
//@[28:0054) |   |   └─IdentifierSyntax
//@[28:0054) |   |     └─Token(Identifier) |objectWithInvalidRecursion|
//@[54:0055) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type arrayWithInvalidMember = objectWithInvalidRecursion[]
//@[00:0058) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0027) | ├─IdentifierSyntax
//@[05:0027) | | └─Token(Identifier) |arrayWithInvalidMember|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0058) | └─ArrayTypeSyntax
//@[30:0056) |   ├─ArrayTypeMemberSyntax
//@[30:0056) |   | └─VariableAccessSyntax
//@[30:0056) |   |   └─IdentifierSyntax
//@[30:0056) |   |     └─Token(Identifier) |objectWithInvalidRecursion|
//@[56:0057) |   ├─Token(LeftSquare) |[|
//@[57:0058) |   └─Token(RightSquare) |]|
//@[58:0060) ├─Token(NewLine) |\n\n|

@sealed()
//@[00:0040) ├─ParameterDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |sealed|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0010) | ├─Token(NewLine) |\n|
param sealedStringParam string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0023) | ├─IdentifierSyntax
//@[06:0023) | | └─Token(Identifier) |sealedStringParam|
//@[24:0030) | └─VariableAccessSyntax
//@[24:0030) |   └─IdentifierSyntax
//@[24:0030) |     └─Token(Identifier) |string|
//@[30:0032) ├─Token(NewLine) |\n\n|

param disallowedUnionParam 'foo'|-99
//@[00:0036) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0026) | ├─IdentifierSyntax
//@[06:0026) | | └─Token(Identifier) |disallowedUnionParam|
//@[27:0036) | └─UnionTypeSyntax
//@[27:0032) |   ├─UnionTypeMemberSyntax
//@[27:0032) |   | └─StringSyntax
//@[27:0032) |   |   └─Token(StringComplete) |'foo'|
//@[32:0033) |   ├─Token(Pipe) |||
//@[33:0036) |   └─UnionTypeMemberSyntax
//@[33:0036) |     └─UnaryOperationSyntax
//@[33:0034) |       ├─Token(Minus) |-|
//@[34:0036) |       └─IntegerLiteralSyntax
//@[34:0036) |         └─Token(Integer) |99|
//@[36:0038) ├─Token(NewLine) |\n\n|

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[00:0064) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0037) | ├─IdentifierSyntax
//@[06:0037) | | └─Token(Identifier) |objectWithInvalidRecursionParam|
//@[38:0064) | └─VariableAccessSyntax
//@[38:0064) |   └─IdentifierSyntax
//@[38:0064) |     └─Token(Identifier) |objectWithInvalidRecursion|
//@[64:0066) ├─Token(NewLine) |\n\n|

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

@discriminator('type')
//@[00:0051) ├─TypeDeclarationSyntax
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
type unionAB = typeA | typeB
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0012) | ├─IdentifierSyntax
//@[05:0012) | | └─Token(Identifier) |unionAB|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0028) | └─UnionTypeSyntax
//@[15:0020) |   ├─UnionTypeMemberSyntax
//@[15:0020) |   | └─VariableAccessSyntax
//@[15:0020) |   |   └─IdentifierSyntax
//@[15:0020) |   |     └─Token(Identifier) |typeA|
//@[21:0022) |   ├─Token(Pipe) |||
//@[23:0028) |   └─UnionTypeMemberSyntax
//@[23:0028) |     └─VariableAccessSyntax
//@[23:0028) |       └─IdentifierSyntax
//@[23:0028) |         └─Token(Identifier) |typeB|
//@[28:0030) ├─Token(NewLine) |\n\n|

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
//@[00:0040) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |typeE|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0040) | └─ObjectTypeSyntax
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
  *: string
//@[02:0011) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[02:0003) |   | ├─Token(Asterisk) |*|
//@[03:0004) |   | ├─Token(Colon) |:|
//@[05:0011) |   | └─VariableAccessSyntax
//@[05:0011) |   |   └─IdentifierSyntax
//@[05:0011) |   |     └─Token(Identifier) |string|
//@[11:0012) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type typeF = {
//@[00:0042) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |typeF|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0042) | └─ObjectTypeSyntax
//@[13:0014) |   ├─Token(LeftBrace) |{|
//@[14:0015) |   ├─Token(NewLine) |\n|
  type: 0
//@[02:0009) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |type|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0009) |   | └─IntegerLiteralSyntax
//@[08:0009) |   |   └─Token(Integer) |0|
//@[09:0010) |   ├─Token(NewLine) |\n|
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

type typeG = {
//@[00:0045) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |typeG|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0045) | └─ObjectTypeSyntax
//@[13:0014) |   ├─Token(LeftBrace) |{|
//@[14:0015) |   ├─Token(NewLine) |\n|
  type: 'g'?
//@[02:0012) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |type|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0012) |   | └─NullableTypeSyntax
//@[08:0011) |   |   ├─StringSyntax
//@[08:0011) |   |   | └─Token(StringComplete) |'g'|
//@[11:0012) |   |   └─Token(Question) |?|
//@[12:0013) |   ├─Token(NewLine) |\n|
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

type objectUnion = typeA | typeB
//@[00:0032) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0016) | ├─IdentifierSyntax
//@[05:0016) | | └─Token(Identifier) |objectUnion|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0032) | └─UnionTypeSyntax
//@[19:0024) |   ├─UnionTypeMemberSyntax
//@[19:0024) |   | └─VariableAccessSyntax
//@[19:0024) |   |   └─IdentifierSyntax
//@[19:0024) |   |     └─Token(Identifier) |typeA|
//@[25:0026) |   ├─Token(Pipe) |||
//@[27:0032) |   └─UnionTypeMemberSyntax
//@[27:0032) |     └─VariableAccessSyntax
//@[27:0032) |       └─IdentifierSyntax
//@[27:0032) |         └─Token(Identifier) |typeB|
//@[32:0034) ├─Token(NewLine) |\n\n|

@discriminator()
//@[00:0058) ├─TypeDeclarationSyntax
//@[00:0016) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0016) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0016) | |   └─Token(RightParen) |)|
//@[16:0017) | ├─Token(NewLine) |\n|
type noDiscriminatorParam = typeA | typeB
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0025) | ├─IdentifierSyntax
//@[05:0025) | | └─Token(Identifier) |noDiscriminatorParam|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0041) | └─UnionTypeSyntax
//@[28:0033) |   ├─UnionTypeMemberSyntax
//@[28:0033) |   | └─VariableAccessSyntax
//@[28:0033) |   |   └─IdentifierSyntax
//@[28:0033) |   |     └─Token(Identifier) |typeA|
//@[34:0035) |   ├─Token(Pipe) |||
//@[36:0041) |   └─UnionTypeMemberSyntax
//@[36:0041) |     └─VariableAccessSyntax
//@[36:0041) |       └─IdentifierSyntax
//@[36:0041) |         └─Token(Identifier) |typeB|
//@[41:0043) ├─Token(NewLine) |\n\n|

@discriminator(true)
//@[00:0069) ├─TypeDeclarationSyntax
//@[00:0020) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0020) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0019) | |   ├─FunctionArgumentSyntax
//@[15:0019) | |   | └─BooleanLiteralSyntax
//@[15:0019) | |   |   └─Token(TrueKeyword) |true|
//@[19:0020) | |   └─Token(RightParen) |)|
//@[20:0021) | ├─Token(NewLine) |\n|
type wrongDiscriminatorParamType = typeA | typeB
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0032) | ├─IdentifierSyntax
//@[05:0032) | | └─Token(Identifier) |wrongDiscriminatorParamType|
//@[33:0034) | ├─Token(Assignment) |=|
//@[35:0048) | └─UnionTypeSyntax
//@[35:0040) |   ├─UnionTypeMemberSyntax
//@[35:0040) |   | └─VariableAccessSyntax
//@[35:0040) |   |   └─IdentifierSyntax
//@[35:0040) |   |     └─Token(Identifier) |typeA|
//@[41:0042) |   ├─Token(Pipe) |||
//@[43:0048) |   └─UnionTypeMemberSyntax
//@[43:0048) |     └─VariableAccessSyntax
//@[43:0048) |       └─IdentifierSyntax
//@[43:0048) |         └─Token(Identifier) |typeB|
//@[48:0050) ├─Token(NewLine) |\n\n|

@discriminator('nonexistent')
//@[00:0085) ├─TypeDeclarationSyntax
//@[00:0029) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0029) | | └─FunctionCallSyntax
//@[01:0014) | |   ├─IdentifierSyntax
//@[01:0014) | |   | └─Token(Identifier) |discriminator|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0028) | |   ├─FunctionArgumentSyntax
//@[15:0028) | |   | └─StringSyntax
//@[15:0028) | |   |   └─Token(StringComplete) |'nonexistent'|
//@[28:0029) | |   └─Token(RightParen) |)|
//@[29:0030) | ├─Token(NewLine) |\n|
type discriminatorPropertyNotExistAtAll = typeA | typeB
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0039) | ├─IdentifierSyntax
//@[05:0039) | | └─Token(Identifier) |discriminatorPropertyNotExistAtAll|
//@[40:0041) | ├─Token(Assignment) |=|
//@[42:0055) | └─UnionTypeSyntax
//@[42:0047) |   ├─UnionTypeMemberSyntax
//@[42:0047) |   | └─VariableAccessSyntax
//@[42:0047) |   |   └─IdentifierSyntax
//@[42:0047) |   |     └─Token(Identifier) |typeA|
//@[48:0049) |   ├─Token(Pipe) |||
//@[50:0055) |   └─UnionTypeMemberSyntax
//@[50:0055) |     └─VariableAccessSyntax
//@[50:0055) |       └─IdentifierSyntax
//@[50:0055) |         └─Token(Identifier) |typeB|
//@[55:0057) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0095) ├─TypeDeclarationSyntax
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
type discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0046) | ├─IdentifierSyntax
//@[05:0046) | | └─Token(Identifier) |discriminatorPropertyNotExistOnAtLeastOne|
//@[47:0048) | ├─Token(Assignment) |=|
//@[49:0072) | └─UnionTypeSyntax
//@[49:0054) |   ├─UnionTypeMemberSyntax
//@[49:0054) |   | └─VariableAccessSyntax
//@[49:0054) |   |   └─IdentifierSyntax
//@[49:0054) |   |     └─Token(Identifier) |typeA|
//@[55:0056) |   ├─Token(Pipe) |||
//@[57:0072) |   └─UnionTypeMemberSyntax
//@[57:0072) |     └─ObjectTypeSyntax
//@[57:0058) |       ├─Token(LeftBrace) |{|
//@[59:0070) |       ├─ObjectTypePropertySyntax
//@[59:0064) |       | ├─IdentifierSyntax
//@[59:0064) |       | | └─Token(Identifier) |value|
//@[64:0065) |       | ├─Token(Colon) |:|
//@[66:0070) |       | └─VariableAccessSyntax
//@[66:0070) |       |   └─IdentifierSyntax
//@[66:0070) |       |     └─Token(Identifier) |bool|
//@[71:0072) |       └─Token(RightBrace) |}|
//@[72:0074) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0066) ├─TypeDeclarationSyntax
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
type discriminatorWithOnlyOneMember = typeA
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0035) | ├─IdentifierSyntax
//@[05:0035) | | └─Token(Identifier) |discriminatorWithOnlyOneMember|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0043) | └─VariableAccessSyntax
//@[38:0043) |   └─IdentifierSyntax
//@[38:0043) |     └─Token(Identifier) |typeA|
//@[43:0045) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0090) ├─TypeDeclarationSyntax
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
type discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0051) | ├─IdentifierSyntax
//@[05:0051) | | └─Token(Identifier) |discriminatorPropertyNotRequiredStringLiteral1|
//@[52:0053) | ├─Token(Assignment) |=|
//@[54:0067) | └─UnionTypeSyntax
//@[54:0059) |   ├─UnionTypeMemberSyntax
//@[54:0059) |   | └─VariableAccessSyntax
//@[54:0059) |   |   └─IdentifierSyntax
//@[54:0059) |   |     └─Token(Identifier) |typeA|
//@[60:0061) |   ├─Token(Pipe) |||
//@[62:0067) |   └─UnionTypeMemberSyntax
//@[62:0067) |     └─VariableAccessSyntax
//@[62:0067) |       └─IdentifierSyntax
//@[62:0067) |         └─Token(Identifier) |typeF|
//@[67:0069) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0090) ├─TypeDeclarationSyntax
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
type discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0051) | ├─IdentifierSyntax
//@[05:0051) | | └─Token(Identifier) |discriminatorPropertyNotRequiredStringLiteral2|
//@[52:0053) | ├─Token(Assignment) |=|
//@[54:0067) | └─UnionTypeSyntax
//@[54:0059) |   ├─UnionTypeMemberSyntax
//@[54:0059) |   | └─VariableAccessSyntax
//@[54:0059) |   |   └─IdentifierSyntax
//@[54:0059) |   |     └─Token(Identifier) |typeA|
//@[60:0061) |   ├─Token(Pipe) |||
//@[62:0067) |   └─UnionTypeMemberSyntax
//@[62:0067) |     └─VariableAccessSyntax
//@[62:0067) |       └─IdentifierSyntax
//@[62:0067) |         └─Token(Identifier) |typeG|
//@[67:0069) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0074) ├─TypeDeclarationSyntax
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
type discriminatorDuplicatedMember1 = typeA | typeA
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0035) | ├─IdentifierSyntax
//@[05:0035) | | └─Token(Identifier) |discriminatorDuplicatedMember1|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0051) | └─UnionTypeSyntax
//@[38:0043) |   ├─UnionTypeMemberSyntax
//@[38:0043) |   | └─VariableAccessSyntax
//@[38:0043) |   |   └─IdentifierSyntax
//@[38:0043) |   |     └─Token(Identifier) |typeA|
//@[44:0045) |   ├─Token(Pipe) |||
//@[46:0051) |   └─UnionTypeMemberSyntax
//@[46:0051) |     └─VariableAccessSyntax
//@[46:0051) |       └─IdentifierSyntax
//@[46:0051) |         └─Token(Identifier) |typeA|
//@[51:0053) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0098) ├─TypeDeclarationSyntax
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
type discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0035) | ├─IdentifierSyntax
//@[05:0035) | | └─Token(Identifier) |discriminatorDuplicatedMember2|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0075) | └─UnionTypeSyntax
//@[38:0043) |   ├─UnionTypeMemberSyntax
//@[38:0043) |   | └─VariableAccessSyntax
//@[38:0043) |   |   └─IdentifierSyntax
//@[38:0043) |   |     └─Token(Identifier) |typeA|
//@[44:0045) |   ├─Token(Pipe) |||
//@[46:0075) |   └─UnionTypeMemberSyntax
//@[46:0075) |     └─ObjectTypeSyntax
//@[46:0047) |       ├─Token(LeftBrace) |{|
//@[48:0057) |       ├─ObjectTypePropertySyntax
//@[48:0052) |       | ├─IdentifierSyntax
//@[48:0052) |       | | └─Token(Identifier) |type|
//@[52:0053) |       | ├─Token(Colon) |:|
//@[54:0057) |       | └─StringSyntax
//@[54:0057) |       |   └─Token(StringComplete) |'a'|
//@[57:0058) |       ├─Token(Comma) |,|
//@[59:0073) |       ├─ObjectTypePropertySyntax
//@[59:0065) |       | ├─IdentifierSyntax
//@[59:0065) |       | | └─Token(Identifier) |config|
//@[65:0066) |       | ├─Token(Colon) |:|
//@[67:0073) |       | └─VariableAccessSyntax
//@[67:0073) |       |   └─IdentifierSyntax
//@[67:0073) |       |     └─Token(Identifier) |object|
//@[74:0075) |       └─Token(RightBrace) |}|
//@[75:0077) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0083) ├─TypeDeclarationSyntax
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
type discriminatorSelfCycle = typeA | discriminatorSelfCycle
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0027) | ├─IdentifierSyntax
//@[05:0027) | | └─Token(Identifier) |discriminatorSelfCycle|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0060) | └─UnionTypeSyntax
//@[30:0035) |   ├─UnionTypeMemberSyntax
//@[30:0035) |   | └─VariableAccessSyntax
//@[30:0035) |   |   └─IdentifierSyntax
//@[30:0035) |   |     └─Token(Identifier) |typeA|
//@[36:0037) |   ├─Token(Pipe) |||
//@[38:0060) |   └─UnionTypeMemberSyntax
//@[38:0060) |     └─VariableAccessSyntax
//@[38:0060) |       └─IdentifierSyntax
//@[38:0060) |         └─Token(Identifier) |discriminatorSelfCycle|
//@[60:0062) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0093) ├─TypeDeclarationSyntax
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
type discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0032) | ├─IdentifierSyntax
//@[05:0032) | | └─Token(Identifier) |discriminatorTopLevelCycleA|
//@[33:0034) | ├─Token(Assignment) |=|
//@[35:0070) | └─UnionTypeSyntax
//@[35:0040) |   ├─UnionTypeMemberSyntax
//@[35:0040) |   | └─VariableAccessSyntax
//@[35:0040) |   |   └─IdentifierSyntax
//@[35:0040) |   |     └─Token(Identifier) |typeA|
//@[41:0042) |   ├─Token(Pipe) |||
//@[43:0070) |   └─UnionTypeMemberSyntax
//@[43:0070) |     └─VariableAccessSyntax
//@[43:0070) |       └─IdentifierSyntax
//@[43:0070) |         └─Token(Identifier) |discriminatorTopLevelCycleB|
//@[70:0071) ├─Token(NewLine) |\n|
@discriminator('type')
//@[00:0093) ├─TypeDeclarationSyntax
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
type discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0032) | ├─IdentifierSyntax
//@[05:0032) | | └─Token(Identifier) |discriminatorTopLevelCycleB|
//@[33:0034) | ├─Token(Assignment) |=|
//@[35:0070) | └─UnionTypeSyntax
//@[35:0040) |   ├─UnionTypeMemberSyntax
//@[35:0040) |   | └─VariableAccessSyntax
//@[35:0040) |   |   └─IdentifierSyntax
//@[35:0040) |   |     └─Token(Identifier) |typeB|
//@[41:0042) |   ├─Token(Pipe) |||
//@[43:0070) |   └─UnionTypeMemberSyntax
//@[43:0070) |     └─VariableAccessSyntax
//@[43:0070) |       └─IdentifierSyntax
//@[43:0070) |         └─Token(Identifier) |discriminatorTopLevelCycleA|
//@[70:0072) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[00:0120) ├─TypeDeclarationSyntax
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
type discriminatorInnerSelfCycle1 = typeA | {
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0033) | ├─IdentifierSyntax
//@[05:0033) | | └─Token(Identifier) |discriminatorInnerSelfCycle1|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0097) | └─UnionTypeSyntax
//@[36:0041) |   ├─UnionTypeMemberSyntax
//@[36:0041) |   | └─VariableAccessSyntax
//@[36:0041) |   |   └─IdentifierSyntax
//@[36:0041) |   |     └─Token(Identifier) |typeA|
//@[42:0043) |   ├─Token(Pipe) |||
//@[44:0097) |   └─UnionTypeMemberSyntax
//@[44:0097) |     └─ObjectTypeSyntax
//@[44:0045) |       ├─Token(LeftBrace) |{|
//@[45:0046) |       ├─Token(NewLine) |\n|
  type: 'b'
//@[02:0011) |       ├─ObjectTypePropertySyntax
//@[02:0006) |       | ├─IdentifierSyntax
//@[02:0006) |       | | └─Token(Identifier) |type|
//@[06:0007) |       | ├─Token(Colon) |:|
//@[08:0011) |       | └─StringSyntax
//@[08:0011) |       |   └─Token(StringComplete) |'b'|
//@[11:0012) |       ├─Token(NewLine) |\n|
  value: discriminatorInnerSelfCycle1
//@[02:0037) |       ├─ObjectTypePropertySyntax
//@[02:0007) |       | ├─IdentifierSyntax
//@[02:0007) |       | | └─Token(Identifier) |value|
//@[07:0008) |       | ├─Token(Colon) |:|
//@[09:0037) |       | └─VariableAccessSyntax
//@[09:0037) |       |   └─IdentifierSyntax
//@[09:0037) |       |     └─Token(Identifier) |discriminatorInnerSelfCycle1|
//@[37:0038) |       ├─Token(NewLine) |\n|
}
//@[00:0001) |       └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type discriminatorInnerSelfCycle2Helper = {
//@[00:0095) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0039) | ├─IdentifierSyntax
//@[05:0039) | | └─Token(Identifier) |discriminatorInnerSelfCycle2Helper|
//@[40:0041) | ├─Token(Assignment) |=|
//@[42:0095) | └─ObjectTypeSyntax
//@[42:0043) |   ├─Token(LeftBrace) |{|
//@[43:0044) |   ├─Token(NewLine) |\n|
  type: 'b'
//@[02:0011) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |type|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0011) |   | └─StringSyntax
//@[08:0011) |   |   └─Token(StringComplete) |'b'|
//@[11:0012) |   ├─Token(NewLine) |\n|
  value: discriminatorInnerSelfCycle2
//@[02:0037) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |value|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0037) |   | └─VariableAccessSyntax
//@[09:0037) |   |   └─IdentifierSyntax
//@[09:0037) |   |     └─Token(Identifier) |discriminatorInnerSelfCycle2|
//@[37:0038) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
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
type discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0033) | ├─IdentifierSyntax
//@[05:0033) | | └─Token(Identifier) |discriminatorInnerSelfCycle2|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0078) | └─UnionTypeSyntax
//@[36:0041) |   ├─UnionTypeMemberSyntax
//@[36:0041) |   | └─VariableAccessSyntax
//@[36:0041) |   |   └─IdentifierSyntax
//@[36:0041) |   |     └─Token(Identifier) |typeA|
//@[42:0043) |   ├─Token(Pipe) |||
//@[44:0078) |   └─UnionTypeMemberSyntax
//@[44:0078) |     └─VariableAccessSyntax
//@[44:0078) |       └─IdentifierSyntax
//@[44:0078) |         └─Token(Identifier) |discriminatorInnerSelfCycle2Helper|
//@[78:0079) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
