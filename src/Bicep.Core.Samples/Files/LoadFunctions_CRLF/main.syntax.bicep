var loadedText1 = loadTextContent('Assets/TextFile.CRLF.txt')
//@[0:61) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |loadedText1|
//@[16:17)  Assignment |=|
//@[18:61)  FunctionCallSyntax
//@[18:33)   IdentifierSyntax
//@[18:33)    Identifier |loadTextContent|
//@[33:34)   LeftParen |(|
//@[34:60)   FunctionArgumentSyntax
//@[34:60)    StringSyntax
//@[34:60)     StringComplete |'Assets/TextFile.CRLF.txt'|
//@[60:61)   RightParen |)|
//@[61:63) NewLine |\r\n|
var loadedText2 = sys.loadTextContent('Assets/TextFile.LF.txt')
//@[0:63) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |loadedText2|
//@[16:17)  Assignment |=|
//@[18:63)  InstanceFunctionCallSyntax
//@[18:21)   VariableAccessSyntax
//@[18:21)    IdentifierSyntax
//@[18:21)     Identifier |sys|
//@[21:22)   Dot |.|
//@[22:37)   IdentifierSyntax
//@[22:37)    Identifier |loadTextContent|
//@[37:38)   LeftParen |(|
//@[38:62)   FunctionArgumentSyntax
//@[38:62)    StringSyntax
//@[38:62)     StringComplete |'Assets/TextFile.LF.txt'|
//@[62:63)   RightParen |)|
//@[63:65) NewLine |\r\n|
var loadedTextEncoding1 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[0:82) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |loadedTextEncoding1|
//@[24:25)  Assignment |=|
//@[26:82)  FunctionCallSyntax
//@[26:41)   IdentifierSyntax
//@[26:41)    Identifier |loadTextContent|
//@[41:42)   LeftParen |(|
//@[42:70)   FunctionArgumentSyntax
//@[42:69)    StringSyntax
//@[42:69)     StringComplete |'Assets/encoding-ascii.txt'|
//@[69:70)    Comma |,|
//@[71:81)   FunctionArgumentSyntax
//@[71:81)    StringSyntax
//@[71:81)     StringComplete |'us-ascii'|
//@[81:82)   RightParen |)|
//@[82:84) NewLine |\r\n|
var loadedTextEncoding2 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[0:78) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |loadedTextEncoding2|
//@[24:25)  Assignment |=|
//@[26:78)  FunctionCallSyntax
//@[26:41)   IdentifierSyntax
//@[26:41)    Identifier |loadTextContent|
//@[41:42)   LeftParen |(|
//@[42:69)   FunctionArgumentSyntax
//@[42:68)    StringSyntax
//@[42:68)     StringComplete |'Assets/encoding-utf8.txt'|
//@[68:69)    Comma |,|
//@[70:77)   FunctionArgumentSyntax
//@[70:77)    StringSyntax
//@[70:77)     StringComplete |'utf-8'|
//@[77:78)   RightParen |)|
//@[78:80) NewLine |\r\n|
var loadedTextEncoding3 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16')
//@[0:80) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |loadedTextEncoding3|
//@[24:25)  Assignment |=|
//@[26:80)  FunctionCallSyntax
//@[26:41)   IdentifierSyntax
//@[26:41)    Identifier |loadTextContent|
//@[41:42)   LeftParen |(|
//@[42:70)   FunctionArgumentSyntax
//@[42:69)    StringSyntax
//@[42:69)     StringComplete |'Assets/encoding-utf16.txt'|
//@[69:70)    Comma |,|
//@[71:79)   FunctionArgumentSyntax
//@[71:79)    StringSyntax
//@[71:79)     StringComplete |'utf-16'|
//@[79:80)   RightParen |)|
//@[80:82) NewLine |\r\n|
var loadedTextEncoding4 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16BE')
//@[0:84) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |loadedTextEncoding4|
//@[24:25)  Assignment |=|
//@[26:84)  FunctionCallSyntax
//@[26:41)   IdentifierSyntax
//@[26:41)    Identifier |loadTextContent|
//@[41:42)   LeftParen |(|
//@[42:72)   FunctionArgumentSyntax
//@[42:71)    StringSyntax
//@[42:71)     StringComplete |'Assets/encoding-utf16be.txt'|
//@[71:72)    Comma |,|
//@[73:83)   FunctionArgumentSyntax
//@[73:83)    StringSyntax
//@[73:83)     StringComplete |'utf-16BE'|
//@[83:84)   RightParen |)|
//@[84:86) NewLine |\r\n|
var loadedTextEncoding5 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[0:82) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |loadedTextEncoding5|
//@[24:25)  Assignment |=|
//@[26:82)  FunctionCallSyntax
//@[26:41)   IdentifierSyntax
//@[26:41)    Identifier |loadTextContent|
//@[41:42)   LeftParen |(|
//@[42:68)   FunctionArgumentSyntax
//@[42:67)    StringSyntax
//@[42:67)     StringComplete |'Assets/encoding-iso.txt'|
//@[67:68)    Comma |,|
//@[69:81)   FunctionArgumentSyntax
//@[69:81)    StringSyntax
//@[69:81)     StringComplete |'iso-8859-1'|
//@[81:82)   RightParen |)|
//@[82:86) NewLine |\r\n\r\n|

var loadedBinary1 = loadFileAsBase64('Assets/binary')
//@[0:53) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |loadedBinary1|
//@[18:19)  Assignment |=|
//@[20:53)  FunctionCallSyntax
//@[20:36)   IdentifierSyntax
//@[20:36)    Identifier |loadFileAsBase64|
//@[36:37)   LeftParen |(|
//@[37:52)   FunctionArgumentSyntax
//@[37:52)    StringSyntax
//@[37:52)     StringComplete |'Assets/binary'|
//@[52:53)   RightParen |)|
//@[53:55) NewLine |\r\n|
var loadedBinary2 = sys.loadFileAsBase64('Assets/binary')
//@[0:57) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |loadedBinary2|
//@[18:19)  Assignment |=|
//@[20:57)  InstanceFunctionCallSyntax
//@[20:23)   VariableAccessSyntax
//@[20:23)    IdentifierSyntax
//@[20:23)     Identifier |sys|
//@[23:24)   Dot |.|
//@[24:40)   IdentifierSyntax
//@[24:40)    Identifier |loadFileAsBase64|
//@[40:41)   LeftParen |(|
//@[41:56)   FunctionArgumentSyntax
//@[41:56)    StringSyntax
//@[41:56)     StringComplete |'Assets/binary'|
//@[56:57)   RightParen |)|
//@[57:61) NewLine |\r\n\r\n|

var loadedTextInterpolation1 = 'Text: ${loadTextContent('Assets/TextFile.CRLF.txt')}'
//@[0:85) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |loadedTextInterpolation1|
//@[29:30)  Assignment |=|
//@[31:85)  StringSyntax
//@[31:40)   StringLeftPiece |'Text: ${|
//@[40:83)   FunctionCallSyntax
//@[40:55)    IdentifierSyntax
//@[40:55)     Identifier |loadTextContent|
//@[55:56)    LeftParen |(|
//@[56:82)    FunctionArgumentSyntax
//@[56:82)     StringSyntax
//@[56:82)      StringComplete |'Assets/TextFile.CRLF.txt'|
//@[82:83)    RightParen |)|
//@[83:85)   StringRightPiece |}'|
//@[85:87) NewLine |\r\n|
var loadedTextInterpolation2 = 'Text: ${loadTextContent('Assets/TextFile.LF.txt')}'
//@[0:83) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |loadedTextInterpolation2|
//@[29:30)  Assignment |=|
//@[31:83)  StringSyntax
//@[31:40)   StringLeftPiece |'Text: ${|
//@[40:81)   FunctionCallSyntax
//@[40:55)    IdentifierSyntax
//@[40:55)     Identifier |loadTextContent|
//@[55:56)    LeftParen |(|
//@[56:80)    FunctionArgumentSyntax
//@[56:80)     StringSyntax
//@[56:80)      StringComplete |'Assets/TextFile.LF.txt'|
//@[80:81)    RightParen |)|
//@[81:83)   StringRightPiece |}'|
//@[83:87) NewLine |\r\n\r\n|

var loadedTextObject1 = {
//@[0:84) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |loadedTextObject1|
//@[22:23)  Assignment |=|
//@[24:84)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:27)   NewLine |\r\n|
  'text' : loadTextContent('Assets/TextFile.CRLF.txt')
//@[2:54)   ObjectPropertySyntax
//@[2:8)    StringSyntax
//@[2:8)     StringComplete |'text'|
//@[9:10)    Colon |:|
//@[11:54)    FunctionCallSyntax
//@[11:26)     IdentifierSyntax
//@[11:26)      Identifier |loadTextContent|
//@[26:27)     LeftParen |(|
//@[27:53)     FunctionArgumentSyntax
//@[27:53)      StringSyntax
//@[27:53)       StringComplete |'Assets/TextFile.CRLF.txt'|
//@[53:54)     RightParen |)|
//@[54:56)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
var loadedTextObject2 = {
//@[0:84) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |loadedTextObject2|
//@[22:23)  Assignment |=|
//@[24:84)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:27)   NewLine |\r\n|
  'text' : loadTextContent('Assets/TextFile.LF.txt')  
//@[2:52)   ObjectPropertySyntax
//@[2:8)    StringSyntax
//@[2:8)     StringComplete |'text'|
//@[9:10)    Colon |:|
//@[11:52)    FunctionCallSyntax
//@[11:26)     IdentifierSyntax
//@[11:26)      Identifier |loadTextContent|
//@[26:27)     LeftParen |(|
//@[27:51)     FunctionArgumentSyntax
//@[27:51)      StringSyntax
//@[27:51)       StringComplete |'Assets/TextFile.LF.txt'|
//@[51:52)     RightParen |)|
//@[54:56)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
var loadedBinaryInObject = {
//@[0:74) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |loadedBinaryInObject|
//@[25:26)  Assignment |=|
//@[27:74)  ObjectSyntax
//@[27:28)   LeftBrace |{|
//@[28:30)   NewLine |\r\n|
  file: loadFileAsBase64('Assets/binary')
//@[2:41)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |file|
//@[6:7)    Colon |:|
//@[8:41)    FunctionCallSyntax
//@[8:24)     IdentifierSyntax
//@[8:24)      Identifier |loadFileAsBase64|
//@[24:25)     LeftParen |(|
//@[25:40)     FunctionArgumentSyntax
//@[25:40)      StringSyntax
//@[25:40)       StringComplete |'Assets/binary'|
//@[40:41)     RightParen |)|
//@[41:43)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var loadedTextArray = [
//@[0:108) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |loadedTextArray|
//@[20:21)  Assignment |=|
//@[22:108)  ArraySyntax
//@[22:23)   LeftSquare |[|
//@[23:25)   NewLine |\r\n|
  loadTextContent('Assets/TextFile.LF.txt')
//@[2:43)   ArrayItemSyntax
//@[2:43)    FunctionCallSyntax
//@[2:17)     IdentifierSyntax
//@[2:17)      Identifier |loadTextContent|
//@[17:18)     LeftParen |(|
//@[18:42)     FunctionArgumentSyntax
//@[18:42)      StringSyntax
//@[18:42)       StringComplete |'Assets/TextFile.LF.txt'|
//@[42:43)     RightParen |)|
//@[43:45)   NewLine |\r\n|
  loadFileAsBase64('Assets/binary')
//@[2:35)   ArrayItemSyntax
//@[2:35)    FunctionCallSyntax
//@[2:18)     IdentifierSyntax
//@[2:18)      Identifier |loadFileAsBase64|
//@[18:19)     LeftParen |(|
//@[19:34)     FunctionArgumentSyntax
//@[19:34)      StringSyntax
//@[19:34)       StringComplete |'Assets/binary'|
//@[34:35)     RightParen |)|
//@[35:37)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

var loadedTextArrayInObject = {
//@[0:142) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |loadedTextArrayInObject|
//@[28:29)  Assignment |=|
//@[30:142)  ObjectSyntax
//@[30:31)   LeftBrace |{|
//@[31:33)   NewLine |\r\n|
  'files' : [
//@[2:106)   ObjectPropertySyntax
//@[2:9)    StringSyntax
//@[2:9)     StringComplete |'files'|
//@[10:11)    Colon |:|
//@[12:106)    ArraySyntax
//@[12:13)     LeftSquare |[|
//@[13:15)     NewLine |\r\n|
    loadTextContent('Assets/TextFile.CRLF.txt')
//@[4:47)     ArrayItemSyntax
//@[4:47)      FunctionCallSyntax
//@[4:19)       IdentifierSyntax
//@[4:19)        Identifier |loadTextContent|
//@[19:20)       LeftParen |(|
//@[20:46)       FunctionArgumentSyntax
//@[20:46)        StringSyntax
//@[20:46)         StringComplete |'Assets/TextFile.CRLF.txt'|
//@[46:47)       RightParen |)|
//@[47:49)     NewLine |\r\n|
    loadFileAsBase64('Assets/binary')
//@[4:37)     ArrayItemSyntax
//@[4:37)      FunctionCallSyntax
//@[4:20)       IdentifierSyntax
//@[4:20)        Identifier |loadFileAsBase64|
//@[20:21)       LeftParen |(|
//@[21:36)       FunctionArgumentSyntax
//@[21:36)        StringSyntax
//@[21:36)         StringComplete |'Assets/binary'|
//@[36:37)       RightParen |)|
//@[37:39)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var loadedTextArrayInObjectFunctions = {
//@[0:277) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:36)  IdentifierSyntax
//@[4:36)   Identifier |loadedTextArrayInObjectFunctions|
//@[37:38)  Assignment |=|
//@[39:277)  ObjectSyntax
//@[39:40)   LeftBrace |{|
//@[40:42)   NewLine |\r\n|
  'files' : [
//@[2:232)   ObjectPropertySyntax
//@[2:9)    StringSyntax
//@[2:9)     StringComplete |'files'|
//@[10:11)    Colon |:|
//@[12:232)    ArraySyntax
//@[12:13)     LeftSquare |[|
//@[13:15)     NewLine |\r\n|
    length(loadTextContent('Assets/TextFile.CRLF.txt'))
//@[4:55)     ArrayItemSyntax
//@[4:55)      FunctionCallSyntax
//@[4:10)       IdentifierSyntax
//@[4:10)        Identifier |length|
//@[10:11)       LeftParen |(|
//@[11:54)       FunctionArgumentSyntax
//@[11:54)        FunctionCallSyntax
//@[11:26)         IdentifierSyntax
//@[11:26)          Identifier |loadTextContent|
//@[26:27)         LeftParen |(|
//@[27:53)         FunctionArgumentSyntax
//@[27:53)          StringSyntax
//@[27:53)           StringComplete |'Assets/TextFile.CRLF.txt'|
//@[53:54)         RightParen |)|
//@[54:55)       RightParen |)|
//@[55:57)     NewLine |\r\n|
    sys.length(loadTextContent('Assets/TextFile.LF.txt'))
//@[4:57)     ArrayItemSyntax
//@[4:57)      InstanceFunctionCallSyntax
//@[4:7)       VariableAccessSyntax
//@[4:7)        IdentifierSyntax
//@[4:7)         Identifier |sys|
//@[7:8)       Dot |.|
//@[8:14)       IdentifierSyntax
//@[8:14)        Identifier |length|
//@[14:15)       LeftParen |(|
//@[15:56)       FunctionArgumentSyntax
//@[15:56)        FunctionCallSyntax
//@[15:30)         IdentifierSyntax
//@[15:30)          Identifier |loadTextContent|
//@[30:31)         LeftParen |(|
//@[31:55)         FunctionArgumentSyntax
//@[31:55)          StringSyntax
//@[31:55)           StringComplete |'Assets/TextFile.LF.txt'|
//@[55:56)         RightParen |)|
//@[56:57)       RightParen |)|
//@[57:59)     NewLine |\r\n|
    length(loadFileAsBase64('Assets/binary'))
//@[4:45)     ArrayItemSyntax
//@[4:45)      FunctionCallSyntax
//@[4:10)       IdentifierSyntax
//@[4:10)        Identifier |length|
//@[10:11)       LeftParen |(|
//@[11:44)       FunctionArgumentSyntax
//@[11:44)        FunctionCallSyntax
//@[11:27)         IdentifierSyntax
//@[11:27)          Identifier |loadFileAsBase64|
//@[27:28)         LeftParen |(|
//@[28:43)         FunctionArgumentSyntax
//@[28:43)          StringSyntax
//@[28:43)           StringComplete |'Assets/binary'|
//@[43:44)         RightParen |)|
//@[44:45)       RightParen |)|
//@[45:47)     NewLine |\r\n|
    sys.length(loadFileAsBase64('Assets/binary'))
//@[4:49)     ArrayItemSyntax
//@[4:49)      InstanceFunctionCallSyntax
//@[4:7)       VariableAccessSyntax
//@[4:7)        IdentifierSyntax
//@[4:7)         Identifier |sys|
//@[7:8)       Dot |.|
//@[8:14)       IdentifierSyntax
//@[8:14)        Identifier |length|
//@[14:15)       LeftParen |(|
//@[15:48)       FunctionArgumentSyntax
//@[15:48)        FunctionCallSyntax
//@[15:31)         IdentifierSyntax
//@[15:31)          Identifier |loadFileAsBase64|
//@[31:32)         LeftParen |(|
//@[32:47)         FunctionArgumentSyntax
//@[32:47)          StringSyntax
//@[32:47)           StringComplete |'Assets/binary'|
//@[47:48)         RightParen |)|
//@[48:49)       RightParen |)|
//@[49:51)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:7) NewLine |\r\n\r\n\r\n|


module module1 'modulea.bicep' = {
//@[0:127) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:14)  IdentifierSyntax
//@[7:14)   Identifier |module1|
//@[15:30)  StringSyntax
//@[15:30)   StringComplete |'modulea.bicep'|
//@[31:32)  Assignment |=|
//@[33:127)  ObjectSyntax
//@[33:34)   LeftBrace |{|
//@[34:36)   NewLine |\r\n|
  name: 'module1'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:17)    StringSyntax
//@[8:17)     StringComplete |'module1'|
//@[17:19)   NewLine |\r\n|
  params: {
//@[2:69)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:69)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:13)     NewLine |\r\n|
    text: loadTextContent('Assets/TextFile.LF.txt')
//@[4:51)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |text|
//@[8:9)      Colon |:|
//@[10:51)      FunctionCallSyntax
//@[10:25)       IdentifierSyntax
//@[10:25)        Identifier |loadTextContent|
//@[25:26)       LeftParen |(|
//@[26:50)       FunctionArgumentSyntax
//@[26:50)        StringSyntax
//@[26:50)         StringComplete |'Assets/TextFile.LF.txt'|
//@[50:51)       RightParen |)|
//@[51:53)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module module2 'modulea.bicep' = {
//@[0:119) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:14)  IdentifierSyntax
//@[7:14)   Identifier |module2|
//@[15:30)  StringSyntax
//@[15:30)   StringComplete |'modulea.bicep'|
//@[31:32)  Assignment |=|
//@[33:119)  ObjectSyntax
//@[33:34)   LeftBrace |{|
//@[34:36)   NewLine |\r\n|
  name: 'module2'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:17)    StringSyntax
//@[8:17)     StringComplete |'module2'|
//@[17:19)   NewLine |\r\n|
  params: {
//@[2:61)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:61)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:13)     NewLine |\r\n|
    text: loadFileAsBase64('Assets/binary')
//@[4:43)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |text|
//@[8:9)      Colon |:|
//@[10:43)      FunctionCallSyntax
//@[10:26)       IdentifierSyntax
//@[10:26)        Identifier |loadFileAsBase64|
//@[26:27)       LeftParen |(|
//@[27:42)       FunctionArgumentSyntax
//@[27:42)        StringSyntax
//@[27:42)         StringComplete |'Assets/binary'|
//@[42:43)       RightParen |)|
//@[43:45)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var textFileInSubdirectories = loadTextContent('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt')
//@[0:145) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |textFileInSubdirectories|
//@[29:30)  Assignment |=|
//@[31:145)  FunctionCallSyntax
//@[31:46)   IdentifierSyntax
//@[31:46)    Identifier |loadTextContent|
//@[46:47)   LeftParen |(|
//@[47:144)   FunctionArgumentSyntax
//@[47:144)    StringSyntax
//@[47:144)     StringComplete |'Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt'|
//@[144:145)   RightParen |)|
//@[145:147) NewLine |\r\n|
var binaryFileInSubdirectories = loadFileAsBase64('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary')
//@[0:142) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:30)  IdentifierSyntax
//@[4:30)   Identifier |binaryFileInSubdirectories|
//@[31:32)  Assignment |=|
//@[33:142)  FunctionCallSyntax
//@[33:49)   IdentifierSyntax
//@[33:49)    Identifier |loadFileAsBase64|
//@[49:50)   LeftParen |(|
//@[50:141)   FunctionArgumentSyntax
//@[50:141)    StringSyntax
//@[50:141)     StringComplete |'Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary'|
//@[141:142)   RightParen |)|
//@[142:146) NewLine |\r\n\r\n|

var loadWithEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |loadWithEncoding01|
//@[23:24)  Assignment |=|
//@[25:81)  FunctionCallSyntax
//@[25:40)   IdentifierSyntax
//@[25:40)    Identifier |loadTextContent|
//@[40:41)   LeftParen |(|
//@[41:67)   FunctionArgumentSyntax
//@[41:66)    StringSyntax
//@[41:66)     StringComplete |'Assets/encoding-iso.txt'|
//@[66:67)    Comma |,|
//@[68:80)   FunctionArgumentSyntax
//@[68:80)    StringSyntax
//@[68:80)     StringComplete |'iso-8859-1'|
//@[80:81)   RightParen |)|
//@[81:83) NewLine |\r\n|
var loadWithEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |loadWithEncoding06|
//@[23:24)  Assignment |=|
//@[25:81)  FunctionCallSyntax
//@[25:40)   IdentifierSyntax
//@[25:40)    Identifier |loadTextContent|
//@[40:41)   LeftParen |(|
//@[41:69)   FunctionArgumentSyntax
//@[41:68)    StringSyntax
//@[41:68)     StringComplete |'Assets/encoding-ascii.txt'|
//@[68:69)    Comma |,|
//@[70:80)   FunctionArgumentSyntax
//@[70:80)    StringSyntax
//@[70:80)     StringComplete |'us-ascii'|
//@[80:81)   RightParen |)|
//@[81:83) NewLine |\r\n|
var loadWithEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[0:83) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |loadWithEncoding07|
//@[23:24)  Assignment |=|
//@[25:83)  FunctionCallSyntax
//@[25:40)   IdentifierSyntax
//@[25:40)    Identifier |loadTextContent|
//@[40:41)   LeftParen |(|
//@[41:69)   FunctionArgumentSyntax
//@[41:68)    StringSyntax
//@[41:68)     StringComplete |'Assets/encoding-ascii.txt'|
//@[68:69)    Comma |,|
//@[70:82)   FunctionArgumentSyntax
//@[70:82)    StringSyntax
//@[70:82)     StringComplete |'iso-8859-1'|
//@[82:83)   RightParen |)|
//@[83:85) NewLine |\r\n|
var loadWithEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[0:78) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |loadWithEncoding08|
//@[23:24)  Assignment |=|
//@[25:78)  FunctionCallSyntax
//@[25:40)   IdentifierSyntax
//@[25:40)    Identifier |loadTextContent|
//@[40:41)   LeftParen |(|
//@[41:69)   FunctionArgumentSyntax
//@[41:68)    StringSyntax
//@[41:68)     StringComplete |'Assets/encoding-ascii.txt'|
//@[68:69)    Comma |,|
//@[70:77)   FunctionArgumentSyntax
//@[70:77)    StringSyntax
//@[70:77)     StringComplete |'utf-8'|
//@[77:78)   RightParen |)|
//@[78:80) NewLine |\r\n|
var loadWithEncoding11 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[0:77) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |loadWithEncoding11|
//@[23:24)  Assignment |=|
//@[25:77)  FunctionCallSyntax
//@[25:40)   IdentifierSyntax
//@[25:40)    Identifier |loadTextContent|
//@[40:41)   LeftParen |(|
//@[41:68)   FunctionArgumentSyntax
//@[41:67)    StringSyntax
//@[41:67)     StringComplete |'Assets/encoding-utf8.txt'|
//@[67:68)    Comma |,|
//@[69:76)   FunctionArgumentSyntax
//@[69:76)    StringSyntax
//@[69:76)     StringComplete |'utf-8'|
//@[76:77)   RightParen |)|
//@[77:79) NewLine |\r\n|
var loadWithEncoding12 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-8')
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |loadWithEncoding12|
//@[23:24)  Assignment |=|
//@[25:81)  FunctionCallSyntax
//@[25:40)   IdentifierSyntax
//@[25:40)    Identifier |loadTextContent|
//@[40:41)   LeftParen |(|
//@[41:72)   FunctionArgumentSyntax
//@[41:71)    StringSyntax
//@[41:71)     StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[71:72)    Comma |,|
//@[73:80)   FunctionArgumentSyntax
//@[73:80)    StringSyntax
//@[73:80)     StringComplete |'utf-8'|
//@[80:81)   RightParen |)|
//@[81:85) NewLine |\r\n\r\n|

var testJson = json(loadTextContent('./Assets/test.json.txt'))
//@[0:62) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:12)  IdentifierSyntax
//@[4:12)   Identifier |testJson|
//@[13:14)  Assignment |=|
//@[15:62)  FunctionCallSyntax
//@[15:19)   IdentifierSyntax
//@[15:19)    Identifier |json|
//@[19:20)   LeftParen |(|
//@[20:61)   FunctionArgumentSyntax
//@[20:61)    FunctionCallSyntax
//@[20:35)     IdentifierSyntax
//@[20:35)      Identifier |loadTextContent|
//@[35:36)     LeftParen |(|
//@[36:60)     FunctionArgumentSyntax
//@[36:60)      StringSyntax
//@[36:60)       StringComplete |'./Assets/test.json.txt'|
//@[60:61)     RightParen |)|
//@[61:62)   RightParen |)|
//@[62:64) NewLine |\r\n|
var testJsonString = testJson.string
//@[0:36) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |testJsonString|
//@[19:20)  Assignment |=|
//@[21:36)  PropertyAccessSyntax
//@[21:29)   VariableAccessSyntax
//@[21:29)    IdentifierSyntax
//@[21:29)     Identifier |testJson|
//@[29:30)   Dot |.|
//@[30:36)   IdentifierSyntax
//@[30:36)    Identifier |string|
//@[36:38) NewLine |\r\n|
var testJsonInt = testJson.int
//@[0:30) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |testJsonInt|
//@[16:17)  Assignment |=|
//@[18:30)  PropertyAccessSyntax
//@[18:26)   VariableAccessSyntax
//@[18:26)    IdentifierSyntax
//@[18:26)     Identifier |testJson|
//@[26:27)   Dot |.|
//@[27:30)   IdentifierSyntax
//@[27:30)    Identifier |int|
//@[30:32) NewLine |\r\n|
var testJsonArrayVal = testJson.array[0]
//@[0:40) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |testJsonArrayVal|
//@[21:22)  Assignment |=|
//@[23:40)  ArrayAccessSyntax
//@[23:37)   PropertyAccessSyntax
//@[23:31)    VariableAccessSyntax
//@[23:31)     IdentifierSyntax
//@[23:31)      Identifier |testJson|
//@[31:32)    Dot |.|
//@[32:37)    IdentifierSyntax
//@[32:37)     Identifier |array|
//@[37:38)   LeftSquare |[|
//@[38:39)   IntegerLiteralSyntax
//@[38:39)    Integer |0|
//@[39:40)   RightSquare |]|
//@[40:42) NewLine |\r\n|
var testJsonObject = testJson.object
//@[0:36) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |testJsonObject|
//@[19:20)  Assignment |=|
//@[21:36)  PropertyAccessSyntax
//@[21:29)   VariableAccessSyntax
//@[21:29)    IdentifierSyntax
//@[21:29)     Identifier |testJson|
//@[29:30)   Dot |.|
//@[30:36)   IdentifierSyntax
//@[30:36)    Identifier |object|
//@[36:38) NewLine |\r\n|
var testJsonNestedString = testJson.object.nestedString
//@[0:55) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |testJsonNestedString|
//@[25:26)  Assignment |=|
//@[27:55)  PropertyAccessSyntax
//@[27:42)   PropertyAccessSyntax
//@[27:35)    VariableAccessSyntax
//@[27:35)     IdentifierSyntax
//@[27:35)      Identifier |testJson|
//@[35:36)    Dot |.|
//@[36:42)    IdentifierSyntax
//@[36:42)     Identifier |object|
//@[42:43)   Dot |.|
//@[43:55)   IdentifierSyntax
//@[43:55)    Identifier |nestedString|
//@[55:57) NewLine |\r\n|

//@[0:0) EndOfFile ||
