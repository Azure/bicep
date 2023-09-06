targetScope
//@[00:179) ProgramSyntax
//@[00:011) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[11:011) | ├─SkippedTriviaSyntax
//@[11:011) | └─SkippedTriviaSyntax
//@[11:013) ├─Token(NewLine) |\n\n|

// #completionTest(12) -> empty
//@[31:032) ├─Token(NewLine) |\n|
targetScope 
//@[00:012) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[12:012) | ├─SkippedTriviaSyntax
//@[12:012) | └─SkippedTriviaSyntax
//@[12:014) ├─Token(NewLine) |\n\n|

// #completionTest(13,14) -> targetScopes
//@[41:042) ├─Token(NewLine) |\n|
targetScope = 
//@[00:014) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:014) | └─SkippedTriviaSyntax
//@[14:017) ├─Token(NewLine) |\n\n\n|


targetScope = 'asdfds'
//@[00:022) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:022) | └─StringSyntax
//@[14:022) |   └─Token(StringComplete) |'asdfds'|
//@[22:024) ├─Token(NewLine) |\n\n|

targetScope = { }
//@[00:017) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:017) | └─ObjectSyntax
//@[14:015) |   ├─Token(LeftBrace) |{|
//@[16:017) |   └─Token(RightBrace) |}|
//@[17:019) ├─Token(NewLine) |\n\n|

targetScope = true
//@[00:018) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:018) | └─BooleanLiteralSyntax
//@[14:018) |   └─Token(TrueKeyword) |true|
//@[18:018) └─Token(EndOfFile) ||
