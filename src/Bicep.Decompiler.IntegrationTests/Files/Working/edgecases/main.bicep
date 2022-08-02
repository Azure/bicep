param test string

var test_var = 'abcdef${test}ghi/jkl.${test}'
//@[4:12) [no-unused-vars (Warning)] Variable "test_var" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |test_var|
var concats = 'abcdefghijkllmopqr'
//@[4:11) [no-unused-vars (Warning)] Variable "concats" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |concats|
var formats = '>>abc<<>>---abc.def---<<'
//@[4:11) [no-unused-vars (Warning)] Variable "formats" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |formats|
var escaped = '[]'
//@[4:11) [no-unused-vars (Warning)] Variable "escaped" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |escaped|
var keyescaping = {
//@[4:15) [no-unused-vars (Warning)] Variable "keyescaping" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |keyescaping|
  '[]': 'shouldbeescaped'
  '>>abc<<>>---abc.def---<<': 'shouldbeescaped'
  'abcdef${test}ghi/jkl.${test}': 'shouldbeescaped'
}
