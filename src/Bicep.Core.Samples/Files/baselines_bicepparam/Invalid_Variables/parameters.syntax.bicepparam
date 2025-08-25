using 'main.bicep'
//@[00:87) ProgramSyntax
//@[00:18) ├─UsingDeclarationSyntax
//@[00:05) | ├─Token(Identifier) |using|
//@[06:18) | ├─StringSyntax
//@[06:18) | | └─Token(StringComplete) |'main.bicep'|
//@[18:18) | └─SkippedTriviaSyntax
//@[18:22) ├─Token(NewLine) |\r\n\r\n|

var foo
//@[00:07) ├─VariableDeclarationSyntax
//@[00:03) | ├─Token(Identifier) |var|
//@[04:07) | ├─IdentifierSyntax
//@[04:07) | | └─Token(Identifier) |foo|
//@[07:07) | ├─SkippedTriviaSyntax
//@[07:07) | └─SkippedTriviaSyntax
//@[07:11) ├─Token(NewLine) |\r\n\r\n|

var foo2 =
//@[00:10) ├─VariableDeclarationSyntax
//@[00:03) | ├─Token(Identifier) |var|
//@[04:08) | ├─IdentifierSyntax
//@[04:08) | | └─Token(Identifier) |foo2|
//@[09:10) | ├─Token(Assignment) |=|
//@[10:10) | └─SkippedTriviaSyntax
//@[10:14) ├─Token(NewLine) |\r\n\r\n|

var foo3 = asdf
//@[00:15) ├─VariableDeclarationSyntax
//@[00:03) | ├─Token(Identifier) |var|
//@[04:08) | ├─IdentifierSyntax
//@[04:08) | | └─Token(Identifier) |foo3|
//@[09:10) | ├─Token(Assignment) |=|
//@[11:15) | └─VariableAccessSyntax
//@[11:15) |   └─IdentifierSyntax
//@[11:15) |     └─Token(Identifier) |asdf|
//@[15:19) ├─Token(NewLine) |\r\n\r\n|

var foo4 = utcNow()
//@[00:19) ├─VariableDeclarationSyntax
//@[00:03) | ├─Token(Identifier) |var|
//@[04:08) | ├─IdentifierSyntax
//@[04:08) | | └─Token(Identifier) |foo4|
//@[09:10) | ├─Token(Assignment) |=|
//@[11:19) | └─FunctionCallSyntax
//@[11:17) |   ├─IdentifierSyntax
//@[11:17) |   | └─Token(Identifier) |utcNow|
//@[17:18) |   ├─Token(LeftParen) |(|
//@[18:19) |   └─Token(RightParen) |)|
//@[19:21) ├─Token(NewLine) |\r\n|

//@[00:00) └─Token(EndOfFile) ||
