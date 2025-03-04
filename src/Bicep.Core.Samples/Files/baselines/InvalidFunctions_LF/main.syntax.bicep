func useRuntimeFunction() string => reference('foo').bar
//@[000:1360) ProgramSyntax
//@[000:0056) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0023) | ├─IdentifierSyntax
//@[005:0023) | | └─Token(Identifier) |useRuntimeFunction|
//@[023:0056) | └─TypedLambdaSyntax
//@[023:0025) |   ├─TypedVariableBlockSyntax
//@[023:0024) |   | ├─Token(LeftParen) |(|
//@[024:0025) |   | └─Token(RightParen) |)|
//@[026:0032) |   ├─TypeVariableAccessSyntax
//@[026:0032) |   | └─IdentifierSyntax
//@[026:0032) |   |   └─Token(Identifier) |string|
//@[033:0035) |   ├─Token(Arrow) |=>|
//@[036:0056) |   └─PropertyAccessSyntax
//@[036:0052) |     ├─FunctionCallSyntax
//@[036:0045) |     | ├─IdentifierSyntax
//@[036:0045) |     | | └─Token(Identifier) |reference|
//@[045:0046) |     | ├─Token(LeftParen) |(|
//@[046:0051) |     | ├─FunctionArgumentSyntax
//@[046:0051) |     | | └─StringSyntax
//@[046:0051) |     | |   └─Token(StringComplete) |'foo'|
//@[051:0052) |     | └─Token(RightParen) |)|
//@[052:0053) |     ├─Token(Dot) |.|
//@[053:0056) |     └─IdentifierSyntax
//@[053:0056) |       └─Token(Identifier) |bar|
//@[056:0058) ├─Token(NewLine) |\n\n|

func missingArgType(input) string => input
//@[000:0042) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0019) | ├─IdentifierSyntax
//@[005:0019) | | └─Token(Identifier) |missingArgType|
//@[019:0042) | └─TypedLambdaSyntax
//@[019:0026) |   ├─TypedVariableBlockSyntax
//@[019:0020) |   | ├─Token(LeftParen) |(|
//@[020:0025) |   | ├─TypedLocalVariableSyntax
//@[020:0025) |   | | ├─IdentifierSyntax
//@[020:0025) |   | | | └─Token(Identifier) |input|
//@[025:0025) |   | | └─SkippedTriviaSyntax
//@[025:0026) |   | └─Token(RightParen) |)|
//@[027:0033) |   ├─TypeVariableAccessSyntax
//@[027:0033) |   | └─IdentifierSyntax
//@[027:0033) |   |   └─Token(Identifier) |string|
//@[034:0036) |   ├─Token(Arrow) |=>|
//@[037:0042) |   └─VariableAccessSyntax
//@[037:0042) |     └─IdentifierSyntax
//@[037:0042) |       └─Token(Identifier) |input|
//@[042:0044) ├─Token(NewLine) |\n\n|

func missingOutputType(input string) => input
//@[000:0045) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0022) | ├─IdentifierSyntax
//@[005:0022) | | └─Token(Identifier) |missingOutputType|
//@[022:0045) | └─TypedLambdaSyntax
//@[022:0036) |   ├─TypedVariableBlockSyntax
//@[022:0023) |   | ├─Token(LeftParen) |(|
//@[023:0035) |   | ├─TypedLocalVariableSyntax
//@[023:0028) |   | | ├─IdentifierSyntax
//@[023:0028) |   | | | └─Token(Identifier) |input|
//@[029:0035) |   | | └─TypeVariableAccessSyntax
//@[029:0035) |   | |   └─IdentifierSyntax
//@[029:0035) |   | |     └─Token(Identifier) |string|
//@[035:0036) |   | └─Token(RightParen) |)|
//@[037:0045) |   ├─SkippedTriviaSyntax
//@[037:0039) |   | ├─Token(Arrow) |=>|
//@[040:0045) |   | └─Token(Identifier) |input|
//@[045:0045) |   ├─SkippedTriviaSyntax
//@[045:0045) |   └─SkippedTriviaSyntax
//@[045:0047) ├─Token(NewLine) |\n\n|

func invalidType(input string) string => input
//@[000:0046) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0016) | ├─IdentifierSyntax
//@[005:0016) | | └─Token(Identifier) |invalidType|
//@[016:0046) | └─TypedLambdaSyntax
//@[016:0030) |   ├─TypedVariableBlockSyntax
//@[016:0017) |   | ├─Token(LeftParen) |(|
//@[017:0029) |   | ├─TypedLocalVariableSyntax
//@[017:0022) |   | | ├─IdentifierSyntax
//@[017:0022) |   | | | └─Token(Identifier) |input|
//@[023:0029) |   | | └─TypeVariableAccessSyntax
//@[023:0029) |   | |   └─IdentifierSyntax
//@[023:0029) |   | |     └─Token(Identifier) |string|
//@[029:0030) |   | └─Token(RightParen) |)|
//@[031:0037) |   ├─TypeVariableAccessSyntax
//@[031:0037) |   | └─IdentifierSyntax
//@[031:0037) |   |   └─Token(Identifier) |string|
//@[038:0040) |   ├─Token(Arrow) |=>|
//@[041:0046) |   └─VariableAccessSyntax
//@[041:0046) |     └─IdentifierSyntax
//@[041:0046) |       └─Token(Identifier) |input|
//@[046:0048) ├─Token(NewLine) |\n\n|

output invalidType string = invalidType(true)
//@[000:0045) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0018) | ├─IdentifierSyntax
//@[007:0018) | | └─Token(Identifier) |invalidType|
//@[019:0025) | ├─TypeVariableAccessSyntax
//@[019:0025) | | └─IdentifierSyntax
//@[019:0025) | |   └─Token(Identifier) |string|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0045) | └─FunctionCallSyntax
//@[028:0039) |   ├─IdentifierSyntax
//@[028:0039) |   | └─Token(Identifier) |invalidType|
//@[039:0040) |   ├─Token(LeftParen) |(|
//@[040:0044) |   ├─FunctionArgumentSyntax
//@[040:0044) |   | └─BooleanLiteralSyntax
//@[040:0044) |   |   └─Token(TrueKeyword) |true|
//@[044:0045) |   └─Token(RightParen) |)|
//@[045:0047) ├─Token(NewLine) |\n\n|

func madeUpTypeArgs(a notAType, b alsoNotAType) string => '${a}-${b}'
//@[000:0069) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0019) | ├─IdentifierSyntax
//@[005:0019) | | └─Token(Identifier) |madeUpTypeArgs|
//@[019:0069) | └─TypedLambdaSyntax
//@[019:0047) |   ├─TypedVariableBlockSyntax
//@[019:0020) |   | ├─Token(LeftParen) |(|
//@[020:0030) |   | ├─TypedLocalVariableSyntax
//@[020:0021) |   | | ├─IdentifierSyntax
//@[020:0021) |   | | | └─Token(Identifier) |a|
//@[022:0030) |   | | └─TypeVariableAccessSyntax
//@[022:0030) |   | |   └─IdentifierSyntax
//@[022:0030) |   | |     └─Token(Identifier) |notAType|
//@[030:0031) |   | ├─Token(Comma) |,|
//@[032:0046) |   | ├─TypedLocalVariableSyntax
//@[032:0033) |   | | ├─IdentifierSyntax
//@[032:0033) |   | | | └─Token(Identifier) |b|
//@[034:0046) |   | | └─TypeVariableAccessSyntax
//@[034:0046) |   | |   └─IdentifierSyntax
//@[034:0046) |   | |     └─Token(Identifier) |alsoNotAType|
//@[046:0047) |   | └─Token(RightParen) |)|
//@[048:0054) |   ├─TypeVariableAccessSyntax
//@[048:0054) |   | └─IdentifierSyntax
//@[048:0054) |   |   └─Token(Identifier) |string|
//@[055:0057) |   ├─Token(Arrow) |=>|
//@[058:0069) |   └─StringSyntax
//@[058:0061) |     ├─Token(StringLeftPiece) |'${|
//@[061:0062) |     ├─VariableAccessSyntax
//@[061:0062) |     | └─IdentifierSyntax
//@[061:0062) |     |   └─Token(Identifier) |a|
//@[062:0066) |     ├─Token(StringMiddlePiece) |}-${|
//@[066:0067) |     ├─VariableAccessSyntax
//@[066:0067) |     | └─IdentifierSyntax
//@[066:0067) |     |   └─Token(Identifier) |b|
//@[067:0069) |     └─Token(StringRightPiece) |}'|
//@[069:0071) ├─Token(NewLine) |\n\n|

func noLambda('foo') string => ''
//@[000:0033) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |noLambda|
//@[013:0033) | └─TypedLambdaSyntax
//@[013:0020) |   ├─TypedVariableBlockSyntax
//@[013:0014) |   | ├─Token(LeftParen) |(|
//@[014:0019) |   | ├─TypedLocalVariableSyntax
//@[014:0014) |   | | ├─IdentifierSyntax
//@[014:0014) |   | | | └─SkippedTriviaSyntax
//@[014:0019) |   | | └─StringTypeLiteralSyntax
//@[014:0019) |   | |   └─Token(StringComplete) |'foo'|
//@[019:0020) |   | └─Token(RightParen) |)|
//@[021:0027) |   ├─TypeVariableAccessSyntax
//@[021:0027) |   | └─IdentifierSyntax
//@[021:0027) |   |   └─Token(Identifier) |string|
//@[028:0030) |   ├─Token(Arrow) |=>|
//@[031:0033) |   └─StringSyntax
//@[031:0033) |     └─Token(StringComplete) |''|
//@[033:0035) ├─Token(NewLine) |\n\n|

func noLambda2 = (sdf 'foo') string => ''
//@[000:0041) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |noLambda2|
//@[015:0041) | └─SkippedTriviaSyntax
//@[015:0016) |   ├─Token(Assignment) |=|
//@[017:0018) |   ├─Token(LeftParen) |(|
//@[018:0021) |   ├─Token(Identifier) |sdf|
//@[022:0027) |   ├─Token(StringComplete) |'foo'|
//@[027:0028) |   ├─Token(RightParen) |)|
//@[029:0035) |   ├─Token(Identifier) |string|
//@[036:0038) |   ├─Token(Arrow) |=>|
//@[039:0041) |   └─Token(StringComplete) |''|
//@[041:0043) ├─Token(NewLine) |\n\n|

func noLambda3 = string 'asdf'
//@[000:0030) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |noLambda3|
//@[015:0030) | └─SkippedTriviaSyntax
//@[015:0016) |   ├─Token(Assignment) |=|
//@[017:0023) |   ├─Token(Identifier) |string|
//@[024:0030) |   └─Token(StringComplete) |'asdf'|
//@[030:0032) ├─Token(NewLine) |\n\n|

func argLengthMismatch(a string, b string, c string) array => ([a, b, c])
//@[000:0073) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0022) | ├─IdentifierSyntax
//@[005:0022) | | └─Token(Identifier) |argLengthMismatch|
//@[022:0073) | └─TypedLambdaSyntax
//@[022:0052) |   ├─TypedVariableBlockSyntax
//@[022:0023) |   | ├─Token(LeftParen) |(|
//@[023:0031) |   | ├─TypedLocalVariableSyntax
//@[023:0024) |   | | ├─IdentifierSyntax
//@[023:0024) |   | | | └─Token(Identifier) |a|
//@[025:0031) |   | | └─TypeVariableAccessSyntax
//@[025:0031) |   | |   └─IdentifierSyntax
//@[025:0031) |   | |     └─Token(Identifier) |string|
//@[031:0032) |   | ├─Token(Comma) |,|
//@[033:0041) |   | ├─TypedLocalVariableSyntax
//@[033:0034) |   | | ├─IdentifierSyntax
//@[033:0034) |   | | | └─Token(Identifier) |b|
//@[035:0041) |   | | └─TypeVariableAccessSyntax
//@[035:0041) |   | |   └─IdentifierSyntax
//@[035:0041) |   | |     └─Token(Identifier) |string|
//@[041:0042) |   | ├─Token(Comma) |,|
//@[043:0051) |   | ├─TypedLocalVariableSyntax
//@[043:0044) |   | | ├─IdentifierSyntax
//@[043:0044) |   | | | └─Token(Identifier) |c|
//@[045:0051) |   | | └─TypeVariableAccessSyntax
//@[045:0051) |   | |   └─IdentifierSyntax
//@[045:0051) |   | |     └─Token(Identifier) |string|
//@[051:0052) |   | └─Token(RightParen) |)|
//@[053:0058) |   ├─TypeVariableAccessSyntax
//@[053:0058) |   | └─IdentifierSyntax
//@[053:0058) |   |   └─Token(Identifier) |array|
//@[059:0061) |   ├─Token(Arrow) |=>|
//@[062:0073) |   └─ParenthesizedExpressionSyntax
//@[062:0063) |     ├─Token(LeftParen) |(|
//@[063:0072) |     ├─ArraySyntax
//@[063:0064) |     | ├─Token(LeftSquare) |[|
//@[064:0065) |     | ├─ArrayItemSyntax
//@[064:0065) |     | | └─VariableAccessSyntax
//@[064:0065) |     | |   └─IdentifierSyntax
//@[064:0065) |     | |     └─Token(Identifier) |a|
//@[065:0066) |     | ├─Token(Comma) |,|
//@[067:0068) |     | ├─ArrayItemSyntax
//@[067:0068) |     | | └─VariableAccessSyntax
//@[067:0068) |     | |   └─IdentifierSyntax
//@[067:0068) |     | |     └─Token(Identifier) |b|
//@[068:0069) |     | ├─Token(Comma) |,|
//@[070:0071) |     | ├─ArrayItemSyntax
//@[070:0071) |     | | └─VariableAccessSyntax
//@[070:0071) |     | |   └─IdentifierSyntax
//@[070:0071) |     | |     └─Token(Identifier) |c|
//@[071:0072) |     | └─Token(RightSquare) |]|
//@[072:0073) |     └─Token(RightParen) |)|
//@[073:0074) ├─Token(NewLine) |\n|
var sdf = argLengthMismatch('asdf')
//@[000:0035) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |sdf|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0035) | └─FunctionCallSyntax
//@[010:0027) |   ├─IdentifierSyntax
//@[010:0027) |   | └─Token(Identifier) |argLengthMismatch|
//@[027:0028) |   ├─Token(LeftParen) |(|
//@[028:0034) |   ├─FunctionArgumentSyntax
//@[028:0034) |   | └─StringSyntax
//@[028:0034) |   |   └─Token(StringComplete) |'asdf'|
//@[034:0035) |   └─Token(RightParen) |)|
//@[035:0037) ├─Token(NewLine) |\n\n|

var asdfwdf = noLambda('asd')
//@[000:0029) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |asdfwdf|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0029) | └─FunctionCallSyntax
//@[014:0022) |   ├─IdentifierSyntax
//@[014:0022) |   | └─Token(Identifier) |noLambda|
//@[022:0023) |   ├─Token(LeftParen) |(|
//@[023:0028) |   ├─FunctionArgumentSyntax
//@[023:0028) |   | └─StringSyntax
//@[023:0028) |   |   └─Token(StringComplete) |'asd'|
//@[028:0029) |   └─Token(RightParen) |)|
//@[029:0031) ├─Token(NewLine) |\n\n|

func sayHello(name string) string => 'Hi ${name}!'
//@[000:0050) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |sayHello|
//@[013:0050) | └─TypedLambdaSyntax
//@[013:0026) |   ├─TypedVariableBlockSyntax
//@[013:0014) |   | ├─Token(LeftParen) |(|
//@[014:0025) |   | ├─TypedLocalVariableSyntax
//@[014:0018) |   | | ├─IdentifierSyntax
//@[014:0018) |   | | | └─Token(Identifier) |name|
//@[019:0025) |   | | └─TypeVariableAccessSyntax
//@[019:0025) |   | |   └─IdentifierSyntax
//@[019:0025) |   | |     └─Token(Identifier) |string|
//@[025:0026) |   | └─Token(RightParen) |)|
//@[027:0033) |   ├─TypeVariableAccessSyntax
//@[027:0033) |   | └─IdentifierSyntax
//@[027:0033) |   |   └─Token(Identifier) |string|
//@[034:0036) |   ├─Token(Arrow) |=>|
//@[037:0050) |   └─StringSyntax
//@[037:0043) |     ├─Token(StringLeftPiece) |'Hi ${|
//@[043:0047) |     ├─VariableAccessSyntax
//@[043:0047) |     | └─IdentifierSyntax
//@[043:0047) |     |   └─Token(Identifier) |name|
//@[047:0050) |     └─Token(StringRightPiece) |}!'|
//@[050:0051) ├─Token(NewLine) |\n|
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[000:0055) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0013) | ├─IdentifierSyntax
//@[007:0013) | | └─Token(Identifier) |hellos|
//@[014:0019) | ├─TypeVariableAccessSyntax
//@[014:0019) | | └─IdentifierSyntax
//@[014:0019) | |   └─Token(Identifier) |array|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0055) | └─FunctionCallSyntax
//@[022:0025) |   ├─IdentifierSyntax
//@[022:0025) |   | └─Token(Identifier) |map|
//@[025:0026) |   ├─Token(LeftParen) |(|
//@[026:0044) |   ├─FunctionArgumentSyntax
//@[026:0044) |   | └─ArraySyntax
//@[026:0027) |   |   ├─Token(LeftSquare) |[|
//@[027:0033) |   |   ├─ArrayItemSyntax
//@[027:0033) |   |   | └─StringSyntax
//@[027:0033) |   |   |   └─Token(StringComplete) |'Evie'|
//@[033:0034) |   |   ├─Token(Comma) |,|
//@[035:0043) |   |   ├─ArrayItemSyntax
//@[035:0043) |   |   | └─StringSyntax
//@[035:0043) |   |   |   └─Token(StringComplete) |'Casper'|
//@[043:0044) |   |   └─Token(RightSquare) |]|
//@[044:0045) |   ├─Token(Comma) |,|
//@[046:0054) |   ├─FunctionArgumentSyntax
//@[046:0054) |   | └─VariableAccessSyntax
//@[046:0054) |   |   └─IdentifierSyntax
//@[046:0054) |   |     └─Token(Identifier) |sayHello|
//@[054:0055) |   └─Token(RightParen) |)|
//@[113:0115) ├─Token(NewLine) |\n\n|

func sayHelloBadNewlines(
//@[000:0064) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0024) | ├─IdentifierSyntax
//@[005:0024) | | └─Token(Identifier) |sayHelloBadNewlines|
//@[024:0064) | └─TypedLambdaSyntax
//@[024:0040) |   ├─TypedVariableBlockSyntax
//@[024:0025) |   | ├─Token(LeftParen) |(|
//@[025:0026) |   | ├─Token(NewLine) |\n|
  name string) string => 'Hi ${name}!'
//@[002:0013) |   | ├─TypedLocalVariableSyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[007:0013) |   | | └─TypeVariableAccessSyntax
//@[007:0013) |   | |   └─IdentifierSyntax
//@[007:0013) |   | |     └─Token(Identifier) |string|
//@[013:0014) |   | └─Token(RightParen) |)|
//@[015:0021) |   ├─TypeVariableAccessSyntax
//@[015:0021) |   | └─IdentifierSyntax
//@[015:0021) |   |   └─Token(Identifier) |string|
//@[022:0024) |   ├─Token(Arrow) |=>|
//@[025:0038) |   └─StringSyntax
//@[025:0031) |     ├─Token(StringLeftPiece) |'Hi ${|
//@[031:0035) |     ├─VariableAccessSyntax
//@[031:0035) |     | └─IdentifierSyntax
//@[031:0035) |     |   └─Token(Identifier) |name|
//@[035:0038) |     └─Token(StringRightPiece) |}!'|
//@[038:0040) ├─Token(NewLine) |\n\n|

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[000:0048) ├─TypeDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0028) | ├─IdentifierSyntax
//@[005:0028) | | └─Token(Identifier) |validStringLiteralUnion|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0048) | └─UnionTypeSyntax
//@[031:0036) |   ├─UnionTypeMemberSyntax
//@[031:0036) |   | └─StringTypeLiteralSyntax
//@[031:0036) |   |   └─Token(StringComplete) |'foo'|
//@[036:0037) |   ├─Token(Pipe) |||
//@[037:0042) |   ├─UnionTypeMemberSyntax
//@[037:0042) |   | └─StringTypeLiteralSyntax
//@[037:0042) |   |   └─Token(StringComplete) |'bar'|
//@[042:0043) |   ├─Token(Pipe) |||
//@[043:0048) |   └─UnionTypeMemberSyntax
//@[043:0048) |     └─StringTypeLiteralSyntax
//@[043:0048) |       └─Token(StringComplete) |'baz'|
//@[048:0049) ├─Token(NewLine) |\n|
func invalidArgs(a validStringLiteralUnion, b string) string => a
//@[000:0065) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0016) | ├─IdentifierSyntax
//@[005:0016) | | └─Token(Identifier) |invalidArgs|
//@[016:0065) | └─TypedLambdaSyntax
//@[016:0053) |   ├─TypedVariableBlockSyntax
//@[016:0017) |   | ├─Token(LeftParen) |(|
//@[017:0042) |   | ├─TypedLocalVariableSyntax
//@[017:0018) |   | | ├─IdentifierSyntax
//@[017:0018) |   | | | └─Token(Identifier) |a|
//@[019:0042) |   | | └─TypeVariableAccessSyntax
//@[019:0042) |   | |   └─IdentifierSyntax
//@[019:0042) |   | |     └─Token(Identifier) |validStringLiteralUnion|
//@[042:0043) |   | ├─Token(Comma) |,|
//@[044:0052) |   | ├─TypedLocalVariableSyntax
//@[044:0045) |   | | ├─IdentifierSyntax
//@[044:0045) |   | | | └─Token(Identifier) |b|
//@[046:0052) |   | | └─TypeVariableAccessSyntax
//@[046:0052) |   | |   └─IdentifierSyntax
//@[046:0052) |   | |     └─Token(Identifier) |string|
//@[052:0053) |   | └─Token(RightParen) |)|
//@[054:0060) |   ├─TypeVariableAccessSyntax
//@[054:0060) |   | └─IdentifierSyntax
//@[054:0060) |   |   └─Token(Identifier) |string|
//@[061:0063) |   ├─Token(Arrow) |=>|
//@[064:0065) |   └─VariableAccessSyntax
//@[064:0065) |     └─IdentifierSyntax
//@[064:0065) |       └─Token(Identifier) |a|
//@[065:0066) ├─Token(NewLine) |\n|
func invalidOutput() validStringLiteralUnion => 'foo'
//@[000:0053) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0018) | ├─IdentifierSyntax
//@[005:0018) | | └─Token(Identifier) |invalidOutput|
//@[018:0053) | └─TypedLambdaSyntax
//@[018:0020) |   ├─TypedVariableBlockSyntax
//@[018:0019) |   | ├─Token(LeftParen) |(|
//@[019:0020) |   | └─Token(RightParen) |)|
//@[021:0044) |   ├─TypeVariableAccessSyntax
//@[021:0044) |   | └─IdentifierSyntax
//@[021:0044) |   |   └─Token(Identifier) |validStringLiteralUnion|
//@[045:0047) |   ├─Token(Arrow) |=>|
//@[048:0053) |   └─StringSyntax
//@[048:0053) |     └─Token(StringComplete) |'foo'|
//@[053:0055) ├─Token(NewLine) |\n\n|

func recursive() string => recursive()
//@[000:0038) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |recursive|
//@[014:0038) | └─TypedLambdaSyntax
//@[014:0016) |   ├─TypedVariableBlockSyntax
//@[014:0015) |   | ├─Token(LeftParen) |(|
//@[015:0016) |   | └─Token(RightParen) |)|
//@[017:0023) |   ├─TypeVariableAccessSyntax
//@[017:0023) |   | └─IdentifierSyntax
//@[017:0023) |   |   └─Token(Identifier) |string|
//@[024:0026) |   ├─Token(Arrow) |=>|
//@[027:0038) |   └─FunctionCallSyntax
//@[027:0036) |     ├─IdentifierSyntax
//@[027:0036) |     | └─Token(Identifier) |recursive|
//@[036:0037) |     ├─Token(LeftParen) |(|
//@[037:0038) |     └─Token(RightParen) |)|
//@[038:0040) ├─Token(NewLine) |\n\n|

func recursiveA() string => recursiveB()
//@[000:0040) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0015) | ├─IdentifierSyntax
//@[005:0015) | | └─Token(Identifier) |recursiveA|
//@[015:0040) | └─TypedLambdaSyntax
//@[015:0017) |   ├─TypedVariableBlockSyntax
//@[015:0016) |   | ├─Token(LeftParen) |(|
//@[016:0017) |   | └─Token(RightParen) |)|
//@[018:0024) |   ├─TypeVariableAccessSyntax
//@[018:0024) |   | └─IdentifierSyntax
//@[018:0024) |   |   └─Token(Identifier) |string|
//@[025:0027) |   ├─Token(Arrow) |=>|
//@[028:0040) |   └─FunctionCallSyntax
//@[028:0038) |     ├─IdentifierSyntax
//@[028:0038) |     | └─Token(Identifier) |recursiveB|
//@[038:0039) |     ├─Token(LeftParen) |(|
//@[039:0040) |     └─Token(RightParen) |)|
//@[040:0041) ├─Token(NewLine) |\n|
func recursiveB() string => recursiveA()
//@[000:0040) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0015) | ├─IdentifierSyntax
//@[005:0015) | | └─Token(Identifier) |recursiveB|
//@[015:0040) | └─TypedLambdaSyntax
//@[015:0017) |   ├─TypedVariableBlockSyntax
//@[015:0016) |   | ├─Token(LeftParen) |(|
//@[016:0017) |   | └─Token(RightParen) |)|
//@[018:0024) |   ├─TypeVariableAccessSyntax
//@[018:0024) |   | └─IdentifierSyntax
//@[018:0024) |   |   └─Token(Identifier) |string|
//@[025:0027) |   ├─Token(Arrow) |=>|
//@[028:0040) |   └─FunctionCallSyntax
//@[028:0038) |     ├─IdentifierSyntax
//@[028:0038) |     | └─Token(Identifier) |recursiveA|
//@[038:0039) |     ├─Token(LeftParen) |(|
//@[039:0040) |     └─Token(RightParen) |)|
//@[040:0042) ├─Token(NewLine) |\n\n|

func onlyComma(,) string => 'foo'
//@[000:0033) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0014) | ├─IdentifierSyntax
//@[005:0014) | | └─Token(Identifier) |onlyComma|
//@[014:0033) | └─TypedLambdaSyntax
//@[014:0017) |   ├─TypedVariableBlockSyntax
//@[014:0015) |   | ├─Token(LeftParen) |(|
//@[015:0015) |   | ├─TypedLocalVariableSyntax
//@[015:0015) |   | | ├─IdentifierSyntax
//@[015:0015) |   | | | └─SkippedTriviaSyntax
//@[015:0015) |   | | └─SkippedTriviaSyntax
//@[015:0016) |   | ├─Token(Comma) |,|
//@[016:0017) |   | └─Token(RightParen) |)|
//@[018:0024) |   ├─TypeVariableAccessSyntax
//@[018:0024) |   | └─IdentifierSyntax
//@[018:0024) |   |   └─Token(Identifier) |string|
//@[025:0027) |   ├─Token(Arrow) |=>|
//@[028:0033) |   └─StringSyntax
//@[028:0033) |     └─Token(StringComplete) |'foo'|
//@[033:0034) ├─Token(NewLine) |\n|
func trailingCommas(a string,,) string => 'foo'
//@[000:0047) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0019) | ├─IdentifierSyntax
//@[005:0019) | | └─Token(Identifier) |trailingCommas|
//@[019:0047) | └─TypedLambdaSyntax
//@[019:0031) |   ├─TypedVariableBlockSyntax
//@[019:0020) |   | ├─Token(LeftParen) |(|
//@[020:0028) |   | ├─TypedLocalVariableSyntax
//@[020:0021) |   | | ├─IdentifierSyntax
//@[020:0021) |   | | | └─Token(Identifier) |a|
//@[022:0028) |   | | └─TypeVariableAccessSyntax
//@[022:0028) |   | |   └─IdentifierSyntax
//@[022:0028) |   | |     └─Token(Identifier) |string|
//@[028:0029) |   | ├─Token(Comma) |,|
//@[029:0029) |   | ├─TypedLocalVariableSyntax
//@[029:0029) |   | | ├─IdentifierSyntax
//@[029:0029) |   | | | └─SkippedTriviaSyntax
//@[029:0029) |   | | └─SkippedTriviaSyntax
//@[029:0030) |   | ├─Token(Comma) |,|
//@[030:0031) |   | └─Token(RightParen) |)|
//@[032:0038) |   ├─TypeVariableAccessSyntax
//@[032:0038) |   | └─IdentifierSyntax
//@[032:0038) |   |   └─Token(Identifier) |string|
//@[039:0041) |   ├─Token(Arrow) |=>|
//@[042:0047) |   └─StringSyntax
//@[042:0047) |     └─Token(StringComplete) |'foo'|
//@[047:0048) ├─Token(NewLine) |\n|
func multiLineOnly(
//@[000:0058) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0018) | ├─IdentifierSyntax
//@[005:0018) | | └─Token(Identifier) |multiLineOnly|
//@[018:0058) | └─TypedLambdaSyntax
//@[018:0058) |   ├─TypedVariableBlockSyntax
//@[018:0019) |   | ├─Token(LeftParen) |(|
//@[019:0020) |   | ├─Token(NewLine) |\n|
  a string
//@[002:0010) |   | ├─TypedLocalVariableSyntax
//@[002:0003) |   | | ├─IdentifierSyntax
//@[002:0003) |   | | | └─Token(Identifier) |a|
//@[004:0010) |   | | └─TypeVariableAccessSyntax
//@[004:0010) |   | |   └─IdentifierSyntax
//@[004:0010) |   | |     └─Token(Identifier) |string|
//@[010:0011) |   | ├─Token(NewLine) |\n|
  b string) string => 'foo'
//@[002:0027) |   | └─SkippedTriviaSyntax
//@[002:0003) |   |   ├─Token(Identifier) |b|
//@[004:0010) |   |   ├─Token(Identifier) |string|
//@[010:0011) |   |   ├─Token(RightParen) |)|
//@[012:0018) |   |   ├─Token(Identifier) |string|
//@[019:0021) |   |   ├─Token(Arrow) |=>|
//@[022:0027) |   |   └─Token(StringComplete) |'foo'|
//@[027:0027) |   ├─SkippedTriviaSyntax
//@[027:0027) |   ├─SkippedTriviaSyntax
//@[027:0027) |   └─SkippedTriviaSyntax
//@[027:0029) ├─Token(NewLine) |\n\n|

func multiLineTrailingCommas(
//@[000:0062) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0028) | ├─IdentifierSyntax
//@[005:0028) | | └─Token(Identifier) |multiLineTrailingCommas|
//@[028:0062) | └─TypedLambdaSyntax
//@[028:0046) |   ├─TypedVariableBlockSyntax
//@[028:0029) |   | ├─Token(LeftParen) |(|
//@[029:0030) |   | ├─Token(NewLine) |\n|
  a string,
//@[002:0010) |   | ├─TypedLocalVariableSyntax
//@[002:0003) |   | | ├─IdentifierSyntax
//@[002:0003) |   | | | └─Token(Identifier) |a|
//@[004:0010) |   | | └─TypeVariableAccessSyntax
//@[004:0010) |   | |   └─IdentifierSyntax
//@[004:0010) |   | |     └─Token(Identifier) |string|
//@[010:0011) |   | ├─Token(Comma) |,|
//@[011:0012) |   | ├─Token(NewLine) |\n|
  ,) string => 'foo'
//@[002:0002) |   | ├─TypedLocalVariableSyntax
//@[002:0002) |   | | ├─IdentifierSyntax
//@[002:0002) |   | | | └─SkippedTriviaSyntax
//@[002:0002) |   | | └─SkippedTriviaSyntax
//@[002:0003) |   | ├─Token(Comma) |,|
//@[003:0004) |   | └─Token(RightParen) |)|
//@[005:0011) |   ├─TypeVariableAccessSyntax
//@[005:0011) |   | └─IdentifierSyntax
//@[005:0011) |   |   └─Token(Identifier) |string|
//@[012:0014) |   ├─Token(Arrow) |=>|
//@[015:0020) |   └─StringSyntax
//@[015:0020) |     └─Token(StringComplete) |'foo'|
//@[020:0022) ├─Token(NewLine) |\n\n|

func lineBeforeComma(
//@[000:0061) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0020) | ├─IdentifierSyntax
//@[005:0020) | | └─Token(Identifier) |lineBeforeComma|
//@[020:0061) | └─TypedLambdaSyntax
//@[020:0045) |   ├─TypedVariableBlockSyntax
//@[020:0021) |   | ├─Token(LeftParen) |(|
//@[021:0022) |   | ├─Token(NewLine) |\n|
  a string
//@[002:0010) |   | ├─TypedLocalVariableSyntax
//@[002:0003) |   | | ├─IdentifierSyntax
//@[002:0003) |   | | | └─Token(Identifier) |a|
//@[004:0010) |   | | └─TypeVariableAccessSyntax
//@[004:0010) |   | |   └─IdentifierSyntax
//@[004:0010) |   | |     └─Token(Identifier) |string|
//@[010:0011) |   | ├─Token(NewLine) |\n|
  ,b string) string => 'foo'
//@[002:0003) |   | ├─Token(Comma) |,|
//@[003:0011) |   | ├─TypedLocalVariableSyntax
//@[003:0004) |   | | ├─IdentifierSyntax
//@[003:0004) |   | | | └─Token(Identifier) |b|
//@[005:0011) |   | | └─TypeVariableAccessSyntax
//@[005:0011) |   | |   └─IdentifierSyntax
//@[005:0011) |   | |     └─Token(Identifier) |string|
//@[011:0012) |   | └─Token(RightParen) |)|
//@[013:0019) |   ├─TypeVariableAccessSyntax
//@[013:0019) |   | └─IdentifierSyntax
//@[013:0019) |   |   └─Token(Identifier) |string|
//@[020:0022) |   ├─Token(Arrow) |=>|
//@[023:0028) |   └─StringSyntax
//@[023:0028) |     └─Token(StringComplete) |'foo'|
//@[028:0029) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
