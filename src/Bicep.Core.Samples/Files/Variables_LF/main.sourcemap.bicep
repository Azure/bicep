
// int
@sys.description('an int variable')
var myInt = 42
//@[76:76]     "myInt": 42,\r

// string
@sys.description('a string variable')
var myStr = 'str'
//@[77:77]     "myStr": "str",\r
var curliesWithNoInterp = '}{1}{'
//@[78:78]     "curliesWithNoInterp": "}{1}{",\r
var interp1 = 'abc${123}def'
//@[79:79]     "interp1": "[format('abc{0}def', 123)]",\r
var interp2 = '${123}def'
//@[80:80]     "interp2": "[format('{0}def', 123)]",\r
var interp3 = 'abc${123}'
//@[81:81]     "interp3": "[format('abc{0}', 123)]",\r
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[82:82]     "interp4": "[format('abc{0}{1}jk$l{2}p$', 123, 456, 789)]",\r
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[83:83]     "doubleInterp": "[format('abc{0}_{1}', format('def{0}', 123), format('{0}{1}', 456, 789))]",\r
var curliesInInterp = '{${123}{0}${true}}'
//@[84:84]     "curliesInInterp": "[format('{{{0}{{0}}{1}}}', 123, true())]",\r

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[85:85]     "bracketInTheMiddle": "a[b]",\r
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
//@[86:86]     "bracketAtBeginning": "[test",\r
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
//@[87:87]     "enclosingBrackets": "[[test]",\r
var emptyJsonArray = '[]'
//@[88:88]     "emptyJsonArray": "[[]",\r
var interpolatedBrackets = '[${myInt}]'
//@[89:89]     "interpolatedBrackets": "[format('[{0}]', variables('myInt'))]",\r
var nestedBrackets = '[test[]test2]'
//@[90:90]     "nestedBrackets": "[[test[]test2]",\r
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[91:91]     "nestedInterpolatedBrackets": "[format('[{0}]', variables('emptyJsonArray'))]",\r
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[92:92]     "bracketStringInExpression": "[concat('[', '''test''', ']')]",\r

// booleans
@sys.description('a bool variable')
var myTruth = true
//@[93:93]     "myTruth": true,\r
var myFalsehood = false
//@[94:94]     "myFalsehood": false,\r

var myEmptyObj = { }
//@[95:95]     "myEmptyObj": {},\r
var myEmptyArray = [ ]
//@[96:96]     "myEmptyArray": [],\r

// object
@sys.description('a object variable')
var myObj = {
//@[97:116]     "myObj": {\r
  a: 'a'
//@[98:98]       "a": "a",\r
  b: -12
//@[99:99]       "b": -12,\r
  c: true
//@[100:100]       "c": true,\r
  d: !true
//@[101:101]       "d": "[not(true())]",\r
  list: [
//@[102:110]       "list": [\r
    1
//@[103:103]         1,\r
    2
//@[104:104]         2,\r
    2+1
//@[105:105]         "[add(2, 1)]",\r
    {
      test: 144 > 33 && true || 99 <= 199
//@[107:107]           "test": "[or(and(greater(144, 33), true()), lessOrEquals(99, 199))]"\r
    }
    'a' =~ 'b'
//@[109:109]         "[equals(toLower('a'), toLower('b'))]"\r
  ]
  obj: {
//@[111:115]       "obj": {\r
    nested: [
//@[112:114]         "nested": [\r
      'hello'
//@[113:113]           "hello"\r
    ]
  }
}

@sys.description('a object with interp')
var objWithInterp = {
//@[117:121]     "objWithInterp": {\r
  '${myStr}': 1
//@[118:118]       "[format('{0}', variables('myStr'))]": 1,\r
  'abc${myStr}def': 2
//@[119:119]       "[format('abc{0}def', variables('myStr'))]": 2,\r
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[120:120]       "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]": "[format('{0}abc{1}', variables('interp1'), variables('interp2'))]"\r
}

// array
var myArr = [
//@[122:126]     "myArr": [\r
  'pirates'
//@[123:123]       "pirates",\r
  'say'
//@[124:124]       "say",\r
  'arr'
//@[125:125]       "arr"\r
]

// array with objects
var myArrWithObjects = [
//@[127:136]     "myArrWithObjects": [\r
  {
    name: 'one'
//@[129:129]         "name": "one",\r
    enable: true
//@[130:130]         "enable": true\r
  }
  {
    name: 'two'
//@[133:133]         "name": "two",\r
    enable: false && false || 'two' !~ 'three'
//@[134:134]         "enable": "[or(and(false(), false()), not(equals(toLower('two'), toLower('three'))))]"\r
  }
]

var expressionIndexOnAny = any({
//@[137:137]     "expressionIndexOnAny": "[createObject()[resourceGroup().location]]",\r
})[az.resourceGroup().location]

var anyIndexOnAny = any(true)[any(false)]
//@[138:138]     "anyIndexOnAny": "[true()[false()]]",\r

var deploymentName = deployment().name
//@[139:139]     "deploymentName": "[deployment().name]",\r
var templateContentVersion = deployment().properties.template.contentVersion
//@[140:140]     "templateContentVersion": "[deployment().properties.template.contentVersion]",\r
var templateLinkUri = deployment().properties.templateLink.uri
//@[141:141]     "templateLinkUri": "[deployment().properties.templateLink.uri]",\r
var templateLinkId = deployment().properties.templateLink.id
//@[142:142]     "templateLinkId": "[deployment().properties.templateLink.id]",\r

var portalEndpoint = environment().portal
//@[143:143]     "portalEndpoint": "[environment().portal]",\r
var loginEndpoint = environment().authentication.loginEndpoint
//@[144:144]     "loginEndpoint": "[environment().authentication.loginEndpoint]",\r

var namedPropertyIndexer = {
//@[145:145]     "namedPropertyIndexer": "[createObject('foo', 's').foo]",\r
  foo: 's'
}['foo']

var intIndexer = [
//@[146:146]     "intIndexer": "[createArray('s')[0]]",\r
  's'
][0]

var functionOnIndexer1 = concat([
//@[147:147]     "functionOnIndexer1": "[concat(createArray('s')[0], 's')]",\r
  's'
][0], 's')

var functionOnIndexer2 = concat([
//@[148:148]     "functionOnIndexer2": "[concat(createArray()[0], 's')]",\r
][0], 's')

var functionOnIndexer3 = concat([
//@[149:149]     "functionOnIndexer3": "[concat(createArray()[0], 's')]",\r
][0], any('s'))

var singleQuote = '\''
//@[150:150]     "singleQuote": "'",\r
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[151:151]     "myPropertyName": "[format('{0}foo{1}', variables('singleQuote'), variables('singleQuote'))]",\r

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
var unusedIntermediateRef = unusedIntermediate.secondaryKey

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@[152:164]     "previousEmitLimit": [\r
  concat('s')
//@[153:153]       "[concat('s')]",\r
  '${4}'
//@[154:154]       "[format('{0}', 4)]",\r
  {
    a: {
//@[156:162]         "a": {\r
      b: base64('s')
//@[157:157]           "b": "[base64('s')]",\r
      c: concat([
//@[158:158]           "c": "[concat(createArray(add(12, 3)), createArray(not(true()), 'hello'))]",\r
        12 + 3
      ], [
        !true
        'hello'
      ])
      d: az.resourceGroup().location
//@[159:159]           "d": "[resourceGroup().location]",\r
      e: concat([
//@[160:160]           "e": "[concat(createArray(true()))]",\r
        true
      ])
      f: concat([
//@[161:161]           "f": "[concat(createArray(equals('s', 12)))]"\r
        's' == 12
      ])
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit2 = [
//@[165:177]     "previousEmitLimit2": [\r
  concat('s')
//@[166:166]       "[concat('s')]",\r
  '${4}'
//@[167:167]       "[format('{0}', 4)]",\r
  {
    a: {
//@[169:175]         "a": {\r
      b: base64('s')
//@[170:170]           "b": "[base64('s')]",\r
      c: union({
//@[171:171]           "c": "[union(createObject('a', add(12, 3)), createObject('b', not(true()), 'c', 'hello'))]",\r
        a: 12 + 3
      }, {
        b: !true
        c: 'hello'
      })
      d: az.resourceGroup().location
//@[172:172]           "d": "[resourceGroup().location]",\r
      e: union({
//@[173:173]           "e": "[union(createObject('x', true()), createObject())]",\r
        x: true
      }, {})
      f: intersection({
//@[174:174]           "f": "[intersection(createObject('q', equals('s', 12)), createObject())]"\r
        q: 's' == 12
      }, {})
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit3 = {
//@[178:183]     "previousEmitLimit3": {\r
  a: {
//@[179:182]       "a": {\r
    b: {
//@[180:180]         "b": "[equals(createObject('a', resourceGroup().location), 2)]",\r
      a: az.resourceGroup().location
    } == 2
    c: concat([
//@[181:181]         "c": "[concat(createArray(), createArray(true()))]"\r

    ], [
      true
    ])
  }
}

// #completionTest(0) -> declarations

var myVar = 'hello'
//@[184:184]     "myVar": "hello",\r
var myVar2 = any({
//@[185:187]     "myVar2": {\r
  something: myVar
//@[186:186]       "something": "[variables('myVar')]"\r
})
var myVar3 = any(any({
//@[188:190]     "myVar3": {\r
  something: myVar
//@[189:189]       "something": "[variables('myVar')]"\r
}))
var myVar4 = length(any(concat('s','a')))
//@[191:191]     "myVar4": "[length(concat('s', 'a'))]",\r

// verify that unqualified banned function identifiers can be used as declaration identifiers
var variables = true
//@[192:192]     "variables": true,\r
param parameters bool = true
//@[11:14]     "parameters": {\r
var if = true
//@[193:193]     "if": true,\r
var createArray = true
//@[194:194]     "createArray": true,\r
var createObject = true
//@[195:195]     "createObject": true,\r
var add = true
//@[196:196]     "add": true,\r
var sub = true
//@[197:197]     "sub": true,\r
var mul = true
//@[198:198]     "mul": true,\r
var div = true
//@[199:199]     "div": true,\r
param mod bool = true
//@[15:18]     "mod": {\r
var less = true
//@[200:200]     "less": true,\r
var lessOrEquals = true
//@[201:201]     "lessOrEquals": true,\r
var greater = true
//@[202:202]     "greater": true,\r
var greaterOrEquals = true
//@[203:203]     "greaterOrEquals": true,\r
param equals bool = true
//@[19:22]     "equals": {\r
var not = true
//@[204:204]     "not": true,\r
var and = true
//@[205:205]     "and": true,\r
var or = true
//@[206:206]     "or": true,\r
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[207:207]     "I_WANT_IT_ALL": "[and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(and(variables('variables'), parameters('parameters')), variables('if')), variables('createArray')), variables('createObject')), variables('add')), variables('sub')), variables('mul')), variables('div')), parameters('mod')), variables('less')), variables('lessOrEquals')), variables('greater')), variables('greaterOrEquals')), parameters('equals')), variables('not')), variables('and')), variables('or'))]",\r

// identifiers can have underscores
var _ = 3
//@[208:208]     "_": 3,\r
var __ = 10 * _
//@[209:209]     "__": "[mul(10, variables('_'))]",\r
var _0a_1b = true
//@[210:210]     "_0a_1b": true,\r
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[211:211]     "_1_": "[or(variables('_0a_1b'), equals(add(variables('__'), mod(variables('_'), 2)), 0))]",\r

// fully qualified access
var resourceGroup = 'something'
//@[212:212]     "resourceGroup": "something",\r
var resourceGroupName = az.resourceGroup().name
//@[213:213]     "resourceGroupName": "[resourceGroup().name]",\r
var resourceGroupObject = az.resourceGroup()
//@[214:214]     "resourceGroupObject": "[resourceGroup()]",\r
var propertyAccessFromObject = resourceGroupObject.name
//@[215:215]     "propertyAccessFromObject": "[variables('resourceGroupObject').name]",\r
var isTrue = sys.max(1, 2) == 3
//@[216:216]     "isTrue": "[equals(max(1, 2), 3)]",\r
var isFalse = !isTrue
//@[217:217]     "isFalse": "[not(variables('isTrue'))]",\r
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[218:218]     "someText": "[if(variables('isTrue'), concat('a', concat('b', 'c')), 'someText')]",\r

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
//@[219:222]     "scopesWithoutArmRepresentation": {\r
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@[220:220]       "subscription": "[createObject()]",\r
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@[221:221]       "resourceGroup": "[createObject()]"\r
}

var scopesWithArmRepresentation = {
//@[223:227]     "scopesWithArmRepresentation": {\r
  tenant: tenant()
//@[224:224]       "tenant": "[tenant()]",\r
  subscription: subscription()
//@[225:225]       "subscription": "[subscription()]",\r
  resourceGroup: az.resourceGroup()
//@[226:226]       "resourceGroup": "[resourceGroup()]"\r
}

// Issue #1332
var issue1332_propname = 'ptest'
//@[228:228]     "issue1332_propname": "ptest",\r
var issue1332 = true ? {
//@[229:229]     "issue1332": "[if(true(), createObject('prop1', createObject(format('{0}', variables('issue1332_propname')), createObject())), createObject())]",\r
    prop1: {
        '${issue1332_propname}': {}
    }
} : {}

// Issue #486
var myBigInt = 2199023255552
//@[230:230]     "myBigInt": 2199023255552,\r
var myIntExpression = 5 * 5
//@[231:231]     "myIntExpression": "[mul(5, 5)]",\r
var myBigIntExpression = 2199023255552 * 2
//@[232:232]     "myBigIntExpression": "[mul(json('2199023255552'), 2)]",\r
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[233:233]     "myBigIntExpression2": "[mul(json('2199023255552'), json('2199023255552'))]",\r

// variable loops
var incrementingNumbers = [for i in range(0,10) : i]
//@[26:30]         "name": "incrementingNumbers",\r
var loopInput = [
//@[234:237]     "loopInput": [\r
  'one'
//@[235:235]       "one",\r
  'two'
//@[236:236]       "two"\r
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[31:35]         "name": "arrayOfStringsViaLoop",\r
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[36:44]         "name": "arrayOfObjectsViaLoop",\r
  index: i
//@[40:40]           "index": "[copyIndex('arrayOfObjectsViaLoop')]",\r
  name: name
//@[41:41]           "name": "[variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')]]",\r
  value: 'prefix-${i}-${name}-suffix'
//@[42:42]           "value": "[format('prefix-{0}-{1}-suffix', copyIndex('arrayOfObjectsViaLoop'), variables('loopInput')[copyIndex('arrayOfObjectsViaLoop')])]"\r
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[45:49]         "name": "arrayOfArraysViaLoop",\r
  i
  name
  'prefix-${i}-${name}-suffix'
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[50:54]         "name": "arrayOfBooleans",\r
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[55:59]         "name": "arrayOfHardCodedNumbers",\r
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[60:64]         "name": "arrayOfHardCodedBools",\r
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[65:69]         "name": "arrayOfHardCodedStrings",\r
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[70:74]         "name": "arrayOfNonRuntimeFunctionCalls",\r

var multilineString = '''
//@[238:238]     "multilineString": "HELLO!\n",\r
HELLO!
'''

var multilineEmpty = ''''''
//@[239:239]     "multilineEmpty": "",\r
var multilineEmptyNewline = '''
//@[240:240]     "multilineEmptyNewline": "",\r
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''
//@[241:241]     "multilineExtraQuotes": "'abc'",\r

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
//@[242:242]     "multilineExtraQuotesNewlines": "'\nabc\n'",\r
abc
''''

var multilineSingleLine = '''hello!'''
//@[243:243]     "multilineSingleLine": "hello!",\r

var multilineFormatted = format('''
//@[244:244]     "multilineFormatted": "[format('Hello,\nmy\nname is\n{0}\n', 'Anthony')]",\r
Hello,
my
name is
{0}
''', 'Anthony')

var multilineJavaScript = '''
//@[245:245]     "multilineJavaScript": "// NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n"\r
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

