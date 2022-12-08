param ids array
//@[000:005) Identifier |param|
//@[006:009) Identifier |ids|
//@[010:015) Identifier |array|
//@[015:017) NewLine |\n\n|

var flatten1 = flatten('abc')
//@[000:003) Identifier |var|
//@[004:012) Identifier |flatten1|
//@[013:014) Assignment |=|
//@[015:022) Identifier |flatten|
//@[022:023) LeftParen |(|
//@[023:028) StringComplete |'abc'|
//@[028:029) RightParen |)|
//@[029:030) NewLine |\n|
var flatten2 = flatten(['abc'], 'def')
//@[000:003) Identifier |var|
//@[004:012) Identifier |flatten2|
//@[013:014) Assignment |=|
//@[015:022) Identifier |flatten|
//@[022:023) LeftParen |(|
//@[023:024) LeftSquare |[|
//@[024:029) StringComplete |'abc'|
//@[029:030) RightSquare |]|
//@[030:031) Comma |,|
//@[032:037) StringComplete |'def'|
//@[037:038) RightParen |)|
//@[038:040) NewLine |\n\n|

var map1 = map('abc')
//@[000:003) Identifier |var|
//@[004:008) Identifier |map1|
//@[009:010) Assignment |=|
//@[011:014) Identifier |map|
//@[014:015) LeftParen |(|
//@[015:020) StringComplete |'abc'|
//@[020:021) RightParen |)|
//@[021:022) NewLine |\n|
var map2 = map('abc', 'def')
//@[000:003) Identifier |var|
//@[004:008) Identifier |map2|
//@[009:010) Assignment |=|
//@[011:014) Identifier |map|
//@[014:015) LeftParen |(|
//@[015:020) StringComplete |'abc'|
//@[020:021) Comma |,|
//@[022:027) StringComplete |'def'|
//@[027:028) RightParen |)|
//@[028:029) NewLine |\n|
var map3 = map(range(0, 10), 'def')
//@[000:003) Identifier |var|
//@[004:008) Identifier |map3|
//@[009:010) Assignment |=|
//@[011:014) Identifier |map|
//@[014:015) LeftParen |(|
//@[015:020) Identifier |range|
//@[020:021) LeftParen |(|
//@[021:022) Integer |0|
//@[022:023) Comma |,|
//@[024:026) Integer |10|
//@[026:027) RightParen |)|
//@[027:028) Comma |,|
//@[029:034) StringComplete |'def'|
//@[034:035) RightParen |)|
//@[035:036) NewLine |\n|
var map4 = map(range(0, 10), () => null)
//@[000:003) Identifier |var|
//@[004:008) Identifier |map4|
//@[009:010) Assignment |=|
//@[011:014) Identifier |map|
//@[014:015) LeftParen |(|
//@[015:020) Identifier |range|
//@[020:021) LeftParen |(|
//@[021:022) Integer |0|
//@[022:023) Comma |,|
//@[024:026) Integer |10|
//@[026:027) RightParen |)|
//@[027:028) Comma |,|
//@[029:030) LeftParen |(|
//@[030:031) RightParen |)|
//@[032:034) Arrow |=>|
//@[035:039) NullKeyword |null|
//@[039:040) RightParen |)|
//@[040:042) NewLine |\n\n|

var filter1 = filter('abc')
//@[000:003) Identifier |var|
//@[004:011) Identifier |filter1|
//@[012:013) Assignment |=|
//@[014:020) Identifier |filter|
//@[020:021) LeftParen |(|
//@[021:026) StringComplete |'abc'|
//@[026:027) RightParen |)|
//@[027:028) NewLine |\n|
var filter2 = filter('abc', 'def')
//@[000:003) Identifier |var|
//@[004:011) Identifier |filter2|
//@[012:013) Assignment |=|
//@[014:020) Identifier |filter|
//@[020:021) LeftParen |(|
//@[021:026) StringComplete |'abc'|
//@[026:027) Comma |,|
//@[028:033) StringComplete |'def'|
//@[033:034) RightParen |)|
//@[034:035) NewLine |\n|
var filter3 = filter(range(0, 10), 'def')
//@[000:003) Identifier |var|
//@[004:011) Identifier |filter3|
//@[012:013) Assignment |=|
//@[014:020) Identifier |filter|
//@[020:021) LeftParen |(|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |0|
//@[028:029) Comma |,|
//@[030:032) Integer |10|
//@[032:033) RightParen |)|
//@[033:034) Comma |,|
//@[035:040) StringComplete |'def'|
//@[040:041) RightParen |)|
//@[041:042) NewLine |\n|
var filter4 = filter(range(0, 10), () => null)
//@[000:003) Identifier |var|
//@[004:011) Identifier |filter4|
//@[012:013) Assignment |=|
//@[014:020) Identifier |filter|
//@[020:021) LeftParen |(|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |0|
//@[028:029) Comma |,|
//@[030:032) Integer |10|
//@[032:033) RightParen |)|
//@[033:034) Comma |,|
//@[035:036) LeftParen |(|
//@[036:037) RightParen |)|
//@[038:040) Arrow |=>|
//@[041:045) NullKeyword |null|
//@[045:046) RightParen |)|
//@[046:047) NewLine |\n|
var filter5 = filter(range(0, 10), i => i)
//@[000:003) Identifier |var|
//@[004:011) Identifier |filter5|
//@[012:013) Assignment |=|
//@[014:020) Identifier |filter|
//@[020:021) LeftParen |(|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |0|
//@[028:029) Comma |,|
//@[030:032) Integer |10|
//@[032:033) RightParen |)|
//@[033:034) Comma |,|
//@[035:036) Identifier |i|
//@[037:039) Arrow |=>|
//@[040:041) Identifier |i|
//@[041:042) RightParen |)|
//@[042:043) NewLine |\n|
var filter6 = filter([true, 'hello!'], i => i)
//@[000:003) Identifier |var|
//@[004:011) Identifier |filter6|
//@[012:013) Assignment |=|
//@[014:020) Identifier |filter|
//@[020:021) LeftParen |(|
//@[021:022) LeftSquare |[|
//@[022:026) TrueKeyword |true|
//@[026:027) Comma |,|
//@[028:036) StringComplete |'hello!'|
//@[036:037) RightSquare |]|
//@[037:038) Comma |,|
//@[039:040) Identifier |i|
//@[041:043) Arrow |=>|
//@[044:045) Identifier |i|
//@[045:046) RightParen |)|
//@[046:048) NewLine |\n\n|

var sort1 = sort('abc')
//@[000:003) Identifier |var|
//@[004:009) Identifier |sort1|
//@[010:011) Assignment |=|
//@[012:016) Identifier |sort|
//@[016:017) LeftParen |(|
//@[017:022) StringComplete |'abc'|
//@[022:023) RightParen |)|
//@[023:024) NewLine |\n|
var sort2 = sort('abc', 'def')
//@[000:003) Identifier |var|
//@[004:009) Identifier |sort2|
//@[010:011) Assignment |=|
//@[012:016) Identifier |sort|
//@[016:017) LeftParen |(|
//@[017:022) StringComplete |'abc'|
//@[022:023) Comma |,|
//@[024:029) StringComplete |'def'|
//@[029:030) RightParen |)|
//@[030:031) NewLine |\n|
var sort3 = sort(range(0, 10), 'def')
//@[000:003) Identifier |var|
//@[004:009) Identifier |sort3|
//@[010:011) Assignment |=|
//@[012:016) Identifier |sort|
//@[016:017) LeftParen |(|
//@[017:022) Identifier |range|
//@[022:023) LeftParen |(|
//@[023:024) Integer |0|
//@[024:025) Comma |,|
//@[026:028) Integer |10|
//@[028:029) RightParen |)|
//@[029:030) Comma |,|
//@[031:036) StringComplete |'def'|
//@[036:037) RightParen |)|
//@[037:038) NewLine |\n|
var sort4 = sort(range(0, 10), () => null)
//@[000:003) Identifier |var|
//@[004:009) Identifier |sort4|
//@[010:011) Assignment |=|
//@[012:016) Identifier |sort|
//@[016:017) LeftParen |(|
//@[017:022) Identifier |range|
//@[022:023) LeftParen |(|
//@[023:024) Integer |0|
//@[024:025) Comma |,|
//@[026:028) Integer |10|
//@[028:029) RightParen |)|
//@[029:030) Comma |,|
//@[031:032) LeftParen |(|
//@[032:033) RightParen |)|
//@[034:036) Arrow |=>|
//@[037:041) NullKeyword |null|
//@[041:042) RightParen |)|
//@[042:043) NewLine |\n|
var sort5 = sort(range(0, 10), i => i)
//@[000:003) Identifier |var|
//@[004:009) Identifier |sort5|
//@[010:011) Assignment |=|
//@[012:016) Identifier |sort|
//@[016:017) LeftParen |(|
//@[017:022) Identifier |range|
//@[022:023) LeftParen |(|
//@[023:024) Integer |0|
//@[024:025) Comma |,|
//@[026:028) Integer |10|
//@[028:029) RightParen |)|
//@[029:030) Comma |,|
//@[031:032) Identifier |i|
//@[033:035) Arrow |=>|
//@[036:037) Identifier |i|
//@[037:038) RightParen |)|
//@[038:039) NewLine |\n|
var sort6 = sort(range(0, 10), (i, j) => i)
//@[000:003) Identifier |var|
//@[004:009) Identifier |sort6|
//@[010:011) Assignment |=|
//@[012:016) Identifier |sort|
//@[016:017) LeftParen |(|
//@[017:022) Identifier |range|
//@[022:023) LeftParen |(|
//@[023:024) Integer |0|
//@[024:025) Comma |,|
//@[026:028) Integer |10|
//@[028:029) RightParen |)|
//@[029:030) Comma |,|
//@[031:032) LeftParen |(|
//@[032:033) Identifier |i|
//@[033:034) Comma |,|
//@[035:036) Identifier |j|
//@[036:037) RightParen |)|
//@[038:040) Arrow |=>|
//@[041:042) Identifier |i|
//@[042:043) RightParen |)|
//@[043:045) NewLine |\n\n|

var reduce1 = reduce('abc')
//@[000:003) Identifier |var|
//@[004:011) Identifier |reduce1|
//@[012:013) Assignment |=|
//@[014:020) Identifier |reduce|
//@[020:021) LeftParen |(|
//@[021:026) StringComplete |'abc'|
//@[026:027) RightParen |)|
//@[027:028) NewLine |\n|
var reduce2 = reduce('abc', 'def', 'ghi')
//@[000:003) Identifier |var|
//@[004:011) Identifier |reduce2|
//@[012:013) Assignment |=|
//@[014:020) Identifier |reduce|
//@[020:021) LeftParen |(|
//@[021:026) StringComplete |'abc'|
//@[026:027) Comma |,|
//@[028:033) StringComplete |'def'|
//@[033:034) Comma |,|
//@[035:040) StringComplete |'ghi'|
//@[040:041) RightParen |)|
//@[041:042) NewLine |\n|
var reduce3 = reduce(range(0, 10), 0, 'def')
//@[000:003) Identifier |var|
//@[004:011) Identifier |reduce3|
//@[012:013) Assignment |=|
//@[014:020) Identifier |reduce|
//@[020:021) LeftParen |(|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |0|
//@[028:029) Comma |,|
//@[030:032) Integer |10|
//@[032:033) RightParen |)|
//@[033:034) Comma |,|
//@[035:036) Integer |0|
//@[036:037) Comma |,|
//@[038:043) StringComplete |'def'|
//@[043:044) RightParen |)|
//@[044:045) NewLine |\n|
var reduce4 = reduce(range(0, 10), 0, () => null)
//@[000:003) Identifier |var|
//@[004:011) Identifier |reduce4|
//@[012:013) Assignment |=|
//@[014:020) Identifier |reduce|
//@[020:021) LeftParen |(|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |0|
//@[028:029) Comma |,|
//@[030:032) Integer |10|
//@[032:033) RightParen |)|
//@[033:034) Comma |,|
//@[035:036) Integer |0|
//@[036:037) Comma |,|
//@[038:039) LeftParen |(|
//@[039:040) RightParen |)|
//@[041:043) Arrow |=>|
//@[044:048) NullKeyword |null|
//@[048:049) RightParen |)|
//@[049:050) NewLine |\n|
var reduce5 = reduce(range(0, 10), 0, i => i)
//@[000:003) Identifier |var|
//@[004:011) Identifier |reduce5|
//@[012:013) Assignment |=|
//@[014:020) Identifier |reduce|
//@[020:021) LeftParen |(|
//@[021:026) Identifier |range|
//@[026:027) LeftParen |(|
//@[027:028) Integer |0|
//@[028:029) Comma |,|
//@[030:032) Integer |10|
//@[032:033) RightParen |)|
//@[033:034) Comma |,|
//@[035:036) Integer |0|
//@[036:037) Comma |,|
//@[038:039) Identifier |i|
//@[040:042) Arrow |=>|
//@[043:044) Identifier |i|
//@[044:045) RightParen |)|
//@[045:047) NewLine |\n\n|

var toObject1 = toObject('abc')
//@[000:003) Identifier |var|
//@[004:013) Identifier |toObject1|
//@[014:015) Assignment |=|
//@[016:024) Identifier |toObject|
//@[024:025) LeftParen |(|
//@[025:030) StringComplete |'abc'|
//@[030:031) RightParen |)|
//@[031:032) NewLine |\n|
var toObject2 = toObject('abc', 'def')
//@[000:003) Identifier |var|
//@[004:013) Identifier |toObject2|
//@[014:015) Assignment |=|
//@[016:024) Identifier |toObject|
//@[024:025) LeftParen |(|
//@[025:030) StringComplete |'abc'|
//@[030:031) Comma |,|
//@[032:037) StringComplete |'def'|
//@[037:038) RightParen |)|
//@[038:039) NewLine |\n|
var toObject3 = toObject(range(0, 10), 'def')
//@[000:003) Identifier |var|
//@[004:013) Identifier |toObject3|
//@[014:015) Assignment |=|
//@[016:024) Identifier |toObject|
//@[024:025) LeftParen |(|
//@[025:030) Identifier |range|
//@[030:031) LeftParen |(|
//@[031:032) Integer |0|
//@[032:033) Comma |,|
//@[034:036) Integer |10|
//@[036:037) RightParen |)|
//@[037:038) Comma |,|
//@[039:044) StringComplete |'def'|
//@[044:045) RightParen |)|
//@[045:046) NewLine |\n|
var toObject4 = toObject(range(0, 10), () => null)
//@[000:003) Identifier |var|
//@[004:013) Identifier |toObject4|
//@[014:015) Assignment |=|
//@[016:024) Identifier |toObject|
//@[024:025) LeftParen |(|
//@[025:030) Identifier |range|
//@[030:031) LeftParen |(|
//@[031:032) Integer |0|
//@[032:033) Comma |,|
//@[034:036) Integer |10|
//@[036:037) RightParen |)|
//@[037:038) Comma |,|
//@[039:040) LeftParen |(|
//@[040:041) RightParen |)|
//@[042:044) Arrow |=>|
//@[045:049) NullKeyword |null|
//@[049:050) RightParen |)|
//@[050:051) NewLine |\n|
var toObject5 = toObject(range(0, 10), i => i)
//@[000:003) Identifier |var|
//@[004:013) Identifier |toObject5|
//@[014:015) Assignment |=|
//@[016:024) Identifier |toObject|
//@[024:025) LeftParen |(|
//@[025:030) Identifier |range|
//@[030:031) LeftParen |(|
//@[031:032) Integer |0|
//@[032:033) Comma |,|
//@[034:036) Integer |10|
//@[036:037) RightParen |)|
//@[037:038) Comma |,|
//@[039:040) Identifier |i|
//@[041:043) Arrow |=>|
//@[044:045) Identifier |i|
//@[045:046) RightParen |)|
//@[046:047) NewLine |\n|
var toObject6 = toObject(range(0, 10), i => '${i}', 'def')
//@[000:003) Identifier |var|
//@[004:013) Identifier |toObject6|
//@[014:015) Assignment |=|
//@[016:024) Identifier |toObject|
//@[024:025) LeftParen |(|
//@[025:030) Identifier |range|
//@[030:031) LeftParen |(|
//@[031:032) Integer |0|
//@[032:033) Comma |,|
//@[034:036) Integer |10|
//@[036:037) RightParen |)|
//@[037:038) Comma |,|
//@[039:040) Identifier |i|
//@[041:043) Arrow |=>|
//@[044:047) StringLeftPiece |'${|
//@[047:048) Identifier |i|
//@[048:050) StringRightPiece |}'|
//@[050:051) Comma |,|
//@[052:057) StringComplete |'def'|
//@[057:058) RightParen |)|
//@[058:059) NewLine |\n|
var toObject7 = toObject(range(0, 10), i => '${i}', () => null)
//@[000:003) Identifier |var|
//@[004:013) Identifier |toObject7|
//@[014:015) Assignment |=|
//@[016:024) Identifier |toObject|
//@[024:025) LeftParen |(|
//@[025:030) Identifier |range|
//@[030:031) LeftParen |(|
//@[031:032) Integer |0|
//@[032:033) Comma |,|
//@[034:036) Integer |10|
//@[036:037) RightParen |)|
//@[037:038) Comma |,|
//@[039:040) Identifier |i|
//@[041:043) Arrow |=>|
//@[044:047) StringLeftPiece |'${|
//@[047:048) Identifier |i|
//@[048:050) StringRightPiece |}'|
//@[050:051) Comma |,|
//@[052:053) LeftParen |(|
//@[053:054) RightParen |)|
//@[055:057) Arrow |=>|
//@[058:062) NullKeyword |null|
//@[062:063) RightParen |)|
//@[063:065) NewLine |\n\n|

var ternary = map([123], true ? i => '${i}' : i => 'hello!')
//@[000:003) Identifier |var|
//@[004:011) Identifier |ternary|
//@[012:013) Assignment |=|
//@[014:017) Identifier |map|
//@[017:018) LeftParen |(|
//@[018:019) LeftSquare |[|
//@[019:022) Integer |123|
//@[022:023) RightSquare |]|
//@[023:024) Comma |,|
//@[025:029) TrueKeyword |true|
//@[030:031) Question |?|
//@[032:033) Identifier |i|
//@[034:036) Arrow |=>|
//@[037:040) StringLeftPiece |'${|
//@[040:041) Identifier |i|
//@[041:043) StringRightPiece |}'|
//@[044:045) Colon |:|
//@[046:047) Identifier |i|
//@[048:050) Arrow |=>|
//@[051:059) StringComplete |'hello!'|
//@[059:060) RightParen |)|
//@[060:062) NewLine |\n\n|

var outsideArgs = i => 123
//@[000:003) Identifier |var|
//@[004:015) Identifier |outsideArgs|
//@[016:017) Assignment |=|
//@[018:019) Identifier |i|
//@[020:022) Arrow |=>|
//@[023:026) Integer |123|
//@[026:027) NewLine |\n|
var outsideArgs2 = (x, y, z) => '${x}${y}${z}'
//@[000:003) Identifier |var|
//@[004:016) Identifier |outsideArgs2|
//@[017:018) Assignment |=|
//@[019:020) LeftParen |(|
//@[020:021) Identifier |x|
//@[021:022) Comma |,|
//@[023:024) Identifier |y|
//@[024:025) Comma |,|
//@[026:027) Identifier |z|
//@[027:028) RightParen |)|
//@[029:031) Arrow |=>|
//@[032:035) StringLeftPiece |'${|
//@[035:036) Identifier |x|
//@[036:039) StringMiddlePiece |}${|
//@[039:040) Identifier |y|
//@[040:043) StringMiddlePiece |}${|
//@[043:044) Identifier |z|
//@[044:046) StringRightPiece |}'|
//@[046:047) NewLine |\n|
var partial = i =>
//@[000:003) Identifier |var|
//@[004:011) Identifier |partial|
//@[012:013) Assignment |=|
//@[014:015) Identifier |i|
//@[016:018) Arrow |=>|
//@[018:021) NewLine |\n\n\n|


var inObject = {
//@[000:003) Identifier |var|
//@[004:012) Identifier |inObject|
//@[013:014) Assignment |=|
//@[015:016) LeftBrace |{|
//@[016:017) NewLine |\n|
  a: i => i
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:006) Identifier |i|
//@[007:009) Arrow |=>|
//@[010:011) Identifier |i|
//@[011:012) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var inArray = [
//@[000:003) Identifier |var|
//@[004:011) Identifier |inArray|
//@[012:013) Assignment |=|
//@[014:015) LeftSquare |[|
//@[015:016) NewLine |\n|
  i => i
//@[002:003) Identifier |i|
//@[004:006) Arrow |=>|
//@[007:008) Identifier |i|
//@[008:009) NewLine |\n|
  j => j
//@[002:003) Identifier |j|
//@[004:006) Arrow |=>|
//@[007:008) Identifier |j|
//@[008:009) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

resource stg 'Microsoft.Storage/storageAccounts@2021-09-01' = [for i in range(0, 2): {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |stg|
//@[013:059) StringComplete |'Microsoft.Storage/storageAccounts@2021-09-01'|
//@[060:061) Assignment |=|
//@[062:063) LeftSquare |[|
//@[063:066) Identifier |for|
//@[067:068) Identifier |i|
//@[069:071) Identifier |in|
//@[072:077) Identifier |range|
//@[077:078) LeftParen |(|
//@[078:079) Integer |0|
//@[079:080) Comma |,|
//@[081:082) Integer |2|
//@[082:083) RightParen |)|
//@[083:084) Colon |:|
//@[085:086) LeftBrace |{|
//@[086:087) NewLine |\n|
  name: 'antteststg${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:021) StringLeftPiece |'antteststg${|
//@[021:022) Identifier |i|
//@[022:024) StringRightPiece |}'|
//@[024:025) NewLine |\n|
  location: 'West US'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:021) StringComplete |'West US'|
//@[021:022) NewLine |\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:020) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

output stgKeys array = map(range(0, 2), i => stg[i].listKeys().keys[0].value)
//@[000:006) Identifier |output|
//@[007:014) Identifier |stgKeys|
//@[015:020) Identifier |array|
//@[021:022) Assignment |=|
//@[023:026) Identifier |map|
//@[026:027) LeftParen |(|
//@[027:032) Identifier |range|
//@[032:033) LeftParen |(|
//@[033:034) Integer |0|
//@[034:035) Comma |,|
//@[036:037) Integer |2|
//@[037:038) RightParen |)|
//@[038:039) Comma |,|
//@[040:041) Identifier |i|
//@[042:044) Arrow |=>|
//@[045:048) Identifier |stg|
//@[048:049) LeftSquare |[|
//@[049:050) Identifier |i|
//@[050:051) RightSquare |]|
//@[051:052) Dot |.|
//@[052:060) Identifier |listKeys|
//@[060:061) LeftParen |(|
//@[061:062) RightParen |)|
//@[062:063) Dot |.|
//@[063:067) Identifier |keys|
//@[067:068) LeftSquare |[|
//@[068:069) Integer |0|
//@[069:070) RightSquare |]|
//@[070:071) Dot |.|
//@[071:076) Identifier |value|
//@[076:077) RightParen |)|
//@[077:078) NewLine |\n|
output stgKeys2 array = map(range(0, 2), j => stg[((j + 2) % 123)].listKeys().keys[0].value)
//@[000:006) Identifier |output|
//@[007:015) Identifier |stgKeys2|
//@[016:021) Identifier |array|
//@[022:023) Assignment |=|
//@[024:027) Identifier |map|
//@[027:028) LeftParen |(|
//@[028:033) Identifier |range|
//@[033:034) LeftParen |(|
//@[034:035) Integer |0|
//@[035:036) Comma |,|
//@[037:038) Integer |2|
//@[038:039) RightParen |)|
//@[039:040) Comma |,|
//@[041:042) Identifier |j|
//@[043:045) Arrow |=>|
//@[046:049) Identifier |stg|
//@[049:050) LeftSquare |[|
//@[050:051) LeftParen |(|
//@[051:052) LeftParen |(|
//@[052:053) Identifier |j|
//@[054:055) Plus |+|
//@[056:057) Integer |2|
//@[057:058) RightParen |)|
//@[059:060) Modulo |%|
//@[061:064) Integer |123|
//@[064:065) RightParen |)|
//@[065:066) RightSquare |]|
//@[066:067) Dot |.|
//@[067:075) Identifier |listKeys|
//@[075:076) LeftParen |(|
//@[076:077) RightParen |)|
//@[077:078) Dot |.|
//@[078:082) Identifier |keys|
//@[082:083) LeftSquare |[|
//@[083:084) Integer |0|
//@[084:085) RightSquare |]|
//@[085:086) Dot |.|
//@[086:091) Identifier |value|
//@[091:092) RightParen |)|
//@[092:093) NewLine |\n|
output stgKeys3 array = map(ids, id => listKeys(id, stg[0].apiVersion).keys[0].value)
//@[000:006) Identifier |output|
//@[007:015) Identifier |stgKeys3|
//@[016:021) Identifier |array|
//@[022:023) Assignment |=|
//@[024:027) Identifier |map|
//@[027:028) LeftParen |(|
//@[028:031) Identifier |ids|
//@[031:032) Comma |,|
//@[033:035) Identifier |id|
//@[036:038) Arrow |=>|
//@[039:047) Identifier |listKeys|
//@[047:048) LeftParen |(|
//@[048:050) Identifier |id|
//@[050:051) Comma |,|
//@[052:055) Identifier |stg|
//@[055:056) LeftSquare |[|
//@[056:057) Integer |0|
//@[057:058) RightSquare |]|
//@[058:059) Dot |.|
//@[059:069) Identifier |apiVersion|
//@[069:070) RightParen |)|
//@[070:071) Dot |.|
//@[071:075) Identifier |keys|
//@[075:076) LeftSquare |[|
//@[076:077) Integer |0|
//@[077:078) RightSquare |]|
//@[078:079) Dot |.|
//@[079:084) Identifier |value|
//@[084:085) RightParen |)|
//@[085:086) NewLine |\n|
output accessTiers array = map(range(0, 2), k => stg[k].properties.accessTier)
//@[000:006) Identifier |output|
//@[007:018) Identifier |accessTiers|
//@[019:024) Identifier |array|
//@[025:026) Assignment |=|
//@[027:030) Identifier |map|
//@[030:031) LeftParen |(|
//@[031:036) Identifier |range|
//@[036:037) LeftParen |(|
//@[037:038) Integer |0|
//@[038:039) Comma |,|
//@[040:041) Integer |2|
//@[041:042) RightParen |)|
//@[042:043) Comma |,|
//@[044:045) Identifier |k|
//@[046:048) Arrow |=>|
//@[049:052) Identifier |stg|
//@[052:053) LeftSquare |[|
//@[053:054) Identifier |k|
//@[054:055) RightSquare |]|
//@[055:056) Dot |.|
//@[056:066) Identifier |properties|
//@[066:067) Dot |.|
//@[067:077) Identifier |accessTier|
//@[077:078) RightParen |)|
//@[078:079) NewLine |\n|
output accessTiers2 array = map(range(0, 2), x => map(range(0, 2), y => stg[x / y].properties.accessTier))
//@[000:006) Identifier |output|
//@[007:019) Identifier |accessTiers2|
//@[020:025) Identifier |array|
//@[026:027) Assignment |=|
//@[028:031) Identifier |map|
//@[031:032) LeftParen |(|
//@[032:037) Identifier |range|
//@[037:038) LeftParen |(|
//@[038:039) Integer |0|
//@[039:040) Comma |,|
//@[041:042) Integer |2|
//@[042:043) RightParen |)|
//@[043:044) Comma |,|
//@[045:046) Identifier |x|
//@[047:049) Arrow |=>|
//@[050:053) Identifier |map|
//@[053:054) LeftParen |(|
//@[054:059) Identifier |range|
//@[059:060) LeftParen |(|
//@[060:061) Integer |0|
//@[061:062) Comma |,|
//@[063:064) Integer |2|
//@[064:065) RightParen |)|
//@[065:066) Comma |,|
//@[067:068) Identifier |y|
//@[069:071) Arrow |=>|
//@[072:075) Identifier |stg|
//@[075:076) LeftSquare |[|
//@[076:077) Identifier |x|
//@[078:079) Slash |/|
//@[080:081) Identifier |y|
//@[081:082) RightSquare |]|
//@[082:083) Dot |.|
//@[083:093) Identifier |properties|
//@[093:094) Dot |.|
//@[094:104) Identifier |accessTier|
//@[104:105) RightParen |)|
//@[105:106) RightParen |)|
//@[106:107) NewLine |\n|
output accessTiers3 array = map(ids, foo => reference('${foo}').accessTier)
//@[000:006) Identifier |output|
//@[007:019) Identifier |accessTiers3|
//@[020:025) Identifier |array|
//@[026:027) Assignment |=|
//@[028:031) Identifier |map|
//@[031:032) LeftParen |(|
//@[032:035) Identifier |ids|
//@[035:036) Comma |,|
//@[037:040) Identifier |foo|
//@[041:043) Arrow |=>|
//@[044:053) Identifier |reference|
//@[053:054) LeftParen |(|
//@[054:057) StringLeftPiece |'${|
//@[057:060) Identifier |foo|
//@[060:062) StringRightPiece |}'|
//@[062:063) RightParen |)|
//@[063:064) Dot |.|
//@[064:074) Identifier |accessTier|
//@[074:075) RightParen |)|
//@[075:077) NewLine |\n\n|

module modLoop './empty.bicep' = [for item in range(0, 5): {
//@[000:006) Identifier |module|
//@[007:014) Identifier |modLoop|
//@[015:030) StringComplete |'./empty.bicep'|
//@[031:032) Assignment |=|
//@[033:034) LeftSquare |[|
//@[034:037) Identifier |for|
//@[038:042) Identifier |item|
//@[043:045) Identifier |in|
//@[046:051) Identifier |range|
//@[051:052) LeftParen |(|
//@[052:053) Integer |0|
//@[053:054) Comma |,|
//@[055:056) Integer |5|
//@[056:057) RightParen |)|
//@[057:058) Colon |:|
//@[059:060) LeftBrace |{|
//@[060:061) NewLine |\n|
  name: 'foo${item}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringLeftPiece |'foo${|
//@[014:018) Identifier |item|
//@[018:020) StringRightPiece |}'|
//@[020:021) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

var modLoopNames = map(modLoop, i => i.name)
//@[000:003) Identifier |var|
//@[004:016) Identifier |modLoopNames|
//@[017:018) Assignment |=|
//@[019:022) Identifier |map|
//@[022:023) LeftParen |(|
//@[023:030) Identifier |modLoop|
//@[030:031) Comma |,|
//@[032:033) Identifier |i|
//@[034:036) Arrow |=>|
//@[037:038) Identifier |i|
//@[038:039) Dot |.|
//@[039:043) Identifier |name|
//@[043:044) RightParen |)|
//@[044:045) NewLine |\n|
output modOutputs array = map(range(0, 5), i => modLoop[i].outputs.foo)
//@[000:006) Identifier |output|
//@[007:017) Identifier |modOutputs|
//@[018:023) Identifier |array|
//@[024:025) Assignment |=|
//@[026:029) Identifier |map|
//@[029:030) LeftParen |(|
//@[030:035) Identifier |range|
//@[035:036) LeftParen |(|
//@[036:037) Integer |0|
//@[037:038) Comma |,|
//@[039:040) Integer |5|
//@[040:041) RightParen |)|
//@[041:042) Comma |,|
//@[043:044) Identifier |i|
//@[045:047) Arrow |=>|
//@[048:055) Identifier |modLoop|
//@[055:056) LeftSquare |[|
//@[056:057) Identifier |i|
//@[057:058) RightSquare |]|
//@[058:059) Dot |.|
//@[059:066) Identifier |outputs|
//@[066:067) Dot |.|
//@[067:070) Identifier |foo|
//@[070:071) RightParen |)|
//@[071:072) NewLine |\n|

//@[000:000) EndOfFile ||
