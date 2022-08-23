/*
This is a
multiline comment!
*/
//@[02:04) NewLine |\n\n|

// This is a single line comment
//@[32:34) NewLine |\n\n|

// using keyword for specifying a Bicep file
//@[44:45) NewLine |\n|
using './main.bicep'
//@[00:05) Identifier |using|
//@[06:20) StringComplete |'./main.bicep'|
//@[20:22) NewLine |\n\n|

// parameter assignment to literals
//@[35:36) NewLine |\n|
param myString = 'hello world!!'
//@[00:05) Identifier |param|
//@[06:14) Identifier |myString|
//@[15:16) Assignment |=|
//@[17:32) StringComplete |'hello world!!'|
//@[32:33) NewLine |\n|
param myInt = 42
//@[00:05) Identifier |param|
//@[06:11) Identifier |myInt|
//@[12:13) Assignment |=|
//@[14:16) Integer |42|
//@[16:17) NewLine |\n|
param myBool = true
//@[00:05) Identifier |param|
//@[06:12) Identifier |myBool|
//@[13:14) Assignment |=|
//@[15:19) TrueKeyword |true|
//@[19:21) NewLine |\n\n|

// parameter assignment to objects
//@[34:35) NewLine |\n|
param password = 'strongPassword'
//@[00:05) Identifier |param|
//@[06:14) Identifier |password|
//@[15:16) Assignment |=|
//@[17:33) StringComplete |'strongPassword'|
//@[33:34) NewLine |\n|
param secretObject = {
//@[00:05) Identifier |param|
//@[06:18) Identifier |secretObject|
//@[19:20) Assignment |=|
//@[21:22) LeftBrace |{|
//@[22:23) NewLine |\n|
    name : 'vm2'
//@[04:08) Identifier |name|
//@[09:10) Colon |:|
//@[11:16) StringComplete |'vm2'|
//@[16:17) NewLine |\n|
    location : 'westus'
//@[04:12) Identifier |location|
//@[13:14) Colon |:|
//@[15:23) StringComplete |'westus'|
//@[23:24) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
param storageSku = 'Standard_LRS'
//@[00:05) Identifier |param|
//@[06:16) Identifier |storageSku|
//@[17:18) Assignment |=|
//@[19:33) StringComplete |'Standard_LRS'|
//@[33:34) NewLine |\n|
param storageName = 'myStorage'
//@[00:05) Identifier |param|
//@[06:17) Identifier |storageName|
//@[18:19) Assignment |=|
//@[20:31) StringComplete |'myStorage'|
//@[31:32) NewLine |\n|
param someArray = [
//@[00:05) Identifier |param|
//@[06:15) Identifier |someArray|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:20) NewLine |\n|
    'a'
//@[04:07) StringComplete |'a'|
//@[07:08) NewLine |\n|
    'b'
//@[04:07) StringComplete |'b'|
//@[07:08) NewLine |\n|
    'c'
//@[04:07) StringComplete |'c'|
//@[07:08) NewLine |\n|
    'd'
//@[04:07) StringComplete |'d'|
//@[07:08) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
param emptyMetadata = 'empty!'
//@[00:05) Identifier |param|
//@[06:19) Identifier |emptyMetadata|
//@[20:21) Assignment |=|
//@[22:30) StringComplete |'empty!'|
//@[30:31) NewLine |\n|
param description = 'descriptive description'
//@[00:05) Identifier |param|
//@[06:17) Identifier |description|
//@[18:19) Assignment |=|
//@[20:45) StringComplete |'descriptive description'|
//@[45:46) NewLine |\n|
param description2 = 'also descriptive'
//@[00:05) Identifier |param|
//@[06:18) Identifier |description2|
//@[19:20) Assignment |=|
//@[21:39) StringComplete |'also descriptive'|
//@[39:40) NewLine |\n|
param additionalMetadata = 'more metadata'
//@[00:05) Identifier |param|
//@[06:24) Identifier |additionalMetadata|
//@[25:26) Assignment |=|
//@[27:42) StringComplete |'more metadata'|
//@[42:43) NewLine |\n|
param someParameter = 'three'
//@[00:05) Identifier |param|
//@[06:19) Identifier |someParameter|
//@[20:21) Assignment |=|
//@[22:29) StringComplete |'three'|
//@[29:30) NewLine |\n|
param stringLiteral = 'abc'
//@[00:05) Identifier |param|
//@[06:19) Identifier |stringLiteral|
//@[20:21) Assignment |=|
//@[22:27) StringComplete |'abc'|
//@[27:28) NewLine |\n|
param decoratedString = 'Apple'
//@[00:05) Identifier |param|
//@[06:21) Identifier |decoratedString|
//@[22:23) Assignment |=|
//@[24:31) StringComplete |'Apple'|
//@[31:32) NewLine |\n|

//@[00:00) EndOfFile ||
