targetScope
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. |targetScope|
//@[11:11) [BCP018 (Error)] Expected the "=" character at this location. ||
//@[11:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// #completionTest(12) -> empty
targetScope 
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. |targetScope|
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. ||
//@[12:12) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// #completionTest(13,14) -> targetScopes
targetScope = 
//@[0:11) [BCP112 (Error)] The "targetScope" cannot be declared multiple times in one file. |targetScope|
//@[14:14) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
