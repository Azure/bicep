@allowed(['abc', 'def', 'ghi'])
//@      "allowedValues": [
//@        "abc",
//@        "def",
//@        "ghi"
//@      ]
param foo string
//@    "foo": {
//@      "type": "string",
//@    }

var singleLineFunction = concat('abc', 'def')
//@    "singleLineFunction": "[concat('abc', 'def')]",

var multiLineFunction = concat(
//@    "multiLineFunction": "[concat('abc', 'def')]",
  'abc',
  'def'
)

var multiLineFunctionUnusualFormatting = concat(
//@    "multiLineFunctionUnusualFormatting": "[concat('abc', createArray('hello'), 'def')]",
              'abc',          any(['hello']),
'def')

var nestedTest = concat(
//@    "nestedTest": "[concat(concat(concat(concat(concat('level', 'one'), 'two'), 'three'), 'four'), 'five')]",
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
//@    "singleLineArray": [
//@      "abc",
//@      "def"
//@    ],
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@    "singleLineArrayTrailingCommas": [
//@      "abc",
//@      "def"
//@    ],

var multiLineArray = [
//@    "multiLineArray": [
//@    ],
  'abc'
//@      "abc",
  'def'
//@      "def"
]

var mixedArray = ['abc', 'def'
//@    "mixedArray": [
//@      "abc",
//@      "def",
//@    ],
'ghi', 'jkl'
//@      "ghi",
//@      "jkl",
'lmn']
//@      "lmn"

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@    "singleLineObject": {
//@      "abc": "def",
//@      "ghi": "jkl"
//@    },
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@    "singleLineObjectTrailingCommas": {
//@      "abc": "def",
//@      "ghi": "jkl"
//@    },
var multiLineObject = {
//@    "multiLineObject": {
//@    },
  abc: 'def'
//@      "abc": "def",
  ghi: 'jkl'
//@      "ghi": "jkl"
}
var mixedObject = { abc: 'abc', def: 'def'
//@    "mixedObject": {
//@      "abc": "abc",
//@      "def": "def",
//@    },
ghi: 'ghi', jkl: 'jkl'
//@      "ghi": "ghi",
//@      "jkl": "jkl",
lmn: 'lmn' }
//@      "lmn": "lmn"

var nestedMixed = {
//@    "nestedMixed": {
//@    },
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@      "abc": {
//@        "def": "ghi",
//@        "abc": "def",
//@        "foo": [
//@        ]
//@      }
    'bar', 'blah'
//@          "bar",
//@          "blah"
  ] }
}

var brokenFormatting = [      /*foo */ 'bar'   /*
//@    "brokenFormatting": [
//@      "bar",
//@    ]

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''
//@      "asdfdsf",
//@      12324,
//@      "",
//@      "\n\n",


'''
123,      233535
//@      123,
//@      233535,
true
//@      true
              ]

