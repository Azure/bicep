import {foo, fizz, pop, greet} from 'modules/mod.bicep'
//@[00:407) ProgramSyntax
//@[00:055) ├─CompileTimeImportDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |import|
//@[07:030) | ├─ImportedSymbolsListSyntax
//@[07:008) | | ├─Token(LeftBrace) |{|
//@[08:011) | | ├─ImportedSymbolsListItemSyntax
//@[08:011) | | | └─IdentifierSyntax
//@[08:011) | | |   └─Token(Identifier) |foo|
//@[11:012) | | ├─Token(Comma) |,|
//@[13:017) | | ├─ImportedSymbolsListItemSyntax
//@[13:017) | | | └─IdentifierSyntax
//@[13:017) | | |   └─Token(Identifier) |fizz|
//@[17:018) | | ├─Token(Comma) |,|
//@[19:022) | | ├─ImportedSymbolsListItemSyntax
//@[19:022) | | | └─IdentifierSyntax
//@[19:022) | | |   └─Token(Identifier) |pop|
//@[22:023) | | ├─Token(Comma) |,|
//@[24:029) | | ├─ImportedSymbolsListItemSyntax
//@[24:029) | | | └─IdentifierSyntax
//@[24:029) | | |   └─Token(Identifier) |greet|
//@[29:030) | | └─Token(RightBrace) |}|
//@[31:055) | └─CompileTimeImportFromClauseSyntax
//@[31:035) |   ├─Token(Identifier) |from|
//@[36:055) |   └─StringSyntax
//@[36:055) |     └─Token(StringComplete) |'modules/mod.bicep'|
//@[55:056) ├─Token(NewLine) |\n|
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
//@[20:021) ├─Token(NewLine) |\n|
var aliasedBar = mod2.foo
//@[00:025) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:014) | ├─IdentifierSyntax
//@[04:014) | | └─Token(Identifier) |aliasedBar|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:025) | └─PropertyAccessSyntax
//@[17:021) |   ├─VariableAccessSyntax
//@[17:021) |   | └─IdentifierSyntax
//@[17:021) |   |   └─Token(Identifier) |mod2|
//@[21:022) |   ├─Token(Dot) |.|
//@[22:025) |   └─IdentifierSyntax
//@[22:025) |     └─Token(Identifier) |foo|
//@[25:027) ├─Token(NewLine) |\n\n|

type fizzes = fizz[]
//@[00:020) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |fizzes|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:020) | └─ArrayTypeSyntax
//@[14:018) |   ├─ArrayTypeMemberSyntax
//@[14:018) |   | └─TypeVariableAccessSyntax
//@[14:018) |   |   └─IdentifierSyntax
//@[14:018) |   |     └─Token(Identifier) |fizz|
//@[18:019) |   ├─Token(LeftSquare) |[|
//@[19:020) |   └─Token(RightSquare) |]|
//@[20:022) ├─Token(NewLine) |\n\n|

param fizzParam mod2.fizz
//@[00:025) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:015) | ├─IdentifierSyntax
//@[06:015) | | └─Token(Identifier) |fizzParam|
//@[16:025) | └─TypePropertyAccessSyntax
//@[16:020) |   ├─TypeVariableAccessSyntax
//@[16:020) |   | └─IdentifierSyntax
//@[16:020) |   |   └─Token(Identifier) |mod2|
//@[20:021) |   ├─Token(Dot) |.|
//@[21:025) |   └─IdentifierSyntax
//@[21:025) |     └─Token(Identifier) |fizz|
//@[25:026) ├─Token(NewLine) |\n|
output magicWord pop = refersToCopyVariable[3].value
//@[00:052) ├─OutputDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |output|
//@[07:016) | ├─IdentifierSyntax
//@[07:016) | | └─Token(Identifier) |magicWord|
//@[17:020) | ├─TypeVariableAccessSyntax
//@[17:020) | | └─IdentifierSyntax
//@[17:020) | |   └─Token(Identifier) |pop|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:052) | └─PropertyAccessSyntax
//@[23:046) |   ├─ArrayAccessSyntax
//@[23:043) |   | ├─VariableAccessSyntax
//@[23:043) |   | | └─IdentifierSyntax
//@[23:043) |   | |   └─Token(Identifier) |refersToCopyVariable|
//@[43:044) |   | ├─Token(LeftSquare) |[|
//@[44:045) |   | ├─IntegerLiteralSyntax
//@[44:045) |   | | └─Token(Integer) |3|
//@[45:046) |   | └─Token(RightSquare) |]|
//@[46:047) |   ├─Token(Dot) |.|
//@[47:052) |   └─IdentifierSyntax
//@[47:052) |     └─Token(Identifier) |value|
//@[52:054) ├─Token(NewLine) |\n\n|

output greeting string = greet('friend')
//@[00:040) ├─OutputDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |output|
//@[07:015) | ├─IdentifierSyntax
//@[07:015) | | └─Token(Identifier) |greeting|
//@[16:022) | ├─TypeVariableAccessSyntax
//@[16:022) | | └─IdentifierSyntax
//@[16:022) | |   └─Token(Identifier) |string|
//@[23:024) | ├─Token(Assignment) |=|
//@[25:040) | └─FunctionCallSyntax
//@[25:030) |   ├─IdentifierSyntax
//@[25:030) |   | └─Token(Identifier) |greet|
//@[30:031) |   ├─Token(LeftParen) |(|
//@[31:039) |   ├─FunctionArgumentSyntax
//@[31:039) |   | └─StringSyntax
//@[31:039) |   |   └─Token(StringComplete) |'friend'|
//@[39:040) |   └─Token(RightParen) |)|
//@[40:041) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
