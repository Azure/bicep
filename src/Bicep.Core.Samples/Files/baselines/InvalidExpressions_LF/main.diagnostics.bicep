/*
  This tests the various cases of invalid expressions.
*/

// bad expressions
var bad = a+
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[10:11) [BCP057 (Error)] The name "a" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |a|
//@[12:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
var bad = *
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |*|
var bad = /
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |/|
var bad = %
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |%|
var bad = 33-
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[13:13) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
var bad = --33
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |-|
var bad = 3 * 4 /
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[17:17) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
var bad = 222222222222222222222222222222222222222222 * 4
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[10:52) [BCP010 (Error)] Expected a valid 64-bit signed integer. (bicep https://aka.ms/bicep/core-diagnostics#BCP010) |222222222222222222222222222222222222222222|
var bad = (null) ?
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[18:18) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
var bad = (null) ? :
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[19:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |:|
//@[20:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
var bad = (null) ? !
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[20:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
//@[20:20) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
var bad = (null)!
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
var bad = (null)[0]
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[10:16) [BCP076 (Error)] Cannot index over expression of type "null". Arrays or objects are required. (bicep https://aka.ms/bicep/core-diagnostics#BCP076) |(null)|
var bad = ()
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[11:11) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||
var bad = 
//@[04:07) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bad|
//@[04:07) [no-unused-vars (Warning)] Variable "bad" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bad|
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// variables not supported
var x = a + 2
//@[08:09) [BCP057 (Error)] The name "a" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |a|

// unary NOT
var not = !null
//@[04:07) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |not|
//@[04:07) [no-unused-vars (Warning)] Variable "not" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |not|
//@[10:15) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!null|
var not = !4
//@[04:07) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |not|
//@[04:07) [no-unused-vars (Warning)] Variable "not" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |not|
//@[10:12) [BCP044 (Error)] Cannot apply operator "!" to operand of type "4". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!4|
var not = !'s'
//@[04:07) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |not|
//@[04:07) [no-unused-vars (Warning)] Variable "not" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "'s'". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!'s'|
var not = ![
//@[04:07) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |not|
//@[04:07) [no-unused-vars (Warning)] Variable "not" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |![\n]|
]
var not = !{
//@[04:07) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |not|
//@[04:07) [no-unused-vars (Warning)] Variable "not" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!{\n}|
}

// unary not chaining will be added in the future
var not = !!!!!!!true
//@[04:07) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |not|
//@[04:07) [no-unused-vars (Warning)] Variable "not" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |not|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |!|

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12
//@[04:09) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |minus|
//@[04:09) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |minus|
//@[13:14) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |-|

// unary minus
var minus = -true
//@[04:09) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |minus|
//@[04:09) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |minus|
//@[12:17) [BCP044 (Error)] Cannot apply operator "-" to operand of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |-true|
var minus = -null
//@[04:09) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |minus|
//@[04:09) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |minus|
//@[12:17) [BCP044 (Error)] Cannot apply operator "-" to operand of type "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |-null|
var minus = -'s'
//@[04:09) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |minus|
//@[04:09) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "'s'". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |-'s'|
var minus = -[
//@[04:09) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |minus|
//@[04:09) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |-[\n]|
]
var minus = -{
//@[04:09) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |minus|
//@[04:09) [no-unused-vars (Warning)] Variable "minus" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |-{\n}|
}

// multiplicative
var mod = 's' % true
//@[04:07) [no-unused-vars (Warning)] Variable "mod" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mod|
//@[10:20) [BCP045 (Error)] Cannot apply operator "%" to operands of type "'s'" and "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |'s' % true|
var mul = true * null
//@[04:07) [no-unused-vars (Warning)] Variable "mul" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mul|
//@[10:21) [BCP045 (Error)] Cannot apply operator "*" to operands of type "true" and "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |true * null|
var div = {
//@[04:07) [no-unused-vars (Warning)] Variable "div" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |div|
//@[10:19) [BCP045 (Error)] Cannot apply operator "/" to operands of type "object" and "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |{\n} / [\n]|
} / [
]

// additive
var add = null + 's'
//@[04:07) [BCP028 (Error)] Identifier "add" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |add|
//@[04:07) [no-unused-vars (Warning)] Variable "add" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |add|
//@[10:20) [BCP045 (Error)] Cannot apply operator "+" to operands of type "null" and "'s'". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |null + 's'|
var sub = true - false
//@[04:07) [no-unused-vars (Warning)] Variable "sub" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sub|
//@[10:22) [BCP045 (Error)] Cannot apply operator "-" to operands of type "true" and "false". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |true - false|
var add = 'bad' + 'str'
//@[04:07) [BCP028 (Error)] Identifier "add" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |add|
//@[04:07) [no-unused-vars (Warning)] Variable "add" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |add|
//@[10:23) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'bad'" and "'str'". Use string interpolation instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |'bad' + 'str'|

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
//@[04:06) [no-unused-vars (Warning)] Variable "eq" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |eq|
//@[09:21) [BCP045 (Error)] Cannot apply operator "=~" to operands of type "true" and "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |true =~ null|
var ne = 15 !~ [
//@[04:06) [no-unused-vars (Warning)] Variable "ne" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |ne|
//@[09:18) [BCP045 (Error)] Cannot apply operator "!~" to operands of type "15" and "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |15 !~ [\n]|
]

// relational
var lt = 4 < 's'
//@[04:06) [no-unused-vars (Warning)] Variable "lt" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |lt|
//@[09:16) [BCP045 (Error)] Cannot apply operator "<" to operands of type "4" and "'s'". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |4 < 's'|
var lteq = null <= 10
//@[04:08) [no-unused-vars (Warning)] Variable "lteq" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |lteq|
//@[11:21) [BCP045 (Error)] Cannot apply operator "<=" to operands of type "null" and "10". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |null <= 10|
var gt = false>[
//@[04:06) [no-unused-vars (Warning)] Variable "gt" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |gt|
//@[09:18) [BCP045 (Error)] Cannot apply operator ">" to operands of type "false" and "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |false>[\n]|
]
var gteq = {
//@[04:08) [no-unused-vars (Warning)] Variable "gteq" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |gteq|
//@[11:23) [BCP045 (Error)] Cannot apply operator ">=" to operands of type "object" and "false". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |{\n} >= false|
} >= false

// logical
var and = null && 'a'
//@[04:07) [no-unused-vars (Warning)] Variable "and" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |and|
//@[10:21) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "null" and "'a'". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |null && 'a'|
var or = 10 || 4
//@[04:06) [no-unused-vars (Warning)] Variable "or" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |or|
//@[09:16) [BCP045 (Error)] Cannot apply operator "||" to operands of type "10" and "4". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |10 || 4|

// conditional
var ternary = null ? 4 : false
//@[04:11) [no-unused-vars (Warning)] Variable "ternary" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |ternary|
//@[14:18) [BCP046 (Error)] Expected a value of type "bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP046) |null|

// complex expressions
var complex = test(2 + 3*4, true || false && null)
//@[04:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |complex|
//@[04:11) [no-unused-vars (Warning)] Variable "complex" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |complex|
//@[14:18) [BCP057 (Error)] The name "test" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |test|
//@[36:49) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "false" and "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |false && null|
var complex = -2 && 3 && !4 && 5
//@[04:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |complex|
//@[04:11) [no-unused-vars (Warning)] Variable "complex" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |complex|
//@[14:21) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "-2" and "3". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |-2 && 3|
//@[25:27) [BCP044 (Error)] Cannot apply operator "!" to operand of type "4". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!4|
var complex = null ? !4: false
//@[04:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |complex|
//@[04:11) [no-unused-vars (Warning)] Variable "complex" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |complex|
//@[21:23) [BCP044 (Error)] Cannot apply operator "!" to operand of type "4". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!4|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[04:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |complex|
//@[04:11) [no-unused-vars (Warning)] Variable "complex" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |complex|
//@[50:57) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "-2" and "3". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |-2 && 3|
//@[61:63) [BCP044 (Error)] Cannot apply operator "!" to operand of type "4". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!4|
//@[79:92) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "false" and "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |false && null|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[04:17) [BCP028 (Error)] Identifier "nestedTernary" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |nestedTernary|
//@[04:17) [no-unused-vars (Warning)] Variable "nestedTernary" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nestedTernary|
//@[31:32) [BCP046 (Error)] Expected a value of type "bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP046) |2|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[04:17) [BCP028 (Error)] Identifier "nestedTernary" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |nestedTernary|
//@[04:17) [no-unused-vars (Warning)] Variable "nestedTernary" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nestedTernary|
//@[21:25) [BCP046 (Error)] Expected a value of type "bool". (bicep https://aka.ms/bicep/core-diagnostics#BCP046) |null|

// bad array access
var errorInsideArrayAccess = [
//@[04:26) [no-unused-vars (Warning)] Variable "errorInsideArrayAccess" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |errorInsideArrayAccess|
  !null
//@[02:07) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!null|
][!0]
//@[02:04) [BCP044 (Error)] Cannot apply operator "!" to operand of type "0". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!0|
var integerIndexOnNonArray = (null)[0]
//@[04:26) [no-unused-vars (Warning)] Variable "integerIndexOnNonArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |integerIndexOnNonArray|
//@[29:35) [BCP076 (Error)] Cannot index over expression of type "null". Arrays or objects are required. (bicep https://aka.ms/bicep/core-diagnostics#BCP076) |(null)|
var stringIndexOnNonObject = 'test'['test']
//@[04:26) [no-unused-vars (Warning)] Variable "stringIndexOnNonObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |stringIndexOnNonObject|
//@[29:35) [BCP076 (Error)] Cannot index over expression of type "'test'". Arrays or objects are required. (bicep https://aka.ms/bicep/core-diagnostics#BCP076) |'test'|
//@[35:43) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['test']|
var malformedStringIndex = {
//@[04:24) [no-unused-vars (Warning)] Variable "malformedStringIndex" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |malformedStringIndex|
}['test\e']
//@[07:09) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". (bicep https://aka.ms/bicep/core-diagnostics#BCP006) |\e|
var invalidIndexTypeOverAny = any(true)[true]
//@[04:27) [no-unused-vars (Warning)] Variable "invalidIndexTypeOverAny" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidIndexTypeOverAny|
//@[40:44) [BCP049 (Error)] The array index must be of type "string" or "int" but the provided index was of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP049) |true|
var badIndexOverArray = [][null]
//@[04:21) [no-unused-vars (Warning)] Variable "badIndexOverArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badIndexOverArray|
//@[27:31) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP074) |null|
var badIndexOverArray2 = []['s']
//@[04:22) [no-unused-vars (Warning)] Variable "badIndexOverArray2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badIndexOverArray2|
//@[27:32) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['s']|
//@[28:31) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". (bicep https://aka.ms/bicep/core-diagnostics#BCP074) |'s'|
var badIndexOverObj = {}[true]
//@[04:19) [no-unused-vars (Warning)] Variable "badIndexOverObj" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badIndexOverObj|
//@[25:29) [BCP075 (Error)] Indexing over objects requires an index of type "string" but the provided index was of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP075) |true|
var badIndexOverObj2 = {}[0]
//@[04:20) [no-unused-vars (Warning)] Variable "badIndexOverObj2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badIndexOverObj2|
//@[26:27) [BCP075 (Error)] Indexing over objects requires an index of type "string" but the provided index was of type "0". (bicep https://aka.ms/bicep/core-diagnostics#BCP075) |0|
var badExpressionIndexer = {}[base64('a')]
//@[04:24) [no-unused-vars (Warning)] Variable "badExpressionIndexer" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badExpressionIndexer|
//@[30:41) [BCP052 (Error)] The type "object" does not contain property "YQ==". (bicep https://aka.ms/bicep/core-diagnostics#BCP052) |base64('a')|

// bad propertyAccess
var dotAccessOnNonObject = true.foo
//@[04:24) [no-unused-vars (Warning)] Variable "dotAccessOnNonObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |dotAccessOnNonObject|
//@[32:35) [BCP055 (Error)] Cannot access properties of type "true". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP055) |foo|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[04:33) [no-unused-vars (Warning)] Variable "badExpressionInPropertyAccess" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badExpressionInPropertyAccess|
//@[52:63) [BCP044 (Error)] Cannot apply operator "!" to operand of type "'location'". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!'location'|

var propertyAccessOnVariable = x.foo
//@[04:28) [no-unused-vars (Warning)] Variable "propertyAccessOnVariable" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |propertyAccessOnVariable|
//@[31:32) [BCP062 (Error)] The referenced declaration with name "x" is not valid. (bicep https://aka.ms/bicep/core-diagnostics#BCP062) |x|

// missing property in property access
var oneValidDeclaration = {}
var missingPropertyName = oneValidDeclaration.
//@[04:23) [no-unused-vars (Warning)] Variable "missingPropertyName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |missingPropertyName|
//@[46:46) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||
var missingPropertyInsideAnExpression = oneValidDeclaration. + oneValidDeclaration.
//@[04:37) [no-unused-vars (Warning)] Variable "missingPropertyInsideAnExpression" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |missingPropertyInsideAnExpression|
//@[61:61) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||
//@[83:83) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
//@[04:14) [no-unused-vars (Warning)] Variable "funcvarvar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |funcvarvar|
//@[17:23) [BCP063 (Error)] The name "concat" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |concat|
//@[26:32) [BCP063 (Error)] The name "base64" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |base64|
//@[37:49) [BCP063 (Error)] The name "uniqueString" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |uniqueString|
param funcvarparam bool = concat
//@[06:18) [no-unused-params (Warning)] Parameter "funcvarparam" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |funcvarparam|
//@[26:32) [BCP063 (Error)] The name "concat" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |concat|
output funcvarout array = padLeft
//@[18:23) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[26:33) [BCP063 (Error)] The name "padLeft" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |padLeft|

// non-existent function
var fakeFunc = red() + green() * orange()
//@[04:12) [no-unused-vars (Warning)] Variable "fakeFunc" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |fakeFunc|
//@[15:18) [BCP057 (Error)] The name "red" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |red|
//@[23:28) [BCP057 (Error)] The name "green" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |green|
//@[33:39) [BCP082 (Error)] The name "orange" does not exist in the current context. Did you mean "range"? (bicep https://aka.ms/bicep/core-diagnostics#BCP082) |orange|
param fakeFuncP string = blue()
//@[06:15) [no-unused-params (Warning)] Parameter "fakeFuncP" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |fakeFuncP|
//@[25:29) [BCP057 (Error)] The name "blue" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |blue|

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')
//@[04:11) [no-unused-vars (Warning)] Variable "fakeVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |fakeVar|
//@[21:35) [BCP057 (Error)] The name "totallyFakeVar" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |totallyFakeVar|

// bad functions arguments
var concatNotEnough = concat()
//@[04:19) [no-unused-vars (Warning)] Variable "concatNotEnough" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |concatNotEnough|
//@[28:30) [BCP071 (Error)] Expected at least 1 argument, but got 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |()|
var padLeftNotEnough = padLeft('s')
//@[04:20) [no-unused-vars (Warning)] Variable "padLeftNotEnough" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |padLeftNotEnough|
//@[30:35) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('s')|
var takeTooMany = take([
//@[04:15) [no-unused-vars (Warning)] Variable "takeTooMany" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |takeTooMany|
//@[22:35) [BCP071 (Error)] Expected 2 arguments, but got 4. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |([\n],1,2,'s')|
],1,2,'s')

// missing arguments
var trailingArgumentComma = format('s',)
//@[04:25) [no-unused-vars (Warning)] Variable "trailingArgumentComma" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |trailingArgumentComma|
//@[39:40) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |)|
var onlyArgumentComma = concat(,)
//@[04:21) [no-unused-vars (Warning)] Variable "onlyArgumentComma" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |onlyArgumentComma|
//@[31:32) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
//@[32:33) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |)|
var multipleArgumentCommas = concat(,,,,,)
//@[04:26) [no-unused-vars (Warning)] Variable "multipleArgumentCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multipleArgumentCommas|
//@[36:37) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
//@[37:38) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
//@[38:39) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
//@[39:40) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
//@[40:41) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
//@[41:42) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |)|
var emptyArgInBetween = concat(true,,false)
//@[04:21) [no-unused-vars (Warning)] Variable "emptyArgInBetween" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |emptyArgInBetween|
//@[36:37) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
var leadingEmptyArg = concat(,[])
//@[04:19) [no-unused-vars (Warning)] Variable "leadingEmptyArg" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |leadingEmptyArg|
//@[29:30) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
var leadingAndTrailingEmptyArg = concat(,'s',)
//@[04:30) [no-unused-vars (Warning)] Variable "leadingAndTrailingEmptyArg" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |leadingAndTrailingEmptyArg|
//@[40:41) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
//@[45:46) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |)|

// wrong argument types
var concatWrongTypes = concat({
//@[04:20) [no-unused-vars (Warning)] Variable "concatWrongTypes" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |concatWrongTypes|
//@[30:33) [BCP048 (Error)] Cannot resolve function overload.\n  Overload 1 of 2, "(... : array): array", gave the following error:\n    Argument of type "object" is not assignable to parameter of type "array".\n  Overload 2 of 2, "(... : bool | int | string): string", gave the following error:\n    Argument of type "object" is not assignable to parameter of type "bool | int | string". (bicep https://aka.ms/bicep/core-diagnostics#BCP048) |{\n}|
})
var concatWrongTypesContradiction = concat('s', [
//@[04:33) [no-unused-vars (Warning)] Variable "concatWrongTypesContradiction" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |concatWrongTypesContradiction|
//@[48:51) [BCP070 (Error)] Argument of type "<empty array>" is not assignable to parameter of type "bool | int | string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |[\n]|
])
var indexOfWrongTypes = indexOf(1,1)
//@[04:21) [no-unused-vars (Warning)] Variable "indexOfWrongTypes" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |indexOfWrongTypes|
//@[32:33) [BCP048 (Error)] Cannot resolve function overload.\n  Overload 1 of 2, "(stringToSearch: string, stringToFind: string): int", gave the following error:\n    Argument of type "1" is not assignable to parameter of type "string".\n  Overload 2 of 2, "(array: array, itemToFind: any): int", gave the following error:\n    Argument of type "1" is not assignable to parameter of type "array". (bicep https://aka.ms/bicep/core-diagnostics#BCP048) |1|

// not enough params
var test1 = listKeys('abcd')
//@[04:09) [no-unused-vars (Warning)] Variable "test1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |test1|
//@[20:28) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('abcd')|

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')
//@[04:09) [no-unused-vars (Warning)] Variable "test2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |test2|
//@[12:20) [BCP057 (Error)] The name "lsitKeys" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |lsitKeys|

// just 'lis' instead of 'list'
var test3 = lis('abcd', '2020-01-01')
//@[04:09) [no-unused-vars (Warning)] Variable "test3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |test3|
//@[12:15) [BCP057 (Error)] The name "lis" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |lis|

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
//@[04:15) [no-unused-vars (Warning)] Variable "badProperty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badProperty|
//@[31:37) [BCP053 (Error)] The type "object" does not contain property "myFake". Available properties include "myArr", "myBool", "myInner", "myInt", "myNull", "myStr". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |myFake|
var badSpelling = sampleObject.myNul
//@[04:15) [no-unused-vars (Warning)] Variable "badSpelling" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badSpelling|
//@[31:36) [BCP083 (Error)] The type "object" does not contain property "myNul". Did you mean "myNull"? (bicep https://aka.ms/bicep/core-diagnostics#BCP083) |myNul|
var badPropertyIndexer = sampleObject['fake']
//@[04:22) [no-unused-vars (Warning)] Variable "badPropertyIndexer" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badPropertyIndexer|
//@[37:45) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['fake']|
//@[38:44) [BCP053 (Error)] The type "object" does not contain property "fake". Available properties include "myArr", "myBool", "myInner", "myInt", "myNull", "myStr". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |'fake'|
var badType = sampleObject.myStr / 32
//@[04:11) [no-unused-vars (Warning)] Variable "badType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badType|
//@[14:37) [BCP045 (Error)] Cannot apply operator "/" to operands of type "'s'" and "32". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |sampleObject.myStr / 32|
var badInnerProperty = sampleObject.myInner.fake
//@[04:20) [no-unused-vars (Warning)] Variable "badInnerProperty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badInnerProperty|
//@[44:48) [BCP053 (Error)] The type "object" does not contain property "fake". Available properties include "anotherStr", "otherArr". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |fake|
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[04:16) [no-unused-vars (Warning)] Variable "badInnerType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badInnerType|
//@[19:54) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'a'" and "2". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |sampleObject.myInner.anotherStr + 2|
var badArrayIndexer = sampleObject.myArr['s']
//@[04:19) [no-unused-vars (Warning)] Variable "badArrayIndexer" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badArrayIndexer|
//@[40:45) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['s']|
//@[41:44) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". (bicep https://aka.ms/bicep/core-diagnostics#BCP074) |'s'|
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[04:24) [no-unused-vars (Warning)] Variable "badInnerArrayIndexer" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badInnerArrayIndexer|
//@[56:61) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['s']|
//@[57:60) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". (bicep https://aka.ms/bicep/core-diagnostics#BCP074) |'s'|
var badIndexer = sampleObject.myStr['s']
//@[04:14) [no-unused-vars (Warning)] Variable "badIndexer" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badIndexer|
//@[17:35) [BCP076 (Error)] Cannot index over expression of type "'s'". Arrays or objects are required. (bicep https://aka.ms/bicep/core-diagnostics#BCP076) |sampleObject.myStr|
//@[35:40) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['s']|
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[04:17) [no-unused-vars (Warning)] Variable "badInnerArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badInnerArray|
//@[41:48) [BCP053 (Error)] The type "object" does not contain property "fakeArr". Available properties include "anotherStr", "otherArr". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |fakeArr|
//@[48:53) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |['s']|
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
//@[04:47) [no-unused-vars (Warning)] Variable "invalidPropertyCallOnInstanceFunctionAccess" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidPropertyCallOnInstanceFunctionAccess|
//@[50:51) [BCP057 (Error)] The name "a" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |a|
var invalidInstanceFunctionAccess = a.b.c.bar()
//@[04:33) [no-unused-vars (Warning)] Variable "invalidInstanceFunctionAccess" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidInstanceFunctionAccess|
//@[36:37) [BCP057 (Error)] The name "a" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |a|
var invalidInstanceFunctionCall = az.az()
//@[04:31) [no-unused-vars (Warning)] Variable "invalidInstanceFunctionCall" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidInstanceFunctionCall|
//@[37:39) [BCP107 (Error)] The function "az" does not exist in namespace "az". (bicep https://aka.ms/bicep/core-diagnostics#BCP107) |az|
var invalidPropertyAccessOnAzNamespace = az.az
//@[04:38) [no-unused-vars (Warning)] Variable "invalidPropertyAccessOnAzNamespace" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidPropertyAccessOnAzNamespace|
//@[44:46) [BCP052 (Error)] The type "az" does not contain property "az". (bicep https://aka.ms/bicep/core-diagnostics#BCP052) |az|
var invalidPropertyAccessOnSysNamespace = sys.az
//@[04:39) [no-unused-vars (Warning)] Variable "invalidPropertyAccessOnSysNamespace" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidPropertyAccessOnSysNamespace|
//@[46:48) [BCP053 (Error)] The type "sys" does not contain property "az". Available properties include "any", "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |az|
var invalidOperands = 1 + az
//@[04:19) [no-unused-vars (Warning)] Variable "invalidOperands" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidOperands|
//@[22:28) [BCP045 (Error)] Cannot apply operator "+" to operands of type "1" and "az". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |1 + az|
var invalidStringAddition = 'hello' + sampleObject.myStr
//@[04:25) [no-unused-vars (Warning)] Variable "invalidStringAddition" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidStringAddition|
//@[28:56) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'hello'" and "'s'". Use string interpolation instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |'hello' + sampleObject.myStr|

var bannedFunctions = {
//@[04:19) [no-unused-vars (Warning)] Variable "bannedFunctions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bannedFunctions|
  var: variables()
//@[07:16) [BCP060 (Error)] The "variables" function is not supported. Directly reference variables by their symbolic names. (bicep https://aka.ms/bicep/core-diagnostics#BCP060) |variables|
  param: parameters() + 2
//@[09:19) [BCP061 (Error)] The "parameters" function is not supported. Directly reference parameters by their symbolic names. (bicep https://aka.ms/bicep/core-diagnostics#BCP061) |parameters|
  if: sys.if(null,null)
//@[10:12) [BCP100 (Error)] The function "if" is not supported. Use the "?:" (ternary conditional) operator instead, e.g. condition ? ValueIfTrue : ValueIfFalse (bicep https://aka.ms/bicep/core-diagnostics#BCP100) |if|
  obj: sys.createArray()
//@[11:22) [BCP101 (Error)] The "createArray" function is not supported. Construct an array literal using []. (bicep https://aka.ms/bicep/core-diagnostics#BCP101) |createArray|
  arr: sys.createObject()
//@[11:23) [BCP102 (Error)] The "createObject" function is not supported. Construct an object literal using {}. (bicep https://aka.ms/bicep/core-diagnostics#BCP102) |createObject|
  numeric: sys.add(1) + sys.sub(2,3) + sys.mul(8,'s') + sys.div(true) + sys.mod(null, false)
//@[15:18) [BCP069 (Error)] The function "add" is not supported. Use the "+" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |add|
//@[28:31) [BCP069 (Error)] The function "sub" is not supported. Use the "-" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |sub|
//@[43:46) [BCP069 (Error)] The function "mul" is not supported. Use the "*" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |mul|
//@[60:63) [BCP069 (Error)] The function "div" is not supported. Use the "/" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |div|
//@[76:79) [BCP069 (Error)] The function "mod" is not supported. Use the "%" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |mod|
  relational: sys.less() && sys.lessOrEquals() && sys.greater() && sys.greaterOrEquals()
//@[18:22) [BCP069 (Error)] The function "less" is not supported. Use the "<" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |less|
//@[32:44) [BCP069 (Error)] The function "lessOrEquals" is not supported. Use the "<=" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |lessOrEquals|
//@[54:61) [BCP069 (Error)] The function "greater" is not supported. Use the ">" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |greater|
//@[71:86) [BCP069 (Error)] The function "greaterOrEquals" is not supported. Use the ">=" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |greaterOrEquals|
  equals: sys.equals()
//@[14:20) [BCP069 (Error)] The function "equals" is not supported. Use the "==" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |equals|
  bool: sys.not() || sys.and() || sys.or()
//@[12:15) [BCP069 (Error)] The function "not" is not supported. Use the "!" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |not|
//@[25:28) [BCP069 (Error)] The function "and" is not supported. Use the "&&" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |and|
//@[38:40) [BCP069 (Error)] The function "or" is not supported. Use the "||" operator instead. (bicep https://aka.ms/bicep/core-diagnostics#BCP069) |or|
}

// we can get function completions from namespaces
// #completionTest(22) -> azFunctions
var azFunctions = az.a
//@[04:15) [no-unused-vars (Warning)] Variable "azFunctions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |azFunctions|
//@[21:22) [BCP052 (Error)] The type "az" does not contain property "a". (bicep https://aka.ms/bicep/core-diagnostics#BCP052) |a|
// #completionTest(24) -> sysFunctions
var sysFunctions = sys.a
//@[04:16) [no-unused-vars (Warning)] Variable "sysFunctions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sysFunctions|
//@[23:24) [BCP053 (Error)] The type "sys" does not contain property "a". Available properties include "any", "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |a|

// #completionTest(33) -> sysFunctions
var sysFunctionsInParens = (sys.a)
//@[04:24) [no-unused-vars (Warning)] Variable "sysFunctionsInParens" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sysFunctionsInParens|
//@[32:33) [BCP053 (Error)] The type "sys" does not contain property "a". Available properties include "any", "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |a|

// missing method name
var missingMethodName = az.()
//@[04:21) [no-unused-vars (Warning)] Variable "missingMethodName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |missingMethodName|
//@[27:27) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||

// missing indexer
var missingIndexerOnLiteralArray = [][][]
//@[04:32) [no-unused-vars (Warning)] Variable "missingIndexerOnLiteralArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |missingIndexerOnLiteralArray|
//@[38:38) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP117) ||
//@[40:40) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP117) ||
var missingIndexerOnIdentifier = nonExistentIdentifier[][1][]
//@[04:30) [no-unused-vars (Warning)] Variable "missingIndexerOnIdentifier" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |missingIndexerOnIdentifier|
//@[33:54) [BCP057 (Error)] The name "nonExistentIdentifier" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |nonExistentIdentifier|
//@[55:55) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP117) ||
//@[60:60) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP117) ||

// empty parens - should produce expected expression diagnostic
var emptyParens = ()
//@[04:15) [no-unused-vars (Warning)] Variable "emptyParens" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |emptyParens|
//@[19:19) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||

// #completionTest(26) -> symbols
var anotherEmptyParens = ()
//@[04:22) [no-unused-vars (Warning)] Variable "anotherEmptyParens" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |anotherEmptyParens|
//@[26:26) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||

// keywords can't be called like functions
var nullness = null()
//@[04:12) [no-unused-vars (Warning)] Variable "nullness" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nullness|
//@[19:20) [BCP019 (Error)] Expected a new line character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP019) |(|
var truth = true()
//@[04:09) [no-unused-vars (Warning)] Variable "truth" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |truth|
//@[16:17) [BCP019 (Error)] Expected a new line character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP019) |(|
var falsehood = false()
//@[04:13) [no-unused-vars (Warning)] Variable "falsehood" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |falsehood|
//@[21:22) [BCP019 (Error)] Expected a new line character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP019) |(|

var partialObject = {
//@[04:17) [no-unused-vars (Warning)] Variable "partialObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |partialObject|
  2: true
//@[02:03) [BCP022 (Error)] Expected a property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP022) |2|
  +
//@[02:03) [BCP022 (Error)] Expected a property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP022) |+|
//@[03:03) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
  3 : concat('s')
//@[02:03) [BCP022 (Error)] Expected a property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP022) |3|
  
  's' 
//@[02:05) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |'s'|
//@[02:05) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP025) |'s'|
//@[06:06) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
  's' \
//@[02:05) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |'s'|
//@[02:05) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP025) |'s'|
//@[06:07) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |\|
//@[06:07) [BCP001 (Error)] The following token is not recognized: "\". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |\|
//@[07:07) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
  'e'   =
//@[02:05) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |'e'|
//@[08:09) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |=|
//@[09:09) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
  's' :
//@[02:05) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |'s'|
//@[02:05) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP025) |'s'|
//@[07:07) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

  a
//@[02:03) [BCP025 (Error)] The property "a" is declared multiple times in this object. Remove or rename the duplicate properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP025) |a|
//@[03:03) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
  b $
//@[04:05) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |$|
//@[04:05) [BCP001 (Error)] The following token is not recognized: "$". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |$|
//@[05:05) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
  a # 22
//@[02:03) [BCP025 (Error)] The property "a" is declared multiple times in this object. Remove or rename the duplicate properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP025) |a|
//@[04:05) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |#|
//@[04:05) [BCP001 (Error)] The following token is not recognized: "#". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |#|
//@[08:08) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
  c :
//@[05:05) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
  d  : %
//@[07:08) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |%|
}

//nameof expressions
var nameOfConstant = nameof('abc')
//@[04:18) [no-unused-vars (Warning)] Variable "nameOfConstant" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameOfConstant|
//@[28:33) [BCP408 (Error)] The "nameof" function can only be used with an expression which has a name. (bicep https://aka.ms/bicep/core-diagnostics#BCP408) |'abc'|
var nameOfKeyword1 = nameof(param)
//@[04:18) [no-unused-vars (Warning)] Variable "nameOfKeyword1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameOfKeyword1|
//@[28:33) [BCP057 (Error)] The name "param" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |param|
var nameOfKeyword2 = nameof(var)
//@[04:18) [no-unused-vars (Warning)] Variable "nameOfKeyword2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameOfKeyword2|
//@[28:31) [BCP057 (Error)] The name "var" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |var|
var nameOfKeyword3 = nameof(resource)
//@[04:18) [no-unused-vars (Warning)] Variable "nameOfKeyword3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameOfKeyword3|
//@[28:36) [BCP057 (Error)] The name "resource" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |resource|
var nameOfKeyword4 = nameof(module)
//@[04:18) [no-unused-vars (Warning)] Variable "nameOfKeyword4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameOfKeyword4|
//@[28:34) [BCP057 (Error)] The name "module" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |module|
var nameOfKeyword5 = nameof(output)
//@[04:18) [no-unused-vars (Warning)] Variable "nameOfKeyword5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameOfKeyword5|
//@[28:34) [BCP057 (Error)] The name "output" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |output|
var nameofExpression1 = nameof(1 + 2)
//@[04:21) [no-unused-vars (Warning)] Variable "nameofExpression1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameofExpression1|
//@[31:36) [BCP408 (Error)] The "nameof" function can only be used with an expression which has a name. (bicep https://aka.ms/bicep/core-diagnostics#BCP408) |1 + 2|
var nameofVar= 'abc'
var nameofExpression2 = nameof(true ? nameofVar : nameofVar)
//@[04:21) [no-unused-vars (Warning)] Variable "nameofExpression2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameofExpression2|
//@[31:59) [BCP408 (Error)] The "nameof" function can only be used with an expression which has a name. (bicep https://aka.ms/bicep/core-diagnostics#BCP408) |true ? nameofVar : nameofVar|
var nameofUnknown = nameof(symbolNotFound)
//@[04:17) [no-unused-vars (Warning)] Variable "nameofUnknown" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameofUnknown|
//@[27:41) [BCP057 (Error)] The name "symbolNotFound" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |symbolNotFound|
var nameofEmpty = nameof()
//@[04:15) [no-unused-vars (Warning)] Variable "nameofEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nameofEmpty|
//@[24:26) [BCP071 (Error)] Expected 1 argument, but got 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |()|

// dangling decorators - to make sure the tests work, please do not add contents after this line
@concat()
//@[01:07) [BCP152 (Error)] Function "concat" cannot be used as a decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP152) |concat|
@sys.secure()
//@[00:13) [BCP292 (Error)] Expected a parameter, output, or type declaration after the decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP292) |@sys.secure()|
xxxxx
//@[00:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |xxxxx|


var noElements = ()
//@[04:14) [no-unused-vars (Warning)] Variable "noElements" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |noElements|
//@[18:18) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) ||
var justAComma = (,)
//@[04:14) [no-unused-vars (Warning)] Variable "justAComma" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |justAComma|
//@[18:19) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
//@[18:19) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) |,|
var twoElements = (1, 2)
//@[04:15) [no-unused-vars (Warning)] Variable "twoElements" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |twoElements|
//@[19:23) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) |1, 2|
var threeElements = (1, 2, 3)
//@[04:17) [no-unused-vars (Warning)] Variable "threeElements" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |threeElements|
//@[21:28) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) |1, 2, 3|
var unterminated1 = (
//@[04:17) [no-unused-vars (Warning)] Variable "unterminated1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |unterminated1|
//@[21:25) [BCP243 (Error)] Parentheses must contain exactly one expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP243) |\nvar|
var unterminated2 = (,
//@[04:17) [BCP018 (Error)] Expected the ")" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |unterminated2|

// trailing decorator with no declaration
@minLength()
//@[00:12) [BCP292 (Error)] Expected a parameter, output, or type declaration after the decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP292) |@minLength()|
//@[10:12) [BCP071 (Error)] Expected 1 argument, but got 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |()|


