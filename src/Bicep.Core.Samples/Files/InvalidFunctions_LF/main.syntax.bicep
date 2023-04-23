func useRuntimeFunction = () => reference('foo').bar
//@[000:936) ProgramSyntax
//@[000:052) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:023) | ├─IdentifierSyntax
//@[005:023) | | └─Token(Identifier) |useRuntimeFunction|
//@[024:025) | ├─Token(Assignment) |=|
//@[026:052) | └─TypedLambdaSyntax
//@[026:028) |   ├─TypedVariableBlockSyntax
//@[026:027) |   | ├─Token(LeftParen) |(|
//@[027:028) |   | └─Token(RightParen) |)|
//@[029:031) |   ├─Token(Arrow) |=>|
//@[032:052) |   └─PropertyAccessSyntax
//@[032:048) |     ├─FunctionCallSyntax
//@[032:041) |     | ├─IdentifierSyntax
//@[032:041) |     | | └─Token(Identifier) |reference|
//@[041:042) |     | ├─Token(LeftParen) |(|
//@[042:047) |     | ├─FunctionArgumentSyntax
//@[042:047) |     | | └─StringSyntax
//@[042:047) |     | |   └─Token(StringComplete) |'foo'|
//@[047:048) |     | └─Token(RightParen) |)|
//@[048:049) |     ├─Token(Dot) |.|
//@[049:052) |     └─IdentifierSyntax
//@[049:052) |       └─Token(Identifier) |bar|
//@[052:054) ├─Token(NewLine) |\n\n|

func constFunc = () => 'A'
//@[000:026) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |constFunc|
//@[015:016) | ├─Token(Assignment) |=|
//@[017:026) | └─TypedLambdaSyntax
//@[017:019) |   ├─TypedVariableBlockSyntax
//@[017:018) |   | ├─Token(LeftParen) |(|
//@[018:019) |   | └─Token(RightParen) |)|
//@[020:022) |   ├─Token(Arrow) |=>|
//@[023:026) |   └─StringSyntax
//@[023:026) |     └─Token(StringComplete) |'A'|
//@[026:027) ├─Token(NewLine) |\n|
func funcWithOtherFuncRef = () => constFunc()
//@[000:045) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:025) | ├─IdentifierSyntax
//@[005:025) | | └─Token(Identifier) |funcWithOtherFuncRef|
//@[026:027) | ├─Token(Assignment) |=|
//@[028:045) | └─TypedLambdaSyntax
//@[028:030) |   ├─TypedVariableBlockSyntax
//@[028:029) |   | ├─Token(LeftParen) |(|
//@[029:030) |   | └─Token(RightParen) |)|
//@[031:033) |   ├─Token(Arrow) |=>|
//@[034:045) |   └─FunctionCallSyntax
//@[034:043) |     ├─IdentifierSyntax
//@[034:043) |     | └─Token(Identifier) |constFunc|
//@[043:044) |     ├─Token(LeftParen) |(|
//@[044:045) |     └─Token(RightParen) |)|
//@[045:047) ├─Token(NewLine) |\n\n|

func invalidType = (string input) => input
//@[000:042) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:016) | ├─IdentifierSyntax
//@[005:016) | | └─Token(Identifier) |invalidType|
//@[017:018) | ├─Token(Assignment) |=|
//@[019:042) | └─TypedLambdaSyntax
//@[019:033) |   ├─TypedVariableBlockSyntax
//@[019:020) |   | ├─Token(LeftParen) |(|
//@[020:032) |   | ├─TypedLocalVariableSyntax
//@[020:026) |   | | ├─VariableAccessSyntax
//@[020:026) |   | | | └─IdentifierSyntax
//@[020:026) |   | | |   └─Token(Identifier) |string|
//@[027:032) |   | | └─IdentifierSyntax
//@[027:032) |   | |   └─Token(Identifier) |input|
//@[032:033) |   | └─Token(RightParen) |)|
//@[034:036) |   ├─Token(Arrow) |=>|
//@[037:042) |   └─VariableAccessSyntax
//@[037:042) |     └─IdentifierSyntax
//@[037:042) |       └─Token(Identifier) |input|
//@[042:044) ├─Token(NewLine) |\n\n|

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

func madeUpTypeArgs = (notAType a, alsoNotAType b) => '${a}-${b}'
//@[000:065) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:019) | ├─IdentifierSyntax
//@[005:019) | | └─Token(Identifier) |madeUpTypeArgs|
//@[020:021) | ├─Token(Assignment) |=|
//@[022:065) | └─TypedLambdaSyntax
//@[022:050) |   ├─TypedVariableBlockSyntax
//@[022:023) |   | ├─Token(LeftParen) |(|
//@[023:033) |   | ├─TypedLocalVariableSyntax
//@[023:031) |   | | ├─VariableAccessSyntax
//@[023:031) |   | | | └─IdentifierSyntax
//@[023:031) |   | | |   └─Token(Identifier) |notAType|
//@[032:033) |   | | └─IdentifierSyntax
//@[032:033) |   | |   └─Token(Identifier) |a|
//@[033:034) |   | ├─Token(Comma) |,|
//@[035:049) |   | ├─TypedLocalVariableSyntax
//@[035:047) |   | | ├─VariableAccessSyntax
//@[035:047) |   | | | └─IdentifierSyntax
//@[035:047) |   | | |   └─Token(Identifier) |alsoNotAType|
//@[048:049) |   | | └─IdentifierSyntax
//@[048:049) |   | |   └─Token(Identifier) |b|
//@[049:050) |   | └─Token(RightParen) |)|
//@[051:053) |   ├─Token(Arrow) |=>|
//@[054:065) |   └─StringSyntax
//@[054:057) |     ├─Token(StringLeftPiece) |'${|
//@[057:058) |     ├─VariableAccessSyntax
//@[057:058) |     | └─IdentifierSyntax
//@[057:058) |     |   └─Token(Identifier) |a|
//@[058:062) |     ├─Token(StringMiddlePiece) |}-${|
//@[062:063) |     ├─VariableAccessSyntax
//@[062:063) |     | └─IdentifierSyntax
//@[062:063) |     |   └─Token(Identifier) |b|
//@[063:065) |     └─Token(StringRightPiece) |}'|
//@[065:067) ├─Token(NewLine) |\n\n|

func noLambda = ('foo') => ''
//@[000:029) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |noLambda|
//@[014:015) | ├─Token(Assignment) |=|
//@[016:029) | └─TypedLambdaSyntax
//@[016:023) |   ├─TypedVariableBlockSyntax
//@[016:017) |   | ├─Token(LeftParen) |(|
//@[017:022) |   | ├─TypedLocalVariableSyntax
//@[017:022) |   | | ├─StringSyntax
//@[017:022) |   | | | └─Token(StringComplete) |'foo'|
//@[022:022) |   | | └─IdentifierSyntax
//@[022:022) |   | |   └─SkippedTriviaSyntax
//@[022:023) |   | └─Token(RightParen) |)|
//@[024:026) |   ├─Token(Arrow) |=>|
//@[027:029) |   └─StringSyntax
//@[027:029) |     └─Token(StringComplete) |''|
//@[029:031) ├─Token(NewLine) |\n\n|

func noLambda2 = ('foo' sdf) => ''
//@[000:034) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |noLambda2|
//@[015:016) | ├─Token(Assignment) |=|
//@[017:034) | └─TypedLambdaSyntax
//@[017:028) |   ├─TypedVariableBlockSyntax
//@[017:018) |   | ├─Token(LeftParen) |(|
//@[018:027) |   | ├─TypedLocalVariableSyntax
//@[018:023) |   | | ├─StringSyntax
//@[018:023) |   | | | └─Token(StringComplete) |'foo'|
//@[024:027) |   | | └─IdentifierSyntax
//@[024:027) |   | |   └─Token(Identifier) |sdf|
//@[027:028) |   | └─Token(RightParen) |)|
//@[029:031) |   ├─Token(Arrow) |=>|
//@[032:034) |   └─StringSyntax
//@[032:034) |     └─Token(StringComplete) |''|
//@[034:036) ├─Token(NewLine) |\n\n|

func noLambda3 = 'asdf'
//@[000:023) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:014) | ├─IdentifierSyntax
//@[005:014) | | └─Token(Identifier) |noLambda3|
//@[015:016) | ├─Token(Assignment) |=|
//@[017:023) | └─SkippedTriviaSyntax
//@[017:023) |   └─Token(StringComplete) |'asdf'|
//@[023:025) ├─Token(NewLine) |\n\n|

func argLengthMismatch = (string a, string b, string c) => [a, b, c]
//@[000:068) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:022) | ├─IdentifierSyntax
//@[005:022) | | └─Token(Identifier) |argLengthMismatch|
//@[023:024) | ├─Token(Assignment) |=|
//@[025:068) | └─TypedLambdaSyntax
//@[025:055) |   ├─TypedVariableBlockSyntax
//@[025:026) |   | ├─Token(LeftParen) |(|
//@[026:034) |   | ├─TypedLocalVariableSyntax
//@[026:032) |   | | ├─VariableAccessSyntax
//@[026:032) |   | | | └─IdentifierSyntax
//@[026:032) |   | | |   └─Token(Identifier) |string|
//@[033:034) |   | | └─IdentifierSyntax
//@[033:034) |   | |   └─Token(Identifier) |a|
//@[034:035) |   | ├─Token(Comma) |,|
//@[036:044) |   | ├─TypedLocalVariableSyntax
//@[036:042) |   | | ├─VariableAccessSyntax
//@[036:042) |   | | | └─IdentifierSyntax
//@[036:042) |   | | |   └─Token(Identifier) |string|
//@[043:044) |   | | └─IdentifierSyntax
//@[043:044) |   | |   └─Token(Identifier) |b|
//@[044:045) |   | ├─Token(Comma) |,|
//@[046:054) |   | ├─TypedLocalVariableSyntax
//@[046:052) |   | | ├─VariableAccessSyntax
//@[046:052) |   | | | └─IdentifierSyntax
//@[046:052) |   | | |   └─Token(Identifier) |string|
//@[053:054) |   | | └─IdentifierSyntax
//@[053:054) |   | |   └─Token(Identifier) |c|
//@[054:055) |   | └─Token(RightParen) |)|
//@[056:058) |   ├─Token(Arrow) |=>|
//@[059:068) |   └─ArraySyntax
//@[059:060) |     ├─Token(LeftSquare) |[|
//@[060:061) |     ├─ArrayItemSyntax
//@[060:061) |     | └─VariableAccessSyntax
//@[060:061) |     |   └─IdentifierSyntax
//@[060:061) |     |     └─Token(Identifier) |a|
//@[061:062) |     ├─Token(Comma) |,|
//@[063:064) |     ├─ArrayItemSyntax
//@[063:064) |     | └─VariableAccessSyntax
//@[063:064) |     |   └─IdentifierSyntax
//@[063:064) |     |     └─Token(Identifier) |b|
//@[064:065) |     ├─Token(Comma) |,|
//@[066:067) |     ├─ArrayItemSyntax
//@[066:067) |     | └─VariableAccessSyntax
//@[066:067) |     |   └─IdentifierSyntax
//@[066:067) |     |     └─Token(Identifier) |c|
//@[067:068) |     └─Token(RightSquare) |]|
//@[068:069) ├─Token(NewLine) |\n|
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

func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:137) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |buildUrl|
//@[014:015) | ├─Token(Assignment) |=|
//@[016:137) | └─TypedLambdaSyntax
//@[016:058) |   ├─TypedVariableBlockSyntax
//@[016:017) |   | ├─Token(LeftParen) |(|
//@[017:027) |   | ├─TypedLocalVariableSyntax
//@[017:021) |   | | ├─VariableAccessSyntax
//@[017:021) |   | | | └─IdentifierSyntax
//@[017:021) |   | | |   └─Token(Identifier) |bool|
//@[022:027) |   | | └─IdentifierSyntax
//@[022:027) |   | |   └─Token(Identifier) |https|
//@[027:028) |   | ├─Token(Comma) |,|
//@[029:044) |   | ├─TypedLocalVariableSyntax
//@[029:035) |   | | ├─VariableAccessSyntax
//@[029:035) |   | | | └─IdentifierSyntax
//@[029:035) |   | | |   └─Token(Identifier) |string|
//@[036:044) |   | | └─IdentifierSyntax
//@[036:044) |   | |   └─Token(Identifier) |hostname|
//@[044:045) |   | ├─Token(Comma) |,|
//@[046:057) |   | ├─TypedLocalVariableSyntax
//@[046:052) |   | | ├─VariableAccessSyntax
//@[046:052) |   | | | └─IdentifierSyntax
//@[046:052) |   | | |   └─Token(Identifier) |string|
//@[053:057) |   | | └─IdentifierSyntax
//@[053:057) |   | |   └─Token(Identifier) |path|
//@[057:058) |   | └─Token(RightParen) |)|
//@[059:061) |   ├─Token(Arrow) |=>|
//@[062:137) |   └─StringSyntax
//@[062:065) |     ├─Token(StringLeftPiece) |'${|
//@[065:089) |     ├─TernaryOperationSyntax
//@[065:070) |     | ├─VariableAccessSyntax
//@[065:070) |     | | └─IdentifierSyntax
//@[065:070) |     | |   └─Token(Identifier) |https|
//@[071:072) |     | ├─Token(Question) |?|
//@[073:080) |     | ├─StringSyntax
//@[073:080) |     | | └─Token(StringComplete) |'https'|
//@[081:082) |     | ├─Token(Colon) |:|
//@[083:089) |     | └─StringSyntax
//@[083:089) |     |   └─Token(StringComplete) |'http'|
//@[089:095) |     ├─Token(StringMiddlePiece) |}://${|
//@[095:103) |     ├─VariableAccessSyntax
//@[095:103) |     | └─IdentifierSyntax
//@[095:103) |     |   └─Token(Identifier) |hostname|
//@[103:106) |     ├─Token(StringMiddlePiece) |}${|
//@[106:135) |     ├─TernaryOperationSyntax
//@[106:117) |     | ├─FunctionCallSyntax
//@[106:111) |     | | ├─IdentifierSyntax
//@[106:111) |     | | | └─Token(Identifier) |empty|
//@[111:112) |     | | ├─Token(LeftParen) |(|
//@[112:116) |     | | ├─FunctionArgumentSyntax
//@[112:116) |     | | | └─VariableAccessSyntax
//@[112:116) |     | | |   └─IdentifierSyntax
//@[112:116) |     | | |     └─Token(Identifier) |path|
//@[116:117) |     | | └─Token(RightParen) |)|
//@[118:119) |     | ├─Token(Question) |?|
//@[120:122) |     | ├─StringSyntax
//@[120:122) |     | | └─Token(StringComplete) |''|
//@[123:124) |     | ├─Token(Colon) |:|
//@[125:135) |     | └─StringSyntax
//@[125:129) |     |   ├─Token(StringLeftPiece) |'/${|
//@[129:133) |     |   ├─VariableAccessSyntax
//@[129:133) |     |   | └─IdentifierSyntax
//@[129:133) |     |   |   └─Token(Identifier) |path|
//@[133:135) |     |   └─Token(StringRightPiece) |}'|
//@[135:137) |     └─Token(StringRightPiece) |}'|
//@[137:139) ├─Token(NewLine) |\n\n|

output foo array = buildUrl(true, 'google.com', 'search')
//@[000:057) ├─OutputDeclarationSyntax
//@[000:006) | ├─Token(Identifier) |output|
//@[007:010) | ├─IdentifierSyntax
//@[007:010) | | └─Token(Identifier) |foo|
//@[011:016) | ├─VariableAccessSyntax
//@[011:016) | | └─IdentifierSyntax
//@[011:016) | |   └─Token(Identifier) |array|
//@[017:018) | ├─Token(Assignment) |=|
//@[019:057) | └─FunctionCallSyntax
//@[019:027) |   ├─IdentifierSyntax
//@[019:027) |   | └─Token(Identifier) |buildUrl|
//@[027:028) |   ├─Token(LeftParen) |(|
//@[028:032) |   ├─FunctionArgumentSyntax
//@[028:032) |   | └─BooleanLiteralSyntax
//@[028:032) |   |   └─Token(TrueKeyword) |true|
//@[032:033) |   ├─Token(Comma) |,|
//@[034:046) |   ├─FunctionArgumentSyntax
//@[034:046) |   | └─StringSyntax
//@[034:046) |   |   └─Token(StringComplete) |'google.com'|
//@[046:047) |   ├─Token(Comma) |,|
//@[048:056) |   ├─FunctionArgumentSyntax
//@[048:056) |   | └─StringSyntax
//@[048:056) |   |   └─Token(StringComplete) |'search'|
//@[056:057) |   └─Token(RightParen) |)|
//@[057:059) ├─Token(NewLine) |\n\n|

func sayHello = (string name) => 'Hi ${name}!'
//@[000:046) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |sayHello|
//@[014:015) | ├─Token(Assignment) |=|
//@[016:046) | └─TypedLambdaSyntax
//@[016:029) |   ├─TypedVariableBlockSyntax
//@[016:017) |   | ├─Token(LeftParen) |(|
//@[017:028) |   | ├─TypedLocalVariableSyntax
//@[017:023) |   | | ├─VariableAccessSyntax
//@[017:023) |   | | | └─IdentifierSyntax
//@[017:023) |   | | |   └─Token(Identifier) |string|
//@[024:028) |   | | └─IdentifierSyntax
//@[024:028) |   | |   └─Token(Identifier) |name|
//@[028:029) |   | └─Token(RightParen) |)|
//@[030:032) |   ├─Token(Arrow) |=>|
//@[033:046) |   └─StringSyntax
//@[033:039) |     ├─Token(StringLeftPiece) |'Hi ${|
//@[039:043) |     ├─VariableAccessSyntax
//@[039:043) |     | └─IdentifierSyntax
//@[039:043) |     |   └─Token(Identifier) |name|
//@[043:046) |     └─Token(StringRightPiece) |}!'|
//@[046:047) ├─Token(NewLine) |\n|
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
//@[027:028) | └─SkippedTriviaSyntax
//@[027:028) |   └─Token(LeftParen) |(|
//@[028:029) ├─Token(NewLine) |\n|
  string name) => 'Hi ${name}!'
//@[002:031) ├─SkippedTriviaSyntax
//@[002:008) | ├─Token(Identifier) |string|
//@[009:013) | ├─Token(Identifier) |name|
//@[013:014) | ├─Token(RightParen) |)|
//@[015:017) | ├─Token(Arrow) |=>|
//@[018:024) | ├─Token(StringLeftPiece) |'Hi ${|
//@[024:028) | ├─Token(Identifier) |name|
//@[028:031) | └─Token(StringRightPiece) |}!'|
//@[031:032) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
