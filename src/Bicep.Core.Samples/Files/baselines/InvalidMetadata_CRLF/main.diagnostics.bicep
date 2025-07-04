// wrong declaration
metadata
//@[08:08) [BCP266 (Error)] Expected a metadata identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP266) ||

// blank identifier name
metadata 
//@[09:09) [BCP266 (Error)] Expected a metadata identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP266) ||

// invalid identifier name
metadata 2
//@[09:10) [BCP266 (Error)] Expected a metadata identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP266) |2|
//@[10:10) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
metadata _2
//@[09:11) [BCP268 (Error)] Invalid identifier: "_2". Metadata identifiers starting with '_' are reserved. Please use a different identifier. (bicep https://aka.ms/bicep/core-diagnostics#BCP268) |_2|
//@[11:11) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// missing value
metadata missingValueAndType = 
//@[31:31) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

metadata missingAssignment 'noAssignmentOperator'
//@[27:49) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |'noAssignmentOperator'|
//@[49:49) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// metadata referencing metadata
metadata myMetadata = 'hello'
var attemptToReferenceMetadata = myMetadata
//@[04:30) [no-unused-vars (Warning)] Variable "attemptToReferenceMetadata" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |attemptToReferenceMetadata|
//@[33:43) [BCP057 (Error)] The name "myMetadata" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |myMetadata|

// two meta blocks with same identifier name
metadata same = 'value1'
//@[09:13) [BCP145 (Error)] Output "same" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |same|
metadata same = 'value2'
//@[09:13) [BCP145 (Error)] Output "same" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP145) |same|

// metadata referencing vars
var testSymbol = 42
metadata test = testSymbol
//@[16:26) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |testSymbol|


// metadata referencing itself
metadata selfRef = selfRef
//@[19:26) [BCP057 (Error)] The name "selfRef" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |selfRef|

// metadata with decorators
@description('this is a description')
//@[01:12) [BCP269 (Error)] Function "description" cannot be used as a metadata decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP269) |description|
metadata decoratedDescription = 'hasDescription'

@secure()
//@[01:07) [BCP269 (Error)] Function "secure" cannot be used as a metadata decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP269) |secure|
metadata secureMetadata = 'notSupported'


