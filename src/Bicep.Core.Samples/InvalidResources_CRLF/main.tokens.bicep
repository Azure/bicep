
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
//@[1:5) NewLine |\r\n\r\n|

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
//@[1:1) EndOfFile ||
