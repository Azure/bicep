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
//@[4:7) Variable bad. Type: null. Declaration start char: 0, length: 16
var bad = (null)[0]
//@[4:7) Variable bad. Type: error. Declaration start char: 0, length: 19
var bad = ()
var bad = 

// variables not supported
var x = a + 2
//@[4:5) Variable x. Type: error. Declaration start char: 0, length: 13

// unary NOT
var not = !null
//@[4:7) Variable not. Type: error. Declaration start char: 0, length: 15
var not = !4
//@[4:7) Variable not. Type: error. Declaration start char: 0, length: 12
var not = !'s'
//@[4:7) Variable not. Type: error. Declaration start char: 0, length: 14
var not = ![
//@[4:7) Variable not. Type: error. Declaration start char: 0, length: 14
]
var not = !{
//@[4:7) Variable not. Type: error. Declaration start char: 0, length: 14
}

// unary not chaining will be added in the future
var not = !!!!!!!true

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12

// unary minus
var minus = -true
//@[4:9) Variable minus. Type: error. Declaration start char: 0, length: 17
var minus = -null
//@[4:9) Variable minus. Type: error. Declaration start char: 0, length: 17
var minus = -'s'
//@[4:9) Variable minus. Type: error. Declaration start char: 0, length: 16
var minus = -[
//@[4:9) Variable minus. Type: error. Declaration start char: 0, length: 16
]
var minus = -{
//@[4:9) Variable minus. Type: error. Declaration start char: 0, length: 16
}

// multiplicative
var mod = 's' % true
//@[4:7) Variable mod. Type: error. Declaration start char: 0, length: 20
var mul = true * null
//@[4:7) Variable mul. Type: error. Declaration start char: 0, length: 21
var div = {
//@[4:7) Variable div. Type: error. Declaration start char: 0, length: 19
} / [
]

// additive
var add = null + 's'
//@[4:7) Variable add. Type: error. Declaration start char: 0, length: 20
var sub = true - false
//@[4:7) Variable sub. Type: error. Declaration start char: 0, length: 22

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
//@[4:6) Variable eq. Type: error. Declaration start char: 0, length: 21
var ne = 15 !~ [
//@[4:6) Variable ne. Type: error. Declaration start char: 0, length: 18
]

// relational
var lt = 4 < 's'
//@[4:6) Variable lt. Type: error. Declaration start char: 0, length: 16
var lteq = null <= 10
//@[4:8) Variable lteq. Type: error. Declaration start char: 0, length: 21
var gt = false>[
//@[4:6) Variable gt. Type: error. Declaration start char: 0, length: 18
]
var gteq = {
//@[4:8) Variable gteq. Type: error. Declaration start char: 0, length: 23
} >= false

// logical
var and = null && 'a'
//@[4:7) Variable and. Type: error. Declaration start char: 0, length: 21
var or = 10 || 4
//@[4:6) Variable or. Type: error. Declaration start char: 0, length: 16

// conditional
var ternary = null ? 4 : false
//@[4:11) Variable ternary. Type: error. Declaration start char: 0, length: 30

// complex expressions
var complex = test(2 + 3*4, true || false && null)
//@[4:11) Variable complex. Type: error. Declaration start char: 0, length: 50
var complex = -2 && 3 && !4 && 5
//@[4:11) Variable complex. Type: error. Declaration start char: 0, length: 32
var complex = null ? !4: false
//@[4:11) Variable complex. Type: error. Declaration start char: 0, length: 30
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[4:11) Variable complex. Type: error. Declaration start char: 0, length: 92

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[4:17) Variable nestedTernary. Type: error. Declaration start char: 0, length: 69
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[4:17) Variable nestedTernary. Type: error. Declaration start char: 0, length: 75

// bad array access
var errorInsideArrayAccess = [
//@[4:26) Variable errorInsideArrayAccess. Type: error. Declaration start char: 0, length: 44
  !null
][!0]
var integerIndexOnNonArray = (null)[0]
//@[4:26) Variable integerIndexOnNonArray. Type: error. Declaration start char: 0, length: 38
var stringIndexOnNonObject = 'test'['test']
//@[4:26) Variable stringIndexOnNonObject. Type: error. Declaration start char: 0, length: 43
var malformedStringIndex = {
//@[4:24) Variable malformedStringIndex. Type: error. Declaration start char: 0, length: 40
}['test\e']
var invalidIndexTypeOverAny = any(true)[true]
//@[4:27) Variable invalidIndexTypeOverAny. Type: error. Declaration start char: 0, length: 45
var badIndexOverArray = [][null]
//@[4:21) Variable badIndexOverArray. Type: error. Declaration start char: 0, length: 32
var badIndexOverArray2 = []['s']
//@[4:22) Variable badIndexOverArray2. Type: error. Declaration start char: 0, length: 32
var badIndexOverObj = {}[true]
//@[4:19) Variable badIndexOverObj. Type: error. Declaration start char: 0, length: 30
var badIndexOverObj2 = {}[0]
//@[4:20) Variable badIndexOverObj2. Type: error. Declaration start char: 0, length: 28
var badExpressionIndexer = {}[base64('a')]
//@[4:24) Variable badExpressionIndexer. Type: error. Declaration start char: 0, length: 42

// bad propertyAccess
var dotAccessOnNonObject = true.foo
//@[4:24) Variable dotAccessOnNonObject. Type: error. Declaration start char: 0, length: 35
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[4:33) Variable badExpressionInPropertyAccess. Type: error. Declaration start char: 0, length: 64

var propertyAccessOnVariable = x.foo
//@[4:28) Variable propertyAccessOnVariable. Type: error. Declaration start char: 0, length: 36

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
//@[4:14) Variable funcvarvar. Type: error. Declaration start char: 0, length: 49
param funcvarparam bool = concat
//@[6:18) Parameter funcvarparam. Type: bool. Declaration start char: 0, length: 32
output funcvarout array = padLeft
//@[7:17) Output funcvarout. Type: array. Declaration start char: 0, length: 33

// non-existent function
var fakeFunc = red() + green() * orange()
//@[4:12) Variable fakeFunc. Type: error. Declaration start char: 0, length: 41
param fakeFuncP string {
//@[6:15) Parameter fakeFuncP. Type: string. Declaration start char: 0, length: 44
  default: blue()
}

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')
//@[4:11) Variable fakeVar. Type: error. Declaration start char: 0, length: 41

// bad functions arguments
var concatNotEnough = concat()
//@[4:19) Variable concatNotEnough. Type: error. Declaration start char: 0, length: 30
var padLeftNotEnough = padLeft('s')
//@[4:20) Variable padLeftNotEnough. Type: error. Declaration start char: 0, length: 35
var takeTooMany = take([
//@[4:15) Variable takeTooMany. Type: error. Declaration start char: 0, length: 35
],1,2,'s')

// wrong argument types
var concatWrongTypes = concat({
//@[4:20) Variable concatWrongTypes. Type: error. Declaration start char: 0, length: 34
})
var concatWrongTypesContradiction = concat('s', [
//@[4:33) Variable concatWrongTypesContradiction. Type: error. Declaration start char: 0, length: 52
])
var indexOfWrongTypes = indexOf(1,1)
//@[4:21) Variable indexOfWrongTypes. Type: error. Declaration start char: 0, length: 36

// not enough params
var test1 = listKeys('abcd')
//@[4:9) Variable test1. Type: error. Declaration start char: 0, length: 28

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')
//@[4:9) Variable test2. Type: error. Declaration start char: 0, length: 42

// just 'list' 
var test3 = list('abcd', '2020-01-01')
//@[4:9) Variable test3. Type: error. Declaration start char: 0, length: 38

// cannot compile an expression like this
var emitLimit = [
//@[4:13) Variable emitLimit. Type: array. Declaration start char: 0, length: 315
  concat('s')
  '${4}'
  {
    a: {
      b: base64('s')
      c: union({
        a: 12 + 3
      }, {
        b: !true
        c: 'hello'
      })
      d: resourceGroup().location
      e: union({
        x: true
      }, {})
      f: intersection({
        q: 's' == 12
      }, {})
    }
  }
]

// cannot compile an expression like this
var emitLimit2 = {
//@[4:14) Variable emitLimit2. Type: error. Declaration start char: 0, length: 115
  a: {
    b: {
      a: resourceGroup().location
    } == 2
    c: concat([

    ], true)
  }
}

var sampleObject = {
//@[4:16) Variable sampleObject. Type: object. Declaration start char: 0, length: 190
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
//@[4:15) Variable badProperty. Type: error. Declaration start char: 0, length: 37
var badPropertyIndexer = sampleObject['fake']
//@[4:22) Variable badPropertyIndexer. Type: error. Declaration start char: 0, length: 45
var badType = sampleObject.myStr / 32
//@[4:11) Variable badType. Type: error. Declaration start char: 0, length: 37
var badInnerProperty = sampleObject.myInner.fake
//@[4:20) Variable badInnerProperty. Type: error. Declaration start char: 0, length: 48
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[4:16) Variable badInnerType. Type: error. Declaration start char: 0, length: 54
var badArrayIndexer = sampleObject.myArr['s']
//@[4:19) Variable badArrayIndexer. Type: error. Declaration start char: 0, length: 45
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[4:24) Variable badInnerArrayIndexer. Type: error. Declaration start char: 0, length: 61
var badIndexer = sampleObject.myStr['s']
//@[4:14) Variable badIndexer. Type: error. Declaration start char: 0, length: 40
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[4:17) Variable badInnerArray. Type: error. Declaration start char: 0, length: 53
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
//@[4:47) Variable invalidPropertyCallOnInstanceFunctionAccess. Type: error. Declaration start char: 0, length: 65
var invalidInstanceFunctionAccess = a.b.c.bar()
//@[4:33) Variable invalidInstanceFunctionAccess. Type: error. Declaration start char: 0, length: 47

