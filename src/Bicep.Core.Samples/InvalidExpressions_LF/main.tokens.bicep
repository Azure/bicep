/*
  This tests the various cases of invalid expressions.
*/
//@[2:6) NewLine |\r\n\r\n|

// bad expressions
//@[18:20) NewLine |\r\n|
var bad = a+
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) Identifier |a|
//@[11:12) Plus |+|
//@[12:14) NewLine |\r\n|
var bad = *
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) Asterisk |*|
//@[11:13) NewLine |\r\n|
var bad = /
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) Slash |/|
//@[11:13) NewLine |\r\n|
var bad = %
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) Modulo |%|
//@[11:13) NewLine |\r\n|
var bad = 33-
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:12) Number |33|
//@[12:13) Minus |-|
//@[13:15) NewLine |\r\n|
var bad = --33
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) Minus |-|
//@[11:12) Minus |-|
//@[12:14) Number |33|
//@[14:16) NewLine |\r\n|
var bad = 3 * 4 /
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) Number |3|
//@[12:13) Asterisk |*|
//@[14:15) Number |4|
//@[16:17) Slash |/|
//@[17:19) NewLine |\r\n|
var bad = 222222222222222222222222222222222222222222 * 4
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:52) Number |222222222222222222222222222222222222222222|
//@[53:54) Asterisk |*|
//@[55:56) Number |4|
//@[56:58) NewLine |\r\n|
var bad = (null) ?
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[17:18) Question |?|
//@[18:20) NewLine |\r\n|
var bad = (null) ? :
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[17:18) Question |?|
//@[19:20) Colon |:|
//@[20:22) NewLine |\r\n|
var bad = (null) ? !
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[17:18) Question |?|
//@[19:20) Exclamation |!|
//@[20:22) NewLine |\r\n|
var bad = (null)!
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[16:17) Exclamation |!|
//@[17:19) NewLine |\r\n|
var bad = (null)[0]
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:15) NullKeyword |null|
//@[15:16) RightParen |)|
//@[16:17) LeftSquare |[|
//@[17:18) Number |0|
//@[18:19) RightSquare |]|
//@[19:21) NewLine |\r\n|
var bad = ()
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:14) NewLine |\r\n|
var bad = {}
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) LeftBrace |{|
//@[11:12) RightBrace |}|
//@[12:14) NewLine |\r\n|
var bad = []
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:11) LeftSquare |[|
//@[11:12) RightSquare |]|
//@[12:14) NewLine |\r\n|
var bad = 
//@[0:3) Identifier |var|
//@[4:7) Identifier |bad|
//@[8:9) Assignment |=|
//@[10:14) NewLine |\r\n\r\n|

// variables not supported
//@[26:28) NewLine |\r\n|
var x = a + 2
//@[0:3) Identifier |var|
//@[4:5) Identifier |x|
//@[6:7) Assignment |=|
//@[8:9) Identifier |a|
//@[10:11) Plus |+|
//@[12:13) Number |2|
//@[13:17) NewLine |\r\n\r\n|

// unary NOT
//@[12:14) NewLine |\r\n|
var not = !null
//@[0:3) Identifier |var|
//@[4:7) Identifier |not|
//@[8:9) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:15) NullKeyword |null|
//@[15:17) NewLine |\r\n|
var not = !4
//@[0:3) Identifier |var|
//@[4:7) Identifier |not|
//@[8:9) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:12) Number |4|
//@[12:14) NewLine |\r\n|
var not = !'s'
//@[0:3) Identifier |var|
//@[4:7) Identifier |not|
//@[8:9) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:14) StringComplete |'s'|
//@[14:16) NewLine |\r\n|
var not = ![
//@[0:3) Identifier |var|
//@[4:7) Identifier |not|
//@[8:9) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:12) LeftSquare |[|
//@[12:14) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
var not = !{
//@[0:3) Identifier |var|
//@[4:7) Identifier |not|
//@[8:9) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:12) LeftBrace |{|
//@[12:14) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// unary not chaining will be added in the future
//@[49:51) NewLine |\r\n|
var not = !!!!!!!true
//@[0:3) Identifier |var|
//@[4:7) Identifier |not|
//@[8:9) Assignment |=|
//@[10:11) Exclamation |!|
//@[11:12) Exclamation |!|
//@[12:13) Exclamation |!|
//@[13:14) Exclamation |!|
//@[14:15) Exclamation |!|
//@[15:16) Exclamation |!|
//@[16:17) Exclamation |!|
//@[17:21) TrueKeyword |true|
//@[21:25) NewLine |\r\n\r\n|

// unary minus chaining will not be supported (to reserve -- in case we need it)
//@[80:82) NewLine |\r\n|
var minus = ------12
//@[0:3) Identifier |var|
//@[4:9) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:14) Minus |-|
//@[14:15) Minus |-|
//@[15:16) Minus |-|
//@[16:17) Minus |-|
//@[17:18) Minus |-|
//@[18:20) Number |12|
//@[20:24) NewLine |\r\n\r\n|

// unary minus
//@[14:16) NewLine |\r\n|
var minus = -true
//@[0:3) Identifier |var|
//@[4:9) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:17) TrueKeyword |true|
//@[17:19) NewLine |\r\n|
var minus = -null
//@[0:3) Identifier |var|
//@[4:9) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:17) NullKeyword |null|
//@[17:19) NewLine |\r\n|
var minus = -'s'
//@[0:3) Identifier |var|
//@[4:9) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:16) StringComplete |'s'|
//@[16:18) NewLine |\r\n|
var minus = -[
//@[0:3) Identifier |var|
//@[4:9) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
var minus = -{
//@[0:3) Identifier |var|
//@[4:9) Identifier |minus|
//@[10:11) Assignment |=|
//@[12:13) Minus |-|
//@[13:14) LeftBrace |{|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// multiplicative
//@[17:19) NewLine |\r\n|
var mod = 's' % true
//@[0:3) Identifier |var|
//@[4:7) Identifier |mod|
//@[8:9) Assignment |=|
//@[10:13) StringComplete |'s'|
//@[14:15) Modulo |%|
//@[16:20) TrueKeyword |true|
//@[20:22) NewLine |\r\n|
var mul = true * null
//@[0:3) Identifier |var|
//@[4:7) Identifier |mul|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[15:16) Asterisk |*|
//@[17:21) NullKeyword |null|
//@[21:23) NewLine |\r\n|
var div = {
//@[0:3) Identifier |var|
//@[4:7) Identifier |div|
//@[8:9) Assignment |=|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
} / [
//@[0:1) RightBrace |}|
//@[2:3) Slash |/|
//@[4:5) LeftSquare |[|
//@[5:7) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

// additive
//@[11:13) NewLine |\r\n|
var add = null + 's'
//@[0:3) Identifier |var|
//@[4:7) Identifier |add|
//@[8:9) Assignment |=|
//@[10:14) NullKeyword |null|
//@[15:16) Plus |+|
//@[17:20) StringComplete |'s'|
//@[20:22) NewLine |\r\n|
var sub = true - false
//@[0:3) Identifier |var|
//@[4:7) Identifier |sub|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[15:16) Minus |-|
//@[17:22) FalseKeyword |false|
//@[22:26) NewLine |\r\n\r\n|

// equality (== and != can't have a type error because they work on "any" type)
//@[79:81) NewLine |\r\n|
var eq = true =~ null
//@[0:3) Identifier |var|
//@[4:6) Identifier |eq|
//@[7:8) Assignment |=|
//@[9:13) TrueKeyword |true|
//@[14:16) EqualsInsensitive |=~|
//@[17:21) NullKeyword |null|
//@[21:23) NewLine |\r\n|
var ne = 15 !~ [
//@[0:3) Identifier |var|
//@[4:6) Identifier |ne|
//@[7:8) Assignment |=|
//@[9:11) Number |15|
//@[12:14) NotEqualsInsensitive |!~|
//@[15:16) LeftSquare |[|
//@[16:18) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

// relational
//@[13:15) NewLine |\r\n|
var lt = 4 < 's'
//@[0:3) Identifier |var|
//@[4:6) Identifier |lt|
//@[7:8) Assignment |=|
//@[9:10) Number |4|
//@[11:12) LessThan |<|
//@[13:16) StringComplete |'s'|
//@[16:18) NewLine |\r\n|
var lteq = null <= 10
//@[0:3) Identifier |var|
//@[4:8) Identifier |lteq|
//@[9:10) Assignment |=|
//@[11:15) NullKeyword |null|
//@[16:18) LessThanOrEqual |<=|
//@[19:21) Number |10|
//@[21:23) NewLine |\r\n|
var gt = false>[
//@[0:3) Identifier |var|
//@[4:6) Identifier |gt|
//@[7:8) Assignment |=|
//@[9:14) FalseKeyword |false|
//@[14:15) GreaterThan |>|
//@[15:16) LeftSquare |[|
//@[16:18) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
var gteq = {
//@[0:3) Identifier |var|
//@[4:8) Identifier |gteq|
//@[9:10) Assignment |=|
//@[11:12) LeftBrace |{|
//@[12:14) NewLine |\r\n|
} >= false
//@[0:1) RightBrace |}|
//@[2:4) GreaterThanOrEqual |>=|
//@[5:10) FalseKeyword |false|
//@[10:14) NewLine |\r\n\r\n|

// logical
//@[10:12) NewLine |\r\n|
var and = null && 'a'
//@[0:3) Identifier |var|
//@[4:7) Identifier |and|
//@[8:9) Assignment |=|
//@[10:14) NullKeyword |null|
//@[15:17) LogicalAnd |&&|
//@[18:21) StringComplete |'a'|
//@[21:23) NewLine |\r\n|
var or = 10 || 4
//@[0:3) Identifier |var|
//@[4:6) Identifier |or|
//@[7:8) Assignment |=|
//@[9:11) Number |10|
//@[12:14) LogicalOr ||||
//@[15:16) Number |4|
//@[16:20) NewLine |\r\n\r\n|

// conditional
//@[14:16) NewLine |\r\n|
var ternary = null ? 4 : false
//@[0:3) Identifier |var|
//@[4:11) Identifier |ternary|
//@[12:13) Assignment |=|
//@[14:18) NullKeyword |null|
//@[19:20) Question |?|
//@[21:22) Number |4|
//@[23:24) Colon |:|
//@[25:30) FalseKeyword |false|
//@[30:34) NewLine |\r\n\r\n|

// complex expressions
//@[22:24) NewLine |\r\n|
var complex = test(2 + 3*4, true || false && null)
//@[0:3) Identifier |var|
//@[4:11) Identifier |complex|
//@[12:13) Assignment |=|
//@[14:18) Identifier |test|
//@[18:19) LeftParen |(|
//@[19:20) Number |2|
//@[21:22) Plus |+|
//@[23:24) Number |3|
//@[24:25) Asterisk |*|
//@[25:26) Number |4|
//@[26:27) Comma |,|
//@[28:32) TrueKeyword |true|
//@[33:35) LogicalOr ||||
//@[36:41) FalseKeyword |false|
//@[42:44) LogicalAnd |&&|
//@[45:49) NullKeyword |null|
//@[49:50) RightParen |)|
//@[50:52) NewLine |\r\n|
var complex = -2 && 3 && !4 && 5
//@[0:3) Identifier |var|
//@[4:11) Identifier |complex|
//@[12:13) Assignment |=|
//@[14:15) Minus |-|
//@[15:16) Number |2|
//@[17:19) LogicalAnd |&&|
//@[20:21) Number |3|
//@[22:24) LogicalAnd |&&|
//@[25:26) Exclamation |!|
//@[26:27) Number |4|
//@[28:30) LogicalAnd |&&|
//@[31:32) Number |5|
//@[32:34) NewLine |\r\n|
var complex = null ? !4: false
//@[0:3) Identifier |var|
//@[4:11) Identifier |complex|
//@[12:13) Assignment |=|
//@[14:18) NullKeyword |null|
//@[19:20) Question |?|
//@[21:22) Exclamation |!|
//@[22:23) Number |4|
//@[23:24) Colon |:|
//@[25:30) FalseKeyword |false|
//@[30:32) NewLine |\r\n|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[0:3) Identifier |var|
//@[4:11) Identifier |complex|
//@[12:13) Assignment |=|
//@[14:18) TrueKeyword |true|
//@[19:21) Equals |==|
//@[22:27) FalseKeyword |false|
//@[28:30) NotEquals |!=|
//@[31:35) NullKeyword |null|
//@[36:38) Equals |==|
//@[39:40) Number |4|
//@[41:43) NotEquals |!=|
//@[44:47) StringComplete |'a'|
//@[48:49) Question |?|
//@[50:51) Minus |-|
//@[51:52) Number |2|
//@[53:55) LogicalAnd |&&|
//@[56:57) Number |3|
//@[58:60) LogicalAnd |&&|
//@[61:62) Exclamation |!|
//@[62:63) Number |4|
//@[64:66) LogicalAnd |&&|
//@[67:68) Number |5|
//@[69:70) Colon |:|
//@[71:75) TrueKeyword |true|
//@[76:78) LogicalOr ||||
//@[79:84) FalseKeyword |false|
//@[85:87) LogicalAnd |&&|
//@[88:92) NullKeyword |null|
//@[92:96) NewLine |\r\n\r\n|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[0:3) Identifier |var|
//@[4:17) Identifier |nestedTernary|
//@[18:19) Assignment |=|
//@[20:24) NullKeyword |null|
//@[25:26) Question |?|
//@[27:28) Number |1|
//@[29:30) Colon |:|
//@[31:32) Number |2|
//@[33:34) Question |?|
//@[35:39) TrueKeyword |true|
//@[40:41) Question |?|
//@[42:45) StringComplete |'a'|
//@[45:46) Colon |:|
//@[47:50) StringComplete |'b'|
//@[51:52) Colon |:|
//@[53:58) FalseKeyword |false|
//@[59:60) Question |?|
//@[61:64) StringComplete |'d'|
//@[65:66) Colon |:|
//@[67:69) Number |15|
//@[69:71) NewLine |\r\n|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[0:3) Identifier |var|
//@[4:17) Identifier |nestedTernary|
//@[18:19) Assignment |=|
//@[20:21) LeftParen |(|
//@[21:25) NullKeyword |null|
//@[26:27) Question |?|
//@[28:29) Number |1|
//@[30:31) Colon |:|
//@[32:33) Number |2|
//@[33:34) RightParen |)|
//@[35:36) Question |?|
//@[37:38) LeftParen |(|
//@[38:42) TrueKeyword |true|
//@[43:44) Question |?|
//@[45:48) StringComplete |'a'|
//@[48:49) Colon |:|
//@[50:53) StringComplete |'b'|
//@[53:54) RightParen |)|
//@[55:56) Colon |:|
//@[57:58) LeftParen |(|
//@[58:63) FalseKeyword |false|
//@[64:65) Question |?|
//@[66:69) StringComplete |'d'|
//@[70:71) Colon |:|
//@[72:74) Number |15|
//@[74:75) RightParen |)|
//@[75:79) NewLine |\r\n\r\n|

// bad array access
//@[19:21) NewLine |\r\n|
var errorInsideArrayAccess = [
//@[0:3) Identifier |var|
//@[4:26) Identifier |errorInsideArrayAccess|
//@[27:28) Assignment |=|
//@[29:30) LeftSquare |[|
//@[30:32) NewLine |\r\n|
  !null
//@[2:3) Exclamation |!|
//@[3:7) NullKeyword |null|
//@[7:9) NewLine |\r\n|
][!0]
//@[0:1) RightSquare |]|
//@[1:2) LeftSquare |[|
//@[2:3) Exclamation |!|
//@[3:4) Number |0|
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
var integerIndexOnNonArray = (null)[0]
//@[0:3) Identifier |var|
//@[4:26) Identifier |integerIndexOnNonArray|
//@[27:28) Assignment |=|
//@[29:30) LeftParen |(|
//@[30:34) NullKeyword |null|
//@[34:35) RightParen |)|
//@[35:36) LeftSquare |[|
//@[36:37) Number |0|
//@[37:38) RightSquare |]|
//@[38:40) NewLine |\r\n|
var stringIndexOnNonObject = 'test'['test']
//@[0:3) Identifier |var|
//@[4:26) Identifier |stringIndexOnNonObject|
//@[27:28) Assignment |=|
//@[29:35) StringComplete |'test'|
//@[35:36) LeftSquare |[|
//@[36:42) StringComplete |'test'|
//@[42:43) RightSquare |]|
//@[43:45) NewLine |\r\n|
var malformedStringIndex = {
//@[0:3) Identifier |var|
//@[4:24) Identifier |malformedStringIndex|
//@[25:26) Assignment |=|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\r\n|
}['test\e']
//@[0:1) RightBrace |}|
//@[1:2) LeftSquare |[|
//@[2:10) StringComplete |'test\e'|
//@[10:11) RightSquare |]|
//@[11:15) NewLine |\r\n\r\n|

// bad propertyAccess
//@[21:23) NewLine |\r\n|
var dotAccessOnNonObject = true.foo
//@[0:3) Identifier |var|
//@[4:24) Identifier |dotAccessOnNonObject|
//@[25:26) Assignment |=|
//@[27:31) TrueKeyword |true|
//@[31:32) Dot |.|
//@[32:35) Identifier |foo|
//@[35:37) NewLine |\r\n|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[0:3) Identifier |var|
//@[4:33) Identifier |badExpressionInPropertyAccess|
//@[34:35) Assignment |=|
//@[36:49) Identifier |resourceGroup|
//@[49:50) LeftParen |(|
//@[50:51) RightParen |)|
//@[51:52) LeftSquare |[|
//@[52:53) Exclamation |!|
//@[53:63) StringComplete |'location'|
//@[63:64) RightSquare |]|
//@[64:68) NewLine |\r\n\r\n|

var propertyAccessOnVariable = x.foo
//@[0:3) Identifier |var|
//@[4:28) Identifier |propertyAccessOnVariable|
//@[29:30) Assignment |=|
//@[31:32) Identifier |x|
//@[32:33) Dot |.|
//@[33:36) Identifier |foo|
//@[36:40) NewLine |\r\n\r\n|

// function used like a variable
//@[32:34) NewLine |\r\n|
var funcvarvar = concat + base64 || !uniqueString
//@[0:3) Identifier |var|
//@[4:14) Identifier |funcvarvar|
//@[15:16) Assignment |=|
//@[17:23) Identifier |concat|
//@[24:25) Plus |+|
//@[26:32) Identifier |base64|
//@[33:35) LogicalOr ||||
//@[36:37) Exclamation |!|
//@[37:49) Identifier |uniqueString|
//@[49:51) NewLine |\r\n|
param funcvarparam bool = concat
//@[0:5) Identifier |param|
//@[6:18) Identifier |funcvarparam|
//@[19:23) Identifier |bool|
//@[24:25) Assignment |=|
//@[26:32) Identifier |concat|
//@[32:34) NewLine |\r\n|
output funcvarout array = padLeft
//@[0:6) Identifier |output|
//@[7:17) Identifier |funcvarout|
//@[18:23) Identifier |array|
//@[24:25) Assignment |=|
//@[26:33) Identifier |padLeft|
//@[33:37) NewLine |\r\n\r\n|

// non-existent function
//@[24:26) NewLine |\r\n|
var fakeFunc = red() + green() * orange()
//@[0:3) Identifier |var|
//@[4:12) Identifier |fakeFunc|
//@[13:14) Assignment |=|
//@[15:18) Identifier |red|
//@[18:19) LeftParen |(|
//@[19:20) RightParen |)|
//@[21:22) Plus |+|
//@[23:28) Identifier |green|
//@[28:29) LeftParen |(|
//@[29:30) RightParen |)|
//@[31:32) Asterisk |*|
//@[33:39) Identifier |orange|
//@[39:40) LeftParen |(|
//@[40:41) RightParen |)|
//@[41:43) NewLine |\r\n|
param fakeFuncP string {
//@[0:5) Identifier |param|
//@[6:15) Identifier |fakeFuncP|
//@[16:22) Identifier |string|
//@[23:24) LeftBrace |{|
//@[24:26) NewLine |\r\n|
  default: blue()
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:15) Identifier |blue|
//@[15:16) LeftParen |(|
//@[16:17) RightParen |)|
//@[17:19) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// non-existent variable
//@[24:26) NewLine |\r\n|
var fakeVar = concat(totallyFakeVar, 's')
//@[0:3) Identifier |var|
//@[4:11) Identifier |fakeVar|
//@[12:13) Assignment |=|
//@[14:20) Identifier |concat|
//@[20:21) LeftParen |(|
//@[21:35) Identifier |totallyFakeVar|
//@[35:36) Comma |,|
//@[37:40) StringComplete |'s'|
//@[40:41) RightParen |)|
//@[41:45) NewLine |\r\n\r\n|

// bad functions arguments
//@[26:28) NewLine |\r\n|
var concatNotEnough = concat()
//@[0:3) Identifier |var|
//@[4:19) Identifier |concatNotEnough|
//@[20:21) Assignment |=|
//@[22:28) Identifier |concat|
//@[28:29) LeftParen |(|
//@[29:30) RightParen |)|
//@[30:32) NewLine |\r\n|
var padLeftNotEnough = padLeft('s')
//@[0:3) Identifier |var|
//@[4:20) Identifier |padLeftNotEnough|
//@[21:22) Assignment |=|
//@[23:30) Identifier |padLeft|
//@[30:31) LeftParen |(|
//@[31:34) StringComplete |'s'|
//@[34:35) RightParen |)|
//@[35:37) NewLine |\r\n|
var takeTooMany = take([
//@[0:3) Identifier |var|
//@[4:15) Identifier |takeTooMany|
//@[16:17) Assignment |=|
//@[18:22) Identifier |take|
//@[22:23) LeftParen |(|
//@[23:24) LeftSquare |[|
//@[24:26) NewLine |\r\n|
],1,2,'s')
//@[0:1) RightSquare |]|
//@[1:2) Comma |,|
//@[2:3) Number |1|
//@[3:4) Comma |,|
//@[4:5) Number |2|
//@[5:6) Comma |,|
//@[6:9) StringComplete |'s'|
//@[9:10) RightParen |)|
//@[10:14) NewLine |\r\n\r\n|

// wrong argument types
//@[23:25) NewLine |\r\n|
var concatWrongTypes = concat({
//@[0:3) Identifier |var|
//@[4:20) Identifier |concatWrongTypes|
//@[21:22) Assignment |=|
//@[23:29) Identifier |concat|
//@[29:30) LeftParen |(|
//@[30:31) LeftBrace |{|
//@[31:33) NewLine |\r\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:4) NewLine |\r\n|
var concatWrongTypesContradiction = concat('s', [
//@[0:3) Identifier |var|
//@[4:33) Identifier |concatWrongTypesContradiction|
//@[34:35) Assignment |=|
//@[36:42) Identifier |concat|
//@[42:43) LeftParen |(|
//@[43:46) StringComplete |'s'|
//@[46:47) Comma |,|
//@[48:49) LeftSquare |[|
//@[49:51) NewLine |\r\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:4) NewLine |\r\n|
var indexOfWrongTypes = indexOf(1,1)
//@[0:3) Identifier |var|
//@[4:21) Identifier |indexOfWrongTypes|
//@[22:23) Assignment |=|
//@[24:31) Identifier |indexOf|
//@[31:32) LeftParen |(|
//@[32:33) Number |1|
//@[33:34) Comma |,|
//@[34:35) Number |1|
//@[35:36) RightParen |)|
//@[36:40) NewLine |\r\n\r\n|

// not enough params
//@[20:22) NewLine |\r\n|
var test1 = listKeys('abcd')
//@[0:3) Identifier |var|
//@[4:9) Identifier |test1|
//@[10:11) Assignment |=|
//@[12:20) Identifier |listKeys|
//@[20:21) LeftParen |(|
//@[21:27) StringComplete |'abcd'|
//@[27:28) RightParen |)|
//@[28:32) NewLine |\r\n\r\n|

// list spelled wrong 
//@[22:24) NewLine |\r\n|
var test2 = lsitKeys('abcd', '2020-01-01')
//@[0:3) Identifier |var|
//@[4:9) Identifier |test2|
//@[10:11) Assignment |=|
//@[12:20) Identifier |lsitKeys|
//@[20:21) LeftParen |(|
//@[21:27) StringComplete |'abcd'|
//@[27:28) Comma |,|
//@[29:41) StringComplete |'2020-01-01'|
//@[41:42) RightParen |)|
//@[42:46) NewLine |\r\n\r\n|

// just 'list' 
//@[15:17) NewLine |\r\n|
var test3 = list('abcd', '2020-01-01')
//@[0:3) Identifier |var|
//@[4:9) Identifier |test3|
//@[10:11) Assignment |=|
//@[12:16) Identifier |list|
//@[16:17) LeftParen |(|
//@[17:23) StringComplete |'abcd'|
//@[23:24) Comma |,|
//@[25:37) StringComplete |'2020-01-01'|
//@[37:38) RightParen |)|
//@[38:38) EndOfFile ||
