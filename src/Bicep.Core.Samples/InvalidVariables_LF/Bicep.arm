
// unknown declaration
bad

// incomplete variable declaration
variable

// unassigned variable
variable foo

// no value assigned
variable foo =

// bad token value
variable foo = &

// bad value
variable foo = *

// expressions
variable bar = x
variable bar = foo()
variable x = 2 + !3
variable y = false ? true + 1 : !4

// test for array item recovery
variable x = [
  3 + 4
  =
  !null
]

// test for object property recovery
variable y = {
  =
  foo: !2
}
