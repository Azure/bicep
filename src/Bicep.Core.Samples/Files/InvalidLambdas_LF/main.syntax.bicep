param ids array
//@[000:2036) ProgramSyntax
//@[000:0015) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0009) | ├─IdentifierSyntax
//@[006:0009) | | └─Token(Identifier) |ids|
//@[010:0015) | └─SimpleTypeSyntax
//@[010:0015) |   └─Token(Identifier) |array|
//@[015:0017) ├─Token(NewLine) |\n\n|

var flatten1 = flatten('abc')
//@[000:0029) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |flatten1|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0029) | └─FunctionCallSyntax
//@[015:0022) |   ├─IdentifierSyntax
//@[015:0022) |   | └─Token(Identifier) |flatten|
//@[022:0023) |   ├─Token(LeftParen) |(|
//@[023:0028) |   ├─FunctionArgumentSyntax
//@[023:0028) |   | └─StringSyntax
//@[023:0028) |   |   └─Token(StringComplete) |'abc'|
//@[028:0029) |   └─Token(RightParen) |)|
//@[029:0030) ├─Token(NewLine) |\n|
var flatten2 = flatten(['abc'], 'def')
//@[000:0038) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |flatten2|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0038) | └─FunctionCallSyntax
//@[015:0022) |   ├─IdentifierSyntax
//@[015:0022) |   | └─Token(Identifier) |flatten|
//@[022:0023) |   ├─Token(LeftParen) |(|
//@[023:0030) |   ├─FunctionArgumentSyntax
//@[023:0030) |   | └─ArraySyntax
//@[023:0024) |   |   ├─Token(LeftSquare) |[|
//@[024:0029) |   |   ├─ArrayItemSyntax
//@[024:0029) |   |   | └─StringSyntax
//@[024:0029) |   |   |   └─Token(StringComplete) |'abc'|
//@[029:0030) |   |   └─Token(RightSquare) |]|
//@[030:0031) |   ├─Token(Comma) |,|
//@[032:0037) |   ├─FunctionArgumentSyntax
//@[032:0037) |   | └─StringSyntax
//@[032:0037) |   |   └─Token(StringComplete) |'def'|
//@[037:0038) |   └─Token(RightParen) |)|
//@[038:0040) ├─Token(NewLine) |\n\n|

var map1 = map('abc')
//@[000:0021) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |map1|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0021) | └─FunctionCallSyntax
//@[011:0014) |   ├─IdentifierSyntax
//@[011:0014) |   | └─Token(Identifier) |map|
//@[014:0015) |   ├─Token(LeftParen) |(|
//@[015:0020) |   ├─FunctionArgumentSyntax
//@[015:0020) |   | └─StringSyntax
//@[015:0020) |   |   └─Token(StringComplete) |'abc'|
//@[020:0021) |   └─Token(RightParen) |)|
//@[021:0022) ├─Token(NewLine) |\n|
var map2 = map('abc', 'def')
//@[000:0028) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |map2|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0028) | └─FunctionCallSyntax
//@[011:0014) |   ├─IdentifierSyntax
//@[011:0014) |   | └─Token(Identifier) |map|
//@[014:0015) |   ├─Token(LeftParen) |(|
//@[015:0020) |   ├─FunctionArgumentSyntax
//@[015:0020) |   | └─StringSyntax
//@[015:0020) |   |   └─Token(StringComplete) |'abc'|
//@[020:0021) |   ├─Token(Comma) |,|
//@[022:0027) |   ├─FunctionArgumentSyntax
//@[022:0027) |   | └─StringSyntax
//@[022:0027) |   |   └─Token(StringComplete) |'def'|
//@[027:0028) |   └─Token(RightParen) |)|
//@[028:0029) ├─Token(NewLine) |\n|
var map3 = map(range(0, 10), 'def')
//@[000:0035) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |map3|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0035) | └─FunctionCallSyntax
//@[011:0014) |   ├─IdentifierSyntax
//@[011:0014) |   | └─Token(Identifier) |map|
//@[014:0015) |   ├─Token(LeftParen) |(|
//@[015:0027) |   ├─FunctionArgumentSyntax
//@[015:0027) |   | └─FunctionCallSyntax
//@[015:0020) |   |   ├─IdentifierSyntax
//@[015:0020) |   |   | └─Token(Identifier) |range|
//@[020:0021) |   |   ├─Token(LeftParen) |(|
//@[021:0022) |   |   ├─FunctionArgumentSyntax
//@[021:0022) |   |   | └─IntegerLiteralSyntax
//@[021:0022) |   |   |   └─Token(Integer) |0|
//@[022:0023) |   |   ├─Token(Comma) |,|
//@[024:0026) |   |   ├─FunctionArgumentSyntax
//@[024:0026) |   |   | └─IntegerLiteralSyntax
//@[024:0026) |   |   |   └─Token(Integer) |10|
//@[026:0027) |   |   └─Token(RightParen) |)|
//@[027:0028) |   ├─Token(Comma) |,|
//@[029:0034) |   ├─FunctionArgumentSyntax
//@[029:0034) |   | └─StringSyntax
//@[029:0034) |   |   └─Token(StringComplete) |'def'|
//@[034:0035) |   └─Token(RightParen) |)|
//@[035:0036) ├─Token(NewLine) |\n|
var map4 = map(range(0, 10), () => null)
//@[000:0040) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |map4|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0040) | └─FunctionCallSyntax
//@[011:0014) |   ├─IdentifierSyntax
//@[011:0014) |   | └─Token(Identifier) |map|
//@[014:0015) |   ├─Token(LeftParen) |(|
//@[015:0027) |   ├─FunctionArgumentSyntax
//@[015:0027) |   | └─FunctionCallSyntax
//@[015:0020) |   |   ├─IdentifierSyntax
//@[015:0020) |   |   | └─Token(Identifier) |range|
//@[020:0021) |   |   ├─Token(LeftParen) |(|
//@[021:0022) |   |   ├─FunctionArgumentSyntax
//@[021:0022) |   |   | └─IntegerLiteralSyntax
//@[021:0022) |   |   |   └─Token(Integer) |0|
//@[022:0023) |   |   ├─Token(Comma) |,|
//@[024:0026) |   |   ├─FunctionArgumentSyntax
//@[024:0026) |   |   | └─IntegerLiteralSyntax
//@[024:0026) |   |   |   └─Token(Integer) |10|
//@[026:0027) |   |   └─Token(RightParen) |)|
//@[027:0028) |   ├─Token(Comma) |,|
//@[029:0039) |   ├─FunctionArgumentSyntax
//@[029:0039) |   | └─LambdaSyntax
//@[029:0031) |   |   ├─VariableBlockSyntax
//@[029:0030) |   |   | ├─Token(LeftParen) |(|
//@[030:0031) |   |   | └─Token(RightParen) |)|
//@[032:0034) |   |   ├─Token(Arrow) |=>|
//@[035:0039) |   |   └─NullLiteralSyntax
//@[035:0039) |   |     └─Token(NullKeyword) |null|
//@[039:0040) |   └─Token(RightParen) |)|
//@[040:0042) ├─Token(NewLine) |\n\n|

var filter1 = filter('abc')
//@[000:0027) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |filter1|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0027) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |filter|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0026) |   ├─FunctionArgumentSyntax
//@[021:0026) |   | └─StringSyntax
//@[021:0026) |   |   └─Token(StringComplete) |'abc'|
//@[026:0027) |   └─Token(RightParen) |)|
//@[027:0028) ├─Token(NewLine) |\n|
var filter2 = filter('abc', 'def')
//@[000:0034) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |filter2|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0034) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |filter|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0026) |   ├─FunctionArgumentSyntax
//@[021:0026) |   | └─StringSyntax
//@[021:0026) |   |   └─Token(StringComplete) |'abc'|
//@[026:0027) |   ├─Token(Comma) |,|
//@[028:0033) |   ├─FunctionArgumentSyntax
//@[028:0033) |   | └─StringSyntax
//@[028:0033) |   |   └─Token(StringComplete) |'def'|
//@[033:0034) |   └─Token(RightParen) |)|
//@[034:0035) ├─Token(NewLine) |\n|
var filter3 = filter(range(0, 10), 'def')
//@[000:0041) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |filter3|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0041) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |filter|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0033) |   ├─FunctionArgumentSyntax
//@[021:0033) |   | └─FunctionCallSyntax
//@[021:0026) |   |   ├─IdentifierSyntax
//@[021:0026) |   |   | └─Token(Identifier) |range|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0028) |   |   ├─FunctionArgumentSyntax
//@[027:0028) |   |   | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   └─Token(Integer) |0|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0032) |   |   ├─FunctionArgumentSyntax
//@[030:0032) |   |   | └─IntegerLiteralSyntax
//@[030:0032) |   |   |   └─Token(Integer) |10|
//@[032:0033) |   |   └─Token(RightParen) |)|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0040) |   ├─FunctionArgumentSyntax
//@[035:0040) |   | └─StringSyntax
//@[035:0040) |   |   └─Token(StringComplete) |'def'|
//@[040:0041) |   └─Token(RightParen) |)|
//@[041:0042) ├─Token(NewLine) |\n|
var filter4 = filter(range(0, 10), () => null)
//@[000:0046) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |filter4|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0046) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |filter|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0033) |   ├─FunctionArgumentSyntax
//@[021:0033) |   | └─FunctionCallSyntax
//@[021:0026) |   |   ├─IdentifierSyntax
//@[021:0026) |   |   | └─Token(Identifier) |range|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0028) |   |   ├─FunctionArgumentSyntax
//@[027:0028) |   |   | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   └─Token(Integer) |0|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0032) |   |   ├─FunctionArgumentSyntax
//@[030:0032) |   |   | └─IntegerLiteralSyntax
//@[030:0032) |   |   |   └─Token(Integer) |10|
//@[032:0033) |   |   └─Token(RightParen) |)|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0045) |   ├─FunctionArgumentSyntax
//@[035:0045) |   | └─LambdaSyntax
//@[035:0037) |   |   ├─VariableBlockSyntax
//@[035:0036) |   |   | ├─Token(LeftParen) |(|
//@[036:0037) |   |   | └─Token(RightParen) |)|
//@[038:0040) |   |   ├─Token(Arrow) |=>|
//@[041:0045) |   |   └─NullLiteralSyntax
//@[041:0045) |   |     └─Token(NullKeyword) |null|
//@[045:0046) |   └─Token(RightParen) |)|
//@[046:0047) ├─Token(NewLine) |\n|
var filter5 = filter(range(0, 10), i => i)
//@[000:0042) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |filter5|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0042) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |filter|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0033) |   ├─FunctionArgumentSyntax
//@[021:0033) |   | └─FunctionCallSyntax
//@[021:0026) |   |   ├─IdentifierSyntax
//@[021:0026) |   |   | └─Token(Identifier) |range|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0028) |   |   ├─FunctionArgumentSyntax
//@[027:0028) |   |   | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   └─Token(Integer) |0|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0032) |   |   ├─FunctionArgumentSyntax
//@[030:0032) |   |   | └─IntegerLiteralSyntax
//@[030:0032) |   |   |   └─Token(Integer) |10|
//@[032:0033) |   |   └─Token(RightParen) |)|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0041) |   ├─FunctionArgumentSyntax
//@[035:0041) |   | └─LambdaSyntax
//@[035:0036) |   |   ├─LocalVariableSyntax
//@[035:0036) |   |   | └─IdentifierSyntax
//@[035:0036) |   |   |   └─Token(Identifier) |i|
//@[037:0039) |   |   ├─Token(Arrow) |=>|
//@[040:0041) |   |   └─VariableAccessSyntax
//@[040:0041) |   |     └─IdentifierSyntax
//@[040:0041) |   |       └─Token(Identifier) |i|
//@[041:0042) |   └─Token(RightParen) |)|
//@[042:0043) ├─Token(NewLine) |\n|
var filter6 = filter([true, 'hello!'], i => i)
//@[000:0046) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |filter6|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0046) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |filter|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0037) |   ├─FunctionArgumentSyntax
//@[021:0037) |   | └─ArraySyntax
//@[021:0022) |   |   ├─Token(LeftSquare) |[|
//@[022:0026) |   |   ├─ArrayItemSyntax
//@[022:0026) |   |   | └─BooleanLiteralSyntax
//@[022:0026) |   |   |   └─Token(TrueKeyword) |true|
//@[026:0027) |   |   ├─Token(Comma) |,|
//@[028:0036) |   |   ├─ArrayItemSyntax
//@[028:0036) |   |   | └─StringSyntax
//@[028:0036) |   |   |   └─Token(StringComplete) |'hello!'|
//@[036:0037) |   |   └─Token(RightSquare) |]|
//@[037:0038) |   ├─Token(Comma) |,|
//@[039:0045) |   ├─FunctionArgumentSyntax
//@[039:0045) |   | └─LambdaSyntax
//@[039:0040) |   |   ├─LocalVariableSyntax
//@[039:0040) |   |   | └─IdentifierSyntax
//@[039:0040) |   |   |   └─Token(Identifier) |i|
//@[041:0043) |   |   ├─Token(Arrow) |=>|
//@[044:0045) |   |   └─VariableAccessSyntax
//@[044:0045) |   |     └─IdentifierSyntax
//@[044:0045) |   |       └─Token(Identifier) |i|
//@[045:0046) |   └─Token(RightParen) |)|
//@[046:0048) ├─Token(NewLine) |\n\n|

var sort1 = sort('abc')
//@[000:0023) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |sort1|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0023) | └─FunctionCallSyntax
//@[012:0016) |   ├─IdentifierSyntax
//@[012:0016) |   | └─Token(Identifier) |sort|
//@[016:0017) |   ├─Token(LeftParen) |(|
//@[017:0022) |   ├─FunctionArgumentSyntax
//@[017:0022) |   | └─StringSyntax
//@[017:0022) |   |   └─Token(StringComplete) |'abc'|
//@[022:0023) |   └─Token(RightParen) |)|
//@[023:0024) ├─Token(NewLine) |\n|
var sort2 = sort('abc', 'def')
//@[000:0030) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |sort2|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0030) | └─FunctionCallSyntax
//@[012:0016) |   ├─IdentifierSyntax
//@[012:0016) |   | └─Token(Identifier) |sort|
//@[016:0017) |   ├─Token(LeftParen) |(|
//@[017:0022) |   ├─FunctionArgumentSyntax
//@[017:0022) |   | └─StringSyntax
//@[017:0022) |   |   └─Token(StringComplete) |'abc'|
//@[022:0023) |   ├─Token(Comma) |,|
//@[024:0029) |   ├─FunctionArgumentSyntax
//@[024:0029) |   | └─StringSyntax
//@[024:0029) |   |   └─Token(StringComplete) |'def'|
//@[029:0030) |   └─Token(RightParen) |)|
//@[030:0031) ├─Token(NewLine) |\n|
var sort3 = sort(range(0, 10), 'def')
//@[000:0037) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |sort3|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0037) | └─FunctionCallSyntax
//@[012:0016) |   ├─IdentifierSyntax
//@[012:0016) |   | └─Token(Identifier) |sort|
//@[016:0017) |   ├─Token(LeftParen) |(|
//@[017:0029) |   ├─FunctionArgumentSyntax
//@[017:0029) |   | └─FunctionCallSyntax
//@[017:0022) |   |   ├─IdentifierSyntax
//@[017:0022) |   |   | └─Token(Identifier) |range|
//@[022:0023) |   |   ├─Token(LeftParen) |(|
//@[023:0024) |   |   ├─FunctionArgumentSyntax
//@[023:0024) |   |   | └─IntegerLiteralSyntax
//@[023:0024) |   |   |   └─Token(Integer) |0|
//@[024:0025) |   |   ├─Token(Comma) |,|
//@[026:0028) |   |   ├─FunctionArgumentSyntax
//@[026:0028) |   |   | └─IntegerLiteralSyntax
//@[026:0028) |   |   |   └─Token(Integer) |10|
//@[028:0029) |   |   └─Token(RightParen) |)|
//@[029:0030) |   ├─Token(Comma) |,|
//@[031:0036) |   ├─FunctionArgumentSyntax
//@[031:0036) |   | └─StringSyntax
//@[031:0036) |   |   └─Token(StringComplete) |'def'|
//@[036:0037) |   └─Token(RightParen) |)|
//@[037:0038) ├─Token(NewLine) |\n|
var sort4 = sort(range(0, 10), () => null)
//@[000:0042) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |sort4|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0042) | └─FunctionCallSyntax
//@[012:0016) |   ├─IdentifierSyntax
//@[012:0016) |   | └─Token(Identifier) |sort|
//@[016:0017) |   ├─Token(LeftParen) |(|
//@[017:0029) |   ├─FunctionArgumentSyntax
//@[017:0029) |   | └─FunctionCallSyntax
//@[017:0022) |   |   ├─IdentifierSyntax
//@[017:0022) |   |   | └─Token(Identifier) |range|
//@[022:0023) |   |   ├─Token(LeftParen) |(|
//@[023:0024) |   |   ├─FunctionArgumentSyntax
//@[023:0024) |   |   | └─IntegerLiteralSyntax
//@[023:0024) |   |   |   └─Token(Integer) |0|
//@[024:0025) |   |   ├─Token(Comma) |,|
//@[026:0028) |   |   ├─FunctionArgumentSyntax
//@[026:0028) |   |   | └─IntegerLiteralSyntax
//@[026:0028) |   |   |   └─Token(Integer) |10|
//@[028:0029) |   |   └─Token(RightParen) |)|
//@[029:0030) |   ├─Token(Comma) |,|
//@[031:0041) |   ├─FunctionArgumentSyntax
//@[031:0041) |   | └─LambdaSyntax
//@[031:0033) |   |   ├─VariableBlockSyntax
//@[031:0032) |   |   | ├─Token(LeftParen) |(|
//@[032:0033) |   |   | └─Token(RightParen) |)|
//@[034:0036) |   |   ├─Token(Arrow) |=>|
//@[037:0041) |   |   └─NullLiteralSyntax
//@[037:0041) |   |     └─Token(NullKeyword) |null|
//@[041:0042) |   └─Token(RightParen) |)|
//@[042:0043) ├─Token(NewLine) |\n|
var sort5 = sort(range(0, 10), i => i)
//@[000:0038) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |sort5|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0038) | └─FunctionCallSyntax
//@[012:0016) |   ├─IdentifierSyntax
//@[012:0016) |   | └─Token(Identifier) |sort|
//@[016:0017) |   ├─Token(LeftParen) |(|
//@[017:0029) |   ├─FunctionArgumentSyntax
//@[017:0029) |   | └─FunctionCallSyntax
//@[017:0022) |   |   ├─IdentifierSyntax
//@[017:0022) |   |   | └─Token(Identifier) |range|
//@[022:0023) |   |   ├─Token(LeftParen) |(|
//@[023:0024) |   |   ├─FunctionArgumentSyntax
//@[023:0024) |   |   | └─IntegerLiteralSyntax
//@[023:0024) |   |   |   └─Token(Integer) |0|
//@[024:0025) |   |   ├─Token(Comma) |,|
//@[026:0028) |   |   ├─FunctionArgumentSyntax
//@[026:0028) |   |   | └─IntegerLiteralSyntax
//@[026:0028) |   |   |   └─Token(Integer) |10|
//@[028:0029) |   |   └─Token(RightParen) |)|
//@[029:0030) |   ├─Token(Comma) |,|
//@[031:0037) |   ├─FunctionArgumentSyntax
//@[031:0037) |   | └─LambdaSyntax
//@[031:0032) |   |   ├─LocalVariableSyntax
//@[031:0032) |   |   | └─IdentifierSyntax
//@[031:0032) |   |   |   └─Token(Identifier) |i|
//@[033:0035) |   |   ├─Token(Arrow) |=>|
//@[036:0037) |   |   └─VariableAccessSyntax
//@[036:0037) |   |     └─IdentifierSyntax
//@[036:0037) |   |       └─Token(Identifier) |i|
//@[037:0038) |   └─Token(RightParen) |)|
//@[038:0039) ├─Token(NewLine) |\n|
var sort6 = sort(range(0, 10), (i, j) => i)
//@[000:0043) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |sort6|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0043) | └─FunctionCallSyntax
//@[012:0016) |   ├─IdentifierSyntax
//@[012:0016) |   | └─Token(Identifier) |sort|
//@[016:0017) |   ├─Token(LeftParen) |(|
//@[017:0029) |   ├─FunctionArgumentSyntax
//@[017:0029) |   | └─FunctionCallSyntax
//@[017:0022) |   |   ├─IdentifierSyntax
//@[017:0022) |   |   | └─Token(Identifier) |range|
//@[022:0023) |   |   ├─Token(LeftParen) |(|
//@[023:0024) |   |   ├─FunctionArgumentSyntax
//@[023:0024) |   |   | └─IntegerLiteralSyntax
//@[023:0024) |   |   |   └─Token(Integer) |0|
//@[024:0025) |   |   ├─Token(Comma) |,|
//@[026:0028) |   |   ├─FunctionArgumentSyntax
//@[026:0028) |   |   | └─IntegerLiteralSyntax
//@[026:0028) |   |   |   └─Token(Integer) |10|
//@[028:0029) |   |   └─Token(RightParen) |)|
//@[029:0030) |   ├─Token(Comma) |,|
//@[031:0042) |   ├─FunctionArgumentSyntax
//@[031:0042) |   | └─LambdaSyntax
//@[031:0037) |   |   ├─VariableBlockSyntax
//@[031:0032) |   |   | ├─Token(LeftParen) |(|
//@[032:0033) |   |   | ├─LocalVariableSyntax
//@[032:0033) |   |   | | └─IdentifierSyntax
//@[032:0033) |   |   | |   └─Token(Identifier) |i|
//@[033:0034) |   |   | ├─Token(Comma) |,|
//@[035:0036) |   |   | ├─LocalVariableSyntax
//@[035:0036) |   |   | | └─IdentifierSyntax
//@[035:0036) |   |   | |   └─Token(Identifier) |j|
//@[036:0037) |   |   | └─Token(RightParen) |)|
//@[038:0040) |   |   ├─Token(Arrow) |=>|
//@[041:0042) |   |   └─VariableAccessSyntax
//@[041:0042) |   |     └─IdentifierSyntax
//@[041:0042) |   |       └─Token(Identifier) |i|
//@[042:0043) |   └─Token(RightParen) |)|
//@[043:0045) ├─Token(NewLine) |\n\n|

var reduce1 = reduce('abc')
//@[000:0027) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |reduce1|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0027) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |reduce|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0026) |   ├─FunctionArgumentSyntax
//@[021:0026) |   | └─StringSyntax
//@[021:0026) |   |   └─Token(StringComplete) |'abc'|
//@[026:0027) |   └─Token(RightParen) |)|
//@[027:0028) ├─Token(NewLine) |\n|
var reduce2 = reduce('abc', 'def', 'ghi')
//@[000:0041) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |reduce2|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0041) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |reduce|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0026) |   ├─FunctionArgumentSyntax
//@[021:0026) |   | └─StringSyntax
//@[021:0026) |   |   └─Token(StringComplete) |'abc'|
//@[026:0027) |   ├─Token(Comma) |,|
//@[028:0033) |   ├─FunctionArgumentSyntax
//@[028:0033) |   | └─StringSyntax
//@[028:0033) |   |   └─Token(StringComplete) |'def'|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0040) |   ├─FunctionArgumentSyntax
//@[035:0040) |   | └─StringSyntax
//@[035:0040) |   |   └─Token(StringComplete) |'ghi'|
//@[040:0041) |   └─Token(RightParen) |)|
//@[041:0042) ├─Token(NewLine) |\n|
var reduce3 = reduce(range(0, 10), 0, 'def')
//@[000:0044) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |reduce3|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0044) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |reduce|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0033) |   ├─FunctionArgumentSyntax
//@[021:0033) |   | └─FunctionCallSyntax
//@[021:0026) |   |   ├─IdentifierSyntax
//@[021:0026) |   |   | └─Token(Identifier) |range|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0028) |   |   ├─FunctionArgumentSyntax
//@[027:0028) |   |   | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   └─Token(Integer) |0|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0032) |   |   ├─FunctionArgumentSyntax
//@[030:0032) |   |   | └─IntegerLiteralSyntax
//@[030:0032) |   |   |   └─Token(Integer) |10|
//@[032:0033) |   |   └─Token(RightParen) |)|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0036) |   ├─FunctionArgumentSyntax
//@[035:0036) |   | └─IntegerLiteralSyntax
//@[035:0036) |   |   └─Token(Integer) |0|
//@[036:0037) |   ├─Token(Comma) |,|
//@[038:0043) |   ├─FunctionArgumentSyntax
//@[038:0043) |   | └─StringSyntax
//@[038:0043) |   |   └─Token(StringComplete) |'def'|
//@[043:0044) |   └─Token(RightParen) |)|
//@[044:0045) ├─Token(NewLine) |\n|
var reduce4 = reduce(range(0, 10), 0, () => null)
//@[000:0049) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |reduce4|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0049) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |reduce|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0033) |   ├─FunctionArgumentSyntax
//@[021:0033) |   | └─FunctionCallSyntax
//@[021:0026) |   |   ├─IdentifierSyntax
//@[021:0026) |   |   | └─Token(Identifier) |range|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0028) |   |   ├─FunctionArgumentSyntax
//@[027:0028) |   |   | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   └─Token(Integer) |0|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0032) |   |   ├─FunctionArgumentSyntax
//@[030:0032) |   |   | └─IntegerLiteralSyntax
//@[030:0032) |   |   |   └─Token(Integer) |10|
//@[032:0033) |   |   └─Token(RightParen) |)|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0036) |   ├─FunctionArgumentSyntax
//@[035:0036) |   | └─IntegerLiteralSyntax
//@[035:0036) |   |   └─Token(Integer) |0|
//@[036:0037) |   ├─Token(Comma) |,|
//@[038:0048) |   ├─FunctionArgumentSyntax
//@[038:0048) |   | └─LambdaSyntax
//@[038:0040) |   |   ├─VariableBlockSyntax
//@[038:0039) |   |   | ├─Token(LeftParen) |(|
//@[039:0040) |   |   | └─Token(RightParen) |)|
//@[041:0043) |   |   ├─Token(Arrow) |=>|
//@[044:0048) |   |   └─NullLiteralSyntax
//@[044:0048) |   |     └─Token(NullKeyword) |null|
//@[048:0049) |   └─Token(RightParen) |)|
//@[049:0050) ├─Token(NewLine) |\n|
var reduce5 = reduce(range(0, 10), 0, i => i)
//@[000:0045) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |reduce5|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0045) | └─FunctionCallSyntax
//@[014:0020) |   ├─IdentifierSyntax
//@[014:0020) |   | └─Token(Identifier) |reduce|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0033) |   ├─FunctionArgumentSyntax
//@[021:0033) |   | └─FunctionCallSyntax
//@[021:0026) |   |   ├─IdentifierSyntax
//@[021:0026) |   |   | └─Token(Identifier) |range|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0028) |   |   ├─FunctionArgumentSyntax
//@[027:0028) |   |   | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   └─Token(Integer) |0|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0032) |   |   ├─FunctionArgumentSyntax
//@[030:0032) |   |   | └─IntegerLiteralSyntax
//@[030:0032) |   |   |   └─Token(Integer) |10|
//@[032:0033) |   |   └─Token(RightParen) |)|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0036) |   ├─FunctionArgumentSyntax
//@[035:0036) |   | └─IntegerLiteralSyntax
//@[035:0036) |   |   └─Token(Integer) |0|
//@[036:0037) |   ├─Token(Comma) |,|
//@[038:0044) |   ├─FunctionArgumentSyntax
//@[038:0044) |   | └─LambdaSyntax
//@[038:0039) |   |   ├─LocalVariableSyntax
//@[038:0039) |   |   | └─IdentifierSyntax
//@[038:0039) |   |   |   └─Token(Identifier) |i|
//@[040:0042) |   |   ├─Token(Arrow) |=>|
//@[043:0044) |   |   └─VariableAccessSyntax
//@[043:0044) |   |     └─IdentifierSyntax
//@[043:0044) |   |       └─Token(Identifier) |i|
//@[044:0045) |   └─Token(RightParen) |)|
//@[045:0047) ├─Token(NewLine) |\n\n|

var ternary = map([123], true ? i => '${i}' : i => 'hello!')
//@[000:0060) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |ternary|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0060) | └─FunctionCallSyntax
//@[014:0017) |   ├─IdentifierSyntax
//@[014:0017) |   | └─Token(Identifier) |map|
//@[017:0018) |   ├─Token(LeftParen) |(|
//@[018:0023) |   ├─FunctionArgumentSyntax
//@[018:0023) |   | └─ArraySyntax
//@[018:0019) |   |   ├─Token(LeftSquare) |[|
//@[019:0022) |   |   ├─ArrayItemSyntax
//@[019:0022) |   |   | └─IntegerLiteralSyntax
//@[019:0022) |   |   |   └─Token(Integer) |123|
//@[022:0023) |   |   └─Token(RightSquare) |]|
//@[023:0024) |   ├─Token(Comma) |,|
//@[025:0059) |   ├─FunctionArgumentSyntax
//@[025:0059) |   | └─TernaryOperationSyntax
//@[025:0029) |   |   ├─BooleanLiteralSyntax
//@[025:0029) |   |   | └─Token(TrueKeyword) |true|
//@[030:0031) |   |   ├─Token(Question) |?|
//@[032:0043) |   |   ├─LambdaSyntax
//@[032:0033) |   |   | ├─LocalVariableSyntax
//@[032:0033) |   |   | | └─IdentifierSyntax
//@[032:0033) |   |   | |   └─Token(Identifier) |i|
//@[034:0036) |   |   | ├─Token(Arrow) |=>|
//@[037:0043) |   |   | └─StringSyntax
//@[037:0040) |   |   |   ├─Token(StringLeftPiece) |'${|
//@[040:0041) |   |   |   ├─VariableAccessSyntax
//@[040:0041) |   |   |   | └─IdentifierSyntax
//@[040:0041) |   |   |   |   └─Token(Identifier) |i|
//@[041:0043) |   |   |   └─Token(StringRightPiece) |}'|
//@[044:0045) |   |   ├─Token(Colon) |:|
//@[046:0059) |   |   └─LambdaSyntax
//@[046:0047) |   |     ├─LocalVariableSyntax
//@[046:0047) |   |     | └─IdentifierSyntax
//@[046:0047) |   |     |   └─Token(Identifier) |i|
//@[048:0050) |   |     ├─Token(Arrow) |=>|
//@[051:0059) |   |     └─StringSyntax
//@[051:0059) |   |       └─Token(StringComplete) |'hello!'|
//@[059:0060) |   └─Token(RightParen) |)|
//@[060:0062) ├─Token(NewLine) |\n\n|

var outsideArgs = i => 123
//@[000:0026) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |outsideArgs|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0026) | └─LambdaSyntax
//@[018:0019) |   ├─LocalVariableSyntax
//@[018:0019) |   | └─IdentifierSyntax
//@[018:0019) |   |   └─Token(Identifier) |i|
//@[020:0022) |   ├─Token(Arrow) |=>|
//@[023:0026) |   └─IntegerLiteralSyntax
//@[023:0026) |     └─Token(Integer) |123|
//@[026:0027) ├─Token(NewLine) |\n|
var outsideArgs2 = (x, y, z) => '${x}${y}${z}'
//@[000:0046) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |outsideArgs2|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0046) | └─LambdaSyntax
//@[019:0028) |   ├─VariableBlockSyntax
//@[019:0020) |   | ├─Token(LeftParen) |(|
//@[020:0021) |   | ├─LocalVariableSyntax
//@[020:0021) |   | | └─IdentifierSyntax
//@[020:0021) |   | |   └─Token(Identifier) |x|
//@[021:0022) |   | ├─Token(Comma) |,|
//@[023:0024) |   | ├─LocalVariableSyntax
//@[023:0024) |   | | └─IdentifierSyntax
//@[023:0024) |   | |   └─Token(Identifier) |y|
//@[024:0025) |   | ├─Token(Comma) |,|
//@[026:0027) |   | ├─LocalVariableSyntax
//@[026:0027) |   | | └─IdentifierSyntax
//@[026:0027) |   | |   └─Token(Identifier) |z|
//@[027:0028) |   | └─Token(RightParen) |)|
//@[029:0031) |   ├─Token(Arrow) |=>|
//@[032:0046) |   └─StringSyntax
//@[032:0035) |     ├─Token(StringLeftPiece) |'${|
//@[035:0036) |     ├─VariableAccessSyntax
//@[035:0036) |     | └─IdentifierSyntax
//@[035:0036) |     |   └─Token(Identifier) |x|
//@[036:0039) |     ├─Token(StringMiddlePiece) |}${|
//@[039:0040) |     ├─VariableAccessSyntax
//@[039:0040) |     | └─IdentifierSyntax
//@[039:0040) |     |   └─Token(Identifier) |y|
//@[040:0043) |     ├─Token(StringMiddlePiece) |}${|
//@[043:0044) |     ├─VariableAccessSyntax
//@[043:0044) |     | └─IdentifierSyntax
//@[043:0044) |     |   └─Token(Identifier) |z|
//@[044:0046) |     └─Token(StringRightPiece) |}'|
//@[046:0047) ├─Token(NewLine) |\n|
var partial = i =>
//@[000:0018) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |partial|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0018) | └─LambdaSyntax
//@[014:0015) |   ├─LocalVariableSyntax
//@[014:0015) |   | └─IdentifierSyntax
//@[014:0015) |   |   └─Token(Identifier) |i|
//@[016:0018) |   ├─Token(Arrow) |=>|
//@[018:0018) |   └─SkippedTriviaSyntax
//@[018:0021) ├─Token(NewLine) |\n\n\n|


var inObject = {
//@[000:0030) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |inObject|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0030) | └─ObjectSyntax
//@[015:0016) |   ├─Token(LeftBrace) |{|
//@[016:0017) |   ├─Token(NewLine) |\n|
  a: i => i
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |a|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0011) |   | └─LambdaSyntax
//@[005:0006) |   |   ├─LocalVariableSyntax
//@[005:0006) |   |   | └─IdentifierSyntax
//@[005:0006) |   |   |   └─Token(Identifier) |i|
//@[007:0009) |   |   ├─Token(Arrow) |=>|
//@[010:0011) |   |   └─VariableAccessSyntax
//@[010:0011) |   |     └─IdentifierSyntax
//@[010:0011) |   |       └─Token(Identifier) |i|
//@[011:0012) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

var inArray = [
//@[000:0035) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |inArray|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0035) | └─ArraySyntax
//@[014:0015) |   ├─Token(LeftSquare) |[|
//@[015:0016) |   ├─Token(NewLine) |\n|
  i => i
//@[002:0008) |   ├─ArrayItemSyntax
//@[002:0008) |   | └─LambdaSyntax
//@[002:0003) |   |   ├─LocalVariableSyntax
//@[002:0003) |   |   | └─IdentifierSyntax
//@[002:0003) |   |   |   └─Token(Identifier) |i|
//@[004:0006) |   |   ├─Token(Arrow) |=>|
//@[007:0008) |   |   └─VariableAccessSyntax
//@[007:0008) |   |     └─IdentifierSyntax
//@[007:0008) |   |       └─Token(Identifier) |i|
//@[008:0009) |   ├─Token(NewLine) |\n|
  j => j
//@[002:0008) |   ├─ArrayItemSyntax
//@[002:0008) |   | └─LambdaSyntax
//@[002:0003) |   |   ├─LocalVariableSyntax
//@[002:0003) |   |   | └─IdentifierSyntax
//@[002:0003) |   |   |   └─Token(Identifier) |j|
//@[004:0006) |   |   ├─Token(Arrow) |=>|
//@[007:0008) |   |   └─VariableAccessSyntax
//@[007:0008) |   |     └─IdentifierSyntax
//@[007:0008) |   |       └─Token(Identifier) |j|
//@[008:0009) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

resource stg 'Microsoft.Storage/storageAccounts@2021-09-01' = [for i in range(0, 2): {
//@[000:0194) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0012) | ├─IdentifierSyntax
//@[009:0012) | | └─Token(Identifier) |stg|
//@[013:0059) | ├─StringSyntax
//@[013:0059) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2021-09-01'|
//@[060:0061) | ├─Token(Assignment) |=|
//@[062:0194) | └─ForSyntax
//@[062:0063) |   ├─Token(LeftSquare) |[|
//@[063:0066) |   ├─Token(Identifier) |for|
//@[067:0068) |   ├─LocalVariableSyntax
//@[067:0068) |   | └─IdentifierSyntax
//@[067:0068) |   |   └─Token(Identifier) |i|
//@[069:0071) |   ├─Token(Identifier) |in|
//@[072:0083) |   ├─FunctionCallSyntax
//@[072:0077) |   | ├─IdentifierSyntax
//@[072:0077) |   | | └─Token(Identifier) |range|
//@[077:0078) |   | ├─Token(LeftParen) |(|
//@[078:0079) |   | ├─FunctionArgumentSyntax
//@[078:0079) |   | | └─IntegerLiteralSyntax
//@[078:0079) |   | |   └─Token(Integer) |0|
//@[079:0080) |   | ├─Token(Comma) |,|
//@[081:0082) |   | ├─FunctionArgumentSyntax
//@[081:0082) |   | | └─IntegerLiteralSyntax
//@[081:0082) |   | |   └─Token(Integer) |2|
//@[082:0083) |   | └─Token(RightParen) |)|
//@[083:0084) |   ├─Token(Colon) |:|
//@[085:0193) |   ├─ObjectSyntax
//@[085:0086) |   | ├─Token(LeftBrace) |{|
//@[086:0087) |   | ├─Token(NewLine) |\n|
  name: 'antteststg${i}'
//@[002:0024) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0024) |   | | └─StringSyntax
//@[008:0021) |   | |   ├─Token(StringLeftPiece) |'antteststg${|
//@[021:0022) |   | |   ├─VariableAccessSyntax
//@[021:0022) |   | |   | └─IdentifierSyntax
//@[021:0022) |   | |   |   └─Token(Identifier) |i|
//@[022:0024) |   | |   └─Token(StringRightPiece) |}'|
//@[024:0025) |   | ├─Token(NewLine) |\n|
  location: 'West US'
//@[002:0021) |   | ├─ObjectPropertySyntax
//@[002:0010) |   | | ├─IdentifierSyntax
//@[002:0010) |   | | | └─Token(Identifier) |location|
//@[010:0011) |   | | ├─Token(Colon) |:|
//@[012:0021) |   | | └─StringSyntax
//@[012:0021) |   | |   └─Token(StringComplete) |'West US'|
//@[021:0022) |   | ├─Token(NewLine) |\n|
  sku: {
//@[002:0037) |   | ├─ObjectPropertySyntax
//@[002:0005) |   | | ├─IdentifierSyntax
//@[002:0005) |   | | | └─Token(Identifier) |sku|
//@[005:0006) |   | | ├─Token(Colon) |:|
//@[007:0037) |   | | └─ObjectSyntax
//@[007:0008) |   | |   ├─Token(LeftBrace) |{|
//@[008:0009) |   | |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[004:0024) |   | |   ├─ObjectPropertySyntax
//@[004:0008) |   | |   | ├─IdentifierSyntax
//@[004:0008) |   | |   | | └─Token(Identifier) |name|
//@[008:0009) |   | |   | ├─Token(Colon) |:|
//@[010:0024) |   | |   | └─StringSyntax
//@[010:0024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:0025) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0004) |   | ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[002:0019) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |kind|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0019) |   | | └─StringSyntax
//@[008:0019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:0020) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0004) ├─Token(NewLine) |\n\n|

output stgKeys array = map(range(0, 2), i => stg[i].listKeys().keys[0].value)
//@[000:0077) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0014) | ├─IdentifierSyntax
//@[007:0014) | | └─Token(Identifier) |stgKeys|
//@[015:0020) | ├─SimpleTypeSyntax
//@[015:0020) | | └─Token(Identifier) |array|
//@[021:0022) | ├─Token(Assignment) |=|
//@[023:0077) | └─FunctionCallSyntax
//@[023:0026) |   ├─IdentifierSyntax
//@[023:0026) |   | └─Token(Identifier) |map|
//@[026:0027) |   ├─Token(LeftParen) |(|
//@[027:0038) |   ├─FunctionArgumentSyntax
//@[027:0038) |   | └─FunctionCallSyntax
//@[027:0032) |   |   ├─IdentifierSyntax
//@[027:0032) |   |   | └─Token(Identifier) |range|
//@[032:0033) |   |   ├─Token(LeftParen) |(|
//@[033:0034) |   |   ├─FunctionArgumentSyntax
//@[033:0034) |   |   | └─IntegerLiteralSyntax
//@[033:0034) |   |   |   └─Token(Integer) |0|
//@[034:0035) |   |   ├─Token(Comma) |,|
//@[036:0037) |   |   ├─FunctionArgumentSyntax
//@[036:0037) |   |   | └─IntegerLiteralSyntax
//@[036:0037) |   |   |   └─Token(Integer) |2|
//@[037:0038) |   |   └─Token(RightParen) |)|
//@[038:0039) |   ├─Token(Comma) |,|
//@[040:0076) |   ├─FunctionArgumentSyntax
//@[040:0076) |   | └─LambdaSyntax
//@[040:0041) |   |   ├─LocalVariableSyntax
//@[040:0041) |   |   | └─IdentifierSyntax
//@[040:0041) |   |   |   └─Token(Identifier) |i|
//@[042:0044) |   |   ├─Token(Arrow) |=>|
//@[045:0076) |   |   └─PropertyAccessSyntax
//@[045:0070) |   |     ├─ArrayAccessSyntax
//@[045:0067) |   |     | ├─PropertyAccessSyntax
//@[045:0062) |   |     | | ├─InstanceFunctionCallSyntax
//@[045:0051) |   |     | | | ├─ArrayAccessSyntax
//@[045:0048) |   |     | | | | ├─VariableAccessSyntax
//@[045:0048) |   |     | | | | | └─IdentifierSyntax
//@[045:0048) |   |     | | | | |   └─Token(Identifier) |stg|
//@[048:0049) |   |     | | | | ├─Token(LeftSquare) |[|
//@[049:0050) |   |     | | | | ├─VariableAccessSyntax
//@[049:0050) |   |     | | | | | └─IdentifierSyntax
//@[049:0050) |   |     | | | | |   └─Token(Identifier) |i|
//@[050:0051) |   |     | | | | └─Token(RightSquare) |]|
//@[051:0052) |   |     | | | ├─Token(Dot) |.|
//@[052:0060) |   |     | | | ├─IdentifierSyntax
//@[052:0060) |   |     | | | | └─Token(Identifier) |listKeys|
//@[060:0061) |   |     | | | ├─Token(LeftParen) |(|
//@[061:0062) |   |     | | | └─Token(RightParen) |)|
//@[062:0063) |   |     | | ├─Token(Dot) |.|
//@[063:0067) |   |     | | └─IdentifierSyntax
//@[063:0067) |   |     | |   └─Token(Identifier) |keys|
//@[067:0068) |   |     | ├─Token(LeftSquare) |[|
//@[068:0069) |   |     | ├─IntegerLiteralSyntax
//@[068:0069) |   |     | | └─Token(Integer) |0|
//@[069:0070) |   |     | └─Token(RightSquare) |]|
//@[070:0071) |   |     ├─Token(Dot) |.|
//@[071:0076) |   |     └─IdentifierSyntax
//@[071:0076) |   |       └─Token(Identifier) |value|
//@[076:0077) |   └─Token(RightParen) |)|
//@[077:0078) ├─Token(NewLine) |\n|
output stgKeys2 array = map(range(0, 2), j => stg[((j + 2) % 123)].listKeys().keys[0].value)
//@[000:0092) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |stgKeys2|
//@[016:0021) | ├─SimpleTypeSyntax
//@[016:0021) | | └─Token(Identifier) |array|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0092) | └─FunctionCallSyntax
//@[024:0027) |   ├─IdentifierSyntax
//@[024:0027) |   | └─Token(Identifier) |map|
//@[027:0028) |   ├─Token(LeftParen) |(|
//@[028:0039) |   ├─FunctionArgumentSyntax
//@[028:0039) |   | └─FunctionCallSyntax
//@[028:0033) |   |   ├─IdentifierSyntax
//@[028:0033) |   |   | └─Token(Identifier) |range|
//@[033:0034) |   |   ├─Token(LeftParen) |(|
//@[034:0035) |   |   ├─FunctionArgumentSyntax
//@[034:0035) |   |   | └─IntegerLiteralSyntax
//@[034:0035) |   |   |   └─Token(Integer) |0|
//@[035:0036) |   |   ├─Token(Comma) |,|
//@[037:0038) |   |   ├─FunctionArgumentSyntax
//@[037:0038) |   |   | └─IntegerLiteralSyntax
//@[037:0038) |   |   |   └─Token(Integer) |2|
//@[038:0039) |   |   └─Token(RightParen) |)|
//@[039:0040) |   ├─Token(Comma) |,|
//@[041:0091) |   ├─FunctionArgumentSyntax
//@[041:0091) |   | └─LambdaSyntax
//@[041:0042) |   |   ├─LocalVariableSyntax
//@[041:0042) |   |   | └─IdentifierSyntax
//@[041:0042) |   |   |   └─Token(Identifier) |j|
//@[043:0045) |   |   ├─Token(Arrow) |=>|
//@[046:0091) |   |   └─PropertyAccessSyntax
//@[046:0085) |   |     ├─ArrayAccessSyntax
//@[046:0082) |   |     | ├─PropertyAccessSyntax
//@[046:0077) |   |     | | ├─InstanceFunctionCallSyntax
//@[046:0066) |   |     | | | ├─ArrayAccessSyntax
//@[046:0049) |   |     | | | | ├─VariableAccessSyntax
//@[046:0049) |   |     | | | | | └─IdentifierSyntax
//@[046:0049) |   |     | | | | |   └─Token(Identifier) |stg|
//@[049:0050) |   |     | | | | ├─Token(LeftSquare) |[|
//@[050:0065) |   |     | | | | ├─ParenthesizedExpressionSyntax
//@[050:0051) |   |     | | | | | ├─Token(LeftParen) |(|
//@[051:0064) |   |     | | | | | ├─BinaryOperationSyntax
//@[051:0058) |   |     | | | | | | ├─ParenthesizedExpressionSyntax
//@[051:0052) |   |     | | | | | | | ├─Token(LeftParen) |(|
//@[052:0057) |   |     | | | | | | | ├─BinaryOperationSyntax
//@[052:0053) |   |     | | | | | | | | ├─VariableAccessSyntax
//@[052:0053) |   |     | | | | | | | | | └─IdentifierSyntax
//@[052:0053) |   |     | | | | | | | | |   └─Token(Identifier) |j|
//@[054:0055) |   |     | | | | | | | | ├─Token(Plus) |+|
//@[056:0057) |   |     | | | | | | | | └─IntegerLiteralSyntax
//@[056:0057) |   |     | | | | | | | |   └─Token(Integer) |2|
//@[057:0058) |   |     | | | | | | | └─Token(RightParen) |)|
//@[059:0060) |   |     | | | | | | ├─Token(Modulo) |%|
//@[061:0064) |   |     | | | | | | └─IntegerLiteralSyntax
//@[061:0064) |   |     | | | | | |   └─Token(Integer) |123|
//@[064:0065) |   |     | | | | | └─Token(RightParen) |)|
//@[065:0066) |   |     | | | | └─Token(RightSquare) |]|
//@[066:0067) |   |     | | | ├─Token(Dot) |.|
//@[067:0075) |   |     | | | ├─IdentifierSyntax
//@[067:0075) |   |     | | | | └─Token(Identifier) |listKeys|
//@[075:0076) |   |     | | | ├─Token(LeftParen) |(|
//@[076:0077) |   |     | | | └─Token(RightParen) |)|
//@[077:0078) |   |     | | ├─Token(Dot) |.|
//@[078:0082) |   |     | | └─IdentifierSyntax
//@[078:0082) |   |     | |   └─Token(Identifier) |keys|
//@[082:0083) |   |     | ├─Token(LeftSquare) |[|
//@[083:0084) |   |     | ├─IntegerLiteralSyntax
//@[083:0084) |   |     | | └─Token(Integer) |0|
//@[084:0085) |   |     | └─Token(RightSquare) |]|
//@[085:0086) |   |     ├─Token(Dot) |.|
//@[086:0091) |   |     └─IdentifierSyntax
//@[086:0091) |   |       └─Token(Identifier) |value|
//@[091:0092) |   └─Token(RightParen) |)|
//@[092:0093) ├─Token(NewLine) |\n|
output stgKeys3 array = map(ids, id => listKeys(id, stg[0].apiVersion).keys[0].value)
//@[000:0085) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |stgKeys3|
//@[016:0021) | ├─SimpleTypeSyntax
//@[016:0021) | | └─Token(Identifier) |array|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0085) | └─FunctionCallSyntax
//@[024:0027) |   ├─IdentifierSyntax
//@[024:0027) |   | └─Token(Identifier) |map|
//@[027:0028) |   ├─Token(LeftParen) |(|
//@[028:0031) |   ├─FunctionArgumentSyntax
//@[028:0031) |   | └─VariableAccessSyntax
//@[028:0031) |   |   └─IdentifierSyntax
//@[028:0031) |   |     └─Token(Identifier) |ids|
//@[031:0032) |   ├─Token(Comma) |,|
//@[033:0084) |   ├─FunctionArgumentSyntax
//@[033:0084) |   | └─LambdaSyntax
//@[033:0035) |   |   ├─LocalVariableSyntax
//@[033:0035) |   |   | └─IdentifierSyntax
//@[033:0035) |   |   |   └─Token(Identifier) |id|
//@[036:0038) |   |   ├─Token(Arrow) |=>|
//@[039:0084) |   |   └─PropertyAccessSyntax
//@[039:0078) |   |     ├─ArrayAccessSyntax
//@[039:0075) |   |     | ├─PropertyAccessSyntax
//@[039:0070) |   |     | | ├─FunctionCallSyntax
//@[039:0047) |   |     | | | ├─IdentifierSyntax
//@[039:0047) |   |     | | | | └─Token(Identifier) |listKeys|
//@[047:0048) |   |     | | | ├─Token(LeftParen) |(|
//@[048:0050) |   |     | | | ├─FunctionArgumentSyntax
//@[048:0050) |   |     | | | | └─VariableAccessSyntax
//@[048:0050) |   |     | | | |   └─IdentifierSyntax
//@[048:0050) |   |     | | | |     └─Token(Identifier) |id|
//@[050:0051) |   |     | | | ├─Token(Comma) |,|
//@[052:0069) |   |     | | | ├─FunctionArgumentSyntax
//@[052:0069) |   |     | | | | └─PropertyAccessSyntax
//@[052:0058) |   |     | | | |   ├─ArrayAccessSyntax
//@[052:0055) |   |     | | | |   | ├─VariableAccessSyntax
//@[052:0055) |   |     | | | |   | | └─IdentifierSyntax
//@[052:0055) |   |     | | | |   | |   └─Token(Identifier) |stg|
//@[055:0056) |   |     | | | |   | ├─Token(LeftSquare) |[|
//@[056:0057) |   |     | | | |   | ├─IntegerLiteralSyntax
//@[056:0057) |   |     | | | |   | | └─Token(Integer) |0|
//@[057:0058) |   |     | | | |   | └─Token(RightSquare) |]|
//@[058:0059) |   |     | | | |   ├─Token(Dot) |.|
//@[059:0069) |   |     | | | |   └─IdentifierSyntax
//@[059:0069) |   |     | | | |     └─Token(Identifier) |apiVersion|
//@[069:0070) |   |     | | | └─Token(RightParen) |)|
//@[070:0071) |   |     | | ├─Token(Dot) |.|
//@[071:0075) |   |     | | └─IdentifierSyntax
//@[071:0075) |   |     | |   └─Token(Identifier) |keys|
//@[075:0076) |   |     | ├─Token(LeftSquare) |[|
//@[076:0077) |   |     | ├─IntegerLiteralSyntax
//@[076:0077) |   |     | | └─Token(Integer) |0|
//@[077:0078) |   |     | └─Token(RightSquare) |]|
//@[078:0079) |   |     ├─Token(Dot) |.|
//@[079:0084) |   |     └─IdentifierSyntax
//@[079:0084) |   |       └─Token(Identifier) |value|
//@[084:0085) |   └─Token(RightParen) |)|
//@[085:0086) ├─Token(NewLine) |\n|
output accessTiers array = map(range(0, 2), k => stg[k].properties.accessTier)
//@[000:0078) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0018) | ├─IdentifierSyntax
//@[007:0018) | | └─Token(Identifier) |accessTiers|
//@[019:0024) | ├─SimpleTypeSyntax
//@[019:0024) | | └─Token(Identifier) |array|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0078) | └─FunctionCallSyntax
//@[027:0030) |   ├─IdentifierSyntax
//@[027:0030) |   | └─Token(Identifier) |map|
//@[030:0031) |   ├─Token(LeftParen) |(|
//@[031:0042) |   ├─FunctionArgumentSyntax
//@[031:0042) |   | └─FunctionCallSyntax
//@[031:0036) |   |   ├─IdentifierSyntax
//@[031:0036) |   |   | └─Token(Identifier) |range|
//@[036:0037) |   |   ├─Token(LeftParen) |(|
//@[037:0038) |   |   ├─FunctionArgumentSyntax
//@[037:0038) |   |   | └─IntegerLiteralSyntax
//@[037:0038) |   |   |   └─Token(Integer) |0|
//@[038:0039) |   |   ├─Token(Comma) |,|
//@[040:0041) |   |   ├─FunctionArgumentSyntax
//@[040:0041) |   |   | └─IntegerLiteralSyntax
//@[040:0041) |   |   |   └─Token(Integer) |2|
//@[041:0042) |   |   └─Token(RightParen) |)|
//@[042:0043) |   ├─Token(Comma) |,|
//@[044:0077) |   ├─FunctionArgumentSyntax
//@[044:0077) |   | └─LambdaSyntax
//@[044:0045) |   |   ├─LocalVariableSyntax
//@[044:0045) |   |   | └─IdentifierSyntax
//@[044:0045) |   |   |   └─Token(Identifier) |k|
//@[046:0048) |   |   ├─Token(Arrow) |=>|
//@[049:0077) |   |   └─PropertyAccessSyntax
//@[049:0066) |   |     ├─PropertyAccessSyntax
//@[049:0055) |   |     | ├─ArrayAccessSyntax
//@[049:0052) |   |     | | ├─VariableAccessSyntax
//@[049:0052) |   |     | | | └─IdentifierSyntax
//@[049:0052) |   |     | | |   └─Token(Identifier) |stg|
//@[052:0053) |   |     | | ├─Token(LeftSquare) |[|
//@[053:0054) |   |     | | ├─VariableAccessSyntax
//@[053:0054) |   |     | | | └─IdentifierSyntax
//@[053:0054) |   |     | | |   └─Token(Identifier) |k|
//@[054:0055) |   |     | | └─Token(RightSquare) |]|
//@[055:0056) |   |     | ├─Token(Dot) |.|
//@[056:0066) |   |     | └─IdentifierSyntax
//@[056:0066) |   |     |   └─Token(Identifier) |properties|
//@[066:0067) |   |     ├─Token(Dot) |.|
//@[067:0077) |   |     └─IdentifierSyntax
//@[067:0077) |   |       └─Token(Identifier) |accessTier|
//@[077:0078) |   └─Token(RightParen) |)|
//@[078:0079) ├─Token(NewLine) |\n|
output accessTiers2 array = map(range(0, 2), x => map(range(0, 2), y => stg[x / y].properties.accessTier))
//@[000:0106) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0019) | ├─IdentifierSyntax
//@[007:0019) | | └─Token(Identifier) |accessTiers2|
//@[020:0025) | ├─SimpleTypeSyntax
//@[020:0025) | | └─Token(Identifier) |array|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0106) | └─FunctionCallSyntax
//@[028:0031) |   ├─IdentifierSyntax
//@[028:0031) |   | └─Token(Identifier) |map|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0043) |   ├─FunctionArgumentSyntax
//@[032:0043) |   | └─FunctionCallSyntax
//@[032:0037) |   |   ├─IdentifierSyntax
//@[032:0037) |   |   | └─Token(Identifier) |range|
//@[037:0038) |   |   ├─Token(LeftParen) |(|
//@[038:0039) |   |   ├─FunctionArgumentSyntax
//@[038:0039) |   |   | └─IntegerLiteralSyntax
//@[038:0039) |   |   |   └─Token(Integer) |0|
//@[039:0040) |   |   ├─Token(Comma) |,|
//@[041:0042) |   |   ├─FunctionArgumentSyntax
//@[041:0042) |   |   | └─IntegerLiteralSyntax
//@[041:0042) |   |   |   └─Token(Integer) |2|
//@[042:0043) |   |   └─Token(RightParen) |)|
//@[043:0044) |   ├─Token(Comma) |,|
//@[045:0105) |   ├─FunctionArgumentSyntax
//@[045:0105) |   | └─LambdaSyntax
//@[045:0046) |   |   ├─LocalVariableSyntax
//@[045:0046) |   |   | └─IdentifierSyntax
//@[045:0046) |   |   |   └─Token(Identifier) |x|
//@[047:0049) |   |   ├─Token(Arrow) |=>|
//@[050:0105) |   |   └─FunctionCallSyntax
//@[050:0053) |   |     ├─IdentifierSyntax
//@[050:0053) |   |     | └─Token(Identifier) |map|
//@[053:0054) |   |     ├─Token(LeftParen) |(|
//@[054:0065) |   |     ├─FunctionArgumentSyntax
//@[054:0065) |   |     | └─FunctionCallSyntax
//@[054:0059) |   |     |   ├─IdentifierSyntax
//@[054:0059) |   |     |   | └─Token(Identifier) |range|
//@[059:0060) |   |     |   ├─Token(LeftParen) |(|
//@[060:0061) |   |     |   ├─FunctionArgumentSyntax
//@[060:0061) |   |     |   | └─IntegerLiteralSyntax
//@[060:0061) |   |     |   |   └─Token(Integer) |0|
//@[061:0062) |   |     |   ├─Token(Comma) |,|
//@[063:0064) |   |     |   ├─FunctionArgumentSyntax
//@[063:0064) |   |     |   | └─IntegerLiteralSyntax
//@[063:0064) |   |     |   |   └─Token(Integer) |2|
//@[064:0065) |   |     |   └─Token(RightParen) |)|
//@[065:0066) |   |     ├─Token(Comma) |,|
//@[067:0104) |   |     ├─FunctionArgumentSyntax
//@[067:0104) |   |     | └─LambdaSyntax
//@[067:0068) |   |     |   ├─LocalVariableSyntax
//@[067:0068) |   |     |   | └─IdentifierSyntax
//@[067:0068) |   |     |   |   └─Token(Identifier) |y|
//@[069:0071) |   |     |   ├─Token(Arrow) |=>|
//@[072:0104) |   |     |   └─PropertyAccessSyntax
//@[072:0093) |   |     |     ├─PropertyAccessSyntax
//@[072:0082) |   |     |     | ├─ArrayAccessSyntax
//@[072:0075) |   |     |     | | ├─VariableAccessSyntax
//@[072:0075) |   |     |     | | | └─IdentifierSyntax
//@[072:0075) |   |     |     | | |   └─Token(Identifier) |stg|
//@[075:0076) |   |     |     | | ├─Token(LeftSquare) |[|
//@[076:0081) |   |     |     | | ├─BinaryOperationSyntax
//@[076:0077) |   |     |     | | | ├─VariableAccessSyntax
//@[076:0077) |   |     |     | | | | └─IdentifierSyntax
//@[076:0077) |   |     |     | | | |   └─Token(Identifier) |x|
//@[078:0079) |   |     |     | | | ├─Token(Slash) |/|
//@[080:0081) |   |     |     | | | └─VariableAccessSyntax
//@[080:0081) |   |     |     | | |   └─IdentifierSyntax
//@[080:0081) |   |     |     | | |     └─Token(Identifier) |y|
//@[081:0082) |   |     |     | | └─Token(RightSquare) |]|
//@[082:0083) |   |     |     | ├─Token(Dot) |.|
//@[083:0093) |   |     |     | └─IdentifierSyntax
//@[083:0093) |   |     |     |   └─Token(Identifier) |properties|
//@[093:0094) |   |     |     ├─Token(Dot) |.|
//@[094:0104) |   |     |     └─IdentifierSyntax
//@[094:0104) |   |     |       └─Token(Identifier) |accessTier|
//@[104:0105) |   |     └─Token(RightParen) |)|
//@[105:0106) |   └─Token(RightParen) |)|
//@[106:0107) ├─Token(NewLine) |\n|
output accessTiers3 array = map(ids, foo => reference('${foo}').accessTier)
//@[000:0075) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0019) | ├─IdentifierSyntax
//@[007:0019) | | └─Token(Identifier) |accessTiers3|
//@[020:0025) | ├─SimpleTypeSyntax
//@[020:0025) | | └─Token(Identifier) |array|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0075) | └─FunctionCallSyntax
//@[028:0031) |   ├─IdentifierSyntax
//@[028:0031) |   | └─Token(Identifier) |map|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0035) |   ├─FunctionArgumentSyntax
//@[032:0035) |   | └─VariableAccessSyntax
//@[032:0035) |   |   └─IdentifierSyntax
//@[032:0035) |   |     └─Token(Identifier) |ids|
//@[035:0036) |   ├─Token(Comma) |,|
//@[037:0074) |   ├─FunctionArgumentSyntax
//@[037:0074) |   | └─LambdaSyntax
//@[037:0040) |   |   ├─LocalVariableSyntax
//@[037:0040) |   |   | └─IdentifierSyntax
//@[037:0040) |   |   |   └─Token(Identifier) |foo|
//@[041:0043) |   |   ├─Token(Arrow) |=>|
//@[044:0074) |   |   └─PropertyAccessSyntax
//@[044:0063) |   |     ├─FunctionCallSyntax
//@[044:0053) |   |     | ├─IdentifierSyntax
//@[044:0053) |   |     | | └─Token(Identifier) |reference|
//@[053:0054) |   |     | ├─Token(LeftParen) |(|
//@[054:0062) |   |     | ├─FunctionArgumentSyntax
//@[054:0062) |   |     | | └─StringSyntax
//@[054:0057) |   |     | |   ├─Token(StringLeftPiece) |'${|
//@[057:0060) |   |     | |   ├─VariableAccessSyntax
//@[057:0060) |   |     | |   | └─IdentifierSyntax
//@[057:0060) |   |     | |   |   └─Token(Identifier) |foo|
//@[060:0062) |   |     | |   └─Token(StringRightPiece) |}'|
//@[062:0063) |   |     | └─Token(RightParen) |)|
//@[063:0064) |   |     ├─Token(Dot) |.|
//@[064:0074) |   |     └─IdentifierSyntax
//@[064:0074) |   |       └─Token(Identifier) |accessTier|
//@[074:0075) |   └─Token(RightParen) |)|
//@[075:0077) ├─Token(NewLine) |\n\n|

module modLoop './empty.bicep' = [for item in range(0, 5): {
//@[000:0084) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0014) | ├─IdentifierSyntax
//@[007:0014) | | └─Token(Identifier) |modLoop|
//@[015:0030) | ├─StringSyntax
//@[015:0030) | | └─Token(StringComplete) |'./empty.bicep'|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0084) | └─ForSyntax
//@[033:0034) |   ├─Token(LeftSquare) |[|
//@[034:0037) |   ├─Token(Identifier) |for|
//@[038:0042) |   ├─LocalVariableSyntax
//@[038:0042) |   | └─IdentifierSyntax
//@[038:0042) |   |   └─Token(Identifier) |item|
//@[043:0045) |   ├─Token(Identifier) |in|
//@[046:0057) |   ├─FunctionCallSyntax
//@[046:0051) |   | ├─IdentifierSyntax
//@[046:0051) |   | | └─Token(Identifier) |range|
//@[051:0052) |   | ├─Token(LeftParen) |(|
//@[052:0053) |   | ├─FunctionArgumentSyntax
//@[052:0053) |   | | └─IntegerLiteralSyntax
//@[052:0053) |   | |   └─Token(Integer) |0|
//@[053:0054) |   | ├─Token(Comma) |,|
//@[055:0056) |   | ├─FunctionArgumentSyntax
//@[055:0056) |   | | └─IntegerLiteralSyntax
//@[055:0056) |   | |   └─Token(Integer) |5|
//@[056:0057) |   | └─Token(RightParen) |)|
//@[057:0058) |   ├─Token(Colon) |:|
//@[059:0083) |   ├─ObjectSyntax
//@[059:0060) |   | ├─Token(LeftBrace) |{|
//@[060:0061) |   | ├─Token(NewLine) |\n|
  name: 'foo${item}'
//@[002:0020) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0020) |   | | └─StringSyntax
//@[008:0014) |   | |   ├─Token(StringLeftPiece) |'foo${|
//@[014:0018) |   | |   ├─VariableAccessSyntax
//@[014:0018) |   | |   | └─IdentifierSyntax
//@[014:0018) |   | |   |   └─Token(Identifier) |item|
//@[018:0020) |   | |   └─Token(StringRightPiece) |}'|
//@[020:0021) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0004) ├─Token(NewLine) |\n\n|

var modLoopNames = map(modLoop, i => i.name)
//@[000:0044) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |modLoopNames|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0044) | └─FunctionCallSyntax
//@[019:0022) |   ├─IdentifierSyntax
//@[019:0022) |   | └─Token(Identifier) |map|
//@[022:0023) |   ├─Token(LeftParen) |(|
//@[023:0030) |   ├─FunctionArgumentSyntax
//@[023:0030) |   | └─VariableAccessSyntax
//@[023:0030) |   |   └─IdentifierSyntax
//@[023:0030) |   |     └─Token(Identifier) |modLoop|
//@[030:0031) |   ├─Token(Comma) |,|
//@[032:0043) |   ├─FunctionArgumentSyntax
//@[032:0043) |   | └─LambdaSyntax
//@[032:0033) |   |   ├─LocalVariableSyntax
//@[032:0033) |   |   | └─IdentifierSyntax
//@[032:0033) |   |   |   └─Token(Identifier) |i|
//@[034:0036) |   |   ├─Token(Arrow) |=>|
//@[037:0043) |   |   └─PropertyAccessSyntax
//@[037:0038) |   |     ├─VariableAccessSyntax
//@[037:0038) |   |     | └─IdentifierSyntax
//@[037:0038) |   |     |   └─Token(Identifier) |i|
//@[038:0039) |   |     ├─Token(Dot) |.|
//@[039:0043) |   |     └─IdentifierSyntax
//@[039:0043) |   |       └─Token(Identifier) |name|
//@[043:0044) |   └─Token(RightParen) |)|
//@[044:0045) ├─Token(NewLine) |\n|
output modOutputs array = map(range(0, 5), i => modLoop[i].outputs.foo)
//@[000:0071) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0017) | ├─IdentifierSyntax
//@[007:0017) | | └─Token(Identifier) |modOutputs|
//@[018:0023) | ├─SimpleTypeSyntax
//@[018:0023) | | └─Token(Identifier) |array|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0071) | └─FunctionCallSyntax
//@[026:0029) |   ├─IdentifierSyntax
//@[026:0029) |   | └─Token(Identifier) |map|
//@[029:0030) |   ├─Token(LeftParen) |(|
//@[030:0041) |   ├─FunctionArgumentSyntax
//@[030:0041) |   | └─FunctionCallSyntax
//@[030:0035) |   |   ├─IdentifierSyntax
//@[030:0035) |   |   | └─Token(Identifier) |range|
//@[035:0036) |   |   ├─Token(LeftParen) |(|
//@[036:0037) |   |   ├─FunctionArgumentSyntax
//@[036:0037) |   |   | └─IntegerLiteralSyntax
//@[036:0037) |   |   |   └─Token(Integer) |0|
//@[037:0038) |   |   ├─Token(Comma) |,|
//@[039:0040) |   |   ├─FunctionArgumentSyntax
//@[039:0040) |   |   | └─IntegerLiteralSyntax
//@[039:0040) |   |   |   └─Token(Integer) |5|
//@[040:0041) |   |   └─Token(RightParen) |)|
//@[041:0042) |   ├─Token(Comma) |,|
//@[043:0070) |   ├─FunctionArgumentSyntax
//@[043:0070) |   | └─LambdaSyntax
//@[043:0044) |   |   ├─LocalVariableSyntax
//@[043:0044) |   |   | └─IdentifierSyntax
//@[043:0044) |   |   |   └─Token(Identifier) |i|
//@[045:0047) |   |   ├─Token(Arrow) |=>|
//@[048:0070) |   |   └─PropertyAccessSyntax
//@[048:0066) |   |     ├─PropertyAccessSyntax
//@[048:0058) |   |     | ├─ArrayAccessSyntax
//@[048:0055) |   |     | | ├─VariableAccessSyntax
//@[048:0055) |   |     | | | └─IdentifierSyntax
//@[048:0055) |   |     | | |   └─Token(Identifier) |modLoop|
//@[055:0056) |   |     | | ├─Token(LeftSquare) |[|
//@[056:0057) |   |     | | ├─VariableAccessSyntax
//@[056:0057) |   |     | | | └─IdentifierSyntax
//@[056:0057) |   |     | | |   └─Token(Identifier) |i|
//@[057:0058) |   |     | | └─Token(RightSquare) |]|
//@[058:0059) |   |     | ├─Token(Dot) |.|
//@[059:0066) |   |     | └─IdentifierSyntax
//@[059:0066) |   |     |   └─Token(Identifier) |outputs|
//@[066:0067) |   |     ├─Token(Dot) |.|
//@[067:0070) |   |     └─IdentifierSyntax
//@[067:0070) |   |       └─Token(Identifier) |foo|
//@[070:0071) |   └─Token(RightParen) |)|
//@[071:0072) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
