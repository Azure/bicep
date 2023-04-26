func useRuntimeFunction = () string => reference('foo').bar
//@[000:004) Identifier |func|
//@[005:023) Identifier |useRuntimeFunction|
//@[024:025) Assignment |=|
//@[026:027) LeftParen |(|
//@[027:028) RightParen |)|
//@[029:035) Identifier |string|
//@[036:038) Arrow |=>|
//@[039:048) Identifier |reference|
//@[048:049) LeftParen |(|
//@[049:054) StringComplete |'foo'|
//@[054:055) RightParen |)|
//@[055:056) Dot |.|
//@[056:059) Identifier |bar|
//@[059:061) NewLine |\n\n|

func constFunc = () string => 'A'
//@[000:004) Identifier |func|
//@[005:014) Identifier |constFunc|
//@[015:016) Assignment |=|
//@[017:018) LeftParen |(|
//@[018:019) RightParen |)|
//@[020:026) Identifier |string|
//@[027:029) Arrow |=>|
//@[030:033) StringComplete |'A'|
//@[033:034) NewLine |\n|
func funcWithOtherFuncRef = () string => constFunc()
//@[000:004) Identifier |func|
//@[005:025) Identifier |funcWithOtherFuncRef|
//@[026:027) Assignment |=|
//@[028:029) LeftParen |(|
//@[029:030) RightParen |)|
//@[031:037) Identifier |string|
//@[038:040) Arrow |=>|
//@[041:050) Identifier |constFunc|
//@[050:051) LeftParen |(|
//@[051:052) RightParen |)|
//@[052:054) NewLine |\n\n|

func missingArgType = (input) string => input
//@[000:004) Identifier |func|
//@[005:019) Identifier |missingArgType|
//@[020:021) Assignment |=|
//@[022:023) LeftParen |(|
//@[023:028) Identifier |input|
//@[028:029) RightParen |)|
//@[030:036) Identifier |string|
//@[037:039) Arrow |=>|
//@[040:045) Identifier |input|
//@[045:047) NewLine |\n\n|

func missingOutputType = (input string) => input
//@[000:004) Identifier |func|
//@[005:022) Identifier |missingOutputType|
//@[023:024) Assignment |=|
//@[025:026) LeftParen |(|
//@[026:031) Identifier |input|
//@[032:038) Identifier |string|
//@[038:039) RightParen |)|
//@[040:042) Arrow |=>|
//@[043:048) Identifier |input|
//@[048:050) NewLine |\n\n|

func invalidType = (input string) string => input
//@[000:004) Identifier |func|
//@[005:016) Identifier |invalidType|
//@[017:018) Assignment |=|
//@[019:020) LeftParen |(|
//@[020:025) Identifier |input|
//@[026:032) Identifier |string|
//@[032:033) RightParen |)|
//@[034:040) Identifier |string|
//@[041:043) Arrow |=>|
//@[044:049) Identifier |input|
//@[049:051) NewLine |\n\n|

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

func madeUpTypeArgs = (a notAType, b alsoNotAType) string => '${a}-${b}'
//@[000:004) Identifier |func|
//@[005:019) Identifier |madeUpTypeArgs|
//@[020:021) Assignment |=|
//@[022:023) LeftParen |(|
//@[023:024) Identifier |a|
//@[025:033) Identifier |notAType|
//@[033:034) Comma |,|
//@[035:036) Identifier |b|
//@[037:049) Identifier |alsoNotAType|
//@[049:050) RightParen |)|
//@[051:057) Identifier |string|
//@[058:060) Arrow |=>|
//@[061:064) StringLeftPiece |'${|
//@[064:065) Identifier |a|
//@[065:069) StringMiddlePiece |}-${|
//@[069:070) Identifier |b|
//@[070:072) StringRightPiece |}'|
//@[072:074) NewLine |\n\n|

func noLambda = ('foo') string => ''
//@[000:004) Identifier |func|
//@[005:013) Identifier |noLambda|
//@[014:015) Assignment |=|
//@[016:017) LeftParen |(|
//@[017:022) StringComplete |'foo'|
//@[022:023) RightParen |)|
//@[024:030) Identifier |string|
//@[031:033) Arrow |=>|
//@[034:036) StringComplete |''|
//@[036:038) NewLine |\n\n|

func noLambda2 = (sdf 'foo') string => ''
//@[000:004) Identifier |func|
//@[005:014) Identifier |noLambda2|
//@[015:016) Assignment |=|
//@[017:018) LeftParen |(|
//@[018:021) Identifier |sdf|
//@[022:027) StringComplete |'foo'|
//@[027:028) RightParen |)|
//@[029:035) Identifier |string|
//@[036:038) Arrow |=>|
//@[039:041) StringComplete |''|
//@[041:043) NewLine |\n\n|

func noLambda3 = string 'asdf'
//@[000:004) Identifier |func|
//@[005:014) Identifier |noLambda3|
//@[015:016) Assignment |=|
//@[017:023) Identifier |string|
//@[024:030) StringComplete |'asdf'|
//@[030:032) NewLine |\n\n|

func argLengthMismatch = (a string, b string, c string) array => ([a, b, c])
//@[000:004) Identifier |func|
//@[005:022) Identifier |argLengthMismatch|
//@[023:024) Assignment |=|
//@[025:026) LeftParen |(|
//@[026:027) Identifier |a|
//@[028:034) Identifier |string|
//@[034:035) Comma |,|
//@[036:037) Identifier |b|
//@[038:044) Identifier |string|
//@[044:045) Comma |,|
//@[046:047) Identifier |c|
//@[048:054) Identifier |string|
//@[054:055) RightParen |)|
//@[056:061) Identifier |array|
//@[062:064) Arrow |=>|
//@[065:066) LeftParen |(|
//@[066:067) LeftSquare |[|
//@[067:068) Identifier |a|
//@[068:069) Comma |,|
//@[070:071) Identifier |b|
//@[071:072) Comma |,|
//@[073:074) Identifier |c|
//@[074:075) RightSquare |]|
//@[075:076) RightParen |)|
//@[076:077) NewLine |\n|
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

func sayHello = (name string) string => 'Hi ${name}!'
//@[000:004) Identifier |func|
//@[005:013) Identifier |sayHello|
//@[014:015) Assignment |=|
//@[016:017) LeftParen |(|
//@[017:021) Identifier |name|
//@[022:028) Identifier |string|
//@[028:029) RightParen |)|
//@[030:036) Identifier |string|
//@[037:039) Arrow |=>|
//@[040:046) StringLeftPiece |'Hi ${|
//@[046:050) Identifier |name|
//@[050:053) StringRightPiece |}!'|
//@[053:054) NewLine |\n|
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
  name string) string => 'Hi ${name}!'
//@[002:006) Identifier |name|
//@[007:013) Identifier |string|
//@[013:014) RightParen |)|
//@[015:021) Identifier |string|
//@[022:024) Arrow |=>|
//@[025:031) StringLeftPiece |'Hi ${|
//@[031:035) Identifier |name|
//@[035:038) StringRightPiece |}!'|
//@[038:039) NewLine |\n|

//@[000:000) EndOfFile ||
