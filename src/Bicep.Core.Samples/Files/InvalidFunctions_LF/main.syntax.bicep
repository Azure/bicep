func useRuntimeFunction = () string => reference('foo').bar
//@[000:913) ProgramSyntax
//@[000:059) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:023) | ├─IdentifierSyntax
//@[005:023) | | └─Token(Identifier) |useRuntimeFunction|
//@[024:025) | ├─Token(Assignment) |=|
//@[026:059) | └─TypedLambdaSyntax
//@[026:028) |   ├─TypedVariableBlockSyntax
//@[026:027) |   | ├─Token(LeftParen) |(|
//@[027:028) |   | └─Token(RightParen) |)|
//@[029:035) |   ├─VariableAccessSyntax
//@[029:035) |   | └─IdentifierSyntax
//@[029:035) |   |   └─Token(Identifier) |string|
//@[036:038) |   ├─Token(Arrow) |=>|
//@[039:059) |   └─PropertyAccessSyntax
//@[039:055) |     ├─FunctionCallSyntax
//@[039:048) |     | ├─IdentifierSyntax
//@[039:048) |     | | └─Token(Identifier) |reference|
//@[048:049) |     | ├─Token(LeftParen) |(|
//@[049:054) |     | ├─FunctionArgumentSyntax
//@[049:054) |     | | └─StringSyntax
//@[049:054) |     | |   └─Token(StringComplete) |'foo'|
//@[054:055) |     | └─Token(RightParen) |)|
//@[055:056) |     ├─Token(Dot) |.|
//@[056:059) |     └─IdentifierSyntax
//@[056:059) |       └─Token(Identifier) |bar|
//@[059:061) ├─Token(NewLine) |\n\n|

func constFunc = () string => 'A'
//@[000:033) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |constFunc|
//@[015:016) | ├─Token(Assignment) |=|
//@[017:033) | └─TypedLambdaSyntax
//@[017:019) |   ├─TypedVariableBlockSyntax
//@[017:018) |   | ├─Token(LeftParen) |(|
//@[018:019) |   | └─Token(RightParen) |)|
//@[020:026) |   ├─VariableAccessSyntax
//@[020:026) |   | └─IdentifierSyntax
//@[020:026) |   |   └─Token(Identifier) |string|
//@[027:029) |   ├─Token(Arrow) |=>|
//@[030:033) |   └─StringSyntax
//@[030:033) |     └─Token(StringComplete) |'A'|
//@[033:034) ├─Token(NewLine) |\n|
func funcWithOtherFuncRef = () string => constFunc()
//@[000:052) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:025) | ├─IdentifierSyntax
//@[005:025) | | └─Token(Identifier) |funcWithOtherFuncRef|
//@[026:027) | ├─Token(Assignment) |=|
//@[028:052) | └─TypedLambdaSyntax
//@[028:030) |   ├─TypedVariableBlockSyntax
//@[028:029) |   | ├─Token(LeftParen) |(|
//@[029:030) |   | └─Token(RightParen) |)|
//@[031:037) |   ├─VariableAccessSyntax
//@[031:037) |   | └─IdentifierSyntax
//@[031:037) |   |   └─Token(Identifier) |string|
//@[038:040) |   ├─Token(Arrow) |=>|
//@[041:052) |   └─FunctionCallSyntax
//@[041:050) |     ├─IdentifierSyntax
//@[041:050) |     | └─Token(Identifier) |constFunc|
//@[050:051) |     ├─Token(LeftParen) |(|
//@[051:052) |     └─Token(RightParen) |)|
//@[052:054) ├─Token(NewLine) |\n\n|

func missingArgType = (input) string => input
//@[000:045) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:019) | ├─IdentifierSyntax
//@[005:019) | | └─Token(Identifier) |missingArgType|
//@[020:021) | ├─Token(Assignment) |=|
//@[022:045) | └─TypedLambdaSyntax
//@[022:029) |   ├─TypedVariableBlockSyntax
//@[022:023) |   | ├─Token(LeftParen) |(|
//@[023:028) |   | ├─TypedLocalVariableSyntax
//@[023:028) |   | | ├─IdentifierSyntax
//@[023:028) |   | | | └─Token(Identifier) |input|
//@[028:028) |   | | └─SkippedTriviaSyntax
//@[028:029) |   | └─Token(RightParen) |)|
//@[030:036) |   ├─VariableAccessSyntax
//@[030:036) |   | └─IdentifierSyntax
//@[030:036) |   |   └─Token(Identifier) |string|
//@[037:039) |   ├─Token(Arrow) |=>|
//@[040:045) |   └─VariableAccessSyntax
//@[040:045) |     └─IdentifierSyntax
//@[040:045) |       └─Token(Identifier) |input|
//@[045:047) ├─Token(NewLine) |\n\n|

func missingOutputType = (input string) => input
//@[000:048) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:022) | ├─IdentifierSyntax
//@[005:022) | | └─Token(Identifier) |missingOutputType|
//@[023:024) | ├─Token(Assignment) |=|
//@[025:048) | └─TypedLambdaSyntax
//@[025:039) |   ├─TypedVariableBlockSyntax
//@[025:026) |   | ├─Token(LeftParen) |(|
//@[026:038) |   | ├─TypedLocalVariableSyntax
//@[026:031) |   | | ├─IdentifierSyntax
//@[026:031) |   | | | └─Token(Identifier) |input|
//@[032:038) |   | | └─VariableAccessSyntax
//@[032:038) |   | |   └─IdentifierSyntax
//@[032:038) |   | |     └─Token(Identifier) |string|
//@[038:039) |   | └─Token(RightParen) |)|
//@[040:048) |   ├─SkippedTriviaSyntax
//@[040:042) |   | ├─Token(Arrow) |=>|
//@[043:048) |   | └─Token(Identifier) |input|
//@[048:048) |   ├─SkippedTriviaSyntax
//@[048:048) |   └─SkippedTriviaSyntax
//@[048:050) ├─Token(NewLine) |\n\n|

func invalidType = (input string) string => input
//@[000:049) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:016) | ├─IdentifierSyntax
//@[005:016) | | └─Token(Identifier) |invalidType|
//@[017:018) | ├─Token(Assignment) |=|
//@[019:049) | └─TypedLambdaSyntax
//@[019:033) |   ├─TypedVariableBlockSyntax
//@[019:020) |   | ├─Token(LeftParen) |(|
//@[020:032) |   | ├─TypedLocalVariableSyntax
//@[020:025) |   | | ├─IdentifierSyntax
//@[020:025) |   | | | └─Token(Identifier) |input|
//@[026:032) |   | | └─VariableAccessSyntax
//@[026:032) |   | |   └─IdentifierSyntax
//@[026:032) |   | |     └─Token(Identifier) |string|
//@[032:033) |   | └─Token(RightParen) |)|
//@[034:040) |   ├─VariableAccessSyntax
//@[034:040) |   | └─IdentifierSyntax
//@[034:040) |   |   └─Token(Identifier) |string|
//@[041:043) |   ├─Token(Arrow) |=>|
//@[044:049) |   └─VariableAccessSyntax
//@[044:049) |     └─IdentifierSyntax
//@[044:049) |       └─Token(Identifier) |input|
//@[049:051) ├─Token(NewLine) |\n\n|

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

func madeUpTypeArgs = (a notAType, b alsoNotAType) string => '${a}-${b}'
//@[000:072) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:019) | ├─IdentifierSyntax
//@[005:019) | | └─Token(Identifier) |madeUpTypeArgs|
//@[020:021) | ├─Token(Assignment) |=|
//@[022:072) | └─TypedLambdaSyntax
//@[022:050) |   ├─TypedVariableBlockSyntax
//@[022:023) |   | ├─Token(LeftParen) |(|
//@[023:033) |   | ├─TypedLocalVariableSyntax
//@[023:024) |   | | ├─IdentifierSyntax
//@[023:024) |   | | | └─Token(Identifier) |a|
//@[025:033) |   | | └─VariableAccessSyntax
//@[025:033) |   | |   └─IdentifierSyntax
//@[025:033) |   | |     └─Token(Identifier) |notAType|
//@[033:034) |   | ├─Token(Comma) |,|
//@[035:049) |   | ├─TypedLocalVariableSyntax
//@[035:036) |   | | ├─IdentifierSyntax
//@[035:036) |   | | | └─Token(Identifier) |b|
//@[037:049) |   | | └─VariableAccessSyntax
//@[037:049) |   | |   └─IdentifierSyntax
//@[037:049) |   | |     └─Token(Identifier) |alsoNotAType|
//@[049:050) |   | └─Token(RightParen) |)|
//@[051:057) |   ├─VariableAccessSyntax
//@[051:057) |   | └─IdentifierSyntax
//@[051:057) |   |   └─Token(Identifier) |string|
//@[058:060) |   ├─Token(Arrow) |=>|
//@[061:072) |   └─StringSyntax
//@[061:064) |     ├─Token(StringLeftPiece) |'${|
//@[064:065) |     ├─VariableAccessSyntax
//@[064:065) |     | └─IdentifierSyntax
//@[064:065) |     |   └─Token(Identifier) |a|
//@[065:069) |     ├─Token(StringMiddlePiece) |}-${|
//@[069:070) |     ├─VariableAccessSyntax
//@[069:070) |     | └─IdentifierSyntax
//@[069:070) |     |   └─Token(Identifier) |b|
//@[070:072) |     └─Token(StringRightPiece) |}'|
//@[072:074) ├─Token(NewLine) |\n\n|

func noLambda = ('foo') string => ''
//@[000:036) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |noLambda|
//@[014:015) | ├─Token(Assignment) |=|
//@[016:036) | └─TypedLambdaSyntax
//@[016:023) |   ├─TypedVariableBlockSyntax
//@[016:017) |   | ├─Token(LeftParen) |(|
//@[017:022) |   | ├─TypedLocalVariableSyntax
//@[017:017) |   | | ├─IdentifierSyntax
//@[017:017) |   | | | └─SkippedTriviaSyntax
//@[017:022) |   | | └─StringSyntax
//@[017:022) |   | |   └─Token(StringComplete) |'foo'|
//@[022:023) |   | └─Token(RightParen) |)|
//@[024:030) |   ├─VariableAccessSyntax
//@[024:030) |   | └─IdentifierSyntax
//@[024:030) |   |   └─Token(Identifier) |string|
//@[031:033) |   ├─Token(Arrow) |=>|
//@[034:036) |   └─StringSyntax
//@[034:036) |     └─Token(StringComplete) |''|
//@[036:038) ├─Token(NewLine) |\n\n|

func noLambda2 = (sdf 'foo') string => ''
//@[000:041) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |noLambda2|
//@[015:016) | ├─Token(Assignment) |=|
//@[017:041) | └─TypedLambdaSyntax
//@[017:028) |   ├─TypedVariableBlockSyntax
//@[017:018) |   | ├─Token(LeftParen) |(|
//@[018:027) |   | ├─TypedLocalVariableSyntax
//@[018:021) |   | | ├─IdentifierSyntax
//@[018:021) |   | | | └─Token(Identifier) |sdf|
//@[022:027) |   | | └─StringSyntax
//@[022:027) |   | |   └─Token(StringComplete) |'foo'|
//@[027:028) |   | └─Token(RightParen) |)|
//@[029:035) |   ├─VariableAccessSyntax
//@[029:035) |   | └─IdentifierSyntax
//@[029:035) |   |   └─Token(Identifier) |string|
//@[036:038) |   ├─Token(Arrow) |=>|
//@[039:041) |   └─StringSyntax
//@[039:041) |     └─Token(StringComplete) |''|
//@[041:043) ├─Token(NewLine) |\n\n|

func noLambda3 = string 'asdf'
//@[000:030) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |noLambda3|
//@[015:016) | ├─Token(Assignment) |=|
//@[017:030) | └─SkippedTriviaSyntax
//@[017:023) |   ├─Token(Identifier) |string|
//@[024:030) |   └─Token(StringComplete) |'asdf'|
//@[030:032) ├─Token(NewLine) |\n\n|

func argLengthMismatch = (a string, b string, c string) array => ([a, b, c])
//@[000:076) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:022) | ├─IdentifierSyntax
//@[005:022) | | └─Token(Identifier) |argLengthMismatch|
//@[023:024) | ├─Token(Assignment) |=|
//@[025:076) | └─TypedLambdaSyntax
//@[025:055) |   ├─TypedVariableBlockSyntax
//@[025:026) |   | ├─Token(LeftParen) |(|
//@[026:034) |   | ├─TypedLocalVariableSyntax
//@[026:027) |   | | ├─IdentifierSyntax
//@[026:027) |   | | | └─Token(Identifier) |a|
//@[028:034) |   | | └─VariableAccessSyntax
//@[028:034) |   | |   └─IdentifierSyntax
//@[028:034) |   | |     └─Token(Identifier) |string|
//@[034:035) |   | ├─Token(Comma) |,|
//@[036:044) |   | ├─TypedLocalVariableSyntax
//@[036:037) |   | | ├─IdentifierSyntax
//@[036:037) |   | | | └─Token(Identifier) |b|
//@[038:044) |   | | └─VariableAccessSyntax
//@[038:044) |   | |   └─IdentifierSyntax
//@[038:044) |   | |     └─Token(Identifier) |string|
//@[044:045) |   | ├─Token(Comma) |,|
//@[046:054) |   | ├─TypedLocalVariableSyntax
//@[046:047) |   | | ├─IdentifierSyntax
//@[046:047) |   | | | └─Token(Identifier) |c|
//@[048:054) |   | | └─VariableAccessSyntax
//@[048:054) |   | |   └─IdentifierSyntax
//@[048:054) |   | |     └─Token(Identifier) |string|
//@[054:055) |   | └─Token(RightParen) |)|
//@[056:061) |   ├─VariableAccessSyntax
//@[056:061) |   | └─IdentifierSyntax
//@[056:061) |   |   └─Token(Identifier) |array|
//@[062:064) |   ├─Token(Arrow) |=>|
//@[065:076) |   └─ParenthesizedExpressionSyntax
//@[065:066) |     ├─Token(LeftParen) |(|
//@[066:075) |     ├─ArraySyntax
//@[066:067) |     | ├─Token(LeftSquare) |[|
//@[067:068) |     | ├─ArrayItemSyntax
//@[067:068) |     | | └─VariableAccessSyntax
//@[067:068) |     | |   └─IdentifierSyntax
//@[067:068) |     | |     └─Token(Identifier) |a|
//@[068:069) |     | ├─Token(Comma) |,|
//@[070:071) |     | ├─ArrayItemSyntax
//@[070:071) |     | | └─VariableAccessSyntax
//@[070:071) |     | |   └─IdentifierSyntax
//@[070:071) |     | |     └─Token(Identifier) |b|
//@[071:072) |     | ├─Token(Comma) |,|
//@[073:074) |     | ├─ArrayItemSyntax
//@[073:074) |     | | └─VariableAccessSyntax
//@[073:074) |     | |   └─IdentifierSyntax
//@[073:074) |     | |     └─Token(Identifier) |c|
//@[074:075) |     | └─Token(RightSquare) |]|
//@[075:076) |     └─Token(RightParen) |)|
//@[076:077) ├─Token(NewLine) |\n|
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

func sayHello = (name string) string => 'Hi ${name}!'
//@[000:053) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |sayHello|
//@[014:015) | ├─Token(Assignment) |=|
//@[016:053) | └─TypedLambdaSyntax
//@[016:029) |   ├─TypedVariableBlockSyntax
//@[016:017) |   | ├─Token(LeftParen) |(|
//@[017:028) |   | ├─TypedLocalVariableSyntax
//@[017:021) |   | | ├─IdentifierSyntax
//@[017:021) |   | | | └─Token(Identifier) |name|
//@[022:028) |   | | └─VariableAccessSyntax
//@[022:028) |   | |   └─IdentifierSyntax
//@[022:028) |   | |     └─Token(Identifier) |string|
//@[028:029) |   | └─Token(RightParen) |)|
//@[030:036) |   ├─VariableAccessSyntax
//@[030:036) |   | └─IdentifierSyntax
//@[030:036) |   |   └─Token(Identifier) |string|
//@[037:039) |   ├─Token(Arrow) |=>|
//@[040:053) |   └─StringSyntax
//@[040:046) |     ├─Token(StringLeftPiece) |'Hi ${|
//@[046:050) |     ├─VariableAccessSyntax
//@[046:050) |     | └─IdentifierSyntax
//@[046:050) |     |   └─Token(Identifier) |name|
//@[050:053) |     └─Token(StringRightPiece) |}!'|
//@[053:054) ├─Token(NewLine) |\n|
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

func sayHelloBadNewlines = (
//@[000:028) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:024) | ├─IdentifierSyntax
//@[005:024) | | └─Token(Identifier) |sayHelloBadNewlines|
//@[025:026) | ├─Token(Assignment) |=|
//@[027:028) | └─TypedLambdaSyntax
//@[027:028) |   ├─TypedVariableBlockSyntax
//@[027:028) |   | ├─Token(LeftParen) |(|
//@[028:028) |   | ├─TypedLocalVariableSyntax
//@[028:028) |   | | ├─IdentifierSyntax
//@[028:028) |   | | | └─SkippedTriviaSyntax
//@[028:028) |   | | └─SkippedTriviaSyntax
//@[028:028) |   | └─SkippedTriviaSyntax
//@[028:028) |   ├─SkippedTriviaSyntax
//@[028:028) |   ├─SkippedTriviaSyntax
//@[028:028) |   └─SkippedTriviaSyntax
//@[028:029) ├─Token(NewLine) |\n|
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
