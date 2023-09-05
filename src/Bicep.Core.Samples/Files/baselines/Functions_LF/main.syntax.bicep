func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:600) ProgramSyntax
//@[000:141) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |buildUrl|
//@[013:141) | └─TypedLambdaSyntax
//@[013:055) |   ├─TypedVariableBlockSyntax
//@[013:014) |   | ├─Token(LeftParen) |(|
//@[014:024) |   | ├─TypedLocalVariableSyntax
//@[014:019) |   | | ├─IdentifierSyntax
//@[014:019) |   | | | └─Token(Identifier) |https|
//@[020:024) |   | | └─VariableAccessSyntax
//@[020:024) |   | |   └─IdentifierSyntax
//@[020:024) |   | |     └─Token(Identifier) |bool|
//@[024:025) |   | ├─Token(Comma) |,|
//@[026:041) |   | ├─TypedLocalVariableSyntax
//@[026:034) |   | | ├─IdentifierSyntax
//@[026:034) |   | | | └─Token(Identifier) |hostname|
//@[035:041) |   | | └─VariableAccessSyntax
//@[035:041) |   | |   └─IdentifierSyntax
//@[035:041) |   | |     └─Token(Identifier) |string|
//@[041:042) |   | ├─Token(Comma) |,|
//@[043:054) |   | ├─TypedLocalVariableSyntax
//@[043:047) |   | | ├─IdentifierSyntax
//@[043:047) |   | | | └─Token(Identifier) |path|
//@[048:054) |   | | └─VariableAccessSyntax
//@[048:054) |   | |   └─IdentifierSyntax
//@[048:054) |   | |     └─Token(Identifier) |string|
//@[054:055) |   | └─Token(RightParen) |)|
//@[056:062) |   ├─VariableAccessSyntax
//@[056:062) |   | └─IdentifierSyntax
//@[056:062) |   |   └─Token(Identifier) |string|
//@[063:065) |   ├─Token(Arrow) |=>|
//@[066:141) |   └─StringSyntax
//@[066:069) |     ├─Token(StringLeftPiece) |'${|
//@[069:093) |     ├─TernaryOperationSyntax
//@[069:074) |     | ├─VariableAccessSyntax
//@[069:074) |     | | └─IdentifierSyntax
//@[069:074) |     | |   └─Token(Identifier) |https|
//@[075:076) |     | ├─Token(Question) |?|
//@[077:084) |     | ├─StringSyntax
//@[077:084) |     | | └─Token(StringComplete) |'https'|
//@[085:086) |     | ├─Token(Colon) |:|
//@[087:093) |     | └─StringSyntax
//@[087:093) |     |   └─Token(StringComplete) |'http'|
//@[093:099) |     ├─Token(StringMiddlePiece) |}://${|
//@[099:107) |     ├─VariableAccessSyntax
//@[099:107) |     | └─IdentifierSyntax
//@[099:107) |     |   └─Token(Identifier) |hostname|
//@[107:110) |     ├─Token(StringMiddlePiece) |}${|
//@[110:139) |     ├─TernaryOperationSyntax
//@[110:121) |     | ├─FunctionCallSyntax
//@[110:115) |     | | ├─IdentifierSyntax
//@[110:115) |     | | | └─Token(Identifier) |empty|
//@[115:116) |     | | ├─Token(LeftParen) |(|
//@[116:120) |     | | ├─FunctionArgumentSyntax
//@[116:120) |     | | | └─VariableAccessSyntax
//@[116:120) |     | | |   └─IdentifierSyntax
//@[116:120) |     | | |     └─Token(Identifier) |path|
//@[120:121) |     | | └─Token(RightParen) |)|
//@[122:123) |     | ├─Token(Question) |?|
//@[124:126) |     | ├─StringSyntax
//@[124:126) |     | | └─Token(StringComplete) |''|
//@[127:128) |     | ├─Token(Colon) |:|
//@[129:139) |     | └─StringSyntax
//@[129:133) |     |   ├─Token(StringLeftPiece) |'/${|
//@[133:137) |     |   ├─VariableAccessSyntax
//@[133:137) |     |   | └─IdentifierSyntax
//@[133:137) |     |   |   └─Token(Identifier) |path|
//@[137:139) |     |   └─Token(StringRightPiece) |}'|
//@[139:141) |     └─Token(StringRightPiece) |}'|
//@[141:143) ├─Token(NewLine) |\n\n|

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
//@[050:052) ├─Token(NewLine) |\n\n|

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

func objReturnType(name string) object => {
//@[000:068) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:018) | ├─IdentifierSyntax
//@[005:018) | | └─Token(Identifier) |objReturnType|
//@[018:068) | └─TypedLambdaSyntax
//@[018:031) |   ├─TypedVariableBlockSyntax
//@[018:019) |   | ├─Token(LeftParen) |(|
//@[019:030) |   | ├─TypedLocalVariableSyntax
//@[019:023) |   | | ├─IdentifierSyntax
//@[019:023) |   | | | └─Token(Identifier) |name|
//@[024:030) |   | | └─VariableAccessSyntax
//@[024:030) |   | |   └─IdentifierSyntax
//@[024:030) |   | |     └─Token(Identifier) |string|
//@[030:031) |   | └─Token(RightParen) |)|
//@[032:038) |   ├─VariableAccessSyntax
//@[032:038) |   | └─IdentifierSyntax
//@[032:038) |   |   └─Token(Identifier) |object|
//@[039:041) |   ├─Token(Arrow) |=>|
//@[042:068) |   └─ObjectSyntax
//@[042:043) |     ├─Token(LeftBrace) |{|
//@[043:044) |     ├─Token(NewLine) |\n|
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

func arrayReturnType(name string) array => [
//@[000:053) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:020) | ├─IdentifierSyntax
//@[005:020) | | └─Token(Identifier) |arrayReturnType|
//@[020:053) | └─TypedLambdaSyntax
//@[020:033) |   ├─TypedVariableBlockSyntax
//@[020:021) |   | ├─Token(LeftParen) |(|
//@[021:032) |   | ├─TypedLocalVariableSyntax
//@[021:025) |   | | ├─IdentifierSyntax
//@[021:025) |   | | | └─Token(Identifier) |name|
//@[026:032) |   | | └─VariableAccessSyntax
//@[026:032) |   | |   └─IdentifierSyntax
//@[026:032) |   | |     └─Token(Identifier) |string|
//@[032:033) |   | └─Token(RightParen) |)|
//@[034:039) |   ├─VariableAccessSyntax
//@[034:039) |   | └─IdentifierSyntax
//@[034:039) |   |   └─Token(Identifier) |array|
//@[040:042) |   ├─Token(Arrow) |=>|
//@[043:053) |   └─ArraySyntax
//@[043:044) |     ├─Token(LeftSquare) |[|
//@[044:045) |     ├─Token(NewLine) |\n|
  name
//@[002:006) |     ├─ArrayItemSyntax
//@[002:006) |     | └─VariableAccessSyntax
//@[002:006) |     |   └─IdentifierSyntax
//@[002:006) |     |     └─Token(Identifier) |name|
//@[006:007) |     ├─Token(NewLine) |\n|
]
//@[000:001) |     └─Token(RightSquare) |]|
//@[001:003) ├─Token(NewLine) |\n\n|

func asdf(name string) array => [
//@[000:051) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:009) | ├─IdentifierSyntax
//@[005:009) | | └─Token(Identifier) |asdf|
//@[009:051) | └─TypedLambdaSyntax
//@[009:022) |   ├─TypedVariableBlockSyntax
//@[009:010) |   | ├─Token(LeftParen) |(|
//@[010:021) |   | ├─TypedLocalVariableSyntax
//@[010:014) |   | | ├─IdentifierSyntax
//@[010:014) |   | | | └─Token(Identifier) |name|
//@[015:021) |   | | └─VariableAccessSyntax
//@[015:021) |   | |   └─IdentifierSyntax
//@[015:021) |   | |     └─Token(Identifier) |string|
//@[021:022) |   | └─Token(RightParen) |)|
//@[023:028) |   ├─VariableAccessSyntax
//@[023:028) |   | └─IdentifierSyntax
//@[023:028) |   |   └─Token(Identifier) |array|
//@[029:031) |   ├─Token(Arrow) |=>|
//@[032:051) |   └─ArraySyntax
//@[032:033) |     ├─Token(LeftSquare) |[|
//@[033:034) |     ├─Token(NewLine) |\n|
  'asdf'
//@[002:008) |     ├─ArrayItemSyntax
//@[002:008) |     | └─StringSyntax
//@[002:008) |     |   └─Token(StringComplete) |'asdf'|
//@[008:009) |     ├─Token(NewLine) |\n|
  name
//@[002:006) |     ├─ArrayItemSyntax
//@[002:006) |     | └─VariableAccessSyntax
//@[002:006) |     |   └─IdentifierSyntax
//@[002:006) |     |     └─Token(Identifier) |name|
//@[006:007) |     ├─Token(NewLine) |\n|
]
//@[000:001) |     └─Token(RightSquare) |]|
//@[001:003) ├─Token(NewLine) |\n\n|

@minValue(0)
//@[000:035) ├─TypeDeclarationSyntax
//@[000:012) | ├─DecoratorSyntax
//@[000:001) | | ├─Token(At) |@|
//@[001:012) | | └─FunctionCallSyntax
//@[001:009) | |   ├─IdentifierSyntax
//@[001:009) | |   | └─Token(Identifier) |minValue|
//@[009:010) | |   ├─Token(LeftParen) |(|
//@[010:011) | |   ├─FunctionArgumentSyntax
//@[010:011) | |   | └─IntegerLiteralSyntax
//@[010:011) | |   |   └─Token(Integer) |0|
//@[011:012) | |   └─Token(RightParen) |)|
//@[012:013) | ├─Token(NewLine) |\n|
type positiveInt = int
//@[000:004) | ├─Token(Identifier) |type|
//@[005:016) | ├─IdentifierSyntax
//@[005:016) | | └─Token(Identifier) |positiveInt|
//@[017:018) | ├─Token(Assignment) |=|
//@[019:022) | └─VariableAccessSyntax
//@[019:022) |   └─IdentifierSyntax
//@[019:022) |     └─Token(Identifier) |int|
//@[022:024) ├─Token(NewLine) |\n\n|

func typedArg(input string[]) positiveInt => length(input)
//@[000:058) ├─FunctionDeclarationSyntax
//@[000:004) | ├─Token(Identifier) |func|
//@[005:013) | ├─IdentifierSyntax
//@[005:013) | | └─Token(Identifier) |typedArg|
//@[013:058) | └─TypedLambdaSyntax
//@[013:029) |   ├─TypedVariableBlockSyntax
//@[013:014) |   | ├─Token(LeftParen) |(|
//@[014:028) |   | ├─TypedLocalVariableSyntax
//@[014:019) |   | | ├─IdentifierSyntax
//@[014:019) |   | | | └─Token(Identifier) |input|
//@[020:028) |   | | └─ArrayTypeSyntax
//@[020:026) |   | |   ├─ArrayTypeMemberSyntax
//@[020:026) |   | |   | └─VariableAccessSyntax
//@[020:026) |   | |   |   └─IdentifierSyntax
//@[020:026) |   | |   |     └─Token(Identifier) |string|
//@[026:027) |   | |   ├─Token(LeftSquare) |[|
//@[027:028) |   | |   └─Token(RightSquare) |]|
//@[028:029) |   | └─Token(RightParen) |)|
//@[030:041) |   ├─VariableAccessSyntax
//@[030:041) |   | └─IdentifierSyntax
//@[030:041) |   |   └─Token(Identifier) |positiveInt|
//@[042:044) |   ├─Token(Arrow) |=>|
//@[045:058) |   └─FunctionCallSyntax
//@[045:051) |     ├─IdentifierSyntax
//@[045:051) |     | └─Token(Identifier) |length|
//@[051:052) |     ├─Token(LeftParen) |(|
//@[052:057) |     ├─FunctionArgumentSyntax
//@[052:057) |     | └─VariableAccessSyntax
//@[052:057) |     |   └─IdentifierSyntax
//@[052:057) |     |     └─Token(Identifier) |input|
//@[057:058) |     └─Token(RightParen) |)|
//@[058:059) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
