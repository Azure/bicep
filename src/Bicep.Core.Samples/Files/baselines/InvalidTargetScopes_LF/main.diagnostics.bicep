targetScope
//@[00:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (bicep https://aka.ms/bicep/core-diagnostics#BCP112) |targetScope|
//@[11:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
//@[11:11) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(12) -> empty
targetScope 
//@[00:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (bicep https://aka.ms/bicep/core-diagnostics#BCP112) |targetScope|
//@[12:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(13,14) -> targetScopes
targetScope = 
//@[00:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (bicep https://aka.ms/bicep/core-diagnostics#BCP112) |targetScope|
//@[14:14) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||


targetScope = 'asdfds'
//@[00:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (bicep https://aka.ms/bicep/core-diagnostics#BCP112) |targetScope|
//@[14:22) [BCP033 (Error)] Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "'asdfds'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'asdfds'|

targetScope = { }
//@[00:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (bicep https://aka.ms/bicep/core-diagnostics#BCP112) |targetScope|
//@[14:17) [BCP033 (Error)] Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |{ }|

targetScope = true
//@[00:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. (bicep https://aka.ms/bicep/core-diagnostics#BCP112) |targetScope|
//@[14:18) [BCP033 (Error)] Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |true|
