
// an int variable
var myInt = 42

// a string variable
var myStr = 'str'
var curliesWithNoInterp = '}{1}{'
var interp1 = 'abc${123}def'
var interp2 = '${123}def'
var interp3 = 'abc${123}'
var interp4 = 'abc${123}${456}jk$l${789}p$'
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
var curliesInInterp = '{${123}{0}${true}}'

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
// #completionTest(25) -> symbolsPlusTypes
var bracketAtBeginning = '[test'
var enclosingBrackets = '[test]'
var emptyJsonArray = '[]'
var interpolatedBrackets = '[${myInt}]'
var nestedBrackets = '[test[]test2]'
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
var bracketStringInExpression = concat('[', '\'test\'',']')

// booleans
var myTruth = true
var myFalsehood = false

var myEmptyObj = { }
var myEmptyArray = [ ]

// object
var myObj = {
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
  'pirates'
  'say'
  'arr'
]

// array with objects
var myArrWithObjects = [
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
})[resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]

var namedPropertyIndexer = {
  foo: 's'
}['foo']

var intIndexer = [
  's'
][0]

var functionOnIndexer1 = concat([
  's'
][0], 's')

var functionOnIndexer2 = concat([
][0], 's')

var functionOnIndexer3 = concat([
][0], any('s'))

var singleQuote = '\''
var myPropertyName = '${singleQuote}foo${singleQuote}'

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
var unusedIntermediateRef = unusedIntermediate.secondaryKey

// previously this was not possible to emit correctly
var previousEmitLimit = [
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

// #completionTest(0) -> declarations

var myVar = 'hello'
var myVar2 = any({
  something: myVar
})
var myVar3 = any(any({
  something: myVar
}))
var myVar4 = length(any(concat('s','a')))

// identifiers can have underscores
var _ = 3
var __ = 10 * _
var _0a_1b = true
var _1_ = _0a_1b || (__ + _ % 2 == 0)