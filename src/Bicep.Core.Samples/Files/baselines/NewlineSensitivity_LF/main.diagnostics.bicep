@allowed(['abc', 'def', 'ghi'])
param foo string
//@[06:009) [no-unused-params (Warning)] Parameter "foo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |foo|

var singleLineFunction = concat('abc', 'def')
//@[04:022) [no-unused-vars (Warning)] Variable "singleLineFunction" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |singleLineFunction|
//@[25:045) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat('abc', 'def')|

var multiLineFunction = concat(
//@[04:021) [no-unused-vars (Warning)] Variable "multiLineFunction" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineFunction|
//@[24:050) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(\n  'abc',\n  'def'\n)|
  'abc',
  'def'
)

var multiLineFunctionUnusualFormatting = concat(
//@[04:038) [no-unused-vars (Warning)] Variable "multiLineFunctionUnusualFormatting" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineFunctionUnusualFormatting|
//@[41:101) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(\n              'abc',          any(['hello']),\n'def')|
              'abc',          any(['hello']),
'def')

var nestedTest = concat(
//@[04:014) [no-unused-vars (Warning)] Variable "nestedTest" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nestedTest|
//@[17:108) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(\nconcat(\nconcat(\nconcat(\nconcat(\n'level',\n'one'),\n'two'),\n'three'),\n'four'),\n'five')|
concat(
//@[00:074) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(\nconcat(\nconcat(\nconcat(\n'level',\n'one'),\n'two'),\n'three'),\n'four')|
concat(
//@[00:057) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(\nconcat(\nconcat(\n'level',\n'one'),\n'two'),\n'three')|
concat(
//@[00:039) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(\nconcat(\n'level',\n'one'),\n'two')|
concat(
//@[00:023) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(\n'level',\n'one')|
'level',
'one'),
'two'),
'three'),
'four'),
'five')

var singleLineArray = ['abc', 'def']
//@[04:019) [no-unused-vars (Warning)] Variable "singleLineArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |singleLineArray|
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[04:033) [no-unused-vars (Warning)] Variable "singleLineArrayTrailingCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |singleLineArrayTrailingCommas|

var multiLineArray = [
//@[04:018) [no-unused-vars (Warning)] Variable "multiLineArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineArray|
  'abc'
  'def'
]

var mixedArray = ['abc', 'def'
//@[04:014) [no-unused-vars (Warning)] Variable "mixedArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mixedArray|
'ghi', 'jkl'
'lmn']

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[04:020) [no-unused-vars (Warning)] Variable "singleLineObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |singleLineObject|
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[04:034) [no-unused-vars (Warning)] Variable "singleLineObjectTrailingCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |singleLineObjectTrailingCommas|
var multiLineObject = {
//@[04:019) [no-unused-vars (Warning)] Variable "multiLineObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineObject|
  abc: 'def'
  ghi: 'jkl'
}
var mixedObject = { abc: 'abc', def: 'def'
//@[04:015) [no-unused-vars (Warning)] Variable "mixedObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mixedObject|
ghi: 'ghi', jkl: 'jkl'
lmn: 'lmn' }

var nestedMixed = {
//@[04:015) [no-unused-vars (Warning)] Variable "nestedMixed" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nestedMixed|
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[09:014) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |'def'|
    'bar', 'blah'
  ] }
}

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[04:020) [no-unused-vars (Warning)] Variable "brokenFormatting" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |brokenFormatting|

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''


'''
123,      233535
true
              ]

