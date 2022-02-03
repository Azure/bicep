var singleLineFunction = concat('abc', 'def')
//@[4:22) [no-unused-vars (Warning)] Variable "singleLineFunction" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singleLineFunction|
//@[25:45) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat('abc', 'def')|

var multiLineFunction = concat(
//@[4:21) [no-unused-vars (Warning)] Variable "multiLineFunction" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineFunction|
//@[24:50) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(\n  'abc',\n  'def'\n)|
  'abc',
  'def'
)

var singleLineArray = ['abc', 'def']
//@[4:19) [no-unused-vars (Warning)] Variable "singleLineArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singleLineArray|
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[4:33) [no-unused-vars (Warning)] Variable "singleLineArrayTrailingCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singleLineArrayTrailingCommas|

var multiLineArray = [
//@[4:18) [no-unused-vars (Warning)] Variable "multiLineArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineArray|
  'abc'
  'def'
]
var multiLineArrayCommas = [
//@[4:24) [no-unused-vars (Warning)] Variable "multiLineArrayCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineArrayCommas|
  'abc',
  'def',
]

var mixedArray = ['abc', 'def'
//@[4:14) [no-unused-vars (Warning)] Variable "mixedArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mixedArray|
'ghi', 'jkl',
'lmn']

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[4:20) [no-unused-vars (Warning)] Variable "singleLineObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singleLineObject|
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[4:34) [no-unused-vars (Warning)] Variable "singleLineObjectTrailingCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singleLineObjectTrailingCommas|
var multiLineObject = {
//@[4:19) [no-unused-vars (Warning)] Variable "multiLineObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineObject|
  abc: 'def'
  ghi: 'jkl'
}
var multiLineObjectCommas = {
//@[4:25) [no-unused-vars (Warning)] Variable "multiLineObjectCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineObjectCommas|
  abc: 'def',
  ghi: 'jkl',
}
var mixedObject = { abc: 'abc', def: 'def'
//@[4:15) [no-unused-vars (Warning)] Variable "mixedObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mixedObject|
ghi: 'ghi', jkl: 'jkl',
lmn: 'lmn' }

var nestedMixed = {
//@[4:15) [no-unused-vars (Warning)] Variable "nestedMixed" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedMixed|
  abc: { 'def': 'ghi', abc: 'def', foo: [
    'bar', 'blah',
  ] }
}

