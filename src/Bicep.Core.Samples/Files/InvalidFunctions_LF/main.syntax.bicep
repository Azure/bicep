func useRuntimeFunction() string => reference('foo').bar
//@[000:880) ProgramSyntax
//@[000:056) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:023) | ├─IdentifierSyntax
//@[005:023) | | └─Token(Identifier) |useRuntimeFunction|
//@[023:056) | └─TypedLambdaSyntax
//@[023:025) |   ├─TypedVariableBlockSyntax
//@[023:024) |   | ├─Token(LeftParen) |(|
//@[024:025) |   | └─Token(RightParen) |)|
//@[026:032) |   ├─VariableAccessSyntax
//@[026:032) |   | └─IdentifierSyntax
//@[026:032) |   |   └─Token(Identifier) |string|
//@[033:035) |   ├─Token(Arrow) |=>|
//@[036:056) |   └─PropertyAccessSyntax
//@[036:052) |     ├─FunctionCallSyntax
//@[036:045) |     | ├─IdentifierSyntax
//@[036:045) |     | | └─Token(Identifier) |reference|
//@[045:046) |     | ├─Token(LeftParen) |(|
//@[046:051) |     | ├─FunctionArgumentSyntax
//@[046:051) |     | | └─StringSyntax
//@[046:051) |     | |   └─Token(StringComplete) |'foo'|
//@[051:052) |     | └─Token(RightParen) |)|
//@[052:053) |     ├─Token(Dot) |.|
//@[053:056) |     └─IdentifierSyntax
//@[053:056) |       └─Token(Identifier) |bar|
//@[056:058) ├─Token(NewLine) |\n\n|

func constFunc() string => 'A'
//@[000:030) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |constFunc|
//@[014:030) | └─TypedLambdaSyntax
//@[014:016) |   ├─TypedVariableBlockSyntax
//@[014:015) |   | ├─Token(LeftParen) |(|
//@[015:016) |   | └─Token(RightParen) |)|
//@[017:023) |   ├─VariableAccessSyntax
//@[017:023) |   | └─IdentifierSyntax
//@[017:023) |   |   └─Token(Identifier) |string|
//@[024:026) |   ├─Token(Arrow) |=>|
//@[027:030) |   └─StringSyntax
//@[027:030) |     └─Token(StringComplete) |'A'|
//@[030:031) ├─Token(NewLine) |\n|
func funcWithOtherFuncRef() string => constFunc()
//@[000:049) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:025) | ├─IdentifierSyntax
//@[005:025) | | └─Token(Identifier) |funcWithOtherFuncRef|
//@[025:049) | └─TypedLambdaSyntax
//@[025:027) |   ├─TypedVariableBlockSyntax
//@[025:026) |   | ├─Token(LeftParen) |(|
//@[026:027) |   | └─Token(RightParen) |)|
//@[028:034) |   ├─VariableAccessSyntax
//@[028:034) |   | └─IdentifierSyntax
//@[028:034) |   |   └─Token(Identifier) |string|
//@[035:037) |   ├─Token(Arrow) |=>|
//@[038:049) |   └─FunctionCallSyntax
//@[038:047) |     ├─IdentifierSyntax
//@[038:047) |     | └─Token(Identifier) |constFunc|
//@[047:048) |     ├─Token(LeftParen) |(|
//@[048:049) |     └─Token(RightParen) |)|
//@[049:051) ├─Token(NewLine) |\n\n|

func missingArgType(input) string => input
//@[000:042) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:019) | ├─IdentifierSyntax
//@[005:019) | | └─Token(Identifier) |missingArgType|
//@[019:042) | └─TypedLambdaSyntax
//@[019:026) |   ├─TypedVariableBlockSyntax
//@[019:020) |   | ├─Token(LeftParen) |(|
//@[020:025) |   | ├─TypedLocalVariableSyntax
//@[020:025) |   | | ├─IdentifierSyntax
//@[020:025) |   | | | └─Token(Identifier) |input|
//@[025:025) |   | | └─SkippedTriviaSyntax
//@[025:026) |   | └─Token(RightParen) |)|
//@[027:033) |   ├─VariableAccessSyntax
//@[027:033) |   | └─IdentifierSyntax
//@[027:033) |   |   └─Token(Identifier) |string|
//@[034:036) |   ├─Token(Arrow) |=>|
//@[037:042) |   └─VariableAccessSyntax
//@[037:042) |     └─IdentifierSyntax
//@[037:042) |       └─Token(Identifier) |input|
//@[042:044) ├─Token(NewLine) |\n\n|

func missingOutputType(input string) => input
//@[000:045) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:022) | ├─IdentifierSyntax
//@[005:022) | | └─Token(Identifier) |missingOutputType|
//@[022:045) | └─TypedLambdaSyntax
//@[022:036) |   ├─TypedVariableBlockSyntax
//@[022:023) |   | ├─Token(LeftParen) |(|
//@[023:035) |   | ├─TypedLocalVariableSyntax
//@[023:028) |   | | ├─IdentifierSyntax
//@[023:028) |   | | | └─Token(Identifier) |input|
//@[029:035) |   | | └─VariableAccessSyntax
//@[029:035) |   | |   └─IdentifierSyntax
//@[029:035) |   | |     └─Token(Identifier) |string|
//@[035:036) |   | └─Token(RightParen) |)|
//@[037:045) |   ├─SkippedTriviaSyntax
//@[037:039) |   | ├─Token(Arrow) |=>|
//@[040:045) |   | └─Token(Identifier) |input|
//@[045:045) |   ├─SkippedTriviaSyntax
//@[045:045) |   └─SkippedTriviaSyntax
//@[045:047) ├─Token(NewLine) |\n\n|

func invalidType(input string) string => input
//@[000:046) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:016) | ├─IdentifierSyntax
//@[005:016) | | └─Token(Identifier) |invalidType|
//@[016:046) | └─TypedLambdaSyntax
//@[016:030) |   ├─TypedVariableBlockSyntax
//@[016:017) |   | ├─Token(LeftParen) |(|
//@[017:029) |   | ├─TypedLocalVariableSyntax
//@[017:022) |   | | ├─IdentifierSyntax
//@[017:022) |   | | | └─Token(Identifier) |input|
//@[023:029) |   | | └─VariableAccessSyntax
//@[023:029) |   | |   └─IdentifierSyntax
//@[023:029) |   | |     └─Token(Identifier) |string|
//@[029:030) |   | └─Token(RightParen) |)|
//@[031:037) |   ├─VariableAccessSyntax
//@[031:037) |   | └─IdentifierSyntax
//@[031:037) |   |   └─Token(Identifier) |string|
//@[038:040) |   ├─Token(Arrow) |=>|
//@[041:046) |   └─VariableAccessSyntax
//@[041:046) |     └─IdentifierSyntax
//@[041:046) |       └─Token(Identifier) |input|
//@[046:048) ├─Token(NewLine) |\n\n|

output invalidType string = invalidType(true)
//@[000:045) ├─OutputDeclarationSyntax
//@[000:006) | ├─Token(Identifier) |output|
//@[007:018) | ├─IdentifierSyntax
//@[007:018) | | └─Token(Identifier) |invalidType|
//@[019:025) | ├─VariableAccessSyntax
//@[019:025) | | └─IdentifierSyntax
//@[019:025) | |   └─Token(Identifier) |string|
//@[026:027) | ├─Token(Assignment) |=|
//@[028:045) | └─FunctionCallSyntax
//@[028:039) |   ├─IdentifierSyntax
//@[028:039) |   | └─Token(Identifier) |invalidType|
//@[039:040) |   ├─Token(LeftParen) |(|
//@[040:044) |   ├─FunctionArgumentSyntax
//@[040:044) |   | └─BooleanLiteralSyntax
//@[040:044) |   |   └─Token(TrueKeyword) |true|
//@[044:045) |   └─Token(RightParen) |)|
//@[045:047) ├─Token(NewLine) |\n\n|

func madeUpTypeArgs(a notAType, b alsoNotAType) string => '${a}-${b}'
//@[000:069) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:019) | ├─IdentifierSyntax
//@[005:019) | | └─Token(Identifier) |madeUpTypeArgs|
//@[019:069) | └─TypedLambdaSyntax
//@[019:047) |   ├─TypedVariableBlockSyntax
//@[019:020) |   | ├─Token(LeftParen) |(|
//@[020:030) |   | ├─TypedLocalVariableSyntax
//@[020:021) |   | | ├─IdentifierSyntax
//@[020:021) |   | | | └─Token(Identifier) |a|
//@[022:030) |   | | └─VariableAccessSyntax
//@[022:030) |   | |   └─IdentifierSyntax
//@[022:030) |   | |     └─Token(Identifier) |notAType|
//@[030:031) |   | ├─Token(Comma) |,|
//@[032:046) |   | ├─TypedLocalVariableSyntax
//@[032:033) |   | | ├─IdentifierSyntax
//@[032:033) |   | | | └─Token(Identifier) |b|
//@[034:046) |   | | └─VariableAccessSyntax
//@[034:046) |   | |   └─IdentifierSyntax
//@[034:046) |   | |     └─Token(Identifier) |alsoNotAType|
//@[046:047) |   | └─Token(RightParen) |)|
//@[048:054) |   ├─VariableAccessSyntax
//@[048:054) |   | └─IdentifierSyntax
//@[048:054) |   |   └─Token(Identifier) |string|
//@[055:057) |   ├─Token(Arrow) |=>|
//@[058:069) |   └─StringSyntax
//@[058:061) |     ├─Token(StringLeftPiece) |'${|
//@[061:062) |     ├─VariableAccessSyntax
//@[061:062) |     | └─IdentifierSyntax
//@[061:062) |     |   └─Token(Identifier) |a|
//@[062:066) |     ├─Token(StringMiddlePiece) |}-${|
//@[066:067) |     ├─VariableAccessSyntax
//@[066:067) |     | └─IdentifierSyntax
//@[066:067) |     |   └─Token(Identifier) |b|
//@[067:069) |     └─Token(StringRightPiece) |}'|
//@[069:071) ├─Token(NewLine) |\n\n|

func noLambda('foo') string => ''
//@[000:033) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |noLambda|
//@[013:033) | └─TypedLambdaSyntax
//@[013:020) |   ├─TypedVariableBlockSyntax
//@[013:014) |   | ├─Token(LeftParen) |(|
//@[014:019) |   | ├─TypedLocalVariableSyntax
//@[014:014) |   | | ├─IdentifierSyntax
//@[014:014) |   | | | └─SkippedTriviaSyntax
//@[014:019) |   | | └─StringSyntax
//@[014:019) |   | |   └─Token(StringComplete) |'foo'|
//@[019:020) |   | └─Token(RightParen) |)|
//@[021:027) |   ├─VariableAccessSyntax
//@[021:027) |   | └─IdentifierSyntax
//@[021:027) |   |   └─Token(Identifier) |string|
//@[028:030) |   ├─Token(Arrow) |=>|
//@[031:033) |   └─StringSyntax
//@[031:033) |     └─Token(StringComplete) |''|
//@[033:035) ├─Token(NewLine) |\n\n|

func noLambda2 = (sdf 'foo') string => ''
//@[000:041) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |noLambda2|
//@[015:041) | └─SkippedTriviaSyntax
//@[015:016) |   ├─Token(Assignment) |=|
//@[017:018) |   ├─Token(LeftParen) |(|
//@[018:021) |   ├─Token(Identifier) |sdf|
//@[022:027) |   ├─Token(StringComplete) |'foo'|
//@[027:028) |   ├─Token(RightParen) |)|
//@[029:035) |   ├─Token(Identifier) |string|
//@[036:038) |   ├─Token(Arrow) |=>|
//@[039:041) |   └─Token(StringComplete) |''|
//@[041:043) ├─Token(NewLine) |\n\n|

func noLambda3 = string 'asdf'
//@[000:030) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |noLambda3|
//@[015:030) | └─SkippedTriviaSyntax
//@[015:016) |   ├─Token(Assignment) |=|
//@[017:023) |   ├─Token(Identifier) |string|
//@[024:030) |   └─Token(StringComplete) |'asdf'|
//@[030:032) ├─Token(NewLine) |\n\n|

func argLengthMismatch(a string, b string, c string) array => ([a, b, c])
//@[000:073) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:022) | ├─IdentifierSyntax
//@[005:022) | | └─Token(Identifier) |argLengthMismatch|
//@[022:073) | └─TypedLambdaSyntax
//@[022:052) |   ├─TypedVariableBlockSyntax
//@[022:023) |   | ├─Token(LeftParen) |(|
//@[023:031) |   | ├─TypedLocalVariableSyntax
//@[023:024) |   | | ├─IdentifierSyntax
//@[023:024) |   | | | └─Token(Identifier) |a|
//@[025:031) |   | | └─VariableAccessSyntax
//@[025:031) |   | |   └─IdentifierSyntax
//@[025:031) |   | |     └─Token(Identifier) |string|
//@[031:032) |   | ├─Token(Comma) |,|
//@[033:041) |   | ├─TypedLocalVariableSyntax
//@[033:034) |   | | ├─IdentifierSyntax
//@[033:034) |   | | | └─Token(Identifier) |b|
//@[035:041) |   | | └─VariableAccessSyntax
//@[035:041) |   | |   └─IdentifierSyntax
//@[035:041) |   | |     └─Token(Identifier) |string|
//@[041:042) |   | ├─Token(Comma) |,|
//@[043:051) |   | ├─TypedLocalVariableSyntax
//@[043:044) |   | | ├─IdentifierSyntax
//@[043:044) |   | | | └─Token(Identifier) |c|
//@[045:051) |   | | └─VariableAccessSyntax
//@[045:051) |   | |   └─IdentifierSyntax
//@[045:051) |   | |     └─Token(Identifier) |string|
//@[051:052) |   | └─Token(RightParen) |)|
//@[053:058) |   ├─VariableAccessSyntax
//@[053:058) |   | └─IdentifierSyntax
//@[053:058) |   |   └─Token(Identifier) |array|
//@[059:061) |   ├─Token(Arrow) |=>|
//@[062:073) |   └─ParenthesizedExpressionSyntax
//@[062:063) |     ├─Token(LeftParen) |(|
//@[063:072) |     ├─ArraySyntax
//@[063:064) |     | ├─Token(LeftSquare) |[|
//@[064:065) |     | ├─ArrayItemSyntax
//@[064:065) |     | | └─VariableAccessSyntax
//@[064:065) |     | |   └─IdentifierSyntax
//@[064:065) |     | |     └─Token(Identifier) |a|
//@[065:066) |     | ├─Token(Comma) |,|
//@[067:068) |     | ├─ArrayItemSyntax
//@[067:068) |     | | └─VariableAccessSyntax
//@[067:068) |     | |   └─IdentifierSyntax
//@[067:068) |     | |     └─Token(Identifier) |b|
//@[068:069) |     | ├─Token(Comma) |,|
//@[070:071) |     | ├─ArrayItemSyntax
//@[070:071) |     | | └─VariableAccessSyntax
//@[070:071) |     | |   └─IdentifierSyntax
//@[070:071) |     | |     └─Token(Identifier) |c|
//@[071:072) |     | └─Token(RightSquare) |]|
//@[072:073) |     └─Token(RightParen) |)|
//@[073:074) ├─Token(NewLine) |\n|
var sdf = argLengthMismatch('asdf')
//@[000:035) ├─VariableDeclarationSyntax
//@[000:003) | ├─Token(Identifier) |var|
//@[004:007) | ├─IdentifierSyntax
//@[004:007) | | └─Token(Identifier) |sdf|
//@[008:009) | ├─Token(Assignment) |=|
//@[010:035) | └─FunctionCallSyntax
//@[010:027) |   ├─IdentifierSyntax
//@[010:027) |   | └─Token(Identifier) |argLengthMismatch|
//@[027:028) |   ├─Token(LeftParen) |(|
//@[028:034) |   ├─FunctionArgumentSyntax
//@[028:034) |   | └─StringSyntax
//@[028:034) |   |   └─Token(StringComplete) |'asdf'|
//@[034:035) |   └─Token(RightParen) |)|
//@[035:037) ├─Token(NewLine) |\n\n|

var asdfwdf = noLambda('asd')
//@[000:029) ├─VariableDeclarationSyntax
//@[000:003) | ├─Token(Identifier) |var|
//@[004:011) | ├─IdentifierSyntax
//@[004:011) | | └─Token(Identifier) |asdfwdf|
//@[012:013) | ├─Token(Assignment) |=|
//@[014:029) | └─FunctionCallSyntax
//@[014:022) |   ├─IdentifierSyntax
//@[014:022) |   | └─Token(Identifier) |noLambda|
//@[022:023) |   ├─Token(LeftParen) |(|
//@[023:028) |   ├─FunctionArgumentSyntax
//@[023:028) |   | └─StringSyntax
//@[023:028) |   |   └─Token(StringComplete) |'asd'|
//@[028:029) |   └─Token(RightParen) |)|
//@[029:031) ├─Token(NewLine) |\n\n|

func sayHello(name string) string => 'Hi ${name}!'
//@[000:050) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |sayHello|
//@[013:050) | └─TypedLambdaSyntax
//@[013:026) |   ├─TypedVariableBlockSyntax
//@[013:014) |   | ├─Token(LeftParen) |(|
//@[014:025) |   | ├─TypedLocalVariableSyntax
//@[014:018) |   | | ├─IdentifierSyntax
//@[014:018) |   | | | └─Token(Identifier) |name|
//@[019:025) |   | | └─VariableAccessSyntax
//@[019:025) |   | |   └─IdentifierSyntax
//@[019:025) |   | |     └─Token(Identifier) |string|
//@[025:026) |   | └─Token(RightParen) |)|
//@[027:033) |   ├─VariableAccessSyntax
//@[027:033) |   | └─IdentifierSyntax
//@[027:033) |   |   └─Token(Identifier) |string|
//@[034:036) |   ├─Token(Arrow) |=>|
//@[037:050) |   └─StringSyntax
//@[037:043) |     ├─Token(StringLeftPiece) |'Hi ${|
//@[043:047) |     ├─VariableAccessSyntax
//@[043:047) |     | └─IdentifierSyntax
//@[043:047) |     |   └─Token(Identifier) |name|
//@[047:050) |     └─Token(StringRightPiece) |}!'|
//@[050:051) ├─Token(NewLine) |\n|
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[000:055) ├─OutputDeclarationSyntax
//@[000:006) | ├─Token(Identifier) |output|
//@[007:013) | ├─IdentifierSyntax
//@[007:013) | | └─Token(Identifier) |hellos|
//@[014:019) | ├─VariableAccessSyntax
//@[014:019) | | └─IdentifierSyntax
//@[014:019) | |   └─Token(Identifier) |array|
//@[020:021) | ├─Token(Assignment) |=|
//@[022:055) | └─FunctionCallSyntax
//@[022:025) |   ├─IdentifierSyntax
//@[022:025) |   | └─Token(Identifier) |map|
//@[025:026) |   ├─Token(LeftParen) |(|
//@[026:044) |   ├─FunctionArgumentSyntax
//@[026:044) |   | └─ArraySyntax
//@[026:027) |   |   ├─Token(LeftSquare) |[|
//@[027:033) |   |   ├─ArrayItemSyntax
//@[027:033) |   |   | └─StringSyntax
//@[027:033) |   |   |   └─Token(StringComplete) |'Evie'|
//@[033:034) |   |   ├─Token(Comma) |,|
//@[035:043) |   |   ├─ArrayItemSyntax
//@[035:043) |   |   | └─StringSyntax
//@[035:043) |   |   |   └─Token(StringComplete) |'Casper'|
//@[043:044) |   |   └─Token(RightSquare) |]|
//@[044:045) |   ├─Token(Comma) |,|
//@[046:054) |   ├─FunctionArgumentSyntax
//@[046:054) |   | └─VariableAccessSyntax
//@[046:054) |   |   └─IdentifierSyntax
//@[046:054) |   |     └─Token(Identifier) |sayHello|
//@[054:055) |   └─Token(RightParen) |)|
//@[113:115) ├─Token(NewLine) |\n\n|

func sayHelloBadNewlines(
//@[000:025) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:024) | ├─IdentifierSyntax
//@[005:024) | | └─Token(Identifier) |sayHelloBadNewlines|
//@[024:025) | └─TypedLambdaSyntax
//@[024:025) |   ├─TypedVariableBlockSyntax
//@[024:025) |   | ├─Token(LeftParen) |(|
//@[025:025) |   | ├─TypedLocalVariableSyntax
//@[025:025) |   | | ├─IdentifierSyntax
//@[025:025) |   | | | └─SkippedTriviaSyntax
//@[025:025) |   | | └─SkippedTriviaSyntax
//@[025:025) |   | └─SkippedTriviaSyntax
//@[025:025) |   ├─SkippedTriviaSyntax
//@[025:025) |   ├─SkippedTriviaSyntax
//@[025:025) |   └─SkippedTriviaSyntax
//@[025:026) ├─Token(NewLine) |\n|
  name string) string => 'Hi ${name}!'
//@[002:038) ├─SkippedTriviaSyntax
//@[002:006) | ├─Token(Identifier) |name|
//@[007:013) | ├─Token(Identifier) |string|
//@[013:014) | ├─Token(RightParen) |)|
//@[015:021) | ├─Token(Identifier) |string|
//@[022:024) | ├─Token(Arrow) |=>|
//@[025:031) | ├─Token(StringLeftPiece) |'Hi ${|
//@[031:035) | ├─Token(Identifier) |name|
//@[035:038) | └─Token(StringRightPiece) |}!'|
//@[038:039) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
