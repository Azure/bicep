var doggos = [
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]

var numbers = range(0, 4)

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[04:12) [no-unused-vars (Warning)] Variable "sayHello" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sayHello|

var isEven = filter(numbers, i => 0 == i % 2)
//@[04:10) [no-unused-vars (Warning)] Variable "isEven" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |isEven|

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[04:27) [no-unused-vars (Warning)] Variable "evenDoggosNestedLambdas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |evenDoggosNestedLambdas|

var flattenedArrayOfArrays = flatten([0, 1], [2, 3], [4, 5])
//@[04:26) [no-unused-vars (Warning)] Variable "flattenedArrayOfArrays" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |flattenedArrayOfArrays|
var flattenedEmptyArray = flatten()
//@[04:23) [no-unused-vars (Warning)] Variable "flattenedEmptyArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |flattenedEmptyArray|

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[04:12) [no-unused-vars (Warning)] Variable "mapSayHi" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapSayHi|
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[04:12) [no-unused-vars (Warning)] Variable "mapEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapEmpty|
var mapObject = map(range(0, length(doggos)), i => {
  i: i
  doggo: doggos[i]
  greeting: 'Ahoy, ${doggos[i]}!'
})
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@[04:12) [no-unused-vars (Warning)] Variable "mapArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapArray|
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[04:21) [no-unused-vars (Warning)] Variable "mapMultiLineArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapMultiLineArray|
  i * 3
  (i * 3) + 1
  (i * 3) + 2
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[04:23) [no-unused-vars (Warning)] Variable "filterEqualityCheck" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filterEqualityCheck|
var filterEmpty = filter([], foo => 'def' == foo)
//@[04:15) [no-unused-vars (Warning)] Variable "filterEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filterEmpty|

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[04:15) [no-unused-vars (Warning)] Variable "sortNumeric" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortNumeric|
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[04:13) [no-unused-vars (Warning)] Variable "sortAlpha" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortAlpha|
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[04:20) [no-unused-vars (Warning)] Variable "sortAlphaReverse" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortAlphaReverse|
var sortByObjectKey = sort([
//@[04:19) [no-unused-vars (Warning)] Variable "sortByObjectKey" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortByObjectKey|
  { key: 124, name: 'Second' }
  { key: 298, name: 'Third' }
  { key: 24, name: 'First' }
  { key: 1232, name: 'Fourth' }
], (x, y) => int(x.key) < int(y.key))
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[04:13) [no-unused-vars (Warning)] Variable "sortEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortEmpty|

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[04:22) [no-unused-vars (Warning)] Variable "reduceStringConcat" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceStringConcat|
//@[74:91) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(cur, next)|
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[04:19) [no-unused-vars (Warning)] Variable "reduceFactorial" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceFactorial|
var reduceObjectUnion = reduce([
//@[04:21) [no-unused-vars (Warning)] Variable "reduceObjectUnion" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceObjectUnion|
  { foo: 123 }
  { bar: 456 }
  { baz: 789 }
], {}, (cur, next) => union(cur, next))
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[04:15) [no-unused-vars (Warning)] Variable "reduceEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceEmpty|

var itemForLoop = [for item in range(0, 10): item]
var filteredLoop = filter(itemForLoop, i => i > 5)
//@[04:16) [no-unused-vars (Warning)] Variable "filteredLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filteredLoop|

output doggoGreetings array = [for item in mapObject: item.greeting]

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: 'asdfsadf'
}
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)
//@[04:18) [no-unused-vars (Warning)] Variable "mappedResProps" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mappedResProps|

module myMod './test.bicep' = {
  name: 'asdfsadf'
  params: {
    outputThis: map(mapObject, obj => obj.doggo)
  }
}
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')
//@[04:24) [no-unused-vars (Warning)] Variable "mappedModOutputProps" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mappedModOutputProps|

var parentheses = map([123], (i => '${i}'))
//@[04:15) [no-unused-vars (Warning)] Variable "parentheses" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |parentheses|

