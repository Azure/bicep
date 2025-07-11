param test string

var test_var = 'abcdef${test}ghi/jkl.${test}'
//@[4:12) [decompiler-cleanup (Warning)] The name of variable 'test_var' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality). (bicep core linter https://aka.ms/bicep/linter-diagnostics#decompiler-cleanup) |test_var|
//@[4:12) [no-unused-vars (Warning)] Variable "test_var" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |test_var|
var concats = 'abcdefghijkllmopqr'
//@[4:11) [no-unused-vars (Warning)] Variable "concats" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |concats|
var formats = '>>abc<<>>---abc.def---<<'
//@[4:11) [no-unused-vars (Warning)] Variable "formats" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |formats|
var escaped = '[]'
//@[4:11) [no-unused-vars (Warning)] Variable "escaped" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |escaped|
var keyescaping = {
//@[4:15) [no-unused-vars (Warning)] Variable "keyescaping" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |keyescaping|
  '[]': 'shouldbeescaped'
  '>>abc<<>>---abc.def---<<': 'shouldbeescaped'
  'abcdef${test}ghi/jkl.${test}': 'shouldbeescaped'
}

