type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[00:356) ProgramSyntax
//@[00:067) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:008) | ├─IdentifierSyntax
//@[05:008) | | └─Token(Identifier) |foo|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:067) | └─ParameterizedTypeInstantiationSyntax
//@[11:019) |   ├─IdentifierSyntax
//@[11:019) |   | └─Token(Identifier) |resource|
//@[19:020) |   ├─Token(LeftChevron) |<|
//@[20:066) |   ├─ParameterizedTypeArgumentSyntax
//@[20:066) |   | └─StringSyntax
//@[20:066) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[66:067) |   └─Token(RightChevron) |>|
//@[67:069) ├─Token(NewLine) |\n\n|

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@[00:160) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:009) | ├─IdentifierSyntax
//@[06:009) | | └─Token(Identifier) |bar|
//@[10:057) | ├─ParameterizedTypeInstantiationSyntax
//@[10:018) | | ├─IdentifierSyntax
//@[10:018) | | | └─Token(Identifier) |resource|
//@[18:019) | | ├─Token(LeftChevron) |<|
//@[19:056) | | ├─ParameterizedTypeArgumentSyntax
//@[19:056) | | | └─StringSyntax
//@[19:056) | | |   └─Token(StringComplete) |'Microsoft.Resources/tags@2022-09-01'|
//@[56:057) | | └─Token(RightChevron) |>|
//@[58:160) | └─ParameterDefaultValueSyntax
//@[58:059) |   ├─Token(Assignment) |=|
//@[60:160) |   └─ObjectSyntax
//@[60:061) |     ├─Token(LeftBrace) |{|
//@[61:062) |     ├─Token(NewLine) |\n|
  name: 'default'
//@[02:017) |     ├─ObjectPropertySyntax
//@[02:006) |     | ├─IdentifierSyntax
//@[02:006) |     | | └─Token(Identifier) |name|
//@[06:007) |     | ├─Token(Colon) |:|
//@[08:017) |     | └─StringSyntax
//@[08:017) |     |   └─Token(StringComplete) |'default'|
//@[17:018) |     ├─Token(NewLine) |\n|
  properties: {
//@[02:078) |     ├─ObjectPropertySyntax
//@[02:012) |     | ├─IdentifierSyntax
//@[02:012) |     | | └─Token(Identifier) |properties|
//@[12:013) |     | ├─Token(Colon) |:|
//@[14:078) |     | └─ObjectSyntax
//@[14:015) |     |   ├─Token(LeftBrace) |{|
//@[15:016) |     |   ├─Token(NewLine) |\n|
    tags: {
//@[04:058) |     |   ├─ObjectPropertySyntax
//@[04:008) |     |   | ├─IdentifierSyntax
//@[04:008) |     |   | | └─Token(Identifier) |tags|
//@[08:009) |     |   | ├─Token(Colon) |:|
//@[10:058) |     |   | └─ObjectSyntax
//@[10:011) |     |   |   ├─Token(LeftBrace) |{|
//@[11:012) |     |   |   ├─Token(NewLine) |\n|
      fizz: 'buzz'
//@[06:018) |     |   |   ├─ObjectPropertySyntax
//@[06:010) |     |   |   | ├─IdentifierSyntax
//@[06:010) |     |   |   | | └─Token(Identifier) |fizz|
//@[10:011) |     |   |   | ├─Token(Colon) |:|
//@[12:018) |     |   |   | └─StringSyntax
//@[12:018) |     |   |   |   └─Token(StringComplete) |'buzz'|
//@[18:019) |     |   |   ├─Token(NewLine) |\n|
      snap: 'crackle'
//@[06:021) |     |   |   ├─ObjectPropertySyntax
//@[06:010) |     |   |   | ├─IdentifierSyntax
//@[06:010) |     |   |   | | └─Token(Identifier) |snap|
//@[10:011) |     |   |   | ├─Token(Colon) |:|
//@[12:021) |     |   |   | └─StringSyntax
//@[12:021) |     |   |   |   └─Token(StringComplete) |'crackle'|
//@[21:022) |     |   |   ├─Token(NewLine) |\n|
    }
//@[04:005) |     |   |   └─Token(RightBrace) |}|
//@[05:006) |     |   ├─Token(NewLine) |\n|
  }
//@[02:003) |     |   └─Token(RightBrace) |}|
//@[03:004) |     ├─Token(NewLine) |\n|
}
//@[00:001) |     └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
//@[00:124) ├─OutputDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |output|
//@[07:010) | ├─IdentifierSyntax
//@[07:010) | | └─Token(Identifier) |baz|
//@[11:082) | ├─ParameterizedTypeInstantiationSyntax
//@[11:019) | | ├─IdentifierSyntax
//@[11:019) | | | └─Token(Identifier) |resource|
//@[19:020) | | ├─Token(LeftChevron) |<|
//@[20:081) | | ├─ParameterizedTypeArgumentSyntax
//@[20:081) | | | └─StringSyntax
//@[20:081) | | |   └─Token(StringComplete) |'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'|
//@[81:082) | | └─Token(RightChevron) |>|
//@[83:084) | ├─Token(Assignment) |=|
//@[85:124) | └─ObjectSyntax
//@[85:086) |   ├─Token(LeftBrace) |{|
//@[86:087) |   ├─Token(NewLine) |\n|
  name: 'myId'
//@[02:014) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:014) |   | └─StringSyntax
//@[08:014) |   |   └─Token(StringComplete) |'myId'|
//@[14:015) |   ├─Token(NewLine) |\n|
  location: 'eastus'
//@[02:020) |   ├─ObjectPropertySyntax
//@[02:010) |   | ├─IdentifierSyntax
//@[02:010) |   | | └─Token(Identifier) |location|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:020) |   | └─StringSyntax
//@[12:020) |   |   └─Token(StringComplete) |'eastus'|
//@[20:021) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
