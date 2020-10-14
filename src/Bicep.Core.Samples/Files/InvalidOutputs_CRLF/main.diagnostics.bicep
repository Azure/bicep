
// wrong declaration
bad
//@[0:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |bad|

// incomplete
output 
//@[7:7) [BCP016 (Error)] Expected an output identifier at this location. ||

output foo
//@[7:10) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[10:10) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// space after identifier #completionTest(20) -> outputTypes
output spaceAfterId 
//@[20:20) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
output partialType obj
//@[19:22) [BCP030 (Error)] The output type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |obj|
//@[22:22) [BCP018 (Error)] Expected the "=" character at this location. ||

// malformed identifier
output 2
//@[7:8) [BCP016 (Error)] Expected an output identifier at this location. |2|
//@[8:8) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// malformed type
output malformedType 3
//@[21:22) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |3|
//@[22:22) [BCP018 (Error)] Expected the "=" character at this location. ||

// malformed type but type check should still happen
output malformedType2 3 = 2 + null
//@[22:23) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |3|
//@[26:34) [BCP045 (Error)] Cannot apply operator "+" to operands of type "int" and "null". |2 + null|

// malformed type assignment
output malformedAssignment 2 = 2
//@[27:28) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |2|

// malformed type before assignment
output lol 2 = true
//@[11:12) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |2|

// wrong type + missing value
output foo fluffy
//@[7:10) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[11:17) [BCP030 (Error)] The output type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffy|
//@[17:17) [BCP018 (Error)] Expected the "=" character at this location. ||

// missing value
output foo string
//@[7:10) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[17:17) [BCP018 (Error)] Expected the "=" character at this location. ||

// missing value
output foo string =
//@[7:10) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[19:19) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// wrong string output values
output str string = true
//@[7:10) [BCP028 (Error)] Identifier "str" is declared multiple times. Remove or rename the duplicates. |str|
//@[20:24) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "bool". |true|
output str string = false
//@[7:10) [BCP028 (Error)] Identifier "str" is declared multiple times. Remove or rename the duplicates. |str|
//@[20:25) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "bool". |false|
output str string = [
//@[7:10) [BCP028 (Error)] Identifier "str" is declared multiple times. Remove or rename the duplicates. |str|
//@[20:24) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "array". |[\r\n]|
]
output str string = {
//@[7:10) [BCP028 (Error)] Identifier "str" is declared multiple times. Remove or rename the duplicates. |str|
//@[20:24) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "object". |{\r\n}|
}
output str string = 52
//@[7:10) [BCP028 (Error)] Identifier "str" is declared multiple times. Remove or rename the duplicates. |str|
//@[20:22) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "int". |52|

// wrong int output values
output i int = true
//@[7:8) [BCP028 (Error)] Identifier "i" is declared multiple times. Remove or rename the duplicates. |i|
//@[15:19) [BCP026 (Error)] The output expects a value of type "int" but the provided value is of type "bool". |true|
output i int = false
//@[7:8) [BCP028 (Error)] Identifier "i" is declared multiple times. Remove or rename the duplicates. |i|
//@[15:20) [BCP026 (Error)] The output expects a value of type "int" but the provided value is of type "bool". |false|
output i int = [
//@[7:8) [BCP028 (Error)] Identifier "i" is declared multiple times. Remove or rename the duplicates. |i|
//@[15:19) [BCP026 (Error)] The output expects a value of type "int" but the provided value is of type "array". |[\r\n]|
]
output i int = }
//@[7:8) [BCP028 (Error)] Identifier "i" is declared multiple times. Remove or rename the duplicates. |i|
//@[15:16) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |}|
}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |}|
output i int = 'test'
//@[7:8) [BCP028 (Error)] Identifier "i" is declared multiple times. Remove or rename the duplicates. |i|
//@[15:21) [BCP026 (Error)] The output expects a value of type "int" but the provided value is of type "'test'". |'test'|

// wrong bool output values
output b bool = [
//@[7:8) [BCP028 (Error)] Identifier "b" is declared multiple times. Remove or rename the duplicates. |b|
//@[16:20) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "array". |[\r\n]|
]
output b bool = {
//@[7:8) [BCP028 (Error)] Identifier "b" is declared multiple times. Remove or rename the duplicates. |b|
//@[16:20) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "object". |{\r\n}|
}
output b bool = 32
//@[7:8) [BCP028 (Error)] Identifier "b" is declared multiple times. Remove or rename the duplicates. |b|
//@[16:18) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "int". |32|
output b bool = 'str'
//@[7:8) [BCP028 (Error)] Identifier "b" is declared multiple times. Remove or rename the duplicates. |b|
//@[16:21) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "'str'". |'str'|

// wrong array output values
output arr array = 32
//@[7:10) [BCP028 (Error)] Identifier "arr" is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:21) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "int". |32|
output arr array = true
//@[7:10) [BCP028 (Error)] Identifier "arr" is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:23) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "bool". |true|
output arr array = false
//@[7:10) [BCP028 (Error)] Identifier "arr" is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:24) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "bool". |false|
output arr array = {
//@[7:10) [BCP028 (Error)] Identifier "arr" is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:23) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "object". |{\r\n}|
}
output arr array = 'str'
//@[7:10) [BCP028 (Error)] Identifier "arr" is declared multiple times. Remove or rename the duplicates. |arr|
//@[19:24) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "'str'". |'str'|

// wrong object output values
output o object = 32
//@[7:8) [BCP028 (Error)] Identifier "o" is declared multiple times. Remove or rename the duplicates. |o|
//@[18:20) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "int". |32|
output o object = true
//@[7:8) [BCP028 (Error)] Identifier "o" is declared multiple times. Remove or rename the duplicates. |o|
//@[18:22) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "bool". |true|
output o object = false
//@[7:8) [BCP028 (Error)] Identifier "o" is declared multiple times. Remove or rename the duplicates. |o|
//@[18:23) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "bool". |false|
output o object = [
//@[7:8) [BCP028 (Error)] Identifier "o" is declared multiple times. Remove or rename the duplicates. |o|
//@[18:22) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "array". |[\r\n]|
]
output o object = 'str'
//@[7:8) [BCP028 (Error)] Identifier "o" is declared multiple times. Remove or rename the duplicates. |o|
//@[18:23) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "'str'". |'str'|

// a few expression cases
output exp string = 2 + 3
//@[20:25) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "int". |2 + 3|
output union string = true ? 's' : 1
//@[22:36) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "'s' | int". |true ? 's' : 1|
output bad int = true && !4
//@[25:27) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
output deeper bool = true ? -true : (14 && 's') + 10
//@[28:33) [BCP044 (Error)] Cannot apply operator "-" to operand of type "bool". |-true|
//@[37:46) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "int" and "'s'". |14 && 's'|

