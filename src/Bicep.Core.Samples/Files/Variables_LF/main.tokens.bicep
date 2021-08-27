
//@[0:1) NewLine |\n|
// int
//@[6:7) NewLine |\n|
@sys.description('an int variable')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:34) StringComplete |'an int variable'|
//@[34:35) RightParen |)|
//@[35:36) NewLine |\n|
var myInt = 42
//@[0:3) Identifier |var|
//@[4:9) Identifier |myInt|
//@[10:11) Assignment |=|
//@[12:14) Integer |42|
//@[14:16) NewLine |\n\n|

// string
//@[9:10) NewLine |\n|
@sys.description('a string variable')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:36) StringComplete |'a string variable'|
//@[36:37) RightParen |)|
//@[37:38) NewLine |\n|
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
//@[20:23) Integer |123|
//@[23:28) StringRightPiece |}def'|
//@[28:29) NewLine |\n|
var interp2 = '${123}def'
//@[0:3) Identifier |var|
//@[4:11) Identifier |interp2|
//@[12:13) Assignment |=|
//@[14:17) StringLeftPiece |'${|
//@[17:20) Integer |123|
//@[20:25) StringRightPiece |}def'|
//@[25:26) NewLine |\n|
var interp3 = 'abc${123}'
//@[0:3) Identifier |var|
//@[4:11) Identifier |interp3|
//@[12:13) Assignment |=|
//@[14:20) StringLeftPiece |'abc${|
//@[20:23) Integer |123|
//@[23:25) StringRightPiece |}'|
//@[25:26) NewLine |\n|
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[0:3) Identifier |var|
//@[4:11) Identifier |interp4|
//@[12:13) Assignment |=|
//@[14:20) StringLeftPiece |'abc${|
//@[20:23) Integer |123|
//@[23:26) StringMiddlePiece |}${|
//@[26:29) Integer |456|
//@[29:36) StringMiddlePiece |}jk$l${|
//@[36:39) Integer |789|
//@[39:43) StringRightPiece |}p$'|
//@[43:44) NewLine |\n|
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[0:3) Identifier |var|
//@[4:16) Identifier |doubleInterp|
//@[17:18) Assignment |=|
//@[19:25) StringLeftPiece |'abc${|
//@[25:31) StringLeftPiece |'def${|
//@[31:34) Integer |123|
//@[34:36) StringRightPiece |}'|
//@[36:40) StringMiddlePiece |}_${|
//@[40:43) StringLeftPiece |'${|
//@[43:46) Integer |456|
//@[46:49) StringMiddlePiece |}${|
//@[49:52) Integer |789|
//@[52:54) StringRightPiece |}'|
//@[54:56) StringRightPiece |}'|
//@[56:57) NewLine |\n|
var curliesInInterp = '{${123}{0}${true}}'
//@[0:3) Identifier |var|
//@[4:19) Identifier |curliesInInterp|
//@[20:21) Assignment |=|
//@[22:26) StringLeftPiece |'{${|
//@[26:29) Integer |123|
//@[29:35) StringMiddlePiece |}{0}${|
//@[35:39) TrueKeyword |true|
//@[39:42) StringRightPiece |}}'|
//@[42:44) NewLine |\n\n|

// #completionTest(0) -> declarations
//@[37:39) NewLine |\n\n|

// verify correct bracket escaping
//@[34:35) NewLine |\n|
var bracketInTheMiddle = 'a[b]'
//@[0:3) Identifier |var|
//@[4:22) Identifier |bracketInTheMiddle|
//@[23:24) Assignment |=|
//@[25:31) StringComplete |'a[b]'|
//@[31:32) NewLine |\n|
// #completionTest(25) -> empty
//@[31:32) NewLine |\n|
var bracketAtBeginning = '[test'
//@[0:3) Identifier |var|
//@[4:22) Identifier |bracketAtBeginning|
//@[23:24) Assignment |=|
//@[25:32) StringComplete |'[test'|
//@[32:33) NewLine |\n|
// #completionTest(23) -> symbolsPlusTypes
//@[42:43) NewLine |\n|
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
@sys.description('a bool variable')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:34) StringComplete |'a bool variable'|
//@[34:35) RightParen |)|
//@[35:36) NewLine |\n|
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
@sys.description('a object variable')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:36) StringComplete |'a object variable'|
//@[36:37) RightParen |)|
//@[37:38) NewLine |\n|
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
//@[6:8) Integer |12|
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
//@[4:5) Integer |1|
//@[5:6) NewLine |\n|
    2
//@[4:5) Integer |2|
//@[5:6) NewLine |\n|
    2+1
//@[4:5) Integer |2|
//@[5:6) Plus |+|
//@[6:7) Integer |1|
//@[7:8) NewLine |\n|
    {
//@[4:5) LeftBrace |{|
//@[5:6) NewLine |\n|
      test: 144 > 33 && true || 99 <= 199
//@[6:10) Identifier |test|
//@[10:11) Colon |:|
//@[12:15) Integer |144|
//@[16:17) GreaterThan |>|
//@[18:20) Integer |33|
//@[21:23) LogicalAnd |&&|
//@[24:28) TrueKeyword |true|
//@[29:31) LogicalOr ||||
//@[32:34) Integer |99|
//@[35:37) LessThanOrEqual |<=|
//@[38:41) Integer |199|
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

@sys.description('a object with interp')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:39) StringComplete |'a object with interp'|
//@[39:40) RightParen |)|
//@[40:41) NewLine |\n|
var objWithInterp = {
//@[0:3) Identifier |var|
//@[4:17) Identifier |objWithInterp|
//@[18:19) Assignment |=|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
  '${myStr}': 1
//@[2:5) StringLeftPiece |'${|
//@[5:10) Identifier |myStr|
//@[10:12) StringRightPiece |}'|
//@[12:13) Colon |:|
//@[14:15) Integer |1|
//@[15:16) NewLine |\n|
  'abc${myStr}def': 2
//@[2:8) StringLeftPiece |'abc${|
//@[8:13) Identifier |myStr|
//@[13:18) StringRightPiece |}def'|
//@[18:19) Colon |:|
//@[20:21) Integer |2|
//@[21:22) NewLine |\n|
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[2:5) StringLeftPiece |'${|
//@[5:12) Identifier |interp1|
//@[12:18) StringMiddlePiece |}abc${|
//@[18:25) Identifier |interp2|
//@[25:27) StringRightPiece |}'|
//@[27:28) Colon |:|
//@[29:32) StringLeftPiece |'${|
//@[32:39) Identifier |interp1|
//@[39:45) StringMiddlePiece |}abc${|
//@[45:52) Identifier |interp2|
//@[52:54) StringRightPiece |}'|
//@[54:55) NewLine |\n|
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
})[az.resourceGroup().location]
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) LeftSquare |[|
//@[3:5) Identifier |az|
//@[5:6) Dot |.|
//@[6:19) Identifier |resourceGroup|
//@[19:20) LeftParen |(|
//@[20:21) RightParen |)|
//@[21:22) Dot |.|
//@[22:30) Identifier |location|
//@[30:31) RightSquare |]|
//@[31:33) NewLine |\n\n|

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

var deploymentName = deployment().name
//@[0:3) Identifier |var|
//@[4:18) Identifier |deploymentName|
//@[19:20) Assignment |=|
//@[21:31) Identifier |deployment|
//@[31:32) LeftParen |(|
//@[32:33) RightParen |)|
//@[33:34) Dot |.|
//@[34:38) Identifier |name|
//@[38:39) NewLine |\n|
var templateLinkUri = deployment().properties.templateLink.uri
//@[0:3) Identifier |var|
//@[4:19) Identifier |templateLinkUri|
//@[20:21) Assignment |=|
//@[22:32) Identifier |deployment|
//@[32:33) LeftParen |(|
//@[33:34) RightParen |)|
//@[34:35) Dot |.|
//@[35:45) Identifier |properties|
//@[45:46) Dot |.|
//@[46:58) Identifier |templateLink|
//@[58:59) Dot |.|
//@[59:62) Identifier |uri|
//@[62:63) NewLine |\n|
var templateLinkId = deployment().properties.templateLink.id
//@[0:3) Identifier |var|
//@[4:18) Identifier |templateLinkId|
//@[19:20) Assignment |=|
//@[21:31) Identifier |deployment|
//@[31:32) LeftParen |(|
//@[32:33) RightParen |)|
//@[33:34) Dot |.|
//@[34:44) Identifier |properties|
//@[44:45) Dot |.|
//@[45:57) Identifier |templateLink|
//@[57:58) Dot |.|
//@[58:60) Identifier |id|
//@[60:62) NewLine |\n\n|

var portalEndpoint = environment().portal
//@[0:3) Identifier |var|
//@[4:18) Identifier |portalEndpoint|
//@[19:20) Assignment |=|
//@[21:32) Identifier |environment|
//@[32:33) LeftParen |(|
//@[33:34) RightParen |)|
//@[34:35) Dot |.|
//@[35:41) Identifier |portal|
//@[41:42) NewLine |\n|
var loginEndpoint = environment().authentication.loginEndpoint
//@[0:3) Identifier |var|
//@[4:17) Identifier |loginEndpoint|
//@[18:19) Assignment |=|
//@[20:31) Identifier |environment|
//@[31:32) LeftParen |(|
//@[32:33) RightParen |)|
//@[33:34) Dot |.|
//@[34:48) Identifier |authentication|
//@[48:49) Dot |.|
//@[49:62) Identifier |loginEndpoint|
//@[62:64) NewLine |\n\n|

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
//@[2:3) Integer |0|
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
//@[2:3) Integer |0|
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
//@[2:3) Integer |0|
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
//@[2:3) Integer |0|
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
//@[59:61) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[53:54) NewLine |\n|
var previousEmitLimit = [
//@[0:3) Identifier |var|
//@[4:21) Identifier |previousEmitLimit|
//@[22:23) Assignment |=|
//@[24:25) LeftSquare |[|
//@[25:26) NewLine |\n|
  concat('s')
//@[2:8) Identifier |concat|
//@[8:9) LeftParen |(|
//@[9:12) StringComplete |'s'|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
  '${4}'
//@[2:5) StringLeftPiece |'${|
//@[5:6) Integer |4|
//@[6:8) StringRightPiece |}'|
//@[8:9) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    a: {
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
      b: base64('s')
//@[6:7) Identifier |b|
//@[7:8) Colon |:|
//@[9:15) Identifier |base64|
//@[15:16) LeftParen |(|
//@[16:19) StringComplete |'s'|
//@[19:20) RightParen |)|
//@[20:21) NewLine |\n|
      c: concat([
//@[6:7) Identifier |c|
//@[7:8) Colon |:|
//@[9:15) Identifier |concat|
//@[15:16) LeftParen |(|
//@[16:17) LeftSquare |[|
//@[17:18) NewLine |\n|
        12 + 3
//@[8:10) Integer |12|
//@[11:12) Plus |+|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
      ], [
//@[6:7) RightSquare |]|
//@[7:8) Comma |,|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
        !true
//@[8:9) Exclamation |!|
//@[9:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
        'hello'
//@[8:15) StringComplete |'hello'|
//@[15:16) NewLine |\n|
      ])
//@[6:7) RightSquare |]|
//@[7:8) RightParen |)|
//@[8:9) NewLine |\n|
      d: az.resourceGroup().location
//@[6:7) Identifier |d|
//@[7:8) Colon |:|
//@[9:11) Identifier |az|
//@[11:12) Dot |.|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:37) NewLine |\n|
      e: concat([
//@[6:7) Identifier |e|
//@[7:8) Colon |:|
//@[9:15) Identifier |concat|
//@[15:16) LeftParen |(|
//@[16:17) LeftSquare |[|
//@[17:18) NewLine |\n|
        true
//@[8:12) TrueKeyword |true|
//@[12:13) NewLine |\n|
      ])
//@[6:7) RightSquare |]|
//@[7:8) RightParen |)|
//@[8:9) NewLine |\n|
      f: concat([
//@[6:7) Identifier |f|
//@[7:8) Colon |:|
//@[9:15) Identifier |concat|
//@[15:16) LeftParen |(|
//@[16:17) LeftSquare |[|
//@[17:18) NewLine |\n|
        's' == 12
//@[8:11) StringComplete |'s'|
//@[12:14) Equals |==|
//@[15:17) Integer |12|
//@[17:18) NewLine |\n|
      ])
//@[6:7) RightSquare |]|
//@[7:8) RightParen |)|
//@[8:9) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[53:54) NewLine |\n|
var previousEmitLimit2 = [
//@[0:3) Identifier |var|
//@[4:22) Identifier |previousEmitLimit2|
//@[23:24) Assignment |=|
//@[25:26) LeftSquare |[|
//@[26:27) NewLine |\n|
  concat('s')
//@[2:8) Identifier |concat|
//@[8:9) LeftParen |(|
//@[9:12) StringComplete |'s'|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
  '${4}'
//@[2:5) StringLeftPiece |'${|
//@[5:6) Integer |4|
//@[6:8) StringRightPiece |}'|
//@[8:9) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    a: {
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
      b: base64('s')
//@[6:7) Identifier |b|
//@[7:8) Colon |:|
//@[9:15) Identifier |base64|
//@[15:16) LeftParen |(|
//@[16:19) StringComplete |'s'|
//@[19:20) RightParen |)|
//@[20:21) NewLine |\n|
      c: union({
//@[6:7) Identifier |c|
//@[7:8) Colon |:|
//@[9:14) Identifier |union|
//@[14:15) LeftParen |(|
//@[15:16) LeftBrace |{|
//@[16:17) NewLine |\n|
        a: 12 + 3
//@[8:9) Identifier |a|
//@[9:10) Colon |:|
//@[11:13) Integer |12|
//@[14:15) Plus |+|
//@[16:17) Integer |3|
//@[17:18) NewLine |\n|
      }, {
//@[6:7) RightBrace |}|
//@[7:8) Comma |,|
//@[9:10) LeftBrace |{|
//@[10:11) NewLine |\n|
        b: !true
//@[8:9) Identifier |b|
//@[9:10) Colon |:|
//@[11:12) Exclamation |!|
//@[12:16) TrueKeyword |true|
//@[16:17) NewLine |\n|
        c: 'hello'
//@[8:9) Identifier |c|
//@[9:10) Colon |:|
//@[11:18) StringComplete |'hello'|
//@[18:19) NewLine |\n|
      })
//@[6:7) RightBrace |}|
//@[7:8) RightParen |)|
//@[8:9) NewLine |\n|
      d: az.resourceGroup().location
//@[6:7) Identifier |d|
//@[7:8) Colon |:|
//@[9:11) Identifier |az|
//@[11:12) Dot |.|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:37) NewLine |\n|
      e: union({
//@[6:7) Identifier |e|
//@[7:8) Colon |:|
//@[9:14) Identifier |union|
//@[14:15) LeftParen |(|
//@[15:16) LeftBrace |{|
//@[16:17) NewLine |\n|
        x: true
//@[8:9) Identifier |x|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
      }, {})
//@[6:7) RightBrace |}|
//@[7:8) Comma |,|
//@[9:10) LeftBrace |{|
//@[10:11) RightBrace |}|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
      f: intersection({
//@[6:7) Identifier |f|
//@[7:8) Colon |:|
//@[9:21) Identifier |intersection|
//@[21:22) LeftParen |(|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
        q: 's' == 12
//@[8:9) Identifier |q|
//@[9:10) Colon |:|
//@[11:14) StringComplete |'s'|
//@[15:17) Equals |==|
//@[18:20) Integer |12|
//@[20:21) NewLine |\n|
      }, {})
//@[6:7) RightBrace |}|
//@[7:8) Comma |,|
//@[9:10) LeftBrace |{|
//@[10:11) RightBrace |}|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[53:54) NewLine |\n|
var previousEmitLimit3 = {
//@[0:3) Identifier |var|
//@[4:22) Identifier |previousEmitLimit3|
//@[23:24) Assignment |=|
//@[25:26) LeftBrace |{|
//@[26:27) NewLine |\n|
  a: {
//@[2:3) Identifier |a|
//@[3:4) Colon |:|
//@[5:6) LeftBrace |{|
//@[6:7) NewLine |\n|
    b: {
//@[4:5) Identifier |b|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
      a: az.resourceGroup().location
//@[6:7) Identifier |a|
//@[7:8) Colon |:|
//@[9:11) Identifier |az|
//@[11:12) Dot |.|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:37) NewLine |\n|
    } == 2
//@[4:5) RightBrace |}|
//@[6:8) Equals |==|
//@[9:10) Integer |2|
//@[10:11) NewLine |\n|
    c: concat([
//@[4:5) Identifier |c|
//@[5:6) Colon |:|
//@[7:13) Identifier |concat|
//@[13:14) LeftParen |(|
//@[14:15) LeftSquare |[|
//@[15:17) NewLine |\n\n|

    ], [
//@[4:5) RightSquare |]|
//@[5:6) Comma |,|
//@[7:8) LeftSquare |[|
//@[8:9) NewLine |\n|
      true
//@[6:10) TrueKeyword |true|
//@[10:11) NewLine |\n|
    ])
//@[4:5) RightSquare |]|
//@[5:6) RightParen |)|
//@[6:7) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(0) -> declarations
//@[37:39) NewLine |\n\n|

var myVar = 'hello'
//@[0:3) Identifier |var|
//@[4:9) Identifier |myVar|
//@[10:11) Assignment |=|
//@[12:19) StringComplete |'hello'|
//@[19:20) NewLine |\n|
var myVar2 = any({
//@[0:3) Identifier |var|
//@[4:10) Identifier |myVar2|
//@[11:12) Assignment |=|
//@[13:16) Identifier |any|
//@[16:17) LeftParen |(|
//@[17:18) LeftBrace |{|
//@[18:19) NewLine |\n|
  something: myVar
//@[2:11) Identifier |something|
//@[11:12) Colon |:|
//@[13:18) Identifier |myVar|
//@[18:19) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
var myVar3 = any(any({
//@[0:3) Identifier |var|
//@[4:10) Identifier |myVar3|
//@[11:12) Assignment |=|
//@[13:16) Identifier |any|
//@[16:17) LeftParen |(|
//@[17:20) Identifier |any|
//@[20:21) LeftParen |(|
//@[21:22) LeftBrace |{|
//@[22:23) NewLine |\n|
  something: myVar
//@[2:11) Identifier |something|
//@[11:12) Colon |:|
//@[13:18) Identifier |myVar|
//@[18:19) NewLine |\n|
}))
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) RightParen |)|
//@[3:4) NewLine |\n|
var myVar4 = length(any(concat('s','a')))
//@[0:3) Identifier |var|
//@[4:10) Identifier |myVar4|
//@[11:12) Assignment |=|
//@[13:19) Identifier |length|
//@[19:20) LeftParen |(|
//@[20:23) Identifier |any|
//@[23:24) LeftParen |(|
//@[24:30) Identifier |concat|
//@[30:31) LeftParen |(|
//@[31:34) StringComplete |'s'|
//@[34:35) Comma |,|
//@[35:38) StringComplete |'a'|
//@[38:39) RightParen |)|
//@[39:40) RightParen |)|
//@[40:41) RightParen |)|
//@[41:43) NewLine |\n\n|

// verify that unqualified banned function identifiers can be used as declaration identifiers
//@[93:94) NewLine |\n|
var variables = true
//@[0:3) Identifier |var|
//@[4:13) Identifier |variables|
//@[14:15) Assignment |=|
//@[16:20) TrueKeyword |true|
//@[20:21) NewLine |\n|
param parameters bool = true
//@[0:5) Identifier |param|
//@[6:16) Identifier |parameters|
//@[17:21) Identifier |bool|
//@[22:23) Assignment |=|
//@[24:28) TrueKeyword |true|
//@[28:29) NewLine |\n|
var if = true
//@[0:3) Identifier |var|
//@[4:6) Identifier |if|
//@[7:8) Assignment |=|
//@[9:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
var createArray = true
//@[0:3) Identifier |var|
//@[4:15) Identifier |createArray|
//@[16:17) Assignment |=|
//@[18:22) TrueKeyword |true|
//@[22:23) NewLine |\n|
var createObject = true
//@[0:3) Identifier |var|
//@[4:16) Identifier |createObject|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[23:24) NewLine |\n|
var add = true
//@[0:3) Identifier |var|
//@[4:7) Identifier |add|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
var sub = true
//@[0:3) Identifier |var|
//@[4:7) Identifier |sub|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
var mul = true
//@[0:3) Identifier |var|
//@[4:7) Identifier |mul|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
var div = true
//@[0:3) Identifier |var|
//@[4:7) Identifier |div|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
param mod bool = true
//@[0:5) Identifier |param|
//@[6:9) Identifier |mod|
//@[10:14) Identifier |bool|
//@[15:16) Assignment |=|
//@[17:21) TrueKeyword |true|
//@[21:22) NewLine |\n|
var less = true
//@[0:3) Identifier |var|
//@[4:8) Identifier |less|
//@[9:10) Assignment |=|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
var lessOrEquals = true
//@[0:3) Identifier |var|
//@[4:16) Identifier |lessOrEquals|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[23:24) NewLine |\n|
var greater = true
//@[0:3) Identifier |var|
//@[4:11) Identifier |greater|
//@[12:13) Assignment |=|
//@[14:18) TrueKeyword |true|
//@[18:19) NewLine |\n|
var greaterOrEquals = true
//@[0:3) Identifier |var|
//@[4:19) Identifier |greaterOrEquals|
//@[20:21) Assignment |=|
//@[22:26) TrueKeyword |true|
//@[26:27) NewLine |\n|
param equals bool = true
//@[0:5) Identifier |param|
//@[6:12) Identifier |equals|
//@[13:17) Identifier |bool|
//@[18:19) Assignment |=|
//@[20:24) TrueKeyword |true|
//@[24:25) NewLine |\n|
var not = true
//@[0:3) Identifier |var|
//@[4:7) Identifier |not|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
var and = true
//@[0:3) Identifier |var|
//@[4:7) Identifier |and|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
var or = true
//@[0:3) Identifier |var|
//@[4:6) Identifier |or|
//@[7:8) Assignment |=|
//@[9:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[0:3) Identifier |var|
//@[4:17) Identifier |I_WANT_IT_ALL|
//@[18:19) Assignment |=|
//@[20:29) Identifier |variables|
//@[30:32) LogicalAnd |&&|
//@[33:43) Identifier |parameters|
//@[44:46) LogicalAnd |&&|
//@[47:49) Identifier |if|
//@[50:52) LogicalAnd |&&|
//@[53:64) Identifier |createArray|
//@[65:67) LogicalAnd |&&|
//@[68:80) Identifier |createObject|
//@[81:83) LogicalAnd |&&|
//@[84:87) Identifier |add|
//@[88:90) LogicalAnd |&&|
//@[91:94) Identifier |sub|
//@[95:97) LogicalAnd |&&|
//@[98:101) Identifier |mul|
//@[102:104) LogicalAnd |&&|
//@[105:108) Identifier |div|
//@[109:111) LogicalAnd |&&|
//@[112:115) Identifier |mod|
//@[116:118) LogicalAnd |&&|
//@[119:123) Identifier |less|
//@[124:126) LogicalAnd |&&|
//@[127:139) Identifier |lessOrEquals|
//@[140:142) LogicalAnd |&&|
//@[143:150) Identifier |greater|
//@[151:153) LogicalAnd |&&|
//@[154:169) Identifier |greaterOrEquals|
//@[170:172) LogicalAnd |&&|
//@[173:179) Identifier |equals|
//@[180:182) LogicalAnd |&&|
//@[183:186) Identifier |not|
//@[187:189) LogicalAnd |&&|
//@[190:193) Identifier |and|
//@[194:196) LogicalAnd |&&|
//@[197:199) Identifier |or|
//@[199:201) NewLine |\n\n|

// identifiers can have underscores
//@[35:36) NewLine |\n|
var _ = 3
//@[0:3) Identifier |var|
//@[4:5) Identifier |_|
//@[6:7) Assignment |=|
//@[8:9) Integer |3|
//@[9:10) NewLine |\n|
var __ = 10 * _
//@[0:3) Identifier |var|
//@[4:6) Identifier |__|
//@[7:8) Assignment |=|
//@[9:11) Integer |10|
//@[12:13) Asterisk |*|
//@[14:15) Identifier |_|
//@[15:16) NewLine |\n|
var _0a_1b = true
//@[0:3) Identifier |var|
//@[4:10) Identifier |_0a_1b|
//@[11:12) Assignment |=|
//@[13:17) TrueKeyword |true|
//@[17:18) NewLine |\n|
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[0:3) Identifier |var|
//@[4:7) Identifier |_1_|
//@[8:9) Assignment |=|
//@[10:16) Identifier |_0a_1b|
//@[17:19) LogicalOr ||||
//@[20:21) LeftParen |(|
//@[21:23) Identifier |__|
//@[24:25) Plus |+|
//@[26:27) Identifier |_|
//@[28:29) Modulo |%|
//@[30:31) Integer |2|
//@[32:34) Equals |==|
//@[35:36) Integer |0|
//@[36:37) RightParen |)|
//@[37:39) NewLine |\n\n|

// fully qualified access
//@[25:26) NewLine |\n|
var resourceGroup = 'something'
//@[0:3) Identifier |var|
//@[4:17) Identifier |resourceGroup|
//@[18:19) Assignment |=|
//@[20:31) StringComplete |'something'|
//@[31:32) NewLine |\n|
var resourceGroupName = az.resourceGroup().name
//@[0:3) Identifier |var|
//@[4:21) Identifier |resourceGroupName|
//@[22:23) Assignment |=|
//@[24:26) Identifier |az|
//@[26:27) Dot |.|
//@[27:40) Identifier |resourceGroup|
//@[40:41) LeftParen |(|
//@[41:42) RightParen |)|
//@[42:43) Dot |.|
//@[43:47) Identifier |name|
//@[47:48) NewLine |\n|
var resourceGroupObject = az.resourceGroup()
//@[0:3) Identifier |var|
//@[4:23) Identifier |resourceGroupObject|
//@[24:25) Assignment |=|
//@[26:28) Identifier |az|
//@[28:29) Dot |.|
//@[29:42) Identifier |resourceGroup|
//@[42:43) LeftParen |(|
//@[43:44) RightParen |)|
//@[44:45) NewLine |\n|
var propertyAccessFromObject = resourceGroupObject.name
//@[0:3) Identifier |var|
//@[4:28) Identifier |propertyAccessFromObject|
//@[29:30) Assignment |=|
//@[31:50) Identifier |resourceGroupObject|
//@[50:51) Dot |.|
//@[51:55) Identifier |name|
//@[55:56) NewLine |\n|
var isTrue = sys.max(1, 2) == 3
//@[0:3) Identifier |var|
//@[4:10) Identifier |isTrue|
//@[11:12) Assignment |=|
//@[13:16) Identifier |sys|
//@[16:17) Dot |.|
//@[17:20) Identifier |max|
//@[20:21) LeftParen |(|
//@[21:22) Integer |1|
//@[22:23) Comma |,|
//@[24:25) Integer |2|
//@[25:26) RightParen |)|
//@[27:29) Equals |==|
//@[30:31) Integer |3|
//@[31:32) NewLine |\n|
var isFalse = !isTrue
//@[0:3) Identifier |var|
//@[4:11) Identifier |isFalse|
//@[12:13) Assignment |=|
//@[14:15) Exclamation |!|
//@[15:21) Identifier |isTrue|
//@[21:22) NewLine |\n|
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[0:3) Identifier |var|
//@[4:12) Identifier |someText|
//@[13:14) Assignment |=|
//@[15:21) Identifier |isTrue|
//@[22:23) Question |?|
//@[24:27) Identifier |sys|
//@[27:28) Dot |.|
//@[28:34) Identifier |concat|
//@[34:35) LeftParen |(|
//@[35:38) StringComplete |'a'|
//@[38:39) Comma |,|
//@[40:43) Identifier |sys|
//@[43:44) Dot |.|
//@[44:50) Identifier |concat|
//@[50:51) LeftParen |(|
//@[51:54) StringComplete |'b'|
//@[54:55) Comma |,|
//@[56:59) StringComplete |'c'|
//@[59:60) RightParen |)|
//@[60:61) RightParen |)|
//@[62:63) Colon |:|
//@[64:74) StringComplete |'someText'|
//@[74:76) NewLine |\n\n|

// Bicep functions that cannot be converted into ARM functions
//@[62:63) NewLine |\n|
var scopesWithoutArmRepresentation = {
//@[0:3) Identifier |var|
//@[4:34) Identifier |scopesWithoutArmRepresentation|
//@[35:36) Assignment |=|
//@[37:38) LeftBrace |{|
//@[38:39) NewLine |\n|
  tenant: tenant()
//@[2:8) Identifier |tenant|
//@[8:9) Colon |:|
//@[10:16) Identifier |tenant|
//@[16:17) LeftParen |(|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@[2:14) Identifier |subscription|
//@[14:15) Colon |:|
//@[16:28) Identifier |subscription|
//@[28:29) LeftParen |(|
//@[29:67) StringComplete |'10b57a01-6350-4ce2-972a-6a13642f00bf'|
//@[67:68) RightParen |)|
//@[68:69) NewLine |\n|
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@[2:15) Identifier |resourceGroup|
//@[15:16) Colon |:|
//@[17:19) Identifier |az|
//@[19:20) Dot |.|
//@[20:33) Identifier |resourceGroup|
//@[33:34) LeftParen |(|
//@[34:72) StringComplete |'10b57a01-6350-4ce2-972a-6a13642f00bf'|
//@[72:73) Comma |,|
//@[74:84) StringComplete |'myRgName'|
//@[84:85) RightParen |)|
//@[85:86) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// Issue #1332
//@[14:15) NewLine |\n|
var issue1332_propname = 'ptest'
//@[0:3) Identifier |var|
//@[4:22) Identifier |issue1332_propname|
//@[23:24) Assignment |=|
//@[25:32) StringComplete |'ptest'|
//@[32:33) NewLine |\n|
var issue1332 = true ? {
//@[0:3) Identifier |var|
//@[4:13) Identifier |issue1332|
//@[14:15) Assignment |=|
//@[16:20) TrueKeyword |true|
//@[21:22) Question |?|
//@[23:24) LeftBrace |{|
//@[24:25) NewLine |\n|
    prop1: {
//@[4:9) Identifier |prop1|
//@[9:10) Colon |:|
//@[11:12) LeftBrace |{|
//@[12:13) NewLine |\n|
        '${issue1332_propname}': {}
//@[8:11) StringLeftPiece |'${|
//@[11:29) Identifier |issue1332_propname|
//@[29:31) StringRightPiece |}'|
//@[31:32) Colon |:|
//@[33:34) LeftBrace |{|
//@[34:35) RightBrace |}|
//@[35:36) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
} : {}
//@[0:1) RightBrace |}|
//@[2:3) Colon |:|
//@[4:5) LeftBrace |{|
//@[5:6) RightBrace |}|
//@[6:8) NewLine |\n\n|

// Issue #486
//@[13:14) NewLine |\n|
var myBigInt = 2199023255552
//@[0:3) Identifier |var|
//@[4:12) Identifier |myBigInt|
//@[13:14) Assignment |=|
//@[15:28) Integer |2199023255552|
//@[28:29) NewLine |\n|
var myIntExpression = 5 * 5
//@[0:3) Identifier |var|
//@[4:19) Identifier |myIntExpression|
//@[20:21) Assignment |=|
//@[22:23) Integer |5|
//@[24:25) Asterisk |*|
//@[26:27) Integer |5|
//@[27:28) NewLine |\n|
var myBigIntExpression = 2199023255552 * 2
//@[0:3) Identifier |var|
//@[4:22) Identifier |myBigIntExpression|
//@[23:24) Assignment |=|
//@[25:38) Integer |2199023255552|
//@[39:40) Asterisk |*|
//@[41:42) Integer |2|
//@[42:43) NewLine |\n|
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[0:3) Identifier |var|
//@[4:23) Identifier |myBigIntExpression2|
//@[24:25) Assignment |=|
//@[26:39) Integer |2199023255552|
//@[40:41) Asterisk |*|
//@[42:55) Integer |2199023255552|
//@[55:57) NewLine |\n\n|

// variable loops
//@[17:18) NewLine |\n|
var incrementingNumbers = [for i in range(0,10) : i]
//@[0:3) Identifier |var|
//@[4:23) Identifier |incrementingNumbers|
//@[24:25) Assignment |=|
//@[26:27) LeftSquare |[|
//@[27:30) Identifier |for|
//@[31:32) Identifier |i|
//@[33:35) Identifier |in|
//@[36:41) Identifier |range|
//@[41:42) LeftParen |(|
//@[42:43) Integer |0|
//@[43:44) Comma |,|
//@[44:46) Integer |10|
//@[46:47) RightParen |)|
//@[48:49) Colon |:|
//@[50:51) Identifier |i|
//@[51:52) RightSquare |]|
//@[52:53) NewLine |\n|
var loopInput = [
//@[0:3) Identifier |var|
//@[4:13) Identifier |loopInput|
//@[14:15) Assignment |=|
//@[16:17) LeftSquare |[|
//@[17:18) NewLine |\n|
  'one'
//@[2:7) StringComplete |'one'|
//@[7:8) NewLine |\n|
  'two'
//@[2:7) StringComplete |'two'|
//@[7:8) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:2) NewLine |\n|
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[0:3) Identifier |var|
//@[4:25) Identifier |arrayOfStringsViaLoop|
//@[26:27) Assignment |=|
//@[28:29) LeftSquare |[|
//@[29:32) Identifier |for|
//@[33:34) LeftParen |(|
//@[34:38) Identifier |name|
//@[38:39) Comma |,|
//@[40:41) Identifier |i|
//@[41:42) RightParen |)|
//@[43:45) Identifier |in|
//@[46:55) Identifier |loopInput|
//@[55:56) Colon |:|
//@[57:67) StringLeftPiece |'prefix-${|
//@[67:68) Identifier |i|
//@[68:72) StringMiddlePiece |}-${|
//@[72:76) Identifier |name|
//@[76:78) StringRightPiece |}'|
//@[78:79) RightSquare |]|
//@[79:80) NewLine |\n|
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[0:3) Identifier |var|
//@[4:25) Identifier |arrayOfObjectsViaLoop|
//@[26:27) Assignment |=|
//@[28:29) LeftSquare |[|
//@[29:32) Identifier |for|
//@[33:34) LeftParen |(|
//@[34:38) Identifier |name|
//@[38:39) Comma |,|
//@[40:41) Identifier |i|
//@[41:42) RightParen |)|
//@[43:45) Identifier |in|
//@[46:55) Identifier |loopInput|
//@[55:56) Colon |:|
//@[57:58) LeftBrace |{|
//@[58:59) NewLine |\n|
  index: i
//@[2:7) Identifier |index|
//@[7:8) Colon |:|
//@[9:10) Identifier |i|
//@[10:11) NewLine |\n|
  name: name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) Identifier |name|
//@[12:13) NewLine |\n|
  value: 'prefix-${i}-${name}-suffix'
//@[2:7) Identifier |value|
//@[7:8) Colon |:|
//@[9:19) StringLeftPiece |'prefix-${|
//@[19:20) Identifier |i|
//@[20:24) StringMiddlePiece |}-${|
//@[24:28) Identifier |name|
//@[28:37) StringRightPiece |}-suffix'|
//@[37:38) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:3) NewLine |\n|
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[0:3) Identifier |var|
//@[4:24) Identifier |arrayOfArraysViaLoop|
//@[25:26) Assignment |=|
//@[27:28) LeftSquare |[|
//@[28:31) Identifier |for|
//@[32:33) LeftParen |(|
//@[33:37) Identifier |name|
//@[37:38) Comma |,|
//@[39:40) Identifier |i|
//@[40:41) RightParen |)|
//@[42:44) Identifier |in|
//@[45:54) Identifier |loopInput|
//@[54:55) Colon |:|
//@[56:57) LeftSquare |[|
//@[57:58) NewLine |\n|
  i
//@[2:3) Identifier |i|
//@[3:4) NewLine |\n|
  name
//@[2:6) Identifier |name|
//@[6:7) NewLine |\n|
  'prefix-${i}-${name}-suffix'
//@[2:12) StringLeftPiece |'prefix-${|
//@[12:13) Identifier |i|
//@[13:17) StringMiddlePiece |}-${|
//@[17:21) Identifier |name|
//@[21:30) StringRightPiece |}-suffix'|
//@[30:31) NewLine |\n|
]]
//@[0:1) RightSquare |]|
//@[1:2) RightSquare |]|
//@[2:3) NewLine |\n|
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[0:3) Identifier |var|
//@[4:19) Identifier |arrayOfBooleans|
//@[20:21) Assignment |=|
//@[22:23) LeftSquare |[|
//@[23:26) Identifier |for|
//@[27:28) LeftParen |(|
//@[28:32) Identifier |name|
//@[32:33) Comma |,|
//@[34:35) Identifier |i|
//@[35:36) RightParen |)|
//@[37:39) Identifier |in|
//@[40:49) Identifier |loopInput|
//@[49:50) Colon |:|
//@[51:52) Identifier |i|
//@[53:54) Modulo |%|
//@[55:56) Integer |2|
//@[57:59) Equals |==|
//@[60:61) Integer |0|
//@[61:62) RightSquare |]|
//@[62:63) NewLine |\n|
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[0:3) Identifier |var|
//@[4:27) Identifier |arrayOfHardCodedNumbers|
//@[28:29) Assignment |=|
//@[30:31) LeftSquare |[|
//@[31:34) Identifier |for|
//@[35:36) Identifier |i|
//@[37:39) Identifier |in|
//@[40:45) Identifier |range|
//@[45:46) LeftParen |(|
//@[46:47) Integer |0|
//@[47:48) Comma |,|
//@[48:50) Integer |10|
//@[50:51) RightParen |)|
//@[51:52) Colon |:|
//@[53:54) Integer |3|
//@[54:55) RightSquare |]|
//@[55:56) NewLine |\n|
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[0:3) Identifier |var|
//@[4:25) Identifier |arrayOfHardCodedBools|
//@[26:27) Assignment |=|
//@[28:29) LeftSquare |[|
//@[29:32) Identifier |for|
//@[33:34) Identifier |i|
//@[35:37) Identifier |in|
//@[38:43) Identifier |range|
//@[43:44) LeftParen |(|
//@[44:45) Integer |0|
//@[45:46) Comma |,|
//@[46:48) Integer |10|
//@[48:49) RightParen |)|
//@[49:50) Colon |:|
//@[51:56) FalseKeyword |false|
//@[56:57) RightSquare |]|
//@[57:58) NewLine |\n|
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[0:3) Identifier |var|
//@[4:27) Identifier |arrayOfHardCodedStrings|
//@[28:29) Assignment |=|
//@[30:31) LeftSquare |[|
//@[31:34) Identifier |for|
//@[35:36) Identifier |i|
//@[37:39) Identifier |in|
//@[40:45) Identifier |range|
//@[45:46) LeftParen |(|
//@[46:47) Integer |0|
//@[47:48) Comma |,|
//@[48:49) Integer |3|
//@[49:50) RightParen |)|
//@[50:51) Colon |:|
//@[52:56) StringComplete |'hi'|
//@[56:57) RightSquare |]|
//@[57:58) NewLine |\n|
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[0:3) Identifier |var|
//@[4:34) Identifier |arrayOfNonRuntimeFunctionCalls|
//@[35:36) Assignment |=|
//@[37:38) LeftSquare |[|
//@[38:41) Identifier |for|
//@[42:43) Identifier |i|
//@[44:46) Identifier |in|
//@[47:52) Identifier |range|
//@[52:53) LeftParen |(|
//@[53:54) Integer |0|
//@[54:55) Comma |,|
//@[55:56) Integer |3|
//@[56:57) RightParen |)|
//@[57:58) Colon |:|
//@[59:65) Identifier |concat|
//@[65:66) LeftParen |(|
//@[66:70) StringComplete |'hi'|
//@[70:71) Comma |,|
//@[72:73) Identifier |i|
//@[73:74) RightParen |)|
//@[74:75) RightSquare |]|
//@[75:77) NewLine |\n\n|

var multilineString = '''
//@[0:3) Identifier |var|
//@[4:19) Identifier |multilineString|
//@[20:21) Assignment |=|
//@[22:36) MultilineString |'''\nHELLO!\n'''|
HELLO!
'''
//@[3:5) NewLine |\n\n|

var multilineEmpty = ''''''
//@[0:3) Identifier |var|
//@[4:18) Identifier |multilineEmpty|
//@[19:20) Assignment |=|
//@[21:27) MultilineString |''''''|
//@[27:28) NewLine |\n|
var multilineEmptyNewline = '''
//@[0:3) Identifier |var|
//@[4:25) Identifier |multilineEmptyNewline|
//@[26:27) Assignment |=|
//@[28:35) MultilineString |'''\n'''|
'''
//@[3:5) NewLine |\n\n|

// evaluates to '\'abc\''
//@[25:26) NewLine |\n|
var multilineExtraQuotes = ''''abc''''
//@[0:3) Identifier |var|
//@[4:24) Identifier |multilineExtraQuotes|
//@[25:26) Assignment |=|
//@[27:38) MultilineString |''''abc''''|
//@[38:40) NewLine |\n\n|

// evaluates to '\'\nabc\n\''
//@[29:30) NewLine |\n|
var multilineExtraQuotesNewlines = ''''
//@[0:3) Identifier |var|
//@[4:32) Identifier |multilineExtraQuotesNewlines|
//@[33:34) Assignment |=|
//@[35:48) MultilineString |''''\nabc\n''''|
abc
''''
//@[4:6) NewLine |\n\n|

var multilineSingleLine = '''hello!'''
//@[0:3) Identifier |var|
//@[4:23) Identifier |multilineSingleLine|
//@[24:25) Assignment |=|
//@[26:38) MultilineString |'''hello!'''|
//@[38:40) NewLine |\n\n|

var multilineFormatted = format('''
//@[0:3) Identifier |var|
//@[4:22) Identifier |multilineFormatted|
//@[23:24) Assignment |=|
//@[25:31) Identifier |format|
//@[31:32) LeftParen |(|
//@[32:61) MultilineString |'''\nHello,\nmy\nname is\n{0}\n'''|
Hello,
my
name is
{0}
''', 'Anthony')
//@[3:4) Comma |,|
//@[5:14) StringComplete |'Anthony'|
//@[14:15) RightParen |)|
//@[15:17) NewLine |\n\n|

var multilineJavaScript = '''
//@[0:3) Identifier |var|
//@[4:23) Identifier |multilineJavaScript|
//@[24:25) Assignment |=|
//@[26:586) MultilineString |'''\n// NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n'''|
// NOT RECOMMENDED PATTERN
const fs = require('fs');

module.exports = function (context) {
    fs.readFile('./hello.txt', (err, data) => {
        if (err) {
            context.log.error('ERROR', err);
            // BUG #1: This will result in an uncaught exception that crashes the entire process
            throw err;
        }
        context.log(`Data from file: ${data}`);
        // context.done() should be called here
    });
    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends
    context.done();
}
'''
//@[3:4) NewLine |\n|

//@[0:0) EndOfFile ||
