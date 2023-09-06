var textLoadDirectory = loadTextContent('Assets/path/to/nothing')
//@[00:4068) ProgramSyntax
//@[00:0065) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |textLoadDirectory|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0065) | └─FunctionCallSyntax
//@[24:0039) |   ├─IdentifierSyntax
//@[24:0039) |   | └─Token(Identifier) |loadTextContent|
//@[39:0040) |   ├─Token(LeftParen) |(|
//@[40:0064) |   ├─FunctionArgumentSyntax
//@[40:0064) |   | └─StringSyntax
//@[40:0064) |   |   └─Token(StringComplete) |'Assets/path/to/nothing'|
//@[64:0065) |   └─Token(RightParen) |)|
//@[65:0067) ├─Token(NewLine) |\r\n|
var binaryLoadDirectory = loadFileAsBase64('Assets/path/to/nothing')
//@[00:0068) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0023) | ├─IdentifierSyntax
//@[04:0023) | | └─Token(Identifier) |binaryLoadDirectory|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0068) | └─FunctionCallSyntax
//@[26:0042) |   ├─IdentifierSyntax
//@[26:0042) |   | └─Token(Identifier) |loadFileAsBase64|
//@[42:0043) |   ├─Token(LeftParen) |(|
//@[43:0067) |   ├─FunctionArgumentSyntax
//@[43:0067) |   | └─StringSyntax
//@[43:0067) |   |   └─Token(StringComplete) |'Assets/path/to/nothing'|
//@[67:0068) |   └─Token(RightParen) |)|
//@[68:0072) ├─Token(NewLine) |\r\n\r\n|

var textLoadFileMissing = loadTextContent('Assets/nothing.file')
//@[00:0064) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0023) | ├─IdentifierSyntax
//@[04:0023) | | └─Token(Identifier) |textLoadFileMissing|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0064) | └─FunctionCallSyntax
//@[26:0041) |   ├─IdentifierSyntax
//@[26:0041) |   | └─Token(Identifier) |loadTextContent|
//@[41:0042) |   ├─Token(LeftParen) |(|
//@[42:0063) |   ├─FunctionArgumentSyntax
//@[42:0063) |   | └─StringSyntax
//@[42:0063) |   |   └─Token(StringComplete) |'Assets/nothing.file'|
//@[63:0064) |   └─Token(RightParen) |)|
//@[64:0066) ├─Token(NewLine) |\r\n|
var binaryLoadFileMissing = loadFileAsBase64('Assets/nothing.file')
//@[00:0067) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0025) | ├─IdentifierSyntax
//@[04:0025) | | └─Token(Identifier) |binaryLoadFileMissing|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0067) | └─FunctionCallSyntax
//@[28:0044) |   ├─IdentifierSyntax
//@[28:0044) |   | └─Token(Identifier) |loadFileAsBase64|
//@[44:0045) |   ├─Token(LeftParen) |(|
//@[45:0066) |   ├─FunctionArgumentSyntax
//@[45:0066) |   | └─StringSyntax
//@[45:0066) |   |   └─Token(StringComplete) |'Assets/nothing.file'|
//@[66:0067) |   └─Token(RightParen) |)|
//@[67:0071) ├─Token(NewLine) |\r\n\r\n|

var textLoadFilePathEmpty = loadTextContent('')
//@[00:0047) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0025) | ├─IdentifierSyntax
//@[04:0025) | | └─Token(Identifier) |textLoadFilePathEmpty|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0047) | └─FunctionCallSyntax
//@[28:0043) |   ├─IdentifierSyntax
//@[28:0043) |   | └─Token(Identifier) |loadTextContent|
//@[43:0044) |   ├─Token(LeftParen) |(|
//@[44:0046) |   ├─FunctionArgumentSyntax
//@[44:0046) |   | └─StringSyntax
//@[44:0046) |   |   └─Token(StringComplete) |''|
//@[46:0047) |   └─Token(RightParen) |)|
//@[47:0049) ├─Token(NewLine) |\r\n|
var binaryLoadFilePathEmpty = loadFileAsBase64('')
//@[00:0050) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |binaryLoadFilePathEmpty|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0050) | └─FunctionCallSyntax
//@[30:0046) |   ├─IdentifierSyntax
//@[30:0046) |   | └─Token(Identifier) |loadFileAsBase64|
//@[46:0047) |   ├─Token(LeftParen) |(|
//@[47:0049) |   ├─FunctionArgumentSyntax
//@[47:0049) |   | └─StringSyntax
//@[47:0049) |   |   └─Token(StringComplete) |''|
//@[49:0050) |   └─Token(RightParen) |)|
//@[50:0054) ├─Token(NewLine) |\r\n\r\n|

var textLoadInvalidCharactersPath1 = loadTextContent('Assets\\TextFile.txt')
//@[00:0076) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0034) | ├─IdentifierSyntax
//@[04:0034) | | └─Token(Identifier) |textLoadInvalidCharactersPath1|
//@[35:0036) | ├─Token(Assignment) |=|
//@[37:0076) | └─FunctionCallSyntax
//@[37:0052) |   ├─IdentifierSyntax
//@[37:0052) |   | └─Token(Identifier) |loadTextContent|
//@[52:0053) |   ├─Token(LeftParen) |(|
//@[53:0075) |   ├─FunctionArgumentSyntax
//@[53:0075) |   | └─StringSyntax
//@[53:0075) |   |   └─Token(StringComplete) |'Assets\\TextFile.txt'|
//@[75:0076) |   └─Token(RightParen) |)|
//@[76:0078) ├─Token(NewLine) |\r\n|
var binaryLoadInvalidCharactersPath1 = loadFileAsBase64('Assets\\binary')
//@[00:0073) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0036) | ├─IdentifierSyntax
//@[04:0036) | | └─Token(Identifier) |binaryLoadInvalidCharactersPath1|
//@[37:0038) | ├─Token(Assignment) |=|
//@[39:0073) | └─FunctionCallSyntax
//@[39:0055) |   ├─IdentifierSyntax
//@[39:0055) |   | └─Token(Identifier) |loadFileAsBase64|
//@[55:0056) |   ├─Token(LeftParen) |(|
//@[56:0072) |   ├─FunctionArgumentSyntax
//@[56:0072) |   | └─StringSyntax
//@[56:0072) |   |   └─Token(StringComplete) |'Assets\\binary'|
//@[72:0073) |   └─Token(RightParen) |)|
//@[73:0077) ├─Token(NewLine) |\r\n\r\n|

var textLoadInvalidCharactersPath2 = loadTextContent('/Assets/TextFile.txt')
//@[00:0076) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0034) | ├─IdentifierSyntax
//@[04:0034) | | └─Token(Identifier) |textLoadInvalidCharactersPath2|
//@[35:0036) | ├─Token(Assignment) |=|
//@[37:0076) | └─FunctionCallSyntax
//@[37:0052) |   ├─IdentifierSyntax
//@[37:0052) |   | └─Token(Identifier) |loadTextContent|
//@[52:0053) |   ├─Token(LeftParen) |(|
//@[53:0075) |   ├─FunctionArgumentSyntax
//@[53:0075) |   | └─StringSyntax
//@[53:0075) |   |   └─Token(StringComplete) |'/Assets/TextFile.txt'|
//@[75:0076) |   └─Token(RightParen) |)|
//@[76:0078) ├─Token(NewLine) |\r\n|
var binaryLoadInvalidCharactersPath2 = loadFileAsBase64('/Assets/binary')
//@[00:0073) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0036) | ├─IdentifierSyntax
//@[04:0036) | | └─Token(Identifier) |binaryLoadInvalidCharactersPath2|
//@[37:0038) | ├─Token(Assignment) |=|
//@[39:0073) | └─FunctionCallSyntax
//@[39:0055) |   ├─IdentifierSyntax
//@[39:0055) |   | └─Token(Identifier) |loadFileAsBase64|
//@[55:0056) |   ├─Token(LeftParen) |(|
//@[56:0072) |   ├─FunctionArgumentSyntax
//@[56:0072) |   | └─StringSyntax
//@[56:0072) |   |   └─Token(StringComplete) |'/Assets/binary'|
//@[72:0073) |   └─Token(RightParen) |)|
//@[73:0077) ├─Token(NewLine) |\r\n\r\n|

var textLoadInvalidCharactersPath3 = loadTextContent('file://Assets/TextFile.txt')
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0034) | ├─IdentifierSyntax
//@[04:0034) | | └─Token(Identifier) |textLoadInvalidCharactersPath3|
//@[35:0036) | ├─Token(Assignment) |=|
//@[37:0082) | └─FunctionCallSyntax
//@[37:0052) |   ├─IdentifierSyntax
//@[37:0052) |   | └─Token(Identifier) |loadTextContent|
//@[52:0053) |   ├─Token(LeftParen) |(|
//@[53:0081) |   ├─FunctionArgumentSyntax
//@[53:0081) |   | └─StringSyntax
//@[53:0081) |   |   └─Token(StringComplete) |'file://Assets/TextFile.txt'|
//@[81:0082) |   └─Token(RightParen) |)|
//@[82:0084) ├─Token(NewLine) |\r\n|
var binaryLoadInvalidCharactersPath3 = loadFileAsBase64('file://Assets/binary')
//@[00:0079) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0036) | ├─IdentifierSyntax
//@[04:0036) | | └─Token(Identifier) |binaryLoadInvalidCharactersPath3|
//@[37:0038) | ├─Token(Assignment) |=|
//@[39:0079) | └─FunctionCallSyntax
//@[39:0055) |   ├─IdentifierSyntax
//@[39:0055) |   | └─Token(Identifier) |loadFileAsBase64|
//@[55:0056) |   ├─Token(LeftParen) |(|
//@[56:0078) |   ├─FunctionArgumentSyntax
//@[56:0078) |   | └─StringSyntax
//@[56:0078) |   |   └─Token(StringComplete) |'file://Assets/binary'|
//@[78:0079) |   └─Token(RightParen) |)|
//@[79:0085) ├─Token(NewLine) |\r\n\r\n\r\n|


var textLoadUnsupportedEncoding = loadTextContent('Assets/TextFile.txt', 'windows-1250')
//@[00:0088) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0031) | ├─IdentifierSyntax
//@[04:0031) | | └─Token(Identifier) |textLoadUnsupportedEncoding|
//@[32:0033) | ├─Token(Assignment) |=|
//@[34:0088) | └─FunctionCallSyntax
//@[34:0049) |   ├─IdentifierSyntax
//@[34:0049) |   | └─Token(Identifier) |loadTextContent|
//@[49:0050) |   ├─Token(LeftParen) |(|
//@[50:0071) |   ├─FunctionArgumentSyntax
//@[50:0071) |   | └─StringSyntax
//@[50:0071) |   |   └─Token(StringComplete) |'Assets/TextFile.txt'|
//@[71:0072) |   ├─Token(Comma) |,|
//@[73:0087) |   ├─FunctionArgumentSyntax
//@[73:0087) |   | └─StringSyntax
//@[73:0087) |   |   └─Token(StringComplete) |'windows-1250'|
//@[87:0088) |   └─Token(RightParen) |)|
//@[88:0092) ├─Token(NewLine) |\r\n\r\n|

var textLoadWrongEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'us-ascii')
//@[00:0084) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding01|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0084) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0071) |   ├─FunctionArgumentSyntax
//@[46:0071) |   | └─StringSyntax
//@[46:0071) |   |   └─Token(StringComplete) |'Assets/encoding-iso.txt'|
//@[71:0072) |   ├─Token(Comma) |,|
//@[73:0083) |   ├─FunctionArgumentSyntax
//@[73:0083) |   | └─StringSyntax
//@[73:0083) |   |   └─Token(StringComplete) |'us-ascii'|
//@[83:0084) |   └─Token(RightParen) |)|
//@[84:0086) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding02 = loadTextContent('Assets/encoding-iso.txt', 'utf-8')
//@[00:0081) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding02|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0081) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0071) |   ├─FunctionArgumentSyntax
//@[46:0071) |   | └─StringSyntax
//@[46:0071) |   |   └─Token(StringComplete) |'Assets/encoding-iso.txt'|
//@[71:0072) |   ├─Token(Comma) |,|
//@[73:0080) |   ├─FunctionArgumentSyntax
//@[73:0080) |   | └─StringSyntax
//@[73:0080) |   |   └─Token(StringComplete) |'utf-8'|
//@[80:0081) |   └─Token(RightParen) |)|
//@[81:0083) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding03 = loadTextContent('Assets/encoding-iso.txt', 'utf-16BE')
//@[00:0084) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding03|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0084) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0071) |   ├─FunctionArgumentSyntax
//@[46:0071) |   | └─StringSyntax
//@[46:0071) |   |   └─Token(StringComplete) |'Assets/encoding-iso.txt'|
//@[71:0072) |   ├─Token(Comma) |,|
//@[73:0083) |   ├─FunctionArgumentSyntax
//@[73:0083) |   | └─StringSyntax
//@[73:0083) |   |   └─Token(StringComplete) |'utf-16BE'|
//@[83:0084) |   └─Token(RightParen) |)|
//@[84:0086) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding04 = loadTextContent('Assets/encoding-iso.txt', 'utf-16')
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding04|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0082) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0071) |   ├─FunctionArgumentSyntax
//@[46:0071) |   | └─StringSyntax
//@[46:0071) |   |   └─Token(StringComplete) |'Assets/encoding-iso.txt'|
//@[71:0072) |   ├─Token(Comma) |,|
//@[73:0081) |   ├─FunctionArgumentSyntax
//@[73:0081) |   | └─StringSyntax
//@[73:0081) |   |   └─Token(StringComplete) |'utf-16'|
//@[81:0082) |   └─Token(RightParen) |)|
//@[82:0084) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding05 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[00:0088) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding05|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0088) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0073) |   ├─FunctionArgumentSyntax
//@[46:0073) |   | └─StringSyntax
//@[46:0073) |   |   └─Token(StringComplete) |'Assets/encoding-ascii.txt'|
//@[73:0074) |   ├─Token(Comma) |,|
//@[75:0087) |   ├─FunctionArgumentSyntax
//@[75:0087) |   | └─StringSyntax
//@[75:0087) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[87:0088) |   └─Token(RightParen) |)|
//@[88:0090) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[00:0083) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding06|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0083) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0073) |   ├─FunctionArgumentSyntax
//@[46:0073) |   | └─StringSyntax
//@[46:0073) |   |   └─Token(StringComplete) |'Assets/encoding-ascii.txt'|
//@[73:0074) |   ├─Token(Comma) |,|
//@[75:0082) |   ├─FunctionArgumentSyntax
//@[75:0082) |   | └─StringSyntax
//@[75:0082) |   |   └─Token(StringComplete) |'utf-8'|
//@[82:0083) |   └─Token(RightParen) |)|
//@[83:0085) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'utf-16BE')
//@[00:0086) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding07|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0086) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0073) |   ├─FunctionArgumentSyntax
//@[46:0073) |   | └─StringSyntax
//@[46:0073) |   |   └─Token(StringComplete) |'Assets/encoding-ascii.txt'|
//@[73:0074) |   ├─Token(Comma) |,|
//@[75:0085) |   ├─FunctionArgumentSyntax
//@[75:0085) |   | └─StringSyntax
//@[75:0085) |   |   └─Token(StringComplete) |'utf-16BE'|
//@[85:0086) |   └─Token(RightParen) |)|
//@[86:0088) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-16')
//@[00:0084) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding08|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0084) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0073) |   ├─FunctionArgumentSyntax
//@[46:0073) |   | └─StringSyntax
//@[46:0073) |   |   └─Token(StringComplete) |'Assets/encoding-ascii.txt'|
//@[73:0074) |   ├─Token(Comma) |,|
//@[75:0083) |   ├─FunctionArgumentSyntax
//@[75:0083) |   | └─StringSyntax
//@[75:0083) |   |   └─Token(StringComplete) |'utf-16'|
//@[83:0084) |   └─Token(RightParen) |)|
//@[84:0086) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding09 = loadTextContent('Assets/encoding-utf16.txt', 'iso-8859-1')
//@[00:0088) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding09|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0088) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0073) |   ├─FunctionArgumentSyntax
//@[46:0073) |   | └─StringSyntax
//@[46:0073) |   |   └─Token(StringComplete) |'Assets/encoding-utf16.txt'|
//@[73:0074) |   ├─Token(Comma) |,|
//@[75:0087) |   ├─FunctionArgumentSyntax
//@[75:0087) |   | └─StringSyntax
//@[75:0087) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[87:0088) |   └─Token(RightParen) |)|
//@[88:0090) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding10 = loadTextContent('Assets/encoding-utf16.txt', 'utf-8')
//@[00:0083) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding10|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0083) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0073) |   ├─FunctionArgumentSyntax
//@[46:0073) |   | └─StringSyntax
//@[46:0073) |   |   └─Token(StringComplete) |'Assets/encoding-utf16.txt'|
//@[73:0074) |   ├─Token(Comma) |,|
//@[75:0082) |   ├─FunctionArgumentSyntax
//@[75:0082) |   | └─StringSyntax
//@[75:0082) |   |   └─Token(StringComplete) |'utf-8'|
//@[82:0083) |   └─Token(RightParen) |)|
//@[83:0085) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding11 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16BE')
//@[00:0086) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding11|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0086) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0073) |   ├─FunctionArgumentSyntax
//@[46:0073) |   | └─StringSyntax
//@[46:0073) |   |   └─Token(StringComplete) |'Assets/encoding-utf16.txt'|
//@[73:0074) |   ├─Token(Comma) |,|
//@[75:0085) |   ├─FunctionArgumentSyntax
//@[75:0085) |   | └─StringSyntax
//@[75:0085) |   |   └─Token(StringComplete) |'utf-16BE'|
//@[85:0086) |   └─Token(RightParen) |)|
//@[86:0088) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding12 = loadTextContent('Assets/encoding-utf16.txt', 'us-ascii')
//@[00:0086) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding12|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0086) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0073) |   ├─FunctionArgumentSyntax
//@[46:0073) |   | └─StringSyntax
//@[46:0073) |   |   └─Token(StringComplete) |'Assets/encoding-utf16.txt'|
//@[73:0074) |   ├─Token(Comma) |,|
//@[75:0085) |   ├─FunctionArgumentSyntax
//@[75:0085) |   | └─StringSyntax
//@[75:0085) |   |   └─Token(StringComplete) |'us-ascii'|
//@[85:0086) |   └─Token(RightParen) |)|
//@[86:0088) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding13 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16')
//@[00:0086) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding13|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0086) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0075) |   ├─FunctionArgumentSyntax
//@[46:0075) |   | └─StringSyntax
//@[46:0075) |   |   └─Token(StringComplete) |'Assets/encoding-utf16be.txt'|
//@[75:0076) |   ├─Token(Comma) |,|
//@[77:0085) |   ├─FunctionArgumentSyntax
//@[77:0085) |   | └─StringSyntax
//@[77:0085) |   |   └─Token(StringComplete) |'utf-16'|
//@[85:0086) |   └─Token(RightParen) |)|
//@[86:0088) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding14 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-8')
//@[00:0085) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding14|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0085) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0075) |   ├─FunctionArgumentSyntax
//@[46:0075) |   | └─StringSyntax
//@[46:0075) |   |   └─Token(StringComplete) |'Assets/encoding-utf16be.txt'|
//@[75:0076) |   ├─Token(Comma) |,|
//@[77:0084) |   ├─FunctionArgumentSyntax
//@[77:0084) |   | └─StringSyntax
//@[77:0084) |   |   └─Token(StringComplete) |'utf-8'|
//@[84:0085) |   └─Token(RightParen) |)|
//@[85:0087) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding15 = loadTextContent('Assets/encoding-utf16be.txt', 'us-ascii')
//@[00:0088) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding15|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0088) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0075) |   ├─FunctionArgumentSyntax
//@[46:0075) |   | └─StringSyntax
//@[46:0075) |   |   └─Token(StringComplete) |'Assets/encoding-utf16be.txt'|
//@[75:0076) |   ├─Token(Comma) |,|
//@[77:0087) |   ├─FunctionArgumentSyntax
//@[77:0087) |   | └─StringSyntax
//@[77:0087) |   |   └─Token(StringComplete) |'us-ascii'|
//@[87:0088) |   └─Token(RightParen) |)|
//@[88:0090) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding16 = loadTextContent('Assets/encoding-utf16be.txt', 'iso-8859-1')
//@[00:0090) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding16|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0090) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0075) |   ├─FunctionArgumentSyntax
//@[46:0075) |   | └─StringSyntax
//@[46:0075) |   |   └─Token(StringComplete) |'Assets/encoding-utf16be.txt'|
//@[75:0076) |   ├─Token(Comma) |,|
//@[77:0089) |   ├─FunctionArgumentSyntax
//@[77:0089) |   | └─StringSyntax
//@[77:0089) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[89:0090) |   └─Token(RightParen) |)|
//@[90:0092) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding17 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-16BE')
//@[00:0092) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding17|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0092) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0079) |   ├─FunctionArgumentSyntax
//@[46:0079) |   | └─StringSyntax
//@[46:0079) |   |   └─Token(StringComplete) |'Assets/encoding-windows1250.txt'|
//@[79:0080) |   ├─Token(Comma) |,|
//@[81:0091) |   ├─FunctionArgumentSyntax
//@[81:0091) |   | └─StringSyntax
//@[81:0091) |   |   └─Token(StringComplete) |'utf-16BE'|
//@[91:0092) |   └─Token(RightParen) |)|
//@[92:0094) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding18 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-16')
//@[00:0090) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding18|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0090) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0079) |   ├─FunctionArgumentSyntax
//@[46:0079) |   | └─StringSyntax
//@[46:0079) |   |   └─Token(StringComplete) |'Assets/encoding-windows1250.txt'|
//@[79:0080) |   ├─Token(Comma) |,|
//@[81:0089) |   ├─FunctionArgumentSyntax
//@[81:0089) |   | └─StringSyntax
//@[81:0089) |   |   └─Token(StringComplete) |'utf-16'|
//@[89:0090) |   └─Token(RightParen) |)|
//@[90:0092) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding19 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-8')
//@[00:0089) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding19|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0089) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0079) |   ├─FunctionArgumentSyntax
//@[46:0079) |   | └─StringSyntax
//@[46:0079) |   |   └─Token(StringComplete) |'Assets/encoding-windows1250.txt'|
//@[79:0080) |   ├─Token(Comma) |,|
//@[81:0088) |   ├─FunctionArgumentSyntax
//@[81:0088) |   | └─StringSyntax
//@[81:0088) |   |   └─Token(StringComplete) |'utf-8'|
//@[88:0089) |   └─Token(RightParen) |)|
//@[89:0091) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding20 = loadTextContent('Assets/encoding-windows1250.txt', 'us-ascii')
//@[00:0092) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding20|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0092) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0079) |   ├─FunctionArgumentSyntax
//@[46:0079) |   | └─StringSyntax
//@[46:0079) |   |   └─Token(StringComplete) |'Assets/encoding-windows1250.txt'|
//@[79:0080) |   ├─Token(Comma) |,|
//@[81:0091) |   ├─FunctionArgumentSyntax
//@[81:0091) |   | └─StringSyntax
//@[81:0091) |   |   └─Token(StringComplete) |'us-ascii'|
//@[91:0092) |   └─Token(RightParen) |)|
//@[92:0094) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding21 = loadTextContent('Assets/encoding-windows1250.txt', 'iso-8859-1')
//@[00:0094) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding21|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0094) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0079) |   ├─FunctionArgumentSyntax
//@[46:0079) |   | └─StringSyntax
//@[46:0079) |   |   └─Token(StringComplete) |'Assets/encoding-windows1250.txt'|
//@[79:0080) |   ├─Token(Comma) |,|
//@[81:0093) |   ├─FunctionArgumentSyntax
//@[81:0093) |   | └─StringSyntax
//@[81:0093) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[93:0094) |   └─Token(RightParen) |)|
//@[94:0096) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding22 = loadTextContent('Assets/encoding-utf8.txt', 'iso-8859-1')
//@[00:0087) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding22|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0087) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0072) |   ├─FunctionArgumentSyntax
//@[46:0072) |   | └─StringSyntax
//@[46:0072) |   |   └─Token(StringComplete) |'Assets/encoding-utf8.txt'|
//@[72:0073) |   ├─Token(Comma) |,|
//@[74:0086) |   ├─FunctionArgumentSyntax
//@[74:0086) |   | └─StringSyntax
//@[74:0086) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[86:0087) |   └─Token(RightParen) |)|
//@[87:0089) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding23 = loadTextContent('Assets/encoding-utf8.txt', 'utf-16')
//@[00:0083) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding23|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0083) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0072) |   ├─FunctionArgumentSyntax
//@[46:0072) |   | └─StringSyntax
//@[46:0072) |   |   └─Token(StringComplete) |'Assets/encoding-utf8.txt'|
//@[72:0073) |   ├─Token(Comma) |,|
//@[74:0082) |   ├─FunctionArgumentSyntax
//@[74:0082) |   | └─StringSyntax
//@[74:0082) |   |   └─Token(StringComplete) |'utf-16'|
//@[82:0083) |   └─Token(RightParen) |)|
//@[83:0085) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding24 = loadTextContent('Assets/encoding-utf8.txt', 'utf-16BE')
//@[00:0085) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding24|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0085) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0072) |   ├─FunctionArgumentSyntax
//@[46:0072) |   | └─StringSyntax
//@[46:0072) |   |   └─Token(StringComplete) |'Assets/encoding-utf8.txt'|
//@[72:0073) |   ├─Token(Comma) |,|
//@[74:0084) |   ├─FunctionArgumentSyntax
//@[74:0084) |   | └─StringSyntax
//@[74:0084) |   |   └─Token(StringComplete) |'utf-16BE'|
//@[84:0085) |   └─Token(RightParen) |)|
//@[85:0087) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding25 = loadTextContent('Assets/encoding-utf8.txt', 'us-ascii')
//@[00:0085) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding25|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0085) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0072) |   ├─FunctionArgumentSyntax
//@[46:0072) |   | └─StringSyntax
//@[46:0072) |   |   └─Token(StringComplete) |'Assets/encoding-utf8.txt'|
//@[72:0073) |   ├─Token(Comma) |,|
//@[74:0084) |   ├─FunctionArgumentSyntax
//@[74:0084) |   | └─StringSyntax
//@[74:0084) |   |   └─Token(StringComplete) |'us-ascii'|
//@[84:0085) |   └─Token(RightParen) |)|
//@[85:0087) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding26 = loadTextContent('Assets/encoding-utf8-bom.txt', 'iso-8859-1')
//@[00:0091) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding26|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0091) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0076) |   ├─FunctionArgumentSyntax
//@[46:0076) |   | └─StringSyntax
//@[46:0076) |   |   └─Token(StringComplete) |'Assets/encoding-utf8-bom.txt'|
//@[76:0077) |   ├─Token(Comma) |,|
//@[78:0090) |   ├─FunctionArgumentSyntax
//@[78:0090) |   | └─StringSyntax
//@[78:0090) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[90:0091) |   └─Token(RightParen) |)|
//@[91:0093) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding27 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-16')
//@[00:0087) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding27|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0087) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0076) |   ├─FunctionArgumentSyntax
//@[46:0076) |   | └─StringSyntax
//@[46:0076) |   |   └─Token(StringComplete) |'Assets/encoding-utf8-bom.txt'|
//@[76:0077) |   ├─Token(Comma) |,|
//@[78:0086) |   ├─FunctionArgumentSyntax
//@[78:0086) |   | └─StringSyntax
//@[78:0086) |   |   └─Token(StringComplete) |'utf-16'|
//@[86:0087) |   └─Token(RightParen) |)|
//@[87:0089) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding28 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-16BE')
//@[00:0089) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding28|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0089) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0076) |   ├─FunctionArgumentSyntax
//@[46:0076) |   | └─StringSyntax
//@[46:0076) |   |   └─Token(StringComplete) |'Assets/encoding-utf8-bom.txt'|
//@[76:0077) |   ├─Token(Comma) |,|
//@[78:0088) |   ├─FunctionArgumentSyntax
//@[78:0088) |   | └─StringSyntax
//@[78:0088) |   |   └─Token(StringComplete) |'utf-16BE'|
//@[88:0089) |   └─Token(RightParen) |)|
//@[89:0091) ├─Token(NewLine) |\r\n|
var textLoadWrongEncoding29 = loadTextContent('Assets/encoding-utf8-bom.txt', 'us-ascii')
//@[00:0089) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |textLoadWrongEncoding29|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0089) | └─FunctionCallSyntax
//@[30:0045) |   ├─IdentifierSyntax
//@[30:0045) |   | └─Token(Identifier) |loadTextContent|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0076) |   ├─FunctionArgumentSyntax
//@[46:0076) |   | └─StringSyntax
//@[46:0076) |   |   └─Token(StringComplete) |'Assets/encoding-utf8-bom.txt'|
//@[76:0077) |   ├─Token(Comma) |,|
//@[78:0088) |   ├─FunctionArgumentSyntax
//@[78:0088) |   | └─StringSyntax
//@[78:0088) |   |   └─Token(StringComplete) |'us-ascii'|
//@[88:0089) |   └─Token(RightParen) |)|
//@[89:0093) ├─Token(NewLine) |\r\n\r\n|

var textOversize = loadTextContent('Assets/oversizeText.txt')
//@[00:0061) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |textOversize|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0061) | └─FunctionCallSyntax
//@[19:0034) |   ├─IdentifierSyntax
//@[19:0034) |   | └─Token(Identifier) |loadTextContent|
//@[34:0035) |   ├─Token(LeftParen) |(|
//@[35:0060) |   ├─FunctionArgumentSyntax
//@[35:0060) |   | └─StringSyntax
//@[35:0060) |   |   └─Token(StringComplete) |'Assets/oversizeText.txt'|
//@[60:0061) |   └─Token(RightParen) |)|
//@[61:0063) ├─Token(NewLine) |\r\n|
var binaryOversize = loadFileAsBase64('Assets/oversizeBinary')
//@[00:0062) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |binaryOversize|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0062) | └─FunctionCallSyntax
//@[21:0037) |   ├─IdentifierSyntax
//@[21:0037) |   | └─Token(Identifier) |loadFileAsBase64|
//@[37:0038) |   ├─Token(LeftParen) |(|
//@[38:0061) |   ├─FunctionArgumentSyntax
//@[38:0061) |   | └─StringSyntax
//@[38:0061) |   |   └─Token(StringComplete) |'Assets/oversizeBinary'|
//@[61:0062) |   └─Token(RightParen) |)|
//@[62:0066) ├─Token(NewLine) |\r\n\r\n|

var binaryAsText = loadTextContent('Assets/binary')
//@[00:0051) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |binaryAsText|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0051) | └─FunctionCallSyntax
//@[19:0034) |   ├─IdentifierSyntax
//@[19:0034) |   | └─Token(Identifier) |loadTextContent|
//@[34:0035) |   ├─Token(LeftParen) |(|
//@[35:0050) |   ├─FunctionArgumentSyntax
//@[35:0050) |   | └─StringSyntax
//@[35:0050) |   |   └─Token(StringComplete) |'Assets/binary'|
//@[50:0051) |   └─Token(RightParen) |)|
//@[51:0055) ├─Token(NewLine) |\r\n\r\n|

var jsonObject1 = loadJsonContent('Assets/jsonInvalid.json.txt')
//@[00:0064) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |jsonObject1|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0064) | └─FunctionCallSyntax
//@[18:0033) |   ├─IdentifierSyntax
//@[18:0033) |   | └─Token(Identifier) |loadJsonContent|
//@[33:0034) |   ├─Token(LeftParen) |(|
//@[34:0063) |   ├─FunctionArgumentSyntax
//@[34:0063) |   | └─StringSyntax
//@[34:0063) |   |   └─Token(StringComplete) |'Assets/jsonInvalid.json.txt'|
//@[63:0064) |   └─Token(RightParen) |)|
//@[64:0066) ├─Token(NewLine) |\r\n|
var jsonObject2 = loadJsonContent('Assets/jsonValid.json.txt', '.')
//@[00:0067) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |jsonObject2|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0067) | └─FunctionCallSyntax
//@[18:0033) |   ├─IdentifierSyntax
//@[18:0033) |   | └─Token(Identifier) |loadJsonContent|
//@[33:0034) |   ├─Token(LeftParen) |(|
//@[34:0061) |   ├─FunctionArgumentSyntax
//@[34:0061) |   | └─StringSyntax
//@[34:0061) |   |   └─Token(StringComplete) |'Assets/jsonValid.json.txt'|
//@[61:0062) |   ├─Token(Comma) |,|
//@[63:0066) |   ├─FunctionArgumentSyntax
//@[63:0066) |   | └─StringSyntax
//@[63:0066) |   |   └─Token(StringComplete) |'.'|
//@[66:0067) |   └─Token(RightParen) |)|
//@[67:0069) ├─Token(NewLine) |\r\n|
var jsonObject3 = loadJsonContent('Assets/jsonValid.json.txt', '$.')
//@[00:0068) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |jsonObject3|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0068) | └─FunctionCallSyntax
//@[18:0033) |   ├─IdentifierSyntax
//@[18:0033) |   | └─Token(Identifier) |loadJsonContent|
//@[33:0034) |   ├─Token(LeftParen) |(|
//@[34:0061) |   ├─FunctionArgumentSyntax
//@[34:0061) |   | └─StringSyntax
//@[34:0061) |   |   └─Token(StringComplete) |'Assets/jsonValid.json.txt'|
//@[61:0062) |   ├─Token(Comma) |,|
//@[63:0067) |   ├─FunctionArgumentSyntax
//@[63:0067) |   | └─StringSyntax
//@[63:0067) |   |   └─Token(StringComplete) |'$.'|
//@[67:0068) |   └─Token(RightParen) |)|
//@[68:0070) ├─Token(NewLine) |\r\n|
var jsonObject4 = loadJsonContent('Assets/jsonValid.json.txt', '.propertyThatDoesNotExist')
//@[00:0091) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |jsonObject4|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0091) | └─FunctionCallSyntax
//@[18:0033) |   ├─IdentifierSyntax
//@[18:0033) |   | └─Token(Identifier) |loadJsonContent|
//@[33:0034) |   ├─Token(LeftParen) |(|
//@[34:0061) |   ├─FunctionArgumentSyntax
//@[34:0061) |   | └─StringSyntax
//@[34:0061) |   |   └─Token(StringComplete) |'Assets/jsonValid.json.txt'|
//@[61:0062) |   ├─Token(Comma) |,|
//@[63:0090) |   ├─FunctionArgumentSyntax
//@[63:0090) |   | └─StringSyntax
//@[63:0090) |   |   └─Token(StringComplete) |'.propertyThatDoesNotExist'|
//@[90:0091) |   └─Token(RightParen) |)|
//@[91:0093) ├─Token(NewLine) |\r\n|
var jsonObject5 = loadJsonContent('Assets/fileNotExists')
//@[00:0057) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |jsonObject5|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0057) | └─FunctionCallSyntax
//@[18:0033) |   ├─IdentifierSyntax
//@[18:0033) |   | └─Token(Identifier) |loadJsonContent|
//@[33:0034) |   ├─Token(LeftParen) |(|
//@[34:0056) |   ├─FunctionArgumentSyntax
//@[34:0056) |   | └─StringSyntax
//@[34:0056) |   |   └─Token(StringComplete) |'Assets/fileNotExists'|
//@[56:0057) |   └─Token(RightParen) |)|
//@[57:0059) ├─Token(NewLine) |\r\n|

//@[00:0000) └─Token(EndOfFile) ||
