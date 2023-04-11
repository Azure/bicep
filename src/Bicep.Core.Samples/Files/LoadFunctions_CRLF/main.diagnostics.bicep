var loadedText1 = loadTextContent('Assets/TextFile.CRLF.txt')
//@[4:15) [no-unused-vars (Warning)] Variable "loadedText1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedText1|
var loadedText2 = sys.loadTextContent('Assets/TextFile.LF.txt')
//@[4:15) [no-unused-vars (Warning)] Variable "loadedText2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedText2|
var loadedTextEncoding1 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[4:23) [no-unused-vars (Warning)] Variable "loadedTextEncoding1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextEncoding1|
var loadedTextEncoding2 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[4:23) [no-unused-vars (Warning)] Variable "loadedTextEncoding2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextEncoding2|
var loadedTextEncoding3 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16')
//@[4:23) [no-unused-vars (Warning)] Variable "loadedTextEncoding3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextEncoding3|
var loadedTextEncoding4 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16BE')
//@[4:23) [no-unused-vars (Warning)] Variable "loadedTextEncoding4" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextEncoding4|
var loadedTextEncoding5 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[4:23) [no-unused-vars (Warning)] Variable "loadedTextEncoding5" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextEncoding5|

var loadedBinary1 = loadFileAsBase64('Assets/binary')
//@[4:17) [no-unused-vars (Warning)] Variable "loadedBinary1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedBinary1|
var loadedBinary2 = sys.loadFileAsBase64('Assets/binary')
//@[4:17) [no-unused-vars (Warning)] Variable "loadedBinary2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedBinary2|

var loadedTextInterpolation1 = 'Text: ${loadTextContent('Assets/TextFile.CRLF.txt')}'
//@[4:28) [no-unused-vars (Warning)] Variable "loadedTextInterpolation1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextInterpolation1|
var loadedTextInterpolation2 = 'Text: ${loadTextContent('Assets/TextFile.LF.txt')}'
//@[4:28) [no-unused-vars (Warning)] Variable "loadedTextInterpolation2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextInterpolation2|

var loadedTextObject1 = {
//@[4:21) [no-unused-vars (Warning)] Variable "loadedTextObject1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextObject1|
  'text' : loadTextContent('Assets/TextFile.CRLF.txt')
//@[2:08) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'text'|
}
var loadedTextObject2 = {
//@[4:21) [no-unused-vars (Warning)] Variable "loadedTextObject2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextObject2|
  'text' : loadTextContent('Assets/TextFile.LF.txt')  
//@[2:08) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'text'|
}
var loadedBinaryInObject = {
//@[4:24) [no-unused-vars (Warning)] Variable "loadedBinaryInObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedBinaryInObject|
  file: loadFileAsBase64('Assets/binary')
}

var loadedTextArray = [
//@[4:19) [no-unused-vars (Warning)] Variable "loadedTextArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextArray|
  loadTextContent('Assets/TextFile.LF.txt')
  loadFileAsBase64('Assets/binary')
]

var loadedTextArrayInObject = {
//@[4:27) [no-unused-vars (Warning)] Variable "loadedTextArrayInObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextArrayInObject|
  'files' : [
//@[2:09) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'files'|
    loadTextContent('Assets/TextFile.CRLF.txt')
    loadFileAsBase64('Assets/binary')
  ]
}

var loadedTextArrayInObjectFunctions = {
//@[4:36) [no-unused-vars (Warning)] Variable "loadedTextArrayInObjectFunctions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadedTextArrayInObjectFunctions|
  'files' : [
//@[2:09) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'files'|
    length(loadTextContent('Assets/TextFile.CRLF.txt'))
    sys.length(loadTextContent('Assets/TextFile.LF.txt'))
    length(loadFileAsBase64('Assets/binary'))
    sys.length(loadFileAsBase64('Assets/binary'))
  ]
}


module module1 'modulea.bicep' = {
  name: 'module1'
  params: {
    text: loadTextContent('Assets/TextFile.LF.txt')
  }
}

module module2 'modulea.bicep' = {
  name: 'module2'
  params: {
    text: loadFileAsBase64('Assets/binary')
  }
}

var textFileInSubdirectories = loadTextContent('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt')
//@[4:28) [no-unused-vars (Warning)] Variable "textFileInSubdirectories" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |textFileInSubdirectories|
var binaryFileInSubdirectories = loadFileAsBase64('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary')
//@[4:30) [no-unused-vars (Warning)] Variable "binaryFileInSubdirectories" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |binaryFileInSubdirectories|

var loadWithEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[4:22) [no-unused-vars (Warning)] Variable "loadWithEncoding01" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadWithEncoding01|
var loadWithEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[4:22) [no-unused-vars (Warning)] Variable "loadWithEncoding06" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadWithEncoding06|
var loadWithEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[4:22) [no-unused-vars (Warning)] Variable "loadWithEncoding07" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadWithEncoding07|
var loadWithEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[4:22) [no-unused-vars (Warning)] Variable "loadWithEncoding08" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadWithEncoding08|
var loadWithEncoding11 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[4:22) [no-unused-vars (Warning)] Variable "loadWithEncoding11" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadWithEncoding11|
var loadWithEncoding12 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-8')
//@[4:22) [no-unused-vars (Warning)] Variable "loadWithEncoding12" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loadWithEncoding12|

var testJson = json(loadTextContent('./Assets/test.json.txt'))
var testJsonString = testJson.string
//@[4:18) [no-unused-vars (Warning)] Variable "testJsonString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonString|
var testJsonInt = testJson.int
//@[4:15) [no-unused-vars (Warning)] Variable "testJsonInt" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonInt|
var testJsonArrayVal = testJson.array[0]
//@[4:20) [no-unused-vars (Warning)] Variable "testJsonArrayVal" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonArrayVal|
var testJsonObject = testJson.object
//@[4:18) [no-unused-vars (Warning)] Variable "testJsonObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonObject|
var testJsonNestedString = testJson.object.nestedString
//@[4:24) [no-unused-vars (Warning)] Variable "testJsonNestedString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonNestedString|

var testJson2 = loadJsonContent('./Assets/test.json.txt')
//@[4:13) [no-unused-vars (Warning)] Variable "testJson2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJson2|
var testJsonString2 = testJson.string
//@[4:19) [no-unused-vars (Warning)] Variable "testJsonString2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonString2|
var testJsonString2_1 = loadJsonContent('./Assets/test.json.txt', '.string')
//@[4:21) [no-unused-vars (Warning)] Variable "testJsonString2_1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonString2_1|
var testJsonInt2 = testJson.int
//@[4:16) [no-unused-vars (Warning)] Variable "testJsonInt2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonInt2|
var testJsonInt2_1 = loadJsonContent('./Assets/test.json.txt', '.int')
//@[4:18) [no-unused-vars (Warning)] Variable "testJsonInt2_1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonInt2_1|
var testJsonArrayVal2 = testJson.array[0]
//@[4:21) [no-unused-vars (Warning)] Variable "testJsonArrayVal2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonArrayVal2|
var testJsonArrayVal2_1 = loadJsonContent('./Assets/test.json.txt', '.array[0]')
//@[4:23) [no-unused-vars (Warning)] Variable "testJsonArrayVal2_1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonArrayVal2_1|
var testJsonObject2 = testJson.object
//@[4:19) [no-unused-vars (Warning)] Variable "testJsonObject2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonObject2|
var testJsonObject2_1 = loadJsonContent('./Assets/test.json.txt', '.object')
var testJsonNestedString2 = testJson.object.nestedString
//@[4:25) [no-unused-vars (Warning)] Variable "testJsonNestedString2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonNestedString2|
var testJsonNestedString2_1 = testJsonObject2_1.nestedString
//@[4:27) [no-unused-vars (Warning)] Variable "testJsonNestedString2_1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonNestedString2_1|
var testJsonNestedString2_2 = loadJsonContent('./Assets/test.json.txt', '.object.nestedString')
//@[4:27) [no-unused-vars (Warning)] Variable "testJsonNestedString2_2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonNestedString2_2|

var testJsonTokensAsArray = loadJsonContent('./Assets/test2.json.txt', '.products[?(@.price > 3)].name')
//@[4:25) [no-unused-vars (Warning)] Variable "testJsonTokensAsArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testJsonTokensAsArray|

var testYaml = loadYamlContent('./Assets/test.yaml.txt')
var testYamlString = testYaml.string
//@[4:18) [no-unused-vars (Warning)] Variable "testYamlString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testYamlString|

