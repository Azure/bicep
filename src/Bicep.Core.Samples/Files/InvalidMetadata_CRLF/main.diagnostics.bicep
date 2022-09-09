// wrong declaration
metadata
//@[08:08) [BCP266 (Error)] Expected a metadata identifier at this location. (CodeDescription: none) ||

// blank identifier name
metadata 
//@[09:09) [BCP266 (Error)] Expected a metadata identifier at this location. (CodeDescription: none) ||

// invalid identifier name
metadata 2
//@[09:10) [BCP266 (Error)] Expected a metadata identifier at this location. (CodeDescription: none) |2|
//@[10:10) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
metadata _2
//@[09:11) [BCP268 (Error)] Invalid identifier: "_2". Metadata identifiers starting with '_' are reserved. Please use a different identifier. (CodeDescription: none) |_2|
//@[11:11) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// missing value
metadata missingValueAndType = 
//@[31:31) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

metadata missingAssignment 'noAssingmentOperator'
//@[27:49) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |'noAssingmentOperator'|
//@[49:49) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// metadata referencing metadata
metadata myMetadata = 'hello'
var attemptToReferenceMetadata = myMetadata
//@[04:30) [no-unused-vars (Warning)] Variable "attemptToReferenceMetadata" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |attemptToReferenceMetadata|
//@[33:43) [BCP063 (Error)] The name "myMetadata" is not a parameter, variable, resource or module. (CodeDescription: none) |myMetadata|

// two meta blocks with same identifier name
metadata same = 'value1'
//@[09:13) [BCP145 (Error)] Output "same" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |same|
metadata same = 'value2'
//@[09:13) [BCP145 (Error)] Output "same" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |same|

// metadata referencing vars
var testSymbol = 42
metadata test = testSymbol
//@[16:26) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |testSymbol|


// metadata referencing itself
metadata selfRef = selfRef
//@[19:26) [BCP063 (Error)] The name "selfRef" is not a parameter, variable, resource or module. (CodeDescription: none) |selfRef|

// metadata with decorators
@description('this is a description')
metadata decoratedDescription = 'hasDescription'

@secure()
//@[01:07) [BCP269 (Error)] Function "secure" cannot be used as a metadata decorator. (CodeDescription: none) |secure|
metadata secureMetadata = 'notSupported'


