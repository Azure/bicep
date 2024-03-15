var doggos = [
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]
var numbers = range(0, 4)
var sayHello = map(doggos, i => 'Hello ${i}!')
//@[4:12) [no-unused-vars (Warning)] Variable "sayHello" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sayHello|
var isEven = filter(numbers, i => (0 == (i % 2)))
//@[4:10) [no-unused-vars (Warning)] Variable "isEven" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |isEven|
var evenDoggosNestedLambdas = map(
//@[4:27) [no-unused-vars (Warning)] Variable "evenDoggosNestedLambdas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |evenDoggosNestedLambdas|
  filter(numbers, i => contains(filter(numbers, j => (0 == (j % 2))), i)),
  x => doggos[x]
)
var flattenedArrayOfArrays = flatten([
//@[4:26) [no-unused-vars (Warning)] Variable "flattenedArrayOfArrays" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |flattenedArrayOfArrays|
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
//@[4:23) [no-unused-vars (Warning)] Variable "flattenedEmptyArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |flattenedEmptyArray|
var mapSayHi = map(
//@[4:12) [no-unused-vars (Warning)] Variable "mapSayHi" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapSayHi|
  [
    'abc'
    'def'
    'ghi'
  ],
  foo => 'Hi ${foo}!'
)
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[4:12) [no-unused-vars (Warning)] Variable "mapEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapEmpty|
var mapObject = map(
  range(0, length(doggos)),
  i => {
    i: i
    doggo: doggos[i]
    greeting: 'Ahoy, ${doggos[i]}!'
  }
)
var mapArray = flatten(map(
//@[4:12) [no-unused-vars (Warning)] Variable "mapArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapArray|
  range(1, 3),
  i => [
    (i * 2)
    ((i * 2) + 1)
  ]
))
var mapMultiLineArray = flatten(map(
//@[4:21) [no-unused-vars (Warning)] Variable "mapMultiLineArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mapMultiLineArray|
  range(1, 3),
  i => [
    (i * 3)
    ((i * 3) + 1)
    ((i * 3) + 2)
  ]
))
var filterEqualityCheck = filter(
//@[4:23) [no-unused-vars (Warning)] Variable "filterEqualityCheck" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filterEqualityCheck|
  [
    'abc'
    'def'
    'ghi'
  ],
  foo => ('def' == foo)
)
var filterEmpty = filter([], foo => ('def' == foo))
//@[4:15) [no-unused-vars (Warning)] Variable "filterEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filterEmpty|
var sortNumeric = sort(
//@[4:15) [no-unused-vars (Warning)] Variable "sortNumeric" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortNumeric|
  [
    8
    3
    10
    13
    5
  ],
  (x, y) => (x < y)
)
var sortAlpha = sort(
//@[4:13) [no-unused-vars (Warning)] Variable "sortAlpha" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortAlpha|
  [
    'ghi'
    'abc'
    'def'
  ],
  (x, y) => (x < y)
)
var sortAlphaReverse = sort(
//@[4:20) [no-unused-vars (Warning)] Variable "sortAlphaReverse" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortAlphaReverse|
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
//@[4:13) [no-unused-vars (Warning)] Variable "sortEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sortEmpty|
var reduceStringConcat = reduce(
//@[4:22) [no-unused-vars (Warning)] Variable "reduceStringConcat" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceStringConcat|
  [
    'abc'
    'def'
    'ghi'
  ],
  '',
  (cur, next) => concat(cur, next)
)
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => (cur * next))
//@[4:19) [no-unused-vars (Warning)] Variable "reduceFactorial" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceFactorial|
var reduceObjectUnion = reduce(
//@[4:21) [no-unused-vars (Warning)] Variable "reduceObjectUnion" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceObjectUnion|
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
//@[4:15) [no-unused-vars (Warning)] Variable "reduceEmpty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |reduceEmpty|
var filteredLoop = filter(itemForLoop, i => (i > 5))
//@[4:16) [no-unused-vars (Warning)] Variable "filteredLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |filteredLoop|
var parentheses = map(
//@[4:15) [no-unused-vars (Warning)] Variable "parentheses" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |parentheses|
  [
    123
  ],
  i => i
)
var objectMap = toObject(
//@[4:13) [no-unused-vars (Warning)] Variable "objectMap" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap|
  [
    123
    456
    789
  ],
  i => (i / 100)
//@[2:16) [BCP070 (Error)] Argument of type "any => int" is not assignable to parameter of type "any => string". (CodeDescription: none) |i => (i / 100)|
)
var objectMap2 = toObject(
//@[4:14) [no-unused-vars (Warning)] Variable "objectMap2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap2|
  range(0, 10),
  i => i,
  i => {
    isEven: ((i % 2) == 0)
    isGreaterThan4: (i > 4)
  }
)
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@[4:14) [no-unused-vars (Warning)] Variable "objectMap3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectMap3|
var itemForLoop = [for i in range(0, length(range(0, 10))): range(0, 10)[i]]

module asdfsadf './nested_asdfsadf.bicep' = {
  name: 'asdfsadf'
  params: {
    outputThis: map(mapObject, obj => obj.doggo)
  }
}

output doggoGreetings array = [for item in mapObject: item.greeting]

