targetScope
//@[0:11) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[11:11)  SkippedTriviaSyntax
//@[11:11)  SkippedTriviaSyntax
//@[11:13) NewLine |\n\n|

// #completionTest(12) -> empty
//@[31:32) NewLine |\n|
targetScope 
//@[0:12) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:12)  SkippedTriviaSyntax
//@[12:12)  SkippedTriviaSyntax
//@[12:14) NewLine |\n\n|

// #completionTest(13,14) -> targetScopes
//@[41:42) NewLine |\n|
targetScope = 
//@[0:14) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:14)  SkippedTriviaSyntax
//@[14:14) EndOfFile ||
