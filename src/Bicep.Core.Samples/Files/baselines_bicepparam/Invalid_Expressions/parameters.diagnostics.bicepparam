using 'main.bicep'

param testAny = any('foo')
param testArray = array({})
//@[00:27) [BCP260 (Error)] The parameter "testArray" expects a value of type "object" but the provided value is of type "array". (CodeDescription: none) |param testArray = array({})|
param testBase64ToString = base64ToString(concat(base64('abc'), '@'))
//@[00:69) [BCP260 (Error)] The parameter "testBase64ToString" expects a value of type "object" but the provided value is of type "string". (CodeDescription: none) |param testBase64ToString = base64ToString(concat(base64('abc'), '@'))|
//@[27:69) [BCP338 (Error)] Failed to evaluate parameter "testBase64ToString": The template language function 'base64ToString' was invoked with a parameter that is not valid. The value cannot be decoded from base64 representation. (CodeDescription: none) |base64ToString(concat(base64('abc'), '@'))|
//@[42:68) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(base64('abc'), '@')|
param testBase64ToJson = base64ToJson(base64('{"hi": "there"')).hi
param testBool = bool('sdf')
//@[00:28) [BCP260 (Error)] The parameter "testBool" expects a value of type "object" but the provided value is of type "bool". (CodeDescription: none) |param testBool = bool('sdf')|
//@[17:28) [BCP338 (Error)] Failed to evaluate parameter "testBool": The template language function 'bool' was invoked with a parameter that is not valid. The value cannot be converted to the target type. (CodeDescription: none) |bool('sdf')|
param testConcat = concat(['abc'], {foo: 'bar'})
//@[35:47) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "array". (CodeDescription: none) |{foo: 'bar'}|
param testContains = contains('foo/bar', {})
//@[41:43) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (CodeDescription: none) |{}|
param testDataUriToString = dataUriToString(concat(dataUri('abc'), '@'))
//@[00:72) [BCP260 (Error)] The parameter "testDataUriToString" expects a value of type "object" but the provided value is of type "string". (CodeDescription: none) |param testDataUriToString = dataUriToString(concat(dataUri('abc'), '@'))|
//@[28:72) [BCP338 (Error)] Failed to evaluate parameter "testDataUriToString": The template language function 'dataUriToString' was invoked with a parameter that is not valid. The value cannot be converted to the target type. (CodeDescription: none) |dataUriToString(concat(dataUri('abc'), '@'))|
//@[44:71) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(dataUri('abc'), '@')|
param testDateTimeAdd = dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')  
//@[00:81) [BCP260 (Error)] The parameter "testDateTimeAdd" expects a value of type "object" but the provided value is of type "string". (CodeDescription: none) |param testDateTimeAdd = dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')|
//@[24:81) [BCP338 (Error)] Failed to evaluate parameter "testDateTimeAdd": The template function 'dateTimeAdd' has an invalid parameter. The second parameter 'PTASDIONS1D' is not a valid ISO8601 Duration string. Please see https://aka.ms/arm-syntax-functions . (CodeDescription: none) |dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')|
param testDateTimeToEpoch = dateTimeToEpoch(dateTimeFromEpoch('adfasdf'))
//@[62:71) [BCP070 (Error)] Argument of type "'adfasdf'" is not assignable to parameter of type "int". (CodeDescription: none) |'adfasdf'|
param testEmpty = empty([])
//@[00:27) [BCP260 (Error)] The parameter "testEmpty" expects a value of type "object" but the provided value is of type "true". (CodeDescription: none) |param testEmpty = empty([])|
param testEndsWith = endsWith('foo', [])
//@[37:39) [BCP070 (Error)] Argument of type "<empty array>" is not assignable to parameter of type "string". (CodeDescription: none) |[]|
param testFilter = filter([1, 2], i => i < 'foo')
//@[00:49) [BCP260 (Error)] The parameter "testFilter" expects a value of type "object" but the provided value is of type "(1 | 2)[]". (CodeDescription: none) |param testFilter = filter([1, 2], i => i < 'foo')|
//@[19:49) [BCP338 (Error)] Failed to evaluate parameter "testFilter": The template language function 'less' expects two parameters of matching types. The function was invoked with values of type 'Integer' and 'String' that do not match. (CodeDescription: none) |filter([1, 2], i => i < 'foo')|
//@[39:48) [BCP045 (Error)] Cannot apply operator "<" to operands of type "1 | 2" and "'foo'". (CodeDescription: none) |i < 'foo'|
param testFirst = first('asdfds')
//@[00:33) [BCP260 (Error)] The parameter "testFirst" expects a value of type "object" but the provided value is of type "'a'". (CodeDescription: none) |param testFirst = first('asdfds')|
param testFlatten = flatten({foo: 'bar'})
//@[28:40) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "array[]". (CodeDescription: none) |{foo: 'bar'}|
param testFormat = format('->{123}<-', 123)
//@[00:43) [BCP260 (Error)] The parameter "testFormat" expects a value of type "object" but the provided value is of type "string". (CodeDescription: none) |param testFormat = format('->{123}<-', 123)|
//@[19:43) [BCP338 (Error)] Failed to evaluate parameter "testFormat": Unable to evaluate language function 'format': the format is invalid: 'Index (zero based) must be greater than or equal to zero and less than the size of the argument list.'. Please see https://aka.ms/arm-function-format for usage details. (CodeDescription: none) |format('->{123}<-', 123)|
//@[26:42) [BCP234 (Warning)] The ARM function "format" failed when invoked on the value [->{123}<-, 123]: Unable to evaluate language function 'format': the format is invalid: 'Index (zero based) must be greater than or equal to zero and less than the size of the argument list.'. Please see https://aka.ms/arm-function-format for usage details. (CodeDescription: none) |'->{123}<-', 123|
param testGuid = guid({})
//@[22:24) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (CodeDescription: none) |{}|
param testIndexOf = indexOf('abc', {})
//@[35:37) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (CodeDescription: none) |{}|
param testInt = int('asdf')
//@[00:27) [BCP260 (Error)] The parameter "testInt" expects a value of type "object" but the provided value is of type "int". (CodeDescription: none) |param testInt = int('asdf')|
//@[16:27) [BCP338 (Error)] Failed to evaluate parameter "testInt": The template language function 'int' cannot convert provided value 'asdf' to integer value. Please see https://aka.ms/arm-function-int for usage details. (CodeDescription: none) |int('asdf')|
param testIntersection = intersection([1, 2, 3], 'foo')
//@[49:54) [BCP070 (Error)] Argument of type "'foo'" is not assignable to parameter of type "array". (CodeDescription: none) |'foo'|
param testItems = items('asdfas')
//@[24:32) [BCP070 (Error)] Argument of type "'asdfas'" is not assignable to parameter of type "object". (CodeDescription: none) |'asdfas'|
param testJoin = join(['abc', 'def', 'ghi'], {})
//@[45:47) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (CodeDescription: none) |{}|
param testLast = last('asdf')
//@[00:29) [BCP260 (Error)] The parameter "testLast" expects a value of type "object" but the provided value is of type "'f'". (CodeDescription: none) |param testLast = last('asdf')|
param testLastIndexOf = lastIndexOf('abcba', {})
//@[45:47) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (CodeDescription: none) |{}|
param testLength = length({})
//@[00:29) [BCP260 (Error)] The parameter "testLength" expects a value of type "object" but the provided value is of type "0". (CodeDescription: none) |param testLength = length({})|
param testLoadFileAsBase64 = loadFileAsBase64('test.txt')
//@[00:57) [BCP260 (Error)] The parameter "testLoadFileAsBase64" expects a value of type "object" but the provided value is of type "test.txt". (CodeDescription: none) |param testLoadFileAsBase64 = loadFileAsBase64('test.txt')|
param testLoadJsonContent = loadJsonContent('test.json').adsfsd
//@[57:63) [BCP053 (Error)] The type "object" does not contain property "adsfsd". Available properties include "hello from". (CodeDescription: none) |adsfsd|
param testLoadTextContent = loadTextContent('test.txt')
//@[00:55) [BCP260 (Error)] The parameter "testLoadTextContent" expects a value of type "object" but the provided value is of type "'Hello from text file'". (CodeDescription: none) |param testLoadTextContent = loadTextContent('test.txt')|
param testMap = map(range(0, 3), i => dataUriToString('Hi ${i}!'))
//@[00:66) [BCP260 (Error)] The parameter "testMap" expects a value of type "object" but the provided value is of type "string[]". (CodeDescription: none) |param testMap = map(range(0, 3), i => dataUriToString('Hi ${i}!'))|
//@[16:66) [BCP338 (Error)] Failed to evaluate parameter "testMap": The template language function 'dataUriToString' expects its parameter to be formatted as a valid data URI. The provided value 'Hi 0!' was not formatted correctly. Please see https://aka.ms/arm-functions#dataUriToString for usage details. (CodeDescription: none) |map(range(0, 3), i => dataUriToString('Hi ${i}!'))|
param testMax = max(1, 2, '3')
//@[26:29) [BCP070 (Error)] Argument of type "'3'" is not assignable to parameter of type "int". (CodeDescription: none) |'3'|
param testMin = min(1, 2, {})
//@[26:28) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "int". (CodeDescription: none) |{}|
param testPadLeft = padLeft(13, 'foo')
//@[32:37) [BCP070 (Error)] Argument of type "'foo'" is not assignable to parameter of type "int". (CodeDescription: none) |'foo'|
param testRange = range(0, 'foo')
//@[27:32) [BCP070 (Error)] Argument of type "'foo'" is not assignable to parameter of type "int". (CodeDescription: none) |'foo'|
param testReduce = reduce(['a', 'b', 'c'], '', (a, b) => '${toObject(a)}-${b}')
//@[19:79) [BCP338 (Error)] Failed to evaluate parameter "testReduce": Unable to evaluate template language function 'toObject': function requires between 2 and 3 argument(s) while 1 were provided. Please see https://aka.ms/arm-resource-functions/#toObject for usage details. (CodeDescription: none) |reduce(['a', 'b', 'c'], '', (a, b) => '${toObject(a)}-${b}')|
//@[68:71) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. (CodeDescription: none) |(a)|
param testReplace = replace('abc', 'b', {})
//@[40:42) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (CodeDescription: none) |{}|
param testSkip = skip([1, 2, 3], '1')
//@[33:36) [BCP070 (Error)] Argument of type "'1'" is not assignable to parameter of type "int". (CodeDescription: none) |'1'|
param testSort = sort(['c', 'd', 'a'], (a, b) => a + b)
//@[00:55) [BCP260 (Error)] The parameter "testSort" expects a value of type "object" but the provided value is of type "('a' | 'c' | 'd')[]". (CodeDescription: none) |param testSort = sort(['c', 'd', 'a'], (a, b) => a + b)|
//@[17:55) [BCP338 (Error)] Failed to evaluate parameter "testSort": Unhandled exception during evaluating template language function 'sort' is not handled. (CodeDescription: none) |sort(['c', 'd', 'a'], (a, b) => a + b)|
//@[49:54) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'a' | 'c' | 'd'" and "'a' | 'c' | 'd'". Use string interpolation instead. (CodeDescription: none) |a + b|
param testSplit = split('a/b/c', 1 + 2)
//@[33:38) [BCP070 (Error)] Argument of type "3" is not assignable to parameter of type "array | string". (CodeDescription: none) |1 + 2|
param testStartsWith = startsWith('abc', {})
//@[41:43) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (CodeDescription: none) |{}|
param testString = string({})
//@[00:29) [BCP260 (Error)] The parameter "testString" expects a value of type "object" but the provided value is of type "string". (CodeDescription: none) |param testString = string({})|
param testSubstring = substring('asdfasf', '3')
//@[43:46) [BCP070 (Error)] Argument of type "'3'" is not assignable to parameter of type "int". (CodeDescription: none) |'3'|
param testTake = take([1, 2, 3], '2')
//@[33:36) [BCP070 (Error)] Argument of type "'2'" is not assignable to parameter of type "int". (CodeDescription: none) |'2'|
param testToLower = toLower(123)
//@[28:31) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (CodeDescription: none) |123|
param testToObject = toObject(['a', 'b', 'c'], x => {x: x}, x => 'Hi ${x}!')
//@[47:58) [BCP070 (Error)] Argument of type "('a' | 'b' | 'c') => object" is not assignable to parameter of type "any => string". (CodeDescription: none) |x => {x: x}|
param testToUpper = toUpper([123])
//@[28:33) [BCP070 (Error)] Argument of type "[123]" is not assignable to parameter of type "string". (CodeDescription: none) |[123]|
param testTrim = trim(123)
//@[22:25) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (CodeDescription: none) |123|
param testUnion = union({ abc: 'def' }, [123])
//@[40:45) [BCP070 (Error)] Argument of type "[123]" is not assignable to parameter of type "object". (CodeDescription: none) |[123]|
param testUniqueString = uniqueString('asd', 'asdf', 'asdf')
//@[00:60) [BCP260 (Error)] The parameter "testUniqueString" expects a value of type "object" but the provided value is of type "'iizpqit7ih3cc'". (CodeDescription: none) |param testUniqueString = uniqueString('asd', 'asdf', 'asdf')|
param testUri = uri('github.com', 'Azure/bicep')
//@[00:48) [BCP260 (Error)] The parameter "testUri" expects a value of type "object" but the provided value is of type "string". (CodeDescription: none) |param testUri = uri('github.com', 'Azure/bicep')|
//@[16:48) [BCP338 (Error)] Failed to evaluate parameter "testUri": The template language function 'uri' expects its first argument to be a uri string. The provided value is 'github.com'. Please see https://aka.ms/arm-function-datauri for usage details. (CodeDescription: none) |uri('github.com', 'Azure/bicep')|
//@[20:47) [BCP234 (Warning)] The ARM function "uri" failed when invoked on the value [github.com, Azure/bicep]: The template language function 'uri' expects its first argument to be a uri string. The provided value is 'github.com'. Please see https://aka.ms/arm-function-datauri for usage details. (CodeDescription: none) |'github.com', 'Azure/bicep'|
param testUriComponent = uriComponent(123)
//@[38:41) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (CodeDescription: none) |123|
param testUriComponentToString = uriComponentToString({})
//@[54:56) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (CodeDescription: none) |{}|

param myObj = {
  newGuid: newGuid()
//@[11:18) [BCP065 (Error)] Function "newGuid" is not valid at this location. It can only be used as a parameter default value. (CodeDescription: none) |newGuid|
  utcNow: utcNow()
//@[10:16) [BCP065 (Error)] Function "utcNow" is not valid at this location. It can only be used as a parameter default value. (CodeDescription: none) |utcNow|
  resourceId: resourceId('Microsoft.ContainerService/managedClusters', 'blah')
//@[14:24) [BCP057 (Error)] The name "resourceId" does not exist in the current context. (CodeDescription: none) |resourceId|
  deployment: deployment()
//@[14:24) [BCP057 (Error)] The name "deployment" does not exist in the current context. (CodeDescription: none) |deployment|
  environment: environment()
//@[15:26) [BCP057 (Error)] The name "environment" does not exist in the current context. (CodeDescription: none) |environment|
  azNs: az
  azNsFunc: az.providers('Microsoft.Compute')
//@[15:24) [BCP107 (Error)] The function "providers" does not exist in namespace "az". (CodeDescription: none) |providers|
}
