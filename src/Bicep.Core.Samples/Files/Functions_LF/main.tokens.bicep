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

output foo string = buildUrl(true, 'google.com', 'search')
//@[000:006) Identifier |output|
//@[007:010) Identifier |foo|
//@[011:017) Identifier |string|
//@[018:019) Assignment |=|
//@[020:028) Identifier |buildUrl|
//@[028:029) LeftParen |(|
//@[029:033) TrueKeyword |true|
//@[033:034) Comma |,|
//@[035:047) StringComplete |'google.com'|
//@[047:048) Comma |,|
//@[049:057) StringComplete |'search'|
//@[057:058) RightParen |)|
//@[058:060) NewLine |\n\n|

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
//@[046:048) NewLine |\n\n|

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
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
//@[046:050) Identifier |name|
//@[051:053) Arrow |=>|
//@[054:062) Identifier |sayHello|
//@[062:063) LeftParen |(|
//@[063:067) Identifier |name|
//@[067:068) RightParen |)|
//@[068:069) RightParen |)|
//@[069:070) NewLine |\n|

//@[000:000) EndOfFile ||
