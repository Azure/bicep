param variables_outer ? /* TODO: fill in correct type */
//@[22:23) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |?|

var test = variables_outer
var test2 = test
//@[04:09) [no-unused-vars (Warning)] Variable "test2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |test2|
