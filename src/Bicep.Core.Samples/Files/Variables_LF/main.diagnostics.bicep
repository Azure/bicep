
//@[0:0) [linter-internal-error (Warning)] Analyzer 'core' encountered an unexpected exception. Rewrite to string interpolation not successful ||
// an int variable
var myInt = 42

// a string variable
var myStr = 'str'
var curliesWithNoInterp = '}{1}{'
//@[4:23) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |curliesWithNoInterp|
var interp1 = 'abc${123}def'
var interp2 = '${123}def'
var interp3 = 'abc${123}'
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |interp3|
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |interp4|
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[4:16) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |doubleInterp|
var curliesInInterp = '{${123}{0}${true}}'
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |curliesInInterp|

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bracketInTheMiddle|
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bracketAtBeginning|
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
//@[4:21) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |enclosingBrackets|
var emptyJsonArray = '[]'
var interpolatedBrackets = '[${myInt}]'
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |interpolatedBrackets|
var nestedBrackets = '[test[]test2]'
//@[4:18) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |nestedBrackets|
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[4:30) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |nestedInterpolatedBrackets|
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[4:29) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |bracketStringInExpression|

// booleans
var myTruth = true
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myTruth|
var myFalsehood = false
//@[4:15) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myFalsehood|

var myEmptyObj = { }
//@[4:14) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myEmptyObj|
var myEmptyArray = [ ]
//@[4:16) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myEmptyArray|

// object
var myObj = {
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myObj|
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

var objWithInterp = {
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |objWithInterp|
  '${myStr}': 1
  'abc${myStr}def': 2
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
}

// array
var myArr = [
//@[4:9) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myArr|
  'pirates'
  'say'
  'arr'
]

// array with objects
var myArrWithObjects = [
//@[4:20) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myArrWithObjects|
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
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |expressionIndexOnAny|
})[az.resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |anyIndexOnAny|

var deploymentName = deployment().name
//@[4:18) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |deploymentName|
var templateLinkUri = deployment().properties.templateLink.uri
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |templateLinkUri|
var templateLinkId = deployment().properties.templateLink.id
//@[4:18) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |templateLinkId|

var portalEndpoint = environment().portal
//@[4:18) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |portalEndpoint|
var loginEndpoint = environment().authentication.loginEndpoint
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |loginEndpoint|

var namedPropertyIndexer = {
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |namedPropertyIndexer|
  foo: 's'
}['foo']

var intIndexer = [
//@[4:14) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |intIndexer|
  's'
][0]

var functionOnIndexer1 = concat([
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |functionOnIndexer1|
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |functionOnIndexer2|
][0], 's')

var functionOnIndexer3 = concat([
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |functionOnIndexer3|
][0], any('s'))

var singleQuote = '\''
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[4:18) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myPropertyName|

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[4:25) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |unusedIntermediateRef|

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@[4:21) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |previousEmitLimit|
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
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |previousEmitLimit2|
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
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |previousEmitLimit3|
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
var myVar2 = any({
//@[4:10) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myVar2|
  something: myVar
})
var myVar3 = any(any({
//@[4:10) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myVar3|
  something: myVar
}))
var myVar4 = length(any(concat('s','a')))
//@[4:10) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myVar4|

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
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |I_WANT_IT_ALL|

// identifiers can have underscores
var _ = 3
var __ = 10 * _
var _0a_1b = true
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[4:7) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |_1_|

// fully qualified access
var resourceGroup = 'something'
//@[4:17) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |resourceGroup|
var resourceGroupName = az.resourceGroup().name
//@[4:21) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |resourceGroupName|
var resourceGroupObject = az.resourceGroup()
var propertyAccessFromObject = resourceGroupObject.name
//@[4:28) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |propertyAccessFromObject|
var isTrue = sys.max(1, 2) == 3
var isFalse = !isTrue
//@[4:11) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |isFalse|
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[4:12) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |someText|

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
//@[4:34) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |scopesWithoutArmRepresentation|
  tenant: tenant()
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
}

// Issue #1332
var issue1332_propname = 'ptest'
var issue1332 = true ? {
//@[4:13) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |issue1332|
    prop1: {
        '${issue1332_propname}': {}
    }
} : {}

// Issue #486
var myBigInt = 2199023255552
//@[4:12) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myBigInt|
var myIntExpression = 5 * 5
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myIntExpression|
var myBigIntExpression = 2199023255552 * 2
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myBigIntExpression|
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[4:23) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |myBigIntExpression2|

// variable loops
var incrementingNumbers = [for i in range(0,10) : i]
//@[4:23) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |incrementingNumbers|
var loopInput = [
  'one'
  'two'
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[4:25) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |arrayOfStringsViaLoop|
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[4:25) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |arrayOfObjectsViaLoop|
  index: i
  name: name
  value: 'prefix-${i}-${name}-suffix'
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |arrayOfArraysViaLoop|
  i
  name
  'prefix-${i}-${name}-suffix'
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |arrayOfBooleans|
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[4:27) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |arrayOfHardCodedNumbers|
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[4:25) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |arrayOfHardCodedBools|
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[4:27) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |arrayOfHardCodedStrings|
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[4:34) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |arrayOfNonRuntimeFunctionCalls|

var multilineString = '''
//@[4:19) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multilineString|
HELLO!
'''

var multilineEmpty = ''''''
//@[4:18) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multilineEmpty|
var multilineEmptyNewline = '''
//@[4:25) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multilineEmptyNewline|
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''
//@[4:24) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multilineExtraQuotes|

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
//@[4:32) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multilineExtraQuotesNewlines|
abc
''''

var multilineSingleLine = '''hello!'''
//@[4:23) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multilineSingleLine|

var multilineFormatted = format('''
//@[4:22) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multilineFormatted|
Hello,
my
name is
{0}
''', 'Anthony')

var multilineJavaScript = '''
//@[4:23) [no-unused-vars (Warning)] Variable is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-vars |multilineJavaScript|
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

