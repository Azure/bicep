
// unknown declaration
bad

// incomplete variable declaration #completionTest(0,1,2) -> declarations
var
//@[03:03) Variable <missing>. Type: error. Declaration start char: 0, length: 3

// missing identifier #completionTest(4) -> empty
var 
//@[04:04) Variable <missing>. Type: error. Declaration start char: 0, length: 4

// incomplete keyword
// #completionTest(0,1) -> declarations
v
// #completionTest(0,1,2) -> declarations
va

// unassigned variable
var foo
//@[04:07) Variable foo. Type: error. Declaration start char: 0, length: 7

// #completionTest(18,19) -> symbols
var missingValue = 
//@[04:16) Variable missingValue. Type: error. Declaration start char: 0, length: 19

// malformed identifier
var 2 
//@[04:05) Variable <error>. Type: error. Declaration start char: 0, length: 6
var $ = 23
//@[04:05) Variable <error>. Type: int. Declaration start char: 0, length: 10
var # 33 = 43
//@[04:08) Variable <error>. Type: int. Declaration start char: 0, length: 13

// no value assigned
var foo =
//@[04:07) Variable foo. Type: error. Declaration start char: 0, length: 9

// bad =
var badEquals 2
//@[04:13) Variable badEquals. Type: error. Declaration start char: 0, length: 15
var badEquals2 3 true
//@[04:14) Variable badEquals2. Type: error. Declaration start char: 0, length: 21

// malformed identifier but type check should happen regardless
var 2 = x
//@[04:05) Variable <error>. Type: error. Declaration start char: 0, length: 9

// bad token value
var foo = &
//@[04:07) Variable foo. Type: error. Declaration start char: 0, length: 11

// bad value
var foo = *
//@[04:07) Variable foo. Type: error. Declaration start char: 0, length: 11

// expressions
var bar = x
//@[04:07) Variable bar. Type: error. Declaration start char: 0, length: 11
var bar = foo()
//@[04:07) Variable bar. Type: error. Declaration start char: 0, length: 15
var x = 2 + !3
//@[04:05) Variable x. Type: error. Declaration start char: 0, length: 14
var y = false ? true + 1 : !4
//@[04:05) Variable y. Type: error. Declaration start char: 0, length: 29

// test for array item recovery
var x = [
//@[04:05) Variable x. Type: error. Declaration start char: 0, length: 31
  3 + 4
  =
  !null
]

// test for object property recovery
var y = {
//@[04:05) Variable y. Type: error. Declaration start char: 0, length: 25
  =
  foo: !2
}

// utcNow and newGuid used outside a param default value
var test = utcNow('u')
//@[04:08) Variable test. Type: error. Declaration start char: 0, length: 22
var test2 = newGuid()
//@[04:09) Variable test2. Type: error. Declaration start char: 0, length: 21

// bad string escape sequence in object key
var test3 = {
//@[04:09) Variable test3. Type: object. Declaration start char: 0, length: 36
  'bad\escape': true
}

// duplicate properties
var testDupe = {
//@[04:12) Variable testDupe. Type: object. Declaration start char: 0, length: 56
  'duplicate': true
  duplicate: true
}

// interpolation with type errors in key
var objWithInterp = {
//@[04:17) Variable objWithInterp. Type: error. Declaration start char: 0, length: 62
  'ab${nonExistentIdentifier}cd': true
}

// invalid fully qualified function access
var mySum = az.add(1,2)
//@[04:09) Variable mySum. Type: error. Declaration start char: 0, length: 23
var myConcat = sys.concat('a', az.concat('b', 'c'))
//@[04:12) Variable myConcat. Type: error. Declaration start char: 0, length: 51

// invalid string using double quotes
var doubleString = "bad string"
//@[04:16) Variable doubleString. Type: error. Declaration start char: 0, length: 31

var resourceGroup = ''
//@[04:17) Variable resourceGroup. Type: ''. Declaration start char: 0, length: 22
var rgName = resourceGroup().name
//@[04:10) Variable rgName. Type: error. Declaration start char: 0, length: 33

var subscription = ''
//@[04:16) Variable subscription. Type: ''. Declaration start char: 0, length: 21
var subName = subscription().name
//@[04:11) Variable subName. Type: error. Declaration start char: 0, length: 33

// this does not work at the resource group scope
var invalidLocationVar = deployment().location
//@[04:22) Variable invalidLocationVar. Type: error. Declaration start char: 0, length: 46

var invalidEnvironmentVar = environment().aosdufhsad
//@[04:25) Variable invalidEnvironmentVar. Type: error. Declaration start char: 0, length: 52
var invalidEnvAuthVar = environment().authentication.asdgdsag
//@[04:21) Variable invalidEnvAuthVar. Type: error. Declaration start char: 0, length: 61

// invalid use of reserved namespace
var az = 1
//@[04:06) Variable az. Type: int. Declaration start char: 0, length: 10

// cannot assign a variable to a namespace
var invalidNamespaceAssignment = az
//@[04:30) Variable invalidNamespaceAssignment. Type: error. Declaration start char: 0, length: 35

var objectLiteralType = {
//@[04:21) Variable objectLiteralType. Type: object. Declaration start char: 0, length: 199
  first: true
  second: false
  third: 42
  fourth: 'test'
  fifth: [
    {
      one: true
    }
    {
      one: false
    }
  ]
  sixth: [
    {
      two: 44
    }
  ]
}

// #completionTest(54) -> objectVarTopLevel
var objectVarTopLevelCompletions = objectLiteralType.f
//@[04:32) Variable objectVarTopLevelCompletions. Type: error. Declaration start char: 0, length: 54
// #completionTest(54) -> objectVarTopLevel
var objectVarTopLevelCompletions2 = objectLiteralType.
//@[04:33) Variable objectVarTopLevelCompletions2. Type: error. Declaration start char: 0, length: 54

// this does not produce any completions because mixed array items are of type "any"
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
//@[04:29) Variable mixedArrayTypeCompletions. Type: any. Declaration start char: 0, length: 60
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions2 = objectLiteralType.fifth[0].
//@[04:30) Variable mixedArrayTypeCompletions2. Type: any. Declaration start char: 0, length: 60

// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions = objectLiteralType.sixth[0].t
//@[04:27) Variable oneArrayItemCompletions. Type: error. Declaration start char: 0, length: 58
// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions2 = objectLiteralType.sixth[0].
//@[04:28) Variable oneArrayItemCompletions2. Type: error. Declaration start char: 0, length: 58

// #completionTest(65) -> objectVarTopLevelIndexes
var objectVarTopLevelArrayIndexCompletions = objectLiteralType[f]
//@[04:42) Variable objectVarTopLevelArrayIndexCompletions. Type: error. Declaration start char: 0, length: 65

// #completionTest(58) -> twoIndexPlusSymbols
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]
//@[04:28) Variable oneArrayIndexCompletions. Type: error. Declaration start char: 0, length: 59

// Issue 486
var myFloat = 3.14
//@[04:11) Variable myFloat. Type: error. Declaration start char: 0, length: 16

// secure cannot be used as a variable decorator
@sys.secure()
var something = 1
//@[04:13) Variable something. Type: int. Declaration start char: 0, length: 31

// #completionTest(1) -> sysAndDescription
@
// #completionTest(5) -> description
@sys.
var anotherThing = true
//@[04:16) Variable anotherThing. Type: bool. Declaration start char: 0, length: 68

// invalid identifier character classes
var ☕ = true
//@[04:05) Variable <error>. Type: bool. Declaration start char: 0, length: 12
var a☕ = true
//@[04:05) Variable a. Type: error. Declaration start char: 0, length: 13

var missingArrayVariable = [for thing in stuff: 4]
//@[32:37) Local thing. Type: any. Declaration start char: 32, length: 5
//@[04:24) Variable missingArrayVariable. Type: error. Declaration start char: 0, length: 50

// loops are only allowed at the top level
var nonTopLevelLoop = {
//@[04:19) Variable nonTopLevelLoop. Type: error. Declaration start char: 0, length: 62
  notOkHere: [for thing in stuff: 4]
//@[18:23) Local thing. Type: any. Declaration start char: 18, length: 5
}

// loops with conditions won't even parse
var noFilteredLoopsInVariables = [for thing in stuff: if]
//@[38:43) Local thing. Type: any. Declaration start char: 38, length: 5
//@[04:30) Variable noFilteredLoopsInVariables. Type: error. Declaration start char: 0, length: 57

// nested loops are also not allowed
var noNestedVariableLoopsEither = [for thing in stuff: {
//@[39:44) Local thing. Type: any. Declaration start char: 39, length: 5
//@[04:31) Variable noNestedVariableLoopsEither. Type: error. Declaration start char: 0, length: 89
  hello: [for thing in []: 4]
//@[14:19) Local thing. Type: any. Declaration start char: 14, length: 5
}]

// loops in inner properties of a variable are also not supported
var innerPropertyLoop = {
//@[04:21) Variable innerPropertyLoop. Type: object. Declaration start char: 0, length: 58
  a: [for i in range(0,10): i]
//@[10:11) Local i. Type: int. Declaration start char: 10, length: 1
}
var innerPropertyLoop2 = {
//@[04:22) Variable innerPropertyLoop2. Type: object. Declaration start char: 0, length: 72
  b: {
    a: [for i in range(0,10): i]
//@[12:13) Local i. Type: int. Declaration start char: 12, length: 1
  }
}

// loops using expressions with a runtime dependency are also not allowed
var keys = listKeys('fake','fake')
//@[04:08) Variable keys. Type: any. Declaration start char: 0, length: 34
var indirection = keys
//@[04:15) Variable indirection. Type: any. Declaration start char: 0, length: 22

var runtimeLoop = [for (item, index) in []: indirection]
//@[24:28) Local item. Type: any. Declaration start char: 24, length: 4
//@[30:35) Local index. Type: int. Declaration start char: 30, length: 5
//@[04:15) Variable runtimeLoop. Type: any[]. Declaration start char: 0, length: 56
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
//@[25:29) Local item. Type: any. Declaration start char: 25, length: 4
//@[31:36) Local index. Type: int. Declaration start char: 31, length: 5
//@[04:16) Variable runtimeLoop2. Type: 's'[]. Declaration start char: 0, length: 63

var zoneInput = []
//@[04:13) Variable zoneInput. Type: array. Declaration start char: 0, length: 18
resource zones 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone, i) in zoneInput: {
//@[63:67) Local zone. Type: any. Declaration start char: 63, length: 4
//@[69:70) Local i. Type: int. Declaration start char: 69, length: 1
//@[09:14) Resource zones. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 143
  name: zone
  location: az.resourceGroup().location
}]
var inlinedVariable = zones[0].properties.zoneType
//@[04:19) Variable inlinedVariable. Type: 'Private' | 'Public'. Declaration start char: 0, length: 50

var runtimeLoop3 = [for (zone, i) in zoneInput: {
//@[25:29) Local zone. Type: any. Declaration start char: 25, length: 4
//@[31:32) Local i. Type: int. Declaration start char: 31, length: 1
//@[04:16) Variable runtimeLoop3. Type: object[]. Declaration start char: 0, length: 73
  a: inlinedVariable
}]

var runtimeLoop4 = [for (zone, i) in zones[0].properties.registrationVirtualNetworks: {
//@[25:29) Local zone. Type: SubResource. Declaration start char: 25, length: 4
//@[31:32) Local i. Type: int. Declaration start char: 31, length: 1
//@[04:16) Variable runtimeLoop4. Type: object[]. Declaration start char: 0, length: 97
  a: 0
}]

var notRuntime = concat('a','b')
//@[04:14) Variable notRuntime. Type: string. Declaration start char: 0, length: 32
var evenMoreIndirection = concat(notRuntime, string(moreIndirection))
//@[04:23) Variable evenMoreIndirection. Type: string. Declaration start char: 0, length: 69
var moreIndirection = reference('s','s', 'Full')
//@[04:19) Variable moreIndirection. Type: object. Declaration start char: 0, length: 48

var myRef = [
//@[04:09) Variable myRef. Type: string[]. Declaration start char: 0, length: 37
  evenMoreIndirection
]
var runtimeLoop5 = [for (item, index) in myRef: 's']
//@[25:29) Local item. Type: string. Declaration start char: 25, length: 4
//@[31:36) Local index. Type: int. Declaration start char: 31, length: 5
//@[04:16) Variable runtimeLoop5. Type: 's'[]. Declaration start char: 0, length: 52

// cannot use loops in expressions
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])
//@[32:37) Local thing. Type: any. Declaration start char: 32, length: 5
//@[57:62) Local thing. Type: any. Declaration start char: 57, length: 5
//@[04:18) Variable loopExpression. Type: error. Declaration start char: 0, length: 79

@batchSize(1)
var batchSizeMakesNoSenseHere = false
//@[04:29) Variable batchSizeMakesNoSenseHere. Type: bool. Declaration start char: 0, length: 51


//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[09:11) Resource kv. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 88
  name: 'testkeyvault'
}

var keyVaultSecretVar = kv.getSecret('mySecret')
//@[04:21) Variable keyVaultSecretVar. Type: string. Declaration start char: 0, length: 48
var keyVaultSecretInterpolatedVar = '${kv.getSecret('mySecret')}'
//@[04:33) Variable keyVaultSecretInterpolatedVar. Type: string. Declaration start char: 0, length: 65
var keyVaultSecretObjectVar = {
//@[04:27) Variable keyVaultSecretObjectVar. Type: object. Declaration start char: 0, length: 68
  secret: kv.getSecret('mySecret')
}
var keyVaultSecretArrayVar = [
//@[04:26) Variable keyVaultSecretArrayVar. Type: string[]. Declaration start char: 0, length: 59
  kv.getSecret('mySecret')
]
var keyVaultSecretArrayInterpolatedVar = [
//@[04:38) Variable keyVaultSecretArrayInterpolatedVar. Type: string[]. Declaration start char: 0, length: 76
  '${kv.getSecret('mySecret')}'
]

var listSecrets= ''
//@[04:15) Variable listSecrets. Type: ''. Declaration start char: 0, length: 19
var listSecretsVar = listSecrets()
//@[04:18) Variable listSecretsVar. Type: error. Declaration start char: 0, length: 34

var copy = [
//@[04:08) Variable copy. Type: object[]. Declaration start char: 0, length: 82
  {
    name: 'one'
    count: '[notAFunction()]'
    input: {}
  }
]

