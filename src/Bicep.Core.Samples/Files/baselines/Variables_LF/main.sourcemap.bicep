
// int
@sys.description('an int variable')
var myInt = 42
//@    "myInt": 42,

// string
@sys.description('a string variable')
var myStr = 'str'
//@    "myStr": "str",
var curliesWithNoInterp = '}{1}{'
//@    "curliesWithNoInterp": "}{1}{",
var interp1 = 'abc${123}def'
//@    "interp1": "[format('abc{0}def', 123)]",
var interp2 = '${123}def'
//@    "interp2": "[format('{0}def', 123)]",
var interp3 = 'abc${123}'
//@    "interp3": "[format('abc{0}', 123)]",
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@    "interp4": "[format('abc{0}{1}jk$l{2}p$', 123, 456, 789)]",
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@    "doubleInterp": "[format('abc{0}_{1}', format('def{0}', 123), format('{0}{1}', 456, 789))]",
var curliesInInterp = '{${123}{0}${true}}'
//@    "curliesInInterp": "[format('{{{0}{{0}}{1}}}', 123, true())]",

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@    "bracketInTheMiddle": "a[b]",
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
//@    "bracketAtBeginning": "[test",
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
//@    "enclosingBrackets": "[[test]",
var emptyJsonArray = '[]'
//@    "emptyJsonArray": "[[]",
var interpolatedBrackets = '[${myInt}]'
//@    "interpolatedBrackets": "[format('[{0}]', variables('myInt'))]",
var nestedBrackets = '[test[]test2]'
//@    "nestedBrackets": "[[test[]test2]",
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@    "nestedInterpolatedBrackets": "[format('[{0}]', variables('emptyJsonArray'))]",
var bracketStringInExpression = concat('[', '\'test\'',']')
//@    "bracketStringInExpression": "[concat('[', '''test''', ']')]",

// booleans
@sys.description('a bool variable')
var myTruth = true
//@    "myTruth": true,
var myFalsehood = false
//@    "myFalsehood": false,

var myEmptyObj = { }
//@    "myEmptyObj": {},
var myEmptyArray = [ ]
//@    "myEmptyArray": [],

// object
@sys.description('a object variable')
var myObj = {
//@    "myObj": {
//@    },
  a: 'a'
//@      "a": "a",
  b: -12
//@      "b": -12,
  c: true
//@      "c": true,
  d: !true
//@      "d": "[not(true())]",
  list: [
//@      "list": [
//@      ],
    1
//@        1,
    2
//@        2,
    2+1
//@        "[add(2, 1)]",
    {
//@        {
//@        },
      test: 144 > 33 && true || 99 <= 199
//@          "test": "[or(and(greater(144, 33), true()), lessOrEquals(99, 199))]"
    }
    'a' =~ 'b'
//@        "[equals(toLower('a'), toLower('b'))]"
  ]
  obj: {
//@      "obj": {
//@      }
    nested: [
//@        "nested": [
//@        ]
      'hello'
//@          "hello"
    ]
  }
}

@sys.description('a object with interp')
var objWithInterp = {
//@    "objWithInterp": {
//@    },
  '${myStr}': 1
//@      "[format('{0}', variables('myStr'))]": 1,
  'abc${myStr}def': 2
//@      "[format('abc{0}def', variables('myStr'))]": 2,
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@      "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]": "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]"
}

// array
var myArr = [
//@    "myArr": [
//@    ],
  'pirates'
//@      "pirates",
  'say'
//@      "say",
  'arr'
//@      "arr"
]

// array with objects
var myArrWithObjects = [
//@    "myArrWithObjects": [
//@    ],
  {
//@      {
//@      },
    name: 'one'
//@        "name": "one",
    enable: true
//@        "enable": true
  }
  {
//@      {
//@      }
    name: 'two'
//@        "name": "two",
    enable: false && false || 'two' !~ 'three'
//@        "enable": "[or(and(false(), false()), not(equals(toLower('two'), toLower('three'))))]"
  }
]

var expressionIndexOnAny = any({
//@    "expressionIndexOnAny": "[createObject()[resourceGroup().location]]",
})[az.resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@    "anyIndexOnAny": "[true()[false()]]",

var deploymentName = deployment().name
//@    "deploymentName": "[deployment().name]",
var templateContentVersion = deployment().properties.template.contentVersion
//@    "templateContentVersion": "[deployment().properties.template.contentVersion]",
var templateLinkUri = deployment().properties.templateLink.uri
//@    "templateLinkUri": "[deployment().properties.templateLink.uri]",
var templateLinkId = deployment().properties.templateLink.id
//@    "templateLinkId": "[deployment().properties.templateLink.id]",

var portalEndpoint = environment().portal
//@    "portalEndpoint": "[environment().portal]",
var loginEndpoint = environment().authentication.loginEndpoint
//@    "loginEndpoint": "[environment().authentication.loginEndpoint]",

var namedPropertyIndexer = {
//@    "namedPropertyIndexer": "[createObject('foo', 's').foo]",
  foo: 's'
}['foo']

var intIndexer = [
//@    "intIndexer": "[createArray('s')[0]]",
  's'
][0]

var functionOnIndexer1 = concat([
//@    "functionOnIndexer1": "[concat(createArray('s')[0], 's')]",
  's'
][0], 's')

var singleQuote = '\''
//@    "singleQuote": "'",
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@    "myPropertyName": "[format('{0}foo{1}', variables('singleQuote'), variables('singleQuote'))]",

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
var unusedIntermediateRef = unusedIntermediate.secondaryKey

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@    "previousEmitLimit": [
//@    ],
  concat('s')
//@      "[concat('s')]",
  '${4}'
//@      "[format('{0}', 4)]",
  {
//@      {
//@      }
    a: {
//@        "a": {
//@        }
      b: base64('s')
//@          "b": "[base64('s')]",
      c: concat([
//@          "c": "[concat(createArray(add(12, 3)), createArray(not(true()), 'hello'))]",
        12 + 3
      ], [
        !true
        'hello'
      ])
      d: az.resourceGroup().location
//@          "d": "[resourceGroup().location]",
      e: concat([
//@          "e": "[concat(createArray(true()))]",
        true
      ])
      f: concat([
//@          "f": "[concat(createArray(equals('s', 12)))]"
        's' == 12
      ])
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit2 = [
//@    "previousEmitLimit2": [
//@    ],
  concat('s')
//@      "[concat('s')]",
  '${4}'
//@      "[format('{0}', 4)]",
  {
//@      {
//@      }
    a: {
//@        "a": {
//@        }
      b: base64('s')
//@          "b": "[base64('s')]",
      c: union({
//@          "c": "[union(createObject('a', add(12, 3)), createObject('b', not(true()), 'c', 'hello'))]",
        a: 12 + 3
      }, {
        b: !true
        c: 'hello'
      })
      d: az.resourceGroup().location
//@          "d": "[resourceGroup().location]",
      e: union({
//@          "e": "[union(createObject('x', true()), createObject())]",
        x: true
      }, {})
      f: intersection({
//@          "f": "[intersection(createObject('q', equals('s', 12)), createObject())]"
        q: 's' == 12
      }, {})
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit3 = {
//@    "previousEmitLimit3": {
//@    },
  a: {
//@      "a": {
//@      }
    b: {
//@        "b": "[equals(createObject('a', resourceGroup().location), 2)]",
      a: az.resourceGroup().location
    } == 2
    c: concat([
//@        "c": "[concat(createArray(), createArray(true()))]"

    ], [
      true
    ])
  }
}

// #completionTest(0) -> declarations

var myVar = 'hello'
//@    "myVar": "hello",
var myVar2 = any({
//@    "myVar2": {
//@    },
  something: myVar
//@      "something": "[variables('myVar')]"
})
var myVar3 = any(any({
//@    "myVar3": {
//@    },
  something: myVar
//@      "something": "[variables('myVar')]"
}))
var myVar4 = length(any(concat('s','a')))
//@    "myVar4": "[length(concat('s', 'a'))]",

// verify that unqualified banned function identifiers can be used as declaration identifiers
var variables = true
//@    "variables": true,
param parameters bool = true
//@    "parameters": {
//@      "type": "bool",
//@      "defaultValue": true
//@    },
var if = true
//@    "if": true,
var createArray = true
//@    "createArray": true,
var createObject = true
//@    "createObject": true,
var add = true
//@    "add": true,
var sub = true
//@    "sub": true,
var mul = true
//@    "mul": true,
var div = true
//@    "div": true,
param mod bool = true
//@    "mod": {
//@      "type": "bool",
//@      "defaultValue": true
//@    },
var less = true
//@    "less": true,
var lessOrEquals = true
//@    "lessOrEquals": true,
var greater = true
//@    "greater": true,
var greaterOrEquals = true
//@    "greaterOrEquals": true,
param equals bool = true
//@    "equals": {
//@      "type": "bool",
//@      "defaultValue": true
//@    }
var not = true
//@    "not": true,
var and = true
//@    "and": true,
var or = true
//@    "or": true,
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@    "I_WANT_IT_ALL": "[and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(variables('variables'), parameters('parameters')), variables('if')), variables('createArray')), variables('createObject')), variables('add')), variables('sub')), variables('mul')), variables('div')), parameters('mod')), variables('less')), variables('lessOrEquals')), variables('greater')), variables('greaterOrEquals')), parameters('equals')), variables('not')), variables('and')), variables('or'))]",

// identifiers can have underscores
var _ = 3
//@    "_": 3,
var __ = 10 * _
//@    "__": "[mul(10, variables('_'))]",
var _0a_1b = true
//@    "_0a_1b": true,
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@    "_1_": "[or(variables('_0a_1b'), equals(add(variables('__'), mod(variables('_'), 2)), 0))]",

// fully qualified access
var resourceGroup = 'something'
//@    "resourceGroup": "something",
var resourceGroupName = az.resourceGroup().name
//@    "resourceGroupName": "[resourceGroup().name]",
var resourceGroupObject = az.resourceGroup()
//@    "resourceGroupObject": "[resourceGroup()]",
var propertyAccessFromObject = resourceGroupObject.name
//@    "propertyAccessFromObject": "[variables('resourceGroupObject').name]",
var isTrue = sys.max(1, 2) == 3
//@    "isTrue": "[equals(max(1, 2), 3)]",
var isFalse = !isTrue
//@    "isFalse": "[not(variables('isTrue'))]",
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@    "someText": "[if(variables('isTrue'), concat('a', concat('b', 'c')), 'someText')]",

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
//@    "scopesWithoutArmRepresentation": {
//@    },
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@      "subscription": {},
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@      "resourceGroup": {}
}

var scopesWithArmRepresentation = {
//@    "scopesWithArmRepresentation": {
//@    },
  tenant: tenant()
//@      "tenant": "[tenant()]",
  subscription: subscription()
//@      "subscription": "[subscription()]",
  resourceGroup: az.resourceGroup()
//@      "resourceGroup": "[resourceGroup()]"
}

// Issue #1332
var issue1332_propname = 'ptest'
//@    "issue1332_propname": "ptest",
var issue1332 = true ? {
//@    "issue1332": "[if(true(), createObject('prop1', createObject(format('{0}', variables('issue1332_propname')), createObject())), createObject())]",
    prop1: {
        '${issue1332_propname}': {}
    }
} : {}

// Issue #486
var myBigInt = 2199023255552
//@    "myBigInt": 2199023255552,
var myIntExpression = 5 * 5
//@    "myIntExpression": "[mul(5, 5)]",
var myBigIntExpression = 2199023255552 * 2
//@    "myBigIntExpression": "[mul(json('2199023255552'), 2)]",
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@    "myBigIntExpression2": "[mul(json('2199023255552'), json('2199023255552'))]",

// variable loops
var incrementingNumbers = [for i in range(0,10) : i]
//@      {
//@        "name": "incrementingNumbers",
//@        "count": "[length(range(0, 10))]",
//@        "input": "[range(0, 10)[copyIndex('incrementingNumbers')]]"
//@      },
var printToSingleLine1 = [
    for i in range(0,20) : i
//@      {
//@        "name": "printToSingleLine1",
//@        "count": "[length(range(0, 20))]",
//@        "input": "[range(0, 20)[copyIndex('printToSingleLine1')]]"
//@      },
]
var printToSingleLine2 = [
    /* harmless comment */ for i in range(0,20) : i
//@      {
//@        "name": "printToSingleLine2",
//@        "count": "[length(range(0, 20))]",
//@        "input": "[range(0, 20)[copyIndex('printToSingleLine2')]]"
//@      },
]
var printToSingleLine3 = [
    for i in range(0,20) : i /* harmless comment */
//@      {
//@        "name": "printToSingleLine3",
//@        "count": "[length(range(0, 20))]",
//@        "input": "[range(0, 20)[copyIndex('printToSingleLine3')]]"
//@      },
]
var forceLineBreaks1 = [
    // force line breaks
    for i in range(0,    30) : i
//@      {
//@        "name": "forceLineBreaks1",
//@        "count": "[length(range(0, 30))]",
//@        "input": "[range(0, 30)[copyIndex('forceLineBreaks1')]]"
//@      },
]
var forceLineBreaks2 = [
    for i in range(0,    30) : i
//@      {
//@        "name": "forceLineBreaks2",
//@        "count": "[length(range(0, 30))]",
//@        "input": "[range(0, 30)[copyIndex('forceLineBreaks2')]]"
//@      },
    // force line breaks
]
var forceLineBreaks3 = [
    /* force line breaks */
    for i in range(0,    30) : i
//@      {
//@        "name": "forceLineBreaks3",
//@        "count": "[length(range(0, 30))]",
//@        "input": "[range(0, 30)[copyIndex('forceLineBreaks3')]]"
//@      },
    /* force line breaks */
]

var loopInput = [
//@    "loopInput": [
//@    ],
  'one'
//@      "one",
  'two'
//@      "two"
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@      {
//@        "name": "arrayOfStringsViaLoop",
//@        "count": "[length(variables('loopInput'))]",
//@        "input": "[format('prefix-{0}-{1}', copyIndex('arrayOfStringsViaLoop'), variables('loopInput')[copyIndex('arrayOfStringsViaLoop')])]"
//@      },
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@      {
//@        "name": "arrayOfObjectsViaLoop",
//@        "count": "[length(variables('loopInput'))]",
//@        "input": {
//@        }
//@      },
  index: i
//@          "index": "[copyIndex('arrayOfObjectsViaLoop')]",
  name: name
//@          "name": "[variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')]]",
  value: 'prefix-${i}-${name}-suffix'
//@          "value": "[format('prefix-{0}-{1}-suffix', copyIndex('arrayOfObjectsViaLoop'), variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')])]"
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@      {
//@        "name": "arrayOfArraysViaLoop",
//@        "count": "[length(variables('loopInput'))]",
//@        "input": "[createArray(copyIndex('arrayOfArraysViaLoop'), variables('loopInput')[copyIndex('arrayOfArraysViaLoop')], format('prefix-{0}-{1}-suffix', copyIndex('arrayOfArraysViaLoop'), variables('loopInput')[copyIndex('arrayOfArraysViaLoop')]))]"
//@      },
  i
  name
  'prefix-${i}-${name}-suffix'
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@      {
//@        "name": "arrayOfBooleans",
//@        "count": "[length(variables('loopInput'))]",
//@        "input": "[equals(mod(copyIndex('arrayOfBooleans'), 2), 0)]"
//@      },
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@      {
//@        "name": "arrayOfHardCodedNumbers",
//@        "count": "[length(range(0, 10))]",
//@        "input": "[int(3)]"
//@      },
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@      {
//@        "name": "arrayOfHardCodedBools",
//@        "count": "[length(range(0, 10))]",
//@        "input": "[false()]"
//@      },
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@      {
//@        "name": "arrayOfHardCodedStrings",
//@        "count": "[length(range(0, 3))]",
//@        "input": "hi"
//@      },
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@      {
//@        "name": "arrayOfNonRuntimeFunctionCalls",
//@        "count": "[length(range(0, 3))]",
//@        "input": "[concat('hi', range(0, 3)[copyIndex('arrayOfNonRuntimeFunctionCalls')])]"
//@      }

var multilineString = '''
//@    "multilineString": "HELLO!\n",
HELLO!
'''

var multilineEmpty = ''''''
//@    "multilineEmpty": "",
var multilineEmptyNewline = '''
//@    "multilineEmptyNewline": "",
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''
//@    "multilineExtraQuotes": "'abc'",

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
//@    "multilineExtraQuotesNewlines": "'\nabc\n'",
abc
''''

var multilineSingleLine = '''hello!'''
//@    "multilineSingleLine": "hello!",

var multilineFormatted = format('''
//@    "multilineFormatted": "[format('Hello,\nmy\nname is\n{0}\n', 'Anthony')]",
Hello,
my
name is
{0}
''', 'Anthony')

var multilineJavaScript = '''
//@    "multilineJavaScript": "// NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n",
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
//@    "providersTest": "[providers('Microsoft.Resources').namespace]",
var providersTest2 = providers('Microsoft.Resources', 'deployments').locations
//@    "providersTest2": "[providers('Microsoft.Resources', 'deployments').locations]",

var copyBlockInObject = {
//@    "copyBlockInObject": {
//@    },
  copy: [
//@      "[string('copy')]": [
//@      ]
    {
//@        {
//@        }
      name: 'blah'
//@          "name": "blah",
      count: '[notAFunction()]'
//@          "count": "[[notAFunction()]",
      input: {}
//@          "input": {}
    }
  ]
}

var joinedString = join(['I', 'love', 'Bicep!'], ' ')
//@    "joinedString": "[join(createArray('I', 'love', 'Bicep!'), ' ')]",

var prefix = take('food', 3)
//@    "prefix": "[take('food', 3)]",
var isPrefixed = startsWith('food', 'foo')
//@    "isPrefixed": "[startsWith('food', 'foo')]",

var spread = {
//@    "spread": "[shallowMerge(createArray(createObject('foo', 'abc'), variables('issue1332')))]",
  foo: 'abc'
  ...issue1332
}

var test = {
//@    "test": "[shallowMerge(createArray(variables('spread'), createObject('bar', 'def')))]",
  ...spread
  bar: 'def'
}

var arraySpread = [...arrayOfBooleans, ...arrayOfHardCodedNumbers, ...arrayOfHardCodedStrings]
//@    "arraySpread": "[flatten(createArray(variables('arrayOfBooleans'), variables('arrayOfHardCodedNumbers'), variables('arrayOfHardCodedStrings')))]",


var nameof1 = nameof(arraySpread)
//@    "nameof1": "arraySpread",
var nameof2 = nameof(spread.foo)
//@    "nameof2": "foo",
var nameof3 = nameof(myObj.obj.nested)
//@    "nameof3": "nested"

