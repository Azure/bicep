param ids array

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

resource stg 'Microsoft.Storage/storageAccounts@2021-09-01' = [for i in range(0, 2): {
  name: 'antteststg${i}'
  location: 'West US'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]

output stgKeys array = map(range(0, 2), i => stg[i].listKeys().keys[0].value)
output stgKeys2 array = map(range(0, 2), j => stg[((j + 2) % 123)].listKeys().keys[0].value)
output stgKeys3 array = map(ids, id => listKeys(id, stg[0].apiVersion).keys[0].value)
output accessTiers array = map(range(0, 2), k => stg[k].properties.accessTier)
output accessTiers2 array = map(range(0, 2), x => map(range(0, 2), y => stg[x / y].properties.accessTier))
output accessTiers3 array = map(ids, foo => reference('${foo}').accessTier)

module modLoop './empty.bicep' = [for item in range(0, 5): {
  name: 'foo${item}'
}]

var modLoopNames = map(modLoop, i => i.name)
output modOutputs array = map(range(0, 5), i => modLoop[i].outputs.foo)
