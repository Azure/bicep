@allowed(['abc', 'def', 'ghi'])
//@[14:16]         "abc",\r
param foo string
//@[11:18]     "foo": {\r

var singleLineFunction = concat('abc', 'def')
//@[21:21]     "singleLineFunction": "[concat('abc', 'def')]",\r

var multiLineFunction = concat(
//@[22:22]     "multiLineFunction": "[concat('abc', 'def')]",\r
  'abc',
  'def'
)

var multiLineFunctionUnusualFormatting = concat(
//@[23:23]     "multiLineFunctionUnusualFormatting": "[concat('abc', createArray('hello'), 'def')]",\r
              'abc',          any(['hello']),
'def')

var nestedTest = concat(
//@[24:24]     "nestedTest": "[concat(concat(concat(concat(concat('level', 'one'), 'two'), 'three'), 'four'), 'five')]",\r
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
//@[25:28]     "singleLineArray": [\r
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[29:32]     "singleLineArrayTrailingCommas": [\r

var multiLineArray = [
//@[33:36]     "multiLineArray": [\r
  'abc'
//@[34:34]       "abc",\r
  'def'
//@[35:35]       "def"\r
]

var mixedArray = ['abc', 'def'
//@[37:43]     "mixedArray": [\r
'ghi', 'jkl'
//@[40:41]       "ghi",\r
'lmn']
//@[42:42]       "lmn"\r

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[44:47]     "singleLineObject": {\r
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[48:51]     "singleLineObjectTrailingCommas": {\r
var multiLineObject = {
//@[52:55]     "multiLineObject": {\r
  abc: 'def'
//@[53:53]       "abc": "def",\r
  ghi: 'jkl'
//@[54:54]       "ghi": "jkl"\r
}
var mixedObject = { abc: 'abc', def: 'def'
//@[56:62]     "mixedObject": {\r
ghi: 'ghi', jkl: 'jkl'
//@[59:60]       "ghi": "ghi",\r
lmn: 'lmn' }
//@[61:61]       "lmn": "lmn"\r

var nestedMixed = {
//@[63:72]     "nestedMixed": {\r
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[64:71]       "abc": {\r
    'bar', 'blah'
//@[68:69]           "bar",\r
  ] }
}

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[73:82]     "brokenFormatting": [\r

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''
//@[75:78]       "asdfdsf",\r


'''
123,      233535
//@[79:80]       123,\r
true
//@[81:81]       true\r
              ]

