using 'main.bicep'

param testAny = any('foo')
//@[6:13) ParameterAssignment testAny. Type: any. Declaration start char: 0, length: 26
param testArray = array({})
//@[6:15) ParameterAssignment testArray. Type: array. Declaration start char: 0, length: 27
param testBase64ToString = base64ToString(concat(base64('abc'), '@'))
//@[6:24) ParameterAssignment testBase64ToString. Type: string. Declaration start char: 0, length: 69
param testBase64ToJson = base64ToJson(base64('{"hi": "there"')).hi
//@[6:22) ParameterAssignment testBase64ToJson. Type: any. Declaration start char: 0, length: 66
param testBool = bool('sdf')
//@[6:14) ParameterAssignment testBool. Type: bool. Declaration start char: 0, length: 28
param testConcat = concat(['abc'], {foo: 'bar'})
//@[6:16) ParameterAssignment testConcat. Type: error. Declaration start char: 0, length: 48
param testContains = contains('foo/bar', {})
//@[6:18) ParameterAssignment testContains. Type: error. Declaration start char: 0, length: 44
param testDataUriToString = dataUriToString(concat(dataUri('abc'), '@'))
//@[6:25) ParameterAssignment testDataUriToString. Type: string. Declaration start char: 0, length: 72
param testDateTimeAdd = dateTimeAdd(dateTimeFromEpoch(1680224438), 'PTASDIONS1D')  
//@[6:21) ParameterAssignment testDateTimeAdd. Type: string. Declaration start char: 0, length: 81
param testDateTimeToEpoch = dateTimeToEpoch(dateTimeFromEpoch('adfasdf'))
//@[6:25) ParameterAssignment testDateTimeToEpoch. Type: error. Declaration start char: 0, length: 73
param testEmpty = empty([])
//@[6:15) ParameterAssignment testEmpty. Type: true. Declaration start char: 0, length: 27
param testEndsWith = endsWith('foo', [])
//@[6:18) ParameterAssignment testEndsWith. Type: error. Declaration start char: 0, length: 40
param testFilter = filter([1, 2], i => i < 'foo')
//@[6:16) ParameterAssignment testFilter. Type: (1 | 2)[]. Declaration start char: 0, length: 49
param testFirst = first('asdfds')
//@[6:15) ParameterAssignment testFirst. Type: 'a'. Declaration start char: 0, length: 33
param testFlatten = flatten({foo: 'bar'})
//@[6:17) ParameterAssignment testFlatten. Type: error. Declaration start char: 0, length: 41
param testFormat = format('->{123}<-', 123)
//@[6:16) ParameterAssignment testFormat. Type: string. Declaration start char: 0, length: 43
param testGuid = guid({})
//@[6:14) ParameterAssignment testGuid. Type: error. Declaration start char: 0, length: 25
param testIndexOf = indexOf('abc', {})
//@[6:17) ParameterAssignment testIndexOf. Type: error. Declaration start char: 0, length: 38
param testInt = int('asdf')
//@[6:13) ParameterAssignment testInt. Type: int. Declaration start char: 0, length: 27
param testIntersection = intersection([1, 2, 3], 'foo')
//@[6:22) ParameterAssignment testIntersection. Type: error. Declaration start char: 0, length: 55
param testItems = items('asdfas')
//@[6:15) ParameterAssignment testItems. Type: error. Declaration start char: 0, length: 33
param testJoin = join(['abc', 'def', 'ghi'], {})
//@[6:14) ParameterAssignment testJoin. Type: error. Declaration start char: 0, length: 48
param testLast = last('asdf')
//@[6:14) ParameterAssignment testLast. Type: 'f'. Declaration start char: 0, length: 29
param testLastIndexOf = lastIndexOf('abcba', {})
//@[6:21) ParameterAssignment testLastIndexOf. Type: error. Declaration start char: 0, length: 48
param testLength = length({})
//@[6:16) ParameterAssignment testLength. Type: 0. Declaration start char: 0, length: 29
param testLoadFileAsBase64 = loadFileAsBase64('test.txt')
//@[6:26) ParameterAssignment testLoadFileAsBase64. Type: test.txt. Declaration start char: 0, length: 57
param testLoadJsonContent = loadJsonContent('test.json').adsfsd
//@[6:25) ParameterAssignment testLoadJsonContent. Type: error. Declaration start char: 0, length: 63
param testLoadTextContent = loadTextContent('test.txt')
//@[6:25) ParameterAssignment testLoadTextContent. Type: 'Hello from text file'. Declaration start char: 0, length: 55
param testMap = map(range(0, 3), i => dataUriToString('Hi ${i}!'))
//@[6:13) ParameterAssignment testMap. Type: string[]. Declaration start char: 0, length: 66
param testMax = max(1, 2, '3')
//@[6:13) ParameterAssignment testMax. Type: error. Declaration start char: 0, length: 30
param testMin = min(1, 2, {})
//@[6:13) ParameterAssignment testMin. Type: error. Declaration start char: 0, length: 29
param testPadLeft = padLeft(13, 'foo')
//@[6:17) ParameterAssignment testPadLeft. Type: error. Declaration start char: 0, length: 38
param testRange = range(0, 'foo')
//@[6:15) ParameterAssignment testRange. Type: error. Declaration start char: 0, length: 33
param testReduce = reduce(['a', 'b', 'c'], '', (a, b) => '${toObject(a)}-${b}')
//@[6:16) ParameterAssignment testReduce. Type: any. Declaration start char: 0, length: 79
param testReplace = replace('abc', 'b', {})
//@[6:17) ParameterAssignment testReplace. Type: error. Declaration start char: 0, length: 43
param testSkip = skip([1, 2, 3], '1')
//@[6:14) ParameterAssignment testSkip. Type: error. Declaration start char: 0, length: 37
param testSort = sort(['c', 'd', 'a'], (a, b) => a + b)
//@[6:14) ParameterAssignment testSort. Type: ('a' | 'c' | 'd')[]. Declaration start char: 0, length: 55
param testSplit = split('a/b/c', 1 + 2)
//@[6:15) ParameterAssignment testSplit. Type: error. Declaration start char: 0, length: 39
param testStartsWith = startsWith('abc', {})
//@[6:20) ParameterAssignment testStartsWith. Type: error. Declaration start char: 0, length: 44
param testString = string({})
//@[6:16) ParameterAssignment testString. Type: string. Declaration start char: 0, length: 29
param testSubstring = substring('asdfasf', '3')
//@[6:19) ParameterAssignment testSubstring. Type: error. Declaration start char: 0, length: 47
param testTake = take([1, 2, 3], '2')
//@[6:14) ParameterAssignment testTake. Type: error. Declaration start char: 0, length: 37
param testToLower = toLower(123)
//@[6:17) ParameterAssignment testToLower. Type: error. Declaration start char: 0, length: 32
param testToObject = toObject(['a', 'b', 'c'], x => {x: x}, x => 'Hi ${x}!')
//@[6:18) ParameterAssignment testToObject. Type: error. Declaration start char: 0, length: 76
param testToUpper = toUpper([123])
//@[6:17) ParameterAssignment testToUpper. Type: error. Declaration start char: 0, length: 34
param testTrim = trim(123)
//@[6:14) ParameterAssignment testTrim. Type: error. Declaration start char: 0, length: 26
param testUnion = union({ abc: 'def' }, [123])
//@[6:15) ParameterAssignment testUnion. Type: error. Declaration start char: 0, length: 46
param testUniqueString = uniqueString('asd', 'asdf', 'asdf')
//@[6:22) ParameterAssignment testUniqueString. Type: 'iizpqit7ih3cc'. Declaration start char: 0, length: 60
param testUri = uri('github.com', 'Azure/bicep')
//@[6:13) ParameterAssignment testUri. Type: string. Declaration start char: 0, length: 48
param testUriComponent = uriComponent(123)
//@[6:22) ParameterAssignment testUriComponent. Type: error. Declaration start char: 0, length: 42
param testUriComponentToString = uriComponentToString({})
//@[6:30) ParameterAssignment testUriComponentToString. Type: error. Declaration start char: 0, length: 57

param myObj = {
//@[6:11) ParameterAssignment myObj. Type: error. Declaration start char: 0, length: 249
  newGuid: newGuid()
  utcNow: utcNow()
  resourceId: resourceId('Microsoft.ContainerService/managedClusters', 'blah')
  deployment: deployment()
  environment: environment()
  azNs: az
  azNsFunc: az.providers('Microsoft.Compute')
}
