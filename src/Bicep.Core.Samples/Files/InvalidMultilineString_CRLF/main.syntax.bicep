var unterminatedMultilineString = '''
hello!''
//@[0:46) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:31)  IdentifierSyntax
//@[4:31)   Identifier |unterminatedMultilineString|
//@[32:33)  Assignment |=|
//@[34:46)  SkippedTriviaSyntax
//@[34:46)   MultilineString |'''\nhello!''|
