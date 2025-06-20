/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
//@[06:014) [no-unused-params (Warning)] Parameter "myString" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myString|
wrong
//@[00:005) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |wrong|

param myInt int
//@[06:011) [no-unused-params (Warning)] Parameter "myInt" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myInt|
param
//@[05:005) [BCP013 (Error)] Expected a parameter identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP013) ||

param 3
//@[06:007) [BCP013 (Error)] Expected a parameter identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP013) |3|
//@[07:007) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||
param % string
//@[06:007) [BCP013 (Error)] Expected a parameter identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP013) |%|
param % string 3 = 's'
//@[06:007) [BCP013 (Error)] Expected a parameter identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP013) |%|
//@[15:016) [BCP008 (Error)] Expected the "=" token, or a newline at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP008) |3|

param myBool bool
//@[06:012) [no-unused-params (Warning)] Parameter "myBool" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myBool|

param missingType
//@[06:017) [no-unused-params (Warning)] Parameter "missingType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |missingType|
//@[17:017) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// space after identifier #completionTest(32) -> paramTypes
param missingTypeWithSpaceAfter 
//@[06:031) [no-unused-params (Warning)] Parameter "missingTypeWithSpaceAfter" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |missingTypeWithSpaceAfter|
//@[32:032) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// tab after identifier #completionTest(30) -> paramTypes
param missingTypeWithTabAfter	
//@[06:029) [no-unused-params (Warning)] Parameter "missingTypeWithTabAfter" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |missingTypeWithTabAfter|
//@[30:030) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// #completionTest(20) -> paramTypes
param trailingSpace  
//@[06:019) [no-unused-params (Warning)] Parameter "trailingSpace" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |trailingSpace|
//@[21:021) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
param partialType str
//@[06:017) [no-unused-params (Warning)] Parameter "partialType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |partialType|
//@[18:021) [BCP302 (Error)] The name "str" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |str|

param malformedType 44
//@[06:019) [no-unused-params (Warning)] Parameter "malformedType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |malformedType|

// malformed type but type check should still happen
param malformedType2 44 = f
//@[06:020) [no-unused-params (Warning)] Parameter "malformedType2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |malformedType2|
//@[26:027) [BCP057 (Error)] The name "f" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |f|

// malformed type but type check should still happen
@secure('s')
//@[07:012) [BCP071 (Error)] Expected 0 arguments, but got 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('s')|
param malformedModifier 44
//@[06:023) [no-unused-params (Warning)] Parameter "malformedModifier" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |malformedModifier|

param myString2 string = 'string value'
//@[06:015) [no-unused-params (Warning)] Parameter "myString2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myString2|

param wrongDefaultValue string = 42
//@[06:023) [no-unused-params (Warning)] Parameter "wrongDefaultValue" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongDefaultValue|
//@[33:035) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "42". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |42|

param myInt2 int = 42
//@[06:012) [no-unused-params (Warning)] Parameter "myInt2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myInt2|
param noValueAfterColon int =   
//@[06:023) [no-unused-params (Warning)] Parameter "noValueAfterColon" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |noValueAfterColon|
//@[32:032) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

param myTruth bool = 'not a boolean'
//@[06:013) [no-unused-params (Warning)] Parameter "myTruth" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myTruth|
//@[21:036) [BCP033 (Error)] Expected a value of type "bool" but the provided value is of type "'not a boolean'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'not a boolean'|
param myFalsehood bool = 'false'
//@[06:017) [no-unused-params (Warning)] Parameter "myFalsehood" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myFalsehood|
//@[25:032) [BCP033 (Error)] Expected a value of type "bool" but the provided value is of type "'false'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'false'|

param wrongAssignmentToken string: 'hello'
//@[06:026) [no-unused-params (Warning)] Parameter "wrongAssignmentToken" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongAssignmentToken|
//@[33:034) [BCP008 (Error)] Expected the "=" token, or a newline at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP008) |:|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[06:267) [no-unused-params (Warning)] Parameter "WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|
//@[06:267) [BCP024 (Error)] The identifier exceeds the limit of 255. Reduce the length of the identifier. (bicep https://aka.ms/bicep/core-diagnostics#BCP024) |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|

// #completionTest(28,29) -> boolPlusSymbols
param boolCompletions bool = 
//@[06:021) [no-unused-params (Warning)] Parameter "boolCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |boolCompletions|
//@[29:029) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// #completionTest(30,31) -> arrayPlusSymbols
param arrayCompletions array = 
//@[06:022) [no-unused-params (Warning)] Parameter "arrayCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |arrayCompletions|
//@[23:028) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[31:031) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// #completionTest(32,33) -> objectPlusSymbols
param objectCompletions object = 
//@[06:023) [no-unused-params (Warning)] Parameter "objectCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |objectCompletions|
//@[24:030) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[33:033) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[36:037) [BCP019 (Error)] Expected a new line character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP019) |s|
//@[45:046) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |'|

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[35:037) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". (bicep https://aka.ms/bicep/core-diagnostics#BCP006) |\s|

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[30:046) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |'what\'s up doc?|

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[41:041) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
//@[41:041) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||
param wrongType fluffyBunny = 'what\'s ${up
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[43:043) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||
param wrongType fluffyBunny = 'what\'s ${up}
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[43:044) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |}|
param wrongType fluffyBunny = 'what\'s ${'up
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[41:044) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |'up|
//@[44:044) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[46:046) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
//@[46:046) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||
param wrongType fluffyBunny = 'what\'s ${'up${
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[46:046) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||
//@[46:046) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[49:049) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[49:050) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |}|
//@[50:050) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[51:051) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[51:053) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |}?|

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|
//@[33:034) [BCP087 (Error)] Array and object literals are not allowed here. (bicep https://aka.ms/bicep/core-diagnostics#BCP087) |{|
//@[53:054) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |'|
//@[54:054) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) ||

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[06:027) [no-unused-params (Warning)] Parameter "badInterpolatedString" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |badInterpolatedString|
//@[46:049) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |}!'|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[06:028) [no-unused-params (Warning)] Parameter "badInterpolatedString2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |badInterpolatedString2|
//@[49:052) [BCP064 (Error)] Found unexpected tokens in interpolated expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP064) |b c|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongType|
//@[16:027) [BCP302 (Error)] The name "fluffyBunny" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |fluffyBunny|

// modifier on an invalid type
@minLength(3)
@maxLength(24)
param someArray arra
//@[06:015) [no-unused-params (Warning)] Parameter "someArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |someArray|
//@[16:020) [BCP302 (Error)] The name "arra" is not a valid type. Please specify one of the following types: "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP302) |arra|

@secure()
//@[00:009) [BCP124 (Error)] The decorator "secure" can only be attached to targets of type "object | string", but the target has type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP124) |@secure()|
@minLength(3)
//@[00:013) [BCP124 (Error)] The decorator "minLength" can only be attached to targets of type "array | string", but the target has type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP124) |@minLength(3)|
@maxLength(123)
//@[00:015) [BCP124 (Error)] The decorator "maxLength" can only be attached to targets of type "array | string", but the target has type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP124) |@maxLength(123)|
param secureInt int
//@[06:015) [no-unused-params (Warning)] Parameter "secureInt" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |secureInt|

// wrong modifier value types
@allowed([
  'test'
//@[02:008) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'test'". (bicep https://aka.ms/bicep/core-diagnostics#BCP034) |'test'|
  true
//@[02:006) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP034) |true|
])
@minValue({
//@[10:013) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |{\n}|
})
@maxValue([
//@[10:013) [BCP070 (Error)] Argument of type "<empty array>" is not assignable to parameter of type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |[\n]|
])
@metadata('wrong')
//@[10:017) [BCP070 (Error)] Argument of type "'wrong'" is not assignable to parameter of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP070) |'wrong'|
param wrongIntModifier int = true
//@[06:022) [no-unused-params (Warning)] Parameter "wrongIntModifier" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongIntModifier|
//@[29:033) [BCP033 (Error)] Expected a value of type "int" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |true|

@metadata(any([]))
//@[10:017) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |any([])|
@allowed(any(2))
//@[09:015) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |any(2)|
param fatalErrorInIssue1713
//@[06:027) [no-unused-params (Warning)] Parameter "fatalErrorInIssue1713" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |fatalErrorInIssue1713|
//@[27:027) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) ||

// wrong metadata schema
@metadata({
  description: true
//@[15:019) [BCP036 (Error)] The property "description" expected a value of type "string" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP036) |true|
})
param wrongMetadataSchema string
//@[06:025) [no-unused-params (Warning)] Parameter "wrongMetadataSchema" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |wrongMetadataSchema|

// expression in modifier
@maxLength(a + 2)
//@[11:012) [BCP057 (Error)] The name "a" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |a|
@minLength(foo())
//@[11:014) [BCP057 (Error)] The name "foo" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |foo|
@allowed([
  i
//@[02:003) [BCP057 (Error)] The name "i" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |i|
])
param expressionInModifier string = 2 + 3
//@[06:026) [no-unused-params (Warning)] Parameter "expressionInModifier" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |expressionInModifier|
//@[36:041) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "5". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |2 + 3|

@maxLength(2 + 3)
//@[11:016) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |2 + 3|
@minLength(length([]))
//@[11:021) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |length([])|
@allowed([
  resourceGroup().id
//@[02:020) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |resourceGroup().id|
])
param nonCompileTimeConstant string
//@[06:028) [no-unused-params (Warning)] Parameter "nonCompileTimeConstant" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |nonCompileTimeConstant|


@allowed([])
//@[09:011) [BCP099 (Error)] The "allowed" array must contain one or more items. (bicep https://aka.ms/bicep/core-diagnostics#BCP099) |[]|
param emptyAllowedString string
//@[06:024) [no-unused-params (Warning)] Parameter "emptyAllowedString" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |emptyAllowedString|

@allowed([])
//@[09:011) [BCP099 (Error)] The "allowed" array must contain one or more items. (bicep https://aka.ms/bicep/core-diagnostics#BCP099) |[]|
param emptyAllowedInt int
//@[06:021) [no-unused-params (Warning)] Parameter "emptyAllowedInt" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |emptyAllowedInt|

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[36:056) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP079) |paramDefaultOneCycle|

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[37:058) [BCP080 (Error)] The expression is involved in a cycle ("paramDefaultTwoCycle2" -> "paramDefaultTwoCycle1"). (bicep https://aka.ms/bicep/core-diagnostics#BCP080) |paramDefaultTwoCycle2|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[37:058) [BCP080 (Error)] The expression is involved in a cycle ("paramDefaultTwoCycle1" -> "paramDefaultTwoCycle2"). (bicep https://aka.ms/bicep/core-diagnostics#BCP080) |paramDefaultTwoCycle1|

@allowed([
  paramModifierSelfCycle
//@[02:024) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (bicep https://aka.ms/bicep/core-diagnostics#BCP079) |paramModifierSelfCycle|
])
param paramModifierSelfCycle string

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[24:055) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'Microsoft.Foo/foos@2020-02-02'|
  name: 'foo'
}
output sampleOutput string = 'hello'

param paramAccessingVar string = concat(sampleVar, 's')
//@[33:055) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(sampleVar, 's')|
//@[40:049) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. (bicep https://aka.ms/bicep/core-diagnostics#BCP072) |sampleVar|

param paramAccessingResource string = sampleResource
//@[06:028) [no-unused-params (Warning)] Parameter "paramAccessingResource" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |paramAccessingResource|
//@[38:052) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "Microsoft.Foo/foos@2020-02-02". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |sampleResource|
//@[38:052) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. (bicep https://aka.ms/bicep/core-diagnostics#BCP072) |sampleResource|

param paramAccessingOutput string = sampleOutput
//@[06:026) [no-unused-params (Warning)] Parameter "paramAccessingOutput" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |paramAccessingOutput|
//@[36:048) [BCP057 (Error)] The name "sampleOutput" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |sampleOutput|

// #completionTest(6) -> empty
param 
//@[06:006) [BCP013 (Error)] Expected a parameter identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP013) ||

// #completionTest(46,47) -> justSymbols
param defaultValueOneLinerCompletions string = 
//@[06:037) [no-unused-params (Warning)] Parameter "defaultValueOneLinerCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |defaultValueOneLinerCompletions|
//@[47:047) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// invalid comma separator (array)
@metadata({
  description: 'Name of Virtual Machine'
})
@allowed([
  'abc',
//@[08:008) [BCP238 (Error)] Unexpected new line character after a comma. (bicep https://aka.ms/bicep/core-diagnostics#BCP238) ||
  'def'
])
param commaOne string
//@[06:014) [no-unused-params (Warning)] Parameter "commaOne" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |commaOne|

@secure
//@[01:007) [BCP063 (Error)] The name "secure" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |secure|
@
//@[01:001) [BCP123 (Error)] Expected a namespace or decorator name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP123) ||
@&& xxx
//@[01:003) [BCP123 (Error)] Expected a namespace or decorator name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP123) |&&|
@sys
//@[01:004) [BCP141 (Error)] The expression cannot be used as a decorator as it is not callable. (bicep https://aka.ms/bicep/core-diagnostics#BCP141) |sys|
@paramAccessingVar
//@[01:018) [BCP141 (Error)] The expression cannot be used as a decorator as it is not callable. (bicep https://aka.ms/bicep/core-diagnostics#BCP141) |paramAccessingVar|
param incompleteDecorators string
//@[06:026) [no-unused-params (Warning)] Parameter "incompleteDecorators" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |incompleteDecorators|

@concat(1, 2)
//@[01:007) [BCP152 (Error)] Function "concat" cannot be used as a decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP152) |concat|
@sys.concat('a', 'b')
//@[05:011) [BCP152 (Error)] Function "concat" cannot be used as a decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP152) |concat|
@secure()
// wrong target type
@minValue(20)
//@[00:013) [BCP124 (Error)] The decorator "minValue" can only be attached to targets of type "int", but the target has type "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP124) |@minValue(20)|
param someString string
//@[06:016) [no-unused-params (Warning)] Parameter "someString" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |someString|

@allowed([
    true
//@[04:008) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP034) |true|
    10
    'foo'
//@[04:009) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'foo'". (bicep https://aka.ms/bicep/core-diagnostics#BCP034) |'foo'|
])
@secure()
//@[00:009) [BCP124 (Error)] The decorator "secure" can only be attached to targets of type "object | string", but the target has type "int". (bicep https://aka.ms/bicep/core-diagnostics#BCP124) |@secure()|
// #completionTest(1, 2, 3) -> intParameterDecoratorsPlusNamespace
@  
//@[03:003) [BCP123 (Error)] Expected a namespace or decorator name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP123) ||
// #completionTest(5, 6) -> intParameterDecorators
@sys.   
//@[08:008) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||
param someInteger int = 20
//@[06:017) [no-unused-params (Warning)] Parameter "someInteger" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |someInteger|
//@[22:026) [secure-parameter-default (Warning)] Secure parameters should not have hardcoded defaults (except for empty or newGuid()). (bicep core linter https://aka.ms/bicep/linter-diagnostics#secure-parameter-default) |= 20|

@allowed([], [], 2)
//@[08:019) [BCP071 (Error)] Expected 1 argument, but got 3. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |([], [], 2)|
// #completionTest(4) -> empty
@az.
//@[04:004) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||
param tooManyArguments1 int = 20
//@[06:023) [no-unused-params (Warning)] Parameter "tooManyArguments1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |tooManyArguments1|

@metadata({}, {}, true)
//@[09:023) [BCP071 (Error)] Expected 1 argument, but got 3. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |({}, {}, true)|
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
@m
//@[01:002) [BCP057 (Error)] The name "m" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |m|
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
@   
//@[04:004) [BCP123 (Error)] Expected a namespace or decorator name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP123) ||
// #completionTest(5) -> stringParameterDecorators
@sys.
//@[05:005) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||
param tooManyArguments2 string
//@[06:023) [no-unused-params (Warning)] Parameter "tooManyArguments2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |tooManyArguments2|

@description(sys.concat(2))
//@[13:026) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |sys.concat(2)|
@allowed([for thing in []: 's'])
//@[09:031) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |[for thing in []: 's']|
//@[10:013) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
param nonConstantInDecorator string
//@[06:028) [no-unused-params (Warning)] Parameter "nonConstantInDecorator" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |nonConstantInDecorator|

@minValue(-length('s'))
//@[11:022) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |length('s')|
@metadata({
  bool: !true
//@[08:013) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |!true|
})
param unaryMinusOnFunction int
//@[06:026) [no-unused-params (Warning)] Parameter "unaryMinusOnFunction" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |unaryMinusOnFunction|

@minLength(1)
//@[00:013) [BCP166 (Error)] Duplicate "minLength" decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP166) |@minLength(1)|
@minLength(2)
//@[00:013) [BCP166 (Error)] Duplicate "minLength" decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP166) |@minLength(2)|
@secure()
@maxLength(3)
//@[00:013) [BCP166 (Error)] Duplicate "maxLength" decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP166) |@maxLength(3)|
@maxLength(4)
//@[00:013) [BCP166 (Error)] Duplicate "maxLength" decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP166) |@maxLength(4)|
param duplicateDecorators string
//@[06:025) [no-unused-params (Warning)] Parameter "duplicateDecorators" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |duplicateDecorators|

@maxLength(-1)
//@[11:013) [BCP168 (Error)] Length must not be a negative value. (bicep https://aka.ms/bicep/core-diagnostics#BCP168) |-1|
@minLength(-100)
//@[11:015) [BCP168 (Error)] Length must not be a negative value. (bicep https://aka.ms/bicep/core-diagnostics#BCP168) |-100|
param invalidLength string
//@[06:019) [no-unused-params (Warning)] Parameter "invalidLength" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |invalidLength|

@allowed([
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ApiManagement/service'
	'Microsoft.Network/applicationGateways'
	'Microsoft.Automation/automationAccounts'
	'Microsoft.ContainerInstance/containerGroups'
	'Microsoft.ContainerRegistry/registries'
	'Microsoft.ContainerService/managedClusters'
])
param invalidPermutation array = [
//@[06:024) [no-unused-params (Warning)] Parameter "invalidPermutation" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |invalidPermutation|
//@[25:030) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
	'foobar'
//@[01:009) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "'foobar'". (bicep https://aka.ms/bicep/core-diagnostics#BCP034) |'foobar'|
	true
//@[01:005) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP034) |true|
    100
//@[04:007) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "100". (bicep https://aka.ms/bicep/core-diagnostics#BCP034) |100|
]

@allowed([
	[
		'Microsoft.AnalysisServices/servers'
		'Microsoft.ApiManagement/service'
	]
	[
		'Microsoft.Network/applicationGateways'
		'Microsoft.Automation/automationAccounts'
	]
])
param invalidDefaultWithAllowedArrayDecorator array = true
//@[06:045) [no-unused-params (Warning)] Parameter "invalidDefaultWithAllowedArrayDecorator" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |invalidDefaultWithAllowedArrayDecorator|
//@[46:051) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
//@[54:058) [BCP033 (Error)] Expected a value of type "['Microsoft.AnalysisServices/servers', 'Microsoft.ApiManagement/service'] | ['Microsoft.Network/applicationGateways', 'Microsoft.Automation/automationAccounts']" but the provided value is of type "true". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |true|

// unterminated multi-line comment
/*    
//@[00:007) [BCP002 (Error)] The multi-line comment at this location is not terminated. Terminate it with the */ character sequence. (bicep https://aka.ms/bicep/core-diagnostics#BCP002) |/*    \n|

