
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
