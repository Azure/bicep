
// int
@sys.description('an int variable')
var myInt = 42
//@[04:09) Variable myInt. Type: 42. Declaration start char: 0, length: 50

// string
@sys.description('a string variable')
var myStr = 'str'
//@[04:09) Variable myStr. Type: 'str'. Declaration start char: 0, length: 55
var curliesWithNoInterp = '}{1}{'
//@[04:23) Variable curliesWithNoInterp. Type: '}{1}{'. Declaration start char: 0, length: 33
var interp1 = 'abc${123}def'
//@[04:11) Variable interp1. Type: string. Declaration start char: 0, length: 28
var interp2 = '${123}def'
//@[04:11) Variable interp2. Type: string. Declaration start char: 0, length: 25
var interp3 = 'abc${123}'
//@[04:11) Variable interp3. Type: string. Declaration start char: 0, length: 25
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[04:11) Variable interp4. Type: string. Declaration start char: 0, length: 43
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[04:16) Variable doubleInterp. Type: string. Declaration start char: 0, length: 56
var curliesInInterp = '{${123}{0}${true}}'
//@[04:19) Variable curliesInInterp. Type: string. Declaration start char: 0, length: 42

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[04:22) Variable bracketInTheMiddle. Type: 'a[b]'. Declaration start char: 0, length: 31
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
//@[04:22) Variable bracketAtBeginning. Type: '[test'. Declaration start char: 0, length: 32
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
//@[04:21) Variable enclosingBrackets. Type: '[test]'. Declaration start char: 0, length: 32
var emptyJsonArray = '[]'
//@[04:18) Variable emptyJsonArray. Type: '[]'. Declaration start char: 0, length: 25
var interpolatedBrackets = '[${myInt}]'
//@[04:24) Variable interpolatedBrackets. Type: string. Declaration start char: 0, length: 39
var nestedBrackets = '[test[]test2]'
//@[04:18) Variable nestedBrackets. Type: '[test[]test2]'. Declaration start char: 0, length: 36
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[04:30) Variable nestedInterpolatedBrackets. Type: string. Declaration start char: 0, length: 54
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[04:29) Variable bracketStringInExpression. Type: string. Declaration start char: 0, length: 59

// booleans
@sys.description('a bool variable')
var myTruth = true
//@[04:11) Variable myTruth. Type: true. Declaration start char: 0, length: 54
var myFalsehood = false
//@[04:15) Variable myFalsehood. Type: false. Declaration start char: 0, length: 23

var myEmptyObj = { }
//@[04:14) Variable myEmptyObj. Type: object. Declaration start char: 0, length: 20
var myEmptyArray = [ ]
//@[04:16) Variable myEmptyArray. Type: []. Declaration start char: 0, length: 22

// object
@sys.description('a object variable')
var myObj = {
//@[04:09) Variable myObj. Type: object. Declaration start char: 0, length: 242
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

@sys.description('a object with interp')
var objWithInterp = {
//@[04:17) Variable objWithInterp. Type: object. Declaration start char: 0, length: 157
  '${myStr}': 1
  'abc${myStr}def': 2
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
}

// array
var myArr = [
//@[04:09) Variable myArr. Type: ['pirates', 'say', 'arr']. Declaration start char: 0, length: 43
  'pirates'
  'say'
  'arr'
]

// array with objects
var myArrWithObjects = [
//@[04:20) Variable myArrWithObjects. Type: [object, object]. Declaration start char: 0, length: 138
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
//@[04:24) Variable expressionIndexOnAny. Type: any. Declaration start char: 0, length: 64
})[az.resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[04:17) Variable anyIndexOnAny. Type: any. Declaration start char: 0, length: 41

var deploymentName = deployment().name
//@[04:18) Variable deploymentName. Type: string. Declaration start char: 0, length: 38
var templateContentVersion = deployment().properties.template.contentVersion
//@[04:26) Variable templateContentVersion. Type: string. Declaration start char: 0, length: 76
var templateLinkUri = deployment().properties.templateLink.uri
//@[04:19) Variable templateLinkUri. Type: string. Declaration start char: 0, length: 62
var templateLinkId = deployment().properties.templateLink.id
//@[04:18) Variable templateLinkId. Type: string. Declaration start char: 0, length: 60

var portalEndpoint = environment().portal
//@[04:18) Variable portalEndpoint. Type: string. Declaration start char: 0, length: 41
var loginEndpoint = environment().authentication.loginEndpoint
//@[04:17) Variable loginEndpoint. Type: string. Declaration start char: 0, length: 62

var namedPropertyIndexer = {
//@[04:24) Variable namedPropertyIndexer. Type: 's'. Declaration start char: 0, length: 48
  foo: 's'
}['foo']

var intIndexer = [
//@[04:14) Variable intIndexer. Type: 's'. Declaration start char: 0, length: 29
  's'
][0]

var functionOnIndexer1 = concat([
//@[04:22) Variable functionOnIndexer1. Type: string. Declaration start char: 0, length: 50
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[04:22) Variable functionOnIndexer2. Type: string. Declaration start char: 0, length: 44
][0], 's')

var functionOnIndexer3 = concat([
//@[04:22) Variable functionOnIndexer3. Type: array. Declaration start char: 0, length: 49
][0], any('s'))

var singleQuote = '\''
//@[04:15) Variable singleQuote. Type: '\''. Declaration start char: 0, length: 22
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[04:18) Variable myPropertyName. Type: string. Declaration start char: 0, length: 54

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
//@[04:22) Variable unusedIntermediate. Type: any. Declaration start char: 0, length: 84
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[04:25) Variable unusedIntermediateRef. Type: any. Declaration start char: 0, length: 59

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@[04:21) Variable previousEmitLimit. Type: [string, string, object]. Declaration start char: 0, length: 299
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
//@[04:22) Variable previousEmitLimit2. Type: [string, string, object]. Declaration start char: 0, length: 327
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
//@[04:22) Variable previousEmitLimit3. Type: object. Declaration start char: 0, length: 140
  a: {
    b: {
      a: az.resourceGroup().location
    } == 2
    c: concat([

    ], [
      true
    ])
  }
}

// #completionTest(0) -> declarations

var myVar = 'hello'
//@[04:09) Variable myVar. Type: 'hello'. Declaration start char: 0, length: 19
var myVar2 = any({
//@[04:10) Variable myVar2. Type: any. Declaration start char: 0, length: 40
  something: myVar
})
var myVar3 = any(any({
//@[04:10) Variable myVar3. Type: any. Declaration start char: 0, length: 45
  something: myVar
}))
var myVar4 = length(any(concat('s','a')))
//@[04:10) Variable myVar4. Type: int. Declaration start char: 0, length: 41

// verify that unqualified banned function identifiers can be used as declaration identifiers
var variables = true
//@[04:13) Variable variables. Type: true. Declaration start char: 0, length: 20
param parameters bool = true
//@[06:16) Parameter parameters. Type: bool. Declaration start char: 0, length: 28
var if = true
//@[04:06) Variable if. Type: true. Declaration start char: 0, length: 13
var createArray = true
//@[04:15) Variable createArray. Type: true. Declaration start char: 0, length: 22
var createObject = true
//@[04:16) Variable createObject. Type: true. Declaration start char: 0, length: 23
var add = true
//@[04:07) Variable add. Type: true. Declaration start char: 0, length: 14
var sub = true
//@[04:07) Variable sub. Type: true. Declaration start char: 0, length: 14
var mul = true
//@[04:07) Variable mul. Type: true. Declaration start char: 0, length: 14
var div = true
//@[04:07) Variable div. Type: true. Declaration start char: 0, length: 14
param mod bool = true
//@[06:09) Parameter mod. Type: bool. Declaration start char: 0, length: 21
var less = true
//@[04:08) Variable less. Type: true. Declaration start char: 0, length: 15
var lessOrEquals = true
//@[04:16) Variable lessOrEquals. Type: true. Declaration start char: 0, length: 23
var greater = true
//@[04:11) Variable greater. Type: true. Declaration start char: 0, length: 18
var greaterOrEquals = true
//@[04:19) Variable greaterOrEquals. Type: true. Declaration start char: 0, length: 26
param equals bool = true
//@[06:12) Parameter equals. Type: bool. Declaration start char: 0, length: 24
var not = true
//@[04:07) Variable not. Type: true. Declaration start char: 0, length: 14
var and = true
//@[04:07) Variable and. Type: true. Declaration start char: 0, length: 14
var or = true
//@[04:06) Variable or. Type: true. Declaration start char: 0, length: 13
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[04:17) Variable I_WANT_IT_ALL. Type: bool. Declaration start char: 0, length: 199

// identifiers can have underscores
var _ = 3
//@[04:05) Variable _. Type: 3. Declaration start char: 0, length: 9
var __ = 10 * _
//@[04:06) Variable __. Type: 30. Declaration start char: 0, length: 15
var _0a_1b = true
//@[04:10) Variable _0a_1b. Type: true. Declaration start char: 0, length: 17
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[04:07) Variable _1_. Type: true. Declaration start char: 0, length: 37

// fully qualified access
var resourceGroup = 'something'
//@[04:17) Variable resourceGroup. Type: 'something'. Declaration start char: 0, length: 31
var resourceGroupName = az.resourceGroup().name
//@[04:21) Variable resourceGroupName. Type: string. Declaration start char: 0, length: 47
var resourceGroupObject = az.resourceGroup()
//@[04:23) Variable resourceGroupObject. Type: resourceGroup. Declaration start char: 0, length: 44
var propertyAccessFromObject = resourceGroupObject.name
//@[04:28) Variable propertyAccessFromObject. Type: string. Declaration start char: 0, length: 55
var isTrue = sys.max(1, 2) == 3
//@[04:10) Variable isTrue. Type: false. Declaration start char: 0, length: 31
var isFalse = !isTrue
//@[04:11) Variable isFalse. Type: true. Declaration start char: 0, length: 21
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[04:12) Variable someText. Type: 'someText' | string. Declaration start char: 0, length: 74

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
//@[04:34) Variable scopesWithoutArmRepresentation. Type: object. Declaration start char: 0, length: 195
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
}

var scopesWithArmRepresentation = {
//@[04:31) Variable scopesWithArmRepresentation. Type: object. Declaration start char: 0, length: 123
  tenant: tenant()
  subscription: subscription()
  resourceGroup: az.resourceGroup()
}

// Issue #1332
var issue1332_propname = 'ptest'
//@[04:22) Variable issue1332_propname. Type: 'ptest'. Declaration start char: 0, length: 32
var issue1332 = true ? {
//@[04:13) Variable issue1332. Type: object | object. Declaration start char: 0, length: 86
    prop1: {
        '${issue1332_propname}': {}
    }
} : {}

// Issue #486
var myBigInt = 2199023255552
//@[04:12) Variable myBigInt. Type: 2199023255552. Declaration start char: 0, length: 28
var myIntExpression = 5 * 5
//@[04:19) Variable myIntExpression. Type: 25. Declaration start char: 0, length: 27
var myBigIntExpression = 2199023255552 * 2
//@[04:22) Variable myBigIntExpression. Type: 4398046511104. Declaration start char: 0, length: 42
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[04:23) Variable myBigIntExpression2. Type: int. Declaration start char: 0, length: 55

// variable loops
var incrementingNumbers = [for i in range(0,10) : i]
//@[31:32) Local i. Type: int. Declaration start char: 31, length: 1
//@[04:23) Variable incrementingNumbers. Type: int[]. Declaration start char: 0, length: 52
var loopInput = [
//@[04:13) Variable loopInput. Type: ['one', 'two']. Declaration start char: 0, length: 35
  'one'
  'two'
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[34:38) Local name. Type: 'one' | 'two'. Declaration start char: 34, length: 4
//@[40:41) Local i. Type: int. Declaration start char: 40, length: 1
//@[04:25) Variable arrayOfStringsViaLoop. Type: string[]. Declaration start char: 0, length: 79
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[34:38) Local name. Type: 'one' | 'two'. Declaration start char: 34, length: 4
//@[40:41) Local i. Type: int. Declaration start char: 40, length: 1
//@[04:25) Variable arrayOfObjectsViaLoop. Type: object[]. Declaration start char: 0, length: 123
  index: i
  name: name
  value: 'prefix-${i}-${name}-suffix'
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[33:37) Local name. Type: 'one' | 'two'. Declaration start char: 33, length: 4
//@[39:40) Local i. Type: int. Declaration start char: 39, length: 1
//@[04:24) Variable arrayOfArraysViaLoop. Type: [int, 'one' | 'two', string][]. Declaration start char: 0, length: 102
  i
  name
  'prefix-${i}-${name}-suffix'
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[28:32) Local name. Type: 'one' | 'two'. Declaration start char: 28, length: 4
//@[34:35) Local i. Type: int. Declaration start char: 34, length: 1
//@[04:19) Variable arrayOfBooleans. Type: bool[]. Declaration start char: 0, length: 62
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[35:36) Local i. Type: int. Declaration start char: 35, length: 1
//@[04:27) Variable arrayOfHardCodedNumbers. Type: 3[]. Declaration start char: 0, length: 55
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[33:34) Local i. Type: int. Declaration start char: 33, length: 1
//@[04:25) Variable arrayOfHardCodedBools. Type: false[]. Declaration start char: 0, length: 57
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[35:36) Local i. Type: int. Declaration start char: 35, length: 1
//@[04:27) Variable arrayOfHardCodedStrings. Type: 'hi'[]. Declaration start char: 0, length: 57
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[42:43) Local i. Type: int. Declaration start char: 42, length: 1
//@[04:34) Variable arrayOfNonRuntimeFunctionCalls. Type: string[]. Declaration start char: 0, length: 75

var multilineString = '''
//@[04:19) Variable multilineString. Type: 'HELLO!\n'. Declaration start char: 0, length: 36
HELLO!
'''

var multilineEmpty = ''''''
//@[04:18) Variable multilineEmpty. Type: ''. Declaration start char: 0, length: 27
var multilineEmptyNewline = '''
//@[04:25) Variable multilineEmptyNewline. Type: ''. Declaration start char: 0, length: 35
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''
//@[04:24) Variable multilineExtraQuotes. Type: '\'abc\''. Declaration start char: 0, length: 38

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
//@[04:32) Variable multilineExtraQuotesNewlines. Type: '\'\nabc\n\''. Declaration start char: 0, length: 48
abc
''''

var multilineSingleLine = '''hello!'''
//@[04:23) Variable multilineSingleLine. Type: 'hello!'. Declaration start char: 0, length: 38

var multilineFormatted = format('''
//@[04:22) Variable multilineFormatted. Type: 'Hello,\nmy\nname is\nAnthony\n'. Declaration start char: 0, length: 73
Hello,
my
name is
{0}
''', 'Anthony')

var multilineJavaScript = '''
//@[04:23) Variable multilineJavaScript. Type: '// NOT RECOMMENDED PATTERN\nconst fs = require(\'fs\');\n\nmodule.exports = function (context) {\n    fs.readFile(\'./hello.txt\', (err, data) => {\n        if (err) {\n            context.log.error(\'ERROR\', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: \${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function\'s invocation ends\n    context.done();\n}\n'. Declaration start char: 0, length: 586
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

var providersTest = providers('Microsoft.Resources').namespace
//@[04:17) Variable providersTest. Type: string. Declaration start char: 0, length: 62
var providersTest2 = providers('Microsoft.Resources', 'deployments').locations
//@[04:18) Variable providersTest2. Type: string[]. Declaration start char: 0, length: 78

var copyBlockInObject = {
//@[04:21) Variable copyBlockInObject. Type: object. Declaration start char: 0, length: 120
  copy: [
    {
      name: 'blah'
      count: '[notAFunction()]'
      input: {}
    }
  ]
}

var joinedString = join(['I', 'love', 'Bicep!'], ' ')
//@[04:16) Variable joinedString. Type: 'I love Bicep!'. Declaration start char: 0, length: 53

var prefix = take('food', 3)
//@[04:10) Variable prefix. Type: 'foo'. Declaration start char: 0, length: 28
var isPrefixed = startsWith('food', 'foo')
//@[04:14) Variable isPrefixed. Type: true. Declaration start char: 0, length: 42

