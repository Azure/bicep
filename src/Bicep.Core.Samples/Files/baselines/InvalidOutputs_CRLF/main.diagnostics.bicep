
// wrong declaration
bad
//@[00:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |bad|

// incomplete #completionTest(7) -> empty
output 
//@[07:07) [BCP016 (Error)] Expected an output identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP016) ||

var testSymbol = 42
//@[04:14) [no-unused-vars (Warning)] Variable "testSymbol" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |testSymbol|

// #completionTest(28,29) -> symbols
output missingValueAndType = 
//@[27:28) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) |=|
//@[29:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// #completionTest(28,29) -> symbols
output missingValue string = 
//@[29:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// #completionTest(31,32) -> arrayPlusSymbols
output arrayCompletions array = 
//@[24:29) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[32:32) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// #completionTest(33,34) -> objectPlusSymbols
output objectCompletions object = 
//@[25:31) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[34:34) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// #completionTest(29,30) -> boolPlusSymbols
output boolCompletions bool = 
//@[30:30) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

output foo
//@[07:10) [BCP145 (Error)] Output "foo" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |foo|
//@[10:10) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// space after identifier #completionTest(20) -> outputTypes
output spaceAfterId 
//@[20:20) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// #completionTest(25) -> outputTypes
output spacesAfterCursor  
//@[26:26) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
output partialType obj
//@[19:22) [BCP302 (Error)] The name "obj" is not a valid type. Please specify one of the following types: "any", "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |obj|
//@[22:22) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// malformed identifier
output 2
//@[07:08) [BCP016 (Error)] Expected an output identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP016) |2|
//@[08:08) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// malformed type
output malformedType 3
//@[22:22) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// malformed type but type check should still happen
output malformedType2 3 = 2 + null
//@[26:34) [BCP045 (Error)] Cannot apply operator "+" to operands of type "2" and "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |2 + null|

// malformed type assignment
output malformedAssignment 2 = 2

// malformed type before assignment
output lol 2 = true
//@[15:19) [BCP033 (Error)] Expected a value of type "2" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |true|

// wrong type + missing value
output foo fluffy
//@[07:10) [BCP145 (Error)] Output "foo" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |foo|
//@[11:17) [BCP302 (Error)] The name "fluffy" is not a valid type. Please specify one of the following types: "any", "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffy|
//@[17:17) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// missing value
output foo string
//@[07:10) [BCP145 (Error)] Output "foo" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |foo|
//@[17:17) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// missing value
output foo string =
//@[07:10) [BCP145 (Error)] Output "foo" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |foo|
//@[19:19) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// wrong string output values
output str string = true
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |str|
//@[20:24) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |true|
output str string = false
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |str|
//@[20:25) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "false". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |false|
output str string = [
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |str|
//@[20:24) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |[\r\n]|
]
output str string = {
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |str|
//@[20:24) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |{\r\n}|
}
output str string = 52
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |str|
//@[20:22) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "52". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |52|

// wrong int output values
output i int = true
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |i|
//@[15:19) [BCP033 (Error)] Expected a value of type "int" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |true|
output i int = false
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |i|
//@[15:20) [BCP033 (Error)] Expected a value of type "int" but the provided value is of type "false". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |false|
output i int = [
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |i|
//@[15:19) [BCP033 (Error)] Expected a value of type "int" but the provided value is of type "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |[\r\n]|
]
output i int = }
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |i|
//@[15:16) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |}|
}
//@[00:01) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |}|
output i int = 'test'
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |i|
//@[15:21) [BCP033 (Error)] Expected a value of type "int" but the provided value is of type "'test'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'test'|

// wrong bool output values
output b bool = [
//@[07:08) [BCP145 (Error)] Output "b" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |b|
//@[16:20) [BCP033 (Error)] Expected a value of type "bool" but the provided value is of type "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |[\r\n]|
]
output b bool = {
//@[07:08) [BCP145 (Error)] Output "b" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |b|
//@[16:20) [BCP033 (Error)] Expected a value of type "bool" but the provided value is of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |{\r\n}|
}
output b bool = 32
//@[07:08) [BCP145 (Error)] Output "b" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |b|
//@[16:18) [BCP033 (Error)] Expected a value of type "bool" but the provided value is of type "32". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |32|
output b bool = 'str'
//@[07:08) [BCP145 (Error)] Output "b" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |b|
//@[16:21) [BCP033 (Error)] Expected a value of type "bool" but the provided value is of type "'str'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'str'|

// wrong array output values
output arr array = 32
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |arr|
//@[11:16) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[19:21) [BCP033 (Error)] Expected a value of type "array" but the provided value is of type "32". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |32|
output arr array = true
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |arr|
//@[11:16) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[19:23) [BCP033 (Error)] Expected a value of type "array" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |true|
output arr array = false
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |arr|
//@[11:16) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[19:24) [BCP033 (Error)] Expected a value of type "array" but the provided value is of type "false". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |false|
output arr array = {
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |arr|
//@[11:16) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[19:23) [BCP033 (Error)] Expected a value of type "array" but the provided value is of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |{\r\n}|
}
output arr array = 'str'
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |arr|
//@[11:16) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[19:24) [BCP033 (Error)] Expected a value of type "array" but the provided value is of type "'str'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'str'|

// wrong object output values
output o object = 32
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |o|
//@[09:15) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[18:20) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "32". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |32|
output o object = true
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |o|
//@[09:15) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[18:22) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |true|
output o object = false
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |o|
//@[09:15) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[18:23) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "false". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |false|
output o object = [
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |o|
//@[09:15) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[18:22) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |[\r\n]|
]
output o object = 'str'
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |o|
//@[09:15) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[18:23) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "'str'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'str'|

// a few expression cases
output exp string = 2 + 3
//@[20:25) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "5". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |2 + 3|
output union string = true ? 's' : 1
output bad int = true && !4
//@[25:27) [BCP044 (Error)] Cannot apply operator "!" to operand of type "4". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!4|
output deeper bool = true ? -true : (14 && 's') + 10
//@[28:33) [BCP044 (Error)] Cannot apply operator "-" to operand of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |-true|
//@[37:46) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "14" and "'s'". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |14 && 's'|

output myOutput string = 'hello'
var attemptToReferenceAnOutput = myOutput
//@[04:30) [no-unused-vars (Warning)] Variable "attemptToReferenceAnOutput" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |attemptToReferenceAnOutput|
//@[33:41) [BCP057 (Error)] The name "myOutput" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |myOutput|

@sys.maxValue(20)
@minValue(10)
output notAttachableDecorators int = 32
//@[37:39) [BCP327 (Error)] The provided value (which will always be greater than or equal to 32) is too large to assign to a target for which the maximum allowable value is 20. (bicep https://aka.ms/bicep/core-diagnostics#BCP327) |32|

// nested loops inside output loops are not supported
output noNestedLoops array = [for thing in things: {
//@[21:26) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[43:49) [BCP057 (Error)] The name "things" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |things|
  something: [
    [for thing in things: true]
//@[05:08) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
//@[18:24) [BCP057 (Error)] The name "things" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |things|
  ]
}]

// loops in inner properties inside outputs are not supported
output noInnerLoopsInOutputs object = {
//@[29:35) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
  a: [for i in range(0,10): i]
//@[06:09) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
}
output noInnerLoopsInOutputs2 object = {
//@[30:36) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
  a: [for i in range(0,10): {
//@[06:09) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
    b: [for j in range(0,10): i+j]
//@[08:11) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
  }]
}

//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

output keyVaultSecretOutput string = kv.getSecret('mySecret')
//@[37:61) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
output keyVaultSecretInterpolatedOutput string = '${kv.getSecret('mySecret')}'
//@[49:78) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#simplify-interpolation) |'${kv.getSecret('mySecret')}'|
//@[52:76) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
output keyVaultSecretObjectOutput object = {
//@[34:40) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
  secret: kv.getSecret('mySecret')
//@[10:34) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
}
output keyVaultSecretArrayOutput array = [
//@[33:38) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
  kv.getSecret('mySecret')
//@[02:26) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
]
output keyVaultSecretArrayInterpolatedOutput array = [
//@[45:50) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
  '${kv.getSecret('mySecret')}'
//@[05:29) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
]

// WARNING!!!!! dangling decorators

// #completionTest(1) -> decoratorsPlusNamespace
@
//@[01:01) [BCP123 (Error)] Expected a namespace or decorator name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP123) ||
// #completionTest(5) -> decorators
@sys.
//@[05:05) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||

// WARNING!!!!! dangling decorators - to make sure the tests work, please do not add contents after this line 
