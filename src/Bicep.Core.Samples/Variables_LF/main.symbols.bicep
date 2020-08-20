
// an int variable
var myInt = 42
//@[4:9] Variable myInt

// a string variable
var myStr = 'str'
//@[4:9] Variable myStr
var curliesWithNoInterp = '}{1}{'
//@[4:23] Variable curliesWithNoInterp
var interp1 = 'abc${123}def'
//@[4:11] Variable interp1
var interp2 = '${123}def'
//@[4:11] Variable interp2
var interp3 = 'abc${123}'
//@[4:11] Variable interp3
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[4:11] Variable interp4
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[4:16] Variable doubleInterp
var curliesInInterp = '{${123}{0}${true}}'
//@[4:19] Variable curliesInInterp

// booleans
var myTruth = true
//@[4:11] Variable myTruth
var myFalsehood = false
//@[4:15] Variable myFalsehood

// object
var myObj = {
//@[4:9] Variable myObj
  a: 'a'
  b: -12
  c: true
  d: !true
  list: [
    1
    2
    2+1
    {
      test: 144 > 33 && true || 99 <= 199
    }
    'a' =~ 'b'
  ]
  obj: {
    nested: [
      'hello'
    ]
  }
}

// array
var myArr = [
//@[4:9] Variable myArr
  'pirates'
  'say'
  'arr'
]

// array with objects
var myArrWithObjects = [
//@[4:20] Variable myArrWithObjects
  {
    name: 'one'
    enable: true
  }
  {
    name: 'two'
    enable: false && false || 'two' !~ 'three'
  }
]

var expressionIndexOnAny = any({
//@[4:24] Variable expressionIndexOnAny
})[resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[4:17] Variable anyIndexOnAny

var namedPropertyIndexer = {
//@[4:24] Variable namedPropertyIndexer
}['foo']

var intIndexer = [
//@[4:14] Variable intIndexer
  's'
][0]

var functionOnIndexer1 = concat([
//@[4:22] Variable functionOnIndexer1
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[4:22] Variable functionOnIndexer2
][0], 's')

var functionOnIndexer3 = concat([
//@[4:22] Variable functionOnIndexer3
][0], any('s'))

var singleQuote = '\''
//@[4:15] Variable singleQuote
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[4:18] Variable myPropertyName

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
//@[4:22] Variable unusedIntermediate
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[4:25] Variable unusedIntermediateRef
