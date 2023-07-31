using 'main.bicep'
//@[06:18) [BCP258 (Error)] The following parameters are declared in the Bicep file but are missing an assignment in the params file: "foo", "fooObj". (CodeDescription: none) |'main.bicep'|

var foo
//@[04:07) [no-unused-vars (Warning)] Variable "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |foo|
//@[07:07) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

var foo2 =
//@[04:08) [no-unused-vars (Warning)] Variable "foo2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |foo2|
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

var foo3 = asdf
//@[04:08) [no-unused-vars (Warning)] Variable "foo3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |foo3|
//@[11:15) [BCP057 (Error)] The name "asdf" does not exist in the current context. (CodeDescription: none) |asdf|

var foo4 = utcNow()
//@[04:08) [no-unused-vars (Warning)] Variable "foo4" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |foo4|
//@[11:17) [BCP065 (Error)] Function "utcNow" is not valid at this location. It can only be used as a parameter default value. (CodeDescription: none) |utcNow|

