
//@[0:2) NewLine |\r\n|
// wrong declaration
//@[20:22) NewLine |\r\n|
bad
//@[0:3) Identifier |bad|
//@[3:7) NewLine |\r\n\r\n|

// incomplete #completionTest(9) -> empty
//@[41:43) NewLine |\r\n|
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
//@[18:22) NewLine |\r\n\r\n|

// #completionTest(23) -> resourceTypes
//@[39:41) NewLine |\r\n|
resource trailingSpace  
//@[0:8) Identifier |resource|
//@[9:22) Identifier |trailingSpace|
//@[24:28) NewLine |\r\n\r\n|

// #completionTest(19,20) -> object
//@[35:37) NewLine |\r\n|
resource foo 'ddd'= 
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:18) StringComplete |'ddd'|
//@[18:19) Assignment |=|
//@[20:24) NewLine |\r\n\r\n|

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

resource foo 'ddd'=if (1 + 1 == 2) {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:18) StringComplete |'ddd'|
//@[18:19) Assignment |=|
//@[19:21) Identifier |if|
//@[22:23) LeftParen |(|
//@[23:24) Number |1|
//@[25:26) Plus |+|
//@[27:28) Number |1|
//@[29:31) Equals |==|
//@[32:33) Number |2|
//@[33:34) RightParen |)|
//@[35:36) LeftBrace |{|
//@[36:38) NewLine |\r\n|
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

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:26) StringLeftPiece |'Microsoft.${|
//@[26:34) Identifier |provider|
//@[34:58) StringRightPiece |}/foos@2020-02-02-alpha'|
//@[58:59) Assignment |=|
//@[60:62) Identifier |if|
//@[63:64) LeftParen |(|
//@[64:68) TrueKeyword |true|
//@[68:69) RightParen |)|
//@[70:71) LeftBrace |{|
//@[71:73) NewLine |\r\n|
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

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[56:60) Identifier |name|
//@[61:63) Equals |==|
//@[64:71) StringComplete |'value'|
//@[71:72) RightParen |)|
//@[73:74) LeftBrace |{|
//@[74:76) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[56:57) LeftBrace |{|
//@[58:61) StringComplete |'a'|
//@[61:62) Colon |:|
//@[63:64) Identifier |b|
//@[65:66) RightBrace |}|
//@[66:67) Dot |.|
//@[67:68) Identifier |a|
//@[69:71) Equals |==|
//@[72:77) StringComplete |'foo'|
//@[77:78) RightParen |)|
//@[79:80) LeftBrace |{|
//@[80:82) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// simulate typing if condition
//@[31:33) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[54:58) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[56:60) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[56:60) TrueKeyword |true|
//@[60:64) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[56:60) TrueKeyword |true|
//@[60:61) RightParen |)|
//@[61:65) NewLine |\r\n\r\n|

// missing condition
//@[20:22) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftBrace |{|
//@[56:58) NewLine |\r\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:15) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// empty condition
//@[18:20) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[56:57) RightParen |)|
//@[58:59) LeftBrace |{|
//@[59:61) NewLine |\r\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:15) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// invalid condition type
//@[25:27) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[56:59) Number |123|
//@[59:60) RightParen |)|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:15) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// runtime functions are no allowed in resource conditions
//@[58:60) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[51:52) Assignment |=|
//@[53:55) Identifier |if|
//@[56:57) LeftParen |(|
//@[57:66) Identifier |reference|
//@[66:67) LeftParen |(|
//@[67:109) StringComplete |'Micorosft.Management/managementGroups/MG'|
//@[109:110) Comma |,|
//@[111:123) StringComplete |'2020-05-01'|
//@[123:124) RightParen |)|
//@[124:125) Dot |.|
//@[125:129) Identifier |name|
//@[130:132) Equals |==|
//@[133:144) StringComplete |'something'|
//@[144:145) RightParen |)|
//@[146:147) LeftBrace |{|
//@[147:149) NewLine |\r\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:15) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[51:52) Assignment |=|
//@[53:55) Identifier |if|
//@[56:57) LeftParen |(|
//@[57:65) Identifier |listKeys|
//@[65:66) LeftParen |(|
//@[66:71) StringComplete |'foo'|
//@[71:72) Comma |,|
//@[73:85) StringComplete |'2020-05-01'|
//@[85:86) RightParen |)|
//@[86:87) Dot |.|
//@[87:90) Identifier |bar|
//@[91:93) Equals |==|
//@[94:98) TrueKeyword |true|
//@[98:99) RightParen |)|
//@[100:101) LeftBrace |{|
//@[101:103) NewLine |\r\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:15) NewLine |\r\n|
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

// duplicate property at the top level with string literal syntax
//@[65:67) NewLine |\r\n|
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
  'name': true
//@[2:8) StringComplete |'name'|
//@[8:9) Colon |:|
//@[10:14) TrueKeyword |true|
//@[14:16) NewLine |\r\n|
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

// duplicate property inside with string literal syntax
//@[55:57) NewLine |\r\n|
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
    'foo': 'a'
//@[4:9) StringComplete |'foo'|
//@[9:10) Colon |:|
//@[11:14) StringComplete |'a'|
//@[14:16) NewLine |\r\n|
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
//@[30:34) NewLine |\r\n\r\n|

// attempting to set read-only properties
//@[41:43) NewLine |\r\n|
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |baz|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[51:52) Assignment |=|
//@[53:54) LeftBrace |{|
//@[54:56) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  id: 2
//@[2:4) Identifier |id|
//@[4:5) Colon |:|
//@[6:7) Number |2|
//@[7:9) NewLine |\r\n|
  type: 'hello'
//@[2:6) Identifier |type|
//@[6:7) Colon |:|
//@[8:15) StringComplete |'hello'|
//@[15:17) NewLine |\r\n|
  apiVersion: true
//@[2:12) Identifier |apiVersion|
//@[12:13) Colon |:|
//@[14:18) TrueKeyword |true|
//@[18:20) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:8) Identifier |resource|
//@[9:19) Identifier |badDepends|
//@[20:57) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[58:59) Assignment |=|
//@[60:61) LeftBrace |{|
//@[61:63) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    baz.id
//@[4:7) Identifier |baz|
//@[7:8) Dot |.|
//@[8:10) Identifier |id|
//@[10:12) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:8) Identifier |resource|
//@[9:20) Identifier |badDepends2|
//@[21:58) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    'hello'
//@[4:11) StringComplete |'hello'|
//@[11:13) NewLine |\r\n|
    true
//@[4:8) TrueKeyword |true|
//@[8:10) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:8) Identifier |resource|
//@[9:20) Identifier |badDepends3|
//@[21:58) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:8) Identifier |resource|
//@[9:20) Identifier |badDepends4|
//@[21:58) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    badDepends3
//@[4:15) Identifier |badDepends3|
//@[15:17) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:8) Identifier |resource|
//@[9:20) Identifier |badDepends5|
//@[21:58) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  dependsOn: badDepends3.dependsOn
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:24) Identifier |badDepends3|
//@[24:25) Dot |.|
//@[25:34) Identifier |dependsOn|
//@[34:36) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var interpVal = 'abc'
//@[0:3) Identifier |var|
//@[4:13) Identifier |interpVal|
//@[14:15) Assignment |=|
//@[16:21) StringComplete |'abc'|
//@[21:23) NewLine |\r\n|
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:8) Identifier |resource|
//@[9:18) Identifier |badInterp|
//@[19:56) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[57:58) Assignment |=|
//@[59:60) LeftBrace |{|
//@[60:62) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
//@[2:5) StringLeftPiece |'${|
//@[5:14) Identifier |interpVal|
//@[14:16) StringRightPiece |}'|
//@[16:17) Colon |:|
//@[18:31) StringComplete |'unsupported'|
//@[94:96) NewLine |\r\n|
  '${undefinedSymbol}': true
//@[2:5) StringLeftPiece |'${|
//@[5:20) Identifier |undefinedSymbol|
//@[20:22) StringRightPiece |}'|
//@[22:23) Colon |:|
//@[24:28) TrueKeyword |true|
//@[28:30) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module validModule './module.bicep' = {
//@[0:6) Identifier |module|
//@[7:18) Identifier |validModule|
//@[19:35) StringComplete |'./module.bicep'|
//@[36:37) Assignment |=|
//@[38:39) LeftBrace |{|
//@[39:41) NewLine |\r\n|
  name: 'storageDeploy'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:23) StringComplete |'storageDeploy'|
//@[23:25) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    name: 'contoso'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:19) StringComplete |'contoso'|
//@[19:21) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes1|
//@[26:72) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[73:74) Assignment |=|
//@[75:76) LeftBrace |{|
//@[76:78) NewLine |\r\n|
  name: 'name1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) StringComplete |'name1'|
//@[15:17) NewLine |\r\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:22) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    evictionPolicy: 'Deallocate'
//@[4:18) Identifier |evictionPolicy|
//@[18:19) Colon |:|
//@[20:32) StringComplete |'Deallocate'|
//@[32:34) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes2|
//@[26:76) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[77:78) Assignment |=|
//@[79:80) LeftBrace |{|
//@[80:82) NewLine |\r\n|
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |concat|
//@[14:15) LeftParen |(|
//@[15:21) Identifier |concat|
//@[21:22) LeftParen |(|
//@[22:38) Identifier |runtimeValidRes1|
//@[38:39) Dot |.|
//@[39:41) Identifier |id|
//@[41:42) Comma |,|
//@[43:59) Identifier |runtimeValidRes1|
//@[59:60) Dot |.|
//@[60:64) Identifier |name|
//@[64:65) RightParen |)|
//@[65:66) Comma |,|
//@[67:83) Identifier |runtimeValidRes1|
//@[83:84) Dot |.|
//@[84:88) Identifier |type|
//@[88:89) RightParen |)|
//@[89:91) NewLine |\r\n|
  kind:'AzureCLI'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[7:17) StringComplete |'AzureCLI'|
//@[17:19) NewLine |\r\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:22) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    azCliVersion: '2.0'
//@[4:16) Identifier |azCliVersion|
//@[16:17) Colon |:|
//@[18:23) StringComplete |'2.0'|
//@[23:25) NewLine |\r\n|
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
//@[4:21) Identifier |retentionInterval|
//@[21:22) Colon |:|
//@[23:39) Identifier |runtimeValidRes1|
//@[39:40) Dot |.|
//@[40:50) Identifier |properties|
//@[50:51) Dot |.|
//@[51:65) Identifier |evictionPolicy|
//@[65:67) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes3 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes3|
//@[26:85) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
  name: '${runtimeValidRes1.name}_v1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:27) Identifier |runtimeValidRes1|
//@[27:28) Dot |.|
//@[28:32) Identifier |name|
//@[32:37) StringRightPiece |}_v1'|
//@[37:39) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes4|
//@[26:85) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
  name: concat(validModule['name'], 'v1')
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |concat|
//@[14:15) LeftParen |(|
//@[15:26) Identifier |validModule|
//@[26:27) LeftSquare |[|
//@[27:33) StringComplete |'name'|
//@[33:34) RightSquare |]|
//@[34:35) Comma |,|
//@[36:40) StringComplete |'v1'|
//@[40:41) RightParen |)|
//@[41:43) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes5|
//@[26:85) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
  name: '${validModule.name}_v1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:22) Identifier |validModule|
//@[22:23) Dot |.|
//@[23:27) Identifier |name|
//@[27:32) StringRightPiece |}_v1'|
//@[32:34) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes1|
//@[28:87) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeValidRes1.location
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:33) Identifier |location|
//@[33:35) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes2|
//@[28:87) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeValidRes1['location']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) LeftSquare |[|
//@[25:35) StringComplete |'location'|
//@[35:36) RightSquare |]|
//@[36:38) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes3|
//@[28:78) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[79:80) Assignment |=|
//@[81:82) LeftBrace |{|
//@[82:84) NewLine |\r\n|
  name: runtimeValidRes1.properties.evictionPolicy
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:35) Identifier |properties|
//@[35:36) Dot |.|
//@[36:50) Identifier |evictionPolicy|
//@[50:52) NewLine |\r\n|
  kind:'AzureCLI'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[7:17) StringComplete |'AzureCLI'|
//@[17:19) NewLine |\r\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:22) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    azCliVersion: '2.0'
//@[4:16) Identifier |azCliVersion|
//@[16:17) Colon |:|
//@[18:23) StringComplete |'2.0'|
//@[23:25) NewLine |\r\n|
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
//@[4:21) Identifier |retentionInterval|
//@[21:22) Colon |:|
//@[23:39) Identifier |runtimeValidRes1|
//@[39:40) Dot |.|
//@[40:50) Identifier |properties|
//@[50:51) Dot |.|
//@[51:65) Identifier |evictionPolicy|
//@[65:67) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes4|
//@[28:87) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeValidRes1['properties'].evictionPolicy
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) LeftSquare |[|
//@[25:37) StringComplete |'properties'|
//@[37:38) RightSquare |]|
//@[38:39) Dot |.|
//@[39:53) Identifier |evictionPolicy|
//@[53:55) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes5|
//@[28:87) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeValidRes1['properties']['evictionPolicy']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) LeftSquare |[|
//@[25:37) StringComplete |'properties'|
//@[37:38) RightSquare |]|
//@[38:39) LeftSquare |[|
//@[39:55) StringComplete |'evictionPolicy'|
//@[55:56) RightSquare |]|
//@[56:58) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes6|
//@[28:87) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeValidRes1.properties['evictionPolicy']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:35) Identifier |properties|
//@[35:36) LeftSquare |[|
//@[36:52) StringComplete |'evictionPolicy'|
//@[52:53) RightSquare |]|
//@[53:55) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes7|
//@[28:87) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeValidRes2.properties.azCliVersion
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes2|
//@[24:25) Dot |.|
//@[25:35) Identifier |properties|
//@[35:36) Dot |.|
//@[36:48) Identifier |azCliVersion|
//@[48:50) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var magicString1 = 'location'
//@[0:3) Identifier |var|
//@[4:16) Identifier |magicString1|
//@[17:18) Assignment |=|
//@[19:29) StringComplete |'location'|
//@[29:31) NewLine |\r\n|
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes8|
//@[28:87) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeValidRes2['${magicString1}']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes2|
//@[24:25) LeftSquare |[|
//@[25:28) StringLeftPiece |'${|
//@[28:40) Identifier |magicString1|
//@[40:42) StringRightPiece |}'|
//@[42:43) RightSquare |]|
//@[43:45) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// note: this should be fine, but we block string interpolation all together if there's a potential runtime property usage for name.
//@[132:134) NewLine |\r\n|
var magicString2 = 'name'
//@[0:3) Identifier |var|
//@[4:16) Identifier |magicString2|
//@[17:18) Assignment |=|
//@[19:25) StringComplete |'name'|
//@[25:27) NewLine |\r\n|
resource runtimeInvalidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |runtimeInvalidRes9|
//@[28:87) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeValidRes2['${magicString2}']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes2|
//@[24:25) LeftSquare |[|
//@[25:28) StringLeftPiece |'${|
//@[28:40) Identifier |magicString2|
//@[40:42) StringRightPiece |}'|
//@[42:43) RightSquare |]|
//@[43:45) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes10|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: '${runtimeValidRes3.location}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:27) Identifier |runtimeValidRes3|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:38) StringRightPiece |}'|
//@[38:40) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes11|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: validModule.params['name']
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:19) Identifier |validModule|
//@[19:20) Dot |.|
//@[20:26) Identifier |params|
//@[26:27) LeftSquare |[|
//@[27:33) StringComplete |'name'|
//@[33:34) RightSquare |]|
//@[34:36) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes12|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |concat|
//@[14:15) LeftParen |(|
//@[15:31) Identifier |runtimeValidRes1|
//@[31:32) Dot |.|
//@[32:40) Identifier |location|
//@[40:41) Comma |,|
//@[42:58) Identifier |runtimeValidRes2|
//@[58:59) LeftSquare |[|
//@[59:69) StringComplete |'location'|
//@[69:70) RightSquare |]|
//@[70:71) Comma |,|
//@[72:90) Identifier |runtimeInvalidRes3|
//@[90:91) LeftSquare |[|
//@[91:103) StringComplete |'properties'|
//@[103:104) RightSquare |]|
//@[104:105) Dot |.|
//@[105:117) Identifier |azCliVersion|
//@[117:118) Comma |,|
//@[119:130) Identifier |validModule|
//@[130:131) Dot |.|
//@[131:137) Identifier |params|
//@[137:138) Dot |.|
//@[138:142) Identifier |name|
//@[142:143) RightParen |)|
//@[143:145) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes13|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:27) Identifier |runtimeValidRes1|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:39) StringMiddlePiece |}${|
//@[39:55) Identifier |runtimeValidRes2|
//@[55:56) LeftSquare |[|
//@[56:66) StringComplete |'location'|
//@[66:67) RightSquare |]|
//@[67:70) StringMiddlePiece |}${|
//@[70:88) Identifier |runtimeInvalidRes3|
//@[88:89) Dot |.|
//@[89:99) Identifier |properties|
//@[99:100) LeftSquare |[|
//@[100:114) StringComplete |'azCliVersion'|
//@[114:115) RightSquare |]|
//@[115:118) StringMiddlePiece |}${|
//@[118:129) Identifier |validModule|
//@[129:130) LeftSquare |[|
//@[130:138) StringComplete |'params'|
//@[138:139) RightSquare |]|
//@[139:140) Dot |.|
//@[140:144) Identifier |name|
//@[144:146) StringRightPiece |}'|
//@[146:148) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// variable related runtime validation
//@[38:40) NewLine |\r\n|
var runtimefoo1 = runtimeValidRes1['location']
//@[0:3) Identifier |var|
//@[4:15) Identifier |runtimefoo1|
//@[16:17) Assignment |=|
//@[18:34) Identifier |runtimeValidRes1|
//@[34:35) LeftSquare |[|
//@[35:45) StringComplete |'location'|
//@[45:46) RightSquare |]|
//@[46:48) NewLine |\r\n|
var runtimefoo2 = runtimeValidRes2['properties'].azCliVersion
//@[0:3) Identifier |var|
//@[4:15) Identifier |runtimefoo2|
//@[16:17) Assignment |=|
//@[18:34) Identifier |runtimeValidRes2|
//@[34:35) LeftSquare |[|
//@[35:47) StringComplete |'properties'|
//@[47:48) RightSquare |]|
//@[48:49) Dot |.|
//@[49:61) Identifier |azCliVersion|
//@[61:63) NewLine |\r\n|
var runtimefoo3 = runtimeValidRes2
//@[0:3) Identifier |var|
//@[4:15) Identifier |runtimefoo3|
//@[16:17) Assignment |=|
//@[18:34) Identifier |runtimeValidRes2|
//@[34:38) NewLine |\r\n\r\n|

var runtimeInvalid = {
//@[0:3) Identifier |var|
//@[4:18) Identifier |runtimeInvalid|
//@[19:20) Assignment |=|
//@[21:22) LeftBrace |{|
//@[22:24) NewLine |\r\n|
  foo1: runtimefoo1
//@[2:6) Identifier |foo1|
//@[6:7) Colon |:|
//@[8:19) Identifier |runtimefoo1|
//@[19:21) NewLine |\r\n|
  foo2: runtimefoo2
//@[2:6) Identifier |foo2|
//@[6:7) Colon |:|
//@[8:19) Identifier |runtimefoo2|
//@[19:21) NewLine |\r\n|
  foo3: runtimefoo3
//@[2:6) Identifier |foo3|
//@[6:7) Colon |:|
//@[8:19) Identifier |runtimefoo3|
//@[19:21) NewLine |\r\n|
  foo4: runtimeValidRes1.name
//@[2:6) Identifier |foo4|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:29) Identifier |name|
//@[29:31) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var runtimeValid = {
//@[0:3) Identifier |var|
//@[4:16) Identifier |runtimeValid|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
  foo1: runtimeValidRes1.name
//@[2:6) Identifier |foo1|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:29) Identifier |name|
//@[29:31) NewLine |\r\n|
  foo2: runtimeValidRes1.apiVersion
//@[2:6) Identifier |foo2|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes1|
//@[24:25) Dot |.|
//@[25:35) Identifier |apiVersion|
//@[35:37) NewLine |\r\n|
  foo3: runtimeValidRes2.type
//@[2:6) Identifier |foo3|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes2|
//@[24:25) Dot |.|
//@[25:29) Identifier |type|
//@[29:31) NewLine |\r\n|
  foo4: runtimeValidRes2.id
//@[2:6) Identifier |foo4|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeValidRes2|
//@[24:25) Dot |.|
//@[25:27) Identifier |id|
//@[27:29) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes14 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes14|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: runtimeInvalid.foo1
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) Identifier |runtimeInvalid|
//@[22:23) Dot |.|
//@[23:27) Identifier |foo1|
//@[27:29) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes15|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: runtimeInvalid.foo2
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) Identifier |runtimeInvalid|
//@[22:23) Dot |.|
//@[23:27) Identifier |foo2|
//@[27:29) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes16|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: runtimeInvalid.foo3.properties.azCliVersion
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) Identifier |runtimeInvalid|
//@[22:23) Dot |.|
//@[23:27) Identifier |foo3|
//@[27:28) Dot |.|
//@[28:38) Identifier |properties|
//@[38:39) Dot |.|
//@[39:51) Identifier |azCliVersion|
//@[51:53) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
//@[128:130) NewLine |\r\n|
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes17|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: runtimeInvalid.foo4
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) Identifier |runtimeInvalid|
//@[22:23) Dot |.|
//@[23:27) Identifier |foo4|
//@[27:29) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:26) Identifier |runtimeValidRes18|
//@[27:86) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftBrace |{|
//@[90:92) NewLine |\r\n|
  name: runtimeValid.foo1
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:20) Identifier |runtimeValid|
//@[20:21) Dot |.|
//@[21:25) Identifier |foo1|
//@[25:27) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes19 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:26) Identifier |runtimeValidRes19|
//@[27:86) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftBrace |{|
//@[90:92) NewLine |\r\n|
  name: runtimeValid.foo2
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:20) Identifier |runtimeValid|
//@[20:21) Dot |.|
//@[21:25) Identifier |foo2|
//@[25:27) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes20 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:26) Identifier |runtimeValidRes20|
//@[27:86) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftBrace |{|
//@[90:92) NewLine |\r\n|
  name: runtimeValid.foo3
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:20) Identifier |runtimeValid|
//@[20:21) Dot |.|
//@[21:25) Identifier |foo3|
//@[25:27) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes21 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:26) Identifier |runtimeValidRes21|
//@[27:86) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftBrace |{|
//@[90:92) NewLine |\r\n|
  name: runtimeValid.foo4
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:20) Identifier |runtimeValid|
//@[20:21) Dot |.|
//@[21:25) Identifier |foo4|
//@[25:27) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |missingTopLevelProperties|
//@[35:89) StringComplete |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[90:91) Assignment |=|
//@[92:93) LeftBrace |{|
//@[93:95) NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelProperties
//@[51:55) NewLine |\r\n\r\n|

}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[0:8) Identifier |resource|
//@[9:44) Identifier |missingTopLevelPropertiesExceptName|
//@[45:99) StringComplete |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[100:101) Assignment |=|
//@[102:103) LeftBrace |{|
//@[103:105) NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
//@[60:62) NewLine |\r\n|
  name: 'me'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) StringComplete |'me'|
//@[12:14) NewLine |\r\n|
  // do not remove whitespace before the closing curly
//@[54:56) NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
//@[60:62) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// #completionTest(24,25,26,49,65) -> resourceTypes
//@[51:53) NewLine |\r\n|
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:23) Identifier |unfinishedVnet|
//@[24:70) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[71:72) Assignment |=|
//@[73:74) LeftBrace |{|
//@[74:76) NewLine |\r\n|
  name: 'v'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringComplete |'v'|
//@[11:13) NewLine |\r\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:22) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    subnets: [
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
      {
//@[6:7) LeftBrace |{|
//@[7:9) NewLine |\r\n|
        // #completionTest(0,1,2,3,4,5,6,7) -> subnetPropertiesMinusProperties
//@[78:80) NewLine |\r\n|
        properties: {
//@[8:18) Identifier |properties|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:23) NewLine |\r\n|
          delegations: [
//@[10:21) Identifier |delegations|
//@[21:22) Colon |:|
//@[23:24) LeftSquare |[|
//@[24:26) NewLine |\r\n|
            {
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
              // #completionTest(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14) -> delegationProperties
//@[92:94) NewLine |\r\n|
              
//@[14:16) NewLine |\r\n|
            }
//@[12:13) RightBrace |}|
//@[13:15) NewLine |\r\n|
          ]
//@[10:11) RightSquare |]|
//@[11:13) NewLine |\r\n|
        }
//@[8:9) RightBrace |}|
//@[9:11) NewLine |\r\n|
      }
//@[6:7) RightBrace |}|
//@[7:9) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource discriminatorKeyMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:32) Identifier |discriminatorKeyMissing|
//@[33:83) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[84:85) Assignment |=|
//@[86:87) LeftBrace |{|
//@[87:89) NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[52:54) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:37) Identifier |discriminatorKeyValueMissing|
//@[38:88) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
//@[66:68) NewLine |\r\n|
  kind:   
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[10:12) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[0:3) Identifier |var|
//@[4:43) Identifier |discriminatorKeyValueMissingCompletions|
//@[44:45) Assignment |=|
//@[46:74) Identifier |discriminatorKeyValueMissing|
//@[74:75) Dot |.|
//@[75:76) Identifier |p|
//@[76:78) NewLine |\r\n|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[0:3) Identifier |var|
//@[4:44) Identifier |discriminatorKeyValueMissingCompletions2|
//@[45:46) Assignment |=|
//@[47:75) Identifier |discriminatorKeyValueMissing|
//@[75:76) Dot |.|
//@[76:80) NewLine |\r\n\r\n|

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
//@[70:72) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[0:3) Identifier |var|
//@[4:44) Identifier |discriminatorKeyValueMissingCompletions3|
//@[45:46) Assignment |=|
//@[47:75) Identifier |discriminatorKeyValueMissing|
//@[75:76) LeftSquare |[|
//@[76:77) RightSquare |]|
//@[77:81) NewLine |\r\n\r\n|

resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:31) Identifier |discriminatorKeySetOne|
//@[32:82) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[83:84) Assignment |=|
//@[85:86) LeftBrace |{|
//@[86:88) NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:18) StringComplete |'AzureCLI'|
//@[18:20) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59) NewLine |\r\n\r\n|

  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[66:68) NewLine |\r\n|
    
//@[4:6) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(75) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
//@[0:3) Identifier |var|
//@[4:37) Identifier |discriminatorKeySetOneCompletions|
//@[38:39) Assignment |=|
//@[40:62) Identifier |discriminatorKeySetOne|
//@[62:63) Dot |.|
//@[63:73) Identifier |properties|
//@[73:74) Dot |.|
//@[74:75) Identifier |a|
//@[75:77) NewLine |\r\n|
// #completionTest(75) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[0:3) Identifier |var|
//@[4:38) Identifier |discriminatorKeySetOneCompletions2|
//@[39:40) Assignment |=|
//@[41:63) Identifier |discriminatorKeySetOne|
//@[63:64) Dot |.|
//@[64:74) Identifier |properties|
//@[74:75) Dot |.|
//@[75:79) NewLine |\r\n\r\n|

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
//@[61:63) NewLine |\r\n|
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[0:3) Identifier |var|
//@[4:38) Identifier |discriminatorKeySetOneCompletions3|
//@[39:40) Assignment |=|
//@[41:63) Identifier |discriminatorKeySetOne|
//@[63:64) Dot |.|
//@[64:74) Identifier |properties|
//@[74:75) LeftSquare |[|
//@[75:76) RightSquare |]|
//@[76:80) NewLine |\r\n\r\n|

resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:31) Identifier |discriminatorKeySetTwo|
//@[32:82) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[83:84) Assignment |=|
//@[85:86) LeftBrace |{|
//@[86:88) NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:25) StringComplete |'AzurePowerShell'|
//@[25:27) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59) NewLine |\r\n\r\n|

  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[65:67) NewLine |\r\n|
    
//@[4:6) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(75) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
//@[0:3) Identifier |var|
//@[4:37) Identifier |discriminatorKeySetTwoCompletions|
//@[38:39) Assignment |=|
//@[40:62) Identifier |discriminatorKeySetTwo|
//@[62:63) Dot |.|
//@[63:73) Identifier |properties|
//@[73:74) Dot |.|
//@[74:75) Identifier |a|
//@[75:77) NewLine |\r\n|
// #completionTest(75) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[0:3) Identifier |var|
//@[4:38) Identifier |discriminatorKeySetTwoCompletions2|
//@[39:40) Assignment |=|
//@[41:63) Identifier |discriminatorKeySetTwo|
//@[63:64) Dot |.|
//@[64:74) Identifier |properties|
//@[74:75) Dot |.|
//@[75:79) NewLine |\r\n\r\n|

// #completionTest(90) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
//@[0:3) Identifier |var|
//@[4:49) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer|
//@[50:51) Assignment |=|
//@[52:74) Identifier |discriminatorKeySetTwo|
//@[74:75) LeftSquare |[|
//@[75:87) StringComplete |'properties'|
//@[87:88) RightSquare |]|
//@[88:89) Dot |.|
//@[89:90) Identifier |a|
//@[90:92) NewLine |\r\n|
// #completionTest(90) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[0:3) Identifier |var|
//@[4:50) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2|
//@[51:52) Assignment |=|
//@[53:75) Identifier |discriminatorKeySetTwo|
//@[75:76) LeftSquare |[|
//@[76:88) StringComplete |'properties'|
//@[88:89) RightSquare |]|
//@[89:90) Dot |.|
//@[90:94) NewLine |\r\n\r\n|

resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:31) Identifier |incorrectPropertiesKey|
//@[32:82) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[83:84) Assignment |=|
//@[85:86) LeftBrace |{|
//@[86:88) NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:18) StringComplete |'AzureCLI'|
//@[18:22) NewLine |\r\n\r\n|

  propertes: {
//@[2:11) Identifier |propertes|
//@[11:12) Colon |:|
//@[13:14) LeftBrace |{|
//@[14:16) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var mock = incorrectPropertiesKey.p
//@[0:3) Identifier |var|
//@[4:8) Identifier |mock|
//@[9:10) Assignment |=|
//@[11:33) Identifier |incorrectPropertiesKey|
//@[33:34) Dot |.|
//@[34:35) Identifier |p|
//@[35:39) NewLine |\r\n\r\n|

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:32) Identifier |incorrectPropertiesKey2|
//@[33:83) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[84:85) Assignment |=|
//@[86:87) LeftBrace |{|
//@[87:89) NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:18) StringComplete |'AzureCLI'|
//@[18:20) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  location: ''
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:14) StringComplete |''|
//@[14:16) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    azCliVersion: '2'
//@[4:16) Identifier |azCliVersion|
//@[16:17) Colon |:|
//@[18:21) StringComplete |'2'|
//@[21:23) NewLine |\r\n|
    retentionInterval: 'PT1H'
//@[4:21) Identifier |retentionInterval|
//@[21:22) Colon |:|
//@[23:29) StringComplete |'PT1H'|
//@[29:31) NewLine |\r\n|
    
//@[4:6) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliPropertiesMinusSpecified
//@[80:82) NewLine |\r\n|
    
//@[4:6) NewLine |\r\n|
    // #completionTest(22,23) -> cleanupPreferencesPlusSymbols
//@[62:64) NewLine |\r\n|
    cleanupPreference: 
//@[4:21) Identifier |cleanupPreference|
//@[21:22) Colon |:|
//@[23:27) NewLine |\r\n\r\n|

    // #completionTest(25,26) -> arrayPlusSymbols
//@[49:51) NewLine |\r\n|
    supportingScriptUris: 
//@[4:24) Identifier |supportingScriptUris|
//@[24:25) Colon |:|
//@[26:30) NewLine |\r\n\r\n|

    // #completionTest(27,28) -> objectPlusSymbols
//@[50:52) NewLine |\r\n|
    storageAccountSettings: 
//@[4:26) Identifier |storageAccountSettings|
//@[26:27) Colon |:|
//@[28:32) NewLine |\r\n\r\n|

    environmentVariables: [
//@[4:24) Identifier |environmentVariables|
//@[24:25) Colon |:|
//@[26:27) LeftSquare |[|
//@[27:29) NewLine |\r\n|
      {
//@[6:7) LeftBrace |{|
//@[7:9) NewLine |\r\n|
        // #completionTest(0,2,4,6,8) -> environmentVariableProperties
//@[70:72) NewLine |\r\n|
        
//@[8:10) NewLine |\r\n|
      }
//@[6:7) RightBrace |}|
//@[7:9) NewLine |\r\n|
      // #completionTest(0,1,2,3,4,5,6) -> objectPlusSymbols
//@[60:62) NewLine |\r\n|
      
//@[6:8) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// #completionTest(21) -> resourceTypes
//@[39:41) NewLine |\r\n|
resource missingType 
//@[0:8) Identifier |resource|
//@[9:20) Identifier |missingType|
//@[21:25) NewLine |\r\n\r\n|

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
//@[60:62) NewLine |\r\n|
resource startedTypingTypeWithQuotes 'virma'
//@[0:8) Identifier |resource|
//@[9:36) Identifier |startedTypingTypeWithQuotes|
//@[37:44) StringComplete |'virma'|
//@[44:48) NewLine |\r\n\r\n|

// #completionTest(40,41,42,43,44,45) -> resourceTypes
//@[54:56) NewLine |\r\n|
resource startedTypingTypeWithoutQuotes virma
//@[0:8) Identifier |resource|
//@[9:39) Identifier |startedTypingTypeWithoutQuotes|
//@[40:45) Identifier |virma|
//@[45:49) NewLine |\r\n\r\n|

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[0:8) Identifier |resource|
//@[9:30) Identifier |dashesInPropertyNames|
//@[31:86) StringComplete |'Microsoft.ContainerService/managedClusters@2020-09-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftBrace |{|
//@[90:92) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
//@[61:63) NewLine |\r\n|
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
//@[0:3) Identifier |var|
//@[4:23) Identifier |letsAccessTheDashes|
//@[24:25) Assignment |=|
//@[26:47) Identifier |dashesInPropertyNames|
//@[47:48) Dot |.|
//@[48:58) Identifier |properties|
//@[58:59) Dot |.|
//@[59:76) Identifier |autoScalerProfile|
//@[76:77) Dot |.|
//@[77:78) Identifier |s|
//@[78:80) NewLine |\r\n|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
//@[61:63) NewLine |\r\n|
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[0:3) Identifier |var|
//@[4:24) Identifier |letsAccessTheDashes2|
//@[25:26) Assignment |=|
//@[27:48) Identifier |dashesInPropertyNames|
//@[48:49) Dot |.|
//@[49:59) Identifier |properties|
//@[59:60) Dot |.|
//@[60:77) Identifier |autoScalerProfile|
//@[77:78) Dot |.|
//@[78:82) NewLine |\r\n\r\n|

resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[0:8) Identifier |resource|
//@[9:38) Identifier |nestedDiscriminatorMissingKey|
//@[39:97) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[98:99) Assignment |=|
//@[100:101) LeftBrace |{|
//@[101:103) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  location: 'l'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:15) StringComplete |'l'|
//@[15:17) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    //createMode: 'Default'
//@[27:31) NewLine |\r\n\r\n|

  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(90) -> createMode
//@[36:38) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
//@[0:3) Identifier |var|
//@[4:44) Identifier |nestedDiscriminatorMissingKeyCompletions|
//@[45:46) Assignment |=|
//@[47:76) Identifier |nestedDiscriminatorMissingKey|
//@[76:77) Dot |.|
//@[77:87) Identifier |properties|
//@[87:88) Dot |.|
//@[88:90) Identifier |cr|
//@[90:92) NewLine |\r\n|
// #completionTest(92) -> createMode
//@[36:38) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[0:3) Identifier |var|
//@[4:45) Identifier |nestedDiscriminatorMissingKeyCompletions2|
//@[46:47) Assignment |=|
//@[48:77) Identifier |nestedDiscriminatorMissingKey|
//@[77:78) LeftSquare |[|
//@[78:90) StringComplete |'properties'|
//@[90:91) RightSquare |]|
//@[91:92) Dot |.|
//@[92:96) NewLine |\r\n\r\n|

// #completionTest(94) -> createModeIndexPlusSymbols
//@[52:54) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[0:3) Identifier |var|
//@[4:49) Identifier |nestedDiscriminatorMissingKeyIndexCompletions|
//@[50:51) Assignment |=|
//@[52:81) Identifier |nestedDiscriminatorMissingKey|
//@[81:82) Dot |.|
//@[82:92) Identifier |properties|
//@[92:93) LeftSquare |[|
//@[93:95) StringComplete |''|
//@[95:96) RightSquare |]|
//@[96:100) NewLine |\r\n\r\n|

resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |nestedDiscriminator|
//@[29:87) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  location: 'l'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:15) StringComplete |'l'|
//@[15:17) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    createMode: 'Default'
//@[4:14) Identifier |createMode|
//@[14:15) Colon |:|
//@[16:25) StringComplete |'Default'|
//@[25:29) NewLine |\r\n\r\n|

  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(69) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
//@[0:3) Identifier |var|
//@[4:34) Identifier |nestedDiscriminatorCompletions|
//@[35:36) Assignment |=|
//@[37:56) Identifier |nestedDiscriminator|
//@[56:57) Dot |.|
//@[57:67) Identifier |properties|
//@[67:68) Dot |.|
//@[68:69) Identifier |a|
//@[69:71) NewLine |\r\n|
// #completionTest(73) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
//@[0:3) Identifier |var|
//@[4:35) Identifier |nestedDiscriminatorCompletions2|
//@[36:37) Assignment |=|
//@[38:57) Identifier |nestedDiscriminator|
//@[57:58) LeftSquare |[|
//@[58:70) StringComplete |'properties'|
//@[70:71) RightSquare |]|
//@[71:72) Dot |.|
//@[72:73) Identifier |a|
//@[73:75) NewLine |\r\n|
// #completionTest(69) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[0:3) Identifier |var|
//@[4:35) Identifier |nestedDiscriminatorCompletions3|
//@[36:37) Assignment |=|
//@[38:57) Identifier |nestedDiscriminator|
//@[57:58) Dot |.|
//@[58:68) Identifier |properties|
//@[68:69) Dot |.|
//@[69:71) NewLine |\r\n|
// #completionTest(72) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[0:3) Identifier |var|
//@[4:35) Identifier |nestedDiscriminatorCompletions4|
//@[36:37) Assignment |=|
//@[38:57) Identifier |nestedDiscriminator|
//@[57:58) LeftSquare |[|
//@[58:70) StringComplete |'properties'|
//@[70:71) RightSquare |]|
//@[71:72) Dot |.|
//@[72:76) NewLine |\r\n\r\n|

// #completionTest(79) -> defaultCreateModeIndexes
//@[50:52) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[0:3) Identifier |var|
//@[4:44) Identifier |nestedDiscriminatorArrayIndexCompletions|
//@[45:46) Assignment |=|
//@[47:66) Identifier |nestedDiscriminator|
//@[66:67) Dot |.|
//@[67:77) Identifier |properties|
//@[77:78) LeftSquare |[|
//@[78:79) Identifier |a|
//@[79:80) RightSquare |]|
//@[80:84) NewLine |\r\n\r\n|

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
//@[0:8) Identifier |resource|
//@[9:18) Identifier |selfScope|
//@[19:50) StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[51:52) Assignment |=|
//@[53:54) LeftBrace |{|
//@[54:56) NewLine |\r\n|
  name: 'selfScope'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'selfScope'|
//@[19:21) NewLine |\r\n|
  scope: selfScope
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:18) Identifier |selfScope|
//@[18:20) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var notAResource = {
//@[0:3) Identifier |var|
//@[4:16) Identifier |notAResource|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
  im: 'not'
//@[2:4) Identifier |im|
//@[4:5) Colon |:|
//@[6:11) StringComplete |'not'|
//@[11:13) NewLine |\r\n|
  a: 'resource!'
//@[2:3) Identifier |a|
//@[3:4) Colon |:|
//@[5:16) StringComplete |'resource!'|
//@[16:18) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
//@[0:8) Identifier |resource|
//@[9:21) Identifier |invalidScope|
//@[22:53) StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[54:55) Assignment |=|
//@[56:57) LeftBrace |{|
//@[57:59) NewLine |\r\n|
  name: 'invalidScope'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'invalidScope'|
//@[22:24) NewLine |\r\n|
  scope: notAResource
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:21) Identifier |notAResource|
//@[21:23) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
//@[0:8) Identifier |resource|
//@[9:22) Identifier |invalidScope2|
//@[23:54) StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[55:56) Assignment |=|
//@[57:58) LeftBrace |{|
//@[58:60) NewLine |\r\n|
  name: 'invalidScope2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:23) StringComplete |'invalidScope2'|
//@[23:25) NewLine |\r\n|
  scope: resourceGroup()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:24) RightParen |)|
//@[24:26) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
//@[0:8) Identifier |resource|
//@[9:22) Identifier |invalidScope3|
//@[23:54) StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[55:56) Assignment |=|
//@[57:58) LeftBrace |{|
//@[58:60) NewLine |\r\n|
  name: 'invalidScope3'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:23) StringComplete |'invalidScope3'|
//@[23:25) NewLine |\r\n|
  scope: subscription()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:23) RightParen |)|
//@[23:25) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:1) EndOfFile ||
