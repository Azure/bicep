var doggos = [
//@[004:010) Variable doggos. Type: ['Evie', 'Casper', 'Indy', 'Kira']. Declaration start char: 0, length: 54
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]

func isEven(i int) bool => i % 2 == 0
//@[012:013) Local i. Type: int. Declaration start char: 12, length: 5
//@[005:011) Function isEven. Type: int => bool. Declaration start char: 0, length: 37

var numbers = range(0, 4)
//@[004:011) Variable numbers. Type: int[]. Declaration start char: 0, length: 25

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[027:028) Local i. Type: 'Casper' | 'Evie' | 'Indy' | 'Kira'. Declaration start char: 27, length: 1
//@[004:012) Variable sayHello. Type: string[]. Declaration start char: 0, length: 46
// optional index parameter for map lambda
var sayHello2 = map(doggos, (dog, i) => '${isEven(i) ? 'Hi' : 'Ahoy'} ${dog}!')
//@[029:032) Local dog. Type: 'Casper' | 'Evie' | 'Indy' | 'Kira'. Declaration start char: 29, length: 3
//@[034:035) Local i. Type: int. Declaration start char: 34, length: 1
//@[004:013) Variable sayHello2. Type: string[]. Declaration start char: 0, length: 79

var evenNumbers = filter(numbers, i => isEven(i))
//@[034:035) Local i. Type: int. Declaration start char: 34, length: 1
//@[004:015) Variable evenNumbers. Type: int[]. Declaration start char: 0, length: 49
// optional index parameter for filter lambda
var evenEntries = filter(['a', 'b', 'c', 'd'], (item, i) => isEven(i))
//@[048:052) Local item. Type: 'a' | 'b' | 'c' | 'd'. Declaration start char: 48, length: 4
//@[054:055) Local i. Type: int. Declaration start char: 54, length: 1
//@[004:015) Variable evenEntries. Type: ('a' | 'b' | 'c' | 'd')[]. Declaration start char: 0, length: 70

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[080:081) Local j. Type: int. Declaration start char: 80, length: 1
//@[050:051) Local i. Type: int. Declaration start char: 50, length: 1
//@[103:104) Local x. Type: int. Declaration start char: 103, length: 1
//@[004:027) Variable evenDoggosNestedLambdas. Type: ('Casper' | 'Evie' | 'Indy' | 'Kira')[]. Declaration start char: 0, length: 118

var flattenedArrayOfArrays = flatten([[0, 1], [2, 3], [4, 5]])
//@[004:026) Variable flattenedArrayOfArrays. Type: [0, 1, 2, 3, 4, 5]. Declaration start char: 0, length: 62
var flattenedEmptyArray = flatten([])
//@[004:023) Variable flattenedEmptyArray. Type: <empty array>. Declaration start char: 0, length: 37

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[042:045) Local foo. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 42, length: 3
//@[004:012) Variable mapSayHi. Type: string[]. Declaration start char: 0, length: 62
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[023:026) Local foo. Type: never. Declaration start char: 23, length: 3
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
//@[004:012) Variable mapArray. Type: int[]. Declaration start char: 0, length: 67
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[049:050) Local i. Type: int. Declaration start char: 49, length: 1
//@[004:021) Variable mapMultiLineArray. Type: int[]. Declaration start char: 0, length: 95
  i * 3
  (i * 3) + 1
  (i * 3) + 2
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[056:059) Local foo. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 56, length: 3
//@[004:023) Variable filterEqualityCheck. Type: ('abc' | 'def' | 'ghi')[]. Declaration start char: 0, length: 76
var filterEmpty = filter([], foo => 'def' == foo)
//@[029:032) Local foo. Type: never. Declaration start char: 29, length: 3
//@[004:015) Variable filterEmpty. Type: never[]. Declaration start char: 0, length: 49

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[044:045) Local x. Type: -13 | 10 | 3 | 5 | 8. Declaration start char: 44, length: 1
//@[047:048) Local y. Type: -13 | 10 | 3 | 5 | 8. Declaration start char: 47, length: 1
//@[004:015) Variable sortNumeric. Type: (-13 | 10 | 3 | 5 | 8)[]. Declaration start char: 0, length: 59
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[045:046) Local x. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 45, length: 1
//@[048:049) Local y. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 48, length: 1
//@[004:013) Variable sortAlpha. Type: ('abc' | 'def' | 'ghi')[]. Declaration start char: 0, length: 60
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[052:053) Local x. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 52, length: 1
//@[055:056) Local y. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 55, length: 1
//@[004:020) Variable sortAlphaReverse. Type: ('abc' | 'def' | 'ghi')[]. Declaration start char: 0, length: 67
var sortByObjectKey = sort([
//@[004:019) Variable sortByObjectKey. Type: (object | object | object | object)[]. Declaration start char: 0, length: 188
  { key: 124, name: 'Second' }
  { key: 298, name: 'Third' }
  { key: 24, name: 'First' }
  { key: 1232, name: 'Fourth' }
], (x, y) => int(x.key) < int(y.key))
//@[004:005) Local x. Type: object | object | object | object. Declaration start char: 4, length: 1
//@[007:008) Local y. Type: object | object | object | object. Declaration start char: 7, length: 1
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[026:027) Local x. Type: never. Declaration start char: 26, length: 1
//@[029:030) Local y. Type: never. Declaration start char: 29, length: 1
//@[004:013) Variable sortEmpty. Type: never[]. Declaration start char: 0, length: 51

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[060:063) Local cur. Type: string. Declaration start char: 60, length: 3
//@[065:069) Local next. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 65, length: 4
//@[004:022) Variable reduceStringConcat. Type: string. Declaration start char: 0, length: 92
var reduceStringConcatEven = reduce(['abc', 'def', 'ghi'], '', (cur, next, i) => isEven(i) ? concat(cur, next) : cur)
//@[064:067) Local cur. Type: string. Declaration start char: 64, length: 3
//@[069:073) Local next. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 69, length: 4
//@[075:076) Local i. Type: int. Declaration start char: 75, length: 1
//@[004:026) Variable reduceStringConcatEven. Type: string. Declaration start char: 0, length: 117
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[046:049) Local cur. Type: int. Declaration start char: 46, length: 3
//@[051:055) Local next. Type: int. Declaration start char: 51, length: 4
//@[004:019) Variable reduceFactorial. Type: int. Declaration start char: 0, length: 71
var reduceObjectUnion = reduce([
//@[004:021) Variable reduceObjectUnion. Type: object. Declaration start char: 0, length: 117
  { foo: 123 }
  { bar: 456 }
  { baz: 789 }
], {}, (cur, next) => union(cur, next))
//@[008:011) Local cur. Type: object. Declaration start char: 8, length: 3
//@[013:017) Local next. Type: object | object | object. Declaration start char: 13, length: 4
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[033:036) Local cur. Type: 0. Declaration start char: 33, length: 3
//@[038:042) Local next. Type: never. Declaration start char: 38, length: 4
//@[004:015) Variable reduceEmpty. Type: 0. Declaration start char: 0, length: 51

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
//@[004:018) Variable mappedResProps. Type: array. Declaration start char: 0, length: 93

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

var objectMap = toObject([123, 456, 789], i => '${i / 100}')
//@[042:043) Local i. Type: 123 | 456 | 789. Declaration start char: 42, length: 1
//@[004:013) Variable objectMap. Type: object. Declaration start char: 0, length: 60
var objectMap2 = toObject(range(0, 10), i => '${i}', i => {
//@[040:041) Local i. Type: int. Declaration start char: 40, length: 1
//@[053:054) Local i. Type: int. Declaration start char: 53, length: 1
//@[004:014) Variable objectMap2. Type: object. Declaration start char: 0, length: 111
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@[043:044) Local x. Type: object | object | object | object. Declaration start char: 43, length: 1
//@[004:014) Variable objectMap3. Type: object. Declaration start char: 0, length: 55
var objectMap4 = toObject(sortByObjectKey, x =>
//@[043:044) Local x. Type: object | object | object | object. Declaration start char: 43, length: 1
//@[004:014) Variable objectMap4. Type: object. Declaration start char: 0, length: 60
  
  x.name)
var objectMap5 = toObject(sortByObjectKey, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx => xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.name)
//@[043:081) Local xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx. Type: object | object | object | object. Declaration start char: 43, length: 38
//@[004:014) Variable objectMap5. Type: object. Declaration start char: 0, length: 129
var objectMap6 = toObject(range(0, 10), i => '${i}', i => // comment
//@[040:041) Local i. Type: int. Declaration start char: 40, length: 1
//@[053:054) Local i. Type: int. Declaration start char: 53, length: 1
//@[004:014) Variable objectMap6. Type: object. Declaration start char: 0, length: 122
{
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})

var multiLine = reduce(['abc', 'def', 'ghi'], '', (
//@[004:013) Variable multiLine. Type: string. Declaration start char: 0, length: 89
  cur,
//@[002:005) Local cur. Type: string. Declaration start char: 2, length: 3
  next
//@[002:006) Local next. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 2, length: 4
) => concat(cur, next))

var multiLineWithComment = reduce(['abc', 'def', 'ghi'], '', (
//@[004:024) Variable multiLineWithComment. Type: string. Declaration start char: 0, length: 113
  // comment
  cur,
//@[002:005) Local cur. Type: string. Declaration start char: 2, length: 3
  next
//@[002:006) Local next. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 2, length: 4
) => concat(cur, next))

var mapVals = mapValues({
//@[004:011) Variable mapVals. Type: object. Declaration start char: 0, length: 62
  a: 123
  b: 456
}, val => val * 2)
//@[003:006) Local val. Type: 123 | 456. Declaration start char: 3, length: 3

var objectKeysTest = objectKeys({
//@[004:018) Variable objectKeysTest. Type: ('a' | 'b')[]. Declaration start char: 0, length: 54
  a: 123
  b: 456
})

var shallowMergeTest = shallowMerge([{
//@[004:020) Variable shallowMergeTest. Type: { a: 123, b: 456 }. Declaration start char: 0, length: 65
  a: 123
}, {
  b: 456
}])

var groupByTest = groupBy([
//@[004:015) Variable groupByTest. Type: object. Declaration start char: 0, length: 131
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type)
//@[003:006) Local arg. Type: object | object | object. Declaration start char: 3, length: 3

var groupByWithValMapTest = groupBy([
//@[004:025) Variable groupByWithValMapTest. Type: object. Declaration start char: 0, length: 159
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type, arg => arg.value)
//@[003:006) Local arg. Type: object | object | object. Declaration start char: 3, length: 3
//@[020:023) Local arg. Type: object | object | object. Declaration start char: 20, length: 3

