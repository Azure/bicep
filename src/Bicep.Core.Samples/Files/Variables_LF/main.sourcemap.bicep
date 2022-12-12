
// int
@sys.description('an int variable')
var myInt = 42
//@[line003->line076]     "myInt": 42,

// string
@sys.description('a string variable')
var myStr = 'str'
//@[line007->line077]     "myStr": "str",
var curliesWithNoInterp = '}{1}{'
//@[line008->line078]     "curliesWithNoInterp": "}{1}{",
var interp1 = 'abc${123}def'
//@[line009->line079]     "interp1": "[format('abc{0}def', 123)]",
var interp2 = '${123}def'
//@[line010->line080]     "interp2": "[format('{0}def', 123)]",
var interp3 = 'abc${123}'
//@[line011->line081]     "interp3": "[format('abc{0}', 123)]",
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[line012->line082]     "interp4": "[format('abc{0}{1}jk$l{2}p$', 123, 456, 789)]",
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[line013->line083]     "doubleInterp": "[format('abc{0}_{1}', format('def{0}', 123), format('{0}{1}', 456, 789))]",
var curliesInInterp = '{${123}{0}${true}}'
//@[line014->line084]     "curliesInInterp": "[format('{{{0}{{0}}{1}}}', 123, true())]",

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[line019->line085]     "bracketInTheMiddle": "a[b]",
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
//@[line021->line086]     "bracketAtBeginning": "[test",
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
//@[line023->line087]     "enclosingBrackets": "[[test]",
var emptyJsonArray = '[]'
//@[line024->line088]     "emptyJsonArray": "[[]",
var interpolatedBrackets = '[${myInt}]'
//@[line025->line089]     "interpolatedBrackets": "[format('[{0}]', variables('myInt'))]",
var nestedBrackets = '[test[]test2]'
//@[line026->line090]     "nestedBrackets": "[[test[]test2]",
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[line027->line091]     "nestedInterpolatedBrackets": "[format('[{0}]', variables('emptyJsonArray'))]",
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[line028->line092]     "bracketStringInExpression": "[concat('[', '''test''', ']')]",

// booleans
@sys.description('a bool variable')
var myTruth = true
//@[line032->line093]     "myTruth": true,
var myFalsehood = false
//@[line033->line094]     "myFalsehood": false,

var myEmptyObj = { }
//@[line035->line095]     "myEmptyObj": {},
var myEmptyArray = [ ]
//@[line036->line096]     "myEmptyArray": [],

// object
@sys.description('a object variable')
var myObj = {
//@[line040->line097]     "myObj": {
//@[line040->line116]     },
  a: 'a'
//@[line041->line098]       "a": "a",
  b: -12
//@[line042->line099]       "b": -12,
  c: true
//@[line043->line100]       "c": true,
  d: !true
//@[line044->line101]       "d": "[not(true())]",
  list: [
//@[line045->line102]       "list": [
//@[line045->line110]       ],
    1
//@[line046->line103]         1,
    2
//@[line047->line104]         2,
    2+1
//@[line048->line105]         "[add(2, 1)]",
    {
//@[line049->line106]         {
//@[line049->line108]         },
      test: 144 > 33 && true || 99 <= 199
//@[line050->line107]           "test": "[or(and(greater(144, 33), true()), lessOrEquals(99, 199))]"
    }
    'a' =~ 'b'
//@[line052->line109]         "[equals(toLower('a'), toLower('b'))]"
  ]
  obj: {
//@[line054->line111]       "obj": {
//@[line054->line115]       }
    nested: [
//@[line055->line112]         "nested": [
//@[line055->line114]         ]
      'hello'
//@[line056->line113]           "hello"
    ]
  }
}

@sys.description('a object with interp')
var objWithInterp = {
//@[line062->line117]     "objWithInterp": {
//@[line062->line121]     },
  '${myStr}': 1
//@[line063->line118]       "[format('{0}', variables('myStr'))]": 1,
  'abc${myStr}def': 2
//@[line064->line119]       "[format('abc{0}def', variables('myStr'))]": 2,
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[line065->line120]       "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]": "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]"
}

// array
var myArr = [
//@[line069->line122]     "myArr": [
//@[line069->line126]     ],
  'pirates'
//@[line070->line123]       "pirates",
  'say'
//@[line071->line124]       "say",
  'arr'
//@[line072->line125]       "arr"
]

// array with objects
var myArrWithObjects = [
//@[line076->line127]     "myArrWithObjects": [
//@[line076->line136]     ],
  {
//@[line077->line128]       {
//@[line077->line131]       },
    name: 'one'
//@[line078->line129]         "name": "one",
    enable: true
//@[line079->line130]         "enable": true
  }
  {
//@[line081->line132]       {
//@[line081->line135]       }
    name: 'two'
//@[line082->line133]         "name": "two",
    enable: false && false || 'two' !~ 'three'
//@[line083->line134]         "enable": "[or(and(false(), false()), not(equals(toLower('two'), toLower('three'))))]"
  }
]

var expressionIndexOnAny = any({
//@[line087->line137]     "expressionIndexOnAny": "[createObject()[resourceGroup().location]]",
})[az.resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[line090->line138]     "anyIndexOnAny": "[true()[false()]]",

var deploymentName = deployment().name
//@[line092->line139]     "deploymentName": "[deployment().name]",
var templateContentVersion = deployment().properties.template.contentVersion
//@[line093->line140]     "templateContentVersion": "[deployment().properties.template.contentVersion]",
var templateLinkUri = deployment().properties.templateLink.uri
//@[line094->line141]     "templateLinkUri": "[deployment().properties.templateLink.uri]",
var templateLinkId = deployment().properties.templateLink.id
//@[line095->line142]     "templateLinkId": "[deployment().properties.templateLink.id]",

var portalEndpoint = environment().portal
//@[line097->line143]     "portalEndpoint": "[environment().portal]",
var loginEndpoint = environment().authentication.loginEndpoint
//@[line098->line144]     "loginEndpoint": "[environment().authentication.loginEndpoint]",

var namedPropertyIndexer = {
//@[line100->line145]     "namedPropertyIndexer": "[createObject('foo', 's').foo]",
  foo: 's'
}['foo']

var intIndexer = [
//@[line104->line146]     "intIndexer": "[createArray('s')[0]]",
  's'
][0]

var functionOnIndexer1 = concat([
//@[line108->line147]     "functionOnIndexer1": "[concat(createArray('s')[0], 's')]",
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[line112->line148]     "functionOnIndexer2": "[concat(createArray()[0], 's')]",
][0], 's')

var functionOnIndexer3 = concat([
//@[line115->line149]     "functionOnIndexer3": "[concat(createArray()[0], 's')]",
][0], any('s'))

var singleQuote = '\''
//@[line118->line150]     "singleQuote": "'",
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[line119->line151]     "myPropertyName": "[format('{0}foo{1}', variables('singleQuote'), variables('singleQuote'))]",

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
var unusedIntermediateRef = unusedIntermediate.secondaryKey

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@[line125->line152]     "previousEmitLimit": [
//@[line125->line164]     ],
  concat('s')
//@[line126->line153]       "[concat('s')]",
  '${4}'
//@[line127->line154]       "[format('{0}', 4)]",
  {
//@[line128->line155]       {
//@[line128->line163]       }
    a: {
//@[line129->line156]         "a": {
//@[line129->line162]         }
      b: base64('s')
//@[line130->line157]           "b": "[base64('s')]",
      c: concat([
//@[line131->line158]           "c": "[concat(createArray(add(12, 3)), createArray(not(true()), 'hello'))]",
        12 + 3
      ], [
        !true
        'hello'
      ])
      d: az.resourceGroup().location
//@[line137->line159]           "d": "[resourceGroup().location]",
      e: concat([
//@[line138->line160]           "e": "[concat(createArray(true()))]",
        true
      ])
      f: concat([
//@[line141->line161]           "f": "[concat(createArray(equals('s', 12)))]"
        's' == 12
      ])
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit2 = [
//@[line149->line165]     "previousEmitLimit2": [
//@[line149->line177]     ],
  concat('s')
//@[line150->line166]       "[concat('s')]",
  '${4}'
//@[line151->line167]       "[format('{0}', 4)]",
  {
//@[line152->line168]       {
//@[line152->line176]       }
    a: {
//@[line153->line169]         "a": {
//@[line153->line175]         }
      b: base64('s')
//@[line154->line170]           "b": "[base64('s')]",
      c: union({
//@[line155->line171]           "c": "[union(createObject('a', add(12, 3)), createObject('b', not(true()), 'c', 'hello'))]",
        a: 12 + 3
      }, {
        b: !true
        c: 'hello'
      })
      d: az.resourceGroup().location
//@[line161->line172]           "d": "[resourceGroup().location]",
      e: union({
//@[line162->line173]           "e": "[union(createObject('x', true()), createObject())]",
        x: true
      }, {})
      f: intersection({
//@[line165->line174]           "f": "[intersection(createObject('q', equals('s', 12)), createObject())]"
        q: 's' == 12
      }, {})
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit3 = {
//@[line173->line178]     "previousEmitLimit3": {
//@[line173->line183]     },
  a: {
//@[line174->line179]       "a": {
//@[line174->line182]       }
    b: {
//@[line175->line180]         "b": "[equals(createObject('a', resourceGroup().location), 2)]",
      a: az.resourceGroup().location
    } == 2
    c: concat([
//@[line178->line181]         "c": "[concat(createArray(), createArray(true()))]"

    ], [
      true
    ])
  }
}

// #completionTest(0) -> declarations

var myVar = 'hello'
//@[line188->line184]     "myVar": "hello",
var myVar2 = any({
//@[line189->line185]     "myVar2": {
//@[line189->line187]     },
  something: myVar
//@[line190->line186]       "something": "[variables('myVar')]"
})
var myVar3 = any(any({
//@[line192->line188]     "myVar3": {
//@[line192->line190]     },
  something: myVar
//@[line193->line189]       "something": "[variables('myVar')]"
}))
var myVar4 = length(any(concat('s','a')))
//@[line195->line191]     "myVar4": "[length(concat('s', 'a'))]",

// verify that unqualified banned function identifiers can be used as declaration identifiers
var variables = true
//@[line198->line192]     "variables": true,
param parameters bool = true
//@[line199->line011]     "parameters": {
//@[line199->line012]       "type": "bool",
//@[line199->line013]       "defaultValue": true
//@[line199->line014]     },
var if = true
//@[line200->line193]     "if": true,
var createArray = true
//@[line201->line194]     "createArray": true,
var createObject = true
//@[line202->line195]     "createObject": true,
var add = true
//@[line203->line196]     "add": true,
var sub = true
//@[line204->line197]     "sub": true,
var mul = true
//@[line205->line198]     "mul": true,
var div = true
//@[line206->line199]     "div": true,
param mod bool = true
//@[line207->line015]     "mod": {
//@[line207->line016]       "type": "bool",
//@[line207->line017]       "defaultValue": true
//@[line207->line018]     },
var less = true
//@[line208->line200]     "less": true,
var lessOrEquals = true
//@[line209->line201]     "lessOrEquals": true,
var greater = true
//@[line210->line202]     "greater": true,
var greaterOrEquals = true
//@[line211->line203]     "greaterOrEquals": true,
param equals bool = true
//@[line212->line019]     "equals": {
//@[line212->line020]       "type": "bool",
//@[line212->line021]       "defaultValue": true
//@[line212->line022]     }
var not = true
//@[line213->line204]     "not": true,
var and = true
//@[line214->line205]     "and": true,
var or = true
//@[line215->line206]     "or": true,
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[line216->line207]     "I_WANT_IT_ALL": "[and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(variables('variables'), parameters('parameters')), variables('if')), variables('createArray')), variables('createObject')), variables('add')), variables('sub')), variables('mul')), variables('div')), parameters('mod')), variables('less')), variables('lessOrEquals')), variables('greater')), variables('greaterOrEquals')), parameters('equals')), variables('not')), variables('and')), variables('or'))]",

// identifiers can have underscores
var _ = 3
//@[line219->line208]     "_": 3,
var __ = 10 * _
//@[line220->line209]     "__": "[mul(10, variables('_'))]",
var _0a_1b = true
//@[line221->line210]     "_0a_1b": true,
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[line222->line211]     "_1_": "[or(variables('_0a_1b'), equals(add(variables('__'), mod(variables('_'), 2)), 0))]",

// fully qualified access
var resourceGroup = 'something'
//@[line225->line212]     "resourceGroup": "something",
var resourceGroupName = az.resourceGroup().name
//@[line226->line213]     "resourceGroupName": "[resourceGroup().name]",
var resourceGroupObject = az.resourceGroup()
//@[line227->line214]     "resourceGroupObject": "[resourceGroup()]",
var propertyAccessFromObject = resourceGroupObject.name
//@[line228->line215]     "propertyAccessFromObject": "[variables('resourceGroupObject').name]",
var isTrue = sys.max(1, 2) == 3
//@[line229->line216]     "isTrue": "[equals(max(1, 2), 3)]",
var isFalse = !isTrue
//@[line230->line217]     "isFalse": "[not(variables('isTrue'))]",
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[line231->line218]     "someText": "[if(variables('isTrue'), concat('a', concat('b', 'c')), 'someText')]",

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
//@[line234->line219]     "scopesWithoutArmRepresentation": {
//@[line234->line222]     },
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@[line235->line220]       "subscription": "[createObject()]",
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@[line236->line221]       "resourceGroup": "[createObject()]"
}

var scopesWithArmRepresentation = {
//@[line239->line223]     "scopesWithArmRepresentation": {
//@[line239->line227]     },
  tenant: tenant()
//@[line240->line224]       "tenant": "[tenant()]",
  subscription: subscription()
//@[line241->line225]       "subscription": "[subscription()]",
  resourceGroup: az.resourceGroup()
//@[line242->line226]       "resourceGroup": "[resourceGroup()]"
}

// Issue #1332
var issue1332_propname = 'ptest'
//@[line246->line228]     "issue1332_propname": "ptest",
var issue1332 = true ? {
//@[line247->line229]     "issue1332": "[if(true(), createObject('prop1', createObject(format('{0}', variables('issue1332_propname')), createObject())), createObject())]",
    prop1: {
        '${issue1332_propname}': {}
    }
} : {}

// Issue #486
var myBigInt = 2199023255552
//@[line254->line230]     "myBigInt": 2199023255552,
var myIntExpression = 5 * 5
//@[line255->line231]     "myIntExpression": "[mul(5, 5)]",
var myBigIntExpression = 2199023255552 * 2
//@[line256->line232]     "myBigIntExpression": "[mul(json('2199023255552'), 2)]",
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[line257->line233]     "myBigIntExpression2": "[mul(json('2199023255552'), json('2199023255552'))]",

// variable loops
var incrementingNumbers = [for i in range(0,10) : i]
//@[line260->line026]       {
//@[line260->line027]         "name": "incrementingNumbers",
//@[line260->line028]         "count": "[length(range(0, 10))]",
//@[line260->line029]         "input": "[range(0, 10)[copyIndex('incrementingNumbers')]]"
//@[line260->line030]       },
var loopInput = [
//@[line261->line234]     "loopInput": [
//@[line261->line237]     ],
  'one'
//@[line262->line235]       "one",
  'two'
//@[line263->line236]       "two"
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[line265->line031]       {
//@[line265->line032]         "name": "arrayOfStringsViaLoop",
//@[line265->line033]         "count": "[length(variables('loopInput'))]",
//@[line265->line034]         "input": "[format('prefix-{0}-{1}', copyIndex('arrayOfStringsViaLoop'), variables('loopInput')[copyIndex('arrayOfStringsViaLoop')])]"
//@[line265->line035]       },
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[line266->line036]       {
//@[line266->line037]         "name": "arrayOfObjectsViaLoop",
//@[line266->line038]         "count": "[length(variables('loopInput'))]",
//@[line266->line039]         "input": {
//@[line266->line043]         }
//@[line266->line044]       },
  index: i
//@[line267->line040]           "index": "[copyIndex('arrayOfObjectsViaLoop')]",
  name: name
//@[line268->line041]           "name": "[variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')]]",
  value: 'prefix-${i}-${name}-suffix'
//@[line269->line042]           "value": "[format('prefix-{0}-{1}-suffix', copyIndex('arrayOfObjectsViaLoop'), variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')])]"
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[line271->line045]       {
//@[line271->line046]         "name": "arrayOfArraysViaLoop",
//@[line271->line047]         "count": "[length(variables('loopInput'))]",
//@[line271->line048]         "input": "[createArray(copyIndex('arrayOfArraysViaLoop'), variables('loopInput')[copyIndex('arrayOfArraysViaLoop')], format('prefix-{0}-{1}-suffix', copyIndex('arrayOfArraysViaLoop'), variables('loopInput')[copyIndex('arrayOfArraysViaLoop')]))]"
//@[line271->line049]       },
  i
  name
  'prefix-${i}-${name}-suffix'
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[line276->line050]       {
//@[line276->line051]         "name": "arrayOfBooleans",
//@[line276->line052]         "count": "[length(variables('loopInput'))]",
//@[line276->line053]         "input": "[equals(mod(copyIndex('arrayOfBooleans'), 2), 0)]"
//@[line276->line054]       },
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[line277->line055]       {
//@[line277->line056]         "name": "arrayOfHardCodedNumbers",
//@[line277->line057]         "count": "[length(range(0, 10))]",
//@[line277->line058]         "input": "[int(3)]"
//@[line277->line059]       },
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[line278->line060]       {
//@[line278->line061]         "name": "arrayOfHardCodedBools",
//@[line278->line062]         "count": "[length(range(0, 10))]",
//@[line278->line063]         "input": "[false()]"
//@[line278->line064]       },
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[line279->line065]       {
//@[line279->line066]         "name": "arrayOfHardCodedStrings",
//@[line279->line067]         "count": "[length(range(0, 3))]",
//@[line279->line068]         "input": "hi"
//@[line279->line069]       },
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[line280->line070]       {
//@[line280->line071]         "name": "arrayOfNonRuntimeFunctionCalls",
//@[line280->line072]         "count": "[length(range(0, 3))]",
//@[line280->line073]         "input": "[concat('hi', range(0, 3)[copyIndex('arrayOfNonRuntimeFunctionCalls')])]"
//@[line280->line074]       }

var multilineString = '''
//@[line282->line238]     "multilineString": "HELLO!\n",
HELLO!
'''

var multilineEmpty = ''''''
//@[line286->line239]     "multilineEmpty": "",
var multilineEmptyNewline = '''
//@[line287->line240]     "multilineEmptyNewline": "",
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''
//@[line291->line241]     "multilineExtraQuotes": "'abc'",

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
//@[line294->line242]     "multilineExtraQuotesNewlines": "'\nabc\n'",
abc
''''

var multilineSingleLine = '''hello!'''
//@[line298->line243]     "multilineSingleLine": "hello!",

var multilineFormatted = format('''
//@[line300->line244]     "multilineFormatted": "[format('Hello,\nmy\nname is\n{0}\n', 'Anthony')]",
Hello,
my
name is
{0}
''', 'Anthony')

var multilineJavaScript = '''
//@[line307->line245]     "multilineJavaScript": "// NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n",
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
//@[line326->line246]     "providersTest": "[providers('Microsoft.Resources').namespace]",
var providersTest2 = providers('Microsoft.Resources', 'deployments').locations
//@[line327->line247]     "providersTest2": "[providers('Microsoft.Resources', 'deployments').locations]",

var copyBlockInObject = {
//@[line329->line248]     "copyBlockInObject": {
//@[line329->line256]     },
  copy: [
//@[line330->line249]       "[string('copy')]": [
//@[line330->line255]       ]
    {
//@[line331->line250]         {
//@[line331->line254]         }
      name: 'blah'
//@[line332->line251]           "name": "blah",
      count: '[notAFunction()]'
//@[line333->line252]           "count": "[[notAFunction()]",
      input: {}
//@[line334->line253]           "input": {}
    }
  ]
}

var joinedString = join(['I', 'love', 'Bicep!'], ' ')
//@[line339->line257]     "joinedString": "[join(createArray('I', 'love', 'Bicep!'), ' ')]",

var prefix = take('food', 3)
//@[line341->line258]     "prefix": "[take('food', 3)]",
var isPrefixed = startsWith('food', 'foo')
//@[line342->line259]     "isPrefixed": "[startsWith('food', 'foo')]"

