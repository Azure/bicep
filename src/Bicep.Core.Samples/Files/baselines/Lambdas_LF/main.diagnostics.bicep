var doggos = [
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]

func isEven(i int) bool => i % 2 == 0

var numbers = range(0, 4)

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[4:12) [no-unused-vars (Warning)] Variable "sayHello" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sayHello|
// optional index parameter for map lambda
var sayHello2 = map(doggos, (dog, i) => '${isEven(i) ? 'Hi' : 'Ahoy'} ${dog}!')
//@[4:13) [no-unused-vars (Warning)] Variable "sayHello2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sayHello2|

var evenNumbers = filter(numbers, i => isEven(i))
//@[4:15) [no-unused-vars (Warning)] Variable "evenNumbers" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |evenNumbers|
// optional index parameter for filter lambda
var evenEntries = filter(['a', 'b', 'c', 'd'], (item, i) => isEven(i))
//@[4:15) [no-unused-vars (Warning)] Variable "evenEntries" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |evenEntries|

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[4:27) [no-unused-vars (Warning)] Variable "evenDoggosNestedLambdas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |evenDoggosNestedLambdas|

var flattenedArrayOfArrays = flatten([[0, 1], [2, 3], [4, 5]])
//@[4:26) [no-unused-vars (Warning)] Variable "flattenedArrayOfArrays" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |flattenedArrayOfArrays|
var flattenedEmptyArray = flatten([])
//@[4:23) [no-unused-vars (Warning)] Variable "flattenedEmptyArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |flattenedEmptyArray|

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[4:12) [no-unused-vars (Warning)] Variable "mapSayHi" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapSayHi|
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[4:12) [no-unused-vars (Warning)] Variable "mapEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapEmpty|
var mapObject = map(range(0, length(doggos)), i => {
  i: i
  doggo: doggos[i]
  greeting: 'Ahoy, ${doggos[i]}!'
})
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@[4:12) [no-unused-vars (Warning)] Variable "mapArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapArray|
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[4:21) [no-unused-vars (Warning)] Variable "mapMultiLineArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapMultiLineArray|
  i * 3
  (i * 3) + 1
  (i * 3) + 2
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[4:23) [no-unused-vars (Warning)] Variable "filterEqualityCheck" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filterEqualityCheck|
var filterEmpty = filter([], foo => 'def' == foo)
//@[4:15) [no-unused-vars (Warning)] Variable "filterEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filterEmpty|

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[4:15) [no-unused-vars (Warning)] Variable "sortNumeric" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortNumeric|
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[4:13) [no-unused-vars (Warning)] Variable "sortAlpha" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortAlpha|
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[4:20) [no-unused-vars (Warning)] Variable "sortAlphaReverse" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortAlphaReverse|
var sortByObjectKey = sort([
  { key: 124, name: 'Second' }
  { key: 298, name: 'Third' }
  { key: 24, name: 'First' }
  { key: 1232, name: 'Fourth' }
], (x, y) => int(x.key) < int(y.key))
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[4:13) [no-unused-vars (Warning)] Variable "sortEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortEmpty|

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[4:22) [no-unused-vars (Warning)] Variable "reduceStringConcat" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceStringConcat|
var reduceStringConcatEven = reduce(['abc', 'def', 'ghi'], '', (cur, next, i) => isEven(i) ? concat(cur, next) : cur)
//@[4:26) [no-unused-vars (Warning)] Variable "reduceStringConcatEven" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceStringConcatEven|
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[4:19) [no-unused-vars (Warning)] Variable "reduceFactorial" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceFactorial|
var reduceObjectUnion = reduce([
//@[4:21) [no-unused-vars (Warning)] Variable "reduceObjectUnion" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceObjectUnion|
  { foo: 123 }
  { bar: 456 }
  { baz: 789 }
], {}, (cur, next) => union(cur, next))
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[4:15) [no-unused-vars (Warning)] Variable "reduceEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceEmpty|

var itemForLoop = [for item in range(0, 10): item]
var filteredLoop = filter(itemForLoop, i => i > 5)
//@[4:16) [no-unused-vars (Warning)] Variable "filteredLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filteredLoop|

output doggoGreetings array = [for item in mapObject: item.greeting]

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: 'asdfsadf'
}
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)
//@[4:18) [no-unused-vars (Warning)] Variable "mappedResProps" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mappedResProps|

module myMod './test.bicep' = {
  name: 'asdfsadf'
  params: {
    outputThis: map(mapObject, obj => obj.doggo)
  }
}
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')
//@[4:24) [no-unused-vars (Warning)] Variable "mappedModOutputProps" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mappedModOutputProps|

var parentheses = map([123], (i => '${i}'))
//@[4:15) [no-unused-vars (Warning)] Variable "parentheses" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |parentheses|

var objectMap = toObject([123, 456, 789], i => '${i / 100}')
//@[4:13) [no-unused-vars (Warning)] Variable "objectMap" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap|
var objectMap2 = toObject(range(0, 10), i => '${i}', i => {
//@[4:14) [no-unused-vars (Warning)] Variable "objectMap2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap2|
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@[4:14) [no-unused-vars (Warning)] Variable "objectMap3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap3|
var objectMap4 = toObject(sortByObjectKey, x =>
//@[4:14) [no-unused-vars (Warning)] Variable "objectMap4" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap4|
  
  x.name)
var objectMap5 = toObject(sortByObjectKey, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx => xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.name)
//@[4:14) [no-unused-vars (Warning)] Variable "objectMap5" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap5|
var objectMap6 = toObject(range(0, 10), i => '${i}', i => // comment
//@[4:14) [no-unused-vars (Warning)] Variable "objectMap6" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap6|
{
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})

var multiLine = reduce(['abc', 'def', 'ghi'], '', (
//@[4:13) [no-unused-vars (Warning)] Variable "multiLine" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLine|
  cur,
  next
) => concat(cur, next))

var multiLineWithComment = reduce(['abc', 'def', 'ghi'], '', (
//@[4:24) [no-unused-vars (Warning)] Variable "multiLineWithComment" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineWithComment|
  // comment
  cur,
  next
) => concat(cur, next))

var mapVals = mapValues({
//@[4:11) [no-unused-vars (Warning)] Variable "mapVals" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapVals|
  a: 123
  b: 456
}, val => val * 2)

var objectKeysTest = objectKeys({
//@[4:18) [no-unused-vars (Warning)] Variable "objectKeysTest" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectKeysTest|
  a: 123
  b: 456
})

var shallowMergeTest = shallowMerge([{
//@[4:20) [no-unused-vars (Warning)] Variable "shallowMergeTest" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |shallowMergeTest|
  a: 123
}, {
  b: 456
}])

var groupByTest = groupBy([
//@[4:15) [no-unused-vars (Warning)] Variable "groupByTest" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |groupByTest|
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type)

var groupByWithValMapTest = groupBy([
//@[4:25) [no-unused-vars (Warning)] Variable "groupByWithValMapTest" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |groupByWithValMapTest|
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type, arg => arg.value)

