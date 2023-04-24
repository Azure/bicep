func buildUrl = (bool https, string hostname, string path) => string '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
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
//@[062:068) Identifier |string|
//@[069:072) StringLeftPiece |'${|
//@[072:077) Identifier |https|
//@[078:079) Question |?|
//@[080:087) StringComplete |'https'|
//@[088:089) Colon |:|
//@[090:096) StringComplete |'http'|
//@[096:102) StringMiddlePiece |}://${|
//@[102:110) Identifier |hostname|
//@[110:113) StringMiddlePiece |}${|
//@[113:118) Identifier |empty|
//@[118:119) LeftParen |(|
//@[119:123) Identifier |path|
//@[123:124) RightParen |)|
//@[125:126) Question |?|
//@[127:129) StringComplete |''|
//@[130:131) Colon |:|
//@[132:136) StringLeftPiece |'/${|
//@[136:140) Identifier |path|
//@[140:142) StringRightPiece |}'|
//@[142:144) StringRightPiece |}'|
//@[144:146) NewLine |\n\n|

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

func sayHello = (string name) => string 'Hi ${name}!'
//@[000:004) Identifier |func|
//@[005:013) Identifier |sayHello|
//@[014:015) Assignment |=|
//@[016:017) LeftParen |(|
//@[017:023) Identifier |string|
//@[024:028) Identifier |name|
//@[028:029) RightParen |)|
//@[030:032) Arrow |=>|
//@[033:039) Identifier |string|
//@[040:046) StringLeftPiece |'Hi ${|
//@[046:050) Identifier |name|
//@[050:053) StringRightPiece |}!'|
//@[053:055) NewLine |\n\n|

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
//@[069:071) NewLine |\n\n|

func objReturnType = (string name) => object {
//@[000:004) Identifier |func|
//@[005:018) Identifier |objReturnType|
//@[019:020) Assignment |=|
//@[021:022) LeftParen |(|
//@[022:028) Identifier |string|
//@[029:033) Identifier |name|
//@[033:034) RightParen |)|
//@[035:037) Arrow |=>|
//@[038:044) Identifier |object|
//@[045:046) LeftBrace |{|
//@[046:047) NewLine |\n|
  hello: 'Hi ${name}!'
//@[002:007) Identifier |hello|
//@[007:008) Colon |:|
//@[009:015) StringLeftPiece |'Hi ${|
//@[015:019) Identifier |name|
//@[019:022) StringRightPiece |}!'|
//@[022:023) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

func arrayReturnType = (string name) => array ([
//@[000:004) Identifier |func|
//@[005:020) Identifier |arrayReturnType|
//@[021:022) Assignment |=|
//@[023:024) LeftParen |(|
//@[024:030) Identifier |string|
//@[031:035) Identifier |name|
//@[035:036) RightParen |)|
//@[037:039) Arrow |=>|
//@[040:045) Identifier |array|
//@[046:047) LeftParen |(|
//@[047:048) LeftSquare |[|
//@[048:049) NewLine |\n|
  name
//@[002:006) Identifier |name|
//@[006:007) NewLine |\n|
])
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|

//@[000:000) EndOfFile ||
