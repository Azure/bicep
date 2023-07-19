var singleLineFunctionNoCommas = concat('abc' 'def')
//@[4:30) Variable singleLineFunctionNoCommas. Type: string. Declaration start char: 0, length: 52

var multiLineFunctionTrailingComma = concat(
//@[4:34) Variable multiLineFunctionTrailingComma. Type: string. Declaration start char: 0, length: 64
  'abc',
  'def',
)

var singleLineArrayNoCommas = ['abc' 'def']
//@[4:27) Variable singleLineArrayNoCommas. Type: ['abc', 'def']. Declaration start char: 0, length: 43

var multiLineArrayMultipleCommas = [
//@[4:32) Variable multiLineArrayMultipleCommas. Type: ['abc', 'def']. Declaration start char: 0, length: 59
  'abc',,
  'def',,,
]


var singleLineObjectNoCommas = { abc: 'def' ghi: 'jkl'}
//@[4:28) Variable singleLineObjectNoCommas. Type: object. Declaration start char: 0, length: 55
var multiLineObjectMultipleCommas = {
//@[4:33) Variable multiLineObjectMultipleCommas. Type: error. Declaration start char: 0, length: 70
  abc: 'def',,,
  ghi: 'jkl',,
}

