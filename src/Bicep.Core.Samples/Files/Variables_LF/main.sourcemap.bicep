
// int
@sys.description('an int variable')
var myInt = 42
//@[77:77]     "myInt": 42,

// string
@sys.description('a string variable')
var myStr = 'str'
//@[78:78]     "myStr": "str",
var curliesWithNoInterp = '}{1}{'
//@[79:79]     "curliesWithNoInterp": "}{1}{",
var interp1 = 'abc${123}def'
//@[80:80]     "interp1": "[format('abc{0}def', 123)]",
var interp2 = '${123}def'
//@[81:81]     "interp2": "[format('{0}def', 123)]",
var interp3 = 'abc${123}'
//@[82:82]     "interp3": "[format('abc{0}', 123)]",
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[83:83]     "interp4": "[format('abc{0}{1}jk$l{2}p$', 123, 456, 789)]",
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[84:84]     "doubleInterp": "[format('abc{0}_{1}', format('def{0}', 123), format('{0}{1}', 456, 789))]",
var curliesInInterp = '{${123}{0}${true}}'
//@[85:85]     "curliesInInterp": "[format('{{{0}{{0}}{1}}}', 123, true())]",

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[86:86]     "bracketInTheMiddle": "a[b]",
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
//@[87:87]     "bracketAtBeginning": "[test",
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
//@[88:88]     "enclosingBrackets": "[[test]",
var emptyJsonArray = '[]'
//@[89:89]     "emptyJsonArray": "[[]",
var interpolatedBrackets = '[${myInt}]'
//@[90:90]     "interpolatedBrackets": "[format('[{0}]', variables('myInt'))]",
var nestedBrackets = '[test[]test2]'
//@[91:91]     "nestedBrackets": "[[test[]test2]",
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[92:92]     "nestedInterpolatedBrackets": "[format('[{0}]', variables('emptyJsonArray'))]",
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[93:93]     "bracketStringInExpression": "[concat('[', '''test''', ']')]",

// booleans
@sys.description('a bool variable')
var myTruth = true
//@[94:94]     "myTruth": true,
var myFalsehood = false
//@[95:95]     "myFalsehood": false,

var myEmptyObj = { }
//@[96:96]     "myEmptyObj": {},
var myEmptyArray = [ ]
//@[97:97]     "myEmptyArray": [],

// object
@sys.description('a object variable')
var myObj = {
//@[98:117]     "myObj": {
  a: 'a'
//@[99:99]       "a": "a",
  b: -12
//@[100:100]       "b": -12,
  c: true
//@[101:101]       "c": true,
  d: !true
//@[102:102]       "d": "[not(true())]",
  list: [
//@[103:111]       "list": [
    1
//@[104:104]         1,
    2
//@[105:105]         2,
    2+1
//@[106:106]         "[add(2, 1)]",
    {
      test: 144 > 33 && true || 99 <= 199
//@[108:108]           "test": "[or(and(greater(144, 33), true()), lessOrEquals(99, 199))]"
    }
    'a' =~ 'b'
//@[110:110]         "[equals(toLower('a'), toLower('b'))]"
  ]
  obj: {
//@[112:116]       "obj": {
    nested: [
//@[113:115]         "nested": [
      'hello'
//@[114:114]           "hello"
    ]
  }
}

@sys.description('a object with interp')
var objWithInterp = {
//@[118:122]     "objWithInterp": {
  '${myStr}': 1
//@[119:119]       "[format('{0}', variables('myStr'))]": 1,
  'abc${myStr}def': 2
//@[120:120]       "[format('abc{0}def', variables('myStr'))]": 2,
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[121:121]       "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]": "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]"
}

// array
var myArr = [
//@[123:127]     "myArr": [
  'pirates'
//@[124:124]       "pirates",
  'say'
//@[125:125]       "say",
  'arr'
//@[126:126]       "arr"
]

// array with objects
var myArrWithObjects = [
//@[128:137]     "myArrWithObjects": [
  {
    name: 'one'
//@[130:130]         "name": "one",
    enable: true
//@[131:131]         "enable": true
  }
  {
    name: 'two'
//@[134:134]         "name": "two",
    enable: false && false || 'two' !~ 'three'
//@[135:135]         "enable": "[or(and(false(), false()), not(equals(toLower('two'), toLower('three'))))]"
  }
]

var expressionIndexOnAny = any({
//@[138:138]     "expressionIndexOnAny": "[createObject()[resourceGroup().location]]",
})[az.resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[139:139]     "anyIndexOnAny": "[true()[false()]]",

var deploymentName = deployment().name
//@[140:140]     "deploymentName": "[deployment().name]",
var templateContentVersion = deployment().properties.template.contentVersion
//@[141:141]     "templateContentVersion": "[deployment().properties.template.contentVersion]",
var templateLinkUri = deployment().properties.templateLink.uri
//@[142:142]     "templateLinkUri": "[deployment().properties.templateLink.uri]",
var templateLinkId = deployment().properties.templateLink.id
//@[143:143]     "templateLinkId": "[deployment().properties.templateLink.id]",

var portalEndpoint = environment().portal
//@[144:144]     "portalEndpoint": "[environment().portal]",
var loginEndpoint = environment().authentication.loginEndpoint
//@[145:145]     "loginEndpoint": "[environment().authentication.loginEndpoint]",

var namedPropertyIndexer = {
//@[146:146]     "namedPropertyIndexer": "[createObject('foo', 's').foo]",
  foo: 's'
}['foo']

var intIndexer = [
//@[147:147]     "intIndexer": "[createArray('s')[0]]",
  's'
][0]

var functionOnIndexer1 = concat([
//@[148:148]     "functionOnIndexer1": "[concat(createArray('s')[0], 's')]",
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[149:149]     "functionOnIndexer2": "[concat(createArray()[0], 's')]",
][0], 's')

var functionOnIndexer3 = concat([
//@[150:150]     "functionOnIndexer3": "[concat(createArray()[0], 's')]",
][0], any('s'))

var singleQuote = '\''
//@[151:151]     "singleQuote": "'",
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[152:152]     "myPropertyName": "[format('{0}foo{1}', variables('singleQuote'), variables('singleQuote'))]",

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
var unusedIntermediateRef = unusedIntermediate.secondaryKey

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@[153:165]     "previousEmitLimit": [
  concat('s')
//@[154:154]       "[concat('s')]",
  '${4}'
//@[155:155]       "[format('{0}', 4)]",
  {
    a: {
//@[157:163]         "a": {
      b: base64('s')
//@[158:158]           "b": "[base64('s')]",
      c: concat([
//@[159:159]           "c": "[concat(createArray(add(12, 3)), createArray(not(true()), 'hello'))]",
        12 + 3
      ], [
        !true
        'hello'
      ])
      d: az.resourceGroup().location
//@[160:160]           "d": "[resourceGroup().location]",
      e: concat([
//@[161:161]           "e": "[concat(createArray(true()))]",
        true
      ])
      f: concat([
//@[162:162]           "f": "[concat(createArray(equals('s', 12)))]"
        's' == 12
      ])
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit2 = [
//@[166:178]     "previousEmitLimit2": [
  concat('s')
//@[167:167]       "[concat('s')]",
  '${4}'
//@[168:168]       "[format('{0}', 4)]",
  {
    a: {
//@[170:176]         "a": {
      b: base64('s')
//@[171:171]           "b": "[base64('s')]",
      c: union({
//@[172:172]           "c": "[union(createObject('a', add(12, 3)), createObject('b', not(true()), 'c', 'hello'))]",
        a: 12 + 3
      }, {
        b: !true
        c: 'hello'
      })
      d: az.resourceGroup().location
//@[173:173]           "d": "[resourceGroup().location]",
      e: union({
//@[174:174]           "e": "[union(createObject('x', true()), createObject())]",
        x: true
      }, {})
      f: intersection({
//@[175:175]           "f": "[intersection(createObject('q', equals('s', 12)), createObject())]"
        q: 's' == 12
      }, {})
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit3 = {
//@[179:184]     "previousEmitLimit3": {
  a: {
//@[180:183]       "a": {
    b: {
//@[181:181]         "b": "[equals(createObject('a', resourceGroup().location), 2)]",
      a: az.resourceGroup().location
    } == 2
    c: concat([
//@[182:182]         "c": "[concat(createArray(), createArray(true()))]"

    ], [
      true
    ])
  }
}

// #completionTest(0) -> declarations

var myVar = 'hello'
//@[185:185]     "myVar": "hello",
var myVar2 = any({
//@[186:188]     "myVar2": {
  something: myVar
//@[187:187]       "something": "[variables('myVar')]"
})
var myVar3 = any(any({
//@[189:191]     "myVar3": {
  something: myVar
//@[190:190]       "something": "[variables('myVar')]"
}))
var myVar4 = length(any(concat('s','a')))
//@[192:192]     "myVar4": "[length(concat('s', 'a'))]",

// verify that unqualified banned function identifiers can be used as declaration identifiers
var variables = true
//@[193:193]     "variables": true,
param parameters bool = true
//@[12:15]     "parameters": {
var if = true
//@[194:194]     "if": true,
var createArray = true
//@[195:195]     "createArray": true,
var createObject = true
//@[196:196]     "createObject": true,
var add = true
//@[197:197]     "add": true,
var sub = true
//@[198:198]     "sub": true,
var mul = true
//@[199:199]     "mul": true,
var div = true
//@[200:200]     "div": true,
param mod bool = true
//@[16:19]     "mod": {
var less = true
//@[201:201]     "less": true,
var lessOrEquals = true
//@[202:202]     "lessOrEquals": true,
var greater = true
//@[203:203]     "greater": true,
var greaterOrEquals = true
//@[204:204]     "greaterOrEquals": true,
param equals bool = true
//@[20:23]     "equals": {
var not = true
//@[205:205]     "not": true,
var and = true
//@[206:206]     "and": true,
var or = true
//@[207:207]     "or": true,
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[208:208]     "I_WANT_IT_ALL": "[and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(variables('variables'), parameters('parameters')), variables('if')), variables('createArray')), variables('createObject')), variables('add')), variables('sub')), variables('mul')), variables('div')), parameters('mod')), variables('less')), variables('lessOrEquals')), variables('greater')), variables('greaterOrEquals')), parameters('equals')), variables('not')), variables('and')), variables('or'))]",

// identifiers can have underscores
var _ = 3
//@[209:209]     "_": 3,
var __ = 10 * _
//@[210:210]     "__": "[mul(10, variables('_'))]",
var _0a_1b = true
//@[211:211]     "_0a_1b": true,
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[212:212]     "_1_": "[or(variables('_0a_1b'), equals(add(variables('__'), mod(variables('_'), 2)), 0))]",

// fully qualified access
var resourceGroup = 'something'
//@[213:213]     "resourceGroup": "something",
var resourceGroupName = az.resourceGroup().name
//@[214:214]     "resourceGroupName": "[resourceGroup().name]",
var resourceGroupObject = az.resourceGroup()
//@[215:215]     "resourceGroupObject": "[resourceGroup()]",
var propertyAccessFromObject = resourceGroupObject.name
//@[216:216]     "propertyAccessFromObject": "[variables('resourceGroupObject').name]",
var isTrue = sys.max(1, 2) == 3
//@[217:217]     "isTrue": "[equals(max(1, 2), 3)]",
var isFalse = !isTrue
//@[218:218]     "isFalse": "[not(variables('isTrue'))]",
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[219:219]     "someText": "[if(variables('isTrue'), concat('a', concat('b', 'c')), 'someText')]",

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
//@[220:223]     "scopesWithoutArmRepresentation": {
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@[221:221]       "subscription": "[createObject()]",
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@[222:222]       "resourceGroup": "[createObject()]"
}

var scopesWithArmRepresentation = {
//@[224:228]     "scopesWithArmRepresentation": {
  tenant: tenant()
//@[225:225]       "tenant": "[tenant()]",
  subscription: subscription()
//@[226:226]       "subscription": "[subscription()]",
  resourceGroup: az.resourceGroup()
//@[227:227]       "resourceGroup": "[resourceGroup()]"
}

// Issue #1332
var issue1332_propname = 'ptest'
//@[229:229]     "issue1332_propname": "ptest",
var issue1332 = true ? {
//@[230:230]     "issue1332": "[if(true(), createObject('prop1', createObject(format('{0}', variables('issue1332_propname')), createObject())), createObject())]",
    prop1: {
        '${issue1332_propname}': {}
    }
} : {}

// Issue #486
var myBigInt = 2199023255552
//@[231:231]     "myBigInt": 2199023255552,
var myIntExpression = 5 * 5
//@[232:232]     "myIntExpression": "[mul(5, 5)]",
var myBigIntExpression = 2199023255552 * 2
//@[233:233]     "myBigIntExpression": "[mul(json('2199023255552'), 2)]",
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[234:234]     "myBigIntExpression2": "[mul(json('2199023255552'), json('2199023255552'))]",

// variable loops
var incrementingNumbers = [for i in range(0,10) : i]
//@[27:31]         "name": "incrementingNumbers",
var loopInput = [
//@[235:238]     "loopInput": [
  'one'
//@[236:236]       "one",
  'two'
//@[237:237]       "two"
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[32:36]         "name": "arrayOfStringsViaLoop",
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[37:45]         "name": "arrayOfObjectsViaLoop",
  index: i
//@[41:41]           "index": "[copyIndex('arrayOfObjectsViaLoop')]",
  name: name
//@[42:42]           "name": "[variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')]]",
  value: 'prefix-${i}-${name}-suffix'
//@[43:43]           "value": "[format('prefix-{0}-{1}-suffix', copyIndex('arrayOfObjectsViaLoop'), variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')])]"
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[46:50]         "name": "arrayOfArraysViaLoop",
  i
  name
  'prefix-${i}-${name}-suffix'
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[51:55]         "name": "arrayOfBooleans",
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[56:60]         "name": "arrayOfHardCodedNumbers",
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[61:65]         "name": "arrayOfHardCodedBools",
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[66:70]         "name": "arrayOfHardCodedStrings",
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[71:75]         "name": "arrayOfNonRuntimeFunctionCalls",

var multilineString = '''
//@[239:239]     "multilineString": "HELLO!\n",
HELLO!
'''

var multilineEmpty = ''''''
//@[240:240]     "multilineEmpty": "",
var multilineEmptyNewline = '''
//@[241:241]     "multilineEmptyNewline": "",
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''
//@[242:242]     "multilineExtraQuotes": "'abc'",

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
//@[243:243]     "multilineExtraQuotesNewlines": "'\nabc\n'",
abc
''''

var multilineSingleLine = '''hello!'''
//@[244:244]     "multilineSingleLine": "hello!",

var multilineFormatted = format('''
//@[245:245]     "multilineFormatted": "[format('Hello,\nmy\nname is\n{0}\n', 'Anthony')]",
Hello,
my
name is
{0}
''', 'Anthony')

var multilineJavaScript = '''
//@[246:246]     "multilineJavaScript": "// NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n"
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

