var doggos = [
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]

func isEven(i int) bool => i % 2 == 0

var numbers = range(0, 4)

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[04:012) [no-unused-vars (Warning)] Variable "sayHello" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sayHello|
// optional index parameter for map lambda
var sayHello2 = map(doggos, (dog, i) => '${isEven(i) ? 'Hi' : 'Ahoy'} ${dog}!')
//@[04:013) [no-unused-vars (Warning)] Variable "sayHello2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sayHello2|

var evenNumbers = filter(numbers, i => isEven(i))
//@[04:015) [no-unused-vars (Warning)] Variable "evenNumbers" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |evenNumbers|
// optional index parameter for filter lambda
var evenEntries = filter(['a', 'b', 'c', 'd'], (item, i) => isEven(i))
//@[04:015) [no-unused-vars (Warning)] Variable "evenEntries" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |evenEntries|

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[04:027) [no-unused-vars (Warning)] Variable "evenDoggosNestedLambdas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |evenDoggosNestedLambdas|

var flattenedArrayOfArrays = flatten([[0, 1], [2, 3], [4, 5]])
//@[04:026) [no-unused-vars (Warning)] Variable "flattenedArrayOfArrays" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |flattenedArrayOfArrays|
var flattenedEmptyArray = flatten([])
//@[04:023) [no-unused-vars (Warning)] Variable "flattenedEmptyArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |flattenedEmptyArray|

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[04:012) [no-unused-vars (Warning)] Variable "mapSayHi" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapSayHi|
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[04:012) [no-unused-vars (Warning)] Variable "mapEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapEmpty|
var mapObject = map(range(0, length(doggos)), i => {
  i: i
  doggo: doggos[i]
  greeting: 'Ahoy, ${doggos[i]}!'
})
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@[04:012) [no-unused-vars (Warning)] Variable "mapArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapArray|
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[04:021) [no-unused-vars (Warning)] Variable "mapMultiLineArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapMultiLineArray|
  i * 3
  (i * 3) + 1
  (i * 3) + 2
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[04:023) [no-unused-vars (Warning)] Variable "filterEqualityCheck" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filterEqualityCheck|
var filterEmpty = filter([], foo => 'def' == foo)
//@[04:015) [no-unused-vars (Warning)] Variable "filterEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filterEmpty|

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[04:015) [no-unused-vars (Warning)] Variable "sortNumeric" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sortNumeric|
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[04:013) [no-unused-vars (Warning)] Variable "sortAlpha" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sortAlpha|
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[04:020) [no-unused-vars (Warning)] Variable "sortAlphaReverse" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sortAlphaReverse|
var sortByObjectKey = sort([
  { key: 124, name: 'Second' }
  { key: 298, name: 'Third' }
  { key: 24, name: 'First' }
  { key: 1232, name: 'Fourth' }
], (x, y) => int(x.key) < int(y.key))
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[04:013) [no-unused-vars (Warning)] Variable "sortEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sortEmpty|

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[04:022) [no-unused-vars (Warning)] Variable "reduceStringConcat" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceStringConcat|
//@[74:091) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(cur, next)|
var reduceStringConcatEven = reduce(['abc', 'def', 'ghi'], '', (cur, next, i) => isEven(i) ? concat(cur, next) : cur)
//@[04:026) [no-unused-vars (Warning)] Variable "reduceStringConcatEven" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceStringConcatEven|
//@[93:110) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(cur, next)|
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[04:019) [no-unused-vars (Warning)] Variable "reduceFactorial" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceFactorial|
var reduceObjectUnion = reduce([
//@[04:021) [no-unused-vars (Warning)] Variable "reduceObjectUnion" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceObjectUnion|
  { foo: 123 }
  { bar: 456 }
  { baz: 789 }
], {}, (cur, next) => union(cur, next))
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[04:015) [no-unused-vars (Warning)] Variable "reduceEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceEmpty|

var itemForLoop = [for item in range(0, 10): item]
var filteredLoop = filter(itemForLoop, i => i > 5)
//@[04:016) [no-unused-vars (Warning)] Variable "filteredLoop" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filteredLoop|

output doggoGreetings array = [for item in mapObject: item.greeting]
//@[22:027) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: 'asdfsadf'
}
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)
//@[04:018) [no-unused-vars (Warning)] Variable "mappedResProps" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mappedResProps|

module myMod './test.bicep' = {
  name: 'asdfsadf'
  params: {
    outputThis: map(mapObject, obj => obj.doggo)
  }
}
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')
//@[04:024) [no-unused-vars (Warning)] Variable "mappedModOutputProps" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mappedModOutputProps|

var parentheses = map([123], (i => '${i}'))
//@[04:015) [no-unused-vars (Warning)] Variable "parentheses" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |parentheses|

var objectMap = toObject([123, 456, 789], i => '${i / 100}')
//@[04:013) [no-unused-vars (Warning)] Variable "objectMap" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap|
var objectMap2 = toObject(range(0, 10), i => '${i}', i => {
//@[04:014) [no-unused-vars (Warning)] Variable "objectMap2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap2|
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@[04:014) [no-unused-vars (Warning)] Variable "objectMap3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap3|
var objectMap4 = toObject(sortByObjectKey, x =>
//@[04:014) [no-unused-vars (Warning)] Variable "objectMap4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap4|
  
  x.name)
var objectMap5 = toObject(sortByObjectKey, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx => xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.name)
//@[04:014) [no-unused-vars (Warning)] Variable "objectMap5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap5|
var objectMap6 = toObject(range(0, 10), i => '${i}', i => // comment
//@[04:014) [no-unused-vars (Warning)] Variable "objectMap6" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap6|
{
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})

var multiLine = reduce(['abc', 'def', 'ghi'], '', (
//@[04:013) [no-unused-vars (Warning)] Variable "multiLine" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLine|
  cur,
  next
) => concat(cur, next))
//@[05:022) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(cur, next)|

var multiLineWithComment = reduce(['abc', 'def', 'ghi'], '', (
//@[04:024) [no-unused-vars (Warning)] Variable "multiLineWithComment" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineWithComment|
  // comment
  cur,
  next
) => concat(cur, next))
//@[05:022) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(cur, next)|

var mapVals = mapValues({
//@[04:011) [no-unused-vars (Warning)] Variable "mapVals" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapVals|
  a: 123
  b: 456
}, val => val * 2)

var objectKeysTest = objectKeys({
//@[04:018) [no-unused-vars (Warning)] Variable "objectKeysTest" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectKeysTest|
  a: 123
  b: 456
})

var shallowMergeTest = shallowMerge([{
//@[04:020) [no-unused-vars (Warning)] Variable "shallowMergeTest" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |shallowMergeTest|
  a: 123
}, {
  b: 456
}])

var groupByTest = groupBy([
//@[04:015) [no-unused-vars (Warning)] Variable "groupByTest" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |groupByTest|
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type)

var groupByWithValMapTest = groupBy([
//@[04:025) [no-unused-vars (Warning)] Variable "groupByWithValMapTest" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |groupByWithValMapTest|
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type, arg => arg.value)

