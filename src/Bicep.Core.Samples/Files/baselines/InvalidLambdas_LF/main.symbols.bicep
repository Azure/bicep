param ids array
//@[06:09) Parameter ids. Type: array. Declaration start char: 0, length: 15

var flatten1 = flatten('abc')
//@[04:12) Variable flatten1. Type: error. Declaration start char: 0, length: 29
var flatten2 = flatten(['abc'], 'def')
//@[04:12) Variable flatten2. Type: error. Declaration start char: 0, length: 38

var map1 = map('abc')
//@[04:08) Variable map1. Type: error. Declaration start char: 0, length: 21
var map2 = map('abc', 'def')
//@[04:08) Variable map2. Type: error. Declaration start char: 0, length: 28
var map3 = map(range(0, 10), 'def')
//@[04:08) Variable map3. Type: error. Declaration start char: 0, length: 35
var map4 = map(range(0, 10), () => null)
//@[04:08) Variable map4. Type: error. Declaration start char: 0, length: 40
var map5 = map(range(0, 10), (a, b, c) => a)
//@[30:31) Local a. Type: int. Declaration start char: 30, length: 1
//@[33:34) Local b. Type: int. Declaration start char: 33, length: 1
//@[36:37) Local c. Type: any. Declaration start char: 36, length: 1
//@[04:08) Variable map5. Type: error. Declaration start char: 0, length: 44

var filter1 = filter('abc')
//@[04:11) Variable filter1. Type: error. Declaration start char: 0, length: 27
var filter2 = filter('abc', 'def')
//@[04:11) Variable filter2. Type: error. Declaration start char: 0, length: 34
var filter3 = filter(range(0, 10), 'def')
//@[04:11) Variable filter3. Type: error. Declaration start char: 0, length: 41
var filter4 = filter(range(0, 10), () => null)
//@[04:11) Variable filter4. Type: error. Declaration start char: 0, length: 46
var filter5 = filter(range(0, 10), i => i)
//@[35:36) Local i. Type: int. Declaration start char: 35, length: 1
//@[04:11) Variable filter5. Type: error. Declaration start char: 0, length: 42
var filter6 = filter([true, 'hello!'], i => i)
//@[39:40) Local i. Type: 'hello!' | true. Declaration start char: 39, length: 1
//@[04:11) Variable filter6. Type: error. Declaration start char: 0, length: 46
var filter7 = filter(range(0, 10), (a, b, c) => true)
//@[36:37) Local a. Type: int. Declaration start char: 36, length: 1
//@[39:40) Local b. Type: int. Declaration start char: 39, length: 1
//@[42:43) Local c. Type: any. Declaration start char: 42, length: 1
//@[04:11) Variable filter7. Type: error. Declaration start char: 0, length: 53

var sort1 = sort('abc')
//@[04:09) Variable sort1. Type: error. Declaration start char: 0, length: 23
var sort2 = sort('abc', 'def')
//@[04:09) Variable sort2. Type: error. Declaration start char: 0, length: 30
var sort3 = sort(range(0, 10), 'def')
//@[04:09) Variable sort3. Type: error. Declaration start char: 0, length: 37
var sort4 = sort(range(0, 10), () => null)
//@[04:09) Variable sort4. Type: error. Declaration start char: 0, length: 42
var sort5 = sort(range(0, 10), i => i)
//@[31:32) Local i. Type: int. Declaration start char: 31, length: 1
//@[04:09) Variable sort5. Type: error. Declaration start char: 0, length: 38
var sort6 = sort(range(0, 10), (i, j) => i)
//@[32:33) Local i. Type: int. Declaration start char: 32, length: 1
//@[35:36) Local j. Type: int. Declaration start char: 35, length: 1
//@[04:09) Variable sort6. Type: error. Declaration start char: 0, length: 43

var reduce1 = reduce('abc')
//@[04:11) Variable reduce1. Type: error. Declaration start char: 0, length: 27
var reduce2 = reduce('abc', 'def', 'ghi')
//@[04:11) Variable reduce2. Type: error. Declaration start char: 0, length: 41
var reduce3 = reduce(range(0, 10), 0, 'def')
//@[04:11) Variable reduce3. Type: error. Declaration start char: 0, length: 44
var reduce4 = reduce(range(0, 10), 0, () => null)
//@[04:11) Variable reduce4. Type: error. Declaration start char: 0, length: 49
var reduce5 = reduce(range(0, 10), 0, i => i)
//@[38:39) Local i. Type: int. Declaration start char: 38, length: 1
//@[04:11) Variable reduce5. Type: error. Declaration start char: 0, length: 45
var reduce6 = reduce(range(0, 10), 0, (a, b, c, d) => a)
//@[39:40) Local a. Type: int. Declaration start char: 39, length: 1
//@[42:43) Local b. Type: int. Declaration start char: 42, length: 1
//@[45:46) Local c. Type: int. Declaration start char: 45, length: 1
//@[48:49) Local d. Type: any. Declaration start char: 48, length: 1
//@[04:11) Variable reduce6. Type: error. Declaration start char: 0, length: 56

var toObject1 = toObject('abc')
//@[04:13) Variable toObject1. Type: error. Declaration start char: 0, length: 31
var toObject2 = toObject('abc', 'def')
//@[04:13) Variable toObject2. Type: error. Declaration start char: 0, length: 38
var toObject3 = toObject(range(0, 10), 'def')
//@[04:13) Variable toObject3. Type: error. Declaration start char: 0, length: 45
var toObject4 = toObject(range(0, 10), () => null)
//@[04:13) Variable toObject4. Type: error. Declaration start char: 0, length: 50
var toObject5 = toObject(range(0, 10), i => i)
//@[39:40) Local i. Type: int. Declaration start char: 39, length: 1
//@[04:13) Variable toObject5. Type: error. Declaration start char: 0, length: 46
var toObject6 = toObject(range(0, 10), i => '${i}', 'def')
//@[39:40) Local i. Type: int. Declaration start char: 39, length: 1
//@[04:13) Variable toObject6. Type: error. Declaration start char: 0, length: 58
var toObject7 = toObject(range(0, 10), i => '${i}', () => null)
//@[39:40) Local i. Type: int. Declaration start char: 39, length: 1
//@[04:13) Variable toObject7. Type: error. Declaration start char: 0, length: 63

var ternary = map([123], true ? i => '${i}' : i => 'hello!')
//@[32:33) Local i. Type: any. Declaration start char: 32, length: 1
//@[46:47) Local i. Type: any. Declaration start char: 46, length: 1
//@[04:11) Variable ternary. Type: string[]. Declaration start char: 0, length: 60

var outsideArgs = i => 123
//@[18:19) Local i. Type: any. Declaration start char: 18, length: 1
//@[04:15) Variable outsideArgs. Type: any => 123. Declaration start char: 0, length: 26
var outsideArgs2 = (x, y, z) => '${x}${y}${z}'
//@[20:21) Local x. Type: any. Declaration start char: 20, length: 1
//@[23:24) Local y. Type: any. Declaration start char: 23, length: 1
//@[26:27) Local z. Type: any. Declaration start char: 26, length: 1
//@[04:16) Variable outsideArgs2. Type: (any, any, any) => string. Declaration start char: 0, length: 46
var partial = i =>
//@[14:15) Local i. Type: any. Declaration start char: 14, length: 1
//@[04:11) Variable partial. Type: any => error. Declaration start char: 0, length: 18


var inObject = {
//@[04:12) Variable inObject. Type: object. Declaration start char: 0, length: 30
  a: i => i
//@[05:06) Local i. Type: any. Declaration start char: 5, length: 1
}

var inArray = [
//@[04:11) Variable inArray. Type: [any => any, any => any]. Declaration start char: 0, length: 35
  i => i
//@[02:03) Local i. Type: any. Declaration start char: 2, length: 1
  j => j
//@[02:03) Local j. Type: any. Declaration start char: 2, length: 1
]

resource stg 'Microsoft.Storage/storageAccounts@2021-09-01' = [for i in range(0, 2): {
//@[67:68) Local i. Type: int. Declaration start char: 67, length: 1
//@[09:12) Resource stg. Type: Microsoft.Storage/storageAccounts@2021-09-01[]. Declaration start char: 0, length: 194
  name: 'antteststg${i}'
  location: 'West US'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]

output stgKeys array = map(range(0, 2), i => stg[i].listKeys().keys[0].value)
//@[40:41) Local i. Type: int. Declaration start char: 40, length: 1
//@[07:14) Output stgKeys. Type: array. Declaration start char: 0, length: 77
output stgKeys2 array = map(range(0, 2), j => stg[((j + 2) % 123)].listKeys().keys[0].value)
//@[41:42) Local j. Type: int. Declaration start char: 41, length: 1
//@[07:15) Output stgKeys2. Type: array. Declaration start char: 0, length: 92
output stgKeys3 array = map(ids, id => listKeys(id, stg[0].apiVersion).keys[0].value)
//@[33:35) Local id. Type: any. Declaration start char: 33, length: 2
//@[07:15) Output stgKeys3. Type: array. Declaration start char: 0, length: 85
output accessTiers array = map(range(0, 2), k => stg[k].properties.accessTier)
//@[44:45) Local k. Type: int. Declaration start char: 44, length: 1
//@[07:18) Output accessTiers. Type: array. Declaration start char: 0, length: 78
output accessTiers2 array = map(range(0, 2), x => map(range(0, 2), y => stg[x / y].properties.accessTier))
//@[67:68) Local y. Type: int. Declaration start char: 67, length: 1
//@[45:46) Local x. Type: int. Declaration start char: 45, length: 1
//@[07:19) Output accessTiers2. Type: array. Declaration start char: 0, length: 106
output accessTiers3 array = map(ids, foo => reference('${foo}').accessTier)
//@[37:40) Local foo. Type: any. Declaration start char: 37, length: 3
//@[07:19) Output accessTiers3. Type: array. Declaration start char: 0, length: 75

module modLoop './empty.bicep' = [for item in range(0, 5): {
//@[38:42) Local item. Type: int. Declaration start char: 38, length: 4
//@[07:14) Module modLoop. Type: module[]. Declaration start char: 0, length: 84
  name: 'foo${item}'
}]

var modLoopNames = map(modLoop, i => i.name)
//@[32:33) Local i. Type: module. Declaration start char: 32, length: 1
//@[04:16) Variable modLoopNames. Type: string[]. Declaration start char: 0, length: 44
output modOutputs array = map(range(0, 5), i => modLoop[i].outputs.foo)
//@[43:44) Local i. Type: int. Declaration start char: 43, length: 1
//@[07:17) Output modOutputs. Type: array. Declaration start char: 0, length: 71

var onlyComma = map([0], (,) => 'foo')
//@[04:13) Variable onlyComma. Type: error. Declaration start char: 0, length: 38
var trailingCommas = map([0], (a,,) => 'foo')
//@[31:32) Local a. Type: 0. Declaration start char: 31, length: 1
//@[04:18) Variable trailingCommas. Type: 'foo'[]. Declaration start char: 0, length: 45
var multiLineOnly = map([0], (
//@[04:17) Variable multiLineOnly. Type: any. Declaration start char: 0, length: 51
  a
  b) => 'foo')
)

var multiLineTrailingCommas = map([0], (
//@[04:27) Variable multiLineTrailingCommas. Type: 'foo'[]. Declaration start char: 0, length: 60
  a,
//@[02:03) Local a. Type: 0. Declaration start char: 2, length: 1
  ,) => 'foo')

var lineBeforeComma = map([0], (
//@[04:19) Variable lineBeforeComma. Type: 'foo'[]. Declaration start char: 0, length: 52
  a
//@[02:03) Local a. Type: 0. Declaration start char: 2, length: 1
  ,b) => 'foo')
//@[03:04) Local b. Type: int. Declaration start char: 3, length: 1

