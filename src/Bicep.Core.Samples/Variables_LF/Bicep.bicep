
// an int variable
variable myInt = 42

// a string variable
variable myStr = 'str'
variable curliesWithNoInterp = '}{1}{'
variable interp1 = 'abc${123}def'
variable interp2 = '${123}def'
variable interp3 = 'abc${123}'
variable interp4 = 'abc${123}${456}jk$l${789}p$'
variable doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
variable curliesInInterp = '{${123}{0}${true}}'

// booleans
variable myTruth = true
variable myFalsehood = false

// object
variable myObj = {
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
variable myArr = [
  'pirates'
  'say'
  'arr'
]

// array with objects
variable myArrWithObjects = [
  {
    name: 'one'
    enable: true
  }
  {
    name: 'two'
    enable: false && false || 'two' !~ 'three'
  }
]

variable expressionIndexOnAny = any({
})[resourceGroup().location]

variable anyIndexOnAny = any(true)[any(false)]

variable namedPropertyIndexer = {
}['foo']

variable intIndexer = [
  's'
][0]

variable functionOnIndexer1 = concat([
  's'
][0], 's')

variable functionOnIndexer2 = concat([
][0], 's')

variable functionOnIndexer3 = concat([
][0], any('s'))

variable singleQuote = '\''
variable myPropertyName = '${singleQuote}foo${singleQuote}'