var loadedText1 = loadTextContent('Assets/TextFile.CRLF.txt')
var loadedText2 = sys.loadTextContent('Assets/TextFile.LF.txt')
var loadedTextEncoding1 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
var loadedTextEncoding2 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
var loadedTextEncoding3 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16')
var loadedTextEncoding4 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16BE')
var loadedTextEncoding5 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')

var loadedBinary1 = loadFileAsBase64('Assets/binary')
var loadedBinary2 = sys.loadFileAsBase64('Assets/binary')

var loadedTextInterpolation1 = 'Text: ${loadTextContent('Assets/TextFile.CRLF.txt')}'
var loadedTextInterpolation2 = 'Text: ${loadTextContent('Assets/TextFile.LF.txt')}'

var loadedTextObject1 = {
  'text': loadTextContent('Assets/TextFile.CRLF.txt')
}
var loadedTextObject2 = {
  'text': loadTextContent('Assets/TextFile.LF.txt')
}
var loadedBinaryInObject = {
  file: loadFileAsBase64('Assets/binary')
}

var loadedTextArray = [
  loadTextContent('Assets/TextFile.LF.txt')
  loadFileAsBase64('Assets/binary')
]

var loadedTextArrayInObject = {
  'files': [
    loadTextContent('Assets/TextFile.CRLF.txt')
    loadFileAsBase64('Assets/binary')
  ]
}

var loadedTextArrayInObjectFunctions = {
  'files': [
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
var binaryFileInSubdirectories = loadFileAsBase64('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary')

var loadWithEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
var loadWithEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
var loadWithEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
var loadWithEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
var loadWithEncoding11 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
var loadWithEncoding12 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-8')

var testJson = json(loadTextContent('./Assets/test.json.txt'))
var testJsonString = testJson.string
var testJsonInt = testJson.int
var testJsonArrayVal = testJson.array[0]
var testJsonObject = testJson.object
var testJsonNestedString = testJson.object.nestedString

var testJson2 = loadJsonContent('./Assets/test.json.txt')
var testJsonString2 = testJson.string
var testJsonString2_1 = loadJsonContent('./Assets/test.json.txt', '.string')
var testJsonInt2 = testJson.int
var testJsonInt2_1 = loadJsonContent('./Assets/test.json.txt', '.int')
var testJsonArrayVal2 = testJson.array[0]
var testJsonArrayVal2_1 = loadJsonContent('./Assets/test.json.txt', '.array[0]')
var testJsonObject2 = testJson.object
var testJsonObject2_1 = loadJsonContent('./Assets/test.json.txt', '.object')
var testJsonNestedString2 = testJson.object.nestedString
var testJsonNestedString2_1 = testJsonObject2_1.nestedString
var testJsonNestedString2_2 = loadJsonContent('./Assets/test.json.txt', '.object.nestedString')

var testJsonTokensAsArray = loadJsonContent('./Assets/test2.json.txt', '.products[?(@.price > 3)].name')

var directoryInfo = loadDirectoryFileInfo('./Assets')
var directoryInfoWildcard = loadDirectoryFileInfo('./Assets', '*.txt')

var testYaml = loadYamlContent('./Assets/test.yaml.txt')
var testYamlString = testYaml.string
var testYamlInt = testYaml.int
var testYamlBool = testYaml.bool
var testYamlArrayInt = testYaml.arrayInt
var testYamlArrayIntVal = testYaml.arrayInt[0]
var testYamlArrayString = testYaml.arrayString
var testYamlArrayStringVal = testYaml.arrayString[0]
var testYamlArrayBool = testYaml.arrayBool
var testYamlArrayBoolVal = testYaml.arrayBool[0]
var testYamlObject = testYaml.object
var testYamlObjectNestedString = testYaml.object.nestedString
var testYamlObjectNestedInt = testYaml.object.nestedInt
var testYamlObjectNestedBool = testYaml.object.nestedBool

output testYamlString string = testYamlString
output testYamlInt int = testYamlInt
output testYamlBool bool = testYamlBool
output testYamlArrayInt array = testYamlArrayInt
output testYamlArrayIntVal int = testYamlArrayIntVal
output testYamlArrayString array = testYamlArrayString
output testYamlArrayStringVal string = testYamlArrayStringVal
output testYamlArrayBool array = testYamlArrayBool
output testYamlArrayBoolVal bool = testYamlArrayBoolVal
output testYamlObject object = testYamlObject
output testYamlObjectNestedString string = testYamlObjectNestedString
output testYamlObjectNestedInt int = testYamlObjectNestedInt
output testYamlObjectNestedBool bool = testYamlObjectNestedBool
