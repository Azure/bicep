type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[00:1215) ProgramSyntax
//@[00:0072) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0008) | ├─IdentifierSyntax
//@[05:0008) | | └─Token(Identifier) |foo|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0072) | └─TypePropertyAccessSyntax
//@[11:0067) |   ├─ParameterizedTypeInstantiationSyntax
//@[11:0019) |   | ├─IdentifierSyntax
//@[11:0019) |   | | └─Token(Identifier) |resource|
//@[19:0020) |   | ├─Token(LeftChevron) |<|
//@[20:0066) |   | ├─ParameterizedTypeArgumentSyntax
//@[20:0066) |   | | └─StringTypeLiteralSyntax
//@[20:0066) |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[66:0067) |   | └─Token(RightChevron) |>|
//@[67:0068) |   ├─Token(Dot) |.|
//@[68:0072) |   └─IdentifierSyntax
//@[68:0072) |     └─Token(Identifier) |name|
//@[72:0074) ├─Token(NewLine) |\n\n|

type test = {
//@[00:0254) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0009) | ├─IdentifierSyntax
//@[05:0009) | | └─Token(Identifier) |test|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0254) | └─ObjectTypeSyntax
//@[12:0013) |   ├─Token(LeftBrace) |{|
//@[13:0014) |   ├─Token(NewLine) |\n|
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[02:0069) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |resA|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0069) |   | └─TypePropertyAccessSyntax
//@[08:0064) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[08:0016) |   |   | ├─IdentifierSyntax
//@[08:0016) |   |   | | └─Token(Identifier) |resource|
//@[16:0017) |   |   | ├─Token(LeftChevron) |<|
//@[17:0063) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[17:0063) |   |   | | └─StringTypeLiteralSyntax
//@[17:0063) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[63:0064) |   |   | └─Token(RightChevron) |>|
//@[64:0065) |   |   ├─Token(Dot) |.|
//@[65:0069) |   |   └─IdentifierSyntax
//@[65:0069) |   |     └─Token(Identifier) |name|
//@[69:0070) |   ├─Token(NewLine) |\n|
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[02:0073) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |resB|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0073) |   | └─TypePropertyAccessSyntax
//@[08:0068) |   |   ├─InstanceParameterizedTypeInstantiationSyntax
//@[08:0011) |   |   | ├─TypeVariableAccessSyntax
//@[08:0011) |   |   | | └─IdentifierSyntax
//@[08:0011) |   |   | |   └─Token(Identifier) |sys|
//@[11:0012) |   |   | ├─Token(Dot) |.|
//@[12:0020) |   |   | ├─IdentifierSyntax
//@[12:0020) |   |   | | └─Token(Identifier) |resource|
//@[20:0021) |   |   | ├─Token(LeftChevron) |<|
//@[21:0067) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[21:0067) |   |   | | └─StringTypeLiteralSyntax
//@[21:0067) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[67:0068) |   |   | └─Token(RightChevron) |>|
//@[68:0069) |   |   ├─Token(Dot) |.|
//@[69:0073) |   |   └─IdentifierSyntax
//@[69:0073) |   |     └─Token(Identifier) |name|
//@[73:0074) |   ├─Token(NewLine) |\n|
  resC: sys.array
//@[02:0017) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |resC|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0017) |   | └─TypePropertyAccessSyntax
//@[08:0011) |   |   ├─TypeVariableAccessSyntax
//@[08:0011) |   |   | └─IdentifierSyntax
//@[08:0011) |   |   |   └─Token(Identifier) |sys|
//@[11:0012) |   |   ├─Token(Dot) |.|
//@[12:0017) |   |   └─IdentifierSyntax
//@[12:0017) |   |     └─Token(Identifier) |array|
//@[17:0018) |   ├─Token(NewLine) |\n|
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[02:0076) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |resD|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0076) |   | └─TypePropertyAccessSyntax
//@[08:0071) |   |   ├─InstanceParameterizedTypeInstantiationSyntax
//@[08:0011) |   |   | ├─TypeVariableAccessSyntax
//@[08:0011) |   |   | | └─IdentifierSyntax
//@[08:0011) |   |   | |   └─Token(Identifier) |sys|
//@[11:0012) |   |   | ├─Token(Dot) |.|
//@[12:0020) |   |   | ├─IdentifierSyntax
//@[12:0020) |   |   | | └─Token(Identifier) |resource|
//@[20:0021) |   |   | ├─Token(LeftChevron) |<|
//@[21:0070) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[21:0070) |   |   | | └─StringTypeLiteralSyntax
//@[21:0070) |   |   | |   └─Token(StringComplete) |'az:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[70:0071) |   |   | └─Token(RightChevron) |>|
//@[71:0072) |   |   ├─Token(Dot) |.|
//@[72:0076) |   |   └─IdentifierSyntax
//@[72:0076) |   |     └─Token(Identifier) |name|
//@[76:0077) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type strangeFormattings = {
//@[00:0273) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0023) | ├─IdentifierSyntax
//@[05:0023) | | └─Token(Identifier) |strangeFormattings|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0273) | └─ObjectTypeSyntax
//@[26:0027) |   ├─Token(LeftBrace) |{|
//@[27:0028) |   ├─Token(NewLine) |\n|
  test: resource<
//@[02:0080) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |test|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0080) |   | └─TypePropertyAccessSyntax
//@[08:0075) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[08:0016) |   |   | ├─IdentifierSyntax
//@[08:0016) |   |   | | └─Token(Identifier) |resource|
//@[16:0017) |   |   | ├─Token(LeftChevron) |<|
//@[17:0019) |   |   | ├─Token(NewLine) |\n\n|

  'Astronomer.Astro/organizations@2023-08-01-preview'
//@[02:0053) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[02:0053) |   |   | | └─StringTypeLiteralSyntax
//@[02:0053) |   |   | |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[53:0055) |   |   | ├─Token(NewLine) |\n\n|

>.name
//@[00:0001) |   |   | └─Token(RightChevron) |>|
//@[01:0002) |   |   ├─Token(Dot) |.|
//@[02:0006) |   |   └─IdentifierSyntax
//@[02:0006) |   |     └─Token(Identifier) |name|
//@[06:0007) |   ├─Token(NewLine) |\n|
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[02:0074) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |test2|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0074) |   | └─TypePropertyAccessSyntax
//@[09:0069) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[09:0017) |   |   | ├─IdentifierSyntax
//@[09:0017) |   |   | | └─Token(Identifier) |resource|
//@[21:0022) |   |   | ├─Token(LeftChevron) |<|
//@[22:0068) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[22:0068) |   |   | | └─StringTypeLiteralSyntax
//@[22:0068) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[68:0069) |   |   | └─Token(RightChevron) |>|
//@[69:0070) |   |   ├─Token(Dot) |.|
//@[70:0074) |   |   └─IdentifierSyntax
//@[70:0074) |   |     └─Token(Identifier) |name|
//@[74:0075) |   ├─Token(NewLine) |\n|
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
//@[02:0087) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |test3|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0087) |   | └─TypePropertyAccessSyntax
//@[09:0082) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[09:0017) |   |   | ├─IdentifierSyntax
//@[09:0017) |   |   | | └─Token(Identifier) |resource|
//@[17:0018) |   |   | ├─Token(LeftChevron) |<|
//@[26:0072) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[26:0072) |   |   | | └─StringTypeLiteralSyntax
//@[26:0072) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[81:0082) |   |   | └─Token(RightChevron) |>|
//@[82:0083) |   |   ├─Token(Dot) |.|
//@[83:0087) |   |   └─IdentifierSyntax
//@[83:0087) |   |     └─Token(Identifier) |name|
//@[87:0088) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

@description('I love space(s)')
//@[00:0120) ├─TypeDeclarationSyntax
//@[00:0031) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0031) | | └─FunctionCallSyntax
//@[01:0012) | |   ├─IdentifierSyntax
//@[01:0012) | |   | └─Token(Identifier) |description|
//@[12:0013) | |   ├─Token(LeftParen) |(|
//@[13:0030) | |   ├─FunctionArgumentSyntax
//@[13:0030) | |   | └─StringSyntax
//@[13:0030) | |   |   └─Token(StringComplete) |'I love space(s)'|
//@[30:0031) | |   └─Token(RightParen) |)|
//@[31:0032) | ├─Token(NewLine) |\n|
type test2 = resource<
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |test2|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0088) | └─TypePropertyAccessSyntax
//@[13:0083) |   ├─ParameterizedTypeInstantiationSyntax
//@[13:0021) |   | ├─IdentifierSyntax
//@[13:0021) |   | | └─Token(Identifier) |resource|
//@[21:0022) |   | ├─Token(LeftChevron) |<|
//@[22:0024) |   | ├─Token(NewLine) |\n\n|

     'Astronomer.Astro/organizations@2023-08-01-preview'
//@[05:0056) |   | ├─ParameterizedTypeArgumentSyntax
//@[05:0056) |   | | └─StringTypeLiteralSyntax
//@[05:0056) |   | |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[56:0058) |   | ├─Token(NewLine) |\n\n|

>.name
//@[00:0001) |   | └─Token(RightChevron) |>|
//@[01:0002) |   ├─Token(Dot) |.|
//@[02:0006) |   └─IdentifierSyntax
//@[02:0006) |     └─Token(Identifier) |name|
//@[06:0008) ├─Token(NewLine) |\n\n|

param bar resource<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@[00:0125) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |bar|
//@[10:0068) | ├─TypePropertyAccessSyntax
//@[10:0057) | | ├─ParameterizedTypeInstantiationSyntax
//@[10:0018) | | | ├─IdentifierSyntax
//@[10:0018) | | | | └─Token(Identifier) |resource|
//@[18:0019) | | | ├─Token(LeftChevron) |<|
//@[19:0056) | | | ├─ParameterizedTypeArgumentSyntax
//@[19:0056) | | | | └─StringTypeLiteralSyntax
//@[19:0056) | | | |   └─Token(StringComplete) |'Microsoft.Resources/tags@2022-09-01'|
//@[56:0057) | | | └─Token(RightChevron) |>|
//@[57:0058) | | ├─Token(Dot) |.|
//@[58:0068) | | └─IdentifierSyntax
//@[58:0068) | |   └─Token(Identifier) |properties|
//@[69:0125) | └─ParameterDefaultValueSyntax
//@[69:0070) |   ├─Token(Assignment) |=|
//@[71:0125) |   └─ObjectSyntax
//@[71:0072) |     ├─Token(LeftBrace) |{|
//@[72:0073) |     ├─Token(NewLine) |\n|
  tags: {
//@[02:0050) |     ├─ObjectPropertySyntax
//@[02:0006) |     | ├─IdentifierSyntax
//@[02:0006) |     | | └─Token(Identifier) |tags|
//@[06:0007) |     | ├─Token(Colon) |:|
//@[08:0050) |     | └─ObjectSyntax
//@[08:0009) |     |   ├─Token(LeftBrace) |{|
//@[09:0010) |     |   ├─Token(NewLine) |\n|
    fizz: 'buzz'
//@[04:0016) |     |   ├─ObjectPropertySyntax
//@[04:0008) |     |   | ├─IdentifierSyntax
//@[04:0008) |     |   | | └─Token(Identifier) |fizz|
//@[08:0009) |     |   | ├─Token(Colon) |:|
//@[10:0016) |     |   | └─StringSyntax
//@[10:0016) |     |   |   └─Token(StringComplete) |'buzz'|
//@[16:0017) |     |   ├─Token(NewLine) |\n|
    snap: 'crackle'
//@[04:0019) |     |   ├─ObjectPropertySyntax
//@[04:0008) |     |   | ├─IdentifierSyntax
//@[04:0008) |     |   | | └─Token(Identifier) |snap|
//@[08:0009) |     |   | ├─Token(Colon) |:|
//@[10:0019) |     |   | └─StringSyntax
//@[10:0019) |     |   |   └─Token(StringComplete) |'crackle'|
//@[19:0020) |     |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |     |   └─Token(RightBrace) |}|
//@[03:0004) |     ├─Token(NewLine) |\n|
}
//@[00:0001) |     └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@[00:0096) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0010) | ├─IdentifierSyntax
//@[07:0010) | | └─Token(Identifier) |baz|
//@[11:0087) | ├─TypePropertyAccessSyntax
//@[11:0082) | | ├─ParameterizedTypeInstantiationSyntax
//@[11:0019) | | | ├─IdentifierSyntax
//@[11:0019) | | | | └─Token(Identifier) |resource|
//@[19:0020) | | | ├─Token(LeftChevron) |<|
//@[20:0081) | | | ├─ParameterizedTypeArgumentSyntax
//@[20:0081) | | | | └─StringTypeLiteralSyntax
//@[20:0081) | | | |   └─Token(StringComplete) |'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'|
//@[81:0082) | | | └─Token(RightChevron) |>|
//@[82:0083) | | ├─Token(Dot) |.|
//@[83:0087) | | └─IdentifierSyntax
//@[83:0087) | |   └─Token(Identifier) |name|
//@[88:0089) | ├─Token(Assignment) |=|
//@[90:0096) | └─StringSyntax
//@[90:0096) |   └─Token(StringComplete) |'myId'|
//@[96:0098) ├─Token(NewLine) |\n\n|

type storageAccountName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[00:0087) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0023) | ├─IdentifierSyntax
//@[05:0023) | | └─Token(Identifier) |storageAccountName|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0087) | └─TypePropertyAccessSyntax
//@[26:0082) |   ├─ParameterizedTypeInstantiationSyntax
//@[26:0034) |   | ├─IdentifierSyntax
//@[26:0034) |   | | └─Token(Identifier) |resource|
//@[34:0035) |   | ├─Token(LeftChevron) |<|
//@[35:0081) |   | ├─ParameterizedTypeArgumentSyntax
//@[35:0081) |   | | └─StringTypeLiteralSyntax
//@[35:0081) |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[81:0082) |   | └─Token(RightChevron) |>|
//@[82:0083) |   ├─Token(Dot) |.|
//@[83:0087) |   └─IdentifierSyntax
//@[83:0087) |     └─Token(Identifier) |name|
//@[87:0088) ├─Token(NewLine) |\n|
type accessPolicy = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[00:0097) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0017) | ├─IdentifierSyntax
//@[05:0017) | | └─Token(Identifier) |accessPolicy|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0097) | └─TypeItemsAccessSyntax
//@[20:0094) |   ├─TypePropertyAccessSyntax
//@[20:0079) |   | ├─TypePropertyAccessSyntax
//@[20:0068) |   | | ├─ParameterizedTypeInstantiationSyntax
//@[20:0028) |   | | | ├─IdentifierSyntax
//@[20:0028) |   | | | | └─Token(Identifier) |resource|
//@[28:0029) |   | | | ├─Token(LeftChevron) |<|
//@[29:0067) |   | | | ├─ParameterizedTypeArgumentSyntax
//@[29:0067) |   | | | | └─StringTypeLiteralSyntax
//@[29:0067) |   | | | |   └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[67:0068) |   | | | └─Token(RightChevron) |>|
//@[68:0069) |   | | ├─Token(Dot) |.|
//@[69:0079) |   | | └─IdentifierSyntax
//@[69:0079) |   | |   └─Token(Identifier) |properties|
//@[79:0080) |   | ├─Token(Dot) |.|
//@[80:0094) |   | └─IdentifierSyntax
//@[80:0094) |   |   └─Token(Identifier) |accessPolicies|
//@[94:0095) |   ├─Token(LeftSquare) |[|
//@[95:0096) |   ├─Token(Asterisk) |*|
//@[96:0097) |   └─Token(RightSquare) |]|
//@[97:0098) ├─Token(NewLine) |\n|
type tag = resource<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[00:0076) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0008) | ├─IdentifierSyntax
//@[05:0008) | | └─Token(Identifier) |tag|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0076) | └─TypeAdditionalPropertiesAccessSyntax
//@[11:0074) |   ├─TypePropertyAccessSyntax
//@[11:0069) |   | ├─TypePropertyAccessSyntax
//@[11:0058) |   | | ├─ParameterizedTypeInstantiationSyntax
//@[11:0019) |   | | | ├─IdentifierSyntax
//@[11:0019) |   | | | | └─Token(Identifier) |resource|
//@[19:0020) |   | | | ├─Token(LeftChevron) |<|
//@[20:0057) |   | | | ├─ParameterizedTypeArgumentSyntax
//@[20:0057) |   | | | | └─StringTypeLiteralSyntax
//@[20:0057) |   | | | |   └─Token(StringComplete) |'Microsoft.Resources/tags@2022-09-01'|
//@[57:0058) |   | | | └─Token(RightChevron) |>|
//@[58:0059) |   | | ├─Token(Dot) |.|
//@[59:0069) |   | | └─IdentifierSyntax
//@[59:0069) |   | |   └─Token(Identifier) |properties|
//@[69:0070) |   | ├─Token(Dot) |.|
//@[70:0074) |   | └─IdentifierSyntax
//@[70:0074) |   |   └─Token(Identifier) |tags|
//@[74:0075) |   ├─Token(Dot) |.|
//@[75:0076) |   └─Token(Asterisk) |*|
//@[76:0077) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
