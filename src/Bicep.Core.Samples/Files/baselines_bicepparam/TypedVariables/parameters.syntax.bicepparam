using 'main.bicep'
//@[00:350) ProgramSyntax
//@[00:018) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:018) | └─StringSyntax
//@[06:018) |   └─Token(StringComplete) |'main.bicep'|
//@[18:020) ├─Token(NewLine) |\n\n|

import { FooType } from './types.bicep'
//@[00:039) ├─CompileTimeImportDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |import|
//@[07:018) | ├─ImportedSymbolsListSyntax
//@[07:008) | | ├─Token(LeftBrace) |{|
//@[09:016) | | ├─ImportedSymbolsListItemSyntax
//@[09:016) | | | └─IdentifierSyntax
//@[09:016) | | |   └─Token(Identifier) |FooType|
//@[17:018) | | └─Token(RightBrace) |}|
//@[19:039) | └─CompileTimeImportFromClauseSyntax
//@[19:023) |   ├─Token(Identifier) |from|
//@[24:039) |   └─StringSyntax
//@[24:039) |     └─Token(StringComplete) |'./types.bicep'|
//@[39:041) ├─Token(NewLine) |\n\n|

var imported FooType = {
//@[00:064) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:012) | ├─IdentifierSyntax
//@[04:012) | | └─Token(Identifier) |imported|
//@[13:020) | ├─TypeVariableAccessSyntax
//@[13:020) | | └─IdentifierSyntax
//@[13:020) | |   └─Token(Identifier) |FooType|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:064) | └─ObjectSyntax
//@[23:024) |   ├─Token(LeftBrace) |{|
//@[24:025) |   ├─Token(NewLine) |\n|
  stringProp: 'adfadf'
//@[02:022) |   ├─ObjectPropertySyntax
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |stringProp|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:022) |   | └─StringSyntax
//@[14:022) |   |   └─Token(StringComplete) |'adfadf'|
//@[22:023) |   ├─Token(NewLine) |\n|
  intProp: 123
//@[02:014) |   ├─ObjectPropertySyntax
//@[02:009) |   | ├─IdentifierSyntax
//@[02:009) |   | | └─Token(Identifier) |intProp|
//@[09:010) |   | ├─Token(Colon) |:|
//@[11:014) |   | └─IntegerLiteralSyntax
//@[11:014) |   |   └─Token(Integer) |123|
//@[14:015) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

var inline {
//@[00:095) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |inline|
//@[11:050) | ├─ObjectTypeSyntax
//@[11:012) | | ├─Token(LeftBrace) |{|
//@[12:013) | | ├─Token(NewLine) |\n|
  stringProp: string
//@[02:020) | | ├─ObjectTypePropertySyntax
//@[02:012) | | | ├─IdentifierSyntax
//@[02:012) | | | | └─Token(Identifier) |stringProp|
//@[12:013) | | | ├─Token(Colon) |:|
//@[14:020) | | | └─TypeVariableAccessSyntax
//@[14:020) | | |   └─IdentifierSyntax
//@[14:020) | | |     └─Token(Identifier) |string|
//@[20:021) | | ├─Token(NewLine) |\n|
  intProp: int
//@[02:014) | | ├─ObjectTypePropertySyntax
//@[02:009) | | | ├─IdentifierSyntax
//@[02:009) | | | | └─Token(Identifier) |intProp|
//@[09:010) | | | ├─Token(Colon) |:|
//@[11:014) | | | └─TypeVariableAccessSyntax
//@[11:014) | | |   └─IdentifierSyntax
//@[11:014) | | |     └─Token(Identifier) |int|
//@[14:015) | | ├─Token(NewLine) |\n|
} = {
//@[00:001) | | └─Token(RightBrace) |}|
//@[02:003) | ├─Token(Assignment) |=|
//@[04:046) | └─ObjectSyntax
//@[04:005) |   ├─Token(LeftBrace) |{|
//@[05:006) |   ├─Token(NewLine) |\n|
  stringProp: 'asdaosd'
//@[02:023) |   ├─ObjectPropertySyntax
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |stringProp|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:023) |   | └─StringSyntax
//@[14:023) |   |   └─Token(StringComplete) |'asdaosd'|
//@[23:024) |   ├─Token(NewLine) |\n|
  intProp: 123
//@[02:014) |   ├─ObjectPropertySyntax
//@[02:009) |   | ├─IdentifierSyntax
//@[02:009) |   | | └─Token(Identifier) |intProp|
//@[09:010) |   | ├─Token(Colon) |:|
//@[11:014) |   | └─IntegerLiteralSyntax
//@[11:014) |   |   └─Token(Integer) |123|
//@[14:015) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

type InFileType = {
//@[00:057) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:015) | ├─IdentifierSyntax
//@[05:015) | | └─Token(Identifier) |InFileType|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:057) | └─ObjectTypeSyntax
//@[18:019) |   ├─Token(LeftBrace) |{|
//@[19:020) |   ├─Token(NewLine) |\n|
  stringProp: string
//@[02:020) |   ├─ObjectTypePropertySyntax
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |stringProp|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:020) |   | └─TypeVariableAccessSyntax
//@[14:020) |   |   └─IdentifierSyntax
//@[14:020) |   |     └─Token(Identifier) |string|
//@[20:021) |   ├─Token(NewLine) |\n|
  intProp: int
//@[02:014) |   ├─ObjectTypePropertySyntax
//@[02:009) |   | ├─IdentifierSyntax
//@[02:009) |   | | └─Token(Identifier) |intProp|
//@[09:010) |   | ├─Token(Colon) |:|
//@[11:014) |   | └─TypeVariableAccessSyntax
//@[11:014) |   |   └─IdentifierSyntax
//@[11:014) |   |     └─Token(Identifier) |int|
//@[14:015) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

var inFile InFileType = {
//@[00:066) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |inFile|
//@[11:021) | ├─TypeVariableAccessSyntax
//@[11:021) | | └─IdentifierSyntax
//@[11:021) | |   └─Token(Identifier) |InFileType|
//@[22:023) | ├─Token(Assignment) |=|
//@[24:066) | └─ObjectSyntax
//@[24:025) |   ├─Token(LeftBrace) |{|
//@[25:026) |   ├─Token(NewLine) |\n|
  stringProp: 'asdaosd'
//@[02:023) |   ├─ObjectPropertySyntax
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |stringProp|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:023) |   | └─StringSyntax
//@[14:023) |   |   └─Token(StringComplete) |'asdaosd'|
//@[23:024) |   ├─Token(NewLine) |\n|
  intProp: 123
//@[02:014) |   ├─ObjectPropertySyntax
//@[02:009) |   | ├─IdentifierSyntax
//@[02:009) |   | | └─Token(Identifier) |intProp|
//@[09:010) |   | ├─Token(Colon) |:|
//@[11:014) |   | └─IntegerLiteralSyntax
//@[11:014) |   |   └─Token(Integer) |123|
//@[14:015) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
