var singleLineFunction = concat('abc', 'def')
//@[4:22) Variable singleLineFunction. Type: string. Declaration start char: 0, length: 45

var multiLineFunction = concat(
//@[4:21) Variable multiLineFunction. Type: string. Declaration start char: 0, length: 50
  'abc',
  'def'
)

var singleLineArray = ['abc', 'def']
//@[4:19) Variable singleLineArray. Type: array. Declaration start char: 0, length: 36
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[4:33) Variable singleLineArrayTrailingCommas. Type: array. Declaration start char: 0, length: 51

var multiLineArray = [
//@[4:18) Variable multiLineArray. Type: array. Declaration start char: 0, length: 40
  'abc'
  'def'
]
var multiLineArrayCommas = [
//@[4:24) Variable multiLineArrayCommas. Type: array. Declaration start char: 0, length: 48
  'abc',
  'def',
]

var mixedArray = ['abc', 'def'
//@[4:14) Variable mixedArray. Type: array. Declaration start char: 0, length: 51
'ghi', 'jkl',
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
var multiLineObjectCommas = {
//@[4:25) Variable multiLineObjectCommas. Type: object. Declaration start char: 0, length: 59
  abc: 'def',
  ghi: 'jkl',
}
var mixedObject = { abc: 'abc', def: 'def'
//@[4:15) Variable mixedObject. Type: object. Declaration start char: 0, length: 79
ghi: 'ghi', jkl: 'jkl',
lmn: 'lmn' }

var nestedMixed = {
//@[4:15) Variable nestedMixed. Type: object. Declaration start char: 0, length: 88
  abc: { 'def': 'ghi', abc: 'def', foo: [
    'bar', 'blah',
  ] }
}

