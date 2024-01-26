type invalid1 = resource
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid1|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:026) NewLine |\n\n|

type invalid2 = resource<>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid2|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:025) LeftChevron |<|
//@[025:026) RightChevron |>|
//@[026:028) NewLine |\n\n|

type invalid3 = resource<'abc', 'def'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid3|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:025) LeftChevron |<|
//@[025:030) StringComplete |'abc'|
//@[030:031) Comma |,|
//@[032:037) StringComplete |'def'|
//@[037:038) RightChevron |>|
//@[038:039) NewLine |\n|
type invalid4 = resource<hello>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid4|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:025) LeftChevron |<|
//@[025:030) Identifier |hello|
//@[030:031) RightChevron |>|
//@[031:032) NewLine |\n|
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid5|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:025) LeftChevron |<|
//@[025:060) StringComplete |'Microsoft.Storage/storageAccounts'|
//@[060:061) RightChevron |>|
//@[061:062) NewLine |\n|
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid6|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:025) LeftChevron |<|
//@[025:061) StringComplete |'Microsoft.Storage/storageAccounts@'|
//@[061:062) RightChevron |>|
//@[062:063) NewLine |\n|
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid7|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:025) LeftChevron |<|
//@[025:066) StringComplete |'Microsoft.Storage/storageAccounts@hello'|
//@[066:067) RightChevron |>|
//@[067:068) NewLine |\n|
type invalid8 = resource<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid8|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:025) LeftChevron |<|
//@[025:089) StringComplete |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[089:090) RightChevron |>|
//@[090:091) NewLine |\n|
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid9|
//@[014:015) Assignment |=|
//@[016:024) Identifier |resource|
//@[024:025) LeftChevron |<|
//@[025:072) StringComplete |':Microsoft.Storage/storageAccounts@2022-09-01'|
//@[072:073) RightChevron |>|
//@[073:074) NewLine |\n|
type invalid10 = resource<'abc' 'def'>
//@[000:004) Identifier |type|
//@[005:014) Identifier |invalid10|
//@[015:016) Assignment |=|
//@[017:025) Identifier |resource|
//@[025:026) LeftChevron |<|
//@[026:031) StringComplete |'abc'|
//@[032:037) StringComplete |'def'|
//@[037:038) RightChevron |>|
//@[038:039) NewLine |\n|
type invalid11 = resource<123>
//@[000:004) Identifier |type|
//@[005:014) Identifier |invalid11|
//@[015:016) Assignment |=|
//@[017:025) Identifier |resource|
//@[025:026) LeftChevron |<|
//@[026:029) Integer |123|
//@[029:030) RightChevron |>|
//@[030:031) NewLine |\n|
type invalid12 = resource<resourceGroup()>
//@[000:004) Identifier |type|
//@[005:014) Identifier |invalid12|
//@[015:016) Assignment |=|
//@[017:025) Identifier |resource|
//@[025:026) LeftChevron |<|
//@[026:039) Identifier |resourceGroup|
//@[039:040) LeftParen |(|
//@[040:041) RightParen |)|
//@[041:042) RightChevron |>|
//@[042:044) NewLine |\n\n|

type thisIsWeird = resource</*
//@[000:004) Identifier |type|
//@[005:016) Identifier |thisIsWeird|
//@[017:018) Assignment |=|
//@[019:027) Identifier |resource|
//@[027:028) LeftChevron |<|
*/'Astronomer.Astro/organizations@2023-08-01-preview'
//@[002:053) StringComplete |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[053:054) NewLine |\n|
///  >
//@[006:007) NewLine |\n|
>
//@[000:001) RightChevron |>|
//@[001:003) NewLine |\n\n|

type interpolated = resource<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[000:004) Identifier |type|
//@[005:017) Identifier |interpolated|
//@[018:019) Assignment |=|
//@[020:028) Identifier |resource|
//@[028:029) LeftChevron |<|
//@[029:042) StringLeftPiece |'Microsoft.${|
//@[042:051) StringComplete |'Storage'|
//@[051:080) StringRightPiece |}/storageAccounts@2022-09-01'|
//@[080:081) RightChevron |>|
//@[081:083) NewLine |\n\n|

@sealed()
//@[000:001) At |@|
//@[001:007) Identifier |sealed|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
type shouldNotBeSealable = resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:004) Identifier |type|
//@[005:024) Identifier |shouldNotBeSealable|
//@[025:026) Assignment |=|
//@[027:035) Identifier |resource|
//@[035:036) LeftChevron |<|
//@[036:082) StringComplete |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[082:083) RightChevron |>|
//@[083:085) NewLine |\n\n|

type hello = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |hello|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  @discriminator('hi')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:021) StringComplete |'hi'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[002:005) Identifier |bar|
//@[005:006) Colon |:|
//@[007:015) Identifier |resource|
//@[015:016) LeftChevron |<|
//@[016:067) StringComplete |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[067:068) RightChevron |>|
//@[068:069) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typoInPropertyName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
//@[000:004) Identifier |type|
//@[005:023) Identifier |typoInPropertyName|
//@[024:025) Assignment |=|
//@[026:034) Identifier |resource|
//@[034:035) LeftChevron |<|
//@[035:081) StringComplete |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[081:082) RightChevron |>|
//@[082:083) Dot |.|
//@[083:086) Identifier |nom|
//@[086:087) NewLine |\n|
type typoInPropertyName2 = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
//@[000:004) Identifier |type|
//@[005:024) Identifier |typoInPropertyName2|
//@[025:026) Assignment |=|
//@[027:035) Identifier |resource|
//@[035:036) LeftChevron |<|
//@[036:074) StringComplete |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[074:075) RightChevron |>|
//@[075:076) Dot |.|
//@[076:086) Identifier |properties|
//@[086:087) Dot |.|
//@[087:101) Identifier |accessPolicies|
//@[101:102) LeftSquare |[|
//@[102:103) Asterisk |*|
//@[103:104) RightSquare |]|
//@[104:105) Dot |.|
//@[105:112) Identifier |tenatId|
//@[112:113) NewLine |\n|
type typoInPropertyName3 = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
//@[000:004) Identifier |type|
//@[005:024) Identifier |typoInPropertyName3|
//@[025:026) Assignment |=|
//@[027:035) Identifier |resource|
//@[035:036) LeftChevron |<|
//@[036:074) StringComplete |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[074:075) RightChevron |>|
//@[075:076) Dot |.|
//@[076:086) Identifier |properties|
//@[086:087) LeftSquare |[|
//@[087:088) Asterisk |*|
//@[088:089) RightSquare |]|
//@[089:090) Dot |.|
//@[090:104) Identifier |accessPolicies|
//@[104:105) Dot |.|
//@[105:113) Identifier |tenantId|
//@[113:114) NewLine |\n|
type typoInPropertyName4 = resource<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
//@[000:004) Identifier |type|
//@[005:024) Identifier |typoInPropertyName4|
//@[025:026) Assignment |=|
//@[027:035) Identifier |resource|
//@[035:036) LeftChevron |<|
//@[036:073) StringComplete |'Microsoft.Web/customApis@2016-06-01'|
//@[073:074) RightChevron |>|
//@[074:075) Dot |.|
//@[075:085) Identifier |properties|
//@[085:086) Dot |.|
//@[086:106) Identifier |connectionParameters|
//@[106:107) Dot |.|
//@[107:108) Asterisk |*|
//@[108:109) Dot |.|
//@[109:114) Identifier |tyype|
//@[114:115) NewLine |\n|
type typoInPropertyName5 = resource<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type
//@[000:004) Identifier |type|
//@[005:024) Identifier |typoInPropertyName5|
//@[025:026) Assignment |=|
//@[027:035) Identifier |resource|
//@[035:036) LeftChevron |<|
//@[036:073) StringComplete |'Microsoft.Web/customApis@2016-06-01'|
//@[073:074) RightChevron |>|
//@[074:075) Dot |.|
//@[075:085) Identifier |properties|
//@[085:086) Dot |.|
//@[086:087) Asterisk |*|
//@[087:088) Dot |.|
//@[088:108) Identifier |connectionParameters|
//@[108:109) Dot |.|
//@[109:113) Identifier |type|
//@[113:115) NewLine |\n\n|

module mod 'modules/mod.json' = {
//@[000:006) Identifier |module|
//@[007:010) Identifier |mod|
//@[011:029) StringComplete |'modules/mod.json'|
//@[030:031) Assignment |=|
//@[032:033) LeftBrace |{|
//@[033:034) NewLine |\n|
  name: 'mod'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'mod'|
//@[013:014) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    foo: {}
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:011) RightBrace |}|
//@[011:012) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|

//@[000:000) EndOfFile ||
