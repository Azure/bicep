var doggos = [
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]
var numbers = range(0, 4)
var sayHello = map(doggos, i => 'Hello ${i}!')
//@[04:12) [no-unused-vars (Warning)] Variable "sayHello" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sayHello|
var isEven = filter(numbers, i => (0 == (i % 2)))
//@[04:10) [no-unused-vars (Warning)] Variable "isEven" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |isEven|
var evenDoggosNestedLambdas = map(
//@[04:27) [no-unused-vars (Warning)] Variable "evenDoggosNestedLambdas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |evenDoggosNestedLambdas|
  filter(numbers, i => contains(filter(numbers, j => (0 == (j % 2))), i)),
  x => doggos[x]
)
var flattenedArrayOfArrays = flatten([
//@[04:26) [no-unused-vars (Warning)] Variable "flattenedArrayOfArrays" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |flattenedArrayOfArrays|
  [
    0
    1
  ]
  [
    2
    3
  ]
  [
    4
    5
  ]
])
var flattenedEmptyArray = flatten([])
//@[04:23) [no-unused-vars (Warning)] Variable "flattenedEmptyArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |flattenedEmptyArray|
var mapSayHi = map(
//@[04:12) [no-unused-vars (Warning)] Variable "mapSayHi" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapSayHi|
  [
    'abc'
    'def'
    'ghi'
  ],
  foo => 'Hi ${foo}!'
)
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[04:12) [no-unused-vars (Warning)] Variable "mapEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapEmpty|
var mapObject = map(range(0, length(doggos)), i => {
  i: i
  doggo: doggos[i]
  greeting: 'Ahoy, ${doggos[i]}!'
})
var mapArray = flatten(map(range(1, 3), i => [
//@[04:12) [no-unused-vars (Warning)] Variable "mapArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapArray|
  (i * 2)
  ((i * 2) + 1)
]))
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[04:21) [no-unused-vars (Warning)] Variable "mapMultiLineArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mapMultiLineArray|
  (i * 3)
  ((i * 3) + 1)
  ((i * 3) + 2)
]))
var filterEqualityCheck = filter(
//@[04:23) [no-unused-vars (Warning)] Variable "filterEqualityCheck" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filterEqualityCheck|
  [
    'abc'
    'def'
    'ghi'
  ],
  foo => ('def' == foo)
)
var filterEmpty = filter([], foo => ('def' == foo))
//@[04:15) [no-unused-vars (Warning)] Variable "filterEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filterEmpty|
var sortNumeric = sort(
//@[04:15) [no-unused-vars (Warning)] Variable "sortNumeric" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sortNumeric|
  [
    8
    3
    10
    -13
    5
  ],
  (x, y) => (x < y)
)
var sortAlpha = sort(
//@[04:13) [no-unused-vars (Warning)] Variable "sortAlpha" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sortAlpha|
  [
    'ghi'
    'abc'
    'def'
  ],
  (x, y) => (x < y)
)
var sortAlphaReverse = sort(
//@[04:20) [no-unused-vars (Warning)] Variable "sortAlphaReverse" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sortAlphaReverse|
  [
    'ghi'
    'abc'
    'def'
  ],
  (x, y) => (x > y)
)
var sortByObjectKey = sort(
  [
    {
      key: 124
      name: 'Second'
    }
    {
      key: 298
      name: 'Third'
    }
    {
      key: 24
      name: 'First'
    }
    {
      key: 1232
      name: 'Fourth'
    }
  ],
  (x, y) => (int(x.key) < int(y.key))
)
var sortEmpty = sort([], (x, y) => (int(x) < int(y)))
//@[04:13) [no-unused-vars (Warning)] Variable "sortEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sortEmpty|
var reduceStringConcat = reduce(
//@[04:22) [no-unused-vars (Warning)] Variable "reduceStringConcat" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceStringConcat|
  [
    'abc'
    'def'
    'ghi'
  ],
  '',
  (cur, next) => concat(cur, next)
//@[17:34) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(cur, next)|
)
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => (cur * next))
//@[04:19) [no-unused-vars (Warning)] Variable "reduceFactorial" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceFactorial|
var reduceObjectUnion = reduce(
//@[04:21) [no-unused-vars (Warning)] Variable "reduceObjectUnion" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceObjectUnion|
  [
    {
      foo: 123
    }
    {
      bar: 456
    }
    {
      baz: 789
    }
  ],
  {},
  (cur, next) => union(cur, next)
)
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[04:15) [no-unused-vars (Warning)] Variable "reduceEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduceEmpty|
var filteredLoop = filter(itemForLoop, i => (i > 5))
//@[04:16) [no-unused-vars (Warning)] Variable "filteredLoop" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filteredLoop|
var parentheses = map(
//@[04:15) [no-unused-vars (Warning)] Variable "parentheses" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |parentheses|
  [
    123
  ],
  i => i
)
var objectMap = toObject(
//@[04:13) [no-unused-vars (Warning)] Variable "objectMap" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap|
  [
    123
    456
    789
  ],
  i => (i / 100)
//@[02:16) [BCP070 (Error)] Argument of type "(123 | 456 | 789) => int" is not assignable to parameter of type "any => string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => (i / 100)|
)
var objectMap2 = toObject(range(0, 10), i => i, i => {
//@[04:14) [no-unused-vars (Warning)] Variable "objectMap2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap2|
//@[40:46) [BCP070 (Error)] Argument of type "int => int" is not assignable to parameter of type "any => string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => i|
  isEven: ((i % 2) == 0)
  isGreaterThan4: (i > 4)
})
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@[04:14) [no-unused-vars (Warning)] Variable "objectMap3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectMap3|
var itemForLoop = [for i in range(0, length(range(0, 10))): range(0, 10)[i]]

module asdfsadf './nested_asdfsadf.bicep' = {
  name: 'asdfsadf'
  params: {
    outputThis: map(mapObject, obj => obj.doggo)
  }
}

output doggoGreetings array = [for item in mapObject: item.greeting]

