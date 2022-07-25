var doggos = [
//@[000:2299) ProgramSyntax
//@[000:0054) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0010) | ├─IdentifierSyntax
//@[004:0010) | | └─Token(Identifier) |doggos|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0054) | └─ArraySyntax
//@[013:0014) |   ├─Token(LeftSquare) |[|
//@[014:0015) |   ├─Token(NewLine) |\n|
  'Evie'
//@[002:0008) |   ├─ArrayItemSyntax
//@[002:0008) |   | └─StringSyntax
//@[002:0008) |   |   └─Token(StringComplete) |'Evie'|
//@[008:0009) |   ├─Token(NewLine) |\n|
  'Casper'
//@[002:0010) |   ├─ArrayItemSyntax
//@[002:0010) |   | └─StringSyntax
//@[002:0010) |   |   └─Token(StringComplete) |'Casper'|
//@[010:0011) |   ├─Token(NewLine) |\n|
  'Indy'
//@[002:0008) |   ├─ArrayItemSyntax
//@[002:0008) |   | └─StringSyntax
//@[002:0008) |   |   └─Token(StringComplete) |'Indy'|
//@[008:0009) |   ├─Token(NewLine) |\n|
  'Kira'
//@[002:0008) |   ├─ArrayItemSyntax
//@[002:0008) |   | └─StringSyntax
//@[002:0008) |   |   └─Token(StringComplete) |'Kira'|
//@[008:0009) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

var numbers = range(0, 4)
//@[000:0025) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |numbers|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0025) | └─FunctionCallSyntax
//@[014:0019) |   ├─IdentifierSyntax
//@[014:0019) |   | └─Token(Identifier) |range|
//@[019:0020) |   ├─Token(LeftParen) |(|
//@[020:0021) |   ├─FunctionArgumentSyntax
//@[020:0021) |   | └─IntegerLiteralSyntax
//@[020:0021) |   |   └─Token(Integer) |0|
//@[021:0022) |   ├─Token(Comma) |,|
//@[023:0024) |   ├─FunctionArgumentSyntax
//@[023:0024) |   | └─IntegerLiteralSyntax
//@[023:0024) |   |   └─Token(Integer) |4|
//@[024:0025) |   └─Token(RightParen) |)|
//@[025:0027) ├─Token(NewLine) |\n\n|

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[000:0046) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |sayHello|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0046) | └─FunctionCallSyntax
//@[015:0018) |   ├─IdentifierSyntax
//@[015:0018) |   | └─Token(Identifier) |map|
//@[018:0019) |   ├─Token(LeftParen) |(|
//@[019:0025) |   ├─FunctionArgumentSyntax
//@[019:0025) |   | └─VariableAccessSyntax
//@[019:0025) |   |   └─IdentifierSyntax
//@[019:0025) |   |     └─Token(Identifier) |doggos|
//@[025:0026) |   ├─Token(Comma) |,|
//@[027:0045) |   ├─FunctionArgumentSyntax
//@[027:0045) |   | └─LambdaSyntax
//@[027:0028) |   |   ├─LocalVariableSyntax
//@[027:0028) |   |   | └─IdentifierSyntax
//@[027:0028) |   |   |   └─Token(Identifier) |i|
//@[029:0031) |   |   ├─Token(Arrow) |=>|
//@[032:0045) |   |   └─StringSyntax
//@[032:0041) |   |     ├─Token(StringLeftPiece) |'Hello ${|
//@[041:0042) |   |     ├─VariableAccessSyntax
//@[041:0042) |   |     | └─IdentifierSyntax
//@[041:0042) |   |     |   └─Token(Identifier) |i|
//@[042:0045) |   |     └─Token(StringRightPiece) |}!'|
//@[045:0046) |   └─Token(RightParen) |)|
//@[046:0048) ├─Token(NewLine) |\n\n|

var isEven = filter(numbers, i => 0 == i % 2)
//@[000:0045) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0010) | ├─IdentifierSyntax
//@[004:0010) | | └─Token(Identifier) |isEven|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0045) | └─FunctionCallSyntax
//@[013:0019) |   ├─IdentifierSyntax
//@[013:0019) |   | └─Token(Identifier) |filter|
//@[019:0020) |   ├─Token(LeftParen) |(|
//@[020:0027) |   ├─FunctionArgumentSyntax
//@[020:0027) |   | └─VariableAccessSyntax
//@[020:0027) |   |   └─IdentifierSyntax
//@[020:0027) |   |     └─Token(Identifier) |numbers|
//@[027:0028) |   ├─Token(Comma) |,|
//@[029:0044) |   ├─FunctionArgumentSyntax
//@[029:0044) |   | └─LambdaSyntax
//@[029:0030) |   |   ├─LocalVariableSyntax
//@[029:0030) |   |   | └─IdentifierSyntax
//@[029:0030) |   |   |   └─Token(Identifier) |i|
//@[031:0033) |   |   ├─Token(Arrow) |=>|
//@[034:0044) |   |   └─BinaryOperationSyntax
//@[034:0035) |   |     ├─IntegerLiteralSyntax
//@[034:0035) |   |     | └─Token(Integer) |0|
//@[036:0038) |   |     ├─Token(Equals) |==|
//@[039:0044) |   |     └─BinaryOperationSyntax
//@[039:0040) |   |       ├─VariableAccessSyntax
//@[039:0040) |   |       | └─IdentifierSyntax
//@[039:0040) |   |       |   └─Token(Identifier) |i|
//@[041:0042) |   |       ├─Token(Modulo) |%|
//@[043:0044) |   |       └─IntegerLiteralSyntax
//@[043:0044) |   |         └─Token(Integer) |2|
//@[044:0045) |   └─Token(RightParen) |)|
//@[045:0047) ├─Token(NewLine) |\n\n|

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[000:0118) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0027) | ├─IdentifierSyntax
//@[004:0027) | | └─Token(Identifier) |evenDoggosNestedLambdas|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0118) | └─FunctionCallSyntax
//@[030:0033) |   ├─IdentifierSyntax
//@[030:0033) |   | └─Token(Identifier) |map|
//@[033:0034) |   ├─Token(LeftParen) |(|
//@[034:0101) |   ├─FunctionArgumentSyntax
//@[034:0101) |   | └─FunctionCallSyntax
//@[034:0040) |   |   ├─IdentifierSyntax
//@[034:0040) |   |   | └─Token(Identifier) |filter|
//@[040:0041) |   |   ├─Token(LeftParen) |(|
//@[041:0048) |   |   ├─FunctionArgumentSyntax
//@[041:0048) |   |   | └─VariableAccessSyntax
//@[041:0048) |   |   |   └─IdentifierSyntax
//@[041:0048) |   |   |     └─Token(Identifier) |numbers|
//@[048:0049) |   |   ├─Token(Comma) |,|
//@[050:0100) |   |   ├─FunctionArgumentSyntax
//@[050:0100) |   |   | └─LambdaSyntax
//@[050:0051) |   |   |   ├─LocalVariableSyntax
//@[050:0051) |   |   |   | └─IdentifierSyntax
//@[050:0051) |   |   |   |   └─Token(Identifier) |i|
//@[052:0054) |   |   |   ├─Token(Arrow) |=>|
//@[055:0100) |   |   |   └─FunctionCallSyntax
//@[055:0063) |   |   |     ├─IdentifierSyntax
//@[055:0063) |   |   |     | └─Token(Identifier) |contains|
//@[063:0064) |   |   |     ├─Token(LeftParen) |(|
//@[064:0096) |   |   |     ├─FunctionArgumentSyntax
//@[064:0096) |   |   |     | └─FunctionCallSyntax
//@[064:0070) |   |   |     |   ├─IdentifierSyntax
//@[064:0070) |   |   |     |   | └─Token(Identifier) |filter|
//@[070:0071) |   |   |     |   ├─Token(LeftParen) |(|
//@[071:0078) |   |   |     |   ├─FunctionArgumentSyntax
//@[071:0078) |   |   |     |   | └─VariableAccessSyntax
//@[071:0078) |   |   |     |   |   └─IdentifierSyntax
//@[071:0078) |   |   |     |   |     └─Token(Identifier) |numbers|
//@[078:0079) |   |   |     |   ├─Token(Comma) |,|
//@[080:0095) |   |   |     |   ├─FunctionArgumentSyntax
//@[080:0095) |   |   |     |   | └─LambdaSyntax
//@[080:0081) |   |   |     |   |   ├─LocalVariableSyntax
//@[080:0081) |   |   |     |   |   | └─IdentifierSyntax
//@[080:0081) |   |   |     |   |   |   └─Token(Identifier) |j|
//@[082:0084) |   |   |     |   |   ├─Token(Arrow) |=>|
//@[085:0095) |   |   |     |   |   └─BinaryOperationSyntax
//@[085:0086) |   |   |     |   |     ├─IntegerLiteralSyntax
//@[085:0086) |   |   |     |   |     | └─Token(Integer) |0|
//@[087:0089) |   |   |     |   |     ├─Token(Equals) |==|
//@[090:0095) |   |   |     |   |     └─BinaryOperationSyntax
//@[090:0091) |   |   |     |   |       ├─VariableAccessSyntax
//@[090:0091) |   |   |     |   |       | └─IdentifierSyntax
//@[090:0091) |   |   |     |   |       |   └─Token(Identifier) |j|
//@[092:0093) |   |   |     |   |       ├─Token(Modulo) |%|
//@[094:0095) |   |   |     |   |       └─IntegerLiteralSyntax
//@[094:0095) |   |   |     |   |         └─Token(Integer) |2|
//@[095:0096) |   |   |     |   └─Token(RightParen) |)|
//@[096:0097) |   |   |     ├─Token(Comma) |,|
//@[098:0099) |   |   |     ├─FunctionArgumentSyntax
//@[098:0099) |   |   |     | └─VariableAccessSyntax
//@[098:0099) |   |   |     |   └─IdentifierSyntax
//@[098:0099) |   |   |     |     └─Token(Identifier) |i|
//@[099:0100) |   |   |     └─Token(RightParen) |)|
//@[100:0101) |   |   └─Token(RightParen) |)|
//@[101:0102) |   ├─Token(Comma) |,|
//@[103:0117) |   ├─FunctionArgumentSyntax
//@[103:0117) |   | └─LambdaSyntax
//@[103:0104) |   |   ├─LocalVariableSyntax
//@[103:0104) |   |   | └─IdentifierSyntax
//@[103:0104) |   |   |   └─Token(Identifier) |x|
//@[105:0107) |   |   ├─Token(Arrow) |=>|
//@[108:0117) |   |   └─ArrayAccessSyntax
//@[108:0114) |   |     ├─VariableAccessSyntax
//@[108:0114) |   |     | └─IdentifierSyntax
//@[108:0114) |   |     |   └─Token(Identifier) |doggos|
//@[114:0115) |   |     ├─Token(LeftSquare) |[|
//@[115:0116) |   |     ├─VariableAccessSyntax
//@[115:0116) |   |     | └─IdentifierSyntax
//@[115:0116) |   |     |   └─Token(Identifier) |x|
//@[116:0117) |   |     └─Token(RightSquare) |]|
//@[117:0118) |   └─Token(RightParen) |)|
//@[118:0120) ├─Token(NewLine) |\n\n|

var flattenedArrayOfArrays = flatten([0, 1], [2, 3], [4, 5])
//@[000:0060) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0026) | ├─IdentifierSyntax
//@[004:0026) | | └─Token(Identifier) |flattenedArrayOfArrays|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0060) | └─FunctionCallSyntax
//@[029:0036) |   ├─IdentifierSyntax
//@[029:0036) |   | └─Token(Identifier) |flatten|
//@[036:0037) |   ├─Token(LeftParen) |(|
//@[037:0043) |   ├─FunctionArgumentSyntax
//@[037:0043) |   | └─ArraySyntax
//@[037:0038) |   |   ├─Token(LeftSquare) |[|
//@[038:0039) |   |   ├─ArrayItemSyntax
//@[038:0039) |   |   | └─IntegerLiteralSyntax
//@[038:0039) |   |   |   └─Token(Integer) |0|
//@[039:0040) |   |   ├─Token(Comma) |,|
//@[041:0042) |   |   ├─ArrayItemSyntax
//@[041:0042) |   |   | └─IntegerLiteralSyntax
//@[041:0042) |   |   |   └─Token(Integer) |1|
//@[042:0043) |   |   └─Token(RightSquare) |]|
//@[043:0044) |   ├─Token(Comma) |,|
//@[045:0051) |   ├─FunctionArgumentSyntax
//@[045:0051) |   | └─ArraySyntax
//@[045:0046) |   |   ├─Token(LeftSquare) |[|
//@[046:0047) |   |   ├─ArrayItemSyntax
//@[046:0047) |   |   | └─IntegerLiteralSyntax
//@[046:0047) |   |   |   └─Token(Integer) |2|
//@[047:0048) |   |   ├─Token(Comma) |,|
//@[049:0050) |   |   ├─ArrayItemSyntax
//@[049:0050) |   |   | └─IntegerLiteralSyntax
//@[049:0050) |   |   |   └─Token(Integer) |3|
//@[050:0051) |   |   └─Token(RightSquare) |]|
//@[051:0052) |   ├─Token(Comma) |,|
//@[053:0059) |   ├─FunctionArgumentSyntax
//@[053:0059) |   | └─ArraySyntax
//@[053:0054) |   |   ├─Token(LeftSquare) |[|
//@[054:0055) |   |   ├─ArrayItemSyntax
//@[054:0055) |   |   | └─IntegerLiteralSyntax
//@[054:0055) |   |   |   └─Token(Integer) |4|
//@[055:0056) |   |   ├─Token(Comma) |,|
//@[057:0058) |   |   ├─ArrayItemSyntax
//@[057:0058) |   |   | └─IntegerLiteralSyntax
//@[057:0058) |   |   |   └─Token(Integer) |5|
//@[058:0059) |   |   └─Token(RightSquare) |]|
//@[059:0060) |   └─Token(RightParen) |)|
//@[060:0061) ├─Token(NewLine) |\n|
var flattenedEmptyArray = flatten()
//@[000:0035) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |flattenedEmptyArray|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0035) | └─FunctionCallSyntax
//@[026:0033) |   ├─IdentifierSyntax
//@[026:0033) |   | └─Token(Identifier) |flatten|
//@[033:0034) |   ├─Token(LeftParen) |(|
//@[034:0035) |   └─Token(RightParen) |)|
//@[035:0037) ├─Token(NewLine) |\n\n|

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[000:0062) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |mapSayHi|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0062) | └─FunctionCallSyntax
//@[015:0018) |   ├─IdentifierSyntax
//@[015:0018) |   | └─Token(Identifier) |map|
//@[018:0019) |   ├─Token(LeftParen) |(|
//@[019:0040) |   ├─FunctionArgumentSyntax
//@[019:0040) |   | └─ArraySyntax
//@[019:0020) |   |   ├─Token(LeftSquare) |[|
//@[020:0025) |   |   ├─ArrayItemSyntax
//@[020:0025) |   |   | └─StringSyntax
//@[020:0025) |   |   |   └─Token(StringComplete) |'abc'|
//@[025:0026) |   |   ├─Token(Comma) |,|
//@[027:0032) |   |   ├─ArrayItemSyntax
//@[027:0032) |   |   | └─StringSyntax
//@[027:0032) |   |   |   └─Token(StringComplete) |'def'|
//@[032:0033) |   |   ├─Token(Comma) |,|
//@[034:0039) |   |   ├─ArrayItemSyntax
//@[034:0039) |   |   | └─StringSyntax
//@[034:0039) |   |   |   └─Token(StringComplete) |'ghi'|
//@[039:0040) |   |   └─Token(RightSquare) |]|
//@[040:0041) |   ├─Token(Comma) |,|
//@[042:0061) |   ├─FunctionArgumentSyntax
//@[042:0061) |   | └─LambdaSyntax
//@[042:0045) |   |   ├─LocalVariableSyntax
//@[042:0045) |   |   | └─IdentifierSyntax
//@[042:0045) |   |   |   └─Token(Identifier) |foo|
//@[046:0048) |   |   ├─Token(Arrow) |=>|
//@[049:0061) |   |   └─StringSyntax
//@[049:0055) |   |     ├─Token(StringLeftPiece) |'Hi ${|
//@[055:0058) |   |     ├─VariableAccessSyntax
//@[055:0058) |   |     | └─IdentifierSyntax
//@[055:0058) |   |     |   └─Token(Identifier) |foo|
//@[058:0061) |   |     └─Token(StringRightPiece) |}!'|
//@[061:0062) |   └─Token(RightParen) |)|
//@[062:0063) ├─Token(NewLine) |\n|
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[000:0043) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |mapEmpty|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0043) | └─FunctionCallSyntax
//@[015:0018) |   ├─IdentifierSyntax
//@[015:0018) |   | └─Token(Identifier) |map|
//@[018:0019) |   ├─Token(LeftParen) |(|
//@[019:0021) |   ├─FunctionArgumentSyntax
//@[019:0021) |   | └─ArraySyntax
//@[019:0020) |   |   ├─Token(LeftSquare) |[|
//@[020:0021) |   |   └─Token(RightSquare) |]|
//@[021:0022) |   ├─Token(Comma) |,|
//@[023:0042) |   ├─FunctionArgumentSyntax
//@[023:0042) |   | └─LambdaSyntax
//@[023:0026) |   |   ├─LocalVariableSyntax
//@[023:0026) |   |   | └─IdentifierSyntax
//@[023:0026) |   |   |   └─Token(Identifier) |foo|
//@[027:0029) |   |   ├─Token(Arrow) |=>|
//@[030:0042) |   |   └─StringSyntax
//@[030:0036) |   |     ├─Token(StringLeftPiece) |'Hi ${|
//@[036:0039) |   |     ├─VariableAccessSyntax
//@[036:0039) |   |     | └─IdentifierSyntax
//@[036:0039) |   |     |   └─Token(Identifier) |foo|
//@[039:0042) |   |     └─Token(StringRightPiece) |}!'|
//@[042:0043) |   └─Token(RightParen) |)|
//@[043:0044) ├─Token(NewLine) |\n|
var mapObject = map(range(0, length(doggos)), i => {
//@[000:0115) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |mapObject|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0115) | └─FunctionCallSyntax
//@[016:0019) |   ├─IdentifierSyntax
//@[016:0019) |   | └─Token(Identifier) |map|
//@[019:0020) |   ├─Token(LeftParen) |(|
//@[020:0044) |   ├─FunctionArgumentSyntax
//@[020:0044) |   | └─FunctionCallSyntax
//@[020:0025) |   |   ├─IdentifierSyntax
//@[020:0025) |   |   | └─Token(Identifier) |range|
//@[025:0026) |   |   ├─Token(LeftParen) |(|
//@[026:0027) |   |   ├─FunctionArgumentSyntax
//@[026:0027) |   |   | └─IntegerLiteralSyntax
//@[026:0027) |   |   |   └─Token(Integer) |0|
//@[027:0028) |   |   ├─Token(Comma) |,|
//@[029:0043) |   |   ├─FunctionArgumentSyntax
//@[029:0043) |   |   | └─FunctionCallSyntax
//@[029:0035) |   |   |   ├─IdentifierSyntax
//@[029:0035) |   |   |   | └─Token(Identifier) |length|
//@[035:0036) |   |   |   ├─Token(LeftParen) |(|
//@[036:0042) |   |   |   ├─FunctionArgumentSyntax
//@[036:0042) |   |   |   | └─VariableAccessSyntax
//@[036:0042) |   |   |   |   └─IdentifierSyntax
//@[036:0042) |   |   |   |     └─Token(Identifier) |doggos|
//@[042:0043) |   |   |   └─Token(RightParen) |)|
//@[043:0044) |   |   └─Token(RightParen) |)|
//@[044:0045) |   ├─Token(Comma) |,|
//@[046:0114) |   ├─FunctionArgumentSyntax
//@[046:0114) |   | └─LambdaSyntax
//@[046:0047) |   |   ├─LocalVariableSyntax
//@[046:0047) |   |   | └─IdentifierSyntax
//@[046:0047) |   |   |   └─Token(Identifier) |i|
//@[048:0050) |   |   ├─Token(Arrow) |=>|
//@[051:0114) |   |   └─ObjectSyntax
//@[051:0052) |   |     ├─Token(LeftBrace) |{|
//@[052:0053) |   |     ├─Token(NewLine) |\n|
  i: i
//@[002:0006) |   |     ├─ObjectPropertySyntax
//@[002:0003) |   |     | ├─IdentifierSyntax
//@[002:0003) |   |     | | └─Token(Identifier) |i|
//@[003:0004) |   |     | ├─Token(Colon) |:|
//@[005:0006) |   |     | └─VariableAccessSyntax
//@[005:0006) |   |     |   └─IdentifierSyntax
//@[005:0006) |   |     |     └─Token(Identifier) |i|
//@[006:0007) |   |     ├─Token(NewLine) |\n|
  doggo: doggos[i]
//@[002:0018) |   |     ├─ObjectPropertySyntax
//@[002:0007) |   |     | ├─IdentifierSyntax
//@[002:0007) |   |     | | └─Token(Identifier) |doggo|
//@[007:0008) |   |     | ├─Token(Colon) |:|
//@[009:0018) |   |     | └─ArrayAccessSyntax
//@[009:0015) |   |     |   ├─VariableAccessSyntax
//@[009:0015) |   |     |   | └─IdentifierSyntax
//@[009:0015) |   |     |   |   └─Token(Identifier) |doggos|
//@[015:0016) |   |     |   ├─Token(LeftSquare) |[|
//@[016:0017) |   |     |   ├─VariableAccessSyntax
//@[016:0017) |   |     |   | └─IdentifierSyntax
//@[016:0017) |   |     |   |   └─Token(Identifier) |i|
//@[017:0018) |   |     |   └─Token(RightSquare) |]|
//@[018:0019) |   |     ├─Token(NewLine) |\n|
  greeting: 'Ahoy, ${doggos[i]}!'
//@[002:0033) |   |     ├─ObjectPropertySyntax
//@[002:0010) |   |     | ├─IdentifierSyntax
//@[002:0010) |   |     | | └─Token(Identifier) |greeting|
//@[010:0011) |   |     | ├─Token(Colon) |:|
//@[012:0033) |   |     | └─StringSyntax
//@[012:0021) |   |     |   ├─Token(StringLeftPiece) |'Ahoy, ${|
//@[021:0030) |   |     |   ├─ArrayAccessSyntax
//@[021:0027) |   |     |   | ├─VariableAccessSyntax
//@[021:0027) |   |     |   | | └─IdentifierSyntax
//@[021:0027) |   |     |   | |   └─Token(Identifier) |doggos|
//@[027:0028) |   |     |   | ├─Token(LeftSquare) |[|
//@[028:0029) |   |     |   | ├─VariableAccessSyntax
//@[028:0029) |   |     |   | | └─IdentifierSyntax
//@[028:0029) |   |     |   | |   └─Token(Identifier) |i|
//@[029:0030) |   |     |   | └─Token(RightSquare) |]|
//@[030:0033) |   |     |   └─Token(StringRightPiece) |}!'|
//@[033:0034) |   |     ├─Token(NewLine) |\n|
})
//@[000:0001) |   |     └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightParen) |)|
//@[002:0003) ├─Token(NewLine) |\n|
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@[000:0067) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |mapArray|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0067) | └─FunctionCallSyntax
//@[015:0022) |   ├─IdentifierSyntax
//@[015:0022) |   | └─Token(Identifier) |flatten|
//@[022:0023) |   ├─Token(LeftParen) |(|
//@[023:0066) |   ├─FunctionArgumentSyntax
//@[023:0066) |   | └─FunctionCallSyntax
//@[023:0026) |   |   ├─IdentifierSyntax
//@[023:0026) |   |   | └─Token(Identifier) |map|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0038) |   |   ├─FunctionArgumentSyntax
//@[027:0038) |   |   | └─FunctionCallSyntax
//@[027:0032) |   |   |   ├─IdentifierSyntax
//@[027:0032) |   |   |   | └─Token(Identifier) |range|
//@[032:0033) |   |   |   ├─Token(LeftParen) |(|
//@[033:0034) |   |   |   ├─FunctionArgumentSyntax
//@[033:0034) |   |   |   | └─IntegerLiteralSyntax
//@[033:0034) |   |   |   |   └─Token(Integer) |1|
//@[034:0035) |   |   |   ├─Token(Comma) |,|
//@[036:0037) |   |   |   ├─FunctionArgumentSyntax
//@[036:0037) |   |   |   | └─IntegerLiteralSyntax
//@[036:0037) |   |   |   |   └─Token(Integer) |3|
//@[037:0038) |   |   |   └─Token(RightParen) |)|
//@[038:0039) |   |   ├─Token(Comma) |,|
//@[040:0065) |   |   ├─FunctionArgumentSyntax
//@[040:0065) |   |   | └─LambdaSyntax
//@[040:0041) |   |   |   ├─LocalVariableSyntax
//@[040:0041) |   |   |   | └─IdentifierSyntax
//@[040:0041) |   |   |   |   └─Token(Identifier) |i|
//@[042:0044) |   |   |   ├─Token(Arrow) |=>|
//@[045:0065) |   |   |   └─ArraySyntax
//@[045:0046) |   |   |     ├─Token(LeftSquare) |[|
//@[046:0051) |   |   |     ├─ArrayItemSyntax
//@[046:0051) |   |   |     | └─BinaryOperationSyntax
//@[046:0047) |   |   |     |   ├─VariableAccessSyntax
//@[046:0047) |   |   |     |   | └─IdentifierSyntax
//@[046:0047) |   |   |     |   |   └─Token(Identifier) |i|
//@[048:0049) |   |   |     |   ├─Token(Asterisk) |*|
//@[050:0051) |   |   |     |   └─IntegerLiteralSyntax
//@[050:0051) |   |   |     |     └─Token(Integer) |2|
//@[051:0052) |   |   |     ├─Token(Comma) |,|
//@[053:0064) |   |   |     ├─ArrayItemSyntax
//@[053:0064) |   |   |     | └─BinaryOperationSyntax
//@[053:0060) |   |   |     |   ├─ParenthesizedExpressionSyntax
//@[053:0054) |   |   |     |   | ├─Token(LeftParen) |(|
//@[054:0059) |   |   |     |   | ├─BinaryOperationSyntax
//@[054:0055) |   |   |     |   | | ├─VariableAccessSyntax
//@[054:0055) |   |   |     |   | | | └─IdentifierSyntax
//@[054:0055) |   |   |     |   | | |   └─Token(Identifier) |i|
//@[056:0057) |   |   |     |   | | ├─Token(Asterisk) |*|
//@[058:0059) |   |   |     |   | | └─IntegerLiteralSyntax
//@[058:0059) |   |   |     |   | |   └─Token(Integer) |2|
//@[059:0060) |   |   |     |   | └─Token(RightParen) |)|
//@[061:0062) |   |   |     |   ├─Token(Plus) |+|
//@[063:0064) |   |   |     |   └─IntegerLiteralSyntax
//@[063:0064) |   |   |     |     └─Token(Integer) |1|
//@[064:0065) |   |   |     └─Token(RightSquare) |]|
//@[065:0066) |   |   └─Token(RightParen) |)|
//@[066:0067) |   └─Token(RightParen) |)|
//@[067:0068) ├─Token(NewLine) |\n|
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[000:0095) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |mapMultiLineArray|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0095) | └─FunctionCallSyntax
//@[024:0031) |   ├─IdentifierSyntax
//@[024:0031) |   | └─Token(Identifier) |flatten|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0094) |   ├─FunctionArgumentSyntax
//@[032:0094) |   | └─FunctionCallSyntax
//@[032:0035) |   |   ├─IdentifierSyntax
//@[032:0035) |   |   | └─Token(Identifier) |map|
//@[035:0036) |   |   ├─Token(LeftParen) |(|
//@[036:0047) |   |   ├─FunctionArgumentSyntax
//@[036:0047) |   |   | └─FunctionCallSyntax
//@[036:0041) |   |   |   ├─IdentifierSyntax
//@[036:0041) |   |   |   | └─Token(Identifier) |range|
//@[041:0042) |   |   |   ├─Token(LeftParen) |(|
//@[042:0043) |   |   |   ├─FunctionArgumentSyntax
//@[042:0043) |   |   |   | └─IntegerLiteralSyntax
//@[042:0043) |   |   |   |   └─Token(Integer) |1|
//@[043:0044) |   |   |   ├─Token(Comma) |,|
//@[045:0046) |   |   |   ├─FunctionArgumentSyntax
//@[045:0046) |   |   |   | └─IntegerLiteralSyntax
//@[045:0046) |   |   |   |   └─Token(Integer) |3|
//@[046:0047) |   |   |   └─Token(RightParen) |)|
//@[047:0048) |   |   ├─Token(Comma) |,|
//@[049:0093) |   |   ├─FunctionArgumentSyntax
//@[049:0093) |   |   | └─LambdaSyntax
//@[049:0050) |   |   |   ├─LocalVariableSyntax
//@[049:0050) |   |   |   | └─IdentifierSyntax
//@[049:0050) |   |   |   |   └─Token(Identifier) |i|
//@[051:0053) |   |   |   ├─Token(Arrow) |=>|
//@[054:0093) |   |   |   └─ArraySyntax
//@[054:0055) |   |   |     ├─Token(LeftSquare) |[|
//@[055:0056) |   |   |     ├─Token(NewLine) |\n|
  i * 3
//@[002:0007) |   |   |     ├─ArrayItemSyntax
//@[002:0007) |   |   |     | └─BinaryOperationSyntax
//@[002:0003) |   |   |     |   ├─VariableAccessSyntax
//@[002:0003) |   |   |     |   | └─IdentifierSyntax
//@[002:0003) |   |   |     |   |   └─Token(Identifier) |i|
//@[004:0005) |   |   |     |   ├─Token(Asterisk) |*|
//@[006:0007) |   |   |     |   └─IntegerLiteralSyntax
//@[006:0007) |   |   |     |     └─Token(Integer) |3|
//@[007:0008) |   |   |     ├─Token(NewLine) |\n|
  (i * 3) + 1
//@[002:0013) |   |   |     ├─ArrayItemSyntax
//@[002:0013) |   |   |     | └─BinaryOperationSyntax
//@[002:0009) |   |   |     |   ├─ParenthesizedExpressionSyntax
//@[002:0003) |   |   |     |   | ├─Token(LeftParen) |(|
//@[003:0008) |   |   |     |   | ├─BinaryOperationSyntax
//@[003:0004) |   |   |     |   | | ├─VariableAccessSyntax
//@[003:0004) |   |   |     |   | | | └─IdentifierSyntax
//@[003:0004) |   |   |     |   | | |   └─Token(Identifier) |i|
//@[005:0006) |   |   |     |   | | ├─Token(Asterisk) |*|
//@[007:0008) |   |   |     |   | | └─IntegerLiteralSyntax
//@[007:0008) |   |   |     |   | |   └─Token(Integer) |3|
//@[008:0009) |   |   |     |   | └─Token(RightParen) |)|
//@[010:0011) |   |   |     |   ├─Token(Plus) |+|
//@[012:0013) |   |   |     |   └─IntegerLiteralSyntax
//@[012:0013) |   |   |     |     └─Token(Integer) |1|
//@[013:0014) |   |   |     ├─Token(NewLine) |\n|
  (i * 3) + 2
//@[002:0013) |   |   |     ├─ArrayItemSyntax
//@[002:0013) |   |   |     | └─BinaryOperationSyntax
//@[002:0009) |   |   |     |   ├─ParenthesizedExpressionSyntax
//@[002:0003) |   |   |     |   | ├─Token(LeftParen) |(|
//@[003:0008) |   |   |     |   | ├─BinaryOperationSyntax
//@[003:0004) |   |   |     |   | | ├─VariableAccessSyntax
//@[003:0004) |   |   |     |   | | | └─IdentifierSyntax
//@[003:0004) |   |   |     |   | | |   └─Token(Identifier) |i|
//@[005:0006) |   |   |     |   | | ├─Token(Asterisk) |*|
//@[007:0008) |   |   |     |   | | └─IntegerLiteralSyntax
//@[007:0008) |   |   |     |   | |   └─Token(Integer) |3|
//@[008:0009) |   |   |     |   | └─Token(RightParen) |)|
//@[010:0011) |   |   |     |   ├─Token(Plus) |+|
//@[012:0013) |   |   |     |   └─IntegerLiteralSyntax
//@[012:0013) |   |   |     |     └─Token(Integer) |2|
//@[013:0014) |   |   |     ├─Token(NewLine) |\n|
]))
//@[000:0001) |   |   |     └─Token(RightSquare) |]|
//@[001:0002) |   |   └─Token(RightParen) |)|
//@[002:0003) |   └─Token(RightParen) |)|
//@[003:0005) ├─Token(NewLine) |\n\n|

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[000:0076) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |filterEqualityCheck|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0076) | └─FunctionCallSyntax
//@[026:0032) |   ├─IdentifierSyntax
//@[026:0032) |   | └─Token(Identifier) |filter|
//@[032:0033) |   ├─Token(LeftParen) |(|
//@[033:0054) |   ├─FunctionArgumentSyntax
//@[033:0054) |   | └─ArraySyntax
//@[033:0034) |   |   ├─Token(LeftSquare) |[|
//@[034:0039) |   |   ├─ArrayItemSyntax
//@[034:0039) |   |   | └─StringSyntax
//@[034:0039) |   |   |   └─Token(StringComplete) |'abc'|
//@[039:0040) |   |   ├─Token(Comma) |,|
//@[041:0046) |   |   ├─ArrayItemSyntax
//@[041:0046) |   |   | └─StringSyntax
//@[041:0046) |   |   |   └─Token(StringComplete) |'def'|
//@[046:0047) |   |   ├─Token(Comma) |,|
//@[048:0053) |   |   ├─ArrayItemSyntax
//@[048:0053) |   |   | └─StringSyntax
//@[048:0053) |   |   |   └─Token(StringComplete) |'ghi'|
//@[053:0054) |   |   └─Token(RightSquare) |]|
//@[054:0055) |   ├─Token(Comma) |,|
//@[056:0075) |   ├─FunctionArgumentSyntax
//@[056:0075) |   | └─LambdaSyntax
//@[056:0059) |   |   ├─LocalVariableSyntax
//@[056:0059) |   |   | └─IdentifierSyntax
//@[056:0059) |   |   |   └─Token(Identifier) |foo|
//@[060:0062) |   |   ├─Token(Arrow) |=>|
//@[063:0075) |   |   └─BinaryOperationSyntax
//@[063:0068) |   |     ├─StringSyntax
//@[063:0068) |   |     | └─Token(StringComplete) |'def'|
//@[069:0071) |   |     ├─Token(Equals) |==|
//@[072:0075) |   |     └─VariableAccessSyntax
//@[072:0075) |   |       └─IdentifierSyntax
//@[072:0075) |   |         └─Token(Identifier) |foo|
//@[075:0076) |   └─Token(RightParen) |)|
//@[076:0077) ├─Token(NewLine) |\n|
var filterEmpty = filter([], foo => 'def' == foo)
//@[000:0049) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |filterEmpty|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0049) | └─FunctionCallSyntax
//@[018:0024) |   ├─IdentifierSyntax
//@[018:0024) |   | └─Token(Identifier) |filter|
//@[024:0025) |   ├─Token(LeftParen) |(|
//@[025:0027) |   ├─FunctionArgumentSyntax
//@[025:0027) |   | └─ArraySyntax
//@[025:0026) |   |   ├─Token(LeftSquare) |[|
//@[026:0027) |   |   └─Token(RightSquare) |]|
//@[027:0028) |   ├─Token(Comma) |,|
//@[029:0048) |   ├─FunctionArgumentSyntax
//@[029:0048) |   | └─LambdaSyntax
//@[029:0032) |   |   ├─LocalVariableSyntax
//@[029:0032) |   |   | └─IdentifierSyntax
//@[029:0032) |   |   |   └─Token(Identifier) |foo|
//@[033:0035) |   |   ├─Token(Arrow) |=>|
//@[036:0048) |   |   └─BinaryOperationSyntax
//@[036:0041) |   |     ├─StringSyntax
//@[036:0041) |   |     | └─Token(StringComplete) |'def'|
//@[042:0044) |   |     ├─Token(Equals) |==|
//@[045:0048) |   |     └─VariableAccessSyntax
//@[045:0048) |   |       └─IdentifierSyntax
//@[045:0048) |   |         └─Token(Identifier) |foo|
//@[048:0049) |   └─Token(RightParen) |)|
//@[049:0051) ├─Token(NewLine) |\n\n|

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[000:0059) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |sortNumeric|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0059) | └─FunctionCallSyntax
//@[018:0022) |   ├─IdentifierSyntax
//@[018:0022) |   | └─Token(Identifier) |sort|
//@[022:0023) |   ├─Token(LeftParen) |(|
//@[023:0041) |   ├─FunctionArgumentSyntax
//@[023:0041) |   | └─ArraySyntax
//@[023:0024) |   |   ├─Token(LeftSquare) |[|
//@[024:0025) |   |   ├─ArrayItemSyntax
//@[024:0025) |   |   | └─IntegerLiteralSyntax
//@[024:0025) |   |   |   └─Token(Integer) |8|
//@[025:0026) |   |   ├─Token(Comma) |,|
//@[027:0028) |   |   ├─ArrayItemSyntax
//@[027:0028) |   |   | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   └─Token(Integer) |3|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0032) |   |   ├─ArrayItemSyntax
//@[030:0032) |   |   | └─IntegerLiteralSyntax
//@[030:0032) |   |   |   └─Token(Integer) |10|
//@[032:0033) |   |   ├─Token(Comma) |,|
//@[034:0037) |   |   ├─ArrayItemSyntax
//@[034:0037) |   |   | └─UnaryOperationSyntax
//@[034:0035) |   |   |   ├─Token(Minus) |-|
//@[035:0037) |   |   |   └─IntegerLiteralSyntax
//@[035:0037) |   |   |     └─Token(Integer) |13|
//@[037:0038) |   |   ├─Token(Comma) |,|
//@[039:0040) |   |   ├─ArrayItemSyntax
//@[039:0040) |   |   | └─IntegerLiteralSyntax
//@[039:0040) |   |   |   └─Token(Integer) |5|
//@[040:0041) |   |   └─Token(RightSquare) |]|
//@[041:0042) |   ├─Token(Comma) |,|
//@[043:0058) |   ├─FunctionArgumentSyntax
//@[043:0058) |   | └─LambdaSyntax
//@[043:0049) |   |   ├─VariableBlockSyntax
//@[043:0044) |   |   | ├─Token(LeftParen) |(|
//@[044:0045) |   |   | ├─LocalVariableSyntax
//@[044:0045) |   |   | | └─IdentifierSyntax
//@[044:0045) |   |   | |   └─Token(Identifier) |x|
//@[045:0046) |   |   | ├─Token(Comma) |,|
//@[047:0048) |   |   | ├─LocalVariableSyntax
//@[047:0048) |   |   | | └─IdentifierSyntax
//@[047:0048) |   |   | |   └─Token(Identifier) |y|
//@[048:0049) |   |   | └─Token(RightParen) |)|
//@[050:0052) |   |   ├─Token(Arrow) |=>|
//@[053:0058) |   |   └─BinaryOperationSyntax
//@[053:0054) |   |     ├─VariableAccessSyntax
//@[053:0054) |   |     | └─IdentifierSyntax
//@[053:0054) |   |     |   └─Token(Identifier) |x|
//@[055:0056) |   |     ├─Token(LessThan) |<|
//@[057:0058) |   |     └─VariableAccessSyntax
//@[057:0058) |   |       └─IdentifierSyntax
//@[057:0058) |   |         └─Token(Identifier) |y|
//@[058:0059) |   └─Token(RightParen) |)|
//@[059:0060) ├─Token(NewLine) |\n|
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[000:0060) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |sortAlpha|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0060) | └─FunctionCallSyntax
//@[016:0020) |   ├─IdentifierSyntax
//@[016:0020) |   | └─Token(Identifier) |sort|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0042) |   ├─FunctionArgumentSyntax
//@[021:0042) |   | └─ArraySyntax
//@[021:0022) |   |   ├─Token(LeftSquare) |[|
//@[022:0027) |   |   ├─ArrayItemSyntax
//@[022:0027) |   |   | └─StringSyntax
//@[022:0027) |   |   |   └─Token(StringComplete) |'ghi'|
//@[027:0028) |   |   ├─Token(Comma) |,|
//@[029:0034) |   |   ├─ArrayItemSyntax
//@[029:0034) |   |   | └─StringSyntax
//@[029:0034) |   |   |   └─Token(StringComplete) |'abc'|
//@[034:0035) |   |   ├─Token(Comma) |,|
//@[036:0041) |   |   ├─ArrayItemSyntax
//@[036:0041) |   |   | └─StringSyntax
//@[036:0041) |   |   |   └─Token(StringComplete) |'def'|
//@[041:0042) |   |   └─Token(RightSquare) |]|
//@[042:0043) |   ├─Token(Comma) |,|
//@[044:0059) |   ├─FunctionArgumentSyntax
//@[044:0059) |   | └─LambdaSyntax
//@[044:0050) |   |   ├─VariableBlockSyntax
//@[044:0045) |   |   | ├─Token(LeftParen) |(|
//@[045:0046) |   |   | ├─LocalVariableSyntax
//@[045:0046) |   |   | | └─IdentifierSyntax
//@[045:0046) |   |   | |   └─Token(Identifier) |x|
//@[046:0047) |   |   | ├─Token(Comma) |,|
//@[048:0049) |   |   | ├─LocalVariableSyntax
//@[048:0049) |   |   | | └─IdentifierSyntax
//@[048:0049) |   |   | |   └─Token(Identifier) |y|
//@[049:0050) |   |   | └─Token(RightParen) |)|
//@[051:0053) |   |   ├─Token(Arrow) |=>|
//@[054:0059) |   |   └─BinaryOperationSyntax
//@[054:0055) |   |     ├─VariableAccessSyntax
//@[054:0055) |   |     | └─IdentifierSyntax
//@[054:0055) |   |     |   └─Token(Identifier) |x|
//@[056:0057) |   |     ├─Token(LessThan) |<|
//@[058:0059) |   |     └─VariableAccessSyntax
//@[058:0059) |   |       └─IdentifierSyntax
//@[058:0059) |   |         └─Token(Identifier) |y|
//@[059:0060) |   └─Token(RightParen) |)|
//@[060:0061) ├─Token(NewLine) |\n|
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[000:0067) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0020) | ├─IdentifierSyntax
//@[004:0020) | | └─Token(Identifier) |sortAlphaReverse|
//@[021:0022) | ├─Token(Assignment) |=|
//@[023:0067) | └─FunctionCallSyntax
//@[023:0027) |   ├─IdentifierSyntax
//@[023:0027) |   | └─Token(Identifier) |sort|
//@[027:0028) |   ├─Token(LeftParen) |(|
//@[028:0049) |   ├─FunctionArgumentSyntax
//@[028:0049) |   | └─ArraySyntax
//@[028:0029) |   |   ├─Token(LeftSquare) |[|
//@[029:0034) |   |   ├─ArrayItemSyntax
//@[029:0034) |   |   | └─StringSyntax
//@[029:0034) |   |   |   └─Token(StringComplete) |'ghi'|
//@[034:0035) |   |   ├─Token(Comma) |,|
//@[036:0041) |   |   ├─ArrayItemSyntax
//@[036:0041) |   |   | └─StringSyntax
//@[036:0041) |   |   |   └─Token(StringComplete) |'abc'|
//@[041:0042) |   |   ├─Token(Comma) |,|
//@[043:0048) |   |   ├─ArrayItemSyntax
//@[043:0048) |   |   | └─StringSyntax
//@[043:0048) |   |   |   └─Token(StringComplete) |'def'|
//@[048:0049) |   |   └─Token(RightSquare) |]|
//@[049:0050) |   ├─Token(Comma) |,|
//@[051:0066) |   ├─FunctionArgumentSyntax
//@[051:0066) |   | └─LambdaSyntax
//@[051:0057) |   |   ├─VariableBlockSyntax
//@[051:0052) |   |   | ├─Token(LeftParen) |(|
//@[052:0053) |   |   | ├─LocalVariableSyntax
//@[052:0053) |   |   | | └─IdentifierSyntax
//@[052:0053) |   |   | |   └─Token(Identifier) |x|
//@[053:0054) |   |   | ├─Token(Comma) |,|
//@[055:0056) |   |   | ├─LocalVariableSyntax
//@[055:0056) |   |   | | └─IdentifierSyntax
//@[055:0056) |   |   | |   └─Token(Identifier) |y|
//@[056:0057) |   |   | └─Token(RightParen) |)|
//@[058:0060) |   |   ├─Token(Arrow) |=>|
//@[061:0066) |   |   └─BinaryOperationSyntax
//@[061:0062) |   |     ├─VariableAccessSyntax
//@[061:0062) |   |     | └─IdentifierSyntax
//@[061:0062) |   |     |   └─Token(Identifier) |x|
//@[063:0064) |   |     ├─Token(GreaterThan) |>|
//@[065:0066) |   |     └─VariableAccessSyntax
//@[065:0066) |   |       └─IdentifierSyntax
//@[065:0066) |   |         └─Token(Identifier) |y|
//@[066:0067) |   └─Token(RightParen) |)|
//@[067:0068) ├─Token(NewLine) |\n|
var sortByObjectKey = sort([
//@[000:0188) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |sortByObjectKey|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0188) | └─FunctionCallSyntax
//@[022:0026) |   ├─IdentifierSyntax
//@[022:0026) |   | └─Token(Identifier) |sort|
//@[026:0027) |   ├─Token(LeftParen) |(|
//@[027:0152) |   ├─FunctionArgumentSyntax
//@[027:0152) |   | └─ArraySyntax
//@[027:0028) |   |   ├─Token(LeftSquare) |[|
//@[028:0029) |   |   ├─Token(NewLine) |\n|
  { key: 124, name: 'Second' }
//@[002:0030) |   |   ├─ArrayItemSyntax
//@[002:0030) |   |   | └─ObjectSyntax
//@[002:0003) |   |   |   ├─Token(LeftBrace) |{|
//@[004:0012) |   |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   |   | | └─Token(Identifier) |key|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0012) |   |   |   | └─IntegerLiteralSyntax
//@[009:0012) |   |   |   |   └─Token(Integer) |124|
//@[012:0013) |   |   |   ├─Token(Comma) |,|
//@[014:0028) |   |   |   ├─ObjectPropertySyntax
//@[014:0018) |   |   |   | ├─IdentifierSyntax
//@[014:0018) |   |   |   | | └─Token(Identifier) |name|
//@[018:0019) |   |   |   | ├─Token(Colon) |:|
//@[020:0028) |   |   |   | └─StringSyntax
//@[020:0028) |   |   |   |   └─Token(StringComplete) |'Second'|
//@[029:0030) |   |   |   └─Token(RightBrace) |}|
//@[030:0031) |   |   ├─Token(NewLine) |\n|
  { key: 298, name: 'Third' }
//@[002:0029) |   |   ├─ArrayItemSyntax
//@[002:0029) |   |   | └─ObjectSyntax
//@[002:0003) |   |   |   ├─Token(LeftBrace) |{|
//@[004:0012) |   |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   |   | | └─Token(Identifier) |key|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0012) |   |   |   | └─IntegerLiteralSyntax
//@[009:0012) |   |   |   |   └─Token(Integer) |298|
//@[012:0013) |   |   |   ├─Token(Comma) |,|
//@[014:0027) |   |   |   ├─ObjectPropertySyntax
//@[014:0018) |   |   |   | ├─IdentifierSyntax
//@[014:0018) |   |   |   | | └─Token(Identifier) |name|
//@[018:0019) |   |   |   | ├─Token(Colon) |:|
//@[020:0027) |   |   |   | └─StringSyntax
//@[020:0027) |   |   |   |   └─Token(StringComplete) |'Third'|
//@[028:0029) |   |   |   └─Token(RightBrace) |}|
//@[029:0030) |   |   ├─Token(NewLine) |\n|
  { key: 24, name: 'First' }
//@[002:0028) |   |   ├─ArrayItemSyntax
//@[002:0028) |   |   | └─ObjectSyntax
//@[002:0003) |   |   |   ├─Token(LeftBrace) |{|
//@[004:0011) |   |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   |   | | └─Token(Identifier) |key|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0011) |   |   |   | └─IntegerLiteralSyntax
//@[009:0011) |   |   |   |   └─Token(Integer) |24|
//@[011:0012) |   |   |   ├─Token(Comma) |,|
//@[013:0026) |   |   |   ├─ObjectPropertySyntax
//@[013:0017) |   |   |   | ├─IdentifierSyntax
//@[013:0017) |   |   |   | | └─Token(Identifier) |name|
//@[017:0018) |   |   |   | ├─Token(Colon) |:|
//@[019:0026) |   |   |   | └─StringSyntax
//@[019:0026) |   |   |   |   └─Token(StringComplete) |'First'|
//@[027:0028) |   |   |   └─Token(RightBrace) |}|
//@[028:0029) |   |   ├─Token(NewLine) |\n|
  { key: 1232, name: 'Fourth' }
//@[002:0031) |   |   ├─ArrayItemSyntax
//@[002:0031) |   |   | └─ObjectSyntax
//@[002:0003) |   |   |   ├─Token(LeftBrace) |{|
//@[004:0013) |   |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   |   | | └─Token(Identifier) |key|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0013) |   |   |   | └─IntegerLiteralSyntax
//@[009:0013) |   |   |   |   └─Token(Integer) |1232|
//@[013:0014) |   |   |   ├─Token(Comma) |,|
//@[015:0029) |   |   |   ├─ObjectPropertySyntax
//@[015:0019) |   |   |   | ├─IdentifierSyntax
//@[015:0019) |   |   |   | | └─Token(Identifier) |name|
//@[019:0020) |   |   |   | ├─Token(Colon) |:|
//@[021:0029) |   |   |   | └─StringSyntax
//@[021:0029) |   |   |   |   └─Token(StringComplete) |'Fourth'|
//@[030:0031) |   |   |   └─Token(RightBrace) |}|
//@[031:0032) |   |   ├─Token(NewLine) |\n|
], (x, y) => int(x.key) < int(y.key))
//@[000:0001) |   |   └─Token(RightSquare) |]|
//@[001:0002) |   ├─Token(Comma) |,|
//@[003:0036) |   ├─FunctionArgumentSyntax
//@[003:0036) |   | └─LambdaSyntax
//@[003:0009) |   |   ├─VariableBlockSyntax
//@[003:0004) |   |   | ├─Token(LeftParen) |(|
//@[004:0005) |   |   | ├─LocalVariableSyntax
//@[004:0005) |   |   | | └─IdentifierSyntax
//@[004:0005) |   |   | |   └─Token(Identifier) |x|
//@[005:0006) |   |   | ├─Token(Comma) |,|
//@[007:0008) |   |   | ├─LocalVariableSyntax
//@[007:0008) |   |   | | └─IdentifierSyntax
//@[007:0008) |   |   | |   └─Token(Identifier) |y|
//@[008:0009) |   |   | └─Token(RightParen) |)|
//@[010:0012) |   |   ├─Token(Arrow) |=>|
//@[013:0036) |   |   └─BinaryOperationSyntax
//@[013:0023) |   |     ├─FunctionCallSyntax
//@[013:0016) |   |     | ├─IdentifierSyntax
//@[013:0016) |   |     | | └─Token(Identifier) |int|
//@[016:0017) |   |     | ├─Token(LeftParen) |(|
//@[017:0022) |   |     | ├─FunctionArgumentSyntax
//@[017:0022) |   |     | | └─PropertyAccessSyntax
//@[017:0018) |   |     | |   ├─VariableAccessSyntax
//@[017:0018) |   |     | |   | └─IdentifierSyntax
//@[017:0018) |   |     | |   |   └─Token(Identifier) |x|
//@[018:0019) |   |     | |   ├─Token(Dot) |.|
//@[019:0022) |   |     | |   └─IdentifierSyntax
//@[019:0022) |   |     | |     └─Token(Identifier) |key|
//@[022:0023) |   |     | └─Token(RightParen) |)|
//@[024:0025) |   |     ├─Token(LessThan) |<|
//@[026:0036) |   |     └─FunctionCallSyntax
//@[026:0029) |   |       ├─IdentifierSyntax
//@[026:0029) |   |       | └─Token(Identifier) |int|
//@[029:0030) |   |       ├─Token(LeftParen) |(|
//@[030:0035) |   |       ├─FunctionArgumentSyntax
//@[030:0035) |   |       | └─PropertyAccessSyntax
//@[030:0031) |   |       |   ├─VariableAccessSyntax
//@[030:0031) |   |       |   | └─IdentifierSyntax
//@[030:0031) |   |       |   |   └─Token(Identifier) |y|
//@[031:0032) |   |       |   ├─Token(Dot) |.|
//@[032:0035) |   |       |   └─IdentifierSyntax
//@[032:0035) |   |       |     └─Token(Identifier) |key|
//@[035:0036) |   |       └─Token(RightParen) |)|
//@[036:0037) |   └─Token(RightParen) |)|
//@[037:0038) ├─Token(NewLine) |\n|
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[000:0051) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |sortEmpty|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0051) | └─FunctionCallSyntax
//@[016:0020) |   ├─IdentifierSyntax
//@[016:0020) |   | └─Token(Identifier) |sort|
//@[020:0021) |   ├─Token(LeftParen) |(|
//@[021:0023) |   ├─FunctionArgumentSyntax
//@[021:0023) |   | └─ArraySyntax
//@[021:0022) |   |   ├─Token(LeftSquare) |[|
//@[022:0023) |   |   └─Token(RightSquare) |]|
//@[023:0024) |   ├─Token(Comma) |,|
//@[025:0050) |   ├─FunctionArgumentSyntax
//@[025:0050) |   | └─LambdaSyntax
//@[025:0031) |   |   ├─VariableBlockSyntax
//@[025:0026) |   |   | ├─Token(LeftParen) |(|
//@[026:0027) |   |   | ├─LocalVariableSyntax
//@[026:0027) |   |   | | └─IdentifierSyntax
//@[026:0027) |   |   | |   └─Token(Identifier) |x|
//@[027:0028) |   |   | ├─Token(Comma) |,|
//@[029:0030) |   |   | ├─LocalVariableSyntax
//@[029:0030) |   |   | | └─IdentifierSyntax
//@[029:0030) |   |   | |   └─Token(Identifier) |y|
//@[030:0031) |   |   | └─Token(RightParen) |)|
//@[032:0034) |   |   ├─Token(Arrow) |=>|
//@[035:0050) |   |   └─BinaryOperationSyntax
//@[035:0041) |   |     ├─FunctionCallSyntax
//@[035:0038) |   |     | ├─IdentifierSyntax
//@[035:0038) |   |     | | └─Token(Identifier) |int|
//@[038:0039) |   |     | ├─Token(LeftParen) |(|
//@[039:0040) |   |     | ├─FunctionArgumentSyntax
//@[039:0040) |   |     | | └─VariableAccessSyntax
//@[039:0040) |   |     | |   └─IdentifierSyntax
//@[039:0040) |   |     | |     └─Token(Identifier) |x|
//@[040:0041) |   |     | └─Token(RightParen) |)|
//@[042:0043) |   |     ├─Token(LessThan) |<|
//@[044:0050) |   |     └─FunctionCallSyntax
//@[044:0047) |   |       ├─IdentifierSyntax
//@[044:0047) |   |       | └─Token(Identifier) |int|
//@[047:0048) |   |       ├─Token(LeftParen) |(|
//@[048:0049) |   |       ├─FunctionArgumentSyntax
//@[048:0049) |   |       | └─VariableAccessSyntax
//@[048:0049) |   |       |   └─IdentifierSyntax
//@[048:0049) |   |       |     └─Token(Identifier) |y|
//@[049:0050) |   |       └─Token(RightParen) |)|
//@[050:0051) |   └─Token(RightParen) |)|
//@[051:0053) ├─Token(NewLine) |\n\n|

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[000:0092) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |reduceStringConcat|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0092) | └─FunctionCallSyntax
//@[025:0031) |   ├─IdentifierSyntax
//@[025:0031) |   | └─Token(Identifier) |reduce|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0053) |   ├─FunctionArgumentSyntax
//@[032:0053) |   | └─ArraySyntax
//@[032:0033) |   |   ├─Token(LeftSquare) |[|
//@[033:0038) |   |   ├─ArrayItemSyntax
//@[033:0038) |   |   | └─StringSyntax
//@[033:0038) |   |   |   └─Token(StringComplete) |'abc'|
//@[038:0039) |   |   ├─Token(Comma) |,|
//@[040:0045) |   |   ├─ArrayItemSyntax
//@[040:0045) |   |   | └─StringSyntax
//@[040:0045) |   |   |   └─Token(StringComplete) |'def'|
//@[045:0046) |   |   ├─Token(Comma) |,|
//@[047:0052) |   |   ├─ArrayItemSyntax
//@[047:0052) |   |   | └─StringSyntax
//@[047:0052) |   |   |   └─Token(StringComplete) |'ghi'|
//@[052:0053) |   |   └─Token(RightSquare) |]|
//@[053:0054) |   ├─Token(Comma) |,|
//@[055:0057) |   ├─FunctionArgumentSyntax
//@[055:0057) |   | └─StringSyntax
//@[055:0057) |   |   └─Token(StringComplete) |''|
//@[057:0058) |   ├─Token(Comma) |,|
//@[059:0091) |   ├─FunctionArgumentSyntax
//@[059:0091) |   | └─LambdaSyntax
//@[059:0070) |   |   ├─VariableBlockSyntax
//@[059:0060) |   |   | ├─Token(LeftParen) |(|
//@[060:0063) |   |   | ├─LocalVariableSyntax
//@[060:0063) |   |   | | └─IdentifierSyntax
//@[060:0063) |   |   | |   └─Token(Identifier) |cur|
//@[063:0064) |   |   | ├─Token(Comma) |,|
//@[065:0069) |   |   | ├─LocalVariableSyntax
//@[065:0069) |   |   | | └─IdentifierSyntax
//@[065:0069) |   |   | |   └─Token(Identifier) |next|
//@[069:0070) |   |   | └─Token(RightParen) |)|
//@[071:0073) |   |   ├─Token(Arrow) |=>|
//@[074:0091) |   |   └─FunctionCallSyntax
//@[074:0080) |   |     ├─IdentifierSyntax
//@[074:0080) |   |     | └─Token(Identifier) |concat|
//@[080:0081) |   |     ├─Token(LeftParen) |(|
//@[081:0084) |   |     ├─FunctionArgumentSyntax
//@[081:0084) |   |     | └─VariableAccessSyntax
//@[081:0084) |   |     |   └─IdentifierSyntax
//@[081:0084) |   |     |     └─Token(Identifier) |cur|
//@[084:0085) |   |     ├─Token(Comma) |,|
//@[086:0090) |   |     ├─FunctionArgumentSyntax
//@[086:0090) |   |     | └─VariableAccessSyntax
//@[086:0090) |   |     |   └─IdentifierSyntax
//@[086:0090) |   |     |     └─Token(Identifier) |next|
//@[090:0091) |   |     └─Token(RightParen) |)|
//@[091:0092) |   └─Token(RightParen) |)|
//@[092:0093) ├─Token(NewLine) |\n|
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[000:0071) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |reduceFactorial|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0071) | └─FunctionCallSyntax
//@[022:0028) |   ├─IdentifierSyntax
//@[022:0028) |   | └─Token(Identifier) |reduce|
//@[028:0029) |   ├─Token(LeftParen) |(|
//@[029:0040) |   ├─FunctionArgumentSyntax
//@[029:0040) |   | └─FunctionCallSyntax
//@[029:0034) |   |   ├─IdentifierSyntax
//@[029:0034) |   |   | └─Token(Identifier) |range|
//@[034:0035) |   |   ├─Token(LeftParen) |(|
//@[035:0036) |   |   ├─FunctionArgumentSyntax
//@[035:0036) |   |   | └─IntegerLiteralSyntax
//@[035:0036) |   |   |   └─Token(Integer) |1|
//@[036:0037) |   |   ├─Token(Comma) |,|
//@[038:0039) |   |   ├─FunctionArgumentSyntax
//@[038:0039) |   |   | └─IntegerLiteralSyntax
//@[038:0039) |   |   |   └─Token(Integer) |5|
//@[039:0040) |   |   └─Token(RightParen) |)|
//@[040:0041) |   ├─Token(Comma) |,|
//@[042:0043) |   ├─FunctionArgumentSyntax
//@[042:0043) |   | └─IntegerLiteralSyntax
//@[042:0043) |   |   └─Token(Integer) |1|
//@[043:0044) |   ├─Token(Comma) |,|
//@[045:0070) |   ├─FunctionArgumentSyntax
//@[045:0070) |   | └─LambdaSyntax
//@[045:0056) |   |   ├─VariableBlockSyntax
//@[045:0046) |   |   | ├─Token(LeftParen) |(|
//@[046:0049) |   |   | ├─LocalVariableSyntax
//@[046:0049) |   |   | | └─IdentifierSyntax
//@[046:0049) |   |   | |   └─Token(Identifier) |cur|
//@[049:0050) |   |   | ├─Token(Comma) |,|
//@[051:0055) |   |   | ├─LocalVariableSyntax
//@[051:0055) |   |   | | └─IdentifierSyntax
//@[051:0055) |   |   | |   └─Token(Identifier) |next|
//@[055:0056) |   |   | └─Token(RightParen) |)|
//@[057:0059) |   |   ├─Token(Arrow) |=>|
//@[060:0070) |   |   └─BinaryOperationSyntax
//@[060:0063) |   |     ├─VariableAccessSyntax
//@[060:0063) |   |     | └─IdentifierSyntax
//@[060:0063) |   |     |   └─Token(Identifier) |cur|
//@[064:0065) |   |     ├─Token(Asterisk) |*|
//@[066:0070) |   |     └─VariableAccessSyntax
//@[066:0070) |   |       └─IdentifierSyntax
//@[066:0070) |   |         └─Token(Identifier) |next|
//@[070:0071) |   └─Token(RightParen) |)|
//@[071:0072) ├─Token(NewLine) |\n|
var reduceObjectUnion = reduce([
//@[000:0117) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |reduceObjectUnion|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0117) | └─FunctionCallSyntax
//@[024:0030) |   ├─IdentifierSyntax
//@[024:0030) |   | └─Token(Identifier) |reduce|
//@[030:0031) |   ├─Token(LeftParen) |(|
//@[031:0079) |   ├─FunctionArgumentSyntax
//@[031:0079) |   | └─ArraySyntax
//@[031:0032) |   |   ├─Token(LeftSquare) |[|
//@[032:0033) |   |   ├─Token(NewLine) |\n|
  { foo: 123 }
//@[002:0014) |   |   ├─ArrayItemSyntax
//@[002:0014) |   |   | └─ObjectSyntax
//@[002:0003) |   |   |   ├─Token(LeftBrace) |{|
//@[004:0012) |   |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   |   | | └─Token(Identifier) |foo|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0012) |   |   |   | └─IntegerLiteralSyntax
//@[009:0012) |   |   |   |   └─Token(Integer) |123|
//@[013:0014) |   |   |   └─Token(RightBrace) |}|
//@[014:0015) |   |   ├─Token(NewLine) |\n|
  { bar: 456 }
//@[002:0014) |   |   ├─ArrayItemSyntax
//@[002:0014) |   |   | └─ObjectSyntax
//@[002:0003) |   |   |   ├─Token(LeftBrace) |{|
//@[004:0012) |   |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   |   | | └─Token(Identifier) |bar|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0012) |   |   |   | └─IntegerLiteralSyntax
//@[009:0012) |   |   |   |   └─Token(Integer) |456|
//@[013:0014) |   |   |   └─Token(RightBrace) |}|
//@[014:0015) |   |   ├─Token(NewLine) |\n|
  { baz: 789 }
//@[002:0014) |   |   ├─ArrayItemSyntax
//@[002:0014) |   |   | └─ObjectSyntax
//@[002:0003) |   |   |   ├─Token(LeftBrace) |{|
//@[004:0012) |   |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   |   | | └─Token(Identifier) |baz|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0012) |   |   |   | └─IntegerLiteralSyntax
//@[009:0012) |   |   |   |   └─Token(Integer) |789|
//@[013:0014) |   |   |   └─Token(RightBrace) |}|
//@[014:0015) |   |   ├─Token(NewLine) |\n|
], {}, (cur, next) => union(cur, next))
//@[000:0001) |   |   └─Token(RightSquare) |]|
//@[001:0002) |   ├─Token(Comma) |,|
//@[003:0005) |   ├─FunctionArgumentSyntax
//@[003:0005) |   | └─ObjectSyntax
//@[003:0004) |   |   ├─Token(LeftBrace) |{|
//@[004:0005) |   |   └─Token(RightBrace) |}|
//@[005:0006) |   ├─Token(Comma) |,|
//@[007:0038) |   ├─FunctionArgumentSyntax
//@[007:0038) |   | └─LambdaSyntax
//@[007:0018) |   |   ├─VariableBlockSyntax
//@[007:0008) |   |   | ├─Token(LeftParen) |(|
//@[008:0011) |   |   | ├─LocalVariableSyntax
//@[008:0011) |   |   | | └─IdentifierSyntax
//@[008:0011) |   |   | |   └─Token(Identifier) |cur|
//@[011:0012) |   |   | ├─Token(Comma) |,|
//@[013:0017) |   |   | ├─LocalVariableSyntax
//@[013:0017) |   |   | | └─IdentifierSyntax
//@[013:0017) |   |   | |   └─Token(Identifier) |next|
//@[017:0018) |   |   | └─Token(RightParen) |)|
//@[019:0021) |   |   ├─Token(Arrow) |=>|
//@[022:0038) |   |   └─FunctionCallSyntax
//@[022:0027) |   |     ├─IdentifierSyntax
//@[022:0027) |   |     | └─Token(Identifier) |union|
//@[027:0028) |   |     ├─Token(LeftParen) |(|
//@[028:0031) |   |     ├─FunctionArgumentSyntax
//@[028:0031) |   |     | └─VariableAccessSyntax
//@[028:0031) |   |     |   └─IdentifierSyntax
//@[028:0031) |   |     |     └─Token(Identifier) |cur|
//@[031:0032) |   |     ├─Token(Comma) |,|
//@[033:0037) |   |     ├─FunctionArgumentSyntax
//@[033:0037) |   |     | └─VariableAccessSyntax
//@[033:0037) |   |     |   └─IdentifierSyntax
//@[033:0037) |   |     |     └─Token(Identifier) |next|
//@[037:0038) |   |     └─Token(RightParen) |)|
//@[038:0039) |   └─Token(RightParen) |)|
//@[039:0040) ├─Token(NewLine) |\n|
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[000:0051) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |reduceEmpty|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0051) | └─FunctionCallSyntax
//@[018:0024) |   ├─IdentifierSyntax
//@[018:0024) |   | └─Token(Identifier) |reduce|
//@[024:0025) |   ├─Token(LeftParen) |(|
//@[025:0027) |   ├─FunctionArgumentSyntax
//@[025:0027) |   | └─ArraySyntax
//@[025:0026) |   |   ├─Token(LeftSquare) |[|
//@[026:0027) |   |   └─Token(RightSquare) |]|
//@[027:0028) |   ├─Token(Comma) |,|
//@[029:0030) |   ├─FunctionArgumentSyntax
//@[029:0030) |   | └─IntegerLiteralSyntax
//@[029:0030) |   |   └─Token(Integer) |0|
//@[030:0031) |   ├─Token(Comma) |,|
//@[032:0050) |   ├─FunctionArgumentSyntax
//@[032:0050) |   | └─LambdaSyntax
//@[032:0043) |   |   ├─VariableBlockSyntax
//@[032:0033) |   |   | ├─Token(LeftParen) |(|
//@[033:0036) |   |   | ├─LocalVariableSyntax
//@[033:0036) |   |   | | └─IdentifierSyntax
//@[033:0036) |   |   | |   └─Token(Identifier) |cur|
//@[036:0037) |   |   | ├─Token(Comma) |,|
//@[038:0042) |   |   | ├─LocalVariableSyntax
//@[038:0042) |   |   | | └─IdentifierSyntax
//@[038:0042) |   |   | |   └─Token(Identifier) |next|
//@[042:0043) |   |   | └─Token(RightParen) |)|
//@[044:0046) |   |   ├─Token(Arrow) |=>|
//@[047:0050) |   |   └─VariableAccessSyntax
//@[047:0050) |   |     └─IdentifierSyntax
//@[047:0050) |   |       └─Token(Identifier) |cur|
//@[050:0051) |   └─Token(RightParen) |)|
//@[051:0053) ├─Token(NewLine) |\n\n|

var itemForLoop = [for item in range(0, 10): item]
//@[000:0050) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |itemForLoop|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0050) | └─ForSyntax
//@[018:0019) |   ├─Token(LeftSquare) |[|
//@[019:0022) |   ├─Token(Identifier) |for|
//@[023:0027) |   ├─LocalVariableSyntax
//@[023:0027) |   | └─IdentifierSyntax
//@[023:0027) |   |   └─Token(Identifier) |item|
//@[028:0030) |   ├─Token(Identifier) |in|
//@[031:0043) |   ├─FunctionCallSyntax
//@[031:0036) |   | ├─IdentifierSyntax
//@[031:0036) |   | | └─Token(Identifier) |range|
//@[036:0037) |   | ├─Token(LeftParen) |(|
//@[037:0038) |   | ├─FunctionArgumentSyntax
//@[037:0038) |   | | └─IntegerLiteralSyntax
//@[037:0038) |   | |   └─Token(Integer) |0|
//@[038:0039) |   | ├─Token(Comma) |,|
//@[040:0042) |   | ├─FunctionArgumentSyntax
//@[040:0042) |   | | └─IntegerLiteralSyntax
//@[040:0042) |   | |   └─Token(Integer) |10|
//@[042:0043) |   | └─Token(RightParen) |)|
//@[043:0044) |   ├─Token(Colon) |:|
//@[045:0049) |   ├─VariableAccessSyntax
//@[045:0049) |   | └─IdentifierSyntax
//@[045:0049) |   |   └─Token(Identifier) |item|
//@[049:0050) |   └─Token(RightSquare) |]|
//@[050:0051) ├─Token(NewLine) |\n|
var filteredLoop = filter(itemForLoop, i => i > 5)
//@[000:0050) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |filteredLoop|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0050) | └─FunctionCallSyntax
//@[019:0025) |   ├─IdentifierSyntax
//@[019:0025) |   | └─Token(Identifier) |filter|
//@[025:0026) |   ├─Token(LeftParen) |(|
//@[026:0037) |   ├─FunctionArgumentSyntax
//@[026:0037) |   | └─VariableAccessSyntax
//@[026:0037) |   |   └─IdentifierSyntax
//@[026:0037) |   |     └─Token(Identifier) |itemForLoop|
//@[037:0038) |   ├─Token(Comma) |,|
//@[039:0049) |   ├─FunctionArgumentSyntax
//@[039:0049) |   | └─LambdaSyntax
//@[039:0040) |   |   ├─LocalVariableSyntax
//@[039:0040) |   |   | └─IdentifierSyntax
//@[039:0040) |   |   |   └─Token(Identifier) |i|
//@[041:0043) |   |   ├─Token(Arrow) |=>|
//@[044:0049) |   |   └─BinaryOperationSyntax
//@[044:0045) |   |     ├─VariableAccessSyntax
//@[044:0045) |   |     | └─IdentifierSyntax
//@[044:0045) |   |     |   └─Token(Identifier) |i|
//@[046:0047) |   |     ├─Token(GreaterThan) |>|
//@[048:0049) |   |     └─IntegerLiteralSyntax
//@[048:0049) |   |       └─Token(Integer) |5|
//@[049:0050) |   └─Token(RightParen) |)|
//@[050:0052) ├─Token(NewLine) |\n\n|

output doggoGreetings array = [for item in mapObject: item.greeting]
//@[000:0068) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0021) | ├─IdentifierSyntax
//@[007:0021) | | └─Token(Identifier) |doggoGreetings|
//@[022:0027) | ├─SimpleTypeSyntax
//@[022:0027) | | └─Token(Identifier) |array|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0068) | └─ForSyntax
//@[030:0031) |   ├─Token(LeftSquare) |[|
//@[031:0034) |   ├─Token(Identifier) |for|
//@[035:0039) |   ├─LocalVariableSyntax
//@[035:0039) |   | └─IdentifierSyntax
//@[035:0039) |   |   └─Token(Identifier) |item|
//@[040:0042) |   ├─Token(Identifier) |in|
//@[043:0052) |   ├─VariableAccessSyntax
//@[043:0052) |   | └─IdentifierSyntax
//@[043:0052) |   |   └─Token(Identifier) |mapObject|
//@[052:0053) |   ├─Token(Colon) |:|
//@[054:0067) |   ├─PropertyAccessSyntax
//@[054:0058) |   | ├─VariableAccessSyntax
//@[054:0058) |   | | └─IdentifierSyntax
//@[054:0058) |   | |   └─Token(Identifier) |item|
//@[058:0059) |   | ├─Token(Dot) |.|
//@[059:0067) |   | └─IdentifierSyntax
//@[059:0067) |   |   └─Token(Identifier) |greeting|
//@[067:0068) |   └─Token(RightSquare) |]|
//@[068:0070) ├─Token(NewLine) |\n\n|

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
//@[000:0100) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0019) | ├─IdentifierSyntax
//@[009:0019) | | └─Token(Identifier) |storageAcc|
//@[020:0066) | ├─StringSyntax
//@[020:0066) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2021-09-01'|
//@[067:0075) | ├─Token(Identifier) |existing|
//@[076:0077) | ├─Token(Assignment) |=|
//@[078:0100) | └─ObjectSyntax
//@[078:0079) |   ├─Token(LeftBrace) |{|
//@[079:0080) |   ├─Token(NewLine) |\n|
  name: 'asdfsadf'
//@[002:0018) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0018) |   | └─StringSyntax
//@[008:0018) |   |   └─Token(StringComplete) |'asdfsadf'|
//@[018:0019) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)
//@[000:0093) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |mappedResProps|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0093) | └─FunctionCallSyntax
//@[021:0024) |   ├─IdentifierSyntax
//@[021:0024) |   | └─Token(Identifier) |map|
//@[024:0025) |   ├─Token(LeftParen) |(|
//@[025:0072) |   ├─FunctionArgumentSyntax
//@[025:0072) |   | └─FunctionCallSyntax
//@[025:0030) |   |   ├─IdentifierSyntax
//@[025:0030) |   |   | └─Token(Identifier) |items|
//@[030:0031) |   |   ├─Token(LeftParen) |(|
//@[031:0071) |   |   ├─FunctionArgumentSyntax
//@[031:0071) |   |   | └─PropertyAccessSyntax
//@[031:0052) |   |   |   ├─PropertyAccessSyntax
//@[031:0041) |   |   |   | ├─VariableAccessSyntax
//@[031:0041) |   |   |   | | └─IdentifierSyntax
//@[031:0041) |   |   |   | |   └─Token(Identifier) |storageAcc|
//@[041:0042) |   |   |   | ├─Token(Dot) |.|
//@[042:0052) |   |   |   | └─IdentifierSyntax
//@[042:0052) |   |   |   |   └─Token(Identifier) |properties|
//@[052:0053) |   |   |   ├─Token(Dot) |.|
//@[053:0071) |   |   |   └─IdentifierSyntax
//@[053:0071) |   |   |     └─Token(Identifier) |secondaryEndpoints|
//@[071:0072) |   |   └─Token(RightParen) |)|
//@[072:0073) |   ├─Token(Comma) |,|
//@[074:0092) |   ├─FunctionArgumentSyntax
//@[074:0092) |   | └─LambdaSyntax
//@[074:0078) |   |   ├─LocalVariableSyntax
//@[074:0078) |   |   | └─IdentifierSyntax
//@[074:0078) |   |   |   └─Token(Identifier) |item|
//@[079:0081) |   |   ├─Token(Arrow) |=>|
//@[082:0092) |   |   └─PropertyAccessSyntax
//@[082:0086) |   |     ├─VariableAccessSyntax
//@[082:0086) |   |     | └─IdentifierSyntax
//@[082:0086) |   |     |   └─Token(Identifier) |item|
//@[086:0087) |   |     ├─Token(Dot) |.|
//@[087:0092) |   |     └─IdentifierSyntax
//@[087:0092) |   |       └─Token(Identifier) |value|
//@[092:0093) |   └─Token(RightParen) |)|
//@[093:0095) ├─Token(NewLine) |\n\n|

module myMod './test.bicep' = {
//@[000:0117) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0012) | ├─IdentifierSyntax
//@[007:0012) | | └─Token(Identifier) |myMod|
//@[013:0027) | ├─StringSyntax
//@[013:0027) | | └─Token(StringComplete) |'./test.bicep'|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0117) | └─ObjectSyntax
//@[030:0031) |   ├─Token(LeftBrace) |{|
//@[031:0032) |   ├─Token(NewLine) |\n|
  name: 'asdfsadf'
//@[002:0018) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0018) |   | └─StringSyntax
//@[008:0018) |   |   └─Token(StringComplete) |'asdfsadf'|
//@[018:0019) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0064) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0064) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    outputThis: map(mapObject, obj => obj.doggo)
//@[004:0048) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |outputThis|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0048) |   |   | └─FunctionCallSyntax
//@[016:0019) |   |   |   ├─IdentifierSyntax
//@[016:0019) |   |   |   | └─Token(Identifier) |map|
//@[019:0020) |   |   |   ├─Token(LeftParen) |(|
//@[020:0029) |   |   |   ├─FunctionArgumentSyntax
//@[020:0029) |   |   |   | └─VariableAccessSyntax
//@[020:0029) |   |   |   |   └─IdentifierSyntax
//@[020:0029) |   |   |   |     └─Token(Identifier) |mapObject|
//@[029:0030) |   |   |   ├─Token(Comma) |,|
//@[031:0047) |   |   |   ├─FunctionArgumentSyntax
//@[031:0047) |   |   |   | └─LambdaSyntax
//@[031:0034) |   |   |   |   ├─LocalVariableSyntax
//@[031:0034) |   |   |   |   | └─IdentifierSyntax
//@[031:0034) |   |   |   |   |   └─Token(Identifier) |obj|
//@[035:0037) |   |   |   |   ├─Token(Arrow) |=>|
//@[038:0047) |   |   |   |   └─PropertyAccessSyntax
//@[038:0041) |   |   |   |     ├─VariableAccessSyntax
//@[038:0041) |   |   |   |     | └─IdentifierSyntax
//@[038:0041) |   |   |   |     |   └─Token(Identifier) |obj|
//@[041:0042) |   |   |   |     ├─Token(Dot) |.|
//@[042:0047) |   |   |   |     └─IdentifierSyntax
//@[042:0047) |   |   |   |       └─Token(Identifier) |doggo|
//@[047:0048) |   |   |   └─Token(RightParen) |)|
//@[048:0049) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')
//@[000:0087) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0024) | ├─IdentifierSyntax
//@[004:0024) | | └─Token(Identifier) |mappedModOutputProps|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0087) | └─FunctionCallSyntax
//@[027:0030) |   ├─IdentifierSyntax
//@[027:0030) |   | └─Token(Identifier) |map|
//@[030:0031) |   ├─Token(LeftParen) |(|
//@[031:0055) |   ├─FunctionArgumentSyntax
//@[031:0055) |   | └─PropertyAccessSyntax
//@[031:0044) |   |   ├─PropertyAccessSyntax
//@[031:0036) |   |   | ├─VariableAccessSyntax
//@[031:0036) |   |   | | └─IdentifierSyntax
//@[031:0036) |   |   | |   └─Token(Identifier) |myMod|
//@[036:0037) |   |   | ├─Token(Dot) |.|
//@[037:0044) |   |   | └─IdentifierSyntax
//@[037:0044) |   |   |   └─Token(Identifier) |outputs|
//@[044:0045) |   |   ├─Token(Dot) |.|
//@[045:0055) |   |   └─IdentifierSyntax
//@[045:0055) |   |     └─Token(Identifier) |outputThis|
//@[055:0056) |   ├─Token(Comma) |,|
//@[057:0086) |   ├─FunctionArgumentSyntax
//@[057:0086) |   | └─LambdaSyntax
//@[057:0062) |   |   ├─LocalVariableSyntax
//@[057:0062) |   |   | └─IdentifierSyntax
//@[057:0062) |   |   |   └─Token(Identifier) |doggo|
//@[063:0065) |   |   ├─Token(Arrow) |=>|
//@[066:0086) |   |   └─StringSyntax
//@[066:0069) |   |     ├─Token(StringLeftPiece) |'${|
//@[069:0074) |   |     ├─VariableAccessSyntax
//@[069:0074) |   |     | └─IdentifierSyntax
//@[069:0074) |   |     |   └─Token(Identifier) |doggo|
//@[074:0086) |   |     └─Token(StringRightPiece) |} says bork'|
//@[086:0087) |   └─Token(RightParen) |)|
//@[087:0089) ├─Token(NewLine) |\n\n|

var parentheses = map([123], (i => '${i}'))
//@[000:0043) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |parentheses|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0043) | └─FunctionCallSyntax
//@[018:0021) |   ├─IdentifierSyntax
//@[018:0021) |   | └─Token(Identifier) |map|
//@[021:0022) |   ├─Token(LeftParen) |(|
//@[022:0027) |   ├─FunctionArgumentSyntax
//@[022:0027) |   | └─ArraySyntax
//@[022:0023) |   |   ├─Token(LeftSquare) |[|
//@[023:0026) |   |   ├─ArrayItemSyntax
//@[023:0026) |   |   | └─IntegerLiteralSyntax
//@[023:0026) |   |   |   └─Token(Integer) |123|
//@[026:0027) |   |   └─Token(RightSquare) |]|
//@[027:0028) |   ├─Token(Comma) |,|
//@[029:0042) |   ├─FunctionArgumentSyntax
//@[029:0042) |   | └─ParenthesizedExpressionSyntax
//@[029:0030) |   |   ├─Token(LeftParen) |(|
//@[030:0041) |   |   ├─LambdaSyntax
//@[030:0031) |   |   | ├─LocalVariableSyntax
//@[030:0031) |   |   | | └─IdentifierSyntax
//@[030:0031) |   |   | |   └─Token(Identifier) |i|
//@[032:0034) |   |   | ├─Token(Arrow) |=>|
//@[035:0041) |   |   | └─StringSyntax
//@[035:0038) |   |   |   ├─Token(StringLeftPiece) |'${|
//@[038:0039) |   |   |   ├─VariableAccessSyntax
//@[038:0039) |   |   |   | └─IdentifierSyntax
//@[038:0039) |   |   |   |   └─Token(Identifier) |i|
//@[039:0041) |   |   |   └─Token(StringRightPiece) |}'|
//@[041:0042) |   |   └─Token(RightParen) |)|
//@[042:0043) |   └─Token(RightParen) |)|
//@[043:0044) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
