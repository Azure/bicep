
//@[000:001) NewLine |\n|
// int
//@[006:007) NewLine |\n|
@sys.description('an int variable')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:034) StringComplete |'an int variable'|
//@[034:035) RightParen |)|
//@[035:036) NewLine |\n|
var myInt = 42
//@[000:003) Identifier |var|
//@[004:009) Identifier |myInt|
//@[010:011) Assignment |=|
//@[012:014) Integer |42|
//@[014:016) NewLine |\n\n|

// string
//@[009:010) NewLine |\n|
@sys.description('a string variable')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:036) StringComplete |'a string variable'|
//@[036:037) RightParen |)|
//@[037:038) NewLine |\n|
var myStr = 'str'
//@[000:003) Identifier |var|
//@[004:009) Identifier |myStr|
//@[010:011) Assignment |=|
//@[012:017) StringComplete |'str'|
//@[017:018) NewLine |\n|
var curliesWithNoInterp = '}{1}{'
//@[000:003) Identifier |var|
//@[004:023) Identifier |curliesWithNoInterp|
//@[024:025) Assignment |=|
//@[026:033) StringComplete |'}{1}{'|
//@[033:034) NewLine |\n|
var interp1 = 'abc${123}def'
//@[000:003) Identifier |var|
//@[004:011) Identifier |interp1|
//@[012:013) Assignment |=|
//@[014:020) StringLeftPiece |'abc${|
//@[020:023) Integer |123|
//@[023:028) StringRightPiece |}def'|
//@[028:029) NewLine |\n|
var interp2 = '${123}def'
//@[000:003) Identifier |var|
//@[004:011) Identifier |interp2|
//@[012:013) Assignment |=|
//@[014:017) StringLeftPiece |'${|
//@[017:020) Integer |123|
//@[020:025) StringRightPiece |}def'|
//@[025:026) NewLine |\n|
var interp3 = 'abc${123}'
//@[000:003) Identifier |var|
//@[004:011) Identifier |interp3|
//@[012:013) Assignment |=|
//@[014:020) StringLeftPiece |'abc${|
//@[020:023) Integer |123|
//@[023:025) StringRightPiece |}'|
//@[025:026) NewLine |\n|
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[000:003) Identifier |var|
//@[004:011) Identifier |interp4|
//@[012:013) Assignment |=|
//@[014:020) StringLeftPiece |'abc${|
//@[020:023) Integer |123|
//@[023:026) StringMiddlePiece |}${|
//@[026:029) Integer |456|
//@[029:036) StringMiddlePiece |}jk$l${|
//@[036:039) Integer |789|
//@[039:043) StringRightPiece |}p$'|
//@[043:044) NewLine |\n|
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[000:003) Identifier |var|
//@[004:016) Identifier |doubleInterp|
//@[017:018) Assignment |=|
//@[019:025) StringLeftPiece |'abc${|
//@[025:031) StringLeftPiece |'def${|
//@[031:034) Integer |123|
//@[034:036) StringRightPiece |}'|
//@[036:040) StringMiddlePiece |}_${|
//@[040:043) StringLeftPiece |'${|
//@[043:046) Integer |456|
//@[046:049) StringMiddlePiece |}${|
//@[049:052) Integer |789|
//@[052:054) StringRightPiece |}'|
//@[054:056) StringRightPiece |}'|
//@[056:057) NewLine |\n|
var curliesInInterp = '{${123}{0}${true}}'
//@[000:003) Identifier |var|
//@[004:019) Identifier |curliesInInterp|
//@[020:021) Assignment |=|
//@[022:026) StringLeftPiece |'{${|
//@[026:029) Integer |123|
//@[029:035) StringMiddlePiece |}{0}${|
//@[035:039) TrueKeyword |true|
//@[039:042) StringRightPiece |}}'|
//@[042:044) NewLine |\n\n|

// #completionTest(0) -> declarations
//@[037:039) NewLine |\n\n|

// verify correct bracket escaping
//@[034:035) NewLine |\n|
var bracketInTheMiddle = 'a[b]'
//@[000:003) Identifier |var|
//@[004:022) Identifier |bracketInTheMiddle|
//@[023:024) Assignment |=|
//@[025:031) StringComplete |'a[b]'|
//@[031:032) NewLine |\n|
// #completionTest(25) -> empty
//@[031:032) NewLine |\n|
var bracketAtBeginning = '[test'
//@[000:003) Identifier |var|
//@[004:022) Identifier |bracketAtBeginning|
//@[023:024) Assignment |=|
//@[025:032) StringComplete |'[test'|
//@[032:033) NewLine |\n|
// #completionTest(23) -> symbolsPlusTypes
//@[042:043) NewLine |\n|
var enclosingBrackets = '[test]'
//@[000:003) Identifier |var|
//@[004:021) Identifier |enclosingBrackets|
//@[022:023) Assignment |=|
//@[024:032) StringComplete |'[test]'|
//@[032:033) NewLine |\n|
var emptyJsonArray = '[]'
//@[000:003) Identifier |var|
//@[004:018) Identifier |emptyJsonArray|
//@[019:020) Assignment |=|
//@[021:025) StringComplete |'[]'|
//@[025:026) NewLine |\n|
var interpolatedBrackets = '[${myInt}]'
//@[000:003) Identifier |var|
//@[004:024) Identifier |interpolatedBrackets|
//@[025:026) Assignment |=|
//@[027:031) StringLeftPiece |'[${|
//@[031:036) Identifier |myInt|
//@[036:039) StringRightPiece |}]'|
//@[039:040) NewLine |\n|
var nestedBrackets = '[test[]test2]'
//@[000:003) Identifier |var|
//@[004:018) Identifier |nestedBrackets|
//@[019:020) Assignment |=|
//@[021:036) StringComplete |'[test[]test2]'|
//@[036:037) NewLine |\n|
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[000:003) Identifier |var|
//@[004:030) Identifier |nestedInterpolatedBrackets|
//@[031:032) Assignment |=|
//@[033:037) StringLeftPiece |'[${|
//@[037:051) Identifier |emptyJsonArray|
//@[051:054) StringRightPiece |}]'|
//@[054:055) NewLine |\n|
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[000:003) Identifier |var|
//@[004:029) Identifier |bracketStringInExpression|
//@[030:031) Assignment |=|
//@[032:038) Identifier |concat|
//@[038:039) LeftParen |(|
//@[039:042) StringComplete |'['|
//@[042:043) Comma |,|
//@[044:054) StringComplete |'\'test\''|
//@[054:055) Comma |,|
//@[055:058) StringComplete |']'|
//@[058:059) RightParen |)|
//@[059:061) NewLine |\n\n|

// booleans
//@[011:012) NewLine |\n|
@sys.description('a bool variable')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:034) StringComplete |'a bool variable'|
//@[034:035) RightParen |)|
//@[035:036) NewLine |\n|
var myTruth = true
//@[000:003) Identifier |var|
//@[004:011) Identifier |myTruth|
//@[012:013) Assignment |=|
//@[014:018) TrueKeyword |true|
//@[018:019) NewLine |\n|
var myFalsehood = false
//@[000:003) Identifier |var|
//@[004:015) Identifier |myFalsehood|
//@[016:017) Assignment |=|
//@[018:023) FalseKeyword |false|
//@[023:025) NewLine |\n\n|

var myEmptyObj = { }
//@[000:003) Identifier |var|
//@[004:014) Identifier |myEmptyObj|
//@[015:016) Assignment |=|
//@[017:018) LeftBrace |{|
//@[019:020) RightBrace |}|
//@[020:021) NewLine |\n|
var myEmptyArray = [ ]
//@[000:003) Identifier |var|
//@[004:016) Identifier |myEmptyArray|
//@[017:018) Assignment |=|
//@[019:020) LeftSquare |[|
//@[021:022) RightSquare |]|
//@[022:024) NewLine |\n\n|

// object
//@[009:010) NewLine |\n|
@sys.description('a object variable')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:036) StringComplete |'a object variable'|
//@[036:037) RightParen |)|
//@[037:038) NewLine |\n|
var myObj = {
//@[000:003) Identifier |var|
//@[004:009) Identifier |myObj|
//@[010:011) Assignment |=|
//@[012:013) LeftBrace |{|
//@[013:014) NewLine |\n|
  a: 'a'
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:008) StringComplete |'a'|
//@[008:009) NewLine |\n|
  b: -12
//@[002:003) Identifier |b|
//@[003:004) Colon |:|
//@[005:006) Minus |-|
//@[006:008) Integer |12|
//@[008:009) NewLine |\n|
  c: true
//@[002:003) Identifier |c|
//@[003:004) Colon |:|
//@[005:009) TrueKeyword |true|
//@[009:010) NewLine |\n|
  d: !true
//@[002:003) Identifier |d|
//@[003:004) Colon |:|
//@[005:006) Exclamation |!|
//@[006:010) TrueKeyword |true|
//@[010:011) NewLine |\n|
  list: [
//@[002:006) Identifier |list|
//@[006:007) Colon |:|
//@[008:009) LeftSquare |[|
//@[009:010) NewLine |\n|
    1
//@[004:005) Integer |1|
//@[005:006) NewLine |\n|
    2
//@[004:005) Integer |2|
//@[005:006) NewLine |\n|
    2+1
//@[004:005) Integer |2|
//@[005:006) Plus |+|
//@[006:007) Integer |1|
//@[007:008) NewLine |\n|
    {
//@[004:005) LeftBrace |{|
//@[005:006) NewLine |\n|
      test: 144 > 33 && true || 99 <= 199
//@[006:010) Identifier |test|
//@[010:011) Colon |:|
//@[012:015) Integer |144|
//@[016:017) RightChevron |>|
//@[018:020) Integer |33|
//@[021:023) LogicalAnd |&&|
//@[024:028) TrueKeyword |true|
//@[029:031) LogicalOr ||||
//@[032:034) Integer |99|
//@[035:037) LessThanOrEqual |<=|
//@[038:041) Integer |199|
//@[041:042) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
    'a' =~ 'b'
//@[004:007) StringComplete |'a'|
//@[008:010) EqualsInsensitive |=~|
//@[011:014) StringComplete |'b'|
//@[014:015) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
  obj: {
//@[002:005) Identifier |obj|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    nested: [
//@[004:010) Identifier |nested|
//@[010:011) Colon |:|
//@[012:013) LeftSquare |[|
//@[013:014) NewLine |\n|
      'hello'
//@[006:013) StringComplete |'hello'|
//@[013:014) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

@sys.description('a object with interp')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:039) StringComplete |'a object with interp'|
//@[039:040) RightParen |)|
//@[040:041) NewLine |\n|
var objWithInterp = {
//@[000:003) Identifier |var|
//@[004:017) Identifier |objWithInterp|
//@[018:019) Assignment |=|
//@[020:021) LeftBrace |{|
//@[021:022) NewLine |\n|
  '${myStr}': 1
//@[002:005) StringLeftPiece |'${|
//@[005:010) Identifier |myStr|
//@[010:012) StringRightPiece |}'|
//@[012:013) Colon |:|
//@[014:015) Integer |1|
//@[015:016) NewLine |\n|
  'abc${myStr}def': 2
//@[002:008) StringLeftPiece |'abc${|
//@[008:013) Identifier |myStr|
//@[013:018) StringRightPiece |}def'|
//@[018:019) Colon |:|
//@[020:021) Integer |2|
//@[021:022) NewLine |\n|
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[002:005) StringLeftPiece |'${|
//@[005:012) Identifier |interp1|
//@[012:018) StringMiddlePiece |}abc${|
//@[018:025) Identifier |interp2|
//@[025:027) StringRightPiece |}'|
//@[027:028) Colon |:|
//@[029:032) StringLeftPiece |'${|
//@[032:039) Identifier |interp1|
//@[039:045) StringMiddlePiece |}abc${|
//@[045:052) Identifier |interp2|
//@[052:054) StringRightPiece |}'|
//@[054:055) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// array
//@[008:009) NewLine |\n|
var myArr = [
//@[000:003) Identifier |var|
//@[004:009) Identifier |myArr|
//@[010:011) Assignment |=|
//@[012:013) LeftSquare |[|
//@[013:014) NewLine |\n|
  'pirates'
//@[002:011) StringComplete |'pirates'|
//@[011:012) NewLine |\n|
  'say'
//@[002:007) StringComplete |'say'|
//@[007:008) NewLine |\n|
  'arr'
//@[002:007) StringComplete |'arr'|
//@[007:008) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

// array with objects
//@[021:022) NewLine |\n|
var myArrWithObjects = [
//@[000:003) Identifier |var|
//@[004:020) Identifier |myArrWithObjects|
//@[021:022) Assignment |=|
//@[023:024) LeftSquare |[|
//@[024:025) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    name: 'one'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'one'|
//@[015:016) NewLine |\n|
    enable: true
//@[004:010) Identifier |enable|
//@[010:011) Colon |:|
//@[012:016) TrueKeyword |true|
//@[016:017) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    name: 'two'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'two'|
//@[015:016) NewLine |\n|
    enable: false && false || 'two' !~ 'three'
//@[004:010) Identifier |enable|
//@[010:011) Colon |:|
//@[012:017) FalseKeyword |false|
//@[018:020) LogicalAnd |&&|
//@[021:026) FalseKeyword |false|
//@[027:029) LogicalOr ||||
//@[030:035) StringComplete |'two'|
//@[036:038) NotEqualsInsensitive |!~|
//@[039:046) StringComplete |'three'|
//@[046:047) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

var expressionIndexOnAny = any({
//@[000:003) Identifier |var|
//@[004:024) Identifier |expressionIndexOnAny|
//@[025:026) Assignment |=|
//@[027:030) Identifier |any|
//@[030:031) LeftParen |(|
//@[031:032) LeftBrace |{|
//@[032:033) NewLine |\n|
})[az.resourceGroup().location]
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) LeftSquare |[|
//@[003:005) Identifier |az|
//@[005:006) Dot |.|
//@[006:019) Identifier |resourceGroup|
//@[019:020) LeftParen |(|
//@[020:021) RightParen |)|
//@[021:022) Dot |.|
//@[022:030) Identifier |location|
//@[030:031) RightSquare |]|
//@[031:033) NewLine |\n\n|

var anyIndexOnAny = any(true)[any(false)]
//@[000:003) Identifier |var|
//@[004:017) Identifier |anyIndexOnAny|
//@[018:019) Assignment |=|
//@[020:023) Identifier |any|
//@[023:024) LeftParen |(|
//@[024:028) TrueKeyword |true|
//@[028:029) RightParen |)|
//@[029:030) LeftSquare |[|
//@[030:033) Identifier |any|
//@[033:034) LeftParen |(|
//@[034:039) FalseKeyword |false|
//@[039:040) RightParen |)|
//@[040:041) RightSquare |]|
//@[041:043) NewLine |\n\n|

var deploymentName = deployment().name
//@[000:003) Identifier |var|
//@[004:018) Identifier |deploymentName|
//@[019:020) Assignment |=|
//@[021:031) Identifier |deployment|
//@[031:032) LeftParen |(|
//@[032:033) RightParen |)|
//@[033:034) Dot |.|
//@[034:038) Identifier |name|
//@[038:039) NewLine |\n|
var templateContentVersion = deployment().properties.template.contentVersion
//@[000:003) Identifier |var|
//@[004:026) Identifier |templateContentVersion|
//@[027:028) Assignment |=|
//@[029:039) Identifier |deployment|
//@[039:040) LeftParen |(|
//@[040:041) RightParen |)|
//@[041:042) Dot |.|
//@[042:052) Identifier |properties|
//@[052:053) Dot |.|
//@[053:061) Identifier |template|
//@[061:062) Dot |.|
//@[062:076) Identifier |contentVersion|
//@[076:077) NewLine |\n|
var templateLinkUri = deployment().properties.templateLink.uri
//@[000:003) Identifier |var|
//@[004:019) Identifier |templateLinkUri|
//@[020:021) Assignment |=|
//@[022:032) Identifier |deployment|
//@[032:033) LeftParen |(|
//@[033:034) RightParen |)|
//@[034:035) Dot |.|
//@[035:045) Identifier |properties|
//@[045:046) Dot |.|
//@[046:058) Identifier |templateLink|
//@[058:059) Dot |.|
//@[059:062) Identifier |uri|
//@[062:063) NewLine |\n|
var templateLinkId = deployment().properties.templateLink.id
//@[000:003) Identifier |var|
//@[004:018) Identifier |templateLinkId|
//@[019:020) Assignment |=|
//@[021:031) Identifier |deployment|
//@[031:032) LeftParen |(|
//@[032:033) RightParen |)|
//@[033:034) Dot |.|
//@[034:044) Identifier |properties|
//@[044:045) Dot |.|
//@[045:057) Identifier |templateLink|
//@[057:058) Dot |.|
//@[058:060) Identifier |id|
//@[060:062) NewLine |\n\n|

var portalEndpoint = environment().portal
//@[000:003) Identifier |var|
//@[004:018) Identifier |portalEndpoint|
//@[019:020) Assignment |=|
//@[021:032) Identifier |environment|
//@[032:033) LeftParen |(|
//@[033:034) RightParen |)|
//@[034:035) Dot |.|
//@[035:041) Identifier |portal|
//@[041:042) NewLine |\n|
var loginEndpoint = environment().authentication.loginEndpoint
//@[000:003) Identifier |var|
//@[004:017) Identifier |loginEndpoint|
//@[018:019) Assignment |=|
//@[020:031) Identifier |environment|
//@[031:032) LeftParen |(|
//@[032:033) RightParen |)|
//@[033:034) Dot |.|
//@[034:048) Identifier |authentication|
//@[048:049) Dot |.|
//@[049:062) Identifier |loginEndpoint|
//@[062:064) NewLine |\n\n|

var namedPropertyIndexer = {
//@[000:003) Identifier |var|
//@[004:024) Identifier |namedPropertyIndexer|
//@[025:026) Assignment |=|
//@[027:028) LeftBrace |{|
//@[028:029) NewLine |\n|
  foo: 's'
//@[002:005) Identifier |foo|
//@[005:006) Colon |:|
//@[007:010) StringComplete |'s'|
//@[010:011) NewLine |\n|
}['foo']
//@[000:001) RightBrace |}|
//@[001:002) LeftSquare |[|
//@[002:007) StringComplete |'foo'|
//@[007:008) RightSquare |]|
//@[008:010) NewLine |\n\n|

var intIndexer = [
//@[000:003) Identifier |var|
//@[004:014) Identifier |intIndexer|
//@[015:016) Assignment |=|
//@[017:018) LeftSquare |[|
//@[018:019) NewLine |\n|
  's'
//@[002:005) StringComplete |'s'|
//@[005:006) NewLine |\n|
][0]
//@[000:001) RightSquare |]|
//@[001:002) LeftSquare |[|
//@[002:003) Integer |0|
//@[003:004) RightSquare |]|
//@[004:006) NewLine |\n\n|

var functionOnIndexer1 = concat([
//@[000:003) Identifier |var|
//@[004:022) Identifier |functionOnIndexer1|
//@[023:024) Assignment |=|
//@[025:031) Identifier |concat|
//@[031:032) LeftParen |(|
//@[032:033) LeftSquare |[|
//@[033:034) NewLine |\n|
  's'
//@[002:005) StringComplete |'s'|
//@[005:006) NewLine |\n|
][0], 's')
//@[000:001) RightSquare |]|
//@[001:002) LeftSquare |[|
//@[002:003) Integer |0|
//@[003:004) RightSquare |]|
//@[004:005) Comma |,|
//@[006:009) StringComplete |'s'|
//@[009:010) RightParen |)|
//@[010:012) NewLine |\n\n|

var singleQuote = '\''
//@[000:003) Identifier |var|
//@[004:015) Identifier |singleQuote|
//@[016:017) Assignment |=|
//@[018:022) StringComplete |'\''|
//@[022:023) NewLine |\n|
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[000:003) Identifier |var|
//@[004:018) Identifier |myPropertyName|
//@[019:020) Assignment |=|
//@[021:024) StringLeftPiece |'${|
//@[024:035) Identifier |singleQuote|
//@[035:041) StringMiddlePiece |}foo${|
//@[041:052) Identifier |singleQuote|
//@[052:054) StringRightPiece |}'|
//@[054:056) NewLine |\n\n|

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
//@[000:003) Identifier |var|
//@[004:022) Identifier |unusedIntermediate|
//@[023:024) Assignment |=|
//@[025:033) Identifier |listKeys|
//@[033:034) LeftParen |(|
//@[034:044) Identifier |resourceId|
//@[044:045) LeftParen |(|
//@[045:059) StringComplete |'Mock.RP/type'|
//@[059:060) Comma |,|
//@[061:068) StringComplete |'steve'|
//@[068:069) RightParen |)|
//@[069:070) Comma |,|
//@[071:083) StringComplete |'2020-01-01'|
//@[083:084) RightParen |)|
//@[084:085) NewLine |\n|
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[000:003) Identifier |var|
//@[004:025) Identifier |unusedIntermediateRef|
//@[026:027) Assignment |=|
//@[028:046) Identifier |unusedIntermediate|
//@[046:047) Dot |.|
//@[047:059) Identifier |secondaryKey|
//@[059:061) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[053:054) NewLine |\n|
var previousEmitLimit = [
//@[000:003) Identifier |var|
//@[004:021) Identifier |previousEmitLimit|
//@[022:023) Assignment |=|
//@[024:025) LeftSquare |[|
//@[025:026) NewLine |\n|
  concat('s')
//@[002:008) Identifier |concat|
//@[008:009) LeftParen |(|
//@[009:012) StringComplete |'s'|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
  '${4}'
//@[002:005) StringLeftPiece |'${|
//@[005:006) Integer |4|
//@[006:008) StringRightPiece |}'|
//@[008:009) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    a: {
//@[004:005) Identifier |a|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
      b: base64('s')
//@[006:007) Identifier |b|
//@[007:008) Colon |:|
//@[009:015) Identifier |base64|
//@[015:016) LeftParen |(|
//@[016:019) StringComplete |'s'|
//@[019:020) RightParen |)|
//@[020:021) NewLine |\n|
      c: concat([
//@[006:007) Identifier |c|
//@[007:008) Colon |:|
//@[009:015) Identifier |concat|
//@[015:016) LeftParen |(|
//@[016:017) LeftSquare |[|
//@[017:018) NewLine |\n|
        12 + 3
//@[008:010) Integer |12|
//@[011:012) Plus |+|
//@[013:014) Integer |3|
//@[014:015) NewLine |\n|
      ], [
//@[006:007) RightSquare |]|
//@[007:008) Comma |,|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
        !true
//@[008:009) Exclamation |!|
//@[009:013) TrueKeyword |true|
//@[013:014) NewLine |\n|
        'hello'
//@[008:015) StringComplete |'hello'|
//@[015:016) NewLine |\n|
      ])
//@[006:007) RightSquare |]|
//@[007:008) RightParen |)|
//@[008:009) NewLine |\n|
      d: az.resourceGroup().location
//@[006:007) Identifier |d|
//@[007:008) Colon |:|
//@[009:011) Identifier |az|
//@[011:012) Dot |.|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:037) NewLine |\n|
      e: concat([
//@[006:007) Identifier |e|
//@[007:008) Colon |:|
//@[009:015) Identifier |concat|
//@[015:016) LeftParen |(|
//@[016:017) LeftSquare |[|
//@[017:018) NewLine |\n|
        true
//@[008:012) TrueKeyword |true|
//@[012:013) NewLine |\n|
      ])
//@[006:007) RightSquare |]|
//@[007:008) RightParen |)|
//@[008:009) NewLine |\n|
      f: concat([
//@[006:007) Identifier |f|
//@[007:008) Colon |:|
//@[009:015) Identifier |concat|
//@[015:016) LeftParen |(|
//@[016:017) LeftSquare |[|
//@[017:018) NewLine |\n|
        's' == 12
//@[008:011) StringComplete |'s'|
//@[012:014) Equals |==|
//@[015:017) Integer |12|
//@[017:018) NewLine |\n|
      ])
//@[006:007) RightSquare |]|
//@[007:008) RightParen |)|
//@[008:009) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[053:054) NewLine |\n|
var previousEmitLimit2 = [
//@[000:003) Identifier |var|
//@[004:022) Identifier |previousEmitLimit2|
//@[023:024) Assignment |=|
//@[025:026) LeftSquare |[|
//@[026:027) NewLine |\n|
  concat('s')
//@[002:008) Identifier |concat|
//@[008:009) LeftParen |(|
//@[009:012) StringComplete |'s'|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
  '${4}'
//@[002:005) StringLeftPiece |'${|
//@[005:006) Integer |4|
//@[006:008) StringRightPiece |}'|
//@[008:009) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    a: {
//@[004:005) Identifier |a|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
      b: base64('s')
//@[006:007) Identifier |b|
//@[007:008) Colon |:|
//@[009:015) Identifier |base64|
//@[015:016) LeftParen |(|
//@[016:019) StringComplete |'s'|
//@[019:020) RightParen |)|
//@[020:021) NewLine |\n|
      c: union({
//@[006:007) Identifier |c|
//@[007:008) Colon |:|
//@[009:014) Identifier |union|
//@[014:015) LeftParen |(|
//@[015:016) LeftBrace |{|
//@[016:017) NewLine |\n|
        a: 12 + 3
//@[008:009) Identifier |a|
//@[009:010) Colon |:|
//@[011:013) Integer |12|
//@[014:015) Plus |+|
//@[016:017) Integer |3|
//@[017:018) NewLine |\n|
      }, {
//@[006:007) RightBrace |}|
//@[007:008) Comma |,|
//@[009:010) LeftBrace |{|
//@[010:011) NewLine |\n|
        b: !true
//@[008:009) Identifier |b|
//@[009:010) Colon |:|
//@[011:012) Exclamation |!|
//@[012:016) TrueKeyword |true|
//@[016:017) NewLine |\n|
        c: 'hello'
//@[008:009) Identifier |c|
//@[009:010) Colon |:|
//@[011:018) StringComplete |'hello'|
//@[018:019) NewLine |\n|
      })
//@[006:007) RightBrace |}|
//@[007:008) RightParen |)|
//@[008:009) NewLine |\n|
      d: az.resourceGroup().location
//@[006:007) Identifier |d|
//@[007:008) Colon |:|
//@[009:011) Identifier |az|
//@[011:012) Dot |.|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:037) NewLine |\n|
      e: union({
//@[006:007) Identifier |e|
//@[007:008) Colon |:|
//@[009:014) Identifier |union|
//@[014:015) LeftParen |(|
//@[015:016) LeftBrace |{|
//@[016:017) NewLine |\n|
        x: true
//@[008:009) Identifier |x|
//@[009:010) Colon |:|
//@[011:015) TrueKeyword |true|
//@[015:016) NewLine |\n|
      }, {})
//@[006:007) RightBrace |}|
//@[007:008) Comma |,|
//@[009:010) LeftBrace |{|
//@[010:011) RightBrace |}|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
      f: intersection({
//@[006:007) Identifier |f|
//@[007:008) Colon |:|
//@[009:021) Identifier |intersection|
//@[021:022) LeftParen |(|
//@[022:023) LeftBrace |{|
//@[023:024) NewLine |\n|
        q: 's' == 12
//@[008:009) Identifier |q|
//@[009:010) Colon |:|
//@[011:014) StringComplete |'s'|
//@[015:017) Equals |==|
//@[018:020) Integer |12|
//@[020:021) NewLine |\n|
      }, {})
//@[006:007) RightBrace |}|
//@[007:008) Comma |,|
//@[009:010) LeftBrace |{|
//@[010:011) RightBrace |}|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[053:054) NewLine |\n|
var previousEmitLimit3 = {
//@[000:003) Identifier |var|
//@[004:022) Identifier |previousEmitLimit3|
//@[023:024) Assignment |=|
//@[025:026) LeftBrace |{|
//@[026:027) NewLine |\n|
  a: {
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:006) LeftBrace |{|
//@[006:007) NewLine |\n|
    b: {
//@[004:005) Identifier |b|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
      a: az.resourceGroup().location
//@[006:007) Identifier |a|
//@[007:008) Colon |:|
//@[009:011) Identifier |az|
//@[011:012) Dot |.|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:037) NewLine |\n|
    } == 2
//@[004:005) RightBrace |}|
//@[006:008) Equals |==|
//@[009:010) Integer |2|
//@[010:011) NewLine |\n|
    c: concat([
//@[004:005) Identifier |c|
//@[005:006) Colon |:|
//@[007:013) Identifier |concat|
//@[013:014) LeftParen |(|
//@[014:015) LeftSquare |[|
//@[015:017) NewLine |\n\n|

    ], [
//@[004:005) RightSquare |]|
//@[005:006) Comma |,|
//@[007:008) LeftSquare |[|
//@[008:009) NewLine |\n|
      true
//@[006:010) TrueKeyword |true|
//@[010:011) NewLine |\n|
    ])
//@[004:005) RightSquare |]|
//@[005:006) RightParen |)|
//@[006:007) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// #completionTest(0) -> declarations
//@[037:039) NewLine |\n\n|

var myVar = 'hello'
//@[000:003) Identifier |var|
//@[004:009) Identifier |myVar|
//@[010:011) Assignment |=|
//@[012:019) StringComplete |'hello'|
//@[019:020) NewLine |\n|
var myVar2 = any({
//@[000:003) Identifier |var|
//@[004:010) Identifier |myVar2|
//@[011:012) Assignment |=|
//@[013:016) Identifier |any|
//@[016:017) LeftParen |(|
//@[017:018) LeftBrace |{|
//@[018:019) NewLine |\n|
  something: myVar
//@[002:011) Identifier |something|
//@[011:012) Colon |:|
//@[013:018) Identifier |myVar|
//@[018:019) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
var myVar3 = any(any({
//@[000:003) Identifier |var|
//@[004:010) Identifier |myVar3|
//@[011:012) Assignment |=|
//@[013:016) Identifier |any|
//@[016:017) LeftParen |(|
//@[017:020) Identifier |any|
//@[020:021) LeftParen |(|
//@[021:022) LeftBrace |{|
//@[022:023) NewLine |\n|
  something: myVar
//@[002:011) Identifier |something|
//@[011:012) Colon |:|
//@[013:018) Identifier |myVar|
//@[018:019) NewLine |\n|
}))
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) RightParen |)|
//@[003:004) NewLine |\n|
var myVar4 = length(any(concat('s','a')))
//@[000:003) Identifier |var|
//@[004:010) Identifier |myVar4|
//@[011:012) Assignment |=|
//@[013:019) Identifier |length|
//@[019:020) LeftParen |(|
//@[020:023) Identifier |any|
//@[023:024) LeftParen |(|
//@[024:030) Identifier |concat|
//@[030:031) LeftParen |(|
//@[031:034) StringComplete |'s'|
//@[034:035) Comma |,|
//@[035:038) StringComplete |'a'|
//@[038:039) RightParen |)|
//@[039:040) RightParen |)|
//@[040:041) RightParen |)|
//@[041:043) NewLine |\n\n|

// verify that unqualified banned function identifiers can be used as declaration identifiers
//@[093:094) NewLine |\n|
var variables = true
//@[000:003) Identifier |var|
//@[004:013) Identifier |variables|
//@[014:015) Assignment |=|
//@[016:020) TrueKeyword |true|
//@[020:021) NewLine |\n|
param parameters bool = true
//@[000:005) Identifier |param|
//@[006:016) Identifier |parameters|
//@[017:021) Identifier |bool|
//@[022:023) Assignment |=|
//@[024:028) TrueKeyword |true|
//@[028:029) NewLine |\n|
var if = true
//@[000:003) Identifier |var|
//@[004:006) Identifier |if|
//@[007:008) Assignment |=|
//@[009:013) TrueKeyword |true|
//@[013:014) NewLine |\n|
var createArray = true
//@[000:003) Identifier |var|
//@[004:015) Identifier |createArray|
//@[016:017) Assignment |=|
//@[018:022) TrueKeyword |true|
//@[022:023) NewLine |\n|
var createObject = true
//@[000:003) Identifier |var|
//@[004:016) Identifier |createObject|
//@[017:018) Assignment |=|
//@[019:023) TrueKeyword |true|
//@[023:024) NewLine |\n|
var add = true
//@[000:003) Identifier |var|
//@[004:007) Identifier |add|
//@[008:009) Assignment |=|
//@[010:014) TrueKeyword |true|
//@[014:015) NewLine |\n|
var sub = true
//@[000:003) Identifier |var|
//@[004:007) Identifier |sub|
//@[008:009) Assignment |=|
//@[010:014) TrueKeyword |true|
//@[014:015) NewLine |\n|
var mul = true
//@[000:003) Identifier |var|
//@[004:007) Identifier |mul|
//@[008:009) Assignment |=|
//@[010:014) TrueKeyword |true|
//@[014:015) NewLine |\n|
var div = true
//@[000:003) Identifier |var|
//@[004:007) Identifier |div|
//@[008:009) Assignment |=|
//@[010:014) TrueKeyword |true|
//@[014:015) NewLine |\n|
param mod bool = true
//@[000:005) Identifier |param|
//@[006:009) Identifier |mod|
//@[010:014) Identifier |bool|
//@[015:016) Assignment |=|
//@[017:021) TrueKeyword |true|
//@[021:022) NewLine |\n|
var less = true
//@[000:003) Identifier |var|
//@[004:008) Identifier |less|
//@[009:010) Assignment |=|
//@[011:015) TrueKeyword |true|
//@[015:016) NewLine |\n|
var lessOrEquals = true
//@[000:003) Identifier |var|
//@[004:016) Identifier |lessOrEquals|
//@[017:018) Assignment |=|
//@[019:023) TrueKeyword |true|
//@[023:024) NewLine |\n|
var greater = true
//@[000:003) Identifier |var|
//@[004:011) Identifier |greater|
//@[012:013) Assignment |=|
//@[014:018) TrueKeyword |true|
//@[018:019) NewLine |\n|
var greaterOrEquals = true
//@[000:003) Identifier |var|
//@[004:019) Identifier |greaterOrEquals|
//@[020:021) Assignment |=|
//@[022:026) TrueKeyword |true|
//@[026:027) NewLine |\n|
param equals bool = true
//@[000:005) Identifier |param|
//@[006:012) Identifier |equals|
//@[013:017) Identifier |bool|
//@[018:019) Assignment |=|
//@[020:024) TrueKeyword |true|
//@[024:025) NewLine |\n|
var not = true
//@[000:003) Identifier |var|
//@[004:007) Identifier |not|
//@[008:009) Assignment |=|
//@[010:014) TrueKeyword |true|
//@[014:015) NewLine |\n|
var and = true
//@[000:003) Identifier |var|
//@[004:007) Identifier |and|
//@[008:009) Assignment |=|
//@[010:014) TrueKeyword |true|
//@[014:015) NewLine |\n|
var or = true
//@[000:003) Identifier |var|
//@[004:006) Identifier |or|
//@[007:008) Assignment |=|
//@[009:013) TrueKeyword |true|
//@[013:014) NewLine |\n|
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[000:003) Identifier |var|
//@[004:017) Identifier |I_WANT_IT_ALL|
//@[018:019) Assignment |=|
//@[020:029) Identifier |variables|
//@[030:032) LogicalAnd |&&|
//@[033:043) Identifier |parameters|
//@[044:046) LogicalAnd |&&|
//@[047:049) Identifier |if|
//@[050:052) LogicalAnd |&&|
//@[053:064) Identifier |createArray|
//@[065:067) LogicalAnd |&&|
//@[068:080) Identifier |createObject|
//@[081:083) LogicalAnd |&&|
//@[084:087) Identifier |add|
//@[088:090) LogicalAnd |&&|
//@[091:094) Identifier |sub|
//@[095:097) LogicalAnd |&&|
//@[098:101) Identifier |mul|
//@[102:104) LogicalAnd |&&|
//@[105:108) Identifier |div|
//@[109:111) LogicalAnd |&&|
//@[112:115) Identifier |mod|
//@[116:118) LogicalAnd |&&|
//@[119:123) Identifier |less|
//@[124:126) LogicalAnd |&&|
//@[127:139) Identifier |lessOrEquals|
//@[140:142) LogicalAnd |&&|
//@[143:150) Identifier |greater|
//@[151:153) LogicalAnd |&&|
//@[154:169) Identifier |greaterOrEquals|
//@[170:172) LogicalAnd |&&|
//@[173:179) Identifier |equals|
//@[180:182) LogicalAnd |&&|
//@[183:186) Identifier |not|
//@[187:189) LogicalAnd |&&|
//@[190:193) Identifier |and|
//@[194:196) LogicalAnd |&&|
//@[197:199) Identifier |or|
//@[199:201) NewLine |\n\n|

// identifiers can have underscores
//@[035:036) NewLine |\n|
var _ = 3
//@[000:003) Identifier |var|
//@[004:005) Identifier |_|
//@[006:007) Assignment |=|
//@[008:009) Integer |3|
//@[009:010) NewLine |\n|
var __ = 10 * _
//@[000:003) Identifier |var|
//@[004:006) Identifier |__|
//@[007:008) Assignment |=|
//@[009:011) Integer |10|
//@[012:013) Asterisk |*|
//@[014:015) Identifier |_|
//@[015:016) NewLine |\n|
var _0a_1b = true
//@[000:003) Identifier |var|
//@[004:010) Identifier |_0a_1b|
//@[011:012) Assignment |=|
//@[013:017) TrueKeyword |true|
//@[017:018) NewLine |\n|
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[000:003) Identifier |var|
//@[004:007) Identifier |_1_|
//@[008:009) Assignment |=|
//@[010:016) Identifier |_0a_1b|
//@[017:019) LogicalOr ||||
//@[020:021) LeftParen |(|
//@[021:023) Identifier |__|
//@[024:025) Plus |+|
//@[026:027) Identifier |_|
//@[028:029) Modulo |%|
//@[030:031) Integer |2|
//@[032:034) Equals |==|
//@[035:036) Integer |0|
//@[036:037) RightParen |)|
//@[037:039) NewLine |\n\n|

// fully qualified access
//@[025:026) NewLine |\n|
var resourceGroup = 'something'
//@[000:003) Identifier |var|
//@[004:017) Identifier |resourceGroup|
//@[018:019) Assignment |=|
//@[020:031) StringComplete |'something'|
//@[031:032) NewLine |\n|
var resourceGroupName = az.resourceGroup().name
//@[000:003) Identifier |var|
//@[004:021) Identifier |resourceGroupName|
//@[022:023) Assignment |=|
//@[024:026) Identifier |az|
//@[026:027) Dot |.|
//@[027:040) Identifier |resourceGroup|
//@[040:041) LeftParen |(|
//@[041:042) RightParen |)|
//@[042:043) Dot |.|
//@[043:047) Identifier |name|
//@[047:048) NewLine |\n|
var resourceGroupObject = az.resourceGroup()
//@[000:003) Identifier |var|
//@[004:023) Identifier |resourceGroupObject|
//@[024:025) Assignment |=|
//@[026:028) Identifier |az|
//@[028:029) Dot |.|
//@[029:042) Identifier |resourceGroup|
//@[042:043) LeftParen |(|
//@[043:044) RightParen |)|
//@[044:045) NewLine |\n|
var propertyAccessFromObject = resourceGroupObject.name
//@[000:003) Identifier |var|
//@[004:028) Identifier |propertyAccessFromObject|
//@[029:030) Assignment |=|
//@[031:050) Identifier |resourceGroupObject|
//@[050:051) Dot |.|
//@[051:055) Identifier |name|
//@[055:056) NewLine |\n|
var isTrue = sys.max(1, 2) == 3
//@[000:003) Identifier |var|
//@[004:010) Identifier |isTrue|
//@[011:012) Assignment |=|
//@[013:016) Identifier |sys|
//@[016:017) Dot |.|
//@[017:020) Identifier |max|
//@[020:021) LeftParen |(|
//@[021:022) Integer |1|
//@[022:023) Comma |,|
//@[024:025) Integer |2|
//@[025:026) RightParen |)|
//@[027:029) Equals |==|
//@[030:031) Integer |3|
//@[031:032) NewLine |\n|
var isFalse = !isTrue
//@[000:003) Identifier |var|
//@[004:011) Identifier |isFalse|
//@[012:013) Assignment |=|
//@[014:015) Exclamation |!|
//@[015:021) Identifier |isTrue|
//@[021:022) NewLine |\n|
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[000:003) Identifier |var|
//@[004:012) Identifier |someText|
//@[013:014) Assignment |=|
//@[015:021) Identifier |isTrue|
//@[022:023) Question |?|
//@[024:027) Identifier |sys|
//@[027:028) Dot |.|
//@[028:034) Identifier |concat|
//@[034:035) LeftParen |(|
//@[035:038) StringComplete |'a'|
//@[038:039) Comma |,|
//@[040:043) Identifier |sys|
//@[043:044) Dot |.|
//@[044:050) Identifier |concat|
//@[050:051) LeftParen |(|
//@[051:054) StringComplete |'b'|
//@[054:055) Comma |,|
//@[056:059) StringComplete |'c'|
//@[059:060) RightParen |)|
//@[060:061) RightParen |)|
//@[062:063) Colon |:|
//@[064:074) StringComplete |'someText'|
//@[074:076) NewLine |\n\n|

// Bicep functions that cannot be converted into ARM functions
//@[062:063) NewLine |\n|
var scopesWithoutArmRepresentation = {
//@[000:003) Identifier |var|
//@[004:034) Identifier |scopesWithoutArmRepresentation|
//@[035:036) Assignment |=|
//@[037:038) LeftBrace |{|
//@[038:039) NewLine |\n|
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@[002:014) Identifier |subscription|
//@[014:015) Colon |:|
//@[016:028) Identifier |subscription|
//@[028:029) LeftParen |(|
//@[029:067) StringComplete |'10b57a01-6350-4ce2-972a-6a13642f00bf'|
//@[067:068) RightParen |)|
//@[068:069) NewLine |\n|
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@[002:015) Identifier |resourceGroup|
//@[015:016) Colon |:|
//@[017:019) Identifier |az|
//@[019:020) Dot |.|
//@[020:033) Identifier |resourceGroup|
//@[033:034) LeftParen |(|
//@[034:072) StringComplete |'10b57a01-6350-4ce2-972a-6a13642f00bf'|
//@[072:073) Comma |,|
//@[074:084) StringComplete |'myRgName'|
//@[084:085) RightParen |)|
//@[085:086) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var scopesWithArmRepresentation = {
//@[000:003) Identifier |var|
//@[004:031) Identifier |scopesWithArmRepresentation|
//@[032:033) Assignment |=|
//@[034:035) LeftBrace |{|
//@[035:036) NewLine |\n|
  tenant: tenant()
//@[002:008) Identifier |tenant|
//@[008:009) Colon |:|
//@[010:016) Identifier |tenant|
//@[016:017) LeftParen |(|
//@[017:018) RightParen |)|
//@[018:019) NewLine |\n|
  subscription: subscription()
//@[002:014) Identifier |subscription|
//@[014:015) Colon |:|
//@[016:028) Identifier |subscription|
//@[028:029) LeftParen |(|
//@[029:030) RightParen |)|
//@[030:031) NewLine |\n|
  resourceGroup: az.resourceGroup()
//@[002:015) Identifier |resourceGroup|
//@[015:016) Colon |:|
//@[017:019) Identifier |az|
//@[019:020) Dot |.|
//@[020:033) Identifier |resourceGroup|
//@[033:034) LeftParen |(|
//@[034:035) RightParen |)|
//@[035:036) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// Issue #1332
//@[014:015) NewLine |\n|
var issue1332_propname = 'ptest'
//@[000:003) Identifier |var|
//@[004:022) Identifier |issue1332_propname|
//@[023:024) Assignment |=|
//@[025:032) StringComplete |'ptest'|
//@[032:033) NewLine |\n|
var issue1332 = true ? {
//@[000:003) Identifier |var|
//@[004:013) Identifier |issue1332|
//@[014:015) Assignment |=|
//@[016:020) TrueKeyword |true|
//@[021:022) Question |?|
//@[023:024) LeftBrace |{|
//@[024:025) NewLine |\n|
    prop1: {
//@[004:009) Identifier |prop1|
//@[009:010) Colon |:|
//@[011:012) LeftBrace |{|
//@[012:013) NewLine |\n|
        '${issue1332_propname}': {}
//@[008:011) StringLeftPiece |'${|
//@[011:029) Identifier |issue1332_propname|
//@[029:031) StringRightPiece |}'|
//@[031:032) Colon |:|
//@[033:034) LeftBrace |{|
//@[034:035) RightBrace |}|
//@[035:036) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
} : {}
//@[000:001) RightBrace |}|
//@[002:003) Colon |:|
//@[004:005) LeftBrace |{|
//@[005:006) RightBrace |}|
//@[006:008) NewLine |\n\n|

// Issue #486
//@[013:014) NewLine |\n|
var myBigInt = 2199023255552
//@[000:003) Identifier |var|
//@[004:012) Identifier |myBigInt|
//@[013:014) Assignment |=|
//@[015:028) Integer |2199023255552|
//@[028:029) NewLine |\n|
var myIntExpression = 5 * 5
//@[000:003) Identifier |var|
//@[004:019) Identifier |myIntExpression|
//@[020:021) Assignment |=|
//@[022:023) Integer |5|
//@[024:025) Asterisk |*|
//@[026:027) Integer |5|
//@[027:028) NewLine |\n|
var myBigIntExpression = 2199023255552 * 2
//@[000:003) Identifier |var|
//@[004:022) Identifier |myBigIntExpression|
//@[023:024) Assignment |=|
//@[025:038) Integer |2199023255552|
//@[039:040) Asterisk |*|
//@[041:042) Integer |2|
//@[042:043) NewLine |\n|
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[000:003) Identifier |var|
//@[004:023) Identifier |myBigIntExpression2|
//@[024:025) Assignment |=|
//@[026:039) Integer |2199023255552|
//@[040:041) Asterisk |*|
//@[042:055) Integer |2199023255552|
//@[055:057) NewLine |\n\n|

// variable loops
//@[017:018) NewLine |\n|
var incrementingNumbers = [for i in range(0,10) : i]
//@[000:003) Identifier |var|
//@[004:023) Identifier |incrementingNumbers|
//@[024:025) Assignment |=|
//@[026:027) LeftSquare |[|
//@[027:030) Identifier |for|
//@[031:032) Identifier |i|
//@[033:035) Identifier |in|
//@[036:041) Identifier |range|
//@[041:042) LeftParen |(|
//@[042:043) Integer |0|
//@[043:044) Comma |,|
//@[044:046) Integer |10|
//@[046:047) RightParen |)|
//@[048:049) Colon |:|
//@[050:051) Identifier |i|
//@[051:052) RightSquare |]|
//@[052:053) NewLine |\n|
var printToSingleLine1 = [
//@[000:003) Identifier |var|
//@[004:022) Identifier |printToSingleLine1|
//@[023:024) Assignment |=|
//@[025:026) LeftSquare |[|
//@[026:027) NewLine |\n|
    for i in range(0,20) : i
//@[004:007) Identifier |for|
//@[008:009) Identifier |i|
//@[010:012) Identifier |in|
//@[013:018) Identifier |range|
//@[018:019) LeftParen |(|
//@[019:020) Integer |0|
//@[020:021) Comma |,|
//@[021:023) Integer |20|
//@[023:024) RightParen |)|
//@[025:026) Colon |:|
//@[027:028) Identifier |i|
//@[028:029) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:002) NewLine |\n|
var printToSingleLine2 = [
//@[000:003) Identifier |var|
//@[004:022) Identifier |printToSingleLine2|
//@[023:024) Assignment |=|
//@[025:026) LeftSquare |[|
//@[026:027) NewLine |\n|
    /* harmless comment */ for i in range(0,20) : i
//@[027:030) Identifier |for|
//@[031:032) Identifier |i|
//@[033:035) Identifier |in|
//@[036:041) Identifier |range|
//@[041:042) LeftParen |(|
//@[042:043) Integer |0|
//@[043:044) Comma |,|
//@[044:046) Integer |20|
//@[046:047) RightParen |)|
//@[048:049) Colon |:|
//@[050:051) Identifier |i|
//@[051:052) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:002) NewLine |\n|
var printToSingleLine3 = [
//@[000:003) Identifier |var|
//@[004:022) Identifier |printToSingleLine3|
//@[023:024) Assignment |=|
//@[025:026) LeftSquare |[|
//@[026:027) NewLine |\n|
    for i in range(0,20) : i /* harmless comment */
//@[004:007) Identifier |for|
//@[008:009) Identifier |i|
//@[010:012) Identifier |in|
//@[013:018) Identifier |range|
//@[018:019) LeftParen |(|
//@[019:020) Integer |0|
//@[020:021) Comma |,|
//@[021:023) Integer |20|
//@[023:024) RightParen |)|
//@[025:026) Colon |:|
//@[027:028) Identifier |i|
//@[051:052) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:002) NewLine |\n|
var forceLineBreaks1 = [
//@[000:003) Identifier |var|
//@[004:020) Identifier |forceLineBreaks1|
//@[021:022) Assignment |=|
//@[023:024) LeftSquare |[|
//@[024:025) NewLine |\n|
    // force line breaks
//@[024:025) NewLine |\n|
    for i in range(0,    30) : i
//@[004:007) Identifier |for|
//@[008:009) Identifier |i|
//@[010:012) Identifier |in|
//@[013:018) Identifier |range|
//@[018:019) LeftParen |(|
//@[019:020) Integer |0|
//@[020:021) Comma |,|
//@[025:027) Integer |30|
//@[027:028) RightParen |)|
//@[029:030) Colon |:|
//@[031:032) Identifier |i|
//@[032:033) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:002) NewLine |\n|
var forceLineBreaks2 = [
//@[000:003) Identifier |var|
//@[004:020) Identifier |forceLineBreaks2|
//@[021:022) Assignment |=|
//@[023:024) LeftSquare |[|
//@[024:025) NewLine |\n|
    for i in range(0,    30) : i
//@[004:007) Identifier |for|
//@[008:009) Identifier |i|
//@[010:012) Identifier |in|
//@[013:018) Identifier |range|
//@[018:019) LeftParen |(|
//@[019:020) Integer |0|
//@[020:021) Comma |,|
//@[025:027) Integer |30|
//@[027:028) RightParen |)|
//@[029:030) Colon |:|
//@[031:032) Identifier |i|
//@[032:033) NewLine |\n|
    // force line breaks
//@[024:025) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:002) NewLine |\n|
var forceLineBreaks3 = [
//@[000:003) Identifier |var|
//@[004:020) Identifier |forceLineBreaks3|
//@[021:022) Assignment |=|
//@[023:024) LeftSquare |[|
//@[024:025) NewLine |\n|
    /* force line breaks */
//@[027:028) NewLine |\n|
    for i in range(0,    30) : i
//@[004:007) Identifier |for|
//@[008:009) Identifier |i|
//@[010:012) Identifier |in|
//@[013:018) Identifier |range|
//@[018:019) LeftParen |(|
//@[019:020) Integer |0|
//@[020:021) Comma |,|
//@[025:027) Integer |30|
//@[027:028) RightParen |)|
//@[029:030) Colon |:|
//@[031:032) Identifier |i|
//@[032:033) NewLine |\n|
    /* force line breaks */
//@[027:028) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

var loopInput = [
//@[000:003) Identifier |var|
//@[004:013) Identifier |loopInput|
//@[014:015) Assignment |=|
//@[016:017) LeftSquare |[|
//@[017:018) NewLine |\n|
  'one'
//@[002:007) StringComplete |'one'|
//@[007:008) NewLine |\n|
  'two'
//@[002:007) StringComplete |'two'|
//@[007:008) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:002) NewLine |\n|
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[000:003) Identifier |var|
//@[004:025) Identifier |arrayOfStringsViaLoop|
//@[026:027) Assignment |=|
//@[028:029) LeftSquare |[|
//@[029:032) Identifier |for|
//@[033:034) LeftParen |(|
//@[034:038) Identifier |name|
//@[038:039) Comma |,|
//@[040:041) Identifier |i|
//@[041:042) RightParen |)|
//@[043:045) Identifier |in|
//@[046:055) Identifier |loopInput|
//@[055:056) Colon |:|
//@[057:067) StringLeftPiece |'prefix-${|
//@[067:068) Identifier |i|
//@[068:072) StringMiddlePiece |}-${|
//@[072:076) Identifier |name|
//@[076:078) StringRightPiece |}'|
//@[078:079) RightSquare |]|
//@[079:080) NewLine |\n|
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[000:003) Identifier |var|
//@[004:025) Identifier |arrayOfObjectsViaLoop|
//@[026:027) Assignment |=|
//@[028:029) LeftSquare |[|
//@[029:032) Identifier |for|
//@[033:034) LeftParen |(|
//@[034:038) Identifier |name|
//@[038:039) Comma |,|
//@[040:041) Identifier |i|
//@[041:042) RightParen |)|
//@[043:045) Identifier |in|
//@[046:055) Identifier |loopInput|
//@[055:056) Colon |:|
//@[057:058) LeftBrace |{|
//@[058:059) NewLine |\n|
  index: i
//@[002:007) Identifier |index|
//@[007:008) Colon |:|
//@[009:010) Identifier |i|
//@[010:011) NewLine |\n|
  name: name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) Identifier |name|
//@[012:013) NewLine |\n|
  value: 'prefix-${i}-${name}-suffix'
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:019) StringLeftPiece |'prefix-${|
//@[019:020) Identifier |i|
//@[020:024) StringMiddlePiece |}-${|
//@[024:028) Identifier |name|
//@[028:037) StringRightPiece |}-suffix'|
//@[037:038) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[000:003) Identifier |var|
//@[004:024) Identifier |arrayOfArraysViaLoop|
//@[025:026) Assignment |=|
//@[027:028) LeftSquare |[|
//@[028:031) Identifier |for|
//@[032:033) LeftParen |(|
//@[033:037) Identifier |name|
//@[037:038) Comma |,|
//@[039:040) Identifier |i|
//@[040:041) RightParen |)|
//@[042:044) Identifier |in|
//@[045:054) Identifier |loopInput|
//@[054:055) Colon |:|
//@[056:057) LeftSquare |[|
//@[057:058) NewLine |\n|
  i
//@[002:003) Identifier |i|
//@[003:004) NewLine |\n|
  name
//@[002:006) Identifier |name|
//@[006:007) NewLine |\n|
  'prefix-${i}-${name}-suffix'
//@[002:012) StringLeftPiece |'prefix-${|
//@[012:013) Identifier |i|
//@[013:017) StringMiddlePiece |}-${|
//@[017:021) Identifier |name|
//@[021:030) StringRightPiece |}-suffix'|
//@[030:031) NewLine |\n|
]]
//@[000:001) RightSquare |]|
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[000:003) Identifier |var|
//@[004:019) Identifier |arrayOfBooleans|
//@[020:021) Assignment |=|
//@[022:023) LeftSquare |[|
//@[023:026) Identifier |for|
//@[027:028) LeftParen |(|
//@[028:032) Identifier |name|
//@[032:033) Comma |,|
//@[034:035) Identifier |i|
//@[035:036) RightParen |)|
//@[037:039) Identifier |in|
//@[040:049) Identifier |loopInput|
//@[049:050) Colon |:|
//@[051:052) Identifier |i|
//@[053:054) Modulo |%|
//@[055:056) Integer |2|
//@[057:059) Equals |==|
//@[060:061) Integer |0|
//@[061:062) RightSquare |]|
//@[062:063) NewLine |\n|
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[000:003) Identifier |var|
//@[004:027) Identifier |arrayOfHardCodedNumbers|
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
//@[053:054) Integer |3|
//@[054:055) RightSquare |]|
//@[055:056) NewLine |\n|
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[000:003) Identifier |var|
//@[004:025) Identifier |arrayOfHardCodedBools|
//@[026:027) Assignment |=|
//@[028:029) LeftSquare |[|
//@[029:032) Identifier |for|
//@[033:034) Identifier |i|
//@[035:037) Identifier |in|
//@[038:043) Identifier |range|
//@[043:044) LeftParen |(|
//@[044:045) Integer |0|
//@[045:046) Comma |,|
//@[046:048) Integer |10|
//@[048:049) RightParen |)|
//@[049:050) Colon |:|
//@[051:056) FalseKeyword |false|
//@[056:057) RightSquare |]|
//@[057:058) NewLine |\n|
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[000:003) Identifier |var|
//@[004:027) Identifier |arrayOfHardCodedStrings|
//@[028:029) Assignment |=|
//@[030:031) LeftSquare |[|
//@[031:034) Identifier |for|
//@[035:036) Identifier |i|
//@[037:039) Identifier |in|
//@[040:045) Identifier |range|
//@[045:046) LeftParen |(|
//@[046:047) Integer |0|
//@[047:048) Comma |,|
//@[048:049) Integer |3|
//@[049:050) RightParen |)|
//@[050:051) Colon |:|
//@[052:056) StringComplete |'hi'|
//@[056:057) RightSquare |]|
//@[057:058) NewLine |\n|
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[000:003) Identifier |var|
//@[004:034) Identifier |arrayOfNonRuntimeFunctionCalls|
//@[035:036) Assignment |=|
//@[037:038) LeftSquare |[|
//@[038:041) Identifier |for|
//@[042:043) Identifier |i|
//@[044:046) Identifier |in|
//@[047:052) Identifier |range|
//@[052:053) LeftParen |(|
//@[053:054) Integer |0|
//@[054:055) Comma |,|
//@[055:056) Integer |3|
//@[056:057) RightParen |)|
//@[057:058) Colon |:|
//@[059:065) Identifier |concat|
//@[065:066) LeftParen |(|
//@[066:070) StringComplete |'hi'|
//@[070:071) Comma |,|
//@[072:073) Identifier |i|
//@[073:074) RightParen |)|
//@[074:075) RightSquare |]|
//@[075:077) NewLine |\n\n|

var multilineString = '''
//@[000:003) Identifier |var|
//@[004:019) Identifier |multilineString|
//@[020:021) Assignment |=|
//@[022:036) StringComplete |'''\nHELLO!\n'''|
HELLO!
'''
//@[003:005) NewLine |\n\n|

var multilineEmpty = ''''''
//@[000:003) Identifier |var|
//@[004:018) Identifier |multilineEmpty|
//@[019:020) Assignment |=|
//@[021:027) StringComplete |''''''|
//@[027:028) NewLine |\n|
var multilineEmptyNewline = '''
//@[000:003) Identifier |var|
//@[004:025) Identifier |multilineEmptyNewline|
//@[026:027) Assignment |=|
//@[028:035) StringComplete |'''\n'''|
'''
//@[003:005) NewLine |\n\n|

// evaluates to '\'abc\''
//@[025:026) NewLine |\n|
var multilineExtraQuotes = ''''abc''''
//@[000:003) Identifier |var|
//@[004:024) Identifier |multilineExtraQuotes|
//@[025:026) Assignment |=|
//@[027:038) StringComplete |''''abc''''|
//@[038:040) NewLine |\n\n|

// evaluates to '\'\nabc\n\''
//@[029:030) NewLine |\n|
var multilineExtraQuotesNewlines = ''''
//@[000:003) Identifier |var|
//@[004:032) Identifier |multilineExtraQuotesNewlines|
//@[033:034) Assignment |=|
//@[035:048) StringComplete |''''\nabc\n''''|
abc
''''
//@[004:006) NewLine |\n\n|

var multilineSingleLine = '''hello!'''
//@[000:003) Identifier |var|
//@[004:023) Identifier |multilineSingleLine|
//@[024:025) Assignment |=|
//@[026:038) StringComplete |'''hello!'''|
//@[038:040) NewLine |\n\n|

var multilineFormatted = format('''
//@[000:003) Identifier |var|
//@[004:022) Identifier |multilineFormatted|
//@[023:024) Assignment |=|
//@[025:031) Identifier |format|
//@[031:032) LeftParen |(|
//@[032:061) StringComplete |'''\nHello,\nmy\nname is\n{0}\n'''|
Hello,
my
name is
{0}
''', 'Anthony')
//@[003:004) Comma |,|
//@[005:014) StringComplete |'Anthony'|
//@[014:015) RightParen |)|
//@[015:017) NewLine |\n\n|

var multilineJavaScript = '''
//@[000:003) Identifier |var|
//@[004:023) Identifier |multilineJavaScript|
//@[024:025) Assignment |=|
//@[026:586) StringComplete |'''\n// NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n'''|
// NOT RECOMMENDED PATTERN
const fs = require('fs');

module.exports = function (context) {
    fs.readFile('./hello.txt', (err, data) => {
        if (err) {
            context.log.error('ERROR', err);
            // BUG #1: This will result in an uncaught exception that crashes the entire process
            throw err;
        }
        context.log(`Data from file: ${data}`);
        // context.done() should be called here
    });
    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends
    context.done();
}
'''
//@[003:005) NewLine |\n\n|

var providersTest = providers('Microsoft.Resources').namespace
//@[000:003) Identifier |var|
//@[004:017) Identifier |providersTest|
//@[018:019) Assignment |=|
//@[020:029) Identifier |providers|
//@[029:030) LeftParen |(|
//@[030:051) StringComplete |'Microsoft.Resources'|
//@[051:052) RightParen |)|
//@[052:053) Dot |.|
//@[053:062) Identifier |namespace|
//@[062:063) NewLine |\n|
var providersTest2 = providers('Microsoft.Resources', 'deployments').locations
//@[000:003) Identifier |var|
//@[004:018) Identifier |providersTest2|
//@[019:020) Assignment |=|
//@[021:030) Identifier |providers|
//@[030:031) LeftParen |(|
//@[031:052) StringComplete |'Microsoft.Resources'|
//@[052:053) Comma |,|
//@[054:067) StringComplete |'deployments'|
//@[067:068) RightParen |)|
//@[068:069) Dot |.|
//@[069:078) Identifier |locations|
//@[078:080) NewLine |\n\n|

var copyBlockInObject = {
//@[000:003) Identifier |var|
//@[004:021) Identifier |copyBlockInObject|
//@[022:023) Assignment |=|
//@[024:025) LeftBrace |{|
//@[025:026) NewLine |\n|
  copy: [
//@[002:006) Identifier |copy|
//@[006:007) Colon |:|
//@[008:009) LeftSquare |[|
//@[009:010) NewLine |\n|
    {
//@[004:005) LeftBrace |{|
//@[005:006) NewLine |\n|
      name: 'blah'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:018) StringComplete |'blah'|
//@[018:019) NewLine |\n|
      count: '[notAFunction()]'
//@[006:011) Identifier |count|
//@[011:012) Colon |:|
//@[013:031) StringComplete |'[notAFunction()]'|
//@[031:032) NewLine |\n|
      input: {}
//@[006:011) Identifier |input|
//@[011:012) Colon |:|
//@[013:014) LeftBrace |{|
//@[014:015) RightBrace |}|
//@[015:016) NewLine |\n|
    }
//@[004:005) RightBrace |}|
//@[005:006) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var joinedString = join(['I', 'love', 'Bicep!'], ' ')
//@[000:003) Identifier |var|
//@[004:016) Identifier |joinedString|
//@[017:018) Assignment |=|
//@[019:023) Identifier |join|
//@[023:024) LeftParen |(|
//@[024:025) LeftSquare |[|
//@[025:028) StringComplete |'I'|
//@[028:029) Comma |,|
//@[030:036) StringComplete |'love'|
//@[036:037) Comma |,|
//@[038:046) StringComplete |'Bicep!'|
//@[046:047) RightSquare |]|
//@[047:048) Comma |,|
//@[049:052) StringComplete |' '|
//@[052:053) RightParen |)|
//@[053:055) NewLine |\n\n|

var prefix = take('food', 3)
//@[000:003) Identifier |var|
//@[004:010) Identifier |prefix|
//@[011:012) Assignment |=|
//@[013:017) Identifier |take|
//@[017:018) LeftParen |(|
//@[018:024) StringComplete |'food'|
//@[024:025) Comma |,|
//@[026:027) Integer |3|
//@[027:028) RightParen |)|
//@[028:029) NewLine |\n|
var isPrefixed = startsWith('food', 'foo')
//@[000:003) Identifier |var|
//@[004:014) Identifier |isPrefixed|
//@[015:016) Assignment |=|
//@[017:027) Identifier |startsWith|
//@[027:028) LeftParen |(|
//@[028:034) StringComplete |'food'|
//@[034:035) Comma |,|
//@[036:041) StringComplete |'foo'|
//@[041:042) RightParen |)|
//@[042:044) NewLine |\n\n|

var spread = {
//@[000:003) Identifier |var|
//@[004:010) Identifier |spread|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  foo: 'abc'
//@[002:005) Identifier |foo|
//@[005:006) Colon |:|
//@[007:012) StringComplete |'abc'|
//@[012:013) NewLine |\n|
  ...issue1332
//@[002:005) Ellipsis |...|
//@[005:014) Identifier |issue1332|
//@[014:015) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var test = {
//@[000:003) Identifier |var|
//@[004:008) Identifier |test|
//@[009:010) Assignment |=|
//@[011:012) LeftBrace |{|
//@[012:013) NewLine |\n|
  ...spread
//@[002:005) Ellipsis |...|
//@[005:011) Identifier |spread|
//@[011:012) NewLine |\n|
  bar: 'def'
//@[002:005) Identifier |bar|
//@[005:006) Colon |:|
//@[007:012) StringComplete |'def'|
//@[012:013) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var arraySpread = [...arrayOfBooleans, ...arrayOfHardCodedNumbers, ...arrayOfHardCodedStrings]
//@[000:003) Identifier |var|
//@[004:015) Identifier |arraySpread|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:022) Ellipsis |...|
//@[022:037) Identifier |arrayOfBooleans|
//@[037:038) Comma |,|
//@[039:042) Ellipsis |...|
//@[042:065) Identifier |arrayOfHardCodedNumbers|
//@[065:066) Comma |,|
//@[067:070) Ellipsis |...|
//@[070:093) Identifier |arrayOfHardCodedStrings|
//@[093:094) RightSquare |]|
//@[094:097) NewLine |\n\n\n|


var nameof1 = nameof(arraySpread)
//@[000:003) Identifier |var|
//@[004:011) Identifier |nameof1|
//@[012:013) Assignment |=|
//@[014:020) Identifier |nameof|
//@[020:021) LeftParen |(|
//@[021:032) Identifier |arraySpread|
//@[032:033) RightParen |)|
//@[033:034) NewLine |\n|
var nameof2 = nameof(spread.foo)
//@[000:003) Identifier |var|
//@[004:011) Identifier |nameof2|
//@[012:013) Assignment |=|
//@[014:020) Identifier |nameof|
//@[020:021) LeftParen |(|
//@[021:027) Identifier |spread|
//@[027:028) Dot |.|
//@[028:031) Identifier |foo|
//@[031:032) RightParen |)|
//@[032:033) NewLine |\n|
var nameof3 = nameof(myObj.obj.nested)
//@[000:003) Identifier |var|
//@[004:011) Identifier |nameof3|
//@[012:013) Assignment |=|
//@[014:020) Identifier |nameof|
//@[020:021) LeftParen |(|
//@[021:026) Identifier |myObj|
//@[026:027) Dot |.|
//@[027:030) Identifier |obj|
//@[030:031) Dot |.|
//@[031:037) Identifier |nested|
//@[037:038) RightParen |)|
//@[038:039) NewLine |\n|

//@[000:000) EndOfFile ||
