
// unknown declaration
bad
//@[0:3) Error This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |bad|

// incomplete variable declaration
var
//@[3:3) Error Expected a variable identifier at this location. ||

// unassigned variable
var foo
//@[7:7) Error Expected the '=' character at this location. ||

// no value assigned
var foo =
//@[9:9) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// bad token value
var foo = &
//@[10:11) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |&|
//@[10:11) Error The following token is not recognized: '&'. |&|

// bad value
var foo = *
//@[10:11) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |*|

// expressions
var bar = x
//@[4:7) Error Identifier 'bar' is declared multiple times. Remove or rename the duplicates. |bar|
//@[10:11) Error The referenced declaration with name 'x' is not valid. |x|
var bar = foo()
//@[4:7) Error Identifier 'bar' is declared multiple times. Remove or rename the duplicates. |bar|
//@[10:13) Error The name 'foo' does not exist in the current context. |foo|
var x = 2 + !3
//@[4:5) Error Identifier 'x' is declared multiple times. Remove or rename the duplicates. |x|
//@[12:14) Error Cannot apply operator '!' to operand of type int. |!3|
var y = false ? true + 1 : !4
//@[4:5) Error Identifier 'y' is declared multiple times. Remove or rename the duplicates. |y|
//@[16:24) Error Cannot apply operator '+' to operands of type bool and int. |true + 1|
//@[27:29) Error Cannot apply operator '!' to operand of type int. |!4|

// test for array item recovery
var x = [
//@[4:5) Error Identifier 'x' is declared multiple times. Remove or rename the duplicates. |x|
  3 + 4
  =
//@[2:3) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |=|
  !null
//@[2:7) Error Cannot apply operator '!' to operand of type null. |!null|
]

// test for object property recovery
var y = {
//@[4:5) Error Identifier 'y' is declared multiple times. Remove or rename the duplicates. |y|
  =
//@[2:3) Error Expected a property name at this location. |=|
  foo: !2
//@[7:9) Error Cannot apply operator '!' to operand of type int. |!2|
}

// utcNow and newGuid used outside a param default value
var test = utcNow('u')
//@[11:17) Error Function 'utcNow' is not valid at this location. It can only be used in parameter default declarations. |utcNow|
var test2 = newGuid()
//@[12:19) Error Function 'newGuid' is not valid at this location. It can only be used in parameter default declarations. |newGuid|

// bad string escape sequence in object key
var test3 = {
  'bad\escape': true
//@[2:14) Error Expected a property name at this location. |'bad\escape'|
//@[6:8) Error The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: \$, \', \\, \n, \r, \t. |\e|
}

// duplicate properties
var testDupe = {
  'duplicate': true
//@[2:13) Error The property 'duplicate' is declared multiple times in this object. Remove or rename the duplicate properties. |'duplicate'|
  duplicate: true
//@[2:11) Error The property 'duplicate' is declared multiple times in this object. Remove or rename the duplicate properties. |duplicate|
}
