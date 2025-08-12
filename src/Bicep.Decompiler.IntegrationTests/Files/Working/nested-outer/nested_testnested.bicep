param variables_outer ? /* TODO: fill in correct type */
//@[22:23) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) |?|

var test = variables_outer
var test2 = test
//@[04:09) [no-unused-vars (Warning)] Variable "test2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |test2|

