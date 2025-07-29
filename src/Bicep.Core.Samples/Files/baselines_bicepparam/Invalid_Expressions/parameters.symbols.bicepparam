using 'main.bicep'

param testAny = any('foo')
//@[06:13) ParameterAssignment testAny. Type: any. Declaration start char: 0, length: 26
param testArray = array({})
//@[06:15) ParameterAssignment testArray. Type: array. Declaration start char: 0, length: 27
param testBase64ToString = base64ToString(concat(base64('abc'), '@'))
//@[06:24) ParameterAssignment testBase64ToString. Type: string. Declaration start char: 0, length: 69
param testBase64ToJson = base64ToJson(base64('{"hi": "there"')).hi
//@[06:22) ParameterAssignment testBase64ToJson. Type: any. Declaration start char: 0, length: 66
param testBool = bool('sdf')
//@[06:14) ParameterAssignment testBool. Type: bool. Declaration start char: 0, length: 28
param testConcat = concat(['abc'], {foo: 'bar'})
//@[06:16) ParameterAssignment testConcat. Type: error. Declaration start char: 0, length: 48
param testContains = contains('foo/bar', {})
//@[06:18) ParameterAssignment testContains. Type: error. Declaration start char: 0, length: 44
param testDataUriToString = dataUriToString(concat(dataUri('abc'), '@'))
//@[06:25) ParameterAssignment testDataUriToString. Type: string. Declaration start char: 0, length: 72
param testDateTimeAdd = dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')  
//@[06:21) ParameterAssignment testDateTimeAdd. Type: string. Declaration start char: 0, length: 81
param testDateTimeToEpoch = dateTimeToEpoch(dateTimeFromEpoch('adfasdf'))
//@[06:25) ParameterAssignment testDateTimeToEpoch. Type: error. Declaration start char: 0, length: 73
param testEmpty = empty([])
//@[06:15) ParameterAssignment testEmpty. Type: true. Declaration start char: 0, length: 27
param testEndsWith = endsWith('foo', [])
//@[06:18) ParameterAssignment testEndsWith. Type: error. Declaration start char: 0, length: 40
param testFilter = filter([1, 2], i => i < 'foo')
//@[34:35) Local i. Type: 1 | 2. Declaration start char: 34, length: 1
//@[06:16) ParameterAssignment testFilter. Type: error. Declaration start char: 0, length: 49
param testFirst = first('asdfds')
//@[06:15) ParameterAssignment testFirst. Type: 'a'. Declaration start char: 0, length: 33
param testFlatten = flatten({foo: 'bar'})
//@[06:17) ParameterAssignment testFlatten. Type: error. Declaration start char: 0, length: 41
param testFormat = format('->{123}<-', 123)
//@[06:16) ParameterAssignment testFormat. Type: string. Declaration start char: 0, length: 43
param testGuid = guid({})
//@[06:14) ParameterAssignment testGuid. Type: error. Declaration start char: 0, length: 25
param testIndexOf = indexOf('abc', {})
//@[06:17) ParameterAssignment testIndexOf. Type: error. Declaration start char: 0, length: 38
param testInt = int('asdf')
//@[06:13) ParameterAssignment testInt. Type: int. Declaration start char: 0, length: 27
param testIntersection = intersection([1, 2, 3], 'foo')
//@[06:22) ParameterAssignment testIntersection. Type: error. Declaration start char: 0, length: 55
param testItems = items('asdfas')
//@[06:15) ParameterAssignment testItems. Type: error. Declaration start char: 0, length: 33
param testJoin = join(['abc', 'def', 'ghi'], {})
//@[06:14) ParameterAssignment testJoin. Type: error. Declaration start char: 0, length: 48
param testLast = last('asdf')
//@[06:14) ParameterAssignment testLast. Type: 'f'. Declaration start char: 0, length: 29
param testLastIndexOf = lastIndexOf('abcba', {})
//@[06:21) ParameterAssignment testLastIndexOf. Type: error. Declaration start char: 0, length: 48
param testLength = length({})
//@[06:16) ParameterAssignment testLength. Type: 0. Declaration start char: 0, length: 29
param testLoadFileAsBase64 = loadFileAsBase64('test.txt')
//@[06:26) ParameterAssignment testLoadFileAsBase64. Type: test.txt. Declaration start char: 0, length: 57
param testLoadJsonContent = loadJsonContent('test.json').adsfsd
//@[06:25) ParameterAssignment testLoadJsonContent. Type: error. Declaration start char: 0, length: 63
param testLoadTextContent = loadTextContent('test.txt')
//@[06:25) ParameterAssignment testLoadTextContent. Type: 'Hello from text file'. Declaration start char: 0, length: 55
param testMap = map(range(0, 3), i => dataUriToString('Hi ${i}!'))
//@[33:34) Local i. Type: int. Declaration start char: 33, length: 1
//@[06:13) ParameterAssignment testMap. Type: string[]. Declaration start char: 0, length: 66
param testMax = max(1, 2, '3')
//@[06:13) ParameterAssignment testMax. Type: error. Declaration start char: 0, length: 30
param testMin = min(1, 2, {})
//@[06:13) ParameterAssignment testMin. Type: error. Declaration start char: 0, length: 29
param testPadLeft = padLeft(13, 'foo')
//@[06:17) ParameterAssignment testPadLeft. Type: error. Declaration start char: 0, length: 38
param testRange = range(0, 'foo')
//@[06:15) ParameterAssignment testRange. Type: error. Declaration start char: 0, length: 33
param testReduce = reduce(['a', 'b', 'c'], '', (a, b) => '${toObject(a)}-${b}')
//@[48:49) Local a. Type: string. Declaration start char: 48, length: 1
//@[51:52) Local b. Type: 'a' | 'b' | 'c'. Declaration start char: 51, length: 1
//@[06:16) ParameterAssignment testReduce. Type: error. Declaration start char: 0, length: 79
param testReplace = replace('abc', 'b', {})
//@[06:17) ParameterAssignment testReplace. Type: error. Declaration start char: 0, length: 43
param testSkip = skip([1, 2, 3], '1')
//@[06:14) ParameterAssignment testSkip. Type: error. Declaration start char: 0, length: 37
param testSort = sort(['c', 'd', 'a'], (a, b) => a + b)
//@[40:41) Local a. Type: 'a' | 'c' | 'd'. Declaration start char: 40, length: 1
//@[43:44) Local b. Type: 'a' | 'c' | 'd'. Declaration start char: 43, length: 1
//@[06:14) ParameterAssignment testSort. Type: error. Declaration start char: 0, length: 55
param testSplit = split('a/b/c', 1 + 2)
//@[06:15) ParameterAssignment testSplit. Type: error. Declaration start char: 0, length: 39
param testStartsWith = startsWith('abc', {})
//@[06:20) ParameterAssignment testStartsWith. Type: error. Declaration start char: 0, length: 44
param testString = string({})
//@[06:16) ParameterAssignment testString. Type: string. Declaration start char: 0, length: 29
param testSubstring = substring('asdfasf', '3')
//@[06:19) ParameterAssignment testSubstring. Type: error. Declaration start char: 0, length: 47
param testTake = take([1, 2, 3], '2')
//@[06:14) ParameterAssignment testTake. Type: error. Declaration start char: 0, length: 37
param testToLower = toLower(123)
//@[06:17) ParameterAssignment testToLower. Type: error. Declaration start char: 0, length: 32
param testToObject = toObject(['a', 'b', 'c'], x => {x: x}, x => 'Hi ${x}!')
//@[47:48) Local x. Type: 'a' | 'b' | 'c'. Declaration start char: 47, length: 1
//@[60:61) Local x. Type: 'a' | 'b' | 'c'. Declaration start char: 60, length: 1
//@[06:18) ParameterAssignment testToObject. Type: error. Declaration start char: 0, length: 76
param testToUpper = toUpper([123])
//@[06:17) ParameterAssignment testToUpper. Type: error. Declaration start char: 0, length: 34
param testTrim = trim(123)
//@[06:14) ParameterAssignment testTrim. Type: error. Declaration start char: 0, length: 26
param testUnion = union({ abc: 'def' }, [123])
//@[06:15) ParameterAssignment testUnion. Type: error. Declaration start char: 0, length: 46
param testUniqueString = uniqueString('asd', 'asdf', 'asdf')
//@[06:22) ParameterAssignment testUniqueString. Type: 'iizpqit7ih3cc'. Declaration start char: 0, length: 60
param testUri = uri('github.com', 'Azure/bicep')
//@[06:13) ParameterAssignment testUri. Type: string. Declaration start char: 0, length: 48
param testUriComponent = uriComponent(123)
//@[06:22) ParameterAssignment testUriComponent. Type: error. Declaration start char: 0, length: 42
param testUriComponentToString = uriComponentToString({})
//@[06:30) ParameterAssignment testUriComponentToString. Type: error. Declaration start char: 0, length: 57

param myObj = {
//@[06:11) ParameterAssignment myObj. Type: error. Declaration start char: 0, length: 249
  newGuid: newGuid()
  utcNow: utcNow()
  resourceId: resourceId('Microsoft.ContainerService/managedClusters', 'blah')
  deployment: deployment()
  environment: environment()
  azNs: az
  azNsFunc: az.providers('Microsoft.Compute')
}
