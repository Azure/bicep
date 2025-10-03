func useRuntimeFunction() string => reference('foo').bar
//@[36:52) [BCP341 (Error)] This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment. (bicep https://aka.ms/bicep/core-diagnostics#BCP341) |reference('foo')|

func missingArgType(input) string => input
//@[25:26) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "any", "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) |)|

func missingOutputType(input string) => input
//@[37:39) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "any", "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) |=>|
//@[45:45) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
//@[45:45) [BCP018 (Error)] Expected the "=>" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

func invalidType(input string) string => input

output invalidType string = invalidType(true)
//@[40:44) [BCP070 (Error)] Argument of type "true" is not assignable to parameter of type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |true|

func madeUpTypeArgs(a notAType, b alsoNotAType) string => '${a}-${b}'
//@[61:62) [BCP062 (Error)] The referenced declaration with name "a" is not valid. (bicep https://aka.ms/bicep/core-diagnostics#BCP062) |a|
//@[66:67) [BCP062 (Error)] The referenced declaration with name "b" is not valid. (bicep https://aka.ms/bicep/core-diagnostics#BCP062) |b|

func noLambda('foo') string => ''
//@[14:14) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) ||

func noLambda2 = (sdf 'foo') string => ''
//@[15:16) [BCP018 (Error)] Expected the "(" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |=|

func noLambda3 = string 'asdf'
//@[15:16) [BCP018 (Error)] Expected the "(" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |=|

func argLengthMismatch(a string, b string, c string) array => ([a, b, c])
//@[53:58) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
var sdf = argLengthMismatch('asdf')
//@[04:07) [no-unused-vars (Warning)] Variable "sdf" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |sdf|
//@[27:35) [BCP071 (Error)] Expected 3 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('asdf')|

var asdfwdf = noLambda('asd')
//@[04:11) [no-unused-vars (Warning)] Variable "asdfwdf" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |asdfwdf|
//@[23:28) [BCP070 (Error)] Argument of type "'asd'" is not assignable to parameter of type "'foo'". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'asd'|

func sayHello(name string) string => 'Hi ${name}!'
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[14:19) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[46:54) [BCP063 (Error)] The name "sayHello" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |sayHello|

func sayHelloBadNewlines(
  name string) string => 'Hi ${name}!'

type validStringLiteralUnion = 'foo'|'bar'|'baz'
func invalidArgs(a validStringLiteralUnion, b string) string => a
func invalidOutput() validStringLiteralUnion => 'foo'

func recursive() string => recursive()
//@[27:38) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP079) |recursive()|

func recursiveA() string => recursiveB()
//@[28:40) [BCP080 (Error)] The expression is involved in a cycle ("recursiveB" -> "recursiveA"). (bicep https://aka.ms/bicep/core-diagnostics#BCP080) |recursiveB()|
func recursiveB() string => recursiveA()
//@[28:40) [BCP080 (Error)] The expression is involved in a cycle ("recursiveA" -> "recursiveB"). (bicep https://aka.ms/bicep/core-diagnostics#BCP080) |recursiveA()|

func onlyComma(,) string => 'foo'
//@[15:15) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) ||
//@[15:16) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "any", "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) |,|
func trailingCommas(a string,,) string => 'foo'
//@[29:29) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) ||
//@[29:30) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "any", "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) |,|
func multiLineOnly(
  a string
  b string) string => 'foo'
//@[02:03) [BCP018 (Error)] Expected the ")" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |b|
//@[27:27) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
//@[27:27) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "any", "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||
//@[27:27) [BCP018 (Error)] Expected the "=>" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

func multiLineTrailingCommas(
  a string,
  ,) string => 'foo'
//@[02:02) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) ||
//@[02:03) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "any", "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) |,|

func lineBeforeComma(
  a string
  ,b string) string => 'foo'

