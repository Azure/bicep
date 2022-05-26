var singleLineFunctionNoCommas = concat('abc' 'def')
//@[4:30) [no-unused-vars (Warning)] Variable "singleLineFunctionNoCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singleLineFunctionNoCommas|
//@[46:46) [BCP235 (Error)] Expected a new line or comma character at this location. (CodeDescription: none) ||

var multiLineFunctionTrailingComma = concat(
//@[4:34) [no-unused-vars (Warning)] Variable "multiLineFunctionTrailingComma" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineFunctionTrailingComma|
  'abc',
  'def',
)
//@[0:1) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|

var singleLineArrayNoCommas = ['abc' 'def']
//@[4:27) [no-unused-vars (Warning)] Variable "singleLineArrayNoCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singleLineArrayNoCommas|
//@[37:37) [BCP235 (Error)] Expected a new line or comma character at this location. (CodeDescription: none) ||

var multiLineArrayMultipleCommas = [
//@[4:32) [no-unused-vars (Warning)] Variable "multiLineArrayMultipleCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineArrayMultipleCommas|
  'abc',,
//@[8:9) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |,|
  'def',,,
//@[8:9) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |,|
]


var singleLineObjectNoCommas = { abc: 'def' ghi: 'jkl'}
//@[4:28) [no-unused-vars (Warning)] Variable "singleLineObjectNoCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singleLineObjectNoCommas|
//@[44:44) [BCP235 (Error)] Expected a new line or comma character at this location. (CodeDescription: none) ||
var multiLineObjectMultipleCommas = {
//@[4:33) [no-unused-vars (Warning)] Variable "multiLineObjectMultipleCommas" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |multiLineObjectMultipleCommas|
  abc: 'def',,,
//@[13:14) [BCP022 (Error)] Expected a property name at this location. (CodeDescription: none) |,|
//@[15:15) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
  ghi: 'jkl',,
//@[13:14) [BCP022 (Error)] Expected a property name at this location. (CodeDescription: none) |,|
//@[14:14) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
}

