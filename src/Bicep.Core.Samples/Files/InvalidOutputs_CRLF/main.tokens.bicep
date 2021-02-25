
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
//@[5:7) NewLine |\r\n|

//@[0:0) EndOfFile ||
