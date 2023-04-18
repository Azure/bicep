func useRuntimeFunction = () => reference('foo').bar
//@[000:004) Identifier |func|
//@[005:023) Identifier |useRuntimeFunction|
//@[024:025) Assignment |=|
//@[026:027) LeftParen |(|
//@[027:028) RightParen |)|
//@[029:031) Arrow |=>|
//@[032:041) Identifier |reference|
//@[041:042) LeftParen |(|
//@[042:047) StringComplete |'foo'|
//@[047:048) RightParen |)|
//@[048:049) Dot |.|
//@[049:052) Identifier |bar|
//@[052:054) NewLine |\n\n|

func constFunc = () => 'A'
//@[000:004) Identifier |func|
//@[005:014) Identifier |constFunc|
//@[015:016) Assignment |=|
//@[017:018) LeftParen |(|
//@[018:019) RightParen |)|
//@[020:022) Arrow |=>|
//@[023:026) StringComplete |'A'|
//@[026:027) NewLine |\n|
func funcWithOtherFuncRef = () => constFunc()
//@[000:004) Identifier |func|
//@[005:025) Identifier |funcWithOtherFuncRef|
//@[026:027) Assignment |=|
//@[028:029) LeftParen |(|
//@[029:030) RightParen |)|
//@[031:033) Arrow |=>|
//@[034:043) Identifier |constFunc|
//@[043:044) LeftParen |(|
//@[044:045) RightParen |)|
//@[045:047) NewLine |\n\n|

func invalidType = (string input) => input
//@[000:004) Identifier |func|
//@[005:016) Identifier |invalidType|
//@[017:018) Assignment |=|
//@[019:020) LeftParen |(|
//@[020:026) Identifier |string|
//@[027:032) Identifier |input|
//@[032:033) RightParen |)|
//@[034:036) Arrow |=>|
//@[037:042) Identifier |input|
//@[042:044) NewLine |\n\n|

output invalidType string = invalidType(true)
//@[000:006) Identifier |output|
//@[007:018) Identifier |invalidType|
//@[019:025) Identifier |string|
//@[026:027) Assignment |=|
//@[028:039) Identifier |invalidType|
//@[039:040) LeftParen |(|
//@[040:044) TrueKeyword |true|
//@[044:045) RightParen |)|
//@[045:047) NewLine |\n\n|

func madeUpTypeArgs = (notAType a, alsoNotAType b) => '${a}-${b}'
//@[000:004) Identifier |func|
//@[005:019) Identifier |madeUpTypeArgs|
//@[020:021) Assignment |=|
//@[022:023) LeftParen |(|
//@[023:031) Identifier |notAType|
//@[032:033) Identifier |a|
//@[033:034) Comma |,|
//@[035:047) Identifier |alsoNotAType|
//@[048:049) Identifier |b|
//@[049:050) RightParen |)|
//@[051:053) Arrow |=>|
//@[054:057) StringLeftPiece |'${|
//@[057:058) Identifier |a|
//@[058:062) StringMiddlePiece |}-${|
//@[062:063) Identifier |b|
//@[063:065) StringRightPiece |}'|
//@[065:067) NewLine |\n\n|

func noLambda = ('foo') => ''
//@[000:004) Identifier |func|
//@[005:013) Identifier |noLambda|
//@[014:015) Assignment |=|
//@[016:017) LeftParen |(|
//@[017:022) StringComplete |'foo'|
//@[022:023) RightParen |)|
//@[024:026) Arrow |=>|
//@[027:029) StringComplete |''|
//@[029:031) NewLine |\n\n|

func noLambda2 = ('foo' sdf) => ''
//@[000:004) Identifier |func|
//@[005:014) Identifier |noLambda2|
//@[015:016) Assignment |=|
//@[017:018) LeftParen |(|
//@[018:023) StringComplete |'foo'|
//@[024:027) Identifier |sdf|
//@[027:028) RightParen |)|
//@[029:031) Arrow |=>|
//@[032:034) StringComplete |''|
//@[034:036) NewLine |\n\n|

func noLambda3 = 'asdf'
//@[000:004) Identifier |func|
//@[005:014) Identifier |noLambda3|
//@[015:016) Assignment |=|
//@[017:023) StringComplete |'asdf'|
//@[023:025) NewLine |\n\n|

func argLengthMismatch = (string a, string b, string c) => [a, b, c]
//@[000:004) Identifier |func|
//@[005:022) Identifier |argLengthMismatch|
//@[023:024) Assignment |=|
//@[025:026) LeftParen |(|
//@[026:032) Identifier |string|
//@[033:034) Identifier |a|
//@[034:035) Comma |,|
//@[036:042) Identifier |string|
//@[043:044) Identifier |b|
//@[044:045) Comma |,|
//@[046:052) Identifier |string|
//@[053:054) Identifier |c|
//@[054:055) RightParen |)|
//@[056:058) Arrow |=>|
//@[059:060) LeftSquare |[|
//@[060:061) Identifier |a|
//@[061:062) Comma |,|
//@[063:064) Identifier |b|
//@[064:065) Comma |,|
//@[066:067) Identifier |c|
//@[067:068) RightSquare |]|
//@[068:069) NewLine |\n|
var sdf = argLengthMismatch('asdf')
//@[000:003) Identifier |var|
//@[004:007) Identifier |sdf|
//@[008:009) Assignment |=|
//@[010:027) Identifier |argLengthMismatch|
//@[027:028) LeftParen |(|
//@[028:034) StringComplete |'asdf'|
//@[034:035) RightParen |)|
//@[035:037) NewLine |\n\n|

var asdfwdf = noLambda('asd')
//@[000:003) Identifier |var|
//@[004:011) Identifier |asdfwdf|
//@[012:013) Assignment |=|
//@[014:022) Identifier |noLambda|
//@[022:023) LeftParen |(|
//@[023:028) StringComplete |'asd'|
//@[028:029) RightParen |)|
//@[029:031) NewLine |\n\n|

func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:004) Identifier |func|
//@[005:013) Identifier |buildUrl|
//@[014:015) Assignment |=|
//@[016:017) LeftParen |(|
//@[017:021) Identifier |bool|
//@[022:027) Identifier |https|
//@[027:028) Comma |,|
//@[029:035) Identifier |string|
//@[036:044) Identifier |hostname|
//@[044:045) Comma |,|
//@[046:052) Identifier |string|
//@[053:057) Identifier |path|
//@[057:058) RightParen |)|
//@[059:061) Arrow |=>|
//@[062:065) StringLeftPiece |'${|
//@[065:070) Identifier |https|
//@[071:072) Question |?|
//@[073:080) StringComplete |'https'|
//@[081:082) Colon |:|
//@[083:089) StringComplete |'http'|
//@[089:095) StringMiddlePiece |}://${|
//@[095:103) Identifier |hostname|
//@[103:106) StringMiddlePiece |}${|
//@[106:111) Identifier |empty|
//@[111:112) LeftParen |(|
//@[112:116) Identifier |path|
//@[116:117) RightParen |)|
//@[118:119) Question |?|
//@[120:122) StringComplete |''|
//@[123:124) Colon |:|
//@[125:129) StringLeftPiece |'/${|
//@[129:133) Identifier |path|
//@[133:135) StringRightPiece |}'|
//@[135:137) StringRightPiece |}'|
//@[137:139) NewLine |\n\n|

output foo array = buildUrl(true, 'google.com', 'search')
//@[000:006) Identifier |output|
//@[007:010) Identifier |foo|
//@[011:016) Identifier |array|
//@[017:018) Assignment |=|
//@[019:027) Identifier |buildUrl|
//@[027:028) LeftParen |(|
//@[028:032) TrueKeyword |true|
//@[032:033) Comma |,|
//@[034:046) StringComplete |'google.com'|
//@[046:047) Comma |,|
//@[048:056) StringComplete |'search'|
//@[056:057) RightParen |)|
//@[057:059) NewLine |\n\n|

func sayHello = (string name) => 'Hi ${name}!'
//@[000:004) Identifier |func|
//@[005:013) Identifier |sayHello|
//@[014:015) Assignment |=|
//@[016:017) LeftParen |(|
//@[017:023) Identifier |string|
//@[024:028) Identifier |name|
//@[028:029) RightParen |)|
//@[030:032) Arrow |=>|
//@[033:039) StringLeftPiece |'Hi ${|
//@[039:043) Identifier |name|
//@[043:046) StringRightPiece |}!'|
//@[046:047) NewLine |\n|
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[000:006) Identifier |output|
//@[007:013) Identifier |hellos|
//@[014:019) Identifier |array|
//@[020:021) Assignment |=|
//@[022:025) Identifier |map|
//@[025:026) LeftParen |(|
//@[026:027) LeftSquare |[|
//@[027:033) StringComplete |'Evie'|
//@[033:034) Comma |,|
//@[035:043) StringComplete |'Casper'|
//@[043:044) RightSquare |]|
//@[044:045) Comma |,|
//@[046:054) Identifier |sayHello|
//@[054:055) RightParen |)|
//@[113:115) NewLine |\n\n|

func sayHelloBadNewlines = (
//@[000:004) Identifier |func|
//@[005:024) Identifier |sayHelloBadNewlines|
//@[025:026) Assignment |=|
//@[027:028) LeftParen |(|
//@[028:029) NewLine |\n|
  string name) => 'Hi ${name}!'
//@[002:008) Identifier |string|
//@[009:013) Identifier |name|
//@[013:014) RightParen |)|
//@[015:017) Arrow |=>|
//@[018:024) StringLeftPiece |'Hi ${|
//@[024:028) Identifier |name|
//@[028:031) StringRightPiece |}!'|
//@[031:032) NewLine |\n|

//@[000:000) EndOfFile ||
