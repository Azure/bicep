
// wrong declaration
bad
//@[0:3) Error This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |bad|

// incomplete
output 
//@[7:7) Error Expected an output identifier at this location. ||

// missing type
output foo
//@[10:10) Error Expected a parameter type at this location. Please specify one of the following types: array, bool, int, object, string. ||

// wrong type + missing value
output foo fluffy
//@[17:17) Error Expected the '=' character at this location. ||

// missing value
output foo string
//@[17:17) Error Expected the '=' character at this location. ||

// missing value
output foo string =
//@[19:19) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// wrong string output values
output str string = true
//@[7:10) Error Identifier 'str' is declared multiple times. Remove or rename the duplicates. |str|
//@[20:24) Error The output expects a value of type string but the provided value is of type bool. |true|
output str string = false
//@[7:10) Error Identifier 'str' is declared multiple times. Remove or rename the duplicates. |str|
//@[20:25) Error The output expects a value of type string but the provided value is of type bool. |false|
output str string = [
//@[7:10) Error Identifier 'str' is declared multiple times. Remove or rename the duplicates. |str|
//@[20:24) Error The output expects a value of type string but the provided value is of type array. |[\r\n]|
]
output str string = {
//@[7:10) Error Identifier 'str' is declared multiple times. Remove or rename the duplicates. |str|
//@[20:24) Error The output expects a value of type string but the provided value is of type object. |{\r\n}|
}
output str string = 52
//@[7:10) Error Identifier 'str' is declared multiple times. Remove or rename the duplicates. |str|
//@[20:22) Error The output expects a value of type string but the provided value is of type int. |52|

// wrong int output values
output i int = true
//@[7:8) Error Identifier 'i' is declared multiple times. Remove or rename the duplicates. |i|
//@[15:19) Error The output expects a value of type int but the provided value is of type bool. |true|
output i int = false
//@[7:8) Error Identifier 'i' is declared multiple times. Remove or rename the duplicates. |i|
//@[15:20) Error The output expects a value of type int but the provided value is of type bool. |false|
output i int = [
//@[7:8) Error Identifier 'i' is declared multiple times. Remove or rename the duplicates. |i|
//@[15:19) Error The output expects a value of type int but the provided value is of type array. |[\r\n]|
]
output i int = }
//@[15:16) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |}|
}
//@[0:1) Error This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |}|
output i int = 'test'
//@[7:8) Error Identifier 'i' is declared multiple times. Remove or rename the duplicates. |i|
//@[15:21) Error The output expects a value of type int but the provided value is of type 'test'. |'test'|

// wrong bool output values
output b bool = [
//@[7:8) Error Identifier 'b' is declared multiple times. Remove or rename the duplicates. |b|
//@[16:20) Error The output expects a value of type bool but the provided value is of type array. |[\r\n]|
]
output b bool = {
//@[7:8) Error Identifier 'b' is declared multiple times. Remove or rename the duplicates. |b|
//@[16:20) Error The output expects a value of type bool but the provided value is of type object. |{\r\n}|
}
output b bool = 32
//@[7:8) Error Identifier 'b' is declared multiple times. Remove or rename the duplicates. |b|
//@[16:18) Error The output expects a value of type bool but the provided value is of type int. |32|
output b bool = 'str'
//@[7:8) Error Identifier 'b' is declared multiple times. Remove or rename the duplicates. |b|
//@[16:21) Error The output expects a value of type bool but the provided value is of type 'str'. |'str'|

// wrong array output values
output arr array = 32
//@[7:10) Error Identifier 'arr' is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:21) Error The output expects a value of type array but the provided value is of type int. |32|
output arr array = true
//@[7:10) Error Identifier 'arr' is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:23) Error The output expects a value of type array but the provided value is of type bool. |true|
output arr array = false
//@[7:10) Error Identifier 'arr' is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:24) Error The output expects a value of type array but the provided value is of type bool. |false|
output arr array = {
//@[7:10) Error Identifier 'arr' is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:23) Error The output expects a value of type array but the provided value is of type object. |{\r\n}|
}
output arr array = 'str'
//@[7:10) Error Identifier 'arr' is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:24) Error The output expects a value of type array but the provided value is of type 'str'. |'str'|

// wrong object output values
output o object = 32
//@[7:8) Error Identifier 'o' is declared multiple times. Remove or rename the duplicates. |o|
//@[18:20) Error The output expects a value of type object but the provided value is of type int. |32|
output o object = true
//@[7:8) Error Identifier 'o' is declared multiple times. Remove or rename the duplicates. |o|
//@[18:22) Error The output expects a value of type object but the provided value is of type bool. |true|
output o object = false
//@[7:8) Error Identifier 'o' is declared multiple times. Remove or rename the duplicates. |o|
//@[18:23) Error The output expects a value of type object but the provided value is of type bool. |false|
output o object = [
//@[7:8) Error Identifier 'o' is declared multiple times. Remove or rename the duplicates. |o|
//@[18:22) Error The output expects a value of type object but the provided value is of type array. |[\r\n]|
]
output o object = 'str'
//@[7:8) Error Identifier 'o' is declared multiple times. Remove or rename the duplicates. |o|
//@[18:23) Error The output expects a value of type object but the provided value is of type 'str'. |'str'|

// a few expression cases
output exp string = 2 + 3
//@[20:25) Error The output expects a value of type string but the provided value is of type int. |2 + 3|
output union string = true ? 's' : 1
//@[22:36) Error The output expects a value of type string but the provided value is of type 's' | int. |true ? 's' : 1|
output bad int = true && !4
//@[25:27) Error Cannot apply operator '!' to operand of type int. |!4|
output deeper bool = true ? -true : (14 && 's') + 10
//@[28:33) Error Cannot apply operator '-' to operand of type bool. |-true|
//@[37:46) Error Cannot apply operator '&&' to operands of type int and 's'. |14 && 's'|

