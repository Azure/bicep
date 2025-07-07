param ids array
//@[10:15) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|

var flatten1 = flatten('abc')
//@[04:12) [no-unused-vars (Warning)] Variable "flatten1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |flatten1|
//@[23:28) [BCP070 (Error)] Argument of type "'abc'" is not assignable to parameter of type "array[]". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'abc'|
var flatten2 = flatten(['abc'], 'def')
//@[04:12) [no-unused-vars (Warning)] Variable "flatten2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |flatten2|
//@[22:38) [BCP071 (Error)] Expected 1 argument, but got 2. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |(['abc'], 'def')|

var map1 = map('abc')
//@[04:08) [no-unused-vars (Warning)] Variable "map1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |map1|
//@[14:21) [BCP071 (Error)] Expected 2 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('abc')|
var map2 = map('abc', 'def')
//@[04:08) [no-unused-vars (Warning)] Variable "map2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |map2|
//@[15:20) [BCP070 (Error)] Argument of type "'abc'" is not assignable to parameter of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'abc'|
var map3 = map(range(0, 10), 'def')
//@[04:08) [no-unused-vars (Warning)] Variable "map3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |map3|
//@[29:34) [BCP070 (Error)] Argument of type "'def'" is not assignable to parameter of type "(any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'def'|
var map4 = map(range(0, 10), () => null)
//@[04:08) [no-unused-vars (Warning)] Variable "map4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |map4|
//@[29:39) [BCP070 (Error)] Argument of type "() => null" is not assignable to parameter of type "(any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |() => null|
var map5 = map(range(0, 10), (a, b, c) => a)
//@[04:08) [no-unused-vars (Warning)] Variable "map5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |map5|
//@[29:43) [BCP070 (Error)] Argument of type "(int, int, any) => int" is not assignable to parameter of type "(any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |(a, b, c) => a|

var filter1 = filter('abc')
//@[04:11) [no-unused-vars (Warning)] Variable "filter1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filter1|
//@[20:27) [BCP071 (Error)] Expected 2 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('abc')|
var filter2 = filter('abc', 'def')
//@[04:11) [no-unused-vars (Warning)] Variable "filter2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filter2|
//@[21:26) [BCP070 (Error)] Argument of type "'abc'" is not assignable to parameter of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'abc'|
var filter3 = filter(range(0, 10), 'def')
//@[04:11) [no-unused-vars (Warning)] Variable "filter3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filter3|
//@[35:40) [BCP070 (Error)] Argument of type "'def'" is not assignable to parameter of type "(any[, int]) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'def'|
var filter4 = filter(range(0, 10), () => null)
//@[04:11) [no-unused-vars (Warning)] Variable "filter4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filter4|
//@[35:45) [BCP070 (Error)] Argument of type "() => null" is not assignable to parameter of type "(any[, int]) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |() => null|
var filter5 = filter(range(0, 10), i => i)
//@[04:11) [no-unused-vars (Warning)] Variable "filter5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filter5|
//@[35:41) [BCP070 (Error)] Argument of type "int => int" is not assignable to parameter of type "(any[, int]) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => i|
var filter6 = filter([true, 'hello!'], i => i)
//@[04:11) [no-unused-vars (Warning)] Variable "filter6" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filter6|
//@[39:45) [BCP070 (Error)] Argument of type "('hello!' | true) => ('hello!' | true)" is not assignable to parameter of type "(any[, int]) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => i|
var filter7 = filter(range(0, 10), (a, b, c) => true)
//@[04:11) [no-unused-vars (Warning)] Variable "filter7" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |filter7|
//@[35:52) [BCP070 (Error)] Argument of type "(int, int, any) => true" is not assignable to parameter of type "(any[, int]) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |(a, b, c) => true|

var sort1 = sort('abc')
//@[04:09) [no-unused-vars (Warning)] Variable "sort1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sort1|
//@[16:23) [BCP071 (Error)] Expected 2 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('abc')|
var sort2 = sort('abc', 'def')
//@[04:09) [no-unused-vars (Warning)] Variable "sort2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sort2|
//@[17:22) [BCP070 (Error)] Argument of type "'abc'" is not assignable to parameter of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'abc'|
var sort3 = sort(range(0, 10), 'def')
//@[04:09) [no-unused-vars (Warning)] Variable "sort3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sort3|
//@[31:36) [BCP070 (Error)] Argument of type "'def'" is not assignable to parameter of type "(any, any) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'def'|
var sort4 = sort(range(0, 10), () => null)
//@[04:09) [no-unused-vars (Warning)] Variable "sort4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sort4|
//@[31:41) [BCP070 (Error)] Argument of type "() => null" is not assignable to parameter of type "(any, any) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |() => null|
var sort5 = sort(range(0, 10), i => i)
//@[04:09) [no-unused-vars (Warning)] Variable "sort5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sort5|
//@[31:37) [BCP070 (Error)] Argument of type "int => int" is not assignable to parameter of type "(any, any) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => i|
var sort6 = sort(range(0, 10), (i, j) => i)
//@[04:09) [no-unused-vars (Warning)] Variable "sort6" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sort6|
//@[31:42) [BCP070 (Error)] Argument of type "(int, int) => int" is not assignable to parameter of type "(any, any) => bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |(i, j) => i|

var reduce1 = reduce('abc')
//@[04:11) [no-unused-vars (Warning)] Variable "reduce1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduce1|
//@[20:27) [BCP071 (Error)] Expected 3 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('abc')|
var reduce2 = reduce('abc', 'def', 'ghi')
//@[04:11) [no-unused-vars (Warning)] Variable "reduce2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduce2|
//@[21:26) [BCP070 (Error)] Argument of type "'abc'" is not assignable to parameter of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'abc'|
var reduce3 = reduce(range(0, 10), 0, 'def')
//@[04:11) [no-unused-vars (Warning)] Variable "reduce3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduce3|
//@[38:43) [BCP070 (Error)] Argument of type "'def'" is not assignable to parameter of type "(any, any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'def'|
var reduce4 = reduce(range(0, 10), 0, () => null)
//@[04:11) [no-unused-vars (Warning)] Variable "reduce4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduce4|
//@[38:48) [BCP070 (Error)] Argument of type "() => null" is not assignable to parameter of type "(any, any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |() => null|
var reduce5 = reduce(range(0, 10), 0, i => i)
//@[04:11) [no-unused-vars (Warning)] Variable "reduce5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduce5|
//@[38:44) [BCP070 (Error)] Argument of type "int => int" is not assignable to parameter of type "(any, any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => i|
var reduce6 = reduce(range(0, 10), 0, (a, b, c, d) => a)
//@[04:11) [no-unused-vars (Warning)] Variable "reduce6" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |reduce6|
//@[38:55) [BCP070 (Error)] Argument of type "(int, int, int, any) => int" is not assignable to parameter of type "(any, any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |(a, b, c, d) => a|

var toObject1 = toObject('abc')
//@[04:13) [no-unused-vars (Warning)] Variable "toObject1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |toObject1|
//@[24:31) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('abc')|
var toObject2 = toObject('abc', 'def')
//@[04:13) [no-unused-vars (Warning)] Variable "toObject2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |toObject2|
//@[25:30) [BCP070 (Error)] Argument of type "'abc'" is not assignable to parameter of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'abc'|
var toObject3 = toObject(range(0, 10), 'def')
//@[04:13) [no-unused-vars (Warning)] Variable "toObject3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |toObject3|
//@[39:44) [BCP070 (Error)] Argument of type "'def'" is not assignable to parameter of type "any => string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'def'|
var toObject4 = toObject(range(0, 10), () => null)
//@[04:13) [no-unused-vars (Warning)] Variable "toObject4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |toObject4|
//@[39:49) [BCP070 (Error)] Argument of type "() => null" is not assignable to parameter of type "any => string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |() => null|
var toObject5 = toObject(range(0, 10), i => i)
//@[04:13) [no-unused-vars (Warning)] Variable "toObject5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |toObject5|
//@[39:45) [BCP070 (Error)] Argument of type "int => int" is not assignable to parameter of type "any => string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => i|
var toObject6 = toObject(range(0, 10), i => '${i}', 'def')
//@[04:13) [no-unused-vars (Warning)] Variable "toObject6" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |toObject6|
//@[52:57) [BCP070 (Error)] Argument of type "'def'" is not assignable to parameter of type "any => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'def'|
var toObject7 = toObject(range(0, 10), i => '${i}', () => null)
//@[04:13) [no-unused-vars (Warning)] Variable "toObject7" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |toObject7|
//@[52:62) [BCP070 (Error)] Argument of type "() => null" is not assignable to parameter of type "any => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |() => null|

var ternary = map([123], true ? i => '${i}' : i => 'hello!')
//@[04:11) [no-unused-vars (Warning)] Variable "ternary" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |ternary|
//@[32:43) [BCP242 (Error)] Lambda functions may only be specified directly as function arguments. (bicep https://aka.ms/bicep/core-diagnostics#BCP242) |i => '${i}'|
//@[46:59) [BCP242 (Error)] Lambda functions may only be specified directly as function arguments. (bicep https://aka.ms/bicep/core-diagnostics#BCP242) |i => 'hello!'|

var outsideArgs = i => 123
//@[04:15) [no-unused-vars (Warning)] Variable "outsideArgs" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |outsideArgs|
//@[18:26) [BCP242 (Error)] Lambda functions may only be specified directly as function arguments. (bicep https://aka.ms/bicep/core-diagnostics#BCP242) |i => 123|
var outsideArgs2 = (x, y, z) => '${x}${y}${z}'
//@[04:16) [no-unused-vars (Warning)] Variable "outsideArgs2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |outsideArgs2|
//@[19:46) [BCP242 (Error)] Lambda functions may only be specified directly as function arguments. (bicep https://aka.ms/bicep/core-diagnostics#BCP242) |(x, y, z) => '${x}${y}${z}'|
var partial = i =>
//@[04:11) [no-unused-vars (Warning)] Variable "partial" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |partial|
//@[14:18) [BCP242 (Error)] Lambda functions may only be specified directly as function arguments. (bicep https://aka.ms/bicep/core-diagnostics#BCP242) |i =>|
//@[18:18) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||


var inObject = {
//@[04:12) [no-unused-vars (Warning)] Variable "inObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |inObject|
  a: i => i
//@[05:11) [BCP242 (Error)] Lambda functions may only be specified directly as function arguments. (bicep https://aka.ms/bicep/core-diagnostics#BCP242) |i => i|
}

var inArray = [
//@[04:11) [no-unused-vars (Warning)] Variable "inArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |inArray|
  i => i
//@[02:08) [BCP242 (Error)] Lambda functions may only be specified directly as function arguments. (bicep https://aka.ms/bicep/core-diagnostics#BCP242) |i => i|
  j => j
//@[02:08) [BCP242 (Error)] Lambda functions may only be specified directly as function arguments. (bicep https://aka.ms/bicep/core-diagnostics#BCP242) |j => j|
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
//@[15:20) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[49:50) [BCP247 (Error)] Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: "i". (bicep https://aka.ms/bicep/core-diagnostics#BCP247) |i|
output stgKeys2 array = map(range(0, 2), j => stg[((j + 2) % 123)].listKeys().keys[0].value)
//@[16:21) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[50:65) [BCP247 (Error)] Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: "j". (bicep https://aka.ms/bicep/core-diagnostics#BCP247) |((j + 2) % 123)|
output stgKeys3 array = map(ids, id => listKeys(id, stg[0].apiVersion).keys[0].value)
//@[16:21) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[39:70) [outputs-should-not-contain-secrets (Warning)] Outputs should not contain secrets. Found possible secret: function 'listKeys' (bicep core linter https://aka.ms/bicep/linter-diagnostics#outputs-should-not-contain-secrets) |listKeys(id, stg[0].apiVersion)|
//@[39:70) [BCP248 (Error)] Using lambda variables inside the "listKeys" function is not currently supported. Found the following lambda variable(s) being accessed: "id". (bicep https://aka.ms/bicep/core-diagnostics#BCP248) |listKeys(id, stg[0].apiVersion)|
output accessTiers array = map(range(0, 2), k => stg[k].properties.accessTier)
//@[19:24) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[53:54) [BCP247 (Error)] Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: "k". (bicep https://aka.ms/bicep/core-diagnostics#BCP247) |k|
output accessTiers2 array = map(range(0, 2), x => map(range(0, 2), y => stg[x / y].properties.accessTier))
//@[20:25) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[76:81) [BCP247 (Error)] Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: "x", "y". (bicep https://aka.ms/bicep/core-diagnostics#BCP247) |x / y|
output accessTiers3 array = map(ids, foo => reference('${foo}').accessTier)
//@[20:25) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[44:63) [BCP248 (Error)] Using lambda variables inside the "reference" function is not currently supported. Found the following lambda variable(s) being accessed: "foo". (bicep https://aka.ms/bicep/core-diagnostics#BCP248) |reference('${foo}')|

module modLoop './empty.bicep' = [for item in range(0, 5): {
  name: 'foo${item}'
}]

var modLoopNames = map(modLoop, i => i.name)
//@[04:16) [no-unused-vars (Warning)] Variable "modLoopNames" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |modLoopNames|
//@[23:30) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP144) |modLoop|
output modOutputs array = map(range(0, 5), i => modLoop[i].outputs.foo)
//@[18:23) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[43:70) [BCP070 (Error)] Argument of type "int => error" is not assignable to parameter of type "(any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |i => modLoop[i].outputs.foo|
//@[56:57) [BCP247 (Error)] Using lambda variables inside resource or module array access is not currently supported. Found the following lambda variable(s) being accessed: "i". (bicep https://aka.ms/bicep/core-diagnostics#BCP247) |i|
//@[67:70) [BCP052 (Error)] The type "outputs" does not contain property "foo". (bicep https://aka.ms/bicep/core-diagnostics#BCP052) |foo|

var onlyComma = map([0], (,) => 'foo')
//@[04:13) [no-unused-vars (Warning)] Variable "onlyComma" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |onlyComma|
//@[25:37) [BCP070 (Error)] Argument of type "() => 'foo'" is not assignable to parameter of type "(any[, int]) => any". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |(,) => 'foo'|
//@[26:27) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
var trailingCommas = map([0], (a,,) => 'foo')
//@[04:18) [no-unused-vars (Warning)] Variable "trailingCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |trailingCommas|
//@[33:34) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
var multiLineOnly = map([0], (
//@[04:17) [no-unused-vars (Warning)] Variable "multiLineOnly" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineOnly|
//@[30:35) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) |\n  a\n|
  a
  b) => 'foo')
//@[02:03) [BCP018 (Error)] Expected the ")" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |b|
)

var multiLineTrailingCommas = map([0], (
//@[04:27) [no-unused-vars (Warning)] Variable "multiLineTrailingCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineTrailingCommas|
  a,
  ,) => 'foo')
//@[02:03) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|

var lineBeforeComma = map([0], (
//@[04:19) [no-unused-vars (Warning)] Variable "lineBeforeComma" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |lineBeforeComma|
  a
  ,b) => 'foo')

