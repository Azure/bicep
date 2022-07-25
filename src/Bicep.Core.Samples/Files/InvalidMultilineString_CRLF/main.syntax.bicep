var unterminatedMultilineString = '''
hello!''
//@[00:46) ProgramSyntax
//@[00:46) ├─VariableDeclarationSyntax
//@[00:03) | ├─Token(Identifier) |var|
//@[04:31) | ├─IdentifierSyntax
//@[04:31) | | └─Token(Identifier) |unterminatedMultilineString|
//@[32:33) | ├─Token(Assignment) |=|
//@[34:46) | └─SkippedTriviaSyntax
//@[34:46) |   └─Token(MultilineString) |'''\nhello!''|
