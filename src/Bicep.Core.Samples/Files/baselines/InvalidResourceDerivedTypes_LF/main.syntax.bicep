type invalid1 = resource
//@[00:979) ProgramSyntax
//@[00:024) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid1|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:024) | └─ResourceTypeSyntax
//@[16:024) |   ├─Token(Identifier) |resource|
//@[24:024) |   └─SkippedTriviaSyntax
//@[24:026) ├─Token(NewLine) |\n\n|

type invalid2 = resource<>
//@[00:026) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid2|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:026) | └─ParameterizedTypeInstantiationSyntax
//@[16:024) |   ├─IdentifierSyntax
//@[16:024) |   | └─Token(Identifier) |resource|
//@[24:025) |   ├─Token(LeftChevron) |<|
//@[25:026) |   └─Token(RightChevron) |>|
//@[26:028) ├─Token(NewLine) |\n\n|

type invalid3 = resource<'abc', 'def'>
//@[00:038) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid3|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:038) | └─ParameterizedTypeInstantiationSyntax
//@[16:024) |   ├─IdentifierSyntax
//@[16:024) |   | └─Token(Identifier) |resource|
//@[24:025) |   ├─Token(LeftChevron) |<|
//@[25:030) |   ├─ParameterizedTypeArgumentSyntax
//@[25:030) |   | └─StringSyntax
//@[25:030) |   |   └─Token(StringComplete) |'abc'|
//@[30:031) |   ├─Token(Comma) |,|
//@[32:037) |   ├─ParameterizedTypeArgumentSyntax
//@[32:037) |   | └─StringSyntax
//@[32:037) |   |   └─Token(StringComplete) |'def'|
//@[37:038) |   └─Token(RightChevron) |>|
//@[38:039) ├─Token(NewLine) |\n|
type invalid4 = resource<hello>
//@[00:031) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid4|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:031) | └─ParameterizedTypeInstantiationSyntax
//@[16:024) |   ├─IdentifierSyntax
//@[16:024) |   | └─Token(Identifier) |resource|
//@[24:025) |   ├─Token(LeftChevron) |<|
//@[25:030) |   ├─ParameterizedTypeArgumentSyntax
//@[25:030) |   | └─VariableAccessSyntax
//@[25:030) |   |   └─IdentifierSyntax
//@[25:030) |   |     └─Token(Identifier) |hello|
//@[30:031) |   └─Token(RightChevron) |>|
//@[31:032) ├─Token(NewLine) |\n|
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
//@[00:061) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid5|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:061) | └─ParameterizedTypeInstantiationSyntax
//@[16:024) |   ├─IdentifierSyntax
//@[16:024) |   | └─Token(Identifier) |resource|
//@[24:025) |   ├─Token(LeftChevron) |<|
//@[25:060) |   ├─ParameterizedTypeArgumentSyntax
//@[25:060) |   | └─StringSyntax
//@[25:060) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts'|
//@[60:061) |   └─Token(RightChevron) |>|
//@[61:062) ├─Token(NewLine) |\n|
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
//@[00:062) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid6|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:062) | └─ParameterizedTypeInstantiationSyntax
//@[16:024) |   ├─IdentifierSyntax
//@[16:024) |   | └─Token(Identifier) |resource|
//@[24:025) |   ├─Token(LeftChevron) |<|
//@[25:061) |   ├─ParameterizedTypeArgumentSyntax
//@[25:061) |   | └─StringSyntax
//@[25:061) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@'|
//@[61:062) |   └─Token(RightChevron) |>|
//@[62:063) ├─Token(NewLine) |\n|
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
//@[00:067) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid7|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:067) | └─ParameterizedTypeInstantiationSyntax
//@[16:024) |   ├─IdentifierSyntax
//@[16:024) |   | └─Token(Identifier) |resource|
//@[24:025) |   ├─Token(LeftChevron) |<|
//@[25:066) |   ├─ParameterizedTypeArgumentSyntax
//@[25:066) |   | └─StringSyntax
//@[25:066) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@hello'|
//@[66:067) |   └─Token(RightChevron) |>|
//@[67:068) ├─Token(NewLine) |\n|
type invalid8 = resource<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:090) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid8|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:090) | └─ParameterizedTypeInstantiationSyntax
//@[16:024) |   ├─IdentifierSyntax
//@[16:024) |   | └─Token(Identifier) |resource|
//@[24:025) |   ├─Token(LeftChevron) |<|
//@[25:089) |   ├─ParameterizedTypeArgumentSyntax
//@[25:089) |   | └─StringSyntax
//@[25:089) |   |   └─Token(StringComplete) |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[89:090) |   └─Token(RightChevron) |>|
//@[90:091) ├─Token(NewLine) |\n|
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:073) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |invalid9|
//@[14:015) | ├─Token(Assignment) |=|
//@[16:073) | └─ParameterizedTypeInstantiationSyntax
//@[16:024) |   ├─IdentifierSyntax
//@[16:024) |   | └─Token(Identifier) |resource|
//@[24:025) |   ├─Token(LeftChevron) |<|
//@[25:072) |   ├─ParameterizedTypeArgumentSyntax
//@[25:072) |   | └─StringSyntax
//@[25:072) |   |   └─Token(StringComplete) |':Microsoft.Storage/storageAccounts@2022-09-01'|
//@[72:073) |   └─Token(RightChevron) |>|
//@[73:074) ├─Token(NewLine) |\n|
type invalid10 = resource<'abc' 'def'>
//@[00:038) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |invalid10|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:038) | └─ParameterizedTypeInstantiationSyntax
//@[17:025) |   ├─IdentifierSyntax
//@[17:025) |   | └─Token(Identifier) |resource|
//@[25:026) |   ├─Token(LeftChevron) |<|
//@[26:031) |   ├─ParameterizedTypeArgumentSyntax
//@[26:031) |   | └─StringSyntax
//@[26:031) |   |   └─Token(StringComplete) |'abc'|
//@[32:032) |   ├─SkippedTriviaSyntax
//@[32:037) |   ├─ParameterizedTypeArgumentSyntax
//@[32:037) |   | └─StringSyntax
//@[32:037) |   |   └─Token(StringComplete) |'def'|
//@[37:038) |   └─Token(RightChevron) |>|
//@[38:039) ├─Token(NewLine) |\n|
type invalid11 = resource<123>
//@[00:030) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |invalid11|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:030) | └─ParameterizedTypeInstantiationSyntax
//@[17:025) |   ├─IdentifierSyntax
//@[17:025) |   | └─Token(Identifier) |resource|
//@[25:026) |   ├─Token(LeftChevron) |<|
//@[26:029) |   ├─ParameterizedTypeArgumentSyntax
//@[26:029) |   | └─IntegerLiteralSyntax
//@[26:029) |   |   └─Token(Integer) |123|
//@[29:030) |   └─Token(RightChevron) |>|
//@[30:031) ├─Token(NewLine) |\n|
type invalid12 = resource<resourceGroup()>
//@[00:042) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |invalid12|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:042) | └─ParameterizedTypeInstantiationSyntax
//@[17:025) |   ├─IdentifierSyntax
//@[17:025) |   | └─Token(Identifier) |resource|
//@[25:026) |   ├─Token(LeftChevron) |<|
//@[26:039) |   ├─ParameterizedTypeArgumentSyntax
//@[26:039) |   | └─VariableAccessSyntax
//@[26:039) |   |   └─IdentifierSyntax
//@[26:039) |   |     └─Token(Identifier) |resourceGroup|
//@[39:039) |   ├─SkippedTriviaSyntax
//@[39:041) |   ├─ParameterizedTypeArgumentSyntax
//@[39:041) |   | └─ParenthesizedExpressionSyntax
//@[39:040) |   |   ├─Token(LeftParen) |(|
//@[40:040) |   |   ├─SkippedTriviaSyntax
//@[40:041) |   |   └─Token(RightParen) |)|
//@[41:042) |   └─Token(RightChevron) |>|
//@[42:044) ├─Token(NewLine) |\n\n|

type thisIsWeird = resource</*
//@[00:093) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:016) | ├─IdentifierSyntax
//@[05:016) | | └─Token(Identifier) |thisIsWeird|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:093) | └─ParameterizedTypeInstantiationSyntax
//@[19:027) |   ├─IdentifierSyntax
//@[19:027) |   | └─Token(Identifier) |resource|
//@[27:028) |   ├─Token(LeftChevron) |<|
*/'Astronomer.Astro/organizations@2023-08-01-preview'
//@[02:053) |   ├─ParameterizedTypeArgumentSyntax
//@[02:053) |   | └─StringSyntax
//@[02:053) |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[53:053) |   ├─SkippedTriviaSyntax
//@[53:054) |   ├─Token(NewLine) |\n|
///  >
//@[06:007) |   ├─Token(NewLine) |\n|
>
//@[00:001) |   └─Token(RightChevron) |>|
//@[01:003) ├─Token(NewLine) |\n\n|

type interpolated = resource<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[00:081) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:017) | ├─IdentifierSyntax
//@[05:017) | | └─Token(Identifier) |interpolated|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:081) | └─ParameterizedTypeInstantiationSyntax
//@[20:028) |   ├─IdentifierSyntax
//@[20:028) |   | └─Token(Identifier) |resource|
//@[28:029) |   ├─Token(LeftChevron) |<|
//@[29:080) |   ├─ParameterizedTypeArgumentSyntax
//@[29:080) |   | └─StringSyntax
//@[29:042) |   |   ├─Token(StringLeftPiece) |'Microsoft.${|
//@[42:051) |   |   ├─StringSyntax
//@[42:051) |   |   | └─Token(StringComplete) |'Storage'|
//@[51:080) |   |   └─Token(StringRightPiece) |}/storageAccounts@2022-09-01'|
//@[80:081) |   └─Token(RightChevron) |>|
//@[81:083) ├─Token(NewLine) |\n\n|

@sealed()
//@[00:093) ├─TypeDeclarationSyntax
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |sealed|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable = resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:004) | ├─Token(Identifier) |type|
//@[05:024) | ├─IdentifierSyntax
//@[05:024) | | └─Token(Identifier) |shouldNotBeSealable|
//@[25:026) | ├─Token(Assignment) |=|
//@[27:083) | └─ParameterizedTypeInstantiationSyntax
//@[27:035) |   ├─IdentifierSyntax
//@[27:035) |   | └─Token(Identifier) |resource|
//@[35:036) |   ├─Token(LeftChevron) |<|
//@[36:082) |   ├─ParameterizedTypeArgumentSyntax
//@[36:082) |   | └─StringSyntax
//@[36:082) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[82:083) |   └─Token(RightChevron) |>|
//@[83:085) ├─Token(NewLine) |\n\n|

type hello = {
//@[00:108) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:010) | ├─IdentifierSyntax
//@[05:010) | | └─Token(Identifier) |hello|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:108) | └─ObjectTypeSyntax
//@[13:014) |   ├─Token(LeftBrace) |{|
//@[14:015) |   ├─Token(NewLine) |\n|
  @discriminator('hi')
//@[02:091) |   ├─ObjectTypePropertySyntax
//@[02:022) |   | ├─DecoratorSyntax
//@[02:003) |   | | ├─Token(At) |@|
//@[03:022) |   | | └─FunctionCallSyntax
//@[03:016) |   | |   ├─IdentifierSyntax
//@[03:016) |   | |   | └─Token(Identifier) |discriminator|
//@[16:017) |   | |   ├─Token(LeftParen) |(|
//@[17:021) |   | |   ├─FunctionArgumentSyntax
//@[17:021) |   | |   | └─StringSyntax
//@[17:021) |   | |   |   └─Token(StringComplete) |'hi'|
//@[21:022) |   | |   └─Token(RightParen) |)|
//@[22:023) |   | ├─Token(NewLine) |\n|
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |bar|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:068) |   | └─ParameterizedTypeInstantiationSyntax
//@[07:015) |   |   ├─IdentifierSyntax
//@[07:015) |   |   | └─Token(Identifier) |resource|
//@[15:016) |   |   ├─Token(LeftChevron) |<|
//@[16:067) |   |   ├─ParameterizedTypeArgumentSyntax
//@[16:067) |   |   | └─StringSyntax
//@[16:067) |   |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[67:068) |   |   └─Token(RightChevron) |>|
//@[68:069) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
