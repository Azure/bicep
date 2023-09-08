
//@[000:002) NewLine |\r\n|
@sys.description('string output description')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:044) StringComplete |'string output description'|
//@[044:045) RightParen |)|
//@[045:047) NewLine |\r\n|
output myStr string = 'hello'
//@[000:006) Identifier |output|
//@[007:012) Identifier |myStr|
//@[013:019) Identifier |string|
//@[020:021) Assignment |=|
//@[022:029) StringComplete |'hello'|
//@[029:033) NewLine |\r\n\r\n|

@sys.description('int output description')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:041) StringComplete |'int output description'|
//@[041:042) RightParen |)|
//@[042:044) NewLine |\r\n|
output myInt int = 7
//@[000:006) Identifier |output|
//@[007:012) Identifier |myInt|
//@[013:016) Identifier |int|
//@[017:018) Assignment |=|
//@[019:020) Integer |7|
//@[020:022) NewLine |\r\n|
output myOtherInt int = 20 / 13 + 80 % -4
//@[000:006) Identifier |output|
//@[007:017) Identifier |myOtherInt|
//@[018:021) Identifier |int|
//@[022:023) Assignment |=|
//@[024:026) Integer |20|
//@[027:028) Slash |/|
//@[029:031) Integer |13|
//@[032:033) Plus |+|
//@[034:036) Integer |80|
//@[037:038) Modulo |%|
//@[039:040) Minus |-|
//@[040:041) Integer |4|
//@[041:045) NewLine |\r\n\r\n|

@sys.description('bool output description')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:042) StringComplete |'bool output description'|
//@[042:043) RightParen |)|
//@[043:045) NewLine |\r\n|
output myBool bool = !false
//@[000:006) Identifier |output|
//@[007:013) Identifier |myBool|
//@[014:018) Identifier |bool|
//@[019:020) Assignment |=|
//@[021:022) Exclamation |!|
//@[022:027) FalseKeyword |false|
//@[027:029) NewLine |\r\n|
output myOtherBool bool = true
//@[000:006) Identifier |output|
//@[007:018) Identifier |myOtherBool|
//@[019:023) Identifier |bool|
//@[024:025) Assignment |=|
//@[026:030) TrueKeyword |true|
//@[030:034) NewLine |\r\n\r\n|

@sys.description('object array description')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:043) StringComplete |'object array description'|
//@[043:044) RightParen |)|
//@[044:046) NewLine |\r\n|
output suchEmpty array = [
//@[000:006) Identifier |output|
//@[007:016) Identifier |suchEmpty|
//@[017:022) Identifier |array|
//@[023:024) Assignment |=|
//@[025:026) LeftSquare |[|
//@[026:028) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

output suchEmpty2 object = {
//@[000:006) Identifier |output|
//@[007:017) Identifier |suchEmpty2|
//@[018:024) Identifier |object|
//@[025:026) Assignment |=|
//@[027:028) LeftBrace |{|
//@[028:030) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

@sys.description('object output description')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:044) StringComplete |'object output description'|
//@[044:045) RightParen |)|
//@[045:047) NewLine |\r\n|
output obj object = {
//@[000:006) Identifier |output|
//@[007:010) Identifier |obj|
//@[011:017) Identifier |object|
//@[018:019) Assignment |=|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
  a: 'a'
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:008) StringComplete |'a'|
//@[008:010) NewLine |\r\n|
  b: 12
//@[002:003) Identifier |b|
//@[003:004) Colon |:|
//@[005:007) Integer |12|
//@[007:009) NewLine |\r\n|
  c: true
//@[002:003) Identifier |c|
//@[003:004) Colon |:|
//@[005:009) TrueKeyword |true|
//@[009:011) NewLine |\r\n|
  d: null
//@[002:003) Identifier |d|
//@[003:004) Colon |:|
//@[005:009) NullKeyword |null|
//@[009:011) NewLine |\r\n|
  list: [
//@[002:006) Identifier |list|
//@[006:007) Colon |:|
//@[008:009) LeftSquare |[|
//@[009:011) NewLine |\r\n|
    1
//@[004:005) Integer |1|
//@[005:007) NewLine |\r\n|
    2
//@[004:005) Integer |2|
//@[005:007) NewLine |\r\n|
    3
//@[004:005) Integer |3|
//@[005:007) NewLine |\r\n|
    null
//@[004:008) NullKeyword |null|
//@[008:010) NewLine |\r\n|
    {
//@[004:005) LeftBrace |{|
//@[005:007) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
  obj: {
//@[002:005) Identifier |obj|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    nested: [
//@[004:010) Identifier |nested|
//@[010:011) Colon |:|
//@[012:013) LeftSquare |[|
//@[013:015) NewLine |\r\n|
      'hello'
//@[006:013) StringComplete |'hello'|
//@[013:015) NewLine |\r\n|
    ]
//@[004:005) RightSquare |]|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

output myArr array = [
//@[000:006) Identifier |output|
//@[007:012) Identifier |myArr|
//@[013:018) Identifier |array|
//@[019:020) Assignment |=|
//@[021:022) LeftSquare |[|
//@[022:024) NewLine |\r\n|
  'pirates'
//@[002:011) StringComplete |'pirates'|
//@[011:013) NewLine |\r\n|
  'say'
//@[002:007) StringComplete |'say'|
//@[007:009) NewLine |\r\n|
   false ? 'arr2' : 'arr'
//@[003:008) FalseKeyword |false|
//@[009:010) Question |?|
//@[011:017) StringComplete |'arr2'|
//@[018:019) Colon |:|
//@[020:025) StringComplete |'arr'|
//@[025:027) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

output rgLocation string = resourceGroup().location
//@[000:006) Identifier |output|
//@[007:017) Identifier |rgLocation|
//@[018:024) Identifier |string|
//@[025:026) Assignment |=|
//@[027:040) Identifier |resourceGroup|
//@[040:041) LeftParen |(|
//@[041:042) RightParen |)|
//@[042:043) Dot |.|
//@[043:051) Identifier |location|
//@[051:055) NewLine |\r\n\r\n|

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[000:006) Identifier |output|
//@[007:015) Identifier |isWestUs|
//@[016:020) Identifier |bool|
//@[021:022) Assignment |=|
//@[023:036) Identifier |resourceGroup|
//@[036:037) LeftParen |(|
//@[037:038) RightParen |)|
//@[038:039) Dot |.|
//@[039:047) Identifier |location|
//@[048:050) NotEquals |!=|
//@[051:059) StringComplete |'westus'|
//@[060:061) Question |?|
//@[062:067) FalseKeyword |false|
//@[068:069) Colon |:|
//@[070:074) TrueKeyword |true|
//@[074:078) NewLine |\r\n\r\n|

output expressionBasedIndexer string = {
//@[000:006) Identifier |output|
//@[007:029) Identifier |expressionBasedIndexer|
//@[030:036) Identifier |string|
//@[037:038) Assignment |=|
//@[039:040) LeftBrace |{|
//@[040:042) NewLine |\r\n|
  eastus: {
//@[002:008) Identifier |eastus|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    foo: true
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:013) TrueKeyword |true|
//@[013:015) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  westus: {
//@[002:008) Identifier |westus|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    foo: false
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:014) FalseKeyword |false|
//@[014:016) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}[resourceGroup().location].foo
//@[000:001) RightBrace |}|
//@[001:002) LeftSquare |[|
//@[002:015) Identifier |resourceGroup|
//@[015:016) LeftParen |(|
//@[016:017) RightParen |)|
//@[017:018) Dot |.|
//@[018:026) Identifier |location|
//@[026:027) RightSquare |]|
//@[027:028) Dot |.|
//@[028:031) Identifier |foo|
//@[031:035) NewLine |\r\n\r\n|

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[000:003) Identifier |var|
//@[004:031) Identifier |secondaryKeyIntermediateVar|
//@[032:033) Assignment |=|
//@[034:042) Identifier |listKeys|
//@[042:043) LeftParen |(|
//@[043:053) Identifier |resourceId|
//@[053:054) LeftParen |(|
//@[054:068) StringComplete |'Mock.RP/type'|
//@[068:069) Comma |,|
//@[070:077) StringComplete |'steve'|
//@[077:078) RightParen |)|
//@[078:079) Comma |,|
//@[080:092) StringComplete |'2020-01-01'|
//@[092:093) RightParen |)|
//@[093:094) Dot |.|
//@[094:106) Identifier |secondaryKey|
//@[106:110) NewLine |\r\n\r\n|

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[000:006) Identifier |output|
//@[007:017) Identifier |primaryKey|
//@[018:024) Identifier |string|
//@[025:026) Assignment |=|
//@[027:035) Identifier |listKeys|
//@[035:036) LeftParen |(|
//@[036:046) Identifier |resourceId|
//@[046:047) LeftParen |(|
//@[047:061) StringComplete |'Mock.RP/type'|
//@[061:062) Comma |,|
//@[063:070) StringComplete |'nigel'|
//@[070:071) RightParen |)|
//@[071:072) Comma |,|
//@[073:085) StringComplete |'2020-01-01'|
//@[085:086) RightParen |)|
//@[086:087) Dot |.|
//@[087:097) Identifier |primaryKey|
//@[097:099) NewLine |\r\n|
output secondaryKey string = secondaryKeyIntermediateVar
//@[000:006) Identifier |output|
//@[007:019) Identifier |secondaryKey|
//@[020:026) Identifier |string|
//@[027:028) Assignment |=|
//@[029:056) Identifier |secondaryKeyIntermediateVar|
//@[056:060) NewLine |\r\n\r\n|

var varWithOverlappingOutput = 'hello'
//@[000:003) Identifier |var|
//@[004:028) Identifier |varWithOverlappingOutput|
//@[029:030) Assignment |=|
//@[031:038) StringComplete |'hello'|
//@[038:040) NewLine |\r\n|
param paramWithOverlappingOutput string
//@[000:005) Identifier |param|
//@[006:032) Identifier |paramWithOverlappingOutput|
//@[033:039) Identifier |string|
//@[039:043) NewLine |\r\n\r\n|

output varWithOverlappingOutput string = varWithOverlappingOutput
//@[000:006) Identifier |output|
//@[007:031) Identifier |varWithOverlappingOutput|
//@[032:038) Identifier |string|
//@[039:040) Assignment |=|
//@[041:065) Identifier |varWithOverlappingOutput|
//@[065:067) NewLine |\r\n|
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@[000:006) Identifier |output|
//@[007:033) Identifier |paramWithOverlappingOutput|
//@[034:040) Identifier |string|
//@[041:042) Assignment |=|
//@[043:069) Identifier |paramWithOverlappingOutput|
//@[069:073) NewLine |\r\n\r\n|

// top-level output loops are supported
//@[039:041) NewLine |\r\n|
output generatedArray array = [for i in range(0,10): i]
//@[000:006) Identifier |output|
//@[007:021) Identifier |generatedArray|
//@[022:027) Identifier |array|
//@[028:029) Assignment |=|
//@[030:031) LeftSquare |[|
//@[031:034) Identifier |for|
//@[035:036) Identifier |i|
//@[037:039) Identifier |in|
//@[040:045) Identifier |range|
//@[045:046) LeftParen |(|
//@[046:047) Integer |0|
//@[047:048) Comma |,|
//@[048:050) Integer |10|
//@[050:051) RightParen |)|
//@[051:052) Colon |:|
//@[053:054) Identifier |i|
//@[054:055) RightSquare |]|
//@[055:057) NewLine |\r\n|

//@[000:000) EndOfFile ||
