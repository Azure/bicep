/*
  This tests the various cases of invalid expressions.
*/

// bad expressions
var bad = a+
var bad = *
var bad = /
var bad = %
var bad = 33-
var bad = --33
var bad = 3 * 4 /
var bad = 222222222222222222222222222222222222222222 * 4
var bad = (null) ?
var bad = (null) ? :
var bad = (null) ? !
var bad = (null)!
var bad = (null)[0]
//@[4:7] Variable bad
var bad = ()
var bad = {}
var bad = []
var bad = 

// variables not supported
var x = a + 2
//@[4:5] Variable x

// unary NOT
var not = !null
//@[4:7] Variable not
var not = !4
//@[4:7] Variable not
var not = !'s'
//@[4:7] Variable not
var not = ![
//@[4:7] Variable not
]
var not = !{
//@[4:7] Variable not
}

// unary not chaining will be added in the future
var not = !!!!!!!true

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12

// unary minus
var minus = -true
//@[4:9] Variable minus
var minus = -null
//@[4:9] Variable minus
var minus = -'s'
//@[4:9] Variable minus
var minus = -[
//@[4:9] Variable minus
]
var minus = -{
//@[4:9] Variable minus
}

// multiplicative
var mod = 's' % true
//@[4:7] Variable mod
var mul = true * null
//@[4:7] Variable mul
var div = {
//@[4:7] Variable div
} / [
]

// additive
var add = null + 's'
//@[4:7] Variable add
var sub = true - false
//@[4:7] Variable sub

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
//@[4:6] Variable eq
var ne = 15 !~ [
//@[4:6] Variable ne
]

// relational
var lt = 4 < 's'
//@[4:6] Variable lt
var lteq = null <= 10
//@[4:8] Variable lteq
var gt = false>[
//@[4:6] Variable gt
]
var gteq = {
//@[4:8] Variable gteq
} >= false

// logical
var and = null && 'a'
//@[4:7] Variable and
var or = 10 || 4
//@[4:6] Variable or

// conditional
var ternary = null ? 4 : false
//@[4:11] Variable ternary

// complex expressions
var complex = test(2 + 3*4, true || false && null)
//@[4:11] Variable complex
var complex = -2 && 3 && !4 && 5
//@[4:11] Variable complex
var complex = null ? !4: false
//@[4:11] Variable complex
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[4:11] Variable complex

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[4:17] Variable nestedTernary
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[4:17] Variable nestedTernary

// bad array access
var errorInsideArrayAccess = [
//@[4:26] Variable errorInsideArrayAccess
  !null
][!0]
var integerIndexOnNonArray = (null)[0]
//@[4:26] Variable integerIndexOnNonArray
var stringIndexOnNonObject = 'test'['test']
//@[4:26] Variable stringIndexOnNonObject
var malformedStringIndex = {
//@[4:24] Variable malformedStringIndex
}['test\e']

// bad propertyAccess
var dotAccessOnNonObject = true.foo
//@[4:24] Variable dotAccessOnNonObject
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[4:33] Variable badExpressionInPropertyAccess

var propertyAccessOnVariable = x.foo
//@[4:28] Variable propertyAccessOnVariable

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
//@[4:14] Variable funcvarvar
param funcvarparam bool = concat
//@[6:18] Parameter funcvarparam
output funcvarout array = padLeft
//@[7:17] Output funcvarout

// non-existent function
var fakeFunc = red() + green() * orange()
//@[4:12] Variable fakeFunc
param fakeFuncP string {
//@[6:15] Parameter fakeFuncP
  default: blue()
}

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')
//@[4:11] Variable fakeVar

// bad functions arguments
var concatNotEnough = concat()
//@[4:19] Variable concatNotEnough
var padLeftNotEnough = padLeft('s')
//@[4:20] Variable padLeftNotEnough
var takeTooMany = take([
//@[4:15] Variable takeTooMany
],1,2,'s')

// wrong argument types
var concatWrongTypes = concat({
//@[4:20] Variable concatWrongTypes
})
var concatWrongTypesContradiction = concat('s', [
//@[4:33] Variable concatWrongTypesContradiction
])
var indexOfWrongTypes = indexOf(1,1)
//@[4:21] Variable indexOfWrongTypes

// not enough params
var test1 = listKeys('abcd')
//@[4:9] Variable test1

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')
//@[4:9] Variable test2

// just 'list' 
var test3 = list('abcd', '2020-01-01')
//@[4:9] Variable test3
