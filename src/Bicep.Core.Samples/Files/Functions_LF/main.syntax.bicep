func buildUrl = (bool https, string hostname, string path) => string '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:464) ProgramSyntax
//@[000:144) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |buildUrl|
//@[014:015) | ├─Token(Assignment) |=|
//@[016:144) | └─TypedLambdaSyntax
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
//@[062:068) |   ├─VariableAccessSyntax
//@[062:068) |   | └─IdentifierSyntax
//@[062:068) |   |   └─Token(Identifier) |string|
//@[069:144) |   └─StringSyntax
//@[069:072) |     ├─Token(StringLeftPiece) |'${|
//@[072:096) |     ├─TernaryOperationSyntax
//@[072:077) |     | ├─VariableAccessSyntax
//@[072:077) |     | | └─IdentifierSyntax
//@[072:077) |     | |   └─Token(Identifier) |https|
//@[078:079) |     | ├─Token(Question) |?|
//@[080:087) |     | ├─StringSyntax
//@[080:087) |     | | └─Token(StringComplete) |'https'|
//@[088:089) |     | ├─Token(Colon) |:|
//@[090:096) |     | └─StringSyntax
//@[090:096) |     |   └─Token(StringComplete) |'http'|
//@[096:102) |     ├─Token(StringMiddlePiece) |}://${|
//@[102:110) |     ├─VariableAccessSyntax
//@[102:110) |     | └─IdentifierSyntax
//@[102:110) |     |   └─Token(Identifier) |hostname|
//@[110:113) |     ├─Token(StringMiddlePiece) |}${|
//@[113:142) |     ├─TernaryOperationSyntax
//@[113:124) |     | ├─FunctionCallSyntax
//@[113:118) |     | | ├─IdentifierSyntax
//@[113:118) |     | | | └─Token(Identifier) |empty|
//@[118:119) |     | | ├─Token(LeftParen) |(|
//@[119:123) |     | | ├─FunctionArgumentSyntax
//@[119:123) |     | | | └─VariableAccessSyntax
//@[119:123) |     | | |   └─IdentifierSyntax
//@[119:123) |     | | |     └─Token(Identifier) |path|
//@[123:124) |     | | └─Token(RightParen) |)|
//@[125:126) |     | ├─Token(Question) |?|
//@[127:129) |     | ├─StringSyntax
//@[127:129) |     | | └─Token(StringComplete) |''|
//@[130:131) |     | ├─Token(Colon) |:|
//@[132:142) |     | └─StringSyntax
//@[132:136) |     |   ├─Token(StringLeftPiece) |'/${|
//@[136:140) |     |   ├─VariableAccessSyntax
//@[136:140) |     |   | └─IdentifierSyntax
//@[136:140) |     |   |   └─Token(Identifier) |path|
//@[140:142) |     |   └─Token(StringRightPiece) |}'|
//@[142:144) |     └─Token(StringRightPiece) |}'|
//@[144:146) ├─Token(NewLine) |\n\n|

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

func sayHello = (string name) => string 'Hi ${name}!'
//@[000:053) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |sayHello|
//@[014:015) | ├─Token(Assignment) |=|
//@[016:053) | └─TypedLambdaSyntax
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
//@[033:039) |   ├─VariableAccessSyntax
//@[033:039) |   | └─IdentifierSyntax
//@[033:039) |   |   └─Token(Identifier) |string|
//@[040:053) |   └─StringSyntax
//@[040:046) |     ├─Token(StringLeftPiece) |'Hi ${|
//@[046:050) |     ├─VariableAccessSyntax
//@[046:050) |     | └─IdentifierSyntax
//@[046:050) |     |   └─Token(Identifier) |name|
//@[050:053) |     └─Token(StringRightPiece) |}!'|
//@[053:055) ├─Token(NewLine) |\n\n|

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
//@[069:071) ├─Token(NewLine) |\n\n|

func objReturnType = (string name) => object {
//@[000:071) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:018) | ├─IdentifierSyntax
//@[005:018) | | └─Token(Identifier) |objReturnType|
//@[019:020) | ├─Token(Assignment) |=|
//@[021:071) | └─TypedLambdaSyntax
//@[021:034) |   ├─TypedVariableBlockSyntax
//@[021:022) |   | ├─Token(LeftParen) |(|
//@[022:033) |   | ├─TypedLocalVariableSyntax
//@[022:028) |   | | ├─VariableAccessSyntax
//@[022:028) |   | | | └─IdentifierSyntax
//@[022:028) |   | | |   └─Token(Identifier) |string|
//@[029:033) |   | | └─IdentifierSyntax
//@[029:033) |   | |   └─Token(Identifier) |name|
//@[033:034) |   | └─Token(RightParen) |)|
//@[035:037) |   ├─Token(Arrow) |=>|
//@[038:044) |   ├─VariableAccessSyntax
//@[038:044) |   | └─IdentifierSyntax
//@[038:044) |   |   └─Token(Identifier) |object|
//@[045:071) |   └─ObjectSyntax
//@[045:046) |     ├─Token(LeftBrace) |{|
//@[046:047) |     ├─Token(NewLine) |\n|
  hello: 'Hi ${name}!'
//@[002:022) |     ├─ObjectPropertySyntax
//@[002:007) |     | ├─IdentifierSyntax
//@[002:007) |     | | └─Token(Identifier) |hello|
//@[007:008) |     | ├─Token(Colon) |:|
//@[009:022) |     | └─StringSyntax
//@[009:015) |     |   ├─Token(StringLeftPiece) |'Hi ${|
//@[015:019) |     |   ├─VariableAccessSyntax
//@[015:019) |     |   | └─IdentifierSyntax
//@[015:019) |     |   |   └─Token(Identifier) |name|
//@[019:022) |     |   └─Token(StringRightPiece) |}!'|
//@[022:023) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

func arrayReturnType = (string name) => array ([
//@[000:058) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:020) | ├─IdentifierSyntax
//@[005:020) | | └─Token(Identifier) |arrayReturnType|
//@[021:022) | ├─Token(Assignment) |=|
//@[023:058) | └─TypedLambdaSyntax
//@[023:036) |   ├─TypedVariableBlockSyntax
//@[023:024) |   | ├─Token(LeftParen) |(|
//@[024:035) |   | ├─TypedLocalVariableSyntax
//@[024:030) |   | | ├─VariableAccessSyntax
//@[024:030) |   | | | └─IdentifierSyntax
//@[024:030) |   | | |   └─Token(Identifier) |string|
//@[031:035) |   | | └─IdentifierSyntax
//@[031:035) |   | |   └─Token(Identifier) |name|
//@[035:036) |   | └─Token(RightParen) |)|
//@[037:039) |   ├─Token(Arrow) |=>|
//@[040:045) |   ├─VariableAccessSyntax
//@[040:045) |   | └─IdentifierSyntax
//@[040:045) |   |   └─Token(Identifier) |array|
//@[046:058) |   └─ParenthesizedExpressionSyntax
//@[046:047) |     ├─Token(LeftParen) |(|
//@[047:057) |     ├─ArraySyntax
//@[047:048) |     | ├─Token(LeftSquare) |[|
//@[048:049) |     | ├─Token(NewLine) |\n|
  name
//@[002:006) |     | ├─ArrayItemSyntax
//@[002:006) |     | | └─VariableAccessSyntax
//@[002:006) |     | |   └─IdentifierSyntax
//@[002:006) |     | |     └─Token(Identifier) |name|
//@[006:007) |     | ├─Token(NewLine) |\n|
])
//@[000:001) |     | └─Token(RightSquare) |]|
//@[001:002) |     └─Token(RightParen) |)|
//@[002:003) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
