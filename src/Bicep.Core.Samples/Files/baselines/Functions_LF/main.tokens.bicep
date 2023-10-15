func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:004) Identifier |func|
//@[005:013) Identifier |buildUrl|
//@[013:014) LeftParen |(|
//@[014:019) Identifier |https|
//@[020:024) Identifier |bool|
//@[024:025) Comma |,|
//@[026:034) Identifier |hostname|
//@[035:041) Identifier |string|
//@[041:042) Comma |,|
//@[043:047) Identifier |path|
//@[048:054) Identifier |string|
//@[054:055) RightParen |)|
//@[056:062) Identifier |string|
//@[063:065) Arrow |=>|
//@[066:069) StringLeftPiece |'${|
//@[069:074) Identifier |https|
//@[075:076) Question |?|
//@[077:084) StringComplete |'https'|
//@[085:086) Colon |:|
//@[087:093) StringComplete |'http'|
//@[093:099) StringMiddlePiece |}://${|
//@[099:107) Identifier |hostname|
//@[107:110) StringMiddlePiece |}${|
//@[110:115) Identifier |empty|
//@[115:116) LeftParen |(|
//@[116:120) Identifier |path|
//@[120:121) RightParen |)|
//@[122:123) Question |?|
//@[124:126) StringComplete |''|
//@[127:128) Colon |:|
//@[129:133) StringLeftPiece |'/${|
//@[133:137) Identifier |path|
//@[137:139) StringRightPiece |}'|
//@[139:141) StringRightPiece |}'|
//@[141:143) NewLine |\n\n|

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

func sayHello(name string) string => 'Hi ${name}!'
//@[000:004) Identifier |func|
//@[005:013) Identifier |sayHello|
//@[013:014) LeftParen |(|
//@[014:018) Identifier |name|
//@[019:025) Identifier |string|
//@[025:026) RightParen |)|
//@[027:033) Identifier |string|
//@[034:036) Arrow |=>|
//@[037:043) StringLeftPiece |'Hi ${|
//@[043:047) Identifier |name|
//@[047:050) StringRightPiece |}!'|
//@[050:052) NewLine |\n\n|

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

func objReturnType(name string) object => {
//@[000:004) Identifier |func|
//@[005:018) Identifier |objReturnType|
//@[018:019) LeftParen |(|
//@[019:023) Identifier |name|
//@[024:030) Identifier |string|
//@[030:031) RightParen |)|
//@[032:038) Identifier |object|
//@[039:041) Arrow |=>|
//@[042:043) LeftBrace |{|
//@[043:044) NewLine |\n|
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

func arrayReturnType(name string) array => [
//@[000:004) Identifier |func|
//@[005:020) Identifier |arrayReturnType|
//@[020:021) LeftParen |(|
//@[021:025) Identifier |name|
//@[026:032) Identifier |string|
//@[032:033) RightParen |)|
//@[034:039) Identifier |array|
//@[040:042) Arrow |=>|
//@[043:044) LeftSquare |[|
//@[044:045) NewLine |\n|
  name
//@[002:006) Identifier |name|
//@[006:007) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

func asdf(name string) array => [
//@[000:004) Identifier |func|
//@[005:009) Identifier |asdf|
//@[009:010) LeftParen |(|
//@[010:014) Identifier |name|
//@[015:021) Identifier |string|
//@[021:022) RightParen |)|
//@[023:028) Identifier |array|
//@[029:031) Arrow |=>|
//@[032:033) LeftSquare |[|
//@[033:034) NewLine |\n|
  'asdf'
//@[002:008) StringComplete |'asdf'|
//@[008:009) NewLine |\n|
  name
//@[002:006) Identifier |name|
//@[006:007) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

@minValue(0)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) Integer |0|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
type positiveInt = int
//@[000:004) Identifier |type|
//@[005:016) Identifier |positiveInt|
//@[017:018) Assignment |=|
//@[019:022) Identifier |int|
//@[022:024) NewLine |\n\n|

func typedArg(input string[]) positiveInt => length(input)
//@[000:004) Identifier |func|
//@[005:013) Identifier |typedArg|
//@[013:014) LeftParen |(|
//@[014:019) Identifier |input|
//@[020:026) Identifier |string|
//@[026:027) LeftSquare |[|
//@[027:028) RightSquare |]|
//@[028:029) RightParen |)|
//@[030:041) Identifier |positiveInt|
//@[042:044) Arrow |=>|
//@[045:051) Identifier |length|
//@[051:052) LeftParen |(|
//@[052:057) Identifier |input|
//@[057:058) RightParen |)|
//@[058:060) NewLine |\n\n|

func barTest() array => ['abc', 'def']
//@[000:004) Identifier |func|
//@[005:012) Identifier |barTest|
//@[012:013) LeftParen |(|
//@[013:014) RightParen |)|
//@[015:020) Identifier |array|
//@[021:023) Arrow |=>|
//@[024:025) LeftSquare |[|
//@[025:030) StringComplete |'abc'|
//@[030:031) Comma |,|
//@[032:037) StringComplete |'def'|
//@[037:038) RightSquare |]|
//@[038:039) NewLine |\n|
func fooTest() array => map(barTest(), a => 'Hello ${a}!')
//@[000:004) Identifier |func|
//@[005:012) Identifier |fooTest|
//@[012:013) LeftParen |(|
//@[013:014) RightParen |)|
//@[015:020) Identifier |array|
//@[021:023) Arrow |=>|
//@[024:027) Identifier |map|
//@[027:028) LeftParen |(|
//@[028:035) Identifier |barTest|
//@[035:036) LeftParen |(|
//@[036:037) RightParen |)|
//@[037:038) Comma |,|
//@[039:040) Identifier |a|
//@[041:043) Arrow |=>|
//@[044:053) StringLeftPiece |'Hello ${|
//@[053:054) Identifier |a|
//@[054:057) StringRightPiece |}!'|
//@[057:058) RightParen |)|
//@[058:060) NewLine |\n\n|

output fooValue array = fooTest()
//@[000:006) Identifier |output|
//@[007:015) Identifier |fooValue|
//@[016:021) Identifier |array|
//@[022:023) Assignment |=|
//@[024:031) Identifier |fooTest|
//@[031:032) LeftParen |(|
//@[032:033) RightParen |)|
//@[033:034) NewLine |\n|

//@[000:000) EndOfFile ||
