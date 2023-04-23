func useRuntimeFunction = () => reference('foo').bar

func constFunc = () => 'A'
func funcWithOtherFuncRef = () => constFunc()
//@[34:43) [BCP057 (Error)] The name "constFunc" does not exist in the current context. (CodeDescription: none) |constFunc|

func invalidType = (string input) => input

output invalidType string = invalidType(true)
//@[40:44) [BCP070 (Error)] Argument of type "true" is not assignable to parameter of type "string". (CodeDescription: none) |true|

func madeUpTypeArgs = (notAType a, alsoNotAType b) => '${a}-${b}'
//@[23:31) [BCP302 (Error)] The name "notAType" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |notAType|
//@[35:47) [BCP302 (Error)] The name "alsoNotAType" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |alsoNotAType|
//@[57:58) [BCP062 (Error)] The referenced declaration with name "a" is not valid. (CodeDescription: none) |a|
//@[62:63) [BCP062 (Error)] The referenced declaration with name "b" is not valid. (CodeDescription: none) |b|

func noLambda = ('foo') => ''
//@[17:22) [BCP283 (Error)] Using a literal value as a type requires enabling EXPERIMENTAL feature "UserDefinedTypes". (CodeDescription: none) |'foo'|
//@[22:22) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) ||

func noLambda2 = ('foo' sdf) => ''
//@[18:23) [BCP283 (Error)] Using a literal value as a type requires enabling EXPERIMENTAL feature "UserDefinedTypes". (CodeDescription: none) |'foo'|

func noLambda3 = 'asdf'
//@[17:23) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |'asdf'|

func argLengthMismatch = (string a, string b, string c) => [a, b, c]
//@[59:68) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "[string, string, string]". (CodeDescription: none) |[a, b, c]|
var sdf = argLengthMismatch('asdf')
//@[04:07) [no-unused-vars (Warning)] Variable "sdf" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sdf|
//@[27:35) [BCP071 (Error)] Expected 3 arguments, but got 1. (CodeDescription: none) |('asdf')|

var asdfwdf = noLambda('asd')
//@[04:11) [no-unused-vars (Warning)] Variable "asdfwdf" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |asdfwdf|
//@[23:28) [BCP070 (Error)] Argument of type "'asd'" is not assignable to parameter of type "error". (CodeDescription: none) |'asd'|

func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo array = buildUrl(true, 'google.com', 'search')
//@[19:57) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "string". (CodeDescription: none) |buildUrl(true, 'google.com', 'search')|

func sayHello = (string name) => 'Hi ${name}!'
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[46:54) [BCP063 (Error)] The name "sayHello" is not a parameter, variable, resource or module. (CodeDescription: none) |sayHello|

func sayHelloBadNewlines = (
//@[28:28) [BCP018 (Error)] Expected the "=>" character at this location. (CodeDescription: none) ||
  string name) => 'Hi ${name}!'
//@[02:08) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |string|

