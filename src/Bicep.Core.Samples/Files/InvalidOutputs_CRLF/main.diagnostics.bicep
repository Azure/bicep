
// wrong declaration
bad
//@[00:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |bad|

// incomplete #completionTest(7) -> empty
output 
//@[07:07) [BCP016 (Error)] Expected an output identifier at this location. (CodeDescription: none) ||

var testSymbol = 42
//@[04:14) [no-unused-vars (Warning)] Variable "testSymbol" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testSymbol|

// #completionTest(28,29) -> symbols
output missingValueAndType = 
//@[27:28) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |=|
//@[29:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// #completionTest(28,29) -> symbols
output missingValue string = 
//@[29:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// #completionTest(31,32) -> arrayPlusSymbols
output arrayCompletions array = 
//@[32:32) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// #completionTest(33,34) -> objectPlusSymbols
output objectCompletions object = 
//@[34:34) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// #completionTest(29,30) -> boolPlusSymbols
output boolCompletions bool = 
//@[30:30) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

output foo
//@[07:10) [BCP145 (Error)] Output "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[10:10) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// space after identifier #completionTest(20) -> outputTypes
output spaceAfterId 
//@[20:20) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// #completionTest(25) -> outputTypes
output spacesAfterCursor  
//@[26:26) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
output partialType obj
//@[19:22) [BCP030 (Error)] The output type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |obj|
//@[22:22) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// malformed identifier
output 2
//@[07:08) [BCP016 (Error)] Expected an output identifier at this location. (CodeDescription: none) |2|
//@[08:08) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// malformed type
output malformedType 3
//@[21:22) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |3|
//@[22:22) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// malformed type but type check should still happen
output malformedType2 3 = 2 + null
//@[22:23) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |3|
//@[26:34) [BCP045 (Error)] Cannot apply operator "+" to operands of type "int" and "null". (CodeDescription: none) |2 + null|

// malformed type assignment
output malformedAssignment 2 = 2
//@[27:28) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |2|

// malformed type before assignment
output lol 2 = true
//@[11:12) [BCP146 (Error)] Expected an output type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |2|

// wrong type + missing value
output foo fluffy
//@[07:10) [BCP145 (Error)] Output "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[11:17) [BCP030 (Error)] The output type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffy|
//@[17:17) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// missing value
output foo string
//@[07:10) [BCP145 (Error)] Output "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[17:17) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// missing value
output foo string =
//@[07:10) [BCP145 (Error)] Output "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[19:19) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// wrong string output values
output str string = true
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |str|
//@[20:24) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "bool". (CodeDescription: none) |true|
output str string = false
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |str|
//@[20:25) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "bool". (CodeDescription: none) |false|
output str string = [
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |str|
//@[20:24) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "array". (CodeDescription: none) |[\r\n]|
]
output str string = {
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |str|
//@[20:24) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "object". (CodeDescription: none) |{\r\n}|
}
output str string = 52
//@[07:10) [BCP145 (Error)] Output "str" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |str|
//@[20:22) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "int". (CodeDescription: none) |52|

// wrong int output values
output i int = true
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |i|
//@[15:19) [BCP026 (Error)] The output expects a value of type "int" but the provided value is of type "bool". (CodeDescription: none) |true|
output i int = false
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |i|
//@[15:20) [BCP026 (Error)] The output expects a value of type "int" but the provided value is of type "bool". (CodeDescription: none) |false|
output i int = [
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |i|
//@[15:19) [BCP026 (Error)] The output expects a value of type "int" but the provided value is of type "array". (CodeDescription: none) |[\r\n]|
]
output i int = }
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |i|
//@[15:16) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |}|
}
//@[00:01) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
output i int = 'test'
//@[07:08) [BCP145 (Error)] Output "i" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |i|
//@[15:21) [BCP026 (Error)] The output expects a value of type "int" but the provided value is of type "'test'". (CodeDescription: none) |'test'|

// wrong bool output values
output b bool = [
//@[07:08) [BCP145 (Error)] Output "b" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |b|
//@[16:20) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "array". (CodeDescription: none) |[\r\n]|
]
output b bool = {
//@[07:08) [BCP145 (Error)] Output "b" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |b|
//@[16:20) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "object". (CodeDescription: none) |{\r\n}|
}
output b bool = 32
//@[07:08) [BCP145 (Error)] Output "b" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |b|
//@[16:18) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "int". (CodeDescription: none) |32|
output b bool = 'str'
//@[07:08) [BCP145 (Error)] Output "b" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |b|
//@[16:21) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "'str'". (CodeDescription: none) |'str'|

// wrong array output values
output arr array = 32
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |arr|
//@[19:21) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "int". (CodeDescription: none) |32|
output arr array = true
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |arr|
//@[19:23) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "bool". (CodeDescription: none) |true|
output arr array = false
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |arr|
//@[19:24) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "bool". (CodeDescription: none) |false|
output arr array = {
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |arr|
//@[19:23) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "object". (CodeDescription: none) |{\r\n}|
}
output arr array = 'str'
//@[07:10) [BCP145 (Error)] Output "arr" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |arr|
//@[19:24) [BCP026 (Error)] The output expects a value of type "array" but the provided value is of type "'str'". (CodeDescription: none) |'str'|

// wrong object output values
output o object = 32
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |o|
//@[18:20) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "int". (CodeDescription: none) |32|
output o object = true
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |o|
//@[18:22) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "bool". (CodeDescription: none) |true|
output o object = false
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |o|
//@[18:23) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "bool". (CodeDescription: none) |false|
output o object = [
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |o|
//@[18:22) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "array". (CodeDescription: none) |[\r\n]|
]
output o object = 'str'
//@[07:08) [BCP145 (Error)] Output "o" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |o|
//@[18:23) [BCP026 (Error)] The output expects a value of type "object" but the provided value is of type "'str'". (CodeDescription: none) |'str'|

// a few expression cases
output exp string = 2 + 3
//@[20:25) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "int". (CodeDescription: none) |2 + 3|
output union string = true ? 's' : 1
//@[22:36) [BCP026 (Error)] The output expects a value of type "string" but the provided value is of type "'s' | int". (CodeDescription: none) |true ? 's' : 1|
output bad int = true && !4
//@[25:27) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!4|
output deeper bool = true ? -true : (14 && 's') + 10
//@[28:33) [BCP044 (Error)] Cannot apply operator "-" to operand of type "bool". (CodeDescription: none) |-true|
//@[37:46) [BCP045 (Error)] Cannot apply operator "&&" to operands of type "int" and "'s'". (CodeDescription: none) |14 && 's'|

output myOutput string = 'hello'
var attemptToReferenceAnOutput = myOutput
//@[04:30) [no-unused-vars (Warning)] Variable "attemptToReferenceAnOutput" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |attemptToReferenceAnOutput|
//@[33:41) [BCP058 (Error)] The name "myOutput" is an output. Outputs cannot be referenced in expressions. (CodeDescription: none) |myOutput|

@sys.maxValue(20)
//@[05:13) [BCP129 (Error)] Function "maxValue" cannot be used as an output decorator. (CodeDescription: none) |maxValue|
@minValue(10)
//@[01:09) [BCP129 (Error)] Function "minValue" cannot be used as an output decorator. (CodeDescription: none) |minValue|
output notAttachableDecorators int = 32

// nested loops inside output loops are not supported
output noNestedLoops array = [for thing in things: {
//@[43:49) [BCP057 (Error)] The name "things" does not exist in the current context. (CodeDescription: none) |things|
  something: [
    [for thing in things: true]
//@[05:08) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
//@[18:24) [BCP057 (Error)] The name "things" does not exist in the current context. (CodeDescription: none) |things|
  ]
}]

// loops in inner properties inside outputs are not supported
output noInnerLoopsInOutputs object = {
  a: [for i in range(0,10): i]
//@[06:09) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
}
output noInnerLoopsInOutputs2 object = {
  a: [for i in range(0,10): {
//@[06:09) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
    b: [for j in range(0,10): i+j]
//@[08:11) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
  }]
}

//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

output keyVaultSecretOutput string = kv.getSecret('mySecret')
//@[37:61) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
output keyVaultSecretInterpolatedOutput string = '${kv.getSecret('mySecret')}'
//@[49:78) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/simplify-interpolation)) |'${kv.getSecret('mySecret')}'|
//@[52:76) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
output keyVaultSecretObjectOutput object = {
  secret: kv.getSecret('mySecret')
//@[10:34) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
}
output keyVaultSecretArrayOutput array = [
  kv.getSecret('mySecret')
//@[02:26) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
]
output keyVaultSecretArrayInterpolatedOutput array = [
  '${kv.getSecret('mySecret')}'
//@[05:29) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
]

// WARNING!!!!! dangling decorators

// #completionTest(1) -> decoratorsPlusNamespace
@
//@[01:01) [BCP123 (Error)] Expected a namespace or decorator name at this location. (CodeDescription: none) ||
// #completionTest(5) -> decorators
@sys.
//@[05:05) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// WARNING!!!!! dangling decorators - to make sure the tests work, please do not add contents after this line 
