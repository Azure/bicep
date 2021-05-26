/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
//@[6:14) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |myString|
wrong
//@[0:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |wrong|

param myInt int
//@[6:11) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |myInt|
param
//@[5:5) [BCP013 (Error)] Expected a parameter identifier at this location. ||
//@[5:5) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params ||

param 3
//@[6:7) [BCP013 (Error)] Expected a parameter identifier at this location. |3|
//@[6:7) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |3|
//@[7:7) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||
param % string
//@[6:7) [BCP013 (Error)] Expected a parameter identifier at this location. |%|
//@[6:7) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |%|
param % string 3 = 's'
//@[6:7) [BCP013 (Error)] Expected a parameter identifier at this location. |%|
//@[6:7) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |%|
//@[15:16) [BCP008 (Error)] Expected the "=" token, or a newline at this location. |3|

param myBool bool
//@[6:12) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |myBool|

param missingType
//@[6:17) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |missingType|
//@[17:17) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// space after identifier #completionTest(32) -> paramTypes
param missingTypeWithSpaceAfter 
//@[6:31) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |missingTypeWithSpaceAfter|
//@[32:32) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// tab after identifier #completionTest(30) -> paramTypes
param missingTypeWithTabAfter	
//@[6:29) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |missingTypeWithTabAfter|
//@[30:30) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// #completionTest(20) -> paramTypes
param trailingSpace  
//@[6:19) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |trailingSpace|
//@[21:21) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
param partialType str
//@[6:17) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |partialType|
//@[18:21) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |str|

param malformedType 44
//@[6:19) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |malformedType|
//@[20:22) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |44|

// malformed type but type check should still happen
param malformedType2 44 = f
//@[6:20) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |malformedType2|
//@[21:23) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |44|
//@[26:27) [BCP057 (Error)] The name "f" does not exist in the current context. |f|

// malformed type but type check should still happen
@secure('s')
//@[7:12) [BCP071 (Error)] Expected 0 arguments, but got 1. |('s')|
param malformedModifier 44
//@[6:23) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |malformedModifier|
//@[24:26) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |44|

param myString2 string = 'string value'
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |myString2|

param wrongDefaultValue string = 42
//@[6:23) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongDefaultValue|
//@[33:35) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "int". |42|

param myInt2 int = 42
//@[6:12) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |myInt2|
param noValueAfterColon int =   
//@[6:23) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |noValueAfterColon|
//@[32:32) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

param myTruth bool = 'not a boolean'
//@[6:13) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |myTruth|
//@[21:36) [BCP027 (Error)] The parameter expects a default value of type "bool" but provided value is of type "'not a boolean'". |'not a boolean'|
param myFalsehood bool = 'false'
//@[6:17) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |myFalsehood|
//@[25:32) [BCP027 (Error)] The parameter expects a default value of type "bool" but provided value is of type "'false'". |'false'|

param wrongAssignmentToken string: 'hello'
//@[6:26) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongAssignmentToken|
//@[33:34) [BCP008 (Error)] Expected the "=" token, or a newline at this location. |:|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[6:267) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|
//@[6:267) [BCP024 (Error)] The identifier exceeds the limit of 255. Reduce the length of the identifier. |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|

// #completionTest(28,29) -> boolPlusSymbols
param boolCompletions bool = 
//@[6:21) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |boolCompletions|
//@[29:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// #completionTest(30,31) -> arrayPlusSymbols
param arrayCompletions array = 
//@[6:22) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |arrayCompletions|
//@[31:31) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// #completionTest(32,33) -> objectPlusSymbols
param objectCompletions object = 
//@[6:23) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |objectCompletions|
//@[33:33) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[36:37) [BCP019 (Error)] Expected a new line character at this location. |s|
//@[45:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'|

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[35:37) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". |\s|

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[30:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'what\'s up doc?|

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[41:41) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[41:41) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${up
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[43:43) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${up}
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[43:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}|
param wrongType fluffyBunny = 'what\'s ${'up
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[41:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'up|
//@[44:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[46:46) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[46:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[46:46) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[46:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[49:49) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[49:50) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}|
//@[50:50) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[51:51) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[51:53) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}?|

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[33:34) [BCP087 (Error)] Array and object literals are not allowed here. |{|
//@[53:54) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'|
//@[54:54) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[6:27) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |badInterpolatedString|
//@[46:49) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |}!'|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[6:28) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |badInterpolatedString2|
//@[49:52) [BCP064 (Error)] Found unexpected tokens in interpolated expression. |b c|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|

// modifier on an invalid type
@minLength(3)
@maxLength(24)
param someArray arra
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |someArray|
//@[16:20) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |arra|

@secure()
//@[0:9) [BCP124 (Error)] The decorator "secure" can only be attached to targets of type "object | string", but the target has type "int". |@secure()|
@minLength(3)
//@[0:13) [BCP124 (Error)] The decorator "minLength" can only be attached to targets of type "array | string", but the target has type "int". |@minLength(3)|
@maxLength(123)
//@[0:15) [BCP124 (Error)] The decorator "maxLength" can only be attached to targets of type "array | string", but the target has type "int". |@maxLength(123)|
param secureInt int
//@[6:15) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |secureInt|

// wrong modifier value types
@allowed([
  'test'
//@[2:8) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'test'". |'test'|
  true
//@[2:6) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "bool". |true|
])
@minValue({
//@[10:13) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "int". |{\n}|
})
@maxValue([
//@[10:13) [BCP070 (Error)] Argument of type "array" is not assignable to parameter of type "int". |[\n]|
])
@metadata('wrong')
//@[10:17) [BCP070 (Error)] Argument of type "'wrong'" is not assignable to parameter of type "object". |'wrong'|
param wrongIntModifier int = true
//@[6:22) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongIntModifier|
//@[29:33) [BCP027 (Error)] The parameter expects a default value of type "int" but provided value is of type "bool". |true|

@metadata(any([]))
//@[10:17) [BCP032 (Error)] The value must be a compile-time constant. |any([])|
@allowed(any(2))
//@[9:15) [BCP032 (Error)] The value must be a compile-time constant. |any(2)|
param fatalErrorInIssue1713
//@[6:27) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |fatalErrorInIssue1713|
//@[27:27) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// wrong metadata schema
@metadata({
  description: true
//@[15:19) [BCP036 (Error)] The property "description" expected a value of type "string" but the provided value is of type "bool". |true|
})
param wrongMetadataSchema string
//@[6:25) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |wrongMetadataSchema|

// expression in modifier
@maxLength(a + 2)
//@[11:12) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
@minLength(foo())
//@[11:14) [BCP057 (Error)] The name "foo" does not exist in the current context. |foo|
@allowed([
  i
//@[2:3) [BCP057 (Error)] The name "i" does not exist in the current context. |i|
])
param expressionInModifier string = 2 + 3
//@[6:26) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |expressionInModifier|
//@[36:41) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "int". |2 + 3|

@maxLength(2 + 3)
//@[11:16) [BCP032 (Error)] The value must be a compile-time constant. |2 + 3|
@minLength(length([]))
//@[11:21) [BCP032 (Error)] The value must be a compile-time constant. |length([])|
@allowed([
  resourceGroup().id
//@[2:20) [BCP032 (Error)] The value must be a compile-time constant. |resourceGroup().id|
])
param nonCompileTimeConstant string
//@[6:28) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |nonCompileTimeConstant|


@allowed([])
//@[9:11) [BCP099 (Error)] The "allowed" array must contain one or more items. |[]|
param emptyAllowedString string
//@[6:24) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |emptyAllowedString|

@allowed([])
//@[9:11) [BCP099 (Error)] The "allowed" array must contain one or more items. |[]|
param emptyAllowedInt int
//@[6:21) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |emptyAllowedInt|

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[36:56) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramDefaultOneCycle|

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[37:58) [BCP080 (Error)] The expression is involved in a cycle ("paramDefaultTwoCycle2" -> "paramDefaultTwoCycle1"). |paramDefaultTwoCycle2|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[37:58) [BCP080 (Error)] The expression is involved in a cycle ("paramDefaultTwoCycle1" -> "paramDefaultTwoCycle2"). |paramDefaultTwoCycle1|

@allowed([
  paramModifierSelfCycle
//@[2:24) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramModifierSelfCycle|
])
param paramModifierSelfCycle string

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[24:55) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02" does not have types available. |'Microsoft.Foo/foos@2020-02-02'|
  name: 'foo'
}
output sampleOutput string = 'hello'

param paramAccessingVar string = concat(sampleVar, 's')
//@[33:55) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function.\nSee https://aka.ms/bicep/linter/prefer-interpolation |concat(sampleVar, 's')|
//@[40:49) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleVar|

param paramAccessingResource string = sampleResource
//@[6:28) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |paramAccessingResource|
//@[38:52) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "Microsoft.Foo/foos@2020-02-02". |sampleResource|
//@[38:52) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleResource|

param paramAccessingOutput string = sampleOutput
//@[6:26) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |paramAccessingOutput|
//@[36:48) [BCP058 (Error)] The name "sampleOutput" is an output. Outputs cannot be referenced in expressions. |sampleOutput|

// #completionTest(6) -> empty
param 
//@[6:6) [BCP013 (Error)] Expected a parameter identifier at this location. ||
//@[6:6) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params ||

// #completionTest(46,47) -> justSymbols
param defaultValueOneLinerCompletions string = 
//@[6:37) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |defaultValueOneLinerCompletions|
//@[47:47) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

// invalid comma separator (array)
@metadata({
  description: 'Name of Virtual Machine'
})
@allowed([
  'abc',
//@[7:8) [BCP106 (Error)] Expected a new line character at this location. Commas are not used as separator delimiters. |,|
  'def'
])
param commaOne string
//@[6:14) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |commaOne|

@secure
//@[1:7) [BCP063 (Error)] The name "secure" is not a parameter, variable, resource or module. |secure|
@
//@[1:1) [BCP123 (Error)] Expected a namespace or decorator name at this location. ||
@&& xxx
//@[1:3) [BCP123 (Error)] Expected a namespace or decorator name at this location. |&&|
@sys
//@[1:4) [BCP141 (Error)] The expression cannot be used as a decorator as it is not callable. |sys|
@paramAccessingVar
//@[1:18) [BCP141 (Error)] The expression cannot be used as a decorator as it is not callable. |paramAccessingVar|
param incompleteDecorators string
//@[6:26) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |incompleteDecorators|

@concat(1, 2)
//@[1:7) [BCP152 (Error)] Function "concat" cannot be used as a decorator. |concat|
//@[1:13) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function.\nSee https://aka.ms/bicep/linter/prefer-interpolation |concat(1, 2)|
@sys.concat('a', 'b')
//@[5:11) [BCP152 (Error)] Function "concat" cannot be used as a decorator. |concat|
@secure()
// wrong target type
@minValue(20)
//@[0:13) [BCP124 (Error)] The decorator "minValue" can only be attached to targets of type "int", but the target has type "string". |@minValue(20)|
param someString string
//@[6:16) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |someString|

@allowed([
    true
//@[4:8) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "bool". |true|
    10
    'foo'
//@[4:9) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'foo'". |'foo'|
])
@secure()
//@[0:9) [BCP124 (Error)] The decorator "secure" can only be attached to targets of type "object | string", but the target has type "int". |@secure()|
// #completionTest(1, 2, 3) -> intParameterDecoratorsPlusNamespace
@  
//@[3:3) [BCP123 (Error)] Expected a namespace or decorator name at this location. ||
// #completionTest(5, 6) -> intParameterDecorators
@sys.   
//@[8:8) [BCP020 (Error)] Expected a function or property name at this location. ||
param someInteger int = 20
//@[6:17) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |someInteger|
//@[22:26) [secure-parameter-default (Warning)] Secure parameters should not have hardcoded defaults (except for empty or newGuid()).\nSee https://aka.ms/bicep/linter/secure-parameter-default |= 20|

@allowed([], [], 2)
//@[8:19) [BCP071 (Error)] Expected 1 argument, but got 3. |([], [], 2)|
// #completionTest(4) -> empty
@az.
//@[4:4) [BCP020 (Error)] Expected a function or property name at this location. ||
param tooManyArguments1 int = 20
//@[6:23) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |tooManyArguments1|

@metadata({}, {}, true)
//@[9:23) [BCP071 (Error)] Expected 1 argument, but got 3. |({}, {}, true)|
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
@m
//@[1:2) [BCP057 (Error)] The name "m" does not exist in the current context. |m|
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
@   
//@[4:4) [BCP123 (Error)] Expected a namespace or decorator name at this location. ||
// #completionTest(5) -> stringParameterDecorators
@sys.
//@[5:5) [BCP020 (Error)] Expected a function or property name at this location. ||
param tooManyArguments2 string
//@[6:23) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |tooManyArguments2|

@description(sys.concat(2))
//@[13:26) [BCP032 (Error)] The value must be a compile-time constant. |sys.concat(2)|
@allowed([for thing in []: 's'])
//@[9:31) [BCP032 (Error)] The value must be a compile-time constant. |[for thing in []: 's']|
//@[10:13) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. |for|
param nonConstantInDecorator string
//@[6:28) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |nonConstantInDecorator|

@minValue(-length('s'))
//@[11:22) [BCP032 (Error)] The value must be a compile-time constant. |length('s')|
@metadata({
  bool: !true
//@[8:13) [BCP032 (Error)] The value must be a compile-time constant. |!true|
//@[8:13) [BCP032 (Error)] The value must be a compile-time constant. |!true|
})
param unaryMinusOnFunction int
//@[6:26) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |unaryMinusOnFunction|

@minLength(1)
//@[0:13) [BCP166 (Error)] Duplicate "minLength" decorator. |@minLength(1)|
@minLength(2)
//@[0:13) [BCP166 (Error)] Duplicate "minLength" decorator. |@minLength(2)|
@secure()
@maxLength(3)
//@[0:13) [BCP166 (Error)] Duplicate "maxLength" decorator. |@maxLength(3)|
@maxLength(4)
//@[0:13) [BCP166 (Error)] Duplicate "maxLength" decorator. |@maxLength(4)|
param duplicateDecorators string
//@[6:25) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |duplicateDecorators|

@minLength(-1)
//@[11:13) [BCP168 (Error)] Length must not be a negative value. |-1|
@maxLength(-100)
//@[11:15) [BCP168 (Error)] Length must not be a negative value. |-100|
param invalidLength string
//@[6:19) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |invalidLength|

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
//@[6:24) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |invalidPermutation|
	'foobar'
//@[1:9) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "'foobar'". |'foobar'|
	true
//@[1:5) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "bool". |true|
    100
//@[4:7) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "int". |100|
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
//@[6:45) [no-unused-params (Warning)] Parameter is declared but never used.\nSee https://aka.ms/bicep/linter/no-unused-params |invalidDefaultWithAllowedArrayDecorator|
//@[54:58) [BCP027 (Error)] The parameter expects a default value of type "array" but provided value is of type "bool". |true|

// unterminated multi-line comment
/*    
//@[0:7) [BCP002 (Error)] The multi-line comment at this location is not terminated. Terminate it with the */ character sequence. |/*    \n|

