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
//@[4:7) Variable bad. Declaration start char: 0, length: 20
var bad = ()
var bad = 

// variables not supported
var x = a + 2
//@[4:5) Variable x. Declaration start char: 0, length: 15

// unary NOT
var not = !null
//@[4:7) Variable not. Declaration start char: 0, length: 16
var not = !4
//@[4:7) Variable not. Declaration start char: 0, length: 13
var not = !'s'
//@[4:7) Variable not. Declaration start char: 0, length: 15
var not = ![
//@[4:7) Variable not. Declaration start char: 0, length: 15
]
var not = !{
//@[4:7) Variable not. Declaration start char: 0, length: 16
}

// unary not chaining will be added in the future
var not = !!!!!!!true

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12

// unary minus
var minus = -true
//@[4:9) Variable minus. Declaration start char: 0, length: 18
var minus = -null
//@[4:9) Variable minus. Declaration start char: 0, length: 18
var minus = -'s'
//@[4:9) Variable minus. Declaration start char: 0, length: 17
var minus = -[
//@[4:9) Variable minus. Declaration start char: 0, length: 17
]
var minus = -{
//@[4:9) Variable minus. Declaration start char: 0, length: 18
}

// multiplicative
var mod = 's' % true
//@[4:7) Variable mod. Declaration start char: 0, length: 21
var mul = true * null
//@[4:7) Variable mul. Declaration start char: 0, length: 22
var div = {
//@[4:7) Variable div. Declaration start char: 0, length: 21
} / [
]

// additive
var add = null + 's'
//@[4:7) Variable add. Declaration start char: 0, length: 21
var sub = true - false
//@[4:7) Variable sub. Declaration start char: 0, length: 24

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
//@[4:6) Variable eq. Declaration start char: 0, length: 22
var ne = 15 !~ [
//@[4:6) Variable ne. Declaration start char: 0, length: 20
]

// relational
var lt = 4 < 's'
//@[4:6) Variable lt. Declaration start char: 0, length: 17
var lteq = null <= 10
//@[4:8) Variable lteq. Declaration start char: 0, length: 22
var gt = false>[
//@[4:6) Variable gt. Declaration start char: 0, length: 19
]
var gteq = {
//@[4:8) Variable gteq. Declaration start char: 0, length: 25
} >= false

// logical
var and = null && 'a'
//@[4:7) Variable and. Declaration start char: 0, length: 22
var or = 10 || 4
//@[4:6) Variable or. Declaration start char: 0, length: 18

// conditional
var ternary = null ? 4 : false
//@[4:11) Variable ternary. Declaration start char: 0, length: 32

// complex expressions
var complex = test(2 + 3*4, true || false && null)
//@[4:11) Variable complex. Declaration start char: 0, length: 51
var complex = -2 && 3 && !4 && 5
//@[4:11) Variable complex. Declaration start char: 0, length: 33
var complex = null ? !4: false
//@[4:11) Variable complex. Declaration start char: 0, length: 31
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[4:11) Variable complex. Declaration start char: 0, length: 94

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[4:17) Variable nestedTernary. Declaration start char: 0, length: 70
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[4:17) Variable nestedTernary. Declaration start char: 0, length: 77

// bad array access
var errorInsideArrayAccess = [
//@[4:26) Variable errorInsideArrayAccess. Declaration start char: 0, length: 45
  !null
][!0]
var integerIndexOnNonArray = (null)[0]
//@[4:26) Variable integerIndexOnNonArray. Declaration start char: 0, length: 39
var stringIndexOnNonObject = 'test'['test']
//@[4:26) Variable stringIndexOnNonObject. Declaration start char: 0, length: 44
var malformedStringIndex = {
//@[4:24) Variable malformedStringIndex. Declaration start char: 0, length: 42
}['test\e']

// bad propertyAccess
var dotAccessOnNonObject = true.foo
//@[4:24) Variable dotAccessOnNonObject. Declaration start char: 0, length: 36
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[4:33) Variable badExpressionInPropertyAccess. Declaration start char: 0, length: 66

var propertyAccessOnVariable = x.foo
//@[4:28) Variable propertyAccessOnVariable. Declaration start char: 0, length: 38

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
//@[4:14) Variable funcvarvar. Declaration start char: 0, length: 50
param funcvarparam bool = concat
//@[6:18) Parameter funcvarparam. Declaration start char: 0, length: 33
output funcvarout array = padLeft
//@[7:17) Output funcvarout. Declaration start char: 0, length: 35

// non-existent function
var fakeFunc = red() + green() * orange()
//@[4:12) Variable fakeFunc. Declaration start char: 0, length: 42
param fakeFuncP string {
//@[6:15) Parameter fakeFuncP. Declaration start char: 0, length: 46
  default: blue()
}

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')
//@[4:11) Variable fakeVar. Declaration start char: 0, length: 43

// bad functions arguments
var concatNotEnough = concat()
//@[4:19) Variable concatNotEnough. Declaration start char: 0, length: 31
var padLeftNotEnough = padLeft('s')
//@[4:20) Variable padLeftNotEnough. Declaration start char: 0, length: 36
var takeTooMany = take([
//@[4:15) Variable takeTooMany. Declaration start char: 0, length: 37
],1,2,'s')

// wrong argument types
var concatWrongTypes = concat({
//@[4:20) Variable concatWrongTypes. Declaration start char: 0, length: 35
})
var concatWrongTypesContradiction = concat('s', [
//@[4:33) Variable concatWrongTypesContradiction. Declaration start char: 0, length: 53
])
var indexOfWrongTypes = indexOf(1,1)
//@[4:21) Variable indexOfWrongTypes. Declaration start char: 0, length: 38

// not enough params
var test1 = listKeys('abcd')
//@[4:9) Variable test1. Declaration start char: 0, length: 30

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')
//@[4:9) Variable test2. Declaration start char: 0, length: 44

// just 'list' 
var test3 = list('abcd', '2020-01-01')
//@[4:9) Variable test3. Declaration start char: 0, length: 40

// cannot compile an expression like this
var emitLimit = [
//@[4:13) Variable emitLimit. Declaration start char: 0, length: 290
  concat('s')
  '${4}'
  {
    a: {
      b: base64('s')
      c: concat([
        12 + 3
      ], [
        !true
        'hello'
      ])
      d: resourceGroup().location
      e: concat([
        true
      ])
      f: concat([
        's' == 12
      ])
    }
  }
]

// cannot compile an expression like this
var emitLimit2 = {
//@[4:14) Variable emitLimit2. Declaration start char: 0, length: 115
  a: {
    b: {
      a: resourceGroup().location
    } == 2
    c: concat([

    ], true)
  }
}
