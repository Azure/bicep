func useRuntimeFunction() string => reference('foo').bar
//@[000:004) Identifier |func|
//@[005:023) Identifier |useRuntimeFunction|
//@[023:024) LeftParen |(|
//@[024:025) RightParen |)|
//@[026:032) Identifier |string|
//@[033:035) Arrow |=>|
//@[036:045) Identifier |reference|
//@[045:046) LeftParen |(|
//@[046:051) StringComplete |'foo'|
//@[051:052) RightParen |)|
//@[052:053) Dot |.|
//@[053:056) Identifier |bar|
//@[056:058) NewLine |\n\n|

func missingArgType(input) string => input
//@[000:004) Identifier |func|
//@[005:019) Identifier |missingArgType|
//@[019:020) LeftParen |(|
//@[020:025) Identifier |input|
//@[025:026) RightParen |)|
//@[027:033) Identifier |string|
//@[034:036) Arrow |=>|
//@[037:042) Identifier |input|
//@[042:044) NewLine |\n\n|

func missingOutputType(input string) => input
//@[000:004) Identifier |func|
//@[005:022) Identifier |missingOutputType|
//@[022:023) LeftParen |(|
//@[023:028) Identifier |input|
//@[029:035) Identifier |string|
//@[035:036) RightParen |)|
//@[037:039) Arrow |=>|
//@[040:045) Identifier |input|
//@[045:047) NewLine |\n\n|

func invalidType(input string) string => input
//@[000:004) Identifier |func|
//@[005:016) Identifier |invalidType|
//@[016:017) LeftParen |(|
//@[017:022) Identifier |input|
//@[023:029) Identifier |string|
//@[029:030) RightParen |)|
//@[031:037) Identifier |string|
//@[038:040) Arrow |=>|
//@[041:046) Identifier |input|
//@[046:048) NewLine |\n\n|

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

func madeUpTypeArgs(a notAType, b alsoNotAType) string => '${a}-${b}'
//@[000:004) Identifier |func|
//@[005:019) Identifier |madeUpTypeArgs|
//@[019:020) LeftParen |(|
//@[020:021) Identifier |a|
//@[022:030) Identifier |notAType|
//@[030:031) Comma |,|
//@[032:033) Identifier |b|
//@[034:046) Identifier |alsoNotAType|
//@[046:047) RightParen |)|
//@[048:054) Identifier |string|
//@[055:057) Arrow |=>|
//@[058:061) StringLeftPiece |'${|
//@[061:062) Identifier |a|
//@[062:066) StringMiddlePiece |}-${|
//@[066:067) Identifier |b|
//@[067:069) StringRightPiece |}'|
//@[069:071) NewLine |\n\n|

func noLambda('foo') string => ''
//@[000:004) Identifier |func|
//@[005:013) Identifier |noLambda|
//@[013:014) LeftParen |(|
//@[014:019) StringComplete |'foo'|
//@[019:020) RightParen |)|
//@[021:027) Identifier |string|
//@[028:030) Arrow |=>|
//@[031:033) StringComplete |''|
//@[033:035) NewLine |\n\n|

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

func argLengthMismatch(a string, b string, c string) array => ([a, b, c])
//@[000:004) Identifier |func|
//@[005:022) Identifier |argLengthMismatch|
//@[022:023) LeftParen |(|
//@[023:024) Identifier |a|
//@[025:031) Identifier |string|
//@[031:032) Comma |,|
//@[033:034) Identifier |b|
//@[035:041) Identifier |string|
//@[041:042) Comma |,|
//@[043:044) Identifier |c|
//@[045:051) Identifier |string|
//@[051:052) RightParen |)|
//@[053:058) Identifier |array|
//@[059:061) Arrow |=>|
//@[062:063) LeftParen |(|
//@[063:064) LeftSquare |[|
//@[064:065) Identifier |a|
//@[065:066) Comma |,|
//@[067:068) Identifier |b|
//@[068:069) Comma |,|
//@[070:071) Identifier |c|
//@[071:072) RightSquare |]|
//@[072:073) RightParen |)|
//@[073:074) NewLine |\n|
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
//@[050:051) NewLine |\n|
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

func sayHelloBadNewlines(
//@[000:004) Identifier |func|
//@[005:024) Identifier |sayHelloBadNewlines|
//@[024:025) LeftParen |(|
//@[025:026) NewLine |\n|
  name string) string => 'Hi ${name}!'
//@[002:006) Identifier |name|
//@[007:013) Identifier |string|
//@[013:014) RightParen |)|
//@[015:021) Identifier |string|
//@[022:024) Arrow |=>|
//@[025:031) StringLeftPiece |'Hi ${|
//@[031:035) Identifier |name|
//@[035:038) StringRightPiece |}!'|
//@[038:040) NewLine |\n\n|

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[000:004) Identifier |type|
//@[005:028) Identifier |validStringLiteralUnion|
//@[029:030) Assignment |=|
//@[031:036) StringComplete |'foo'|
//@[036:037) Pipe |||
//@[037:042) StringComplete |'bar'|
//@[042:043) Pipe |||
//@[043:048) StringComplete |'baz'|
//@[048:049) NewLine |\n|
func invalidArgs(a validStringLiteralUnion, b string) string => a
//@[000:004) Identifier |func|
//@[005:016) Identifier |invalidArgs|
//@[016:017) LeftParen |(|
//@[017:018) Identifier |a|
//@[019:042) Identifier |validStringLiteralUnion|
//@[042:043) Comma |,|
//@[044:045) Identifier |b|
//@[046:052) Identifier |string|
//@[052:053) RightParen |)|
//@[054:060) Identifier |string|
//@[061:063) Arrow |=>|
//@[064:065) Identifier |a|
//@[065:066) NewLine |\n|
func invalidOutput() validStringLiteralUnion => 'foo'
//@[000:004) Identifier |func|
//@[005:018) Identifier |invalidOutput|
//@[018:019) LeftParen |(|
//@[019:020) RightParen |)|
//@[021:044) Identifier |validStringLiteralUnion|
//@[045:047) Arrow |=>|
//@[048:053) StringComplete |'foo'|
//@[053:055) NewLine |\n\n|

func recursive() string => recursive()
//@[000:004) Identifier |func|
//@[005:014) Identifier |recursive|
//@[014:015) LeftParen |(|
//@[015:016) RightParen |)|
//@[017:023) Identifier |string|
//@[024:026) Arrow |=>|
//@[027:036) Identifier |recursive|
//@[036:037) LeftParen |(|
//@[037:038) RightParen |)|
//@[038:040) NewLine |\n\n|

func recursiveA() string => recursiveB()
//@[000:004) Identifier |func|
//@[005:015) Identifier |recursiveA|
//@[015:016) LeftParen |(|
//@[016:017) RightParen |)|
//@[018:024) Identifier |string|
//@[025:027) Arrow |=>|
//@[028:038) Identifier |recursiveB|
//@[038:039) LeftParen |(|
//@[039:040) RightParen |)|
//@[040:041) NewLine |\n|
func recursiveB() string => recursiveA()
//@[000:004) Identifier |func|
//@[005:015) Identifier |recursiveB|
//@[015:016) LeftParen |(|
//@[016:017) RightParen |)|
//@[018:024) Identifier |string|
//@[025:027) Arrow |=>|
//@[028:038) Identifier |recursiveA|
//@[038:039) LeftParen |(|
//@[039:040) RightParen |)|
//@[040:042) NewLine |\n\n|

func onlyComma(,) string => 'foo'
//@[000:004) Identifier |func|
//@[005:014) Identifier |onlyComma|
//@[014:015) LeftParen |(|
//@[015:016) Comma |,|
//@[016:017) RightParen |)|
//@[018:024) Identifier |string|
//@[025:027) Arrow |=>|
//@[028:033) StringComplete |'foo'|
//@[033:034) NewLine |\n|
func trailingCommas(a string,,) string => 'foo'
//@[000:004) Identifier |func|
//@[005:019) Identifier |trailingCommas|
//@[019:020) LeftParen |(|
//@[020:021) Identifier |a|
//@[022:028) Identifier |string|
//@[028:029) Comma |,|
//@[029:030) Comma |,|
//@[030:031) RightParen |)|
//@[032:038) Identifier |string|
//@[039:041) Arrow |=>|
//@[042:047) StringComplete |'foo'|
//@[047:048) NewLine |\n|
func multiLineOnly(
//@[000:004) Identifier |func|
//@[005:018) Identifier |multiLineOnly|
//@[018:019) LeftParen |(|
//@[019:020) NewLine |\n|
  a string
//@[002:003) Identifier |a|
//@[004:010) Identifier |string|
//@[010:011) NewLine |\n|
  b string) string => 'foo'
//@[002:003) Identifier |b|
//@[004:010) Identifier |string|
//@[010:011) RightParen |)|
//@[012:018) Identifier |string|
//@[019:021) Arrow |=>|
//@[022:027) StringComplete |'foo'|
//@[027:029) NewLine |\n\n|

func multiLineTrailingCommas(
//@[000:004) Identifier |func|
//@[005:028) Identifier |multiLineTrailingCommas|
//@[028:029) LeftParen |(|
//@[029:030) NewLine |\n|
  a string,
//@[002:003) Identifier |a|
//@[004:010) Identifier |string|
//@[010:011) Comma |,|
//@[011:012) NewLine |\n|
  ,) string => 'foo'
//@[002:003) Comma |,|
//@[003:004) RightParen |)|
//@[005:011) Identifier |string|
//@[012:014) Arrow |=>|
//@[015:020) StringComplete |'foo'|
//@[020:022) NewLine |\n\n|

func lineBeforeComma(
//@[000:004) Identifier |func|
//@[005:020) Identifier |lineBeforeComma|
//@[020:021) LeftParen |(|
//@[021:022) NewLine |\n|
  a string
//@[002:003) Identifier |a|
//@[004:010) Identifier |string|
//@[010:011) NewLine |\n|
  ,b string) string => 'foo'
//@[002:003) Comma |,|
//@[003:004) Identifier |b|
//@[005:011) Identifier |string|
//@[011:012) RightParen |)|
//@[013:019) Identifier |string|
//@[020:022) Arrow |=>|
//@[023:028) StringComplete |'foo'|
//@[028:031) NewLine |\n\n\n|


output likeWrongArgcount bool =like('abc')
//@[000:006) Identifier |output|
//@[007:024) Identifier |likeWrongArgcount|
//@[025:029) Identifier |bool|
//@[030:031) Assignment |=|
//@[031:035) Identifier |like|
//@[035:036) LeftParen |(|
//@[036:041) StringComplete |'abc'|
//@[041:042) RightParen |)|
//@[042:043) NewLine |\n|
output likeWrongArgcount2 bool =like('abcdef','a*','abcd*')
//@[000:006) Identifier |output|
//@[007:025) Identifier |likeWrongArgcount2|
//@[026:030) Identifier |bool|
//@[031:032) Assignment |=|
//@[032:036) Identifier |like|
//@[036:037) LeftParen |(|
//@[037:045) StringComplete |'abcdef'|
//@[045:046) Comma |,|
//@[046:050) StringComplete |'a*'|
//@[050:051) Comma |,|
//@[051:058) StringComplete |'abcd*'|
//@[058:059) RightParen |)|
//@[059:060) NewLine |\n|
output likeWrongType bool =like(123,'a*')
//@[000:006) Identifier |output|
//@[007:020) Identifier |likeWrongType|
//@[021:025) Identifier |bool|
//@[026:027) Assignment |=|
//@[027:031) Identifier |like|
//@[031:032) LeftParen |(|
//@[032:035) Integer |123|
//@[035:036) Comma |,|
//@[036:040) StringComplete |'a*'|
//@[040:041) RightParen |)|
//@[041:042) NewLine |\n|
output likeWrongReturnType string=like('abcd','a*')
//@[000:006) Identifier |output|
//@[007:026) Identifier |likeWrongReturnType|
//@[027:033) Identifier |string|
//@[033:034) Assignment |=|
//@[034:038) Identifier |like|
//@[038:039) LeftParen |(|
//@[039:045) StringComplete |'abcd'|
//@[045:046) Comma |,|
//@[046:050) StringComplete |'a*'|
//@[050:051) RightParen |)|
//@[051:052) NewLine |\n|

//@[000:000) EndOfFile ||
