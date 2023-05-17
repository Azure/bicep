/*
//@[00:819) ProgramSyntax
This is a
multiline comment!
*/
//@[02:004) ├─Token(NewLine) |\n\n|

// This is a single line comment
//@[32:034) ├─Token(NewLine) |\n\n|

// using keyword for specifying a Bicep file
//@[44:045) ├─Token(NewLine) |\n|
using './main.bicep'
//@[00:020) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:020) | └─StringSyntax
//@[06:020) |   └─Token(StringComplete) |'./main.bicep'|
//@[20:022) ├─Token(NewLine) |\n\n|

// parameter assignment to literals
//@[35:036) ├─Token(NewLine) |\n|
param myString = 'hello world!!'
//@[00:032) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:014) | ├─IdentifierSyntax
//@[06:014) | | └─Token(Identifier) |myString|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:032) | └─StringSyntax
//@[17:032) |   └─Token(StringComplete) |'hello world!!'|
//@[32:033) ├─Token(NewLine) |\n|
param myInt = 42
//@[00:016) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:011) | ├─IdentifierSyntax
//@[06:011) | | └─Token(Identifier) |myInt|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:016) | └─IntegerLiteralSyntax
//@[14:016) |   └─Token(Integer) |42|
//@[16:017) ├─Token(NewLine) |\n|
param myBool = false
//@[00:020) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:012) | ├─IdentifierSyntax
//@[06:012) | | └─Token(Identifier) |myBool|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:020) | └─BooleanLiteralSyntax
//@[15:020) |   └─Token(FalseKeyword) |false|
//@[20:022) ├─Token(NewLine) |\n\n|

param numberOfVMs = 1
//@[00:021) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:017) | ├─IdentifierSyntax
//@[06:017) | | └─Token(Identifier) |numberOfVMs|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:021) | └─IntegerLiteralSyntax
//@[20:021) |   └─Token(Integer) |1|
//@[21:023) ├─Token(NewLine) |\n\n|

// parameter assignment to objects
//@[34:035) ├─Token(NewLine) |\n|
param password = 'strongPassword'
//@[00:033) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:014) | ├─IdentifierSyntax
//@[06:014) | | └─Token(Identifier) |password|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:033) | └─StringSyntax
//@[17:033) |   └─Token(StringComplete) |'strongPassword'|
//@[33:034) ├─Token(NewLine) |\n|
param secretObject = {
//@[00:061) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:018) | ├─IdentifierSyntax
//@[06:018) | | └─Token(Identifier) |secretObject|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:061) | └─ObjectSyntax
//@[21:022) |   ├─Token(LeftBrace) |{|
//@[22:023) |   ├─Token(NewLine) |\n|
  name : 'vm2'
//@[02:014) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:014) |   | └─StringSyntax
//@[09:014) |   |   └─Token(StringComplete) |'vm2'|
//@[14:015) |   ├─Token(NewLine) |\n|
  location : 'westus'
//@[02:021) |   ├─ObjectPropertySyntax
//@[02:010) |   | ├─IdentifierSyntax
//@[02:010) |   | | └─Token(Identifier) |location|
//@[11:012) |   | ├─Token(Colon) |:|
//@[13:021) |   | └─StringSyntax
//@[13:021) |   |   └─Token(StringComplete) |'westus'|
//@[21:022) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|
param storageSku = 'Standard_LRS'
//@[00:033) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:016) | ├─IdentifierSyntax
//@[06:016) | | └─Token(Identifier) |storageSku|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:033) | └─StringSyntax
//@[19:033) |   └─Token(StringComplete) |'Standard_LRS'|
//@[33:034) ├─Token(NewLine) |\n|
param storageName = 'myStorage'
//@[00:031) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:017) | ├─IdentifierSyntax
//@[06:017) | | └─Token(Identifier) |storageName|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:031) | └─StringSyntax
//@[20:031) |   └─Token(StringComplete) |'myStorage'|
//@[31:032) ├─Token(NewLine) |\n|
param someArray = [
//@[00:045) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:015) | ├─IdentifierSyntax
//@[06:015) | | └─Token(Identifier) |someArray|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:045) | └─ArraySyntax
//@[18:019) |   ├─Token(LeftSquare) |[|
//@[19:020) |   ├─Token(NewLine) |\n|
  'a'
//@[02:005) |   ├─ArrayItemSyntax
//@[02:005) |   | └─StringSyntax
//@[02:005) |   |   └─Token(StringComplete) |'a'|
//@[05:006) |   ├─Token(NewLine) |\n|
  'b'
//@[02:005) |   ├─ArrayItemSyntax
//@[02:005) |   | └─StringSyntax
//@[02:005) |   |   └─Token(StringComplete) |'b'|
//@[05:006) |   ├─Token(NewLine) |\n|
  'c'
//@[02:005) |   ├─ArrayItemSyntax
//@[02:005) |   | └─StringSyntax
//@[02:005) |   |   └─Token(StringComplete) |'c'|
//@[05:006) |   ├─Token(NewLine) |\n|
  'd'
//@[02:005) |   ├─ArrayItemSyntax
//@[02:005) |   | └─StringSyntax
//@[02:005) |   |   └─Token(StringComplete) |'d'|
//@[05:006) |   ├─Token(NewLine) |\n|
]
//@[00:001) |   └─Token(RightSquare) |]|
//@[01:002) ├─Token(NewLine) |\n|
param emptyMetadata = 'empty!'
//@[00:030) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:019) | ├─IdentifierSyntax
//@[06:019) | | └─Token(Identifier) |emptyMetadata|
//@[20:021) | ├─Token(Assignment) |=|
//@[22:030) | └─StringSyntax
//@[22:030) |   └─Token(StringComplete) |'empty!'|
//@[30:031) ├─Token(NewLine) |\n|
param description = 'descriptive description'
//@[00:045) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:017) | ├─IdentifierSyntax
//@[06:017) | | └─Token(Identifier) |description|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:045) | └─StringSyntax
//@[20:045) |   └─Token(StringComplete) |'descriptive description'|
//@[45:046) ├─Token(NewLine) |\n|
param description2 = 'also descriptive'
//@[00:039) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:018) | ├─IdentifierSyntax
//@[06:018) | | └─Token(Identifier) |description2|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:039) | └─StringSyntax
//@[21:039) |   └─Token(StringComplete) |'also descriptive'|
//@[39:040) ├─Token(NewLine) |\n|
param additionalMetadata = 'more metadata'
//@[00:042) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:024) | ├─IdentifierSyntax
//@[06:024) | | └─Token(Identifier) |additionalMetadata|
//@[25:026) | ├─Token(Assignment) |=|
//@[27:042) | └─StringSyntax
//@[27:042) |   └─Token(StringComplete) |'more metadata'|
//@[42:043) ├─Token(NewLine) |\n|
param someParameter = 'three'
//@[00:029) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:019) | ├─IdentifierSyntax
//@[06:019) | | └─Token(Identifier) |someParameter|
//@[20:021) | ├─Token(Assignment) |=|
//@[22:029) | └─StringSyntax
//@[22:029) |   └─Token(StringComplete) |'three'|
//@[29:030) ├─Token(NewLine) |\n|
param stringLiteral = 'abc'
//@[00:027) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:019) | ├─IdentifierSyntax
//@[06:019) | | └─Token(Identifier) |stringLiteral|
//@[20:021) | ├─Token(Assignment) |=|
//@[22:027) | └─StringSyntax
//@[22:027) |   └─Token(StringComplete) |'abc'|
//@[27:028) ├─Token(NewLine) |\n|
param decoratedString = 'Apple'
//@[00:031) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:021) | ├─IdentifierSyntax
//@[06:021) | | └─Token(Identifier) |decoratedString|
//@[22:023) | ├─Token(Assignment) |=|
//@[24:031) | └─StringSyntax
//@[24:031) |   └─Token(StringComplete) |'Apple'|
//@[31:032) ├─Token(NewLine) |\n|
param stringfromEnvironmentVariables = readEnvironmentVariable('envVariableName')
//@[00:081) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:036) | ├─IdentifierSyntax
//@[06:036) | | └─Token(Identifier) |stringfromEnvironmentVariables|
//@[37:038) | ├─Token(Assignment) |=|
//@[39:081) | └─FunctionCallSyntax
//@[39:062) |   ├─IdentifierSyntax
//@[39:062) |   | └─Token(Identifier) |readEnvironmentVariable|
//@[62:063) |   ├─Token(LeftParen) |(|
//@[63:080) |   ├─FunctionArgumentSyntax
//@[63:080) |   | └─StringSyntax
//@[63:080) |   |   └─Token(StringComplete) |'envVariableName'|
//@[80:081) |   └─Token(RightParen) |)|
//@[81:082) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
