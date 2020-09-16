
// unknown declaration
bad

// incomplete variable declaration
var

// unassigned variable
var foo

// no value assigned
var foo =

// bad token value
var foo = &

// bad value
var foo = *

// expressions
var bar = x
//@[4:7) Variable bar. Type: error. Declaration start char: 0, length: 12
var bar = foo()
//@[4:7) Variable bar. Type: error. Declaration start char: 0, length: 16
var x = 2 + !3
//@[4:5) Variable x. Type: error. Declaration start char: 0, length: 15
var y = false ? true + 1 : !4
//@[4:5) Variable y. Type: error. Declaration start char: 0, length: 31

// test for array item recovery
var x = [
//@[4:5) Variable x. Type: error. Declaration start char: 0, length: 33
  3 + 4
  =
  !null
]

// test for object property recovery
var y = {
//@[4:5) Variable y. Type: error. Declaration start char: 0, length: 27
  =
  foo: !2
}

// utcNow and newGuid used outside a param default value
var test = utcNow('u')
//@[4:8) Variable test. Type: error. Declaration start char: 0, length: 23
var test2 = newGuid()
//@[4:9) Variable test2. Type: error. Declaration start char: 0, length: 23

// bad string escape sequence in object key
var test3 = {
//@[4:9) Variable test3. Type: object. Declaration start char: 0, length: 38
  'bad\escape': true
}

// duplicate properties
var testDupe = {
//@[4:12) Variable testDupe. Type: object. Declaration start char: 0, length: 56
  'duplicate': true
  duplicate: true
}
