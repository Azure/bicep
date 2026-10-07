type invalid1 = resourceInput
//@[000:3258) ProgramSyntax
//@[000:0029) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid1|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0029) | └─TypeVariableAccessSyntax
//@[016:0029) |   └─IdentifierSyntax
//@[016:0029) |     └─Token(Identifier) |resourceInput|
//@[029:0031) ├─Token(NewLine) |\n\n|

type invalid2 = resourceInput<>
//@[000:0031) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid2|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0031) | └─ParameterizedTypeInstantiationSyntax
//@[016:0029) |   ├─IdentifierSyntax
//@[016:0029) |   | └─Token(Identifier) |resourceInput|
//@[029:0030) |   ├─Token(LeftChevron) |<|
//@[030:0031) |   └─Token(RightChevron) |>|
//@[031:0033) ├─Token(NewLine) |\n\n|

type invalid3 = resourceInput<'abc', 'def'>
//@[000:0043) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid3|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0043) | └─ParameterizedTypeInstantiationSyntax
//@[016:0029) |   ├─IdentifierSyntax
//@[016:0029) |   | └─Token(Identifier) |resourceInput|
//@[029:0030) |   ├─Token(LeftChevron) |<|
//@[030:0035) |   ├─ParameterizedTypeArgumentSyntax
//@[030:0035) |   | └─StringTypeLiteralSyntax
//@[030:0035) |   |   └─Token(StringComplete) |'abc'|
//@[035:0036) |   ├─Token(Comma) |,|
//@[037:0042) |   ├─ParameterizedTypeArgumentSyntax
//@[037:0042) |   | └─StringTypeLiteralSyntax
//@[037:0042) |   |   └─Token(StringComplete) |'def'|
//@[042:0043) |   └─Token(RightChevron) |>|
//@[043:0044) ├─Token(NewLine) |\n|
type invalid4 = resourceInput<hello>
//@[000:0036) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid4|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0036) | └─ParameterizedTypeInstantiationSyntax
//@[016:0029) |   ├─IdentifierSyntax
//@[016:0029) |   | └─Token(Identifier) |resourceInput|
//@[029:0030) |   ├─Token(LeftChevron) |<|
//@[030:0035) |   ├─ParameterizedTypeArgumentSyntax
//@[030:0035) |   | └─TypeVariableAccessSyntax
//@[030:0035) |   |   └─IdentifierSyntax
//@[030:0035) |   |     └─Token(Identifier) |hello|
//@[035:0036) |   └─Token(RightChevron) |>|
//@[036:0037) ├─Token(NewLine) |\n|
type invalid5 = resourceInput<'Microsoft.Storage/storageAccounts'>
//@[000:0066) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid5|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0066) | └─ParameterizedTypeInstantiationSyntax
//@[016:0029) |   ├─IdentifierSyntax
//@[016:0029) |   | └─Token(Identifier) |resourceInput|
//@[029:0030) |   ├─Token(LeftChevron) |<|
//@[030:0065) |   ├─ParameterizedTypeArgumentSyntax
//@[030:0065) |   | └─StringTypeLiteralSyntax
//@[030:0065) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts'|
//@[065:0066) |   └─Token(RightChevron) |>|
//@[066:0067) ├─Token(NewLine) |\n|
type invalid6 = resourceInput<'Microsoft.Storage/storageAccounts@'>
//@[000:0067) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid6|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0067) | └─ParameterizedTypeInstantiationSyntax
//@[016:0029) |   ├─IdentifierSyntax
//@[016:0029) |   | └─Token(Identifier) |resourceInput|
//@[029:0030) |   ├─Token(LeftChevron) |<|
//@[030:0066) |   ├─ParameterizedTypeArgumentSyntax
//@[030:0066) |   | └─StringTypeLiteralSyntax
//@[030:0066) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@'|
//@[066:0067) |   └─Token(RightChevron) |>|
//@[067:0068) ├─Token(NewLine) |\n|
type invalid7 = resourceInput<'Microsoft.Storage/storageAccounts@hello'>
//@[000:0072) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid7|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0072) | └─ParameterizedTypeInstantiationSyntax
//@[016:0029) |   ├─IdentifierSyntax
//@[016:0029) |   | └─Token(Identifier) |resourceInput|
//@[029:0030) |   ├─Token(LeftChevron) |<|
//@[030:0071) |   ├─ParameterizedTypeArgumentSyntax
//@[030:0071) |   | └─StringTypeLiteralSyntax
//@[030:0071) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@hello'|
//@[071:0072) |   └─Token(RightChevron) |>|
//@[072:0073) ├─Token(NewLine) |\n|
type invalid8 = resourceInput<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:0095) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid8|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0095) | └─ParameterizedTypeInstantiationSyntax
//@[016:0029) |   ├─IdentifierSyntax
//@[016:0029) |   | └─Token(Identifier) |resourceInput|
//@[029:0030) |   ├─Token(LeftChevron) |<|
//@[030:0094) |   ├─ParameterizedTypeArgumentSyntax
//@[030:0094) |   | └─StringTypeLiteralSyntax
//@[030:0094) |   |   └─Token(StringComplete) |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[094:0095) |   └─Token(RightChevron) |>|
//@[095:0096) ├─Token(NewLine) |\n|
type invalid9 = resourceInput<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:0078) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |invalid9|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0078) | └─ParameterizedTypeInstantiationSyntax
//@[016:0029) |   ├─IdentifierSyntax
//@[016:0029) |   | └─Token(Identifier) |resourceInput|
//@[029:0030) |   ├─Token(LeftChevron) |<|
//@[030:0077) |   ├─ParameterizedTypeArgumentSyntax
//@[030:0077) |   | └─StringTypeLiteralSyntax
//@[030:0077) |   |   └─Token(StringComplete) |':Microsoft.Storage/storageAccounts@2022-09-01'|
//@[077:0078) |   └─Token(RightChevron) |>|
//@[078:0079) ├─Token(NewLine) |\n|
type invalid10 = resourceInput<'abc' 'def'>
//@[000:0043) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |invalid10|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0043) | └─ParameterizedTypeInstantiationSyntax
//@[017:0030) |   ├─IdentifierSyntax
//@[017:0030) |   | └─Token(Identifier) |resourceInput|
//@[030:0031) |   ├─Token(LeftChevron) |<|
//@[031:0036) |   ├─ParameterizedTypeArgumentSyntax
//@[031:0036) |   | └─StringTypeLiteralSyntax
//@[031:0036) |   |   └─Token(StringComplete) |'abc'|
//@[037:0037) |   ├─SkippedTriviaSyntax
//@[037:0042) |   ├─ParameterizedTypeArgumentSyntax
//@[037:0042) |   | └─StringTypeLiteralSyntax
//@[037:0042) |   |   └─Token(StringComplete) |'def'|
//@[042:0043) |   └─Token(RightChevron) |>|
//@[043:0044) ├─Token(NewLine) |\n|
type invalid11 = resourceInput<123>
//@[000:0035) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |invalid11|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0035) | └─ParameterizedTypeInstantiationSyntax
//@[017:0030) |   ├─IdentifierSyntax
//@[017:0030) |   | └─Token(Identifier) |resourceInput|
//@[030:0031) |   ├─Token(LeftChevron) |<|
//@[031:0034) |   ├─ParameterizedTypeArgumentSyntax
//@[031:0034) |   | └─IntegerTypeLiteralSyntax
//@[031:0034) |   |   └─Token(Integer) |123|
//@[034:0035) |   └─Token(RightChevron) |>|
//@[035:0036) ├─Token(NewLine) |\n|
type invalid12 = resourceInput<resourceGroup()>
//@[000:0047) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |invalid12|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0047) | └─ParameterizedTypeInstantiationSyntax
//@[017:0030) |   ├─IdentifierSyntax
//@[017:0030) |   | └─Token(Identifier) |resourceInput|
//@[030:0031) |   ├─Token(LeftChevron) |<|
//@[031:0044) |   ├─ParameterizedTypeArgumentSyntax
//@[031:0044) |   | └─TypeVariableAccessSyntax
//@[031:0044) |   |   └─IdentifierSyntax
//@[031:0044) |   |     └─Token(Identifier) |resourceGroup|
//@[044:0044) |   ├─SkippedTriviaSyntax
//@[044:0046) |   ├─ParameterizedTypeArgumentSyntax
//@[044:0046) |   | └─ParenthesizedTypeSyntax
//@[044:0045) |   |   ├─Token(LeftParen) |(|
//@[045:0045) |   |   ├─SkippedTriviaSyntax
//@[045:0046) |   |   └─Token(RightParen) |)|
//@[046:0047) |   └─Token(RightChevron) |>|
//@[047:0049) ├─Token(NewLine) |\n\n|

type thisIsWeird = resourceInput</*
//@[000:0098) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0016) | ├─IdentifierSyntax
//@[005:0016) | | └─Token(Identifier) |thisIsWeird|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0098) | └─ParameterizedTypeInstantiationSyntax
//@[019:0032) |   ├─IdentifierSyntax
//@[019:0032) |   | └─Token(Identifier) |resourceInput|
//@[032:0033) |   ├─Token(LeftChevron) |<|
*/'Astronomer.Astro/organizations@2023-08-01-preview'
//@[002:0053) |   ├─ParameterizedTypeArgumentSyntax
//@[002:0053) |   | └─StringTypeLiteralSyntax
//@[002:0053) |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[053:0054) |   ├─Token(NewLine) |\n|
///  >
//@[006:0007) |   ├─Token(NewLine) |\n|
>
//@[000:0001) |   └─Token(RightChevron) |>|
//@[001:0003) ├─Token(NewLine) |\n\n|

type interpolated = resourceInput<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[000:0086) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0017) | ├─IdentifierSyntax
//@[005:0017) | | └─Token(Identifier) |interpolated|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0086) | └─ParameterizedTypeInstantiationSyntax
//@[020:0033) |   ├─IdentifierSyntax
//@[020:0033) |   | └─Token(Identifier) |resourceInput|
//@[033:0034) |   ├─Token(LeftChevron) |<|
//@[034:0085) |   ├─ParameterizedTypeArgumentSyntax
//@[034:0085) |   | └─StringTypeLiteralSyntax
//@[034:0047) |   |   ├─Token(StringLeftPiece) |'Microsoft.${|
//@[047:0056) |   |   ├─StringSyntax
//@[047:0056) |   |   | └─Token(StringComplete) |'Storage'|
//@[056:0085) |   |   └─Token(StringRightPiece) |}/storageAccounts@2022-09-01'|
//@[085:0086) |   └─Token(RightChevron) |>|
//@[086:0088) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0098) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |shouldNotBeSealable|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0088) | └─ParameterizedTypeInstantiationSyntax
//@[027:0040) |   ├─IdentifierSyntax
//@[027:0040) |   | └─Token(Identifier) |resourceInput|
//@[040:0041) |   ├─Token(LeftChevron) |<|
//@[041:0087) |   ├─ParameterizedTypeArgumentSyntax
//@[041:0087) |   | └─StringTypeLiteralSyntax
//@[041:0087) |   |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[087:0088) |   └─Token(RightChevron) |>|
//@[088:0090) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0110) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable2 = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |shouldNotBeSealable2|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0100) | └─TypePropertyAccessSyntax
//@[028:0089) |   ├─ParameterizedTypeInstantiationSyntax
//@[028:0041) |   | ├─IdentifierSyntax
//@[028:0041) |   | | └─Token(Identifier) |resourceInput|
//@[041:0042) |   | ├─Token(LeftChevron) |<|
//@[042:0088) |   | ├─ParameterizedTypeArgumentSyntax
//@[042:0088) |   | | └─StringTypeLiteralSyntax
//@[042:0088) |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[088:0089) |   | └─Token(RightChevron) |>|
//@[089:0090) |   ├─Token(Dot) |.|
//@[090:0100) |   └─IdentifierSyntax
//@[090:0100) |     └─Token(Identifier) |properties|
//@[100:0102) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0112) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable3 = resourceOutput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties?
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |shouldNotBeSealable3|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0102) | └─NullableTypeSyntax
//@[028:0101) |   ├─TypePropertyAccessSyntax
//@[028:0090) |   | ├─ParameterizedTypeInstantiationSyntax
//@[028:0042) |   | | ├─IdentifierSyntax
//@[028:0042) |   | | | └─Token(Identifier) |resourceOutput|
//@[042:0043) |   | | ├─Token(LeftChevron) |<|
//@[043:0089) |   | | ├─ParameterizedTypeArgumentSyntax
//@[043:0089) |   | | | └─StringTypeLiteralSyntax
//@[043:0089) |   | | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[089:0090) |   | | └─Token(RightChevron) |>|
//@[090:0091) |   | ├─Token(Dot) |.|
//@[091:0101) |   | └─IdentifierSyntax
//@[091:0101) |   |   └─Token(Identifier) |properties|
//@[101:0102) |   └─Token(Question) |?|
//@[102:0104) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0124) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable4 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |shouldNotBeSealable4|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0114) | └─TypeAdditionalPropertiesAccessSyntax
//@[028:0112) |   ├─TypePropertyAccessSyntax
//@[028:0091) |   | ├─TypePropertyAccessSyntax
//@[028:0080) |   | | ├─ParameterizedTypeInstantiationSyntax
//@[028:0041) |   | | | ├─IdentifierSyntax
//@[028:0041) |   | | | | └─Token(Identifier) |resourceInput|
//@[041:0042) |   | | | ├─Token(LeftChevron) |<|
//@[042:0079) |   | | | ├─ParameterizedTypeArgumentSyntax
//@[042:0079) |   | | | | └─StringTypeLiteralSyntax
//@[042:0079) |   | | | |   └─Token(StringComplete) |'Microsoft.Web/customApis@2016-06-01'|
//@[079:0080) |   | | | └─Token(RightChevron) |>|
//@[080:0081) |   | | ├─Token(Dot) |.|
//@[081:0091) |   | | └─IdentifierSyntax
//@[081:0091) |   | |   └─Token(Identifier) |properties|
//@[091:0092) |   | ├─Token(Dot) |.|
//@[092:0112) |   | └─IdentifierSyntax
//@[092:0112) |   |   └─Token(Identifier) |connectionParameters|
//@[112:0113) |   ├─Token(Dot) |.|
//@[113:0114) |   └─Token(Asterisk) |*|
//@[114:0116) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0058) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable5 = shouldNotBeSealable2
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |shouldNotBeSealable5|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0048) | └─TypeVariableAccessSyntax
//@[028:0048) |   └─IdentifierSyntax
//@[028:0048) |     └─Token(Identifier) |shouldNotBeSealable2|
//@[048:0050) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0109) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
param shouldNotBeSealable6 resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |shouldNotBeSealable6|
//@[027:0099) | └─TypePropertyAccessSyntax
//@[027:0088) |   ├─ParameterizedTypeInstantiationSyntax
//@[027:0040) |   | ├─IdentifierSyntax
//@[027:0040) |   | | └─Token(Identifier) |resourceInput|
//@[040:0041) |   | ├─Token(LeftChevron) |<|
//@[041:0087) |   | ├─ParameterizedTypeArgumentSyntax
//@[041:0087) |   | | └─StringTypeLiteralSyntax
//@[041:0087) |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[087:0088) |   | └─Token(RightChevron) |>|
//@[088:0089) |   ├─Token(Dot) |.|
//@[089:0099) |   └─IdentifierSyntax
//@[089:0099) |     └─Token(Identifier) |properties|
//@[099:0101) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0057) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
param shouldNotBeSealable7 shouldNotBeSealable2
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0026) | ├─IdentifierSyntax
//@[006:0026) | | └─Token(Identifier) |shouldNotBeSealable7|
//@[027:0047) | └─TypeVariableAccessSyntax
//@[027:0047) |   └─IdentifierSyntax
//@[027:0047) |     └─Token(Identifier) |shouldNotBeSealable2|
//@[047:0049) ├─Token(NewLine) |\n\n|

type shouldNotBeSealable8 = {
//@[000:0124) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |shouldNotBeSealable8|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0124) | └─ObjectTypeSyntax
//@[028:0029) |   ├─Token(LeftBrace) |{|
//@[029:0030) |   ├─Token(NewLine) |\n|
  @sealed()
//@[002:0092) |   ├─ObjectTypePropertySyntax
//@[002:0011) |   | ├─DecoratorSyntax
//@[002:0003) |   | | ├─Token(At) |@|
//@[003:0011) |   | | └─FunctionCallSyntax
//@[003:0009) |   | |   ├─IdentifierSyntax
//@[003:0009) |   | |   | └─Token(Identifier) |sealed|
//@[009:0010) |   | |   ├─Token(LeftParen) |(|
//@[010:0011) |   | |   └─Token(RightParen) |)|
//@[011:0012) |   | ├─Token(NewLine) |\n|
  prop: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |prop|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0080) |   | └─TypePropertyAccessSyntax
//@[008:0069) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[008:0021) |   |   | ├─IdentifierSyntax
//@[008:0021) |   |   | | └─Token(Identifier) |resourceInput|
//@[021:0022) |   |   | ├─Token(LeftChevron) |<|
//@[022:0068) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[022:0068) |   |   | | └─StringTypeLiteralSyntax
//@[022:0068) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[068:0069) |   |   | └─Token(RightChevron) |>|
//@[069:0070) |   |   ├─Token(Dot) |.|
//@[070:0080) |   |   └─IdentifierSyntax
//@[070:0080) |   |     └─Token(Identifier) |properties|
//@[080:0081) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type containsResourceDerivedTypes = {
//@[000:0374) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0033) | ├─IdentifierSyntax
//@[005:0033) | | └─Token(Identifier) |containsResourceDerivedTypes|
//@[034:0035) | ├─Token(Assignment) |=|
//@[036:0374) | └─ObjectTypeSyntax
//@[036:0037) |   ├─Token(LeftBrace) |{|
//@[037:0038) |   ├─Token(NewLine) |\n|
  prop: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
//@[002:0080) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |prop|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0080) |   | └─TypePropertyAccessSyntax
//@[008:0069) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[008:0021) |   |   | ├─IdentifierSyntax
//@[008:0021) |   |   | | └─Token(Identifier) |resourceInput|
//@[021:0022) |   |   | ├─Token(LeftChevron) |<|
//@[022:0068) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[022:0068) |   |   | | └─StringTypeLiteralSyntax
//@[022:0068) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[068:0069) |   |   | └─Token(RightChevron) |>|
//@[069:0070) |   |   ├─Token(Dot) |.|
//@[070:0080) |   |   └─IdentifierSyntax
//@[070:0080) |   |     └─Token(Identifier) |properties|
//@[080:0081) |   ├─Token(NewLine) |\n|
  tuple: [string, resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties]
//@[002:0091) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |tuple|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0091) |   | └─TupleTypeSyntax
//@[009:0010) |   |   ├─Token(LeftSquare) |[|
//@[010:0016) |   |   ├─TupleTypeItemSyntax
//@[010:0016) |   |   | └─TypeVariableAccessSyntax
//@[010:0016) |   |   |   └─IdentifierSyntax
//@[010:0016) |   |   |     └─Token(Identifier) |string|
//@[016:0017) |   |   ├─Token(Comma) |,|
//@[018:0090) |   |   ├─TupleTypeItemSyntax
//@[018:0090) |   |   | └─TypePropertyAccessSyntax
//@[018:0079) |   |   |   ├─ParameterizedTypeInstantiationSyntax
//@[018:0031) |   |   |   | ├─IdentifierSyntax
//@[018:0031) |   |   |   | | └─Token(Identifier) |resourceInput|
//@[031:0032) |   |   |   | ├─Token(LeftChevron) |<|
//@[032:0078) |   |   |   | ├─ParameterizedTypeArgumentSyntax
//@[032:0078) |   |   |   | | └─StringTypeLiteralSyntax
//@[032:0078) |   |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[078:0079) |   |   |   | └─Token(RightChevron) |>|
//@[079:0080) |   |   |   ├─Token(Dot) |.|
//@[080:0090) |   |   |   └─IdentifierSyntax
//@[080:0090) |   |   |     └─Token(Identifier) |properties|
//@[090:0091) |   |   └─Token(RightSquare) |]|
//@[091:0092) |   ├─Token(NewLine) |\n|
  array: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties[]
//@[002:0083) |   ├─ObjectTypePropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |array|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0083) |   | └─ArrayTypeSyntax
//@[009:0081) |   |   ├─ArrayTypeMemberSyntax
//@[009:0081) |   |   | └─TypePropertyAccessSyntax
//@[009:0070) |   |   |   ├─ParameterizedTypeInstantiationSyntax
//@[009:0022) |   |   |   | ├─IdentifierSyntax
//@[009:0022) |   |   |   | | └─Token(Identifier) |resourceInput|
//@[022:0023) |   |   |   | ├─Token(LeftChevron) |<|
//@[023:0069) |   |   |   | ├─ParameterizedTypeArgumentSyntax
//@[023:0069) |   |   |   | | └─StringTypeLiteralSyntax
//@[023:0069) |   |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[069:0070) |   |   |   | └─Token(RightChevron) |>|
//@[070:0071) |   |   |   ├─Token(Dot) |.|
//@[071:0081) |   |   |   └─IdentifierSyntax
//@[071:0081) |   |   |     └─Token(Identifier) |properties|
//@[081:0082) |   |   ├─Token(LeftSquare) |[|
//@[082:0083) |   |   └─Token(RightSquare) |]|
//@[083:0084) |   ├─Token(NewLine) |\n|
  *: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.properties
//@[002:0077) |   ├─ObjectTypeAdditionalPropertiesSyntax
//@[002:0003) |   | ├─Token(Asterisk) |*|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0077) |   | └─TypePropertyAccessSyntax
//@[005:0066) |   |   ├─ParameterizedTypeInstantiationSyntax
//@[005:0018) |   |   | ├─IdentifierSyntax
//@[005:0018) |   |   | | └─Token(Identifier) |resourceInput|
//@[018:0019) |   |   | ├─Token(LeftChevron) |<|
//@[019:0065) |   |   | ├─ParameterizedTypeArgumentSyntax
//@[019:0065) |   |   | | └─StringTypeLiteralSyntax
//@[019:0065) |   |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[065:0066) |   |   | └─Token(RightChevron) |>|
//@[066:0067) |   |   ├─Token(Dot) |.|
//@[067:0077) |   |   └─IdentifierSyntax
//@[067:0077) |   |     └─Token(Identifier) |properties|
//@[077:0078) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0071) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable9 = containsResourceDerivedTypes.prop
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0025) | ├─IdentifierSyntax
//@[005:0025) | | └─Token(Identifier) |shouldNotBeSealable9|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0061) | └─TypePropertyAccessSyntax
//@[028:0056) |   ├─TypeVariableAccessSyntax
//@[028:0056) |   | └─IdentifierSyntax
//@[028:0056) |   |   └─Token(Identifier) |containsResourceDerivedTypes|
//@[056:0057) |   ├─Token(Dot) |.|
//@[057:0061) |   └─IdentifierSyntax
//@[057:0061) |     └─Token(Identifier) |prop|
//@[061:0063) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0076) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable10 = containsResourceDerivedTypes.tuple[1]
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0026) | ├─IdentifierSyntax
//@[005:0026) | | └─Token(Identifier) |shouldNotBeSealable10|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0066) | └─TypeArrayAccessSyntax
//@[029:0063) |   ├─TypePropertyAccessSyntax
//@[029:0057) |   | ├─TypeVariableAccessSyntax
//@[029:0057) |   | | └─IdentifierSyntax
//@[029:0057) |   | |   └─Token(Identifier) |containsResourceDerivedTypes|
//@[057:0058) |   | ├─Token(Dot) |.|
//@[058:0063) |   | └─IdentifierSyntax
//@[058:0063) |   |   └─Token(Identifier) |tuple|
//@[063:0064) |   ├─Token(LeftSquare) |[|
//@[064:0065) |   ├─IntegerLiteralSyntax
//@[064:0065) |   | └─Token(Integer) |1|
//@[065:0066) |   └─Token(RightSquare) |]|
//@[066:0068) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0076) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable11 = containsResourceDerivedTypes.array[*]
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0026) | ├─IdentifierSyntax
//@[005:0026) | | └─Token(Identifier) |shouldNotBeSealable11|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0066) | └─TypeItemsAccessSyntax
//@[029:0063) |   ├─TypePropertyAccessSyntax
//@[029:0057) |   | ├─TypeVariableAccessSyntax
//@[029:0057) |   | | └─IdentifierSyntax
//@[029:0057) |   | |   └─Token(Identifier) |containsResourceDerivedTypes|
//@[057:0058) |   | ├─Token(Dot) |.|
//@[058:0063) |   | └─IdentifierSyntax
//@[058:0063) |   |   └─Token(Identifier) |array|
//@[063:0064) |   ├─Token(LeftSquare) |[|
//@[064:0065) |   ├─Token(Asterisk) |*|
//@[065:0066) |   └─Token(RightSquare) |]|
//@[066:0068) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0069) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable12 = containsResourceDerivedTypes.*
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0026) | ├─IdentifierSyntax
//@[005:0026) | | └─Token(Identifier) |shouldNotBeSealable12|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0059) | └─TypeAdditionalPropertiesAccessSyntax
//@[029:0057) |   ├─TypeVariableAccessSyntax
//@[029:0057) |   | └─IdentifierSyntax
//@[029:0057) |   |   └─Token(Identifier) |containsResourceDerivedTypes|
//@[057:0058) |   ├─Token(Dot) |.|
//@[058:0059) |   └─Token(Asterisk) |*|
//@[059:0061) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0075) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type shouldNotBeSealable13 = containsResourceDerivedTypes['prop']
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0026) | ├─IdentifierSyntax
//@[005:0026) | | └─Token(Identifier) |shouldNotBeSealable13|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0065) | └─TypeArrayAccessSyntax
//@[029:0057) |   ├─TypeVariableAccessSyntax
//@[029:0057) |   | └─IdentifierSyntax
//@[029:0057) |   |   └─Token(Identifier) |containsResourceDerivedTypes|
//@[057:0058) |   ├─Token(LeftSquare) |[|
//@[058:0064) |   ├─StringSyntax
//@[058:0064) |   | └─Token(StringComplete) |'prop'|
//@[064:0065) |   └─Token(RightSquare) |]|
//@[065:0067) ├─Token(NewLine) |\n\n|

@sealed()
//@[000:0088) ├─TypeDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |sealed|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
type sealableUserDefinedObject = {
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0030) | ├─IdentifierSyntax
//@[005:0030) | | └─Token(Identifier) |sealableUserDefinedObject|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0078) | └─ObjectTypeSyntax
//@[033:0034) |   ├─Token(LeftBrace) |{|
//@[034:0035) |   ├─Token(NewLine) |\n|
  prop: containsResourceDerivedTypes.prop
//@[002:0041) |   ├─ObjectTypePropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |prop|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0041) |   | └─TypePropertyAccessSyntax
//@[008:0036) |   |   ├─TypeVariableAccessSyntax
//@[008:0036) |   |   | └─IdentifierSyntax
//@[008:0036) |   |   |   └─Token(Identifier) |containsResourceDerivedTypes|
//@[036:0037) |   |   ├─Token(Dot) |.|
//@[037:0041) |   |   └─IdentifierSyntax
//@[037:0041) |   |     └─Token(Identifier) |prop|
//@[041:0042) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type hello = {
//@[000:0113) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |hello|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0113) | └─ObjectTypeSyntax
//@[013:0014) |   ├─Token(LeftBrace) |{|
//@[014:0015) |   ├─Token(NewLine) |\n|
  @discriminator('hi')
//@[002:0096) |   ├─ObjectTypePropertySyntax
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
  bar: resourceInput<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[002:0005) |   | ├─IdentifierSyntax
//@[002:0005) |   | | └─Token(Identifier) |bar|
//@[005:0006) |   | ├─Token(Colon) |:|
//@[007:0073) |   | └─ParameterizedTypeInstantiationSyntax
//@[007:0020) |   |   ├─IdentifierSyntax
//@[007:0020) |   |   | └─Token(Identifier) |resourceInput|
//@[020:0021) |   |   ├─Token(LeftChevron) |<|
//@[021:0072) |   |   ├─ParameterizedTypeArgumentSyntax
//@[021:0072) |   |   | └─StringTypeLiteralSyntax
//@[021:0072) |   |   |   └─Token(StringComplete) |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[072:0073) |   |   └─Token(RightChevron) |>|
//@[073:0074) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

type typoInPropertyName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
//@[000:0091) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0023) | ├─IdentifierSyntax
//@[005:0023) | | └─Token(Identifier) |typoInPropertyName|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0091) | └─TypePropertyAccessSyntax
//@[026:0087) |   ├─ParameterizedTypeInstantiationSyntax
//@[026:0039) |   | ├─IdentifierSyntax
//@[026:0039) |   | | └─Token(Identifier) |resourceInput|
//@[039:0040) |   | ├─Token(LeftChevron) |<|
//@[040:0086) |   | ├─ParameterizedTypeArgumentSyntax
//@[040:0086) |   | | └─StringTypeLiteralSyntax
//@[040:0086) |   | |   └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[086:0087) |   | └─Token(RightChevron) |>|
//@[087:0088) |   ├─Token(Dot) |.|
//@[088:0091) |   └─IdentifierSyntax
//@[088:0091) |     └─Token(Identifier) |nom|
//@[091:0092) ├─Token(NewLine) |\n|
type typoInPropertyName2 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
//@[000:0117) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |typoInPropertyName2|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0117) | └─TypePropertyAccessSyntax
//@[027:0109) |   ├─TypeItemsAccessSyntax
//@[027:0106) |   | ├─TypePropertyAccessSyntax
//@[027:0091) |   | | ├─TypePropertyAccessSyntax
//@[027:0080) |   | | | ├─ParameterizedTypeInstantiationSyntax
//@[027:0040) |   | | | | ├─IdentifierSyntax
//@[027:0040) |   | | | | | └─Token(Identifier) |resourceInput|
//@[040:0041) |   | | | | ├─Token(LeftChevron) |<|
//@[041:0079) |   | | | | ├─ParameterizedTypeArgumentSyntax
//@[041:0079) |   | | | | | └─StringTypeLiteralSyntax
//@[041:0079) |   | | | | |   └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[079:0080) |   | | | | └─Token(RightChevron) |>|
//@[080:0081) |   | | | ├─Token(Dot) |.|
//@[081:0091) |   | | | └─IdentifierSyntax
//@[081:0091) |   | | |   └─Token(Identifier) |properties|
//@[091:0092) |   | | ├─Token(Dot) |.|
//@[092:0106) |   | | └─IdentifierSyntax
//@[092:0106) |   | |   └─Token(Identifier) |accessPolicies|
//@[106:0107) |   | ├─Token(LeftSquare) |[|
//@[107:0108) |   | ├─Token(Asterisk) |*|
//@[108:0109) |   | └─Token(RightSquare) |]|
//@[109:0110) |   ├─Token(Dot) |.|
//@[110:0117) |   └─IdentifierSyntax
//@[110:0117) |     └─Token(Identifier) |tenatId|
//@[117:0118) ├─Token(NewLine) |\n|
type typoInPropertyName3 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
//@[000:0118) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |typoInPropertyName3|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0118) | └─TypePropertyAccessSyntax
//@[027:0109) |   ├─TypePropertyAccessSyntax
//@[027:0094) |   | ├─TypeItemsAccessSyntax
//@[027:0091) |   | | ├─TypePropertyAccessSyntax
//@[027:0080) |   | | | ├─ParameterizedTypeInstantiationSyntax
//@[027:0040) |   | | | | ├─IdentifierSyntax
//@[027:0040) |   | | | | | └─Token(Identifier) |resourceInput|
//@[040:0041) |   | | | | ├─Token(LeftChevron) |<|
//@[041:0079) |   | | | | ├─ParameterizedTypeArgumentSyntax
//@[041:0079) |   | | | | | └─StringTypeLiteralSyntax
//@[041:0079) |   | | | | |   └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[079:0080) |   | | | | └─Token(RightChevron) |>|
//@[080:0081) |   | | | ├─Token(Dot) |.|
//@[081:0091) |   | | | └─IdentifierSyntax
//@[081:0091) |   | | |   └─Token(Identifier) |properties|
//@[091:0092) |   | | ├─Token(LeftSquare) |[|
//@[092:0093) |   | | ├─Token(Asterisk) |*|
//@[093:0094) |   | | └─Token(RightSquare) |]|
//@[094:0095) |   | ├─Token(Dot) |.|
//@[095:0109) |   | └─IdentifierSyntax
//@[095:0109) |   |   └─Token(Identifier) |accessPolicies|
//@[109:0110) |   ├─Token(Dot) |.|
//@[110:0118) |   └─IdentifierSyntax
//@[110:0118) |     └─Token(Identifier) |tenantId|
//@[118:0119) ├─Token(NewLine) |\n|
type typoInPropertyName4 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
//@[000:0119) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |typoInPropertyName4|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0119) | └─TypePropertyAccessSyntax
//@[027:0113) |   ├─TypeAdditionalPropertiesAccessSyntax
//@[027:0111) |   | ├─TypePropertyAccessSyntax
//@[027:0090) |   | | ├─TypePropertyAccessSyntax
//@[027:0079) |   | | | ├─ParameterizedTypeInstantiationSyntax
//@[027:0040) |   | | | | ├─IdentifierSyntax
//@[027:0040) |   | | | | | └─Token(Identifier) |resourceInput|
//@[040:0041) |   | | | | ├─Token(LeftChevron) |<|
//@[041:0078) |   | | | | ├─ParameterizedTypeArgumentSyntax
//@[041:0078) |   | | | | | └─StringTypeLiteralSyntax
//@[041:0078) |   | | | | |   └─Token(StringComplete) |'Microsoft.Web/customApis@2016-06-01'|
//@[078:0079) |   | | | | └─Token(RightChevron) |>|
//@[079:0080) |   | | | ├─Token(Dot) |.|
//@[080:0090) |   | | | └─IdentifierSyntax
//@[080:0090) |   | | |   └─Token(Identifier) |properties|
//@[090:0091) |   | | ├─Token(Dot) |.|
//@[091:0111) |   | | └─IdentifierSyntax
//@[091:0111) |   | |   └─Token(Identifier) |connectionParameters|
//@[111:0112) |   | ├─Token(Dot) |.|
//@[112:0113) |   | └─Token(Asterisk) |*|
//@[113:0114) |   ├─Token(Dot) |.|
//@[114:0119) |   └─IdentifierSyntax
//@[114:0119) |     └─Token(Identifier) |tyype|
//@[119:0120) ├─Token(NewLine) |\n|
type typoInPropertyName5 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type
//@[000:0118) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |typoInPropertyName5|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0118) | └─TypePropertyAccessSyntax
//@[027:0113) |   ├─TypePropertyAccessSyntax
//@[027:0092) |   | ├─TypeAdditionalPropertiesAccessSyntax
//@[027:0090) |   | | ├─TypePropertyAccessSyntax
//@[027:0079) |   | | | ├─ParameterizedTypeInstantiationSyntax
//@[027:0040) |   | | | | ├─IdentifierSyntax
//@[027:0040) |   | | | | | └─Token(Identifier) |resourceInput|
//@[040:0041) |   | | | | ├─Token(LeftChevron) |<|
//@[041:0078) |   | | | | ├─ParameterizedTypeArgumentSyntax
//@[041:0078) |   | | | | | └─StringTypeLiteralSyntax
//@[041:0078) |   | | | | |   └─Token(StringComplete) |'Microsoft.Web/customApis@2016-06-01'|
//@[078:0079) |   | | | | └─Token(RightChevron) |>|
//@[079:0080) |   | | | ├─Token(Dot) |.|
//@[080:0090) |   | | | └─IdentifierSyntax
//@[080:0090) |   | | |   └─Token(Identifier) |properties|
//@[090:0091) |   | | ├─Token(Dot) |.|
//@[091:0092) |   | | └─Token(Asterisk) |*|
//@[092:0093) |   | ├─Token(Dot) |.|
//@[093:0113) |   | └─IdentifierSyntax
//@[093:0113) |   |   └─Token(Identifier) |connectionParameters|
//@[113:0114) |   ├─Token(Dot) |.|
//@[114:0118) |   └─IdentifierSyntax
//@[114:0118) |     └─Token(Identifier) |type|
//@[118:0120) ├─Token(NewLine) |\n\n|

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
