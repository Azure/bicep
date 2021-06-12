var textLoadDirectory = loadTextContent('Assets/path/to/nothing')
//@[0:3) Identifier |var|
//@[4:21) Identifier |textLoadDirectory|
//@[22:23) Assignment |=|
//@[24:39) Identifier |loadTextContent|
//@[39:40) LeftParen |(|
//@[40:64) StringComplete |'Assets/path/to/nothing'|
//@[64:65) RightParen |)|
//@[65:67) NewLine |\r\n|
var binaryLoadDirectory = loadFileAsBase64('Assets/path/to/nothing')
//@[0:3) Identifier |var|
//@[4:23) Identifier |binaryLoadDirectory|
//@[24:25) Assignment |=|
//@[26:42) Identifier |loadFileAsBase64|
//@[42:43) LeftParen |(|
//@[43:67) StringComplete |'Assets/path/to/nothing'|
//@[67:68) RightParen |)|
//@[68:72) NewLine |\r\n\r\n|

var textLoadFileMissing = loadTextContent('Assets/nothing.file')
//@[0:3) Identifier |var|
//@[4:23) Identifier |textLoadFileMissing|
//@[24:25) Assignment |=|
//@[26:41) Identifier |loadTextContent|
//@[41:42) LeftParen |(|
//@[42:63) StringComplete |'Assets/nothing.file'|
//@[63:64) RightParen |)|
//@[64:66) NewLine |\r\n|
var binaryLoadFileMissing = loadFileAsBase64('Assets/nothing.file')
//@[0:3) Identifier |var|
//@[4:25) Identifier |binaryLoadFileMissing|
//@[26:27) Assignment |=|
//@[28:44) Identifier |loadFileAsBase64|
//@[44:45) LeftParen |(|
//@[45:66) StringComplete |'Assets/nothing.file'|
//@[66:67) RightParen |)|
//@[67:71) NewLine |\r\n\r\n|

var textLoadFilePathEmpty = loadTextContent('')
//@[0:3) Identifier |var|
//@[4:25) Identifier |textLoadFilePathEmpty|
//@[26:27) Assignment |=|
//@[28:43) Identifier |loadTextContent|
//@[43:44) LeftParen |(|
//@[44:46) StringComplete |''|
//@[46:47) RightParen |)|
//@[47:49) NewLine |\r\n|
var binaryLoadFilePathEmpty = loadFileAsBase64('')
//@[0:3) Identifier |var|
//@[4:27) Identifier |binaryLoadFilePathEmpty|
//@[28:29) Assignment |=|
//@[30:46) Identifier |loadFileAsBase64|
//@[46:47) LeftParen |(|
//@[47:49) StringComplete |''|
//@[49:50) RightParen |)|
//@[50:54) NewLine |\r\n\r\n|

var textLoadInvalidCharactersPath1 = loadTextContent('Assets\\TextFile.txt')
//@[0:3) Identifier |var|
//@[4:34) Identifier |textLoadInvalidCharactersPath1|
//@[35:36) Assignment |=|
//@[37:52) Identifier |loadTextContent|
//@[52:53) LeftParen |(|
//@[53:75) StringComplete |'Assets\\TextFile.txt'|
//@[75:76) RightParen |)|
//@[76:78) NewLine |\r\n|
var binaryLoadInvalidCharactersPath1 = loadFileAsBase64('Assets\\binary')
//@[0:3) Identifier |var|
//@[4:36) Identifier |binaryLoadInvalidCharactersPath1|
//@[37:38) Assignment |=|
//@[39:55) Identifier |loadFileAsBase64|
//@[55:56) LeftParen |(|
//@[56:72) StringComplete |'Assets\\binary'|
//@[72:73) RightParen |)|
//@[73:77) NewLine |\r\n\r\n|

var textLoadInvalidCharactersPath2 = loadTextContent('/Assets/TextFile.txt')
//@[0:3) Identifier |var|
//@[4:34) Identifier |textLoadInvalidCharactersPath2|
//@[35:36) Assignment |=|
//@[37:52) Identifier |loadTextContent|
//@[52:53) LeftParen |(|
//@[53:75) StringComplete |'/Assets/TextFile.txt'|
//@[75:76) RightParen |)|
//@[76:78) NewLine |\r\n|
var binaryLoadInvalidCharactersPath2 = loadFileAsBase64('/Assets/binary')
//@[0:3) Identifier |var|
//@[4:36) Identifier |binaryLoadInvalidCharactersPath2|
//@[37:38) Assignment |=|
//@[39:55) Identifier |loadFileAsBase64|
//@[55:56) LeftParen |(|
//@[56:72) StringComplete |'/Assets/binary'|
//@[72:73) RightParen |)|
//@[73:77) NewLine |\r\n\r\n|

var textLoadInvalidCharactersPath3 = loadTextContent('file://Assets/TextFile.txt')
//@[0:3) Identifier |var|
//@[4:34) Identifier |textLoadInvalidCharactersPath3|
//@[35:36) Assignment |=|
//@[37:52) Identifier |loadTextContent|
//@[52:53) LeftParen |(|
//@[53:81) StringComplete |'file://Assets/TextFile.txt'|
//@[81:82) RightParen |)|
//@[82:84) NewLine |\r\n|
var binaryLoadInvalidCharactersPath3 = loadFileAsBase64('file://Assets/binary')
//@[0:3) Identifier |var|
//@[4:36) Identifier |binaryLoadInvalidCharactersPath3|
//@[37:38) Assignment |=|
//@[39:55) Identifier |loadFileAsBase64|
//@[55:56) LeftParen |(|
//@[56:78) StringComplete |'file://Assets/binary'|
//@[78:79) RightParen |)|
//@[79:85) NewLine |\r\n\r\n\r\n|


var textLoadUnsupportedEncoding = loadTextContent('Assets/TextFile.txt', 'windows-1250')
//@[0:3) Identifier |var|
//@[4:31) Identifier |textLoadUnsupportedEncoding|
//@[32:33) Assignment |=|
//@[34:49) Identifier |loadTextContent|
//@[49:50) LeftParen |(|
//@[50:71) StringComplete |'Assets/TextFile.txt'|
//@[71:72) Comma |,|
//@[73:87) StringComplete |'windows-1250'|
//@[87:88) RightParen |)|
//@[88:92) NewLine |\r\n\r\n|

var textLoadWrongEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'us-ascii')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding01|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:71) StringComplete |'Assets/encoding-iso.txt'|
//@[71:72) Comma |,|
//@[73:83) StringComplete |'us-ascii'|
//@[83:84) RightParen |)|
//@[84:86) NewLine |\r\n|
var textLoadWrongEncoding02 = loadTextContent('Assets/encoding-iso.txt', 'utf-8')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding02|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:71) StringComplete |'Assets/encoding-iso.txt'|
//@[71:72) Comma |,|
//@[73:80) StringComplete |'utf-8'|
//@[80:81) RightParen |)|
//@[81:83) NewLine |\r\n|
var textLoadWrongEncoding03 = loadTextContent('Assets/encoding-iso.txt', 'utf-16BE')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding03|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:71) StringComplete |'Assets/encoding-iso.txt'|
//@[71:72) Comma |,|
//@[73:83) StringComplete |'utf-16BE'|
//@[83:84) RightParen |)|
//@[84:86) NewLine |\r\n|
var textLoadWrongEncoding04 = loadTextContent('Assets/encoding-iso.txt', 'utf-16')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding04|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:71) StringComplete |'Assets/encoding-iso.txt'|
//@[71:72) Comma |,|
//@[73:81) StringComplete |'utf-16'|
//@[81:82) RightParen |)|
//@[82:84) NewLine |\r\n|
var textLoadWrongEncoding05 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding05|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:73) StringComplete |'Assets/encoding-ascii.txt'|
//@[73:74) Comma |,|
//@[75:87) StringComplete |'iso-8859-1'|
//@[87:88) RightParen |)|
//@[88:90) NewLine |\r\n|
var textLoadWrongEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding06|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:73) StringComplete |'Assets/encoding-ascii.txt'|
//@[73:74) Comma |,|
//@[75:82) StringComplete |'utf-8'|
//@[82:83) RightParen |)|
//@[83:85) NewLine |\r\n|
var textLoadWrongEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'utf-16BE')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding07|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:73) StringComplete |'Assets/encoding-ascii.txt'|
//@[73:74) Comma |,|
//@[75:85) StringComplete |'utf-16BE'|
//@[85:86) RightParen |)|
//@[86:88) NewLine |\r\n|
var textLoadWrongEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-16')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding08|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:73) StringComplete |'Assets/encoding-ascii.txt'|
//@[73:74) Comma |,|
//@[75:83) StringComplete |'utf-16'|
//@[83:84) RightParen |)|
//@[84:86) NewLine |\r\n|
var textLoadWrongEncoding09 = loadTextContent('Assets/encoding-utf16.txt', 'iso-8859-1')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding09|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:73) StringComplete |'Assets/encoding-utf16.txt'|
//@[73:74) Comma |,|
//@[75:87) StringComplete |'iso-8859-1'|
//@[87:88) RightParen |)|
//@[88:90) NewLine |\r\n|
var textLoadWrongEncoding10 = loadTextContent('Assets/encoding-utf16.txt', 'utf-8')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding10|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:73) StringComplete |'Assets/encoding-utf16.txt'|
//@[73:74) Comma |,|
//@[75:82) StringComplete |'utf-8'|
//@[82:83) RightParen |)|
//@[83:85) NewLine |\r\n|
var textLoadWrongEncoding11 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16BE')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding11|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:73) StringComplete |'Assets/encoding-utf16.txt'|
//@[73:74) Comma |,|
//@[75:85) StringComplete |'utf-16BE'|
//@[85:86) RightParen |)|
//@[86:88) NewLine |\r\n|
var textLoadWrongEncoding12 = loadTextContent('Assets/encoding-utf16.txt', 'us-ascii')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding12|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:73) StringComplete |'Assets/encoding-utf16.txt'|
//@[73:74) Comma |,|
//@[75:85) StringComplete |'us-ascii'|
//@[85:86) RightParen |)|
//@[86:88) NewLine |\r\n|
var textLoadWrongEncoding13 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding13|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:75) StringComplete |'Assets/encoding-utf16be.txt'|
//@[75:76) Comma |,|
//@[77:85) StringComplete |'utf-16'|
//@[85:86) RightParen |)|
//@[86:88) NewLine |\r\n|
var textLoadWrongEncoding14 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-8')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding14|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:75) StringComplete |'Assets/encoding-utf16be.txt'|
//@[75:76) Comma |,|
//@[77:84) StringComplete |'utf-8'|
//@[84:85) RightParen |)|
//@[85:87) NewLine |\r\n|
var textLoadWrongEncoding15 = loadTextContent('Assets/encoding-utf16be.txt', 'us-ascii')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding15|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:75) StringComplete |'Assets/encoding-utf16be.txt'|
//@[75:76) Comma |,|
//@[77:87) StringComplete |'us-ascii'|
//@[87:88) RightParen |)|
//@[88:90) NewLine |\r\n|
var textLoadWrongEncoding16 = loadTextContent('Assets/encoding-utf16be.txt', 'iso-8859-1')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding16|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:75) StringComplete |'Assets/encoding-utf16be.txt'|
//@[75:76) Comma |,|
//@[77:89) StringComplete |'iso-8859-1'|
//@[89:90) RightParen |)|
//@[90:92) NewLine |\r\n|
var textLoadWrongEncoding17 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-16BE')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding17|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:79) StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80) Comma |,|
//@[81:91) StringComplete |'utf-16BE'|
//@[91:92) RightParen |)|
//@[92:94) NewLine |\r\n|
var textLoadWrongEncoding18 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-16')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding18|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:79) StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80) Comma |,|
//@[81:89) StringComplete |'utf-16'|
//@[89:90) RightParen |)|
//@[90:92) NewLine |\r\n|
var textLoadWrongEncoding19 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-8')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding19|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:79) StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80) Comma |,|
//@[81:88) StringComplete |'utf-8'|
//@[88:89) RightParen |)|
//@[89:91) NewLine |\r\n|
var textLoadWrongEncoding20 = loadTextContent('Assets/encoding-windows1250.txt', 'us-ascii')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding20|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:79) StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80) Comma |,|
//@[81:91) StringComplete |'us-ascii'|
//@[91:92) RightParen |)|
//@[92:94) NewLine |\r\n|
var textLoadWrongEncoding21 = loadTextContent('Assets/encoding-windows1250.txt', 'iso-8859-1')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding21|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:79) StringComplete |'Assets/encoding-windows1250.txt'|
//@[79:80) Comma |,|
//@[81:93) StringComplete |'iso-8859-1'|
//@[93:94) RightParen |)|
//@[94:96) NewLine |\r\n|
var textLoadWrongEncoding22 = loadTextContent('Assets/encoding-utf8.txt', 'iso-8859-1')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding22|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:72) StringComplete |'Assets/encoding-utf8.txt'|
//@[72:73) Comma |,|
//@[74:86) StringComplete |'iso-8859-1'|
//@[86:87) RightParen |)|
//@[87:89) NewLine |\r\n|
var textLoadWrongEncoding23 = loadTextContent('Assets/encoding-utf8.txt', 'utf-16')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding23|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:72) StringComplete |'Assets/encoding-utf8.txt'|
//@[72:73) Comma |,|
//@[74:82) StringComplete |'utf-16'|
//@[82:83) RightParen |)|
//@[83:85) NewLine |\r\n|
var textLoadWrongEncoding24 = loadTextContent('Assets/encoding-utf8.txt', 'utf-16BE')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding24|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:72) StringComplete |'Assets/encoding-utf8.txt'|
//@[72:73) Comma |,|
//@[74:84) StringComplete |'utf-16BE'|
//@[84:85) RightParen |)|
//@[85:87) NewLine |\r\n|
var textLoadWrongEncoding25 = loadTextContent('Assets/encoding-utf8.txt', 'us-ascii')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding25|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:72) StringComplete |'Assets/encoding-utf8.txt'|
//@[72:73) Comma |,|
//@[74:84) StringComplete |'us-ascii'|
//@[84:85) RightParen |)|
//@[85:87) NewLine |\r\n|
var textLoadWrongEncoding26 = loadTextContent('Assets/encoding-utf8-bom.txt', 'iso-8859-1')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding26|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:76) StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[76:77) Comma |,|
//@[78:90) StringComplete |'iso-8859-1'|
//@[90:91) RightParen |)|
//@[91:93) NewLine |\r\n|
var textLoadWrongEncoding27 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-16')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding27|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:76) StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[76:77) Comma |,|
//@[78:86) StringComplete |'utf-16'|
//@[86:87) RightParen |)|
//@[87:89) NewLine |\r\n|
var textLoadWrongEncoding28 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-16BE')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding28|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:76) StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[76:77) Comma |,|
//@[78:88) StringComplete |'utf-16BE'|
//@[88:89) RightParen |)|
//@[89:91) NewLine |\r\n|
var textLoadWrongEncoding29 = loadTextContent('Assets/encoding-utf8-bom.txt', 'us-ascii')
//@[0:3) Identifier |var|
//@[4:27) Identifier |textLoadWrongEncoding29|
//@[28:29) Assignment |=|
//@[30:45) Identifier |loadTextContent|
//@[45:46) LeftParen |(|
//@[46:76) StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[76:77) Comma |,|
//@[78:88) StringComplete |'us-ascii'|
//@[88:89) RightParen |)|
//@[89:93) NewLine |\r\n\r\n|

var textOversize = loadTextContent('Assets/oversizeText.txt')
//@[0:3) Identifier |var|
//@[4:16) Identifier |textOversize|
//@[17:18) Assignment |=|
//@[19:34) Identifier |loadTextContent|
//@[34:35) LeftParen |(|
//@[35:60) StringComplete |'Assets/oversizeText.txt'|
//@[60:61) RightParen |)|
//@[61:63) NewLine |\r\n|
var binaryOversize = loadFileAsBase64('Assets/oversizeBinary')
//@[0:3) Identifier |var|
//@[4:18) Identifier |binaryOversize|
//@[19:20) Assignment |=|
//@[21:37) Identifier |loadFileAsBase64|
//@[37:38) LeftParen |(|
//@[38:61) StringComplete |'Assets/oversizeBinary'|
//@[61:62) RightParen |)|
//@[62:66) NewLine |\r\n\r\n|

var binaryAsText = loadTextContent('Assets/binary')
//@[0:3) Identifier |var|
//@[4:16) Identifier |binaryAsText|
//@[17:18) Assignment |=|
//@[19:34) Identifier |loadTextContent|
//@[34:35) LeftParen |(|
//@[35:50) StringComplete |'Assets/binary'|
//@[50:51) RightParen |)|
//@[51:51) EndOfFile ||
