
// an int variable
var myInt = 42
//@[4:9) Variable myInt. Declaration start char: 0, length: 16

// a string variable
var myStr = 'str'
//@[4:9) Variable myStr. Declaration start char: 0, length: 18
var curliesWithNoInterp = '}{1}{'
//@[4:23) Variable curliesWithNoInterp. Declaration start char: 0, length: 34
var interp1 = 'abc${123}def'
//@[4:11) Variable interp1. Declaration start char: 0, length: 29
var interp2 = '${123}def'
//@[4:11) Variable interp2. Declaration start char: 0, length: 26
var interp3 = 'abc${123}'
//@[4:11) Variable interp3. Declaration start char: 0, length: 26
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[4:11) Variable interp4. Declaration start char: 0, length: 44
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[4:16) Variable doubleInterp. Declaration start char: 0, length: 57
var curliesInInterp = '{${123}{0}${true}}'
//@[4:19) Variable curliesInInterp. Declaration start char: 0, length: 44

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[4:22) Variable bracketInTheMiddle. Declaration start char: 0, length: 32
var bracketAtBeginning = '[test'
//@[4:22) Variable bracketAtBeginning. Declaration start char: 0, length: 33
var enclosingBrackets = '[test]'
//@[4:21) Variable enclosingBrackets. Declaration start char: 0, length: 33
var emptyJsonArray = '[]'
//@[4:18) Variable emptyJsonArray. Declaration start char: 0, length: 26
var interpolatedBrackets = '[${myInt}]'
//@[4:24) Variable interpolatedBrackets. Declaration start char: 0, length: 40
var nestedBrackets = '[test[]test2]'
//@[4:18) Variable nestedBrackets. Declaration start char: 0, length: 37
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[4:30) Variable nestedInterpolatedBrackets. Declaration start char: 0, length: 55
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[4:29) Variable bracketStringInExpression. Declaration start char: 0, length: 61

// booleans
var myTruth = true
//@[4:11) Variable myTruth. Declaration start char: 0, length: 19
var myFalsehood = false
//@[4:15) Variable myFalsehood. Declaration start char: 0, length: 25

// object
var myObj = {
//@[4:9) Variable myObj. Declaration start char: 0, length: 206
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
//@[4:9) Variable myArr. Declaration start char: 0, length: 45
  'pirates'
  'say'
  'arr'
]

// array with objects
var myArrWithObjects = [
//@[4:20) Variable myArrWithObjects. Declaration start char: 0, length: 140
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
//@[4:24) Variable expressionIndexOnAny. Declaration start char: 0, length: 63
})[resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[4:17) Variable anyIndexOnAny. Declaration start char: 0, length: 43

var namedPropertyIndexer = {
//@[4:24) Variable namedPropertyIndexer. Declaration start char: 0, length: 39
}['foo']

var intIndexer = [
//@[4:14) Variable intIndexer. Declaration start char: 0, length: 31
  's'
][0]

var functionOnIndexer1 = concat([
//@[4:22) Variable functionOnIndexer1. Declaration start char: 0, length: 52
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[4:22) Variable functionOnIndexer2. Declaration start char: 0, length: 46
][0], 's')

var functionOnIndexer3 = concat([
//@[4:22) Variable functionOnIndexer3. Declaration start char: 0, length: 51
][0], any('s'))

var singleQuote = '\''
//@[4:15) Variable singleQuote. Declaration start char: 0, length: 23
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[4:18) Variable myPropertyName. Declaration start char: 0, length: 56

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
//@[4:22) Variable unusedIntermediate. Declaration start char: 0, length: 85
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[4:25) Variable unusedIntermediateRef. Declaration start char: 0, length: 59
