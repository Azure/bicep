////////////////////////////////////////////////////////////////////////////////
//@[000:3351) ProgramSyntax
//@[080:0081) ├─Token(NewLine) |\n|
//////////////////////////// Baselines for width 40 ////////////////////////////
//@[080:0081) ├─Token(NewLine) |\n|
////////////////////////////////////////////////////////////////////////////////
//@[080:0081) ├─Token(NewLine) |\n|
var w38 = [true, /* xxxxx */ true, 12]      // suffix
//@[000:0038) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w38|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0038) | └─ArraySyntax
//@[010:0011) |   ├─Token(LeftSquare) |[|
//@[011:0015) |   ├─ArrayItemSyntax
//@[011:0015) |   | └─BooleanLiteralSyntax
//@[011:0015) |   |   └─Token(TrueKeyword) |true|
//@[015:0016) |   ├─Token(Comma) |,|
//@[029:0033) |   ├─ArrayItemSyntax
//@[029:0033) |   | └─BooleanLiteralSyntax
//@[029:0033) |   |   └─Token(TrueKeyword) |true|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0037) |   ├─ArrayItemSyntax
//@[035:0037) |   | └─IntegerLiteralSyntax
//@[035:0037) |   |   └─Token(Integer) |12|
//@[037:0038) |   └─Token(RightSquare) |]|
//@[053:0054) ├─Token(NewLine) |\n|
var w39 = [true, true
//@[000:0042) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w39|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0042) | └─ArraySyntax
//@[010:0011) |   ├─Token(LeftSquare) |[|
//@[011:0015) |   ├─ArrayItemSyntax
//@[011:0015) |   | └─BooleanLiteralSyntax
//@[011:0015) |   |   └─Token(TrueKeyword) |true|
//@[015:0016) |   ├─Token(Comma) |,|
//@[017:0021) |   ├─ArrayItemSyntax
//@[017:0021) |   | └─BooleanLiteralSyntax
//@[017:0021) |   |   └─Token(TrueKeyword) |true|
//@[021:0022) |   ├─Token(NewLine) |\n|
    true, true, 123]
//@[004:0008) |   ├─ArrayItemSyntax
//@[004:0008) |   | └─BooleanLiteralSyntax
//@[004:0008) |   |   └─Token(TrueKeyword) |true|
//@[008:0009) |   ├─Token(Comma) |,|
//@[010:0014) |   ├─ArrayItemSyntax
//@[010:0014) |   | └─BooleanLiteralSyntax
//@[010:0014) |   |   └─Token(TrueKeyword) |true|
//@[014:0015) |   ├─Token(Comma) |,|
//@[016:0019) |   ├─ArrayItemSyntax
//@[016:0019) |   | └─IntegerLiteralSyntax
//@[016:0019) |   |   └─Token(Integer) |123|
//@[019:0020) |   └─Token(RightSquare) |]|
//@[020:0021) ├─Token(NewLine) |\n|
var w40 =[
//@[000:0043) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w40|
//@[008:0009) | ├─Token(Assignment) |=|
//@[009:0043) | └─ArraySyntax
//@[009:0010) |   ├─Token(LeftSquare) |[|
//@[010:0011) |   ├─Token(NewLine) |\n|
    true, true, 1234/* xxxxx */]  // suffix
//@[004:0008) |   ├─ArrayItemSyntax
//@[004:0008) |   | └─BooleanLiteralSyntax
//@[004:0008) |   |   └─Token(TrueKeyword) |true|
//@[008:0009) |   ├─Token(Comma) |,|
//@[010:0014) |   ├─ArrayItemSyntax
//@[010:0014) |   | └─BooleanLiteralSyntax
//@[010:0014) |   |   └─Token(TrueKeyword) |true|
//@[014:0015) |   ├─Token(Comma) |,|
//@[016:0020) |   ├─ArrayItemSyntax
//@[016:0020) |   | └─IntegerLiteralSyntax
//@[016:0020) |   |   └─Token(Integer) |1234|
//@[031:0032) |   └─Token(RightSquare) |]|
//@[043:0044) ├─Token(NewLine) |\n|
var w41 =[ true, true, true, true, 12345 ]
//@[000:0042) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w41|
//@[008:0009) | ├─Token(Assignment) |=|
//@[009:0042) | └─ArraySyntax
//@[009:0010) |   ├─Token(LeftSquare) |[|
//@[011:0015) |   ├─ArrayItemSyntax
//@[011:0015) |   | └─BooleanLiteralSyntax
//@[011:0015) |   |   └─Token(TrueKeyword) |true|
//@[015:0016) |   ├─Token(Comma) |,|
//@[017:0021) |   ├─ArrayItemSyntax
//@[017:0021) |   | └─BooleanLiteralSyntax
//@[017:0021) |   |   └─Token(TrueKeyword) |true|
//@[021:0022) |   ├─Token(Comma) |,|
//@[023:0027) |   ├─ArrayItemSyntax
//@[023:0027) |   | └─BooleanLiteralSyntax
//@[023:0027) |   |   └─Token(TrueKeyword) |true|
//@[027:0028) |   ├─Token(Comma) |,|
//@[029:0033) |   ├─ArrayItemSyntax
//@[029:0033) |   | └─BooleanLiteralSyntax
//@[029:0033) |   |   └─Token(TrueKeyword) |true|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0040) |   ├─ArrayItemSyntax
//@[035:0040) |   | └─IntegerLiteralSyntax
//@[035:0040) |   |   └─Token(Integer) |12345|
//@[041:0042) |   └─Token(RightSquare) |]|
//@[042:0043) ├─Token(NewLine) |\n|
var w42 =[true, /* xxx */ 12 /* xx */, 1]
//@[000:0041) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w42|
//@[008:0009) | ├─Token(Assignment) |=|
//@[009:0041) | └─ArraySyntax
//@[009:0010) |   ├─Token(LeftSquare) |[|
//@[010:0014) |   ├─ArrayItemSyntax
//@[010:0014) |   | └─BooleanLiteralSyntax
//@[010:0014) |   |   └─Token(TrueKeyword) |true|
//@[014:0015) |   ├─Token(Comma) |,|
//@[026:0028) |   ├─ArrayItemSyntax
//@[026:0028) |   | └─IntegerLiteralSyntax
//@[026:0028) |   |   └─Token(Integer) |12|
//@[037:0038) |   ├─Token(Comma) |,|
//@[039:0040) |   ├─ArrayItemSyntax
//@[039:0040) |   | └─IntegerLiteralSyntax
//@[039:0040) |   |   └─Token(Integer) |1|
//@[040:0041) |   └─Token(RightSquare) |]|
//@[041:0043) ├─Token(NewLine) |\n\n|

var w38_= { foo: true, bar: 1234567
//@[000:0037) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |w38_|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0037) | └─ObjectSyntax
//@[010:0011) |   ├─Token(LeftBrace) |{|
//@[012:0021) |   ├─ObjectPropertySyntax
//@[012:0015) |   | ├─IdentifierSyntax
//@[012:0015) |   | | └─Token(Identifier) |foo|
//@[015:0016) |   | ├─Token(Colon) |:|
//@[017:0021) |   | └─BooleanLiteralSyntax
//@[017:0021) |   |   └─Token(TrueKeyword) |true|
//@[021:0022) |   ├─Token(Comma) |,|
//@[023:0035) |   ├─ObjectPropertySyntax
//@[023:0026) |   | ├─IdentifierSyntax
//@[023:0026) |   | | └─Token(Identifier) |bar|
//@[026:0027) |   | ├─Token(Colon) |:|
//@[028:0035) |   | └─IntegerLiteralSyntax
//@[028:0035) |   |   └─Token(Integer) |1234567|
//@[035:0036) |   ├─Token(NewLine) |\n|
} // suffix
//@[000:0001) |   └─Token(RightBrace) |}|
//@[011:0012) ├─Token(NewLine) |\n|
var        w39_= { foo: true
//@[000:0046) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[011:0015) | ├─IdentifierSyntax
//@[011:0015) | | └─Token(Identifier) |w39_|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0046) | └─ObjectSyntax
//@[017:0018) |   ├─Token(LeftBrace) |{|
//@[019:0028) |   ├─ObjectPropertySyntax
//@[019:0022) |   | ├─IdentifierSyntax
//@[019:0022) |   | | └─Token(Identifier) |foo|
//@[022:0023) |   | ├─Token(Colon) |:|
//@[024:0028) |   | └─BooleanLiteralSyntax
//@[024:0028) |   |   └─Token(TrueKeyword) |true|
//@[028:0029) |   ├─Token(NewLine) |\n|
  bar: 12345678 } // suffix
//@[002:0015) |   ├─ObjectPropertySyntax
//@[002:0005) |   | ├─IdentifierSyntax
//@[002:0005) |   | | └─Token(Identifier) |bar|
//@[005:0006) |   | ├─Token(Colon) |:|
//@[007:0015) |   | └─IntegerLiteralSyntax
//@[007:0015) |   |   └─Token(Integer) |12345678|
//@[016:0017) |   └─Token(RightBrace) |}|
//@[027:0028) ├─Token(NewLine) |\n|
    var w40_= { foo: 1, bar:    1 /* xxxx */ }
//@[004:0046) ├─VariableDeclarationSyntax
//@[004:0007) | ├─Token(Identifier) |var|
//@[008:0012) | ├─IdentifierSyntax
//@[008:0012) | | └─Token(Identifier) |w40_|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0046) | └─ObjectSyntax
//@[014:0015) |   ├─Token(LeftBrace) |{|
//@[016:0022) |   ├─ObjectPropertySyntax
//@[016:0019) |   | ├─IdentifierSyntax
//@[016:0019) |   | | └─Token(Identifier) |foo|
//@[019:0020) |   | ├─Token(Colon) |:|
//@[021:0022) |   | └─IntegerLiteralSyntax
//@[021:0022) |   |   └─Token(Integer) |1|
//@[022:0023) |   ├─Token(Comma) |,|
//@[024:0033) |   ├─ObjectPropertySyntax
//@[024:0027) |   | ├─IdentifierSyntax
//@[024:0027) |   | | └─Token(Identifier) |bar|
//@[027:0028) |   | ├─Token(Colon) |:|
//@[032:0033) |   | └─IntegerLiteralSyntax
//@[032:0033) |   |   └─Token(Integer) |1|
//@[045:0046) |   └─Token(RightBrace) |}|
//@[046:0047) ├─Token(NewLine) |\n|
var w41_={ foo: true, bar    : 1234567890 }
//@[000:0043) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |w41_|
//@[008:0009) | ├─Token(Assignment) |=|
//@[009:0043) | └─ObjectSyntax
//@[009:0010) |   ├─Token(LeftBrace) |{|
//@[011:0020) |   ├─ObjectPropertySyntax
//@[011:0014) |   | ├─IdentifierSyntax
//@[011:0014) |   | | └─Token(Identifier) |foo|
//@[014:0015) |   | ├─Token(Colon) |:|
//@[016:0020) |   | └─BooleanLiteralSyntax
//@[016:0020) |   |   └─Token(TrueKeyword) |true|
//@[020:0021) |   ├─Token(Comma) |,|
//@[022:0041) |   ├─ObjectPropertySyntax
//@[022:0025) |   | ├─IdentifierSyntax
//@[022:0025) |   | | └─Token(Identifier) |bar|
//@[029:0030) |   | ├─Token(Colon) |:|
//@[031:0041) |   | └─IntegerLiteralSyntax
//@[031:0041) |   |   └─Token(Integer) |1234567890|
//@[042:0043) |   └─Token(RightBrace) |}|
//@[043:0044) ├─Token(NewLine) |\n|
var w42_= { foo: true
//@[000:0044) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |w42_|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0044) | └─ObjectSyntax
//@[010:0011) |   ├─Token(LeftBrace) |{|
//@[012:0021) |   ├─ObjectPropertySyntax
//@[012:0015) |   | ├─IdentifierSyntax
//@[012:0015) |   | | └─Token(Identifier) |foo|
//@[015:0016) |   | ├─Token(Colon) |:|
//@[017:0021) |   | └─BooleanLiteralSyntax
//@[017:0021) |   |   └─Token(TrueKeyword) |true|
//@[021:0022) |   ├─Token(NewLine) |\n|
    bar: 12345678901 } // suffix
//@[004:0020) |   ├─ObjectPropertySyntax
//@[004:0007) |   | ├─IdentifierSyntax
//@[004:0007) |   | | └─Token(Identifier) |bar|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0020) |   | └─IntegerLiteralSyntax
//@[009:0020) |   |   └─Token(Integer) |12345678901|
//@[021:0022) |   └─Token(RightBrace) |}|
//@[032:0034) ├─Token(NewLine) |\n\n|

   var w38__ =    concat('xxxxxx', 'xxxxxx')
//@[003:0044) ├─VariableDeclarationSyntax
//@[003:0006) | ├─Token(Identifier) |var|
//@[007:0012) | ├─IdentifierSyntax
//@[007:0012) | | └─Token(Identifier) |w38__|
//@[013:0014) | ├─Token(Assignment) |=|
//@[018:0044) | └─FunctionCallSyntax
//@[018:0024) |   ├─IdentifierSyntax
//@[018:0024) |   | └─Token(Identifier) |concat|
//@[024:0025) |   ├─Token(LeftParen) |(|
//@[025:0033) |   ├─FunctionArgumentSyntax
//@[025:0033) |   | └─StringSyntax
//@[025:0033) |   |   └─Token(StringComplete) |'xxxxxx'|
//@[033:0034) |   ├─Token(Comma) |,|
//@[035:0043) |   ├─FunctionArgumentSyntax
//@[035:0043) |   | └─StringSyntax
//@[035:0043) |   |   └─Token(StringComplete) |'xxxxxx'|
//@[043:0044) |   └─Token(RightParen) |)|
//@[044:0045) ├─Token(NewLine) |\n|
var w39__ = concat('xxxxxx', 'xxxxxxx'
//@[000:0040) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |w39__|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0040) | └─FunctionCallSyntax
//@[012:0018) |   ├─IdentifierSyntax
//@[012:0018) |   | └─Token(Identifier) |concat|
//@[018:0019) |   ├─Token(LeftParen) |(|
//@[019:0027) |   ├─FunctionArgumentSyntax
//@[019:0027) |   | └─StringSyntax
//@[019:0027) |   |   └─Token(StringComplete) |'xxxxxx'|
//@[027:0028) |   ├─Token(Comma) |,|
//@[029:0038) |   ├─FunctionArgumentSyntax
//@[029:0038) |   | └─StringSyntax
//@[029:0038) |   |   └─Token(StringComplete) |'xxxxxxx'|
//@[038:0039) |   ├─Token(NewLine) |\n|
) // suffix
//@[000:0001) |   └─Token(RightParen) |)|
//@[011:0012) ├─Token(NewLine) |\n|
var w40__ = concat('xxxxxx',
//@[000:0040) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |w40__|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0040) | └─FunctionCallSyntax
//@[012:0018) |   ├─IdentifierSyntax
//@[012:0018) |   | └─Token(Identifier) |concat|
//@[018:0019) |   ├─Token(LeftParen) |(|
//@[019:0027) |   ├─FunctionArgumentSyntax
//@[019:0027) |   | └─StringSyntax
//@[019:0027) |   |   └─Token(StringComplete) |'xxxxxx'|
//@[027:0028) |   ├─Token(Comma) |,|
//@[028:0029) |   ├─Token(NewLine) |\n|
'xxxxxxxx') // suffix
//@[000:0010) |   ├─FunctionArgumentSyntax
//@[000:0010) |   | └─StringSyntax
//@[000:0010) |   |   └─Token(StringComplete) |'xxxxxxxx'|
//@[010:0011) |   └─Token(RightParen) |)|
//@[021:0023) ├─Token(NewLine) |\n\n|

var        w41__= concat('xxxxx'/* xxxxxxx */)
//@[000:0046) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[011:0016) | ├─IdentifierSyntax
//@[011:0016) | | └─Token(Identifier) |w41__|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0046) | └─FunctionCallSyntax
//@[018:0024) |   ├─IdentifierSyntax
//@[018:0024) |   | └─Token(Identifier) |concat|
//@[024:0025) |   ├─Token(LeftParen) |(|
//@[025:0032) |   ├─FunctionArgumentSyntax
//@[025:0032) |   | └─StringSyntax
//@[025:0032) |   |   └─Token(StringComplete) |'xxxxx'|
//@[045:0046) |   └─Token(RightParen) |)|
//@[046:0047) ├─Token(NewLine) |\n|
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@[000:0042) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |w42__|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0042) | └─FunctionCallSyntax
//@[012:0018) |   ├─IdentifierSyntax
//@[012:0018) |   | └─Token(Identifier) |concat|
//@[018:0019) |   ├─Token(LeftParen) |(|
//@[019:0026) |   ├─FunctionArgumentSyntax
//@[019:0026) |   | └─StringSyntax
//@[019:0026) |   |   └─Token(StringComplete) |'xxxxx'|
//@[026:0027) |   ├─Token(Comma) |,|
//@[028:0041) |   ├─FunctionArgumentSyntax
//@[028:0041) |   | └─StringSyntax
//@[028:0041) |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[041:0042) |   └─Token(RightParen) |)|
//@[042:0044) ├─Token(NewLine) |\n\n|

////////////////////////////////////////////////////////////////////////////////
//@[080:0081) ├─Token(NewLine) |\n|
//////////////////////////// Baselines for width 80 ////////////////////////////
//@[080:0081) ├─Token(NewLine) |\n|
////////////////////////////////////////////////////////////////////////////////
//@[080:0081) ├─Token(NewLine) |\n|
var w78 = [
//@[000:0083) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w78|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0083) | └─ArraySyntax
//@[010:0011) |   ├─Token(LeftSquare) |[|
//@[011:0012) |   ├─Token(NewLine) |\n|
    true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxx' ]
//@[004:0008) |   ├─ArrayItemSyntax
//@[004:0008) |   | └─BooleanLiteralSyntax
//@[004:0008) |   |   └─Token(TrueKeyword) |true|
//@[008:0009) |   ├─Token(Comma) |,|
//@[010:0047) |   ├─ArrayItemSyntax
//@[010:0047) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[012:0035) |   |   ├─ObjectPropertySyntax
//@[012:0015) |   |   | ├─IdentifierSyntax
//@[012:0015) |   |   | | └─Token(Identifier) |foo|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0035) |   |   | └─StringSyntax
//@[017:0035) |   |   |   └─Token(StringComplete) |'object width: 37'|
//@[046:0047) |   |   └─Token(RightBrace) |}|
//@[047:0048) |   ├─Token(Comma) |,|
//@[049:0069) |   ├─ArrayItemSyntax
//@[049:0069) |   | └─StringSyntax
//@[049:0069) |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxx'|
//@[070:0071) |   └─Token(RightSquare) |]|
//@[071:0072) ├─Token(NewLine) |\n|
var w79 = [true
//@[000:0085) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w79|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0085) | └─ArraySyntax
//@[010:0011) |   ├─Token(LeftSquare) |[|
//@[011:0015) |   ├─ArrayItemSyntax
//@[011:0015) |   | └─BooleanLiteralSyntax
//@[011:0015) |   |   └─Token(TrueKeyword) |true|
//@[015:0016) |   ├─Token(NewLine) |\n|
    { /* xxxx */ foo: 'object width: 38' }
//@[004:0042) |   ├─ArrayItemSyntax
//@[004:0042) |   | └─ObjectSyntax
//@[004:0005) |   |   ├─Token(LeftBrace) |{|
//@[017:0040) |   |   ├─ObjectPropertySyntax
//@[017:0020) |   |   | ├─IdentifierSyntax
//@[017:0020) |   |   | | └─Token(Identifier) |foo|
//@[020:0021) |   |   | ├─Token(Colon) |:|
//@[022:0040) |   |   | └─StringSyntax
//@[022:0040) |   |   |   └─Token(StringComplete) |'object width: 38'|
//@[041:0042) |   |   └─Token(RightBrace) |}|
//@[042:0043) |   ├─Token(NewLine) |\n|
    'xxxxxxxxxxxxxxxxxx' ]
//@[004:0024) |   ├─ArrayItemSyntax
//@[004:0024) |   | └─StringSyntax
//@[004:0024) |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxx'|
//@[025:0026) |   └─Token(RightSquare) |]|
//@[026:0027) ├─Token(NewLine) |\n|
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }
//@[000:0083) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w80|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0083) | └─ArraySyntax
//@[010:0011) |   ├─Token(LeftSquare) |[|
//@[011:0015) |   ├─ArrayItemSyntax
//@[011:0015) |   | └─BooleanLiteralSyntax
//@[011:0015) |   |   └─Token(TrueKeyword) |true|
//@[015:0016) |   ├─Token(Comma) |,|
//@[017:0056) |   ├─ArrayItemSyntax
//@[017:0056) |   | └─ObjectSyntax
//@[017:0018) |   |   ├─Token(LeftBrace) |{|
//@[019:0054) |   |   ├─ObjectPropertySyntax
//@[019:0022) |   |   | ├─IdentifierSyntax
//@[019:0022) |   |   | | └─Token(Identifier) |foo|
//@[022:0023) |   |   | ├─Token(Colon) |:|
//@[024:0054) |   |   | └─StringSyntax
//@[024:0054) |   |   |   └─Token(StringComplete) |'object width: 39 xxxxxxxxxxx'|
//@[055:0056) |   |   └─Token(RightBrace) |}|
//@[056:0057) |   ├─Token(NewLine) |\n|
    'xxxxxxxxxxxxxxxxxxx']
//@[004:0025) |   ├─ArrayItemSyntax
//@[004:0025) |   | └─StringSyntax
//@[004:0025) |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxxx'|
//@[025:0026) |   └─Token(RightSquare) |]|
//@[026:0027) ├─Token(NewLine) |\n|
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxx' ]
//@[000:0081) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w81|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0081) | └─ArraySyntax
//@[010:0011) |   ├─Token(LeftSquare) |[|
//@[011:0015) |   ├─ArrayItemSyntax
//@[011:0015) |   | └─BooleanLiteralSyntax
//@[011:0015) |   |   └─Token(TrueKeyword) |true|
//@[015:0016) |   ├─Token(Comma) |,|
//@[017:0057) |   ├─ArrayItemSyntax
//@[017:0057) |   | └─ObjectSyntax
//@[017:0018) |   |   ├─Token(LeftBrace) |{|
//@[019:0055) |   |   ├─ObjectPropertySyntax
//@[019:0022) |   |   | ├─IdentifierSyntax
//@[019:0022) |   |   | | └─Token(Identifier) |foo|
//@[022:0023) |   |   | ├─Token(Colon) |:|
//@[024:0055) |   |   | └─StringSyntax
//@[024:0055) |   |   |   └─Token(StringComplete) |'object width: 40 xxxxxxxxxxxx'|
//@[056:0057) |   |   └─Token(RightBrace) |}|
//@[057:0058) |   ├─Token(Comma) |,|
//@[059:0079) |   ├─ArrayItemSyntax
//@[059:0079) |   | └─StringSyntax
//@[059:0079) |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxx'|
//@[080:0081) |   └─Token(RightSquare) |]|
//@[081:0082) ├─Token(NewLine) |\n|
var w82 = [  true, concat(/* function width: 41 */123, 456) /* xxxxxxxxxxxxxxxx */ ]
//@[000:0084) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |w82|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0084) | └─ArraySyntax
//@[010:0011) |   ├─Token(LeftSquare) |[|
//@[013:0017) |   ├─ArrayItemSyntax
//@[013:0017) |   | └─BooleanLiteralSyntax
//@[013:0017) |   |   └─Token(TrueKeyword) |true|
//@[017:0018) |   ├─Token(Comma) |,|
//@[019:0059) |   ├─ArrayItemSyntax
//@[019:0059) |   | └─FunctionCallSyntax
//@[019:0025) |   |   ├─IdentifierSyntax
//@[019:0025) |   |   | └─Token(Identifier) |concat|
//@[025:0026) |   |   ├─Token(LeftParen) |(|
//@[050:0053) |   |   ├─FunctionArgumentSyntax
//@[050:0053) |   |   | └─IntegerLiteralSyntax
//@[050:0053) |   |   |   └─Token(Integer) |123|
//@[053:0054) |   |   ├─Token(Comma) |,|
//@[055:0058) |   |   ├─FunctionArgumentSyntax
//@[055:0058) |   |   | └─IntegerLiteralSyntax
//@[055:0058) |   |   |   └─Token(Integer) |456|
//@[058:0059) |   |   └─Token(RightParen) |)|
//@[083:0084) |   └─Token(RightSquare) |]|
//@[084:0086) ├─Token(NewLine) |\n\n|

var w78_ ={ foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
//@[000:0077) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |w78_|
//@[009:0010) | ├─Token(Assignment) |=|
//@[010:0077) | └─ObjectSyntax
//@[010:0011) |   ├─Token(LeftBrace) |{|
//@[012:0020) |   ├─ObjectPropertySyntax
//@[012:0015) |   | ├─IdentifierSyntax
//@[012:0015) |   | | └─Token(Identifier) |foo|
//@[015:0016) |   | ├─Token(Colon) |:|
//@[017:0020) |   | └─IntegerLiteralSyntax
//@[017:0020) |   |   └─Token(Integer) |123|
//@[020:0021) |   ├─Token(Comma) |,|
//@[033:0075) |   ├─ObjectPropertySyntax
//@[033:0036) |   | ├─IdentifierSyntax
//@[033:0036) |   | | └─Token(Identifier) |baz|
//@[036:0037) |   | ├─Token(Colon) |:|
//@[038:0075) |   | └─ArraySyntax
//@[038:0039) |   |   ├─Token(LeftSquare) |[|
//@[039:0052) |   |   ├─ArrayItemSyntax
//@[039:0052) |   |   | └─StringSyntax
//@[039:0052) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[052:0053) |   |   ├─Token(Comma) |,|
//@[054:0074) |   |   ├─ArrayItemSyntax
//@[054:0074) |   |   | └─StringSyntax
//@[054:0074) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxx'|
//@[074:0075) |   |   └─Token(RightSquare) |]|
//@[076:0077) |   └─Token(RightBrace) |}|
//@[077:0078) ├─Token(NewLine) |\n|
/* should print a newline after this */ var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@[040:0108) ├─VariableDeclarationSyntax
//@[040:0043) | ├─Token(Identifier) |var|
//@[044:0048) | ├─IdentifierSyntax
//@[044:0048) | | └─Token(Identifier) |w79_|
//@[049:0050) | ├─Token(Assignment) |=|
//@[051:0108) | └─ObjectSyntax
//@[051:0052) |   ├─Token(LeftBrace) |{|
//@[053:0061) |   ├─ObjectPropertySyntax
//@[053:0056) |   | ├─IdentifierSyntax
//@[053:0056) |   | | └─Token(Identifier) |foo|
//@[056:0057) |   | ├─Token(Colon) |:|
//@[058:0061) |   | └─IntegerLiteralSyntax
//@[058:0061) |   |   └─Token(Integer) |123|
//@[061:0062) |   ├─Token(Comma) |,|
//@[063:0072) |   ├─ObjectPropertySyntax
//@[063:0066) |   | ├─IdentifierSyntax
//@[063:0066) |   | | └─Token(Identifier) |bar|
//@[066:0067) |   | ├─Token(Colon) |:|
//@[068:0072) |   | └─BooleanLiteralSyntax
//@[068:0072) |   |   └─Token(TrueKeyword) |true|
//@[072:0073) |   ├─Token(Comma) |,|
//@[074:0106) |   ├─ObjectPropertySyntax
//@[074:0077) |   | ├─IdentifierSyntax
//@[074:0077) |   | | └─Token(Identifier) |baz|
//@[077:0078) |   | ├─Token(Colon) |:|
//@[079:0106) |   | └─ArraySyntax
//@[079:0080) |   |   ├─Token(LeftSquare) |[|
//@[080:0093) |   |   ├─ArrayItemSyntax
//@[080:0093) |   |   | └─StringSyntax
//@[080:0093) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[093:0094) |   |   ├─Token(Comma) |,|
//@[095:0105) |   |   ├─ArrayItemSyntax
//@[095:0105) |   |   | └─StringSyntax
//@[095:0105) |   |   |   └─Token(StringComplete) |'xxxxxxxx'|
//@[105:0106) |   |   └─Token(RightSquare) |]|
//@[107:0108) |   └─Token(RightBrace) |}|
//@[108:0109) ├─Token(NewLine) |\n|
var w80_ = { foo: 123, bar: true, baz: [
//@[000:0085) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |w80_|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0085) | └─ObjectSyntax
//@[011:0012) |   ├─Token(LeftBrace) |{|
//@[013:0021) |   ├─ObjectPropertySyntax
//@[013:0016) |   | ├─IdentifierSyntax
//@[013:0016) |   | | └─Token(Identifier) |foo|
//@[016:0017) |   | ├─Token(Colon) |:|
//@[018:0021) |   | └─IntegerLiteralSyntax
//@[018:0021) |   |   └─Token(Integer) |123|
//@[021:0022) |   ├─Token(Comma) |,|
//@[023:0032) |   ├─ObjectPropertySyntax
//@[023:0026) |   | ├─IdentifierSyntax
//@[023:0026) |   | | └─Token(Identifier) |bar|
//@[026:0027) |   | ├─Token(Colon) |:|
//@[028:0032) |   | └─BooleanLiteralSyntax
//@[028:0032) |   |   └─Token(TrueKeyword) |true|
//@[032:0033) |   ├─Token(Comma) |,|
//@[034:0083) |   ├─ObjectPropertySyntax
//@[034:0037) |   | ├─IdentifierSyntax
//@[034:0037) |   | | └─Token(Identifier) |baz|
//@[037:0038) |   | ├─Token(Colon) |:|
//@[039:0083) |   | └─ArraySyntax
//@[039:0040) |   |   ├─Token(LeftSquare) |[|
//@[040:0041) |   |   ├─Token(NewLine) |\n|
    'xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxx'] } // suffix
//@[004:0017) |   |   ├─ArrayItemSyntax
//@[004:0017) |   |   | └─StringSyntax
//@[004:0017) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[017:0018) |   |   ├─Token(Comma) |,|
//@[019:0041) |   |   ├─ArrayItemSyntax
//@[019:0041) |   |   | └─StringSyntax
//@[019:0041) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxxxx'|
//@[041:0042) |   |   └─Token(RightSquare) |]|
//@[043:0044) |   └─Token(RightBrace) |}|
//@[054:0055) ├─Token(NewLine) |\n|
var w81_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx'] }
//@[000:0081) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |w81_|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0081) | └─ObjectSyntax
//@[011:0012) |   ├─Token(LeftBrace) |{|
//@[013:0021) |   ├─ObjectPropertySyntax
//@[013:0016) |   | ├─IdentifierSyntax
//@[013:0016) |   | | └─Token(Identifier) |foo|
//@[016:0017) |   | ├─Token(Colon) |:|
//@[018:0021) |   | └─IntegerLiteralSyntax
//@[018:0021) |   |   └─Token(Integer) |123|
//@[021:0022) |   ├─Token(Comma) |,|
//@[023:0032) |   ├─ObjectPropertySyntax
//@[023:0026) |   | ├─IdentifierSyntax
//@[023:0026) |   | | └─Token(Identifier) |bar|
//@[026:0027) |   | ├─Token(Colon) |:|
//@[028:0032) |   | └─BooleanLiteralSyntax
//@[028:0032) |   |   └─Token(TrueKeyword) |true|
//@[032:0033) |   ├─Token(Comma) |,|
//@[034:0079) |   ├─ObjectPropertySyntax
//@[034:0037) |   | ├─IdentifierSyntax
//@[034:0037) |   | | └─Token(Identifier) |baz|
//@[037:0038) |   | ├─Token(Colon) |:|
//@[039:0079) |   | └─ArraySyntax
//@[039:0040) |   |   ├─Token(LeftSquare) |[|
//@[040:0053) |   |   ├─ArrayItemSyntax
//@[040:0053) |   |   | └─StringSyntax
//@[040:0053) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[053:0054) |   |   ├─Token(Comma) |,|
//@[055:0078) |   |   ├─ArrayItemSyntax
//@[055:0078) |   |   | └─StringSyntax
//@[055:0078) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxxxxx'|
//@[078:0079) |   |   └─Token(RightSquare) |]|
//@[080:0081) |   └─Token(RightBrace) |}|
//@[081:0082) ├─Token(NewLine) |\n|
var w82_ = { foo: 123, bar: true, baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx'] }
//@[000:0082) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |w82_|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0082) | └─ObjectSyntax
//@[011:0012) |   ├─Token(LeftBrace) |{|
//@[013:0021) |   ├─ObjectPropertySyntax
//@[013:0016) |   | ├─IdentifierSyntax
//@[013:0016) |   | | └─Token(Identifier) |foo|
//@[016:0017) |   | ├─Token(Colon) |:|
//@[018:0021) |   | └─IntegerLiteralSyntax
//@[018:0021) |   |   └─Token(Integer) |123|
//@[021:0022) |   ├─Token(Comma) |,|
//@[023:0032) |   ├─ObjectPropertySyntax
//@[023:0026) |   | ├─IdentifierSyntax
//@[023:0026) |   | | └─Token(Identifier) |bar|
//@[026:0027) |   | ├─Token(Colon) |:|
//@[028:0032) |   | └─BooleanLiteralSyntax
//@[028:0032) |   |   └─Token(TrueKeyword) |true|
//@[032:0033) |   ├─Token(Comma) |,|
//@[034:0080) |   ├─ObjectPropertySyntax
//@[034:0037) |   | ├─IdentifierSyntax
//@[034:0037) |   | | └─Token(Identifier) |baz|
//@[037:0038) |   | ├─Token(Colon) |:|
//@[039:0080) |   | └─ArraySyntax
//@[039:0040) |   |   ├─Token(LeftSquare) |[|
//@[040:0058) |   |   ├─ArrayItemSyntax
//@[040:0058) |   |   | └─StringSyntax
//@[040:0058) |   |   |   └─Token(StringComplete) |'array length: 41'|
//@[058:0059) |   |   ├─Token(Comma) |,|
//@[060:0079) |   |   ├─ArrayItemSyntax
//@[060:0079) |   |   | └─StringSyntax
//@[060:0079) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxx'|
//@[079:0080) |   |   └─Token(RightSquare) |]|
//@[081:0082) |   └─Token(RightBrace) |}|
//@[082:0084) ├─Token(NewLine) |\n\n|

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx'})
//@[000:0078) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |w78__|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0078) | └─FunctionCallSyntax
//@[012:0017) |   ├─IdentifierSyntax
//@[012:0017) |   | └─Token(Identifier) |union|
//@[017:0018) |   ├─Token(LeftParen) |(|
//@[018:0034) |   ├─FunctionArgumentSyntax
//@[018:0034) |   | └─ObjectSyntax
//@[018:0019) |   |   ├─Token(LeftBrace) |{|
//@[020:0032) |   |   ├─ObjectPropertySyntax
//@[020:0023) |   |   | ├─IdentifierSyntax
//@[020:0023) |   |   | | └─Token(Identifier) |foo|
//@[023:0024) |   |   | ├─Token(Colon) |:|
//@[025:0032) |   |   | └─StringSyntax
//@[025:0032) |   |   |   └─Token(StringComplete) |'xxxxx'|
//@[033:0034) |   |   └─Token(RightBrace) |}|
//@[034:0035) |   ├─Token(Comma) |,|
//@[036:0056) |   ├─FunctionArgumentSyntax
//@[036:0056) |   | └─ObjectSyntax
//@[036:0037) |   |   ├─Token(LeftBrace) |{|
//@[038:0054) |   |   ├─ObjectPropertySyntax
//@[038:0041) |   |   | ├─IdentifierSyntax
//@[038:0041) |   |   | | └─Token(Identifier) |bar|
//@[041:0042) |   |   | ├─Token(Colon) |:|
//@[043:0054) |   |   | └─StringSyntax
//@[043:0054) |   |   |   └─Token(StringComplete) |'xxxxxxxxx'|
//@[055:0056) |   |   └─Token(RightBrace) |}|
//@[056:0057) |   ├─Token(Comma) |,|
//@[058:0077) |   ├─FunctionArgumentSyntax
//@[058:0077) |   | └─ObjectSyntax
//@[058:0059) |   |   ├─Token(LeftBrace) |{|
//@[060:0076) |   |   ├─ObjectPropertySyntax
//@[060:0063) |   |   | ├─IdentifierSyntax
//@[060:0063) |   |   | | └─Token(Identifier) |baz|
//@[063:0064) |   |   | ├─Token(Colon) |:|
//@[065:0076) |   |   | └─StringSyntax
//@[065:0076) |   |   |   └─Token(StringComplete) |'xxxxxxxxx'|
//@[076:0077) |   |   └─Token(RightBrace) |}|
//@[077:0078) |   └─Token(RightParen) |)|
//@[078:0079) ├─Token(NewLine) |\n|
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' },
//@[000:0083) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |w79__|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0083) | └─FunctionCallSyntax
//@[012:0017) |   ├─IdentifierSyntax
//@[012:0017) |   | └─Token(Identifier) |union|
//@[017:0018) |   ├─Token(LeftParen) |(|
//@[018:0034) |   ├─FunctionArgumentSyntax
//@[018:0034) |   | └─ObjectSyntax
//@[018:0019) |   |   ├─Token(LeftBrace) |{|
//@[020:0032) |   |   ├─ObjectPropertySyntax
//@[020:0023) |   |   | ├─IdentifierSyntax
//@[020:0023) |   |   | | └─Token(Identifier) |foo|
//@[023:0024) |   |   | ├─Token(Colon) |:|
//@[025:0032) |   |   | └─StringSyntax
//@[025:0032) |   |   |   └─Token(StringComplete) |'xxxxx'|
//@[033:0034) |   |   └─Token(RightBrace) |}|
//@[034:0035) |   ├─Token(Comma) |,|
//@[036:0056) |   ├─FunctionArgumentSyntax
//@[036:0056) |   | └─ObjectSyntax
//@[036:0037) |   |   ├─Token(LeftBrace) |{|
//@[038:0054) |   |   ├─ObjectPropertySyntax
//@[038:0041) |   |   | ├─IdentifierSyntax
//@[038:0041) |   |   | | └─Token(Identifier) |bar|
//@[041:0042) |   |   | ├─Token(Colon) |:|
//@[043:0054) |   |   | └─StringSyntax
//@[043:0054) |   |   |   └─Token(StringComplete) |'xxxxxxxxx'|
//@[055:0056) |   |   └─Token(RightBrace) |}|
//@[056:0057) |   ├─Token(Comma) |,|
//@[057:0058) |   ├─Token(NewLine) |\n|
    { baz: 'xxxxxxxxxx'}) // suffix
//@[004:0024) |   ├─FunctionArgumentSyntax
//@[004:0024) |   | └─ObjectSyntax
//@[004:0005) |   |   ├─Token(LeftBrace) |{|
//@[006:0023) |   |   ├─ObjectPropertySyntax
//@[006:0009) |   |   | ├─IdentifierSyntax
//@[006:0009) |   |   | | └─Token(Identifier) |baz|
//@[009:0010) |   |   | ├─Token(Colon) |:|
//@[011:0023) |   |   | └─StringSyntax
//@[011:0023) |   |   |   └─Token(StringComplete) |'xxxxxxxxxx'|
//@[023:0024) |   |   └─Token(RightBrace) |}|
//@[024:0025) |   └─Token(RightParen) |)|
//@[035:0036) ├─Token(NewLine) |\n|
var w80__ = union(
//@[000:0093) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |w80__|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0093) | └─FunctionCallSyntax
//@[012:0017) |   ├─IdentifierSyntax
//@[012:0017) |   | └─Token(Identifier) |union|
//@[017:0018) |   ├─Token(LeftParen) |(|
//@[018:0019) |   ├─Token(NewLine) |\n|
    { foo: 'xxxxxx' },
//@[004:0021) |   ├─FunctionArgumentSyntax
//@[004:0021) |   | └─ObjectSyntax
//@[004:0005) |   |   ├─Token(LeftBrace) |{|
//@[006:0019) |   |   ├─ObjectPropertySyntax
//@[006:0009) |   |   | ├─IdentifierSyntax
//@[006:0009) |   |   | | └─Token(Identifier) |foo|
//@[009:0010) |   |   | ├─Token(Colon) |:|
//@[011:0019) |   |   | └─StringSyntax
//@[011:0019) |   |   |   └─Token(StringComplete) |'xxxxxx'|
//@[020:0021) |   |   └─Token(RightBrace) |}|
//@[021:0022) |   ├─Token(Comma) |,|
//@[022:0023) |   ├─Token(NewLine) |\n|
    { bar: 'xxxxxx' },
//@[004:0021) |   ├─FunctionArgumentSyntax
//@[004:0021) |   | └─ObjectSyntax
//@[004:0005) |   |   ├─Token(LeftBrace) |{|
//@[006:0019) |   |   ├─ObjectPropertySyntax
//@[006:0009) |   |   | ├─IdentifierSyntax
//@[006:0009) |   |   | | └─Token(Identifier) |bar|
//@[009:0010) |   |   | ├─Token(Colon) |:|
//@[011:0019) |   |   | └─StringSyntax
//@[011:0019) |   |   |   └─Token(StringComplete) |'xxxxxx'|
//@[020:0021) |   |   └─Token(RightBrace) |}|
//@[021:0022) |   ├─Token(Comma) |,|
//@[022:0023) |   ├─Token(NewLine) |\n|
    { baz: 'xxxxxxxxxxxxx'})
//@[004:0027) |   ├─FunctionArgumentSyntax
//@[004:0027) |   | └─ObjectSyntax
//@[004:0005) |   |   ├─Token(LeftBrace) |{|
//@[006:0026) |   |   ├─ObjectPropertySyntax
//@[006:0009) |   |   | ├─IdentifierSyntax
//@[006:0009) |   |   | | └─Token(Identifier) |baz|
//@[009:0010) |   |   | ├─Token(Colon) |:|
//@[011:0026) |   |   | └─StringSyntax
//@[011:0026) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxx'|
//@[026:0027) |   |   └─Token(RightBrace) |}|
//@[027:0028) |   └─Token(RightParen) |)|
//@[028:0029) ├─Token(NewLine) |\n|
var w81__ = union({ foo: 'x' } /* xxx */, any({ baz: 'func call length: 38  ' }))
//@[000:0081) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |w81__|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0081) | └─FunctionCallSyntax
//@[012:0017) |   ├─IdentifierSyntax
//@[012:0017) |   | └─Token(Identifier) |union|
//@[017:0018) |   ├─Token(LeftParen) |(|
//@[018:0030) |   ├─FunctionArgumentSyntax
//@[018:0030) |   | └─ObjectSyntax
//@[018:0019) |   |   ├─Token(LeftBrace) |{|
//@[020:0028) |   |   ├─ObjectPropertySyntax
//@[020:0023) |   |   | ├─IdentifierSyntax
//@[020:0023) |   |   | | └─Token(Identifier) |foo|
//@[023:0024) |   |   | ├─Token(Colon) |:|
//@[025:0028) |   |   | └─StringSyntax
//@[025:0028) |   |   |   └─Token(StringComplete) |'x'|
//@[029:0030) |   |   └─Token(RightBrace) |}|
//@[040:0041) |   ├─Token(Comma) |,|
//@[042:0080) |   ├─FunctionArgumentSyntax
//@[042:0080) |   | └─FunctionCallSyntax
//@[042:0045) |   |   ├─IdentifierSyntax
//@[042:0045) |   |   | └─Token(Identifier) |any|
//@[045:0046) |   |   ├─Token(LeftParen) |(|
//@[046:0079) |   |   ├─FunctionArgumentSyntax
//@[046:0079) |   |   | └─ObjectSyntax
//@[046:0047) |   |   |   ├─Token(LeftBrace) |{|
//@[048:0077) |   |   |   ├─ObjectPropertySyntax
//@[048:0051) |   |   |   | ├─IdentifierSyntax
//@[048:0051) |   |   |   | | └─Token(Identifier) |baz|
//@[051:0052) |   |   |   | ├─Token(Colon) |:|
//@[053:0077) |   |   |   | └─StringSyntax
//@[053:0077) |   |   |   |   └─Token(StringComplete) |'func call length: 38  '|
//@[078:0079) |   |   |   └─Token(RightBrace) |}|
//@[079:0080) |   |   └─Token(RightParen) |)|
//@[080:0081) |   └─Token(RightParen) |)|
//@[081:0082) ├─Token(NewLine) |\n|
var w82__ = union({ foo: 'x', bar: 'x' }, any({ baz: 'func call length: 39   ' }))
//@[000:0082) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |w82__|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0082) | └─FunctionCallSyntax
//@[012:0017) |   ├─IdentifierSyntax
//@[012:0017) |   | └─Token(Identifier) |union|
//@[017:0018) |   ├─Token(LeftParen) |(|
//@[018:0040) |   ├─FunctionArgumentSyntax
//@[018:0040) |   | └─ObjectSyntax
//@[018:0019) |   |   ├─Token(LeftBrace) |{|
//@[020:0028) |   |   ├─ObjectPropertySyntax
//@[020:0023) |   |   | ├─IdentifierSyntax
//@[020:0023) |   |   | | └─Token(Identifier) |foo|
//@[023:0024) |   |   | ├─Token(Colon) |:|
//@[025:0028) |   |   | └─StringSyntax
//@[025:0028) |   |   |   └─Token(StringComplete) |'x'|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0038) |   |   ├─ObjectPropertySyntax
//@[030:0033) |   |   | ├─IdentifierSyntax
//@[030:0033) |   |   | | └─Token(Identifier) |bar|
//@[033:0034) |   |   | ├─Token(Colon) |:|
//@[035:0038) |   |   | └─StringSyntax
//@[035:0038) |   |   |   └─Token(StringComplete) |'x'|
//@[039:0040) |   |   └─Token(RightBrace) |}|
//@[040:0041) |   ├─Token(Comma) |,|
//@[042:0081) |   ├─FunctionArgumentSyntax
//@[042:0081) |   | └─FunctionCallSyntax
//@[042:0045) |   |   ├─IdentifierSyntax
//@[042:0045) |   |   | └─Token(Identifier) |any|
//@[045:0046) |   |   ├─Token(LeftParen) |(|
//@[046:0080) |   |   ├─FunctionArgumentSyntax
//@[046:0080) |   |   | └─ObjectSyntax
//@[046:0047) |   |   |   ├─Token(LeftBrace) |{|
//@[048:0078) |   |   |   ├─ObjectPropertySyntax
//@[048:0051) |   |   |   | ├─IdentifierSyntax
//@[048:0051) |   |   |   | | └─Token(Identifier) |baz|
//@[051:0052) |   |   |   | ├─Token(Colon) |:|
//@[053:0078) |   |   |   | └─StringSyntax
//@[053:0078) |   |   |   |   └─Token(StringComplete) |'func call length: 39   '|
//@[079:0080) |   |   |   └─Token(RightBrace) |}|
//@[080:0081) |   |   └─Token(RightParen) |)|
//@[081:0082) |   └─Token(RightParen) |)|
//@[082:0084) ├─Token(NewLine) |\n\n|

////////////////////////////////////////////////////////////////////////////////
//@[080:0081) ├─Token(NewLine) |\n|
////////////////////////// Baselines for line breakers /////////////////////////
//@[080:0081) ├─Token(NewLine) |\n|
////////////////////////////////////////////////////////////////////////////////
//@[080:0081) ├─Token(NewLine) |\n|
var forceBreak1 = {
//@[000:0035) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |forceBreak1|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0035) | └─ObjectSyntax
//@[018:0019) |   ├─Token(LeftBrace) |{|
//@[019:0020) |   ├─Token(NewLine) |\n|
    foo: true
//@[004:0013) |   ├─ObjectPropertySyntax
//@[004:0007) |   | ├─IdentifierSyntax
//@[004:0007) |   | | └─Token(Identifier) |foo|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0013) |   | └─BooleanLiteralSyntax
//@[009:0013) |   |   └─Token(TrueKeyword) |true|
//@[013:0014) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
/* should print a newline after this */var forceBreak2 = {
//@[039:0086) ├─VariableDeclarationSyntax
//@[039:0042) | ├─Token(Identifier) |var|
//@[043:0054) | ├─IdentifierSyntax
//@[043:0054) | | └─Token(Identifier) |forceBreak2|
//@[055:0056) | ├─Token(Assignment) |=|
//@[057:0086) | └─ObjectSyntax
//@[057:0058) |   ├─Token(LeftBrace) |{|
//@[058:0059) |   ├─Token(NewLine) |\n|
    foo: true, bar: false
//@[004:0013) |   ├─ObjectPropertySyntax
//@[004:0007) |   | ├─IdentifierSyntax
//@[004:0007) |   | | └─Token(Identifier) |foo|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0013) |   | └─BooleanLiteralSyntax
//@[009:0013) |   |   └─Token(TrueKeyword) |true|
//@[013:0014) |   ├─Token(Comma) |,|
//@[015:0025) |   ├─ObjectPropertySyntax
//@[015:0018) |   | ├─IdentifierSyntax
//@[015:0018) |   | | └─Token(Identifier) |bar|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0025) |   | └─BooleanLiteralSyntax
//@[020:0025) |   |   └─Token(FalseKeyword) |false|
//@[025:0026) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
var forceBreak3 = [1, 2, {
//@[000:0049) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |forceBreak3|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0049) | └─ArraySyntax
//@[018:0019) |   ├─Token(LeftSquare) |[|
//@[019:0020) |   ├─ArrayItemSyntax
//@[019:0020) |   | └─IntegerLiteralSyntax
//@[019:0020) |   |   └─Token(Integer) |1|
//@[020:0021) |   ├─Token(Comma) |,|
//@[022:0023) |   ├─ArrayItemSyntax
//@[022:0023) |   | └─IntegerLiteralSyntax
//@[022:0023) |   |   └─Token(Integer) |2|
//@[023:0024) |   ├─Token(Comma) |,|
//@[025:0042) |   ├─ArrayItemSyntax
//@[025:0042) |   | └─ObjectSyntax
//@[025:0026) |   |   ├─Token(LeftBrace) |{|
//@[026:0027) |   |   ├─Token(NewLine) |\n|
    foo: true }, 3, 4]
//@[004:0013) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |foo|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0013) |   |   | └─BooleanLiteralSyntax
//@[009:0013) |   |   |   └─Token(TrueKeyword) |true|
//@[014:0015) |   |   └─Token(RightBrace) |}|
//@[015:0016) |   ├─Token(Comma) |,|
//@[017:0018) |   ├─ArrayItemSyntax
//@[017:0018) |   | └─IntegerLiteralSyntax
//@[017:0018) |   |   └─Token(Integer) |3|
//@[018:0019) |   ├─Token(Comma) |,|
//@[020:0021) |   ├─ArrayItemSyntax
//@[020:0021) |   | └─IntegerLiteralSyntax
//@[020:0021) |   |   └─Token(Integer) |4|
//@[021:0022) |   └─Token(RightSquare) |]|
//@[022:0023) ├─Token(NewLine) |\n|
var forceBreak4 = { foo: true, bar: false // force break
//@[000:0058) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |forceBreak4|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0058) | └─ObjectSyntax
//@[018:0019) |   ├─Token(LeftBrace) |{|
//@[020:0029) |   ├─ObjectPropertySyntax
//@[020:0023) |   | ├─IdentifierSyntax
//@[020:0023) |   | | └─Token(Identifier) |foo|
//@[023:0024) |   | ├─Token(Colon) |:|
//@[025:0029) |   | └─BooleanLiteralSyntax
//@[025:0029) |   |   └─Token(TrueKeyword) |true|
//@[029:0030) |   ├─Token(Comma) |,|
//@[031:0041) |   ├─ObjectPropertySyntax
//@[031:0034) |   | ├─IdentifierSyntax
//@[031:0034) |   | | └─Token(Identifier) |bar|
//@[034:0035) |   | ├─Token(Colon) |:|
//@[036:0041) |   | └─BooleanLiteralSyntax
//@[036:0041) |   |   └─Token(FalseKeyword) |false|
//@[056:0057) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
var forceBreak5 = { foo: true
//@[000:0048) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |forceBreak5|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0048) | └─ObjectSyntax
//@[018:0019) |   ├─Token(LeftBrace) |{|
//@[020:0029) |   ├─ObjectPropertySyntax
//@[020:0023) |   | ├─IdentifierSyntax
//@[020:0023) |   | | └─Token(Identifier) |foo|
//@[023:0024) |   | ├─Token(Colon) |:|
//@[025:0029) |   | └─BooleanLiteralSyntax
//@[025:0029) |   |   └─Token(TrueKeyword) |true|
//@[029:0030) |   ├─Token(NewLine) |\n|
/* force break */}
//@[017:0018) |   └─Token(RightBrace) |}|
//@[018:0019) ├─Token(NewLine) |\n|
var forceBreak6 = { foo: true
//@[000:0076) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |forceBreak6|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0076) | └─ObjectSyntax
//@[018:0019) |   ├─Token(LeftBrace) |{|
//@[020:0029) |   ├─ObjectPropertySyntax
//@[020:0023) |   | ├─IdentifierSyntax
//@[020:0023) |   | | └─Token(Identifier) |foo|
//@[023:0024) |   | ├─Token(Colon) |:|
//@[025:0029) |   | └─BooleanLiteralSyntax
//@[025:0029) |   |   └─Token(TrueKeyword) |true|
//@[029:0030) |   ├─Token(NewLine) |\n|
    bar: false
//@[004:0014) |   ├─ObjectPropertySyntax
//@[004:0007) |   | ├─IdentifierSyntax
//@[004:0007) |   | | └─Token(Identifier) |bar|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0014) |   | └─BooleanLiteralSyntax
//@[009:0014) |   |   └─Token(FalseKeyword) |false|
//@[014:0015) |   ├─Token(NewLine) |\n|
    baz: 123
//@[004:0012) |   ├─ObjectPropertySyntax
//@[004:0007) |   | ├─IdentifierSyntax
//@[004:0007) |   | | └─Token(Identifier) |baz|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0012) |   | └─IntegerLiteralSyntax
//@[009:0012) |   |   └─Token(Integer) |123|
//@[012:0013) |   ├─Token(NewLine) |\n|
/* force break */}
//@[017:0018) |   └─Token(RightBrace) |}|
//@[018:0019) ├─Token(NewLine) |\n|
var forceBreak7 = [1, 2 // force break
//@[000:0040) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |forceBreak7|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0040) | └─ArraySyntax
//@[018:0019) |   ├─Token(LeftSquare) |[|
//@[019:0020) |   ├─ArrayItemSyntax
//@[019:0020) |   | └─IntegerLiteralSyntax
//@[019:0020) |   |   └─Token(Integer) |1|
//@[020:0021) |   ├─Token(Comma) |,|
//@[022:0023) |   ├─ArrayItemSyntax
//@[022:0023) |   | └─IntegerLiteralSyntax
//@[022:0023) |   |   └─Token(Integer) |2|
//@[038:0039) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0002) ├─Token(NewLine) |\n|
var forceBreak8 = [1, 2
//@[000:0047) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |forceBreak8|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0047) | └─ArraySyntax
//@[018:0019) |   ├─Token(LeftSquare) |[|
//@[019:0020) |   ├─ArrayItemSyntax
//@[019:0020) |   | └─IntegerLiteralSyntax
//@[019:0020) |   |   └─Token(Integer) |1|
//@[020:0021) |   ├─Token(Comma) |,|
//@[022:0023) |   ├─ArrayItemSyntax
//@[022:0023) |   | └─IntegerLiteralSyntax
//@[022:0023) |   |   └─Token(Integer) |2|
//@[023:0024) |   ├─Token(NewLine) |\n|
    /* force break */ ]
//@[022:0023) |   └─Token(RightSquare) |]|
//@[023:0024) ├─Token(NewLine) |\n|
var forceBreak9 = [1, 2, {
//@[000:0058) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |forceBreak9|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0058) | └─ArraySyntax
//@[018:0019) |   ├─Token(LeftSquare) |[|
//@[019:0020) |   ├─ArrayItemSyntax
//@[019:0020) |   | └─IntegerLiteralSyntax
//@[019:0020) |   |   └─Token(Integer) |1|
//@[020:0021) |   ├─Token(Comma) |,|
//@[022:0023) |   ├─ArrayItemSyntax
//@[022:0023) |   | └─IntegerLiteralSyntax
//@[022:0023) |   |   └─Token(Integer) |2|
//@[023:0024) |   ├─Token(Comma) |,|
//@[025:0057) |   ├─ArrayItemSyntax
//@[025:0057) |   | └─ObjectSyntax
//@[025:0026) |   |   ├─Token(LeftBrace) |{|
//@[026:0027) |   |   ├─Token(NewLine) |\n|
    foo: true
//@[004:0013) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |foo|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0013) |   |   | └─BooleanLiteralSyntax
//@[009:0013) |   |   |   └─Token(TrueKeyword) |true|
//@[013:0014) |   |   ├─Token(NewLine) |\n|
    bar: false
//@[004:0014) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |bar|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0014) |   |   | └─BooleanLiteralSyntax
//@[009:0014) |   |   |   └─Token(FalseKeyword) |false|
//@[014:0015) |   |   ├─Token(NewLine) |\n|
}]
//@[000:0001) |   |   └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0003) ├─Token(NewLine) |\n|
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@[000:0082) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |forceBreak10|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0082) | └─ArraySyntax
//@[019:0020) |   ├─Token(LeftSquare) |[|
//@[020:0021) |   ├─ArrayItemSyntax
//@[020:0021) |   | └─IntegerLiteralSyntax
//@[020:0021) |   |   └─Token(Integer) |1|
//@[021:0022) |   ├─Token(Comma) |,|
//@[023:0024) |   ├─ArrayItemSyntax
//@[023:0024) |   | └─IntegerLiteralSyntax
//@[023:0024) |   |   └─Token(Integer) |2|
//@[024:0025) |   ├─Token(Comma) |,|
//@[026:0081) |   ├─ArrayItemSyntax
//@[026:0081) |   | └─FunctionCallSyntax
//@[026:0038) |   |   ├─IdentifierSyntax
//@[026:0038) |   |   | └─Token(Identifier) |intersection|
//@[038:0039) |   |   ├─Token(LeftParen) |(|
//@[039:0064) |   |   ├─FunctionArgumentSyntax
//@[039:0064) |   |   | └─ObjectSyntax
//@[039:0040) |   |   |   ├─Token(LeftBrace) |{|
//@[041:0050) |   |   |   ├─ObjectPropertySyntax
//@[041:0044) |   |   |   | ├─IdentifierSyntax
//@[041:0044) |   |   |   | | └─Token(Identifier) |foo|
//@[044:0045) |   |   |   | ├─Token(Colon) |:|
//@[046:0050) |   |   |   | └─BooleanLiteralSyntax
//@[046:0050) |   |   |   |   └─Token(TrueKeyword) |true|
//@[050:0051) |   |   |   ├─Token(Comma) |,|
//@[052:0062) |   |   |   ├─ObjectPropertySyntax
//@[052:0055) |   |   |   | ├─IdentifierSyntax
//@[052:0055) |   |   |   | | └─Token(Identifier) |bar|
//@[055:0056) |   |   |   | ├─Token(Colon) |:|
//@[057:0062) |   |   |   | └─BooleanLiteralSyntax
//@[057:0062) |   |   |   |   └─Token(FalseKeyword) |false|
//@[063:0064) |   |   |   └─Token(RightBrace) |}|
//@[064:0065) |   |   ├─Token(Comma) |,|
//@[066:0080) |   |   ├─FunctionArgumentSyntax
//@[066:0080) |   |   | └─ObjectSyntax
//@[066:0067) |   |   |   ├─Token(LeftBrace) |{|
//@[067:0068) |   |   |   ├─Token(NewLine) |\n|
  foo: true})]
//@[002:0011) |   |   |   ├─ObjectPropertySyntax
//@[002:0005) |   |   |   | ├─IdentifierSyntax
//@[002:0005) |   |   |   | | └─Token(Identifier) |foo|
//@[005:0006) |   |   |   | ├─Token(Colon) |:|
//@[007:0011) |   |   |   | └─BooleanLiteralSyntax
//@[007:0011) |   |   |   |   └─Token(TrueKeyword) |true|
//@[011:0012) |   |   |   └─Token(RightBrace) |}|
//@[012:0013) |   |   └─Token(RightParen) |)|
//@[013:0014) |   └─Token(RightSquare) |]|
//@[014:0015) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
