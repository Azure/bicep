type invalid1 = resource
//@[00:1020) ProgramSyntax
//@[00:0024) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid1|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0024) | └─ResourceTypeSyntax
//@[16:0024) |   ├─Token(Identifier) |resource|
//@[24:0024) |   └─SkippedTriviaSyntax
//@[24:0026) ├─Token(NewLine) |\n\n|

type invalid2 = resource<>
//@[00:0026) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid2|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0026) | └─ParameterizedTypeInstantiationSyntax
//@[16:0024) |   ├─IdentifierSyntax
//@[16:0024) |   | └─Token(Identifier) |resource|
//@[24:0025) |   ├─Token(LeftChevron) |<|
//@[25:0026) |   └─Token(RightChevron) |>|
//@[26:0028) ├─Token(NewLine) |\n\n|

type invalid3 = resource<'abc', 'def'>
//@[00:0038) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid3|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0038) | └─ParameterizedTypeInstantiationSyntax
//@[16:0024) |   ├─IdentifierSyntax
//@[16:0024) |   | └─Token(Identifier) |resource|
//@[24:0025) |   ├─Token(LeftChevron) |<|
//@[25:0030) |   ├─ParameterizedTypeArgumentSyntax
//@[25:0030) |   | └─StringSyntax
//@[25:0030) |   |   └─Token(StringComplete) |'abc'|
//@[30:0031) |   ├─Token(Comma) |,|
//@[32:0037) |   ├─ParameterizedTypeArgumentSyntax
//@[32:0037) |   | └─StringSyntax
//@[32:0037) |   |   └─Token(StringComplete) |'def'|
//@[37:0038) |   └─Token(RightChevron) |>|
//@[38:0039) ├─Token(NewLine) |\n|
type invalid4 = resource<hello>
//@[00:0031) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid4|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0031) | └─ParameterizedTypeInstantiationSyntax
//@[16:0024) |   ├─IdentifierSyntax
//@[16:0024) |   | └─Token(Identifier) |resource|
//@[24:0025) |   ├─Token(LeftChevron) |<|
//@[25:0030) |   ├─ParameterizedTypeArgumentSyntax
//@[25:0030) |   | └─VariableAccessSyntax
//@[25:0030) |   |   └─IdentifierSyntax
//@[25:0030) |   |     └─Token(Identifier) |hello|
//@[30:0031) |   └─Token(RightChevron) |>|
//@[31:0032) ├─Token(NewLine) |\n|
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
//@[00:0061) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid5|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0061) | └─ParameterizedTypeInstantiationSyntax
//@[16:0024) |   ├─IdentifierSyntax
//@[16:0024) |   | └─Token(Identifier) |resource|
//@[24:0025) |   ├─Token(LeftChevron) |<|
//@[25:0060) |   ├─ParameterizedTypeArgumentSyntax
//@[25:0060) |   | └─StringSyntax
//@[25:0060) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts'|
//@[60:0061) |   └─Token(RightChevron) |>|
//@[61:0062) ├─Token(NewLine) |\n|
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
//@[00:0062) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid6|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0062) | └─ParameterizedTypeInstantiationSyntax
//@[16:0024) |   ├─IdentifierSyntax
//@[16:0024) |   | └─Token(Identifier) |resource|
//@[24:0025) |   ├─Token(LeftChevron) |<|
//@[25:0061) |   ├─ParameterizedTypeArgumentSyntax
//@[25:0061) |   | └─StringSyntax
//@[25:0061) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@'|
//@[61:0062) |   └─Token(RightChevron) |>|
//@[62:0063) ├─Token(NewLine) |\n|
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
//@[00:0067) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid7|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0067) | └─ParameterizedTypeInstantiationSyntax
//@[16:0024) |   ├─IdentifierSyntax
//@[16:0024) |   | └─Token(Identifier) |resource|
//@[24:0025) |   ├─Token(LeftChevron) |<|
//@[25:0066) |   ├─ParameterizedTypeArgumentSyntax
//@[25:0066) |   | └─StringSyntax
//@[25:0066) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@hello'|
//@[66:0067) |   └─Token(RightChevron) |>|
//@[67:0068) ├─Token(NewLine) |\n|
type invalid8 = resource<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:0090) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid8|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0090) | └─ParameterizedTypeInstantiationSyntax
//@[16:0024) |   ├─IdentifierSyntax
//@[16:0024) |   | └─Token(Identifier) |resource|
//@[24:0025) |   ├─Token(LeftChevron) |<|
//@[25:0089) |   ├─ParameterizedTypeArgumentSyntax
//@[25:0089) |   | └─StringSyntax
//@[25:0089) |   |   └─Token(StringComplete) |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[89:0090) |   └─Token(RightChevron) |>|
//@[90:0091) ├─Token(NewLine) |\n|
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:0073) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |invalid9|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0073) | └─ParameterizedTypeInstantiationSyntax
//@[16:0024) |   ├─IdentifierSyntax
//@[16:0024) |   | └─Token(Identifier) |resource|
//@[24:0025) |   ├─Token(LeftChevron) |<|
//@[25:0072) |   ├─ParameterizedTypeArgumentSyntax
//@[25:0072) |   | └─StringSyntax
//@[25:0072) |   |   └─Token(StringComplete) |':Microsoft.Storage/storageAccounts@2022-09-01'|
//@[72:0073) |   └─Token(RightChevron) |>|
//@[73:0074) ├─Token(NewLine) |\n|
type invalid10 = resource<'abc' 'def'>
//@[00:0038) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |invalid10|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0038) | └─ParameterizedTypeInstantiationSyntax
//@[17:0025) |   ├─IdentifierSyntax
//@[17:0025) |   | └─Token(Identifier) |resource|
//@[25:0026) |   ├─Token(LeftChevron) |<|
//@[26:0031) |   ├─ParameterizedTypeArgumentSyntax
//@[26:0031) |   | └─StringSyntax
//@[26:0031) |   |   └─Token(StringComplete) |'abc'|
//@[32:0032) |   ├─SkippedTriviaSyntax
//@[32:0037) |   ├─ParameterizedTypeArgumentSyntax
//@[32:0037) |   | └─StringSyntax
//@[32:0037) |   |   └─Token(StringComplete) |'def'|
//@[37:0038) |   └─Token(RightChevron) |>|
//@[38:0039) ├─Token(NewLine) |\n|
type invalid11 = resource<123>
//@[00:0030) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |invalid11|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0030) | └─ParameterizedTypeInstantiationSyntax
//@[17:0025) |   ├─IdentifierSyntax
//@[17:0025) |   | └─Token(Identifier) |resource|
//@[25:0026) |   ├─Token(LeftChevron) |<|
//@[26:0029) |   ├─ParameterizedTypeArgumentSyntax
//@[26:0029) |   | └─IntegerLiteralSyntax
//@[26:0029) |   |   └─Token(Integer) |123|
//@[29:0030) |   └─Token(RightChevron) |>|
//@[30:0031) ├─Token(NewLine) |\n|
type invalid12 = resource<resourceGroup()>
//@[00:0042) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |invalid12|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0042) | └─ParameterizedTypeInstantiationSyntax
//@[17:0025) |   ├─IdentifierSyntax
//@[17:0025) |   | └─Token(Identifier) |resource|
//@[25:0026) |   ├─Token(LeftChevron) |<|
//@[26:0039) |   ├─ParameterizedTypeArgumentSyntax
//@[26:0039) |   | └─VariableAccessSyntax
//@[26:0039) |   |   └─IdentifierSyntax
//@[26:0039) |   |     └─Token(Identifier) |resourceGroup|
//@[39:0039) |   ├─SkippedTriviaSyntax
//@[39:0041) |   ├─ParameterizedTypeArgumentSyntax
//@[39:0041) |   | └─ParenthesizedExpressionSyntax
//@[39:0040) |   |   ├─Token(LeftParen) |(|
//@[40:0040) |   |   ├─SkippedTriviaSyntax
//@[40:0041) |   |   └─Token(RightParen) |)|
//@[41:0042) |   └─Token(RightChevron) |>|
//@[42:0044) ├─Token(NewLine) |\n\n|

type thisIsWeird = resource</*
//@[00:0094) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0016) | ├─IdentifierSyntax
//@[05:0016) | | └─Token(Identifier) |thisIsWeird|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0094) | └─ParameterizedTypeInstantiationSyntax
//@[19:0027) |   ├─IdentifierSyntax
//@[19:0027) |   | └─Token(Identifier) |resource|
//@[27:0028) |   ├─Token(LeftChevron) |<|
*/'Astronomer.Astro/organizations@2023-08-01-preview' 
//@[02:0053) |   ├─ParameterizedTypeArgumentSyntax
//@[02:0053) |   | └─StringSyntax
//@[02:0053) |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[54:0054) |   ├─SkippedTriviaSyntax
//@[54:0055) |   ├─Token(NewLine) |\n|
///  >
//@[06:0007) |   ├─Token(NewLine) |\n|
>
//@[00:0001) |   └─Token(RightChevron) |>|
//@[01:0003) ├─Token(NewLine) |\n\n|

type shouldWeBlockThis = resource<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[00:0086) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0022) | ├─IdentifierSyntax
//@[05:0022) | | └─Token(Identifier) |shouldWeBlockThis|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0086) | └─ParameterizedTypeInstantiationSyntax
//@[25:0033) |   ├─IdentifierSyntax
//@[25:0033) |   | └─Token(Identifier) |resource|
//@[33:0034) |   ├─Token(LeftChevron) |<|
//@[34:0085) |   ├─ParameterizedTypeArgumentSyntax
//@[34:0085) |   | └─StringSyntax
//@[34:0047) |   |   ├─Token(StringLeftPiece) |'Microsoft.${|
//@[47:0056) |   |   ├─StringSyntax
//@[47:0056) |   |   | └─Token(StringComplete) |'Storage'|
//@[56:0085) |   |   └─Token(StringRightPiece) |}/storageAccounts@2022-09-01'|
//@[85:0086) |   └─Token(RightChevron) |>|
//@[86:0088) ├─Token(NewLine) |\n\n|

@sealed() // this was offered as a completion
//@[00:0128) ├─TypeDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |sealed|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[45:0046) | ├─Token(NewLine) |\n|
type shouldWeBlockThis2 = resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0023) | ├─IdentifierSyntax
//@[05:0023) | | └─Token(Identifier) |shouldWeBlockThis2|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0082) | └─ParameterizedTypeInstantiationSyntax
//@[26:0034) |   ├─IdentifierSyntax
//@[26:0034) |   | └─Token(Identifier) |resource|
//@[34:0035) |   ├─Token(LeftChevron) |<|
//@[35:0081) |   ├─ParameterizedTypeArgumentSyntax
//@[35:0081) |   | └─StringSyntax
//@[35:0081) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[81:0082) |   └─Token(RightChevron) |>|
//@[82:0084) ├─Token(NewLine) |\n\n|

type hello = {
//@[00:0108) ├─TypeDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |type|
//@[05:0010) | ├─IdentifierSyntax
//@[05:0010) | | └─Token(Identifier) |hello|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0108) | └─ObjectTypeSyntax
//@[13:0014) |   ├─Token(LeftBrace) |{|
//@[14:0015) |   ├─Token(NewLine) |\n|
  @discriminator('hi')
//@[02:0091) |   ├─ObjectTypePropertySyntax
//@[02:0022) |   | ├─DecoratorSyntax
//@[02:0003) |   | | ├─Token(At) |@|
//@[03:0022) |   | | └─FunctionCallSyntax
//@[03:0016) |   | |   ├─IdentifierSyntax
//@[03:0016) |   | |   | └─Token(Identifier) |discriminator|
//@[16:0017) |   | |   ├─Token(LeftParen) |(|
//@[17:0021) |   | |   ├─FunctionArgumentSyntax
//@[17:0021) |   | |   | └─StringSyntax
//@[17:0021) |   | |   |   └─Token(StringComplete) |'hi'|
//@[21:0022) |   | |   └─Token(RightParen) |)|
//@[22:0023) |   | ├─Token(NewLine) |\n|
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |bar|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0068) |   | └─ParameterizedTypeInstantiationSyntax
//@[07:0015) |   |   ├─IdentifierSyntax
//@[07:0015) |   |   | └─Token(Identifier) |resource|
//@[15:0016) |   |   ├─Token(LeftChevron) |<|
//@[16:0067) |   |   ├─ParameterizedTypeArgumentSyntax
//@[16:0067) |   |   | └─StringSyntax
//@[16:0067) |   |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[67:0068) |   |   └─Token(RightChevron) |>|
//@[68:0069) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
