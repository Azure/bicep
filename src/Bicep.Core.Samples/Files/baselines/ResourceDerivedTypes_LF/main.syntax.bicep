type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[00:974) ProgramSyntax
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

type test = {
//@[00:239) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:009) | ├─IdentifierSyntax
//@[05:009) | | └─Token(Identifier) |test|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:239) | └─ObjectTypeSyntax
//@[12:013) |   ├─Token(LeftBrace) |{|
//@[13:014) |   ├─Token(NewLine) |\n|
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[02:064) |   ├─ObjectTypePropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |resA|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:064) |   | └─ParameterizedTypeInstantiationSyntax
//@[08:016) |   |   ├─IdentifierSyntax
//@[08:016) |   |   | └─Token(Identifier) |resource|
//@[16:017) |   |   ├─Token(LeftChevron) |<|
//@[17:063) |   |   ├─ParameterizedTypeArgumentSyntax
//@[17:063) |   |   | └─StringSyntax
//@[17:063) |   |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[63:064) |   |   └─Token(RightChevron) |>|
//@[64:065) |   ├─Token(NewLine) |\n|
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[02:068) |   ├─ObjectTypePropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |resB|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:068) |   | └─InstanceParameterizedTypeInstantiationSyntax
//@[08:011) |   |   ├─VariableAccessSyntax
//@[08:011) |   |   | └─IdentifierSyntax
//@[08:011) |   |   |   └─Token(Identifier) |sys|
//@[11:012) |   |   ├─Token(Dot) |.|
//@[12:020) |   |   ├─IdentifierSyntax
//@[12:020) |   |   | └─Token(Identifier) |resource|
//@[20:021) |   |   ├─Token(LeftChevron) |<|
//@[21:067) |   |   ├─ParameterizedTypeArgumentSyntax
//@[21:067) |   |   | └─StringSyntax
//@[21:067) |   |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[67:068) |   |   └─Token(RightChevron) |>|
//@[68:069) |   ├─Token(NewLine) |\n|
  resC: sys.array
//@[02:017) |   ├─ObjectTypePropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |resC|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:017) |   | └─PropertyAccessSyntax
//@[08:011) |   |   ├─VariableAccessSyntax
//@[08:011) |   |   | └─IdentifierSyntax
//@[08:011) |   |   |   └─Token(Identifier) |sys|
//@[11:012) |   |   ├─Token(Dot) |.|
//@[12:017) |   |   └─IdentifierSyntax
//@[12:017) |   |     └─Token(Identifier) |array|
//@[17:018) |   ├─Token(NewLine) |\n|
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[02:071) |   ├─ObjectTypePropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |resD|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:071) |   | └─InstanceParameterizedTypeInstantiationSyntax
//@[08:011) |   |   ├─VariableAccessSyntax
//@[08:011) |   |   | └─IdentifierSyntax
//@[08:011) |   |   |   └─Token(Identifier) |sys|
//@[11:012) |   |   ├─Token(Dot) |.|
//@[12:020) |   |   ├─IdentifierSyntax
//@[12:020) |   |   | └─Token(Identifier) |resource|
//@[20:021) |   |   ├─Token(LeftChevron) |<|
//@[21:070) |   |   ├─ParameterizedTypeArgumentSyntax
//@[21:070) |   |   | └─StringSyntax
//@[21:070) |   |   |   └─Token(StringComplete) |'az:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[70:071) |   |   └─Token(RightChevron) |>|
//@[71:072) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

type strangeFormattings = {
//@[00:258) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:023) | ├─IdentifierSyntax
//@[05:023) | | └─Token(Identifier) |strangeFormattings|
//@[24:025) | ├─Token(Assignment) |=|
//@[26:258) | └─ObjectTypeSyntax
//@[26:027) |   ├─Token(LeftBrace) |{|
//@[27:028) |   ├─Token(NewLine) |\n|
  test: resource<
//@[02:075) |   ├─ObjectTypePropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |test|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:075) |   | └─ParameterizedTypeInstantiationSyntax
//@[08:016) |   |   ├─IdentifierSyntax
//@[08:016) |   |   | └─Token(Identifier) |resource|
//@[16:017) |   |   ├─Token(LeftChevron) |<|
//@[17:019) |   |   ├─Token(NewLine) |\n\n|

  'Astronomer.Astro/organizations@2023-08-01-preview'
//@[02:053) |   |   ├─ParameterizedTypeArgumentSyntax
//@[02:053) |   |   | └─StringSyntax
//@[02:053) |   |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[53:055) |   |   ├─Token(NewLine) |\n\n|

>
//@[00:001) |   |   └─Token(RightChevron) |>|
//@[01:002) |   ├─Token(NewLine) |\n|
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[02:069) |   ├─ObjectTypePropertySyntax
//@[02:007) |   | ├─IdentifierSyntax
//@[02:007) |   | | └─Token(Identifier) |test2|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:069) |   | └─ParameterizedTypeInstantiationSyntax
//@[09:017) |   |   ├─IdentifierSyntax
//@[09:017) |   |   | └─Token(Identifier) |resource|
//@[21:022) |   |   ├─Token(LeftChevron) |<|
//@[22:068) |   |   ├─ParameterizedTypeArgumentSyntax
//@[22:068) |   |   | └─StringSyntax
//@[22:068) |   |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[68:069) |   |   └─Token(RightChevron) |>|
//@[69:070) |   ├─Token(NewLine) |\n|
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>
//@[02:082) |   ├─ObjectTypePropertySyntax
//@[02:007) |   | ├─IdentifierSyntax
//@[02:007) |   | | └─Token(Identifier) |test3|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:082) |   | └─ParameterizedTypeInstantiationSyntax
//@[09:017) |   |   ├─IdentifierSyntax
//@[09:017) |   |   | └─Token(Identifier) |resource|
//@[17:018) |   |   ├─Token(LeftChevron) |<|
//@[26:072) |   |   ├─ParameterizedTypeArgumentSyntax
//@[26:072) |   |   | └─StringSyntax
//@[26:072) |   |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[81:082) |   |   └─Token(RightChevron) |>|
//@[82:083) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

@description('I love space(s)')
//@[00:115) ├─TypeDeclarationSyntax
//@[00:031) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:031) | | └─FunctionCallSyntax
//@[01:012) | |   ├─IdentifierSyntax
//@[01:012) | |   | └─Token(Identifier) |description|
//@[12:013) | |   ├─Token(LeftParen) |(|
//@[13:030) | |   ├─FunctionArgumentSyntax
//@[13:030) | |   | └─StringSyntax
//@[13:030) | |   |   └─Token(StringComplete) |'I love space(s)'|
//@[30:031) | |   └─Token(RightParen) |)|
//@[31:032) | ├─Token(NewLine) |\n|
type test2 = resource<
//@[00:004) | ├─Token(Identifier) |type|
//@[05:010) | ├─IdentifierSyntax
//@[05:010) | | └─Token(Identifier) |test2|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:083) | └─ParameterizedTypeInstantiationSyntax
//@[13:021) |   ├─IdentifierSyntax
//@[13:021) |   | └─Token(Identifier) |resource|
//@[21:022) |   ├─Token(LeftChevron) |<|
//@[22:024) |   ├─Token(NewLine) |\n\n|

     'Astronomer.Astro/organizations@2023-08-01-preview'
//@[05:056) |   ├─ParameterizedTypeArgumentSyntax
//@[05:056) |   | └─StringSyntax
//@[05:056) |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[56:058) |   ├─Token(NewLine) |\n\n|

>
//@[00:001) |   └─Token(RightChevron) |>|
//@[01:003) ├─Token(NewLine) |\n\n|

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
