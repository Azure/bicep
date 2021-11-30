/*
  This tests the various cases of invalid expressions.
*/

// bad expressions
var bad = a+
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[10:11) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|
//@[12:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var bad = *
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |*|
var bad = /
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |/|
var bad = %
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |%|
var bad = 33-
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[13:13) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var bad = --33
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |-|
var bad = 3 * 4 /
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[17:17) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var bad = 222222222222222222222222222222222222222222 * 4
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[10:52) [BCP010 (Error)] Expected a valid 32-bit signed integer. (CodeDescription: none) |222222222222222222222222222222222222222222|
var bad = (null) ?
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[18:18) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var bad = (null) ? :
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[19:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |:|
//@[20:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var bad = (null) ? !
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[20:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[20:20) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
var bad = (null)!
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[16:17) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |!|
var bad = (null)[0]
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[10:16) [BCP076 (Error)] Cannot index over expression of type "null". Arrays or objects are required. (CodeDescription: none) |(null)|
var bad = ()
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|
var bad = 
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bad|
//@[4:7) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bad|
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// variables not supported
var x = a + 2
//@[8:9) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|

// unary NOT
var not = !null
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |not|
//@[4:7) [no-unused-vars (Warning)] Variable "not" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |not|
//@[10:15) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". (CodeDescription: none) |!null|
var not = !4
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |not|
//@[4:7) [no-unused-vars (Warning)] Variable "not" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |not|
//@[10:12) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!4|
var not = !'s'
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |not|
//@[4:7) [no-unused-vars (Warning)] Variable "not" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "'s'". (CodeDescription: none) |!'s'|
var not = ![
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |not|
//@[4:7) [no-unused-vars (Warning)] Variable "not" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "array". (CodeDescription: none) |![\n]|
]
var not = !{
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |not|
//@[4:7) [no-unused-vars (Warning)] Variable "not" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "object". (CodeDescription: none) |!{\n}|
}

// unary not chaining will be added in the future
var not = !!!!!!!true
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |not|
//@[4:7) [no-unused-vars (Warning)] Variable "not" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |not|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |!|

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |minus|
//@[4:9) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |minus|
//@[13:14) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |-|

// unary minus
var minus = -true
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |minus|
//@[4:9) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |minus|
//@[12:17) [BCP044 (Error)] Cannot apply operator "-" to operand of type "bool". (CodeDescription: none) |-true|
var minus = -null
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |minus|
//@[4:9) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |minus|
//@[12:17) [BCP044 (Error)] Cannot apply operator "-" to operand of type "null". (CodeDescription: none) |-null|
var minus = -'s'
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |minus|
//@[4:9) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "'s'". (CodeDescription: none) |-'s'|
var minus = -[
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |minus|
//@[4:9) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "array". (CodeDescription: none) |-[\n]|
]
var minus = -{
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |minus|
//@[4:9) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "object". (CodeDescription: none) |-{\n}|
}

// multiplicative
var mod = 's' % true
//@[4:7) [no-unused-vars (Warning)] Variable "mod" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mod|
//@[10:20) [BCP045 (Error)] Cannot apply operator "%" to operands of type "'s'" and "bool". (CodeDescription: none) |'s' % true|
var mul = true * null
//@[4:7) [no-unused-vars (Warning)] Variable "mul" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mul|
//@[10:21) [BCP045 (Error)] Cannot apply operator "*" to operands of type "bool" and "null". (CodeDescription: none) |true * null|
var div = {
//@[4:7) [no-unused-vars (Warning)] Variable "div" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |div|
//@[10:19) [BCP045 (Error)] Cannot apply operator "/" to operands of type "object" and "array". (CodeDescription: none) |{\n} / [\n]|
} / [
]

// additive
var add = null + 's'
//@[4:7) [BCP028 (Error)] Identifier "add" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |add|
//@[4:7) [no-unused-vars (Warning)] Variable "add" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |add|
//@[10:20) [BCP045 (Error)] Cannot apply operator "+" to operands of type "null" and "'s'". (CodeDescription: none) |null + 's'|
var sub = true - false
//@[4:7) [no-unused-vars (Warning)] Variable "sub" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sub|
//@[10:22) [BCP045 (Error)] Cannot apply operator "-" to operands of type "bool" and "bool". (CodeDescription: none) |true - false|
var add = 'bad' + 'str'
//@[4:7) [BCP028 (Error)] Identifier "add" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |add|
//@[4:7) [no-unused-vars (Warning)] Variable "add" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |add|
//@[10:23) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'bad'" and "'str'". Use string interpolation instead. (CodeDescription: none) |'bad' + 'str'|

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
//@[4:6) [no-unused-vars (Warning)] Variable "eq" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |eq|
//@[9:21) [BCP045 (Error)] Cannot apply operator "=~" to operands of type "bool" and "null". (CodeDescription: none) |true =~ null|
var ne = 15 !~ [
//@[4:6) [no-unused-vars (Warning)] Variable "ne" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |ne|
//@[9:18) [BCP045 (Error)] Cannot apply operator "!~" to operands of type "int" and "array". (CodeDescription: none) |15 !~ [\n]|
]

// relational
var lt = 4 < 's'
//@[4:6) [no-unused-vars (Warning)] Variable "lt" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |lt|
//@[9:16) [BCP045 (Error)] Cannot apply operator "<" to operands of type "int" and "'s'". (CodeDescription: none) |4 < 's'|
var lteq = null <= 10
//@[4:8) [no-unused-vars (Warning)] Variable "lteq" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |lteq|
//@[11:21) [BCP045 (Error)] Cannot apply operator "<=" to operands of type "null" and "int". (CodeDescription: none) |null <= 10|
var gt = false>[
//@[4:6) [no-unused-vars (Warning)] Variable "gt" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |gt|
//@[9:18) [BCP045 (Error)] Cannot apply operator ">" to operands of type "bool" and "array". (CodeDescription: none) |false>[\n]|
]
var gteq = {
//@[4:8) [no-unused-vars (Warning)] Variable "gteq" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |gteq|
//@[11:23) [BCP045 (Error)] Cannot apply operator ">=" to operands of type "object" and "bool". (CodeDescription: none) |{\n} >= false|
} >= false

// logical
var and = null && 'a'
//@[4:7) [no-unused-vars (Warning)] Variable "and" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |and|
//@[10:21) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "null" and "'a'". (CodeDescription: none) |null && 'a'|
var or = 10 || 4
//@[4:6) [no-unused-vars (Warning)] Variable "or" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |or|
//@[9:16) [BCP045 (Error)] Cannot apply operator "||" to operands of type "int" and "int". (CodeDescription: none) |10 || 4|

// conditional
var ternary = null ? 4 : false
//@[4:11) [no-unused-vars (Warning)] Variable "ternary" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |ternary|
//@[14:18) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |null|

// complex expressions
var complex = test(2 + 3*4, true || false && null)
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |complex|
//@[4:11) [no-unused-vars (Warning)] Variable "complex" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |complex|
//@[14:18) [BCP057 (Error)] The name "test" does not exist in the current context. (CodeDescription: none) |test|
//@[36:49) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "bool" and "null". (CodeDescription: none) |false && null|
var complex = -2 && 3 && !4 && 5
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |complex|
//@[4:11) [no-unused-vars (Warning)] Variable "complex" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |complex|
//@[14:21) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "int" and "int". (CodeDescription: none) |-2 && 3|
//@[25:27) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!4|
var complex = null ? !4: false
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |complex|
//@[4:11) [no-unused-vars (Warning)] Variable "complex" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |complex|
//@[21:23) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!4|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |complex|
//@[4:11) [no-unused-vars (Warning)] Variable "complex" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |complex|
//@[50:57) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "int" and "int". (CodeDescription: none) |-2 && 3|
//@[61:63) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!4|
//@[79:92) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "bool" and "null". (CodeDescription: none) |false && null|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[4:17) [BCP028 (Error)] Identifier "nestedTernary" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nestedTernary|
//@[4:17) [no-unused-vars (Warning)] Variable "nestedTernary" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedTernary|
//@[31:32) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |2|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[4:17) [BCP028 (Error)] Identifier "nestedTernary" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nestedTernary|
//@[4:17) [no-unused-vars (Warning)] Variable "nestedTernary" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedTernary|
//@[21:25) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |null|

// bad array access
var errorInsideArrayAccess = [
//@[4:26) [no-unused-vars (Warning)] Variable "errorInsideArrayAccess" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |errorInsideArrayAccess|
  !null
//@[2:7) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". (CodeDescription: none) |!null|
][!0]
//@[2:4) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!0|
var integerIndexOnNonArray = (null)[0]
//@[4:26) [no-unused-vars (Warning)] Variable "integerIndexOnNonArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |integerIndexOnNonArray|
//@[29:35) [BCP076 (Error)] Cannot index over expression of type "null". Arrays or objects are required. (CodeDescription: none) |(null)|
var stringIndexOnNonObject = 'test'['test']
//@[4:26) [no-unused-vars (Warning)] Variable "stringIndexOnNonObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |stringIndexOnNonObject|
//@[29:35) [BCP076 (Error)] Cannot index over expression of type "'test'". Arrays or objects are required. (CodeDescription: none) |'test'|
var malformedStringIndex = {
//@[4:24) [no-unused-vars (Warning)] Variable "malformedStringIndex" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |malformedStringIndex|
}['test\e']
//@[7:9) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". (CodeDescription: none) |\e|
var invalidIndexTypeOverAny = any(true)[true]
//@[4:27) [no-unused-vars (Warning)] Variable "invalidIndexTypeOverAny" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidIndexTypeOverAny|
//@[40:44) [BCP049 (Error)] The array index must be of type "string" or "int" but the provided index was of type "bool". (CodeDescription: none) |true|
var badIndexOverArray = [][null]
//@[4:21) [no-unused-vars (Warning)] Variable "badIndexOverArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badIndexOverArray|
//@[27:31) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "null". (CodeDescription: none) |null|
var badIndexOverArray2 = []['s']
//@[4:22) [no-unused-vars (Warning)] Variable "badIndexOverArray2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badIndexOverArray2|
//@[28:31) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". (CodeDescription: none) |'s'|
var badIndexOverObj = {}[true]
//@[4:19) [no-unused-vars (Warning)] Variable "badIndexOverObj" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badIndexOverObj|
//@[25:29) [BCP075 (Error)] Indexing over objects requires an index of type "string" but the provided index was of type "bool". (CodeDescription: none) |true|
var badIndexOverObj2 = {}[0]
//@[4:20) [no-unused-vars (Warning)] Variable "badIndexOverObj2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badIndexOverObj2|
//@[26:27) [BCP075 (Error)] Indexing over objects requires an index of type "string" but the provided index was of type "int". (CodeDescription: none) |0|
var badExpressionIndexer = {}[base64('a')]
//@[4:24) [no-unused-vars (Warning)] Variable "badExpressionIndexer" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badExpressionIndexer|
//@[30:41) [BCP054 (Error)] The type "object" does not contain any properties. (CodeDescription: none) |base64('a')|

// bad propertyAccess
var dotAccessOnNonObject = true.foo
//@[4:24) [no-unused-vars (Warning)] Variable "dotAccessOnNonObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |dotAccessOnNonObject|
//@[32:35) [BCP055 (Error)] Cannot access properties of type "bool". An "object" type is required. (CodeDescription: none) |foo|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[4:33) [no-unused-vars (Warning)] Variable "badExpressionInPropertyAccess" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badExpressionInPropertyAccess|
//@[52:63) [BCP044 (Error)] Cannot apply operator "!" to operand of type "'location'". (CodeDescription: none) |!'location'|

var propertyAccessOnVariable = x.foo
//@[4:28) [no-unused-vars (Warning)] Variable "propertyAccessOnVariable" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |propertyAccessOnVariable|
//@[31:32) [BCP062 (Error)] The referenced declaration with name "x" is not valid. (CodeDescription: none) |x|

// missing property in property access
var oneValidDeclaration = {}
var missingPropertyName = oneValidDeclaration.
//@[4:23) [no-unused-vars (Warning)] Variable "missingPropertyName" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |missingPropertyName|
//@[46:46) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
var missingPropertyInsideAnExpression = oneValidDeclaration. + oneValidDeclaration.
//@[4:37) [no-unused-vars (Warning)] Variable "missingPropertyInsideAnExpression" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |missingPropertyInsideAnExpression|
//@[61:61) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
//@[83:83) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
//@[4:14) [no-unused-vars (Warning)] Variable "funcvarvar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |funcvarvar|
//@[17:23) [BCP063 (Error)] The name "concat" is not a parameter, variable, resource or module. (CodeDescription: none) |concat|
//@[26:32) [BCP063 (Error)] The name "base64" is not a parameter, variable, resource or module. (CodeDescription: none) |base64|
//@[37:49) [BCP063 (Error)] The name "uniqueString" is not a parameter, variable, resource or module. (CodeDescription: none) |uniqueString|
param funcvarparam bool = concat
//@[6:18) [no-unused-params (Warning)] Parameter "funcvarparam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |funcvarparam|
//@[26:32) [BCP063 (Error)] The name "concat" is not a parameter, variable, resource or module. (CodeDescription: none) |concat|
output funcvarout array = padLeft
//@[26:33) [BCP063 (Error)] The name "padLeft" is not a parameter, variable, resource or module. (CodeDescription: none) |padLeft|

// non-existent function
var fakeFunc = red() + green() * orange()
//@[4:12) [no-unused-vars (Warning)] Variable "fakeFunc" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |fakeFunc|
//@[15:18) [BCP057 (Error)] The name "red" does not exist in the current context. (CodeDescription: none) |red|
//@[23:28) [BCP057 (Error)] The name "green" does not exist in the current context. (CodeDescription: none) |green|
//@[33:39) [BCP082 (Error)] The name "orange" does not exist in the current context. Did you mean "range"? (CodeDescription: none) |orange|
param fakeFuncP string = blue()
//@[6:15) [no-unused-params (Warning)] Parameter "fakeFuncP" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |fakeFuncP|
//@[25:29) [BCP057 (Error)] The name "blue" does not exist in the current context. (CodeDescription: none) |blue|

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')
//@[4:11) [no-unused-vars (Warning)] Variable "fakeVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |fakeVar|
//@[21:35) [BCP057 (Error)] The name "totallyFakeVar" does not exist in the current context. (CodeDescription: none) |totallyFakeVar|

// bad functions arguments
var concatNotEnough = concat()
//@[4:19) [no-unused-vars (Warning)] Variable "concatNotEnough" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |concatNotEnough|
//@[28:30) [BCP071 (Error)] Expected at least 1 argument, but got 0. (CodeDescription: none) |()|
var padLeftNotEnough = padLeft('s')
//@[4:20) [no-unused-vars (Warning)] Variable "padLeftNotEnough" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |padLeftNotEnough|
//@[30:35) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. (CodeDescription: none) |('s')|
var takeTooMany = take([
//@[4:15) [no-unused-vars (Warning)] Variable "takeTooMany" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |takeTooMany|
//@[22:35) [BCP071 (Error)] Expected 2 arguments, but got 4. (CodeDescription: none) |([\n],1,2,'s')|
],1,2,'s')

// missing arguments
var trailingArgumentComma = format('s',)
//@[4:25) [no-unused-vars (Warning)] Variable "trailingArgumentComma" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |trailingArgumentComma|
//@[39:39) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var onlyArgumentComma = concat(,)
//@[4:21) [no-unused-vars (Warning)] Variable "onlyArgumentComma" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |onlyArgumentComma|
//@[31:31) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[32:32) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var multipleArgumentCommas = concat(,,,,,)
//@[4:26) [no-unused-vars (Warning)] Variable "multipleArgumentCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multipleArgumentCommas|
//@[36:36) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[37:37) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[38:38) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[39:39) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[40:40) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[41:41) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var emptyArgInBetween = concat(true,,false)
//@[4:21) [no-unused-vars (Warning)] Variable "emptyArgInBetween" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |emptyArgInBetween|
//@[36:36) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var leadingEmptyArg = concat(,[])
//@[4:19) [no-unused-vars (Warning)] Variable "leadingEmptyArg" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |leadingEmptyArg|
//@[29:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var leadingAndTrailingEmptyArg = concat(,'s',)
//@[4:30) [no-unused-vars (Warning)] Variable "leadingAndTrailingEmptyArg" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |leadingAndTrailingEmptyArg|
//@[40:40) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[45:45) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// wrong argument types
var concatWrongTypes = concat({
//@[4:20) [no-unused-vars (Warning)] Variable "concatWrongTypes" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |concatWrongTypes|
//@[30:33) [BCP048 (Error)] Cannot resolve function overload.\n  Overload 1 of 2, "(... : array): array", gave the following error:\n    Argument of type "object" is not assignable to parameter of type "array".\n  Overload 2 of 2, "(... : bool | int | string): string", gave the following error:\n    Argument of type "object" is not assignable to parameter of type "bool | int | string". (CodeDescription: none) |{\n}|
})
var concatWrongTypesContradiction = concat('s', [
//@[4:33) [no-unused-vars (Warning)] Variable "concatWrongTypesContradiction" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |concatWrongTypesContradiction|
//@[48:51) [BCP070 (Error)] Argument of type "array" is not assignable to parameter of type "bool | int | string". (CodeDescription: none) |[\n]|
])
var indexOfWrongTypes = indexOf(1,1)
//@[4:21) [no-unused-vars (Warning)] Variable "indexOfWrongTypes" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |indexOfWrongTypes|
//@[32:34) [BCP070 (Error)] Argument of type "int" is not assignable to parameter of type "string". (CodeDescription: none) |1,|

// not enough params
var test1 = listKeys('abcd')
//@[4:9) [no-unused-vars (Warning)] Variable "test1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |test1|
//@[20:28) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. (CodeDescription: none) |('abcd')|

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')
//@[4:9) [no-unused-vars (Warning)] Variable "test2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |test2|
//@[12:20) [BCP057 (Error)] The name "lsitKeys" does not exist in the current context. (CodeDescription: none) |lsitKeys|

// just 'lis' instead of 'list'
var test3 = lis('abcd', '2020-01-01')
//@[4:9) [no-unused-vars (Warning)] Variable "test3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |test3|
//@[12:15) [BCP057 (Error)] The name "lis" does not exist in the current context. (CodeDescription: none) |lis|

var sampleObject = {
  myInt: 42
  myStr: 's'
  myBool: false
  myNull: null
  myInner: {
    anotherStr: 'a'
    otherArr: [
      's'
      'a'
    ]
  }
  myArr: [
    1
    2
    3
  ]
}

var badProperty = sampleObject.myFake
//@[4:15) [no-unused-vars (Warning)] Variable "badProperty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badProperty|
//@[31:37) [BCP053 (Error)] The type "object" does not contain property "myFake". Available properties include "myArr", "myBool", "myInner", "myInt", "myNull", "myStr". (CodeDescription: none) |myFake|
var badSpelling = sampleObject.myNul
//@[4:15) [no-unused-vars (Warning)] Variable "badSpelling" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badSpelling|
//@[31:36) [BCP083 (Error)] The type "object" does not contain property "myNul". Did you mean "myNull"? (CodeDescription: none) |myNul|
var badPropertyIndexer = sampleObject['fake']
//@[4:22) [no-unused-vars (Warning)] Variable "badPropertyIndexer" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badPropertyIndexer|
//@[38:44) [BCP053 (Error)] The type "object" does not contain property "fake". Available properties include "myArr", "myBool", "myInner", "myInt", "myNull", "myStr". (CodeDescription: none) |'fake'|
var badType = sampleObject.myStr / 32
//@[4:11) [no-unused-vars (Warning)] Variable "badType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badType|
//@[14:37) [BCP045 (Error)] Cannot apply operator "/" to operands of type "'s'" and "int". (CodeDescription: none) |sampleObject.myStr / 32|
var badInnerProperty = sampleObject.myInner.fake
//@[4:20) [no-unused-vars (Warning)] Variable "badInnerProperty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badInnerProperty|
//@[44:48) [BCP053 (Error)] The type "object" does not contain property "fake". Available properties include "anotherStr", "otherArr". (CodeDescription: none) |fake|
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[4:16) [no-unused-vars (Warning)] Variable "badInnerType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badInnerType|
//@[19:54) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'a'" and "int". (CodeDescription: none) |sampleObject.myInner.anotherStr + 2|
var badArrayIndexer = sampleObject.myArr['s']
//@[4:19) [no-unused-vars (Warning)] Variable "badArrayIndexer" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badArrayIndexer|
//@[41:44) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". (CodeDescription: none) |'s'|
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[4:24) [no-unused-vars (Warning)] Variable "badInnerArrayIndexer" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badInnerArrayIndexer|
//@[57:60) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". (CodeDescription: none) |'s'|
var badIndexer = sampleObject.myStr['s']
//@[4:14) [no-unused-vars (Warning)] Variable "badIndexer" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badIndexer|
//@[17:35) [BCP076 (Error)] Cannot index over expression of type "'s'". Arrays or objects are required. (CodeDescription: none) |sampleObject.myStr|
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[4:17) [no-unused-vars (Warning)] Variable "badInnerArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badInnerArray|
//@[41:48) [BCP053 (Error)] The type "object" does not contain property "fakeArr". Available properties include "anotherStr", "otherArr". (CodeDescription: none) |fakeArr|
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
//@[4:47) [no-unused-vars (Warning)] Variable "invalidPropertyCallOnInstanceFunctionAccess" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidPropertyCallOnInstanceFunctionAccess|
//@[50:51) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|
var invalidInstanceFunctionAccess = a.b.c.bar()
//@[4:33) [no-unused-vars (Warning)] Variable "invalidInstanceFunctionAccess" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidInstanceFunctionAccess|
//@[36:37) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|
var invalidInstanceFunctionCall = az.az()
//@[4:31) [no-unused-vars (Warning)] Variable "invalidInstanceFunctionCall" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidInstanceFunctionCall|
//@[37:39) [BCP107 (Error)] The function "az" does not exist in namespace "az". (CodeDescription: none) |az|
var invalidPropertyAccessOnAzNamespace = az.az
//@[4:38) [no-unused-vars (Warning)] Variable "invalidPropertyAccessOnAzNamespace" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidPropertyAccessOnAzNamespace|
//@[44:46) [BCP052 (Error)] The type "az" does not contain property "az". (CodeDescription: none) |az|
var invalidPropertyAccessOnSysNamespace = sys.az
//@[4:39) [no-unused-vars (Warning)] Variable "invalidPropertyAccessOnSysNamespace" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidPropertyAccessOnSysNamespace|
//@[46:48) [BCP052 (Error)] The type "sys" does not contain property "az". (CodeDescription: none) |az|
var invalidOperands = 1 + az
//@[4:19) [no-unused-vars (Warning)] Variable "invalidOperands" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidOperands|
//@[22:28) [BCP045 (Error)] Cannot apply operator "+" to operands of type "int" and "az". (CodeDescription: none) |1 + az|
var invalidStringAddition = 'hello' + sampleObject.myStr
//@[4:25) [no-unused-vars (Warning)] Variable "invalidStringAddition" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidStringAddition|
//@[28:56) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'hello'" and "'s'". Use string interpolation instead. (CodeDescription: none) |'hello' + sampleObject.myStr|

var bannedFunctions = {
//@[4:19) [no-unused-vars (Warning)] Variable "bannedFunctions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bannedFunctions|
  var: variables()
//@[7:16) [BCP060 (Error)] The "variables" function is not supported. Directly reference variables by their symbolic names. (CodeDescription: none) |variables|
  param: parameters() + 2
//@[9:19) [BCP061 (Error)] The "parameters" function is not supported. Directly reference parameters by their symbolic names. (CodeDescription: none) |parameters|
  if: sys.if(null,null)
//@[10:12) [BCP100 (Error)] The "if" function is not supported. Use the ternary conditional operator instead. (CodeDescription: none) |if|
  obj: sys.createArray()
//@[11:22) [BCP101 (Error)] The "createArray" function is not supported. Construct an array literal using []. (CodeDescription: none) |createArray|
  arr: sys.createObject()
//@[11:23) [BCP102 (Error)] The "createObject" function is not supported. Construct an object literal using {}. (CodeDescription: none) |createObject|
  numeric: sys.add(1) + sys.sub(2,3) + sys.mul(8,'s') + sys.div(true) + sys.mod(null, false)
//@[15:18) [BCP069 (Error)] The function "add" is not supported. Use the "+" operator instead. (CodeDescription: none) |add|
//@[28:31) [BCP069 (Error)] The function "sub" is not supported. Use the "-" operator instead. (CodeDescription: none) |sub|
//@[43:46) [BCP069 (Error)] The function "mul" is not supported. Use the "*" operator instead. (CodeDescription: none) |mul|
//@[60:63) [BCP069 (Error)] The function "div" is not supported. Use the "/" operator instead. (CodeDescription: none) |div|
//@[76:79) [BCP069 (Error)] The function "mod" is not supported. Use the "%" operator instead. (CodeDescription: none) |mod|
  relational: sys.less() && sys.lessOrEquals() && sys.greater() && sys.greaterOrEquals()
//@[18:22) [BCP069 (Error)] The function "less" is not supported. Use the "<" operator instead. (CodeDescription: none) |less|
//@[32:44) [BCP069 (Error)] The function "lessOrEquals" is not supported. Use the "<=" operator instead. (CodeDescription: none) |lessOrEquals|
//@[54:61) [BCP069 (Error)] The function "greater" is not supported. Use the ">" operator instead. (CodeDescription: none) |greater|
//@[71:86) [BCP069 (Error)] The function "greaterOrEquals" is not supported. Use the ">=" operator instead. (CodeDescription: none) |greaterOrEquals|
  equals: sys.equals()
//@[14:20) [BCP069 (Error)] The function "equals" is not supported. Use the "==" operator instead. (CodeDescription: none) |equals|
  bool: sys.not() || sys.and() || sys.or()
//@[12:15) [BCP069 (Error)] The function "not" is not supported. Use the "!" operator instead. (CodeDescription: none) |not|
//@[25:28) [BCP069 (Error)] The function "and" is not supported. Use the "&&" operator instead. (CodeDescription: none) |and|
//@[38:40) [BCP069 (Error)] The function "or" is not supported. Use the "||" operator instead. (CodeDescription: none) |or|
}

// we can get function completions from namespaces
// #completionTest(22) -> azFunctions
var azFunctions = az.a
//@[4:15) [no-unused-vars (Warning)] Variable "azFunctions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |azFunctions|
//@[21:22) [BCP052 (Error)] The type "az" does not contain property "a". (CodeDescription: none) |a|
// #completionTest(24) -> sysFunctions
var sysFunctions = sys.a
//@[4:16) [no-unused-vars (Warning)] Variable "sysFunctions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sysFunctions|
//@[23:24) [BCP052 (Error)] The type "sys" does not contain property "a". (CodeDescription: none) |a|

// #completionTest(33) -> sysFunctions
var sysFunctionsInParens = (sys.a)
//@[4:24) [no-unused-vars (Warning)] Variable "sysFunctionsInParens" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sysFunctionsInParens|
//@[32:33) [BCP052 (Error)] The type "sys" does not contain property "a". (CodeDescription: none) |a|

// missing method name
var missingMethodName = az.()
//@[4:21) [no-unused-vars (Warning)] Variable "missingMethodName" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |missingMethodName|
//@[27:27) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// missing indexer
var missingIndexerOnLiteralArray = [][][]
//@[4:32) [no-unused-vars (Warning)] Variable "missingIndexerOnLiteralArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |missingIndexerOnLiteralArray|
//@[38:38) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||
//@[40:40) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||
var missingIndexerOnIdentifier = nonExistentIdentifier[][1][]
//@[4:30) [no-unused-vars (Warning)] Variable "missingIndexerOnIdentifier" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |missingIndexerOnIdentifier|
//@[33:54) [BCP057 (Error)] The name "nonExistentIdentifier" does not exist in the current context. (CodeDescription: none) |nonExistentIdentifier|
//@[55:55) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||
//@[60:60) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

// empty parens - should produce expected expression diagnostic
var emptyParens = ()
//@[4:15) [no-unused-vars (Warning)] Variable "emptyParens" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |emptyParens|
//@[19:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|

// #completionTest(26) -> symbols
var anotherEmptyParens = ()
//@[4:22) [no-unused-vars (Warning)] Variable "anotherEmptyParens" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |anotherEmptyParens|
//@[26:27) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|

// keywords can't be called like functions
var nullness = null()
//@[4:12) [no-unused-vars (Warning)] Variable "nullness" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nullness|
//@[19:20) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |(|
var truth = true()
//@[4:9) [no-unused-vars (Warning)] Variable "truth" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |truth|
//@[16:17) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |(|
var falsehood = false()
//@[4:13) [no-unused-vars (Warning)] Variable "falsehood" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |falsehood|
//@[21:22) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |(|

var partialObject = {
//@[4:17) [no-unused-vars (Warning)] Variable "partialObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |partialObject|
  2: true
//@[2:3) [BCP022 (Error)] Expected a property name at this location. (CodeDescription: none) |2|
  +
//@[2:3) [BCP022 (Error)] Expected a property name at this location. (CodeDescription: none) |+|
//@[3:3) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
  3 : concat('s')
//@[2:3) [BCP022 (Error)] Expected a property name at this location. (CodeDescription: none) |3|
  
  's' 
//@[2:5) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'s'|
//@[6:6) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
  's' \
//@[2:5) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'s'|
//@[6:7) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |\|
//@[6:7) [BCP001 (Error)] The following token is not recognized: "\". (CodeDescription: none) |\|
//@[7:7) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
  'e'   =
//@[8:9) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |=|
//@[9:9) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
  's' :
//@[2:5) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'s'|
//@[7:7) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

  a
//@[2:3) [BCP025 (Error)] The property "a" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |a|
//@[3:3) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
  b $
//@[4:5) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |$|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "$". (CodeDescription: none) |$|
//@[5:5) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
  a # 22
//@[2:3) [BCP025 (Error)] The property "a" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |a|
//@[4:5) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |#|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "#". (CodeDescription: none) |#|
//@[8:8) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
  c :
//@[5:5) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
  d  : %
//@[7:8) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |%|
}

// dangling decorators - to make sure the tests work, please do not add contents after this line
@concat()
//@[1:7) [BCP152 (Error)] Function "concat" cannot be used as a decorator. (CodeDescription: none) |concat|
@sys.secure()
//@[0:13) [BCP147 (Error)] Expected a parameter declaration after the decorator. (CodeDescription: none) |@sys.secure()|
xxxxx
//@[0:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |xxxxx|


@minLength()
//@[0:12) [BCP147 (Error)] Expected a parameter declaration after the decorator. (CodeDescription: none) |@minLength()|
//@[10:12) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|









