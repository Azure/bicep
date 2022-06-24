
// int
@sys.description('an int variable')
var myInt = 42
//@[76:76]     "myInt": 42,

// string
@sys.description('a string variable')
var myStr = 'str'
//@[77:77]     "myStr": "str",
var curliesWithNoInterp = '}{1}{'
//@[78:78]     "curliesWithNoInterp": "}{1}{",
var interp1 = 'abc${123}def'
//@[79:79]     "interp1": "[format('abc{0}def', 123)]",
var interp2 = '${123}def'
//@[80:80]     "interp2": "[format('{0}def', 123)]",
var interp3 = 'abc${123}'
//@[81:81]     "interp3": "[format('abc{0}', 123)]",
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[82:82]     "interp4": "[format('abc{0}{1}jk$l{2}p$', 123, 456, 789)]",
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[83:83]     "doubleInterp": "[format('abc{0}_{1}', format('def{0}', 123), format('{0}{1}', 456, 789))]",
var curliesInInterp = '{${123}{0}${true}}'
//@[84:84]     "curliesInInterp": "[format('{{{0}{{0}}{1}}}', 123, true())]",

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[85:85]     "bracketInTheMiddle": "a[b]",
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
//@[86:86]     "bracketAtBeginning": "[test",
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
//@[87:87]     "enclosingBrackets": "[[test]",
var emptyJsonArray = '[]'
//@[88:88]     "emptyJsonArray": "[[]",
var interpolatedBrackets = '[${myInt}]'
//@[89:89]     "interpolatedBrackets": "[format('[{0}]', variables('myInt'))]",
var nestedBrackets = '[test[]test2]'
//@[90:90]     "nestedBrackets": "[[test[]test2]",
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[91:91]     "nestedInterpolatedBrackets": "[format('[{0}]', variables('emptyJsonArray'))]",
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[92:92]     "bracketStringInExpression": "[concat('[', '''test''', ']')]",

// booleans
@sys.description('a bool variable')
var myTruth = true
//@[93:93]     "myTruth": true,
var myFalsehood = false
//@[94:94]     "myFalsehood": false,

var myEmptyObj = { }
//@[95:95]     "myEmptyObj": {},
var myEmptyArray = [ ]
//@[96:96]     "myEmptyArray": [],

// object
@sys.description('a object variable')
var myObj = {
//@[97:116]     "myObj": {
  a: 'a'
//@[98:98]       "a": "a",
  b: -12
//@[99:99]       "b": -12,
  c: true
//@[100:100]       "c": true,
  d: !true
//@[101:101]       "d": "[not(true())]",
  list: [
//@[102:110]       "list": [
    1
//@[103:103]         1,
    2
//@[104:104]         2,
    2+1
//@[105:105]         "[add(2, 1)]",
    {
      test: 144 > 33 && true || 99 <= 199
//@[107:107]           "test": "[or(and(greater(144, 33), true()), lessOrEquals(99, 199))]"
    }
    'a' =~ 'b'
//@[109:109]         "[equals(toLower('a'), toLower('b'))]"
  ]
  obj: {
//@[111:115]       "obj": {
    nested: [
//@[112:114]         "nested": [
      'hello'
//@[113:113]           "hello"
    ]
  }
}

@sys.description('a object with interp')
var objWithInterp = {
//@[117:121]     "objWithInterp": {
  '${myStr}': 1
//@[118:118]       "[format('{0}', variables('myStr'))]": 1,
  'abc${myStr}def': 2
//@[119:119]       "[format('abc{0}def', variables('myStr'))]": 2,
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[120:120]       "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]": "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]"
}

// array
var myArr = [
//@[122:126]     "myArr": [
  'pirates'
//@[123:123]       "pirates",
  'say'
//@[124:124]       "say",
  'arr'
//@[125:125]       "arr"
]

// array with objects
var myArrWithObjects = [
//@[127:136]     "myArrWithObjects": [
  {
    name: 'one'
//@[129:129]         "name": "one",
    enable: true
//@[130:130]         "enable": true
  }
  {
    name: 'two'
//@[133:133]         "name": "two",
    enable: false && false || 'two' !~ 'three'
//@[134:134]         "enable": "[or(and(false(), false()), not(equals(toLower('two'), toLower('three'))))]"
  }
]

var expressionIndexOnAny = any({
//@[137:137]     "expressionIndexOnAny": "[createObject()[resourceGroup().location]]",
})[az.resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[138:138]     "anyIndexOnAny": "[true()[false()]]",

var deploymentName = deployment().name
//@[139:139]     "deploymentName": "[deployment().name]",
var templateContentVersion = deployment().properties.template.contentVersion
//@[140:140]     "templateContentVersion": "[deployment().properties.template.contentVersion]",
var templateLinkUri = deployment().properties.templateLink.uri
//@[141:141]     "templateLinkUri": "[deployment().properties.templateLink.uri]",
var templateLinkId = deployment().properties.templateLink.id
//@[142:142]     "templateLinkId": "[deployment().properties.templateLink.id]",

var portalEndpoint = environment().portal
//@[143:143]     "portalEndpoint": "[environment().portal]",
var loginEndpoint = environment().authentication.loginEndpoint
//@[144:144]     "loginEndpoint": "[environment().authentication.loginEndpoint]",

var namedPropertyIndexer = {
//@[145:145]     "namedPropertyIndexer": "[createObject('foo', 's').foo]",
  foo: 's'
}['foo']

var intIndexer = [
//@[146:146]     "intIndexer": "[createArray('s')[0]]",
  's'
][0]

var functionOnIndexer1 = concat([
//@[147:147]     "functionOnIndexer1": "[concat(createArray('s')[0], 's')]",
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[148:148]     "functionOnIndexer2": "[concat(createArray()[0], 's')]",
][0], 's')

var functionOnIndexer3 = concat([
//@[149:149]     "functionOnIndexer3": "[concat(createArray()[0], 's')]",
][0], any('s'))

var singleQuote = '\''
//@[150:150]     "singleQuote": "'",
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[151:151]     "myPropertyName": "[format('{0}foo{1}', variables('singleQuote'), variables('singleQuote'))]",

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
var unusedIntermediateRef = unusedIntermediate.secondaryKey

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@[152:164]     "previousEmitLimit": [
  concat('s')
//@[153:153]       "[concat('s')]",
  '${4}'
//@[154:154]       "[format('{0}', 4)]",
  {
    a: {
//@[156:162]         "a": {
      b: base64('s')
//@[157:157]           "b": "[base64('s')]",
      c: concat([
//@[158:158]           "c": "[concat(createArray(add(12, 3)), createArray(not(true()), 'hello'))]",
        12 + 3
      ], [
        !true
        'hello'
      ])
      d: az.resourceGroup().location
//@[159:159]           "d": "[resourceGroup().location]",
      e: concat([
//@[160:160]           "e": "[concat(createArray(true()))]",
        true
      ])
      f: concat([
//@[161:161]           "f": "[concat(createArray(equals('s', 12)))]"
        's' == 12
      ])
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit2 = [
//@[165:177]     "previousEmitLimit2": [
  concat('s')
//@[166:166]       "[concat('s')]",
  '${4}'
//@[167:167]       "[format('{0}', 4)]",
  {
    a: {
//@[169:175]         "a": {
      b: base64('s')
//@[170:170]           "b": "[base64('s')]",
      c: union({
//@[171:171]           "c": "[union(createObject('a', add(12, 3)), createObject('b', not(true()), 'c', 'hello'))]",
        a: 12 + 3
      }, {
        b: !true
        c: 'hello'
      })
      d: az.resourceGroup().location
//@[172:172]           "d": "[resourceGroup().location]",
      e: union({
//@[173:173]           "e": "[union(createObject('x', true()), createObject())]",
        x: true
      }, {})
      f: intersection({
//@[174:174]           "f": "[intersection(createObject('q', equals('s', 12)), createObject())]"
        q: 's' == 12
      }, {})
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit3 = {
//@[178:183]     "previousEmitLimit3": {
  a: {
//@[179:182]       "a": {
    b: {
//@[180:180]         "b": "[equals(createObject('a', resourceGroup().location), 2)]",
      a: az.resourceGroup().location
    } == 2
    c: concat([
//@[181:181]         "c": "[concat(createArray(), createArray(true()))]"

    ], [
      true
    ])
  }
}

// #completionTest(0) -> declarations

var myVar = 'hello'
//@[184:184]     "myVar": "hello",
var myVar2 = any({
//@[185:187]     "myVar2": {
  something: myVar
//@[186:186]       "something": "[variables('myVar')]"
})
var myVar3 = any(any({
//@[188:190]     "myVar3": {
  something: myVar
//@[189:189]       "something": "[variables('myVar')]"
}))
var myVar4 = length(any(concat('s','a')))
//@[191:191]     "myVar4": "[length(concat('s', 'a'))]",

// verify that unqualified banned function identifiers can be used as declaration identifiers
var variables = true
//@[192:192]     "variables": true,
param parameters bool = true
//@[11:14]     "parameters": {
var if = true
//@[193:193]     "if": true,
var createArray = true
//@[194:194]     "createArray": true,
var createObject = true
//@[195:195]     "createObject": true,
var add = true
//@[196:196]     "add": true,
var sub = true
//@[197:197]     "sub": true,
var mul = true
//@[198:198]     "mul": true,
var div = true
//@[199:199]     "div": true,
param mod bool = true
//@[15:18]     "mod": {
var less = true
//@[200:200]     "less": true,
var lessOrEquals = true
//@[201:201]     "lessOrEquals": true,
var greater = true
//@[202:202]     "greater": true,
var greaterOrEquals = true
//@[203:203]     "greaterOrEquals": true,
param equals bool = true
//@[19:22]     "equals": {
var not = true
//@[204:204]     "not": true,
var and = true
//@[205:205]     "and": true,
var or = true
//@[206:206]     "or": true,
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[207:207]     "I_WANT_IT_ALL": "[and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(variables('variables'), parameters('parameters')), variables('if')), variables('createArray')), variables('createObject')), variables('add')), variables('sub')), variables('mul')), variables('div')), parameters('mod')), variables('less')), variables('lessOrEquals')), variables('greater')), variables('greaterOrEquals')), parameters('equals')), variables('not')), variables('and')), variables('or'))]",

// identifiers can have underscores
var _ = 3
//@[208:208]     "_": 3,
var __ = 10 * _
//@[209:209]     "__": "[mul(10, variables('_'))]",
var _0a_1b = true
//@[210:210]     "_0a_1b": true,
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[211:211]     "_1_": "[or(variables('_0a_1b'), equals(add(variables('__'), mod(variables('_'), 2)), 0))]",

// fully qualified access
var resourceGroup = 'something'
//@[212:212]     "resourceGroup": "something",
var resourceGroupName = az.resourceGroup().name
//@[213:213]     "resourceGroupName": "[resourceGroup().name]",
var resourceGroupObject = az.resourceGroup()
//@[214:214]     "resourceGroupObject": "[resourceGroup()]",
var propertyAccessFromObject = resourceGroupObject.name
//@[215:215]     "propertyAccessFromObject": "[variables('resourceGroupObject').name]",
var isTrue = sys.max(1, 2) == 3
//@[216:216]     "isTrue": "[equals(max(1, 2), 3)]",
var isFalse = !isTrue
//@[217:217]     "isFalse": "[not(variables('isTrue'))]",
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[218:218]     "someText": "[if(variables('isTrue'), concat('a', concat('b', 'c')), 'someText')]",

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
//@[219:222]     "scopesWithoutArmRepresentation": {
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@[220:220]       "subscription": "[createObject()]",
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@[221:221]       "resourceGroup": "[createObject()]"
}

var scopesWithArmRepresentation = {
//@[223:227]     "scopesWithArmRepresentation": {
  tenant: tenant()
//@[224:224]       "tenant": "[tenant()]",
  subscription: subscription()
//@[225:225]       "subscription": "[subscription()]",
  resourceGroup: az.resourceGroup()
//@[226:226]       "resourceGroup": "[resourceGroup()]"
}

// Issue #1332
var issue1332_propname = 'ptest'
//@[228:228]     "issue1332_propname": "ptest",
var issue1332 = true ? {
//@[229:229]     "issue1332": "[if(true(), createObject('prop1', createObject(format('{0}', variables('issue1332_propname')), createObject())), createObject())]",
    prop1: {
        '${issue1332_propname}': {}
    }
} : {}

// Issue #486
var myBigInt = 2199023255552
//@[230:230]     "myBigInt": 2199023255552,
var myIntExpression = 5 * 5
//@[231:231]     "myIntExpression": "[mul(5, 5)]",
var myBigIntExpression = 2199023255552 * 2
//@[232:232]     "myBigIntExpression": "[mul(json('2199023255552'), 2)]",
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[233:233]     "myBigIntExpression2": "[mul(json('2199023255552'), json('2199023255552'))]",

// variable loops
var incrementingNumbers = [for i in range(0,10) : i]
//@[26:30]         "name": "incrementingNumbers",
var loopInput = [
//@[234:237]     "loopInput": [
  'one'
//@[235:235]       "one",
  'two'
//@[236:236]       "two"
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[31:35]         "name": "arrayOfStringsViaLoop",
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[36:44]         "name": "arrayOfObjectsViaLoop",
  index: i
//@[40:40]           "index": "[copyIndex('arrayOfObjectsViaLoop')]",
  name: name
//@[41:41]           "name": "[variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')]]",
  value: 'prefix-${i}-${name}-suffix'
//@[42:42]           "value": "[format('prefix-{0}-{1}-suffix', copyIndex('arrayOfObjectsViaLoop'), variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')])]"
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[45:49]         "name": "arrayOfArraysViaLoop",
  i
  name
  'prefix-${i}-${name}-suffix'
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[50:54]         "name": "arrayOfBooleans",
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[55:59]         "name": "arrayOfHardCodedNumbers",
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[60:64]         "name": "arrayOfHardCodedBools",
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[65:69]         "name": "arrayOfHardCodedStrings",
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[70:74]         "name": "arrayOfNonRuntimeFunctionCalls",

var multilineString = '''
//@[238:238]     "multilineString": "HELLO!\n",
HELLO!
'''

var multilineEmpty = ''''''
//@[239:239]     "multilineEmpty": "",
var multilineEmptyNewline = '''
//@[240:240]     "multilineEmptyNewline": "",
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''
//@[241:241]     "multilineExtraQuotes": "'abc'",

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
//@[242:242]     "multilineExtraQuotesNewlines": "'\nabc\n'",
abc
''''

var multilineSingleLine = '''hello!'''
//@[243:243]     "multilineSingleLine": "hello!",

var multilineFormatted = format('''
//@[244:244]     "multilineFormatted": "[format('Hello,\nmy\nname is\n{0}\n', 'Anthony')]",
Hello,
my
name is
{0}
''', 'Anthony')

var multilineJavaScript = '''
//@[245:245]     "multilineJavaScript": "// NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n",
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

var copyBlockInObject = {
//@[246:254]     "copyBlockInObject": {
  copy: [
//@[247:253]       "[string('copy')]": [
    {
      name: 'blah'
//@[249:249]           "name": "blah",
      count: '[notAFunction()]'
//@[250:250]           "count": "[[notAFunction()]",
      input: {}
//@[251:251]           "input": {}
    }
  ]
}

var joinedString = join(['I', 'love', 'Bicep!'], ' ')
//@[255:255]     "joinedString": "[join(createArray('I', 'love', 'Bicep!'), ' ')]"

