using 'main.bicep'
//@[00:97) ProgramSyntax
//@[00:18) ├─UsingDeclarationSyntax
//@[00:05) | ├─Token(Identifier) |using|
//@[06:18) | ├─StringSyntax
//@[06:18) | | └─Token(StringComplete) |'main.bicep'|
//@[18:18) | └─SkippedTriviaSyntax
//@[18:20) ├─Token(NewLine) |\n\n|

extends 'shared.bicepparam'
//@[00:27) ├─ExtendsDeclarationSyntax
//@[00:07) | ├─Token(Identifier) |extends|
//@[08:27) | └─StringSyntax
//@[08:27) |   └─Token(StringComplete) |'shared.bicepparam'|
//@[27:29) ├─Token(NewLine) |\n\n|

param one = 'param one'
//@[00:23) ├─ParameterAssignmentSyntax
//@[00:05) | ├─Token(Identifier) |param|
//@[06:09) | ├─IdentifierSyntax
//@[06:09) | | └─Token(Identifier) |one|
//@[10:11) | ├─Token(Assignment) |=|
//@[12:23) | └─StringSyntax
//@[12:23) |   └─Token(StringComplete) |'param one'|
//@[23:24) ├─Token(NewLine) |\n|
param two = 'param two'
//@[00:23) ├─ParameterAssignmentSyntax
//@[00:05) | ├─Token(Identifier) |param|
//@[06:09) | ├─IdentifierSyntax
//@[06:09) | | └─Token(Identifier) |two|
//@[10:11) | ├─Token(Assignment) |=|
//@[12:23) | └─StringSyntax
//@[12:23) |   └─Token(StringComplete) |'param two'|
//@[23:24) ├─Token(NewLine) |\n|

//@[00:00) └─Token(EndOfFile) ||
