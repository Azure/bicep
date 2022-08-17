// wrong declaration
//@[00:701) ProgramSyntax
//@[20:022) ├─Token(NewLine) |\r\n|
bad
//@[00:003) ├─SkippedTriviaSyntax
//@[00:003) | └─Token(Identifier) |bad|
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

// blank identifier name
//@[24:026) ├─Token(NewLine) |\r\n|
meta 
//@[00:005) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:005) | ├─IdentifierSyntax
//@[05:005) | | └─SkippedTriviaSyntax
//@[05:005) | ├─SkippedTriviaSyntax
//@[05:005) | └─SkippedTriviaSyntax
//@[05:009) ├─Token(NewLine) |\r\n\r\n|

// invalid identifier name
//@[26:028) ├─Token(NewLine) |\r\n|
meta 2
//@[00:006) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:006) | ├─IdentifierSyntax
//@[05:006) | | └─SkippedTriviaSyntax
//@[05:006) | |   └─Token(Integer) |2|
//@[06:006) | ├─SkippedTriviaSyntax
//@[06:006) | └─SkippedTriviaSyntax
//@[06:008) ├─Token(NewLine) |\r\n|
meta _2
//@[00:007) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:007) | ├─IdentifierSyntax
//@[05:007) | | └─Token(Identifier) |_2|
//@[07:007) | ├─SkippedTriviaSyntax
//@[07:007) | └─SkippedTriviaSyntax
//@[07:011) ├─Token(NewLine) |\r\n\r\n|

// missing value
//@[16:018) ├─Token(NewLine) |\r\n|
meta missingValueAndType = 
//@[00:027) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:024) | ├─IdentifierSyntax
//@[05:024) | | └─Token(Identifier) |missingValueAndType|
//@[25:026) | ├─Token(Assignment) |=|
//@[27:027) | └─SkippedTriviaSyntax
//@[27:031) ├─Token(NewLine) |\r\n\r\n|

meta missingAssignment 'noAssingmentOperator'
//@[00:045) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:022) | ├─IdentifierSyntax
//@[05:022) | | └─Token(Identifier) |missingAssignment|
//@[23:045) | ├─SkippedTriviaSyntax
//@[23:045) | | └─Token(StringComplete) |'noAssingmentOperator'|
//@[45:045) | └─SkippedTriviaSyntax
//@[45:049) ├─Token(NewLine) |\r\n\r\n|

// metadata referencing metadata
//@[32:034) ├─Token(NewLine) |\r\n|
meta myMeta = 'hello'
//@[00:021) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |myMeta|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:021) | └─StringSyntax
//@[14:021) |   └─Token(StringComplete) |'hello'|
//@[21:023) ├─Token(NewLine) |\r\n|
var attemptToReferenceMetadata = myMeta
//@[00:039) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:030) | ├─IdentifierSyntax
//@[04:030) | | └─Token(Identifier) |attemptToReferenceMetadata|
//@[31:032) | ├─Token(Assignment) |=|
//@[33:039) | └─VariableAccessSyntax
//@[33:039) |   └─IdentifierSyntax
//@[33:039) |     └─Token(Identifier) |myMeta|
//@[39:043) ├─Token(NewLine) |\r\n\r\n|

// two meta blocks with same identifier name
//@[44:046) ├─Token(NewLine) |\r\n|
meta same = 'value1'
//@[00:020) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:009) | ├─IdentifierSyntax
//@[05:009) | | └─Token(Identifier) |same|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:020) | └─StringSyntax
//@[12:020) |   └─Token(StringComplete) |'value1'|
//@[20:022) ├─Token(NewLine) |\r\n|
meta same = 'value2'
//@[00:020) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:009) | ├─IdentifierSyntax
//@[05:009) | | └─Token(Identifier) |same|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:020) | └─StringSyntax
//@[12:020) |   └─Token(StringComplete) |'value2'|
//@[20:024) ├─Token(NewLine) |\r\n\r\n|

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
meta test = testSymbol
//@[00:022) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:009) | ├─IdentifierSyntax
//@[05:009) | | └─Token(Identifier) |test|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:022) | └─VariableAccessSyntax
//@[12:022) |   └─IdentifierSyntax
//@[12:022) |     └─Token(Identifier) |testSymbol|
//@[22:028) ├─Token(NewLine) |\r\n\r\n\r\n|


// metadata referencing itself
//@[30:032) ├─Token(NewLine) |\r\n|
meta selfRef = selfRef
//@[00:022) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:012) | ├─IdentifierSyntax
//@[05:012) | | └─Token(Identifier) |selfRef|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:022) | └─VariableAccessSyntax
//@[15:022) |   └─IdentifierSyntax
//@[15:022) |     └─Token(Identifier) |selfRef|
//@[22:026) ├─Token(NewLine) |\r\n\r\n|

// metadata with decorators
//@[27:029) ├─Token(NewLine) |\r\n|
@description('this is a description')
//@[00:083) ├─MetadataDeclarationSyntax
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
meta decoratedDescription = 'hasDescription'
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:025) | ├─IdentifierSyntax
//@[05:025) | | └─Token(Identifier) |decoratedDescription|
//@[26:027) | ├─Token(Assignment) |=|
//@[28:044) | └─StringSyntax
//@[28:044) |   └─Token(StringComplete) |'hasDescription'|
//@[44:048) ├─Token(NewLine) |\r\n\r\n|

@secure()
//@[00:043) ├─MetadataDeclarationSyntax
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |secure|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:011) | ├─Token(NewLine) |\r\n|
meta secureMeta = 'notSupported'
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:015) | ├─IdentifierSyntax
//@[05:015) | | └─Token(Identifier) |secureMeta|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:032) | └─StringSyntax
//@[18:032) |   └─Token(StringComplete) |'notSupported'|
//@[32:036) ├─Token(NewLine) |\r\n\r\n|


//@[00:000) └─Token(EndOfFile) ||
