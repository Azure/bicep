/* 
//@[000:6034) ProgramSyntax
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/
//@[002:0004) ├─Token(NewLine) |\n\n|

param myString string
//@[000:0021) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0014) | ├─IdentifierSyntax
//@[006:0014) | | └─Token(Identifier) |myString|
//@[015:0021) | └─SimpleTypeSyntax
//@[015:0021) |   └─Token(Identifier) |string|
//@[021:0022) ├─Token(NewLine) |\n|
wrong
//@[000:0005) ├─SkippedTriviaSyntax
//@[000:0005) | └─Token(Identifier) |wrong|
//@[005:0007) ├─Token(NewLine) |\n\n|

param myInt int
//@[000:0015) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0011) | ├─IdentifierSyntax
//@[006:0011) | | └─Token(Identifier) |myInt|
//@[012:0015) | └─SimpleTypeSyntax
//@[012:0015) |   └─Token(Identifier) |int|
//@[015:0016) ├─Token(NewLine) |\n|
param
//@[000:0005) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[005:0005) | ├─IdentifierSyntax
//@[005:0005) | | └─SkippedTriviaSyntax
//@[005:0005) | └─SkippedTriviaSyntax
//@[005:0007) ├─Token(NewLine) |\n\n|

param 3
//@[000:0007) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0007) | ├─IdentifierSyntax
//@[006:0007) | | └─SkippedTriviaSyntax
//@[006:0007) | |   └─Token(Integer) |3|
//@[007:0007) | └─SkippedTriviaSyntax
//@[007:0008) ├─Token(NewLine) |\n|
param % string
//@[000:0014) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0007) | ├─IdentifierSyntax
//@[006:0007) | | └─SkippedTriviaSyntax
//@[006:0007) | |   └─Token(Modulo) |%|
//@[008:0014) | └─SimpleTypeSyntax
//@[008:0014) |   └─Token(Identifier) |string|
//@[014:0015) ├─Token(NewLine) |\n|
param % string 3 = 's'
//@[000:0022) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0007) | ├─IdentifierSyntax
//@[006:0007) | | └─SkippedTriviaSyntax
//@[006:0007) | |   └─Token(Modulo) |%|
//@[008:0014) | ├─SimpleTypeSyntax
//@[008:0014) | | └─Token(Identifier) |string|
//@[015:0022) | └─SkippedTriviaSyntax
//@[015:0016) |   ├─Token(Integer) |3|
//@[017:0018) |   ├─Token(Assignment) |=|
//@[019:0022) |   └─Token(StringComplete) |'s'|
//@[022:0024) ├─Token(NewLine) |\n\n|

param myBool bool
//@[000:0017) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0012) | ├─IdentifierSyntax
//@[006:0012) | | └─Token(Identifier) |myBool|
//@[013:0017) | └─SimpleTypeSyntax
//@[013:0017) |   └─Token(Identifier) |bool|
//@[017:0019) ├─Token(NewLine) |\n\n|

param missingType
//@[000:0017) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |missingType|
//@[017:0017) | └─SkippedTriviaSyntax
//@[017:0019) ├─Token(NewLine) |\n\n|

// space after identifier #completionTest(32) -> paramTypes
//@[059:0060) ├─Token(NewLine) |\n|
param missingTypeWithSpaceAfter 
//@[000:0032) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0031) | ├─IdentifierSyntax
//@[006:0031) | | └─Token(Identifier) |missingTypeWithSpaceAfter|
//@[032:0032) | └─SkippedTriviaSyntax
//@[032:0034) ├─Token(NewLine) |\n\n|

// tab after identifier #completionTest(30) -> paramTypes
//@[057:0058) ├─Token(NewLine) |\n|
param missingTypeWithTabAfter	
//@[000:0030) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0029) | ├─IdentifierSyntax
//@[006:0029) | | └─Token(Identifier) |missingTypeWithTabAfter|
//@[030:0030) | └─SkippedTriviaSyntax
//@[030:0032) ├─Token(NewLine) |\n\n|

// #completionTest(20) -> paramTypes
//@[036:0037) ├─Token(NewLine) |\n|
param trailingSpace  
//@[000:0021) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |trailingSpace|
//@[021:0021) | └─SkippedTriviaSyntax
//@[021:0023) ├─Token(NewLine) |\n\n|

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
//@[061:0062) ├─Token(NewLine) |\n|
param partialType str
//@[000:0021) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |partialType|
//@[018:0021) | └─SimpleTypeSyntax
//@[018:0021) |   └─Token(Identifier) |str|
//@[021:0023) ├─Token(NewLine) |\n\n|

param malformedType 44
//@[000:0022) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |malformedType|
//@[020:0022) | └─SkippedTriviaSyntax
//@[020:0022) |   └─Token(Integer) |44|
//@[022:0024) ├─Token(NewLine) |\n\n|

// malformed type but type check should still happen
//@[052:0053) ├─Token(NewLine) |\n|
param malformedType2 44 = f
//@[000:0027) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0020) | ├─IdentifierSyntax
//@[006:0020) | | └─Token(Identifier) |malformedType2|
//@[021:0023) | ├─SkippedTriviaSyntax
//@[021:0023) | | └─Token(Integer) |44|
//@[024:0027) | └─ParameterDefaultValueSyntax
//@[024:0025) |   ├─Token(Assignment) |=|
//@[026:0027) |   └─VariableAccessSyntax
//@[026:0027) |     └─IdentifierSyntax
//@[026:0027) |       └─Token(Identifier) |f|
//@[027:0029) ├─Token(NewLine) |\n\n|

// malformed type but type check should still happen
//@[052:0053) ├─Token(NewLine) |\n|
@secure('s')
//@[000:0039) ├─ParameterDeclarationSyntax
//@[000:0012) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0012) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0011) | |   ├─FunctionArgumentSyntax
//@[008:0011) | |   | └─StringSyntax
//@[008:0011) | |   |   └─Token(StringComplete) |'s'|
//@[011:0012) | |   └─Token(RightParen) |)|
//@[012:0013) | ├─Token(NewLine) |\n|
param malformedModifier 44
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |malformedModifier|
//@[024:0026) | └─SkippedTriviaSyntax
//@[024:0026) |   └─Token(Integer) |44|
//@[026:0028) ├─Token(NewLine) |\n\n|

param myString2 string = 'string value'
//@[000:0039) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |myString2|
//@[016:0022) | ├─SimpleTypeSyntax
//@[016:0022) | | └─Token(Identifier) |string|
//@[023:0039) | └─ParameterDefaultValueSyntax
//@[023:0024) |   ├─Token(Assignment) |=|
//@[025:0039) |   └─StringSyntax
//@[025:0039) |     └─Token(StringComplete) |'string value'|
//@[039:0041) ├─Token(NewLine) |\n\n|

param wrongDefaultValue string = 42
//@[000:0035) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |wrongDefaultValue|
//@[024:0030) | ├─SimpleTypeSyntax
//@[024:0030) | | └─Token(Identifier) |string|
//@[031:0035) | └─ParameterDefaultValueSyntax
//@[031:0032) |   ├─Token(Assignment) |=|
//@[033:0035) |   └─IntegerLiteralSyntax
//@[033:0035) |     └─Token(Integer) |42|
//@[035:0037) ├─Token(NewLine) |\n\n|

param myInt2 int = 42
//@[000:0021) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0012) | ├─IdentifierSyntax
//@[006:0012) | | └─Token(Identifier) |myInt2|
//@[013:0016) | ├─SimpleTypeSyntax
//@[013:0016) | | └─Token(Identifier) |int|
//@[017:0021) | └─ParameterDefaultValueSyntax
//@[017:0018) |   ├─Token(Assignment) |=|
//@[019:0021) |   └─IntegerLiteralSyntax
//@[019:0021) |     └─Token(Integer) |42|
//@[021:0022) ├─Token(NewLine) |\n|
param noValueAfterColon int =   
//@[000:0032) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |noValueAfterColon|
//@[024:0027) | ├─SimpleTypeSyntax
//@[024:0027) | | └─Token(Identifier) |int|
//@[028:0032) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[032:0032) |   └─SkippedTriviaSyntax
//@[032:0034) ├─Token(NewLine) |\n\n|

param myTruth bool = 'not a boolean'
//@[000:0036) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0013) | ├─IdentifierSyntax
//@[006:0013) | | └─Token(Identifier) |myTruth|
//@[014:0018) | ├─SimpleTypeSyntax
//@[014:0018) | | └─Token(Identifier) |bool|
//@[019:0036) | └─ParameterDefaultValueSyntax
//@[019:0020) |   ├─Token(Assignment) |=|
//@[021:0036) |   └─StringSyntax
//@[021:0036) |     └─Token(StringComplete) |'not a boolean'|
//@[036:0037) ├─Token(NewLine) |\n|
param myFalsehood bool = 'false'
//@[000:0032) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |myFalsehood|
//@[018:0022) | ├─SimpleTypeSyntax
//@[018:0022) | | └─Token(Identifier) |bool|
//@[023:0032) | └─ParameterDefaultValueSyntax
//@[023:0024) |   ├─Token(Assignment) |=|
//@[025:0032) |   └─StringSyntax
//@[025:0032) |     └─Token(StringComplete) |'false'|
//@[032:0034) ├─Token(NewLine) |\n\n|

param wrongAssignmentToken string: 'hello'
//@[000:0042) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |wrongAssignmentToken|
//@[027:0033) | ├─SimpleTypeSyntax
//@[027:0033) | | └─Token(Identifier) |string|
//@[033:0042) | └─SkippedTriviaSyntax
//@[033:0034) |   ├─Token(Colon) |:|
//@[035:0042) |   └─Token(StringComplete) |'hello'|
//@[042:0044) ├─Token(NewLine) |\n\n|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[000:0287) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0267) | ├─IdentifierSyntax
//@[006:0267) | | └─Token(Identifier) |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|
//@[268:0274) | ├─SimpleTypeSyntax
//@[268:0274) | | └─Token(Identifier) |string|
//@[275:0287) | └─ParameterDefaultValueSyntax
//@[275:0276) |   ├─Token(Assignment) |=|
//@[277:0287) |   └─StringSyntax
//@[277:0287) |     └─Token(StringComplete) |'why not?'|
//@[287:0289) ├─Token(NewLine) |\n\n|

// #completionTest(28,29) -> boolPlusSymbols
//@[044:0045) ├─Token(NewLine) |\n|
param boolCompletions bool = 
//@[000:0029) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |boolCompletions|
//@[022:0026) | ├─SimpleTypeSyntax
//@[022:0026) | | └─Token(Identifier) |bool|
//@[027:0029) | └─ParameterDefaultValueSyntax
//@[027:0028) |   ├─Token(Assignment) |=|
//@[029:0029) |   └─SkippedTriviaSyntax
//@[029:0031) ├─Token(NewLine) |\n\n|

// #completionTest(30,31) -> arrayPlusSymbols
//@[045:0046) ├─Token(NewLine) |\n|
param arrayCompletions array = 
//@[000:0031) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0022) | ├─IdentifierSyntax
//@[006:0022) | | └─Token(Identifier) |arrayCompletions|
//@[023:0028) | ├─SimpleTypeSyntax
//@[023:0028) | | └─Token(Identifier) |array|
//@[029:0031) | └─ParameterDefaultValueSyntax
//@[029:0030) |   ├─Token(Assignment) |=|
//@[031:0031) |   └─SkippedTriviaSyntax
//@[031:0033) ├─Token(NewLine) |\n\n|

// #completionTest(32,33) -> objectPlusSymbols
//@[046:0047) ├─Token(NewLine) |\n|
param objectCompletions object = 
//@[000:0033) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |objectCompletions|
//@[024:0030) | ├─SimpleTypeSyntax
//@[024:0030) | | └─Token(Identifier) |object|
//@[031:0033) | └─ParameterDefaultValueSyntax
//@[031:0032) |   ├─Token(Assignment) |=|
//@[033:0033) |   └─SkippedTriviaSyntax
//@[033:0035) ├─Token(NewLine) |\n\n|

// badly escaped string
//@[023:0024) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what's up doc?'
//@[000:0036) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0036) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0036) |   └─StringSyntax
//@[030:0036) |     └─Token(StringComplete) |'what'|
//@[036:0048) ├─SkippedTriviaSyntax
//@[036:0037) | ├─Token(Identifier) |s|
//@[038:0040) | ├─Token(Identifier) |up|
//@[041:0044) | ├─Token(Identifier) |doc|
//@[044:0045) | ├─Token(Question) |?|
//@[045:0046) | ├─Token(StringComplete) |'|
//@[046:0048) | └─Token(NewLine) |\n\n|

// invalid escape
//@[017:0018) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\s up doc?'
//@[000:0046) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0046) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0046) |   └─SkippedTriviaSyntax
//@[030:0046) |     └─Token(StringComplete) |'what\s up doc?'|
//@[046:0048) ├─Token(NewLine) |\n\n|

// unterminated string 
//@[023:0024) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s up doc?
//@[000:0046) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0046) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0046) |   └─SkippedTriviaSyntax
//@[030:0046) |     └─Token(StringComplete) |'what\'s up doc?|
//@[046:0048) ├─Token(NewLine) |\n\n|

// unterminated interpolated string
//@[035:0036) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${
//@[000:0041) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0041) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0041) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0041) |     ├─SkippedTriviaSyntax
//@[041:0041) |     └─Token(StringRightPiece) ||
//@[041:0042) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${up
//@[000:0043) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0043) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0043) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0043) |     ├─VariableAccessSyntax
//@[041:0043) |     | └─IdentifierSyntax
//@[041:0043) |     |   └─Token(Identifier) |up|
//@[043:0043) |     └─Token(StringRightPiece) ||
//@[043:0044) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${up}
//@[000:0044) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0044) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0044) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0043) |     ├─VariableAccessSyntax
//@[041:0043) |     | └─IdentifierSyntax
//@[041:0043) |     |   └─Token(Identifier) |up|
//@[043:0044) |     └─Token(StringRightPiece) |}|
//@[044:0045) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${'up
//@[000:0044) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0044) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0044) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0044) |     ├─SkippedTriviaSyntax
//@[041:0044) |     | └─Token(StringComplete) |'up|
//@[044:0044) |     └─Token(StringRightPiece) ||
//@[044:0046) ├─Token(NewLine) |\n\n|

// unterminated nested interpolated string
//@[042:0043) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[000:0046) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0046) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0046) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0046) |     └─SkippedTriviaSyntax
//@[041:0046) |       ├─Token(StringLeftPiece) |'up${|
//@[046:0046) |       ├─SkippedTriviaSyntax
//@[046:0046) |       └─Token(StringRightPiece) ||
//@[046:0047) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[000:0046) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0046) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0046) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0046) |     └─SkippedTriviaSyntax
//@[041:0046) |       ├─Token(StringLeftPiece) |'up${|
//@[046:0046) |       ├─SkippedTriviaSyntax
//@[046:0046) |       └─Token(StringRightPiece) ||
//@[046:0047) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[000:0049) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0049) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0049) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0049) |     └─SkippedTriviaSyntax
//@[041:0046) |       ├─Token(StringLeftPiece) |'up${|
//@[046:0049) |       ├─VariableAccessSyntax
//@[046:0049) |       | └─IdentifierSyntax
//@[046:0049) |       |   └─Token(Identifier) |doc|
//@[049:0049) |       └─Token(StringRightPiece) ||
//@[049:0050) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[000:0050) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0050) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0050) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0050) |     ├─SkippedTriviaSyntax
//@[041:0046) |     | ├─Token(StringLeftPiece) |'up${|
//@[046:0049) |     | ├─VariableAccessSyntax
//@[046:0049) |     | | └─IdentifierSyntax
//@[046:0049) |     | |   └─Token(Identifier) |doc|
//@[049:0050) |     | └─Token(StringRightPiece) |}|
//@[050:0050) |     └─Token(StringRightPiece) ||
//@[050:0051) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[000:0051) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0051) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0051) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0051) |     ├─StringSyntax
//@[041:0046) |     | ├─Token(StringLeftPiece) |'up${|
//@[046:0049) |     | ├─VariableAccessSyntax
//@[046:0049) |     | | └─IdentifierSyntax
//@[046:0049) |     | |   └─Token(Identifier) |doc|
//@[049:0051) |     | └─Token(StringRightPiece) |}'|
//@[051:0051) |     └─Token(StringRightPiece) ||
//@[051:0052) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[000:0053) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0053) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0053) |   └─SkippedTriviaSyntax
//@[030:0041) |     ├─Token(StringLeftPiece) |'what\'s ${|
//@[041:0051) |     ├─StringSyntax
//@[041:0046) |     | ├─Token(StringLeftPiece) |'up${|
//@[046:0049) |     | ├─VariableAccessSyntax
//@[046:0049) |     | | └─IdentifierSyntax
//@[046:0049) |     | |   └─Token(Identifier) |doc|
//@[049:0051) |     | └─Token(StringRightPiece) |}'|
//@[051:0053) |     └─Token(StringRightPiece) |}?|
//@[053:0055) ├─Token(NewLine) |\n\n|

// object literal inside interpolated string
//@[044:0045) ├─Token(NewLine) |\n|
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[000:0054) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0054) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0054) |   └─SkippedTriviaSyntax
//@[030:0033) |     ├─Token(StringLeftPiece) |'${|
//@[033:0054) |     ├─SkippedTriviaSyntax
//@[033:0034) |     | ├─Token(LeftBrace) |{|
//@[034:0038) |     | ├─Token(Identifier) |this|
//@[038:0039) |     | ├─Token(Colon) |:|
//@[040:0046) |     | ├─Token(Identifier) |doesnt|
//@[046:0047) |     | ├─Token(RightBrace) |}|
//@[047:0048) |     | ├─Token(Dot) |.|
//@[048:0052) |     | ├─Token(Identifier) |work|
//@[052:0053) |     | ├─Token(RightBrace) |}|
//@[053:0054) |     | └─Token(StringComplete) |'|
//@[054:0054) |     └─Token(StringRightPiece) ||
//@[054:0056) ├─Token(NewLine) |\n\n|

// bad interpolated string format
//@[033:0034) ├─Token(NewLine) |\n|
param badInterpolatedString string = 'hello ${}!'
//@[000:0049) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0027) | ├─IdentifierSyntax
//@[006:0027) | | └─Token(Identifier) |badInterpolatedString|
//@[028:0034) | ├─SimpleTypeSyntax
//@[028:0034) | | └─Token(Identifier) |string|
//@[035:0049) | └─ParameterDefaultValueSyntax
//@[035:0036) |   ├─Token(Assignment) |=|
//@[037:0049) |   └─StringSyntax
//@[037:0046) |     ├─Token(StringLeftPiece) |'hello ${|
//@[046:0046) |     ├─SkippedTriviaSyntax
//@[046:0049) |     └─Token(StringRightPiece) |}!'|
//@[049:0050) ├─Token(NewLine) |\n|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[000:0055) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0028) | ├─IdentifierSyntax
//@[006:0028) | | └─Token(Identifier) |badInterpolatedString2|
//@[029:0035) | ├─SimpleTypeSyntax
//@[029:0035) | | └─Token(Identifier) |string|
//@[036:0055) | └─ParameterDefaultValueSyntax
//@[036:0037) |   ├─Token(Assignment) |=|
//@[038:0055) |   └─StringSyntax
//@[038:0047) |     ├─Token(StringLeftPiece) |'hello ${|
//@[047:0052) |     ├─SkippedTriviaSyntax
//@[047:0048) |     | ├─VariableAccessSyntax
//@[047:0048) |     | | └─IdentifierSyntax
//@[047:0048) |     | |   └─Token(Identifier) |a|
//@[049:0052) |     | └─SkippedTriviaSyntax
//@[049:0050) |     |   ├─Token(Identifier) |b|
//@[051:0052) |     |   └─Token(Identifier) |c|
//@[052:0055) |     └─Token(StringRightPiece) |}!'|
//@[055:0057) ├─Token(NewLine) |\n\n|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[000:0047) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |wrongType|
//@[016:0027) | ├─SimpleTypeSyntax
//@[016:0027) | | └─Token(Identifier) |fluffyBunny|
//@[028:0047) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0047) |   └─StringSyntax
//@[030:0047) |     └─Token(StringComplete) |'what\'s up doc?'|
//@[047:0049) ├─Token(NewLine) |\n\n|

// modifier on an invalid type
//@[030:0031) ├─Token(NewLine) |\n|
@minLength(3)
//@[000:0049) ├─ParameterDeclarationSyntax
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
@maxLength(24)
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0013) | |   ├─FunctionArgumentSyntax
//@[011:0013) | |   | └─IntegerLiteralSyntax
//@[011:0013) | |   |   └─Token(Integer) |24|
//@[013:0014) | |   └─Token(RightParen) |)|
//@[014:0015) | ├─Token(NewLine) |\n|
param someArray arra
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |someArray|
//@[016:0020) | └─SimpleTypeSyntax
//@[016:0020) |   └─Token(Identifier) |arra|
//@[020:0022) ├─Token(NewLine) |\n\n|

@secure()
//@[000:0059) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
@minLength(3)
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
@maxLength(123)
//@[000:0015) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0015) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0014) | |   ├─FunctionArgumentSyntax
//@[011:0014) | |   | └─IntegerLiteralSyntax
//@[011:0014) | |   |   └─Token(Integer) |123|
//@[014:0015) | |   └─Token(RightParen) |)|
//@[015:0016) | ├─Token(NewLine) |\n|
param secureInt int
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |secureInt|
//@[016:0019) | └─SimpleTypeSyntax
//@[016:0019) |   └─Token(Identifier) |int|
//@[019:0021) ├─Token(NewLine) |\n\n|

// wrong modifier value types
//@[029:0030) ├─Token(NewLine) |\n|
@allowed([
//@[000:0112) ├─ParameterDeclarationSyntax
//@[000:0029) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0029) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0028) | |   ├─FunctionArgumentSyntax
//@[009:0028) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  'test'
//@[002:0008) | |   |   ├─ArrayItemSyntax
//@[002:0008) | |   |   | └─StringSyntax
//@[002:0008) | |   |   |   └─Token(StringComplete) |'test'|
//@[008:0009) | |   |   ├─Token(NewLine) |\n|
  true
//@[002:0006) | |   |   ├─ArrayItemSyntax
//@[002:0006) | |   |   | └─BooleanLiteralSyntax
//@[002:0006) | |   |   |   └─Token(TrueKeyword) |true|
//@[006:0007) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
@minValue({
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0013) | |   ├─FunctionArgumentSyntax
//@[010:0013) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
@maxValue([
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |maxValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0013) | |   ├─FunctionArgumentSyntax
//@[010:0013) | |   | └─ArraySyntax
//@[010:0011) | |   |   ├─Token(LeftSquare) |[|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
@metadata('wrong')
//@[000:0018) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0018) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0017) | |   ├─FunctionArgumentSyntax
//@[010:0017) | |   | └─StringSyntax
//@[010:0017) | |   |   └─Token(StringComplete) |'wrong'|
//@[017:0018) | |   └─Token(RightParen) |)|
//@[018:0019) | ├─Token(NewLine) |\n|
param wrongIntModifier int = true
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0022) | ├─IdentifierSyntax
//@[006:0022) | | └─Token(Identifier) |wrongIntModifier|
//@[023:0026) | ├─SimpleTypeSyntax
//@[023:0026) | | └─Token(Identifier) |int|
//@[027:0033) | └─ParameterDefaultValueSyntax
//@[027:0028) |   ├─Token(Assignment) |=|
//@[029:0033) |   └─BooleanLiteralSyntax
//@[029:0033) |     └─Token(TrueKeyword) |true|
//@[033:0035) ├─Token(NewLine) |\n\n|

@metadata(any([]))
//@[000:0063) ├─ParameterDeclarationSyntax
//@[000:0018) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0018) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0017) | |   ├─FunctionArgumentSyntax
//@[010:0017) | |   | └─FunctionCallSyntax
//@[010:0013) | |   |   ├─IdentifierSyntax
//@[010:0013) | |   |   | └─Token(Identifier) |any|
//@[013:0014) | |   |   ├─Token(LeftParen) |(|
//@[014:0016) | |   |   ├─FunctionArgumentSyntax
//@[014:0016) | |   |   | └─ArraySyntax
//@[014:0015) | |   |   |   ├─Token(LeftSquare) |[|
//@[015:0016) | |   |   |   └─Token(RightSquare) |]|
//@[016:0017) | |   |   └─Token(RightParen) |)|
//@[017:0018) | |   └─Token(RightParen) |)|
//@[018:0019) | ├─Token(NewLine) |\n|
@allowed(any(2))
//@[000:0016) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0016) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0015) | |   ├─FunctionArgumentSyntax
//@[009:0015) | |   | └─FunctionCallSyntax
//@[009:0012) | |   |   ├─IdentifierSyntax
//@[009:0012) | |   |   | └─Token(Identifier) |any|
//@[012:0013) | |   |   ├─Token(LeftParen) |(|
//@[013:0014) | |   |   ├─FunctionArgumentSyntax
//@[013:0014) | |   |   | └─IntegerLiteralSyntax
//@[013:0014) | |   |   |   └─Token(Integer) |2|
//@[014:0015) | |   |   └─Token(RightParen) |)|
//@[015:0016) | |   └─Token(RightParen) |)|
//@[016:0017) | ├─Token(NewLine) |\n|
param fatalErrorInIssue1713
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0027) | ├─IdentifierSyntax
//@[006:0027) | | └─Token(Identifier) |fatalErrorInIssue1713|
//@[027:0027) | └─SkippedTriviaSyntax
//@[027:0029) ├─Token(NewLine) |\n\n|

// wrong metadata schema
//@[024:0025) ├─Token(NewLine) |\n|
@metadata({
//@[000:0067) ├─ParameterDeclarationSyntax
//@[000:0034) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0034) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0033) | |   ├─FunctionArgumentSyntax
//@[010:0033) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  description: true
//@[002:0019) | |   |   ├─ObjectPropertySyntax
//@[002:0013) | |   |   | ├─IdentifierSyntax
//@[002:0013) | |   |   | | └─Token(Identifier) |description|
//@[013:0014) | |   |   | ├─Token(Colon) |:|
//@[015:0019) | |   |   | └─BooleanLiteralSyntax
//@[015:0019) | |   |   |   └─Token(TrueKeyword) |true|
//@[019:0020) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param wrongMetadataSchema string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0025) | ├─IdentifierSyntax
//@[006:0025) | | └─Token(Identifier) |wrongMetadataSchema|
//@[026:0032) | └─SimpleTypeSyntax
//@[026:0032) |   └─Token(Identifier) |string|
//@[032:0034) ├─Token(NewLine) |\n\n|

// expression in modifier
//@[025:0026) ├─Token(NewLine) |\n|
@maxLength(a + 2)
//@[000:0095) ├─ParameterDeclarationSyntax
//@[000:0017) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0017) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0016) | |   ├─FunctionArgumentSyntax
//@[011:0016) | |   | └─BinaryOperationSyntax
//@[011:0012) | |   |   ├─VariableAccessSyntax
//@[011:0012) | |   |   | └─IdentifierSyntax
//@[011:0012) | |   |   |   └─Token(Identifier) |a|
//@[013:0014) | |   |   ├─Token(Plus) |+|
//@[015:0016) | |   |   └─IntegerLiteralSyntax
//@[015:0016) | |   |     └─Token(Integer) |2|
//@[016:0017) | |   └─Token(RightParen) |)|
//@[017:0018) | ├─Token(NewLine) |\n|
@minLength(foo())
//@[000:0017) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0017) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0016) | |   ├─FunctionArgumentSyntax
//@[011:0016) | |   | └─FunctionCallSyntax
//@[011:0014) | |   |   ├─IdentifierSyntax
//@[011:0014) | |   |   | └─Token(Identifier) |foo|
//@[014:0015) | |   |   ├─Token(LeftParen) |(|
//@[015:0016) | |   |   └─Token(RightParen) |)|
//@[016:0017) | |   └─Token(RightParen) |)|
//@[017:0018) | ├─Token(NewLine) |\n|
@allowed([
//@[000:0017) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0017) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0016) | |   ├─FunctionArgumentSyntax
//@[009:0016) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  i
//@[002:0003) | |   |   ├─ArrayItemSyntax
//@[002:0003) | |   |   | └─VariableAccessSyntax
//@[002:0003) | |   |   |   └─IdentifierSyntax
//@[002:0003) | |   |   |     └─Token(Identifier) |i|
//@[003:0004) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param expressionInModifier string = 2 + 3
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |expressionInModifier|
//@[027:0033) | ├─SimpleTypeSyntax
//@[027:0033) | | └─Token(Identifier) |string|
//@[034:0041) | └─ParameterDefaultValueSyntax
//@[034:0035) |   ├─Token(Assignment) |=|
//@[036:0041) |   └─BinaryOperationSyntax
//@[036:0037) |     ├─IntegerLiteralSyntax
//@[036:0037) |     | └─Token(Integer) |2|
//@[038:0039) |     ├─Token(Plus) |+|
//@[040:0041) |     └─IntegerLiteralSyntax
//@[040:0041) |       └─Token(Integer) |3|
//@[041:0043) ├─Token(NewLine) |\n\n|

@maxLength(2 + 3)
//@[000:0111) ├─ParameterDeclarationSyntax
//@[000:0017) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0017) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0016) | |   ├─FunctionArgumentSyntax
//@[011:0016) | |   | └─BinaryOperationSyntax
//@[011:0012) | |   |   ├─IntegerLiteralSyntax
//@[011:0012) | |   |   | └─Token(Integer) |2|
//@[013:0014) | |   |   ├─Token(Plus) |+|
//@[015:0016) | |   |   └─IntegerLiteralSyntax
//@[015:0016) | |   |     └─Token(Integer) |3|
//@[016:0017) | |   └─Token(RightParen) |)|
//@[017:0018) | ├─Token(NewLine) |\n|
@minLength(length([]))
//@[000:0022) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0022) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0021) | |   ├─FunctionArgumentSyntax
//@[011:0021) | |   | └─FunctionCallSyntax
//@[011:0017) | |   |   ├─IdentifierSyntax
//@[011:0017) | |   |   | └─Token(Identifier) |length|
//@[017:0018) | |   |   ├─Token(LeftParen) |(|
//@[018:0020) | |   |   ├─FunctionArgumentSyntax
//@[018:0020) | |   |   | └─ArraySyntax
//@[018:0019) | |   |   |   ├─Token(LeftSquare) |[|
//@[019:0020) | |   |   |   └─Token(RightSquare) |]|
//@[020:0021) | |   |   └─Token(RightParen) |)|
//@[021:0022) | |   └─Token(RightParen) |)|
//@[022:0023) | ├─Token(NewLine) |\n|
@allowed([
//@[000:0034) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0034) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0033) | |   ├─FunctionArgumentSyntax
//@[009:0033) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  resourceGroup().id
//@[002:0020) | |   |   ├─ArrayItemSyntax
//@[002:0020) | |   |   | └─PropertyAccessSyntax
//@[002:0017) | |   |   |   ├─FunctionCallSyntax
//@[002:0015) | |   |   |   | ├─IdentifierSyntax
//@[002:0015) | |   |   |   | | └─Token(Identifier) |resourceGroup|
//@[015:0016) | |   |   |   | ├─Token(LeftParen) |(|
//@[016:0017) | |   |   |   | └─Token(RightParen) |)|
//@[017:0018) | |   |   |   ├─Token(Dot) |.|
//@[018:0020) | |   |   |   └─IdentifierSyntax
//@[018:0020) | |   |   |     └─Token(Identifier) |id|
//@[020:0021) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param nonCompileTimeConstant string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0028) | ├─IdentifierSyntax
//@[006:0028) | | └─Token(Identifier) |nonCompileTimeConstant|
//@[029:0035) | └─SimpleTypeSyntax
//@[029:0035) |   └─Token(Identifier) |string|
//@[035:0038) ├─Token(NewLine) |\n\n\n|


@allowed([])
//@[000:0044) ├─ParameterDeclarationSyntax
//@[000:0012) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0012) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0011) | |   ├─FunctionArgumentSyntax
//@[009:0011) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   └─Token(RightSquare) |]|
//@[011:0012) | |   └─Token(RightParen) |)|
//@[012:0013) | ├─Token(NewLine) |\n|
param emptyAllowedString string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0024) | ├─IdentifierSyntax
//@[006:0024) | | └─Token(Identifier) |emptyAllowedString|
//@[025:0031) | └─SimpleTypeSyntax
//@[025:0031) |   └─Token(Identifier) |string|
//@[031:0033) ├─Token(NewLine) |\n\n|

@allowed([])
//@[000:0038) ├─ParameterDeclarationSyntax
//@[000:0012) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0012) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0011) | |   ├─FunctionArgumentSyntax
//@[009:0011) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   └─Token(RightSquare) |]|
//@[011:0012) | |   └─Token(RightParen) |)|
//@[012:0013) | ├─Token(NewLine) |\n|
param emptyAllowedInt int
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |emptyAllowedInt|
//@[022:0025) | └─SimpleTypeSyntax
//@[022:0025) |   └─Token(Identifier) |int|
//@[025:0027) ├─Token(NewLine) |\n\n|

// 1-cycle in params
//@[020:0021) ├─Token(NewLine) |\n|
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[000:0056) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |paramDefaultOneCycle|
//@[027:0033) | ├─SimpleTypeSyntax
//@[027:0033) | | └─Token(Identifier) |string|
//@[034:0056) | └─ParameterDefaultValueSyntax
//@[034:0035) |   ├─Token(Assignment) |=|
//@[036:0056) |   └─VariableAccessSyntax
//@[036:0056) |     └─IdentifierSyntax
//@[036:0056) |       └─Token(Identifier) |paramDefaultOneCycle|
//@[056:0058) ├─Token(NewLine) |\n\n|

// 2-cycle in params
//@[020:0021) ├─Token(NewLine) |\n|
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[000:0058) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0027) | ├─IdentifierSyntax
//@[006:0027) | | └─Token(Identifier) |paramDefaultTwoCycle1|
//@[028:0034) | ├─SimpleTypeSyntax
//@[028:0034) | | └─Token(Identifier) |string|
//@[035:0058) | └─ParameterDefaultValueSyntax
//@[035:0036) |   ├─Token(Assignment) |=|
//@[037:0058) |   └─VariableAccessSyntax
//@[037:0058) |     └─IdentifierSyntax
//@[037:0058) |       └─Token(Identifier) |paramDefaultTwoCycle2|
//@[058:0059) ├─Token(NewLine) |\n|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[000:0058) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0027) | ├─IdentifierSyntax
//@[006:0027) | | └─Token(Identifier) |paramDefaultTwoCycle2|
//@[028:0034) | ├─SimpleTypeSyntax
//@[028:0034) | | └─Token(Identifier) |string|
//@[035:0058) | └─ParameterDefaultValueSyntax
//@[035:0036) |   ├─Token(Assignment) |=|
//@[037:0058) |   └─VariableAccessSyntax
//@[037:0058) |     └─IdentifierSyntax
//@[037:0058) |       └─Token(Identifier) |paramDefaultTwoCycle1|
//@[058:0060) ├─Token(NewLine) |\n\n|

@allowed([
//@[000:0074) ├─ParameterDeclarationSyntax
//@[000:0038) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0038) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0037) | |   ├─FunctionArgumentSyntax
//@[009:0037) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  paramModifierSelfCycle
//@[002:0024) | |   |   ├─ArrayItemSyntax
//@[002:0024) | |   |   | └─VariableAccessSyntax
//@[002:0024) | |   |   |   └─IdentifierSyntax
//@[002:0024) | |   |   |     └─Token(Identifier) |paramModifierSelfCycle|
//@[024:0025) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param paramModifierSelfCycle string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0028) | ├─IdentifierSyntax
//@[006:0028) | | └─Token(Identifier) |paramModifierSelfCycle|
//@[029:0035) | └─SimpleTypeSyntax
//@[029:0035) |   └─Token(Identifier) |string|
//@[035:0037) ├─Token(NewLine) |\n\n|

// wrong types of "variable"/identifier access
//@[046:0047) ├─Token(NewLine) |\n|
var sampleVar = 'sample'
//@[000:0024) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |sampleVar|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0024) | └─StringSyntax
//@[016:0024) |   └─Token(StringComplete) |'sample'|
//@[024:0025) ├─Token(NewLine) |\n|
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[000:0075) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0023) | ├─IdentifierSyntax
//@[009:0023) | | └─Token(Identifier) |sampleResource|
//@[024:0055) | ├─StringSyntax
//@[024:0055) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02'|
//@[056:0057) | ├─Token(Assignment) |=|
//@[058:0075) | └─ObjectSyntax
//@[058:0059) |   ├─Token(LeftBrace) |{|
//@[059:0060) |   ├─Token(NewLine) |\n|
  name: 'foo'
//@[002:0013) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0013) |   | └─StringSyntax
//@[008:0013) |   |   └─Token(StringComplete) |'foo'|
//@[013:0014) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
output sampleOutput string = 'hello'
//@[000:0036) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0019) | ├─IdentifierSyntax
//@[007:0019) | | └─Token(Identifier) |sampleOutput|
//@[020:0026) | ├─SimpleTypeSyntax
//@[020:0026) | | └─Token(Identifier) |string|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0036) | └─StringSyntax
//@[029:0036) |   └─Token(StringComplete) |'hello'|
//@[036:0038) ├─Token(NewLine) |\n\n|

param paramAccessingVar string = concat(sampleVar, 's')
//@[000:0055) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |paramAccessingVar|
//@[024:0030) | ├─SimpleTypeSyntax
//@[024:0030) | | └─Token(Identifier) |string|
//@[031:0055) | └─ParameterDefaultValueSyntax
//@[031:0032) |   ├─Token(Assignment) |=|
//@[033:0055) |   └─FunctionCallSyntax
//@[033:0039) |     ├─IdentifierSyntax
//@[033:0039) |     | └─Token(Identifier) |concat|
//@[039:0040) |     ├─Token(LeftParen) |(|
//@[040:0049) |     ├─FunctionArgumentSyntax
//@[040:0049) |     | └─VariableAccessSyntax
//@[040:0049) |     |   └─IdentifierSyntax
//@[040:0049) |     |     └─Token(Identifier) |sampleVar|
//@[049:0050) |     ├─Token(Comma) |,|
//@[051:0054) |     ├─FunctionArgumentSyntax
//@[051:0054) |     | └─StringSyntax
//@[051:0054) |     |   └─Token(StringComplete) |'s'|
//@[054:0055) |     └─Token(RightParen) |)|
//@[055:0057) ├─Token(NewLine) |\n\n|

param paramAccessingResource string = sampleResource
//@[000:0052) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0028) | ├─IdentifierSyntax
//@[006:0028) | | └─Token(Identifier) |paramAccessingResource|
//@[029:0035) | ├─SimpleTypeSyntax
//@[029:0035) | | └─Token(Identifier) |string|
//@[036:0052) | └─ParameterDefaultValueSyntax
//@[036:0037) |   ├─Token(Assignment) |=|
//@[038:0052) |   └─VariableAccessSyntax
//@[038:0052) |     └─IdentifierSyntax
//@[038:0052) |       └─Token(Identifier) |sampleResource|
//@[052:0054) ├─Token(NewLine) |\n\n|

param paramAccessingOutput string = sampleOutput
//@[000:0048) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |paramAccessingOutput|
//@[027:0033) | ├─SimpleTypeSyntax
//@[027:0033) | | └─Token(Identifier) |string|
//@[034:0048) | └─ParameterDefaultValueSyntax
//@[034:0035) |   ├─Token(Assignment) |=|
//@[036:0048) |   └─VariableAccessSyntax
//@[036:0048) |     └─IdentifierSyntax
//@[036:0048) |       └─Token(Identifier) |sampleOutput|
//@[048:0050) ├─Token(NewLine) |\n\n|

// #completionTest(6) -> empty
//@[030:0031) ├─Token(NewLine) |\n|
param 
//@[000:0006) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0006) | ├─IdentifierSyntax
//@[006:0006) | | └─SkippedTriviaSyntax
//@[006:0006) | └─SkippedTriviaSyntax
//@[006:0008) ├─Token(NewLine) |\n\n|

// #completionTest(46,47) -> justSymbols
//@[040:0041) ├─Token(NewLine) |\n|
param defaultValueOneLinerCompletions string = 
//@[000:0047) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0037) | ├─IdentifierSyntax
//@[006:0037) | | └─Token(Identifier) |defaultValueOneLinerCompletions|
//@[038:0044) | ├─SimpleTypeSyntax
//@[038:0044) | | └─Token(Identifier) |string|
//@[045:0047) | └─ParameterDefaultValueSyntax
//@[045:0046) |   ├─Token(Assignment) |=|
//@[047:0047) |   └─SkippedTriviaSyntax
//@[047:0049) ├─Token(NewLine) |\n\n|

// invalid comma separator (array)
//@[034:0035) ├─Token(NewLine) |\n|
@metadata({
//@[000:0108) ├─ParameterDeclarationSyntax
//@[000:0055) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0055) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0054) | |   ├─FunctionArgumentSyntax
//@[010:0054) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  description: 'Name of Virtual Machine'
//@[002:0040) | |   |   ├─ObjectPropertySyntax
//@[002:0013) | |   |   | ├─IdentifierSyntax
//@[002:0013) | |   |   | | └─Token(Identifier) |description|
//@[013:0014) | |   |   | ├─Token(Colon) |:|
//@[015:0040) | |   |   | └─StringSyntax
//@[015:0040) | |   |   |   └─Token(StringComplete) |'Name of Virtual Machine'|
//@[040:0041) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
@allowed([
//@[000:0030) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0030) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0029) | |   ├─FunctionArgumentSyntax
//@[009:0029) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  'abc',
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'abc'|
//@[007:0008) | |   |   ├─Token(Comma) |,|
//@[008:0008) | |   |   ├─SkippedTriviaSyntax
//@[008:0009) | |   |   ├─Token(NewLine) |\n|
  'def'
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'def'|
//@[007:0008) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param commaOne string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0014) | ├─IdentifierSyntax
//@[006:0014) | | └─Token(Identifier) |commaOne|
//@[015:0021) | └─SimpleTypeSyntax
//@[015:0021) |   └─Token(Identifier) |string|
//@[021:0023) ├─Token(NewLine) |\n\n|

@secure
//@[000:0075) ├─ParameterDeclarationSyntax
//@[000:0007) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0007) | | └─VariableAccessSyntax
//@[001:0007) | |   └─IdentifierSyntax
//@[001:0007) | |     └─Token(Identifier) |secure|
//@[007:0008) | ├─Token(NewLine) |\n|
@
//@[000:0001) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0001) | | └─SkippedTriviaSyntax
//@[001:0002) | ├─Token(NewLine) |\n|
@&& xxx
//@[000:0007) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0007) | | └─SkippedTriviaSyntax
//@[001:0003) | |   ├─Token(LogicalAnd) |&&|
//@[004:0007) | |   └─Token(Identifier) |xxx|
//@[007:0008) | ├─Token(NewLine) |\n|
@sys
//@[000:0004) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0004) | | └─VariableAccessSyntax
//@[001:0004) | |   └─IdentifierSyntax
//@[001:0004) | |     └─Token(Identifier) |sys|
//@[004:0005) | ├─Token(NewLine) |\n|
@paramAccessingVar
//@[000:0018) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0018) | | └─VariableAccessSyntax
//@[001:0018) | |   └─IdentifierSyntax
//@[001:0018) | |     └─Token(Identifier) |paramAccessingVar|
//@[018:0019) | ├─Token(NewLine) |\n|
param incompleteDecorators string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |incompleteDecorators|
//@[027:0033) | └─SimpleTypeSyntax
//@[027:0033) |   └─Token(Identifier) |string|
//@[033:0035) ├─Token(NewLine) |\n\n|

@concat(1, 2)
//@[000:0104) ├─ParameterDeclarationSyntax
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |concat|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   ├─FunctionArgumentSyntax
//@[008:0009) | |   | └─IntegerLiteralSyntax
//@[008:0009) | |   |   └─Token(Integer) |1|
//@[009:0010) | |   ├─Token(Comma) |,|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |2|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
@sys.concat('a', 'b')
//@[000:0021) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0021) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0011) | |   ├─IdentifierSyntax
//@[005:0011) | |   | └─Token(Identifier) |concat|
//@[011:0012) | |   ├─Token(LeftParen) |(|
//@[012:0015) | |   ├─FunctionArgumentSyntax
//@[012:0015) | |   | └─StringSyntax
//@[012:0015) | |   |   └─Token(StringComplete) |'a'|
//@[015:0016) | |   ├─Token(Comma) |,|
//@[017:0020) | |   ├─FunctionArgumentSyntax
//@[017:0020) | |   | └─StringSyntax
//@[017:0020) | |   |   └─Token(StringComplete) |'b'|
//@[020:0021) | |   └─Token(RightParen) |)|
//@[021:0022) | ├─Token(NewLine) |\n|
@secure()
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
// wrong target type
//@[020:0021) | ├─Token(NewLine) |\n|
@minValue(20)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0012) | |   ├─FunctionArgumentSyntax
//@[010:0012) | |   | └─IntegerLiteralSyntax
//@[010:0012) | |   |   └─Token(Integer) |20|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
param someString string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0016) | ├─IdentifierSyntax
//@[006:0016) | | └─Token(Identifier) |someString|
//@[017:0023) | └─SimpleTypeSyntax
//@[017:0023) |   └─Token(Identifier) |string|
//@[023:0025) ├─Token(NewLine) |\n\n|

@allowed([
//@[000:0207) ├─ParameterDeclarationSyntax
//@[000:0039) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0039) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0038) | |   ├─FunctionArgumentSyntax
//@[009:0038) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
    true
//@[004:0008) | |   |   ├─ArrayItemSyntax
//@[004:0008) | |   |   | └─BooleanLiteralSyntax
//@[004:0008) | |   |   |   └─Token(TrueKeyword) |true|
//@[008:0009) | |   |   ├─Token(NewLine) |\n|
    10
//@[004:0006) | |   |   ├─ArrayItemSyntax
//@[004:0006) | |   |   | └─IntegerLiteralSyntax
//@[004:0006) | |   |   |   └─Token(Integer) |10|
//@[006:0007) | |   |   ├─Token(NewLine) |\n|
    'foo'
//@[004:0009) | |   |   ├─ArrayItemSyntax
//@[004:0009) | |   |   | └─StringSyntax
//@[004:0009) | |   |   |   └─Token(StringComplete) |'foo'|
//@[009:0010) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
@secure()
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
// #completionTest(1, 2, 3) -> intParameterDecoratorsPlusNamespace
//@[066:0067) | ├─Token(NewLine) |\n|
@  
//@[000:0003) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[003:0003) | | └─SkippedTriviaSyntax
//@[003:0004) | ├─Token(NewLine) |\n|
// #completionTest(5, 6) -> intParameterDecorators
//@[050:0051) | ├─Token(NewLine) |\n|
@sys.   
//@[000:0008) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0008) | | └─PropertyAccessSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[008:0008) | |   └─IdentifierSyntax
//@[008:0008) | |     └─SkippedTriviaSyntax
//@[008:0009) | ├─Token(NewLine) |\n|
param someInteger int = 20
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |someInteger|
//@[018:0021) | ├─SimpleTypeSyntax
//@[018:0021) | | └─Token(Identifier) |int|
//@[022:0026) | └─ParameterDefaultValueSyntax
//@[022:0023) |   ├─Token(Assignment) |=|
//@[024:0026) |   └─IntegerLiteralSyntax
//@[024:0026) |     └─Token(Integer) |20|
//@[026:0028) ├─Token(NewLine) |\n\n|

@allowed([], [], 2)
//@[000:0088) ├─ParameterDeclarationSyntax
//@[000:0019) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0019) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0011) | |   ├─FunctionArgumentSyntax
//@[009:0011) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   └─Token(RightSquare) |]|
//@[011:0012) | |   ├─Token(Comma) |,|
//@[013:0015) | |   ├─FunctionArgumentSyntax
//@[013:0015) | |   | └─ArraySyntax
//@[013:0014) | |   |   ├─Token(LeftSquare) |[|
//@[014:0015) | |   |   └─Token(RightSquare) |]|
//@[015:0016) | |   ├─Token(Comma) |,|
//@[017:0018) | |   ├─FunctionArgumentSyntax
//@[017:0018) | |   | └─IntegerLiteralSyntax
//@[017:0018) | |   |   └─Token(Integer) |2|
//@[018:0019) | |   └─Token(RightParen) |)|
//@[019:0020) | ├─Token(NewLine) |\n|
// #completionTest(4) -> empty
//@[030:0031) | ├─Token(NewLine) |\n|
@az.
//@[000:0004) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0004) | | └─PropertyAccessSyntax
//@[001:0003) | |   ├─VariableAccessSyntax
//@[001:0003) | |   | └─IdentifierSyntax
//@[001:0003) | |   |   └─Token(Identifier) |az|
//@[003:0004) | |   ├─Token(Dot) |.|
//@[004:0004) | |   └─IdentifierSyntax
//@[004:0004) | |     └─SkippedTriviaSyntax
//@[004:0005) | ├─Token(NewLine) |\n|
param tooManyArguments1 int = 20
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |tooManyArguments1|
//@[024:0027) | ├─SimpleTypeSyntax
//@[024:0027) | | └─Token(Identifier) |int|
//@[028:0032) | └─ParameterDefaultValueSyntax
//@[028:0029) |   ├─Token(Assignment) |=|
//@[030:0032) |   └─IntegerLiteralSyntax
//@[030:0032) |     └─Token(Integer) |20|
//@[032:0034) ├─Token(NewLine) |\n\n|

@metadata({}, {}, true)
//@[000:0253) ├─ParameterDeclarationSyntax
//@[000:0023) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0023) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0012) | |   ├─FunctionArgumentSyntax
//@[010:0012) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   └─Token(RightBrace) |}|
//@[012:0013) | |   ├─Token(Comma) |,|
//@[014:0016) | |   ├─FunctionArgumentSyntax
//@[014:0016) | |   | └─ObjectSyntax
//@[014:0015) | |   |   ├─Token(LeftBrace) |{|
//@[015:0016) | |   |   └─Token(RightBrace) |}|
//@[016:0017) | |   ├─Token(Comma) |,|
//@[018:0022) | |   ├─FunctionArgumentSyntax
//@[018:0022) | |   | └─BooleanLiteralSyntax
//@[018:0022) | |   |   └─Token(TrueKeyword) |true|
//@[022:0023) | |   └─Token(RightParen) |)|
//@[023:0024) | ├─Token(NewLine) |\n|
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
//@[063:0064) | ├─Token(NewLine) |\n|
@m
//@[000:0002) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0002) | | └─VariableAccessSyntax
//@[001:0002) | |   └─IdentifierSyntax
//@[001:0002) | |     └─Token(Identifier) |m|
//@[002:0003) | ├─Token(NewLine) |\n|
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
//@[069:0070) | ├─Token(NewLine) |\n|
@   
//@[000:0004) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[004:0004) | | └─SkippedTriviaSyntax
//@[004:0005) | ├─Token(NewLine) |\n|
// #completionTest(5) -> stringParameterDecorators
//@[050:0051) | ├─Token(NewLine) |\n|
@sys.
//@[000:0005) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0005) | | └─PropertyAccessSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0005) | |   └─IdentifierSyntax
//@[005:0005) | |     └─SkippedTriviaSyntax
//@[005:0006) | ├─Token(NewLine) |\n|
param tooManyArguments2 string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |tooManyArguments2|
//@[024:0030) | └─SimpleTypeSyntax
//@[024:0030) |   └─Token(Identifier) |string|
//@[030:0032) ├─Token(NewLine) |\n\n|

@description(sys.concat(2))
//@[000:0096) ├─ParameterDeclarationSyntax
//@[000:0027) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0027) | | └─FunctionCallSyntax
//@[001:0012) | |   ├─IdentifierSyntax
//@[001:0012) | |   | └─Token(Identifier) |description|
//@[012:0013) | |   ├─Token(LeftParen) |(|
//@[013:0026) | |   ├─FunctionArgumentSyntax
//@[013:0026) | |   | └─InstanceFunctionCallSyntax
//@[013:0016) | |   |   ├─VariableAccessSyntax
//@[013:0016) | |   |   | └─IdentifierSyntax
//@[013:0016) | |   |   |   └─Token(Identifier) |sys|
//@[016:0017) | |   |   ├─Token(Dot) |.|
//@[017:0023) | |   |   ├─IdentifierSyntax
//@[017:0023) | |   |   | └─Token(Identifier) |concat|
//@[023:0024) | |   |   ├─Token(LeftParen) |(|
//@[024:0025) | |   |   ├─FunctionArgumentSyntax
//@[024:0025) | |   |   | └─IntegerLiteralSyntax
//@[024:0025) | |   |   |   └─Token(Integer) |2|
//@[025:0026) | |   |   └─Token(RightParen) |)|
//@[026:0027) | |   └─Token(RightParen) |)|
//@[027:0028) | ├─Token(NewLine) |\n|
@allowed([for thing in []: 's'])
//@[000:0032) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0032) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0031) | |   ├─FunctionArgumentSyntax
//@[009:0031) | |   | └─ForSyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0013) | |   |   ├─Token(Identifier) |for|
//@[014:0019) | |   |   ├─LocalVariableSyntax
//@[014:0019) | |   |   | └─IdentifierSyntax
//@[014:0019) | |   |   |   └─Token(Identifier) |thing|
//@[020:0022) | |   |   ├─Token(Identifier) |in|
//@[023:0025) | |   |   ├─ArraySyntax
//@[023:0024) | |   |   | ├─Token(LeftSquare) |[|
//@[024:0025) | |   |   | └─Token(RightSquare) |]|
//@[025:0026) | |   |   ├─Token(Colon) |:|
//@[027:0030) | |   |   ├─StringSyntax
//@[027:0030) | |   |   | └─Token(StringComplete) |'s'|
//@[030:0031) | |   |   └─Token(RightSquare) |]|
//@[031:0032) | |   └─Token(RightParen) |)|
//@[032:0033) | ├─Token(NewLine) |\n|
param nonConstantInDecorator string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0028) | ├─IdentifierSyntax
//@[006:0028) | | └─Token(Identifier) |nonConstantInDecorator|
//@[029:0035) | └─SimpleTypeSyntax
//@[029:0035) |   └─Token(Identifier) |string|
//@[035:0037) ├─Token(NewLine) |\n\n|

@minValue(-length('s'))
//@[000:0083) ├─ParameterDeclarationSyntax
//@[000:0023) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0023) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0022) | |   ├─FunctionArgumentSyntax
//@[010:0022) | |   | └─UnaryOperationSyntax
//@[010:0011) | |   |   ├─Token(Minus) |-|
//@[011:0022) | |   |   └─FunctionCallSyntax
//@[011:0017) | |   |     ├─IdentifierSyntax
//@[011:0017) | |   |     | └─Token(Identifier) |length|
//@[017:0018) | |   |     ├─Token(LeftParen) |(|
//@[018:0021) | |   |     ├─FunctionArgumentSyntax
//@[018:0021) | |   |     | └─StringSyntax
//@[018:0021) | |   |     |   └─Token(StringComplete) |'s'|
//@[021:0022) | |   |     └─Token(RightParen) |)|
//@[022:0023) | |   └─Token(RightParen) |)|
//@[023:0024) | ├─Token(NewLine) |\n|
@metadata({
//@[000:0028) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0028) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0027) | |   ├─FunctionArgumentSyntax
//@[010:0027) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  bool: !true
//@[002:0013) | |   |   ├─ObjectPropertySyntax
//@[002:0006) | |   |   | ├─IdentifierSyntax
//@[002:0006) | |   |   | | └─Token(Identifier) |bool|
//@[006:0007) | |   |   | ├─Token(Colon) |:|
//@[008:0013) | |   |   | └─UnaryOperationSyntax
//@[008:0009) | |   |   |   ├─Token(Exclamation) |!|
//@[009:0013) | |   |   |   └─BooleanLiteralSyntax
//@[009:0013) | |   |   |     └─Token(TrueKeyword) |true|
//@[013:0014) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param unaryMinusOnFunction int
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |unaryMinusOnFunction|
//@[027:0030) | └─SimpleTypeSyntax
//@[027:0030) |   └─Token(Identifier) |int|
//@[030:0032) ├─Token(NewLine) |\n\n|

@minLength(1)
//@[000:0098) ├─ParameterDeclarationSyntax
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |1|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
@minLength(2)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |2|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
@secure()
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
@maxLength(3)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |3|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
@maxLength(4)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |4|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
param duplicateDecorators string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0025) | ├─IdentifierSyntax
//@[006:0025) | | └─Token(Identifier) |duplicateDecorators|
//@[026:0032) | └─SimpleTypeSyntax
//@[026:0032) |   └─Token(Identifier) |string|
//@[032:0034) ├─Token(NewLine) |\n\n|

@minLength(-1)
//@[000:0058) ├─ParameterDeclarationSyntax
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0013) | |   ├─FunctionArgumentSyntax
//@[011:0013) | |   | └─UnaryOperationSyntax
//@[011:0012) | |   |   ├─Token(Minus) |-|
//@[012:0013) | |   |   └─IntegerLiteralSyntax
//@[012:0013) | |   |     └─Token(Integer) |1|
//@[013:0014) | |   └─Token(RightParen) |)|
//@[014:0015) | ├─Token(NewLine) |\n|
@maxLength(-100)
//@[000:0016) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0016) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0015) | |   ├─FunctionArgumentSyntax
//@[011:0015) | |   | └─UnaryOperationSyntax
//@[011:0012) | |   |   ├─Token(Minus) |-|
//@[012:0015) | |   |   └─IntegerLiteralSyntax
//@[012:0015) | |   |     └─Token(Integer) |100|
//@[015:0016) | |   └─Token(RightParen) |)|
//@[016:0017) | ├─Token(NewLine) |\n|
param invalidLength string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |invalidLength|
//@[020:0026) | └─SimpleTypeSyntax
//@[020:0026) |   └─Token(Identifier) |string|
//@[026:0028) ├─Token(NewLine) |\n\n|

@allowed([
//@[000:0366) ├─ParameterDeclarationSyntax
//@[000:0305) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0305) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0304) | |   ├─FunctionArgumentSyntax
//@[009:0304) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
	'Microsoft.AnalysisServices/servers'
//@[001:0037) | |   |   ├─ArrayItemSyntax
//@[001:0037) | |   |   | └─StringSyntax
//@[001:0037) | |   |   |   └─Token(StringComplete) |'Microsoft.AnalysisServices/servers'|
//@[037:0038) | |   |   ├─Token(NewLine) |\n|
	'Microsoft.ApiManagement/service'
//@[001:0034) | |   |   ├─ArrayItemSyntax
//@[001:0034) | |   |   | └─StringSyntax
//@[001:0034) | |   |   |   └─Token(StringComplete) |'Microsoft.ApiManagement/service'|
//@[034:0035) | |   |   ├─Token(NewLine) |\n|
	'Microsoft.Network/applicationGateways'
//@[001:0040) | |   |   ├─ArrayItemSyntax
//@[001:0040) | |   |   | └─StringSyntax
//@[001:0040) | |   |   |   └─Token(StringComplete) |'Microsoft.Network/applicationGateways'|
//@[040:0041) | |   |   ├─Token(NewLine) |\n|
	'Microsoft.Automation/automationAccounts'
//@[001:0042) | |   |   ├─ArrayItemSyntax
//@[001:0042) | |   |   | └─StringSyntax
//@[001:0042) | |   |   |   └─Token(StringComplete) |'Microsoft.Automation/automationAccounts'|
//@[042:0043) | |   |   ├─Token(NewLine) |\n|
	'Microsoft.ContainerInstance/containerGroups'
//@[001:0046) | |   |   ├─ArrayItemSyntax
//@[001:0046) | |   |   | └─StringSyntax
//@[001:0046) | |   |   |   └─Token(StringComplete) |'Microsoft.ContainerInstance/containerGroups'|
//@[046:0047) | |   |   ├─Token(NewLine) |\n|
	'Microsoft.ContainerRegistry/registries'
//@[001:0041) | |   |   ├─ArrayItemSyntax
//@[001:0041) | |   |   | └─StringSyntax
//@[001:0041) | |   |   |   └─Token(StringComplete) |'Microsoft.ContainerRegistry/registries'|
//@[041:0042) | |   |   ├─Token(NewLine) |\n|
	'Microsoft.ContainerService/managedClusters'
//@[001:0045) | |   |   ├─ArrayItemSyntax
//@[001:0045) | |   |   | └─StringSyntax
//@[001:0045) | |   |   |   └─Token(StringComplete) |'Microsoft.ContainerService/managedClusters'|
//@[045:0046) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param invalidPermutation array = [
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0024) | ├─IdentifierSyntax
//@[006:0024) | | └─Token(Identifier) |invalidPermutation|
//@[025:0030) | ├─SimpleTypeSyntax
//@[025:0030) | | └─Token(Identifier) |array|
//@[031:0060) | └─ParameterDefaultValueSyntax
//@[031:0032) |   ├─Token(Assignment) |=|
//@[033:0060) |   └─ArraySyntax
//@[033:0034) |     ├─Token(LeftSquare) |[|
//@[034:0035) |     ├─Token(NewLine) |\n|
	'foobar'
//@[001:0009) |     ├─ArrayItemSyntax
//@[001:0009) |     | └─StringSyntax
//@[001:0009) |     |   └─Token(StringComplete) |'foobar'|
//@[009:0010) |     ├─Token(NewLine) |\n|
	true
//@[001:0005) |     ├─ArrayItemSyntax
//@[001:0005) |     | └─BooleanLiteralSyntax
//@[001:0005) |     |   └─Token(TrueKeyword) |true|
//@[005:0006) |     ├─Token(NewLine) |\n|
    100
//@[004:0007) |     ├─ArrayItemSyntax
//@[004:0007) |     | └─IntegerLiteralSyntax
//@[004:0007) |     |   └─Token(Integer) |100|
//@[007:0008) |     ├─Token(NewLine) |\n|
]
//@[000:0001) |     └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

@allowed([
//@[000:0245) ├─ParameterDeclarationSyntax
//@[000:0186) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0186) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0185) | |   ├─FunctionArgumentSyntax
//@[009:0185) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
	[
//@[001:0080) | |   |   ├─ArrayItemSyntax
//@[001:0080) | |   |   | └─ArraySyntax
//@[001:0002) | |   |   |   ├─Token(LeftSquare) |[|
//@[002:0003) | |   |   |   ├─Token(NewLine) |\n|
		'Microsoft.AnalysisServices/servers'
//@[002:0038) | |   |   |   ├─ArrayItemSyntax
//@[002:0038) | |   |   |   | └─StringSyntax
//@[002:0038) | |   |   |   |   └─Token(StringComplete) |'Microsoft.AnalysisServices/servers'|
//@[038:0039) | |   |   |   ├─Token(NewLine) |\n|
		'Microsoft.ApiManagement/service'
//@[002:0035) | |   |   |   ├─ArrayItemSyntax
//@[002:0035) | |   |   |   | └─StringSyntax
//@[002:0035) | |   |   |   |   └─Token(StringComplete) |'Microsoft.ApiManagement/service'|
//@[035:0036) | |   |   |   ├─Token(NewLine) |\n|
	]
//@[001:0002) | |   |   |   └─Token(RightSquare) |]|
//@[002:0003) | |   |   ├─Token(NewLine) |\n|
	[
//@[001:0091) | |   |   ├─ArrayItemSyntax
//@[001:0091) | |   |   | └─ArraySyntax
//@[001:0002) | |   |   |   ├─Token(LeftSquare) |[|
//@[002:0003) | |   |   |   ├─Token(NewLine) |\n|
		'Microsoft.Network/applicationGateways'
//@[002:0041) | |   |   |   ├─ArrayItemSyntax
//@[002:0041) | |   |   |   | └─StringSyntax
//@[002:0041) | |   |   |   |   └─Token(StringComplete) |'Microsoft.Network/applicationGateways'|
//@[041:0042) | |   |   |   ├─Token(NewLine) |\n|
		'Microsoft.Automation/automationAccounts'
//@[002:0043) | |   |   |   ├─ArrayItemSyntax
//@[002:0043) | |   |   |   | └─StringSyntax
//@[002:0043) | |   |   |   |   └─Token(StringComplete) |'Microsoft.Automation/automationAccounts'|
//@[043:0044) | |   |   |   ├─Token(NewLine) |\n|
	]
//@[001:0002) | |   |   |   └─Token(RightSquare) |]|
//@[002:0003) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param invalidDefaultWithAllowedArrayDecorator array = true
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0045) | ├─IdentifierSyntax
//@[006:0045) | | └─Token(Identifier) |invalidDefaultWithAllowedArrayDecorator|
//@[046:0051) | ├─SimpleTypeSyntax
//@[046:0051) | | └─Token(Identifier) |array|
//@[052:0058) | └─ParameterDefaultValueSyntax
//@[052:0053) |   ├─Token(Assignment) |=|
//@[054:0058) |   └─BooleanLiteralSyntax
//@[054:0058) |     └─Token(TrueKeyword) |true|
//@[058:0060) ├─Token(NewLine) |\n\n|

// unterminated multi-line comment
//@[034:0035) ├─Token(NewLine) |\n|
/*    

//@[000:0000) └─Token(EndOfFile) ||
