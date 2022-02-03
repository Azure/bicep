var singleLineFunctionNoCommas = concat('abc' 'def')
//@[0:3) Identifier |var|
//@[4:30) Identifier |singleLineFunctionNoCommas|
//@[31:32) Assignment |=|
//@[33:39) Identifier |concat|
//@[39:40) LeftParen |(|
//@[40:45) StringComplete |'abc'|
//@[46:51) StringComplete |'def'|
//@[51:52) RightParen |)|
//@[52:54) NewLine |\n\n|

var multiLineFunctionTrailingComma = concat(
//@[0:3) Identifier |var|
//@[4:34) Identifier |multiLineFunctionTrailingComma|
//@[35:36) Assignment |=|
//@[37:43) Identifier |concat|
//@[43:44) LeftParen |(|
//@[44:45) NewLine |\n|
  'abc',
//@[2:7) StringComplete |'abc'|
//@[7:8) Comma |,|
//@[8:9) NewLine |\n|
  'def',
//@[2:7) StringComplete |'def'|
//@[7:8) Comma |,|
//@[8:9) NewLine |\n|
)
//@[0:1) RightParen |)|
//@[1:3) NewLine |\n\n|

var singleLineArrayNoCommas = ['abc' 'def']
//@[0:3) Identifier |var|
//@[4:27) Identifier |singleLineArrayNoCommas|
//@[28:29) Assignment |=|
//@[30:31) LeftSquare |[|
//@[31:36) StringComplete |'abc'|
//@[37:42) StringComplete |'def'|
//@[42:43) RightSquare |]|
//@[43:45) NewLine |\n\n|

var multiLineArrayMultipleCommas = [
//@[0:3) Identifier |var|
//@[4:32) Identifier |multiLineArrayMultipleCommas|
//@[33:34) Assignment |=|
//@[35:36) LeftSquare |[|
//@[36:37) NewLine |\n|
  'abc',,
//@[2:7) StringComplete |'abc'|
//@[7:8) Comma |,|
//@[8:9) Comma |,|
//@[9:10) NewLine |\n|
  'def',,,
//@[2:7) StringComplete |'def'|
//@[7:8) Comma |,|
//@[8:9) Comma |,|
//@[9:10) Comma |,|
//@[10:11) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:4) NewLine |\n\n\n|


var singleLineObjectNoCommas = { abc: 'def' ghi: 'jkl'}
//@[0:3) Identifier |var|
//@[4:28) Identifier |singleLineObjectNoCommas|
//@[29:30) Assignment |=|
//@[31:32) LeftBrace |{|
//@[33:36) Identifier |abc|
//@[36:37) Colon |:|
//@[38:43) StringComplete |'def'|
//@[44:47) Identifier |ghi|
//@[47:48) Colon |:|
//@[49:54) StringComplete |'jkl'|
//@[54:55) RightBrace |}|
//@[55:56) NewLine |\n|
var multiLineObjectMultipleCommas = {
//@[0:3) Identifier |var|
//@[4:33) Identifier |multiLineObjectMultipleCommas|
//@[34:35) Assignment |=|
//@[36:37) LeftBrace |{|
//@[37:38) NewLine |\n|
  abc: 'def',,,
//@[2:5) Identifier |abc|
//@[5:6) Colon |:|
//@[7:12) StringComplete |'def'|
//@[12:13) Comma |,|
//@[13:14) Comma |,|
//@[14:15) Comma |,|
//@[15:16) NewLine |\n|
  ghi: 'jkl',,
//@[2:5) Identifier |ghi|
//@[5:6) Colon |:|
//@[7:12) StringComplete |'jkl'|
//@[12:13) Comma |,|
//@[13:14) Comma |,|
//@[14:15) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
