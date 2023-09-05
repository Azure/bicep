
//@[000:002) NewLine |\r\n|
// wrong declaration
//@[020:022) NewLine |\r\n|
bad
//@[000:003) Identifier |bad|
//@[003:007) NewLine |\r\n\r\n|

// incomplete #completionTest(9) -> empty
//@[041:043) NewLine |\r\n|
resource 
//@[000:008) Identifier |resource|
//@[009:011) NewLine |\r\n|
resource foo
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[012:014) NewLine |\r\n|
resource fo/o
//@[000:008) Identifier |resource|
//@[009:011) Identifier |fo|
//@[011:012) Slash |/|
//@[012:013) Identifier |o|
//@[013:015) NewLine |\r\n|
resource foo 'ddd'
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:018) StringComplete |'ddd'|
//@[018:022) NewLine |\r\n\r\n|

// #completionTest(23) -> resourceTypes
//@[039:041) NewLine |\r\n|
resource trailingSpace  
//@[000:008) Identifier |resource|
//@[009:022) Identifier |trailingSpace|
//@[024:028) NewLine |\r\n\r\n|

// #completionTest(19,20) -> resourceObject
//@[043:045) NewLine |\r\n|
resource foo 'ddd'= 
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:018) StringComplete |'ddd'|
//@[018:019) Assignment |=|
//@[020:024) NewLine |\r\n\r\n|

// wrong resource type
//@[022:024) NewLine |\r\n|
resource foo 'ddd'={
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:018) StringComplete |'ddd'|
//@[018:019) Assignment |=|
//@[019:020) LeftBrace |{|
//@[020:022) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource foo 'ddd'=if (1 + 1 == 2) {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:018) StringComplete |'ddd'|
//@[018:019) Assignment |=|
//@[019:021) Identifier |if|
//@[022:023) LeftParen |(|
//@[023:024) Integer |1|
//@[025:026) Plus |+|
//@[027:028) Integer |1|
//@[029:031) Equals |==|
//@[032:033) Integer |2|
//@[033:034) RightParen |)|
//@[035:036) LeftBrace |{|
//@[036:038) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// using string interpolation for the resource type
//@[051:053) NewLine |\r\n|
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:026) StringLeftPiece |'Microsoft.${|
//@[026:034) Identifier |provider|
//@[034:058) StringRightPiece |}/foos@2020-02-02-alpha'|
//@[058:059) Assignment |=|
//@[060:061) LeftBrace |{|
//@[061:063) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:026) StringLeftPiece |'Microsoft.${|
//@[026:034) Identifier |provider|
//@[034:058) StringRightPiece |}/foos@2020-02-02-alpha'|
//@[058:059) Assignment |=|
//@[060:062) Identifier |if|
//@[063:064) LeftParen |(|
//@[064:068) TrueKeyword |true|
//@[068:069) RightParen |)|
//@[070:071) LeftBrace |{|
//@[071:073) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// missing required property
//@[028:030) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[051:052) LeftBrace |{|
//@[052:054) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[056:060) Identifier |name|
//@[061:063) Equals |==|
//@[064:071) StringComplete |'value'|
//@[071:072) RightParen |)|
//@[073:074) LeftBrace |{|
//@[074:076) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[056:057) LeftBrace |{|
//@[058:061) StringComplete |'a'|
//@[061:062) Colon |:|
//@[063:064) Identifier |b|
//@[065:066) RightBrace |}|
//@[066:067) Dot |.|
//@[067:068) Identifier |a|
//@[069:071) Equals |==|
//@[072:077) StringComplete |'foo'|
//@[077:078) RightParen |)|
//@[079:080) LeftBrace |{|
//@[080:082) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// simulate typing if condition
//@[031:033) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[054:058) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[056:060) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[056:060) TrueKeyword |true|
//@[060:064) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[056:060) TrueKeyword |true|
//@[060:061) RightParen |)|
//@[061:065) NewLine |\r\n\r\n|

// missing condition
//@[020:022) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftBrace |{|
//@[056:058) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// empty condition
//@[018:020) NewLine |\r\n|
// #completionTest(56) -> symbols
//@[033:035) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[056:057) RightParen |)|
//@[058:059) LeftBrace |{|
//@[059:061) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// #completionTest(57, 59) -> symbols
//@[037:039) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[061:062) RightParen |)|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// invalid condition type
//@[025:027) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[056:059) Integer |123|
//@[059:060) RightParen |)|
//@[061:062) LeftBrace |{|
//@[062:064) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// runtime functions are no allowed in resource conditions
//@[058:060) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[051:052) Assignment |=|
//@[053:055) Identifier |if|
//@[056:057) LeftParen |(|
//@[057:066) Identifier |reference|
//@[066:067) LeftParen |(|
//@[067:109) StringComplete |'Micorosft.Management/managementGroups/MG'|
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
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[051:052) Assignment |=|
//@[053:055) Identifier |if|
//@[056:057) LeftParen |(|
//@[057:065) Identifier |listKeys|
//@[065:066) LeftParen |(|
//@[066:071) StringComplete |'foo'|
//@[071:072) Comma |,|
//@[073:085) StringComplete |'2020-05-01'|
//@[085:086) RightParen |)|
//@[086:087) Dot |.|
//@[087:090) Identifier |bar|
//@[091:093) Equals |==|
//@[094:098) TrueKeyword |true|
//@[098:099) RightParen |)|
//@[100:101) LeftBrace |{|
//@[101:103) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// duplicate property at the top level
//@[038:040) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[053:055) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
  name: true
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) TrueKeyword |true|
//@[012:014) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// duplicate property at the top level with string literal syntax
//@[065:067) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[053:055) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
  'name': true
//@[002:008) StringComplete |'name'|
//@[008:009) Colon |:|
//@[010:014) TrueKeyword |true|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// duplicate property inside
//@[028:030) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[053:055) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    foo: 'a'
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:012) StringComplete |'a'|
//@[012:014) NewLine |\r\n|
    foo: 'a'
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:012) StringComplete |'a'|
//@[012:014) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// duplicate property inside with string literal syntax
//@[055:057) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[053:055) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    foo: 'a'
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:012) StringComplete |'a'|
//@[012:014) NewLine |\r\n|
    'foo': 'a'
//@[004:009) StringComplete |'foo'|
//@[009:010) Colon |:|
//@[011:014) StringComplete |'a'|
//@[014:016) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// wrong property types
//@[023:025) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[053:055) NewLine |\r\n|
  name: 'foo'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'foo'|
//@[013:015) NewLine |\r\n|
  location: [
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:013) LeftSquare |[|
//@[013:015) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
  tags: 'tag are not a string?'
//@[002:006) Identifier |tags|
//@[006:007) Colon |:|
//@[008:031) StringComplete |'tag are not a string?'|
//@[031:033) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |bar|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[051:052) Assignment |=|
//@[053:054) LeftBrace |{|
//@[054:056) NewLine |\r\n|
  name: true ? 's' : 'a' + 1
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) TrueKeyword |true|
//@[013:014) Question |?|
//@[015:018) StringComplete |'s'|
//@[019:020) Colon |:|
//@[021:024) StringComplete |'a'|
//@[025:026) Plus |+|
//@[027:028) Integer |1|
//@[028:030) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    x: foo()
//@[004:005) Identifier |x|
//@[005:006) Colon |:|
//@[007:010) Identifier |foo|
//@[010:011) LeftParen |(|
//@[011:012) RightParen |)|
//@[012:014) NewLine |\r\n|
    y: true && (null || !4)
//@[004:005) Identifier |y|
//@[005:006) Colon |:|
//@[007:011) TrueKeyword |true|
//@[012:014) LogicalAnd |&&|
//@[015:016) LeftParen |(|
//@[016:020) NullKeyword |null|
//@[021:023) LogicalOr ||||
//@[024:025) Exclamation |!|
//@[025:026) Integer |4|
//@[026:027) RightParen |)|
//@[027:029) NewLine |\r\n|
    a: [
//@[004:005) Identifier |a|
//@[005:006) Colon |:|
//@[007:008) LeftSquare |[|
//@[008:010) NewLine |\r\n|
      a
//@[006:007) Identifier |a|
//@[007:009) NewLine |\r\n|
      !null
//@[006:007) Exclamation |!|
//@[007:011) NullKeyword |null|
//@[011:013) NewLine |\r\n|
      true && true || true + -true * 4
//@[006:010) TrueKeyword |true|
//@[011:013) LogicalAnd |&&|
//@[014:018) TrueKeyword |true|
//@[019:021) LogicalOr ||||
//@[022:026) TrueKeyword |true|
//@[027:028) Plus |+|
//@[029:030) Minus |-|
//@[030:034) TrueKeyword |true|
//@[035:036) Asterisk |*|
//@[037:038) Integer |4|
//@[038:040) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// there should be no completions without the colon
//@[051:053) NewLine |\r\n|
resource noCompletionsWithoutColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |noCompletionsWithoutColon|
//@[035:085) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  // #completionTest(7,8) -> empty
//@[034:036) NewLine |\r\n|
  kind  
//@[002:006) Identifier |kind|
//@[008:010) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource noCompletionsBeforeColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:033) Identifier |noCompletionsBeforeColon|
//@[034:084) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[085:086) Assignment |=|
//@[087:088) LeftBrace |{|
//@[088:090) NewLine |\r\n|
  // #completionTest(7,8) -> empty
//@[034:036) NewLine |\r\n|
  kind  :
//@[002:006) Identifier |kind|
//@[008:009) Colon |:|
//@[009:011) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// unsupported resource ref
//@[027:029) NewLine |\r\n|
var resrefvar = bar.name
//@[000:003) Identifier |var|
//@[004:013) Identifier |resrefvar|
//@[014:015) Assignment |=|
//@[016:019) Identifier |bar|
//@[019:020) Dot |.|
//@[020:024) Identifier |name|
//@[024:028) NewLine |\r\n\r\n|

param resrefpar string = foo.id
//@[000:005) Identifier |param|
//@[006:015) Identifier |resrefpar|
//@[016:022) Identifier |string|
//@[023:024) Assignment |=|
//@[025:028) Identifier |foo|
//@[028:029) Dot |.|
//@[029:031) Identifier |id|
//@[031:035) NewLine |\r\n\r\n|

output resrefout bool = bar.id
//@[000:006) Identifier |output|
//@[007:016) Identifier |resrefout|
//@[017:021) Identifier |bool|
//@[022:023) Assignment |=|
//@[024:027) Identifier |bar|
//@[027:028) Dot |.|
//@[028:030) Identifier |id|
//@[030:034) NewLine |\r\n\r\n|

// attempting to set read-only properties
//@[041:043) NewLine |\r\n|
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |baz|
//@[013:050) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[051:052) Assignment |=|
//@[053:054) LeftBrace |{|
//@[054:056) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  id: 2
//@[002:004) Identifier |id|
//@[004:005) Colon |:|
//@[006:007) Integer |2|
//@[007:009) NewLine |\r\n|
  type: 'hello'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:015) StringComplete |'hello'|
//@[015:017) NewLine |\r\n|
  apiVersion: true
//@[002:012) Identifier |apiVersion|
//@[012:013) Colon |:|
//@[014:018) TrueKeyword |true|
//@[018:020) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource readOnlyPropertyAssignment 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |readOnlyPropertyAssignment|
//@[036:082) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[086:088) NewLine |\r\n|
  name: 'vnet-bicep'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) StringComplete |'vnet-bicep'|
//@[020:022) NewLine |\r\n|
  location: 'westeurope'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:024) StringComplete |'westeurope'|
//@[024:026) NewLine |\r\n|
  etag: 'assigning-to-read-only-value'
//@[002:006) Identifier |etag|
//@[006:007) Colon |:|
//@[008:038) StringComplete |'assigning-to-read-only-value'|
//@[038:040) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    resourceGuid: 'assigning-to-read-only-value'
//@[004:016) Identifier |resourceGuid|
//@[016:017) Colon |:|
//@[018:048) StringComplete |'assigning-to-read-only-value'|
//@[048:050) NewLine |\r\n|
    addressSpace: {
//@[004:016) Identifier |addressSpace|
//@[016:017) Colon |:|
//@[018:019) LeftBrace |{|
//@[019:021) NewLine |\r\n|
      addressPrefixes: [
//@[006:021) Identifier |addressPrefixes|
//@[021:022) Colon |:|
//@[023:024) LeftSquare |[|
//@[024:026) NewLine |\r\n|
        '10.0.0.0/16'
//@[008:021) StringComplete |'10.0.0.0/16'|
//@[021:023) NewLine |\r\n|
      ]
//@[006:007) RightSquare |]|
//@[007:009) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
    subnets: []
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) RightSquare |]|
//@[015:017) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |badDepends|
//@[020:057) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[058:059) Assignment |=|
//@[060:061) LeftBrace |{|
//@[061:063) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    baz.id
//@[004:007) Identifier |baz|
//@[007:008) Dot |.|
//@[008:010) Identifier |id|
//@[010:012) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:008) Identifier |resource|
//@[009:020) Identifier |badDepends2|
//@[021:058) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[059:060) Assignment |=|
//@[061:062) LeftBrace |{|
//@[062:064) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    'hello'
//@[004:011) StringComplete |'hello'|
//@[011:013) NewLine |\r\n|
    true
//@[004:008) TrueKeyword |true|
//@[008:010) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:008) Identifier |resource|
//@[009:020) Identifier |badDepends3|
//@[021:058) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[059:060) Assignment |=|
//@[061:062) LeftBrace |{|
//@[062:064) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:008) Identifier |resource|
//@[009:020) Identifier |badDepends4|
//@[021:058) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[059:060) Assignment |=|
//@[061:062) LeftBrace |{|
//@[062:064) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    badDepends3
//@[004:015) Identifier |badDepends3|
//@[015:017) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:008) Identifier |resource|
//@[009:020) Identifier |badDepends5|
//@[021:058) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[059:060) Assignment |=|
//@[061:062) LeftBrace |{|
//@[062:064) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  dependsOn: badDepends3.dependsOn
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:024) Identifier |badDepends3|
//@[024:025) Dot |.|
//@[025:034) Identifier |dependsOn|
//@[034:036) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var interpVal = 'abc'
//@[000:003) Identifier |var|
//@[004:013) Identifier |interpVal|
//@[014:015) Assignment |=|
//@[016:021) StringComplete |'abc'|
//@[021:023) NewLine |\r\n|
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |badInterp|
//@[019:056) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[057:058) Assignment |=|
//@[059:060) LeftBrace |{|
//@[060:062) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
//@[002:005) StringLeftPiece |'${|
//@[005:014) Identifier |interpVal|
//@[014:016) StringRightPiece |}'|
//@[016:017) Colon |:|
//@[018:031) StringComplete |'unsupported'|
//@[094:096) NewLine |\r\n|
  '${undefinedSymbol}': true
//@[002:005) StringLeftPiece |'${|
//@[005:020) Identifier |undefinedSymbol|
//@[020:022) StringRightPiece |}'|
//@[022:023) Colon |:|
//@[024:028) TrueKeyword |true|
//@[028:030) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module validModule './module.bicep' = {
//@[000:006) Identifier |module|
//@[007:018) Identifier |validModule|
//@[019:035) StringComplete |'./module.bicep'|
//@[036:037) Assignment |=|
//@[038:039) LeftBrace |{|
//@[039:041) NewLine |\r\n|
  name: 'storageDeploy'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'storageDeploy'|
//@[023:025) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    name: 'contoso'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:019) StringComplete |'contoso'|
//@[019:021) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes1|
//@[026:072) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[073:074) Assignment |=|
//@[075:076) LeftBrace |{|
//@[076:078) NewLine |\r\n|
  name: 'name1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringComplete |'name1'|
//@[015:017) NewLine |\r\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    evictionPolicy: 'Deallocate'
//@[004:018) Identifier |evictionPolicy|
//@[018:019) Colon |:|
//@[020:032) StringComplete |'Deallocate'|
//@[032:034) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes2|
//@[026:076) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[077:078) Assignment |=|
//@[079:080) LeftBrace |{|
//@[080:082) NewLine |\r\n|
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:021) Identifier |concat|
//@[021:022) LeftParen |(|
//@[022:038) Identifier |runtimeValidRes1|
//@[038:039) Dot |.|
//@[039:041) Identifier |id|
//@[041:042) Comma |,|
//@[043:059) Identifier |runtimeValidRes1|
//@[059:060) Dot |.|
//@[060:064) Identifier |name|
//@[064:065) RightParen |)|
//@[065:066) Comma |,|
//@[067:083) Identifier |runtimeValidRes1|
//@[083:084) Dot |.|
//@[084:088) Identifier |type|
//@[088:089) RightParen |)|
//@[089:091) NewLine |\r\n|
  kind:'AzureCLI'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[007:017) StringComplete |'AzureCLI'|
//@[017:019) NewLine |\r\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    azCliVersion: '2.0'
//@[004:016) Identifier |azCliVersion|
//@[016:017) Colon |:|
//@[018:023) StringComplete |'2.0'|
//@[023:025) NewLine |\r\n|
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
//@[004:021) Identifier |retentionInterval|
//@[021:022) Colon |:|
//@[023:039) Identifier |runtimeValidRes1|
//@[039:040) Dot |.|
//@[040:050) Identifier |properties|
//@[050:051) Dot |.|
//@[051:065) Identifier |evictionPolicy|
//@[065:067) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes3 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes3|
//@[026:085) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  name: '${runtimeValidRes1.name}_v1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:027) Identifier |runtimeValidRes1|
//@[027:028) Dot |.|
//@[028:032) Identifier |name|
//@[032:037) StringRightPiece |}_v1'|
//@[037:039) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes4|
//@[026:085) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  name: concat(validModule['name'], 'v1')
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:026) Identifier |validModule|
//@[026:027) LeftSquare |[|
//@[027:033) StringComplete |'name'|
//@[033:034) RightSquare |]|
//@[034:035) Comma |,|
//@[036:040) StringComplete |'v1'|
//@[040:041) RightParen |)|
//@[041:043) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes5|
//@[026:085) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  name: '${validModule.name}_v1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:022) Identifier |validModule|
//@[022:023) Dot |.|
//@[023:027) Identifier |name|
//@[027:032) StringRightPiece |}_v1'|
//@[032:034) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |runtimeInvalidRes1|
//@[028:087) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: runtimeValidRes1.location
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:033) Identifier |location|
//@[033:035) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |runtimeInvalidRes2|
//@[028:087) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: runtimeValidRes1['location']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) LeftSquare |[|
//@[025:035) StringComplete |'location'|
//@[035:036) RightSquare |]|
//@[036:038) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |runtimeInvalidRes3|
//@[028:078) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[079:080) Assignment |=|
//@[081:082) LeftBrace |{|
//@[082:084) NewLine |\r\n|
  name: runtimeValidRes1.properties.evictionPolicy
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:035) Identifier |properties|
//@[035:036) Dot |.|
//@[036:050) Identifier |evictionPolicy|
//@[050:052) NewLine |\r\n|
  kind:'AzureCLI'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[007:017) StringComplete |'AzureCLI'|
//@[017:019) NewLine |\r\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    azCliVersion: '2.0'
//@[004:016) Identifier |azCliVersion|
//@[016:017) Colon |:|
//@[018:023) StringComplete |'2.0'|
//@[023:025) NewLine |\r\n|
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
//@[004:021) Identifier |retentionInterval|
//@[021:022) Colon |:|
//@[023:039) Identifier |runtimeValidRes1|
//@[039:040) Dot |.|
//@[040:050) Identifier |properties|
//@[050:051) Dot |.|
//@[051:065) Identifier |evictionPolicy|
//@[065:067) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |runtimeInvalidRes4|
//@[028:087) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: runtimeValidRes1['properties'].evictionPolicy
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) LeftSquare |[|
//@[025:037) StringComplete |'properties'|
//@[037:038) RightSquare |]|
//@[038:039) Dot |.|
//@[039:053) Identifier |evictionPolicy|
//@[053:055) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |runtimeInvalidRes5|
//@[028:087) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: runtimeValidRes1['properties']['evictionPolicy']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) LeftSquare |[|
//@[025:037) StringComplete |'properties'|
//@[037:038) RightSquare |]|
//@[038:039) LeftSquare |[|
//@[039:055) StringComplete |'evictionPolicy'|
//@[055:056) RightSquare |]|
//@[056:058) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |runtimeInvalidRes6|
//@[028:087) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: runtimeValidRes1.properties['evictionPolicy']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:035) Identifier |properties|
//@[035:036) LeftSquare |[|
//@[036:052) StringComplete |'evictionPolicy'|
//@[052:053) RightSquare |]|
//@[053:055) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |runtimeInvalidRes7|
//@[028:087) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: runtimeValidRes2.properties.azCliVersion
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes2|
//@[024:025) Dot |.|
//@[025:035) Identifier |properties|
//@[035:036) Dot |.|
//@[036:048) Identifier |azCliVersion|
//@[048:050) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var magicString1 = 'location'
//@[000:003) Identifier |var|
//@[004:016) Identifier |magicString1|
//@[017:018) Assignment |=|
//@[019:029) StringComplete |'location'|
//@[029:031) NewLine |\r\n|
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |runtimeInvalidRes8|
//@[028:087) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: runtimeValidRes2['${magicString1}']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes2|
//@[024:025) LeftSquare |[|
//@[025:028) StringLeftPiece |'${|
//@[028:040) Identifier |magicString1|
//@[040:042) StringRightPiece |}'|
//@[042:043) RightSquare |]|
//@[043:045) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes10|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: '${runtimeValidRes3.location}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:027) Identifier |runtimeValidRes3|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:038) StringRightPiece |}'|
//@[038:040) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes11|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: validModule.params['name']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) Identifier |validModule|
//@[019:020) Dot |.|
//@[020:026) Identifier |params|
//@[026:027) LeftSquare |[|
//@[027:033) StringComplete |'name'|
//@[033:034) RightSquare |]|
//@[034:036) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes12|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:031) Identifier |runtimeValidRes1|
//@[031:032) Dot |.|
//@[032:040) Identifier |location|
//@[040:041) Comma |,|
//@[042:058) Identifier |runtimeValidRes2|
//@[058:059) LeftSquare |[|
//@[059:069) StringComplete |'location'|
//@[069:070) RightSquare |]|
//@[070:071) Comma |,|
//@[072:090) Identifier |runtimeInvalidRes3|
//@[090:091) LeftSquare |[|
//@[091:103) StringComplete |'properties'|
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
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes13|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:027) Identifier |runtimeValidRes1|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:039) StringMiddlePiece |}${|
//@[039:055) Identifier |runtimeValidRes2|
//@[055:056) LeftSquare |[|
//@[056:066) StringComplete |'location'|
//@[066:067) RightSquare |]|
//@[067:070) StringMiddlePiece |}${|
//@[070:088) Identifier |runtimeInvalidRes3|
//@[088:089) Dot |.|
//@[089:099) Identifier |properties|
//@[099:100) LeftSquare |[|
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
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// variable related runtime validation
//@[038:040) NewLine |\r\n|
var runtimefoo1 = runtimeValidRes1['location']
//@[000:003) Identifier |var|
//@[004:015) Identifier |runtimefoo1|
//@[016:017) Assignment |=|
//@[018:034) Identifier |runtimeValidRes1|
//@[034:035) LeftSquare |[|
//@[035:045) StringComplete |'location'|
//@[045:046) RightSquare |]|
//@[046:048) NewLine |\r\n|
var runtimefoo2 = runtimeValidRes2['properties'].azCliVersion
//@[000:003) Identifier |var|
//@[004:015) Identifier |runtimefoo2|
//@[016:017) Assignment |=|
//@[018:034) Identifier |runtimeValidRes2|
//@[034:035) LeftSquare |[|
//@[035:047) StringComplete |'properties'|
//@[047:048) RightSquare |]|
//@[048:049) Dot |.|
//@[049:061) Identifier |azCliVersion|
//@[061:063) NewLine |\r\n|
var runtimefoo3 = runtimeValidRes2
//@[000:003) Identifier |var|
//@[004:015) Identifier |runtimefoo3|
//@[016:017) Assignment |=|
//@[018:034) Identifier |runtimeValidRes2|
//@[034:036) NewLine |\r\n|
var runtimefoo4 = {
//@[000:003) Identifier |var|
//@[004:015) Identifier |runtimefoo4|
//@[016:017) Assignment |=|
//@[018:019) LeftBrace |{|
//@[019:021) NewLine |\r\n|
  hop: runtimefoo2
//@[002:005) Identifier |hop|
//@[005:006) Colon |:|
//@[007:018) Identifier |runtimefoo2|
//@[018:020) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var runtimeInvalid = {
//@[000:003) Identifier |var|
//@[004:018) Identifier |runtimeInvalid|
//@[019:020) Assignment |=|
//@[021:022) LeftBrace |{|
//@[022:024) NewLine |\r\n|
  foo1: runtimefoo1
//@[002:006) Identifier |foo1|
//@[006:007) Colon |:|
//@[008:019) Identifier |runtimefoo1|
//@[019:021) NewLine |\r\n|
  foo2: runtimefoo2
//@[002:006) Identifier |foo2|
//@[006:007) Colon |:|
//@[008:019) Identifier |runtimefoo2|
//@[019:021) NewLine |\r\n|
  foo3: runtimefoo3
//@[002:006) Identifier |foo3|
//@[006:007) Colon |:|
//@[008:019) Identifier |runtimefoo3|
//@[019:021) NewLine |\r\n|
  foo4: runtimeValidRes1.name
//@[002:006) Identifier |foo4|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:029) Identifier |name|
//@[029:031) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var runtimeValid = {
//@[000:003) Identifier |var|
//@[004:016) Identifier |runtimeValid|
//@[017:018) Assignment |=|
//@[019:020) LeftBrace |{|
//@[020:022) NewLine |\r\n|
  foo1: runtimeValidRes1.name
//@[002:006) Identifier |foo1|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:029) Identifier |name|
//@[029:031) NewLine |\r\n|
  foo2: runtimeValidRes1.apiVersion
//@[002:006) Identifier |foo2|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:035) Identifier |apiVersion|
//@[035:037) NewLine |\r\n|
  foo3: runtimeValidRes2.type
//@[002:006) Identifier |foo3|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes2|
//@[024:025) Dot |.|
//@[025:029) Identifier |type|
//@[029:031) NewLine |\r\n|
  foo4: runtimeValidRes2.id
//@[002:006) Identifier |foo4|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes2|
//@[024:025) Dot |.|
//@[025:027) Identifier |id|
//@[027:029) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes14 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes14|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: runtimeInvalid.foo1
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) Identifier |runtimeInvalid|
//@[022:023) Dot |.|
//@[023:027) Identifier |foo1|
//@[027:029) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes15|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: runtimeInvalid.foo2
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) Identifier |runtimeInvalid|
//@[022:023) Dot |.|
//@[023:027) Identifier |foo2|
//@[027:029) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes16|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: runtimeInvalid.foo3.properties.azCliVersion
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) Identifier |runtimeInvalid|
//@[022:023) Dot |.|
//@[023:027) Identifier |foo3|
//@[027:028) Dot |.|
//@[028:038) Identifier |properties|
//@[038:039) Dot |.|
//@[039:051) Identifier |azCliVersion|
//@[051:053) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
//@[128:130) NewLine |\r\n|
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes17|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: runtimeInvalid.foo4
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) Identifier |runtimeInvalid|
//@[022:023) Dot |.|
//@[023:027) Identifier |foo4|
//@[027:029) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |runtimeInvalidRes18|
//@[029:088) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:029) Identifier |runtimeInvalid|
//@[029:030) Dot |.|
//@[030:034) Identifier |foo1|
//@[034:035) Comma |,|
//@[036:052) Identifier |runtimeValidRes2|
//@[052:053) LeftSquare |[|
//@[053:065) StringComplete |'properties'|
//@[065:066) RightSquare |]|
//@[066:067) Dot |.|
//@[067:079) Identifier |azCliVersion|
//@[079:080) Comma |,|
//@[081:084) StringLeftPiece |'${|
//@[084:100) Identifier |runtimeValidRes1|
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
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes6|
//@[026:085) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  name: runtimeValid.foo1
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) Identifier |runtimeValid|
//@[020:021) Dot |.|
//@[021:025) Identifier |foo1|
//@[025:027) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes7|
//@[026:085) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  name: runtimeValid.foo2
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) Identifier |runtimeValid|
//@[020:021) Dot |.|
//@[021:025) Identifier |foo2|
//@[025:027) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes8|
//@[026:085) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  name: runtimeValid.foo3
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) Identifier |runtimeValid|
//@[020:021) Dot |.|
//@[021:025) Identifier |foo3|
//@[025:027) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource runtimeValidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes9|
//@[026:085) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  name: runtimeValid.foo4
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) Identifier |runtimeValid|
//@[020:021) Dot |.|
//@[021:025) Identifier |foo4|
//@[025:027) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var magicString2 = 'name'
//@[000:003) Identifier |var|
//@[004:016) Identifier |magicString2|
//@[017:018) Assignment |=|
//@[019:025) StringComplete |'name'|
//@[025:027) NewLine |\r\n|
resource runtimeValidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:026) Identifier |runtimeValidRes10|
//@[027:086) StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[087:088) Assignment |=|
//@[089:090) LeftBrace |{|
//@[090:092) NewLine |\r\n|
  name: runtimeValidRes2['${magicString2}']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes2|
//@[024:025) LeftSquare |[|
//@[025:028) StringLeftPiece |'${|
//@[028:040) Identifier |magicString2|
//@[040:042) StringRightPiece |}'|
//@[042:043) RightSquare |]|
//@[043:045) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource loopForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |loopForRuntimeCheck|
//@[029:068) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[069:070) Assignment |=|
//@[071:072) LeftSquare |[|
//@[072:075) Identifier |for|
//@[076:081) Identifier |thing|
//@[082:084) Identifier |in|
//@[085:086) LeftSquare |[|
//@[086:087) RightSquare |]|
//@[087:088) Colon |:|
//@[089:090) LeftBrace |{|
//@[090:092) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'test'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:018) StringComplete |'test'|
//@[018:020) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
//@[000:003) Identifier |var|
//@[004:019) Identifier |runtimeCheckVar|
//@[020:021) Assignment |=|
//@[022:041) Identifier |loopForRuntimeCheck|
//@[041:042) LeftSquare |[|
//@[042:043) Integer |0|
//@[043:044) RightSquare |]|
//@[044:045) Dot |.|
//@[045:055) Identifier |properties|
//@[055:056) Dot |.|
//@[056:064) Identifier |zoneType|
//@[064:066) NewLine |\r\n|
var runtimeCheckVar2 = runtimeCheckVar
//@[000:003) Identifier |var|
//@[004:020) Identifier |runtimeCheckVar2|
//@[021:022) Assignment |=|
//@[023:038) Identifier |runtimeCheckVar|
//@[038:042) NewLine |\r\n\r\n|

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:008) Identifier |resource|
//@[009:038) Identifier |singleResourceForRuntimeCheck|
//@[039:078) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[079:080) Assignment |=|
//@[081:082) LeftBrace |{|
//@[082:084) NewLine |\r\n|
  name: runtimeCheckVar2
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeCheckVar2|
//@[024:026) NewLine |\r\n|
  location: 'test'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:018) StringComplete |'test'|
//@[018:020) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:029) Identifier |loopForRuntimeCheck2|
//@[030:069) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:071) Assignment |=|
//@[072:073) LeftSquare |[|
//@[073:076) Identifier |for|
//@[077:082) Identifier |thing|
//@[083:085) Identifier |in|
//@[086:087) LeftSquare |[|
//@[087:088) RightSquare |]|
//@[088:089) Colon |:|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: runtimeCheckVar2
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeCheckVar2|
//@[024:026) NewLine |\r\n|
  location: 'test'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:018) StringComplete |'test'|
//@[018:020) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[000:008) Identifier |resource|
//@[009:029) Identifier |loopForRuntimeCheck3|
//@[030:069) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:071) Assignment |=|
//@[072:073) LeftSquare |[|
//@[073:076) Identifier |for|
//@[077:087) Identifier |otherThing|
//@[088:090) Identifier |in|
//@[091:092) LeftSquare |[|
//@[092:093) RightSquare |]|
//@[093:094) Colon |:|
//@[095:096) LeftBrace |{|
//@[096:098) NewLine |\r\n|
  name: loopForRuntimeCheck[0].properties.zoneType
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) Identifier |loopForRuntimeCheck|
//@[027:028) LeftSquare |[|
//@[028:029) Integer |0|
//@[029:030) RightSquare |]|
//@[030:031) Dot |.|
//@[031:041) Identifier |properties|
//@[041:042) Dot |.|
//@[042:050) Identifier |zoneType|
//@[050:052) NewLine |\r\n|
  location: 'test'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:018) StringComplete |'test'|
//@[018:020) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
//@[000:003) Identifier |var|
//@[004:024) Identifier |varForRuntimeCheck4a|
//@[025:026) Assignment |=|
//@[027:046) Identifier |loopForRuntimeCheck|
//@[046:047) LeftSquare |[|
//@[047:048) Integer |0|
//@[048:049) RightSquare |]|
//@[049:050) Dot |.|
//@[050:060) Identifier |properties|
//@[060:061) Dot |.|
//@[061:069) Identifier |zoneType|
//@[069:071) NewLine |\r\n|
var varForRuntimeCheck4b = varForRuntimeCheck4a
//@[000:003) Identifier |var|
//@[004:024) Identifier |varForRuntimeCheck4b|
//@[025:026) Assignment |=|
//@[027:047) Identifier |varForRuntimeCheck4a|
//@[047:049) NewLine |\r\n|
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[000:008) Identifier |resource|
//@[009:029) Identifier |loopForRuntimeCheck4|
//@[030:069) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:071) Assignment |=|
//@[072:073) LeftSquare |[|
//@[073:076) Identifier |for|
//@[077:087) Identifier |otherThing|
//@[088:090) Identifier |in|
//@[091:092) LeftSquare |[|
//@[092:093) RightSquare |]|
//@[093:094) Colon |:|
//@[095:096) LeftBrace |{|
//@[096:098) NewLine |\r\n|
  name: varForRuntimeCheck4b
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:028) Identifier |varForRuntimeCheck4b|
//@[028:030) NewLine |\r\n|
  location: 'test'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:018) StringComplete |'test'|
//@[018:020) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |missingTopLevelProperties|
//@[035:089) StringComplete |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[090:091) Assignment |=|
//@[092:093) LeftBrace |{|
//@[093:095) NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelProperties
//@[051:053) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[000:008) Identifier |resource|
//@[009:044) Identifier |missingTopLevelPropertiesExceptName|
//@[045:099) StringComplete |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[100:101) Assignment |=|
//@[102:103) LeftBrace |{|
//@[103:105) NewLine |\r\n|
  // #completionTest(2) -> topLevelPropertiesMinusNameNoColon
//@[061:063) NewLine |\r\n|
  name: 'me'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) StringComplete |'me'|
//@[012:014) NewLine |\r\n|
  // do not remove whitespace before the closing curly
//@[054:056) NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
//@[060:062) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// #completionTest(24,25,26,49,65,69,70) -> virtualNetworksResourceTypes
//@[072:074) NewLine |\r\n|
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:023) Identifier |unfinishedVnet|
//@[024:070) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[071:072) Assignment |=|
//@[073:074) LeftBrace |{|
//@[074:076) NewLine |\r\n|
  name: 'v'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'v'|
//@[011:013) NewLine |\r\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: [
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
      {
//@[006:007) LeftBrace |{|
//@[007:009) NewLine |\r\n|
        // #completionTest(0,1,2,3,4,5,6,7) -> subnetPropertiesMinusProperties
//@[078:080) NewLine |\r\n|
       
//@[007:009) NewLine |\r\n|
        // #completionTest(0,1,2,3,4,5,6,7) -> empty
//@[052:054) NewLine |\r\n|
        properties: {
//@[008:018) Identifier |properties|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
          delegations: [
//@[010:021) Identifier |delegations|
//@[021:022) Colon |:|
//@[023:024) LeftSquare |[|
//@[024:026) NewLine |\r\n|
            {
//@[012:013) LeftBrace |{|
//@[013:015) NewLine |\r\n|
              // #completionTest(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14) -> delegationProperties
//@[092:094) NewLine |\r\n|
              
//@[014:016) NewLine |\r\n|
            }
//@[012:013) RightBrace |}|
//@[013:015) NewLine |\r\n|
          ]
//@[010:011) RightSquare |]|
//@[011:013) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

/*
Discriminator key missing
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeyMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:032) Identifier |discriminatorKeyMissing|
//@[033:083) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[084:085) Assignment |=|
//@[086:087) LeftBrace |{|
//@[087:089) NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[052:054) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

/*
Discriminator key missing (conditional)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |discriminatorKeyMissing_if|
//@[036:086) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[087:088) Assignment |=|
//@[089:091) Identifier |if|
//@[091:092) LeftParen |(|
//@[092:096) TrueKeyword |true|
//@[096:097) RightParen |)|
//@[098:099) LeftBrace |{|
//@[099:101) NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[052:054) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

/*
Discriminator key missing (loop)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeyMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:036) Identifier |discriminatorKeyMissing_for|
//@[037:087) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[088:089) Assignment |=|
//@[090:091) LeftSquare |[|
//@[091:094) Identifier |for|
//@[095:100) Identifier |thing|
//@[101:103) Identifier |in|
//@[104:105) LeftSquare |[|
//@[105:106) RightSquare |]|
//@[106:107) Colon |:|
//@[108:109) LeftBrace |{|
//@[109:111) NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[052:054) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

/*
Discriminator key missing (filtered loop)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeyMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[000:008) Identifier |resource|
//@[009:039) Identifier |discriminatorKeyMissing_for_if|
//@[040:090) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[091:092) Assignment |=|
//@[093:094) LeftSquare |[|
//@[094:097) Identifier |for|
//@[098:103) Identifier |thing|
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
//@[052:054) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:037) Identifier |discriminatorKeyValueMissing|
//@[038:088) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
//@[066:068) NewLine |\r\n|
  kind:   
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[010:012) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
//@[060:062) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[000:003) Identifier |var|
//@[004:043) Identifier |discriminatorKeyValueMissingCompletions|
//@[044:045) Assignment |=|
//@[046:074) Identifier |discriminatorKeyValueMissing|
//@[074:075) Dot |.|
//@[075:076) Identifier |p|
//@[076:078) NewLine |\r\n|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
//@[060:062) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[000:003) Identifier |var|
//@[004:044) Identifier |discriminatorKeyValueMissingCompletions2|
//@[045:046) Assignment |=|
//@[047:075) Identifier |discriminatorKeyValueMissing|
//@[075:076) Dot |.|
//@[076:080) NewLine |\r\n\r\n|

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
//@[070:072) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[000:003) Identifier |var|
//@[004:044) Identifier |discriminatorKeyValueMissingCompletions3|
//@[045:046) Assignment |=|
//@[047:075) Identifier |discriminatorKeyValueMissing|
//@[075:076) LeftSquare |[|
//@[076:077) RightSquare |]|
//@[077:081) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access (conditional)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
//@[000:008) Identifier |resource|
//@[009:040) Identifier |discriminatorKeyValueMissing_if|
//@[041:091) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[092:093) Assignment |=|
//@[094:096) Identifier |if|
//@[096:097) LeftParen |(|
//@[097:102) FalseKeyword |false|
//@[102:103) RightParen |)|
//@[104:105) LeftBrace |{|
//@[105:107) NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
//@[069:071) NewLine |\r\n|
  kind:   
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[010:012) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
//@[060:062) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
//@[000:003) Identifier |var|
//@[004:046) Identifier |discriminatorKeyValueMissingCompletions_if|
//@[047:048) Assignment |=|
//@[049:080) Identifier |discriminatorKeyValueMissing_if|
//@[080:081) Dot |.|
//@[081:082) Identifier |p|
//@[082:084) NewLine |\r\n|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
//@[060:062) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.
//@[000:003) Identifier |var|
//@[004:047) Identifier |discriminatorKeyValueMissingCompletions2_if|
//@[048:049) Assignment |=|
//@[050:081) Identifier |discriminatorKeyValueMissing_if|
//@[081:082) Dot |.|
//@[082:086) NewLine |\r\n\r\n|

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
//@[073:075) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]
//@[000:003) Identifier |var|
//@[004:047) Identifier |discriminatorKeyValueMissingCompletions3_if|
//@[048:049) Assignment |=|
//@[050:081) Identifier |discriminatorKeyValueMissing_if|
//@[081:082) LeftSquare |[|
//@[082:083) RightSquare |]|
//@[083:087) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access (loops)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:041) Identifier |discriminatorKeyValueMissing_for|
//@[042:092) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[093:094) Assignment |=|
//@[095:096) LeftSquare |[|
//@[096:099) Identifier |for|
//@[100:105) Identifier |thing|
//@[106:108) Identifier |in|
//@[109:110) LeftSquare |[|
//@[110:111) RightSquare |]|
//@[111:112) Colon |:|
//@[113:114) LeftBrace |{|
//@[114:116) NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
//@[070:072) NewLine |\r\n|
  kind:   
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[010:012) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// cannot . access properties of a resource loop
//@[048:050) NewLine |\r\n|
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind
//@[000:003) Identifier |var|
//@[004:035) Identifier |resourceListIsNotSingleResource|
//@[036:037) Assignment |=|
//@[038:070) Identifier |discriminatorKeyValueMissing_for|
//@[070:071) Dot |.|
//@[071:075) Identifier |kind|
//@[075:079) NewLine |\r\n\r\n|

// #completionTest(87) -> missingDiscriminatorPropertyAccess
//@[060:062) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
//@[000:003) Identifier |var|
//@[004:047) Identifier |discriminatorKeyValueMissingCompletions_for|
//@[048:049) Assignment |=|
//@[050:082) Identifier |discriminatorKeyValueMissing_for|
//@[082:083) LeftSquare |[|
//@[083:084) Integer |0|
//@[084:085) RightSquare |]|
//@[085:086) Dot |.|
//@[086:087) Identifier |p|
//@[087:089) NewLine |\r\n|
// #completionTest(87) -> missingDiscriminatorPropertyAccess
//@[060:062) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].
//@[000:003) Identifier |var|
//@[004:048) Identifier |discriminatorKeyValueMissingCompletions2_for|
//@[049:050) Assignment |=|
//@[051:083) Identifier |discriminatorKeyValueMissing_for|
//@[083:084) LeftSquare |[|
//@[084:085) Integer |0|
//@[085:086) RightSquare |]|
//@[086:087) Dot |.|
//@[087:091) NewLine |\r\n\r\n|

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
//@[074:076) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]
//@[000:003) Identifier |var|
//@[004:048) Identifier |discriminatorKeyValueMissingCompletions3_for|
//@[049:050) Assignment |=|
//@[051:083) Identifier |discriminatorKeyValueMissing_for|
//@[083:084) LeftSquare |[|
//@[084:085) Integer |0|
//@[085:086) RightSquare |]|
//@[086:087) LeftSquare |[|
//@[087:088) RightSquare |]|
//@[088:092) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access (filtered loops)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[000:008) Identifier |resource|
//@[009:044) Identifier |discriminatorKeyValueMissing_for_if|
//@[045:095) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[096:097) Assignment |=|
//@[098:099) LeftSquare |[|
//@[099:102) Identifier |for|
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
//@[073:075) NewLine |\r\n|
  kind:   
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[010:012) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// cannot . access properties of a resource loop
//@[048:050) NewLine |\r\n|
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind
//@[000:003) Identifier |var|
//@[004:038) Identifier |resourceListIsNotSingleResource_if|
//@[039:040) Assignment |=|
//@[041:076) Identifier |discriminatorKeyValueMissing_for_if|
//@[076:077) Dot |.|
//@[077:081) Identifier |kind|
//@[081:085) NewLine |\r\n\r\n|

// #completionTest(93) -> missingDiscriminatorPropertyAccess
//@[060:062) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
//@[000:003) Identifier |var|
//@[004:050) Identifier |discriminatorKeyValueMissingCompletions_for_if|
//@[051:052) Assignment |=|
//@[053:088) Identifier |discriminatorKeyValueMissing_for_if|
//@[088:089) LeftSquare |[|
//@[089:090) Integer |0|
//@[090:091) RightSquare |]|
//@[091:092) Dot |.|
//@[092:093) Identifier |p|
//@[093:095) NewLine |\r\n|
// #completionTest(93) -> missingDiscriminatorPropertyAccess
//@[060:062) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].
//@[000:003) Identifier |var|
//@[004:051) Identifier |discriminatorKeyValueMissingCompletions2_for_if|
//@[052:053) Assignment |=|
//@[054:089) Identifier |discriminatorKeyValueMissing_for_if|
//@[089:090) LeftSquare |[|
//@[090:091) Integer |0|
//@[091:092) RightSquare |]|
//@[092:093) Dot |.|
//@[093:097) NewLine |\r\n\r\n|

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
//@[077:079) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]
//@[000:003) Identifier |var|
//@[004:051) Identifier |discriminatorKeyValueMissingCompletions3_for_if|
//@[052:053) Assignment |=|
//@[054:089) Identifier |discriminatorKeyValueMissing_for_if|
//@[089:090) LeftSquare |[|
//@[090:091) Integer |0|
//@[091:092) RightSquare |]|
//@[092:093) LeftSquare |[|
//@[093:094) RightSquare |]|
//@[094:098) NewLine |\r\n\r\n|

/*
Discriminator value set 1
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |discriminatorKeySetOne|
//@[032:082) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[086:088) NewLine |\r\n|
  kind: 'AzureCLI'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'AzureCLI'|
//@[018:020) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:057) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[066:068) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(75) -> cliPropertyAccess
//@[043:045) NewLine |\r\n|
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
//@[000:003) Identifier |var|
//@[004:037) Identifier |discriminatorKeySetOneCompletions|
//@[038:039) Assignment |=|
//@[040:062) Identifier |discriminatorKeySetOne|
//@[062:063) Dot |.|
//@[063:073) Identifier |properties|
//@[073:074) Dot |.|
//@[074:075) Identifier |a|
//@[075:077) NewLine |\r\n|
// #completionTest(75) -> cliPropertyAccess
//@[043:045) NewLine |\r\n|
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[000:003) Identifier |var|
//@[004:038) Identifier |discriminatorKeySetOneCompletions2|
//@[039:040) Assignment |=|
//@[041:063) Identifier |discriminatorKeySetOne|
//@[063:064) Dot |.|
//@[064:074) Identifier |properties|
//@[074:075) Dot |.|
//@[075:079) NewLine |\r\n\r\n|

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
//@[061:063) NewLine |\r\n|
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[000:003) Identifier |var|
//@[004:038) Identifier |discriminatorKeySetOneCompletions3|
//@[039:040) Assignment |=|
//@[041:063) Identifier |discriminatorKeySetOne|
//@[063:064) Dot |.|
//@[064:074) Identifier |properties|
//@[074:075) LeftSquare |[|
//@[075:076) RightSquare |]|
//@[076:080) NewLine |\r\n\r\n|

/*
Discriminator value set 1 (conditional)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |discriminatorKeySetOne_if|
//@[035:085) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[086:087) Assignment |=|
//@[088:090) Identifier |if|
//@[090:091) LeftParen |(|
//@[091:092) Integer |2|
//@[092:094) Equals |==|
//@[094:095) Integer |3|
//@[095:096) RightParen |)|
//@[097:098) LeftBrace |{|
//@[098:100) NewLine |\r\n|
  kind: 'AzureCLI'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'AzureCLI'|
//@[018:020) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:057) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[066:068) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(81) -> cliPropertyAccess
//@[043:045) NewLine |\r\n|
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
//@[000:003) Identifier |var|
//@[004:040) Identifier |discriminatorKeySetOneCompletions_if|
//@[041:042) Assignment |=|
//@[043:068) Identifier |discriminatorKeySetOne_if|
//@[068:069) Dot |.|
//@[069:079) Identifier |properties|
//@[079:080) Dot |.|
//@[080:081) Identifier |a|
//@[081:083) NewLine |\r\n|
// #completionTest(81) -> cliPropertyAccess
//@[043:045) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.
//@[000:003) Identifier |var|
//@[004:041) Identifier |discriminatorKeySetOneCompletions2_if|
//@[042:043) Assignment |=|
//@[044:069) Identifier |discriminatorKeySetOne_if|
//@[069:070) Dot |.|
//@[070:080) Identifier |properties|
//@[080:081) Dot |.|
//@[081:085) NewLine |\r\n\r\n|

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
//@[064:066) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]
//@[000:003) Identifier |var|
//@[004:041) Identifier |discriminatorKeySetOneCompletions3_if|
//@[042:043) Assignment |=|
//@[044:069) Identifier |discriminatorKeySetOne_if|
//@[069:070) Dot |.|
//@[070:080) Identifier |properties|
//@[080:081) LeftSquare |[|
//@[081:082) RightSquare |]|
//@[082:086) NewLine |\r\n\r\n|

/*
Discriminator value set 1 (loop)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |discriminatorKeySetOne_for|
//@[036:086) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[087:088) Assignment |=|
//@[089:090) LeftSquare |[|
//@[091:094) Identifier |for|
//@[095:100) Identifier |thing|
//@[101:103) Identifier |in|
//@[104:105) LeftSquare |[|
//@[105:106) RightSquare |]|
//@[106:107) Colon |:|
//@[108:109) LeftBrace |{|
//@[109:111) NewLine |\r\n|
  kind: 'AzureCLI'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'AzureCLI'|
//@[018:020) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:057) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[066:068) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
// #completionTest(86) -> cliPropertyAccess
//@[043:045) NewLine |\r\n|
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
//@[000:003) Identifier |var|
//@[004:041) Identifier |discriminatorKeySetOneCompletions_for|
//@[042:043) Assignment |=|
//@[044:070) Identifier |discriminatorKeySetOne_for|
//@[070:071) LeftSquare |[|
//@[071:072) Integer |0|
//@[072:073) RightSquare |]|
//@[073:074) Dot |.|
//@[074:084) Identifier |properties|
//@[084:085) Dot |.|
//@[085:086) Identifier |a|
//@[086:088) NewLine |\r\n|
// #completionTest(94) -> cliPropertyAccess
//@[043:045) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.
//@[000:003) Identifier |var|
//@[004:042) Identifier |discriminatorKeySetOneCompletions2_for|
//@[043:044) Assignment |=|
//@[045:071) Identifier |discriminatorKeySetOne_for|
//@[071:072) LeftSquare |[|
//@[072:075) Identifier |any|
//@[075:076) LeftParen |(|
//@[076:080) TrueKeyword |true|
//@[080:081) RightParen |)|
//@[081:082) RightSquare |]|
//@[082:083) Dot |.|
//@[083:093) Identifier |properties|
//@[093:094) Dot |.|
//@[094:098) NewLine |\r\n\r\n|

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
//@[065:067) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]
//@[000:003) Identifier |var|
//@[004:042) Identifier |discriminatorKeySetOneCompletions3_for|
//@[043:044) Assignment |=|
//@[045:071) Identifier |discriminatorKeySetOne_for|
//@[071:072) LeftSquare |[|
//@[072:073) Integer |1|
//@[073:074) RightSquare |]|
//@[074:075) Dot |.|
//@[075:085) Identifier |properties|
//@[085:086) LeftSquare |[|
//@[086:087) RightSquare |]|
//@[087:091) NewLine |\r\n\r\n|

/*
Discriminator value set 1 (filtered loop)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
//@[000:008) Identifier |resource|
//@[009:038) Identifier |discriminatorKeySetOne_for_if|
//@[039:089) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[090:091) Assignment |=|
//@[092:093) LeftSquare |[|
//@[094:097) Identifier |for|
//@[098:103) Identifier |thing|
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
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'AzureCLI'|
//@[018:020) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:057) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[066:068) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
// #completionTest(92) -> cliPropertyAccess
//@[043:045) NewLine |\r\n|
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
//@[000:003) Identifier |var|
//@[004:044) Identifier |discriminatorKeySetOneCompletions_for_if|
//@[045:046) Assignment |=|
//@[047:076) Identifier |discriminatorKeySetOne_for_if|
//@[076:077) LeftSquare |[|
//@[077:078) Integer |0|
//@[078:079) RightSquare |]|
//@[079:080) Dot |.|
//@[080:090) Identifier |properties|
//@[090:091) Dot |.|
//@[091:092) Identifier |a|
//@[092:094) NewLine |\r\n|
// #completionTest(100) -> cliPropertyAccess
//@[044:046) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.
//@[000:003) Identifier |var|
//@[004:045) Identifier |discriminatorKeySetOneCompletions2_for_if|
//@[046:047) Assignment |=|
//@[048:077) Identifier |discriminatorKeySetOne_for_if|
//@[077:078) LeftSquare |[|
//@[078:081) Identifier |any|
//@[081:082) LeftParen |(|
//@[082:086) TrueKeyword |true|
//@[086:087) RightParen |)|
//@[087:088) RightSquare |]|
//@[088:089) Dot |.|
//@[089:099) Identifier |properties|
//@[099:100) Dot |.|
//@[100:104) NewLine |\r\n\r\n|

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
//@[068:070) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]
//@[000:003) Identifier |var|
//@[004:045) Identifier |discriminatorKeySetOneCompletions3_for_if|
//@[046:047) Assignment |=|
//@[048:077) Identifier |discriminatorKeySetOne_for_if|
//@[077:078) LeftSquare |[|
//@[078:079) Integer |1|
//@[079:080) RightSquare |]|
//@[080:081) Dot |.|
//@[081:091) Identifier |properties|
//@[091:092) LeftSquare |[|
//@[092:093) RightSquare |]|
//@[093:099) NewLine |\r\n\r\n\r\n|


/*
Discriminator value set 2
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |discriminatorKeySetTwo|
//@[032:082) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[086:088) NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'AzurePowerShell'|
//@[025:027) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:057) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[065:067) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(75) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
//@[000:003) Identifier |var|
//@[004:037) Identifier |discriminatorKeySetTwoCompletions|
//@[038:039) Assignment |=|
//@[040:062) Identifier |discriminatorKeySetTwo|
//@[062:063) Dot |.|
//@[063:073) Identifier |properties|
//@[073:074) Dot |.|
//@[074:075) Identifier |a|
//@[075:077) NewLine |\r\n|
// #completionTest(75) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[000:003) Identifier |var|
//@[004:038) Identifier |discriminatorKeySetTwoCompletions2|
//@[039:040) Assignment |=|
//@[041:063) Identifier |discriminatorKeySetTwo|
//@[063:064) Dot |.|
//@[064:074) Identifier |properties|
//@[074:075) Dot |.|
//@[075:079) NewLine |\r\n\r\n|

// #completionTest(90) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
//@[000:003) Identifier |var|
//@[004:049) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer|
//@[050:051) Assignment |=|
//@[052:074) Identifier |discriminatorKeySetTwo|
//@[074:075) LeftSquare |[|
//@[075:087) StringComplete |'properties'|
//@[087:088) RightSquare |]|
//@[088:089) Dot |.|
//@[089:090) Identifier |a|
//@[090:092) NewLine |\r\n|
// #completionTest(90) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[000:003) Identifier |var|
//@[004:050) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2|
//@[051:052) Assignment |=|
//@[053:075) Identifier |discriminatorKeySetTwo|
//@[075:076) LeftSquare |[|
//@[076:088) StringComplete |'properties'|
//@[088:089) RightSquare |]|
//@[089:090) Dot |.|
//@[090:094) NewLine |\r\n\r\n|

/*
Discriminator value set 2 (conditional)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |discriminatorKeySetTwo_if|
//@[035:085) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'AzurePowerShell'|
//@[025:027) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:057) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[065:067) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(81) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
//@[000:003) Identifier |var|
//@[004:040) Identifier |discriminatorKeySetTwoCompletions_if|
//@[041:042) Assignment |=|
//@[043:068) Identifier |discriminatorKeySetTwo_if|
//@[068:069) Dot |.|
//@[069:079) Identifier |properties|
//@[079:080) Dot |.|
//@[080:081) Identifier |a|
//@[081:083) NewLine |\r\n|
// #completionTest(81) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.
//@[000:003) Identifier |var|
//@[004:041) Identifier |discriminatorKeySetTwoCompletions2_if|
//@[042:043) Assignment |=|
//@[044:069) Identifier |discriminatorKeySetTwo_if|
//@[069:070) Dot |.|
//@[070:080) Identifier |properties|
//@[080:081) Dot |.|
//@[081:085) NewLine |\r\n\r\n|

// #completionTest(96) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
//@[000:003) Identifier |var|
//@[004:052) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_if|
//@[053:054) Assignment |=|
//@[055:080) Identifier |discriminatorKeySetTwo_if|
//@[080:081) LeftSquare |[|
//@[081:093) StringComplete |'properties'|
//@[093:094) RightSquare |]|
//@[094:095) Dot |.|
//@[095:096) Identifier |a|
//@[096:098) NewLine |\r\n|
// #completionTest(96) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].
//@[000:003) Identifier |var|
//@[004:053) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_if|
//@[054:055) Assignment |=|
//@[056:081) Identifier |discriminatorKeySetTwo_if|
//@[081:082) LeftSquare |[|
//@[082:094) StringComplete |'properties'|
//@[094:095) RightSquare |]|
//@[095:096) Dot |.|
//@[096:102) NewLine |\r\n\r\n\r\n|


/*
Discriminator value set 2 (loops)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |discriminatorKeySetTwo_for|
//@[036:086) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[087:088) Assignment |=|
//@[089:090) LeftSquare |[|
//@[090:093) Identifier |for|
//@[094:099) Identifier |thing|
//@[100:102) Identifier |in|
//@[103:104) LeftSquare |[|
//@[104:105) RightSquare |]|
//@[105:106) Colon |:|
//@[107:108) LeftBrace |{|
//@[108:110) NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'AzurePowerShell'|
//@[025:027) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:057) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[065:067) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
// #completionTest(86) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
//@[000:003) Identifier |var|
//@[004:041) Identifier |discriminatorKeySetTwoCompletions_for|
//@[042:043) Assignment |=|
//@[044:070) Identifier |discriminatorKeySetTwo_for|
//@[070:071) LeftSquare |[|
//@[071:072) Integer |0|
//@[072:073) RightSquare |]|
//@[073:074) Dot |.|
//@[074:084) Identifier |properties|
//@[084:085) Dot |.|
//@[085:086) Identifier |a|
//@[086:088) NewLine |\r\n|
// #completionTest(86) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.
//@[000:003) Identifier |var|
//@[004:042) Identifier |discriminatorKeySetTwoCompletions2_for|
//@[043:044) Assignment |=|
//@[045:071) Identifier |discriminatorKeySetTwo_for|
//@[071:072) LeftSquare |[|
//@[072:073) Integer |0|
//@[073:074) RightSquare |]|
//@[074:075) Dot |.|
//@[075:085) Identifier |properties|
//@[085:086) Dot |.|
//@[086:090) NewLine |\r\n\r\n|

// #completionTest(101) -> powershellPropertyAccess
//@[051:053) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
//@[000:003) Identifier |var|
//@[004:053) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_for|
//@[054:055) Assignment |=|
//@[056:082) Identifier |discriminatorKeySetTwo_for|
//@[082:083) LeftSquare |[|
//@[083:084) Integer |0|
//@[084:085) RightSquare |]|
//@[085:086) LeftSquare |[|
//@[086:098) StringComplete |'properties'|
//@[098:099) RightSquare |]|
//@[099:100) Dot |.|
//@[100:101) Identifier |a|
//@[101:103) NewLine |\r\n|
// #completionTest(101) -> powershellPropertyAccess
//@[051:053) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].
//@[000:003) Identifier |var|
//@[004:054) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_for|
//@[055:056) Assignment |=|
//@[057:083) Identifier |discriminatorKeySetTwo_for|
//@[083:084) LeftSquare |[|
//@[084:085) Integer |0|
//@[085:086) RightSquare |]|
//@[086:087) LeftSquare |[|
//@[087:099) StringComplete |'properties'|
//@[099:100) RightSquare |]|
//@[100:101) Dot |.|
//@[101:107) NewLine |\r\n\r\n\r\n|


/*
Discriminator value set 2 (filtered loops)
*/
//@[002:004) NewLine |\r\n|
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[000:008) Identifier |resource|
//@[009:038) Identifier |discriminatorKeySetTwo_for_if|
//@[039:089) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[090:091) Assignment |=|
//@[092:093) LeftSquare |[|
//@[093:096) Identifier |for|
//@[097:102) Identifier |thing|
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
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'AzurePowerShell'|
//@[025:027) NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:057) NewLine |\r\n|
  
//@[002:004) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[065:067) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
// #completionTest(92) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
//@[000:003) Identifier |var|
//@[004:044) Identifier |discriminatorKeySetTwoCompletions_for_if|
//@[045:046) Assignment |=|
//@[047:076) Identifier |discriminatorKeySetTwo_for_if|
//@[076:077) LeftSquare |[|
//@[077:078) Integer |0|
//@[078:079) RightSquare |]|
//@[079:080) Dot |.|
//@[080:090) Identifier |properties|
//@[090:091) Dot |.|
//@[091:092) Identifier |a|
//@[092:094) NewLine |\r\n|
// #completionTest(92) -> powershellPropertyAccess
//@[050:052) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.
//@[000:003) Identifier |var|
//@[004:045) Identifier |discriminatorKeySetTwoCompletions2_for_if|
//@[046:047) Assignment |=|
//@[048:077) Identifier |discriminatorKeySetTwo_for_if|
//@[077:078) LeftSquare |[|
//@[078:079) Integer |0|
//@[079:080) RightSquare |]|
//@[080:081) Dot |.|
//@[081:091) Identifier |properties|
//@[091:092) Dot |.|
//@[092:096) NewLine |\r\n\r\n|

// #completionTest(107) -> powershellPropertyAccess
//@[051:053) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
//@[000:003) Identifier |var|
//@[004:056) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_for_if|
//@[057:058) Assignment |=|
//@[059:088) Identifier |discriminatorKeySetTwo_for_if|
//@[088:089) LeftSquare |[|
//@[089:090) Integer |0|
//@[090:091) RightSquare |]|
//@[091:092) LeftSquare |[|
//@[092:104) StringComplete |'properties'|
//@[104:105) RightSquare |]|
//@[105:106) Dot |.|
//@[106:107) Identifier |a|
//@[107:109) NewLine |\r\n|
// #completionTest(107) -> powershellPropertyAccess
//@[051:053) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].
//@[000:003) Identifier |var|
//@[004:057) Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_for_if|
//@[058:059) Assignment |=|
//@[060:089) Identifier |discriminatorKeySetTwo_for_if|
//@[089:090) LeftSquare |[|
//@[090:091) Integer |0|
//@[091:092) RightSquare |]|
//@[092:093) LeftSquare |[|
//@[093:105) StringComplete |'properties'|
//@[105:106) RightSquare |]|
//@[106:107) Dot |.|
//@[107:115) NewLine |\r\n\r\n\r\n\r\n|



resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |incorrectPropertiesKey|
//@[032:082) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[086:088) NewLine |\r\n|
  kind: 'AzureCLI'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'AzureCLI'|
//@[018:022) NewLine |\r\n\r\n|

  propertes: {
//@[002:011) Identifier |propertes|
//@[011:012) Colon |:|
//@[013:014) LeftBrace |{|
//@[014:016) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var mock = incorrectPropertiesKey.p
//@[000:003) Identifier |var|
//@[004:008) Identifier |mock|
//@[009:010) Assignment |=|
//@[011:033) Identifier |incorrectPropertiesKey|
//@[033:034) Dot |.|
//@[034:035) Identifier |p|
//@[035:039) NewLine |\r\n\r\n|

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:032) Identifier |incorrectPropertiesKey2|
//@[033:083) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[084:085) Assignment |=|
//@[086:087) LeftBrace |{|
//@[087:089) NewLine |\r\n|
  kind: 'AzureCLI'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'AzureCLI'|
//@[018:020) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: ''
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:014) StringComplete |''|
//@[014:016) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    azCliVersion: '2'
//@[004:016) Identifier |azCliVersion|
//@[016:017) Colon |:|
//@[018:021) StringComplete |'2'|
//@[021:023) NewLine |\r\n|
    retentionInterval: 'PT1H'
//@[004:021) Identifier |retentionInterval|
//@[021:022) Colon |:|
//@[023:029) StringComplete |'PT1H'|
//@[029:031) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliPropertiesMinusSpecified
//@[080:082) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
    // #completionTest(22,23) -> cleanupPreferencesPlusSymbols
//@[062:064) NewLine |\r\n|
    cleanupPreference: 
//@[004:021) Identifier |cleanupPreference|
//@[021:022) Colon |:|
//@[023:027) NewLine |\r\n\r\n|

    // #completionTest(25,26) -> arrayPlusSymbols
//@[049:051) NewLine |\r\n|
    supportingScriptUris: 
//@[004:024) Identifier |supportingScriptUris|
//@[024:025) Colon |:|
//@[026:030) NewLine |\r\n\r\n|

    // #completionTest(27,28) -> objectPlusSymbols
//@[050:052) NewLine |\r\n|
    storageAccountSettings: 
//@[004:026) Identifier |storageAccountSettings|
//@[026:027) Colon |:|
//@[028:032) NewLine |\r\n\r\n|

    environmentVariables: [
//@[004:024) Identifier |environmentVariables|
//@[024:025) Colon |:|
//@[026:027) LeftSquare |[|
//@[027:029) NewLine |\r\n|
      {
//@[006:007) LeftBrace |{|
//@[007:009) NewLine |\r\n|
        // #completionTest(0,2,4,6,8) -> environmentVariableProperties
//@[070:072) NewLine |\r\n|
        
//@[008:010) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
      // #completionTest(0,1,2,3,4,5,6) -> objectPlusSymbolsWithRequiredProperties
//@[082:084) NewLine |\r\n|
      
//@[006:008) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// #completionTest(21) -> resourceTypes
//@[039:041) NewLine |\r\n|
resource missingType 
//@[000:008) Identifier |resource|
//@[009:020) Identifier |missingType|
//@[021:025) NewLine |\r\n\r\n|

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
//@[060:062) NewLine |\r\n|
resource startedTypingTypeWithQuotes 'virma'
//@[000:008) Identifier |resource|
//@[009:036) Identifier |startedTypingTypeWithQuotes|
//@[037:044) StringComplete |'virma'|
//@[044:048) NewLine |\r\n\r\n|

// #completionTest(40,41,42,43,44,45) -> resourceTypes
//@[054:056) NewLine |\r\n|
resource startedTypingTypeWithoutQuotes virma
//@[000:008) Identifier |resource|
//@[009:039) Identifier |startedTypingTypeWithoutQuotes|
//@[040:045) Identifier |virma|
//@[045:049) NewLine |\r\n\r\n|

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |dashesInPropertyNames|
//@[031:086) StringComplete |'Microsoft.ContainerService/managedClusters@2020-09-01'|
//@[087:088) Assignment |=|
//@[089:090) LeftBrace |{|
//@[090:092) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
//@[061:063) NewLine |\r\n|
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
//@[000:003) Identifier |var|
//@[004:023) Identifier |letsAccessTheDashes|
//@[024:025) Assignment |=|
//@[026:047) Identifier |dashesInPropertyNames|
//@[047:048) Dot |.|
//@[048:058) Identifier |properties|
//@[058:059) Dot |.|
//@[059:076) Identifier |autoScalerProfile|
//@[076:077) Dot |.|
//@[077:078) Identifier |s|
//@[078:080) NewLine |\r\n|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
//@[061:063) NewLine |\r\n|
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[000:003) Identifier |var|
//@[004:024) Identifier |letsAccessTheDashes2|
//@[025:026) Assignment |=|
//@[027:048) Identifier |dashesInPropertyNames|
//@[048:049) Dot |.|
//@[049:059) Identifier |properties|
//@[059:060) Dot |.|
//@[060:077) Identifier |autoScalerProfile|
//@[077:078) Dot |.|
//@[078:082) NewLine |\r\n\r\n|

/* 
Nested discriminator missing key
*/
//@[002:004) NewLine |\r\n|
resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[000:008) Identifier |resource|
//@[009:038) Identifier |nestedDiscriminatorMissingKey|
//@[039:097) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[098:099) Assignment |=|
//@[100:101) LeftBrace |{|
//@[101:103) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'l'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:015) StringComplete |'l'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    //createMode: 'Default'
//@[027:031) NewLine |\r\n\r\n|

  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(90) -> createMode
//@[036:038) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
//@[000:003) Identifier |var|
//@[004:044) Identifier |nestedDiscriminatorMissingKeyCompletions|
//@[045:046) Assignment |=|
//@[047:076) Identifier |nestedDiscriminatorMissingKey|
//@[076:077) Dot |.|
//@[077:087) Identifier |properties|
//@[087:088) Dot |.|
//@[088:090) Identifier |cr|
//@[090:092) NewLine |\r\n|
// #completionTest(92) -> createMode
//@[036:038) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[000:003) Identifier |var|
//@[004:045) Identifier |nestedDiscriminatorMissingKeyCompletions2|
//@[046:047) Assignment |=|
//@[048:077) Identifier |nestedDiscriminatorMissingKey|
//@[077:078) LeftSquare |[|
//@[078:090) StringComplete |'properties'|
//@[090:091) RightSquare |]|
//@[091:092) Dot |.|
//@[092:096) NewLine |\r\n\r\n|

// #completionTest(94) -> createModeIndexPlusSymbols
//@[052:054) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[000:003) Identifier |var|
//@[004:049) Identifier |nestedDiscriminatorMissingKeyIndexCompletions|
//@[050:051) Assignment |=|
//@[052:081) Identifier |nestedDiscriminatorMissingKey|
//@[081:082) Dot |.|
//@[082:092) Identifier |properties|
//@[092:093) LeftSquare |[|
//@[093:095) StringComplete |''|
//@[095:096) RightSquare |]|
//@[096:100) NewLine |\r\n\r\n|

/* 
Nested discriminator missing key (conditional)
*/
//@[002:004) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
//@[000:008) Identifier |resource|
//@[009:041) Identifier |nestedDiscriminatorMissingKey_if|
//@[042:100) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
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
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'l'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:015) StringComplete |'l'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    //createMode: 'Default'
//@[027:031) NewLine |\r\n\r\n|

  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(96) -> createMode
//@[036:038) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
//@[000:003) Identifier |var|
//@[004:047) Identifier |nestedDiscriminatorMissingKeyCompletions_if|
//@[048:049) Assignment |=|
//@[050:082) Identifier |nestedDiscriminatorMissingKey_if|
//@[082:083) Dot |.|
//@[083:093) Identifier |properties|
//@[093:094) Dot |.|
//@[094:096) Identifier |cr|
//@[096:098) NewLine |\r\n|
// #completionTest(98) -> createMode
//@[036:038) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].
//@[000:003) Identifier |var|
//@[004:048) Identifier |nestedDiscriminatorMissingKeyCompletions2_if|
//@[049:050) Assignment |=|
//@[051:083) Identifier |nestedDiscriminatorMissingKey_if|
//@[083:084) LeftSquare |[|
//@[084:096) StringComplete |'properties'|
//@[096:097) RightSquare |]|
//@[097:098) Dot |.|
//@[098:102) NewLine |\r\n\r\n|

// #completionTest(100) -> createModeIndexPlusSymbols_if
//@[056:058) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']
//@[000:003) Identifier |var|
//@[004:052) Identifier |nestedDiscriminatorMissingKeyIndexCompletions_if|
//@[053:054) Assignment |=|
//@[055:087) Identifier |nestedDiscriminatorMissingKey_if|
//@[087:088) Dot |.|
//@[088:098) Identifier |properties|
//@[098:099) LeftSquare |[|
//@[099:101) StringComplete |''|
//@[101:102) RightSquare |]|
//@[102:106) NewLine |\r\n\r\n|

/* 
Nested discriminator missing key (loop)
*/
//@[002:004) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:042) Identifier |nestedDiscriminatorMissingKey_for|
//@[043:101) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
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
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'l'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:015) StringComplete |'l'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    //createMode: 'Default'
//@[027:031) NewLine |\r\n\r\n|

  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
// #completionTest(101) -> createMode
//@[037:039) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
//@[000:003) Identifier |var|
//@[004:048) Identifier |nestedDiscriminatorMissingKeyCompletions_for|
//@[049:050) Assignment |=|
//@[051:084) Identifier |nestedDiscriminatorMissingKey_for|
//@[084:085) LeftSquare |[|
//@[085:086) Integer |0|
//@[086:087) RightSquare |]|
//@[087:088) Dot |.|
//@[088:098) Identifier |properties|
//@[098:099) Dot |.|
//@[099:101) Identifier |cr|
//@[101:103) NewLine |\r\n|
// #completionTest(103) -> createMode
//@[037:039) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].
//@[000:003) Identifier |var|
//@[004:049) Identifier |nestedDiscriminatorMissingKeyCompletions2_for|
//@[050:051) Assignment |=|
//@[052:085) Identifier |nestedDiscriminatorMissingKey_for|
//@[085:086) LeftSquare |[|
//@[086:087) Integer |0|
//@[087:088) RightSquare |]|
//@[088:089) LeftSquare |[|
//@[089:101) StringComplete |'properties'|
//@[101:102) RightSquare |]|
//@[102:103) Dot |.|
//@[103:107) NewLine |\r\n\r\n|

// #completionTest(105) -> createModeIndexPlusSymbols_for
//@[057:059) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']
//@[000:003) Identifier |var|
//@[004:053) Identifier |nestedDiscriminatorMissingKeyIndexCompletions_for|
//@[054:055) Assignment |=|
//@[056:089) Identifier |nestedDiscriminatorMissingKey_for|
//@[089:090) LeftSquare |[|
//@[090:091) Integer |0|
//@[091:092) RightSquare |]|
//@[092:093) Dot |.|
//@[093:103) Identifier |properties|
//@[103:104) LeftSquare |[|
//@[104:106) StringComplete |''|
//@[106:107) RightSquare |]|
//@[107:113) NewLine |\r\n\r\n\r\n|


/* 
Nested discriminator missing key (filtered loop)
*/
//@[002:004) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[000:008) Identifier |resource|
//@[009:045) Identifier |nestedDiscriminatorMissingKey_for_if|
//@[046:104) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
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
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'l'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:015) StringComplete |'l'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    //createMode: 'Default'
//@[027:031) NewLine |\r\n\r\n|

  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
// #completionTest(107) -> createMode
//@[037:039) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
//@[000:003) Identifier |var|
//@[004:051) Identifier |nestedDiscriminatorMissingKeyCompletions_for_if|
//@[052:053) Assignment |=|
//@[054:090) Identifier |nestedDiscriminatorMissingKey_for_if|
//@[090:091) LeftSquare |[|
//@[091:092) Integer |0|
//@[092:093) RightSquare |]|
//@[093:094) Dot |.|
//@[094:104) Identifier |properties|
//@[104:105) Dot |.|
//@[105:107) Identifier |cr|
//@[107:109) NewLine |\r\n|
// #completionTest(109) -> createMode
//@[037:039) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].
//@[000:003) Identifier |var|
//@[004:052) Identifier |nestedDiscriminatorMissingKeyCompletions2_for_if|
//@[053:054) Assignment |=|
//@[055:091) Identifier |nestedDiscriminatorMissingKey_for_if|
//@[091:092) LeftSquare |[|
//@[092:093) Integer |0|
//@[093:094) RightSquare |]|
//@[094:095) LeftSquare |[|
//@[095:107) StringComplete |'properties'|
//@[107:108) RightSquare |]|
//@[108:109) Dot |.|
//@[109:113) NewLine |\r\n\r\n|

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
//@[060:062) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']
//@[000:003) Identifier |var|
//@[004:056) Identifier |nestedDiscriminatorMissingKeyIndexCompletions_for_if|
//@[057:058) Assignment |=|
//@[059:095) Identifier |nestedDiscriminatorMissingKey_for_if|
//@[095:096) LeftSquare |[|
//@[096:097) Integer |0|
//@[097:098) RightSquare |]|
//@[098:099) Dot |.|
//@[099:109) Identifier |properties|
//@[109:110) LeftSquare |[|
//@[110:112) StringComplete |''|
//@[112:113) RightSquare |]|
//@[113:119) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator
*/
//@[002:004) NewLine |\r\n|
resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |nestedDiscriminator|
//@[029:087) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'l'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:015) StringComplete |'l'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    createMode: 'Default'
//@[004:014) Identifier |createMode|
//@[014:015) Colon |:|
//@[016:025) StringComplete |'Default'|
//@[025:029) NewLine |\r\n\r\n|

  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(69) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
//@[000:003) Identifier |var|
//@[004:034) Identifier |nestedDiscriminatorCompletions|
//@[035:036) Assignment |=|
//@[037:056) Identifier |nestedDiscriminator|
//@[056:057) Dot |.|
//@[057:067) Identifier |properties|
//@[067:068) Dot |.|
//@[068:069) Identifier |a|
//@[069:071) NewLine |\r\n|
// #completionTest(73) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
//@[000:003) Identifier |var|
//@[004:035) Identifier |nestedDiscriminatorCompletions2|
//@[036:037) Assignment |=|
//@[038:057) Identifier |nestedDiscriminator|
//@[057:058) LeftSquare |[|
//@[058:070) StringComplete |'properties'|
//@[070:071) RightSquare |]|
//@[071:072) Dot |.|
//@[072:073) Identifier |a|
//@[073:075) NewLine |\r\n|
// #completionTest(69) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[000:003) Identifier |var|
//@[004:035) Identifier |nestedDiscriminatorCompletions3|
//@[036:037) Assignment |=|
//@[038:057) Identifier |nestedDiscriminator|
//@[057:058) Dot |.|
//@[058:068) Identifier |properties|
//@[068:069) Dot |.|
//@[069:071) NewLine |\r\n|
// #completionTest(72) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[000:003) Identifier |var|
//@[004:035) Identifier |nestedDiscriminatorCompletions4|
//@[036:037) Assignment |=|
//@[038:057) Identifier |nestedDiscriminator|
//@[057:058) LeftSquare |[|
//@[058:070) StringComplete |'properties'|
//@[070:071) RightSquare |]|
//@[071:072) Dot |.|
//@[072:076) NewLine |\r\n\r\n|

// #completionTest(79) -> defaultCreateModeIndexes
//@[050:052) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[000:003) Identifier |var|
//@[004:044) Identifier |nestedDiscriminatorArrayIndexCompletions|
//@[045:046) Assignment |=|
//@[047:066) Identifier |nestedDiscriminator|
//@[066:067) Dot |.|
//@[067:077) Identifier |properties|
//@[077:078) LeftSquare |[|
//@[078:079) Identifier |a|
//@[079:080) RightSquare |]|
//@[080:084) NewLine |\r\n\r\n|

/*
Nested discriminator (conditional)
*/
//@[002:004) NewLine |\r\n|
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |nestedDiscriminator_if|
//@[032:090) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[091:092) Assignment |=|
//@[093:095) Identifier |if|
//@[095:096) LeftParen |(|
//@[096:100) TrueKeyword |true|
//@[100:101) RightParen |)|
//@[102:103) LeftBrace |{|
//@[103:105) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'l'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:015) StringComplete |'l'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    createMode: 'Default'
//@[004:014) Identifier |createMode|
//@[014:015) Colon |:|
//@[016:025) StringComplete |'Default'|
//@[025:029) NewLine |\r\n\r\n|

  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// #completionTest(75) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
//@[000:003) Identifier |var|
//@[004:037) Identifier |nestedDiscriminatorCompletions_if|
//@[038:039) Assignment |=|
//@[040:062) Identifier |nestedDiscriminator_if|
//@[062:063) Dot |.|
//@[063:073) Identifier |properties|
//@[073:074) Dot |.|
//@[074:075) Identifier |a|
//@[075:077) NewLine |\r\n|
// #completionTest(79) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
//@[000:003) Identifier |var|
//@[004:038) Identifier |nestedDiscriminatorCompletions2_if|
//@[039:040) Assignment |=|
//@[041:063) Identifier |nestedDiscriminator_if|
//@[063:064) LeftSquare |[|
//@[064:076) StringComplete |'properties'|
//@[076:077) RightSquare |]|
//@[077:078) Dot |.|
//@[078:079) Identifier |a|
//@[079:081) NewLine |\r\n|
// #completionTest(75) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
//@[000:003) Identifier |var|
//@[004:038) Identifier |nestedDiscriminatorCompletions3_if|
//@[039:040) Assignment |=|
//@[041:063) Identifier |nestedDiscriminator_if|
//@[063:064) Dot |.|
//@[064:074) Identifier |properties|
//@[074:075) Dot |.|
//@[075:077) NewLine |\r\n|
// #completionTest(78) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].
//@[000:003) Identifier |var|
//@[004:038) Identifier |nestedDiscriminatorCompletions4_if|
//@[039:040) Assignment |=|
//@[041:063) Identifier |nestedDiscriminator_if|
//@[063:064) LeftSquare |[|
//@[064:076) StringComplete |'properties'|
//@[076:077) RightSquare |]|
//@[077:078) Dot |.|
//@[078:082) NewLine |\r\n\r\n|

// #completionTest(85) -> defaultCreateModeIndexes_if
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]
//@[000:003) Identifier |var|
//@[004:047) Identifier |nestedDiscriminatorArrayIndexCompletions_if|
//@[048:049) Assignment |=|
//@[050:072) Identifier |nestedDiscriminator_if|
//@[072:073) Dot |.|
//@[073:083) Identifier |properties|
//@[083:084) LeftSquare |[|
//@[084:085) Identifier |a|
//@[085:086) RightSquare |]|
//@[086:092) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator (loop)
*/
//@[002:004) NewLine |\r\n|
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:032) Identifier |nestedDiscriminator_for|
//@[033:091) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[092:093) Assignment |=|
//@[094:095) LeftSquare |[|
//@[095:098) Identifier |for|
//@[099:104) Identifier |thing|
//@[105:107) Identifier |in|
//@[108:109) LeftSquare |[|
//@[109:110) RightSquare |]|
//@[110:111) Colon |:|
//@[112:113) LeftBrace |{|
//@[113:115) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'l'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:015) StringComplete |'l'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    createMode: 'Default'
//@[004:014) Identifier |createMode|
//@[014:015) Colon |:|
//@[016:025) StringComplete |'Default'|
//@[025:029) NewLine |\r\n\r\n|

  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
// #completionTest(80) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
//@[000:003) Identifier |var|
//@[004:038) Identifier |nestedDiscriminatorCompletions_for|
//@[039:040) Assignment |=|
//@[041:064) Identifier |nestedDiscriminator_for|
//@[064:065) LeftSquare |[|
//@[065:066) Integer |0|
//@[066:067) RightSquare |]|
//@[067:068) Dot |.|
//@[068:078) Identifier |properties|
//@[078:079) Dot |.|
//@[079:080) Identifier |a|
//@[080:082) NewLine |\r\n|
// #completionTest(84) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
//@[000:003) Identifier |var|
//@[004:039) Identifier |nestedDiscriminatorCompletions2_for|
//@[040:041) Assignment |=|
//@[042:065) Identifier |nestedDiscriminator_for|
//@[065:066) LeftSquare |[|
//@[066:067) Integer |0|
//@[067:068) RightSquare |]|
//@[068:069) LeftSquare |[|
//@[069:081) StringComplete |'properties'|
//@[081:082) RightSquare |]|
//@[082:083) Dot |.|
//@[083:084) Identifier |a|
//@[084:086) NewLine |\r\n|
// #completionTest(80) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
//@[000:003) Identifier |var|
//@[004:039) Identifier |nestedDiscriminatorCompletions3_for|
//@[040:041) Assignment |=|
//@[042:065) Identifier |nestedDiscriminator_for|
//@[065:066) LeftSquare |[|
//@[066:067) Integer |0|
//@[067:068) RightSquare |]|
//@[068:069) Dot |.|
//@[069:079) Identifier |properties|
//@[079:080) Dot |.|
//@[080:082) NewLine |\r\n|
// #completionTest(83) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].
//@[000:003) Identifier |var|
//@[004:039) Identifier |nestedDiscriminatorCompletions4_for|
//@[040:041) Assignment |=|
//@[042:065) Identifier |nestedDiscriminator_for|
//@[065:066) LeftSquare |[|
//@[066:067) Integer |0|
//@[067:068) RightSquare |]|
//@[068:069) LeftSquare |[|
//@[069:081) StringComplete |'properties'|
//@[081:082) RightSquare |]|
//@[082:083) Dot |.|
//@[083:087) NewLine |\r\n\r\n|

// #completionTest(90) -> defaultCreateModeIndexes_for
//@[054:056) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]
//@[000:003) Identifier |var|
//@[004:048) Identifier |nestedDiscriminatorArrayIndexCompletions_for|
//@[049:050) Assignment |=|
//@[051:074) Identifier |nestedDiscriminator_for|
//@[074:075) LeftSquare |[|
//@[075:076) Integer |0|
//@[076:077) RightSquare |]|
//@[077:078) Dot |.|
//@[078:088) Identifier |properties|
//@[088:089) LeftSquare |[|
//@[089:090) Identifier |a|
//@[090:091) RightSquare |]|
//@[091:097) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator (filtered loop)
*/
//@[002:004) NewLine |\r\n|
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |nestedDiscriminator_for_if|
//@[036:094) StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[095:096) Assignment |=|
//@[097:098) LeftSquare |[|
//@[098:101) Identifier |for|
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
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  location: 'l'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:015) StringComplete |'l'|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    createMode: 'Default'
//@[004:014) Identifier |createMode|
//@[014:015) Colon |:|
//@[016:025) StringComplete |'Default'|
//@[025:029) NewLine |\r\n\r\n|

  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
// #completionTest(86) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
//@[000:003) Identifier |var|
//@[004:041) Identifier |nestedDiscriminatorCompletions_for_if|
//@[042:043) Assignment |=|
//@[044:070) Identifier |nestedDiscriminator_for_if|
//@[070:071) LeftSquare |[|
//@[071:072) Integer |0|
//@[072:073) RightSquare |]|
//@[073:074) Dot |.|
//@[074:084) Identifier |properties|
//@[084:085) Dot |.|
//@[085:086) Identifier |a|
//@[086:088) NewLine |\r\n|
// #completionTest(90) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
//@[000:003) Identifier |var|
//@[004:042) Identifier |nestedDiscriminatorCompletions2_for_if|
//@[043:044) Assignment |=|
//@[045:071) Identifier |nestedDiscriminator_for_if|
//@[071:072) LeftSquare |[|
//@[072:073) Integer |0|
//@[073:074) RightSquare |]|
//@[074:075) LeftSquare |[|
//@[075:087) StringComplete |'properties'|
//@[087:088) RightSquare |]|
//@[088:089) Dot |.|
//@[089:090) Identifier |a|
//@[090:092) NewLine |\r\n|
// #completionTest(86) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
//@[000:003) Identifier |var|
//@[004:042) Identifier |nestedDiscriminatorCompletions3_for_if|
//@[043:044) Assignment |=|
//@[045:071) Identifier |nestedDiscriminator_for_if|
//@[071:072) LeftSquare |[|
//@[072:073) Integer |0|
//@[073:074) RightSquare |]|
//@[074:075) Dot |.|
//@[075:085) Identifier |properties|
//@[085:086) Dot |.|
//@[086:088) NewLine |\r\n|
// #completionTest(89) -> defaultCreateModeProperties
//@[053:055) NewLine |\r\n|
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].
//@[000:003) Identifier |var|
//@[004:042) Identifier |nestedDiscriminatorCompletions4_for_if|
//@[043:044) Assignment |=|
//@[045:071) Identifier |nestedDiscriminator_for_if|
//@[071:072) LeftSquare |[|
//@[072:073) Integer |0|
//@[073:074) RightSquare |]|
//@[074:075) LeftSquare |[|
//@[075:087) StringComplete |'properties'|
//@[087:088) RightSquare |]|
//@[088:089) Dot |.|
//@[089:093) NewLine |\r\n\r\n|

// #completionTest(96) -> defaultCreateModeIndexes_for_if
//@[057:059) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]
//@[000:003) Identifier |var|
//@[004:051) Identifier |nestedDiscriminatorArrayIndexCompletions_for_if|
//@[052:053) Assignment |=|
//@[054:080) Identifier |nestedDiscriminator_for_if|
//@[080:081) LeftSquare |[|
//@[081:082) Integer |0|
//@[082:083) RightSquare |]|
//@[083:084) Dot |.|
//@[084:094) Identifier |properties|
//@[094:095) LeftSquare |[|
//@[095:096) Identifier |a|
//@[096:097) RightSquare |]|
//@[097:105) NewLine |\r\n\r\n\r\n\r\n|



// sample resource to validate completions on the next declarations
//@[067:069) NewLine |\r\n|
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
//@[000:008) Identifier |resource|
//@[009:042) Identifier |nestedPropertyAccessOnConditional|
//@[043:089) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[090:091) Assignment |=|
//@[092:094) Identifier |if|
//@[094:095) LeftParen |(|
//@[095:099) TrueKeyword |true|
//@[099:100) RightParen |)|
//@[101:102) LeftBrace |{|
//@[102:104) NewLine |\r\n|
  location: 'test'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:018) StringComplete |'test'|
//@[018:020) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    additionalCapabilities: {
//@[004:026) Identifier |additionalCapabilities|
//@[026:027) Colon |:|
//@[028:029) LeftBrace |{|
//@[029:031) NewLine |\r\n|
      
//@[006:008) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
// this validates that we can get nested property access completions on a conditional resource
//@[094:096) NewLine |\r\n|
//#completionTest(56) -> vmProperties
//@[037:039) NewLine |\r\n|
var sigh = nestedPropertyAccessOnConditional.properties.
//@[000:003) Identifier |var|
//@[004:008) Identifier |sigh|
//@[009:010) Assignment |=|
//@[011:044) Identifier |nestedPropertyAccessOnConditional|
//@[044:045) Dot |.|
//@[045:055) Identifier |properties|
//@[055:056) Dot |.|
//@[056:060) NewLine |\r\n\r\n|

/*
  boolean property value completions
*/ 
//@[003:005) NewLine |\r\n|
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:036) Identifier |booleanPropertyPartialValue|
//@[037:094) StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[095:096) Assignment |=|
//@[097:098) LeftBrace |{|
//@[098:100) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
//@[065:067) NewLine |\r\n|
    autoUpgradeMinorVersion: t
//@[004:027) Identifier |autoUpgradeMinorVersion|
//@[027:028) Colon |:|
//@[029:030) Identifier |t|
//@[030:032) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |selfScope|
//@[019:050) StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[051:052) Assignment |=|
//@[053:054) LeftBrace |{|
//@[054:056) NewLine |\r\n|
  name: 'selfScope'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'selfScope'|
//@[019:021) NewLine |\r\n|
  scope: selfScope
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:018) Identifier |selfScope|
//@[018:020) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var notAResource = {
//@[000:003) Identifier |var|
//@[004:016) Identifier |notAResource|
//@[017:018) Assignment |=|
//@[019:020) LeftBrace |{|
//@[020:022) NewLine |\r\n|
  im: 'not'
//@[002:004) Identifier |im|
//@[004:005) Colon |:|
//@[006:011) StringComplete |'not'|
//@[011:013) NewLine |\r\n|
  a: 'resource!'
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:016) StringComplete |'resource!'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |invalidScope|
//@[022:053) StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'invalidScope'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'invalidScope'|
//@[022:024) NewLine |\r\n|
  scope: notAResource
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:021) Identifier |notAResource|
//@[021:023) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
//@[000:008) Identifier |resource|
//@[009:022) Identifier |invalidScope2|
//@[023:054) StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[055:056) Assignment |=|
//@[057:058) LeftBrace |{|
//@[058:060) NewLine |\r\n|
  name: 'invalidScope2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'invalidScope2'|
//@[023:025) NewLine |\r\n|
  scope: resourceGroup()
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceGroup|
//@[022:023) LeftParen |(|
//@[023:024) RightParen |)|
//@[024:026) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
//@[000:008) Identifier |resource|
//@[009:022) Identifier |invalidScope3|
//@[023:054) StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[055:056) Assignment |=|
//@[057:058) LeftBrace |{|
//@[058:060) NewLine |\r\n|
  name: 'invalidScope3'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'invalidScope3'|
//@[023:025) NewLine |\r\n|
  scope: subscription()
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:021) Identifier |subscription|
//@[021:022) LeftParen |(|
//@[022:023) RightParen |)|
//@[023:025) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |invalidDuplicateName1|
//@[031:064) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[065:066) Assignment |=|
//@[067:068) LeftBrace |{|
//@[068:070) NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringComplete |'invalidDuplicateName'|
//@[030:032) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |invalidDuplicateName2|
//@[031:064) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[065:066) Assignment |=|
//@[067:068) LeftBrace |{|
//@[068:070) NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringComplete |'invalidDuplicateName'|
//@[030:032) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |invalidDuplicateName3|
//@[031:064) StringComplete |'Mock.Rp/mockResource@2019-01-01'|
//@[065:066) Assignment |=|
//@[067:068) LeftBrace |{|
//@[068:070) NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringComplete |'invalidDuplicateName'|
//@[030:032) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:062) Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[063:096) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[097:098) Assignment |=|
//@[099:100) LeftBrace |{|
//@[100:102) NewLine |\r\n|
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:063) StringComplete |'validResourceForInvalidExtensionResourceDuplicateName'|
//@[063:065) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:047) Identifier |invalidExtensionResourceDuplicateName1|
//@[048:084) StringComplete |'Mock.Rp/mockExtResource@2020-01-01'|
//@[085:086) Assignment |=|
//@[087:088) LeftBrace |{|
//@[088:090) NewLine |\r\n|
  name: 'invalidExtensionResourceDuplicateName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:047) StringComplete |'invalidExtensionResourceDuplicateName'|
//@[047:049) NewLine |\r\n|
  scope: validResourceForInvalidExtensionResourceDuplicateName
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:062) Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[062:064) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
//@[000:008) Identifier |resource|
//@[009:047) Identifier |invalidExtensionResourceDuplicateName2|
//@[048:084) StringComplete |'Mock.Rp/mockExtResource@2019-01-01'|
//@[085:086) Assignment |=|
//@[087:088) LeftBrace |{|
//@[088:090) NewLine |\r\n|
  name: 'invalidExtensionResourceDuplicateName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:047) StringComplete |'invalidExtensionResourceDuplicateName'|
//@[047:049) NewLine |\r\n|
  scope: validResourceForInvalidExtensionResourceDuplicateName
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:062) Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[062:064) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

@concat('foo', 'bar')
//@[000:001) At |@|
//@[001:007) Identifier |concat|
//@[007:008) LeftParen |(|
//@[008:013) StringComplete |'foo'|
//@[013:014) Comma |,|
//@[015:020) StringComplete |'bar'|
//@[020:021) RightParen |)|
//@[021:023) NewLine |\r\n|
@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:011) NewLine |\r\n|
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |invalidDecorator|
//@[026:063) StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  name: 'invalidDecorator'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:026) StringComplete |'invalidDecorator'|
//@[026:028) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |cyclicRes|
//@[019:060) StringComplete |'Mock.Rp/mockExistingResource@2020-01-01'|
//@[061:062) Assignment |=|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  name: 'cyclicRes'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'cyclicRes'|
//@[019:021) NewLine |\r\n|
  scope: cyclicRes
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:018) Identifier |cyclicRes|
//@[018:020) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
//@[000:008) Identifier |resource|
//@[009:026) Identifier |cyclicExistingRes|
//@[027:068) StringComplete |'Mock.Rp/mockExistingResource@2020-01-01'|
//@[069:077) Identifier |existing|
//@[078:079) Assignment |=|
//@[080:081) LeftBrace |{|
//@[081:083) NewLine |\r\n|
  name: 'cyclicExistingRes'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) StringComplete |'cyclicExistingRes'|
//@[027:029) NewLine |\r\n|
  scope: cyclicExistingRes
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:026) Identifier |cyclicExistingRes|
//@[026:028) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// loop parsing cases
//@[021:023) NewLine |\r\n|
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []
//@[000:008) Identifier |resource|
//@[009:027) Identifier |expectedForKeyword|
//@[028:074) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[075:076) Assignment |=|
//@[077:078) LeftSquare |[|
//@[078:079) RightSquare |]|
//@[079:083) NewLine |\r\n\r\n|

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]
//@[000:008) Identifier |resource|
//@[009:028) Identifier |expectedForKeyword2|
//@[029:075) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftSquare |[|
//@[079:080) Identifier |f|
//@[080:081) RightSquare |]|
//@[081:085) NewLine |\r\n\r\n|

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]
//@[000:008) Identifier |resource|
//@[009:024) Identifier |expectedLoopVar|
//@[025:071) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:073) Assignment |=|
//@[074:075) LeftSquare |[|
//@[075:078) Identifier |for|
//@[078:079) RightSquare |]|
//@[079:083) NewLine |\r\n\r\n|

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]
//@[000:008) Identifier |resource|
//@[009:026) Identifier |expectedInKeyword|
//@[027:073) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[074:075) Assignment |=|
//@[076:077) LeftSquare |[|
//@[077:080) Identifier |for|
//@[081:082) Identifier |x|
//@[082:083) RightSquare |]|
//@[083:087) NewLine |\r\n\r\n|

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]
//@[000:008) Identifier |resource|
//@[009:027) Identifier |expectedInKeyword2|
//@[028:074) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[075:076) Assignment |=|
//@[077:078) LeftSquare |[|
//@[078:081) Identifier |for|
//@[082:083) Identifier |x|
//@[084:085) Identifier |b|
//@[085:086) RightSquare |]|
//@[086:090) NewLine |\r\n\r\n|

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]
//@[000:008) Identifier |resource|
//@[009:032) Identifier |expectedArrayExpression|
//@[033:079) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[080:081) Assignment |=|
//@[082:083) LeftSquare |[|
//@[083:086) Identifier |for|
//@[087:088) Identifier |x|
//@[089:091) Identifier |in|
//@[091:092) RightSquare |]|
//@[092:096) NewLine |\r\n\r\n|

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]
//@[000:008) Identifier |resource|
//@[009:022) Identifier |expectedColon|
//@[023:069) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[070:071) Assignment |=|
//@[072:073) LeftSquare |[|
//@[073:076) Identifier |for|
//@[077:078) Identifier |x|
//@[079:081) Identifier |in|
//@[082:083) Identifier |y|
//@[083:084) RightSquare |]|
//@[084:088) NewLine |\r\n\r\n|

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]
//@[000:008) Identifier |resource|
//@[009:025) Identifier |expectedLoopBody|
//@[026:072) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:074) Assignment |=|
//@[075:076) LeftSquare |[|
//@[076:079) Identifier |for|
//@[080:081) Identifier |x|
//@[082:084) Identifier |in|
//@[085:086) Identifier |y|
//@[086:087) Colon |:|
//@[087:088) RightSquare |]|
//@[088:092) NewLine |\r\n\r\n|

// loop index parsing cases
//@[027:029) NewLine |\r\n|
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]
//@[000:008) Identifier |resource|
//@[009:029) Identifier |expectedLoopItemName|
//@[030:069) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:071) Assignment |=|
//@[072:073) LeftSquare |[|
//@[073:076) Identifier |for|
//@[077:078) LeftParen |(|
//@[078:079) RightParen |)|
//@[079:080) RightSquare |]|
//@[080:084) NewLine |\r\n\r\n|

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (
//@[000:008) Identifier |resource|
//@[009:030) Identifier |expectedLoopItemName2|
//@[031:070) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[071:072) Assignment |=|
//@[073:074) LeftSquare |[|
//@[074:077) Identifier |for|
//@[078:079) LeftParen |(|
//@[079:083) NewLine |\r\n\r\n|

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]
//@[000:008) Identifier |resource|
//@[009:022) Identifier |expectedComma|
//@[023:062) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[063:064) Assignment |=|
//@[065:066) LeftSquare |[|
//@[066:069) Identifier |for|
//@[070:071) LeftParen |(|
//@[071:072) Identifier |x|
//@[072:073) RightParen |)|
//@[073:074) RightSquare |]|
//@[074:078) NewLine |\r\n\r\n|

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]
//@[000:008) Identifier |resource|
//@[009:030) Identifier |expectedLoopIndexName|
//@[031:070) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[071:072) Assignment |=|
//@[073:074) LeftSquare |[|
//@[074:077) Identifier |for|
//@[078:079) LeftParen |(|
//@[079:080) Identifier |x|
//@[080:081) Comma |,|
//@[082:083) RightParen |)|
//@[083:084) RightSquare |]|
//@[084:088) NewLine |\r\n\r\n|

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]
//@[000:008) Identifier |resource|
//@[009:027) Identifier |expectedInKeyword3|
//@[028:067) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[068:069) Assignment |=|
//@[070:071) LeftSquare |[|
//@[071:074) Identifier |for|
//@[075:076) LeftParen |(|
//@[076:077) Identifier |x|
//@[077:078) Comma |,|
//@[079:080) Identifier |y|
//@[080:081) RightParen |)|
//@[081:082) RightSquare |]|
//@[082:086) NewLine |\r\n\r\n|

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]
//@[000:008) Identifier |resource|
//@[009:027) Identifier |expectedInKeyword4|
//@[028:067) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[068:069) Assignment |=|
//@[070:071) LeftSquare |[|
//@[071:074) Identifier |for|
//@[075:076) LeftParen |(|
//@[076:077) Identifier |x|
//@[077:078) Comma |,|
//@[079:080) Identifier |y|
//@[080:081) RightParen |)|
//@[082:083) Identifier |z|
//@[083:084) RightSquare |]|
//@[084:088) NewLine |\r\n\r\n|

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]
//@[000:008) Identifier |resource|
//@[009:033) Identifier |expectedArrayExpression2|
//@[034:073) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[074:075) Assignment |=|
//@[076:077) LeftSquare |[|
//@[077:080) Identifier |for|
//@[081:082) LeftParen |(|
//@[082:083) Identifier |x|
//@[083:084) Comma |,|
//@[085:086) Identifier |y|
//@[086:087) RightParen |)|
//@[088:090) Identifier |in|
//@[091:092) RightSquare |]|
//@[092:096) NewLine |\r\n\r\n|

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]
//@[000:008) Identifier |resource|
//@[009:023) Identifier |expectedColon2|
//@[024:063) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[064:065) Assignment |=|
//@[066:067) LeftSquare |[|
//@[067:070) Identifier |for|
//@[071:072) LeftParen |(|
//@[072:073) Identifier |x|
//@[073:074) Comma |,|
//@[075:076) Identifier |y|
//@[076:077) RightParen |)|
//@[078:080) Identifier |in|
//@[081:082) Identifier |z|
//@[082:083) RightSquare |]|
//@[083:087) NewLine |\r\n\r\n|

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]
//@[000:008) Identifier |resource|
//@[009:026) Identifier |expectedLoopBody2|
//@[027:066) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[067:068) Assignment |=|
//@[069:070) LeftSquare |[|
//@[070:073) Identifier |for|
//@[074:075) LeftParen |(|
//@[075:076) Identifier |x|
//@[076:077) Comma |,|
//@[078:079) Identifier |y|
//@[079:080) RightParen |)|
//@[081:083) Identifier |in|
//@[084:085) Identifier |z|
//@[085:086) Colon |:|
//@[086:087) RightSquare |]|
//@[087:091) NewLine |\r\n\r\n|

// loop filter parsing cases
//@[028:030) NewLine |\r\n|
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
//@[000:008) Identifier |resource|
//@[009:036) Identifier |expectedLoopFilterOpenParen|
//@[037:083) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[084:085) Assignment |=|
//@[086:087) LeftSquare |[|
//@[087:090) Identifier |for|
//@[091:092) Identifier |x|
//@[093:095) Identifier |in|
//@[096:097) Identifier |y|
//@[097:098) Colon |:|
//@[099:101) Identifier |if|
//@[101:102) RightSquare |]|
//@[102:104) NewLine |\r\n|
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]
//@[000:008) Identifier |resource|
//@[009:037) Identifier |expectedLoopFilterOpenParen2|
//@[038:077) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[078:079) Assignment |=|
//@[080:081) LeftSquare |[|
//@[081:084) Identifier |for|
//@[085:086) LeftParen |(|
//@[086:087) Identifier |x|
//@[087:088) Comma |,|
//@[089:090) Identifier |y|
//@[090:091) RightParen |)|
//@[092:094) Identifier |in|
//@[095:096) Identifier |z|
//@[096:097) Colon |:|
//@[098:100) Identifier |if|
//@[100:101) RightSquare |]|
//@[101:105) NewLine |\r\n\r\n|

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
//@[000:008) Identifier |resource|
//@[009:043) Identifier |expectedLoopFilterPredicateAndBody|
//@[044:090) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[091:092) Assignment |=|
//@[093:094) LeftSquare |[|
//@[094:097) Identifier |for|
//@[098:099) Identifier |x|
//@[100:102) Identifier |in|
//@[103:104) Identifier |y|
//@[104:105) Colon |:|
//@[106:108) Identifier |if|
//@[108:109) LeftParen |(|
//@[109:110) RightParen |)|
//@[110:111) RightSquare |]|
//@[111:113) NewLine |\r\n|
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]
//@[000:008) Identifier |resource|
//@[009:044) Identifier |expectedLoopFilterPredicateAndBody2|
//@[045:084) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[085:086) Assignment |=|
//@[087:088) LeftSquare |[|
//@[088:091) Identifier |for|
//@[092:093) LeftParen |(|
//@[093:094) Identifier |x|
//@[094:095) Comma |,|
//@[096:097) Identifier |y|
//@[097:098) RightParen |)|
//@[099:101) Identifier |in|
//@[102:103) Identifier |z|
//@[103:104) Colon |:|
//@[105:107) Identifier |if|
//@[107:108) LeftParen |(|
//@[108:109) RightParen |)|
//@[109:110) RightSquare |]|
//@[110:114) NewLine |\r\n\r\n|

// wrong body type
//@[018:020) NewLine |\r\n|
var emptyArray = []
//@[000:003) Identifier |var|
//@[004:014) Identifier |emptyArray|
//@[015:016) Assignment |=|
//@[017:018) LeftSquare |[|
//@[018:019) RightSquare |]|
//@[019:021) NewLine |\r\n|
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
//@[000:008) Identifier |resource|
//@[009:026) Identifier |wrongLoopBodyType|
//@[027:073) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[074:075) Assignment |=|
//@[076:077) LeftSquare |[|
//@[077:080) Identifier |for|
//@[081:082) Identifier |x|
//@[083:085) Identifier |in|
//@[086:096) Identifier |emptyArray|
//@[096:097) Colon |:|
//@[097:098) Integer |4|
//@[098:099) RightSquare |]|
//@[099:101) NewLine |\r\n|
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]
//@[000:008) Identifier |resource|
//@[009:027) Identifier |wrongLoopBodyType2|
//@[028:074) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[075:076) Assignment |=|
//@[077:078) LeftSquare |[|
//@[078:081) Identifier |for|
//@[082:083) LeftParen |(|
//@[083:084) Identifier |x|
//@[085:086) Comma |,|
//@[086:087) Identifier |i|
//@[087:088) RightParen |)|
//@[089:091) Identifier |in|
//@[092:102) Identifier |emptyArray|
//@[102:103) Colon |:|
//@[103:104) Integer |4|
//@[104:105) RightSquare |]|
//@[105:109) NewLine |\r\n\r\n|

// duplicate variable in the same scope
//@[039:041) NewLine |\r\n|
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
//@[000:008) Identifier |resource|
//@[009:029) Identifier |itemAndIndexSameName|
//@[030:071) StringComplete |'Microsoft.AAD/domainServices@2020-01-01'|
//@[072:073) Assignment |=|
//@[074:075) LeftSquare |[|
//@[075:078) Identifier |for|
//@[079:080) LeftParen |(|
//@[080:084) Identifier |same|
//@[084:085) Comma |,|
//@[086:090) Identifier |same|
//@[090:091) RightParen |)|
//@[092:094) Identifier |in|
//@[095:105) Identifier |emptyArray|
//@[105:106) Colon |:|
//@[107:108) LeftBrace |{|
//@[108:110) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// errors in the array expression
//@[033:035) NewLine |\r\n|
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |arrayExpressionErrors|
//@[031:077) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[078:079) Assignment |=|
//@[080:081) LeftSquare |[|
//@[081:084) Identifier |for|
//@[085:092) Identifier |account|
//@[093:095) Identifier |in|
//@[096:101) Identifier |union|
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
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |arrayExpressionErrors2|
//@[032:078) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[079:080) Assignment |=|
//@[081:082) LeftSquare |[|
//@[082:085) Identifier |for|
//@[086:087) LeftParen |(|
//@[087:094) Identifier |account|
//@[094:095) Comma |,|
//@[095:096) Identifier |k|
//@[096:097) RightParen |)|
//@[098:100) Identifier |in|
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
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// wrong array type
//@[019:021) NewLine |\r\n|
var notAnArray = true
//@[000:003) Identifier |var|
//@[004:014) Identifier |notAnArray|
//@[015:016) Assignment |=|
//@[017:021) TrueKeyword |true|
//@[021:023) NewLine |\r\n|
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
//@[000:008) Identifier |resource|
//@[009:023) Identifier |wrongArrayType|
//@[024:070) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[071:072) Assignment |=|
//@[073:074) LeftSquare |[|
//@[074:077) Identifier |for|
//@[078:085) Identifier |account|
//@[086:088) Identifier |in|
//@[089:099) Identifier |notAnArray|
//@[099:100) Colon |:|
//@[101:102) LeftBrace |{|
//@[102:104) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
//@[000:008) Identifier |resource|
//@[009:024) Identifier |wrongArrayType2|
//@[025:071) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:073) Assignment |=|
//@[074:075) LeftSquare |[|
//@[075:078) Identifier |for|
//@[079:080) LeftParen |(|
//@[080:087) Identifier |account|
//@[087:088) Comma |,|
//@[088:089) Identifier |i|
//@[089:090) RightParen |)|
//@[091:093) Identifier |in|
//@[094:104) Identifier |notAnArray|
//@[104:105) Colon |:|
//@[106:107) LeftBrace |{|
//@[107:109) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// wrong filter expression type
//@[031:033) NewLine |\r\n|
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |wrongFilterExpressionType|
//@[035:081) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[082:083) Assignment |=|
//@[084:085) LeftSquare |[|
//@[085:088) Identifier |for|
//@[089:096) Identifier |account|
//@[097:099) Identifier |in|
//@[100:110) Identifier |emptyArray|
//@[110:111) Colon |:|
//@[112:114) Identifier |if|
//@[114:115) LeftParen |(|
//@[115:116) Integer |4|
//@[116:117) RightParen |)|
//@[118:119) LeftBrace |{|
//@[119:121) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
//@[000:008) Identifier |resource|
//@[009:035) Identifier |wrongFilterExpressionType2|
//@[036:082) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftSquare |[|
//@[086:089) Identifier |for|
//@[090:091) LeftParen |(|
//@[091:098) Identifier |account|
//@[098:099) Comma |,|
//@[099:100) Identifier |i|
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
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// missing required properties
//@[030:032) NewLine |\r\n|
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |missingRequiredProperties|
//@[035:081) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[082:083) Assignment |=|
//@[084:085) LeftSquare |[|
//@[085:088) Identifier |for|
//@[089:096) Identifier |account|
//@[097:099) Identifier |in|
//@[100:101) LeftSquare |[|
//@[101:102) RightSquare |]|
//@[102:103) Colon |:|
//@[104:105) LeftBrace |{|
//@[105:107) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |missingRequiredProperties2|
//@[036:082) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftSquare |[|
//@[086:089) Identifier |for|
//@[090:091) LeftParen |(|
//@[091:098) Identifier |account|
//@[098:099) Comma |,|
//@[099:100) Identifier |j|
//@[100:101) RightParen |)|
//@[102:104) Identifier |in|
//@[105:106) LeftSquare |[|
//@[106:107) RightSquare |]|
//@[107:108) Colon |:|
//@[109:110) LeftBrace |{|
//@[110:112) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// fewer missing required properties and a wrong property
//@[057:059) NewLine |\r\n|
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[000:008) Identifier |resource|
//@[009:039) Identifier |missingFewerRequiredProperties|
//@[040:086) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[087:088) Assignment |=|
//@[089:090) LeftSquare |[|
//@[090:093) Identifier |for|
//@[094:101) Identifier |account|
//@[102:104) Identifier |in|
//@[105:106) LeftSquare |[|
//@[106:107) RightSquare |]|
//@[107:108) Colon |:|
//@[109:110) LeftBrace |{|
//@[110:112) NewLine |\r\n|
  name: account
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) Identifier |account|
//@[015:017) NewLine |\r\n|
  location: 'eastus42'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:022) StringComplete |'eastus42'|
//@[022:024) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    wrong: 'test'
//@[004:009) Identifier |wrong|
//@[009:010) Colon |:|
//@[011:017) StringComplete |'test'|
//@[017:019) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// wrong property inside the nested property loop
//@[049:051) NewLine |\r\n|
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |wrongPropertyInNestedLoop|
//@[035:081) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[082:083) Assignment |=|
//@[084:085) LeftSquare |[|
//@[085:088) Identifier |for|
//@[089:090) Identifier |i|
//@[091:093) Identifier |in|
//@[094:099) Identifier |range|
//@[099:100) LeftParen |(|
//@[100:101) Integer |0|
//@[101:102) Comma |,|
//@[103:104) Integer |3|
//@[104:105) RightParen |)|
//@[105:106) Colon |:|
//@[107:108) LeftBrace |{|
//@[108:110) NewLine |\r\n|
  name: 'vnet-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:017) Identifier |i|
//@[017:019) StringRightPiece |}'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: [for j in range(0, 4): {
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:017) Identifier |for|
//@[018:019) Identifier |j|
//@[020:022) Identifier |in|
//@[023:028) Identifier |range|
//@[028:029) LeftParen |(|
//@[029:030) Integer |0|
//@[030:031) Comma |,|
//@[032:033) Integer |4|
//@[033:034) RightParen |)|
//@[034:035) Colon |:|
//@[036:037) LeftBrace |{|
//@[037:039) NewLine |\r\n|
      doesNotExist: 'test'
//@[006:018) Identifier |doesNotExist|
//@[018:019) Colon |:|
//@[020:026) StringComplete |'test'|
//@[026:028) NewLine |\r\n|
      name: 'subnet-${i}-${j}'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:022) StringLeftPiece |'subnet-${|
//@[022:023) Identifier |i|
//@[023:027) StringMiddlePiece |}-${|
//@[027:028) Identifier |j|
//@[028:030) StringRightPiece |}'|
//@[030:032) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |wrongPropertyInNestedLoop2|
//@[036:082) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftSquare |[|
//@[086:089) Identifier |for|
//@[090:091) LeftParen |(|
//@[091:092) Identifier |i|
//@[092:093) Comma |,|
//@[093:094) Identifier |k|
//@[094:095) RightParen |)|
//@[096:098) Identifier |in|
//@[099:104) Identifier |range|
//@[104:105) LeftParen |(|
//@[105:106) Integer |0|
//@[106:107) Comma |,|
//@[108:109) Integer |3|
//@[109:110) RightParen |)|
//@[110:111) Colon |:|
//@[112:113) LeftBrace |{|
//@[113:115) NewLine |\r\n|
  name: 'vnet-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:017) Identifier |i|
//@[017:019) StringRightPiece |}'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: [for j in range(0, 4): {
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:017) Identifier |for|
//@[018:019) Identifier |j|
//@[020:022) Identifier |in|
//@[023:028) Identifier |range|
//@[028:029) LeftParen |(|
//@[029:030) Integer |0|
//@[030:031) Comma |,|
//@[032:033) Integer |4|
//@[033:034) RightParen |)|
//@[034:035) Colon |:|
//@[036:037) LeftBrace |{|
//@[037:039) NewLine |\r\n|
      doesNotExist: 'test'
//@[006:018) Identifier |doesNotExist|
//@[018:019) Colon |:|
//@[020:026) StringComplete |'test'|
//@[026:028) NewLine |\r\n|
      name: 'subnet-${i}-${j}-${k}'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:022) StringLeftPiece |'subnet-${|
//@[022:023) Identifier |i|
//@[023:027) StringMiddlePiece |}-${|
//@[027:028) Identifier |j|
//@[028:032) StringMiddlePiece |}-${|
//@[032:033) Identifier |k|
//@[033:035) StringRightPiece |}'|
//@[035:037) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// nonexistent arrays and loop variables
//@[040:042) NewLine |\r\n|
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
//@[000:008) Identifier |resource|
//@[009:026) Identifier |nonexistentArrays|
//@[027:073) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[074:075) Assignment |=|
//@[076:077) LeftSquare |[|
//@[077:080) Identifier |for|
//@[081:082) Identifier |i|
//@[083:085) Identifier |in|
//@[086:095) Identifier |notAThing|
//@[095:096) Colon |:|
//@[097:098) LeftBrace |{|
//@[098:100) NewLine |\r\n|
  name: 'vnet-${justPlainWrong}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:030) Identifier |justPlainWrong|
//@[030:032) StringRightPiece |}'|
//@[032:034) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: [for j in alsoNotAThing: {
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:017) Identifier |for|
//@[018:019) Identifier |j|
//@[020:022) Identifier |in|
//@[023:036) Identifier |alsoNotAThing|
//@[036:037) Colon |:|
//@[038:039) LeftBrace |{|
//@[039:041) NewLine |\r\n|
      doesNotExist: 'test'
//@[006:018) Identifier |doesNotExist|
//@[018:019) Colon |:|
//@[020:026) StringComplete |'test'|
//@[026:028) NewLine |\r\n|
      name: 'subnet-${fake}-${totallyFake}'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:022) StringLeftPiece |'subnet-${|
//@[022:026) Identifier |fake|
//@[026:030) StringMiddlePiece |}-${|
//@[030:041) Identifier |totallyFake|
//@[041:043) StringRightPiece |}'|
//@[043:045) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// property loops cannot be nested
//@[034:036) NewLine |\r\n|
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:008) Identifier |resource|
//@[009:032) Identifier |propertyLoopsCannotNest|
//@[033:079) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[080:081) Assignment |=|
//@[082:083) LeftSquare |[|
//@[083:086) Identifier |for|
//@[087:094) Identifier |account|
//@[095:097) Identifier |in|
//@[098:113) Identifier |storageAccounts|
//@[113:114) Colon |:|
//@[115:116) LeftBrace |{|
//@[116:118) NewLine |\r\n|
  name: account.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) Identifier |account|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:022) NewLine |\r\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:030) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:019) NewLine |\r\n\r\n|

    networkAcls: {
//@[004:015) Identifier |networkAcls|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[018:020) NewLine |\r\n|
      virtualNetworkRules: [for rule in []: {
//@[006:025) Identifier |virtualNetworkRules|
//@[025:026) Colon |:|
//@[027:028) LeftSquare |[|
//@[028:031) Identifier |for|
//@[032:036) Identifier |rule|
//@[037:039) Identifier |in|
//@[040:041) LeftSquare |[|
//@[041:042) RightSquare |]|
//@[042:043) Colon |:|
//@[044:045) LeftBrace |{|
//@[045:047) NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[008:010) Identifier |id|
//@[010:011) Colon |:|
//@[012:015) StringLeftPiece |'${|
//@[015:022) Identifier |account|
//@[022:023) Dot |.|
//@[023:027) Identifier |name|
//@[027:031) StringMiddlePiece |}-${|
//@[031:038) Identifier |account|
//@[038:039) Dot |.|
//@[039:047) Identifier |location|
//@[047:049) StringRightPiece |}'|
//@[049:051) NewLine |\r\n|
        state: [for lol in []: 4]
//@[008:013) Identifier |state|
//@[013:014) Colon |:|
//@[015:016) LeftSquare |[|
//@[016:019) Identifier |for|
//@[020:023) Identifier |lol|
//@[024:026) Identifier |in|
//@[027:028) LeftSquare |[|
//@[028:029) RightSquare |]|
//@[029:030) Colon |:|
//@[031:032) Integer |4|
//@[032:033) RightSquare |]|
//@[033:035) NewLine |\r\n|
      }]
//@[006:007) RightBrace |}|
//@[007:008) RightSquare |]|
//@[008:010) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
//@[000:008) Identifier |resource|
//@[009:033) Identifier |propertyLoopsCannotNest2|
//@[034:080) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[081:082) Assignment |=|
//@[083:084) LeftSquare |[|
//@[084:087) Identifier |for|
//@[088:089) LeftParen |(|
//@[089:096) Identifier |account|
//@[096:097) Comma |,|
//@[097:098) Identifier |i|
//@[098:099) RightParen |)|
//@[100:102) Identifier |in|
//@[103:118) Identifier |storageAccounts|
//@[118:119) Colon |:|
//@[120:121) LeftBrace |{|
//@[121:123) NewLine |\r\n|
  name: account.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) Identifier |account|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:022) NewLine |\r\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:030) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:019) NewLine |\r\n\r\n|

    networkAcls: {
//@[004:015) Identifier |networkAcls|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[018:020) NewLine |\r\n|
      virtualNetworkRules: [for (rule,j) in []: {
//@[006:025) Identifier |virtualNetworkRules|
//@[025:026) Colon |:|
//@[027:028) LeftSquare |[|
//@[028:031) Identifier |for|
//@[032:033) LeftParen |(|
//@[033:037) Identifier |rule|
//@[037:038) Comma |,|
//@[038:039) Identifier |j|
//@[039:040) RightParen |)|
//@[041:043) Identifier |in|
//@[044:045) LeftSquare |[|
//@[045:046) RightSquare |]|
//@[046:047) Colon |:|
//@[048:049) LeftBrace |{|
//@[049:051) NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[008:010) Identifier |id|
//@[010:011) Colon |:|
//@[012:015) StringLeftPiece |'${|
//@[015:022) Identifier |account|
//@[022:023) Dot |.|
//@[023:027) Identifier |name|
//@[027:031) StringMiddlePiece |}-${|
//@[031:038) Identifier |account|
//@[038:039) Dot |.|
//@[039:047) Identifier |location|
//@[047:049) StringRightPiece |}'|
//@[049:051) NewLine |\r\n|
        state: [for (lol,k) in []: 4]
//@[008:013) Identifier |state|
//@[013:014) Colon |:|
//@[015:016) LeftSquare |[|
//@[016:019) Identifier |for|
//@[020:021) LeftParen |(|
//@[021:024) Identifier |lol|
//@[024:025) Comma |,|
//@[025:026) Identifier |k|
//@[026:027) RightParen |)|
//@[028:030) Identifier |in|
//@[031:032) LeftSquare |[|
//@[032:033) RightSquare |]|
//@[033:034) Colon |:|
//@[035:036) Integer |4|
//@[036:037) RightSquare |]|
//@[037:039) NewLine |\r\n|
      }]
//@[006:007) RightBrace |}|
//@[007:008) RightSquare |]|
//@[008:010) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// property loops cannot be nested (even more nesting)
//@[054:056) NewLine |\r\n|
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:008) Identifier |resource|
//@[009:033) Identifier |propertyLoopsCannotNest2|
//@[034:080) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[081:082) Assignment |=|
//@[083:084) LeftSquare |[|
//@[084:087) Identifier |for|
//@[088:095) Identifier |account|
//@[096:098) Identifier |in|
//@[099:114) Identifier |storageAccounts|
//@[114:115) Colon |:|
//@[116:117) LeftBrace |{|
//@[117:119) NewLine |\r\n|
  name: account.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) Identifier |account|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:022) NewLine |\r\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:030) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    networkAcls:  {
//@[004:015) Identifier |networkAcls|
//@[015:016) Colon |:|
//@[018:019) LeftBrace |{|
//@[019:021) NewLine |\r\n|
      virtualNetworkRules: [for rule in []: {
//@[006:025) Identifier |virtualNetworkRules|
//@[025:026) Colon |:|
//@[027:028) LeftSquare |[|
//@[028:031) Identifier |for|
//@[032:036) Identifier |rule|
//@[037:039) Identifier |in|
//@[040:041) LeftSquare |[|
//@[041:042) RightSquare |]|
//@[042:043) Colon |:|
//@[044:045) LeftBrace |{|
//@[045:047) NewLine |\r\n|
        // #completionTest(15,31) -> symbolsPlusRule
//@[052:054) NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[008:010) Identifier |id|
//@[010:011) Colon |:|
//@[012:015) StringLeftPiece |'${|
//@[015:022) Identifier |account|
//@[022:023) Dot |.|
//@[023:027) Identifier |name|
//@[027:031) StringMiddlePiece |}-${|
//@[031:038) Identifier |account|
//@[038:039) Dot |.|
//@[039:047) Identifier |location|
//@[047:049) StringRightPiece |}'|
//@[049:051) NewLine |\r\n|
        state: [for state in []: {
//@[008:013) Identifier |state|
//@[013:014) Colon |:|
//@[015:016) LeftSquare |[|
//@[016:019) Identifier |for|
//@[020:025) Identifier |state|
//@[026:028) Identifier |in|
//@[029:030) LeftSquare |[|
//@[030:031) RightSquare |]|
//@[031:032) Colon |:|
//@[033:034) LeftBrace |{|
//@[034:036) NewLine |\r\n|
          // #completionTest(38) -> empty #completionTest(16) -> symbolsPlusAccountRuleState
//@[092:094) NewLine |\r\n|
          fake: [for something in []: true]
//@[010:014) Identifier |fake|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:020) Identifier |for|
//@[021:030) Identifier |something|
//@[031:033) Identifier |in|
//@[034:035) LeftSquare |[|
//@[035:036) RightSquare |]|
//@[036:037) Colon |:|
//@[038:042) TrueKeyword |true|
//@[042:043) RightSquare |]|
//@[043:045) NewLine |\r\n|
        }]
//@[008:009) RightBrace |}|
//@[009:010) RightSquare |]|
//@[010:012) NewLine |\r\n|
      }]
//@[006:007) RightBrace |}|
//@[007:008) RightSquare |]|
//@[008:010) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// loops cannot be used inside of expressions
//@[045:047) NewLine |\r\n|
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:008) Identifier |resource|
//@[009:015) Identifier |stuffs|
//@[016:062) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[063:064) Assignment |=|
//@[065:066) LeftSquare |[|
//@[066:069) Identifier |for|
//@[070:077) Identifier |account|
//@[078:080) Identifier |in|
//@[081:096) Identifier |storageAccounts|
//@[096:097) Colon |:|
//@[098:099) LeftBrace |{|
//@[099:101) NewLine |\r\n|
  name: account.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) Identifier |account|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:022) NewLine |\r\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:030) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    networkAcls: {
//@[004:015) Identifier |networkAcls|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[018:020) NewLine |\r\n|
      virtualNetworkRules: concat([for lol in []: {
//@[006:025) Identifier |virtualNetworkRules|
//@[025:026) Colon |:|
//@[027:033) Identifier |concat|
//@[033:034) LeftParen |(|
//@[034:035) LeftSquare |[|
//@[035:038) Identifier |for|
//@[039:042) Identifier |lol|
//@[043:045) Identifier |in|
//@[046:047) LeftSquare |[|
//@[047:048) RightSquare |]|
//@[048:049) Colon |:|
//@[050:051) LeftBrace |{|
//@[051:053) NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[008:010) Identifier |id|
//@[010:011) Colon |:|
//@[012:015) StringLeftPiece |'${|
//@[015:022) Identifier |account|
//@[022:023) Dot |.|
//@[023:027) Identifier |name|
//@[027:031) StringMiddlePiece |}-${|
//@[031:038) Identifier |account|
//@[038:039) Dot |.|
//@[039:047) Identifier |location|
//@[047:049) StringRightPiece |}'|
//@[049:051) NewLine |\r\n|
      }])
//@[006:007) RightBrace |}|
//@[007:008) RightSquare |]|
//@[008:009) RightParen |)|
//@[009:011) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// using the same loop variable in a new language scope should be allowed
//@[073:075) NewLine |\r\n|
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:008) Identifier |resource|
//@[009:024) Identifier |premiumStorages|
//@[025:071) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:073) Assignment |=|
//@[074:075) LeftSquare |[|
//@[075:078) Identifier |for|
//@[079:086) Identifier |account|
//@[087:089) Identifier |in|
//@[090:105) Identifier |storageAccounts|
//@[105:106) Colon |:|
//@[107:108) LeftBrace |{|
//@[108:110) NewLine |\r\n|
  // #completionTest(7) -> symbolsPlusAccount1
//@[046:048) NewLine |\r\n|
  name: account.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) Identifier |account|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:022) NewLine |\r\n|
  // #completionTest(12) -> symbolsPlusAccount2
//@[047:049) NewLine |\r\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:030) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    // #completionTest(9,10) -> storageSkuNamePlusSymbols
//@[057:059) NewLine |\r\n|
    name: 
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:012) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

var directRefViaVar = premiumStorages
//@[000:003) Identifier |var|
//@[004:019) Identifier |directRefViaVar|
//@[020:021) Assignment |=|
//@[022:037) Identifier |premiumStorages|
//@[037:039) NewLine |\r\n|
output directRefViaOutput array = union(premiumStorages, stuffs)
//@[000:006) Identifier |output|
//@[007:025) Identifier |directRefViaOutput|
//@[026:031) Identifier |array|
//@[032:033) Assignment |=|
//@[034:039) Identifier |union|
//@[039:040) LeftParen |(|
//@[040:055) Identifier |premiumStorages|
//@[055:056) Comma |,|
//@[057:063) Identifier |stuffs|
//@[063:064) RightParen |)|
//@[064:068) NewLine |\r\n\r\n|

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
//@[000:008) Identifier |resource|
//@[009:039) Identifier |directRefViaSingleResourceBody|
//@[040:079) StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[080:081) Assignment |=|
//@[082:083) LeftBrace |{|
//@[083:085) NewLine |\r\n|
  name: 'myZone2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringComplete |'myZone2'|
//@[017:019) NewLine |\r\n|
  location: 'global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'global'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    registrationVirtualNetworks: premiumStorages
//@[004:031) Identifier |registrationVirtualNetworks|
//@[031:032) Colon |:|
//@[033:048) Identifier |premiumStorages|
//@[048:050) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
//@[000:008) Identifier |resource|
//@[009:050) Identifier |directRefViaSingleConditionalResourceBody|
//@[051:090) StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[091:092) Assignment |=|
//@[093:095) Identifier |if|
//@[095:096) LeftParen |(|
//@[096:100) TrueKeyword |true|
//@[100:101) RightParen |)|
//@[102:103) LeftBrace |{|
//@[103:105) NewLine |\r\n|
  name: 'myZone3'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringComplete |'myZone3'|
//@[017:019) NewLine |\r\n|
  location: 'global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'global'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
//@[004:031) Identifier |registrationVirtualNetworks|
//@[031:032) Colon |:|
//@[033:039) Identifier |concat|
//@[039:040) LeftParen |(|
//@[040:055) Identifier |premiumStorages|
//@[055:056) Comma |,|
//@[057:063) Identifier |stuffs|
//@[063:064) RightParen |)|
//@[064:066) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

@batchSize()
//@[000:001) At |@|
//@[001:010) Identifier |batchSize|
//@[010:011) LeftParen |(|
//@[011:012) RightParen |)|
//@[012:014) NewLine |\r\n|
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:008) Identifier |resource|
//@[009:043) Identifier |directRefViaSingleLoopResourceBody|
//@[044:090) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[091:092) Assignment |=|
//@[093:094) LeftSquare |[|
//@[094:097) Identifier |for|
//@[098:099) Identifier |i|
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
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:017) Identifier |i|
//@[017:019) StringRightPiece |}'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: premiumStorages
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:028) Identifier |premiumStorages|
//@[028:030) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

@batchSize(0)
//@[000:001) At |@|
//@[001:010) Identifier |batchSize|
//@[010:011) LeftParen |(|
//@[011:012) Integer |0|
//@[012:013) RightParen |)|
//@[013:015) NewLine |\r\n|
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:008) Identifier |resource|
//@[009:061) Identifier |directRefViaSingleLoopResourceBodyWithExtraDependsOn|
//@[062:108) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
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
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:017) Identifier |i|
//@[017:019) StringRightPiece |}'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: premiumStorages
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:028) Identifier |premiumStorages|
//@[028:030) NewLine |\r\n|
    dependsOn: [
//@[004:013) Identifier |dependsOn|
//@[013:014) Colon |:|
//@[015:016) LeftSquare |[|
//@[016:018) NewLine |\r\n|
      premiumStorages
//@[006:021) Identifier |premiumStorages|
//@[021:023) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    
//@[004:006) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

var expressionInPropertyLoopVar = true
//@[000:003) Identifier |var|
//@[004:031) Identifier |expressionInPropertyLoopVar|
//@[032:033) Assignment |=|
//@[034:038) TrueKeyword |true|
//@[038:040) NewLine |\r\n|
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:008) Identifier |resource|
//@[009:038) Identifier |expressionsInPropertyLoopName|
//@[039:078) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[079:080) Assignment |=|
//@[081:082) LeftBrace |{|
//@[082:084) NewLine |\r\n|
  name: 'hello'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringComplete |'hello'|
//@[015:017) NewLine |\r\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
//@[004:032) StringLeftPiece |'resolutionVirtualNetworks${|
//@[032:059) Identifier |expressionInPropertyLoopVar|
//@[059:061) StringRightPiece |}'|
//@[061:062) Colon |:|
//@[063:064) LeftSquare |[|
//@[064:067) Identifier |for|
//@[068:073) Identifier |thing|
//@[074:076) Identifier |in|
//@[077:078) LeftSquare |[|
//@[078:079) RightSquare |]|
//@[079:080) Colon |:|
//@[081:082) LeftBrace |{|
//@[082:083) RightBrace |}|
//@[083:084) RightSquare |]|
//@[084:086) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// resource loop body that isn't an object
//@[042:044) NewLine |\r\n|
@batchSize(-1)
//@[000:001) At |@|
//@[001:010) Identifier |batchSize|
//@[010:011) LeftParen |(|
//@[011:012) Minus |-|
//@[012:013) Integer |1|
//@[013:014) RightParen |)|
//@[014:016) NewLine |\r\n|
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
//@[000:008) Identifier |resource|
//@[009:034) Identifier |nonObjectResourceLoopBody|
//@[035:074) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[075:076) Assignment |=|
//@[077:078) LeftSquare |[|
//@[078:081) Identifier |for|
//@[082:087) Identifier |thing|
//@[088:090) Identifier |in|
//@[091:092) LeftSquare |[|
//@[092:093) RightSquare |]|
//@[093:094) Colon |:|
//@[095:101) StringComplete |'test'|
//@[101:102) RightSquare |]|
//@[102:104) NewLine |\r\n|
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
//@[000:008) Identifier |resource|
//@[009:035) Identifier |nonObjectResourceLoopBody2|
//@[036:075) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftSquare |[|
//@[079:082) Identifier |for|
//@[083:088) Identifier |thing|
//@[089:091) Identifier |in|
//@[092:093) LeftSquare |[|
//@[093:094) RightSquare |]|
//@[094:095) Colon |:|
//@[096:107) Identifier |environment|
//@[107:108) LeftParen |(|
//@[108:109) RightParen |)|
//@[109:110) RightSquare |]|
//@[110:112) NewLine |\r\n|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
//@[000:008) Identifier |resource|
//@[009:035) Identifier |nonObjectResourceLoopBody3|
//@[036:075) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftSquare |[|
//@[079:082) Identifier |for|
//@[083:084) LeftParen |(|
//@[084:089) Identifier |thing|
//@[089:090) Comma |,|
//@[090:091) Identifier |i|
//@[091:092) RightParen |)|
//@[093:095) Identifier |in|
//@[096:097) LeftSquare |[|
//@[097:098) RightSquare |]|
//@[098:099) Colon |:|
//@[100:106) StringComplete |'test'|
//@[106:107) RightSquare |]|
//@[107:109) NewLine |\r\n|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
//@[000:008) Identifier |resource|
//@[009:035) Identifier |nonObjectResourceLoopBody4|
//@[036:075) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftSquare |[|
//@[079:082) Identifier |for|
//@[083:084) LeftParen |(|
//@[084:089) Identifier |thing|
//@[089:090) Comma |,|
//@[090:091) Identifier |i|
//@[091:092) RightParen |)|
//@[093:095) Identifier |in|
//@[096:097) LeftSquare |[|
//@[097:098) RightSquare |]|
//@[098:099) Colon |:|
//@[100:111) Identifier |environment|
//@[111:112) LeftParen |(|
//@[112:113) RightParen |)|
//@[113:114) RightSquare |]|
//@[114:116) NewLine |\r\n|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
//@[000:008) Identifier |resource|
//@[009:035) Identifier |nonObjectResourceLoopBody3|
//@[036:075) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftSquare |[|
//@[079:082) Identifier |for|
//@[083:084) LeftParen |(|
//@[084:089) Identifier |thing|
//@[089:090) Comma |,|
//@[090:091) Identifier |i|
//@[091:092) RightParen |)|
//@[093:095) Identifier |in|
//@[096:097) LeftSquare |[|
//@[097:098) RightSquare |]|
//@[098:099) Colon |:|
//@[100:102) Identifier |if|
//@[102:103) LeftParen |(|
//@[103:107) TrueKeyword |true|
//@[107:108) RightParen |)|
//@[109:115) StringComplete |'test'|
//@[115:116) RightSquare |]|
//@[116:118) NewLine |\r\n|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]
//@[000:008) Identifier |resource|
//@[009:035) Identifier |nonObjectResourceLoopBody4|
//@[036:075) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftSquare |[|
//@[079:082) Identifier |for|
//@[083:084) LeftParen |(|
//@[084:089) Identifier |thing|
//@[089:090) Comma |,|
//@[090:091) Identifier |i|
//@[091:092) RightParen |)|
//@[093:095) Identifier |in|
//@[096:097) LeftSquare |[|
//@[097:098) RightSquare |]|
//@[098:099) Colon |:|
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
//@[042:044) NewLine |\r\n|
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:052) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[053:054) Assignment |=|
//@[055:059) NewLine |\r\n\r\n|

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |foo|
//@[013:052) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[053:054) Assignment |=|
//@[055:056) LeftSquare |[|
//@[056:059) Identifier |for|
//@[060:064) Identifier |item|
//@[065:067) Identifier |in|
//@[068:069) LeftSquare |[|
//@[069:070) RightSquare |]|
//@[070:071) Colon |:|
//@[072:073) LeftBrace |{|
//@[073:075) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
//@[055:057) NewLine |\r\n|
    registrationVirtualNetworks: 
//@[004:031) Identifier |registrationVirtualNetworks|
//@[031:032) Colon |:|
//@[033:035) NewLine |\r\n|
    resolutionVirtualNetworks: [for lol in []: {
//@[004:029) Identifier |resolutionVirtualNetworks|
//@[029:030) Colon |:|
//@[031:032) LeftSquare |[|
//@[032:035) Identifier |for|
//@[036:039) Identifier |lol|
//@[040:042) Identifier |in|
//@[043:044) LeftSquare |[|
//@[044:045) RightSquare |]|
//@[045:046) Colon |:|
//@[047:048) LeftBrace |{|
//@[048:050) NewLine |\r\n|
      
//@[006:008) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:013) Identifier |vnet|
//@[014:060) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[061:062) Assignment |=|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    virtualNetworkPeerings: [for item in []: {
//@[004:026) Identifier |virtualNetworkPeerings|
//@[026:027) Colon |:|
//@[028:029) LeftSquare |[|
//@[029:032) Identifier |for|
//@[033:037) Identifier |item|
//@[038:040) Identifier |in|
//@[041:042) LeftSquare |[|
//@[042:043) RightSquare |]|
//@[043:044) Colon |:|
//@[045:046) LeftBrace |{|
//@[046:048) NewLine |\r\n|
        properties: {
//@[008:018) Identifier |properties|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
          remoteAddressSpace: {
//@[010:028) Identifier |remoteAddressSpace|
//@[028:029) Colon |:|
//@[030:031) LeftBrace |{|
//@[031:033) NewLine |\r\n|
            // #completionTest(28,29) -> symbolsPlusArrayWithoutFor
//@[067:069) NewLine |\r\n|
            addressPrefixes: 
//@[012:027) Identifier |addressPrefixes|
//@[027:028) Colon |:|
//@[029:031) NewLine |\r\n|
          }
//@[010:011) RightBrace |}|
//@[011:013) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// parent property with 'existing' resource at different scope
//@[062:064) NewLine |\r\n|
resource p1_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p1_res1|
//@[017:053) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:062) Identifier |existing|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  scope: subscription()
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:021) Identifier |subscription|
//@[021:022) LeftParen |(|
//@[022:023) RightParen |)|
//@[023:025) NewLine |\r\n|
  name: 'res1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res1'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |p1_child1|
//@[019:062) StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  parent: p1_res1
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p1_res1|
//@[017:019) NewLine |\r\n|
  name: 'child1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'child1'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// parent property with scope on child resource
//@[047:049) NewLine |\r\n|
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p2_res1|
//@[017:053) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'res1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res1'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p2_res2|
//@[017:053) StringComplete |'Microsoft.Rp2/resource2@2020-06-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'res2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res2'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |p2_res2child|
//@[022:065) StringComplete |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[066:067) Assignment |=|
//@[068:069) LeftBrace |{|
//@[069:071) NewLine |\r\n|
  scope: p2_res1
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:016) Identifier |p2_res1|
//@[016:018) NewLine |\r\n|
  parent: p2_res2
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p2_res2|
//@[017:019) NewLine |\r\n|
  name: 'child2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'child2'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// parent property self-cycle
//@[029:031) NewLine |\r\n|
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:017) Identifier |p3_vmExt|
//@[018:075) StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftBrace |{|
//@[079:081) NewLine |\r\n|
  parent: p3_vmExt
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:018) Identifier |p3_vmExt|
//@[018:020) NewLine |\r\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:022) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// parent property 2-cycle
//@[026:028) NewLine |\r\n|
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:014) Identifier |p4_vm|
//@[015:061) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[062:063) Assignment |=|
//@[064:065) LeftBrace |{|
//@[065:067) NewLine |\r\n|
  parent: p4_vmExt
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:018) Identifier |p4_vmExt|
//@[018:020) NewLine |\r\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:022) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:017) Identifier |p4_vmExt|
//@[018:075) StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftBrace |{|
//@[079:081) NewLine |\r\n|
  parent: p4_vm
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:015) Identifier |p4_vm|
//@[015:017) NewLine |\r\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:022) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// parent property with invalid child
//@[037:039) NewLine |\r\n|
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p5_res1|
//@[017:053) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'res1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res1'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p5_res2|
//@[017:060) StringComplete |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[061:062) Assignment |=|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  parent: p5_res1
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p5_res1|
//@[017:019) NewLine |\r\n|
  name: 'res2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res2'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// parent property with invalid parent
//@[038:040) NewLine |\r\n|
resource p6_res1 '${true}' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p6_res1|
//@[017:020) StringLeftPiece |'${|
//@[020:024) TrueKeyword |true|
//@[024:026) StringRightPiece |}'|
//@[027:028) Assignment |=|
//@[029:030) LeftBrace |{|
//@[030:032) NewLine |\r\n|
  name: 'res1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res1'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p6_res2|
//@[017:060) StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[061:062) Assignment |=|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  parent: p6_res1
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p6_res1|
//@[017:019) NewLine |\r\n|
  name: 'res2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res2'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// parent property with incorrectly-formatted name
//@[050:052) NewLine |\r\n|
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p7_res1|
//@[017:053) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'res1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res1'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p7_res2|
//@[017:060) StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[061:062) Assignment |=|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  parent: p7_res1
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p7_res1|
//@[017:019) NewLine |\r\n|
  name: 'res1/res2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'res1/res2'|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p7_res3|
//@[017:060) StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[061:062) Assignment |=|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  parent: p7_res1
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p7_res1|
//@[017:019) NewLine |\r\n|
  name: '${p7_res1.name}/res2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:018) Identifier |p7_res1|
//@[018:019) Dot |.|
//@[019:023) Identifier |name|
//@[023:030) StringRightPiece |}/res2'|
//@[030:032) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// top-level resource with too many '/' characters
//@[050:052) NewLine |\r\n|
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p8_res1|
//@[017:053) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'res1/res2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'res1/res2'|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource existingResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |existingResProperty|
//@[029:075) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[076:084) Identifier |existing|
//@[085:086) Assignment |=|
//@[087:088) LeftBrace |{|
//@[088:090) NewLine |\r\n|
  name: 'existingResProperty'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:029) StringComplete |'existingResProperty'|
//@[029:031) NewLine |\r\n|
  location: 'westeurope'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:024) StringComplete |'westeurope'|
//@[024:026) NewLine |\r\n|
  properties: {}
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource invalidExistingLocationRef 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |invalidExistingLocationRef|
//@[036:093) StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[094:095) Assignment |=|
//@[096:097) LeftBrace |{|
//@[097:099) NewLine |\r\n|
    parent: existingResProperty
//@[004:010) Identifier |parent|
//@[010:011) Colon |:|
//@[012:031) Identifier |existingResProperty|
//@[031:033) NewLine |\r\n|
    name: 'myExt'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:017) StringComplete |'myExt'|
//@[017:019) NewLine |\r\n|
    location: existingResProperty.location
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:033) Identifier |existingResProperty|
//@[033:034) Dot |.|
//@[034:042) Identifier |location|
//@[042:044) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource anyTypeInDependsOn 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |anyTypeInDependsOn|
//@[028:067) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[068:069) Assignment |=|
//@[070:071) LeftBrace |{|
//@[071:073) NewLine |\r\n|
  name: 'anyTypeInDependsOn'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:028) StringComplete |'anyTypeInDependsOn'|
//@[028:030) NewLine |\r\n|
  location: resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:038) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)
//@[004:007) Identifier |any|
//@[007:008) LeftParen |(|
//@[008:034) Identifier |invalidExistingLocationRef|
//@[034:035) Dot |.|
//@[035:045) Identifier |properties|
//@[045:046) Dot |.|
//@[046:069) Identifier |autoUpgradeMinorVersion|
//@[069:070) RightParen |)|
//@[070:072) NewLine |\r\n|
    's'
//@[004:007) StringComplete |'s'|
//@[007:009) NewLine |\r\n|
    any(true)
//@[004:007) Identifier |any|
//@[007:008) LeftParen |(|
//@[008:012) TrueKeyword |true|
//@[012:013) RightParen |)|
//@[013:015) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource anyTypeInParent 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
//@[000:008) Identifier |resource|
//@[009:024) Identifier |anyTypeInParent|
//@[025:070) StringComplete |'Microsoft.Network/dnsZones/CNAME@2018-05-01'|
//@[071:072) Assignment |=|
//@[073:074) LeftBrace |{|
//@[074:076) NewLine |\r\n|
  parent: any(true)
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:013) Identifier |any|
//@[013:014) LeftParen |(|
//@[014:018) TrueKeyword |true|
//@[018:019) RightParen |)|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource anyTypeInParentLoop 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |anyTypeInParentLoop|
//@[029:074) StringComplete |'Microsoft.Network/dnsZones/CNAME@2018-05-01'|
//@[075:076) Assignment |=|
//@[077:078) LeftSquare |[|
//@[078:081) Identifier |for|
//@[082:087) Identifier |thing|
//@[088:090) Identifier |in|
//@[091:092) LeftSquare |[|
//@[092:093) RightSquare |]|
//@[093:094) Colon |:|
//@[095:096) LeftBrace |{|
//@[096:098) NewLine |\r\n|
  parent: any(true)
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:013) Identifier |any|
//@[013:014) LeftParen |(|
//@[014:018) TrueKeyword |true|
//@[018:019) RightParen |)|
//@[019:021) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource anyTypeInScope 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:008) Identifier |resource|
//@[009:023) Identifier |anyTypeInScope|
//@[024:066) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[067:068) Assignment |=|
//@[069:070) LeftBrace |{|
//@[070:072) NewLine |\r\n|
  scope: any(invalidExistingLocationRef)
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:012) Identifier |any|
//@[012:013) LeftParen |(|
//@[013:039) Identifier |invalidExistingLocationRef|
//@[039:040) RightParen |)|
//@[040:042) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource anyTypeInScopeConditional 'Microsoft.Authorization/locks@2016-09-01' = if(true) {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |anyTypeInScopeConditional|
//@[035:077) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[078:079) Assignment |=|
//@[080:082) Identifier |if|
//@[082:083) LeftParen |(|
//@[083:087) TrueKeyword |true|
//@[087:088) RightParen |)|
//@[089:090) LeftBrace |{|
//@[090:092) NewLine |\r\n|
  scope: any(invalidExistingLocationRef)
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:012) Identifier |any|
//@[012:013) LeftParen |(|
//@[013:039) Identifier |invalidExistingLocationRef|
//@[039:040) RightParen |)|
//@[040:042) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource anyTypeInExistingScope 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |anyTypeInExistingScope|
//@[032:076) StringComplete |'Microsoft.Network/dnsZones/AAAA@2018-05-01'|
//@[077:085) Identifier |existing|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
  parent: any('')
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:013) Identifier |any|
//@[013:014) LeftParen |(|
//@[014:016) StringComplete |''|
//@[016:017) RightParen |)|
//@[017:019) NewLine |\r\n|
  scope: any(false)
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:012) Identifier |any|
//@[012:013) LeftParen |(|
//@[013:018) FalseKeyword |false|
//@[018:019) RightParen |)|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource anyTypeInExistingScopeLoop 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = [for thing in []: {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |anyTypeInExistingScopeLoop|
//@[036:080) StringComplete |'Microsoft.Network/dnsZones/AAAA@2018-05-01'|
//@[081:089) Identifier |existing|
//@[090:091) Assignment |=|
//@[092:093) LeftSquare |[|
//@[093:096) Identifier |for|
//@[097:102) Identifier |thing|
//@[103:105) Identifier |in|
//@[106:107) LeftSquare |[|
//@[107:108) RightSquare |]|
//@[108:109) Colon |:|
//@[110:111) LeftBrace |{|
//@[111:113) NewLine |\r\n|
  parent: any('')
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:013) Identifier |any|
//@[013:014) LeftParen |(|
//@[014:016) StringComplete |''|
//@[016:017) RightParen |)|
//@[017:019) NewLine |\r\n|
  scope: any(false)
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:012) Identifier |any|
//@[012:013) LeftParen |(|
//@[013:018) FalseKeyword |false|
//@[018:019) RightParen |)|
//@[019:021) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource tenantLevelResourceBlocked 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[000:008) Identifier |resource|
//@[009:035) Identifier |tenantLevelResourceBlocked|
//@[036:086) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[087:088) Assignment |=|
//@[089:090) LeftBrace |{|
//@[090:092) NewLine |\r\n|
  name: 'tenantLevelResourceBlocked'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:036) StringComplete |'tenantLevelResourceBlocked'|
//@[036:038) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// #completionTest(15,36,37) -> resourceTypes
//@[045:047) NewLine |\r\n|
resource comp1 'Microsoft.Resources/'
//@[000:008) Identifier |resource|
//@[009:014) Identifier |comp1|
//@[015:037) StringComplete |'Microsoft.Resources/'|
//@[037:041) NewLine |\r\n\r\n|

// #completionTest(15,16,17) -> resourceTypes
//@[045:047) NewLine |\r\n|
resource comp2 ''
//@[000:008) Identifier |resource|
//@[009:014) Identifier |comp2|
//@[015:017) StringComplete |''|
//@[017:021) NewLine |\r\n\r\n|

// #completionTest(38) -> resourceTypes
//@[039:041) NewLine |\r\n|
resource comp3 'Microsoft.Resources/t'
//@[000:008) Identifier |resource|
//@[009:014) Identifier |comp3|
//@[015:038) StringComplete |'Microsoft.Resources/t'|
//@[038:042) NewLine |\r\n\r\n|

// #completionTest(40) -> resourceTypes
//@[039:041) NewLine |\r\n|
resource comp4 'Microsoft.Resources/t/v'
//@[000:008) Identifier |resource|
//@[009:014) Identifier |comp4|
//@[015:040) StringComplete |'Microsoft.Resources/t/v'|
//@[040:044) NewLine |\r\n\r\n|

// #completionTest(49) -> resourceTypes
//@[039:041) NewLine |\r\n|
resource comp5 'Microsoft.Storage/storageAccounts'
//@[000:008) Identifier |resource|
//@[009:014) Identifier |comp5|
//@[015:050) StringComplete |'Microsoft.Storage/storageAccounts'|
//@[050:054) NewLine |\r\n\r\n|

// #completionTest(50) -> storageAccountsResourceTypes
//@[054:056) NewLine |\r\n|
resource comp6 'Microsoft.Storage/storageAccounts@'
//@[000:008) Identifier |resource|
//@[009:014) Identifier |comp6|
//@[015:051) StringComplete |'Microsoft.Storage/storageAccounts@'|
//@[051:055) NewLine |\r\n\r\n|

// #completionTest(52) -> templateSpecsResourceTypes
//@[052:054) NewLine |\r\n|
resource comp7 'Microsoft.Resources/templateSpecs@20'
//@[000:008) Identifier |resource|
//@[009:014) Identifier |comp7|
//@[015:053) StringComplete |'Microsoft.Resources/templateSpecs@20'|
//@[053:057) NewLine |\r\n\r\n|

// #completionTest(60,61) -> virtualNetworksResourceTypes
//@[057:059) NewLine |\r\n|
resource comp8 'Microsoft.Network/virtualNetworks@2020-06-01'
//@[000:008) Identifier |resource|
//@[009:014) Identifier |comp8|
//@[015:061) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[061:067) NewLine |\r\n\r\n\r\n|


// issue #3000
//@[014:016) NewLine |\r\n|
resource issue3000LogicApp1 'Microsoft.Logic/workflows@2019-05-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |issue3000LogicApp1|
//@[028:066) StringComplete |'Microsoft.Logic/workflows@2019-05-01'|
//@[067:068) Assignment |=|
//@[069:070) LeftBrace |{|
//@[070:072) NewLine |\r\n|
  name: 'issue3000LogicApp1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:028) StringComplete |'issue3000LogicApp1'|
//@[028:030) NewLine |\r\n|
  location: resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:038) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    state: 'Enabled'
//@[004:009) Identifier |state|
//@[009:010) Colon |:|
//@[011:020) StringComplete |'Enabled'|
//@[020:022) NewLine |\r\n|
    definition: ''
//@[004:014) Identifier |definition|
//@[014:015) Colon |:|
//@[016:018) StringComplete |''|
//@[018:020) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  identity: {
//@[002:010) Identifier |identity|
//@[010:011) Colon |:|
//@[012:013) LeftBrace |{|
//@[013:015) NewLine |\r\n|
    type: 'SystemAssigned'
//@[004:008) Identifier |type|
//@[008:009) Colon |:|
//@[010:026) StringComplete |'SystemAssigned'|
//@[026:028) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  extendedLocation: {}
//@[002:018) Identifier |extendedLocation|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:022) RightBrace |}|
//@[022:024) NewLine |\r\n|
  sku: {}
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
  kind: 'V1'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:012) StringComplete |'V1'|
//@[012:014) NewLine |\r\n|
  managedBy: 'string'
//@[002:011) Identifier |managedBy|
//@[011:012) Colon |:|
//@[013:021) StringComplete |'string'|
//@[021:023) NewLine |\r\n|
  mangedByExtended: [
//@[002:018) Identifier |mangedByExtended|
//@[018:019) Colon |:|
//@[020:021) LeftSquare |[|
//@[021:023) NewLine |\r\n|
   'str1'
//@[003:009) StringComplete |'str1'|
//@[009:011) NewLine |\r\n|
   'str2'
//@[003:009) StringComplete |'str2'|
//@[009:011) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
  zones: [
//@[002:007) Identifier |zones|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:012) NewLine |\r\n|
   'str1'
//@[003:009) StringComplete |'str1'|
//@[009:011) NewLine |\r\n|
   'str2'
//@[003:009) StringComplete |'str2'|
//@[009:011) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
  plan: {}
//@[002:006) Identifier |plan|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[009:010) RightBrace |}|
//@[010:012) NewLine |\r\n|
  eTag: ''
//@[002:006) Identifier |eTag|
//@[006:007) Colon |:|
//@[008:010) StringComplete |''|
//@[010:012) NewLine |\r\n|
  scale: {}  
//@[002:007) Identifier |scale|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:011) RightBrace |}|
//@[013:015) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource issue3000LogicApp2 'Microsoft.Logic/workflows@2019-05-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |issue3000LogicApp2|
//@[028:066) StringComplete |'Microsoft.Logic/workflows@2019-05-01'|
//@[067:068) Assignment |=|
//@[069:070) LeftBrace |{|
//@[070:072) NewLine |\r\n|
  name: 'issue3000LogicApp2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:028) StringComplete |'issue3000LogicApp2'|
//@[028:030) NewLine |\r\n|
  location: resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:038) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    state: 'Enabled'
//@[004:009) Identifier |state|
//@[009:010) Colon |:|
//@[011:020) StringComplete |'Enabled'|
//@[020:022) NewLine |\r\n|
    definition: ''
//@[004:014) Identifier |definition|
//@[014:015) Colon |:|
//@[016:018) StringComplete |''|
//@[018:020) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  identity: 'SystemAssigned'
//@[002:010) Identifier |identity|
//@[010:011) Colon |:|
//@[012:028) StringComplete |'SystemAssigned'|
//@[028:030) NewLine |\r\n|
  extendedLocation: 'eastus'
//@[002:018) Identifier |extendedLocation|
//@[018:019) Colon |:|
//@[020:028) StringComplete |'eastus'|
//@[028:030) NewLine |\r\n|
  sku: 'Basic'
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:014) StringComplete |'Basic'|
//@[014:016) NewLine |\r\n|
  kind: {
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[009:011) NewLine |\r\n|
    name: 'V1'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:014) StringComplete |'V1'|
//@[014:016) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  managedBy: {}
//@[002:011) Identifier |managedBy|
//@[011:012) Colon |:|
//@[013:014) LeftBrace |{|
//@[014:015) RightBrace |}|
//@[015:017) NewLine |\r\n|
  mangedByExtended: [
//@[002:018) Identifier |mangedByExtended|
//@[018:019) Colon |:|
//@[020:021) LeftSquare |[|
//@[021:023) NewLine |\r\n|
   {}
//@[003:004) LeftBrace |{|
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
   {}
//@[003:004) LeftBrace |{|
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
  zones: [
//@[002:007) Identifier |zones|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:012) NewLine |\r\n|
   {}
//@[003:004) LeftBrace |{|
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
   {}
//@[003:004) LeftBrace |{|
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
  plan: ''
//@[002:006) Identifier |plan|
//@[006:007) Colon |:|
//@[008:010) StringComplete |''|
//@[010:012) NewLine |\r\n|
  eTag: {}
//@[002:006) Identifier |eTag|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[009:010) RightBrace |}|
//@[010:012) NewLine |\r\n|
  scale: [
//@[002:007) Identifier |scale|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:012) NewLine |\r\n|
  {}
//@[002:003) LeftBrace |{|
//@[003:004) RightBrace |}|
//@[004:006) NewLine |\r\n|
  ]  
//@[002:003) RightSquare |]|
//@[005:007) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource issue3000stg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |issue3000stg|
//@[022:068) StringComplete |'Microsoft.Storage/storageAccounts@2021-04-01'|
//@[069:070) Assignment |=|
//@[071:072) LeftBrace |{|
//@[072:074) NewLine |\r\n|
  name: 'issue3000stg'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'issue3000stg'|
//@[022:024) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
  location: 'West US'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:021) StringComplete |'West US'|
//@[021:023) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Premium_LRS'    
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:023) StringComplete |'Premium_LRS'|
//@[027:029) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  madeUpProperty: {}
//@[002:016) Identifier |madeUpProperty|
//@[016:017) Colon |:|
//@[018:019) LeftBrace |{|
//@[019:020) RightBrace |}|
//@[020:022) NewLine |\r\n|
  managedByExtended: []
//@[002:019) Identifier |managedByExtended|
//@[019:020) Colon |:|
//@[021:022) LeftSquare |[|
//@[022:023) RightSquare |]|
//@[023:025) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var issue3000stgMadeUpProperty = issue3000stg.madeUpProperty
//@[000:003) Identifier |var|
//@[004:030) Identifier |issue3000stgMadeUpProperty|
//@[031:032) Assignment |=|
//@[033:045) Identifier |issue3000stg|
//@[045:046) Dot |.|
//@[046:060) Identifier |madeUpProperty|
//@[060:062) NewLine |\r\n|
var issue3000stgManagedBy = issue3000stg.managedBy
//@[000:003) Identifier |var|
//@[004:025) Identifier |issue3000stgManagedBy|
//@[026:027) Assignment |=|
//@[028:040) Identifier |issue3000stg|
//@[040:041) Dot |.|
//@[041:050) Identifier |managedBy|
//@[050:052) NewLine |\r\n|
var issue3000stgManagedByExtended = issue3000stg.managedByExtended
//@[000:003) Identifier |var|
//@[004:033) Identifier |issue3000stgManagedByExtended|
//@[034:035) Assignment |=|
//@[036:048) Identifier |issue3000stg|
//@[048:049) Dot |.|
//@[049:066) Identifier |managedByExtended|
//@[066:070) NewLine |\r\n\r\n|

param dataCollectionRule object
//@[000:005) Identifier |param|
//@[006:024) Identifier |dataCollectionRule|
//@[025:031) Identifier |object|
//@[031:033) NewLine |\r\n|
param tags object
//@[000:005) Identifier |param|
//@[006:010) Identifier |tags|
//@[011:017) Identifier |object|
//@[017:021) NewLine |\r\n\r\n|

var defaultLogAnalyticsWorkspace = {
//@[000:003) Identifier |var|
//@[004:032) Identifier |defaultLogAnalyticsWorkspace|
//@[033:034) Assignment |=|
//@[035:036) LeftBrace |{|
//@[036:038) NewLine |\r\n|
  subscriptionId: subscription().subscriptionId
//@[002:016) Identifier |subscriptionId|
//@[016:017) Colon |:|
//@[018:030) Identifier |subscription|
//@[030:031) LeftParen |(|
//@[031:032) RightParen |)|
//@[032:033) Dot |.|
//@[033:047) Identifier |subscriptionId|
//@[047:049) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource logAnalyticsWorkspaces 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = [for logAnalyticsWorkspace in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |logAnalyticsWorkspaces|
//@[032:085) StringComplete |'Microsoft.OperationalInsights/workspaces@2020-10-01'|
//@[086:094) Identifier |existing|
//@[095:096) Assignment |=|
//@[097:098) LeftSquare |[|
//@[098:101) Identifier |for|
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
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:029) Identifier |logAnalyticsWorkspace|
//@[029:030) Dot |.|
//@[030:034) Identifier |name|
//@[034:036) NewLine |\r\n|
  scope: resourceGroup( union( defaultLogAnalyticsWorkspace, logAnalyticsWorkspace ).subscriptionId, logAnalyticsWorkspace.resourceGroup )
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceGroup|
//@[022:023) LeftParen |(|
//@[024:029) Identifier |union|
//@[029:030) LeftParen |(|
//@[031:059) Identifier |defaultLogAnalyticsWorkspace|
//@[059:060) Comma |,|
//@[061:082) Identifier |logAnalyticsWorkspace|
//@[083:084) RightParen |)|
//@[084:085) Dot |.|
//@[085:099) Identifier |subscriptionId|
//@[099:100) Comma |,|
//@[101:122) Identifier |logAnalyticsWorkspace|
//@[122:123) Dot |.|
//@[123:136) Identifier |resourceGroup|
//@[137:138) RightParen |)|
//@[138:140) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource dataCollectionRuleRes 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |dataCollectionRuleRes|
//@[031:082) StringComplete |'Microsoft.Insights/dataCollectionRules@2021-04-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[086:088) NewLine |\r\n|
  name: dataCollectionRule.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:026) Identifier |dataCollectionRule|
//@[026:027) Dot |.|
//@[027:031) Identifier |name|
//@[031:033) NewLine |\r\n|
  location: dataCollectionRule.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:030) Identifier |dataCollectionRule|
//@[030:031) Dot |.|
//@[031:039) Identifier |location|
//@[039:041) NewLine |\r\n|
  tags: tags
//@[002:006) Identifier |tags|
//@[006:007) Colon |:|
//@[008:012) Identifier |tags|
//@[012:014) NewLine |\r\n|
  kind: dataCollectionRule.kind
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:026) Identifier |dataCollectionRule|
//@[026:027) Dot |.|
//@[027:031) Identifier |kind|
//@[031:033) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    description: dataCollectionRule.description
//@[004:015) Identifier |description|
//@[015:016) Colon |:|
//@[017:035) Identifier |dataCollectionRule|
//@[035:036) Dot |.|
//@[036:047) Identifier |description|
//@[047:049) NewLine |\r\n|
    destinations: union(empty(dataCollectionRule.destinations.azureMonitorMetrics.name) ? {} : {
//@[004:016) Identifier |destinations|
//@[016:017) Colon |:|
//@[018:023) Identifier |union|
//@[023:024) LeftParen |(|
//@[024:029) Identifier |empty|
//@[029:030) LeftParen |(|
//@[030:048) Identifier |dataCollectionRule|
//@[048:049) Dot |.|
//@[049:061) Identifier |destinations|
//@[061:062) Dot |.|
//@[062:081) Identifier |azureMonitorMetrics|
//@[081:082) Dot |.|
//@[082:086) Identifier |name|
//@[086:087) RightParen |)|
//@[088:089) Question |?|
//@[090:091) LeftBrace |{|
//@[091:092) RightBrace |}|
//@[093:094) Colon |:|
//@[095:096) LeftBrace |{|
//@[096:098) NewLine |\r\n|
      azureMonitorMetrics: {
//@[006:025) Identifier |azureMonitorMetrics|
//@[025:026) Colon |:|
//@[027:028) LeftBrace |{|
//@[028:030) NewLine |\r\n|
        name: dataCollectionRule.destinations.azureMonitorMetrics.name
//@[008:012) Identifier |name|
//@[012:013) Colon |:|
//@[014:032) Identifier |dataCollectionRule|
//@[032:033) Dot |.|
//@[033:045) Identifier |destinations|
//@[045:046) Dot |.|
//@[046:065) Identifier |azureMonitorMetrics|
//@[065:066) Dot |.|
//@[066:070) Identifier |name|
//@[070:072) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    },{
//@[004:005) RightBrace |}|
//@[005:006) Comma |,|
//@[006:007) LeftBrace |{|
//@[007:009) NewLine |\r\n|
      logAnalytics: [for (logAnalyticsWorkspace, i) in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[006:018) Identifier |logAnalytics|
//@[018:019) Colon |:|
//@[020:021) LeftSquare |[|
//@[021:024) Identifier |for|
//@[025:026) LeftParen |(|
//@[026:047) Identifier |logAnalyticsWorkspace|
//@[047:048) Comma |,|
//@[049:050) Identifier |i|
//@[050:051) RightParen |)|
//@[052:054) Identifier |in|
//@[055:073) Identifier |dataCollectionRule|
//@[073:074) Dot |.|
//@[074:086) Identifier |destinations|
//@[086:087) Dot |.|
//@[087:109) Identifier |logAnalyticsWorkspaces|
//@[109:110) Colon |:|
//@[111:112) LeftBrace |{|
//@[112:114) NewLine |\r\n|
        name: logAnalyticsWorkspace.destinationName
//@[008:012) Identifier |name|
//@[012:013) Colon |:|
//@[014:035) Identifier |logAnalyticsWorkspace|
//@[035:036) Dot |.|
//@[036:051) Identifier |destinationName|
//@[051:053) NewLine |\r\n|
        workspaceResourceId: logAnalyticsWorkspaces[i].id
//@[008:027) Identifier |workspaceResourceId|
//@[027:028) Colon |:|
//@[029:051) Identifier |logAnalyticsWorkspaces|
//@[051:052) LeftSquare |[|
//@[052:053) Identifier |i|
//@[053:054) RightSquare |]|
//@[054:055) Dot |.|
//@[055:057) Identifier |id|
//@[057:059) NewLine |\r\n|
      }]
//@[006:007) RightBrace |}|
//@[007:008) RightSquare |]|
//@[008:010) NewLine |\r\n|
    })
//@[004:005) RightBrace |}|
//@[005:006) RightParen |)|
//@[006:008) NewLine |\r\n|
    dataSources: dataCollectionRule.dataSources
//@[004:015) Identifier |dataSources|
//@[015:016) Colon |:|
//@[017:035) Identifier |dataCollectionRule|
//@[035:036) Dot |.|
//@[036:047) Identifier |dataSources|
//@[047:049) NewLine |\r\n|
    dataFlows: dataCollectionRule.dataFlows
//@[004:013) Identifier |dataFlows|
//@[013:014) Colon |:|
//@[015:033) Identifier |dataCollectionRule|
//@[033:034) Dot |.|
//@[034:043) Identifier |dataFlows|
//@[043:045) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource dataCollectionRuleRes2 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |dataCollectionRuleRes2|
//@[032:083) StringComplete |'Microsoft.Insights/dataCollectionRules@2021-04-01'|
//@[084:085) Assignment |=|
//@[086:087) LeftBrace |{|
//@[087:089) NewLine |\r\n|
  name: dataCollectionRule.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:026) Identifier |dataCollectionRule|
//@[026:027) Dot |.|
//@[027:031) Identifier |name|
//@[031:033) NewLine |\r\n|
  location: dataCollectionRule.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:030) Identifier |dataCollectionRule|
//@[030:031) Dot |.|
//@[031:039) Identifier |location|
//@[039:041) NewLine |\r\n|
  tags: tags
//@[002:006) Identifier |tags|
//@[006:007) Colon |:|
//@[008:012) Identifier |tags|
//@[012:014) NewLine |\r\n|
  kind: dataCollectionRule.kind
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:026) Identifier |dataCollectionRule|
//@[026:027) Dot |.|
//@[027:031) Identifier |kind|
//@[031:033) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    description: dataCollectionRule.description
//@[004:015) Identifier |description|
//@[015:016) Colon |:|
//@[017:035) Identifier |dataCollectionRule|
//@[035:036) Dot |.|
//@[036:047) Identifier |description|
//@[047:049) NewLine |\r\n|
    destinations: empty([]) ? [for x in []: {}] : [for x in []: {}]
//@[004:016) Identifier |destinations|
//@[016:017) Colon |:|
//@[018:023) Identifier |empty|
//@[023:024) LeftParen |(|
//@[024:025) LeftSquare |[|
//@[025:026) RightSquare |]|
//@[026:027) RightParen |)|
//@[028:029) Question |?|
//@[030:031) LeftSquare |[|
//@[031:034) Identifier |for|
//@[035:036) Identifier |x|
//@[037:039) Identifier |in|
//@[040:041) LeftSquare |[|
//@[041:042) RightSquare |]|
//@[042:043) Colon |:|
//@[044:045) LeftBrace |{|
//@[045:046) RightBrace |}|
//@[046:047) RightSquare |]|
//@[048:049) Colon |:|
//@[050:051) LeftSquare |[|
//@[051:054) Identifier |for|
//@[055:056) Identifier |x|
//@[057:059) Identifier |in|
//@[060:061) LeftSquare |[|
//@[061:062) RightSquare |]|
//@[062:063) Colon |:|
//@[064:065) LeftBrace |{|
//@[065:066) RightBrace |}|
//@[066:067) RightSquare |]|
//@[067:069) NewLine |\r\n|
    dataSources: dataCollectionRule.dataSources
//@[004:015) Identifier |dataSources|
//@[015:016) Colon |:|
//@[017:035) Identifier |dataCollectionRule|
//@[035:036) Dot |.|
//@[036:047) Identifier |dataSources|
//@[047:049) NewLine |\r\n|
    dataFlows: dataCollectionRule.dataFlows
//@[004:013) Identifier |dataFlows|
//@[013:014) Colon |:|
//@[015:033) Identifier |dataCollectionRule|
//@[033:034) Dot |.|
//@[034:043) Identifier |dataFlows|
//@[043:045) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

@description('The language of the Deployment Script. AzurePowerShell or AzureCLI.')
//@[000:001) At |@|
//@[001:012) Identifier |description|
//@[012:013) LeftParen |(|
//@[013:082) StringComplete |'The language of the Deployment Script. AzurePowerShell or AzureCLI.'|
//@[082:083) RightParen |)|
//@[083:085) NewLine |\r\n|
@allowed([
//@[000:001) At |@|
//@[001:008) Identifier |allowed|
//@[008:009) LeftParen |(|
//@[009:010) LeftSquare |[|
//@[010:012) NewLine |\r\n|
  'AzureCLI'
//@[002:012) StringComplete |'AzureCLI'|
//@[012:014) NewLine |\r\n|
  'AzurePowerShell'
//@[002:019) StringComplete |'AzurePowerShell'|
//@[019:021) NewLine |\r\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:004) NewLine |\r\n|
param issue4668_kind string = 'AzureCLI'
//@[000:005) Identifier |param|
//@[006:020) Identifier |issue4668_kind|
//@[021:027) Identifier |string|
//@[028:029) Assignment |=|
//@[030:040) StringComplete |'AzureCLI'|
//@[040:042) NewLine |\r\n|
@description('The identity that will be used to execute the Deployment Script.')
//@[000:001) At |@|
//@[001:012) Identifier |description|
//@[012:013) LeftParen |(|
//@[013:079) StringComplete |'The identity that will be used to execute the Deployment Script.'|
//@[079:080) RightParen |)|
//@[080:082) NewLine |\r\n|
param issue4668_identity object
//@[000:005) Identifier |param|
//@[006:024) Identifier |issue4668_identity|
//@[025:031) Identifier |object|
//@[031:033) NewLine |\r\n|
@description('The properties of the Deployment Script.')
//@[000:001) At |@|
//@[001:012) Identifier |description|
//@[012:013) LeftParen |(|
//@[013:055) StringComplete |'The properties of the Deployment Script.'|
//@[055:056) RightParen |)|
//@[056:058) NewLine |\r\n|
param issue4668_properties object
//@[000:005) Identifier |param|
//@[006:026) Identifier |issue4668_properties|
//@[027:033) Identifier |object|
//@[033:035) NewLine |\r\n|
resource issue4668_mainResource 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:008) Identifier |resource|
//@[009:031) Identifier |issue4668_mainResource|
//@[032:082) StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[086:088) NewLine |\r\n|
  name: 'testscript'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) StringComplete |'testscript'|
//@[020:022) NewLine |\r\n|
  location: 'westeurope'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:024) StringComplete |'westeurope'|
//@[024:026) NewLine |\r\n|
  kind: issue4668_kind
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:022) Identifier |issue4668_kind|
//@[022:024) NewLine |\r\n|
  identity: issue4668_identity
//@[002:010) Identifier |identity|
//@[010:011) Colon |:|
//@[012:030) Identifier |issue4668_identity|
//@[030:032) NewLine |\r\n|
  properties: issue4668_properties
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:034) Identifier |issue4668_properties|
//@[034:036) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// https://github.com/Azure/bicep/issues/8516
//@[045:047) NewLine |\r\n|
resource storage 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |storage|
//@[017:063) StringComplete |'Microsoft.Storage/storageAccounts@2022-05-01'|
//@[064:072) Identifier |existing|
//@[073:074) Assignment |=|
//@[075:076) LeftBrace |{|
//@[076:078) NewLine |\r\n|
  resource blobServices 'blobServices' existing = {
//@[002:010) Identifier |resource|
//@[011:023) Identifier |blobServices|
//@[024:038) StringComplete |'blobServices'|
//@[039:047) Identifier |existing|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:053) NewLine |\r\n|
    name: $account
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:011) Unrecognized |$|
//@[011:018) Identifier |account|
//@[018:020) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|

//@[000:000) EndOfFile ||
