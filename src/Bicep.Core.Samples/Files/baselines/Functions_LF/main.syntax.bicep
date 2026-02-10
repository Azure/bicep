func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:1564) ProgramSyntax
//@[000:0141) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |buildUrl|
//@[013:0141) | └─TypedLambdaSyntax
//@[013:0055) |   ├─TypedVariableBlockSyntax
//@[013:0014) |   | ├─Token(LeftParen) |(|
//@[014:0024) |   | ├─TypedLocalVariableSyntax
//@[014:0019) |   | | ├─IdentifierSyntax
//@[014:0019) |   | | | └─Token(Identifier) |https|
//@[020:0024) |   | | └─TypeVariableAccessSyntax
//@[020:0024) |   | |   └─IdentifierSyntax
//@[020:0024) |   | |     └─Token(Identifier) |bool|
//@[024:0025) |   | ├─Token(Comma) |,|
//@[026:0041) |   | ├─TypedLocalVariableSyntax
//@[026:0034) |   | | ├─IdentifierSyntax
//@[026:0034) |   | | | └─Token(Identifier) |hostname|
//@[035:0041) |   | | └─TypeVariableAccessSyntax
//@[035:0041) |   | |   └─IdentifierSyntax
//@[035:0041) |   | |     └─Token(Identifier) |string|
//@[041:0042) |   | ├─Token(Comma) |,|
//@[043:0054) |   | ├─TypedLocalVariableSyntax
//@[043:0047) |   | | ├─IdentifierSyntax
//@[043:0047) |   | | | └─Token(Identifier) |path|
//@[048:0054) |   | | └─TypeVariableAccessSyntax
//@[048:0054) |   | |   └─IdentifierSyntax
//@[048:0054) |   | |     └─Token(Identifier) |string|
//@[054:0055) |   | └─Token(RightParen) |)|
//@[056:0062) |   ├─TypeVariableAccessSyntax
//@[056:0062) |   | └─IdentifierSyntax
//@[056:0062) |   |   └─Token(Identifier) |string|
//@[063:0065) |   ├─Token(Arrow) |=>|
//@[066:0141) |   └─StringSyntax
//@[066:0069) |     ├─Token(StringLeftPiece) |'${|
//@[069:0093) |     ├─TernaryOperationSyntax
//@[069:0074) |     | ├─VariableAccessSyntax
//@[069:0074) |     | | └─IdentifierSyntax
//@[069:0074) |     | |   └─Token(Identifier) |https|
//@[075:0076) |     | ├─Token(Question) |?|
//@[077:0084) |     | ├─StringSyntax
//@[077:0084) |     | | └─Token(StringComplete) |'https'|
//@[085:0086) |     | ├─Token(Colon) |:|
//@[087:0093) |     | └─StringSyntax
//@[087:0093) |     |   └─Token(StringComplete) |'http'|
//@[093:0099) |     ├─Token(StringMiddlePiece) |}://${|
//@[099:0107) |     ├─VariableAccessSyntax
//@[099:0107) |     | └─IdentifierSyntax
//@[099:0107) |     |   └─Token(Identifier) |hostname|
//@[107:0110) |     ├─Token(StringMiddlePiece) |}${|
//@[110:0139) |     ├─TernaryOperationSyntax
//@[110:0121) |     | ├─FunctionCallSyntax
//@[110:0115) |     | | ├─IdentifierSyntax
//@[110:0115) |     | | | └─Token(Identifier) |empty|
//@[115:0116) |     | | ├─Token(LeftParen) |(|
//@[116:0120) |     | | ├─FunctionArgumentSyntax
//@[116:0120) |     | | | └─VariableAccessSyntax
//@[116:0120) |     | | |   └─IdentifierSyntax
//@[116:0120) |     | | |     └─Token(Identifier) |path|
//@[120:0121) |     | | └─Token(RightParen) |)|
//@[122:0123) |     | ├─Token(Question) |?|
//@[124:0126) |     | ├─StringSyntax
//@[124:0126) |     | | └─Token(StringComplete) |''|
//@[127:0128) |     | ├─Token(Colon) |:|
//@[129:0139) |     | └─StringSyntax
//@[129:0133) |     |   ├─Token(StringLeftPiece) |'/${|
//@[133:0137) |     |   ├─VariableAccessSyntax
//@[133:0137) |     |   | └─IdentifierSyntax
//@[133:0137) |     |   |   └─Token(Identifier) |path|
//@[137:0139) |     |   └─Token(StringRightPiece) |}'|
//@[139:0141) |     └─Token(StringRightPiece) |}'|
//@[141:0143) ├─Token(NewLine) |\n\n|

output foo string = buildUrl(true, 'google.com', 'search')
//@[000:0058) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |foo|
//@[011:0017) | ├─TypeVariableAccessSyntax
//@[011:0017) | | └─IdentifierSyntax
//@[011:0017) | |   └─Token(Identifier) |string|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0058) | └─FunctionCallSyntax
//@[020:0028) |   ├─IdentifierSyntax
//@[020:0028) |   | └─Token(Identifier) |buildUrl|
//@[028:0029) |   ├─Token(LeftParen) |(|
//@[029:0033) |   ├─FunctionArgumentSyntax
//@[029:0033) |   | └─BooleanLiteralSyntax
//@[029:0033) |   |   └─Token(TrueKeyword) |true|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0047) |   ├─FunctionArgumentSyntax
//@[035:0047) |   | └─StringSyntax
//@[035:0047) |   |   └─Token(StringComplete) |'google.com'|
//@[047:0048) |   ├─Token(Comma) |,|
//@[049:0057) |   ├─FunctionArgumentSyntax
//@[049:0057) |   | └─StringSyntax
//@[049:0057) |   |   └─Token(StringComplete) |'search'|
//@[057:0058) |   └─Token(RightParen) |)|
//@[058:0060) ├─Token(NewLine) |\n\n|

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
//@[050:0052) ├─Token(NewLine) |\n\n|

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[000:0069) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0013) | ├─IdentifierSyntax
//@[007:0013) | | └─Token(Identifier) |hellos|
//@[014:0019) | ├─TypeVariableAccessSyntax
//@[014:0019) | | └─IdentifierSyntax
//@[014:0019) | |   └─Token(Identifier) |array|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0069) | └─FunctionCallSyntax
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
//@[046:0068) |   ├─FunctionArgumentSyntax
//@[046:0068) |   | └─LambdaSyntax
//@[046:0050) |   |   ├─LocalVariableSyntax
//@[046:0050) |   |   | └─IdentifierSyntax
//@[046:0050) |   |   |   └─Token(Identifier) |name|
//@[051:0053) |   |   ├─Token(Arrow) |=>|
//@[054:0068) |   |   └─FunctionCallSyntax
//@[054:0062) |   |     ├─IdentifierSyntax
//@[054:0062) |   |     | └─Token(Identifier) |sayHello|
//@[062:0063) |   |     ├─Token(LeftParen) |(|
//@[063:0067) |   |     ├─FunctionArgumentSyntax
//@[063:0067) |   |     | └─VariableAccessSyntax
//@[063:0067) |   |     |   └─IdentifierSyntax
//@[063:0067) |   |     |     └─Token(Identifier) |name|
//@[067:0068) |   |     └─Token(RightParen) |)|
//@[068:0069) |   └─Token(RightParen) |)|
//@[069:0071) ├─Token(NewLine) |\n\n|

func objReturnType(name string) object => {
//@[000:0068) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0018) | ├─IdentifierSyntax
//@[005:0018) | | └─Token(Identifier) |objReturnType|
//@[018:0068) | └─TypedLambdaSyntax
//@[018:0031) |   ├─TypedVariableBlockSyntax
//@[018:0019) |   | ├─Token(LeftParen) |(|
//@[019:0030) |   | ├─TypedLocalVariableSyntax
//@[019:0023) |   | | ├─IdentifierSyntax
//@[019:0023) |   | | | └─Token(Identifier) |name|
//@[024:0030) |   | | └─TypeVariableAccessSyntax
//@[024:0030) |   | |   └─IdentifierSyntax
//@[024:0030) |   | |     └─Token(Identifier) |string|
//@[030:0031) |   | └─Token(RightParen) |)|
//@[032:0038) |   ├─TypeVariableAccessSyntax
//@[032:0038) |   | └─IdentifierSyntax
//@[032:0038) |   |   └─Token(Identifier) |object|
//@[039:0041) |   ├─Token(Arrow) |=>|
//@[042:0068) |   └─ObjectSyntax
//@[042:0043) |     ├─Token(LeftBrace) |{|
//@[043:0044) |     ├─Token(NewLine) |\n|
  hello: 'Hi ${name}!'
//@[002:0022) |     ├─ObjectPropertySyntax
//@[002:0007) |     | ├─IdentifierSyntax
//@[002:0007) |     | | └─Token(Identifier) |hello|
//@[007:0008) |     | ├─Token(Colon) |:|
//@[009:0022) |     | └─StringSyntax
//@[009:0015) |     |   ├─Token(StringLeftPiece) |'Hi ${|
//@[015:0019) |     |   ├─VariableAccessSyntax
//@[015:0019) |     |   | └─IdentifierSyntax
//@[015:0019) |     |   |   └─Token(Identifier) |name|
//@[019:0022) |     |   └─Token(StringRightPiece) |}!'|
//@[022:0023) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

func arrayReturnType(name string) array => [
//@[000:0053) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0020) | ├─IdentifierSyntax
//@[005:0020) | | └─Token(Identifier) |arrayReturnType|
//@[020:0053) | └─TypedLambdaSyntax
//@[020:0033) |   ├─TypedVariableBlockSyntax
//@[020:0021) |   | ├─Token(LeftParen) |(|
//@[021:0032) |   | ├─TypedLocalVariableSyntax
//@[021:0025) |   | | ├─IdentifierSyntax
//@[021:0025) |   | | | └─Token(Identifier) |name|
//@[026:0032) |   | | └─TypeVariableAccessSyntax
//@[026:0032) |   | |   └─IdentifierSyntax
//@[026:0032) |   | |     └─Token(Identifier) |string|
//@[032:0033) |   | └─Token(RightParen) |)|
//@[034:0039) |   ├─TypeVariableAccessSyntax
//@[034:0039) |   | └─IdentifierSyntax
//@[034:0039) |   |   └─Token(Identifier) |array|
//@[040:0042) |   ├─Token(Arrow) |=>|
//@[043:0053) |   └─ArraySyntax
//@[043:0044) |     ├─Token(LeftSquare) |[|
//@[044:0045) |     ├─Token(NewLine) |\n|
  name
//@[002:0006) |     ├─ArrayItemSyntax
//@[002:0006) |     | └─VariableAccessSyntax
//@[002:0006) |     |   └─IdentifierSyntax
//@[002:0006) |     |     └─Token(Identifier) |name|
//@[006:0007) |     ├─Token(NewLine) |\n|
]
//@[000:0001) |     └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

func asdf(name string) array => [
//@[000:0051) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0009) | ├─IdentifierSyntax
//@[005:0009) | | └─Token(Identifier) |asdf|
//@[009:0051) | └─TypedLambdaSyntax
//@[009:0022) |   ├─TypedVariableBlockSyntax
//@[009:0010) |   | ├─Token(LeftParen) |(|
//@[010:0021) |   | ├─TypedLocalVariableSyntax
//@[010:0014) |   | | ├─IdentifierSyntax
//@[010:0014) |   | | | └─Token(Identifier) |name|
//@[015:0021) |   | | └─TypeVariableAccessSyntax
//@[015:0021) |   | |   └─IdentifierSyntax
//@[015:0021) |   | |     └─Token(Identifier) |string|
//@[021:0022) |   | └─Token(RightParen) |)|
//@[023:0028) |   ├─TypeVariableAccessSyntax
//@[023:0028) |   | └─IdentifierSyntax
//@[023:0028) |   |   └─Token(Identifier) |array|
//@[029:0031) |   ├─Token(Arrow) |=>|
//@[032:0051) |   └─ArraySyntax
//@[032:0033) |     ├─Token(LeftSquare) |[|
//@[033:0034) |     ├─Token(NewLine) |\n|
  'asdf'
//@[002:0008) |     ├─ArrayItemSyntax
//@[002:0008) |     | └─StringSyntax
//@[002:0008) |     |   └─Token(StringComplete) |'asdf'|
//@[008:0009) |     ├─Token(NewLine) |\n|
  name
//@[002:0006) |     ├─ArrayItemSyntax
//@[002:0006) |     | └─VariableAccessSyntax
//@[002:0006) |     |   └─IdentifierSyntax
//@[002:0006) |     |     └─Token(Identifier) |name|
//@[006:0007) |     ├─Token(NewLine) |\n|
]
//@[000:0001) |     └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

@minValue(0)
//@[000:0035) ├─TypeDeclarationSyntax
//@[000:0012) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0012) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0011) | |   ├─FunctionArgumentSyntax
//@[010:0011) | |   | └─IntegerLiteralSyntax
//@[010:0011) | |   |   └─Token(Integer) |0|
//@[011:0012) | |   └─Token(RightParen) |)|
//@[012:0013) | ├─Token(NewLine) |\n|
type positiveInt = int
//@[000:0004) | ├─Token(Identifier) |type|
//@[005:0016) | ├─IdentifierSyntax
//@[005:0016) | | └─Token(Identifier) |positiveInt|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0022) | └─TypeVariableAccessSyntax
//@[019:0022) |   └─IdentifierSyntax
//@[019:0022) |     └─Token(Identifier) |int|
//@[022:0024) ├─Token(NewLine) |\n\n|

func typedArg(input string[]) positiveInt => length(input)
//@[000:0058) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0013) | ├─IdentifierSyntax
//@[005:0013) | | └─Token(Identifier) |typedArg|
//@[013:0058) | └─TypedLambdaSyntax
//@[013:0029) |   ├─TypedVariableBlockSyntax
//@[013:0014) |   | ├─Token(LeftParen) |(|
//@[014:0028) |   | ├─TypedLocalVariableSyntax
//@[014:0019) |   | | ├─IdentifierSyntax
//@[014:0019) |   | | | └─Token(Identifier) |input|
//@[020:0028) |   | | └─ArrayTypeSyntax
//@[020:0026) |   | |   ├─ArrayTypeMemberSyntax
//@[020:0026) |   | |   | └─TypeVariableAccessSyntax
//@[020:0026) |   | |   |   └─IdentifierSyntax
//@[020:0026) |   | |   |     └─Token(Identifier) |string|
//@[026:0027) |   | |   ├─Token(LeftSquare) |[|
//@[027:0028) |   | |   └─Token(RightSquare) |]|
//@[028:0029) |   | └─Token(RightParen) |)|
//@[030:0041) |   ├─TypeVariableAccessSyntax
//@[030:0041) |   | └─IdentifierSyntax
//@[030:0041) |   |   └─Token(Identifier) |positiveInt|
//@[042:0044) |   ├─Token(Arrow) |=>|
//@[045:0058) |   └─FunctionCallSyntax
//@[045:0051) |     ├─IdentifierSyntax
//@[045:0051) |     | └─Token(Identifier) |length|
//@[051:0052) |     ├─Token(LeftParen) |(|
//@[052:0057) |     ├─FunctionArgumentSyntax
//@[052:0057) |     | └─VariableAccessSyntax
//@[052:0057) |     |   └─IdentifierSyntax
//@[052:0057) |     |     └─Token(Identifier) |input|
//@[057:0058) |     └─Token(RightParen) |)|
//@[058:0060) ├─Token(NewLine) |\n\n|

func barTest() array => ['abc', 'def']
//@[000:0038) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0012) | ├─IdentifierSyntax
//@[005:0012) | | └─Token(Identifier) |barTest|
//@[012:0038) | └─TypedLambdaSyntax
//@[012:0014) |   ├─TypedVariableBlockSyntax
//@[012:0013) |   | ├─Token(LeftParen) |(|
//@[013:0014) |   | └─Token(RightParen) |)|
//@[015:0020) |   ├─TypeVariableAccessSyntax
//@[015:0020) |   | └─IdentifierSyntax
//@[015:0020) |   |   └─Token(Identifier) |array|
//@[021:0023) |   ├─Token(Arrow) |=>|
//@[024:0038) |   └─ArraySyntax
//@[024:0025) |     ├─Token(LeftSquare) |[|
//@[025:0030) |     ├─ArrayItemSyntax
//@[025:0030) |     | └─StringSyntax
//@[025:0030) |     |   └─Token(StringComplete) |'abc'|
//@[030:0031) |     ├─Token(Comma) |,|
//@[032:0037) |     ├─ArrayItemSyntax
//@[032:0037) |     | └─StringSyntax
//@[032:0037) |     |   └─Token(StringComplete) |'def'|
//@[037:0038) |     └─Token(RightSquare) |]|
//@[038:0039) ├─Token(NewLine) |\n|
func fooTest() array => map(barTest(), a => 'Hello ${a}!')
//@[000:0058) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0012) | ├─IdentifierSyntax
//@[005:0012) | | └─Token(Identifier) |fooTest|
//@[012:0058) | └─TypedLambdaSyntax
//@[012:0014) |   ├─TypedVariableBlockSyntax
//@[012:0013) |   | ├─Token(LeftParen) |(|
//@[013:0014) |   | └─Token(RightParen) |)|
//@[015:0020) |   ├─TypeVariableAccessSyntax
//@[015:0020) |   | └─IdentifierSyntax
//@[015:0020) |   |   └─Token(Identifier) |array|
//@[021:0023) |   ├─Token(Arrow) |=>|
//@[024:0058) |   └─FunctionCallSyntax
//@[024:0027) |     ├─IdentifierSyntax
//@[024:0027) |     | └─Token(Identifier) |map|
//@[027:0028) |     ├─Token(LeftParen) |(|
//@[028:0037) |     ├─FunctionArgumentSyntax
//@[028:0037) |     | └─FunctionCallSyntax
//@[028:0035) |     |   ├─IdentifierSyntax
//@[028:0035) |     |   | └─Token(Identifier) |barTest|
//@[035:0036) |     |   ├─Token(LeftParen) |(|
//@[036:0037) |     |   └─Token(RightParen) |)|
//@[037:0038) |     ├─Token(Comma) |,|
//@[039:0057) |     ├─FunctionArgumentSyntax
//@[039:0057) |     | └─LambdaSyntax
//@[039:0040) |     |   ├─LocalVariableSyntax
//@[039:0040) |     |   | └─IdentifierSyntax
//@[039:0040) |     |   |   └─Token(Identifier) |a|
//@[041:0043) |     |   ├─Token(Arrow) |=>|
//@[044:0057) |     |   └─StringSyntax
//@[044:0053) |     |     ├─Token(StringLeftPiece) |'Hello ${|
//@[053:0054) |     |     ├─VariableAccessSyntax
//@[053:0054) |     |     | └─IdentifierSyntax
//@[053:0054) |     |     |   └─Token(Identifier) |a|
//@[054:0057) |     |     └─Token(StringRightPiece) |}!'|
//@[057:0058) |     └─Token(RightParen) |)|
//@[058:0060) ├─Token(NewLine) |\n\n|

output fooValue array = fooTest()
//@[000:0033) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |fooValue|
//@[016:0021) | ├─TypeVariableAccessSyntax
//@[016:0021) | | └─IdentifierSyntax
//@[016:0021) | |   └─Token(Identifier) |array|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0033) | └─FunctionCallSyntax
//@[024:0031) |   ├─IdentifierSyntax
//@[024:0031) |   | └─Token(Identifier) |fooTest|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0033) |   └─Token(RightParen) |)|
//@[033:0035) ├─Token(NewLine) |\n\n|

func test() object => loadJsonContent('./repro-data.json')
//@[000:0058) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0009) | ├─IdentifierSyntax
//@[005:0009) | | └─Token(Identifier) |test|
//@[009:0058) | └─TypedLambdaSyntax
//@[009:0011) |   ├─TypedVariableBlockSyntax
//@[009:0010) |   | ├─Token(LeftParen) |(|
//@[010:0011) |   | └─Token(RightParen) |)|
//@[012:0018) |   ├─TypeVariableAccessSyntax
//@[012:0018) |   | └─IdentifierSyntax
//@[012:0018) |   |   └─Token(Identifier) |object|
//@[019:0021) |   ├─Token(Arrow) |=>|
//@[022:0058) |   └─FunctionCallSyntax
//@[022:0037) |     ├─IdentifierSyntax
//@[022:0037) |     | └─Token(Identifier) |loadJsonContent|
//@[037:0038) |     ├─Token(LeftParen) |(|
//@[038:0057) |     ├─FunctionArgumentSyntax
//@[038:0057) |     | └─StringSyntax
//@[038:0057) |     |   └─Token(StringComplete) |'./repro-data.json'|
//@[057:0058) |     └─Token(RightParen) |)|
//@[058:0059) ├─Token(NewLine) |\n|
func test2() string => loadTextContent('./repro-data.json')
//@[000:0059) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |test2|
//@[010:0059) | └─TypedLambdaSyntax
//@[010:0012) |   ├─TypedVariableBlockSyntax
//@[010:0011) |   | ├─Token(LeftParen) |(|
//@[011:0012) |   | └─Token(RightParen) |)|
//@[013:0019) |   ├─TypeVariableAccessSyntax
//@[013:0019) |   | └─IdentifierSyntax
//@[013:0019) |   |   └─Token(Identifier) |string|
//@[020:0022) |   ├─Token(Arrow) |=>|
//@[023:0059) |   └─FunctionCallSyntax
//@[023:0038) |     ├─IdentifierSyntax
//@[023:0038) |     | └─Token(Identifier) |loadTextContent|
//@[038:0039) |     ├─Token(LeftParen) |(|
//@[039:0058) |     ├─FunctionArgumentSyntax
//@[039:0058) |     | └─StringSyntax
//@[039:0058) |     |   └─Token(StringComplete) |'./repro-data.json'|
//@[058:0059) |     └─Token(RightParen) |)|
//@[059:0060) ├─Token(NewLine) |\n|
func test3() object => loadYamlContent('./repro-data.json')
//@[000:0059) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |test3|
//@[010:0059) | └─TypedLambdaSyntax
//@[010:0012) |   ├─TypedVariableBlockSyntax
//@[010:0011) |   | ├─Token(LeftParen) |(|
//@[011:0012) |   | └─Token(RightParen) |)|
//@[013:0019) |   ├─TypeVariableAccessSyntax
//@[013:0019) |   | └─IdentifierSyntax
//@[013:0019) |   |   └─Token(Identifier) |object|
//@[020:0022) |   ├─Token(Arrow) |=>|
//@[023:0059) |   └─FunctionCallSyntax
//@[023:0038) |     ├─IdentifierSyntax
//@[023:0038) |     | └─Token(Identifier) |loadYamlContent|
//@[038:0039) |     ├─Token(LeftParen) |(|
//@[039:0058) |     ├─FunctionArgumentSyntax
//@[039:0058) |     | └─StringSyntax
//@[039:0058) |     |   └─Token(StringComplete) |'./repro-data.json'|
//@[058:0059) |     └─Token(RightParen) |)|
//@[059:0060) ├─Token(NewLine) |\n|
func test4() string => loadFileAsBase64('./repro-data.json')
//@[000:0060) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0010) | ├─IdentifierSyntax
//@[005:0010) | | └─Token(Identifier) |test4|
//@[010:0060) | └─TypedLambdaSyntax
//@[010:0012) |   ├─TypedVariableBlockSyntax
//@[010:0011) |   | ├─Token(LeftParen) |(|
//@[011:0012) |   | └─Token(RightParen) |)|
//@[013:0019) |   ├─TypeVariableAccessSyntax
//@[013:0019) |   | └─IdentifierSyntax
//@[013:0019) |   |   └─Token(Identifier) |string|
//@[020:0022) |   ├─Token(Arrow) |=>|
//@[023:0060) |   └─FunctionCallSyntax
//@[023:0039) |     ├─IdentifierSyntax
//@[023:0039) |     | └─Token(Identifier) |loadFileAsBase64|
//@[039:0040) |     ├─Token(LeftParen) |(|
//@[040:0059) |     ├─FunctionArgumentSyntax
//@[040:0059) |     | └─StringSyntax
//@[040:0059) |     |   └─Token(StringComplete) |'./repro-data.json'|
//@[059:0060) |     └─Token(RightParen) |)|
//@[060:0062) ├─Token(NewLine) |\n\n|

// validate formatter works (https://github.com/Azure/bicep/issues/12913)
//@[073:0074) ├─Token(NewLine) |\n|
func a(____________________________________________________________________________________________ string) string => 'a'
//@[000:0121) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0006) | ├─IdentifierSyntax
//@[005:0006) | | └─Token(Identifier) |a|
//@[006:0121) | └─TypedLambdaSyntax
//@[006:0107) |   ├─TypedVariableBlockSyntax
//@[006:0007) |   | ├─Token(LeftParen) |(|
//@[007:0106) |   | ├─TypedLocalVariableSyntax
//@[007:0099) |   | | ├─IdentifierSyntax
//@[007:0099) |   | | | └─Token(Identifier) |____________________________________________________________________________________________|
//@[100:0106) |   | | └─TypeVariableAccessSyntax
//@[100:0106) |   | |   └─IdentifierSyntax
//@[100:0106) |   | |     └─Token(Identifier) |string|
//@[106:0107) |   | └─Token(RightParen) |)|
//@[108:0114) |   ├─TypeVariableAccessSyntax
//@[108:0114) |   | └─IdentifierSyntax
//@[108:0114) |   |   └─Token(Identifier) |string|
//@[115:0117) |   ├─Token(Arrow) |=>|
//@[118:0121) |   └─StringSyntax
//@[118:0121) |     └─Token(StringComplete) |'a'|
//@[121:0122) ├─Token(NewLine) |\n|
func b(longParameterName1 string, longParameterName2 string, longParameterName3 string, longParameterName4 string) string => 'b'
//@[000:0128) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0006) | ├─IdentifierSyntax
//@[005:0006) | | └─Token(Identifier) |b|
//@[006:0128) | └─TypedLambdaSyntax
//@[006:0114) |   ├─TypedVariableBlockSyntax
//@[006:0007) |   | ├─Token(LeftParen) |(|
//@[007:0032) |   | ├─TypedLocalVariableSyntax
//@[007:0025) |   | | ├─IdentifierSyntax
//@[007:0025) |   | | | └─Token(Identifier) |longParameterName1|
//@[026:0032) |   | | └─TypeVariableAccessSyntax
//@[026:0032) |   | |   └─IdentifierSyntax
//@[026:0032) |   | |     └─Token(Identifier) |string|
//@[032:0033) |   | ├─Token(Comma) |,|
//@[034:0059) |   | ├─TypedLocalVariableSyntax
//@[034:0052) |   | | ├─IdentifierSyntax
//@[034:0052) |   | | | └─Token(Identifier) |longParameterName2|
//@[053:0059) |   | | └─TypeVariableAccessSyntax
//@[053:0059) |   | |   └─IdentifierSyntax
//@[053:0059) |   | |     └─Token(Identifier) |string|
//@[059:0060) |   | ├─Token(Comma) |,|
//@[061:0086) |   | ├─TypedLocalVariableSyntax
//@[061:0079) |   | | ├─IdentifierSyntax
//@[061:0079) |   | | | └─Token(Identifier) |longParameterName3|
//@[080:0086) |   | | └─TypeVariableAccessSyntax
//@[080:0086) |   | |   └─IdentifierSyntax
//@[080:0086) |   | |     └─Token(Identifier) |string|
//@[086:0087) |   | ├─Token(Comma) |,|
//@[088:0113) |   | ├─TypedLocalVariableSyntax
//@[088:0106) |   | | ├─IdentifierSyntax
//@[088:0106) |   | | | └─Token(Identifier) |longParameterName4|
//@[107:0113) |   | | └─TypeVariableAccessSyntax
//@[107:0113) |   | |   └─IdentifierSyntax
//@[107:0113) |   | |     └─Token(Identifier) |string|
//@[113:0114) |   | └─Token(RightParen) |)|
//@[115:0121) |   ├─TypeVariableAccessSyntax
//@[115:0121) |   | └─IdentifierSyntax
//@[115:0121) |   |   └─Token(Identifier) |string|
//@[122:0124) |   ├─Token(Arrow) |=>|
//@[125:0128) |   └─StringSyntax
//@[125:0128) |     └─Token(StringComplete) |'b'|
//@[128:0130) ├─Token(NewLine) |\n\n|

func buildUrlMultiLine(
//@[000:0158) ├─FunctionDeclarationSyntax
//@[000:0004) | ├─Token(Identifier) |func|
//@[005:0022) | ├─IdentifierSyntax
//@[005:0022) | | └─Token(Identifier) |buildUrlMultiLine|
//@[022:0158) | └─TypedLambdaSyntax
//@[022:0072) |   ├─TypedVariableBlockSyntax
//@[022:0023) |   | ├─Token(LeftParen) |(|
//@[023:0024) |   | ├─Token(NewLine) |\n|
  https bool,
//@[002:0012) |   | ├─TypedLocalVariableSyntax
//@[002:0007) |   | | ├─IdentifierSyntax
//@[002:0007) |   | | | └─Token(Identifier) |https|
//@[008:0012) |   | | └─TypeVariableAccessSyntax
//@[008:0012) |   | |   └─IdentifierSyntax
//@[008:0012) |   | |     └─Token(Identifier) |bool|
//@[012:0013) |   | ├─Token(Comma) |,|
//@[013:0014) |   | ├─Token(NewLine) |\n|
  hostname string,
//@[002:0017) |   | ├─TypedLocalVariableSyntax
//@[002:0010) |   | | ├─IdentifierSyntax
//@[002:0010) |   | | | └─Token(Identifier) |hostname|
//@[011:0017) |   | | └─TypeVariableAccessSyntax
//@[011:0017) |   | |   └─IdentifierSyntax
//@[011:0017) |   | |     └─Token(Identifier) |string|
//@[017:0018) |   | ├─Token(Comma) |,|
//@[018:0019) |   | ├─Token(NewLine) |\n|
  path string
//@[002:0013) |   | ├─TypedLocalVariableSyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |path|
//@[007:0013) |   | | └─TypeVariableAccessSyntax
//@[007:0013) |   | |   └─IdentifierSyntax
//@[007:0013) |   | |     └─Token(Identifier) |string|
//@[013:0014) |   | ├─Token(NewLine) |\n|
) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:0001) |   | └─Token(RightParen) |)|
//@[002:0008) |   ├─TypeVariableAccessSyntax
//@[002:0008) |   | └─IdentifierSyntax
//@[002:0008) |   |   └─Token(Identifier) |string|
//@[009:0011) |   ├─Token(Arrow) |=>|
//@[012:0087) |   └─StringSyntax
//@[012:0015) |     ├─Token(StringLeftPiece) |'${|
//@[015:0039) |     ├─TernaryOperationSyntax
//@[015:0020) |     | ├─VariableAccessSyntax
//@[015:0020) |     | | └─IdentifierSyntax
//@[015:0020) |     | |   └─Token(Identifier) |https|
//@[021:0022) |     | ├─Token(Question) |?|
//@[023:0030) |     | ├─StringSyntax
//@[023:0030) |     | | └─Token(StringComplete) |'https'|
//@[031:0032) |     | ├─Token(Colon) |:|
//@[033:0039) |     | └─StringSyntax
//@[033:0039) |     |   └─Token(StringComplete) |'http'|
//@[039:0045) |     ├─Token(StringMiddlePiece) |}://${|
//@[045:0053) |     ├─VariableAccessSyntax
//@[045:0053) |     | └─IdentifierSyntax
//@[045:0053) |     |   └─Token(Identifier) |hostname|
//@[053:0056) |     ├─Token(StringMiddlePiece) |}${|
//@[056:0085) |     ├─TernaryOperationSyntax
//@[056:0067) |     | ├─FunctionCallSyntax
//@[056:0061) |     | | ├─IdentifierSyntax
//@[056:0061) |     | | | └─Token(Identifier) |empty|
//@[061:0062) |     | | ├─Token(LeftParen) |(|
//@[062:0066) |     | | ├─FunctionArgumentSyntax
//@[062:0066) |     | | | └─VariableAccessSyntax
//@[062:0066) |     | | |   └─IdentifierSyntax
//@[062:0066) |     | | |     └─Token(Identifier) |path|
//@[066:0067) |     | | └─Token(RightParen) |)|
//@[068:0069) |     | ├─Token(Question) |?|
//@[070:0072) |     | ├─StringSyntax
//@[070:0072) |     | | └─Token(StringComplete) |''|
//@[073:0074) |     | ├─Token(Colon) |:|
//@[075:0085) |     | └─StringSyntax
//@[075:0079) |     |   ├─Token(StringLeftPiece) |'/${|
//@[079:0083) |     |   ├─VariableAccessSyntax
//@[079:0083) |     |   | └─IdentifierSyntax
//@[079:0083) |     |   |   └─Token(Identifier) |path|
//@[083:0085) |     |   └─Token(StringRightPiece) |}'|
//@[085:0087) |     └─Token(StringRightPiece) |}'|
//@[087:0089) ├─Token(NewLine) |\n\n|

output likeExactMatch bool =like('abc', 'abc')
//@[000:0046) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0021) | ├─IdentifierSyntax
//@[007:0021) | | └─Token(Identifier) |likeExactMatch|
//@[022:0026) | ├─TypeVariableAccessSyntax
//@[022:0026) | | └─IdentifierSyntax
//@[022:0026) | |   └─Token(Identifier) |bool|
//@[027:0028) | ├─Token(Assignment) |=|
//@[028:0046) | └─FunctionCallSyntax
//@[028:0032) |   ├─IdentifierSyntax
//@[028:0032) |   | └─Token(Identifier) |like|
//@[032:0033) |   ├─Token(LeftParen) |(|
//@[033:0038) |   ├─FunctionArgumentSyntax
//@[033:0038) |   | └─StringSyntax
//@[033:0038) |   |   └─Token(StringComplete) |'abc'|
//@[038:0039) |   ├─Token(Comma) |,|
//@[040:0045) |   ├─FunctionArgumentSyntax
//@[040:0045) |   | └─StringSyntax
//@[040:0045) |   |   └─Token(StringComplete) |'abc'|
//@[045:0046) |   └─Token(RightParen) |)|
//@[046:0047) ├─Token(NewLine) |\n|
output likeWildCardMatch bool= like ('abcdef', 'a*c*')
//@[000:0054) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0024) | ├─IdentifierSyntax
//@[007:0024) | | └─Token(Identifier) |likeWildCardMatch|
//@[025:0029) | ├─TypeVariableAccessSyntax
//@[025:0029) | | └─IdentifierSyntax
//@[025:0029) | |   └─Token(Identifier) |bool|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0054) | └─FunctionCallSyntax
//@[031:0035) |   ├─IdentifierSyntax
//@[031:0035) |   | └─Token(Identifier) |like|
//@[036:0037) |   ├─Token(LeftParen) |(|
//@[037:0045) |   ├─FunctionArgumentSyntax
//@[037:0045) |   | └─StringSyntax
//@[037:0045) |   |   └─Token(StringComplete) |'abcdef'|
//@[045:0046) |   ├─Token(Comma) |,|
//@[047:0053) |   ├─FunctionArgumentSyntax
//@[047:0053) |   | └─StringSyntax
//@[047:0053) |   |   └─Token(StringComplete) |'a*c*'|
//@[053:0054) |   └─Token(RightParen) |)|
//@[054:0055) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
