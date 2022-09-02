var doggos = [
//@[004:010) Variable doggos. Type: ('Casper' | 'Evie' | 'Indy' | 'Kira')[]. Declaration start char: 0, length: 54
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]

var numbers = range(0, 4)
//@[004:011) Variable numbers. Type: int[]. Declaration start char: 0, length: 25

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[027:028) Local i. Type: 'Casper' | 'Evie' | 'Indy' | 'Kira'. Declaration start char: 27, length: 1
//@[004:012) Variable sayHello. Type: string[]. Declaration start char: 0, length: 46

var isEven = filter(numbers, i => 0 == i % 2)
//@[029:030) Local i. Type: int. Declaration start char: 29, length: 1
//@[004:010) Variable isEven. Type: int[]. Declaration start char: 0, length: 45

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[080:081) Local j. Type: int. Declaration start char: 80, length: 1
//@[050:051) Local i. Type: int. Declaration start char: 50, length: 1
//@[103:104) Local x. Type: int. Declaration start char: 103, length: 1
//@[004:027) Variable evenDoggosNestedLambdas. Type: ('Casper' | 'Evie' | 'Indy' | 'Kira')[]. Declaration start char: 0, length: 118

var flattenedArrayOfArrays = flatten([0, 1], [2, 3], [4, 5])
//@[004:026) Variable flattenedArrayOfArrays. Type: array. Declaration start char: 0, length: 60
var flattenedEmptyArray = flatten()
//@[004:023) Variable flattenedEmptyArray. Type: array. Declaration start char: 0, length: 35

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[042:045) Local foo. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 42, length: 3
//@[004:012) Variable mapSayHi. Type: string[]. Declaration start char: 0, length: 62
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[023:026) Local foo. Type: any. Declaration start char: 23, length: 3
//@[004:012) Variable mapEmpty. Type: string[]. Declaration start char: 0, length: 43
var mapObject = map(range(0, length(doggos)), i => {
//@[046:047) Local i. Type: int. Declaration start char: 46, length: 1
//@[004:013) Variable mapObject. Type: object[]. Declaration start char: 0, length: 115
  i: i
  doggo: doggos[i]
  greeting: 'Ahoy, ${doggos[i]}!'
})
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@[040:041) Local i. Type: int. Declaration start char: 40, length: 1
//@[004:012) Variable mapArray. Type: array. Declaration start char: 0, length: 67
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[049:050) Local i. Type: int. Declaration start char: 49, length: 1
//@[004:021) Variable mapMultiLineArray. Type: array. Declaration start char: 0, length: 95
  i * 3
  (i * 3) + 1
  (i * 3) + 2
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[056:059) Local foo. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 56, length: 3
//@[004:023) Variable filterEqualityCheck. Type: ('abc' | 'def' | 'ghi')[]. Declaration start char: 0, length: 76
var filterEmpty = filter([], foo => 'def' == foo)
//@[029:032) Local foo. Type: any. Declaration start char: 29, length: 3
//@[004:015) Variable filterEmpty. Type: array. Declaration start char: 0, length: 49

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[044:045) Local x. Type: int. Declaration start char: 44, length: 1
//@[047:048) Local y. Type: int. Declaration start char: 47, length: 1
//@[004:015) Variable sortNumeric. Type: int[]. Declaration start char: 0, length: 59
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[045:046) Local x. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 45, length: 1
//@[048:049) Local y. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 48, length: 1
//@[004:013) Variable sortAlpha. Type: ('abc' | 'def' | 'ghi')[]. Declaration start char: 0, length: 60
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[052:053) Local x. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 52, length: 1
//@[055:056) Local y. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 55, length: 1
//@[004:020) Variable sortAlphaReverse. Type: ('abc' | 'def' | 'ghi')[]. Declaration start char: 0, length: 67
var sortByObjectKey = sort([
//@[004:019) Variable sortByObjectKey. Type: array. Declaration start char: 0, length: 188
  { key: 124, name: 'Second' }
  { key: 298, name: 'Third' }
  { key: 24, name: 'First' }
  { key: 1232, name: 'Fourth' }
], (x, y) => int(x.key) < int(y.key))
//@[004:005) Local x. Type: any. Declaration start char: 4, length: 1
//@[007:008) Local y. Type: any. Declaration start char: 7, length: 1
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[026:027) Local x. Type: any. Declaration start char: 26, length: 1
//@[029:030) Local y. Type: any. Declaration start char: 29, length: 1
//@[004:013) Variable sortEmpty. Type: array. Declaration start char: 0, length: 51

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[060:063) Local cur. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 60, length: 3
//@[065:069) Local next. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 65, length: 4
//@[004:022) Variable reduceStringConcat. Type: string. Declaration start char: 0, length: 92
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[046:049) Local cur. Type: int. Declaration start char: 46, length: 3
//@[051:055) Local next. Type: int. Declaration start char: 51, length: 4
//@[004:019) Variable reduceFactorial. Type: int. Declaration start char: 0, length: 71
var reduceObjectUnion = reduce([
//@[004:021) Variable reduceObjectUnion. Type: any. Declaration start char: 0, length: 117
  { foo: 123 }
  { bar: 456 }
  { baz: 789 }
], {}, (cur, next) => union(cur, next))
//@[008:011) Local cur. Type: any. Declaration start char: 8, length: 3
//@[013:017) Local next. Type: any. Declaration start char: 13, length: 4
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[033:036) Local cur. Type: any. Declaration start char: 33, length: 3
//@[038:042) Local next. Type: any. Declaration start char: 38, length: 4
//@[004:015) Variable reduceEmpty. Type: any. Declaration start char: 0, length: 51

var itemForLoop = [for item in range(0, 10): item]
//@[023:027) Local item. Type: int. Declaration start char: 23, length: 4
//@[004:015) Variable itemForLoop. Type: int[]. Declaration start char: 0, length: 50
var filteredLoop = filter(itemForLoop, i => i > 5)
//@[039:040) Local i. Type: int. Declaration start char: 39, length: 1
//@[004:016) Variable filteredLoop. Type: int[]. Declaration start char: 0, length: 50

output doggoGreetings array = [for item in mapObject: item.greeting]
//@[035:039) Local item. Type: object. Declaration start char: 35, length: 4
//@[007:021) Output doggoGreetings. Type: array. Declaration start char: 0, length: 68

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
//@[009:019) Resource storageAcc. Type: Microsoft.Storage/storageAccounts@2021-09-01. Declaration start char: 0, length: 100
  name: 'asdfsadf'
}
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)
//@[074:078) Local item. Type: object. Declaration start char: 74, length: 4
//@[004:018) Variable mappedResProps. Type: any[]. Declaration start char: 0, length: 93

module myMod './test.bicep' = {
//@[007:012) Module myMod. Type: module. Declaration start char: 0, length: 117
  name: 'asdfsadf'
  params: {
    outputThis: map(mapObject, obj => obj.doggo)
//@[031:034) Local obj. Type: object. Declaration start char: 31, length: 3
  }
}
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')
//@[057:062) Local doggo. Type: any. Declaration start char: 57, length: 5
//@[004:024) Variable mappedModOutputProps. Type: string[]. Declaration start char: 0, length: 87

var parentheses = map([123], (i => '${i}'))
//@[030:031) Local i. Type: any. Declaration start char: 30, length: 1
//@[004:015) Variable parentheses. Type: string[]. Declaration start char: 0, length: 43

