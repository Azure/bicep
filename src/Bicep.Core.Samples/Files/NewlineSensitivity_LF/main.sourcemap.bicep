@allowed(['abc', 'def', 'ghi'])
//@[12:17]       "type": "string",
param foo string
//@[11:18]     "foo": {

var singleLineFunction = concat('abc', 'def')
//@[21:21]     "singleLineFunction": "[concat('abc', 'def')]",

var multiLineFunction = concat(
//@[22:22]     "multiLineFunction": "[concat('abc', 'def')]",
  'abc',
  'def'
)

var multiLineFunctionUnusualFormatting = concat(
//@[23:23]     "multiLineFunctionUnusualFormatting": "[concat('abc', createArray('hello'), 'def')]",
              'abc',          any(['hello']),
'def')

var nestedTest = concat(
//@[24:24]     "nestedTest": "[concat(concat(concat(concat(concat('level', 'one'), 'two'), 'three'), 'four'), 'five')]",
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
//@[25:28]     "singleLineArray": [
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[29:32]     "singleLineArrayTrailingCommas": [

var multiLineArray = [
//@[33:36]     "multiLineArray": [
  'abc'
//@[34:34]       "abc",
  'def'
//@[35:35]       "def"
]

var mixedArray = ['abc', 'def'
//@[37:43]     "mixedArray": [
'ghi', 'jkl'
//@[40:41]       "ghi",
'lmn']
//@[42:42]       "lmn"

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[44:47]     "singleLineObject": {
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[48:51]     "singleLineObjectTrailingCommas": {
var multiLineObject = {
//@[52:55]     "multiLineObject": {
  abc: 'def'
//@[53:53]       "abc": "def",
  ghi: 'jkl'
//@[54:54]       "ghi": "jkl"
}
var mixedObject = { abc: 'abc', def: 'def'
//@[56:62]     "mixedObject": {
ghi: 'ghi', jkl: 'jkl'
//@[59:60]       "ghi": "ghi",
lmn: 'lmn' }
//@[61:61]       "lmn": "lmn"

var nestedMixed = {
//@[63:72]     "nestedMixed": {
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[64:71]       "abc": {
    'bar', 'blah'
//@[68:69]           "bar",
  ] }
}

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[73:82]     "brokenFormatting": [

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''
//@[75:78]       "asdfdsf",


'''
123,      233535
//@[79:80]       123,
true
//@[81:81]       true
              ]

