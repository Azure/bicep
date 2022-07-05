
//@[000:3447) ProgramSyntax
//@[000:0002) ├─Token(NewLine) |\r\n|
// wrong declaration
//@[020:0022) ├─Token(NewLine) |\r\n|
bad
//@[000:0003) ├─SkippedTriviaSyntax
//@[000:0003) | └─Token(Identifier) |bad|
//@[003:0007) ├─Token(NewLine) |\r\n\r\n|

// incomplete #completionTest(7) -> empty
//@[041:0043) ├─Token(NewLine) |\r\n|
output 
//@[000:0007) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0007) | ├─IdentifierSyntax
//@[007:0007) | | └─SkippedTriviaSyntax
//@[007:0007) | ├─SkippedTriviaSyntax
//@[007:0007) | ├─SkippedTriviaSyntax
//@[007:0007) | └─SkippedTriviaSyntax
//@[007:0011) ├─Token(NewLine) |\r\n\r\n|

var testSymbol = 42
//@[000:0019) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0014) | ├─IdentifierSyntax
//@[004:0014) | | └─Token(Identifier) |testSymbol|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0019) | └─IntegerLiteralSyntax
//@[017:0019) |   └─Token(Integer) |42|
//@[019:0023) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(28,29) -> symbols
//@[036:0038) ├─Token(NewLine) |\r\n|
output missingValueAndType = 
//@[000:0029) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0026) | ├─IdentifierSyntax
//@[007:0026) | | └─Token(Identifier) |missingValueAndType|
//@[027:0027) | ├─SkippedTriviaSyntax
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0029) | └─SkippedTriviaSyntax
//@[029:0033) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(28,29) -> symbols
//@[036:0038) ├─Token(NewLine) |\r\n|
output missingValue string = 
//@[000:0029) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0019) | ├─IdentifierSyntax
//@[007:0019) | | └─Token(Identifier) |missingValue|
//@[020:0026) | ├─SimpleTypeSyntax
//@[020:0026) | | └─Token(Identifier) |string|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0029) | └─SkippedTriviaSyntax
//@[029:0033) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(31,32) -> arrayPlusSymbols
//@[045:0047) ├─Token(NewLine) |\r\n|
output arrayCompletions array = 
//@[000:0032) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0023) | ├─IdentifierSyntax
//@[007:0023) | | └─Token(Identifier) |arrayCompletions|
//@[024:0029) | ├─SimpleTypeSyntax
//@[024:0029) | | └─Token(Identifier) |array|
//@[030:0031) | ├─Token(Assignment) |=|
//@[032:0032) | └─SkippedTriviaSyntax
//@[032:0036) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(33,34) -> objectPlusSymbols
//@[046:0048) ├─Token(NewLine) |\r\n|
output objectCompletions object = 
//@[000:0034) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0024) | ├─IdentifierSyntax
//@[007:0024) | | └─Token(Identifier) |objectCompletions|
//@[025:0031) | ├─SimpleTypeSyntax
//@[025:0031) | | └─Token(Identifier) |object|
//@[032:0033) | ├─Token(Assignment) |=|
//@[034:0034) | └─SkippedTriviaSyntax
//@[034:0038) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(29,30) -> boolPlusSymbols
//@[044:0046) ├─Token(NewLine) |\r\n|
output boolCompletions bool = 
//@[000:0030) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0022) | ├─IdentifierSyntax
//@[007:0022) | | └─Token(Identifier) |boolCompletions|
//@[023:0027) | ├─SimpleTypeSyntax
//@[023:0027) | | └─Token(Identifier) |bool|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0030) | └─SkippedTriviaSyntax
//@[030:0034) ├─Token(NewLine) |\r\n\r\n|

output foo
//@[000:0010) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |foo|
//@[010:0010) | ├─SkippedTriviaSyntax
//@[010:0010) | ├─SkippedTriviaSyntax
//@[010:0010) | └─SkippedTriviaSyntax
//@[010:0014) ├─Token(NewLine) |\r\n\r\n|

// space after identifier #completionTest(20) -> outputTypes
//@[060:0062) ├─Token(NewLine) |\r\n|
output spaceAfterId 
//@[000:0020) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0019) | ├─IdentifierSyntax
//@[007:0019) | | └─Token(Identifier) |spaceAfterId|
//@[020:0020) | ├─SkippedTriviaSyntax
//@[020:0020) | ├─SkippedTriviaSyntax
//@[020:0020) | └─SkippedTriviaSyntax
//@[020:0024) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(25) -> outputTypes
//@[037:0039) ├─Token(NewLine) |\r\n|
output spacesAfterCursor  
//@[000:0026) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0024) | ├─IdentifierSyntax
//@[007:0024) | | └─Token(Identifier) |spacesAfterCursor|
//@[026:0026) | ├─SkippedTriviaSyntax
//@[026:0026) | ├─SkippedTriviaSyntax
//@[026:0026) | └─SkippedTriviaSyntax
//@[026:0030) ├─Token(NewLine) |\r\n\r\n|

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
//@[062:0064) ├─Token(NewLine) |\r\n|
output partialType obj
//@[000:0022) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0018) | ├─IdentifierSyntax
//@[007:0018) | | └─Token(Identifier) |partialType|
//@[019:0022) | ├─SimpleTypeSyntax
//@[019:0022) | | └─Token(Identifier) |obj|
//@[022:0022) | ├─SkippedTriviaSyntax
//@[022:0022) | └─SkippedTriviaSyntax
//@[022:0026) ├─Token(NewLine) |\r\n\r\n|

// malformed identifier
//@[023:0025) ├─Token(NewLine) |\r\n|
output 2
//@[000:0008) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─SkippedTriviaSyntax
//@[007:0008) | |   └─Token(Integer) |2|
//@[008:0008) | ├─SkippedTriviaSyntax
//@[008:0008) | ├─SkippedTriviaSyntax
//@[008:0008) | └─SkippedTriviaSyntax
//@[008:0012) ├─Token(NewLine) |\r\n\r\n|

// malformed type
//@[017:0019) ├─Token(NewLine) |\r\n|
output malformedType 3
//@[000:0022) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0020) | ├─IdentifierSyntax
//@[007:0020) | | └─Token(Identifier) |malformedType|
//@[021:0022) | ├─SkippedTriviaSyntax
//@[021:0022) | | └─Token(Integer) |3|
//@[022:0022) | ├─SkippedTriviaSyntax
//@[022:0022) | └─SkippedTriviaSyntax
//@[022:0026) ├─Token(NewLine) |\r\n\r\n|

// malformed type but type check should still happen
//@[052:0054) ├─Token(NewLine) |\r\n|
output malformedType2 3 = 2 + null
//@[000:0034) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0021) | ├─IdentifierSyntax
//@[007:0021) | | └─Token(Identifier) |malformedType2|
//@[022:0023) | ├─SkippedTriviaSyntax
//@[022:0023) | | └─Token(Integer) |3|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0034) | └─BinaryOperationSyntax
//@[026:0027) |   ├─IntegerLiteralSyntax
//@[026:0027) |   | └─Token(Integer) |2|
//@[028:0029) |   ├─Token(Plus) |+|
//@[030:0034) |   └─NullLiteralSyntax
//@[030:0034) |     └─Token(NullKeyword) |null|
//@[034:0038) ├─Token(NewLine) |\r\n\r\n|

// malformed type assignment
//@[028:0030) ├─Token(NewLine) |\r\n|
output malformedAssignment 2 = 2
//@[000:0032) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0026) | ├─IdentifierSyntax
//@[007:0026) | | └─Token(Identifier) |malformedAssignment|
//@[027:0028) | ├─SkippedTriviaSyntax
//@[027:0028) | | └─Token(Integer) |2|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0032) | └─IntegerLiteralSyntax
//@[031:0032) |   └─Token(Integer) |2|
//@[032:0036) ├─Token(NewLine) |\r\n\r\n|

// malformed type before assignment
//@[035:0037) ├─Token(NewLine) |\r\n|
output lol 2 = true
//@[000:0019) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |lol|
//@[011:0012) | ├─SkippedTriviaSyntax
//@[011:0012) | | └─Token(Integer) |2|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0019) | └─BooleanLiteralSyntax
//@[015:0019) |   └─Token(TrueKeyword) |true|
//@[019:0023) ├─Token(NewLine) |\r\n\r\n|

// wrong type + missing value
//@[029:0031) ├─Token(NewLine) |\r\n|
output foo fluffy
//@[000:0017) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |foo|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |fluffy|
//@[017:0017) | ├─SkippedTriviaSyntax
//@[017:0017) | └─SkippedTriviaSyntax
//@[017:0021) ├─Token(NewLine) |\r\n\r\n|

// missing value
//@[016:0018) ├─Token(NewLine) |\r\n|
output foo string
//@[000:0017) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |foo|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |string|
//@[017:0017) | ├─SkippedTriviaSyntax
//@[017:0017) | └─SkippedTriviaSyntax
//@[017:0021) ├─Token(NewLine) |\r\n\r\n|

// missing value
//@[016:0018) ├─Token(NewLine) |\r\n|
output foo string =
//@[000:0019) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |foo|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |string|
//@[018:0019) | ├─Token(Assignment) |=|
//@[019:0019) | └─SkippedTriviaSyntax
//@[019:0023) ├─Token(NewLine) |\r\n\r\n|

// wrong string output values
//@[029:0031) ├─Token(NewLine) |\r\n|
output str string = true
//@[000:0024) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |str|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |string|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0024) | └─BooleanLiteralSyntax
//@[020:0024) |   └─Token(TrueKeyword) |true|
//@[024:0026) ├─Token(NewLine) |\r\n|
output str string = false
//@[000:0025) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |str|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |string|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0025) | └─BooleanLiteralSyntax
//@[020:0025) |   └─Token(FalseKeyword) |false|
//@[025:0027) ├─Token(NewLine) |\r\n|
output str string = [
//@[000:0024) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |str|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |string|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0024) | └─ArraySyntax
//@[020:0021) |   ├─Token(LeftSquare) |[|
//@[021:0023) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\r\n|
output str string = {
//@[000:0024) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |str|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |string|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0024) | └─ObjectSyntax
//@[020:0021) |   ├─Token(LeftBrace) |{|
//@[021:0023) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|
output str string = 52
//@[000:0022) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |str|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |string|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0022) | └─IntegerLiteralSyntax
//@[020:0022) |   └─Token(Integer) |52|
//@[022:0026) ├─Token(NewLine) |\r\n\r\n|

// wrong int output values
//@[026:0028) ├─Token(NewLine) |\r\n|
output i int = true
//@[000:0019) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |i|
//@[009:0012) | ├─SimpleTypeSyntax
//@[009:0012) | | └─Token(Identifier) |int|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0019) | └─BooleanLiteralSyntax
//@[015:0019) |   └─Token(TrueKeyword) |true|
//@[019:0021) ├─Token(NewLine) |\r\n|
output i int = false
//@[000:0020) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |i|
//@[009:0012) | ├─SimpleTypeSyntax
//@[009:0012) | | └─Token(Identifier) |int|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0020) | └─BooleanLiteralSyntax
//@[015:0020) |   └─Token(FalseKeyword) |false|
//@[020:0022) ├─Token(NewLine) |\r\n|
output i int = [
//@[000:0019) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |i|
//@[009:0012) | ├─SimpleTypeSyntax
//@[009:0012) | | └─Token(Identifier) |int|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0019) | └─ArraySyntax
//@[015:0016) |   ├─Token(LeftSquare) |[|
//@[016:0018) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\r\n|
output i int = }
//@[000:0016) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |i|
//@[009:0012) | ├─SimpleTypeSyntax
//@[009:0012) | | └─Token(Identifier) |int|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0016) | └─SkippedTriviaSyntax
//@[015:0016) |   └─Token(RightBrace) |}|
//@[016:0018) ├─Token(NewLine) |\r\n|
}
//@[000:0001) ├─SkippedTriviaSyntax
//@[000:0001) | └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|
output i int = 'test'
//@[000:0021) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |i|
//@[009:0012) | ├─SimpleTypeSyntax
//@[009:0012) | | └─Token(Identifier) |int|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0021) | └─StringSyntax
//@[015:0021) |   └─Token(StringComplete) |'test'|
//@[021:0025) ├─Token(NewLine) |\r\n\r\n|

// wrong bool output values
//@[027:0029) ├─Token(NewLine) |\r\n|
output b bool = [
//@[000:0020) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |b|
//@[009:0013) | ├─SimpleTypeSyntax
//@[009:0013) | | └─Token(Identifier) |bool|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0020) | └─ArraySyntax
//@[016:0017) |   ├─Token(LeftSquare) |[|
//@[017:0019) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\r\n|
output b bool = {
//@[000:0020) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |b|
//@[009:0013) | ├─SimpleTypeSyntax
//@[009:0013) | | └─Token(Identifier) |bool|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0020) | └─ObjectSyntax
//@[016:0017) |   ├─Token(LeftBrace) |{|
//@[017:0019) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|
output b bool = 32
//@[000:0018) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |b|
//@[009:0013) | ├─SimpleTypeSyntax
//@[009:0013) | | └─Token(Identifier) |bool|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0018) | └─IntegerLiteralSyntax
//@[016:0018) |   └─Token(Integer) |32|
//@[018:0020) ├─Token(NewLine) |\r\n|
output b bool = 'str'
//@[000:0021) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |b|
//@[009:0013) | ├─SimpleTypeSyntax
//@[009:0013) | | └─Token(Identifier) |bool|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0021) | └─StringSyntax
//@[016:0021) |   └─Token(StringComplete) |'str'|
//@[021:0025) ├─Token(NewLine) |\r\n\r\n|

// wrong array output values
//@[028:0030) ├─Token(NewLine) |\r\n|
output arr array = 32
//@[000:0021) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |arr|
//@[011:0016) | ├─SimpleTypeSyntax
//@[011:0016) | | └─Token(Identifier) |array|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0021) | └─IntegerLiteralSyntax
//@[019:0021) |   └─Token(Integer) |32|
//@[021:0023) ├─Token(NewLine) |\r\n|
output arr array = true
//@[000:0023) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |arr|
//@[011:0016) | ├─SimpleTypeSyntax
//@[011:0016) | | └─Token(Identifier) |array|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0023) | └─BooleanLiteralSyntax
//@[019:0023) |   └─Token(TrueKeyword) |true|
//@[023:0025) ├─Token(NewLine) |\r\n|
output arr array = false
//@[000:0024) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |arr|
//@[011:0016) | ├─SimpleTypeSyntax
//@[011:0016) | | └─Token(Identifier) |array|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0024) | └─BooleanLiteralSyntax
//@[019:0024) |   └─Token(FalseKeyword) |false|
//@[024:0026) ├─Token(NewLine) |\r\n|
output arr array = {
//@[000:0023) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |arr|
//@[011:0016) | ├─SimpleTypeSyntax
//@[011:0016) | | └─Token(Identifier) |array|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0023) | └─ObjectSyntax
//@[019:0020) |   ├─Token(LeftBrace) |{|
//@[020:0022) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|
output arr array = 'str'
//@[000:0024) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |arr|
//@[011:0016) | ├─SimpleTypeSyntax
//@[011:0016) | | └─Token(Identifier) |array|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0024) | └─StringSyntax
//@[019:0024) |   └─Token(StringComplete) |'str'|
//@[024:0028) ├─Token(NewLine) |\r\n\r\n|

// wrong object output values
//@[029:0031) ├─Token(NewLine) |\r\n|
output o object = 32
//@[000:0020) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |o|
//@[009:0015) | ├─SimpleTypeSyntax
//@[009:0015) | | └─Token(Identifier) |object|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0020) | └─IntegerLiteralSyntax
//@[018:0020) |   └─Token(Integer) |32|
//@[020:0022) ├─Token(NewLine) |\r\n|
output o object = true
//@[000:0022) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |o|
//@[009:0015) | ├─SimpleTypeSyntax
//@[009:0015) | | └─Token(Identifier) |object|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0022) | └─BooleanLiteralSyntax
//@[018:0022) |   └─Token(TrueKeyword) |true|
//@[022:0024) ├─Token(NewLine) |\r\n|
output o object = false
//@[000:0023) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |o|
//@[009:0015) | ├─SimpleTypeSyntax
//@[009:0015) | | └─Token(Identifier) |object|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0023) | └─BooleanLiteralSyntax
//@[018:0023) |   └─Token(FalseKeyword) |false|
//@[023:0025) ├─Token(NewLine) |\r\n|
output o object = [
//@[000:0022) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |o|
//@[009:0015) | ├─SimpleTypeSyntax
//@[009:0015) | | └─Token(Identifier) |object|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0022) | └─ArraySyntax
//@[018:0019) |   ├─Token(LeftSquare) |[|
//@[019:0021) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\r\n|
output o object = 'str'
//@[000:0023) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0008) | ├─IdentifierSyntax
//@[007:0008) | | └─Token(Identifier) |o|
//@[009:0015) | ├─SimpleTypeSyntax
//@[009:0015) | | └─Token(Identifier) |object|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0023) | └─StringSyntax
//@[018:0023) |   └─Token(StringComplete) |'str'|
//@[023:0027) ├─Token(NewLine) |\r\n\r\n|

// a few expression cases
//@[025:0027) ├─Token(NewLine) |\r\n|
output exp string = 2 + 3
//@[000:0025) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |exp|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |string|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0025) | └─BinaryOperationSyntax
//@[020:0021) |   ├─IntegerLiteralSyntax
//@[020:0021) |   | └─Token(Integer) |2|
//@[022:0023) |   ├─Token(Plus) |+|
//@[024:0025) |   └─IntegerLiteralSyntax
//@[024:0025) |     └─Token(Integer) |3|
//@[025:0027) ├─Token(NewLine) |\r\n|
output union string = true ? 's' : 1
//@[000:0036) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0012) | ├─IdentifierSyntax
//@[007:0012) | | └─Token(Identifier) |union|
//@[013:0019) | ├─SimpleTypeSyntax
//@[013:0019) | | └─Token(Identifier) |string|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0036) | └─TernaryOperationSyntax
//@[022:0026) |   ├─BooleanLiteralSyntax
//@[022:0026) |   | └─Token(TrueKeyword) |true|
//@[027:0028) |   ├─Token(Question) |?|
//@[029:0032) |   ├─StringSyntax
//@[029:0032) |   | └─Token(StringComplete) |'s'|
//@[033:0034) |   ├─Token(Colon) |:|
//@[035:0036) |   └─IntegerLiteralSyntax
//@[035:0036) |     └─Token(Integer) |1|
//@[036:0038) ├─Token(NewLine) |\r\n|
output bad int = true && !4
//@[000:0027) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |bad|
//@[011:0014) | ├─SimpleTypeSyntax
//@[011:0014) | | └─Token(Identifier) |int|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0027) | └─BinaryOperationSyntax
//@[017:0021) |   ├─BooleanLiteralSyntax
//@[017:0021) |   | └─Token(TrueKeyword) |true|
//@[022:0024) |   ├─Token(LogicalAnd) |&&|
//@[025:0027) |   └─UnaryOperationSyntax
//@[025:0026) |     ├─Token(Exclamation) |!|
//@[026:0027) |     └─IntegerLiteralSyntax
//@[026:0027) |       └─Token(Integer) |4|
//@[027:0029) ├─Token(NewLine) |\r\n|
output deeper bool = true ? -true : (14 && 's') + 10
//@[000:0052) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0013) | ├─IdentifierSyntax
//@[007:0013) | | └─Token(Identifier) |deeper|
//@[014:0018) | ├─SimpleTypeSyntax
//@[014:0018) | | └─Token(Identifier) |bool|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0052) | └─TernaryOperationSyntax
//@[021:0025) |   ├─BooleanLiteralSyntax
//@[021:0025) |   | └─Token(TrueKeyword) |true|
//@[026:0027) |   ├─Token(Question) |?|
//@[028:0033) |   ├─UnaryOperationSyntax
//@[028:0029) |   | ├─Token(Minus) |-|
//@[029:0033) |   | └─BooleanLiteralSyntax
//@[029:0033) |   |   └─Token(TrueKeyword) |true|
//@[034:0035) |   ├─Token(Colon) |:|
//@[036:0052) |   └─BinaryOperationSyntax
//@[036:0047) |     ├─ParenthesizedExpressionSyntax
//@[036:0037) |     | ├─Token(LeftParen) |(|
//@[037:0046) |     | ├─BinaryOperationSyntax
//@[037:0039) |     | | ├─IntegerLiteralSyntax
//@[037:0039) |     | | | └─Token(Integer) |14|
//@[040:0042) |     | | ├─Token(LogicalAnd) |&&|
//@[043:0046) |     | | └─StringSyntax
//@[043:0046) |     | |   └─Token(StringComplete) |'s'|
//@[046:0047) |     | └─Token(RightParen) |)|
//@[048:0049) |     ├─Token(Plus) |+|
//@[050:0052) |     └─IntegerLiteralSyntax
//@[050:0052) |       └─Token(Integer) |10|
//@[052:0056) ├─Token(NewLine) |\r\n\r\n|

output myOutput string = 'hello'
//@[000:0032) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |myOutput|
//@[016:0022) | ├─SimpleTypeSyntax
//@[016:0022) | | └─Token(Identifier) |string|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0032) | └─StringSyntax
//@[025:0032) |   └─Token(StringComplete) |'hello'|
//@[032:0034) ├─Token(NewLine) |\r\n|
var attemptToReferenceAnOutput = myOutput
//@[000:0041) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0030) | ├─IdentifierSyntax
//@[004:0030) | | └─Token(Identifier) |attemptToReferenceAnOutput|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0041) | └─VariableAccessSyntax
//@[033:0041) |   └─IdentifierSyntax
//@[033:0041) |     └─Token(Identifier) |myOutput|
//@[041:0045) ├─Token(NewLine) |\r\n\r\n|

@sys.maxValue(20)
//@[000:0073) ├─OutputDeclarationSyntax
//@[000:0017) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0017) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0013) | |   ├─IdentifierSyntax
//@[005:0013) | |   | └─Token(Identifier) |maxValue|
//@[013:0014) | |   ├─Token(LeftParen) |(|
//@[014:0016) | |   ├─FunctionArgumentSyntax
//@[014:0016) | |   | └─IntegerLiteralSyntax
//@[014:0016) | |   |   └─Token(Integer) |20|
//@[016:0017) | |   └─Token(RightParen) |)|
//@[017:0019) | ├─Token(NewLine) |\r\n|
@minValue(10)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0012) | |   ├─FunctionArgumentSyntax
//@[010:0012) | |   | └─IntegerLiteralSyntax
//@[010:0012) | |   |   └─Token(Integer) |10|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0015) | ├─Token(NewLine) |\r\n|
output notAttachableDecorators int = 32
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0030) | ├─IdentifierSyntax
//@[007:0030) | | └─Token(Identifier) |notAttachableDecorators|
//@[031:0034) | ├─SimpleTypeSyntax
//@[031:0034) | | └─Token(Identifier) |int|
//@[035:0036) | ├─Token(Assignment) |=|
//@[037:0039) | └─IntegerLiteralSyntax
//@[037:0039) |   └─Token(Integer) |32|
//@[039:0043) ├─Token(NewLine) |\r\n\r\n|

// nested loops inside output loops are not supported
//@[053:0055) ├─Token(NewLine) |\r\n|
output noNestedLoops array = [for thing in things: {
//@[000:0110) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0020) | ├─IdentifierSyntax
//@[007:0020) | | └─Token(Identifier) |noNestedLoops|
//@[021:0026) | ├─SimpleTypeSyntax
//@[021:0026) | | └─Token(Identifier) |array|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0110) | └─ForSyntax
//@[029:0030) |   ├─Token(LeftSquare) |[|
//@[030:0033) |   ├─Token(Identifier) |for|
//@[034:0039) |   ├─LocalVariableSyntax
//@[034:0039) |   | └─IdentifierSyntax
//@[034:0039) |   |   └─Token(Identifier) |thing|
//@[040:0042) |   ├─Token(Identifier) |in|
//@[043:0049) |   ├─VariableAccessSyntax
//@[043:0049) |   | └─IdentifierSyntax
//@[043:0049) |   |   └─Token(Identifier) |things|
//@[049:0050) |   ├─Token(Colon) |:|
//@[051:0109) |   ├─ObjectSyntax
//@[051:0052) |   | ├─Token(LeftBrace) |{|
//@[052:0054) |   | ├─Token(NewLine) |\r\n|
  something: [
//@[002:0052) |   | ├─ObjectPropertySyntax
//@[002:0011) |   | | ├─IdentifierSyntax
//@[002:0011) |   | | | └─Token(Identifier) |something|
//@[011:0012) |   | | ├─Token(Colon) |:|
//@[013:0052) |   | | └─ArraySyntax
//@[013:0014) |   | |   ├─Token(LeftSquare) |[|
//@[014:0016) |   | |   ├─Token(NewLine) |\r\n|
    [for thing in things: true]
//@[004:0031) |   | |   ├─ArrayItemSyntax
//@[004:0031) |   | |   | └─ForSyntax
//@[004:0005) |   | |   |   ├─Token(LeftSquare) |[|
//@[005:0008) |   | |   |   ├─Token(Identifier) |for|
//@[009:0014) |   | |   |   ├─LocalVariableSyntax
//@[009:0014) |   | |   |   | └─IdentifierSyntax
//@[009:0014) |   | |   |   |   └─Token(Identifier) |thing|
//@[015:0017) |   | |   |   ├─Token(Identifier) |in|
//@[018:0024) |   | |   |   ├─VariableAccessSyntax
//@[018:0024) |   | |   |   | └─IdentifierSyntax
//@[018:0024) |   | |   |   |   └─Token(Identifier) |things|
//@[024:0025) |   | |   |   ├─Token(Colon) |:|
//@[026:0030) |   | |   |   ├─BooleanLiteralSyntax
//@[026:0030) |   | |   |   | └─Token(TrueKeyword) |true|
//@[030:0031) |   | |   |   └─Token(RightSquare) |]|
//@[031:0033) |   | |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   | |   └─Token(RightSquare) |]|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

// loops in inner properties inside outputs are not supported
//@[061:0063) ├─Token(NewLine) |\r\n|
output noInnerLoopsInOutputs object = {
//@[000:0074) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0028) | ├─IdentifierSyntax
//@[007:0028) | | └─Token(Identifier) |noInnerLoopsInOutputs|
//@[029:0035) | ├─SimpleTypeSyntax
//@[029:0035) | | └─Token(Identifier) |object|
//@[036:0037) | ├─Token(Assignment) |=|
//@[038:0074) | └─ObjectSyntax
//@[038:0039) |   ├─Token(LeftBrace) |{|
//@[039:0041) |   ├─Token(NewLine) |\r\n|
  a: [for i in range(0,10): i]
//@[002:0030) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |a|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0030) |   | └─ForSyntax
//@[005:0006) |   |   ├─Token(LeftSquare) |[|
//@[006:0009) |   |   ├─Token(Identifier) |for|
//@[010:0011) |   |   ├─LocalVariableSyntax
//@[010:0011) |   |   | └─IdentifierSyntax
//@[010:0011) |   |   |   └─Token(Identifier) |i|
//@[012:0014) |   |   ├─Token(Identifier) |in|
//@[015:0026) |   |   ├─FunctionCallSyntax
//@[015:0020) |   |   | ├─IdentifierSyntax
//@[015:0020) |   |   | | └─Token(Identifier) |range|
//@[020:0021) |   |   | ├─Token(LeftParen) |(|
//@[021:0022) |   |   | ├─FunctionArgumentSyntax
//@[021:0022) |   |   | | └─IntegerLiteralSyntax
//@[021:0022) |   |   | |   └─Token(Integer) |0|
//@[022:0023) |   |   | ├─Token(Comma) |,|
//@[023:0025) |   |   | ├─FunctionArgumentSyntax
//@[023:0025) |   |   | | └─IntegerLiteralSyntax
//@[023:0025) |   |   | |   └─Token(Integer) |10|
//@[025:0026) |   |   | └─Token(RightParen) |)|
//@[026:0027) |   |   ├─Token(Colon) |:|
//@[028:0029) |   |   ├─VariableAccessSyntax
//@[028:0029) |   |   | └─IdentifierSyntax
//@[028:0029) |   |   |   └─Token(Identifier) |i|
//@[029:0030) |   |   └─Token(RightSquare) |]|
//@[030:0032) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|
output noInnerLoopsInOutputs2 object = {
//@[000:0116) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0029) | ├─IdentifierSyntax
//@[007:0029) | | └─Token(Identifier) |noInnerLoopsInOutputs2|
//@[030:0036) | ├─SimpleTypeSyntax
//@[030:0036) | | └─Token(Identifier) |object|
//@[037:0038) | ├─Token(Assignment) |=|
//@[039:0116) | └─ObjectSyntax
//@[039:0040) |   ├─Token(LeftBrace) |{|
//@[040:0042) |   ├─Token(NewLine) |\r\n|
  a: [for i in range(0,10): {
//@[002:0071) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |a|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0071) |   | └─ForSyntax
//@[005:0006) |   |   ├─Token(LeftSquare) |[|
//@[006:0009) |   |   ├─Token(Identifier) |for|
//@[010:0011) |   |   ├─LocalVariableSyntax
//@[010:0011) |   |   | └─IdentifierSyntax
//@[010:0011) |   |   |   └─Token(Identifier) |i|
//@[012:0014) |   |   ├─Token(Identifier) |in|
//@[015:0026) |   |   ├─FunctionCallSyntax
//@[015:0020) |   |   | ├─IdentifierSyntax
//@[015:0020) |   |   | | └─Token(Identifier) |range|
//@[020:0021) |   |   | ├─Token(LeftParen) |(|
//@[021:0022) |   |   | ├─FunctionArgumentSyntax
//@[021:0022) |   |   | | └─IntegerLiteralSyntax
//@[021:0022) |   |   | |   └─Token(Integer) |0|
//@[022:0023) |   |   | ├─Token(Comma) |,|
//@[023:0025) |   |   | ├─FunctionArgumentSyntax
//@[023:0025) |   |   | | └─IntegerLiteralSyntax
//@[023:0025) |   |   | |   └─Token(Integer) |10|
//@[025:0026) |   |   | └─Token(RightParen) |)|
//@[026:0027) |   |   ├─Token(Colon) |:|
//@[028:0070) |   |   ├─ObjectSyntax
//@[028:0029) |   |   | ├─Token(LeftBrace) |{|
//@[029:0031) |   |   | ├─Token(NewLine) |\r\n|
    b: [for j in range(0,10): i+j]
//@[004:0034) |   |   | ├─ObjectPropertySyntax
//@[004:0005) |   |   | | ├─IdentifierSyntax
//@[004:0005) |   |   | | | └─Token(Identifier) |b|
//@[005:0006) |   |   | | ├─Token(Colon) |:|
//@[007:0034) |   |   | | └─ForSyntax
//@[007:0008) |   |   | |   ├─Token(LeftSquare) |[|
//@[008:0011) |   |   | |   ├─Token(Identifier) |for|
//@[012:0013) |   |   | |   ├─LocalVariableSyntax
//@[012:0013) |   |   | |   | └─IdentifierSyntax
//@[012:0013) |   |   | |   |   └─Token(Identifier) |j|
//@[014:0016) |   |   | |   ├─Token(Identifier) |in|
//@[017:0028) |   |   | |   ├─FunctionCallSyntax
//@[017:0022) |   |   | |   | ├─IdentifierSyntax
//@[017:0022) |   |   | |   | | └─Token(Identifier) |range|
//@[022:0023) |   |   | |   | ├─Token(LeftParen) |(|
//@[023:0024) |   |   | |   | ├─FunctionArgumentSyntax
//@[023:0024) |   |   | |   | | └─IntegerLiteralSyntax
//@[023:0024) |   |   | |   | |   └─Token(Integer) |0|
//@[024:0025) |   |   | |   | ├─Token(Comma) |,|
//@[025:0027) |   |   | |   | ├─FunctionArgumentSyntax
//@[025:0027) |   |   | |   | | └─IntegerLiteralSyntax
//@[025:0027) |   |   | |   | |   └─Token(Integer) |10|
//@[027:0028) |   |   | |   | └─Token(RightParen) |)|
//@[028:0029) |   |   | |   ├─Token(Colon) |:|
//@[030:0033) |   |   | |   ├─BinaryOperationSyntax
//@[030:0031) |   |   | |   | ├─VariableAccessSyntax
//@[030:0031) |   |   | |   | | └─IdentifierSyntax
//@[030:0031) |   |   | |   | |   └─Token(Identifier) |i|
//@[031:0032) |   |   | |   | ├─Token(Plus) |+|
//@[032:0033) |   |   | |   | └─VariableAccessSyntax
//@[032:0033) |   |   | |   |   └─IdentifierSyntax
//@[032:0033) |   |   | |   |     └─Token(Identifier) |j|
//@[033:0034) |   |   | |   └─Token(RightSquare) |]|
//@[034:0036) |   |   | ├─Token(NewLine) |\r\n|
  }]
//@[002:0003) |   |   | └─Token(RightBrace) |}|
//@[003:0004) |   |   └─Token(RightSquare) |]|
//@[004:0006) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

//KeyVault Secret Reference
//@[027:0029) ├─Token(NewLine) |\r\n|
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0090) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0011) | ├─IdentifierSyntax
//@[009:0011) | | └─Token(Identifier) |kv|
//@[012:0050) | ├─StringSyntax
//@[012:0050) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[051:0059) | ├─Token(Identifier) |existing|
//@[060:0061) | ├─Token(Assignment) |=|
//@[062:0090) | └─ObjectSyntax
//@[062:0063) |   ├─Token(LeftBrace) |{|
//@[063:0065) |   ├─Token(NewLine) |\r\n|
  name: 'testkeyvault'
//@[002:0022) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0022) |   | └─StringSyntax
//@[008:0022) |   |   └─Token(StringComplete) |'testkeyvault'|
//@[022:0024) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

output keyVaultSecretOutput string = kv.getSecret('mySecret')
//@[000:0061) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0027) | ├─IdentifierSyntax
//@[007:0027) | | └─Token(Identifier) |keyVaultSecretOutput|
//@[028:0034) | ├─SimpleTypeSyntax
//@[028:0034) | | └─Token(Identifier) |string|
//@[035:0036) | ├─Token(Assignment) |=|
//@[037:0061) | └─InstanceFunctionCallSyntax
//@[037:0039) |   ├─VariableAccessSyntax
//@[037:0039) |   | └─IdentifierSyntax
//@[037:0039) |   |   └─Token(Identifier) |kv|
//@[039:0040) |   ├─Token(Dot) |.|
//@[040:0049) |   ├─IdentifierSyntax
//@[040:0049) |   | └─Token(Identifier) |getSecret|
//@[049:0050) |   ├─Token(LeftParen) |(|
//@[050:0060) |   ├─FunctionArgumentSyntax
//@[050:0060) |   | └─StringSyntax
//@[050:0060) |   |   └─Token(StringComplete) |'mySecret'|
//@[060:0061) |   └─Token(RightParen) |)|
//@[061:0063) ├─Token(NewLine) |\r\n|
output keyVaultSecretInterpolatedOutput string = '${kv.getSecret('mySecret')}'
//@[000:0078) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0039) | ├─IdentifierSyntax
//@[007:0039) | | └─Token(Identifier) |keyVaultSecretInterpolatedOutput|
//@[040:0046) | ├─SimpleTypeSyntax
//@[040:0046) | | └─Token(Identifier) |string|
//@[047:0048) | ├─Token(Assignment) |=|
//@[049:0078) | └─StringSyntax
//@[049:0052) |   ├─Token(StringLeftPiece) |'${|
//@[052:0076) |   ├─InstanceFunctionCallSyntax
//@[052:0054) |   | ├─VariableAccessSyntax
//@[052:0054) |   | | └─IdentifierSyntax
//@[052:0054) |   | |   └─Token(Identifier) |kv|
//@[054:0055) |   | ├─Token(Dot) |.|
//@[055:0064) |   | ├─IdentifierSyntax
//@[055:0064) |   | | └─Token(Identifier) |getSecret|
//@[064:0065) |   | ├─Token(LeftParen) |(|
//@[065:0075) |   | ├─FunctionArgumentSyntax
//@[065:0075) |   | | └─StringSyntax
//@[065:0075) |   | |   └─Token(StringComplete) |'mySecret'|
//@[075:0076) |   | └─Token(RightParen) |)|
//@[076:0078) |   └─Token(StringRightPiece) |}'|
//@[078:0080) ├─Token(NewLine) |\r\n|
output keyVaultSecretObjectOutput object = {
//@[000:0083) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0033) | ├─IdentifierSyntax
//@[007:0033) | | └─Token(Identifier) |keyVaultSecretObjectOutput|
//@[034:0040) | ├─SimpleTypeSyntax
//@[034:0040) | | └─Token(Identifier) |object|
//@[041:0042) | ├─Token(Assignment) |=|
//@[043:0083) | └─ObjectSyntax
//@[043:0044) |   ├─Token(LeftBrace) |{|
//@[044:0046) |   ├─Token(NewLine) |\r\n|
  secret: kv.getSecret('mySecret')
//@[002:0034) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |secret|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0034) |   | └─InstanceFunctionCallSyntax
//@[010:0012) |   |   ├─VariableAccessSyntax
//@[010:0012) |   |   | └─IdentifierSyntax
//@[010:0012) |   |   |   └─Token(Identifier) |kv|
//@[012:0013) |   |   ├─Token(Dot) |.|
//@[013:0022) |   |   ├─IdentifierSyntax
//@[013:0022) |   |   | └─Token(Identifier) |getSecret|
//@[022:0023) |   |   ├─Token(LeftParen) |(|
//@[023:0033) |   |   ├─FunctionArgumentSyntax
//@[023:0033) |   |   | └─StringSyntax
//@[023:0033) |   |   |   └─Token(StringComplete) |'mySecret'|
//@[033:0034) |   |   └─Token(RightParen) |)|
//@[034:0036) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|
output keyVaultSecretArrayOutput array = [
//@[000:0073) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0032) | ├─IdentifierSyntax
//@[007:0032) | | └─Token(Identifier) |keyVaultSecretArrayOutput|
//@[033:0038) | ├─SimpleTypeSyntax
//@[033:0038) | | └─Token(Identifier) |array|
//@[039:0040) | ├─Token(Assignment) |=|
//@[041:0073) | └─ArraySyntax
//@[041:0042) |   ├─Token(LeftSquare) |[|
//@[042:0044) |   ├─Token(NewLine) |\r\n|
  kv.getSecret('mySecret')
//@[002:0026) |   ├─ArrayItemSyntax
//@[002:0026) |   | └─InstanceFunctionCallSyntax
//@[002:0004) |   |   ├─VariableAccessSyntax
//@[002:0004) |   |   | └─IdentifierSyntax
//@[002:0004) |   |   |   └─Token(Identifier) |kv|
//@[004:0005) |   |   ├─Token(Dot) |.|
//@[005:0014) |   |   ├─IdentifierSyntax
//@[005:0014) |   |   | └─Token(Identifier) |getSecret|
//@[014:0015) |   |   ├─Token(LeftParen) |(|
//@[015:0025) |   |   ├─FunctionArgumentSyntax
//@[015:0025) |   |   | └─StringSyntax
//@[015:0025) |   |   |   └─Token(StringComplete) |'mySecret'|
//@[025:0026) |   |   └─Token(RightParen) |)|
//@[026:0028) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\r\n|
output keyVaultSecretArrayInterpolatedOutput array = [
//@[000:0090) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0044) | ├─IdentifierSyntax
//@[007:0044) | | └─Token(Identifier) |keyVaultSecretArrayInterpolatedOutput|
//@[045:0050) | ├─SimpleTypeSyntax
//@[045:0050) | | └─Token(Identifier) |array|
//@[051:0052) | ├─Token(Assignment) |=|
//@[053:0090) | └─ArraySyntax
//@[053:0054) |   ├─Token(LeftSquare) |[|
//@[054:0056) |   ├─Token(NewLine) |\r\n|
  '${kv.getSecret('mySecret')}'
//@[002:0031) |   ├─ArrayItemSyntax
//@[002:0031) |   | └─StringSyntax
//@[002:0005) |   |   ├─Token(StringLeftPiece) |'${|
//@[005:0029) |   |   ├─InstanceFunctionCallSyntax
//@[005:0007) |   |   | ├─VariableAccessSyntax
//@[005:0007) |   |   | | └─IdentifierSyntax
//@[005:0007) |   |   | |   └─Token(Identifier) |kv|
//@[007:0008) |   |   | ├─Token(Dot) |.|
//@[008:0017) |   |   | ├─IdentifierSyntax
//@[008:0017) |   |   | | └─Token(Identifier) |getSecret|
//@[017:0018) |   |   | ├─Token(LeftParen) |(|
//@[018:0028) |   |   | ├─FunctionArgumentSyntax
//@[018:0028) |   |   | | └─StringSyntax
//@[018:0028) |   |   | |   └─Token(StringComplete) |'mySecret'|
//@[028:0029) |   |   | └─Token(RightParen) |)|
//@[029:0031) |   |   └─Token(StringRightPiece) |}'|
//@[031:0033) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

// WARNING!!!!! dangling decorators
//@[035:0039) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(1) -> decoratorsPlusNamespace
//@[048:0050) ├─Token(NewLine) |\r\n|
@
//@[000:0049) ├─MissingDeclarationSyntax
//@[000:0001) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0001) | | └─SkippedTriviaSyntax
//@[001:0003) | ├─Token(NewLine) |\r\n|
// #completionTest(5) -> decorators
//@[035:0037) | ├─Token(NewLine) |\r\n|
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
//@[005:0009) | └─Token(NewLine) |\r\n\r\n|

// WARNING!!!!! dangling decorators - to make sure the tests work, please do not add contents after this line 
//@[110:0110) └─Token(EndOfFile) ||
