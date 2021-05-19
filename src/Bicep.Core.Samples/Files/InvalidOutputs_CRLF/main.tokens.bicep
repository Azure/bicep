
//@[0:2) NewLine |\r\n|
// wrong declaration
//@[20:22) NewLine |\r\n|
bad
//@[0:3) Identifier |bad|
//@[3:7) NewLine |\r\n\r\n|

// incomplete #completionTest(7) -> empty
//@[41:43) NewLine |\r\n|
output 
//@[0:6) Identifier |output|
//@[7:11) NewLine |\r\n\r\n|

var testSymbol = 42
//@[0:3) Identifier |var|
//@[4:14) Identifier |testSymbol|
//@[15:16) Assignment |=|
//@[17:19) Integer |42|
//@[19:23) NewLine |\r\n\r\n|

// #completionTest(28,29) -> symbols
//@[36:38) NewLine |\r\n|
output missingValueAndType = 
//@[0:6) Identifier |output|
//@[7:26) Identifier |missingValueAndType|
//@[27:28) Assignment |=|
//@[29:33) NewLine |\r\n\r\n|

// #completionTest(28,29) -> symbols
//@[36:38) NewLine |\r\n|
output missingValue string = 
//@[0:6) Identifier |output|
//@[7:19) Identifier |missingValue|
//@[20:26) Identifier |string|
//@[27:28) Assignment |=|
//@[29:33) NewLine |\r\n\r\n|

// #completionTest(31,32) -> arrayPlusSymbols
//@[45:47) NewLine |\r\n|
output arrayCompletions array = 
//@[0:6) Identifier |output|
//@[7:23) Identifier |arrayCompletions|
//@[24:29) Identifier |array|
//@[30:31) Assignment |=|
//@[32:36) NewLine |\r\n\r\n|

// #completionTest(33,34) -> objectPlusSymbols
//@[46:48) NewLine |\r\n|
output objectCompletions object = 
//@[0:6) Identifier |output|
//@[7:24) Identifier |objectCompletions|
//@[25:31) Identifier |object|
//@[32:33) Assignment |=|
//@[34:38) NewLine |\r\n\r\n|

// #completionTest(29,30) -> boolPlusSymbols
//@[44:46) NewLine |\r\n|
output boolCompletions bool = 
//@[0:6) Identifier |output|
//@[7:22) Identifier |boolCompletions|
//@[23:27) Identifier |bool|
//@[28:29) Assignment |=|
//@[30:34) NewLine |\r\n\r\n|

output foo
//@[0:6) Identifier |output|
//@[7:10) Identifier |foo|
//@[10:14) NewLine |\r\n\r\n|

// space after identifier #completionTest(20) -> outputTypes
//@[60:62) NewLine |\r\n|
output spaceAfterId 
//@[0:6) Identifier |output|
//@[7:19) Identifier |spaceAfterId|
//@[20:24) NewLine |\r\n\r\n|

// #completionTest(25) -> outputTypes
//@[37:39) NewLine |\r\n|
output spacesAfterCursor  
//@[0:6) Identifier |output|
//@[7:24) Identifier |spacesAfterCursor|
//@[26:30) NewLine |\r\n\r\n|

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
//@[62:64) NewLine |\r\n|
output partialType obj
//@[0:6) Identifier |output|
//@[7:18) Identifier |partialType|
//@[19:22) Identifier |obj|
//@[22:26) NewLine |\r\n\r\n|

// malformed identifier
//@[23:25) NewLine |\r\n|
output 2
//@[0:6) Identifier |output|
//@[7:8) Integer |2|
//@[8:12) NewLine |\r\n\r\n|

// malformed type
//@[17:19) NewLine |\r\n|
output malformedType 3
//@[0:6) Identifier |output|
//@[7:20) Identifier |malformedType|
//@[21:22) Integer |3|
//@[22:26) NewLine |\r\n\r\n|

// malformed type but type check should still happen
//@[52:54) NewLine |\r\n|
output malformedType2 3 = 2 + null
//@[0:6) Identifier |output|
//@[7:21) Identifier |malformedType2|
//@[22:23) Integer |3|
//@[24:25) Assignment |=|
//@[26:27) Integer |2|
//@[28:29) Plus |+|
//@[30:34) NullKeyword |null|
//@[34:38) NewLine |\r\n\r\n|

// malformed type assignment
//@[28:30) NewLine |\r\n|
output malformedAssignment 2 = 2
//@[0:6) Identifier |output|
//@[7:26) Identifier |malformedAssignment|
//@[27:28) Integer |2|
//@[29:30) Assignment |=|
//@[31:32) Integer |2|
//@[32:36) NewLine |\r\n\r\n|

// malformed type before assignment
//@[35:37) NewLine |\r\n|
output lol 2 = true
//@[0:6) Identifier |output|
//@[7:10) Identifier |lol|
//@[11:12) Integer |2|
//@[13:14) Assignment |=|
//@[15:19) TrueKeyword |true|
//@[19:23) NewLine |\r\n\r\n|

// wrong type + missing value
//@[29:31) NewLine |\r\n|
output foo fluffy
//@[0:6) Identifier |output|
//@[7:10) Identifier |foo|
//@[11:17) Identifier |fluffy|
//@[17:21) NewLine |\r\n\r\n|

// missing value
//@[16:18) NewLine |\r\n|
output foo string
//@[0:6) Identifier |output|
//@[7:10) Identifier |foo|
//@[11:17) Identifier |string|
//@[17:21) NewLine |\r\n\r\n|

// missing value
//@[16:18) NewLine |\r\n|
output foo string =
//@[0:6) Identifier |output|
//@[7:10) Identifier |foo|
//@[11:17) Identifier |string|
//@[18:19) Assignment |=|
//@[19:23) NewLine |\r\n\r\n|

// wrong string output values
//@[29:31) NewLine |\r\n|
output str string = true
//@[0:6) Identifier |output|
//@[7:10) Identifier |str|
//@[11:17) Identifier |string|
//@[18:19) Assignment |=|
//@[20:24) TrueKeyword |true|
//@[24:26) NewLine |\r\n|
output str string = false
//@[0:6) Identifier |output|
//@[7:10) Identifier |str|
//@[11:17) Identifier |string|
//@[18:19) Assignment |=|
//@[20:25) FalseKeyword |false|
//@[25:27) NewLine |\r\n|
output str string = [
//@[0:6) Identifier |output|
//@[7:10) Identifier |str|
//@[11:17) Identifier |string|
//@[18:19) Assignment |=|
//@[20:21) LeftSquare |[|
//@[21:23) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
output str string = {
//@[0:6) Identifier |output|
//@[7:10) Identifier |str|
//@[11:17) Identifier |string|
//@[18:19) Assignment |=|
//@[20:21) LeftBrace |{|
//@[21:23) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
output str string = 52
//@[0:6) Identifier |output|
//@[7:10) Identifier |str|
//@[11:17) Identifier |string|
//@[18:19) Assignment |=|
//@[20:22) Integer |52|
//@[22:26) NewLine |\r\n\r\n|

// wrong int output values
//@[26:28) NewLine |\r\n|
output i int = true
//@[0:6) Identifier |output|
//@[7:8) Identifier |i|
//@[9:12) Identifier |int|
//@[13:14) Assignment |=|
//@[15:19) TrueKeyword |true|
//@[19:21) NewLine |\r\n|
output i int = false
//@[0:6) Identifier |output|
//@[7:8) Identifier |i|
//@[9:12) Identifier |int|
//@[13:14) Assignment |=|
//@[15:20) FalseKeyword |false|
//@[20:22) NewLine |\r\n|
output i int = [
//@[0:6) Identifier |output|
//@[7:8) Identifier |i|
//@[9:12) Identifier |int|
//@[13:14) Assignment |=|
//@[15:16) LeftSquare |[|
//@[16:18) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
output i int = }
//@[0:6) Identifier |output|
//@[7:8) Identifier |i|
//@[9:12) Identifier |int|
//@[13:14) Assignment |=|
//@[15:16) RightBrace |}|
//@[16:18) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
output i int = 'test'
//@[0:6) Identifier |output|
//@[7:8) Identifier |i|
//@[9:12) Identifier |int|
//@[13:14) Assignment |=|
//@[15:21) StringComplete |'test'|
//@[21:25) NewLine |\r\n\r\n|

// wrong bool output values
//@[27:29) NewLine |\r\n|
output b bool = [
//@[0:6) Identifier |output|
//@[7:8) Identifier |b|
//@[9:13) Identifier |bool|
//@[14:15) Assignment |=|
//@[16:17) LeftSquare |[|
//@[17:19) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
output b bool = {
//@[0:6) Identifier |output|
//@[7:8) Identifier |b|
//@[9:13) Identifier |bool|
//@[14:15) Assignment |=|
//@[16:17) LeftBrace |{|
//@[17:19) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
output b bool = 32
//@[0:6) Identifier |output|
//@[7:8) Identifier |b|
//@[9:13) Identifier |bool|
//@[14:15) Assignment |=|
//@[16:18) Integer |32|
//@[18:20) NewLine |\r\n|
output b bool = 'str'
//@[0:6) Identifier |output|
//@[7:8) Identifier |b|
//@[9:13) Identifier |bool|
//@[14:15) Assignment |=|
//@[16:21) StringComplete |'str'|
//@[21:25) NewLine |\r\n\r\n|

// wrong array output values
//@[28:30) NewLine |\r\n|
output arr array = 32
//@[0:6) Identifier |output|
//@[7:10) Identifier |arr|
//@[11:16) Identifier |array|
//@[17:18) Assignment |=|
//@[19:21) Integer |32|
//@[21:23) NewLine |\r\n|
output arr array = true
//@[0:6) Identifier |output|
//@[7:10) Identifier |arr|
//@[11:16) Identifier |array|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[23:25) NewLine |\r\n|
output arr array = false
//@[0:6) Identifier |output|
//@[7:10) Identifier |arr|
//@[11:16) Identifier |array|
//@[17:18) Assignment |=|
//@[19:24) FalseKeyword |false|
//@[24:26) NewLine |\r\n|
output arr array = {
//@[0:6) Identifier |output|
//@[7:10) Identifier |arr|
//@[11:16) Identifier |array|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
output arr array = 'str'
//@[0:6) Identifier |output|
//@[7:10) Identifier |arr|
//@[11:16) Identifier |array|
//@[17:18) Assignment |=|
//@[19:24) StringComplete |'str'|
//@[24:28) NewLine |\r\n\r\n|

// wrong object output values
//@[29:31) NewLine |\r\n|
output o object = 32
//@[0:6) Identifier |output|
//@[7:8) Identifier |o|
//@[9:15) Identifier |object|
//@[16:17) Assignment |=|
//@[18:20) Integer |32|
//@[20:22) NewLine |\r\n|
output o object = true
//@[0:6) Identifier |output|
//@[7:8) Identifier |o|
//@[9:15) Identifier |object|
//@[16:17) Assignment |=|
//@[18:22) TrueKeyword |true|
//@[22:24) NewLine |\r\n|
output o object = false
//@[0:6) Identifier |output|
//@[7:8) Identifier |o|
//@[9:15) Identifier |object|
//@[16:17) Assignment |=|
//@[18:23) FalseKeyword |false|
//@[23:25) NewLine |\r\n|
output o object = [
//@[0:6) Identifier |output|
//@[7:8) Identifier |o|
//@[9:15) Identifier |object|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:21) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
output o object = 'str'
//@[0:6) Identifier |output|
//@[7:8) Identifier |o|
//@[9:15) Identifier |object|
//@[16:17) Assignment |=|
//@[18:23) StringComplete |'str'|
//@[23:27) NewLine |\r\n\r\n|

// a few expression cases
//@[25:27) NewLine |\r\n|
output exp string = 2 + 3
//@[0:6) Identifier |output|
//@[7:10) Identifier |exp|
//@[11:17) Identifier |string|
//@[18:19) Assignment |=|
//@[20:21) Integer |2|
//@[22:23) Plus |+|
//@[24:25) Integer |3|
//@[25:27) NewLine |\r\n|
output union string = true ? 's' : 1
//@[0:6) Identifier |output|
//@[7:12) Identifier |union|
//@[13:19) Identifier |string|
//@[20:21) Assignment |=|
//@[22:26) TrueKeyword |true|
//@[27:28) Question |?|
//@[29:32) StringComplete |'s'|
//@[33:34) Colon |:|
//@[35:36) Integer |1|
//@[36:38) NewLine |\r\n|
output bad int = true && !4
//@[0:6) Identifier |output|
//@[7:10) Identifier |bad|
//@[11:14) Identifier |int|
//@[15:16) Assignment |=|
//@[17:21) TrueKeyword |true|
//@[22:24) LogicalAnd |&&|
//@[25:26) Exclamation |!|
//@[26:27) Integer |4|
//@[27:29) NewLine |\r\n|
output deeper bool = true ? -true : (14 && 's') + 10
//@[0:6) Identifier |output|
//@[7:13) Identifier |deeper|
//@[14:18) Identifier |bool|
//@[19:20) Assignment |=|
//@[21:25) TrueKeyword |true|
//@[26:27) Question |?|
//@[28:29) Minus |-|
//@[29:33) TrueKeyword |true|
//@[34:35) Colon |:|
//@[36:37) LeftParen |(|
//@[37:39) Integer |14|
//@[40:42) LogicalAnd |&&|
//@[43:46) StringComplete |'s'|
//@[46:47) RightParen |)|
//@[48:49) Plus |+|
//@[50:52) Integer |10|
//@[52:56) NewLine |\r\n\r\n|

output myOutput string = 'hello'
//@[0:6) Identifier |output|
//@[7:15) Identifier |myOutput|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:32) StringComplete |'hello'|
//@[32:34) NewLine |\r\n|
var attemptToReferenceAnOutput = myOutput
//@[0:3) Identifier |var|
//@[4:30) Identifier |attemptToReferenceAnOutput|
//@[31:32) Assignment |=|
//@[33:41) Identifier |myOutput|
//@[41:45) NewLine |\r\n\r\n|

@sys.maxValue(20)
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:13) Identifier |maxValue|
//@[13:14) LeftParen |(|
//@[14:16) Integer |20|
//@[16:17) RightParen |)|
//@[17:19) NewLine |\r\n|
@minValue(10)
//@[0:1) At |@|
//@[1:9) Identifier |minValue|
//@[9:10) LeftParen |(|
//@[10:12) Integer |10|
//@[12:13) RightParen |)|
//@[13:15) NewLine |\r\n|
output notAttachableDecorators int = 32
//@[0:6) Identifier |output|
//@[7:30) Identifier |notAttachableDecorators|
//@[31:34) Identifier |int|
//@[35:36) Assignment |=|
//@[37:39) Integer |32|
//@[39:43) NewLine |\r\n\r\n|

// nested loops inside output loops are not supported
//@[53:55) NewLine |\r\n|
output noNestedLoops array = [for thing in things: {
//@[0:6) Identifier |output|
//@[7:20) Identifier |noNestedLoops|
//@[21:26) Identifier |array|
//@[27:28) Assignment |=|
//@[29:30) LeftSquare |[|
//@[30:33) Identifier |for|
//@[34:39) Identifier |thing|
//@[40:42) Identifier |in|
//@[43:49) Identifier |things|
//@[49:50) Colon |:|
//@[51:52) LeftBrace |{|
//@[52:54) NewLine |\r\n|
  something: [
//@[2:11) Identifier |something|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    [for thing in things: true]
//@[4:5) LeftSquare |[|
//@[5:8) Identifier |for|
//@[9:14) Identifier |thing|
//@[15:17) Identifier |in|
//@[18:24) Identifier |things|
//@[24:25) Colon |:|
//@[26:30) TrueKeyword |true|
//@[30:31) RightSquare |]|
//@[31:33) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// loops in inner properties inside outputs are not supported
//@[61:63) NewLine |\r\n|
output noInnerLoopsInOutputs object = {
//@[0:6) Identifier |output|
//@[7:28) Identifier |noInnerLoopsInOutputs|
//@[29:35) Identifier |object|
//@[36:37) Assignment |=|
//@[38:39) LeftBrace |{|
//@[39:41) NewLine |\r\n|
  a: [for i in range(0,10): i]
//@[2:3) Identifier |a|
//@[3:4) Colon |:|
//@[5:6) LeftSquare |[|
//@[6:9) Identifier |for|
//@[10:11) Identifier |i|
//@[12:14) Identifier |in|
//@[15:20) Identifier |range|
//@[20:21) LeftParen |(|
//@[21:22) Integer |0|
//@[22:23) Comma |,|
//@[23:25) Integer |10|
//@[25:26) RightParen |)|
//@[26:27) Colon |:|
//@[28:29) Identifier |i|
//@[29:30) RightSquare |]|
//@[30:32) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
output noInnerLoopsInOutputs2 object = {
//@[0:6) Identifier |output|
//@[7:29) Identifier |noInnerLoopsInOutputs2|
//@[30:36) Identifier |object|
//@[37:38) Assignment |=|
//@[39:40) LeftBrace |{|
//@[40:42) NewLine |\r\n|
  a: [for i in range(0,10): {
//@[2:3) Identifier |a|
//@[3:4) Colon |:|
//@[5:6) LeftSquare |[|
//@[6:9) Identifier |for|
//@[10:11) Identifier |i|
//@[12:14) Identifier |in|
//@[15:20) Identifier |range|
//@[20:21) LeftParen |(|
//@[21:22) Integer |0|
//@[22:23) Comma |,|
//@[23:25) Integer |10|
//@[25:26) RightParen |)|
//@[26:27) Colon |:|
//@[28:29) LeftBrace |{|
//@[29:31) NewLine |\r\n|
    b: [for j in range(0,10): i+j]
//@[4:5) Identifier |b|
//@[5:6) Colon |:|
//@[7:8) LeftSquare |[|
//@[8:11) Identifier |for|
//@[12:13) Identifier |j|
//@[14:16) Identifier |in|
//@[17:22) Identifier |range|
//@[22:23) LeftParen |(|
//@[23:24) Integer |0|
//@[24:25) Comma |,|
//@[25:27) Integer |10|
//@[27:28) RightParen |)|
//@[28:29) Colon |:|
//@[30:31) Identifier |i|
//@[31:32) Plus |+|
//@[32:33) Identifier |j|
//@[33:34) RightSquare |]|
//@[34:36) NewLine |\r\n|
  }]
//@[2:3) RightBrace |}|
//@[3:4) RightSquare |]|
//@[4:6) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

//KeyVault Secret Reference
//@[27:29) NewLine |\r\n|
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[0:8) Identifier |resource|
//@[9:11) Identifier |kv|
//@[12:50) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[51:59) Identifier |existing|
//@[60:61) Assignment |=|
//@[62:63) LeftBrace |{|
//@[63:65) NewLine |\r\n|
  name: 'testkeyvault'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'testkeyvault'|
//@[22:24) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output keyVaultSecretOutput string = kv.getSecret('mySecret')
//@[0:6) Identifier |output|
//@[7:27) Identifier |keyVaultSecretOutput|
//@[28:34) Identifier |string|
//@[35:36) Assignment |=|
//@[37:39) Identifier |kv|
//@[39:40) Dot |.|
//@[40:49) Identifier |getSecret|
//@[49:50) LeftParen |(|
//@[50:60) StringComplete |'mySecret'|
//@[60:61) RightParen |)|
//@[61:63) NewLine |\r\n|
output keyVaultSecretInterpolatedOutput string = '${kv.getSecret('mySecret')}'
//@[0:6) Identifier |output|
//@[7:39) Identifier |keyVaultSecretInterpolatedOutput|
//@[40:46) Identifier |string|
//@[47:48) Assignment |=|
//@[49:52) StringLeftPiece |'${|
//@[52:54) Identifier |kv|
//@[54:55) Dot |.|
//@[55:64) Identifier |getSecret|
//@[64:65) LeftParen |(|
//@[65:75) StringComplete |'mySecret'|
//@[75:76) RightParen |)|
//@[76:78) StringRightPiece |}'|
//@[78:80) NewLine |\r\n|
output keyVaultSecretObjectOutput object = {
//@[0:6) Identifier |output|
//@[7:33) Identifier |keyVaultSecretObjectOutput|
//@[34:40) Identifier |object|
//@[41:42) Assignment |=|
//@[43:44) LeftBrace |{|
//@[44:46) NewLine |\r\n|
  secret: kv.getSecret('mySecret')
//@[2:8) Identifier |secret|
//@[8:9) Colon |:|
//@[10:12) Identifier |kv|
//@[12:13) Dot |.|
//@[13:22) Identifier |getSecret|
//@[22:23) LeftParen |(|
//@[23:33) StringComplete |'mySecret'|
//@[33:34) RightParen |)|
//@[34:36) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
output keyVaultSecretArrayOutput array = [
//@[0:6) Identifier |output|
//@[7:32) Identifier |keyVaultSecretArrayOutput|
//@[33:38) Identifier |array|
//@[39:40) Assignment |=|
//@[41:42) LeftSquare |[|
//@[42:44) NewLine |\r\n|
  kv.getSecret('mySecret')
//@[2:4) Identifier |kv|
//@[4:5) Dot |.|
//@[5:14) Identifier |getSecret|
//@[14:15) LeftParen |(|
//@[15:25) StringComplete |'mySecret'|
//@[25:26) RightParen |)|
//@[26:28) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
output keyVaultSecretArrayInterpolatedOutput array = [
//@[0:6) Identifier |output|
//@[7:44) Identifier |keyVaultSecretArrayInterpolatedOutput|
//@[45:50) Identifier |array|
//@[51:52) Assignment |=|
//@[53:54) LeftSquare |[|
//@[54:56) NewLine |\r\n|
  '${kv.getSecret('mySecret')}'
//@[2:5) StringLeftPiece |'${|
//@[5:7) Identifier |kv|
//@[7:8) Dot |.|
//@[8:17) Identifier |getSecret|
//@[17:18) LeftParen |(|
//@[18:28) StringComplete |'mySecret'|
//@[28:29) RightParen |)|
//@[29:31) StringRightPiece |}'|
//@[31:33) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

// WARNING!!!!! dangling decorators
//@[35:39) NewLine |\r\n\r\n|

// #completionTest(1) -> decoratorsPlusNamespace
//@[48:50) NewLine |\r\n|
@
//@[0:1) At |@|
//@[1:3) NewLine |\r\n|
// #completionTest(5) -> decorators
//@[35:37) NewLine |\r\n|
@sys.
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:9) NewLine |\r\n\r\n|

// WARNING!!!!! dangling decorators - to make sure the tests work, please do not add contents after this line 
//@[110:110) EndOfFile ||
