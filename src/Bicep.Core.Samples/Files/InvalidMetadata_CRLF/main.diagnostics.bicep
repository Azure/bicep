// wrong declaration
bad
//@[00:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |bad|

// blank identifier name
meta 
//@[05:05) [BCP250 (Error)] Expected a metadata identifier at this location. (CodeDescription: none) ||

// invalid identifier name
meta 2
//@[05:06) [BCP250 (Error)] Expected a metadata identifier at this location. (CodeDescription: none) |2|
//@[06:06) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
meta _2
//@[05:07) [BCP239 (Error)] Identifier "_2" is a reserved Bicep symbol name and cannot be used in this context. (CodeDescription: none) |_2|
//@[07:07) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// missing value
meta missingValueAndType = 
//@[27:27) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

meta missingAssignment 'noAssingmentOperator'
//@[23:45) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |'noAssingmentOperator'|
//@[45:45) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// metadata referencing metadata
meta myMeta = 'hello'
var attemptToReferenceMetadata = myMeta
//@[04:30) [no-unused-vars (Warning)] Variable "attemptToReferenceMetadata" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |attemptToReferenceMetadata|
//@[33:39) [BCP063 (Error)] The name "myMeta" is not a parameter, variable, resource or module. (CodeDescription: none) |myMeta|

// two meta blocks with same identifier name
meta same = 'value1'
//@[05:09) [BCP145 (Error)] Output "same" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |same|
meta same = 'value2'
//@[05:09) [BCP145 (Error)] Output "same" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |same|

// metadata referencing vars
var testSymbol = 42
meta test = testSymbol
//@[12:22) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |testSymbol|


// metadata referencing itself
meta selfRef = selfRef
//@[15:22) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |selfRef|

// metadata with decorators
@description('this is a description')
//@[01:37) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |description('this is a description')|
meta decoratedDescription = 'hasDescription'

@secure()
//@[01:09) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |secure()|
meta secureMeta = 'notSupported'


