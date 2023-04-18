func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:317) ProgramSyntax
//@[000:137) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |buildUrl|
//@[014:015) | ├─Token(Assignment) |=|
//@[016:137) | └─TypedLambdaSyntax
//@[016:058) |   ├─TypedVariableBlockSyntax
//@[016:017) |   | ├─Token(LeftParen) |(|
//@[017:027) |   | ├─TypedLocalVariableSyntax
//@[022:027) |   | | ├─IdentifierSyntax
//@[022:027) |   | | | └─Token(Identifier) |https|
//@[017:021) |   | | └─VariableAccessSyntax
//@[017:021) |   | |   └─IdentifierSyntax
//@[017:021) |   | |     └─Token(Identifier) |bool|
//@[027:028) |   | ├─Token(Comma) |,|
//@[029:044) |   | ├─TypedLocalVariableSyntax
//@[036:044) |   | | ├─IdentifierSyntax
//@[036:044) |   | | | └─Token(Identifier) |hostname|
//@[029:035) |   | | └─VariableAccessSyntax
//@[029:035) |   | |   └─IdentifierSyntax
//@[029:035) |   | |     └─Token(Identifier) |string|
//@[044:045) |   | ├─Token(Comma) |,|
//@[046:057) |   | ├─TypedLocalVariableSyntax
//@[053:057) |   | | ├─IdentifierSyntax
//@[053:057) |   | | | └─Token(Identifier) |path|
//@[046:052) |   | | └─VariableAccessSyntax
//@[046:052) |   | |   └─IdentifierSyntax
//@[046:052) |   | |     └─Token(Identifier) |string|
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

output foo string = buildUrl(true, 'google.com', 'search')
//@[000:058) ├─OutputDeclarationSyntax
//@[000:006) | ├─Token(Identifier) |output|
//@[007:010) | ├─IdentifierSyntax
//@[007:010) | | └─Token(Identifier) |foo|
//@[011:017) | ├─VariableAccessSyntax
//@[011:017) | | └─IdentifierSyntax
//@[011:017) | |   └─Token(Identifier) |string|
//@[018:019) | ├─Token(Assignment) |=|
//@[020:058) | └─FunctionCallSyntax
//@[020:028) |   ├─IdentifierSyntax
//@[020:028) |   | └─Token(Identifier) |buildUrl|
//@[028:029) |   ├─Token(LeftParen) |(|
//@[029:033) |   ├─FunctionArgumentSyntax
//@[029:033) |   | └─BooleanLiteralSyntax
//@[029:033) |   |   └─Token(TrueKeyword) |true|
//@[033:034) |   ├─Token(Comma) |,|
//@[035:047) |   ├─FunctionArgumentSyntax
//@[035:047) |   | └─StringSyntax
//@[035:047) |   |   └─Token(StringComplete) |'google.com'|
//@[047:048) |   ├─Token(Comma) |,|
//@[049:057) |   ├─FunctionArgumentSyntax
//@[049:057) |   | └─StringSyntax
//@[049:057) |   |   └─Token(StringComplete) |'search'|
//@[057:058) |   └─Token(RightParen) |)|
//@[058:060) ├─Token(NewLine) |\n\n|

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
//@[024:028) |   | | ├─IdentifierSyntax
//@[024:028) |   | | | └─Token(Identifier) |name|
//@[017:023) |   | | └─VariableAccessSyntax
//@[017:023) |   | |   └─IdentifierSyntax
//@[017:023) |   | |     └─Token(Identifier) |string|
//@[028:029) |   | └─Token(RightParen) |)|
//@[030:032) |   ├─Token(Arrow) |=>|
//@[033:046) |   └─StringSyntax
//@[033:039) |     ├─Token(StringLeftPiece) |'Hi ${|
//@[039:043) |     ├─VariableAccessSyntax
//@[039:043) |     | └─IdentifierSyntax
//@[039:043) |     |   └─Token(Identifier) |name|
//@[043:046) |     └─Token(StringRightPiece) |}!'|
//@[046:048) ├─Token(NewLine) |\n\n|

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[000:069) ├─OutputDeclarationSyntax
//@[000:006) | ├─Token(Identifier) |output|
//@[007:013) | ├─IdentifierSyntax
//@[007:013) | | └─Token(Identifier) |hellos|
//@[014:019) | ├─VariableAccessSyntax
//@[014:019) | | └─IdentifierSyntax
//@[014:019) | |   └─Token(Identifier) |array|
//@[020:021) | ├─Token(Assignment) |=|
//@[022:069) | └─FunctionCallSyntax
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
//@[046:068) |   ├─FunctionArgumentSyntax
//@[046:068) |   | └─LambdaSyntax
//@[046:050) |   |   ├─LocalVariableSyntax
//@[046:050) |   |   | └─IdentifierSyntax
//@[046:050) |   |   |   └─Token(Identifier) |name|
//@[051:053) |   |   ├─Token(Arrow) |=>|
//@[054:068) |   |   └─FunctionCallSyntax
//@[054:062) |   |     ├─IdentifierSyntax
//@[054:062) |   |     | └─Token(Identifier) |sayHello|
//@[062:063) |   |     ├─Token(LeftParen) |(|
//@[063:067) |   |     ├─FunctionArgumentSyntax
//@[063:067) |   |     | └─VariableAccessSyntax
//@[063:067) |   |     |   └─IdentifierSyntax
//@[063:067) |   |     |     └─Token(Identifier) |name|
//@[067:068) |   |     └─Token(RightParen) |)|
//@[068:069) |   └─Token(RightParen) |)|
//@[069:070) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
