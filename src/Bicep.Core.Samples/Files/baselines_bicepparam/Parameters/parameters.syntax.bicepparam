/*
//@[00:1030) ProgramSyntax
This is a
multiline comment!
*/
//@[02:0004) ├─Token(NewLine) |\n\n|

// This is a single line comment
//@[32:0034) ├─Token(NewLine) |\n\n|

// using keyword for specifying a Bicep file
//@[44:0045) ├─Token(NewLine) |\n|
using './main.bicep'
//@[00:0020) ├─UsingDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |using|
//@[06:0020) | └─StringSyntax
//@[06:0020) |   └─Token(StringComplete) |'./main.bicep'|
//@[20:0022) ├─Token(NewLine) |\n\n|

// parameter assignment to literals
//@[35:0036) ├─Token(NewLine) |\n|
param myString = 'hello world!!'
//@[00:0032) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |myString|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0032) | └─StringSyntax
//@[17:0032) |   └─Token(StringComplete) |'hello world!!'|
//@[32:0033) ├─Token(NewLine) |\n|
param myInt = 42
//@[00:0016) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0011) | ├─IdentifierSyntax
//@[06:0011) | | └─Token(Identifier) |myInt|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0016) | └─IntegerLiteralSyntax
//@[14:0016) |   └─Token(Integer) |42|
//@[16:0017) ├─Token(NewLine) |\n|
param myBool = false
//@[00:0020) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |myBool|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0020) | └─BooleanLiteralSyntax
//@[15:0020) |   └─Token(FalseKeyword) |false|
//@[20:0022) ├─Token(NewLine) |\n\n|

param numberOfVMs = 1
//@[00:0021) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |numberOfVMs|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0021) | └─IntegerLiteralSyntax
//@[20:0021) |   └─Token(Integer) |1|
//@[21:0023) ├─Token(NewLine) |\n\n|

// parameter assignment to objects
//@[34:0035) ├─Token(NewLine) |\n|
param password = 'strongPassword'
//@[00:0033) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |password|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0033) | └─StringSyntax
//@[17:0033) |   └─Token(StringComplete) |'strongPassword'|
//@[33:0034) ├─Token(NewLine) |\n|
param secretObject = {
//@[00:0061) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |secretObject|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0061) | └─ObjectSyntax
//@[21:0022) |   ├─Token(LeftBrace) |{|
//@[22:0023) |   ├─Token(NewLine) |\n|
  name : 'vm2'
//@[02:0014) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0014) |   | └─StringSyntax
//@[09:0014) |   |   └─Token(StringComplete) |'vm2'|
//@[14:0015) |   ├─Token(NewLine) |\n|
  location : 'westus'
//@[02:0021) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |location|
//@[11:0012) |   | ├─Token(Colon) |:|
//@[13:0021) |   | └─StringSyntax
//@[13:0021) |   |   └─Token(StringComplete) |'westus'|
//@[21:0022) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
param storageSku = 'Standard_LRS'
//@[00:0033) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |storageSku|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0033) | └─StringSyntax
//@[19:0033) |   └─Token(StringComplete) |'Standard_LRS'|
//@[33:0034) ├─Token(NewLine) |\n|
param storageName = 'myStorage'
//@[00:0031) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |storageName|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0031) | └─StringSyntax
//@[20:0031) |   └─Token(StringComplete) |'myStorage'|
//@[31:0032) ├─Token(NewLine) |\n|
param someArray = [
//@[00:0045) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |someArray|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0045) | └─ArraySyntax
//@[18:0019) |   ├─Token(LeftSquare) |[|
//@[19:0020) |   ├─Token(NewLine) |\n|
  'a'
//@[02:0005) |   ├─ArrayItemSyntax
//@[02:0005) |   | └─StringSyntax
//@[02:0005) |   |   └─Token(StringComplete) |'a'|
//@[05:0006) |   ├─Token(NewLine) |\n|
  'b'
//@[02:0005) |   ├─ArrayItemSyntax
//@[02:0005) |   | └─StringSyntax
//@[02:0005) |   |   └─Token(StringComplete) |'b'|
//@[05:0006) |   ├─Token(NewLine) |\n|
  'c'
//@[02:0005) |   ├─ArrayItemSyntax
//@[02:0005) |   | └─StringSyntax
//@[02:0005) |   |   └─Token(StringComplete) |'c'|
//@[05:0006) |   ├─Token(NewLine) |\n|
  'd'
//@[02:0005) |   ├─ArrayItemSyntax
//@[02:0005) |   | └─StringSyntax
//@[02:0005) |   |   └─Token(StringComplete) |'d'|
//@[05:0006) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
param emptyMetadata = 'empty!'
//@[00:0030) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0019) | ├─IdentifierSyntax
//@[06:0019) | | └─Token(Identifier) |emptyMetadata|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0030) | └─StringSyntax
//@[22:0030) |   └─Token(StringComplete) |'empty!'|
//@[30:0031) ├─Token(NewLine) |\n|
param description = 'descriptive description'
//@[00:0045) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |description|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0045) | └─StringSyntax
//@[20:0045) |   └─Token(StringComplete) |'descriptive description'|
//@[45:0046) ├─Token(NewLine) |\n|
param description2 = 'also descriptive'
//@[00:0039) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |description2|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0039) | └─StringSyntax
//@[21:0039) |   └─Token(StringComplete) |'also descriptive'|
//@[39:0040) ├─Token(NewLine) |\n|
param additionalMetadata = 'more metadata'
//@[00:0042) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0024) | ├─IdentifierSyntax
//@[06:0024) | | └─Token(Identifier) |additionalMetadata|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0042) | └─StringSyntax
//@[27:0042) |   └─Token(StringComplete) |'more metadata'|
//@[42:0043) ├─Token(NewLine) |\n|
param someParameter = 'three'
//@[00:0029) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0019) | ├─IdentifierSyntax
//@[06:0019) | | └─Token(Identifier) |someParameter|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0029) | └─StringSyntax
//@[22:0029) |   └─Token(StringComplete) |'three'|
//@[29:0030) ├─Token(NewLine) |\n|
param stringLiteral = 'abc'
//@[00:0027) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0019) | ├─IdentifierSyntax
//@[06:0019) | | └─Token(Identifier) |stringLiteral|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0027) | └─StringSyntax
//@[22:0027) |   └─Token(StringComplete) |'abc'|
//@[27:0028) ├─Token(NewLine) |\n|
param decoratedString = 'Apple'
//@[00:0031) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |decoratedString|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0031) | └─StringSyntax
//@[24:0031) |   └─Token(StringComplete) |'Apple'|
//@[31:0032) ├─Token(NewLine) |\n|
param stringfromEnvironmentVariables = readEnvironmentVariable('stringEnvVariableName')
//@[00:0087) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0036) | ├─IdentifierSyntax
//@[06:0036) | | └─Token(Identifier) |stringfromEnvironmentVariables|
//@[37:0038) | ├─Token(Assignment) |=|
//@[39:0087) | └─FunctionCallSyntax
//@[39:0062) |   ├─IdentifierSyntax
//@[39:0062) |   | └─Token(Identifier) |readEnvironmentVariable|
//@[62:0063) |   ├─Token(LeftParen) |(|
//@[63:0086) |   ├─FunctionArgumentSyntax
//@[63:0086) |   | └─StringSyntax
//@[63:0086) |   |   └─Token(StringComplete) |'stringEnvVariableName'|
//@[86:0087) |   └─Token(RightParen) |)|
//@[87:0088) ├─Token(NewLine) |\n|
param intfromEnvironmentVariables = int(readEnvironmentVariable('intEnvVariableName'))
//@[00:0086) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0033) | ├─IdentifierSyntax
//@[06:0033) | | └─Token(Identifier) |intfromEnvironmentVariables|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0086) | └─FunctionCallSyntax
//@[36:0039) |   ├─IdentifierSyntax
//@[36:0039) |   | └─Token(Identifier) |int|
//@[39:0040) |   ├─Token(LeftParen) |(|
//@[40:0085) |   ├─FunctionArgumentSyntax
//@[40:0085) |   | └─FunctionCallSyntax
//@[40:0063) |   |   ├─IdentifierSyntax
//@[40:0063) |   |   | └─Token(Identifier) |readEnvironmentVariable|
//@[63:0064) |   |   ├─Token(LeftParen) |(|
//@[64:0084) |   |   ├─FunctionArgumentSyntax
//@[64:0084) |   |   | └─StringSyntax
//@[64:0084) |   |   |   └─Token(StringComplete) |'intEnvVariableName'|
//@[84:0085) |   |   └─Token(RightParen) |)|
//@[85:0086) |   └─Token(RightParen) |)|
//@[86:0087) ├─Token(NewLine) |\n|
param boolfromEnvironmentVariables = bool(readEnvironmentVariable('boolEnvironmentVariable'))
//@[00:0093) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0034) | ├─IdentifierSyntax
//@[06:0034) | | └─Token(Identifier) |boolfromEnvironmentVariables|
//@[35:0036) | ├─Token(Assignment) |=|
//@[37:0093) | └─FunctionCallSyntax
//@[37:0041) |   ├─IdentifierSyntax
//@[37:0041) |   | └─Token(Identifier) |bool|
//@[41:0042) |   ├─Token(LeftParen) |(|
//@[42:0092) |   ├─FunctionArgumentSyntax
//@[42:0092) |   | └─FunctionCallSyntax
//@[42:0065) |   |   ├─IdentifierSyntax
//@[42:0065) |   |   | └─Token(Identifier) |readEnvironmentVariable|
//@[65:0066) |   |   ├─Token(LeftParen) |(|
//@[66:0091) |   |   ├─FunctionArgumentSyntax
//@[66:0091) |   |   | └─StringSyntax
//@[66:0091) |   |   |   └─Token(StringComplete) |'boolEnvironmentVariable'|
//@[91:0092) |   |   └─Token(RightParen) |)|
//@[92:0093) |   └─Token(RightParen) |)|
//@[93:0094) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
