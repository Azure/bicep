var doggos = [
//@    "doggos": [
//@    ],
  'Evie'
//@      "Evie",
  'Casper'
//@      "Casper",
  'Indy'
//@      "Indy",
  'Kira'
//@      "Kira"
]

func isEven(i int) bool => i % 2 == 0
//@        "isEven": {
//@          "parameters": [
//@            {
//@              "type": "int",
//@              "name": "i"
//@            }
//@          ],
//@          "output": {
//@            "type": "bool",
//@            "value": "[equals(mod(parameters('i'), 2), 0)]"
//@          }
//@        }

var numbers = range(0, 4)
//@    "numbers": "[range(0, 4)]",

var sayHello = map(doggos, i => 'Hello ${i}!')
//@    "sayHello": "[map(variables('doggos'), lambda('i', format('Hello {0}!', lambdaVariables('i'))))]",
// optional index parameter for map lambda
var sayHello2 = map(doggos, (dog, i) => '${isEven(i) ? 'Hi' : 'Ahoy'} ${dog}!')
//@    "sayHello2": "[map(variables('doggos'), lambda('dog', 'i', format('{0} {1}!', if(__bicep.isEven(lambdaVariables('i')), 'Hi', 'Ahoy'), lambdaVariables('dog'))))]",

var evenNumbers = filter(numbers, i => isEven(i))
//@    "evenNumbers": "[filter(variables('numbers'), lambda('i', __bicep.isEven(lambdaVariables('i'))))]",
// optional index parameter for filter lambda
var evenEntries = filter(['a', 'b', 'c', 'd'], (item, i) => isEven(i))
//@    "evenEntries": "[filter(createArray('a', 'b', 'c', 'd'), lambda('item', 'i', __bicep.isEven(lambdaVariables('i'))))]",

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@    "evenDoggosNestedLambdas": "[map(filter(variables('numbers'), lambda('i', contains(filter(variables('numbers'), lambda('j', equals(0, mod(lambdaVariables('j'), 2)))), lambdaVariables('i')))), lambda('x', variables('doggos')[lambdaVariables('x')]))]",

var flattenedArrayOfArrays = flatten([[0, 1], [2, 3], [4, 5]])
//@    "flattenedArrayOfArrays": "[flatten(createArray(createArray(0, 1), createArray(2, 3), createArray(4, 5)))]",
var flattenedEmptyArray = flatten([])
//@    "flattenedEmptyArray": "[flatten(createArray())]",

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@    "mapSayHi": "[map(createArray('abc', 'def', 'ghi'), lambda('foo', format('Hi {0}!', lambdaVariables('foo'))))]",
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@    "mapEmpty": "[map(createArray(), lambda('foo', format('Hi {0}!', lambdaVariables('foo'))))]",
var mapObject = map(range(0, length(doggos)), i => {
//@    "mapObject": "[map(range(0, length(variables('doggos'))), lambda('i', createObject('i', lambdaVariables('i'), 'doggo', variables('doggos')[lambdaVariables('i')], 'greeting', format('Ahoy, {0}!', variables('doggos')[lambdaVariables('i')]))))]",
  i: i
  doggo: doggos[i]
  greeting: 'Ahoy, ${doggos[i]}!'
})
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@    "mapArray": "[flatten(map(range(1, 3), lambda('i', createArray(mul(lambdaVariables('i'), 2), add(mul(lambdaVariables('i'), 2), 1)))))]",
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@    "mapMultiLineArray": "[flatten(map(range(1, 3), lambda('i', createArray(mul(lambdaVariables('i'), 3), add(mul(lambdaVariables('i'), 3), 1), add(mul(lambdaVariables('i'), 3), 2)))))]",
  i * 3
  (i * 3) + 1
  (i * 3) + 2
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@    "filterEqualityCheck": "[filter(createArray('abc', 'def', 'ghi'), lambda('foo', equals('def', lambdaVariables('foo'))))]",
var filterEmpty = filter([], foo => 'def' == foo)
//@    "filterEmpty": "[filter(createArray(), lambda('foo', equals('def', lambdaVariables('foo'))))]",

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@    "sortNumeric": "[sort(createArray(8, 3, 10, -13, 5), lambda('x', 'y', less(lambdaVariables('x'), lambdaVariables('y'))))]",
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@    "sortAlpha": "[sort(createArray('ghi', 'abc', 'def'), lambda('x', 'y', less(lambdaVariables('x'), lambdaVariables('y'))))]",
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@    "sortAlphaReverse": "[sort(createArray('ghi', 'abc', 'def'), lambda('x', 'y', greater(lambdaVariables('x'), lambdaVariables('y'))))]",
var sortByObjectKey = sort([
//@    "sortByObjectKey": "[sort(createArray(createObject('key', 124, 'name', 'Second'), createObject('key', 298, 'name', 'Third'), createObject('key', 24, 'name', 'First'), createObject('key', 1232, 'name', 'Fourth')), lambda('x', 'y', less(int(lambdaVariables('x').key), int(lambdaVariables('y').key))))]",
  { key: 124, name: 'Second' }
  { key: 298, name: 'Third' }
  { key: 24, name: 'First' }
  { key: 1232, name: 'Fourth' }
], (x, y) => int(x.key) < int(y.key))
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@    "sortEmpty": "[sort(createArray(), lambda('x', 'y', less(int(lambdaVariables('x')), int(lambdaVariables('y')))))]",

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@    "reduceStringConcat": "[reduce(createArray('abc', 'def', 'ghi'), '', lambda('cur', 'next', concat(lambdaVariables('cur'), lambdaVariables('next'))))]",
var reduceStringConcatEven = reduce(['abc', 'def', 'ghi'], '', (cur, next, i) => isEven(i) ? concat(cur, next) : cur)
//@    "reduceStringConcatEven": "[reduce(createArray('abc', 'def', 'ghi'), '', lambda('cur', 'next', 'i', if(__bicep.isEven(lambdaVariables('i')), concat(lambdaVariables('cur'), lambdaVariables('next')), lambdaVariables('cur'))))]",
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@    "reduceFactorial": "[reduce(range(1, 5), 1, lambda('cur', 'next', mul(lambdaVariables('cur'), lambdaVariables('next'))))]",
var reduceObjectUnion = reduce([
//@    "reduceObjectUnion": "[reduce(createArray(createObject('foo', 123), createObject('bar', 456), createObject('baz', 789)), createObject(), lambda('cur', 'next', union(lambdaVariables('cur'), lambdaVariables('next'))))]",
  { foo: 123 }
  { bar: 456 }
  { baz: 789 }
], {}, (cur, next) => union(cur, next))
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@    "reduceEmpty": "[reduce(createArray(), 0, lambda('cur', 'next', lambdaVariables('cur')))]",

var itemForLoop = [for item in range(0, 10): item]
//@      {
//@        "name": "itemForLoop",
//@        "count": "[length(range(0, 10))]",
//@        "input": "[range(0, 10)[copyIndex('itemForLoop')]]"
//@      }
var filteredLoop = filter(itemForLoop, i => i > 5)
//@    "filteredLoop": "[filter(variables('itemForLoop'), lambda('i', greater(lambdaVariables('i'), 5)))]",

output doggoGreetings array = [for item in mapObject: item.greeting]
//@    "doggoGreetings": {
//@      "type": "array",
//@      "copy": {
//@        "count": "[length(variables('mapObject'))]",
//@        "input": "[variables('mapObject')[copyIndex()].greeting]"
//@      }
//@    }

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
//@    "storageAcc": {
//@      "existing": true,
//@      "type": "Microsoft.Storage/storageAccounts",
//@      "apiVersion": "2021-09-01",
//@      "name": "asdfsadf"
//@    },
  name: 'asdfsadf'
}
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)

module myMod './test.bicep' = {
//@    "myMod": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "3700372143120853182"
//@            }
//@          },
//@          "parameters": {
//@            "outputThis": {
//@              "type": "array"
//@            }
//@          },
//@          "resources": [],
//@          "outputs": {
//@            "outputThis": {
//@              "type": "array",
//@              "value": "[parameters('outputThis')]"
//@            }
//@          }
//@        }
//@      }
//@    }
  name: 'asdfsadf'
//@      "name": "asdfsadf",
  params: {
//@        "parameters": {
//@        },
    outputThis: map(mapObject, obj => obj.doggo)
//@          "outputThis": {
//@            "value": "[map(variables('mapObject'), lambda('obj', lambdaVariables('obj').doggo))]"
//@          }
  }
}
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')

var parentheses = map([123], (i => '${i}'))
//@    "parentheses": "[map(createArray(123), lambda('i', format('{0}', lambdaVariables('i'))))]",

var objectMap = toObject([123, 456, 789], i => '${i / 100}')
//@    "objectMap": "[toObject(createArray(123, 456, 789), lambda('i', format('{0}', div(lambdaVariables('i'), 100))))]",
var objectMap2 = toObject(range(0, 10), i => '${i}', i => {
//@    "objectMap2": "[toObject(range(0, 10), lambda('i', format('{0}', lambdaVariables('i'))), lambda('i', createObject('isEven', equals(mod(lambdaVariables('i'), 2), 0), 'isGreaterThan4', greater(lambdaVariables('i'), 4))))]",
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@    "objectMap3": "[toObject(variables('sortByObjectKey'), lambda('x', lambdaVariables('x').name))]",
var objectMap4 = toObject(sortByObjectKey, x =>
//@    "objectMap4": "[toObject(variables('sortByObjectKey'), lambda('x', lambdaVariables('x').name))]",
  
  x.name)
var objectMap5 = toObject(sortByObjectKey, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx => xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.name)
//@    "objectMap5": "[toObject(variables('sortByObjectKey'), lambda('xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx', lambdaVariables('xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx').name))]",
var objectMap6 = toObject(range(0, 10), i => '${i}', i => // comment
//@    "objectMap6": "[toObject(range(0, 10), lambda('i', format('{0}', lambdaVariables('i'))), lambda('i', createObject('isEven', equals(mod(lambdaVariables('i'), 2), 0), 'isGreaterThan4', greater(lambdaVariables('i'), 4))))]",
{
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})

var multiLine = reduce(['abc', 'def', 'ghi'], '', (
//@    "multiLine": "[reduce(createArray('abc', 'def', 'ghi'), '', lambda('cur', 'next', concat(lambdaVariables('cur'), lambdaVariables('next'))))]",
  cur,
  next
) => concat(cur, next))

var multiLineWithComment = reduce(['abc', 'def', 'ghi'], '', (
//@    "multiLineWithComment": "[reduce(createArray('abc', 'def', 'ghi'), '', lambda('cur', 'next', concat(lambdaVariables('cur'), lambdaVariables('next'))))]",
  // comment
  cur,
  next
) => concat(cur, next))

var mapVals = mapValues({
//@    "mapVals": "[mapValues(createObject('a', 123, 'b', 456), lambda('val', mul(lambdaVariables('val'), 2)))]",
  a: 123
  b: 456
}, val => val * 2)

var objectKeysTest = objectKeys({
//@    "objectKeysTest": "[objectKeys(createObject('a', 123, 'b', 456))]",
  a: 123
  b: 456
})

var shallowMergeTest = shallowMerge([{
//@    "shallowMergeTest": "[shallowMerge(createArray(createObject('a', 123), createObject('b', 456)))]",
  a: 123
}, {
  b: 456
}])

var groupByTest = groupBy([
//@    "groupByTest": "[groupBy(createArray(createObject('type', 'a', 'value', 123), createObject('type', 'b', 'value', 456), createObject('type', 'a', 'value', 789)), lambda('arg', lambdaVariables('arg').type))]",
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type)

var groupByWithValMapTest = groupBy([
//@    "groupByWithValMapTest": "[groupBy(createArray(createObject('type', 'a', 'value', 123), createObject('type', 'b', 'value', 456), createObject('type', 'a', 'value', 789)), lambda('arg', lambdaVariables('arg').type), lambda('arg', lambdaVariables('arg').value))]"
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type, arg => arg.value)

