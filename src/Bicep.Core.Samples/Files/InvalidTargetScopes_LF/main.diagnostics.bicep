targetScope
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (CodeDescription: none) |targetScope|
//@[11:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[11:11) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(12) -> empty
targetScope 
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (CodeDescription: none) |targetScope|
//@[12:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(13,14) -> targetScopes
targetScope = 
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (CodeDescription: none) |targetScope|
//@[14:14) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||


targetScope = 'asdfds'
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (CodeDescription: none) |targetScope|
//@[14:22) [BCP033 (Error)] Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "'asdfds'". (CodeDescription: none) |'asdfds'|

targetScope = { }
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (CodeDescription: none) |targetScope|
//@[14:17) [BCP033 (Error)] Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "object". (CodeDescription: none) |{ }|

targetScope = true
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (CodeDescription: none) |targetScope|
//@[14:18) [BCP033 (Error)] Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "bool". (CodeDescription: none) |true|
