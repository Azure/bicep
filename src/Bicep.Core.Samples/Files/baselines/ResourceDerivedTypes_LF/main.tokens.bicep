type foo = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[000:004) Identifier |type|
//@[005:008) Identifier |foo|
//@[009:010) Assignment |=|
//@[011:024) Identifier |resourceInput|
//@[024:025) LeftChevron |<|
//@[025:071) StringComplete |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[071:072) RightChevron |>|
//@[072:073) Dot |.|
//@[073:077) Identifier |name|
//@[077:079) NewLine |\n\n|

type test = {
//@[000:004) Identifier |type|
//@[005:009) Identifier |test|
//@[010:011) Assignment |=|
//@[012:013) LeftBrace |{|
//@[013:014) NewLine |\n|
  resA: resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[002:006) Identifier |resA|
//@[006:007) Colon |:|
//@[008:021) Identifier |resourceInput|
//@[021:022) LeftChevron |<|
//@[022:068) StringComplete |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[068:069) RightChevron |>|
//@[069:070) Dot |.|
//@[070:074) Identifier |name|
//@[074:075) NewLine |\n|
  resB: sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[002:006) Identifier |resB|
//@[006:007) Colon |:|
//@[008:011) Identifier |sys|
//@[011:012) Dot |.|
//@[012:025) Identifier |resourceInput|
//@[025:026) LeftChevron |<|
//@[026:072) StringComplete |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[072:073) RightChevron |>|
//@[073:074) Dot |.|
//@[074:078) Identifier |name|
//@[078:079) NewLine |\n|
  resC: sys.array
//@[002:006) Identifier |resC|
//@[006:007) Colon |:|
//@[008:011) Identifier |sys|
//@[011:012) Dot |.|
//@[012:017) Identifier |array|
//@[017:018) NewLine |\n|
  resD: sys.resourceInput<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@[002:006) Identifier |resD|
//@[006:007) Colon |:|
//@[008:011) Identifier |sys|
//@[011:012) Dot |.|
//@[012:025) Identifier |resourceInput|
//@[025:026) LeftChevron |<|
//@[026:075) StringComplete |'az:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[075:076) RightChevron |>|
//@[076:077) Dot |.|
//@[077:081) Identifier |name|
//@[081:082) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type strangeFormatting = {
//@[000:004) Identifier |type|
//@[005:022) Identifier |strangeFormatting|
//@[023:024) Assignment |=|
//@[025:026) LeftBrace |{|
//@[026:027) NewLine |\n|
  test: resourceInput<
//@[002:006) Identifier |test|
//@[006:007) Colon |:|
//@[008:021) Identifier |resourceInput|
//@[021:022) LeftChevron |<|
//@[022:024) NewLine |\n\n|

  'Astronomer.Astro/organizations@2023-08-01-preview'
//@[002:053) StringComplete |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[053:055) NewLine |\n\n|

>.name
//@[000:001) RightChevron |>|
//@[001:002) Dot |.|
//@[002:006) Identifier |name|
//@[006:007) NewLine |\n|
  test2: resourceInput    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[002:007) Identifier |test2|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceInput|
//@[026:027) LeftChevron |<|
//@[027:073) StringComplete |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[073:074) RightChevron |>|
//@[074:075) Dot |.|
//@[075:079) Identifier |name|
//@[079:080) NewLine |\n|
  test3: resourceInput</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
//@[002:007) Identifier |test3|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceInput|
//@[022:023) LeftChevron |<|
//@[031:077) StringComplete |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[086:087) RightChevron |>|
//@[087:088) Dot |.|
//@[088:092) Identifier |name|
//@[092:093) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

@description('I love space(s)')
//@[000:001) At |@|
//@[001:012) Identifier |description|
//@[012:013) LeftParen |(|
//@[013:030) StringComplete |'I love space(s)'|
//@[030:031) RightParen |)|
//@[031:032) NewLine |\n|
type test2 = resourceInput<
//@[000:004) Identifier |type|
//@[005:010) Identifier |test2|
//@[011:012) Assignment |=|
//@[013:026) Identifier |resourceInput|
//@[026:027) LeftChevron |<|
//@[027:029) NewLine |\n\n|

     'Astronomer.Astro/organizations@2023-08-01-preview'
//@[005:056) StringComplete |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[056:058) NewLine |\n\n|

>.name
//@[000:001) RightChevron |>|
//@[001:002) Dot |.|
//@[002:006) Identifier |name|
//@[006:008) NewLine |\n\n|

param bar resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@[000:005) Identifier |param|
//@[006:009) Identifier |bar|
//@[010:023) Identifier |resourceInput|
//@[023:024) LeftChevron |<|
//@[024:061) StringComplete |'Microsoft.Resources/tags@2022-09-01'|
//@[061:062) RightChevron |>|
//@[062:063) Dot |.|
//@[063:073) Identifier |properties|
//@[074:075) Assignment |=|
//@[076:077) LeftBrace |{|
//@[077:078) NewLine |\n|
  tags: {
//@[002:006) Identifier |tags|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[009:010) NewLine |\n|
    fizz: 'buzz'
//@[004:008) Identifier |fizz|
//@[008:009) Colon |:|
//@[010:016) StringComplete |'buzz'|
//@[016:017) NewLine |\n|
    snap: 'crackle'
//@[004:008) Identifier |snap|
//@[008:009) Colon |:|
//@[010:019) StringComplete |'crackle'|
//@[019:020) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

output baz resourceInput<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@[000:006) Identifier |output|
//@[007:010) Identifier |baz|
//@[011:024) Identifier |resourceInput|
//@[024:025) LeftChevron |<|
//@[025:086) StringComplete |'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'|
//@[086:087) RightChevron |>|
//@[087:088) Dot |.|
//@[088:092) Identifier |name|
//@[093:094) Assignment |=|
//@[095:101) StringComplete |'myId'|
//@[101:103) NewLine |\n\n|

type storageAccountName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@[000:004) Identifier |type|
//@[005:023) Identifier |storageAccountName|
//@[024:025) Assignment |=|
//@[026:039) Identifier |resourceInput|
//@[039:040) LeftChevron |<|
//@[040:086) StringComplete |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[086:087) RightChevron |>|
//@[087:088) Dot |.|
//@[088:092) Identifier |name|
//@[092:093) NewLine |\n|
type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@[000:004) Identifier |type|
//@[005:017) Identifier |accessPolicy|
//@[018:019) Assignment |=|
//@[020:033) Identifier |resourceInput|
//@[033:034) LeftChevron |<|
//@[034:072) StringComplete |'Microsoft.KeyVault/vaults@2022-07-01'|
//@[072:073) RightChevron |>|
//@[073:074) Dot |.|
//@[074:084) Identifier |properties|
//@[084:085) Dot |.|
//@[085:099) Identifier |accessPolicies|
//@[099:100) LeftSquare |[|
//@[100:101) Asterisk |*|
//@[101:102) RightSquare |]|
//@[102:103) NewLine |\n|
type tag = resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@[000:004) Identifier |type|
//@[005:008) Identifier |tag|
//@[009:010) Assignment |=|
//@[011:024) Identifier |resourceInput|
//@[024:025) LeftChevron |<|
//@[025:062) StringComplete |'Microsoft.Resources/tags@2022-09-01'|
//@[062:063) RightChevron |>|
//@[063:064) Dot |.|
//@[064:074) Identifier |properties|
//@[074:075) Dot |.|
//@[075:079) Identifier |tags|
//@[079:080) Dot |.|
//@[080:081) Asterisk |*|
//@[081:082) NewLine |\n|

//@[000:000) EndOfFile ||
