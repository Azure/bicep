type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[00:1238) ProgramSyntax
//@[00:0067) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0008) | ├─IdentifierSyntax
//@[05:0008) | | └─Token(Identifier) |foo|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0067) | └─ParameterizedTypeInstantiationSyntax
//@[11:0019) |   ├─IdentifierSyntax
//@[11:0019) |   | └─Token(Identifier) |resource|
//@[19:0020) |   ├─Token(LeftChevron) |<|
//@[20:0066) |   ├─ParameterizedTypeArgumentSyntax
//@[20:0066) |   | └─StringSyntax
//@[20:0066) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[66:0067) |   └─Token(RightChevron) |>|
//@[67:0069) ├─Token(NewLine) |\n\n|

type test = {
//@[00:0239) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0009) | ├─IdentifierSyntax
//@[05:0009) | | └─Token(Identifier) |test|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0239) | └─ObjectTypeSyntax
//@[12:0013) |   ├─Token(LeftBrace) |{|
//@[13:0014) |   ├─Token(NewLine) |\n|
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[02:0064) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |resA|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0064) |   | └─ParameterizedTypeInstantiationSyntax
//@[08:0016) |   |   ├─IdentifierSyntax
//@[08:0016) |   |   | └─Token(Identifier) |resource|
//@[16:0017) |   |   ├─Token(LeftChevron) |<|
//@[17:0063) |   |   ├─ParameterizedTypeArgumentSyntax
//@[17:0063) |   |   | └─StringSyntax
//@[17:0063) |   |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[63:0064) |   |   └─Token(RightChevron) |>|
//@[64:0065) |   ├─Token(NewLine) |\n|
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[02:0068) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |resB|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0068) |   | └─InstanceParameterizedTypeInstantiationSyntax
//@[08:0011) |   |   ├─VariableAccessSyntax
//@[08:0011) |   |   | └─IdentifierSyntax
//@[08:0011) |   |   |   └─Token(Identifier) |sys|
//@[11:0012) |   |   ├─Token(Dot) |.|
//@[12:0020) |   |   ├─IdentifierSyntax
//@[12:0020) |   |   | └─Token(Identifier) |resource|
//@[20:0021) |   |   ├─Token(LeftChevron) |<|
//@[21:0067) |   |   ├─ParameterizedTypeArgumentSyntax
//@[21:0067) |   |   | └─StringSyntax
//@[21:0067) |   |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[67:0068) |   |   └─Token(RightChevron) |>|
//@[68:0069) |   ├─Token(NewLine) |\n|
  resC: sys.array
//@[02:0017) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |resC|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0017) |   | └─TypePropertyAccessSyntax
//@[08:0011) |   |   ├─VariableAccessSyntax
//@[08:0011) |   |   | └─IdentifierSyntax
//@[08:0011) |   |   |   └─Token(Identifier) |sys|
//@[11:0012) |   |   ├─Token(Dot) |.|
//@[12:0017) |   |   └─IdentifierSyntax
//@[12:0017) |   |     └─Token(Identifier) |array|
//@[17:0018) |   ├─Token(NewLine) |\n|
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[02:0071) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |resD|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0071) |   | └─InstanceParameterizedTypeInstantiationSyntax
//@[08:0011) |   |   ├─VariableAccessSyntax
//@[08:0011) |   |   | └─IdentifierSyntax
//@[08:0011) |   |   |   └─Token(Identifier) |sys|
//@[11:0012) |   |   ├─Token(Dot) |.|
//@[12:0020) |   |   ├─IdentifierSyntax
//@[12:0020) |   |   | └─Token(Identifier) |resource|
//@[20:0021) |   |   ├─Token(LeftChevron) |<|
//@[21:0070) |   |   ├─ParameterizedTypeArgumentSyntax
//@[21:0070) |   |   | └─StringSyntax
//@[21:0070) |   |   |   └─Token(StringComplete) |'az:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[70:0071) |   |   └─Token(RightChevron) |>|
//@[71:0072) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

type strangeFormattings = {
//@[00:0258) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0023) | ├─IdentifierSyntax
//@[05:0023) | | └─Token(Identifier) |strangeFormattings|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0258) | └─ObjectTypeSyntax
//@[26:0027) |   ├─Token(LeftBrace) |{|
//@[27:0028) |   ├─Token(NewLine) |\n|
  test: resource<
//@[02:0075) |   ├─ObjectTypePropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |test|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0075) |   | └─ParameterizedTypeInstantiationSyntax
//@[08:0016) |   |   ├─IdentifierSyntax
//@[08:0016) |   |   | └─Token(Identifier) |resource|
//@[16:0017) |   |   ├─Token(LeftChevron) |<|
//@[17:0019) |   |   ├─Token(NewLine) |\n\n|

  'Astronomer.Astro/organizations@2023-08-01-preview'
//@[02:0053) |   |   ├─ParameterizedTypeArgumentSyntax
//@[02:0053) |   |   | └─StringSyntax
//@[02:0053) |   |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[53:0055) |   |   ├─Token(NewLine) |\n\n|

>
//@[00:0001) |   |   └─Token(RightChevron) |>|
//@[01:0002) |   ├─Token(NewLine) |\n|
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[02:0069) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |test2|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0069) |   | └─ParameterizedTypeInstantiationSyntax
//@[09:0017) |   |   ├─IdentifierSyntax
//@[09:0017) |   |   | └─Token(Identifier) |resource|
//@[21:0022) |   |   ├─Token(LeftChevron) |<|
//@[22:0068) |   |   ├─ParameterizedTypeArgumentSyntax
//@[22:0068) |   |   | └─StringSyntax
//@[22:0068) |   |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[68:0069) |   |   └─Token(RightChevron) |>|
//@[69:0070) |   ├─Token(NewLine) |\n|
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>
//@[02:0082) |   ├─ObjectTypePropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |test3|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0082) |   | └─ParameterizedTypeInstantiationSyntax
//@[09:0017) |   |   ├─IdentifierSyntax
//@[09:0017) |   |   | └─Token(Identifier) |resource|
//@[17:0018) |   |   ├─Token(LeftChevron) |<|
//@[26:0072) |   |   ├─ParameterizedTypeArgumentSyntax
//@[26:0072) |   |   | └─StringSyntax
//@[26:0072) |   |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[81:0082) |   |   └─Token(RightChevron) |>|
//@[82:0083) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

@description('I love space(s)')
//@[00:0115) ├─TypeDeclarationSyntax
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
//@[13:0083) | └─ParameterizedTypeInstantiationSyntax
//@[13:0021) |   ├─IdentifierSyntax
//@[13:0021) |   | └─Token(Identifier) |resource|
//@[21:0022) |   ├─Token(LeftChevron) |<|
//@[22:0024) |   ├─Token(NewLine) |\n\n|

     'Astronomer.Astro/organizations@2023-08-01-preview'
//@[05:0056) |   ├─ParameterizedTypeArgumentSyntax
//@[05:0056) |   | └─StringSyntax
//@[05:0056) |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[56:0058) |   ├─Token(NewLine) |\n\n|

>
//@[00:0001) |   └─Token(RightChevron) |>|
//@[01:0003) ├─Token(NewLine) |\n\n|

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@[00:0160) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |bar|
//@[10:0057) | ├─ParameterizedTypeInstantiationSyntax
//@[10:0018) | | ├─IdentifierSyntax
//@[10:0018) | | | └─Token(Identifier) |resource|
//@[18:0019) | | ├─Token(LeftChevron) |<|
//@[19:0056) | | ├─ParameterizedTypeArgumentSyntax
//@[19:0056) | | | └─StringSyntax
//@[19:0056) | | |   └─Token(StringComplete) |'Microsoft.Resources/tags@2022-09-01'|
//@[56:0057) | | └─Token(RightChevron) |>|
//@[58:0160) | └─ParameterDefaultValueSyntax
//@[58:0059) |   ├─Token(Assignment) |=|
//@[60:0160) |   └─ObjectSyntax
//@[60:0061) |     ├─Token(LeftBrace) |{|
//@[61:0062) |     ├─Token(NewLine) |\n|
  name: 'default'
//@[02:0017) |     ├─ObjectPropertySyntax
//@[02:0006) |     | ├─IdentifierSyntax
//@[02:0006) |     | | └─Token(Identifier) |name|
//@[06:0007) |     | ├─Token(Colon) |:|
//@[08:0017) |     | └─StringSyntax
//@[08:0017) |     |   └─Token(StringComplete) |'default'|
//@[17:0018) |     ├─Token(NewLine) |\n|
  properties: {
//@[02:0078) |     ├─ObjectPropertySyntax
//@[02:0012) |     | ├─IdentifierSyntax
//@[02:0012) |     | | └─Token(Identifier) |properties|
//@[12:0013) |     | ├─Token(Colon) |:|
//@[14:0078) |     | └─ObjectSyntax
//@[14:0015) |     |   ├─Token(LeftBrace) |{|
//@[15:0016) |     |   ├─Token(NewLine) |\n|
    tags: {
//@[04:0058) |     |   ├─ObjectPropertySyntax
//@[04:0008) |     |   | ├─IdentifierSyntax
//@[04:0008) |     |   | | └─Token(Identifier) |tags|
//@[08:0009) |     |   | ├─Token(Colon) |:|
//@[10:0058) |     |   | └─ObjectSyntax
//@[10:0011) |     |   |   ├─Token(LeftBrace) |{|
//@[11:0012) |     |   |   ├─Token(NewLine) |\n|
      fizz: 'buzz'
//@[06:0018) |     |   |   ├─ObjectPropertySyntax
//@[06:0010) |     |   |   | ├─IdentifierSyntax
//@[06:0010) |     |   |   | | └─Token(Identifier) |fizz|
//@[10:0011) |     |   |   | ├─Token(Colon) |:|
//@[12:0018) |     |   |   | └─StringSyntax
//@[12:0018) |     |   |   |   └─Token(StringComplete) |'buzz'|
//@[18:0019) |     |   |   ├─Token(NewLine) |\n|
      snap: 'crackle'
//@[06:0021) |     |   |   ├─ObjectPropertySyntax
//@[06:0010) |     |   |   | ├─IdentifierSyntax
//@[06:0010) |     |   |   | | └─Token(Identifier) |snap|
//@[10:0011) |     |   |   | ├─Token(Colon) |:|
//@[12:0021) |     |   |   | └─StringSyntax
//@[12:0021) |     |   |   |   └─Token(StringComplete) |'crackle'|
//@[21:0022) |     |   |   ├─Token(NewLine) |\n|
    }
//@[04:0005) |     |   |   └─Token(RightBrace) |}|
//@[05:0006) |     |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |     |   └─Token(RightBrace) |}|
//@[03:0004) |     ├─Token(NewLine) |\n|
}
//@[00:0001) |     └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
//@[00:0124) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0010) | ├─IdentifierSyntax
//@[07:0010) | | └─Token(Identifier) |baz|
//@[11:0082) | ├─ParameterizedTypeInstantiationSyntax
//@[11:0019) | | ├─IdentifierSyntax
//@[11:0019) | | | └─Token(Identifier) |resource|
//@[19:0020) | | ├─Token(LeftChevron) |<|
//@[20:0081) | | ├─ParameterizedTypeArgumentSyntax
//@[20:0081) | | | └─StringSyntax
//@[20:0081) | | |   └─Token(StringComplete) |'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'|
//@[81:0082) | | └─Token(RightChevron) |>|
//@[83:0084) | ├─Token(Assignment) |=|
//@[85:0124) | └─ObjectSyntax
//@[85:0086) |   ├─Token(LeftBrace) |{|
//@[86:0087) |   ├─Token(NewLine) |\n|
  name: 'myId'
//@[02:0014) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0014) |   | └─StringSyntax
//@[08:0014) |   |   └─Token(StringComplete) |'myId'|
//@[14:0015) |   ├─Token(NewLine) |\n|
  location: 'eastus'
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |location|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0020) |   | └─StringSyntax
//@[12:0020) |   |   └─Token(StringComplete) |'eastus'|
//@[20:0021) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

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
//@[35:0081) |   | | └─StringSyntax
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
//@[29:0067) |   | | | | └─StringSyntax
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
//@[20:0057) |   | | | | └─StringSyntax
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
