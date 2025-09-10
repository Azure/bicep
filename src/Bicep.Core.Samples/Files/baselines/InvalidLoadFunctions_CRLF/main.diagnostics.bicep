var textLoadDirectory = loadTextContent('Assets/path/to/nothing')
//@[04:21) [no-unused-vars (Warning)] Variable "textLoadDirectory" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadDirectory|
//@[40:64) [BCP275 (Error)] Unable to open file at path "${TEST_OUTPUT_DIR}/Assets/path/to/nothing". Found a directory instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP275) |'Assets/path/to/nothing'|
var binaryLoadDirectory = loadFileAsBase64('Assets/path/to/nothing')
//@[04:23) [no-unused-vars (Warning)] Variable "binaryLoadDirectory" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |binaryLoadDirectory|
//@[43:67) [BCP275 (Error)] Unable to open file at path "${TEST_OUTPUT_DIR}/Assets/path/to/nothing". Found a directory instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP275) |'Assets/path/to/nothing'|

var textLoadFileMissing = loadTextContent('Assets/nothing.file')
//@[04:23) [no-unused-vars (Warning)] Variable "textLoadFileMissing" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadFileMissing|
//@[42:63) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/Assets/nothing.file'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'Assets/nothing.file'|
var binaryLoadFileMissing = loadFileAsBase64('Assets/nothing.file')
//@[04:25) [no-unused-vars (Warning)] Variable "binaryLoadFileMissing" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |binaryLoadFileMissing|
//@[45:66) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/Assets/nothing.file'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'Assets/nothing.file'|

var textLoadFilePathEmpty = loadTextContent('')
//@[04:25) [no-unused-vars (Warning)] Variable "textLoadFilePathEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadFilePathEmpty|
//@[44:46) [BCP050 (Error)] The specified path is empty. (bicep https://aka.ms/bicep/core-diagnostics#BCP050) |''|
var binaryLoadFilePathEmpty = loadFileAsBase64('')
//@[04:27) [no-unused-vars (Warning)] Variable "binaryLoadFilePathEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |binaryLoadFilePathEmpty|
//@[47:49) [BCP050 (Error)] The specified path is empty. (bicep https://aka.ms/bicep/core-diagnostics#BCP050) |''|

var textLoadInvalidCharactersPath1 = loadTextContent('Assets\\TextFile.txt')
//@[04:34) [no-unused-vars (Warning)] Variable "textLoadInvalidCharactersPath1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadInvalidCharactersPath1|
//@[53:75) [BCP098 (Error)] The specified file path contains a "\" character. Use "/" instead as the directory separator character. (bicep https://aka.ms/bicep/core-diagnostics#BCP098) |'Assets\\TextFile.txt'|
var binaryLoadInvalidCharactersPath1 = loadFileAsBase64('Assets\\binary')
//@[04:36) [no-unused-vars (Warning)] Variable "binaryLoadInvalidCharactersPath1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |binaryLoadInvalidCharactersPath1|
//@[56:72) [BCP098 (Error)] The specified file path contains a "\" character. Use "/" instead as the directory separator character. (bicep https://aka.ms/bicep/core-diagnostics#BCP098) |'Assets\\binary'|

var textLoadInvalidCharactersPath2 = loadTextContent('/Assets/TextFile.txt')
//@[04:34) [no-unused-vars (Warning)] Variable "textLoadInvalidCharactersPath2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadInvalidCharactersPath2|
//@[53:75) [BCP051 (Error)] The specified path seems to reference an absolute path. Files must be referenced using relative paths. (bicep https://aka.ms/bicep/core-diagnostics#BCP051) |'/Assets/TextFile.txt'|
var binaryLoadInvalidCharactersPath2 = loadFileAsBase64('/Assets/binary')
//@[04:36) [no-unused-vars (Warning)] Variable "binaryLoadInvalidCharactersPath2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |binaryLoadInvalidCharactersPath2|
//@[56:72) [BCP051 (Error)] The specified path seems to reference an absolute path. Files must be referenced using relative paths. (bicep https://aka.ms/bicep/core-diagnostics#BCP051) |'/Assets/binary'|

var textLoadInvalidCharactersPath3 = loadTextContent('file://Assets/TextFile.txt')
//@[04:34) [no-unused-vars (Warning)] Variable "textLoadInvalidCharactersPath3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadInvalidCharactersPath3|
//@[53:81) [BCP085 (Error)] The specified file path contains one ore more invalid path characters. The following are not permitted: """, "*", ":", "<", ">", "?", "\", "|". (bicep https://aka.ms/bicep/core-diagnostics#BCP085) |'file://Assets/TextFile.txt'|
var binaryLoadInvalidCharactersPath3 = loadFileAsBase64('file://Assets/binary')
//@[04:36) [no-unused-vars (Warning)] Variable "binaryLoadInvalidCharactersPath3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |binaryLoadInvalidCharactersPath3|
//@[56:78) [BCP085 (Error)] The specified file path contains one ore more invalid path characters. The following are not permitted: """, "*", ":", "<", ">", "?", "\", "|". (bicep https://aka.ms/bicep/core-diagnostics#BCP085) |'file://Assets/binary'|


var textLoadUnsupportedEncoding = loadTextContent('Assets/TextFile.txt', 'windows-1250')
//@[04:31) [no-unused-vars (Warning)] Variable "textLoadUnsupportedEncoding" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadUnsupportedEncoding|
//@[73:87) [BCP070 (Error)] Argument of type "'windows-1250'" is not assignable to parameter of type "'iso-8859-1' | 'us-ascii' | 'utf-16' | 'utf-16BE' | 'utf-8'". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'windows-1250'|

var textLoadWrongEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'us-ascii')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding01" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding01|
var textLoadWrongEncoding02 = loadTextContent('Assets/encoding-iso.txt', 'utf-8')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding02" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding02|
var textLoadWrongEncoding03 = loadTextContent('Assets/encoding-iso.txt', 'utf-16BE')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding03" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding03|
var textLoadWrongEncoding04 = loadTextContent('Assets/encoding-iso.txt', 'utf-16')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding04" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding04|
var textLoadWrongEncoding05 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding05" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding05|
var textLoadWrongEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding06" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding06|
var textLoadWrongEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'utf-16BE')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding07" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding07|
var textLoadWrongEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-16')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding08" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding08|
var textLoadWrongEncoding09 = loadTextContent('Assets/encoding-utf16.txt', 'iso-8859-1')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding09" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding09|
//@[75:87) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-16' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'iso-8859-1'|
var textLoadWrongEncoding10 = loadTextContent('Assets/encoding-utf16.txt', 'utf-8')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding10" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding10|
//@[75:82) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-16' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'utf-8'|
var textLoadWrongEncoding11 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16BE')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding11" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding11|
//@[75:85) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-16' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'utf-16BE'|
var textLoadWrongEncoding12 = loadTextContent('Assets/encoding-utf16.txt', 'us-ascii')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding12" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding12|
//@[75:85) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-16' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'us-ascii'|
var textLoadWrongEncoding13 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding13" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding13|
//@[77:85) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-16BE' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'utf-16'|
var textLoadWrongEncoding14 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-8')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding14" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding14|
//@[77:84) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-16BE' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'utf-8'|
var textLoadWrongEncoding15 = loadTextContent('Assets/encoding-utf16be.txt', 'us-ascii')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding15" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding15|
//@[77:87) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-16BE' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'us-ascii'|
var textLoadWrongEncoding16 = loadTextContent('Assets/encoding-utf16be.txt', 'iso-8859-1')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding16" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding16|
//@[77:89) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-16BE' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'iso-8859-1'|
var textLoadWrongEncoding17 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-16BE')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding17" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding17|
var textLoadWrongEncoding18 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-16')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding18" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding18|
var textLoadWrongEncoding19 = loadTextContent('Assets/encoding-windows1250.txt', 'utf-8')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding19" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding19|
var textLoadWrongEncoding20 = loadTextContent('Assets/encoding-windows1250.txt', 'us-ascii')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding20" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding20|
var textLoadWrongEncoding21 = loadTextContent('Assets/encoding-windows1250.txt', 'iso-8859-1')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding21" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding21|
var textLoadWrongEncoding22 = loadTextContent('Assets/encoding-utf8.txt', 'iso-8859-1')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding22" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding22|
var textLoadWrongEncoding23 = loadTextContent('Assets/encoding-utf8.txt', 'utf-16')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding23" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding23|
var textLoadWrongEncoding24 = loadTextContent('Assets/encoding-utf8.txt', 'utf-16BE')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding24" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding24|
var textLoadWrongEncoding25 = loadTextContent('Assets/encoding-utf8.txt', 'us-ascii')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding25" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding25|
var textLoadWrongEncoding26 = loadTextContent('Assets/encoding-utf8-bom.txt', 'iso-8859-1')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding26" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding26|
//@[78:90) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-8' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'iso-8859-1'|
var textLoadWrongEncoding27 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-16')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding27" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding27|
//@[78:86) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-8' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'utf-16'|
var textLoadWrongEncoding28 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-16BE')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding28" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding28|
//@[78:88) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-8' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'utf-16BE'|
var textLoadWrongEncoding29 = loadTextContent('Assets/encoding-utf8-bom.txt', 'us-ascii')
//@[04:27) [no-unused-vars (Warning)] Variable "textLoadWrongEncoding29" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textLoadWrongEncoding29|
//@[78:88) [BCP185 (Info)] Encoding mismatch. File was loaded with 'utf-8' encoding. (bicep https://aka.ms/bicep/core-diagnostics#BCP185) |'us-ascii'|

var textOversize = loadTextContent('Assets/oversizeText.txt')
//@[04:16) [no-unused-vars (Warning)] Variable "textOversize" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |textOversize|
//@[35:60) [BCP184 (Error)] File '${TEST_OUTPUT_DIR}/Assets/oversizeText.txt' exceeded maximum size of 131072 characters. (bicep https://aka.ms/bicep/core-diagnostics#BCP184) |'Assets/oversizeText.txt'|
var binaryOversize = loadFileAsBase64('Assets/oversizeBinary')
//@[04:18) [no-unused-vars (Warning)] Variable "binaryOversize" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |binaryOversize|
//@[38:61) [BCP184 (Error)] File '${TEST_OUTPUT_DIR}/Assets/oversizeBinary' exceeded maximum size of 98304 bytes. (bicep https://aka.ms/bicep/core-diagnostics#BCP184) |'Assets/oversizeBinary'|

var binaryAsText = loadTextContent('Assets/binary')
//@[04:16) [no-unused-vars (Warning)] Variable "binaryAsText" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |binaryAsText|

var jsonObject1 = loadJsonContent('Assets/jsonInvalid.json.txt')
//@[04:15) [no-unused-vars (Warning)] Variable "jsonObject1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |jsonObject1|
var jsonObject2 = loadJsonContent('Assets/jsonValid.json.txt', '.')
//@[04:15) [no-unused-vars (Warning)] Variable "jsonObject2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |jsonObject2|
//@[63:66) [BCP235 (Error)] Specified JSONPath does not exist in the given file or is invalid. (bicep https://aka.ms/bicep/core-diagnostics#BCP235) |'.'|
var jsonObject3 = loadJsonContent('Assets/jsonValid.json.txt', '$.')
//@[04:15) [no-unused-vars (Warning)] Variable "jsonObject3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |jsonObject3|
//@[63:67) [BCP235 (Error)] Specified JSONPath does not exist in the given file or is invalid. (bicep https://aka.ms/bicep/core-diagnostics#BCP235) |'$.'|
var jsonObject4 = loadJsonContent('Assets/jsonValid.json.txt', '.propertyThatDoesNotExist')
//@[04:15) [no-unused-vars (Warning)] Variable "jsonObject4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |jsonObject4|
//@[63:90) [BCP235 (Error)] Specified JSONPath does not exist in the given file or is invalid. (bicep https://aka.ms/bicep/core-diagnostics#BCP235) |'.propertyThatDoesNotExist'|
var jsonObject5 = loadJsonContent('Assets/fileNotExists')
//@[04:15) [no-unused-vars (Warning)] Variable "jsonObject5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |jsonObject5|
//@[34:56) [BCP091 (Error)] An error occurred reading file. Could not find file '${TEST_OUTPUT_DIR}/Assets/fileNotExists'. (bicep https://aka.ms/bicep/core-diagnostics#BCP091) |'Assets/fileNotExists'|

