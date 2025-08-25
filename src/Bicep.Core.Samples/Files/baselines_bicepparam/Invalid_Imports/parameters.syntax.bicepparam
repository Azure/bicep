using 'main.bicep'
//@[00:85) ProgramSyntax
//@[00:18) ├─UsingDeclarationSyntax
//@[00:05) | ├─Token(Identifier) |using|
//@[06:18) | ├─StringSyntax
//@[06:18) | | └─Token(StringComplete) |'main.bicep'|
//@[18:18) | └─SkippedTriviaSyntax
//@[18:20) ├─Token(NewLine) |\n\n|

import * as foo from 'foo.bicep'
//@[00:32) ├─CompileTimeImportDeclarationSyntax
//@[00:06) | ├─Token(Identifier) |import|
//@[07:15) | ├─WildcardImportSyntax
//@[07:08) | | ├─Token(Asterisk) |*|
//@[09:15) | | └─AliasAsClauseSyntax
//@[09:11) | |   ├─Token(Identifier) |as|
//@[12:15) | |   └─IdentifierSyntax
//@[12:15) | |     └─Token(Identifier) |foo|
//@[16:32) | └─CompileTimeImportFromClauseSyntax
//@[16:20) |   ├─Token(Identifier) |from|
//@[21:32) |   └─StringSyntax
//@[21:32) |     └─Token(StringComplete) |'foo.bicep'|
//@[32:33) ├─Token(NewLine) |\n|
import { bar } from 'foo.bicep'
//@[00:31) ├─CompileTimeImportDeclarationSyntax
//@[00:06) | ├─Token(Identifier) |import|
//@[07:14) | ├─ImportedSymbolsListSyntax
//@[07:08) | | ├─Token(LeftBrace) |{|
//@[09:12) | | ├─ImportedSymbolsListItemSyntax
//@[09:12) | | | └─IdentifierSyntax
//@[09:12) | | |   └─Token(Identifier) |bar|
//@[13:14) | | └─Token(RightBrace) |}|
//@[15:31) | └─CompileTimeImportFromClauseSyntax
//@[15:19) |   ├─Token(Identifier) |from|
//@[20:31) |   └─StringSyntax
//@[20:31) |     └─Token(StringComplete) |'foo.bicep'|
//@[31:32) ├─Token(NewLine) |\n|

//@[00:00) └─Token(EndOfFile) ||
