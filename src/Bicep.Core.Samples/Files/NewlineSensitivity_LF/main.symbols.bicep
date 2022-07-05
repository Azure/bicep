@allowed(['abc', 'def', 'ghi'])
param foo string
//@[6:09) Parameter foo. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 0, length: 48

var singleLineFunction = concat('abc', 'def')
//@[4:22) Variable singleLineFunction. Type: string. Declaration start char: 0, length: 45

var multiLineFunction = concat(
//@[4:21) Variable multiLineFunction. Type: string. Declaration start char: 0, length: 50
  'abc',
  'def'
)

var multiLineFunctionUnusualFormatting = concat(
//@[4:38) Variable multiLineFunctionUnusualFormatting. Type: string. Declaration start char: 0, length: 101
              'abc',          any(['hello']),
'def')

var nestedTest = concat(
//@[4:14) Variable nestedTest. Type: string. Declaration start char: 0, length: 108
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
//@[4:19) Variable singleLineArray. Type: ('abc' | 'def')[]. Declaration start char: 0, length: 36
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[4:33) Variable singleLineArrayTrailingCommas. Type: ('abc' | 'def')[]. Declaration start char: 0, length: 51

var multiLineArray = [
//@[4:18) Variable multiLineArray. Type: ('abc' | 'def')[]. Declaration start char: 0, length: 40
  'abc'
  'def'
]

var mixedArray = ['abc', 'def'
//@[4:14) Variable mixedArray. Type: ('abc' | 'def' | 'ghi' | 'jkl' | 'lmn')[]. Declaration start char: 0, length: 50
'ghi', 'jkl'
'lmn']

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[4:20) Variable singleLineObject. Type: object. Declaration start char: 0, length: 48
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[4:34) Variable singleLineObjectTrailingCommas. Type: object. Declaration start char: 0, length: 63
var multiLineObject = {
//@[4:19) Variable multiLineObject. Type: object. Declaration start char: 0, length: 51
  abc: 'def'
  ghi: 'jkl'
}
var mixedObject = { abc: 'abc', def: 'def'
//@[4:15) Variable mixedObject. Type: object. Declaration start char: 0, length: 78
ghi: 'ghi', jkl: 'jkl'
lmn: 'lmn' }

var nestedMixed = {
//@[4:15) Variable nestedMixed. Type: object. Declaration start char: 0, length: 87
  abc: { 'def': 'ghi', abc: 'def', foo: [
    'bar', 'blah'
  ] }
}

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[4:20) Variable brokenFormatting. Type: array. Declaration start char: 0, length: 172

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''


'''
123,      233535
true
              ]

