
// unknown declaration
bad

// incomplete variable declaration #completionTest(0,1,2) -> declarations
var
//@[3:3) Variable <missing>. Type: error. Declaration start char: 0, length: 3

// missing identifier #completionTest(4) -> empty
var 
//@[4:4) Variable <missing>. Type: error. Declaration start char: 0, length: 4

// incomplete keyword
// #completionTest(0,1) -> declarations
v
// #completionTest(0,1,2) -> declarations
va

// unassigned variable
var foo
//@[4:7) Variable foo. Type: error. Declaration start char: 0, length: 7

// #completionTest(18,19) -> symbols
var missingValue = 
//@[4:16) Variable missingValue. Type: error. Declaration start char: 0, length: 19

// malformed identifier
var 2 
//@[4:5) Variable <error>. Type: error. Declaration start char: 0, length: 6
var $ = 23
//@[4:5) Variable <error>. Type: int. Declaration start char: 0, length: 10
var # 33 = 43
//@[4:8) Variable <error>. Type: int. Declaration start char: 0, length: 13

// no value assigned
var foo =
//@[4:7) Variable foo. Type: error. Declaration start char: 0, length: 9

// bad =
var badEquals 2
//@[4:13) Variable badEquals. Type: error. Declaration start char: 0, length: 15
var badEquals2 3 true
//@[4:14) Variable badEquals2. Type: error. Declaration start char: 0, length: 21

// malformed identifier but type check should happen regardless
var 2 = x
//@[4:5) Variable <error>. Type: error. Declaration start char: 0, length: 9

// bad token value
var foo = &
//@[4:7) Variable foo. Type: error. Declaration start char: 0, length: 11

// bad value
var foo = *
//@[4:7) Variable foo. Type: error. Declaration start char: 0, length: 11

// expressions
var bar = x
//@[4:7) Variable bar. Type: error. Declaration start char: 0, length: 11
var bar = foo()
//@[4:7) Variable bar. Type: error. Declaration start char: 0, length: 15
var x = 2 + !3
//@[4:5) Variable x. Type: error. Declaration start char: 0, length: 14
var y = false ? true + 1 : !4
//@[4:5) Variable y. Type: error. Declaration start char: 0, length: 29

// test for array item recovery
var x = [
//@[4:5) Variable x. Type: error. Declaration start char: 0, length: 31
  3 + 4
  =
  !null
]

// test for object property recovery
var y = {
//@[4:5) Variable y. Type: error. Declaration start char: 0, length: 25
  =
  foo: !2
}

// utcNow and newGuid used outside a param default value
var test = utcNow('u')
//@[4:8) Variable test. Type: error. Declaration start char: 0, length: 22
var test2 = newGuid()
//@[4:9) Variable test2. Type: error. Declaration start char: 0, length: 21

// bad string escape sequence in object key
var test3 = {
//@[4:9) Variable test3. Type: object. Declaration start char: 0, length: 36
  'bad\escape': true
}

// duplicate properties
var testDupe = {
//@[4:12) Variable testDupe. Type: object. Declaration start char: 0, length: 56
  'duplicate': true
  duplicate: true
}

// interpolation with type errors in key
var objWithInterp = {
//@[4:17) Variable objWithInterp. Type: error. Declaration start char: 0, length: 62
  'ab${nonExistentIdentifier}cd': true
}

// invalid fully qualified function access
var mySum = az.add(1,2)
//@[4:9) Variable mySum. Type: error. Declaration start char: 0, length: 23
var myConcat = sys.concat('a', az.concat('b', 'c'))
//@[4:12) Variable myConcat. Type: error. Declaration start char: 0, length: 51

// invalid string using double quotes
var doubleString = "bad string"
//@[4:16) Variable doubleString. Type: error. Declaration start char: 0, length: 31

var resourceGroup = ''
//@[4:17) Variable resourceGroup. Type: ''. Declaration start char: 0, length: 22
var rgName = resourceGroup().name
//@[4:10) Variable rgName. Type: error. Declaration start char: 0, length: 33

// this does not work at the resource group scope
var invalidLocationVar = deployment().location
//@[4:22) Variable invalidLocationVar. Type: error. Declaration start char: 0, length: 46

var invalidEnvironmentVar = environment().aosdufhsad
//@[4:25) Variable invalidEnvironmentVar. Type: error. Declaration start char: 0, length: 52
var invalidEnvAuthVar = environment().authentication.asdgdsag
//@[4:21) Variable invalidEnvAuthVar. Type: error. Declaration start char: 0, length: 61

// invalid use of reserved namespace
var az = 1
//@[4:6) Variable az. Type: int. Declaration start char: 0, length: 10

// cannot assign a variable to a namespace
var invalidNamespaceAssignment = az
//@[4:30) Variable invalidNamespaceAssignment. Type: error. Declaration start char: 0, length: 35

var objectLiteralType = {
//@[4:21) Variable objectLiteralType. Type: object. Declaration start char: 0, length: 199
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
//@[4:32) Variable objectVarTopLevelCompletions. Type: error. Declaration start char: 0, length: 54
// #completionTest(54) -> objectVarTopLevel
var objectVarTopLevelCompletions2 = objectLiteralType.
//@[4:33) Variable objectVarTopLevelCompletions2. Type: error. Declaration start char: 0, length: 54

// this does not produce any completions because mixed array items are of type "any"
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
//@[4:29) Variable mixedArrayTypeCompletions. Type: any. Declaration start char: 0, length: 60
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions2 = objectLiteralType.fifth[0].
//@[4:30) Variable mixedArrayTypeCompletions2. Type: any. Declaration start char: 0, length: 60

// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions = objectLiteralType.sixth[0].t
//@[4:27) Variable oneArrayItemCompletions. Type: error. Declaration start char: 0, length: 58
// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions2 = objectLiteralType.sixth[0].
//@[4:28) Variable oneArrayItemCompletions2. Type: error. Declaration start char: 0, length: 58

// #completionTest(65) -> objectVarTopLevelIndexes
var objectVarTopLevelArrayIndexCompletions = objectLiteralType[f]
//@[4:42) Variable objectVarTopLevelArrayIndexCompletions. Type: error. Declaration start char: 0, length: 65

// #completionTest(58) -> twoIndexPlusSymbols
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]
//@[4:28) Variable oneArrayIndexCompletions. Type: error. Declaration start char: 0, length: 59

// Issue 486
var myFloat = 3.14
//@[4:11) Variable myFloat. Type: error. Declaration start char: 0, length: 16

// secure cannot be used as a variable decorator
@sys.secure()
var something = 1
//@[4:13) Variable something. Type: int. Declaration start char: 0, length: 31

// #completionTest(1) -> sysAndDescription
@
// #completionTest(5) -> description
@sys.
var anotherThing = true
//@[4:16) Variable anotherThing. Type: bool. Declaration start char: 0, length: 68

// invalid identifier character classes
var ☕ = true
//@[4:5) Variable <error>. Type: bool. Declaration start char: 0, length: 12
var a☕ = true
//@[4:5) Variable a. Type: error. Declaration start char: 0, length: 13

var missingArrayVariable = [for thing in stuff: 4]
//@[32:37) Local thing. Type: any. Declaration start char: 32, length: 5
//@[4:24) Variable missingArrayVariable. Type: error. Declaration start char: 0, length: 50

// loops are only allowed at the top level
var nonTopLevelLoop = {
//@[4:19) Variable nonTopLevelLoop. Type: error. Declaration start char: 0, length: 62
  notOkHere: [for thing in stuff: 4]
//@[18:23) Local thing. Type: any. Declaration start char: 18, length: 5
}

// loops with conditions won't even parse
var noFilteredLoopsInVariables = [for thing in stuff: if]
//@[38:43) Local thing. Type: any. Declaration start char: 38, length: 5
//@[4:30) Variable noFilteredLoopsInVariables. Type: error. Declaration start char: 0, length: 57

// nested loops are also not allowed
var noNestedVariableLoopsEither = [for thing in stuff: {
//@[39:44) Local thing. Type: any. Declaration start char: 39, length: 5
//@[4:31) Variable noNestedVariableLoopsEither. Type: error. Declaration start char: 0, length: 89
  hello: [for thing in []: 4]
//@[14:19) Local thing. Type: any. Declaration start char: 14, length: 5
}]

// loops in inner properties of a variable are also not supported
var innerPropertyLoop = {
//@[4:21) Variable innerPropertyLoop. Type: object. Declaration start char: 0, length: 58
  a: [for i in range(0,10): i]
//@[10:11) Local i. Type: int. Declaration start char: 10, length: 1
}
var innerPropertyLoop2 = {
//@[4:22) Variable innerPropertyLoop2. Type: object. Declaration start char: 0, length: 72
  b: {
    a: [for i in range(0,10): i]
//@[12:13) Local i. Type: int. Declaration start char: 12, length: 1
  }
}

// loops using expressions with a runtime dependency are also not allowed
var keys = listKeys('fake','fake')
//@[4:8) Variable keys. Type: any. Declaration start char: 0, length: 34
var indirection = keys
//@[4:15) Variable indirection. Type: any. Declaration start char: 0, length: 22

var runtimeLoop = [for (item, index) in []: indirection]
//@[24:28) Local item. Type: any. Declaration start char: 24, length: 4
//@[30:35) Local index. Type: int. Declaration start char: 30, length: 5
//@[4:15) Variable runtimeLoop. Type: any[]. Declaration start char: 0, length: 56
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
//@[25:29) Local item. Type: any. Declaration start char: 25, length: 4
//@[31:36) Local index. Type: int. Declaration start char: 31, length: 5
//@[4:16) Variable runtimeLoop2. Type: 's'[]. Declaration start char: 0, length: 63

var zoneInput = []
//@[4:13) Variable zoneInput. Type: array. Declaration start char: 0, length: 18
resource zones 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone, i) in zoneInput: {
//@[63:67) Local zone. Type: any. Declaration start char: 63, length: 4
//@[69:70) Local i. Type: int. Declaration start char: 69, length: 1
//@[9:14) Resource zones. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 143
  name: zone
  location: az.resourceGroup().location
}]
var inlinedVariable = zones[0].properties.zoneType
//@[4:19) Variable inlinedVariable. Type: 'Private' | 'Public'. Declaration start char: 0, length: 50

var runtimeLoop3 = [for (zone, i) in zoneInput: {
//@[25:29) Local zone. Type: any. Declaration start char: 25, length: 4
//@[31:32) Local i. Type: int. Declaration start char: 31, length: 1
//@[4:16) Variable runtimeLoop3. Type: object[]. Declaration start char: 0, length: 73
  a: inlinedVariable
}]

var runtimeLoop4 = [for (zone, i) in zones[0].properties.registrationVirtualNetworks: {
//@[25:29) Local zone. Type: SubResource. Declaration start char: 25, length: 4
//@[31:32) Local i. Type: int. Declaration start char: 31, length: 1
//@[4:16) Variable runtimeLoop4. Type: object[]. Declaration start char: 0, length: 97
  a: 0
}]

var notRuntime = concat('a','b')
//@[4:14) Variable notRuntime. Type: string. Declaration start char: 0, length: 32
var evenMoreIndirection = concat(notRuntime, string(moreIndirection))
//@[4:23) Variable evenMoreIndirection. Type: string. Declaration start char: 0, length: 69
var moreIndirection = reference('s','s', 'Full')
//@[4:19) Variable moreIndirection. Type: object. Declaration start char: 0, length: 48

var myRef = [
//@[4:9) Variable myRef. Type: string[]. Declaration start char: 0, length: 37
  evenMoreIndirection
]
var runtimeLoop5 = [for (item, index) in myRef: 's']
//@[25:29) Local item. Type: string. Declaration start char: 25, length: 4
//@[31:36) Local index. Type: int. Declaration start char: 31, length: 5
//@[4:16) Variable runtimeLoop5. Type: 's'[]. Declaration start char: 0, length: 52

// cannot use loops in expressions
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])
//@[32:37) Local thing. Type: any. Declaration start char: 32, length: 5
//@[57:62) Local thing. Type: any. Declaration start char: 57, length: 5
//@[4:18) Variable loopExpression. Type: error. Declaration start char: 0, length: 79

@batchSize(1)
var batchSizeMakesNoSenseHere = false
//@[4:29) Variable batchSizeMakesNoSenseHere. Type: bool. Declaration start char: 0, length: 51


//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[9:11) Resource kv. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 88
  name: 'testkeyvault'
}

var keyVaultSecretVar = kv.getSecret('mySecret')
//@[4:21) Variable keyVaultSecretVar. Type: string. Declaration start char: 0, length: 48
var keyVaultSecretInterpolatedVar = '${kv.getSecret('mySecret')}'
//@[4:33) Variable keyVaultSecretInterpolatedVar. Type: string. Declaration start char: 0, length: 65
var keyVaultSecretObjectVar = {
//@[4:27) Variable keyVaultSecretObjectVar. Type: object. Declaration start char: 0, length: 68
  secret: kv.getSecret('mySecret')
}
var keyVaultSecretArrayVar = [
//@[4:26) Variable keyVaultSecretArrayVar. Type: string[]. Declaration start char: 0, length: 59
  kv.getSecret('mySecret')
]
var keyVaultSecretArrayInterpolatedVar = [
//@[4:38) Variable keyVaultSecretArrayInterpolatedVar. Type: string[]. Declaration start char: 0, length: 76
  '${kv.getSecret('mySecret')}'
]

