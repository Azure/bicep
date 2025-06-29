type foo = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[000:1279) ProgramSyntax
//@[000:0077) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0008) | ├─IdentifierSyntax
//@[005:0008) | | └─Token(Identifier) |foo|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0077) | └─TypePropertyAccessSyntax
//@[011:0072) |   ├─ParameterizedTypeInstantiationSyntax
//@[011:0024) |   | ├─IdentifierSyntax
//@[011:0024) |   | | └─Token(Identifier) |resourceInput|
//@[024:0025) |   | ├─Token(LeftChevron) |<|
//@[025:0071) |   | ├─ParameterizedTypeArgumentSyntax
//@[025:0071) |   | | └─StringTypeLiteralSyntax
//@[025:0071) |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[071:0072) |   | └─Token(RightChevron) |>|
//@[072:0073) |   ├─Token(Dot) |.|
//@[073:0077) |   └─IdentifierSyntax
//@[073:0077) |     └─Token(Identifier) |name|
//@[077:0079) ├─Token(NewLine) |\n\n|

type test = {
//@[000:0269) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0009) | ├─IdentifierSyntax
//@[005:0009) | | └─Token(Identifier) |test|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0269) | └─ObjectTypeSyntax
//@[012:0013) |   ├─Token(LeftBrace) |{|
//@[013:0014) |   ├─Token(NewLine) |\n|
  resA: resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[002:0074) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |resA|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0074) |   | └─TypePropertyAccessSyntax
//@[008:0069) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[008:0021) |   |   | ├─IdentifierSyntax
//@[008:0021) |   |   | | └─Token(Identifier) |resourceInput|
//@[021:0022) |   |   | ├─Token(LeftChevron) |<|
//@[022:0068) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[022:0068) |   |   | | └─StringTypeLiteralSyntax
//@[022:0068) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[068:0069) |   |   | └─Token(RightChevron) |>|
//@[069:0070) |   |   ├─Token(Dot) |.|
//@[070:0074) |   |   └─IdentifierSyntax
//@[070:0074) |   |     └─Token(Identifier) |name|
//@[074:0075) |   ├─Token(NewLine) |\n|
  resB: sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[002:0078) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |resB|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0078) |   | └─TypePropertyAccessSyntax
//@[008:0073) |   |   ├─InstanceParameterizedTypeInstantiationSyntax
//@[008:0011) |   |   | ├─TypeVariableAccessSyntax
//@[008:0011) |   |   | | └─IdentifierSyntax
//@[008:0011) |   |   | |   └─Token(Identifier) |sys|
//@[011:0012) |   |   | ├─Token(Dot) |.|
//@[012:0025) |   |   | ├─IdentifierSyntax
//@[012:0025) |   |   | | └─Token(Identifier) |resourceInput|
//@[025:0026) |   |   | ├─Token(LeftChevron) |<|
//@[026:0072) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[026:0072) |   |   | | └─StringTypeLiteralSyntax
//@[026:0072) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[072:0073) |   |   | └─Token(RightChevron) |>|
//@[073:0074) |   |   ├─Token(Dot) |.|
//@[074:0078) |   |   └─IdentifierSyntax
//@[074:0078) |   |     └─Token(Identifier) |name|
//@[078:0079) |   ├─Token(NewLine) |\n|
  resC: sys.array
//@[002:0017) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |resC|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0017) |   | └─TypePropertyAccessSyntax
//@[008:0011) |   |   ├─TypeVariableAccessSyntax
//@[008:0011) |   |   | └─IdentifierSyntax
//@[008:0011) |   |   |   └─Token(Identifier) |sys|
//@[011:0012) |   |   ├─Token(Dot) |.|
//@[012:0017) |   |   └─IdentifierSyntax
//@[012:0017) |   |     └─Token(Identifier) |array|
//@[017:0018) |   ├─Token(NewLine) |\n|
  resD: sys.resourceInput<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[002:0081) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |resD|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0081) |   | └─TypePropertyAccessSyntax
//@[008:0076) |   |   ├─InstanceParameterizedTypeInstantiationSyntax
//@[008:0011) |   |   | ├─TypeVariableAccessSyntax
//@[008:0011) |   |   | | └─IdentifierSyntax
//@[008:0011) |   |   | |   └─Token(Identifier) |sys|
//@[011:0012) |   |   | ├─Token(Dot) |.|
//@[012:0025) |   |   | ├─IdentifierSyntax
//@[012:0025) |   |   | | └─Token(Identifier) |resourceInput|
//@[025:0026) |   |   | ├─Token(LeftChevron) |<|
//@[026:0075) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[026:0075) |   |   | | └─StringTypeLiteralSyntax
//@[026:0075) |   |   | |   └─Token(StringComplete) |'az:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[075:0076) |   |   | └─Token(RightChevron) |>|
//@[076:0077) |   |   ├─Token(Dot) |.|
//@[077:0081) |   |   └─IdentifierSyntax
//@[077:0081) |   |     └─Token(Identifier) |name|
//@[081:0082) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type strangeFormatting = {
//@[000:0287) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0022) | ├─IdentifierSyntax
//@[005:0022) | | └─Token(Identifier) |strangeFormatting|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0287) | └─ObjectTypeSyntax
//@[025:0026) |   ├─Token(LeftBrace) |{|
//@[026:0027) |   ├─Token(NewLine) |\n|
  test: resourceInput<
//@[002:0085) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |test|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0085) |   | └─TypePropertyAccessSyntax
//@[008:0080) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[008:0021) |   |   | ├─IdentifierSyntax
//@[008:0021) |   |   | | └─Token(Identifier) |resourceInput|
//@[021:0022) |   |   | ├─Token(LeftChevron) |<|
//@[022:0024) |   |   | ├─Token(NewLine) |\n\n|

  'Astronomer.Astro/organizations@2023-08-01-preview'
//@[002:0053) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[002:0053) |   |   | | └─StringTypeLiteralSyntax
//@[002:0053) |   |   | |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[053:0055) |   |   | ├─Token(NewLine) |\n\n|

>.name
//@[000:0001) |   |   | └─Token(RightChevron) |>|
//@[001:0002) |   |   ├─Token(Dot) |.|
//@[002:0006) |   |   └─IdentifierSyntax
//@[002:0006) |   |     └─Token(Identifier) |name|
//@[006:0007) |   ├─Token(NewLine) |\n|
  test2: resourceInput    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[002:0079) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |test2|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0079) |   | └─TypePropertyAccessSyntax
//@[009:0074) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[009:0022) |   |   | ├─IdentifierSyntax
//@[009:0022) |   |   | | └─Token(Identifier) |resourceInput|
//@[026:0027) |   |   | ├─Token(LeftChevron) |<|
//@[027:0073) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[027:0073) |   |   | | └─StringTypeLiteralSyntax
//@[027:0073) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[073:0074) |   |   | └─Token(RightChevron) |>|
//@[074:0075) |   |   ├─Token(Dot) |.|
//@[075:0079) |   |   └─IdentifierSyntax
//@[075:0079) |   |     └─Token(Identifier) |name|
//@[079:0080) |   ├─Token(NewLine) |\n|
  test3: resourceInput</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
//@[002:0092) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |test3|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0092) |   | └─TypePropertyAccessSyntax
//@[009:0087) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[009:0022) |   |   | ├─IdentifierSyntax
//@[009:0022) |   |   | | └─Token(Identifier) |resourceInput|
//@[022:0023) |   |   | ├─Token(LeftChevron) |<|
//@[031:0077) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[031:0077) |   |   | | └─StringTypeLiteralSyntax
//@[031:0077) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[086:0087) |   |   | └─Token(RightChevron) |>|
//@[087:0088) |   |   ├─Token(Dot) |.|
//@[088:0092) |   |   └─IdentifierSyntax
//@[088:0092) |   |     └─Token(Identifier) |name|
//@[092:0093) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@description('I love space(s)')
//@[000:0125) ├─TypeDeclarationSyntax
//@[000:0031) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0031) | | └─FunctionCallSyntax
//@[001:0012) | |   ├─IdentifierSyntax
//@[001:0012) | |   | └─Token(Identifier) |description|
//@[012:0013) | |   ├─Token(LeftParen) |(|
//@[013:0030) | |   ├─FunctionArgumentSyntax
//@[013:0030) | |   | └─StringSyntax
//@[013:0030) | |   |   └─Token(StringComplete) |'I love space(s)'|
//@[030:0031) | |   └─Token(RightParen) |)|
//@[031:0032) | ├─Token(NewLine) |\n|
type test2 = resourceInput<
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |test2|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0093) | └─TypePropertyAccessSyntax
//@[013:0088) |   ├─ParameterizedTypeInstantiationSyntax
//@[013:0026) |   | ├─IdentifierSyntax
//@[013:0026) |   | | └─Token(Identifier) |resourceInput|
//@[026:0027) |   | ├─Token(LeftChevron) |<|
//@[027:0029) |   | ├─Token(NewLine) |\n\n|

     'Astronomer.Astro/organizations@2023-08-01-preview'
//@[005:0056) |   | ├─ParameterizedTypeArgumentSyntax
//@[005:0056) |   | | └─StringTypeLiteralSyntax
//@[005:0056) |   | |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[056:0058) |   | ├─Token(NewLine) |\n\n|

>.name
//@[000:0001) |   | └─Token(RightChevron) |>|
//@[001:0002) |   ├─Token(Dot) |.|
//@[002:0006) |   └─IdentifierSyntax
//@[002:0006) |     └─Token(Identifier) |name|
//@[006:0008) ├─Token(NewLine) |\n\n|

param bar resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@[000:0130) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0009) | ├─IdentifierSyntax
//@[006:0009) | | └─Token(Identifier) |bar|
//@[010:0073) | ├─TypePropertyAccessSyntax
//@[010:0062) | | ├─ParameterizedTypeInstantiationSyntax
//@[010:0023) | | | ├─IdentifierSyntax
//@[010:0023) | | | | └─Token(Identifier) |resourceInput|
//@[023:0024) | | | ├─Token(LeftChevron) |<|
//@[024:0061) | | | ├─ParameterizedTypeArgumentSyntax
//@[024:0061) | | | | └─StringTypeLiteralSyntax
//@[024:0061) | | | |   └─Token(StringComplete) |'Microsoft.Resources/tags@2022-09-01'|
//@[061:0062) | | | └─Token(RightChevron) |>|
//@[062:0063) | | ├─Token(Dot) |.|
//@[063:0073) | | └─IdentifierSyntax
//@[063:0073) | |   └─Token(Identifier) |properties|
//@[074:0130) | └─ParameterDefaultValueSyntax
//@[074:0075) |   ├─Token(Assignment) |=|
//@[076:0130) |   └─ObjectSyntax
//@[076:0077) |     ├─Token(LeftBrace) |{|
//@[077:0078) |     ├─Token(NewLine) |\n|
  tags: {
//@[002:0050) |     ├─ObjectPropertySyntax
//@[002:0006) |     | ├─IdentifierSyntax
//@[002:0006) |     | | └─Token(Identifier) |tags|
//@[006:0007) |     | ├─Token(Colon) |:|
//@[008:0050) |     | └─ObjectSyntax
//@[008:0009) |     |   ├─Token(LeftBrace) |{|
//@[009:0010) |     |   ├─Token(NewLine) |\n|
    fizz: 'buzz'
//@[004:0016) |     |   ├─ObjectPropertySyntax
//@[004:0008) |     |   | ├─IdentifierSyntax
//@[004:0008) |     |   | | └─Token(Identifier) |fizz|
//@[008:0009) |     |   | ├─Token(Colon) |:|
//@[010:0016) |     |   | └─StringSyntax
//@[010:0016) |     |   |   └─Token(StringComplete) |'buzz'|
//@[016:0017) |     |   ├─Token(NewLine) |\n|
    snap: 'crackle'
//@[004:0019) |     |   ├─ObjectPropertySyntax
//@[004:0008) |     |   | ├─IdentifierSyntax
//@[004:0008) |     |   | | └─Token(Identifier) |snap|
//@[008:0009) |     |   | ├─Token(Colon) |:|
//@[010:0019) |     |   | └─StringSyntax
//@[010:0019) |     |   |   └─Token(StringComplete) |'crackle'|
//@[019:0020) |     |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |     |   └─Token(RightBrace) |}|
//@[003:0004) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

output baz resourceInput<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@[000:0101) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |baz|
//@[011:0092) | ├─TypePropertyAccessSyntax
//@[011:0087) | | ├─ParameterizedTypeInstantiationSyntax
//@[011:0024) | | | ├─IdentifierSyntax
//@[011:0024) | | | | └─Token(Identifier) |resourceInput|
//@[024:0025) | | | ├─Token(LeftChevron) |<|
//@[025:0086) | | | ├─ParameterizedTypeArgumentSyntax
//@[025:0086) | | | | └─StringTypeLiteralSyntax
//@[025:0086) | | | |   └─Token(StringComplete) |'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'|
//@[086:0087) | | | └─Token(RightChevron) |>|
//@[087:0088) | | ├─Token(Dot) |.|
//@[088:0092) | | └─IdentifierSyntax
//@[088:0092) | |   └─Token(Identifier) |name|
//@[093:0094) | ├─Token(Assignment) |=|
//@[095:0101) | └─StringSyntax
//@[095:0101) |   └─Token(StringComplete) |'myId'|
//@[101:0103) ├─Token(NewLine) |\n\n|

type storageAccountName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[000:0092) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0023) | ├─IdentifierSyntax
//@[005:0023) | | └─Token(Identifier) |storageAccountName|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0092) | └─TypePropertyAccessSyntax
//@[026:0087) |   ├─ParameterizedTypeInstantiationSyntax
//@[026:0039) |   | ├─IdentifierSyntax
//@[026:0039) |   | | └─Token(Identifier) |resourceInput|
//@[039:0040) |   | ├─Token(LeftChevron) |<|
//@[040:0086) |   | ├─ParameterizedTypeArgumentSyntax
//@[040:0086) |   | | └─StringTypeLiteralSyntax
//@[040:0086) |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[086:0087) |   | └─Token(RightChevron) |>|
//@[087:0088) |   ├─Token(Dot) |.|
//@[088:0092) |   └─IdentifierSyntax
//@[088:0092) |     └─Token(Identifier) |name|
//@[092:0093) ├─Token(NewLine) |\n|
type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[000:0102) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0017) | ├─IdentifierSyntax
//@[005:0017) | | └─Token(Identifier) |accessPolicy|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0102) | └─TypeItemsAccessSyntax
//@[020:0099) |   ├─TypePropertyAccessSyntax
//@[020:0084) |   | ├─TypePropertyAccessSyntax
//@[020:0073) |   | | ├─ParameterizedTypeInstantiationSyntax
//@[020:0033) |   | | | ├─IdentifierSyntax
//@[020:0033) |   | | | | └─Token(Identifier) |resourceInput|
//@[033:0034) |   | | | ├─Token(LeftChevron) |<|
//@[034:0072) |   | | | ├─ParameterizedTypeArgumentSyntax
//@[034:0072) |   | | | | └─StringTypeLiteralSyntax
//@[034:0072) |   | | | |   └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[072:0073) |   | | | └─Token(RightChevron) |>|
//@[073:0074) |   | | ├─Token(Dot) |.|
//@[074:0084) |   | | └─IdentifierSyntax
//@[074:0084) |   | |   └─Token(Identifier) |properties|
//@[084:0085) |   | ├─Token(Dot) |.|
//@[085:0099) |   | └─IdentifierSyntax
//@[085:0099) |   |   └─Token(Identifier) |accessPolicies|
//@[099:0100) |   ├─Token(LeftSquare) |[|
//@[100:0101) |   ├─Token(Asterisk) |*|
//@[101:0102) |   └─Token(RightSquare) |]|
//@[102:0103) ├─Token(NewLine) |\n|
type tag = resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[000:0081) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0008) | ├─IdentifierSyntax
//@[005:0008) | | └─Token(Identifier) |tag|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0081) | └─TypeAdditionalPropertiesAccessSyntax
//@[011:0079) |   ├─TypePropertyAccessSyntax
//@[011:0074) |   | ├─TypePropertyAccessSyntax
//@[011:0063) |   | | ├─ParameterizedTypeInstantiationSyntax
//@[011:0024) |   | | | ├─IdentifierSyntax
//@[011:0024) |   | | | | └─Token(Identifier) |resourceInput|
//@[024:0025) |   | | | ├─Token(LeftChevron) |<|
//@[025:0062) |   | | | ├─ParameterizedTypeArgumentSyntax
//@[025:0062) |   | | | | └─StringTypeLiteralSyntax
//@[025:0062) |   | | | |   └─Token(StringComplete) |'Microsoft.Resources/tags@2022-09-01'|
//@[062:0063) |   | | | └─Token(RightChevron) |>|
//@[063:0064) |   | | ├─Token(Dot) |.|
//@[064:0074) |   | | └─IdentifierSyntax
//@[064:0074) |   | |   └─Token(Identifier) |properties|
//@[074:0075) |   | ├─Token(Dot) |.|
//@[075:0079) |   | └─IdentifierSyntax
//@[075:0079) |   |   └─Token(Identifier) |tags|
//@[079:0080) |   ├─Token(Dot) |.|
//@[080:0081) |   └─Token(Asterisk) |*|
//@[081:0082) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
