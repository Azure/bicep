/*
  This tests the various cases of invalid expressions.
*/

// bad expressions
var bad = a+
//@[12:12) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = *
//@[10:11) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |*|
var bad = /
//@[10:11) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |/|
var bad = %
//@[10:11) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |%|
var bad = 33-
//@[13:13) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = --33
//@[11:12) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |-|
var bad = 3 * 4 /
//@[17:17) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = 222222222222222222222222222222222222222222 * 4
//@[10:52) Error Expected a valid 32-bit signed integer. |222222222222222222222222222222222222222222|
var bad = (null) ?
//@[18:18) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = (null) ? :
//@[19:20) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |:|
var bad = (null) ? !
//@[20:20) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
var bad = (null)!
//@[16:17) Error Expected a new line character at this location. |!|
var bad = (null)[0]
//@[10:16) Error Cannot index over expression of type 'null'. Arrays or objects are required. |(null)|
var bad = ()
//@[11:12) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |)|
var bad = 
//@[10:10) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// variables not supported
var x = a + 2
//@[8:9) Error The name 'a' does not exist in the current context. |a|

// unary NOT
var not = !null
//@[4:7) Error Identifier 'not' is declared multiple times. Remove or rename the duplicates. |not|
//@[10:15) Error Cannot apply operator '!' to operand of type null. |!null|
var not = !4
//@[4:7) Error Identifier 'not' is declared multiple times. Remove or rename the duplicates. |not|
//@[10:12) Error Cannot apply operator '!' to operand of type int. |!4|
var not = !'s'
//@[4:7) Error Identifier 'not' is declared multiple times. Remove or rename the duplicates. |not|
//@[10:14) Error Cannot apply operator '!' to operand of type 's'. |!'s'|
var not = ![
//@[4:7) Error Identifier 'not' is declared multiple times. Remove or rename the duplicates. |not|
//@[10:14) Error Cannot apply operator '!' to operand of type array. |![\n]|
]
var not = !{
//@[4:7) Error Identifier 'not' is declared multiple times. Remove or rename the duplicates. |not|
//@[10:14) Error Cannot apply operator '!' to operand of type object. |!{\n}|
}

// unary not chaining will be added in the future
var not = !!!!!!!true
//@[11:12) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |!|

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12
//@[13:14) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |-|

// unary minus
var minus = -true
//@[4:9) Error Identifier 'minus' is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:17) Error Cannot apply operator '-' to operand of type bool. |-true|
var minus = -null
//@[4:9) Error Identifier 'minus' is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:17) Error Cannot apply operator '-' to operand of type null. |-null|
var minus = -'s'
//@[4:9) Error Identifier 'minus' is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:16) Error Cannot apply operator '-' to operand of type 's'. |-'s'|
var minus = -[
//@[4:9) Error Identifier 'minus' is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:16) Error Cannot apply operator '-' to operand of type array. |-[\n]|
]
var minus = -{
//@[4:9) Error Identifier 'minus' is declared multiple times. Remove or rename the duplicates. |minus|
//@[12:16) Error Cannot apply operator '-' to operand of type object. |-{\n}|
}

// multiplicative
var mod = 's' % true
//@[10:20) Error Cannot apply operator '%' to operands of type 's' and bool. |'s' % true|
var mul = true * null
//@[10:21) Error Cannot apply operator '*' to operands of type bool and null. |true * null|
var div = {
//@[10:19) Error Cannot apply operator '/' to operands of type object and array. |{\n} / [\n]|
} / [
]

// additive
var add = null + 's'
//@[10:20) Error Cannot apply operator '+' to operands of type null and 's'. |null + 's'|
var sub = true - false
//@[10:22) Error Cannot apply operator '-' to operands of type bool and bool. |true - false|

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
//@[9:21) Error Cannot apply operator '=~' to operands of type bool and null. |true =~ null|
var ne = 15 !~ [
//@[9:18) Error Cannot apply operator '!~' to operands of type int and array. |15 !~ [\n]|
]

// relational
var lt = 4 < 's'
//@[9:16) Error Cannot apply operator '<' to operands of type int and 's'. |4 < 's'|
var lteq = null <= 10
//@[11:21) Error Cannot apply operator '<=' to operands of type null and int. |null <= 10|
var gt = false>[
//@[9:18) Error Cannot apply operator '>' to operands of type bool and array. |false>[\n]|
]
var gteq = {
//@[11:23) Error Cannot apply operator '>=' to operands of type object and bool. |{\n} >= false|
} >= false

// logical
var and = null && 'a'
//@[10:21) Error Cannot apply operator '&&' to operands of type null and 'a'. |null && 'a'|
var or = 10 || 4
//@[9:16) Error Cannot apply operator '||' to operands of type int and int. |10 || 4|

// conditional
var ternary = null ? 4 : false
//@[14:18) Error Expected a value of type bool. |null|

// complex expressions
var complex = test(2 + 3*4, true || false && null)
//@[4:11) Error Identifier 'complex' is declared multiple times. Remove or rename the duplicates. |complex|
//@[14:18) Error The name 'test' does not exist in the current context. |test|
//@[36:49) Error Cannot apply operator '&&' to operands of type bool and null. |false && null|
var complex = -2 && 3 && !4 && 5
//@[4:11) Error Identifier 'complex' is declared multiple times. Remove or rename the duplicates. |complex|
//@[14:21) Error Cannot apply operator '&&' to operands of type int and int. |-2 && 3|
//@[25:27) Error Cannot apply operator '!' to operand of type int. |!4|
var complex = null ? !4: false
//@[4:11) Error Identifier 'complex' is declared multiple times. Remove or rename the duplicates. |complex|
//@[21:23) Error Cannot apply operator '!' to operand of type int. |!4|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[4:11) Error Identifier 'complex' is declared multiple times. Remove or rename the duplicates. |complex|
//@[50:57) Error Cannot apply operator '&&' to operands of type int and int. |-2 && 3|
//@[61:63) Error Cannot apply operator '!' to operand of type int. |!4|
//@[79:92) Error Cannot apply operator '&&' to operands of type bool and null. |false && null|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[4:17) Error Identifier 'nestedTernary' is declared multiple times. Remove or rename the duplicates. |nestedTernary|
//@[31:32) Error Expected a value of type bool. |2|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[4:17) Error Identifier 'nestedTernary' is declared multiple times. Remove or rename the duplicates. |nestedTernary|
//@[21:25) Error Expected a value of type bool. |null|

// bad array access
var errorInsideArrayAccess = [
  !null
//@[2:7) Error Cannot apply operator '!' to operand of type null. |!null|
][!0]
//@[2:4) Error Cannot apply operator '!' to operand of type int. |!0|
var integerIndexOnNonArray = (null)[0]
//@[29:35) Error Cannot index over expression of type 'null'. Arrays or objects are required. |(null)|
var stringIndexOnNonObject = 'test'['test']
//@[29:35) Error Cannot index over expression of type ''test''. Arrays or objects are required. |'test'|
var malformedStringIndex = {
}['test\e']
//@[7:9) Error The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: \$, \', \\, \n, \r, \t. |\e|
var invalidIndexTypeOverAny = any(true)[true]
//@[40:44) Error The array index must be of type string or int but the provided index was of type bool. |true|
var badIndexOverArray = [][null]
//@[27:31) Error Indexing over arrays requires an index of type 'int' but the provided index was of type 'null'. |null|
var badIndexOverArray2 = []['s']
//@[28:31) Error Indexing over arrays requires an index of type 'int' but the provided index was of type ''s''. |'s'|
var badIndexOverObj = {}[true]
//@[25:29) Error Indexing over objects requires an index of type 'string' but the provided index was of type 'bool'. |true|
var badIndexOverObj2 = {}[0]
//@[26:27) Error Indexing over objects requires an index of type 'string' but the provided index was of type 'int'. |0|
var badExpressionIndexer = {}[base64('a')]
//@[30:41) Error The type object does not contain any properties. |base64('a')|

// bad propertyAccess
var dotAccessOnNonObject = true.foo
//@[32:35) Error Cannot access properties of type bool. An object type is required. |foo|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[52:63) Error Cannot apply operator '!' to operand of type 'location'. |!'location'|

var propertyAccessOnVariable = x.foo
//@[31:32) Error The referenced declaration with name 'x' is not valid. |x|

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
//@[17:23) Error The name 'concat' is not a parameter or variable. |concat|
//@[26:32) Error The name 'base64' is not a parameter or variable. |base64|
//@[37:49) Error The name 'uniqueString' is not a parameter or variable. |uniqueString|
param funcvarparam bool = concat
//@[26:32) Error The name 'concat' is not a parameter or variable. |concat|
output funcvarout array = padLeft
//@[26:33) Error The name 'padLeft' is not a parameter or variable. |padLeft|

// non-existent function
var fakeFunc = red() + green() * orange()
//@[15:18) Error The name 'red' does not exist in the current context. |red|
//@[23:28) Error The name 'green' does not exist in the current context. |green|
//@[33:39) Error The name 'orange' does not exist in the current context. |orange|
param fakeFuncP string {
  default: blue()
//@[11:15) Error The name 'blue' does not exist in the current context. |blue|
}

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')
//@[21:35) Error The name 'totallyFakeVar' does not exist in the current context. |totallyFakeVar|

// bad functions arguments
var concatNotEnough = concat()
//@[28:30) Error Expected as least 1 argument, but got 0. |()|
var padLeftNotEnough = padLeft('s')
//@[30:35) Error Expected 2 to 3 arguments, but got 1. |('s')|
var takeTooMany = take([
//@[22:35) Error Expected 2 arguments, but got 4. |([\n],1,2,'s')|
],1,2,'s')

// wrong argument types
var concatWrongTypes = concat({
//@[30:33) Error Cannot resolve function overload.\n  Overload 1 of 2, '(param0: array): array', gave the following error:\n    Argument of type object is not assignable to parameter of type array.\n  Overload 2 of 2, '(param0: bool | int | string): string', gave the following error:\n    Argument of type object is not assignable to parameter of type bool | int | string. |{\n}|
})
var concatWrongTypesContradiction = concat('s', [
//@[48:51) Error Argument of type array is not assignable to parameter of type bool | int | string. |[\n]|
])
var indexOfWrongTypes = indexOf(1,1)
//@[32:34) Error Argument of type int is not assignable to parameter of type string. |1,|

// not enough params
var test1 = listKeys('abcd')
//@[20:28) Error Expected 2 to 3 arguments, but got 1. |('abcd')|

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')
//@[12:20) Error The name 'lsitKeys' does not exist in the current context. |lsitKeys|

// just 'list' 
var test3 = list('abcd', '2020-01-01')
//@[12:16) Error The name 'list' does not exist in the current context. |list|

// cannot compile an expression like this
var emitLimit = [
  concat('s')
  '${4}'
  {
    a: {
      b: base64('s')
      c: union({
        a: 12 + 3
//@[11:17) Error The expression is inside an object literal that is itself part of another expression. This is not currently supported. |12 + 3|
      }, {
        b: !true
//@[11:16) Error The expression is inside an object literal that is itself part of another expression. This is not currently supported. |!true|
        c: 'hello'
      })
      d: resourceGroup().location
      e: union({
        x: true
      }, {})
      f: intersection({
        q: 's' == 12
//@[11:20) Error The expression is inside an object literal that is itself part of another expression. This is not currently supported. |'s' == 12|
      }, {})
    }
  }
]

// cannot compile an expression like this
var emitLimit2 = {
  a: {
    b: {
      a: resourceGroup().location
//@[9:33) Error The expression is inside an object literal that is itself part of another expression. This is not currently supported. |resourceGroup().location|
    } == 2
    c: concat([

    ], true)
//@[7:11) Error Argument of type bool is not assignable to parameter of type array. |true|
  }
}

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
//@[31:37) Error The type object does not contain property 'myFake'. |myFake|
var badPropertyIndexer = sampleObject['fake']
//@[38:44) Error The type object does not contain property 'fake'. |'fake'|
var badType = sampleObject.myStr / 32
//@[14:37) Error Cannot apply operator '/' to operands of type 's' and int. |sampleObject.myStr / 32|
var badInnerProperty = sampleObject.myInner.fake
//@[44:48) Error The type object does not contain property 'fake'. |fake|
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[19:54) Error Cannot apply operator '+' to operands of type 'a' and int. |sampleObject.myInner.anotherStr + 2|
var badArrayIndexer = sampleObject.myArr['s']
//@[41:44) Error Indexing over arrays requires an index of type 'int' but the provided index was of type ''s''. |'s'|
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[57:60) Error Indexing over arrays requires an index of type 'int' but the provided index was of type ''s''. |'s'|
var badIndexer = sampleObject.myStr['s']
//@[17:35) Error Cannot index over expression of type ''s''. Arrays or objects are required. |sampleObject.myStr|
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[41:48) Error The type object does not contain property 'fakeArr'. |fakeArr|
