using 'main.bicep'

param testAny = any('foo')
param testArray = array({})
//@[18:27) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |array({})|
param testBase64ToString = base64ToString(concat(base64('abc'), '@'))
//@[27:69) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |base64ToString(concat(base64('abc'), '@'))|
//@[27:69) [BCP338 (Error)] Failed to evaluate parameter "testBase64ToString": The template language function 'base64ToString' was invoked with a parameter that is not valid. The value cannot be decoded from base64 representation. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |base64ToString(concat(base64('abc'), '@'))|
//@[42:68) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(base64('abc'), '@')|
param testBase64ToJson = base64ToJson(base64('{"hi": "there"')).hi
param testBool = bool('sdf')
//@[17:28) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |bool('sdf')|
//@[17:28) [BCP338 (Error)] Failed to evaluate parameter "testBool": The template language function 'bool' was invoked with a parameter that is not valid. The value cannot be converted to the target type. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |bool('sdf')|
param testConcat = concat(['abc'], {foo: 'bar'})
//@[35:47) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{foo: 'bar'}|
param testContains = contains('foo/bar', {})
//@[41:43) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|
param testDataUriToString = dataUriToString(concat(dataUri('abc'), '@'))
//@[28:72) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |dataUriToString(concat(dataUri('abc'), '@'))|
//@[28:72) [BCP338 (Error)] Failed to evaluate parameter "testDataUriToString": The template language function 'dataUriToString' was invoked with a parameter that is not valid. The value cannot be converted to the target type. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |dataUriToString(concat(dataUri('abc'), '@'))|
//@[44:71) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(dataUri('abc'), '@')|
param testDateTimeAdd = dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')  
//@[24:81) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')|
//@[24:81) [BCP338 (Error)] Failed to evaluate parameter "testDateTimeAdd": The template function 'dateTimeAdd' has an invalid parameter. The second parameter 'PTASDIONS1D' is not a valid ISO8601 Duration string. Please see https://aka.ms/arm-syntax-functions . (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')|
param testDateTimeToEpoch = dateTimeToEpoch(dateTimeFromEpoch('adfasdf'))
//@[62:71) [BCP070 (Error)] Argument of type "'adfasdf'" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'adfasdf'|
param testEmpty = empty([])
//@[18:27) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |empty([])|
param testEndsWith = endsWith('foo', [])
//@[37:39) [BCP070 (Error)] Argument of type "<empty array>" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |[]|
param testFilter = filter([1, 2], i => i < 'foo')
//@[34:48) [BCP070 (Error)] Argument of type "(1 | 2) => error" is not assignable to parameter of type "(any[, int]) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => i < 'foo'|
param testFirst = first('asdfds')
//@[18:33) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "'a'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |first('asdfds')|
param testFlatten = flatten({foo: 'bar'})
//@[28:40) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "array[]". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{foo: 'bar'}|
param testFormat = format('->{123}<-', 123)
//@[19:43) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |format('->{123}<-', 123)|
//@[19:43) [BCP338 (Error)] Failed to evaluate parameter "testFormat": Unable to evaluate language function 'format': the format is invalid: 'Index (zero based) must be greater than or equal to zero and less than the size of the argument list.'. Please see https://aka.ms/arm-function-format for usage details. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |format('->{123}<-', 123)|
//@[26:42) [BCP234 (Warning)] The ARM function "format" failed when invoked on the value [->{123}<-, 123]: Unable to evaluate language function 'format': the format is invalid: 'Index (zero based) must be greater than or equal to zero and less than the size of the argument list.'. Please see https://aka.ms/arm-function-format for usage details. (bicep https://aka.ms/bicep/core-diagnostics#BCP234) |'->{123}<-', 123|
param testGuid = guid({})
//@[22:24) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|
param testIndexOf = indexOf('abc', {})
//@[35:37) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|
param testInt = int('asdf')
//@[16:27) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |int('asdf')|
//@[16:27) [BCP338 (Error)] Failed to evaluate parameter "testInt": The template language function 'int' cannot convert provided value 'asdf' to integer value. Please see https://aka.ms/arm-function-int for usage details. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |int('asdf')|
param testIntersection = intersection([1, 2, 3], 'foo')
//@[49:54) [BCP070 (Error)] Argument of type "'foo'" is not assignable to parameter of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'foo'|
param testItems = items('asdfas')
//@[24:32) [BCP070 (Error)] Argument of type "'asdfas'" is not assignable to parameter of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'asdfas'|
param testJoin = join(['abc', 'def', 'ghi'], {})
//@[45:47) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|
param testLast = last('asdf')
//@[17:29) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "'f'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |last('asdf')|
param testLastIndexOf = lastIndexOf('abcba', {})
//@[45:47) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|
param testLength = length({})
//@[19:29) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "0". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |length({})|
param testLoadFileAsBase64 = loadFileAsBase64('test.txt')
//@[29:57) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "test.txt". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |loadFileAsBase64('test.txt')|
param testLoadJsonContent = loadJsonContent('test.json').adsfsd
//@[57:63) [BCP053 (Error)] The type "object" does not contain property "adsfsd". Available properties include "hello from". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |adsfsd|
param testLoadTextContent = loadTextContent('test.txt')
//@[28:55) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "'Hello from text file'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |loadTextContent('test.txt')|
param testMap = map(range(0, 3), i => dataUriToString('Hi ${i}!'))
//@[16:66) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "string[]". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |map(range(0, 3), i => dataUriToString('Hi ${i}!'))|
//@[16:66) [BCP338 (Error)] Failed to evaluate parameter "testMap": The template language function 'dataUriToString' expects its parameter to be formatted as a valid data URI. The provided value 'Hi 0!' was not formatted correctly. Please see https://aka.ms/arm-functions#dataUriToString for usage details. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |map(range(0, 3), i => dataUriToString('Hi ${i}!'))|
param testMax = max(1, 2, '3')
//@[26:29) [BCP070 (Error)] Argument of type "'3'" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'3'|
param testMin = min(1, 2, {})
//@[26:28) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|
param testPadLeft = padLeft(13, 'foo')
//@[32:37) [BCP070 (Error)] Argument of type "'foo'" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'foo'|
param testRange = range(0, 'foo')
//@[27:32) [BCP070 (Error)] Argument of type "'foo'" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'foo'|
param testReduce = reduce(['a', 'b', 'c'], '', (a, b) => '${toObject(a)}-${b}')
//@[47:78) [BCP070 (Error)] Argument of type "(string, ('a' | 'b' | 'c')) => error" is not assignable to parameter of type "(any, any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |(a, b) => '${toObject(a)}-${b}'|
param testReplace = replace('abc', 'b', {})
//@[40:42) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|
param testSkip = skip([1, 2, 3], '1')
//@[33:36) [BCP070 (Error)] Argument of type "'1'" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'1'|
param testSort = sort(['c', 'd', 'a'], (a, b) => a + b)
//@[39:54) [BCP070 (Error)] Argument of type "(('a' | 'c' | 'd'), ('a' | 'c' | 'd')) => error" is not assignable to parameter of type "(any, any) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |(a, b) => a + b|
param testSplit = split('a/b/c', 1 + 2)
//@[33:38) [BCP070 (Error)] Argument of type "3" is not assignable to parameter of type "array | string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |1 + 2|
param testStartsWith = startsWith('abc', {})
//@[41:43) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|
param testString = string({})
//@[19:29) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |string({})|
param testSubstring = substring('asdfasf', '3')
//@[43:46) [BCP070 (Error)] Argument of type "'3'" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'3'|
param testTake = take([1, 2, 3], '2')
//@[33:36) [BCP070 (Error)] Argument of type "'2'" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'2'|
param testToLower = toLower(123)
//@[28:31) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |123|
param testToObject = toObject(['a', 'b', 'c'], x => {x: x}, x => 'Hi ${x}!')
//@[47:58) [BCP070 (Error)] Argument of type "('a' | 'b' | 'c') => object" is not assignable to parameter of type "any => string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |x => {x: x}|
param testToUpper = toUpper([123])
//@[28:33) [BCP070 (Error)] Argument of type "[123]" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |[123]|
param testTrim = trim(123)
//@[22:25) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |123|
param testUnion = union({ abc: 'def' }, [123])
//@[40:45) [BCP070 (Error)] Argument of type "[123]" is not assignable to parameter of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |[123]|
param testUniqueString = uniqueString('asd', 'asdf', 'asdf')
//@[25:60) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "'iizpqit7ih3cc'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |uniqueString('asd', 'asdf', 'asdf')|
param testUri = uri('github.com', 'Azure/bicep')
//@[16:48) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |uri('github.com', 'Azure/bicep')|
//@[16:48) [BCP338 (Error)] Failed to evaluate parameter "testUri": The template language function 'uri' expects its first argument to be a uri string. The provided value is 'github.com'. Please see https://aka.ms/arm-function-datauri for usage details. (bicep https://aka.ms/bicep/core-diagnostics#BCP338) |uri('github.com', 'Azure/bicep')|
//@[20:47) [BCP234 (Warning)] The ARM function "uri" failed when invoked on the value [github.com, Azure/bicep]: The template language function 'uri' expects its first argument to be a uri string. The provided value is 'github.com'. Please see https://aka.ms/arm-function-datauri for usage details. (bicep https://aka.ms/bicep/core-diagnostics#BCP234) |'github.com', 'Azure/bicep'|
param testUriComponent = uriComponent(123)
//@[38:41) [BCP070 (Error)] Argument of type "123" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |123|
param testUriComponentToString = uriComponentToString({})
//@[54:56) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{}|

param myObj = {
  newGuid: newGuid()
//@[11:18) [BCP065 (Error)] Function "newGuid" is not valid at this location. It can only be used as a parameter default value. (bicep https://aka.ms/bicep/core-diagnostics#BCP065) |newGuid|
  utcNow: utcNow()
//@[10:16) [BCP065 (Error)] Function "utcNow" is not valid at this location. It can only be used as a parameter default value. (bicep https://aka.ms/bicep/core-diagnostics#BCP065) |utcNow|
  resourceId: resourceId('Microsoft.ContainerService/managedClusters', 'blah')
//@[14:24) [BCP057 (Error)] The name "resourceId" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |resourceId|
  deployment: deployment()
//@[14:24) [BCP057 (Error)] The name "deployment" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |deployment|
  environment: environment()
//@[15:26) [BCP057 (Error)] The name "environment" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |environment|
  azNs: az
  azNsFunc: az.providers('Microsoft.Compute')
//@[15:24) [BCP107 (Error)] The function "providers" does not exist in namespace "az". (bicep https://aka.ms/bicep/core-diagnostics#BCP107) |providers|
}
