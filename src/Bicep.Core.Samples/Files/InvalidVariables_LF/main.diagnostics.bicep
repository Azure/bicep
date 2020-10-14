
// unknown declaration
bad
//@[0:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |bad|

// incomplete variable declaration #completionTest(0,1,2) -> declarations
var
//@[3:3) [BCP015 (Error)] Expected a variable identifier at this location. ||

// incomplete keyword
// #completionTest(0,1) -> declarations
v
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |v|
// #completionTest(0,1,2) -> declarations
va
//@[0:2) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |va|

// unassigned variable
var foo
//@[4:7) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[7:7) [BCP018 (Error)] Expected the "=" character at this location. ||

// malformed identifier
var 2 
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. |2|
//@[6:6) [BCP018 (Error)] Expected the "=" character at this location. ||
var $ = 23
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. |$|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "$". |$|
var # 33 = 43
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. |#|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "#". |#|

// no value assigned
var foo =
//@[4:7) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[9:9) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// bad =
var badEquals 2
//@[14:15) [BCP018 (Error)] Expected the "=" character at this location. |2|
//@[15:15) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var badEquals2 3 true
//@[15:16) [BCP018 (Error)] Expected the "=" character at this location. |3|
//@[21:21) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// malformed identifier but type check should happen regardless
var 2 = x
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. |2|
//@[8:9) [BCP062 (Error)] The referenced declaration with name "x" is not valid. |x|

// bad token value
var foo = &
//@[4:7) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |&|
//@[10:11) [BCP001 (Error)] The following token is not recognized: "&". |&|

// bad value
var foo = *
//@[4:7) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |*|

// expressions
var bar = x
//@[4:7) [BCP028 (Error)] Identifier "bar" is declared multiple times. Remove or rename the duplicates. |bar|
//@[10:11) [BCP062 (Error)] The referenced declaration with name "x" is not valid. |x|
var bar = foo()
//@[4:7) [BCP028 (Error)] Identifier "bar" is declared multiple times. Remove or rename the duplicates. |bar|
//@[10:13) [BCP059 (Error)] The name "foo" is not a function. |foo|
var x = 2 + !3
//@[4:5) [BCP028 (Error)] Identifier "x" is declared multiple times. Remove or rename the duplicates. |x|
//@[12:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!3|
var y = false ? true + 1 : !4
//@[4:5) [BCP028 (Error)] Identifier "y" is declared multiple times. Remove or rename the duplicates. |y|
//@[16:24) [BCP045 (Error)] Cannot apply operator "+" to operands of type "bool" and "int". |true + 1|
//@[27:29) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|

// test for array item recovery
var x = [
//@[4:5) [BCP028 (Error)] Identifier "x" is declared multiple times. Remove or rename the duplicates. |x|
  3 + 4
  =
//@[2:3) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |=|
  !null
//@[2:7) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". |!null|
]

// test for object property recovery
var y = {
//@[4:5) [BCP028 (Error)] Identifier "y" is declared multiple times. Remove or rename the duplicates. |y|
  =
//@[2:3) [BCP022 (Error)] Expected a property name at this location. |=|
//@[3:3) [BCP018 (Error)] Expected the ":" character at this location. ||
  foo: !2
//@[7:9) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!2|
}

// utcNow and newGuid used outside a param default value
var test = utcNow('u')
//@[11:17) [BCP065 (Error)] Function "utcNow" is not valid at this location. It can only be used in parameter default declarations. |utcNow|
var test2 = newGuid()
//@[12:19) [BCP065 (Error)] Function "newGuid" is not valid at this location. It can only be used in parameter default declarations. |newGuid|

// bad string escape sequence in object key
var test3 = {
  'bad\escape': true
//@[2:14) [BCP022 (Error)] Expected a property name at this location. |'bad\escape'|
//@[6:8) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: "\$", "\'", "\\", "\n", "\r", "\t". |\e|
}

// duplicate properties
var testDupe = {
  'duplicate': true
//@[2:13) [BCP025 (Error)] The property "duplicate" is declared multiple times in this object. Remove or rename the duplicate properties. |'duplicate'|
  duplicate: true
//@[2:11) [BCP025 (Error)] The property "duplicate" is declared multiple times in this object. Remove or rename the duplicate properties. |duplicate|
}

// interpolation with type errors in key
var objWithInterp = {
  'ab${nonExistentIdentifier}cd': true
//@[7:28) [BCP057 (Error)] The name "nonExistentIdentifier" does not exist in the current context. |nonExistentIdentifier|
}

// invalid fully qualified function access
var mySum = az.add(1,2)
//@[15:18) [BCP086 (Error)] The function "add" does not exist in namespace "az". |add|
var myConcat = sys.concat('a', az.concat('b', 'c'))
//@[34:40) [BCP086 (Error)] The function "concat" does not exist in namespace "az". |concat|

var resourceGroup = ''
var rgName = resourceGroup().name
//@[13:26) [BCP059 (Error)] The name "resourceGroup" is not a function. |resourceGroup|

// invalid use of reserved namespace
var az = 1
//@[4:6) [BCP084 (Error)] The symbolic name "az" is reserved. Please use a different symbolic name. Reserved namespaces are "az", "sys". |az|
