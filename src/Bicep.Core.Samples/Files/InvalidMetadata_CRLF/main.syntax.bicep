// wrong declaration
//@[00:766) ProgramSyntax
//@[20:022) ├─Token(NewLine) |\r\n|
metadata
//@[00:008) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[08:008) | ├─IdentifierSyntax
//@[08:008) | | └─SkippedTriviaSyntax
//@[08:008) | ├─SkippedTriviaSyntax
//@[08:008) | └─SkippedTriviaSyntax
//@[08:012) ├─Token(NewLine) |\r\n\r\n|

// blank identifier name
//@[24:026) ├─Token(NewLine) |\r\n|
metadata 
//@[00:009) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:009) | ├─IdentifierSyntax
//@[09:009) | | └─SkippedTriviaSyntax
//@[09:009) | ├─SkippedTriviaSyntax
//@[09:009) | └─SkippedTriviaSyntax
//@[09:013) ├─Token(NewLine) |\r\n\r\n|

// invalid identifier name
//@[26:028) ├─Token(NewLine) |\r\n|
metadata 2
//@[00:010) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:010) | ├─IdentifierSyntax
//@[09:010) | | └─SkippedTriviaSyntax
//@[09:010) | |   └─Token(Integer) |2|
//@[10:010) | ├─SkippedTriviaSyntax
//@[10:010) | └─SkippedTriviaSyntax
//@[10:012) ├─Token(NewLine) |\r\n|
metadata _2
//@[00:011) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:011) | ├─IdentifierSyntax
//@[09:011) | | └─Token(Identifier) |_2|
//@[11:011) | ├─SkippedTriviaSyntax
//@[11:011) | └─SkippedTriviaSyntax
//@[11:015) ├─Token(NewLine) |\r\n\r\n|

// missing value
//@[16:018) ├─Token(NewLine) |\r\n|
metadata missingValueAndType = 
//@[00:031) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:028) | ├─IdentifierSyntax
//@[09:028) | | └─Token(Identifier) |missingValueAndType|
//@[29:030) | ├─Token(Assignment) |=|
//@[31:031) | └─SkippedTriviaSyntax
//@[31:035) ├─Token(NewLine) |\r\n\r\n|

metadata missingAssignment 'noAssingmentOperator'
//@[00:049) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:026) | ├─IdentifierSyntax
//@[09:026) | | └─Token(Identifier) |missingAssignment|
//@[27:049) | ├─SkippedTriviaSyntax
//@[27:049) | | └─Token(StringComplete) |'noAssingmentOperator'|
//@[49:049) | └─SkippedTriviaSyntax
//@[49:053) ├─Token(NewLine) |\r\n\r\n|

// metadata referencing metadata
//@[32:034) ├─Token(NewLine) |\r\n|
metadata myMetadata = 'hello'
//@[00:029) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:019) | ├─IdentifierSyntax
//@[09:019) | | └─Token(Identifier) |myMetadata|
//@[20:021) | ├─Token(Assignment) |=|
//@[22:029) | └─StringSyntax
//@[22:029) |   └─Token(StringComplete) |'hello'|
//@[29:031) ├─Token(NewLine) |\r\n|
var attemptToReferenceMetadata = myMetadata
//@[00:043) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:030) | ├─IdentifierSyntax
//@[04:030) | | └─Token(Identifier) |attemptToReferenceMetadata|
//@[31:032) | ├─Token(Assignment) |=|
//@[33:043) | └─VariableAccessSyntax
//@[33:043) |   └─IdentifierSyntax
//@[33:043) |     └─Token(Identifier) |myMetadata|
//@[43:047) ├─Token(NewLine) |\r\n\r\n|

// two meta blocks with same identifier name
//@[44:046) ├─Token(NewLine) |\r\n|
metadata same = 'value1'
//@[00:024) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:013) | ├─IdentifierSyntax
//@[09:013) | | └─Token(Identifier) |same|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:024) | └─StringSyntax
//@[16:024) |   └─Token(StringComplete) |'value1'|
//@[24:026) ├─Token(NewLine) |\r\n|
metadata same = 'value2'
//@[00:024) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:013) | ├─IdentifierSyntax
//@[09:013) | | └─Token(Identifier) |same|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:024) | └─StringSyntax
//@[16:024) |   └─Token(StringComplete) |'value2'|
//@[24:028) ├─Token(NewLine) |\r\n\r\n|

// metadata referencing vars
//@[28:030) ├─Token(NewLine) |\r\n|
var testSymbol = 42
//@[00:019) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:014) | ├─IdentifierSyntax
//@[04:014) | | └─Token(Identifier) |testSymbol|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:019) | └─IntegerLiteralSyntax
//@[17:019) |   └─Token(Integer) |42|
//@[19:021) ├─Token(NewLine) |\r\n|
metadata test = testSymbol
//@[00:026) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:013) | ├─IdentifierSyntax
//@[09:013) | | └─Token(Identifier) |test|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:026) | └─VariableAccessSyntax
//@[16:026) |   └─IdentifierSyntax
//@[16:026) |     └─Token(Identifier) |testSymbol|
//@[26:032) ├─Token(NewLine) |\r\n\r\n\r\n|


// metadata referencing itself
//@[30:032) ├─Token(NewLine) |\r\n|
metadata selfRef = selfRef
//@[00:026) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:016) | ├─IdentifierSyntax
//@[09:016) | | └─Token(Identifier) |selfRef|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:026) | └─VariableAccessSyntax
//@[19:026) |   └─IdentifierSyntax
//@[19:026) |     └─Token(Identifier) |selfRef|
//@[26:030) ├─Token(NewLine) |\r\n\r\n|

// metadata with decorators
//@[27:029) ├─Token(NewLine) |\r\n|
@description('this is a description')
//@[00:087) ├─MetadataDeclarationSyntax
//@[00:037) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:037) | | └─FunctionCallSyntax
//@[01:012) | |   ├─IdentifierSyntax
//@[01:012) | |   | └─Token(Identifier) |description|
//@[12:013) | |   ├─Token(LeftParen) |(|
//@[13:036) | |   ├─FunctionArgumentSyntax
//@[13:036) | |   | └─StringSyntax
//@[13:036) | |   |   └─Token(StringComplete) |'this is a description'|
//@[36:037) | |   └─Token(RightParen) |)|
//@[37:039) | ├─Token(NewLine) |\r\n|
metadata decoratedDescription = 'hasDescription'
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:029) | ├─IdentifierSyntax
//@[09:029) | | └─Token(Identifier) |decoratedDescription|
//@[30:031) | ├─Token(Assignment) |=|
//@[32:048) | └─StringSyntax
//@[32:048) |   └─Token(StringComplete) |'hasDescription'|
//@[48:052) ├─Token(NewLine) |\r\n\r\n|

@secure()
//@[00:051) ├─MetadataDeclarationSyntax
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |secure|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:011) | ├─Token(NewLine) |\r\n|
metadata secureMetadata = 'notSupported'
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:023) | ├─IdentifierSyntax
//@[09:023) | | └─Token(Identifier) |secureMetadata|
//@[24:025) | ├─Token(Assignment) |=|
//@[26:040) | └─StringSyntax
//@[26:040) |   └─Token(StringComplete) |'notSupported'|
//@[40:044) ├─Token(NewLine) |\r\n\r\n|


//@[00:000) └─Token(EndOfFile) ||
