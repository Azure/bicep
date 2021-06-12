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

var loadTextCompatibleEncodings = [
  loadTextContent('Assets/encoding-iso.txt', 'us-ascii')
  loadTextContent('Assets/encoding-iso.txt', 'utf-8')
  loadTextContent('Assets/encoding-iso.txt', 'utf-16BE')
  loadTextContent('Assets/encoding-iso.txt', 'utf-16')
  loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
  loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
  loadTextContent('Assets/encoding-ascii.txt', 'utf-16BE')
  loadTextContent('Assets/encoding-ascii.txt', 'utf-16')
]
