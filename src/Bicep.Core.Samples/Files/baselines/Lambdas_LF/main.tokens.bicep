var doggos = [
//@[000:003) Identifier |var|
//@[004:010) Identifier |doggos|
//@[011:012) Assignment |=|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
  'Evie'
//@[002:008) StringComplete |'Evie'|
//@[008:009) NewLine |\n|
  'Casper'
//@[002:010) StringComplete |'Casper'|
//@[010:011) NewLine |\n|
  'Indy'
//@[002:008) StringComplete |'Indy'|
//@[008:009) NewLine |\n|
  'Kira'
//@[002:008) StringComplete |'Kira'|
//@[008:009) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

func isEven(i int) bool => i % 2 == 0
//@[000:004) Identifier |func|
//@[005:011) Identifier |isEven|
//@[011:012) LeftParen |(|
//@[012:013) Identifier |i|
//@[014:017) Identifier |int|
//@[017:018) RightParen |)|
//@[019:023) Identifier |bool|
//@[024:026) Arrow |=>|
//@[027:028) Identifier |i|
//@[029:030) Modulo |%|
//@[031:032) Integer |2|
//@[033:035) Equals |==|
//@[036:037) Integer |0|
//@[037:039) NewLine |\n\n|

var numbers = range(0, 4)
//@[000:003) Identifier |var|
//@[004:011) Identifier |numbers|
//@[012:013) Assignment |=|
//@[014:019) Identifier |range|
//@[019:020) LeftParen |(|
//@[020:021) Integer |0|
//@[021:022) Comma |,|
//@[023:024) Integer |4|
//@[024:025) RightParen |)|
//@[025:027) NewLine |\n\n|

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[000:003) Identifier |var|
//@[004:012) Identifier |sayHello|
//@[013:014) Assignment |=|
//@[015:018) Identifier |map|
//@[018:019) LeftParen |(|
//@[019:025) Identifier |doggos|
//@[025:026) Comma |,|
//@[027:028) Identifier |i|
//@[029:031) Arrow |=>|
//@[032:041) StringLeftPiece |'Hello ${|
//@[041:042) Identifier |i|
//@[042:045) StringRightPiece |}!'|
//@[045:046) RightParen |)|
//@[046:047) NewLine |\n|
// optional index parameter for map lambda
//@[042:043) NewLine |\n|
var sayHello2 = map(doggos, (dog, i) => '${isEven(i) ? 'Hi' : 'Ahoy'} ${dog}!')
//@[000:003) Identifier |var|
//@[004:013) Identifier |sayHello2|
//@[014:015) Assignment |=|
//@[016:019) Identifier |map|
//@[019:020) LeftParen |(|
//@[020:026) Identifier |doggos|
//@[026:027) Comma |,|
//@[028:029) LeftParen |(|
//@[029:032) Identifier |dog|
//@[032:033) Comma |,|
//@[034:035) Identifier |i|
//@[035:036) RightParen |)|
//@[037:039) Arrow |=>|
//@[040:043) StringLeftPiece |'${|
//@[043:049) Identifier |isEven|
//@[049:050) LeftParen |(|
//@[050:051) Identifier |i|
//@[051:052) RightParen |)|
//@[053:054) Question |?|
//@[055:059) StringComplete |'Hi'|
//@[060:061) Colon |:|
//@[062:068) StringComplete |'Ahoy'|
//@[068:072) StringMiddlePiece |} ${|
//@[072:075) Identifier |dog|
//@[075:078) StringRightPiece |}!'|
//@[078:079) RightParen |)|
//@[079:081) NewLine |\n\n|

var evenNumbers = filter(numbers, i => isEven(i))
//@[000:003) Identifier |var|
//@[004:015) Identifier |evenNumbers|
//@[016:017) Assignment |=|
//@[018:024) Identifier |filter|
//@[024:025) LeftParen |(|
//@[025:032) Identifier |numbers|
//@[032:033) Comma |,|
//@[034:035) Identifier |i|
//@[036:038) Arrow |=>|
//@[039:045) Identifier |isEven|
//@[045:046) LeftParen |(|
//@[046:047) Identifier |i|
//@[047:048) RightParen |)|
//@[048:049) RightParen |)|
//@[049:050) NewLine |\n|
// optional index parameter for filter lambda
//@[045:046) NewLine |\n|
var evenEntries = filter(['a', 'b', 'c', 'd'], (item, i) => isEven(i))
//@[000:003) Identifier |var|
//@[004:015) Identifier |evenEntries|
//@[016:017) Assignment |=|
//@[018:024) Identifier |filter|
//@[024:025) LeftParen |(|
//@[025:026) LeftSquare |[|
//@[026:029) StringComplete |'a'|
//@[029:030) Comma |,|
//@[031:034) StringComplete |'b'|
//@[034:035) Comma |,|
//@[036:039) StringComplete |'c'|
//@[039:040) Comma |,|
//@[041:044) StringComplete |'d'|
//@[044:045) RightSquare |]|
//@[045:046) Comma |,|
//@[047:048) LeftParen |(|
//@[048:052) Identifier |item|
//@[052:053) Comma |,|
//@[054:055) Identifier |i|
//@[055:056) RightParen |)|
//@[057:059) Arrow |=>|
//@[060:066) Identifier |isEven|
//@[066:067) LeftParen |(|
//@[067:068) Identifier |i|
//@[068:069) RightParen |)|
//@[069:070) RightParen |)|
//@[070:072) NewLine |\n\n|

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[000:003) Identifier |var|
//@[004:027) Identifier |evenDoggosNestedLambdas|
//@[028:029) Assignment |=|
//@[030:033) Identifier |map|
//@[033:034) LeftParen |(|
//@[034:040) Identifier |filter|
//@[040:041) LeftParen |(|
//@[041:048) Identifier |numbers|
//@[048:049) Comma |,|
//@[050:051) Identifier |i|
//@[052:054) Arrow |=>|
//@[055:063) Identifier |contains|
//@[063:064) LeftParen |(|
//@[064:070) Identifier |filter|
//@[070:071) LeftParen |(|
//@[071:078) Identifier |numbers|
//@[078:079) Comma |,|
//@[080:081) Identifier |j|
//@[082:084) Arrow |=>|
//@[085:086) Integer |0|
//@[087:089) Equals |==|
//@[090:091) Identifier |j|
//@[092:093) Modulo |%|
//@[094:095) Integer |2|
//@[095:096) RightParen |)|
//@[096:097) Comma |,|
//@[098:099) Identifier |i|
//@[099:100) RightParen |)|
//@[100:101) RightParen |)|
//@[101:102) Comma |,|
//@[103:104) Identifier |x|
//@[105:107) Arrow |=>|
//@[108:114) Identifier |doggos|
//@[114:115) LeftSquare |[|
//@[115:116) Identifier |x|
//@[116:117) RightSquare |]|
//@[117:118) RightParen |)|
//@[118:120) NewLine |\n\n|

var flattenedArrayOfArrays = flatten([[0, 1], [2, 3], [4, 5]])
//@[000:003) Identifier |var|
//@[004:026) Identifier |flattenedArrayOfArrays|
//@[027:028) Assignment |=|
//@[029:036) Identifier |flatten|
//@[036:037) LeftParen |(|
//@[037:038) LeftSquare |[|
//@[038:039) LeftSquare |[|
//@[039:040) Integer |0|
//@[040:041) Comma |,|
//@[042:043) Integer |1|
//@[043:044) RightSquare |]|
//@[044:045) Comma |,|
//@[046:047) LeftSquare |[|
//@[047:048) Integer |2|
//@[048:049) Comma |,|
//@[050:051) Integer |3|
//@[051:052) RightSquare |]|
//@[052:053) Comma |,|
//@[054:055) LeftSquare |[|
//@[055:056) Integer |4|
//@[056:057) Comma |,|
//@[058:059) Integer |5|
//@[059:060) RightSquare |]|
//@[060:061) RightSquare |]|
//@[061:062) RightParen |)|
//@[062:063) NewLine |\n|
var flattenedEmptyArray = flatten([])
//@[000:003) Identifier |var|
//@[004:023) Identifier |flattenedEmptyArray|
//@[024:025) Assignment |=|
//@[026:033) Identifier |flatten|
//@[033:034) LeftParen |(|
//@[034:035) LeftSquare |[|
//@[035:036) RightSquare |]|
//@[036:037) RightParen |)|
//@[037:039) NewLine |\n\n|

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[000:003) Identifier |var|
//@[004:012) Identifier |mapSayHi|
//@[013:014) Assignment |=|
//@[015:018) Identifier |map|
//@[018:019) LeftParen |(|
//@[019:020) LeftSquare |[|
//@[020:025) StringComplete |'abc'|
//@[025:026) Comma |,|
//@[027:032) StringComplete |'def'|
//@[032:033) Comma |,|
//@[034:039) StringComplete |'ghi'|
//@[039:040) RightSquare |]|
//@[040:041) Comma |,|
//@[042:045) Identifier |foo|
//@[046:048) Arrow |=>|
//@[049:055) StringLeftPiece |'Hi ${|
//@[055:058) Identifier |foo|
//@[058:061) StringRightPiece |}!'|
//@[061:062) RightParen |)|
//@[062:063) NewLine |\n|
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[000:003) Identifier |var|
//@[004:012) Identifier |mapEmpty|
//@[013:014) Assignment |=|
//@[015:018) Identifier |map|
//@[018:019) LeftParen |(|
//@[019:020) LeftSquare |[|
//@[020:021) RightSquare |]|
//@[021:022) Comma |,|
//@[023:026) Identifier |foo|
//@[027:029) Arrow |=>|
//@[030:036) StringLeftPiece |'Hi ${|
//@[036:039) Identifier |foo|
//@[039:042) StringRightPiece |}!'|
//@[042:043) RightParen |)|
//@[043:044) NewLine |\n|
var mapObject = map(range(0, length(doggos)), i => {
//@[000:003) Identifier |var|
//@[004:013) Identifier |mapObject|
//@[014:015) Assignment |=|
//@[016:019) Identifier |map|
//@[019:020) LeftParen |(|
//@[020:025) Identifier |range|
//@[025:026) LeftParen |(|
//@[026:027) Integer |0|
//@[027:028) Comma |,|
//@[029:035) Identifier |length|
//@[035:036) LeftParen |(|
//@[036:042) Identifier |doggos|
//@[042:043) RightParen |)|
//@[043:044) RightParen |)|
//@[044:045) Comma |,|
//@[046:047) Identifier |i|
//@[048:050) Arrow |=>|
//@[051:052) LeftBrace |{|
//@[052:053) NewLine |\n|
  i: i
//@[002:003) Identifier |i|
//@[003:004) Colon |:|
//@[005:006) Identifier |i|
//@[006:007) NewLine |\n|
  doggo: doggos[i]
//@[002:007) Identifier |doggo|
//@[007:008) Colon |:|
//@[009:015) Identifier |doggos|
//@[015:016) LeftSquare |[|
//@[016:017) Identifier |i|
//@[017:018) RightSquare |]|
//@[018:019) NewLine |\n|
  greeting: 'Ahoy, ${doggos[i]}!'
//@[002:010) Identifier |greeting|
//@[010:011) Colon |:|
//@[012:021) StringLeftPiece |'Ahoy, ${|
//@[021:027) Identifier |doggos|
//@[027:028) LeftSquare |[|
//@[028:029) Identifier |i|
//@[029:030) RightSquare |]|
//@[030:033) StringRightPiece |}!'|
//@[033:034) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@[000:003) Identifier |var|
//@[004:012) Identifier |mapArray|
//@[013:014) Assignment |=|
//@[015:022) Identifier |flatten|
//@[022:023) LeftParen |(|
//@[023:026) Identifier |map|
//@[026:027) LeftParen |(|
//@[027:032) Identifier |range|
//@[032:033) LeftParen |(|
//@[033:034) Integer |1|
//@[034:035) Comma |,|
//@[036:037) Integer |3|
//@[037:038) RightParen |)|
//@[038:039) Comma |,|
//@[040:041) Identifier |i|
//@[042:044) Arrow |=>|
//@[045:046) LeftSquare |[|
//@[046:047) Identifier |i|
//@[048:049) Asterisk |*|
//@[050:051) Integer |2|
//@[051:052) Comma |,|
//@[053:054) LeftParen |(|
//@[054:055) Identifier |i|
//@[056:057) Asterisk |*|
//@[058:059) Integer |2|
//@[059:060) RightParen |)|
//@[061:062) Plus |+|
//@[063:064) Integer |1|
//@[064:065) RightSquare |]|
//@[065:066) RightParen |)|
//@[066:067) RightParen |)|
//@[067:068) NewLine |\n|
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[000:003) Identifier |var|
//@[004:021) Identifier |mapMultiLineArray|
//@[022:023) Assignment |=|
//@[024:031) Identifier |flatten|
//@[031:032) LeftParen |(|
//@[032:035) Identifier |map|
//@[035:036) LeftParen |(|
//@[036:041) Identifier |range|
//@[041:042) LeftParen |(|
//@[042:043) Integer |1|
//@[043:044) Comma |,|
//@[045:046) Integer |3|
//@[046:047) RightParen |)|
//@[047:048) Comma |,|
//@[049:050) Identifier |i|
//@[051:053) Arrow |=>|
//@[054:055) LeftSquare |[|
//@[055:056) NewLine |\n|
  i * 3
//@[002:003) Identifier |i|
//@[004:005) Asterisk |*|
//@[006:007) Integer |3|
//@[007:008) NewLine |\n|
  (i * 3) + 1
//@[002:003) LeftParen |(|
//@[003:004) Identifier |i|
//@[005:006) Asterisk |*|
//@[007:008) Integer |3|
//@[008:009) RightParen |)|
//@[010:011) Plus |+|
//@[012:013) Integer |1|
//@[013:014) NewLine |\n|
  (i * 3) + 2
//@[002:003) LeftParen |(|
//@[003:004) Identifier |i|
//@[005:006) Asterisk |*|
//@[007:008) Integer |3|
//@[008:009) RightParen |)|
//@[010:011) Plus |+|
//@[012:013) Integer |2|
//@[013:014) NewLine |\n|
]))
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) RightParen |)|
//@[003:005) NewLine |\n\n|

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[000:003) Identifier |var|
//@[004:023) Identifier |filterEqualityCheck|
//@[024:025) Assignment |=|
//@[026:032) Identifier |filter|
//@[032:033) LeftParen |(|
//@[033:034) LeftSquare |[|
//@[034:039) StringComplete |'abc'|
//@[039:040) Comma |,|
//@[041:046) StringComplete |'def'|
//@[046:047) Comma |,|
//@[048:053) StringComplete |'ghi'|
//@[053:054) RightSquare |]|
//@[054:055) Comma |,|
//@[056:059) Identifier |foo|
//@[060:062) Arrow |=>|
//@[063:068) StringComplete |'def'|
//@[069:071) Equals |==|
//@[072:075) Identifier |foo|
//@[075:076) RightParen |)|
//@[076:077) NewLine |\n|
var filterEmpty = filter([], foo => 'def' == foo)
//@[000:003) Identifier |var|
//@[004:015) Identifier |filterEmpty|
//@[016:017) Assignment |=|
//@[018:024) Identifier |filter|
//@[024:025) LeftParen |(|
//@[025:026) LeftSquare |[|
//@[026:027) RightSquare |]|
//@[027:028) Comma |,|
//@[029:032) Identifier |foo|
//@[033:035) Arrow |=>|
//@[036:041) StringComplete |'def'|
//@[042:044) Equals |==|
//@[045:048) Identifier |foo|
//@[048:049) RightParen |)|
//@[049:051) NewLine |\n\n|

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[000:003) Identifier |var|
//@[004:015) Identifier |sortNumeric|
//@[016:017) Assignment |=|
//@[018:022) Identifier |sort|
//@[022:023) LeftParen |(|
//@[023:024) LeftSquare |[|
//@[024:025) Integer |8|
//@[025:026) Comma |,|
//@[027:028) Integer |3|
//@[028:029) Comma |,|
//@[030:032) Integer |10|
//@[032:033) Comma |,|
//@[034:035) Minus |-|
//@[035:037) Integer |13|
//@[037:038) Comma |,|
//@[039:040) Integer |5|
//@[040:041) RightSquare |]|
//@[041:042) Comma |,|
//@[043:044) LeftParen |(|
//@[044:045) Identifier |x|
//@[045:046) Comma |,|
//@[047:048) Identifier |y|
//@[048:049) RightParen |)|
//@[050:052) Arrow |=>|
//@[053:054) Identifier |x|
//@[055:056) LeftChevron |<|
//@[057:058) Identifier |y|
//@[058:059) RightParen |)|
//@[059:060) NewLine |\n|
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[000:003) Identifier |var|
//@[004:013) Identifier |sortAlpha|
//@[014:015) Assignment |=|
//@[016:020) Identifier |sort|
//@[020:021) LeftParen |(|
//@[021:022) LeftSquare |[|
//@[022:027) StringComplete |'ghi'|
//@[027:028) Comma |,|
//@[029:034) StringComplete |'abc'|
//@[034:035) Comma |,|
//@[036:041) StringComplete |'def'|
//@[041:042) RightSquare |]|
//@[042:043) Comma |,|
//@[044:045) LeftParen |(|
//@[045:046) Identifier |x|
//@[046:047) Comma |,|
//@[048:049) Identifier |y|
//@[049:050) RightParen |)|
//@[051:053) Arrow |=>|
//@[054:055) Identifier |x|
//@[056:057) LeftChevron |<|
//@[058:059) Identifier |y|
//@[059:060) RightParen |)|
//@[060:061) NewLine |\n|
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[000:003) Identifier |var|
//@[004:020) Identifier |sortAlphaReverse|
//@[021:022) Assignment |=|
//@[023:027) Identifier |sort|
//@[027:028) LeftParen |(|
//@[028:029) LeftSquare |[|
//@[029:034) StringComplete |'ghi'|
//@[034:035) Comma |,|
//@[036:041) StringComplete |'abc'|
//@[041:042) Comma |,|
//@[043:048) StringComplete |'def'|
//@[048:049) RightSquare |]|
//@[049:050) Comma |,|
//@[051:052) LeftParen |(|
//@[052:053) Identifier |x|
//@[053:054) Comma |,|
//@[055:056) Identifier |y|
//@[056:057) RightParen |)|
//@[058:060) Arrow |=>|
//@[061:062) Identifier |x|
//@[063:064) RightChevron |>|
//@[065:066) Identifier |y|
//@[066:067) RightParen |)|
//@[067:068) NewLine |\n|
var sortByObjectKey = sort([
//@[000:003) Identifier |var|
//@[004:019) Identifier |sortByObjectKey|
//@[020:021) Assignment |=|
//@[022:026) Identifier |sort|
//@[026:027) LeftParen |(|
//@[027:028) LeftSquare |[|
//@[028:029) NewLine |\n|
  { key: 124, name: 'Second' }
//@[002:003) LeftBrace |{|
//@[004:007) Identifier |key|
//@[007:008) Colon |:|
//@[009:012) Integer |124|
//@[012:013) Comma |,|
//@[014:018) Identifier |name|
//@[018:019) Colon |:|
//@[020:028) StringComplete |'Second'|
//@[029:030) RightBrace |}|
//@[030:031) NewLine |\n|
  { key: 298, name: 'Third' }
//@[002:003) LeftBrace |{|
//@[004:007) Identifier |key|
//@[007:008) Colon |:|
//@[009:012) Integer |298|
//@[012:013) Comma |,|
//@[014:018) Identifier |name|
//@[018:019) Colon |:|
//@[020:027) StringComplete |'Third'|
//@[028:029) RightBrace |}|
//@[029:030) NewLine |\n|
  { key: 24, name: 'First' }
//@[002:003) LeftBrace |{|
//@[004:007) Identifier |key|
//@[007:008) Colon |:|
//@[009:011) Integer |24|
//@[011:012) Comma |,|
//@[013:017) Identifier |name|
//@[017:018) Colon |:|
//@[019:026) StringComplete |'First'|
//@[027:028) RightBrace |}|
//@[028:029) NewLine |\n|
  { key: 1232, name: 'Fourth' }
//@[002:003) LeftBrace |{|
//@[004:007) Identifier |key|
//@[007:008) Colon |:|
//@[009:013) Integer |1232|
//@[013:014) Comma |,|
//@[015:019) Identifier |name|
//@[019:020) Colon |:|
//@[021:029) StringComplete |'Fourth'|
//@[030:031) RightBrace |}|
//@[031:032) NewLine |\n|
], (x, y) => int(x.key) < int(y.key))
//@[000:001) RightSquare |]|
//@[001:002) Comma |,|
//@[003:004) LeftParen |(|
//@[004:005) Identifier |x|
//@[005:006) Comma |,|
//@[007:008) Identifier |y|
//@[008:009) RightParen |)|
//@[010:012) Arrow |=>|
//@[013:016) Identifier |int|
//@[016:017) LeftParen |(|
//@[017:018) Identifier |x|
//@[018:019) Dot |.|
//@[019:022) Identifier |key|
//@[022:023) RightParen |)|
//@[024:025) LeftChevron |<|
//@[026:029) Identifier |int|
//@[029:030) LeftParen |(|
//@[030:031) Identifier |y|
//@[031:032) Dot |.|
//@[032:035) Identifier |key|
//@[035:036) RightParen |)|
//@[036:037) RightParen |)|
//@[037:038) NewLine |\n|
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[000:003) Identifier |var|
//@[004:013) Identifier |sortEmpty|
//@[014:015) Assignment |=|
//@[016:020) Identifier |sort|
//@[020:021) LeftParen |(|
//@[021:022) LeftSquare |[|
//@[022:023) RightSquare |]|
//@[023:024) Comma |,|
//@[025:026) LeftParen |(|
//@[026:027) Identifier |x|
//@[027:028) Comma |,|
//@[029:030) Identifier |y|
//@[030:031) RightParen |)|
//@[032:034) Arrow |=>|
//@[035:038) Identifier |int|
//@[038:039) LeftParen |(|
//@[039:040) Identifier |x|
//@[040:041) RightParen |)|
//@[042:043) LeftChevron |<|
//@[044:047) Identifier |int|
//@[047:048) LeftParen |(|
//@[048:049) Identifier |y|
//@[049:050) RightParen |)|
//@[050:051) RightParen |)|
//@[051:053) NewLine |\n\n|

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[000:003) Identifier |var|
//@[004:022) Identifier |reduceStringConcat|
//@[023:024) Assignment |=|
//@[025:031) Identifier |reduce|
//@[031:032) LeftParen |(|
//@[032:033) LeftSquare |[|
//@[033:038) StringComplete |'abc'|
//@[038:039) Comma |,|
//@[040:045) StringComplete |'def'|
//@[045:046) Comma |,|
//@[047:052) StringComplete |'ghi'|
//@[052:053) RightSquare |]|
//@[053:054) Comma |,|
//@[055:057) StringComplete |''|
//@[057:058) Comma |,|
//@[059:060) LeftParen |(|
//@[060:063) Identifier |cur|
//@[063:064) Comma |,|
//@[065:069) Identifier |next|
//@[069:070) RightParen |)|
//@[071:073) Arrow |=>|
//@[074:080) Identifier |concat|
//@[080:081) LeftParen |(|
//@[081:084) Identifier |cur|
//@[084:085) Comma |,|
//@[086:090) Identifier |next|
//@[090:091) RightParen |)|
//@[091:092) RightParen |)|
//@[092:093) NewLine |\n|
var reduceStringConcatEven = reduce(['abc', 'def', 'ghi'], '', (cur, next, i) => isEven(i) ? concat(cur, next) : cur)
//@[000:003) Identifier |var|
//@[004:026) Identifier |reduceStringConcatEven|
//@[027:028) Assignment |=|
//@[029:035) Identifier |reduce|
//@[035:036) LeftParen |(|
//@[036:037) LeftSquare |[|
//@[037:042) StringComplete |'abc'|
//@[042:043) Comma |,|
//@[044:049) StringComplete |'def'|
//@[049:050) Comma |,|
//@[051:056) StringComplete |'ghi'|
//@[056:057) RightSquare |]|
//@[057:058) Comma |,|
//@[059:061) StringComplete |''|
//@[061:062) Comma |,|
//@[063:064) LeftParen |(|
//@[064:067) Identifier |cur|
//@[067:068) Comma |,|
//@[069:073) Identifier |next|
//@[073:074) Comma |,|
//@[075:076) Identifier |i|
//@[076:077) RightParen |)|
//@[078:080) Arrow |=>|
//@[081:087) Identifier |isEven|
//@[087:088) LeftParen |(|
//@[088:089) Identifier |i|
//@[089:090) RightParen |)|
//@[091:092) Question |?|
//@[093:099) Identifier |concat|
//@[099:100) LeftParen |(|
//@[100:103) Identifier |cur|
//@[103:104) Comma |,|
//@[105:109) Identifier |next|
//@[109:110) RightParen |)|
//@[111:112) Colon |:|
//@[113:116) Identifier |cur|
//@[116:117) RightParen |)|
//@[117:118) NewLine |\n|
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[000:003) Identifier |var|
//@[004:019) Identifier |reduceFactorial|
//@[020:021) Assignment |=|
//@[022:028) Identifier |reduce|
//@[028:029) LeftParen |(|
//@[029:034) Identifier |range|
//@[034:035) LeftParen |(|
//@[035:036) Integer |1|
//@[036:037) Comma |,|
//@[038:039) Integer |5|
//@[039:040) RightParen |)|
//@[040:041) Comma |,|
//@[042:043) Integer |1|
//@[043:044) Comma |,|
//@[045:046) LeftParen |(|
//@[046:049) Identifier |cur|
//@[049:050) Comma |,|
//@[051:055) Identifier |next|
//@[055:056) RightParen |)|
//@[057:059) Arrow |=>|
//@[060:063) Identifier |cur|
//@[064:065) Asterisk |*|
//@[066:070) Identifier |next|
//@[070:071) RightParen |)|
//@[071:072) NewLine |\n|
var reduceObjectUnion = reduce([
//@[000:003) Identifier |var|
//@[004:021) Identifier |reduceObjectUnion|
//@[022:023) Assignment |=|
//@[024:030) Identifier |reduce|
//@[030:031) LeftParen |(|
//@[031:032) LeftSquare |[|
//@[032:033) NewLine |\n|
  { foo: 123 }
//@[002:003) LeftBrace |{|
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:012) Integer |123|
//@[013:014) RightBrace |}|
//@[014:015) NewLine |\n|
  { bar: 456 }
//@[002:003) LeftBrace |{|
//@[004:007) Identifier |bar|
//@[007:008) Colon |:|
//@[009:012) Integer |456|
//@[013:014) RightBrace |}|
//@[014:015) NewLine |\n|
  { baz: 789 }
//@[002:003) LeftBrace |{|
//@[004:007) Identifier |baz|
//@[007:008) Colon |:|
//@[009:012) Integer |789|
//@[013:014) RightBrace |}|
//@[014:015) NewLine |\n|
], {}, (cur, next) => union(cur, next))
//@[000:001) RightSquare |]|
//@[001:002) Comma |,|
//@[003:004) LeftBrace |{|
//@[004:005) RightBrace |}|
//@[005:006) Comma |,|
//@[007:008) LeftParen |(|
//@[008:011) Identifier |cur|
//@[011:012) Comma |,|
//@[013:017) Identifier |next|
//@[017:018) RightParen |)|
//@[019:021) Arrow |=>|
//@[022:027) Identifier |union|
//@[027:028) LeftParen |(|
//@[028:031) Identifier |cur|
//@[031:032) Comma |,|
//@[033:037) Identifier |next|
//@[037:038) RightParen |)|
//@[038:039) RightParen |)|
//@[039:040) NewLine |\n|
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[000:003) Identifier |var|
//@[004:015) Identifier |reduceEmpty|
//@[016:017) Assignment |=|
//@[018:024) Identifier |reduce|
//@[024:025) LeftParen |(|
//@[025:026) LeftSquare |[|
//@[026:027) RightSquare |]|
//@[027:028) Comma |,|
//@[029:030) Integer |0|
//@[030:031) Comma |,|
//@[032:033) LeftParen |(|
//@[033:036) Identifier |cur|
//@[036:037) Comma |,|
//@[038:042) Identifier |next|
//@[042:043) RightParen |)|
//@[044:046) Arrow |=>|
//@[047:050) Identifier |cur|
//@[050:051) RightParen |)|
//@[051:053) NewLine |\n\n|

var itemForLoop = [for item in range(0, 10): item]
//@[000:003) Identifier |var|
//@[004:015) Identifier |itemForLoop|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:022) Identifier |for|
//@[023:027) Identifier |item|
//@[028:030) Identifier |in|
//@[031:036) Identifier |range|
//@[036:037) LeftParen |(|
//@[037:038) Integer |0|
//@[038:039) Comma |,|
//@[040:042) Integer |10|
//@[042:043) RightParen |)|
//@[043:044) Colon |:|
//@[045:049) Identifier |item|
//@[049:050) RightSquare |]|
//@[050:051) NewLine |\n|
var filteredLoop = filter(itemForLoop, i => i > 5)
//@[000:003) Identifier |var|
//@[004:016) Identifier |filteredLoop|
//@[017:018) Assignment |=|
//@[019:025) Identifier |filter|
//@[025:026) LeftParen |(|
//@[026:037) Identifier |itemForLoop|
//@[037:038) Comma |,|
//@[039:040) Identifier |i|
//@[041:043) Arrow |=>|
//@[044:045) Identifier |i|
//@[046:047) RightChevron |>|
//@[048:049) Integer |5|
//@[049:050) RightParen |)|
//@[050:052) NewLine |\n\n|

output doggoGreetings array = [for item in mapObject: item.greeting]
//@[000:006) Identifier |output|
//@[007:021) Identifier |doggoGreetings|
//@[022:027) Identifier |array|
//@[028:029) Assignment |=|
//@[030:031) LeftSquare |[|
//@[031:034) Identifier |for|
//@[035:039) Identifier |item|
//@[040:042) Identifier |in|
//@[043:052) Identifier |mapObject|
//@[052:053) Colon |:|
//@[054:058) Identifier |item|
//@[058:059) Dot |.|
//@[059:067) Identifier |greeting|
//@[067:068) RightSquare |]|
//@[068:070) NewLine |\n\n|

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |storageAcc|
//@[020:066) StringComplete |'Microsoft.Storage/storageAccounts@2021-09-01'|
//@[067:075) Identifier |existing|
//@[076:077) Assignment |=|
//@[078:079) LeftBrace |{|
//@[079:080) NewLine |\n|
  name: 'asdfsadf'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'asdfsadf'|
//@[018:019) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)
//@[000:003) Identifier |var|
//@[004:018) Identifier |mappedResProps|
//@[019:020) Assignment |=|
//@[021:024) Identifier |map|
//@[024:025) LeftParen |(|
//@[025:030) Identifier |items|
//@[030:031) LeftParen |(|
//@[031:041) Identifier |storageAcc|
//@[041:042) Dot |.|
//@[042:052) Identifier |properties|
//@[052:053) Dot |.|
//@[053:071) Identifier |secondaryEndpoints|
//@[071:072) RightParen |)|
//@[072:073) Comma |,|
//@[074:078) Identifier |item|
//@[079:081) Arrow |=>|
//@[082:086) Identifier |item|
//@[086:087) Dot |.|
//@[087:092) Identifier |value|
//@[092:093) RightParen |)|
//@[093:095) NewLine |\n\n|

module myMod './test.bicep' = {
//@[000:006) Identifier |module|
//@[007:012) Identifier |myMod|
//@[013:027) StringComplete |'./test.bicep'|
//@[028:029) Assignment |=|
//@[030:031) LeftBrace |{|
//@[031:032) NewLine |\n|
  name: 'asdfsadf'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'asdfsadf'|
//@[018:019) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    outputThis: map(mapObject, obj => obj.doggo)
//@[004:014) Identifier |outputThis|
//@[014:015) Colon |:|
//@[016:019) Identifier |map|
//@[019:020) LeftParen |(|
//@[020:029) Identifier |mapObject|
//@[029:030) Comma |,|
//@[031:034) Identifier |obj|
//@[035:037) Arrow |=>|
//@[038:041) Identifier |obj|
//@[041:042) Dot |.|
//@[042:047) Identifier |doggo|
//@[047:048) RightParen |)|
//@[048:049) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')
//@[000:003) Identifier |var|
//@[004:024) Identifier |mappedModOutputProps|
//@[025:026) Assignment |=|
//@[027:030) Identifier |map|
//@[030:031) LeftParen |(|
//@[031:036) Identifier |myMod|
//@[036:037) Dot |.|
//@[037:044) Identifier |outputs|
//@[044:045) Dot |.|
//@[045:055) Identifier |outputThis|
//@[055:056) Comma |,|
//@[057:062) Identifier |doggo|
//@[063:065) Arrow |=>|
//@[066:069) StringLeftPiece |'${|
//@[069:074) Identifier |doggo|
//@[074:086) StringRightPiece |} says bork'|
//@[086:087) RightParen |)|
//@[087:089) NewLine |\n\n|

var parentheses = map([123], (i => '${i}'))
//@[000:003) Identifier |var|
//@[004:015) Identifier |parentheses|
//@[016:017) Assignment |=|
//@[018:021) Identifier |map|
//@[021:022) LeftParen |(|
//@[022:023) LeftSquare |[|
//@[023:026) Integer |123|
//@[026:027) RightSquare |]|
//@[027:028) Comma |,|
//@[029:030) LeftParen |(|
//@[030:031) Identifier |i|
//@[032:034) Arrow |=>|
//@[035:038) StringLeftPiece |'${|
//@[038:039) Identifier |i|
//@[039:041) StringRightPiece |}'|
//@[041:042) RightParen |)|
//@[042:043) RightParen |)|
//@[043:045) NewLine |\n\n|

var objectMap = toObject([123, 456, 789], i => '${i / 100}')
//@[000:003) Identifier |var|
//@[004:013) Identifier |objectMap|
//@[014:015) Assignment |=|
//@[016:024) Identifier |toObject|
//@[024:025) LeftParen |(|
//@[025:026) LeftSquare |[|
//@[026:029) Integer |123|
//@[029:030) Comma |,|
//@[031:034) Integer |456|
//@[034:035) Comma |,|
//@[036:039) Integer |789|
//@[039:040) RightSquare |]|
//@[040:041) Comma |,|
//@[042:043) Identifier |i|
//@[044:046) Arrow |=>|
//@[047:050) StringLeftPiece |'${|
//@[050:051) Identifier |i|
//@[052:053) Slash |/|
//@[054:057) Integer |100|
//@[057:059) StringRightPiece |}'|
//@[059:060) RightParen |)|
//@[060:061) NewLine |\n|
var objectMap2 = toObject(range(0, 10), i => '${i}', i => {
//@[000:003) Identifier |var|
//@[004:014) Identifier |objectMap2|
//@[015:016) Assignment |=|
//@[017:025) Identifier |toObject|
//@[025:026) LeftParen |(|
//@[026:031) Identifier |range|
//@[031:032) LeftParen |(|
//@[032:033) Integer |0|
//@[033:034) Comma |,|
//@[035:037) Integer |10|
//@[037:038) RightParen |)|
//@[038:039) Comma |,|
//@[040:041) Identifier |i|
//@[042:044) Arrow |=>|
//@[045:048) StringLeftPiece |'${|
//@[048:049) Identifier |i|
//@[049:051) StringRightPiece |}'|
//@[051:052) Comma |,|
//@[053:054) Identifier |i|
//@[055:057) Arrow |=>|
//@[058:059) LeftBrace |{|
//@[059:060) NewLine |\n|
  isEven: (i % 2) == 0
//@[002:008) Identifier |isEven|
//@[008:009) Colon |:|
//@[010:011) LeftParen |(|
//@[011:012) Identifier |i|
//@[013:014) Modulo |%|
//@[015:016) Integer |2|
//@[016:017) RightParen |)|
//@[018:020) Equals |==|
//@[021:022) Integer |0|
//@[022:023) NewLine |\n|
  isGreaterThan4: (i > 4)
//@[002:016) Identifier |isGreaterThan4|
//@[016:017) Colon |:|
//@[018:019) LeftParen |(|
//@[019:020) Identifier |i|
//@[021:022) RightChevron |>|
//@[023:024) Integer |4|
//@[024:025) RightParen |)|
//@[025:026) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@[000:003) Identifier |var|
//@[004:014) Identifier |objectMap3|
//@[015:016) Assignment |=|
//@[017:025) Identifier |toObject|
//@[025:026) LeftParen |(|
//@[026:041) Identifier |sortByObjectKey|
//@[041:042) Comma |,|
//@[043:044) Identifier |x|
//@[045:047) Arrow |=>|
//@[048:049) Identifier |x|
//@[049:050) Dot |.|
//@[050:054) Identifier |name|
//@[054:055) RightParen |)|
//@[055:056) NewLine |\n|
var objectMap4 = toObject(sortByObjectKey, x =>
//@[000:003) Identifier |var|
//@[004:014) Identifier |objectMap4|
//@[015:016) Assignment |=|
//@[017:025) Identifier |toObject|
//@[025:026) LeftParen |(|
//@[026:041) Identifier |sortByObjectKey|
//@[041:042) Comma |,|
//@[043:044) Identifier |x|
//@[045:047) Arrow |=>|
//@[047:048) NewLine |\n|
  
//@[002:003) NewLine |\n|
  x.name)
//@[002:003) Identifier |x|
//@[003:004) Dot |.|
//@[004:008) Identifier |name|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
var objectMap5 = toObject(sortByObjectKey, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx => xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.name)
//@[000:003) Identifier |var|
//@[004:014) Identifier |objectMap5|
//@[015:016) Assignment |=|
//@[017:025) Identifier |toObject|
//@[025:026) LeftParen |(|
//@[026:041) Identifier |sortByObjectKey|
//@[041:042) Comma |,|
//@[043:081) Identifier |xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx|
//@[082:084) Arrow |=>|
//@[085:123) Identifier |xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx|
//@[123:124) Dot |.|
//@[124:128) Identifier |name|
//@[128:129) RightParen |)|
//@[129:130) NewLine |\n|
var objectMap6 = toObject(range(0, 10), i => '${i}', i => // comment
//@[000:003) Identifier |var|
//@[004:014) Identifier |objectMap6|
//@[015:016) Assignment |=|
//@[017:025) Identifier |toObject|
//@[025:026) LeftParen |(|
//@[026:031) Identifier |range|
//@[031:032) LeftParen |(|
//@[032:033) Integer |0|
//@[033:034) Comma |,|
//@[035:037) Integer |10|
//@[037:038) RightParen |)|
//@[038:039) Comma |,|
//@[040:041) Identifier |i|
//@[042:044) Arrow |=>|
//@[045:048) StringLeftPiece |'${|
//@[048:049) Identifier |i|
//@[049:051) StringRightPiece |}'|
//@[051:052) Comma |,|
//@[053:054) Identifier |i|
//@[055:057) Arrow |=>|
//@[068:069) NewLine |\n|
{
//@[000:001) LeftBrace |{|
//@[001:002) NewLine |\n|
  isEven: (i % 2) == 0
//@[002:008) Identifier |isEven|
//@[008:009) Colon |:|
//@[010:011) LeftParen |(|
//@[011:012) Identifier |i|
//@[013:014) Modulo |%|
//@[015:016) Integer |2|
//@[016:017) RightParen |)|
//@[018:020) Equals |==|
//@[021:022) Integer |0|
//@[022:023) NewLine |\n|
  isGreaterThan4: (i > 4)
//@[002:016) Identifier |isGreaterThan4|
//@[016:017) Colon |:|
//@[018:019) LeftParen |(|
//@[019:020) Identifier |i|
//@[021:022) RightChevron |>|
//@[023:024) Integer |4|
//@[024:025) RightParen |)|
//@[025:026) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:004) NewLine |\n\n|

var multiLine = reduce(['abc', 'def', 'ghi'], '', (
//@[000:003) Identifier |var|
//@[004:013) Identifier |multiLine|
//@[014:015) Assignment |=|
//@[016:022) Identifier |reduce|
//@[022:023) LeftParen |(|
//@[023:024) LeftSquare |[|
//@[024:029) StringComplete |'abc'|
//@[029:030) Comma |,|
//@[031:036) StringComplete |'def'|
//@[036:037) Comma |,|
//@[038:043) StringComplete |'ghi'|
//@[043:044) RightSquare |]|
//@[044:045) Comma |,|
//@[046:048) StringComplete |''|
//@[048:049) Comma |,|
//@[050:051) LeftParen |(|
//@[051:052) NewLine |\n|
  cur,
//@[002:005) Identifier |cur|
//@[005:006) Comma |,|
//@[006:007) NewLine |\n|
  next
//@[002:006) Identifier |next|
//@[006:007) NewLine |\n|
) => concat(cur, next))
//@[000:001) RightParen |)|
//@[002:004) Arrow |=>|
//@[005:011) Identifier |concat|
//@[011:012) LeftParen |(|
//@[012:015) Identifier |cur|
//@[015:016) Comma |,|
//@[017:021) Identifier |next|
//@[021:022) RightParen |)|
//@[022:023) RightParen |)|
//@[023:025) NewLine |\n\n|

var multiLineWithComment = reduce(['abc', 'def', 'ghi'], '', (
//@[000:003) Identifier |var|
//@[004:024) Identifier |multiLineWithComment|
//@[025:026) Assignment |=|
//@[027:033) Identifier |reduce|
//@[033:034) LeftParen |(|
//@[034:035) LeftSquare |[|
//@[035:040) StringComplete |'abc'|
//@[040:041) Comma |,|
//@[042:047) StringComplete |'def'|
//@[047:048) Comma |,|
//@[049:054) StringComplete |'ghi'|
//@[054:055) RightSquare |]|
//@[055:056) Comma |,|
//@[057:059) StringComplete |''|
//@[059:060) Comma |,|
//@[061:062) LeftParen |(|
//@[062:063) NewLine |\n|
  // comment
//@[012:013) NewLine |\n|
  cur,
//@[002:005) Identifier |cur|
//@[005:006) Comma |,|
//@[006:007) NewLine |\n|
  next
//@[002:006) Identifier |next|
//@[006:007) NewLine |\n|
) => concat(cur, next))
//@[000:001) RightParen |)|
//@[002:004) Arrow |=>|
//@[005:011) Identifier |concat|
//@[011:012) LeftParen |(|
//@[012:015) Identifier |cur|
//@[015:016) Comma |,|
//@[017:021) Identifier |next|
//@[021:022) RightParen |)|
//@[022:023) RightParen |)|
//@[023:025) NewLine |\n\n|

var mapVals = mapValues({
//@[000:003) Identifier |var|
//@[004:011) Identifier |mapVals|
//@[012:013) Assignment |=|
//@[014:023) Identifier |mapValues|
//@[023:024) LeftParen |(|
//@[024:025) LeftBrace |{|
//@[025:026) NewLine |\n|
  a: 123
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:008) Integer |123|
//@[008:009) NewLine |\n|
  b: 456
//@[002:003) Identifier |b|
//@[003:004) Colon |:|
//@[005:008) Integer |456|
//@[008:009) NewLine |\n|
}, val => val * 2)
//@[000:001) RightBrace |}|
//@[001:002) Comma |,|
//@[003:006) Identifier |val|
//@[007:009) Arrow |=>|
//@[010:013) Identifier |val|
//@[014:015) Asterisk |*|
//@[016:017) Integer |2|
//@[017:018) RightParen |)|
//@[018:020) NewLine |\n\n|

var objectKeysTest = objectKeys({
//@[000:003) Identifier |var|
//@[004:018) Identifier |objectKeysTest|
//@[019:020) Assignment |=|
//@[021:031) Identifier |objectKeys|
//@[031:032) LeftParen |(|
//@[032:033) LeftBrace |{|
//@[033:034) NewLine |\n|
  a: 123
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:008) Integer |123|
//@[008:009) NewLine |\n|
  b: 456
//@[002:003) Identifier |b|
//@[003:004) Colon |:|
//@[005:008) Integer |456|
//@[008:009) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:004) NewLine |\n\n|

var shallowMergeTest = shallowMerge([{
//@[000:003) Identifier |var|
//@[004:020) Identifier |shallowMergeTest|
//@[021:022) Assignment |=|
//@[023:035) Identifier |shallowMerge|
//@[035:036) LeftParen |(|
//@[036:037) LeftSquare |[|
//@[037:038) LeftBrace |{|
//@[038:039) NewLine |\n|
  a: 123
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:008) Integer |123|
//@[008:009) NewLine |\n|
}, {
//@[000:001) RightBrace |}|
//@[001:002) Comma |,|
//@[003:004) LeftBrace |{|
//@[004:005) NewLine |\n|
  b: 456
//@[002:003) Identifier |b|
//@[003:004) Colon |:|
//@[005:008) Integer |456|
//@[008:009) NewLine |\n|
}])
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:003) RightParen |)|
//@[003:005) NewLine |\n\n|

var groupByTest = groupBy([
//@[000:003) Identifier |var|
//@[004:015) Identifier |groupByTest|
//@[016:017) Assignment |=|
//@[018:025) Identifier |groupBy|
//@[025:026) LeftParen |(|
//@[026:027) LeftSquare |[|
//@[027:028) NewLine |\n|
  { type: 'a', value: 123 }
//@[002:003) LeftBrace |{|
//@[004:008) Identifier |type|
//@[008:009) Colon |:|
//@[010:013) StringComplete |'a'|
//@[013:014) Comma |,|
//@[015:020) Identifier |value|
//@[020:021) Colon |:|
//@[022:025) Integer |123|
//@[026:027) RightBrace |}|
//@[027:028) NewLine |\n|
  { type: 'b', value: 456 }
//@[002:003) LeftBrace |{|
//@[004:008) Identifier |type|
//@[008:009) Colon |:|
//@[010:013) StringComplete |'b'|
//@[013:014) Comma |,|
//@[015:020) Identifier |value|
//@[020:021) Colon |:|
//@[022:025) Integer |456|
//@[026:027) RightBrace |}|
//@[027:028) NewLine |\n|
  { type: 'a', value: 789 }
//@[002:003) LeftBrace |{|
//@[004:008) Identifier |type|
//@[008:009) Colon |:|
//@[010:013) StringComplete |'a'|
//@[013:014) Comma |,|
//@[015:020) Identifier |value|
//@[020:021) Colon |:|
//@[022:025) Integer |789|
//@[026:027) RightBrace |}|
//@[027:028) NewLine |\n|
], arg => arg.type)
//@[000:001) RightSquare |]|
//@[001:002) Comma |,|
//@[003:006) Identifier |arg|
//@[007:009) Arrow |=>|
//@[010:013) Identifier |arg|
//@[013:014) Dot |.|
//@[014:018) Identifier |type|
//@[018:019) RightParen |)|
//@[019:021) NewLine |\n\n|

var groupByWithValMapTest = groupBy([
//@[000:003) Identifier |var|
//@[004:025) Identifier |groupByWithValMapTest|
//@[026:027) Assignment |=|
//@[028:035) Identifier |groupBy|
//@[035:036) LeftParen |(|
//@[036:037) LeftSquare |[|
//@[037:038) NewLine |\n|
  { type: 'a', value: 123 }
//@[002:003) LeftBrace |{|
//@[004:008) Identifier |type|
//@[008:009) Colon |:|
//@[010:013) StringComplete |'a'|
//@[013:014) Comma |,|
//@[015:020) Identifier |value|
//@[020:021) Colon |:|
//@[022:025) Integer |123|
//@[026:027) RightBrace |}|
//@[027:028) NewLine |\n|
  { type: 'b', value: 456 }
//@[002:003) LeftBrace |{|
//@[004:008) Identifier |type|
//@[008:009) Colon |:|
//@[010:013) StringComplete |'b'|
//@[013:014) Comma |,|
//@[015:020) Identifier |value|
//@[020:021) Colon |:|
//@[022:025) Integer |456|
//@[026:027) RightBrace |}|
//@[027:028) NewLine |\n|
  { type: 'a', value: 789 }
//@[002:003) LeftBrace |{|
//@[004:008) Identifier |type|
//@[008:009) Colon |:|
//@[010:013) StringComplete |'a'|
//@[013:014) Comma |,|
//@[015:020) Identifier |value|
//@[020:021) Colon |:|
//@[022:025) Integer |789|
//@[026:027) RightBrace |}|
//@[027:028) NewLine |\n|
], arg => arg.type, arg => arg.value)
//@[000:001) RightSquare |]|
//@[001:002) Comma |,|
//@[003:006) Identifier |arg|
//@[007:009) Arrow |=>|
//@[010:013) Identifier |arg|
//@[013:014) Dot |.|
//@[014:018) Identifier |type|
//@[018:019) Comma |,|
//@[020:023) Identifier |arg|
//@[024:026) Arrow |=>|
//@[027:030) Identifier |arg|
//@[030:031) Dot |.|
//@[031:036) Identifier |value|
//@[036:037) RightParen |)|
//@[037:038) NewLine |\n|

//@[000:000) EndOfFile ||
