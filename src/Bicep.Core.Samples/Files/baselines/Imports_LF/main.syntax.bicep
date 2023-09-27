import {foo, fizz} from 'modules/mod.bicep'
//@[00:247) ProgramSyntax
//@[00:043) ├─CompileTimeImportDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |import|
//@[07:018) | ├─ImportedSymbolsListSyntax
//@[07:008) | | ├─Token(LeftBrace) |{|
//@[08:011) | | ├─ImportedSymbolsListItemSyntax
//@[08:011) | | | └─IdentifierSyntax
//@[08:011) | | |   └─Token(Identifier) |foo|
//@[11:012) | | ├─Token(Comma) |,|
//@[13:017) | | ├─ImportedSymbolsListItemSyntax
//@[13:017) | | | └─IdentifierSyntax
//@[13:017) | | |   └─Token(Identifier) |fizz|
//@[17:018) | | └─Token(RightBrace) |}|
//@[19:043) | └─CompileTimeImportFromClauseSyntax
//@[19:023) |   ├─Token(Identifier) |from|
//@[24:043) |   └─StringSyntax
//@[24:043) |     └─Token(StringComplete) |'modules/mod.bicep'|
//@[43:044) ├─Token(NewLine) |\n|
import * as mod2 from 'modules/mod2.bicep'
//@[00:042) ├─CompileTimeImportDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |import|
//@[07:016) | ├─WildcardImportSyntax
//@[07:008) | | ├─Token(Asterisk) |*|
//@[09:016) | | └─AliasAsClauseSyntax
//@[09:011) | |   ├─Token(AsKeyword) |as|
//@[12:016) | |   └─IdentifierSyntax
//@[12:016) | |     └─Token(Identifier) |mod2|
//@[17:042) | └─CompileTimeImportFromClauseSyntax
//@[17:021) |   ├─Token(Identifier) |from|
//@[22:042) |   └─StringSyntax
//@[22:042) |     └─Token(StringComplete) |'modules/mod2.bicep'|
//@[42:043) ├─Token(NewLine) |\n|
import {
//@[00:115) ├─CompileTimeImportDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |import|
//@[07:091) | ├─ImportedSymbolsListSyntax
//@[07:008) | | ├─Token(LeftBrace) |{|
//@[08:009) | | ├─Token(NewLine) |\n|
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
//@[02:057) | | ├─ImportedSymbolsListItemSyntax
//@[02:032) | | | ├─StringSyntax
//@[02:032) | | | | └─Token(StringComplete) |'not-a-valid-bicep-identifier'|
//@[33:057) | | | └─AliasAsClauseSyntax
//@[33:035) | | |   ├─Token(AsKeyword) |as|
//@[36:057) | | |   └─IdentifierSyntax
//@[36:057) | | |     └─Token(Identifier) |withInvalidIdentifier|
//@[57:058) | | ├─Token(NewLine) |\n|
  refersToCopyVariable
//@[02:022) | | ├─ImportedSymbolsListItemSyntax
//@[02:022) | | | └─IdentifierSyntax
//@[02:022) | | |   └─Token(Identifier) |refersToCopyVariable|
//@[22:023) | | ├─Token(NewLine) |\n|
} from 'modules/mod.json'
//@[00:001) | | └─Token(RightBrace) |}|
//@[02:025) | └─CompileTimeImportFromClauseSyntax
//@[02:006) |   ├─Token(Identifier) |from|
//@[07:025) |   └─StringSyntax
//@[07:025) |     └─Token(StringComplete) |'modules/mod.json'|
//@[25:027) ├─Token(NewLine) |\n\n|

var aliasedFoo = foo
//@[00:020) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:014) | ├─IdentifierSyntax
//@[04:014) | | └─Token(Identifier) |aliasedFoo|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:020) | └─VariableAccessSyntax
//@[17:020) |   └─IdentifierSyntax
//@[17:020) |     └─Token(Identifier) |foo|
//@[20:022) ├─Token(NewLine) |\n\n|

type fizzes = fizz[]
//@[00:020) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |fizzes|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:020) | └─ArrayTypeSyntax
//@[14:018) |   ├─ArrayTypeMemberSyntax
//@[14:018) |   | └─VariableAccessSyntax
//@[14:018) |   |   └─IdentifierSyntax
//@[14:018) |   |     └─Token(Identifier) |fizz|
//@[18:019) |   ├─Token(LeftSquare) |[|
//@[19:020) |   └─Token(RightSquare) |]|
//@[20:021) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
