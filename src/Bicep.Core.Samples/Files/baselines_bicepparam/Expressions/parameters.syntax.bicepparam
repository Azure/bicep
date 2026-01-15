using 'main.bicep'
//@[00:2208) ProgramSyntax
//@[00:0018) ├─UsingDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |using|
//@[06:0018) | ├─StringSyntax
//@[06:0018) | | └─Token(StringComplete) |'main.bicep'|
//@[18:0018) | └─SkippedTriviaSyntax
//@[18:0020) ├─Token(NewLine) |\n\n|

param myObject = {
//@[00:1926) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |myObject|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:1926) | └─ObjectSyntax
//@[17:0018) |   ├─Token(LeftBrace) |{|
//@[18:0019) |   ├─Token(NewLine) |\n|
  any: any('foo')
//@[02:0017) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |any|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0017) |   | └─FunctionCallSyntax
//@[07:0010) |   |   ├─IdentifierSyntax
//@[07:0010) |   |   | └─Token(Identifier) |any|
//@[10:0011) |   |   ├─Token(LeftParen) |(|
//@[11:0016) |   |   ├─FunctionArgumentSyntax
//@[11:0016) |   |   | └─StringSyntax
//@[11:0016) |   |   |   └─Token(StringComplete) |'foo'|
//@[16:0017) |   |   └─Token(RightParen) |)|
//@[17:0018) |   ├─Token(NewLine) |\n|
  array: array([])
//@[02:0018) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |array|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0018) |   | └─FunctionCallSyntax
//@[09:0014) |   |   ├─IdentifierSyntax
//@[09:0014) |   |   | └─Token(Identifier) |array|
//@[14:0015) |   |   ├─Token(LeftParen) |(|
//@[15:0017) |   |   ├─FunctionArgumentSyntax
//@[15:0017) |   |   | └─ArraySyntax
//@[15:0016) |   |   |   ├─Token(LeftSquare) |[|
//@[16:0017) |   |   |   └─Token(RightSquare) |]|
//@[17:0018) |   |   └─Token(RightParen) |)|
//@[18:0019) |   ├─Token(NewLine) |\n|
  base64ToString: base64ToString(base64('abc'))
//@[02:0047) |   ├─ObjectPropertySyntax
//@[02:0016) |   | ├─IdentifierSyntax
//@[02:0016) |   | | └─Token(Identifier) |base64ToString|
//@[16:0017) |   | ├─Token(Colon) |:|
//@[18:0047) |   | └─FunctionCallSyntax
//@[18:0032) |   |   ├─IdentifierSyntax
//@[18:0032) |   |   | └─Token(Identifier) |base64ToString|
//@[32:0033) |   |   ├─Token(LeftParen) |(|
//@[33:0046) |   |   ├─FunctionArgumentSyntax
//@[33:0046) |   |   | └─FunctionCallSyntax
//@[33:0039) |   |   |   ├─IdentifierSyntax
//@[33:0039) |   |   |   | └─Token(Identifier) |base64|
//@[39:0040) |   |   |   ├─Token(LeftParen) |(|
//@[40:0045) |   |   |   ├─FunctionArgumentSyntax
//@[40:0045) |   |   |   | └─StringSyntax
//@[40:0045) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[45:0046) |   |   |   └─Token(RightParen) |)|
//@[46:0047) |   |   └─Token(RightParen) |)|
//@[47:0048) |   ├─Token(NewLine) |\n|
  base64ToJson: base64ToJson(base64('{"hi": "there"}')).hi
//@[02:0058) |   ├─ObjectPropertySyntax
//@[02:0014) |   | ├─IdentifierSyntax
//@[02:0014) |   | | └─Token(Identifier) |base64ToJson|
//@[14:0015) |   | ├─Token(Colon) |:|
//@[16:0058) |   | └─PropertyAccessSyntax
//@[16:0055) |   |   ├─FunctionCallSyntax
//@[16:0028) |   |   | ├─IdentifierSyntax
//@[16:0028) |   |   | | └─Token(Identifier) |base64ToJson|
//@[28:0029) |   |   | ├─Token(LeftParen) |(|
//@[29:0054) |   |   | ├─FunctionArgumentSyntax
//@[29:0054) |   |   | | └─FunctionCallSyntax
//@[29:0035) |   |   | |   ├─IdentifierSyntax
//@[29:0035) |   |   | |   | └─Token(Identifier) |base64|
//@[35:0036) |   |   | |   ├─Token(LeftParen) |(|
//@[36:0053) |   |   | |   ├─FunctionArgumentSyntax
//@[36:0053) |   |   | |   | └─StringSyntax
//@[36:0053) |   |   | |   |   └─Token(StringComplete) |'{"hi": "there"}'|
//@[53:0054) |   |   | |   └─Token(RightParen) |)|
//@[54:0055) |   |   | └─Token(RightParen) |)|
//@[55:0056) |   |   ├─Token(Dot) |.|
//@[56:0058) |   |   └─IdentifierSyntax
//@[56:0058) |   |     └─Token(Identifier) |hi|
//@[58:0059) |   ├─Token(NewLine) |\n|
  bool: bool(true)
//@[02:0018) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |bool|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0018) |   | └─FunctionCallSyntax
//@[08:0012) |   |   ├─IdentifierSyntax
//@[08:0012) |   |   | └─Token(Identifier) |bool|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0017) |   |   ├─FunctionArgumentSyntax
//@[13:0017) |   |   | └─BooleanLiteralSyntax
//@[13:0017) |   |   |   └─Token(TrueKeyword) |true|
//@[17:0018) |   |   └─Token(RightParen) |)|
//@[18:0019) |   ├─Token(NewLine) |\n|
  concat: concat(['abc'], ['def'])
//@[02:0034) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |concat|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0034) |   | └─FunctionCallSyntax
//@[10:0016) |   |   ├─IdentifierSyntax
//@[10:0016) |   |   | └─Token(Identifier) |concat|
//@[16:0017) |   |   ├─Token(LeftParen) |(|
//@[17:0024) |   |   ├─FunctionArgumentSyntax
//@[17:0024) |   |   | └─ArraySyntax
//@[17:0018) |   |   |   ├─Token(LeftSquare) |[|
//@[18:0023) |   |   |   ├─ArrayItemSyntax
//@[18:0023) |   |   |   | └─StringSyntax
//@[18:0023) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[23:0024) |   |   |   └─Token(RightSquare) |]|
//@[24:0025) |   |   ├─Token(Comma) |,|
//@[26:0033) |   |   ├─FunctionArgumentSyntax
//@[26:0033) |   |   | └─ArraySyntax
//@[26:0027) |   |   |   ├─Token(LeftSquare) |[|
//@[27:0032) |   |   |   ├─ArrayItemSyntax
//@[27:0032) |   |   |   | └─StringSyntax
//@[27:0032) |   |   |   |   └─Token(StringComplete) |'def'|
//@[32:0033) |   |   |   └─Token(RightSquare) |]|
//@[33:0034) |   |   └─Token(RightParen) |)|
//@[34:0035) |   ├─Token(NewLine) |\n|
  contains: contains('foo/bar', '/')
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |contains|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0036) |   | └─FunctionCallSyntax
//@[12:0020) |   |   ├─IdentifierSyntax
//@[12:0020) |   |   | └─Token(Identifier) |contains|
//@[20:0021) |   |   ├─Token(LeftParen) |(|
//@[21:0030) |   |   ├─FunctionArgumentSyntax
//@[21:0030) |   |   | └─StringSyntax
//@[21:0030) |   |   |   └─Token(StringComplete) |'foo/bar'|
//@[30:0031) |   |   ├─Token(Comma) |,|
//@[32:0035) |   |   ├─FunctionArgumentSyntax
//@[32:0035) |   |   | └─StringSyntax
//@[32:0035) |   |   |   └─Token(StringComplete) |'/'|
//@[35:0036) |   |   └─Token(RightParen) |)|
//@[36:0037) |   ├─Token(NewLine) |\n|
  dataUriToString: dataUriToString(dataUri('abc'))
//@[02:0050) |   ├─ObjectPropertySyntax
//@[02:0017) |   | ├─IdentifierSyntax
//@[02:0017) |   | | └─Token(Identifier) |dataUriToString|
//@[17:0018) |   | ├─Token(Colon) |:|
//@[19:0050) |   | └─FunctionCallSyntax
//@[19:0034) |   |   ├─IdentifierSyntax
//@[19:0034) |   |   | └─Token(Identifier) |dataUriToString|
//@[34:0035) |   |   ├─Token(LeftParen) |(|
//@[35:0049) |   |   ├─FunctionArgumentSyntax
//@[35:0049) |   |   | └─FunctionCallSyntax
//@[35:0042) |   |   |   ├─IdentifierSyntax
//@[35:0042) |   |   |   | └─Token(Identifier) |dataUri|
//@[42:0043) |   |   |   ├─Token(LeftParen) |(|
//@[43:0048) |   |   |   ├─FunctionArgumentSyntax
//@[43:0048) |   |   |   | └─StringSyntax
//@[43:0048) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[48:0049) |   |   |   └─Token(RightParen) |)|
//@[49:0050) |   |   └─Token(RightParen) |)|
//@[50:0051) |   ├─Token(NewLine) |\n|
  dateTimeAdd: dateTimeAdd(dateTimeFromEpoch(1680224438), 'P1D')  
//@[02:0064) |   ├─ObjectPropertySyntax
//@[02:0013) |   | ├─IdentifierSyntax
//@[02:0013) |   | | └─Token(Identifier) |dateTimeAdd|
//@[13:0014) |   | ├─Token(Colon) |:|
//@[15:0064) |   | └─FunctionCallSyntax
//@[15:0026) |   |   ├─IdentifierSyntax
//@[15:0026) |   |   | └─Token(Identifier) |dateTimeAdd|
//@[26:0027) |   |   ├─Token(LeftParen) |(|
//@[27:0056) |   |   ├─FunctionArgumentSyntax
//@[27:0056) |   |   | └─FunctionCallSyntax
//@[27:0044) |   |   |   ├─IdentifierSyntax
//@[27:0044) |   |   |   | └─Token(Identifier) |dateTimeFromEpoch|
//@[44:0045) |   |   |   ├─Token(LeftParen) |(|
//@[45:0055) |   |   |   ├─FunctionArgumentSyntax
//@[45:0055) |   |   |   | └─IntegerLiteralSyntax
//@[45:0055) |   |   |   |   └─Token(Integer) |1680224438|
//@[55:0056) |   |   |   └─Token(RightParen) |)|
//@[56:0057) |   |   ├─Token(Comma) |,|
//@[58:0063) |   |   ├─FunctionArgumentSyntax
//@[58:0063) |   |   | └─StringSyntax
//@[58:0063) |   |   |   └─Token(StringComplete) |'P1D'|
//@[63:0064) |   |   └─Token(RightParen) |)|
//@[66:0067) |   ├─Token(NewLine) |\n|
  dateTimeToEpoch: dateTimeToEpoch(dateTimeFromEpoch(1680224438))
//@[02:0065) |   ├─ObjectPropertySyntax
//@[02:0017) |   | ├─IdentifierSyntax
//@[02:0017) |   | | └─Token(Identifier) |dateTimeToEpoch|
//@[17:0018) |   | ├─Token(Colon) |:|
//@[19:0065) |   | └─FunctionCallSyntax
//@[19:0034) |   |   ├─IdentifierSyntax
//@[19:0034) |   |   | └─Token(Identifier) |dateTimeToEpoch|
//@[34:0035) |   |   ├─Token(LeftParen) |(|
//@[35:0064) |   |   ├─FunctionArgumentSyntax
//@[35:0064) |   |   | └─FunctionCallSyntax
//@[35:0052) |   |   |   ├─IdentifierSyntax
//@[35:0052) |   |   |   | └─Token(Identifier) |dateTimeFromEpoch|
//@[52:0053) |   |   |   ├─Token(LeftParen) |(|
//@[53:0063) |   |   |   ├─FunctionArgumentSyntax
//@[53:0063) |   |   |   | └─IntegerLiteralSyntax
//@[53:0063) |   |   |   |   └─Token(Integer) |1680224438|
//@[63:0064) |   |   |   └─Token(RightParen) |)|
//@[64:0065) |   |   └─Token(RightParen) |)|
//@[65:0066) |   ├─Token(NewLine) |\n|
  empty: empty([])
//@[02:0018) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |empty|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0018) |   | └─FunctionCallSyntax
//@[09:0014) |   |   ├─IdentifierSyntax
//@[09:0014) |   |   | └─Token(Identifier) |empty|
//@[14:0015) |   |   ├─Token(LeftParen) |(|
//@[15:0017) |   |   ├─FunctionArgumentSyntax
//@[15:0017) |   |   | └─ArraySyntax
//@[15:0016) |   |   |   ├─Token(LeftSquare) |[|
//@[16:0017) |   |   |   └─Token(RightSquare) |]|
//@[17:0018) |   |   └─Token(RightParen) |)|
//@[18:0019) |   ├─Token(NewLine) |\n|
  endsWith: endsWith('foo', 'o')
//@[02:0032) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |endsWith|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0032) |   | └─FunctionCallSyntax
//@[12:0020) |   |   ├─IdentifierSyntax
//@[12:0020) |   |   | └─Token(Identifier) |endsWith|
//@[20:0021) |   |   ├─Token(LeftParen) |(|
//@[21:0026) |   |   ├─FunctionArgumentSyntax
//@[21:0026) |   |   | └─StringSyntax
//@[21:0026) |   |   |   └─Token(StringComplete) |'foo'|
//@[26:0027) |   |   ├─Token(Comma) |,|
//@[28:0031) |   |   ├─FunctionArgumentSyntax
//@[28:0031) |   |   | └─StringSyntax
//@[28:0031) |   |   |   └─Token(StringComplete) |'o'|
//@[31:0032) |   |   └─Token(RightParen) |)|
//@[32:0033) |   ├─Token(NewLine) |\n|
  filter: filter([1, 2], i => i < 2)
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |filter|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0036) |   | └─FunctionCallSyntax
//@[10:0016) |   |   ├─IdentifierSyntax
//@[10:0016) |   |   | └─Token(Identifier) |filter|
//@[16:0017) |   |   ├─Token(LeftParen) |(|
//@[17:0023) |   |   ├─FunctionArgumentSyntax
//@[17:0023) |   |   | └─ArraySyntax
//@[17:0018) |   |   |   ├─Token(LeftSquare) |[|
//@[18:0019) |   |   |   ├─ArrayItemSyntax
//@[18:0019) |   |   |   | └─IntegerLiteralSyntax
//@[18:0019) |   |   |   |   └─Token(Integer) |1|
//@[19:0020) |   |   |   ├─Token(Comma) |,|
//@[21:0022) |   |   |   ├─ArrayItemSyntax
//@[21:0022) |   |   |   | └─IntegerLiteralSyntax
//@[21:0022) |   |   |   |   └─Token(Integer) |2|
//@[22:0023) |   |   |   └─Token(RightSquare) |]|
//@[23:0024) |   |   ├─Token(Comma) |,|
//@[25:0035) |   |   ├─FunctionArgumentSyntax
//@[25:0035) |   |   | └─LambdaSyntax
//@[25:0026) |   |   |   ├─LocalVariableSyntax
//@[25:0026) |   |   |   | └─IdentifierSyntax
//@[25:0026) |   |   |   |   └─Token(Identifier) |i|
//@[27:0029) |   |   |   ├─Token(Arrow) |=>|
//@[30:0035) |   |   |   └─BinaryOperationSyntax
//@[30:0031) |   |   |     ├─VariableAccessSyntax
//@[30:0031) |   |   |     | └─IdentifierSyntax
//@[30:0031) |   |   |     |   └─Token(Identifier) |i|
//@[32:0033) |   |   |     ├─Token(LeftChevron) |<|
//@[34:0035) |   |   |     └─IntegerLiteralSyntax
//@[34:0035) |   |   |       └─Token(Integer) |2|
//@[35:0036) |   |   └─Token(RightParen) |)|
//@[36:0037) |   ├─Token(NewLine) |\n|
  first: first([124, 25])
//@[02:0025) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |first|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0025) |   | └─FunctionCallSyntax
//@[09:0014) |   |   ├─IdentifierSyntax
//@[09:0014) |   |   | └─Token(Identifier) |first|
//@[14:0015) |   |   ├─Token(LeftParen) |(|
//@[15:0024) |   |   ├─FunctionArgumentSyntax
//@[15:0024) |   |   | └─ArraySyntax
//@[15:0016) |   |   |   ├─Token(LeftSquare) |[|
//@[16:0019) |   |   |   ├─ArrayItemSyntax
//@[16:0019) |   |   |   | └─IntegerLiteralSyntax
//@[16:0019) |   |   |   |   └─Token(Integer) |124|
//@[19:0020) |   |   |   ├─Token(Comma) |,|
//@[21:0023) |   |   |   ├─ArrayItemSyntax
//@[21:0023) |   |   |   | └─IntegerLiteralSyntax
//@[21:0023) |   |   |   |   └─Token(Integer) |25|
//@[23:0024) |   |   |   └─Token(RightSquare) |]|
//@[24:0025) |   |   └─Token(RightParen) |)|
//@[25:0026) |   ├─Token(NewLine) |\n|
  flatten: flatten([['abc'], ['def']])
//@[02:0038) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |flatten|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0038) |   | └─FunctionCallSyntax
//@[11:0018) |   |   ├─IdentifierSyntax
//@[11:0018) |   |   | └─Token(Identifier) |flatten|
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0037) |   |   ├─FunctionArgumentSyntax
//@[19:0037) |   |   | └─ArraySyntax
//@[19:0020) |   |   |   ├─Token(LeftSquare) |[|
//@[20:0027) |   |   |   ├─ArrayItemSyntax
//@[20:0027) |   |   |   | └─ArraySyntax
//@[20:0021) |   |   |   |   ├─Token(LeftSquare) |[|
//@[21:0026) |   |   |   |   ├─ArrayItemSyntax
//@[21:0026) |   |   |   |   | └─StringSyntax
//@[21:0026) |   |   |   |   |   └─Token(StringComplete) |'abc'|
//@[26:0027) |   |   |   |   └─Token(RightSquare) |]|
//@[27:0028) |   |   |   ├─Token(Comma) |,|
//@[29:0036) |   |   |   ├─ArrayItemSyntax
//@[29:0036) |   |   |   | └─ArraySyntax
//@[29:0030) |   |   |   |   ├─Token(LeftSquare) |[|
//@[30:0035) |   |   |   |   ├─ArrayItemSyntax
//@[30:0035) |   |   |   |   | └─StringSyntax
//@[30:0035) |   |   |   |   |   └─Token(StringComplete) |'def'|
//@[35:0036) |   |   |   |   └─Token(RightSquare) |]|
//@[36:0037) |   |   |   └─Token(RightSquare) |]|
//@[37:0038) |   |   └─Token(RightParen) |)|
//@[38:0039) |   ├─Token(NewLine) |\n|
  format: format('->{0}<-', 123)
//@[02:0032) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |format|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0032) |   | └─FunctionCallSyntax
//@[10:0016) |   |   ├─IdentifierSyntax
//@[10:0016) |   |   | └─Token(Identifier) |format|
//@[16:0017) |   |   ├─Token(LeftParen) |(|
//@[17:0026) |   |   ├─FunctionArgumentSyntax
//@[17:0026) |   |   | └─StringSyntax
//@[17:0026) |   |   |   └─Token(StringComplete) |'->{0}<-'|
//@[26:0027) |   |   ├─Token(Comma) |,|
//@[28:0031) |   |   ├─FunctionArgumentSyntax
//@[28:0031) |   |   | └─IntegerLiteralSyntax
//@[28:0031) |   |   |   └─Token(Integer) |123|
//@[31:0032) |   |   └─Token(RightParen) |)|
//@[32:0033) |   ├─Token(NewLine) |\n|
  guid: guid('asdf')
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |guid|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0020) |   | └─FunctionCallSyntax
//@[08:0012) |   |   ├─IdentifierSyntax
//@[08:0012) |   |   | └─Token(Identifier) |guid|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0019) |   |   ├─FunctionArgumentSyntax
//@[13:0019) |   |   | └─StringSyntax
//@[13:0019) |   |   |   └─Token(StringComplete) |'asdf'|
//@[19:0020) |   |   └─Token(RightParen) |)|
//@[20:0021) |   ├─Token(NewLine) |\n|
  indexOf: indexOf('abc', 'b')
//@[02:0030) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |indexOf|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0030) |   | └─FunctionCallSyntax
//@[11:0018) |   |   ├─IdentifierSyntax
//@[11:0018) |   |   | └─Token(Identifier) |indexOf|
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0024) |   |   ├─FunctionArgumentSyntax
//@[19:0024) |   |   | └─StringSyntax
//@[19:0024) |   |   |   └─Token(StringComplete) |'abc'|
//@[24:0025) |   |   ├─Token(Comma) |,|
//@[26:0029) |   |   ├─FunctionArgumentSyntax
//@[26:0029) |   |   | └─StringSyntax
//@[26:0029) |   |   |   └─Token(StringComplete) |'b'|
//@[29:0030) |   |   └─Token(RightParen) |)|
//@[30:0031) |   ├─Token(NewLine) |\n|
  int: int(123)
//@[02:0015) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |int|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0015) |   | └─FunctionCallSyntax
//@[07:0010) |   |   ├─IdentifierSyntax
//@[07:0010) |   |   | └─Token(Identifier) |int|
//@[10:0011) |   |   ├─Token(LeftParen) |(|
//@[11:0014) |   |   ├─FunctionArgumentSyntax
//@[11:0014) |   |   | └─IntegerLiteralSyntax
//@[11:0014) |   |   |   └─Token(Integer) |123|
//@[14:0015) |   |   └─Token(RightParen) |)|
//@[15:0016) |   ├─Token(NewLine) |\n|
  intersection: intersection([1, 2, 3], [2, 3, 4])
//@[02:0050) |   ├─ObjectPropertySyntax
//@[02:0014) |   | ├─IdentifierSyntax
//@[02:0014) |   | | └─Token(Identifier) |intersection|
//@[14:0015) |   | ├─Token(Colon) |:|
//@[16:0050) |   | └─FunctionCallSyntax
//@[16:0028) |   |   ├─IdentifierSyntax
//@[16:0028) |   |   | └─Token(Identifier) |intersection|
//@[28:0029) |   |   ├─Token(LeftParen) |(|
//@[29:0038) |   |   ├─FunctionArgumentSyntax
//@[29:0038) |   |   | └─ArraySyntax
//@[29:0030) |   |   |   ├─Token(LeftSquare) |[|
//@[30:0031) |   |   |   ├─ArrayItemSyntax
//@[30:0031) |   |   |   | └─IntegerLiteralSyntax
//@[30:0031) |   |   |   |   └─Token(Integer) |1|
//@[31:0032) |   |   |   ├─Token(Comma) |,|
//@[33:0034) |   |   |   ├─ArrayItemSyntax
//@[33:0034) |   |   |   | └─IntegerLiteralSyntax
//@[33:0034) |   |   |   |   └─Token(Integer) |2|
//@[34:0035) |   |   |   ├─Token(Comma) |,|
//@[36:0037) |   |   |   ├─ArrayItemSyntax
//@[36:0037) |   |   |   | └─IntegerLiteralSyntax
//@[36:0037) |   |   |   |   └─Token(Integer) |3|
//@[37:0038) |   |   |   └─Token(RightSquare) |]|
//@[38:0039) |   |   ├─Token(Comma) |,|
//@[40:0049) |   |   ├─FunctionArgumentSyntax
//@[40:0049) |   |   | └─ArraySyntax
//@[40:0041) |   |   |   ├─Token(LeftSquare) |[|
//@[41:0042) |   |   |   ├─ArrayItemSyntax
//@[41:0042) |   |   |   | └─IntegerLiteralSyntax
//@[41:0042) |   |   |   |   └─Token(Integer) |2|
//@[42:0043) |   |   |   ├─Token(Comma) |,|
//@[44:0045) |   |   |   ├─ArrayItemSyntax
//@[44:0045) |   |   |   | └─IntegerLiteralSyntax
//@[44:0045) |   |   |   |   └─Token(Integer) |3|
//@[45:0046) |   |   |   ├─Token(Comma) |,|
//@[47:0048) |   |   |   ├─ArrayItemSyntax
//@[47:0048) |   |   |   | └─IntegerLiteralSyntax
//@[47:0048) |   |   |   |   └─Token(Integer) |4|
//@[48:0049) |   |   |   └─Token(RightSquare) |]|
//@[49:0050) |   |   └─Token(RightParen) |)|
//@[50:0051) |   ├─Token(NewLine) |\n|
  items: items({ foo: 'abc', bar: 123 })
//@[02:0040) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |items|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0040) |   | └─FunctionCallSyntax
//@[09:0014) |   |   ├─IdentifierSyntax
//@[09:0014) |   |   | └─Token(Identifier) |items|
//@[14:0015) |   |   ├─Token(LeftParen) |(|
//@[15:0039) |   |   ├─FunctionArgumentSyntax
//@[15:0039) |   |   | └─ObjectSyntax
//@[15:0016) |   |   |   ├─Token(LeftBrace) |{|
//@[17:0027) |   |   |   ├─ObjectPropertySyntax
//@[17:0020) |   |   |   | ├─IdentifierSyntax
//@[17:0020) |   |   |   | | └─Token(Identifier) |foo|
//@[20:0021) |   |   |   | ├─Token(Colon) |:|
//@[22:0027) |   |   |   | └─StringSyntax
//@[22:0027) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[27:0028) |   |   |   ├─Token(Comma) |,|
//@[29:0037) |   |   |   ├─ObjectPropertySyntax
//@[29:0032) |   |   |   | ├─IdentifierSyntax
//@[29:0032) |   |   |   | | └─Token(Identifier) |bar|
//@[32:0033) |   |   |   | ├─Token(Colon) |:|
//@[34:0037) |   |   |   | └─IntegerLiteralSyntax
//@[34:0037) |   |   |   |   └─Token(Integer) |123|
//@[38:0039) |   |   |   └─Token(RightBrace) |}|
//@[39:0040) |   |   └─Token(RightParen) |)|
//@[40:0041) |   ├─Token(NewLine) |\n|
  join: join(['abc', 'def', 'ghi'], '/')
//@[02:0040) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |join|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0040) |   | └─FunctionCallSyntax
//@[08:0012) |   |   ├─IdentifierSyntax
//@[08:0012) |   |   | └─Token(Identifier) |join|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0034) |   |   ├─FunctionArgumentSyntax
//@[13:0034) |   |   | └─ArraySyntax
//@[13:0014) |   |   |   ├─Token(LeftSquare) |[|
//@[14:0019) |   |   |   ├─ArrayItemSyntax
//@[14:0019) |   |   |   | └─StringSyntax
//@[14:0019) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[19:0020) |   |   |   ├─Token(Comma) |,|
//@[21:0026) |   |   |   ├─ArrayItemSyntax
//@[21:0026) |   |   |   | └─StringSyntax
//@[21:0026) |   |   |   |   └─Token(StringComplete) |'def'|
//@[26:0027) |   |   |   ├─Token(Comma) |,|
//@[28:0033) |   |   |   ├─ArrayItemSyntax
//@[28:0033) |   |   |   | └─StringSyntax
//@[28:0033) |   |   |   |   └─Token(StringComplete) |'ghi'|
//@[33:0034) |   |   |   └─Token(RightSquare) |]|
//@[34:0035) |   |   ├─Token(Comma) |,|
//@[36:0039) |   |   ├─FunctionArgumentSyntax
//@[36:0039) |   |   | └─StringSyntax
//@[36:0039) |   |   |   └─Token(StringComplete) |'/'|
//@[39:0040) |   |   └─Token(RightParen) |)|
//@[40:0041) |   ├─Token(NewLine) |\n|
  last: last([1, 2])
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |last|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0020) |   | └─FunctionCallSyntax
//@[08:0012) |   |   ├─IdentifierSyntax
//@[08:0012) |   |   | └─Token(Identifier) |last|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0019) |   |   ├─FunctionArgumentSyntax
//@[13:0019) |   |   | └─ArraySyntax
//@[13:0014) |   |   |   ├─Token(LeftSquare) |[|
//@[14:0015) |   |   |   ├─ArrayItemSyntax
//@[14:0015) |   |   |   | └─IntegerLiteralSyntax
//@[14:0015) |   |   |   |   └─Token(Integer) |1|
//@[15:0016) |   |   |   ├─Token(Comma) |,|
//@[17:0018) |   |   |   ├─ArrayItemSyntax
//@[17:0018) |   |   |   | └─IntegerLiteralSyntax
//@[17:0018) |   |   |   |   └─Token(Integer) |2|
//@[18:0019) |   |   |   └─Token(RightSquare) |]|
//@[19:0020) |   |   └─Token(RightParen) |)|
//@[20:0021) |   ├─Token(NewLine) |\n|
  lastIndexOf: lastIndexOf('abcba', 'b')
//@[02:0040) |   ├─ObjectPropertySyntax
//@[02:0013) |   | ├─IdentifierSyntax
//@[02:0013) |   | | └─Token(Identifier) |lastIndexOf|
//@[13:0014) |   | ├─Token(Colon) |:|
//@[15:0040) |   | └─FunctionCallSyntax
//@[15:0026) |   |   ├─IdentifierSyntax
//@[15:0026) |   |   | └─Token(Identifier) |lastIndexOf|
//@[26:0027) |   |   ├─Token(LeftParen) |(|
//@[27:0034) |   |   ├─FunctionArgumentSyntax
//@[27:0034) |   |   | └─StringSyntax
//@[27:0034) |   |   |   └─Token(StringComplete) |'abcba'|
//@[34:0035) |   |   ├─Token(Comma) |,|
//@[36:0039) |   |   ├─FunctionArgumentSyntax
//@[36:0039) |   |   | └─StringSyntax
//@[36:0039) |   |   |   └─Token(StringComplete) |'b'|
//@[39:0040) |   |   └─Token(RightParen) |)|
//@[40:0041) |   ├─Token(NewLine) |\n|
  length: length([0, 1, 2])
//@[02:0027) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |length|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0027) |   | └─FunctionCallSyntax
//@[10:0016) |   |   ├─IdentifierSyntax
//@[10:0016) |   |   | └─Token(Identifier) |length|
//@[16:0017) |   |   ├─Token(LeftParen) |(|
//@[17:0026) |   |   ├─FunctionArgumentSyntax
//@[17:0026) |   |   | └─ArraySyntax
//@[17:0018) |   |   |   ├─Token(LeftSquare) |[|
//@[18:0019) |   |   |   ├─ArrayItemSyntax
//@[18:0019) |   |   |   | └─IntegerLiteralSyntax
//@[18:0019) |   |   |   |   └─Token(Integer) |0|
//@[19:0020) |   |   |   ├─Token(Comma) |,|
//@[21:0022) |   |   |   ├─ArrayItemSyntax
//@[21:0022) |   |   |   | └─IntegerLiteralSyntax
//@[21:0022) |   |   |   |   └─Token(Integer) |1|
//@[22:0023) |   |   |   ├─Token(Comma) |,|
//@[24:0025) |   |   |   ├─ArrayItemSyntax
//@[24:0025) |   |   |   | └─IntegerLiteralSyntax
//@[24:0025) |   |   |   |   └─Token(Integer) |2|
//@[25:0026) |   |   |   └─Token(RightSquare) |]|
//@[26:0027) |   |   └─Token(RightParen) |)|
//@[27:0028) |   ├─Token(NewLine) |\n|
  loadFileAsBase64: loadFileAsBase64('test.txt')
//@[02:0048) |   ├─ObjectPropertySyntax
//@[02:0018) |   | ├─IdentifierSyntax
//@[02:0018) |   | | └─Token(Identifier) |loadFileAsBase64|
//@[18:0019) |   | ├─Token(Colon) |:|
//@[20:0048) |   | └─FunctionCallSyntax
//@[20:0036) |   |   ├─IdentifierSyntax
//@[20:0036) |   |   | └─Token(Identifier) |loadFileAsBase64|
//@[36:0037) |   |   ├─Token(LeftParen) |(|
//@[37:0047) |   |   ├─FunctionArgumentSyntax
//@[37:0047) |   |   | └─StringSyntax
//@[37:0047) |   |   |   └─Token(StringComplete) |'test.txt'|
//@[47:0048) |   |   └─Token(RightParen) |)|
//@[48:0049) |   ├─Token(NewLine) |\n|
  loadJsonContent: loadJsonContent('test.json')
//@[02:0047) |   ├─ObjectPropertySyntax
//@[02:0017) |   | ├─IdentifierSyntax
//@[02:0017) |   | | └─Token(Identifier) |loadJsonContent|
//@[17:0018) |   | ├─Token(Colon) |:|
//@[19:0047) |   | └─FunctionCallSyntax
//@[19:0034) |   |   ├─IdentifierSyntax
//@[19:0034) |   |   | └─Token(Identifier) |loadJsonContent|
//@[34:0035) |   |   ├─Token(LeftParen) |(|
//@[35:0046) |   |   ├─FunctionArgumentSyntax
//@[35:0046) |   |   | └─StringSyntax
//@[35:0046) |   |   |   └─Token(StringComplete) |'test.json'|
//@[46:0047) |   |   └─Token(RightParen) |)|
//@[47:0048) |   ├─Token(NewLine) |\n|
  loadTextContent: loadTextContent('test.txt')
//@[02:0046) |   ├─ObjectPropertySyntax
//@[02:0017) |   | ├─IdentifierSyntax
//@[02:0017) |   | | └─Token(Identifier) |loadTextContent|
//@[17:0018) |   | ├─Token(Colon) |:|
//@[19:0046) |   | └─FunctionCallSyntax
//@[19:0034) |   |   ├─IdentifierSyntax
//@[19:0034) |   |   | └─Token(Identifier) |loadTextContent|
//@[34:0035) |   |   ├─Token(LeftParen) |(|
//@[35:0045) |   |   ├─FunctionArgumentSyntax
//@[35:0045) |   |   | └─StringSyntax
//@[35:0045) |   |   |   └─Token(StringComplete) |'test.txt'|
//@[45:0046) |   |   └─Token(RightParen) |)|
//@[46:0047) |   ├─Token(NewLine) |\n|
  map: map(range(0, 3), i => 'Hi ${i}!')
//@[02:0040) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |map|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0040) |   | └─FunctionCallSyntax
//@[07:0010) |   |   ├─IdentifierSyntax
//@[07:0010) |   |   | └─Token(Identifier) |map|
//@[10:0011) |   |   ├─Token(LeftParen) |(|
//@[11:0022) |   |   ├─FunctionArgumentSyntax
//@[11:0022) |   |   | └─FunctionCallSyntax
//@[11:0016) |   |   |   ├─IdentifierSyntax
//@[11:0016) |   |   |   | └─Token(Identifier) |range|
//@[16:0017) |   |   |   ├─Token(LeftParen) |(|
//@[17:0018) |   |   |   ├─FunctionArgumentSyntax
//@[17:0018) |   |   |   | └─IntegerLiteralSyntax
//@[17:0018) |   |   |   |   └─Token(Integer) |0|
//@[18:0019) |   |   |   ├─Token(Comma) |,|
//@[20:0021) |   |   |   ├─FunctionArgumentSyntax
//@[20:0021) |   |   |   | └─IntegerLiteralSyntax
//@[20:0021) |   |   |   |   └─Token(Integer) |3|
//@[21:0022) |   |   |   └─Token(RightParen) |)|
//@[22:0023) |   |   ├─Token(Comma) |,|
//@[24:0039) |   |   ├─FunctionArgumentSyntax
//@[24:0039) |   |   | └─LambdaSyntax
//@[24:0025) |   |   |   ├─LocalVariableSyntax
//@[24:0025) |   |   |   | └─IdentifierSyntax
//@[24:0025) |   |   |   |   └─Token(Identifier) |i|
//@[26:0028) |   |   |   ├─Token(Arrow) |=>|
//@[29:0039) |   |   |   └─StringSyntax
//@[29:0035) |   |   |     ├─Token(StringLeftPiece) |'Hi ${|
//@[35:0036) |   |   |     ├─VariableAccessSyntax
//@[35:0036) |   |   |     | └─IdentifierSyntax
//@[35:0036) |   |   |     |   └─Token(Identifier) |i|
//@[36:0039) |   |   |     └─Token(StringRightPiece) |}!'|
//@[39:0040) |   |   └─Token(RightParen) |)|
//@[40:0041) |   ├─Token(NewLine) |\n|
  max: max(1, 2, 3)
//@[02:0019) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |max|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0019) |   | └─FunctionCallSyntax
//@[07:0010) |   |   ├─IdentifierSyntax
//@[07:0010) |   |   | └─Token(Identifier) |max|
//@[10:0011) |   |   ├─Token(LeftParen) |(|
//@[11:0012) |   |   ├─FunctionArgumentSyntax
//@[11:0012) |   |   | └─IntegerLiteralSyntax
//@[11:0012) |   |   |   └─Token(Integer) |1|
//@[12:0013) |   |   ├─Token(Comma) |,|
//@[14:0015) |   |   ├─FunctionArgumentSyntax
//@[14:0015) |   |   | └─IntegerLiteralSyntax
//@[14:0015) |   |   |   └─Token(Integer) |2|
//@[15:0016) |   |   ├─Token(Comma) |,|
//@[17:0018) |   |   ├─FunctionArgumentSyntax
//@[17:0018) |   |   | └─IntegerLiteralSyntax
//@[17:0018) |   |   |   └─Token(Integer) |3|
//@[18:0019) |   |   └─Token(RightParen) |)|
//@[19:0020) |   ├─Token(NewLine) |\n|
  min: min(1, 2, 3)
//@[02:0019) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |min|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0019) |   | └─FunctionCallSyntax
//@[07:0010) |   |   ├─IdentifierSyntax
//@[07:0010) |   |   | └─Token(Identifier) |min|
//@[10:0011) |   |   ├─Token(LeftParen) |(|
//@[11:0012) |   |   ├─FunctionArgumentSyntax
//@[11:0012) |   |   | └─IntegerLiteralSyntax
//@[11:0012) |   |   |   └─Token(Integer) |1|
//@[12:0013) |   |   ├─Token(Comma) |,|
//@[14:0015) |   |   ├─FunctionArgumentSyntax
//@[14:0015) |   |   | └─IntegerLiteralSyntax
//@[14:0015) |   |   |   └─Token(Integer) |2|
//@[15:0016) |   |   ├─Token(Comma) |,|
//@[17:0018) |   |   ├─FunctionArgumentSyntax
//@[17:0018) |   |   | └─IntegerLiteralSyntax
//@[17:0018) |   |   |   └─Token(Integer) |3|
//@[18:0019) |   |   └─Token(RightParen) |)|
//@[19:0020) |   ├─Token(NewLine) |\n|
  padLeft: padLeft(13, 5)
//@[02:0025) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |padLeft|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0025) |   | └─FunctionCallSyntax
//@[11:0018) |   |   ├─IdentifierSyntax
//@[11:0018) |   |   | └─Token(Identifier) |padLeft|
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0021) |   |   ├─FunctionArgumentSyntax
//@[19:0021) |   |   | └─IntegerLiteralSyntax
//@[19:0021) |   |   |   └─Token(Integer) |13|
//@[21:0022) |   |   ├─Token(Comma) |,|
//@[23:0024) |   |   ├─FunctionArgumentSyntax
//@[23:0024) |   |   | └─IntegerLiteralSyntax
//@[23:0024) |   |   |   └─Token(Integer) |5|
//@[24:0025) |   |   └─Token(RightParen) |)|
//@[25:0026) |   ├─Token(NewLine) |\n|
  range: range(0, 3)
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |range|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0020) |   | └─FunctionCallSyntax
//@[09:0014) |   |   ├─IdentifierSyntax
//@[09:0014) |   |   | └─Token(Identifier) |range|
//@[14:0015) |   |   ├─Token(LeftParen) |(|
//@[15:0016) |   |   ├─FunctionArgumentSyntax
//@[15:0016) |   |   | └─IntegerLiteralSyntax
//@[15:0016) |   |   |   └─Token(Integer) |0|
//@[16:0017) |   |   ├─Token(Comma) |,|
//@[18:0019) |   |   ├─FunctionArgumentSyntax
//@[18:0019) |   |   | └─IntegerLiteralSyntax
//@[18:0019) |   |   |   └─Token(Integer) |3|
//@[19:0020) |   |   └─Token(RightParen) |)|
//@[20:0021) |   ├─Token(NewLine) |\n|
  reduce: reduce(['a', 'b', 'c'], '', (a, b) => '${a}-${b}')
//@[02:0060) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |reduce|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0060) |   | └─FunctionCallSyntax
//@[10:0016) |   |   ├─IdentifierSyntax
//@[10:0016) |   |   | └─Token(Identifier) |reduce|
//@[16:0017) |   |   ├─Token(LeftParen) |(|
//@[17:0032) |   |   ├─FunctionArgumentSyntax
//@[17:0032) |   |   | └─ArraySyntax
//@[17:0018) |   |   |   ├─Token(LeftSquare) |[|
//@[18:0021) |   |   |   ├─ArrayItemSyntax
//@[18:0021) |   |   |   | └─StringSyntax
//@[18:0021) |   |   |   |   └─Token(StringComplete) |'a'|
//@[21:0022) |   |   |   ├─Token(Comma) |,|
//@[23:0026) |   |   |   ├─ArrayItemSyntax
//@[23:0026) |   |   |   | └─StringSyntax
//@[23:0026) |   |   |   |   └─Token(StringComplete) |'b'|
//@[26:0027) |   |   |   ├─Token(Comma) |,|
//@[28:0031) |   |   |   ├─ArrayItemSyntax
//@[28:0031) |   |   |   | └─StringSyntax
//@[28:0031) |   |   |   |   └─Token(StringComplete) |'c'|
//@[31:0032) |   |   |   └─Token(RightSquare) |]|
//@[32:0033) |   |   ├─Token(Comma) |,|
//@[34:0036) |   |   ├─FunctionArgumentSyntax
//@[34:0036) |   |   | └─StringSyntax
//@[34:0036) |   |   |   └─Token(StringComplete) |''|
//@[36:0037) |   |   ├─Token(Comma) |,|
//@[38:0059) |   |   ├─FunctionArgumentSyntax
//@[38:0059) |   |   | └─LambdaSyntax
//@[38:0044) |   |   |   ├─VariableBlockSyntax
//@[38:0039) |   |   |   | ├─Token(LeftParen) |(|
//@[39:0040) |   |   |   | ├─LocalVariableSyntax
//@[39:0040) |   |   |   | | └─IdentifierSyntax
//@[39:0040) |   |   |   | |   └─Token(Identifier) |a|
//@[40:0041) |   |   |   | ├─Token(Comma) |,|
//@[42:0043) |   |   |   | ├─LocalVariableSyntax
//@[42:0043) |   |   |   | | └─IdentifierSyntax
//@[42:0043) |   |   |   | |   └─Token(Identifier) |b|
//@[43:0044) |   |   |   | └─Token(RightParen) |)|
//@[45:0047) |   |   |   ├─Token(Arrow) |=>|
//@[48:0059) |   |   |   └─StringSyntax
//@[48:0051) |   |   |     ├─Token(StringLeftPiece) |'${|
//@[51:0052) |   |   |     ├─VariableAccessSyntax
//@[51:0052) |   |   |     | └─IdentifierSyntax
//@[51:0052) |   |   |     |   └─Token(Identifier) |a|
//@[52:0056) |   |   |     ├─Token(StringMiddlePiece) |}-${|
//@[56:0057) |   |   |     ├─VariableAccessSyntax
//@[56:0057) |   |   |     | └─IdentifierSyntax
//@[56:0057) |   |   |     |   └─Token(Identifier) |b|
//@[57:0059) |   |   |     └─Token(StringRightPiece) |}'|
//@[59:0060) |   |   └─Token(RightParen) |)|
//@[60:0061) |   ├─Token(NewLine) |\n|
  replace: replace('abc', 'b', '/')
//@[02:0035) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |replace|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0035) |   | └─FunctionCallSyntax
//@[11:0018) |   |   ├─IdentifierSyntax
//@[11:0018) |   |   | └─Token(Identifier) |replace|
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0024) |   |   ├─FunctionArgumentSyntax
//@[19:0024) |   |   | └─StringSyntax
//@[19:0024) |   |   |   └─Token(StringComplete) |'abc'|
//@[24:0025) |   |   ├─Token(Comma) |,|
//@[26:0029) |   |   ├─FunctionArgumentSyntax
//@[26:0029) |   |   | └─StringSyntax
//@[26:0029) |   |   |   └─Token(StringComplete) |'b'|
//@[29:0030) |   |   ├─Token(Comma) |,|
//@[31:0034) |   |   ├─FunctionArgumentSyntax
//@[31:0034) |   |   | └─StringSyntax
//@[31:0034) |   |   |   └─Token(StringComplete) |'/'|
//@[34:0035) |   |   └─Token(RightParen) |)|
//@[35:0036) |   ├─Token(NewLine) |\n|
  skip: skip([1, 2, 3], 1)
//@[02:0026) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |skip|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0026) |   | └─FunctionCallSyntax
//@[08:0012) |   |   ├─IdentifierSyntax
//@[08:0012) |   |   | └─Token(Identifier) |skip|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0022) |   |   ├─FunctionArgumentSyntax
//@[13:0022) |   |   | └─ArraySyntax
//@[13:0014) |   |   |   ├─Token(LeftSquare) |[|
//@[14:0015) |   |   |   ├─ArrayItemSyntax
//@[14:0015) |   |   |   | └─IntegerLiteralSyntax
//@[14:0015) |   |   |   |   └─Token(Integer) |1|
//@[15:0016) |   |   |   ├─Token(Comma) |,|
//@[17:0018) |   |   |   ├─ArrayItemSyntax
//@[17:0018) |   |   |   | └─IntegerLiteralSyntax
//@[17:0018) |   |   |   |   └─Token(Integer) |2|
//@[18:0019) |   |   |   ├─Token(Comma) |,|
//@[20:0021) |   |   |   ├─ArrayItemSyntax
//@[20:0021) |   |   |   | └─IntegerLiteralSyntax
//@[20:0021) |   |   |   |   └─Token(Integer) |3|
//@[21:0022) |   |   |   └─Token(RightSquare) |]|
//@[22:0023) |   |   ├─Token(Comma) |,|
//@[24:0025) |   |   ├─FunctionArgumentSyntax
//@[24:0025) |   |   | └─IntegerLiteralSyntax
//@[24:0025) |   |   |   └─Token(Integer) |1|
//@[25:0026) |   |   └─Token(RightParen) |)|
//@[26:0027) |   ├─Token(NewLine) |\n|
  sort: sort(['c', 'd', 'a'], (a, b) => a < b)
//@[02:0046) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |sort|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0046) |   | └─FunctionCallSyntax
//@[08:0012) |   |   ├─IdentifierSyntax
//@[08:0012) |   |   | └─Token(Identifier) |sort|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0028) |   |   ├─FunctionArgumentSyntax
//@[13:0028) |   |   | └─ArraySyntax
//@[13:0014) |   |   |   ├─Token(LeftSquare) |[|
//@[14:0017) |   |   |   ├─ArrayItemSyntax
//@[14:0017) |   |   |   | └─StringSyntax
//@[14:0017) |   |   |   |   └─Token(StringComplete) |'c'|
//@[17:0018) |   |   |   ├─Token(Comma) |,|
//@[19:0022) |   |   |   ├─ArrayItemSyntax
//@[19:0022) |   |   |   | └─StringSyntax
//@[19:0022) |   |   |   |   └─Token(StringComplete) |'d'|
//@[22:0023) |   |   |   ├─Token(Comma) |,|
//@[24:0027) |   |   |   ├─ArrayItemSyntax
//@[24:0027) |   |   |   | └─StringSyntax
//@[24:0027) |   |   |   |   └─Token(StringComplete) |'a'|
//@[27:0028) |   |   |   └─Token(RightSquare) |]|
//@[28:0029) |   |   ├─Token(Comma) |,|
//@[30:0045) |   |   ├─FunctionArgumentSyntax
//@[30:0045) |   |   | └─LambdaSyntax
//@[30:0036) |   |   |   ├─VariableBlockSyntax
//@[30:0031) |   |   |   | ├─Token(LeftParen) |(|
//@[31:0032) |   |   |   | ├─LocalVariableSyntax
//@[31:0032) |   |   |   | | └─IdentifierSyntax
//@[31:0032) |   |   |   | |   └─Token(Identifier) |a|
//@[32:0033) |   |   |   | ├─Token(Comma) |,|
//@[34:0035) |   |   |   | ├─LocalVariableSyntax
//@[34:0035) |   |   |   | | └─IdentifierSyntax
//@[34:0035) |   |   |   | |   └─Token(Identifier) |b|
//@[35:0036) |   |   |   | └─Token(RightParen) |)|
//@[37:0039) |   |   |   ├─Token(Arrow) |=>|
//@[40:0045) |   |   |   └─BinaryOperationSyntax
//@[40:0041) |   |   |     ├─VariableAccessSyntax
//@[40:0041) |   |   |     | └─IdentifierSyntax
//@[40:0041) |   |   |     |   └─Token(Identifier) |a|
//@[42:0043) |   |   |     ├─Token(LeftChevron) |<|
//@[44:0045) |   |   |     └─VariableAccessSyntax
//@[44:0045) |   |   |       └─IdentifierSyntax
//@[44:0045) |   |   |         └─Token(Identifier) |b|
//@[45:0046) |   |   └─Token(RightParen) |)|
//@[46:0047) |   ├─Token(NewLine) |\n|
  split: split('a/b/c', '/')
//@[02:0028) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |split|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0028) |   | └─FunctionCallSyntax
//@[09:0014) |   |   ├─IdentifierSyntax
//@[09:0014) |   |   | └─Token(Identifier) |split|
//@[14:0015) |   |   ├─Token(LeftParen) |(|
//@[15:0022) |   |   ├─FunctionArgumentSyntax
//@[15:0022) |   |   | └─StringSyntax
//@[15:0022) |   |   |   └─Token(StringComplete) |'a/b/c'|
//@[22:0023) |   |   ├─Token(Comma) |,|
//@[24:0027) |   |   ├─FunctionArgumentSyntax
//@[24:0027) |   |   | └─StringSyntax
//@[24:0027) |   |   |   └─Token(StringComplete) |'/'|
//@[27:0028) |   |   └─Token(RightParen) |)|
//@[28:0029) |   ├─Token(NewLine) |\n|
  startsWith: startsWith('abc', 'a')
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |startsWith|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0036) |   | └─FunctionCallSyntax
//@[14:0024) |   |   ├─IdentifierSyntax
//@[14:0024) |   |   | └─Token(Identifier) |startsWith|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0030) |   |   ├─FunctionArgumentSyntax
//@[25:0030) |   |   | └─StringSyntax
//@[25:0030) |   |   |   └─Token(StringComplete) |'abc'|
//@[30:0031) |   |   ├─Token(Comma) |,|
//@[32:0035) |   |   ├─FunctionArgumentSyntax
//@[32:0035) |   |   | └─StringSyntax
//@[32:0035) |   |   |   └─Token(StringComplete) |'a'|
//@[35:0036) |   |   └─Token(RightParen) |)|
//@[36:0037) |   ├─Token(NewLine) |\n|
  string: string('asdf')
//@[02:0024) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |string|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0024) |   | └─FunctionCallSyntax
//@[10:0016) |   |   ├─IdentifierSyntax
//@[10:0016) |   |   | └─Token(Identifier) |string|
//@[16:0017) |   |   ├─Token(LeftParen) |(|
//@[17:0023) |   |   ├─FunctionArgumentSyntax
//@[17:0023) |   |   | └─StringSyntax
//@[17:0023) |   |   |   └─Token(StringComplete) |'asdf'|
//@[23:0024) |   |   └─Token(RightParen) |)|
//@[24:0025) |   ├─Token(NewLine) |\n|
  substring: substring('asdfasf', 3)
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0011) |   | ├─IdentifierSyntax
//@[02:0011) |   | | └─Token(Identifier) |substring|
//@[11:0012) |   | ├─Token(Colon) |:|
//@[13:0036) |   | └─FunctionCallSyntax
//@[13:0022) |   |   ├─IdentifierSyntax
//@[13:0022) |   |   | └─Token(Identifier) |substring|
//@[22:0023) |   |   ├─Token(LeftParen) |(|
//@[23:0032) |   |   ├─FunctionArgumentSyntax
//@[23:0032) |   |   | └─StringSyntax
//@[23:0032) |   |   |   └─Token(StringComplete) |'asdfasf'|
//@[32:0033) |   |   ├─Token(Comma) |,|
//@[34:0035) |   |   ├─FunctionArgumentSyntax
//@[34:0035) |   |   | └─IntegerLiteralSyntax
//@[34:0035) |   |   |   └─Token(Integer) |3|
//@[35:0036) |   |   └─Token(RightParen) |)|
//@[36:0037) |   ├─Token(NewLine) |\n|
  take: take([1, 2, 3], 2)
//@[02:0026) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |take|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0026) |   | └─FunctionCallSyntax
//@[08:0012) |   |   ├─IdentifierSyntax
//@[08:0012) |   |   | └─Token(Identifier) |take|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0022) |   |   ├─FunctionArgumentSyntax
//@[13:0022) |   |   | └─ArraySyntax
//@[13:0014) |   |   |   ├─Token(LeftSquare) |[|
//@[14:0015) |   |   |   ├─ArrayItemSyntax
//@[14:0015) |   |   |   | └─IntegerLiteralSyntax
//@[14:0015) |   |   |   |   └─Token(Integer) |1|
//@[15:0016) |   |   |   ├─Token(Comma) |,|
//@[17:0018) |   |   |   ├─ArrayItemSyntax
//@[17:0018) |   |   |   | └─IntegerLiteralSyntax
//@[17:0018) |   |   |   |   └─Token(Integer) |2|
//@[18:0019) |   |   |   ├─Token(Comma) |,|
//@[20:0021) |   |   |   ├─ArrayItemSyntax
//@[20:0021) |   |   |   | └─IntegerLiteralSyntax
//@[20:0021) |   |   |   |   └─Token(Integer) |3|
//@[21:0022) |   |   |   └─Token(RightSquare) |]|
//@[22:0023) |   |   ├─Token(Comma) |,|
//@[24:0025) |   |   ├─FunctionArgumentSyntax
//@[24:0025) |   |   | └─IntegerLiteralSyntax
//@[24:0025) |   |   |   └─Token(Integer) |2|
//@[25:0026) |   |   └─Token(RightParen) |)|
//@[26:0027) |   ├─Token(NewLine) |\n|
  toLower: toLower('AiKInIniIN')
//@[02:0032) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |toLower|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0032) |   | └─FunctionCallSyntax
//@[11:0018) |   |   ├─IdentifierSyntax
//@[11:0018) |   |   | └─Token(Identifier) |toLower|
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0031) |   |   ├─FunctionArgumentSyntax
//@[19:0031) |   |   | └─StringSyntax
//@[19:0031) |   |   |   └─Token(StringComplete) |'AiKInIniIN'|
//@[31:0032) |   |   └─Token(RightParen) |)|
//@[32:0033) |   ├─Token(NewLine) |\n|
  toObject: toObject(['a', 'b', 'c'], x => x, x => 'Hi ${x}!')
//@[02:0062) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |toObject|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0062) |   | └─FunctionCallSyntax
//@[12:0020) |   |   ├─IdentifierSyntax
//@[12:0020) |   |   | └─Token(Identifier) |toObject|
//@[20:0021) |   |   ├─Token(LeftParen) |(|
//@[21:0036) |   |   ├─FunctionArgumentSyntax
//@[21:0036) |   |   | └─ArraySyntax
//@[21:0022) |   |   |   ├─Token(LeftSquare) |[|
//@[22:0025) |   |   |   ├─ArrayItemSyntax
//@[22:0025) |   |   |   | └─StringSyntax
//@[22:0025) |   |   |   |   └─Token(StringComplete) |'a'|
//@[25:0026) |   |   |   ├─Token(Comma) |,|
//@[27:0030) |   |   |   ├─ArrayItemSyntax
//@[27:0030) |   |   |   | └─StringSyntax
//@[27:0030) |   |   |   |   └─Token(StringComplete) |'b'|
//@[30:0031) |   |   |   ├─Token(Comma) |,|
//@[32:0035) |   |   |   ├─ArrayItemSyntax
//@[32:0035) |   |   |   | └─StringSyntax
//@[32:0035) |   |   |   |   └─Token(StringComplete) |'c'|
//@[35:0036) |   |   |   └─Token(RightSquare) |]|
//@[36:0037) |   |   ├─Token(Comma) |,|
//@[38:0044) |   |   ├─FunctionArgumentSyntax
//@[38:0044) |   |   | └─LambdaSyntax
//@[38:0039) |   |   |   ├─LocalVariableSyntax
//@[38:0039) |   |   |   | └─IdentifierSyntax
//@[38:0039) |   |   |   |   └─Token(Identifier) |x|
//@[40:0042) |   |   |   ├─Token(Arrow) |=>|
//@[43:0044) |   |   |   └─VariableAccessSyntax
//@[43:0044) |   |   |     └─IdentifierSyntax
//@[43:0044) |   |   |       └─Token(Identifier) |x|
//@[44:0045) |   |   ├─Token(Comma) |,|
//@[46:0061) |   |   ├─FunctionArgumentSyntax
//@[46:0061) |   |   | └─LambdaSyntax
//@[46:0047) |   |   |   ├─LocalVariableSyntax
//@[46:0047) |   |   |   | └─IdentifierSyntax
//@[46:0047) |   |   |   |   └─Token(Identifier) |x|
//@[48:0050) |   |   |   ├─Token(Arrow) |=>|
//@[51:0061) |   |   |   └─StringSyntax
//@[51:0057) |   |   |     ├─Token(StringLeftPiece) |'Hi ${|
//@[57:0058) |   |   |     ├─VariableAccessSyntax
//@[57:0058) |   |   |     | └─IdentifierSyntax
//@[57:0058) |   |   |     |   └─Token(Identifier) |x|
//@[58:0061) |   |   |     └─Token(StringRightPiece) |}!'|
//@[61:0062) |   |   └─Token(RightParen) |)|
//@[62:0063) |   ├─Token(NewLine) |\n|
  toUpper: toUpper('AiKInIniIN')
//@[02:0032) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |toUpper|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0032) |   | └─FunctionCallSyntax
//@[11:0018) |   |   ├─IdentifierSyntax
//@[11:0018) |   |   | └─Token(Identifier) |toUpper|
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0031) |   |   ├─FunctionArgumentSyntax
//@[19:0031) |   |   | └─StringSyntax
//@[19:0031) |   |   |   └─Token(StringComplete) |'AiKInIniIN'|
//@[31:0032) |   |   └─Token(RightParen) |)|
//@[32:0033) |   ├─Token(NewLine) |\n|
  trim: trim('  adf asdf  ')
//@[02:0028) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |trim|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0028) |   | └─FunctionCallSyntax
//@[08:0012) |   |   ├─IdentifierSyntax
//@[08:0012) |   |   | └─Token(Identifier) |trim|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0027) |   |   ├─FunctionArgumentSyntax
//@[13:0027) |   |   | └─StringSyntax
//@[13:0027) |   |   |   └─Token(StringComplete) |'  adf asdf  '|
//@[27:0028) |   |   └─Token(RightParen) |)|
//@[28:0029) |   ├─Token(NewLine) |\n|
  union: union({ abc: 'def' }, { def: 'ghi' })
//@[02:0046) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |union|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0046) |   | └─FunctionCallSyntax
//@[09:0014) |   |   ├─IdentifierSyntax
//@[09:0014) |   |   | └─Token(Identifier) |union|
//@[14:0015) |   |   ├─Token(LeftParen) |(|
//@[15:0029) |   |   ├─FunctionArgumentSyntax
//@[15:0029) |   |   | └─ObjectSyntax
//@[15:0016) |   |   |   ├─Token(LeftBrace) |{|
//@[17:0027) |   |   |   ├─ObjectPropertySyntax
//@[17:0020) |   |   |   | ├─IdentifierSyntax
//@[17:0020) |   |   |   | | └─Token(Identifier) |abc|
//@[20:0021) |   |   |   | ├─Token(Colon) |:|
//@[22:0027) |   |   |   | └─StringSyntax
//@[22:0027) |   |   |   |   └─Token(StringComplete) |'def'|
//@[28:0029) |   |   |   └─Token(RightBrace) |}|
//@[29:0030) |   |   ├─Token(Comma) |,|
//@[31:0045) |   |   ├─FunctionArgumentSyntax
//@[31:0045) |   |   | └─ObjectSyntax
//@[31:0032) |   |   |   ├─Token(LeftBrace) |{|
//@[33:0043) |   |   |   ├─ObjectPropertySyntax
//@[33:0036) |   |   |   | ├─IdentifierSyntax
//@[33:0036) |   |   |   | | └─Token(Identifier) |def|
//@[36:0037) |   |   |   | ├─Token(Colon) |:|
//@[38:0043) |   |   |   | └─StringSyntax
//@[38:0043) |   |   |   |   └─Token(StringComplete) |'ghi'|
//@[44:0045) |   |   |   └─Token(RightBrace) |}|
//@[45:0046) |   |   └─Token(RightParen) |)|
//@[46:0047) |   ├─Token(NewLine) |\n|
  uniqueString: uniqueString('asd', 'asdf', 'asdf')
//@[02:0051) |   ├─ObjectPropertySyntax
//@[02:0014) |   | ├─IdentifierSyntax
//@[02:0014) |   | | └─Token(Identifier) |uniqueString|
//@[14:0015) |   | ├─Token(Colon) |:|
//@[16:0051) |   | └─FunctionCallSyntax
//@[16:0028) |   |   ├─IdentifierSyntax
//@[16:0028) |   |   | └─Token(Identifier) |uniqueString|
//@[28:0029) |   |   ├─Token(LeftParen) |(|
//@[29:0034) |   |   ├─FunctionArgumentSyntax
//@[29:0034) |   |   | └─StringSyntax
//@[29:0034) |   |   |   └─Token(StringComplete) |'asd'|
//@[34:0035) |   |   ├─Token(Comma) |,|
//@[36:0042) |   |   ├─FunctionArgumentSyntax
//@[36:0042) |   |   | └─StringSyntax
//@[36:0042) |   |   |   └─Token(StringComplete) |'asdf'|
//@[42:0043) |   |   ├─Token(Comma) |,|
//@[44:0050) |   |   ├─FunctionArgumentSyntax
//@[44:0050) |   |   | └─StringSyntax
//@[44:0050) |   |   |   └─Token(StringComplete) |'asdf'|
//@[50:0051) |   |   └─Token(RightParen) |)|
//@[51:0052) |   ├─Token(NewLine) |\n|
  uri: uri('https://github.com', 'Azure/bicep')
//@[02:0047) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |uri|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0047) |   | └─FunctionCallSyntax
//@[07:0010) |   |   ├─IdentifierSyntax
//@[07:0010) |   |   | └─Token(Identifier) |uri|
//@[10:0011) |   |   ├─Token(LeftParen) |(|
//@[11:0031) |   |   ├─FunctionArgumentSyntax
//@[11:0031) |   |   | └─StringSyntax
//@[11:0031) |   |   |   └─Token(StringComplete) |'https://github.com'|
//@[31:0032) |   |   ├─Token(Comma) |,|
//@[33:0046) |   |   ├─FunctionArgumentSyntax
//@[33:0046) |   |   | └─StringSyntax
//@[33:0046) |   |   |   └─Token(StringComplete) |'Azure/bicep'|
//@[46:0047) |   |   └─Token(RightParen) |)|
//@[47:0048) |   ├─Token(NewLine) |\n|
  uriComponent: uriComponent('UB*8h 0+=_)9h9n')
//@[02:0047) |   ├─ObjectPropertySyntax
//@[02:0014) |   | ├─IdentifierSyntax
//@[02:0014) |   | | └─Token(Identifier) |uriComponent|
//@[14:0015) |   | ├─Token(Colon) |:|
//@[16:0047) |   | └─FunctionCallSyntax
//@[16:0028) |   |   ├─IdentifierSyntax
//@[16:0028) |   |   | └─Token(Identifier) |uriComponent|
//@[28:0029) |   |   ├─Token(LeftParen) |(|
//@[29:0046) |   |   ├─FunctionArgumentSyntax
//@[29:0046) |   |   | └─StringSyntax
//@[29:0046) |   |   |   └─Token(StringComplete) |'UB*8h 0+=_)9h9n'|
//@[46:0047) |   |   └─Token(RightParen) |)|
//@[47:0048) |   ├─Token(NewLine) |\n|
  uriComponentToString: uriComponentToString('%20%25%20')
//@[02:0057) |   ├─ObjectPropertySyntax
//@[02:0022) |   | ├─IdentifierSyntax
//@[02:0022) |   | | └─Token(Identifier) |uriComponentToString|
//@[22:0023) |   | ├─Token(Colon) |:|
//@[24:0057) |   | └─FunctionCallSyntax
//@[24:0044) |   |   ├─IdentifierSyntax
//@[24:0044) |   |   | └─Token(Identifier) |uriComponentToString|
//@[44:0045) |   |   ├─Token(LeftParen) |(|
//@[45:0056) |   |   ├─FunctionArgumentSyntax
//@[45:0056) |   |   | └─StringSyntax
//@[45:0056) |   |   |   └─Token(StringComplete) |'%20%25%20'|
//@[56:0057) |   |   └─Token(RightParen) |)|
//@[57:0058) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

param myBool = true
//@[00:0019) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |myBool|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0019) | └─BooleanLiteralSyntax
//@[15:0019) |   └─Token(TrueKeyword) |true|
//@[19:0020) ├─Token(NewLine) |\n|
param myInt = sys.int(myBool ? 123 : 456)
//@[00:0041) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0011) | ├─IdentifierSyntax
//@[06:0011) | | └─Token(Identifier) |myInt|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0041) | └─InstanceFunctionCallSyntax
//@[14:0017) |   ├─VariableAccessSyntax
//@[14:0017) |   | └─IdentifierSyntax
//@[14:0017) |   |   └─Token(Identifier) |sys|
//@[17:0018) |   ├─Token(Dot) |.|
//@[18:0021) |   ├─IdentifierSyntax
//@[18:0021) |   | └─Token(Identifier) |int|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0040) |   ├─FunctionArgumentSyntax
//@[22:0040) |   | └─TernaryOperationSyntax
//@[22:0028) |   |   ├─VariableAccessSyntax
//@[22:0028) |   |   | └─IdentifierSyntax
//@[22:0028) |   |   |   └─Token(Identifier) |myBool|
//@[29:0030) |   |   ├─Token(Question) |?|
//@[31:0034) |   |   ├─IntegerLiteralSyntax
//@[31:0034) |   |   | └─Token(Integer) |123|
//@[35:0036) |   |   ├─Token(Colon) |:|
//@[37:0040) |   |   └─IntegerLiteralSyntax
//@[37:0040) |   |     └─Token(Integer) |456|
//@[40:0041) |   └─Token(RightParen) |)|
//@[41:0043) ├─Token(NewLine) |\n\n|

param myArray = [
//@[00:0123) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |myArray|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0123) | └─ArraySyntax
//@[16:0017) |   ├─Token(LeftSquare) |[|
//@[17:0018) |   ├─Token(NewLine) |\n|
  (true ? 'a' : 'b')
//@[02:0020) |   ├─ArrayItemSyntax
//@[02:0020) |   | └─ParenthesizedExpressionSyntax
//@[02:0003) |   |   ├─Token(LeftParen) |(|
//@[03:0019) |   |   ├─TernaryOperationSyntax
//@[03:0007) |   |   | ├─BooleanLiteralSyntax
//@[03:0007) |   |   | | └─Token(TrueKeyword) |true|
//@[08:0009) |   |   | ├─Token(Question) |?|
//@[10:0013) |   |   | ├─StringSyntax
//@[10:0013) |   |   | | └─Token(StringComplete) |'a'|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0019) |   |   | └─StringSyntax
//@[16:0019) |   |   |   └─Token(StringComplete) |'b'|
//@[19:0020) |   |   └─Token(RightParen) |)|
//@[20:0021) |   ├─Token(NewLine) |\n|
  !true
//@[02:0007) |   ├─ArrayItemSyntax
//@[02:0007) |   | └─UnaryOperationSyntax
//@[02:0003) |   |   ├─Token(Exclamation) |!|
//@[03:0007) |   |   └─BooleanLiteralSyntax
//@[03:0007) |   |     └─Token(TrueKeyword) |true|
//@[07:0008) |   ├─Token(NewLine) |\n|
  123 + 456
//@[02:0011) |   ├─ArrayItemSyntax
//@[02:0011) |   | └─BinaryOperationSyntax
//@[02:0005) |   |   ├─IntegerLiteralSyntax
//@[02:0005) |   |   | └─Token(Integer) |123|
//@[06:0007) |   |   ├─Token(Plus) |+|
//@[08:0011) |   |   └─IntegerLiteralSyntax
//@[08:0011) |   |     └─Token(Integer) |456|
//@[11:0012) |   ├─Token(NewLine) |\n|
  456 - 123
//@[02:0011) |   ├─ArrayItemSyntax
//@[02:0011) |   | └─BinaryOperationSyntax
//@[02:0005) |   |   ├─IntegerLiteralSyntax
//@[02:0005) |   |   | └─Token(Integer) |456|
//@[06:0007) |   |   ├─Token(Minus) |-|
//@[08:0011) |   |   └─IntegerLiteralSyntax
//@[08:0011) |   |     └─Token(Integer) |123|
//@[11:0012) |   ├─Token(NewLine) |\n|
  2 * 3
//@[02:0007) |   ├─ArrayItemSyntax
//@[02:0007) |   | └─BinaryOperationSyntax
//@[02:0003) |   |   ├─IntegerLiteralSyntax
//@[02:0003) |   |   | └─Token(Integer) |2|
//@[04:0005) |   |   ├─Token(Asterisk) |*|
//@[06:0007) |   |   └─IntegerLiteralSyntax
//@[06:0007) |   |     └─Token(Integer) |3|
//@[07:0008) |   ├─Token(NewLine) |\n|
  10 / 2
//@[02:0008) |   ├─ArrayItemSyntax
//@[02:0008) |   | └─BinaryOperationSyntax
//@[02:0004) |   |   ├─IntegerLiteralSyntax
//@[02:0004) |   |   | └─Token(Integer) |10|
//@[05:0006) |   |   ├─Token(Slash) |/|
//@[07:0008) |   |   └─IntegerLiteralSyntax
//@[07:0008) |   |     └─Token(Integer) |2|
//@[08:0009) |   ├─Token(NewLine) |\n|
  1 < 2
//@[02:0007) |   ├─ArrayItemSyntax
//@[02:0007) |   | └─BinaryOperationSyntax
//@[02:0003) |   |   ├─IntegerLiteralSyntax
//@[02:0003) |   |   | └─Token(Integer) |1|
//@[04:0005) |   |   ├─Token(LeftChevron) |<|
//@[06:0007) |   |   └─IntegerLiteralSyntax
//@[06:0007) |   |     └─Token(Integer) |2|
//@[07:0008) |   ├─Token(NewLine) |\n|
  1 > 2
//@[02:0007) |   ├─ArrayItemSyntax
//@[02:0007) |   | └─BinaryOperationSyntax
//@[02:0003) |   |   ├─IntegerLiteralSyntax
//@[02:0003) |   |   | └─Token(Integer) |1|
//@[04:0005) |   |   ├─Token(RightChevron) |>|
//@[06:0007) |   |   └─IntegerLiteralSyntax
//@[06:0007) |   |     └─Token(Integer) |2|
//@[07:0008) |   ├─Token(NewLine) |\n|
  1 >= 2
//@[02:0008) |   ├─ArrayItemSyntax
//@[02:0008) |   | └─BinaryOperationSyntax
//@[02:0003) |   |   ├─IntegerLiteralSyntax
//@[02:0003) |   |   | └─Token(Integer) |1|
//@[04:0006) |   |   ├─Token(GreaterThanOrEqual) |>=|
//@[07:0008) |   |   └─IntegerLiteralSyntax
//@[07:0008) |   |     └─Token(Integer) |2|
//@[08:0009) |   ├─Token(NewLine) |\n|
  1 <= 2
//@[02:0008) |   ├─ArrayItemSyntax
//@[02:0008) |   | └─BinaryOperationSyntax
//@[02:0003) |   |   ├─IntegerLiteralSyntax
//@[02:0003) |   |   | └─Token(Integer) |1|
//@[04:0006) |   |   ├─Token(LessThanOrEqual) |<=|
//@[07:0008) |   |   └─IntegerLiteralSyntax
//@[07:0008) |   |     └─Token(Integer) |2|
//@[08:0009) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
param myString = '''
//@[00:0072) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |myString|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0072) | └─StringSyntax
//@[17:0072) |   └─Token(StringComplete) |'''\nTHis\n  is\n    a\n      multiline\n        string!\n'''|
THis
  is
    a
      multiline
        string!
'''
//@[03:0004) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
