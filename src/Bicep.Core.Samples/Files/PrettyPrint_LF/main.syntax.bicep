////////////////////////////////////////////////////////////////////////////////
//@[00:4175) ProgramSyntax
//@[80:0081) ├─Token(NewLine) |\n|
//////////////////////////// Baselines for width 40 ////////////////////////////
//@[80:0081) ├─Token(NewLine) |\n|
////////////////////////////////////////////////////////////////////////////////
//@[80:0081) ├─Token(NewLine) |\n|
var w38 = [true, /* xxxxx */ true, 12]      // suffix
//@[00:0038) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w38|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0038) | └─ArraySyntax
//@[10:0011) |   ├─Token(LeftSquare) |[|
//@[11:0015) |   ├─ArrayItemSyntax
//@[11:0015) |   | └─BooleanLiteralSyntax
//@[11:0015) |   |   └─Token(TrueKeyword) |true|
//@[15:0016) |   ├─Token(Comma) |,|
//@[29:0033) |   ├─ArrayItemSyntax
//@[29:0033) |   | └─BooleanLiteralSyntax
//@[29:0033) |   |   └─Token(TrueKeyword) |true|
//@[33:0034) |   ├─Token(Comma) |,|
//@[35:0037) |   ├─ArrayItemSyntax
//@[35:0037) |   | └─IntegerLiteralSyntax
//@[35:0037) |   |   └─Token(Integer) |12|
//@[37:0038) |   └─Token(RightSquare) |]|
//@[53:0054) ├─Token(NewLine) |\n|
var w39 = [true, true
//@[00:0042) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w39|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0042) | └─ArraySyntax
//@[10:0011) |   ├─Token(LeftSquare) |[|
//@[11:0015) |   ├─ArrayItemSyntax
//@[11:0015) |   | └─BooleanLiteralSyntax
//@[11:0015) |   |   └─Token(TrueKeyword) |true|
//@[15:0016) |   ├─Token(Comma) |,|
//@[17:0021) |   ├─ArrayItemSyntax
//@[17:0021) |   | └─BooleanLiteralSyntax
//@[17:0021) |   |   └─Token(TrueKeyword) |true|
//@[21:0022) |   ├─Token(NewLine) |\n|
    true, true, 123]
//@[04:0008) |   ├─ArrayItemSyntax
//@[04:0008) |   | └─BooleanLiteralSyntax
//@[04:0008) |   |   └─Token(TrueKeyword) |true|
//@[08:0009) |   ├─Token(Comma) |,|
//@[10:0014) |   ├─ArrayItemSyntax
//@[10:0014) |   | └─BooleanLiteralSyntax
//@[10:0014) |   |   └─Token(TrueKeyword) |true|
//@[14:0015) |   ├─Token(Comma) |,|
//@[16:0019) |   ├─ArrayItemSyntax
//@[16:0019) |   | └─IntegerLiteralSyntax
//@[16:0019) |   |   └─Token(Integer) |123|
//@[19:0020) |   └─Token(RightSquare) |]|
//@[20:0021) ├─Token(NewLine) |\n|
var w40 =[
//@[00:0043) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w40|
//@[08:0009) | ├─Token(Assignment) |=|
//@[09:0043) | └─ArraySyntax
//@[09:0010) |   ├─Token(LeftSquare) |[|
//@[10:0011) |   ├─Token(NewLine) |\n|
    true, true, 1234/* xxxxx */]  // suffix
//@[04:0008) |   ├─ArrayItemSyntax
//@[04:0008) |   | └─BooleanLiteralSyntax
//@[04:0008) |   |   └─Token(TrueKeyword) |true|
//@[08:0009) |   ├─Token(Comma) |,|
//@[10:0014) |   ├─ArrayItemSyntax
//@[10:0014) |   | └─BooleanLiteralSyntax
//@[10:0014) |   |   └─Token(TrueKeyword) |true|
//@[14:0015) |   ├─Token(Comma) |,|
//@[16:0020) |   ├─ArrayItemSyntax
//@[16:0020) |   | └─IntegerLiteralSyntax
//@[16:0020) |   |   └─Token(Integer) |1234|
//@[31:0032) |   └─Token(RightSquare) |]|
//@[43:0044) ├─Token(NewLine) |\n|
var w41 =[ true, true, true, true, 12345 ]
//@[00:0042) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w41|
//@[08:0009) | ├─Token(Assignment) |=|
//@[09:0042) | └─ArraySyntax
//@[09:0010) |   ├─Token(LeftSquare) |[|
//@[11:0015) |   ├─ArrayItemSyntax
//@[11:0015) |   | └─BooleanLiteralSyntax
//@[11:0015) |   |   └─Token(TrueKeyword) |true|
//@[15:0016) |   ├─Token(Comma) |,|
//@[17:0021) |   ├─ArrayItemSyntax
//@[17:0021) |   | └─BooleanLiteralSyntax
//@[17:0021) |   |   └─Token(TrueKeyword) |true|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0027) |   ├─ArrayItemSyntax
//@[23:0027) |   | └─BooleanLiteralSyntax
//@[23:0027) |   |   └─Token(TrueKeyword) |true|
//@[27:0028) |   ├─Token(Comma) |,|
//@[29:0033) |   ├─ArrayItemSyntax
//@[29:0033) |   | └─BooleanLiteralSyntax
//@[29:0033) |   |   └─Token(TrueKeyword) |true|
//@[33:0034) |   ├─Token(Comma) |,|
//@[35:0040) |   ├─ArrayItemSyntax
//@[35:0040) |   | └─IntegerLiteralSyntax
//@[35:0040) |   |   └─Token(Integer) |12345|
//@[41:0042) |   └─Token(RightSquare) |]|
//@[42:0043) ├─Token(NewLine) |\n|
var w42 =[true, /* xxx */ 12 /* xx */, 1]
//@[00:0041) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w42|
//@[08:0009) | ├─Token(Assignment) |=|
//@[09:0041) | └─ArraySyntax
//@[09:0010) |   ├─Token(LeftSquare) |[|
//@[10:0014) |   ├─ArrayItemSyntax
//@[10:0014) |   | └─BooleanLiteralSyntax
//@[10:0014) |   |   └─Token(TrueKeyword) |true|
//@[14:0015) |   ├─Token(Comma) |,|
//@[26:0028) |   ├─ArrayItemSyntax
//@[26:0028) |   | └─IntegerLiteralSyntax
//@[26:0028) |   |   └─Token(Integer) |12|
//@[37:0038) |   ├─Token(Comma) |,|
//@[39:0040) |   ├─ArrayItemSyntax
//@[39:0040) |   | └─IntegerLiteralSyntax
//@[39:0040) |   |   └─Token(Integer) |1|
//@[40:0041) |   └─Token(RightSquare) |]|
//@[41:0043) ├─Token(NewLine) |\n\n|

var w38_= { foo: true, bar: 1234567
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |w38_|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0037) | └─ObjectSyntax
//@[10:0011) |   ├─Token(LeftBrace) |{|
//@[12:0021) |   ├─ObjectPropertySyntax
//@[12:0015) |   | ├─IdentifierSyntax
//@[12:0015) |   | | └─Token(Identifier) |foo|
//@[15:0016) |   | ├─Token(Colon) |:|
//@[17:0021) |   | └─BooleanLiteralSyntax
//@[17:0021) |   |   └─Token(TrueKeyword) |true|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0035) |   ├─ObjectPropertySyntax
//@[23:0026) |   | ├─IdentifierSyntax
//@[23:0026) |   | | └─Token(Identifier) |bar|
//@[26:0027) |   | ├─Token(Colon) |:|
//@[28:0035) |   | └─IntegerLiteralSyntax
//@[28:0035) |   |   └─Token(Integer) |1234567|
//@[35:0036) |   ├─Token(NewLine) |\n|
} // suffix
//@[00:0001) |   └─Token(RightBrace) |}|
//@[11:0012) ├─Token(NewLine) |\n|
var        w39_= { foo: true
//@[00:0046) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[11:0015) | ├─IdentifierSyntax
//@[11:0015) | | └─Token(Identifier) |w39_|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0046) | └─ObjectSyntax
//@[17:0018) |   ├─Token(LeftBrace) |{|
//@[19:0028) |   ├─ObjectPropertySyntax
//@[19:0022) |   | ├─IdentifierSyntax
//@[19:0022) |   | | └─Token(Identifier) |foo|
//@[22:0023) |   | ├─Token(Colon) |:|
//@[24:0028) |   | └─BooleanLiteralSyntax
//@[24:0028) |   |   └─Token(TrueKeyword) |true|
//@[28:0029) |   ├─Token(NewLine) |\n|
  bar: 12345678 } // suffix
//@[02:0015) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |bar|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0015) |   | └─IntegerLiteralSyntax
//@[07:0015) |   |   └─Token(Integer) |12345678|
//@[16:0017) |   └─Token(RightBrace) |}|
//@[27:0028) ├─Token(NewLine) |\n|
    var w40_= { foo: 1, bar:    1 /* xxxx */ }
//@[04:0046) ├─VariableDeclarationSyntax
//@[04:0007) | ├─Token(Identifier) |var|
//@[08:0012) | ├─IdentifierSyntax
//@[08:0012) | | └─Token(Identifier) |w40_|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0046) | └─ObjectSyntax
//@[14:0015) |   ├─Token(LeftBrace) |{|
//@[16:0022) |   ├─ObjectPropertySyntax
//@[16:0019) |   | ├─IdentifierSyntax
//@[16:0019) |   | | └─Token(Identifier) |foo|
//@[19:0020) |   | ├─Token(Colon) |:|
//@[21:0022) |   | └─IntegerLiteralSyntax
//@[21:0022) |   |   └─Token(Integer) |1|
//@[22:0023) |   ├─Token(Comma) |,|
//@[24:0033) |   ├─ObjectPropertySyntax
//@[24:0027) |   | ├─IdentifierSyntax
//@[24:0027) |   | | └─Token(Identifier) |bar|
//@[27:0028) |   | ├─Token(Colon) |:|
//@[32:0033) |   | └─IntegerLiteralSyntax
//@[32:0033) |   |   └─Token(Integer) |1|
//@[45:0046) |   └─Token(RightBrace) |}|
//@[46:0047) ├─Token(NewLine) |\n|
var w41_={ foo: true, bar    : 1234567890 }
//@[00:0043) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |w41_|
//@[08:0009) | ├─Token(Assignment) |=|
//@[09:0043) | └─ObjectSyntax
//@[09:0010) |   ├─Token(LeftBrace) |{|
//@[11:0020) |   ├─ObjectPropertySyntax
//@[11:0014) |   | ├─IdentifierSyntax
//@[11:0014) |   | | └─Token(Identifier) |foo|
//@[14:0015) |   | ├─Token(Colon) |:|
//@[16:0020) |   | └─BooleanLiteralSyntax
//@[16:0020) |   |   └─Token(TrueKeyword) |true|
//@[20:0021) |   ├─Token(Comma) |,|
//@[22:0041) |   ├─ObjectPropertySyntax
//@[22:0025) |   | ├─IdentifierSyntax
//@[22:0025) |   | | └─Token(Identifier) |bar|
//@[29:0030) |   | ├─Token(Colon) |:|
//@[31:0041) |   | └─IntegerLiteralSyntax
//@[31:0041) |   |   └─Token(Integer) |1234567890|
//@[42:0043) |   └─Token(RightBrace) |}|
//@[43:0044) ├─Token(NewLine) |\n|
var w42_= { foo: true
//@[00:0044) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |w42_|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0044) | └─ObjectSyntax
//@[10:0011) |   ├─Token(LeftBrace) |{|
//@[12:0021) |   ├─ObjectPropertySyntax
//@[12:0015) |   | ├─IdentifierSyntax
//@[12:0015) |   | | └─Token(Identifier) |foo|
//@[15:0016) |   | ├─Token(Colon) |:|
//@[17:0021) |   | └─BooleanLiteralSyntax
//@[17:0021) |   |   └─Token(TrueKeyword) |true|
//@[21:0022) |   ├─Token(NewLine) |\n|
    bar: 12345678901 } // suffix
//@[04:0020) |   ├─ObjectPropertySyntax
//@[04:0007) |   | ├─IdentifierSyntax
//@[04:0007) |   | | └─Token(Identifier) |bar|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0020) |   | └─IntegerLiteralSyntax
//@[09:0020) |   |   └─Token(Integer) |12345678901|
//@[21:0022) |   └─Token(RightBrace) |}|
//@[32:0034) ├─Token(NewLine) |\n\n|

   var w38__ =    concat('xxxxxx', 'xxxxxx')
//@[03:0044) ├─VariableDeclarationSyntax
//@[03:0006) | ├─Token(Identifier) |var|
//@[07:0012) | ├─IdentifierSyntax
//@[07:0012) | | └─Token(Identifier) |w38__|
//@[13:0014) | ├─Token(Assignment) |=|
//@[18:0044) | └─FunctionCallSyntax
//@[18:0024) |   ├─IdentifierSyntax
//@[18:0024) |   | └─Token(Identifier) |concat|
//@[24:0025) |   ├─Token(LeftParen) |(|
//@[25:0033) |   ├─FunctionArgumentSyntax
//@[25:0033) |   | └─StringSyntax
//@[25:0033) |   |   └─Token(StringComplete) |'xxxxxx'|
//@[33:0034) |   ├─Token(Comma) |,|
//@[35:0043) |   ├─FunctionArgumentSyntax
//@[35:0043) |   | └─StringSyntax
//@[35:0043) |   |   └─Token(StringComplete) |'xxxxxx'|
//@[43:0044) |   └─Token(RightParen) |)|
//@[44:0045) ├─Token(NewLine) |\n|
var w39__ = concat('xxxxxx', 'xxxxxxx'
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |w39__|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0040) | └─FunctionCallSyntax
//@[12:0018) |   ├─IdentifierSyntax
//@[12:0018) |   | └─Token(Identifier) |concat|
//@[18:0019) |   ├─Token(LeftParen) |(|
//@[19:0027) |   ├─FunctionArgumentSyntax
//@[19:0027) |   | └─StringSyntax
//@[19:0027) |   |   └─Token(StringComplete) |'xxxxxx'|
//@[27:0028) |   ├─Token(Comma) |,|
//@[29:0038) |   ├─FunctionArgumentSyntax
//@[29:0038) |   | └─StringSyntax
//@[29:0038) |   |   └─Token(StringComplete) |'xxxxxxx'|
//@[38:0039) |   ├─Token(NewLine) |\n|
) // suffix
//@[00:0001) |   └─Token(RightParen) |)|
//@[11:0012) ├─Token(NewLine) |\n|
var w40__ = concat('xxxxxx',
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |w40__|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0040) | └─FunctionCallSyntax
//@[12:0018) |   ├─IdentifierSyntax
//@[12:0018) |   | └─Token(Identifier) |concat|
//@[18:0019) |   ├─Token(LeftParen) |(|
//@[19:0027) |   ├─FunctionArgumentSyntax
//@[19:0027) |   | └─StringSyntax
//@[19:0027) |   |   └─Token(StringComplete) |'xxxxxx'|
//@[27:0028) |   ├─Token(Comma) |,|
//@[28:0029) |   ├─Token(NewLine) |\n|
'xxxxxxxx') // suffix
//@[00:0010) |   ├─FunctionArgumentSyntax
//@[00:0010) |   | └─StringSyntax
//@[00:0010) |   |   └─Token(StringComplete) |'xxxxxxxx'|
//@[10:0011) |   └─Token(RightParen) |)|
//@[21:0023) ├─Token(NewLine) |\n\n|

var        w41__= concat('xxxxx'/* xxxxxxx */)
//@[00:0046) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[11:0016) | ├─IdentifierSyntax
//@[11:0016) | | └─Token(Identifier) |w41__|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0046) | └─FunctionCallSyntax
//@[18:0024) |   ├─IdentifierSyntax
//@[18:0024) |   | └─Token(Identifier) |concat|
//@[24:0025) |   ├─Token(LeftParen) |(|
//@[25:0032) |   ├─FunctionArgumentSyntax
//@[25:0032) |   | └─StringSyntax
//@[25:0032) |   |   └─Token(StringComplete) |'xxxxx'|
//@[45:0046) |   └─Token(RightParen) |)|
//@[46:0047) ├─Token(NewLine) |\n|
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@[00:0042) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |w42__|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0042) | └─FunctionCallSyntax
//@[12:0018) |   ├─IdentifierSyntax
//@[12:0018) |   | └─Token(Identifier) |concat|
//@[18:0019) |   ├─Token(LeftParen) |(|
//@[19:0026) |   ├─FunctionArgumentSyntax
//@[19:0026) |   | └─StringSyntax
//@[19:0026) |   |   └─Token(StringComplete) |'xxxxx'|
//@[26:0027) |   ├─Token(Comma) |,|
//@[28:0041) |   ├─FunctionArgumentSyntax
//@[28:0041) |   | └─StringSyntax
//@[28:0041) |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[41:0042) |   └─Token(RightParen) |)|
//@[42:0044) ├─Token(NewLine) |\n\n|

var w38___ = true? 'xxxxx' : 'xxxxxx'
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w38___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0037) | └─TernaryOperationSyntax
//@[13:0017) |   ├─BooleanLiteralSyntax
//@[13:0017) |   | └─Token(TrueKeyword) |true|
//@[17:0018) |   ├─Token(Question) |?|
//@[19:0026) |   ├─StringSyntax
//@[19:0026) |   | └─Token(StringComplete) |'xxxxx'|
//@[27:0028) |   ├─Token(Colon) |:|
//@[29:0037) |   └─StringSyntax
//@[29:0037) |     └─Token(StringComplete) |'xxxxxx'|
//@[37:0038) ├─Token(NewLine) |\n|
var w39___ = true
//@[00:0039) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w39___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0039) | └─TernaryOperationSyntax
//@[13:0017) |   ├─BooleanLiteralSyntax
//@[13:0017) |   | └─Token(TrueKeyword) |true|
//@[17:0018) |   ├─Token(NewLine) |\n|
? 'xxxxxx' : 'xxxxxx' // suffix
//@[00:0001) |   ├─Token(Question) |?|
//@[02:0010) |   ├─StringSyntax
//@[02:0010) |   | └─Token(StringComplete) |'xxxxxx'|
//@[11:0012) |   ├─Token(Colon) |:|
//@[13:0021) |   └─StringSyntax
//@[13:0021) |     └─Token(StringComplete) |'xxxxxx'|
//@[31:0032) ├─Token(NewLine) |\n|
var w40___ = true ?'xxxxxx' : 'xxxxxxx'
//@[00:0039) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w40___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0039) | └─TernaryOperationSyntax
//@[13:0017) |   ├─BooleanLiteralSyntax
//@[13:0017) |   | └─Token(TrueKeyword) |true|
//@[18:0019) |   ├─Token(Question) |?|
//@[19:0027) |   ├─StringSyntax
//@[19:0027) |   | └─Token(StringComplete) |'xxxxxx'|
//@[28:0029) |   ├─Token(Colon) |:|
//@[30:0039) |   └─StringSyntax
//@[30:0039) |     └─Token(StringComplete) |'xxxxxxx'|
//@[39:0040) ├─Token(NewLine) |\n|
var w41___ = true ? 'xxxxxxx' :         'xxxxxxx'
//@[00:0049) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w41___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0049) | └─TernaryOperationSyntax
//@[13:0017) |   ├─BooleanLiteralSyntax
//@[13:0017) |   | └─Token(TrueKeyword) |true|
//@[18:0019) |   ├─Token(Question) |?|
//@[20:0029) |   ├─StringSyntax
//@[20:0029) |   | └─Token(StringComplete) |'xxxxxxx'|
//@[30:0031) |   ├─Token(Colon) |:|
//@[40:0049) |   └─StringSyntax
//@[40:0049) |     └─Token(StringComplete) |'xxxxxxx'|
//@[49:0050) ├─Token(NewLine) |\n|
var w42___ = true ? 'xxxxxxx':'xxxxxxxx'
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w42___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0040) | └─TernaryOperationSyntax
//@[13:0017) |   ├─BooleanLiteralSyntax
//@[13:0017) |   | └─Token(TrueKeyword) |true|
//@[18:0019) |   ├─Token(Question) |?|
//@[20:0029) |   ├─StringSyntax
//@[20:0029) |   | └─Token(StringComplete) |'xxxxxxx'|
//@[29:0030) |   ├─Token(Colon) |:|
//@[30:0040) |   └─StringSyntax
//@[30:0040) |     └─Token(StringComplete) |'xxxxxxxx'|
//@[40:0042) ├─Token(NewLine) |\n\n|

////////////////////////////////////////////////////////////////////////////////
//@[80:0081) ├─Token(NewLine) |\n|
//////////////////////////// Baselines for width 80 ////////////////////////////
//@[80:0081) ├─Token(NewLine) |\n|
////////////////////////////////////////////////////////////////////////////////
//@[80:0081) ├─Token(NewLine) |\n|
var w78 = [
//@[00:0084) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w78|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0084) | └─ArraySyntax
//@[10:0011) |   ├─Token(LeftSquare) |[|
//@[11:0012) |   ├─Token(NewLine) |\n|
    true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[04:0008) |   ├─ArrayItemSyntax
//@[04:0008) |   | └─BooleanLiteralSyntax
//@[04:0008) |   |   └─Token(TrueKeyword) |true|
//@[08:0009) |   ├─Token(Comma) |,|
//@[10:0047) |   ├─ArrayItemSyntax
//@[10:0047) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[12:0035) |   |   ├─ObjectPropertySyntax
//@[12:0015) |   |   | ├─IdentifierSyntax
//@[12:0015) |   |   | | └─Token(Identifier) |foo|
//@[15:0016) |   |   | ├─Token(Colon) |:|
//@[17:0035) |   |   | └─StringSyntax
//@[17:0035) |   |   |   └─Token(StringComplete) |'object width: 37'|
//@[46:0047) |   |   └─Token(RightBrace) |}|
//@[47:0048) |   ├─Token(Comma) |,|
//@[49:0070) |   ├─ArrayItemSyntax
//@[49:0070) |   | └─StringSyntax
//@[49:0070) |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxxx'|
//@[71:0072) |   └─Token(RightSquare) |]|
//@[72:0073) ├─Token(NewLine) |\n|
var w79 = [true
//@[00:0085) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w79|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0085) | └─ArraySyntax
//@[10:0011) |   ├─Token(LeftSquare) |[|
//@[11:0015) |   ├─ArrayItemSyntax
//@[11:0015) |   | └─BooleanLiteralSyntax
//@[11:0015) |   |   └─Token(TrueKeyword) |true|
//@[15:0016) |   ├─Token(NewLine) |\n|
    { /* xxxx */ foo: 'object width: 38' }
//@[04:0042) |   ├─ArrayItemSyntax
//@[04:0042) |   | └─ObjectSyntax
//@[04:0005) |   |   ├─Token(LeftBrace) |{|
//@[17:0040) |   |   ├─ObjectPropertySyntax
//@[17:0020) |   |   | ├─IdentifierSyntax
//@[17:0020) |   |   | | └─Token(Identifier) |foo|
//@[20:0021) |   |   | ├─Token(Colon) |:|
//@[22:0040) |   |   | └─StringSyntax
//@[22:0040) |   |   |   └─Token(StringComplete) |'object width: 38'|
//@[41:0042) |   |   └─Token(RightBrace) |}|
//@[42:0043) |   ├─Token(NewLine) |\n|
    'xxxxxxxxxxxxxxxxxx' ]
//@[04:0024) |   ├─ArrayItemSyntax
//@[04:0024) |   | └─StringSyntax
//@[04:0024) |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxx'|
//@[25:0026) |   └─Token(RightSquare) |]|
//@[26:0027) ├─Token(NewLine) |\n|
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }
//@[00:0083) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w80|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0083) | └─ArraySyntax
//@[10:0011) |   ├─Token(LeftSquare) |[|
//@[11:0015) |   ├─ArrayItemSyntax
//@[11:0015) |   | └─BooleanLiteralSyntax
//@[11:0015) |   |   └─Token(TrueKeyword) |true|
//@[15:0016) |   ├─Token(Comma) |,|
//@[17:0056) |   ├─ArrayItemSyntax
//@[17:0056) |   | └─ObjectSyntax
//@[17:0018) |   |   ├─Token(LeftBrace) |{|
//@[19:0054) |   |   ├─ObjectPropertySyntax
//@[19:0022) |   |   | ├─IdentifierSyntax
//@[19:0022) |   |   | | └─Token(Identifier) |foo|
//@[22:0023) |   |   | ├─Token(Colon) |:|
//@[24:0054) |   |   | └─StringSyntax
//@[24:0054) |   |   |   └─Token(StringComplete) |'object width: 39 xxxxxxxxxxx'|
//@[55:0056) |   |   └─Token(RightBrace) |}|
//@[56:0057) |   ├─Token(NewLine) |\n|
    'xxxxxxxxxxxxxxxxxxx']
//@[04:0025) |   ├─ArrayItemSyntax
//@[04:0025) |   | └─StringSyntax
//@[04:0025) |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxxx'|
//@[25:0026) |   └─Token(RightSquare) |]|
//@[26:0027) ├─Token(NewLine) |\n|
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w81|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0082) | └─ArraySyntax
//@[10:0011) |   ├─Token(LeftSquare) |[|
//@[11:0015) |   ├─ArrayItemSyntax
//@[11:0015) |   | └─BooleanLiteralSyntax
//@[11:0015) |   |   └─Token(TrueKeyword) |true|
//@[15:0016) |   ├─Token(Comma) |,|
//@[17:0057) |   ├─ArrayItemSyntax
//@[17:0057) |   | └─ObjectSyntax
//@[17:0018) |   |   ├─Token(LeftBrace) |{|
//@[19:0055) |   |   ├─ObjectPropertySyntax
//@[19:0022) |   |   | ├─IdentifierSyntax
//@[19:0022) |   |   | | └─Token(Identifier) |foo|
//@[22:0023) |   |   | ├─Token(Colon) |:|
//@[24:0055) |   |   | └─StringSyntax
//@[24:0055) |   |   |   └─Token(StringComplete) |'object width: 40 xxxxxxxxxxxx'|
//@[56:0057) |   |   └─Token(RightBrace) |}|
//@[57:0058) |   ├─Token(Comma) |,|
//@[59:0080) |   ├─ArrayItemSyntax
//@[59:0080) |   | └─StringSyntax
//@[59:0080) |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxxx'|
//@[81:0082) |   └─Token(RightSquare) |]|
//@[82:0083) ├─Token(NewLine) |\n|
var w82 = [  true, concat(/* function width: 41 */123, 456) /* xxxxxxxxxxxxxxxx */ ]
//@[00:0084) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |w82|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0084) | └─ArraySyntax
//@[10:0011) |   ├─Token(LeftSquare) |[|
//@[13:0017) |   ├─ArrayItemSyntax
//@[13:0017) |   | └─BooleanLiteralSyntax
//@[13:0017) |   |   └─Token(TrueKeyword) |true|
//@[17:0018) |   ├─Token(Comma) |,|
//@[19:0059) |   ├─ArrayItemSyntax
//@[19:0059) |   | └─FunctionCallSyntax
//@[19:0025) |   |   ├─IdentifierSyntax
//@[19:0025) |   |   | └─Token(Identifier) |concat|
//@[25:0026) |   |   ├─Token(LeftParen) |(|
//@[50:0053) |   |   ├─FunctionArgumentSyntax
//@[50:0053) |   |   | └─IntegerLiteralSyntax
//@[50:0053) |   |   |   └─Token(Integer) |123|
//@[53:0054) |   |   ├─Token(Comma) |,|
//@[55:0058) |   |   ├─FunctionArgumentSyntax
//@[55:0058) |   |   | └─IntegerLiteralSyntax
//@[55:0058) |   |   |   └─Token(Integer) |456|
//@[58:0059) |   |   └─Token(RightParen) |)|
//@[83:0084) |   └─Token(RightSquare) |]|
//@[84:0086) ├─Token(NewLine) |\n\n|

var w78_ ={ foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
//@[00:0077) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |w78_|
//@[09:0010) | ├─Token(Assignment) |=|
//@[10:0077) | └─ObjectSyntax
//@[10:0011) |   ├─Token(LeftBrace) |{|
//@[12:0020) |   ├─ObjectPropertySyntax
//@[12:0015) |   | ├─IdentifierSyntax
//@[12:0015) |   | | └─Token(Identifier) |foo|
//@[15:0016) |   | ├─Token(Colon) |:|
//@[17:0020) |   | └─IntegerLiteralSyntax
//@[17:0020) |   |   └─Token(Integer) |123|
//@[20:0021) |   ├─Token(Comma) |,|
//@[33:0075) |   ├─ObjectPropertySyntax
//@[33:0036) |   | ├─IdentifierSyntax
//@[33:0036) |   | | └─Token(Identifier) |baz|
//@[36:0037) |   | ├─Token(Colon) |:|
//@[38:0075) |   | └─ArraySyntax
//@[38:0039) |   |   ├─Token(LeftSquare) |[|
//@[39:0052) |   |   ├─ArrayItemSyntax
//@[39:0052) |   |   | └─StringSyntax
//@[39:0052) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[52:0053) |   |   ├─Token(Comma) |,|
//@[54:0074) |   |   ├─ArrayItemSyntax
//@[54:0074) |   |   | └─StringSyntax
//@[54:0074) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxx'|
//@[74:0075) |   |   └─Token(RightSquare) |]|
//@[76:0077) |   └─Token(RightBrace) |}|
//@[77:0078) ├─Token(NewLine) |\n|
var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@[00:0068) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |w79_|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0068) | └─ObjectSyntax
//@[11:0012) |   ├─Token(LeftBrace) |{|
//@[13:0021) |   ├─ObjectPropertySyntax
//@[13:0016) |   | ├─IdentifierSyntax
//@[13:0016) |   | | └─Token(Identifier) |foo|
//@[16:0017) |   | ├─Token(Colon) |:|
//@[18:0021) |   | └─IntegerLiteralSyntax
//@[18:0021) |   |   └─Token(Integer) |123|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0032) |   ├─ObjectPropertySyntax
//@[23:0026) |   | ├─IdentifierSyntax
//@[23:0026) |   | | └─Token(Identifier) |bar|
//@[26:0027) |   | ├─Token(Colon) |:|
//@[28:0032) |   | └─BooleanLiteralSyntax
//@[28:0032) |   |   └─Token(TrueKeyword) |true|
//@[32:0033) |   ├─Token(Comma) |,|
//@[34:0066) |   ├─ObjectPropertySyntax
//@[34:0037) |   | ├─IdentifierSyntax
//@[34:0037) |   | | └─Token(Identifier) |baz|
//@[37:0038) |   | ├─Token(Colon) |:|
//@[39:0066) |   | └─ArraySyntax
//@[39:0040) |   |   ├─Token(LeftSquare) |[|
//@[40:0053) |   |   ├─ArrayItemSyntax
//@[40:0053) |   |   | └─StringSyntax
//@[40:0053) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[53:0054) |   |   ├─Token(Comma) |,|
//@[55:0065) |   |   ├─ArrayItemSyntax
//@[55:0065) |   |   | └─StringSyntax
//@[55:0065) |   |   |   └─Token(StringComplete) |'xxxxxxxx'|
//@[65:0066) |   |   └─Token(RightSquare) |]|
//@[67:0068) |   └─Token(RightBrace) |}|
//@[68:0069) ├─Token(NewLine) |\n|
var w80_ = { foo: 123, bar: true, baz: [
//@[00:0085) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |w80_|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0085) | └─ObjectSyntax
//@[11:0012) |   ├─Token(LeftBrace) |{|
//@[13:0021) |   ├─ObjectPropertySyntax
//@[13:0016) |   | ├─IdentifierSyntax
//@[13:0016) |   | | └─Token(Identifier) |foo|
//@[16:0017) |   | ├─Token(Colon) |:|
//@[18:0021) |   | └─IntegerLiteralSyntax
//@[18:0021) |   |   └─Token(Integer) |123|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0032) |   ├─ObjectPropertySyntax
//@[23:0026) |   | ├─IdentifierSyntax
//@[23:0026) |   | | └─Token(Identifier) |bar|
//@[26:0027) |   | ├─Token(Colon) |:|
//@[28:0032) |   | └─BooleanLiteralSyntax
//@[28:0032) |   |   └─Token(TrueKeyword) |true|
//@[32:0033) |   ├─Token(Comma) |,|
//@[34:0083) |   ├─ObjectPropertySyntax
//@[34:0037) |   | ├─IdentifierSyntax
//@[34:0037) |   | | └─Token(Identifier) |baz|
//@[37:0038) |   | ├─Token(Colon) |:|
//@[39:0083) |   | └─ArraySyntax
//@[39:0040) |   |   ├─Token(LeftSquare) |[|
//@[40:0041) |   |   ├─Token(NewLine) |\n|
    'xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxx'] } // suffix
//@[04:0017) |   |   ├─ArrayItemSyntax
//@[04:0017) |   |   | └─StringSyntax
//@[04:0017) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[17:0018) |   |   ├─Token(Comma) |,|
//@[19:0041) |   |   ├─ArrayItemSyntax
//@[19:0041) |   |   | └─StringSyntax
//@[19:0041) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxxxx'|
//@[41:0042) |   |   └─Token(RightSquare) |]|
//@[43:0044) |   └─Token(RightBrace) |}|
//@[54:0055) ├─Token(NewLine) |\n|
var w81_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx'] }
//@[00:0081) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |w81_|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0081) | └─ObjectSyntax
//@[11:0012) |   ├─Token(LeftBrace) |{|
//@[13:0021) |   ├─ObjectPropertySyntax
//@[13:0016) |   | ├─IdentifierSyntax
//@[13:0016) |   | | └─Token(Identifier) |foo|
//@[16:0017) |   | ├─Token(Colon) |:|
//@[18:0021) |   | └─IntegerLiteralSyntax
//@[18:0021) |   |   └─Token(Integer) |123|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0032) |   ├─ObjectPropertySyntax
//@[23:0026) |   | ├─IdentifierSyntax
//@[23:0026) |   | | └─Token(Identifier) |bar|
//@[26:0027) |   | ├─Token(Colon) |:|
//@[28:0032) |   | └─BooleanLiteralSyntax
//@[28:0032) |   |   └─Token(TrueKeyword) |true|
//@[32:0033) |   ├─Token(Comma) |,|
//@[34:0079) |   ├─ObjectPropertySyntax
//@[34:0037) |   | ├─IdentifierSyntax
//@[34:0037) |   | | └─Token(Identifier) |baz|
//@[37:0038) |   | ├─Token(Colon) |:|
//@[39:0079) |   | └─ArraySyntax
//@[39:0040) |   |   ├─Token(LeftSquare) |[|
//@[40:0053) |   |   ├─ArrayItemSyntax
//@[40:0053) |   |   | └─StringSyntax
//@[40:0053) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxx'|
//@[53:0054) |   |   ├─Token(Comma) |,|
//@[55:0078) |   |   ├─ArrayItemSyntax
//@[55:0078) |   |   | └─StringSyntax
//@[55:0078) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxxxxxx'|
//@[78:0079) |   |   └─Token(RightSquare) |]|
//@[80:0081) |   └─Token(RightBrace) |}|
//@[81:0082) ├─Token(NewLine) |\n|
var w82_ = { foo: 123, bar: true, baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx'] }
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |w82_|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0082) | └─ObjectSyntax
//@[11:0012) |   ├─Token(LeftBrace) |{|
//@[13:0021) |   ├─ObjectPropertySyntax
//@[13:0016) |   | ├─IdentifierSyntax
//@[13:0016) |   | | └─Token(Identifier) |foo|
//@[16:0017) |   | ├─Token(Colon) |:|
//@[18:0021) |   | └─IntegerLiteralSyntax
//@[18:0021) |   |   └─Token(Integer) |123|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0032) |   ├─ObjectPropertySyntax
//@[23:0026) |   | ├─IdentifierSyntax
//@[23:0026) |   | | └─Token(Identifier) |bar|
//@[26:0027) |   | ├─Token(Colon) |:|
//@[28:0032) |   | └─BooleanLiteralSyntax
//@[28:0032) |   |   └─Token(TrueKeyword) |true|
//@[32:0033) |   ├─Token(Comma) |,|
//@[34:0080) |   ├─ObjectPropertySyntax
//@[34:0037) |   | ├─IdentifierSyntax
//@[34:0037) |   | | └─Token(Identifier) |baz|
//@[37:0038) |   | ├─Token(Colon) |:|
//@[39:0080) |   | └─ArraySyntax
//@[39:0040) |   |   ├─Token(LeftSquare) |[|
//@[40:0058) |   |   ├─ArrayItemSyntax
//@[40:0058) |   |   | └─StringSyntax
//@[40:0058) |   |   |   └─Token(StringComplete) |'array length: 41'|
//@[58:0059) |   |   ├─Token(Comma) |,|
//@[60:0079) |   |   ├─ArrayItemSyntax
//@[60:0079) |   |   | └─StringSyntax
//@[60:0079) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxxxxxx'|
//@[79:0080) |   |   └─Token(RightSquare) |]|
//@[81:0082) |   └─Token(RightBrace) |}|
//@[82:0084) ├─Token(NewLine) |\n\n|

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx'})
//@[00:0078) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |w78__|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0078) | └─FunctionCallSyntax
//@[12:0017) |   ├─IdentifierSyntax
//@[12:0017) |   | └─Token(Identifier) |union|
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0034) |   ├─FunctionArgumentSyntax
//@[18:0034) |   | └─ObjectSyntax
//@[18:0019) |   |   ├─Token(LeftBrace) |{|
//@[20:0032) |   |   ├─ObjectPropertySyntax
//@[20:0023) |   |   | ├─IdentifierSyntax
//@[20:0023) |   |   | | └─Token(Identifier) |foo|
//@[23:0024) |   |   | ├─Token(Colon) |:|
//@[25:0032) |   |   | └─StringSyntax
//@[25:0032) |   |   |   └─Token(StringComplete) |'xxxxx'|
//@[33:0034) |   |   └─Token(RightBrace) |}|
//@[34:0035) |   ├─Token(Comma) |,|
//@[36:0056) |   ├─FunctionArgumentSyntax
//@[36:0056) |   | └─ObjectSyntax
//@[36:0037) |   |   ├─Token(LeftBrace) |{|
//@[38:0054) |   |   ├─ObjectPropertySyntax
//@[38:0041) |   |   | ├─IdentifierSyntax
//@[38:0041) |   |   | | └─Token(Identifier) |bar|
//@[41:0042) |   |   | ├─Token(Colon) |:|
//@[43:0054) |   |   | └─StringSyntax
//@[43:0054) |   |   |   └─Token(StringComplete) |'xxxxxxxxx'|
//@[55:0056) |   |   └─Token(RightBrace) |}|
//@[56:0057) |   ├─Token(Comma) |,|
//@[58:0077) |   ├─FunctionArgumentSyntax
//@[58:0077) |   | └─ObjectSyntax
//@[58:0059) |   |   ├─Token(LeftBrace) |{|
//@[60:0076) |   |   ├─ObjectPropertySyntax
//@[60:0063) |   |   | ├─IdentifierSyntax
//@[60:0063) |   |   | | └─Token(Identifier) |baz|
//@[63:0064) |   |   | ├─Token(Colon) |:|
//@[65:0076) |   |   | └─StringSyntax
//@[65:0076) |   |   |   └─Token(StringComplete) |'xxxxxxxxx'|
//@[76:0077) |   |   └─Token(RightBrace) |}|
//@[77:0078) |   └─Token(RightParen) |)|
//@[78:0079) ├─Token(NewLine) |\n|
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' },
//@[00:0083) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |w79__|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0083) | └─FunctionCallSyntax
//@[12:0017) |   ├─IdentifierSyntax
//@[12:0017) |   | └─Token(Identifier) |union|
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0034) |   ├─FunctionArgumentSyntax
//@[18:0034) |   | └─ObjectSyntax
//@[18:0019) |   |   ├─Token(LeftBrace) |{|
//@[20:0032) |   |   ├─ObjectPropertySyntax
//@[20:0023) |   |   | ├─IdentifierSyntax
//@[20:0023) |   |   | | └─Token(Identifier) |foo|
//@[23:0024) |   |   | ├─Token(Colon) |:|
//@[25:0032) |   |   | └─StringSyntax
//@[25:0032) |   |   |   └─Token(StringComplete) |'xxxxx'|
//@[33:0034) |   |   └─Token(RightBrace) |}|
//@[34:0035) |   ├─Token(Comma) |,|
//@[36:0056) |   ├─FunctionArgumentSyntax
//@[36:0056) |   | └─ObjectSyntax
//@[36:0037) |   |   ├─Token(LeftBrace) |{|
//@[38:0054) |   |   ├─ObjectPropertySyntax
//@[38:0041) |   |   | ├─IdentifierSyntax
//@[38:0041) |   |   | | └─Token(Identifier) |bar|
//@[41:0042) |   |   | ├─Token(Colon) |:|
//@[43:0054) |   |   | └─StringSyntax
//@[43:0054) |   |   |   └─Token(StringComplete) |'xxxxxxxxx'|
//@[55:0056) |   |   └─Token(RightBrace) |}|
//@[56:0057) |   ├─Token(Comma) |,|
//@[57:0058) |   ├─Token(NewLine) |\n|
    { baz: 'xxxxxxxxxx'}) // suffix
//@[04:0024) |   ├─FunctionArgumentSyntax
//@[04:0024) |   | └─ObjectSyntax
//@[04:0005) |   |   ├─Token(LeftBrace) |{|
//@[06:0023) |   |   ├─ObjectPropertySyntax
//@[06:0009) |   |   | ├─IdentifierSyntax
//@[06:0009) |   |   | | └─Token(Identifier) |baz|
//@[09:0010) |   |   | ├─Token(Colon) |:|
//@[11:0023) |   |   | └─StringSyntax
//@[11:0023) |   |   |   └─Token(StringComplete) |'xxxxxxxxxx'|
//@[23:0024) |   |   └─Token(RightBrace) |}|
//@[24:0025) |   └─Token(RightParen) |)|
//@[35:0036) ├─Token(NewLine) |\n|
var w80__ = union(
//@[00:0093) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |w80__|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0093) | └─FunctionCallSyntax
//@[12:0017) |   ├─IdentifierSyntax
//@[12:0017) |   | └─Token(Identifier) |union|
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0019) |   ├─Token(NewLine) |\n|
    { foo: 'xxxxxx' },
//@[04:0021) |   ├─FunctionArgumentSyntax
//@[04:0021) |   | └─ObjectSyntax
//@[04:0005) |   |   ├─Token(LeftBrace) |{|
//@[06:0019) |   |   ├─ObjectPropertySyntax
//@[06:0009) |   |   | ├─IdentifierSyntax
//@[06:0009) |   |   | | └─Token(Identifier) |foo|
//@[09:0010) |   |   | ├─Token(Colon) |:|
//@[11:0019) |   |   | └─StringSyntax
//@[11:0019) |   |   |   └─Token(StringComplete) |'xxxxxx'|
//@[20:0021) |   |   └─Token(RightBrace) |}|
//@[21:0022) |   ├─Token(Comma) |,|
//@[22:0023) |   ├─Token(NewLine) |\n|
    { bar: 'xxxxxx' },
//@[04:0021) |   ├─FunctionArgumentSyntax
//@[04:0021) |   | └─ObjectSyntax
//@[04:0005) |   |   ├─Token(LeftBrace) |{|
//@[06:0019) |   |   ├─ObjectPropertySyntax
//@[06:0009) |   |   | ├─IdentifierSyntax
//@[06:0009) |   |   | | └─Token(Identifier) |bar|
//@[09:0010) |   |   | ├─Token(Colon) |:|
//@[11:0019) |   |   | └─StringSyntax
//@[11:0019) |   |   |   └─Token(StringComplete) |'xxxxxx'|
//@[20:0021) |   |   └─Token(RightBrace) |}|
//@[21:0022) |   ├─Token(Comma) |,|
//@[22:0023) |   ├─Token(NewLine) |\n|
    { baz: 'xxxxxxxxxxxxx'})
//@[04:0027) |   ├─FunctionArgumentSyntax
//@[04:0027) |   | └─ObjectSyntax
//@[04:0005) |   |   ├─Token(LeftBrace) |{|
//@[06:0026) |   |   ├─ObjectPropertySyntax
//@[06:0009) |   |   | ├─IdentifierSyntax
//@[06:0009) |   |   | | └─Token(Identifier) |baz|
//@[09:0010) |   |   | ├─Token(Colon) |:|
//@[11:0026) |   |   | └─StringSyntax
//@[11:0026) |   |   |   └─Token(StringComplete) |'xxxxxxxxxxxxx'|
//@[26:0027) |   |   └─Token(RightBrace) |}|
//@[27:0028) |   └─Token(RightParen) |)|
//@[28:0029) ├─Token(NewLine) |\n|
var w81__ = union({ foo: 'x' } /* xxx */, any({ baz: 'func call length: 38  ' }))
//@[00:0081) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |w81__|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0081) | └─FunctionCallSyntax
//@[12:0017) |   ├─IdentifierSyntax
//@[12:0017) |   | └─Token(Identifier) |union|
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0030) |   ├─FunctionArgumentSyntax
//@[18:0030) |   | └─ObjectSyntax
//@[18:0019) |   |   ├─Token(LeftBrace) |{|
//@[20:0028) |   |   ├─ObjectPropertySyntax
//@[20:0023) |   |   | ├─IdentifierSyntax
//@[20:0023) |   |   | | └─Token(Identifier) |foo|
//@[23:0024) |   |   | ├─Token(Colon) |:|
//@[25:0028) |   |   | └─StringSyntax
//@[25:0028) |   |   |   └─Token(StringComplete) |'x'|
//@[29:0030) |   |   └─Token(RightBrace) |}|
//@[40:0041) |   ├─Token(Comma) |,|
//@[42:0080) |   ├─FunctionArgumentSyntax
//@[42:0080) |   | └─FunctionCallSyntax
//@[42:0045) |   |   ├─IdentifierSyntax
//@[42:0045) |   |   | └─Token(Identifier) |any|
//@[45:0046) |   |   ├─Token(LeftParen) |(|
//@[46:0079) |   |   ├─FunctionArgumentSyntax
//@[46:0079) |   |   | └─ObjectSyntax
//@[46:0047) |   |   |   ├─Token(LeftBrace) |{|
//@[48:0077) |   |   |   ├─ObjectPropertySyntax
//@[48:0051) |   |   |   | ├─IdentifierSyntax
//@[48:0051) |   |   |   | | └─Token(Identifier) |baz|
//@[51:0052) |   |   |   | ├─Token(Colon) |:|
//@[53:0077) |   |   |   | └─StringSyntax
//@[53:0077) |   |   |   |   └─Token(StringComplete) |'func call length: 38  '|
//@[78:0079) |   |   |   └─Token(RightBrace) |}|
//@[79:0080) |   |   └─Token(RightParen) |)|
//@[80:0081) |   └─Token(RightParen) |)|
//@[81:0082) ├─Token(NewLine) |\n|
var w82__ = union({ foo: 'x', bar: 'x' }, any({ baz: 'func call length: 39   ' }))
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |w82__|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0082) | └─FunctionCallSyntax
//@[12:0017) |   ├─IdentifierSyntax
//@[12:0017) |   | └─Token(Identifier) |union|
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0040) |   ├─FunctionArgumentSyntax
//@[18:0040) |   | └─ObjectSyntax
//@[18:0019) |   |   ├─Token(LeftBrace) |{|
//@[20:0028) |   |   ├─ObjectPropertySyntax
//@[20:0023) |   |   | ├─IdentifierSyntax
//@[20:0023) |   |   | | └─Token(Identifier) |foo|
//@[23:0024) |   |   | ├─Token(Colon) |:|
//@[25:0028) |   |   | └─StringSyntax
//@[25:0028) |   |   |   └─Token(StringComplete) |'x'|
//@[28:0029) |   |   ├─Token(Comma) |,|
//@[30:0038) |   |   ├─ObjectPropertySyntax
//@[30:0033) |   |   | ├─IdentifierSyntax
//@[30:0033) |   |   | | └─Token(Identifier) |bar|
//@[33:0034) |   |   | ├─Token(Colon) |:|
//@[35:0038) |   |   | └─StringSyntax
//@[35:0038) |   |   |   └─Token(StringComplete) |'x'|
//@[39:0040) |   |   └─Token(RightBrace) |}|
//@[40:0041) |   ├─Token(Comma) |,|
//@[42:0081) |   ├─FunctionArgumentSyntax
//@[42:0081) |   | └─FunctionCallSyntax
//@[42:0045) |   |   ├─IdentifierSyntax
//@[42:0045) |   |   | └─Token(Identifier) |any|
//@[45:0046) |   |   ├─Token(LeftParen) |(|
//@[46:0080) |   |   ├─FunctionArgumentSyntax
//@[46:0080) |   |   | └─ObjectSyntax
//@[46:0047) |   |   |   ├─Token(LeftBrace) |{|
//@[48:0078) |   |   |   ├─ObjectPropertySyntax
//@[48:0051) |   |   |   | ├─IdentifierSyntax
//@[48:0051) |   |   |   | | └─Token(Identifier) |baz|
//@[51:0052) |   |   |   | ├─Token(Colon) |:|
//@[53:0078) |   |   |   | └─StringSyntax
//@[53:0078) |   |   |   |   └─Token(StringComplete) |'func call length: 39   '|
//@[79:0080) |   |   |   └─Token(RightBrace) |}|
//@[80:0081) |   |   └─Token(RightParen) |)|
//@[81:0082) |   └─Token(RightParen) |)|
//@[82:0084) ├─Token(NewLine) |\n\n|

var w78___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true
//@[00:0078) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w78___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[48:0078) | └─TernaryOperationSyntax
//@[48:0052) |   ├─BooleanLiteralSyntax
//@[48:0052) |   | └─Token(TrueKeyword) |true|
//@[52:0053) |   ├─Token(NewLine) |\n|
? 1234567890
//@[00:0001) |   ├─Token(Question) |?|
//@[02:0012) |   ├─IntegerLiteralSyntax
//@[02:0012) |   | └─Token(Integer) |1234567890|
//@[12:0013) |   ├─Token(NewLine) |\n|
: 1234567890
//@[00:0001) |   ├─Token(Colon) |:|
//@[02:0012) |   └─IntegerLiteralSyntax
//@[02:0012) |     └─Token(Integer) |1234567890|
//@[12:0013) ├─Token(NewLine) |\n|
var w79___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? { foo: 1 } : [12345678]
//@[00:0079) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w79___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[49:0079) | └─TernaryOperationSyntax
//@[49:0053) |   ├─BooleanLiteralSyntax
//@[49:0053) |   | └─Token(TrueKeyword) |true|
//@[54:0055) |   ├─Token(Question) |?|
//@[56:0066) |   ├─ObjectSyntax
//@[56:0057) |   | ├─Token(LeftBrace) |{|
//@[58:0064) |   | ├─ObjectPropertySyntax
//@[58:0061) |   | | ├─IdentifierSyntax
//@[58:0061) |   | | | └─Token(Identifier) |foo|
//@[61:0062) |   | | ├─Token(Colon) |:|
//@[63:0064) |   | | └─IntegerLiteralSyntax
//@[63:0064) |   | |   └─Token(Integer) |1|
//@[65:0066) |   | └─Token(RightBrace) |}|
//@[67:0068) |   ├─Token(Colon) |:|
//@[69:0079) |   └─ArraySyntax
//@[69:0070) |     ├─Token(LeftSquare) |[|
//@[70:0078) |     ├─ArrayItemSyntax
//@[70:0078) |     | └─IntegerLiteralSyntax
//@[70:0078) |     |   └─Token(Integer) |12345678|
//@[78:0079) |     └─Token(RightSquare) |]|
//@[79:0080) ├─Token(NewLine) |\n|
var w80___ = true ? { foo: true, bar: false } : [123, 234, 456, { xyz: 'xxxx' }]
//@[00:0080) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w80___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0080) | └─TernaryOperationSyntax
//@[13:0017) |   ├─BooleanLiteralSyntax
//@[13:0017) |   | └─Token(TrueKeyword) |true|
//@[18:0019) |   ├─Token(Question) |?|
//@[20:0045) |   ├─ObjectSyntax
//@[20:0021) |   | ├─Token(LeftBrace) |{|
//@[22:0031) |   | ├─ObjectPropertySyntax
//@[22:0025) |   | | ├─IdentifierSyntax
//@[22:0025) |   | | | └─Token(Identifier) |foo|
//@[25:0026) |   | | ├─Token(Colon) |:|
//@[27:0031) |   | | └─BooleanLiteralSyntax
//@[27:0031) |   | |   └─Token(TrueKeyword) |true|
//@[31:0032) |   | ├─Token(Comma) |,|
//@[33:0043) |   | ├─ObjectPropertySyntax
//@[33:0036) |   | | ├─IdentifierSyntax
//@[33:0036) |   | | | └─Token(Identifier) |bar|
//@[36:0037) |   | | ├─Token(Colon) |:|
//@[38:0043) |   | | └─BooleanLiteralSyntax
//@[38:0043) |   | |   └─Token(FalseKeyword) |false|
//@[44:0045) |   | └─Token(RightBrace) |}|
//@[46:0047) |   ├─Token(Colon) |:|
//@[48:0080) |   └─ArraySyntax
//@[48:0049) |     ├─Token(LeftSquare) |[|
//@[49:0052) |     ├─ArrayItemSyntax
//@[49:0052) |     | └─IntegerLiteralSyntax
//@[49:0052) |     |   └─Token(Integer) |123|
//@[52:0053) |     ├─Token(Comma) |,|
//@[54:0057) |     ├─ArrayItemSyntax
//@[54:0057) |     | └─IntegerLiteralSyntax
//@[54:0057) |     |   └─Token(Integer) |234|
//@[57:0058) |     ├─Token(Comma) |,|
//@[59:0062) |     ├─ArrayItemSyntax
//@[59:0062) |     | └─IntegerLiteralSyntax
//@[59:0062) |     |   └─Token(Integer) |456|
//@[62:0063) |     ├─Token(Comma) |,|
//@[64:0079) |     ├─ArrayItemSyntax
//@[64:0079) |     | └─ObjectSyntax
//@[64:0065) |     |   ├─Token(LeftBrace) |{|
//@[66:0077) |     |   ├─ObjectPropertySyntax
//@[66:0069) |     |   | ├─IdentifierSyntax
//@[66:0069) |     |   | | └─Token(Identifier) |xyz|
//@[69:0070) |     |   | ├─Token(Colon) |:|
//@[71:0077) |     |   | └─StringSyntax
//@[71:0077) |     |   |   └─Token(StringComplete) |'xxxx'|
//@[78:0079) |     |   └─Token(RightBrace) |}|
//@[79:0080) |     └─Token(RightSquare) |]|
//@[80:0081) ├─Token(NewLine) |\n|
var w81___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[00:0081) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w81___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[51:0081) | └─TernaryOperationSyntax
//@[51:0055) |   ├─BooleanLiteralSyntax
//@[51:0055) |   | └─Token(TrueKeyword) |true|
//@[56:0057) |   ├─Token(Question) |?|
//@[58:0068) |   ├─IntegerLiteralSyntax
//@[58:0068) |   | └─Token(Integer) |1234567890|
//@[69:0070) |   ├─Token(Colon) |:|
//@[71:0081) |   └─IntegerLiteralSyntax
//@[71:0081) |     └─Token(Integer) |1234567890|
//@[81:0082) ├─Token(NewLine) |\n|
var w82___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |w82___|
//@[11:0012) | ├─Token(Assignment) |=|
//@[52:0082) | └─TernaryOperationSyntax
//@[52:0056) |   ├─BooleanLiteralSyntax
//@[52:0056) |   | └─Token(TrueKeyword) |true|
//@[57:0058) |   ├─Token(Question) |?|
//@[59:0069) |   ├─IntegerLiteralSyntax
//@[59:0069) |   | └─Token(Integer) |1234567890|
//@[70:0071) |   ├─Token(Colon) |:|
//@[72:0082) |   └─IntegerLiteralSyntax
//@[72:0082) |     └─Token(Integer) |1234567890|
//@[82:0084) ├─Token(NewLine) |\n\n|

////////////////////////////////////////////////////////////////////////////////
//@[80:0081) ├─Token(NewLine) |\n|
////////////////////////// Baselines for line breakers /////////////////////////
//@[80:0081) ├─Token(NewLine) |\n|
////////////////////////////////////////////////////////////////////////////////
//@[80:0081) ├─Token(NewLine) |\n|
var forceBreak1 = {
//@[00:0035) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak1|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0035) | └─ObjectSyntax
//@[18:0019) |   ├─Token(LeftBrace) |{|
//@[19:0020) |   ├─Token(NewLine) |\n|
    foo: true
//@[04:0013) |   ├─ObjectPropertySyntax
//@[04:0007) |   | ├─IdentifierSyntax
//@[04:0007) |   | | └─Token(Identifier) |foo|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0013) |   | └─BooleanLiteralSyntax
//@[09:0013) |   |   └─Token(TrueKeyword) |true|
//@[13:0014) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
var forceBreak2 = {
//@[00:0047) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak2|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0047) | └─ObjectSyntax
//@[18:0019) |   ├─Token(LeftBrace) |{|
//@[19:0020) |   ├─Token(NewLine) |\n|
    foo: true, bar: false
//@[04:0013) |   ├─ObjectPropertySyntax
//@[04:0007) |   | ├─IdentifierSyntax
//@[04:0007) |   | | └─Token(Identifier) |foo|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0013) |   | └─BooleanLiteralSyntax
//@[09:0013) |   |   └─Token(TrueKeyword) |true|
//@[13:0014) |   ├─Token(Comma) |,|
//@[15:0025) |   ├─ObjectPropertySyntax
//@[15:0018) |   | ├─IdentifierSyntax
//@[15:0018) |   | | └─Token(Identifier) |bar|
//@[18:0019) |   | ├─Token(Colon) |:|
//@[20:0025) |   | └─BooleanLiteralSyntax
//@[20:0025) |   |   └─Token(FalseKeyword) |false|
//@[25:0026) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
var forceBreak3 = [1, 2, {
//@[00:0049) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak3|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0049) | └─ArraySyntax
//@[18:0019) |   ├─Token(LeftSquare) |[|
//@[19:0020) |   ├─ArrayItemSyntax
//@[19:0020) |   | └─IntegerLiteralSyntax
//@[19:0020) |   |   └─Token(Integer) |1|
//@[20:0021) |   ├─Token(Comma) |,|
//@[22:0023) |   ├─ArrayItemSyntax
//@[22:0023) |   | └─IntegerLiteralSyntax
//@[22:0023) |   |   └─Token(Integer) |2|
//@[23:0024) |   ├─Token(Comma) |,|
//@[25:0042) |   ├─ArrayItemSyntax
//@[25:0042) |   | └─ObjectSyntax
//@[25:0026) |   |   ├─Token(LeftBrace) |{|
//@[26:0027) |   |   ├─Token(NewLine) |\n|
    foo: true }, 3, 4]
//@[04:0013) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |foo|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0013) |   |   | └─BooleanLiteralSyntax
//@[09:0013) |   |   |   └─Token(TrueKeyword) |true|
//@[14:0015) |   |   └─Token(RightBrace) |}|
//@[15:0016) |   ├─Token(Comma) |,|
//@[17:0018) |   ├─ArrayItemSyntax
//@[17:0018) |   | └─IntegerLiteralSyntax
//@[17:0018) |   |   └─Token(Integer) |3|
//@[18:0019) |   ├─Token(Comma) |,|
//@[20:0021) |   ├─ArrayItemSyntax
//@[20:0021) |   | └─IntegerLiteralSyntax
//@[20:0021) |   |   └─Token(Integer) |4|
//@[21:0022) |   └─Token(RightSquare) |]|
//@[22:0023) ├─Token(NewLine) |\n|
var forceBreak4 = { foo: true, bar: false // force break
//@[00:0058) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak4|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0058) | └─ObjectSyntax
//@[18:0019) |   ├─Token(LeftBrace) |{|
//@[20:0029) |   ├─ObjectPropertySyntax
//@[20:0023) |   | ├─IdentifierSyntax
//@[20:0023) |   | | └─Token(Identifier) |foo|
//@[23:0024) |   | ├─Token(Colon) |:|
//@[25:0029) |   | └─BooleanLiteralSyntax
//@[25:0029) |   |   └─Token(TrueKeyword) |true|
//@[29:0030) |   ├─Token(Comma) |,|
//@[31:0041) |   ├─ObjectPropertySyntax
//@[31:0034) |   | ├─IdentifierSyntax
//@[31:0034) |   | | └─Token(Identifier) |bar|
//@[34:0035) |   | ├─Token(Colon) |:|
//@[36:0041) |   | └─BooleanLiteralSyntax
//@[36:0041) |   |   └─Token(FalseKeyword) |false|
//@[56:0057) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
var forceBreak5 = { foo: true
//@[00:0048) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak5|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0048) | └─ObjectSyntax
//@[18:0019) |   ├─Token(LeftBrace) |{|
//@[20:0029) |   ├─ObjectPropertySyntax
//@[20:0023) |   | ├─IdentifierSyntax
//@[20:0023) |   | | └─Token(Identifier) |foo|
//@[23:0024) |   | ├─Token(Colon) |:|
//@[25:0029) |   | └─BooleanLiteralSyntax
//@[25:0029) |   |   └─Token(TrueKeyword) |true|
//@[29:0030) |   ├─Token(NewLine) |\n|
/* force break */}
//@[17:0018) |   └─Token(RightBrace) |}|
//@[18:0019) ├─Token(NewLine) |\n|
var forceBreak6 = { foo: true
//@[00:0076) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak6|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0076) | └─ObjectSyntax
//@[18:0019) |   ├─Token(LeftBrace) |{|
//@[20:0029) |   ├─ObjectPropertySyntax
//@[20:0023) |   | ├─IdentifierSyntax
//@[20:0023) |   | | └─Token(Identifier) |foo|
//@[23:0024) |   | ├─Token(Colon) |:|
//@[25:0029) |   | └─BooleanLiteralSyntax
//@[25:0029) |   |   └─Token(TrueKeyword) |true|
//@[29:0030) |   ├─Token(NewLine) |\n|
    bar: false
//@[04:0014) |   ├─ObjectPropertySyntax
//@[04:0007) |   | ├─IdentifierSyntax
//@[04:0007) |   | | └─Token(Identifier) |bar|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0014) |   | └─BooleanLiteralSyntax
//@[09:0014) |   |   └─Token(FalseKeyword) |false|
//@[14:0015) |   ├─Token(NewLine) |\n|
    baz: 123
//@[04:0012) |   ├─ObjectPropertySyntax
//@[04:0007) |   | ├─IdentifierSyntax
//@[04:0007) |   | | └─Token(Identifier) |baz|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0012) |   | └─IntegerLiteralSyntax
//@[09:0012) |   |   └─Token(Integer) |123|
//@[12:0013) |   ├─Token(NewLine) |\n|
/* force break */}
//@[17:0018) |   └─Token(RightBrace) |}|
//@[18:0019) ├─Token(NewLine) |\n|
var forceBreak7 = [1, 2 // force break
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak7|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0040) | └─ArraySyntax
//@[18:0019) |   ├─Token(LeftSquare) |[|
//@[19:0020) |   ├─ArrayItemSyntax
//@[19:0020) |   | └─IntegerLiteralSyntax
//@[19:0020) |   |   └─Token(Integer) |1|
//@[20:0021) |   ├─Token(Comma) |,|
//@[22:0023) |   ├─ArrayItemSyntax
//@[22:0023) |   | └─IntegerLiteralSyntax
//@[22:0023) |   |   └─Token(Integer) |2|
//@[38:0039) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
var forceBreak8 = [1, 2
//@[00:0047) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak8|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0047) | └─ArraySyntax
//@[18:0019) |   ├─Token(LeftSquare) |[|
//@[19:0020) |   ├─ArrayItemSyntax
//@[19:0020) |   | └─IntegerLiteralSyntax
//@[19:0020) |   |   └─Token(Integer) |1|
//@[20:0021) |   ├─Token(Comma) |,|
//@[22:0023) |   ├─ArrayItemSyntax
//@[22:0023) |   | └─IntegerLiteralSyntax
//@[22:0023) |   |   └─Token(Integer) |2|
//@[23:0024) |   ├─Token(NewLine) |\n|
    /* force break */ ]
//@[22:0023) |   └─Token(RightSquare) |]|
//@[23:0024) ├─Token(NewLine) |\n|
var forceBreak9 = [1, 2, {
//@[00:0058) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |forceBreak9|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0058) | └─ArraySyntax
//@[18:0019) |   ├─Token(LeftSquare) |[|
//@[19:0020) |   ├─ArrayItemSyntax
//@[19:0020) |   | └─IntegerLiteralSyntax
//@[19:0020) |   |   └─Token(Integer) |1|
//@[20:0021) |   ├─Token(Comma) |,|
//@[22:0023) |   ├─ArrayItemSyntax
//@[22:0023) |   | └─IntegerLiteralSyntax
//@[22:0023) |   |   └─Token(Integer) |2|
//@[23:0024) |   ├─Token(Comma) |,|
//@[25:0057) |   ├─ArrayItemSyntax
//@[25:0057) |   | └─ObjectSyntax
//@[25:0026) |   |   ├─Token(LeftBrace) |{|
//@[26:0027) |   |   ├─Token(NewLine) |\n|
    foo: true
//@[04:0013) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |foo|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0013) |   |   | └─BooleanLiteralSyntax
//@[09:0013) |   |   |   └─Token(TrueKeyword) |true|
//@[13:0014) |   |   ├─Token(NewLine) |\n|
    bar: false
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |bar|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0014) |   |   | └─BooleanLiteralSyntax
//@[09:0014) |   |   |   └─Token(FalseKeyword) |false|
//@[14:0015) |   |   ├─Token(NewLine) |\n|
}]
//@[00:0001) |   |   └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0003) ├─Token(NewLine) |\n|
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |forceBreak10|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0082) | └─ArraySyntax
//@[19:0020) |   ├─Token(LeftSquare) |[|
//@[20:0021) |   ├─ArrayItemSyntax
//@[20:0021) |   | └─IntegerLiteralSyntax
//@[20:0021) |   |   └─Token(Integer) |1|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0024) |   ├─ArrayItemSyntax
//@[23:0024) |   | └─IntegerLiteralSyntax
//@[23:0024) |   |   └─Token(Integer) |2|
//@[24:0025) |   ├─Token(Comma) |,|
//@[26:0081) |   ├─ArrayItemSyntax
//@[26:0081) |   | └─FunctionCallSyntax
//@[26:0038) |   |   ├─IdentifierSyntax
//@[26:0038) |   |   | └─Token(Identifier) |intersection|
//@[38:0039) |   |   ├─Token(LeftParen) |(|
//@[39:0064) |   |   ├─FunctionArgumentSyntax
//@[39:0064) |   |   | └─ObjectSyntax
//@[39:0040) |   |   |   ├─Token(LeftBrace) |{|
//@[41:0050) |   |   |   ├─ObjectPropertySyntax
//@[41:0044) |   |   |   | ├─IdentifierSyntax
//@[41:0044) |   |   |   | | └─Token(Identifier) |foo|
//@[44:0045) |   |   |   | ├─Token(Colon) |:|
//@[46:0050) |   |   |   | └─BooleanLiteralSyntax
//@[46:0050) |   |   |   |   └─Token(TrueKeyword) |true|
//@[50:0051) |   |   |   ├─Token(Comma) |,|
//@[52:0062) |   |   |   ├─ObjectPropertySyntax
//@[52:0055) |   |   |   | ├─IdentifierSyntax
//@[52:0055) |   |   |   | | └─Token(Identifier) |bar|
//@[55:0056) |   |   |   | ├─Token(Colon) |:|
//@[57:0062) |   |   |   | └─BooleanLiteralSyntax
//@[57:0062) |   |   |   |   └─Token(FalseKeyword) |false|
//@[63:0064) |   |   |   └─Token(RightBrace) |}|
//@[64:0065) |   |   ├─Token(Comma) |,|
//@[66:0080) |   |   ├─FunctionArgumentSyntax
//@[66:0080) |   |   | └─ObjectSyntax
//@[66:0067) |   |   |   ├─Token(LeftBrace) |{|
//@[67:0068) |   |   |   ├─Token(NewLine) |\n|
  foo: true})]
//@[02:0011) |   |   |   ├─ObjectPropertySyntax
//@[02:0005) |   |   |   | ├─IdentifierSyntax
//@[02:0005) |   |   |   | | └─Token(Identifier) |foo|
//@[05:0006) |   |   |   | ├─Token(Colon) |:|
//@[07:0011) |   |   |   | └─BooleanLiteralSyntax
//@[07:0011) |   |   |   |   └─Token(TrueKeyword) |true|
//@[11:0012) |   |   |   └─Token(RightBrace) |}|
//@[12:0013) |   |   └─Token(RightParen) |)|
//@[13:0014) |   └─Token(RightSquare) |]|
//@[14:0016) ├─Token(NewLine) |\n\n|

var forceBreak11 = true // comment
//@[00:0057) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |forceBreak11|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0057) | └─TernaryOperationSyntax
//@[19:0023) |   ├─BooleanLiteralSyntax
//@[19:0023) |   | └─Token(TrueKeyword) |true|
//@[34:0035) |   ├─Token(NewLine) |\n|
    ? true
//@[04:0005) |   ├─Token(Question) |?|
//@[06:0010) |   ├─BooleanLiteralSyntax
//@[06:0010) |   | └─Token(TrueKeyword) |true|
//@[10:0011) |   ├─Token(NewLine) |\n|
    : false
//@[04:0005) |   ├─Token(Colon) |:|
//@[06:0011) |   └─BooleanLiteralSyntax
//@[06:0011) |     └─Token(FalseKeyword) |false|
//@[11:0012) ├─Token(NewLine) |\n|
var forceBreak12 = true ? true // comment
//@[00:0053) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |forceBreak12|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0053) | └─TernaryOperationSyntax
//@[19:0023) |   ├─BooleanLiteralSyntax
//@[19:0023) |   | └─Token(TrueKeyword) |true|
//@[24:0025) |   ├─Token(Question) |?|
//@[26:0030) |   ├─BooleanLiteralSyntax
//@[26:0030) |   | └─Token(TrueKeyword) |true|
//@[41:0042) |   ├─Token(NewLine) |\n|
    : false
//@[04:0005) |   ├─Token(Colon) |:|
//@[06:0011) |   └─BooleanLiteralSyntax
//@[06:0011) |     └─Token(FalseKeyword) |false|
//@[11:0012) ├─Token(NewLine) |\n|
var forceBreak13 = true
//@[00:0057) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |forceBreak13|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0057) | └─TernaryOperationSyntax
//@[19:0023) |   ├─BooleanLiteralSyntax
//@[19:0023) |   | └─Token(TrueKeyword) |true|
//@[23:0024) |   ├─Token(NewLine) |\n|
    ? true // comment
//@[04:0005) |   ├─Token(Question) |?|
//@[06:0010) |   ├─BooleanLiteralSyntax
//@[06:0010) |   | └─Token(TrueKeyword) |true|
//@[21:0022) |   ├─Token(NewLine) |\n|
    : false
//@[04:0005) |   ├─Token(Colon) |:|
//@[06:0011) |   └─BooleanLiteralSyntax
//@[06:0011) |     └─Token(FalseKeyword) |false|
//@[11:0012) ├─Token(NewLine) |\n|
var forceBreak14 = true ? {
//@[00:0049) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |forceBreak14|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0049) | └─TernaryOperationSyntax
//@[19:0023) |   ├─BooleanLiteralSyntax
//@[19:0023) |   | └─Token(TrueKeyword) |true|
//@[24:0025) |   ├─Token(Question) |?|
//@[26:0041) |   ├─ObjectSyntax
//@[26:0027) |   | ├─Token(LeftBrace) |{|
//@[27:0028) |   | ├─Token(NewLine) |\n|
    foo: 42
//@[04:0011) |   | ├─ObjectPropertySyntax
//@[04:0007) |   | | ├─IdentifierSyntax
//@[04:0007) |   | | | └─Token(Identifier) |foo|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0011) |   | | └─IntegerLiteralSyntax
//@[09:0011) |   | |   └─Token(Integer) |42|
//@[11:0012) |   | ├─Token(NewLine) |\n|
} : false
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[02:0003) |   ├─Token(Colon) |:|
//@[04:0009) |   └─BooleanLiteralSyntax
//@[04:0009) |     └─Token(FalseKeyword) |false|
//@[09:0010) ├─Token(NewLine) |\n|
var forceBreak15 = true ? { foo: 0 } : {
//@[00:0052) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |forceBreak15|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0052) | └─TernaryOperationSyntax
//@[19:0023) |   ├─BooleanLiteralSyntax
//@[19:0023) |   | └─Token(TrueKeyword) |true|
//@[24:0025) |   ├─Token(Question) |?|
//@[26:0036) |   ├─ObjectSyntax
//@[26:0027) |   | ├─Token(LeftBrace) |{|
//@[28:0034) |   | ├─ObjectPropertySyntax
//@[28:0031) |   | | ├─IdentifierSyntax
//@[28:0031) |   | | | └─Token(Identifier) |foo|
//@[31:0032) |   | | ├─Token(Colon) |:|
//@[33:0034) |   | | └─IntegerLiteralSyntax
//@[33:0034) |   | |   └─Token(Integer) |0|
//@[35:0036) |   | └─Token(RightBrace) |}|
//@[37:0038) |   ├─Token(Colon) |:|
//@[39:0052) |   └─ObjectSyntax
//@[39:0040) |     ├─Token(LeftBrace) |{|
//@[40:0041) |     ├─Token(NewLine) |\n|
    bar: 1}
//@[04:0010) |     ├─ObjectPropertySyntax
//@[04:0007) |     | ├─IdentifierSyntax
//@[04:0007) |     | | └─Token(Identifier) |bar|
//@[07:0008) |     | ├─Token(Colon) |:|
//@[09:0010) |     | └─IntegerLiteralSyntax
//@[09:0010) |     |   └─Token(Integer) |1|
//@[10:0011) |     └─Token(RightBrace) |}|
//@[11:0013) ├─Token(NewLine) |\n\n|


//@[00:0000) └─Token(EndOfFile) ||
