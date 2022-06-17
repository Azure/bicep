@allowed(['abc', 'def', 'ghi'])
//@[15:17]         "abc",
param foo string
//@[12:19]     "foo": {

var singleLineFunction = concat('abc', 'def')
//@[22:22]     "singleLineFunction": "[concat('abc', 'def')]",

var multiLineFunction = concat(
//@[23:23]     "multiLineFunction": "[concat('abc', 'def')]",
  'abc',
  'def'
)

var multiLineFunctionUnusualFormatting = concat(
//@[24:24]     "multiLineFunctionUnusualFormatting": "[concat('abc', createArray('hello'), 'def')]",
              'abc',          any(['hello']),
'def')

var nestedTest = concat(
//@[25:25]     "nestedTest": "[concat(concat(concat(concat(concat('level', 'one'), 'two'), 'three'), 'four'), 'five')]",
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
//@[26:29]     "singleLineArray": [
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[30:33]     "singleLineArrayTrailingCommas": [

var multiLineArray = [
//@[34:37]     "multiLineArray": [
  'abc'
//@[35:35]       "abc",
  'def'
//@[36:36]       "def"
]

var mixedArray = ['abc', 'def'
//@[38:44]     "mixedArray": [
'ghi', 'jkl'
//@[41:42]       "ghi",
'lmn']
//@[43:43]       "lmn"

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[45:48]     "singleLineObject": {
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[49:52]     "singleLineObjectTrailingCommas": {
var multiLineObject = {
//@[53:56]     "multiLineObject": {
  abc: 'def'
//@[54:54]       "abc": "def",
  ghi: 'jkl'
//@[55:55]       "ghi": "jkl"
}
var mixedObject = { abc: 'abc', def: 'def'
//@[57:63]     "mixedObject": {
ghi: 'ghi', jkl: 'jkl'
//@[60:61]       "ghi": "ghi",
lmn: 'lmn' }
//@[62:62]       "lmn": "lmn"

var nestedMixed = {
//@[64:73]     "nestedMixed": {
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[65:72]       "abc": {
    'bar', 'blah'
//@[69:70]           "bar",
  ] }
}

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[74:83]     "brokenFormatting": [

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''
//@[76:79]       "asdfdsf",


'''
123,      233535
//@[80:81]       123,
true
//@[82:82]       true
              ]

