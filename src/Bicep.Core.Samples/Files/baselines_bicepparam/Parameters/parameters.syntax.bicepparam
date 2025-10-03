/*
//@[000:1136) ProgramSyntax
This is a
multiline comment!
*/
//@[002:0004) ├─Token(NewLine) |\n\n|

// This is a single line comment
//@[032:0034) ├─Token(NewLine) |\n\n|

// using keyword for specifying a Bicep file
//@[044:0045) ├─Token(NewLine) |\n|
using './main.bicep'
//@[000:0020) ├─UsingDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |using|
//@[006:0020) | ├─StringSyntax
//@[006:0020) | | └─Token(StringComplete) |'./main.bicep'|
//@[020:0020) | └─SkippedTriviaSyntax
//@[020:0022) ├─Token(NewLine) |\n\n|

// parameter assignment to literals
//@[035:0036) ├─Token(NewLine) |\n|
param myString = 'hello world!!'
//@[000:0032) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0014) | ├─IdentifierSyntax
//@[006:0014) | | └─Token(Identifier) |myString|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0032) | └─StringSyntax
//@[017:0032) |   └─Token(StringComplete) |'hello world!!'|
//@[032:0033) ├─Token(NewLine) |\n|
param myInt = 42
//@[000:0016) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0011) | ├─IdentifierSyntax
//@[006:0011) | | └─Token(Identifier) |myInt|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0016) | └─IntegerLiteralSyntax
//@[014:0016) |   └─Token(Integer) |42|
//@[016:0017) ├─Token(NewLine) |\n|
param myBool = false
//@[000:0020) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0012) | ├─IdentifierSyntax
//@[006:0012) | | └─Token(Identifier) |myBool|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0020) | └─BooleanLiteralSyntax
//@[015:0020) |   └─Token(FalseKeyword) |false|
//@[020:0022) ├─Token(NewLine) |\n\n|

param numberOfVMs = 1
//@[000:0021) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |numberOfVMs|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0021) | └─IntegerLiteralSyntax
//@[020:0021) |   └─Token(Integer) |1|
//@[021:0023) ├─Token(NewLine) |\n\n|

// parameter assignment to objects
//@[034:0035) ├─Token(NewLine) |\n|
param password = 'strongPassword'
//@[000:0033) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0014) | ├─IdentifierSyntax
//@[006:0014) | | └─Token(Identifier) |password|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0033) | └─StringSyntax
//@[017:0033) |   └─Token(StringComplete) |'strongPassword'|
//@[033:0034) ├─Token(NewLine) |\n|
param secretObject = {
//@[000:0061) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0018) | ├─IdentifierSyntax
//@[006:0018) | | └─Token(Identifier) |secretObject|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0061) | └─ObjectSyntax
//@[021:0022) |   ├─Token(LeftBrace) |{|
//@[022:0023) |   ├─Token(NewLine) |\n|
  name : 'vm2'
//@[002:0014) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0014) |   | └─StringSyntax
//@[009:0014) |   |   └─Token(StringComplete) |'vm2'|
//@[014:0015) |   ├─Token(NewLine) |\n|
  location : 'westus'
//@[002:0021) |   ├─ObjectPropertySyntax
//@[002:0010) |   | ├─IdentifierSyntax
//@[002:0010) |   | | └─Token(Identifier) |location|
//@[011:0012) |   | ├─Token(Colon) |:|
//@[013:0021) |   | └─StringSyntax
//@[013:0021) |   |   └─Token(StringComplete) |'westus'|
//@[021:0022) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
param storageSku = 'Standard_LRS'
//@[000:0033) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0016) | ├─IdentifierSyntax
//@[006:0016) | | └─Token(Identifier) |storageSku|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0033) | └─StringSyntax
//@[019:0033) |   └─Token(StringComplete) |'Standard_LRS'|
//@[033:0034) ├─Token(NewLine) |\n|
param storageName = 'myStorage'
//@[000:0031) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |storageName|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0031) | └─StringSyntax
//@[020:0031) |   └─Token(StringComplete) |'myStorage'|
//@[031:0032) ├─Token(NewLine) |\n|
param someArray = [
//@[000:0045) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |someArray|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0045) | └─ArraySyntax
//@[018:0019) |   ├─Token(LeftSquare) |[|
//@[019:0020) |   ├─Token(NewLine) |\n|
  'a'
//@[002:0005) |   ├─ArrayItemSyntax
//@[002:0005) |   | └─StringSyntax
//@[002:0005) |   |   └─Token(StringComplete) |'a'|
//@[005:0006) |   ├─Token(NewLine) |\n|
  'b'
//@[002:0005) |   ├─ArrayItemSyntax
//@[002:0005) |   | └─StringSyntax
//@[002:0005) |   |   └─Token(StringComplete) |'b'|
//@[005:0006) |   ├─Token(NewLine) |\n|
  'c'
//@[002:0005) |   ├─ArrayItemSyntax
//@[002:0005) |   | └─StringSyntax
//@[002:0005) |   |   └─Token(StringComplete) |'c'|
//@[005:0006) |   ├─Token(NewLine) |\n|
  'd'
//@[002:0005) |   ├─ArrayItemSyntax
//@[002:0005) |   | └─StringSyntax
//@[002:0005) |   |   └─Token(StringComplete) |'d'|
//@[005:0006) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0002) ├─Token(NewLine) |\n|
param emptyMetadata = 'empty!'
//@[000:0030) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |emptyMetadata|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0030) | └─StringSyntax
//@[022:0030) |   └─Token(StringComplete) |'empty!'|
//@[030:0031) ├─Token(NewLine) |\n|
param description = 'descriptive description'
//@[000:0045) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |description|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0045) | └─StringSyntax
//@[020:0045) |   └─Token(StringComplete) |'descriptive description'|
//@[045:0046) ├─Token(NewLine) |\n|
param description2 = 'also descriptive'
//@[000:0039) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0018) | ├─IdentifierSyntax
//@[006:0018) | | └─Token(Identifier) |description2|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0039) | └─StringSyntax
//@[021:0039) |   └─Token(StringComplete) |'also descriptive'|
//@[039:0040) ├─Token(NewLine) |\n|
param additionalMetadata = 'more metadata'
//@[000:0042) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0024) | ├─IdentifierSyntax
//@[006:0024) | | └─Token(Identifier) |additionalMetadata|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0042) | └─StringSyntax
//@[027:0042) |   └─Token(StringComplete) |'more metadata'|
//@[042:0043) ├─Token(NewLine) |\n|
param someParameter = 'three'
//@[000:0029) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |someParameter|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0029) | └─StringSyntax
//@[022:0029) |   └─Token(StringComplete) |'three'|
//@[029:0030) ├─Token(NewLine) |\n|
param stringLiteral = 'abc'
//@[000:0027) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |stringLiteral|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0027) | └─StringSyntax
//@[022:0027) |   └─Token(StringComplete) |'abc'|
//@[027:0028) ├─Token(NewLine) |\n|
param decoratedString = 'Apple'
//@[000:0031) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |decoratedString|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0031) | └─StringSyntax
//@[024:0031) |   └─Token(StringComplete) |'Apple'|
//@[031:0032) ├─Token(NewLine) |\n|
param stringfromEnvironmentVariables = readEnvironmentVariable('stringEnvVariableName')
//@[000:0087) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0036) | ├─IdentifierSyntax
//@[006:0036) | | └─Token(Identifier) |stringfromEnvironmentVariables|
//@[037:0038) | ├─Token(Assignment) |=|
//@[039:0087) | └─FunctionCallSyntax
//@[039:0062) |   ├─IdentifierSyntax
//@[039:0062) |   | └─Token(Identifier) |readEnvironmentVariable|
//@[062:0063) |   ├─Token(LeftParen) |(|
//@[063:0086) |   ├─FunctionArgumentSyntax
//@[063:0086) |   | └─StringSyntax
//@[063:0086) |   |   └─Token(StringComplete) |'stringEnvVariableName'|
//@[086:0087) |   └─Token(RightParen) |)|
//@[087:0088) ├─Token(NewLine) |\n|
param intfromEnvironmentVariables = int(readEnvironmentVariable('intEnvVariableName'))
//@[000:0086) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0033) | ├─IdentifierSyntax
//@[006:0033) | | └─Token(Identifier) |intfromEnvironmentVariables|
//@[034:0035) | ├─Token(Assignment) |=|
//@[036:0086) | └─FunctionCallSyntax
//@[036:0039) |   ├─IdentifierSyntax
//@[036:0039) |   | └─Token(Identifier) |int|
//@[039:0040) |   ├─Token(LeftParen) |(|
//@[040:0085) |   ├─FunctionArgumentSyntax
//@[040:0085) |   | └─FunctionCallSyntax
//@[040:0063) |   |   ├─IdentifierSyntax
//@[040:0063) |   |   | └─Token(Identifier) |readEnvironmentVariable|
//@[063:0064) |   |   ├─Token(LeftParen) |(|
//@[064:0084) |   |   ├─FunctionArgumentSyntax
//@[064:0084) |   |   | └─StringSyntax
//@[064:0084) |   |   |   └─Token(StringComplete) |'intEnvVariableName'|
//@[084:0085) |   |   └─Token(RightParen) |)|
//@[085:0086) |   └─Token(RightParen) |)|
//@[086:0087) ├─Token(NewLine) |\n|
param boolfromEnvironmentVariables = bool(readEnvironmentVariable('boolEnvironmentVariable'))
//@[000:0093) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0034) | ├─IdentifierSyntax
//@[006:0034) | | └─Token(Identifier) |boolfromEnvironmentVariables|
//@[035:0036) | ├─Token(Assignment) |=|
//@[037:0093) | └─FunctionCallSyntax
//@[037:0041) |   ├─IdentifierSyntax
//@[037:0041) |   | └─Token(Identifier) |bool|
//@[041:0042) |   ├─Token(LeftParen) |(|
//@[042:0092) |   ├─FunctionArgumentSyntax
//@[042:0092) |   | └─FunctionCallSyntax
//@[042:0065) |   |   ├─IdentifierSyntax
//@[042:0065) |   |   | └─Token(Identifier) |readEnvironmentVariable|
//@[065:0066) |   |   ├─Token(LeftParen) |(|
//@[066:0091) |   |   ├─FunctionArgumentSyntax
//@[066:0091) |   |   | └─StringSyntax
//@[066:0091) |   |   |   └─Token(StringComplete) |'boolEnvironmentVariable'|
//@[091:0092) |   |   └─Token(RightParen) |)|
//@[092:0093) |   └─Token(RightParen) |)|
//@[093:0094) ├─Token(NewLine) |\n|
param intfromEnvironmentVariablesDefault = int(readEnvironmentVariable('intDefaultEnvVariableName','12'))
//@[000:0105) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0040) | ├─IdentifierSyntax
//@[006:0040) | | └─Token(Identifier) |intfromEnvironmentVariablesDefault|
//@[041:0042) | ├─Token(Assignment) |=|
//@[043:0105) | └─FunctionCallSyntax
//@[043:0046) |   ├─IdentifierSyntax
//@[043:0046) |   | └─Token(Identifier) |int|
//@[046:0047) |   ├─Token(LeftParen) |(|
//@[047:0104) |   ├─FunctionArgumentSyntax
//@[047:0104) |   | └─FunctionCallSyntax
//@[047:0070) |   |   ├─IdentifierSyntax
//@[047:0070) |   |   | └─Token(Identifier) |readEnvironmentVariable|
//@[070:0071) |   |   ├─Token(LeftParen) |(|
//@[071:0098) |   |   ├─FunctionArgumentSyntax
//@[071:0098) |   |   | └─StringSyntax
//@[071:0098) |   |   |   └─Token(StringComplete) |'intDefaultEnvVariableName'|
//@[098:0099) |   |   ├─Token(Comma) |,|
//@[099:0103) |   |   ├─FunctionArgumentSyntax
//@[099:0103) |   |   | └─StringSyntax
//@[099:0103) |   |   |   └─Token(StringComplete) |'12'|
//@[103:0104) |   |   └─Token(RightParen) |)|
//@[104:0105) |   └─Token(RightParen) |)|
//@[105:0106) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
