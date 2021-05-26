/*
  This tests the various cases of invalid expressions.
*/

// bad expressions
var bad = a+
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[10:11) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
//@[12:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = *
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |*|
var bad = /
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |/|
var bad = %
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |%|
var bad = 33-
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[13:13) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = --33
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |-|
var bad = 3 * 4 /
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[17:17) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = 222222222222222222222222222222222222222222 * 4
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[10:52) [BCP010 (Error)] Expected a valid 32-bit signed integer. |222222222222222222222222222222222222222222|
var bad = (null) ?
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[18:18) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = (null) ? :
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[19:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |:|
//@[20:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = (null) ? !
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[20:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[20:20) [BCP018 (Error)] Expected the ":" character at this location. ||
var bad = (null)!
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[16:17) [BCP019 (Error)] Expected a new line character at this location. |!|
var bad = (null)[0]
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[10:16) [BCP076 (Error)] Cannot index over expression of type "null". Arrays or objects are required. |(null)|
var bad = ()
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |)|
var bad = 
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bad|
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// variables not supported
var x = a + 2
//@[8:9) [BCP057 (Error)] The name "a" does not exist in the current context. |a|

// unary NOT
var not = !null
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |not|
//@[10:15) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". |!null|
var not = !4
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |not|
//@[10:12) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
var not = !'s'
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "'s'". |!'s'|
var not = ![
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "array". |![\n]|
]
var not = !{
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "object". |!{\n}|
}

// unary not chaining will be added in the future
var not = !!!!!!!true
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |not|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |!|

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |minus|
//@[13:14) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |-|

// unary minus
var minus = -true
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |minus|
//@[12:17) [BCP044 (Error)] Cannot apply operator "-" to operand of type "bool". |-true|
var minus = -null
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |minus|
//@[12:17) [BCP044 (Error)] Cannot apply operator "-" to operand of type "null". |-null|
var minus = -'s'
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "'s'". |-'s'|
var minus = -[
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "array". |-[\n]|
]
var minus = -{
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "object". |-{\n}|
}

// multiplicative
var mod = 's' % true
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |mod|
//@[10:20) [BCP045 (Error)] Cannot apply operator "%" to operands of type "'s'" and "bool". |'s' % true|
var mul = true * null
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |mul|
//@[10:21) [BCP045 (Error)] Cannot apply operator "*" to operands of type "bool" and "null". |true * null|
var div = {
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |div|
//@[10:19) [BCP045 (Error)] Cannot apply operator "/" to operands of type "object" and "array". |{\n} / [\n]|
} / [
]

// additive
var add = null + 's'
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |add|
//@[10:20) [BCP045 (Error)] Cannot apply operator "+" to operands of type "null" and "'s'". |null + 's'|
var sub = true - false
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |sub|
//@[10:22) [BCP045 (Error)] Cannot apply operator "-" to operands of type "bool" and "bool". |true - false|

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
//@[4:6) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |eq|
//@[9:21) [BCP045 (Error)] Cannot apply operator "=~" to operands of type "bool" and "null". |true =~ null|
var ne = 15 !~ [
//@[4:6) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |ne|
//@[9:18) [BCP045 (Error)] Cannot apply operator "!~" to operands of type "int" and "array". |15 !~ [\n]|
]

// relational
var lt = 4 < 's'
//@[4:6) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |lt|
//@[9:16) [BCP045 (Error)] Cannot apply operator "<" to operands of type "int" and "'s'". |4 < 's'|
var lteq = null <= 10
//@[4:8) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |lteq|
//@[11:21) [BCP045 (Error)] Cannot apply operator "<=" to operands of type "null" and "int". |null <= 10|
var gt = false>[
//@[4:6) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |gt|
//@[9:18) [BCP045 (Error)] Cannot apply operator ">" to operands of type "bool" and "array". |false>[\n]|
]
var gteq = {
//@[4:8) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |gteq|
//@[11:23) [BCP045 (Error)] Cannot apply operator ">=" to operands of type "object" and "bool". |{\n} >= false|
} >= false

// logical
var and = null && 'a'
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |and|
//@[10:21) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "null" and "'a'". |null && 'a'|
var or = 10 || 4
//@[4:6) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |or|
//@[9:16) [BCP045 (Error)] Cannot apply operator "||" to operands of type "int" and "int". |10 || 4|

// conditional
var ternary = null ? 4 : false
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |ternary|
//@[14:18) [BCP046 (Error)] Expected a value of type "bool". |null|

// complex expressions
var complex = test(2 + 3*4, true || false && null)
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. |complex|
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |complex|
//@[14:18) [BCP057 (Error)] The name "test" does not exist in the current context. |test|
//@[36:49) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "bool" and "null". |false && null|
var complex = -2 && 3 && !4 && 5
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. |complex|
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |complex|
//@[14:21) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "int" and "int". |-2 && 3|
//@[25:27) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
var complex = null ? !4: false
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. |complex|
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |complex|
//@[21:23) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. |complex|
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |complex|
//@[50:57) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "int" and "int". |-2 && 3|
//@[61:63) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
//@[79:92) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "bool" and "null". |false && null|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[4:17) [BCP028 (Error)] Identifier "nestedTernary" is declared multiple times. Remove or rename the duplicates. |nestedTernary|
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |nestedTernary|
//@[31:32) [BCP046 (Error)] Expected a value of type "bool". |2|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[4:17) [BCP028 (Error)] Identifier "nestedTernary" is declared multiple times. Remove or rename the duplicates. |nestedTernary|
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |nestedTernary|
//@[21:25) [BCP046 (Error)] Expected a value of type "bool". |null|

// bad array access
var errorInsideArrayAccess = [
//@[4:26) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |errorInsideArrayAccess|
  !null
//@[2:7) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". |!null|
][!0]
//@[2:4) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!0|
var integerIndexOnNonArray = (null)[0]
//@[4:26) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |integerIndexOnNonArray|
//@[29:35) [BCP076 (Error)] Cannot index over expression of type "null". Arrays or objects are required. |(null)|
var stringIndexOnNonObject = 'test'['test']
//@[4:26) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |stringIndexOnNonObject|
//@[29:35) [BCP076 (Error)] Cannot index over expression of type "'test'". Arrays or objects are required. |'test'|
var malformedStringIndex = {
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |malformedStringIndex|
}['test\e']
//@[7:9) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". |\e|
var invalidIndexTypeOverAny = any(true)[true]
//@[4:27) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |invalidIndexTypeOverAny|
//@[40:44) [BCP049 (Error)] The array index must be of type "string" or "int" but the provided index was of type "bool". |true|
var badIndexOverArray = [][null]
//@[4:21) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badIndexOverArray|
//@[27:31) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "null". |null|
var badIndexOverArray2 = []['s']
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badIndexOverArray2|
//@[28:31) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". |'s'|
var badIndexOverObj = {}[true]
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badIndexOverObj|
//@[25:29) [BCP075 (Error)] Indexing over objects requires an index of type "string" but the provided index was of type "bool". |true|
var badIndexOverObj2 = {}[0]
//@[4:20) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badIndexOverObj2|
//@[26:27) [BCP075 (Error)] Indexing over objects requires an index of type "string" but the provided index was of type "int". |0|
var badExpressionIndexer = {}[base64('a')]
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badExpressionIndexer|
//@[30:41) [BCP054 (Error)] The type "object" does not contain any properties. |base64('a')|

// bad propertyAccess
var dotAccessOnNonObject = true.foo
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |dotAccessOnNonObject|
//@[32:35) [BCP055 (Error)] Cannot access properties of type "bool". An "object" type is required. |foo|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[4:33) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badExpressionInPropertyAccess|
//@[52:63) [BCP044 (Error)] Cannot apply operator "!" to operand of type "'location'". |!'location'|

var propertyAccessOnVariable = x.foo
//@[4:28) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |propertyAccessOnVariable|
//@[31:32) [BCP062 (Error)] The referenced declaration with name "x" is not valid. |x|

// missing property in property access
var oneValidDeclaration = {}
var missingPropertyName = oneValidDeclaration.
//@[4:23) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |missingPropertyName|
//@[46:46) [BCP020 (Error)] Expected a function or property name at this location. ||
var missingPropertyInsideAnExpression = oneValidDeclaration. + oneValidDeclaration.
//@[4:37) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |missingPropertyInsideAnExpression|
//@[61:61) [BCP020 (Error)] Expected a function or property name at this location. ||
//@[83:83) [BCP020 (Error)] Expected a function or property name at this location. ||

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
//@[4:14) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |funcvarvar|
//@[17:23) [BCP063 (Error)] The name "concat" is not a parameter, variable, resource or module. |concat|
//@[26:32) [BCP063 (Error)] The name "base64" is not a parameter, variable, resource or module. |base64|
//@[37:49) [BCP063 (Error)] The name "uniqueString" is not a parameter, variable, resource or module. |uniqueString|
param funcvarparam bool = concat
//@[6:18) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |funcvarparam|
//@[26:32) [BCP063 (Error)] The name "concat" is not a parameter, variable, resource or module. |concat|
output funcvarout array = padLeft
//@[26:33) [BCP063 (Error)] The name "padLeft" is not a parameter, variable, resource or module. |padLeft|

// non-existent function
var fakeFunc = red() + green() * orange()
//@[4:12) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |fakeFunc|
//@[15:18) [BCP057 (Error)] The name "red" does not exist in the current context. |red|
//@[23:28) [BCP057 (Error)] The name "green" does not exist in the current context. |green|
//@[33:39) [BCP082 (Error)] The name "orange" does not exist in the current context. Did you mean "range"? |orange|
param fakeFuncP string {
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |fakeFuncP|
//@[23:44) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: blue()\n}|
  default: blue()
//@[11:15) [BCP057 (Error)] The name "blue" does not exist in the current context. |blue|
}

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |fakeVar|
//@[21:35) [BCP057 (Error)] The name "totallyFakeVar" does not exist in the current context. |totallyFakeVar|

// bad functions arguments
var concatNotEnough = concat()
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |concatNotEnough|
//@[28:30) [BCP071 (Error)] Expected as least 1 argument, but got 0. |()|
var padLeftNotEnough = padLeft('s')
//@[4:20) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |padLeftNotEnough|
//@[30:35) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. |('s')|
var takeTooMany = take([
//@[4:15) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |takeTooMany|
//@[22:35) [BCP071 (Error)] Expected 2 arguments, but got 4. |([\n],1,2,'s')|
],1,2,'s')

// missing arguments
var trailingArgumentComma = format('s',)
//@[4:25) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |trailingArgumentComma|
//@[39:39) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var onlyArgumentComma = concat(,)
//@[4:21) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |onlyArgumentComma|
//@[31:31) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[32:32) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var multipleArgumentCommas = concat(,,,,,)
//@[4:26) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multipleArgumentCommas|
//@[36:36) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[37:37) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[38:38) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[39:39) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[40:40) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[41:41) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var emptyArgInBetween = concat(true,,false)
//@[4:21) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |emptyArgInBetween|
//@[36:36) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var leadingEmptyArg = concat(,[])
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |leadingEmptyArg|
//@[29:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var leadingAndTrailingEmptyArg = concat(,'s',)
//@[4:30) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |leadingAndTrailingEmptyArg|
//@[40:40) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[45:45) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// wrong argument types
var concatWrongTypes = concat({
//@[4:20) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |concatWrongTypes|
//@[30:33) [BCP048 (Error)] Cannot resolve function overload.\n  Overload 1 of 2, "(... : array): array", gave the following error:\n    Argument of type "object" is not assignable to parameter of type "array".\n  Overload 2 of 2, "(... : bool | int | string): string", gave the following error:\n    Argument of type "object" is not assignable to parameter of type "bool | int | string". |{\n}|
})
var concatWrongTypesContradiction = concat('s', [
//@[4:33) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |concatWrongTypesContradiction|
//@[48:51) [BCP070 (Error)] Argument of type "array" is not assignable to parameter of type "bool | int | string". |[\n]|
])
var indexOfWrongTypes = indexOf(1,1)
//@[4:21) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |indexOfWrongTypes|
//@[32:34) [BCP070 (Error)] Argument of type "int" is not assignable to parameter of type "string". |1,|

// not enough params
var test1 = listKeys('abcd')
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |test1|
//@[20:28) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. |('abcd')|

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |test2|
//@[12:20) [BCP082 (Error)] The name "lsitKeys" does not exist in the current context. Did you mean "listKeys"? |lsitKeys|

// just 'lis' instead of 'list'
var test3 = lis('abcd', '2020-01-01')
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |test3|
//@[12:15) [BCP057 (Error)] The name "lis" does not exist in the current context. |lis|

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
//@[4:15) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badProperty|
//@[31:37) [BCP053 (Error)] The type "object" does not contain property "myFake". Available properties include "myArr", "myBool", "myInner", "myInt", "myNull", "myStr". |myFake|
var badSpelling = sampleObject.myNul
//@[4:15) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badSpelling|
//@[31:36) [BCP083 (Error)] The type "object" does not contain property "myNul". Did you mean "myNull"? |myNul|
var badPropertyIndexer = sampleObject['fake']
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badPropertyIndexer|
//@[38:44) [BCP053 (Error)] The type "object" does not contain property "fake". Available properties include "myArr", "myBool", "myInner", "myInt", "myNull", "myStr". |'fake'|
var badType = sampleObject.myStr / 32
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badType|
//@[14:37) [BCP045 (Error)] Cannot apply operator "/" to operands of type "'s'" and "int". |sampleObject.myStr / 32|
var badInnerProperty = sampleObject.myInner.fake
//@[4:20) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badInnerProperty|
//@[44:48) [BCP053 (Error)] The type "object" does not contain property "fake". Available properties include "anotherStr", "otherArr". |fake|
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[4:16) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badInnerType|
//@[19:54) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'a'" and "int". |sampleObject.myInner.anotherStr + 2|
var badArrayIndexer = sampleObject.myArr['s']
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badArrayIndexer|
//@[41:44) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". |'s'|
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badInnerArrayIndexer|
//@[57:60) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". |'s'|
var badIndexer = sampleObject.myStr['s']
//@[4:14) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badIndexer|
//@[17:35) [BCP076 (Error)] Cannot index over expression of type "'s'". Arrays or objects are required. |sampleObject.myStr|
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |badInnerArray|
//@[41:48) [BCP053 (Error)] The type "object" does not contain property "fakeArr". Available properties include "anotherStr", "otherArr". |fakeArr|
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
//@[4:47) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |invalidPropertyCallOnInstanceFunctionAccess|
//@[50:51) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
var invalidInstanceFunctionAccess = a.b.c.bar()
//@[4:33) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |invalidInstanceFunctionAccess|
//@[36:37) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
var invalidInstanceFunctionCall = az.az()
//@[4:31) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |invalidInstanceFunctionCall|
//@[37:39) [BCP107 (Error)] The function "az" does not exist in namespace "az". |az|
var invalidPropertyAccessOnAzNamespace = az.az
//@[4:38) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |invalidPropertyAccessOnAzNamespace|
//@[44:46) [BCP052 (Error)] The type "az" does not contain property "az". |az|
var invalidPropertyAccessOnSysNamespace = sys.az
//@[4:39) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |invalidPropertyAccessOnSysNamespace|
//@[46:48) [BCP052 (Error)] The type "sys" does not contain property "az". |az|
var invalidOperands = 1 + az
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |invalidOperands|
//@[22:28) [BCP045 (Error)] Cannot apply operator "+" to operands of type "int" and "az". |1 + az|

var bannedFunctions = {
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bannedFunctions|
  var: variables()
//@[7:16) [BCP060 (Error)] The "variables" function is not supported. Directly reference variables by their symbolic names. |variables|
  param: parameters() + 2
//@[9:19) [BCP061 (Error)] The "parameters" function is not supported. Directly reference parameters by their symbolic names. |parameters|
  if: sys.if(null,null)
//@[10:12) [BCP100 (Error)] The "if" function is not supported. Use the ternary conditional operator instead. |if|
  obj: sys.createArray()
//@[11:22) [BCP101 (Error)] The "createArray" function is not supported. Construct an array literal using []. |createArray|
  arr: sys.createObject()
//@[11:23) [BCP102 (Error)] The "createObject" function is not supported. Construct an object literal using {}. |createObject|
  numeric: sys.add(1) + sys.sub(2,3) + sys.mul(8,'s') + sys.div(true) + sys.mod(null, false)
//@[15:18) [BCP069 (Error)] The function "add" is not supported. Use the "+" operator instead. |add|
//@[28:31) [BCP069 (Error)] The function "sub" is not supported. Use the "-" operator instead. |sub|
//@[43:46) [BCP069 (Error)] The function "mul" is not supported. Use the "*" operator instead. |mul|
//@[60:63) [BCP069 (Error)] The function "div" is not supported. Use the "/" operator instead. |div|
//@[76:79) [BCP069 (Error)] The function "mod" is not supported. Use the "%" operator instead. |mod|
  relational: sys.less() && sys.lessOrEquals() && sys.greater() && sys.greaterOrEquals()
//@[18:22) [BCP069 (Error)] The function "less" is not supported. Use the "<" operator instead. |less|
//@[32:44) [BCP069 (Error)] The function "lessOrEquals" is not supported. Use the "<=" operator instead. |lessOrEquals|
//@[54:61) [BCP069 (Error)] The function "greater" is not supported. Use the ">" operator instead. |greater|
//@[71:86) [BCP069 (Error)] The function "greaterOrEquals" is not supported. Use the ">=" operator instead. |greaterOrEquals|
  equals: sys.equals()
//@[14:20) [BCP069 (Error)] The function "equals" is not supported. Use the "==" operator instead. |equals|
  bool: sys.not() || sys.and() || sys.or()
//@[12:15) [BCP069 (Error)] The function "not" is not supported. Use the "!" operator instead. |not|
//@[25:28) [BCP069 (Error)] The function "and" is not supported. Use the "&&" operator instead. |and|
//@[38:40) [BCP069 (Error)] The function "or" is not supported. Use the "||" operator instead. |or|
}

// we can get function completions from namespaces
// #completionTest(22) -> azFunctions
var azFunctions = az.a
//@[4:15) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |azFunctions|
//@[21:22) [BCP052 (Error)] The type "az" does not contain property "a". |a|
// #completionTest(24) -> sysFunctions
var sysFunctions = sys.a
//@[4:16) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |sysFunctions|
//@[23:24) [BCP052 (Error)] The type "sys" does not contain property "a". |a|

// #completionTest(33) -> sysFunctions
var sysFunctionsInParens = (sys.a)
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |sysFunctionsInParens|
//@[32:33) [BCP052 (Error)] The type "sys" does not contain property "a". |a|

// missing method name
var missingMethodName = az.()
//@[4:21) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |missingMethodName|
//@[27:27) [BCP020 (Error)] Expected a function or property name at this location. ||

// missing indexer
var missingIndexerOnLiteralArray = [][][]
//@[4:32) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |missingIndexerOnLiteralArray|
//@[38:38) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. ||
//@[40:40) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. ||
var missingIndexerOnIdentifier = nonExistentIdentifier[][1][]
//@[4:30) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |missingIndexerOnIdentifier|
//@[33:54) [BCP057 (Error)] The name "nonExistentIdentifier" does not exist in the current context. |nonExistentIdentifier|
//@[55:55) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. ||
//@[60:60) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. ||

// empty parens - should produce expected expression diagnostic
var emptyParens = ()
//@[4:15) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |emptyParens|
//@[19:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |)|

// #completionTest(26) -> symbols
var anotherEmptyParens = ()
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |anotherEmptyParens|
//@[26:27) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |)|

// keywords can't be called like functions
var nullness = null()
//@[4:12) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |nullness|
//@[19:20) [BCP019 (Error)] Expected a new line character at this location. |(|
var truth = true()
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |truth|
//@[16:17) [BCP019 (Error)] Expected a new line character at this location. |(|
var falsehood = false()
//@[4:13) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |falsehood|
//@[21:22) [BCP019 (Error)] Expected a new line character at this location. |(|

var partialObject = {
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |partialObject|
  2: true
//@[2:3) [BCP022 (Error)] Expected a property name at this location. |2|
  +
//@[2:3) [BCP022 (Error)] Expected a property name at this location. |+|
//@[3:3) [BCP018 (Error)] Expected the ":" character at this location. ||
  3 : concat('s')
//@[2:3) [BCP022 (Error)] Expected a property name at this location. |3|
//@[6:17) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function.\nSee https://aka.ms/bicep/linter/prefer-interpolation |concat('s')|
  
  's' 
//@[2:5) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. |'s'|
//@[6:6) [BCP018 (Error)] Expected the ":" character at this location. ||
  's' \
//@[2:5) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. |'s'|
//@[6:7) [BCP018 (Error)] Expected the ":" character at this location. |\|
//@[6:7) [BCP001 (Error)] The following token is not recognized: "\". |\|
//@[7:7) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
  'e'   =
//@[8:9) [BCP018 (Error)] Expected the ":" character at this location. |=|
//@[9:9) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
  's' :
//@[2:5) [BCP025 (Error)] The property "s" is declared multiple times in this object. Remove or rename the duplicate properties. |'s'|
//@[7:7) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

  a
//@[2:3) [BCP025 (Error)] The property "a" is declared multiple times in this object. Remove or rename the duplicate properties. |a|
//@[3:3) [BCP018 (Error)] Expected the ":" character at this location. ||
  b $
//@[4:5) [BCP018 (Error)] Expected the ":" character at this location. |$|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "$". |$|
//@[5:5) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
  a # 22
//@[2:3) [BCP025 (Error)] The property "a" is declared multiple times in this object. Remove or rename the duplicate properties. |a|
//@[4:5) [BCP018 (Error)] Expected the ":" character at this location. |#|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "#". |#|
//@[8:8) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
  c :
//@[5:5) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
  d  : %
//@[7:8) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |%|
}

// dangling decorators - to make sure the tests work, please do not add contents after this line
@concat()
//@[1:7) [BCP152 (Error)] Function "concat" cannot be used as a decorator. |concat|
@sys.secure()
//@[0:13) [BCP147 (Error)] Expected a parameter declaration after the decorator. |@sys.secure()|
xxxxx
//@[0:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |xxxxx|


@minLength()
//@[0:12) [BCP147 (Error)] Expected a parameter declaration after the decorator. |@minLength()|
//@[10:12) [BCP071 (Error)] Expected 1 argument, but got 0. |()|









