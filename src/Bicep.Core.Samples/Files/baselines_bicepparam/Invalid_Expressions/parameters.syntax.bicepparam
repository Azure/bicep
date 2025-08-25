using 'main.bicep'
//@[00:2624) ProgramSyntax
//@[00:0018) ├─UsingDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |using|
//@[06:0018) | ├─StringSyntax
//@[06:0018) | | └─Token(StringComplete) |'main.bicep'|
//@[18:0018) | └─SkippedTriviaSyntax
//@[18:0020) ├─Token(NewLine) |\n\n|

param testAny = any('foo')
//@[00:0026) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |testAny|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0026) | └─FunctionCallSyntax
//@[16:0019) |   ├─IdentifierSyntax
//@[16:0019) |   | └─Token(Identifier) |any|
//@[19:0020) |   ├─Token(LeftParen) |(|
//@[20:0025) |   ├─FunctionArgumentSyntax
//@[20:0025) |   | └─StringSyntax
//@[20:0025) |   |   └─Token(StringComplete) |'foo'|
//@[25:0026) |   └─Token(RightParen) |)|
//@[26:0027) ├─Token(NewLine) |\n|
param testArray = array({})
//@[00:0027) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |testArray|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0027) | └─FunctionCallSyntax
//@[18:0023) |   ├─IdentifierSyntax
//@[18:0023) |   | └─Token(Identifier) |array|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0026) |   ├─FunctionArgumentSyntax
//@[24:0026) |   | └─ObjectSyntax
//@[24:0025) |   |   ├─Token(LeftBrace) |{|
//@[25:0026) |   |   └─Token(RightBrace) |}|
//@[26:0027) |   └─Token(RightParen) |)|
//@[27:0028) ├─Token(NewLine) |\n|
param testBase64ToString = base64ToString(concat(base64('abc'), '@'))
//@[00:0069) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0024) | ├─IdentifierSyntax
//@[06:0024) | | └─Token(Identifier) |testBase64ToString|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0069) | └─FunctionCallSyntax
//@[27:0041) |   ├─IdentifierSyntax
//@[27:0041) |   | └─Token(Identifier) |base64ToString|
//@[41:0042) |   ├─Token(LeftParen) |(|
//@[42:0068) |   ├─FunctionArgumentSyntax
//@[42:0068) |   | └─FunctionCallSyntax
//@[42:0048) |   |   ├─IdentifierSyntax
//@[42:0048) |   |   | └─Token(Identifier) |concat|
//@[48:0049) |   |   ├─Token(LeftParen) |(|
//@[49:0062) |   |   ├─FunctionArgumentSyntax
//@[49:0062) |   |   | └─FunctionCallSyntax
//@[49:0055) |   |   |   ├─IdentifierSyntax
//@[49:0055) |   |   |   | └─Token(Identifier) |base64|
//@[55:0056) |   |   |   ├─Token(LeftParen) |(|
//@[56:0061) |   |   |   ├─FunctionArgumentSyntax
//@[56:0061) |   |   |   | └─StringSyntax
//@[56:0061) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[61:0062) |   |   |   └─Token(RightParen) |)|
//@[62:0063) |   |   ├─Token(Comma) |,|
//@[64:0067) |   |   ├─FunctionArgumentSyntax
//@[64:0067) |   |   | └─StringSyntax
//@[64:0067) |   |   |   └─Token(StringComplete) |'@'|
//@[67:0068) |   |   └─Token(RightParen) |)|
//@[68:0069) |   └─Token(RightParen) |)|
//@[69:0070) ├─Token(NewLine) |\n|
param testBase64ToJson = base64ToJson(base64('{"hi": "there"')).hi
//@[00:0066) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0022) | ├─IdentifierSyntax
//@[06:0022) | | └─Token(Identifier) |testBase64ToJson|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0066) | └─PropertyAccessSyntax
//@[25:0063) |   ├─FunctionCallSyntax
//@[25:0037) |   | ├─IdentifierSyntax
//@[25:0037) |   | | └─Token(Identifier) |base64ToJson|
//@[37:0038) |   | ├─Token(LeftParen) |(|
//@[38:0062) |   | ├─FunctionArgumentSyntax
//@[38:0062) |   | | └─FunctionCallSyntax
//@[38:0044) |   | |   ├─IdentifierSyntax
//@[38:0044) |   | |   | └─Token(Identifier) |base64|
//@[44:0045) |   | |   ├─Token(LeftParen) |(|
//@[45:0061) |   | |   ├─FunctionArgumentSyntax
//@[45:0061) |   | |   | └─StringSyntax
//@[45:0061) |   | |   |   └─Token(StringComplete) |'{"hi": "there"'|
//@[61:0062) |   | |   └─Token(RightParen) |)|
//@[62:0063) |   | └─Token(RightParen) |)|
//@[63:0064) |   ├─Token(Dot) |.|
//@[64:0066) |   └─IdentifierSyntax
//@[64:0066) |     └─Token(Identifier) |hi|
//@[66:0067) ├─Token(NewLine) |\n|
param testBool = bool('sdf')
//@[00:0028) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |testBool|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0028) | └─FunctionCallSyntax
//@[17:0021) |   ├─IdentifierSyntax
//@[17:0021) |   | └─Token(Identifier) |bool|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0027) |   ├─FunctionArgumentSyntax
//@[22:0027) |   | └─StringSyntax
//@[22:0027) |   |   └─Token(StringComplete) |'sdf'|
//@[27:0028) |   └─Token(RightParen) |)|
//@[28:0029) ├─Token(NewLine) |\n|
param testConcat = concat(['abc'], {foo: 'bar'})
//@[00:0048) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |testConcat|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0048) | └─FunctionCallSyntax
//@[19:0025) |   ├─IdentifierSyntax
//@[19:0025) |   | └─Token(Identifier) |concat|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0033) |   ├─FunctionArgumentSyntax
//@[26:0033) |   | └─ArraySyntax
//@[26:0027) |   |   ├─Token(LeftSquare) |[|
//@[27:0032) |   |   ├─ArrayItemSyntax
//@[27:0032) |   |   | └─StringSyntax
//@[27:0032) |   |   |   └─Token(StringComplete) |'abc'|
//@[32:0033) |   |   └─Token(RightSquare) |]|
//@[33:0034) |   ├─Token(Comma) |,|
//@[35:0047) |   ├─FunctionArgumentSyntax
//@[35:0047) |   | └─ObjectSyntax
//@[35:0036) |   |   ├─Token(LeftBrace) |{|
//@[36:0046) |   |   ├─ObjectPropertySyntax
//@[36:0039) |   |   | ├─IdentifierSyntax
//@[36:0039) |   |   | | └─Token(Identifier) |foo|
//@[39:0040) |   |   | ├─Token(Colon) |:|
//@[41:0046) |   |   | └─StringSyntax
//@[41:0046) |   |   |   └─Token(StringComplete) |'bar'|
//@[46:0047) |   |   └─Token(RightBrace) |}|
//@[47:0048) |   └─Token(RightParen) |)|
//@[48:0049) ├─Token(NewLine) |\n|
param testContains = contains('foo/bar', {})
//@[00:0044) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |testContains|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0044) | └─FunctionCallSyntax
//@[21:0029) |   ├─IdentifierSyntax
//@[21:0029) |   | └─Token(Identifier) |contains|
//@[29:0030) |   ├─Token(LeftParen) |(|
//@[30:0039) |   ├─FunctionArgumentSyntax
//@[30:0039) |   | └─StringSyntax
//@[30:0039) |   |   └─Token(StringComplete) |'foo/bar'|
//@[39:0040) |   ├─Token(Comma) |,|
//@[41:0043) |   ├─FunctionArgumentSyntax
//@[41:0043) |   | └─ObjectSyntax
//@[41:0042) |   |   ├─Token(LeftBrace) |{|
//@[42:0043) |   |   └─Token(RightBrace) |}|
//@[43:0044) |   └─Token(RightParen) |)|
//@[44:0045) ├─Token(NewLine) |\n|
param testDataUriToString = dataUriToString(concat(dataUri('abc'), '@'))
//@[00:0072) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0025) | ├─IdentifierSyntax
//@[06:0025) | | └─Token(Identifier) |testDataUriToString|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0072) | └─FunctionCallSyntax
//@[28:0043) |   ├─IdentifierSyntax
//@[28:0043) |   | └─Token(Identifier) |dataUriToString|
//@[43:0044) |   ├─Token(LeftParen) |(|
//@[44:0071) |   ├─FunctionArgumentSyntax
//@[44:0071) |   | └─FunctionCallSyntax
//@[44:0050) |   |   ├─IdentifierSyntax
//@[44:0050) |   |   | └─Token(Identifier) |concat|
//@[50:0051) |   |   ├─Token(LeftParen) |(|
//@[51:0065) |   |   ├─FunctionArgumentSyntax
//@[51:0065) |   |   | └─FunctionCallSyntax
//@[51:0058) |   |   |   ├─IdentifierSyntax
//@[51:0058) |   |   |   | └─Token(Identifier) |dataUri|
//@[58:0059) |   |   |   ├─Token(LeftParen) |(|
//@[59:0064) |   |   |   ├─FunctionArgumentSyntax
//@[59:0064) |   |   |   | └─StringSyntax
//@[59:0064) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[64:0065) |   |   |   └─Token(RightParen) |)|
//@[65:0066) |   |   ├─Token(Comma) |,|
//@[67:0070) |   |   ├─FunctionArgumentSyntax
//@[67:0070) |   |   | └─StringSyntax
//@[67:0070) |   |   |   └─Token(StringComplete) |'@'|
//@[70:0071) |   |   └─Token(RightParen) |)|
//@[71:0072) |   └─Token(RightParen) |)|
//@[72:0073) ├─Token(NewLine) |\n|
param testDateTimeAdd = dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')  
//@[00:0081) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |testDateTimeAdd|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0081) | └─FunctionCallSyntax
//@[24:0035) |   ├─IdentifierSyntax
//@[24:0035) |   | └─Token(Identifier) |dateTimeAdd|
//@[35:0036) |   ├─Token(LeftParen) |(|
//@[36:0065) |   ├─FunctionArgumentSyntax
//@[36:0065) |   | └─FunctionCallSyntax
//@[36:0053) |   |   ├─IdentifierSyntax
//@[36:0053) |   |   | └─Token(Identifier) |dateTimeFromEpoch|
//@[53:0054) |   |   ├─Token(LeftParen) |(|
//@[54:0064) |   |   ├─FunctionArgumentSyntax
//@[54:0064) |   |   | └─IntegerLiteralSyntax
//@[54:0064) |   |   |   └─Token(Integer) |1680224438|
//@[64:0065) |   |   └─Token(RightParen) |)|
//@[65:0066) |   ├─Token(Comma) |,|
//@[67:0080) |   ├─FunctionArgumentSyntax
//@[67:0080) |   | └─StringSyntax
//@[67:0080) |   |   └─Token(StringComplete) |'PTASDIONS1D'|
//@[80:0081) |   └─Token(RightParen) |)|
//@[83:0084) ├─Token(NewLine) |\n|
param testDateTimeToEpoch = dateTimeToEpoch(dateTimeFromEpoch('adfasdf'))
//@[00:0073) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0025) | ├─IdentifierSyntax
//@[06:0025) | | └─Token(Identifier) |testDateTimeToEpoch|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0073) | └─FunctionCallSyntax
//@[28:0043) |   ├─IdentifierSyntax
//@[28:0043) |   | └─Token(Identifier) |dateTimeToEpoch|
//@[43:0044) |   ├─Token(LeftParen) |(|
//@[44:0072) |   ├─FunctionArgumentSyntax
//@[44:0072) |   | └─FunctionCallSyntax
//@[44:0061) |   |   ├─IdentifierSyntax
//@[44:0061) |   |   | └─Token(Identifier) |dateTimeFromEpoch|
//@[61:0062) |   |   ├─Token(LeftParen) |(|
//@[62:0071) |   |   ├─FunctionArgumentSyntax
//@[62:0071) |   |   | └─StringSyntax
//@[62:0071) |   |   |   └─Token(StringComplete) |'adfasdf'|
//@[71:0072) |   |   └─Token(RightParen) |)|
//@[72:0073) |   └─Token(RightParen) |)|
//@[73:0074) ├─Token(NewLine) |\n|
param testEmpty = empty([])
//@[00:0027) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |testEmpty|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0027) | └─FunctionCallSyntax
//@[18:0023) |   ├─IdentifierSyntax
//@[18:0023) |   | └─Token(Identifier) |empty|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0026) |   ├─FunctionArgumentSyntax
//@[24:0026) |   | └─ArraySyntax
//@[24:0025) |   |   ├─Token(LeftSquare) |[|
//@[25:0026) |   |   └─Token(RightSquare) |]|
//@[26:0027) |   └─Token(RightParen) |)|
//@[27:0028) ├─Token(NewLine) |\n|
param testEndsWith = endsWith('foo', [])
//@[00:0040) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |testEndsWith|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0040) | └─FunctionCallSyntax
//@[21:0029) |   ├─IdentifierSyntax
//@[21:0029) |   | └─Token(Identifier) |endsWith|
//@[29:0030) |   ├─Token(LeftParen) |(|
//@[30:0035) |   ├─FunctionArgumentSyntax
//@[30:0035) |   | └─StringSyntax
//@[30:0035) |   |   └─Token(StringComplete) |'foo'|
//@[35:0036) |   ├─Token(Comma) |,|
//@[37:0039) |   ├─FunctionArgumentSyntax
//@[37:0039) |   | └─ArraySyntax
//@[37:0038) |   |   ├─Token(LeftSquare) |[|
//@[38:0039) |   |   └─Token(RightSquare) |]|
//@[39:0040) |   └─Token(RightParen) |)|
//@[40:0041) ├─Token(NewLine) |\n|
param testFilter = filter([1, 2], i => i < 'foo')
//@[00:0049) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |testFilter|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0049) | └─FunctionCallSyntax
//@[19:0025) |   ├─IdentifierSyntax
//@[19:0025) |   | └─Token(Identifier) |filter|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0032) |   ├─FunctionArgumentSyntax
//@[26:0032) |   | └─ArraySyntax
//@[26:0027) |   |   ├─Token(LeftSquare) |[|
//@[27:0028) |   |   ├─ArrayItemSyntax
//@[27:0028) |   |   | └─IntegerLiteralSyntax
//@[27:0028) |   |   |   └─Token(Integer) |1|
//@[28:0029) |   |   ├─Token(Comma) |,|
//@[30:0031) |   |   ├─ArrayItemSyntax
//@[30:0031) |   |   | └─IntegerLiteralSyntax
//@[30:0031) |   |   |   └─Token(Integer) |2|
//@[31:0032) |   |   └─Token(RightSquare) |]|
//@[32:0033) |   ├─Token(Comma) |,|
//@[34:0048) |   ├─FunctionArgumentSyntax
//@[34:0048) |   | └─LambdaSyntax
//@[34:0035) |   |   ├─LocalVariableSyntax
//@[34:0035) |   |   | └─IdentifierSyntax
//@[34:0035) |   |   |   └─Token(Identifier) |i|
//@[36:0038) |   |   ├─Token(Arrow) |=>|
//@[39:0048) |   |   └─BinaryOperationSyntax
//@[39:0040) |   |     ├─VariableAccessSyntax
//@[39:0040) |   |     | └─IdentifierSyntax
//@[39:0040) |   |     |   └─Token(Identifier) |i|
//@[41:0042) |   |     ├─Token(LeftChevron) |<|
//@[43:0048) |   |     └─StringSyntax
//@[43:0048) |   |       └─Token(StringComplete) |'foo'|
//@[48:0049) |   └─Token(RightParen) |)|
//@[49:0050) ├─Token(NewLine) |\n|
param testFirst = first('asdfds')
//@[00:0033) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |testFirst|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0033) | └─FunctionCallSyntax
//@[18:0023) |   ├─IdentifierSyntax
//@[18:0023) |   | └─Token(Identifier) |first|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0032) |   ├─FunctionArgumentSyntax
//@[24:0032) |   | └─StringSyntax
//@[24:0032) |   |   └─Token(StringComplete) |'asdfds'|
//@[32:0033) |   └─Token(RightParen) |)|
//@[33:0034) ├─Token(NewLine) |\n|
param testFlatten = flatten({foo: 'bar'})
//@[00:0041) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |testFlatten|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0041) | └─FunctionCallSyntax
//@[20:0027) |   ├─IdentifierSyntax
//@[20:0027) |   | └─Token(Identifier) |flatten|
//@[27:0028) |   ├─Token(LeftParen) |(|
//@[28:0040) |   ├─FunctionArgumentSyntax
//@[28:0040) |   | └─ObjectSyntax
//@[28:0029) |   |   ├─Token(LeftBrace) |{|
//@[29:0039) |   |   ├─ObjectPropertySyntax
//@[29:0032) |   |   | ├─IdentifierSyntax
//@[29:0032) |   |   | | └─Token(Identifier) |foo|
//@[32:0033) |   |   | ├─Token(Colon) |:|
//@[34:0039) |   |   | └─StringSyntax
//@[34:0039) |   |   |   └─Token(StringComplete) |'bar'|
//@[39:0040) |   |   └─Token(RightBrace) |}|
//@[40:0041) |   └─Token(RightParen) |)|
//@[41:0042) ├─Token(NewLine) |\n|
param testFormat = format('->{123}<-', 123)
//@[00:0043) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |testFormat|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0043) | └─FunctionCallSyntax
//@[19:0025) |   ├─IdentifierSyntax
//@[19:0025) |   | └─Token(Identifier) |format|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0037) |   ├─FunctionArgumentSyntax
//@[26:0037) |   | └─StringSyntax
//@[26:0037) |   |   └─Token(StringComplete) |'->{123}<-'|
//@[37:0038) |   ├─Token(Comma) |,|
//@[39:0042) |   ├─FunctionArgumentSyntax
//@[39:0042) |   | └─IntegerLiteralSyntax
//@[39:0042) |   |   └─Token(Integer) |123|
//@[42:0043) |   └─Token(RightParen) |)|
//@[43:0044) ├─Token(NewLine) |\n|
param testGuid = guid({})
//@[00:0025) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |testGuid|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0025) | └─FunctionCallSyntax
//@[17:0021) |   ├─IdentifierSyntax
//@[17:0021) |   | └─Token(Identifier) |guid|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0024) |   ├─FunctionArgumentSyntax
//@[22:0024) |   | └─ObjectSyntax
//@[22:0023) |   |   ├─Token(LeftBrace) |{|
//@[23:0024) |   |   └─Token(RightBrace) |}|
//@[24:0025) |   └─Token(RightParen) |)|
//@[25:0026) ├─Token(NewLine) |\n|
param testIndexOf = indexOf('abc', {})
//@[00:0038) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |testIndexOf|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0038) | └─FunctionCallSyntax
//@[20:0027) |   ├─IdentifierSyntax
//@[20:0027) |   | └─Token(Identifier) |indexOf|
//@[27:0028) |   ├─Token(LeftParen) |(|
//@[28:0033) |   ├─FunctionArgumentSyntax
//@[28:0033) |   | └─StringSyntax
//@[28:0033) |   |   └─Token(StringComplete) |'abc'|
//@[33:0034) |   ├─Token(Comma) |,|
//@[35:0037) |   ├─FunctionArgumentSyntax
//@[35:0037) |   | └─ObjectSyntax
//@[35:0036) |   |   ├─Token(LeftBrace) |{|
//@[36:0037) |   |   └─Token(RightBrace) |}|
//@[37:0038) |   └─Token(RightParen) |)|
//@[38:0039) ├─Token(NewLine) |\n|
param testInt = int('asdf')
//@[00:0027) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |testInt|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0027) | └─FunctionCallSyntax
//@[16:0019) |   ├─IdentifierSyntax
//@[16:0019) |   | └─Token(Identifier) |int|
//@[19:0020) |   ├─Token(LeftParen) |(|
//@[20:0026) |   ├─FunctionArgumentSyntax
//@[20:0026) |   | └─StringSyntax
//@[20:0026) |   |   └─Token(StringComplete) |'asdf'|
//@[26:0027) |   └─Token(RightParen) |)|
//@[27:0028) ├─Token(NewLine) |\n|
param testIntersection = intersection([1, 2, 3], 'foo')
//@[00:0055) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0022) | ├─IdentifierSyntax
//@[06:0022) | | └─Token(Identifier) |testIntersection|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0055) | └─FunctionCallSyntax
//@[25:0037) |   ├─IdentifierSyntax
//@[25:0037) |   | └─Token(Identifier) |intersection|
//@[37:0038) |   ├─Token(LeftParen) |(|
//@[38:0047) |   ├─FunctionArgumentSyntax
//@[38:0047) |   | └─ArraySyntax
//@[38:0039) |   |   ├─Token(LeftSquare) |[|
//@[39:0040) |   |   ├─ArrayItemSyntax
//@[39:0040) |   |   | └─IntegerLiteralSyntax
//@[39:0040) |   |   |   └─Token(Integer) |1|
//@[40:0041) |   |   ├─Token(Comma) |,|
//@[42:0043) |   |   ├─ArrayItemSyntax
//@[42:0043) |   |   | └─IntegerLiteralSyntax
//@[42:0043) |   |   |   └─Token(Integer) |2|
//@[43:0044) |   |   ├─Token(Comma) |,|
//@[45:0046) |   |   ├─ArrayItemSyntax
//@[45:0046) |   |   | └─IntegerLiteralSyntax
//@[45:0046) |   |   |   └─Token(Integer) |3|
//@[46:0047) |   |   └─Token(RightSquare) |]|
//@[47:0048) |   ├─Token(Comma) |,|
//@[49:0054) |   ├─FunctionArgumentSyntax
//@[49:0054) |   | └─StringSyntax
//@[49:0054) |   |   └─Token(StringComplete) |'foo'|
//@[54:0055) |   └─Token(RightParen) |)|
//@[55:0056) ├─Token(NewLine) |\n|
param testItems = items('asdfas')
//@[00:0033) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |testItems|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0033) | └─FunctionCallSyntax
//@[18:0023) |   ├─IdentifierSyntax
//@[18:0023) |   | └─Token(Identifier) |items|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0032) |   ├─FunctionArgumentSyntax
//@[24:0032) |   | └─StringSyntax
//@[24:0032) |   |   └─Token(StringComplete) |'asdfas'|
//@[32:0033) |   └─Token(RightParen) |)|
//@[33:0034) ├─Token(NewLine) |\n|
param testJoin = join(['abc', 'def', 'ghi'], {})
//@[00:0048) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |testJoin|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0048) | └─FunctionCallSyntax
//@[17:0021) |   ├─IdentifierSyntax
//@[17:0021) |   | └─Token(Identifier) |join|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0043) |   ├─FunctionArgumentSyntax
//@[22:0043) |   | └─ArraySyntax
//@[22:0023) |   |   ├─Token(LeftSquare) |[|
//@[23:0028) |   |   ├─ArrayItemSyntax
//@[23:0028) |   |   | └─StringSyntax
//@[23:0028) |   |   |   └─Token(StringComplete) |'abc'|
//@[28:0029) |   |   ├─Token(Comma) |,|
//@[30:0035) |   |   ├─ArrayItemSyntax
//@[30:0035) |   |   | └─StringSyntax
//@[30:0035) |   |   |   └─Token(StringComplete) |'def'|
//@[35:0036) |   |   ├─Token(Comma) |,|
//@[37:0042) |   |   ├─ArrayItemSyntax
//@[37:0042) |   |   | └─StringSyntax
//@[37:0042) |   |   |   └─Token(StringComplete) |'ghi'|
//@[42:0043) |   |   └─Token(RightSquare) |]|
//@[43:0044) |   ├─Token(Comma) |,|
//@[45:0047) |   ├─FunctionArgumentSyntax
//@[45:0047) |   | └─ObjectSyntax
//@[45:0046) |   |   ├─Token(LeftBrace) |{|
//@[46:0047) |   |   └─Token(RightBrace) |}|
//@[47:0048) |   └─Token(RightParen) |)|
//@[48:0049) ├─Token(NewLine) |\n|
param testLast = last('asdf')
//@[00:0029) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |testLast|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0029) | └─FunctionCallSyntax
//@[17:0021) |   ├─IdentifierSyntax
//@[17:0021) |   | └─Token(Identifier) |last|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0028) |   ├─FunctionArgumentSyntax
//@[22:0028) |   | └─StringSyntax
//@[22:0028) |   |   └─Token(StringComplete) |'asdf'|
//@[28:0029) |   └─Token(RightParen) |)|
//@[29:0030) ├─Token(NewLine) |\n|
param testLastIndexOf = lastIndexOf('abcba', {})
//@[00:0048) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |testLastIndexOf|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0048) | └─FunctionCallSyntax
//@[24:0035) |   ├─IdentifierSyntax
//@[24:0035) |   | └─Token(Identifier) |lastIndexOf|
//@[35:0036) |   ├─Token(LeftParen) |(|
//@[36:0043) |   ├─FunctionArgumentSyntax
//@[36:0043) |   | └─StringSyntax
//@[36:0043) |   |   └─Token(StringComplete) |'abcba'|
//@[43:0044) |   ├─Token(Comma) |,|
//@[45:0047) |   ├─FunctionArgumentSyntax
//@[45:0047) |   | └─ObjectSyntax
//@[45:0046) |   |   ├─Token(LeftBrace) |{|
//@[46:0047) |   |   └─Token(RightBrace) |}|
//@[47:0048) |   └─Token(RightParen) |)|
//@[48:0049) ├─Token(NewLine) |\n|
param testLength = length({})
//@[00:0029) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |testLength|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0029) | └─FunctionCallSyntax
//@[19:0025) |   ├─IdentifierSyntax
//@[19:0025) |   | └─Token(Identifier) |length|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0028) |   ├─FunctionArgumentSyntax
//@[26:0028) |   | └─ObjectSyntax
//@[26:0027) |   |   ├─Token(LeftBrace) |{|
//@[27:0028) |   |   └─Token(RightBrace) |}|
//@[28:0029) |   └─Token(RightParen) |)|
//@[29:0030) ├─Token(NewLine) |\n|
param testLoadFileAsBase64 = loadFileAsBase64('test.txt')
//@[00:0057) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0026) | ├─IdentifierSyntax
//@[06:0026) | | └─Token(Identifier) |testLoadFileAsBase64|
//@[27:0028) | ├─Token(Assignment) |=|
//@[29:0057) | └─FunctionCallSyntax
//@[29:0045) |   ├─IdentifierSyntax
//@[29:0045) |   | └─Token(Identifier) |loadFileAsBase64|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0056) |   ├─FunctionArgumentSyntax
//@[46:0056) |   | └─StringSyntax
//@[46:0056) |   |   └─Token(StringComplete) |'test.txt'|
//@[56:0057) |   └─Token(RightParen) |)|
//@[57:0058) ├─Token(NewLine) |\n|
param testLoadJsonContent = loadJsonContent('test.json').adsfsd
//@[00:0063) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0025) | ├─IdentifierSyntax
//@[06:0025) | | └─Token(Identifier) |testLoadJsonContent|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0063) | └─PropertyAccessSyntax
//@[28:0056) |   ├─FunctionCallSyntax
//@[28:0043) |   | ├─IdentifierSyntax
//@[28:0043) |   | | └─Token(Identifier) |loadJsonContent|
//@[43:0044) |   | ├─Token(LeftParen) |(|
//@[44:0055) |   | ├─FunctionArgumentSyntax
//@[44:0055) |   | | └─StringSyntax
//@[44:0055) |   | |   └─Token(StringComplete) |'test.json'|
//@[55:0056) |   | └─Token(RightParen) |)|
//@[56:0057) |   ├─Token(Dot) |.|
//@[57:0063) |   └─IdentifierSyntax
//@[57:0063) |     └─Token(Identifier) |adsfsd|
//@[63:0064) ├─Token(NewLine) |\n|
param testLoadTextContent = loadTextContent('test.txt')
//@[00:0055) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0025) | ├─IdentifierSyntax
//@[06:0025) | | └─Token(Identifier) |testLoadTextContent|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0055) | └─FunctionCallSyntax
//@[28:0043) |   ├─IdentifierSyntax
//@[28:0043) |   | └─Token(Identifier) |loadTextContent|
//@[43:0044) |   ├─Token(LeftParen) |(|
//@[44:0054) |   ├─FunctionArgumentSyntax
//@[44:0054) |   | └─StringSyntax
//@[44:0054) |   |   └─Token(StringComplete) |'test.txt'|
//@[54:0055) |   └─Token(RightParen) |)|
//@[55:0056) ├─Token(NewLine) |\n|
param testMap = map(range(0, 3), i => dataUriToString('Hi ${i}!'))
//@[00:0066) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |testMap|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0066) | └─FunctionCallSyntax
//@[16:0019) |   ├─IdentifierSyntax
//@[16:0019) |   | └─Token(Identifier) |map|
//@[19:0020) |   ├─Token(LeftParen) |(|
//@[20:0031) |   ├─FunctionArgumentSyntax
//@[20:0031) |   | └─FunctionCallSyntax
//@[20:0025) |   |   ├─IdentifierSyntax
//@[20:0025) |   |   | └─Token(Identifier) |range|
//@[25:0026) |   |   ├─Token(LeftParen) |(|
//@[26:0027) |   |   ├─FunctionArgumentSyntax
//@[26:0027) |   |   | └─IntegerLiteralSyntax
//@[26:0027) |   |   |   └─Token(Integer) |0|
//@[27:0028) |   |   ├─Token(Comma) |,|
//@[29:0030) |   |   ├─FunctionArgumentSyntax
//@[29:0030) |   |   | └─IntegerLiteralSyntax
//@[29:0030) |   |   |   └─Token(Integer) |3|
//@[30:0031) |   |   └─Token(RightParen) |)|
//@[31:0032) |   ├─Token(Comma) |,|
//@[33:0065) |   ├─FunctionArgumentSyntax
//@[33:0065) |   | └─LambdaSyntax
//@[33:0034) |   |   ├─LocalVariableSyntax
//@[33:0034) |   |   | └─IdentifierSyntax
//@[33:0034) |   |   |   └─Token(Identifier) |i|
//@[35:0037) |   |   ├─Token(Arrow) |=>|
//@[38:0065) |   |   └─FunctionCallSyntax
//@[38:0053) |   |     ├─IdentifierSyntax
//@[38:0053) |   |     | └─Token(Identifier) |dataUriToString|
//@[53:0054) |   |     ├─Token(LeftParen) |(|
//@[54:0064) |   |     ├─FunctionArgumentSyntax
//@[54:0064) |   |     | └─StringSyntax
//@[54:0060) |   |     |   ├─Token(StringLeftPiece) |'Hi ${|
//@[60:0061) |   |     |   ├─VariableAccessSyntax
//@[60:0061) |   |     |   | └─IdentifierSyntax
//@[60:0061) |   |     |   |   └─Token(Identifier) |i|
//@[61:0064) |   |     |   └─Token(StringRightPiece) |}!'|
//@[64:0065) |   |     └─Token(RightParen) |)|
//@[65:0066) |   └─Token(RightParen) |)|
//@[66:0067) ├─Token(NewLine) |\n|
param testMax = max(1, 2, '3')
//@[00:0030) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |testMax|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0030) | └─FunctionCallSyntax
//@[16:0019) |   ├─IdentifierSyntax
//@[16:0019) |   | └─Token(Identifier) |max|
//@[19:0020) |   ├─Token(LeftParen) |(|
//@[20:0021) |   ├─FunctionArgumentSyntax
//@[20:0021) |   | └─IntegerLiteralSyntax
//@[20:0021) |   |   └─Token(Integer) |1|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0024) |   ├─FunctionArgumentSyntax
//@[23:0024) |   | └─IntegerLiteralSyntax
//@[23:0024) |   |   └─Token(Integer) |2|
//@[24:0025) |   ├─Token(Comma) |,|
//@[26:0029) |   ├─FunctionArgumentSyntax
//@[26:0029) |   | └─StringSyntax
//@[26:0029) |   |   └─Token(StringComplete) |'3'|
//@[29:0030) |   └─Token(RightParen) |)|
//@[30:0031) ├─Token(NewLine) |\n|
param testMin = min(1, 2, {})
//@[00:0029) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |testMin|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0029) | └─FunctionCallSyntax
//@[16:0019) |   ├─IdentifierSyntax
//@[16:0019) |   | └─Token(Identifier) |min|
//@[19:0020) |   ├─Token(LeftParen) |(|
//@[20:0021) |   ├─FunctionArgumentSyntax
//@[20:0021) |   | └─IntegerLiteralSyntax
//@[20:0021) |   |   └─Token(Integer) |1|
//@[21:0022) |   ├─Token(Comma) |,|
//@[23:0024) |   ├─FunctionArgumentSyntax
//@[23:0024) |   | └─IntegerLiteralSyntax
//@[23:0024) |   |   └─Token(Integer) |2|
//@[24:0025) |   ├─Token(Comma) |,|
//@[26:0028) |   ├─FunctionArgumentSyntax
//@[26:0028) |   | └─ObjectSyntax
//@[26:0027) |   |   ├─Token(LeftBrace) |{|
//@[27:0028) |   |   └─Token(RightBrace) |}|
//@[28:0029) |   └─Token(RightParen) |)|
//@[29:0030) ├─Token(NewLine) |\n|
param testPadLeft = padLeft(13, 'foo')
//@[00:0038) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |testPadLeft|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0038) | └─FunctionCallSyntax
//@[20:0027) |   ├─IdentifierSyntax
//@[20:0027) |   | └─Token(Identifier) |padLeft|
//@[27:0028) |   ├─Token(LeftParen) |(|
//@[28:0030) |   ├─FunctionArgumentSyntax
//@[28:0030) |   | └─IntegerLiteralSyntax
//@[28:0030) |   |   └─Token(Integer) |13|
//@[30:0031) |   ├─Token(Comma) |,|
//@[32:0037) |   ├─FunctionArgumentSyntax
//@[32:0037) |   | └─StringSyntax
//@[32:0037) |   |   └─Token(StringComplete) |'foo'|
//@[37:0038) |   └─Token(RightParen) |)|
//@[38:0039) ├─Token(NewLine) |\n|
param testRange = range(0, 'foo')
//@[00:0033) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |testRange|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0033) | └─FunctionCallSyntax
//@[18:0023) |   ├─IdentifierSyntax
//@[18:0023) |   | └─Token(Identifier) |range|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0025) |   ├─FunctionArgumentSyntax
//@[24:0025) |   | └─IntegerLiteralSyntax
//@[24:0025) |   |   └─Token(Integer) |0|
//@[25:0026) |   ├─Token(Comma) |,|
//@[27:0032) |   ├─FunctionArgumentSyntax
//@[27:0032) |   | └─StringSyntax
//@[27:0032) |   |   └─Token(StringComplete) |'foo'|
//@[32:0033) |   └─Token(RightParen) |)|
//@[33:0034) ├─Token(NewLine) |\n|
param testReduce = reduce(['a', 'b', 'c'], '', (a, b) => '${toObject(a)}-${b}')
//@[00:0079) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |testReduce|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0079) | └─FunctionCallSyntax
//@[19:0025) |   ├─IdentifierSyntax
//@[19:0025) |   | └─Token(Identifier) |reduce|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0041) |   ├─FunctionArgumentSyntax
//@[26:0041) |   | └─ArraySyntax
//@[26:0027) |   |   ├─Token(LeftSquare) |[|
//@[27:0030) |   |   ├─ArrayItemSyntax
//@[27:0030) |   |   | └─StringSyntax
//@[27:0030) |   |   |   └─Token(StringComplete) |'a'|
//@[30:0031) |   |   ├─Token(Comma) |,|
//@[32:0035) |   |   ├─ArrayItemSyntax
//@[32:0035) |   |   | └─StringSyntax
//@[32:0035) |   |   |   └─Token(StringComplete) |'b'|
//@[35:0036) |   |   ├─Token(Comma) |,|
//@[37:0040) |   |   ├─ArrayItemSyntax
//@[37:0040) |   |   | └─StringSyntax
//@[37:0040) |   |   |   └─Token(StringComplete) |'c'|
//@[40:0041) |   |   └─Token(RightSquare) |]|
//@[41:0042) |   ├─Token(Comma) |,|
//@[43:0045) |   ├─FunctionArgumentSyntax
//@[43:0045) |   | └─StringSyntax
//@[43:0045) |   |   └─Token(StringComplete) |''|
//@[45:0046) |   ├─Token(Comma) |,|
//@[47:0078) |   ├─FunctionArgumentSyntax
//@[47:0078) |   | └─LambdaSyntax
//@[47:0053) |   |   ├─VariableBlockSyntax
//@[47:0048) |   |   | ├─Token(LeftParen) |(|
//@[48:0049) |   |   | ├─LocalVariableSyntax
//@[48:0049) |   |   | | └─IdentifierSyntax
//@[48:0049) |   |   | |   └─Token(Identifier) |a|
//@[49:0050) |   |   | ├─Token(Comma) |,|
//@[51:0052) |   |   | ├─LocalVariableSyntax
//@[51:0052) |   |   | | └─IdentifierSyntax
//@[51:0052) |   |   | |   └─Token(Identifier) |b|
//@[52:0053) |   |   | └─Token(RightParen) |)|
//@[54:0056) |   |   ├─Token(Arrow) |=>|
//@[57:0078) |   |   └─StringSyntax
//@[57:0060) |   |     ├─Token(StringLeftPiece) |'${|
//@[60:0071) |   |     ├─FunctionCallSyntax
//@[60:0068) |   |     | ├─IdentifierSyntax
//@[60:0068) |   |     | | └─Token(Identifier) |toObject|
//@[68:0069) |   |     | ├─Token(LeftParen) |(|
//@[69:0070) |   |     | ├─FunctionArgumentSyntax
//@[69:0070) |   |     | | └─VariableAccessSyntax
//@[69:0070) |   |     | |   └─IdentifierSyntax
//@[69:0070) |   |     | |     └─Token(Identifier) |a|
//@[70:0071) |   |     | └─Token(RightParen) |)|
//@[71:0075) |   |     ├─Token(StringMiddlePiece) |}-${|
//@[75:0076) |   |     ├─VariableAccessSyntax
//@[75:0076) |   |     | └─IdentifierSyntax
//@[75:0076) |   |     |   └─Token(Identifier) |b|
//@[76:0078) |   |     └─Token(StringRightPiece) |}'|
//@[78:0079) |   └─Token(RightParen) |)|
//@[79:0080) ├─Token(NewLine) |\n|
param testReplace = replace('abc', 'b', {})
//@[00:0043) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |testReplace|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0043) | └─FunctionCallSyntax
//@[20:0027) |   ├─IdentifierSyntax
//@[20:0027) |   | └─Token(Identifier) |replace|
//@[27:0028) |   ├─Token(LeftParen) |(|
//@[28:0033) |   ├─FunctionArgumentSyntax
//@[28:0033) |   | └─StringSyntax
//@[28:0033) |   |   └─Token(StringComplete) |'abc'|
//@[33:0034) |   ├─Token(Comma) |,|
//@[35:0038) |   ├─FunctionArgumentSyntax
//@[35:0038) |   | └─StringSyntax
//@[35:0038) |   |   └─Token(StringComplete) |'b'|
//@[38:0039) |   ├─Token(Comma) |,|
//@[40:0042) |   ├─FunctionArgumentSyntax
//@[40:0042) |   | └─ObjectSyntax
//@[40:0041) |   |   ├─Token(LeftBrace) |{|
//@[41:0042) |   |   └─Token(RightBrace) |}|
//@[42:0043) |   └─Token(RightParen) |)|
//@[43:0044) ├─Token(NewLine) |\n|
param testSkip = skip([1, 2, 3], '1')
//@[00:0037) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |testSkip|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0037) | └─FunctionCallSyntax
//@[17:0021) |   ├─IdentifierSyntax
//@[17:0021) |   | └─Token(Identifier) |skip|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0031) |   ├─FunctionArgumentSyntax
//@[22:0031) |   | └─ArraySyntax
//@[22:0023) |   |   ├─Token(LeftSquare) |[|
//@[23:0024) |   |   ├─ArrayItemSyntax
//@[23:0024) |   |   | └─IntegerLiteralSyntax
//@[23:0024) |   |   |   └─Token(Integer) |1|
//@[24:0025) |   |   ├─Token(Comma) |,|
//@[26:0027) |   |   ├─ArrayItemSyntax
//@[26:0027) |   |   | └─IntegerLiteralSyntax
//@[26:0027) |   |   |   └─Token(Integer) |2|
//@[27:0028) |   |   ├─Token(Comma) |,|
//@[29:0030) |   |   ├─ArrayItemSyntax
//@[29:0030) |   |   | └─IntegerLiteralSyntax
//@[29:0030) |   |   |   └─Token(Integer) |3|
//@[30:0031) |   |   └─Token(RightSquare) |]|
//@[31:0032) |   ├─Token(Comma) |,|
//@[33:0036) |   ├─FunctionArgumentSyntax
//@[33:0036) |   | └─StringSyntax
//@[33:0036) |   |   └─Token(StringComplete) |'1'|
//@[36:0037) |   └─Token(RightParen) |)|
//@[37:0038) ├─Token(NewLine) |\n|
param testSort = sort(['c', 'd', 'a'], (a, b) => a + b)
//@[00:0055) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |testSort|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0055) | └─FunctionCallSyntax
//@[17:0021) |   ├─IdentifierSyntax
//@[17:0021) |   | └─Token(Identifier) |sort|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0037) |   ├─FunctionArgumentSyntax
//@[22:0037) |   | └─ArraySyntax
//@[22:0023) |   |   ├─Token(LeftSquare) |[|
//@[23:0026) |   |   ├─ArrayItemSyntax
//@[23:0026) |   |   | └─StringSyntax
//@[23:0026) |   |   |   └─Token(StringComplete) |'c'|
//@[26:0027) |   |   ├─Token(Comma) |,|
//@[28:0031) |   |   ├─ArrayItemSyntax
//@[28:0031) |   |   | └─StringSyntax
//@[28:0031) |   |   |   └─Token(StringComplete) |'d'|
//@[31:0032) |   |   ├─Token(Comma) |,|
//@[33:0036) |   |   ├─ArrayItemSyntax
//@[33:0036) |   |   | └─StringSyntax
//@[33:0036) |   |   |   └─Token(StringComplete) |'a'|
//@[36:0037) |   |   └─Token(RightSquare) |]|
//@[37:0038) |   ├─Token(Comma) |,|
//@[39:0054) |   ├─FunctionArgumentSyntax
//@[39:0054) |   | └─LambdaSyntax
//@[39:0045) |   |   ├─VariableBlockSyntax
//@[39:0040) |   |   | ├─Token(LeftParen) |(|
//@[40:0041) |   |   | ├─LocalVariableSyntax
//@[40:0041) |   |   | | └─IdentifierSyntax
//@[40:0041) |   |   | |   └─Token(Identifier) |a|
//@[41:0042) |   |   | ├─Token(Comma) |,|
//@[43:0044) |   |   | ├─LocalVariableSyntax
//@[43:0044) |   |   | | └─IdentifierSyntax
//@[43:0044) |   |   | |   └─Token(Identifier) |b|
//@[44:0045) |   |   | └─Token(RightParen) |)|
//@[46:0048) |   |   ├─Token(Arrow) |=>|
//@[49:0054) |   |   └─BinaryOperationSyntax
//@[49:0050) |   |     ├─VariableAccessSyntax
//@[49:0050) |   |     | └─IdentifierSyntax
//@[49:0050) |   |     |   └─Token(Identifier) |a|
//@[51:0052) |   |     ├─Token(Plus) |+|
//@[53:0054) |   |     └─VariableAccessSyntax
//@[53:0054) |   |       └─IdentifierSyntax
//@[53:0054) |   |         └─Token(Identifier) |b|
//@[54:0055) |   └─Token(RightParen) |)|
//@[55:0056) ├─Token(NewLine) |\n|
param testSplit = split('a/b/c', 1 + 2)
//@[00:0039) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |testSplit|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0039) | └─FunctionCallSyntax
//@[18:0023) |   ├─IdentifierSyntax
//@[18:0023) |   | └─Token(Identifier) |split|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0031) |   ├─FunctionArgumentSyntax
//@[24:0031) |   | └─StringSyntax
//@[24:0031) |   |   └─Token(StringComplete) |'a/b/c'|
//@[31:0032) |   ├─Token(Comma) |,|
//@[33:0038) |   ├─FunctionArgumentSyntax
//@[33:0038) |   | └─BinaryOperationSyntax
//@[33:0034) |   |   ├─IntegerLiteralSyntax
//@[33:0034) |   |   | └─Token(Integer) |1|
//@[35:0036) |   |   ├─Token(Plus) |+|
//@[37:0038) |   |   └─IntegerLiteralSyntax
//@[37:0038) |   |     └─Token(Integer) |2|
//@[38:0039) |   └─Token(RightParen) |)|
//@[39:0040) ├─Token(NewLine) |\n|
param testStartsWith = startsWith('abc', {})
//@[00:0044) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0020) | ├─IdentifierSyntax
//@[06:0020) | | └─Token(Identifier) |testStartsWith|
//@[21:0022) | ├─Token(Assignment) |=|
//@[23:0044) | └─FunctionCallSyntax
//@[23:0033) |   ├─IdentifierSyntax
//@[23:0033) |   | └─Token(Identifier) |startsWith|
//@[33:0034) |   ├─Token(LeftParen) |(|
//@[34:0039) |   ├─FunctionArgumentSyntax
//@[34:0039) |   | └─StringSyntax
//@[34:0039) |   |   └─Token(StringComplete) |'abc'|
//@[39:0040) |   ├─Token(Comma) |,|
//@[41:0043) |   ├─FunctionArgumentSyntax
//@[41:0043) |   | └─ObjectSyntax
//@[41:0042) |   |   ├─Token(LeftBrace) |{|
//@[42:0043) |   |   └─Token(RightBrace) |}|
//@[43:0044) |   └─Token(RightParen) |)|
//@[44:0045) ├─Token(NewLine) |\n|
param testString = string({})
//@[00:0029) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |testString|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0029) | └─FunctionCallSyntax
//@[19:0025) |   ├─IdentifierSyntax
//@[19:0025) |   | └─Token(Identifier) |string|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0028) |   ├─FunctionArgumentSyntax
//@[26:0028) |   | └─ObjectSyntax
//@[26:0027) |   |   ├─Token(LeftBrace) |{|
//@[27:0028) |   |   └─Token(RightBrace) |}|
//@[28:0029) |   └─Token(RightParen) |)|
//@[29:0030) ├─Token(NewLine) |\n|
param testSubstring = substring('asdfasf', '3')
//@[00:0047) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0019) | ├─IdentifierSyntax
//@[06:0019) | | └─Token(Identifier) |testSubstring|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0047) | └─FunctionCallSyntax
//@[22:0031) |   ├─IdentifierSyntax
//@[22:0031) |   | └─Token(Identifier) |substring|
//@[31:0032) |   ├─Token(LeftParen) |(|
//@[32:0041) |   ├─FunctionArgumentSyntax
//@[32:0041) |   | └─StringSyntax
//@[32:0041) |   |   └─Token(StringComplete) |'asdfasf'|
//@[41:0042) |   ├─Token(Comma) |,|
//@[43:0046) |   ├─FunctionArgumentSyntax
//@[43:0046) |   | └─StringSyntax
//@[43:0046) |   |   └─Token(StringComplete) |'3'|
//@[46:0047) |   └─Token(RightParen) |)|
//@[47:0048) ├─Token(NewLine) |\n|
param testTake = take([1, 2, 3], '2')
//@[00:0037) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |testTake|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0037) | └─FunctionCallSyntax
//@[17:0021) |   ├─IdentifierSyntax
//@[17:0021) |   | └─Token(Identifier) |take|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0031) |   ├─FunctionArgumentSyntax
//@[22:0031) |   | └─ArraySyntax
//@[22:0023) |   |   ├─Token(LeftSquare) |[|
//@[23:0024) |   |   ├─ArrayItemSyntax
//@[23:0024) |   |   | └─IntegerLiteralSyntax
//@[23:0024) |   |   |   └─Token(Integer) |1|
//@[24:0025) |   |   ├─Token(Comma) |,|
//@[26:0027) |   |   ├─ArrayItemSyntax
//@[26:0027) |   |   | └─IntegerLiteralSyntax
//@[26:0027) |   |   |   └─Token(Integer) |2|
//@[27:0028) |   |   ├─Token(Comma) |,|
//@[29:0030) |   |   ├─ArrayItemSyntax
//@[29:0030) |   |   | └─IntegerLiteralSyntax
//@[29:0030) |   |   |   └─Token(Integer) |3|
//@[30:0031) |   |   └─Token(RightSquare) |]|
//@[31:0032) |   ├─Token(Comma) |,|
//@[33:0036) |   ├─FunctionArgumentSyntax
//@[33:0036) |   | └─StringSyntax
//@[33:0036) |   |   └─Token(StringComplete) |'2'|
//@[36:0037) |   └─Token(RightParen) |)|
//@[37:0038) ├─Token(NewLine) |\n|
param testToLower = toLower(123)
//@[00:0032) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |testToLower|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0032) | └─FunctionCallSyntax
//@[20:0027) |   ├─IdentifierSyntax
//@[20:0027) |   | └─Token(Identifier) |toLower|
//@[27:0028) |   ├─Token(LeftParen) |(|
//@[28:0031) |   ├─FunctionArgumentSyntax
//@[28:0031) |   | └─IntegerLiteralSyntax
//@[28:0031) |   |   └─Token(Integer) |123|
//@[31:0032) |   └─Token(RightParen) |)|
//@[32:0033) ├─Token(NewLine) |\n|
param testToObject = toObject(['a', 'b', 'c'], x => {x: x}, x => 'Hi ${x}!')
//@[00:0076) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |testToObject|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0076) | └─FunctionCallSyntax
//@[21:0029) |   ├─IdentifierSyntax
//@[21:0029) |   | └─Token(Identifier) |toObject|
//@[29:0030) |   ├─Token(LeftParen) |(|
//@[30:0045) |   ├─FunctionArgumentSyntax
//@[30:0045) |   | └─ArraySyntax
//@[30:0031) |   |   ├─Token(LeftSquare) |[|
//@[31:0034) |   |   ├─ArrayItemSyntax
//@[31:0034) |   |   | └─StringSyntax
//@[31:0034) |   |   |   └─Token(StringComplete) |'a'|
//@[34:0035) |   |   ├─Token(Comma) |,|
//@[36:0039) |   |   ├─ArrayItemSyntax
//@[36:0039) |   |   | └─StringSyntax
//@[36:0039) |   |   |   └─Token(StringComplete) |'b'|
//@[39:0040) |   |   ├─Token(Comma) |,|
//@[41:0044) |   |   ├─ArrayItemSyntax
//@[41:0044) |   |   | └─StringSyntax
//@[41:0044) |   |   |   └─Token(StringComplete) |'c'|
//@[44:0045) |   |   └─Token(RightSquare) |]|
//@[45:0046) |   ├─Token(Comma) |,|
//@[47:0058) |   ├─FunctionArgumentSyntax
//@[47:0058) |   | └─LambdaSyntax
//@[47:0048) |   |   ├─LocalVariableSyntax
//@[47:0048) |   |   | └─IdentifierSyntax
//@[47:0048) |   |   |   └─Token(Identifier) |x|
//@[49:0051) |   |   ├─Token(Arrow) |=>|
//@[52:0058) |   |   └─ObjectSyntax
//@[52:0053) |   |     ├─Token(LeftBrace) |{|
//@[53:0057) |   |     ├─ObjectPropertySyntax
//@[53:0054) |   |     | ├─IdentifierSyntax
//@[53:0054) |   |     | | └─Token(Identifier) |x|
//@[54:0055) |   |     | ├─Token(Colon) |:|
//@[56:0057) |   |     | └─VariableAccessSyntax
//@[56:0057) |   |     |   └─IdentifierSyntax
//@[56:0057) |   |     |     └─Token(Identifier) |x|
//@[57:0058) |   |     └─Token(RightBrace) |}|
//@[58:0059) |   ├─Token(Comma) |,|
//@[60:0075) |   ├─FunctionArgumentSyntax
//@[60:0075) |   | └─LambdaSyntax
//@[60:0061) |   |   ├─LocalVariableSyntax
//@[60:0061) |   |   | └─IdentifierSyntax
//@[60:0061) |   |   |   └─Token(Identifier) |x|
//@[62:0064) |   |   ├─Token(Arrow) |=>|
//@[65:0075) |   |   └─StringSyntax
//@[65:0071) |   |     ├─Token(StringLeftPiece) |'Hi ${|
//@[71:0072) |   |     ├─VariableAccessSyntax
//@[71:0072) |   |     | └─IdentifierSyntax
//@[71:0072) |   |     |   └─Token(Identifier) |x|
//@[72:0075) |   |     └─Token(StringRightPiece) |}!'|
//@[75:0076) |   └─Token(RightParen) |)|
//@[76:0077) ├─Token(NewLine) |\n|
param testToUpper = toUpper([123])
//@[00:0034) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |testToUpper|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0034) | └─FunctionCallSyntax
//@[20:0027) |   ├─IdentifierSyntax
//@[20:0027) |   | └─Token(Identifier) |toUpper|
//@[27:0028) |   ├─Token(LeftParen) |(|
//@[28:0033) |   ├─FunctionArgumentSyntax
//@[28:0033) |   | └─ArraySyntax
//@[28:0029) |   |   ├─Token(LeftSquare) |[|
//@[29:0032) |   |   ├─ArrayItemSyntax
//@[29:0032) |   |   | └─IntegerLiteralSyntax
//@[29:0032) |   |   |   └─Token(Integer) |123|
//@[32:0033) |   |   └─Token(RightSquare) |]|
//@[33:0034) |   └─Token(RightParen) |)|
//@[34:0035) ├─Token(NewLine) |\n|
param testTrim = trim(123)
//@[00:0026) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |testTrim|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0026) | └─FunctionCallSyntax
//@[17:0021) |   ├─IdentifierSyntax
//@[17:0021) |   | └─Token(Identifier) |trim|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0025) |   ├─FunctionArgumentSyntax
//@[22:0025) |   | └─IntegerLiteralSyntax
//@[22:0025) |   |   └─Token(Integer) |123|
//@[25:0026) |   └─Token(RightParen) |)|
//@[26:0027) ├─Token(NewLine) |\n|
param testUnion = union({ abc: 'def' }, [123])
//@[00:0046) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |testUnion|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0046) | └─FunctionCallSyntax
//@[18:0023) |   ├─IdentifierSyntax
//@[18:0023) |   | └─Token(Identifier) |union|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0038) |   ├─FunctionArgumentSyntax
//@[24:0038) |   | └─ObjectSyntax
//@[24:0025) |   |   ├─Token(LeftBrace) |{|
//@[26:0036) |   |   ├─ObjectPropertySyntax
//@[26:0029) |   |   | ├─IdentifierSyntax
//@[26:0029) |   |   | | └─Token(Identifier) |abc|
//@[29:0030) |   |   | ├─Token(Colon) |:|
//@[31:0036) |   |   | └─StringSyntax
//@[31:0036) |   |   |   └─Token(StringComplete) |'def'|
//@[37:0038) |   |   └─Token(RightBrace) |}|
//@[38:0039) |   ├─Token(Comma) |,|
//@[40:0045) |   ├─FunctionArgumentSyntax
//@[40:0045) |   | └─ArraySyntax
//@[40:0041) |   |   ├─Token(LeftSquare) |[|
//@[41:0044) |   |   ├─ArrayItemSyntax
//@[41:0044) |   |   | └─IntegerLiteralSyntax
//@[41:0044) |   |   |   └─Token(Integer) |123|
//@[44:0045) |   |   └─Token(RightSquare) |]|
//@[45:0046) |   └─Token(RightParen) |)|
//@[46:0047) ├─Token(NewLine) |\n|
param testUniqueString = uniqueString('asd', 'asdf', 'asdf')
//@[00:0060) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0022) | ├─IdentifierSyntax
//@[06:0022) | | └─Token(Identifier) |testUniqueString|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0060) | └─FunctionCallSyntax
//@[25:0037) |   ├─IdentifierSyntax
//@[25:0037) |   | └─Token(Identifier) |uniqueString|
//@[37:0038) |   ├─Token(LeftParen) |(|
//@[38:0043) |   ├─FunctionArgumentSyntax
//@[38:0043) |   | └─StringSyntax
//@[38:0043) |   |   └─Token(StringComplete) |'asd'|
//@[43:0044) |   ├─Token(Comma) |,|
//@[45:0051) |   ├─FunctionArgumentSyntax
//@[45:0051) |   | └─StringSyntax
//@[45:0051) |   |   └─Token(StringComplete) |'asdf'|
//@[51:0052) |   ├─Token(Comma) |,|
//@[53:0059) |   ├─FunctionArgumentSyntax
//@[53:0059) |   | └─StringSyntax
//@[53:0059) |   |   └─Token(StringComplete) |'asdf'|
//@[59:0060) |   └─Token(RightParen) |)|
//@[60:0061) ├─Token(NewLine) |\n|
param testUri = uri('github.com', 'Azure/bicep')
//@[00:0048) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |testUri|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0048) | └─FunctionCallSyntax
//@[16:0019) |   ├─IdentifierSyntax
//@[16:0019) |   | └─Token(Identifier) |uri|
//@[19:0020) |   ├─Token(LeftParen) |(|
//@[20:0032) |   ├─FunctionArgumentSyntax
//@[20:0032) |   | └─StringSyntax
//@[20:0032) |   |   └─Token(StringComplete) |'github.com'|
//@[32:0033) |   ├─Token(Comma) |,|
//@[34:0047) |   ├─FunctionArgumentSyntax
//@[34:0047) |   | └─StringSyntax
//@[34:0047) |   |   └─Token(StringComplete) |'Azure/bicep'|
//@[47:0048) |   └─Token(RightParen) |)|
//@[48:0049) ├─Token(NewLine) |\n|
param testUriComponent = uriComponent(123)
//@[00:0042) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0022) | ├─IdentifierSyntax
//@[06:0022) | | └─Token(Identifier) |testUriComponent|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0042) | └─FunctionCallSyntax
//@[25:0037) |   ├─IdentifierSyntax
//@[25:0037) |   | └─Token(Identifier) |uriComponent|
//@[37:0038) |   ├─Token(LeftParen) |(|
//@[38:0041) |   ├─FunctionArgumentSyntax
//@[38:0041) |   | └─IntegerLiteralSyntax
//@[38:0041) |   |   └─Token(Integer) |123|
//@[41:0042) |   └─Token(RightParen) |)|
//@[42:0043) ├─Token(NewLine) |\n|
param testUriComponentToString = uriComponentToString({})
//@[00:0057) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0030) | ├─IdentifierSyntax
//@[06:0030) | | └─Token(Identifier) |testUriComponentToString|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0057) | └─FunctionCallSyntax
//@[33:0053) |   ├─IdentifierSyntax
//@[33:0053) |   | └─Token(Identifier) |uriComponentToString|
//@[53:0054) |   ├─Token(LeftParen) |(|
//@[54:0056) |   ├─FunctionArgumentSyntax
//@[54:0056) |   | └─ObjectSyntax
//@[54:0055) |   |   ├─Token(LeftBrace) |{|
//@[55:0056) |   |   └─Token(RightBrace) |}|
//@[56:0057) |   └─Token(RightParen) |)|
//@[57:0059) ├─Token(NewLine) |\n\n|

param myObj = {
//@[00:0249) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0011) | ├─IdentifierSyntax
//@[06:0011) | | └─Token(Identifier) |myObj|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0249) | └─ObjectSyntax
//@[14:0015) |   ├─Token(LeftBrace) |{|
//@[15:0016) |   ├─Token(NewLine) |\n|
  newGuid: newGuid()
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |newGuid|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0020) |   | └─FunctionCallSyntax
//@[11:0018) |   |   ├─IdentifierSyntax
//@[11:0018) |   |   | └─Token(Identifier) |newGuid|
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0020) |   |   └─Token(RightParen) |)|
//@[20:0021) |   ├─Token(NewLine) |\n|
  utcNow: utcNow()
//@[02:0018) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |utcNow|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0018) |   | └─FunctionCallSyntax
//@[10:0016) |   |   ├─IdentifierSyntax
//@[10:0016) |   |   | └─Token(Identifier) |utcNow|
//@[16:0017) |   |   ├─Token(LeftParen) |(|
//@[17:0018) |   |   └─Token(RightParen) |)|
//@[18:0019) |   ├─Token(NewLine) |\n|
  resourceId: resourceId('Microsoft.ContainerService/managedClusters', 'blah')
//@[02:0078) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |resourceId|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0078) |   | └─FunctionCallSyntax
//@[14:0024) |   |   ├─IdentifierSyntax
//@[14:0024) |   |   | └─Token(Identifier) |resourceId|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0069) |   |   ├─FunctionArgumentSyntax
//@[25:0069) |   |   | └─StringSyntax
//@[25:0069) |   |   |   └─Token(StringComplete) |'Microsoft.ContainerService/managedClusters'|
//@[69:0070) |   |   ├─Token(Comma) |,|
//@[71:0077) |   |   ├─FunctionArgumentSyntax
//@[71:0077) |   |   | └─StringSyntax
//@[71:0077) |   |   |   └─Token(StringComplete) |'blah'|
//@[77:0078) |   |   └─Token(RightParen) |)|
//@[78:0079) |   ├─Token(NewLine) |\n|
  deployment: deployment()
//@[02:0026) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |deployment|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0026) |   | └─FunctionCallSyntax
//@[14:0024) |   |   ├─IdentifierSyntax
//@[14:0024) |   |   | └─Token(Identifier) |deployment|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0026) |   |   └─Token(RightParen) |)|
//@[26:0027) |   ├─Token(NewLine) |\n|
  environment: environment()
//@[02:0028) |   ├─ObjectPropertySyntax
//@[02:0013) |   | ├─IdentifierSyntax
//@[02:0013) |   | | └─Token(Identifier) |environment|
//@[13:0014) |   | ├─Token(Colon) |:|
//@[15:0028) |   | └─FunctionCallSyntax
//@[15:0026) |   |   ├─IdentifierSyntax
//@[15:0026) |   |   | └─Token(Identifier) |environment|
//@[26:0027) |   |   ├─Token(LeftParen) |(|
//@[27:0028) |   |   └─Token(RightParen) |)|
//@[28:0029) |   ├─Token(NewLine) |\n|
  azNs: az
//@[02:0010) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |azNs|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0010) |   | └─VariableAccessSyntax
//@[08:0010) |   |   └─IdentifierSyntax
//@[08:0010) |   |     └─Token(Identifier) |az|
//@[10:0011) |   ├─Token(NewLine) |\n|
  azNsFunc: az.providers('Microsoft.Compute')
//@[02:0045) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |azNsFunc|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0045) |   | └─InstanceFunctionCallSyntax
//@[12:0014) |   |   ├─VariableAccessSyntax
//@[12:0014) |   |   | └─IdentifierSyntax
//@[12:0014) |   |   |   └─Token(Identifier) |az|
//@[14:0015) |   |   ├─Token(Dot) |.|
//@[15:0024) |   |   ├─IdentifierSyntax
//@[15:0024) |   |   | └─Token(Identifier) |providers|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0044) |   |   ├─FunctionArgumentSyntax
//@[25:0044) |   |   | └─StringSyntax
//@[25:0044) |   |   |   └─Token(StringComplete) |'Microsoft.Compute'|
//@[44:0045) |   |   └─Token(RightParen) |)|
//@[45:0046) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0001) └─Token(EndOfFile) ||
