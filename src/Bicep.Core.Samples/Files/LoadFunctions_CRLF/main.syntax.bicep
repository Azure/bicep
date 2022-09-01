var loadedText1 = loadTextContent('Assets/TextFile.CRLF.txt')
//@[000:3790) ProgramSyntax
//@[000:0061) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |loadedText1|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0061) | └─FunctionCallSyntax
//@[018:0033) |   ├─IdentifierSyntax
//@[018:0033) |   | └─Token(Identifier) |loadTextContent|
//@[033:0034) |   ├─Token(LeftParen) |(|
//@[034:0060) |   ├─FunctionArgumentSyntax
//@[034:0060) |   | └─StringSyntax
//@[034:0060) |   |   └─Token(StringComplete) |'Assets/TextFile.CRLF.txt'|
//@[060:0061) |   └─Token(RightParen) |)|
//@[061:0063) ├─Token(NewLine) |\r\n|
var loadedText2 = sys.loadTextContent('Assets/TextFile.LF.txt')
//@[000:0063) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |loadedText2|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0063) | └─InstanceFunctionCallSyntax
//@[018:0021) |   ├─VariableAccessSyntax
//@[018:0021) |   | └─IdentifierSyntax
//@[018:0021) |   |   └─Token(Identifier) |sys|
//@[021:0022) |   ├─Token(Dot) |.|
//@[022:0037) |   ├─IdentifierSyntax
//@[022:0037) |   | └─Token(Identifier) |loadTextContent|
//@[037:0038) |   ├─Token(LeftParen) |(|
//@[038:0062) |   ├─FunctionArgumentSyntax
//@[038:0062) |   | └─StringSyntax
//@[038:0062) |   |   └─Token(StringComplete) |'Assets/TextFile.LF.txt'|
//@[062:0063) |   └─Token(RightParen) |)|
//@[063:0065) ├─Token(NewLine) |\r\n|
var loadedTextEncoding1 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[000:0082) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |loadedTextEncoding1|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0082) | └─FunctionCallSyntax
//@[026:0041) |   ├─IdentifierSyntax
//@[026:0041) |   | └─Token(Identifier) |loadTextContent|
//@[041:0042) |   ├─Token(LeftParen) |(|
//@[042:0069) |   ├─FunctionArgumentSyntax
//@[042:0069) |   | └─StringSyntax
//@[042:0069) |   |   └─Token(StringComplete) |'Assets/encoding-ascii.txt'|
//@[069:0070) |   ├─Token(Comma) |,|
//@[071:0081) |   ├─FunctionArgumentSyntax
//@[071:0081) |   | └─StringSyntax
//@[071:0081) |   |   └─Token(StringComplete) |'us-ascii'|
//@[081:0082) |   └─Token(RightParen) |)|
//@[082:0084) ├─Token(NewLine) |\r\n|
var loadedTextEncoding2 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[000:0078) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |loadedTextEncoding2|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0078) | └─FunctionCallSyntax
//@[026:0041) |   ├─IdentifierSyntax
//@[026:0041) |   | └─Token(Identifier) |loadTextContent|
//@[041:0042) |   ├─Token(LeftParen) |(|
//@[042:0068) |   ├─FunctionArgumentSyntax
//@[042:0068) |   | └─StringSyntax
//@[042:0068) |   |   └─Token(StringComplete) |'Assets/encoding-utf8.txt'|
//@[068:0069) |   ├─Token(Comma) |,|
//@[070:0077) |   ├─FunctionArgumentSyntax
//@[070:0077) |   | └─StringSyntax
//@[070:0077) |   |   └─Token(StringComplete) |'utf-8'|
//@[077:0078) |   └─Token(RightParen) |)|
//@[078:0080) ├─Token(NewLine) |\r\n|
var loadedTextEncoding3 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16')
//@[000:0080) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |loadedTextEncoding3|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0080) | └─FunctionCallSyntax
//@[026:0041) |   ├─IdentifierSyntax
//@[026:0041) |   | └─Token(Identifier) |loadTextContent|
//@[041:0042) |   ├─Token(LeftParen) |(|
//@[042:0069) |   ├─FunctionArgumentSyntax
//@[042:0069) |   | └─StringSyntax
//@[042:0069) |   |   └─Token(StringComplete) |'Assets/encoding-utf16.txt'|
//@[069:0070) |   ├─Token(Comma) |,|
//@[071:0079) |   ├─FunctionArgumentSyntax
//@[071:0079) |   | └─StringSyntax
//@[071:0079) |   |   └─Token(StringComplete) |'utf-16'|
//@[079:0080) |   └─Token(RightParen) |)|
//@[080:0082) ├─Token(NewLine) |\r\n|
var loadedTextEncoding4 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16BE')
//@[000:0084) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |loadedTextEncoding4|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0084) | └─FunctionCallSyntax
//@[026:0041) |   ├─IdentifierSyntax
//@[026:0041) |   | └─Token(Identifier) |loadTextContent|
//@[041:0042) |   ├─Token(LeftParen) |(|
//@[042:0071) |   ├─FunctionArgumentSyntax
//@[042:0071) |   | └─StringSyntax
//@[042:0071) |   |   └─Token(StringComplete) |'Assets/encoding-utf16be.txt'|
//@[071:0072) |   ├─Token(Comma) |,|
//@[073:0083) |   ├─FunctionArgumentSyntax
//@[073:0083) |   | └─StringSyntax
//@[073:0083) |   |   └─Token(StringComplete) |'utf-16BE'|
//@[083:0084) |   └─Token(RightParen) |)|
//@[084:0086) ├─Token(NewLine) |\r\n|
var loadedTextEncoding5 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[000:0082) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |loadedTextEncoding5|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0082) | └─FunctionCallSyntax
//@[026:0041) |   ├─IdentifierSyntax
//@[026:0041) |   | └─Token(Identifier) |loadTextContent|
//@[041:0042) |   ├─Token(LeftParen) |(|
//@[042:0067) |   ├─FunctionArgumentSyntax
//@[042:0067) |   | └─StringSyntax
//@[042:0067) |   |   └─Token(StringComplete) |'Assets/encoding-iso.txt'|
//@[067:0068) |   ├─Token(Comma) |,|
//@[069:0081) |   ├─FunctionArgumentSyntax
//@[069:0081) |   | └─StringSyntax
//@[069:0081) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[081:0082) |   └─Token(RightParen) |)|
//@[082:0086) ├─Token(NewLine) |\r\n\r\n|

var loadedBinary1 = loadFileAsBase64('Assets/binary')
//@[000:0053) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |loadedBinary1|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0053) | └─FunctionCallSyntax
//@[020:0036) |   ├─IdentifierSyntax
//@[020:0036) |   | └─Token(Identifier) |loadFileAsBase64|
//@[036:0037) |   ├─Token(LeftParen) |(|
//@[037:0052) |   ├─FunctionArgumentSyntax
//@[037:0052) |   | └─StringSyntax
//@[037:0052) |   |   └─Token(StringComplete) |'Assets/binary'|
//@[052:0053) |   └─Token(RightParen) |)|
//@[053:0055) ├─Token(NewLine) |\r\n|
var loadedBinary2 = sys.loadFileAsBase64('Assets/binary')
//@[000:0057) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |loadedBinary2|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0057) | └─InstanceFunctionCallSyntax
//@[020:0023) |   ├─VariableAccessSyntax
//@[020:0023) |   | └─IdentifierSyntax
//@[020:0023) |   |   └─Token(Identifier) |sys|
//@[023:0024) |   ├─Token(Dot) |.|
//@[024:0040) |   ├─IdentifierSyntax
//@[024:0040) |   | └─Token(Identifier) |loadFileAsBase64|
//@[040:0041) |   ├─Token(LeftParen) |(|
//@[041:0056) |   ├─FunctionArgumentSyntax
//@[041:0056) |   | └─StringSyntax
//@[041:0056) |   |   └─Token(StringComplete) |'Assets/binary'|
//@[056:0057) |   └─Token(RightParen) |)|
//@[057:0061) ├─Token(NewLine) |\r\n\r\n|

var loadedTextInterpolation1 = 'Text: ${loadTextContent('Assets/TextFile.CRLF.txt')}'
//@[000:0085) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0028) | ├─IdentifierSyntax
//@[004:0028) | | └─Token(Identifier) |loadedTextInterpolation1|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0085) | └─StringSyntax
//@[031:0040) |   ├─Token(StringLeftPiece) |'Text: ${|
//@[040:0083) |   ├─FunctionCallSyntax
//@[040:0055) |   | ├─IdentifierSyntax
//@[040:0055) |   | | └─Token(Identifier) |loadTextContent|
//@[055:0056) |   | ├─Token(LeftParen) |(|
//@[056:0082) |   | ├─FunctionArgumentSyntax
//@[056:0082) |   | | └─StringSyntax
//@[056:0082) |   | |   └─Token(StringComplete) |'Assets/TextFile.CRLF.txt'|
//@[082:0083) |   | └─Token(RightParen) |)|
//@[083:0085) |   └─Token(StringRightPiece) |}'|
//@[085:0087) ├─Token(NewLine) |\r\n|
var loadedTextInterpolation2 = 'Text: ${loadTextContent('Assets/TextFile.LF.txt')}'
//@[000:0083) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0028) | ├─IdentifierSyntax
//@[004:0028) | | └─Token(Identifier) |loadedTextInterpolation2|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0083) | └─StringSyntax
//@[031:0040) |   ├─Token(StringLeftPiece) |'Text: ${|
//@[040:0081) |   ├─FunctionCallSyntax
//@[040:0055) |   | ├─IdentifierSyntax
//@[040:0055) |   | | └─Token(Identifier) |loadTextContent|
//@[055:0056) |   | ├─Token(LeftParen) |(|
//@[056:0080) |   | ├─FunctionArgumentSyntax
//@[056:0080) |   | | └─StringSyntax
//@[056:0080) |   | |   └─Token(StringComplete) |'Assets/TextFile.LF.txt'|
//@[080:0081) |   | └─Token(RightParen) |)|
//@[081:0083) |   └─Token(StringRightPiece) |}'|
//@[083:0087) ├─Token(NewLine) |\r\n\r\n|

var loadedTextObject1 = {
//@[000:0084) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |loadedTextObject1|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0084) | └─ObjectSyntax
//@[024:0025) |   ├─Token(LeftBrace) |{|
//@[025:0027) |   ├─Token(NewLine) |\r\n|
  'text' : loadTextContent('Assets/TextFile.CRLF.txt')
//@[002:0054) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─StringSyntax
//@[002:0008) |   | | └─Token(StringComplete) |'text'|
//@[009:0010) |   | ├─Token(Colon) |:|
//@[011:0054) |   | └─FunctionCallSyntax
//@[011:0026) |   |   ├─IdentifierSyntax
//@[011:0026) |   |   | └─Token(Identifier) |loadTextContent|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0053) |   |   ├─FunctionArgumentSyntax
//@[027:0053) |   |   | └─StringSyntax
//@[027:0053) |   |   |   └─Token(StringComplete) |'Assets/TextFile.CRLF.txt'|
//@[053:0054) |   |   └─Token(RightParen) |)|
//@[054:0056) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|
var loadedTextObject2 = {
//@[000:0084) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |loadedTextObject2|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0084) | └─ObjectSyntax
//@[024:0025) |   ├─Token(LeftBrace) |{|
//@[025:0027) |   ├─Token(NewLine) |\r\n|
  'text' : loadTextContent('Assets/TextFile.LF.txt')  
//@[002:0052) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─StringSyntax
//@[002:0008) |   | | └─Token(StringComplete) |'text'|
//@[009:0010) |   | ├─Token(Colon) |:|
//@[011:0052) |   | └─FunctionCallSyntax
//@[011:0026) |   |   ├─IdentifierSyntax
//@[011:0026) |   |   | └─Token(Identifier) |loadTextContent|
//@[026:0027) |   |   ├─Token(LeftParen) |(|
//@[027:0051) |   |   ├─FunctionArgumentSyntax
//@[027:0051) |   |   | └─StringSyntax
//@[027:0051) |   |   |   └─Token(StringComplete) |'Assets/TextFile.LF.txt'|
//@[051:0052) |   |   └─Token(RightParen) |)|
//@[054:0056) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|
var loadedBinaryInObject = {
//@[000:0074) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0024) | ├─IdentifierSyntax
//@[004:0024) | | └─Token(Identifier) |loadedBinaryInObject|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0074) | └─ObjectSyntax
//@[027:0028) |   ├─Token(LeftBrace) |{|
//@[028:0030) |   ├─Token(NewLine) |\r\n|
  file: loadFileAsBase64('Assets/binary')
//@[002:0041) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |file|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0041) |   | └─FunctionCallSyntax
//@[008:0024) |   |   ├─IdentifierSyntax
//@[008:0024) |   |   | └─Token(Identifier) |loadFileAsBase64|
//@[024:0025) |   |   ├─Token(LeftParen) |(|
//@[025:0040) |   |   ├─FunctionArgumentSyntax
//@[025:0040) |   |   | └─StringSyntax
//@[025:0040) |   |   |   └─Token(StringComplete) |'Assets/binary'|
//@[040:0041) |   |   └─Token(RightParen) |)|
//@[041:0043) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

var loadedTextArray = [
//@[000:0108) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |loadedTextArray|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0108) | └─ArraySyntax
//@[022:0023) |   ├─Token(LeftSquare) |[|
//@[023:0025) |   ├─Token(NewLine) |\r\n|
  loadTextContent('Assets/TextFile.LF.txt')
//@[002:0043) |   ├─ArrayItemSyntax
//@[002:0043) |   | └─FunctionCallSyntax
//@[002:0017) |   |   ├─IdentifierSyntax
//@[002:0017) |   |   | └─Token(Identifier) |loadTextContent|
//@[017:0018) |   |   ├─Token(LeftParen) |(|
//@[018:0042) |   |   ├─FunctionArgumentSyntax
//@[018:0042) |   |   | └─StringSyntax
//@[018:0042) |   |   |   └─Token(StringComplete) |'Assets/TextFile.LF.txt'|
//@[042:0043) |   |   └─Token(RightParen) |)|
//@[043:0045) |   ├─Token(NewLine) |\r\n|
  loadFileAsBase64('Assets/binary')
//@[002:0035) |   ├─ArrayItemSyntax
//@[002:0035) |   | └─FunctionCallSyntax
//@[002:0018) |   |   ├─IdentifierSyntax
//@[002:0018) |   |   | └─Token(Identifier) |loadFileAsBase64|
//@[018:0019) |   |   ├─Token(LeftParen) |(|
//@[019:0034) |   |   ├─FunctionArgumentSyntax
//@[019:0034) |   |   | └─StringSyntax
//@[019:0034) |   |   |   └─Token(StringComplete) |'Assets/binary'|
//@[034:0035) |   |   └─Token(RightParen) |)|
//@[035:0037) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

var loadedTextArrayInObject = {
//@[000:0142) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0027) | ├─IdentifierSyntax
//@[004:0027) | | └─Token(Identifier) |loadedTextArrayInObject|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0142) | └─ObjectSyntax
//@[030:0031) |   ├─Token(LeftBrace) |{|
//@[031:0033) |   ├─Token(NewLine) |\r\n|
  'files' : [
//@[002:0106) |   ├─ObjectPropertySyntax
//@[002:0009) |   | ├─StringSyntax
//@[002:0009) |   | | └─Token(StringComplete) |'files'|
//@[010:0011) |   | ├─Token(Colon) |:|
//@[012:0106) |   | └─ArraySyntax
//@[012:0013) |   |   ├─Token(LeftSquare) |[|
//@[013:0015) |   |   ├─Token(NewLine) |\r\n|
    loadTextContent('Assets/TextFile.CRLF.txt')
//@[004:0047) |   |   ├─ArrayItemSyntax
//@[004:0047) |   |   | └─FunctionCallSyntax
//@[004:0019) |   |   |   ├─IdentifierSyntax
//@[004:0019) |   |   |   | └─Token(Identifier) |loadTextContent|
//@[019:0020) |   |   |   ├─Token(LeftParen) |(|
//@[020:0046) |   |   |   ├─FunctionArgumentSyntax
//@[020:0046) |   |   |   | └─StringSyntax
//@[020:0046) |   |   |   |   └─Token(StringComplete) |'Assets/TextFile.CRLF.txt'|
//@[046:0047) |   |   |   └─Token(RightParen) |)|
//@[047:0049) |   |   ├─Token(NewLine) |\r\n|
    loadFileAsBase64('Assets/binary')
//@[004:0037) |   |   ├─ArrayItemSyntax
//@[004:0037) |   |   | └─FunctionCallSyntax
//@[004:0020) |   |   |   ├─IdentifierSyntax
//@[004:0020) |   |   |   | └─Token(Identifier) |loadFileAsBase64|
//@[020:0021) |   |   |   ├─Token(LeftParen) |(|
//@[021:0036) |   |   |   ├─FunctionArgumentSyntax
//@[021:0036) |   |   |   | └─StringSyntax
//@[021:0036) |   |   |   |   └─Token(StringComplete) |'Assets/binary'|
//@[036:0037) |   |   |   └─Token(RightParen) |)|
//@[037:0039) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   |   └─Token(RightSquare) |]|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

var loadedTextArrayInObjectFunctions = {
//@[000:0277) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0036) | ├─IdentifierSyntax
//@[004:0036) | | └─Token(Identifier) |loadedTextArrayInObjectFunctions|
//@[037:0038) | ├─Token(Assignment) |=|
//@[039:0277) | └─ObjectSyntax
//@[039:0040) |   ├─Token(LeftBrace) |{|
//@[040:0042) |   ├─Token(NewLine) |\r\n|
  'files' : [
//@[002:0232) |   ├─ObjectPropertySyntax
//@[002:0009) |   | ├─StringSyntax
//@[002:0009) |   | | └─Token(StringComplete) |'files'|
//@[010:0011) |   | ├─Token(Colon) |:|
//@[012:0232) |   | └─ArraySyntax
//@[012:0013) |   |   ├─Token(LeftSquare) |[|
//@[013:0015) |   |   ├─Token(NewLine) |\r\n|
    length(loadTextContent('Assets/TextFile.CRLF.txt'))
//@[004:0055) |   |   ├─ArrayItemSyntax
//@[004:0055) |   |   | └─FunctionCallSyntax
//@[004:0010) |   |   |   ├─IdentifierSyntax
//@[004:0010) |   |   |   | └─Token(Identifier) |length|
//@[010:0011) |   |   |   ├─Token(LeftParen) |(|
//@[011:0054) |   |   |   ├─FunctionArgumentSyntax
//@[011:0054) |   |   |   | └─FunctionCallSyntax
//@[011:0026) |   |   |   |   ├─IdentifierSyntax
//@[011:0026) |   |   |   |   | └─Token(Identifier) |loadTextContent|
//@[026:0027) |   |   |   |   ├─Token(LeftParen) |(|
//@[027:0053) |   |   |   |   ├─FunctionArgumentSyntax
//@[027:0053) |   |   |   |   | └─StringSyntax
//@[027:0053) |   |   |   |   |   └─Token(StringComplete) |'Assets/TextFile.CRLF.txt'|
//@[053:0054) |   |   |   |   └─Token(RightParen) |)|
//@[054:0055) |   |   |   └─Token(RightParen) |)|
//@[055:0057) |   |   ├─Token(NewLine) |\r\n|
    sys.length(loadTextContent('Assets/TextFile.LF.txt'))
//@[004:0057) |   |   ├─ArrayItemSyntax
//@[004:0057) |   |   | └─InstanceFunctionCallSyntax
//@[004:0007) |   |   |   ├─VariableAccessSyntax
//@[004:0007) |   |   |   | └─IdentifierSyntax
//@[004:0007) |   |   |   |   └─Token(Identifier) |sys|
//@[007:0008) |   |   |   ├─Token(Dot) |.|
//@[008:0014) |   |   |   ├─IdentifierSyntax
//@[008:0014) |   |   |   | └─Token(Identifier) |length|
//@[014:0015) |   |   |   ├─Token(LeftParen) |(|
//@[015:0056) |   |   |   ├─FunctionArgumentSyntax
//@[015:0056) |   |   |   | └─FunctionCallSyntax
//@[015:0030) |   |   |   |   ├─IdentifierSyntax
//@[015:0030) |   |   |   |   | └─Token(Identifier) |loadTextContent|
//@[030:0031) |   |   |   |   ├─Token(LeftParen) |(|
//@[031:0055) |   |   |   |   ├─FunctionArgumentSyntax
//@[031:0055) |   |   |   |   | └─StringSyntax
//@[031:0055) |   |   |   |   |   └─Token(StringComplete) |'Assets/TextFile.LF.txt'|
//@[055:0056) |   |   |   |   └─Token(RightParen) |)|
//@[056:0057) |   |   |   └─Token(RightParen) |)|
//@[057:0059) |   |   ├─Token(NewLine) |\r\n|
    length(loadFileAsBase64('Assets/binary'))
//@[004:0045) |   |   ├─ArrayItemSyntax
//@[004:0045) |   |   | └─FunctionCallSyntax
//@[004:0010) |   |   |   ├─IdentifierSyntax
//@[004:0010) |   |   |   | └─Token(Identifier) |length|
//@[010:0011) |   |   |   ├─Token(LeftParen) |(|
//@[011:0044) |   |   |   ├─FunctionArgumentSyntax
//@[011:0044) |   |   |   | └─FunctionCallSyntax
//@[011:0027) |   |   |   |   ├─IdentifierSyntax
//@[011:0027) |   |   |   |   | └─Token(Identifier) |loadFileAsBase64|
//@[027:0028) |   |   |   |   ├─Token(LeftParen) |(|
//@[028:0043) |   |   |   |   ├─FunctionArgumentSyntax
//@[028:0043) |   |   |   |   | └─StringSyntax
//@[028:0043) |   |   |   |   |   └─Token(StringComplete) |'Assets/binary'|
//@[043:0044) |   |   |   |   └─Token(RightParen) |)|
//@[044:0045) |   |   |   └─Token(RightParen) |)|
//@[045:0047) |   |   ├─Token(NewLine) |\r\n|
    sys.length(loadFileAsBase64('Assets/binary'))
//@[004:0049) |   |   ├─ArrayItemSyntax
//@[004:0049) |   |   | └─InstanceFunctionCallSyntax
//@[004:0007) |   |   |   ├─VariableAccessSyntax
//@[004:0007) |   |   |   | └─IdentifierSyntax
//@[004:0007) |   |   |   |   └─Token(Identifier) |sys|
//@[007:0008) |   |   |   ├─Token(Dot) |.|
//@[008:0014) |   |   |   ├─IdentifierSyntax
//@[008:0014) |   |   |   | └─Token(Identifier) |length|
//@[014:0015) |   |   |   ├─Token(LeftParen) |(|
//@[015:0048) |   |   |   ├─FunctionArgumentSyntax
//@[015:0048) |   |   |   | └─FunctionCallSyntax
//@[015:0031) |   |   |   |   ├─IdentifierSyntax
//@[015:0031) |   |   |   |   | └─Token(Identifier) |loadFileAsBase64|
//@[031:0032) |   |   |   |   ├─Token(LeftParen) |(|
//@[032:0047) |   |   |   |   ├─FunctionArgumentSyntax
//@[032:0047) |   |   |   |   | └─StringSyntax
//@[032:0047) |   |   |   |   |   └─Token(StringComplete) |'Assets/binary'|
//@[047:0048) |   |   |   |   └─Token(RightParen) |)|
//@[048:0049) |   |   |   └─Token(RightParen) |)|
//@[049:0051) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   |   └─Token(RightSquare) |]|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0007) ├─Token(NewLine) |\r\n\r\n\r\n|


module module1 'modulea.bicep' = {
//@[000:0127) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0014) | ├─IdentifierSyntax
//@[007:0014) | | └─Token(Identifier) |module1|
//@[015:0030) | ├─StringSyntax
//@[015:0030) | | └─Token(StringComplete) |'modulea.bicep'|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0127) | └─ObjectSyntax
//@[033:0034) |   ├─Token(LeftBrace) |{|
//@[034:0036) |   ├─Token(NewLine) |\r\n|
  name: 'module1'
//@[002:0017) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0017) |   | └─StringSyntax
//@[008:0017) |   |   └─Token(StringComplete) |'module1'|
//@[017:0019) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0069) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0069) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    text: loadTextContent('Assets/TextFile.LF.txt')
//@[004:0051) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |text|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0051) |   |   | └─FunctionCallSyntax
//@[010:0025) |   |   |   ├─IdentifierSyntax
//@[010:0025) |   |   |   | └─Token(Identifier) |loadTextContent|
//@[025:0026) |   |   |   ├─Token(LeftParen) |(|
//@[026:0050) |   |   |   ├─FunctionArgumentSyntax
//@[026:0050) |   |   |   | └─StringSyntax
//@[026:0050) |   |   |   |   └─Token(StringComplete) |'Assets/TextFile.LF.txt'|
//@[050:0051) |   |   |   └─Token(RightParen) |)|
//@[051:0053) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module module2 'modulea.bicep' = {
//@[000:0119) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0014) | ├─IdentifierSyntax
//@[007:0014) | | └─Token(Identifier) |module2|
//@[015:0030) | ├─StringSyntax
//@[015:0030) | | └─Token(StringComplete) |'modulea.bicep'|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0119) | └─ObjectSyntax
//@[033:0034) |   ├─Token(LeftBrace) |{|
//@[034:0036) |   ├─Token(NewLine) |\r\n|
  name: 'module2'
//@[002:0017) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0017) |   | └─StringSyntax
//@[008:0017) |   |   └─Token(StringComplete) |'module2'|
//@[017:0019) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0061) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0061) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    text: loadFileAsBase64('Assets/binary')
//@[004:0043) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |text|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0043) |   |   | └─FunctionCallSyntax
//@[010:0026) |   |   |   ├─IdentifierSyntax
//@[010:0026) |   |   |   | └─Token(Identifier) |loadFileAsBase64|
//@[026:0027) |   |   |   ├─Token(LeftParen) |(|
//@[027:0042) |   |   |   ├─FunctionArgumentSyntax
//@[027:0042) |   |   |   | └─StringSyntax
//@[027:0042) |   |   |   |   └─Token(StringComplete) |'Assets/binary'|
//@[042:0043) |   |   |   └─Token(RightParen) |)|
//@[043:0045) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

var textFileInSubdirectories = loadTextContent('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt')
//@[000:0145) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0028) | ├─IdentifierSyntax
//@[004:0028) | | └─Token(Identifier) |textFileInSubdirectories|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0145) | └─FunctionCallSyntax
//@[031:0046) |   ├─IdentifierSyntax
//@[031:0046) |   | └─Token(Identifier) |loadTextContent|
//@[046:0047) |   ├─Token(LeftParen) |(|
//@[047:0144) |   ├─FunctionArgumentSyntax
//@[047:0144) |   | └─StringSyntax
//@[047:0144) |   |   └─Token(StringComplete) |'Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt'|
//@[144:0145) |   └─Token(RightParen) |)|
//@[145:0147) ├─Token(NewLine) |\r\n|
var binaryFileInSubdirectories = loadFileAsBase64('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary')
//@[000:0142) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0030) | ├─IdentifierSyntax
//@[004:0030) | | └─Token(Identifier) |binaryFileInSubdirectories|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0142) | └─FunctionCallSyntax
//@[033:0049) |   ├─IdentifierSyntax
//@[033:0049) |   | └─Token(Identifier) |loadFileAsBase64|
//@[049:0050) |   ├─Token(LeftParen) |(|
//@[050:0141) |   ├─FunctionArgumentSyntax
//@[050:0141) |   | └─StringSyntax
//@[050:0141) |   |   └─Token(StringComplete) |'Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary'|
//@[141:0142) |   └─Token(RightParen) |)|
//@[142:0146) ├─Token(NewLine) |\r\n\r\n|

var loadWithEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[000:0081) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |loadWithEncoding01|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0081) | └─FunctionCallSyntax
//@[025:0040) |   ├─IdentifierSyntax
//@[025:0040) |   | └─Token(Identifier) |loadTextContent|
//@[040:0041) |   ├─Token(LeftParen) |(|
//@[041:0066) |   ├─FunctionArgumentSyntax
//@[041:0066) |   | └─StringSyntax
//@[041:0066) |   |   └─Token(StringComplete) |'Assets/encoding-iso.txt'|
//@[066:0067) |   ├─Token(Comma) |,|
//@[068:0080) |   ├─FunctionArgumentSyntax
//@[068:0080) |   | └─StringSyntax
//@[068:0080) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[080:0081) |   └─Token(RightParen) |)|
//@[081:0083) ├─Token(NewLine) |\r\n|
var loadWithEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[000:0081) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |loadWithEncoding06|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0081) | └─FunctionCallSyntax
//@[025:0040) |   ├─IdentifierSyntax
//@[025:0040) |   | └─Token(Identifier) |loadTextContent|
//@[040:0041) |   ├─Token(LeftParen) |(|
//@[041:0068) |   ├─FunctionArgumentSyntax
//@[041:0068) |   | └─StringSyntax
//@[041:0068) |   |   └─Token(StringComplete) |'Assets/encoding-ascii.txt'|
//@[068:0069) |   ├─Token(Comma) |,|
//@[070:0080) |   ├─FunctionArgumentSyntax
//@[070:0080) |   | └─StringSyntax
//@[070:0080) |   |   └─Token(StringComplete) |'us-ascii'|
//@[080:0081) |   └─Token(RightParen) |)|
//@[081:0083) ├─Token(NewLine) |\r\n|
var loadWithEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[000:0083) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |loadWithEncoding07|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0083) | └─FunctionCallSyntax
//@[025:0040) |   ├─IdentifierSyntax
//@[025:0040) |   | └─Token(Identifier) |loadTextContent|
//@[040:0041) |   ├─Token(LeftParen) |(|
//@[041:0068) |   ├─FunctionArgumentSyntax
//@[041:0068) |   | └─StringSyntax
//@[041:0068) |   |   └─Token(StringComplete) |'Assets/encoding-ascii.txt'|
//@[068:0069) |   ├─Token(Comma) |,|
//@[070:0082) |   ├─FunctionArgumentSyntax
//@[070:0082) |   | └─StringSyntax
//@[070:0082) |   |   └─Token(StringComplete) |'iso-8859-1'|
//@[082:0083) |   └─Token(RightParen) |)|
//@[083:0085) ├─Token(NewLine) |\r\n|
var loadWithEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[000:0078) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |loadWithEncoding08|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0078) | └─FunctionCallSyntax
//@[025:0040) |   ├─IdentifierSyntax
//@[025:0040) |   | └─Token(Identifier) |loadTextContent|
//@[040:0041) |   ├─Token(LeftParen) |(|
//@[041:0068) |   ├─FunctionArgumentSyntax
//@[041:0068) |   | └─StringSyntax
//@[041:0068) |   |   └─Token(StringComplete) |'Assets/encoding-ascii.txt'|
//@[068:0069) |   ├─Token(Comma) |,|
//@[070:0077) |   ├─FunctionArgumentSyntax
//@[070:0077) |   | └─StringSyntax
//@[070:0077) |   |   └─Token(StringComplete) |'utf-8'|
//@[077:0078) |   └─Token(RightParen) |)|
//@[078:0080) ├─Token(NewLine) |\r\n|
var loadWithEncoding11 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[000:0077) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |loadWithEncoding11|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0077) | └─FunctionCallSyntax
//@[025:0040) |   ├─IdentifierSyntax
//@[025:0040) |   | └─Token(Identifier) |loadTextContent|
//@[040:0041) |   ├─Token(LeftParen) |(|
//@[041:0067) |   ├─FunctionArgumentSyntax
//@[041:0067) |   | └─StringSyntax
//@[041:0067) |   |   └─Token(StringComplete) |'Assets/encoding-utf8.txt'|
//@[067:0068) |   ├─Token(Comma) |,|
//@[069:0076) |   ├─FunctionArgumentSyntax
//@[069:0076) |   | └─StringSyntax
//@[069:0076) |   |   └─Token(StringComplete) |'utf-8'|
//@[076:0077) |   └─Token(RightParen) |)|
//@[077:0079) ├─Token(NewLine) |\r\n|
var loadWithEncoding12 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-8')
//@[000:0081) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |loadWithEncoding12|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0081) | └─FunctionCallSyntax
//@[025:0040) |   ├─IdentifierSyntax
//@[025:0040) |   | └─Token(Identifier) |loadTextContent|
//@[040:0041) |   ├─Token(LeftParen) |(|
//@[041:0071) |   ├─FunctionArgumentSyntax
//@[041:0071) |   | └─StringSyntax
//@[041:0071) |   |   └─Token(StringComplete) |'Assets/encoding-utf8-bom.txt'|
//@[071:0072) |   ├─Token(Comma) |,|
//@[073:0080) |   ├─FunctionArgumentSyntax
//@[073:0080) |   | └─StringSyntax
//@[073:0080) |   |   └─Token(StringComplete) |'utf-8'|
//@[080:0081) |   └─Token(RightParen) |)|
//@[081:0085) ├─Token(NewLine) |\r\n\r\n|

var testJson = json(loadTextContent('./Assets/test.json.txt'))
//@[000:0062) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |testJson|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0062) | └─FunctionCallSyntax
//@[015:0019) |   ├─IdentifierSyntax
//@[015:0019) |   | └─Token(Identifier) |json|
//@[019:0020) |   ├─Token(LeftParen) |(|
//@[020:0061) |   ├─FunctionArgumentSyntax
//@[020:0061) |   | └─FunctionCallSyntax
//@[020:0035) |   |   ├─IdentifierSyntax
//@[020:0035) |   |   | └─Token(Identifier) |loadTextContent|
//@[035:0036) |   |   ├─Token(LeftParen) |(|
//@[036:0060) |   |   ├─FunctionArgumentSyntax
//@[036:0060) |   |   | └─StringSyntax
//@[036:0060) |   |   |   └─Token(StringComplete) |'./Assets/test.json.txt'|
//@[060:0061) |   |   └─Token(RightParen) |)|
//@[061:0062) |   └─Token(RightParen) |)|
//@[062:0064) ├─Token(NewLine) |\r\n|
var testJsonString = testJson.string
//@[000:0036) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |testJsonString|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0036) | └─PropertyAccessSyntax
//@[021:0029) |   ├─VariableAccessSyntax
//@[021:0029) |   | └─IdentifierSyntax
//@[021:0029) |   |   └─Token(Identifier) |testJson|
//@[029:0030) |   ├─Token(Dot) |.|
//@[030:0036) |   └─IdentifierSyntax
//@[030:0036) |     └─Token(Identifier) |string|
//@[036:0038) ├─Token(NewLine) |\r\n|
var testJsonInt = testJson.int
//@[000:0030) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |testJsonInt|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0030) | └─PropertyAccessSyntax
//@[018:0026) |   ├─VariableAccessSyntax
//@[018:0026) |   | └─IdentifierSyntax
//@[018:0026) |   |   └─Token(Identifier) |testJson|
//@[026:0027) |   ├─Token(Dot) |.|
//@[027:0030) |   └─IdentifierSyntax
//@[027:0030) |     └─Token(Identifier) |int|
//@[030:0032) ├─Token(NewLine) |\r\n|
var testJsonArrayVal = testJson.array[0]
//@[000:0040) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0020) | ├─IdentifierSyntax
//@[004:0020) | | └─Token(Identifier) |testJsonArrayVal|
//@[021:0022) | ├─Token(Assignment) |=|
//@[023:0040) | └─ArrayAccessSyntax
//@[023:0037) |   ├─PropertyAccessSyntax
//@[023:0031) |   | ├─VariableAccessSyntax
//@[023:0031) |   | | └─IdentifierSyntax
//@[023:0031) |   | |   └─Token(Identifier) |testJson|
//@[031:0032) |   | ├─Token(Dot) |.|
//@[032:0037) |   | └─IdentifierSyntax
//@[032:0037) |   |   └─Token(Identifier) |array|
//@[037:0038) |   ├─Token(LeftSquare) |[|
//@[038:0039) |   ├─IntegerLiteralSyntax
//@[038:0039) |   | └─Token(Integer) |0|
//@[039:0040) |   └─Token(RightSquare) |]|
//@[040:0042) ├─Token(NewLine) |\r\n|
var testJsonObject = testJson.object
//@[000:0036) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |testJsonObject|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0036) | └─PropertyAccessSyntax
//@[021:0029) |   ├─VariableAccessSyntax
//@[021:0029) |   | └─IdentifierSyntax
//@[021:0029) |   |   └─Token(Identifier) |testJson|
//@[029:0030) |   ├─Token(Dot) |.|
//@[030:0036) |   └─IdentifierSyntax
//@[030:0036) |     └─Token(Identifier) |object|
//@[036:0038) ├─Token(NewLine) |\r\n|
var testJsonNestedString = testJson.object.nestedString
//@[000:0055) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0024) | ├─IdentifierSyntax
//@[004:0024) | | └─Token(Identifier) |testJsonNestedString|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0055) | └─PropertyAccessSyntax
//@[027:0042) |   ├─PropertyAccessSyntax
//@[027:0035) |   | ├─VariableAccessSyntax
//@[027:0035) |   | | └─IdentifierSyntax
//@[027:0035) |   | |   └─Token(Identifier) |testJson|
//@[035:0036) |   | ├─Token(Dot) |.|
//@[036:0042) |   | └─IdentifierSyntax
//@[036:0042) |   |   └─Token(Identifier) |object|
//@[042:0043) |   ├─Token(Dot) |.|
//@[043:0055) |   └─IdentifierSyntax
//@[043:0055) |     └─Token(Identifier) |nestedString|
//@[055:0059) ├─Token(NewLine) |\r\n\r\n|

var testJson2 = loadJsonContent('./Assets/test.json.txt')
//@[000:0057) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |testJson2|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0057) | └─FunctionCallSyntax
//@[016:0031) |   ├─IdentifierSyntax
//@[016:0031) |   | └─Token(Identifier) |loadJsonContent|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0056) |   ├─FunctionArgumentSyntax
//@[032:0056) |   | └─StringSyntax
//@[032:0056) |   |   └─Token(StringComplete) |'./Assets/test.json.txt'|
//@[056:0057) |   └─Token(RightParen) |)|
//@[057:0059) ├─Token(NewLine) |\r\n|
var testJsonString2 = testJson.string
//@[000:0037) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |testJsonString2|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0037) | └─PropertyAccessSyntax
//@[022:0030) |   ├─VariableAccessSyntax
//@[022:0030) |   | └─IdentifierSyntax
//@[022:0030) |   |   └─Token(Identifier) |testJson|
//@[030:0031) |   ├─Token(Dot) |.|
//@[031:0037) |   └─IdentifierSyntax
//@[031:0037) |     └─Token(Identifier) |string|
//@[037:0039) ├─Token(NewLine) |\r\n|
var testJsonString2_1 = loadJsonContent('./Assets/test.json.txt', '.string')
//@[000:0076) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |testJsonString2_1|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0076) | └─FunctionCallSyntax
//@[024:0039) |   ├─IdentifierSyntax
//@[024:0039) |   | └─Token(Identifier) |loadJsonContent|
//@[039:0040) |   ├─Token(LeftParen) |(|
//@[040:0064) |   ├─FunctionArgumentSyntax
//@[040:0064) |   | └─StringSyntax
//@[040:0064) |   |   └─Token(StringComplete) |'./Assets/test.json.txt'|
//@[064:0065) |   ├─Token(Comma) |,|
//@[066:0075) |   ├─FunctionArgumentSyntax
//@[066:0075) |   | └─StringSyntax
//@[066:0075) |   |   └─Token(StringComplete) |'.string'|
//@[075:0076) |   └─Token(RightParen) |)|
//@[076:0078) ├─Token(NewLine) |\r\n|
var testJsonInt2 = testJson.int
//@[000:0031) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |testJsonInt2|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0031) | └─PropertyAccessSyntax
//@[019:0027) |   ├─VariableAccessSyntax
//@[019:0027) |   | └─IdentifierSyntax
//@[019:0027) |   |   └─Token(Identifier) |testJson|
//@[027:0028) |   ├─Token(Dot) |.|
//@[028:0031) |   └─IdentifierSyntax
//@[028:0031) |     └─Token(Identifier) |int|
//@[031:0033) ├─Token(NewLine) |\r\n|
var testJsonInt2_1 = loadJsonContent('./Assets/test.json.txt', '.int')
//@[000:0070) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |testJsonInt2_1|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0070) | └─FunctionCallSyntax
//@[021:0036) |   ├─IdentifierSyntax
//@[021:0036) |   | └─Token(Identifier) |loadJsonContent|
//@[036:0037) |   ├─Token(LeftParen) |(|
//@[037:0061) |   ├─FunctionArgumentSyntax
//@[037:0061) |   | └─StringSyntax
//@[037:0061) |   |   └─Token(StringComplete) |'./Assets/test.json.txt'|
//@[061:0062) |   ├─Token(Comma) |,|
//@[063:0069) |   ├─FunctionArgumentSyntax
//@[063:0069) |   | └─StringSyntax
//@[063:0069) |   |   └─Token(StringComplete) |'.int'|
//@[069:0070) |   └─Token(RightParen) |)|
//@[070:0072) ├─Token(NewLine) |\r\n|
var testJsonArrayVal2 = testJson.array[0]
//@[000:0041) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |testJsonArrayVal2|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0041) | └─ArrayAccessSyntax
//@[024:0038) |   ├─PropertyAccessSyntax
//@[024:0032) |   | ├─VariableAccessSyntax
//@[024:0032) |   | | └─IdentifierSyntax
//@[024:0032) |   | |   └─Token(Identifier) |testJson|
//@[032:0033) |   | ├─Token(Dot) |.|
//@[033:0038) |   | └─IdentifierSyntax
//@[033:0038) |   |   └─Token(Identifier) |array|
//@[038:0039) |   ├─Token(LeftSquare) |[|
//@[039:0040) |   ├─IntegerLiteralSyntax
//@[039:0040) |   | └─Token(Integer) |0|
//@[040:0041) |   └─Token(RightSquare) |]|
//@[041:0043) ├─Token(NewLine) |\r\n|
var testJsonArrayVal2_1 = loadJsonContent('./Assets/test.json.txt', '.array[0]')
//@[000:0080) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |testJsonArrayVal2_1|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0080) | └─FunctionCallSyntax
//@[026:0041) |   ├─IdentifierSyntax
//@[026:0041) |   | └─Token(Identifier) |loadJsonContent|
//@[041:0042) |   ├─Token(LeftParen) |(|
//@[042:0066) |   ├─FunctionArgumentSyntax
//@[042:0066) |   | └─StringSyntax
//@[042:0066) |   |   └─Token(StringComplete) |'./Assets/test.json.txt'|
//@[066:0067) |   ├─Token(Comma) |,|
//@[068:0079) |   ├─FunctionArgumentSyntax
//@[068:0079) |   | └─StringSyntax
//@[068:0079) |   |   └─Token(StringComplete) |'.array[0]'|
//@[079:0080) |   └─Token(RightParen) |)|
//@[080:0082) ├─Token(NewLine) |\r\n|
var testJsonObject2 = testJson.object
//@[000:0037) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |testJsonObject2|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0037) | └─PropertyAccessSyntax
//@[022:0030) |   ├─VariableAccessSyntax
//@[022:0030) |   | └─IdentifierSyntax
//@[022:0030) |   |   └─Token(Identifier) |testJson|
//@[030:0031) |   ├─Token(Dot) |.|
//@[031:0037) |   └─IdentifierSyntax
//@[031:0037) |     └─Token(Identifier) |object|
//@[037:0039) ├─Token(NewLine) |\r\n|
var testJsonObject2_1 = loadJsonContent('./Assets/test.json.txt', '.object')
//@[000:0076) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |testJsonObject2_1|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0076) | └─FunctionCallSyntax
//@[024:0039) |   ├─IdentifierSyntax
//@[024:0039) |   | └─Token(Identifier) |loadJsonContent|
//@[039:0040) |   ├─Token(LeftParen) |(|
//@[040:0064) |   ├─FunctionArgumentSyntax
//@[040:0064) |   | └─StringSyntax
//@[040:0064) |   |   └─Token(StringComplete) |'./Assets/test.json.txt'|
//@[064:0065) |   ├─Token(Comma) |,|
//@[066:0075) |   ├─FunctionArgumentSyntax
//@[066:0075) |   | └─StringSyntax
//@[066:0075) |   |   └─Token(StringComplete) |'.object'|
//@[075:0076) |   └─Token(RightParen) |)|
//@[076:0078) ├─Token(NewLine) |\r\n|
var testJsonNestedString2 = testJson.object.nestedString
//@[000:0056) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |testJsonNestedString2|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0056) | └─PropertyAccessSyntax
//@[028:0043) |   ├─PropertyAccessSyntax
//@[028:0036) |   | ├─VariableAccessSyntax
//@[028:0036) |   | | └─IdentifierSyntax
//@[028:0036) |   | |   └─Token(Identifier) |testJson|
//@[036:0037) |   | ├─Token(Dot) |.|
//@[037:0043) |   | └─IdentifierSyntax
//@[037:0043) |   |   └─Token(Identifier) |object|
//@[043:0044) |   ├─Token(Dot) |.|
//@[044:0056) |   └─IdentifierSyntax
//@[044:0056) |     └─Token(Identifier) |nestedString|
//@[056:0058) ├─Token(NewLine) |\r\n|
var testJsonNestedString2_1 = testJsonObject2_1.nestedString
//@[000:0060) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0027) | ├─IdentifierSyntax
//@[004:0027) | | └─Token(Identifier) |testJsonNestedString2_1|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0060) | └─PropertyAccessSyntax
//@[030:0047) |   ├─VariableAccessSyntax
//@[030:0047) |   | └─IdentifierSyntax
//@[030:0047) |   |   └─Token(Identifier) |testJsonObject2_1|
//@[047:0048) |   ├─Token(Dot) |.|
//@[048:0060) |   └─IdentifierSyntax
//@[048:0060) |     └─Token(Identifier) |nestedString|
//@[060:0062) ├─Token(NewLine) |\r\n|
var testJsonNestedString2_2 = loadJsonContent('./Assets/test.json.txt', '.object.nestedString')
//@[000:0095) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0027) | ├─IdentifierSyntax
//@[004:0027) | | └─Token(Identifier) |testJsonNestedString2_2|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0095) | └─FunctionCallSyntax
//@[030:0045) |   ├─IdentifierSyntax
//@[030:0045) |   | └─Token(Identifier) |loadJsonContent|
//@[045:0046) |   ├─Token(LeftParen) |(|
//@[046:0070) |   ├─FunctionArgumentSyntax
//@[046:0070) |   | └─StringSyntax
//@[046:0070) |   |   └─Token(StringComplete) |'./Assets/test.json.txt'|
//@[070:0071) |   ├─Token(Comma) |,|
//@[072:0094) |   ├─FunctionArgumentSyntax
//@[072:0094) |   | └─StringSyntax
//@[072:0094) |   |   └─Token(StringComplete) |'.object.nestedString'|
//@[094:0095) |   └─Token(RightParen) |)|
//@[095:0099) ├─Token(NewLine) |\r\n\r\n|

var testJsonTokensAsArray = loadJsonContent('./Assets/test2.json.txt', '.products[?(@.price > 3)].name')
//@[000:0104) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |testJsonTokensAsArray|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0104) | └─FunctionCallSyntax
//@[028:0043) |   ├─IdentifierSyntax
//@[028:0043) |   | └─Token(Identifier) |loadJsonContent|
//@[043:0044) |   ├─Token(LeftParen) |(|
//@[044:0069) |   ├─FunctionArgumentSyntax
//@[044:0069) |   | └─StringSyntax
//@[044:0069) |   |   └─Token(StringComplete) |'./Assets/test2.json.txt'|
//@[069:0070) |   ├─Token(Comma) |,|
//@[071:0103) |   ├─FunctionArgumentSyntax
//@[071:0103) |   | └─StringSyntax
//@[071:0103) |   |   └─Token(StringComplete) |'.products[?(@.price > 3)].name'|
//@[103:0104) |   └─Token(RightParen) |)|
//@[104:0106) ├─Token(NewLine) |\r\n|

//@[000:0000) └─Token(EndOfFile) ||
