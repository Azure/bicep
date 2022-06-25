var flatten1 = flatten('abc')
var flatten2 = flatten(['abc'], 'def')

var map1 = map('abc')
var map2 = map('abc', 'def')
var map3 = map(range(0, 10), 'def')
var map4 = map(range(0, 10), () => null)

var filter1 = filter('abc')
var filter2 = filter('abc', 'def')
var filter3 = filter(range(0, 10), 'def')
var filter4 = filter(range(0, 10), () => null)
var filter5 = filter(range(0, 10), i => i)
var filter6 = filter([true, 'hello!'], i => i)

var sort1 = sort('abc')
var sort2 = sort('abc', 'def')
var sort3 = sort(range(0, 10), 'def')
var sort4 = sort(range(0, 10), () => null)
var sort5 = sort(range(0, 10), i => i)
var sort6 = sort(range(0, 10), (i, j) => i)

var reduce1 = reduce('abc')
var reduce2 = reduce('abc', 'def', 'ghi')
var reduce3 = reduce(range(0, 10), 0, 'def')
var reduce4 = reduce(range(0, 10), 0, () => null)
var reduce5 = reduce(range(0, 10), 0, i => i)

var ternary = map([123], true ? i => '${i}' : i => 'hello!')

var outsideArgs = i => 123
var outsideArgs2 = (x, y, z) => '${x}${y}${z}'
var partial = i =>


var inObject = {
  a: i => i
}

var inArray = [
  i => i
  j => j
]

resource resLoop 'Microsoft.Storage/storageAccounts@2021-09-01' existing = [for item in range(0, 5): {
  name: 'foo${item}'
}]
var resLoopNames = map(resLoop, i => i.name)

module modLoop './empty.bicep' = [for item in range(0, 5): {
  name: 'foo${item}'
}]
var modLoopNames = map(modLoop, i => i.name)
