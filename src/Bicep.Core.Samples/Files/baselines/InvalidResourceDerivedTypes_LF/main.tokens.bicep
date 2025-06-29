type invalid1 = resourceInput
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid1|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:031) NewLine |\n\n|

type invalid2 = resourceInput<>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid2|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:030) LeftChevron |<|
//@[030:031) RightChevron |>|
//@[031:033) NewLine |\n\n|

type invalid3 = resourceInput<'abc', 'def'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid3|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:030) LeftChevron |<|
//@[030:035) StringComplete |'abc'|
//@[035:036) Comma |,|
//@[037:042) StringComplete |'def'|
//@[042:043) RightChevron |>|
//@[043:044) NewLine |\n|
type invalid4 = resourceInput<hello>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid4|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:030) LeftChevron |<|
//@[030:035) Identifier |hello|
//@[035:036) RightChevron |>|
//@[036:037) NewLine |\n|
type invalid5 = resourceInput<'Microsoft.Storage/storageAccounts'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid5|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:030) LeftChevron |<|
//@[030:065) StringComplete |'Microsoft.Storage/storageAccounts'|
//@[065:066) RightChevron |>|
//@[066:067) NewLine |\n|
type invalid6 = resourceInput<'Microsoft.Storage/storageAccounts@'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid6|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:030) LeftChevron |<|
//@[030:066) StringComplete |'Microsoft.Storage/storageAccounts@'|
//@[066:067) RightChevron |>|
//@[067:068) NewLine |\n|
type invalid7 = resourceInput<'Microsoft.Storage/storageAccounts@hello'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid7|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:030) LeftChevron |<|
//@[030:071) StringComplete |'Microsoft.Storage/storageAccounts@hello'|
//@[071:072) RightChevron |>|
//@[072:073) NewLine |\n|
type invalid8 = resourceInput<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid8|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:030) LeftChevron |<|
//@[030:094) StringComplete |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[094:095) RightChevron |>|
//@[095:096) NewLine |\n|
type invalid9 = resourceInput<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:004) Identifier |type|
//@[005:013) Identifier |invalid9|
//@[014:015) Assignment |=|
//@[016:029) Identifier |resourceInput|
//@[029:030) LeftChevron |<|
//@[030:077) StringComplete |':Microsoft.Storage/storageAccounts@2022-09-01'|
//@[077:078) RightChevron |>|
//@[078:079) NewLine |\n|
type invalid10 = resourceInput<'abc' 'def'>
//@[000:004) Identifier |type|
//@[005:014) Identifier |invalid10|
//@[015:016) Assignment |=|
//@[017:030) Identifier |resourceInput|
//@[030:031) LeftChevron |<|
//@[031:036) StringComplete |'abc'|
//@[037:042) StringComplete |'def'|
//@[042:043) RightChevron |>|
//@[043:044) NewLine |\n|
type invalid11 = resourceInput<123>
//@[000:004) Identifier |type|
//@[005:014) Identifier |invalid11|
//@[015:016) Assignment |=|
//@[017:030) Identifier |resourceInput|
//@[030:031) LeftChevron |<|
//@[031:034) Integer |123|
//@[034:035) RightChevron |>|
//@[035:036) NewLine |\n|
type invalid12 = resourceInput<resourceGroup()>
//@[000:004) Identifier |type|
//@[005:014) Identifier |invalid12|
//@[015:016) Assignment |=|
//@[017:030) Identifier |resourceInput|
//@[030:031) LeftChevron |<|
//@[031:044) Identifier |resourceGroup|
//@[044:045) LeftParen |(|
//@[045:046) RightParen |)|
//@[046:047) RightChevron |>|
//@[047:049) NewLine |\n\n|

type thisIsWeird = resourceInput</*
//@[000:004) Identifier |type|
//@[005:016) Identifier |thisIsWeird|
//@[017:018) Assignment |=|
//@[019:032) Identifier |resourceInput|
//@[032:033) LeftChevron |<|
*/'Astronomer.Astro/organizations@2023-08-01-preview'
//@[002:053) StringComplete |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[053:054) NewLine |\n|
///  >
//@[006:007) NewLine |\n|
>
//@[000:001) RightChevron |>|
//@[001:003) NewLine |\n\n|

type interpolated = resourceInput<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[000:004) Identifier |type|
//@[005:017) Identifier |interpolated|
//@[018:019) Assignment |=|
//@[020:033) Identifier |resourceInput|
//@[033:034) LeftChevron |<|
//@[034:047) StringLeftPiece |'Microsoft.${|
//@[047:056) StringComplete |'Storage'|
//@[056:085) StringRightPiece |}/storageAccounts@2022-09-01'|
//@[085:086) RightChevron |>|
//@[086:088) NewLine |\n\n|

@sealed()
//@[000:001) At |@|
//@[001:007) Identifier |sealed|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
type shouldNotBeSealable = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[000:004) Identifier |type|
//@[005:024) Identifier |shouldNotBeSealable|
//@[025:026) Assignment |=|
//@[027:040) Identifier |resourceInput|
//@[040:041) LeftChevron |<|
//@[041:087) StringComplete |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[087:088) RightChevron |>|
//@[088:090) NewLine |\n\n|

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
  bar: resourceInput<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[002:005) Identifier |bar|
//@[005:006) Colon |:|
//@[007:020) Identifier |resourceInput|
//@[020:021) LeftChevron |<|
//@[021:072) StringComplete |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[072:073) RightChevron |>|
//@[073:074) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typoInPropertyName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.nom
//@[000:004) Identifier |type|
//@[005:023) Identifier |typoInPropertyName|
//@[024:025) Assignment |=|
//@[026:039) Identifier |resourceInput|
//@[039:040) LeftChevron |<|
//@[040:086) StringComplete |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[086:087) RightChevron |>|
//@[087:088) Dot |.|
//@[088:091) Identifier |nom|
//@[091:092) NewLine |\n|
type typoInPropertyName2 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*].tenatId
//@[000:004) Identifier |type|
//@[005:024) Identifier |typoInPropertyName2|
//@[025:026) Assignment |=|
//@[027:040) Identifier |resourceInput|
//@[040:041) LeftChevron |<|
//@[041:079) StringComplete |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[079:080) RightChevron |>|
//@[080:081) Dot |.|
//@[081:091) Identifier |properties|
//@[091:092) Dot |.|
//@[092:106) Identifier |accessPolicies|
//@[106:107) LeftSquare |[|
//@[107:108) Asterisk |*|
//@[108:109) RightSquare |]|
//@[109:110) Dot |.|
//@[110:117) Identifier |tenatId|
//@[117:118) NewLine |\n|
type typoInPropertyName3 = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties[*].accessPolicies.tenantId
//@[000:004) Identifier |type|
//@[005:024) Identifier |typoInPropertyName3|
//@[025:026) Assignment |=|
//@[027:040) Identifier |resourceInput|
//@[040:041) LeftChevron |<|
//@[041:079) StringComplete |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[079:080) RightChevron |>|
//@[080:081) Dot |.|
//@[081:091) Identifier |properties|
//@[091:092) LeftSquare |[|
//@[092:093) Asterisk |*|
//@[093:094) RightSquare |]|
//@[094:095) Dot |.|
//@[095:109) Identifier |accessPolicies|
//@[109:110) Dot |.|
//@[110:118) Identifier |tenantId|
//@[118:119) NewLine |\n|
type typoInPropertyName4 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.connectionParameters.*.tyype
//@[000:004) Identifier |type|
//@[005:024) Identifier |typoInPropertyName4|
//@[025:026) Assignment |=|
//@[027:040) Identifier |resourceInput|
//@[040:041) LeftChevron |<|
//@[041:078) StringComplete |'Microsoft.Web/customApis@2016-06-01'|
//@[078:079) RightChevron |>|
//@[079:080) Dot |.|
//@[080:090) Identifier |properties|
//@[090:091) Dot |.|
//@[091:111) Identifier |connectionParameters|
//@[111:112) Dot |.|
//@[112:113) Asterisk |*|
//@[113:114) Dot |.|
//@[114:119) Identifier |tyype|
//@[119:120) NewLine |\n|
type typoInPropertyName5 = resourceInput<'Microsoft.Web/customApis@2016-06-01'>.properties.*.connectionParameters.type
//@[000:004) Identifier |type|
//@[005:024) Identifier |typoInPropertyName5|
//@[025:026) Assignment |=|
//@[027:040) Identifier |resourceInput|
//@[040:041) LeftChevron |<|
//@[041:078) StringComplete |'Microsoft.Web/customApis@2016-06-01'|
//@[078:079) RightChevron |>|
//@[079:080) Dot |.|
//@[080:090) Identifier |properties|
//@[090:091) Dot |.|
//@[091:092) Asterisk |*|
//@[092:093) Dot |.|
//@[093:113) Identifier |connectionParameters|
//@[113:114) Dot |.|
//@[114:118) Identifier |type|
//@[118:120) NewLine |\n\n|

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
