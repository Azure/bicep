/*
This is a
multiline comment!
*/
//@[002:004) NewLine |\n\n|

// This is a single line comment
//@[032:034) NewLine |\n\n|

// using keyword for specifying a Bicep file
//@[044:045) NewLine |\n|
using './main.bicep'
//@[000:005) Identifier |using|
//@[006:020) StringComplete |'./main.bicep'|
//@[020:022) NewLine |\n\n|

// parameter assignment to literals
//@[035:036) NewLine |\n|
param myString = 'hello world!!'
//@[000:005) Identifier |param|
//@[006:014) Identifier |myString|
//@[015:016) Assignment |=|
//@[017:032) StringComplete |'hello world!!'|
//@[032:033) NewLine |\n|
param myInt = 42
//@[000:005) Identifier |param|
//@[006:011) Identifier |myInt|
//@[012:013) Assignment |=|
//@[014:016) Integer |42|
//@[016:017) NewLine |\n|
param myBool = false
//@[000:005) Identifier |param|
//@[006:012) Identifier |myBool|
//@[013:014) Assignment |=|
//@[015:020) FalseKeyword |false|
//@[020:022) NewLine |\n\n|

param numberOfVMs = 1
//@[000:005) Identifier |param|
//@[006:017) Identifier |numberOfVMs|
//@[018:019) Assignment |=|
//@[020:021) Integer |1|
//@[021:023) NewLine |\n\n|

// parameter assignment to objects
//@[034:035) NewLine |\n|
param password = 'strongPassword'
//@[000:005) Identifier |param|
//@[006:014) Identifier |password|
//@[015:016) Assignment |=|
//@[017:033) StringComplete |'strongPassword'|
//@[033:034) NewLine |\n|
param secretObject = {
//@[000:005) Identifier |param|
//@[006:018) Identifier |secretObject|
//@[019:020) Assignment |=|
//@[021:022) LeftBrace |{|
//@[022:023) NewLine |\n|
  name : 'vm2'
//@[002:006) Identifier |name|
//@[007:008) Colon |:|
//@[009:014) StringComplete |'vm2'|
//@[014:015) NewLine |\n|
  location : 'westus'
//@[002:010) Identifier |location|
//@[011:012) Colon |:|
//@[013:021) StringComplete |'westus'|
//@[021:022) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
param storageSku = 'Standard_LRS'
//@[000:005) Identifier |param|
//@[006:016) Identifier |storageSku|
//@[017:018) Assignment |=|
//@[019:033) StringComplete |'Standard_LRS'|
//@[033:034) NewLine |\n|
param storageName = 'myStorage'
//@[000:005) Identifier |param|
//@[006:017) Identifier |storageName|
//@[018:019) Assignment |=|
//@[020:031) StringComplete |'myStorage'|
//@[031:032) NewLine |\n|
param someArray = [
//@[000:005) Identifier |param|
//@[006:015) Identifier |someArray|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:020) NewLine |\n|
  'a'
//@[002:005) StringComplete |'a'|
//@[005:006) NewLine |\n|
  'b'
//@[002:005) StringComplete |'b'|
//@[005:006) NewLine |\n|
  'c'
//@[002:005) StringComplete |'c'|
//@[005:006) NewLine |\n|
  'd'
//@[002:005) StringComplete |'d'|
//@[005:006) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:002) NewLine |\n|
param emptyMetadata = 'empty!'
//@[000:005) Identifier |param|
//@[006:019) Identifier |emptyMetadata|
//@[020:021) Assignment |=|
//@[022:030) StringComplete |'empty!'|
//@[030:031) NewLine |\n|
param description = 'descriptive description'
//@[000:005) Identifier |param|
//@[006:017) Identifier |description|
//@[018:019) Assignment |=|
//@[020:045) StringComplete |'descriptive description'|
//@[045:046) NewLine |\n|
param description2 = 'also descriptive'
//@[000:005) Identifier |param|
//@[006:018) Identifier |description2|
//@[019:020) Assignment |=|
//@[021:039) StringComplete |'also descriptive'|
//@[039:040) NewLine |\n|
param additionalMetadata = 'more metadata'
//@[000:005) Identifier |param|
//@[006:024) Identifier |additionalMetadata|
//@[025:026) Assignment |=|
//@[027:042) StringComplete |'more metadata'|
//@[042:043) NewLine |\n|
param someParameter = 'three'
//@[000:005) Identifier |param|
//@[006:019) Identifier |someParameter|
//@[020:021) Assignment |=|
//@[022:029) StringComplete |'three'|
//@[029:030) NewLine |\n|
param stringLiteral = 'abc'
//@[000:005) Identifier |param|
//@[006:019) Identifier |stringLiteral|
//@[020:021) Assignment |=|
//@[022:027) StringComplete |'abc'|
//@[027:028) NewLine |\n|
param decoratedString = 'Apple'
//@[000:005) Identifier |param|
//@[006:021) Identifier |decoratedString|
//@[022:023) Assignment |=|
//@[024:031) StringComplete |'Apple'|
//@[031:032) NewLine |\n|
param stringfromEnvironmentVariables = readEnvironmentVariable('stringEnvVariableName')
//@[000:005) Identifier |param|
//@[006:036) Identifier |stringfromEnvironmentVariables|
//@[037:038) Assignment |=|
//@[039:062) Identifier |readEnvironmentVariable|
//@[062:063) LeftParen |(|
//@[063:086) StringComplete |'stringEnvVariableName'|
//@[086:087) RightParen |)|
//@[087:088) NewLine |\n|
param intfromEnvironmentVariables = int(readEnvironmentVariable('intEnvVariableName'))
//@[000:005) Identifier |param|
//@[006:033) Identifier |intfromEnvironmentVariables|
//@[034:035) Assignment |=|
//@[036:039) Identifier |int|
//@[039:040) LeftParen |(|
//@[040:063) Identifier |readEnvironmentVariable|
//@[063:064) LeftParen |(|
//@[064:084) StringComplete |'intEnvVariableName'|
//@[084:085) RightParen |)|
//@[085:086) RightParen |)|
//@[086:087) NewLine |\n|
param boolfromEnvironmentVariables = bool(readEnvironmentVariable('boolEnvironmentVariable'))
//@[000:005) Identifier |param|
//@[006:034) Identifier |boolfromEnvironmentVariables|
//@[035:036) Assignment |=|
//@[037:041) Identifier |bool|
//@[041:042) LeftParen |(|
//@[042:065) Identifier |readEnvironmentVariable|
//@[065:066) LeftParen |(|
//@[066:091) StringComplete |'boolEnvironmentVariable'|
//@[091:092) RightParen |)|
//@[092:093) RightParen |)|
//@[093:094) NewLine |\n|
param intfromEnvironmentVariablesDefault = int(readEnvironmentVariable('intDefaultEnvVariableName','12'))
//@[000:005) Identifier |param|
//@[006:040) Identifier |intfromEnvironmentVariablesDefault|
//@[041:042) Assignment |=|
//@[043:046) Identifier |int|
//@[046:047) LeftParen |(|
//@[047:070) Identifier |readEnvironmentVariable|
//@[070:071) LeftParen |(|
//@[071:098) StringComplete |'intDefaultEnvVariableName'|
//@[098:099) Comma |,|
//@[099:103) StringComplete |'12'|
//@[103:104) RightParen |)|
//@[104:105) RightParen |)|
//@[105:106) NewLine |\n|

//@[000:000) EndOfFile ||
