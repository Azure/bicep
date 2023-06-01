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
//@[00:05) Identifier |param|
//@[06:21) Identifier |decoratedString|
//@[22:23) Assignment |=|
//@[24:31) StringComplete |'Apple'|
//@[31:32) NewLine |\n|
param stringfromEnvironmentVariables = readEnvironmentVariable('stringEnvVariableName')
//@[00:05) Identifier |param|
//@[06:36) Identifier |stringfromEnvironmentVariables|
//@[37:38) Assignment |=|
//@[39:62) Identifier |readEnvironmentVariable|
//@[62:63) LeftParen |(|
//@[63:86) StringComplete |'stringEnvVariableName'|
//@[86:87) RightParen |)|
//@[87:88) NewLine |\n|
param intfromEnvironmentVariables = int(readEnvironmentVariable('intEnvVariableName'))
//@[00:05) Identifier |param|
//@[06:33) Identifier |intfromEnvironmentVariables|
//@[34:35) Assignment |=|
//@[36:39) Identifier |int|
//@[39:40) LeftParen |(|
//@[40:63) Identifier |readEnvironmentVariable|
//@[63:64) LeftParen |(|
//@[64:84) StringComplete |'intEnvVariableName'|
//@[84:85) RightParen |)|
//@[85:86) RightParen |)|
//@[86:87) NewLine |\n|
param boolfromEnvironmentVariables = bool(readEnvironmentVariable('boolEnvironmentVariable'))
//@[00:05) Identifier |param|
//@[06:34) Identifier |boolfromEnvironmentVariables|
//@[35:36) Assignment |=|
//@[37:41) Identifier |bool|
//@[41:42) LeftParen |(|
//@[42:65) Identifier |readEnvironmentVariable|
//@[65:66) LeftParen |(|
//@[66:91) StringComplete |'boolEnvironmentVariable'|
//@[91:92) RightParen |)|
//@[92:93) RightParen |)|
//@[93:94) NewLine |\n|

//@[000:000) EndOfFile ||
