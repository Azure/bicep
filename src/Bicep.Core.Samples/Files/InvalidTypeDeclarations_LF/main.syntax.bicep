type 44
//@[00:1407) ProgramSyntax
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

func invalidArgs(a validStringLiteralUnion, b string) string => a
//@[00:0065) ├─FunctionDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |func|
//@[05:0016) | ├─IdentifierSyntax
//@[05:0016) | | └─Token(Identifier) |invalidArgs|
//@[16:0065) | └─TypedLambdaSyntax
//@[16:0053) |   ├─TypedVariableBlockSyntax
//@[16:0017) |   | ├─Token(LeftParen) |(|
//@[17:0042) |   | ├─TypedLocalVariableSyntax
//@[17:0018) |   | | ├─IdentifierSyntax
//@[17:0018) |   | | | └─Token(Identifier) |a|
//@[19:0042) |   | | └─VariableAccessSyntax
//@[19:0042) |   | |   └─IdentifierSyntax
//@[19:0042) |   | |     └─Token(Identifier) |validStringLiteralUnion|
//@[42:0043) |   | ├─Token(Comma) |,|
//@[44:0052) |   | ├─TypedLocalVariableSyntax
//@[44:0045) |   | | ├─IdentifierSyntax
//@[44:0045) |   | | | └─Token(Identifier) |b|
//@[46:0052) |   | | └─VariableAccessSyntax
//@[46:0052) |   | |   └─IdentifierSyntax
//@[46:0052) |   | |     └─Token(Identifier) |string|
//@[52:0053) |   | └─Token(RightParen) |)|
//@[54:0060) |   ├─VariableAccessSyntax
//@[54:0060) |   | └─IdentifierSyntax
//@[54:0060) |   |   └─Token(Identifier) |string|
//@[61:0063) |   ├─Token(Arrow) |=>|
//@[64:0065) |   └─VariableAccessSyntax
//@[64:0065) |     └─IdentifierSyntax
//@[64:0065) |       └─Token(Identifier) |a|
//@[65:0067) ├─Token(NewLine) |\n\n|

func invalidOutput() validStringLiteralUnion => 'foo'
//@[00:0053) ├─FunctionDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |func|
//@[05:0018) | ├─IdentifierSyntax
//@[05:0018) | | └─Token(Identifier) |invalidOutput|
//@[18:0053) | └─TypedLambdaSyntax
//@[18:0020) |   ├─TypedVariableBlockSyntax
//@[18:0019) |   | ├─Token(LeftParen) |(|
//@[19:0020) |   | └─Token(RightParen) |)|
//@[21:0044) |   ├─VariableAccessSyntax
//@[21:0044) |   | └─IdentifierSyntax
//@[21:0044) |   |   └─Token(Identifier) |validStringLiteralUnion|
//@[45:0047) |   ├─Token(Arrow) |=>|
//@[48:0053) |   └─StringSyntax
//@[48:0053) |     └─Token(StringComplete) |'foo'|
//@[53:0054) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
