/*
  This tests the various cases of invalid expressions.
*/

// bad expressions
var bad = a+
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[12:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = *
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |*|
var bad = /
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |/|
var bad = %
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |%|
var bad = 33-
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[13:13) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = --33
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |-|
var bad = 3 * 4 /
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[17:17) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = 222222222222222222222222222222222222222222 * 4
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[10:52) [BCP010 (Error)] Expected a valid 32-bit signed integer. |222222222222222222222222222222222222222222|
var bad = (null) ?
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[18:18) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = (null) ? :
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[19:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |:|
var bad = (null) ? !
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[20:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = (null)!
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[16:17) [BCP019 (Error)] Expected a new line character at this location. |!|
var bad = (null)[0]
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[10:16) [BCP076 (Error)] Cannot index over expression of type "null". Arrays or objects are required. |(null)|
var bad = ()
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |)|
var bad = 
//@[4:7) [BCP028 (Error)] Identifier "bad" is declared multiple times. Remove or rename the duplicates. |bad|
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// variables not supported
var x = a + 2
//@[8:9) [BCP057 (Error)] The name "a" does not exist in the current context. |a|

// unary NOT
var not = !null
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[10:15) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". |!null|
var not = !4
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[10:12) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
var not = !'s'
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "'s'". |!'s'|
var not = ![
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "array". |![\n]|
]
var not = !{
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[10:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "object". |!{\n}|
}

// unary not chaining will be added in the future
var not = !!!!!!!true
//@[4:7) [BCP028 (Error)] Identifier "not" is declared multiple times. Remove or rename the duplicates. |not|
//@[11:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |!|

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[13:14) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |-|

// unary minus
var minus = -true
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:17) [BCP044 (Error)] Cannot apply operator "-" to operand of type "bool". |-true|
var minus = -null
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:17) [BCP044 (Error)] Cannot apply operator "-" to operand of type "null". |-null|
var minus = -'s'
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "'s'". |-'s'|
var minus = -[
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "array". |-[\n]|
]
var minus = -{
//@[4:9) [BCP028 (Error)] Identifier "minus" is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:16) [BCP044 (Error)] Cannot apply operator "-" to operand of type "object". |-{\n}|
}

// multiplicative
var mod = 's' % true
//@[10:20) [BCP045 (Error)] Cannot apply operator "%" to operands of type "'s'" and "bool". |'s' % true|
var mul = true * null
//@[10:21) [BCP045 (Error)] Cannot apply operator "*" to operands of type "bool" and "null". |true * null|
var div = {
//@[10:19) [BCP045 (Error)] Cannot apply operator "/" to operands of type "object" and "array". |{\n} / [\n]|
} / [
]

// additive
var add = null + 's'
//@[10:20) [BCP045 (Error)] Cannot apply operator "+" to operands of type "null" and "'s'". |null + 's'|
var sub = true - false
//@[10:22) [BCP045 (Error)] Cannot apply operator "-" to operands of type "bool" and "bool". |true - false|

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
//@[9:21) [BCP045 (Error)] Cannot apply operator "=~" to operands of type "bool" and "null". |true =~ null|
var ne = 15 !~ [
//@[9:18) [BCP045 (Error)] Cannot apply operator "!~" to operands of type "int" and "array". |15 !~ [\n]|
]

// relational
var lt = 4 < 's'
//@[9:16) [BCP045 (Error)] Cannot apply operator "<" to operands of type "int" and "'s'". |4 < 's'|
var lteq = null <= 10
//@[11:21) [BCP045 (Error)] Cannot apply operator "<=" to operands of type "null" and "int". |null <= 10|
var gt = false>[
//@[9:18) [BCP045 (Error)] Cannot apply operator ">" to operands of type "bool" and "array". |false>[\n]|
]
var gteq = {
//@[11:23) [BCP045 (Error)] Cannot apply operator ">=" to operands of type "object" and "bool". |{\n} >= false|
} >= false

// logical
var and = null && 'a'
//@[10:21) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "null" and "'a'". |null && 'a'|
var or = 10 || 4
//@[9:16) [BCP045 (Error)] Cannot apply operator "||" to operands of type "int" and "int". |10 || 4|

// conditional
var ternary = null ? 4 : false
//@[14:18) [BCP046 (Error)] Expected a value of type "bool". |null|

// complex expressions
var complex = test(2 + 3*4, true || false && null)
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. |complex|
//@[14:18) [BCP082 (Error)] The name "test" does not exist in the current context. Did you mean "test1"? |test|
//@[36:49) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "bool" and "null". |false && null|
var complex = -2 && 3 && !4 && 5
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. |complex|
//@[14:21) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "int" and "int". |-2 && 3|
//@[25:27) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
var complex = null ? !4: false
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. |complex|
//@[21:23) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[4:11) [BCP028 (Error)] Identifier "complex" is declared multiple times. Remove or rename the duplicates. |complex|
//@[50:57) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "int" and "int". |-2 && 3|
//@[61:63) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
//@[79:92) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "bool" and "null". |false && null|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[4:17) [BCP028 (Error)] Identifier "nestedTernary" is declared multiple times. Remove or rename the duplicates. |nestedTernary|
//@[31:32) [BCP046 (Error)] Expected a value of type "bool". |2|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[4:17) [BCP028 (Error)] Identifier "nestedTernary" is declared multiple times. Remove or rename the duplicates. |nestedTernary|
//@[21:25) [BCP046 (Error)] Expected a value of type "bool". |null|

// bad array access
var errorInsideArrayAccess = [
  !null
//@[2:7) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". |!null|
][!0]
//@[2:4) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!0|
var integerIndexOnNonArray = (null)[0]
//@[29:35) [BCP076 (Error)] Cannot index over expression of type "null". Arrays or objects are required. |(null)|
var stringIndexOnNonObject = 'test'['test']
//@[29:35) [BCP076 (Error)] Cannot index over expression of type "'test'". Arrays or objects are required. |'test'|
var malformedStringIndex = {
}['test\e']
//@[7:9) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: "\$", "\'", "\\", "\n", "\r", "\t". |\e|
var invalidIndexTypeOverAny = any(true)[true]
//@[40:44) [BCP049 (Error)] The array index must be of type "string" or "int" but the provided index was of type "bool". |true|
var badIndexOverArray = [][null]
//@[27:31) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "null". |null|
var badIndexOverArray2 = []['s']
//@[28:31) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". |'s'|
var badIndexOverObj = {}[true]
//@[25:29) [BCP075 (Error)] Indexing over objects requires an index of type "string" but the provided index was of type "bool". |true|
var badIndexOverObj2 = {}[0]
//@[26:27) [BCP075 (Error)] Indexing over objects requires an index of type "string" but the provided index was of type "int". |0|
var badExpressionIndexer = {}[base64('a')]
//@[30:41) [BCP054 (Error)] The type "object" does not contain any properties. |base64('a')|

// bad propertyAccess
var dotAccessOnNonObject = true.foo
//@[32:35) [BCP055 (Error)] Cannot access properties of type "bool". An "object" type is required. |foo|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[52:63) [BCP044 (Error)] Cannot apply operator "!" to operand of type "'location'". |!'location'|

var propertyAccessOnVariable = x.foo
//@[31:32) [BCP062 (Error)] The referenced declaration with name "x" is not valid. |x|

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
//@[17:23) [BCP063 (Error)] The name "concat" is not a parameter or variable. |concat|
//@[26:32) [BCP063 (Error)] The name "base64" is not a parameter or variable. |base64|
//@[37:49) [BCP063 (Error)] The name "uniqueString" is not a parameter or variable. |uniqueString|
param funcvarparam bool = concat
//@[26:32) [BCP063 (Error)] The name "concat" is not a parameter or variable. |concat|
output funcvarout array = padLeft
//@[26:33) [BCP063 (Error)] The name "padLeft" is not a parameter or variable. |padLeft|

// non-existent function
var fakeFunc = red() + green() * orange()
//@[15:18) [BCP057 (Error)] The name "red" does not exist in the current context. |red|
//@[23:28) [BCP057 (Error)] The name "green" does not exist in the current context. |green|
//@[33:39) [BCP082 (Error)] The name "orange" does not exist in the current context. Did you mean "range"? |orange|
param fakeFuncP string {
  default: blue()
//@[11:15) [BCP057 (Error)] The name "blue" does not exist in the current context. |blue|
}

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')
//@[21:35) [BCP057 (Error)] The name "totallyFakeVar" does not exist in the current context. |totallyFakeVar|

// bad functions arguments
var concatNotEnough = concat()
//@[28:30) [BCP071 (Error)] Expected as least 1 argument, but got 0. |()|
var padLeftNotEnough = padLeft('s')
//@[30:35) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. |('s')|
var takeTooMany = take([
//@[22:35) [BCP071 (Error)] Expected 2 arguments, but got 4. |([\n],1,2,'s')|
],1,2,'s')

// wrong argument types
var concatWrongTypes = concat({
//@[30:33) [BCP048 (Error)] Cannot resolve function overload.\n  Overload 1 of 2, "(param0: array): array", gave the following error:\n    Argument of type "object" is not assignable to parameter of type "array".\n  Overload 2 of 2, "(param0: bool | int | string): string", gave the following error:\n    Argument of type "object" is not assignable to parameter of type "bool | int | string". |{\n}|
})
var concatWrongTypesContradiction = concat('s', [
//@[48:51) [BCP070 (Error)] Argument of type "array" is not assignable to parameter of type "bool | int | string". |[\n]|
])
var indexOfWrongTypes = indexOf(1,1)
//@[32:34) [BCP070 (Error)] Argument of type "int" is not assignable to parameter of type "string". |1,|

// not enough params
var test1 = listKeys('abcd')
//@[20:28) [BCP071 (Error)] Expected 2 to 3 arguments, but got 1. |('abcd')|

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')
//@[12:20) [BCP082 (Error)] The name "lsitKeys" does not exist in the current context. Did you mean "listKeys"? |lsitKeys|

// just 'list' 
var test3 = list('abcd', '2020-01-01')
//@[12:16) [BCP082 (Error)] The name "list" does not exist in the current context. Did you mean "last"? |list|

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
//@[31:37) [BCP053 (Error)] The type "object" does not contain property "myFake". Available properties include "myArr", "myBool", "myInner", "myInt", "myNull", "myStr". |myFake|
var badSpelling = sampleObject.myNul
//@[31:36) [BCP083 (Error)] The type "object" does not contain property "myNul". Did you mean "myNull"? |myNul|
var badPropertyIndexer = sampleObject['fake']
//@[38:44) [BCP053 (Error)] The type "object" does not contain property "fake". Available properties include "myArr", "myBool", "myInner", "myInt", "myNull", "myStr". |'fake'|
var badType = sampleObject.myStr / 32
//@[14:37) [BCP045 (Error)] Cannot apply operator "/" to operands of type "'s'" and "int". |sampleObject.myStr / 32|
var badInnerProperty = sampleObject.myInner.fake
//@[44:48) [BCP053 (Error)] The type "object" does not contain property "fake". Available properties include "anotherStr", "otherArr". |fake|
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[19:54) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'a'" and "int". |sampleObject.myInner.anotherStr + 2|
var badArrayIndexer = sampleObject.myArr['s']
//@[41:44) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". |'s'|
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[57:60) [BCP074 (Error)] Indexing over arrays requires an index of type "int" but the provided index was of type "'s'". |'s'|
var badIndexer = sampleObject.myStr['s']
//@[17:35) [BCP076 (Error)] Cannot index over expression of type "'s'". Arrays or objects are required. |sampleObject.myStr|
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[41:48) [BCP053 (Error)] The type "object" does not contain property "fakeArr". Available properties include "anotherStr", "otherArr". |fakeArr|
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
//@[50:51) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
var invalidInstanceFunctionAccess = a.b.c.bar()
//@[36:37) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
var invalidInstanceFunctionCall = az.az()
//@[37:39) [BCP086 (Error)] The function "az" does not exist in namespace "az". |az|
var invalidPropertyAccessOnAzNamespace = az.az
//@[44:46) [BCP055 (Error)] Cannot access properties of type "namespace". An "object" type is required. |az|
var invalidPropertyAccessOnSysNamespace = sys.az
//@[46:48) [BCP055 (Error)] Cannot access properties of type "namespace". An "object" type is required. |az|
var invalidOperands = 1 + az
//@[22:28) [BCP045 (Error)] Cannot apply operator "+" to operands of type "int" and "namespace". |1 + az|

var partialObject = {
  2: true
//@[2:3) [BCP022 (Error)] Expected a property name at this location. |2|
  +
//@[2:3) [BCP022 (Error)] Expected a property name at this location. |+|
//@[3:3) [BCP018 (Error)] Expected the ":" character at this location. ||
  3 : concat('s')
//@[2:3) [BCP022 (Error)] Expected a property name at this location. |3|
  
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
