
// an int variable
var myInt = 42
//@[4:9) Variable myInt. Type: int. Declaration start char: 0, length: 16

// a string variable
var myStr = 'str'
//@[4:9) Variable myStr. Type: 'str'. Declaration start char: 0, length: 18
var curliesWithNoInterp = '}{1}{'
//@[4:23) Variable curliesWithNoInterp. Type: '}{1}{'. Declaration start char: 0, length: 34
var interp1 = 'abc${123}def'
//@[4:11) Variable interp1. Type: string. Declaration start char: 0, length: 29
var interp2 = '${123}def'
//@[4:11) Variable interp2. Type: string. Declaration start char: 0, length: 26
var interp3 = 'abc${123}'
//@[4:11) Variable interp3. Type: string. Declaration start char: 0, length: 26
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[4:11) Variable interp4. Type: string. Declaration start char: 0, length: 44
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[4:16) Variable doubleInterp. Type: string. Declaration start char: 0, length: 57
var curliesInInterp = '{${123}{0}${true}}'
//@[4:19) Variable curliesInInterp. Type: string. Declaration start char: 0, length: 44

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[4:22) Variable bracketInTheMiddle. Type: 'a[b]'. Declaration start char: 0, length: 32
var bracketAtBeginning = '[test'
//@[4:22) Variable bracketAtBeginning. Type: '[test'. Declaration start char: 0, length: 33
var enclosingBrackets = '[test]'
//@[4:21) Variable enclosingBrackets. Type: '[test]'. Declaration start char: 0, length: 33
var emptyJsonArray = '[]'
//@[4:18) Variable emptyJsonArray. Type: '[]'. Declaration start char: 0, length: 26
var interpolatedBrackets = '[${myInt}]'
//@[4:24) Variable interpolatedBrackets. Type: string. Declaration start char: 0, length: 40
var nestedBrackets = '[test[]test2]'
//@[4:18) Variable nestedBrackets. Type: '[test[]test2]'. Declaration start char: 0, length: 37
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[4:30) Variable nestedInterpolatedBrackets. Type: string. Declaration start char: 0, length: 55
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[4:29) Variable bracketStringInExpression. Type: string. Declaration start char: 0, length: 61

// booleans
var myTruth = true
//@[4:11) Variable myTruth. Type: bool. Declaration start char: 0, length: 19
var myFalsehood = false
//@[4:15) Variable myFalsehood. Type: bool. Declaration start char: 0, length: 25

var myEmptyObj = { }
//@[4:14) Variable myEmptyObj. Type: object. Declaration start char: 0, length: 21
var myEmptyArray = [ ]
//@[4:16) Variable myEmptyArray. Type: array. Declaration start char: 0, length: 24

// object
var myObj = {
//@[4:9) Variable myObj. Type: object. Declaration start char: 0, length: 206
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
//@[4:9) Variable myArr. Type: array. Declaration start char: 0, length: 45
  'pirates'
  'say'
  'arr'
]

// array with objects
var myArrWithObjects = [
//@[4:20) Variable myArrWithObjects. Type: array. Declaration start char: 0, length: 140
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
//@[4:24) Variable expressionIndexOnAny. Type: any. Declaration start char: 0, length: 63
})[resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[4:17) Variable anyIndexOnAny. Type: any. Declaration start char: 0, length: 43

var namedPropertyIndexer = {
//@[4:24) Variable namedPropertyIndexer. Type: 's'. Declaration start char: 0, length: 50
  foo: 's'
}['foo']

var intIndexer = [
//@[4:14) Variable intIndexer. Type: 's'. Declaration start char: 0, length: 31
  's'
][0]

var functionOnIndexer1 = concat([
//@[4:22) Variable functionOnIndexer1. Type: string. Declaration start char: 0, length: 52
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[4:22) Variable functionOnIndexer2. Type: string. Declaration start char: 0, length: 46
][0], 's')

var functionOnIndexer3 = concat([
//@[4:22) Variable functionOnIndexer3. Type: any. Declaration start char: 0, length: 51
][0], any('s'))

var singleQuote = '\''
//@[4:15) Variable singleQuote. Type: '\''. Declaration start char: 0, length: 23
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[4:18) Variable myPropertyName. Type: string. Declaration start char: 0, length: 56

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
//@[4:22) Variable unusedIntermediate. Type: any. Declaration start char: 0, length: 85
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[4:25) Variable unusedIntermediateRef. Type: any. Declaration start char: 0, length: 61

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@[4:21) Variable previousEmitLimit. Type: array. Declaration start char: 0, length: 296
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
