
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

// #completionTest(19,20) -> resourceObject
//@[43:45) NewLine |\r\n|
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

// there should be no completions without the colon
//@[51:53) NewLine |\r\n|
resource noCompletionsWithoutColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |noCompletionsWithoutColon|
//@[35:85) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
  // #completionTest(7,8) -> empty
//@[34:36) NewLine |\r\n|
  kind  
//@[2:6) Identifier |kind|
//@[8:10) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource noCompletionsBeforeColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:8) Identifier |resource|
//@[9:33) Identifier |noCompletionsBeforeColon|
//@[34:84) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[85:86) Assignment |=|
//@[87:88) LeftBrace |{|
//@[88:90) NewLine |\r\n|
  // #completionTest(7,8) -> empty
//@[34:36) NewLine |\r\n|
  kind  :
//@[2:6) Identifier |kind|
//@[8:9) Colon |:|
//@[9:11) NewLine |\r\n|
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
//@[1:7) NewLine |\r\n\r\n\r\n|


resource loopForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |loopForRuntimeCheck|
//@[29:68) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[69:70) Assignment |=|
//@[71:72) LeftSquare |[|
//@[72:75) Identifier |for|
//@[76:81) Identifier |thing|
//@[82:84) Identifier |in|
//@[85:86) LeftSquare |[|
//@[86:87) RightSquare |]|
//@[87:88) Colon |:|
//@[89:90) LeftBrace |{|
//@[90:92) NewLine |\r\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
  location: 'test'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:18) StringComplete |'test'|
//@[18:20) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
//@[0:3) Identifier |var|
//@[4:19) Identifier |runtimeCheckVar|
//@[20:21) Assignment |=|
//@[22:41) Identifier |loopForRuntimeCheck|
//@[41:42) LeftSquare |[|
//@[42:43) Integer |0|
//@[43:44) RightSquare |]|
//@[44:45) Dot |.|
//@[45:55) Identifier |properties|
//@[55:56) Dot |.|
//@[56:64) Identifier |zoneType|
//@[64:66) NewLine |\r\n|
var runtimeCheckVar2 = runtimeCheckVar
//@[0:3) Identifier |var|
//@[4:20) Identifier |runtimeCheckVar2|
//@[21:22) Assignment |=|
//@[23:38) Identifier |runtimeCheckVar|
//@[38:42) NewLine |\r\n\r\n|

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:8) Identifier |resource|
//@[9:38) Identifier |singleResourceForRuntimeCheck|
//@[39:78) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[79:80) Assignment |=|
//@[81:82) LeftBrace |{|
//@[82:84) NewLine |\r\n|
  name: runtimeCheckVar2
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeCheckVar2|
//@[24:26) NewLine |\r\n|
  location: 'test'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:18) StringComplete |'test'|
//@[18:20) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:29) Identifier |loopForRuntimeCheck2|
//@[30:69) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71) Assignment |=|
//@[72:73) LeftSquare |[|
//@[73:76) Identifier |for|
//@[77:82) Identifier |thing|
//@[83:85) Identifier |in|
//@[86:87) LeftSquare |[|
//@[87:88) RightSquare |]|
//@[88:89) Colon |:|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: runtimeCheckVar2
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:24) Identifier |runtimeCheckVar2|
//@[24:26) NewLine |\r\n|
  location: 'test'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:18) StringComplete |'test'|
//@[18:20) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[0:8) Identifier |resource|
//@[9:29) Identifier |loopForRuntimeCheck3|
//@[30:69) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71) Assignment |=|
//@[72:73) LeftSquare |[|
//@[73:76) Identifier |for|
//@[77:87) Identifier |otherThing|
//@[88:90) Identifier |in|
//@[91:92) LeftSquare |[|
//@[92:93) RightSquare |]|
//@[93:94) Colon |:|
//@[95:96) LeftBrace |{|
//@[96:98) NewLine |\r\n|
  name: loopForRuntimeCheck[0].properties.zoneType
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:27) Identifier |loopForRuntimeCheck|
//@[27:28) LeftSquare |[|
//@[28:29) Integer |0|
//@[29:30) RightSquare |]|
//@[30:31) Dot |.|
//@[31:41) Identifier |properties|
//@[41:42) Dot |.|
//@[42:50) Identifier |zoneType|
//@[50:52) NewLine |\r\n|
  location: 'test'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:18) StringComplete |'test'|
//@[18:20) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
//@[0:3) Identifier |var|
//@[4:24) Identifier |varForRuntimeCheck4a|
//@[25:26) Assignment |=|
//@[27:46) Identifier |loopForRuntimeCheck|
//@[46:47) LeftSquare |[|
//@[47:48) Integer |0|
//@[48:49) RightSquare |]|
//@[49:50) Dot |.|
//@[50:60) Identifier |properties|
//@[60:61) Dot |.|
//@[61:69) Identifier |zoneType|
//@[69:71) NewLine |\r\n|
var varForRuntimeCheck4b = varForRuntimeCheck4a
//@[0:3) Identifier |var|
//@[4:24) Identifier |varForRuntimeCheck4b|
//@[25:26) Assignment |=|
//@[27:47) Identifier |varForRuntimeCheck4a|
//@[47:49) NewLine |\r\n|
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[0:8) Identifier |resource|
//@[9:29) Identifier |loopForRuntimeCheck4|
//@[30:69) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71) Assignment |=|
//@[72:73) LeftSquare |[|
//@[73:76) Identifier |for|
//@[77:87) Identifier |otherThing|
//@[88:90) Identifier |in|
//@[91:92) LeftSquare |[|
//@[92:93) RightSquare |]|
//@[93:94) Colon |:|
//@[95:96) LeftBrace |{|
//@[96:98) NewLine |\r\n|
  name: varForRuntimeCheck4b
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:28) Identifier |varForRuntimeCheck4b|
//@[28:30) NewLine |\r\n|
  location: 'test'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:18) StringComplete |'test'|
//@[18:20) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |missingTopLevelProperties|
//@[35:89) StringComplete |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[90:91) Assignment |=|
//@[92:93) LeftBrace |{|
//@[93:95) NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelProperties
//@[51:53) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
  // #completionTest(2) -> topLevelPropertiesMinusNameNoColon
//@[61:63) NewLine |\r\n|
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

// #completionTest(24,25,26,49,65,69,70) -> virtualNetworksResourceTypes
//@[72:74) NewLine |\r\n|
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
       
//@[7:9) NewLine |\r\n|
        // #completionTest(0,1,2,3,4,5,6,7) -> empty
//@[52:54) NewLine |\r\n|
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
Discriminator key missing (filtered loop)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[0:8) Identifier |resource|
//@[9:39) Identifier |discriminatorKeyMissing_for_if|
//@[40:90) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[91:92) Assignment |=|
//@[93:94) LeftSquare |[|
//@[94:97) Identifier |for|
//@[98:103) Identifier |thing|
//@[104:106) Identifier |in|
//@[107:108) LeftSquare |[|
//@[108:109) RightSquare |]|
//@[109:110) Colon |:|
//@[111:113) Identifier |if|
//@[113:114) LeftParen |(|
//@[114:118) TrueKeyword |true|
//@[118:119) RightParen |)|
//@[120:121) LeftBrace |{|
//@[121:123) NewLine |\r\n|
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
//@[2:4) NewLine |\r\n|
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
Discriminator key value missing with property access (filtered loops)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[0:8) Identifier |resource|
//@[9:44) Identifier |discriminatorKeyValueMissing_for_if|
//@[45:95) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[96:97) Assignment |=|
//@[98:99) LeftSquare |[|
//@[99:102) Identifier |for|
//@[103:108) Identifier |thing|
//@[109:111) Identifier |in|
//@[112:113) LeftSquare |[|
//@[113:114) RightSquare |]|
//@[114:115) Colon |:|
//@[116:118) Identifier |if|
//@[118:119) LeftParen |(|
//@[119:123) TrueKeyword |true|
//@[123:124) RightParen |)|
//@[125:126) LeftBrace |{|
//@[126:128) NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for_if
//@[73:75) NewLine |\r\n|
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
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind
//@[0:3) Identifier |var|
//@[4:38) Identifier |resourceListIsNotSingleResource_if|
//@[39:40) Assignment |=|
//@[41:76) Identifier |discriminatorKeyValueMissing_for_if|
//@[76:77) Dot |.|
//@[77:81) Identifier |kind|
//@[81:85) NewLine |\r\n\r\n|

// #completionTest(93) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
//@[0:3) Identifier |var|
//@[4:50) Identifier |discriminatorKeyValueMissingCompletions_for_if|
//@[51:52) Assignment |=|
//@[53:88) Identifier |discriminatorKeyValueMissing_for_if|
//@[88:89) LeftSquare |[|
//@[89:90) Integer |0|
//@[90:91) RightSquare |]|
//@[91:92) Dot |.|
//@[92:93) Identifier |p|
//@[93:95) NewLine |\r\n|
// #completionTest(93) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].
//@[0:3) Identifier |var|
//@[4:51) Identifier |discriminatorKeyValueMissingCompletions2_for_if|
//@[52:53) Assignment |=|
//@[54:89) Identifier |discriminatorKeyValueMissing_for_if|
//@[89:90) LeftSquare |[|
//@[90:91) Integer |0|
//@[91:92) RightSquare |]|
//@[92:93) Dot |.|
//@[93:97) NewLine |\r\n\r\n|

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
//@[77:79) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]
//@[0:3) Identifier |var|
//@[4:51) Identifier |discriminatorKeyValueMissingCompletions3_for_if|
//@[52:53) Assignment |=|
//@[54:89) Identifier |discriminatorKeyValueMissing_for_if|
//@[89:90) LeftSquare |[|
//@[90:91) Integer |0|
//@[91:92) RightSquare |]|
//@[92:93) LeftSquare |[|
//@[93:94) RightSquare |]|
//@[94:98) NewLine |\r\n\r\n|

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
//@[55:57) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
//@[55:57) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
//@[55:57) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
Discriminator value set 1 (filtered loop)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
//@[0:8) Identifier |resource|
//@[9:38) Identifier |discriminatorKeySetOne_for_if|
//@[39:89) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[90:91) Assignment |=|
//@[92:93) LeftSquare |[|
//@[94:97) Identifier |for|
//@[98:103) Identifier |thing|
//@[104:106) Identifier |in|
//@[107:108) LeftSquare |[|
//@[108:109) RightSquare |]|
//@[109:110) Colon |:|
//@[111:113) Identifier |if|
//@[113:114) LeftParen |(|
//@[114:118) TrueKeyword |true|
//@[118:119) RightParen |)|
//@[120:121) LeftBrace |{|
//@[121:123) NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:18) StringComplete |'AzureCLI'|
//@[18:20) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:57) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
// #completionTest(92) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
//@[0:3) Identifier |var|
//@[4:44) Identifier |discriminatorKeySetOneCompletions_for_if|
//@[45:46) Assignment |=|
//@[47:76) Identifier |discriminatorKeySetOne_for_if|
//@[76:77) LeftSquare |[|
//@[77:78) Integer |0|
//@[78:79) RightSquare |]|
//@[79:80) Dot |.|
//@[80:90) Identifier |properties|
//@[90:91) Dot |.|
//@[91:92) Identifier |a|
//@[92:94) NewLine |\r\n|
// #completionTest(100) -> cliPropertyAccess
//@[44:46) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.
//@[0:3) Identifier |var|
//@[4:45) Identifier |discriminatorKeySetOneCompletions2_for_if|
//@[46:47) Assignment |=|
//@[48:77) Identifier |discriminatorKeySetOne_for_if|
//@[77:78) LeftSquare |[|
//@[78:81) Identifier |any|
//@[81:82) LeftParen |(|
//@[82:86) TrueKeyword |true|
//@[86:87) RightParen |)|
//@[87:88) RightSquare |]|
//@[88:89) Dot |.|
//@[89:99) Identifier |properties|
//@[99:100) Dot |.|
//@[100:104) NewLine |\r\n\r\n|

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
//@[68:70) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]
//@[0:3) Identifier |var|
//@[4:45) Identifier |discriminatorKeySetOneCompletions3_for_if|
//@[46:47) Assignment |=|
//@[48:77) Identifier |discriminatorKeySetOne_for_if|
//@[77:78) LeftSquare |[|
//@[78:79) Integer |1|
//@[79:80) RightSquare |]|
//@[80:81) Dot |.|
//@[81:91) Identifier |properties|
//@[91:92) LeftSquare |[|
//@[92:93) RightSquare |]|
//@[93:99) NewLine |\r\n\r\n\r\n|


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
//@[55:57) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
//@[55:57) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
//@[96:102) NewLine |\r\n\r\n\r\n|


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
//@[55:57) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
//@[101:107) NewLine |\r\n\r\n\r\n|


/*
Discriminator value set 2 (filtered loops)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[0:8) Identifier |resource|
//@[9:38) Identifier |discriminatorKeySetTwo_for_if|
//@[39:89) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[90:91) Assignment |=|
//@[92:93) LeftSquare |[|
//@[93:96) Identifier |for|
//@[97:102) Identifier |thing|
//@[103:105) Identifier |in|
//@[106:107) LeftSquare |[|
//@[107:108) RightSquare |]|
//@[108:109) Colon |:|
//@[110:112) Identifier |if|
//@[112:113) LeftParen |(|
//@[113:117) TrueKeyword |true|
//@[117:118) RightParen |)|
//@[119:120) LeftBrace |{|
//@[120:122) NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:25) StringComplete |'AzurePowerShell'|
//@[25:27) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:57) NewLine |\r\n|
  
//@[2:4) NewLine |\r\n|
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
// #completionTest(92) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
//@[0:3) Identifier |var|
//@[4:44) Identifier |discriminatorKeySetTwoCompletions_for_if|
//@[45:46) Assignment |=|
//@[47:76) Identifier |discriminatorKeySetTwo_for_if|
//@[76:77) LeftSquare |[|
//@[77:78) Integer |0|
//@[78:79) RightSquare |]|
//@[79:80) Dot |.|
//@[80:90) Identifier |properties|
//@[90:91) Dot |.|
//@[91:92) Identifier |a|
//@[92:94) NewLine |\r\n|
// #completionTest(92) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.
//@[0:3) Identifier |var|
//@[4:45) Identifier |discriminatorKeySetTwoCompletions2_for_if|
//@[46:47) Assignment |=|
//@[48:77) Identifier |discriminatorKeySetTwo_for_if|
//@[77:78) LeftSquare |[|
//@[78:79) Integer |0|
//@[79:80) RightSquare |]|
//@[80:81) Dot |.|
//@[81:91) Identifier |properties|
//@[91:92) Dot |.|
//@[92:96) NewLine |\r\n\r\n|

// #completionTest(107) -> powershellPropertyAccess
//@[51:53) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
//@[0:3) Identifier |var|
//@[4:56) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_for_if|
//@[57:58) Assignment |=|
//@[59:88) Identifier |discriminatorKeySetTwo_for_if|
//@[88:89) LeftSquare |[|
//@[89:90) Integer |0|
//@[90:91) RightSquare |]|
//@[91:92) LeftSquare |[|
//@[92:104) StringComplete |'properties'|
//@[104:105) RightSquare |]|
//@[105:106) Dot |.|
//@[106:107) Identifier |a|
//@[107:109) NewLine |\r\n|
// #completionTest(107) -> powershellPropertyAccess
//@[51:53) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].
//@[0:3) Identifier |var|
//@[4:57) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_for_if|
//@[58:59) Assignment |=|
//@[60:89) Identifier |discriminatorKeySetTwo_for_if|
//@[89:90) LeftSquare |[|
//@[90:91) Integer |0|
//@[91:92) RightSquare |]|
//@[92:93) LeftSquare |[|
//@[93:105) StringComplete |'properties'|
//@[105:106) RightSquare |]|
//@[106:107) Dot |.|
//@[107:115) NewLine |\r\n\r\n\r\n\r\n|



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
      // #completionTest(0,1,2,3,4,5,6) -> objectPlusSymbolsWithRequiredProperties
//@[82:84) NewLine |\r\n|
      
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
Nested discriminator missing key (filtered loop)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[0:8) Identifier |resource|
//@[9:45) Identifier |nestedDiscriminatorMissingKey_for_if|
//@[46:104) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[105:106) Assignment |=|
//@[107:108) LeftSquare |[|
//@[108:111) Identifier |for|
//@[112:117) Identifier |thing|
//@[118:120) Identifier |in|
//@[121:122) LeftSquare |[|
//@[122:123) RightSquare |]|
//@[123:124) Colon |:|
//@[125:127) Identifier |if|
//@[127:128) LeftParen |(|
//@[128:132) TrueKeyword |true|
//@[132:133) RightParen |)|
//@[134:135) LeftBrace |{|
//@[135:137) NewLine |\r\n|
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
// #completionTest(107) -> createMode
//@[37:39) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
//@[0:3) Identifier |var|
//@[4:51) Identifier |nestedDiscriminatorMissingKeyCompletions_for_if|
//@[52:53) Assignment |=|
//@[54:90) Identifier |nestedDiscriminatorMissingKey_for_if|
//@[90:91) LeftSquare |[|
//@[91:92) Integer |0|
//@[92:93) RightSquare |]|
//@[93:94) Dot |.|
//@[94:104) Identifier |properties|
//@[104:105) Dot |.|
//@[105:107) Identifier |cr|
//@[107:109) NewLine |\r\n|
// #completionTest(109) -> createMode
//@[37:39) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].
//@[0:3) Identifier |var|
//@[4:52) Identifier |nestedDiscriminatorMissingKeyCompletions2_for_if|
//@[53:54) Assignment |=|
//@[55:91) Identifier |nestedDiscriminatorMissingKey_for_if|
//@[91:92) LeftSquare |[|
//@[92:93) Integer |0|
//@[93:94) RightSquare |]|
//@[94:95) LeftSquare |[|
//@[95:107) StringComplete |'properties'|
//@[107:108) RightSquare |]|
//@[108:109) Dot |.|
//@[109:113) NewLine |\r\n\r\n|

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
//@[60:62) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']
//@[0:3) Identifier |var|
//@[4:56) Identifier |nestedDiscriminatorMissingKeyIndexCompletions_for_if|
//@[57:58) Assignment |=|
//@[59:95) Identifier |nestedDiscriminatorMissingKey_for_if|
//@[95:96) LeftSquare |[|
//@[96:97) Integer |0|
//@[97:98) RightSquare |]|
//@[98:99) Dot |.|
//@[99:109) Identifier |properties|
//@[109:110) LeftSquare |[|
//@[110:112) StringComplete |''|
//@[112:113) RightSquare |]|
//@[113:119) NewLine |\r\n\r\n\r\n|


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
//@[91:97) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator (filtered loop)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |nestedDiscriminator_for_if|
//@[36:94) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[95:96) Assignment |=|
//@[97:98) LeftSquare |[|
//@[98:101) Identifier |for|
//@[102:107) Identifier |thing|
//@[108:110) Identifier |in|
//@[111:112) LeftSquare |[|
//@[112:113) RightSquare |]|
//@[113:114) Colon |:|
//@[115:117) Identifier |if|
//@[117:118) LeftParen |(|
//@[118:122) TrueKeyword |true|
//@[122:123) RightParen |)|
//@[124:125) LeftBrace |{|
//@[125:127) NewLine |\r\n|
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
// #completionTest(86) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
//@[0:3) Identifier |var|
//@[4:41) Identifier |nestedDiscriminatorCompletions_for_if|
//@[42:43) Assignment |=|
//@[44:70) Identifier |nestedDiscriminator_for_if|
//@[70:71) LeftSquare |[|
//@[71:72) Integer |0|
//@[72:73) RightSquare |]|
//@[73:74) Dot |.|
//@[74:84) Identifier |properties|
//@[84:85) Dot |.|
//@[85:86) Identifier |a|
//@[86:88) NewLine |\r\n|
// #completionTest(90) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
//@[0:3) Identifier |var|
//@[4:42) Identifier |nestedDiscriminatorCompletions2_for_if|
//@[43:44) Assignment |=|
//@[45:71) Identifier |nestedDiscriminator_for_if|
//@[71:72) LeftSquare |[|
//@[72:73) Integer |0|
//@[73:74) RightSquare |]|
//@[74:75) LeftSquare |[|
//@[75:87) StringComplete |'properties'|
//@[87:88) RightSquare |]|
//@[88:89) Dot |.|
//@[89:90) Identifier |a|
//@[90:92) NewLine |\r\n|
// #completionTest(86) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
//@[0:3) Identifier |var|
//@[4:42) Identifier |nestedDiscriminatorCompletions3_for_if|
//@[43:44) Assignment |=|
//@[45:71) Identifier |nestedDiscriminator_for_if|
//@[71:72) LeftSquare |[|
//@[72:73) Integer |0|
//@[73:74) RightSquare |]|
//@[74:75) Dot |.|
//@[75:85) Identifier |properties|
//@[85:86) Dot |.|
//@[86:88) NewLine |\r\n|
// #completionTest(89) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].
//@[0:3) Identifier |var|
//@[4:42) Identifier |nestedDiscriminatorCompletions4_for_if|
//@[43:44) Assignment |=|
//@[45:71) Identifier |nestedDiscriminator_for_if|
//@[71:72) LeftSquare |[|
//@[72:73) Integer |0|
//@[73:74) RightSquare |]|
//@[74:75) LeftSquare |[|
//@[75:87) StringComplete |'properties'|
//@[87:88) RightSquare |]|
//@[88:89) Dot |.|
//@[89:93) NewLine |\r\n\r\n|

// #completionTest(96) -> defaultCreateModeIndexes_for_if
//@[57:59) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]
//@[0:3) Identifier |var|
//@[4:51) Identifier |nestedDiscriminatorArrayIndexCompletions_for_if|
//@[52:53) Assignment |=|
//@[54:80) Identifier |nestedDiscriminator_for_if|
//@[80:81) LeftSquare |[|
//@[81:82) Integer |0|
//@[82:83) RightSquare |]|
//@[83:84) Dot |.|
//@[84:94) Identifier |properties|
//@[94:95) LeftSquare |[|
//@[95:96) Identifier |a|
//@[96:97) RightSquare |]|
//@[97:105) NewLine |\r\n\r\n\r\n\r\n|



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

/*
  boolean property value completions
*/ 
//@[3:5) NewLine |\r\n|
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:36) Identifier |booleanPropertyPartialValue|
//@[37:94) StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[95:96) Assignment |=|
//@[97:98) LeftBrace |{|
//@[98:100) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
//@[65:67) NewLine |\r\n|
    autoUpgradeMinorVersion: t
//@[4:27) Identifier |autoUpgradeMinorVersion|
//@[27:28) Colon |:|
//@[29:30) Identifier |t|
//@[30:32) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

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

// loop index parsing cases
//@[27:29) NewLine |\r\n|
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]
//@[0:8) Identifier |resource|
//@[9:29) Identifier |expectedLoopItemName|
//@[30:69) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71) Assignment |=|
//@[72:73) LeftSquare |[|
//@[73:76) Identifier |for|
//@[77:78) LeftParen |(|
//@[78:79) RightParen |)|
//@[79:80) RightSquare |]|
//@[80:84) NewLine |\r\n\r\n|

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (
//@[0:8) Identifier |resource|
//@[9:30) Identifier |expectedLoopItemName2|
//@[31:70) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[71:72) Assignment |=|
//@[73:74) LeftSquare |[|
//@[74:77) Identifier |for|
//@[78:79) LeftParen |(|
//@[79:83) NewLine |\r\n\r\n|

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]
//@[0:8) Identifier |resource|
//@[9:22) Identifier |expectedComma|
//@[23:62) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[63:64) Assignment |=|
//@[65:66) LeftSquare |[|
//@[66:69) Identifier |for|
//@[70:71) LeftParen |(|
//@[71:72) Identifier |x|
//@[72:73) RightParen |)|
//@[73:74) RightSquare |]|
//@[74:78) NewLine |\r\n\r\n|

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]
//@[0:8) Identifier |resource|
//@[9:30) Identifier |expectedLoopIndexName|
//@[31:70) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[71:72) Assignment |=|
//@[73:74) LeftSquare |[|
//@[74:77) Identifier |for|
//@[78:79) LeftParen |(|
//@[79:80) Identifier |x|
//@[80:81) Comma |,|
//@[82:83) RightParen |)|
//@[83:84) RightSquare |]|
//@[84:88) NewLine |\r\n\r\n|

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]
//@[0:8) Identifier |resource|
//@[9:27) Identifier |expectedInKeyword3|
//@[28:67) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[68:69) Assignment |=|
//@[70:71) LeftSquare |[|
//@[71:74) Identifier |for|
//@[75:76) LeftParen |(|
//@[76:77) Identifier |x|
//@[77:78) Comma |,|
//@[79:80) Identifier |y|
//@[80:81) RightParen |)|
//@[81:82) RightSquare |]|
//@[82:86) NewLine |\r\n\r\n|

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]
//@[0:8) Identifier |resource|
//@[9:27) Identifier |expectedInKeyword4|
//@[28:67) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[68:69) Assignment |=|
//@[70:71) LeftSquare |[|
//@[71:74) Identifier |for|
//@[75:76) LeftParen |(|
//@[76:77) Identifier |x|
//@[77:78) Comma |,|
//@[79:80) Identifier |y|
//@[80:81) RightParen |)|
//@[82:83) Identifier |z|
//@[83:84) RightSquare |]|
//@[84:88) NewLine |\r\n\r\n|

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]
//@[0:8) Identifier |resource|
//@[9:33) Identifier |expectedArrayExpression2|
//@[34:73) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[74:75) Assignment |=|
//@[76:77) LeftSquare |[|
//@[77:80) Identifier |for|
//@[81:82) LeftParen |(|
//@[82:83) Identifier |x|
//@[83:84) Comma |,|
//@[85:86) Identifier |y|
//@[86:87) RightParen |)|
//@[88:90) Identifier |in|
//@[91:92) RightSquare |]|
//@[92:96) NewLine |\r\n\r\n|

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]
//@[0:8) Identifier |resource|
//@[9:23) Identifier |expectedColon2|
//@[24:63) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[64:65) Assignment |=|
//@[66:67) LeftSquare |[|
//@[67:70) Identifier |for|
//@[71:72) LeftParen |(|
//@[72:73) Identifier |x|
//@[73:74) Comma |,|
//@[75:76) Identifier |y|
//@[76:77) RightParen |)|
//@[78:80) Identifier |in|
//@[81:82) Identifier |z|
//@[82:83) RightSquare |]|
//@[83:87) NewLine |\r\n\r\n|

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]
//@[0:8) Identifier |resource|
//@[9:26) Identifier |expectedLoopBody2|
//@[27:66) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[67:68) Assignment |=|
//@[69:70) LeftSquare |[|
//@[70:73) Identifier |for|
//@[74:75) LeftParen |(|
//@[75:76) Identifier |x|
//@[76:77) Comma |,|
//@[78:79) Identifier |y|
//@[79:80) RightParen |)|
//@[81:83) Identifier |in|
//@[84:85) Identifier |z|
//@[85:86) Colon |:|
//@[86:87) RightSquare |]|
//@[87:91) NewLine |\r\n\r\n|

// loop filter parsing cases
//@[28:30) NewLine |\r\n|
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
//@[0:8) Identifier |resource|
//@[9:36) Identifier |expectedLoopFilterOpenParen|
//@[37:83) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[84:85) Assignment |=|
//@[86:87) LeftSquare |[|
//@[87:90) Identifier |for|
//@[91:92) Identifier |x|
//@[93:95) Identifier |in|
//@[96:97) Identifier |y|
//@[97:98) Colon |:|
//@[99:101) Identifier |if|
//@[101:102) RightSquare |]|
//@[102:104) NewLine |\r\n|
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]
//@[0:8) Identifier |resource|
//@[9:37) Identifier |expectedLoopFilterOpenParen2|
//@[38:77) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[78:79) Assignment |=|
//@[80:81) LeftSquare |[|
//@[81:84) Identifier |for|
//@[85:86) LeftParen |(|
//@[86:87) Identifier |x|
//@[87:88) Comma |,|
//@[89:90) Identifier |y|
//@[90:91) RightParen |)|
//@[92:94) Identifier |in|
//@[95:96) Identifier |z|
//@[96:97) Colon |:|
//@[98:100) Identifier |if|
//@[100:101) RightSquare |]|
//@[101:105) NewLine |\r\n\r\n|

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
//@[0:8) Identifier |resource|
//@[9:43) Identifier |expectedLoopFilterPredicateAndBody|
//@[44:90) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[91:92) Assignment |=|
//@[93:94) LeftSquare |[|
//@[94:97) Identifier |for|
//@[98:99) Identifier |x|
//@[100:102) Identifier |in|
//@[103:104) Identifier |y|
//@[104:105) Colon |:|
//@[106:108) Identifier |if|
//@[108:109) LeftParen |(|
//@[109:110) RightParen |)|
//@[110:111) RightSquare |]|
//@[111:113) NewLine |\r\n|
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]
//@[0:8) Identifier |resource|
//@[9:44) Identifier |expectedLoopFilterPredicateAndBody2|
//@[45:84) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[85:86) Assignment |=|
//@[87:88) LeftSquare |[|
//@[88:91) Identifier |for|
//@[92:93) LeftParen |(|
//@[93:94) Identifier |x|
//@[94:95) Comma |,|
//@[96:97) Identifier |y|
//@[97:98) RightParen |)|
//@[99:101) Identifier |in|
//@[102:103) Identifier |z|
//@[103:104) Colon |:|
//@[105:107) Identifier |if|
//@[107:108) LeftParen |(|
//@[108:109) RightParen |)|
//@[109:110) RightSquare |]|
//@[110:114) NewLine |\r\n\r\n|

// wrong body type
//@[18:20) NewLine |\r\n|
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
//@[99:101) NewLine |\r\n|
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]
//@[0:8) Identifier |resource|
//@[9:27) Identifier |wrongLoopBodyType2|
//@[28:74) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[75:76) Assignment |=|
//@[77:78) LeftSquare |[|
//@[78:81) Identifier |for|
//@[82:83) LeftParen |(|
//@[83:84) Identifier |x|
//@[85:86) Comma |,|
//@[86:87) Identifier |i|
//@[87:88) RightParen |)|
//@[89:91) Identifier |in|
//@[92:102) Identifier |emptyArray|
//@[102:103) Colon |:|
//@[103:104) Integer |4|
//@[104:105) RightSquare |]|
//@[105:109) NewLine |\r\n\r\n|

// duplicate variable in the same scope
//@[39:41) NewLine |\r\n|
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
//@[0:8) Identifier |resource|
//@[9:29) Identifier |itemAndIndexSameName|
//@[30:71) StringComplete |'Microsoft.AAD/domainServices@2020-01-01'|
//@[72:73) Assignment |=|
//@[74:75) LeftSquare |[|
//@[75:78) Identifier |for|
//@[79:80) LeftParen |(|
//@[80:84) Identifier |same|
//@[84:85) Comma |,|
//@[86:90) Identifier |same|
//@[90:91) RightParen |)|
//@[92:94) Identifier |in|
//@[95:105) Identifier |emptyArray|
//@[105:106) Colon |:|
//@[107:108) LeftBrace |{|
//@[108:110) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

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
//@[2:4) NewLine |\r\n|
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
//@[0:8) Identifier |resource|
//@[9:31) Identifier |arrayExpressionErrors2|
//@[32:78) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[79:80) Assignment |=|
//@[81:82) LeftSquare |[|
//@[82:85) Identifier |for|
//@[86:87) LeftParen |(|
//@[87:94) Identifier |account|
//@[94:95) Comma |,|
//@[95:96) Identifier |k|
//@[96:97) RightParen |)|
//@[98:100) Identifier |in|
//@[101:106) Identifier |union|
//@[106:107) LeftParen |(|
//@[107:108) LeftSquare |[|
//@[108:109) RightSquare |]|
//@[109:110) Comma |,|
//@[111:112) Integer |2|
//@[112:113) RightParen |)|
//@[113:114) Colon |:|
//@[115:116) LeftBrace |{|
//@[116:118) NewLine |\r\n|
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
//@[2:4) NewLine |\r\n|
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
//@[0:8) Identifier |resource|
//@[9:24) Identifier |wrongArrayType2|
//@[25:71) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73) Assignment |=|
//@[74:75) LeftSquare |[|
//@[75:78) Identifier |for|
//@[79:80) LeftParen |(|
//@[80:87) Identifier |account|
//@[87:88) Comma |,|
//@[88:89) Identifier |i|
//@[89:90) RightParen |)|
//@[91:93) Identifier |in|
//@[94:104) Identifier |notAnArray|
//@[104:105) Colon |:|
//@[106:107) LeftBrace |{|
//@[107:109) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// wrong filter expression type
//@[31:33) NewLine |\r\n|
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |wrongFilterExpressionType|
//@[35:81) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[82:83) Assignment |=|
//@[84:85) LeftSquare |[|
//@[85:88) Identifier |for|
//@[89:96) Identifier |account|
//@[97:99) Identifier |in|
//@[100:110) Identifier |emptyArray|
//@[110:111) Colon |:|
//@[112:114) Identifier |if|
//@[114:115) LeftParen |(|
//@[115:116) Integer |4|
//@[116:117) RightParen |)|
//@[118:119) LeftBrace |{|
//@[119:121) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
//@[0:8) Identifier |resource|
//@[9:35) Identifier |wrongFilterExpressionType2|
//@[36:82) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[83:84) Assignment |=|
//@[85:86) LeftSquare |[|
//@[86:89) Identifier |for|
//@[90:91) LeftParen |(|
//@[91:98) Identifier |account|
//@[98:99) Comma |,|
//@[99:100) Identifier |i|
//@[100:101) RightParen |)|
//@[102:104) Identifier |in|
//@[105:115) Identifier |emptyArray|
//@[115:116) Colon |:|
//@[117:119) Identifier |if|
//@[119:120) LeftParen |(|
//@[120:126) Identifier |concat|
//@[126:127) LeftParen |(|
//@[127:130) StringComplete |'s'|
//@[130:131) RightParen |)|
//@[131:132) RightParen |)|
//@[132:133) LeftBrace |{|
//@[133:135) NewLine |\r\n|
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
//@[2:4) NewLine |\r\n|
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |missingRequiredProperties2|
//@[36:82) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[83:84) Assignment |=|
//@[85:86) LeftSquare |[|
//@[86:89) Identifier |for|
//@[90:91) LeftParen |(|
//@[91:98) Identifier |account|
//@[98:99) Comma |,|
//@[99:100) Identifier |j|
//@[100:101) RightParen |)|
//@[102:104) Identifier |in|
//@[105:106) LeftSquare |[|
//@[106:107) RightSquare |]|
//@[107:108) Colon |:|
//@[109:110) LeftBrace |{|
//@[110:112) NewLine |\r\n|
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
//@[2:4) NewLine |\r\n|
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |wrongPropertyInNestedLoop2|
//@[36:82) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[83:84) Assignment |=|
//@[85:86) LeftSquare |[|
//@[86:89) Identifier |for|
//@[90:91) LeftParen |(|
//@[91:92) Identifier |i|
//@[92:93) Comma |,|
//@[93:94) Identifier |k|
//@[94:95) RightParen |)|
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
      name: 'subnet-${i}-${j}-${k}'
//@[6:10) Identifier |name|
//@[10:11) Colon |:|
//@[12:22) StringLeftPiece |'subnet-${|
//@[22:23) Identifier |i|
//@[23:27) StringMiddlePiece |}-${|
//@[27:28) Identifier |j|
//@[28:32) StringMiddlePiece |}-${|
//@[32:33) Identifier |k|
//@[33:35) StringRightPiece |}'|
//@[35:37) NewLine |\r\n|
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

// property loops cannot be nested
//@[34:36) NewLine |\r\n|
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:8) Identifier |resource|
//@[9:32) Identifier |propertyLoopsCannotNest|
//@[33:79) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[80:81) Assignment |=|
//@[82:83) LeftSquare |[|
//@[83:86) Identifier |for|
//@[87:94) Identifier |account|
//@[95:97) Identifier |in|
//@[98:113) Identifier |storageAccounts|
//@[113:114) Colon |:|
//@[115:116) LeftBrace |{|
//@[116:118) NewLine |\r\n|
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
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:19) NewLine |\r\n\r\n|

    networkAcls: {
//@[4:15) Identifier |networkAcls|
//@[15:16) Colon |:|
//@[17:18) LeftBrace |{|
//@[18:20) NewLine |\r\n|
      virtualNetworkRules: [for rule in []: {
//@[6:25) Identifier |virtualNetworkRules|
//@[25:26) Colon |:|
//@[27:28) LeftSquare |[|
//@[28:31) Identifier |for|
//@[32:36) Identifier |rule|
//@[37:39) Identifier |in|
//@[40:41) LeftSquare |[|
//@[41:42) RightSquare |]|
//@[42:43) Colon |:|
//@[44:45) LeftBrace |{|
//@[45:47) NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[8:10) Identifier |id|
//@[10:11) Colon |:|
//@[12:15) StringLeftPiece |'${|
//@[15:22) Identifier |account|
//@[22:23) Dot |.|
//@[23:27) Identifier |name|
//@[27:31) StringMiddlePiece |}-${|
//@[31:38) Identifier |account|
//@[38:39) Dot |.|
//@[39:47) Identifier |location|
//@[47:49) StringRightPiece |}'|
//@[49:51) NewLine |\r\n|
        state: [for lol in []: 4]
//@[8:13) Identifier |state|
//@[13:14) Colon |:|
//@[15:16) LeftSquare |[|
//@[16:19) Identifier |for|
//@[20:23) Identifier |lol|
//@[24:26) Identifier |in|
//@[27:28) LeftSquare |[|
//@[28:29) RightSquare |]|
//@[29:30) Colon |:|
//@[31:32) Integer |4|
//@[32:33) RightSquare |]|
//@[33:35) NewLine |\r\n|
      }]
//@[6:7) RightBrace |}|
//@[7:8) RightSquare |]|
//@[8:10) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
//@[0:8) Identifier |resource|
//@[9:33) Identifier |propertyLoopsCannotNest2|
//@[34:80) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[81:82) Assignment |=|
//@[83:84) LeftSquare |[|
//@[84:87) Identifier |for|
//@[88:89) LeftParen |(|
//@[89:96) Identifier |account|
//@[96:97) Comma |,|
//@[97:98) Identifier |i|
//@[98:99) RightParen |)|
//@[100:102) Identifier |in|
//@[103:118) Identifier |storageAccounts|
//@[118:119) Colon |:|
//@[120:121) LeftBrace |{|
//@[121:123) NewLine |\r\n|
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
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:19) NewLine |\r\n\r\n|

    networkAcls: {
//@[4:15) Identifier |networkAcls|
//@[15:16) Colon |:|
//@[17:18) LeftBrace |{|
//@[18:20) NewLine |\r\n|
      virtualNetworkRules: [for (rule,j) in []: {
//@[6:25) Identifier |virtualNetworkRules|
//@[25:26) Colon |:|
//@[27:28) LeftSquare |[|
//@[28:31) Identifier |for|
//@[32:33) LeftParen |(|
//@[33:37) Identifier |rule|
//@[37:38) Comma |,|
//@[38:39) Identifier |j|
//@[39:40) RightParen |)|
//@[41:43) Identifier |in|
//@[44:45) LeftSquare |[|
//@[45:46) RightSquare |]|
//@[46:47) Colon |:|
//@[48:49) LeftBrace |{|
//@[49:51) NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[8:10) Identifier |id|
//@[10:11) Colon |:|
//@[12:15) StringLeftPiece |'${|
//@[15:22) Identifier |account|
//@[22:23) Dot |.|
//@[23:27) Identifier |name|
//@[27:31) StringMiddlePiece |}-${|
//@[31:38) Identifier |account|
//@[38:39) Dot |.|
//@[39:47) Identifier |location|
//@[47:49) StringRightPiece |}'|
//@[49:51) NewLine |\r\n|
        state: [for (lol,k) in []: 4]
//@[8:13) Identifier |state|
//@[13:14) Colon |:|
//@[15:16) LeftSquare |[|
//@[16:19) Identifier |for|
//@[20:21) LeftParen |(|
//@[21:24) Identifier |lol|
//@[24:25) Comma |,|
//@[25:26) Identifier |k|
//@[26:27) RightParen |)|
//@[28:30) Identifier |in|
//@[31:32) LeftSquare |[|
//@[32:33) RightSquare |]|
//@[33:34) Colon |:|
//@[35:36) Integer |4|
//@[36:37) RightSquare |]|
//@[37:39) NewLine |\r\n|
      }]
//@[6:7) RightBrace |}|
//@[7:8) RightSquare |]|
//@[8:10) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// property loops cannot be nested (even more nesting)
//@[54:56) NewLine |\r\n|
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:8) Identifier |resource|
//@[9:33) Identifier |propertyLoopsCannotNest2|
//@[34:80) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[81:82) Assignment |=|
//@[83:84) LeftSquare |[|
//@[84:87) Identifier |for|
//@[88:95) Identifier |account|
//@[96:98) Identifier |in|
//@[99:114) Identifier |storageAccounts|
//@[114:115) Colon |:|
//@[116:117) LeftBrace |{|
//@[117:119) NewLine |\r\n|
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
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    networkAcls:  {
//@[4:15) Identifier |networkAcls|
//@[15:16) Colon |:|
//@[18:19) LeftBrace |{|
//@[19:21) NewLine |\r\n|
      virtualNetworkRules: [for rule in []: {
//@[6:25) Identifier |virtualNetworkRules|
//@[25:26) Colon |:|
//@[27:28) LeftSquare |[|
//@[28:31) Identifier |for|
//@[32:36) Identifier |rule|
//@[37:39) Identifier |in|
//@[40:41) LeftSquare |[|
//@[41:42) RightSquare |]|
//@[42:43) Colon |:|
//@[44:45) LeftBrace |{|
//@[45:47) NewLine |\r\n|
        // #completionTest(15,31) -> symbolsPlusRule
//@[52:54) NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[8:10) Identifier |id|
//@[10:11) Colon |:|
//@[12:15) StringLeftPiece |'${|
//@[15:22) Identifier |account|
//@[22:23) Dot |.|
//@[23:27) Identifier |name|
//@[27:31) StringMiddlePiece |}-${|
//@[31:38) Identifier |account|
//@[38:39) Dot |.|
//@[39:47) Identifier |location|
//@[47:49) StringRightPiece |}'|
//@[49:51) NewLine |\r\n|
        state: [for state in []: {
//@[8:13) Identifier |state|
//@[13:14) Colon |:|
//@[15:16) LeftSquare |[|
//@[16:19) Identifier |for|
//@[20:25) Identifier |state|
//@[26:28) Identifier |in|
//@[29:30) LeftSquare |[|
//@[30:31) RightSquare |]|
//@[31:32) Colon |:|
//@[33:34) LeftBrace |{|
//@[34:36) NewLine |\r\n|
          // #completionTest(38) -> empty #completionTest(16) -> symbolsPlusAccountRuleState
//@[92:94) NewLine |\r\n|
          fake: [for something in []: true]
//@[10:14) Identifier |fake|
//@[14:15) Colon |:|
//@[16:17) LeftSquare |[|
//@[17:20) Identifier |for|
//@[21:30) Identifier |something|
//@[31:33) Identifier |in|
//@[34:35) LeftSquare |[|
//@[35:36) RightSquare |]|
//@[36:37) Colon |:|
//@[38:42) TrueKeyword |true|
//@[42:43) RightSquare |]|
//@[43:45) NewLine |\r\n|
        }]
//@[8:9) RightBrace |}|
//@[9:10) RightSquare |]|
//@[10:12) NewLine |\r\n|
      }]
//@[6:7) RightBrace |}|
//@[7:8) RightSquare |]|
//@[8:10) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// loops cannot be used inside of expressions
//@[45:47) NewLine |\r\n|
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:8) Identifier |resource|
//@[9:15) Identifier |stuffs|
//@[16:62) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[63:64) Assignment |=|
//@[65:66) LeftSquare |[|
//@[66:69) Identifier |for|
//@[70:77) Identifier |account|
//@[78:80) Identifier |in|
//@[81:96) Identifier |storageAccounts|
//@[96:97) Colon |:|
//@[98:99) LeftBrace |{|
//@[99:101) NewLine |\r\n|
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
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    networkAcls: {
//@[4:15) Identifier |networkAcls|
//@[15:16) Colon |:|
//@[17:18) LeftBrace |{|
//@[18:20) NewLine |\r\n|
      virtualNetworkRules: concat([for lol in []: {
//@[6:25) Identifier |virtualNetworkRules|
//@[25:26) Colon |:|
//@[27:33) Identifier |concat|
//@[33:34) LeftParen |(|
//@[34:35) LeftSquare |[|
//@[35:38) Identifier |for|
//@[39:42) Identifier |lol|
//@[43:45) Identifier |in|
//@[46:47) LeftSquare |[|
//@[47:48) RightSquare |]|
//@[48:49) Colon |:|
//@[50:51) LeftBrace |{|
//@[51:53) NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[8:10) Identifier |id|
//@[10:11) Colon |:|
//@[12:15) StringLeftPiece |'${|
//@[15:22) Identifier |account|
//@[22:23) Dot |.|
//@[23:27) Identifier |name|
//@[27:31) StringMiddlePiece |}-${|
//@[31:38) Identifier |account|
//@[38:39) Dot |.|
//@[39:47) Identifier |location|
//@[47:49) StringRightPiece |}'|
//@[49:51) NewLine |\r\n|
      }])
//@[6:7) RightBrace |}|
//@[7:8) RightSquare |]|
//@[8:9) RightParen |)|
//@[9:11) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

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
  // #completionTest(7,8) -> symbolsPlusAccount2
//@[48:50) NewLine |\r\n|
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
//@[2:6) NewLine |\r\n\r\n|

var directRefViaVar = premiumStorages
//@[0:3) Identifier |var|
//@[4:19) Identifier |directRefViaVar|
//@[20:21) Assignment |=|
//@[22:37) Identifier |premiumStorages|
//@[37:39) NewLine |\r\n|
output directRefViaOutput array = union(premiumStorages, stuffs)
//@[0:6) Identifier |output|
//@[7:25) Identifier |directRefViaOutput|
//@[26:31) Identifier |array|
//@[32:33) Assignment |=|
//@[34:39) Identifier |union|
//@[39:40) LeftParen |(|
//@[40:55) Identifier |premiumStorages|
//@[55:56) Comma |,|
//@[57:63) Identifier |stuffs|
//@[63:64) RightParen |)|
//@[64:68) NewLine |\r\n\r\n|

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
//@[0:8) Identifier |resource|
//@[9:39) Identifier |directRefViaSingleResourceBody|
//@[40:79) StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[80:81) Assignment |=|
//@[82:83) LeftBrace |{|
//@[83:85) NewLine |\r\n|
  name: 'myZone2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:17) StringComplete |'myZone2'|
//@[17:19) NewLine |\r\n|
  location: 'global'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'global'|
//@[20:22) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    registrationVirtualNetworks: premiumStorages
//@[4:31) Identifier |registrationVirtualNetworks|
//@[31:32) Colon |:|
//@[33:48) Identifier |premiumStorages|
//@[48:50) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
//@[0:8) Identifier |resource|
//@[9:50) Identifier |directRefViaSingleConditionalResourceBody|
//@[51:90) StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[91:92) Assignment |=|
//@[93:95) Identifier |if|
//@[95:96) LeftParen |(|
//@[96:100) TrueKeyword |true|
//@[100:101) RightParen |)|
//@[102:103) LeftBrace |{|
//@[103:105) NewLine |\r\n|
  name: 'myZone3'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:17) StringComplete |'myZone3'|
//@[17:19) NewLine |\r\n|
  location: 'global'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'global'|
//@[20:22) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
//@[4:31) Identifier |registrationVirtualNetworks|
//@[31:32) Colon |:|
//@[33:39) Identifier |concat|
//@[39:40) LeftParen |(|
//@[40:55) Identifier |premiumStorages|
//@[55:56) Comma |,|
//@[57:63) Identifier |stuffs|
//@[63:64) RightParen |)|
//@[64:66) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@batchSize()
//@[0:1) At |@|
//@[1:10) Identifier |batchSize|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:14) NewLine |\r\n|
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:8) Identifier |resource|
//@[9:43) Identifier |directRefViaSingleLoopResourceBody|
//@[44:90) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[91:92) Assignment |=|
//@[93:94) LeftSquare |[|
//@[94:97) Identifier |for|
//@[98:99) Identifier |i|
//@[100:102) Identifier |in|
//@[103:108) Identifier |range|
//@[108:109) LeftParen |(|
//@[109:110) Integer |0|
//@[110:111) Comma |,|
//@[112:113) Integer |3|
//@[113:114) RightParen |)|
//@[114:115) Colon |:|
//@[116:117) LeftBrace |{|
//@[117:119) NewLine |\r\n|
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
    subnets: premiumStorages
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:28) Identifier |premiumStorages|
//@[28:30) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

@batchSize(0)
//@[0:1) At |@|
//@[1:10) Identifier |batchSize|
//@[10:11) LeftParen |(|
//@[11:12) Integer |0|
//@[12:13) RightParen |)|
//@[13:15) NewLine |\r\n|
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:8) Identifier |resource|
//@[9:61) Identifier |directRefViaSingleLoopResourceBodyWithExtraDependsOn|
//@[62:108) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[109:110) Assignment |=|
//@[111:112) LeftSquare |[|
//@[112:115) Identifier |for|
//@[116:117) Identifier |i|
//@[118:120) Identifier |in|
//@[121:126) Identifier |range|
//@[126:127) LeftParen |(|
//@[127:128) Integer |0|
//@[128:129) Comma |,|
//@[130:131) Integer |3|
//@[131:132) RightParen |)|
//@[132:133) Colon |:|
//@[134:135) LeftBrace |{|
//@[135:137) NewLine |\r\n|
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
    subnets: premiumStorages
//@[4:11) Identifier |subnets|
//@[11:12) Colon |:|
//@[13:28) Identifier |premiumStorages|
//@[28:30) NewLine |\r\n|
    dependsOn: [
//@[4:13) Identifier |dependsOn|
//@[13:14) Colon |:|
//@[15:16) LeftSquare |[|
//@[16:18) NewLine |\r\n|
      premiumStorages
//@[6:21) Identifier |premiumStorages|
//@[21:23) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    
//@[4:6) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

var expressionInPropertyLoopVar = true
//@[0:3) Identifier |var|
//@[4:31) Identifier |expressionInPropertyLoopVar|
//@[32:33) Assignment |=|
//@[34:38) TrueKeyword |true|
//@[38:40) NewLine |\r\n|
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:8) Identifier |resource|
//@[9:38) Identifier |expressionsInPropertyLoopName|
//@[39:78) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[79:80) Assignment |=|
//@[81:82) LeftBrace |{|
//@[82:84) NewLine |\r\n|
  name: 'hello'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) StringComplete |'hello'|
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
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
//@[4:32) StringLeftPiece |'resolutionVirtualNetworks${|
//@[32:59) Identifier |expressionInPropertyLoopVar|
//@[59:61) StringRightPiece |}'|
//@[61:62) Colon |:|
//@[63:64) LeftSquare |[|
//@[64:67) Identifier |for|
//@[68:73) Identifier |thing|
//@[74:76) Identifier |in|
//@[77:78) LeftSquare |[|
//@[78:79) RightSquare |]|
//@[79:80) Colon |:|
//@[81:82) LeftBrace |{|
//@[82:83) RightBrace |}|
//@[83:84) RightSquare |]|
//@[84:86) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// resource loop body that isn't an object
//@[42:44) NewLine |\r\n|
@batchSize(-1)
//@[0:1) At |@|
//@[1:10) Identifier |batchSize|
//@[10:11) LeftParen |(|
//@[11:12) Minus |-|
//@[12:13) Integer |1|
//@[13:14) RightParen |)|
//@[14:16) NewLine |\r\n|
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
//@[0:8) Identifier |resource|
//@[9:34) Identifier |nonObjectResourceLoopBody|
//@[35:74) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[75:76) Assignment |=|
//@[77:78) LeftSquare |[|
//@[78:81) Identifier |for|
//@[82:87) Identifier |thing|
//@[88:90) Identifier |in|
//@[91:92) LeftSquare |[|
//@[92:93) RightSquare |]|
//@[93:94) Colon |:|
//@[95:101) StringComplete |'test'|
//@[101:102) RightSquare |]|
//@[102:104) NewLine |\r\n|
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
//@[0:8) Identifier |resource|
//@[9:35) Identifier |nonObjectResourceLoopBody2|
//@[36:75) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftSquare |[|
//@[79:82) Identifier |for|
//@[83:88) Identifier |thing|
//@[89:91) Identifier |in|
//@[92:93) LeftSquare |[|
//@[93:94) RightSquare |]|
//@[94:95) Colon |:|
//@[96:107) Identifier |environment|
//@[107:108) LeftParen |(|
//@[108:109) RightParen |)|
//@[109:110) RightSquare |]|
//@[110:112) NewLine |\r\n|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
//@[0:8) Identifier |resource|
//@[9:35) Identifier |nonObjectResourceLoopBody3|
//@[36:75) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftSquare |[|
//@[79:82) Identifier |for|
//@[83:84) LeftParen |(|
//@[84:89) Identifier |thing|
//@[89:90) Comma |,|
//@[90:91) Identifier |i|
//@[91:92) RightParen |)|
//@[93:95) Identifier |in|
//@[96:97) LeftSquare |[|
//@[97:98) RightSquare |]|
//@[98:99) Colon |:|
//@[100:106) StringComplete |'test'|
//@[106:107) RightSquare |]|
//@[107:109) NewLine |\r\n|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
//@[0:8) Identifier |resource|
//@[9:35) Identifier |nonObjectResourceLoopBody4|
//@[36:75) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftSquare |[|
//@[79:82) Identifier |for|
//@[83:84) LeftParen |(|
//@[84:89) Identifier |thing|
//@[89:90) Comma |,|
//@[90:91) Identifier |i|
//@[91:92) RightParen |)|
//@[93:95) Identifier |in|
//@[96:97) LeftSquare |[|
//@[97:98) RightSquare |]|
//@[98:99) Colon |:|
//@[100:111) Identifier |environment|
//@[111:112) LeftParen |(|
//@[112:113) RightParen |)|
//@[113:114) RightSquare |]|
//@[114:116) NewLine |\r\n|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
//@[0:8) Identifier |resource|
//@[9:35) Identifier |nonObjectResourceLoopBody3|
//@[36:75) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftSquare |[|
//@[79:82) Identifier |for|
//@[83:84) LeftParen |(|
//@[84:89) Identifier |thing|
//@[89:90) Comma |,|
//@[90:91) Identifier |i|
//@[91:92) RightParen |)|
//@[93:95) Identifier |in|
//@[96:97) LeftSquare |[|
//@[97:98) RightSquare |]|
//@[98:99) Colon |:|
//@[100:102) Identifier |if|
//@[102:103) LeftParen |(|
//@[103:107) TrueKeyword |true|
//@[107:108) RightParen |)|
//@[109:115) StringComplete |'test'|
//@[115:116) RightSquare |]|
//@[116:118) NewLine |\r\n|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]
//@[0:8) Identifier |resource|
//@[9:35) Identifier |nonObjectResourceLoopBody4|
//@[36:75) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftSquare |[|
//@[79:82) Identifier |for|
//@[83:84) LeftParen |(|
//@[84:89) Identifier |thing|
//@[89:90) Comma |,|
//@[90:91) Identifier |i|
//@[91:92) RightParen |)|
//@[93:95) Identifier |in|
//@[96:97) LeftSquare |[|
//@[97:98) RightSquare |]|
//@[98:99) Colon |:|
//@[100:102) Identifier |if|
//@[102:103) LeftParen |(|
//@[103:107) TrueKeyword |true|
//@[107:108) RightParen |)|
//@[109:120) Identifier |environment|
//@[120:121) LeftParen |(|
//@[121:122) RightParen |)|
//@[122:123) RightSquare |]|
//@[123:127) NewLine |\r\n\r\n|

// #completionTest(54,55) -> objectPlusFor
//@[42:44) NewLine |\r\n|
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:52) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[53:54) Assignment |=|
//@[55:59) NewLine |\r\n\r\n|

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |foo|
//@[13:52) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[53:54) Assignment |=|
//@[55:56) LeftSquare |[|
//@[56:59) Identifier |for|
//@[60:64) Identifier |item|
//@[65:67) Identifier |in|
//@[68:69) LeftSquare |[|
//@[69:70) RightSquare |]|
//@[70:71) Colon |:|
//@[72:73) LeftBrace |{|
//@[73:75) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
//@[55:57) NewLine |\r\n|
    registrationVirtualNetworks: 
//@[4:31) Identifier |registrationVirtualNetworks|
//@[31:32) Colon |:|
//@[33:35) NewLine |\r\n|
    resolutionVirtualNetworks: [for lol in []: {
//@[4:29) Identifier |resolutionVirtualNetworks|
//@[29:30) Colon |:|
//@[31:32) LeftSquare |[|
//@[32:35) Identifier |for|
//@[36:39) Identifier |lol|
//@[40:42) Identifier |in|
//@[43:44) LeftSquare |[|
//@[44:45) RightSquare |]|
//@[45:46) Colon |:|
//@[47:48) LeftBrace |{|
//@[48:50) NewLine |\r\n|
      
//@[6:8) NewLine |\r\n|
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

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:13) Identifier |vnet|
//@[14:60) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    virtualNetworkPeerings: [for item in []: {
//@[4:26) Identifier |virtualNetworkPeerings|
//@[26:27) Colon |:|
//@[28:29) LeftSquare |[|
//@[29:32) Identifier |for|
//@[33:37) Identifier |item|
//@[38:40) Identifier |in|
//@[41:42) LeftSquare |[|
//@[42:43) RightSquare |]|
//@[43:44) Colon |:|
//@[45:46) LeftBrace |{|
//@[46:48) NewLine |\r\n|
        properties: {
//@[8:18) Identifier |properties|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:23) NewLine |\r\n|
          remoteAddressSpace: {
//@[10:28) Identifier |remoteAddressSpace|
//@[28:29) Colon |:|
//@[30:31) LeftBrace |{|
//@[31:33) NewLine |\r\n|
            // #completionTest(28,29) -> symbolsPlusArrayWithoutFor
//@[67:69) NewLine |\r\n|
            addressPrefixes: 
//@[12:27) Identifier |addressPrefixes|
//@[27:28) Colon |:|
//@[29:31) NewLine |\r\n|
          }
//@[10:11) RightBrace |}|
//@[11:13) NewLine |\r\n|
        }
//@[8:9) RightBrace |}|
//@[9:11) NewLine |\r\n|
    }]
//@[4:5) RightBrace |}|
//@[5:6) RightSquare |]|
//@[6:8) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with 'existing' resource at different scope
//@[62:64) NewLine |\r\n|
resource p1_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p1_res1|
//@[17:53) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:62) Identifier |existing|
//@[63:64) Assignment |=|
//@[65:66) LeftBrace |{|
//@[66:68) NewLine |\r\n|
  scope: subscription()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:21) Identifier |subscription|
//@[21:22) LeftParen |(|
//@[22:23) RightParen |)|
//@[23:25) NewLine |\r\n|
  name: 'res1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'res1'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:18) Identifier |p1_child1|
//@[19:62) StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[63:64) Assignment |=|
//@[65:66) LeftBrace |{|
//@[66:68) NewLine |\r\n|
  parent: p1_res1
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:17) Identifier |p1_res1|
//@[17:19) NewLine |\r\n|
  name: 'child1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringComplete |'child1'|
//@[16:18) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with scope on child resource
//@[47:49) NewLine |\r\n|
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p2_res1|
//@[17:53) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55) Assignment |=|
//@[56:57) LeftBrace |{|
//@[57:59) NewLine |\r\n|
  name: 'res1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'res1'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p2_res2|
//@[17:53) StringComplete |'Microsoft.Rp2/resource2@2020-06-01'|
//@[54:55) Assignment |=|
//@[56:57) LeftBrace |{|
//@[57:59) NewLine |\r\n|
  name: 'res2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'res2'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:21) Identifier |p2_res2child|
//@[22:65) StringComplete |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[66:67) Assignment |=|
//@[68:69) LeftBrace |{|
//@[69:71) NewLine |\r\n|
  scope: p2_res1
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:16) Identifier |p2_res1|
//@[16:18) NewLine |\r\n|
  parent: p2_res2
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:17) Identifier |p2_res2|
//@[17:19) NewLine |\r\n|
  name: 'child2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringComplete |'child2'|
//@[16:18) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property self-cycle
//@[29:31) NewLine |\r\n|
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:17) Identifier |p3_vmExt|
//@[18:75) StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftBrace |{|
//@[79:81) NewLine |\r\n|
  parent: p3_vmExt
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:18) Identifier |p3_vmExt|
//@[18:20) NewLine |\r\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:22) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property 2-cycle
//@[26:28) NewLine |\r\n|
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:14) Identifier |p4_vm|
//@[15:61) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[62:63) Assignment |=|
//@[64:65) LeftBrace |{|
//@[65:67) NewLine |\r\n|
  parent: p4_vmExt
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:18) Identifier |p4_vmExt|
//@[18:20) NewLine |\r\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:22) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:17) Identifier |p4_vmExt|
//@[18:75) StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftBrace |{|
//@[79:81) NewLine |\r\n|
  parent: p4_vm
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:15) Identifier |p4_vm|
//@[15:17) NewLine |\r\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:22) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with invalid child
//@[37:39) NewLine |\r\n|
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p5_res1|
//@[17:53) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55) Assignment |=|
//@[56:57) LeftBrace |{|
//@[57:59) NewLine |\r\n|
  name: 'res1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'res1'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p5_res2|
//@[17:60) StringComplete |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  parent: p5_res1
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:17) Identifier |p5_res1|
//@[17:19) NewLine |\r\n|
  name: 'res2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'res2'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with invalid parent
//@[38:40) NewLine |\r\n|
resource p6_res1 '${true}' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p6_res1|
//@[17:20) StringLeftPiece |'${|
//@[20:24) TrueKeyword |true|
//@[24:26) StringRightPiece |}'|
//@[27:28) Assignment |=|
//@[29:30) LeftBrace |{|
//@[30:32) NewLine |\r\n|
  name: 'res1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'res1'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p6_res2|
//@[17:60) StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  parent: p6_res1
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:17) Identifier |p6_res1|
//@[17:19) NewLine |\r\n|
  name: 'res2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'res2'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with incorrectly-formatted name
//@[50:52) NewLine |\r\n|
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p7_res1|
//@[17:53) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55) Assignment |=|
//@[56:57) LeftBrace |{|
//@[57:59) NewLine |\r\n|
  name: 'res1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'res1'|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p7_res2|
//@[17:60) StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  parent: p7_res1
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:17) Identifier |p7_res1|
//@[17:19) NewLine |\r\n|
  name: 'res1/res2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'res1/res2'|
//@[19:21) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p7_res3|
//@[17:60) StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  parent: p7_res1
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:17) Identifier |p7_res1|
//@[17:19) NewLine |\r\n|
  name: '${p7_res1.name}/res2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:18) Identifier |p7_res1|
//@[18:19) Dot |.|
//@[19:23) Identifier |name|
//@[23:30) StringRightPiece |}/res2'|
//@[30:32) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// top-level resource with too many '/' characters
//@[50:52) NewLine |\r\n|
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |p8_res1|
//@[17:53) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55) Assignment |=|
//@[56:57) LeftBrace |{|
//@[57:59) NewLine |\r\n|
  name: 'res1/res2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'res1/res2'|
//@[19:21) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource existingResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |existingResProperty|
//@[29:75) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[76:84) Identifier |existing|
//@[85:86) Assignment |=|
//@[87:88) LeftBrace |{|
//@[88:90) NewLine |\r\n|
  name: 'existingResProperty'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:29) StringComplete |'existingResProperty'|
//@[29:31) NewLine |\r\n|
  location: 'westeurope'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:24) StringComplete |'westeurope'|
//@[24:26) NewLine |\r\n|
  properties: {}
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) RightBrace |}|
//@[16:18) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidExistingLocationRef 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |invalidExistingLocationRef|
//@[36:93) StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[94:95) Assignment |=|
//@[96:97) LeftBrace |{|
//@[97:99) NewLine |\r\n|
    parent: existingResProperty
//@[4:10) Identifier |parent|
//@[10:11) Colon |:|
//@[12:31) Identifier |existingResProperty|
//@[31:33) NewLine |\r\n|
    name: 'myExt'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:17) StringComplete |'myExt'|
//@[17:19) NewLine |\r\n|
    location: existingResProperty.location
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:33) Identifier |existingResProperty|
//@[33:34) Dot |.|
//@[34:42) Identifier |location|
//@[42:44) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource anyTypeInDependsOn 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |anyTypeInDependsOn|
//@[28:67) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[68:69) Assignment |=|
//@[70:71) LeftBrace |{|
//@[71:73) NewLine |\r\n|
  name: 'anyTypeInDependsOn'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:28) StringComplete |'anyTypeInDependsOn'|
//@[28:30) NewLine |\r\n|
  location: resourceGroup().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:38) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)
//@[4:7) Identifier |any|
//@[7:8) LeftParen |(|
//@[8:34) Identifier |invalidExistingLocationRef|
//@[34:35) Dot |.|
//@[35:45) Identifier |properties|
//@[45:46) Dot |.|
//@[46:69) Identifier |autoUpgradeMinorVersion|
//@[69:70) RightParen |)|
//@[70:72) NewLine |\r\n|
    's'
//@[4:7) StringComplete |'s'|
//@[7:9) NewLine |\r\n|
    any(true)
//@[4:7) Identifier |any|
//@[7:8) LeftParen |(|
//@[8:12) TrueKeyword |true|
//@[12:13) RightParen |)|
//@[13:15) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource anyTypeInParent 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
//@[0:8) Identifier |resource|
//@[9:24) Identifier |anyTypeInParent|
//@[25:70) StringComplete |'Microsoft.Network/dnsZones/CNAME@2018-05-01'|
//@[71:72) Assignment |=|
//@[73:74) LeftBrace |{|
//@[74:76) NewLine |\r\n|
  parent: any(true)
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:13) Identifier |any|
//@[13:14) LeftParen |(|
//@[14:18) TrueKeyword |true|
//@[18:19) RightParen |)|
//@[19:21) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource anyTypeInParentLoop 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |anyTypeInParentLoop|
//@[29:74) StringComplete |'Microsoft.Network/dnsZones/CNAME@2018-05-01'|
//@[75:76) Assignment |=|
//@[77:78) LeftSquare |[|
//@[78:81) Identifier |for|
//@[82:87) Identifier |thing|
//@[88:90) Identifier |in|
//@[91:92) LeftSquare |[|
//@[92:93) RightSquare |]|
//@[93:94) Colon |:|
//@[95:96) LeftBrace |{|
//@[96:98) NewLine |\r\n|
  parent: any(true)
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:13) Identifier |any|
//@[13:14) LeftParen |(|
//@[14:18) TrueKeyword |true|
//@[18:19) RightParen |)|
//@[19:21) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource anyTypeInScope 'Microsoft.Authorization/locks@2016-09-01' = {
//@[0:8) Identifier |resource|
//@[9:23) Identifier |anyTypeInScope|
//@[24:66) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[67:68) Assignment |=|
//@[69:70) LeftBrace |{|
//@[70:72) NewLine |\r\n|
  scope: any(invalidExistingLocationRef)
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:12) Identifier |any|
//@[12:13) LeftParen |(|
//@[13:39) Identifier |invalidExistingLocationRef|
//@[39:40) RightParen |)|
//@[40:42) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource anyTypeInScopeConditional 'Microsoft.Authorization/locks@2016-09-01' = if(true) {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |anyTypeInScopeConditional|
//@[35:77) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[78:79) Assignment |=|
//@[80:82) Identifier |if|
//@[82:83) LeftParen |(|
//@[83:87) TrueKeyword |true|
//@[87:88) RightParen |)|
//@[89:90) LeftBrace |{|
//@[90:92) NewLine |\r\n|
  scope: any(invalidExistingLocationRef)
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:12) Identifier |any|
//@[12:13) LeftParen |(|
//@[13:39) Identifier |invalidExistingLocationRef|
//@[39:40) RightParen |)|
//@[40:42) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource anyTypeInExistingScope 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = {
//@[0:8) Identifier |resource|
//@[9:31) Identifier |anyTypeInExistingScope|
//@[32:76) StringComplete |'Microsoft.Network/dnsZones/AAAA@2018-05-01'|
//@[77:85) Identifier |existing|
//@[86:87) Assignment |=|
//@[88:89) LeftBrace |{|
//@[89:91) NewLine |\r\n|
  parent: any('')
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:13) Identifier |any|
//@[13:14) LeftParen |(|
//@[14:16) StringComplete |''|
//@[16:17) RightParen |)|
//@[17:19) NewLine |\r\n|
  scope: any(false)
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:12) Identifier |any|
//@[12:13) LeftParen |(|
//@[13:18) FalseKeyword |false|
//@[18:19) RightParen |)|
//@[19:21) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource anyTypeInExistingScopeLoop 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = [for thing in []: {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |anyTypeInExistingScopeLoop|
//@[36:80) StringComplete |'Microsoft.Network/dnsZones/AAAA@2018-05-01'|
//@[81:89) Identifier |existing|
//@[90:91) Assignment |=|
//@[92:93) LeftSquare |[|
//@[93:96) Identifier |for|
//@[97:102) Identifier |thing|
//@[103:105) Identifier |in|
//@[106:107) LeftSquare |[|
//@[107:108) RightSquare |]|
//@[108:109) Colon |:|
//@[110:111) LeftBrace |{|
//@[111:113) NewLine |\r\n|
  parent: any('')
//@[2:8) Identifier |parent|
//@[8:9) Colon |:|
//@[10:13) Identifier |any|
//@[13:14) LeftParen |(|
//@[14:16) StringComplete |''|
//@[16:17) RightParen |)|
//@[17:19) NewLine |\r\n|
  scope: any(false)
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:12) Identifier |any|
//@[12:13) LeftParen |(|
//@[13:18) FalseKeyword |false|
//@[18:19) RightParen |)|
//@[19:21) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource tenantLevelResourceBlocked 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[0:8) Identifier |resource|
//@[9:35) Identifier |tenantLevelResourceBlocked|
//@[36:86) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftBrace |{|
//@[90:92) NewLine |\r\n|
  name: 'tenantLevelResourceBlocked'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:36) StringComplete |'tenantLevelResourceBlocked'|
//@[36:38) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// #completionTest(15,36,37) -> resourceTypes
//@[45:47) NewLine |\r\n|
resource comp1 'Microsoft.Resources/'
//@[0:8) Identifier |resource|
//@[9:14) Identifier |comp1|
//@[15:37) StringComplete |'Microsoft.Resources/'|
//@[37:41) NewLine |\r\n\r\n|

// #completionTest(15,16,17) -> resourceTypes
//@[45:47) NewLine |\r\n|
resource comp2 ''
//@[0:8) Identifier |resource|
//@[9:14) Identifier |comp2|
//@[15:17) StringComplete |''|
//@[17:21) NewLine |\r\n\r\n|

// #completionTest(38) -> resourceTypes
//@[39:41) NewLine |\r\n|
resource comp3 'Microsoft.Resources/t'
//@[0:8) Identifier |resource|
//@[9:14) Identifier |comp3|
//@[15:38) StringComplete |'Microsoft.Resources/t'|
//@[38:42) NewLine |\r\n\r\n|

// #completionTest(40) -> resourceTypes
//@[39:41) NewLine |\r\n|
resource comp4 'Microsoft.Resources/t/v'
//@[0:8) Identifier |resource|
//@[9:14) Identifier |comp4|
//@[15:40) StringComplete |'Microsoft.Resources/t/v'|
//@[40:44) NewLine |\r\n\r\n|

// #completionTest(49) -> resourceTypes
//@[39:41) NewLine |\r\n|
resource comp5 'Microsoft.Storage/storageAccounts'
//@[0:8) Identifier |resource|
//@[9:14) Identifier |comp5|
//@[15:50) StringComplete |'Microsoft.Storage/storageAccounts'|
//@[50:54) NewLine |\r\n\r\n|

// #completionTest(50) -> storageAccountsResourceTypes
//@[54:56) NewLine |\r\n|
resource comp6 'Microsoft.Storage/storageAccounts@'
//@[0:8) Identifier |resource|
//@[9:14) Identifier |comp6|
//@[15:51) StringComplete |'Microsoft.Storage/storageAccounts@'|
//@[51:55) NewLine |\r\n\r\n|

// #completionTest(52) -> templateSpecsResourceTypes
//@[52:54) NewLine |\r\n|
resource comp7 'Microsoft.Resources/templateSpecs@20'
//@[0:8) Identifier |resource|
//@[9:14) Identifier |comp7|
//@[15:53) StringComplete |'Microsoft.Resources/templateSpecs@20'|
//@[53:57) NewLine |\r\n\r\n|

// #completionTest(60,61) -> virtualNetworksResourceTypes
//@[57:59) NewLine |\r\n|
resource comp8 'Microsoft.Network/virtualNetworks@2020-06-01'
//@[0:8) Identifier |resource|
//@[9:14) Identifier |comp8|
//@[15:61) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[61:67) NewLine |\r\n\r\n\r\n|


// issue #3000
//@[14:16) NewLine |\r\n|
resource issue3000LogicApp1 'Microsoft.Logic/workflows@2019-05-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |issue3000LogicApp1|
//@[28:66) StringComplete |'Microsoft.Logic/workflows@2019-05-01'|
//@[67:68) Assignment |=|
//@[69:70) LeftBrace |{|
//@[70:72) NewLine |\r\n|
  name: 'issue3000LogicApp1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:28) StringComplete |'issue3000LogicApp1'|
//@[28:30) NewLine |\r\n|
  location: resourceGroup().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:38) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    state: 'Enabled'
//@[4:9) Identifier |state|
//@[9:10) Colon |:|
//@[11:20) StringComplete |'Enabled'|
//@[20:22) NewLine |\r\n|
    definition: ''
//@[4:14) Identifier |definition|
//@[14:15) Colon |:|
//@[16:18) StringComplete |''|
//@[18:20) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  identity: {
//@[2:10) Identifier |identity|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    type: 'SystemAssigned'
//@[4:8) Identifier |type|
//@[8:9) Colon |:|
//@[10:26) StringComplete |'SystemAssigned'|
//@[26:28) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  extendedLocation: {}
//@[2:18) Identifier |extendedLocation|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:22) RightBrace |}|
//@[22:24) NewLine |\r\n|
  sku: {}
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) RightBrace |}|
//@[9:11) NewLine |\r\n|
  kind: 'V1'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:12) StringComplete |'V1'|
//@[12:14) NewLine |\r\n|
  managedBy: 'string'
//@[2:11) Identifier |managedBy|
//@[11:12) Colon |:|
//@[13:21) StringComplete |'string'|
//@[21:23) NewLine |\r\n|
  mangedByExtended: [
//@[2:18) Identifier |mangedByExtended|
//@[18:19) Colon |:|
//@[20:21) LeftSquare |[|
//@[21:23) NewLine |\r\n|
   'str1'
//@[3:9) StringComplete |'str1'|
//@[9:11) NewLine |\r\n|
   'str2'
//@[3:9) StringComplete |'str2'|
//@[9:11) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
  zones: [
//@[2:7) Identifier |zones|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
   'str1'
//@[3:9) StringComplete |'str1'|
//@[9:11) NewLine |\r\n|
   'str2'
//@[3:9) StringComplete |'str2'|
//@[9:11) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
  plan: {}
//@[2:6) Identifier |plan|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:10) RightBrace |}|
//@[10:12) NewLine |\r\n|
  eTag: ''
//@[2:6) Identifier |eTag|
//@[6:7) Colon |:|
//@[8:10) StringComplete |''|
//@[10:12) NewLine |\r\n|
  scale: {}  
//@[2:7) Identifier |scale|
//@[7:8) Colon |:|
//@[9:10) LeftBrace |{|
//@[10:11) RightBrace |}|
//@[13:15) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource issue3000LogicApp2 'Microsoft.Logic/workflows@2019-05-01' = {
//@[0:8) Identifier |resource|
//@[9:27) Identifier |issue3000LogicApp2|
//@[28:66) StringComplete |'Microsoft.Logic/workflows@2019-05-01'|
//@[67:68) Assignment |=|
//@[69:70) LeftBrace |{|
//@[70:72) NewLine |\r\n|
  name: 'issue3000LogicApp2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:28) StringComplete |'issue3000LogicApp2'|
//@[28:30) NewLine |\r\n|
  location: resourceGroup().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:38) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    state: 'Enabled'
//@[4:9) Identifier |state|
//@[9:10) Colon |:|
//@[11:20) StringComplete |'Enabled'|
//@[20:22) NewLine |\r\n|
    definition: ''
//@[4:14) Identifier |definition|
//@[14:15) Colon |:|
//@[16:18) StringComplete |''|
//@[18:20) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  identity: 'SystemAssigned'
//@[2:10) Identifier |identity|
//@[10:11) Colon |:|
//@[12:28) StringComplete |'SystemAssigned'|
//@[28:30) NewLine |\r\n|
  extendedLocation: 'eastus'
//@[2:18) Identifier |extendedLocation|
//@[18:19) Colon |:|
//@[20:28) StringComplete |'eastus'|
//@[28:30) NewLine |\r\n|
  sku: 'Basic'
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:14) StringComplete |'Basic'|
//@[14:16) NewLine |\r\n|
  kind: {
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:11) NewLine |\r\n|
    name: 'V1'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:14) StringComplete |'V1'|
//@[14:16) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  managedBy: {}
//@[2:11) Identifier |managedBy|
//@[11:12) Colon |:|
//@[13:14) LeftBrace |{|
//@[14:15) RightBrace |}|
//@[15:17) NewLine |\r\n|
  mangedByExtended: [
//@[2:18) Identifier |mangedByExtended|
//@[18:19) Colon |:|
//@[20:21) LeftSquare |[|
//@[21:23) NewLine |\r\n|
   {}
//@[3:4) LeftBrace |{|
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
   {}
//@[3:4) LeftBrace |{|
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
  zones: [
//@[2:7) Identifier |zones|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
   {}
//@[3:4) LeftBrace |{|
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
   {}
//@[3:4) LeftBrace |{|
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
  plan: ''
//@[2:6) Identifier |plan|
//@[6:7) Colon |:|
//@[8:10) StringComplete |''|
//@[10:12) NewLine |\r\n|
  eTag: {}
//@[2:6) Identifier |eTag|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:10) RightBrace |}|
//@[10:12) NewLine |\r\n|
  scale: [
//@[2:7) Identifier |scale|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
  {}
//@[2:3) LeftBrace |{|
//@[3:4) RightBrace |}|
//@[4:6) NewLine |\r\n|
  ]  
//@[2:3) RightSquare |]|
//@[5:7) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource issue3000stg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
//@[0:8) Identifier |resource|
//@[9:21) Identifier |issue3000stg|
//@[22:68) StringComplete |'Microsoft.Storage/storageAccounts@2021-04-01'|
//@[69:70) Assignment |=|
//@[71:72) LeftBrace |{|
//@[72:74) NewLine |\r\n|
  name: 'issue3000stg'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'issue3000stg'|
//@[22:24) NewLine |\r\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:21) NewLine |\r\n|
  location: 'West US'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:21) StringComplete |'West US'|
//@[21:23) NewLine |\r\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:10) NewLine |\r\n|
    name: 'Premium_LRS'    
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:23) StringComplete |'Premium_LRS'|
//@[27:29) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  madeUpProperty: {}
//@[2:16) Identifier |madeUpProperty|
//@[16:17) Colon |:|
//@[18:19) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:22) NewLine |\r\n|
  managedByExtended: []
//@[2:19) Identifier |managedByExtended|
//@[19:20) Colon |:|
//@[21:22) LeftSquare |[|
//@[22:23) RightSquare |]|
//@[23:25) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var issue3000stgMadeUpProperty = issue3000stg.madeUpProperty
//@[0:3) Identifier |var|
//@[4:30) Identifier |issue3000stgMadeUpProperty|
//@[31:32) Assignment |=|
//@[33:45) Identifier |issue3000stg|
//@[45:46) Dot |.|
//@[46:60) Identifier |madeUpProperty|
//@[60:62) NewLine |\r\n|
var issue3000stgManagedBy = issue3000stg.managedBy
//@[0:3) Identifier |var|
//@[4:25) Identifier |issue3000stgManagedBy|
//@[26:27) Assignment |=|
//@[28:40) Identifier |issue3000stg|
//@[40:41) Dot |.|
//@[41:50) Identifier |managedBy|
//@[50:52) NewLine |\r\n|
var issue3000stgManagedByExtended = issue3000stg.managedByExtended
//@[0:3) Identifier |var|
//@[4:33) Identifier |issue3000stgManagedByExtended|
//@[34:35) Assignment |=|
//@[36:48) Identifier |issue3000stg|
//@[48:49) Dot |.|
//@[49:66) Identifier |managedByExtended|
//@[66:70) NewLine |\r\n\r\n|

param dataCollectionRule object
//@[0:5) Identifier |param|
//@[6:24) Identifier |dataCollectionRule|
//@[25:31) Identifier |object|
//@[31:33) NewLine |\r\n|
param tags object
//@[0:5) Identifier |param|
//@[6:10) Identifier |tags|
//@[11:17) Identifier |object|
//@[17:21) NewLine |\r\n\r\n|

var defaultLogAnalyticsWorkspace = {
//@[0:3) Identifier |var|
//@[4:32) Identifier |defaultLogAnalyticsWorkspace|
//@[33:34) Assignment |=|
//@[35:36) LeftBrace |{|
//@[36:38) NewLine |\r\n|
  subscriptionId: subscription().subscriptionId
//@[2:16) Identifier |subscriptionId|
//@[16:17) Colon |:|
//@[18:30) Identifier |subscription|
//@[30:31) LeftParen |(|
//@[31:32) RightParen |)|
//@[32:33) Dot |.|
//@[33:47) Identifier |subscriptionId|
//@[47:49) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource logAnalyticsWorkspaces 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = [for logAnalyticsWorkspace in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[0:8) Identifier |resource|
//@[9:31) Identifier |logAnalyticsWorkspaces|
//@[32:85) StringComplete |'Microsoft.OperationalInsights/workspaces@2020-10-01'|
//@[86:94) Identifier |existing|
//@[95:96) Assignment |=|
//@[97:98) LeftSquare |[|
//@[98:101) Identifier |for|
//@[102:123) Identifier |logAnalyticsWorkspace|
//@[124:126) Identifier |in|
//@[127:145) Identifier |dataCollectionRule|
//@[145:146) Dot |.|
//@[146:158) Identifier |destinations|
//@[158:159) Dot |.|
//@[159:181) Identifier |logAnalyticsWorkspaces|
//@[181:182) Colon |:|
//@[183:184) LeftBrace |{|
//@[184:186) NewLine |\r\n|
  name: logAnalyticsWorkspace.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:29) Identifier |logAnalyticsWorkspace|
//@[29:30) Dot |.|
//@[30:34) Identifier |name|
//@[34:36) NewLine |\r\n|
  scope: resourceGroup( union( defaultLogAnalyticsWorkspace, logAnalyticsWorkspace ).subscriptionId, logAnalyticsWorkspace.resourceGroup )
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[24:29) Identifier |union|
//@[29:30) LeftParen |(|
//@[31:59) Identifier |defaultLogAnalyticsWorkspace|
//@[59:60) Comma |,|
//@[61:82) Identifier |logAnalyticsWorkspace|
//@[83:84) RightParen |)|
//@[84:85) Dot |.|
//@[85:99) Identifier |subscriptionId|
//@[99:100) Comma |,|
//@[101:122) Identifier |logAnalyticsWorkspace|
//@[122:123) Dot |.|
//@[123:136) Identifier |resourceGroup|
//@[137:138) RightParen |)|
//@[138:140) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource dataCollectionRuleRes 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[0:8) Identifier |resource|
//@[9:30) Identifier |dataCollectionRuleRes|
//@[31:82) StringComplete |'Microsoft.Insights/dataCollectionRules@2021-04-01'|
//@[83:84) Assignment |=|
//@[85:86) LeftBrace |{|
//@[86:88) NewLine |\r\n|
  name: dataCollectionRule.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:26) Identifier |dataCollectionRule|
//@[26:27) Dot |.|
//@[27:31) Identifier |name|
//@[31:33) NewLine |\r\n|
  location: dataCollectionRule.location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:30) Identifier |dataCollectionRule|
//@[30:31) Dot |.|
//@[31:39) Identifier |location|
//@[39:41) NewLine |\r\n|
  tags: tags
//@[2:6) Identifier |tags|
//@[6:7) Colon |:|
//@[8:12) Identifier |tags|
//@[12:14) NewLine |\r\n|
  kind: dataCollectionRule.kind
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:26) Identifier |dataCollectionRule|
//@[26:27) Dot |.|
//@[27:31) Identifier |kind|
//@[31:33) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    description: dataCollectionRule.description
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:35) Identifier |dataCollectionRule|
//@[35:36) Dot |.|
//@[36:47) Identifier |description|
//@[47:49) NewLine |\r\n|
    destinations: union(empty(dataCollectionRule.destinations.azureMonitorMetrics.name) ? {} : {
//@[4:16) Identifier |destinations|
//@[16:17) Colon |:|
//@[18:23) Identifier |union|
//@[23:24) LeftParen |(|
//@[24:29) Identifier |empty|
//@[29:30) LeftParen |(|
//@[30:48) Identifier |dataCollectionRule|
//@[48:49) Dot |.|
//@[49:61) Identifier |destinations|
//@[61:62) Dot |.|
//@[62:81) Identifier |azureMonitorMetrics|
//@[81:82) Dot |.|
//@[82:86) Identifier |name|
//@[86:87) RightParen |)|
//@[88:89) Question |?|
//@[90:91) LeftBrace |{|
//@[91:92) RightBrace |}|
//@[93:94) Colon |:|
//@[95:96) LeftBrace |{|
//@[96:98) NewLine |\r\n|
      azureMonitorMetrics: {
//@[6:25) Identifier |azureMonitorMetrics|
//@[25:26) Colon |:|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\r\n|
        name: dataCollectionRule.destinations.azureMonitorMetrics.name
//@[8:12) Identifier |name|
//@[12:13) Colon |:|
//@[14:32) Identifier |dataCollectionRule|
//@[32:33) Dot |.|
//@[33:45) Identifier |destinations|
//@[45:46) Dot |.|
//@[46:65) Identifier |azureMonitorMetrics|
//@[65:66) Dot |.|
//@[66:70) Identifier |name|
//@[70:72) NewLine |\r\n|
      }
//@[6:7) RightBrace |}|
//@[7:9) NewLine |\r\n|
    },{
//@[4:5) RightBrace |}|
//@[5:6) Comma |,|
//@[6:7) LeftBrace |{|
//@[7:9) NewLine |\r\n|
      logAnalytics: [for (logAnalyticsWorkspace, i) in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[6:18) Identifier |logAnalytics|
//@[18:19) Colon |:|
//@[20:21) LeftSquare |[|
//@[21:24) Identifier |for|
//@[25:26) LeftParen |(|
//@[26:47) Identifier |logAnalyticsWorkspace|
//@[47:48) Comma |,|
//@[49:50) Identifier |i|
//@[50:51) RightParen |)|
//@[52:54) Identifier |in|
//@[55:73) Identifier |dataCollectionRule|
//@[73:74) Dot |.|
//@[74:86) Identifier |destinations|
//@[86:87) Dot |.|
//@[87:109) Identifier |logAnalyticsWorkspaces|
//@[109:110) Colon |:|
//@[111:112) LeftBrace |{|
//@[112:114) NewLine |\r\n|
        name: logAnalyticsWorkspace.destinationName
//@[8:12) Identifier |name|
//@[12:13) Colon |:|
//@[14:35) Identifier |logAnalyticsWorkspace|
//@[35:36) Dot |.|
//@[36:51) Identifier |destinationName|
//@[51:53) NewLine |\r\n|
        workspaceResourceId: logAnalyticsWorkspaces[i].id
//@[8:27) Identifier |workspaceResourceId|
//@[27:28) Colon |:|
//@[29:51) Identifier |logAnalyticsWorkspaces|
//@[51:52) LeftSquare |[|
//@[52:53) Identifier |i|
//@[53:54) RightSquare |]|
//@[54:55) Dot |.|
//@[55:57) Identifier |id|
//@[57:59) NewLine |\r\n|
      }]
//@[6:7) RightBrace |}|
//@[7:8) RightSquare |]|
//@[8:10) NewLine |\r\n|
    })
//@[4:5) RightBrace |}|
//@[5:6) RightParen |)|
//@[6:8) NewLine |\r\n|
    dataSources: dataCollectionRule.dataSources
//@[4:15) Identifier |dataSources|
//@[15:16) Colon |:|
//@[17:35) Identifier |dataCollectionRule|
//@[35:36) Dot |.|
//@[36:47) Identifier |dataSources|
//@[47:49) NewLine |\r\n|
    dataFlows: dataCollectionRule.dataFlows
//@[4:13) Identifier |dataFlows|
//@[13:14) Colon |:|
//@[15:33) Identifier |dataCollectionRule|
//@[33:34) Dot |.|
//@[34:43) Identifier |dataFlows|
//@[43:45) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource dataCollectionRuleRes2 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[0:8) Identifier |resource|
//@[9:31) Identifier |dataCollectionRuleRes2|
//@[32:83) StringComplete |'Microsoft.Insights/dataCollectionRules@2021-04-01'|
//@[84:85) Assignment |=|
//@[86:87) LeftBrace |{|
//@[87:89) NewLine |\r\n|
  name: dataCollectionRule.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:26) Identifier |dataCollectionRule|
//@[26:27) Dot |.|
//@[27:31) Identifier |name|
//@[31:33) NewLine |\r\n|
  location: dataCollectionRule.location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:30) Identifier |dataCollectionRule|
//@[30:31) Dot |.|
//@[31:39) Identifier |location|
//@[39:41) NewLine |\r\n|
  tags: tags
//@[2:6) Identifier |tags|
//@[6:7) Colon |:|
//@[8:12) Identifier |tags|
//@[12:14) NewLine |\r\n|
  kind: dataCollectionRule.kind
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:26) Identifier |dataCollectionRule|
//@[26:27) Dot |.|
//@[27:31) Identifier |kind|
//@[31:33) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    description: dataCollectionRule.description
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:35) Identifier |dataCollectionRule|
//@[35:36) Dot |.|
//@[36:47) Identifier |description|
//@[47:49) NewLine |\r\n|
    destinations: empty([]) ? [for x in []: {}] : [for x in []: {}]
//@[4:16) Identifier |destinations|
//@[16:17) Colon |:|
//@[18:23) Identifier |empty|
//@[23:24) LeftParen |(|
//@[24:25) LeftSquare |[|
//@[25:26) RightSquare |]|
//@[26:27) RightParen |)|
//@[28:29) Question |?|
//@[30:31) LeftSquare |[|
//@[31:34) Identifier |for|
//@[35:36) Identifier |x|
//@[37:39) Identifier |in|
//@[40:41) LeftSquare |[|
//@[41:42) RightSquare |]|
//@[42:43) Colon |:|
//@[44:45) LeftBrace |{|
//@[45:46) RightBrace |}|
//@[46:47) RightSquare |]|
//@[48:49) Colon |:|
//@[50:51) LeftSquare |[|
//@[51:54) Identifier |for|
//@[55:56) Identifier |x|
//@[57:59) Identifier |in|
//@[60:61) LeftSquare |[|
//@[61:62) RightSquare |]|
//@[62:63) Colon |:|
//@[64:65) LeftBrace |{|
//@[65:66) RightBrace |}|
//@[66:67) RightSquare |]|
//@[67:69) NewLine |\r\n|
    dataSources: dataCollectionRule.dataSources
//@[4:15) Identifier |dataSources|
//@[15:16) Colon |:|
//@[17:35) Identifier |dataCollectionRule|
//@[35:36) Dot |.|
//@[36:47) Identifier |dataSources|
//@[47:49) NewLine |\r\n|
    dataFlows: dataCollectionRule.dataFlows
//@[4:13) Identifier |dataFlows|
//@[13:14) Colon |:|
//@[15:33) Identifier |dataCollectionRule|
//@[33:34) Dot |.|
//@[34:43) Identifier |dataFlows|
//@[43:45) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|

//@[0:0) EndOfFile ||
