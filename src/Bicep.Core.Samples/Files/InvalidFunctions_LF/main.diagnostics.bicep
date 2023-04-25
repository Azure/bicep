func useRuntimeFunction = () string => reference('foo').bar
//@[39:55) [BCP341 (Error)] This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment. (CodeDescription: none) |reference('foo')|

func constFunc = () string => 'A'
func funcWithOtherFuncRef = () string => constFunc()
//@[41:50) [BCP057 (Error)] The name "constFunc" does not exist in the current context. (CodeDescription: none) |constFunc|

func invalidType = (input string) string => input

output invalidType string = invalidType(true)
//@[40:44) [BCP070 (Error)] Argument of type "true" is not assignable to parameter of type "string". (CodeDescription: none) |true|

func madeUpTypeArgs = (a notAType, b alsoNotAType) string => '${a}-${b}'
//@[25:33) [BCP302 (Error)] The name "notAType" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |notAType|
//@[37:49) [BCP302 (Error)] The name "alsoNotAType" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |alsoNotAType|
//@[64:65) [BCP062 (Error)] The referenced declaration with name "a" is not valid. (CodeDescription: none) |a|
//@[69:70) [BCP062 (Error)] The referenced declaration with name "b" is not valid. (CodeDescription: none) |b|

func noLambda = ('foo') string => ''
//@[17:17) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) ||
//@[17:22) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) |'foo'|
//@[17:22) [BCP283 (Error)] Using a literal value as a type requires enabling EXPERIMENTAL feature "UserDefinedTypes". (CodeDescription: none) |'foo'|

func noLambda2 = (sdf 'foo') string => ''
//@[18:27) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) |sdf 'foo'|
//@[22:27) [BCP283 (Error)] Using a literal value as a type requires enabling EXPERIMENTAL feature "UserDefinedTypes". (CodeDescription: none) |'foo'|

func noLambda3 = string 'asdf'
//@[17:23) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |string|

func argLengthMismatch = (a string, b string, c string) array => ([a, b, c])
var sdf = argLengthMismatch('asdf')
//@[04:07) [no-unused-vars (Warning)] Variable "sdf" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sdf|
//@[27:35) [BCP071 (Error)] Expected 3 arguments, but got 1. (CodeDescription: none) |('asdf')|

var asdfwdf = noLambda('asd')
//@[04:11) [no-unused-vars (Warning)] Variable "asdfwdf" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |asdfwdf|
//@[23:28) [BCP070 (Error)] Argument of type "'asd'" is not assignable to parameter of type "error". (CodeDescription: none) |'asd'|

func sayHello = (name string) string => 'Hi ${name}!'
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[46:54) [BCP063 (Error)] The name "sayHello" is not a parameter, variable, resource or module. (CodeDescription: none) |sayHello|

func sayHelloBadNewlines = (
//@[28:28) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[28:28) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
//@[28:28) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
//@[28:28) [BCP018 (Error)] Expected the "=>" character at this location. (CodeDescription: none) ||
//@[28:28) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) ||
  name string) string => 'Hi ${name}!'
//@[02:06) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |name|

