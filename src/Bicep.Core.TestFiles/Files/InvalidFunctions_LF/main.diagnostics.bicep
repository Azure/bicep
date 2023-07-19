func useRuntimeFunction() string => reference('foo').bar
//@[36:52) [BCP341 (Error)] This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment. (CodeDescription: none) |reference('foo')|

func constFunc() string => 'A'
func funcWithOtherFuncRef() string => constFunc()
//@[38:47) [BCP057 (Error)] The name "constFunc" does not exist in the current context. (CodeDescription: none) |constFunc|

func missingArgType(input) string => input
//@[20:25) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) |input|
//@[25:26) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |)|

func missingOutputType(input string) => input
//@[37:39) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |=>|
//@[37:45) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) |=> input|
//@[45:45) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[45:45) [BCP018 (Error)] Expected the "=>" character at this location. (CodeDescription: none) ||

func invalidType(input string) string => input

output invalidType string = invalidType(true)
//@[40:44) [BCP070 (Error)] Argument of type "true" is not assignable to parameter of type "string". (CodeDescription: none) |true|

func madeUpTypeArgs(a notAType, b alsoNotAType) string => '${a}-${b}'
//@[22:30) [BCP302 (Error)] The name "notAType" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "string", "validStringLiteralUnion". (CodeDescription: none) |notAType|
//@[34:46) [BCP302 (Error)] The name "alsoNotAType" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "string", "validStringLiteralUnion". (CodeDescription: none) |alsoNotAType|
//@[61:62) [BCP062 (Error)] The referenced declaration with name "a" is not valid. (CodeDescription: none) |a|
//@[66:67) [BCP062 (Error)] The referenced declaration with name "b" is not valid. (CodeDescription: none) |b|

func noLambda('foo') string => ''
//@[14:14) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) ||
//@[14:19) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) |'foo'|

func noLambda2 = (sdf 'foo') string => ''
//@[15:16) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |=|

func noLambda3 = string 'asdf'
//@[15:16) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |=|

func argLengthMismatch(a string, b string, c string) array => ([a, b, c])
var sdf = argLengthMismatch('asdf')
//@[04:07) [no-unused-vars (Warning)] Variable "sdf" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sdf|
//@[27:35) [BCP071 (Error)] Expected 3 arguments, but got 1. (CodeDescription: none) |('asdf')|

var asdfwdf = noLambda('asd')
//@[04:11) [no-unused-vars (Warning)] Variable "asdfwdf" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |asdfwdf|
//@[23:28) [BCP070 (Error)] Argument of type "'asd'" is not assignable to parameter of type "'foo'". (CodeDescription: none) |'asd'|

func sayHello(name string) string => 'Hi ${name}!'
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[46:54) [BCP063 (Error)] The name "sayHello" is not a parameter, variable, resource or module. (CodeDescription: none) |sayHello|

func sayHelloBadNewlines(
//@[25:25) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[25:25) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
//@[25:25) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
//@[25:25) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) ||
//@[25:25) [BCP018 (Error)] Expected the ")" character at this location. (CodeDescription: none) ||
//@[25:25) [BCP018 (Error)] Expected the "=>" character at this location. (CodeDescription: none) ||
//@[25:25) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) ||
//@[25:25) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) ||
  name string) string => 'Hi ${name}!'
//@[02:06) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |name|

type validStringLiteralUnion = 'foo'|'bar'|'baz'
func invalidArgs(a validStringLiteralUnion, b string) string => a
//@[17:42) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) |a validStringLiteralUnion|
func invalidOutput() validStringLiteralUnion => 'foo'
//@[21:44) [BCP342 (Error)] User-defined types are not supported in user-defined function parameters or outputs. (CodeDescription: none) |validStringLiteralUnion|

