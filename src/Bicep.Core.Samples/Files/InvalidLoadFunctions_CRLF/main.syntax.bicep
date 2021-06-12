var textLoadDirectory = loadTextContent('Assets/path/to/nothing')
//@[0:65) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |textLoadDirectory|
//@[22:23)  Assignment |=|
//@[24:65)  FunctionCallSyntax
//@[24:39)   IdentifierSyntax
//@[24:39)    Identifier |loadTextContent|
//@[39:40)   LeftParen |(|
//@[40:64)   FunctionArgumentSyntax
//@[40:64)    StringSyntax
//@[40:64)     StringComplete |'Assets/path/to/nothing'|
//@[64:65)   RightParen |)|
//@[65:67) NewLine |\r\n|
var binaryLoadDirectory = loadFileAsBase64('Assets/path/to/nothing')
//@[0:68) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |binaryLoadDirectory|
//@[24:25)  Assignment |=|
//@[26:68)  FunctionCallSyntax
//@[26:42)   IdentifierSyntax
//@[26:42)    Identifier |loadFileAsBase64|
//@[42:43)   LeftParen |(|
//@[43:67)   FunctionArgumentSyntax
//@[43:67)    StringSyntax
//@[43:67)     StringComplete |'Assets/path/to/nothing'|
//@[67:68)   RightParen |)|
//@[68:72) NewLine |\r\n\r\n|

var textLoadFileMissing = loadTextContent('Assets/nothing.file')
//@[0:64) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |textLoadFileMissing|
//@[24:25)  Assignment |=|
//@[26:64)  FunctionCallSyntax
//@[26:41)   IdentifierSyntax
//@[26:41)    Identifier |loadTextContent|
//@[41:42)   LeftParen |(|
//@[42:63)   FunctionArgumentSyntax
//@[42:63)    StringSyntax
//@[42:63)     StringComplete |'Assets/nothing.file'|
//@[63:64)   RightParen |)|
//@[64:66) NewLine |\r\n|
var binaryLoadFileMissing = loadFileAsBase64('Assets/nothing.file')
//@[0:67) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:25)  IdentifierSyntax
//@[4:25)   Identifier |binaryLoadFileMissing|
//@[26:27)  Assignment |=|
//@[28:67)  FunctionCallSyntax
//@[28:44)   IdentifierSyntax
//@[28:44)    Identifier |loadFileAsBase64|
//@[44:45)   LeftParen |(|
//@[45:66)   FunctionArgumentSyntax
//@[45:66)    StringSyntax
//@[45:66)     StringComplete |'Assets/nothing.file'|
//@[66:67)   RightParen |)|
//@[67:71) NewLine |\r\n\r\n|

var textLoadFilePathEmpty = loadTextContent('')
//@[0:47) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:25)  IdentifierSyntax
//@[4:25)   Identifier |textLoadFilePathEmpty|
//@[26:27)  Assignment |=|
//@[28:47)  FunctionCallSyntax
//@[28:43)   IdentifierSyntax
//@[28:43)    Identifier |loadTextContent|
//@[43:44)   LeftParen |(|
//@[44:46)   FunctionArgumentSyntax
//@[44:46)    StringSyntax
//@[44:46)     StringComplete |''|
//@[46:47)   RightParen |)|
//@[47:49) NewLine |\r\n|
var binaryLoadFilePathEmpty = loadFileAsBase64('')
//@[0:50) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |binaryLoadFilePathEmpty|
//@[28:29)  Assignment |=|
//@[30:50)  FunctionCallSyntax
//@[30:46)   IdentifierSyntax
//@[30:46)    Identifier |loadFileAsBase64|
//@[46:47)   LeftParen |(|
//@[47:49)   FunctionArgumentSyntax
//@[47:49)    StringSyntax
//@[47:49)     StringComplete |''|
//@[49:50)   RightParen |)|
//@[50:54) NewLine |\r\n\r\n|

var textLoadInvalidCharactersPath1 = loadTextContent('Assets\\TextFile.txt')
//@[0:76) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:34)  IdentifierSyntax
//@[4:34)   Identifier |textLoadInvalidCharactersPath1|
//@[35:36)  Assignment |=|
//@[37:76)  FunctionCallSyntax
//@[37:52)   IdentifierSyntax
//@[37:52)    Identifier |loadTextContent|
//@[52:53)   LeftParen |(|
//@[53:75)   FunctionArgumentSyntax
//@[53:75)    StringSyntax
//@[53:75)     StringComplete |'Assets\\TextFile.txt'|
//@[75:76)   RightParen |)|
//@[76:78) NewLine |\r\n|
var binaryLoadInvalidCharactersPath1 = loadFileAsBase64('Assets\\binary')
//@[0:73) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:36)  IdentifierSyntax
//@[4:36)   Identifier |binaryLoadInvalidCharactersPath1|
//@[37:38)  Assignment |=|
//@[39:73)  FunctionCallSyntax
//@[39:55)   IdentifierSyntax
//@[39:55)    Identifier |loadFileAsBase64|
//@[55:56)   LeftParen |(|
//@[56:72)   FunctionArgumentSyntax
//@[56:72)    StringSyntax
//@[56:72)     StringComplete |'Assets\\binary'|
//@[72:73)   RightParen |)|
//@[73:77) NewLine |\r\n\r\n|

var textLoadInvalidCharactersPath2 = loadTextContent('/Assets/TextFile.txt')
//@[0:76) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:34)  IdentifierSyntax
//@[4:34)   Identifier |textLoadInvalidCharactersPath2|
//@[35:36)  Assignment |=|
//@[37:76)  FunctionCallSyntax
//@[37:52)   IdentifierSyntax
//@[37:52)    Identifier |loadTextContent|
//@[52:53)   LeftParen |(|
//@[53:75)   FunctionArgumentSyntax
//@[53:75)    StringSyntax
//@[53:75)     StringComplete |'/Assets/TextFile.txt'|
//@[75:76)   RightParen |)|
//@[76:78) NewLine |\r\n|
var binaryLoadInvalidCharactersPath2 = loadFileAsBase64('/Assets/binary')
//@[0:73) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:36)  IdentifierSyntax
//@[4:36)   Identifier |binaryLoadInvalidCharactersPath2|
//@[37:38)  Assignment |=|
//@[39:73)  FunctionCallSyntax
//@[39:55)   IdentifierSyntax
//@[39:55)    Identifier |loadFileAsBase64|
//@[55:56)   LeftParen |(|
//@[56:72)   FunctionArgumentSyntax
//@[56:72)    StringSyntax
//@[56:72)     StringComplete |'/Assets/binary'|
//@[72:73)   RightParen |)|
//@[73:77) NewLine |\r\n\r\n|

var textLoadInvalidCharactersPath3 = loadTextContent('file://Assets/TextFile.txt')
//@[0:82) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:34)  IdentifierSyntax
//@[4:34)   Identifier |textLoadInvalidCharactersPath3|
//@[35:36)  Assignment |=|
//@[37:82)  FunctionCallSyntax
//@[37:52)   IdentifierSyntax
//@[37:52)    Identifier |loadTextContent|
//@[52:53)   LeftParen |(|
//@[53:81)   FunctionArgumentSyntax
//@[53:81)    StringSyntax
//@[53:81)     StringComplete |'file://Assets/TextFile.txt'|
//@[81:82)   RightParen |)|
//@[82:84) NewLine |\r\n|
var binaryLoadInvalidCharactersPath3 = loadFileAsBase64('file://Assets/binary')
//@[0:79) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:36)  IdentifierSyntax
//@[4:36)   Identifier |binaryLoadInvalidCharactersPath3|
//@[37:38)  Assignment |=|
//@[39:79)  FunctionCallSyntax
//@[39:55)   IdentifierSyntax
//@[39:55)    Identifier |loadFileAsBase64|
//@[55:56)   LeftParen |(|
//@[56:78)   FunctionArgumentSyntax
//@[56:78)    StringSyntax
//@[56:78)     StringComplete |'file://Assets/binary'|
//@[78:79)   RightParen |)|
//@[79:85) NewLine |\r\n\r\n\r\n|


var textLoadUnsupportedEncoding = loadTextContent('Assets/TextFile.txt', 'windows-1250')
//@[0:88) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:31)  IdentifierSyntax
//@[4:31)   Identifier |textLoadUnsupportedEncoding|
//@[32:33)  Assignment |=|
//@[34:88)  FunctionCallSyntax
//@[34:49)   IdentifierSyntax
//@[34:49)    Identifier |loadTextContent|
//@[49:50)   LeftParen |(|
//@[50:72)   FunctionArgumentSyntax
//@[50:71)    StringSyntax
//@[50:71)     StringComplete |'Assets/TextFile.txt'|
//@[71:72)    Comma |,|
//@[73:87)   FunctionArgumentSyntax
//@[73:87)    StringSyntax
//@[73:87)     StringComplete |'windows-1250'|
//@[87:88)   RightParen |)|
//@[88:92) NewLine |\r\n\r\n|

var textLoadWrongEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'us-ascii')
//@[0:84) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding01|
//@[28:29)  Assignment |=|
//@[30:84)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:72)   FunctionArgumentSyntax
//@[46:71)    StringSyntax
//@[46:71)     StringComplete |'Assets/encoding-iso.txt'|
//@[71:72)    Comma |,|
//@[73:83)   FunctionArgumentSyntax
//@[73:83)    StringSyntax
//@[73:83)     StringComplete |'us-ascii'|
//@[83:84)   RightParen |)|
//@[84:86) NewLine |\r\n|
var textLoadWrongEncoding02 = loadTextContent('Assets/encoding-iso.txt', 'utf-8')
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding02|
//@[28:29)  Assignment |=|
//@[30:81)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:72)   FunctionArgumentSyntax
//@[46:71)    StringSyntax
//@[46:71)     StringComplete |'Assets/encoding-iso.txt'|
//@[71:72)    Comma |,|
//@[73:80)   FunctionArgumentSyntax
//@[73:80)    StringSyntax
//@[73:80)     StringComplete |'utf-8'|
//@[80:81)   RightParen |)|
//@[81:83) NewLine |\r\n|
var textLoadWrongEncoding03 = loadTextContent('Assets/encoding-iso.txt', 'utf-16BE')
//@[0:84) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding03|
//@[28:29)  Assignment |=|
//@[30:84)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:72)   FunctionArgumentSyntax
//@[46:71)    StringSyntax
//@[46:71)     StringComplete |'Assets/encoding-iso.txt'|
//@[71:72)    Comma |,|
//@[73:83)   FunctionArgumentSyntax
//@[73:83)    StringSyntax
//@[73:83)     StringComplete |'utf-16BE'|
//@[83:84)   RightParen |)|
//@[84:86) NewLine |\r\n|
var textLoadWrongEncoding04 = loadTextContent('Assets/encoding-iso.txt', 'utf-16')
//@[0:82) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding04|
//@[28:29)  Assignment |=|
//@[30:82)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:72)   FunctionArgumentSyntax
//@[46:71)    StringSyntax
//@[46:71)     StringComplete |'Assets/encoding-iso.txt'|
//@[71:72)    Comma |,|
//@[73:81)   FunctionArgumentSyntax
//@[73:81)    StringSyntax
//@[73:81)     StringComplete |'utf-16'|
//@[81:82)   RightParen |)|
//@[82:84) NewLine |\r\n|
var textLoadWrongEncoding05 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[0:88) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding05|
//@[28:29)  Assignment |=|
//@[30:88)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:74)   FunctionArgumentSyntax
//@[46:73)    StringSyntax
//@[46:73)     StringComplete |'Assets/encoding-ascii.txt'|
//@[73:74)    Comma |,|
//@[75:87)   FunctionArgumentSyntax
//@[75:87)    StringSyntax
//@[75:87)     StringComplete |'iso-8859-1'|
//@[87:88)   RightParen |)|
//@[88:90) NewLine |\r\n|
var textLoadWrongEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[0:83) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding06|
//@[28:29)  Assignment |=|
//@[30:83)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:74)   FunctionArgumentSyntax
//@[46:73)    StringSyntax
//@[46:73)     StringComplete |'Assets/encoding-ascii.txt'|
//@[73:74)    Comma |,|
//@[75:82)   FunctionArgumentSyntax
//@[75:82)    StringSyntax
//@[75:82)     StringComplete |'utf-8'|
//@[82:83)   RightParen |)|
//@[83:85) NewLine |\r\n|
var textLoadWrongEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'utf-16BE')
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding07|
//@[28:29)  Assignment |=|
//@[30:86)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:74)   FunctionArgumentSyntax
//@[46:73)    StringSyntax
//@[46:73)     StringComplete |'Assets/encoding-ascii.txt'|
//@[73:74)    Comma |,|
//@[75:85)   FunctionArgumentSyntax
//@[75:85)    StringSyntax
//@[75:85)     StringComplete |'utf-16BE'|
//@[85:86)   RightParen |)|
//@[86:88) NewLine |\r\n|
var textLoadWrongEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-16')
//@[0:84) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding08|
//@[28:29)  Assignment |=|
//@[30:84)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:74)   FunctionArgumentSyntax
//@[46:73)    StringSyntax
//@[46:73)     StringComplete |'Assets/encoding-ascii.txt'|
//@[73:74)    Comma |,|
//@[75:83)   FunctionArgumentSyntax
//@[75:83)    StringSyntax
//@[75:83)     StringComplete |'utf-16'|
//@[83:84)   RightParen |)|
//@[84:86) NewLine |\r\n|
var textLoadWrongEncoding09 = loadTextContent('Assets/encoding-utf16.txt', 'iso-8859-1')
//@[0:88) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding09|
//@[28:29)  Assignment |=|
//@[30:88)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:74)   FunctionArgumentSyntax
//@[46:73)    StringSyntax
//@[46:73)     StringComplete |'Assets/encoding-utf16.txt'|
//@[73:74)    Comma |,|
//@[75:87)   FunctionArgumentSyntax
//@[75:87)    StringSyntax
//@[75:87)     StringComplete |'iso-8859-1'|
//@[87:88)   RightParen |)|
//@[88:90) NewLine |\r\n|
var textLoadWrongEncoding10 = loadTextContent('Assets/encoding-utf16.txt', 'utf-8')
//@[0:83) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding10|
//@[28:29)  Assignment |=|
//@[30:83)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:74)   FunctionArgumentSyntax
//@[46:73)    StringSyntax
//@[46:73)     StringComplete |'Assets/encoding-utf16.txt'|
//@[73:74)    Comma |,|
//@[75:82)   FunctionArgumentSyntax
//@[75:82)    StringSyntax
//@[75:82)     StringComplete |'utf-8'|
//@[82:83)   RightParen |)|
//@[83:85) NewLine |\r\n|
var textLoadWrongEncoding11 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16BE')
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding11|
//@[28:29)  Assignment |=|
//@[30:86)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:74)   FunctionArgumentSyntax
//@[46:73)    StringSyntax
//@[46:73)     StringComplete |'Assets/encoding-utf16.txt'|
//@[73:74)    Comma |,|
//@[75:85)   FunctionArgumentSyntax
//@[75:85)    StringSyntax
//@[75:85)     StringComplete |'utf-16BE'|
//@[85:86)   RightParen |)|
//@[86:88) NewLine |\r\n|
var textLoadWrongEncoding12 = loadTextContent('Assets/encoding-utf16.txt', 'us-ascii')
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding12|
//@[28:29)  Assignment |=|
//@[30:86)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:74)   FunctionArgumentSyntax
//@[46:73)    StringSyntax
//@[46:73)     StringComplete |'Assets/encoding-utf16.txt'|
//@[73:74)    Comma |,|
//@[75:85)   FunctionArgumentSyntax
//@[75:85)    StringSyntax
//@[75:85)     StringComplete |'us-ascii'|
//@[85:86)   RightParen |)|
//@[86:88) NewLine |\r\n|
var textLoadWrongEncoding13 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16')
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding13|
//@[28:29)  Assignment |=|
//@[30:86)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:76)   FunctionArgumentSyntax
//@[46:75)    StringSyntax
//@[46:75)     StringComplete |'Assets/encoding-utf16be.txt'|
//@[75:76)    Comma |,|
//@[77:85)   FunctionArgumentSyntax
//@[77:85)    StringSyntax
//@[77:85)     StringComplete |'utf-16'|
//@[85:86)   RightParen |)|
//@[86:88) NewLine |\r\n|
var textLoadWrongEncoding14 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-8')
//@[0:85) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding14|
//@[28:29)  Assignment |=|
//@[30:85)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:76)   FunctionArgumentSyntax
//@[46:75)    StringSyntax
//@[46:75)     StringComplete |'Assets/encoding-utf16be.txt'|
//@[75:76)    Comma |,|
//@[77:84)   FunctionArgumentSyntax
//@[77:84)    StringSyntax
//@[77:84)     StringComplete |'utf-8'|
//@[84:85)   RightParen |)|
//@[85:87) NewLine |\r\n|
var textLoadWrongEncoding15 = loadTextContent('Assets/encoding-utf16be.txt', 'us-ascii')
//@[0:88) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding15|
//@[28:29)  Assignment |=|
//@[30:88)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:76)   FunctionArgumentSyntax
//@[46:75)    StringSyntax
//@[46:75)     StringComplete |'Assets/encoding-utf16be.txt'|
//@[75:76)    Comma |,|
//@[77:87)   FunctionArgumentSyntax
//@[77:87)    StringSyntax
//@[77:87)     StringComplete |'us-ascii'|
//@[87:88)   RightParen |)|
//@[88:90) NewLine |\r\n|
var textLoadWrongEncoding16 = loadTextContent('Assets/encoding-utf16be.txt', 'iso-8859-1')
//@[0:90) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding16|
//@[28:29)  Assignment |=|
//@[30:90)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:76)   FunctionArgumentSyntax
//@[46:75)    StringSyntax
//@[46:75)     StringComplete |'Assets/encoding-utf16be.txt'|
//@[75:76)    Comma |,|
//@[77:89)   FunctionArgumentSyntax
//@[77:89)    StringSyntax
//@[77:89)     StringComplete |'iso-8859-1'|
//@[89:90)   RightParen |)|
//@[90:92) NewLine |\r\n|
var textLoadWrongEncoding17 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-16BE')
//@[0:92) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding17|
//@[28:29)  Assignment |=|
//@[30:92)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:80)   FunctionArgumentSyntax
//@[46:79)    StringSyntax
//@[46:79)     StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80)    Comma |,|
//@[81:91)   FunctionArgumentSyntax
//@[81:91)    StringSyntax
//@[81:91)     StringComplete |'utf-16BE'|
//@[91:92)   RightParen |)|
//@[92:94) NewLine |\r\n|
var textLoadWrongEncoding18 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-16')
//@[0:90) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding18|
//@[28:29)  Assignment |=|
//@[30:90)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:80)   FunctionArgumentSyntax
//@[46:79)    StringSyntax
//@[46:79)     StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80)    Comma |,|
//@[81:89)   FunctionArgumentSyntax
//@[81:89)    StringSyntax
//@[81:89)     StringComplete |'utf-16'|
//@[89:90)   RightParen |)|
//@[90:92) NewLine |\r\n|
var textLoadWrongEncoding19 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-8')
//@[0:89) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding19|
//@[28:29)  Assignment |=|
//@[30:89)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:80)   FunctionArgumentSyntax
//@[46:79)    StringSyntax
//@[46:79)     StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80)    Comma |,|
//@[81:88)   FunctionArgumentSyntax
//@[81:88)    StringSyntax
//@[81:88)     StringComplete |'utf-8'|
//@[88:89)   RightParen |)|
//@[89:91) NewLine |\r\n|
var textLoadWrongEncoding20 = loadTextContent('Assets/encoding-windows1250.txt', 'us-ascii')
//@[0:92) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding20|
//@[28:29)  Assignment |=|
//@[30:92)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:80)   FunctionArgumentSyntax
//@[46:79)    StringSyntax
//@[46:79)     StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80)    Comma |,|
//@[81:91)   FunctionArgumentSyntax
//@[81:91)    StringSyntax
//@[81:91)     StringComplete |'us-ascii'|
//@[91:92)   RightParen |)|
//@[92:94) NewLine |\r\n|
var textLoadWrongEncoding21 = loadTextContent('Assets/encoding-windows1250.txt', 'iso-8859-1')
//@[0:94) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding21|
//@[28:29)  Assignment |=|
//@[30:94)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:80)   FunctionArgumentSyntax
//@[46:79)    StringSyntax
//@[46:79)     StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80)    Comma |,|
//@[81:93)   FunctionArgumentSyntax
//@[81:93)    StringSyntax
//@[81:93)     StringComplete |'iso-8859-1'|
//@[93:94)   RightParen |)|
//@[94:96) NewLine |\r\n|
var textLoadWrongEncoding22 = loadTextContent('Assets/encoding-utf8.txt', 'iso-8859-1')
//@[0:87) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding22|
//@[28:29)  Assignment |=|
//@[30:87)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:73)   FunctionArgumentSyntax
//@[46:72)    StringSyntax
//@[46:72)     StringComplete |'Assets/encoding-utf8.txt'|
//@[72:73)    Comma |,|
//@[74:86)   FunctionArgumentSyntax
//@[74:86)    StringSyntax
//@[74:86)     StringComplete |'iso-8859-1'|
//@[86:87)   RightParen |)|
//@[87:89) NewLine |\r\n|
var textLoadWrongEncoding23 = loadTextContent('Assets/encoding-utf8.txt', 'utf-16')
//@[0:83) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding23|
//@[28:29)  Assignment |=|
//@[30:83)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:73)   FunctionArgumentSyntax
//@[46:72)    StringSyntax
//@[46:72)     StringComplete |'Assets/encoding-utf8.txt'|
//@[72:73)    Comma |,|
//@[74:82)   FunctionArgumentSyntax
//@[74:82)    StringSyntax
//@[74:82)     StringComplete |'utf-16'|
//@[82:83)   RightParen |)|
//@[83:85) NewLine |\r\n|
var textLoadWrongEncoding24 = loadTextContent('Assets/encoding-utf8.txt', 'utf-16BE')
//@[0:85) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding24|
//@[28:29)  Assignment |=|
//@[30:85)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:73)   FunctionArgumentSyntax
//@[46:72)    StringSyntax
//@[46:72)     StringComplete |'Assets/encoding-utf8.txt'|
//@[72:73)    Comma |,|
//@[74:84)   FunctionArgumentSyntax
//@[74:84)    StringSyntax
//@[74:84)     StringComplete |'utf-16BE'|
//@[84:85)   RightParen |)|
//@[85:87) NewLine |\r\n|
var textLoadWrongEncoding25 = loadTextContent('Assets/encoding-utf8.txt', 'us-ascii')
//@[0:85) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding25|
//@[28:29)  Assignment |=|
//@[30:85)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:73)   FunctionArgumentSyntax
//@[46:72)    StringSyntax
//@[46:72)     StringComplete |'Assets/encoding-utf8.txt'|
//@[72:73)    Comma |,|
//@[74:84)   FunctionArgumentSyntax
//@[74:84)    StringSyntax
//@[74:84)     StringComplete |'us-ascii'|
//@[84:85)   RightParen |)|
//@[85:87) NewLine |\r\n|
var textLoadWrongEncoding26 = loadTextContent('Assets/encoding-utf8-bom.txt', 'iso-8859-1')
//@[0:91) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding26|
//@[28:29)  Assignment |=|
//@[30:91)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:77)   FunctionArgumentSyntax
//@[46:76)    StringSyntax
//@[46:76)     StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[76:77)    Comma |,|
//@[78:90)   FunctionArgumentSyntax
//@[78:90)    StringSyntax
//@[78:90)     StringComplete |'iso-8859-1'|
//@[90:91)   RightParen |)|
//@[91:93) NewLine |\r\n|
var textLoadWrongEncoding27 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-16')
//@[0:87) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding27|
//@[28:29)  Assignment |=|
//@[30:87)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:77)   FunctionArgumentSyntax
//@[46:76)    StringSyntax
//@[46:76)     StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[76:77)    Comma |,|
//@[78:86)   FunctionArgumentSyntax
//@[78:86)    StringSyntax
//@[78:86)     StringComplete |'utf-16'|
//@[86:87)   RightParen |)|
//@[87:89) NewLine |\r\n|
var textLoadWrongEncoding28 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-16BE')
//@[0:89) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding28|
//@[28:29)  Assignment |=|
//@[30:89)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:77)   FunctionArgumentSyntax
//@[46:76)    StringSyntax
//@[46:76)     StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[76:77)    Comma |,|
//@[78:88)   FunctionArgumentSyntax
//@[78:88)    StringSyntax
//@[78:88)     StringComplete |'utf-16BE'|
//@[88:89)   RightParen |)|
//@[89:91) NewLine |\r\n|
var textLoadWrongEncoding29 = loadTextContent('Assets/encoding-utf8-bom.txt', 'us-ascii')
//@[0:89) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |textLoadWrongEncoding29|
//@[28:29)  Assignment |=|
//@[30:89)  FunctionCallSyntax
//@[30:45)   IdentifierSyntax
//@[30:45)    Identifier |loadTextContent|
//@[45:46)   LeftParen |(|
//@[46:77)   FunctionArgumentSyntax
//@[46:76)    StringSyntax
//@[46:76)     StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[76:77)    Comma |,|
//@[78:88)   FunctionArgumentSyntax
//@[78:88)    StringSyntax
//@[78:88)     StringComplete |'us-ascii'|
//@[88:89)   RightParen |)|
//@[89:93) NewLine |\r\n\r\n|

var textOversize = loadTextContent('Assets/oversizeText.txt')
//@[0:61) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |textOversize|
//@[17:18)  Assignment |=|
//@[19:61)  FunctionCallSyntax
//@[19:34)   IdentifierSyntax
//@[19:34)    Identifier |loadTextContent|
//@[34:35)   LeftParen |(|
//@[35:60)   FunctionArgumentSyntax
//@[35:60)    StringSyntax
//@[35:60)     StringComplete |'Assets/oversizeText.txt'|
//@[60:61)   RightParen |)|
//@[61:63) NewLine |\r\n|
var binaryOversize = loadFileAsBase64('Assets/oversizeBinary')
//@[0:62) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |binaryOversize|
//@[19:20)  Assignment |=|
//@[21:62)  FunctionCallSyntax
//@[21:37)   IdentifierSyntax
//@[21:37)    Identifier |loadFileAsBase64|
//@[37:38)   LeftParen |(|
//@[38:61)   FunctionArgumentSyntax
//@[38:61)    StringSyntax
//@[38:61)     StringComplete |'Assets/oversizeBinary'|
//@[61:62)   RightParen |)|
//@[62:66) NewLine |\r\n\r\n|

var binaryAsText = loadTextContent('Assets/binary')
//@[0:51) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |binaryAsText|
//@[17:18)  Assignment |=|
//@[19:51)  FunctionCallSyntax
//@[19:34)   IdentifierSyntax
//@[19:34)    Identifier |loadTextContent|
//@[34:35)   LeftParen |(|
//@[35:50)   FunctionArgumentSyntax
//@[35:50)    StringSyntax
//@[35:50)     StringComplete |'Assets/binary'|
//@[50:51)   RightParen |)|
//@[51:51) EndOfFile ||
