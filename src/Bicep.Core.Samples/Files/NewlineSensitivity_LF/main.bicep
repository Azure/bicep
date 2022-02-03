var singleLineFunction = concat('abc', 'def')

var multiLineFunction = concat(
  'abc',
  'def'
)

var singleLineArray = ['abc', 'def']
var singleLineArrayTrailingCommas = ['abc', 'def',]

var multiLineArray = [
  'abc'
  'def'
]
var multiLineArrayCommas = [
  'abc',
  'def',
]

var mixedArray = ['abc', 'def'
'ghi', 'jkl',
'lmn']

var singleLineObject = { abc: 'def', ghi: 'jkl'}
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
var multiLineObject = {
  abc: 'def'
  ghi: 'jkl'
}
var multiLineObjectCommas = {
  abc: 'def',
  ghi: 'jkl',
}
var mixedObject = { abc: 'abc', def: 'def'
ghi: 'ghi', jkl: 'jkl',
lmn: 'lmn' }

var nestedMixed = {
  abc: { 'def': 'ghi', abc: 'def', foo: [
    'bar', 'blah',
  ] }
}
