type 44
//@[000:4594) ProgramSyntax
//@[000:0007) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0007) | ├─IdentifierSyntax
//@[005:0007) | | └─SkippedTriviaSyntax
//@[005:0007) | |   └─Token(Integer) |44|
//@[007:0007) | ├─SkippedTriviaSyntax
//@[007:0007) | └─SkippedTriviaSyntax
//@[007:0009) ├─Token(NewLine) |\n\n|

type noAssignment
//@[000:0017) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0017) | ├─IdentifierSyntax
//@[005:0017) | | └─Token(Identifier) |noAssignment|
//@[017:0017) | ├─SkippedTriviaSyntax
//@[017:0017) | └─SkippedTriviaSyntax
//@[017:0019) ├─Token(NewLine) |\n\n|

type incompleteAssignment =
//@[000:0027) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |incompleteAssignment|
//@[026:0027) | ├─Token(Assignment) |=|
//@[027:0027) | └─SkippedTriviaSyntax
//@[027:0029) ├─Token(NewLine) |\n\n|

type resource = bool
//@[000:0020) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |resource|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0020) | └─VariableAccessSyntax
//@[016:0020) |   └─IdentifierSyntax
//@[016:0020) |     └─Token(Identifier) |bool|
//@[020:0022) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0036) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type sealedString = string
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0017) | ├─IdentifierSyntax
//@[005:0017) | | └─Token(Identifier) |sealedString|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0026) | └─VariableAccessSyntax
//@[020:0026) |   └─IdentifierSyntax
//@[020:0026) |     └─Token(Identifier) |string|
//@[026:0028) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0048) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type sealedDictionary = {
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0021) | ├─IdentifierSyntax
//@[005:0021) | | └─Token(Identifier) |sealedDictionary|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0038) | └─ObjectTypeSyntax
//@[024:0025) |   ├─Token(LeftBrace) |{|
//@[025:0026) |   ├─Token(NewLine) |\n|
	*: string
//@[001:0010) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[001:0002) |   | ├─Token(Asterisk) |*|
//@[002:0003) |   | ├─Token(Colon) |:|
//@[004:0010) |   | └─VariableAccessSyntax
//@[004:0010) |   |   └─IdentifierSyntax
//@[004:0010) |   |     └─Token(Identifier) |string|
//@[010:0011) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type disallowedUnion = 'foo'|21
//@[000:0031) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0020) | ├─IdentifierSyntax
//@[005:0020) | | └─Token(Identifier) |disallowedUnion|
//@[021:0022) | ├─Token(Assignment) |=|
//@[023:0031) | └─UnionTypeSyntax
//@[023:0028) |   ├─UnionTypeMemberSyntax
//@[023:0028) |   | └─StringSyntax
//@[023:0028) |   |   └─Token(StringComplete) |'foo'|
//@[028:0029) |   ├─Token(Pipe) |||
//@[029:0031) |   └─UnionTypeMemberSyntax
//@[029:0031) |     └─IntegerLiteralSyntax
//@[029:0031) |       └─Token(Integer) |21|
//@[031:0033) ├─Token(NewLine) |\n\n|

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[000:0048) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0028) | ├─IdentifierSyntax
//@[005:0028) | | └─Token(Identifier) |validStringLiteralUnion|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0048) | └─UnionTypeSyntax
//@[031:0036) |   ├─UnionTypeMemberSyntax
//@[031:0036) |   | └─StringSyntax
//@[031:0036) |   |   └─Token(StringComplete) |'foo'|
//@[036:0037) |   ├─Token(Pipe) |||
//@[037:0042) |   ├─UnionTypeMemberSyntax
//@[037:0042) |   | └─StringSyntax
//@[037:0042) |   |   └─Token(StringComplete) |'bar'|
//@[042:0043) |   ├─Token(Pipe) |||
//@[043:0048) |   └─UnionTypeMemberSyntax
//@[043:0048) |     └─StringSyntax
//@[043:0048) |       └─Token(StringComplete) |'baz'|
//@[048:0050) ├─Token(NewLine) |\n\n|

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[000:0059) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0030) | ├─IdentifierSyntax
//@[005:0030) | | └─Token(Identifier) |validUnionInvalidAddition|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0059) | └─UnionTypeSyntax
//@[033:0056) |   ├─UnionTypeMemberSyntax
//@[033:0056) |   | └─VariableAccessSyntax
//@[033:0056) |   |   └─IdentifierSyntax
//@[033:0056) |   |     └─Token(Identifier) |validStringLiteralUnion|
//@[056:0057) |   ├─Token(Pipe) |||
//@[057:0059) |   └─UnionTypeMemberSyntax
//@[057:0059) |     └─IntegerLiteralSyntax
//@[057:0059) |       └─Token(Integer) |10|
//@[059:0061) ├─Token(NewLine) |\n\n|

type invalidUnionInvalidAddition = disallowedUnion|true
//@[000:0055) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0032) | ├─IdentifierSyntax
//@[005:0032) | | └─Token(Identifier) |invalidUnionInvalidAddition|
//@[033:0034) | ├─Token(Assignment) |=|
//@[035:0055) | └─UnionTypeSyntax
//@[035:0050) |   ├─UnionTypeMemberSyntax
//@[035:0050) |   | └─VariableAccessSyntax
//@[035:0050) |   |   └─IdentifierSyntax
//@[035:0050) |   |     └─Token(Identifier) |disallowedUnion|
//@[050:0051) |   ├─Token(Pipe) |||
//@[051:0055) |   └─UnionTypeMemberSyntax
//@[051:0055) |     └─BooleanLiteralSyntax
//@[051:0055) |       └─Token(TrueKeyword) |true|
//@[055:0057) ├─Token(NewLine) |\n\n|

type nullLiteral = null
//@[000:0023) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0016) | ├─IdentifierSyntax
//@[005:0016) | | └─Token(Identifier) |nullLiteral|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0023) | └─NullLiteralSyntax
//@[019:0023) |   └─Token(NullKeyword) |null|
//@[023:0025) ├─Token(NewLine) |\n\n|

type unionOfNulls = null|null
//@[000:0029) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0017) | ├─IdentifierSyntax
//@[005:0017) | | └─Token(Identifier) |unionOfNulls|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0029) | └─UnionTypeSyntax
//@[020:0024) |   ├─UnionTypeMemberSyntax
//@[020:0024) |   | └─NullLiteralSyntax
//@[020:0024) |   |   └─Token(NullKeyword) |null|
//@[024:0025) |   ├─Token(Pipe) |||
//@[025:0029) |   └─UnionTypeMemberSyntax
//@[025:0029) |     └─NullLiteralSyntax
//@[025:0029) |       └─Token(NullKeyword) |null|
//@[029:0031) ├─Token(NewLine) |\n\n|

@minLength(3)
//@[000:0045) ├─TypeDeclarationSyntax
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
type lengthConstrainedInt = int
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |lengthConstrainedInt|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0031) | └─VariableAccessSyntax
//@[028:0031) |   └─IdentifierSyntax
//@[028:0031) |     └─Token(Identifier) |int|
//@[031:0033) ├─Token(NewLine) |\n\n|

@minValue(3)
//@[000:0049) ├─TypeDeclarationSyntax
//@[000:0012) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0012) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0011) | |   ├─FunctionArgumentSyntax
//@[010:0011) | |   | └─IntegerLiteralSyntax
//@[010:0011) | |   |   └─Token(Integer) |3|
//@[011:0012) | |   └─Token(RightParen) |)|
//@[012:0013) | ├─Token(NewLine) |\n|
type valueConstrainedString = string
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0027) | ├─IdentifierSyntax
//@[005:0027) | | └─Token(Identifier) |valueConstrainedString|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0036) | └─VariableAccessSyntax
//@[030:0036) |   └─IdentifierSyntax
//@[030:0036) |     └─Token(Identifier) |string|
//@[036:0038) ├─Token(NewLine) |\n\n|

type tautology = tautology
//@[000:0026) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |tautology|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0026) | └─VariableAccessSyntax
//@[017:0026) |   └─IdentifierSyntax
//@[017:0026) |     └─Token(Identifier) |tautology|
//@[026:0028) ├─Token(NewLine) |\n\n|

type tautologicalUnion = tautologicalUnion|'foo'
//@[000:0048) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0022) | ├─IdentifierSyntax
//@[005:0022) | | └─Token(Identifier) |tautologicalUnion|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0048) | └─UnionTypeSyntax
//@[025:0042) |   ├─UnionTypeMemberSyntax
//@[025:0042) |   | └─VariableAccessSyntax
//@[025:0042) |   |   └─IdentifierSyntax
//@[025:0042) |   |     └─Token(Identifier) |tautologicalUnion|
//@[042:0043) |   ├─Token(Pipe) |||
//@[043:0048) |   └─UnionTypeMemberSyntax
//@[043:0048) |     └─StringSyntax
//@[043:0048) |       └─Token(StringComplete) |'foo'|
//@[048:0050) ├─Token(NewLine) |\n\n|

type tautologicalArray = tautologicalArray[]
//@[000:0044) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0022) | ├─IdentifierSyntax
//@[005:0022) | | └─Token(Identifier) |tautologicalArray|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0044) | └─ArrayTypeSyntax
//@[025:0042) |   ├─ArrayTypeMemberSyntax
//@[025:0042) |   | └─VariableAccessSyntax
//@[025:0042) |   |   └─IdentifierSyntax
//@[025:0042) |   |     └─Token(Identifier) |tautologicalArray|
//@[042:0043) |   ├─Token(LeftSquare) |[|
//@[043:0044) |   └─Token(RightSquare) |]|
//@[044:0046) ├─Token(NewLine) |\n\n|

type directCycleStart = directCycleReturn
//@[000:0041) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0021) | ├─IdentifierSyntax
//@[005:0021) | | └─Token(Identifier) |directCycleStart|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0041) | └─VariableAccessSyntax
//@[024:0041) |   └─IdentifierSyntax
//@[024:0041) |     └─Token(Identifier) |directCycleReturn|
//@[041:0043) ├─Token(NewLine) |\n\n|

type directCycleReturn = directCycleStart
//@[000:0041) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0022) | ├─IdentifierSyntax
//@[005:0022) | | └─Token(Identifier) |directCycleReturn|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0041) | └─VariableAccessSyntax
//@[025:0041) |   └─IdentifierSyntax
//@[025:0041) |     └─Token(Identifier) |directCycleStart|
//@[041:0043) ├─Token(NewLine) |\n\n|

type cycleRoot = connector
//@[000:0026) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |cycleRoot|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0026) | └─VariableAccessSyntax
//@[017:0026) |   └─IdentifierSyntax
//@[017:0026) |     └─Token(Identifier) |connector|
//@[026:0028) ├─Token(NewLine) |\n\n|

type connector = cycleBack
//@[000:0026) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |connector|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0026) | └─VariableAccessSyntax
//@[017:0026) |   └─IdentifierSyntax
//@[017:0026) |     └─Token(Identifier) |cycleBack|
//@[026:0028) ├─Token(NewLine) |\n\n|

type cycleBack = cycleRoot
//@[000:0026) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |cycleBack|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0026) | └─VariableAccessSyntax
//@[017:0026) |   └─IdentifierSyntax
//@[017:0026) |     └─Token(Identifier) |cycleRoot|
//@[026:0028) ├─Token(NewLine) |\n\n|

type objectWithInvalidPropertyDecorators = {
//@[000:0168) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0040) | ├─IdentifierSyntax
//@[005:0040) | | └─Token(Identifier) |objectWithInvalidPropertyDecorators|
//@[041:0042) | ├─Token(Assignment) |=|
//@[043:0168) | └─ObjectTypeSyntax
//@[043:0044) |   ├─Token(LeftBrace) |{|
//@[044:0045) |   ├─Token(NewLine) |\n|
  @sealed()
//@[002:0029) |   ├─ObjectTypePropertySyntax
//@[002:0011) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0011) |   | | └─FunctionCallSyntax
//@[003:0009) |   | |   ├─IdentifierSyntax
//@[003:0009) |   | |   | └─Token(Identifier) |sealed|
//@[009:0010) |   | |   ├─Token(LeftParen) |(|
//@[010:0011) |   | |   └─Token(RightParen) |)|
//@[011:0012) |   | ├─Token(NewLine) |\n|
  fooProp: string
//@[002:0009) |   | ├─IdentifierSyntax
//@[002:0009) |   | | └─Token(Identifier) |fooProp|
//@[009:0010) |   | ├─Token(Colon) |:|
//@[011:0017) |   | └─VariableAccessSyntax
//@[011:0017) |   |   └─IdentifierSyntax
//@[011:0017) |   |     └─Token(Identifier) |string|
//@[017:0019) |   ├─Token(NewLine) |\n\n|

  @secure()
//@[002:0029) |   ├─ObjectTypePropertySyntax
//@[002:0011) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0011) |   | | └─FunctionCallSyntax
//@[003:0009) |   | |   ├─IdentifierSyntax
//@[003:0009) |   | |   | └─Token(Identifier) |secure|
//@[009:0010) |   | |   ├─Token(LeftParen) |(|
//@[010:0011) |   | |   └─Token(RightParen) |)|
//@[011:0012) |   | ├─Token(NewLine) |\n|
  barProp: string
//@[002:0009) |   | ├─IdentifierSyntax
//@[002:0009) |   | | └─Token(Identifier) |barProp|
//@[009:0010) |   | ├─Token(Colon) |:|
//@[011:0017) |   | └─VariableAccessSyntax
//@[011:0017) |   |   └─IdentifierSyntax
//@[011:0017) |   |     └─Token(Identifier) |string|
//@[017:0019) |   ├─Token(NewLine) |\n\n|

  @allowed(['snap', 'crackle', 'pop'])
//@[002:0059) |   ├─ObjectTypePropertySyntax
//@[002:0038) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0038) |   | | └─FunctionCallSyntax
//@[003:0010) |   | |   ├─IdentifierSyntax
//@[003:0010) |   | |   | └─Token(Identifier) |allowed|
//@[010:0011) |   | |   ├─Token(LeftParen) |(|
//@[011:0037) |   | |   ├─FunctionArgumentSyntax
//@[011:0037) |   | |   | └─ArraySyntax
//@[011:0012) |   | |   |   ├─Token(LeftSquare) |[|
//@[012:0018) |   | |   |   ├─ArrayItemSyntax
//@[012:0018) |   | |   |   | └─StringSyntax
//@[012:0018) |   | |   |   |   └─Token(StringComplete) |'snap'|
//@[018:0019) |   | |   |   ├─Token(Comma) |,|
//@[020:0029) |   | |   |   ├─ArrayItemSyntax
//@[020:0029) |   | |   |   | └─StringSyntax
//@[020:0029) |   | |   |   |   └─Token(StringComplete) |'crackle'|
//@[029:0030) |   | |   |   ├─Token(Comma) |,|
//@[031:0036) |   | |   |   ├─ArrayItemSyntax
//@[031:0036) |   | |   |   | └─StringSyntax
//@[031:0036) |   | |   |   |   └─Token(StringComplete) |'pop'|
//@[036:0037) |   | |   |   └─Token(RightSquare) |]|
//@[037:0038) |   | |   └─Token(RightParen) |)|
//@[038:0039) |   | ├─Token(NewLine) |\n|
  krispyProp: string
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |krispyProp|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0020) |   | └─VariableAccessSyntax
//@[014:0020) |   |   └─IdentifierSyntax
//@[014:0020) |   |     └─Token(Identifier) |string|
//@[020:0021) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type objectWithInvalidRecursion = {
//@[000:0092) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0031) | ├─IdentifierSyntax
//@[005:0031) | | └─Token(Identifier) |objectWithInvalidRecursion|
//@[032:0033) | ├─Token(Assignment) |=|
//@[034:0092) | └─ObjectTypeSyntax
//@[034:0035) |   ├─Token(LeftBrace) |{|
//@[035:0036) |   ├─Token(NewLine) |\n|
  requiredAndRecursiveProp: objectWithInvalidRecursion
//@[002:0054) |   ├─ObjectTypePropertySyntax
//@[002:0026) |   | ├─IdentifierSyntax
//@[002:0026) |   | | └─Token(Identifier) |requiredAndRecursiveProp|
//@[026:0027) |   | ├─Token(Colon) |:|
//@[028:0054) |   | └─VariableAccessSyntax
//@[028:0054) |   |   └─IdentifierSyntax
//@[028:0054) |   |     └─Token(Identifier) |objectWithInvalidRecursion|
//@[054:0055) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type arrayWithInvalidMember = objectWithInvalidRecursion[]
//@[000:0058) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0027) | ├─IdentifierSyntax
//@[005:0027) | | └─Token(Identifier) |arrayWithInvalidMember|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0058) | └─ArrayTypeSyntax
//@[030:0056) |   ├─ArrayTypeMemberSyntax
//@[030:0056) |   | └─VariableAccessSyntax
//@[030:0056) |   |   └─IdentifierSyntax
//@[030:0056) |   |     └─Token(Identifier) |objectWithInvalidRecursion|
//@[056:0057) |   ├─Token(LeftSquare) |[|
//@[057:0058) |   └─Token(RightSquare) |]|
//@[058:0060) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0040) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
param sealedStringParam string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |sealedStringParam|
//@[024:0030) | └─VariableAccessSyntax
//@[024:0030) |   └─IdentifierSyntax
//@[024:0030) |     └─Token(Identifier) |string|
//@[030:0032) ├─Token(NewLine) |\n\n|

param disallowedUnionParam 'foo'|-99
//@[000:0036) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |disallowedUnionParam|
//@[027:0036) | └─UnionTypeSyntax
//@[027:0032) |   ├─UnionTypeMemberSyntax
//@[027:0032) |   | └─StringSyntax
//@[027:0032) |   |   └─Token(StringComplete) |'foo'|
//@[032:0033) |   ├─Token(Pipe) |||
//@[033:0036) |   └─UnionTypeMemberSyntax
//@[033:0036) |     └─UnaryOperationSyntax
//@[033:0034) |       ├─Token(Minus) |-|
//@[034:0036) |       └─IntegerLiteralSyntax
//@[034:0036) |         └─Token(Integer) |99|
//@[036:0038) ├─Token(NewLine) |\n\n|

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[000:0064) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0037) | ├─IdentifierSyntax
//@[006:0037) | | └─Token(Identifier) |objectWithInvalidRecursionParam|
//@[038:0064) | └─VariableAccessSyntax
//@[038:0064) |   └─IdentifierSyntax
//@[038:0064) |     └─Token(Identifier) |objectWithInvalidRecursion|
//@[064:0066) ├─Token(NewLine) |\n\n|

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

@discriminator('type')
//@[000:0051) ├─TypeDeclarationSyntax
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
type unionAB = typeA | typeB
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0012) | ├─IdentifierSyntax
//@[005:0012) | | └─Token(Identifier) |unionAB|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0028) | └─UnionTypeSyntax
//@[015:0020) |   ├─UnionTypeMemberSyntax
//@[015:0020) |   | └─VariableAccessSyntax
//@[015:0020) |   |   └─IdentifierSyntax
//@[015:0020) |   |     └─Token(Identifier) |typeA|
//@[021:0022) |   ├─Token(Pipe) |||
//@[023:0028) |   └─UnionTypeMemberSyntax
//@[023:0028) |     └─VariableAccessSyntax
//@[023:0028) |       └─IdentifierSyntax
//@[023:0028) |         └─Token(Identifier) |typeB|
//@[028:0030) ├─Token(NewLine) |\n\n|

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
//@[000:0040) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeE|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0040) | └─ObjectTypeSyntax
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

type typeF = {
//@[000:0042) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeF|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0042) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  type: 0
//@[002:0009) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0009) |   | └─IntegerLiteralSyntax
//@[008:0009) |   |   └─Token(Integer) |0|
//@[009:0010) |   ├─Token(NewLine) |\n|
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

type typeG = {
//@[000:0045) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |typeG|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0045) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  type: 'g'?
//@[002:0012) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0012) |   | └─NullableTypeSyntax
//@[008:0011) |   |   ├─StringSyntax
//@[008:0011) |   |   | └─Token(StringComplete) |'g'|
//@[011:0012) |   |   └─Token(Question) |?|
//@[012:0013) |   ├─Token(NewLine) |\n|
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

type objectUnion = typeA | typeB
//@[000:0032) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0016) | ├─IdentifierSyntax
//@[005:0016) | | └─Token(Identifier) |objectUnion|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0032) | └─UnionTypeSyntax
//@[019:0024) |   ├─UnionTypeMemberSyntax
//@[019:0024) |   | └─VariableAccessSyntax
//@[019:0024) |   |   └─IdentifierSyntax
//@[019:0024) |   |     └─Token(Identifier) |typeA|
//@[025:0026) |   ├─Token(Pipe) |||
//@[027:0032) |   └─UnionTypeMemberSyntax
//@[027:0032) |     └─VariableAccessSyntax
//@[027:0032) |       └─IdentifierSyntax
//@[027:0032) |         └─Token(Identifier) |typeB|
//@[032:0034) ├─Token(NewLine) |\n\n|

@discriminator()
//@[000:0058) ├─TypeDeclarationSyntax
//@[000:0016) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0016) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0016) | |   └─Token(RightParen) |)|
//@[016:0017) | ├─Token(NewLine) |\n|
type noDiscriminatorParam = typeA | typeB
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |noDiscriminatorParam|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0041) | └─UnionTypeSyntax
//@[028:0033) |   ├─UnionTypeMemberSyntax
//@[028:0033) |   | └─VariableAccessSyntax
//@[028:0033) |   |   └─IdentifierSyntax
//@[028:0033) |   |     └─Token(Identifier) |typeA|
//@[034:0035) |   ├─Token(Pipe) |||
//@[036:0041) |   └─UnionTypeMemberSyntax
//@[036:0041) |     └─VariableAccessSyntax
//@[036:0041) |       └─IdentifierSyntax
//@[036:0041) |         └─Token(Identifier) |typeB|
//@[041:0043) ├─Token(NewLine) |\n\n|

@discriminator(true)
//@[000:0069) ├─TypeDeclarationSyntax
//@[000:0020) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0020) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0019) | |   ├─FunctionArgumentSyntax
//@[015:0019) | |   | └─BooleanLiteralSyntax
//@[015:0019) | |   |   └─Token(TrueKeyword) |true|
//@[019:0020) | |   └─Token(RightParen) |)|
//@[020:0021) | ├─Token(NewLine) |\n|
type wrongDiscriminatorParamType = typeA | typeB
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0032) | ├─IdentifierSyntax
//@[005:0032) | | └─Token(Identifier) |wrongDiscriminatorParamType|
//@[033:0034) | ├─Token(Assignment) |=|
//@[035:0048) | └─UnionTypeSyntax
//@[035:0040) |   ├─UnionTypeMemberSyntax
//@[035:0040) |   | └─VariableAccessSyntax
//@[035:0040) |   |   └─IdentifierSyntax
//@[035:0040) |   |     └─Token(Identifier) |typeA|
//@[041:0042) |   ├─Token(Pipe) |||
//@[043:0048) |   └─UnionTypeMemberSyntax
//@[043:0048) |     └─VariableAccessSyntax
//@[043:0048) |       └─IdentifierSyntax
//@[043:0048) |         └─Token(Identifier) |typeB|
//@[048:0050) ├─Token(NewLine) |\n\n|

@discriminator('nonexistent')
//@[000:0085) ├─TypeDeclarationSyntax
//@[000:0029) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0029) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0028) | |   ├─FunctionArgumentSyntax
//@[015:0028) | |   | └─StringSyntax
//@[015:0028) | |   |   └─Token(StringComplete) |'nonexistent'|
//@[028:0029) | |   └─Token(RightParen) |)|
//@[029:0030) | ├─Token(NewLine) |\n|
type discriminatorPropertyNotExistAtAll = typeA | typeB
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0039) | ├─IdentifierSyntax
//@[005:0039) | | └─Token(Identifier) |discriminatorPropertyNotExistAtAll|
//@[040:0041) | ├─Token(Assignment) |=|
//@[042:0055) | └─UnionTypeSyntax
//@[042:0047) |   ├─UnionTypeMemberSyntax
//@[042:0047) |   | └─VariableAccessSyntax
//@[042:0047) |   |   └─IdentifierSyntax
//@[042:0047) |   |     └─Token(Identifier) |typeA|
//@[048:0049) |   ├─Token(Pipe) |||
//@[050:0055) |   └─UnionTypeMemberSyntax
//@[050:0055) |     └─VariableAccessSyntax
//@[050:0055) |       └─IdentifierSyntax
//@[050:0055) |         └─Token(Identifier) |typeB|
//@[055:0057) ├─Token(NewLine) |\n\n|

@discriminator('nonexistent')
//@[000:0074) ├─TypeDeclarationSyntax
//@[000:0029) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0029) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0028) | |   ├─FunctionArgumentSyntax
//@[015:0028) | |   | └─StringSyntax
//@[015:0028) | |   |   └─Token(StringComplete) |'nonexistent'|
//@[028:0029) | |   └─Token(RightParen) |)|
//@[029:0030) | ├─Token(NewLine) |\n|
type discriminatorPropertyMismatch = unionAB
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0034) | ├─IdentifierSyntax
//@[005:0034) | | └─Token(Identifier) |discriminatorPropertyMismatch|
//@[035:0036) | ├─Token(Assignment) |=|
//@[037:0044) | └─VariableAccessSyntax
//@[037:0044) |   └─IdentifierSyntax
//@[037:0044) |     └─Token(Identifier) |unionAB|
//@[044:0046) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0095) ├─TypeDeclarationSyntax
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
type discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0046) | ├─IdentifierSyntax
//@[005:0046) | | └─Token(Identifier) |discriminatorPropertyNotExistOnAtLeastOne|
//@[047:0048) | ├─Token(Assignment) |=|
//@[049:0072) | └─UnionTypeSyntax
//@[049:0054) |   ├─UnionTypeMemberSyntax
//@[049:0054) |   | └─VariableAccessSyntax
//@[049:0054) |   |   └─IdentifierSyntax
//@[049:0054) |   |     └─Token(Identifier) |typeA|
//@[055:0056) |   ├─Token(Pipe) |||
//@[057:0072) |   └─UnionTypeMemberSyntax
//@[057:0072) |     └─ObjectTypeSyntax
//@[057:0058) |       ├─Token(LeftBrace) |{|
//@[059:0070) |       ├─ObjectTypePropertySyntax
//@[059:0064) |       | ├─IdentifierSyntax
//@[059:0064) |       | | └─Token(Identifier) |value|
//@[064:0065) |       | ├─Token(Colon) |:|
//@[066:0070) |       | └─VariableAccessSyntax
//@[066:0070) |       |   └─IdentifierSyntax
//@[066:0070) |       |     └─Token(Identifier) |bool|
//@[071:0072) |       └─Token(RightBrace) |}|
//@[072:0074) ├─Token(NewLine) |\n\n|

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
type discriminatorWithOnlyOneMember = typeA
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0035) | ├─IdentifierSyntax
//@[005:0035) | | └─Token(Identifier) |discriminatorWithOnlyOneMember|
//@[036:0037) | ├─Token(Assignment) |=|
//@[038:0043) | └─VariableAccessSyntax
//@[038:0043) |   └─IdentifierSyntax
//@[038:0043) |     └─Token(Identifier) |typeA|
//@[043:0045) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0090) ├─TypeDeclarationSyntax
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
type discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0051) | ├─IdentifierSyntax
//@[005:0051) | | └─Token(Identifier) |discriminatorPropertyNotRequiredStringLiteral1|
//@[052:0053) | ├─Token(Assignment) |=|
//@[054:0067) | └─UnionTypeSyntax
//@[054:0059) |   ├─UnionTypeMemberSyntax
//@[054:0059) |   | └─VariableAccessSyntax
//@[054:0059) |   |   └─IdentifierSyntax
//@[054:0059) |   |     └─Token(Identifier) |typeA|
//@[060:0061) |   ├─Token(Pipe) |||
//@[062:0067) |   └─UnionTypeMemberSyntax
//@[062:0067) |     └─VariableAccessSyntax
//@[062:0067) |       └─IdentifierSyntax
//@[062:0067) |         └─Token(Identifier) |typeF|
//@[067:0069) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0090) ├─TypeDeclarationSyntax
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
type discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0051) | ├─IdentifierSyntax
//@[005:0051) | | └─Token(Identifier) |discriminatorPropertyNotRequiredStringLiteral2|
//@[052:0053) | ├─Token(Assignment) |=|
//@[054:0067) | └─UnionTypeSyntax
//@[054:0059) |   ├─UnionTypeMemberSyntax
//@[054:0059) |   | └─VariableAccessSyntax
//@[054:0059) |   |   └─IdentifierSyntax
//@[054:0059) |   |     └─Token(Identifier) |typeA|
//@[060:0061) |   ├─Token(Pipe) |||
//@[062:0067) |   └─UnionTypeMemberSyntax
//@[062:0067) |     └─VariableAccessSyntax
//@[062:0067) |       └─IdentifierSyntax
//@[062:0067) |         └─Token(Identifier) |typeG|
//@[067:0069) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0074) ├─TypeDeclarationSyntax
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
type discriminatorDuplicatedMember1 = typeA | typeA
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0035) | ├─IdentifierSyntax
//@[005:0035) | | └─Token(Identifier) |discriminatorDuplicatedMember1|
//@[036:0037) | ├─Token(Assignment) |=|
//@[038:0051) | └─UnionTypeSyntax
//@[038:0043) |   ├─UnionTypeMemberSyntax
//@[038:0043) |   | └─VariableAccessSyntax
//@[038:0043) |   |   └─IdentifierSyntax
//@[038:0043) |   |     └─Token(Identifier) |typeA|
//@[044:0045) |   ├─Token(Pipe) |||
//@[046:0051) |   └─UnionTypeMemberSyntax
//@[046:0051) |     └─VariableAccessSyntax
//@[046:0051) |       └─IdentifierSyntax
//@[046:0051) |         └─Token(Identifier) |typeA|
//@[051:0053) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0098) ├─TypeDeclarationSyntax
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
type discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0035) | ├─IdentifierSyntax
//@[005:0035) | | └─Token(Identifier) |discriminatorDuplicatedMember2|
//@[036:0037) | ├─Token(Assignment) |=|
//@[038:0075) | └─UnionTypeSyntax
//@[038:0043) |   ├─UnionTypeMemberSyntax
//@[038:0043) |   | └─VariableAccessSyntax
//@[038:0043) |   |   └─IdentifierSyntax
//@[038:0043) |   |     └─Token(Identifier) |typeA|
//@[044:0045) |   ├─Token(Pipe) |||
//@[046:0075) |   └─UnionTypeMemberSyntax
//@[046:0075) |     └─ObjectTypeSyntax
//@[046:0047) |       ├─Token(LeftBrace) |{|
//@[048:0057) |       ├─ObjectTypePropertySyntax
//@[048:0052) |       | ├─IdentifierSyntax
//@[048:0052) |       | | └─Token(Identifier) |type|
//@[052:0053) |       | ├─Token(Colon) |:|
//@[054:0057) |       | └─StringSyntax
//@[054:0057) |       |   └─Token(StringComplete) |'a'|
//@[057:0058) |       ├─Token(Comma) |,|
//@[059:0073) |       ├─ObjectTypePropertySyntax
//@[059:0065) |       | ├─IdentifierSyntax
//@[059:0065) |       | | └─Token(Identifier) |config|
//@[065:0066) |       | ├─Token(Colon) |:|
//@[067:0073) |       | └─VariableAccessSyntax
//@[067:0073) |       |   └─IdentifierSyntax
//@[067:0073) |       |     └─Token(Identifier) |object|
//@[074:0075) |       └─Token(RightBrace) |}|
//@[075:0077) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0077) ├─TypeDeclarationSyntax
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
type discriminatorOnlyOneNonNullMember1 = typeA | null
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0039) | ├─IdentifierSyntax
//@[005:0039) | | └─Token(Identifier) |discriminatorOnlyOneNonNullMember1|
//@[040:0041) | ├─Token(Assignment) |=|
//@[042:0054) | └─UnionTypeSyntax
//@[042:0047) |   ├─UnionTypeMemberSyntax
//@[042:0047) |   | └─VariableAccessSyntax
//@[042:0047) |   |   └─IdentifierSyntax
//@[042:0047) |   |     └─Token(Identifier) |typeA|
//@[048:0049) |   ├─Token(Pipe) |||
//@[050:0054) |   └─UnionTypeMemberSyntax
//@[050:0054) |     └─NullLiteralSyntax
//@[050:0054) |       └─Token(NullKeyword) |null|
//@[054:0056) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0073) ├─TypeDeclarationSyntax
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
type discriminatorOnlyOneNonNullMember2 = (typeA)?
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0039) | ├─IdentifierSyntax
//@[005:0039) | | └─Token(Identifier) |discriminatorOnlyOneNonNullMember2|
//@[040:0041) | ├─Token(Assignment) |=|
//@[042:0050) | └─NullableTypeSyntax
//@[042:0049) |   ├─ParenthesizedExpressionSyntax
//@[042:0043) |   | ├─Token(LeftParen) |(|
//@[043:0048) |   | ├─VariableAccessSyntax
//@[043:0048) |   | | └─IdentifierSyntax
//@[043:0048) |   | |   └─Token(Identifier) |typeA|
//@[048:0049) |   | └─Token(RightParen) |)|
//@[049:0050) |   └─Token(Question) |?|
//@[050:0052) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0086) ├─TypeDeclarationSyntax
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
type discriminatorMemberHasAdditionalProperties = typeA | typeE
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0047) | ├─IdentifierSyntax
//@[005:0047) | | └─Token(Identifier) |discriminatorMemberHasAdditionalProperties|
//@[048:0049) | ├─Token(Assignment) |=|
//@[050:0063) | └─UnionTypeSyntax
//@[050:0055) |   ├─UnionTypeMemberSyntax
//@[050:0055) |   | └─VariableAccessSyntax
//@[050:0055) |   |   └─IdentifierSyntax
//@[050:0055) |   |     └─Token(Identifier) |typeA|
//@[056:0057) |   ├─Token(Pipe) |||
//@[058:0063) |   └─UnionTypeMemberSyntax
//@[058:0063) |     └─VariableAccessSyntax
//@[058:0063) |       └─IdentifierSyntax
//@[058:0063) |         └─Token(Identifier) |typeE|
//@[063:0065) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0083) ├─TypeDeclarationSyntax
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
type discriminatorSelfCycle = typeA | discriminatorSelfCycle
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0027) | ├─IdentifierSyntax
//@[005:0027) | | └─Token(Identifier) |discriminatorSelfCycle|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0060) | └─UnionTypeSyntax
//@[030:0035) |   ├─UnionTypeMemberSyntax
//@[030:0035) |   | └─VariableAccessSyntax
//@[030:0035) |   |   └─IdentifierSyntax
//@[030:0035) |   |     └─Token(Identifier) |typeA|
//@[036:0037) |   ├─Token(Pipe) |||
//@[038:0060) |   └─UnionTypeMemberSyntax
//@[038:0060) |     └─VariableAccessSyntax
//@[038:0060) |       └─IdentifierSyntax
//@[038:0060) |         └─Token(Identifier) |discriminatorSelfCycle|
//@[060:0062) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0093) ├─TypeDeclarationSyntax
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
type discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0032) | ├─IdentifierSyntax
//@[005:0032) | | └─Token(Identifier) |discriminatorTopLevelCycleA|
//@[033:0034) | ├─Token(Assignment) |=|
//@[035:0070) | └─UnionTypeSyntax
//@[035:0040) |   ├─UnionTypeMemberSyntax
//@[035:0040) |   | └─VariableAccessSyntax
//@[035:0040) |   |   └─IdentifierSyntax
//@[035:0040) |   |     └─Token(Identifier) |typeA|
//@[041:0042) |   ├─Token(Pipe) |||
//@[043:0070) |   └─UnionTypeMemberSyntax
//@[043:0070) |     └─VariableAccessSyntax
//@[043:0070) |       └─IdentifierSyntax
//@[043:0070) |         └─Token(Identifier) |discriminatorTopLevelCycleB|
//@[070:0071) ├─Token(NewLine) |\n|
@discriminator('type')
//@[000:0093) ├─TypeDeclarationSyntax
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
type discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0032) | ├─IdentifierSyntax
//@[005:0032) | | └─Token(Identifier) |discriminatorTopLevelCycleB|
//@[033:0034) | ├─Token(Assignment) |=|
//@[035:0070) | └─UnionTypeSyntax
//@[035:0040) |   ├─UnionTypeMemberSyntax
//@[035:0040) |   | └─VariableAccessSyntax
//@[035:0040) |   |   └─IdentifierSyntax
//@[035:0040) |   |     └─Token(Identifier) |typeB|
//@[041:0042) |   ├─Token(Pipe) |||
//@[043:0070) |   └─UnionTypeMemberSyntax
//@[043:0070) |     └─VariableAccessSyntax
//@[043:0070) |       └─IdentifierSyntax
//@[043:0070) |         └─Token(Identifier) |discriminatorTopLevelCycleA|
//@[070:0072) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0120) ├─TypeDeclarationSyntax
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
type discriminatorInnerSelfCycle1 = typeA | {
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0033) | ├─IdentifierSyntax
//@[005:0033) | | └─Token(Identifier) |discriminatorInnerSelfCycle1|
//@[034:0035) | ├─Token(Assignment) |=|
//@[036:0097) | └─UnionTypeSyntax
//@[036:0041) |   ├─UnionTypeMemberSyntax
//@[036:0041) |   | └─VariableAccessSyntax
//@[036:0041) |   |   └─IdentifierSyntax
//@[036:0041) |   |     └─Token(Identifier) |typeA|
//@[042:0043) |   ├─Token(Pipe) |||
//@[044:0097) |   └─UnionTypeMemberSyntax
//@[044:0097) |     └─ObjectTypeSyntax
//@[044:0045) |       ├─Token(LeftBrace) |{|
//@[045:0046) |       ├─Token(NewLine) |\n|
  type: 'b'
//@[002:0011) |       ├─ObjectTypePropertySyntax
//@[002:0006) |       | ├─IdentifierSyntax
//@[002:0006) |       | | └─Token(Identifier) |type|
//@[006:0007) |       | ├─Token(Colon) |:|
//@[008:0011) |       | └─StringSyntax
//@[008:0011) |       |   └─Token(StringComplete) |'b'|
//@[011:0012) |       ├─Token(NewLine) |\n|
  value: discriminatorInnerSelfCycle1
//@[002:0037) |       ├─ObjectTypePropertySyntax
//@[002:0007) |       | ├─IdentifierSyntax
//@[002:0007) |       | | └─Token(Identifier) |value|
//@[007:0008) |       | ├─Token(Colon) |:|
//@[009:0037) |       | └─VariableAccessSyntax
//@[009:0037) |       |   └─IdentifierSyntax
//@[009:0037) |       |     └─Token(Identifier) |discriminatorInnerSelfCycle1|
//@[037:0038) |       ├─Token(NewLine) |\n|
}
//@[000:0001) |       └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatorInnerSelfCycle2Helper = {
//@[000:0095) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0039) | ├─IdentifierSyntax
//@[005:0039) | | └─Token(Identifier) |discriminatorInnerSelfCycle2Helper|
//@[040:0041) | ├─Token(Assignment) |=|
//@[042:0095) | └─ObjectTypeSyntax
//@[042:0043) |   ├─Token(LeftBrace) |{|
//@[043:0044) |   ├─Token(NewLine) |\n|
  type: 'b'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'b'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  value: discriminatorInnerSelfCycle2
//@[002:0037) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |value|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0037) |   | └─VariableAccessSyntax
//@[009:0037) |   |   └─IdentifierSyntax
//@[009:0037) |   |     └─Token(Identifier) |discriminatorInnerSelfCycle2|
//@[037:0038) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
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
type discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0033) | ├─IdentifierSyntax
//@[005:0033) | | └─Token(Identifier) |discriminatorInnerSelfCycle2|
//@[034:0035) | ├─Token(Assignment) |=|
//@[036:0078) | └─UnionTypeSyntax
//@[036:0041) |   ├─UnionTypeMemberSyntax
//@[036:0041) |   | └─VariableAccessSyntax
//@[036:0041) |   |   └─IdentifierSyntax
//@[036:0041) |   |     └─Token(Identifier) |typeA|
//@[042:0043) |   ├─Token(Pipe) |||
//@[044:0078) |   └─UnionTypeMemberSyntax
//@[044:0078) |     └─VariableAccessSyntax
//@[044:0078) |       └─IdentifierSyntax
//@[044:0078) |         └─Token(Identifier) |discriminatorInnerSelfCycle2Helper|
//@[078:0080) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0071) ├─TypeDeclarationSyntax
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
type discriminatorTupleBadType1 = [typeA, typeB]
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0031) | ├─IdentifierSyntax
//@[005:0031) | | └─Token(Identifier) |discriminatorTupleBadType1|
//@[032:0033) | ├─Token(Assignment) |=|
//@[034:0048) | └─TupleTypeSyntax
//@[034:0035) |   ├─Token(LeftSquare) |[|
//@[035:0040) |   ├─TupleTypeItemSyntax
//@[035:0040) |   | └─VariableAccessSyntax
//@[035:0040) |   |   └─IdentifierSyntax
//@[035:0040) |   |     └─Token(Identifier) |typeA|
//@[040:0041) |   ├─Token(Comma) |,|
//@[042:0047) |   ├─TupleTypeItemSyntax
//@[042:0047) |   | └─VariableAccessSyntax
//@[042:0047) |   |   └─IdentifierSyntax
//@[042:0047) |   |     └─Token(Identifier) |typeB|
//@[047:0048) |   └─Token(RightSquare) |]|
//@[048:0050) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0072) ├─TypeDeclarationSyntax
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
type discriminatorTupleBadType2 = [typeA | typeB]
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0031) | ├─IdentifierSyntax
//@[005:0031) | | └─Token(Identifier) |discriminatorTupleBadType2|
//@[032:0033) | ├─Token(Assignment) |=|
//@[034:0049) | └─TupleTypeSyntax
//@[034:0035) |   ├─Token(LeftSquare) |[|
//@[035:0048) |   ├─TupleTypeItemSyntax
//@[035:0048) |   | └─UnionTypeSyntax
//@[035:0040) |   |   ├─UnionTypeMemberSyntax
//@[035:0040) |   |   | └─VariableAccessSyntax
//@[035:0040) |   |   |   └─IdentifierSyntax
//@[035:0040) |   |   |     └─Token(Identifier) |typeA|
//@[041:0042) |   |   ├─Token(Pipe) |||
//@[043:0048) |   |   └─UnionTypeMemberSyntax
//@[043:0048) |   |     └─VariableAccessSyntax
//@[043:0048) |   |       └─IdentifierSyntax
//@[043:0048) |   |         └─Token(Identifier) |typeB|
//@[048:0049) |   └─Token(RightSquare) |]|
//@[049:0051) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0087) ├─TypeDeclarationSyntax
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
type discriminatorTupleBadType3 = [typeA | typeB, typeC | typeD]
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0031) | ├─IdentifierSyntax
//@[005:0031) | | └─Token(Identifier) |discriminatorTupleBadType3|
//@[032:0033) | ├─Token(Assignment) |=|
//@[034:0064) | └─TupleTypeSyntax
//@[034:0035) |   ├─Token(LeftSquare) |[|
//@[035:0048) |   ├─TupleTypeItemSyntax
//@[035:0048) |   | └─UnionTypeSyntax
//@[035:0040) |   |   ├─UnionTypeMemberSyntax
//@[035:0040) |   |   | └─VariableAccessSyntax
//@[035:0040) |   |   |   └─IdentifierSyntax
//@[035:0040) |   |   |     └─Token(Identifier) |typeA|
//@[041:0042) |   |   ├─Token(Pipe) |||
//@[043:0048) |   |   └─UnionTypeMemberSyntax
//@[043:0048) |   |     └─VariableAccessSyntax
//@[043:0048) |   |       └─IdentifierSyntax
//@[043:0048) |   |         └─Token(Identifier) |typeB|
//@[048:0049) |   ├─Token(Comma) |,|
//@[050:0063) |   ├─TupleTypeItemSyntax
//@[050:0063) |   | └─UnionTypeSyntax
//@[050:0055) |   |   ├─UnionTypeMemberSyntax
//@[050:0055) |   |   | └─VariableAccessSyntax
//@[050:0055) |   |   |   └─IdentifierSyntax
//@[050:0055) |   |   |     └─Token(Identifier) |typeC|
//@[056:0057) |   |   ├─Token(Pipe) |||
//@[058:0063) |   |   └─UnionTypeMemberSyntax
//@[058:0063) |   |     └─VariableAccessSyntax
//@[058:0063) |   |       └─IdentifierSyntax
//@[058:0063) |   |         └─Token(Identifier) |typeD|
//@[063:0064) |   └─Token(RightSquare) |]|
//@[064:0066) ├─Token(NewLine) |\n\n|

type discriminatorInlineAdditionalPropsBadType1 = {
//@[000:0089) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0047) | ├─IdentifierSyntax
//@[005:0047) | | └─Token(Identifier) |discriminatorInlineAdditionalPropsBadType1|
//@[048:0049) | ├─Token(Assignment) |=|
//@[050:0089) | └─ObjectTypeSyntax
//@[050:0051) |   ├─Token(LeftBrace) |{|
//@[051:0052) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0035) |   ├─ObjectTypeAdditionalPropertiesSyntax
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
  *: typeA
//@[002:0003) |   | ├─Token(Asterisk) |*|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0010) |   | └─VariableAccessSyntax
//@[005:0010) |   |   └─IdentifierSyntax
//@[005:0010) |   |     └─Token(Identifier) |typeA|
//@[010:0011) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatorInlineAdditionalPropsBadType2 = {
//@[000:0097) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0047) | ├─IdentifierSyntax
//@[005:0047) | | └─Token(Identifier) |discriminatorInlineAdditionalPropsBadType2|
//@[048:0049) | ├─Token(Assignment) |=|
//@[050:0097) | └─ObjectTypeSyntax
//@[050:0051) |   ├─Token(LeftBrace) |{|
//@[051:0052) |   ├─Token(NewLine) |\n|
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
  *: typeA | typeA
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
//@[013:0018) |   |         └─Token(Identifier) |typeA|
//@[018:0019) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatorInlineAdditionalPropsBadType3 = {
//@[000:0090) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0047) | ├─IdentifierSyntax
//@[005:0047) | | └─Token(Identifier) |discriminatorInlineAdditionalPropsBadType3|
//@[048:0049) | ├─Token(Assignment) |=|
//@[050:0090) | └─ObjectTypeSyntax
//@[050:0051) |   ├─Token(LeftBrace) |{|
//@[051:0052) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0036) |   ├─ObjectTypeAdditionalPropertiesSyntax
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
  *: string
//@[002:0003) |   | ├─Token(Asterisk) |*|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0011) |   | └─VariableAccessSyntax
//@[005:0011) |   |   └─IdentifierSyntax
//@[005:0011) |   |     └─Token(Identifier) |string|
//@[011:0012) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type discriminatorInlineAdditionalPropsCycle1 = {
//@[000:0142) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0045) | ├─IdentifierSyntax
//@[005:0045) | | └─Token(Identifier) |discriminatorInlineAdditionalPropsCycle1|
//@[046:0047) | ├─Token(Assignment) |=|
//@[048:0142) | └─ObjectTypeSyntax
//@[048:0049) |   ├─Token(LeftBrace) |{|
//@[049:0050) |   ├─Token(NewLine) |\n|
  type: 'b'
//@[002:0011) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |type|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0011) |   | └─StringSyntax
//@[008:0011) |   |   └─Token(StringComplete) |'b'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  @discriminator('type')
//@[002:0078) |   ├─ObjectTypeAdditionalPropertiesSyntax
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
  *: typeA | discriminatorInlineAdditionalPropsCycle1
//@[002:0003) |   | ├─Token(Asterisk) |*|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0053) |   | └─UnionTypeSyntax
//@[005:0010) |   |   ├─UnionTypeMemberSyntax
//@[005:0010) |   |   | └─VariableAccessSyntax
//@[005:0010) |   |   |   └─IdentifierSyntax
//@[005:0010) |   |   |     └─Token(Identifier) |typeA|
//@[011:0012) |   |   ├─Token(Pipe) |||
//@[013:0053) |   |   └─UnionTypeMemberSyntax
//@[013:0053) |   |     └─VariableAccessSyntax
//@[013:0053) |   |       └─IdentifierSyntax
//@[013:0053) |   |         └─Token(Identifier) |discriminatorInlineAdditionalPropsCycle1|
//@[053:0054) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0131) ├─TypeDeclarationSyntax
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
type discriminatedUnionDuplicateMemberInsensitive = { type: 'a', value: string } | { type: 'A', value: int }
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0049) | ├─IdentifierSyntax
//@[005:0049) | | └─Token(Identifier) |discriminatedUnionDuplicateMemberInsensitive|
//@[050:0051) | ├─Token(Assignment) |=|
//@[052:0108) | └─UnionTypeSyntax
//@[052:0080) |   ├─UnionTypeMemberSyntax
//@[052:0080) |   | └─ObjectTypeSyntax
//@[052:0053) |   |   ├─Token(LeftBrace) |{|
//@[054:0063) |   |   ├─ObjectTypePropertySyntax
//@[054:0058) |   |   | ├─IdentifierSyntax
//@[054:0058) |   |   | | └─Token(Identifier) |type|
//@[058:0059) |   |   | ├─Token(Colon) |:|
//@[060:0063) |   |   | └─StringSyntax
//@[060:0063) |   |   |   └─Token(StringComplete) |'a'|
//@[063:0064) |   |   ├─Token(Comma) |,|
//@[065:0078) |   |   ├─ObjectTypePropertySyntax
//@[065:0070) |   |   | ├─IdentifierSyntax
//@[065:0070) |   |   | | └─Token(Identifier) |value|
//@[070:0071) |   |   | ├─Token(Colon) |:|
//@[072:0078) |   |   | └─VariableAccessSyntax
//@[072:0078) |   |   |   └─IdentifierSyntax
//@[072:0078) |   |   |     └─Token(Identifier) |string|
//@[079:0080) |   |   └─Token(RightBrace) |}|
//@[081:0082) |   ├─Token(Pipe) |||
//@[083:0108) |   └─UnionTypeMemberSyntax
//@[083:0108) |     └─ObjectTypeSyntax
//@[083:0084) |       ├─Token(LeftBrace) |{|
//@[085:0094) |       ├─ObjectTypePropertySyntax
//@[085:0089) |       | ├─IdentifierSyntax
//@[085:0089) |       | | └─Token(Identifier) |type|
//@[089:0090) |       | ├─Token(Colon) |:|
//@[091:0094) |       | └─StringSyntax
//@[091:0094) |       |   └─Token(StringComplete) |'A'|
//@[094:0095) |       ├─Token(Comma) |,|
//@[096:0106) |       ├─ObjectTypePropertySyntax
//@[096:0101) |       | ├─IdentifierSyntax
//@[096:0101) |       | | └─Token(Identifier) |value|
//@[101:0102) |       | ├─Token(Colon) |:|
//@[103:0106) |       | └─VariableAccessSyntax
//@[103:0106) |       |   └─IdentifierSyntax
//@[103:0106) |       |     └─Token(Identifier) |int|
//@[107:0108) |       └─Token(RightBrace) |}|
//@[108:0110) ├─Token(NewLine) |\n\n|

@discriminator('TYPE')
//@[000:0131) ├─TypeDeclarationSyntax
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0014) | |   ├─IdentifierSyntax
//@[001:0014) | |   | └─Token(Identifier) |discriminator|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0021) | |   ├─FunctionArgumentSyntax
//@[015:0021) | |   | └─StringSyntax
//@[015:0021) | |   |   └─Token(StringComplete) |'TYPE'|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
type discriminatedUnionCaseSensitiveDiscriminator = { type: 'a', value: string } | { type: 'b', value: int }
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0049) | ├─IdentifierSyntax
//@[005:0049) | | └─Token(Identifier) |discriminatedUnionCaseSensitiveDiscriminator|
//@[050:0051) | ├─Token(Assignment) |=|
//@[052:0108) | └─UnionTypeSyntax
//@[052:0080) |   ├─UnionTypeMemberSyntax
//@[052:0080) |   | └─ObjectTypeSyntax
//@[052:0053) |   |   ├─Token(LeftBrace) |{|
//@[054:0063) |   |   ├─ObjectTypePropertySyntax
//@[054:0058) |   |   | ├─IdentifierSyntax
//@[054:0058) |   |   | | └─Token(Identifier) |type|
//@[058:0059) |   |   | ├─Token(Colon) |:|
//@[060:0063) |   |   | └─StringSyntax
//@[060:0063) |   |   |   └─Token(StringComplete) |'a'|
//@[063:0064) |   |   ├─Token(Comma) |,|
//@[065:0078) |   |   ├─ObjectTypePropertySyntax
//@[065:0070) |   |   | ├─IdentifierSyntax
//@[065:0070) |   |   | | └─Token(Identifier) |value|
//@[070:0071) |   |   | ├─Token(Colon) |:|
//@[072:0078) |   |   | └─VariableAccessSyntax
//@[072:0078) |   |   |   └─IdentifierSyntax
//@[072:0078) |   |   |     └─Token(Identifier) |string|
//@[079:0080) |   |   └─Token(RightBrace) |}|
//@[081:0082) |   ├─Token(Pipe) |||
//@[083:0108) |   └─UnionTypeMemberSyntax
//@[083:0108) |     └─ObjectTypeSyntax
//@[083:0084) |       ├─Token(LeftBrace) |{|
//@[085:0094) |       ├─ObjectTypePropertySyntax
//@[085:0089) |       | ├─IdentifierSyntax
//@[085:0089) |       | | └─Token(Identifier) |type|
//@[089:0090) |       | ├─Token(Colon) |:|
//@[091:0094) |       | └─StringSyntax
//@[091:0094) |       |   └─Token(StringComplete) |'b'|
//@[094:0095) |       ├─Token(Comma) |,|
//@[096:0106) |       ├─ObjectTypePropertySyntax
//@[096:0101) |       | ├─IdentifierSyntax
//@[096:0101) |       | | └─Token(Identifier) |value|
//@[101:0102) |       | ├─Token(Colon) |:|
//@[103:0106) |       | └─VariableAccessSyntax
//@[103:0106) |       |   └─IdentifierSyntax
//@[103:0106) |       |     └─Token(Identifier) |int|
//@[107:0108) |       └─Token(RightBrace) |}|
//@[108:0110) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0061) ├─ParameterDeclarationSyntax
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
param discriminatorParamBadType1 typeA
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0032) | ├─IdentifierSyntax
//@[006:0032) | | └─Token(Identifier) |discriminatorParamBadType1|
//@[033:0038) | └─VariableAccessSyntax
//@[033:0038) |   └─IdentifierSyntax
//@[033:0038) |     └─Token(Identifier) |typeA|
//@[038:0040) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0065) ├─ParameterDeclarationSyntax
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
param discriminatorParamBadType2 'a' | 'b'
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0032) | ├─IdentifierSyntax
//@[006:0032) | | └─Token(Identifier) |discriminatorParamBadType2|
//@[033:0042) | └─UnionTypeSyntax
//@[033:0036) |   ├─UnionTypeMemberSyntax
//@[033:0036) |   | └─StringSyntax
//@[033:0036) |   |   └─Token(StringComplete) |'a'|
//@[037:0038) |   ├─Token(Pipe) |||
//@[039:0042) |   └─UnionTypeMemberSyntax
//@[039:0042) |     └─StringSyntax
//@[039:0042) |       └─Token(StringComplete) |'b'|
//@[042:0044) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0091) ├─OutputDeclarationSyntax
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
output discriminatorOutputBadType1 typeA = { type: 'a', value: 'a' }
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0034) | ├─IdentifierSyntax
//@[007:0034) | | └─Token(Identifier) |discriminatorOutputBadType1|
//@[035:0040) | ├─VariableAccessSyntax
//@[035:0040) | | └─IdentifierSyntax
//@[035:0040) | |   └─Token(Identifier) |typeA|
//@[041:0042) | ├─Token(Assignment) |=|
//@[043:0068) | └─ObjectSyntax
//@[043:0044) |   ├─Token(LeftBrace) |{|
//@[045:0054) |   ├─ObjectPropertySyntax
//@[045:0049) |   | ├─IdentifierSyntax
//@[045:0049) |   | | └─Token(Identifier) |type|
//@[049:0050) |   | ├─Token(Colon) |:|
//@[051:0054) |   | └─StringSyntax
//@[051:0054) |   |   └─Token(StringComplete) |'a'|
//@[054:0055) |   ├─Token(Comma) |,|
//@[056:0066) |   ├─ObjectPropertySyntax
//@[056:0061) |   | ├─IdentifierSyntax
//@[056:0061) |   | | └─Token(Identifier) |value|
//@[061:0062) |   | ├─Token(Colon) |:|
//@[063:0066) |   | └─StringSyntax
//@[063:0066) |   |   └─Token(StringComplete) |'a'|
//@[067:0068) |   └─Token(RightBrace) |}|
//@[068:0070) ├─Token(NewLine) |\n\n|

@discriminator('type')
//@[000:0084) ├─OutputDeclarationSyntax
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
output discriminatorOutputBadType2 object = { prop: 'value' }
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0034) | ├─IdentifierSyntax
//@[007:0034) | | └─Token(Identifier) |discriminatorOutputBadType2|
//@[035:0041) | ├─VariableAccessSyntax
//@[035:0041) | | └─IdentifierSyntax
//@[035:0041) | |   └─Token(Identifier) |object|
//@[042:0043) | ├─Token(Assignment) |=|
//@[044:0061) | └─ObjectSyntax
//@[044:0045) |   ├─Token(LeftBrace) |{|
//@[046:0059) |   ├─ObjectPropertySyntax
//@[046:0050) |   | ├─IdentifierSyntax
//@[046:0050) |   | | └─Token(Identifier) |prop|
//@[050:0051) |   | ├─Token(Colon) |:|
//@[052:0059) |   | └─StringSyntax
//@[052:0059) |   |   └─Token(StringComplete) |'value'|
//@[060:0061) |   └─Token(RightBrace) |}|
//@[061:0062) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
