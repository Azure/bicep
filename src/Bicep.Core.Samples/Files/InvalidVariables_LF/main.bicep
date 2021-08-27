
// unknown declaration
bad

// incomplete variable declaration #completionTest(0,1,2) -> declarations
var

// missing identifier #completionTest(4) -> empty
var 

// incomplete keyword
// #completionTest(0,1) -> declarations
v
// #completionTest(0,1,2) -> declarations
va

// unassigned variable
var foo

// #completionTest(18,19) -> symbols
var missingValue = 

// malformed identifier
var 2 
var $ = 23
var # 33 = 43

// no value assigned
var foo =

// bad =
var badEquals 2
var badEquals2 3 true

// malformed identifier but type check should happen regardless
var 2 = x

// bad token value
var foo = &

// bad value
var foo = *

// expressions
var bar = x
var bar = foo()
var x = 2 + !3
var y = false ? true + 1 : !4

// test for array item recovery
var x = [
  3 + 4
  =
  !null
]

// test for object property recovery
var y = {
  =
  foo: !2
}

// utcNow and newGuid used outside a param default value
var test = utcNow('u')
var test2 = newGuid()

// bad string escape sequence in object key
var test3 = {
  'bad\escape': true
}

// duplicate properties
var testDupe = {
  'duplicate': true
  duplicate: true
}

// interpolation with type errors in key
var objWithInterp = {
  'ab${nonExistentIdentifier}cd': true
}

// invalid fully qualified function access
var mySum = az.add(1,2)
var myConcat = sys.concat('a', az.concat('b', 'c'))

// invalid string using double quotes
var doubleString = "bad string"

var resourceGroup = ''
var rgName = resourceGroup().name

// this does not work at the resource group scope
var invalidLocationVar = deployment().location

var invalidEnvironmentVar = environment().aosdufhsad
var invalidEnvAuthVar = environment().authentication.asdgdsag

// invalid use of reserved namespace
var az = 1

// cannot assign a variable to a namespace
var invalidNamespaceAssignment = az

var objectLiteralType = {
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
// #completionTest(54) -> objectVarTopLevel
var objectVarTopLevelCompletions2 = objectLiteralType.

// this does not produce any completions because mixed array items are of type "any"
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions2 = objectLiteralType.fifth[0].

// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions = objectLiteralType.sixth[0].t
// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions2 = objectLiteralType.sixth[0].

// #completionTest(65) -> objectVarTopLevelIndexes
var objectVarTopLevelArrayIndexCompletions = objectLiteralType[f]

// #completionTest(58) -> twoIndexPlusSymbols
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]

// Issue 486
var myFloat = 3.14

// secure cannot be used as a variable decorator
@sys.secure()
var something = 1

// #completionTest(1) -> sysAndDescription
@
// #completionTest(5) -> description
@sys.
var anotherThing = true

// invalid identifier character classes
var ☕ = true
var a☕ = true

var missingArrayVariable = [for thing in stuff: 4]

// loops are only allowed at the top level
var nonTopLevelLoop = {
  notOkHere: [for thing in stuff: 4]
}

// loops with conditions won't even parse
var noFilteredLoopsInVariables = [for thing in stuff: if]

// nested loops are also not allowed
var noNestedVariableLoopsEither = [for thing in stuff: {
  hello: [for thing in []: 4]
}]

// loops in inner properties of a variable are also not supported
var innerPropertyLoop = {
  a: [for i in range(0,10): i]
}
var innerPropertyLoop2 = {
  b: {
    a: [for i in range(0,10): i]
  }
}

// loops using expressions with a runtime dependency are also not allowed
var keys = listKeys('fake','fake')
var indirection = keys

var runtimeLoop = [for (item, index) in []: indirection]
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']

var zoneInput = []
resource zones 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone, i) in zoneInput: {
  name: zone
  location: az.resourceGroup().location
}]
var inlinedVariable = zones[0].properties.zoneType

var runtimeLoop3 = [for (zone, i) in zoneInput: {
  a: inlinedVariable
}]

var runtimeLoop4 = [for (zone, i) in zones[0].properties.registrationVirtualNetworks: {
  a: 0
}]

var notRuntime = concat('a','b')
var evenMoreIndirection = concat(notRuntime, string(moreIndirection))
var moreIndirection = reference('s','s', 'Full')

var myRef = [
  evenMoreIndirection
]
var runtimeLoop5 = [for (item, index) in myRef: 's']

// cannot use loops in expressions
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])

@batchSize(1)
var batchSizeMakesNoSenseHere = false


//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

var keyVaultSecretVar = kv.getSecret('mySecret')
var keyVaultSecretInterpolatedVar = '${kv.getSecret('mySecret')}'
var keyVaultSecretObjectVar = {
  secret: kv.getSecret('mySecret')
}
var keyVaultSecretArrayVar = [
  kv.getSecret('mySecret')
]
var keyVaultSecretArrayInterpolatedVar = [
  '${kv.getSecret('mySecret')}'
]
