
//@[0:1) NewLine |\n|
// unknown declaration
//@[22:23) NewLine |\n|
bad
//@[0:3) Identifier |bad|
//@[3:5) NewLine |\n\n|

// incomplete variable declaration
//@[34:35) NewLine |\n|
var
//@[0:3) Identifier |var|
//@[3:5) NewLine |\n\n|

// unassigned variable
//@[22:23) NewLine |\n|
var foo
//@[0:3) Identifier |var|
//@[4:7) Identifier |foo|
//@[7:9) NewLine |\n\n|

// no value assigned
//@[20:21) NewLine |\n|
var foo =
//@[0:3) Identifier |var|
//@[4:7) Identifier |foo|
//@[8:9) Assignment |=|
//@[9:11) NewLine |\n\n|

// bad token value
//@[18:19) NewLine |\n|
var foo = &
//@[0:3) Identifier |var|
//@[4:7) Identifier |foo|
//@[8:9) Assignment |=|
//@[10:11) Unrecognized |&|
//@[11:13) NewLine |\n\n|

// bad value
//@[12:13) NewLine |\n|
var foo = *
//@[0:3) Identifier |var|
//@[4:7) Identifier |foo|
//@[8:9) Assignment |=|
//@[10:11) Asterisk |*|
//@[11:13) NewLine |\n\n|

// expressions
//@[14:15) NewLine |\n|
var bar = x
//@[0:3) Identifier |var|
//@[4:7) Identifier |bar|
//@[8:9) Assignment |=|
//@[10:11) Identifier |x|
//@[11:12) NewLine |\n|
var bar = foo()
//@[0:3) Identifier |var|
//@[4:7) Identifier |bar|
//@[8:9) Assignment |=|
//@[10:13) Identifier |foo|
//@[13:14) LeftParen |(|
//@[14:15) RightParen |)|
//@[15:16) NewLine |\n|
var x = 2 + !3
//@[0:3) Identifier |var|
//@[4:5) Identifier |x|
//@[6:7) Assignment |=|
//@[8:9) Number |2|
//@[10:11) Plus |+|
//@[12:13) Exclamation |!|
//@[13:14) Number |3|
//@[14:15) NewLine |\n|
var y = false ? true + 1 : !4
//@[0:3) Identifier |var|
//@[4:5) Identifier |y|
//@[6:7) Assignment |=|
//@[8:13) FalseKeyword |false|
//@[14:15) Question |?|
//@[16:20) TrueKeyword |true|
//@[21:22) Plus |+|
//@[23:24) Number |1|
//@[25:26) Colon |:|
//@[27:28) Exclamation |!|
//@[28:29) Number |4|
//@[29:31) NewLine |\n\n|

// test for array item recovery
//@[31:32) NewLine |\n|
var x = [
//@[0:3) Identifier |var|
//@[4:5) Identifier |x|
//@[6:7) Assignment |=|
//@[8:9) LeftSquare |[|
//@[9:10) NewLine |\n|
  3 + 4
//@[2:3) Number |3|
//@[4:5) Plus |+|
//@[6:7) Number |4|
//@[7:8) NewLine |\n|
  =
//@[2:3) Assignment |=|
//@[3:4) NewLine |\n|
  !null
//@[2:3) Exclamation |!|
//@[3:7) NullKeyword |null|
//@[7:8) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

// test for object property recovery
//@[36:37) NewLine |\n|
var y = {
//@[0:3) Identifier |var|
//@[4:5) Identifier |y|
//@[6:7) Assignment |=|
//@[8:9) LeftBrace |{|
//@[9:10) NewLine |\n|
  =
//@[2:3) Assignment |=|
//@[3:4) NewLine |\n|
  foo: !2
//@[2:5) Identifier |foo|
//@[5:6) Colon |:|
//@[7:8) Exclamation |!|
//@[8:9) Number |2|
//@[9:10) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// utcNow and newGuid used outside a param default value
//@[56:57) NewLine |\n|
var test = utcNow('u')
//@[0:3) Identifier |var|
//@[4:8) Identifier |test|
//@[9:10) Assignment |=|
//@[11:17) Identifier |utcNow|
//@[17:18) LeftParen |(|
//@[18:21) StringComplete |'u'|
//@[21:22) RightParen |)|
//@[22:23) NewLine |\n|
var test2 = newGuid()
//@[0:3) Identifier |var|
//@[4:9) Identifier |test2|
//@[10:11) Assignment |=|
//@[12:19) Identifier |newGuid|
//@[19:20) LeftParen |(|
//@[20:21) RightParen |)|
//@[21:23) NewLine |\n\n|

// bad string escape sequence in object key
//@[43:44) NewLine |\n|
var test3 = {
//@[0:3) Identifier |var|
//@[4:9) Identifier |test3|
//@[10:11) Assignment |=|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
  'bad\escape': true
//@[2:14) StringComplete |'bad\escape'|
//@[14:15) Colon |:|
//@[16:20) TrueKeyword |true|
//@[20:21) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// duplicate properties
//@[23:24) NewLine |\n|
var testDupe = {
//@[0:3) Identifier |var|
//@[4:12) Identifier |testDupe|
//@[13:14) Assignment |=|
//@[15:16) LeftBrace |{|
//@[16:17) NewLine |\n|
  'duplicate': true
//@[2:13) StringComplete |'duplicate'|
//@[13:14) Colon |:|
//@[15:19) TrueKeyword |true|
//@[19:20) NewLine |\n|
  duplicate: true
//@[2:11) Identifier |duplicate|
//@[11:12) Colon |:|
//@[13:17) TrueKeyword |true|
//@[17:18) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:1) EndOfFile ||
