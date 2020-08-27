
//@[0:1) NewLine |\n|
// an int variable
//@[18:19) NewLine |\n|
var myInt = 42
//@[0:3) Identifier |var|
//@[4:9) Identifier |myInt|
//@[10:11) Assignment |=|
//@[12:14) Number |42|
//@[14:16) NewLine |\n\n|

// a string variable
//@[20:21) NewLine |\n|
var myStr = 'str'
//@[0:3) Identifier |var|
//@[4:9) Identifier |myStr|
//@[10:11) Assignment |=|
//@[12:17) StringComplete |'str'|
//@[17:18) NewLine |\n|
var curliesWithNoInterp = '}{1}{'
//@[0:3) Identifier |var|
//@[4:23) Identifier |curliesWithNoInterp|
//@[24:25) Assignment |=|
//@[26:33) StringComplete |'}{1}{'|
//@[33:34) NewLine |\n|
var interp1 = 'abc${123}def'
//@[0:3) Identifier |var|
//@[4:11) Identifier |interp1|
//@[12:13) Assignment |=|
//@[14:20) StringLeftPiece |'abc${|
//@[20:23) Number |123|
//@[23:28) StringRightPiece |}def'|
//@[28:29) NewLine |\n|
var interp2 = '${123}def'
//@[0:3) Identifier |var|
//@[4:11) Identifier |interp2|
//@[12:13) Assignment |=|
//@[14:17) StringLeftPiece |'${|
//@[17:20) Number |123|
//@[20:25) StringRightPiece |}def'|
//@[25:26) NewLine |\n|
var interp3 = 'abc${123}'
//@[0:3) Identifier |var|
//@[4:11) Identifier |interp3|
//@[12:13) Assignment |=|
//@[14:20) StringLeftPiece |'abc${|
//@[20:23) Number |123|
//@[23:25) StringRightPiece |}'|
//@[25:26) NewLine |\n|
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[0:3) Identifier |var|
//@[4:11) Identifier |interp4|
//@[12:13) Assignment |=|
//@[14:20) StringLeftPiece |'abc${|
//@[20:23) Number |123|
//@[23:26) StringMiddlePiece |}${|
//@[26:29) Number |456|
//@[29:36) StringMiddlePiece |}jk$l${|
//@[36:39) Number |789|
//@[39:43) StringRightPiece |}p$'|
//@[43:44) NewLine |\n|
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[0:3) Identifier |var|
//@[4:16) Identifier |doubleInterp|
//@[17:18) Assignment |=|
//@[19:25) StringLeftPiece |'abc${|
//@[25:31) StringLeftPiece |'def${|
//@[31:34) Number |123|
//@[34:36) StringRightPiece |}'|
//@[36:40) StringMiddlePiece |}_${|
//@[40:43) StringLeftPiece |'${|
//@[43:46) Number |456|
//@[46:49) StringMiddlePiece |}${|
//@[49:52) Number |789|
//@[52:54) StringRightPiece |}'|
//@[54:56) StringRightPiece |}'|
//@[56:57) NewLine |\n|
var curliesInInterp = '{${123}{0}${true}}'
//@[0:3) Identifier |var|
//@[4:19) Identifier |curliesInInterp|
//@[20:21) Assignment |=|
//@[22:26) StringLeftPiece |'{${|
//@[26:29) Number |123|
//@[29:35) StringMiddlePiece |}{0}${|
//@[35:39) TrueKeyword |true|
//@[39:42) StringRightPiece |}}'|
//@[42:44) NewLine |\n\n|

// verify correct bracket escaping
//@[34:35) NewLine |\n|
var bracketInTheMiddle = 'a[b]'
//@[0:3) Identifier |var|
//@[4:22) Identifier |bracketInTheMiddle|
//@[23:24) Assignment |=|
//@[25:31) StringComplete |'a[b]'|
//@[31:32) NewLine |\n|
var bracketAtBeginning = '[test'
//@[0:3) Identifier |var|
//@[4:22) Identifier |bracketAtBeginning|
//@[23:24) Assignment |=|
//@[25:32) StringComplete |'[test'|
//@[32:33) NewLine |\n|
var enclosingBrackets = '[test]'
//@[0:3) Identifier |var|
//@[4:21) Identifier |enclosingBrackets|
//@[22:23) Assignment |=|
//@[24:32) StringComplete |'[test]'|
//@[32:33) NewLine |\n|
var emptyJsonArray = '[]'
//@[0:3) Identifier |var|
//@[4:18) Identifier |emptyJsonArray|
//@[19:20) Assignment |=|
//@[21:25) StringComplete |'[]'|
//@[25:26) NewLine |\n|
var interpolatedBrackets = '[${myInt}]'
//@[0:3) Identifier |var|
//@[4:24) Identifier |interpolatedBrackets|
//@[25:26) Assignment |=|
//@[27:31) StringLeftPiece |'[${|
//@[31:36) Identifier |myInt|
//@[36:39) StringRightPiece |}]'|
//@[39:40) NewLine |\n|
var nestedBrackets = '[test[]test2]'
//@[0:3) Identifier |var|
//@[4:18) Identifier |nestedBrackets|
//@[19:20) Assignment |=|
//@[21:36) StringComplete |'[test[]test2]'|
//@[36:37) NewLine |\n|
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[0:3) Identifier |var|
//@[4:30) Identifier |nestedInterpolatedBrackets|
//@[31:32) Assignment |=|
//@[33:37) StringLeftPiece |'[${|
//@[37:51) Identifier |emptyJsonArray|
//@[51:54) StringRightPiece |}]'|
//@[54:55) NewLine |\n|
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[0:3) Identifier |var|
//@[4:29) Identifier |bracketStringInExpression|
//@[30:31) Assignment |=|
//@[32:38) Identifier |concat|
//@[38:39) LeftParen |(|
//@[39:42) StringComplete |'['|
//@[42:43) Comma |,|
//@[44:54) StringComplete |'\'test\''|
//@[54:55) Comma |,|
//@[55:58) StringComplete |']'|
//@[58:59) RightParen |)|
//@[59:61) NewLine |\n\n|

// booleans
//@[11:12) NewLine |\n|
var myTruth = true
//@[0:3) Identifier |var|
//@[4:11) Identifier |myTruth|
//@[12:13) Assignment |=|
//@[14:18) TrueKeyword |true|
//@[18:19) NewLine |\n|
var myFalsehood = false
//@[0:3) Identifier |var|
//@[4:15) Identifier |myFalsehood|
//@[16:17) Assignment |=|
//@[18:23) FalseKeyword |false|
//@[23:25) NewLine |\n\n|

var myEmptyObj = { }
//@[0:3) Identifier |var|
//@[4:14) Identifier |myEmptyObj|
//@[15:16) Assignment |=|
//@[17:18) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:21) NewLine |\n|
var myEmptyArray = [ ]
//@[0:3) Identifier |var|
//@[4:16) Identifier |myEmptyArray|
//@[17:18) Assignment |=|
//@[19:20) LeftSquare |[|
//@[21:22) RightSquare |]|
//@[22:24) NewLine |\n\n|

// object
//@[9:10) NewLine |\n|
var myObj = {
//@[0:3) Identifier |var|
//@[4:9) Identifier |myObj|
//@[10:11) Assignment |=|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
  a: 'a'
//@[2:3) Identifier |a|
//@[3:4) Colon |:|
//@[5:8) StringComplete |'a'|
//@[8:9) NewLine |\n|
  b: -12
//@[2:3) Identifier |b|
//@[3:4) Colon |:|
//@[5:6) Minus |-|
//@[6:8) Number |12|
//@[8:9) NewLine |\n|
  c: true
//@[2:3) Identifier |c|
//@[3:4) Colon |:|
//@[5:9) TrueKeyword |true|
//@[9:10) NewLine |\n|
  d: !true
//@[2:3) Identifier |d|
//@[3:4) Colon |:|
//@[5:6) Exclamation |!|
//@[6:10) TrueKeyword |true|
//@[10:11) NewLine |\n|
  list: [
//@[2:6) Identifier |list|
//@[6:7) Colon |:|
//@[8:9) LeftSquare |[|
//@[9:10) NewLine |\n|
    1
//@[4:5) Number |1|
//@[5:6) NewLine |\n|
    2
//@[4:5) Number |2|
//@[5:6) NewLine |\n|
    2+1
//@[4:5) Number |2|
//@[5:6) Plus |+|
//@[6:7) Number |1|
//@[7:8) NewLine |\n|
    {
//@[4:5) LeftBrace |{|
//@[5:6) NewLine |\n|
      test: 144 > 33 && true || 99 <= 199
//@[6:10) Identifier |test|
//@[10:11) Colon |:|
//@[12:15) Number |144|
//@[16:17) GreaterThan |>|
//@[18:20) Number |33|
//@[21:23) LogicalAnd |&&|
//@[24:28) TrueKeyword |true|
//@[29:31) LogicalOr ||||
//@[32:34) Number |99|
//@[35:37) LessThanOrEqual |<=|
//@[38:41) Number |199|
//@[41:42) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
    'a' =~ 'b'
//@[4:7) StringComplete |'a'|
//@[8:10) EqualsInsensitive |=~|
//@[11:14) StringComplete |'b'|
//@[14:15) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  obj: {
//@[2:5) Identifier |obj|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
    nested: [
//@[4:10) Identifier |nested|
//@[10:11) Colon |:|
//@[12:13) LeftSquare |[|
//@[13:14) NewLine |\n|
      'hello'
//@[6:13) StringComplete |'hello'|
//@[13:14) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// array
//@[8:9) NewLine |\n|
var myArr = [
//@[0:3) Identifier |var|
//@[4:9) Identifier |myArr|
//@[10:11) Assignment |=|
//@[12:13) LeftSquare |[|
//@[13:14) NewLine |\n|
  'pirates'
//@[2:11) StringComplete |'pirates'|
//@[11:12) NewLine |\n|
  'say'
//@[2:7) StringComplete |'say'|
//@[7:8) NewLine |\n|
  'arr'
//@[2:7) StringComplete |'arr'|
//@[7:8) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

// array with objects
//@[21:22) NewLine |\n|
var myArrWithObjects = [
//@[0:3) Identifier |var|
//@[4:20) Identifier |myArrWithObjects|
//@[21:22) Assignment |=|
//@[23:24) LeftSquare |[|
//@[24:25) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    name: 'one'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'one'|
//@[15:16) NewLine |\n|
    enable: true
//@[4:10) Identifier |enable|
//@[10:11) Colon |:|
//@[12:16) TrueKeyword |true|
//@[16:17) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    name: 'two'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'two'|
//@[15:16) NewLine |\n|
    enable: false && false || 'two' !~ 'three'
//@[4:10) Identifier |enable|
//@[10:11) Colon |:|
//@[12:17) FalseKeyword |false|
//@[18:20) LogicalAnd |&&|
//@[21:26) FalseKeyword |false|
//@[27:29) LogicalOr ||||
//@[30:35) StringComplete |'two'|
//@[36:38) NotEqualsInsensitive |!~|
//@[39:46) StringComplete |'three'|
//@[46:47) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

var expressionIndexOnAny = any({
//@[0:3) Identifier |var|
//@[4:24) Identifier |expressionIndexOnAny|
//@[25:26) Assignment |=|
//@[27:30) Identifier |any|
//@[30:31) LeftParen |(|
//@[31:32) LeftBrace |{|
//@[32:33) NewLine |\n|
})[resourceGroup().location]
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) LeftSquare |[|
//@[3:16) Identifier |resourceGroup|
//@[16:17) LeftParen |(|
//@[17:18) RightParen |)|
//@[18:19) Dot |.|
//@[19:27) Identifier |location|
//@[27:28) RightSquare |]|
//@[28:30) NewLine |\n\n|

var anyIndexOnAny = any(true)[any(false)]
//@[0:3) Identifier |var|
//@[4:17) Identifier |anyIndexOnAny|
//@[18:19) Assignment |=|
//@[20:23) Identifier |any|
//@[23:24) LeftParen |(|
//@[24:28) TrueKeyword |true|
//@[28:29) RightParen |)|
//@[29:30) LeftSquare |[|
//@[30:33) Identifier |any|
//@[33:34) LeftParen |(|
//@[34:39) FalseKeyword |false|
//@[39:40) RightParen |)|
//@[40:41) RightSquare |]|
//@[41:43) NewLine |\n\n|

var namedPropertyIndexer = {
//@[0:3) Identifier |var|
//@[4:24) Identifier |namedPropertyIndexer|
//@[25:26) Assignment |=|
//@[27:28) LeftBrace |{|
//@[28:29) NewLine |\n|
  foo: 's'
//@[2:5) Identifier |foo|
//@[5:6) Colon |:|
//@[7:10) StringComplete |'s'|
//@[10:11) NewLine |\n|
}['foo']
//@[0:1) RightBrace |}|
//@[1:2) LeftSquare |[|
//@[2:7) StringComplete |'foo'|
//@[7:8) RightSquare |]|
//@[8:10) NewLine |\n\n|

var intIndexer = [
//@[0:3) Identifier |var|
//@[4:14) Identifier |intIndexer|
//@[15:16) Assignment |=|
//@[17:18) LeftSquare |[|
//@[18:19) NewLine |\n|
  's'
//@[2:5) StringComplete |'s'|
//@[5:6) NewLine |\n|
][0]
//@[0:1) RightSquare |]|
//@[1:2) LeftSquare |[|
//@[2:3) Number |0|
//@[3:4) RightSquare |]|
//@[4:6) NewLine |\n\n|

var functionOnIndexer1 = concat([
//@[0:3) Identifier |var|
//@[4:22) Identifier |functionOnIndexer1|
//@[23:24) Assignment |=|
//@[25:31) Identifier |concat|
//@[31:32) LeftParen |(|
//@[32:33) LeftSquare |[|
//@[33:34) NewLine |\n|
  's'
//@[2:5) StringComplete |'s'|
//@[5:6) NewLine |\n|
][0], 's')
//@[0:1) RightSquare |]|
//@[1:2) LeftSquare |[|
//@[2:3) Number |0|
//@[3:4) RightSquare |]|
//@[4:5) Comma |,|
//@[6:9) StringComplete |'s'|
//@[9:10) RightParen |)|
//@[10:12) NewLine |\n\n|

var functionOnIndexer2 = concat([
//@[0:3) Identifier |var|
//@[4:22) Identifier |functionOnIndexer2|
//@[23:24) Assignment |=|
//@[25:31) Identifier |concat|
//@[31:32) LeftParen |(|
//@[32:33) LeftSquare |[|
//@[33:34) NewLine |\n|
][0], 's')
//@[0:1) RightSquare |]|
//@[1:2) LeftSquare |[|
//@[2:3) Number |0|
//@[3:4) RightSquare |]|
//@[4:5) Comma |,|
//@[6:9) StringComplete |'s'|
//@[9:10) RightParen |)|
//@[10:12) NewLine |\n\n|

var functionOnIndexer3 = concat([
//@[0:3) Identifier |var|
//@[4:22) Identifier |functionOnIndexer3|
//@[23:24) Assignment |=|
//@[25:31) Identifier |concat|
//@[31:32) LeftParen |(|
//@[32:33) LeftSquare |[|
//@[33:34) NewLine |\n|
][0], any('s'))
//@[0:1) RightSquare |]|
//@[1:2) LeftSquare |[|
//@[2:3) Number |0|
//@[3:4) RightSquare |]|
//@[4:5) Comma |,|
//@[6:9) Identifier |any|
//@[9:10) LeftParen |(|
//@[10:13) StringComplete |'s'|
//@[13:14) RightParen |)|
//@[14:15) RightParen |)|
//@[15:17) NewLine |\n\n|

var singleQuote = '\''
//@[0:3) Identifier |var|
//@[4:15) Identifier |singleQuote|
//@[16:17) Assignment |=|
//@[18:22) StringComplete |'\''|
//@[22:23) NewLine |\n|
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[0:3) Identifier |var|
//@[4:18) Identifier |myPropertyName|
//@[19:20) Assignment |=|
//@[21:24) StringLeftPiece |'${|
//@[24:35) Identifier |singleQuote|
//@[35:41) StringMiddlePiece |}foo${|
//@[41:52) Identifier |singleQuote|
//@[52:54) StringRightPiece |}'|
//@[54:56) NewLine |\n\n|

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
//@[0:3) Identifier |var|
//@[4:22) Identifier |unusedIntermediate|
//@[23:24) Assignment |=|
//@[25:33) Identifier |listKeys|
//@[33:34) LeftParen |(|
//@[34:44) Identifier |resourceId|
//@[44:45) LeftParen |(|
//@[45:59) StringComplete |'Mock.RP/type'|
//@[59:60) Comma |,|
//@[61:68) StringComplete |'steve'|
//@[68:69) RightParen |)|
//@[69:70) Comma |,|
//@[71:83) StringComplete |'2020-01-01'|
//@[83:84) RightParen |)|
//@[84:85) NewLine |\n|
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[0:3) Identifier |var|
//@[4:25) Identifier |unusedIntermediateRef|
//@[26:27) Assignment |=|
//@[28:46) Identifier |unusedIntermediate|
//@[46:47) Dot |.|
//@[47:59) Identifier |secondaryKey|
//@[59:59) EndOfFile ||
