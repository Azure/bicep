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
var bad = (null) !
var bad = (null)[0]
var bad = ()
var bad = 

// variables not supported
var x = a + 2

// unary NOT
var not = !null
var not = !4
var not = !'s'
var not = ![]
var not = !{}

// unary not chaining will be added in the future
var not = !!!!!!!true

// unary minus chaining will not be supported (to reserve -- in case we need it)
var minus = ------12

// unary minus
var minus = -true
var minus = -null
var minus = -'s'
var minus = -[]
var minus = -{}

// multiplicative
var mod = 's' % true
var mul = true * null
var div = {} / []

// additive
var add = null + 's'
var sub = true - false

// equality (== and != can't have a type error because they work on "any" type)
var eq = true =~ null
var ne = 15 !~ []

// relational
var lt = 4 < 's'
var lteq = null <= 10
var gt = false > []
var gteq = {} >= false

// logical
var and = null && 'a'
var or = 10 || 4

// conditional
var ternary = null ? 4 : false

// complex expressions
var complex = test(2 + 3 * 4, true || false && null)
var complex = -2 && 3 && !4 && 5
var complex = null ? !4 : false
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null

var nestedTernary = null ? 1 : 2 ? true ? 'a' : 'b' : false ? 'd' : 15
var nestedTernary = (null ? 1 : 2) ? (true ? 'a' : 'b') : (false ? 'd' : 15)

// bad array access
var errorInsideArrayAccess = [
  !null
][!0]
var integerIndexOnNonArray = (null)[0]
var stringIndexOnNonObject = 'test'['test']
var malformedStringIndex = {}[ 'test\e']
var invalidIndexTypeOverAny = any(true)[true]
var badIndexOverArray = [][null]
var badIndexOverArray2 = []['s']
var badIndexOverObj = {}[true]
var badIndexOverObj2 = {}[0]
var badExpressionIndexer = {}[base64('a')]

// bad propertyAccess
var dotAccessOnNonObject = true.foo
var badExpressionInPropertyAccess = resourceGroup()[!'location']

var propertyAccessOnVariable = x.foo

// missing property in property access
var oneValidDeclaration = {}
var missingPropertyName = oneValidDeclaration.
var missingPropertyInsideAnExpression = oneValidDeclaration. + oneValidDeclaration.

// function used like a variable
var funcvarvar = concat + base64 || !uniqueString
param funcvarparam bool = concat
output funcvarout array = padLeft

// non-existent function
var fakeFunc = red() + green() * orange()
param fakeFuncP string = blue()

// non-existent variable
var fakeVar = concat(totallyFakeVar, 's')

// bad functions arguments
var concatNotEnough = concat()
var padLeftNotEnough = padLeft('s')
var takeTooMany = take([], 1, 2, 's')

// missing arguments
var trailingArgumentComma = format('s',)
var onlyArgumentComma = concat(,)
var multipleArgumentCommas = concat(,,,,,)
var emptyArgInBetween = concat(true,,false)
var leadingEmptyArg = concat(,[])
var leadingAndTrailingEmptyArg = concat(,'s',)

// wrong argument types
var concatWrongTypes = concat({})
var concatWrongTypesContradiction = concat('s', [])
var indexOfWrongTypes = indexOf(1, 1)

// not enough params
var test1 = listKeys('abcd')

// list spelled wrong 
var test2 = lsitKeys('abcd', '2020-01-01')

// just 'lis' instead of 'list'
var test3 = lis('abcd', '2020-01-01')

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
var badSpelling = sampleObject.myNul
var badPropertyIndexer = sampleObject['fake']
var badType = sampleObject.myStr / 32
var badInnerProperty = sampleObject.myInner.fake
var badInnerType = sampleObject.myInner.anotherStr + 2
var badArrayIndexer = sampleObject.myArr['s']
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
var badIndexer = sampleObject.myStr['s']
var badInnerArray = sampleObject.myInner.fakeArr['s']
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
var invalidInstanceFunctionAccess = a.b.c.bar()
var invalidInstanceFunctionCall = az.az()
var invalidPropertyAccessOnAzNamespace = az.az
var invalidPropertyAccessOnSysNamespace = sys.az
var invalidOperands = 1 + az

var bannedFunctions = {
  var: variables()
  param: parameters() + 2
  if: sys.if(null, null)
  obj: sys.createArray()
  arr: sys.createObject()
  numeric: sys.add(1) + sys.sub(2, 3) + sys.mul(8, 's') + sys.div(true) + sys.mod(null, false)
  relational: sys.less() && sys.lessOrEquals() && sys.greater() && sys.greaterOrEquals()
  equals: sys.equals()
  bool: sys.not() || sys.and() || sys.or()
}

// we can get function completions from namespaces
// #completionTest(22) -> azFunctions
var azFunctions = az.a
// #completionTest(24) -> sysFunctions
var sysFunctions = sys.a

// #completionTest(33) -> sysFunctions
var sysFunctionsInParens = (sys.a)

// missing method name
var missingMethodName = az.()

// missing indexer
var missingIndexerOnLiteralArray = [][][]
var missingIndexerOnIdentifier = nonExistentIdentifier[][1][]

// empty parens - should produce expected expression diagnostic
var emptyParens = ()

// #completionTest(26) -> symbols
var anotherEmptyParens = ()

// keywords can't be called like functions
var nullness = null ()
var truth = true ()
var falsehood = false ()

var partialObject = {
  2: true
  +
  3 : concat('s')
  
  's' 
  's' \
  'e'   =
  's' :

  a
  b $
  a # 22
  c :
  d  : %
}

// dangling decorators - to make sure the tests work, please do not add contents after this line
@concat()
@sys.secure()
xxxxx

@minLength()
