// int
@sys.description('an int variable')
var myInt = 42

// string
@sys.description('a string variable')
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
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
var emptyJsonArray = '[]'
var interpolatedBrackets = '[${myInt}]'
var nestedBrackets = '[test[]test2]'
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
var bracketStringInExpression = concat('[', '\'test\'', ']')

// booleans
@sys.description('a bool variable')
var myTruth = true
var myFalsehood = false

var myEmptyObj = {}
var myEmptyArray = []

// object
@sys.description('a object variable')
var myObj = {
  a: 'a'
  b: -12
  c: true
  d: !true
  list: [
    1
    2
    2 + 1
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

@sys.description('a object with interp')
var objWithInterp = {
  '${myStr}': 1
  'abc${myStr}def': 2
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
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

var expressionIndexOnAny = any({})[az.resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]

var deploymentName = deployment().name
var templateLinkUri = deployment().properties.templateLink.uri
var templateLinkId = deployment().properties.templateLink.id

var portalEndpoint = environment().portal
var loginEndpoint = environment().authentication.loginEndpoint

var namedPropertyIndexer = {
  foo: 's'
}['foo']

var intIndexer = [
  's'
][0]

var functionOnIndexer1 = concat([
  's'
][0], 's')

var functionOnIndexer2 = concat([][0], 's')

var functionOnIndexer3 = concat([][0], any('s'))

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
      d: az.resourceGroup().location
      e: concat([
        true
      ])
      f: concat([
        's' == 12
      ])
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit2 = [
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
      d: az.resourceGroup().location
      e: union({
        x: true
      }, {})
      f: intersection({
        q: 's' == 12
      }, {})
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit3 = {
  a: {
    b: {
      a: az.resourceGroup().location
    } == 2
    c: concat([], [
      true
    ])
  }
}

// #completionTest(0) -> declarations

var myVar = 'hello'
var myVar2 = any({
  something: myVar
})
var myVar3 = any(any({
  something: myVar
}))
var myVar4 = length(any(concat('s', 'a')))

// verify that unqualified banned function identifiers can be used as declaration identifiers
var variables = true
param parameters bool = true
var if = true
var createArray = true
var createObject = true
var add = true
var sub = true
var mul = true
var div = true
param mod bool = true
var less = true
var lessOrEquals = true
var greater = true
var greaterOrEquals = true
param equals bool = true
var not = true
var and = true
var or = true
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or

// identifiers can have underscores
var _ = 3
var __ = 10 * _
var _0a_1b = true
var _1_ = _0a_1b || (__ + _ % 2 == 0)

// fully qualified access
var resourceGroup = 'something'
var resourceGroupName = az.resourceGroup().name
var resourceGroupObject = az.resourceGroup()
var propertyAccessFromObject = resourceGroupObject.name
var isTrue = sys.max(1, 2) == 3
var isFalse = !isTrue
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
  tenant: tenant()
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
}

// Issue #1332
var issue1332_propname = 'ptest'
var issue1332 = true ? {
  prop1: {
    '${issue1332_propname}': {}
  }
} : {}

// Issue #486
var myBigInt = 2199023255552
var myIntExpression = 5 * 5
var myBigIntExpression = 2199023255552 * 2
var myBigIntExpression2 = 2199023255552 * 2199023255552

// variable loops
var incrementingNumbers = [for i in range(0, 10): i]
var loopInput = [
  'one'
  'two'
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
  index: i
  name: name
  value: 'prefix-${i}-${name}-suffix'
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
  i
  name
  'prefix-${i}-${name}-suffix'
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
var arrayOfHardCodedNumbers = [for i in range(0, 10): 3]
var arrayOfHardCodedBools = [for i in range(0, 10): false]
var arrayOfHardCodedStrings = [for i in range(0, 3): 'hi']
var arrayOfNonRuntimeFunctionCalls = [for i in range(0, 3): concat('hi', i)]

var multilineString = '''
HELLO!
'''

var multilineEmpty = ''''''
var multilineEmptyNewline = '''
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
abc
''''

var multilineSingleLine = '''hello!'''

var multilineFormatted = format('''
Hello,
my
name is
{0}
''', 'Anthony')

var multilineJavaScript = '''
// NOT RECOMMENDED PATTERN
const fs = require('fs');

module.exports = function (context) {
    fs.readFile('./hello.txt', (err, data) => {
        if (err) {
            context.log.error('ERROR', err);
            // BUG #1: This will result in an uncaught exception that crashes the entire process
            throw err;
        }
        context.log(`Data from file: ${data}`);
        // context.done() should be called here
    });
    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends
    context.done();
}
'''
