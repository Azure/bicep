
// unknown declaration
bad

// incomplete variable declaration #completionTest(0,1,2) -> declarations
var
//@[3:3) Variable <missing>. Type: error. Declaration start char: 0, length: 3

// incomplete keyword
// #completionTest(0,1) -> declarations
v
// #completionTest(0,1,2) -> declarations
va

// unassigned variable
var foo
//@[4:7) Variable foo. Type: error. Declaration start char: 0, length: 7

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

var resourceGroup = ''
//@[4:17) Variable resourceGroup. Type: ''. Declaration start char: 0, length: 22
var rgName = resourceGroup().name
//@[4:10) Variable rgName. Type: error. Declaration start char: 0, length: 33

// invalid use of reserved namespace
var az = 1
//@[4:6) Variable az. Type: int. Declaration start char: 0, length: 10
