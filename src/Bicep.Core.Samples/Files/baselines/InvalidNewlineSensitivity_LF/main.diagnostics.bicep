var singleLineFunctionNoCommas = concat('abc' 'def')
//@[04:30) [no-unused-vars (Warning)] Variable "singleLineFunctionNoCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |singleLineFunctionNoCommas|
//@[46:46) [BCP236 (Error)] Expected a new line or comma character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP236) ||

var multiLineFunctionTrailingComma = concat(
//@[04:34) [no-unused-vars (Warning)] Variable "multiLineFunctionTrailingComma" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineFunctionTrailingComma|
  'abc',
  'def',
)
//@[00:01) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |)|

var singleLineArrayNoCommas = ['abc' 'def']
//@[04:27) [no-unused-vars (Warning)] Variable "singleLineArrayNoCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |singleLineArrayNoCommas|
//@[37:37) [BCP236 (Error)] Expected a new line or comma character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP236) ||

var multiLineArrayMultipleCommas = [
//@[04:32) [no-unused-vars (Warning)] Variable "multiLineArrayMultipleCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineArrayMultipleCommas|
  'abc',,
//@[08:09) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
  'def',,,
//@[08:09) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
]


var singleLineObjectNoCommas = { abc: 'def' ghi: 'jkl'}
//@[04:28) [no-unused-vars (Warning)] Variable "singleLineObjectNoCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |singleLineObjectNoCommas|
//@[44:44) [BCP236 (Error)] Expected a new line or comma character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP236) ||
var multiLineObjectMultipleCommas = {
//@[04:33) [no-unused-vars (Warning)] Variable "multiLineObjectMultipleCommas" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multiLineObjectMultipleCommas|
  abc: 'def',,,
//@[13:14) [BCP022 (Error)] Expected a property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP022) |,|
//@[15:15) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
  ghi: 'jkl',,
//@[13:14) [BCP022 (Error)] Expected a property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP022) |,|
//@[14:14) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
}

