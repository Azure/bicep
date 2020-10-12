
// unknown declaration
bad

// incomplete variable declaration #completionTest(0,1,2) -> declarations
var

// incomplete keyword
// #completionTest(0,1) -> declarations
v
// #completionTest(0,1,2) -> declarations
va

// unassigned variable
var foo

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

var resourceGroup = ''
var rgName = resourceGroup().name

// invalid use of reserved namespace
var az = 1