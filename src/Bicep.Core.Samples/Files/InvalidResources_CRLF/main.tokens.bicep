
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
//@[23:24) Integer |1|
//@[25:26) Plus |+|
//@[27:28) Integer |1|
//@[29:31) Equals |==|
//@[32:33) Integer |2|
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
// #completionTest(56) -> symbols
//@[33:35) NewLine |\r\n|
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

// #completionTest(57, 59) -> symbols
//@[37:39) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:50) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51) Assignment |=|
//@[52:54) Identifier |if|
//@[55:56) LeftParen |(|
//@[61:62) RightParen |)|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
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
//@[56:59) Integer |123|
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
//@[27:28) Integer |1|
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
//@[25:26) Integer |4|
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
//@[37:38) Integer |4|
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
//@[6:7) Integer |2|
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
//@[34:36) NewLine |\r\n|
var runtimefoo4 = {
//@[0:3) Identifier |var|
//@[4:15) Identifier |runtimefoo4|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[19:21) NewLine |\r\n|
  hop: runtimefoo2
//@[2:5) Identifier |hop|
//@[5:6) Colon |:|
//@[7:18) Identifier |runtimefoo2|
//@[18:20) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

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

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |runtimeInvalidRes18|
//@[29:88) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90) Assignment |=|
//@[91:92) LeftBrace |{|
//@[92:94) NewLine |\r\n|
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |concat|
//@[14:15) LeftParen |(|
//@[15:29) Identifier |runtimeInvalid|
//@[29:30) Dot |.|
//@[30:34) Identifier |foo1|
//@[34:35) Comma |,|
//@[36:52) Identifier |runtimeValidRes2|
//@[52:53) LeftSquare |[|
//@[53:65) StringComplete |'properties'|
//@[65:66) RightSquare |]|
//@[66:67) Dot |.|
//@[67:79) Identifier |azCliVersion|
//@[79:80) Comma |,|
//@[81:84) StringLeftPiece |'${|
//@[84:100) Identifier |runtimeValidRes1|
//@[100:101) Dot |.|
//@[101:109) Identifier |location|
//@[109:111) StringRightPiece |}'|
//@[111:112) Comma |,|
//@[113:124) Identifier |runtimefoo4|
//@[124:125) Dot |.|
//@[125:128) Identifier |hop|
//@[128:129) RightParen |)|
//@[129:131) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes6|
//@[26:85) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
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

resource runtimeValidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes7|
//@[26:85) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
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

resource runtimeValidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes8|
//@[26:85) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
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

resource runtimeValidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |runtimeValidRes9|
//@[26:85) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
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

/*
Discriminator key missing
*/
//@[2:4) NewLine |\r\n|
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

/*
Discriminator key missing (conditional)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |discriminatorKeyMissing_if|
//@[36:86) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[87:88) Assignment |=|
//@[89:91) Identifier |if|
//@[91:92) LeftParen |(|
//@[92:96) TrueKeyword |true|
//@[96:97) RightParen |)|
//@[98:99) LeftBrace |{|
//@[99:101) NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[52:54) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

/*
Discriminator key missing (loop)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:36) Identifier |discriminatorKeyMissing_for|
//@[37:87) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[88:89) Assignment |=|
//@[90:91) LeftSquare |[|
//@[91:94) Identifier |for|
//@[95:100) Identifier |thing|
//@[101:103) Identifier |in|
//@[104:105) LeftSquare |[|
//@[105:106) RightSquare |]|
//@[106:107) Colon |:|
//@[108:109) LeftBrace |{|
//@[109:111) NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[52:54) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access
*/
//@[2:4) NewLine |\r\n|
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

/*
Discriminator key value missing with property access (conditional)
*/
//@[2:6) NewLine |\r\n\r\n|

resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
//@[0:8) Identifier |resource|
//@[9:40) Identifier |discriminatorKeyValueMissing_if|
//@[41:91) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[92:93) Assignment |=|
//@[94:96) Identifier |if|
//@[96:97) LeftParen |(|
//@[97:102) FalseKeyword |false|
//@[102:103) RightParen |)|
//@[104:105) LeftBrace |{|
//@[105:107) NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
//@[69:71) NewLine |\r\n|
  kind:   
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[10:12) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
//@[0:3) Identifier |var|
//@[4:46) Identifier |discriminatorKeyValueMissingCompletions_if|
//@[47:48) Assignment |=|
//@[49:80) Identifier |discriminatorKeyValueMissing_if|
//@[80:81) Dot |.|
//@[81:82) Identifier |p|
//@[82:84) NewLine |\r\n|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.
//@[0:3) Identifier |var|
//@[4:47) Identifier |discriminatorKeyValueMissingCompletions2_if|
//@[48:49) Assignment |=|
//@[50:81) Identifier |discriminatorKeyValueMissing_if|
//@[81:82) Dot |.|
//@[82:86) NewLine |\r\n\r\n|

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
//@[73:75) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]
//@[0:3) Identifier |var|
//@[4:47) Identifier |discriminatorKeyValueMissingCompletions3_if|
//@[48:49) Assignment |=|
//@[50:81) Identifier |discriminatorKeyValueMissing_if|
//@[81:82) LeftSquare |[|
//@[82:83) RightSquare |]|
//@[83:87) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access (loops)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:41) Identifier |discriminatorKeyValueMissing_for|
//@[42:92) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[93:94) Assignment |=|
//@[95:96) LeftSquare |[|
//@[96:99) Identifier |for|
//@[100:105) Identifier |thing|
//@[106:108) Identifier |in|
//@[109:110) LeftSquare |[|
//@[110:111) RightSquare |]|
//@[111:112) Colon |:|
//@[113:114) LeftBrace |{|
//@[114:116) NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
//@[70:72) NewLine |\r\n|
  kind:   
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[10:12) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// cannot . access properties of a resource loop
//@[48:50) NewLine |\r\n|
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind
//@[0:3) Identifier |var|
//@[4:35) Identifier |resourceListIsNotSingleResource|
//@[36:37) Assignment |=|
//@[38:70) Identifier |discriminatorKeyValueMissing_for|
//@[70:71) Dot |.|
//@[71:75) Identifier |kind|
//@[75:79) NewLine |\r\n\r\n|

// #completionTest(87) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
//@[0:3) Identifier |var|
//@[4:47) Identifier |discriminatorKeyValueMissingCompletions_for|
//@[48:49) Assignment |=|
//@[50:82) Identifier |discriminatorKeyValueMissing_for|
//@[82:83) LeftSquare |[|
//@[83:84) Integer |0|
//@[84:85) RightSquare |]|
//@[85:86) Dot |.|
//@[86:87) Identifier |p|
//@[87:89) NewLine |\r\n|
// #completionTest(87) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].
//@[0:3) Identifier |var|
//@[4:48) Identifier |discriminatorKeyValueMissingCompletions2_for|
//@[49:50) Assignment |=|
//@[51:83) Identifier |discriminatorKeyValueMissing_for|
//@[83:84) LeftSquare |[|
//@[84:85) Integer |0|
//@[85:86) RightSquare |]|
//@[86:87) Dot |.|
//@[87:91) NewLine |\r\n\r\n|

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
//@[74:76) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]
//@[0:3) Identifier |var|
//@[4:48) Identifier |discriminatorKeyValueMissingCompletions3_for|
//@[49:50) Assignment |=|
//@[51:83) Identifier |discriminatorKeyValueMissing_for|
//@[83:84) LeftSquare |[|
//@[84:85) Integer |0|
//@[85:86) RightSquare |]|
//@[86:87) LeftSquare |[|
//@[87:88) RightSquare |]|
//@[88:92) NewLine |\r\n\r\n|

/*
Discriminator value set 1
*/
//@[2:4) NewLine |\r\n|
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

/*
Discriminator value set 1 (conditional)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |discriminatorKeySetOne_if|
//@[35:85) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[86:87) Assignment |=|
//@[88:90) Identifier |if|
//@[90:91) LeftParen |(|
//@[91:92) Integer |2|
//@[92:94) Equals |==|
//@[94:95) Integer |3|
//@[95:96) RightParen |)|
//@[97:98) LeftBrace |{|
//@[98:100) NewLine |\r\n|
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
// #completionTest(81) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
//@[0:3) Identifier |var|
//@[4:40) Identifier |discriminatorKeySetOneCompletions_if|
//@[41:42) Assignment |=|
//@[43:68) Identifier |discriminatorKeySetOne_if|
//@[68:69) Dot |.|
//@[69:79) Identifier |properties|
//@[79:80) Dot |.|
//@[80:81) Identifier |a|
//@[81:83) NewLine |\r\n|
// #completionTest(81) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.
//@[0:3) Identifier |var|
//@[4:41) Identifier |discriminatorKeySetOneCompletions2_if|
//@[42:43) Assignment |=|
//@[44:69) Identifier |discriminatorKeySetOne_if|
//@[69:70) Dot |.|
//@[70:80) Identifier |properties|
//@[80:81) Dot |.|
//@[81:85) NewLine |\r\n\r\n|

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
//@[64:66) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]
//@[0:3) Identifier |var|
//@[4:41) Identifier |discriminatorKeySetOneCompletions3_if|
//@[42:43) Assignment |=|
//@[44:69) Identifier |discriminatorKeySetOne_if|
//@[69:70) Dot |.|
//@[70:80) Identifier |properties|
//@[80:81) LeftSquare |[|
//@[81:82) RightSquare |]|
//@[82:86) NewLine |\r\n\r\n|

/*
Discriminator value set 1 (loop)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |discriminatorKeySetOne_for|
//@[36:86) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftSquare |[|
//@[91:94) Identifier |for|
//@[95:100) Identifier |thing|
//@[101:103) Identifier |in|
//@[104:105) LeftSquare |[|
//@[105:106) RightSquare |]|
//@[106:107) Colon |:|
//@[108:109) LeftBrace |{|
//@[109:111) NewLine |\r\n|
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
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(86) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
//@[0:3) Identifier |var|
//@[4:41) Identifier |discriminatorKeySetOneCompletions_for|
//@[42:43) Assignment |=|
//@[44:70) Identifier |discriminatorKeySetOne_for|
//@[70:71) LeftSquare |[|
//@[71:72) Integer |0|
//@[72:73) RightSquare |]|
//@[73:74) Dot |.|
//@[74:84) Identifier |properties|
//@[84:85) Dot |.|
//@[85:86) Identifier |a|
//@[86:88) NewLine |\r\n|
// #completionTest(94) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.
//@[0:3) Identifier |var|
//@[4:42) Identifier |discriminatorKeySetOneCompletions2_for|
//@[43:44) Assignment |=|
//@[45:71) Identifier |discriminatorKeySetOne_for|
//@[71:72) LeftSquare |[|
//@[72:75) Identifier |any|
//@[75:76) LeftParen |(|
//@[76:80) TrueKeyword |true|
//@[80:81) RightParen |)|
//@[81:82) RightSquare |]|
//@[82:83) Dot |.|
//@[83:93) Identifier |properties|
//@[93:94) Dot |.|
//@[94:98) NewLine |\r\n\r\n|

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
//@[65:67) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]
//@[0:3) Identifier |var|
//@[4:42) Identifier |discriminatorKeySetOneCompletions3_for|
//@[43:44) Assignment |=|
//@[45:71) Identifier |discriminatorKeySetOne_for|
//@[71:72) LeftSquare |[|
//@[72:73) Integer |1|
//@[73:74) RightSquare |]|
//@[74:75) Dot |.|
//@[75:85) Identifier |properties|
//@[85:86) LeftSquare |[|
//@[86:87) RightSquare |]|
//@[87:91) NewLine |\r\n\r\n|

/*
Discriminator value set 2
*/
//@[2:4) NewLine |\r\n|
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

/*
Discriminator value set 2 (conditional)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |discriminatorKeySetTwo_if|
//@[35:85) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
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
// #completionTest(81) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
//@[0:3) Identifier |var|
//@[4:40) Identifier |discriminatorKeySetTwoCompletions_if|
//@[41:42) Assignment |=|
//@[43:68) Identifier |discriminatorKeySetTwo_if|
//@[68:69) Dot |.|
//@[69:79) Identifier |properties|
//@[79:80) Dot |.|
//@[80:81) Identifier |a|
//@[81:83) NewLine |\r\n|
// #completionTest(81) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.
//@[0:3) Identifier |var|
//@[4:41) Identifier |discriminatorKeySetTwoCompletions2_if|
//@[42:43) Assignment |=|
//@[44:69) Identifier |discriminatorKeySetTwo_if|
//@[69:70) Dot |.|
//@[70:80) Identifier |properties|
//@[80:81) Dot |.|
//@[81:85) NewLine |\r\n\r\n|

// #completionTest(96) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
//@[0:3) Identifier |var|
//@[4:52) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_if|
//@[53:54) Assignment |=|
//@[55:80) Identifier |discriminatorKeySetTwo_if|
//@[80:81) LeftSquare |[|
//@[81:93) StringComplete |'properties'|
//@[93:94) RightSquare |]|
//@[94:95) Dot |.|
//@[95:96) Identifier |a|
//@[96:98) NewLine |\r\n|
// #completionTest(96) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].
//@[0:3) Identifier |var|
//@[4:53) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_if|
//@[54:55) Assignment |=|
//@[56:81) Identifier |discriminatorKeySetTwo_if|
//@[81:82) LeftSquare |[|
//@[82:94) StringComplete |'properties'|
//@[94:95) RightSquare |]|
//@[95:96) Dot |.|
//@[96:100) NewLine |\r\n\r\n|

/*
Discriminator value set 2 (loops)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |discriminatorKeySetTwo_for|
//@[36:86) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftSquare |[|
//@[90:93) Identifier |for|
//@[94:99) Identifier |thing|
//@[100:102) Identifier |in|
//@[103:104) LeftSquare |[|
//@[104:105) RightSquare |]|
//@[105:106) Colon |:|
//@[107:108) LeftBrace |{|
//@[108:110) NewLine |\r\n|
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
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(86) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
//@[0:3) Identifier |var|
//@[4:41) Identifier |discriminatorKeySetTwoCompletions_for|
//@[42:43) Assignment |=|
//@[44:70) Identifier |discriminatorKeySetTwo_for|
//@[70:71) LeftSquare |[|
//@[71:72) Integer |0|
//@[72:73) RightSquare |]|
//@[73:74) Dot |.|
//@[74:84) Identifier |properties|
//@[84:85) Dot |.|
//@[85:86) Identifier |a|
//@[86:88) NewLine |\r\n|
// #completionTest(86) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.
//@[0:3) Identifier |var|
//@[4:42) Identifier |discriminatorKeySetTwoCompletions2_for|
//@[43:44) Assignment |=|
//@[45:71) Identifier |discriminatorKeySetTwo_for|
//@[71:72) LeftSquare |[|
//@[72:73) Integer |0|
//@[73:74) RightSquare |]|
//@[74:75) Dot |.|
//@[75:85) Identifier |properties|
//@[85:86) Dot |.|
//@[86:90) NewLine |\r\n\r\n|

// #completionTest(101) -> powershellPropertyAccess
//@[51:53) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
//@[0:3) Identifier |var|
//@[4:53) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_for|
//@[54:55) Assignment |=|
//@[56:82) Identifier |discriminatorKeySetTwo_for|
//@[82:83) LeftSquare |[|
//@[83:84) Integer |0|
//@[84:85) RightSquare |]|
//@[85:86) LeftSquare |[|
//@[86:98) StringComplete |'properties'|
//@[98:99) RightSquare |]|
//@[99:100) Dot |.|
//@[100:101) Identifier |a|
//@[101:103) NewLine |\r\n|
// #completionTest(101) -> powershellPropertyAccess
//@[51:53) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].
//@[0:3) Identifier |var|
//@[4:54) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_for|
//@[55:56) Assignment |=|
//@[57:83) Identifier |discriminatorKeySetTwo_for|
//@[83:84) LeftSquare |[|
//@[84:85) Integer |0|
//@[85:86) RightSquare |]|
//@[86:87) LeftSquare |[|
//@[87:99) StringComplete |'properties'|
//@[99:100) RightSquare |]|
//@[100:101) Dot |.|
//@[101:109) NewLine |\r\n\r\n\r\n\r\n|



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

/* 
Nested discriminator missing key
*/
//@[2:4) NewLine |\r\n|
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

/* 
Nested discriminator missing key (conditional)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
//@[0:8) Identifier |resource|
//@[9:41) Identifier |nestedDiscriminatorMissingKey_if|
//@[42:100) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[101:102) Assignment |=|
//@[103:105) Identifier |if|
//@[105:106) LeftParen |(|
//@[106:110) Identifier |bool|
//@[110:111) LeftParen |(|
//@[111:112) Integer |1|
//@[112:113) RightParen |)|
//@[113:114) RightParen |)|
//@[115:116) LeftBrace |{|
//@[116:118) NewLine |\r\n|
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
// #completionTest(96) -> createMode
//@[36:38) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
//@[0:3) Identifier |var|
//@[4:47) Identifier |nestedDiscriminatorMissingKeyCompletions_if|
//@[48:49) Assignment |=|
//@[50:82) Identifier |nestedDiscriminatorMissingKey_if|
//@[82:83) Dot |.|
//@[83:93) Identifier |properties|
//@[93:94) Dot |.|
//@[94:96) Identifier |cr|
//@[96:98) NewLine |\r\n|
// #completionTest(98) -> createMode
//@[36:38) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].
//@[0:3) Identifier |var|
//@[4:48) Identifier |nestedDiscriminatorMissingKeyCompletions2_if|
//@[49:50) Assignment |=|
//@[51:83) Identifier |nestedDiscriminatorMissingKey_if|
//@[83:84) LeftSquare |[|
//@[84:96) StringComplete |'properties'|
//@[96:97) RightSquare |]|
//@[97:98) Dot |.|
//@[98:102) NewLine |\r\n\r\n|

// #completionTest(100) -> createModeIndexPlusSymbols_if
//@[56:58) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']
//@[0:3) Identifier |var|
//@[4:52) Identifier |nestedDiscriminatorMissingKeyIndexCompletions_if|
//@[53:54) Assignment |=|
//@[55:87) Identifier |nestedDiscriminatorMissingKey_if|
//@[87:88) Dot |.|
//@[88:98) Identifier |properties|
//@[98:99) LeftSquare |[|
//@[99:101) StringComplete |''|
//@[101:102) RightSquare |]|
//@[102:106) NewLine |\r\n\r\n|

/* 
Nested discriminator missing key (loop)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:42) Identifier |nestedDiscriminatorMissingKey_for|
//@[43:101) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[102:103) Assignment |=|
//@[104:105) LeftSquare |[|
//@[105:108) Identifier |for|
//@[109:114) Identifier |thing|
//@[115:117) Identifier |in|
//@[118:119) LeftSquare |[|
//@[119:120) RightSquare |]|
//@[120:121) Colon |:|
//@[122:123) LeftBrace |{|
//@[123:125) NewLine |\r\n|
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
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(101) -> createMode
//@[37:39) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
//@[0:3) Identifier |var|
//@[4:48) Identifier |nestedDiscriminatorMissingKeyCompletions_for|
//@[49:50) Assignment |=|
//@[51:84) Identifier |nestedDiscriminatorMissingKey_for|
//@[84:85) LeftSquare |[|
//@[85:86) Integer |0|
//@[86:87) RightSquare |]|
//@[87:88) Dot |.|
//@[88:98) Identifier |properties|
//@[98:99) Dot |.|
//@[99:101) Identifier |cr|
//@[101:103) NewLine |\r\n|
// #completionTest(103) -> createMode
//@[37:39) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].
//@[0:3) Identifier |var|
//@[4:49) Identifier |nestedDiscriminatorMissingKeyCompletions2_for|
//@[50:51) Assignment |=|
//@[52:85) Identifier |nestedDiscriminatorMissingKey_for|
//@[85:86) LeftSquare |[|
//@[86:87) Integer |0|
//@[87:88) RightSquare |]|
//@[88:89) LeftSquare |[|
//@[89:101) StringComplete |'properties'|
//@[101:102) RightSquare |]|
//@[102:103) Dot |.|
//@[103:107) NewLine |\r\n\r\n|

// #completionTest(105) -> createModeIndexPlusSymbols_for
//@[57:59) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']
//@[0:3) Identifier |var|
//@[4:53) Identifier |nestedDiscriminatorMissingKeyIndexCompletions_for|
//@[54:55) Assignment |=|
//@[56:89) Identifier |nestedDiscriminatorMissingKey_for|
//@[89:90) LeftSquare |[|
//@[90:91) Integer |0|
//@[91:92) RightSquare |]|
//@[92:93) Dot |.|
//@[93:103) Identifier |properties|
//@[103:104) LeftSquare |[|
//@[104:106) StringComplete |''|
//@[106:107) RightSquare |]|
//@[107:113) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator
*/
//@[2:4) NewLine |\r\n|
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

/*
Nested discriminator (conditional)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
//@[0:8) Identifier |resource|
//@[9:31) Identifier |nestedDiscriminator_if|
//@[32:90) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[91:92) Assignment |=|
//@[93:95) Identifier |if|
//@[95:96) LeftParen |(|
//@[96:100) TrueKeyword |true|
//@[100:101) RightParen |)|
//@[102:103) LeftBrace |{|
//@[103:105) NewLine |\r\n|
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
// #completionTest(75) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
//@[0:3) Identifier |var|
//@[4:37) Identifier |nestedDiscriminatorCompletions_if|
//@[38:39) Assignment |=|
//@[40:62) Identifier |nestedDiscriminator_if|
//@[62:63) Dot |.|
//@[63:73) Identifier |properties|
//@[73:74) Dot |.|
//@[74:75) Identifier |a|
//@[75:77) NewLine |\r\n|
// #completionTest(79) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
//@[0:3) Identifier |var|
//@[4:38) Identifier |nestedDiscriminatorCompletions2_if|
//@[39:40) Assignment |=|
//@[41:63) Identifier |nestedDiscriminator_if|
//@[63:64) LeftSquare |[|
//@[64:76) StringComplete |'properties'|
//@[76:77) RightSquare |]|
//@[77:78) Dot |.|
//@[78:79) Identifier |a|
//@[79:81) NewLine |\r\n|
// #completionTest(75) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
//@[0:3) Identifier |var|
//@[4:38) Identifier |nestedDiscriminatorCompletions3_if|
//@[39:40) Assignment |=|
//@[41:63) Identifier |nestedDiscriminator_if|
//@[63:64) Dot |.|
//@[64:74) Identifier |properties|
//@[74:75) Dot |.|
//@[75:77) NewLine |\r\n|
// #completionTest(78) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].
//@[0:3) Identifier |var|
//@[4:38) Identifier |nestedDiscriminatorCompletions4_if|
//@[39:40) Assignment |=|
//@[41:63) Identifier |nestedDiscriminator_if|
//@[63:64) LeftSquare |[|
//@[64:76) StringComplete |'properties'|
//@[76:77) RightSquare |]|
//@[77:78) Dot |.|
//@[78:82) NewLine |\r\n\r\n|

// #completionTest(85) -> defaultCreateModeIndexes_if
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]
//@[0:3) Identifier |var|
//@[4:47) Identifier |nestedDiscriminatorArrayIndexCompletions_if|
//@[48:49) Assignment |=|
//@[50:72) Identifier |nestedDiscriminator_if|
//@[72:73) Dot |.|
//@[73:83) Identifier |properties|
//@[83:84) LeftSquare |[|
//@[84:85) Identifier |a|
//@[85:86) RightSquare |]|
//@[86:92) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator (loop)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:32) Identifier |nestedDiscriminator_for|
//@[33:91) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[92:93) Assignment |=|
//@[94:95) LeftSquare |[|
//@[95:98) Identifier |for|
//@[99:104) Identifier |thing|
//@[105:107) Identifier |in|
//@[108:109) LeftSquare |[|
//@[109:110) RightSquare |]|
//@[110:111) Colon |:|
//@[112:113) LeftBrace |{|
//@[113:115) NewLine |\r\n|
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
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(80) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
//@[0:3) Identifier |var|
//@[4:38) Identifier |nestedDiscriminatorCompletions_for|
//@[39:40) Assignment |=|
//@[41:64) Identifier |nestedDiscriminator_for|
//@[64:65) LeftSquare |[|
//@[65:66) Integer |0|
//@[66:67) RightSquare |]|
//@[67:68) Dot |.|
//@[68:78) Identifier |properties|
//@[78:79) Dot |.|
//@[79:80) Identifier |a|
//@[80:82) NewLine |\r\n|
// #completionTest(84) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
//@[0:3) Identifier |var|
//@[4:39) Identifier |nestedDiscriminatorCompletions2_for|
//@[40:41) Assignment |=|
//@[42:65) Identifier |nestedDiscriminator_for|
//@[65:66) LeftSquare |[|
//@[66:67) Integer |0|
//@[67:68) RightSquare |]|
//@[68:69) LeftSquare |[|
//@[69:81) StringComplete |'properties'|
//@[81:82) RightSquare |]|
//@[82:83) Dot |.|
//@[83:84) Identifier |a|
//@[84:86) NewLine |\r\n|
// #completionTest(80) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
//@[0:3) Identifier |var|
//@[4:39) Identifier |nestedDiscriminatorCompletions3_for|
//@[40:41) Assignment |=|
//@[42:65) Identifier |nestedDiscriminator_for|
//@[65:66) LeftSquare |[|
//@[66:67) Integer |0|
//@[67:68) RightSquare |]|
//@[68:69) Dot |.|
//@[69:79) Identifier |properties|
//@[79:80) Dot |.|
//@[80:82) NewLine |\r\n|
// #completionTest(83) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].
//@[0:3) Identifier |var|
//@[4:39) Identifier |nestedDiscriminatorCompletions4_for|
//@[40:41) Assignment |=|
//@[42:65) Identifier |nestedDiscriminator_for|
//@[65:66) LeftSquare |[|
//@[66:67) Integer |0|
//@[67:68) RightSquare |]|
//@[68:69) LeftSquare |[|
//@[69:81) StringComplete |'properties'|
//@[81:82) RightSquare |]|
//@[82:83) Dot |.|
//@[83:87) NewLine |\r\n\r\n|

// #completionTest(90) -> defaultCreateModeIndexes_for
//@[54:56) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]
//@[0:3) Identifier |var|
//@[4:48) Identifier |nestedDiscriminatorArrayIndexCompletions_for|
//@[49:50) Assignment |=|
//@[51:74) Identifier |nestedDiscriminator_for|
//@[74:75) LeftSquare |[|
//@[75:76) Integer |0|
//@[76:77) RightSquare |]|
//@[77:78) Dot |.|
//@[78:88) Identifier |properties|
//@[88:89) LeftSquare |[|
//@[89:90) Identifier |a|
//@[90:91) RightSquare |]|
//@[91:95) NewLine |\r\n\r\n|

// sample resource to validate completions on the next declarations
//@[67:69) NewLine |\r\n|
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
//@[0:8) Identifier |resource|
//@[9:42) Identifier |nestedPropertyAccessOnConditional|
//@[43:89) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[90:91) Assignment |=|
//@[92:94) Identifier |if|
//@[94:95) LeftParen |(|
//@[95:99) TrueKeyword |true|
//@[99:100) RightParen |)|
//@[101:102) LeftBrace |{|
//@[102:104) NewLine |\r\n|
  location: 'test'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:18) StringComplete |'test'|
//@[18:20) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    additionalCapabilities: {
//@[4:26) Identifier |additionalCapabilities|
//@[26:27) Colon |:|
//@[28:29) LeftBrace |{|
//@[29:31) NewLine |\r\n|
      
//@[6:8) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// this validates that we can get nested property access completions on a conditional resource
//@[94:96) NewLine |\r\n|
//#completionTest(56) -> vmProperties
//@[37:39) NewLine |\r\n|
var sigh = nestedPropertyAccessOnConditional.properties.
//@[0:3) Identifier |var|
//@[4:8) Identifier |sigh|
//@[9:10) Assignment |=|
//@[11:44) Identifier |nestedPropertyAccessOnConditional|
//@[44:45) Dot |.|
//@[45:55) Identifier |properties|
//@[55:56) Dot |.|
//@[56:60) NewLine |\r\n\r\n|

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
//@[1:5) NewLine |\r\n\r\n|

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:30) Identifier |invalidDuplicateName1|
//@[31:64) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[65:66) Assignment |=|
//@[67:68) LeftBrace |{|
//@[68:70) NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:30) StringComplete |'invalidDuplicateName'|
//@[30:32) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:30) Identifier |invalidDuplicateName2|
//@[31:64) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[65:66) Assignment |=|
//@[67:68) LeftBrace |{|
//@[68:70) NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:30) StringComplete |'invalidDuplicateName'|
//@[30:32) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
//@[0:8) Identifier |resource|
//@[9:30) Identifier |invalidDuplicateName3|
//@[31:64) StringComplete |'Mock.Rp/mockResource@2019-01-01'|
//@[65:66) Assignment |=|
//@[67:68) LeftBrace |{|
//@[68:70) NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:30) StringComplete |'invalidDuplicateName'|
//@[30:32) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:62) Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[63:96) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[97:98) Assignment |=|
//@[99:100) LeftBrace |{|
//@[100:102) NewLine |\r\n|
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:63) StringComplete |'validResourceForInvalidExtensionResourceDuplicateName'|
//@[63:65) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:47) Identifier |invalidExtensionResourceDuplicateName1|
//@[48:84) StringComplete |'Mock.Rp/mockExtResource@2020-01-01'|
//@[85:86) Assignment |=|
//@[87:88) LeftBrace |{|
//@[88:90) NewLine |\r\n|
  name: 'invalidExtensionResourceDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:47) StringComplete |'invalidExtensionResourceDuplicateName'|
//@[47:49) NewLine |\r\n|
  scope: validResourceForInvalidExtensionResourceDuplicateName
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:62) Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[62:64) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
//@[0:8) Identifier |resource|
//@[9:47) Identifier |invalidExtensionResourceDuplicateName2|
//@[48:84) StringComplete |'Mock.Rp/mockExtResource@2019-01-01'|
//@[85:86) Assignment |=|
//@[87:88) LeftBrace |{|
//@[88:90) NewLine |\r\n|
  name: 'invalidExtensionResourceDuplicateName'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:47) StringComplete |'invalidExtensionResourceDuplicateName'|
//@[47:49) NewLine |\r\n|
  scope: validResourceForInvalidExtensionResourceDuplicateName
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:62) Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[62:64) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@concat('foo', 'bar')
//@[0:1) At |@|
//@[1:7) Identifier |concat|
//@[7:8) LeftParen |(|
//@[8:13) StringComplete |'foo'|
//@[13:14) Comma |,|
//@[15:20) StringComplete |'bar'|
//@[20:21) RightParen |)|
//@[21:23) NewLine |\r\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:11) NewLine |\r\n|
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |invalidDecorator|
//@[26:63) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[63:64) Assignment |=|
//@[65:66) LeftBrace |{|
//@[66:68) NewLine |\r\n|
  name: 'invalidDecorator'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:26) StringComplete |'invalidDecorator'|
//@[26:28) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:18) Identifier |cyclicRes|
//@[19:60) StringComplete |'Mock.Rp/mockExistingResource@2020-01-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  name: 'cyclicRes'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'cyclicRes'|
//@[19:21) NewLine |\r\n|
  scope: cyclicRes
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:18) Identifier |cyclicRes|
//@[18:20) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
//@[0:8) Identifier |resource|
//@[9:26) Identifier |cyclicExistingRes|
//@[27:68) StringComplete |'Mock.Rp/mockExistingResource@2020-01-01'|
//@[69:77) Identifier |existing|
//@[78:79) Assignment |=|
//@[80:81) LeftBrace |{|
//@[81:83) NewLine |\r\n|
  name: 'cyclicExistingRes'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:27) StringComplete |'cyclicExistingRes'|
//@[27:29) NewLine |\r\n|
  scope: cyclicExistingRes
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:26) Identifier |cyclicExistingRes|
//@[26:28) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// loop parsing cases
//@[21:23) NewLine |\r\n|
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []
//@[0:8) Identifier |resource|
//@[9:27) Identifier |expectedForKeyword|
//@[28:74) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[75:76) Assignment |=|
//@[77:78) LeftSquare |[|
//@[78:79) RightSquare |]|
//@[79:83) NewLine |\r\n\r\n|

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]
//@[0:8) Identifier |resource|
//@[9:28) Identifier |expectedForKeyword2|
//@[29:75) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftSquare |[|
//@[79:80) Identifier |f|
//@[80:81) RightSquare |]|
//@[81:85) NewLine |\r\n\r\n|

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]
//@[0:8) Identifier |resource|
//@[9:24) Identifier |expectedLoopVar|
//@[25:71) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73) Assignment |=|
//@[74:75) LeftSquare |[|
//@[75:78) Identifier |for|
//@[78:79) RightSquare |]|
//@[79:83) NewLine |\r\n\r\n|

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]
//@[0:8) Identifier |resource|
//@[9:26) Identifier |expectedInKeyword|
//@[27:73) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[74:75) Assignment |=|
//@[76:77) LeftSquare |[|
//@[77:80) Identifier |for|
//@[81:82) Identifier |x|
//@[82:83) RightSquare |]|
//@[83:87) NewLine |\r\n\r\n|

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]
//@[0:8) Identifier |resource|
//@[9:27) Identifier |expectedInKeyword2|
//@[28:74) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[75:76) Assignment |=|
//@[77:78) LeftSquare |[|
//@[78:81) Identifier |for|
//@[82:83) Identifier |x|
//@[84:85) Identifier |b|
//@[85:86) RightSquare |]|
//@[86:90) NewLine |\r\n\r\n|

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]
//@[0:8) Identifier |resource|
//@[9:32) Identifier |expectedArrayExpression|
//@[33:79) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[80:81) Assignment |=|
//@[82:83) LeftSquare |[|
//@[83:86) Identifier |for|
//@[87:88) Identifier |x|
//@[89:91) Identifier |in|
//@[91:92) RightSquare |]|
//@[92:96) NewLine |\r\n\r\n|

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]
//@[0:8) Identifier |resource|
//@[9:22) Identifier |expectedColon|
//@[23:69) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[70:71) Assignment |=|
//@[72:73) LeftSquare |[|
//@[73:76) Identifier |for|
//@[77:78) Identifier |x|
//@[79:81) Identifier |in|
//@[82:83) Identifier |y|
//@[83:84) RightSquare |]|
//@[84:88) NewLine |\r\n\r\n|

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]
//@[0:8) Identifier |resource|
//@[9:25) Identifier |expectedLoopBody|
//@[26:72) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74) Assignment |=|
//@[75:76) LeftSquare |[|
//@[76:79) Identifier |for|
//@[80:81) Identifier |x|
//@[82:84) Identifier |in|
//@[85:86) Identifier |y|
//@[86:87) Colon |:|
//@[87:88) RightSquare |]|
//@[88:92) NewLine |\r\n\r\n|

// loop semantic analysis cases
//@[31:33) NewLine |\r\n|
var emptyArray = []
//@[0:3) Identifier |var|
//@[4:14) Identifier |emptyArray|
//@[15:16) Assignment |=|
//@[17:18) LeftSquare |[|
//@[18:19) RightSquare |]|
//@[19:21) NewLine |\r\n|
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
//@[0:8) Identifier |resource|
//@[9:26) Identifier |wrongLoopBodyType|
//@[27:73) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[74:75) Assignment |=|
//@[76:77) LeftSquare |[|
//@[77:80) Identifier |for|
//@[81:82) Identifier |x|
//@[83:85) Identifier |in|
//@[86:96) Identifier |emptyArray|
//@[96:97) Colon |:|
//@[97:98) Integer |4|
//@[98:99) RightSquare |]|
//@[99:103) NewLine |\r\n\r\n|

// errors in the array expression
//@[33:35) NewLine |\r\n|
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
//@[0:8) Identifier |resource|
//@[9:30) Identifier |arrayExpressionErrors|
//@[31:77) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[78:79) Assignment |=|
//@[80:81) LeftSquare |[|
//@[81:84) Identifier |for|
//@[85:92) Identifier |account|
//@[93:95) Identifier |in|
//@[96:101) Identifier |union|
//@[101:102) LeftParen |(|
//@[102:103) LeftSquare |[|
//@[103:104) RightSquare |]|
//@[104:105) Comma |,|
//@[106:107) Integer |2|
//@[107:108) RightParen |)|
//@[108:109) Colon |:|
//@[110:111) LeftBrace |{|
//@[111:113) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// wrong array type
//@[19:21) NewLine |\r\n|
var notAnArray = true
//@[0:3) Identifier |var|
//@[4:14) Identifier |notAnArray|
//@[15:16) Assignment |=|
//@[17:21) TrueKeyword |true|
//@[21:23) NewLine |\r\n|
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
//@[0:8) Identifier |resource|
//@[9:23) Identifier |wrongArrayType|
//@[24:70) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[71:72) Assignment |=|
//@[73:74) LeftSquare |[|
//@[74:77) Identifier |for|
//@[78:85) Identifier |account|
//@[86:88) Identifier |in|
//@[89:99) Identifier |notAnArray|
//@[99:100) Colon |:|
//@[101:102) LeftBrace |{|
//@[102:104) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// missing required properties
//@[30:32) NewLine |\r\n|
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |missingRequiredProperties|
//@[35:81) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[82:83) Assignment |=|
//@[84:85) LeftSquare |[|
//@[85:88) Identifier |for|
//@[89:96) Identifier |account|
//@[97:99) Identifier |in|
//@[100:101) LeftSquare |[|
//@[101:102) RightSquare |]|
//@[102:103) Colon |:|
//@[104:105) LeftBrace |{|
//@[105:107) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// fewer missing required properties and a wrong property
//@[57:59) NewLine |\r\n|
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[0:8) Identifier |resource|
//@[9:39) Identifier |missingFewerRequiredProperties|
//@[40:86) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftSquare |[|
//@[90:93) Identifier |for|
//@[94:101) Identifier |account|
//@[102:104) Identifier |in|
//@[105:106) LeftSquare |[|
//@[106:107) RightSquare |]|
//@[107:108) Colon |:|
//@[109:110) LeftBrace |{|
//@[110:112) NewLine |\r\n|
  name: account
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) Identifier |account|
//@[15:17) NewLine |\r\n|
  location: 'eastus42'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:22) StringComplete |'eastus42'|
//@[22:24) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    wrong: 'test'
//@[4:9) Identifier |wrong|
//@[9:10) Colon |:|
//@[11:17) StringComplete |'test'|
//@[17:19) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// wrong property inside the nested property loop
//@[49:51) NewLine |\r\n|
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |wrongPropertyInNestedLoop|
//@[35:81) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[82:83) Assignment |=|
//@[84:85) LeftSquare |[|
//@[85:88) Identifier |for|
//@[89:90) Identifier |i|
//@[91:93) Identifier |in|
//@[94:99) Identifier |range|
//@[99:100) LeftParen |(|
//@[100:101) Integer |0|
//@[101:102) Comma |,|
//@[103:104) Integer |3|
//@[104:105) RightParen |)|
//@[105:106) Colon |:|
//@[107:108) LeftBrace |{|
//@[108:110) NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'vnet-${|
//@[16:17) Identifier |i|
//@[17:19) StringRightPiece |}'|
//@[19:21) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    subnets: [for j in range(0, 4): {
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:17) Identifier |for|
//@[18:19) Identifier |j|
//@[20:22) Identifier |in|
//@[23:28) Identifier |range|
//@[28:29) LeftParen |(|
//@[29:30) Integer |0|
//@[30:31) Comma |,|
//@[32:33) Integer |4|
//@[33:34) RightParen |)|
//@[34:35) Colon |:|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\r\n|
      doesNotExist: 'test'
//@[6:18) Identifier |doesNotExist|
//@[18:19) Colon |:|
//@[20:26) StringComplete |'test'|
//@[26:28) NewLine |\r\n|
      name: 'subnet-${i}-${j}'
//@[6:10) Identifier |name|
//@[10:11) Colon |:|
//@[12:22) StringLeftPiece |'subnet-${|
//@[22:23) Identifier |i|
//@[23:27) StringMiddlePiece |}-${|
//@[27:28) Identifier |j|
//@[28:30) StringRightPiece |}'|
//@[30:32) NewLine |\r\n|
    }]
//@[4:5) RightBrace |}|
//@[5:6) RightSquare |]|
//@[6:8) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// nonexistent arrays and loop variables
//@[40:42) NewLine |\r\n|
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
//@[0:8) Identifier |resource|
//@[9:26) Identifier |nonexistentArrays|
//@[27:73) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[74:75) Assignment |=|
//@[76:77) LeftSquare |[|
//@[77:80) Identifier |for|
//@[81:82) Identifier |i|
//@[83:85) Identifier |in|
//@[86:95) Identifier |notAThing|
//@[95:96) Colon |:|
//@[97:98) LeftBrace |{|
//@[98:100) NewLine |\r\n|
  name: 'vnet-${justPlainWrong}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'vnet-${|
//@[16:30) Identifier |justPlainWrong|
//@[30:32) StringRightPiece |}'|
//@[32:34) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    subnets: [for j in alsoNotAThing: {
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:17) Identifier |for|
//@[18:19) Identifier |j|
//@[20:22) Identifier |in|
//@[23:36) Identifier |alsoNotAThing|
//@[36:37) Colon |:|
//@[38:39) LeftBrace |{|
//@[39:41) NewLine |\r\n|
      doesNotExist: 'test'
//@[6:18) Identifier |doesNotExist|
//@[18:19) Colon |:|
//@[20:26) StringComplete |'test'|
//@[26:28) NewLine |\r\n|
      name: 'subnet-${fake}-${totallyFake}'
//@[6:10) Identifier |name|
//@[10:11) Colon |:|
//@[12:22) StringLeftPiece |'subnet-${|
//@[22:26) Identifier |fake|
//@[26:30) StringMiddlePiece |}-${|
//@[30:41) Identifier |totallyFake|
//@[41:43) StringRightPiece |}'|
//@[43:45) NewLine |\r\n|
    }]
//@[4:5) RightBrace |}|
//@[5:6) RightSquare |]|
//@[6:8) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

/*
  valid loop cases - this should be moved to Resources_* test case after codegen works
*/ 
//@[3:5) NewLine |\r\n|
var storageAccounts = [
//@[0:3) Identifier |var|
//@[4:19) Identifier |storageAccounts|
//@[20:21) Assignment |=|
//@[22:23) LeftSquare |[|
//@[23:25) NewLine |\r\n|
  {
//@[2:3) LeftBrace |{|
//@[3:5) NewLine |\r\n|
    name: 'one'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'one'|
//@[15:17) NewLine |\r\n|
    location: 'eastus2'
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:23) StringComplete |'eastus2'|
//@[23:25) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  {
//@[2:3) LeftBrace |{|
//@[3:5) NewLine |\r\n|
    name: 'two'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'two'|
//@[15:17) NewLine |\r\n|
    location: 'westus'
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:22) StringComplete |'westus'|
//@[22:24) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|
// duplicate identifiers within the loop are allowed
//@[52:54) NewLine |\r\n|
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:8) Identifier |resource|
//@[9:39) Identifier |duplicateIdentifiersWithinLoop|
//@[40:86) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftSquare |[|
//@[90:93) Identifier |for|
//@[94:95) Identifier |i|
//@[96:98) Identifier |in|
//@[99:104) Identifier |range|
//@[104:105) LeftParen |(|
//@[105:106) Integer |0|
//@[106:107) Comma |,|
//@[108:109) Integer |3|
//@[109:110) RightParen |)|
//@[110:111) Colon |:|
//@[112:113) LeftBrace |{|
//@[113:115) NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'vnet-${|
//@[16:17) Identifier |i|
//@[17:19) StringRightPiece |}'|
//@[19:21) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    subnets: [for i in range(0, 4): {
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:17) Identifier |for|
//@[18:19) Identifier |i|
//@[20:22) Identifier |in|
//@[23:28) Identifier |range|
//@[28:29) LeftParen |(|
//@[29:30) Integer |0|
//@[30:31) Comma |,|
//@[32:33) Integer |4|
//@[33:34) RightParen |)|
//@[34:35) Colon |:|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\r\n|
      name: 'subnet-${i}-${i}'
//@[6:10) Identifier |name|
//@[10:11) Colon |:|
//@[12:22) StringLeftPiece |'subnet-${|
//@[22:23) Identifier |i|
//@[23:27) StringMiddlePiece |}-${|
//@[27:28) Identifier |i|
//@[28:30) StringRightPiece |}'|
//@[30:32) NewLine |\r\n|
    }]
//@[4:5) RightBrace |}|
//@[5:6) RightSquare |]|
//@[6:8) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
//@[100:102) NewLine |\r\n|
var canHaveDuplicatesAcrossScopes = 'hello'
//@[0:3) Identifier |var|
//@[4:33) Identifier |canHaveDuplicatesAcrossScopes|
//@[34:35) Assignment |=|
//@[36:43) StringComplete |'hello'|
//@[43:45) NewLine |\r\n|
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[0:8) Identifier |resource|
//@[9:36) Identifier |duplicateInGlobalAndOneLoop|
//@[37:83) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[84:85) Assignment |=|
//@[86:87) LeftSquare |[|
//@[87:90) Identifier |for|
//@[91:120) Identifier |canHaveDuplicatesAcrossScopes|
//@[121:123) Identifier |in|
//@[124:129) Identifier |range|
//@[129:130) LeftParen |(|
//@[130:131) Integer |0|
//@[131:132) Comma |,|
//@[133:134) Integer |3|
//@[134:135) RightParen |)|
//@[135:136) Colon |:|
//@[137:138) LeftBrace |{|
//@[138:140) NewLine |\r\n|
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'vnet-${|
//@[16:45) Identifier |canHaveDuplicatesAcrossScopes|
//@[45:47) StringRightPiece |}'|
//@[47:49) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    subnets: [for i in range(0, 4): {
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:17) Identifier |for|
//@[18:19) Identifier |i|
//@[20:22) Identifier |in|
//@[23:28) Identifier |range|
//@[28:29) LeftParen |(|
//@[29:30) Integer |0|
//@[30:31) Comma |,|
//@[32:33) Integer |4|
//@[33:34) RightParen |)|
//@[34:35) Colon |:|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\r\n|
      name: 'subnet-${i}-${i}'
//@[6:10) Identifier |name|
//@[10:11) Colon |:|
//@[12:22) StringLeftPiece |'subnet-${|
//@[22:23) Identifier |i|
//@[23:27) StringMiddlePiece |}-${|
//@[27:28) Identifier |i|
//@[28:30) StringRightPiece |}'|
//@[30:32) NewLine |\r\n|
    }]
//@[4:5) RightBrace |}|
//@[5:6) RightSquare |]|
//@[6:8) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
//@[83:85) NewLine |\r\n|
var duplicatesEverywhere = 'hello'
//@[0:3) Identifier |var|
//@[4:24) Identifier |duplicatesEverywhere|
//@[25:26) Assignment |=|
//@[27:34) StringComplete |'hello'|
//@[34:36) NewLine |\r\n|
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[0:8) Identifier |resource|
//@[9:37) Identifier |duplicateInGlobalAndTwoLoops|
//@[38:84) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[85:86) Assignment |=|
//@[87:88) LeftSquare |[|
//@[88:91) Identifier |for|
//@[92:112) Identifier |duplicatesEverywhere|
//@[113:115) Identifier |in|
//@[116:121) Identifier |range|
//@[121:122) LeftParen |(|
//@[122:123) Integer |0|
//@[123:124) Comma |,|
//@[125:126) Integer |3|
//@[126:127) RightParen |)|
//@[127:128) Colon |:|
//@[129:130) LeftBrace |{|
//@[130:132) NewLine |\r\n|
  name: 'vnet-${duplicatesEverywhere}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'vnet-${|
//@[16:36) Identifier |duplicatesEverywhere|
//@[36:38) StringRightPiece |}'|
//@[38:40) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:17) Identifier |for|
//@[18:38) Identifier |duplicatesEverywhere|
//@[39:41) Identifier |in|
//@[42:47) Identifier |range|
//@[47:48) LeftParen |(|
//@[48:49) Integer |0|
//@[49:50) Comma |,|
//@[51:52) Integer |4|
//@[52:53) RightParen |)|
//@[53:54) Colon |:|
//@[55:56) LeftBrace |{|
//@[56:58) NewLine |\r\n|
      name: 'subnet-${duplicatesEverywhere}'
//@[6:10) Identifier |name|
//@[10:11) Colon |:|
//@[12:22) StringLeftPiece |'subnet-${|
//@[22:42) Identifier |duplicatesEverywhere|
//@[42:44) StringRightPiece |}'|
//@[44:46) NewLine |\r\n|
    }]
//@[4:5) RightBrace |}|
//@[5:6) RightSquare |]|
//@[6:8) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// just a storage account loop
//@[30:32) NewLine |\r\n|
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |storageResources|
//@[26:72) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74) Assignment |=|
//@[75:76) LeftSquare |[|
//@[76:79) Identifier |for|
//@[80:87) Identifier |account|
//@[88:90) Identifier |in|
//@[91:106) Identifier |storageAccounts|
//@[106:107) Colon |:|
//@[108:109) LeftBrace |{|
//@[109:111) NewLine |\r\n|
  name: account.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) Identifier |account|
//@[15:16) Dot |.|
//@[16:20) Identifier |name|
//@[20:22) NewLine |\r\n|
  location: account.location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:19) Identifier |account|
//@[19:20) Dot |.|
//@[20:28) Identifier |location|
//@[28:30) NewLine |\r\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:10) NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:26) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:21) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// using the same loop variable in a new language scope should be allowed
//@[73:75) NewLine |\r\n|
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:8) Identifier |resource|
//@[9:24) Identifier |premiumStorages|
//@[25:71) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73) Assignment |=|
//@[74:75) LeftSquare |[|
//@[75:78) Identifier |for|
//@[79:86) Identifier |account|
//@[87:89) Identifier |in|
//@[90:105) Identifier |storageAccounts|
//@[105:106) Colon |:|
//@[107:108) LeftBrace |{|
//@[108:110) NewLine |\r\n|
  name: account.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) Identifier |account|
//@[15:16) Dot |.|
//@[16:20) Identifier |name|
//@[20:22) NewLine |\r\n|
  location: account.location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:19) Identifier |account|
//@[19:20) Dot |.|
//@[20:28) Identifier |location|
//@[28:30) NewLine |\r\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:10) NewLine |\r\n|
    // #completionTest(9,10) -> storageSkuNamePlusSymbols
//@[57:59) NewLine |\r\n|
    name: 
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:12) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:21) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
// basic nested loop
//@[20:22) NewLine |\r\n|
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:8) Identifier |resource|
//@[9:13) Identifier |vnet|
//@[14:60) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftSquare |[|
//@[64:67) Identifier |for|
//@[68:69) Identifier |i|
//@[70:72) Identifier |in|
//@[73:78) Identifier |range|
//@[78:79) LeftParen |(|
//@[79:80) Integer |0|
//@[80:81) Comma |,|
//@[82:83) Integer |3|
//@[83:84) RightParen |)|
//@[84:85) Colon |:|
//@[86:87) LeftBrace |{|
//@[87:89) NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'vnet-${|
//@[16:17) Identifier |i|
//@[17:19) StringRightPiece |}'|
//@[19:21) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    subnets: [for j in range(0, 4): {
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:17) Identifier |for|
//@[18:19) Identifier |j|
//@[20:22) Identifier |in|
//@[23:28) Identifier |range|
//@[28:29) LeftParen |(|
//@[29:30) Integer |0|
//@[30:31) Comma |,|
//@[32:33) Integer |4|
//@[33:34) RightParen |)|
//@[34:35) Colon |:|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\r\n|
      // #completionTest(0,1,2,3,4,5,6) -> subnetIdAndProperties
//@[64:66) NewLine |\r\n|
      name: 'subnet-${i}-${j}'
//@[6:10) Identifier |name|
//@[10:11) Colon |:|
//@[12:22) StringLeftPiece |'subnet-${|
//@[22:23) Identifier |i|
//@[23:27) StringMiddlePiece |}-${|
//@[27:28) Identifier |j|
//@[28:30) StringRightPiece |}'|
//@[30:32) NewLine |\r\n|
    }]
//@[4:5) RightBrace |}|
//@[5:6) RightSquare |]|
//@[6:8) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:2) EndOfFile ||
