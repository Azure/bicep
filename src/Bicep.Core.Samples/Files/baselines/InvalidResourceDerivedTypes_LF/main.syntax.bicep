type invalid1 = resource
//@[000:1602) ProgramSyntax
//@[000:0024) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid1|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0024) | └─ResourceTypeSyntax
//@[016:0024) |   ├─Token(Identifier) |resource|
//@[024:0024) |   └─SkippedTriviaSyntax
//@[024:0026) ├─Token(NewLine) |\n\n|

type invalid2 = resource<>
//@[000:0026) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid2|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0026) | └─ParameterizedTypeInstantiationSyntax
//@[016:0024) |   ├─IdentifierSyntax
//@[016:0024) |   | └─Token(Identifier) |resource|
//@[024:0025) |   ├─Token(LeftChevron) |<|
//@[025:0026) |   └─Token(RightChevron) |>|
//@[026:0028) ├─Token(NewLine) |\n\n|

type invalid3 = resource<'abc', 'def'>
//@[000:0038) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid3|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0038) | └─ParameterizedTypeInstantiationSyntax
//@[016:0024) |   ├─IdentifierSyntax
//@[016:0024) |   | └─Token(Identifier) |resource|
//@[024:0025) |   ├─Token(LeftChevron) |<|
//@[025:0030) |   ├─ParameterizedTypeArgumentSyntax
//@[025:0030) |   | └─StringTypeLiteralSyntax
//@[025:0030) |   |   └─Token(StringComplete) |'abc'|
//@[030:0031) |   ├─Token(Comma) |,|
//@[032:0037) |   ├─ParameterizedTypeArgumentSyntax
//@[032:0037) |   | └─StringTypeLiteralSyntax
//@[032:0037) |   |   └─Token(StringComplete) |'def'|
//@[037:0038) |   └─Token(RightChevron) |>|
//@[038:0039) ├─Token(NewLine) |\n|
type invalid4 = resource<hello>
//@[000:0031) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid4|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0031) | └─ParameterizedTypeInstantiationSyntax
//@[016:0024) |   ├─IdentifierSyntax
//@[016:0024) |   | └─Token(Identifier) |resource|
//@[024:0025) |   ├─Token(LeftChevron) |<|
//@[025:0030) |   ├─ParameterizedTypeArgumentSyntax
//@[025:0030) |   | └─TypeVariableAccessSyntax
//@[025:0030) |   |   └─IdentifierSyntax
//@[025:0030) |   |     └─Token(Identifier) |hello|
//@[030:0031) |   └─Token(RightChevron) |>|
//@[031:0032) ├─Token(NewLine) |\n|
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
//@[000:0061) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid5|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0061) | └─ParameterizedTypeInstantiationSyntax
//@[016:0024) |   ├─IdentifierSyntax
//@[016:0024) |   | └─Token(Identifier) |resource|
//@[024:0025) |   ├─Token(LeftChevron) |<|
//@[025:0060) |   ├─ParameterizedTypeArgumentSyntax
//@[025:0060) |   | └─StringTypeLiteralSyntax
//@[025:0060) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts'|
//@[060:0061) |   └─Token(RightChevron) |>|
//@[061:0062) ├─Token(NewLine) |\n|
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
//@[000:0062) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid6|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0062) | └─ParameterizedTypeInstantiationSyntax
//@[016:0024) |   ├─IdentifierSyntax
//@[016:0024) |   | └─Token(Identifier) |resource|
//@[024:0025) |   ├─Token(LeftChevron) |<|
//@[025:0061) |   ├─ParameterizedTypeArgumentSyntax
//@[025:0061) |   | └─StringTypeLiteralSyntax
//@[025:0061) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@'|
//@[061:0062) |   └─Token(RightChevron) |>|
//@[062:0063) ├─Token(NewLine) |\n|
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
//@[000:0067) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid7|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0067) | └─ParameterizedTypeInstantiationSyntax
//@[016:0024) |   ├─IdentifierSyntax
//@[016:0024) |   | └─Token(Identifier) |resource|
//@[024:0025) |   ├─Token(LeftChevron) |<|
//@[025:0066) |   ├─ParameterizedTypeArgumentSyntax
//@[025:0066) |   | └─StringTypeLiteralSyntax
//@[025:0066) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@hello'|
//@[066:0067) |   └─Token(RightChevron) |>|
//@[067:0068) ├─Token(NewLine) |\n|
type invalid8 = resource<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:0090) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid8|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0090) | └─ParameterizedTypeInstantiationSyntax
//@[016:0024) |   ├─IdentifierSyntax
//@[016:0024) |   | └─Token(Identifier) |resource|
//@[024:0025) |   ├─Token(LeftChevron) |<|
//@[025:0089) |   ├─ParameterizedTypeArgumentSyntax
//@[025:0089) |   | └─StringTypeLiteralSyntax
//@[025:0089) |   |   └─Token(StringComplete) |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[089:0090) |   └─Token(RightChevron) |>|
//@[090:0091) ├─Token(NewLine) |\n|
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:0073) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid9|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0073) | └─ParameterizedTypeInstantiationSyntax
//@[016:0024) |   ├─IdentifierSyntax
//@[016:0024) |   | └─Token(Identifier) |resource|
//@[024:0025) |   ├─Token(LeftChevron) |<|
//@[025:0072) |   ├─ParameterizedTypeArgumentSyntax
//@[025:0072) |   | └─StringTypeLiteralSyntax
//@[025:0072) |   |   └─Token(StringComplete) |':Microsoft.Storage/storageAccounts@2022-09-01'|
//@[072:0073) |   └─Token(RightChevron) |>|
//@[073:0074) ├─Token(NewLine) |\n|
type invalid10 = resource<'abc' 'def'>
//@[000:0038) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |invalid10|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0038) | └─ParameterizedTypeInstantiationSyntax
//@[017:0025) |   ├─IdentifierSyntax
//@[017:0025) |   | └─Token(Identifier) |resource|
//@[025:0026) |   ├─Token(LeftChevron) |<|
//@[026:0031) |   ├─ParameterizedTypeArgumentSyntax
//@[026:0031) |   | └─StringTypeLiteralSyntax
//@[026:0031) |   |   └─Token(StringComplete) |'abc'|
//@[032:0032) |   ├─SkippedTriviaSyntax
//@[032:0037) |   ├─ParameterizedTypeArgumentSyntax
//@[032:0037) |   | └─StringTypeLiteralSyntax
//@[032:0037) |   |   └─Token(StringComplete) |'def'|
//@[037:0038) |   └─Token(RightChevron) |>|
//@[038:0039) ├─Token(NewLine) |\n|
type invalid11 = resource<123>
//@[000:0030) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |invalid11|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0030) | └─ParameterizedTypeInstantiationSyntax
//@[017:0025) |   ├─IdentifierSyntax
//@[017:0025) |   | └─Token(Identifier) |resource|
//@[025:0026) |   ├─Token(LeftChevron) |<|
//@[026:0029) |   ├─ParameterizedTypeArgumentSyntax
//@[026:0029) |   | └─IntegerTypeLiteralSyntax
//@[026:0029) |   |   └─Token(Integer) |123|
//@[029:0030) |   └─Token(RightChevron) |>|
//@[030:0031) ├─Token(NewLine) |\n|
type invalid12 = resource<resourceGroup()>
//@[000:0042) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |invalid12|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0042) | └─ParameterizedTypeInstantiationSyntax
//@[017:0025) |   ├─IdentifierSyntax
//@[017:0025) |   | └─Token(Identifier) |resource|
//@[025:0026) |   ├─Token(LeftChevron) |<|
//@[026:0039) |   ├─ParameterizedTypeArgumentSyntax
//@[026:0039) |   | └─TypeVariableAccessSyntax
//@[026:0039) |   |   └─IdentifierSyntax
//@[026:0039) |   |     └─Token(Identifier) |resourceGroup|
//@[039:0039) |   ├─SkippedTriviaSyntax
//@[039:0041) |   ├─ParameterizedTypeArgumentSyntax
//@[039:0041) |   | └─ParenthesizedTypeSyntax
//@[039:0040) |   |   ├─Token(LeftParen) |(|
//@[040:0040) |   |   ├─SkippedTriviaSyntax
//@[040:0041) |   |   └─Token(RightParen) |)|
//@[041:0042) |   └─Token(RightChevron) |>|
//@[042:0044) ├─Token(NewLine) |\n\n|

type thisIsWeird = resource</*
//@[000:0093) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0016) | ├─IdentifierSyntax
//@[005:0016) | | └─Token(Identifier) |thisIsWeird|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0093) | └─ParameterizedTypeInstantiationSyntax
//@[019:0027) |   ├─IdentifierSyntax
//@[019:0027) |   | └─Token(Identifier) |resource|
//@[027:0028) |   ├─Token(LeftChevron) |<|
*/'Astronomer.Astro/organizations@2023-08-01-preview'
//@[002:0053) |   ├─ParameterizedTypeArgumentSyntax
//@[002:0053) |   | └─StringTypeLiteralSyntax
//@[002:0053) |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[053:0053) |   ├─SkippedTriviaSyntax
//@[053:0054) |   ├─Token(NewLine) |\n|
///  >
//@[006:0007) |   ├─Token(NewLine) |\n|
>
//@[000:0001) |   └─Token(RightChevron) |>|
//@[001:0003) ├─Token(NewLine) |\n\n|

type interpolated = resource<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[000:0081) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0017) | ├─IdentifierSyntax
//@[005:0017) | | └─Token(Identifier) |interpolated|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0081) | └─ParameterizedTypeInstantiationSyntax
//@[020:0028) |   ├─IdentifierSyntax
//@[020:0028) |   | └─Token(Identifier) |resource|
//@[028:0029) |   ├─Token(LeftChevron) |<|
//@[029:0080) |   ├─ParameterizedTypeArgumentSyntax
//@[029:0080) |   | └─StringTypeLiteralSyntax
//@[029:0042) |   |   ├─Token(StringLeftPiece) |'Microsoft.${|
//@[042:0051) |   |   ├─StringSyntax
//@[042:0051) |   |   | └─Token(StringComplete) |'Storage'|
//@[051:0080) |   |   └─Token(StringRightPiece) |}/storageAccounts@2022-09-01'|
//@[080:0081) |   └─Token(RightChevron) |>|
//@[081:0083) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0093) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable = resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |shouldNotBeSealable|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0083) | └─ParameterizedTypeInstantiationSyntax
//@[027:0035) |   ├─IdentifierSyntax
//@[027:0035) |   | └─Token(Identifier) |resource|
//@[035:0036) |   ├─Token(LeftChevron) |<|
//@[036:0082) |   ├─ParameterizedTypeArgumentSyntax
//@[036:0082) |   | └─StringTypeLiteralSyntax
//@[036:0082) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[082:0083) |   └─Token(RightChevron) |>|
//@[083:0085) ├─Token(NewLine) |\n\n|

type hello = {
//@[000:0108) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |hello|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0108) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  @discriminator('hi')
//@[002:0091) |   ├─ObjectTypePropertySyntax
//@[002:0022) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0022) |   | | └─FunctionCallSyntax
//@[003:0016) |   | |   ├─IdentifierSyntax
//@[003:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[016:0017) |   | |   ├─Token(LeftParen) |(|
//@[017:0021) |   | |   ├─FunctionArgumentSyntax
//@[017:0021) |   | |   | └─StringSyntax
//@[017:0021) |   | |   |   └─Token(StringComplete) |'hi'|
//@[021:0022) |   | |   └─Token(RightParen) |)|
//@[022:0023) |   | ├─Token(NewLine) |\n|
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[002:0005) |   | ├─IdentifierSyntax
//@[002:0005) |   | | └─Token(Identifier) |bar|
//@[005:0006) |   | ├─Token(Colon) |:|
//@[007:0068) |   | └─ParameterizedTypeInstantiationSyntax
//@[007:0015) |   |   ├─IdentifierSyntax
//@[007:0015) |   |   | └─Token(Identifier) |resource|
//@[015:0016) |   |   ├─Token(LeftChevron) |<|
//@[016:0067) |   |   ├─ParameterizedTypeArgumentSyntax
//@[016:0067) |   |   | └─StringTypeLiteralSyntax
//@[016:0067) |   |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[067:0068) |   |   └─Token(RightChevron) |>|
//@[068:0069) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type typoInPropertyName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
//@[000:0086) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0023) | ├─IdentifierSyntax
//@[005:0023) | | └─Token(Identifier) |typoInPropertyName|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0086) | └─TypePropertyAccessSyntax
//@[026:0082) |   ├─ParameterizedTypeInstantiationSyntax
//@[026:0034) |   | ├─IdentifierSyntax
//@[026:0034) |   | | └─Token(Identifier) |resource|
//@[034:0035) |   | ├─Token(LeftChevron) |<|
//@[035:0081) |   | ├─ParameterizedTypeArgumentSyntax
//@[035:0081) |   | | └─StringTypeLiteralSyntax
//@[035:0081) |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[081:0082) |   | └─Token(RightChevron) |>|
//@[082:0083) |   ├─Token(Dot) |.|
//@[083:0086) |   └─IdentifierSyntax
//@[083:0086) |     └─Token(Identifier) |nom|
//@[086:0087) ├─Token(NewLine) |\n|
type typoInPropertyName2 = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
//@[000:0112) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |typoInPropertyName2|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0112) | └─TypePropertyAccessSyntax
//@[027:0104) |   ├─TypeItemsAccessSyntax
//@[027:0101) |   | ├─TypePropertyAccessSyntax
//@[027:0086) |   | | ├─TypePropertyAccessSyntax
//@[027:0075) |   | | | ├─ParameterizedTypeInstantiationSyntax
//@[027:0035) |   | | | | ├─IdentifierSyntax
//@[027:0035) |   | | | | | └─Token(Identifier) |resource|
//@[035:0036) |   | | | | ├─Token(LeftChevron) |<|
//@[036:0074) |   | | | | ├─ParameterizedTypeArgumentSyntax
//@[036:0074) |   | | | | | └─StringTypeLiteralSyntax
//@[036:0074) |   | | | | |   └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[074:0075) |   | | | | └─Token(RightChevron) |>|
//@[075:0076) |   | | | ├─Token(Dot) |.|
//@[076:0086) |   | | | └─IdentifierSyntax
//@[076:0086) |   | | |   └─Token(Identifier) |properties|
//@[086:0087) |   | | ├─Token(Dot) |.|
//@[087:0101) |   | | └─IdentifierSyntax
//@[087:0101) |   | |   └─Token(Identifier) |accessPolicies|
//@[101:0102) |   | ├─Token(LeftSquare) |[|
//@[102:0103) |   | ├─Token(Asterisk) |*|
//@[103:0104) |   | └─Token(RightSquare) |]|
//@[104:0105) |   ├─Token(Dot) |.|
//@[105:0112) |   └─IdentifierSyntax
//@[105:0112) |     └─Token(Identifier) |tenatId|
//@[112:0113) ├─Token(NewLine) |\n|
type typoInPropertyName3 = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
//@[000:0113) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |typoInPropertyName3|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0113) | └─TypePropertyAccessSyntax
//@[027:0104) |   ├─TypePropertyAccessSyntax
//@[027:0089) |   | ├─TypeItemsAccessSyntax
//@[027:0086) |   | | ├─TypePropertyAccessSyntax
//@[027:0075) |   | | | ├─ParameterizedTypeInstantiationSyntax
//@[027:0035) |   | | | | ├─IdentifierSyntax
//@[027:0035) |   | | | | | └─Token(Identifier) |resource|
//@[035:0036) |   | | | | ├─Token(LeftChevron) |<|
//@[036:0074) |   | | | | ├─ParameterizedTypeArgumentSyntax
//@[036:0074) |   | | | | | └─StringTypeLiteralSyntax
//@[036:0074) |   | | | | |   └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[074:0075) |   | | | | └─Token(RightChevron) |>|
//@[075:0076) |   | | | ├─Token(Dot) |.|
//@[076:0086) |   | | | └─IdentifierSyntax
//@[076:0086) |   | | |   └─Token(Identifier) |properties|
//@[086:0087) |   | | ├─Token(LeftSquare) |[|
//@[087:0088) |   | | ├─Token(Asterisk) |*|
//@[088:0089) |   | | └─Token(RightSquare) |]|
//@[089:0090) |   | ├─Token(Dot) |.|
//@[090:0104) |   | └─IdentifierSyntax
//@[090:0104) |   |   └─Token(Identifier) |accessPolicies|
//@[104:0105) |   ├─Token(Dot) |.|
//@[105:0113) |   └─IdentifierSyntax
//@[105:0113) |     └─Token(Identifier) |tenantId|
//@[113:0114) ├─Token(NewLine) |\n|
type typoInPropertyName4 = resource<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
//@[000:0114) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |typoInPropertyName4|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0114) | └─TypePropertyAccessSyntax
//@[027:0108) |   ├─TypeAdditionalPropertiesAccessSyntax
//@[027:0106) |   | ├─TypePropertyAccessSyntax
//@[027:0085) |   | | ├─TypePropertyAccessSyntax
//@[027:0074) |   | | | ├─ParameterizedTypeInstantiationSyntax
//@[027:0035) |   | | | | ├─IdentifierSyntax
//@[027:0035) |   | | | | | └─Token(Identifier) |resource|
//@[035:0036) |   | | | | ├─Token(LeftChevron) |<|
//@[036:0073) |   | | | | ├─ParameterizedTypeArgumentSyntax
//@[036:0073) |   | | | | | └─StringTypeLiteralSyntax
//@[036:0073) |   | | | | |   └─Token(StringComplete) |'Microsoft.Web/customApis@2016-06-01'|
//@[073:0074) |   | | | | └─Token(RightChevron) |>|
//@[074:0075) |   | | | ├─Token(Dot) |.|
//@[075:0085) |   | | | └─IdentifierSyntax
//@[075:0085) |   | | |   └─Token(Identifier) |properties|
//@[085:0086) |   | | ├─Token(Dot) |.|
//@[086:0106) |   | | └─IdentifierSyntax
//@[086:0106) |   | |   └─Token(Identifier) |connectionParameters|
//@[106:0107) |   | ├─Token(Dot) |.|
//@[107:0108) |   | └─Token(Asterisk) |*|
//@[108:0109) |   ├─Token(Dot) |.|
//@[109:0114) |   └─IdentifierSyntax
//@[109:0114) |     └─Token(Identifier) |tyype|
//@[114:0115) ├─Token(NewLine) |\n|
type typoInPropertyName5 = resource<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type
//@[000:0113) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |typoInPropertyName5|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0113) | └─TypePropertyAccessSyntax
//@[027:0108) |   ├─TypePropertyAccessSyntax
//@[027:0087) |   | ├─TypeAdditionalPropertiesAccessSyntax
//@[027:0085) |   | | ├─TypePropertyAccessSyntax
//@[027:0074) |   | | | ├─ParameterizedTypeInstantiationSyntax
//@[027:0035) |   | | | | ├─IdentifierSyntax
//@[027:0035) |   | | | | | └─Token(Identifier) |resource|
//@[035:0036) |   | | | | ├─Token(LeftChevron) |<|
//@[036:0073) |   | | | | ├─ParameterizedTypeArgumentSyntax
//@[036:0073) |   | | | | | └─StringTypeLiteralSyntax
//@[036:0073) |   | | | | |   └─Token(StringComplete) |'Microsoft.Web/customApis@2016-06-01'|
//@[073:0074) |   | | | | └─Token(RightChevron) |>|
//@[074:0075) |   | | | ├─Token(Dot) |.|
//@[075:0085) |   | | | └─IdentifierSyntax
//@[075:0085) |   | | |   └─Token(Identifier) |properties|
//@[085:0086) |   | | ├─Token(Dot) |.|
//@[086:0087) |   | | └─Token(Asterisk) |*|
//@[087:0088) |   | ├─Token(Dot) |.|
//@[088:0108) |   | └─IdentifierSyntax
//@[088:0108) |   |   └─Token(Identifier) |connectionParameters|
//@[108:0109) |   ├─Token(Dot) |.|
//@[109:0113) |   └─IdentifierSyntax
//@[109:0113) |     └─Token(Identifier) |type|
//@[113:0115) ├─Token(NewLine) |\n\n|

module mod 'modules/mod.json' = {
//@[000:0077) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |mod|
//@[011:0029) | ├─StringSyntax
//@[011:0029) | | └─Token(StringComplete) |'modules/mod.json'|
//@[030:0031) | ├─Token(Assignment) |=|
//@[032:0077) | └─ObjectSyntax
//@[032:0033) |   ├─Token(LeftBrace) |{|
//@[033:0034) |   ├─Token(NewLine) |\n|
  name: 'mod'
//@[002:0013) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0013) |   | └─StringSyntax
//@[008:0013) |   |   └─Token(StringComplete) |'mod'|
//@[013:0014) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0027) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0027) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    foo: {}
//@[004:0011) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |foo|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0011) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0011) |   |   |   └─Token(RightBrace) |}|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
