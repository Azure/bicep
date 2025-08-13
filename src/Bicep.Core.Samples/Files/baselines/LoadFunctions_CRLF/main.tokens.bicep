var loadedText1 = loadTextContent('Assets/TextFile.CRLF.txt')
//@[000:003) Identifier |var|
//@[004:015) Identifier |loadedText1|
//@[016:017) Assignment |=|
//@[018:033) Identifier |loadTextContent|
//@[033:034) LeftParen |(|
//@[034:060) StringComplete |'Assets/TextFile.CRLF.txt'|
//@[060:061) RightParen |)|
//@[061:063) NewLine |\r\n|
var loadedText2 = sys.loadTextContent('Assets/TextFile.LF.txt')
//@[000:003) Identifier |var|
//@[004:015) Identifier |loadedText2|
//@[016:017) Assignment |=|
//@[018:021) Identifier |sys|
//@[021:022) Dot |.|
//@[022:037) Identifier |loadTextContent|
//@[037:038) LeftParen |(|
//@[038:062) StringComplete |'Assets/TextFile.LF.txt'|
//@[062:063) RightParen |)|
//@[063:065) NewLine |\r\n|
var loadedTextEncoding1 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[000:003) Identifier |var|
//@[004:023) Identifier |loadedTextEncoding1|
//@[024:025) Assignment |=|
//@[026:041) Identifier |loadTextContent|
//@[041:042) LeftParen |(|
//@[042:069) StringComplete |'Assets/encoding-ascii.txt'|
//@[069:070) Comma |,|
//@[071:081) StringComplete |'us-ascii'|
//@[081:082) RightParen |)|
//@[082:084) NewLine |\r\n|
var loadedTextEncoding2 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[000:003) Identifier |var|
//@[004:023) Identifier |loadedTextEncoding2|
//@[024:025) Assignment |=|
//@[026:041) Identifier |loadTextContent|
//@[041:042) LeftParen |(|
//@[042:068) StringComplete |'Assets/encoding-utf8.txt'|
//@[068:069) Comma |,|
//@[070:077) StringComplete |'utf-8'|
//@[077:078) RightParen |)|
//@[078:080) NewLine |\r\n|
var loadedTextEncoding3 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16')
//@[000:003) Identifier |var|
//@[004:023) Identifier |loadedTextEncoding3|
//@[024:025) Assignment |=|
//@[026:041) Identifier |loadTextContent|
//@[041:042) LeftParen |(|
//@[042:069) StringComplete |'Assets/encoding-utf16.txt'|
//@[069:070) Comma |,|
//@[071:079) StringComplete |'utf-16'|
//@[079:080) RightParen |)|
//@[080:082) NewLine |\r\n|
var loadedTextEncoding4 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16BE')
//@[000:003) Identifier |var|
//@[004:023) Identifier |loadedTextEncoding4|
//@[024:025) Assignment |=|
//@[026:041) Identifier |loadTextContent|
//@[041:042) LeftParen |(|
//@[042:071) StringComplete |'Assets/encoding-utf16be.txt'|
//@[071:072) Comma |,|
//@[073:083) StringComplete |'utf-16BE'|
//@[083:084) RightParen |)|
//@[084:086) NewLine |\r\n|
var loadedTextEncoding5 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[000:003) Identifier |var|
//@[004:023) Identifier |loadedTextEncoding5|
//@[024:025) Assignment |=|
//@[026:041) Identifier |loadTextContent|
//@[041:042) LeftParen |(|
//@[042:067) StringComplete |'Assets/encoding-iso.txt'|
//@[067:068) Comma |,|
//@[069:081) StringComplete |'iso-8859-1'|
//@[081:082) RightParen |)|
//@[082:086) NewLine |\r\n\r\n|

var loadedBinary1 = loadFileAsBase64('Assets/binary')
//@[000:003) Identifier |var|
//@[004:017) Identifier |loadedBinary1|
//@[018:019) Assignment |=|
//@[020:036) Identifier |loadFileAsBase64|
//@[036:037) LeftParen |(|
//@[037:052) StringComplete |'Assets/binary'|
//@[052:053) RightParen |)|
//@[053:055) NewLine |\r\n|
var loadedBinary2 = sys.loadFileAsBase64('Assets/binary')
//@[000:003) Identifier |var|
//@[004:017) Identifier |loadedBinary2|
//@[018:019) Assignment |=|
//@[020:023) Identifier |sys|
//@[023:024) Dot |.|
//@[024:040) Identifier |loadFileAsBase64|
//@[040:041) LeftParen |(|
//@[041:056) StringComplete |'Assets/binary'|
//@[056:057) RightParen |)|
//@[057:061) NewLine |\r\n\r\n|

var loadedTextInterpolation1 = 'Text: ${loadTextContent('Assets/TextFile.CRLF.txt')}'
//@[000:003) Identifier |var|
//@[004:028) Identifier |loadedTextInterpolation1|
//@[029:030) Assignment |=|
//@[031:040) StringLeftPiece |'Text: ${|
//@[040:055) Identifier |loadTextContent|
//@[055:056) LeftParen |(|
//@[056:082) StringComplete |'Assets/TextFile.CRLF.txt'|
//@[082:083) RightParen |)|
//@[083:085) StringRightPiece |}'|
//@[085:087) NewLine |\r\n|
var loadedTextInterpolation2 = 'Text: ${loadTextContent('Assets/TextFile.LF.txt')}'
//@[000:003) Identifier |var|
//@[004:028) Identifier |loadedTextInterpolation2|
//@[029:030) Assignment |=|
//@[031:040) StringLeftPiece |'Text: ${|
//@[040:055) Identifier |loadTextContent|
//@[055:056) LeftParen |(|
//@[056:080) StringComplete |'Assets/TextFile.LF.txt'|
//@[080:081) RightParen |)|
//@[081:083) StringRightPiece |}'|
//@[083:087) NewLine |\r\n\r\n|

var loadedTextObject1 = {
//@[000:003) Identifier |var|
//@[004:021) Identifier |loadedTextObject1|
//@[022:023) Assignment |=|
//@[024:025) LeftBrace |{|
//@[025:027) NewLine |\r\n|
  'text' : loadTextContent('Assets/TextFile.CRLF.txt')
//@[002:008) StringComplete |'text'|
//@[009:010) Colon |:|
//@[011:026) Identifier |loadTextContent|
//@[026:027) LeftParen |(|
//@[027:053) StringComplete |'Assets/TextFile.CRLF.txt'|
//@[053:054) RightParen |)|
//@[054:056) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
var loadedTextObject2 = {
//@[000:003) Identifier |var|
//@[004:021) Identifier |loadedTextObject2|
//@[022:023) Assignment |=|
//@[024:025) LeftBrace |{|
//@[025:027) NewLine |\r\n|
  'text' : loadTextContent('Assets/TextFile.LF.txt')  
//@[002:008) StringComplete |'text'|
//@[009:010) Colon |:|
//@[011:026) Identifier |loadTextContent|
//@[026:027) LeftParen |(|
//@[027:051) StringComplete |'Assets/TextFile.LF.txt'|
//@[051:052) RightParen |)|
//@[054:056) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
var loadedBinaryInObject = {
//@[000:003) Identifier |var|
//@[004:024) Identifier |loadedBinaryInObject|
//@[025:026) Assignment |=|
//@[027:028) LeftBrace |{|
//@[028:030) NewLine |\r\n|
  file: loadFileAsBase64('Assets/binary')
//@[002:006) Identifier |file|
//@[006:007) Colon |:|
//@[008:024) Identifier |loadFileAsBase64|
//@[024:025) LeftParen |(|
//@[025:040) StringComplete |'Assets/binary'|
//@[040:041) RightParen |)|
//@[041:043) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var loadedTextArray = [
//@[000:003) Identifier |var|
//@[004:019) Identifier |loadedTextArray|
//@[020:021) Assignment |=|
//@[022:023) LeftSquare |[|
//@[023:025) NewLine |\r\n|
  loadTextContent('Assets/TextFile.LF.txt')
//@[002:017) Identifier |loadTextContent|
//@[017:018) LeftParen |(|
//@[018:042) StringComplete |'Assets/TextFile.LF.txt'|
//@[042:043) RightParen |)|
//@[043:045) NewLine |\r\n|
  loadFileAsBase64('Assets/binary')
//@[002:018) Identifier |loadFileAsBase64|
//@[018:019) LeftParen |(|
//@[019:034) StringComplete |'Assets/binary'|
//@[034:035) RightParen |)|
//@[035:037) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

var loadedTextArrayInObject = {
//@[000:003) Identifier |var|
//@[004:027) Identifier |loadedTextArrayInObject|
//@[028:029) Assignment |=|
//@[030:031) LeftBrace |{|
//@[031:033) NewLine |\r\n|
  'files' : [
//@[002:009) StringComplete |'files'|
//@[010:011) Colon |:|
//@[012:013) LeftSquare |[|
//@[013:015) NewLine |\r\n|
    loadTextContent('Assets/TextFile.CRLF.txt')
//@[004:019) Identifier |loadTextContent|
//@[019:020) LeftParen |(|
//@[020:046) StringComplete |'Assets/TextFile.CRLF.txt'|
//@[046:047) RightParen |)|
//@[047:049) NewLine |\r\n|
    loadFileAsBase64('Assets/binary')
//@[004:020) Identifier |loadFileAsBase64|
//@[020:021) LeftParen |(|
//@[021:036) StringComplete |'Assets/binary'|
//@[036:037) RightParen |)|
//@[037:039) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var loadedTextArrayInObjectFunctions = {
//@[000:003) Identifier |var|
//@[004:036) Identifier |loadedTextArrayInObjectFunctions|
//@[037:038) Assignment |=|
//@[039:040) LeftBrace |{|
//@[040:042) NewLine |\r\n|
  'files' : [
//@[002:009) StringComplete |'files'|
//@[010:011) Colon |:|
//@[012:013) LeftSquare |[|
//@[013:015) NewLine |\r\n|
    length(loadTextContent('Assets/TextFile.CRLF.txt'))
//@[004:010) Identifier |length|
//@[010:011) LeftParen |(|
//@[011:026) Identifier |loadTextContent|
//@[026:027) LeftParen |(|
//@[027:053) StringComplete |'Assets/TextFile.CRLF.txt'|
//@[053:054) RightParen |)|
//@[054:055) RightParen |)|
//@[055:057) NewLine |\r\n|
    sys.length(loadTextContent('Assets/TextFile.LF.txt'))
//@[004:007) Identifier |sys|
//@[007:008) Dot |.|
//@[008:014) Identifier |length|
//@[014:015) LeftParen |(|
//@[015:030) Identifier |loadTextContent|
//@[030:031) LeftParen |(|
//@[031:055) StringComplete |'Assets/TextFile.LF.txt'|
//@[055:056) RightParen |)|
//@[056:057) RightParen |)|
//@[057:059) NewLine |\r\n|
    length(loadFileAsBase64('Assets/binary'))
//@[004:010) Identifier |length|
//@[010:011) LeftParen |(|
//@[011:027) Identifier |loadFileAsBase64|
//@[027:028) LeftParen |(|
//@[028:043) StringComplete |'Assets/binary'|
//@[043:044) RightParen |)|
//@[044:045) RightParen |)|
//@[045:047) NewLine |\r\n|
    sys.length(loadFileAsBase64('Assets/binary'))
//@[004:007) Identifier |sys|
//@[007:008) Dot |.|
//@[008:014) Identifier |length|
//@[014:015) LeftParen |(|
//@[015:031) Identifier |loadFileAsBase64|
//@[031:032) LeftParen |(|
//@[032:047) StringComplete |'Assets/binary'|
//@[047:048) RightParen |)|
//@[048:049) RightParen |)|
//@[049:051) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:007) NewLine |\r\n\r\n\r\n|


module module1 'modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:014) Identifier |module1|
//@[015:030) StringComplete |'modulea.bicep'|
//@[031:032) Assignment |=|
//@[033:034) LeftBrace |{|
//@[034:036) NewLine |\r\n|
  name: 'module1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringComplete |'module1'|
//@[017:019) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    text: loadTextContent('Assets/TextFile.LF.txt')
//@[004:008) Identifier |text|
//@[008:009) Colon |:|
//@[010:025) Identifier |loadTextContent|
//@[025:026) LeftParen |(|
//@[026:050) StringComplete |'Assets/TextFile.LF.txt'|
//@[050:051) RightParen |)|
//@[051:053) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module module2 'modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:014) Identifier |module2|
//@[015:030) StringComplete |'modulea.bicep'|
//@[031:032) Assignment |=|
//@[033:034) LeftBrace |{|
//@[034:036) NewLine |\r\n|
  name: 'module2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringComplete |'module2'|
//@[017:019) NewLine |\r\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\r\n|
    text: loadFileAsBase64('Assets/binary')
//@[004:008) Identifier |text|
//@[008:009) Colon |:|
//@[010:026) Identifier |loadFileAsBase64|
//@[026:027) LeftParen |(|
//@[027:042) StringComplete |'Assets/binary'|
//@[042:043) RightParen |)|
//@[043:045) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var textFileInSubdirectories = loadTextContent('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt')
//@[000:003) Identifier |var|
//@[004:028) Identifier |textFileInSubdirectories|
//@[029:030) Assignment |=|
//@[031:046) Identifier |loadTextContent|
//@[046:047) LeftParen |(|
//@[047:144) StringComplete |'Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt'|
//@[144:145) RightParen |)|
//@[145:147) NewLine |\r\n|
var binaryFileInSubdirectories = loadFileAsBase64('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary')
//@[000:003) Identifier |var|
//@[004:030) Identifier |binaryFileInSubdirectories|
//@[031:032) Assignment |=|
//@[033:049) Identifier |loadFileAsBase64|
//@[049:050) LeftParen |(|
//@[050:141) StringComplete |'Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary'|
//@[141:142) RightParen |)|
//@[142:146) NewLine |\r\n\r\n|

var loadWithEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[000:003) Identifier |var|
//@[004:022) Identifier |loadWithEncoding01|
//@[023:024) Assignment |=|
//@[025:040) Identifier |loadTextContent|
//@[040:041) LeftParen |(|
//@[041:066) StringComplete |'Assets/encoding-iso.txt'|
//@[066:067) Comma |,|
//@[068:080) StringComplete |'iso-8859-1'|
//@[080:081) RightParen |)|
//@[081:083) NewLine |\r\n|
var loadWithEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[000:003) Identifier |var|
//@[004:022) Identifier |loadWithEncoding06|
//@[023:024) Assignment |=|
//@[025:040) Identifier |loadTextContent|
//@[040:041) LeftParen |(|
//@[041:068) StringComplete |'Assets/encoding-ascii.txt'|
//@[068:069) Comma |,|
//@[070:080) StringComplete |'us-ascii'|
//@[080:081) RightParen |)|
//@[081:083) NewLine |\r\n|
var loadWithEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[000:003) Identifier |var|
//@[004:022) Identifier |loadWithEncoding07|
//@[023:024) Assignment |=|
//@[025:040) Identifier |loadTextContent|
//@[040:041) LeftParen |(|
//@[041:068) StringComplete |'Assets/encoding-ascii.txt'|
//@[068:069) Comma |,|
//@[070:082) StringComplete |'iso-8859-1'|
//@[082:083) RightParen |)|
//@[083:085) NewLine |\r\n|
var loadWithEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[000:003) Identifier |var|
//@[004:022) Identifier |loadWithEncoding08|
//@[023:024) Assignment |=|
//@[025:040) Identifier |loadTextContent|
//@[040:041) LeftParen |(|
//@[041:068) StringComplete |'Assets/encoding-ascii.txt'|
//@[068:069) Comma |,|
//@[070:077) StringComplete |'utf-8'|
//@[077:078) RightParen |)|
//@[078:080) NewLine |\r\n|
var loadWithEncoding11 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[000:003) Identifier |var|
//@[004:022) Identifier |loadWithEncoding11|
//@[023:024) Assignment |=|
//@[025:040) Identifier |loadTextContent|
//@[040:041) LeftParen |(|
//@[041:067) StringComplete |'Assets/encoding-utf8.txt'|
//@[067:068) Comma |,|
//@[069:076) StringComplete |'utf-8'|
//@[076:077) RightParen |)|
//@[077:079) NewLine |\r\n|
var loadWithEncoding12 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-8')
//@[000:003) Identifier |var|
//@[004:022) Identifier |loadWithEncoding12|
//@[023:024) Assignment |=|
//@[025:040) Identifier |loadTextContent|
//@[040:041) LeftParen |(|
//@[041:071) StringComplete |'Assets/encoding-utf8-bom.txt'|
//@[071:072) Comma |,|
//@[073:080) StringComplete |'utf-8'|
//@[080:081) RightParen |)|
//@[081:085) NewLine |\r\n\r\n|

var testJson = json(loadTextContent('./Assets/test.json.txt'))
//@[000:003) Identifier |var|
//@[004:012) Identifier |testJson|
//@[013:014) Assignment |=|
//@[015:019) Identifier |json|
//@[019:020) LeftParen |(|
//@[020:035) Identifier |loadTextContent|
//@[035:036) LeftParen |(|
//@[036:060) StringComplete |'./Assets/test.json.txt'|
//@[060:061) RightParen |)|
//@[061:062) RightParen |)|
//@[062:064) NewLine |\r\n|
var testJsonString = testJson.string
//@[000:003) Identifier |var|
//@[004:018) Identifier |testJsonString|
//@[019:020) Assignment |=|
//@[021:029) Identifier |testJson|
//@[029:030) Dot |.|
//@[030:036) Identifier |string|
//@[036:038) NewLine |\r\n|
var testJsonInt = testJson.int
//@[000:003) Identifier |var|
//@[004:015) Identifier |testJsonInt|
//@[016:017) Assignment |=|
//@[018:026) Identifier |testJson|
//@[026:027) Dot |.|
//@[027:030) Identifier |int|
//@[030:032) NewLine |\r\n|
var testJsonArrayVal = testJson.array[0]
//@[000:003) Identifier |var|
//@[004:020) Identifier |testJsonArrayVal|
//@[021:022) Assignment |=|
//@[023:031) Identifier |testJson|
//@[031:032) Dot |.|
//@[032:037) Identifier |array|
//@[037:038) LeftSquare |[|
//@[038:039) Integer |0|
//@[039:040) RightSquare |]|
//@[040:042) NewLine |\r\n|
var testJsonObject = testJson.object
//@[000:003) Identifier |var|
//@[004:018) Identifier |testJsonObject|
//@[019:020) Assignment |=|
//@[021:029) Identifier |testJson|
//@[029:030) Dot |.|
//@[030:036) Identifier |object|
//@[036:038) NewLine |\r\n|
var testJsonNestedString = testJson.object.nestedString
//@[000:003) Identifier |var|
//@[004:024) Identifier |testJsonNestedString|
//@[025:026) Assignment |=|
//@[027:035) Identifier |testJson|
//@[035:036) Dot |.|
//@[036:042) Identifier |object|
//@[042:043) Dot |.|
//@[043:055) Identifier |nestedString|
//@[055:059) NewLine |\r\n\r\n|

var testJson2 = loadJsonContent('./Assets/test.json.txt')
//@[000:003) Identifier |var|
//@[004:013) Identifier |testJson2|
//@[014:015) Assignment |=|
//@[016:031) Identifier |loadJsonContent|
//@[031:032) LeftParen |(|
//@[032:056) StringComplete |'./Assets/test.json.txt'|
//@[056:057) RightParen |)|
//@[057:059) NewLine |\r\n|
var testJsonString2 = testJson.string
//@[000:003) Identifier |var|
//@[004:019) Identifier |testJsonString2|
//@[020:021) Assignment |=|
//@[022:030) Identifier |testJson|
//@[030:031) Dot |.|
//@[031:037) Identifier |string|
//@[037:039) NewLine |\r\n|
var testJsonString2_1 = loadJsonContent('./Assets/test.json.txt', '.string')
//@[000:003) Identifier |var|
//@[004:021) Identifier |testJsonString2_1|
//@[022:023) Assignment |=|
//@[024:039) Identifier |loadJsonContent|
//@[039:040) LeftParen |(|
//@[040:064) StringComplete |'./Assets/test.json.txt'|
//@[064:065) Comma |,|
//@[066:075) StringComplete |'.string'|
//@[075:076) RightParen |)|
//@[076:078) NewLine |\r\n|
var testJsonInt2 = testJson.int
//@[000:003) Identifier |var|
//@[004:016) Identifier |testJsonInt2|
//@[017:018) Assignment |=|
//@[019:027) Identifier |testJson|
//@[027:028) Dot |.|
//@[028:031) Identifier |int|
//@[031:033) NewLine |\r\n|
var testJsonInt2_1 = loadJsonContent('./Assets/test.json.txt', '.int')
//@[000:003) Identifier |var|
//@[004:018) Identifier |testJsonInt2_1|
//@[019:020) Assignment |=|
//@[021:036) Identifier |loadJsonContent|
//@[036:037) LeftParen |(|
//@[037:061) StringComplete |'./Assets/test.json.txt'|
//@[061:062) Comma |,|
//@[063:069) StringComplete |'.int'|
//@[069:070) RightParen |)|
//@[070:072) NewLine |\r\n|
var testJsonArrayVal2 = testJson.array[0]
//@[000:003) Identifier |var|
//@[004:021) Identifier |testJsonArrayVal2|
//@[022:023) Assignment |=|
//@[024:032) Identifier |testJson|
//@[032:033) Dot |.|
//@[033:038) Identifier |array|
//@[038:039) LeftSquare |[|
//@[039:040) Integer |0|
//@[040:041) RightSquare |]|
//@[041:043) NewLine |\r\n|
var testJsonArrayVal2_1 = loadJsonContent('./Assets/test.json.txt', '.array[0]')
//@[000:003) Identifier |var|
//@[004:023) Identifier |testJsonArrayVal2_1|
//@[024:025) Assignment |=|
//@[026:041) Identifier |loadJsonContent|
//@[041:042) LeftParen |(|
//@[042:066) StringComplete |'./Assets/test.json.txt'|
//@[066:067) Comma |,|
//@[068:079) StringComplete |'.array[0]'|
//@[079:080) RightParen |)|
//@[080:082) NewLine |\r\n|
var testJsonObject2 = testJson.object
//@[000:003) Identifier |var|
//@[004:019) Identifier |testJsonObject2|
//@[020:021) Assignment |=|
//@[022:030) Identifier |testJson|
//@[030:031) Dot |.|
//@[031:037) Identifier |object|
//@[037:039) NewLine |\r\n|
var testJsonObject2_1 = loadJsonContent('./Assets/test.json.txt', '.object')
//@[000:003) Identifier |var|
//@[004:021) Identifier |testJsonObject2_1|
//@[022:023) Assignment |=|
//@[024:039) Identifier |loadJsonContent|
//@[039:040) LeftParen |(|
//@[040:064) StringComplete |'./Assets/test.json.txt'|
//@[064:065) Comma |,|
//@[066:075) StringComplete |'.object'|
//@[075:076) RightParen |)|
//@[076:078) NewLine |\r\n|
var testJsonNestedString2 = testJson.object.nestedString
//@[000:003) Identifier |var|
//@[004:025) Identifier |testJsonNestedString2|
//@[026:027) Assignment |=|
//@[028:036) Identifier |testJson|
//@[036:037) Dot |.|
//@[037:043) Identifier |object|
//@[043:044) Dot |.|
//@[044:056) Identifier |nestedString|
//@[056:058) NewLine |\r\n|
var testJsonNestedString2_1 = testJsonObject2_1.nestedString
//@[000:003) Identifier |var|
//@[004:027) Identifier |testJsonNestedString2_1|
//@[028:029) Assignment |=|
//@[030:047) Identifier |testJsonObject2_1|
//@[047:048) Dot |.|
//@[048:060) Identifier |nestedString|
//@[060:062) NewLine |\r\n|
var testJsonNestedString2_2 = loadJsonContent('./Assets/test.json.txt', '.object.nestedString')
//@[000:003) Identifier |var|
//@[004:027) Identifier |testJsonNestedString2_2|
//@[028:029) Assignment |=|
//@[030:045) Identifier |loadJsonContent|
//@[045:046) LeftParen |(|
//@[046:070) StringComplete |'./Assets/test.json.txt'|
//@[070:071) Comma |,|
//@[072:094) StringComplete |'.object.nestedString'|
//@[094:095) RightParen |)|
//@[095:099) NewLine |\r\n\r\n|

var testJsonTokensAsArray = loadJsonContent('./Assets/test2.json.txt', '.products[?(@.price > 3)].name')
//@[000:003) Identifier |var|
//@[004:025) Identifier |testJsonTokensAsArray|
//@[026:027) Assignment |=|
//@[028:043) Identifier |loadJsonContent|
//@[043:044) LeftParen |(|
//@[044:069) StringComplete |'./Assets/test2.json.txt'|
//@[069:070) Comma |,|
//@[071:103) StringComplete |'.products[?(@.price > 3)].name'|
//@[103:104) RightParen |)|
//@[104:108) NewLine |\r\n\r\n|

var directoryInfo = loadDirectoryFileInfo('./Assets')
//@[000:003) Identifier |var|
//@[004:017) Identifier |directoryInfo|
//@[018:019) Assignment |=|
//@[020:041) Identifier |loadDirectoryFileInfo|
//@[041:042) LeftParen |(|
//@[042:052) StringComplete |'./Assets'|
//@[052:053) RightParen |)|
//@[053:055) NewLine |\r\n|
var directoryInfoWildcard = loadDirectoryFileInfo('./Assets', '*.txt')
//@[000:003) Identifier |var|
//@[004:025) Identifier |directoryInfoWildcard|
//@[026:027) Assignment |=|
//@[028:049) Identifier |loadDirectoryFileInfo|
//@[049:050) LeftParen |(|
//@[050:060) StringComplete |'./Assets'|
//@[060:061) Comma |,|
//@[062:069) StringComplete |'*.txt'|
//@[069:070) RightParen |)|
//@[070:074) NewLine |\r\n\r\n|

var testYaml = loadYamlContent('./Assets/test.yaml.txt')
//@[000:003) Identifier |var|
//@[004:012) Identifier |testYaml|
//@[013:014) Assignment |=|
//@[015:030) Identifier |loadYamlContent|
//@[030:031) LeftParen |(|
//@[031:055) StringComplete |'./Assets/test.yaml.txt'|
//@[055:056) RightParen |)|
//@[056:058) NewLine |\r\n|
var testYamlString = testYaml.string
//@[000:003) Identifier |var|
//@[004:018) Identifier |testYamlString|
//@[019:020) Assignment |=|
//@[021:029) Identifier |testYaml|
//@[029:030) Dot |.|
//@[030:036) Identifier |string|
//@[036:038) NewLine |\r\n|
var testYamlInt = testYaml.int
//@[000:003) Identifier |var|
//@[004:015) Identifier |testYamlInt|
//@[016:017) Assignment |=|
//@[018:026) Identifier |testYaml|
//@[026:027) Dot |.|
//@[027:030) Identifier |int|
//@[030:032) NewLine |\r\n|
var testYamlBool = testYaml.bool
//@[000:003) Identifier |var|
//@[004:016) Identifier |testYamlBool|
//@[017:018) Assignment |=|
//@[019:027) Identifier |testYaml|
//@[027:028) Dot |.|
//@[028:032) Identifier |bool|
//@[032:034) NewLine |\r\n|
var testYamlArrayInt = testYaml.arrayInt
//@[000:003) Identifier |var|
//@[004:020) Identifier |testYamlArrayInt|
//@[021:022) Assignment |=|
//@[023:031) Identifier |testYaml|
//@[031:032) Dot |.|
//@[032:040) Identifier |arrayInt|
//@[040:042) NewLine |\r\n|
var testYamlArrayIntVal = testYaml.arrayInt[0]
//@[000:003) Identifier |var|
//@[004:023) Identifier |testYamlArrayIntVal|
//@[024:025) Assignment |=|
//@[026:034) Identifier |testYaml|
//@[034:035) Dot |.|
//@[035:043) Identifier |arrayInt|
//@[043:044) LeftSquare |[|
//@[044:045) Integer |0|
//@[045:046) RightSquare |]|
//@[046:048) NewLine |\r\n|
var testYamlArrayString = testYaml.arrayString
//@[000:003) Identifier |var|
//@[004:023) Identifier |testYamlArrayString|
//@[024:025) Assignment |=|
//@[026:034) Identifier |testYaml|
//@[034:035) Dot |.|
//@[035:046) Identifier |arrayString|
//@[046:048) NewLine |\r\n|
var testYamlArrayStringVal = testYaml.arrayString[0]
//@[000:003) Identifier |var|
//@[004:026) Identifier |testYamlArrayStringVal|
//@[027:028) Assignment |=|
//@[029:037) Identifier |testYaml|
//@[037:038) Dot |.|
//@[038:049) Identifier |arrayString|
//@[049:050) LeftSquare |[|
//@[050:051) Integer |0|
//@[051:052) RightSquare |]|
//@[052:054) NewLine |\r\n|
var testYamlArrayBool = testYaml.arrayBool
//@[000:003) Identifier |var|
//@[004:021) Identifier |testYamlArrayBool|
//@[022:023) Assignment |=|
//@[024:032) Identifier |testYaml|
//@[032:033) Dot |.|
//@[033:042) Identifier |arrayBool|
//@[042:044) NewLine |\r\n|
var testYamlArrayBoolVal = testYaml.arrayBool[0]
//@[000:003) Identifier |var|
//@[004:024) Identifier |testYamlArrayBoolVal|
//@[025:026) Assignment |=|
//@[027:035) Identifier |testYaml|
//@[035:036) Dot |.|
//@[036:045) Identifier |arrayBool|
//@[045:046) LeftSquare |[|
//@[046:047) Integer |0|
//@[047:048) RightSquare |]|
//@[048:050) NewLine |\r\n|
var testYamlObject = testYaml.object
//@[000:003) Identifier |var|
//@[004:018) Identifier |testYamlObject|
//@[019:020) Assignment |=|
//@[021:029) Identifier |testYaml|
//@[029:030) Dot |.|
//@[030:036) Identifier |object|
//@[036:038) NewLine |\r\n|
var testYamlObjectNestedString = testYaml.object.nestedString
//@[000:003) Identifier |var|
//@[004:030) Identifier |testYamlObjectNestedString|
//@[031:032) Assignment |=|
//@[033:041) Identifier |testYaml|
//@[041:042) Dot |.|
//@[042:048) Identifier |object|
//@[048:049) Dot |.|
//@[049:061) Identifier |nestedString|
//@[061:063) NewLine |\r\n|
var testYamlObjectNestedInt = testYaml.object.nestedInt
//@[000:003) Identifier |var|
//@[004:027) Identifier |testYamlObjectNestedInt|
//@[028:029) Assignment |=|
//@[030:038) Identifier |testYaml|
//@[038:039) Dot |.|
//@[039:045) Identifier |object|
//@[045:046) Dot |.|
//@[046:055) Identifier |nestedInt|
//@[055:057) NewLine |\r\n|
var testYamlObjectNestedBool = testYaml.object.nestedBool
//@[000:003) Identifier |var|
//@[004:028) Identifier |testYamlObjectNestedBool|
//@[029:030) Assignment |=|
//@[031:039) Identifier |testYaml|
//@[039:040) Dot |.|
//@[040:046) Identifier |object|
//@[046:047) Dot |.|
//@[047:057) Identifier |nestedBool|
//@[057:061) NewLine |\r\n\r\n|

output testYamlString string = testYamlString
//@[000:006) Identifier |output|
//@[007:021) Identifier |testYamlString|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:045) Identifier |testYamlString|
//@[045:047) NewLine |\r\n|
output testYamlInt int = testYamlInt
//@[000:006) Identifier |output|
//@[007:018) Identifier |testYamlInt|
//@[019:022) Identifier |int|
//@[023:024) Assignment |=|
//@[025:036) Identifier |testYamlInt|
//@[036:038) NewLine |\r\n|
output testYamlBool bool = testYamlBool
//@[000:006) Identifier |output|
//@[007:019) Identifier |testYamlBool|
//@[020:024) Identifier |bool|
//@[025:026) Assignment |=|
//@[027:039) Identifier |testYamlBool|
//@[039:041) NewLine |\r\n|
output testYamlArrayInt array = testYamlArrayInt
//@[000:006) Identifier |output|
//@[007:023) Identifier |testYamlArrayInt|
//@[024:029) Identifier |array|
//@[030:031) Assignment |=|
//@[032:048) Identifier |testYamlArrayInt|
//@[048:050) NewLine |\r\n|
output testYamlArrayIntVal int = testYamlArrayIntVal
//@[000:006) Identifier |output|
//@[007:026) Identifier |testYamlArrayIntVal|
//@[027:030) Identifier |int|
//@[031:032) Assignment |=|
//@[033:052) Identifier |testYamlArrayIntVal|
//@[052:054) NewLine |\r\n|
output testYamlArrayString array = testYamlArrayString
//@[000:006) Identifier |output|
//@[007:026) Identifier |testYamlArrayString|
//@[027:032) Identifier |array|
//@[033:034) Assignment |=|
//@[035:054) Identifier |testYamlArrayString|
//@[054:056) NewLine |\r\n|
output testYamlArrayStringVal string = testYamlArrayStringVal
//@[000:006) Identifier |output|
//@[007:029) Identifier |testYamlArrayStringVal|
//@[030:036) Identifier |string|
//@[037:038) Assignment |=|
//@[039:061) Identifier |testYamlArrayStringVal|
//@[061:063) NewLine |\r\n|
output testYamlArrayBool array = testYamlArrayBool
//@[000:006) Identifier |output|
//@[007:024) Identifier |testYamlArrayBool|
//@[025:030) Identifier |array|
//@[031:032) Assignment |=|
//@[033:050) Identifier |testYamlArrayBool|
//@[050:052) NewLine |\r\n|
output testYamlArrayBoolVal bool = testYamlArrayBoolVal
//@[000:006) Identifier |output|
//@[007:027) Identifier |testYamlArrayBoolVal|
//@[028:032) Identifier |bool|
//@[033:034) Assignment |=|
//@[035:055) Identifier |testYamlArrayBoolVal|
//@[055:057) NewLine |\r\n|
output testYamlObject object = testYamlObject
//@[000:006) Identifier |output|
//@[007:021) Identifier |testYamlObject|
//@[022:028) Identifier |object|
//@[029:030) Assignment |=|
//@[031:045) Identifier |testYamlObject|
//@[045:047) NewLine |\r\n|
output testYamlObjectNestedString string = testYamlObjectNestedString
//@[000:006) Identifier |output|
//@[007:033) Identifier |testYamlObjectNestedString|
//@[034:040) Identifier |string|
//@[041:042) Assignment |=|
//@[043:069) Identifier |testYamlObjectNestedString|
//@[069:071) NewLine |\r\n|
output testYamlObjectNestedInt int = testYamlObjectNestedInt
//@[000:006) Identifier |output|
//@[007:030) Identifier |testYamlObjectNestedInt|
//@[031:034) Identifier |int|
//@[035:036) Assignment |=|
//@[037:060) Identifier |testYamlObjectNestedInt|
//@[060:062) NewLine |\r\n|
output testYamlObjectNestedBool bool = testYamlObjectNestedBool
//@[000:006) Identifier |output|
//@[007:031) Identifier |testYamlObjectNestedBool|
//@[032:036) Identifier |bool|
//@[037:038) Assignment |=|
//@[039:063) Identifier |testYamlObjectNestedBool|
//@[063:065) NewLine |\r\n|

//@[000:000) EndOfFile ||
