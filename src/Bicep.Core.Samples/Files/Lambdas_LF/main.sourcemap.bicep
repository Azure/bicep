var doggos = [
//@[line00->line018]     "doggos": [
//@[line00->line023]     ],
  'Evie'
//@[line01->line019]       "Evie",
  'Casper'
//@[line02->line020]       "Casper",
  'Indy'
//@[line03->line021]       "Indy",
  'Kira'
//@[line04->line022]       "Kira"
]

var numbers = range(0, 4)
//@[line07->line024]     "numbers": "[range(0, 4)]",

var sayHello = map(doggos, i => 'Hello ${i}!')
//@[line09->line025]     "sayHello": "[map(variables('doggos'), lambda('i', format('Hello {0}!', lambdaVariables('i'))))]",

var isEven = filter(numbers, i => 0 == i % 2)
//@[line11->line026]     "isEven": "[filter(variables('numbers'), lambda('i', equals(0, mod(lambdaVariables('i'), 2))))]",

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])
//@[line13->line027]     "evenDoggosNestedLambdas": "[map(filter(variables('numbers'), lambda('i', contains(filter(variables('numbers'), lambda('j', equals(0, mod(lambdaVariables('j'), 2)))), lambdaVariables('i')))), lambda('x', variables('doggos')[lambdaVariables('x')]))]",

var flattenedArrayOfArrays = flatten([[0, 1], [2, 3], [4, 5]])
//@[line15->line028]     "flattenedArrayOfArrays": "[flatten(createArray(createArray(0, 1), createArray(2, 3), createArray(4, 5)))]",
var flattenedEmptyArray = flatten([])
//@[line16->line029]     "flattenedEmptyArray": "[flatten(createArray())]",

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
//@[line18->line030]     "mapSayHi": "[map(createArray('abc', 'def', 'ghi'), lambda('foo', format('Hi {0}!', lambdaVariables('foo'))))]",
var mapEmpty = map([], foo => 'Hi ${foo}!')
//@[line19->line031]     "mapEmpty": "[map(createArray(), lambda('foo', format('Hi {0}!', lambdaVariables('foo'))))]",
var mapObject = map(range(0, length(doggos)), i => {
//@[line20->line032]     "mapObject": "[map(range(0, length(variables('doggos'))), lambda('i', createObject('i', lambdaVariables('i'), 'doggo', variables('doggos')[lambdaVariables('i')], 'greeting', format('Ahoy, {0}!', variables('doggos')[lambdaVariables('i')]))))]",
  i: i
  doggo: doggos[i]
  greeting: 'Ahoy, ${doggos[i]}!'
})
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
//@[line25->line033]     "mapArray": "[flatten(map(range(1, 3), lambda('i', createArray(mul(lambdaVariables('i'), 2), add(mul(lambdaVariables('i'), 2), 1)))))]",
var mapMultiLineArray = flatten(map(range(1, 3), i => [
//@[line26->line034]     "mapMultiLineArray": "[flatten(map(range(1, 3), lambda('i', createArray(mul(lambdaVariables('i'), 3), add(mul(lambdaVariables('i'), 3), 1), add(mul(lambdaVariables('i'), 3), 2)))))]",
  i * 3
  (i * 3) + 1
  (i * 3) + 2
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
//@[line32->line035]     "filterEqualityCheck": "[filter(createArray('abc', 'def', 'ghi'), lambda('foo', equals('def', lambdaVariables('foo'))))]",
var filterEmpty = filter([], foo => 'def' == foo)
//@[line33->line036]     "filterEmpty": "[filter(createArray(), lambda('foo', equals('def', lambdaVariables('foo'))))]",

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
//@[line35->line037]     "sortNumeric": "[sort(createArray(8, 3, 10, -13, 5), lambda('x', 'y', less(lambdaVariables('x'), lambdaVariables('y'))))]",
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
//@[line36->line038]     "sortAlpha": "[sort(createArray('ghi', 'abc', 'def'), lambda('x', 'y', less(lambdaVariables('x'), lambdaVariables('y'))))]",
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
//@[line37->line039]     "sortAlphaReverse": "[sort(createArray('ghi', 'abc', 'def'), lambda('x', 'y', greater(lambdaVariables('x'), lambdaVariables('y'))))]",
var sortByObjectKey = sort([
//@[line38->line040]     "sortByObjectKey": "[sort(createArray(createObject('key', 124, 'name', 'Second'), createObject('key', 298, 'name', 'Third'), createObject('key', 24, 'name', 'First'), createObject('key', 1232, 'name', 'Fourth')), lambda('x', 'y', less(int(lambdaVariables('x').key), int(lambdaVariables('y').key))))]",
  { key: 124, name: 'Second' }
  { key: 298, name: 'Third' }
  { key: 24, name: 'First' }
  { key: 1232, name: 'Fourth' }
], (x, y) => int(x.key) < int(y.key))
var sortEmpty = sort([], (x, y) => int(x) < int(y))
//@[line44->line041]     "sortEmpty": "[sort(createArray(), lambda('x', 'y', less(int(lambdaVariables('x')), int(lambdaVariables('y')))))]",

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
//@[line46->line042]     "reduceStringConcat": "[reduce(createArray('abc', 'def', 'ghi'), '', lambda('cur', 'next', concat(lambdaVariables('cur'), lambdaVariables('next'))))]",
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
//@[line47->line043]     "reduceFactorial": "[reduce(range(1, 5), 1, lambda('cur', 'next', mul(lambdaVariables('cur'), lambdaVariables('next'))))]",
var reduceObjectUnion = reduce([
//@[line48->line044]     "reduceObjectUnion": "[reduce(createArray(createObject('foo', 123), createObject('bar', 456), createObject('baz', 789)), createObject(), lambda('cur', 'next', union(lambdaVariables('cur'), lambdaVariables('next'))))]",
  { foo: 123 }
  { bar: 456 }
  { baz: 789 }
], {}, (cur, next) => union(cur, next))
var reduceEmpty = reduce([], 0, (cur, next) => cur)
//@[line53->line045]     "reduceEmpty": "[reduce(createArray(), 0, lambda('cur', 'next', lambdaVariables('cur')))]",

var itemForLoop = [for item in range(0, 10): item]
//@[line55->line012]       {
//@[line55->line013]         "name": "itemForLoop",
//@[line55->line014]         "count": "[length(range(0, 10))]",
//@[line55->line015]         "input": "[range(0, 10)[copyIndex('itemForLoop')]]"
//@[line55->line016]       }
var filteredLoop = filter(itemForLoop, i => i > 5)
//@[line56->line046]     "filteredLoop": "[filter(variables('itemForLoop'), lambda('i', greater(lambdaVariables('i'), 5)))]",

output doggoGreetings array = [for item in mapObject: item.greeting]
//@[line58->line094]     "doggoGreetings": {
//@[line58->line095]       "type": "array",
//@[line58->line096]       "copy": {
//@[line58->line097]         "count": "[length(variables('mapObject'))]",
//@[line58->line098]         "input": "[variables('mapObject')[copyIndex()].greeting]"
//@[line58->line099]       }
//@[line58->line100]     }

resource storageAcc 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: 'asdfsadf'
}
var mappedResProps = map(items(storageAcc.properties.secondaryEndpoints), item => item.value)

module myMod './test.bicep' = {
//@[line65->line053]     {
//@[line65->line054]       "type": "Microsoft.Resources/deployments",
//@[line65->line055]       "apiVersion": "2020-10-01",
//@[line65->line057]       "properties": {
//@[line65->line058]         "expressionEvaluationOptions": {
//@[line65->line059]           "scope": "inner"
//@[line65->line060]         },
//@[line65->line061]         "mode": "Incremental",
//@[line65->line062]         "parameters": {
//@[line65->line063]           "outputThis": {
//@[line65->line065]           }
//@[line65->line066]         },
//@[line65->line067]         "template": {
//@[line65->line068]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line65->line069]           "contentVersion": "1.0.0.0",
//@[line65->line070]           "metadata": {
//@[line65->line071]             "_generator": {
//@[line65->line072]               "name": "bicep",
//@[line65->line073]               "version": "dev",
//@[line65->line074]               "templateHash": "3700372143120853182"
//@[line65->line075]             }
//@[line65->line076]           },
//@[line65->line077]           "parameters": {
//@[line65->line078]             "outputThis": {
//@[line65->line079]               "type": "array"
//@[line65->line080]             }
//@[line65->line081]           },
//@[line65->line082]           "resources": [],
//@[line65->line083]           "outputs": {
//@[line65->line084]             "outputThis": {
//@[line65->line085]               "type": "array",
//@[line65->line086]               "value": "[parameters('outputThis')]"
//@[line65->line087]             }
//@[line65->line088]           }
//@[line65->line089]         }
//@[line65->line090]       }
//@[line65->line091]     }
  name: 'asdfsadf'
//@[line66->line056]       "name": "asdfsadf",
  params: {
    outputThis: map(mapObject, obj => obj.doggo)
//@[line68->line064]             "value": "[map(variables('mapObject'), lambda('obj', lambdaVariables('obj').doggo))]"
  }
}
var mappedModOutputProps = map(myMod.outputs.outputThis, doggo => '${doggo} says bork')

var parentheses = map([123], (i => '${i}'))
//@[line73->line047]     "parentheses": "[map(createArray(123), lambda('i', format('{0}', lambdaVariables('i'))))]",

var objectMap = toObject([123, 456, 789], i => '${i / 100}')
//@[line75->line048]     "objectMap": "[toObject(createArray(123, 456, 789), lambda('i', format('{0}', div(lambdaVariables('i'), 100))))]",
var objectMap2 = toObject(range(0, 10), i => '${i}', i => {
//@[line76->line049]     "objectMap2": "[toObject(range(0, 10), lambda('i', format('{0}', lambdaVariables('i'))), lambda('i', createObject('isEven', equals(mod(lambdaVariables('i'), 2), 0), 'isGreaterThan4', greater(lambdaVariables('i'), 4))))]",
  isEven: (i % 2) == 0
  isGreaterThan4: (i > 4)
})
var objectMap3 = toObject(sortByObjectKey, x => x.name)
//@[line80->line050]     "objectMap3": "[toObject(variables('sortByObjectKey'), lambda('x', lambdaVariables('x').name))]"

