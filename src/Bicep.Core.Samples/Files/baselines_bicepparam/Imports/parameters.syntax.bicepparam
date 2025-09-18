using 'main.bicep'
//@[000:297) ProgramSyntax
//@[000:018) ├─UsingDeclarationSyntax
//@[000:005) | ├─Token(Identifier) |using|
//@[006:018) | ├─StringSyntax
//@[006:018) | | └─Token(StringComplete) |'main.bicep'|
//@[018:018) | └─SkippedTriviaSyntax
//@[018:019) ├─Token(NewLine) |\n|
import * as bicepconfig from 'bicepconfig.bicep'
//@[000:048) ├─CompileTimeImportDeclarationSyntax
//@[000:006) | ├─Token(Identifier) |import|
//@[007:023) | ├─WildcardImportSyntax
//@[007:008) | | ├─Token(Asterisk) |*|
//@[009:023) | | └─AliasAsClauseSyntax
//@[009:011) | |   ├─Token(Identifier) |as|
//@[012:023) | |   └─IdentifierSyntax
//@[012:023) | |     └─Token(Identifier) |bicepconfig|
//@[024:048) | └─CompileTimeImportFromClauseSyntax
//@[024:028) |   ├─Token(Identifier) |from|
//@[029:048) |   └─StringSyntax
//@[029:048) |     └─Token(StringComplete) |'bicepconfig.bicep'|
//@[048:049) ├─Token(NewLine) |\n|
// ok
//@[005:006) ├─Token(NewLine) |\n|
param one = bicepconfig.directExport
//@[000:036) ├─ParameterAssignmentSyntax
//@[000:005) | ├─Token(Identifier) |param|
//@[006:009) | ├─IdentifierSyntax
//@[006:009) | | └─Token(Identifier) |one|
//@[010:011) | ├─Token(Assignment) |=|
//@[012:036) | └─PropertyAccessSyntax
//@[012:023) |   ├─VariableAccessSyntax
//@[012:023) |   | └─IdentifierSyntax
//@[012:023) |   |   └─Token(Identifier) |bicepconfig|
//@[023:024) |   ├─Token(Dot) |.|
//@[024:036) |   └─IdentifierSyntax
//@[024:036) |     └─Token(Identifier) |directExport|
//@[036:037) ├─Token(NewLine) |\n|
// Failed to evaluate parameter "two"
//@[037:038) ├─Token(NewLine) |\n|
// Unhandled exception during evaluating template language function 'variables' is not handled.bicep(BCP338)
//@[108:109) ├─Token(NewLine) |\n|
param two = bicepconfig.functionExport
//@[000:038) ├─ParameterAssignmentSyntax
//@[000:005) | ├─Token(Identifier) |param|
//@[006:009) | ├─IdentifierSyntax
//@[006:009) | | └─Token(Identifier) |two|
//@[010:011) | ├─Token(Assignment) |=|
//@[012:038) | └─PropertyAccessSyntax
//@[012:023) |   ├─VariableAccessSyntax
//@[012:023) |   | └─IdentifierSyntax
//@[012:023) |   |   └─Token(Identifier) |bicepconfig|
//@[023:024) |   ├─Token(Dot) |.|
//@[024:038) |   └─IdentifierSyntax
//@[024:038) |     └─Token(Identifier) |functionExport|
//@[038:039) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
