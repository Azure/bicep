@allowed(['abc', 'def', 'ghi'])
//@[line00->line13]       "allowedValues": [
//@[line00->line14]         "abc",
//@[line00->line15]         "def",
//@[line00->line16]         "ghi"
//@[line00->line17]       ]
param foo string
//@[line01->line11]     "foo": {
//@[line01->line12]       "type": "string",
//@[line01->line18]     }

var singleLineFunction = concat('abc', 'def')
//@[line03->line21]     "singleLineFunction": "[concat('abc', 'def')]",

var multiLineFunction = concat(
//@[line05->line22]     "multiLineFunction": "[concat('abc', 'def')]",
  'abc',
  'def'
)

var multiLineFunctionUnusualFormatting = concat(
//@[line10->line23]     "multiLineFunctionUnusualFormatting": "[concat('abc', createArray('hello'), 'def')]",
              'abc',          any(['hello']),
'def')

var nestedTest = concat(
//@[line14->line24]     "nestedTest": "[concat(concat(concat(concat(concat('level', 'one'), 'two'), 'three'), 'four'), 'five')]",
concat(
concat(
concat(
concat(
'level',
'one'),
'two'),
'three'),
'four'),
'five')

var singleLineArray = ['abc', 'def']
//@[line26->line25]     "singleLineArray": [
//@[line26->line26]       "abc",
//@[line26->line27]       "def"
//@[line26->line28]     ],
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[line27->line29]     "singleLineArrayTrailingCommas": [
//@[line27->line30]       "abc",
//@[line27->line31]       "def"
//@[line27->line32]     ],

var multiLineArray = [
//@[line29->line33]     "multiLineArray": [
//@[line29->line36]     ],
  'abc'
//@[line30->line34]       "abc",
  'def'
//@[line31->line35]       "def"
]

var mixedArray = ['abc', 'def'
//@[line34->line37]     "mixedArray": [
//@[line34->line38]       "abc",
//@[line34->line39]       "def",
//@[line34->line43]     ],
'ghi', 'jkl'
//@[line35->line40]       "ghi",
//@[line35->line41]       "jkl",
'lmn']
//@[line36->line42]       "lmn"

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[line38->line44]     "singleLineObject": {
//@[line38->line45]       "abc": "def",
//@[line38->line46]       "ghi": "jkl"
//@[line38->line47]     },
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[line39->line48]     "singleLineObjectTrailingCommas": {
//@[line39->line49]       "abc": "def",
//@[line39->line50]       "ghi": "jkl"
//@[line39->line51]     },
var multiLineObject = {
//@[line40->line52]     "multiLineObject": {
//@[line40->line55]     },
  abc: 'def'
//@[line41->line53]       "abc": "def",
  ghi: 'jkl'
//@[line42->line54]       "ghi": "jkl"
}
var mixedObject = { abc: 'abc', def: 'def'
//@[line44->line56]     "mixedObject": {
//@[line44->line57]       "abc": "abc",
//@[line44->line58]       "def": "def",
//@[line44->line62]     },
ghi: 'ghi', jkl: 'jkl'
//@[line45->line59]       "ghi": "ghi",
//@[line45->line60]       "jkl": "jkl",
lmn: 'lmn' }
//@[line46->line61]       "lmn": "lmn"

var nestedMixed = {
//@[line48->line63]     "nestedMixed": {
//@[line48->line72]     },
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[line49->line64]       "abc": {
//@[line49->line65]         "def": "ghi",
//@[line49->line66]         "abc": "def",
//@[line49->line67]         "foo": [
//@[line49->line70]         ]
//@[line49->line71]       }
    'bar', 'blah'
//@[line50->line68]           "bar",
//@[line50->line69]           "blah"
  ] }
}

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[line54->line73]     "brokenFormatting": [
//@[line54->line74]       "bar",
//@[line54->line82]     ]

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''
//@[line58->line75]       "asdfdsf",
//@[line58->line76]       12324,
//@[line58->line77]       "",
//@[line58->line78]       "\n\n",


'''
123,      233535
//@[line62->line79]       123,
//@[line62->line80]       233535,
true
//@[line63->line81]       true
              ]

