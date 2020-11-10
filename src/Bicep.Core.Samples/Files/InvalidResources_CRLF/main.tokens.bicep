
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
//@[80:80) EndOfFile ||
