
//@[0:2) NewLine |\r\n|
// wrong declaration
//@[20:22) NewLine |\r\n|
bad
//@[0:3) Identifier |bad|
//@[3:7) NewLine |\r\n\r\n|

// incomplete
//@[13:15) NewLine |\r\n|
resource 
//@[0:8) Identifier |resource|
//@[9:11) NewLine |\r\n|
resource foo
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[12:14) NewLine |\r\n|
resource fo/o
//@[0:8) Identifier |resource|
//@[9:11) Identifier |fo|
//@[11:12) Slash |/|
//@[12:13) Identifier |o|
//@[13:15) NewLine |\r\n|
resource foo 'ddd'
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:18) StringComplete |'ddd'|
//@[18:20) NewLine |\r\n|
resource foo 'ddd'=
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:18) StringComplete |'ddd'|
//@[18:19) Assignment |=|
//@[19:23) NewLine |\r\n\r\n|

// wrong resource type
//@[22:24) NewLine |\r\n|
resource foo 'ddd'={
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:18) StringComplete |'ddd'|
//@[18:19) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// using string interpolation for the resource type
//@[51:53) NewLine |\r\n|
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:26) StringLeftPiece |'Microsoft.${|
//@[26:34) Identifier |provider|
//@[34:58) StringRightPiece |}/foos@2020-02-02-alpha'|
//@[58:59) Assignment |=|
//@[60:61) LeftBrace |{|
//@[61:63) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// missing required property
//@[28:30) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[51:52) LeftBrace |{|
//@[52:54) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// duplicate property at the top level
//@[38:40) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:53) LeftBrace |{|
//@[53:55) NewLine |\r\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:15) NewLine |\r\n|
  name: true
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) TrueKeyword |true|
//@[12:14) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// duplicate property inside
//@[28:30) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:53) LeftBrace |{|
//@[53:55) NewLine |\r\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:15) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    foo: 'a'
//@[4:7) Identifier |foo|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'a'|
//@[12:14) NewLine |\r\n|
    foo: 'a'
//@[4:7) Identifier |foo|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'a'|
//@[12:14) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// wrong property types
//@[23:25) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:53) LeftBrace |{|
//@[53:55) NewLine |\r\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:15) NewLine |\r\n|
  location: [
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:13) LeftSquare |[|
//@[13:15) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
  tags: 'tag are not a string?'
//@[2:6) Identifier |tags|
//@[6:7) Colon |:|
//@[8:31) StringComplete |'tag are not a string?'|
//@[31:33) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |bar|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[51:52) Assignment |=|
//@[53:54) LeftBrace |{|
//@[54:56) NewLine |\r\n|
  name: true ? 's' : 'a' + 1
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) TrueKeyword |true|
//@[13:14) Question |?|
//@[15:18) StringComplete |'s'|
//@[19:20) Colon |:|
//@[21:24) StringComplete |'a'|
//@[25:26) Plus |+|
//@[27:28) Number |1|
//@[28:30) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    x: foo()
//@[4:5) Identifier |x|
//@[5:6) Colon |:|
//@[7:10) Identifier |foo|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:14) NewLine |\r\n|
    y: true && (null || !4)
//@[4:5) Identifier |y|
//@[5:6) Colon |:|
//@[7:11) TrueKeyword |true|
//@[12:14) LogicalAnd |&&|
//@[15:16) LeftParen |(|
//@[16:20) NullKeyword |null|
//@[21:23) LogicalOr ||||
//@[24:25) Exclamation |!|
//@[25:26) Number |4|
//@[26:27) RightParen |)|
//@[27:29) NewLine |\r\n|
    a: [
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:8) LeftSquare |[|
//@[8:10) NewLine |\r\n|
      a
//@[6:7) Identifier |a|
//@[7:9) NewLine |\r\n|
      !null
//@[6:7) Exclamation |!|
//@[7:11) NullKeyword |null|
//@[11:13) NewLine |\r\n|
      true && true || true + -true * 4
//@[6:10) TrueKeyword |true|
//@[11:13) LogicalAnd |&&|
//@[14:18) TrueKeyword |true|
//@[19:21) LogicalOr ||||
//@[22:26) TrueKeyword |true|
//@[27:28) Plus |+|
//@[29:30) Minus |-|
//@[30:34) TrueKeyword |true|
//@[35:36) Asterisk |*|
//@[37:38) Number |4|
//@[38:40) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// unsupported resource ref
//@[27:29) NewLine |\r\n|
var resrefvar = bar.name
//@[0:3) Identifier |var|
//@[4:13) Identifier |resrefvar|
//@[14:15) Assignment |=|
//@[16:19) Identifier |bar|
//@[19:20) Dot |.|
//@[20:24) Identifier |name|
//@[24:28) NewLine |\r\n\r\n|

param resrefpar string = foo.id
//@[0:5) Identifier |param|
//@[6:15) Identifier |resrefpar|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:28) Identifier |foo|
//@[28:29) Dot |.|
//@[29:31) Identifier |id|
//@[31:35) NewLine |\r\n\r\n|

output resrefout bool = bar.id
//@[0:6) Identifier |output|
//@[7:16) Identifier |resrefout|
//@[17:21) Identifier |bool|
//@[22:23) Assignment |=|
//@[24:27) Identifier |bar|
//@[27:28) Dot |.|
//@[28:30) Identifier |id|
//@[30:30) EndOfFile ||
